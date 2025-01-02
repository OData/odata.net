namespace GeneratorV3.OldToNewConverters
{
    using AbnfParser.CstNodes;
    using GeneratorV3.Abnf;
    using GeneratorV3.OldToNewConverters.Core;
    using Root;
    using System.Linq;

    public sealed class CharValConverter
    {
        private CharValConverter()
        {
        }

        public static CharValConverter Instance { get; } = new CharValConverter();

        public GeneratorV3.Abnf._charⲻval Convert(AbnfParser.CstNodes.CharVal charVal)
        {
            return new _charⲻval(
                DquoteConverter.Instance.Convert(charVal.OpenDquote),
                charVal.Inners.Select(inner =>
                    new Inners._Ⲥpercentx20ⲻ21Ⳇpercentx23ⲻ7EↃ(
                        InnerConverter.Instance.Visit(inner, default))),
                DquoteConverter.Instance.Convert(charVal.CloseDquote));
        }

        private sealed class InnerConverter : CharVal.Inner.Visitor<Abnf.Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E, Root.Void>
        {
            private InnerConverter()
            {
            }

            public static InnerConverter Instance { get; } = new InnerConverter();

            protected internal override Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E Accept(CharVal.Inner.x20 node, Void context)
            {
                return new Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E._percentx20ⲻ21(
                    new Inners._percentx20ⲻ21._20(
                        Inners._2.Instance,
                        Inners._0.Instance));
            }

            protected internal override Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E Accept(CharVal.Inner.x21 node, Void context)
            {
                return new Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E._percentx20ⲻ21(
                    new Inners._percentx20ⲻ21._21(
                        Inners._2.Instance,
                        Inners._1.Instance));
            }

            protected internal override Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E Accept(CharVal.Inner.x23 node, Void context)
            {
                return new Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E._percentx23ⲻ7E(
                    new Inners._percentx23ⲻ7E._23(
                        Inners._2.Instance,
                        Inners._3.Instance));
            }

            protected internal override Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E Accept(CharVal.Inner.x24 node, Void context)
            {
                return new Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E._percentx23ⲻ7E(
                    new Inners._percentx23ⲻ7E._24(
                        Inners._2.Instance,
                        Inners._4.Instance));
            }

            protected internal override Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E Accept(CharVal.Inner.x25 node, Void context)
            {
                return new Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E._percentx23ⲻ7E(
                    new Inners._percentx23ⲻ7E._25(
                        Inners._2.Instance,
                        Inners._5.Instance));
            }

            protected internal override Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E Accept(CharVal.Inner.x26 node, Void context)
            {
                return new Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E._percentx23ⲻ7E(
                    new Inners._percentx23ⲻ7E._26(
                        Inners._2.Instance,
                        Inners._6.Instance));
            }

            protected internal override Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E Accept(CharVal.Inner.x27 node, Void context)
            {
                return new Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E._percentx23ⲻ7E(
                    new Inners._percentx23ⲻ7E._27(
                        Inners._2.Instance,
                        Inners._7.Instance));
            }

            protected internal override Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E Accept(CharVal.Inner.x28 node, Void context)
            {
                return new Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E._percentx23ⲻ7E(
                    new Inners._percentx23ⲻ7E._28(
                        Inners._2.Instance,
                        Inners._8.Instance));
            }

            protected internal override Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E Accept(CharVal.Inner.x29 node, Void context)
            {
                return new Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E._percentx23ⲻ7E(
                    new Inners._percentx23ⲻ7E._29(
                        Inners._2.Instance,
                        Inners._9.Instance));
            }

            protected internal override Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E Accept(CharVal.Inner.x2A node, Void context)
            {
                return new Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E._percentx23ⲻ7E(
                    new Inners._percentx23ⲻ7E._2A(
                        Inners._2.Instance,
                        Inners._A.Instance));
            }

            protected internal override Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E Accept(CharVal.Inner.x2B node, Void context)
            {
                return new Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E._percentx23ⲻ7E(
                    new Inners._percentx23ⲻ7E._2B(
                        Inners._2.Instance,
                        Inners._B.Instance));
            }

            protected internal override Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E Accept(CharVal.Inner.x2C node, Void context)
            {
                return new Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E._percentx23ⲻ7E(
                    new Inners._percentx23ⲻ7E._2C(
                        Inners._2.Instance,
                        Inners._C.Instance));
            }

            protected internal override Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E Accept(CharVal.Inner.x2D node, Void context)
            {
                return new Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E._percentx23ⲻ7E(
                    new Inners._percentx23ⲻ7E._2D(
                        Inners._2.Instance,
                        Inners._D.Instance));
            }

            protected internal override Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E Accept(CharVal.Inner.x2E node, Void context)
            {
                return new Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E._percentx23ⲻ7E(
                    new Inners._percentx23ⲻ7E._2E(
                        Inners._2.Instance,
                        Inners._E.Instance));
            }

            protected internal override Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E Accept(CharVal.Inner.x2F node, Void context)
            {
                return new Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E._percentx23ⲻ7E(
                    new Inners._percentx23ⲻ7E._2F(
                        Inners._2.Instance,
                        Inners._F.Instance));
            }

            protected internal override Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E Accept(CharVal.Inner.x30 node, Void context)
            {
                return new Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E._percentx23ⲻ7E(
                    new Inners._percentx23ⲻ7E._30(
                        Inners._3.Instance,
                        Inners._0.Instance));
            }

            protected internal override Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E Accept(CharVal.Inner.x31 node, Void context)
            {
                return new Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E._percentx23ⲻ7E(
                    new Inners._percentx23ⲻ7E._31(
                        Inners._3.Instance,
                        Inners._1.Instance));
            }

            protected internal override Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E Accept(CharVal.Inner.x32 node, Void context)
            {
                return new Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E._percentx23ⲻ7E(
                    new Inners._percentx23ⲻ7E._32(
                        Inners._3.Instance,
                        Inners._2.Instance));
            }

            protected internal override Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E Accept(CharVal.Inner.x33 node, Void context)
            {
                return new Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E._percentx23ⲻ7E(
                    new Inners._percentx23ⲻ7E._33(
                        Inners._3.Instance,
                        Inners._3.Instance));
            }

            protected internal override Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E Accept(CharVal.Inner.x34 node, Void context)
            {
                return new Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E._percentx23ⲻ7E(
                    new Inners._percentx23ⲻ7E._34(
                        Inners._3.Instance,
                        Inners._4.Instance));
            }

            protected internal override Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E Accept(CharVal.Inner.x35 node, Void context)
            {
                return new Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E._percentx23ⲻ7E(
                    new Inners._percentx23ⲻ7E._35(
                        Inners._3.Instance,
                        Inners._5.Instance));
            }

            protected internal override Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E Accept(CharVal.Inner.x36 node, Void context)
            {
                return new Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E._percentx23ⲻ7E(
                    new Inners._percentx23ⲻ7E._36(
                        Inners._3.Instance,
                        Inners._6.Instance));
            }

            protected internal override Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E Accept(CharVal.Inner.x37 node, Void context)
            {
                return new Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E._percentx23ⲻ7E(
                    new Inners._percentx23ⲻ7E._37(
                        Inners._3.Instance,
                        Inners._7.Instance));
            }

            protected internal override Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E Accept(CharVal.Inner.x38 node, Void context)
            {
                return new Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E._percentx23ⲻ7E(
                    new Inners._percentx23ⲻ7E._38(
                        Inners._3.Instance,
                        Inners._8.Instance));
            }

            protected internal override Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E Accept(CharVal.Inner.x39 node, Void context)
            {
                return new Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E._percentx23ⲻ7E(
                    new Inners._percentx23ⲻ7E._39(
                        Inners._3.Instance,
                        Inners._9.Instance));
            }

            protected internal override Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E Accept(CharVal.Inner.x3A node, Void context)
            {
                return new Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E._percentx23ⲻ7E(
                    new Inners._percentx23ⲻ7E._3A(
                        Inners._3.Instance,
                        Inners._A.Instance));
            }

            protected internal override Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E Accept(CharVal.Inner.x3B node, Void context)
            {
                return new Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E._percentx23ⲻ7E(
                    new Inners._percentx23ⲻ7E._3B(
                        Inners._3.Instance,
                        Inners._B.Instance));
            }

            protected internal override Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E Accept(CharVal.Inner.x3C node, Void context)
            {
                return new Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E._percentx23ⲻ7E(
                    new Inners._percentx23ⲻ7E._3C(
                        Inners._3.Instance,
                        Inners._C.Instance));
            }

            protected internal override Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E Accept(CharVal.Inner.x3D node, Void context)
            {
                return new Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E._percentx23ⲻ7E(
                    new Inners._percentx23ⲻ7E._3D(
                        Inners._3.Instance,
                        Inners._D.Instance));
            }

            protected internal override Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E Accept(CharVal.Inner.x3E node, Void context)
            {
                return new Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E._percentx23ⲻ7E(
                    new Inners._percentx23ⲻ7E._3E(
                        Inners._3.Instance,
                        Inners._E.Instance));
            }

            protected internal override Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E Accept(CharVal.Inner.x3F node, Void context)
            {
                return new Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E._percentx23ⲻ7E(
                    new Inners._percentx23ⲻ7E._3F(
                        Inners._3.Instance,
                        Inners._F.Instance));
            }

            protected internal override Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E Accept(CharVal.Inner.x40 node, Void context)
            {
                return new Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E._percentx23ⲻ7E(
                    new Inners._percentx23ⲻ7E._40(
                        Inners._4.Instance,
                        Inners._0.Instance));
            }

            protected internal override Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E Accept(CharVal.Inner.x41 node, Void context)
            {
                return new Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E._percentx23ⲻ7E(
                    new Inners._percentx23ⲻ7E._41(
                        Inners._4.Instance,
                        Inners._1.Instance));
            }

            protected internal override Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E Accept(CharVal.Inner.x42 node, Void context)
            {
                return new Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E._percentx23ⲻ7E(
                    new Inners._percentx23ⲻ7E._42(
                        Inners._4.Instance,
                        Inners._2.Instance));
            }

            protected internal override Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E Accept(CharVal.Inner.x43 node, Void context)
            {
                return new Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E._percentx23ⲻ7E(
                    new Inners._percentx23ⲻ7E._43(
                        Inners._4.Instance,
                        Inners._3.Instance));
            }

            protected internal override Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E Accept(CharVal.Inner.x44 node, Void context)
            {
                return new Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E._percentx23ⲻ7E(
                    new Inners._percentx23ⲻ7E._44(
                        Inners._4.Instance,
                        Inners._4.Instance));
            }

            protected internal override Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E Accept(CharVal.Inner.x45 node, Void context)
            {
                return new Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E._percentx23ⲻ7E(
                    new Inners._percentx23ⲻ7E._45(
                        Inners._4.Instance,
                        Inners._5.Instance));
            }

            protected internal override Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E Accept(CharVal.Inner.x46 node, Void context)
            {
                return new Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E._percentx23ⲻ7E(
                    new Inners._percentx23ⲻ7E._46(
                        Inners._4.Instance,
                        Inners._6.Instance));
            }

            protected internal override Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E Accept(CharVal.Inner.x47 node, Void context)
            {
                return new Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E._percentx23ⲻ7E(
                    new Inners._percentx23ⲻ7E._47(
                        Inners._4.Instance,
                        Inners._7.Instance));
            }

            protected internal override Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E Accept(CharVal.Inner.x48 node, Void context)
            {
                return new Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E._percentx23ⲻ7E(
                    new Inners._percentx23ⲻ7E._48(
                        Inners._4.Instance,
                        Inners._8.Instance));
            }

            protected internal override Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E Accept(CharVal.Inner.x49 node, Void context)
            {
                return new Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E._percentx23ⲻ7E(
                    new Inners._percentx23ⲻ7E._49(
                        Inners._4.Instance,
                        Inners._9.Instance));
            }

            protected internal override Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E Accept(CharVal.Inner.x4A node, Void context)
            {
                return new Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E._percentx23ⲻ7E(
                    new Inners._percentx23ⲻ7E._4A(
                        Inners._4.Instance,
                        Inners._A.Instance));
            }

            protected internal override Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E Accept(CharVal.Inner.x4B node, Void context)
            {
                return new Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E._percentx23ⲻ7E(
                    new Inners._percentx23ⲻ7E._4B(
                        Inners._4.Instance,
                        Inners._B.Instance));
            }

            protected internal override Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E Accept(CharVal.Inner.x4C node, Void context)
            {
                return new Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E._percentx23ⲻ7E(
                    new Inners._percentx23ⲻ7E._4C(
                        Inners._4.Instance,
                        Inners._C.Instance));
            }

            protected internal override Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E Accept(CharVal.Inner.x4D node, Void context)
            {
                return new Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E._percentx23ⲻ7E(
                    new Inners._percentx23ⲻ7E._4D(
                        Inners._4.Instance,
                        Inners._D.Instance));
            }

            protected internal override Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E Accept(CharVal.Inner.x4E node, Void context)
            {
                return new Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E._percentx23ⲻ7E(
                    new Inners._percentx23ⲻ7E._4E(
                        Inners._4.Instance,
                        Inners._E.Instance));
            }

            protected internal override Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E Accept(CharVal.Inner.x4F node, Void context)
            {
                return new Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E._percentx23ⲻ7E(
                    new Inners._percentx23ⲻ7E._4F(
                        Inners._4.Instance,
                        Inners._F.Instance));
            }

            protected internal override Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E Accept(CharVal.Inner.x50 node, Void context)
            {
                return new Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E._percentx23ⲻ7E(
                    new Inners._percentx23ⲻ7E._50(
                        Inners._5.Instance,
                        Inners._0.Instance));
            }

            protected internal override Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E Accept(CharVal.Inner.x51 node, Void context)
            {
                return new Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E._percentx23ⲻ7E(
                    new Inners._percentx23ⲻ7E._51(
                        Inners._5.Instance,
                        Inners._1.Instance));
            }

            protected internal override Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E Accept(CharVal.Inner.x52 node, Void context)
            {
                return new Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E._percentx23ⲻ7E(
                    new Inners._percentx23ⲻ7E._52(
                        Inners._5.Instance,
                        Inners._2.Instance));
            }

            protected internal override Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E Accept(CharVal.Inner.x53 node, Void context)
            {
                return new Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E._percentx23ⲻ7E(
                    new Inners._percentx23ⲻ7E._53(
                        Inners._5.Instance,
                        Inners._3.Instance));
            }

            protected internal override Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E Accept(CharVal.Inner.x54 node, Void context)
            {
                return new Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E._percentx23ⲻ7E(
                    new Inners._percentx23ⲻ7E._54(
                        Inners._5.Instance,
                        Inners._4.Instance));
            }

            protected internal override Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E Accept(CharVal.Inner.x55 node, Void context)
            {
                return new Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E._percentx23ⲻ7E(
                    new Inners._percentx23ⲻ7E._55(
                        Inners._5.Instance,
                        Inners._5.Instance));
            }

            protected internal override Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E Accept(CharVal.Inner.x56 node, Void context)
            {
                return new Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E._percentx23ⲻ7E(
                    new Inners._percentx23ⲻ7E._56(
                        Inners._5.Instance,
                        Inners._6.Instance));
            }

            protected internal override Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E Accept(CharVal.Inner.x57 node, Void context)
            {
                return new Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E._percentx23ⲻ7E(
                    new Inners._percentx23ⲻ7E._57(
                        Inners._5.Instance,
                        Inners._7.Instance));
            }

            protected internal override Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E Accept(CharVal.Inner.x58 node, Void context)
            {
                return new Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E._percentx23ⲻ7E(
                    new Inners._percentx23ⲻ7E._58(
                        Inners._5.Instance,
                        Inners._8.Instance));
            }

            protected internal override Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E Accept(CharVal.Inner.x59 node, Void context)
            {
                return new Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E._percentx23ⲻ7E(
                    new Inners._percentx23ⲻ7E._59(
                        Inners._5.Instance,
                        Inners._9.Instance));
            }

            protected internal override Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E Accept(CharVal.Inner.x5A node, Void context)
            {
                return new Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E._percentx23ⲻ7E(
                    new Inners._percentx23ⲻ7E._5A(
                        Inners._5.Instance,
                        Inners._A.Instance));
            }

            protected internal override Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E Accept(CharVal.Inner.x5B node, Void context)
            {
                return new Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E._percentx23ⲻ7E(
                    new Inners._percentx23ⲻ7E._5B(
                        Inners._5.Instance,
                        Inners._B.Instance));
            }

            protected internal override Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E Accept(CharVal.Inner.x5C node, Void context)
            {
                return new Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E._percentx23ⲻ7E(
                    new Inners._percentx23ⲻ7E._5C(
                        Inners._5.Instance,
                        Inners._C.Instance));
            }

            protected internal override Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E Accept(CharVal.Inner.x5D node, Void context)
            {
                return new Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E._percentx23ⲻ7E(
                    new Inners._percentx23ⲻ7E._5D(
                        Inners._5.Instance,
                        Inners._D.Instance));
            }

            protected internal override Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E Accept(CharVal.Inner.x5E node, Void context)
            {
                return new Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E._percentx23ⲻ7E(
                    new Inners._percentx23ⲻ7E._5E(
                        Inners._5.Instance,
                        Inners._E.Instance));
            }

            protected internal override Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E Accept(CharVal.Inner.x5F node, Void context)
            {
                return new Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E._percentx23ⲻ7E(
                    new Inners._percentx23ⲻ7E._5F(
                        Inners._5.Instance,
                        Inners._F.Instance));
            }

            protected internal override Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E Accept(CharVal.Inner.x60 node, Void context)
            {
                return new Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E._percentx23ⲻ7E(
                    new Inners._percentx23ⲻ7E._60(
                        Inners._6.Instance,
                        Inners._0.Instance));
            }

            protected internal override Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E Accept(CharVal.Inner.x61 node, Void context)
            {
                return new Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E._percentx23ⲻ7E(
                    new Inners._percentx23ⲻ7E._61(
                        Inners._6.Instance,
                        Inners._1.Instance));
            }

            protected internal override Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E Accept(CharVal.Inner.x62 node, Void context)
            {
                return new Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E._percentx23ⲻ7E(
                    new Inners._percentx23ⲻ7E._62(
                        Inners._6.Instance,
                        Inners._2.Instance));
            }

            protected internal override Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E Accept(CharVal.Inner.x63 node, Void context)
            {
                return new Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E._percentx23ⲻ7E(
                    new Inners._percentx23ⲻ7E._63(
                        Inners._6.Instance,
                        Inners._3.Instance));
            }

            protected internal override Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E Accept(CharVal.Inner.x64 node, Void context)
            {
                return new Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E._percentx23ⲻ7E(
                    new Inners._percentx23ⲻ7E._64(
                        Inners._6.Instance,
                        Inners._4.Instance));
            }

            protected internal override Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E Accept(CharVal.Inner.x65 node, Void context)
            {
                return new Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E._percentx23ⲻ7E(
                    new Inners._percentx23ⲻ7E._65(
                        Inners._6.Instance,
                        Inners._5.Instance));
            }

            protected internal override Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E Accept(CharVal.Inner.x66 node, Void context)
            {
                return new Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E._percentx23ⲻ7E(
                    new Inners._percentx23ⲻ7E._66(
                        Inners._6.Instance,
                        Inners._6.Instance));
            }

            protected internal override Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E Accept(CharVal.Inner.x67 node, Void context)
            {
                return new Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E._percentx23ⲻ7E(
                    new Inners._percentx23ⲻ7E._67(
                        Inners._6.Instance,
                        Inners._7.Instance));
            }

            protected internal override Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E Accept(CharVal.Inner.x68 node, Void context)
            {
                return new Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E._percentx23ⲻ7E(
                    new Inners._percentx23ⲻ7E._68(
                        Inners._6.Instance,
                        Inners._8.Instance));
            }

            protected internal override Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E Accept(CharVal.Inner.x69 node, Void context)
            {
                return new Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E._percentx23ⲻ7E(
                    new Inners._percentx23ⲻ7E._69(
                        Inners._6.Instance,
                        Inners._9.Instance));
            }

            protected internal override Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E Accept(CharVal.Inner.x6A node, Void context)
            {
                return new Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E._percentx23ⲻ7E(
                    new Inners._percentx23ⲻ7E._6A(
                        Inners._6.Instance,
                        Inners._A.Instance));
            }

            protected internal override Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E Accept(CharVal.Inner.x6B node, Void context)
            {
                return new Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E._percentx23ⲻ7E(
                    new Inners._percentx23ⲻ7E._6B(
                        Inners._6.Instance,
                        Inners._B.Instance));
            }

            protected internal override Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E Accept(CharVal.Inner.x6C node, Void context)
            {
                return new Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E._percentx23ⲻ7E(
                    new Inners._percentx23ⲻ7E._6C(
                        Inners._6.Instance,
                        Inners._C.Instance));
            }

            protected internal override Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E Accept(CharVal.Inner.x6D node, Void context)
            {
                return new Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E._percentx23ⲻ7E(
                    new Inners._percentx23ⲻ7E._6D(
                        Inners._6.Instance,
                        Inners._D.Instance));
            }

            protected internal override Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E Accept(CharVal.Inner.x6E node, Void context)
            {
                return new Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E._percentx23ⲻ7E(
                    new Inners._percentx23ⲻ7E._6E(
                        Inners._6.Instance,
                        Inners._E.Instance));
            }

            protected internal override Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E Accept(CharVal.Inner.x6F node, Void context)
            {
                return new Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E._percentx23ⲻ7E(
                    new Inners._percentx23ⲻ7E._6F(
                        Inners._6.Instance,
                        Inners._F.Instance));
            }

            protected internal override Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E Accept(CharVal.Inner.x70 node, Void context)
            {
                return new Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E._percentx23ⲻ7E(
                    new Inners._percentx23ⲻ7E._70(
                        Inners._7.Instance,
                        Inners._0.Instance));
            }

            protected internal override Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E Accept(CharVal.Inner.x71 node, Void context)
            {
                return new Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E._percentx23ⲻ7E(
                    new Inners._percentx23ⲻ7E._71(
                        Inners._7.Instance,
                        Inners._1.Instance));
            }

            protected internal override Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E Accept(CharVal.Inner.x72 node, Void context)
            {
                return new Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E._percentx23ⲻ7E(
                    new Inners._percentx23ⲻ7E._72(
                        Inners._7.Instance,
                        Inners._2.Instance));
            }

            protected internal override Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E Accept(CharVal.Inner.x73 node, Void context)
            {
                return new Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E._percentx23ⲻ7E(
                    new Inners._percentx23ⲻ7E._73(
                        Inners._7.Instance,
                        Inners._3.Instance));
            }

            protected internal override Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E Accept(CharVal.Inner.x74 node, Void context)
            {
                return new Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E._percentx23ⲻ7E(
                    new Inners._percentx23ⲻ7E._74(
                        Inners._7.Instance,
                        Inners._4.Instance));
            }

            protected internal override Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E Accept(CharVal.Inner.x75 node, Void context)
            {
                return new Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E._percentx23ⲻ7E(
                    new Inners._percentx23ⲻ7E._75(
                        Inners._7.Instance,
                        Inners._5.Instance));
            }

            protected internal override Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E Accept(CharVal.Inner.x76 node, Void context)
            {
                return new Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E._percentx23ⲻ7E(
                    new Inners._percentx23ⲻ7E._76(
                        Inners._7.Instance,
                        Inners._6.Instance));
            }

            protected internal override Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E Accept(CharVal.Inner.x77 node, Void context)
            {
                return new Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E._percentx23ⲻ7E(
                    new Inners._percentx23ⲻ7E._77(
                        Inners._7.Instance,
                        Inners._7.Instance));
            }

            protected internal override Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E Accept(CharVal.Inner.x78 node, Void context)
            {
                return new Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E._percentx23ⲻ7E(
                    new Inners._percentx23ⲻ7E._78(
                        Inners._7.Instance,
                        Inners._8.Instance));
            }

            protected internal override Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E Accept(CharVal.Inner.x79 node, Void context)
            {
                return new Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E._percentx23ⲻ7E(
                    new Inners._percentx23ⲻ7E._79(
                        Inners._7.Instance,
                        Inners._9.Instance));
            }

            protected internal override Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E Accept(CharVal.Inner.x7A node, Void context)
            {
                return new Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E._percentx23ⲻ7E(
                    new Inners._percentx23ⲻ7E._7A(
                        Inners._7.Instance,
                        Inners._A.Instance));
            }

            protected internal override Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E Accept(CharVal.Inner.x7B node, Void context)
            {
                return new Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E._percentx23ⲻ7E(
                    new Inners._percentx23ⲻ7E._7B(
                        Inners._7.Instance,
                        Inners._B.Instance));
            }

            protected internal override Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E Accept(CharVal.Inner.x7C node, Void context)
            {
                return new Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E._percentx23ⲻ7E(
                    new Inners._percentx23ⲻ7E._7C(
                        Inners._7.Instance,
                        Inners._C.Instance));
            }

            protected internal override Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E Accept(CharVal.Inner.x7D node, Void context)
            {
                return new Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E._percentx23ⲻ7E(
                    new Inners._percentx23ⲻ7E._7D(
                        Inners._7.Instance,
                        Inners._D.Instance));
            }

            protected internal override Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E Accept(CharVal.Inner.x7E node, Void context)
            {
                return new Inners._percentx20ⲻ21Ⳇpercentx23ⲻ7E._percentx23ⲻ7E(
                    new Inners._percentx23ⲻ7E._7E(
                        Inners._7.Instance,
                        Inners._E.Instance));
            }
        }
    }
}
