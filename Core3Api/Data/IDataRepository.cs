public interface IDataRepository
{
    Task<IEnumerable<QuestionGetManyResponse>> GetQuestions();
    Task<IEnumerable<QuestionGetManyResponse>> GetQuestionsBySearch(string search);
    Task<IEnumerable<QuestionGetManyResponse>> GetUnansweredQuestions();
    Task<QuestionGetSingleResponse> GetQuestion(int questionId);
    Task<bool> QuestionExists(int questionId);
    Task<AnswerGetResponse> GetAnswer(int answerId);
    QuestionGetSingleResponse PostQuestion(QuestionPostFullRequest question);
    QuestionGetSingleResponse PutQuestion(int questionId, QuestionPutRequest question);
    void DeleteQuestion(int questionId);
    AnswerGetResponse PostAnswer(AnswerPostFullRequest answer);
}