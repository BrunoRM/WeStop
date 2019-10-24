namespace WeStop.Core
{
    public class Validation
    {
        public Validation(Answer answer, bool valid)
        {
            Answer = answer;
            Valid = valid;
        }

        public Answer Answer { get; set; }
        public bool Valid { get; set; }
    }
}