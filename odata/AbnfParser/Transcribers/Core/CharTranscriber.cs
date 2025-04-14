namespace AbnfParser.Transcribers.Core
{
    using System.Text;

    using AbnfParser.CstNodes.Core;
    using Root;

    public sealed class CharTranscriber : Char.Visitor<Void, StringBuilder>
    {
        private CharTranscriber()
        {
        }

        public static CharTranscriber Instance { get; } = new CharTranscriber();

        protected internal override Void Accept(Char.x01 node, StringBuilder context)
        {
            return x01Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x02 node, StringBuilder context)
        {
            return x02Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x03 node, StringBuilder context)
        {
            return x03Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x04 node, StringBuilder context)
        {
            return x04Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x05 node, StringBuilder context)
        {
            return x05Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x06 node, StringBuilder context)
        {
            return x06Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x07 node, StringBuilder context)
        {
            return x07Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x08 node, StringBuilder context)
        {
            return x08Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x09 node, StringBuilder context)
        {
            return x09Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x0A node, StringBuilder context)
        {
            return x0ATranscriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x0B node, StringBuilder context)
        {
            return x0BTranscriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x0C node, StringBuilder context)
        {
            return x0CTranscriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x0D node, StringBuilder context)
        {
            return x0DTranscriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x0E node, StringBuilder context)
        {
            return x0ETranscriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x0F node, StringBuilder context)
        {
            return x0FTranscriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x10 node, StringBuilder context)
        {
            return x10Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x11 node, StringBuilder context)
        {
            return x11Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x12 node, StringBuilder context)
        {
            return x12Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x13 node, StringBuilder context)
        {
            return x13Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x14 node, StringBuilder context)
        {
            return x14Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x15 node, StringBuilder context)
        {
            return x15Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x16 node, StringBuilder context)
        {
            return x16Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x17 node, StringBuilder context)
        {
            return x17Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x18 node, StringBuilder context)
        {
            return x18Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x19 node, StringBuilder context)
        {
            return x19Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x1A node, StringBuilder context)
        {
            return x1ATranscriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x1B node, StringBuilder context)
        {
            return x1BTranscriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x1C node, StringBuilder context)
        {
            return x1CTranscriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x1D node, StringBuilder context)
        {
            return x1DTranscriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x1E node, StringBuilder context)
        {
            return x1ETranscriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x1F node, StringBuilder context)
        {
            return x1FTranscriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x20 node, StringBuilder context)
        {
            return x20Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x21 node, StringBuilder context)
        {
            return x21Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x22 node, StringBuilder context)
        {
            return x22Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x23 node, StringBuilder context)
        {
            return x23Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x24 node, StringBuilder context)
        {
            return x24Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x25 node, StringBuilder context)
        {
            return x25Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x26 node, StringBuilder context)
        {
            return x26Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x27 node, StringBuilder context)
        {
            return x27Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x28 node, StringBuilder context)
        {
            return x28Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x29 node, StringBuilder context)
        {
            return x29Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x2A node, StringBuilder context)
        {
            return x2ATranscriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x2B node, StringBuilder context)
        {
            return x2BTranscriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x2C node, StringBuilder context)
        {
            return x2CTranscriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x2D node, StringBuilder context)
        {
            return x2DTranscriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x2E node, StringBuilder context)
        {
            return x2ETranscriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x2F node, StringBuilder context)
        {
            return x2FTranscriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x30 node, StringBuilder context)
        {
            return x30Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x31 node, StringBuilder context)
        {
            return x31Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x32 node, StringBuilder context)
        {
            return x32Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x33 node, StringBuilder context)
        {
            return x33Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x34 node, StringBuilder context)
        {
            return x34Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x35 node, StringBuilder context)
        {
            return x35Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x36 node, StringBuilder context)
        {
            return x36Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x37 node, StringBuilder context)
        {
            return x37Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x38 node, StringBuilder context)
        {
            return x38Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x39 node, StringBuilder context)
        {
            return x39Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x3A node, StringBuilder context)
        {
            return x3ATranscriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x3B node, StringBuilder context)
        {
            return x3BTranscriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x3C node, StringBuilder context)
        {
            return x3CTranscriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x3D node, StringBuilder context)
        {
            return x3DTranscriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x3E node, StringBuilder context)
        {
            return x3ETranscriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x3F node, StringBuilder context)
        {
            return x3FTranscriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x40 node, StringBuilder context)
        {
            return x40Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x41 node, StringBuilder context)
        {
            return x41Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x42 node, StringBuilder context)
        {
            return x42Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x43 node, StringBuilder context)
        {
            return x43Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x44 node, StringBuilder context)
        {
            return x44Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x45 node, StringBuilder context)
        {
            return x45Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x46 node, StringBuilder context)
        {
            return x46Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x47 node, StringBuilder context)
        {
            return x47Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x48 node, StringBuilder context)
        {
            return x48Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x49 node, StringBuilder context)
        {
            return x49Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x4A node, StringBuilder context)
        {
            return x4ATranscriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x4B node, StringBuilder context)
        {
            return x4BTranscriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x4C node, StringBuilder context)
        {
            return x4CTranscriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x4D node, StringBuilder context)
        {
            return x4DTranscriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x4E node, StringBuilder context)
        {
            return x4ETranscriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x4F node, StringBuilder context)
        {
            return x4FTranscriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x50 node, StringBuilder context)
        {
            return x50Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x51 node, StringBuilder context)
        {
            return x51Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x52 node, StringBuilder context)
        {
            return x52Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x53 node, StringBuilder context)
        {
            return x53Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x54 node, StringBuilder context)
        {
            return x54Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x55 node, StringBuilder context)
        {
            return x55Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x56 node, StringBuilder context)
        {
            return x56Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x57 node, StringBuilder context)
        {
            return x57Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x58 node, StringBuilder context)
        {
            return x58Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x59 node, StringBuilder context)
        {
            return x59Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x5A node, StringBuilder context)
        {
            return x5ATranscriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x5B node, StringBuilder context)
        {
            return x5BTranscriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x5C node, StringBuilder context)
        {
            return x5CTranscriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x5D node, StringBuilder context)
        {
            return x5DTranscriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x5E node, StringBuilder context)
        {
            return x5ETranscriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x5F node, StringBuilder context)
        {
            return x5FTranscriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x60 node, StringBuilder context)
        {
            return x60Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x61 node, StringBuilder context)
        {
            return x61Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x62 node, StringBuilder context)
        {
            return x62Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x63 node, StringBuilder context)
        {
            return x63Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x64 node, StringBuilder context)
        {
            return x64Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x65 node, StringBuilder context)
        {
            return x65Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x66 node, StringBuilder context)
        {
            return x66Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x67 node, StringBuilder context)
        {
            return x67Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x68 node, StringBuilder context)
        {
            return x68Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x69 node, StringBuilder context)
        {
            return x69Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x6A node, StringBuilder context)
        {
            return x6ATranscriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x6B node, StringBuilder context)
        {
            return x6BTranscriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x6C node, StringBuilder context)
        {
            return x6CTranscriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x6D node, StringBuilder context)
        {
            return x6DTranscriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x6E node, StringBuilder context)
        {
            return x6ETranscriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x6F node, StringBuilder context)
        {
            return x6FTranscriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x70 node, StringBuilder context)
        {
            return x70Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x71 node, StringBuilder context)
        {
            return x71Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x72 node, StringBuilder context)
        {
            return x72Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x73 node, StringBuilder context)
        {
            return x73Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x74 node, StringBuilder context)
        {
            return x74Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x75 node, StringBuilder context)
        {
            return x75Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x76 node, StringBuilder context)
        {
            return x76Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x77 node, StringBuilder context)
        {
            return x77Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x78 node, StringBuilder context)
        {
            return x78Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x79 node, StringBuilder context)
        {
            return x79Transcriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x7A node, StringBuilder context)
        {
            return x7ATranscriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x7B node, StringBuilder context)
        {
            return x7BTranscriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x7C node, StringBuilder context)
        {
            return x7CTranscriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x7D node, StringBuilder context)
        {
            return x7DTranscriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x7E node, StringBuilder context)
        {
            return x7ETranscriber.Instance.Transcribe(node.Value, context);
        }

        protected internal override Void Accept(Char.x7F node, StringBuilder context)
        {
            return x7FTranscriber.Instance.Transcribe(node.Value, context);
        }

    }
}
