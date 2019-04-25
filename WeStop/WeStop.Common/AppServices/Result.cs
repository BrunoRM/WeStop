using System.Collections.Generic;

namespace WeStop.Common.AppServices
{
    public class Result<T>
    {
        //public Result(T obj)
        //{
        //    Ok = true;
        //    Data = obj;
        //}

        //public Result(ERRORS error)
        //{
        //    Ok = false;
        //    Error = error.ToString("g");
        //}

        //public Result(ERRORS error, T obj)
        //{
        //    Ok = false;
        //    Error = error.ToString("g");
        //    Data = obj;
        //}

        //public Result(WARNING warning, T obj)
        //{
        //    Ok = true;
        //    Warning = warning.ToString("g");
        //    Data = obj;
        //}

        //public Result(Dictionary<string, ICollection<string>> errors)
        //{
        //    Ok = false;
        //    Error = ERRORS.INVALID_REQUEST.ToString("g");
        //    Errors = errors;
        //}

        public Result(string message)
        {
            Ok = true;
            Message = message;
        }

        public Result(string message, T data)
        {
            Ok = true;
            Message = message;
            Data = data;
        }

        public Result(ERRORS error)
        {
            Ok = false;
            Error = error.ToString("g");
        }

        public Result(Dictionary<string, ICollection<string>> errors)
        {
            Ok = false;
            Error = ERRORS.INVALID_REQUEST.ToString("g");
            Errors = errors;
        }

        public bool Ok { get; private set; }
        public string Message { get; set; }
        public string Error { get; private set; }
        public T Data { get; private set; }
        public Dictionary<string, ICollection<string>> Errors { get; private set; }
    }
}
