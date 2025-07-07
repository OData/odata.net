namespace NewStuff._Design._0_Convention.Readers
{
    using System;
    using System.Threading.Tasks;

    public interface IReader<T> where T : allows ref struct
    {
        ValueTask Read();

        T Next();
    }


    public interface IGetRequestReader : IAsyncDisposable
    {
    }


}
