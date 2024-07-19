
using MudBlazor;

namespace POS_OS_GG.Helpers
{
    public class ResponseCreator
    {
        private readonly ISnackbar _snackbar;

        public ResponseCreator(ISnackbar snackbar)
        {
            _snackbar = snackbar;
        }

        public RequestResponse<T> Success<T>(T data, string message = "Success", bool notification = true)
        {
            if (notification)
            {
                _snackbar.Add(message, Severity.Success);
            }

            return new RequestResponse<T>(message, data, true);
        }

        public RequestResponse Success(string message = "Success", bool notification = true)
        {
            if (notification)
            {
                _snackbar.Add(message, Severity.Success);
            }

            return new RequestResponse(message, true);
        }

        public RequestResponse<T> Fail<T>(T data, string message = "Fail", bool notification = true)
        {
            if (notification)
            {
                _snackbar.Add(message, Severity.Warning);
            }

            return new RequestResponse<T>(message, data, false);
        }

        public RequestResponse Fail(string message = "Fail", bool notification = true)
        {
            if (notification)
            {
                _snackbar.Add(message, Severity.Warning);
            }

            return new RequestResponse(message, false);
        }

        public RequestResponse NoContent(string message = "No Content", bool notification = true)
        {
            if (notification)
            {
                _snackbar.Add(message, Severity.Info);
            }

            return new RequestResponse(message, false);
        }

        public RequestResponse<T> NoContent<T>(string message = "No Content", bool notification = true)
        {
            if (notification)
            {
                _snackbar.Add(message, Severity.Info);
            }

            return new RequestResponse<T>(message, default, false);
        }

        public RequestResponse<T> ServerError<T>(string message = "Server Error")
        {
            _snackbar.Add(message, Severity.Error);
            return new RequestResponse<T>(message, default, false);
        }


        public RequestResponse ServerError(string message = "Server Error")
        {
            _snackbar.Add(message, Severity.Error);
            return new RequestResponse(message, false);
        }
    }


    public class RequestResponse
    {
        public RequestResponse(string message, bool isSuccess = true)
        {
            IsSuccess = isSuccess;
            Message = message;
        }

        public RequestResponse()
        {

        }
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class RequestResponse<T>
    {
        public RequestResponse(string message, T data, bool isSuccess = true)
        {
            Data = data;
            IsSuccess = isSuccess;
            Message = message;
        }

        public RequestResponse()
        {

        }

        public T Data { get; set; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
