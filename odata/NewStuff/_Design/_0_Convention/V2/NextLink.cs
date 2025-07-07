namespace NewStuff._Design._0_Convention.V2
{
    using System;

    public sealed class NextLink
    {
        internal NextLink(Uri uri)
        {
            Uri = uri;
        }

        internal Uri Uri { get; }
    }
}
