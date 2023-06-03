using Microsoft.AspNetCore.Mvc;

namespace Core3Api.Controllers;
[ApiController]
[Route("api/[controller]/[action]")]
public class DataController : ControllerBase
{
    #region Get Request
    private readonly IDataRepository _dataRepository;
    public DataController(IDataRepository dataRepository)
    {
        _dataRepository = dataRepository;
    }

    [HttpGet]
    public IEnumerable<QuestionGetManyResponse> GetQuestions()
    {
        return _dataRepository.GetQuestions().GetAwaiter().GetResult();
    }

    [HttpGet]
    [Route("{search}")]
    public IEnumerable<QuestionGetManyResponse> GetQuestions(string search)
    {
        if (string.IsNullOrEmpty(search))
        {
            return _dataRepository.GetQuestions().GetAwaiter().GetResult();
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
        var question = _dataRepository.GetQuestion(questionId).GetAwaiter().GetResult();
        if (question == null)
        {
            return NotFound();
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
    #region Post Request
    [HttpPost]
    public ActionResult<QuestionGetSingleResponse> PostQuestion(QuestionPostRequest questionPostRequest)
    {
        var savedQuestion = _dataRepository.PostQuestion(new QuestionPostFullRequest
        {
            Title = questionPostRequest.Title,
            Content = questionPostRequest.Content,
            UserId = "1",
            UserName = "bob.test@test.com",
            Created = DateTime.UtcNow
        });
        return CreatedAtAction(nameof(GetQuestion), new { questionId = savedQuestion.QuestionId }, savedQuestion);
    }

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
        return savedAnswer;
    }

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

    [HttpDelete("{questionId}")]
    public ActionResult DeleteQuestion(int questionId)
    {
        var question = _dataRepository.GetQuestion(questionId).GetAwaiter().GetResult();
        if (question == null)
        {
            return NotFound();
        }
        _dataRepository.DeleteQuestion(questionId);
        return NoContent();
    }
}