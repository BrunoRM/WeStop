namespace WeStop.Api.Core
{
    public struct Validation
    {
        public Validation(Answer answer)
        {
            Answer = answer;
            Valid = true;
        }

        public Validation(Answer answer, bool valid)
        {
            Answer = answer;
            Valid = valid;
        }

        public Answer Answer { get; set; }
        public bool Valid { get; set; }
    }
}