using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HardwareShop.Core.Bases
{
    public abstract class ModelConfigurationBase<T> where T : EntityBase
    {
        private readonly ModelBuilder modelBuilder;
        protected Action<EntityTypeBuilder<T>>? buildAction;
        protected ModelConfigurationBase(ModelBuilder modelBuilder)
        {
            this.modelBuilder = modelBuilder;
        }
        public void BuildModel()
        {
            if (buildAction != null)
                modelBuilder.Entity<T>(buildAction);
        }

    }
}