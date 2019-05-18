namespace WeStop.Api.Classes
{
    public class AnswerValidation
    {
        public AnswerValidation(string answer, bool valid)
        {
            Answer = answer;
            Valid = valid;
        }

        public string Answer { get; set; }
        public bool Valid { get; set; }
    }
}