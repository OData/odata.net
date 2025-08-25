namespace Odata
{
    public interface IEntityIdHeaderValueReader<out TVersion, out TNext> : IOdataReader<TVersion, IEntityIdReader<TVersion, TNext>>
        where TVersion : Version.V1
    {
    }
}
