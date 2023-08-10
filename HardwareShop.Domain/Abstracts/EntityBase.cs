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
    }
}
