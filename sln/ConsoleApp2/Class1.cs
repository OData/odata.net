using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApp2
{
    static class Class1
    {
        public static IEnumerable<T> Merge<T>(this IEnumerable<T> left, IEnumerable<T> right, IComparer<T> comparer)
        {
            using (var leftEnumerator = left.GetEnumerator())
            using (var rightEnumerator = right.GetEnumerator())
            {
                var leftMove = leftEnumerator.MoveNext();
                var rightMove = rightEnumerator.MoveNext();
                while (leftMove || rightMove)
                {
                    if (leftMove && rightMove)
                    {
                        if (comparer.Compare(leftEnumerator.Current, rightEnumerator.Current) < 0)
                        {
                            yield return leftEnumerator.Current;
                            leftMove = leftEnumerator.MoveNext();
                        }
                        else
                        {
                            yield return rightEnumerator.Current;
                            rightMove = rightEnumerator.MoveNext();
                        }
                    }
                    else if (leftMove)
                    {
                        yield return leftEnumerator.Current;
                        leftMove = leftEnumerator.MoveNext();
                    }
                    else
                    {
                        yield return rightEnumerator.Current;
                        rightMove = rightEnumerator.MoveNext();
                    }
                }
            }
        }
        
        public sealed class Provider : IProvider
        {
            private readonly IProvider left;

            private readonly IProvider right;

            public Provider(IProvider left, IProvider right)
            {
                this.left = left;
                this.right = right;
            }

            public Page<T> FulfillRequest<T>(int maxPageSize, IComparer<T> comparer, byte[] pageToken)
            {
                var token = new AggregatePageToken(pageToken);

                var leftPager = new Pager<T>(this.left, maxPageSize, comparer, token.Left);
                var rightPager = new Pager<T>(this.right, maxPageSize, comparer, token.Right);

                return new Page<T>(
                    leftPager.Merge(rightPager, comparer).Take(maxPageSize),
                    () => new AggregatePageToken(leftPager.CurrentToken, rightPager.CurrentToken).Aggregate);
            }

            public Page<T> FulfillRequest<T>(int maxPageSize, IComparer<T> comparer)
            {
                return FulfillRequest<T>(maxPageSize, comparer, AggregatePageToken.Empty.Aggregate);
            }

            private sealed class Pager<T> : IEnumerable<T>
            {
                private readonly IProvider provider;

                private readonly int maxPageSize;

                private readonly IComparer<T> comparer;

                private byte[] currentToken;

                private int currentCount;

                public Pager(IProvider provider, int maxPageSize, IComparer<T> comparer, PageToken token)
                {
                    this.provider = provider;
                    this.maxPageSize = maxPageSize;
                    this.comparer = comparer;

                    this.currentToken = token.Token;
                    this.currentCount = token.Count;
                }

                public PageToken CurrentToken
                {
                    get
                    {
                        return new PageToken(this.currentToken, this.currentCount);
                    }
                }

                public IEnumerator<T> GetEnumerator()
                {
                    var toSkip = this.currentCount;
                    while (this.currentCount != -1)
                    {
                        Page<T> result;
                        if (this.currentToken == null)
                        {
                            result = this.provider.FulfillRequest<T>(this.maxPageSize, this.comparer);
                        }
                        else
                        {
                            result = this.provider.FulfillRequest<T>(this.maxPageSize, this.comparer, this.currentToken);
                        }

                        foreach (var element in result.Data.Skip(toSkip))
                        {
                            this.currentCount++;
                            yield return element;
                        }

                        toSkip = 0;

                        this.currentToken = result.PageToken;
                        this.currentCount = result.PageToken == null ? -1 : 0;
                    }
                }

                System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
                {
                    return this.GetEnumerator();
                }
            }

            private sealed class AggregatePageToken
            {
                public AggregatePageToken(byte[] token)
                {
                    var parts = Encoding.UTF8.GetString(token).Split(":");

                    this.Left = new PageToken(Convert.FromBase64String(parts[0]), int.Parse(parts[1]));
                    this.Right = new PageToken(Convert.FromBase64String(parts[2]), int.Parse(parts[3]));
                    this.Aggregate = token;
                }

                public AggregatePageToken(PageToken left, PageToken right)
                {
                    this.Left = left;
                    this.Right = right;
                    this.Aggregate = Encoding.UTF8.GetBytes(string.Concat(
                        Convert.ToBase64String(left.Token),
                        ":",
                        left.Count,
                        ":",
                        Convert.ToBase64String(right.Token),
                        ":",
                        right.Count));
                }

                public static AggregatePageToken Empty { get; } = new AggregatePageToken(PageToken.Empty, PageToken.Empty);

                public PageToken Left { get; }

                public PageToken Right { get; }

                public byte[] Aggregate { get; }
            }

            private sealed class PageToken
            {
                public PageToken(byte[] token, int count)
                {
                    this.Token = token;
                    this.Count = count;
                }

                public static PageToken Empty { get; } = new PageToken(null, -1);

                public byte[] Token { get; }

                public int Count { get; }
            }
        }

        public interface IProvider
        {
            Page<T> FulfillRequest<T>(int maxPageSize, IComparer<T> comparer, byte[] pageToken);

            Page<T> FulfillRequest<T>(int maxPageSize, IComparer<T> comparer);
        }

        public sealed class Page<T>
        {
            private readonly IEnumerable<T> data;

            private readonly Func<byte[]> pageTokenFactory;

            private bool enumerated;

            public Page(IEnumerable<T> data, Func<byte[]> pageTokenFactory)
            {
                this.data = data;
                this.pageTokenFactory = pageTokenFactory;

                this.enumerated = false;
            }

            public IEnumerable<T> Data
            {
                get
                {
                    foreach (var element in this.data)
                    {
                        yield return element;
                    }

                    this.enumerated = true;
                }
            }

            public byte[] PageToken
            {
                get
                {
                    if (!this.enumerated)
                    {
                        throw new InvalidOperationException();
                    }

                    return this.pageTokenFactory();
                }
            }
        }
    }
}
