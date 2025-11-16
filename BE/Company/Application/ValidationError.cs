using FluentResults;

namespace Application
{
    public class ValidationError : Error
    {
        public ValidationError(string message) : base(message)
        {
        }
    }
}