namespace Odata
{
    public interface IEntityIdReader<out TVersion, out TNext> : IOdataReader<TVersion, EntityId, TNext>
        where TVersion : Version.V1
    {
    }
}
