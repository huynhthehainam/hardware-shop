namespace HardwareShop.Application.Models
{
    public enum ApplicationErrorType
    {
        Invalid,
        NotPermitted,
        NotFound,
        Existed,
    }
    public class ApplicationError
    {
        public ApplicationErrorType Type { get; set; }
        public string? Message { get; set; }
        public ApplicationError(ApplicationErrorType type, string? message)
        {
            this.Type = type;
            this.Message = message;
        }
        public static ApplicationError CreateInvalidError(string message)
        {
            return new(ApplicationErrorType.Invalid, message);
        }
        public static ApplicationError CreateNotFoundError(string msg)
        {
            return new(ApplicationErrorType.NotFound, msg);
        }
        public static ApplicationError CreateExistedError(string msg)
        {
            return new(ApplicationErrorType.Existed, msg);
        }
        public static ApplicationError CreateNotPermittedError()
        {
            return new(ApplicationErrorType.NotPermitted, "Not permitted");
        }
    }
    public class ApplicationResponse<T>
    {
        public ApplicationError? Error { get; set; }
        public T? Result { get; set; }
        public ApplicationResponse(T result)
        {
            this.Result = result;
        }
        public ApplicationResponse(ApplicationError error)
        {

            Error = error;
        }
        public ApplicationResponse() { }

    }
    public class ApplicationResponse : ApplicationResponse<string>
    {
        public ApplicationResponse() : base("Success")
        {
        }

        public ApplicationResponse(string result) : base(result)
        {
        }

        public ApplicationResponse(ApplicationError error) : base(error)
        {
        }
    }
}