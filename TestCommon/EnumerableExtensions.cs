using System.Collections.Generic;
using System.Threading.Tasks;

namespace TestCommon
{
    public static class EnumerableExtensions
    {
        public static async IAsyncEnumerable<TItem> AsAsyncEnumerable<TItem>(this IEnumerable<TItem> items)
        {
            foreach (var item in items)
                yield return item;

            await Task.CompletedTask;
        }
    }
}