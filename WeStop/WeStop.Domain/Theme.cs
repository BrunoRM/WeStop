namespace WeStop.Domain
{
    public class Theme : Entity
    {
        public Theme(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }
    }
}
