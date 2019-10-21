using System;

namespace WeStop.Api.Core
{
    public struct Theme
    {
        public Theme(string name)
        {
            name = name.Trim();
            if (string.IsNullOrEmpty(name))
                throw new Exception("Nome do tema deve ser informado");

            Name = name;
        }

        public string Name { get; private set; }
    }
}