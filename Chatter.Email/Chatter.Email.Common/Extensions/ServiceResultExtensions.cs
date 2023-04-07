using Chatter.Email.Common.Enums;
using Chatter.Email.Common.ServiceResults;

namespace Chatter.Email.Common.Extensions
{
    public static class ServiceResultExtensions
    {
        public const string DefaultExceptionMessage = "An unhandled exception occurred";

        public const string DefaultNotFoundMessageTemplate = "No items were found by this key {0}";

        public static TResult WithValue<TResult, TValue>(this TResult serviceResult, TValue value)
            where TResult : ValueServiceResult<TValue>
        {
            if (serviceResult is null) 
            {
                throw new ArgumentNullException(nameof(serviceResult));
            }

            serviceResult.Value = value;

            return serviceResult;
        }

        public static TResult WithBusinessError<TResult>(this TResult serviceResult, string message)
            where TResult : ServiceResult
        {
            return serviceResult.WithError(ErrorType.BusinessError,message);
        }
        public static TResult WithDataNotFoundError<TResult>(this TResult serviceResult, object key)
            where TResult : ServiceResult
        {
            return serviceResult.WithError(ErrorType.NoDataError, string.Format(DefaultNotFoundMessageTemplate, key));
        }

        public static T WithDataError<T>(this T serviceResult, string message) where T : ServiceResult
        {
            return serviceResult.WithError(ErrorType.DataError, message);
        }

        public static T WithException<T>(this T serviceResult, string? message = null) where T : ServiceResult
        {
            return serviceResult.WithError(ErrorType.Exception, message ?? DefaultExceptionMessage);
        }
        public static TResult WithError<TResult>(this TResult serviceResult, ErrorType errorType, string message)
            where TResult : ServiceResult
        {
            if (serviceResult is null) 
            {
                throw new ArgumentNullException(nameof(serviceResult));
            }

            serviceResult.Error = new ErrorModel 
            {
                Message = message,
                Type = errorType
            };

            return serviceResult;
        }
    }
}
