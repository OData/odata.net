namespace AbnfParser.CstNodes
{
    using System.Collections.Generic;

    using AbnfParser.CstNodes.Core;

    public class CharVal
    {
        public CharVal(Dquote openDquote, IEnumerable<Inner> inners, Dquote closeDquote)
        {
            OpenDquote = openDquote;
            Inners = inners;
            CloseDquote = closeDquote;
        }

        public Dquote OpenDquote { get; }
        public IEnumerable<Inner> Inners { get; }
        public Dquote CloseDquote { get; }

        public abstract class Inner
        {
            private Inner()
            {
            }

            public sealed class x20 : Inner
            {
                public x20(Core.x20 value)
                {
                    Value = value;
                }

                public Core.x20 Value { get; }
            }

            public sealed class x21 : Inner
            {
                public x21(Core.x21 value)
                {
                    Value = value;
                }

                public Core.x21 Value { get; }
            }

            public sealed class x23 : Inner
            {
                public x23(Core.x23 value)
                {
                    Value = value;
                }

                public Core.x23 Value { get; }
            }

            public sealed class x24 : Inner
            {
                public x24(Core.x24 value)
                {
                    Value = value;
                }

                public Core.x24 Value { get; }
            }

            public sealed class x25 : Inner
            {
                public x25(Core.x25 value)
                {
                    Value = value;
                }

                public Core.x25 Value { get; }
            }

            public sealed class x26 : Inner
            {
                public x26(Core.x26 value)
                {
                    Value = value;
                }

                public Core.x26 Value { get; }
            }

            public sealed class x27 : Inner
            {
                public x27(Core.x27 value)
                {
                    Value = value;
                }

                public Core.x27 Value { get; }
            }

            public sealed class x28 : Inner
            {
                public x28(Core.x28 value)
                {
                    Value = value;
                }

                public Core.x28 Value { get; }
            }

            public sealed class x29 : Inner
            {
                public x29(Core.x29 value)
                {
                    Value = value;
                }

                public Core.x29 Value { get; }
            }

            public sealed class x2A : Inner
            {
                public x2A(Core.x2A value)
                {
                    Value = value;
                }

                public Core.x2A Value { get; }
            }

            public sealed class x2B : Inner
            {
                public x2B(Core.x2B value)
                {
                    Value = value;
                }

                public Core.x2B Value { get; }
            }

            public sealed class x2C : Inner
            {
                public x2C(Core.x2C value)
                {
                    Value = value;
                }

                public Core.x2C Value { get; }
            }

            public sealed class x2D : Inner
            {
                public x2D(Core.x2D value)
                {
                    Value = value;
                }

                public Core.x2D Value { get; }
            }

            public sealed class x2E : Inner
            {
                public x2E(Core.x2E value)
                {
                    Value = value;
                }

                public Core.x2E Value { get; }
            }

            public sealed class x2F : Inner
            {
                public x2F(Core.x2F value)
                {
                    Value = value;
                }

                public Core.x2F Value { get; }
            }

            public sealed class x30 : Inner
            {
                public x30(Core.x30 value)
                {
                    Value = value;
                }

                public Core.x30 Value { get; }
            }

            public sealed class x31 : Inner
            {
                public x31(Core.x31 value)
                {
                    Value = value;
                }

                public Core.x31 Value { get; }
            }

            public sealed class x32 : Inner
            {
                public x32(Core.x32 value)
                {
                    Value = value;
                }

                public Core.x32 Value { get; }
            }

            public sealed class x33 : Inner
            {
                public x33(Core.x33 value)
                {
                    Value = value;
                }

                public Core.x33 Value { get; }
            }

            public sealed class x34 : Inner
            {
                public x34(Core.x34 value)
                {
                    Value = value;
                }

                public Core.x34 Value { get; }
            }

            public sealed class x35 : Inner
            {
                public x35(Core.x35 value)
                {
                    Value = value;
                }

                public Core.x35 Value { get; }
            }

            public sealed class x36 : Inner
            {
                public x36(Core.x36 value)
                {
                    Value = value;
                }

                public Core.x36 Value { get; }
            }

            public sealed class x37 : Inner
            {
                public x37(Core.x37 value)
                {
                    Value = value;
                }

                public Core.x37 Value { get; }
            }

            public sealed class x38 : Inner
            {
                public x38(Core.x38 value)
                {
                    Value = value;
                }

                public Core.x38 Value { get; }
            }

            public sealed class x39 : Inner
            {
                public x39(Core.x39 value)
                {
                    Value = value;
                }

                public Core.x39 Value { get; }
            }

            public sealed class x3A : Inner
            {
                public x3A(Core.x3A value)
                {
                    Value = value;
                }

                public Core.x3A Value { get; }
            }

            public sealed class x3B : Inner
            {
                public x3B(Core.x3B value)
                {
                    Value = value;
                }

                public Core.x3B Value { get; }
            }

            public sealed class x3C : Inner
            {
                public x3C(Core.x3C value)
                {
                    Value = value;
                }

                public Core.x3C Value { get; }
            }

            public sealed class x3D : Inner
            {
                public x3D(Core.x3D value)
                {
                    Value = value;
                }

                public Core.x3D Value { get; }
            }

            public sealed class x3E : Inner
            {
                public x3E(Core.x3E value)
                {
                    Value = value;
                }

                public Core.x3E Value { get; }
            }

            public sealed class x3F : Inner
            {
                public x3F(Core.x3F value)
                {
                    Value = value;
                }

                public Core.x3F Value { get; }
            }

            public sealed class x40 : Inner
            {
                public x40(Core.x40 value)
                {
                    Value = value;
                }

                public Core.x40 Value { get; }
            }

            public sealed class x41 : Inner
            {
                public x41(Core.x41 value)
                {
                    Value = value;
                }

                public Core.x41 Value { get; }
            }

            public sealed class x42 : Inner
            {
                public x42(Core.x42 value)
                {
                    Value = value;
                }

                public Core.x42 Value { get; }
            }

            public sealed class x43 : Inner
            {
                public x43(Core.x43 value)
                {
                    Value = value;
                }

                public Core.x43 Value { get; }
            }

            public sealed class x44 : Inner
            {
                public x44(Core.x44 value)
                {
                    Value = value;
                }

                public Core.x44 Value { get; }
            }

            public sealed class x45 : Inner
            {
                public x45(Core.x45 value)
                {
                    Value = value;
                }

                public Core.x45 Value { get; }
            }

            public sealed class x46 : Inner
            {
                public x46(Core.x46 value)
                {
                    Value = value;
                }

                public Core.x46 Value { get; }
            }

            public sealed class x47 : Inner
            {
                public x47(Core.x47 value)
                {
                    Value = value;
                }

                public Core.x47 Value { get; }
            }

            public sealed class x48 : Inner
            {
                public x48(Core.x48 value)
                {
                    Value = value;
                }

                public Core.x48 Value { get; }
            }

            public sealed class x49 : Inner
            {
                public x49(Core.x49 value)
                {
                    Value = value;
                }

                public Core.x49 Value { get; }
            }

            public sealed class x4A : Inner
            {
                public x4A(Core.x4A value)
                {
                    Value = value;
                }

                public Core.x4A Value { get; }
            }

            public sealed class x4B : Inner
            {
                public x4B(Core.x4B value)
                {
                    Value = value;
                }

                public Core.x4B Value { get; }
            }

            public sealed class x4C : Inner
            {
                public x4C(Core.x4C value)
                {
                    Value = value;
                }

                public Core.x4C Value { get; }
            }

            public sealed class x4D : Inner
            {
                public x4D(Core.x4D value)
                {
                    Value = value;
                }

                public Core.x4D Value { get; }
            }

            public sealed class x4E : Inner
            {
                public x4E(Core.x4E value)
                {
                    Value = value;
                }

                public Core.x4E Value { get; }
            }

            public sealed class x4F : Inner
            {
                public x4F(Core.x4F value)
                {
                    Value = value;
                }

                public Core.x4F Value { get; }
            }

            public sealed class x50 : Inner
            {
                public x50(Core.x50 value)
                {
                    Value = value;
                }

                public Core.x50 Value { get; }
            }

            public sealed class x51 : Inner
            {
                public x51(Core.x51 value)
                {
                    Value = value;
                }

                public Core.x51 Value { get; }
            }

            public sealed class x52 : Inner
            {
                public x52(Core.x52 value)
                {
                    Value = value;
                }

                public Core.x52 Value { get; }
            }

            public sealed class x53 : Inner
            {
                public x53(Core.x53 value)
                {
                    Value = value;
                }

                public Core.x53 Value { get; }
            }

            public sealed class x54 : Inner
            {
                public x54(Core.x54 value)
                {
                    Value = value;
                }

                public Core.x54 Value { get; }
            }

            public sealed class x55 : Inner
            {
                public x55(Core.x55 value)
                {
                    Value = value;
                }

                public Core.x55 Value { get; }
            }

            public sealed class x56 : Inner
            {
                public x56(Core.x56 value)
                {
                    Value = value;
                }

                public Core.x56 Value { get; }
            }

            public sealed class x57 : Inner
            {
                public x57(Core.x57 value)
                {
                    Value = value;
                }

                public Core.x57 Value { get; }
            }

            public sealed class x58 : Inner
            {
                public x58(Core.x58 value)
                {
                    Value = value;
                }

                public Core.x58 Value { get; }
            }

            public sealed class x59 : Inner
            {
                public x59(Core.x59 value)
                {
                    Value = value;
                }

                public Core.x59 Value { get; }
            }

            public sealed class x5A : Inner
            {
                public x5A(Core.x5A value)
                {
                    Value = value;
                }

                public Core.x5A Value { get; }
            }

            public sealed class x5B : Inner
            {
                public x5B(Core.x5B value)
                {
                    Value = value;
                }

                public Core.x5B Value { get; }
            }

            public sealed class x5C : Inner
            {
                public x5C(Core.x5C value)
                {
                    Value = value;
                }

                public Core.x5C Value { get; }
            }

            public sealed class x5D : Inner
            {
                public x5D(Core.x5D value)
                {
                    Value = value;
                }

                public Core.x5D Value { get; }
            }

            public sealed class x5E : Inner
            {
                public x5E(Core.x5E value)
                {
                    Value = value;
                }

                public Core.x5E Value { get; }
            }

            public sealed class x5F : Inner
            {
                public x5F(Core.x5F value)
                {
                    Value = value;
                }

                public Core.x5F Value { get; }
            }

            public sealed class x60 : Inner
            {
                public x60(Core.x60 value)
                {
                    Value = value;
                }

                public Core.x60 Value { get; }
            }

            public sealed class x61 : Inner
            {
                public x61(Core.x61 value)
                {
                    Value = value;
                }

                public Core.x61 Value { get; }
            }

            public sealed class x62 : Inner
            {
                public x62(Core.x62 value)
                {
                    Value = value;
                }

                public Core.x62 Value { get; }
            }

            public sealed class x63 : Inner
            {
                public x63(Core.x63 value)
                {
                    Value = value;
                }

                public Core.x63 Value { get; }
            }

            public sealed class x64 : Inner
            {
                public x64(Core.x64 value)
                {
                    Value = value;
                }

                public Core.x64 Value { get; }
            }

            public sealed class x65 : Inner
            {
                public x65(Core.x65 value)
                {
                    Value = value;
                }

                public Core.x65 Value { get; }
            }

            public sealed class x66 : Inner
            {
                public x66(Core.x66 value)
                {
                    Value = value;
                }

                public Core.x66 Value { get; }
            }

            public sealed class x67 : Inner
            {
                public x67(Core.x67 value)
                {
                    Value = value;
                }

                public Core.x67 Value { get; }
            }

            public sealed class x68 : Inner
            {
                public x68(Core.x68 value)
                {
                    Value = value;
                }

                public Core.x68 Value { get; }
            }

            public sealed class x69 : Inner
            {
                public x69(Core.x69 value)
                {
                    Value = value;
                }

                public Core.x69 Value { get; }
            }

            public sealed class x6A : Inner
            {
                public x6A(Core.x6A value)
                {
                    Value = value;
                }

                public Core.x6A Value { get; }
            }

            public sealed class x6B : Inner
            {
                public x6B(Core.x6B value)
                {
                    Value = value;
                }

                public Core.x6B Value { get; }
            }

            public sealed class x6C : Inner
            {
                public x6C(Core.x6C value)
                {
                    Value = value;
                }

                public Core.x6C Value { get; }
            }

            public sealed class x6D : Inner
            {
                public x6D(Core.x6D value)
                {
                    Value = value;
                }

                public Core.x6D Value { get; }
            }

            public sealed class x6E : Inner
            {
                public x6E(Core.x6E value)
                {
                    Value = value;
                }

                public Core.x6E Value { get; }
            }

            public sealed class x6F : Inner
            {
                public x6F(Core.x6F value)
                {
                    Value = value;
                }

                public Core.x6F Value { get; }
            }

            public sealed class x70 : Inner
            {
                public x70(Core.x70 value)
                {
                    Value = value;
                }

                public Core.x70 Value { get; }
            }

            public sealed class x71 : Inner
            {
                public x71(Core.x71 value)
                {
                    Value = value;
                }

                public Core.x71 Value { get; }
            }

            public sealed class x72 : Inner
            {
                public x72(Core.x72 value)
                {
                    Value = value;
                }

                public Core.x72 Value { get; }
            }

            public sealed class x73 : Inner
            {
                public x73(Core.x73 value)
                {
                    Value = value;
                }

                public Core.x73 Value { get; }
            }

            public sealed class x74 : Inner
            {
                public x74(Core.x74 value)
                {
                    Value = value;
                }

                public Core.x74 Value { get; }
            }

            public sealed class x75 : Inner
            {
                public x75(Core.x75 value)
                {
                    Value = value;
                }

                public Core.x75 Value { get; }
            }

            public sealed class x76 : Inner
            {
                public x76(Core.x76 value)
                {
                    Value = value;
                }

                public Core.x76 Value { get; }
            }

            public sealed class x77 : Inner
            {
                public x77(Core.x77 value)
                {
                    Value = value;
                }

                public Core.x77 Value { get; }
            }

            public sealed class x78 : Inner
            {
                public x78(Core.x78 value)
                {
                    Value = value;
                }

                public Core.x78 Value { get; }
            }

            public sealed class x79 : Inner
            {
                public x79(Core.x79 value)
                {
                    Value = value;
                }

                public Core.x79 Value { get; }
            }

            public sealed class x7A : Inner
            {
                public x7A(Core.x7A value)
                {
                    Value = value;
                }

                public Core.x7A Value { get; }
            }

            public sealed class x7B : Inner
            {
                public x7B(Core.x7B value)
                {
                    Value = value;
                }

                public Core.x7B Value { get; }
            }

            public sealed class x7C : Inner
            {
                public x7C(Core.x7C value)
                {
                    Value = value;
                }

                public Core.x7C Value { get; }
            }

            public sealed class x7D : Inner
            {
                public x7D(Core.x7D value)
                {
                    Value = value;
                }

                public Core.x7D Value { get; }
            }

            public sealed class x7E : Inner
            {
                public x7E(Core.x7E value)
                {
                    Value = value;
                }

                public Core.x7E Value { get; }
            }


        }
    }
}
