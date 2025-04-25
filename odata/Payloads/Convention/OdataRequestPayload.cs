using Payloads.SyntacticCst.Odata;

namespace Payloads.Convention
{
    //// TODO it seems like there will need to be a subsequent step to translate from "syntactic" CST to "semantic" CST (to do things like validate the `metadata` format parameters against the `odata-version` header, for example); then, there can be an optional step to convert from a "semantic" CST to a "convention payload" (meaning, a CLR type that doesn't care about ordering or syntax or anything, just about the *meaning* of the values)
    //// TODO i'm currently trying to define "syntactic" CSTs

    public abstract class OdataRequestPayload
    {
        private OdataRequestPayload()
        {
        }

        public sealed class Json : OdataRequestPayload
        {
            private Json(FormatRequestHeader? accept)
            {
                Accept = accept;
            }

            //// TODO you need to go through the "protocol" doc to capture the things that apply to all requests/responses/etc
            
            public FormatRequestHeader? Accept { get; }
        }
    }
}
