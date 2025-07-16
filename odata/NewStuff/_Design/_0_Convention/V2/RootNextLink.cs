namespace NewStuff._Design._0_Convention.V2
{
    using System;

    public sealed class RootNextLink
    {
        internal RootNextLink(Uri uri)
        {
            Uri = uri;
        }

        internal Uri Uri { get; }
    }
}
