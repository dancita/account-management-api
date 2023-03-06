using System.Net;

namespace AccountManagementAPI.Exceptions
{
    public class AccountManagementAPIException : Exception
    {
        public string? Error { get; }

        public int StatusCode { get; }

        public AccountManagementAPIException(string message, string? error = default,
            int statusCode = (int)HttpStatusCode.InternalServerError)
            : base(message)
        {
            Error = error;
            StatusCode = statusCode;
        }
    }
}