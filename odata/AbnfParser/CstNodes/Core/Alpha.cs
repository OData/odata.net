namespace AbnfParser.CstNodes.Core
{
    public abstract class Alpha
    {
        private Alpha()
        {
        }

        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);

        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(Alpha node, TContext context)
            {
                return node.Dispatch(this, context);
            }

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
        }

        public sealed class x41 : Alpha
        {
            public x41(AbnfParser.CstNodes.Core.x41 value)
            {
                Value = value;
            }

            public Core.x41 Value { get; }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x42 : Alpha
        {
            public x42(AbnfParser.CstNodes.Core.x42 value)
            {
                Value = value;
            }

            public Core.x42 Value { get; }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x43 : Alpha
        {
            public x43(AbnfParser.CstNodes.Core.x43 value)
            {
                Value = value;
            }

            public Core.x43 Value { get; }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x44 : Alpha
        {
            public x44(AbnfParser.CstNodes.Core.x44 value)
            {
                Value = value;
            }

            public Core.x44 Value { get; }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x45 : Alpha
        {
            public x45(AbnfParser.CstNodes.Core.x45 value)
            {
                Value = value;
            }

            public Core.x45 Value { get; }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x46 : Alpha
        {
            public x46(AbnfParser.CstNodes.Core.x46 value)
            {
                Value = value;
            }

            public Core.x46 Value { get; }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x47 : Alpha
        {
            public x47(AbnfParser.CstNodes.Core.x47 value)
            {
                Value = value;
            }

            public Core.x47 Value { get; }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x48 : Alpha
        {
            public x48(AbnfParser.CstNodes.Core.x48 value)
            {
                Value = value;
            }

            public Core.x48 Value { get; }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x49 : Alpha
        {
            public x49(AbnfParser.CstNodes.Core.x49 value)
            {
                Value = value;
            }

            public Core.x49 Value { get; }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x4A : Alpha
        {
            public x4A(AbnfParser.CstNodes.Core.x4A value)
            {
                Value = value;
            }

            public Core.x4A Value { get; }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x4B : Alpha
        {
            public x4B(AbnfParser.CstNodes.Core.x4B value)
            {
                Value = value;
            }

            public Core.x4B Value { get; }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x4C : Alpha
        {
            public x4C(AbnfParser.CstNodes.Core.x4C value)
            {
                Value = value;
            }

            public Core.x4C Value { get; }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x4D : Alpha
        {
            public x4D(AbnfParser.CstNodes.Core.x4D value)
            {
                Value = value;
            }

            public Core.x4D Value { get; }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x4E : Alpha
        {
            public x4E(AbnfParser.CstNodes.Core.x4E value)
            {
                Value = value;
            }

            public Core.x4E Value { get; }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x4F : Alpha
        {
            public x4F(AbnfParser.CstNodes.Core.x4F value)
            {
                Value = value;
            }

            public Core.x4F Value { get; }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x50 : Alpha
        {
            public x50(AbnfParser.CstNodes.Core.x50 value)
            {
                Value = value;
            }

            public Core.x50 Value { get; }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x51 : Alpha
        {
            public x51(AbnfParser.CstNodes.Core.x51 value)
            {
                Value = value;
            }

            public Core.x51 Value { get; }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x52 : Alpha
        {
            public x52(AbnfParser.CstNodes.Core.x52 value)
            {
                Value = value;
            }

            public Core.x52 Value { get; }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x53 : Alpha
        {
            public x53(AbnfParser.CstNodes.Core.x53 value)
            {
                Value = value;
            }

            public Core.x53 Value { get; }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x54 : Alpha
        {
            public x54(AbnfParser.CstNodes.Core.x54 value)
            {
                Value = value;
            }

            public Core.x54 Value { get; }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x55 : Alpha
        {
            public x55(AbnfParser.CstNodes.Core.x55 value)
            {
                Value = value;
            }

            public Core.x55 Value { get; }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x56 : Alpha
        {
            public x56(AbnfParser.CstNodes.Core.x56 value)
            {
                Value = value;
            }

            public Core.x56 Value { get; }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x57 : Alpha
        {
            public x57(AbnfParser.CstNodes.Core.x57 value)
            {
                Value = value;
            }

            public Core.x57 Value { get; }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x58 : Alpha
        {
            public x58(AbnfParser.CstNodes.Core.x58 value)
            {
                Value = value;
            }

            public Core.x58 Value { get; }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x59 : Alpha
        {
            public x59(AbnfParser.CstNodes.Core.x59 value)
            {
                Value = value;
            }

            public Core.x59 Value { get; }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x5A : Alpha
        {
            public x5A(AbnfParser.CstNodes.Core.x5A value)
            {
                Value = value;
            }

            public Core.x5A Value { get; }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x61 : Alpha
        {
            public x61(AbnfParser.CstNodes.Core.x61 value)
            {
                Value = value;
            }

            public Core.x61 Value { get; }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x62 : Alpha
        {
            public x62(AbnfParser.CstNodes.Core.x62 value)
            {
                Value = value;
            }

            public Core.x62 Value { get; }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x63 : Alpha
        {
            public x63(AbnfParser.CstNodes.Core.x63 value)
            {
                Value = value;
            }

            public Core.x63 Value { get; }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x64 : Alpha
        {
            public x64(AbnfParser.CstNodes.Core.x64 value)
            {
                Value = value;
            }

            public Core.x64 Value { get; }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x65 : Alpha
        {
            public x65(AbnfParser.CstNodes.Core.x65 value)
            {
                Value = value;
            }

            public Core.x65 Value { get; }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x66 : Alpha
        {
            public x66(AbnfParser.CstNodes.Core.x66 value)
            {
                Value = value;
            }

            public Core.x66 Value { get; }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x67 : Alpha
        {
            public x67(AbnfParser.CstNodes.Core.x67 value)
            {
                Value = value;
            }

            public Core.x67 Value { get; }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x68 : Alpha
        {
            public x68(AbnfParser.CstNodes.Core.x68 value)
            {
                Value = value;
            }

            public Core.x68 Value { get; }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x69 : Alpha
        {
            public x69(AbnfParser.CstNodes.Core.x69 value)
            {
                Value = value;
            }

            public Core.x69 Value { get; }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x6A : Alpha
        {
            public x6A(AbnfParser.CstNodes.Core.x6A value)
            {
                Value = value;
            }

            public Core.x6A Value { get; }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x6B : Alpha
        {
            public x6B(AbnfParser.CstNodes.Core.x6B value)
            {
                Value = value;
            }

            public Core.x6B Value { get; }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x6C : Alpha
        {
            public x6C(AbnfParser.CstNodes.Core.x6C value)
            {
                Value = value;
            }

            public Core.x6C Value { get; }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x6D : Alpha
        {
            public x6D(AbnfParser.CstNodes.Core.x6D value)
            {
                Value = value;
            }

            public Core.x6D Value { get; }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x6E : Alpha
        {
            public x6E(AbnfParser.CstNodes.Core.x6E value)
            {
                Value = value;
            }

            public Core.x6E Value { get; }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x6F : Alpha
        {
            public x6F(AbnfParser.CstNodes.Core.x6F value)
            {
                Value = value;
            }

            public Core.x6F Value { get; }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x70 : Alpha
        {
            public x70(AbnfParser.CstNodes.Core.x70 value)
            {
                Value = value;
            }

            public Core.x70 Value { get; }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x71 : Alpha
        {
            public x71(AbnfParser.CstNodes.Core.x71 value)
            {
                Value = value;
            }

            public Core.x71 Value { get; }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x72 : Alpha
        {
            public x72(AbnfParser.CstNodes.Core.x72 value)
            {
                Value = value;
            }

            public Core.x72 Value { get; }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x73 : Alpha
        {
            public x73(AbnfParser.CstNodes.Core.x73 value)
            {
                Value = value;
            }

            public Core.x73 Value { get; }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x74 : Alpha
        {
            public x74(AbnfParser.CstNodes.Core.x74 value)
            {
                Value = value;
            }

            public Core.x74 Value { get; }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x75 : Alpha
        {
            public x75(AbnfParser.CstNodes.Core.x75 value)
            {
                Value = value;
            }

            public Core.x75 Value { get; }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x76 : Alpha
        {
            public x76(AbnfParser.CstNodes.Core.x76 value)
            {
                Value = value;
            }

            public Core.x76 Value { get; }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x77 : Alpha
        {
            public x77(AbnfParser.CstNodes.Core.x77 value)
            {
                Value = value;
            }

            public Core.x77 Value { get; }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x78 : Alpha
        {
            public x78(AbnfParser.CstNodes.Core.x78 value)
            {
                Value = value;
            }

            public Core.x78 Value { get; }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x79 : Alpha
        {
            public x79(AbnfParser.CstNodes.Core.x79 value)
            {
                Value = value;
            }

            public Core.x79 Value { get; }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class x7A : Alpha
        {
            public x7A(AbnfParser.CstNodes.Core.x7A value)
            {
                Value = value;
            }

            public Core.x7A Value { get; }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
}
