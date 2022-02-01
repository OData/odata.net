using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData
{
    internal static class PerfExtensions
    {
        internal static void ForEach<TElement>(this IEnumerable<TElement> collection, Action<TElement> action)
        {
            if (collection is List<TElement> list)
            {
                foreach (TElement item in list)
                {
                    action(item);
                }
            }
            else
            {
                foreach (TElement item in collection)
                {
                    action(item);
                }
            }
        }

        internal static void ForEach<TElement, TArg1>(this IEnumerable<TElement> collection, Action<TElement, TArg1> action, TArg1 arg1)
        {
            if (collection is List<TElement> list)
            {
                foreach (TElement item in list)
                {
                    action(item, arg1);
                }
            }
            else
            {
                foreach (TElement item in collection)
                {
                    action(item, arg1);
                }
            }
        }

        internal static void ForEach<TElement, TArg1, TArg2>(this IEnumerable<TElement> collection, Action<TElement, TArg1, TArg2> action, TArg1 arg1, TArg2 arg2)
        {
            if (collection is List<TElement> list)
            {
                foreach (TElement item in list)
                {
                    action(item, arg1, arg2);
                }
            }
            else
            {
                foreach (TElement item in collection)
                {
                    action(item, arg1, arg2);
                }
            }
        }

        internal static void ForEach<TElement, TArg1, TArg2, TArg3>(this IEnumerable<TElement> collection, Action<TElement, TArg1, TArg2, TArg3> action, TArg1 arg1, TArg2 arg2, TArg3 arg3)
        {
            if (collection is List<TElement> list)
            {
                foreach (TElement item in list)
                {
                    action(item, arg1, arg2, arg3);
                }
            }
            else
            {
                foreach (TElement item in collection)
                {
                    action(item, arg1, arg2, arg3);
                }
            }
        }

        internal static void ForEach<TElement, TArg1, TArg2, TArg3, TArg4>(this IEnumerable<TElement> collection, Action<TElement, TArg1, TArg2, TArg3, TArg4> action, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4)
        {
            if (collection is List<TElement> list)
            {
                foreach (TElement item in list)
                {
                    action(item, arg1, arg2, arg3, arg4);
                }
            }
            else
            {
                foreach (TElement item in collection)
                {
                    action(item, arg1, arg2, arg3, arg4);
                }
            }
        }
    }
}
