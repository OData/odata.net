namespace AbnfParser.Transcribers.Core
{
    using System.Text;

    using AbnfParser.CstNodes.Core;
    using Root;

    public sealed class AlphaTranscriber : Alpha.Visitor<Root.Void, StringBuilder>
    {
        private AlphaTranscriber()
        {
        }

        public static AlphaTranscriber Instance { get; } = new AlphaTranscriber();

        protected internal override Void Accept(Alpha.x41 node, StringBuilder context)
        {
            return x41Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Alpha.x42 node, StringBuilder context)
        {
            return x42Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Alpha.x43 node, StringBuilder context)
        {
            return x43Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Alpha.x44 node, StringBuilder context)
        {
            return x44Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Alpha.x45 node, StringBuilder context)
        {
            return x45Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Alpha.x46 node, StringBuilder context)
        {
            return x46Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Alpha.x47 node, StringBuilder context)
        {
            return x47Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Alpha.x48 node, StringBuilder context)
        {
            return x48Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Alpha.x49 node, StringBuilder context)
        {
            return x49Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Alpha.x4A node, StringBuilder context)
        {
            return x4ATranscriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Alpha.x4B node, StringBuilder context)
        {
            return x4BTranscriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Alpha.x4C node, StringBuilder context)
        {
            return x4CTranscriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Alpha.x4D node, StringBuilder context)
        {
            return x4DTranscriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Alpha.x4E node, StringBuilder context)
        {
            return x4ETranscriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Alpha.x4F node, StringBuilder context)
        {
            return x4FTranscriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Alpha.x50 node, StringBuilder context)
        {
            return x50Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Alpha.x51 node, StringBuilder context)
        {
            return x51Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Alpha.x52 node, StringBuilder context)
        {
            return x52Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Alpha.x53 node, StringBuilder context)
        {
            return x53Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Alpha.x54 node, StringBuilder context)
        {
            return x54Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Alpha.x55 node, StringBuilder context)
        {
            return x55Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Alpha.x56 node, StringBuilder context)
        {
            return x56Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Alpha.x57 node, StringBuilder context)
        {
            return x57Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Alpha.x58 node, StringBuilder context)
        {
            return x58Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Alpha.x59 node, StringBuilder context)
        {
            return x59Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Alpha.x5A node, StringBuilder context)
        {
            return x5ATranscriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Alpha.x61 node, StringBuilder context)
        {
            return x61Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Alpha.x62 node, StringBuilder context)
        {
            return x62Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Alpha.x63 node, StringBuilder context)
        {
            return x63Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Alpha.x64 node, StringBuilder context)
        {
            return x64Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Alpha.x65 node, StringBuilder context)
        {
            return x65Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Alpha.x66 node, StringBuilder context)
        {
            return x66Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Alpha.x67 node, StringBuilder context)
        {
            return x67Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Alpha.x68 node, StringBuilder context)
        {
            return x68Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Alpha.x69 node, StringBuilder context)
        {
            return x69Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Alpha.x6A node, StringBuilder context)
        {
            return x6ATranscriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Alpha.x6B node, StringBuilder context)
        {
            return x6BTranscriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Alpha.x6C node, StringBuilder context)
        {
            return x6CTranscriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Alpha.x6D node, StringBuilder context)
        {
            return x6DTranscriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Alpha.x6E node, StringBuilder context)
        {
            return x6ETranscriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Alpha.x6F node, StringBuilder context)
        {
            return x6FTranscriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Alpha.x70 node, StringBuilder context)
        {
            return x70Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Alpha.x71 node, StringBuilder context)
        {
            return x71Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Alpha.x72 node, StringBuilder context)
        {
            return x72Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Alpha.x73 node, StringBuilder context)
        {
            return x73Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Alpha.x74 node, StringBuilder context)
        {
            return x74Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Alpha.x75 node, StringBuilder context)
        {
            return x75Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Alpha.x76 node, StringBuilder context)
        {
            return x76Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Alpha.x77 node, StringBuilder context)
        {
            return x77Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Alpha.x78 node, StringBuilder context)
        {
            return x78Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Alpha.x79 node, StringBuilder context)
        {
            return x79Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Alpha.x7A node, StringBuilder context)
        {
            return x7ATranscriber.Instance.Transcribe(node.Value, context);
        }

    }

}
