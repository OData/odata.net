namespace NewStuff._Design._4.Sample
{
    using System.Collections.Generic;

    public interface ITree<T>
    {
        T Data { get; }

        IEnumerable<ITree<T>> Children { get; }
    }
}
