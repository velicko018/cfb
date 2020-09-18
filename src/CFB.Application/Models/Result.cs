namespace CFB.Application.Models
{
    public class Result
    {
        public static Result Success = new Result(true);
        public static Result Failure = new Result(false);
        public Result(bool success)
        {
            IsSuccess = success;
        }

        public bool IsSuccess { get; private set; }
    }
}
