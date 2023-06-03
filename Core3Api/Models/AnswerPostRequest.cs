using System.ComponentModel.DataAnnotations;

public class AnswerPostRequest
{
    // Nếu ko có dấu ? thì mặc định của int là 0 => lách khỏi validation check => sai logic
    // Khi đó truy cập thì phải AnswerPostRequest.QuestionId.Value
    [Required]
    public int? QuestionId { get; set; }
    [Required]
    public string Content { get; set; }
}
