using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using QandA.Data;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Text.Json;
using QandA.Data.Models;

namespace Core3Api.Controllers;
[ApiController]
[Route("api/[controller]/[action]")]
public class DataController : ControllerBase
{
    private readonly IDataRepository _dataRepository;
    private readonly IQuestionCache _cache;
    // Authentication && Authorization
    private readonly IHttpClientFactory _clientFactory;
    private readonly string _auth0UserInfo;
    // private readonly IHubContext<QuestionsHub> _questionHubContext;
    public DataController(IDataRepository dataRepository, IQuestionCache questionCache, IHttpClientFactory clientFactory, IConfiguration configuration)
    {
        _dataRepository = dataRepository;
        _cache = questionCache;
        // _questionHubContext = questionHubContext;
        _clientFactory = clientFactory;
        _auth0UserInfo = $"{configuration["Auth0:Authority"]}userinfo";

    }

    #region Get Request
    [HttpGet]
    public IEnumerable<QuestionGetManyResponse> GetQuestions()
    {
        return _dataRepository.GetQuestions().GetAwaiter().GetResult();
    }

    [HttpGet]
    [Route("{search?}/{includeAnswers}")]
    public IEnumerable<QuestionGetManyResponse> GetQuestions(string? search, bool includeAnswers)
    {
        if (string.IsNullOrEmpty(search))
        {
            if (includeAnswers)
            {
                return _dataRepository.GetQuestionsWithAnswers();
            }
            else
            {
                return _dataRepository.GetQuestions().GetAwaiter().GetResult();
            }
        }
        else
        {
            return _dataRepository.GetQuestionsBySearch(search).GetAwaiter().GetResult();
        }
    }


    [HttpGet("search")]
    public IEnumerable<QuestionGetManyResponse> GetQuestionsBySearch(string search)
    {
        return _dataRepository.GetQuestionsBySearch(search).GetAwaiter().GetResult();
    }

    [HttpGet]
    [Route("{questionId}")]
    public ActionResult GetQuestion(int questionId)
    {
        var question = _cache.Get(questionId);
        if (question == null)
        {
            question = _dataRepository.GetQuestion(questionId).GetAwaiter().GetResult();
            if (question == null)
            {
                return NotFound();
            }
            _cache.Set(question);
        }
        return Ok(question);
    }
    [HttpGet("unanswered")]
    public IEnumerable<QuestionGetManyResponse>
    GetUnansweredQuestions()
    {
        return _dataRepository.GetUnansweredQuestions().GetAwaiter().GetResult();
    }
    #endregion
    #region Post, Put Request
    [Authorize]
    [HttpPost]
    public ActionResult<QuestionGetSingleResponse> PostQuestion(QuestionPostRequest questionPostRequest)
    {
        var savedQuestion = _dataRepository.PostQuestion(new QuestionPostFullRequest
        {
            Title = questionPostRequest.Title,
            Content = questionPostRequest.Content,
            UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value,
            UserName = GetUserName().GetAwaiter().GetResult(),
            Created = DateTime.UtcNow
        });
        return CreatedAtAction(nameof(GetQuestion), new { questionId = savedQuestion.QuestionId }, savedQuestion);
    }

    [Authorize]
    [HttpPost("answer")]
    public ActionResult<AnswerGetResponse> PostAnswer(AnswerPostRequest answerPostRequest)
    {
        var questionExists = _dataRepository.QuestionExists(answerPostRequest.QuestionId.Value).GetAwaiter().GetResult(); ;
        if (!questionExists)
        {
            return NotFound();
        }
        var savedAnswer = _dataRepository.PostAnswer(new AnswerPostFullRequest()
        {
            QuestionId = answerPostRequest.QuestionId.Value,
            Content = answerPostRequest.Content,
            UserId = "1",
            UserName = "bob.test@test.com",
            Created = DateTime.UtcNow
        });
        // SignalR Hubs
        // _questionHubContext.Clients
        //                     .Group($"Question-{answerPostRequest.QuestionId.Value}")
        //                     .SendAsync("ReceiveQuestion", _dataRepository.GetQuestion(answerPostRequest.QuestionId.Value));
        _cache.Remove(answerPostRequest.QuestionId.Value);
        return savedAnswer;
    }

    [Authorize(Policy = "MustBeQuestionAuthor")]
    [HttpPut("{questionId}")]
    public ActionResult<QuestionGetSingleResponse> PutQuestion(int questionId, QuestionPutRequest questionPutRequest)
    {
        var question = _dataRepository.GetQuestion(questionId).GetAwaiter().GetResult();
        if (question == null)
        {
            return NotFound();
        }
        questionPutRequest.Title = string.IsNullOrEmpty(questionPutRequest.Title) ? question.Title : questionPutRequest.Title;
        questionPutRequest.Content = string.IsNullOrEmpty(questionPutRequest.Content) ? question.Content : questionPutRequest.Content;
        var savedQuestion = _dataRepository.PutQuestion(questionId, questionPutRequest);
        return savedQuestion;
    }

    #endregion
    #region Delete Request


    [Authorize(Policy = "MustBeQuestionAuthor")]
    [HttpDelete("{questionId}")]
    public ActionResult DeleteQuestion(int questionId)
    {
        var question = _dataRepository.GetQuestion(questionId).GetAwaiter().GetResult();
        if (question == null)
        {
            return NotFound();
        }
        _dataRepository.DeleteQuestion(questionId);
        _cache.Remove(questionId);
        return NoContent();
    }
    #endregion
    private async Task<string> GetUserName()
    {
        var request = new HttpRequestMessage(
        HttpMethod.Get,
        _auth0UserInfo);
        request.Headers.Add(
        "Authorization",
        Request.Headers["Authorization"].First());
        var client = _clientFactory.CreateClient();
        var response = await client.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            var jsonContent =
            await response.Content.ReadAsStringAsync();
            var user = JsonSerializer.Deserialize<User>(jsonContent,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            return user.Name;
        }
        else
        {
            return "";
        }
    }

}