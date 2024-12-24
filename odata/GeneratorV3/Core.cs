namespace GeneratorV3.Core
{
    using System.Collections.Generic;
    
    public abstract class ALPHA
    {
        private ALPHA()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(ALPHA node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(ALPHA.percentxFOURONEⲻFIVEA node, TContext context);
            protected internal abstract TResult Accept(ALPHA.percentxSIXONEⲻSEVENA node, TContext context);
        }
        
        public sealed class percentxFOURONEⲻFIVEA : ALPHA
        {
            public percentxFOURONEⲻFIVEA(Inners.percentxFOURONEⲻFIVEA percentxFOURONEⲻFIVEA_1)
            {
                this.percentxFOURONEⲻFIVEA_1 = percentxFOURONEⲻFIVEA_1;
            }
            
            public Inners.percentxFOURONEⲻFIVEA percentxFOURONEⲻFIVEA_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class percentxSIXONEⲻSEVENA : ALPHA
        {
            public percentxSIXONEⲻSEVENA(Inners.percentxSIXONEⲻSEVENA percentxSIXONEⲻSEVENA_1)
            {
                this.percentxSIXONEⲻSEVENA_1 = percentxSIXONEⲻSEVENA_1;
            }
            
            public Inners.percentxSIXONEⲻSEVENA percentxSIXONEⲻSEVENA_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
    public abstract class BIT
    {
        private BIT()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(BIT node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(BIT.doublequotex30doublequote node, TContext context);
            protected internal abstract TResult Accept(BIT.doublequotex31doublequote node, TContext context);
        }
        
        public sealed class doublequotex30doublequote : BIT
        {
            public doublequotex30doublequote(Inners.doublequotex30doublequote doublequotex30doublequote_1)
            {
                this.doublequotex30doublequote_1 = doublequotex30doublequote_1;
            }
            
            public Inners.doublequotex30doublequote doublequotex30doublequote_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class doublequotex31doublequote : BIT
        {
            public doublequotex31doublequote(Inners.doublequotex31doublequote doublequotex31doublequote_1)
            {
                this.doublequotex31doublequote_1 = doublequotex31doublequote_1;
            }
            
            public Inners.doublequotex31doublequote doublequotex31doublequote_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
    public sealed class CHAR
    {
        public CHAR(Inners.percentxZEROONEⲻSEVENF percentxZEROONEⲻSEVENF_1)
        {
            this.percentxZEROONEⲻSEVENF_1 = percentxZEROONEⲻSEVENF_1;
        }
        
        public Inners.percentxZEROONEⲻSEVENF percentxZEROONEⲻSEVENF_1 { get; }
    }
    
    public sealed class CR
    {
        public CR(Inners.percentxZEROD percentxZEROD_1)
        {
            this.percentxZEROD_1 = percentxZEROD_1;
        }
        
        public Inners.percentxZEROD percentxZEROD_1 { get; }
    }
    
    public sealed class CRLF
    {
        public CRLF(GeneratorV3.Core.CR CR_1, GeneratorV3.Core.LF LF_1)
        {
            this.CR_1 = CR_1;
            this.LF_1 = LF_1;
        }
        
        public GeneratorV3.Core.CR CR_1 { get; }
        public GeneratorV3.Core.LF LF_1 { get; }
    }
    
    public abstract class CTL
    {
        private CTL()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(CTL node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(CTL.percentxZEROZEROⲻONEF node, TContext context);
            protected internal abstract TResult Accept(CTL.percentxSEVENF node, TContext context);
        }
        
        public sealed class percentxZEROZEROⲻONEF : CTL
        {
            public percentxZEROZEROⲻONEF(Inners.percentxZEROZEROⲻONEF percentxZEROZEROⲻONEF_1)
            {
                this.percentxZEROZEROⲻONEF_1 = percentxZEROZEROⲻONEF_1;
            }
            
            public Inners.percentxZEROZEROⲻONEF percentxZEROZEROⲻONEF_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class percentxSEVENF : CTL
        {
            public percentxSEVENF(Inners.percentxSEVENF percentxSEVENF_1)
            {
                this.percentxSEVENF_1 = percentxSEVENF_1;
            }
            
            public Inners.percentxSEVENF percentxSEVENF_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
    public sealed class DIGIT
    {
        public DIGIT(Inners.percentxTHREEZEROⲻTHREENINE percentxTHREEZEROⲻTHREENINE_1)
        {
            this.percentxTHREEZEROⲻTHREENINE_1 = percentxTHREEZEROⲻTHREENINE_1;
        }
        
        public Inners.percentxTHREEZEROⲻTHREENINE percentxTHREEZEROⲻTHREENINE_1 { get; }
    }
    
    public sealed class DQUOTE
    {
        public DQUOTE(Inners.percentxTWOTWO percentxTWOTWO_1)
        {
            this.percentxTWOTWO_1 = percentxTWOTWO_1;
        }
        
        public Inners.percentxTWOTWO percentxTWOTWO_1 { get; }
    }
    
    public abstract class HEXDIG
    {
        private HEXDIG()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(HEXDIG node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(HEXDIG.DIGIT node, TContext context);
            protected internal abstract TResult Accept(HEXDIG.doublequotex41doublequote node, TContext context);
            protected internal abstract TResult Accept(HEXDIG.doublequotex42doublequote node, TContext context);
            protected internal abstract TResult Accept(HEXDIG.doublequotex43doublequote node, TContext context);
            protected internal abstract TResult Accept(HEXDIG.doublequotex44doublequote node, TContext context);
            protected internal abstract TResult Accept(HEXDIG.doublequotex45doublequote node, TContext context);
            protected internal abstract TResult Accept(HEXDIG.doublequotex46doublequote node, TContext context);
        }
        
        public sealed class DIGIT : HEXDIG
        {
            public DIGIT(GeneratorV3.Core.DIGIT DIGIT_1)
            {
                this.DIGIT_1 = DIGIT_1;
            }
            
            public GeneratorV3.Core.DIGIT DIGIT_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class doublequotex41doublequote : HEXDIG
        {
            public doublequotex41doublequote(Inners.doublequotex41doublequote doublequotex41doublequote_1)
            {
                this.doublequotex41doublequote_1 = doublequotex41doublequote_1;
            }
            
            public Inners.doublequotex41doublequote doublequotex41doublequote_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class doublequotex42doublequote : HEXDIG
        {
            public doublequotex42doublequote(Inners.doublequotex42doublequote doublequotex42doublequote_1)
            {
                this.doublequotex42doublequote_1 = doublequotex42doublequote_1;
            }
            
            public Inners.doublequotex42doublequote doublequotex42doublequote_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class doublequotex43doublequote : HEXDIG
        {
            public doublequotex43doublequote(Inners.doublequotex43doublequote doublequotex43doublequote_1)
            {
                this.doublequotex43doublequote_1 = doublequotex43doublequote_1;
            }
            
            public Inners.doublequotex43doublequote doublequotex43doublequote_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class doublequotex44doublequote : HEXDIG
        {
            public doublequotex44doublequote(Inners.doublequotex44doublequote doublequotex44doublequote_1)
            {
                this.doublequotex44doublequote_1 = doublequotex44doublequote_1;
            }
            
            public Inners.doublequotex44doublequote doublequotex44doublequote_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class doublequotex45doublequote : HEXDIG
        {
            public doublequotex45doublequote(Inners.doublequotex45doublequote doublequotex45doublequote_1)
            {
                this.doublequotex45doublequote_1 = doublequotex45doublequote_1;
            }
            
            public Inners.doublequotex45doublequote doublequotex45doublequote_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class doublequotex46doublequote : HEXDIG
        {
            public doublequotex46doublequote(Inners.doublequotex46doublequote doublequotex46doublequote_1)
            {
                this.doublequotex46doublequote_1 = doublequotex46doublequote_1;
            }
            
            public Inners.doublequotex46doublequote doublequotex46doublequote_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
    public sealed class HTAB
    {
        public HTAB(Inners.percentxZERONINE percentxZERONINE_1)
        {
            this.percentxZERONINE_1 = percentxZERONINE_1;
        }
        
        public Inners.percentxZERONINE percentxZERONINE_1 { get; }
    }
    
    public sealed class LF
    {
        public LF(Inners.percentxZEROA percentxZEROA_1)
        {
            this.percentxZEROA_1 = percentxZEROA_1;
        }
        
        public Inners.percentxZEROA percentxZEROA_1 { get; }
    }
    
    public sealed class LWSP
    {
        public LWSP(IEnumerable<Inners.openWSPⳆCRLF_WSPↃ> openWSPⳆCRLF_WSPↃ_1)
        {
            this.openWSPⳆCRLF_WSPↃ_1 = openWSPⳆCRLF_WSPↃ_1;
        }
        
        public IEnumerable<Inners.openWSPⳆCRLF_WSPↃ> openWSPⳆCRLF_WSPↃ_1 { get; }
    }
    
    public sealed class OCTET
    {
        public OCTET(Inners.percentxZEROZEROⲻFF percentxZEROZEROⲻFF_1)
        {
            this.percentxZEROZEROⲻFF_1 = percentxZEROZEROⲻFF_1;
        }
        
        public Inners.percentxZEROZEROⲻFF percentxZEROZEROⲻFF_1 { get; }
    }
    
    public sealed class SP
    {
        public SP(Inners.percentxTWOZERO percentxTWOZERO_1)
        {
            this.percentxTWOZERO_1 = percentxTWOZERO_1;
        }
        
        public Inners.percentxTWOZERO percentxTWOZERO_1 { get; }
    }
    
    public sealed class VCHAR
    {
        public VCHAR(Inners.percentxTWOONEⲻSEVENE percentxTWOONEⲻSEVENE_1)
        {
            this.percentxTWOONEⲻSEVENE_1 = percentxTWOONEⲻSEVENE_1;
        }
        
        public Inners.percentxTWOONEⲻSEVENE percentxTWOONEⲻSEVENE_1 { get; }
    }
    
    public abstract class WSP
    {
        private WSP()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(WSP node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(WSP.SP node, TContext context);
            protected internal abstract TResult Accept(WSP.HTAB node, TContext context);
        }
        
        public sealed class SP : WSP
        {
            public SP(GeneratorV3.Core.SP SP_1)
            {
                this.SP_1 = SP_1;
            }
            
            public GeneratorV3.Core.SP SP_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class HTAB : WSP
        {
            public HTAB(GeneratorV3.Core.HTAB HTAB_1)
            {
                this.HTAB_1 = HTAB_1;
            }
            
            public GeneratorV3.Core.HTAB HTAB_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
    public static class Inners
    {
        public sealed class FOUR
        {
            private FOUR()
            {
            }
            
            public static FOUR Instance { get; } = new FOUR();
        }
        
        public sealed class ONE
        {
            private ONE()
            {
            }
            
            public static ONE Instance { get; } = new ONE();
        }
        
        public sealed class TWO
        {
            private TWO()
            {
            }
            
            public static TWO Instance { get; } = new TWO();
        }
        
        public sealed class THREE
        {
            private THREE()
            {
            }
            
            public static THREE Instance { get; } = new THREE();
        }
        
        public sealed class FIVE
        {
            private FIVE()
            {
            }
            
            public static FIVE Instance { get; } = new FIVE();
        }
        
        public sealed class SIX
        {
            private SIX()
            {
            }
            
            public static SIX Instance { get; } = new SIX();
        }
        
        public sealed class SEVEN
        {
            private SEVEN()
            {
            }
            
            public static SEVEN Instance { get; } = new SEVEN();
        }
        
        public sealed class EIGHT
        {
            private EIGHT()
            {
            }
            
            public static EIGHT Instance { get; } = new EIGHT();
        }
        
        public sealed class NINE
        {
            private NINE()
            {
            }
            
            public static NINE Instance { get; } = new NINE();
        }
        
        public sealed class A
        {
            private A()
            {
            }
            
            public static A Instance { get; } = new A();
        }
        
        public sealed class B
        {
            private B()
            {
            }
            
            public static B Instance { get; } = new B();
        }
        
        public sealed class C
        {
            private C()
            {
            }
            
            public static C Instance { get; } = new C();
        }
        
        public sealed class D
        {
            private D()
            {
            }
            
            public static D Instance { get; } = new D();
        }
        
        public sealed class E
        {
            private E()
            {
            }
            
            public static E Instance { get; } = new E();
        }
        
        public sealed class F
        {
            private F()
            {
            }
            
            public static F Instance { get; } = new F();
        }
        
        public sealed class ZERO
        {
            private ZERO()
            {
            }
            
            public static ZERO Instance { get; } = new ZERO();
        }
        
        public abstract class percentxFOURONEⲻFIVEA
        {
            private percentxFOURONEⲻFIVEA()
            {
            }
            
            protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
            
            public abstract class Visitor<TResult, TContext>
            {
                public TResult Visit(percentxFOURONEⲻFIVEA node, TContext context)
                {
                    return node.Dispatch(this, context);
                }
                
                protected internal abstract TResult Accept(percentxFOURONEⲻFIVEA.FOURONE node, TContext context);
                protected internal abstract TResult Accept(percentxFOURONEⲻFIVEA.FOURTWO node, TContext context);
                protected internal abstract TResult Accept(percentxFOURONEⲻFIVEA.FOURTHREE node, TContext context);
                protected internal abstract TResult Accept(percentxFOURONEⲻFIVEA.FOURFOUR node, TContext context);
                protected internal abstract TResult Accept(percentxFOURONEⲻFIVEA.FOURFIVE node, TContext context);
                protected internal abstract TResult Accept(percentxFOURONEⲻFIVEA.FOURSIX node, TContext context);
                protected internal abstract TResult Accept(percentxFOURONEⲻFIVEA.FOURSEVEN node, TContext context);
                protected internal abstract TResult Accept(percentxFOURONEⲻFIVEA.FOUREIGHT node, TContext context);
                protected internal abstract TResult Accept(percentxFOURONEⲻFIVEA.FOURNINE node, TContext context);
                protected internal abstract TResult Accept(percentxFOURONEⲻFIVEA.FOURA node, TContext context);
                protected internal abstract TResult Accept(percentxFOURONEⲻFIVEA.FOURB node, TContext context);
                protected internal abstract TResult Accept(percentxFOURONEⲻFIVEA.FOURC node, TContext context);
                protected internal abstract TResult Accept(percentxFOURONEⲻFIVEA.FOURD node, TContext context);
                protected internal abstract TResult Accept(percentxFOURONEⲻFIVEA.FOURE node, TContext context);
                protected internal abstract TResult Accept(percentxFOURONEⲻFIVEA.FOURF node, TContext context);
                protected internal abstract TResult Accept(percentxFOURONEⲻFIVEA.FIVEZERO node, TContext context);
                protected internal abstract TResult Accept(percentxFOURONEⲻFIVEA.FIVEONE node, TContext context);
                protected internal abstract TResult Accept(percentxFOURONEⲻFIVEA.FIVETWO node, TContext context);
                protected internal abstract TResult Accept(percentxFOURONEⲻFIVEA.FIVETHREE node, TContext context);
                protected internal abstract TResult Accept(percentxFOURONEⲻFIVEA.FIVEFOUR node, TContext context);
                protected internal abstract TResult Accept(percentxFOURONEⲻFIVEA.FIVEFIVE node, TContext context);
                protected internal abstract TResult Accept(percentxFOURONEⲻFIVEA.FIVESIX node, TContext context);
                protected internal abstract TResult Accept(percentxFOURONEⲻFIVEA.FIVESEVEN node, TContext context);
                protected internal abstract TResult Accept(percentxFOURONEⲻFIVEA.FIVEEIGHT node, TContext context);
                protected internal abstract TResult Accept(percentxFOURONEⲻFIVEA.FIVENINE node, TContext context);
                protected internal abstract TResult Accept(percentxFOURONEⲻFIVEA.FIVEA node, TContext context);
            }
            
            public sealed class FOURONE : percentxFOURONEⲻFIVEA
            {
                public FOURONE(Inners.FOUR FOUR_1, Inners.ONE ONE_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.ONE_1 = ONE_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.ONE ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURTWO : percentxFOURONEⲻFIVEA
            {
                public FOURTWO(Inners.FOUR FOUR_1, Inners.TWO TWO_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.TWO_1 = TWO_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.TWO TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURTHREE : percentxFOURONEⲻFIVEA
            {
                public FOURTHREE(Inners.FOUR FOUR_1, Inners.THREE THREE_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.THREE_1 = THREE_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.THREE THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURFOUR : percentxFOURONEⲻFIVEA
            {
                public FOURFOUR(Inners.FOUR FOUR_1, Inners.FOUR FOUR_2)
                {
                    this.FOUR_1 = FOUR_1;
                    this.FOUR_2 = FOUR_2;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.FOUR FOUR_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURFIVE : percentxFOURONEⲻFIVEA
            {
                public FOURFIVE(Inners.FOUR FOUR_1, Inners.FIVE FIVE_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.FIVE_1 = FIVE_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.FIVE FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURSIX : percentxFOURONEⲻFIVEA
            {
                public FOURSIX(Inners.FOUR FOUR_1, Inners.SIX SIX_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.SIX_1 = SIX_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.SIX SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURSEVEN : percentxFOURONEⲻFIVEA
            {
                public FOURSEVEN(Inners.FOUR FOUR_1, Inners.SEVEN SEVEN_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.SEVEN_1 = SEVEN_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.SEVEN SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOUREIGHT : percentxFOURONEⲻFIVEA
            {
                public FOUREIGHT(Inners.FOUR FOUR_1, Inners.EIGHT EIGHT_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.EIGHT_1 = EIGHT_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.EIGHT EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURNINE : percentxFOURONEⲻFIVEA
            {
                public FOURNINE(Inners.FOUR FOUR_1, Inners.NINE NINE_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.NINE_1 = NINE_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.NINE NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURA : percentxFOURONEⲻFIVEA
            {
                public FOURA(Inners.FOUR FOUR_1, Inners.A A_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.A_1 = A_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.A A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURB : percentxFOURONEⲻFIVEA
            {
                public FOURB(Inners.FOUR FOUR_1, Inners.B B_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.B_1 = B_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.B B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURC : percentxFOURONEⲻFIVEA
            {
                public FOURC(Inners.FOUR FOUR_1, Inners.C C_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.C_1 = C_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.C C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURD : percentxFOURONEⲻFIVEA
            {
                public FOURD(Inners.FOUR FOUR_1, Inners.D D_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.D_1 = D_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.D D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURE : percentxFOURONEⲻFIVEA
            {
                public FOURE(Inners.FOUR FOUR_1, Inners.E E_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.E_1 = E_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.E E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURF : percentxFOURONEⲻFIVEA
            {
                public FOURF(Inners.FOUR FOUR_1, Inners.F F_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.F_1 = F_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.F F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVEZERO : percentxFOURONEⲻFIVEA
            {
                public FIVEZERO(Inners.FIVE FIVE_1, Inners.ZERO ZERO_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.ZERO_1 = ZERO_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.ZERO ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVEONE : percentxFOURONEⲻFIVEA
            {
                public FIVEONE(Inners.FIVE FIVE_1, Inners.ONE ONE_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.ONE_1 = ONE_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.ONE ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVETWO : percentxFOURONEⲻFIVEA
            {
                public FIVETWO(Inners.FIVE FIVE_1, Inners.TWO TWO_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.TWO_1 = TWO_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.TWO TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVETHREE : percentxFOURONEⲻFIVEA
            {
                public FIVETHREE(Inners.FIVE FIVE_1, Inners.THREE THREE_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.THREE_1 = THREE_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.THREE THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVEFOUR : percentxFOURONEⲻFIVEA
            {
                public FIVEFOUR(Inners.FIVE FIVE_1, Inners.FOUR FOUR_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.FOUR_1 = FOUR_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.FOUR FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVEFIVE : percentxFOURONEⲻFIVEA
            {
                public FIVEFIVE(Inners.FIVE FIVE_1, Inners.FIVE FIVE_2)
                {
                    this.FIVE_1 = FIVE_1;
                    this.FIVE_2 = FIVE_2;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.FIVE FIVE_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVESIX : percentxFOURONEⲻFIVEA
            {
                public FIVESIX(Inners.FIVE FIVE_1, Inners.SIX SIX_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.SIX_1 = SIX_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.SIX SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVESEVEN : percentxFOURONEⲻFIVEA
            {
                public FIVESEVEN(Inners.FIVE FIVE_1, Inners.SEVEN SEVEN_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.SEVEN_1 = SEVEN_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.SEVEN SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVEEIGHT : percentxFOURONEⲻFIVEA
            {
                public FIVEEIGHT(Inners.FIVE FIVE_1, Inners.EIGHT EIGHT_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.EIGHT_1 = EIGHT_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.EIGHT EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVENINE : percentxFOURONEⲻFIVEA
            {
                public FIVENINE(Inners.FIVE FIVE_1, Inners.NINE NINE_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.NINE_1 = NINE_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.NINE NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVEA : percentxFOURONEⲻFIVEA
            {
                public FIVEA(Inners.FIVE FIVE_1, Inners.A A_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.A_1 = A_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.A A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
        }
        
        public abstract class percentxSIXONEⲻSEVENA
        {
            private percentxSIXONEⲻSEVENA()
            {
            }
            
            protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
            
            public abstract class Visitor<TResult, TContext>
            {
                public TResult Visit(percentxSIXONEⲻSEVENA node, TContext context)
                {
                    return node.Dispatch(this, context);
                }
                
                protected internal abstract TResult Accept(percentxSIXONEⲻSEVENA.SIXONE node, TContext context);
                protected internal abstract TResult Accept(percentxSIXONEⲻSEVENA.SIXTWO node, TContext context);
                protected internal abstract TResult Accept(percentxSIXONEⲻSEVENA.SIXTHREE node, TContext context);
                protected internal abstract TResult Accept(percentxSIXONEⲻSEVENA.SIXFOUR node, TContext context);
                protected internal abstract TResult Accept(percentxSIXONEⲻSEVENA.SIXFIVE node, TContext context);
                protected internal abstract TResult Accept(percentxSIXONEⲻSEVENA.SIXSIX node, TContext context);
                protected internal abstract TResult Accept(percentxSIXONEⲻSEVENA.SIXSEVEN node, TContext context);
                protected internal abstract TResult Accept(percentxSIXONEⲻSEVENA.SIXEIGHT node, TContext context);
                protected internal abstract TResult Accept(percentxSIXONEⲻSEVENA.SIXNINE node, TContext context);
                protected internal abstract TResult Accept(percentxSIXONEⲻSEVENA.SIXA node, TContext context);
                protected internal abstract TResult Accept(percentxSIXONEⲻSEVENA.SIXB node, TContext context);
                protected internal abstract TResult Accept(percentxSIXONEⲻSEVENA.SIXC node, TContext context);
                protected internal abstract TResult Accept(percentxSIXONEⲻSEVENA.SIXD node, TContext context);
                protected internal abstract TResult Accept(percentxSIXONEⲻSEVENA.SIXE node, TContext context);
                protected internal abstract TResult Accept(percentxSIXONEⲻSEVENA.SIXF node, TContext context);
                protected internal abstract TResult Accept(percentxSIXONEⲻSEVENA.SEVENZERO node, TContext context);
                protected internal abstract TResult Accept(percentxSIXONEⲻSEVENA.SEVENONE node, TContext context);
                protected internal abstract TResult Accept(percentxSIXONEⲻSEVENA.SEVENTWO node, TContext context);
                protected internal abstract TResult Accept(percentxSIXONEⲻSEVENA.SEVENTHREE node, TContext context);
                protected internal abstract TResult Accept(percentxSIXONEⲻSEVENA.SEVENFOUR node, TContext context);
                protected internal abstract TResult Accept(percentxSIXONEⲻSEVENA.SEVENFIVE node, TContext context);
                protected internal abstract TResult Accept(percentxSIXONEⲻSEVENA.SEVENSIX node, TContext context);
                protected internal abstract TResult Accept(percentxSIXONEⲻSEVENA.SEVENSEVEN node, TContext context);
                protected internal abstract TResult Accept(percentxSIXONEⲻSEVENA.SEVENEIGHT node, TContext context);
                protected internal abstract TResult Accept(percentxSIXONEⲻSEVENA.SEVENNINE node, TContext context);
                protected internal abstract TResult Accept(percentxSIXONEⲻSEVENA.SEVENA node, TContext context);
            }
            
            public sealed class SIXONE : percentxSIXONEⲻSEVENA
            {
                public SIXONE(Inners.SIX SIX_1, Inners.ONE ONE_1)
                {
                    this.SIX_1 = SIX_1;
                    this.ONE_1 = ONE_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.ONE ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXTWO : percentxSIXONEⲻSEVENA
            {
                public SIXTWO(Inners.SIX SIX_1, Inners.TWO TWO_1)
                {
                    this.SIX_1 = SIX_1;
                    this.TWO_1 = TWO_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.TWO TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXTHREE : percentxSIXONEⲻSEVENA
            {
                public SIXTHREE(Inners.SIX SIX_1, Inners.THREE THREE_1)
                {
                    this.SIX_1 = SIX_1;
                    this.THREE_1 = THREE_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.THREE THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXFOUR : percentxSIXONEⲻSEVENA
            {
                public SIXFOUR(Inners.SIX SIX_1, Inners.FOUR FOUR_1)
                {
                    this.SIX_1 = SIX_1;
                    this.FOUR_1 = FOUR_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.FOUR FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXFIVE : percentxSIXONEⲻSEVENA
            {
                public SIXFIVE(Inners.SIX SIX_1, Inners.FIVE FIVE_1)
                {
                    this.SIX_1 = SIX_1;
                    this.FIVE_1 = FIVE_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.FIVE FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXSIX : percentxSIXONEⲻSEVENA
            {
                public SIXSIX(Inners.SIX SIX_1, Inners.SIX SIX_2)
                {
                    this.SIX_1 = SIX_1;
                    this.SIX_2 = SIX_2;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.SIX SIX_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXSEVEN : percentxSIXONEⲻSEVENA
            {
                public SIXSEVEN(Inners.SIX SIX_1, Inners.SEVEN SEVEN_1)
                {
                    this.SIX_1 = SIX_1;
                    this.SEVEN_1 = SEVEN_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.SEVEN SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXEIGHT : percentxSIXONEⲻSEVENA
            {
                public SIXEIGHT(Inners.SIX SIX_1, Inners.EIGHT EIGHT_1)
                {
                    this.SIX_1 = SIX_1;
                    this.EIGHT_1 = EIGHT_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.EIGHT EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXNINE : percentxSIXONEⲻSEVENA
            {
                public SIXNINE(Inners.SIX SIX_1, Inners.NINE NINE_1)
                {
                    this.SIX_1 = SIX_1;
                    this.NINE_1 = NINE_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.NINE NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXA : percentxSIXONEⲻSEVENA
            {
                public SIXA(Inners.SIX SIX_1, Inners.A A_1)
                {
                    this.SIX_1 = SIX_1;
                    this.A_1 = A_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.A A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXB : percentxSIXONEⲻSEVENA
            {
                public SIXB(Inners.SIX SIX_1, Inners.B B_1)
                {
                    this.SIX_1 = SIX_1;
                    this.B_1 = B_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.B B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXC : percentxSIXONEⲻSEVENA
            {
                public SIXC(Inners.SIX SIX_1, Inners.C C_1)
                {
                    this.SIX_1 = SIX_1;
                    this.C_1 = C_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.C C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXD : percentxSIXONEⲻSEVENA
            {
                public SIXD(Inners.SIX SIX_1, Inners.D D_1)
                {
                    this.SIX_1 = SIX_1;
                    this.D_1 = D_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.D D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXE : percentxSIXONEⲻSEVENA
            {
                public SIXE(Inners.SIX SIX_1, Inners.E E_1)
                {
                    this.SIX_1 = SIX_1;
                    this.E_1 = E_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.E E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXF : percentxSIXONEⲻSEVENA
            {
                public SIXF(Inners.SIX SIX_1, Inners.F F_1)
                {
                    this.SIX_1 = SIX_1;
                    this.F_1 = F_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.F F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENZERO : percentxSIXONEⲻSEVENA
            {
                public SEVENZERO(Inners.SEVEN SEVEN_1, Inners.ZERO ZERO_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.ZERO_1 = ZERO_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.ZERO ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENONE : percentxSIXONEⲻSEVENA
            {
                public SEVENONE(Inners.SEVEN SEVEN_1, Inners.ONE ONE_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.ONE_1 = ONE_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.ONE ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENTWO : percentxSIXONEⲻSEVENA
            {
                public SEVENTWO(Inners.SEVEN SEVEN_1, Inners.TWO TWO_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.TWO_1 = TWO_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.TWO TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENTHREE : percentxSIXONEⲻSEVENA
            {
                public SEVENTHREE(Inners.SEVEN SEVEN_1, Inners.THREE THREE_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.THREE_1 = THREE_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.THREE THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENFOUR : percentxSIXONEⲻSEVENA
            {
                public SEVENFOUR(Inners.SEVEN SEVEN_1, Inners.FOUR FOUR_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.FOUR_1 = FOUR_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.FOUR FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENFIVE : percentxSIXONEⲻSEVENA
            {
                public SEVENFIVE(Inners.SEVEN SEVEN_1, Inners.FIVE FIVE_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.FIVE_1 = FIVE_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.FIVE FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENSIX : percentxSIXONEⲻSEVENA
            {
                public SEVENSIX(Inners.SEVEN SEVEN_1, Inners.SIX SIX_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.SIX_1 = SIX_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.SIX SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENSEVEN : percentxSIXONEⲻSEVENA
            {
                public SEVENSEVEN(Inners.SEVEN SEVEN_1, Inners.SEVEN SEVEN_2)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.SEVEN_2 = SEVEN_2;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.SEVEN SEVEN_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENEIGHT : percentxSIXONEⲻSEVENA
            {
                public SEVENEIGHT(Inners.SEVEN SEVEN_1, Inners.EIGHT EIGHT_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.EIGHT_1 = EIGHT_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.EIGHT EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENNINE : percentxSIXONEⲻSEVENA
            {
                public SEVENNINE(Inners.SEVEN SEVEN_1, Inners.NINE NINE_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.NINE_1 = NINE_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.NINE NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENA : percentxSIXONEⲻSEVENA
            {
                public SEVENA(Inners.SEVEN SEVEN_1, Inners.A A_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.A_1 = A_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.A A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
        }
        
        public sealed class x30
        {
            private x30()
            {
            }
            
            public static x30 Instance { get; } = new x30();
        }
        
        public sealed class doublequotex30doublequote
        {
            public doublequotex30doublequote(Inners.x30 x30_1)
            {
                this.x30_1 = x30_1;
            }
            
            public Inners.x30 x30_1 { get; }
        }
        
        public sealed class x31
        {
            private x31()
            {
            }
            
            public static x31 Instance { get; } = new x31();
        }
        
        public sealed class doublequotex31doublequote
        {
            public doublequotex31doublequote(Inners.x31 x31_1)
            {
                this.x31_1 = x31_1;
            }
            
            public Inners.x31 x31_1 { get; }
        }
        
        public abstract class percentxZEROONEⲻSEVENF
        {
            private percentxZEROONEⲻSEVENF()
            {
            }
            
            protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
            
            public abstract class Visitor<TResult, TContext>
            {
                public TResult Visit(percentxZEROONEⲻSEVENF node, TContext context)
                {
                    return node.Dispatch(this, context);
                }
                
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.ZEROONE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.ZEROTWO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.ZEROTHREE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.ZEROFOUR node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.ZEROFIVE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.ZEROSIX node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.ZEROSEVEN node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.ZEROEIGHT node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.ZERONINE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.ZEROA node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.ZEROB node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.ZEROC node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.ZEROD node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.ZEROE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.ZEROF node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.ONEZERO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.ONEONE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.ONETWO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.ONETHREE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.ONEFOUR node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.ONEFIVE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.ONESIX node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.ONESEVEN node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.ONEEIGHT node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.ONENINE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.ONEA node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.ONEB node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.ONEC node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.ONED node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.ONEE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.ONEF node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.TWOZERO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.TWOONE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.TWOTWO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.TWOTHREE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.TWOFOUR node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.TWOFIVE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.TWOSIX node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.TWOSEVEN node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.TWOEIGHT node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.TWONINE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.TWOA node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.TWOB node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.TWOC node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.TWOD node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.TWOE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.TWOF node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.THREEZERO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.THREEONE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.THREETWO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.THREETHREE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.THREEFOUR node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.THREEFIVE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.THREESIX node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.THREESEVEN node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.THREEEIGHT node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.THREENINE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.THREEA node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.THREEB node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.THREEC node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.THREED node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.THREEE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.THREEF node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.FOURZERO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.FOURONE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.FOURTWO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.FOURTHREE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.FOURFOUR node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.FOURFIVE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.FOURSIX node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.FOURSEVEN node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.FOUREIGHT node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.FOURNINE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.FOURA node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.FOURB node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.FOURC node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.FOURD node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.FOURE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.FOURF node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.FIVEZERO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.FIVEONE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.FIVETWO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.FIVETHREE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.FIVEFOUR node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.FIVEFIVE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.FIVESIX node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.FIVESEVEN node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.FIVEEIGHT node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.FIVENINE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.FIVEA node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.FIVEB node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.FIVEC node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.FIVED node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.FIVEE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.FIVEF node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.SIXZERO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.SIXONE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.SIXTWO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.SIXTHREE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.SIXFOUR node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.SIXFIVE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.SIXSIX node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.SIXSEVEN node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.SIXEIGHT node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.SIXNINE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.SIXA node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.SIXB node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.SIXC node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.SIXD node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.SIXE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.SIXF node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.SEVENZERO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.SEVENONE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.SEVENTWO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.SEVENTHREE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.SEVENFOUR node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.SEVENFIVE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.SEVENSIX node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.SEVENSEVEN node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.SEVENEIGHT node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.SEVENNINE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.SEVENA node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.SEVENB node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.SEVENC node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.SEVEND node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.SEVENE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.SEVENF node, TContext context);
            }
            
            public sealed class ZEROONE : percentxZEROONEⲻSEVENF
            {
                public ZEROONE(Inners.ZERO ZERO_1, Inners.ONE ONE_1)
                {
                    this.ZERO_1 = ZERO_1;
                    this.ONE_1 = ONE_1;
                }
                
                public Inners.ZERO ZERO_1 { get; }
                public Inners.ONE ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ZEROTWO : percentxZEROONEⲻSEVENF
            {
                public ZEROTWO(Inners.ZERO ZERO_1, Inners.TWO TWO_1)
                {
                    this.ZERO_1 = ZERO_1;
                    this.TWO_1 = TWO_1;
                }
                
                public Inners.ZERO ZERO_1 { get; }
                public Inners.TWO TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ZEROTHREE : percentxZEROONEⲻSEVENF
            {
                public ZEROTHREE(Inners.ZERO ZERO_1, Inners.THREE THREE_1)
                {
                    this.ZERO_1 = ZERO_1;
                    this.THREE_1 = THREE_1;
                }
                
                public Inners.ZERO ZERO_1 { get; }
                public Inners.THREE THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ZEROFOUR : percentxZEROONEⲻSEVENF
            {
                public ZEROFOUR(Inners.ZERO ZERO_1, Inners.FOUR FOUR_1)
                {
                    this.ZERO_1 = ZERO_1;
                    this.FOUR_1 = FOUR_1;
                }
                
                public Inners.ZERO ZERO_1 { get; }
                public Inners.FOUR FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ZEROFIVE : percentxZEROONEⲻSEVENF
            {
                public ZEROFIVE(Inners.ZERO ZERO_1, Inners.FIVE FIVE_1)
                {
                    this.ZERO_1 = ZERO_1;
                    this.FIVE_1 = FIVE_1;
                }
                
                public Inners.ZERO ZERO_1 { get; }
                public Inners.FIVE FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ZEROSIX : percentxZEROONEⲻSEVENF
            {
                public ZEROSIX(Inners.ZERO ZERO_1, Inners.SIX SIX_1)
                {
                    this.ZERO_1 = ZERO_1;
                    this.SIX_1 = SIX_1;
                }
                
                public Inners.ZERO ZERO_1 { get; }
                public Inners.SIX SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ZEROSEVEN : percentxZEROONEⲻSEVENF
            {
                public ZEROSEVEN(Inners.ZERO ZERO_1, Inners.SEVEN SEVEN_1)
                {
                    this.ZERO_1 = ZERO_1;
                    this.SEVEN_1 = SEVEN_1;
                }
                
                public Inners.ZERO ZERO_1 { get; }
                public Inners.SEVEN SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ZEROEIGHT : percentxZEROONEⲻSEVENF
            {
                public ZEROEIGHT(Inners.ZERO ZERO_1, Inners.EIGHT EIGHT_1)
                {
                    this.ZERO_1 = ZERO_1;
                    this.EIGHT_1 = EIGHT_1;
                }
                
                public Inners.ZERO ZERO_1 { get; }
                public Inners.EIGHT EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ZERONINE : percentxZEROONEⲻSEVENF
            {
                public ZERONINE(Inners.ZERO ZERO_1, Inners.NINE NINE_1)
                {
                    this.ZERO_1 = ZERO_1;
                    this.NINE_1 = NINE_1;
                }
                
                public Inners.ZERO ZERO_1 { get; }
                public Inners.NINE NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ZEROA : percentxZEROONEⲻSEVENF
            {
                public ZEROA(Inners.ZERO ZERO_1, Inners.A A_1)
                {
                    this.ZERO_1 = ZERO_1;
                    this.A_1 = A_1;
                }
                
                public Inners.ZERO ZERO_1 { get; }
                public Inners.A A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ZEROB : percentxZEROONEⲻSEVENF
            {
                public ZEROB(Inners.ZERO ZERO_1, Inners.B B_1)
                {
                    this.ZERO_1 = ZERO_1;
                    this.B_1 = B_1;
                }
                
                public Inners.ZERO ZERO_1 { get; }
                public Inners.B B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ZEROC : percentxZEROONEⲻSEVENF
            {
                public ZEROC(Inners.ZERO ZERO_1, Inners.C C_1)
                {
                    this.ZERO_1 = ZERO_1;
                    this.C_1 = C_1;
                }
                
                public Inners.ZERO ZERO_1 { get; }
                public Inners.C C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ZEROD : percentxZEROONEⲻSEVENF
            {
                public ZEROD(Inners.ZERO ZERO_1, Inners.D D_1)
                {
                    this.ZERO_1 = ZERO_1;
                    this.D_1 = D_1;
                }
                
                public Inners.ZERO ZERO_1 { get; }
                public Inners.D D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ZEROE : percentxZEROONEⲻSEVENF
            {
                public ZEROE(Inners.ZERO ZERO_1, Inners.E E_1)
                {
                    this.ZERO_1 = ZERO_1;
                    this.E_1 = E_1;
                }
                
                public Inners.ZERO ZERO_1 { get; }
                public Inners.E E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ZEROF : percentxZEROONEⲻSEVENF
            {
                public ZEROF(Inners.ZERO ZERO_1, Inners.F F_1)
                {
                    this.ZERO_1 = ZERO_1;
                    this.F_1 = F_1;
                }
                
                public Inners.ZERO ZERO_1 { get; }
                public Inners.F F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ONEZERO : percentxZEROONEⲻSEVENF
            {
                public ONEZERO(Inners.ONE ONE_1, Inners.ZERO ZERO_1)
                {
                    this.ONE_1 = ONE_1;
                    this.ZERO_1 = ZERO_1;
                }
                
                public Inners.ONE ONE_1 { get; }
                public Inners.ZERO ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ONEONE : percentxZEROONEⲻSEVENF
            {
                public ONEONE(Inners.ONE ONE_1, Inners.ONE ONE_2)
                {
                    this.ONE_1 = ONE_1;
                    this.ONE_2 = ONE_2;
                }
                
                public Inners.ONE ONE_1 { get; }
                public Inners.ONE ONE_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ONETWO : percentxZEROONEⲻSEVENF
            {
                public ONETWO(Inners.ONE ONE_1, Inners.TWO TWO_1)
                {
                    this.ONE_1 = ONE_1;
                    this.TWO_1 = TWO_1;
                }
                
                public Inners.ONE ONE_1 { get; }
                public Inners.TWO TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ONETHREE : percentxZEROONEⲻSEVENF
            {
                public ONETHREE(Inners.ONE ONE_1, Inners.THREE THREE_1)
                {
                    this.ONE_1 = ONE_1;
                    this.THREE_1 = THREE_1;
                }
                
                public Inners.ONE ONE_1 { get; }
                public Inners.THREE THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ONEFOUR : percentxZEROONEⲻSEVENF
            {
                public ONEFOUR(Inners.ONE ONE_1, Inners.FOUR FOUR_1)
                {
                    this.ONE_1 = ONE_1;
                    this.FOUR_1 = FOUR_1;
                }
                
                public Inners.ONE ONE_1 { get; }
                public Inners.FOUR FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ONEFIVE : percentxZEROONEⲻSEVENF
            {
                public ONEFIVE(Inners.ONE ONE_1, Inners.FIVE FIVE_1)
                {
                    this.ONE_1 = ONE_1;
                    this.FIVE_1 = FIVE_1;
                }
                
                public Inners.ONE ONE_1 { get; }
                public Inners.FIVE FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ONESIX : percentxZEROONEⲻSEVENF
            {
                public ONESIX(Inners.ONE ONE_1, Inners.SIX SIX_1)
                {
                    this.ONE_1 = ONE_1;
                    this.SIX_1 = SIX_1;
                }
                
                public Inners.ONE ONE_1 { get; }
                public Inners.SIX SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ONESEVEN : percentxZEROONEⲻSEVENF
            {
                public ONESEVEN(Inners.ONE ONE_1, Inners.SEVEN SEVEN_1)
                {
                    this.ONE_1 = ONE_1;
                    this.SEVEN_1 = SEVEN_1;
                }
                
                public Inners.ONE ONE_1 { get; }
                public Inners.SEVEN SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ONEEIGHT : percentxZEROONEⲻSEVENF
            {
                public ONEEIGHT(Inners.ONE ONE_1, Inners.EIGHT EIGHT_1)
                {
                    this.ONE_1 = ONE_1;
                    this.EIGHT_1 = EIGHT_1;
                }
                
                public Inners.ONE ONE_1 { get; }
                public Inners.EIGHT EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ONENINE : percentxZEROONEⲻSEVENF
            {
                public ONENINE(Inners.ONE ONE_1, Inners.NINE NINE_1)
                {
                    this.ONE_1 = ONE_1;
                    this.NINE_1 = NINE_1;
                }
                
                public Inners.ONE ONE_1 { get; }
                public Inners.NINE NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ONEA : percentxZEROONEⲻSEVENF
            {
                public ONEA(Inners.ONE ONE_1, Inners.A A_1)
                {
                    this.ONE_1 = ONE_1;
                    this.A_1 = A_1;
                }
                
                public Inners.ONE ONE_1 { get; }
                public Inners.A A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ONEB : percentxZEROONEⲻSEVENF
            {
                public ONEB(Inners.ONE ONE_1, Inners.B B_1)
                {
                    this.ONE_1 = ONE_1;
                    this.B_1 = B_1;
                }
                
                public Inners.ONE ONE_1 { get; }
                public Inners.B B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ONEC : percentxZEROONEⲻSEVENF
            {
                public ONEC(Inners.ONE ONE_1, Inners.C C_1)
                {
                    this.ONE_1 = ONE_1;
                    this.C_1 = C_1;
                }
                
                public Inners.ONE ONE_1 { get; }
                public Inners.C C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ONED : percentxZEROONEⲻSEVENF
            {
                public ONED(Inners.ONE ONE_1, Inners.D D_1)
                {
                    this.ONE_1 = ONE_1;
                    this.D_1 = D_1;
                }
                
                public Inners.ONE ONE_1 { get; }
                public Inners.D D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ONEE : percentxZEROONEⲻSEVENF
            {
                public ONEE(Inners.ONE ONE_1, Inners.E E_1)
                {
                    this.ONE_1 = ONE_1;
                    this.E_1 = E_1;
                }
                
                public Inners.ONE ONE_1 { get; }
                public Inners.E E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ONEF : percentxZEROONEⲻSEVENF
            {
                public ONEF(Inners.ONE ONE_1, Inners.F F_1)
                {
                    this.ONE_1 = ONE_1;
                    this.F_1 = F_1;
                }
                
                public Inners.ONE ONE_1 { get; }
                public Inners.F F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOZERO : percentxZEROONEⲻSEVENF
            {
                public TWOZERO(Inners.TWO TWO_1, Inners.ZERO ZERO_1)
                {
                    this.TWO_1 = TWO_1;
                    this.ZERO_1 = ZERO_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.ZERO ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOONE : percentxZEROONEⲻSEVENF
            {
                public TWOONE(Inners.TWO TWO_1, Inners.ONE ONE_1)
                {
                    this.TWO_1 = TWO_1;
                    this.ONE_1 = ONE_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.ONE ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOTWO : percentxZEROONEⲻSEVENF
            {
                public TWOTWO(Inners.TWO TWO_1, Inners.TWO TWO_2)
                {
                    this.TWO_1 = TWO_1;
                    this.TWO_2 = TWO_2;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.TWO TWO_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOTHREE : percentxZEROONEⲻSEVENF
            {
                public TWOTHREE(Inners.TWO TWO_1, Inners.THREE THREE_1)
                {
                    this.TWO_1 = TWO_1;
                    this.THREE_1 = THREE_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.THREE THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOFOUR : percentxZEROONEⲻSEVENF
            {
                public TWOFOUR(Inners.TWO TWO_1, Inners.FOUR FOUR_1)
                {
                    this.TWO_1 = TWO_1;
                    this.FOUR_1 = FOUR_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.FOUR FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOFIVE : percentxZEROONEⲻSEVENF
            {
                public TWOFIVE(Inners.TWO TWO_1, Inners.FIVE FIVE_1)
                {
                    this.TWO_1 = TWO_1;
                    this.FIVE_1 = FIVE_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.FIVE FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOSIX : percentxZEROONEⲻSEVENF
            {
                public TWOSIX(Inners.TWO TWO_1, Inners.SIX SIX_1)
                {
                    this.TWO_1 = TWO_1;
                    this.SIX_1 = SIX_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.SIX SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOSEVEN : percentxZEROONEⲻSEVENF
            {
                public TWOSEVEN(Inners.TWO TWO_1, Inners.SEVEN SEVEN_1)
                {
                    this.TWO_1 = TWO_1;
                    this.SEVEN_1 = SEVEN_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.SEVEN SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOEIGHT : percentxZEROONEⲻSEVENF
            {
                public TWOEIGHT(Inners.TWO TWO_1, Inners.EIGHT EIGHT_1)
                {
                    this.TWO_1 = TWO_1;
                    this.EIGHT_1 = EIGHT_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.EIGHT EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWONINE : percentxZEROONEⲻSEVENF
            {
                public TWONINE(Inners.TWO TWO_1, Inners.NINE NINE_1)
                {
                    this.TWO_1 = TWO_1;
                    this.NINE_1 = NINE_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.NINE NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOA : percentxZEROONEⲻSEVENF
            {
                public TWOA(Inners.TWO TWO_1, Inners.A A_1)
                {
                    this.TWO_1 = TWO_1;
                    this.A_1 = A_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.A A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOB : percentxZEROONEⲻSEVENF
            {
                public TWOB(Inners.TWO TWO_1, Inners.B B_1)
                {
                    this.TWO_1 = TWO_1;
                    this.B_1 = B_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.B B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOC : percentxZEROONEⲻSEVENF
            {
                public TWOC(Inners.TWO TWO_1, Inners.C C_1)
                {
                    this.TWO_1 = TWO_1;
                    this.C_1 = C_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.C C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOD : percentxZEROONEⲻSEVENF
            {
                public TWOD(Inners.TWO TWO_1, Inners.D D_1)
                {
                    this.TWO_1 = TWO_1;
                    this.D_1 = D_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.D D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOE : percentxZEROONEⲻSEVENF
            {
                public TWOE(Inners.TWO TWO_1, Inners.E E_1)
                {
                    this.TWO_1 = TWO_1;
                    this.E_1 = E_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.E E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOF : percentxZEROONEⲻSEVENF
            {
                public TWOF(Inners.TWO TWO_1, Inners.F F_1)
                {
                    this.TWO_1 = TWO_1;
                    this.F_1 = F_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.F F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREEZERO : percentxZEROONEⲻSEVENF
            {
                public THREEZERO(Inners.THREE THREE_1, Inners.ZERO ZERO_1)
                {
                    this.THREE_1 = THREE_1;
                    this.ZERO_1 = ZERO_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.ZERO ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREEONE : percentxZEROONEⲻSEVENF
            {
                public THREEONE(Inners.THREE THREE_1, Inners.ONE ONE_1)
                {
                    this.THREE_1 = THREE_1;
                    this.ONE_1 = ONE_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.ONE ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREETWO : percentxZEROONEⲻSEVENF
            {
                public THREETWO(Inners.THREE THREE_1, Inners.TWO TWO_1)
                {
                    this.THREE_1 = THREE_1;
                    this.TWO_1 = TWO_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.TWO TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREETHREE : percentxZEROONEⲻSEVENF
            {
                public THREETHREE(Inners.THREE THREE_1, Inners.THREE THREE_2)
                {
                    this.THREE_1 = THREE_1;
                    this.THREE_2 = THREE_2;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.THREE THREE_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREEFOUR : percentxZEROONEⲻSEVENF
            {
                public THREEFOUR(Inners.THREE THREE_1, Inners.FOUR FOUR_1)
                {
                    this.THREE_1 = THREE_1;
                    this.FOUR_1 = FOUR_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.FOUR FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREEFIVE : percentxZEROONEⲻSEVENF
            {
                public THREEFIVE(Inners.THREE THREE_1, Inners.FIVE FIVE_1)
                {
                    this.THREE_1 = THREE_1;
                    this.FIVE_1 = FIVE_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.FIVE FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREESIX : percentxZEROONEⲻSEVENF
            {
                public THREESIX(Inners.THREE THREE_1, Inners.SIX SIX_1)
                {
                    this.THREE_1 = THREE_1;
                    this.SIX_1 = SIX_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.SIX SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREESEVEN : percentxZEROONEⲻSEVENF
            {
                public THREESEVEN(Inners.THREE THREE_1, Inners.SEVEN SEVEN_1)
                {
                    this.THREE_1 = THREE_1;
                    this.SEVEN_1 = SEVEN_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.SEVEN SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREEEIGHT : percentxZEROONEⲻSEVENF
            {
                public THREEEIGHT(Inners.THREE THREE_1, Inners.EIGHT EIGHT_1)
                {
                    this.THREE_1 = THREE_1;
                    this.EIGHT_1 = EIGHT_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.EIGHT EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREENINE : percentxZEROONEⲻSEVENF
            {
                public THREENINE(Inners.THREE THREE_1, Inners.NINE NINE_1)
                {
                    this.THREE_1 = THREE_1;
                    this.NINE_1 = NINE_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.NINE NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREEA : percentxZEROONEⲻSEVENF
            {
                public THREEA(Inners.THREE THREE_1, Inners.A A_1)
                {
                    this.THREE_1 = THREE_1;
                    this.A_1 = A_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.A A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREEB : percentxZEROONEⲻSEVENF
            {
                public THREEB(Inners.THREE THREE_1, Inners.B B_1)
                {
                    this.THREE_1 = THREE_1;
                    this.B_1 = B_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.B B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREEC : percentxZEROONEⲻSEVENF
            {
                public THREEC(Inners.THREE THREE_1, Inners.C C_1)
                {
                    this.THREE_1 = THREE_1;
                    this.C_1 = C_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.C C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREED : percentxZEROONEⲻSEVENF
            {
                public THREED(Inners.THREE THREE_1, Inners.D D_1)
                {
                    this.THREE_1 = THREE_1;
                    this.D_1 = D_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.D D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREEE : percentxZEROONEⲻSEVENF
            {
                public THREEE(Inners.THREE THREE_1, Inners.E E_1)
                {
                    this.THREE_1 = THREE_1;
                    this.E_1 = E_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.E E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREEF : percentxZEROONEⲻSEVENF
            {
                public THREEF(Inners.THREE THREE_1, Inners.F F_1)
                {
                    this.THREE_1 = THREE_1;
                    this.F_1 = F_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.F F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURZERO : percentxZEROONEⲻSEVENF
            {
                public FOURZERO(Inners.FOUR FOUR_1, Inners.ZERO ZERO_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.ZERO_1 = ZERO_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.ZERO ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURONE : percentxZEROONEⲻSEVENF
            {
                public FOURONE(Inners.FOUR FOUR_1, Inners.ONE ONE_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.ONE_1 = ONE_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.ONE ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURTWO : percentxZEROONEⲻSEVENF
            {
                public FOURTWO(Inners.FOUR FOUR_1, Inners.TWO TWO_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.TWO_1 = TWO_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.TWO TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURTHREE : percentxZEROONEⲻSEVENF
            {
                public FOURTHREE(Inners.FOUR FOUR_1, Inners.THREE THREE_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.THREE_1 = THREE_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.THREE THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURFOUR : percentxZEROONEⲻSEVENF
            {
                public FOURFOUR(Inners.FOUR FOUR_1, Inners.FOUR FOUR_2)
                {
                    this.FOUR_1 = FOUR_1;
                    this.FOUR_2 = FOUR_2;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.FOUR FOUR_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURFIVE : percentxZEROONEⲻSEVENF
            {
                public FOURFIVE(Inners.FOUR FOUR_1, Inners.FIVE FIVE_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.FIVE_1 = FIVE_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.FIVE FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURSIX : percentxZEROONEⲻSEVENF
            {
                public FOURSIX(Inners.FOUR FOUR_1, Inners.SIX SIX_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.SIX_1 = SIX_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.SIX SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURSEVEN : percentxZEROONEⲻSEVENF
            {
                public FOURSEVEN(Inners.FOUR FOUR_1, Inners.SEVEN SEVEN_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.SEVEN_1 = SEVEN_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.SEVEN SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOUREIGHT : percentxZEROONEⲻSEVENF
            {
                public FOUREIGHT(Inners.FOUR FOUR_1, Inners.EIGHT EIGHT_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.EIGHT_1 = EIGHT_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.EIGHT EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURNINE : percentxZEROONEⲻSEVENF
            {
                public FOURNINE(Inners.FOUR FOUR_1, Inners.NINE NINE_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.NINE_1 = NINE_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.NINE NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURA : percentxZEROONEⲻSEVENF
            {
                public FOURA(Inners.FOUR FOUR_1, Inners.A A_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.A_1 = A_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.A A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURB : percentxZEROONEⲻSEVENF
            {
                public FOURB(Inners.FOUR FOUR_1, Inners.B B_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.B_1 = B_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.B B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURC : percentxZEROONEⲻSEVENF
            {
                public FOURC(Inners.FOUR FOUR_1, Inners.C C_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.C_1 = C_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.C C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURD : percentxZEROONEⲻSEVENF
            {
                public FOURD(Inners.FOUR FOUR_1, Inners.D D_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.D_1 = D_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.D D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURE : percentxZEROONEⲻSEVENF
            {
                public FOURE(Inners.FOUR FOUR_1, Inners.E E_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.E_1 = E_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.E E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURF : percentxZEROONEⲻSEVENF
            {
                public FOURF(Inners.FOUR FOUR_1, Inners.F F_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.F_1 = F_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.F F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVEZERO : percentxZEROONEⲻSEVENF
            {
                public FIVEZERO(Inners.FIVE FIVE_1, Inners.ZERO ZERO_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.ZERO_1 = ZERO_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.ZERO ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVEONE : percentxZEROONEⲻSEVENF
            {
                public FIVEONE(Inners.FIVE FIVE_1, Inners.ONE ONE_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.ONE_1 = ONE_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.ONE ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVETWO : percentxZEROONEⲻSEVENF
            {
                public FIVETWO(Inners.FIVE FIVE_1, Inners.TWO TWO_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.TWO_1 = TWO_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.TWO TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVETHREE : percentxZEROONEⲻSEVENF
            {
                public FIVETHREE(Inners.FIVE FIVE_1, Inners.THREE THREE_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.THREE_1 = THREE_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.THREE THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVEFOUR : percentxZEROONEⲻSEVENF
            {
                public FIVEFOUR(Inners.FIVE FIVE_1, Inners.FOUR FOUR_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.FOUR_1 = FOUR_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.FOUR FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVEFIVE : percentxZEROONEⲻSEVENF
            {
                public FIVEFIVE(Inners.FIVE FIVE_1, Inners.FIVE FIVE_2)
                {
                    this.FIVE_1 = FIVE_1;
                    this.FIVE_2 = FIVE_2;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.FIVE FIVE_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVESIX : percentxZEROONEⲻSEVENF
            {
                public FIVESIX(Inners.FIVE FIVE_1, Inners.SIX SIX_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.SIX_1 = SIX_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.SIX SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVESEVEN : percentxZEROONEⲻSEVENF
            {
                public FIVESEVEN(Inners.FIVE FIVE_1, Inners.SEVEN SEVEN_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.SEVEN_1 = SEVEN_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.SEVEN SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVEEIGHT : percentxZEROONEⲻSEVENF
            {
                public FIVEEIGHT(Inners.FIVE FIVE_1, Inners.EIGHT EIGHT_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.EIGHT_1 = EIGHT_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.EIGHT EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVENINE : percentxZEROONEⲻSEVENF
            {
                public FIVENINE(Inners.FIVE FIVE_1, Inners.NINE NINE_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.NINE_1 = NINE_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.NINE NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVEA : percentxZEROONEⲻSEVENF
            {
                public FIVEA(Inners.FIVE FIVE_1, Inners.A A_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.A_1 = A_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.A A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVEB : percentxZEROONEⲻSEVENF
            {
                public FIVEB(Inners.FIVE FIVE_1, Inners.B B_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.B_1 = B_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.B B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVEC : percentxZEROONEⲻSEVENF
            {
                public FIVEC(Inners.FIVE FIVE_1, Inners.C C_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.C_1 = C_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.C C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVED : percentxZEROONEⲻSEVENF
            {
                public FIVED(Inners.FIVE FIVE_1, Inners.D D_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.D_1 = D_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.D D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVEE : percentxZEROONEⲻSEVENF
            {
                public FIVEE(Inners.FIVE FIVE_1, Inners.E E_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.E_1 = E_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.E E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVEF : percentxZEROONEⲻSEVENF
            {
                public FIVEF(Inners.FIVE FIVE_1, Inners.F F_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.F_1 = F_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.F F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXZERO : percentxZEROONEⲻSEVENF
            {
                public SIXZERO(Inners.SIX SIX_1, Inners.ZERO ZERO_1)
                {
                    this.SIX_1 = SIX_1;
                    this.ZERO_1 = ZERO_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.ZERO ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXONE : percentxZEROONEⲻSEVENF
            {
                public SIXONE(Inners.SIX SIX_1, Inners.ONE ONE_1)
                {
                    this.SIX_1 = SIX_1;
                    this.ONE_1 = ONE_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.ONE ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXTWO : percentxZEROONEⲻSEVENF
            {
                public SIXTWO(Inners.SIX SIX_1, Inners.TWO TWO_1)
                {
                    this.SIX_1 = SIX_1;
                    this.TWO_1 = TWO_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.TWO TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXTHREE : percentxZEROONEⲻSEVENF
            {
                public SIXTHREE(Inners.SIX SIX_1, Inners.THREE THREE_1)
                {
                    this.SIX_1 = SIX_1;
                    this.THREE_1 = THREE_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.THREE THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXFOUR : percentxZEROONEⲻSEVENF
            {
                public SIXFOUR(Inners.SIX SIX_1, Inners.FOUR FOUR_1)
                {
                    this.SIX_1 = SIX_1;
                    this.FOUR_1 = FOUR_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.FOUR FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXFIVE : percentxZEROONEⲻSEVENF
            {
                public SIXFIVE(Inners.SIX SIX_1, Inners.FIVE FIVE_1)
                {
                    this.SIX_1 = SIX_1;
                    this.FIVE_1 = FIVE_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.FIVE FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXSIX : percentxZEROONEⲻSEVENF
            {
                public SIXSIX(Inners.SIX SIX_1, Inners.SIX SIX_2)
                {
                    this.SIX_1 = SIX_1;
                    this.SIX_2 = SIX_2;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.SIX SIX_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXSEVEN : percentxZEROONEⲻSEVENF
            {
                public SIXSEVEN(Inners.SIX SIX_1, Inners.SEVEN SEVEN_1)
                {
                    this.SIX_1 = SIX_1;
                    this.SEVEN_1 = SEVEN_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.SEVEN SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXEIGHT : percentxZEROONEⲻSEVENF
            {
                public SIXEIGHT(Inners.SIX SIX_1, Inners.EIGHT EIGHT_1)
                {
                    this.SIX_1 = SIX_1;
                    this.EIGHT_1 = EIGHT_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.EIGHT EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXNINE : percentxZEROONEⲻSEVENF
            {
                public SIXNINE(Inners.SIX SIX_1, Inners.NINE NINE_1)
                {
                    this.SIX_1 = SIX_1;
                    this.NINE_1 = NINE_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.NINE NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXA : percentxZEROONEⲻSEVENF
            {
                public SIXA(Inners.SIX SIX_1, Inners.A A_1)
                {
                    this.SIX_1 = SIX_1;
                    this.A_1 = A_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.A A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXB : percentxZEROONEⲻSEVENF
            {
                public SIXB(Inners.SIX SIX_1, Inners.B B_1)
                {
                    this.SIX_1 = SIX_1;
                    this.B_1 = B_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.B B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXC : percentxZEROONEⲻSEVENF
            {
                public SIXC(Inners.SIX SIX_1, Inners.C C_1)
                {
                    this.SIX_1 = SIX_1;
                    this.C_1 = C_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.C C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXD : percentxZEROONEⲻSEVENF
            {
                public SIXD(Inners.SIX SIX_1, Inners.D D_1)
                {
                    this.SIX_1 = SIX_1;
                    this.D_1 = D_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.D D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXE : percentxZEROONEⲻSEVENF
            {
                public SIXE(Inners.SIX SIX_1, Inners.E E_1)
                {
                    this.SIX_1 = SIX_1;
                    this.E_1 = E_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.E E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXF : percentxZEROONEⲻSEVENF
            {
                public SIXF(Inners.SIX SIX_1, Inners.F F_1)
                {
                    this.SIX_1 = SIX_1;
                    this.F_1 = F_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.F F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENZERO : percentxZEROONEⲻSEVENF
            {
                public SEVENZERO(Inners.SEVEN SEVEN_1, Inners.ZERO ZERO_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.ZERO_1 = ZERO_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.ZERO ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENONE : percentxZEROONEⲻSEVENF
            {
                public SEVENONE(Inners.SEVEN SEVEN_1, Inners.ONE ONE_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.ONE_1 = ONE_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.ONE ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENTWO : percentxZEROONEⲻSEVENF
            {
                public SEVENTWO(Inners.SEVEN SEVEN_1, Inners.TWO TWO_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.TWO_1 = TWO_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.TWO TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENTHREE : percentxZEROONEⲻSEVENF
            {
                public SEVENTHREE(Inners.SEVEN SEVEN_1, Inners.THREE THREE_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.THREE_1 = THREE_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.THREE THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENFOUR : percentxZEROONEⲻSEVENF
            {
                public SEVENFOUR(Inners.SEVEN SEVEN_1, Inners.FOUR FOUR_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.FOUR_1 = FOUR_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.FOUR FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENFIVE : percentxZEROONEⲻSEVENF
            {
                public SEVENFIVE(Inners.SEVEN SEVEN_1, Inners.FIVE FIVE_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.FIVE_1 = FIVE_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.FIVE FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENSIX : percentxZEROONEⲻSEVENF
            {
                public SEVENSIX(Inners.SEVEN SEVEN_1, Inners.SIX SIX_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.SIX_1 = SIX_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.SIX SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENSEVEN : percentxZEROONEⲻSEVENF
            {
                public SEVENSEVEN(Inners.SEVEN SEVEN_1, Inners.SEVEN SEVEN_2)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.SEVEN_2 = SEVEN_2;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.SEVEN SEVEN_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENEIGHT : percentxZEROONEⲻSEVENF
            {
                public SEVENEIGHT(Inners.SEVEN SEVEN_1, Inners.EIGHT EIGHT_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.EIGHT_1 = EIGHT_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.EIGHT EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENNINE : percentxZEROONEⲻSEVENF
            {
                public SEVENNINE(Inners.SEVEN SEVEN_1, Inners.NINE NINE_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.NINE_1 = NINE_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.NINE NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENA : percentxZEROONEⲻSEVENF
            {
                public SEVENA(Inners.SEVEN SEVEN_1, Inners.A A_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.A_1 = A_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.A A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENB : percentxZEROONEⲻSEVENF
            {
                public SEVENB(Inners.SEVEN SEVEN_1, Inners.B B_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.B_1 = B_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.B B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENC : percentxZEROONEⲻSEVENF
            {
                public SEVENC(Inners.SEVEN SEVEN_1, Inners.C C_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.C_1 = C_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.C C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVEND : percentxZEROONEⲻSEVENF
            {
                public SEVEND(Inners.SEVEN SEVEN_1, Inners.D D_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.D_1 = D_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.D D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENE : percentxZEROONEⲻSEVENF
            {
                public SEVENE(Inners.SEVEN SEVEN_1, Inners.E E_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.E_1 = E_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.E E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENF : percentxZEROONEⲻSEVENF
            {
                public SEVENF(Inners.SEVEN SEVEN_1, Inners.F F_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.F_1 = F_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.F F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
        }
        
        public sealed class percentxZEROD
        {
            public percentxZEROD(Inners.ZERO ZERO_1, Inners.D D_1)
            {
                this.ZERO_1 = ZERO_1;
                this.D_1 = D_1;
            }
            
            public Inners.ZERO ZERO_1 { get; }
            public Inners.D D_1 { get; }
        }
        
        public abstract class percentxZEROZEROⲻONEF
        {
            private percentxZEROZEROⲻONEF()
            {
            }
            
            protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
            
            public abstract class Visitor<TResult, TContext>
            {
                public TResult Visit(percentxZEROZEROⲻONEF node, TContext context)
                {
                    return node.Dispatch(this, context);
                }
                
                protected internal abstract TResult Accept(percentxZEROZEROⲻONEF.ZEROZERO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻONEF.ZEROONE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻONEF.ZEROTWO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻONEF.ZEROTHREE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻONEF.ZEROFOUR node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻONEF.ZEROFIVE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻONEF.ZEROSIX node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻONEF.ZEROSEVEN node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻONEF.ZEROEIGHT node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻONEF.ZERONINE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻONEF.ZEROA node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻONEF.ZEROB node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻONEF.ZEROC node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻONEF.ZEROD node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻONEF.ZEROE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻONEF.ZEROF node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻONEF.ONEZERO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻONEF.ONEONE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻONEF.ONETWO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻONEF.ONETHREE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻONEF.ONEFOUR node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻONEF.ONEFIVE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻONEF.ONESIX node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻONEF.ONESEVEN node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻONEF.ONEEIGHT node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻONEF.ONENINE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻONEF.ONEA node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻONEF.ONEB node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻONEF.ONEC node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻONEF.ONED node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻONEF.ONEE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻONEF.ONEF node, TContext context);
            }
            
            public sealed class ZEROZERO : percentxZEROZEROⲻONEF
            {
                public ZEROZERO(Inners.ZERO ZERO_1, Inners.ZERO ZERO_2)
                {
                    this.ZERO_1 = ZERO_1;
                    this.ZERO_2 = ZERO_2;
                }
                
                public Inners.ZERO ZERO_1 { get; }
                public Inners.ZERO ZERO_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ZEROONE : percentxZEROZEROⲻONEF
            {
                public ZEROONE(Inners.ZERO ZERO_1, Inners.ONE ONE_1)
                {
                    this.ZERO_1 = ZERO_1;
                    this.ONE_1 = ONE_1;
                }
                
                public Inners.ZERO ZERO_1 { get; }
                public Inners.ONE ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ZEROTWO : percentxZEROZEROⲻONEF
            {
                public ZEROTWO(Inners.ZERO ZERO_1, Inners.TWO TWO_1)
                {
                    this.ZERO_1 = ZERO_1;
                    this.TWO_1 = TWO_1;
                }
                
                public Inners.ZERO ZERO_1 { get; }
                public Inners.TWO TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ZEROTHREE : percentxZEROZEROⲻONEF
            {
                public ZEROTHREE(Inners.ZERO ZERO_1, Inners.THREE THREE_1)
                {
                    this.ZERO_1 = ZERO_1;
                    this.THREE_1 = THREE_1;
                }
                
                public Inners.ZERO ZERO_1 { get; }
                public Inners.THREE THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ZEROFOUR : percentxZEROZEROⲻONEF
            {
                public ZEROFOUR(Inners.ZERO ZERO_1, Inners.FOUR FOUR_1)
                {
                    this.ZERO_1 = ZERO_1;
                    this.FOUR_1 = FOUR_1;
                }
                
                public Inners.ZERO ZERO_1 { get; }
                public Inners.FOUR FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ZEROFIVE : percentxZEROZEROⲻONEF
            {
                public ZEROFIVE(Inners.ZERO ZERO_1, Inners.FIVE FIVE_1)
                {
                    this.ZERO_1 = ZERO_1;
                    this.FIVE_1 = FIVE_1;
                }
                
                public Inners.ZERO ZERO_1 { get; }
                public Inners.FIVE FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ZEROSIX : percentxZEROZEROⲻONEF
            {
                public ZEROSIX(Inners.ZERO ZERO_1, Inners.SIX SIX_1)
                {
                    this.ZERO_1 = ZERO_1;
                    this.SIX_1 = SIX_1;
                }
                
                public Inners.ZERO ZERO_1 { get; }
                public Inners.SIX SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ZEROSEVEN : percentxZEROZEROⲻONEF
            {
                public ZEROSEVEN(Inners.ZERO ZERO_1, Inners.SEVEN SEVEN_1)
                {
                    this.ZERO_1 = ZERO_1;
                    this.SEVEN_1 = SEVEN_1;
                }
                
                public Inners.ZERO ZERO_1 { get; }
                public Inners.SEVEN SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ZEROEIGHT : percentxZEROZEROⲻONEF
            {
                public ZEROEIGHT(Inners.ZERO ZERO_1, Inners.EIGHT EIGHT_1)
                {
                    this.ZERO_1 = ZERO_1;
                    this.EIGHT_1 = EIGHT_1;
                }
                
                public Inners.ZERO ZERO_1 { get; }
                public Inners.EIGHT EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ZERONINE : percentxZEROZEROⲻONEF
            {
                public ZERONINE(Inners.ZERO ZERO_1, Inners.NINE NINE_1)
                {
                    this.ZERO_1 = ZERO_1;
                    this.NINE_1 = NINE_1;
                }
                
                public Inners.ZERO ZERO_1 { get; }
                public Inners.NINE NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ZEROA : percentxZEROZEROⲻONEF
            {
                public ZEROA(Inners.ZERO ZERO_1, Inners.A A_1)
                {
                    this.ZERO_1 = ZERO_1;
                    this.A_1 = A_1;
                }
                
                public Inners.ZERO ZERO_1 { get; }
                public Inners.A A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ZEROB : percentxZEROZEROⲻONEF
            {
                public ZEROB(Inners.ZERO ZERO_1, Inners.B B_1)
                {
                    this.ZERO_1 = ZERO_1;
                    this.B_1 = B_1;
                }
                
                public Inners.ZERO ZERO_1 { get; }
                public Inners.B B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ZEROC : percentxZEROZEROⲻONEF
            {
                public ZEROC(Inners.ZERO ZERO_1, Inners.C C_1)
                {
                    this.ZERO_1 = ZERO_1;
                    this.C_1 = C_1;
                }
                
                public Inners.ZERO ZERO_1 { get; }
                public Inners.C C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ZEROD : percentxZEROZEROⲻONEF
            {
                public ZEROD(Inners.ZERO ZERO_1, Inners.D D_1)
                {
                    this.ZERO_1 = ZERO_1;
                    this.D_1 = D_1;
                }
                
                public Inners.ZERO ZERO_1 { get; }
                public Inners.D D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ZEROE : percentxZEROZEROⲻONEF
            {
                public ZEROE(Inners.ZERO ZERO_1, Inners.E E_1)
                {
                    this.ZERO_1 = ZERO_1;
                    this.E_1 = E_1;
                }
                
                public Inners.ZERO ZERO_1 { get; }
                public Inners.E E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ZEROF : percentxZEROZEROⲻONEF
            {
                public ZEROF(Inners.ZERO ZERO_1, Inners.F F_1)
                {
                    this.ZERO_1 = ZERO_1;
                    this.F_1 = F_1;
                }
                
                public Inners.ZERO ZERO_1 { get; }
                public Inners.F F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ONEZERO : percentxZEROZEROⲻONEF
            {
                public ONEZERO(Inners.ONE ONE_1, Inners.ZERO ZERO_1)
                {
                    this.ONE_1 = ONE_1;
                    this.ZERO_1 = ZERO_1;
                }
                
                public Inners.ONE ONE_1 { get; }
                public Inners.ZERO ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ONEONE : percentxZEROZEROⲻONEF
            {
                public ONEONE(Inners.ONE ONE_1, Inners.ONE ONE_2)
                {
                    this.ONE_1 = ONE_1;
                    this.ONE_2 = ONE_2;
                }
                
                public Inners.ONE ONE_1 { get; }
                public Inners.ONE ONE_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ONETWO : percentxZEROZEROⲻONEF
            {
                public ONETWO(Inners.ONE ONE_1, Inners.TWO TWO_1)
                {
                    this.ONE_1 = ONE_1;
                    this.TWO_1 = TWO_1;
                }
                
                public Inners.ONE ONE_1 { get; }
                public Inners.TWO TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ONETHREE : percentxZEROZEROⲻONEF
            {
                public ONETHREE(Inners.ONE ONE_1, Inners.THREE THREE_1)
                {
                    this.ONE_1 = ONE_1;
                    this.THREE_1 = THREE_1;
                }
                
                public Inners.ONE ONE_1 { get; }
                public Inners.THREE THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ONEFOUR : percentxZEROZEROⲻONEF
            {
                public ONEFOUR(Inners.ONE ONE_1, Inners.FOUR FOUR_1)
                {
                    this.ONE_1 = ONE_1;
                    this.FOUR_1 = FOUR_1;
                }
                
                public Inners.ONE ONE_1 { get; }
                public Inners.FOUR FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ONEFIVE : percentxZEROZEROⲻONEF
            {
                public ONEFIVE(Inners.ONE ONE_1, Inners.FIVE FIVE_1)
                {
                    this.ONE_1 = ONE_1;
                    this.FIVE_1 = FIVE_1;
                }
                
                public Inners.ONE ONE_1 { get; }
                public Inners.FIVE FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ONESIX : percentxZEROZEROⲻONEF
            {
                public ONESIX(Inners.ONE ONE_1, Inners.SIX SIX_1)
                {
                    this.ONE_1 = ONE_1;
                    this.SIX_1 = SIX_1;
                }
                
                public Inners.ONE ONE_1 { get; }
                public Inners.SIX SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ONESEVEN : percentxZEROZEROⲻONEF
            {
                public ONESEVEN(Inners.ONE ONE_1, Inners.SEVEN SEVEN_1)
                {
                    this.ONE_1 = ONE_1;
                    this.SEVEN_1 = SEVEN_1;
                }
                
                public Inners.ONE ONE_1 { get; }
                public Inners.SEVEN SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ONEEIGHT : percentxZEROZEROⲻONEF
            {
                public ONEEIGHT(Inners.ONE ONE_1, Inners.EIGHT EIGHT_1)
                {
                    this.ONE_1 = ONE_1;
                    this.EIGHT_1 = EIGHT_1;
                }
                
                public Inners.ONE ONE_1 { get; }
                public Inners.EIGHT EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ONENINE : percentxZEROZEROⲻONEF
            {
                public ONENINE(Inners.ONE ONE_1, Inners.NINE NINE_1)
                {
                    this.ONE_1 = ONE_1;
                    this.NINE_1 = NINE_1;
                }
                
                public Inners.ONE ONE_1 { get; }
                public Inners.NINE NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ONEA : percentxZEROZEROⲻONEF
            {
                public ONEA(Inners.ONE ONE_1, Inners.A A_1)
                {
                    this.ONE_1 = ONE_1;
                    this.A_1 = A_1;
                }
                
                public Inners.ONE ONE_1 { get; }
                public Inners.A A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ONEB : percentxZEROZEROⲻONEF
            {
                public ONEB(Inners.ONE ONE_1, Inners.B B_1)
                {
                    this.ONE_1 = ONE_1;
                    this.B_1 = B_1;
                }
                
                public Inners.ONE ONE_1 { get; }
                public Inners.B B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ONEC : percentxZEROZEROⲻONEF
            {
                public ONEC(Inners.ONE ONE_1, Inners.C C_1)
                {
                    this.ONE_1 = ONE_1;
                    this.C_1 = C_1;
                }
                
                public Inners.ONE ONE_1 { get; }
                public Inners.C C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ONED : percentxZEROZEROⲻONEF
            {
                public ONED(Inners.ONE ONE_1, Inners.D D_1)
                {
                    this.ONE_1 = ONE_1;
                    this.D_1 = D_1;
                }
                
                public Inners.ONE ONE_1 { get; }
                public Inners.D D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ONEE : percentxZEROZEROⲻONEF
            {
                public ONEE(Inners.ONE ONE_1, Inners.E E_1)
                {
                    this.ONE_1 = ONE_1;
                    this.E_1 = E_1;
                }
                
                public Inners.ONE ONE_1 { get; }
                public Inners.E E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ONEF : percentxZEROZEROⲻONEF
            {
                public ONEF(Inners.ONE ONE_1, Inners.F F_1)
                {
                    this.ONE_1 = ONE_1;
                    this.F_1 = F_1;
                }
                
                public Inners.ONE ONE_1 { get; }
                public Inners.F F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
        }
        
        public sealed class percentxSEVENF
        {
            public percentxSEVENF(Inners.SEVEN SEVEN_1, Inners.F F_1)
            {
                this.SEVEN_1 = SEVEN_1;
                this.F_1 = F_1;
            }
            
            public Inners.SEVEN SEVEN_1 { get; }
            public Inners.F F_1 { get; }
        }
        
        public abstract class percentxTHREEZEROⲻTHREENINE
        {
            private percentxTHREEZEROⲻTHREENINE()
            {
            }
            
            protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
            
            public abstract class Visitor<TResult, TContext>
            {
                public TResult Visit(percentxTHREEZEROⲻTHREENINE node, TContext context)
                {
                    return node.Dispatch(this, context);
                }
                
                protected internal abstract TResult Accept(percentxTHREEZEROⲻTHREENINE.THREEZERO node, TContext context);
                protected internal abstract TResult Accept(percentxTHREEZEROⲻTHREENINE.THREEONE node, TContext context);
                protected internal abstract TResult Accept(percentxTHREEZEROⲻTHREENINE.THREETWO node, TContext context);
                protected internal abstract TResult Accept(percentxTHREEZEROⲻTHREENINE.THREETHREE node, TContext context);
                protected internal abstract TResult Accept(percentxTHREEZEROⲻTHREENINE.THREEFOUR node, TContext context);
                protected internal abstract TResult Accept(percentxTHREEZEROⲻTHREENINE.THREEFIVE node, TContext context);
                protected internal abstract TResult Accept(percentxTHREEZEROⲻTHREENINE.THREESIX node, TContext context);
                protected internal abstract TResult Accept(percentxTHREEZEROⲻTHREENINE.THREESEVEN node, TContext context);
                protected internal abstract TResult Accept(percentxTHREEZEROⲻTHREENINE.THREEEIGHT node, TContext context);
                protected internal abstract TResult Accept(percentxTHREEZEROⲻTHREENINE.THREENINE node, TContext context);
            }
            
            public sealed class THREEZERO : percentxTHREEZEROⲻTHREENINE
            {
                public THREEZERO(Inners.THREE THREE_1, Inners.ZERO ZERO_1)
                {
                    this.THREE_1 = THREE_1;
                    this.ZERO_1 = ZERO_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.ZERO ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREEONE : percentxTHREEZEROⲻTHREENINE
            {
                public THREEONE(Inners.THREE THREE_1, Inners.ONE ONE_1)
                {
                    this.THREE_1 = THREE_1;
                    this.ONE_1 = ONE_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.ONE ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREETWO : percentxTHREEZEROⲻTHREENINE
            {
                public THREETWO(Inners.THREE THREE_1, Inners.TWO TWO_1)
                {
                    this.THREE_1 = THREE_1;
                    this.TWO_1 = TWO_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.TWO TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREETHREE : percentxTHREEZEROⲻTHREENINE
            {
                public THREETHREE(Inners.THREE THREE_1, Inners.THREE THREE_2)
                {
                    this.THREE_1 = THREE_1;
                    this.THREE_2 = THREE_2;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.THREE THREE_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREEFOUR : percentxTHREEZEROⲻTHREENINE
            {
                public THREEFOUR(Inners.THREE THREE_1, Inners.FOUR FOUR_1)
                {
                    this.THREE_1 = THREE_1;
                    this.FOUR_1 = FOUR_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.FOUR FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREEFIVE : percentxTHREEZEROⲻTHREENINE
            {
                public THREEFIVE(Inners.THREE THREE_1, Inners.FIVE FIVE_1)
                {
                    this.THREE_1 = THREE_1;
                    this.FIVE_1 = FIVE_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.FIVE FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREESIX : percentxTHREEZEROⲻTHREENINE
            {
                public THREESIX(Inners.THREE THREE_1, Inners.SIX SIX_1)
                {
                    this.THREE_1 = THREE_1;
                    this.SIX_1 = SIX_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.SIX SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREESEVEN : percentxTHREEZEROⲻTHREENINE
            {
                public THREESEVEN(Inners.THREE THREE_1, Inners.SEVEN SEVEN_1)
                {
                    this.THREE_1 = THREE_1;
                    this.SEVEN_1 = SEVEN_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.SEVEN SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREEEIGHT : percentxTHREEZEROⲻTHREENINE
            {
                public THREEEIGHT(Inners.THREE THREE_1, Inners.EIGHT EIGHT_1)
                {
                    this.THREE_1 = THREE_1;
                    this.EIGHT_1 = EIGHT_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.EIGHT EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREENINE : percentxTHREEZEROⲻTHREENINE
            {
                public THREENINE(Inners.THREE THREE_1, Inners.NINE NINE_1)
                {
                    this.THREE_1 = THREE_1;
                    this.NINE_1 = NINE_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.NINE NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
        }
        
        public sealed class percentxTWOTWO
        {
            public percentxTWOTWO(Inners.TWO TWO_1, Inners.TWO TWO_2)
            {
                this.TWO_1 = TWO_1;
                this.TWO_2 = TWO_2;
            }
            
            public Inners.TWO TWO_1 { get; }
            public Inners.TWO TWO_2 { get; }
        }
        
        public sealed class x41
        {
            private x41()
            {
            }
            
            public static x41 Instance { get; } = new x41();
        }
        
        public sealed class doublequotex41doublequote
        {
            public doublequotex41doublequote(Inners.x41 x41_1)
            {
                this.x41_1 = x41_1;
            }
            
            public Inners.x41 x41_1 { get; }
        }
        
        public sealed class x42
        {
            private x42()
            {
            }
            
            public static x42 Instance { get; } = new x42();
        }
        
        public sealed class doublequotex42doublequote
        {
            public doublequotex42doublequote(Inners.x42 x42_1)
            {
                this.x42_1 = x42_1;
            }
            
            public Inners.x42 x42_1 { get; }
        }
        
        public sealed class x43
        {
            private x43()
            {
            }
            
            public static x43 Instance { get; } = new x43();
        }
        
        public sealed class doublequotex43doublequote
        {
            public doublequotex43doublequote(Inners.x43 x43_1)
            {
                this.x43_1 = x43_1;
            }
            
            public Inners.x43 x43_1 { get; }
        }
        
        public sealed class x44
        {
            private x44()
            {
            }
            
            public static x44 Instance { get; } = new x44();
        }
        
        public sealed class doublequotex44doublequote
        {
            public doublequotex44doublequote(Inners.x44 x44_1)
            {
                this.x44_1 = x44_1;
            }
            
            public Inners.x44 x44_1 { get; }
        }
        
        public sealed class x45
        {
            private x45()
            {
            }
            
            public static x45 Instance { get; } = new x45();
        }
        
        public sealed class doublequotex45doublequote
        {
            public doublequotex45doublequote(Inners.x45 x45_1)
            {
                this.x45_1 = x45_1;
            }
            
            public Inners.x45 x45_1 { get; }
        }
        
        public sealed class x46
        {
            private x46()
            {
            }
            
            public static x46 Instance { get; } = new x46();
        }
        
        public sealed class doublequotex46doublequote
        {
            public doublequotex46doublequote(Inners.x46 x46_1)
            {
                this.x46_1 = x46_1;
            }
            
            public Inners.x46 x46_1 { get; }
        }
        
        public sealed class percentxZERONINE
        {
            public percentxZERONINE(Inners.ZERO ZERO_1, Inners.NINE NINE_1)
            {
                this.ZERO_1 = ZERO_1;
                this.NINE_1 = NINE_1;
            }
            
            public Inners.ZERO ZERO_1 { get; }
            public Inners.NINE NINE_1 { get; }
        }
        
        public sealed class percentxZEROA
        {
            public percentxZEROA(Inners.ZERO ZERO_1, Inners.A A_1)
            {
                this.ZERO_1 = ZERO_1;
                this.A_1 = A_1;
            }
            
            public Inners.ZERO ZERO_1 { get; }
            public Inners.A A_1 { get; }
        }
        
        public abstract class WSPⳆCRLF_WSP
        {
            private WSPⳆCRLF_WSP()
            {
            }
            
            protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
            
            public abstract class Visitor<TResult, TContext>
            {
                public TResult Visit(WSPⳆCRLF_WSP node, TContext context)
                {
                    return node.Dispatch(this, context);
                }
                
                protected internal abstract TResult Accept(WSPⳆCRLF_WSP.WSP node, TContext context);
                protected internal abstract TResult Accept(WSPⳆCRLF_WSP.CRLF_WSP node, TContext context);
            }
            
            public sealed class WSP : WSPⳆCRLF_WSP
            {
                public WSP(GeneratorV3.Core.WSP WSP_1)
                {
                    this.WSP_1 = WSP_1;
                }
                
                public GeneratorV3.Core.WSP WSP_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class CRLF_WSP : WSPⳆCRLF_WSP
            {
                public CRLF_WSP(GeneratorV3.Core.CRLF CRLF_1, GeneratorV3.Core.WSP WSP_1)
                {
                    this.CRLF_1 = CRLF_1;
                    this.WSP_1 = WSP_1;
                }
                
                public GeneratorV3.Core.CRLF CRLF_1 { get; }
                public GeneratorV3.Core.WSP WSP_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
        }
        
        public sealed class openWSPⳆCRLF_WSPↃ
        {
            public openWSPⳆCRLF_WSPↃ(Inners.WSPⳆCRLF_WSP WSPⳆCRLF_WSP_1)
            {
                this.WSPⳆCRLF_WSP_1 = WSPⳆCRLF_WSP_1;
            }
            
            public Inners.WSPⳆCRLF_WSP WSPⳆCRLF_WSP_1 { get; }
        }
        
        public abstract class percentxZEROZEROⲻFF
        {
            private percentxZEROZEROⲻFF()
            {
            }
            
            protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
            
            public abstract class Visitor<TResult, TContext>
            {
                public TResult Visit(percentxZEROZEROⲻFF node, TContext context)
                {
                    return node.Dispatch(this, context);
                }
                
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.ZEROZERO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.ZEROONE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.ZEROTWO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.ZEROTHREE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.ZEROFOUR node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.ZEROFIVE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.ZEROSIX node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.ZEROSEVEN node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.ZEROEIGHT node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.ZERONINE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.ZEROA node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.ZEROB node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.ZEROC node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.ZEROD node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.ZEROE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.ZEROF node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.ONEZERO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.ONEONE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.ONETWO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.ONETHREE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.ONEFOUR node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.ONEFIVE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.ONESIX node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.ONESEVEN node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.ONEEIGHT node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.ONENINE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.ONEA node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.ONEB node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.ONEC node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.ONED node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.ONEE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.ONEF node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.TWOZERO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.TWOONE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.TWOTWO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.TWOTHREE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.TWOFOUR node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.TWOFIVE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.TWOSIX node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.TWOSEVEN node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.TWOEIGHT node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.TWONINE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.TWOA node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.TWOB node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.TWOC node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.TWOD node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.TWOE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.TWOF node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.THREEZERO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.THREEONE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.THREETWO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.THREETHREE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.THREEFOUR node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.THREEFIVE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.THREESIX node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.THREESEVEN node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.THREEEIGHT node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.THREENINE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.THREEA node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.THREEB node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.THREEC node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.THREED node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.THREEE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.THREEF node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.FOURZERO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.FOURONE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.FOURTWO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.FOURTHREE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.FOURFOUR node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.FOURFIVE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.FOURSIX node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.FOURSEVEN node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.FOUREIGHT node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.FOURNINE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.FOURA node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.FOURB node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.FOURC node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.FOURD node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.FOURE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.FOURF node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.FIVEZERO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.FIVEONE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.FIVETWO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.FIVETHREE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.FIVEFOUR node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.FIVEFIVE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.FIVESIX node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.FIVESEVEN node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.FIVEEIGHT node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.FIVENINE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.FIVEA node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.FIVEB node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.FIVEC node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.FIVED node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.FIVEE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.FIVEF node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.SIXZERO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.SIXONE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.SIXTWO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.SIXTHREE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.SIXFOUR node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.SIXFIVE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.SIXSIX node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.SIXSEVEN node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.SIXEIGHT node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.SIXNINE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.SIXA node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.SIXB node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.SIXC node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.SIXD node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.SIXE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.SIXF node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.SEVENZERO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.SEVENONE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.SEVENTWO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.SEVENTHREE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.SEVENFOUR node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.SEVENFIVE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.SEVENSIX node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.SEVENSEVEN node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.SEVENEIGHT node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.SEVENNINE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.SEVENA node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.SEVENB node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.SEVENC node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.SEVEND node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.SEVENE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.SEVENF node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.EIGHTZERO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.EIGHTONE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.EIGHTTWO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.EIGHTTHREE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.EIGHTFOUR node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.EIGHTFIVE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.EIGHTSIX node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.EIGHTSEVEN node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.EIGHTEIGHT node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.EIGHTNINE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.EIGHTA node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.EIGHTB node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.EIGHTC node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.EIGHTD node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.EIGHTE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.EIGHTF node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.NINEZERO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.NINEONE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.NINETWO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.NINETHREE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.NINEFOUR node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.NINEFIVE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.NINESIX node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.NINESEVEN node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.NINEEIGHT node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.NINENINE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.NINEA node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.NINEB node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.NINEC node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.NINED node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.NINEE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.NINEF node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.AZERO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.AONE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.ATWO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.ATHREE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.AFOUR node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.AFIVE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.ASIX node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.ASEVEN node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.AEIGHT node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.ANINE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.AA node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.AB node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.AC node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.AD node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.AE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.AF node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.BZERO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.BONE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.BTWO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.BTHREE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.BFOUR node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.BFIVE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.BSIX node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.BSEVEN node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.BEIGHT node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.BNINE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.BA node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.BB node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.BC node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.BD node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.BE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.BF node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.CZERO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.CONE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.CTWO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.CTHREE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.CFOUR node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.CFIVE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.CSIX node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.CSEVEN node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.CEIGHT node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.CNINE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.CA node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.CB node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.CC node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.CD node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.CE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.CF node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.DZERO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.DONE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.DTWO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.DTHREE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.DFOUR node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.DFIVE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.DSIX node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.DSEVEN node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.DEIGHT node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.DNINE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.DA node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.DB node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.DC node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.DD node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.DE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.DF node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.EZERO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.EONE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.ETWO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.ETHREE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.EFOUR node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.EFIVE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.ESIX node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.ESEVEN node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.EEIGHT node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.ENINE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.EA node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.EB node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.EC node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.ED node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.EE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.EF node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.FZERO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.FONE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.FTWO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.FTHREE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.FFOUR node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.FFIVE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.FSIX node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.FSEVEN node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.FEIGHT node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.FNINE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.FA node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.FB node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.FC node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.FD node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.FE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.FF node, TContext context);
            }
            
            public sealed class ZEROZERO : percentxZEROZEROⲻFF
            {
                public ZEROZERO(Inners.ZERO ZERO_1, Inners.ZERO ZERO_2)
                {
                    this.ZERO_1 = ZERO_1;
                    this.ZERO_2 = ZERO_2;
                }
                
                public Inners.ZERO ZERO_1 { get; }
                public Inners.ZERO ZERO_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ZEROONE : percentxZEROZEROⲻFF
            {
                public ZEROONE(Inners.ZERO ZERO_1, Inners.ONE ONE_1)
                {
                    this.ZERO_1 = ZERO_1;
                    this.ONE_1 = ONE_1;
                }
                
                public Inners.ZERO ZERO_1 { get; }
                public Inners.ONE ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ZEROTWO : percentxZEROZEROⲻFF
            {
                public ZEROTWO(Inners.ZERO ZERO_1, Inners.TWO TWO_1)
                {
                    this.ZERO_1 = ZERO_1;
                    this.TWO_1 = TWO_1;
                }
                
                public Inners.ZERO ZERO_1 { get; }
                public Inners.TWO TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ZEROTHREE : percentxZEROZEROⲻFF
            {
                public ZEROTHREE(Inners.ZERO ZERO_1, Inners.THREE THREE_1)
                {
                    this.ZERO_1 = ZERO_1;
                    this.THREE_1 = THREE_1;
                }
                
                public Inners.ZERO ZERO_1 { get; }
                public Inners.THREE THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ZEROFOUR : percentxZEROZEROⲻFF
            {
                public ZEROFOUR(Inners.ZERO ZERO_1, Inners.FOUR FOUR_1)
                {
                    this.ZERO_1 = ZERO_1;
                    this.FOUR_1 = FOUR_1;
                }
                
                public Inners.ZERO ZERO_1 { get; }
                public Inners.FOUR FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ZEROFIVE : percentxZEROZEROⲻFF
            {
                public ZEROFIVE(Inners.ZERO ZERO_1, Inners.FIVE FIVE_1)
                {
                    this.ZERO_1 = ZERO_1;
                    this.FIVE_1 = FIVE_1;
                }
                
                public Inners.ZERO ZERO_1 { get; }
                public Inners.FIVE FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ZEROSIX : percentxZEROZEROⲻFF
            {
                public ZEROSIX(Inners.ZERO ZERO_1, Inners.SIX SIX_1)
                {
                    this.ZERO_1 = ZERO_1;
                    this.SIX_1 = SIX_1;
                }
                
                public Inners.ZERO ZERO_1 { get; }
                public Inners.SIX SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ZEROSEVEN : percentxZEROZEROⲻFF
            {
                public ZEROSEVEN(Inners.ZERO ZERO_1, Inners.SEVEN SEVEN_1)
                {
                    this.ZERO_1 = ZERO_1;
                    this.SEVEN_1 = SEVEN_1;
                }
                
                public Inners.ZERO ZERO_1 { get; }
                public Inners.SEVEN SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ZEROEIGHT : percentxZEROZEROⲻFF
            {
                public ZEROEIGHT(Inners.ZERO ZERO_1, Inners.EIGHT EIGHT_1)
                {
                    this.ZERO_1 = ZERO_1;
                    this.EIGHT_1 = EIGHT_1;
                }
                
                public Inners.ZERO ZERO_1 { get; }
                public Inners.EIGHT EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ZERONINE : percentxZEROZEROⲻFF
            {
                public ZERONINE(Inners.ZERO ZERO_1, Inners.NINE NINE_1)
                {
                    this.ZERO_1 = ZERO_1;
                    this.NINE_1 = NINE_1;
                }
                
                public Inners.ZERO ZERO_1 { get; }
                public Inners.NINE NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ZEROA : percentxZEROZEROⲻFF
            {
                public ZEROA(Inners.ZERO ZERO_1, Inners.A A_1)
                {
                    this.ZERO_1 = ZERO_1;
                    this.A_1 = A_1;
                }
                
                public Inners.ZERO ZERO_1 { get; }
                public Inners.A A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ZEROB : percentxZEROZEROⲻFF
            {
                public ZEROB(Inners.ZERO ZERO_1, Inners.B B_1)
                {
                    this.ZERO_1 = ZERO_1;
                    this.B_1 = B_1;
                }
                
                public Inners.ZERO ZERO_1 { get; }
                public Inners.B B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ZEROC : percentxZEROZEROⲻFF
            {
                public ZEROC(Inners.ZERO ZERO_1, Inners.C C_1)
                {
                    this.ZERO_1 = ZERO_1;
                    this.C_1 = C_1;
                }
                
                public Inners.ZERO ZERO_1 { get; }
                public Inners.C C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ZEROD : percentxZEROZEROⲻFF
            {
                public ZEROD(Inners.ZERO ZERO_1, Inners.D D_1)
                {
                    this.ZERO_1 = ZERO_1;
                    this.D_1 = D_1;
                }
                
                public Inners.ZERO ZERO_1 { get; }
                public Inners.D D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ZEROE : percentxZEROZEROⲻFF
            {
                public ZEROE(Inners.ZERO ZERO_1, Inners.E E_1)
                {
                    this.ZERO_1 = ZERO_1;
                    this.E_1 = E_1;
                }
                
                public Inners.ZERO ZERO_1 { get; }
                public Inners.E E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ZEROF : percentxZEROZEROⲻFF
            {
                public ZEROF(Inners.ZERO ZERO_1, Inners.F F_1)
                {
                    this.ZERO_1 = ZERO_1;
                    this.F_1 = F_1;
                }
                
                public Inners.ZERO ZERO_1 { get; }
                public Inners.F F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ONEZERO : percentxZEROZEROⲻFF
            {
                public ONEZERO(Inners.ONE ONE_1, Inners.ZERO ZERO_1)
                {
                    this.ONE_1 = ONE_1;
                    this.ZERO_1 = ZERO_1;
                }
                
                public Inners.ONE ONE_1 { get; }
                public Inners.ZERO ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ONEONE : percentxZEROZEROⲻFF
            {
                public ONEONE(Inners.ONE ONE_1, Inners.ONE ONE_2)
                {
                    this.ONE_1 = ONE_1;
                    this.ONE_2 = ONE_2;
                }
                
                public Inners.ONE ONE_1 { get; }
                public Inners.ONE ONE_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ONETWO : percentxZEROZEROⲻFF
            {
                public ONETWO(Inners.ONE ONE_1, Inners.TWO TWO_1)
                {
                    this.ONE_1 = ONE_1;
                    this.TWO_1 = TWO_1;
                }
                
                public Inners.ONE ONE_1 { get; }
                public Inners.TWO TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ONETHREE : percentxZEROZEROⲻFF
            {
                public ONETHREE(Inners.ONE ONE_1, Inners.THREE THREE_1)
                {
                    this.ONE_1 = ONE_1;
                    this.THREE_1 = THREE_1;
                }
                
                public Inners.ONE ONE_1 { get; }
                public Inners.THREE THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ONEFOUR : percentxZEROZEROⲻFF
            {
                public ONEFOUR(Inners.ONE ONE_1, Inners.FOUR FOUR_1)
                {
                    this.ONE_1 = ONE_1;
                    this.FOUR_1 = FOUR_1;
                }
                
                public Inners.ONE ONE_1 { get; }
                public Inners.FOUR FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ONEFIVE : percentxZEROZEROⲻFF
            {
                public ONEFIVE(Inners.ONE ONE_1, Inners.FIVE FIVE_1)
                {
                    this.ONE_1 = ONE_1;
                    this.FIVE_1 = FIVE_1;
                }
                
                public Inners.ONE ONE_1 { get; }
                public Inners.FIVE FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ONESIX : percentxZEROZEROⲻFF
            {
                public ONESIX(Inners.ONE ONE_1, Inners.SIX SIX_1)
                {
                    this.ONE_1 = ONE_1;
                    this.SIX_1 = SIX_1;
                }
                
                public Inners.ONE ONE_1 { get; }
                public Inners.SIX SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ONESEVEN : percentxZEROZEROⲻFF
            {
                public ONESEVEN(Inners.ONE ONE_1, Inners.SEVEN SEVEN_1)
                {
                    this.ONE_1 = ONE_1;
                    this.SEVEN_1 = SEVEN_1;
                }
                
                public Inners.ONE ONE_1 { get; }
                public Inners.SEVEN SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ONEEIGHT : percentxZEROZEROⲻFF
            {
                public ONEEIGHT(Inners.ONE ONE_1, Inners.EIGHT EIGHT_1)
                {
                    this.ONE_1 = ONE_1;
                    this.EIGHT_1 = EIGHT_1;
                }
                
                public Inners.ONE ONE_1 { get; }
                public Inners.EIGHT EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ONENINE : percentxZEROZEROⲻFF
            {
                public ONENINE(Inners.ONE ONE_1, Inners.NINE NINE_1)
                {
                    this.ONE_1 = ONE_1;
                    this.NINE_1 = NINE_1;
                }
                
                public Inners.ONE ONE_1 { get; }
                public Inners.NINE NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ONEA : percentxZEROZEROⲻFF
            {
                public ONEA(Inners.ONE ONE_1, Inners.A A_1)
                {
                    this.ONE_1 = ONE_1;
                    this.A_1 = A_1;
                }
                
                public Inners.ONE ONE_1 { get; }
                public Inners.A A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ONEB : percentxZEROZEROⲻFF
            {
                public ONEB(Inners.ONE ONE_1, Inners.B B_1)
                {
                    this.ONE_1 = ONE_1;
                    this.B_1 = B_1;
                }
                
                public Inners.ONE ONE_1 { get; }
                public Inners.B B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ONEC : percentxZEROZEROⲻFF
            {
                public ONEC(Inners.ONE ONE_1, Inners.C C_1)
                {
                    this.ONE_1 = ONE_1;
                    this.C_1 = C_1;
                }
                
                public Inners.ONE ONE_1 { get; }
                public Inners.C C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ONED : percentxZEROZEROⲻFF
            {
                public ONED(Inners.ONE ONE_1, Inners.D D_1)
                {
                    this.ONE_1 = ONE_1;
                    this.D_1 = D_1;
                }
                
                public Inners.ONE ONE_1 { get; }
                public Inners.D D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ONEE : percentxZEROZEROⲻFF
            {
                public ONEE(Inners.ONE ONE_1, Inners.E E_1)
                {
                    this.ONE_1 = ONE_1;
                    this.E_1 = E_1;
                }
                
                public Inners.ONE ONE_1 { get; }
                public Inners.E E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ONEF : percentxZEROZEROⲻFF
            {
                public ONEF(Inners.ONE ONE_1, Inners.F F_1)
                {
                    this.ONE_1 = ONE_1;
                    this.F_1 = F_1;
                }
                
                public Inners.ONE ONE_1 { get; }
                public Inners.F F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOZERO : percentxZEROZEROⲻFF
            {
                public TWOZERO(Inners.TWO TWO_1, Inners.ZERO ZERO_1)
                {
                    this.TWO_1 = TWO_1;
                    this.ZERO_1 = ZERO_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.ZERO ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOONE : percentxZEROZEROⲻFF
            {
                public TWOONE(Inners.TWO TWO_1, Inners.ONE ONE_1)
                {
                    this.TWO_1 = TWO_1;
                    this.ONE_1 = ONE_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.ONE ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOTWO : percentxZEROZEROⲻFF
            {
                public TWOTWO(Inners.TWO TWO_1, Inners.TWO TWO_2)
                {
                    this.TWO_1 = TWO_1;
                    this.TWO_2 = TWO_2;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.TWO TWO_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOTHREE : percentxZEROZEROⲻFF
            {
                public TWOTHREE(Inners.TWO TWO_1, Inners.THREE THREE_1)
                {
                    this.TWO_1 = TWO_1;
                    this.THREE_1 = THREE_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.THREE THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOFOUR : percentxZEROZEROⲻFF
            {
                public TWOFOUR(Inners.TWO TWO_1, Inners.FOUR FOUR_1)
                {
                    this.TWO_1 = TWO_1;
                    this.FOUR_1 = FOUR_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.FOUR FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOFIVE : percentxZEROZEROⲻFF
            {
                public TWOFIVE(Inners.TWO TWO_1, Inners.FIVE FIVE_1)
                {
                    this.TWO_1 = TWO_1;
                    this.FIVE_1 = FIVE_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.FIVE FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOSIX : percentxZEROZEROⲻFF
            {
                public TWOSIX(Inners.TWO TWO_1, Inners.SIX SIX_1)
                {
                    this.TWO_1 = TWO_1;
                    this.SIX_1 = SIX_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.SIX SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOSEVEN : percentxZEROZEROⲻFF
            {
                public TWOSEVEN(Inners.TWO TWO_1, Inners.SEVEN SEVEN_1)
                {
                    this.TWO_1 = TWO_1;
                    this.SEVEN_1 = SEVEN_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.SEVEN SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOEIGHT : percentxZEROZEROⲻFF
            {
                public TWOEIGHT(Inners.TWO TWO_1, Inners.EIGHT EIGHT_1)
                {
                    this.TWO_1 = TWO_1;
                    this.EIGHT_1 = EIGHT_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.EIGHT EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWONINE : percentxZEROZEROⲻFF
            {
                public TWONINE(Inners.TWO TWO_1, Inners.NINE NINE_1)
                {
                    this.TWO_1 = TWO_1;
                    this.NINE_1 = NINE_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.NINE NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOA : percentxZEROZEROⲻFF
            {
                public TWOA(Inners.TWO TWO_1, Inners.A A_1)
                {
                    this.TWO_1 = TWO_1;
                    this.A_1 = A_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.A A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOB : percentxZEROZEROⲻFF
            {
                public TWOB(Inners.TWO TWO_1, Inners.B B_1)
                {
                    this.TWO_1 = TWO_1;
                    this.B_1 = B_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.B B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOC : percentxZEROZEROⲻFF
            {
                public TWOC(Inners.TWO TWO_1, Inners.C C_1)
                {
                    this.TWO_1 = TWO_1;
                    this.C_1 = C_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.C C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOD : percentxZEROZEROⲻFF
            {
                public TWOD(Inners.TWO TWO_1, Inners.D D_1)
                {
                    this.TWO_1 = TWO_1;
                    this.D_1 = D_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.D D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOE : percentxZEROZEROⲻFF
            {
                public TWOE(Inners.TWO TWO_1, Inners.E E_1)
                {
                    this.TWO_1 = TWO_1;
                    this.E_1 = E_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.E E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOF : percentxZEROZEROⲻFF
            {
                public TWOF(Inners.TWO TWO_1, Inners.F F_1)
                {
                    this.TWO_1 = TWO_1;
                    this.F_1 = F_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.F F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREEZERO : percentxZEROZEROⲻFF
            {
                public THREEZERO(Inners.THREE THREE_1, Inners.ZERO ZERO_1)
                {
                    this.THREE_1 = THREE_1;
                    this.ZERO_1 = ZERO_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.ZERO ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREEONE : percentxZEROZEROⲻFF
            {
                public THREEONE(Inners.THREE THREE_1, Inners.ONE ONE_1)
                {
                    this.THREE_1 = THREE_1;
                    this.ONE_1 = ONE_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.ONE ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREETWO : percentxZEROZEROⲻFF
            {
                public THREETWO(Inners.THREE THREE_1, Inners.TWO TWO_1)
                {
                    this.THREE_1 = THREE_1;
                    this.TWO_1 = TWO_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.TWO TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREETHREE : percentxZEROZEROⲻFF
            {
                public THREETHREE(Inners.THREE THREE_1, Inners.THREE THREE_2)
                {
                    this.THREE_1 = THREE_1;
                    this.THREE_2 = THREE_2;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.THREE THREE_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREEFOUR : percentxZEROZEROⲻFF
            {
                public THREEFOUR(Inners.THREE THREE_1, Inners.FOUR FOUR_1)
                {
                    this.THREE_1 = THREE_1;
                    this.FOUR_1 = FOUR_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.FOUR FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREEFIVE : percentxZEROZEROⲻFF
            {
                public THREEFIVE(Inners.THREE THREE_1, Inners.FIVE FIVE_1)
                {
                    this.THREE_1 = THREE_1;
                    this.FIVE_1 = FIVE_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.FIVE FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREESIX : percentxZEROZEROⲻFF
            {
                public THREESIX(Inners.THREE THREE_1, Inners.SIX SIX_1)
                {
                    this.THREE_1 = THREE_1;
                    this.SIX_1 = SIX_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.SIX SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREESEVEN : percentxZEROZEROⲻFF
            {
                public THREESEVEN(Inners.THREE THREE_1, Inners.SEVEN SEVEN_1)
                {
                    this.THREE_1 = THREE_1;
                    this.SEVEN_1 = SEVEN_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.SEVEN SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREEEIGHT : percentxZEROZEROⲻFF
            {
                public THREEEIGHT(Inners.THREE THREE_1, Inners.EIGHT EIGHT_1)
                {
                    this.THREE_1 = THREE_1;
                    this.EIGHT_1 = EIGHT_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.EIGHT EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREENINE : percentxZEROZEROⲻFF
            {
                public THREENINE(Inners.THREE THREE_1, Inners.NINE NINE_1)
                {
                    this.THREE_1 = THREE_1;
                    this.NINE_1 = NINE_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.NINE NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREEA : percentxZEROZEROⲻFF
            {
                public THREEA(Inners.THREE THREE_1, Inners.A A_1)
                {
                    this.THREE_1 = THREE_1;
                    this.A_1 = A_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.A A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREEB : percentxZEROZEROⲻFF
            {
                public THREEB(Inners.THREE THREE_1, Inners.B B_1)
                {
                    this.THREE_1 = THREE_1;
                    this.B_1 = B_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.B B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREEC : percentxZEROZEROⲻFF
            {
                public THREEC(Inners.THREE THREE_1, Inners.C C_1)
                {
                    this.THREE_1 = THREE_1;
                    this.C_1 = C_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.C C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREED : percentxZEROZEROⲻFF
            {
                public THREED(Inners.THREE THREE_1, Inners.D D_1)
                {
                    this.THREE_1 = THREE_1;
                    this.D_1 = D_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.D D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREEE : percentxZEROZEROⲻFF
            {
                public THREEE(Inners.THREE THREE_1, Inners.E E_1)
                {
                    this.THREE_1 = THREE_1;
                    this.E_1 = E_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.E E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREEF : percentxZEROZEROⲻFF
            {
                public THREEF(Inners.THREE THREE_1, Inners.F F_1)
                {
                    this.THREE_1 = THREE_1;
                    this.F_1 = F_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.F F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURZERO : percentxZEROZEROⲻFF
            {
                public FOURZERO(Inners.FOUR FOUR_1, Inners.ZERO ZERO_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.ZERO_1 = ZERO_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.ZERO ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURONE : percentxZEROZEROⲻFF
            {
                public FOURONE(Inners.FOUR FOUR_1, Inners.ONE ONE_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.ONE_1 = ONE_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.ONE ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURTWO : percentxZEROZEROⲻFF
            {
                public FOURTWO(Inners.FOUR FOUR_1, Inners.TWO TWO_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.TWO_1 = TWO_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.TWO TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURTHREE : percentxZEROZEROⲻFF
            {
                public FOURTHREE(Inners.FOUR FOUR_1, Inners.THREE THREE_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.THREE_1 = THREE_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.THREE THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURFOUR : percentxZEROZEROⲻFF
            {
                public FOURFOUR(Inners.FOUR FOUR_1, Inners.FOUR FOUR_2)
                {
                    this.FOUR_1 = FOUR_1;
                    this.FOUR_2 = FOUR_2;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.FOUR FOUR_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURFIVE : percentxZEROZEROⲻFF
            {
                public FOURFIVE(Inners.FOUR FOUR_1, Inners.FIVE FIVE_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.FIVE_1 = FIVE_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.FIVE FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURSIX : percentxZEROZEROⲻFF
            {
                public FOURSIX(Inners.FOUR FOUR_1, Inners.SIX SIX_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.SIX_1 = SIX_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.SIX SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURSEVEN : percentxZEROZEROⲻFF
            {
                public FOURSEVEN(Inners.FOUR FOUR_1, Inners.SEVEN SEVEN_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.SEVEN_1 = SEVEN_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.SEVEN SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOUREIGHT : percentxZEROZEROⲻFF
            {
                public FOUREIGHT(Inners.FOUR FOUR_1, Inners.EIGHT EIGHT_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.EIGHT_1 = EIGHT_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.EIGHT EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURNINE : percentxZEROZEROⲻFF
            {
                public FOURNINE(Inners.FOUR FOUR_1, Inners.NINE NINE_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.NINE_1 = NINE_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.NINE NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURA : percentxZEROZEROⲻFF
            {
                public FOURA(Inners.FOUR FOUR_1, Inners.A A_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.A_1 = A_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.A A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURB : percentxZEROZEROⲻFF
            {
                public FOURB(Inners.FOUR FOUR_1, Inners.B B_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.B_1 = B_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.B B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURC : percentxZEROZEROⲻFF
            {
                public FOURC(Inners.FOUR FOUR_1, Inners.C C_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.C_1 = C_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.C C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURD : percentxZEROZEROⲻFF
            {
                public FOURD(Inners.FOUR FOUR_1, Inners.D D_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.D_1 = D_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.D D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURE : percentxZEROZEROⲻFF
            {
                public FOURE(Inners.FOUR FOUR_1, Inners.E E_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.E_1 = E_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.E E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURF : percentxZEROZEROⲻFF
            {
                public FOURF(Inners.FOUR FOUR_1, Inners.F F_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.F_1 = F_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.F F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVEZERO : percentxZEROZEROⲻFF
            {
                public FIVEZERO(Inners.FIVE FIVE_1, Inners.ZERO ZERO_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.ZERO_1 = ZERO_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.ZERO ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVEONE : percentxZEROZEROⲻFF
            {
                public FIVEONE(Inners.FIVE FIVE_1, Inners.ONE ONE_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.ONE_1 = ONE_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.ONE ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVETWO : percentxZEROZEROⲻFF
            {
                public FIVETWO(Inners.FIVE FIVE_1, Inners.TWO TWO_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.TWO_1 = TWO_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.TWO TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVETHREE : percentxZEROZEROⲻFF
            {
                public FIVETHREE(Inners.FIVE FIVE_1, Inners.THREE THREE_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.THREE_1 = THREE_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.THREE THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVEFOUR : percentxZEROZEROⲻFF
            {
                public FIVEFOUR(Inners.FIVE FIVE_1, Inners.FOUR FOUR_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.FOUR_1 = FOUR_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.FOUR FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVEFIVE : percentxZEROZEROⲻFF
            {
                public FIVEFIVE(Inners.FIVE FIVE_1, Inners.FIVE FIVE_2)
                {
                    this.FIVE_1 = FIVE_1;
                    this.FIVE_2 = FIVE_2;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.FIVE FIVE_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVESIX : percentxZEROZEROⲻFF
            {
                public FIVESIX(Inners.FIVE FIVE_1, Inners.SIX SIX_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.SIX_1 = SIX_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.SIX SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVESEVEN : percentxZEROZEROⲻFF
            {
                public FIVESEVEN(Inners.FIVE FIVE_1, Inners.SEVEN SEVEN_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.SEVEN_1 = SEVEN_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.SEVEN SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVEEIGHT : percentxZEROZEROⲻFF
            {
                public FIVEEIGHT(Inners.FIVE FIVE_1, Inners.EIGHT EIGHT_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.EIGHT_1 = EIGHT_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.EIGHT EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVENINE : percentxZEROZEROⲻFF
            {
                public FIVENINE(Inners.FIVE FIVE_1, Inners.NINE NINE_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.NINE_1 = NINE_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.NINE NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVEA : percentxZEROZEROⲻFF
            {
                public FIVEA(Inners.FIVE FIVE_1, Inners.A A_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.A_1 = A_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.A A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVEB : percentxZEROZEROⲻFF
            {
                public FIVEB(Inners.FIVE FIVE_1, Inners.B B_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.B_1 = B_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.B B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVEC : percentxZEROZEROⲻFF
            {
                public FIVEC(Inners.FIVE FIVE_1, Inners.C C_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.C_1 = C_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.C C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVED : percentxZEROZEROⲻFF
            {
                public FIVED(Inners.FIVE FIVE_1, Inners.D D_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.D_1 = D_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.D D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVEE : percentxZEROZEROⲻFF
            {
                public FIVEE(Inners.FIVE FIVE_1, Inners.E E_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.E_1 = E_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.E E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVEF : percentxZEROZEROⲻFF
            {
                public FIVEF(Inners.FIVE FIVE_1, Inners.F F_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.F_1 = F_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.F F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXZERO : percentxZEROZEROⲻFF
            {
                public SIXZERO(Inners.SIX SIX_1, Inners.ZERO ZERO_1)
                {
                    this.SIX_1 = SIX_1;
                    this.ZERO_1 = ZERO_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.ZERO ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXONE : percentxZEROZEROⲻFF
            {
                public SIXONE(Inners.SIX SIX_1, Inners.ONE ONE_1)
                {
                    this.SIX_1 = SIX_1;
                    this.ONE_1 = ONE_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.ONE ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXTWO : percentxZEROZEROⲻFF
            {
                public SIXTWO(Inners.SIX SIX_1, Inners.TWO TWO_1)
                {
                    this.SIX_1 = SIX_1;
                    this.TWO_1 = TWO_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.TWO TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXTHREE : percentxZEROZEROⲻFF
            {
                public SIXTHREE(Inners.SIX SIX_1, Inners.THREE THREE_1)
                {
                    this.SIX_1 = SIX_1;
                    this.THREE_1 = THREE_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.THREE THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXFOUR : percentxZEROZEROⲻFF
            {
                public SIXFOUR(Inners.SIX SIX_1, Inners.FOUR FOUR_1)
                {
                    this.SIX_1 = SIX_1;
                    this.FOUR_1 = FOUR_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.FOUR FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXFIVE : percentxZEROZEROⲻFF
            {
                public SIXFIVE(Inners.SIX SIX_1, Inners.FIVE FIVE_1)
                {
                    this.SIX_1 = SIX_1;
                    this.FIVE_1 = FIVE_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.FIVE FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXSIX : percentxZEROZEROⲻFF
            {
                public SIXSIX(Inners.SIX SIX_1, Inners.SIX SIX_2)
                {
                    this.SIX_1 = SIX_1;
                    this.SIX_2 = SIX_2;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.SIX SIX_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXSEVEN : percentxZEROZEROⲻFF
            {
                public SIXSEVEN(Inners.SIX SIX_1, Inners.SEVEN SEVEN_1)
                {
                    this.SIX_1 = SIX_1;
                    this.SEVEN_1 = SEVEN_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.SEVEN SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXEIGHT : percentxZEROZEROⲻFF
            {
                public SIXEIGHT(Inners.SIX SIX_1, Inners.EIGHT EIGHT_1)
                {
                    this.SIX_1 = SIX_1;
                    this.EIGHT_1 = EIGHT_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.EIGHT EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXNINE : percentxZEROZEROⲻFF
            {
                public SIXNINE(Inners.SIX SIX_1, Inners.NINE NINE_1)
                {
                    this.SIX_1 = SIX_1;
                    this.NINE_1 = NINE_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.NINE NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXA : percentxZEROZEROⲻFF
            {
                public SIXA(Inners.SIX SIX_1, Inners.A A_1)
                {
                    this.SIX_1 = SIX_1;
                    this.A_1 = A_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.A A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXB : percentxZEROZEROⲻFF
            {
                public SIXB(Inners.SIX SIX_1, Inners.B B_1)
                {
                    this.SIX_1 = SIX_1;
                    this.B_1 = B_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.B B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXC : percentxZEROZEROⲻFF
            {
                public SIXC(Inners.SIX SIX_1, Inners.C C_1)
                {
                    this.SIX_1 = SIX_1;
                    this.C_1 = C_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.C C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXD : percentxZEROZEROⲻFF
            {
                public SIXD(Inners.SIX SIX_1, Inners.D D_1)
                {
                    this.SIX_1 = SIX_1;
                    this.D_1 = D_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.D D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXE : percentxZEROZEROⲻFF
            {
                public SIXE(Inners.SIX SIX_1, Inners.E E_1)
                {
                    this.SIX_1 = SIX_1;
                    this.E_1 = E_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.E E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXF : percentxZEROZEROⲻFF
            {
                public SIXF(Inners.SIX SIX_1, Inners.F F_1)
                {
                    this.SIX_1 = SIX_1;
                    this.F_1 = F_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.F F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENZERO : percentxZEROZEROⲻFF
            {
                public SEVENZERO(Inners.SEVEN SEVEN_1, Inners.ZERO ZERO_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.ZERO_1 = ZERO_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.ZERO ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENONE : percentxZEROZEROⲻFF
            {
                public SEVENONE(Inners.SEVEN SEVEN_1, Inners.ONE ONE_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.ONE_1 = ONE_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.ONE ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENTWO : percentxZEROZEROⲻFF
            {
                public SEVENTWO(Inners.SEVEN SEVEN_1, Inners.TWO TWO_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.TWO_1 = TWO_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.TWO TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENTHREE : percentxZEROZEROⲻFF
            {
                public SEVENTHREE(Inners.SEVEN SEVEN_1, Inners.THREE THREE_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.THREE_1 = THREE_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.THREE THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENFOUR : percentxZEROZEROⲻFF
            {
                public SEVENFOUR(Inners.SEVEN SEVEN_1, Inners.FOUR FOUR_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.FOUR_1 = FOUR_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.FOUR FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENFIVE : percentxZEROZEROⲻFF
            {
                public SEVENFIVE(Inners.SEVEN SEVEN_1, Inners.FIVE FIVE_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.FIVE_1 = FIVE_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.FIVE FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENSIX : percentxZEROZEROⲻFF
            {
                public SEVENSIX(Inners.SEVEN SEVEN_1, Inners.SIX SIX_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.SIX_1 = SIX_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.SIX SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENSEVEN : percentxZEROZEROⲻFF
            {
                public SEVENSEVEN(Inners.SEVEN SEVEN_1, Inners.SEVEN SEVEN_2)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.SEVEN_2 = SEVEN_2;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.SEVEN SEVEN_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENEIGHT : percentxZEROZEROⲻFF
            {
                public SEVENEIGHT(Inners.SEVEN SEVEN_1, Inners.EIGHT EIGHT_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.EIGHT_1 = EIGHT_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.EIGHT EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENNINE : percentxZEROZEROⲻFF
            {
                public SEVENNINE(Inners.SEVEN SEVEN_1, Inners.NINE NINE_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.NINE_1 = NINE_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.NINE NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENA : percentxZEROZEROⲻFF
            {
                public SEVENA(Inners.SEVEN SEVEN_1, Inners.A A_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.A_1 = A_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.A A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENB : percentxZEROZEROⲻFF
            {
                public SEVENB(Inners.SEVEN SEVEN_1, Inners.B B_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.B_1 = B_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.B B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENC : percentxZEROZEROⲻFF
            {
                public SEVENC(Inners.SEVEN SEVEN_1, Inners.C C_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.C_1 = C_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.C C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVEND : percentxZEROZEROⲻFF
            {
                public SEVEND(Inners.SEVEN SEVEN_1, Inners.D D_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.D_1 = D_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.D D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENE : percentxZEROZEROⲻFF
            {
                public SEVENE(Inners.SEVEN SEVEN_1, Inners.E E_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.E_1 = E_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.E E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENF : percentxZEROZEROⲻFF
            {
                public SEVENF(Inners.SEVEN SEVEN_1, Inners.F F_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.F_1 = F_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.F F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class EIGHTZERO : percentxZEROZEROⲻFF
            {
                public EIGHTZERO(Inners.EIGHT EIGHT_1, Inners.ZERO ZERO_1)
                {
                    this.EIGHT_1 = EIGHT_1;
                    this.ZERO_1 = ZERO_1;
                }
                
                public Inners.EIGHT EIGHT_1 { get; }
                public Inners.ZERO ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class EIGHTONE : percentxZEROZEROⲻFF
            {
                public EIGHTONE(Inners.EIGHT EIGHT_1, Inners.ONE ONE_1)
                {
                    this.EIGHT_1 = EIGHT_1;
                    this.ONE_1 = ONE_1;
                }
                
                public Inners.EIGHT EIGHT_1 { get; }
                public Inners.ONE ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class EIGHTTWO : percentxZEROZEROⲻFF
            {
                public EIGHTTWO(Inners.EIGHT EIGHT_1, Inners.TWO TWO_1)
                {
                    this.EIGHT_1 = EIGHT_1;
                    this.TWO_1 = TWO_1;
                }
                
                public Inners.EIGHT EIGHT_1 { get; }
                public Inners.TWO TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class EIGHTTHREE : percentxZEROZEROⲻFF
            {
                public EIGHTTHREE(Inners.EIGHT EIGHT_1, Inners.THREE THREE_1)
                {
                    this.EIGHT_1 = EIGHT_1;
                    this.THREE_1 = THREE_1;
                }
                
                public Inners.EIGHT EIGHT_1 { get; }
                public Inners.THREE THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class EIGHTFOUR : percentxZEROZEROⲻFF
            {
                public EIGHTFOUR(Inners.EIGHT EIGHT_1, Inners.FOUR FOUR_1)
                {
                    this.EIGHT_1 = EIGHT_1;
                    this.FOUR_1 = FOUR_1;
                }
                
                public Inners.EIGHT EIGHT_1 { get; }
                public Inners.FOUR FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class EIGHTFIVE : percentxZEROZEROⲻFF
            {
                public EIGHTFIVE(Inners.EIGHT EIGHT_1, Inners.FIVE FIVE_1)
                {
                    this.EIGHT_1 = EIGHT_1;
                    this.FIVE_1 = FIVE_1;
                }
                
                public Inners.EIGHT EIGHT_1 { get; }
                public Inners.FIVE FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class EIGHTSIX : percentxZEROZEROⲻFF
            {
                public EIGHTSIX(Inners.EIGHT EIGHT_1, Inners.SIX SIX_1)
                {
                    this.EIGHT_1 = EIGHT_1;
                    this.SIX_1 = SIX_1;
                }
                
                public Inners.EIGHT EIGHT_1 { get; }
                public Inners.SIX SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class EIGHTSEVEN : percentxZEROZEROⲻFF
            {
                public EIGHTSEVEN(Inners.EIGHT EIGHT_1, Inners.SEVEN SEVEN_1)
                {
                    this.EIGHT_1 = EIGHT_1;
                    this.SEVEN_1 = SEVEN_1;
                }
                
                public Inners.EIGHT EIGHT_1 { get; }
                public Inners.SEVEN SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class EIGHTEIGHT : percentxZEROZEROⲻFF
            {
                public EIGHTEIGHT(Inners.EIGHT EIGHT_1, Inners.EIGHT EIGHT_2)
                {
                    this.EIGHT_1 = EIGHT_1;
                    this.EIGHT_2 = EIGHT_2;
                }
                
                public Inners.EIGHT EIGHT_1 { get; }
                public Inners.EIGHT EIGHT_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class EIGHTNINE : percentxZEROZEROⲻFF
            {
                public EIGHTNINE(Inners.EIGHT EIGHT_1, Inners.NINE NINE_1)
                {
                    this.EIGHT_1 = EIGHT_1;
                    this.NINE_1 = NINE_1;
                }
                
                public Inners.EIGHT EIGHT_1 { get; }
                public Inners.NINE NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class EIGHTA : percentxZEROZEROⲻFF
            {
                public EIGHTA(Inners.EIGHT EIGHT_1, Inners.A A_1)
                {
                    this.EIGHT_1 = EIGHT_1;
                    this.A_1 = A_1;
                }
                
                public Inners.EIGHT EIGHT_1 { get; }
                public Inners.A A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class EIGHTB : percentxZEROZEROⲻFF
            {
                public EIGHTB(Inners.EIGHT EIGHT_1, Inners.B B_1)
                {
                    this.EIGHT_1 = EIGHT_1;
                    this.B_1 = B_1;
                }
                
                public Inners.EIGHT EIGHT_1 { get; }
                public Inners.B B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class EIGHTC : percentxZEROZEROⲻFF
            {
                public EIGHTC(Inners.EIGHT EIGHT_1, Inners.C C_1)
                {
                    this.EIGHT_1 = EIGHT_1;
                    this.C_1 = C_1;
                }
                
                public Inners.EIGHT EIGHT_1 { get; }
                public Inners.C C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class EIGHTD : percentxZEROZEROⲻFF
            {
                public EIGHTD(Inners.EIGHT EIGHT_1, Inners.D D_1)
                {
                    this.EIGHT_1 = EIGHT_1;
                    this.D_1 = D_1;
                }
                
                public Inners.EIGHT EIGHT_1 { get; }
                public Inners.D D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class EIGHTE : percentxZEROZEROⲻFF
            {
                public EIGHTE(Inners.EIGHT EIGHT_1, Inners.E E_1)
                {
                    this.EIGHT_1 = EIGHT_1;
                    this.E_1 = E_1;
                }
                
                public Inners.EIGHT EIGHT_1 { get; }
                public Inners.E E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class EIGHTF : percentxZEROZEROⲻFF
            {
                public EIGHTF(Inners.EIGHT EIGHT_1, Inners.F F_1)
                {
                    this.EIGHT_1 = EIGHT_1;
                    this.F_1 = F_1;
                }
                
                public Inners.EIGHT EIGHT_1 { get; }
                public Inners.F F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class NINEZERO : percentxZEROZEROⲻFF
            {
                public NINEZERO(Inners.NINE NINE_1, Inners.ZERO ZERO_1)
                {
                    this.NINE_1 = NINE_1;
                    this.ZERO_1 = ZERO_1;
                }
                
                public Inners.NINE NINE_1 { get; }
                public Inners.ZERO ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class NINEONE : percentxZEROZEROⲻFF
            {
                public NINEONE(Inners.NINE NINE_1, Inners.ONE ONE_1)
                {
                    this.NINE_1 = NINE_1;
                    this.ONE_1 = ONE_1;
                }
                
                public Inners.NINE NINE_1 { get; }
                public Inners.ONE ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class NINETWO : percentxZEROZEROⲻFF
            {
                public NINETWO(Inners.NINE NINE_1, Inners.TWO TWO_1)
                {
                    this.NINE_1 = NINE_1;
                    this.TWO_1 = TWO_1;
                }
                
                public Inners.NINE NINE_1 { get; }
                public Inners.TWO TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class NINETHREE : percentxZEROZEROⲻFF
            {
                public NINETHREE(Inners.NINE NINE_1, Inners.THREE THREE_1)
                {
                    this.NINE_1 = NINE_1;
                    this.THREE_1 = THREE_1;
                }
                
                public Inners.NINE NINE_1 { get; }
                public Inners.THREE THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class NINEFOUR : percentxZEROZEROⲻFF
            {
                public NINEFOUR(Inners.NINE NINE_1, Inners.FOUR FOUR_1)
                {
                    this.NINE_1 = NINE_1;
                    this.FOUR_1 = FOUR_1;
                }
                
                public Inners.NINE NINE_1 { get; }
                public Inners.FOUR FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class NINEFIVE : percentxZEROZEROⲻFF
            {
                public NINEFIVE(Inners.NINE NINE_1, Inners.FIVE FIVE_1)
                {
                    this.NINE_1 = NINE_1;
                    this.FIVE_1 = FIVE_1;
                }
                
                public Inners.NINE NINE_1 { get; }
                public Inners.FIVE FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class NINESIX : percentxZEROZEROⲻFF
            {
                public NINESIX(Inners.NINE NINE_1, Inners.SIX SIX_1)
                {
                    this.NINE_1 = NINE_1;
                    this.SIX_1 = SIX_1;
                }
                
                public Inners.NINE NINE_1 { get; }
                public Inners.SIX SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class NINESEVEN : percentxZEROZEROⲻFF
            {
                public NINESEVEN(Inners.NINE NINE_1, Inners.SEVEN SEVEN_1)
                {
                    this.NINE_1 = NINE_1;
                    this.SEVEN_1 = SEVEN_1;
                }
                
                public Inners.NINE NINE_1 { get; }
                public Inners.SEVEN SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class NINEEIGHT : percentxZEROZEROⲻFF
            {
                public NINEEIGHT(Inners.NINE NINE_1, Inners.EIGHT EIGHT_1)
                {
                    this.NINE_1 = NINE_1;
                    this.EIGHT_1 = EIGHT_1;
                }
                
                public Inners.NINE NINE_1 { get; }
                public Inners.EIGHT EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class NINENINE : percentxZEROZEROⲻFF
            {
                public NINENINE(Inners.NINE NINE_1, Inners.NINE NINE_2)
                {
                    this.NINE_1 = NINE_1;
                    this.NINE_2 = NINE_2;
                }
                
                public Inners.NINE NINE_1 { get; }
                public Inners.NINE NINE_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class NINEA : percentxZEROZEROⲻFF
            {
                public NINEA(Inners.NINE NINE_1, Inners.A A_1)
                {
                    this.NINE_1 = NINE_1;
                    this.A_1 = A_1;
                }
                
                public Inners.NINE NINE_1 { get; }
                public Inners.A A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class NINEB : percentxZEROZEROⲻFF
            {
                public NINEB(Inners.NINE NINE_1, Inners.B B_1)
                {
                    this.NINE_1 = NINE_1;
                    this.B_1 = B_1;
                }
                
                public Inners.NINE NINE_1 { get; }
                public Inners.B B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class NINEC : percentxZEROZEROⲻFF
            {
                public NINEC(Inners.NINE NINE_1, Inners.C C_1)
                {
                    this.NINE_1 = NINE_1;
                    this.C_1 = C_1;
                }
                
                public Inners.NINE NINE_1 { get; }
                public Inners.C C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class NINED : percentxZEROZEROⲻFF
            {
                public NINED(Inners.NINE NINE_1, Inners.D D_1)
                {
                    this.NINE_1 = NINE_1;
                    this.D_1 = D_1;
                }
                
                public Inners.NINE NINE_1 { get; }
                public Inners.D D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class NINEE : percentxZEROZEROⲻFF
            {
                public NINEE(Inners.NINE NINE_1, Inners.E E_1)
                {
                    this.NINE_1 = NINE_1;
                    this.E_1 = E_1;
                }
                
                public Inners.NINE NINE_1 { get; }
                public Inners.E E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class NINEF : percentxZEROZEROⲻFF
            {
                public NINEF(Inners.NINE NINE_1, Inners.F F_1)
                {
                    this.NINE_1 = NINE_1;
                    this.F_1 = F_1;
                }
                
                public Inners.NINE NINE_1 { get; }
                public Inners.F F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class AZERO : percentxZEROZEROⲻFF
            {
                public AZERO(Inners.A A_1, Inners.ZERO ZERO_1)
                {
                    this.A_1 = A_1;
                    this.ZERO_1 = ZERO_1;
                }
                
                public Inners.A A_1 { get; }
                public Inners.ZERO ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class AONE : percentxZEROZEROⲻFF
            {
                public AONE(Inners.A A_1, Inners.ONE ONE_1)
                {
                    this.A_1 = A_1;
                    this.ONE_1 = ONE_1;
                }
                
                public Inners.A A_1 { get; }
                public Inners.ONE ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ATWO : percentxZEROZEROⲻFF
            {
                public ATWO(Inners.A A_1, Inners.TWO TWO_1)
                {
                    this.A_1 = A_1;
                    this.TWO_1 = TWO_1;
                }
                
                public Inners.A A_1 { get; }
                public Inners.TWO TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ATHREE : percentxZEROZEROⲻFF
            {
                public ATHREE(Inners.A A_1, Inners.THREE THREE_1)
                {
                    this.A_1 = A_1;
                    this.THREE_1 = THREE_1;
                }
                
                public Inners.A A_1 { get; }
                public Inners.THREE THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class AFOUR : percentxZEROZEROⲻFF
            {
                public AFOUR(Inners.A A_1, Inners.FOUR FOUR_1)
                {
                    this.A_1 = A_1;
                    this.FOUR_1 = FOUR_1;
                }
                
                public Inners.A A_1 { get; }
                public Inners.FOUR FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class AFIVE : percentxZEROZEROⲻFF
            {
                public AFIVE(Inners.A A_1, Inners.FIVE FIVE_1)
                {
                    this.A_1 = A_1;
                    this.FIVE_1 = FIVE_1;
                }
                
                public Inners.A A_1 { get; }
                public Inners.FIVE FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ASIX : percentxZEROZEROⲻFF
            {
                public ASIX(Inners.A A_1, Inners.SIX SIX_1)
                {
                    this.A_1 = A_1;
                    this.SIX_1 = SIX_1;
                }
                
                public Inners.A A_1 { get; }
                public Inners.SIX SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ASEVEN : percentxZEROZEROⲻFF
            {
                public ASEVEN(Inners.A A_1, Inners.SEVEN SEVEN_1)
                {
                    this.A_1 = A_1;
                    this.SEVEN_1 = SEVEN_1;
                }
                
                public Inners.A A_1 { get; }
                public Inners.SEVEN SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class AEIGHT : percentxZEROZEROⲻFF
            {
                public AEIGHT(Inners.A A_1, Inners.EIGHT EIGHT_1)
                {
                    this.A_1 = A_1;
                    this.EIGHT_1 = EIGHT_1;
                }
                
                public Inners.A A_1 { get; }
                public Inners.EIGHT EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ANINE : percentxZEROZEROⲻFF
            {
                public ANINE(Inners.A A_1, Inners.NINE NINE_1)
                {
                    this.A_1 = A_1;
                    this.NINE_1 = NINE_1;
                }
                
                public Inners.A A_1 { get; }
                public Inners.NINE NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class AA : percentxZEROZEROⲻFF
            {
                public AA(Inners.A A_1, Inners.A A_2)
                {
                    this.A_1 = A_1;
                    this.A_2 = A_2;
                }
                
                public Inners.A A_1 { get; }
                public Inners.A A_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class AB : percentxZEROZEROⲻFF
            {
                public AB(Inners.A A_1, Inners.B B_1)
                {
                    this.A_1 = A_1;
                    this.B_1 = B_1;
                }
                
                public Inners.A A_1 { get; }
                public Inners.B B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class AC : percentxZEROZEROⲻFF
            {
                public AC(Inners.A A_1, Inners.C C_1)
                {
                    this.A_1 = A_1;
                    this.C_1 = C_1;
                }
                
                public Inners.A A_1 { get; }
                public Inners.C C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class AD : percentxZEROZEROⲻFF
            {
                public AD(Inners.A A_1, Inners.D D_1)
                {
                    this.A_1 = A_1;
                    this.D_1 = D_1;
                }
                
                public Inners.A A_1 { get; }
                public Inners.D D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class AE : percentxZEROZEROⲻFF
            {
                public AE(Inners.A A_1, Inners.E E_1)
                {
                    this.A_1 = A_1;
                    this.E_1 = E_1;
                }
                
                public Inners.A A_1 { get; }
                public Inners.E E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class AF : percentxZEROZEROⲻFF
            {
                public AF(Inners.A A_1, Inners.F F_1)
                {
                    this.A_1 = A_1;
                    this.F_1 = F_1;
                }
                
                public Inners.A A_1 { get; }
                public Inners.F F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class BZERO : percentxZEROZEROⲻFF
            {
                public BZERO(Inners.B B_1, Inners.ZERO ZERO_1)
                {
                    this.B_1 = B_1;
                    this.ZERO_1 = ZERO_1;
                }
                
                public Inners.B B_1 { get; }
                public Inners.ZERO ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class BONE : percentxZEROZEROⲻFF
            {
                public BONE(Inners.B B_1, Inners.ONE ONE_1)
                {
                    this.B_1 = B_1;
                    this.ONE_1 = ONE_1;
                }
                
                public Inners.B B_1 { get; }
                public Inners.ONE ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class BTWO : percentxZEROZEROⲻFF
            {
                public BTWO(Inners.B B_1, Inners.TWO TWO_1)
                {
                    this.B_1 = B_1;
                    this.TWO_1 = TWO_1;
                }
                
                public Inners.B B_1 { get; }
                public Inners.TWO TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class BTHREE : percentxZEROZEROⲻFF
            {
                public BTHREE(Inners.B B_1, Inners.THREE THREE_1)
                {
                    this.B_1 = B_1;
                    this.THREE_1 = THREE_1;
                }
                
                public Inners.B B_1 { get; }
                public Inners.THREE THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class BFOUR : percentxZEROZEROⲻFF
            {
                public BFOUR(Inners.B B_1, Inners.FOUR FOUR_1)
                {
                    this.B_1 = B_1;
                    this.FOUR_1 = FOUR_1;
                }
                
                public Inners.B B_1 { get; }
                public Inners.FOUR FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class BFIVE : percentxZEROZEROⲻFF
            {
                public BFIVE(Inners.B B_1, Inners.FIVE FIVE_1)
                {
                    this.B_1 = B_1;
                    this.FIVE_1 = FIVE_1;
                }
                
                public Inners.B B_1 { get; }
                public Inners.FIVE FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class BSIX : percentxZEROZEROⲻFF
            {
                public BSIX(Inners.B B_1, Inners.SIX SIX_1)
                {
                    this.B_1 = B_1;
                    this.SIX_1 = SIX_1;
                }
                
                public Inners.B B_1 { get; }
                public Inners.SIX SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class BSEVEN : percentxZEROZEROⲻFF
            {
                public BSEVEN(Inners.B B_1, Inners.SEVEN SEVEN_1)
                {
                    this.B_1 = B_1;
                    this.SEVEN_1 = SEVEN_1;
                }
                
                public Inners.B B_1 { get; }
                public Inners.SEVEN SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class BEIGHT : percentxZEROZEROⲻFF
            {
                public BEIGHT(Inners.B B_1, Inners.EIGHT EIGHT_1)
                {
                    this.B_1 = B_1;
                    this.EIGHT_1 = EIGHT_1;
                }
                
                public Inners.B B_1 { get; }
                public Inners.EIGHT EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class BNINE : percentxZEROZEROⲻFF
            {
                public BNINE(Inners.B B_1, Inners.NINE NINE_1)
                {
                    this.B_1 = B_1;
                    this.NINE_1 = NINE_1;
                }
                
                public Inners.B B_1 { get; }
                public Inners.NINE NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class BA : percentxZEROZEROⲻFF
            {
                public BA(Inners.B B_1, Inners.A A_1)
                {
                    this.B_1 = B_1;
                    this.A_1 = A_1;
                }
                
                public Inners.B B_1 { get; }
                public Inners.A A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class BB : percentxZEROZEROⲻFF
            {
                public BB(Inners.B B_1, Inners.B B_2)
                {
                    this.B_1 = B_1;
                    this.B_2 = B_2;
                }
                
                public Inners.B B_1 { get; }
                public Inners.B B_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class BC : percentxZEROZEROⲻFF
            {
                public BC(Inners.B B_1, Inners.C C_1)
                {
                    this.B_1 = B_1;
                    this.C_1 = C_1;
                }
                
                public Inners.B B_1 { get; }
                public Inners.C C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class BD : percentxZEROZEROⲻFF
            {
                public BD(Inners.B B_1, Inners.D D_1)
                {
                    this.B_1 = B_1;
                    this.D_1 = D_1;
                }
                
                public Inners.B B_1 { get; }
                public Inners.D D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class BE : percentxZEROZEROⲻFF
            {
                public BE(Inners.B B_1, Inners.E E_1)
                {
                    this.B_1 = B_1;
                    this.E_1 = E_1;
                }
                
                public Inners.B B_1 { get; }
                public Inners.E E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class BF : percentxZEROZEROⲻFF
            {
                public BF(Inners.B B_1, Inners.F F_1)
                {
                    this.B_1 = B_1;
                    this.F_1 = F_1;
                }
                
                public Inners.B B_1 { get; }
                public Inners.F F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class CZERO : percentxZEROZEROⲻFF
            {
                public CZERO(Inners.C C_1, Inners.ZERO ZERO_1)
                {
                    this.C_1 = C_1;
                    this.ZERO_1 = ZERO_1;
                }
                
                public Inners.C C_1 { get; }
                public Inners.ZERO ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class CONE : percentxZEROZEROⲻFF
            {
                public CONE(Inners.C C_1, Inners.ONE ONE_1)
                {
                    this.C_1 = C_1;
                    this.ONE_1 = ONE_1;
                }
                
                public Inners.C C_1 { get; }
                public Inners.ONE ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class CTWO : percentxZEROZEROⲻFF
            {
                public CTWO(Inners.C C_1, Inners.TWO TWO_1)
                {
                    this.C_1 = C_1;
                    this.TWO_1 = TWO_1;
                }
                
                public Inners.C C_1 { get; }
                public Inners.TWO TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class CTHREE : percentxZEROZEROⲻFF
            {
                public CTHREE(Inners.C C_1, Inners.THREE THREE_1)
                {
                    this.C_1 = C_1;
                    this.THREE_1 = THREE_1;
                }
                
                public Inners.C C_1 { get; }
                public Inners.THREE THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class CFOUR : percentxZEROZEROⲻFF
            {
                public CFOUR(Inners.C C_1, Inners.FOUR FOUR_1)
                {
                    this.C_1 = C_1;
                    this.FOUR_1 = FOUR_1;
                }
                
                public Inners.C C_1 { get; }
                public Inners.FOUR FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class CFIVE : percentxZEROZEROⲻFF
            {
                public CFIVE(Inners.C C_1, Inners.FIVE FIVE_1)
                {
                    this.C_1 = C_1;
                    this.FIVE_1 = FIVE_1;
                }
                
                public Inners.C C_1 { get; }
                public Inners.FIVE FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class CSIX : percentxZEROZEROⲻFF
            {
                public CSIX(Inners.C C_1, Inners.SIX SIX_1)
                {
                    this.C_1 = C_1;
                    this.SIX_1 = SIX_1;
                }
                
                public Inners.C C_1 { get; }
                public Inners.SIX SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class CSEVEN : percentxZEROZEROⲻFF
            {
                public CSEVEN(Inners.C C_1, Inners.SEVEN SEVEN_1)
                {
                    this.C_1 = C_1;
                    this.SEVEN_1 = SEVEN_1;
                }
                
                public Inners.C C_1 { get; }
                public Inners.SEVEN SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class CEIGHT : percentxZEROZEROⲻFF
            {
                public CEIGHT(Inners.C C_1, Inners.EIGHT EIGHT_1)
                {
                    this.C_1 = C_1;
                    this.EIGHT_1 = EIGHT_1;
                }
                
                public Inners.C C_1 { get; }
                public Inners.EIGHT EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class CNINE : percentxZEROZEROⲻFF
            {
                public CNINE(Inners.C C_1, Inners.NINE NINE_1)
                {
                    this.C_1 = C_1;
                    this.NINE_1 = NINE_1;
                }
                
                public Inners.C C_1 { get; }
                public Inners.NINE NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class CA : percentxZEROZEROⲻFF
            {
                public CA(Inners.C C_1, Inners.A A_1)
                {
                    this.C_1 = C_1;
                    this.A_1 = A_1;
                }
                
                public Inners.C C_1 { get; }
                public Inners.A A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class CB : percentxZEROZEROⲻFF
            {
                public CB(Inners.C C_1, Inners.B B_1)
                {
                    this.C_1 = C_1;
                    this.B_1 = B_1;
                }
                
                public Inners.C C_1 { get; }
                public Inners.B B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class CC : percentxZEROZEROⲻFF
            {
                public CC(Inners.C C_1, Inners.C C_2)
                {
                    this.C_1 = C_1;
                    this.C_2 = C_2;
                }
                
                public Inners.C C_1 { get; }
                public Inners.C C_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class CD : percentxZEROZEROⲻFF
            {
                public CD(Inners.C C_1, Inners.D D_1)
                {
                    this.C_1 = C_1;
                    this.D_1 = D_1;
                }
                
                public Inners.C C_1 { get; }
                public Inners.D D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class CE : percentxZEROZEROⲻFF
            {
                public CE(Inners.C C_1, Inners.E E_1)
                {
                    this.C_1 = C_1;
                    this.E_1 = E_1;
                }
                
                public Inners.C C_1 { get; }
                public Inners.E E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class CF : percentxZEROZEROⲻFF
            {
                public CF(Inners.C C_1, Inners.F F_1)
                {
                    this.C_1 = C_1;
                    this.F_1 = F_1;
                }
                
                public Inners.C C_1 { get; }
                public Inners.F F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class DZERO : percentxZEROZEROⲻFF
            {
                public DZERO(Inners.D D_1, Inners.ZERO ZERO_1)
                {
                    this.D_1 = D_1;
                    this.ZERO_1 = ZERO_1;
                }
                
                public Inners.D D_1 { get; }
                public Inners.ZERO ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class DONE : percentxZEROZEROⲻFF
            {
                public DONE(Inners.D D_1, Inners.ONE ONE_1)
                {
                    this.D_1 = D_1;
                    this.ONE_1 = ONE_1;
                }
                
                public Inners.D D_1 { get; }
                public Inners.ONE ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class DTWO : percentxZEROZEROⲻFF
            {
                public DTWO(Inners.D D_1, Inners.TWO TWO_1)
                {
                    this.D_1 = D_1;
                    this.TWO_1 = TWO_1;
                }
                
                public Inners.D D_1 { get; }
                public Inners.TWO TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class DTHREE : percentxZEROZEROⲻFF
            {
                public DTHREE(Inners.D D_1, Inners.THREE THREE_1)
                {
                    this.D_1 = D_1;
                    this.THREE_1 = THREE_1;
                }
                
                public Inners.D D_1 { get; }
                public Inners.THREE THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class DFOUR : percentxZEROZEROⲻFF
            {
                public DFOUR(Inners.D D_1, Inners.FOUR FOUR_1)
                {
                    this.D_1 = D_1;
                    this.FOUR_1 = FOUR_1;
                }
                
                public Inners.D D_1 { get; }
                public Inners.FOUR FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class DFIVE : percentxZEROZEROⲻFF
            {
                public DFIVE(Inners.D D_1, Inners.FIVE FIVE_1)
                {
                    this.D_1 = D_1;
                    this.FIVE_1 = FIVE_1;
                }
                
                public Inners.D D_1 { get; }
                public Inners.FIVE FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class DSIX : percentxZEROZEROⲻFF
            {
                public DSIX(Inners.D D_1, Inners.SIX SIX_1)
                {
                    this.D_1 = D_1;
                    this.SIX_1 = SIX_1;
                }
                
                public Inners.D D_1 { get; }
                public Inners.SIX SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class DSEVEN : percentxZEROZEROⲻFF
            {
                public DSEVEN(Inners.D D_1, Inners.SEVEN SEVEN_1)
                {
                    this.D_1 = D_1;
                    this.SEVEN_1 = SEVEN_1;
                }
                
                public Inners.D D_1 { get; }
                public Inners.SEVEN SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class DEIGHT : percentxZEROZEROⲻFF
            {
                public DEIGHT(Inners.D D_1, Inners.EIGHT EIGHT_1)
                {
                    this.D_1 = D_1;
                    this.EIGHT_1 = EIGHT_1;
                }
                
                public Inners.D D_1 { get; }
                public Inners.EIGHT EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class DNINE : percentxZEROZEROⲻFF
            {
                public DNINE(Inners.D D_1, Inners.NINE NINE_1)
                {
                    this.D_1 = D_1;
                    this.NINE_1 = NINE_1;
                }
                
                public Inners.D D_1 { get; }
                public Inners.NINE NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class DA : percentxZEROZEROⲻFF
            {
                public DA(Inners.D D_1, Inners.A A_1)
                {
                    this.D_1 = D_1;
                    this.A_1 = A_1;
                }
                
                public Inners.D D_1 { get; }
                public Inners.A A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class DB : percentxZEROZEROⲻFF
            {
                public DB(Inners.D D_1, Inners.B B_1)
                {
                    this.D_1 = D_1;
                    this.B_1 = B_1;
                }
                
                public Inners.D D_1 { get; }
                public Inners.B B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class DC : percentxZEROZEROⲻFF
            {
                public DC(Inners.D D_1, Inners.C C_1)
                {
                    this.D_1 = D_1;
                    this.C_1 = C_1;
                }
                
                public Inners.D D_1 { get; }
                public Inners.C C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class DD : percentxZEROZEROⲻFF
            {
                public DD(Inners.D D_1, Inners.D D_2)
                {
                    this.D_1 = D_1;
                    this.D_2 = D_2;
                }
                
                public Inners.D D_1 { get; }
                public Inners.D D_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class DE : percentxZEROZEROⲻFF
            {
                public DE(Inners.D D_1, Inners.E E_1)
                {
                    this.D_1 = D_1;
                    this.E_1 = E_1;
                }
                
                public Inners.D D_1 { get; }
                public Inners.E E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class DF : percentxZEROZEROⲻFF
            {
                public DF(Inners.D D_1, Inners.F F_1)
                {
                    this.D_1 = D_1;
                    this.F_1 = F_1;
                }
                
                public Inners.D D_1 { get; }
                public Inners.F F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class EZERO : percentxZEROZEROⲻFF
            {
                public EZERO(Inners.E E_1, Inners.ZERO ZERO_1)
                {
                    this.E_1 = E_1;
                    this.ZERO_1 = ZERO_1;
                }
                
                public Inners.E E_1 { get; }
                public Inners.ZERO ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class EONE : percentxZEROZEROⲻFF
            {
                public EONE(Inners.E E_1, Inners.ONE ONE_1)
                {
                    this.E_1 = E_1;
                    this.ONE_1 = ONE_1;
                }
                
                public Inners.E E_1 { get; }
                public Inners.ONE ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ETWO : percentxZEROZEROⲻFF
            {
                public ETWO(Inners.E E_1, Inners.TWO TWO_1)
                {
                    this.E_1 = E_1;
                    this.TWO_1 = TWO_1;
                }
                
                public Inners.E E_1 { get; }
                public Inners.TWO TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ETHREE : percentxZEROZEROⲻFF
            {
                public ETHREE(Inners.E E_1, Inners.THREE THREE_1)
                {
                    this.E_1 = E_1;
                    this.THREE_1 = THREE_1;
                }
                
                public Inners.E E_1 { get; }
                public Inners.THREE THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class EFOUR : percentxZEROZEROⲻFF
            {
                public EFOUR(Inners.E E_1, Inners.FOUR FOUR_1)
                {
                    this.E_1 = E_1;
                    this.FOUR_1 = FOUR_1;
                }
                
                public Inners.E E_1 { get; }
                public Inners.FOUR FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class EFIVE : percentxZEROZEROⲻFF
            {
                public EFIVE(Inners.E E_1, Inners.FIVE FIVE_1)
                {
                    this.E_1 = E_1;
                    this.FIVE_1 = FIVE_1;
                }
                
                public Inners.E E_1 { get; }
                public Inners.FIVE FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ESIX : percentxZEROZEROⲻFF
            {
                public ESIX(Inners.E E_1, Inners.SIX SIX_1)
                {
                    this.E_1 = E_1;
                    this.SIX_1 = SIX_1;
                }
                
                public Inners.E E_1 { get; }
                public Inners.SIX SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ESEVEN : percentxZEROZEROⲻFF
            {
                public ESEVEN(Inners.E E_1, Inners.SEVEN SEVEN_1)
                {
                    this.E_1 = E_1;
                    this.SEVEN_1 = SEVEN_1;
                }
                
                public Inners.E E_1 { get; }
                public Inners.SEVEN SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class EEIGHT : percentxZEROZEROⲻFF
            {
                public EEIGHT(Inners.E E_1, Inners.EIGHT EIGHT_1)
                {
                    this.E_1 = E_1;
                    this.EIGHT_1 = EIGHT_1;
                }
                
                public Inners.E E_1 { get; }
                public Inners.EIGHT EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ENINE : percentxZEROZEROⲻFF
            {
                public ENINE(Inners.E E_1, Inners.NINE NINE_1)
                {
                    this.E_1 = E_1;
                    this.NINE_1 = NINE_1;
                }
                
                public Inners.E E_1 { get; }
                public Inners.NINE NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class EA : percentxZEROZEROⲻFF
            {
                public EA(Inners.E E_1, Inners.A A_1)
                {
                    this.E_1 = E_1;
                    this.A_1 = A_1;
                }
                
                public Inners.E E_1 { get; }
                public Inners.A A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class EB : percentxZEROZEROⲻFF
            {
                public EB(Inners.E E_1, Inners.B B_1)
                {
                    this.E_1 = E_1;
                    this.B_1 = B_1;
                }
                
                public Inners.E E_1 { get; }
                public Inners.B B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class EC : percentxZEROZEROⲻFF
            {
                public EC(Inners.E E_1, Inners.C C_1)
                {
                    this.E_1 = E_1;
                    this.C_1 = C_1;
                }
                
                public Inners.E E_1 { get; }
                public Inners.C C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ED : percentxZEROZEROⲻFF
            {
                public ED(Inners.E E_1, Inners.D D_1)
                {
                    this.E_1 = E_1;
                    this.D_1 = D_1;
                }
                
                public Inners.E E_1 { get; }
                public Inners.D D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class EE : percentxZEROZEROⲻFF
            {
                public EE(Inners.E E_1, Inners.E E_2)
                {
                    this.E_1 = E_1;
                    this.E_2 = E_2;
                }
                
                public Inners.E E_1 { get; }
                public Inners.E E_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class EF : percentxZEROZEROⲻFF
            {
                public EF(Inners.E E_1, Inners.F F_1)
                {
                    this.E_1 = E_1;
                    this.F_1 = F_1;
                }
                
                public Inners.E E_1 { get; }
                public Inners.F F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FZERO : percentxZEROZEROⲻFF
            {
                public FZERO(Inners.F F_1, Inners.ZERO ZERO_1)
                {
                    this.F_1 = F_1;
                    this.ZERO_1 = ZERO_1;
                }
                
                public Inners.F F_1 { get; }
                public Inners.ZERO ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FONE : percentxZEROZEROⲻFF
            {
                public FONE(Inners.F F_1, Inners.ONE ONE_1)
                {
                    this.F_1 = F_1;
                    this.ONE_1 = ONE_1;
                }
                
                public Inners.F F_1 { get; }
                public Inners.ONE ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FTWO : percentxZEROZEROⲻFF
            {
                public FTWO(Inners.F F_1, Inners.TWO TWO_1)
                {
                    this.F_1 = F_1;
                    this.TWO_1 = TWO_1;
                }
                
                public Inners.F F_1 { get; }
                public Inners.TWO TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FTHREE : percentxZEROZEROⲻFF
            {
                public FTHREE(Inners.F F_1, Inners.THREE THREE_1)
                {
                    this.F_1 = F_1;
                    this.THREE_1 = THREE_1;
                }
                
                public Inners.F F_1 { get; }
                public Inners.THREE THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FFOUR : percentxZEROZEROⲻFF
            {
                public FFOUR(Inners.F F_1, Inners.FOUR FOUR_1)
                {
                    this.F_1 = F_1;
                    this.FOUR_1 = FOUR_1;
                }
                
                public Inners.F F_1 { get; }
                public Inners.FOUR FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FFIVE : percentxZEROZEROⲻFF
            {
                public FFIVE(Inners.F F_1, Inners.FIVE FIVE_1)
                {
                    this.F_1 = F_1;
                    this.FIVE_1 = FIVE_1;
                }
                
                public Inners.F F_1 { get; }
                public Inners.FIVE FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FSIX : percentxZEROZEROⲻFF
            {
                public FSIX(Inners.F F_1, Inners.SIX SIX_1)
                {
                    this.F_1 = F_1;
                    this.SIX_1 = SIX_1;
                }
                
                public Inners.F F_1 { get; }
                public Inners.SIX SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FSEVEN : percentxZEROZEROⲻFF
            {
                public FSEVEN(Inners.F F_1, Inners.SEVEN SEVEN_1)
                {
                    this.F_1 = F_1;
                    this.SEVEN_1 = SEVEN_1;
                }
                
                public Inners.F F_1 { get; }
                public Inners.SEVEN SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FEIGHT : percentxZEROZEROⲻFF
            {
                public FEIGHT(Inners.F F_1, Inners.EIGHT EIGHT_1)
                {
                    this.F_1 = F_1;
                    this.EIGHT_1 = EIGHT_1;
                }
                
                public Inners.F F_1 { get; }
                public Inners.EIGHT EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FNINE : percentxZEROZEROⲻFF
            {
                public FNINE(Inners.F F_1, Inners.NINE NINE_1)
                {
                    this.F_1 = F_1;
                    this.NINE_1 = NINE_1;
                }
                
                public Inners.F F_1 { get; }
                public Inners.NINE NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FA : percentxZEROZEROⲻFF
            {
                public FA(Inners.F F_1, Inners.A A_1)
                {
                    this.F_1 = F_1;
                    this.A_1 = A_1;
                }
                
                public Inners.F F_1 { get; }
                public Inners.A A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FB : percentxZEROZEROⲻFF
            {
                public FB(Inners.F F_1, Inners.B B_1)
                {
                    this.F_1 = F_1;
                    this.B_1 = B_1;
                }
                
                public Inners.F F_1 { get; }
                public Inners.B B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FC : percentxZEROZEROⲻFF
            {
                public FC(Inners.F F_1, Inners.C C_1)
                {
                    this.F_1 = F_1;
                    this.C_1 = C_1;
                }
                
                public Inners.F F_1 { get; }
                public Inners.C C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FD : percentxZEROZEROⲻFF
            {
                public FD(Inners.F F_1, Inners.D D_1)
                {
                    this.F_1 = F_1;
                    this.D_1 = D_1;
                }
                
                public Inners.F F_1 { get; }
                public Inners.D D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FE : percentxZEROZEROⲻFF
            {
                public FE(Inners.F F_1, Inners.E E_1)
                {
                    this.F_1 = F_1;
                    this.E_1 = E_1;
                }
                
                public Inners.F F_1 { get; }
                public Inners.E E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FF : percentxZEROZEROⲻFF
            {
                public FF(Inners.F F_1, Inners.F F_2)
                {
                    this.F_1 = F_1;
                    this.F_2 = F_2;
                }
                
                public Inners.F F_1 { get; }
                public Inners.F F_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
        }
        
        public sealed class percentxTWOZERO
        {
            public percentxTWOZERO(Inners.TWO TWO_1, Inners.ZERO ZERO_1)
            {
                this.TWO_1 = TWO_1;
                this.ZERO_1 = ZERO_1;
            }
            
            public Inners.TWO TWO_1 { get; }
            public Inners.ZERO ZERO_1 { get; }
        }
        
        public abstract class percentxTWOONEⲻSEVENE
        {
            private percentxTWOONEⲻSEVENE()
            {
            }
            
            protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
            
            public abstract class Visitor<TResult, TContext>
            {
                public TResult Visit(percentxTWOONEⲻSEVENE node, TContext context)
                {
                    return node.Dispatch(this, context);
                }
                
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.TWOONE node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.TWOTWO node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.TWOTHREE node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.TWOFOUR node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.TWOFIVE node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.TWOSIX node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.TWOSEVEN node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.TWOEIGHT node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.TWONINE node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.TWOA node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.TWOB node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.TWOC node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.TWOD node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.TWOE node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.TWOF node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.THREEZERO node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.THREEONE node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.THREETWO node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.THREETHREE node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.THREEFOUR node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.THREEFIVE node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.THREESIX node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.THREESEVEN node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.THREEEIGHT node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.THREENINE node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.THREEA node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.THREEB node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.THREEC node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.THREED node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.THREEE node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.THREEF node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.FOURZERO node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.FOURONE node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.FOURTWO node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.FOURTHREE node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.FOURFOUR node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.FOURFIVE node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.FOURSIX node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.FOURSEVEN node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.FOUREIGHT node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.FOURNINE node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.FOURA node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.FOURB node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.FOURC node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.FOURD node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.FOURE node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.FOURF node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.FIVEZERO node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.FIVEONE node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.FIVETWO node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.FIVETHREE node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.FIVEFOUR node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.FIVEFIVE node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.FIVESIX node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.FIVESEVEN node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.FIVEEIGHT node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.FIVENINE node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.FIVEA node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.FIVEB node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.FIVEC node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.FIVED node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.FIVEE node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.FIVEF node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.SIXZERO node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.SIXONE node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.SIXTWO node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.SIXTHREE node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.SIXFOUR node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.SIXFIVE node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.SIXSIX node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.SIXSEVEN node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.SIXEIGHT node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.SIXNINE node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.SIXA node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.SIXB node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.SIXC node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.SIXD node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.SIXE node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.SIXF node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.SEVENZERO node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.SEVENONE node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.SEVENTWO node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.SEVENTHREE node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.SEVENFOUR node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.SEVENFIVE node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.SEVENSIX node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.SEVENSEVEN node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.SEVENEIGHT node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.SEVENNINE node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.SEVENA node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.SEVENB node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.SEVENC node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.SEVEND node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.SEVENE node, TContext context);
            }
            
            public sealed class TWOONE : percentxTWOONEⲻSEVENE
            {
                public TWOONE(Inners.TWO TWO_1, Inners.ONE ONE_1)
                {
                    this.TWO_1 = TWO_1;
                    this.ONE_1 = ONE_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.ONE ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOTWO : percentxTWOONEⲻSEVENE
            {
                public TWOTWO(Inners.TWO TWO_1, Inners.TWO TWO_2)
                {
                    this.TWO_1 = TWO_1;
                    this.TWO_2 = TWO_2;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.TWO TWO_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOTHREE : percentxTWOONEⲻSEVENE
            {
                public TWOTHREE(Inners.TWO TWO_1, Inners.THREE THREE_1)
                {
                    this.TWO_1 = TWO_1;
                    this.THREE_1 = THREE_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.THREE THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOFOUR : percentxTWOONEⲻSEVENE
            {
                public TWOFOUR(Inners.TWO TWO_1, Inners.FOUR FOUR_1)
                {
                    this.TWO_1 = TWO_1;
                    this.FOUR_1 = FOUR_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.FOUR FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOFIVE : percentxTWOONEⲻSEVENE
            {
                public TWOFIVE(Inners.TWO TWO_1, Inners.FIVE FIVE_1)
                {
                    this.TWO_1 = TWO_1;
                    this.FIVE_1 = FIVE_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.FIVE FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOSIX : percentxTWOONEⲻSEVENE
            {
                public TWOSIX(Inners.TWO TWO_1, Inners.SIX SIX_1)
                {
                    this.TWO_1 = TWO_1;
                    this.SIX_1 = SIX_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.SIX SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOSEVEN : percentxTWOONEⲻSEVENE
            {
                public TWOSEVEN(Inners.TWO TWO_1, Inners.SEVEN SEVEN_1)
                {
                    this.TWO_1 = TWO_1;
                    this.SEVEN_1 = SEVEN_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.SEVEN SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOEIGHT : percentxTWOONEⲻSEVENE
            {
                public TWOEIGHT(Inners.TWO TWO_1, Inners.EIGHT EIGHT_1)
                {
                    this.TWO_1 = TWO_1;
                    this.EIGHT_1 = EIGHT_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.EIGHT EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWONINE : percentxTWOONEⲻSEVENE
            {
                public TWONINE(Inners.TWO TWO_1, Inners.NINE NINE_1)
                {
                    this.TWO_1 = TWO_1;
                    this.NINE_1 = NINE_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.NINE NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOA : percentxTWOONEⲻSEVENE
            {
                public TWOA(Inners.TWO TWO_1, Inners.A A_1)
                {
                    this.TWO_1 = TWO_1;
                    this.A_1 = A_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.A A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOB : percentxTWOONEⲻSEVENE
            {
                public TWOB(Inners.TWO TWO_1, Inners.B B_1)
                {
                    this.TWO_1 = TWO_1;
                    this.B_1 = B_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.B B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOC : percentxTWOONEⲻSEVENE
            {
                public TWOC(Inners.TWO TWO_1, Inners.C C_1)
                {
                    this.TWO_1 = TWO_1;
                    this.C_1 = C_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.C C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOD : percentxTWOONEⲻSEVENE
            {
                public TWOD(Inners.TWO TWO_1, Inners.D D_1)
                {
                    this.TWO_1 = TWO_1;
                    this.D_1 = D_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.D D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOE : percentxTWOONEⲻSEVENE
            {
                public TWOE(Inners.TWO TWO_1, Inners.E E_1)
                {
                    this.TWO_1 = TWO_1;
                    this.E_1 = E_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.E E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOF : percentxTWOONEⲻSEVENE
            {
                public TWOF(Inners.TWO TWO_1, Inners.F F_1)
                {
                    this.TWO_1 = TWO_1;
                    this.F_1 = F_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.F F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREEZERO : percentxTWOONEⲻSEVENE
            {
                public THREEZERO(Inners.THREE THREE_1, Inners.ZERO ZERO_1)
                {
                    this.THREE_1 = THREE_1;
                    this.ZERO_1 = ZERO_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.ZERO ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREEONE : percentxTWOONEⲻSEVENE
            {
                public THREEONE(Inners.THREE THREE_1, Inners.ONE ONE_1)
                {
                    this.THREE_1 = THREE_1;
                    this.ONE_1 = ONE_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.ONE ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREETWO : percentxTWOONEⲻSEVENE
            {
                public THREETWO(Inners.THREE THREE_1, Inners.TWO TWO_1)
                {
                    this.THREE_1 = THREE_1;
                    this.TWO_1 = TWO_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.TWO TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREETHREE : percentxTWOONEⲻSEVENE
            {
                public THREETHREE(Inners.THREE THREE_1, Inners.THREE THREE_2)
                {
                    this.THREE_1 = THREE_1;
                    this.THREE_2 = THREE_2;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.THREE THREE_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREEFOUR : percentxTWOONEⲻSEVENE
            {
                public THREEFOUR(Inners.THREE THREE_1, Inners.FOUR FOUR_1)
                {
                    this.THREE_1 = THREE_1;
                    this.FOUR_1 = FOUR_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.FOUR FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREEFIVE : percentxTWOONEⲻSEVENE
            {
                public THREEFIVE(Inners.THREE THREE_1, Inners.FIVE FIVE_1)
                {
                    this.THREE_1 = THREE_1;
                    this.FIVE_1 = FIVE_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.FIVE FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREESIX : percentxTWOONEⲻSEVENE
            {
                public THREESIX(Inners.THREE THREE_1, Inners.SIX SIX_1)
                {
                    this.THREE_1 = THREE_1;
                    this.SIX_1 = SIX_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.SIX SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREESEVEN : percentxTWOONEⲻSEVENE
            {
                public THREESEVEN(Inners.THREE THREE_1, Inners.SEVEN SEVEN_1)
                {
                    this.THREE_1 = THREE_1;
                    this.SEVEN_1 = SEVEN_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.SEVEN SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREEEIGHT : percentxTWOONEⲻSEVENE
            {
                public THREEEIGHT(Inners.THREE THREE_1, Inners.EIGHT EIGHT_1)
                {
                    this.THREE_1 = THREE_1;
                    this.EIGHT_1 = EIGHT_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.EIGHT EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREENINE : percentxTWOONEⲻSEVENE
            {
                public THREENINE(Inners.THREE THREE_1, Inners.NINE NINE_1)
                {
                    this.THREE_1 = THREE_1;
                    this.NINE_1 = NINE_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.NINE NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREEA : percentxTWOONEⲻSEVENE
            {
                public THREEA(Inners.THREE THREE_1, Inners.A A_1)
                {
                    this.THREE_1 = THREE_1;
                    this.A_1 = A_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.A A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREEB : percentxTWOONEⲻSEVENE
            {
                public THREEB(Inners.THREE THREE_1, Inners.B B_1)
                {
                    this.THREE_1 = THREE_1;
                    this.B_1 = B_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.B B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREEC : percentxTWOONEⲻSEVENE
            {
                public THREEC(Inners.THREE THREE_1, Inners.C C_1)
                {
                    this.THREE_1 = THREE_1;
                    this.C_1 = C_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.C C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREED : percentxTWOONEⲻSEVENE
            {
                public THREED(Inners.THREE THREE_1, Inners.D D_1)
                {
                    this.THREE_1 = THREE_1;
                    this.D_1 = D_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.D D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREEE : percentxTWOONEⲻSEVENE
            {
                public THREEE(Inners.THREE THREE_1, Inners.E E_1)
                {
                    this.THREE_1 = THREE_1;
                    this.E_1 = E_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.E E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREEF : percentxTWOONEⲻSEVENE
            {
                public THREEF(Inners.THREE THREE_1, Inners.F F_1)
                {
                    this.THREE_1 = THREE_1;
                    this.F_1 = F_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.F F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURZERO : percentxTWOONEⲻSEVENE
            {
                public FOURZERO(Inners.FOUR FOUR_1, Inners.ZERO ZERO_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.ZERO_1 = ZERO_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.ZERO ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURONE : percentxTWOONEⲻSEVENE
            {
                public FOURONE(Inners.FOUR FOUR_1, Inners.ONE ONE_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.ONE_1 = ONE_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.ONE ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURTWO : percentxTWOONEⲻSEVENE
            {
                public FOURTWO(Inners.FOUR FOUR_1, Inners.TWO TWO_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.TWO_1 = TWO_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.TWO TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURTHREE : percentxTWOONEⲻSEVENE
            {
                public FOURTHREE(Inners.FOUR FOUR_1, Inners.THREE THREE_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.THREE_1 = THREE_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.THREE THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURFOUR : percentxTWOONEⲻSEVENE
            {
                public FOURFOUR(Inners.FOUR FOUR_1, Inners.FOUR FOUR_2)
                {
                    this.FOUR_1 = FOUR_1;
                    this.FOUR_2 = FOUR_2;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.FOUR FOUR_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURFIVE : percentxTWOONEⲻSEVENE
            {
                public FOURFIVE(Inners.FOUR FOUR_1, Inners.FIVE FIVE_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.FIVE_1 = FIVE_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.FIVE FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURSIX : percentxTWOONEⲻSEVENE
            {
                public FOURSIX(Inners.FOUR FOUR_1, Inners.SIX SIX_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.SIX_1 = SIX_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.SIX SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURSEVEN : percentxTWOONEⲻSEVENE
            {
                public FOURSEVEN(Inners.FOUR FOUR_1, Inners.SEVEN SEVEN_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.SEVEN_1 = SEVEN_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.SEVEN SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOUREIGHT : percentxTWOONEⲻSEVENE
            {
                public FOUREIGHT(Inners.FOUR FOUR_1, Inners.EIGHT EIGHT_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.EIGHT_1 = EIGHT_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.EIGHT EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURNINE : percentxTWOONEⲻSEVENE
            {
                public FOURNINE(Inners.FOUR FOUR_1, Inners.NINE NINE_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.NINE_1 = NINE_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.NINE NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURA : percentxTWOONEⲻSEVENE
            {
                public FOURA(Inners.FOUR FOUR_1, Inners.A A_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.A_1 = A_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.A A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURB : percentxTWOONEⲻSEVENE
            {
                public FOURB(Inners.FOUR FOUR_1, Inners.B B_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.B_1 = B_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.B B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURC : percentxTWOONEⲻSEVENE
            {
                public FOURC(Inners.FOUR FOUR_1, Inners.C C_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.C_1 = C_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.C C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURD : percentxTWOONEⲻSEVENE
            {
                public FOURD(Inners.FOUR FOUR_1, Inners.D D_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.D_1 = D_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.D D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURE : percentxTWOONEⲻSEVENE
            {
                public FOURE(Inners.FOUR FOUR_1, Inners.E E_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.E_1 = E_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.E E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURF : percentxTWOONEⲻSEVENE
            {
                public FOURF(Inners.FOUR FOUR_1, Inners.F F_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.F_1 = F_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.F F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVEZERO : percentxTWOONEⲻSEVENE
            {
                public FIVEZERO(Inners.FIVE FIVE_1, Inners.ZERO ZERO_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.ZERO_1 = ZERO_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.ZERO ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVEONE : percentxTWOONEⲻSEVENE
            {
                public FIVEONE(Inners.FIVE FIVE_1, Inners.ONE ONE_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.ONE_1 = ONE_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.ONE ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVETWO : percentxTWOONEⲻSEVENE
            {
                public FIVETWO(Inners.FIVE FIVE_1, Inners.TWO TWO_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.TWO_1 = TWO_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.TWO TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVETHREE : percentxTWOONEⲻSEVENE
            {
                public FIVETHREE(Inners.FIVE FIVE_1, Inners.THREE THREE_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.THREE_1 = THREE_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.THREE THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVEFOUR : percentxTWOONEⲻSEVENE
            {
                public FIVEFOUR(Inners.FIVE FIVE_1, Inners.FOUR FOUR_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.FOUR_1 = FOUR_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.FOUR FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVEFIVE : percentxTWOONEⲻSEVENE
            {
                public FIVEFIVE(Inners.FIVE FIVE_1, Inners.FIVE FIVE_2)
                {
                    this.FIVE_1 = FIVE_1;
                    this.FIVE_2 = FIVE_2;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.FIVE FIVE_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVESIX : percentxTWOONEⲻSEVENE
            {
                public FIVESIX(Inners.FIVE FIVE_1, Inners.SIX SIX_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.SIX_1 = SIX_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.SIX SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVESEVEN : percentxTWOONEⲻSEVENE
            {
                public FIVESEVEN(Inners.FIVE FIVE_1, Inners.SEVEN SEVEN_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.SEVEN_1 = SEVEN_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.SEVEN SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVEEIGHT : percentxTWOONEⲻSEVENE
            {
                public FIVEEIGHT(Inners.FIVE FIVE_1, Inners.EIGHT EIGHT_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.EIGHT_1 = EIGHT_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.EIGHT EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVENINE : percentxTWOONEⲻSEVENE
            {
                public FIVENINE(Inners.FIVE FIVE_1, Inners.NINE NINE_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.NINE_1 = NINE_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.NINE NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVEA : percentxTWOONEⲻSEVENE
            {
                public FIVEA(Inners.FIVE FIVE_1, Inners.A A_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.A_1 = A_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.A A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVEB : percentxTWOONEⲻSEVENE
            {
                public FIVEB(Inners.FIVE FIVE_1, Inners.B B_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.B_1 = B_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.B B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVEC : percentxTWOONEⲻSEVENE
            {
                public FIVEC(Inners.FIVE FIVE_1, Inners.C C_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.C_1 = C_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.C C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVED : percentxTWOONEⲻSEVENE
            {
                public FIVED(Inners.FIVE FIVE_1, Inners.D D_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.D_1 = D_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.D D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVEE : percentxTWOONEⲻSEVENE
            {
                public FIVEE(Inners.FIVE FIVE_1, Inners.E E_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.E_1 = E_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.E E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVEF : percentxTWOONEⲻSEVENE
            {
                public FIVEF(Inners.FIVE FIVE_1, Inners.F F_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.F_1 = F_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.F F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXZERO : percentxTWOONEⲻSEVENE
            {
                public SIXZERO(Inners.SIX SIX_1, Inners.ZERO ZERO_1)
                {
                    this.SIX_1 = SIX_1;
                    this.ZERO_1 = ZERO_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.ZERO ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXONE : percentxTWOONEⲻSEVENE
            {
                public SIXONE(Inners.SIX SIX_1, Inners.ONE ONE_1)
                {
                    this.SIX_1 = SIX_1;
                    this.ONE_1 = ONE_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.ONE ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXTWO : percentxTWOONEⲻSEVENE
            {
                public SIXTWO(Inners.SIX SIX_1, Inners.TWO TWO_1)
                {
                    this.SIX_1 = SIX_1;
                    this.TWO_1 = TWO_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.TWO TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXTHREE : percentxTWOONEⲻSEVENE
            {
                public SIXTHREE(Inners.SIX SIX_1, Inners.THREE THREE_1)
                {
                    this.SIX_1 = SIX_1;
                    this.THREE_1 = THREE_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.THREE THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXFOUR : percentxTWOONEⲻSEVENE
            {
                public SIXFOUR(Inners.SIX SIX_1, Inners.FOUR FOUR_1)
                {
                    this.SIX_1 = SIX_1;
                    this.FOUR_1 = FOUR_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.FOUR FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXFIVE : percentxTWOONEⲻSEVENE
            {
                public SIXFIVE(Inners.SIX SIX_1, Inners.FIVE FIVE_1)
                {
                    this.SIX_1 = SIX_1;
                    this.FIVE_1 = FIVE_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.FIVE FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXSIX : percentxTWOONEⲻSEVENE
            {
                public SIXSIX(Inners.SIX SIX_1, Inners.SIX SIX_2)
                {
                    this.SIX_1 = SIX_1;
                    this.SIX_2 = SIX_2;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.SIX SIX_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXSEVEN : percentxTWOONEⲻSEVENE
            {
                public SIXSEVEN(Inners.SIX SIX_1, Inners.SEVEN SEVEN_1)
                {
                    this.SIX_1 = SIX_1;
                    this.SEVEN_1 = SEVEN_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.SEVEN SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXEIGHT : percentxTWOONEⲻSEVENE
            {
                public SIXEIGHT(Inners.SIX SIX_1, Inners.EIGHT EIGHT_1)
                {
                    this.SIX_1 = SIX_1;
                    this.EIGHT_1 = EIGHT_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.EIGHT EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXNINE : percentxTWOONEⲻSEVENE
            {
                public SIXNINE(Inners.SIX SIX_1, Inners.NINE NINE_1)
                {
                    this.SIX_1 = SIX_1;
                    this.NINE_1 = NINE_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.NINE NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXA : percentxTWOONEⲻSEVENE
            {
                public SIXA(Inners.SIX SIX_1, Inners.A A_1)
                {
                    this.SIX_1 = SIX_1;
                    this.A_1 = A_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.A A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXB : percentxTWOONEⲻSEVENE
            {
                public SIXB(Inners.SIX SIX_1, Inners.B B_1)
                {
                    this.SIX_1 = SIX_1;
                    this.B_1 = B_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.B B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXC : percentxTWOONEⲻSEVENE
            {
                public SIXC(Inners.SIX SIX_1, Inners.C C_1)
                {
                    this.SIX_1 = SIX_1;
                    this.C_1 = C_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.C C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXD : percentxTWOONEⲻSEVENE
            {
                public SIXD(Inners.SIX SIX_1, Inners.D D_1)
                {
                    this.SIX_1 = SIX_1;
                    this.D_1 = D_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.D D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXE : percentxTWOONEⲻSEVENE
            {
                public SIXE(Inners.SIX SIX_1, Inners.E E_1)
                {
                    this.SIX_1 = SIX_1;
                    this.E_1 = E_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.E E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXF : percentxTWOONEⲻSEVENE
            {
                public SIXF(Inners.SIX SIX_1, Inners.F F_1)
                {
                    this.SIX_1 = SIX_1;
                    this.F_1 = F_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.F F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENZERO : percentxTWOONEⲻSEVENE
            {
                public SEVENZERO(Inners.SEVEN SEVEN_1, Inners.ZERO ZERO_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.ZERO_1 = ZERO_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.ZERO ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENONE : percentxTWOONEⲻSEVENE
            {
                public SEVENONE(Inners.SEVEN SEVEN_1, Inners.ONE ONE_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.ONE_1 = ONE_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.ONE ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENTWO : percentxTWOONEⲻSEVENE
            {
                public SEVENTWO(Inners.SEVEN SEVEN_1, Inners.TWO TWO_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.TWO_1 = TWO_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.TWO TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENTHREE : percentxTWOONEⲻSEVENE
            {
                public SEVENTHREE(Inners.SEVEN SEVEN_1, Inners.THREE THREE_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.THREE_1 = THREE_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.THREE THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENFOUR : percentxTWOONEⲻSEVENE
            {
                public SEVENFOUR(Inners.SEVEN SEVEN_1, Inners.FOUR FOUR_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.FOUR_1 = FOUR_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.FOUR FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENFIVE : percentxTWOONEⲻSEVENE
            {
                public SEVENFIVE(Inners.SEVEN SEVEN_1, Inners.FIVE FIVE_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.FIVE_1 = FIVE_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.FIVE FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENSIX : percentxTWOONEⲻSEVENE
            {
                public SEVENSIX(Inners.SEVEN SEVEN_1, Inners.SIX SIX_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.SIX_1 = SIX_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.SIX SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENSEVEN : percentxTWOONEⲻSEVENE
            {
                public SEVENSEVEN(Inners.SEVEN SEVEN_1, Inners.SEVEN SEVEN_2)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.SEVEN_2 = SEVEN_2;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.SEVEN SEVEN_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENEIGHT : percentxTWOONEⲻSEVENE
            {
                public SEVENEIGHT(Inners.SEVEN SEVEN_1, Inners.EIGHT EIGHT_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.EIGHT_1 = EIGHT_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.EIGHT EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENNINE : percentxTWOONEⲻSEVENE
            {
                public SEVENNINE(Inners.SEVEN SEVEN_1, Inners.NINE NINE_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.NINE_1 = NINE_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.NINE NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENA : percentxTWOONEⲻSEVENE
            {
                public SEVENA(Inners.SEVEN SEVEN_1, Inners.A A_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.A_1 = A_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.A A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENB : percentxTWOONEⲻSEVENE
            {
                public SEVENB(Inners.SEVEN SEVEN_1, Inners.B B_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.B_1 = B_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.B B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENC : percentxTWOONEⲻSEVENE
            {
                public SEVENC(Inners.SEVEN SEVEN_1, Inners.C C_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.C_1 = C_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.C C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVEND : percentxTWOONEⲻSEVENE
            {
                public SEVEND(Inners.SEVEN SEVEN_1, Inners.D D_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.D_1 = D_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.D D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENE : percentxTWOONEⲻSEVENE
            {
                public SEVENE(Inners.SEVEN SEVEN_1, Inners.E E_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.E_1 = E_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.E E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
        }
    }
    
}
