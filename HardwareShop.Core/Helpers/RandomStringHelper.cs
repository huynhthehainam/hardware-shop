using System.Text;

namespace HardwareShop.Core.Helpers
{
    public static class RandomStringHelper
    {
        public static string RandomString(int length)
        {
            var random = new Random();
            const string pool = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var builder = new StringBuilder();

            for (var i = 0; i < length; i++)
            {
                var c = pool[random.Next(0, pool.Length)];
                builder.Append(c);
            }

            return builder.ToString();
        }
    }
}
