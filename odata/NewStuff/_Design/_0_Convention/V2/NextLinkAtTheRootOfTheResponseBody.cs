namespace NewStuff._Design._0_Convention.V2
{
    using System;

    public sealed class NextLinkAtTheRootOfTheResponseBody
    {
        internal NextLinkAtTheRootOfTheResponseBody(Uri uri)
        {
            Uri = uri;
        }

        internal Uri Uri { get; }
    }
}
