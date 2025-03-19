using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Enums
{
    [TestClass]
    public class AsDu
    {
        [TestMethod]
        public void Perf()
        {
            var iterations = 1000000;
            Stopwatch stopwatch;

            var node = new Node()
            {
                Kind = Kind.Type2.Instance,
                Data = new NodeData()
                {
                    Data = "asdf",
                },
            };
            var handler = Handler.Instance;

            stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < iterations; ++i)
            {
                Handle(node, handler);
            }
            Console.WriteLine(stopwatch.ElapsedTicks);
        }

        public abstract class Kind
        {
            private Kind()
            {
            }

            protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context) where TContext : allows ref struct;

            public abstract class Visitor<TResult, TContext> where TContext : allows ref struct
            {
                public TResult Visit(Kind node, TContext context)
                {
                    ArgumentNullException.ThrowIfNull(node);

                    return node.Dispatch(this, context);
                }
                protected internal abstract TResult Accept(Type1 node, TContext context);
                protected internal abstract TResult Accept(Type2 node, TContext context);
            }

            public sealed class Type1 : Kind
            {
                private Type1()
                {
                }

                public static Type1 Instance { get; } = new Type1();

                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    ArgumentNullException.ThrowIfNull(visitor);

                    return visitor.Accept(this, context);
                }
            }

            public sealed class Type2 : Kind
            {
                private Type2()
                {
                }

                public static Type2 Instance { get; } = new Type2();

                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    ArgumentNullException.ThrowIfNull(visitor);

                    return visitor.Accept(this, context);
                }
            }
        }

        public ref struct Node
        {
            public Kind Kind { get; set; }

            public NodeData Data { get; set; }
        }

        public ref struct Context<TResult>
        {
            public Node Node { get; set; }

            public IHandler<TResult> Handler { get; set; }
        }

        private sealed class Visitor<TResult> : Kind.Visitor<TResult, Context<TResult>>
        {
            private Visitor()
            {
            }

            public static Visitor<TResult> Instance { get; } = new Visitor<TResult>();

            protected internal override TResult Accept(Kind.Type1 node, Context<TResult> context)
            {
                var subNode = new Type1()
                {
                    Data = int.Parse(context.Node.Data.Data),
                };
                return context.Handler.Handle(subNode);
            }

            protected internal override TResult Accept(Kind.Type2 node, Context<TResult> context)
            {
                var subNode = new Type2()
                {
                    Data = context.Node.Data.Data,
                };
                return context.Handler.Handle(subNode);
            }
        }

        public static TResult Handle<TResult>(Node node, IHandler<TResult> handler)
        {
            var context = new Context<TResult>()
            {
                Node = node,
                Handler = handler,
            };
            return Visitor<TResult>.Instance.Visit(node.Kind, context);
        }
    }
}
