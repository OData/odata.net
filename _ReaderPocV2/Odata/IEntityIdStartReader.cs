namespace Odata
{
    public interface IEntityIdStartReader<out TVersion, out TNext> : IOdataReader<TVersion, IIriSchemeReader<TVersion, TNext>>
        where TVersion : Version.V2
    {
    }
}
