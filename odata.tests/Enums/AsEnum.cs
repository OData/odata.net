using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enums
{
    [TestClass]
    public class AsEnum
    {
        public const int Iterations = 1000000;

        [TestMethod]
        public void Perf()
        {
            var iterations = AsEnum.Iterations;
            Stopwatch stopwatch;

            var node = new Node()
            {
                Kind = Kind.Type2,
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

        public enum Kind
        {
            Type1 = 1,
            Type2 = 2,
        }

        public ref struct Node
        {
            public Kind Kind { get; set; }

            public NodeData Data { get; set; }
        }

        public static TResult Handle<TResult>(Node node, IHandler<TResult> handler)
        {
            if (node.Kind == Kind.Type1)
            {
                var subNode = new Type1()
                {
                    Data = int.Parse(node.Data.Data),
                };
                return handler.Handle(subNode);
            }
            else if (node.Kind == Kind.Type2)
            {
                var subNode = new Type2()
                {
                    Data = node.Data.Data,
                };
                return handler.Handle(subNode);
            }

            throw new Exception("tODO unreachable code");
        }
    }
}
