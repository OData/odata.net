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
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOFOUR(
                        Inners._TWO.Instance,
                        Inners._FOUR.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x25 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOFIVE(
                        Inners._TWO.Instance,
                        Inners._FIVE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x26 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOSIX(
                        Inners._TWO.Instance,
                        Inners._SIX.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x27 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOSEVEN(
                        Inners._TWO.Instance,
                        Inners._SEVEN.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x28 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOEIGHT(
                        Inners._TWO.Instance,
                        Inners._EIGHT.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x29 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWONINE(
                        Inners._TWO.Instance,
                        Inners._NINE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x2A node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOA(
                        Inners._TWO.Instance,
                        Inners._A.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x2B node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOB(
                        Inners._TWO.Instance,
                        Inners._B.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x2C node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOC(
                        Inners._TWO.Instance,
                        Inners._C.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x2D node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOD(
                        Inners._TWO.Instance,
                        Inners._D.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x2E node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOE(
                        Inners._TWO.Instance,
                        Inners._E.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x2F node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._TWOF(
                        Inners._TWO.Instance,
                        Inners._F.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x30 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._THREEZERO(
                        Inners._THREE.Instance,
                        Inners._ZERO.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x31 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._THREEONE(
                        Inners._THREE.Instance,
                        Inners._ONE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x32 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._THREETWO(
                        Inners._THREE.Instance,
                        Inners._TWO.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x33 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._THREETHREE(
                        Inners._THREE.Instance,
                        Inners._THREE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x34 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._THREEFOUR(
                        Inners._THREE.Instance,
                        Inners._FOUR.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x35 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._THREEFIVE(
                        Inners._THREE.Instance,
                        Inners._FIVE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x36 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._THREESIX(
                        Inners._THREE.Instance,
                        Inners._SIX.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x37 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._THREESEVEN(
                        Inners._THREE.Instance,
                        Inners._SEVEN.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x38 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._THREEEIGHT(
                        Inners._THREE.Instance,
                        Inners._EIGHT.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x39 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._THREENINE(
                        Inners._THREE.Instance,
                        Inners._NINE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x3A node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._THREEA(
                        Inners._THREE.Instance,
                        Inners._A.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x3B node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._THREEB(
                        Inners._THREE.Instance,
                        Inners._B.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x3C node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._THREEC(
                        Inners._THREE.Instance,
                        Inners._C.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x3D node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._THREED(
                        Inners._THREE.Instance,
                        Inners._D.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x3E node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._THREEE(
                        Inners._THREE.Instance,
                        Inners._E.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x3F node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._THREEF(
                        Inners._THREE.Instance,
                        Inners._F.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x40 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._FOURZERO(
                        Inners._FOUR.Instance,
                        Inners._ZERO.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x41 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._FOURONE(
                        Inners._FOUR.Instance,
                        Inners._ONE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x42 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._FOURTWO(
                        Inners._FOUR.Instance,
                        Inners._TWO.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x43 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._FOURTHREE(
                        Inners._FOUR.Instance,
                        Inners._THREE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x44 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._FOURFOUR(
                        Inners._FOUR.Instance,
                        Inners._FOUR.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x45 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._FOURFIVE(
                        Inners._FOUR.Instance,
                        Inners._FIVE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x46 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._FOURSIX(
                        Inners._FOUR.Instance,
                        Inners._SIX.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x47 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._FOURSEVEN(
                        Inners._FOUR.Instance,
                        Inners._SEVEN.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x48 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._FOUREIGHT(
                        Inners._FOUR.Instance,
                        Inners._EIGHT.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x49 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._FOURNINE(
                        Inners._FOUR.Instance,
                        Inners._NINE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x4A node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._FOURA(
                        Inners._FOUR.Instance,
                        Inners._A.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x4B node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._FOURB(
                        Inners._FOUR.Instance,
                        Inners._B.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x4C node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._FOURC(
                        Inners._FOUR.Instance,
                        Inners._C.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x4D node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._FOURD(
                        Inners._FOUR.Instance,
                        Inners._D.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x4E node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._FOURE(
                        Inners._FOUR.Instance,
                        Inners._E.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x4F node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._FOURF(
                        Inners._FOUR.Instance,
                        Inners._F.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x50 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._FIVEZERO(
                        Inners._FIVE.Instance,
                        Inners._ZERO.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x51 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._FIVEONE(
                        Inners._FIVE.Instance,
                        Inners._ONE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x52 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._FIVETWO(
                        Inners._FIVE.Instance,
                        Inners._TWO.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x53 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._FIVETHREE(
                        Inners._FIVE.Instance,
                        Inners._THREE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x54 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._FIVEFOUR(
                        Inners._FIVE.Instance,
                        Inners._FOUR.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x55 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._FIVEFIVE(
                        Inners._FIVE.Instance,
                        Inners._FIVE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x56 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._FIVESIX(
                        Inners._FIVE.Instance,
                        Inners._SIX.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x57 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._FIVESEVEN(
                        Inners._FIVE.Instance,
                        Inners._SEVEN.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x58 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._FIVEEIGHT(
                        Inners._FIVE.Instance,
                        Inners._EIGHT.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x59 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._FIVENINE(
                        Inners._FIVE.Instance,
                        Inners._NINE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x5A node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._FIVEA(
                        Inners._FIVE.Instance,
                        Inners._A.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x5B node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._FIVEB(
                        Inners._FIVE.Instance,
                        Inners._B.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x5C node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._FIVEC(
                        Inners._FIVE.Instance,
                        Inners._C.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x5D node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._FIVED(
                        Inners._FIVE.Instance,
                        Inners._D.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x5E node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._FIVEE(
                        Inners._FIVE.Instance,
                        Inners._E.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x5F node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._FIVEF(
                        Inners._FIVE.Instance,
                        Inners._F.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x60 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._SIXZERO(
                        Inners._SIX.Instance,
                        Inners._ZERO.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x61 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._SIXONE(
                        Inners._SIX.Instance,
                        Inners._ONE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x62 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._SIXTWO(
                        Inners._SIX.Instance,
                        Inners._TWO.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x63 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._SIXTHREE(
                        Inners._SIX.Instance,
                        Inners._THREE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x64 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._SIXFOUR(
                        Inners._SIX.Instance,
                        Inners._FOUR.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x65 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._SIXFIVE(
                        Inners._SIX.Instance,
                        Inners._FIVE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x66 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._SIXSIX(
                        Inners._SIX.Instance,
                        Inners._SIX.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x67 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._SIXSEVEN(
                        Inners._SIX.Instance,
                        Inners._SEVEN.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x68 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._SIXEIGHT(
                        Inners._SIX.Instance,
                        Inners._EIGHT.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x69 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._SIXNINE(
                        Inners._SIX.Instance,
                        Inners._NINE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x6A node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._SIXA(
                        Inners._SIX.Instance,
                        Inners._A.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x6B node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._SIXB(
                        Inners._SIX.Instance,
                        Inners._B.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x6C node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._SIXC(
                        Inners._SIX.Instance,
                        Inners._C.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x6D node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._SIXD(
                        Inners._SIX.Instance,
                        Inners._D.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x6E node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._SIXE(
                        Inners._SIX.Instance,
                        Inners._E.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x6F node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._SIXF(
                        Inners._SIX.Instance,
                        Inners._F.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x70 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._SEVENZERO(
                        Inners._SEVEN.Instance,
                        Inners._ZERO.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x71 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._SEVENONE(
                        Inners._SEVEN.Instance,
                        Inners._ONE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x72 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._SEVENTWO(
                        Inners._SEVEN.Instance,
                        Inners._TWO.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x73 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._SEVENTHREE(
                        Inners._SEVEN.Instance,
                        Inners._THREE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x74 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._SEVENFOUR(
                        Inners._SEVEN.Instance,
                        Inners._FOUR.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x75 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._SEVENFIVE(
                        Inners._SEVEN.Instance,
                        Inners._FIVE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x76 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._SEVENSIX(
                        Inners._SEVEN.Instance,
                        Inners._SIX.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x77 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._SEVENSEVEN(
                        Inners._SEVEN.Instance,
                        Inners._SEVEN.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x78 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._SEVENEIGHT(
                        Inners._SEVEN.Instance,
                        Inners._EIGHT.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x79 node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._SEVENNINE(
                        Inners._SEVEN.Instance,
                        Inners._NINE.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x7A node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._SEVENA(
                        Inners._SEVEN.Instance,
                        Inners._A.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x7B node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._SEVENB(
                        Inners._SEVEN.Instance,
                        Inners._B.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x7C node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._SEVENC(
                        Inners._SEVEN.Instance,
                        Inners._C.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x7D node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._SEVEND(
                        Inners._SEVEN.Instance,
                        Inners._D.Instance));
            }

            protected internal override Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE Accept(CharVal.Inner.x7E node, Void context)
            {
                return new Inners._percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE._percentxTWOTHREEⲻSEVENE(
                    new Inners._percentxTWOTHREEⲻSEVENE._SEVENE(
                        Inners._SEVEN.Instance,
                        Inners._E.Instance));
            }
        }
    }
}
