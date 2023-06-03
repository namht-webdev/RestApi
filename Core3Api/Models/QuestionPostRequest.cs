
using System.ComponentModel.DataAnnotations;

public class QuestionPostRequest
{
    [Required]
    [StringLength(100)]
    public string Title { get; set; }
    [Required(ErrorMessage = "Please include some content for the question")]
    public string Content { get; set; }
}