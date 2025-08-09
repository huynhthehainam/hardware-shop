using System.Runtime.CompilerServices;

namespace HardwareShop.Domain.Extensions
{
    public static class PocoLoadingExtensions
    {
        public static TRelated? Load<TRelated>(
            this Action<object, string?> loader,
            object entity,
            ref TRelated? navigationField,
            [CallerMemberName] string? navigationName = null)
           where TRelated : class
        {
            loader.Invoke(entity, navigationName);

            return navigationField;
        }
        public static ICollection<TRelated>? Load<TRelated>(
          this Action<object, string?> loader,
          object entity,
          ref ICollection<TRelated>? navigationField,
          [CallerMemberName] string? navigationName = null)
         where TRelated : class
        {
            loader.Invoke(entity, navigationName);

            return navigationField;
        }
    }
}