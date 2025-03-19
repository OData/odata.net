using static Enums.AsEnum;

namespace Enums
{
    public ref struct NodeData
    {
        public string Data { get; set; }
    }

    public ref struct Type1
    {
        public int Data { get; set; }
    }

    public ref struct Type2
    {
        public string Data { get; set; }
    }

    public interface IHandler<TResult>
    {
        TResult Handle(Type1 type1);

        TResult Handle(Type2 type2);
    }

    public sealed class Handler : IHandler<int>
    {
        private Handler()
        {
        }

        public static Handler Instance { get; } = new Handler();

        public int Handle(Type1 type1)
        {
            return type1.Data;
        }

        public int Handle(Type2 type2)
        {
            return type2.Data.Length;
        }
    }
}
