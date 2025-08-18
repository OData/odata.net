namespace Odata
{
    public interface IEntityIdHeaderValueReader<out TVersion, out TNext> : IOdataReader<TVersion, IEntityIdReader<TVersion, TimeZoneNotFoundException>>
        where TVersion : Version.V1
    {
    }
}
