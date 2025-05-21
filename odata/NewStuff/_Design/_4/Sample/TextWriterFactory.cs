namespace NewStuff._Design._4.Sample
{
    using System;
    using System.IO;

    public static class TextWriterFactory
    {
        public static TextWriter Create()
        {
            return Console.Out;
        }
    }
}
