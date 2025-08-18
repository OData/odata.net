namespace Odata
{
    public interface IIriSchemeReader<out TVersion, out TNext> : IOdataReader<TVersion, IriScheme, TNext>
        where TVersion : Version.V2 //// TODO this might be wrong
    {
    }
}
