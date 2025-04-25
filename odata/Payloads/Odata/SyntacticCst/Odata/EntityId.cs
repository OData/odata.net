namespace Payloads.Odata.SyntacticCst.Odata
{
    public sealed class EntityId
    {
        private EntityId(
            Uri uri // NOTE: this does not need to be an OData URI
            )
        {
            Iri = ToIri(uri);
        }

        private Iri Iri { get; }

        private static Iri ToIri(Uri uri)
        {
            //// TODO this should be its own public extension method
            throw new System.Exception("TODO");
        }
    }

    public sealed class Iri //// TODO i think this is based on another rfc and should go in it's own folder
    {
        private Iri()
        {
        }
    }
}
