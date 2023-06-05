using Microsoft.EntityFrameworkCore;
using Models.MyDbContext;
public class DataRepository : IDataRepository
{
    private readonly DataQuery _dataQuery;
    public void DeleteQuestion(int questionId)
    {
        try
        {
            _dataQuery.DataNotReturn($"Question_Delete {questionId}");
        }
        catch (System.Exception)
        {

        }
    }

    public DataRepository(DataQuery dataQuery)
    {
        _dataQuery = dataQuery;
    }
    public async Task<AnswerGetResponse> GetAnswer(int answerId)
    {
        try
        {
            IEnumerable<AnswerGetResponse> responses = await _dataQuery.DataListReturn<AnswerGetResponse>($"Question_GetSingle {answerId}");
            var answer = responses.FirstOrDefault();
            return answer;
        }
        catch (System.Exception)
        {

        }
        return new AnswerGetResponse();
    }

    public async Task<QuestionGetSingleResponse> GetQuestion(int questionId)
    {

        IEnumerable<QuestionGetSingleResponse> responses = await _dataQuery.DataListReturn<QuestionGetSingleResponse>($"Question_GetSingle {questionId}");
        var question = responses.FirstOrDefault();
        if (question != null)
        {
            question.Answers = await _dataQuery.DataListReturn<AnswerGetResponse>($"Answer_Get_ByQuestionId {questionId}");
        }
        return question;
    }

    public async Task<IEnumerable<QuestionGetManyResponse>> GetQuestions()
    {
        try
        {
            IEnumerable<QuestionGetManyResponse> responses = await _dataQuery.DataListReturn<QuestionGetManyResponse>("Question_GetMany");
            return responses;
        }
        catch (System.Exception)
        {

        }
        return new List<QuestionGetManyResponse>();
    }

    public async Task<IEnumerable<QuestionGetManyResponse>> GetQuestionsBySearch(string search)
    {
        try
        {
            IEnumerable<QuestionGetManyResponse> responses = await _dataQuery.DataListReturn<QuestionGetManyResponse>($"Question_GetMany_BySearch '{search}'");
            return responses;
        }
        catch (System.Exception)
        {

        }
        return new List<QuestionGetManyResponse>();
    }

    public async Task<IEnumerable<QuestionGetManyResponse>> GetUnansweredQuestions()
    {
        try
        {
            IEnumerable<QuestionGetManyResponse> responses = await _dataQuery.DataListReturn<QuestionGetManyResponse>($"Question_GetUnanswered");
            return responses;
        }
        catch (System.Exception)
        {

        }
        return new List<QuestionGetManyResponse>();
    }

    public async Task<bool> QuestionExists(int questionId)
    {
        try
        {
            bool exists = await _dataQuery.DataValueReturn<bool>($"Question_Exists {questionId}");
            return exists;
        }
        catch (System.Exception)
        {

        }
        return false;
    }


    public AnswerGetResponse PostAnswer(AnswerPostFullRequest answer)
    {
        try
        {
            IEnumerable<AnswerGetResponse> responses = _dataQuery.DataListReturn<AnswerGetResponse>($"Answer_Post {answer.QuestionId.Value}, '{answer.Content}', '{answer.UserId}', '{answer.UserName}', '{answer.Created}'").GetAwaiter().GetResult();
            return responses.FirstOrDefault();
        }
        catch (System.Exception)
        {

        }
        return new AnswerGetResponse();
    }

    public QuestionGetSingleResponse PostQuestion(QuestionPostFullRequest question)
    {

        int questionId = _dataQuery.DataValueReturn<int>($"Question_Post '{question.Title}', '{question.Content}', '{question.UserId}', '{question.UserName}', '{question.Created}'").GetAwaiter().GetResult();
        var response = GetQuestion(questionId).GetAwaiter().GetResult();
        return response;

    }

    public QuestionGetSingleResponse PutQuestion(int questionId, QuestionPutRequest question)
    {

        _dataQuery.DataNotReturn($"Question_Put '{questionId}', '{question.Title}', '{question.Content}'");
        return GetQuestion(questionId).GetAwaiter().GetResult();

    }


    public IEnumerable<QuestionGetManyResponse> GetQuestionsWithAnswers()
    {
        IEnumerable<QuestionGetManyResponse> questions = _dataQuery.DataListReturn<QuestionGetManyResponse>("Question_GetMany").GetAwaiter().GetResult();
        foreach (var question in questions)
        {
            question.Answers = _dataQuery.DataListReturn<AnswerGetResponse>($"Answer_Get_ByQuestionId {question.QuestionId}").GetAwaiter().GetResult().ToList();
        }
        return questions;
    }
}

