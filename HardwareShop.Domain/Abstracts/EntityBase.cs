namespace HardwareShop.Domain.Abstracts
{
    public abstract class EntityBase
    {
        protected Action<object, string?>? lazyLoader;
        public EntityBase(Action<object, string?> lazyLoader)
        {
            this.lazyLoader = lazyLoader;
        }
        public EntityBase() { }

        private List<DomainEvent> _domainEvents = new();
        public IReadOnlyCollection<DomainEvent> GetDomainEvents() => _domainEvents.AsReadOnly();
        public void AddDomainEvent(DomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }
        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }
    }
}
