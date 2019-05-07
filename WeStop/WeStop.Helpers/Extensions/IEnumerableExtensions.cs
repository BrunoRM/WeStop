using System.Collections.Generic;
using System.Linq;

namespace WeStop.Helpers.Extensions
{
    public static class IEnumerableExtensions
    {
        public static string ToStringWithCommaDelimiter(this IEnumerable<string> list, bool spaceBetweenValues = false)
        {
            string result = string.Empty;

            foreach (var value in list)
            {
                if (string.IsNullOrEmpty(result))
                    result = value;
                else
                {
                    if (spaceBetweenValues)
                        result += $", {value}";
                    else
                        result += $",{value}";
                }
            }

            return result;
        }

        public static bool ContainsOnlySpecifiedValues(this IEnumerable<string> list, string[] values)
        {
            foreach (var value in list)
            {
                if (values.FirstOrDefault(x => x == value) != null)
                    continue;
                else
                    return false;
            }

            return true;
        }
    }
}
