namespace NewStuff._Design._4.Sample
{
    using System;
    using System.IO;

    public static class TreeExtensions
    {
        public static void Write<T>(this ITree<T> tree, Action<Writer, T> write, TextWriter textWriter)
        {
            var writer = new Writer(textWriter);
            Write(tree, writer, write);
        }

        private static void Write<T>(ITree<T> tree, Writer writer, Action<Writer, T> write)
        {
            writer.WriteLine("{");
            write(writer, tree.Data);
            writer.WriteLine("}");
            writer.Indent();
            foreach (var child in tree.Children)
            {
                Write(child, writer, write);
            }

            writer.Unindent();
        }
    }

    public sealed class Writer
    {
        private readonly TextWriter textWriter;

        private int indentCount;

        public Writer(TextWriter textWriter)
        {
            this.textWriter = textWriter;

            this.indentCount = 0;
        }

        public void Indent()
        {
            ++this.indentCount;
        }

        public void Unindent()
        {
            --this.indentCount;
        }

        public void WriteLine(string message)
        {
            this.textWriter.Write(new string('\t', this.indentCount));
            this.textWriter.WriteLine(message);
        }
    }
}
