using System.Collections.Generic;
using System.Linq;

namespace WeStop.Common.Handlers
{
    public class Response
    {
        private ICollection<ValidationError> _validationErrors;

        public Response()
        {
            Ok = true;
        }

        public Response(string error)
        {
            Ok = false;
            Error = error;
        }

        public Response(ICollection<ValidationError> validationErrors)
        {
            Ok = false;
            _validationErrors = validationErrors;
        }

        public bool Ok { get; private set; }
        public string Error { get; private set; }
        public IReadOnlyCollection<ValidationError> Errors { get { return _validationErrors.ToList(); } }
    }

    public class Response<T>
    {
        private ICollection<ValidationError> _validationErrors;

        public Response(string error)
        {
            Ok = false;
            Error = error;
        }

        public Response(T data)
        {
            Ok = true;
            Data = data;
        }

        public Response(string error, ICollection<ValidationError> validationErrors)
        {
            Ok = false;
            Error = error;
            _validationErrors = validationErrors;
        }

        public bool Ok { get; private set; }
        public string Error { get; private set; }
        public IReadOnlyCollection<ValidationError> Errors { get { return _validationErrors.ToList(); } }
        public T Data { get; private set; }
    }

    public class ValidationError
    {
        public string Field { get; set; }
        public string Msg { get; set; }
    }
}
