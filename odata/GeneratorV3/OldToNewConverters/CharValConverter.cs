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
                    new Inners._openpercentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENEↃ(
                        InnerConverter.Instance.Visit(inner, default))),
                DquoteConverter.Instance.Convert(charVal.CloseDquote));
        }

        private sealed class InnerConverter : CharVal.Inner.Visitor<Abnf.Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE, Root.Void>
        {
            private InnerConverter()
            {
            }

            public static InnerConverter Instance { get; } = new InnerConverter();

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x20 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOZEROⲻTWOONE(
                    new Inners._percentxTWOZEROⲻTWOONE._TWOZERO(
                        Inners._TWO.Instance,
                        Inners._ZERO.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x21 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOZEROⲻTWOONE(
                    new Inners._percentxTWOZEROⲻTWOONE._TWOONE(
                        Inners._TWO.Instance,
                        Inners._ONE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x23 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOTHREE(
                        Inners._TWO.Instance,
                        Inners._THREE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x24 node, Void context)
            {
                //// TODO start here
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOTHREE(
                        Inners._TWO.Instance,
                        Inners._THREE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x25 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOTHREE(
                        Inners._TWO.Instance,
                        Inners._THREE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x26 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOTHREE(
                        Inners._TWO.Instance,
                        Inners._THREE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x27 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOTHREE(
                        Inners._TWO.Instance,
                        Inners._THREE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x28 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOTHREE(
                        Inners._TWO.Instance,
                        Inners._THREE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x29 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOTHREE(
                        Inners._TWO.Instance,
                        Inners._THREE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x2A node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOTHREE(
                        Inners._TWO.Instance,
                        Inners._THREE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x2B node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOTHREE(
                        Inners._TWO.Instance,
                        Inners._THREE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x2C node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOTHREE(
                        Inners._TWO.Instance,
                        Inners._THREE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x2D node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOTHREE(
                        Inners._TWO.Instance,
                        Inners._THREE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x2E node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOTHREE(
                        Inners._TWO.Instance,
                        Inners._THREE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x2F node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOTHREE(
                        Inners._TWO.Instance,
                        Inners._THREE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x30 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOTHREE(
                        Inners._TWO.Instance,
                        Inners._THREE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x31 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOTHREE(
                        Inners._TWO.Instance,
                        Inners._THREE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x32 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOTHREE(
                        Inners._TWO.Instance,
                        Inners._THREE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x33 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOTHREE(
                        Inners._TWO.Instance,
                        Inners._THREE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x34 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOTHREE(
                        Inners._TWO.Instance,
                        Inners._THREE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x35 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOTHREE(
                        Inners._TWO.Instance,
                        Inners._THREE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x36 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOTHREE(
                        Inners._TWO.Instance,
                        Inners._THREE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x37 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOTHREE(
                        Inners._TWO.Instance,
                        Inners._THREE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x38 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOTHREE(
                        Inners._TWO.Instance,
                        Inners._THREE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x39 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOTHREE(
                        Inners._TWO.Instance,
                        Inners._THREE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x3A node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOTHREE(
                        Inners._TWO.Instance,
                        Inners._THREE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x3B node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOTHREE(
                        Inners._TWO.Instance,
                        Inners._THREE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x3C node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOTHREE(
                        Inners._TWO.Instance,
                        Inners._THREE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x3D node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOTHREE(
                        Inners._TWO.Instance,
                        Inners._THREE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x3E node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOTHREE(
                        Inners._TWO.Instance,
                        Inners._THREE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x3F node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOTHREE(
                        Inners._TWO.Instance,
                        Inners._THREE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x40 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOTHREE(
                        Inners._TWO.Instance,
                        Inners._THREE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x41 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOTHREE(
                        Inners._TWO.Instance,
                        Inners._THREE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x42 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOTHREE(
                        Inners._TWO.Instance,
                        Inners._THREE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x43 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOTHREE(
                        Inners._TWO.Instance,
                        Inners._THREE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x44 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOTHREE(
                        Inners._TWO.Instance,
                        Inners._THREE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x45 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOTHREE(
                        Inners._TWO.Instance,
                        Inners._THREE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x46 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOTHREE(
                        Inners._TWO.Instance,
                        Inners._THREE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x47 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOTHREE(
                        Inners._TWO.Instance,
                        Inners._THREE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x48 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOTHREE(
                        Inners._TWO.Instance,
                        Inners._THREE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x49 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOTHREE(
                        Inners._TWO.Instance,
                        Inners._THREE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x4A node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOTHREE(
                        Inners._TWO.Instance,
                        Inners._THREE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x4B node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOTHREE(
                        Inners._TWO.Instance,
                        Inners._THREE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x4C node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOTHREE(
                        Inners._TWO.Instance,
                        Inners._THREE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x4D node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOTHREE(
                        Inners._TWO.Instance,
                        Inners._THREE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x4E node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOTHREE(
                        Inners._TWO.Instance,
                        Inners._THREE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x4F node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOTHREE(
                        Inners._TWO.Instance,
                        Inners._THREE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x50 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOTHREE(
                        Inners._TWO.Instance,
                        Inners._THREE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x51 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOTHREE(
                        Inners._TWO.Instance,
                        Inners._THREE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x52 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOTHREE(
                        Inners._TWO.Instance,
                        Inners._THREE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x53 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOTHREE(
                        Inners._TWO.Instance,
                        Inners._THREE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x54 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOTHREE(
                        Inners._TWO.Instance,
                        Inners._THREE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x55 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOTHREE(
                        Inners._TWO.Instance,
                        Inners._THREE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x56 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOTHREE(
                        Inners._TWO.Instance,
                        Inners._THREE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x57 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOTHREE(
                        Inners._TWO.Instance,
                        Inners._THREE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x58 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOTHREE(
                        Inners._TWO.Instance,
                        Inners._THREE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x59 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOTHREE(
                        Inners._TWO.Instance,
                        Inners._THREE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x5A node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOTHREE(
                        Inners._TWO.Instance,
                        Inners._THREE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x5B node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOTHREE(
                        Inners._TWO.Instance,
                        Inners._THREE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x5C node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOTHREE(
                        Inners._TWO.Instance,
                        Inners._THREE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x5D node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOTHREE(
                        Inners._TWO.Instance,
                        Inners._THREE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x5E node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOTHREE(
                        Inners._TWO.Instance,
                        Inners._THREE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x5F node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOTHREE(
                        Inners._TWO.Instance,
                        Inners._THREE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x60 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOTHREE(
                        Inners._TWO.Instance,
                        Inners._THREE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x61 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOTHREE(
                        Inners._TWO.Instance,
                        Inners._THREE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x62 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOTHREE(
                        Inners._TWO.Instance,
                        Inners._THREE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x63 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOTHREE(
                        Inners._TWO.Instance,
                        Inners._THREE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x64 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOTHREE(
                        Inners._TWO.Instance,
                        Inners._THREE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x65 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOTHREE(
                        Inners._TWO.Instance,
                        Inners._THREE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x66 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOTHREE(
                        Inners._TWO.Instance,
                        Inners._THREE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x67 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOTHREE(
                        Inners._TWO.Instance,
                        Inners._THREE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x68 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOTHREE(
                        Inners._TWO.Instance,
                        Inners._THREE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x69 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOTHREE(
                        Inners._TWO.Instance,
                        Inners._THREE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x6A node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOTHREE(
                        Inners._TWO.Instance,
                        Inners._THREE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x6B node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOTHREE(
                        Inners._TWO.Instance,
                        Inners._THREE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x6C node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOTHREE(
                        Inners._TWO.Instance,
                        Inners._THREE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x6D node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOTHREE(
                        Inners._TWO.Instance,
                        Inners._THREE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x6E node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOTHREE(
                        Inners._TWO.Instance,
                        Inners._THREE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x6F node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOTHREE(
                        Inners._TWO.Instance,
                        Inners._THREE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x70 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOTHREE(
                        Inners._TWO.Instance,
                        Inners._THREE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x71 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOTHREE(
                        Inners._TWO.Instance,
                        Inners._THREE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x72 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOTHREE(
                        Inners._TWO.Instance,
                        Inners._THREE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x73 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOTHREE(
                        Inners._TWO.Instance,
                        Inners._THREE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x74 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOTHREE(
                        Inners._TWO.Instance,
                        Inners._THREE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x75 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOTHREE(
                        Inners._TWO.Instance,
                        Inners._THREE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x76 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOTHREE(
                        Inners._TWO.Instance,
                        Inners._THREE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x77 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOTHREE(
                        Inners._TWO.Instance,
                        Inners._THREE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x78 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOTHREE(
                        Inners._TWO.Instance,
                        Inners._THREE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x79 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOTHREE(
                        Inners._TWO.Instance,
                        Inners._THREE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x7A node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOTHREE(
                        Inners._TWO.Instance,
                        Inners._THREE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x7B node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOTHREE(
                        Inners._TWO.Instance,
                        Inners._THREE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x7C node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOTHREE(
                        Inners._TWO.Instance,
                        Inners._THREE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x7D node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOTHREE(
                        Inners._TWO.Instance,
                        Inners._THREE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x7E node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOTHREE(
                        Inners._TWO.Instance,
                        Inners._THREE.Instance));
            }
        }
    }
}
