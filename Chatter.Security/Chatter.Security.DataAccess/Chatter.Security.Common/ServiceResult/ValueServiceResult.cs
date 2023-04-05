namespace Chatter.Security.Common
{

    public class ValueServiceResult<T> : ServiceResult
    {
        public T? Value { get; set; }

        public virtual bool IsEmpty => Value is null;
    }
}