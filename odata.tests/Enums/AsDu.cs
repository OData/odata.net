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
            var iterations = AsEnum.Iterations;
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

            protected abstract TResult Dispatch<TResult>(Visitor<TResult> visitor);

            public abstract class Visitor<TResult>
            {
                public TResult Visit(Kind node)
                {
                    return node.Dispatch(this);
                }
                protected internal abstract TResult Accept(Type1 node);
                protected internal abstract TResult Accept(Type2 node);
            }

            public sealed class Type1 : Kind
            {
                private Type1()
                {
                }

                public static Type1 Instance { get; } = new Type1();

                protected sealed override TResult Dispatch<TResult>(Visitor<TResult> visitor)
                {
                    return visitor.Accept(this);
                }
            }

            public sealed class Type2 : Kind
            {
                private Type2()
                {
                }

                public static Type2 Instance { get; } = new Type2();

                protected sealed override TResult Dispatch<TResult>(Visitor<TResult> visitor)
                {
                    return visitor.Accept(this);
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

        private sealed class Visitor : Kind.Visitor<int>
        {
            private Visitor()
            {
            }

            public static Visitor Instance { get; } = new Visitor();

            protected internal override int Accept(Kind.Type1 node)
            {
                var subNode = new Type1()
                {
                    Data = int.Parse("asdf"), //// TODO
                };
                return Handler.Instance.Handle(subNode);
            }

            protected internal override int Accept(Kind.Type2 node)
            {
                var subNode = new Type2()
                {
                    Data = "asdf", //// TODO
                };
                return Handler.Instance.Handle(subNode);
            }
        }

        public static int Handle<TResult>(Node node, IHandler<TResult> handler)
        {
            return Visitor.Instance.Visit(node.Kind);
        }
    }
}
