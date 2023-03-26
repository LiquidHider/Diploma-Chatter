namespace Chatter.Domain.BusinessLogic.Models
{
    public class ValueServiceResult<T> : ServiceResult
    {
        public T? Value { get; set; }

        public virtual bool IsEmpty => Value is null;
    }
}
