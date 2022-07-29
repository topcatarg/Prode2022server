using System.Security.Claims;

namespace Prode2022Server.Helpers
{
    public static class Extensions
    {
        public static bool IsNullOrEmpty(this string? str)
        {
            return string.IsNullOrEmpty(str);
        }

        public static bool IsNullOrZero(this int? value)
        {
            return value == null || value == 0;
        }

        public static T? GetClaim<T>(this ClaimsPrincipal user, string type)
        {
            var value = user.Claims.FirstOrDefault(c => c.Type == type)?.Value;
            if (value == null)
            {
                return default(T);
            }

            var converter = System.ComponentModel.TypeDescriptor.GetConverter(typeof(T));
            return (T?)converter.ConvertFromString(value);
        }

        public static string? GetClaim(this ClaimsPrincipal user, string type)
        {
            return user.GetClaim<string>(type);
        }
    }
}
