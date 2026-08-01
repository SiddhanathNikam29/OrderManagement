namespace OrderManagement.Domain.Common
{
    public abstract class BaseEntity
    {
        public int Id { get; protected set; }

        protected BaseEntity()
        {
            Id = 0;
        }
    }
}