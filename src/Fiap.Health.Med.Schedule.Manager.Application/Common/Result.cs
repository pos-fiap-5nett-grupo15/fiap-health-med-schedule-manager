namespace Fiap.Health.Med.Schedule.Manager.Application.Common
{
    public class Result
    {
        public bool Success { get; }
        public List<string> Errors { get; } = new();

        protected Result(bool success, List<string>? errors = null)
        {
            Success = success;
            if (errors != null) Errors.AddRange(errors);
        }

        public static Result Ok() => new Result(true);
        public static Result Fail(string error) => new Result(false, new List<string> { error });
        public static Result Fail(List<string> errors) => new Result(false, errors);
    }

    public class Result<T> : Result
    {
        public T? Data { get; }

        private Result(bool success, T? data, List<string>? errors = null) : base(success, errors)
        {
            Data = data;
        }

        public static Result<T> Ok(T data) => new Result<T>(true, data);
        public static Result<T> Fail(string error) => new Result<T>(false, default, new List<string> { error });
        public static Result<T> Fail(List<string> errors) => new Result<T>(false, default, errors);
    }
}
