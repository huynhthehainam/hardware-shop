

using HardwareShop.Core.Bases;

namespace HardwareShop.Core.Models
{
    public class CreateIfNotExistResponse<T> where T : EntityBase
    {
        public bool IsExist { get; internal set; }
        public T Entity { get; internal set; }
        public CreateIfNotExistResponse(bool isExist, T entity)
        {
            IsExist = isExist;
            Entity = entity;
        }
    }
}