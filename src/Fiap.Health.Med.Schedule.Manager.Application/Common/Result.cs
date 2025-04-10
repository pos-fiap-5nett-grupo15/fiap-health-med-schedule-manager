using System.Net;

namespace Fiap.Health.Med.Schedule.Manager.Application.Common
{
    public class Result
    {
        public bool IsSuccess { get; }
        public HttpStatusCode StatusCode { get; }
        public List<string> Errors { get; } = [];

        protected Result(bool isSuccess, HttpStatusCode statusCode, List<string>? errors = null)
        {
            IsSuccess = isSuccess;
            StatusCode = statusCode;
            if (errors != null) Errors.AddRange(errors);
        }

        public static Result Success(HttpStatusCode statusCode) => new(true, statusCode);
        public static Result Fail(HttpStatusCode statusCode, string error) => new(false, statusCode, [error]);
        public static Result Fail(HttpStatusCode statusCode, List<string> errors) => new(false, statusCode, errors);
    }

    public class Result<T> : Result
    {
        public T? Data { get; }

        private Result(bool isSuccess, HttpStatusCode statusCode, T? data, List<string>? errors = null) : base(isSuccess, statusCode, errors)
        {
            Data = data;
        }

        public static Result<T> Success(HttpStatusCode statusCode, T data) => new(true, statusCode, data);
        public static new Result<T> Fail(HttpStatusCode statusCode, string error) => new(false, statusCode, default, [error]);
        public static new Result<T> Fail(HttpStatusCode statusCode, List<string> errors) => new(false, statusCode, default, errors);
    }
}