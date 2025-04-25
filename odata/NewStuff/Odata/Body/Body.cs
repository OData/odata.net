namespace NewStuff.Odata.Body
{
    using NewStuff.Odata.Headers;

    public sealed class BodyParser
    {
        private BodyParser()
        {
        }

        private static Body Parse(Headers headers)
        {
            // from discussion, this method seems to make sense in terms of the flowchart; for a fully-implemented, enterprise-quality parser, the headers need to be parsed, valdiated, and interpreted before the body can start to be parsted; for developers who already "know" (or rather, are making assumptions) about the headers, they don't need to actually parse the headers; what this method allows is for those developers to simply provide a singleton instance of their "known" headers, and the enterprise developers can use the headers parser to create and validate those headers, and that instance can be passed to this method; for the developers who don't need those checks, they could even take a sample response from a service or something and use those headers to generate their headers instance or something

            throw new System.Exception("TODO");
        }
    }

    public sealed class Body
    {
    }
}
