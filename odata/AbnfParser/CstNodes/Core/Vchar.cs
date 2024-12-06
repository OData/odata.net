namespace AbnfParser.CstNodes.Core
{
    public abstract class Vchar
    {
        private Vchar()
        {
        }

        public abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);

        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(Vchar node, TContext context)
            {
                return node.Dispatch(this, context);
            }

            protected internal abstract TResult Accept(x21 node, TContext context);
            protected internal abstract TResult Accept(x22 node, TContext context);
            protected internal abstract TResult Accept(x23 node, TContext context);
            protected internal abstract TResult Accept(x24 node, TContext context);
            protected internal abstract TResult Accept(x25 node, TContext context);
            protected internal abstract TResult Accept(x26 node, TContext context);
            protected internal abstract TResult Accept(x27 node, TContext context);
            protected internal abstract TResult Accept(x28 node, TContext context);
            protected internal abstract TResult Accept(x29 node, TContext context);
            protected internal abstract TResult Accept(x2A node, TContext context);
            protected internal abstract TResult Accept(x2B node, TContext context);
            protected internal abstract TResult Accept(x2C node, TContext context);
            protected internal abstract TResult Accept(x2D node, TContext context);
            protected internal abstract TResult Accept(x2E node, TContext context);
            protected internal abstract TResult Accept(x2F node, TContext context);
            protected internal abstract TResult Accept(x30 node, TContext context);
            protected internal abstract TResult Accept(x31 node, TContext context);
            protected internal abstract TResult Accept(x32 node, TContext context);
            protected internal abstract TResult Accept(x33 node, TContext context);
            protected internal abstract TResult Accept(x34 node, TContext context);
            protected internal abstract TResult Accept(x35 node, TContext context);
            protected internal abstract TResult Accept(x36 node, TContext context);
            protected internal abstract TResult Accept(x37 node, TContext context);
            protected internal abstract TResult Accept(x38 node, TContext context);
            protected internal abstract TResult Accept(x39 node, TContext context);
            protected internal abstract TResult Accept(x3A node, TContext context);
            protected internal abstract TResult Accept(x3B node, TContext context);
            protected internal abstract TResult Accept(x3C node, TContext context);
            protected internal abstract TResult Accept(x3D node, TContext context);
            protected internal abstract TResult Accept(x3E node, TContext context);
            protected internal abstract TResult Accept(x3F node, TContext context);
            protected internal abstract TResult Accept(x40 node, TContext context);
            protected internal abstract TResult Accept(x41 node, TContext context);
            protected internal abstract TResult Accept(x42 node, TContext context);
            protected internal abstract TResult Accept(x43 node, TContext context);
            protected internal abstract TResult Accept(x44 node, TContext context);
            protected internal abstract TResult Accept(x45 node, TContext context);
            protected internal abstract TResult Accept(x46 node, TContext context);
            protected internal abstract TResult Accept(x47 node, TContext context);
            protected internal abstract TResult Accept(x48 node, TContext context);
            protected internal abstract TResult Accept(x49 node, TContext context);
            protected internal abstract TResult Accept(x4A node, TContext context);
            protected internal abstract TResult Accept(x4B node, TContext context);
            protected internal abstract TResult Accept(x4C node, TContext context);
            protected internal abstract TResult Accept(x4D node, TContext context);
            protected internal abstract TResult Accept(x4E node, TContext context);
            protected internal abstract TResult Accept(x4F node, TContext context);
            protected internal abstract TResult Accept(x50 node, TContext context);
            protected internal abstract TResult Accept(x51 node, TContext context);
            protected internal abstract TResult Accept(x52 node, TContext context);
            protected internal abstract TResult Accept(x53 node, TContext context);
            protected internal abstract TResult Accept(x54 node, TContext context);
            protected internal abstract TResult Accept(x55 node, TContext context);
            protected internal abstract TResult Accept(x56 node, TContext context);
            protected internal abstract TResult Accept(x57 node, TContext context);
            protected internal abstract TResult Accept(x58 node, TContext context);
            protected internal abstract TResult Accept(x59 node, TContext context);
            protected internal abstract TResult Accept(x5A node, TContext context);
            protected internal abstract TResult Accept(x5B node, TContext context);
            protected internal abstract TResult Accept(x5C node, TContext context);
            protected internal abstract TResult Accept(x5D node, TContext context);
            protected internal abstract TResult Accept(x5E node, TContext context);
            protected internal abstract TResult Accept(x5F node, TContext context);
            protected internal abstract TResult Accept(x60 node, TContext context);
            protected internal abstract TResult Accept(x61 node, TContext context);
            protected internal abstract TResult Accept(x62 node, TContext context);
            protected internal abstract TResult Accept(x63 node, TContext context);
            protected internal abstract TResult Accept(x64 node, TContext context);
            protected internal abstract TResult Accept(x65 node, TContext context);
            protected internal abstract TResult Accept(x66 node, TContext context);
            protected internal abstract TResult Accept(x67 node, TContext context);
            protected internal abstract TResult Accept(x68 node, TContext context);
            protected internal abstract TResult Accept(x69 node, TContext context);
            protected internal abstract TResult Accept(x6A node, TContext context);
            protected internal abstract TResult Accept(x6B node, TContext context);
            protected internal abstract TResult Accept(x6C node, TContext context);
            protected internal abstract TResult Accept(x6D node, TContext context);
            protected internal abstract TResult Accept(x6E node, TContext context);
            protected internal abstract TResult Accept(x6F node, TContext context);
            protected internal abstract TResult Accept(x70 node, TContext context);
            protected internal abstract TResult Accept(x71 node, TContext context);
            protected internal abstract TResult Accept(x72 node, TContext context);
            protected internal abstract TResult Accept(x73 node, TContext context);
            protected internal abstract TResult Accept(x74 node, TContext context);
            protected internal abstract TResult Accept(x75 node, TContext context);
            protected internal abstract TResult Accept(x76 node, TContext context);
            protected internal abstract TResult Accept(x77 node, TContext context);
            protected internal abstract TResult Accept(x78 node, TContext context);
            protected internal abstract TResult Accept(x79 node, TContext context);
            protected internal abstract TResult Accept(x7A node, TContext context);
            protected internal abstract TResult Accept(x7B node, TContext context);
            protected internal abstract TResult Accept(x7C node, TContext context);
            protected internal abstract TResult Accept(x7D node, TContext context);
            protected internal abstract TResult Accept(x7E node, TContext context);
        }

        public sealed class x21 : Vchar
        {
            public x21(Core.x21 value)
            {
                Value = value;
            }

            public Core.x21 Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x22 : Vchar
        {
            public x22(Core.x22 value)
            {
                Value = value;
            }

            public Core.x22 Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x23 : Vchar
        {
            public x23(Core.x23 value)
            {
                Value = value;
            }

            public Core.x23 Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x24 : Vchar
        {
            public x24(Core.x24 value)
            {
                Value = value;
            }

            public Core.x24 Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x25 : Vchar
        {
            public x25(Core.x25 value)
            {
                Value = value;
            }

            public Core.x25 Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x26 : Vchar
        {
            public x26(Core.x26 value)
            {
                Value = value;
            }

            public Core.x26 Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x27 : Vchar
        {
            public x27(Core.x27 value)
            {
                Value = value;
            }

            public Core.x27 Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x28 : Vchar
        {
            public x28(Core.x28 value)
            {
                Value = value;
            }

            public Core.x28 Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x29 : Vchar
        {
            public x29(Core.x29 value)
            {
                Value = value;
            }

            public Core.x29 Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x2A : Vchar
        {
            public x2A(Core.x2A value)
            {
                Value = value;
            }

            public Core.x2A Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x2B : Vchar
        {
            public x2B(Core.x2B value)
            {
                Value = value;
            }

            public Core.x2B Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x2C : Vchar
        {
            public x2C(Core.x2C value)
            {
                Value = value;
            }

            public Core.x2C Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x2D : Vchar
        {
            public x2D(Core.x2D value)
            {
                Value = value;
            }

            public Core.x2D Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x2E : Vchar
        {
            public x2E(Core.x2E value)
            {
                Value = value;
            }

            public Core.x2E Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x2F : Vchar
        {
            public x2F(Core.x2F value)
            {
                Value = value;
            }

            public Core.x2F Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x30 : Vchar
        {
            public x30(Core.x30 value)
            {
                Value = value;
            }

            public Core.x30 Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x31 : Vchar
        {
            public x31(Core.x31 value)
            {
                Value = value;
            }

            public Core.x31 Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x32 : Vchar
        {
            public x32(Core.x32 value)
            {
                Value = value;
            }

            public Core.x32 Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x33 : Vchar
        {
            public x33(Core.x33 value)
            {
                Value = value;
            }

            public Core.x33 Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x34 : Vchar
        {
            public x34(Core.x34 value)
            {
                Value = value;
            }

            public Core.x34 Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x35 : Vchar
        {
            public x35(Core.x35 value)
            {
                Value = value;
            }

            public Core.x35 Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x36 : Vchar
        {
            public x36(Core.x36 value)
            {
                Value = value;
            }

            public Core.x36 Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x37 : Vchar
        {
            public x37(Core.x37 value)
            {
                Value = value;
            }

            public Core.x37 Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x38 : Vchar
        {
            public x38(Core.x38 value)
            {
                Value = value;
            }

            public Core.x38 Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x39 : Vchar
        {
            public x39(Core.x39 value)
            {
                Value = value;
            }

            public Core.x39 Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x3A : Vchar
        {
            public x3A(Core.x3A value)
            {
                Value = value;
            }

            public Core.x3A Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x3B : Vchar
        {
            public x3B(Core.x3B value)
            {
                Value = value;
            }

            public Core.x3B Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x3C : Vchar
        {
            public x3C(Core.x3C value)
            {
                Value = value;
            }

            public Core.x3C Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x3D : Vchar
        {
            public x3D(Core.x3D value)
            {
                Value = value;
            }

            public Core.x3D Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x3E : Vchar
        {
            public x3E(Core.x3E value)
            {
                Value = value;
            }

            public Core.x3E Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x3F : Vchar
        {
            public x3F(Core.x3F value)
            {
                Value = value;
            }

            public Core.x3F Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x40 : Vchar
        {
            public x40(Core.x40 value)
            {
                Value = value;
            }

            public Core.x40 Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x41 : Vchar
        {
            public x41(Core.x41 value)
            {
                Value = value;
            }

            public Core.x41 Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x42 : Vchar
        {
            public x42(Core.x42 value)
            {
                Value = value;
            }

            public Core.x42 Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x43 : Vchar
        {
            public x43(Core.x43 value)
            {
                Value = value;
            }

            public Core.x43 Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x44 : Vchar
        {
            public x44(Core.x44 value)
            {
                Value = value;
            }

            public Core.x44 Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x45 : Vchar
        {
            public x45(Core.x45 value)
            {
                Value = value;
            }

            public Core.x45 Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x46 : Vchar
        {
            public x46(Core.x46 value)
            {
                Value = value;
            }

            public Core.x46 Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x47 : Vchar
        {
            public x47(Core.x47 value)
            {
                Value = value;
            }

            public Core.x47 Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x48 : Vchar
        {
            public x48(Core.x48 value)
            {
                Value = value;
            }

            public Core.x48 Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x49 : Vchar
        {
            public x49(Core.x49 value)
            {
                Value = value;
            }

            public Core.x49 Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x4A : Vchar
        {
            public x4A(Core.x4A value)
            {
                Value = value;
            }

            public Core.x4A Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x4B : Vchar
        {
            public x4B(Core.x4B value)
            {
                Value = value;
            }

            public Core.x4B Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x4C : Vchar
        {
            public x4C(Core.x4C value)
            {
                Value = value;
            }

            public Core.x4C Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x4D : Vchar
        {
            public x4D(Core.x4D value)
            {
                Value = value;
            }

            public Core.x4D Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x4E : Vchar
        {
            public x4E(Core.x4E value)
            {
                Value = value;
            }

            public Core.x4E Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x4F : Vchar
        {
            public x4F(Core.x4F value)
            {
                Value = value;
            }

            public Core.x4F Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x50 : Vchar
        {
            public x50(Core.x50 value)
            {
                Value = value;
            }

            public Core.x50 Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x51 : Vchar
        {
            public x51(Core.x51 value)
            {
                Value = value;
            }

            public Core.x51 Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x52 : Vchar
        {
            public x52(Core.x52 value)
            {
                Value = value;
            }

            public Core.x52 Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x53 : Vchar
        {
            public x53(Core.x53 value)
            {
                Value = value;
            }

            public Core.x53 Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x54 : Vchar
        {
            public x54(Core.x54 value)
            {
                Value = value;
            }

            public Core.x54 Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x55 : Vchar
        {
            public x55(Core.x55 value)
            {
                Value = value;
            }

            public Core.x55 Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x56 : Vchar
        {
            public x56(Core.x56 value)
            {
                Value = value;
            }

            public Core.x56 Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x57 : Vchar
        {
            public x57(Core.x57 value)
            {
                Value = value;
            }

            public Core.x57 Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x58 : Vchar
        {
            public x58(Core.x58 value)
            {
                Value = value;
            }

            public Core.x58 Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x59 : Vchar
        {
            public x59(Core.x59 value)
            {
                Value = value;
            }

            public Core.x59 Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x5A : Vchar
        {
            public x5A(Core.x5A value)
            {
                Value = value;
            }

            public Core.x5A Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x5B : Vchar
        {
            public x5B(Core.x5B value)
            {
                Value = value;
            }

            public Core.x5B Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x5C : Vchar
        {
            public x5C(Core.x5C value)
            {
                Value = value;
            }

            public Core.x5C Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x5D : Vchar
        {
            public x5D(Core.x5D value)
            {
                Value = value;
            }

            public Core.x5D Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x5E : Vchar
        {
            public x5E(Core.x5E value)
            {
                Value = value;
            }

            public Core.x5E Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x5F : Vchar
        {
            public x5F(Core.x5F value)
            {
                Value = value;
            }

            public Core.x5F Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x60 : Vchar
        {
            public x60(Core.x60 value)
            {
                Value = value;
            }

            public Core.x60 Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x61 : Vchar
        {
            public x61(Core.x61 value)
            {
                Value = value;
            }

            public Core.x61 Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x62 : Vchar
        {
            public x62(Core.x62 value)
            {
                Value = value;
            }

            public Core.x62 Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x63 : Vchar
        {
            public x63(Core.x63 value)
            {
                Value = value;
            }

            public Core.x63 Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x64 : Vchar
        {
            public x64(Core.x64 value)
            {
                Value = value;
            }

            public Core.x64 Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x65 : Vchar
        {
            public x65(Core.x65 value)
            {
                Value = value;
            }

            public Core.x65 Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x66 : Vchar
        {
            public x66(Core.x66 value)
            {
                Value = value;
            }

            public Core.x66 Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x67 : Vchar
        {
            public x67(Core.x67 value)
            {
                Value = value;
            }

            public Core.x67 Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x68 : Vchar
        {
            public x68(Core.x68 value)
            {
                Value = value;
            }

            public Core.x68 Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x69 : Vchar
        {
            public x69(Core.x69 value)
            {
                Value = value;
            }

            public Core.x69 Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x6A : Vchar
        {
            public x6A(Core.x6A value)
            {
                Value = value;
            }

            public Core.x6A Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x6B : Vchar
        {
            public x6B(Core.x6B value)
            {
                Value = value;
            }

            public Core.x6B Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x6C : Vchar
        {
            public x6C(Core.x6C value)
            {
                Value = value;
            }

            public Core.x6C Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x6D : Vchar
        {
            public x6D(Core.x6D value)
            {
                Value = value;
            }

            public Core.x6D Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x6E : Vchar
        {
            public x6E(Core.x6E value)
            {
                Value = value;
            }

            public Core.x6E Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x6F : Vchar
        {
            public x6F(Core.x6F value)
            {
                Value = value;
            }

            public Core.x6F Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x70 : Vchar
        {
            public x70(Core.x70 value)
            {
                Value = value;
            }

            public Core.x70 Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x71 : Vchar
        {
            public x71(Core.x71 value)
            {
                Value = value;
            }

            public Core.x71 Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x72 : Vchar
        {
            public x72(Core.x72 value)
            {
                Value = value;
            }

            public Core.x72 Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x73 : Vchar
        {
            public x73(Core.x73 value)
            {
                Value = value;
            }

            public Core.x73 Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x74 : Vchar
        {
            public x74(Core.x74 value)
            {
                Value = value;
            }

            public Core.x74 Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x75 : Vchar
        {
            public x75(Core.x75 value)
            {
                Value = value;
            }

            public Core.x75 Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x76 : Vchar
        {
            public x76(Core.x76 value)
            {
                Value = value;
            }

            public Core.x76 Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x77 : Vchar
        {
            public x77(Core.x77 value)
            {
                Value = value;
            }

            public Core.x77 Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x78 : Vchar
        {
            public x78(Core.x78 value)
            {
                Value = value;
            }

            public Core.x78 Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x79 : Vchar
        {
            public x79(Core.x79 value)
            {
                Value = value;
            }

            public Core.x79 Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x7A : Vchar
        {
            public x7A(Core.x7A value)
            {
                Value = value;
            }

            public Core.x7A Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x7B : Vchar
        {
            public x7B(Core.x7B value)
            {
                Value = value;
            }

            public Core.x7B Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x7C : Vchar
        {
            public x7C(Core.x7C value)
            {
                Value = value;
            }

            public Core.x7C Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x7D : Vchar
        {
            public x7D(Core.x7D value)
            {
                Value = value;
            }

            public Core.x7D Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x7E : Vchar
        {
            public x7E(Core.x7E value)
            {
                Value = value;
            }

            public Core.x7E Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
}
