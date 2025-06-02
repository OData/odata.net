namespace NewStuff._Design._4.Sample
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    public static class TreeExtensions
    {
        public static async Task Write<T>(this ITree<T> tree, Action<Writer, T> write, TextWriter textWriter)
        {
            var writer = new Writer(textWriter);
            await Write(tree, writer, write).ConfigureAwait(false);
        }

        private static async Task Write<T>(ITree<T> tree, Writer writer, Action<Writer, T> write)
        {
            writer.WriteLine("{");
            write(writer, tree.Data);
            writer.WriteLine("}");
            writer.Indent();
            await foreach (var child in tree.Children.ConfigureAwait(false))
            {
                await Write(child, writer, write).ConfigureAwait(false);
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
