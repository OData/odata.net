namespace AbnfParser.Transcribers
{
    using System.Text;

    using AbnfParser.CstNodes;
    using AbnfParser.Transcribers.Core;
    using Root;

    public sealed class CharValTranscriber
    {
        private CharValTranscriber()
        {
        }

        public static CharValTranscriber Instance { get; } = new CharValTranscriber();

        public Void Transcribe(CharVal node, StringBuilder context)
        {
            DquoteTranscriber.Instance.Transcribe(node.OpenDquote, context);
            foreach (var inner in node.Inners)
            {
                InnerTranscriber.Instance.Visit(inner, context);
            }

            DquoteTranscriber.Instance.Transcribe(node.CloseDquote, context);
            return default;
        }

        public sealed class InnerTranscriber : CharVal.Inner.Visitor<Void, StringBuilder>
        {
            private InnerTranscriber()
            {
            }

            public static InnerTranscriber Instance { get; } = new InnerTranscriber();

            protected internal override Void Accept(CharVal.Inner.x20 node, StringBuilder context)
            {
                return x20Transcriber.Instance.Transcribe(node.Value, context);
            }

            protected internal override Void Accept(CharVal.Inner.x21 node, StringBuilder context)
            {
                return x21Transcriber.Instance.Transcribe(node.Value, context);
            }

            protected internal override Void Accept(CharVal.Inner.x23 node, StringBuilder context)
            {
                return x23Transcriber.Instance.Transcribe(node.Value, context);
            }

            protected internal override Void Accept(CharVal.Inner.x24 node, StringBuilder context)
            {
                return x24Transcriber.Instance.Transcribe(node.Value, context);
            }

            protected internal override Void Accept(CharVal.Inner.x25 node, StringBuilder context)
            {
                return x25Transcriber.Instance.Transcribe(node.Value, context);
            }

            protected internal override Void Accept(CharVal.Inner.x26 node, StringBuilder context)
            {
                return x26Transcriber.Instance.Transcribe(node.Value, context);
            }

            protected internal override Void Accept(CharVal.Inner.x27 node, StringBuilder context)
            {
                return x27Transcriber.Instance.Transcribe(node.Value, context);
            }

            protected internal override Void Accept(CharVal.Inner.x28 node, StringBuilder context)
            {
                return x28Transcriber.Instance.Transcribe(node.Value, context);
            }

            protected internal override Void Accept(CharVal.Inner.x29 node, StringBuilder context)
            {
                return x29Transcriber.Instance.Transcribe(node.Value, context);
            }

            protected internal override Void Accept(CharVal.Inner.x2A node, StringBuilder context)
            {
                return x2ATranscriber.Instance.Transcribe(node.Value, context);
            }

            protected internal override Void Accept(CharVal.Inner.x2B node, StringBuilder context)
            {
                return x2BTranscriber.Instance.Transcribe(node.Value, context);
            }

            protected internal override Void Accept(CharVal.Inner.x2C node, StringBuilder context)
            {
                return x2CTranscriber.Instance.Transcribe(node.Value, context);
            }

            protected internal override Void Accept(CharVal.Inner.x2D node, StringBuilder context)
            {
                return x2DTranscriber.Instance.Transcribe(node.Value, context);
            }

            protected internal override Void Accept(CharVal.Inner.x2E node, StringBuilder context)
            {
                return x2ETranscriber.Instance.Transcribe(node.Value, context);
            }

            protected internal override Void Accept(CharVal.Inner.x2F node, StringBuilder context)
            {
                return x2FTranscriber.Instance.Transcribe(node.Value, context);
            }

            protected internal override Void Accept(CharVal.Inner.x30 node, StringBuilder context)
            {
                return x30Transcriber.Instance.Transcribe(node.Value, context);
            }

            protected internal override Void Accept(CharVal.Inner.x31 node, StringBuilder context)
            {
                return x31Transcriber.Instance.Transcribe(node.Value, context);
            }

            protected internal override Void Accept(CharVal.Inner.x32 node, StringBuilder context)
            {
                return x32Transcriber.Instance.Transcribe(node.Value, context);
            }

            protected internal override Void Accept(CharVal.Inner.x33 node, StringBuilder context)
            {
                return x33Transcriber.Instance.Transcribe(node.Value, context);
            }

            protected internal override Void Accept(CharVal.Inner.x34 node, StringBuilder context)
            {
                return x34Transcriber.Instance.Transcribe(node.Value, context);
            }

            protected internal override Void Accept(CharVal.Inner.x35 node, StringBuilder context)
            {
                return x35Transcriber.Instance.Transcribe(node.Value, context);
            }

            protected internal override Void Accept(CharVal.Inner.x36 node, StringBuilder context)
            {
                return x36Transcriber.Instance.Transcribe(node.Value, context);
            }

            protected internal override Void Accept(CharVal.Inner.x37 node, StringBuilder context)
            {
                return x37Transcriber.Instance.Transcribe(node.Value, context);
            }

            protected internal override Void Accept(CharVal.Inner.x38 node, StringBuilder context)
            {
                return x38Transcriber.Instance.Transcribe(node.Value, context);
            }

            protected internal override Void Accept(CharVal.Inner.x39 node, StringBuilder context)
            {
                return x39Transcriber.Instance.Transcribe(node.Value, context);
            }

            protected internal override Void Accept(CharVal.Inner.x3A node, StringBuilder context)
            {
                return x3ATranscriber.Instance.Transcribe(node.Value, context);
            }

            protected internal override Void Accept(CharVal.Inner.x3B node, StringBuilder context)
            {
                return x3BTranscriber.Instance.Transcribe(node.Value, context);
            }

            protected internal override Void Accept(CharVal.Inner.x3C node, StringBuilder context)
            {
                return x3CTranscriber.Instance.Transcribe(node.Value, context);
            }

            protected internal override Void Accept(CharVal.Inner.x3D node, StringBuilder context)
            {
                return x3DTranscriber.Instance.Transcribe(node.Value, context);
            }

            protected internal override Void Accept(CharVal.Inner.x3E node, StringBuilder context)
            {
                return x3ETranscriber.Instance.Transcribe(node.Value, context);
            }

            protected internal override Void Accept(CharVal.Inner.x3F node, StringBuilder context)
            {
                return x3FTranscriber.Instance.Transcribe(node.Value, context);
            }

            protected internal override Void Accept(CharVal.Inner.x40 node, StringBuilder context)
            {
                return x40Transcriber.Instance.Transcribe(node.Value, context);
            }

            protected internal override Void Accept(CharVal.Inner.x41 node, StringBuilder context)
            {
                return x41Transcriber.Instance.Transcribe(node.Value, context);
            }

            protected internal override Void Accept(CharVal.Inner.x42 node, StringBuilder context)
            {
                return x42Transcriber.Instance.Transcribe(node.Value, context);
            }

            protected internal override Void Accept(CharVal.Inner.x43 node, StringBuilder context)
            {
                return x43Transcriber.Instance.Transcribe(node.Value, context);
            }

            protected internal override Void Accept(CharVal.Inner.x44 node, StringBuilder context)
            {
                return x44Transcriber.Instance.Transcribe(node.Value, context);
            }

            protected internal override Void Accept(CharVal.Inner.x45 node, StringBuilder context)
            {
                return x45Transcriber.Instance.Transcribe(node.Value, context);
            }

            protected internal override Void Accept(CharVal.Inner.x46 node, StringBuilder context)
            {
                return x46Transcriber.Instance.Transcribe(node.Value, context);
            }

            protected internal override Void Accept(CharVal.Inner.x47 node, StringBuilder context)
            {
                return x47Transcriber.Instance.Transcribe(node.Value, context);
            }

            protected internal override Void Accept(CharVal.Inner.x48 node, StringBuilder context)
            {
                return x48Transcriber.Instance.Transcribe(node.Value, context);
            }

            protected internal override Void Accept(CharVal.Inner.x49 node, StringBuilder context)
            {
                return x49Transcriber.Instance.Transcribe(node.Value, context);
            }

            protected internal override Void Accept(CharVal.Inner.x4A node, StringBuilder context)
            {
                return x4ATranscriber.Instance.Transcribe(node.Value, context);
            }

            protected internal override Void Accept(CharVal.Inner.x4B node, StringBuilder context)
            {
                return x4BTranscriber.Instance.Transcribe(node.Value, context);
            }

            protected internal override Void Accept(CharVal.Inner.x4C node, StringBuilder context)
            {
                return x4CTranscriber.Instance.Transcribe(node.Value, context);
            }

            protected internal override Void Accept(CharVal.Inner.x4D node, StringBuilder context)
            {
                return x4DTranscriber.Instance.Transcribe(node.Value, context);
            }

            protected internal override Void Accept(CharVal.Inner.x4E node, StringBuilder context)
            {
                return x4ETranscriber.Instance.Transcribe(node.Value, context);
            }

            protected internal override Void Accept(CharVal.Inner.x4F node, StringBuilder context)
            {
                return x4FTranscriber.Instance.Transcribe(node.Value, context);
            }

            protected internal override Void Accept(CharVal.Inner.x50 node, StringBuilder context)
            {
                return x50Transcriber.Instance.Transcribe(node.Value, context);
            }

            protected internal override Void Accept(CharVal.Inner.x51 node, StringBuilder context)
            {
                return x51Transcriber.Instance.Transcribe(node.Value, context);
            }

            protected internal override Void Accept(CharVal.Inner.x52 node, StringBuilder context)
            {
                return x52Transcriber.Instance.Transcribe(node.Value, context);
            }

            protected internal override Void Accept(CharVal.Inner.x53 node, StringBuilder context)
            {
                return x53Transcriber.Instance.Transcribe(node.Value, context);
            }

            protected internal override Void Accept(CharVal.Inner.x54 node, StringBuilder context)
            {
                return x54Transcriber.Instance.Transcribe(node.Value, context);
            }

            protected internal override Void Accept(CharVal.Inner.x55 node, StringBuilder context)
            {
                return x55Transcriber.Instance.Transcribe(node.Value, context);
            }

            protected internal override Void Accept(CharVal.Inner.x56 node, StringBuilder context)
            {
                return x56Transcriber.Instance.Transcribe(node.Value, context);
            }

            protected internal override Void Accept(CharVal.Inner.x57 node, StringBuilder context)
            {
                return x57Transcriber.Instance.Transcribe(node.Value, context);
            }

            protected internal override Void Accept(CharVal.Inner.x58 node, StringBuilder context)
            {
                return x58Transcriber.Instance.Transcribe(node.Value, context);
            }

            protected internal override Void Accept(CharVal.Inner.x59 node, StringBuilder context)
            {
                return x59Transcriber.Instance.Transcribe(node.Value, context);
            }

            protected internal override Void Accept(CharVal.Inner.x5A node, StringBuilder context)
            {
                return x5ATranscriber.Instance.Transcribe(node.Value, context);
            }

            protected internal override Void Accept(CharVal.Inner.x5B node, StringBuilder context)
            {
                return x5BTranscriber.Instance.Transcribe(node.Value, context);
            }

            protected internal override Void Accept(CharVal.Inner.x5C node, StringBuilder context)
            {
                return x5CTranscriber.Instance.Transcribe(node.Value, context);
            }

            protected internal override Void Accept(CharVal.Inner.x5D node, StringBuilder context)
            {
                return x5DTranscriber.Instance.Transcribe(node.Value, context);
            }

            protected internal override Void Accept(CharVal.Inner.x5E node, StringBuilder context)
            {
                return x5ETranscriber.Instance.Transcribe(node.Value, context);
            }

            protected internal override Void Accept(CharVal.Inner.x5F node, StringBuilder context)
            {
                return x5FTranscriber.Instance.Transcribe(node.Value, context);
            }

            protected internal override Void Accept(CharVal.Inner.x60 node, StringBuilder context)
            {
                return x60Transcriber.Instance.Transcribe(node.Value, context);
            }

            protected internal override Void Accept(CharVal.Inner.x61 node, StringBuilder context)
            {
                return x61Transcriber.Instance.Transcribe(node.Value, context);
            }

            protected internal override Void Accept(CharVal.Inner.x62 node, StringBuilder context)
            {
                return x62Transcriber.Instance.Transcribe(node.Value, context);
            }

            protected internal override Void Accept(CharVal.Inner.x63 node, StringBuilder context)
            {
                return x63Transcriber.Instance.Transcribe(node.Value, context);
            }

            protected internal override Void Accept(CharVal.Inner.x64 node, StringBuilder context)
            {
                return x64Transcriber.Instance.Transcribe(node.Value, context);
            }

            protected internal override Void Accept(CharVal.Inner.x65 node, StringBuilder context)
            {
                return x65Transcriber.Instance.Transcribe(node.Value, context);
            }

            protected internal override Void Accept(CharVal.Inner.x66 node, StringBuilder context)
            {
                return x66Transcriber.Instance.Transcribe(node.Value, context);
            }

            protected internal override Void Accept(CharVal.Inner.x67 node, StringBuilder context)
            {
                return x67Transcriber.Instance.Transcribe(node.Value, context);
            }

            protected internal override Void Accept(CharVal.Inner.x68 node, StringBuilder context)
            {
                return x68Transcriber.Instance.Transcribe(node.Value, context);
            }

            protected internal override Void Accept(CharVal.Inner.x69 node, StringBuilder context)
            {
                return x69Transcriber.Instance.Transcribe(node.Value, context);
            }

            protected internal override Void Accept(CharVal.Inner.x6A node, StringBuilder context)
            {
                return x6ATranscriber.Instance.Transcribe(node.Value, context);
            }

            protected internal override Void Accept(CharVal.Inner.x6B node, StringBuilder context)
            {
                return x6BTranscriber.Instance.Transcribe(node.Value, context);
            }

            protected internal override Void Accept(CharVal.Inner.x6C node, StringBuilder context)
            {
                return x6CTranscriber.Instance.Transcribe(node.Value, context);
            }

            protected internal override Void Accept(CharVal.Inner.x6D node, StringBuilder context)
            {
                return x6DTranscriber.Instance.Transcribe(node.Value, context);
            }

            protected internal override Void Accept(CharVal.Inner.x6E node, StringBuilder context)
            {
                return x6ETranscriber.Instance.Transcribe(node.Value, context);
            }

            protected internal override Void Accept(CharVal.Inner.x6F node, StringBuilder context)
            {
                return x6FTranscriber.Instance.Transcribe(node.Value, context);
            }

            protected internal override Void Accept(CharVal.Inner.x70 node, StringBuilder context)
            {
                return x70Transcriber.Instance.Transcribe(node.Value, context);
            }

            protected internal override Void Accept(CharVal.Inner.x71 node, StringBuilder context)
            {
                return x71Transcriber.Instance.Transcribe(node.Value, context);
            }

            protected internal override Void Accept(CharVal.Inner.x72 node, StringBuilder context)
            {
                return x72Transcriber.Instance.Transcribe(node.Value, context);
            }

            protected internal override Void Accept(CharVal.Inner.x73 node, StringBuilder context)
            {
                return x73Transcriber.Instance.Transcribe(node.Value, context);
            }

            protected internal override Void Accept(CharVal.Inner.x74 node, StringBuilder context)
            {
                return x74Transcriber.Instance.Transcribe(node.Value, context);
            }

            protected internal override Void Accept(CharVal.Inner.x75 node, StringBuilder context)
            {
                return x75Transcriber.Instance.Transcribe(node.Value, context);
            }

            protected internal override Void Accept(CharVal.Inner.x76 node, StringBuilder context)
            {
                return x76Transcriber.Instance.Transcribe(node.Value, context);
            }

            protected internal override Void Accept(CharVal.Inner.x77 node, StringBuilder context)
            {
                return x77Transcriber.Instance.Transcribe(node.Value, context);
            }

            protected internal override Void Accept(CharVal.Inner.x78 node, StringBuilder context)
            {
                return x78Transcriber.Instance.Transcribe(node.Value, context);
            }

            protected internal override Void Accept(CharVal.Inner.x79 node, StringBuilder context)
            {
                return x79Transcriber.Instance.Transcribe(node.Value, context);
            }

            protected internal override Void Accept(CharVal.Inner.x7A node, StringBuilder context)
            {
                return x7ATranscriber.Instance.Transcribe(node.Value, context);
            }

            protected internal override Void Accept(CharVal.Inner.x7B node, StringBuilder context)
            {
                return x7BTranscriber.Instance.Transcribe(node.Value, context);
            }

            protected internal override Void Accept(CharVal.Inner.x7C node, StringBuilder context)
            {
                return x7CTranscriber.Instance.Transcribe(node.Value, context);
            }

            protected internal override Void Accept(CharVal.Inner.x7D node, StringBuilder context)
            {
                return x7DTranscriber.Instance.Transcribe(node.Value, context);
            }

            protected internal override Void Accept(CharVal.Inner.x7E node, StringBuilder context)
            {
                return x7ETranscriber.Instance.Transcribe(node.Value, context);
            }

        }

    }
}
