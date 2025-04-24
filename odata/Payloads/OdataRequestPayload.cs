namespace Payloads
{
    public abstract class OdataRequestPayload
    {
        private OdataRequestPayload()
        {
        }

        public sealed class Json : OdataRequestPayload
        {
            private Json()
            {
            }


        }
    }

    public sealed class JsonRequestHeaders
    {
        private JsonRequestHeaders()
        {
        }
    }
}
