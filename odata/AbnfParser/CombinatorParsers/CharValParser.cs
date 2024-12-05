namespace AbnfParser.CombinatorParsers
{
    using AbnfParser.CombinatorParsers.Core;
    using AbnfParser.CstNodes;
    using Sprache;

    public static class CharValParser
    {
        public static class InnerParser
        {
            public static Parser<CharVal.Inner.x20> x20 { get; } =
                from value in x20Parser.Instance
                select new CharVal.Inner.x20(value);

            public static Parser<CharVal.Inner.x21> x21 { get; } =
                from value in x21Parser.Instance
                select new CharVal.Inner.x21(value);

            public static Parser<CharVal.Inner.x23> x23 { get; } =
                from value in x23Parser.Instance
                select new CharVal.Inner.x23(value);

            public static Parser<CharVal.Inner.x24> x24 { get; } =
                from value in x24Parser.Instance
                select new CharVal.Inner.x24(value);

            public static Parser<CharVal.Inner.x25> x25 { get; } =
                from value in x25Parser.Instance
                select new CharVal.Inner.x25(value);

            public static Parser<CharVal.Inner.x26> x26 { get; } =
                from value in x26Parser.Instance
                select new CharVal.Inner.x26(value);

            public static Parser<CharVal.Inner.x27> x27 { get; } =
                from value in x27Parser.Instance
                select new CharVal.Inner.x27(value);

            public static Parser<CharVal.Inner.x28> x28 { get; } =
                from value in x28Parser.Instance
                select new CharVal.Inner.x28(value);

            public static Parser<CharVal.Inner.x29> x29 { get; } =
                from value in x29Parser.Instance
                select new CharVal.Inner.x29(value);

            public static Parser<CharVal.Inner.x2A> x2A { get; } =
                from value in x2AParser.Instance
                select new CharVal.Inner.x2A(value);

            public static Parser<CharVal.Inner.x2B> x2B { get; } =
                from value in x2BParser.Instance
                select new CharVal.Inner.x2B(value);

            public static Parser<CharVal.Inner.x2C> x2C { get; } =
                from value in x2CParser.Instance
                select new CharVal.Inner.x2C(value);

            public static Parser<CharVal.Inner.x2D> x2D { get; } =
                from value in x2DParser.Instance
                select new CharVal.Inner.x2D(value);

            public static Parser<CharVal.Inner.x2E> x2E { get; } =
                from value in x2EParser.Instance
                select new CharVal.Inner.x2E(value);

            public static Parser<CharVal.Inner.x2F> x2F { get; } =
                from value in x2FParser.Instance
                select new CharVal.Inner.x2F(value);

            public static Parser<CharVal.Inner.x30> x30 { get; } =
                from value in x30Parser.Instance
                select new CharVal.Inner.x30(value);

            public static Parser<CharVal.Inner.x31> x31 { get; } =
                from value in x31Parser.Instance
                select new CharVal.Inner.x31(value);

            public static Parser<CharVal.Inner.x32> x32 { get; } =
                from value in x32Parser.Instance
                select new CharVal.Inner.x32(value);

            public static Parser<CharVal.Inner.x33> x33 { get; } =
                from value in x33Parser.Instance
                select new CharVal.Inner.x33(value);

            public static Parser<CharVal.Inner.x34> x34 { get; } =
                from value in x34Parser.Instance
                select new CharVal.Inner.x34(value);

            public static Parser<CharVal.Inner.x35> x35 { get; } =
                from value in x35Parser.Instance
                select new CharVal.Inner.x35(value);

            public static Parser<CharVal.Inner.x36> x36 { get; } =
                from value in x36Parser.Instance
                select new CharVal.Inner.x36(value);

            public static Parser<CharVal.Inner.x37> x37 { get; } =
                from value in x37Parser.Instance
                select new CharVal.Inner.x37(value);

            public static Parser<CharVal.Inner.x38> x38 { get; } =
                from value in x38Parser.Instance
                select new CharVal.Inner.x38(value);

            public static Parser<CharVal.Inner.x39> x39 { get; } =
                from value in x39Parser.Instance
                select new CharVal.Inner.x39(value);

            public static Parser<CharVal.Inner.x3A> x3A { get; } =
                from value in x3AParser.Instance
                select new CharVal.Inner.x3A(value);

            public static Parser<CharVal.Inner.x3B> x3B { get; } =
                from value in x3BParser.Instance
                select new CharVal.Inner.x3B(value);

            public static Parser<CharVal.Inner.x3C> x3C { get; } =
                from value in x3CParser.Instance
                select new CharVal.Inner.x3C(value);

            public static Parser<CharVal.Inner.x3D> x3D { get; } =
                from value in x3DParser.Instance
                select new CharVal.Inner.x3D(value);

            public static Parser<CharVal.Inner.x3E> x3E { get; } =
                from value in x3EParser.Instance
                select new CharVal.Inner.x3E(value);

            public static Parser<CharVal.Inner.x3F> x3F { get; } =
                from value in x3FParser.Instance
                select new CharVal.Inner.x3F(value);

            public static Parser<CharVal.Inner.x40> x40 { get; } =
                from value in x40Parser.Instance
                select new CharVal.Inner.x40(value);

            public static Parser<CharVal.Inner.x41> x41 { get; } =
                from value in x41Parser.Instance
                select new CharVal.Inner.x41(value);

            public static Parser<CharVal.Inner.x42> x42 { get; } =
                from value in x42Parser.Instance
                select new CharVal.Inner.x42(value);

            public static Parser<CharVal.Inner.x43> x43 { get; } =
                from value in x43Parser.Instance
                select new CharVal.Inner.x43(value);

            public static Parser<CharVal.Inner.x44> x44 { get; } =
                from value in x44Parser.Instance
                select new CharVal.Inner.x44(value);

            public static Parser<CharVal.Inner.x45> x45 { get; } =
                from value in x45Parser.Instance
                select new CharVal.Inner.x45(value);

            public static Parser<CharVal.Inner.x46> x46 { get; } =
                from value in x46Parser.Instance
                select new CharVal.Inner.x46(value);

            public static Parser<CharVal.Inner.x47> x47 { get; } =
                from value in x47Parser.Instance
                select new CharVal.Inner.x47(value);

            public static Parser<CharVal.Inner.x48> x48 { get; } =
                from value in x48Parser.Instance
                select new CharVal.Inner.x48(value);

            public static Parser<CharVal.Inner.x49> x49 { get; } =
                from value in x49Parser.Instance
                select new CharVal.Inner.x49(value);

            public static Parser<CharVal.Inner.x4A> x4A { get; } =
                from value in x4AParser.Instance
                select new CharVal.Inner.x4A(value);

            public static Parser<CharVal.Inner.x4B> x4B { get; } =
                from value in x4BParser.Instance
                select new CharVal.Inner.x4B(value);

            public static Parser<CharVal.Inner.x4C> x4C { get; } =
                from value in x4CParser.Instance
                select new CharVal.Inner.x4C(value);

            public static Parser<CharVal.Inner.x4D> x4D { get; } =
                from value in x4DParser.Instance
                select new CharVal.Inner.x4D(value);

            public static Parser<CharVal.Inner.x4E> x4E { get; } =
                from value in x4EParser.Instance
                select new CharVal.Inner.x4E(value);

            public static Parser<CharVal.Inner.x4F> x4F { get; } =
                from value in x4FParser.Instance
                select new CharVal.Inner.x4F(value);

            public static Parser<CharVal.Inner.x50> x50 { get; } =
                from value in x50Parser.Instance
                select new CharVal.Inner.x50(value);

            public static Parser<CharVal.Inner.x51> x51 { get; } =
                from value in x51Parser.Instance
                select new CharVal.Inner.x51(value);

            public static Parser<CharVal.Inner.x52> x52 { get; } =
                from value in x52Parser.Instance
                select new CharVal.Inner.x52(value);

            public static Parser<CharVal.Inner.x53> x53 { get; } =
                from value in x53Parser.Instance
                select new CharVal.Inner.x53(value);

            public static Parser<CharVal.Inner.x54> x54 { get; } =
                from value in x54Parser.Instance
                select new CharVal.Inner.x54(value);

            public static Parser<CharVal.Inner.x55> x55 { get; } =
                from value in x55Parser.Instance
                select new CharVal.Inner.x55(value);

            public static Parser<CharVal.Inner.x56> x56 { get; } =
                from value in x56Parser.Instance
                select new CharVal.Inner.x56(value);

            public static Parser<CharVal.Inner.x57> x57 { get; } =
                from value in x57Parser.Instance
                select new CharVal.Inner.x57(value);

            public static Parser<CharVal.Inner.x58> x58 { get; } =
                from value in x58Parser.Instance
                select new CharVal.Inner.x58(value);

            public static Parser<CharVal.Inner.x59> x59 { get; } =
                from value in x59Parser.Instance
                select new CharVal.Inner.x59(value);

            public static Parser<CharVal.Inner.x5A> x5A { get; } =
                from value in x5AParser.Instance
                select new CharVal.Inner.x5A(value);

            public static Parser<CharVal.Inner.x5B> x5B { get; } =
                from value in x5BParser.Instance
                select new CharVal.Inner.x5B(value);

            public static Parser<CharVal.Inner.x5C> x5C { get; } =
                from value in x5CParser.Instance
                select new CharVal.Inner.x5C(value);

            public static Parser<CharVal.Inner.x5D> x5D { get; } =
                from value in x5DParser.Instance
                select new CharVal.Inner.x5D(value);

            public static Parser<CharVal.Inner.x5E> x5E { get; } =
                from value in x5EParser.Instance
                select new CharVal.Inner.x5E(value);

            public static Parser<CharVal.Inner.x5F> x5F { get; } =
                from value in x5FParser.Instance
                select new CharVal.Inner.x5F(value);

            public static Parser<CharVal.Inner.x60> x60 { get; } =
                from value in x60Parser.Instance
                select new CharVal.Inner.x60(value);

            public static Parser<CharVal.Inner.x61> x61 { get; } =
                from value in x61Parser.Instance
                select new CharVal.Inner.x61(value);

            public static Parser<CharVal.Inner.x62> x62 { get; } =
                from value in x62Parser.Instance
                select new CharVal.Inner.x62(value);

            public static Parser<CharVal.Inner.x63> x63 { get; } =
                from value in x63Parser.Instance
                select new CharVal.Inner.x63(value);

            public static Parser<CharVal.Inner.x64> x64 { get; } =
                from value in x64Parser.Instance
                select new CharVal.Inner.x64(value);

            public static Parser<CharVal.Inner.x65> x65 { get; } =
                from value in x65Parser.Instance
                select new CharVal.Inner.x65(value);

            public static Parser<CharVal.Inner.x66> x66 { get; } =
                from value in x66Parser.Instance
                select new CharVal.Inner.x66(value);

            public static Parser<CharVal.Inner.x67> x67 { get; } =
                from value in x67Parser.Instance
                select new CharVal.Inner.x67(value);

            public static Parser<CharVal.Inner.x68> x68 { get; } =
                from value in x68Parser.Instance
                select new CharVal.Inner.x68(value);

            public static Parser<CharVal.Inner.x69> x69 { get; } =
                from value in x69Parser.Instance
                select new CharVal.Inner.x69(value);

            public static Parser<CharVal.Inner.x6A> x6A { get; } =
                from value in x6AParser.Instance
                select new CharVal.Inner.x6A(value);

            public static Parser<CharVal.Inner.x6B> x6B { get; } =
                from value in x6BParser.Instance
                select new CharVal.Inner.x6B(value);

            public static Parser<CharVal.Inner.x6C> x6C { get; } =
                from value in x6CParser.Instance
                select new CharVal.Inner.x6C(value);

            public static Parser<CharVal.Inner.x6D> x6D { get; } =
                from value in x6DParser.Instance
                select new CharVal.Inner.x6D(value);

            public static Parser<CharVal.Inner.x6E> x6E { get; } =
                from value in x6EParser.Instance
                select new CharVal.Inner.x6E(value);

            public static Parser<CharVal.Inner.x6F> x6F { get; } =
                from value in x6FParser.Instance
                select new CharVal.Inner.x6F(value);

            public static Parser<CharVal.Inner.x70> x70 { get; } =
                from value in x70Parser.Instance
                select new CharVal.Inner.x70(value);

            public static Parser<CharVal.Inner.x71> x71 { get; } =
                from value in x71Parser.Instance
                select new CharVal.Inner.x71(value);

            public static Parser<CharVal.Inner.x72> x72 { get; } =
                from value in x72Parser.Instance
                select new CharVal.Inner.x72(value);

            public static Parser<CharVal.Inner.x73> x73 { get; } =
                from value in x73Parser.Instance
                select new CharVal.Inner.x73(value);

            public static Parser<CharVal.Inner.x74> x74 { get; } =
                from value in x74Parser.Instance
                select new CharVal.Inner.x74(value);

            public static Parser<CharVal.Inner.x75> x75 { get; } =
                from value in x75Parser.Instance
                select new CharVal.Inner.x75(value);

            public static Parser<CharVal.Inner.x76> x76 { get; } =
                from value in x76Parser.Instance
                select new CharVal.Inner.x76(value);

            public static Parser<CharVal.Inner.x77> x77 { get; } =
                from value in x77Parser.Instance
                select new CharVal.Inner.x77(value);

            public static Parser<CharVal.Inner.x78> x78 { get; } =
                from value in x78Parser.Instance
                select new CharVal.Inner.x78(value);

            public static Parser<CharVal.Inner.x79> x79 { get; } =
                from value in x79Parser.Instance
                select new CharVal.Inner.x79(value);

            public static Parser<CharVal.Inner.x7A> x7A { get; } =
                from value in x7AParser.Instance
                select new CharVal.Inner.x7A(value);

            public static Parser<CharVal.Inner.x7B> x7B { get; } =
                from value in x7BParser.Instance
                select new CharVal.Inner.x7B(value);

            public static Parser<CharVal.Inner.x7C> x7C { get; } =
                from value in x7CParser.Instance
                select new CharVal.Inner.x7C(value);

            public static Parser<CharVal.Inner.x7D> x7D { get; } =
                from value in x7DParser.Instance
                select new CharVal.Inner.x7D(value);

            public static Parser<CharVal.Inner.x7E> x7E { get; } =
                from value in x7EParser.Instance
                select new CharVal.Inner.x7E(value);

            public static Parser<CharVal.Inner> Instance { get; } =
                x20
                .Or<CharVal.Inner>(x21)
                .Or(x23)
                .Or(x24)
                .Or(x25)
                .Or(x26)
                .Or(x27)
                .Or(x28)
                .Or(x29)
                .Or(x2A)
                .Or(x2B)
                .Or(x2C)
                .Or(x2D)
                .Or(x2E)
                .Or(x2F)
                .Or(x30)
                .Or(x31)
                .Or(x32)
                .Or(x33)
                .Or(x34)
                .Or(x35)
                .Or(x36)
                .Or(x37)
                .Or(x38)
                .Or(x39)
                .Or(x3A)
                .Or(x3B)
                .Or(x3C)
                .Or(x3D)
                .Or(x3E)
                .Or(x3F)
                .Or(x40)
                .Or(x41)
                .Or(x42)
                .Or(x43)
                .Or(x44)
                .Or(x45)
                .Or(x46)
                .Or(x47)
                .Or(x48)
                .Or(x49)
                .Or(x4A)
                .Or(x4B)
                .Or(x4C)
                .Or(x4D)
                .Or(x4E)
                .Or(x4F)
                .Or(x50)
                .Or(x51)
                .Or(x52)
                .Or(x53)
                .Or(x54)
                .Or(x55)
                .Or(x56)
                .Or(x57)
                .Or(x58)
                .Or(x59)
                .Or(x5A)
                .Or(x5B)
                .Or(x5C)
                .Or(x5D)
                .Or(x5E)
                .Or(x5F)
                .Or(x60)
                .Or(x61)
                .Or(x62)
                .Or(x63)
                .Or(x64)
                .Or(x65)
                .Or(x66)
                .Or(x67)
                .Or(x68)
                .Or(x69)
                .Or(x6A)
                .Or(x6B)
                .Or(x6C)
                .Or(x6D)
                .Or(x6E)
                .Or(x6F)
                .Or(x70)
                .Or(x71)
                .Or(x72)
                .Or(x73)
                .Or(x74)
                .Or(x75)
                .Or(x76)
                .Or(x77)
                .Or(x78)
                .Or(x79)
                .Or(x7A)
                .Or(x7B)
                .Or(x7C)
                .Or(x7D)
                .Or(x7E);
        }
    }
}
