namespace GeneratorV3.Core
{
    using System.Collections.Generic;
    
    public abstract class _ALPHA
    {
        private _ALPHA()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_ALPHA node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_ALPHA._percentxFOURONEⲻFIVEA node, TContext context);
            protected internal abstract TResult Accept(_ALPHA._percentxSIXONEⲻSEVENA node, TContext context);
        }
        
        public sealed class _percentxFOURONEⲻFIVEA : _ALPHA
        {
            public _percentxFOURONEⲻFIVEA(Inners._percentxFOURONEⲻFIVEA _percentxFOURONEⲻFIVEA_1)
            {
                this._percentxFOURONEⲻFIVEA_1 = _percentxFOURONEⲻFIVEA_1;
            }
            
            public Inners._percentxFOURONEⲻFIVEA _percentxFOURONEⲻFIVEA_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _percentxSIXONEⲻSEVENA : _ALPHA
        {
            public _percentxSIXONEⲻSEVENA(Inners._percentxSIXONEⲻSEVENA _percentxSIXONEⲻSEVENA_1)
            {
                this._percentxSIXONEⲻSEVENA_1 = _percentxSIXONEⲻSEVENA_1;
            }
            
            public Inners._percentxSIXONEⲻSEVENA _percentxSIXONEⲻSEVENA_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
    public abstract class _BIT
    {
        private _BIT()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_BIT node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_BIT._doublequotex30doublequote node, TContext context);
            protected internal abstract TResult Accept(_BIT._doublequotex31doublequote node, TContext context);
        }
        
        public sealed class _doublequotex30doublequote : _BIT
        {
            public _doublequotex30doublequote(Inners._doublequotex30doublequote _doublequotex30doublequote_1)
            {
                this._doublequotex30doublequote_1 = _doublequotex30doublequote_1;
            }
            
            public Inners._doublequotex30doublequote _doublequotex30doublequote_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _doublequotex31doublequote : _BIT
        {
            public _doublequotex31doublequote(Inners._doublequotex31doublequote _doublequotex31doublequote_1)
            {
                this._doublequotex31doublequote_1 = _doublequotex31doublequote_1;
            }
            
            public Inners._doublequotex31doublequote _doublequotex31doublequote_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
    public sealed class _CHAR
    {
        public _CHAR(Inners._percentxZEROONEⲻSEVENF _percentxZEROONEⲻSEVENF_1)
        {
            this._percentxZEROONEⲻSEVENF_1 = _percentxZEROONEⲻSEVENF_1;
        }
        
        public Inners._percentxZEROONEⲻSEVENF _percentxZEROONEⲻSEVENF_1 { get; }
    }
    
    public sealed class _CR
    {
        public _CR(Inners._percentxZEROD _percentxZEROD_1)
        {
            this._percentxZEROD_1 = _percentxZEROD_1;
        }
        
        public Inners._percentxZEROD _percentxZEROD_1 { get; }
    }
    
    public sealed class _CRLF
    {
        public _CRLF(GeneratorV3.Core._CR _CR_1, GeneratorV3.Core._LF _LF_1)
        {
            this._CR_1 = _CR_1;
            this._LF_1 = _LF_1;
        }
        
        public GeneratorV3.Core._CR _CR_1 { get; }
        public GeneratorV3.Core._LF _LF_1 { get; }
    }
    
    public abstract class _CTL
    {
        private _CTL()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_CTL node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_CTL._percentxZEROZEROⲻONEF node, TContext context);
            protected internal abstract TResult Accept(_CTL._percentxSEVENF node, TContext context);
        }
        
        public sealed class _percentxZEROZEROⲻONEF : _CTL
        {
            public _percentxZEROZEROⲻONEF(Inners._percentxZEROZEROⲻONEF _percentxZEROZEROⲻONEF_1)
            {
                this._percentxZEROZEROⲻONEF_1 = _percentxZEROZEROⲻONEF_1;
            }
            
            public Inners._percentxZEROZEROⲻONEF _percentxZEROZEROⲻONEF_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _percentxSEVENF : _CTL
        {
            public _percentxSEVENF(Inners._percentxSEVENF _percentxSEVENF_1)
            {
                this._percentxSEVENF_1 = _percentxSEVENF_1;
            }
            
            public Inners._percentxSEVENF _percentxSEVENF_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
    public sealed class _DIGIT
    {
        public _DIGIT(Inners._percentxTHREEZEROⲻTHREENINE _percentxTHREEZEROⲻTHREENINE_1)
        {
            this._percentxTHREEZEROⲻTHREENINE_1 = _percentxTHREEZEROⲻTHREENINE_1;
        }
        
        public Inners._percentxTHREEZEROⲻTHREENINE _percentxTHREEZEROⲻTHREENINE_1 { get; }
    }
    
    public sealed class _DQUOTE
    {
        public _DQUOTE(Inners._percentxTWOTWO _percentxTWOTWO_1)
        {
            this._percentxTWOTWO_1 = _percentxTWOTWO_1;
        }
        
        public Inners._percentxTWOTWO _percentxTWOTWO_1 { get; }
    }
    
    public abstract class _HEXDIG
    {
        private _HEXDIG()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_HEXDIG node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_HEXDIG._DIGIT node, TContext context);
            protected internal abstract TResult Accept(_HEXDIG._doublequotex41doublequote node, TContext context);
            protected internal abstract TResult Accept(_HEXDIG._doublequotex42doublequote node, TContext context);
            protected internal abstract TResult Accept(_HEXDIG._doublequotex43doublequote node, TContext context);
            protected internal abstract TResult Accept(_HEXDIG._doublequotex44doublequote node, TContext context);
            protected internal abstract TResult Accept(_HEXDIG._doublequotex45doublequote node, TContext context);
            protected internal abstract TResult Accept(_HEXDIG._doublequotex46doublequote node, TContext context);
        }
        
        public sealed class _DIGIT : _HEXDIG
        {
            public _DIGIT(GeneratorV3.Core._DIGIT _DIGIT_1)
            {
                this._DIGIT_1 = _DIGIT_1;
            }
            
            public GeneratorV3.Core._DIGIT _DIGIT_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _doublequotex41doublequote : _HEXDIG
        {
            public _doublequotex41doublequote(Inners._doublequotex41doublequote _doublequotex41doublequote_1)
            {
                this._doublequotex41doublequote_1 = _doublequotex41doublequote_1;
            }
            
            public Inners._doublequotex41doublequote _doublequotex41doublequote_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _doublequotex42doublequote : _HEXDIG
        {
            public _doublequotex42doublequote(Inners._doublequotex42doublequote _doublequotex42doublequote_1)
            {
                this._doublequotex42doublequote_1 = _doublequotex42doublequote_1;
            }
            
            public Inners._doublequotex42doublequote _doublequotex42doublequote_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _doublequotex43doublequote : _HEXDIG
        {
            public _doublequotex43doublequote(Inners._doublequotex43doublequote _doublequotex43doublequote_1)
            {
                this._doublequotex43doublequote_1 = _doublequotex43doublequote_1;
            }
            
            public Inners._doublequotex43doublequote _doublequotex43doublequote_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _doublequotex44doublequote : _HEXDIG
        {
            public _doublequotex44doublequote(Inners._doublequotex44doublequote _doublequotex44doublequote_1)
            {
                this._doublequotex44doublequote_1 = _doublequotex44doublequote_1;
            }
            
            public Inners._doublequotex44doublequote _doublequotex44doublequote_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _doublequotex45doublequote : _HEXDIG
        {
            public _doublequotex45doublequote(Inners._doublequotex45doublequote _doublequotex45doublequote_1)
            {
                this._doublequotex45doublequote_1 = _doublequotex45doublequote_1;
            }
            
            public Inners._doublequotex45doublequote _doublequotex45doublequote_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _doublequotex46doublequote : _HEXDIG
        {
            public _doublequotex46doublequote(Inners._doublequotex46doublequote _doublequotex46doublequote_1)
            {
                this._doublequotex46doublequote_1 = _doublequotex46doublequote_1;
            }
            
            public Inners._doublequotex46doublequote _doublequotex46doublequote_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
    public sealed class _HTAB
    {
        public _HTAB(Inners._percentxZERONINE _percentxZERONINE_1)
        {
            this._percentxZERONINE_1 = _percentxZERONINE_1;
        }
        
        public Inners._percentxZERONINE _percentxZERONINE_1 { get; }
    }
    
    public sealed class _LF
    {
        public _LF(Inners._percentxZEROA _percentxZEROA_1)
        {
            this._percentxZEROA_1 = _percentxZEROA_1;
        }
        
        public Inners._percentxZEROA _percentxZEROA_1 { get; }
    }
    
    public sealed class _LWSP
    {
        public _LWSP(IEnumerable<Inners._openWSPⳆCRLF_WSPↃ> _openWSPⳆCRLF_WSPↃ_1)
        {
            this._openWSPⳆCRLF_WSPↃ_1 = _openWSPⳆCRLF_WSPↃ_1;
        }
        
        public IEnumerable<Inners._openWSPⳆCRLF_WSPↃ> _openWSPⳆCRLF_WSPↃ_1 { get; }
    }
    
    public sealed class _OCTET
    {
        public _OCTET(Inners._percentxZEROZEROⲻFF _percentxZEROZEROⲻFF_1)
        {
            this._percentxZEROZEROⲻFF_1 = _percentxZEROZEROⲻFF_1;
        }
        
        public Inners._percentxZEROZEROⲻFF _percentxZEROZEROⲻFF_1 { get; }
    }
    
    public sealed class _SP
    {
        public _SP(Inners._percentxTWOZERO _percentxTWOZERO_1)
        {
            this._percentxTWOZERO_1 = _percentxTWOZERO_1;
        }
        
        public Inners._percentxTWOZERO _percentxTWOZERO_1 { get; }
    }
    
    public sealed class _VCHAR
    {
        public _VCHAR(Inners._percentxTWOONEⲻSEVENE _percentxTWOONEⲻSEVENE_1)
        {
            this._percentxTWOONEⲻSEVENE_1 = _percentxTWOONEⲻSEVENE_1;
        }
        
        public Inners._percentxTWOONEⲻSEVENE _percentxTWOONEⲻSEVENE_1 { get; }
    }
    
    public abstract class _WSP
    {
        private _WSP()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_WSP node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_WSP._SP node, TContext context);
            protected internal abstract TResult Accept(_WSP._HTAB node, TContext context);
        }
        
        public sealed class _SP : _WSP
        {
            public _SP(GeneratorV3.Core._SP _SP_1)
            {
                this._SP_1 = _SP_1;
            }
            
            public GeneratorV3.Core._SP _SP_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _HTAB : _WSP
        {
            public _HTAB(GeneratorV3.Core._HTAB _HTAB_1)
            {
                this._HTAB_1 = _HTAB_1;
            }
            
            public GeneratorV3.Core._HTAB _HTAB_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
    public static class Inners
    {
        public sealed class _FOUR
        {
            private _FOUR()
            {
            }
            
            public static _FOUR Instance { get; } = new _FOUR();
        }
        
        public sealed class _ONE
        {
            private _ONE()
            {
            }
            
            public static _ONE Instance { get; } = new _ONE();
        }
        
        public sealed class _TWO
        {
            private _TWO()
            {
            }
            
            public static _TWO Instance { get; } = new _TWO();
        }
        
        public sealed class _THREE
        {
            private _THREE()
            {
            }
            
            public static _THREE Instance { get; } = new _THREE();
        }
        
        public sealed class _FIVE
        {
            private _FIVE()
            {
            }
            
            public static _FIVE Instance { get; } = new _FIVE();
        }
        
        public sealed class _SIX
        {
            private _SIX()
            {
            }
            
            public static _SIX Instance { get; } = new _SIX();
        }
        
        public sealed class _SEVEN
        {
            private _SEVEN()
            {
            }
            
            public static _SEVEN Instance { get; } = new _SEVEN();
        }
        
        public sealed class _EIGHT
        {
            private _EIGHT()
            {
            }
            
            public static _EIGHT Instance { get; } = new _EIGHT();
        }
        
        public sealed class _NINE
        {
            private _NINE()
            {
            }
            
            public static _NINE Instance { get; } = new _NINE();
        }
        
        public sealed class _A
        {
            private _A()
            {
            }
            
            public static _A Instance { get; } = new _A();
        }
        
        public sealed class _B
        {
            private _B()
            {
            }
            
            public static _B Instance { get; } = new _B();
        }
        
        public sealed class _C
        {
            private _C()
            {
            }
            
            public static _C Instance { get; } = new _C();
        }
        
        public sealed class _D
        {
            private _D()
            {
            }
            
            public static _D Instance { get; } = new _D();
        }
        
        public sealed class _E
        {
            private _E()
            {
            }
            
            public static _E Instance { get; } = new _E();
        }
        
        public sealed class _F
        {
            private _F()
            {
            }
            
            public static _F Instance { get; } = new _F();
        }
        
        public sealed class _ZERO
        {
            private _ZERO()
            {
            }
            
            public static _ZERO Instance { get; } = new _ZERO();
        }
        
        public abstract class _percentxFOURONEⲻFIVEA
        {
            private _percentxFOURONEⲻFIVEA()
            {
            }
            
            protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
            
            public abstract class Visitor<TResult, TContext>
            {
                public TResult Visit(_percentxFOURONEⲻFIVEA node, TContext context)
                {
                    return node.Dispatch(this, context);
                }
                
                protected internal abstract TResult Accept(_percentxFOURONEⲻFIVEA._FOURONE node, TContext context);
                protected internal abstract TResult Accept(_percentxFOURONEⲻFIVEA._FOURTWO node, TContext context);
                protected internal abstract TResult Accept(_percentxFOURONEⲻFIVEA._FOURTHREE node, TContext context);
                protected internal abstract TResult Accept(_percentxFOURONEⲻFIVEA._FOURFOUR node, TContext context);
                protected internal abstract TResult Accept(_percentxFOURONEⲻFIVEA._FOURFIVE node, TContext context);
                protected internal abstract TResult Accept(_percentxFOURONEⲻFIVEA._FOURSIX node, TContext context);
                protected internal abstract TResult Accept(_percentxFOURONEⲻFIVEA._FOURSEVEN node, TContext context);
                protected internal abstract TResult Accept(_percentxFOURONEⲻFIVEA._FOUREIGHT node, TContext context);
                protected internal abstract TResult Accept(_percentxFOURONEⲻFIVEA._FOURNINE node, TContext context);
                protected internal abstract TResult Accept(_percentxFOURONEⲻFIVEA._FOURA node, TContext context);
                protected internal abstract TResult Accept(_percentxFOURONEⲻFIVEA._FOURB node, TContext context);
                protected internal abstract TResult Accept(_percentxFOURONEⲻFIVEA._FOURC node, TContext context);
                protected internal abstract TResult Accept(_percentxFOURONEⲻFIVEA._FOURD node, TContext context);
                protected internal abstract TResult Accept(_percentxFOURONEⲻFIVEA._FOURE node, TContext context);
                protected internal abstract TResult Accept(_percentxFOURONEⲻFIVEA._FOURF node, TContext context);
                protected internal abstract TResult Accept(_percentxFOURONEⲻFIVEA._FIVEZERO node, TContext context);
                protected internal abstract TResult Accept(_percentxFOURONEⲻFIVEA._FIVEONE node, TContext context);
                protected internal abstract TResult Accept(_percentxFOURONEⲻFIVEA._FIVETWO node, TContext context);
                protected internal abstract TResult Accept(_percentxFOURONEⲻFIVEA._FIVETHREE node, TContext context);
                protected internal abstract TResult Accept(_percentxFOURONEⲻFIVEA._FIVEFOUR node, TContext context);
                protected internal abstract TResult Accept(_percentxFOURONEⲻFIVEA._FIVEFIVE node, TContext context);
                protected internal abstract TResult Accept(_percentxFOURONEⲻFIVEA._FIVESIX node, TContext context);
                protected internal abstract TResult Accept(_percentxFOURONEⲻFIVEA._FIVESEVEN node, TContext context);
                protected internal abstract TResult Accept(_percentxFOURONEⲻFIVEA._FIVEEIGHT node, TContext context);
                protected internal abstract TResult Accept(_percentxFOURONEⲻFIVEA._FIVENINE node, TContext context);
                protected internal abstract TResult Accept(_percentxFOURONEⲻFIVEA._FIVEA node, TContext context);
            }
            
            public sealed class _FOURONE : _percentxFOURONEⲻFIVEA
            {
                public _FOURONE(Inners._FOUR _FOUR_1, Inners._ONE _ONE_1)
                {
                    this._FOUR_1 = _FOUR_1;
                    this._ONE_1 = _ONE_1;
                }
                
                public Inners._FOUR _FOUR_1 { get; }
                public Inners._ONE _ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FOURTWO : _percentxFOURONEⲻFIVEA
            {
                public _FOURTWO(Inners._FOUR _FOUR_1, Inners._TWO _TWO_1)
                {
                    this._FOUR_1 = _FOUR_1;
                    this._TWO_1 = _TWO_1;
                }
                
                public Inners._FOUR _FOUR_1 { get; }
                public Inners._TWO _TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FOURTHREE : _percentxFOURONEⲻFIVEA
            {
                public _FOURTHREE(Inners._FOUR _FOUR_1, Inners._THREE _THREE_1)
                {
                    this._FOUR_1 = _FOUR_1;
                    this._THREE_1 = _THREE_1;
                }
                
                public Inners._FOUR _FOUR_1 { get; }
                public Inners._THREE _THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FOURFOUR : _percentxFOURONEⲻFIVEA
            {
                public _FOURFOUR(Inners._FOUR _FOUR_1, Inners._FOUR _FOUR_2)
                {
                    this._FOUR_1 = _FOUR_1;
                    this._FOUR_2 = _FOUR_2;
                }
                
                public Inners._FOUR _FOUR_1 { get; }
                public Inners._FOUR _FOUR_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FOURFIVE : _percentxFOURONEⲻFIVEA
            {
                public _FOURFIVE(Inners._FOUR _FOUR_1, Inners._FIVE _FIVE_1)
                {
                    this._FOUR_1 = _FOUR_1;
                    this._FIVE_1 = _FIVE_1;
                }
                
                public Inners._FOUR _FOUR_1 { get; }
                public Inners._FIVE _FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FOURSIX : _percentxFOURONEⲻFIVEA
            {
                public _FOURSIX(Inners._FOUR _FOUR_1, Inners._SIX _SIX_1)
                {
                    this._FOUR_1 = _FOUR_1;
                    this._SIX_1 = _SIX_1;
                }
                
                public Inners._FOUR _FOUR_1 { get; }
                public Inners._SIX _SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FOURSEVEN : _percentxFOURONEⲻFIVEA
            {
                public _FOURSEVEN(Inners._FOUR _FOUR_1, Inners._SEVEN _SEVEN_1)
                {
                    this._FOUR_1 = _FOUR_1;
                    this._SEVEN_1 = _SEVEN_1;
                }
                
                public Inners._FOUR _FOUR_1 { get; }
                public Inners._SEVEN _SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FOUREIGHT : _percentxFOURONEⲻFIVEA
            {
                public _FOUREIGHT(Inners._FOUR _FOUR_1, Inners._EIGHT _EIGHT_1)
                {
                    this._FOUR_1 = _FOUR_1;
                    this._EIGHT_1 = _EIGHT_1;
                }
                
                public Inners._FOUR _FOUR_1 { get; }
                public Inners._EIGHT _EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FOURNINE : _percentxFOURONEⲻFIVEA
            {
                public _FOURNINE(Inners._FOUR _FOUR_1, Inners._NINE _NINE_1)
                {
                    this._FOUR_1 = _FOUR_1;
                    this._NINE_1 = _NINE_1;
                }
                
                public Inners._FOUR _FOUR_1 { get; }
                public Inners._NINE _NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FOURA : _percentxFOURONEⲻFIVEA
            {
                public _FOURA(Inners._FOUR _FOUR_1, Inners._A _A_1)
                {
                    this._FOUR_1 = _FOUR_1;
                    this._A_1 = _A_1;
                }
                
                public Inners._FOUR _FOUR_1 { get; }
                public Inners._A _A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FOURB : _percentxFOURONEⲻFIVEA
            {
                public _FOURB(Inners._FOUR _FOUR_1, Inners._B _B_1)
                {
                    this._FOUR_1 = _FOUR_1;
                    this._B_1 = _B_1;
                }
                
                public Inners._FOUR _FOUR_1 { get; }
                public Inners._B _B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FOURC : _percentxFOURONEⲻFIVEA
            {
                public _FOURC(Inners._FOUR _FOUR_1, Inners._C _C_1)
                {
                    this._FOUR_1 = _FOUR_1;
                    this._C_1 = _C_1;
                }
                
                public Inners._FOUR _FOUR_1 { get; }
                public Inners._C _C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FOURD : _percentxFOURONEⲻFIVEA
            {
                public _FOURD(Inners._FOUR _FOUR_1, Inners._D _D_1)
                {
                    this._FOUR_1 = _FOUR_1;
                    this._D_1 = _D_1;
                }
                
                public Inners._FOUR _FOUR_1 { get; }
                public Inners._D _D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FOURE : _percentxFOURONEⲻFIVEA
            {
                public _FOURE(Inners._FOUR _FOUR_1, Inners._E _E_1)
                {
                    this._FOUR_1 = _FOUR_1;
                    this._E_1 = _E_1;
                }
                
                public Inners._FOUR _FOUR_1 { get; }
                public Inners._E _E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FOURF : _percentxFOURONEⲻFIVEA
            {
                public _FOURF(Inners._FOUR _FOUR_1, Inners._F _F_1)
                {
                    this._FOUR_1 = _FOUR_1;
                    this._F_1 = _F_1;
                }
                
                public Inners._FOUR _FOUR_1 { get; }
                public Inners._F _F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FIVEZERO : _percentxFOURONEⲻFIVEA
            {
                public _FIVEZERO(Inners._FIVE _FIVE_1, Inners._ZERO _ZERO_1)
                {
                    this._FIVE_1 = _FIVE_1;
                    this._ZERO_1 = _ZERO_1;
                }
                
                public Inners._FIVE _FIVE_1 { get; }
                public Inners._ZERO _ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FIVEONE : _percentxFOURONEⲻFIVEA
            {
                public _FIVEONE(Inners._FIVE _FIVE_1, Inners._ONE _ONE_1)
                {
                    this._FIVE_1 = _FIVE_1;
                    this._ONE_1 = _ONE_1;
                }
                
                public Inners._FIVE _FIVE_1 { get; }
                public Inners._ONE _ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FIVETWO : _percentxFOURONEⲻFIVEA
            {
                public _FIVETWO(Inners._FIVE _FIVE_1, Inners._TWO _TWO_1)
                {
                    this._FIVE_1 = _FIVE_1;
                    this._TWO_1 = _TWO_1;
                }
                
                public Inners._FIVE _FIVE_1 { get; }
                public Inners._TWO _TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FIVETHREE : _percentxFOURONEⲻFIVEA
            {
                public _FIVETHREE(Inners._FIVE _FIVE_1, Inners._THREE _THREE_1)
                {
                    this._FIVE_1 = _FIVE_1;
                    this._THREE_1 = _THREE_1;
                }
                
                public Inners._FIVE _FIVE_1 { get; }
                public Inners._THREE _THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FIVEFOUR : _percentxFOURONEⲻFIVEA
            {
                public _FIVEFOUR(Inners._FIVE _FIVE_1, Inners._FOUR _FOUR_1)
                {
                    this._FIVE_1 = _FIVE_1;
                    this._FOUR_1 = _FOUR_1;
                }
                
                public Inners._FIVE _FIVE_1 { get; }
                public Inners._FOUR _FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FIVEFIVE : _percentxFOURONEⲻFIVEA
            {
                public _FIVEFIVE(Inners._FIVE _FIVE_1, Inners._FIVE _FIVE_2)
                {
                    this._FIVE_1 = _FIVE_1;
                    this._FIVE_2 = _FIVE_2;
                }
                
                public Inners._FIVE _FIVE_1 { get; }
                public Inners._FIVE _FIVE_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FIVESIX : _percentxFOURONEⲻFIVEA
            {
                public _FIVESIX(Inners._FIVE _FIVE_1, Inners._SIX _SIX_1)
                {
                    this._FIVE_1 = _FIVE_1;
                    this._SIX_1 = _SIX_1;
                }
                
                public Inners._FIVE _FIVE_1 { get; }
                public Inners._SIX _SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FIVESEVEN : _percentxFOURONEⲻFIVEA
            {
                public _FIVESEVEN(Inners._FIVE _FIVE_1, Inners._SEVEN _SEVEN_1)
                {
                    this._FIVE_1 = _FIVE_1;
                    this._SEVEN_1 = _SEVEN_1;
                }
                
                public Inners._FIVE _FIVE_1 { get; }
                public Inners._SEVEN _SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FIVEEIGHT : _percentxFOURONEⲻFIVEA
            {
                public _FIVEEIGHT(Inners._FIVE _FIVE_1, Inners._EIGHT _EIGHT_1)
                {
                    this._FIVE_1 = _FIVE_1;
                    this._EIGHT_1 = _EIGHT_1;
                }
                
                public Inners._FIVE _FIVE_1 { get; }
                public Inners._EIGHT _EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FIVENINE : _percentxFOURONEⲻFIVEA
            {
                public _FIVENINE(Inners._FIVE _FIVE_1, Inners._NINE _NINE_1)
                {
                    this._FIVE_1 = _FIVE_1;
                    this._NINE_1 = _NINE_1;
                }
                
                public Inners._FIVE _FIVE_1 { get; }
                public Inners._NINE _NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FIVEA : _percentxFOURONEⲻFIVEA
            {
                public _FIVEA(Inners._FIVE _FIVE_1, Inners._A _A_1)
                {
                    this._FIVE_1 = _FIVE_1;
                    this._A_1 = _A_1;
                }
                
                public Inners._FIVE _FIVE_1 { get; }
                public Inners._A _A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
        }
        
        public abstract class _percentxSIXONEⲻSEVENA
        {
            private _percentxSIXONEⲻSEVENA()
            {
            }
            
            protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
            
            public abstract class Visitor<TResult, TContext>
            {
                public TResult Visit(_percentxSIXONEⲻSEVENA node, TContext context)
                {
                    return node.Dispatch(this, context);
                }
                
                protected internal abstract TResult Accept(_percentxSIXONEⲻSEVENA._SIXONE node, TContext context);
                protected internal abstract TResult Accept(_percentxSIXONEⲻSEVENA._SIXTWO node, TContext context);
                protected internal abstract TResult Accept(_percentxSIXONEⲻSEVENA._SIXTHREE node, TContext context);
                protected internal abstract TResult Accept(_percentxSIXONEⲻSEVENA._SIXFOUR node, TContext context);
                protected internal abstract TResult Accept(_percentxSIXONEⲻSEVENA._SIXFIVE node, TContext context);
                protected internal abstract TResult Accept(_percentxSIXONEⲻSEVENA._SIXSIX node, TContext context);
                protected internal abstract TResult Accept(_percentxSIXONEⲻSEVENA._SIXSEVEN node, TContext context);
                protected internal abstract TResult Accept(_percentxSIXONEⲻSEVENA._SIXEIGHT node, TContext context);
                protected internal abstract TResult Accept(_percentxSIXONEⲻSEVENA._SIXNINE node, TContext context);
                protected internal abstract TResult Accept(_percentxSIXONEⲻSEVENA._SIXA node, TContext context);
                protected internal abstract TResult Accept(_percentxSIXONEⲻSEVENA._SIXB node, TContext context);
                protected internal abstract TResult Accept(_percentxSIXONEⲻSEVENA._SIXC node, TContext context);
                protected internal abstract TResult Accept(_percentxSIXONEⲻSEVENA._SIXD node, TContext context);
                protected internal abstract TResult Accept(_percentxSIXONEⲻSEVENA._SIXE node, TContext context);
                protected internal abstract TResult Accept(_percentxSIXONEⲻSEVENA._SIXF node, TContext context);
                protected internal abstract TResult Accept(_percentxSIXONEⲻSEVENA._SEVENZERO node, TContext context);
                protected internal abstract TResult Accept(_percentxSIXONEⲻSEVENA._SEVENONE node, TContext context);
                protected internal abstract TResult Accept(_percentxSIXONEⲻSEVENA._SEVENTWO node, TContext context);
                protected internal abstract TResult Accept(_percentxSIXONEⲻSEVENA._SEVENTHREE node, TContext context);
                protected internal abstract TResult Accept(_percentxSIXONEⲻSEVENA._SEVENFOUR node, TContext context);
                protected internal abstract TResult Accept(_percentxSIXONEⲻSEVENA._SEVENFIVE node, TContext context);
                protected internal abstract TResult Accept(_percentxSIXONEⲻSEVENA._SEVENSIX node, TContext context);
                protected internal abstract TResult Accept(_percentxSIXONEⲻSEVENA._SEVENSEVEN node, TContext context);
                protected internal abstract TResult Accept(_percentxSIXONEⲻSEVENA._SEVENEIGHT node, TContext context);
                protected internal abstract TResult Accept(_percentxSIXONEⲻSEVENA._SEVENNINE node, TContext context);
                protected internal abstract TResult Accept(_percentxSIXONEⲻSEVENA._SEVENA node, TContext context);
            }
            
            public sealed class _SIXONE : _percentxSIXONEⲻSEVENA
            {
                public _SIXONE(Inners._SIX _SIX_1, Inners._ONE _ONE_1)
                {
                    this._SIX_1 = _SIX_1;
                    this._ONE_1 = _ONE_1;
                }
                
                public Inners._SIX _SIX_1 { get; }
                public Inners._ONE _ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SIXTWO : _percentxSIXONEⲻSEVENA
            {
                public _SIXTWO(Inners._SIX _SIX_1, Inners._TWO _TWO_1)
                {
                    this._SIX_1 = _SIX_1;
                    this._TWO_1 = _TWO_1;
                }
                
                public Inners._SIX _SIX_1 { get; }
                public Inners._TWO _TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SIXTHREE : _percentxSIXONEⲻSEVENA
            {
                public _SIXTHREE(Inners._SIX _SIX_1, Inners._THREE _THREE_1)
                {
                    this._SIX_1 = _SIX_1;
                    this._THREE_1 = _THREE_1;
                }
                
                public Inners._SIX _SIX_1 { get; }
                public Inners._THREE _THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SIXFOUR : _percentxSIXONEⲻSEVENA
            {
                public _SIXFOUR(Inners._SIX _SIX_1, Inners._FOUR _FOUR_1)
                {
                    this._SIX_1 = _SIX_1;
                    this._FOUR_1 = _FOUR_1;
                }
                
                public Inners._SIX _SIX_1 { get; }
                public Inners._FOUR _FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SIXFIVE : _percentxSIXONEⲻSEVENA
            {
                public _SIXFIVE(Inners._SIX _SIX_1, Inners._FIVE _FIVE_1)
                {
                    this._SIX_1 = _SIX_1;
                    this._FIVE_1 = _FIVE_1;
                }
                
                public Inners._SIX _SIX_1 { get; }
                public Inners._FIVE _FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SIXSIX : _percentxSIXONEⲻSEVENA
            {
                public _SIXSIX(Inners._SIX _SIX_1, Inners._SIX _SIX_2)
                {
                    this._SIX_1 = _SIX_1;
                    this._SIX_2 = _SIX_2;
                }
                
                public Inners._SIX _SIX_1 { get; }
                public Inners._SIX _SIX_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SIXSEVEN : _percentxSIXONEⲻSEVENA
            {
                public _SIXSEVEN(Inners._SIX _SIX_1, Inners._SEVEN _SEVEN_1)
                {
                    this._SIX_1 = _SIX_1;
                    this._SEVEN_1 = _SEVEN_1;
                }
                
                public Inners._SIX _SIX_1 { get; }
                public Inners._SEVEN _SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SIXEIGHT : _percentxSIXONEⲻSEVENA
            {
                public _SIXEIGHT(Inners._SIX _SIX_1, Inners._EIGHT _EIGHT_1)
                {
                    this._SIX_1 = _SIX_1;
                    this._EIGHT_1 = _EIGHT_1;
                }
                
                public Inners._SIX _SIX_1 { get; }
                public Inners._EIGHT _EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SIXNINE : _percentxSIXONEⲻSEVENA
            {
                public _SIXNINE(Inners._SIX _SIX_1, Inners._NINE _NINE_1)
                {
                    this._SIX_1 = _SIX_1;
                    this._NINE_1 = _NINE_1;
                }
                
                public Inners._SIX _SIX_1 { get; }
                public Inners._NINE _NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SIXA : _percentxSIXONEⲻSEVENA
            {
                public _SIXA(Inners._SIX _SIX_1, Inners._A _A_1)
                {
                    this._SIX_1 = _SIX_1;
                    this._A_1 = _A_1;
                }
                
                public Inners._SIX _SIX_1 { get; }
                public Inners._A _A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SIXB : _percentxSIXONEⲻSEVENA
            {
                public _SIXB(Inners._SIX _SIX_1, Inners._B _B_1)
                {
                    this._SIX_1 = _SIX_1;
                    this._B_1 = _B_1;
                }
                
                public Inners._SIX _SIX_1 { get; }
                public Inners._B _B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SIXC : _percentxSIXONEⲻSEVENA
            {
                public _SIXC(Inners._SIX _SIX_1, Inners._C _C_1)
                {
                    this._SIX_1 = _SIX_1;
                    this._C_1 = _C_1;
                }
                
                public Inners._SIX _SIX_1 { get; }
                public Inners._C _C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SIXD : _percentxSIXONEⲻSEVENA
            {
                public _SIXD(Inners._SIX _SIX_1, Inners._D _D_1)
                {
                    this._SIX_1 = _SIX_1;
                    this._D_1 = _D_1;
                }
                
                public Inners._SIX _SIX_1 { get; }
                public Inners._D _D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SIXE : _percentxSIXONEⲻSEVENA
            {
                public _SIXE(Inners._SIX _SIX_1, Inners._E _E_1)
                {
                    this._SIX_1 = _SIX_1;
                    this._E_1 = _E_1;
                }
                
                public Inners._SIX _SIX_1 { get; }
                public Inners._E _E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SIXF : _percentxSIXONEⲻSEVENA
            {
                public _SIXF(Inners._SIX _SIX_1, Inners._F _F_1)
                {
                    this._SIX_1 = _SIX_1;
                    this._F_1 = _F_1;
                }
                
                public Inners._SIX _SIX_1 { get; }
                public Inners._F _F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SEVENZERO : _percentxSIXONEⲻSEVENA
            {
                public _SEVENZERO(Inners._SEVEN _SEVEN_1, Inners._ZERO _ZERO_1)
                {
                    this._SEVEN_1 = _SEVEN_1;
                    this._ZERO_1 = _ZERO_1;
                }
                
                public Inners._SEVEN _SEVEN_1 { get; }
                public Inners._ZERO _ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SEVENONE : _percentxSIXONEⲻSEVENA
            {
                public _SEVENONE(Inners._SEVEN _SEVEN_1, Inners._ONE _ONE_1)
                {
                    this._SEVEN_1 = _SEVEN_1;
                    this._ONE_1 = _ONE_1;
                }
                
                public Inners._SEVEN _SEVEN_1 { get; }
                public Inners._ONE _ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SEVENTWO : _percentxSIXONEⲻSEVENA
            {
                public _SEVENTWO(Inners._SEVEN _SEVEN_1, Inners._TWO _TWO_1)
                {
                    this._SEVEN_1 = _SEVEN_1;
                    this._TWO_1 = _TWO_1;
                }
                
                public Inners._SEVEN _SEVEN_1 { get; }
                public Inners._TWO _TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SEVENTHREE : _percentxSIXONEⲻSEVENA
            {
                public _SEVENTHREE(Inners._SEVEN _SEVEN_1, Inners._THREE _THREE_1)
                {
                    this._SEVEN_1 = _SEVEN_1;
                    this._THREE_1 = _THREE_1;
                }
                
                public Inners._SEVEN _SEVEN_1 { get; }
                public Inners._THREE _THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SEVENFOUR : _percentxSIXONEⲻSEVENA
            {
                public _SEVENFOUR(Inners._SEVEN _SEVEN_1, Inners._FOUR _FOUR_1)
                {
                    this._SEVEN_1 = _SEVEN_1;
                    this._FOUR_1 = _FOUR_1;
                }
                
                public Inners._SEVEN _SEVEN_1 { get; }
                public Inners._FOUR _FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SEVENFIVE : _percentxSIXONEⲻSEVENA
            {
                public _SEVENFIVE(Inners._SEVEN _SEVEN_1, Inners._FIVE _FIVE_1)
                {
                    this._SEVEN_1 = _SEVEN_1;
                    this._FIVE_1 = _FIVE_1;
                }
                
                public Inners._SEVEN _SEVEN_1 { get; }
                public Inners._FIVE _FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SEVENSIX : _percentxSIXONEⲻSEVENA
            {
                public _SEVENSIX(Inners._SEVEN _SEVEN_1, Inners._SIX _SIX_1)
                {
                    this._SEVEN_1 = _SEVEN_1;
                    this._SIX_1 = _SIX_1;
                }
                
                public Inners._SEVEN _SEVEN_1 { get; }
                public Inners._SIX _SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SEVENSEVEN : _percentxSIXONEⲻSEVENA
            {
                public _SEVENSEVEN(Inners._SEVEN _SEVEN_1, Inners._SEVEN _SEVEN_2)
                {
                    this._SEVEN_1 = _SEVEN_1;
                    this._SEVEN_2 = _SEVEN_2;
                }
                
                public Inners._SEVEN _SEVEN_1 { get; }
                public Inners._SEVEN _SEVEN_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SEVENEIGHT : _percentxSIXONEⲻSEVENA
            {
                public _SEVENEIGHT(Inners._SEVEN _SEVEN_1, Inners._EIGHT _EIGHT_1)
                {
                    this._SEVEN_1 = _SEVEN_1;
                    this._EIGHT_1 = _EIGHT_1;
                }
                
                public Inners._SEVEN _SEVEN_1 { get; }
                public Inners._EIGHT _EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SEVENNINE : _percentxSIXONEⲻSEVENA
            {
                public _SEVENNINE(Inners._SEVEN _SEVEN_1, Inners._NINE _NINE_1)
                {
                    this._SEVEN_1 = _SEVEN_1;
                    this._NINE_1 = _NINE_1;
                }
                
                public Inners._SEVEN _SEVEN_1 { get; }
                public Inners._NINE _NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SEVENA : _percentxSIXONEⲻSEVENA
            {
                public _SEVENA(Inners._SEVEN _SEVEN_1, Inners._A _A_1)
                {
                    this._SEVEN_1 = _SEVEN_1;
                    this._A_1 = _A_1;
                }
                
                public Inners._SEVEN _SEVEN_1 { get; }
                public Inners._A _A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
        }
        
        public sealed class _x30
        {
            private _x30()
            {
            }
            
            public static _x30 Instance { get; } = new _x30();
        }
        
        public sealed class _doublequotex30doublequote
        {
            public _doublequotex30doublequote(Inners._x30 _x30_1)
            {
                this._x30_1 = _x30_1;
            }
            
            public Inners._x30 _x30_1 { get; }
        }
        
        public sealed class _x31
        {
            private _x31()
            {
            }
            
            public static _x31 Instance { get; } = new _x31();
        }
        
        public sealed class _doublequotex31doublequote
        {
            public _doublequotex31doublequote(Inners._x31 _x31_1)
            {
                this._x31_1 = _x31_1;
            }
            
            public Inners._x31 _x31_1 { get; }
        }
        
        public abstract class _percentxZEROONEⲻSEVENF
        {
            private _percentxZEROONEⲻSEVENF()
            {
            }
            
            protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
            
            public abstract class Visitor<TResult, TContext>
            {
                public TResult Visit(_percentxZEROONEⲻSEVENF node, TContext context)
                {
                    return node.Dispatch(this, context);
                }
                
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._ZEROONE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._ZEROTWO node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._ZEROTHREE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._ZEROFOUR node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._ZEROFIVE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._ZEROSIX node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._ZEROSEVEN node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._ZEROEIGHT node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._ZERONINE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._ZEROA node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._ZEROB node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._ZEROC node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._ZEROD node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._ZEROE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._ZEROF node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._ONEZERO node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._ONEONE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._ONETWO node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._ONETHREE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._ONEFOUR node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._ONEFIVE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._ONESIX node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._ONESEVEN node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._ONEEIGHT node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._ONENINE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._ONEA node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._ONEB node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._ONEC node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._ONED node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._ONEE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._ONEF node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._TWOZERO node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._TWOONE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._TWOTWO node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._TWOTHREE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._TWOFOUR node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._TWOFIVE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._TWOSIX node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._TWOSEVEN node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._TWOEIGHT node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._TWONINE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._TWOA node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._TWOB node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._TWOC node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._TWOD node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._TWOE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._TWOF node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._THREEZERO node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._THREEONE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._THREETWO node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._THREETHREE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._THREEFOUR node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._THREEFIVE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._THREESIX node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._THREESEVEN node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._THREEEIGHT node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._THREENINE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._THREEA node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._THREEB node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._THREEC node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._THREED node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._THREEE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._THREEF node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._FOURZERO node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._FOURONE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._FOURTWO node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._FOURTHREE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._FOURFOUR node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._FOURFIVE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._FOURSIX node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._FOURSEVEN node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._FOUREIGHT node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._FOURNINE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._FOURA node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._FOURB node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._FOURC node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._FOURD node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._FOURE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._FOURF node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._FIVEZERO node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._FIVEONE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._FIVETWO node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._FIVETHREE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._FIVEFOUR node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._FIVEFIVE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._FIVESIX node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._FIVESEVEN node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._FIVEEIGHT node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._FIVENINE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._FIVEA node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._FIVEB node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._FIVEC node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._FIVED node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._FIVEE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._FIVEF node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._SIXZERO node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._SIXONE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._SIXTWO node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._SIXTHREE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._SIXFOUR node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._SIXFIVE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._SIXSIX node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._SIXSEVEN node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._SIXEIGHT node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._SIXNINE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._SIXA node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._SIXB node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._SIXC node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._SIXD node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._SIXE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._SIXF node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._SEVENZERO node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._SEVENONE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._SEVENTWO node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._SEVENTHREE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._SEVENFOUR node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._SEVENFIVE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._SEVENSIX node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._SEVENSEVEN node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._SEVENEIGHT node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._SEVENNINE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._SEVENA node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._SEVENB node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._SEVENC node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._SEVEND node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._SEVENE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROONEⲻSEVENF._SEVENF node, TContext context);
            }
            
            public sealed class _ZEROONE : _percentxZEROONEⲻSEVENF
            {
                public _ZEROONE(Inners._ZERO _ZERO_1, Inners._ONE _ONE_1)
                {
                    this._ZERO_1 = _ZERO_1;
                    this._ONE_1 = _ONE_1;
                }
                
                public Inners._ZERO _ZERO_1 { get; }
                public Inners._ONE _ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ZEROTWO : _percentxZEROONEⲻSEVENF
            {
                public _ZEROTWO(Inners._ZERO _ZERO_1, Inners._TWO _TWO_1)
                {
                    this._ZERO_1 = _ZERO_1;
                    this._TWO_1 = _TWO_1;
                }
                
                public Inners._ZERO _ZERO_1 { get; }
                public Inners._TWO _TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ZEROTHREE : _percentxZEROONEⲻSEVENF
            {
                public _ZEROTHREE(Inners._ZERO _ZERO_1, Inners._THREE _THREE_1)
                {
                    this._ZERO_1 = _ZERO_1;
                    this._THREE_1 = _THREE_1;
                }
                
                public Inners._ZERO _ZERO_1 { get; }
                public Inners._THREE _THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ZEROFOUR : _percentxZEROONEⲻSEVENF
            {
                public _ZEROFOUR(Inners._ZERO _ZERO_1, Inners._FOUR _FOUR_1)
                {
                    this._ZERO_1 = _ZERO_1;
                    this._FOUR_1 = _FOUR_1;
                }
                
                public Inners._ZERO _ZERO_1 { get; }
                public Inners._FOUR _FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ZEROFIVE : _percentxZEROONEⲻSEVENF
            {
                public _ZEROFIVE(Inners._ZERO _ZERO_1, Inners._FIVE _FIVE_1)
                {
                    this._ZERO_1 = _ZERO_1;
                    this._FIVE_1 = _FIVE_1;
                }
                
                public Inners._ZERO _ZERO_1 { get; }
                public Inners._FIVE _FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ZEROSIX : _percentxZEROONEⲻSEVENF
            {
                public _ZEROSIX(Inners._ZERO _ZERO_1, Inners._SIX _SIX_1)
                {
                    this._ZERO_1 = _ZERO_1;
                    this._SIX_1 = _SIX_1;
                }
                
                public Inners._ZERO _ZERO_1 { get; }
                public Inners._SIX _SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ZEROSEVEN : _percentxZEROONEⲻSEVENF
            {
                public _ZEROSEVEN(Inners._ZERO _ZERO_1, Inners._SEVEN _SEVEN_1)
                {
                    this._ZERO_1 = _ZERO_1;
                    this._SEVEN_1 = _SEVEN_1;
                }
                
                public Inners._ZERO _ZERO_1 { get; }
                public Inners._SEVEN _SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ZEROEIGHT : _percentxZEROONEⲻSEVENF
            {
                public _ZEROEIGHT(Inners._ZERO _ZERO_1, Inners._EIGHT _EIGHT_1)
                {
                    this._ZERO_1 = _ZERO_1;
                    this._EIGHT_1 = _EIGHT_1;
                }
                
                public Inners._ZERO _ZERO_1 { get; }
                public Inners._EIGHT _EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ZERONINE : _percentxZEROONEⲻSEVENF
            {
                public _ZERONINE(Inners._ZERO _ZERO_1, Inners._NINE _NINE_1)
                {
                    this._ZERO_1 = _ZERO_1;
                    this._NINE_1 = _NINE_1;
                }
                
                public Inners._ZERO _ZERO_1 { get; }
                public Inners._NINE _NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ZEROA : _percentxZEROONEⲻSEVENF
            {
                public _ZEROA(Inners._ZERO _ZERO_1, Inners._A _A_1)
                {
                    this._ZERO_1 = _ZERO_1;
                    this._A_1 = _A_1;
                }
                
                public Inners._ZERO _ZERO_1 { get; }
                public Inners._A _A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ZEROB : _percentxZEROONEⲻSEVENF
            {
                public _ZEROB(Inners._ZERO _ZERO_1, Inners._B _B_1)
                {
                    this._ZERO_1 = _ZERO_1;
                    this._B_1 = _B_1;
                }
                
                public Inners._ZERO _ZERO_1 { get; }
                public Inners._B _B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ZEROC : _percentxZEROONEⲻSEVENF
            {
                public _ZEROC(Inners._ZERO _ZERO_1, Inners._C _C_1)
                {
                    this._ZERO_1 = _ZERO_1;
                    this._C_1 = _C_1;
                }
                
                public Inners._ZERO _ZERO_1 { get; }
                public Inners._C _C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ZEROD : _percentxZEROONEⲻSEVENF
            {
                public _ZEROD(Inners._ZERO _ZERO_1, Inners._D _D_1)
                {
                    this._ZERO_1 = _ZERO_1;
                    this._D_1 = _D_1;
                }
                
                public Inners._ZERO _ZERO_1 { get; }
                public Inners._D _D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ZEROE : _percentxZEROONEⲻSEVENF
            {
                public _ZEROE(Inners._ZERO _ZERO_1, Inners._E _E_1)
                {
                    this._ZERO_1 = _ZERO_1;
                    this._E_1 = _E_1;
                }
                
                public Inners._ZERO _ZERO_1 { get; }
                public Inners._E _E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ZEROF : _percentxZEROONEⲻSEVENF
            {
                public _ZEROF(Inners._ZERO _ZERO_1, Inners._F _F_1)
                {
                    this._ZERO_1 = _ZERO_1;
                    this._F_1 = _F_1;
                }
                
                public Inners._ZERO _ZERO_1 { get; }
                public Inners._F _F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ONEZERO : _percentxZEROONEⲻSEVENF
            {
                public _ONEZERO(Inners._ONE _ONE_1, Inners._ZERO _ZERO_1)
                {
                    this._ONE_1 = _ONE_1;
                    this._ZERO_1 = _ZERO_1;
                }
                
                public Inners._ONE _ONE_1 { get; }
                public Inners._ZERO _ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ONEONE : _percentxZEROONEⲻSEVENF
            {
                public _ONEONE(Inners._ONE _ONE_1, Inners._ONE _ONE_2)
                {
                    this._ONE_1 = _ONE_1;
                    this._ONE_2 = _ONE_2;
                }
                
                public Inners._ONE _ONE_1 { get; }
                public Inners._ONE _ONE_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ONETWO : _percentxZEROONEⲻSEVENF
            {
                public _ONETWO(Inners._ONE _ONE_1, Inners._TWO _TWO_1)
                {
                    this._ONE_1 = _ONE_1;
                    this._TWO_1 = _TWO_1;
                }
                
                public Inners._ONE _ONE_1 { get; }
                public Inners._TWO _TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ONETHREE : _percentxZEROONEⲻSEVENF
            {
                public _ONETHREE(Inners._ONE _ONE_1, Inners._THREE _THREE_1)
                {
                    this._ONE_1 = _ONE_1;
                    this._THREE_1 = _THREE_1;
                }
                
                public Inners._ONE _ONE_1 { get; }
                public Inners._THREE _THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ONEFOUR : _percentxZEROONEⲻSEVENF
            {
                public _ONEFOUR(Inners._ONE _ONE_1, Inners._FOUR _FOUR_1)
                {
                    this._ONE_1 = _ONE_1;
                    this._FOUR_1 = _FOUR_1;
                }
                
                public Inners._ONE _ONE_1 { get; }
                public Inners._FOUR _FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ONEFIVE : _percentxZEROONEⲻSEVENF
            {
                public _ONEFIVE(Inners._ONE _ONE_1, Inners._FIVE _FIVE_1)
                {
                    this._ONE_1 = _ONE_1;
                    this._FIVE_1 = _FIVE_1;
                }
                
                public Inners._ONE _ONE_1 { get; }
                public Inners._FIVE _FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ONESIX : _percentxZEROONEⲻSEVENF
            {
                public _ONESIX(Inners._ONE _ONE_1, Inners._SIX _SIX_1)
                {
                    this._ONE_1 = _ONE_1;
                    this._SIX_1 = _SIX_1;
                }
                
                public Inners._ONE _ONE_1 { get; }
                public Inners._SIX _SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ONESEVEN : _percentxZEROONEⲻSEVENF
            {
                public _ONESEVEN(Inners._ONE _ONE_1, Inners._SEVEN _SEVEN_1)
                {
                    this._ONE_1 = _ONE_1;
                    this._SEVEN_1 = _SEVEN_1;
                }
                
                public Inners._ONE _ONE_1 { get; }
                public Inners._SEVEN _SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ONEEIGHT : _percentxZEROONEⲻSEVENF
            {
                public _ONEEIGHT(Inners._ONE _ONE_1, Inners._EIGHT _EIGHT_1)
                {
                    this._ONE_1 = _ONE_1;
                    this._EIGHT_1 = _EIGHT_1;
                }
                
                public Inners._ONE _ONE_1 { get; }
                public Inners._EIGHT _EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ONENINE : _percentxZEROONEⲻSEVENF
            {
                public _ONENINE(Inners._ONE _ONE_1, Inners._NINE _NINE_1)
                {
                    this._ONE_1 = _ONE_1;
                    this._NINE_1 = _NINE_1;
                }
                
                public Inners._ONE _ONE_1 { get; }
                public Inners._NINE _NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ONEA : _percentxZEROONEⲻSEVENF
            {
                public _ONEA(Inners._ONE _ONE_1, Inners._A _A_1)
                {
                    this._ONE_1 = _ONE_1;
                    this._A_1 = _A_1;
                }
                
                public Inners._ONE _ONE_1 { get; }
                public Inners._A _A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ONEB : _percentxZEROONEⲻSEVENF
            {
                public _ONEB(Inners._ONE _ONE_1, Inners._B _B_1)
                {
                    this._ONE_1 = _ONE_1;
                    this._B_1 = _B_1;
                }
                
                public Inners._ONE _ONE_1 { get; }
                public Inners._B _B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ONEC : _percentxZEROONEⲻSEVENF
            {
                public _ONEC(Inners._ONE _ONE_1, Inners._C _C_1)
                {
                    this._ONE_1 = _ONE_1;
                    this._C_1 = _C_1;
                }
                
                public Inners._ONE _ONE_1 { get; }
                public Inners._C _C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ONED : _percentxZEROONEⲻSEVENF
            {
                public _ONED(Inners._ONE _ONE_1, Inners._D _D_1)
                {
                    this._ONE_1 = _ONE_1;
                    this._D_1 = _D_1;
                }
                
                public Inners._ONE _ONE_1 { get; }
                public Inners._D _D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ONEE : _percentxZEROONEⲻSEVENF
            {
                public _ONEE(Inners._ONE _ONE_1, Inners._E _E_1)
                {
                    this._ONE_1 = _ONE_1;
                    this._E_1 = _E_1;
                }
                
                public Inners._ONE _ONE_1 { get; }
                public Inners._E _E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ONEF : _percentxZEROONEⲻSEVENF
            {
                public _ONEF(Inners._ONE _ONE_1, Inners._F _F_1)
                {
                    this._ONE_1 = _ONE_1;
                    this._F_1 = _F_1;
                }
                
                public Inners._ONE _ONE_1 { get; }
                public Inners._F _F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _TWOZERO : _percentxZEROONEⲻSEVENF
            {
                public _TWOZERO(Inners._TWO _TWO_1, Inners._ZERO _ZERO_1)
                {
                    this._TWO_1 = _TWO_1;
                    this._ZERO_1 = _ZERO_1;
                }
                
                public Inners._TWO _TWO_1 { get; }
                public Inners._ZERO _ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _TWOONE : _percentxZEROONEⲻSEVENF
            {
                public _TWOONE(Inners._TWO _TWO_1, Inners._ONE _ONE_1)
                {
                    this._TWO_1 = _TWO_1;
                    this._ONE_1 = _ONE_1;
                }
                
                public Inners._TWO _TWO_1 { get; }
                public Inners._ONE _ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _TWOTWO : _percentxZEROONEⲻSEVENF
            {
                public _TWOTWO(Inners._TWO _TWO_1, Inners._TWO _TWO_2)
                {
                    this._TWO_1 = _TWO_1;
                    this._TWO_2 = _TWO_2;
                }
                
                public Inners._TWO _TWO_1 { get; }
                public Inners._TWO _TWO_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _TWOTHREE : _percentxZEROONEⲻSEVENF
            {
                public _TWOTHREE(Inners._TWO _TWO_1, Inners._THREE _THREE_1)
                {
                    this._TWO_1 = _TWO_1;
                    this._THREE_1 = _THREE_1;
                }
                
                public Inners._TWO _TWO_1 { get; }
                public Inners._THREE _THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _TWOFOUR : _percentxZEROONEⲻSEVENF
            {
                public _TWOFOUR(Inners._TWO _TWO_1, Inners._FOUR _FOUR_1)
                {
                    this._TWO_1 = _TWO_1;
                    this._FOUR_1 = _FOUR_1;
                }
                
                public Inners._TWO _TWO_1 { get; }
                public Inners._FOUR _FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _TWOFIVE : _percentxZEROONEⲻSEVENF
            {
                public _TWOFIVE(Inners._TWO _TWO_1, Inners._FIVE _FIVE_1)
                {
                    this._TWO_1 = _TWO_1;
                    this._FIVE_1 = _FIVE_1;
                }
                
                public Inners._TWO _TWO_1 { get; }
                public Inners._FIVE _FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _TWOSIX : _percentxZEROONEⲻSEVENF
            {
                public _TWOSIX(Inners._TWO _TWO_1, Inners._SIX _SIX_1)
                {
                    this._TWO_1 = _TWO_1;
                    this._SIX_1 = _SIX_1;
                }
                
                public Inners._TWO _TWO_1 { get; }
                public Inners._SIX _SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _TWOSEVEN : _percentxZEROONEⲻSEVENF
            {
                public _TWOSEVEN(Inners._TWO _TWO_1, Inners._SEVEN _SEVEN_1)
                {
                    this._TWO_1 = _TWO_1;
                    this._SEVEN_1 = _SEVEN_1;
                }
                
                public Inners._TWO _TWO_1 { get; }
                public Inners._SEVEN _SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _TWOEIGHT : _percentxZEROONEⲻSEVENF
            {
                public _TWOEIGHT(Inners._TWO _TWO_1, Inners._EIGHT _EIGHT_1)
                {
                    this._TWO_1 = _TWO_1;
                    this._EIGHT_1 = _EIGHT_1;
                }
                
                public Inners._TWO _TWO_1 { get; }
                public Inners._EIGHT _EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _TWONINE : _percentxZEROONEⲻSEVENF
            {
                public _TWONINE(Inners._TWO _TWO_1, Inners._NINE _NINE_1)
                {
                    this._TWO_1 = _TWO_1;
                    this._NINE_1 = _NINE_1;
                }
                
                public Inners._TWO _TWO_1 { get; }
                public Inners._NINE _NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _TWOA : _percentxZEROONEⲻSEVENF
            {
                public _TWOA(Inners._TWO _TWO_1, Inners._A _A_1)
                {
                    this._TWO_1 = _TWO_1;
                    this._A_1 = _A_1;
                }
                
                public Inners._TWO _TWO_1 { get; }
                public Inners._A _A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _TWOB : _percentxZEROONEⲻSEVENF
            {
                public _TWOB(Inners._TWO _TWO_1, Inners._B _B_1)
                {
                    this._TWO_1 = _TWO_1;
                    this._B_1 = _B_1;
                }
                
                public Inners._TWO _TWO_1 { get; }
                public Inners._B _B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _TWOC : _percentxZEROONEⲻSEVENF
            {
                public _TWOC(Inners._TWO _TWO_1, Inners._C _C_1)
                {
                    this._TWO_1 = _TWO_1;
                    this._C_1 = _C_1;
                }
                
                public Inners._TWO _TWO_1 { get; }
                public Inners._C _C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _TWOD : _percentxZEROONEⲻSEVENF
            {
                public _TWOD(Inners._TWO _TWO_1, Inners._D _D_1)
                {
                    this._TWO_1 = _TWO_1;
                    this._D_1 = _D_1;
                }
                
                public Inners._TWO _TWO_1 { get; }
                public Inners._D _D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _TWOE : _percentxZEROONEⲻSEVENF
            {
                public _TWOE(Inners._TWO _TWO_1, Inners._E _E_1)
                {
                    this._TWO_1 = _TWO_1;
                    this._E_1 = _E_1;
                }
                
                public Inners._TWO _TWO_1 { get; }
                public Inners._E _E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _TWOF : _percentxZEROONEⲻSEVENF
            {
                public _TWOF(Inners._TWO _TWO_1, Inners._F _F_1)
                {
                    this._TWO_1 = _TWO_1;
                    this._F_1 = _F_1;
                }
                
                public Inners._TWO _TWO_1 { get; }
                public Inners._F _F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _THREEZERO : _percentxZEROONEⲻSEVENF
            {
                public _THREEZERO(Inners._THREE _THREE_1, Inners._ZERO _ZERO_1)
                {
                    this._THREE_1 = _THREE_1;
                    this._ZERO_1 = _ZERO_1;
                }
                
                public Inners._THREE _THREE_1 { get; }
                public Inners._ZERO _ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _THREEONE : _percentxZEROONEⲻSEVENF
            {
                public _THREEONE(Inners._THREE _THREE_1, Inners._ONE _ONE_1)
                {
                    this._THREE_1 = _THREE_1;
                    this._ONE_1 = _ONE_1;
                }
                
                public Inners._THREE _THREE_1 { get; }
                public Inners._ONE _ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _THREETWO : _percentxZEROONEⲻSEVENF
            {
                public _THREETWO(Inners._THREE _THREE_1, Inners._TWO _TWO_1)
                {
                    this._THREE_1 = _THREE_1;
                    this._TWO_1 = _TWO_1;
                }
                
                public Inners._THREE _THREE_1 { get; }
                public Inners._TWO _TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _THREETHREE : _percentxZEROONEⲻSEVENF
            {
                public _THREETHREE(Inners._THREE _THREE_1, Inners._THREE _THREE_2)
                {
                    this._THREE_1 = _THREE_1;
                    this._THREE_2 = _THREE_2;
                }
                
                public Inners._THREE _THREE_1 { get; }
                public Inners._THREE _THREE_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _THREEFOUR : _percentxZEROONEⲻSEVENF
            {
                public _THREEFOUR(Inners._THREE _THREE_1, Inners._FOUR _FOUR_1)
                {
                    this._THREE_1 = _THREE_1;
                    this._FOUR_1 = _FOUR_1;
                }
                
                public Inners._THREE _THREE_1 { get; }
                public Inners._FOUR _FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _THREEFIVE : _percentxZEROONEⲻSEVENF
            {
                public _THREEFIVE(Inners._THREE _THREE_1, Inners._FIVE _FIVE_1)
                {
                    this._THREE_1 = _THREE_1;
                    this._FIVE_1 = _FIVE_1;
                }
                
                public Inners._THREE _THREE_1 { get; }
                public Inners._FIVE _FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _THREESIX : _percentxZEROONEⲻSEVENF
            {
                public _THREESIX(Inners._THREE _THREE_1, Inners._SIX _SIX_1)
                {
                    this._THREE_1 = _THREE_1;
                    this._SIX_1 = _SIX_1;
                }
                
                public Inners._THREE _THREE_1 { get; }
                public Inners._SIX _SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _THREESEVEN : _percentxZEROONEⲻSEVENF
            {
                public _THREESEVEN(Inners._THREE _THREE_1, Inners._SEVEN _SEVEN_1)
                {
                    this._THREE_1 = _THREE_1;
                    this._SEVEN_1 = _SEVEN_1;
                }
                
                public Inners._THREE _THREE_1 { get; }
                public Inners._SEVEN _SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _THREEEIGHT : _percentxZEROONEⲻSEVENF
            {
                public _THREEEIGHT(Inners._THREE _THREE_1, Inners._EIGHT _EIGHT_1)
                {
                    this._THREE_1 = _THREE_1;
                    this._EIGHT_1 = _EIGHT_1;
                }
                
                public Inners._THREE _THREE_1 { get; }
                public Inners._EIGHT _EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _THREENINE : _percentxZEROONEⲻSEVENF
            {
                public _THREENINE(Inners._THREE _THREE_1, Inners._NINE _NINE_1)
                {
                    this._THREE_1 = _THREE_1;
                    this._NINE_1 = _NINE_1;
                }
                
                public Inners._THREE _THREE_1 { get; }
                public Inners._NINE _NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _THREEA : _percentxZEROONEⲻSEVENF
            {
                public _THREEA(Inners._THREE _THREE_1, Inners._A _A_1)
                {
                    this._THREE_1 = _THREE_1;
                    this._A_1 = _A_1;
                }
                
                public Inners._THREE _THREE_1 { get; }
                public Inners._A _A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _THREEB : _percentxZEROONEⲻSEVENF
            {
                public _THREEB(Inners._THREE _THREE_1, Inners._B _B_1)
                {
                    this._THREE_1 = _THREE_1;
                    this._B_1 = _B_1;
                }
                
                public Inners._THREE _THREE_1 { get; }
                public Inners._B _B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _THREEC : _percentxZEROONEⲻSEVENF
            {
                public _THREEC(Inners._THREE _THREE_1, Inners._C _C_1)
                {
                    this._THREE_1 = _THREE_1;
                    this._C_1 = _C_1;
                }
                
                public Inners._THREE _THREE_1 { get; }
                public Inners._C _C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _THREED : _percentxZEROONEⲻSEVENF
            {
                public _THREED(Inners._THREE _THREE_1, Inners._D _D_1)
                {
                    this._THREE_1 = _THREE_1;
                    this._D_1 = _D_1;
                }
                
                public Inners._THREE _THREE_1 { get; }
                public Inners._D _D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _THREEE : _percentxZEROONEⲻSEVENF
            {
                public _THREEE(Inners._THREE _THREE_1, Inners._E _E_1)
                {
                    this._THREE_1 = _THREE_1;
                    this._E_1 = _E_1;
                }
                
                public Inners._THREE _THREE_1 { get; }
                public Inners._E _E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _THREEF : _percentxZEROONEⲻSEVENF
            {
                public _THREEF(Inners._THREE _THREE_1, Inners._F _F_1)
                {
                    this._THREE_1 = _THREE_1;
                    this._F_1 = _F_1;
                }
                
                public Inners._THREE _THREE_1 { get; }
                public Inners._F _F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FOURZERO : _percentxZEROONEⲻSEVENF
            {
                public _FOURZERO(Inners._FOUR _FOUR_1, Inners._ZERO _ZERO_1)
                {
                    this._FOUR_1 = _FOUR_1;
                    this._ZERO_1 = _ZERO_1;
                }
                
                public Inners._FOUR _FOUR_1 { get; }
                public Inners._ZERO _ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FOURONE : _percentxZEROONEⲻSEVENF
            {
                public _FOURONE(Inners._FOUR _FOUR_1, Inners._ONE _ONE_1)
                {
                    this._FOUR_1 = _FOUR_1;
                    this._ONE_1 = _ONE_1;
                }
                
                public Inners._FOUR _FOUR_1 { get; }
                public Inners._ONE _ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FOURTWO : _percentxZEROONEⲻSEVENF
            {
                public _FOURTWO(Inners._FOUR _FOUR_1, Inners._TWO _TWO_1)
                {
                    this._FOUR_1 = _FOUR_1;
                    this._TWO_1 = _TWO_1;
                }
                
                public Inners._FOUR _FOUR_1 { get; }
                public Inners._TWO _TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FOURTHREE : _percentxZEROONEⲻSEVENF
            {
                public _FOURTHREE(Inners._FOUR _FOUR_1, Inners._THREE _THREE_1)
                {
                    this._FOUR_1 = _FOUR_1;
                    this._THREE_1 = _THREE_1;
                }
                
                public Inners._FOUR _FOUR_1 { get; }
                public Inners._THREE _THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FOURFOUR : _percentxZEROONEⲻSEVENF
            {
                public _FOURFOUR(Inners._FOUR _FOUR_1, Inners._FOUR _FOUR_2)
                {
                    this._FOUR_1 = _FOUR_1;
                    this._FOUR_2 = _FOUR_2;
                }
                
                public Inners._FOUR _FOUR_1 { get; }
                public Inners._FOUR _FOUR_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FOURFIVE : _percentxZEROONEⲻSEVENF
            {
                public _FOURFIVE(Inners._FOUR _FOUR_1, Inners._FIVE _FIVE_1)
                {
                    this._FOUR_1 = _FOUR_1;
                    this._FIVE_1 = _FIVE_1;
                }
                
                public Inners._FOUR _FOUR_1 { get; }
                public Inners._FIVE _FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FOURSIX : _percentxZEROONEⲻSEVENF
            {
                public _FOURSIX(Inners._FOUR _FOUR_1, Inners._SIX _SIX_1)
                {
                    this._FOUR_1 = _FOUR_1;
                    this._SIX_1 = _SIX_1;
                }
                
                public Inners._FOUR _FOUR_1 { get; }
                public Inners._SIX _SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FOURSEVEN : _percentxZEROONEⲻSEVENF
            {
                public _FOURSEVEN(Inners._FOUR _FOUR_1, Inners._SEVEN _SEVEN_1)
                {
                    this._FOUR_1 = _FOUR_1;
                    this._SEVEN_1 = _SEVEN_1;
                }
                
                public Inners._FOUR _FOUR_1 { get; }
                public Inners._SEVEN _SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FOUREIGHT : _percentxZEROONEⲻSEVENF
            {
                public _FOUREIGHT(Inners._FOUR _FOUR_1, Inners._EIGHT _EIGHT_1)
                {
                    this._FOUR_1 = _FOUR_1;
                    this._EIGHT_1 = _EIGHT_1;
                }
                
                public Inners._FOUR _FOUR_1 { get; }
                public Inners._EIGHT _EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FOURNINE : _percentxZEROONEⲻSEVENF
            {
                public _FOURNINE(Inners._FOUR _FOUR_1, Inners._NINE _NINE_1)
                {
                    this._FOUR_1 = _FOUR_1;
                    this._NINE_1 = _NINE_1;
                }
                
                public Inners._FOUR _FOUR_1 { get; }
                public Inners._NINE _NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FOURA : _percentxZEROONEⲻSEVENF
            {
                public _FOURA(Inners._FOUR _FOUR_1, Inners._A _A_1)
                {
                    this._FOUR_1 = _FOUR_1;
                    this._A_1 = _A_1;
                }
                
                public Inners._FOUR _FOUR_1 { get; }
                public Inners._A _A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FOURB : _percentxZEROONEⲻSEVENF
            {
                public _FOURB(Inners._FOUR _FOUR_1, Inners._B _B_1)
                {
                    this._FOUR_1 = _FOUR_1;
                    this._B_1 = _B_1;
                }
                
                public Inners._FOUR _FOUR_1 { get; }
                public Inners._B _B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FOURC : _percentxZEROONEⲻSEVENF
            {
                public _FOURC(Inners._FOUR _FOUR_1, Inners._C _C_1)
                {
                    this._FOUR_1 = _FOUR_1;
                    this._C_1 = _C_1;
                }
                
                public Inners._FOUR _FOUR_1 { get; }
                public Inners._C _C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FOURD : _percentxZEROONEⲻSEVENF
            {
                public _FOURD(Inners._FOUR _FOUR_1, Inners._D _D_1)
                {
                    this._FOUR_1 = _FOUR_1;
                    this._D_1 = _D_1;
                }
                
                public Inners._FOUR _FOUR_1 { get; }
                public Inners._D _D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FOURE : _percentxZEROONEⲻSEVENF
            {
                public _FOURE(Inners._FOUR _FOUR_1, Inners._E _E_1)
                {
                    this._FOUR_1 = _FOUR_1;
                    this._E_1 = _E_1;
                }
                
                public Inners._FOUR _FOUR_1 { get; }
                public Inners._E _E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FOURF : _percentxZEROONEⲻSEVENF
            {
                public _FOURF(Inners._FOUR _FOUR_1, Inners._F _F_1)
                {
                    this._FOUR_1 = _FOUR_1;
                    this._F_1 = _F_1;
                }
                
                public Inners._FOUR _FOUR_1 { get; }
                public Inners._F _F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FIVEZERO : _percentxZEROONEⲻSEVENF
            {
                public _FIVEZERO(Inners._FIVE _FIVE_1, Inners._ZERO _ZERO_1)
                {
                    this._FIVE_1 = _FIVE_1;
                    this._ZERO_1 = _ZERO_1;
                }
                
                public Inners._FIVE _FIVE_1 { get; }
                public Inners._ZERO _ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FIVEONE : _percentxZEROONEⲻSEVENF
            {
                public _FIVEONE(Inners._FIVE _FIVE_1, Inners._ONE _ONE_1)
                {
                    this._FIVE_1 = _FIVE_1;
                    this._ONE_1 = _ONE_1;
                }
                
                public Inners._FIVE _FIVE_1 { get; }
                public Inners._ONE _ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FIVETWO : _percentxZEROONEⲻSEVENF
            {
                public _FIVETWO(Inners._FIVE _FIVE_1, Inners._TWO _TWO_1)
                {
                    this._FIVE_1 = _FIVE_1;
                    this._TWO_1 = _TWO_1;
                }
                
                public Inners._FIVE _FIVE_1 { get; }
                public Inners._TWO _TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FIVETHREE : _percentxZEROONEⲻSEVENF
            {
                public _FIVETHREE(Inners._FIVE _FIVE_1, Inners._THREE _THREE_1)
                {
                    this._FIVE_1 = _FIVE_1;
                    this._THREE_1 = _THREE_1;
                }
                
                public Inners._FIVE _FIVE_1 { get; }
                public Inners._THREE _THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FIVEFOUR : _percentxZEROONEⲻSEVENF
            {
                public _FIVEFOUR(Inners._FIVE _FIVE_1, Inners._FOUR _FOUR_1)
                {
                    this._FIVE_1 = _FIVE_1;
                    this._FOUR_1 = _FOUR_1;
                }
                
                public Inners._FIVE _FIVE_1 { get; }
                public Inners._FOUR _FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FIVEFIVE : _percentxZEROONEⲻSEVENF
            {
                public _FIVEFIVE(Inners._FIVE _FIVE_1, Inners._FIVE _FIVE_2)
                {
                    this._FIVE_1 = _FIVE_1;
                    this._FIVE_2 = _FIVE_2;
                }
                
                public Inners._FIVE _FIVE_1 { get; }
                public Inners._FIVE _FIVE_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FIVESIX : _percentxZEROONEⲻSEVENF
            {
                public _FIVESIX(Inners._FIVE _FIVE_1, Inners._SIX _SIX_1)
                {
                    this._FIVE_1 = _FIVE_1;
                    this._SIX_1 = _SIX_1;
                }
                
                public Inners._FIVE _FIVE_1 { get; }
                public Inners._SIX _SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FIVESEVEN : _percentxZEROONEⲻSEVENF
            {
                public _FIVESEVEN(Inners._FIVE _FIVE_1, Inners._SEVEN _SEVEN_1)
                {
                    this._FIVE_1 = _FIVE_1;
                    this._SEVEN_1 = _SEVEN_1;
                }
                
                public Inners._FIVE _FIVE_1 { get; }
                public Inners._SEVEN _SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FIVEEIGHT : _percentxZEROONEⲻSEVENF
            {
                public _FIVEEIGHT(Inners._FIVE _FIVE_1, Inners._EIGHT _EIGHT_1)
                {
                    this._FIVE_1 = _FIVE_1;
                    this._EIGHT_1 = _EIGHT_1;
                }
                
                public Inners._FIVE _FIVE_1 { get; }
                public Inners._EIGHT _EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FIVENINE : _percentxZEROONEⲻSEVENF
            {
                public _FIVENINE(Inners._FIVE _FIVE_1, Inners._NINE _NINE_1)
                {
                    this._FIVE_1 = _FIVE_1;
                    this._NINE_1 = _NINE_1;
                }
                
                public Inners._FIVE _FIVE_1 { get; }
                public Inners._NINE _NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FIVEA : _percentxZEROONEⲻSEVENF
            {
                public _FIVEA(Inners._FIVE _FIVE_1, Inners._A _A_1)
                {
                    this._FIVE_1 = _FIVE_1;
                    this._A_1 = _A_1;
                }
                
                public Inners._FIVE _FIVE_1 { get; }
                public Inners._A _A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FIVEB : _percentxZEROONEⲻSEVENF
            {
                public _FIVEB(Inners._FIVE _FIVE_1, Inners._B _B_1)
                {
                    this._FIVE_1 = _FIVE_1;
                    this._B_1 = _B_1;
                }
                
                public Inners._FIVE _FIVE_1 { get; }
                public Inners._B _B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FIVEC : _percentxZEROONEⲻSEVENF
            {
                public _FIVEC(Inners._FIVE _FIVE_1, Inners._C _C_1)
                {
                    this._FIVE_1 = _FIVE_1;
                    this._C_1 = _C_1;
                }
                
                public Inners._FIVE _FIVE_1 { get; }
                public Inners._C _C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FIVED : _percentxZEROONEⲻSEVENF
            {
                public _FIVED(Inners._FIVE _FIVE_1, Inners._D _D_1)
                {
                    this._FIVE_1 = _FIVE_1;
                    this._D_1 = _D_1;
                }
                
                public Inners._FIVE _FIVE_1 { get; }
                public Inners._D _D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FIVEE : _percentxZEROONEⲻSEVENF
            {
                public _FIVEE(Inners._FIVE _FIVE_1, Inners._E _E_1)
                {
                    this._FIVE_1 = _FIVE_1;
                    this._E_1 = _E_1;
                }
                
                public Inners._FIVE _FIVE_1 { get; }
                public Inners._E _E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FIVEF : _percentxZEROONEⲻSEVENF
            {
                public _FIVEF(Inners._FIVE _FIVE_1, Inners._F _F_1)
                {
                    this._FIVE_1 = _FIVE_1;
                    this._F_1 = _F_1;
                }
                
                public Inners._FIVE _FIVE_1 { get; }
                public Inners._F _F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SIXZERO : _percentxZEROONEⲻSEVENF
            {
                public _SIXZERO(Inners._SIX _SIX_1, Inners._ZERO _ZERO_1)
                {
                    this._SIX_1 = _SIX_1;
                    this._ZERO_1 = _ZERO_1;
                }
                
                public Inners._SIX _SIX_1 { get; }
                public Inners._ZERO _ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SIXONE : _percentxZEROONEⲻSEVENF
            {
                public _SIXONE(Inners._SIX _SIX_1, Inners._ONE _ONE_1)
                {
                    this._SIX_1 = _SIX_1;
                    this._ONE_1 = _ONE_1;
                }
                
                public Inners._SIX _SIX_1 { get; }
                public Inners._ONE _ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SIXTWO : _percentxZEROONEⲻSEVENF
            {
                public _SIXTWO(Inners._SIX _SIX_1, Inners._TWO _TWO_1)
                {
                    this._SIX_1 = _SIX_1;
                    this._TWO_1 = _TWO_1;
                }
                
                public Inners._SIX _SIX_1 { get; }
                public Inners._TWO _TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SIXTHREE : _percentxZEROONEⲻSEVENF
            {
                public _SIXTHREE(Inners._SIX _SIX_1, Inners._THREE _THREE_1)
                {
                    this._SIX_1 = _SIX_1;
                    this._THREE_1 = _THREE_1;
                }
                
                public Inners._SIX _SIX_1 { get; }
                public Inners._THREE _THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SIXFOUR : _percentxZEROONEⲻSEVENF
            {
                public _SIXFOUR(Inners._SIX _SIX_1, Inners._FOUR _FOUR_1)
                {
                    this._SIX_1 = _SIX_1;
                    this._FOUR_1 = _FOUR_1;
                }
                
                public Inners._SIX _SIX_1 { get; }
                public Inners._FOUR _FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SIXFIVE : _percentxZEROONEⲻSEVENF
            {
                public _SIXFIVE(Inners._SIX _SIX_1, Inners._FIVE _FIVE_1)
                {
                    this._SIX_1 = _SIX_1;
                    this._FIVE_1 = _FIVE_1;
                }
                
                public Inners._SIX _SIX_1 { get; }
                public Inners._FIVE _FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SIXSIX : _percentxZEROONEⲻSEVENF
            {
                public _SIXSIX(Inners._SIX _SIX_1, Inners._SIX _SIX_2)
                {
                    this._SIX_1 = _SIX_1;
                    this._SIX_2 = _SIX_2;
                }
                
                public Inners._SIX _SIX_1 { get; }
                public Inners._SIX _SIX_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SIXSEVEN : _percentxZEROONEⲻSEVENF
            {
                public _SIXSEVEN(Inners._SIX _SIX_1, Inners._SEVEN _SEVEN_1)
                {
                    this._SIX_1 = _SIX_1;
                    this._SEVEN_1 = _SEVEN_1;
                }
                
                public Inners._SIX _SIX_1 { get; }
                public Inners._SEVEN _SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SIXEIGHT : _percentxZEROONEⲻSEVENF
            {
                public _SIXEIGHT(Inners._SIX _SIX_1, Inners._EIGHT _EIGHT_1)
                {
                    this._SIX_1 = _SIX_1;
                    this._EIGHT_1 = _EIGHT_1;
                }
                
                public Inners._SIX _SIX_1 { get; }
                public Inners._EIGHT _EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SIXNINE : _percentxZEROONEⲻSEVENF
            {
                public _SIXNINE(Inners._SIX _SIX_1, Inners._NINE _NINE_1)
                {
                    this._SIX_1 = _SIX_1;
                    this._NINE_1 = _NINE_1;
                }
                
                public Inners._SIX _SIX_1 { get; }
                public Inners._NINE _NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SIXA : _percentxZEROONEⲻSEVENF
            {
                public _SIXA(Inners._SIX _SIX_1, Inners._A _A_1)
                {
                    this._SIX_1 = _SIX_1;
                    this._A_1 = _A_1;
                }
                
                public Inners._SIX _SIX_1 { get; }
                public Inners._A _A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SIXB : _percentxZEROONEⲻSEVENF
            {
                public _SIXB(Inners._SIX _SIX_1, Inners._B _B_1)
                {
                    this._SIX_1 = _SIX_1;
                    this._B_1 = _B_1;
                }
                
                public Inners._SIX _SIX_1 { get; }
                public Inners._B _B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SIXC : _percentxZEROONEⲻSEVENF
            {
                public _SIXC(Inners._SIX _SIX_1, Inners._C _C_1)
                {
                    this._SIX_1 = _SIX_1;
                    this._C_1 = _C_1;
                }
                
                public Inners._SIX _SIX_1 { get; }
                public Inners._C _C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SIXD : _percentxZEROONEⲻSEVENF
            {
                public _SIXD(Inners._SIX _SIX_1, Inners._D _D_1)
                {
                    this._SIX_1 = _SIX_1;
                    this._D_1 = _D_1;
                }
                
                public Inners._SIX _SIX_1 { get; }
                public Inners._D _D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SIXE : _percentxZEROONEⲻSEVENF
            {
                public _SIXE(Inners._SIX _SIX_1, Inners._E _E_1)
                {
                    this._SIX_1 = _SIX_1;
                    this._E_1 = _E_1;
                }
                
                public Inners._SIX _SIX_1 { get; }
                public Inners._E _E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SIXF : _percentxZEROONEⲻSEVENF
            {
                public _SIXF(Inners._SIX _SIX_1, Inners._F _F_1)
                {
                    this._SIX_1 = _SIX_1;
                    this._F_1 = _F_1;
                }
                
                public Inners._SIX _SIX_1 { get; }
                public Inners._F _F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SEVENZERO : _percentxZEROONEⲻSEVENF
            {
                public _SEVENZERO(Inners._SEVEN _SEVEN_1, Inners._ZERO _ZERO_1)
                {
                    this._SEVEN_1 = _SEVEN_1;
                    this._ZERO_1 = _ZERO_1;
                }
                
                public Inners._SEVEN _SEVEN_1 { get; }
                public Inners._ZERO _ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SEVENONE : _percentxZEROONEⲻSEVENF
            {
                public _SEVENONE(Inners._SEVEN _SEVEN_1, Inners._ONE _ONE_1)
                {
                    this._SEVEN_1 = _SEVEN_1;
                    this._ONE_1 = _ONE_1;
                }
                
                public Inners._SEVEN _SEVEN_1 { get; }
                public Inners._ONE _ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SEVENTWO : _percentxZEROONEⲻSEVENF
            {
                public _SEVENTWO(Inners._SEVEN _SEVEN_1, Inners._TWO _TWO_1)
                {
                    this._SEVEN_1 = _SEVEN_1;
                    this._TWO_1 = _TWO_1;
                }
                
                public Inners._SEVEN _SEVEN_1 { get; }
                public Inners._TWO _TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SEVENTHREE : _percentxZEROONEⲻSEVENF
            {
                public _SEVENTHREE(Inners._SEVEN _SEVEN_1, Inners._THREE _THREE_1)
                {
                    this._SEVEN_1 = _SEVEN_1;
                    this._THREE_1 = _THREE_1;
                }
                
                public Inners._SEVEN _SEVEN_1 { get; }
                public Inners._THREE _THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SEVENFOUR : _percentxZEROONEⲻSEVENF
            {
                public _SEVENFOUR(Inners._SEVEN _SEVEN_1, Inners._FOUR _FOUR_1)
                {
                    this._SEVEN_1 = _SEVEN_1;
                    this._FOUR_1 = _FOUR_1;
                }
                
                public Inners._SEVEN _SEVEN_1 { get; }
                public Inners._FOUR _FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SEVENFIVE : _percentxZEROONEⲻSEVENF
            {
                public _SEVENFIVE(Inners._SEVEN _SEVEN_1, Inners._FIVE _FIVE_1)
                {
                    this._SEVEN_1 = _SEVEN_1;
                    this._FIVE_1 = _FIVE_1;
                }
                
                public Inners._SEVEN _SEVEN_1 { get; }
                public Inners._FIVE _FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SEVENSIX : _percentxZEROONEⲻSEVENF
            {
                public _SEVENSIX(Inners._SEVEN _SEVEN_1, Inners._SIX _SIX_1)
                {
                    this._SEVEN_1 = _SEVEN_1;
                    this._SIX_1 = _SIX_1;
                }
                
                public Inners._SEVEN _SEVEN_1 { get; }
                public Inners._SIX _SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SEVENSEVEN : _percentxZEROONEⲻSEVENF
            {
                public _SEVENSEVEN(Inners._SEVEN _SEVEN_1, Inners._SEVEN _SEVEN_2)
                {
                    this._SEVEN_1 = _SEVEN_1;
                    this._SEVEN_2 = _SEVEN_2;
                }
                
                public Inners._SEVEN _SEVEN_1 { get; }
                public Inners._SEVEN _SEVEN_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SEVENEIGHT : _percentxZEROONEⲻSEVENF
            {
                public _SEVENEIGHT(Inners._SEVEN _SEVEN_1, Inners._EIGHT _EIGHT_1)
                {
                    this._SEVEN_1 = _SEVEN_1;
                    this._EIGHT_1 = _EIGHT_1;
                }
                
                public Inners._SEVEN _SEVEN_1 { get; }
                public Inners._EIGHT _EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SEVENNINE : _percentxZEROONEⲻSEVENF
            {
                public _SEVENNINE(Inners._SEVEN _SEVEN_1, Inners._NINE _NINE_1)
                {
                    this._SEVEN_1 = _SEVEN_1;
                    this._NINE_1 = _NINE_1;
                }
                
                public Inners._SEVEN _SEVEN_1 { get; }
                public Inners._NINE _NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SEVENA : _percentxZEROONEⲻSEVENF
            {
                public _SEVENA(Inners._SEVEN _SEVEN_1, Inners._A _A_1)
                {
                    this._SEVEN_1 = _SEVEN_1;
                    this._A_1 = _A_1;
                }
                
                public Inners._SEVEN _SEVEN_1 { get; }
                public Inners._A _A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SEVENB : _percentxZEROONEⲻSEVENF
            {
                public _SEVENB(Inners._SEVEN _SEVEN_1, Inners._B _B_1)
                {
                    this._SEVEN_1 = _SEVEN_1;
                    this._B_1 = _B_1;
                }
                
                public Inners._SEVEN _SEVEN_1 { get; }
                public Inners._B _B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SEVENC : _percentxZEROONEⲻSEVENF
            {
                public _SEVENC(Inners._SEVEN _SEVEN_1, Inners._C _C_1)
                {
                    this._SEVEN_1 = _SEVEN_1;
                    this._C_1 = _C_1;
                }
                
                public Inners._SEVEN _SEVEN_1 { get; }
                public Inners._C _C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SEVEND : _percentxZEROONEⲻSEVENF
            {
                public _SEVEND(Inners._SEVEN _SEVEN_1, Inners._D _D_1)
                {
                    this._SEVEN_1 = _SEVEN_1;
                    this._D_1 = _D_1;
                }
                
                public Inners._SEVEN _SEVEN_1 { get; }
                public Inners._D _D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SEVENE : _percentxZEROONEⲻSEVENF
            {
                public _SEVENE(Inners._SEVEN _SEVEN_1, Inners._E _E_1)
                {
                    this._SEVEN_1 = _SEVEN_1;
                    this._E_1 = _E_1;
                }
                
                public Inners._SEVEN _SEVEN_1 { get; }
                public Inners._E _E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SEVENF : _percentxZEROONEⲻSEVENF
            {
                public _SEVENF(Inners._SEVEN _SEVEN_1, Inners._F _F_1)
                {
                    this._SEVEN_1 = _SEVEN_1;
                    this._F_1 = _F_1;
                }
                
                public Inners._SEVEN _SEVEN_1 { get; }
                public Inners._F _F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
        }
        
        public sealed class _percentxZEROD
        {
            public _percentxZEROD(Inners._ZERO _ZERO_1, Inners._D _D_1)
            {
                this._ZERO_1 = _ZERO_1;
                this._D_1 = _D_1;
            }
            
            public Inners._ZERO _ZERO_1 { get; }
            public Inners._D _D_1 { get; }
        }
        
        public abstract class _percentxZEROZEROⲻONEF
        {
            private _percentxZEROZEROⲻONEF()
            {
            }
            
            protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
            
            public abstract class Visitor<TResult, TContext>
            {
                public TResult Visit(_percentxZEROZEROⲻONEF node, TContext context)
                {
                    return node.Dispatch(this, context);
                }
                
                protected internal abstract TResult Accept(_percentxZEROZEROⲻONEF._ZEROZERO node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻONEF._ZEROONE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻONEF._ZEROTWO node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻONEF._ZEROTHREE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻONEF._ZEROFOUR node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻONEF._ZEROFIVE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻONEF._ZEROSIX node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻONEF._ZEROSEVEN node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻONEF._ZEROEIGHT node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻONEF._ZERONINE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻONEF._ZEROA node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻONEF._ZEROB node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻONEF._ZEROC node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻONEF._ZEROD node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻONEF._ZEROE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻONEF._ZEROF node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻONEF._ONEZERO node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻONEF._ONEONE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻONEF._ONETWO node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻONEF._ONETHREE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻONEF._ONEFOUR node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻONEF._ONEFIVE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻONEF._ONESIX node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻONEF._ONESEVEN node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻONEF._ONEEIGHT node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻONEF._ONENINE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻONEF._ONEA node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻONEF._ONEB node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻONEF._ONEC node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻONEF._ONED node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻONEF._ONEE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻONEF._ONEF node, TContext context);
            }
            
            public sealed class _ZEROZERO : _percentxZEROZEROⲻONEF
            {
                public _ZEROZERO(Inners._ZERO _ZERO_1, Inners._ZERO _ZERO_2)
                {
                    this._ZERO_1 = _ZERO_1;
                    this._ZERO_2 = _ZERO_2;
                }
                
                public Inners._ZERO _ZERO_1 { get; }
                public Inners._ZERO _ZERO_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ZEROONE : _percentxZEROZEROⲻONEF
            {
                public _ZEROONE(Inners._ZERO _ZERO_1, Inners._ONE _ONE_1)
                {
                    this._ZERO_1 = _ZERO_1;
                    this._ONE_1 = _ONE_1;
                }
                
                public Inners._ZERO _ZERO_1 { get; }
                public Inners._ONE _ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ZEROTWO : _percentxZEROZEROⲻONEF
            {
                public _ZEROTWO(Inners._ZERO _ZERO_1, Inners._TWO _TWO_1)
                {
                    this._ZERO_1 = _ZERO_1;
                    this._TWO_1 = _TWO_1;
                }
                
                public Inners._ZERO _ZERO_1 { get; }
                public Inners._TWO _TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ZEROTHREE : _percentxZEROZEROⲻONEF
            {
                public _ZEROTHREE(Inners._ZERO _ZERO_1, Inners._THREE _THREE_1)
                {
                    this._ZERO_1 = _ZERO_1;
                    this._THREE_1 = _THREE_1;
                }
                
                public Inners._ZERO _ZERO_1 { get; }
                public Inners._THREE _THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ZEROFOUR : _percentxZEROZEROⲻONEF
            {
                public _ZEROFOUR(Inners._ZERO _ZERO_1, Inners._FOUR _FOUR_1)
                {
                    this._ZERO_1 = _ZERO_1;
                    this._FOUR_1 = _FOUR_1;
                }
                
                public Inners._ZERO _ZERO_1 { get; }
                public Inners._FOUR _FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ZEROFIVE : _percentxZEROZEROⲻONEF
            {
                public _ZEROFIVE(Inners._ZERO _ZERO_1, Inners._FIVE _FIVE_1)
                {
                    this._ZERO_1 = _ZERO_1;
                    this._FIVE_1 = _FIVE_1;
                }
                
                public Inners._ZERO _ZERO_1 { get; }
                public Inners._FIVE _FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ZEROSIX : _percentxZEROZEROⲻONEF
            {
                public _ZEROSIX(Inners._ZERO _ZERO_1, Inners._SIX _SIX_1)
                {
                    this._ZERO_1 = _ZERO_1;
                    this._SIX_1 = _SIX_1;
                }
                
                public Inners._ZERO _ZERO_1 { get; }
                public Inners._SIX _SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ZEROSEVEN : _percentxZEROZEROⲻONEF
            {
                public _ZEROSEVEN(Inners._ZERO _ZERO_1, Inners._SEVEN _SEVEN_1)
                {
                    this._ZERO_1 = _ZERO_1;
                    this._SEVEN_1 = _SEVEN_1;
                }
                
                public Inners._ZERO _ZERO_1 { get; }
                public Inners._SEVEN _SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ZEROEIGHT : _percentxZEROZEROⲻONEF
            {
                public _ZEROEIGHT(Inners._ZERO _ZERO_1, Inners._EIGHT _EIGHT_1)
                {
                    this._ZERO_1 = _ZERO_1;
                    this._EIGHT_1 = _EIGHT_1;
                }
                
                public Inners._ZERO _ZERO_1 { get; }
                public Inners._EIGHT _EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ZERONINE : _percentxZEROZEROⲻONEF
            {
                public _ZERONINE(Inners._ZERO _ZERO_1, Inners._NINE _NINE_1)
                {
                    this._ZERO_1 = _ZERO_1;
                    this._NINE_1 = _NINE_1;
                }
                
                public Inners._ZERO _ZERO_1 { get; }
                public Inners._NINE _NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ZEROA : _percentxZEROZEROⲻONEF
            {
                public _ZEROA(Inners._ZERO _ZERO_1, Inners._A _A_1)
                {
                    this._ZERO_1 = _ZERO_1;
                    this._A_1 = _A_1;
                }
                
                public Inners._ZERO _ZERO_1 { get; }
                public Inners._A _A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ZEROB : _percentxZEROZEROⲻONEF
            {
                public _ZEROB(Inners._ZERO _ZERO_1, Inners._B _B_1)
                {
                    this._ZERO_1 = _ZERO_1;
                    this._B_1 = _B_1;
                }
                
                public Inners._ZERO _ZERO_1 { get; }
                public Inners._B _B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ZEROC : _percentxZEROZEROⲻONEF
            {
                public _ZEROC(Inners._ZERO _ZERO_1, Inners._C _C_1)
                {
                    this._ZERO_1 = _ZERO_1;
                    this._C_1 = _C_1;
                }
                
                public Inners._ZERO _ZERO_1 { get; }
                public Inners._C _C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ZEROD : _percentxZEROZEROⲻONEF
            {
                public _ZEROD(Inners._ZERO _ZERO_1, Inners._D _D_1)
                {
                    this._ZERO_1 = _ZERO_1;
                    this._D_1 = _D_1;
                }
                
                public Inners._ZERO _ZERO_1 { get; }
                public Inners._D _D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ZEROE : _percentxZEROZEROⲻONEF
            {
                public _ZEROE(Inners._ZERO _ZERO_1, Inners._E _E_1)
                {
                    this._ZERO_1 = _ZERO_1;
                    this._E_1 = _E_1;
                }
                
                public Inners._ZERO _ZERO_1 { get; }
                public Inners._E _E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ZEROF : _percentxZEROZEROⲻONEF
            {
                public _ZEROF(Inners._ZERO _ZERO_1, Inners._F _F_1)
                {
                    this._ZERO_1 = _ZERO_1;
                    this._F_1 = _F_1;
                }
                
                public Inners._ZERO _ZERO_1 { get; }
                public Inners._F _F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ONEZERO : _percentxZEROZEROⲻONEF
            {
                public _ONEZERO(Inners._ONE _ONE_1, Inners._ZERO _ZERO_1)
                {
                    this._ONE_1 = _ONE_1;
                    this._ZERO_1 = _ZERO_1;
                }
                
                public Inners._ONE _ONE_1 { get; }
                public Inners._ZERO _ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ONEONE : _percentxZEROZEROⲻONEF
            {
                public _ONEONE(Inners._ONE _ONE_1, Inners._ONE _ONE_2)
                {
                    this._ONE_1 = _ONE_1;
                    this._ONE_2 = _ONE_2;
                }
                
                public Inners._ONE _ONE_1 { get; }
                public Inners._ONE _ONE_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ONETWO : _percentxZEROZEROⲻONEF
            {
                public _ONETWO(Inners._ONE _ONE_1, Inners._TWO _TWO_1)
                {
                    this._ONE_1 = _ONE_1;
                    this._TWO_1 = _TWO_1;
                }
                
                public Inners._ONE _ONE_1 { get; }
                public Inners._TWO _TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ONETHREE : _percentxZEROZEROⲻONEF
            {
                public _ONETHREE(Inners._ONE _ONE_1, Inners._THREE _THREE_1)
                {
                    this._ONE_1 = _ONE_1;
                    this._THREE_1 = _THREE_1;
                }
                
                public Inners._ONE _ONE_1 { get; }
                public Inners._THREE _THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ONEFOUR : _percentxZEROZEROⲻONEF
            {
                public _ONEFOUR(Inners._ONE _ONE_1, Inners._FOUR _FOUR_1)
                {
                    this._ONE_1 = _ONE_1;
                    this._FOUR_1 = _FOUR_1;
                }
                
                public Inners._ONE _ONE_1 { get; }
                public Inners._FOUR _FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ONEFIVE : _percentxZEROZEROⲻONEF
            {
                public _ONEFIVE(Inners._ONE _ONE_1, Inners._FIVE _FIVE_1)
                {
                    this._ONE_1 = _ONE_1;
                    this._FIVE_1 = _FIVE_1;
                }
                
                public Inners._ONE _ONE_1 { get; }
                public Inners._FIVE _FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ONESIX : _percentxZEROZEROⲻONEF
            {
                public _ONESIX(Inners._ONE _ONE_1, Inners._SIX _SIX_1)
                {
                    this._ONE_1 = _ONE_1;
                    this._SIX_1 = _SIX_1;
                }
                
                public Inners._ONE _ONE_1 { get; }
                public Inners._SIX _SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ONESEVEN : _percentxZEROZEROⲻONEF
            {
                public _ONESEVEN(Inners._ONE _ONE_1, Inners._SEVEN _SEVEN_1)
                {
                    this._ONE_1 = _ONE_1;
                    this._SEVEN_1 = _SEVEN_1;
                }
                
                public Inners._ONE _ONE_1 { get; }
                public Inners._SEVEN _SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ONEEIGHT : _percentxZEROZEROⲻONEF
            {
                public _ONEEIGHT(Inners._ONE _ONE_1, Inners._EIGHT _EIGHT_1)
                {
                    this._ONE_1 = _ONE_1;
                    this._EIGHT_1 = _EIGHT_1;
                }
                
                public Inners._ONE _ONE_1 { get; }
                public Inners._EIGHT _EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ONENINE : _percentxZEROZEROⲻONEF
            {
                public _ONENINE(Inners._ONE _ONE_1, Inners._NINE _NINE_1)
                {
                    this._ONE_1 = _ONE_1;
                    this._NINE_1 = _NINE_1;
                }
                
                public Inners._ONE _ONE_1 { get; }
                public Inners._NINE _NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ONEA : _percentxZEROZEROⲻONEF
            {
                public _ONEA(Inners._ONE _ONE_1, Inners._A _A_1)
                {
                    this._ONE_1 = _ONE_1;
                    this._A_1 = _A_1;
                }
                
                public Inners._ONE _ONE_1 { get; }
                public Inners._A _A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ONEB : _percentxZEROZEROⲻONEF
            {
                public _ONEB(Inners._ONE _ONE_1, Inners._B _B_1)
                {
                    this._ONE_1 = _ONE_1;
                    this._B_1 = _B_1;
                }
                
                public Inners._ONE _ONE_1 { get; }
                public Inners._B _B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ONEC : _percentxZEROZEROⲻONEF
            {
                public _ONEC(Inners._ONE _ONE_1, Inners._C _C_1)
                {
                    this._ONE_1 = _ONE_1;
                    this._C_1 = _C_1;
                }
                
                public Inners._ONE _ONE_1 { get; }
                public Inners._C _C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ONED : _percentxZEROZEROⲻONEF
            {
                public _ONED(Inners._ONE _ONE_1, Inners._D _D_1)
                {
                    this._ONE_1 = _ONE_1;
                    this._D_1 = _D_1;
                }
                
                public Inners._ONE _ONE_1 { get; }
                public Inners._D _D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ONEE : _percentxZEROZEROⲻONEF
            {
                public _ONEE(Inners._ONE _ONE_1, Inners._E _E_1)
                {
                    this._ONE_1 = _ONE_1;
                    this._E_1 = _E_1;
                }
                
                public Inners._ONE _ONE_1 { get; }
                public Inners._E _E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ONEF : _percentxZEROZEROⲻONEF
            {
                public _ONEF(Inners._ONE _ONE_1, Inners._F _F_1)
                {
                    this._ONE_1 = _ONE_1;
                    this._F_1 = _F_1;
                }
                
                public Inners._ONE _ONE_1 { get; }
                public Inners._F _F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
        }
        
        public sealed class _percentxSEVENF
        {
            public _percentxSEVENF(Inners._SEVEN _SEVEN_1, Inners._F _F_1)
            {
                this._SEVEN_1 = _SEVEN_1;
                this._F_1 = _F_1;
            }
            
            public Inners._SEVEN _SEVEN_1 { get; }
            public Inners._F _F_1 { get; }
        }
        
        public abstract class _percentxTHREEZEROⲻTHREENINE
        {
            private _percentxTHREEZEROⲻTHREENINE()
            {
            }
            
            protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
            
            public abstract class Visitor<TResult, TContext>
            {
                public TResult Visit(_percentxTHREEZEROⲻTHREENINE node, TContext context)
                {
                    return node.Dispatch(this, context);
                }
                
                protected internal abstract TResult Accept(_percentxTHREEZEROⲻTHREENINE._THREEZERO node, TContext context);
                protected internal abstract TResult Accept(_percentxTHREEZEROⲻTHREENINE._THREEONE node, TContext context);
                protected internal abstract TResult Accept(_percentxTHREEZEROⲻTHREENINE._THREETWO node, TContext context);
                protected internal abstract TResult Accept(_percentxTHREEZEROⲻTHREENINE._THREETHREE node, TContext context);
                protected internal abstract TResult Accept(_percentxTHREEZEROⲻTHREENINE._THREEFOUR node, TContext context);
                protected internal abstract TResult Accept(_percentxTHREEZEROⲻTHREENINE._THREEFIVE node, TContext context);
                protected internal abstract TResult Accept(_percentxTHREEZEROⲻTHREENINE._THREESIX node, TContext context);
                protected internal abstract TResult Accept(_percentxTHREEZEROⲻTHREENINE._THREESEVEN node, TContext context);
                protected internal abstract TResult Accept(_percentxTHREEZEROⲻTHREENINE._THREEEIGHT node, TContext context);
                protected internal abstract TResult Accept(_percentxTHREEZEROⲻTHREENINE._THREENINE node, TContext context);
            }
            
            public sealed class _THREEZERO : _percentxTHREEZEROⲻTHREENINE
            {
                public _THREEZERO(Inners._THREE _THREE_1, Inners._ZERO _ZERO_1)
                {
                    this._THREE_1 = _THREE_1;
                    this._ZERO_1 = _ZERO_1;
                }
                
                public Inners._THREE _THREE_1 { get; }
                public Inners._ZERO _ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _THREEONE : _percentxTHREEZEROⲻTHREENINE
            {
                public _THREEONE(Inners._THREE _THREE_1, Inners._ONE _ONE_1)
                {
                    this._THREE_1 = _THREE_1;
                    this._ONE_1 = _ONE_1;
                }
                
                public Inners._THREE _THREE_1 { get; }
                public Inners._ONE _ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _THREETWO : _percentxTHREEZEROⲻTHREENINE
            {
                public _THREETWO(Inners._THREE _THREE_1, Inners._TWO _TWO_1)
                {
                    this._THREE_1 = _THREE_1;
                    this._TWO_1 = _TWO_1;
                }
                
                public Inners._THREE _THREE_1 { get; }
                public Inners._TWO _TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _THREETHREE : _percentxTHREEZEROⲻTHREENINE
            {
                public _THREETHREE(Inners._THREE _THREE_1, Inners._THREE _THREE_2)
                {
                    this._THREE_1 = _THREE_1;
                    this._THREE_2 = _THREE_2;
                }
                
                public Inners._THREE _THREE_1 { get; }
                public Inners._THREE _THREE_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _THREEFOUR : _percentxTHREEZEROⲻTHREENINE
            {
                public _THREEFOUR(Inners._THREE _THREE_1, Inners._FOUR _FOUR_1)
                {
                    this._THREE_1 = _THREE_1;
                    this._FOUR_1 = _FOUR_1;
                }
                
                public Inners._THREE _THREE_1 { get; }
                public Inners._FOUR _FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _THREEFIVE : _percentxTHREEZEROⲻTHREENINE
            {
                public _THREEFIVE(Inners._THREE _THREE_1, Inners._FIVE _FIVE_1)
                {
                    this._THREE_1 = _THREE_1;
                    this._FIVE_1 = _FIVE_1;
                }
                
                public Inners._THREE _THREE_1 { get; }
                public Inners._FIVE _FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _THREESIX : _percentxTHREEZEROⲻTHREENINE
            {
                public _THREESIX(Inners._THREE _THREE_1, Inners._SIX _SIX_1)
                {
                    this._THREE_1 = _THREE_1;
                    this._SIX_1 = _SIX_1;
                }
                
                public Inners._THREE _THREE_1 { get; }
                public Inners._SIX _SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _THREESEVEN : _percentxTHREEZEROⲻTHREENINE
            {
                public _THREESEVEN(Inners._THREE _THREE_1, Inners._SEVEN _SEVEN_1)
                {
                    this._THREE_1 = _THREE_1;
                    this._SEVEN_1 = _SEVEN_1;
                }
                
                public Inners._THREE _THREE_1 { get; }
                public Inners._SEVEN _SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _THREEEIGHT : _percentxTHREEZEROⲻTHREENINE
            {
                public _THREEEIGHT(Inners._THREE _THREE_1, Inners._EIGHT _EIGHT_1)
                {
                    this._THREE_1 = _THREE_1;
                    this._EIGHT_1 = _EIGHT_1;
                }
                
                public Inners._THREE _THREE_1 { get; }
                public Inners._EIGHT _EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _THREENINE : _percentxTHREEZEROⲻTHREENINE
            {
                public _THREENINE(Inners._THREE _THREE_1, Inners._NINE _NINE_1)
                {
                    this._THREE_1 = _THREE_1;
                    this._NINE_1 = _NINE_1;
                }
                
                public Inners._THREE _THREE_1 { get; }
                public Inners._NINE _NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
        }
        
        public sealed class _percentxTWOTWO
        {
            public _percentxTWOTWO(Inners._TWO _TWO_1, Inners._TWO _TWO_2)
            {
                this._TWO_1 = _TWO_1;
                this._TWO_2 = _TWO_2;
            }
            
            public Inners._TWO _TWO_1 { get; }
            public Inners._TWO _TWO_2 { get; }
        }
        
        public sealed class _x41
        {
            private _x41()
            {
            }
            
            public static _x41 Instance { get; } = new _x41();
        }
        
        public sealed class _doublequotex41doublequote
        {
            public _doublequotex41doublequote(Inners._x41 _x41_1)
            {
                this._x41_1 = _x41_1;
            }
            
            public Inners._x41 _x41_1 { get; }
        }
        
        public sealed class _x42
        {
            private _x42()
            {
            }
            
            public static _x42 Instance { get; } = new _x42();
        }
        
        public sealed class _doublequotex42doublequote
        {
            public _doublequotex42doublequote(Inners._x42 _x42_1)
            {
                this._x42_1 = _x42_1;
            }
            
            public Inners._x42 _x42_1 { get; }
        }
        
        public sealed class _x43
        {
            private _x43()
            {
            }
            
            public static _x43 Instance { get; } = new _x43();
        }
        
        public sealed class _doublequotex43doublequote
        {
            public _doublequotex43doublequote(Inners._x43 _x43_1)
            {
                this._x43_1 = _x43_1;
            }
            
            public Inners._x43 _x43_1 { get; }
        }
        
        public sealed class _x44
        {
            private _x44()
            {
            }
            
            public static _x44 Instance { get; } = new _x44();
        }
        
        public sealed class _doublequotex44doublequote
        {
            public _doublequotex44doublequote(Inners._x44 _x44_1)
            {
                this._x44_1 = _x44_1;
            }
            
            public Inners._x44 _x44_1 { get; }
        }
        
        public sealed class _x45
        {
            private _x45()
            {
            }
            
            public static _x45 Instance { get; } = new _x45();
        }
        
        public sealed class _doublequotex45doublequote
        {
            public _doublequotex45doublequote(Inners._x45 _x45_1)
            {
                this._x45_1 = _x45_1;
            }
            
            public Inners._x45 _x45_1 { get; }
        }
        
        public sealed class _x46
        {
            private _x46()
            {
            }
            
            public static _x46 Instance { get; } = new _x46();
        }
        
        public sealed class _doublequotex46doublequote
        {
            public _doublequotex46doublequote(Inners._x46 _x46_1)
            {
                this._x46_1 = _x46_1;
            }
            
            public Inners._x46 _x46_1 { get; }
        }
        
        public sealed class _percentxZERONINE
        {
            public _percentxZERONINE(Inners._ZERO _ZERO_1, Inners._NINE _NINE_1)
            {
                this._ZERO_1 = _ZERO_1;
                this._NINE_1 = _NINE_1;
            }
            
            public Inners._ZERO _ZERO_1 { get; }
            public Inners._NINE _NINE_1 { get; }
        }
        
        public sealed class _percentxZEROA
        {
            public _percentxZEROA(Inners._ZERO _ZERO_1, Inners._A _A_1)
            {
                this._ZERO_1 = _ZERO_1;
                this._A_1 = _A_1;
            }
            
            public Inners._ZERO _ZERO_1 { get; }
            public Inners._A _A_1 { get; }
        }
        
        public abstract class _WSPⳆCRLF_WSP
        {
            private _WSPⳆCRLF_WSP()
            {
            }
            
            protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
            
            public abstract class Visitor<TResult, TContext>
            {
                public TResult Visit(_WSPⳆCRLF_WSP node, TContext context)
                {
                    return node.Dispatch(this, context);
                }
                
                protected internal abstract TResult Accept(_WSPⳆCRLF_WSP._WSP node, TContext context);
                protected internal abstract TResult Accept(_WSPⳆCRLF_WSP._CRLF_WSP node, TContext context);
            }
            
            public sealed class _WSP : _WSPⳆCRLF_WSP
            {
                public _WSP(GeneratorV3.Core._WSP _WSP_1)
                {
                    this._WSP_1 = _WSP_1;
                }
                
                public GeneratorV3.Core._WSP _WSP_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _CRLF_WSP : _WSPⳆCRLF_WSP
            {
                public _CRLF_WSP(GeneratorV3.Core._CRLF _CRLF_1, GeneratorV3.Core._WSP _WSP_1)
                {
                    this._CRLF_1 = _CRLF_1;
                    this._WSP_1 = _WSP_1;
                }
                
                public GeneratorV3.Core._CRLF _CRLF_1 { get; }
                public GeneratorV3.Core._WSP _WSP_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
        }
        
        public sealed class _openWSPⳆCRLF_WSPↃ
        {
            public _openWSPⳆCRLF_WSPↃ(Inners._WSPⳆCRLF_WSP _WSPⳆCRLF_WSP_1)
            {
                this._WSPⳆCRLF_WSP_1 = _WSPⳆCRLF_WSP_1;
            }
            
            public Inners._WSPⳆCRLF_WSP _WSPⳆCRLF_WSP_1 { get; }
        }
        
        public abstract class _percentxZEROZEROⲻFF
        {
            private _percentxZEROZEROⲻFF()
            {
            }
            
            protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
            
            public abstract class Visitor<TResult, TContext>
            {
                public TResult Visit(_percentxZEROZEROⲻFF node, TContext context)
                {
                    return node.Dispatch(this, context);
                }
                
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._ZEROZERO node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._ZEROONE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._ZEROTWO node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._ZEROTHREE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._ZEROFOUR node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._ZEROFIVE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._ZEROSIX node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._ZEROSEVEN node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._ZEROEIGHT node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._ZERONINE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._ZEROA node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._ZEROB node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._ZEROC node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._ZEROD node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._ZEROE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._ZEROF node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._ONEZERO node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._ONEONE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._ONETWO node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._ONETHREE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._ONEFOUR node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._ONEFIVE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._ONESIX node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._ONESEVEN node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._ONEEIGHT node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._ONENINE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._ONEA node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._ONEB node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._ONEC node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._ONED node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._ONEE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._ONEF node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._TWOZERO node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._TWOONE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._TWOTWO node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._TWOTHREE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._TWOFOUR node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._TWOFIVE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._TWOSIX node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._TWOSEVEN node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._TWOEIGHT node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._TWONINE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._TWOA node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._TWOB node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._TWOC node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._TWOD node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._TWOE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._TWOF node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._THREEZERO node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._THREEONE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._THREETWO node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._THREETHREE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._THREEFOUR node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._THREEFIVE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._THREESIX node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._THREESEVEN node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._THREEEIGHT node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._THREENINE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._THREEA node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._THREEB node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._THREEC node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._THREED node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._THREEE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._THREEF node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._FOURZERO node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._FOURONE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._FOURTWO node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._FOURTHREE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._FOURFOUR node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._FOURFIVE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._FOURSIX node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._FOURSEVEN node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._FOUREIGHT node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._FOURNINE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._FOURA node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._FOURB node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._FOURC node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._FOURD node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._FOURE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._FOURF node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._FIVEZERO node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._FIVEONE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._FIVETWO node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._FIVETHREE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._FIVEFOUR node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._FIVEFIVE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._FIVESIX node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._FIVESEVEN node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._FIVEEIGHT node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._FIVENINE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._FIVEA node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._FIVEB node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._FIVEC node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._FIVED node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._FIVEE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._FIVEF node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._SIXZERO node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._SIXONE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._SIXTWO node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._SIXTHREE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._SIXFOUR node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._SIXFIVE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._SIXSIX node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._SIXSEVEN node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._SIXEIGHT node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._SIXNINE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._SIXA node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._SIXB node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._SIXC node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._SIXD node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._SIXE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._SIXF node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._SEVENZERO node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._SEVENONE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._SEVENTWO node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._SEVENTHREE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._SEVENFOUR node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._SEVENFIVE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._SEVENSIX node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._SEVENSEVEN node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._SEVENEIGHT node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._SEVENNINE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._SEVENA node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._SEVENB node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._SEVENC node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._SEVEND node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._SEVENE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._SEVENF node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._EIGHTZERO node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._EIGHTONE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._EIGHTTWO node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._EIGHTTHREE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._EIGHTFOUR node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._EIGHTFIVE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._EIGHTSIX node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._EIGHTSEVEN node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._EIGHTEIGHT node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._EIGHTNINE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._EIGHTA node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._EIGHTB node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._EIGHTC node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._EIGHTD node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._EIGHTE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._EIGHTF node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._NINEZERO node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._NINEONE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._NINETWO node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._NINETHREE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._NINEFOUR node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._NINEFIVE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._NINESIX node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._NINESEVEN node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._NINEEIGHT node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._NINENINE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._NINEA node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._NINEB node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._NINEC node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._NINED node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._NINEE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._NINEF node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._AZERO node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._AONE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._ATWO node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._ATHREE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._AFOUR node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._AFIVE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._ASIX node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._ASEVEN node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._AEIGHT node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._ANINE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._AA node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._AB node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._AC node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._AD node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._AE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._AF node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._BZERO node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._BONE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._BTWO node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._BTHREE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._BFOUR node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._BFIVE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._BSIX node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._BSEVEN node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._BEIGHT node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._BNINE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._BA node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._BB node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._BC node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._BD node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._BE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._BF node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._CZERO node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._CONE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._CTWO node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._CTHREE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._CFOUR node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._CFIVE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._CSIX node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._CSEVEN node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._CEIGHT node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._CNINE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._CA node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._CB node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._CC node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._CD node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._CE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._CF node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._DZERO node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._DONE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._DTWO node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._DTHREE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._DFOUR node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._DFIVE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._DSIX node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._DSEVEN node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._DEIGHT node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._DNINE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._DA node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._DB node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._DC node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._DD node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._DE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._DF node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._EZERO node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._EONE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._ETWO node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._ETHREE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._EFOUR node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._EFIVE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._ESIX node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._ESEVEN node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._EEIGHT node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._ENINE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._EA node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._EB node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._EC node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._ED node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._EE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._EF node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._FZERO node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._FONE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._FTWO node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._FTHREE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._FFOUR node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._FFIVE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._FSIX node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._FSEVEN node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._FEIGHT node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._FNINE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._FA node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._FB node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._FC node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._FD node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._FE node, TContext context);
                protected internal abstract TResult Accept(_percentxZEROZEROⲻFF._FF node, TContext context);
            }
            
            public sealed class _ZEROZERO : _percentxZEROZEROⲻFF
            {
                public _ZEROZERO(Inners._ZERO _ZERO_1, Inners._ZERO _ZERO_2)
                {
                    this._ZERO_1 = _ZERO_1;
                    this._ZERO_2 = _ZERO_2;
                }
                
                public Inners._ZERO _ZERO_1 { get; }
                public Inners._ZERO _ZERO_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ZEROONE : _percentxZEROZEROⲻFF
            {
                public _ZEROONE(Inners._ZERO _ZERO_1, Inners._ONE _ONE_1)
                {
                    this._ZERO_1 = _ZERO_1;
                    this._ONE_1 = _ONE_1;
                }
                
                public Inners._ZERO _ZERO_1 { get; }
                public Inners._ONE _ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ZEROTWO : _percentxZEROZEROⲻFF
            {
                public _ZEROTWO(Inners._ZERO _ZERO_1, Inners._TWO _TWO_1)
                {
                    this._ZERO_1 = _ZERO_1;
                    this._TWO_1 = _TWO_1;
                }
                
                public Inners._ZERO _ZERO_1 { get; }
                public Inners._TWO _TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ZEROTHREE : _percentxZEROZEROⲻFF
            {
                public _ZEROTHREE(Inners._ZERO _ZERO_1, Inners._THREE _THREE_1)
                {
                    this._ZERO_1 = _ZERO_1;
                    this._THREE_1 = _THREE_1;
                }
                
                public Inners._ZERO _ZERO_1 { get; }
                public Inners._THREE _THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ZEROFOUR : _percentxZEROZEROⲻFF
            {
                public _ZEROFOUR(Inners._ZERO _ZERO_1, Inners._FOUR _FOUR_1)
                {
                    this._ZERO_1 = _ZERO_1;
                    this._FOUR_1 = _FOUR_1;
                }
                
                public Inners._ZERO _ZERO_1 { get; }
                public Inners._FOUR _FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ZEROFIVE : _percentxZEROZEROⲻFF
            {
                public _ZEROFIVE(Inners._ZERO _ZERO_1, Inners._FIVE _FIVE_1)
                {
                    this._ZERO_1 = _ZERO_1;
                    this._FIVE_1 = _FIVE_1;
                }
                
                public Inners._ZERO _ZERO_1 { get; }
                public Inners._FIVE _FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ZEROSIX : _percentxZEROZEROⲻFF
            {
                public _ZEROSIX(Inners._ZERO _ZERO_1, Inners._SIX _SIX_1)
                {
                    this._ZERO_1 = _ZERO_1;
                    this._SIX_1 = _SIX_1;
                }
                
                public Inners._ZERO _ZERO_1 { get; }
                public Inners._SIX _SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ZEROSEVEN : _percentxZEROZEROⲻFF
            {
                public _ZEROSEVEN(Inners._ZERO _ZERO_1, Inners._SEVEN _SEVEN_1)
                {
                    this._ZERO_1 = _ZERO_1;
                    this._SEVEN_1 = _SEVEN_1;
                }
                
                public Inners._ZERO _ZERO_1 { get; }
                public Inners._SEVEN _SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ZEROEIGHT : _percentxZEROZEROⲻFF
            {
                public _ZEROEIGHT(Inners._ZERO _ZERO_1, Inners._EIGHT _EIGHT_1)
                {
                    this._ZERO_1 = _ZERO_1;
                    this._EIGHT_1 = _EIGHT_1;
                }
                
                public Inners._ZERO _ZERO_1 { get; }
                public Inners._EIGHT _EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ZERONINE : _percentxZEROZEROⲻFF
            {
                public _ZERONINE(Inners._ZERO _ZERO_1, Inners._NINE _NINE_1)
                {
                    this._ZERO_1 = _ZERO_1;
                    this._NINE_1 = _NINE_1;
                }
                
                public Inners._ZERO _ZERO_1 { get; }
                public Inners._NINE _NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ZEROA : _percentxZEROZEROⲻFF
            {
                public _ZEROA(Inners._ZERO _ZERO_1, Inners._A _A_1)
                {
                    this._ZERO_1 = _ZERO_1;
                    this._A_1 = _A_1;
                }
                
                public Inners._ZERO _ZERO_1 { get; }
                public Inners._A _A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ZEROB : _percentxZEROZEROⲻFF
            {
                public _ZEROB(Inners._ZERO _ZERO_1, Inners._B _B_1)
                {
                    this._ZERO_1 = _ZERO_1;
                    this._B_1 = _B_1;
                }
                
                public Inners._ZERO _ZERO_1 { get; }
                public Inners._B _B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ZEROC : _percentxZEROZEROⲻFF
            {
                public _ZEROC(Inners._ZERO _ZERO_1, Inners._C _C_1)
                {
                    this._ZERO_1 = _ZERO_1;
                    this._C_1 = _C_1;
                }
                
                public Inners._ZERO _ZERO_1 { get; }
                public Inners._C _C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ZEROD : _percentxZEROZEROⲻFF
            {
                public _ZEROD(Inners._ZERO _ZERO_1, Inners._D _D_1)
                {
                    this._ZERO_1 = _ZERO_1;
                    this._D_1 = _D_1;
                }
                
                public Inners._ZERO _ZERO_1 { get; }
                public Inners._D _D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ZEROE : _percentxZEROZEROⲻFF
            {
                public _ZEROE(Inners._ZERO _ZERO_1, Inners._E _E_1)
                {
                    this._ZERO_1 = _ZERO_1;
                    this._E_1 = _E_1;
                }
                
                public Inners._ZERO _ZERO_1 { get; }
                public Inners._E _E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ZEROF : _percentxZEROZEROⲻFF
            {
                public _ZEROF(Inners._ZERO _ZERO_1, Inners._F _F_1)
                {
                    this._ZERO_1 = _ZERO_1;
                    this._F_1 = _F_1;
                }
                
                public Inners._ZERO _ZERO_1 { get; }
                public Inners._F _F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ONEZERO : _percentxZEROZEROⲻFF
            {
                public _ONEZERO(Inners._ONE _ONE_1, Inners._ZERO _ZERO_1)
                {
                    this._ONE_1 = _ONE_1;
                    this._ZERO_1 = _ZERO_1;
                }
                
                public Inners._ONE _ONE_1 { get; }
                public Inners._ZERO _ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ONEONE : _percentxZEROZEROⲻFF
            {
                public _ONEONE(Inners._ONE _ONE_1, Inners._ONE _ONE_2)
                {
                    this._ONE_1 = _ONE_1;
                    this._ONE_2 = _ONE_2;
                }
                
                public Inners._ONE _ONE_1 { get; }
                public Inners._ONE _ONE_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ONETWO : _percentxZEROZEROⲻFF
            {
                public _ONETWO(Inners._ONE _ONE_1, Inners._TWO _TWO_1)
                {
                    this._ONE_1 = _ONE_1;
                    this._TWO_1 = _TWO_1;
                }
                
                public Inners._ONE _ONE_1 { get; }
                public Inners._TWO _TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ONETHREE : _percentxZEROZEROⲻFF
            {
                public _ONETHREE(Inners._ONE _ONE_1, Inners._THREE _THREE_1)
                {
                    this._ONE_1 = _ONE_1;
                    this._THREE_1 = _THREE_1;
                }
                
                public Inners._ONE _ONE_1 { get; }
                public Inners._THREE _THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ONEFOUR : _percentxZEROZEROⲻFF
            {
                public _ONEFOUR(Inners._ONE _ONE_1, Inners._FOUR _FOUR_1)
                {
                    this._ONE_1 = _ONE_1;
                    this._FOUR_1 = _FOUR_1;
                }
                
                public Inners._ONE _ONE_1 { get; }
                public Inners._FOUR _FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ONEFIVE : _percentxZEROZEROⲻFF
            {
                public _ONEFIVE(Inners._ONE _ONE_1, Inners._FIVE _FIVE_1)
                {
                    this._ONE_1 = _ONE_1;
                    this._FIVE_1 = _FIVE_1;
                }
                
                public Inners._ONE _ONE_1 { get; }
                public Inners._FIVE _FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ONESIX : _percentxZEROZEROⲻFF
            {
                public _ONESIX(Inners._ONE _ONE_1, Inners._SIX _SIX_1)
                {
                    this._ONE_1 = _ONE_1;
                    this._SIX_1 = _SIX_1;
                }
                
                public Inners._ONE _ONE_1 { get; }
                public Inners._SIX _SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ONESEVEN : _percentxZEROZEROⲻFF
            {
                public _ONESEVEN(Inners._ONE _ONE_1, Inners._SEVEN _SEVEN_1)
                {
                    this._ONE_1 = _ONE_1;
                    this._SEVEN_1 = _SEVEN_1;
                }
                
                public Inners._ONE _ONE_1 { get; }
                public Inners._SEVEN _SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ONEEIGHT : _percentxZEROZEROⲻFF
            {
                public _ONEEIGHT(Inners._ONE _ONE_1, Inners._EIGHT _EIGHT_1)
                {
                    this._ONE_1 = _ONE_1;
                    this._EIGHT_1 = _EIGHT_1;
                }
                
                public Inners._ONE _ONE_1 { get; }
                public Inners._EIGHT _EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ONENINE : _percentxZEROZEROⲻFF
            {
                public _ONENINE(Inners._ONE _ONE_1, Inners._NINE _NINE_1)
                {
                    this._ONE_1 = _ONE_1;
                    this._NINE_1 = _NINE_1;
                }
                
                public Inners._ONE _ONE_1 { get; }
                public Inners._NINE _NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ONEA : _percentxZEROZEROⲻFF
            {
                public _ONEA(Inners._ONE _ONE_1, Inners._A _A_1)
                {
                    this._ONE_1 = _ONE_1;
                    this._A_1 = _A_1;
                }
                
                public Inners._ONE _ONE_1 { get; }
                public Inners._A _A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ONEB : _percentxZEROZEROⲻFF
            {
                public _ONEB(Inners._ONE _ONE_1, Inners._B _B_1)
                {
                    this._ONE_1 = _ONE_1;
                    this._B_1 = _B_1;
                }
                
                public Inners._ONE _ONE_1 { get; }
                public Inners._B _B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ONEC : _percentxZEROZEROⲻFF
            {
                public _ONEC(Inners._ONE _ONE_1, Inners._C _C_1)
                {
                    this._ONE_1 = _ONE_1;
                    this._C_1 = _C_1;
                }
                
                public Inners._ONE _ONE_1 { get; }
                public Inners._C _C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ONED : _percentxZEROZEROⲻFF
            {
                public _ONED(Inners._ONE _ONE_1, Inners._D _D_1)
                {
                    this._ONE_1 = _ONE_1;
                    this._D_1 = _D_1;
                }
                
                public Inners._ONE _ONE_1 { get; }
                public Inners._D _D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ONEE : _percentxZEROZEROⲻFF
            {
                public _ONEE(Inners._ONE _ONE_1, Inners._E _E_1)
                {
                    this._ONE_1 = _ONE_1;
                    this._E_1 = _E_1;
                }
                
                public Inners._ONE _ONE_1 { get; }
                public Inners._E _E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ONEF : _percentxZEROZEROⲻFF
            {
                public _ONEF(Inners._ONE _ONE_1, Inners._F _F_1)
                {
                    this._ONE_1 = _ONE_1;
                    this._F_1 = _F_1;
                }
                
                public Inners._ONE _ONE_1 { get; }
                public Inners._F _F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _TWOZERO : _percentxZEROZEROⲻFF
            {
                public _TWOZERO(Inners._TWO _TWO_1, Inners._ZERO _ZERO_1)
                {
                    this._TWO_1 = _TWO_1;
                    this._ZERO_1 = _ZERO_1;
                }
                
                public Inners._TWO _TWO_1 { get; }
                public Inners._ZERO _ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _TWOONE : _percentxZEROZEROⲻFF
            {
                public _TWOONE(Inners._TWO _TWO_1, Inners._ONE _ONE_1)
                {
                    this._TWO_1 = _TWO_1;
                    this._ONE_1 = _ONE_1;
                }
                
                public Inners._TWO _TWO_1 { get; }
                public Inners._ONE _ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _TWOTWO : _percentxZEROZEROⲻFF
            {
                public _TWOTWO(Inners._TWO _TWO_1, Inners._TWO _TWO_2)
                {
                    this._TWO_1 = _TWO_1;
                    this._TWO_2 = _TWO_2;
                }
                
                public Inners._TWO _TWO_1 { get; }
                public Inners._TWO _TWO_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _TWOTHREE : _percentxZEROZEROⲻFF
            {
                public _TWOTHREE(Inners._TWO _TWO_1, Inners._THREE _THREE_1)
                {
                    this._TWO_1 = _TWO_1;
                    this._THREE_1 = _THREE_1;
                }
                
                public Inners._TWO _TWO_1 { get; }
                public Inners._THREE _THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _TWOFOUR : _percentxZEROZEROⲻFF
            {
                public _TWOFOUR(Inners._TWO _TWO_1, Inners._FOUR _FOUR_1)
                {
                    this._TWO_1 = _TWO_1;
                    this._FOUR_1 = _FOUR_1;
                }
                
                public Inners._TWO _TWO_1 { get; }
                public Inners._FOUR _FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _TWOFIVE : _percentxZEROZEROⲻFF
            {
                public _TWOFIVE(Inners._TWO _TWO_1, Inners._FIVE _FIVE_1)
                {
                    this._TWO_1 = _TWO_1;
                    this._FIVE_1 = _FIVE_1;
                }
                
                public Inners._TWO _TWO_1 { get; }
                public Inners._FIVE _FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _TWOSIX : _percentxZEROZEROⲻFF
            {
                public _TWOSIX(Inners._TWO _TWO_1, Inners._SIX _SIX_1)
                {
                    this._TWO_1 = _TWO_1;
                    this._SIX_1 = _SIX_1;
                }
                
                public Inners._TWO _TWO_1 { get; }
                public Inners._SIX _SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _TWOSEVEN : _percentxZEROZEROⲻFF
            {
                public _TWOSEVEN(Inners._TWO _TWO_1, Inners._SEVEN _SEVEN_1)
                {
                    this._TWO_1 = _TWO_1;
                    this._SEVEN_1 = _SEVEN_1;
                }
                
                public Inners._TWO _TWO_1 { get; }
                public Inners._SEVEN _SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _TWOEIGHT : _percentxZEROZEROⲻFF
            {
                public _TWOEIGHT(Inners._TWO _TWO_1, Inners._EIGHT _EIGHT_1)
                {
                    this._TWO_1 = _TWO_1;
                    this._EIGHT_1 = _EIGHT_1;
                }
                
                public Inners._TWO _TWO_1 { get; }
                public Inners._EIGHT _EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _TWONINE : _percentxZEROZEROⲻFF
            {
                public _TWONINE(Inners._TWO _TWO_1, Inners._NINE _NINE_1)
                {
                    this._TWO_1 = _TWO_1;
                    this._NINE_1 = _NINE_1;
                }
                
                public Inners._TWO _TWO_1 { get; }
                public Inners._NINE _NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _TWOA : _percentxZEROZEROⲻFF
            {
                public _TWOA(Inners._TWO _TWO_1, Inners._A _A_1)
                {
                    this._TWO_1 = _TWO_1;
                    this._A_1 = _A_1;
                }
                
                public Inners._TWO _TWO_1 { get; }
                public Inners._A _A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _TWOB : _percentxZEROZEROⲻFF
            {
                public _TWOB(Inners._TWO _TWO_1, Inners._B _B_1)
                {
                    this._TWO_1 = _TWO_1;
                    this._B_1 = _B_1;
                }
                
                public Inners._TWO _TWO_1 { get; }
                public Inners._B _B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _TWOC : _percentxZEROZEROⲻFF
            {
                public _TWOC(Inners._TWO _TWO_1, Inners._C _C_1)
                {
                    this._TWO_1 = _TWO_1;
                    this._C_1 = _C_1;
                }
                
                public Inners._TWO _TWO_1 { get; }
                public Inners._C _C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _TWOD : _percentxZEROZEROⲻFF
            {
                public _TWOD(Inners._TWO _TWO_1, Inners._D _D_1)
                {
                    this._TWO_1 = _TWO_1;
                    this._D_1 = _D_1;
                }
                
                public Inners._TWO _TWO_1 { get; }
                public Inners._D _D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _TWOE : _percentxZEROZEROⲻFF
            {
                public _TWOE(Inners._TWO _TWO_1, Inners._E _E_1)
                {
                    this._TWO_1 = _TWO_1;
                    this._E_1 = _E_1;
                }
                
                public Inners._TWO _TWO_1 { get; }
                public Inners._E _E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _TWOF : _percentxZEROZEROⲻFF
            {
                public _TWOF(Inners._TWO _TWO_1, Inners._F _F_1)
                {
                    this._TWO_1 = _TWO_1;
                    this._F_1 = _F_1;
                }
                
                public Inners._TWO _TWO_1 { get; }
                public Inners._F _F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _THREEZERO : _percentxZEROZEROⲻFF
            {
                public _THREEZERO(Inners._THREE _THREE_1, Inners._ZERO _ZERO_1)
                {
                    this._THREE_1 = _THREE_1;
                    this._ZERO_1 = _ZERO_1;
                }
                
                public Inners._THREE _THREE_1 { get; }
                public Inners._ZERO _ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _THREEONE : _percentxZEROZEROⲻFF
            {
                public _THREEONE(Inners._THREE _THREE_1, Inners._ONE _ONE_1)
                {
                    this._THREE_1 = _THREE_1;
                    this._ONE_1 = _ONE_1;
                }
                
                public Inners._THREE _THREE_1 { get; }
                public Inners._ONE _ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _THREETWO : _percentxZEROZEROⲻFF
            {
                public _THREETWO(Inners._THREE _THREE_1, Inners._TWO _TWO_1)
                {
                    this._THREE_1 = _THREE_1;
                    this._TWO_1 = _TWO_1;
                }
                
                public Inners._THREE _THREE_1 { get; }
                public Inners._TWO _TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _THREETHREE : _percentxZEROZEROⲻFF
            {
                public _THREETHREE(Inners._THREE _THREE_1, Inners._THREE _THREE_2)
                {
                    this._THREE_1 = _THREE_1;
                    this._THREE_2 = _THREE_2;
                }
                
                public Inners._THREE _THREE_1 { get; }
                public Inners._THREE _THREE_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _THREEFOUR : _percentxZEROZEROⲻFF
            {
                public _THREEFOUR(Inners._THREE _THREE_1, Inners._FOUR _FOUR_1)
                {
                    this._THREE_1 = _THREE_1;
                    this._FOUR_1 = _FOUR_1;
                }
                
                public Inners._THREE _THREE_1 { get; }
                public Inners._FOUR _FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _THREEFIVE : _percentxZEROZEROⲻFF
            {
                public _THREEFIVE(Inners._THREE _THREE_1, Inners._FIVE _FIVE_1)
                {
                    this._THREE_1 = _THREE_1;
                    this._FIVE_1 = _FIVE_1;
                }
                
                public Inners._THREE _THREE_1 { get; }
                public Inners._FIVE _FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _THREESIX : _percentxZEROZEROⲻFF
            {
                public _THREESIX(Inners._THREE _THREE_1, Inners._SIX _SIX_1)
                {
                    this._THREE_1 = _THREE_1;
                    this._SIX_1 = _SIX_1;
                }
                
                public Inners._THREE _THREE_1 { get; }
                public Inners._SIX _SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _THREESEVEN : _percentxZEROZEROⲻFF
            {
                public _THREESEVEN(Inners._THREE _THREE_1, Inners._SEVEN _SEVEN_1)
                {
                    this._THREE_1 = _THREE_1;
                    this._SEVEN_1 = _SEVEN_1;
                }
                
                public Inners._THREE _THREE_1 { get; }
                public Inners._SEVEN _SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _THREEEIGHT : _percentxZEROZEROⲻFF
            {
                public _THREEEIGHT(Inners._THREE _THREE_1, Inners._EIGHT _EIGHT_1)
                {
                    this._THREE_1 = _THREE_1;
                    this._EIGHT_1 = _EIGHT_1;
                }
                
                public Inners._THREE _THREE_1 { get; }
                public Inners._EIGHT _EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _THREENINE : _percentxZEROZEROⲻFF
            {
                public _THREENINE(Inners._THREE _THREE_1, Inners._NINE _NINE_1)
                {
                    this._THREE_1 = _THREE_1;
                    this._NINE_1 = _NINE_1;
                }
                
                public Inners._THREE _THREE_1 { get; }
                public Inners._NINE _NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _THREEA : _percentxZEROZEROⲻFF
            {
                public _THREEA(Inners._THREE _THREE_1, Inners._A _A_1)
                {
                    this._THREE_1 = _THREE_1;
                    this._A_1 = _A_1;
                }
                
                public Inners._THREE _THREE_1 { get; }
                public Inners._A _A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _THREEB : _percentxZEROZEROⲻFF
            {
                public _THREEB(Inners._THREE _THREE_1, Inners._B _B_1)
                {
                    this._THREE_1 = _THREE_1;
                    this._B_1 = _B_1;
                }
                
                public Inners._THREE _THREE_1 { get; }
                public Inners._B _B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _THREEC : _percentxZEROZEROⲻFF
            {
                public _THREEC(Inners._THREE _THREE_1, Inners._C _C_1)
                {
                    this._THREE_1 = _THREE_1;
                    this._C_1 = _C_1;
                }
                
                public Inners._THREE _THREE_1 { get; }
                public Inners._C _C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _THREED : _percentxZEROZEROⲻFF
            {
                public _THREED(Inners._THREE _THREE_1, Inners._D _D_1)
                {
                    this._THREE_1 = _THREE_1;
                    this._D_1 = _D_1;
                }
                
                public Inners._THREE _THREE_1 { get; }
                public Inners._D _D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _THREEE : _percentxZEROZEROⲻFF
            {
                public _THREEE(Inners._THREE _THREE_1, Inners._E _E_1)
                {
                    this._THREE_1 = _THREE_1;
                    this._E_1 = _E_1;
                }
                
                public Inners._THREE _THREE_1 { get; }
                public Inners._E _E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _THREEF : _percentxZEROZEROⲻFF
            {
                public _THREEF(Inners._THREE _THREE_1, Inners._F _F_1)
                {
                    this._THREE_1 = _THREE_1;
                    this._F_1 = _F_1;
                }
                
                public Inners._THREE _THREE_1 { get; }
                public Inners._F _F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FOURZERO : _percentxZEROZEROⲻFF
            {
                public _FOURZERO(Inners._FOUR _FOUR_1, Inners._ZERO _ZERO_1)
                {
                    this._FOUR_1 = _FOUR_1;
                    this._ZERO_1 = _ZERO_1;
                }
                
                public Inners._FOUR _FOUR_1 { get; }
                public Inners._ZERO _ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FOURONE : _percentxZEROZEROⲻFF
            {
                public _FOURONE(Inners._FOUR _FOUR_1, Inners._ONE _ONE_1)
                {
                    this._FOUR_1 = _FOUR_1;
                    this._ONE_1 = _ONE_1;
                }
                
                public Inners._FOUR _FOUR_1 { get; }
                public Inners._ONE _ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FOURTWO : _percentxZEROZEROⲻFF
            {
                public _FOURTWO(Inners._FOUR _FOUR_1, Inners._TWO _TWO_1)
                {
                    this._FOUR_1 = _FOUR_1;
                    this._TWO_1 = _TWO_1;
                }
                
                public Inners._FOUR _FOUR_1 { get; }
                public Inners._TWO _TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FOURTHREE : _percentxZEROZEROⲻFF
            {
                public _FOURTHREE(Inners._FOUR _FOUR_1, Inners._THREE _THREE_1)
                {
                    this._FOUR_1 = _FOUR_1;
                    this._THREE_1 = _THREE_1;
                }
                
                public Inners._FOUR _FOUR_1 { get; }
                public Inners._THREE _THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FOURFOUR : _percentxZEROZEROⲻFF
            {
                public _FOURFOUR(Inners._FOUR _FOUR_1, Inners._FOUR _FOUR_2)
                {
                    this._FOUR_1 = _FOUR_1;
                    this._FOUR_2 = _FOUR_2;
                }
                
                public Inners._FOUR _FOUR_1 { get; }
                public Inners._FOUR _FOUR_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FOURFIVE : _percentxZEROZEROⲻFF
            {
                public _FOURFIVE(Inners._FOUR _FOUR_1, Inners._FIVE _FIVE_1)
                {
                    this._FOUR_1 = _FOUR_1;
                    this._FIVE_1 = _FIVE_1;
                }
                
                public Inners._FOUR _FOUR_1 { get; }
                public Inners._FIVE _FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FOURSIX : _percentxZEROZEROⲻFF
            {
                public _FOURSIX(Inners._FOUR _FOUR_1, Inners._SIX _SIX_1)
                {
                    this._FOUR_1 = _FOUR_1;
                    this._SIX_1 = _SIX_1;
                }
                
                public Inners._FOUR _FOUR_1 { get; }
                public Inners._SIX _SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FOURSEVEN : _percentxZEROZEROⲻFF
            {
                public _FOURSEVEN(Inners._FOUR _FOUR_1, Inners._SEVEN _SEVEN_1)
                {
                    this._FOUR_1 = _FOUR_1;
                    this._SEVEN_1 = _SEVEN_1;
                }
                
                public Inners._FOUR _FOUR_1 { get; }
                public Inners._SEVEN _SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FOUREIGHT : _percentxZEROZEROⲻFF
            {
                public _FOUREIGHT(Inners._FOUR _FOUR_1, Inners._EIGHT _EIGHT_1)
                {
                    this._FOUR_1 = _FOUR_1;
                    this._EIGHT_1 = _EIGHT_1;
                }
                
                public Inners._FOUR _FOUR_1 { get; }
                public Inners._EIGHT _EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FOURNINE : _percentxZEROZEROⲻFF
            {
                public _FOURNINE(Inners._FOUR _FOUR_1, Inners._NINE _NINE_1)
                {
                    this._FOUR_1 = _FOUR_1;
                    this._NINE_1 = _NINE_1;
                }
                
                public Inners._FOUR _FOUR_1 { get; }
                public Inners._NINE _NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FOURA : _percentxZEROZEROⲻFF
            {
                public _FOURA(Inners._FOUR _FOUR_1, Inners._A _A_1)
                {
                    this._FOUR_1 = _FOUR_1;
                    this._A_1 = _A_1;
                }
                
                public Inners._FOUR _FOUR_1 { get; }
                public Inners._A _A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FOURB : _percentxZEROZEROⲻFF
            {
                public _FOURB(Inners._FOUR _FOUR_1, Inners._B _B_1)
                {
                    this._FOUR_1 = _FOUR_1;
                    this._B_1 = _B_1;
                }
                
                public Inners._FOUR _FOUR_1 { get; }
                public Inners._B _B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FOURC : _percentxZEROZEROⲻFF
            {
                public _FOURC(Inners._FOUR _FOUR_1, Inners._C _C_1)
                {
                    this._FOUR_1 = _FOUR_1;
                    this._C_1 = _C_1;
                }
                
                public Inners._FOUR _FOUR_1 { get; }
                public Inners._C _C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FOURD : _percentxZEROZEROⲻFF
            {
                public _FOURD(Inners._FOUR _FOUR_1, Inners._D _D_1)
                {
                    this._FOUR_1 = _FOUR_1;
                    this._D_1 = _D_1;
                }
                
                public Inners._FOUR _FOUR_1 { get; }
                public Inners._D _D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FOURE : _percentxZEROZEROⲻFF
            {
                public _FOURE(Inners._FOUR _FOUR_1, Inners._E _E_1)
                {
                    this._FOUR_1 = _FOUR_1;
                    this._E_1 = _E_1;
                }
                
                public Inners._FOUR _FOUR_1 { get; }
                public Inners._E _E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FOURF : _percentxZEROZEROⲻFF
            {
                public _FOURF(Inners._FOUR _FOUR_1, Inners._F _F_1)
                {
                    this._FOUR_1 = _FOUR_1;
                    this._F_1 = _F_1;
                }
                
                public Inners._FOUR _FOUR_1 { get; }
                public Inners._F _F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FIVEZERO : _percentxZEROZEROⲻFF
            {
                public _FIVEZERO(Inners._FIVE _FIVE_1, Inners._ZERO _ZERO_1)
                {
                    this._FIVE_1 = _FIVE_1;
                    this._ZERO_1 = _ZERO_1;
                }
                
                public Inners._FIVE _FIVE_1 { get; }
                public Inners._ZERO _ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FIVEONE : _percentxZEROZEROⲻFF
            {
                public _FIVEONE(Inners._FIVE _FIVE_1, Inners._ONE _ONE_1)
                {
                    this._FIVE_1 = _FIVE_1;
                    this._ONE_1 = _ONE_1;
                }
                
                public Inners._FIVE _FIVE_1 { get; }
                public Inners._ONE _ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FIVETWO : _percentxZEROZEROⲻFF
            {
                public _FIVETWO(Inners._FIVE _FIVE_1, Inners._TWO _TWO_1)
                {
                    this._FIVE_1 = _FIVE_1;
                    this._TWO_1 = _TWO_1;
                }
                
                public Inners._FIVE _FIVE_1 { get; }
                public Inners._TWO _TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FIVETHREE : _percentxZEROZEROⲻFF
            {
                public _FIVETHREE(Inners._FIVE _FIVE_1, Inners._THREE _THREE_1)
                {
                    this._FIVE_1 = _FIVE_1;
                    this._THREE_1 = _THREE_1;
                }
                
                public Inners._FIVE _FIVE_1 { get; }
                public Inners._THREE _THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FIVEFOUR : _percentxZEROZEROⲻFF
            {
                public _FIVEFOUR(Inners._FIVE _FIVE_1, Inners._FOUR _FOUR_1)
                {
                    this._FIVE_1 = _FIVE_1;
                    this._FOUR_1 = _FOUR_1;
                }
                
                public Inners._FIVE _FIVE_1 { get; }
                public Inners._FOUR _FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FIVEFIVE : _percentxZEROZEROⲻFF
            {
                public _FIVEFIVE(Inners._FIVE _FIVE_1, Inners._FIVE _FIVE_2)
                {
                    this._FIVE_1 = _FIVE_1;
                    this._FIVE_2 = _FIVE_2;
                }
                
                public Inners._FIVE _FIVE_1 { get; }
                public Inners._FIVE _FIVE_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FIVESIX : _percentxZEROZEROⲻFF
            {
                public _FIVESIX(Inners._FIVE _FIVE_1, Inners._SIX _SIX_1)
                {
                    this._FIVE_1 = _FIVE_1;
                    this._SIX_1 = _SIX_1;
                }
                
                public Inners._FIVE _FIVE_1 { get; }
                public Inners._SIX _SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FIVESEVEN : _percentxZEROZEROⲻFF
            {
                public _FIVESEVEN(Inners._FIVE _FIVE_1, Inners._SEVEN _SEVEN_1)
                {
                    this._FIVE_1 = _FIVE_1;
                    this._SEVEN_1 = _SEVEN_1;
                }
                
                public Inners._FIVE _FIVE_1 { get; }
                public Inners._SEVEN _SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FIVEEIGHT : _percentxZEROZEROⲻFF
            {
                public _FIVEEIGHT(Inners._FIVE _FIVE_1, Inners._EIGHT _EIGHT_1)
                {
                    this._FIVE_1 = _FIVE_1;
                    this._EIGHT_1 = _EIGHT_1;
                }
                
                public Inners._FIVE _FIVE_1 { get; }
                public Inners._EIGHT _EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FIVENINE : _percentxZEROZEROⲻFF
            {
                public _FIVENINE(Inners._FIVE _FIVE_1, Inners._NINE _NINE_1)
                {
                    this._FIVE_1 = _FIVE_1;
                    this._NINE_1 = _NINE_1;
                }
                
                public Inners._FIVE _FIVE_1 { get; }
                public Inners._NINE _NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FIVEA : _percentxZEROZEROⲻFF
            {
                public _FIVEA(Inners._FIVE _FIVE_1, Inners._A _A_1)
                {
                    this._FIVE_1 = _FIVE_1;
                    this._A_1 = _A_1;
                }
                
                public Inners._FIVE _FIVE_1 { get; }
                public Inners._A _A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FIVEB : _percentxZEROZEROⲻFF
            {
                public _FIVEB(Inners._FIVE _FIVE_1, Inners._B _B_1)
                {
                    this._FIVE_1 = _FIVE_1;
                    this._B_1 = _B_1;
                }
                
                public Inners._FIVE _FIVE_1 { get; }
                public Inners._B _B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FIVEC : _percentxZEROZEROⲻFF
            {
                public _FIVEC(Inners._FIVE _FIVE_1, Inners._C _C_1)
                {
                    this._FIVE_1 = _FIVE_1;
                    this._C_1 = _C_1;
                }
                
                public Inners._FIVE _FIVE_1 { get; }
                public Inners._C _C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FIVED : _percentxZEROZEROⲻFF
            {
                public _FIVED(Inners._FIVE _FIVE_1, Inners._D _D_1)
                {
                    this._FIVE_1 = _FIVE_1;
                    this._D_1 = _D_1;
                }
                
                public Inners._FIVE _FIVE_1 { get; }
                public Inners._D _D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FIVEE : _percentxZEROZEROⲻFF
            {
                public _FIVEE(Inners._FIVE _FIVE_1, Inners._E _E_1)
                {
                    this._FIVE_1 = _FIVE_1;
                    this._E_1 = _E_1;
                }
                
                public Inners._FIVE _FIVE_1 { get; }
                public Inners._E _E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FIVEF : _percentxZEROZEROⲻFF
            {
                public _FIVEF(Inners._FIVE _FIVE_1, Inners._F _F_1)
                {
                    this._FIVE_1 = _FIVE_1;
                    this._F_1 = _F_1;
                }
                
                public Inners._FIVE _FIVE_1 { get; }
                public Inners._F _F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SIXZERO : _percentxZEROZEROⲻFF
            {
                public _SIXZERO(Inners._SIX _SIX_1, Inners._ZERO _ZERO_1)
                {
                    this._SIX_1 = _SIX_1;
                    this._ZERO_1 = _ZERO_1;
                }
                
                public Inners._SIX _SIX_1 { get; }
                public Inners._ZERO _ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SIXONE : _percentxZEROZEROⲻFF
            {
                public _SIXONE(Inners._SIX _SIX_1, Inners._ONE _ONE_1)
                {
                    this._SIX_1 = _SIX_1;
                    this._ONE_1 = _ONE_1;
                }
                
                public Inners._SIX _SIX_1 { get; }
                public Inners._ONE _ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SIXTWO : _percentxZEROZEROⲻFF
            {
                public _SIXTWO(Inners._SIX _SIX_1, Inners._TWO _TWO_1)
                {
                    this._SIX_1 = _SIX_1;
                    this._TWO_1 = _TWO_1;
                }
                
                public Inners._SIX _SIX_1 { get; }
                public Inners._TWO _TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SIXTHREE : _percentxZEROZEROⲻFF
            {
                public _SIXTHREE(Inners._SIX _SIX_1, Inners._THREE _THREE_1)
                {
                    this._SIX_1 = _SIX_1;
                    this._THREE_1 = _THREE_1;
                }
                
                public Inners._SIX _SIX_1 { get; }
                public Inners._THREE _THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SIXFOUR : _percentxZEROZEROⲻFF
            {
                public _SIXFOUR(Inners._SIX _SIX_1, Inners._FOUR _FOUR_1)
                {
                    this._SIX_1 = _SIX_1;
                    this._FOUR_1 = _FOUR_1;
                }
                
                public Inners._SIX _SIX_1 { get; }
                public Inners._FOUR _FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SIXFIVE : _percentxZEROZEROⲻFF
            {
                public _SIXFIVE(Inners._SIX _SIX_1, Inners._FIVE _FIVE_1)
                {
                    this._SIX_1 = _SIX_1;
                    this._FIVE_1 = _FIVE_1;
                }
                
                public Inners._SIX _SIX_1 { get; }
                public Inners._FIVE _FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SIXSIX : _percentxZEROZEROⲻFF
            {
                public _SIXSIX(Inners._SIX _SIX_1, Inners._SIX _SIX_2)
                {
                    this._SIX_1 = _SIX_1;
                    this._SIX_2 = _SIX_2;
                }
                
                public Inners._SIX _SIX_1 { get; }
                public Inners._SIX _SIX_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SIXSEVEN : _percentxZEROZEROⲻFF
            {
                public _SIXSEVEN(Inners._SIX _SIX_1, Inners._SEVEN _SEVEN_1)
                {
                    this._SIX_1 = _SIX_1;
                    this._SEVEN_1 = _SEVEN_1;
                }
                
                public Inners._SIX _SIX_1 { get; }
                public Inners._SEVEN _SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SIXEIGHT : _percentxZEROZEROⲻFF
            {
                public _SIXEIGHT(Inners._SIX _SIX_1, Inners._EIGHT _EIGHT_1)
                {
                    this._SIX_1 = _SIX_1;
                    this._EIGHT_1 = _EIGHT_1;
                }
                
                public Inners._SIX _SIX_1 { get; }
                public Inners._EIGHT _EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SIXNINE : _percentxZEROZEROⲻFF
            {
                public _SIXNINE(Inners._SIX _SIX_1, Inners._NINE _NINE_1)
                {
                    this._SIX_1 = _SIX_1;
                    this._NINE_1 = _NINE_1;
                }
                
                public Inners._SIX _SIX_1 { get; }
                public Inners._NINE _NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SIXA : _percentxZEROZEROⲻFF
            {
                public _SIXA(Inners._SIX _SIX_1, Inners._A _A_1)
                {
                    this._SIX_1 = _SIX_1;
                    this._A_1 = _A_1;
                }
                
                public Inners._SIX _SIX_1 { get; }
                public Inners._A _A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SIXB : _percentxZEROZEROⲻFF
            {
                public _SIXB(Inners._SIX _SIX_1, Inners._B _B_1)
                {
                    this._SIX_1 = _SIX_1;
                    this._B_1 = _B_1;
                }
                
                public Inners._SIX _SIX_1 { get; }
                public Inners._B _B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SIXC : _percentxZEROZEROⲻFF
            {
                public _SIXC(Inners._SIX _SIX_1, Inners._C _C_1)
                {
                    this._SIX_1 = _SIX_1;
                    this._C_1 = _C_1;
                }
                
                public Inners._SIX _SIX_1 { get; }
                public Inners._C _C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SIXD : _percentxZEROZEROⲻFF
            {
                public _SIXD(Inners._SIX _SIX_1, Inners._D _D_1)
                {
                    this._SIX_1 = _SIX_1;
                    this._D_1 = _D_1;
                }
                
                public Inners._SIX _SIX_1 { get; }
                public Inners._D _D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SIXE : _percentxZEROZEROⲻFF
            {
                public _SIXE(Inners._SIX _SIX_1, Inners._E _E_1)
                {
                    this._SIX_1 = _SIX_1;
                    this._E_1 = _E_1;
                }
                
                public Inners._SIX _SIX_1 { get; }
                public Inners._E _E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SIXF : _percentxZEROZEROⲻFF
            {
                public _SIXF(Inners._SIX _SIX_1, Inners._F _F_1)
                {
                    this._SIX_1 = _SIX_1;
                    this._F_1 = _F_1;
                }
                
                public Inners._SIX _SIX_1 { get; }
                public Inners._F _F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SEVENZERO : _percentxZEROZEROⲻFF
            {
                public _SEVENZERO(Inners._SEVEN _SEVEN_1, Inners._ZERO _ZERO_1)
                {
                    this._SEVEN_1 = _SEVEN_1;
                    this._ZERO_1 = _ZERO_1;
                }
                
                public Inners._SEVEN _SEVEN_1 { get; }
                public Inners._ZERO _ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SEVENONE : _percentxZEROZEROⲻFF
            {
                public _SEVENONE(Inners._SEVEN _SEVEN_1, Inners._ONE _ONE_1)
                {
                    this._SEVEN_1 = _SEVEN_1;
                    this._ONE_1 = _ONE_1;
                }
                
                public Inners._SEVEN _SEVEN_1 { get; }
                public Inners._ONE _ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SEVENTWO : _percentxZEROZEROⲻFF
            {
                public _SEVENTWO(Inners._SEVEN _SEVEN_1, Inners._TWO _TWO_1)
                {
                    this._SEVEN_1 = _SEVEN_1;
                    this._TWO_1 = _TWO_1;
                }
                
                public Inners._SEVEN _SEVEN_1 { get; }
                public Inners._TWO _TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SEVENTHREE : _percentxZEROZEROⲻFF
            {
                public _SEVENTHREE(Inners._SEVEN _SEVEN_1, Inners._THREE _THREE_1)
                {
                    this._SEVEN_1 = _SEVEN_1;
                    this._THREE_1 = _THREE_1;
                }
                
                public Inners._SEVEN _SEVEN_1 { get; }
                public Inners._THREE _THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SEVENFOUR : _percentxZEROZEROⲻFF
            {
                public _SEVENFOUR(Inners._SEVEN _SEVEN_1, Inners._FOUR _FOUR_1)
                {
                    this._SEVEN_1 = _SEVEN_1;
                    this._FOUR_1 = _FOUR_1;
                }
                
                public Inners._SEVEN _SEVEN_1 { get; }
                public Inners._FOUR _FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SEVENFIVE : _percentxZEROZEROⲻFF
            {
                public _SEVENFIVE(Inners._SEVEN _SEVEN_1, Inners._FIVE _FIVE_1)
                {
                    this._SEVEN_1 = _SEVEN_1;
                    this._FIVE_1 = _FIVE_1;
                }
                
                public Inners._SEVEN _SEVEN_1 { get; }
                public Inners._FIVE _FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SEVENSIX : _percentxZEROZEROⲻFF
            {
                public _SEVENSIX(Inners._SEVEN _SEVEN_1, Inners._SIX _SIX_1)
                {
                    this._SEVEN_1 = _SEVEN_1;
                    this._SIX_1 = _SIX_1;
                }
                
                public Inners._SEVEN _SEVEN_1 { get; }
                public Inners._SIX _SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SEVENSEVEN : _percentxZEROZEROⲻFF
            {
                public _SEVENSEVEN(Inners._SEVEN _SEVEN_1, Inners._SEVEN _SEVEN_2)
                {
                    this._SEVEN_1 = _SEVEN_1;
                    this._SEVEN_2 = _SEVEN_2;
                }
                
                public Inners._SEVEN _SEVEN_1 { get; }
                public Inners._SEVEN _SEVEN_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SEVENEIGHT : _percentxZEROZEROⲻFF
            {
                public _SEVENEIGHT(Inners._SEVEN _SEVEN_1, Inners._EIGHT _EIGHT_1)
                {
                    this._SEVEN_1 = _SEVEN_1;
                    this._EIGHT_1 = _EIGHT_1;
                }
                
                public Inners._SEVEN _SEVEN_1 { get; }
                public Inners._EIGHT _EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SEVENNINE : _percentxZEROZEROⲻFF
            {
                public _SEVENNINE(Inners._SEVEN _SEVEN_1, Inners._NINE _NINE_1)
                {
                    this._SEVEN_1 = _SEVEN_1;
                    this._NINE_1 = _NINE_1;
                }
                
                public Inners._SEVEN _SEVEN_1 { get; }
                public Inners._NINE _NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SEVENA : _percentxZEROZEROⲻFF
            {
                public _SEVENA(Inners._SEVEN _SEVEN_1, Inners._A _A_1)
                {
                    this._SEVEN_1 = _SEVEN_1;
                    this._A_1 = _A_1;
                }
                
                public Inners._SEVEN _SEVEN_1 { get; }
                public Inners._A _A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SEVENB : _percentxZEROZEROⲻFF
            {
                public _SEVENB(Inners._SEVEN _SEVEN_1, Inners._B _B_1)
                {
                    this._SEVEN_1 = _SEVEN_1;
                    this._B_1 = _B_1;
                }
                
                public Inners._SEVEN _SEVEN_1 { get; }
                public Inners._B _B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SEVENC : _percentxZEROZEROⲻFF
            {
                public _SEVENC(Inners._SEVEN _SEVEN_1, Inners._C _C_1)
                {
                    this._SEVEN_1 = _SEVEN_1;
                    this._C_1 = _C_1;
                }
                
                public Inners._SEVEN _SEVEN_1 { get; }
                public Inners._C _C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SEVEND : _percentxZEROZEROⲻFF
            {
                public _SEVEND(Inners._SEVEN _SEVEN_1, Inners._D _D_1)
                {
                    this._SEVEN_1 = _SEVEN_1;
                    this._D_1 = _D_1;
                }
                
                public Inners._SEVEN _SEVEN_1 { get; }
                public Inners._D _D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SEVENE : _percentxZEROZEROⲻFF
            {
                public _SEVENE(Inners._SEVEN _SEVEN_1, Inners._E _E_1)
                {
                    this._SEVEN_1 = _SEVEN_1;
                    this._E_1 = _E_1;
                }
                
                public Inners._SEVEN _SEVEN_1 { get; }
                public Inners._E _E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SEVENF : _percentxZEROZEROⲻFF
            {
                public _SEVENF(Inners._SEVEN _SEVEN_1, Inners._F _F_1)
                {
                    this._SEVEN_1 = _SEVEN_1;
                    this._F_1 = _F_1;
                }
                
                public Inners._SEVEN _SEVEN_1 { get; }
                public Inners._F _F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _EIGHTZERO : _percentxZEROZEROⲻFF
            {
                public _EIGHTZERO(Inners._EIGHT _EIGHT_1, Inners._ZERO _ZERO_1)
                {
                    this._EIGHT_1 = _EIGHT_1;
                    this._ZERO_1 = _ZERO_1;
                }
                
                public Inners._EIGHT _EIGHT_1 { get; }
                public Inners._ZERO _ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _EIGHTONE : _percentxZEROZEROⲻFF
            {
                public _EIGHTONE(Inners._EIGHT _EIGHT_1, Inners._ONE _ONE_1)
                {
                    this._EIGHT_1 = _EIGHT_1;
                    this._ONE_1 = _ONE_1;
                }
                
                public Inners._EIGHT _EIGHT_1 { get; }
                public Inners._ONE _ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _EIGHTTWO : _percentxZEROZEROⲻFF
            {
                public _EIGHTTWO(Inners._EIGHT _EIGHT_1, Inners._TWO _TWO_1)
                {
                    this._EIGHT_1 = _EIGHT_1;
                    this._TWO_1 = _TWO_1;
                }
                
                public Inners._EIGHT _EIGHT_1 { get; }
                public Inners._TWO _TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _EIGHTTHREE : _percentxZEROZEROⲻFF
            {
                public _EIGHTTHREE(Inners._EIGHT _EIGHT_1, Inners._THREE _THREE_1)
                {
                    this._EIGHT_1 = _EIGHT_1;
                    this._THREE_1 = _THREE_1;
                }
                
                public Inners._EIGHT _EIGHT_1 { get; }
                public Inners._THREE _THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _EIGHTFOUR : _percentxZEROZEROⲻFF
            {
                public _EIGHTFOUR(Inners._EIGHT _EIGHT_1, Inners._FOUR _FOUR_1)
                {
                    this._EIGHT_1 = _EIGHT_1;
                    this._FOUR_1 = _FOUR_1;
                }
                
                public Inners._EIGHT _EIGHT_1 { get; }
                public Inners._FOUR _FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _EIGHTFIVE : _percentxZEROZEROⲻFF
            {
                public _EIGHTFIVE(Inners._EIGHT _EIGHT_1, Inners._FIVE _FIVE_1)
                {
                    this._EIGHT_1 = _EIGHT_1;
                    this._FIVE_1 = _FIVE_1;
                }
                
                public Inners._EIGHT _EIGHT_1 { get; }
                public Inners._FIVE _FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _EIGHTSIX : _percentxZEROZEROⲻFF
            {
                public _EIGHTSIX(Inners._EIGHT _EIGHT_1, Inners._SIX _SIX_1)
                {
                    this._EIGHT_1 = _EIGHT_1;
                    this._SIX_1 = _SIX_1;
                }
                
                public Inners._EIGHT _EIGHT_1 { get; }
                public Inners._SIX _SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _EIGHTSEVEN : _percentxZEROZEROⲻFF
            {
                public _EIGHTSEVEN(Inners._EIGHT _EIGHT_1, Inners._SEVEN _SEVEN_1)
                {
                    this._EIGHT_1 = _EIGHT_1;
                    this._SEVEN_1 = _SEVEN_1;
                }
                
                public Inners._EIGHT _EIGHT_1 { get; }
                public Inners._SEVEN _SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _EIGHTEIGHT : _percentxZEROZEROⲻFF
            {
                public _EIGHTEIGHT(Inners._EIGHT _EIGHT_1, Inners._EIGHT _EIGHT_2)
                {
                    this._EIGHT_1 = _EIGHT_1;
                    this._EIGHT_2 = _EIGHT_2;
                }
                
                public Inners._EIGHT _EIGHT_1 { get; }
                public Inners._EIGHT _EIGHT_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _EIGHTNINE : _percentxZEROZEROⲻFF
            {
                public _EIGHTNINE(Inners._EIGHT _EIGHT_1, Inners._NINE _NINE_1)
                {
                    this._EIGHT_1 = _EIGHT_1;
                    this._NINE_1 = _NINE_1;
                }
                
                public Inners._EIGHT _EIGHT_1 { get; }
                public Inners._NINE _NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _EIGHTA : _percentxZEROZEROⲻFF
            {
                public _EIGHTA(Inners._EIGHT _EIGHT_1, Inners._A _A_1)
                {
                    this._EIGHT_1 = _EIGHT_1;
                    this._A_1 = _A_1;
                }
                
                public Inners._EIGHT _EIGHT_1 { get; }
                public Inners._A _A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _EIGHTB : _percentxZEROZEROⲻFF
            {
                public _EIGHTB(Inners._EIGHT _EIGHT_1, Inners._B _B_1)
                {
                    this._EIGHT_1 = _EIGHT_1;
                    this._B_1 = _B_1;
                }
                
                public Inners._EIGHT _EIGHT_1 { get; }
                public Inners._B _B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _EIGHTC : _percentxZEROZEROⲻFF
            {
                public _EIGHTC(Inners._EIGHT _EIGHT_1, Inners._C _C_1)
                {
                    this._EIGHT_1 = _EIGHT_1;
                    this._C_1 = _C_1;
                }
                
                public Inners._EIGHT _EIGHT_1 { get; }
                public Inners._C _C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _EIGHTD : _percentxZEROZEROⲻFF
            {
                public _EIGHTD(Inners._EIGHT _EIGHT_1, Inners._D _D_1)
                {
                    this._EIGHT_1 = _EIGHT_1;
                    this._D_1 = _D_1;
                }
                
                public Inners._EIGHT _EIGHT_1 { get; }
                public Inners._D _D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _EIGHTE : _percentxZEROZEROⲻFF
            {
                public _EIGHTE(Inners._EIGHT _EIGHT_1, Inners._E _E_1)
                {
                    this._EIGHT_1 = _EIGHT_1;
                    this._E_1 = _E_1;
                }
                
                public Inners._EIGHT _EIGHT_1 { get; }
                public Inners._E _E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _EIGHTF : _percentxZEROZEROⲻFF
            {
                public _EIGHTF(Inners._EIGHT _EIGHT_1, Inners._F _F_1)
                {
                    this._EIGHT_1 = _EIGHT_1;
                    this._F_1 = _F_1;
                }
                
                public Inners._EIGHT _EIGHT_1 { get; }
                public Inners._F _F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _NINEZERO : _percentxZEROZEROⲻFF
            {
                public _NINEZERO(Inners._NINE _NINE_1, Inners._ZERO _ZERO_1)
                {
                    this._NINE_1 = _NINE_1;
                    this._ZERO_1 = _ZERO_1;
                }
                
                public Inners._NINE _NINE_1 { get; }
                public Inners._ZERO _ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _NINEONE : _percentxZEROZEROⲻFF
            {
                public _NINEONE(Inners._NINE _NINE_1, Inners._ONE _ONE_1)
                {
                    this._NINE_1 = _NINE_1;
                    this._ONE_1 = _ONE_1;
                }
                
                public Inners._NINE _NINE_1 { get; }
                public Inners._ONE _ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _NINETWO : _percentxZEROZEROⲻFF
            {
                public _NINETWO(Inners._NINE _NINE_1, Inners._TWO _TWO_1)
                {
                    this._NINE_1 = _NINE_1;
                    this._TWO_1 = _TWO_1;
                }
                
                public Inners._NINE _NINE_1 { get; }
                public Inners._TWO _TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _NINETHREE : _percentxZEROZEROⲻFF
            {
                public _NINETHREE(Inners._NINE _NINE_1, Inners._THREE _THREE_1)
                {
                    this._NINE_1 = _NINE_1;
                    this._THREE_1 = _THREE_1;
                }
                
                public Inners._NINE _NINE_1 { get; }
                public Inners._THREE _THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _NINEFOUR : _percentxZEROZEROⲻFF
            {
                public _NINEFOUR(Inners._NINE _NINE_1, Inners._FOUR _FOUR_1)
                {
                    this._NINE_1 = _NINE_1;
                    this._FOUR_1 = _FOUR_1;
                }
                
                public Inners._NINE _NINE_1 { get; }
                public Inners._FOUR _FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _NINEFIVE : _percentxZEROZEROⲻFF
            {
                public _NINEFIVE(Inners._NINE _NINE_1, Inners._FIVE _FIVE_1)
                {
                    this._NINE_1 = _NINE_1;
                    this._FIVE_1 = _FIVE_1;
                }
                
                public Inners._NINE _NINE_1 { get; }
                public Inners._FIVE _FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _NINESIX : _percentxZEROZEROⲻFF
            {
                public _NINESIX(Inners._NINE _NINE_1, Inners._SIX _SIX_1)
                {
                    this._NINE_1 = _NINE_1;
                    this._SIX_1 = _SIX_1;
                }
                
                public Inners._NINE _NINE_1 { get; }
                public Inners._SIX _SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _NINESEVEN : _percentxZEROZEROⲻFF
            {
                public _NINESEVEN(Inners._NINE _NINE_1, Inners._SEVEN _SEVEN_1)
                {
                    this._NINE_1 = _NINE_1;
                    this._SEVEN_1 = _SEVEN_1;
                }
                
                public Inners._NINE _NINE_1 { get; }
                public Inners._SEVEN _SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _NINEEIGHT : _percentxZEROZEROⲻFF
            {
                public _NINEEIGHT(Inners._NINE _NINE_1, Inners._EIGHT _EIGHT_1)
                {
                    this._NINE_1 = _NINE_1;
                    this._EIGHT_1 = _EIGHT_1;
                }
                
                public Inners._NINE _NINE_1 { get; }
                public Inners._EIGHT _EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _NINENINE : _percentxZEROZEROⲻFF
            {
                public _NINENINE(Inners._NINE _NINE_1, Inners._NINE _NINE_2)
                {
                    this._NINE_1 = _NINE_1;
                    this._NINE_2 = _NINE_2;
                }
                
                public Inners._NINE _NINE_1 { get; }
                public Inners._NINE _NINE_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _NINEA : _percentxZEROZEROⲻFF
            {
                public _NINEA(Inners._NINE _NINE_1, Inners._A _A_1)
                {
                    this._NINE_1 = _NINE_1;
                    this._A_1 = _A_1;
                }
                
                public Inners._NINE _NINE_1 { get; }
                public Inners._A _A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _NINEB : _percentxZEROZEROⲻFF
            {
                public _NINEB(Inners._NINE _NINE_1, Inners._B _B_1)
                {
                    this._NINE_1 = _NINE_1;
                    this._B_1 = _B_1;
                }
                
                public Inners._NINE _NINE_1 { get; }
                public Inners._B _B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _NINEC : _percentxZEROZEROⲻFF
            {
                public _NINEC(Inners._NINE _NINE_1, Inners._C _C_1)
                {
                    this._NINE_1 = _NINE_1;
                    this._C_1 = _C_1;
                }
                
                public Inners._NINE _NINE_1 { get; }
                public Inners._C _C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _NINED : _percentxZEROZEROⲻFF
            {
                public _NINED(Inners._NINE _NINE_1, Inners._D _D_1)
                {
                    this._NINE_1 = _NINE_1;
                    this._D_1 = _D_1;
                }
                
                public Inners._NINE _NINE_1 { get; }
                public Inners._D _D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _NINEE : _percentxZEROZEROⲻFF
            {
                public _NINEE(Inners._NINE _NINE_1, Inners._E _E_1)
                {
                    this._NINE_1 = _NINE_1;
                    this._E_1 = _E_1;
                }
                
                public Inners._NINE _NINE_1 { get; }
                public Inners._E _E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _NINEF : _percentxZEROZEROⲻFF
            {
                public _NINEF(Inners._NINE _NINE_1, Inners._F _F_1)
                {
                    this._NINE_1 = _NINE_1;
                    this._F_1 = _F_1;
                }
                
                public Inners._NINE _NINE_1 { get; }
                public Inners._F _F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _AZERO : _percentxZEROZEROⲻFF
            {
                public _AZERO(Inners._A _A_1, Inners._ZERO _ZERO_1)
                {
                    this._A_1 = _A_1;
                    this._ZERO_1 = _ZERO_1;
                }
                
                public Inners._A _A_1 { get; }
                public Inners._ZERO _ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _AONE : _percentxZEROZEROⲻFF
            {
                public _AONE(Inners._A _A_1, Inners._ONE _ONE_1)
                {
                    this._A_1 = _A_1;
                    this._ONE_1 = _ONE_1;
                }
                
                public Inners._A _A_1 { get; }
                public Inners._ONE _ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ATWO : _percentxZEROZEROⲻFF
            {
                public _ATWO(Inners._A _A_1, Inners._TWO _TWO_1)
                {
                    this._A_1 = _A_1;
                    this._TWO_1 = _TWO_1;
                }
                
                public Inners._A _A_1 { get; }
                public Inners._TWO _TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ATHREE : _percentxZEROZEROⲻFF
            {
                public _ATHREE(Inners._A _A_1, Inners._THREE _THREE_1)
                {
                    this._A_1 = _A_1;
                    this._THREE_1 = _THREE_1;
                }
                
                public Inners._A _A_1 { get; }
                public Inners._THREE _THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _AFOUR : _percentxZEROZEROⲻFF
            {
                public _AFOUR(Inners._A _A_1, Inners._FOUR _FOUR_1)
                {
                    this._A_1 = _A_1;
                    this._FOUR_1 = _FOUR_1;
                }
                
                public Inners._A _A_1 { get; }
                public Inners._FOUR _FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _AFIVE : _percentxZEROZEROⲻFF
            {
                public _AFIVE(Inners._A _A_1, Inners._FIVE _FIVE_1)
                {
                    this._A_1 = _A_1;
                    this._FIVE_1 = _FIVE_1;
                }
                
                public Inners._A _A_1 { get; }
                public Inners._FIVE _FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ASIX : _percentxZEROZEROⲻFF
            {
                public _ASIX(Inners._A _A_1, Inners._SIX _SIX_1)
                {
                    this._A_1 = _A_1;
                    this._SIX_1 = _SIX_1;
                }
                
                public Inners._A _A_1 { get; }
                public Inners._SIX _SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ASEVEN : _percentxZEROZEROⲻFF
            {
                public _ASEVEN(Inners._A _A_1, Inners._SEVEN _SEVEN_1)
                {
                    this._A_1 = _A_1;
                    this._SEVEN_1 = _SEVEN_1;
                }
                
                public Inners._A _A_1 { get; }
                public Inners._SEVEN _SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _AEIGHT : _percentxZEROZEROⲻFF
            {
                public _AEIGHT(Inners._A _A_1, Inners._EIGHT _EIGHT_1)
                {
                    this._A_1 = _A_1;
                    this._EIGHT_1 = _EIGHT_1;
                }
                
                public Inners._A _A_1 { get; }
                public Inners._EIGHT _EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ANINE : _percentxZEROZEROⲻFF
            {
                public _ANINE(Inners._A _A_1, Inners._NINE _NINE_1)
                {
                    this._A_1 = _A_1;
                    this._NINE_1 = _NINE_1;
                }
                
                public Inners._A _A_1 { get; }
                public Inners._NINE _NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _AA : _percentxZEROZEROⲻFF
            {
                public _AA(Inners._A _A_1, Inners._A _A_2)
                {
                    this._A_1 = _A_1;
                    this._A_2 = _A_2;
                }
                
                public Inners._A _A_1 { get; }
                public Inners._A _A_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _AB : _percentxZEROZEROⲻFF
            {
                public _AB(Inners._A _A_1, Inners._B _B_1)
                {
                    this._A_1 = _A_1;
                    this._B_1 = _B_1;
                }
                
                public Inners._A _A_1 { get; }
                public Inners._B _B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _AC : _percentxZEROZEROⲻFF
            {
                public _AC(Inners._A _A_1, Inners._C _C_1)
                {
                    this._A_1 = _A_1;
                    this._C_1 = _C_1;
                }
                
                public Inners._A _A_1 { get; }
                public Inners._C _C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _AD : _percentxZEROZEROⲻFF
            {
                public _AD(Inners._A _A_1, Inners._D _D_1)
                {
                    this._A_1 = _A_1;
                    this._D_1 = _D_1;
                }
                
                public Inners._A _A_1 { get; }
                public Inners._D _D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _AE : _percentxZEROZEROⲻFF
            {
                public _AE(Inners._A _A_1, Inners._E _E_1)
                {
                    this._A_1 = _A_1;
                    this._E_1 = _E_1;
                }
                
                public Inners._A _A_1 { get; }
                public Inners._E _E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _AF : _percentxZEROZEROⲻFF
            {
                public _AF(Inners._A _A_1, Inners._F _F_1)
                {
                    this._A_1 = _A_1;
                    this._F_1 = _F_1;
                }
                
                public Inners._A _A_1 { get; }
                public Inners._F _F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _BZERO : _percentxZEROZEROⲻFF
            {
                public _BZERO(Inners._B _B_1, Inners._ZERO _ZERO_1)
                {
                    this._B_1 = _B_1;
                    this._ZERO_1 = _ZERO_1;
                }
                
                public Inners._B _B_1 { get; }
                public Inners._ZERO _ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _BONE : _percentxZEROZEROⲻFF
            {
                public _BONE(Inners._B _B_1, Inners._ONE _ONE_1)
                {
                    this._B_1 = _B_1;
                    this._ONE_1 = _ONE_1;
                }
                
                public Inners._B _B_1 { get; }
                public Inners._ONE _ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _BTWO : _percentxZEROZEROⲻFF
            {
                public _BTWO(Inners._B _B_1, Inners._TWO _TWO_1)
                {
                    this._B_1 = _B_1;
                    this._TWO_1 = _TWO_1;
                }
                
                public Inners._B _B_1 { get; }
                public Inners._TWO _TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _BTHREE : _percentxZEROZEROⲻFF
            {
                public _BTHREE(Inners._B _B_1, Inners._THREE _THREE_1)
                {
                    this._B_1 = _B_1;
                    this._THREE_1 = _THREE_1;
                }
                
                public Inners._B _B_1 { get; }
                public Inners._THREE _THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _BFOUR : _percentxZEROZEROⲻFF
            {
                public _BFOUR(Inners._B _B_1, Inners._FOUR _FOUR_1)
                {
                    this._B_1 = _B_1;
                    this._FOUR_1 = _FOUR_1;
                }
                
                public Inners._B _B_1 { get; }
                public Inners._FOUR _FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _BFIVE : _percentxZEROZEROⲻFF
            {
                public _BFIVE(Inners._B _B_1, Inners._FIVE _FIVE_1)
                {
                    this._B_1 = _B_1;
                    this._FIVE_1 = _FIVE_1;
                }
                
                public Inners._B _B_1 { get; }
                public Inners._FIVE _FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _BSIX : _percentxZEROZEROⲻFF
            {
                public _BSIX(Inners._B _B_1, Inners._SIX _SIX_1)
                {
                    this._B_1 = _B_1;
                    this._SIX_1 = _SIX_1;
                }
                
                public Inners._B _B_1 { get; }
                public Inners._SIX _SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _BSEVEN : _percentxZEROZEROⲻFF
            {
                public _BSEVEN(Inners._B _B_1, Inners._SEVEN _SEVEN_1)
                {
                    this._B_1 = _B_1;
                    this._SEVEN_1 = _SEVEN_1;
                }
                
                public Inners._B _B_1 { get; }
                public Inners._SEVEN _SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _BEIGHT : _percentxZEROZEROⲻFF
            {
                public _BEIGHT(Inners._B _B_1, Inners._EIGHT _EIGHT_1)
                {
                    this._B_1 = _B_1;
                    this._EIGHT_1 = _EIGHT_1;
                }
                
                public Inners._B _B_1 { get; }
                public Inners._EIGHT _EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _BNINE : _percentxZEROZEROⲻFF
            {
                public _BNINE(Inners._B _B_1, Inners._NINE _NINE_1)
                {
                    this._B_1 = _B_1;
                    this._NINE_1 = _NINE_1;
                }
                
                public Inners._B _B_1 { get; }
                public Inners._NINE _NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _BA : _percentxZEROZEROⲻFF
            {
                public _BA(Inners._B _B_1, Inners._A _A_1)
                {
                    this._B_1 = _B_1;
                    this._A_1 = _A_1;
                }
                
                public Inners._B _B_1 { get; }
                public Inners._A _A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _BB : _percentxZEROZEROⲻFF
            {
                public _BB(Inners._B _B_1, Inners._B _B_2)
                {
                    this._B_1 = _B_1;
                    this._B_2 = _B_2;
                }
                
                public Inners._B _B_1 { get; }
                public Inners._B _B_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _BC : _percentxZEROZEROⲻFF
            {
                public _BC(Inners._B _B_1, Inners._C _C_1)
                {
                    this._B_1 = _B_1;
                    this._C_1 = _C_1;
                }
                
                public Inners._B _B_1 { get; }
                public Inners._C _C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _BD : _percentxZEROZEROⲻFF
            {
                public _BD(Inners._B _B_1, Inners._D _D_1)
                {
                    this._B_1 = _B_1;
                    this._D_1 = _D_1;
                }
                
                public Inners._B _B_1 { get; }
                public Inners._D _D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _BE : _percentxZEROZEROⲻFF
            {
                public _BE(Inners._B _B_1, Inners._E _E_1)
                {
                    this._B_1 = _B_1;
                    this._E_1 = _E_1;
                }
                
                public Inners._B _B_1 { get; }
                public Inners._E _E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _BF : _percentxZEROZEROⲻFF
            {
                public _BF(Inners._B _B_1, Inners._F _F_1)
                {
                    this._B_1 = _B_1;
                    this._F_1 = _F_1;
                }
                
                public Inners._B _B_1 { get; }
                public Inners._F _F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _CZERO : _percentxZEROZEROⲻFF
            {
                public _CZERO(Inners._C _C_1, Inners._ZERO _ZERO_1)
                {
                    this._C_1 = _C_1;
                    this._ZERO_1 = _ZERO_1;
                }
                
                public Inners._C _C_1 { get; }
                public Inners._ZERO _ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _CONE : _percentxZEROZEROⲻFF
            {
                public _CONE(Inners._C _C_1, Inners._ONE _ONE_1)
                {
                    this._C_1 = _C_1;
                    this._ONE_1 = _ONE_1;
                }
                
                public Inners._C _C_1 { get; }
                public Inners._ONE _ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _CTWO : _percentxZEROZEROⲻFF
            {
                public _CTWO(Inners._C _C_1, Inners._TWO _TWO_1)
                {
                    this._C_1 = _C_1;
                    this._TWO_1 = _TWO_1;
                }
                
                public Inners._C _C_1 { get; }
                public Inners._TWO _TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _CTHREE : _percentxZEROZEROⲻFF
            {
                public _CTHREE(Inners._C _C_1, Inners._THREE _THREE_1)
                {
                    this._C_1 = _C_1;
                    this._THREE_1 = _THREE_1;
                }
                
                public Inners._C _C_1 { get; }
                public Inners._THREE _THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _CFOUR : _percentxZEROZEROⲻFF
            {
                public _CFOUR(Inners._C _C_1, Inners._FOUR _FOUR_1)
                {
                    this._C_1 = _C_1;
                    this._FOUR_1 = _FOUR_1;
                }
                
                public Inners._C _C_1 { get; }
                public Inners._FOUR _FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _CFIVE : _percentxZEROZEROⲻFF
            {
                public _CFIVE(Inners._C _C_1, Inners._FIVE _FIVE_1)
                {
                    this._C_1 = _C_1;
                    this._FIVE_1 = _FIVE_1;
                }
                
                public Inners._C _C_1 { get; }
                public Inners._FIVE _FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _CSIX : _percentxZEROZEROⲻFF
            {
                public _CSIX(Inners._C _C_1, Inners._SIX _SIX_1)
                {
                    this._C_1 = _C_1;
                    this._SIX_1 = _SIX_1;
                }
                
                public Inners._C _C_1 { get; }
                public Inners._SIX _SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _CSEVEN : _percentxZEROZEROⲻFF
            {
                public _CSEVEN(Inners._C _C_1, Inners._SEVEN _SEVEN_1)
                {
                    this._C_1 = _C_1;
                    this._SEVEN_1 = _SEVEN_1;
                }
                
                public Inners._C _C_1 { get; }
                public Inners._SEVEN _SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _CEIGHT : _percentxZEROZEROⲻFF
            {
                public _CEIGHT(Inners._C _C_1, Inners._EIGHT _EIGHT_1)
                {
                    this._C_1 = _C_1;
                    this._EIGHT_1 = _EIGHT_1;
                }
                
                public Inners._C _C_1 { get; }
                public Inners._EIGHT _EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _CNINE : _percentxZEROZEROⲻFF
            {
                public _CNINE(Inners._C _C_1, Inners._NINE _NINE_1)
                {
                    this._C_1 = _C_1;
                    this._NINE_1 = _NINE_1;
                }
                
                public Inners._C _C_1 { get; }
                public Inners._NINE _NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _CA : _percentxZEROZEROⲻFF
            {
                public _CA(Inners._C _C_1, Inners._A _A_1)
                {
                    this._C_1 = _C_1;
                    this._A_1 = _A_1;
                }
                
                public Inners._C _C_1 { get; }
                public Inners._A _A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _CB : _percentxZEROZEROⲻFF
            {
                public _CB(Inners._C _C_1, Inners._B _B_1)
                {
                    this._C_1 = _C_1;
                    this._B_1 = _B_1;
                }
                
                public Inners._C _C_1 { get; }
                public Inners._B _B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _CC : _percentxZEROZEROⲻFF
            {
                public _CC(Inners._C _C_1, Inners._C _C_2)
                {
                    this._C_1 = _C_1;
                    this._C_2 = _C_2;
                }
                
                public Inners._C _C_1 { get; }
                public Inners._C _C_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _CD : _percentxZEROZEROⲻFF
            {
                public _CD(Inners._C _C_1, Inners._D _D_1)
                {
                    this._C_1 = _C_1;
                    this._D_1 = _D_1;
                }
                
                public Inners._C _C_1 { get; }
                public Inners._D _D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _CE : _percentxZEROZEROⲻFF
            {
                public _CE(Inners._C _C_1, Inners._E _E_1)
                {
                    this._C_1 = _C_1;
                    this._E_1 = _E_1;
                }
                
                public Inners._C _C_1 { get; }
                public Inners._E _E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _CF : _percentxZEROZEROⲻFF
            {
                public _CF(Inners._C _C_1, Inners._F _F_1)
                {
                    this._C_1 = _C_1;
                    this._F_1 = _F_1;
                }
                
                public Inners._C _C_1 { get; }
                public Inners._F _F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _DZERO : _percentxZEROZEROⲻFF
            {
                public _DZERO(Inners._D _D_1, Inners._ZERO _ZERO_1)
                {
                    this._D_1 = _D_1;
                    this._ZERO_1 = _ZERO_1;
                }
                
                public Inners._D _D_1 { get; }
                public Inners._ZERO _ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _DONE : _percentxZEROZEROⲻFF
            {
                public _DONE(Inners._D _D_1, Inners._ONE _ONE_1)
                {
                    this._D_1 = _D_1;
                    this._ONE_1 = _ONE_1;
                }
                
                public Inners._D _D_1 { get; }
                public Inners._ONE _ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _DTWO : _percentxZEROZEROⲻFF
            {
                public _DTWO(Inners._D _D_1, Inners._TWO _TWO_1)
                {
                    this._D_1 = _D_1;
                    this._TWO_1 = _TWO_1;
                }
                
                public Inners._D _D_1 { get; }
                public Inners._TWO _TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _DTHREE : _percentxZEROZEROⲻFF
            {
                public _DTHREE(Inners._D _D_1, Inners._THREE _THREE_1)
                {
                    this._D_1 = _D_1;
                    this._THREE_1 = _THREE_1;
                }
                
                public Inners._D _D_1 { get; }
                public Inners._THREE _THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _DFOUR : _percentxZEROZEROⲻFF
            {
                public _DFOUR(Inners._D _D_1, Inners._FOUR _FOUR_1)
                {
                    this._D_1 = _D_1;
                    this._FOUR_1 = _FOUR_1;
                }
                
                public Inners._D _D_1 { get; }
                public Inners._FOUR _FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _DFIVE : _percentxZEROZEROⲻFF
            {
                public _DFIVE(Inners._D _D_1, Inners._FIVE _FIVE_1)
                {
                    this._D_1 = _D_1;
                    this._FIVE_1 = _FIVE_1;
                }
                
                public Inners._D _D_1 { get; }
                public Inners._FIVE _FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _DSIX : _percentxZEROZEROⲻFF
            {
                public _DSIX(Inners._D _D_1, Inners._SIX _SIX_1)
                {
                    this._D_1 = _D_1;
                    this._SIX_1 = _SIX_1;
                }
                
                public Inners._D _D_1 { get; }
                public Inners._SIX _SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _DSEVEN : _percentxZEROZEROⲻFF
            {
                public _DSEVEN(Inners._D _D_1, Inners._SEVEN _SEVEN_1)
                {
                    this._D_1 = _D_1;
                    this._SEVEN_1 = _SEVEN_1;
                }
                
                public Inners._D _D_1 { get; }
                public Inners._SEVEN _SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _DEIGHT : _percentxZEROZEROⲻFF
            {
                public _DEIGHT(Inners._D _D_1, Inners._EIGHT _EIGHT_1)
                {
                    this._D_1 = _D_1;
                    this._EIGHT_1 = _EIGHT_1;
                }
                
                public Inners._D _D_1 { get; }
                public Inners._EIGHT _EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _DNINE : _percentxZEROZEROⲻFF
            {
                public _DNINE(Inners._D _D_1, Inners._NINE _NINE_1)
                {
                    this._D_1 = _D_1;
                    this._NINE_1 = _NINE_1;
                }
                
                public Inners._D _D_1 { get; }
                public Inners._NINE _NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _DA : _percentxZEROZEROⲻFF
            {
                public _DA(Inners._D _D_1, Inners._A _A_1)
                {
                    this._D_1 = _D_1;
                    this._A_1 = _A_1;
                }
                
                public Inners._D _D_1 { get; }
                public Inners._A _A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _DB : _percentxZEROZEROⲻFF
            {
                public _DB(Inners._D _D_1, Inners._B _B_1)
                {
                    this._D_1 = _D_1;
                    this._B_1 = _B_1;
                }
                
                public Inners._D _D_1 { get; }
                public Inners._B _B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _DC : _percentxZEROZEROⲻFF
            {
                public _DC(Inners._D _D_1, Inners._C _C_1)
                {
                    this._D_1 = _D_1;
                    this._C_1 = _C_1;
                }
                
                public Inners._D _D_1 { get; }
                public Inners._C _C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _DD : _percentxZEROZEROⲻFF
            {
                public _DD(Inners._D _D_1, Inners._D _D_2)
                {
                    this._D_1 = _D_1;
                    this._D_2 = _D_2;
                }
                
                public Inners._D _D_1 { get; }
                public Inners._D _D_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _DE : _percentxZEROZEROⲻFF
            {
                public _DE(Inners._D _D_1, Inners._E _E_1)
                {
                    this._D_1 = _D_1;
                    this._E_1 = _E_1;
                }
                
                public Inners._D _D_1 { get; }
                public Inners._E _E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _DF : _percentxZEROZEROⲻFF
            {
                public _DF(Inners._D _D_1, Inners._F _F_1)
                {
                    this._D_1 = _D_1;
                    this._F_1 = _F_1;
                }
                
                public Inners._D _D_1 { get; }
                public Inners._F _F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _EZERO : _percentxZEROZEROⲻFF
            {
                public _EZERO(Inners._E _E_1, Inners._ZERO _ZERO_1)
                {
                    this._E_1 = _E_1;
                    this._ZERO_1 = _ZERO_1;
                }
                
                public Inners._E _E_1 { get; }
                public Inners._ZERO _ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _EONE : _percentxZEROZEROⲻFF
            {
                public _EONE(Inners._E _E_1, Inners._ONE _ONE_1)
                {
                    this._E_1 = _E_1;
                    this._ONE_1 = _ONE_1;
                }
                
                public Inners._E _E_1 { get; }
                public Inners._ONE _ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ETWO : _percentxZEROZEROⲻFF
            {
                public _ETWO(Inners._E _E_1, Inners._TWO _TWO_1)
                {
                    this._E_1 = _E_1;
                    this._TWO_1 = _TWO_1;
                }
                
                public Inners._E _E_1 { get; }
                public Inners._TWO _TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ETHREE : _percentxZEROZEROⲻFF
            {
                public _ETHREE(Inners._E _E_1, Inners._THREE _THREE_1)
                {
                    this._E_1 = _E_1;
                    this._THREE_1 = _THREE_1;
                }
                
                public Inners._E _E_1 { get; }
                public Inners._THREE _THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _EFOUR : _percentxZEROZEROⲻFF
            {
                public _EFOUR(Inners._E _E_1, Inners._FOUR _FOUR_1)
                {
                    this._E_1 = _E_1;
                    this._FOUR_1 = _FOUR_1;
                }
                
                public Inners._E _E_1 { get; }
                public Inners._FOUR _FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _EFIVE : _percentxZEROZEROⲻFF
            {
                public _EFIVE(Inners._E _E_1, Inners._FIVE _FIVE_1)
                {
                    this._E_1 = _E_1;
                    this._FIVE_1 = _FIVE_1;
                }
                
                public Inners._E _E_1 { get; }
                public Inners._FIVE _FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ESIX : _percentxZEROZEROⲻFF
            {
                public _ESIX(Inners._E _E_1, Inners._SIX _SIX_1)
                {
                    this._E_1 = _E_1;
                    this._SIX_1 = _SIX_1;
                }
                
                public Inners._E _E_1 { get; }
                public Inners._SIX _SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ESEVEN : _percentxZEROZEROⲻFF
            {
                public _ESEVEN(Inners._E _E_1, Inners._SEVEN _SEVEN_1)
                {
                    this._E_1 = _E_1;
                    this._SEVEN_1 = _SEVEN_1;
                }
                
                public Inners._E _E_1 { get; }
                public Inners._SEVEN _SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _EEIGHT : _percentxZEROZEROⲻFF
            {
                public _EEIGHT(Inners._E _E_1, Inners._EIGHT _EIGHT_1)
                {
                    this._E_1 = _E_1;
                    this._EIGHT_1 = _EIGHT_1;
                }
                
                public Inners._E _E_1 { get; }
                public Inners._EIGHT _EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ENINE : _percentxZEROZEROⲻFF
            {
                public _ENINE(Inners._E _E_1, Inners._NINE _NINE_1)
                {
                    this._E_1 = _E_1;
                    this._NINE_1 = _NINE_1;
                }
                
                public Inners._E _E_1 { get; }
                public Inners._NINE _NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _EA : _percentxZEROZEROⲻFF
            {
                public _EA(Inners._E _E_1, Inners._A _A_1)
                {
                    this._E_1 = _E_1;
                    this._A_1 = _A_1;
                }
                
                public Inners._E _E_1 { get; }
                public Inners._A _A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _EB : _percentxZEROZEROⲻFF
            {
                public _EB(Inners._E _E_1, Inners._B _B_1)
                {
                    this._E_1 = _E_1;
                    this._B_1 = _B_1;
                }
                
                public Inners._E _E_1 { get; }
                public Inners._B _B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _EC : _percentxZEROZEROⲻFF
            {
                public _EC(Inners._E _E_1, Inners._C _C_1)
                {
                    this._E_1 = _E_1;
                    this._C_1 = _C_1;
                }
                
                public Inners._E _E_1 { get; }
                public Inners._C _C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ED : _percentxZEROZEROⲻFF
            {
                public _ED(Inners._E _E_1, Inners._D _D_1)
                {
                    this._E_1 = _E_1;
                    this._D_1 = _D_1;
                }
                
                public Inners._E _E_1 { get; }
                public Inners._D _D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _EE : _percentxZEROZEROⲻFF
            {
                public _EE(Inners._E _E_1, Inners._E _E_2)
                {
                    this._E_1 = _E_1;
                    this._E_2 = _E_2;
                }
                
                public Inners._E _E_1 { get; }
                public Inners._E _E_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _EF : _percentxZEROZEROⲻFF
            {
                public _EF(Inners._E _E_1, Inners._F _F_1)
                {
                    this._E_1 = _E_1;
                    this._F_1 = _F_1;
                }
                
                public Inners._E _E_1 { get; }
                public Inners._F _F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FZERO : _percentxZEROZEROⲻFF
            {
                public _FZERO(Inners._F _F_1, Inners._ZERO _ZERO_1)
                {
                    this._F_1 = _F_1;
                    this._ZERO_1 = _ZERO_1;
                }
                
                public Inners._F _F_1 { get; }
                public Inners._ZERO _ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FONE : _percentxZEROZEROⲻFF
            {
                public _FONE(Inners._F _F_1, Inners._ONE _ONE_1)
                {
                    this._F_1 = _F_1;
                    this._ONE_1 = _ONE_1;
                }
                
                public Inners._F _F_1 { get; }
                public Inners._ONE _ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FTWO : _percentxZEROZEROⲻFF
            {
                public _FTWO(Inners._F _F_1, Inners._TWO _TWO_1)
                {
                    this._F_1 = _F_1;
                    this._TWO_1 = _TWO_1;
                }
                
                public Inners._F _F_1 { get; }
                public Inners._TWO _TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FTHREE : _percentxZEROZEROⲻFF
            {
                public _FTHREE(Inners._F _F_1, Inners._THREE _THREE_1)
                {
                    this._F_1 = _F_1;
                    this._THREE_1 = _THREE_1;
                }
                
                public Inners._F _F_1 { get; }
                public Inners._THREE _THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FFOUR : _percentxZEROZEROⲻFF
            {
                public _FFOUR(Inners._F _F_1, Inners._FOUR _FOUR_1)
                {
                    this._F_1 = _F_1;
                    this._FOUR_1 = _FOUR_1;
                }
                
                public Inners._F _F_1 { get; }
                public Inners._FOUR _FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FFIVE : _percentxZEROZEROⲻFF
            {
                public _FFIVE(Inners._F _F_1, Inners._FIVE _FIVE_1)
                {
                    this._F_1 = _F_1;
                    this._FIVE_1 = _FIVE_1;
                }
                
                public Inners._F _F_1 { get; }
                public Inners._FIVE _FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FSIX : _percentxZEROZEROⲻFF
            {
                public _FSIX(Inners._F _F_1, Inners._SIX _SIX_1)
                {
                    this._F_1 = _F_1;
                    this._SIX_1 = _SIX_1;
                }
                
                public Inners._F _F_1 { get; }
                public Inners._SIX _SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FSEVEN : _percentxZEROZEROⲻFF
            {
                public _FSEVEN(Inners._F _F_1, Inners._SEVEN _SEVEN_1)
                {
                    this._F_1 = _F_1;
                    this._SEVEN_1 = _SEVEN_1;
                }
                
                public Inners._F _F_1 { get; }
                public Inners._SEVEN _SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FEIGHT : _percentxZEROZEROⲻFF
            {
                public _FEIGHT(Inners._F _F_1, Inners._EIGHT _EIGHT_1)
                {
                    this._F_1 = _F_1;
                    this._EIGHT_1 = _EIGHT_1;
                }
                
                public Inners._F _F_1 { get; }
                public Inners._EIGHT _EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FNINE : _percentxZEROZEROⲻFF
            {
                public _FNINE(Inners._F _F_1, Inners._NINE _NINE_1)
                {
                    this._F_1 = _F_1;
                    this._NINE_1 = _NINE_1;
                }
                
                public Inners._F _F_1 { get; }
                public Inners._NINE _NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FA : _percentxZEROZEROⲻFF
            {
                public _FA(Inners._F _F_1, Inners._A _A_1)
                {
                    this._F_1 = _F_1;
                    this._A_1 = _A_1;
                }
                
                public Inners._F _F_1 { get; }
                public Inners._A _A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FB : _percentxZEROZEROⲻFF
            {
                public _FB(Inners._F _F_1, Inners._B _B_1)
                {
                    this._F_1 = _F_1;
                    this._B_1 = _B_1;
                }
                
                public Inners._F _F_1 { get; }
                public Inners._B _B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FC : _percentxZEROZEROⲻFF
            {
                public _FC(Inners._F _F_1, Inners._C _C_1)
                {
                    this._F_1 = _F_1;
                    this._C_1 = _C_1;
                }
                
                public Inners._F _F_1 { get; }
                public Inners._C _C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FD : _percentxZEROZEROⲻFF
            {
                public _FD(Inners._F _F_1, Inners._D _D_1)
                {
                    this._F_1 = _F_1;
                    this._D_1 = _D_1;
                }
                
                public Inners._F _F_1 { get; }
                public Inners._D _D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FE : _percentxZEROZEROⲻFF
            {
                public _FE(Inners._F _F_1, Inners._E _E_1)
                {
                    this._F_1 = _F_1;
                    this._E_1 = _E_1;
                }
                
                public Inners._F _F_1 { get; }
                public Inners._E _E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FF : _percentxZEROZEROⲻFF
            {
                public _FF(Inners._F _F_1, Inners._F _F_2)
                {
                    this._F_1 = _F_1;
                    this._F_2 = _F_2;
                }
                
                public Inners._F _F_1 { get; }
                public Inners._F _F_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
        }
        
        public sealed class _percentxTWOZERO
        {
            public _percentxTWOZERO(Inners._TWO _TWO_1, Inners._ZERO _ZERO_1)
            {
                this._TWO_1 = _TWO_1;
                this._ZERO_1 = _ZERO_1;
            }
            
            public Inners._TWO _TWO_1 { get; }
            public Inners._ZERO _ZERO_1 { get; }
        }
        
        public abstract class _percentxTWOONEⲻSEVENE
        {
            private _percentxTWOONEⲻSEVENE()
            {
            }
            
            protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
            
            public abstract class Visitor<TResult, TContext>
            {
                public TResult Visit(_percentxTWOONEⲻSEVENE node, TContext context)
                {
                    return node.Dispatch(this, context);
                }
                
                protected internal abstract TResult Accept(_percentxTWOONEⲻSEVENE._TWOONE node, TContext context);
                protected internal abstract TResult Accept(_percentxTWOONEⲻSEVENE._TWOTWO node, TContext context);
                protected internal abstract TResult Accept(_percentxTWOONEⲻSEVENE._TWOTHREE node, TContext context);
                protected internal abstract TResult Accept(_percentxTWOONEⲻSEVENE._TWOFOUR node, TContext context);
                protected internal abstract TResult Accept(_percentxTWOONEⲻSEVENE._TWOFIVE node, TContext context);
                protected internal abstract TResult Accept(_percentxTWOONEⲻSEVENE._TWOSIX node, TContext context);
                protected internal abstract TResult Accept(_percentxTWOONEⲻSEVENE._TWOSEVEN node, TContext context);
                protected internal abstract TResult Accept(_percentxTWOONEⲻSEVENE._TWOEIGHT node, TContext context);
                protected internal abstract TResult Accept(_percentxTWOONEⲻSEVENE._TWONINE node, TContext context);
                protected internal abstract TResult Accept(_percentxTWOONEⲻSEVENE._TWOA node, TContext context);
                protected internal abstract TResult Accept(_percentxTWOONEⲻSEVENE._TWOB node, TContext context);
                protected internal abstract TResult Accept(_percentxTWOONEⲻSEVENE._TWOC node, TContext context);
                protected internal abstract TResult Accept(_percentxTWOONEⲻSEVENE._TWOD node, TContext context);
                protected internal abstract TResult Accept(_percentxTWOONEⲻSEVENE._TWOE node, TContext context);
                protected internal abstract TResult Accept(_percentxTWOONEⲻSEVENE._TWOF node, TContext context);
                protected internal abstract TResult Accept(_percentxTWOONEⲻSEVENE._THREEZERO node, TContext context);
                protected internal abstract TResult Accept(_percentxTWOONEⲻSEVENE._THREEONE node, TContext context);
                protected internal abstract TResult Accept(_percentxTWOONEⲻSEVENE._THREETWO node, TContext context);
                protected internal abstract TResult Accept(_percentxTWOONEⲻSEVENE._THREETHREE node, TContext context);
                protected internal abstract TResult Accept(_percentxTWOONEⲻSEVENE._THREEFOUR node, TContext context);
                protected internal abstract TResult Accept(_percentxTWOONEⲻSEVENE._THREEFIVE node, TContext context);
                protected internal abstract TResult Accept(_percentxTWOONEⲻSEVENE._THREESIX node, TContext context);
                protected internal abstract TResult Accept(_percentxTWOONEⲻSEVENE._THREESEVEN node, TContext context);
                protected internal abstract TResult Accept(_percentxTWOONEⲻSEVENE._THREEEIGHT node, TContext context);
                protected internal abstract TResult Accept(_percentxTWOONEⲻSEVENE._THREENINE node, TContext context);
                protected internal abstract TResult Accept(_percentxTWOONEⲻSEVENE._THREEA node, TContext context);
                protected internal abstract TResult Accept(_percentxTWOONEⲻSEVENE._THREEB node, TContext context);
                protected internal abstract TResult Accept(_percentxTWOONEⲻSEVENE._THREEC node, TContext context);
                protected internal abstract TResult Accept(_percentxTWOONEⲻSEVENE._THREED node, TContext context);
                protected internal abstract TResult Accept(_percentxTWOONEⲻSEVENE._THREEE node, TContext context);
                protected internal abstract TResult Accept(_percentxTWOONEⲻSEVENE._THREEF node, TContext context);
                protected internal abstract TResult Accept(_percentxTWOONEⲻSEVENE._FOURZERO node, TContext context);
                protected internal abstract TResult Accept(_percentxTWOONEⲻSEVENE._FOURONE node, TContext context);
                protected internal abstract TResult Accept(_percentxTWOONEⲻSEVENE._FOURTWO node, TContext context);
                protected internal abstract TResult Accept(_percentxTWOONEⲻSEVENE._FOURTHREE node, TContext context);
                protected internal abstract TResult Accept(_percentxTWOONEⲻSEVENE._FOURFOUR node, TContext context);
                protected internal abstract TResult Accept(_percentxTWOONEⲻSEVENE._FOURFIVE node, TContext context);
                protected internal abstract TResult Accept(_percentxTWOONEⲻSEVENE._FOURSIX node, TContext context);
                protected internal abstract TResult Accept(_percentxTWOONEⲻSEVENE._FOURSEVEN node, TContext context);
                protected internal abstract TResult Accept(_percentxTWOONEⲻSEVENE._FOUREIGHT node, TContext context);
                protected internal abstract TResult Accept(_percentxTWOONEⲻSEVENE._FOURNINE node, TContext context);
                protected internal abstract TResult Accept(_percentxTWOONEⲻSEVENE._FOURA node, TContext context);
                protected internal abstract TResult Accept(_percentxTWOONEⲻSEVENE._FOURB node, TContext context);
                protected internal abstract TResult Accept(_percentxTWOONEⲻSEVENE._FOURC node, TContext context);
                protected internal abstract TResult Accept(_percentxTWOONEⲻSEVENE._FOURD node, TContext context);
                protected internal abstract TResult Accept(_percentxTWOONEⲻSEVENE._FOURE node, TContext context);
                protected internal abstract TResult Accept(_percentxTWOONEⲻSEVENE._FOURF node, TContext context);
                protected internal abstract TResult Accept(_percentxTWOONEⲻSEVENE._FIVEZERO node, TContext context);
                protected internal abstract TResult Accept(_percentxTWOONEⲻSEVENE._FIVEONE node, TContext context);
                protected internal abstract TResult Accept(_percentxTWOONEⲻSEVENE._FIVETWO node, TContext context);
                protected internal abstract TResult Accept(_percentxTWOONEⲻSEVENE._FIVETHREE node, TContext context);
                protected internal abstract TResult Accept(_percentxTWOONEⲻSEVENE._FIVEFOUR node, TContext context);
                protected internal abstract TResult Accept(_percentxTWOONEⲻSEVENE._FIVEFIVE node, TContext context);
                protected internal abstract TResult Accept(_percentxTWOONEⲻSEVENE._FIVESIX node, TContext context);
                protected internal abstract TResult Accept(_percentxTWOONEⲻSEVENE._FIVESEVEN node, TContext context);
                protected internal abstract TResult Accept(_percentxTWOONEⲻSEVENE._FIVEEIGHT node, TContext context);
                protected internal abstract TResult Accept(_percentxTWOONEⲻSEVENE._FIVENINE node, TContext context);
                protected internal abstract TResult Accept(_percentxTWOONEⲻSEVENE._FIVEA node, TContext context);
                protected internal abstract TResult Accept(_percentxTWOONEⲻSEVENE._FIVEB node, TContext context);
                protected internal abstract TResult Accept(_percentxTWOONEⲻSEVENE._FIVEC node, TContext context);
                protected internal abstract TResult Accept(_percentxTWOONEⲻSEVENE._FIVED node, TContext context);
                protected internal abstract TResult Accept(_percentxTWOONEⲻSEVENE._FIVEE node, TContext context);
                protected internal abstract TResult Accept(_percentxTWOONEⲻSEVENE._FIVEF node, TContext context);
                protected internal abstract TResult Accept(_percentxTWOONEⲻSEVENE._SIXZERO node, TContext context);
                protected internal abstract TResult Accept(_percentxTWOONEⲻSEVENE._SIXONE node, TContext context);
                protected internal abstract TResult Accept(_percentxTWOONEⲻSEVENE._SIXTWO node, TContext context);
                protected internal abstract TResult Accept(_percentxTWOONEⲻSEVENE._SIXTHREE node, TContext context);
                protected internal abstract TResult Accept(_percentxTWOONEⲻSEVENE._SIXFOUR node, TContext context);
                protected internal abstract TResult Accept(_percentxTWOONEⲻSEVENE._SIXFIVE node, TContext context);
                protected internal abstract TResult Accept(_percentxTWOONEⲻSEVENE._SIXSIX node, TContext context);
                protected internal abstract TResult Accept(_percentxTWOONEⲻSEVENE._SIXSEVEN node, TContext context);
                protected internal abstract TResult Accept(_percentxTWOONEⲻSEVENE._SIXEIGHT node, TContext context);
                protected internal abstract TResult Accept(_percentxTWOONEⲻSEVENE._SIXNINE node, TContext context);
                protected internal abstract TResult Accept(_percentxTWOONEⲻSEVENE._SIXA node, TContext context);
                protected internal abstract TResult Accept(_percentxTWOONEⲻSEVENE._SIXB node, TContext context);
                protected internal abstract TResult Accept(_percentxTWOONEⲻSEVENE._SIXC node, TContext context);
                protected internal abstract TResult Accept(_percentxTWOONEⲻSEVENE._SIXD node, TContext context);
                protected internal abstract TResult Accept(_percentxTWOONEⲻSEVENE._SIXE node, TContext context);
                protected internal abstract TResult Accept(_percentxTWOONEⲻSEVENE._SIXF node, TContext context);
                protected internal abstract TResult Accept(_percentxTWOONEⲻSEVENE._SEVENZERO node, TContext context);
                protected internal abstract TResult Accept(_percentxTWOONEⲻSEVENE._SEVENONE node, TContext context);
                protected internal abstract TResult Accept(_percentxTWOONEⲻSEVENE._SEVENTWO node, TContext context);
                protected internal abstract TResult Accept(_percentxTWOONEⲻSEVENE._SEVENTHREE node, TContext context);
                protected internal abstract TResult Accept(_percentxTWOONEⲻSEVENE._SEVENFOUR node, TContext context);
                protected internal abstract TResult Accept(_percentxTWOONEⲻSEVENE._SEVENFIVE node, TContext context);
                protected internal abstract TResult Accept(_percentxTWOONEⲻSEVENE._SEVENSIX node, TContext context);
                protected internal abstract TResult Accept(_percentxTWOONEⲻSEVENE._SEVENSEVEN node, TContext context);
                protected internal abstract TResult Accept(_percentxTWOONEⲻSEVENE._SEVENEIGHT node, TContext context);
                protected internal abstract TResult Accept(_percentxTWOONEⲻSEVENE._SEVENNINE node, TContext context);
                protected internal abstract TResult Accept(_percentxTWOONEⲻSEVENE._SEVENA node, TContext context);
                protected internal abstract TResult Accept(_percentxTWOONEⲻSEVENE._SEVENB node, TContext context);
                protected internal abstract TResult Accept(_percentxTWOONEⲻSEVENE._SEVENC node, TContext context);
                protected internal abstract TResult Accept(_percentxTWOONEⲻSEVENE._SEVEND node, TContext context);
                protected internal abstract TResult Accept(_percentxTWOONEⲻSEVENE._SEVENE node, TContext context);
            }
            
            public sealed class _TWOONE : _percentxTWOONEⲻSEVENE
            {
                public _TWOONE(Inners._TWO _TWO_1, Inners._ONE _ONE_1)
                {
                    this._TWO_1 = _TWO_1;
                    this._ONE_1 = _ONE_1;
                }
                
                public Inners._TWO _TWO_1 { get; }
                public Inners._ONE _ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _TWOTWO : _percentxTWOONEⲻSEVENE
            {
                public _TWOTWO(Inners._TWO _TWO_1, Inners._TWO _TWO_2)
                {
                    this._TWO_1 = _TWO_1;
                    this._TWO_2 = _TWO_2;
                }
                
                public Inners._TWO _TWO_1 { get; }
                public Inners._TWO _TWO_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _TWOTHREE : _percentxTWOONEⲻSEVENE
            {
                public _TWOTHREE(Inners._TWO _TWO_1, Inners._THREE _THREE_1)
                {
                    this._TWO_1 = _TWO_1;
                    this._THREE_1 = _THREE_1;
                }
                
                public Inners._TWO _TWO_1 { get; }
                public Inners._THREE _THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _TWOFOUR : _percentxTWOONEⲻSEVENE
            {
                public _TWOFOUR(Inners._TWO _TWO_1, Inners._FOUR _FOUR_1)
                {
                    this._TWO_1 = _TWO_1;
                    this._FOUR_1 = _FOUR_1;
                }
                
                public Inners._TWO _TWO_1 { get; }
                public Inners._FOUR _FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _TWOFIVE : _percentxTWOONEⲻSEVENE
            {
                public _TWOFIVE(Inners._TWO _TWO_1, Inners._FIVE _FIVE_1)
                {
                    this._TWO_1 = _TWO_1;
                    this._FIVE_1 = _FIVE_1;
                }
                
                public Inners._TWO _TWO_1 { get; }
                public Inners._FIVE _FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _TWOSIX : _percentxTWOONEⲻSEVENE
            {
                public _TWOSIX(Inners._TWO _TWO_1, Inners._SIX _SIX_1)
                {
                    this._TWO_1 = _TWO_1;
                    this._SIX_1 = _SIX_1;
                }
                
                public Inners._TWO _TWO_1 { get; }
                public Inners._SIX _SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _TWOSEVEN : _percentxTWOONEⲻSEVENE
            {
                public _TWOSEVEN(Inners._TWO _TWO_1, Inners._SEVEN _SEVEN_1)
                {
                    this._TWO_1 = _TWO_1;
                    this._SEVEN_1 = _SEVEN_1;
                }
                
                public Inners._TWO _TWO_1 { get; }
                public Inners._SEVEN _SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _TWOEIGHT : _percentxTWOONEⲻSEVENE
            {
                public _TWOEIGHT(Inners._TWO _TWO_1, Inners._EIGHT _EIGHT_1)
                {
                    this._TWO_1 = _TWO_1;
                    this._EIGHT_1 = _EIGHT_1;
                }
                
                public Inners._TWO _TWO_1 { get; }
                public Inners._EIGHT _EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _TWONINE : _percentxTWOONEⲻSEVENE
            {
                public _TWONINE(Inners._TWO _TWO_1, Inners._NINE _NINE_1)
                {
                    this._TWO_1 = _TWO_1;
                    this._NINE_1 = _NINE_1;
                }
                
                public Inners._TWO _TWO_1 { get; }
                public Inners._NINE _NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _TWOA : _percentxTWOONEⲻSEVENE
            {
                public _TWOA(Inners._TWO _TWO_1, Inners._A _A_1)
                {
                    this._TWO_1 = _TWO_1;
                    this._A_1 = _A_1;
                }
                
                public Inners._TWO _TWO_1 { get; }
                public Inners._A _A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _TWOB : _percentxTWOONEⲻSEVENE
            {
                public _TWOB(Inners._TWO _TWO_1, Inners._B _B_1)
                {
                    this._TWO_1 = _TWO_1;
                    this._B_1 = _B_1;
                }
                
                public Inners._TWO _TWO_1 { get; }
                public Inners._B _B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _TWOC : _percentxTWOONEⲻSEVENE
            {
                public _TWOC(Inners._TWO _TWO_1, Inners._C _C_1)
                {
                    this._TWO_1 = _TWO_1;
                    this._C_1 = _C_1;
                }
                
                public Inners._TWO _TWO_1 { get; }
                public Inners._C _C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _TWOD : _percentxTWOONEⲻSEVENE
            {
                public _TWOD(Inners._TWO _TWO_1, Inners._D _D_1)
                {
                    this._TWO_1 = _TWO_1;
                    this._D_1 = _D_1;
                }
                
                public Inners._TWO _TWO_1 { get; }
                public Inners._D _D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _TWOE : _percentxTWOONEⲻSEVENE
            {
                public _TWOE(Inners._TWO _TWO_1, Inners._E _E_1)
                {
                    this._TWO_1 = _TWO_1;
                    this._E_1 = _E_1;
                }
                
                public Inners._TWO _TWO_1 { get; }
                public Inners._E _E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _TWOF : _percentxTWOONEⲻSEVENE
            {
                public _TWOF(Inners._TWO _TWO_1, Inners._F _F_1)
                {
                    this._TWO_1 = _TWO_1;
                    this._F_1 = _F_1;
                }
                
                public Inners._TWO _TWO_1 { get; }
                public Inners._F _F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _THREEZERO : _percentxTWOONEⲻSEVENE
            {
                public _THREEZERO(Inners._THREE _THREE_1, Inners._ZERO _ZERO_1)
                {
                    this._THREE_1 = _THREE_1;
                    this._ZERO_1 = _ZERO_1;
                }
                
                public Inners._THREE _THREE_1 { get; }
                public Inners._ZERO _ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _THREEONE : _percentxTWOONEⲻSEVENE
            {
                public _THREEONE(Inners._THREE _THREE_1, Inners._ONE _ONE_1)
                {
                    this._THREE_1 = _THREE_1;
                    this._ONE_1 = _ONE_1;
                }
                
                public Inners._THREE _THREE_1 { get; }
                public Inners._ONE _ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _THREETWO : _percentxTWOONEⲻSEVENE
            {
                public _THREETWO(Inners._THREE _THREE_1, Inners._TWO _TWO_1)
                {
                    this._THREE_1 = _THREE_1;
                    this._TWO_1 = _TWO_1;
                }
                
                public Inners._THREE _THREE_1 { get; }
                public Inners._TWO _TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _THREETHREE : _percentxTWOONEⲻSEVENE
            {
                public _THREETHREE(Inners._THREE _THREE_1, Inners._THREE _THREE_2)
                {
                    this._THREE_1 = _THREE_1;
                    this._THREE_2 = _THREE_2;
                }
                
                public Inners._THREE _THREE_1 { get; }
                public Inners._THREE _THREE_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _THREEFOUR : _percentxTWOONEⲻSEVENE
            {
                public _THREEFOUR(Inners._THREE _THREE_1, Inners._FOUR _FOUR_1)
                {
                    this._THREE_1 = _THREE_1;
                    this._FOUR_1 = _FOUR_1;
                }
                
                public Inners._THREE _THREE_1 { get; }
                public Inners._FOUR _FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _THREEFIVE : _percentxTWOONEⲻSEVENE
            {
                public _THREEFIVE(Inners._THREE _THREE_1, Inners._FIVE _FIVE_1)
                {
                    this._THREE_1 = _THREE_1;
                    this._FIVE_1 = _FIVE_1;
                }
                
                public Inners._THREE _THREE_1 { get; }
                public Inners._FIVE _FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _THREESIX : _percentxTWOONEⲻSEVENE
            {
                public _THREESIX(Inners._THREE _THREE_1, Inners._SIX _SIX_1)
                {
                    this._THREE_1 = _THREE_1;
                    this._SIX_1 = _SIX_1;
                }
                
                public Inners._THREE _THREE_1 { get; }
                public Inners._SIX _SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _THREESEVEN : _percentxTWOONEⲻSEVENE
            {
                public _THREESEVEN(Inners._THREE _THREE_1, Inners._SEVEN _SEVEN_1)
                {
                    this._THREE_1 = _THREE_1;
                    this._SEVEN_1 = _SEVEN_1;
                }
                
                public Inners._THREE _THREE_1 { get; }
                public Inners._SEVEN _SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _THREEEIGHT : _percentxTWOONEⲻSEVENE
            {
                public _THREEEIGHT(Inners._THREE _THREE_1, Inners._EIGHT _EIGHT_1)
                {
                    this._THREE_1 = _THREE_1;
                    this._EIGHT_1 = _EIGHT_1;
                }
                
                public Inners._THREE _THREE_1 { get; }
                public Inners._EIGHT _EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _THREENINE : _percentxTWOONEⲻSEVENE
            {
                public _THREENINE(Inners._THREE _THREE_1, Inners._NINE _NINE_1)
                {
                    this._THREE_1 = _THREE_1;
                    this._NINE_1 = _NINE_1;
                }
                
                public Inners._THREE _THREE_1 { get; }
                public Inners._NINE _NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _THREEA : _percentxTWOONEⲻSEVENE
            {
                public _THREEA(Inners._THREE _THREE_1, Inners._A _A_1)
                {
                    this._THREE_1 = _THREE_1;
                    this._A_1 = _A_1;
                }
                
                public Inners._THREE _THREE_1 { get; }
                public Inners._A _A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _THREEB : _percentxTWOONEⲻSEVENE
            {
                public _THREEB(Inners._THREE _THREE_1, Inners._B _B_1)
                {
                    this._THREE_1 = _THREE_1;
                    this._B_1 = _B_1;
                }
                
                public Inners._THREE _THREE_1 { get; }
                public Inners._B _B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _THREEC : _percentxTWOONEⲻSEVENE
            {
                public _THREEC(Inners._THREE _THREE_1, Inners._C _C_1)
                {
                    this._THREE_1 = _THREE_1;
                    this._C_1 = _C_1;
                }
                
                public Inners._THREE _THREE_1 { get; }
                public Inners._C _C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _THREED : _percentxTWOONEⲻSEVENE
            {
                public _THREED(Inners._THREE _THREE_1, Inners._D _D_1)
                {
                    this._THREE_1 = _THREE_1;
                    this._D_1 = _D_1;
                }
                
                public Inners._THREE _THREE_1 { get; }
                public Inners._D _D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _THREEE : _percentxTWOONEⲻSEVENE
            {
                public _THREEE(Inners._THREE _THREE_1, Inners._E _E_1)
                {
                    this._THREE_1 = _THREE_1;
                    this._E_1 = _E_1;
                }
                
                public Inners._THREE _THREE_1 { get; }
                public Inners._E _E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _THREEF : _percentxTWOONEⲻSEVENE
            {
                public _THREEF(Inners._THREE _THREE_1, Inners._F _F_1)
                {
                    this._THREE_1 = _THREE_1;
                    this._F_1 = _F_1;
                }
                
                public Inners._THREE _THREE_1 { get; }
                public Inners._F _F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FOURZERO : _percentxTWOONEⲻSEVENE
            {
                public _FOURZERO(Inners._FOUR _FOUR_1, Inners._ZERO _ZERO_1)
                {
                    this._FOUR_1 = _FOUR_1;
                    this._ZERO_1 = _ZERO_1;
                }
                
                public Inners._FOUR _FOUR_1 { get; }
                public Inners._ZERO _ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FOURONE : _percentxTWOONEⲻSEVENE
            {
                public _FOURONE(Inners._FOUR _FOUR_1, Inners._ONE _ONE_1)
                {
                    this._FOUR_1 = _FOUR_1;
                    this._ONE_1 = _ONE_1;
                }
                
                public Inners._FOUR _FOUR_1 { get; }
                public Inners._ONE _ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FOURTWO : _percentxTWOONEⲻSEVENE
            {
                public _FOURTWO(Inners._FOUR _FOUR_1, Inners._TWO _TWO_1)
                {
                    this._FOUR_1 = _FOUR_1;
                    this._TWO_1 = _TWO_1;
                }
                
                public Inners._FOUR _FOUR_1 { get; }
                public Inners._TWO _TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FOURTHREE : _percentxTWOONEⲻSEVENE
            {
                public _FOURTHREE(Inners._FOUR _FOUR_1, Inners._THREE _THREE_1)
                {
                    this._FOUR_1 = _FOUR_1;
                    this._THREE_1 = _THREE_1;
                }
                
                public Inners._FOUR _FOUR_1 { get; }
                public Inners._THREE _THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FOURFOUR : _percentxTWOONEⲻSEVENE
            {
                public _FOURFOUR(Inners._FOUR _FOUR_1, Inners._FOUR _FOUR_2)
                {
                    this._FOUR_1 = _FOUR_1;
                    this._FOUR_2 = _FOUR_2;
                }
                
                public Inners._FOUR _FOUR_1 { get; }
                public Inners._FOUR _FOUR_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FOURFIVE : _percentxTWOONEⲻSEVENE
            {
                public _FOURFIVE(Inners._FOUR _FOUR_1, Inners._FIVE _FIVE_1)
                {
                    this._FOUR_1 = _FOUR_1;
                    this._FIVE_1 = _FIVE_1;
                }
                
                public Inners._FOUR _FOUR_1 { get; }
                public Inners._FIVE _FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FOURSIX : _percentxTWOONEⲻSEVENE
            {
                public _FOURSIX(Inners._FOUR _FOUR_1, Inners._SIX _SIX_1)
                {
                    this._FOUR_1 = _FOUR_1;
                    this._SIX_1 = _SIX_1;
                }
                
                public Inners._FOUR _FOUR_1 { get; }
                public Inners._SIX _SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FOURSEVEN : _percentxTWOONEⲻSEVENE
            {
                public _FOURSEVEN(Inners._FOUR _FOUR_1, Inners._SEVEN _SEVEN_1)
                {
                    this._FOUR_1 = _FOUR_1;
                    this._SEVEN_1 = _SEVEN_1;
                }
                
                public Inners._FOUR _FOUR_1 { get; }
                public Inners._SEVEN _SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FOUREIGHT : _percentxTWOONEⲻSEVENE
            {
                public _FOUREIGHT(Inners._FOUR _FOUR_1, Inners._EIGHT _EIGHT_1)
                {
                    this._FOUR_1 = _FOUR_1;
                    this._EIGHT_1 = _EIGHT_1;
                }
                
                public Inners._FOUR _FOUR_1 { get; }
                public Inners._EIGHT _EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FOURNINE : _percentxTWOONEⲻSEVENE
            {
                public _FOURNINE(Inners._FOUR _FOUR_1, Inners._NINE _NINE_1)
                {
                    this._FOUR_1 = _FOUR_1;
                    this._NINE_1 = _NINE_1;
                }
                
                public Inners._FOUR _FOUR_1 { get; }
                public Inners._NINE _NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FOURA : _percentxTWOONEⲻSEVENE
            {
                public _FOURA(Inners._FOUR _FOUR_1, Inners._A _A_1)
                {
                    this._FOUR_1 = _FOUR_1;
                    this._A_1 = _A_1;
                }
                
                public Inners._FOUR _FOUR_1 { get; }
                public Inners._A _A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FOURB : _percentxTWOONEⲻSEVENE
            {
                public _FOURB(Inners._FOUR _FOUR_1, Inners._B _B_1)
                {
                    this._FOUR_1 = _FOUR_1;
                    this._B_1 = _B_1;
                }
                
                public Inners._FOUR _FOUR_1 { get; }
                public Inners._B _B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FOURC : _percentxTWOONEⲻSEVENE
            {
                public _FOURC(Inners._FOUR _FOUR_1, Inners._C _C_1)
                {
                    this._FOUR_1 = _FOUR_1;
                    this._C_1 = _C_1;
                }
                
                public Inners._FOUR _FOUR_1 { get; }
                public Inners._C _C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FOURD : _percentxTWOONEⲻSEVENE
            {
                public _FOURD(Inners._FOUR _FOUR_1, Inners._D _D_1)
                {
                    this._FOUR_1 = _FOUR_1;
                    this._D_1 = _D_1;
                }
                
                public Inners._FOUR _FOUR_1 { get; }
                public Inners._D _D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FOURE : _percentxTWOONEⲻSEVENE
            {
                public _FOURE(Inners._FOUR _FOUR_1, Inners._E _E_1)
                {
                    this._FOUR_1 = _FOUR_1;
                    this._E_1 = _E_1;
                }
                
                public Inners._FOUR _FOUR_1 { get; }
                public Inners._E _E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FOURF : _percentxTWOONEⲻSEVENE
            {
                public _FOURF(Inners._FOUR _FOUR_1, Inners._F _F_1)
                {
                    this._FOUR_1 = _FOUR_1;
                    this._F_1 = _F_1;
                }
                
                public Inners._FOUR _FOUR_1 { get; }
                public Inners._F _F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FIVEZERO : _percentxTWOONEⲻSEVENE
            {
                public _FIVEZERO(Inners._FIVE _FIVE_1, Inners._ZERO _ZERO_1)
                {
                    this._FIVE_1 = _FIVE_1;
                    this._ZERO_1 = _ZERO_1;
                }
                
                public Inners._FIVE _FIVE_1 { get; }
                public Inners._ZERO _ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FIVEONE : _percentxTWOONEⲻSEVENE
            {
                public _FIVEONE(Inners._FIVE _FIVE_1, Inners._ONE _ONE_1)
                {
                    this._FIVE_1 = _FIVE_1;
                    this._ONE_1 = _ONE_1;
                }
                
                public Inners._FIVE _FIVE_1 { get; }
                public Inners._ONE _ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FIVETWO : _percentxTWOONEⲻSEVENE
            {
                public _FIVETWO(Inners._FIVE _FIVE_1, Inners._TWO _TWO_1)
                {
                    this._FIVE_1 = _FIVE_1;
                    this._TWO_1 = _TWO_1;
                }
                
                public Inners._FIVE _FIVE_1 { get; }
                public Inners._TWO _TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FIVETHREE : _percentxTWOONEⲻSEVENE
            {
                public _FIVETHREE(Inners._FIVE _FIVE_1, Inners._THREE _THREE_1)
                {
                    this._FIVE_1 = _FIVE_1;
                    this._THREE_1 = _THREE_1;
                }
                
                public Inners._FIVE _FIVE_1 { get; }
                public Inners._THREE _THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FIVEFOUR : _percentxTWOONEⲻSEVENE
            {
                public _FIVEFOUR(Inners._FIVE _FIVE_1, Inners._FOUR _FOUR_1)
                {
                    this._FIVE_1 = _FIVE_1;
                    this._FOUR_1 = _FOUR_1;
                }
                
                public Inners._FIVE _FIVE_1 { get; }
                public Inners._FOUR _FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FIVEFIVE : _percentxTWOONEⲻSEVENE
            {
                public _FIVEFIVE(Inners._FIVE _FIVE_1, Inners._FIVE _FIVE_2)
                {
                    this._FIVE_1 = _FIVE_1;
                    this._FIVE_2 = _FIVE_2;
                }
                
                public Inners._FIVE _FIVE_1 { get; }
                public Inners._FIVE _FIVE_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FIVESIX : _percentxTWOONEⲻSEVENE
            {
                public _FIVESIX(Inners._FIVE _FIVE_1, Inners._SIX _SIX_1)
                {
                    this._FIVE_1 = _FIVE_1;
                    this._SIX_1 = _SIX_1;
                }
                
                public Inners._FIVE _FIVE_1 { get; }
                public Inners._SIX _SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FIVESEVEN : _percentxTWOONEⲻSEVENE
            {
                public _FIVESEVEN(Inners._FIVE _FIVE_1, Inners._SEVEN _SEVEN_1)
                {
                    this._FIVE_1 = _FIVE_1;
                    this._SEVEN_1 = _SEVEN_1;
                }
                
                public Inners._FIVE _FIVE_1 { get; }
                public Inners._SEVEN _SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FIVEEIGHT : _percentxTWOONEⲻSEVENE
            {
                public _FIVEEIGHT(Inners._FIVE _FIVE_1, Inners._EIGHT _EIGHT_1)
                {
                    this._FIVE_1 = _FIVE_1;
                    this._EIGHT_1 = _EIGHT_1;
                }
                
                public Inners._FIVE _FIVE_1 { get; }
                public Inners._EIGHT _EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FIVENINE : _percentxTWOONEⲻSEVENE
            {
                public _FIVENINE(Inners._FIVE _FIVE_1, Inners._NINE _NINE_1)
                {
                    this._FIVE_1 = _FIVE_1;
                    this._NINE_1 = _NINE_1;
                }
                
                public Inners._FIVE _FIVE_1 { get; }
                public Inners._NINE _NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FIVEA : _percentxTWOONEⲻSEVENE
            {
                public _FIVEA(Inners._FIVE _FIVE_1, Inners._A _A_1)
                {
                    this._FIVE_1 = _FIVE_1;
                    this._A_1 = _A_1;
                }
                
                public Inners._FIVE _FIVE_1 { get; }
                public Inners._A _A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FIVEB : _percentxTWOONEⲻSEVENE
            {
                public _FIVEB(Inners._FIVE _FIVE_1, Inners._B _B_1)
                {
                    this._FIVE_1 = _FIVE_1;
                    this._B_1 = _B_1;
                }
                
                public Inners._FIVE _FIVE_1 { get; }
                public Inners._B _B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FIVEC : _percentxTWOONEⲻSEVENE
            {
                public _FIVEC(Inners._FIVE _FIVE_1, Inners._C _C_1)
                {
                    this._FIVE_1 = _FIVE_1;
                    this._C_1 = _C_1;
                }
                
                public Inners._FIVE _FIVE_1 { get; }
                public Inners._C _C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FIVED : _percentxTWOONEⲻSEVENE
            {
                public _FIVED(Inners._FIVE _FIVE_1, Inners._D _D_1)
                {
                    this._FIVE_1 = _FIVE_1;
                    this._D_1 = _D_1;
                }
                
                public Inners._FIVE _FIVE_1 { get; }
                public Inners._D _D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FIVEE : _percentxTWOONEⲻSEVENE
            {
                public _FIVEE(Inners._FIVE _FIVE_1, Inners._E _E_1)
                {
                    this._FIVE_1 = _FIVE_1;
                    this._E_1 = _E_1;
                }
                
                public Inners._FIVE _FIVE_1 { get; }
                public Inners._E _E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FIVEF : _percentxTWOONEⲻSEVENE
            {
                public _FIVEF(Inners._FIVE _FIVE_1, Inners._F _F_1)
                {
                    this._FIVE_1 = _FIVE_1;
                    this._F_1 = _F_1;
                }
                
                public Inners._FIVE _FIVE_1 { get; }
                public Inners._F _F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SIXZERO : _percentxTWOONEⲻSEVENE
            {
                public _SIXZERO(Inners._SIX _SIX_1, Inners._ZERO _ZERO_1)
                {
                    this._SIX_1 = _SIX_1;
                    this._ZERO_1 = _ZERO_1;
                }
                
                public Inners._SIX _SIX_1 { get; }
                public Inners._ZERO _ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SIXONE : _percentxTWOONEⲻSEVENE
            {
                public _SIXONE(Inners._SIX _SIX_1, Inners._ONE _ONE_1)
                {
                    this._SIX_1 = _SIX_1;
                    this._ONE_1 = _ONE_1;
                }
                
                public Inners._SIX _SIX_1 { get; }
                public Inners._ONE _ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SIXTWO : _percentxTWOONEⲻSEVENE
            {
                public _SIXTWO(Inners._SIX _SIX_1, Inners._TWO _TWO_1)
                {
                    this._SIX_1 = _SIX_1;
                    this._TWO_1 = _TWO_1;
                }
                
                public Inners._SIX _SIX_1 { get; }
                public Inners._TWO _TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SIXTHREE : _percentxTWOONEⲻSEVENE
            {
                public _SIXTHREE(Inners._SIX _SIX_1, Inners._THREE _THREE_1)
                {
                    this._SIX_1 = _SIX_1;
                    this._THREE_1 = _THREE_1;
                }
                
                public Inners._SIX _SIX_1 { get; }
                public Inners._THREE _THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SIXFOUR : _percentxTWOONEⲻSEVENE
            {
                public _SIXFOUR(Inners._SIX _SIX_1, Inners._FOUR _FOUR_1)
                {
                    this._SIX_1 = _SIX_1;
                    this._FOUR_1 = _FOUR_1;
                }
                
                public Inners._SIX _SIX_1 { get; }
                public Inners._FOUR _FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SIXFIVE : _percentxTWOONEⲻSEVENE
            {
                public _SIXFIVE(Inners._SIX _SIX_1, Inners._FIVE _FIVE_1)
                {
                    this._SIX_1 = _SIX_1;
                    this._FIVE_1 = _FIVE_1;
                }
                
                public Inners._SIX _SIX_1 { get; }
                public Inners._FIVE _FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SIXSIX : _percentxTWOONEⲻSEVENE
            {
                public _SIXSIX(Inners._SIX _SIX_1, Inners._SIX _SIX_2)
                {
                    this._SIX_1 = _SIX_1;
                    this._SIX_2 = _SIX_2;
                }
                
                public Inners._SIX _SIX_1 { get; }
                public Inners._SIX _SIX_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SIXSEVEN : _percentxTWOONEⲻSEVENE
            {
                public _SIXSEVEN(Inners._SIX _SIX_1, Inners._SEVEN _SEVEN_1)
                {
                    this._SIX_1 = _SIX_1;
                    this._SEVEN_1 = _SEVEN_1;
                }
                
                public Inners._SIX _SIX_1 { get; }
                public Inners._SEVEN _SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SIXEIGHT : _percentxTWOONEⲻSEVENE
            {
                public _SIXEIGHT(Inners._SIX _SIX_1, Inners._EIGHT _EIGHT_1)
                {
                    this._SIX_1 = _SIX_1;
                    this._EIGHT_1 = _EIGHT_1;
                }
                
                public Inners._SIX _SIX_1 { get; }
                public Inners._EIGHT _EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SIXNINE : _percentxTWOONEⲻSEVENE
            {
                public _SIXNINE(Inners._SIX _SIX_1, Inners._NINE _NINE_1)
                {
                    this._SIX_1 = _SIX_1;
                    this._NINE_1 = _NINE_1;
                }
                
                public Inners._SIX _SIX_1 { get; }
                public Inners._NINE _NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SIXA : _percentxTWOONEⲻSEVENE
            {
                public _SIXA(Inners._SIX _SIX_1, Inners._A _A_1)
                {
                    this._SIX_1 = _SIX_1;
                    this._A_1 = _A_1;
                }
                
                public Inners._SIX _SIX_1 { get; }
                public Inners._A _A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SIXB : _percentxTWOONEⲻSEVENE
            {
                public _SIXB(Inners._SIX _SIX_1, Inners._B _B_1)
                {
                    this._SIX_1 = _SIX_1;
                    this._B_1 = _B_1;
                }
                
                public Inners._SIX _SIX_1 { get; }
                public Inners._B _B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SIXC : _percentxTWOONEⲻSEVENE
            {
                public _SIXC(Inners._SIX _SIX_1, Inners._C _C_1)
                {
                    this._SIX_1 = _SIX_1;
                    this._C_1 = _C_1;
                }
                
                public Inners._SIX _SIX_1 { get; }
                public Inners._C _C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SIXD : _percentxTWOONEⲻSEVENE
            {
                public _SIXD(Inners._SIX _SIX_1, Inners._D _D_1)
                {
                    this._SIX_1 = _SIX_1;
                    this._D_1 = _D_1;
                }
                
                public Inners._SIX _SIX_1 { get; }
                public Inners._D _D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SIXE : _percentxTWOONEⲻSEVENE
            {
                public _SIXE(Inners._SIX _SIX_1, Inners._E _E_1)
                {
                    this._SIX_1 = _SIX_1;
                    this._E_1 = _E_1;
                }
                
                public Inners._SIX _SIX_1 { get; }
                public Inners._E _E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SIXF : _percentxTWOONEⲻSEVENE
            {
                public _SIXF(Inners._SIX _SIX_1, Inners._F _F_1)
                {
                    this._SIX_1 = _SIX_1;
                    this._F_1 = _F_1;
                }
                
                public Inners._SIX _SIX_1 { get; }
                public Inners._F _F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SEVENZERO : _percentxTWOONEⲻSEVENE
            {
                public _SEVENZERO(Inners._SEVEN _SEVEN_1, Inners._ZERO _ZERO_1)
                {
                    this._SEVEN_1 = _SEVEN_1;
                    this._ZERO_1 = _ZERO_1;
                }
                
                public Inners._SEVEN _SEVEN_1 { get; }
                public Inners._ZERO _ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SEVENONE : _percentxTWOONEⲻSEVENE
            {
                public _SEVENONE(Inners._SEVEN _SEVEN_1, Inners._ONE _ONE_1)
                {
                    this._SEVEN_1 = _SEVEN_1;
                    this._ONE_1 = _ONE_1;
                }
                
                public Inners._SEVEN _SEVEN_1 { get; }
                public Inners._ONE _ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SEVENTWO : _percentxTWOONEⲻSEVENE
            {
                public _SEVENTWO(Inners._SEVEN _SEVEN_1, Inners._TWO _TWO_1)
                {
                    this._SEVEN_1 = _SEVEN_1;
                    this._TWO_1 = _TWO_1;
                }
                
                public Inners._SEVEN _SEVEN_1 { get; }
                public Inners._TWO _TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SEVENTHREE : _percentxTWOONEⲻSEVENE
            {
                public _SEVENTHREE(Inners._SEVEN _SEVEN_1, Inners._THREE _THREE_1)
                {
                    this._SEVEN_1 = _SEVEN_1;
                    this._THREE_1 = _THREE_1;
                }
                
                public Inners._SEVEN _SEVEN_1 { get; }
                public Inners._THREE _THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SEVENFOUR : _percentxTWOONEⲻSEVENE
            {
                public _SEVENFOUR(Inners._SEVEN _SEVEN_1, Inners._FOUR _FOUR_1)
                {
                    this._SEVEN_1 = _SEVEN_1;
                    this._FOUR_1 = _FOUR_1;
                }
                
                public Inners._SEVEN _SEVEN_1 { get; }
                public Inners._FOUR _FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SEVENFIVE : _percentxTWOONEⲻSEVENE
            {
                public _SEVENFIVE(Inners._SEVEN _SEVEN_1, Inners._FIVE _FIVE_1)
                {
                    this._SEVEN_1 = _SEVEN_1;
                    this._FIVE_1 = _FIVE_1;
                }
                
                public Inners._SEVEN _SEVEN_1 { get; }
                public Inners._FIVE _FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SEVENSIX : _percentxTWOONEⲻSEVENE
            {
                public _SEVENSIX(Inners._SEVEN _SEVEN_1, Inners._SIX _SIX_1)
                {
                    this._SEVEN_1 = _SEVEN_1;
                    this._SIX_1 = _SIX_1;
                }
                
                public Inners._SEVEN _SEVEN_1 { get; }
                public Inners._SIX _SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SEVENSEVEN : _percentxTWOONEⲻSEVENE
            {
                public _SEVENSEVEN(Inners._SEVEN _SEVEN_1, Inners._SEVEN _SEVEN_2)
                {
                    this._SEVEN_1 = _SEVEN_1;
                    this._SEVEN_2 = _SEVEN_2;
                }
                
                public Inners._SEVEN _SEVEN_1 { get; }
                public Inners._SEVEN _SEVEN_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SEVENEIGHT : _percentxTWOONEⲻSEVENE
            {
                public _SEVENEIGHT(Inners._SEVEN _SEVEN_1, Inners._EIGHT _EIGHT_1)
                {
                    this._SEVEN_1 = _SEVEN_1;
                    this._EIGHT_1 = _EIGHT_1;
                }
                
                public Inners._SEVEN _SEVEN_1 { get; }
                public Inners._EIGHT _EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SEVENNINE : _percentxTWOONEⲻSEVENE
            {
                public _SEVENNINE(Inners._SEVEN _SEVEN_1, Inners._NINE _NINE_1)
                {
                    this._SEVEN_1 = _SEVEN_1;
                    this._NINE_1 = _NINE_1;
                }
                
                public Inners._SEVEN _SEVEN_1 { get; }
                public Inners._NINE _NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SEVENA : _percentxTWOONEⲻSEVENE
            {
                public _SEVENA(Inners._SEVEN _SEVEN_1, Inners._A _A_1)
                {
                    this._SEVEN_1 = _SEVEN_1;
                    this._A_1 = _A_1;
                }
                
                public Inners._SEVEN _SEVEN_1 { get; }
                public Inners._A _A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SEVENB : _percentxTWOONEⲻSEVENE
            {
                public _SEVENB(Inners._SEVEN _SEVEN_1, Inners._B _B_1)
                {
                    this._SEVEN_1 = _SEVEN_1;
                    this._B_1 = _B_1;
                }
                
                public Inners._SEVEN _SEVEN_1 { get; }
                public Inners._B _B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SEVENC : _percentxTWOONEⲻSEVENE
            {
                public _SEVENC(Inners._SEVEN _SEVEN_1, Inners._C _C_1)
                {
                    this._SEVEN_1 = _SEVEN_1;
                    this._C_1 = _C_1;
                }
                
                public Inners._SEVEN _SEVEN_1 { get; }
                public Inners._C _C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SEVEND : _percentxTWOONEⲻSEVENE
            {
                public _SEVEND(Inners._SEVEN _SEVEN_1, Inners._D _D_1)
                {
                    this._SEVEN_1 = _SEVEN_1;
                    this._D_1 = _D_1;
                }
                
                public Inners._SEVEN _SEVEN_1 { get; }
                public Inners._D _D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _SEVENE : _percentxTWOONEⲻSEVENE
            {
                public _SEVENE(Inners._SEVEN _SEVEN_1, Inners._E _E_1)
                {
                    this._SEVEN_1 = _SEVEN_1;
                    this._E_1 = _E_1;
                }
                
                public Inners._SEVEN _SEVEN_1 { get; }
                public Inners._E _E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
        }
    }
    
}
