namespace Chatter.Email.Common.ServiceResults
{
    public class ValueServiceResult<T> : ServiceResult
    {
        public T? Value { get; set; }

        public virtual bool IsEmpty => Value is null;
    }
}
