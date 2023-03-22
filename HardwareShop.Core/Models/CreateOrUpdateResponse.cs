

using HardwareShop.Core.Bases;

namespace HardwareShop.Core.Models
{
    public class CreateOrUpdateResponse<T> where T : EntityBase
    {
        public bool IsUpdate { get; internal set; }
        public T Entity { get; internal set; }
        public CreateOrUpdateResponse(bool isUpdate, T entity)
        {
            IsUpdate = isUpdate;
            Entity = entity;
        }
    }
}