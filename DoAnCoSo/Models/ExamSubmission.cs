namespace DoAnCoSo.Models
{
    public class ExamSubmission
    {
        public int ExamId { get; set; }
        // Assuming a simple key-value pair for question ID and the selected/entered answer
        public Dictionary<int, string> Answers { get; set; }
    }

}
