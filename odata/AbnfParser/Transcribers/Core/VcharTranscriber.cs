namespace AbnfParser.Transcribers.Core
{
    using System.Text;

    using AbnfParser.CstNodes.Core;
    using Root;

    public sealed class VcharTranscriber : Vchar.Visitor<Void, StringBuilder>
    {
        private VcharTranscriber()
        {
        }

        public static VcharTranscriber Instance { get; } = new VcharTranscriber();

        protected internal override Void Accept(Vchar.x21 node, StringBuilder context)
        {
            return x21Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Vchar.x22 node, StringBuilder context)
        {
            return x22Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Vchar.x23 node, StringBuilder context)
        {
            return x23Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Vchar.x24 node, StringBuilder context)
        {
            return x24Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Vchar.x25 node, StringBuilder context)
        {
            return x25Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Vchar.x26 node, StringBuilder context)
        {
            return x26Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Vchar.x27 node, StringBuilder context)
        {
            return x27Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Vchar.x28 node, StringBuilder context)
        {
            return x28Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Vchar.x29 node, StringBuilder context)
        {
            return x29Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Vchar.x2A node, StringBuilder context)
        {
            return x2ATranscriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Vchar.x2B node, StringBuilder context)
        {
            return x2BTranscriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Vchar.x2C node, StringBuilder context)
        {
            return x2CTranscriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Vchar.x2D node, StringBuilder context)
        {
            return x2DTranscriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Vchar.x2E node, StringBuilder context)
        {
            return x2ETranscriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Vchar.x2F node, StringBuilder context)
        {
            return x2FTranscriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Vchar.x30 node, StringBuilder context)
        {
            return x30Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Vchar.x31 node, StringBuilder context)
        {
            return x31Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Vchar.x32 node, StringBuilder context)
        {
            return x32Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Vchar.x33 node, StringBuilder context)
        {
            return x33Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Vchar.x34 node, StringBuilder context)
        {
            return x34Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Vchar.x35 node, StringBuilder context)
        {
            return x35Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Vchar.x36 node, StringBuilder context)
        {
            return x36Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Vchar.x37 node, StringBuilder context)
        {
            return x37Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Vchar.x38 node, StringBuilder context)
        {
            return x38Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Vchar.x39 node, StringBuilder context)
        {
            return x39Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Vchar.x3A node, StringBuilder context)
        {
            return x3ATranscriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Vchar.x3B node, StringBuilder context)
        {
            return x3BTranscriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Vchar.x3C node, StringBuilder context)
        {
            return x3CTranscriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Vchar.x3D node, StringBuilder context)
        {
            return x3DTranscriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Vchar.x3E node, StringBuilder context)
        {
            return x3ETranscriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Vchar.x3F node, StringBuilder context)
        {
            return x3FTranscriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Vchar.x40 node, StringBuilder context)
        {
            return x40Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Vchar.x41 node, StringBuilder context)
        {
            return x41Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Vchar.x42 node, StringBuilder context)
        {
            return x42Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Vchar.x43 node, StringBuilder context)
        {
            return x43Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Vchar.x44 node, StringBuilder context)
        {
            return x44Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Vchar.x45 node, StringBuilder context)
        {
            return x45Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Vchar.x46 node, StringBuilder context)
        {
            return x46Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Vchar.x47 node, StringBuilder context)
        {
            return x47Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Vchar.x48 node, StringBuilder context)
        {
            return x48Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Vchar.x49 node, StringBuilder context)
        {
            return x49Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Vchar.x4A node, StringBuilder context)
        {
            return x4ATranscriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Vchar.x4B node, StringBuilder context)
        {
            return x4BTranscriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Vchar.x4C node, StringBuilder context)
        {
            return x4CTranscriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Vchar.x4D node, StringBuilder context)
        {
            return x4DTranscriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Vchar.x4E node, StringBuilder context)
        {
            return x4ETranscriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Vchar.x4F node, StringBuilder context)
        {
            return x4FTranscriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Vchar.x50 node, StringBuilder context)
        {
            return x50Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Vchar.x51 node, StringBuilder context)
        {
            return x51Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Vchar.x52 node, StringBuilder context)
        {
            return x52Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Vchar.x53 node, StringBuilder context)
        {
            return x53Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Vchar.x54 node, StringBuilder context)
        {
            return x54Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Vchar.x55 node, StringBuilder context)
        {
            return x55Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Vchar.x56 node, StringBuilder context)
        {
            return x56Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Vchar.x57 node, StringBuilder context)
        {
            return x57Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Vchar.x58 node, StringBuilder context)
        {
            return x58Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Vchar.x59 node, StringBuilder context)
        {
            return x59Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Vchar.x5A node, StringBuilder context)
        {
            return x5ATranscriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Vchar.x5B node, StringBuilder context)
        {
            return x5BTranscriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Vchar.x5C node, StringBuilder context)
        {
            return x5CTranscriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Vchar.x5D node, StringBuilder context)
        {
            return x5DTranscriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Vchar.x5E node, StringBuilder context)
        {
            return x5ETranscriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Vchar.x5F node, StringBuilder context)
        {
            return x5FTranscriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Vchar.x60 node, StringBuilder context)
        {
            return x60Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Vchar.x61 node, StringBuilder context)
        {
            return x61Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Vchar.x62 node, StringBuilder context)
        {
            return x62Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Vchar.x63 node, StringBuilder context)
        {
            return x63Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Vchar.x64 node, StringBuilder context)
        {
            return x64Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Vchar.x65 node, StringBuilder context)
        {
            return x65Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Vchar.x66 node, StringBuilder context)
        {
            return x66Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Vchar.x67 node, StringBuilder context)
        {
            return x67Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Vchar.x68 node, StringBuilder context)
        {
            return x68Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Vchar.x69 node, StringBuilder context)
        {
            return x69Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Vchar.x6A node, StringBuilder context)
        {
            return x6ATranscriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Vchar.x6B node, StringBuilder context)
        {
            return x6BTranscriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Vchar.x6C node, StringBuilder context)
        {
            return x6CTranscriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Vchar.x6D node, StringBuilder context)
        {
            return x6DTranscriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Vchar.x6E node, StringBuilder context)
        {
            return x6ETranscriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Vchar.x6F node, StringBuilder context)
        {
            return x6FTranscriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Vchar.x70 node, StringBuilder context)
        {
            return x70Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Vchar.x71 node, StringBuilder context)
        {
            return x71Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Vchar.x72 node, StringBuilder context)
        {
            return x72Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Vchar.x73 node, StringBuilder context)
        {
            return x73Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Vchar.x74 node, StringBuilder context)
        {
            return x74Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Vchar.x75 node, StringBuilder context)
        {
            return x75Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Vchar.x76 node, StringBuilder context)
        {
            return x76Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Vchar.x77 node, StringBuilder context)
        {
            return x77Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Vchar.x78 node, StringBuilder context)
        {
            return x78Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Vchar.x79 node, StringBuilder context)
        {
            return x79Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Vchar.x7A node, StringBuilder context)
        {
            return x7ATranscriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Vchar.x7B node, StringBuilder context)
        {
            return x7BTranscriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Vchar.x7C node, StringBuilder context)
        {
            return x7CTranscriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Vchar.x7D node, StringBuilder context)
        {
            return x7DTranscriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Vchar.x7E node, StringBuilder context)
        {
            return x7ETranscriber.Instance.Transcribe(node.Value, context);
        }
    }
}
