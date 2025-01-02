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
            
            protected internal abstract TResult Accept(_ALPHA._percentx41ⲻ5A node, TContext context);
            protected internal abstract TResult Accept(_ALPHA._percentx61ⲻ7A node, TContext context);
        }
        
        public sealed class _percentx41ⲻ5A : _ALPHA
        {
            public _percentx41ⲻ5A(Inners._percentx41ⲻ5A _percentx41ⲻ5A_1)
            {
                this._percentx41ⲻ5A_1 = _percentx41ⲻ5A_1;
            }
            
            public Inners._percentx41ⲻ5A _percentx41ⲻ5A_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _percentx61ⲻ7A : _ALPHA
        {
            public _percentx61ⲻ7A(Inners._percentx61ⲻ7A _percentx61ⲻ7A_1)
            {
                this._percentx61ⲻ7A_1 = _percentx61ⲻ7A_1;
            }
            
            public Inners._percentx61ⲻ7A _percentx61ⲻ7A_1 { get; }
            
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
        public _CHAR(Inners._percentx01ⲻ7F _percentx01ⲻ7F_1)
        {
            this._percentx01ⲻ7F_1 = _percentx01ⲻ7F_1;
        }
        
        public Inners._percentx01ⲻ7F _percentx01ⲻ7F_1 { get; }
    }
    
    public sealed class _CR
    {
        public _CR(Inners._percentx0D _percentx0D_1)
        {
            this._percentx0D_1 = _percentx0D_1;
        }
        
        public Inners._percentx0D _percentx0D_1 { get; }
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
            
            protected internal abstract TResult Accept(_CTL._percentx00ⲻ1F node, TContext context);
            protected internal abstract TResult Accept(_CTL._percentx7F node, TContext context);
        }
        
        public sealed class _percentx00ⲻ1F : _CTL
        {
            public _percentx00ⲻ1F(Inners._percentx00ⲻ1F _percentx00ⲻ1F_1)
            {
                this._percentx00ⲻ1F_1 = _percentx00ⲻ1F_1;
            }
            
            public Inners._percentx00ⲻ1F _percentx00ⲻ1F_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _percentx7F : _CTL
        {
            public _percentx7F(Inners._percentx7F _percentx7F_1)
            {
                this._percentx7F_1 = _percentx7F_1;
            }
            
            public Inners._percentx7F _percentx7F_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
    public sealed class _DIGIT
    {
        public _DIGIT(Inners._percentx30ⲻ39 _percentx30ⲻ39_1)
        {
            this._percentx30ⲻ39_1 = _percentx30ⲻ39_1;
        }
        
        public Inners._percentx30ⲻ39 _percentx30ⲻ39_1 { get; }
    }
    
    public sealed class _DQUOTE
    {
        public _DQUOTE(Inners._percentx22 _percentx22_1)
        {
            this._percentx22_1 = _percentx22_1;
        }
        
        public Inners._percentx22 _percentx22_1 { get; }
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
        public _HTAB(Inners._percentx09 _percentx09_1)
        {
            this._percentx09_1 = _percentx09_1;
        }
        
        public Inners._percentx09 _percentx09_1 { get; }
    }
    
    public sealed class _LF
    {
        public _LF(Inners._percentx0A _percentx0A_1)
        {
            this._percentx0A_1 = _percentx0A_1;
        }
        
        public Inners._percentx0A _percentx0A_1 { get; }
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
        public _OCTET(Inners._percentx00ⲻFF _percentx00ⲻFF_1)
        {
            this._percentx00ⲻFF_1 = _percentx00ⲻFF_1;
        }
        
        public Inners._percentx00ⲻFF _percentx00ⲻFF_1 { get; }
    }
    
    public sealed class _SP
    {
        public _SP(Inners._percentx20 _percentx20_1)
        {
            this._percentx20_1 = _percentx20_1;
        }
        
        public Inners._percentx20 _percentx20_1 { get; }
    }
    
    public sealed class _VCHAR
    {
        public _VCHAR(Inners._percentx21ⲻ7E _percentx21ⲻ7E_1)
        {
            this._percentx21ⲻ7E_1 = _percentx21ⲻ7E_1;
        }
        
        public Inners._percentx21ⲻ7E _percentx21ⲻ7E_1 { get; }
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
        public sealed class _4
        {
            private _4()
            {
            }
            
            public static _4 Instance { get; } = new _4();
        }
        
        public sealed class _1
        {
            private _1()
            {
            }
            
            public static _1 Instance { get; } = new _1();
        }
        
        public sealed class _2
        {
            private _2()
            {
            }
            
            public static _2 Instance { get; } = new _2();
        }
        
        public sealed class _3
        {
            private _3()
            {
            }
            
            public static _3 Instance { get; } = new _3();
        }
        
        public sealed class _5
        {
            private _5()
            {
            }
            
            public static _5 Instance { get; } = new _5();
        }
        
        public sealed class _6
        {
            private _6()
            {
            }
            
            public static _6 Instance { get; } = new _6();
        }
        
        public sealed class _7
        {
            private _7()
            {
            }
            
            public static _7 Instance { get; } = new _7();
        }
        
        public sealed class _8
        {
            private _8()
            {
            }
            
            public static _8 Instance { get; } = new _8();
        }
        
        public sealed class _9
        {
            private _9()
            {
            }
            
            public static _9 Instance { get; } = new _9();
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
        
        public sealed class _0
        {
            private _0()
            {
            }
            
            public static _0 Instance { get; } = new _0();
        }
        
        public abstract class _percentx41ⲻ5A
        {
            private _percentx41ⲻ5A()
            {
            }
            
            protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
            
            public abstract class Visitor<TResult, TContext>
            {
                public TResult Visit(_percentx41ⲻ5A node, TContext context)
                {
                    return node.Dispatch(this, context);
                }
                
                protected internal abstract TResult Accept(_percentx41ⲻ5A._41 node, TContext context);
                protected internal abstract TResult Accept(_percentx41ⲻ5A._42 node, TContext context);
                protected internal abstract TResult Accept(_percentx41ⲻ5A._43 node, TContext context);
                protected internal abstract TResult Accept(_percentx41ⲻ5A._44 node, TContext context);
                protected internal abstract TResult Accept(_percentx41ⲻ5A._45 node, TContext context);
                protected internal abstract TResult Accept(_percentx41ⲻ5A._46 node, TContext context);
                protected internal abstract TResult Accept(_percentx41ⲻ5A._47 node, TContext context);
                protected internal abstract TResult Accept(_percentx41ⲻ5A._48 node, TContext context);
                protected internal abstract TResult Accept(_percentx41ⲻ5A._49 node, TContext context);
                protected internal abstract TResult Accept(_percentx41ⲻ5A._4A node, TContext context);
                protected internal abstract TResult Accept(_percentx41ⲻ5A._4B node, TContext context);
                protected internal abstract TResult Accept(_percentx41ⲻ5A._4C node, TContext context);
                protected internal abstract TResult Accept(_percentx41ⲻ5A._4D node, TContext context);
                protected internal abstract TResult Accept(_percentx41ⲻ5A._4E node, TContext context);
                protected internal abstract TResult Accept(_percentx41ⲻ5A._4F node, TContext context);
                protected internal abstract TResult Accept(_percentx41ⲻ5A._50 node, TContext context);
                protected internal abstract TResult Accept(_percentx41ⲻ5A._51 node, TContext context);
                protected internal abstract TResult Accept(_percentx41ⲻ5A._52 node, TContext context);
                protected internal abstract TResult Accept(_percentx41ⲻ5A._53 node, TContext context);
                protected internal abstract TResult Accept(_percentx41ⲻ5A._54 node, TContext context);
                protected internal abstract TResult Accept(_percentx41ⲻ5A._55 node, TContext context);
                protected internal abstract TResult Accept(_percentx41ⲻ5A._56 node, TContext context);
                protected internal abstract TResult Accept(_percentx41ⲻ5A._57 node, TContext context);
                protected internal abstract TResult Accept(_percentx41ⲻ5A._58 node, TContext context);
                protected internal abstract TResult Accept(_percentx41ⲻ5A._59 node, TContext context);
                protected internal abstract TResult Accept(_percentx41ⲻ5A._5A node, TContext context);
            }
            
            public sealed class _41 : _percentx41ⲻ5A
            {
                public _41(Inners._4 _4_1, Inners._1 _1_1)
                {
                    this._4_1 = _4_1;
                    this._1_1 = _1_1;
                }
                
                public Inners._4 _4_1 { get; }
                public Inners._1 _1_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _42 : _percentx41ⲻ5A
            {
                public _42(Inners._4 _4_1, Inners._2 _2_1)
                {
                    this._4_1 = _4_1;
                    this._2_1 = _2_1;
                }
                
                public Inners._4 _4_1 { get; }
                public Inners._2 _2_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _43 : _percentx41ⲻ5A
            {
                public _43(Inners._4 _4_1, Inners._3 _3_1)
                {
                    this._4_1 = _4_1;
                    this._3_1 = _3_1;
                }
                
                public Inners._4 _4_1 { get; }
                public Inners._3 _3_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _44 : _percentx41ⲻ5A
            {
                public _44(Inners._4 _4_1, Inners._4 _4_2)
                {
                    this._4_1 = _4_1;
                    this._4_2 = _4_2;
                }
                
                public Inners._4 _4_1 { get; }
                public Inners._4 _4_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _45 : _percentx41ⲻ5A
            {
                public _45(Inners._4 _4_1, Inners._5 _5_1)
                {
                    this._4_1 = _4_1;
                    this._5_1 = _5_1;
                }
                
                public Inners._4 _4_1 { get; }
                public Inners._5 _5_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _46 : _percentx41ⲻ5A
            {
                public _46(Inners._4 _4_1, Inners._6 _6_1)
                {
                    this._4_1 = _4_1;
                    this._6_1 = _6_1;
                }
                
                public Inners._4 _4_1 { get; }
                public Inners._6 _6_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _47 : _percentx41ⲻ5A
            {
                public _47(Inners._4 _4_1, Inners._7 _7_1)
                {
                    this._4_1 = _4_1;
                    this._7_1 = _7_1;
                }
                
                public Inners._4 _4_1 { get; }
                public Inners._7 _7_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _48 : _percentx41ⲻ5A
            {
                public _48(Inners._4 _4_1, Inners._8 _8_1)
                {
                    this._4_1 = _4_1;
                    this._8_1 = _8_1;
                }
                
                public Inners._4 _4_1 { get; }
                public Inners._8 _8_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _49 : _percentx41ⲻ5A
            {
                public _49(Inners._4 _4_1, Inners._9 _9_1)
                {
                    this._4_1 = _4_1;
                    this._9_1 = _9_1;
                }
                
                public Inners._4 _4_1 { get; }
                public Inners._9 _9_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _4A : _percentx41ⲻ5A
            {
                public _4A(Inners._4 _4_1, Inners._A _A_1)
                {
                    this._4_1 = _4_1;
                    this._A_1 = _A_1;
                }
                
                public Inners._4 _4_1 { get; }
                public Inners._A _A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _4B : _percentx41ⲻ5A
            {
                public _4B(Inners._4 _4_1, Inners._B _B_1)
                {
                    this._4_1 = _4_1;
                    this._B_1 = _B_1;
                }
                
                public Inners._4 _4_1 { get; }
                public Inners._B _B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _4C : _percentx41ⲻ5A
            {
                public _4C(Inners._4 _4_1, Inners._C _C_1)
                {
                    this._4_1 = _4_1;
                    this._C_1 = _C_1;
                }
                
                public Inners._4 _4_1 { get; }
                public Inners._C _C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _4D : _percentx41ⲻ5A
            {
                public _4D(Inners._4 _4_1, Inners._D _D_1)
                {
                    this._4_1 = _4_1;
                    this._D_1 = _D_1;
                }
                
                public Inners._4 _4_1 { get; }
                public Inners._D _D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _4E : _percentx41ⲻ5A
            {
                public _4E(Inners._4 _4_1, Inners._E _E_1)
                {
                    this._4_1 = _4_1;
                    this._E_1 = _E_1;
                }
                
                public Inners._4 _4_1 { get; }
                public Inners._E _E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _4F : _percentx41ⲻ5A
            {
                public _4F(Inners._4 _4_1, Inners._F _F_1)
                {
                    this._4_1 = _4_1;
                    this._F_1 = _F_1;
                }
                
                public Inners._4 _4_1 { get; }
                public Inners._F _F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _50 : _percentx41ⲻ5A
            {
                public _50(Inners._5 _5_1, Inners._0 _0_1)
                {
                    this._5_1 = _5_1;
                    this._0_1 = _0_1;
                }
                
                public Inners._5 _5_1 { get; }
                public Inners._0 _0_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _51 : _percentx41ⲻ5A
            {
                public _51(Inners._5 _5_1, Inners._1 _1_1)
                {
                    this._5_1 = _5_1;
                    this._1_1 = _1_1;
                }
                
                public Inners._5 _5_1 { get; }
                public Inners._1 _1_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _52 : _percentx41ⲻ5A
            {
                public _52(Inners._5 _5_1, Inners._2 _2_1)
                {
                    this._5_1 = _5_1;
                    this._2_1 = _2_1;
                }
                
                public Inners._5 _5_1 { get; }
                public Inners._2 _2_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _53 : _percentx41ⲻ5A
            {
                public _53(Inners._5 _5_1, Inners._3 _3_1)
                {
                    this._5_1 = _5_1;
                    this._3_1 = _3_1;
                }
                
                public Inners._5 _5_1 { get; }
                public Inners._3 _3_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _54 : _percentx41ⲻ5A
            {
                public _54(Inners._5 _5_1, Inners._4 _4_1)
                {
                    this._5_1 = _5_1;
                    this._4_1 = _4_1;
                }
                
                public Inners._5 _5_1 { get; }
                public Inners._4 _4_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _55 : _percentx41ⲻ5A
            {
                public _55(Inners._5 _5_1, Inners._5 _5_2)
                {
                    this._5_1 = _5_1;
                    this._5_2 = _5_2;
                }
                
                public Inners._5 _5_1 { get; }
                public Inners._5 _5_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _56 : _percentx41ⲻ5A
            {
                public _56(Inners._5 _5_1, Inners._6 _6_1)
                {
                    this._5_1 = _5_1;
                    this._6_1 = _6_1;
                }
                
                public Inners._5 _5_1 { get; }
                public Inners._6 _6_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _57 : _percentx41ⲻ5A
            {
                public _57(Inners._5 _5_1, Inners._7 _7_1)
                {
                    this._5_1 = _5_1;
                    this._7_1 = _7_1;
                }
                
                public Inners._5 _5_1 { get; }
                public Inners._7 _7_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _58 : _percentx41ⲻ5A
            {
                public _58(Inners._5 _5_1, Inners._8 _8_1)
                {
                    this._5_1 = _5_1;
                    this._8_1 = _8_1;
                }
                
                public Inners._5 _5_1 { get; }
                public Inners._8 _8_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _59 : _percentx41ⲻ5A
            {
                public _59(Inners._5 _5_1, Inners._9 _9_1)
                {
                    this._5_1 = _5_1;
                    this._9_1 = _9_1;
                }
                
                public Inners._5 _5_1 { get; }
                public Inners._9 _9_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _5A : _percentx41ⲻ5A
            {
                public _5A(Inners._5 _5_1, Inners._A _A_1)
                {
                    this._5_1 = _5_1;
                    this._A_1 = _A_1;
                }
                
                public Inners._5 _5_1 { get; }
                public Inners._A _A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
        }
        
        public abstract class _percentx61ⲻ7A
        {
            private _percentx61ⲻ7A()
            {
            }
            
            protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
            
            public abstract class Visitor<TResult, TContext>
            {
                public TResult Visit(_percentx61ⲻ7A node, TContext context)
                {
                    return node.Dispatch(this, context);
                }
                
                protected internal abstract TResult Accept(_percentx61ⲻ7A._61 node, TContext context);
                protected internal abstract TResult Accept(_percentx61ⲻ7A._62 node, TContext context);
                protected internal abstract TResult Accept(_percentx61ⲻ7A._63 node, TContext context);
                protected internal abstract TResult Accept(_percentx61ⲻ7A._64 node, TContext context);
                protected internal abstract TResult Accept(_percentx61ⲻ7A._65 node, TContext context);
                protected internal abstract TResult Accept(_percentx61ⲻ7A._66 node, TContext context);
                protected internal abstract TResult Accept(_percentx61ⲻ7A._67 node, TContext context);
                protected internal abstract TResult Accept(_percentx61ⲻ7A._68 node, TContext context);
                protected internal abstract TResult Accept(_percentx61ⲻ7A._69 node, TContext context);
                protected internal abstract TResult Accept(_percentx61ⲻ7A._6A node, TContext context);
                protected internal abstract TResult Accept(_percentx61ⲻ7A._6B node, TContext context);
                protected internal abstract TResult Accept(_percentx61ⲻ7A._6C node, TContext context);
                protected internal abstract TResult Accept(_percentx61ⲻ7A._6D node, TContext context);
                protected internal abstract TResult Accept(_percentx61ⲻ7A._6E node, TContext context);
                protected internal abstract TResult Accept(_percentx61ⲻ7A._6F node, TContext context);
                protected internal abstract TResult Accept(_percentx61ⲻ7A._70 node, TContext context);
                protected internal abstract TResult Accept(_percentx61ⲻ7A._71 node, TContext context);
                protected internal abstract TResult Accept(_percentx61ⲻ7A._72 node, TContext context);
                protected internal abstract TResult Accept(_percentx61ⲻ7A._73 node, TContext context);
                protected internal abstract TResult Accept(_percentx61ⲻ7A._74 node, TContext context);
                protected internal abstract TResult Accept(_percentx61ⲻ7A._75 node, TContext context);
                protected internal abstract TResult Accept(_percentx61ⲻ7A._76 node, TContext context);
                protected internal abstract TResult Accept(_percentx61ⲻ7A._77 node, TContext context);
                protected internal abstract TResult Accept(_percentx61ⲻ7A._78 node, TContext context);
                protected internal abstract TResult Accept(_percentx61ⲻ7A._79 node, TContext context);
                protected internal abstract TResult Accept(_percentx61ⲻ7A._7A node, TContext context);
            }
            
            public sealed class _61 : _percentx61ⲻ7A
            {
                public _61(Inners._6 _6_1, Inners._1 _1_1)
                {
                    this._6_1 = _6_1;
                    this._1_1 = _1_1;
                }
                
                public Inners._6 _6_1 { get; }
                public Inners._1 _1_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _62 : _percentx61ⲻ7A
            {
                public _62(Inners._6 _6_1, Inners._2 _2_1)
                {
                    this._6_1 = _6_1;
                    this._2_1 = _2_1;
                }
                
                public Inners._6 _6_1 { get; }
                public Inners._2 _2_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _63 : _percentx61ⲻ7A
            {
                public _63(Inners._6 _6_1, Inners._3 _3_1)
                {
                    this._6_1 = _6_1;
                    this._3_1 = _3_1;
                }
                
                public Inners._6 _6_1 { get; }
                public Inners._3 _3_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _64 : _percentx61ⲻ7A
            {
                public _64(Inners._6 _6_1, Inners._4 _4_1)
                {
                    this._6_1 = _6_1;
                    this._4_1 = _4_1;
                }
                
                public Inners._6 _6_1 { get; }
                public Inners._4 _4_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _65 : _percentx61ⲻ7A
            {
                public _65(Inners._6 _6_1, Inners._5 _5_1)
                {
                    this._6_1 = _6_1;
                    this._5_1 = _5_1;
                }
                
                public Inners._6 _6_1 { get; }
                public Inners._5 _5_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _66 : _percentx61ⲻ7A
            {
                public _66(Inners._6 _6_1, Inners._6 _6_2)
                {
                    this._6_1 = _6_1;
                    this._6_2 = _6_2;
                }
                
                public Inners._6 _6_1 { get; }
                public Inners._6 _6_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _67 : _percentx61ⲻ7A
            {
                public _67(Inners._6 _6_1, Inners._7 _7_1)
                {
                    this._6_1 = _6_1;
                    this._7_1 = _7_1;
                }
                
                public Inners._6 _6_1 { get; }
                public Inners._7 _7_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _68 : _percentx61ⲻ7A
            {
                public _68(Inners._6 _6_1, Inners._8 _8_1)
                {
                    this._6_1 = _6_1;
                    this._8_1 = _8_1;
                }
                
                public Inners._6 _6_1 { get; }
                public Inners._8 _8_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _69 : _percentx61ⲻ7A
            {
                public _69(Inners._6 _6_1, Inners._9 _9_1)
                {
                    this._6_1 = _6_1;
                    this._9_1 = _9_1;
                }
                
                public Inners._6 _6_1 { get; }
                public Inners._9 _9_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _6A : _percentx61ⲻ7A
            {
                public _6A(Inners._6 _6_1, Inners._A _A_1)
                {
                    this._6_1 = _6_1;
                    this._A_1 = _A_1;
                }
                
                public Inners._6 _6_1 { get; }
                public Inners._A _A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _6B : _percentx61ⲻ7A
            {
                public _6B(Inners._6 _6_1, Inners._B _B_1)
                {
                    this._6_1 = _6_1;
                    this._B_1 = _B_1;
                }
                
                public Inners._6 _6_1 { get; }
                public Inners._B _B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _6C : _percentx61ⲻ7A
            {
                public _6C(Inners._6 _6_1, Inners._C _C_1)
                {
                    this._6_1 = _6_1;
                    this._C_1 = _C_1;
                }
                
                public Inners._6 _6_1 { get; }
                public Inners._C _C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _6D : _percentx61ⲻ7A
            {
                public _6D(Inners._6 _6_1, Inners._D _D_1)
                {
                    this._6_1 = _6_1;
                    this._D_1 = _D_1;
                }
                
                public Inners._6 _6_1 { get; }
                public Inners._D _D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _6E : _percentx61ⲻ7A
            {
                public _6E(Inners._6 _6_1, Inners._E _E_1)
                {
                    this._6_1 = _6_1;
                    this._E_1 = _E_1;
                }
                
                public Inners._6 _6_1 { get; }
                public Inners._E _E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _6F : _percentx61ⲻ7A
            {
                public _6F(Inners._6 _6_1, Inners._F _F_1)
                {
                    this._6_1 = _6_1;
                    this._F_1 = _F_1;
                }
                
                public Inners._6 _6_1 { get; }
                public Inners._F _F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _70 : _percentx61ⲻ7A
            {
                public _70(Inners._7 _7_1, Inners._0 _0_1)
                {
                    this._7_1 = _7_1;
                    this._0_1 = _0_1;
                }
                
                public Inners._7 _7_1 { get; }
                public Inners._0 _0_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _71 : _percentx61ⲻ7A
            {
                public _71(Inners._7 _7_1, Inners._1 _1_1)
                {
                    this._7_1 = _7_1;
                    this._1_1 = _1_1;
                }
                
                public Inners._7 _7_1 { get; }
                public Inners._1 _1_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _72 : _percentx61ⲻ7A
            {
                public _72(Inners._7 _7_1, Inners._2 _2_1)
                {
                    this._7_1 = _7_1;
                    this._2_1 = _2_1;
                }
                
                public Inners._7 _7_1 { get; }
                public Inners._2 _2_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _73 : _percentx61ⲻ7A
            {
                public _73(Inners._7 _7_1, Inners._3 _3_1)
                {
                    this._7_1 = _7_1;
                    this._3_1 = _3_1;
                }
                
                public Inners._7 _7_1 { get; }
                public Inners._3 _3_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _74 : _percentx61ⲻ7A
            {
                public _74(Inners._7 _7_1, Inners._4 _4_1)
                {
                    this._7_1 = _7_1;
                    this._4_1 = _4_1;
                }
                
                public Inners._7 _7_1 { get; }
                public Inners._4 _4_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _75 : _percentx61ⲻ7A
            {
                public _75(Inners._7 _7_1, Inners._5 _5_1)
                {
                    this._7_1 = _7_1;
                    this._5_1 = _5_1;
                }
                
                public Inners._7 _7_1 { get; }
                public Inners._5 _5_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _76 : _percentx61ⲻ7A
            {
                public _76(Inners._7 _7_1, Inners._6 _6_1)
                {
                    this._7_1 = _7_1;
                    this._6_1 = _6_1;
                }
                
                public Inners._7 _7_1 { get; }
                public Inners._6 _6_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _77 : _percentx61ⲻ7A
            {
                public _77(Inners._7 _7_1, Inners._7 _7_2)
                {
                    this._7_1 = _7_1;
                    this._7_2 = _7_2;
                }
                
                public Inners._7 _7_1 { get; }
                public Inners._7 _7_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _78 : _percentx61ⲻ7A
            {
                public _78(Inners._7 _7_1, Inners._8 _8_1)
                {
                    this._7_1 = _7_1;
                    this._8_1 = _8_1;
                }
                
                public Inners._7 _7_1 { get; }
                public Inners._8 _8_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _79 : _percentx61ⲻ7A
            {
                public _79(Inners._7 _7_1, Inners._9 _9_1)
                {
                    this._7_1 = _7_1;
                    this._9_1 = _9_1;
                }
                
                public Inners._7 _7_1 { get; }
                public Inners._9 _9_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _7A : _percentx61ⲻ7A
            {
                public _7A(Inners._7 _7_1, Inners._A _A_1)
                {
                    this._7_1 = _7_1;
                    this._A_1 = _A_1;
                }
                
                public Inners._7 _7_1 { get; }
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
        
        public abstract class _percentx01ⲻ7F
        {
            private _percentx01ⲻ7F()
            {
            }
            
            protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
            
            public abstract class Visitor<TResult, TContext>
            {
                public TResult Visit(_percentx01ⲻ7F node, TContext context)
                {
                    return node.Dispatch(this, context);
                }
                
                protected internal abstract TResult Accept(_percentx01ⲻ7F._01 node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._02 node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._03 node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._04 node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._05 node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._06 node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._07 node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._08 node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._09 node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._0A node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._0B node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._0C node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._0D node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._0E node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._0F node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._10 node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._11 node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._12 node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._13 node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._14 node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._15 node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._16 node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._17 node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._18 node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._19 node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._1A node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._1B node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._1C node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._1D node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._1E node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._1F node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._20 node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._21 node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._22 node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._23 node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._24 node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._25 node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._26 node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._27 node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._28 node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._29 node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._2A node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._2B node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._2C node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._2D node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._2E node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._2F node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._30 node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._31 node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._32 node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._33 node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._34 node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._35 node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._36 node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._37 node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._38 node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._39 node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._3A node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._3B node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._3C node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._3D node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._3E node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._3F node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._40 node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._41 node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._42 node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._43 node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._44 node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._45 node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._46 node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._47 node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._48 node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._49 node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._4A node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._4B node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._4C node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._4D node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._4E node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._4F node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._50 node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._51 node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._52 node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._53 node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._54 node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._55 node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._56 node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._57 node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._58 node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._59 node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._5A node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._5B node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._5C node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._5D node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._5E node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._5F node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._60 node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._61 node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._62 node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._63 node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._64 node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._65 node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._66 node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._67 node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._68 node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._69 node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._6A node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._6B node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._6C node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._6D node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._6E node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._6F node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._70 node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._71 node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._72 node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._73 node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._74 node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._75 node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._76 node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._77 node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._78 node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._79 node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._7A node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._7B node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._7C node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._7D node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._7E node, TContext context);
                protected internal abstract TResult Accept(_percentx01ⲻ7F._7F node, TContext context);
            }
            
            public sealed class _01 : _percentx01ⲻ7F
            {
                public _01(Inners._0 _0_1, Inners._1 _1_1)
                {
                    this._0_1 = _0_1;
                    this._1_1 = _1_1;
                }
                
                public Inners._0 _0_1 { get; }
                public Inners._1 _1_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _02 : _percentx01ⲻ7F
            {
                public _02(Inners._0 _0_1, Inners._2 _2_1)
                {
                    this._0_1 = _0_1;
                    this._2_1 = _2_1;
                }
                
                public Inners._0 _0_1 { get; }
                public Inners._2 _2_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _03 : _percentx01ⲻ7F
            {
                public _03(Inners._0 _0_1, Inners._3 _3_1)
                {
                    this._0_1 = _0_1;
                    this._3_1 = _3_1;
                }
                
                public Inners._0 _0_1 { get; }
                public Inners._3 _3_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _04 : _percentx01ⲻ7F
            {
                public _04(Inners._0 _0_1, Inners._4 _4_1)
                {
                    this._0_1 = _0_1;
                    this._4_1 = _4_1;
                }
                
                public Inners._0 _0_1 { get; }
                public Inners._4 _4_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _05 : _percentx01ⲻ7F
            {
                public _05(Inners._0 _0_1, Inners._5 _5_1)
                {
                    this._0_1 = _0_1;
                    this._5_1 = _5_1;
                }
                
                public Inners._0 _0_1 { get; }
                public Inners._5 _5_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _06 : _percentx01ⲻ7F
            {
                public _06(Inners._0 _0_1, Inners._6 _6_1)
                {
                    this._0_1 = _0_1;
                    this._6_1 = _6_1;
                }
                
                public Inners._0 _0_1 { get; }
                public Inners._6 _6_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _07 : _percentx01ⲻ7F
            {
                public _07(Inners._0 _0_1, Inners._7 _7_1)
                {
                    this._0_1 = _0_1;
                    this._7_1 = _7_1;
                }
                
                public Inners._0 _0_1 { get; }
                public Inners._7 _7_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _08 : _percentx01ⲻ7F
            {
                public _08(Inners._0 _0_1, Inners._8 _8_1)
                {
                    this._0_1 = _0_1;
                    this._8_1 = _8_1;
                }
                
                public Inners._0 _0_1 { get; }
                public Inners._8 _8_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _09 : _percentx01ⲻ7F
            {
                public _09(Inners._0 _0_1, Inners._9 _9_1)
                {
                    this._0_1 = _0_1;
                    this._9_1 = _9_1;
                }
                
                public Inners._0 _0_1 { get; }
                public Inners._9 _9_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _0A : _percentx01ⲻ7F
            {
                public _0A(Inners._0 _0_1, Inners._A _A_1)
                {
                    this._0_1 = _0_1;
                    this._A_1 = _A_1;
                }
                
                public Inners._0 _0_1 { get; }
                public Inners._A _A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _0B : _percentx01ⲻ7F
            {
                public _0B(Inners._0 _0_1, Inners._B _B_1)
                {
                    this._0_1 = _0_1;
                    this._B_1 = _B_1;
                }
                
                public Inners._0 _0_1 { get; }
                public Inners._B _B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _0C : _percentx01ⲻ7F
            {
                public _0C(Inners._0 _0_1, Inners._C _C_1)
                {
                    this._0_1 = _0_1;
                    this._C_1 = _C_1;
                }
                
                public Inners._0 _0_1 { get; }
                public Inners._C _C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _0D : _percentx01ⲻ7F
            {
                public _0D(Inners._0 _0_1, Inners._D _D_1)
                {
                    this._0_1 = _0_1;
                    this._D_1 = _D_1;
                }
                
                public Inners._0 _0_1 { get; }
                public Inners._D _D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _0E : _percentx01ⲻ7F
            {
                public _0E(Inners._0 _0_1, Inners._E _E_1)
                {
                    this._0_1 = _0_1;
                    this._E_1 = _E_1;
                }
                
                public Inners._0 _0_1 { get; }
                public Inners._E _E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _0F : _percentx01ⲻ7F
            {
                public _0F(Inners._0 _0_1, Inners._F _F_1)
                {
                    this._0_1 = _0_1;
                    this._F_1 = _F_1;
                }
                
                public Inners._0 _0_1 { get; }
                public Inners._F _F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _10 : _percentx01ⲻ7F
            {
                public _10(Inners._1 _1_1, Inners._0 _0_1)
                {
                    this._1_1 = _1_1;
                    this._0_1 = _0_1;
                }
                
                public Inners._1 _1_1 { get; }
                public Inners._0 _0_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _11 : _percentx01ⲻ7F
            {
                public _11(Inners._1 _1_1, Inners._1 _1_2)
                {
                    this._1_1 = _1_1;
                    this._1_2 = _1_2;
                }
                
                public Inners._1 _1_1 { get; }
                public Inners._1 _1_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _12 : _percentx01ⲻ7F
            {
                public _12(Inners._1 _1_1, Inners._2 _2_1)
                {
                    this._1_1 = _1_1;
                    this._2_1 = _2_1;
                }
                
                public Inners._1 _1_1 { get; }
                public Inners._2 _2_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _13 : _percentx01ⲻ7F
            {
                public _13(Inners._1 _1_1, Inners._3 _3_1)
                {
                    this._1_1 = _1_1;
                    this._3_1 = _3_1;
                }
                
                public Inners._1 _1_1 { get; }
                public Inners._3 _3_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _14 : _percentx01ⲻ7F
            {
                public _14(Inners._1 _1_1, Inners._4 _4_1)
                {
                    this._1_1 = _1_1;
                    this._4_1 = _4_1;
                }
                
                public Inners._1 _1_1 { get; }
                public Inners._4 _4_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _15 : _percentx01ⲻ7F
            {
                public _15(Inners._1 _1_1, Inners._5 _5_1)
                {
                    this._1_1 = _1_1;
                    this._5_1 = _5_1;
                }
                
                public Inners._1 _1_1 { get; }
                public Inners._5 _5_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _16 : _percentx01ⲻ7F
            {
                public _16(Inners._1 _1_1, Inners._6 _6_1)
                {
                    this._1_1 = _1_1;
                    this._6_1 = _6_1;
                }
                
                public Inners._1 _1_1 { get; }
                public Inners._6 _6_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _17 : _percentx01ⲻ7F
            {
                public _17(Inners._1 _1_1, Inners._7 _7_1)
                {
                    this._1_1 = _1_1;
                    this._7_1 = _7_1;
                }
                
                public Inners._1 _1_1 { get; }
                public Inners._7 _7_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _18 : _percentx01ⲻ7F
            {
                public _18(Inners._1 _1_1, Inners._8 _8_1)
                {
                    this._1_1 = _1_1;
                    this._8_1 = _8_1;
                }
                
                public Inners._1 _1_1 { get; }
                public Inners._8 _8_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _19 : _percentx01ⲻ7F
            {
                public _19(Inners._1 _1_1, Inners._9 _9_1)
                {
                    this._1_1 = _1_1;
                    this._9_1 = _9_1;
                }
                
                public Inners._1 _1_1 { get; }
                public Inners._9 _9_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _1A : _percentx01ⲻ7F
            {
                public _1A(Inners._1 _1_1, Inners._A _A_1)
                {
                    this._1_1 = _1_1;
                    this._A_1 = _A_1;
                }
                
                public Inners._1 _1_1 { get; }
                public Inners._A _A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _1B : _percentx01ⲻ7F
            {
                public _1B(Inners._1 _1_1, Inners._B _B_1)
                {
                    this._1_1 = _1_1;
                    this._B_1 = _B_1;
                }
                
                public Inners._1 _1_1 { get; }
                public Inners._B _B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _1C : _percentx01ⲻ7F
            {
                public _1C(Inners._1 _1_1, Inners._C _C_1)
                {
                    this._1_1 = _1_1;
                    this._C_1 = _C_1;
                }
                
                public Inners._1 _1_1 { get; }
                public Inners._C _C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _1D : _percentx01ⲻ7F
            {
                public _1D(Inners._1 _1_1, Inners._D _D_1)
                {
                    this._1_1 = _1_1;
                    this._D_1 = _D_1;
                }
                
                public Inners._1 _1_1 { get; }
                public Inners._D _D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _1E : _percentx01ⲻ7F
            {
                public _1E(Inners._1 _1_1, Inners._E _E_1)
                {
                    this._1_1 = _1_1;
                    this._E_1 = _E_1;
                }
                
                public Inners._1 _1_1 { get; }
                public Inners._E _E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _1F : _percentx01ⲻ7F
            {
                public _1F(Inners._1 _1_1, Inners._F _F_1)
                {
                    this._1_1 = _1_1;
                    this._F_1 = _F_1;
                }
                
                public Inners._1 _1_1 { get; }
                public Inners._F _F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _20 : _percentx01ⲻ7F
            {
                public _20(Inners._2 _2_1, Inners._0 _0_1)
                {
                    this._2_1 = _2_1;
                    this._0_1 = _0_1;
                }
                
                public Inners._2 _2_1 { get; }
                public Inners._0 _0_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _21 : _percentx01ⲻ7F
            {
                public _21(Inners._2 _2_1, Inners._1 _1_1)
                {
                    this._2_1 = _2_1;
                    this._1_1 = _1_1;
                }
                
                public Inners._2 _2_1 { get; }
                public Inners._1 _1_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _22 : _percentx01ⲻ7F
            {
                public _22(Inners._2 _2_1, Inners._2 _2_2)
                {
                    this._2_1 = _2_1;
                    this._2_2 = _2_2;
                }
                
                public Inners._2 _2_1 { get; }
                public Inners._2 _2_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _23 : _percentx01ⲻ7F
            {
                public _23(Inners._2 _2_1, Inners._3 _3_1)
                {
                    this._2_1 = _2_1;
                    this._3_1 = _3_1;
                }
                
                public Inners._2 _2_1 { get; }
                public Inners._3 _3_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _24 : _percentx01ⲻ7F
            {
                public _24(Inners._2 _2_1, Inners._4 _4_1)
                {
                    this._2_1 = _2_1;
                    this._4_1 = _4_1;
                }
                
                public Inners._2 _2_1 { get; }
                public Inners._4 _4_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _25 : _percentx01ⲻ7F
            {
                public _25(Inners._2 _2_1, Inners._5 _5_1)
                {
                    this._2_1 = _2_1;
                    this._5_1 = _5_1;
                }
                
                public Inners._2 _2_1 { get; }
                public Inners._5 _5_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _26 : _percentx01ⲻ7F
            {
                public _26(Inners._2 _2_1, Inners._6 _6_1)
                {
                    this._2_1 = _2_1;
                    this._6_1 = _6_1;
                }
                
                public Inners._2 _2_1 { get; }
                public Inners._6 _6_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _27 : _percentx01ⲻ7F
            {
                public _27(Inners._2 _2_1, Inners._7 _7_1)
                {
                    this._2_1 = _2_1;
                    this._7_1 = _7_1;
                }
                
                public Inners._2 _2_1 { get; }
                public Inners._7 _7_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _28 : _percentx01ⲻ7F
            {
                public _28(Inners._2 _2_1, Inners._8 _8_1)
                {
                    this._2_1 = _2_1;
                    this._8_1 = _8_1;
                }
                
                public Inners._2 _2_1 { get; }
                public Inners._8 _8_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _29 : _percentx01ⲻ7F
            {
                public _29(Inners._2 _2_1, Inners._9 _9_1)
                {
                    this._2_1 = _2_1;
                    this._9_1 = _9_1;
                }
                
                public Inners._2 _2_1 { get; }
                public Inners._9 _9_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _2A : _percentx01ⲻ7F
            {
                public _2A(Inners._2 _2_1, Inners._A _A_1)
                {
                    this._2_1 = _2_1;
                    this._A_1 = _A_1;
                }
                
                public Inners._2 _2_1 { get; }
                public Inners._A _A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _2B : _percentx01ⲻ7F
            {
                public _2B(Inners._2 _2_1, Inners._B _B_1)
                {
                    this._2_1 = _2_1;
                    this._B_1 = _B_1;
                }
                
                public Inners._2 _2_1 { get; }
                public Inners._B _B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _2C : _percentx01ⲻ7F
            {
                public _2C(Inners._2 _2_1, Inners._C _C_1)
                {
                    this._2_1 = _2_1;
                    this._C_1 = _C_1;
                }
                
                public Inners._2 _2_1 { get; }
                public Inners._C _C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _2D : _percentx01ⲻ7F
            {
                public _2D(Inners._2 _2_1, Inners._D _D_1)
                {
                    this._2_1 = _2_1;
                    this._D_1 = _D_1;
                }
                
                public Inners._2 _2_1 { get; }
                public Inners._D _D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _2E : _percentx01ⲻ7F
            {
                public _2E(Inners._2 _2_1, Inners._E _E_1)
                {
                    this._2_1 = _2_1;
                    this._E_1 = _E_1;
                }
                
                public Inners._2 _2_1 { get; }
                public Inners._E _E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _2F : _percentx01ⲻ7F
            {
                public _2F(Inners._2 _2_1, Inners._F _F_1)
                {
                    this._2_1 = _2_1;
                    this._F_1 = _F_1;
                }
                
                public Inners._2 _2_1 { get; }
                public Inners._F _F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _30 : _percentx01ⲻ7F
            {
                public _30(Inners._3 _3_1, Inners._0 _0_1)
                {
                    this._3_1 = _3_1;
                    this._0_1 = _0_1;
                }
                
                public Inners._3 _3_1 { get; }
                public Inners._0 _0_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _31 : _percentx01ⲻ7F
            {
                public _31(Inners._3 _3_1, Inners._1 _1_1)
                {
                    this._3_1 = _3_1;
                    this._1_1 = _1_1;
                }
                
                public Inners._3 _3_1 { get; }
                public Inners._1 _1_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _32 : _percentx01ⲻ7F
            {
                public _32(Inners._3 _3_1, Inners._2 _2_1)
                {
                    this._3_1 = _3_1;
                    this._2_1 = _2_1;
                }
                
                public Inners._3 _3_1 { get; }
                public Inners._2 _2_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _33 : _percentx01ⲻ7F
            {
                public _33(Inners._3 _3_1, Inners._3 _3_2)
                {
                    this._3_1 = _3_1;
                    this._3_2 = _3_2;
                }
                
                public Inners._3 _3_1 { get; }
                public Inners._3 _3_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _34 : _percentx01ⲻ7F
            {
                public _34(Inners._3 _3_1, Inners._4 _4_1)
                {
                    this._3_1 = _3_1;
                    this._4_1 = _4_1;
                }
                
                public Inners._3 _3_1 { get; }
                public Inners._4 _4_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _35 : _percentx01ⲻ7F
            {
                public _35(Inners._3 _3_1, Inners._5 _5_1)
                {
                    this._3_1 = _3_1;
                    this._5_1 = _5_1;
                }
                
                public Inners._3 _3_1 { get; }
                public Inners._5 _5_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _36 : _percentx01ⲻ7F
            {
                public _36(Inners._3 _3_1, Inners._6 _6_1)
                {
                    this._3_1 = _3_1;
                    this._6_1 = _6_1;
                }
                
                public Inners._3 _3_1 { get; }
                public Inners._6 _6_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _37 : _percentx01ⲻ7F
            {
                public _37(Inners._3 _3_1, Inners._7 _7_1)
                {
                    this._3_1 = _3_1;
                    this._7_1 = _7_1;
                }
                
                public Inners._3 _3_1 { get; }
                public Inners._7 _7_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _38 : _percentx01ⲻ7F
            {
                public _38(Inners._3 _3_1, Inners._8 _8_1)
                {
                    this._3_1 = _3_1;
                    this._8_1 = _8_1;
                }
                
                public Inners._3 _3_1 { get; }
                public Inners._8 _8_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _39 : _percentx01ⲻ7F
            {
                public _39(Inners._3 _3_1, Inners._9 _9_1)
                {
                    this._3_1 = _3_1;
                    this._9_1 = _9_1;
                }
                
                public Inners._3 _3_1 { get; }
                public Inners._9 _9_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _3A : _percentx01ⲻ7F
            {
                public _3A(Inners._3 _3_1, Inners._A _A_1)
                {
                    this._3_1 = _3_1;
                    this._A_1 = _A_1;
                }
                
                public Inners._3 _3_1 { get; }
                public Inners._A _A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _3B : _percentx01ⲻ7F
            {
                public _3B(Inners._3 _3_1, Inners._B _B_1)
                {
                    this._3_1 = _3_1;
                    this._B_1 = _B_1;
                }
                
                public Inners._3 _3_1 { get; }
                public Inners._B _B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _3C : _percentx01ⲻ7F
            {
                public _3C(Inners._3 _3_1, Inners._C _C_1)
                {
                    this._3_1 = _3_1;
                    this._C_1 = _C_1;
                }
                
                public Inners._3 _3_1 { get; }
                public Inners._C _C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _3D : _percentx01ⲻ7F
            {
                public _3D(Inners._3 _3_1, Inners._D _D_1)
                {
                    this._3_1 = _3_1;
                    this._D_1 = _D_1;
                }
                
                public Inners._3 _3_1 { get; }
                public Inners._D _D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _3E : _percentx01ⲻ7F
            {
                public _3E(Inners._3 _3_1, Inners._E _E_1)
                {
                    this._3_1 = _3_1;
                    this._E_1 = _E_1;
                }
                
                public Inners._3 _3_1 { get; }
                public Inners._E _E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _3F : _percentx01ⲻ7F
            {
                public _3F(Inners._3 _3_1, Inners._F _F_1)
                {
                    this._3_1 = _3_1;
                    this._F_1 = _F_1;
                }
                
                public Inners._3 _3_1 { get; }
                public Inners._F _F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _40 : _percentx01ⲻ7F
            {
                public _40(Inners._4 _4_1, Inners._0 _0_1)
                {
                    this._4_1 = _4_1;
                    this._0_1 = _0_1;
                }
                
                public Inners._4 _4_1 { get; }
                public Inners._0 _0_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _41 : _percentx01ⲻ7F
            {
                public _41(Inners._4 _4_1, Inners._1 _1_1)
                {
                    this._4_1 = _4_1;
                    this._1_1 = _1_1;
                }
                
                public Inners._4 _4_1 { get; }
                public Inners._1 _1_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _42 : _percentx01ⲻ7F
            {
                public _42(Inners._4 _4_1, Inners._2 _2_1)
                {
                    this._4_1 = _4_1;
                    this._2_1 = _2_1;
                }
                
                public Inners._4 _4_1 { get; }
                public Inners._2 _2_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _43 : _percentx01ⲻ7F
            {
                public _43(Inners._4 _4_1, Inners._3 _3_1)
                {
                    this._4_1 = _4_1;
                    this._3_1 = _3_1;
                }
                
                public Inners._4 _4_1 { get; }
                public Inners._3 _3_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _44 : _percentx01ⲻ7F
            {
                public _44(Inners._4 _4_1, Inners._4 _4_2)
                {
                    this._4_1 = _4_1;
                    this._4_2 = _4_2;
                }
                
                public Inners._4 _4_1 { get; }
                public Inners._4 _4_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _45 : _percentx01ⲻ7F
            {
                public _45(Inners._4 _4_1, Inners._5 _5_1)
                {
                    this._4_1 = _4_1;
                    this._5_1 = _5_1;
                }
                
                public Inners._4 _4_1 { get; }
                public Inners._5 _5_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _46 : _percentx01ⲻ7F
            {
                public _46(Inners._4 _4_1, Inners._6 _6_1)
                {
                    this._4_1 = _4_1;
                    this._6_1 = _6_1;
                }
                
                public Inners._4 _4_1 { get; }
                public Inners._6 _6_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _47 : _percentx01ⲻ7F
            {
                public _47(Inners._4 _4_1, Inners._7 _7_1)
                {
                    this._4_1 = _4_1;
                    this._7_1 = _7_1;
                }
                
                public Inners._4 _4_1 { get; }
                public Inners._7 _7_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _48 : _percentx01ⲻ7F
            {
                public _48(Inners._4 _4_1, Inners._8 _8_1)
                {
                    this._4_1 = _4_1;
                    this._8_1 = _8_1;
                }
                
                public Inners._4 _4_1 { get; }
                public Inners._8 _8_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _49 : _percentx01ⲻ7F
            {
                public _49(Inners._4 _4_1, Inners._9 _9_1)
                {
                    this._4_1 = _4_1;
                    this._9_1 = _9_1;
                }
                
                public Inners._4 _4_1 { get; }
                public Inners._9 _9_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _4A : _percentx01ⲻ7F
            {
                public _4A(Inners._4 _4_1, Inners._A _A_1)
                {
                    this._4_1 = _4_1;
                    this._A_1 = _A_1;
                }
                
                public Inners._4 _4_1 { get; }
                public Inners._A _A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _4B : _percentx01ⲻ7F
            {
                public _4B(Inners._4 _4_1, Inners._B _B_1)
                {
                    this._4_1 = _4_1;
                    this._B_1 = _B_1;
                }
                
                public Inners._4 _4_1 { get; }
                public Inners._B _B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _4C : _percentx01ⲻ7F
            {
                public _4C(Inners._4 _4_1, Inners._C _C_1)
                {
                    this._4_1 = _4_1;
                    this._C_1 = _C_1;
                }
                
                public Inners._4 _4_1 { get; }
                public Inners._C _C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _4D : _percentx01ⲻ7F
            {
                public _4D(Inners._4 _4_1, Inners._D _D_1)
                {
                    this._4_1 = _4_1;
                    this._D_1 = _D_1;
                }
                
                public Inners._4 _4_1 { get; }
                public Inners._D _D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _4E : _percentx01ⲻ7F
            {
                public _4E(Inners._4 _4_1, Inners._E _E_1)
                {
                    this._4_1 = _4_1;
                    this._E_1 = _E_1;
                }
                
                public Inners._4 _4_1 { get; }
                public Inners._E _E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _4F : _percentx01ⲻ7F
            {
                public _4F(Inners._4 _4_1, Inners._F _F_1)
                {
                    this._4_1 = _4_1;
                    this._F_1 = _F_1;
                }
                
                public Inners._4 _4_1 { get; }
                public Inners._F _F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _50 : _percentx01ⲻ7F
            {
                public _50(Inners._5 _5_1, Inners._0 _0_1)
                {
                    this._5_1 = _5_1;
                    this._0_1 = _0_1;
                }
                
                public Inners._5 _5_1 { get; }
                public Inners._0 _0_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _51 : _percentx01ⲻ7F
            {
                public _51(Inners._5 _5_1, Inners._1 _1_1)
                {
                    this._5_1 = _5_1;
                    this._1_1 = _1_1;
                }
                
                public Inners._5 _5_1 { get; }
                public Inners._1 _1_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _52 : _percentx01ⲻ7F
            {
                public _52(Inners._5 _5_1, Inners._2 _2_1)
                {
                    this._5_1 = _5_1;
                    this._2_1 = _2_1;
                }
                
                public Inners._5 _5_1 { get; }
                public Inners._2 _2_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _53 : _percentx01ⲻ7F
            {
                public _53(Inners._5 _5_1, Inners._3 _3_1)
                {
                    this._5_1 = _5_1;
                    this._3_1 = _3_1;
                }
                
                public Inners._5 _5_1 { get; }
                public Inners._3 _3_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _54 : _percentx01ⲻ7F
            {
                public _54(Inners._5 _5_1, Inners._4 _4_1)
                {
                    this._5_1 = _5_1;
                    this._4_1 = _4_1;
                }
                
                public Inners._5 _5_1 { get; }
                public Inners._4 _4_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _55 : _percentx01ⲻ7F
            {
                public _55(Inners._5 _5_1, Inners._5 _5_2)
                {
                    this._5_1 = _5_1;
                    this._5_2 = _5_2;
                }
                
                public Inners._5 _5_1 { get; }
                public Inners._5 _5_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _56 : _percentx01ⲻ7F
            {
                public _56(Inners._5 _5_1, Inners._6 _6_1)
                {
                    this._5_1 = _5_1;
                    this._6_1 = _6_1;
                }
                
                public Inners._5 _5_1 { get; }
                public Inners._6 _6_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _57 : _percentx01ⲻ7F
            {
                public _57(Inners._5 _5_1, Inners._7 _7_1)
                {
                    this._5_1 = _5_1;
                    this._7_1 = _7_1;
                }
                
                public Inners._5 _5_1 { get; }
                public Inners._7 _7_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _58 : _percentx01ⲻ7F
            {
                public _58(Inners._5 _5_1, Inners._8 _8_1)
                {
                    this._5_1 = _5_1;
                    this._8_1 = _8_1;
                }
                
                public Inners._5 _5_1 { get; }
                public Inners._8 _8_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _59 : _percentx01ⲻ7F
            {
                public _59(Inners._5 _5_1, Inners._9 _9_1)
                {
                    this._5_1 = _5_1;
                    this._9_1 = _9_1;
                }
                
                public Inners._5 _5_1 { get; }
                public Inners._9 _9_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _5A : _percentx01ⲻ7F
            {
                public _5A(Inners._5 _5_1, Inners._A _A_1)
                {
                    this._5_1 = _5_1;
                    this._A_1 = _A_1;
                }
                
                public Inners._5 _5_1 { get; }
                public Inners._A _A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _5B : _percentx01ⲻ7F
            {
                public _5B(Inners._5 _5_1, Inners._B _B_1)
                {
                    this._5_1 = _5_1;
                    this._B_1 = _B_1;
                }
                
                public Inners._5 _5_1 { get; }
                public Inners._B _B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _5C : _percentx01ⲻ7F
            {
                public _5C(Inners._5 _5_1, Inners._C _C_1)
                {
                    this._5_1 = _5_1;
                    this._C_1 = _C_1;
                }
                
                public Inners._5 _5_1 { get; }
                public Inners._C _C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _5D : _percentx01ⲻ7F
            {
                public _5D(Inners._5 _5_1, Inners._D _D_1)
                {
                    this._5_1 = _5_1;
                    this._D_1 = _D_1;
                }
                
                public Inners._5 _5_1 { get; }
                public Inners._D _D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _5E : _percentx01ⲻ7F
            {
                public _5E(Inners._5 _5_1, Inners._E _E_1)
                {
                    this._5_1 = _5_1;
                    this._E_1 = _E_1;
                }
                
                public Inners._5 _5_1 { get; }
                public Inners._E _E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _5F : _percentx01ⲻ7F
            {
                public _5F(Inners._5 _5_1, Inners._F _F_1)
                {
                    this._5_1 = _5_1;
                    this._F_1 = _F_1;
                }
                
                public Inners._5 _5_1 { get; }
                public Inners._F _F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _60 : _percentx01ⲻ7F
            {
                public _60(Inners._6 _6_1, Inners._0 _0_1)
                {
                    this._6_1 = _6_1;
                    this._0_1 = _0_1;
                }
                
                public Inners._6 _6_1 { get; }
                public Inners._0 _0_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _61 : _percentx01ⲻ7F
            {
                public _61(Inners._6 _6_1, Inners._1 _1_1)
                {
                    this._6_1 = _6_1;
                    this._1_1 = _1_1;
                }
                
                public Inners._6 _6_1 { get; }
                public Inners._1 _1_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _62 : _percentx01ⲻ7F
            {
                public _62(Inners._6 _6_1, Inners._2 _2_1)
                {
                    this._6_1 = _6_1;
                    this._2_1 = _2_1;
                }
                
                public Inners._6 _6_1 { get; }
                public Inners._2 _2_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _63 : _percentx01ⲻ7F
            {
                public _63(Inners._6 _6_1, Inners._3 _3_1)
                {
                    this._6_1 = _6_1;
                    this._3_1 = _3_1;
                }
                
                public Inners._6 _6_1 { get; }
                public Inners._3 _3_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _64 : _percentx01ⲻ7F
            {
                public _64(Inners._6 _6_1, Inners._4 _4_1)
                {
                    this._6_1 = _6_1;
                    this._4_1 = _4_1;
                }
                
                public Inners._6 _6_1 { get; }
                public Inners._4 _4_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _65 : _percentx01ⲻ7F
            {
                public _65(Inners._6 _6_1, Inners._5 _5_1)
                {
                    this._6_1 = _6_1;
                    this._5_1 = _5_1;
                }
                
                public Inners._6 _6_1 { get; }
                public Inners._5 _5_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _66 : _percentx01ⲻ7F
            {
                public _66(Inners._6 _6_1, Inners._6 _6_2)
                {
                    this._6_1 = _6_1;
                    this._6_2 = _6_2;
                }
                
                public Inners._6 _6_1 { get; }
                public Inners._6 _6_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _67 : _percentx01ⲻ7F
            {
                public _67(Inners._6 _6_1, Inners._7 _7_1)
                {
                    this._6_1 = _6_1;
                    this._7_1 = _7_1;
                }
                
                public Inners._6 _6_1 { get; }
                public Inners._7 _7_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _68 : _percentx01ⲻ7F
            {
                public _68(Inners._6 _6_1, Inners._8 _8_1)
                {
                    this._6_1 = _6_1;
                    this._8_1 = _8_1;
                }
                
                public Inners._6 _6_1 { get; }
                public Inners._8 _8_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _69 : _percentx01ⲻ7F
            {
                public _69(Inners._6 _6_1, Inners._9 _9_1)
                {
                    this._6_1 = _6_1;
                    this._9_1 = _9_1;
                }
                
                public Inners._6 _6_1 { get; }
                public Inners._9 _9_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _6A : _percentx01ⲻ7F
            {
                public _6A(Inners._6 _6_1, Inners._A _A_1)
                {
                    this._6_1 = _6_1;
                    this._A_1 = _A_1;
                }
                
                public Inners._6 _6_1 { get; }
                public Inners._A _A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _6B : _percentx01ⲻ7F
            {
                public _6B(Inners._6 _6_1, Inners._B _B_1)
                {
                    this._6_1 = _6_1;
                    this._B_1 = _B_1;
                }
                
                public Inners._6 _6_1 { get; }
                public Inners._B _B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _6C : _percentx01ⲻ7F
            {
                public _6C(Inners._6 _6_1, Inners._C _C_1)
                {
                    this._6_1 = _6_1;
                    this._C_1 = _C_1;
                }
                
                public Inners._6 _6_1 { get; }
                public Inners._C _C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _6D : _percentx01ⲻ7F
            {
                public _6D(Inners._6 _6_1, Inners._D _D_1)
                {
                    this._6_1 = _6_1;
                    this._D_1 = _D_1;
                }
                
                public Inners._6 _6_1 { get; }
                public Inners._D _D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _6E : _percentx01ⲻ7F
            {
                public _6E(Inners._6 _6_1, Inners._E _E_1)
                {
                    this._6_1 = _6_1;
                    this._E_1 = _E_1;
                }
                
                public Inners._6 _6_1 { get; }
                public Inners._E _E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _6F : _percentx01ⲻ7F
            {
                public _6F(Inners._6 _6_1, Inners._F _F_1)
                {
                    this._6_1 = _6_1;
                    this._F_1 = _F_1;
                }
                
                public Inners._6 _6_1 { get; }
                public Inners._F _F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _70 : _percentx01ⲻ7F
            {
                public _70(Inners._7 _7_1, Inners._0 _0_1)
                {
                    this._7_1 = _7_1;
                    this._0_1 = _0_1;
                }
                
                public Inners._7 _7_1 { get; }
                public Inners._0 _0_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _71 : _percentx01ⲻ7F
            {
                public _71(Inners._7 _7_1, Inners._1 _1_1)
                {
                    this._7_1 = _7_1;
                    this._1_1 = _1_1;
                }
                
                public Inners._7 _7_1 { get; }
                public Inners._1 _1_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _72 : _percentx01ⲻ7F
            {
                public _72(Inners._7 _7_1, Inners._2 _2_1)
                {
                    this._7_1 = _7_1;
                    this._2_1 = _2_1;
                }
                
                public Inners._7 _7_1 { get; }
                public Inners._2 _2_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _73 : _percentx01ⲻ7F
            {
                public _73(Inners._7 _7_1, Inners._3 _3_1)
                {
                    this._7_1 = _7_1;
                    this._3_1 = _3_1;
                }
                
                public Inners._7 _7_1 { get; }
                public Inners._3 _3_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _74 : _percentx01ⲻ7F
            {
                public _74(Inners._7 _7_1, Inners._4 _4_1)
                {
                    this._7_1 = _7_1;
                    this._4_1 = _4_1;
                }
                
                public Inners._7 _7_1 { get; }
                public Inners._4 _4_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _75 : _percentx01ⲻ7F
            {
                public _75(Inners._7 _7_1, Inners._5 _5_1)
                {
                    this._7_1 = _7_1;
                    this._5_1 = _5_1;
                }
                
                public Inners._7 _7_1 { get; }
                public Inners._5 _5_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _76 : _percentx01ⲻ7F
            {
                public _76(Inners._7 _7_1, Inners._6 _6_1)
                {
                    this._7_1 = _7_1;
                    this._6_1 = _6_1;
                }
                
                public Inners._7 _7_1 { get; }
                public Inners._6 _6_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _77 : _percentx01ⲻ7F
            {
                public _77(Inners._7 _7_1, Inners._7 _7_2)
                {
                    this._7_1 = _7_1;
                    this._7_2 = _7_2;
                }
                
                public Inners._7 _7_1 { get; }
                public Inners._7 _7_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _78 : _percentx01ⲻ7F
            {
                public _78(Inners._7 _7_1, Inners._8 _8_1)
                {
                    this._7_1 = _7_1;
                    this._8_1 = _8_1;
                }
                
                public Inners._7 _7_1 { get; }
                public Inners._8 _8_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _79 : _percentx01ⲻ7F
            {
                public _79(Inners._7 _7_1, Inners._9 _9_1)
                {
                    this._7_1 = _7_1;
                    this._9_1 = _9_1;
                }
                
                public Inners._7 _7_1 { get; }
                public Inners._9 _9_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _7A : _percentx01ⲻ7F
            {
                public _7A(Inners._7 _7_1, Inners._A _A_1)
                {
                    this._7_1 = _7_1;
                    this._A_1 = _A_1;
                }
                
                public Inners._7 _7_1 { get; }
                public Inners._A _A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _7B : _percentx01ⲻ7F
            {
                public _7B(Inners._7 _7_1, Inners._B _B_1)
                {
                    this._7_1 = _7_1;
                    this._B_1 = _B_1;
                }
                
                public Inners._7 _7_1 { get; }
                public Inners._B _B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _7C : _percentx01ⲻ7F
            {
                public _7C(Inners._7 _7_1, Inners._C _C_1)
                {
                    this._7_1 = _7_1;
                    this._C_1 = _C_1;
                }
                
                public Inners._7 _7_1 { get; }
                public Inners._C _C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _7D : _percentx01ⲻ7F
            {
                public _7D(Inners._7 _7_1, Inners._D _D_1)
                {
                    this._7_1 = _7_1;
                    this._D_1 = _D_1;
                }
                
                public Inners._7 _7_1 { get; }
                public Inners._D _D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _7E : _percentx01ⲻ7F
            {
                public _7E(Inners._7 _7_1, Inners._E _E_1)
                {
                    this._7_1 = _7_1;
                    this._E_1 = _E_1;
                }
                
                public Inners._7 _7_1 { get; }
                public Inners._E _E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _7F : _percentx01ⲻ7F
            {
                public _7F(Inners._7 _7_1, Inners._F _F_1)
                {
                    this._7_1 = _7_1;
                    this._F_1 = _F_1;
                }
                
                public Inners._7 _7_1 { get; }
                public Inners._F _F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
        }
        
        public sealed class _percentx0D
        {
            public _percentx0D(Inners._0 _0_1, Inners._D _D_1)
            {
                this._0_1 = _0_1;
                this._D_1 = _D_1;
            }
            
            public Inners._0 _0_1 { get; }
            public Inners._D _D_1 { get; }
        }
        
        public abstract class _percentx00ⲻ1F
        {
            private _percentx00ⲻ1F()
            {
            }
            
            protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
            
            public abstract class Visitor<TResult, TContext>
            {
                public TResult Visit(_percentx00ⲻ1F node, TContext context)
                {
                    return node.Dispatch(this, context);
                }
                
                protected internal abstract TResult Accept(_percentx00ⲻ1F._00 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻ1F._01 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻ1F._02 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻ1F._03 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻ1F._04 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻ1F._05 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻ1F._06 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻ1F._07 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻ1F._08 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻ1F._09 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻ1F._0A node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻ1F._0B node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻ1F._0C node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻ1F._0D node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻ1F._0E node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻ1F._0F node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻ1F._10 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻ1F._11 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻ1F._12 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻ1F._13 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻ1F._14 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻ1F._15 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻ1F._16 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻ1F._17 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻ1F._18 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻ1F._19 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻ1F._1A node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻ1F._1B node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻ1F._1C node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻ1F._1D node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻ1F._1E node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻ1F._1F node, TContext context);
            }
            
            public sealed class _00 : _percentx00ⲻ1F
            {
                public _00(Inners._0 _0_1, Inners._0 _0_2)
                {
                    this._0_1 = _0_1;
                    this._0_2 = _0_2;
                }
                
                public Inners._0 _0_1 { get; }
                public Inners._0 _0_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _01 : _percentx00ⲻ1F
            {
                public _01(Inners._0 _0_1, Inners._1 _1_1)
                {
                    this._0_1 = _0_1;
                    this._1_1 = _1_1;
                }
                
                public Inners._0 _0_1 { get; }
                public Inners._1 _1_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _02 : _percentx00ⲻ1F
            {
                public _02(Inners._0 _0_1, Inners._2 _2_1)
                {
                    this._0_1 = _0_1;
                    this._2_1 = _2_1;
                }
                
                public Inners._0 _0_1 { get; }
                public Inners._2 _2_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _03 : _percentx00ⲻ1F
            {
                public _03(Inners._0 _0_1, Inners._3 _3_1)
                {
                    this._0_1 = _0_1;
                    this._3_1 = _3_1;
                }
                
                public Inners._0 _0_1 { get; }
                public Inners._3 _3_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _04 : _percentx00ⲻ1F
            {
                public _04(Inners._0 _0_1, Inners._4 _4_1)
                {
                    this._0_1 = _0_1;
                    this._4_1 = _4_1;
                }
                
                public Inners._0 _0_1 { get; }
                public Inners._4 _4_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _05 : _percentx00ⲻ1F
            {
                public _05(Inners._0 _0_1, Inners._5 _5_1)
                {
                    this._0_1 = _0_1;
                    this._5_1 = _5_1;
                }
                
                public Inners._0 _0_1 { get; }
                public Inners._5 _5_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _06 : _percentx00ⲻ1F
            {
                public _06(Inners._0 _0_1, Inners._6 _6_1)
                {
                    this._0_1 = _0_1;
                    this._6_1 = _6_1;
                }
                
                public Inners._0 _0_1 { get; }
                public Inners._6 _6_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _07 : _percentx00ⲻ1F
            {
                public _07(Inners._0 _0_1, Inners._7 _7_1)
                {
                    this._0_1 = _0_1;
                    this._7_1 = _7_1;
                }
                
                public Inners._0 _0_1 { get; }
                public Inners._7 _7_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _08 : _percentx00ⲻ1F
            {
                public _08(Inners._0 _0_1, Inners._8 _8_1)
                {
                    this._0_1 = _0_1;
                    this._8_1 = _8_1;
                }
                
                public Inners._0 _0_1 { get; }
                public Inners._8 _8_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _09 : _percentx00ⲻ1F
            {
                public _09(Inners._0 _0_1, Inners._9 _9_1)
                {
                    this._0_1 = _0_1;
                    this._9_1 = _9_1;
                }
                
                public Inners._0 _0_1 { get; }
                public Inners._9 _9_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _0A : _percentx00ⲻ1F
            {
                public _0A(Inners._0 _0_1, Inners._A _A_1)
                {
                    this._0_1 = _0_1;
                    this._A_1 = _A_1;
                }
                
                public Inners._0 _0_1 { get; }
                public Inners._A _A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _0B : _percentx00ⲻ1F
            {
                public _0B(Inners._0 _0_1, Inners._B _B_1)
                {
                    this._0_1 = _0_1;
                    this._B_1 = _B_1;
                }
                
                public Inners._0 _0_1 { get; }
                public Inners._B _B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _0C : _percentx00ⲻ1F
            {
                public _0C(Inners._0 _0_1, Inners._C _C_1)
                {
                    this._0_1 = _0_1;
                    this._C_1 = _C_1;
                }
                
                public Inners._0 _0_1 { get; }
                public Inners._C _C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _0D : _percentx00ⲻ1F
            {
                public _0D(Inners._0 _0_1, Inners._D _D_1)
                {
                    this._0_1 = _0_1;
                    this._D_1 = _D_1;
                }
                
                public Inners._0 _0_1 { get; }
                public Inners._D _D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _0E : _percentx00ⲻ1F
            {
                public _0E(Inners._0 _0_1, Inners._E _E_1)
                {
                    this._0_1 = _0_1;
                    this._E_1 = _E_1;
                }
                
                public Inners._0 _0_1 { get; }
                public Inners._E _E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _0F : _percentx00ⲻ1F
            {
                public _0F(Inners._0 _0_1, Inners._F _F_1)
                {
                    this._0_1 = _0_1;
                    this._F_1 = _F_1;
                }
                
                public Inners._0 _0_1 { get; }
                public Inners._F _F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _10 : _percentx00ⲻ1F
            {
                public _10(Inners._1 _1_1, Inners._0 _0_1)
                {
                    this._1_1 = _1_1;
                    this._0_1 = _0_1;
                }
                
                public Inners._1 _1_1 { get; }
                public Inners._0 _0_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _11 : _percentx00ⲻ1F
            {
                public _11(Inners._1 _1_1, Inners._1 _1_2)
                {
                    this._1_1 = _1_1;
                    this._1_2 = _1_2;
                }
                
                public Inners._1 _1_1 { get; }
                public Inners._1 _1_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _12 : _percentx00ⲻ1F
            {
                public _12(Inners._1 _1_1, Inners._2 _2_1)
                {
                    this._1_1 = _1_1;
                    this._2_1 = _2_1;
                }
                
                public Inners._1 _1_1 { get; }
                public Inners._2 _2_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _13 : _percentx00ⲻ1F
            {
                public _13(Inners._1 _1_1, Inners._3 _3_1)
                {
                    this._1_1 = _1_1;
                    this._3_1 = _3_1;
                }
                
                public Inners._1 _1_1 { get; }
                public Inners._3 _3_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _14 : _percentx00ⲻ1F
            {
                public _14(Inners._1 _1_1, Inners._4 _4_1)
                {
                    this._1_1 = _1_1;
                    this._4_1 = _4_1;
                }
                
                public Inners._1 _1_1 { get; }
                public Inners._4 _4_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _15 : _percentx00ⲻ1F
            {
                public _15(Inners._1 _1_1, Inners._5 _5_1)
                {
                    this._1_1 = _1_1;
                    this._5_1 = _5_1;
                }
                
                public Inners._1 _1_1 { get; }
                public Inners._5 _5_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _16 : _percentx00ⲻ1F
            {
                public _16(Inners._1 _1_1, Inners._6 _6_1)
                {
                    this._1_1 = _1_1;
                    this._6_1 = _6_1;
                }
                
                public Inners._1 _1_1 { get; }
                public Inners._6 _6_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _17 : _percentx00ⲻ1F
            {
                public _17(Inners._1 _1_1, Inners._7 _7_1)
                {
                    this._1_1 = _1_1;
                    this._7_1 = _7_1;
                }
                
                public Inners._1 _1_1 { get; }
                public Inners._7 _7_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _18 : _percentx00ⲻ1F
            {
                public _18(Inners._1 _1_1, Inners._8 _8_1)
                {
                    this._1_1 = _1_1;
                    this._8_1 = _8_1;
                }
                
                public Inners._1 _1_1 { get; }
                public Inners._8 _8_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _19 : _percentx00ⲻ1F
            {
                public _19(Inners._1 _1_1, Inners._9 _9_1)
                {
                    this._1_1 = _1_1;
                    this._9_1 = _9_1;
                }
                
                public Inners._1 _1_1 { get; }
                public Inners._9 _9_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _1A : _percentx00ⲻ1F
            {
                public _1A(Inners._1 _1_1, Inners._A _A_1)
                {
                    this._1_1 = _1_1;
                    this._A_1 = _A_1;
                }
                
                public Inners._1 _1_1 { get; }
                public Inners._A _A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _1B : _percentx00ⲻ1F
            {
                public _1B(Inners._1 _1_1, Inners._B _B_1)
                {
                    this._1_1 = _1_1;
                    this._B_1 = _B_1;
                }
                
                public Inners._1 _1_1 { get; }
                public Inners._B _B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _1C : _percentx00ⲻ1F
            {
                public _1C(Inners._1 _1_1, Inners._C _C_1)
                {
                    this._1_1 = _1_1;
                    this._C_1 = _C_1;
                }
                
                public Inners._1 _1_1 { get; }
                public Inners._C _C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _1D : _percentx00ⲻ1F
            {
                public _1D(Inners._1 _1_1, Inners._D _D_1)
                {
                    this._1_1 = _1_1;
                    this._D_1 = _D_1;
                }
                
                public Inners._1 _1_1 { get; }
                public Inners._D _D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _1E : _percentx00ⲻ1F
            {
                public _1E(Inners._1 _1_1, Inners._E _E_1)
                {
                    this._1_1 = _1_1;
                    this._E_1 = _E_1;
                }
                
                public Inners._1 _1_1 { get; }
                public Inners._E _E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _1F : _percentx00ⲻ1F
            {
                public _1F(Inners._1 _1_1, Inners._F _F_1)
                {
                    this._1_1 = _1_1;
                    this._F_1 = _F_1;
                }
                
                public Inners._1 _1_1 { get; }
                public Inners._F _F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
        }
        
        public sealed class _percentx7F
        {
            public _percentx7F(Inners._7 _7_1, Inners._F _F_1)
            {
                this._7_1 = _7_1;
                this._F_1 = _F_1;
            }
            
            public Inners._7 _7_1 { get; }
            public Inners._F _F_1 { get; }
        }
        
        public abstract class _percentx30ⲻ39
        {
            private _percentx30ⲻ39()
            {
            }
            
            protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
            
            public abstract class Visitor<TResult, TContext>
            {
                public TResult Visit(_percentx30ⲻ39 node, TContext context)
                {
                    return node.Dispatch(this, context);
                }
                
                protected internal abstract TResult Accept(_percentx30ⲻ39._30 node, TContext context);
                protected internal abstract TResult Accept(_percentx30ⲻ39._31 node, TContext context);
                protected internal abstract TResult Accept(_percentx30ⲻ39._32 node, TContext context);
                protected internal abstract TResult Accept(_percentx30ⲻ39._33 node, TContext context);
                protected internal abstract TResult Accept(_percentx30ⲻ39._34 node, TContext context);
                protected internal abstract TResult Accept(_percentx30ⲻ39._35 node, TContext context);
                protected internal abstract TResult Accept(_percentx30ⲻ39._36 node, TContext context);
                protected internal abstract TResult Accept(_percentx30ⲻ39._37 node, TContext context);
                protected internal abstract TResult Accept(_percentx30ⲻ39._38 node, TContext context);
                protected internal abstract TResult Accept(_percentx30ⲻ39._39 node, TContext context);
            }
            
            public sealed class _30 : _percentx30ⲻ39
            {
                public _30(Inners._3 _3_1, Inners._0 _0_1)
                {
                    this._3_1 = _3_1;
                    this._0_1 = _0_1;
                }
                
                public Inners._3 _3_1 { get; }
                public Inners._0 _0_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _31 : _percentx30ⲻ39
            {
                public _31(Inners._3 _3_1, Inners._1 _1_1)
                {
                    this._3_1 = _3_1;
                    this._1_1 = _1_1;
                }
                
                public Inners._3 _3_1 { get; }
                public Inners._1 _1_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _32 : _percentx30ⲻ39
            {
                public _32(Inners._3 _3_1, Inners._2 _2_1)
                {
                    this._3_1 = _3_1;
                    this._2_1 = _2_1;
                }
                
                public Inners._3 _3_1 { get; }
                public Inners._2 _2_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _33 : _percentx30ⲻ39
            {
                public _33(Inners._3 _3_1, Inners._3 _3_2)
                {
                    this._3_1 = _3_1;
                    this._3_2 = _3_2;
                }
                
                public Inners._3 _3_1 { get; }
                public Inners._3 _3_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _34 : _percentx30ⲻ39
            {
                public _34(Inners._3 _3_1, Inners._4 _4_1)
                {
                    this._3_1 = _3_1;
                    this._4_1 = _4_1;
                }
                
                public Inners._3 _3_1 { get; }
                public Inners._4 _4_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _35 : _percentx30ⲻ39
            {
                public _35(Inners._3 _3_1, Inners._5 _5_1)
                {
                    this._3_1 = _3_1;
                    this._5_1 = _5_1;
                }
                
                public Inners._3 _3_1 { get; }
                public Inners._5 _5_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _36 : _percentx30ⲻ39
            {
                public _36(Inners._3 _3_1, Inners._6 _6_1)
                {
                    this._3_1 = _3_1;
                    this._6_1 = _6_1;
                }
                
                public Inners._3 _3_1 { get; }
                public Inners._6 _6_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _37 : _percentx30ⲻ39
            {
                public _37(Inners._3 _3_1, Inners._7 _7_1)
                {
                    this._3_1 = _3_1;
                    this._7_1 = _7_1;
                }
                
                public Inners._3 _3_1 { get; }
                public Inners._7 _7_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _38 : _percentx30ⲻ39
            {
                public _38(Inners._3 _3_1, Inners._8 _8_1)
                {
                    this._3_1 = _3_1;
                    this._8_1 = _8_1;
                }
                
                public Inners._3 _3_1 { get; }
                public Inners._8 _8_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _39 : _percentx30ⲻ39
            {
                public _39(Inners._3 _3_1, Inners._9 _9_1)
                {
                    this._3_1 = _3_1;
                    this._9_1 = _9_1;
                }
                
                public Inners._3 _3_1 { get; }
                public Inners._9 _9_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
        }
        
        public sealed class _percentx22
        {
            public _percentx22(Inners._2 _2_1, Inners._2 _2_2)
            {
                this._2_1 = _2_1;
                this._2_2 = _2_2;
            }
            
            public Inners._2 _2_1 { get; }
            public Inners._2 _2_2 { get; }
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
        
        public sealed class _percentx09
        {
            public _percentx09(Inners._0 _0_1, Inners._9 _9_1)
            {
                this._0_1 = _0_1;
                this._9_1 = _9_1;
            }
            
            public Inners._0 _0_1 { get; }
            public Inners._9 _9_1 { get; }
        }
        
        public sealed class _percentx0A
        {
            public _percentx0A(Inners._0 _0_1, Inners._A _A_1)
            {
                this._0_1 = _0_1;
                this._A_1 = _A_1;
            }
            
            public Inners._0 _0_1 { get; }
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
        
        public abstract class _percentx00ⲻFF
        {
            private _percentx00ⲻFF()
            {
            }
            
            protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
            
            public abstract class Visitor<TResult, TContext>
            {
                public TResult Visit(_percentx00ⲻFF node, TContext context)
                {
                    return node.Dispatch(this, context);
                }
                
                protected internal abstract TResult Accept(_percentx00ⲻFF._00 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._01 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._02 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._03 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._04 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._05 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._06 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._07 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._08 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._09 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._0A node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._0B node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._0C node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._0D node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._0E node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._0F node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._10 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._11 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._12 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._13 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._14 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._15 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._16 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._17 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._18 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._19 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._1A node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._1B node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._1C node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._1D node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._1E node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._1F node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._20 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._21 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._22 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._23 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._24 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._25 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._26 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._27 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._28 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._29 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._2A node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._2B node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._2C node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._2D node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._2E node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._2F node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._30 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._31 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._32 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._33 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._34 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._35 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._36 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._37 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._38 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._39 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._3A node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._3B node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._3C node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._3D node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._3E node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._3F node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._40 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._41 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._42 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._43 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._44 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._45 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._46 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._47 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._48 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._49 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._4A node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._4B node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._4C node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._4D node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._4E node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._4F node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._50 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._51 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._52 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._53 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._54 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._55 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._56 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._57 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._58 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._59 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._5A node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._5B node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._5C node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._5D node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._5E node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._5F node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._60 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._61 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._62 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._63 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._64 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._65 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._66 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._67 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._68 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._69 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._6A node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._6B node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._6C node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._6D node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._6E node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._6F node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._70 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._71 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._72 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._73 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._74 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._75 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._76 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._77 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._78 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._79 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._7A node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._7B node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._7C node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._7D node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._7E node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._7F node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._80 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._81 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._82 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._83 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._84 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._85 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._86 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._87 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._88 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._89 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._8A node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._8B node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._8C node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._8D node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._8E node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._8F node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._90 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._91 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._92 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._93 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._94 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._95 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._96 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._97 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._98 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._99 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._9A node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._9B node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._9C node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._9D node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._9E node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._9F node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._A0 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._A1 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._A2 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._A3 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._A4 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._A5 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._A6 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._A7 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._A8 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._A9 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._AA node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._AB node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._AC node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._AD node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._AE node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._AF node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._B0 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._B1 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._B2 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._B3 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._B4 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._B5 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._B6 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._B7 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._B8 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._B9 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._BA node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._BB node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._BC node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._BD node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._BE node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._BF node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._C0 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._C1 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._C2 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._C3 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._C4 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._C5 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._C6 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._C7 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._C8 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._C9 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._CA node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._CB node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._CC node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._CD node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._CE node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._CF node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._D0 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._D1 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._D2 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._D3 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._D4 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._D5 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._D6 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._D7 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._D8 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._D9 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._DA node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._DB node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._DC node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._DD node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._DE node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._DF node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._E0 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._E1 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._E2 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._E3 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._E4 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._E5 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._E6 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._E7 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._E8 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._E9 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._EA node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._EB node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._EC node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._ED node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._EE node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._EF node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._F0 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._F1 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._F2 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._F3 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._F4 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._F5 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._F6 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._F7 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._F8 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._F9 node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._FA node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._FB node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._FC node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._FD node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._FE node, TContext context);
                protected internal abstract TResult Accept(_percentx00ⲻFF._FF node, TContext context);
            }
            
            public sealed class _00 : _percentx00ⲻFF
            {
                public _00(Inners._0 _0_1, Inners._0 _0_2)
                {
                    this._0_1 = _0_1;
                    this._0_2 = _0_2;
                }
                
                public Inners._0 _0_1 { get; }
                public Inners._0 _0_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _01 : _percentx00ⲻFF
            {
                public _01(Inners._0 _0_1, Inners._1 _1_1)
                {
                    this._0_1 = _0_1;
                    this._1_1 = _1_1;
                }
                
                public Inners._0 _0_1 { get; }
                public Inners._1 _1_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _02 : _percentx00ⲻFF
            {
                public _02(Inners._0 _0_1, Inners._2 _2_1)
                {
                    this._0_1 = _0_1;
                    this._2_1 = _2_1;
                }
                
                public Inners._0 _0_1 { get; }
                public Inners._2 _2_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _03 : _percentx00ⲻFF
            {
                public _03(Inners._0 _0_1, Inners._3 _3_1)
                {
                    this._0_1 = _0_1;
                    this._3_1 = _3_1;
                }
                
                public Inners._0 _0_1 { get; }
                public Inners._3 _3_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _04 : _percentx00ⲻFF
            {
                public _04(Inners._0 _0_1, Inners._4 _4_1)
                {
                    this._0_1 = _0_1;
                    this._4_1 = _4_1;
                }
                
                public Inners._0 _0_1 { get; }
                public Inners._4 _4_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _05 : _percentx00ⲻFF
            {
                public _05(Inners._0 _0_1, Inners._5 _5_1)
                {
                    this._0_1 = _0_1;
                    this._5_1 = _5_1;
                }
                
                public Inners._0 _0_1 { get; }
                public Inners._5 _5_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _06 : _percentx00ⲻFF
            {
                public _06(Inners._0 _0_1, Inners._6 _6_1)
                {
                    this._0_1 = _0_1;
                    this._6_1 = _6_1;
                }
                
                public Inners._0 _0_1 { get; }
                public Inners._6 _6_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _07 : _percentx00ⲻFF
            {
                public _07(Inners._0 _0_1, Inners._7 _7_1)
                {
                    this._0_1 = _0_1;
                    this._7_1 = _7_1;
                }
                
                public Inners._0 _0_1 { get; }
                public Inners._7 _7_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _08 : _percentx00ⲻFF
            {
                public _08(Inners._0 _0_1, Inners._8 _8_1)
                {
                    this._0_1 = _0_1;
                    this._8_1 = _8_1;
                }
                
                public Inners._0 _0_1 { get; }
                public Inners._8 _8_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _09 : _percentx00ⲻFF
            {
                public _09(Inners._0 _0_1, Inners._9 _9_1)
                {
                    this._0_1 = _0_1;
                    this._9_1 = _9_1;
                }
                
                public Inners._0 _0_1 { get; }
                public Inners._9 _9_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _0A : _percentx00ⲻFF
            {
                public _0A(Inners._0 _0_1, Inners._A _A_1)
                {
                    this._0_1 = _0_1;
                    this._A_1 = _A_1;
                }
                
                public Inners._0 _0_1 { get; }
                public Inners._A _A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _0B : _percentx00ⲻFF
            {
                public _0B(Inners._0 _0_1, Inners._B _B_1)
                {
                    this._0_1 = _0_1;
                    this._B_1 = _B_1;
                }
                
                public Inners._0 _0_1 { get; }
                public Inners._B _B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _0C : _percentx00ⲻFF
            {
                public _0C(Inners._0 _0_1, Inners._C _C_1)
                {
                    this._0_1 = _0_1;
                    this._C_1 = _C_1;
                }
                
                public Inners._0 _0_1 { get; }
                public Inners._C _C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _0D : _percentx00ⲻFF
            {
                public _0D(Inners._0 _0_1, Inners._D _D_1)
                {
                    this._0_1 = _0_1;
                    this._D_1 = _D_1;
                }
                
                public Inners._0 _0_1 { get; }
                public Inners._D _D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _0E : _percentx00ⲻFF
            {
                public _0E(Inners._0 _0_1, Inners._E _E_1)
                {
                    this._0_1 = _0_1;
                    this._E_1 = _E_1;
                }
                
                public Inners._0 _0_1 { get; }
                public Inners._E _E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _0F : _percentx00ⲻFF
            {
                public _0F(Inners._0 _0_1, Inners._F _F_1)
                {
                    this._0_1 = _0_1;
                    this._F_1 = _F_1;
                }
                
                public Inners._0 _0_1 { get; }
                public Inners._F _F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _10 : _percentx00ⲻFF
            {
                public _10(Inners._1 _1_1, Inners._0 _0_1)
                {
                    this._1_1 = _1_1;
                    this._0_1 = _0_1;
                }
                
                public Inners._1 _1_1 { get; }
                public Inners._0 _0_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _11 : _percentx00ⲻFF
            {
                public _11(Inners._1 _1_1, Inners._1 _1_2)
                {
                    this._1_1 = _1_1;
                    this._1_2 = _1_2;
                }
                
                public Inners._1 _1_1 { get; }
                public Inners._1 _1_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _12 : _percentx00ⲻFF
            {
                public _12(Inners._1 _1_1, Inners._2 _2_1)
                {
                    this._1_1 = _1_1;
                    this._2_1 = _2_1;
                }
                
                public Inners._1 _1_1 { get; }
                public Inners._2 _2_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _13 : _percentx00ⲻFF
            {
                public _13(Inners._1 _1_1, Inners._3 _3_1)
                {
                    this._1_1 = _1_1;
                    this._3_1 = _3_1;
                }
                
                public Inners._1 _1_1 { get; }
                public Inners._3 _3_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _14 : _percentx00ⲻFF
            {
                public _14(Inners._1 _1_1, Inners._4 _4_1)
                {
                    this._1_1 = _1_1;
                    this._4_1 = _4_1;
                }
                
                public Inners._1 _1_1 { get; }
                public Inners._4 _4_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _15 : _percentx00ⲻFF
            {
                public _15(Inners._1 _1_1, Inners._5 _5_1)
                {
                    this._1_1 = _1_1;
                    this._5_1 = _5_1;
                }
                
                public Inners._1 _1_1 { get; }
                public Inners._5 _5_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _16 : _percentx00ⲻFF
            {
                public _16(Inners._1 _1_1, Inners._6 _6_1)
                {
                    this._1_1 = _1_1;
                    this._6_1 = _6_1;
                }
                
                public Inners._1 _1_1 { get; }
                public Inners._6 _6_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _17 : _percentx00ⲻFF
            {
                public _17(Inners._1 _1_1, Inners._7 _7_1)
                {
                    this._1_1 = _1_1;
                    this._7_1 = _7_1;
                }
                
                public Inners._1 _1_1 { get; }
                public Inners._7 _7_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _18 : _percentx00ⲻFF
            {
                public _18(Inners._1 _1_1, Inners._8 _8_1)
                {
                    this._1_1 = _1_1;
                    this._8_1 = _8_1;
                }
                
                public Inners._1 _1_1 { get; }
                public Inners._8 _8_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _19 : _percentx00ⲻFF
            {
                public _19(Inners._1 _1_1, Inners._9 _9_1)
                {
                    this._1_1 = _1_1;
                    this._9_1 = _9_1;
                }
                
                public Inners._1 _1_1 { get; }
                public Inners._9 _9_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _1A : _percentx00ⲻFF
            {
                public _1A(Inners._1 _1_1, Inners._A _A_1)
                {
                    this._1_1 = _1_1;
                    this._A_1 = _A_1;
                }
                
                public Inners._1 _1_1 { get; }
                public Inners._A _A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _1B : _percentx00ⲻFF
            {
                public _1B(Inners._1 _1_1, Inners._B _B_1)
                {
                    this._1_1 = _1_1;
                    this._B_1 = _B_1;
                }
                
                public Inners._1 _1_1 { get; }
                public Inners._B _B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _1C : _percentx00ⲻFF
            {
                public _1C(Inners._1 _1_1, Inners._C _C_1)
                {
                    this._1_1 = _1_1;
                    this._C_1 = _C_1;
                }
                
                public Inners._1 _1_1 { get; }
                public Inners._C _C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _1D : _percentx00ⲻFF
            {
                public _1D(Inners._1 _1_1, Inners._D _D_1)
                {
                    this._1_1 = _1_1;
                    this._D_1 = _D_1;
                }
                
                public Inners._1 _1_1 { get; }
                public Inners._D _D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _1E : _percentx00ⲻFF
            {
                public _1E(Inners._1 _1_1, Inners._E _E_1)
                {
                    this._1_1 = _1_1;
                    this._E_1 = _E_1;
                }
                
                public Inners._1 _1_1 { get; }
                public Inners._E _E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _1F : _percentx00ⲻFF
            {
                public _1F(Inners._1 _1_1, Inners._F _F_1)
                {
                    this._1_1 = _1_1;
                    this._F_1 = _F_1;
                }
                
                public Inners._1 _1_1 { get; }
                public Inners._F _F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _20 : _percentx00ⲻFF
            {
                public _20(Inners._2 _2_1, Inners._0 _0_1)
                {
                    this._2_1 = _2_1;
                    this._0_1 = _0_1;
                }
                
                public Inners._2 _2_1 { get; }
                public Inners._0 _0_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _21 : _percentx00ⲻFF
            {
                public _21(Inners._2 _2_1, Inners._1 _1_1)
                {
                    this._2_1 = _2_1;
                    this._1_1 = _1_1;
                }
                
                public Inners._2 _2_1 { get; }
                public Inners._1 _1_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _22 : _percentx00ⲻFF
            {
                public _22(Inners._2 _2_1, Inners._2 _2_2)
                {
                    this._2_1 = _2_1;
                    this._2_2 = _2_2;
                }
                
                public Inners._2 _2_1 { get; }
                public Inners._2 _2_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _23 : _percentx00ⲻFF
            {
                public _23(Inners._2 _2_1, Inners._3 _3_1)
                {
                    this._2_1 = _2_1;
                    this._3_1 = _3_1;
                }
                
                public Inners._2 _2_1 { get; }
                public Inners._3 _3_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _24 : _percentx00ⲻFF
            {
                public _24(Inners._2 _2_1, Inners._4 _4_1)
                {
                    this._2_1 = _2_1;
                    this._4_1 = _4_1;
                }
                
                public Inners._2 _2_1 { get; }
                public Inners._4 _4_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _25 : _percentx00ⲻFF
            {
                public _25(Inners._2 _2_1, Inners._5 _5_1)
                {
                    this._2_1 = _2_1;
                    this._5_1 = _5_1;
                }
                
                public Inners._2 _2_1 { get; }
                public Inners._5 _5_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _26 : _percentx00ⲻFF
            {
                public _26(Inners._2 _2_1, Inners._6 _6_1)
                {
                    this._2_1 = _2_1;
                    this._6_1 = _6_1;
                }
                
                public Inners._2 _2_1 { get; }
                public Inners._6 _6_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _27 : _percentx00ⲻFF
            {
                public _27(Inners._2 _2_1, Inners._7 _7_1)
                {
                    this._2_1 = _2_1;
                    this._7_1 = _7_1;
                }
                
                public Inners._2 _2_1 { get; }
                public Inners._7 _7_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _28 : _percentx00ⲻFF
            {
                public _28(Inners._2 _2_1, Inners._8 _8_1)
                {
                    this._2_1 = _2_1;
                    this._8_1 = _8_1;
                }
                
                public Inners._2 _2_1 { get; }
                public Inners._8 _8_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _29 : _percentx00ⲻFF
            {
                public _29(Inners._2 _2_1, Inners._9 _9_1)
                {
                    this._2_1 = _2_1;
                    this._9_1 = _9_1;
                }
                
                public Inners._2 _2_1 { get; }
                public Inners._9 _9_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _2A : _percentx00ⲻFF
            {
                public _2A(Inners._2 _2_1, Inners._A _A_1)
                {
                    this._2_1 = _2_1;
                    this._A_1 = _A_1;
                }
                
                public Inners._2 _2_1 { get; }
                public Inners._A _A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _2B : _percentx00ⲻFF
            {
                public _2B(Inners._2 _2_1, Inners._B _B_1)
                {
                    this._2_1 = _2_1;
                    this._B_1 = _B_1;
                }
                
                public Inners._2 _2_1 { get; }
                public Inners._B _B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _2C : _percentx00ⲻFF
            {
                public _2C(Inners._2 _2_1, Inners._C _C_1)
                {
                    this._2_1 = _2_1;
                    this._C_1 = _C_1;
                }
                
                public Inners._2 _2_1 { get; }
                public Inners._C _C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _2D : _percentx00ⲻFF
            {
                public _2D(Inners._2 _2_1, Inners._D _D_1)
                {
                    this._2_1 = _2_1;
                    this._D_1 = _D_1;
                }
                
                public Inners._2 _2_1 { get; }
                public Inners._D _D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _2E : _percentx00ⲻFF
            {
                public _2E(Inners._2 _2_1, Inners._E _E_1)
                {
                    this._2_1 = _2_1;
                    this._E_1 = _E_1;
                }
                
                public Inners._2 _2_1 { get; }
                public Inners._E _E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _2F : _percentx00ⲻFF
            {
                public _2F(Inners._2 _2_1, Inners._F _F_1)
                {
                    this._2_1 = _2_1;
                    this._F_1 = _F_1;
                }
                
                public Inners._2 _2_1 { get; }
                public Inners._F _F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _30 : _percentx00ⲻFF
            {
                public _30(Inners._3 _3_1, Inners._0 _0_1)
                {
                    this._3_1 = _3_1;
                    this._0_1 = _0_1;
                }
                
                public Inners._3 _3_1 { get; }
                public Inners._0 _0_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _31 : _percentx00ⲻFF
            {
                public _31(Inners._3 _3_1, Inners._1 _1_1)
                {
                    this._3_1 = _3_1;
                    this._1_1 = _1_1;
                }
                
                public Inners._3 _3_1 { get; }
                public Inners._1 _1_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _32 : _percentx00ⲻFF
            {
                public _32(Inners._3 _3_1, Inners._2 _2_1)
                {
                    this._3_1 = _3_1;
                    this._2_1 = _2_1;
                }
                
                public Inners._3 _3_1 { get; }
                public Inners._2 _2_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _33 : _percentx00ⲻFF
            {
                public _33(Inners._3 _3_1, Inners._3 _3_2)
                {
                    this._3_1 = _3_1;
                    this._3_2 = _3_2;
                }
                
                public Inners._3 _3_1 { get; }
                public Inners._3 _3_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _34 : _percentx00ⲻFF
            {
                public _34(Inners._3 _3_1, Inners._4 _4_1)
                {
                    this._3_1 = _3_1;
                    this._4_1 = _4_1;
                }
                
                public Inners._3 _3_1 { get; }
                public Inners._4 _4_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _35 : _percentx00ⲻFF
            {
                public _35(Inners._3 _3_1, Inners._5 _5_1)
                {
                    this._3_1 = _3_1;
                    this._5_1 = _5_1;
                }
                
                public Inners._3 _3_1 { get; }
                public Inners._5 _5_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _36 : _percentx00ⲻFF
            {
                public _36(Inners._3 _3_1, Inners._6 _6_1)
                {
                    this._3_1 = _3_1;
                    this._6_1 = _6_1;
                }
                
                public Inners._3 _3_1 { get; }
                public Inners._6 _6_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _37 : _percentx00ⲻFF
            {
                public _37(Inners._3 _3_1, Inners._7 _7_1)
                {
                    this._3_1 = _3_1;
                    this._7_1 = _7_1;
                }
                
                public Inners._3 _3_1 { get; }
                public Inners._7 _7_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _38 : _percentx00ⲻFF
            {
                public _38(Inners._3 _3_1, Inners._8 _8_1)
                {
                    this._3_1 = _3_1;
                    this._8_1 = _8_1;
                }
                
                public Inners._3 _3_1 { get; }
                public Inners._8 _8_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _39 : _percentx00ⲻFF
            {
                public _39(Inners._3 _3_1, Inners._9 _9_1)
                {
                    this._3_1 = _3_1;
                    this._9_1 = _9_1;
                }
                
                public Inners._3 _3_1 { get; }
                public Inners._9 _9_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _3A : _percentx00ⲻFF
            {
                public _3A(Inners._3 _3_1, Inners._A _A_1)
                {
                    this._3_1 = _3_1;
                    this._A_1 = _A_1;
                }
                
                public Inners._3 _3_1 { get; }
                public Inners._A _A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _3B : _percentx00ⲻFF
            {
                public _3B(Inners._3 _3_1, Inners._B _B_1)
                {
                    this._3_1 = _3_1;
                    this._B_1 = _B_1;
                }
                
                public Inners._3 _3_1 { get; }
                public Inners._B _B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _3C : _percentx00ⲻFF
            {
                public _3C(Inners._3 _3_1, Inners._C _C_1)
                {
                    this._3_1 = _3_1;
                    this._C_1 = _C_1;
                }
                
                public Inners._3 _3_1 { get; }
                public Inners._C _C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _3D : _percentx00ⲻFF
            {
                public _3D(Inners._3 _3_1, Inners._D _D_1)
                {
                    this._3_1 = _3_1;
                    this._D_1 = _D_1;
                }
                
                public Inners._3 _3_1 { get; }
                public Inners._D _D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _3E : _percentx00ⲻFF
            {
                public _3E(Inners._3 _3_1, Inners._E _E_1)
                {
                    this._3_1 = _3_1;
                    this._E_1 = _E_1;
                }
                
                public Inners._3 _3_1 { get; }
                public Inners._E _E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _3F : _percentx00ⲻFF
            {
                public _3F(Inners._3 _3_1, Inners._F _F_1)
                {
                    this._3_1 = _3_1;
                    this._F_1 = _F_1;
                }
                
                public Inners._3 _3_1 { get; }
                public Inners._F _F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _40 : _percentx00ⲻFF
            {
                public _40(Inners._4 _4_1, Inners._0 _0_1)
                {
                    this._4_1 = _4_1;
                    this._0_1 = _0_1;
                }
                
                public Inners._4 _4_1 { get; }
                public Inners._0 _0_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _41 : _percentx00ⲻFF
            {
                public _41(Inners._4 _4_1, Inners._1 _1_1)
                {
                    this._4_1 = _4_1;
                    this._1_1 = _1_1;
                }
                
                public Inners._4 _4_1 { get; }
                public Inners._1 _1_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _42 : _percentx00ⲻFF
            {
                public _42(Inners._4 _4_1, Inners._2 _2_1)
                {
                    this._4_1 = _4_1;
                    this._2_1 = _2_1;
                }
                
                public Inners._4 _4_1 { get; }
                public Inners._2 _2_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _43 : _percentx00ⲻFF
            {
                public _43(Inners._4 _4_1, Inners._3 _3_1)
                {
                    this._4_1 = _4_1;
                    this._3_1 = _3_1;
                }
                
                public Inners._4 _4_1 { get; }
                public Inners._3 _3_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _44 : _percentx00ⲻFF
            {
                public _44(Inners._4 _4_1, Inners._4 _4_2)
                {
                    this._4_1 = _4_1;
                    this._4_2 = _4_2;
                }
                
                public Inners._4 _4_1 { get; }
                public Inners._4 _4_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _45 : _percentx00ⲻFF
            {
                public _45(Inners._4 _4_1, Inners._5 _5_1)
                {
                    this._4_1 = _4_1;
                    this._5_1 = _5_1;
                }
                
                public Inners._4 _4_1 { get; }
                public Inners._5 _5_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _46 : _percentx00ⲻFF
            {
                public _46(Inners._4 _4_1, Inners._6 _6_1)
                {
                    this._4_1 = _4_1;
                    this._6_1 = _6_1;
                }
                
                public Inners._4 _4_1 { get; }
                public Inners._6 _6_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _47 : _percentx00ⲻFF
            {
                public _47(Inners._4 _4_1, Inners._7 _7_1)
                {
                    this._4_1 = _4_1;
                    this._7_1 = _7_1;
                }
                
                public Inners._4 _4_1 { get; }
                public Inners._7 _7_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _48 : _percentx00ⲻFF
            {
                public _48(Inners._4 _4_1, Inners._8 _8_1)
                {
                    this._4_1 = _4_1;
                    this._8_1 = _8_1;
                }
                
                public Inners._4 _4_1 { get; }
                public Inners._8 _8_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _49 : _percentx00ⲻFF
            {
                public _49(Inners._4 _4_1, Inners._9 _9_1)
                {
                    this._4_1 = _4_1;
                    this._9_1 = _9_1;
                }
                
                public Inners._4 _4_1 { get; }
                public Inners._9 _9_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _4A : _percentx00ⲻFF
            {
                public _4A(Inners._4 _4_1, Inners._A _A_1)
                {
                    this._4_1 = _4_1;
                    this._A_1 = _A_1;
                }
                
                public Inners._4 _4_1 { get; }
                public Inners._A _A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _4B : _percentx00ⲻFF
            {
                public _4B(Inners._4 _4_1, Inners._B _B_1)
                {
                    this._4_1 = _4_1;
                    this._B_1 = _B_1;
                }
                
                public Inners._4 _4_1 { get; }
                public Inners._B _B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _4C : _percentx00ⲻFF
            {
                public _4C(Inners._4 _4_1, Inners._C _C_1)
                {
                    this._4_1 = _4_1;
                    this._C_1 = _C_1;
                }
                
                public Inners._4 _4_1 { get; }
                public Inners._C _C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _4D : _percentx00ⲻFF
            {
                public _4D(Inners._4 _4_1, Inners._D _D_1)
                {
                    this._4_1 = _4_1;
                    this._D_1 = _D_1;
                }
                
                public Inners._4 _4_1 { get; }
                public Inners._D _D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _4E : _percentx00ⲻFF
            {
                public _4E(Inners._4 _4_1, Inners._E _E_1)
                {
                    this._4_1 = _4_1;
                    this._E_1 = _E_1;
                }
                
                public Inners._4 _4_1 { get; }
                public Inners._E _E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _4F : _percentx00ⲻFF
            {
                public _4F(Inners._4 _4_1, Inners._F _F_1)
                {
                    this._4_1 = _4_1;
                    this._F_1 = _F_1;
                }
                
                public Inners._4 _4_1 { get; }
                public Inners._F _F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _50 : _percentx00ⲻFF
            {
                public _50(Inners._5 _5_1, Inners._0 _0_1)
                {
                    this._5_1 = _5_1;
                    this._0_1 = _0_1;
                }
                
                public Inners._5 _5_1 { get; }
                public Inners._0 _0_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _51 : _percentx00ⲻFF
            {
                public _51(Inners._5 _5_1, Inners._1 _1_1)
                {
                    this._5_1 = _5_1;
                    this._1_1 = _1_1;
                }
                
                public Inners._5 _5_1 { get; }
                public Inners._1 _1_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _52 : _percentx00ⲻFF
            {
                public _52(Inners._5 _5_1, Inners._2 _2_1)
                {
                    this._5_1 = _5_1;
                    this._2_1 = _2_1;
                }
                
                public Inners._5 _5_1 { get; }
                public Inners._2 _2_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _53 : _percentx00ⲻFF
            {
                public _53(Inners._5 _5_1, Inners._3 _3_1)
                {
                    this._5_1 = _5_1;
                    this._3_1 = _3_1;
                }
                
                public Inners._5 _5_1 { get; }
                public Inners._3 _3_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _54 : _percentx00ⲻFF
            {
                public _54(Inners._5 _5_1, Inners._4 _4_1)
                {
                    this._5_1 = _5_1;
                    this._4_1 = _4_1;
                }
                
                public Inners._5 _5_1 { get; }
                public Inners._4 _4_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _55 : _percentx00ⲻFF
            {
                public _55(Inners._5 _5_1, Inners._5 _5_2)
                {
                    this._5_1 = _5_1;
                    this._5_2 = _5_2;
                }
                
                public Inners._5 _5_1 { get; }
                public Inners._5 _5_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _56 : _percentx00ⲻFF
            {
                public _56(Inners._5 _5_1, Inners._6 _6_1)
                {
                    this._5_1 = _5_1;
                    this._6_1 = _6_1;
                }
                
                public Inners._5 _5_1 { get; }
                public Inners._6 _6_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _57 : _percentx00ⲻFF
            {
                public _57(Inners._5 _5_1, Inners._7 _7_1)
                {
                    this._5_1 = _5_1;
                    this._7_1 = _7_1;
                }
                
                public Inners._5 _5_1 { get; }
                public Inners._7 _7_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _58 : _percentx00ⲻFF
            {
                public _58(Inners._5 _5_1, Inners._8 _8_1)
                {
                    this._5_1 = _5_1;
                    this._8_1 = _8_1;
                }
                
                public Inners._5 _5_1 { get; }
                public Inners._8 _8_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _59 : _percentx00ⲻFF
            {
                public _59(Inners._5 _5_1, Inners._9 _9_1)
                {
                    this._5_1 = _5_1;
                    this._9_1 = _9_1;
                }
                
                public Inners._5 _5_1 { get; }
                public Inners._9 _9_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _5A : _percentx00ⲻFF
            {
                public _5A(Inners._5 _5_1, Inners._A _A_1)
                {
                    this._5_1 = _5_1;
                    this._A_1 = _A_1;
                }
                
                public Inners._5 _5_1 { get; }
                public Inners._A _A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _5B : _percentx00ⲻFF
            {
                public _5B(Inners._5 _5_1, Inners._B _B_1)
                {
                    this._5_1 = _5_1;
                    this._B_1 = _B_1;
                }
                
                public Inners._5 _5_1 { get; }
                public Inners._B _B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _5C : _percentx00ⲻFF
            {
                public _5C(Inners._5 _5_1, Inners._C _C_1)
                {
                    this._5_1 = _5_1;
                    this._C_1 = _C_1;
                }
                
                public Inners._5 _5_1 { get; }
                public Inners._C _C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _5D : _percentx00ⲻFF
            {
                public _5D(Inners._5 _5_1, Inners._D _D_1)
                {
                    this._5_1 = _5_1;
                    this._D_1 = _D_1;
                }
                
                public Inners._5 _5_1 { get; }
                public Inners._D _D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _5E : _percentx00ⲻFF
            {
                public _5E(Inners._5 _5_1, Inners._E _E_1)
                {
                    this._5_1 = _5_1;
                    this._E_1 = _E_1;
                }
                
                public Inners._5 _5_1 { get; }
                public Inners._E _E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _5F : _percentx00ⲻFF
            {
                public _5F(Inners._5 _5_1, Inners._F _F_1)
                {
                    this._5_1 = _5_1;
                    this._F_1 = _F_1;
                }
                
                public Inners._5 _5_1 { get; }
                public Inners._F _F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _60 : _percentx00ⲻFF
            {
                public _60(Inners._6 _6_1, Inners._0 _0_1)
                {
                    this._6_1 = _6_1;
                    this._0_1 = _0_1;
                }
                
                public Inners._6 _6_1 { get; }
                public Inners._0 _0_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _61 : _percentx00ⲻFF
            {
                public _61(Inners._6 _6_1, Inners._1 _1_1)
                {
                    this._6_1 = _6_1;
                    this._1_1 = _1_1;
                }
                
                public Inners._6 _6_1 { get; }
                public Inners._1 _1_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _62 : _percentx00ⲻFF
            {
                public _62(Inners._6 _6_1, Inners._2 _2_1)
                {
                    this._6_1 = _6_1;
                    this._2_1 = _2_1;
                }
                
                public Inners._6 _6_1 { get; }
                public Inners._2 _2_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _63 : _percentx00ⲻFF
            {
                public _63(Inners._6 _6_1, Inners._3 _3_1)
                {
                    this._6_1 = _6_1;
                    this._3_1 = _3_1;
                }
                
                public Inners._6 _6_1 { get; }
                public Inners._3 _3_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _64 : _percentx00ⲻFF
            {
                public _64(Inners._6 _6_1, Inners._4 _4_1)
                {
                    this._6_1 = _6_1;
                    this._4_1 = _4_1;
                }
                
                public Inners._6 _6_1 { get; }
                public Inners._4 _4_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _65 : _percentx00ⲻFF
            {
                public _65(Inners._6 _6_1, Inners._5 _5_1)
                {
                    this._6_1 = _6_1;
                    this._5_1 = _5_1;
                }
                
                public Inners._6 _6_1 { get; }
                public Inners._5 _5_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _66 : _percentx00ⲻFF
            {
                public _66(Inners._6 _6_1, Inners._6 _6_2)
                {
                    this._6_1 = _6_1;
                    this._6_2 = _6_2;
                }
                
                public Inners._6 _6_1 { get; }
                public Inners._6 _6_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _67 : _percentx00ⲻFF
            {
                public _67(Inners._6 _6_1, Inners._7 _7_1)
                {
                    this._6_1 = _6_1;
                    this._7_1 = _7_1;
                }
                
                public Inners._6 _6_1 { get; }
                public Inners._7 _7_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _68 : _percentx00ⲻFF
            {
                public _68(Inners._6 _6_1, Inners._8 _8_1)
                {
                    this._6_1 = _6_1;
                    this._8_1 = _8_1;
                }
                
                public Inners._6 _6_1 { get; }
                public Inners._8 _8_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _69 : _percentx00ⲻFF
            {
                public _69(Inners._6 _6_1, Inners._9 _9_1)
                {
                    this._6_1 = _6_1;
                    this._9_1 = _9_1;
                }
                
                public Inners._6 _6_1 { get; }
                public Inners._9 _9_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _6A : _percentx00ⲻFF
            {
                public _6A(Inners._6 _6_1, Inners._A _A_1)
                {
                    this._6_1 = _6_1;
                    this._A_1 = _A_1;
                }
                
                public Inners._6 _6_1 { get; }
                public Inners._A _A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _6B : _percentx00ⲻFF
            {
                public _6B(Inners._6 _6_1, Inners._B _B_1)
                {
                    this._6_1 = _6_1;
                    this._B_1 = _B_1;
                }
                
                public Inners._6 _6_1 { get; }
                public Inners._B _B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _6C : _percentx00ⲻFF
            {
                public _6C(Inners._6 _6_1, Inners._C _C_1)
                {
                    this._6_1 = _6_1;
                    this._C_1 = _C_1;
                }
                
                public Inners._6 _6_1 { get; }
                public Inners._C _C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _6D : _percentx00ⲻFF
            {
                public _6D(Inners._6 _6_1, Inners._D _D_1)
                {
                    this._6_1 = _6_1;
                    this._D_1 = _D_1;
                }
                
                public Inners._6 _6_1 { get; }
                public Inners._D _D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _6E : _percentx00ⲻFF
            {
                public _6E(Inners._6 _6_1, Inners._E _E_1)
                {
                    this._6_1 = _6_1;
                    this._E_1 = _E_1;
                }
                
                public Inners._6 _6_1 { get; }
                public Inners._E _E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _6F : _percentx00ⲻFF
            {
                public _6F(Inners._6 _6_1, Inners._F _F_1)
                {
                    this._6_1 = _6_1;
                    this._F_1 = _F_1;
                }
                
                public Inners._6 _6_1 { get; }
                public Inners._F _F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _70 : _percentx00ⲻFF
            {
                public _70(Inners._7 _7_1, Inners._0 _0_1)
                {
                    this._7_1 = _7_1;
                    this._0_1 = _0_1;
                }
                
                public Inners._7 _7_1 { get; }
                public Inners._0 _0_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _71 : _percentx00ⲻFF
            {
                public _71(Inners._7 _7_1, Inners._1 _1_1)
                {
                    this._7_1 = _7_1;
                    this._1_1 = _1_1;
                }
                
                public Inners._7 _7_1 { get; }
                public Inners._1 _1_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _72 : _percentx00ⲻFF
            {
                public _72(Inners._7 _7_1, Inners._2 _2_1)
                {
                    this._7_1 = _7_1;
                    this._2_1 = _2_1;
                }
                
                public Inners._7 _7_1 { get; }
                public Inners._2 _2_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _73 : _percentx00ⲻFF
            {
                public _73(Inners._7 _7_1, Inners._3 _3_1)
                {
                    this._7_1 = _7_1;
                    this._3_1 = _3_1;
                }
                
                public Inners._7 _7_1 { get; }
                public Inners._3 _3_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _74 : _percentx00ⲻFF
            {
                public _74(Inners._7 _7_1, Inners._4 _4_1)
                {
                    this._7_1 = _7_1;
                    this._4_1 = _4_1;
                }
                
                public Inners._7 _7_1 { get; }
                public Inners._4 _4_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _75 : _percentx00ⲻFF
            {
                public _75(Inners._7 _7_1, Inners._5 _5_1)
                {
                    this._7_1 = _7_1;
                    this._5_1 = _5_1;
                }
                
                public Inners._7 _7_1 { get; }
                public Inners._5 _5_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _76 : _percentx00ⲻFF
            {
                public _76(Inners._7 _7_1, Inners._6 _6_1)
                {
                    this._7_1 = _7_1;
                    this._6_1 = _6_1;
                }
                
                public Inners._7 _7_1 { get; }
                public Inners._6 _6_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _77 : _percentx00ⲻFF
            {
                public _77(Inners._7 _7_1, Inners._7 _7_2)
                {
                    this._7_1 = _7_1;
                    this._7_2 = _7_2;
                }
                
                public Inners._7 _7_1 { get; }
                public Inners._7 _7_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _78 : _percentx00ⲻFF
            {
                public _78(Inners._7 _7_1, Inners._8 _8_1)
                {
                    this._7_1 = _7_1;
                    this._8_1 = _8_1;
                }
                
                public Inners._7 _7_1 { get; }
                public Inners._8 _8_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _79 : _percentx00ⲻFF
            {
                public _79(Inners._7 _7_1, Inners._9 _9_1)
                {
                    this._7_1 = _7_1;
                    this._9_1 = _9_1;
                }
                
                public Inners._7 _7_1 { get; }
                public Inners._9 _9_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _7A : _percentx00ⲻFF
            {
                public _7A(Inners._7 _7_1, Inners._A _A_1)
                {
                    this._7_1 = _7_1;
                    this._A_1 = _A_1;
                }
                
                public Inners._7 _7_1 { get; }
                public Inners._A _A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _7B : _percentx00ⲻFF
            {
                public _7B(Inners._7 _7_1, Inners._B _B_1)
                {
                    this._7_1 = _7_1;
                    this._B_1 = _B_1;
                }
                
                public Inners._7 _7_1 { get; }
                public Inners._B _B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _7C : _percentx00ⲻFF
            {
                public _7C(Inners._7 _7_1, Inners._C _C_1)
                {
                    this._7_1 = _7_1;
                    this._C_1 = _C_1;
                }
                
                public Inners._7 _7_1 { get; }
                public Inners._C _C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _7D : _percentx00ⲻFF
            {
                public _7D(Inners._7 _7_1, Inners._D _D_1)
                {
                    this._7_1 = _7_1;
                    this._D_1 = _D_1;
                }
                
                public Inners._7 _7_1 { get; }
                public Inners._D _D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _7E : _percentx00ⲻFF
            {
                public _7E(Inners._7 _7_1, Inners._E _E_1)
                {
                    this._7_1 = _7_1;
                    this._E_1 = _E_1;
                }
                
                public Inners._7 _7_1 { get; }
                public Inners._E _E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _7F : _percentx00ⲻFF
            {
                public _7F(Inners._7 _7_1, Inners._F _F_1)
                {
                    this._7_1 = _7_1;
                    this._F_1 = _F_1;
                }
                
                public Inners._7 _7_1 { get; }
                public Inners._F _F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _80 : _percentx00ⲻFF
            {
                public _80(Inners._8 _8_1, Inners._0 _0_1)
                {
                    this._8_1 = _8_1;
                    this._0_1 = _0_1;
                }
                
                public Inners._8 _8_1 { get; }
                public Inners._0 _0_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _81 : _percentx00ⲻFF
            {
                public _81(Inners._8 _8_1, Inners._1 _1_1)
                {
                    this._8_1 = _8_1;
                    this._1_1 = _1_1;
                }
                
                public Inners._8 _8_1 { get; }
                public Inners._1 _1_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _82 : _percentx00ⲻFF
            {
                public _82(Inners._8 _8_1, Inners._2 _2_1)
                {
                    this._8_1 = _8_1;
                    this._2_1 = _2_1;
                }
                
                public Inners._8 _8_1 { get; }
                public Inners._2 _2_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _83 : _percentx00ⲻFF
            {
                public _83(Inners._8 _8_1, Inners._3 _3_1)
                {
                    this._8_1 = _8_1;
                    this._3_1 = _3_1;
                }
                
                public Inners._8 _8_1 { get; }
                public Inners._3 _3_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _84 : _percentx00ⲻFF
            {
                public _84(Inners._8 _8_1, Inners._4 _4_1)
                {
                    this._8_1 = _8_1;
                    this._4_1 = _4_1;
                }
                
                public Inners._8 _8_1 { get; }
                public Inners._4 _4_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _85 : _percentx00ⲻFF
            {
                public _85(Inners._8 _8_1, Inners._5 _5_1)
                {
                    this._8_1 = _8_1;
                    this._5_1 = _5_1;
                }
                
                public Inners._8 _8_1 { get; }
                public Inners._5 _5_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _86 : _percentx00ⲻFF
            {
                public _86(Inners._8 _8_1, Inners._6 _6_1)
                {
                    this._8_1 = _8_1;
                    this._6_1 = _6_1;
                }
                
                public Inners._8 _8_1 { get; }
                public Inners._6 _6_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _87 : _percentx00ⲻFF
            {
                public _87(Inners._8 _8_1, Inners._7 _7_1)
                {
                    this._8_1 = _8_1;
                    this._7_1 = _7_1;
                }
                
                public Inners._8 _8_1 { get; }
                public Inners._7 _7_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _88 : _percentx00ⲻFF
            {
                public _88(Inners._8 _8_1, Inners._8 _8_2)
                {
                    this._8_1 = _8_1;
                    this._8_2 = _8_2;
                }
                
                public Inners._8 _8_1 { get; }
                public Inners._8 _8_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _89 : _percentx00ⲻFF
            {
                public _89(Inners._8 _8_1, Inners._9 _9_1)
                {
                    this._8_1 = _8_1;
                    this._9_1 = _9_1;
                }
                
                public Inners._8 _8_1 { get; }
                public Inners._9 _9_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _8A : _percentx00ⲻFF
            {
                public _8A(Inners._8 _8_1, Inners._A _A_1)
                {
                    this._8_1 = _8_1;
                    this._A_1 = _A_1;
                }
                
                public Inners._8 _8_1 { get; }
                public Inners._A _A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _8B : _percentx00ⲻFF
            {
                public _8B(Inners._8 _8_1, Inners._B _B_1)
                {
                    this._8_1 = _8_1;
                    this._B_1 = _B_1;
                }
                
                public Inners._8 _8_1 { get; }
                public Inners._B _B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _8C : _percentx00ⲻFF
            {
                public _8C(Inners._8 _8_1, Inners._C _C_1)
                {
                    this._8_1 = _8_1;
                    this._C_1 = _C_1;
                }
                
                public Inners._8 _8_1 { get; }
                public Inners._C _C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _8D : _percentx00ⲻFF
            {
                public _8D(Inners._8 _8_1, Inners._D _D_1)
                {
                    this._8_1 = _8_1;
                    this._D_1 = _D_1;
                }
                
                public Inners._8 _8_1 { get; }
                public Inners._D _D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _8E : _percentx00ⲻFF
            {
                public _8E(Inners._8 _8_1, Inners._E _E_1)
                {
                    this._8_1 = _8_1;
                    this._E_1 = _E_1;
                }
                
                public Inners._8 _8_1 { get; }
                public Inners._E _E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _8F : _percentx00ⲻFF
            {
                public _8F(Inners._8 _8_1, Inners._F _F_1)
                {
                    this._8_1 = _8_1;
                    this._F_1 = _F_1;
                }
                
                public Inners._8 _8_1 { get; }
                public Inners._F _F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _90 : _percentx00ⲻFF
            {
                public _90(Inners._9 _9_1, Inners._0 _0_1)
                {
                    this._9_1 = _9_1;
                    this._0_1 = _0_1;
                }
                
                public Inners._9 _9_1 { get; }
                public Inners._0 _0_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _91 : _percentx00ⲻFF
            {
                public _91(Inners._9 _9_1, Inners._1 _1_1)
                {
                    this._9_1 = _9_1;
                    this._1_1 = _1_1;
                }
                
                public Inners._9 _9_1 { get; }
                public Inners._1 _1_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _92 : _percentx00ⲻFF
            {
                public _92(Inners._9 _9_1, Inners._2 _2_1)
                {
                    this._9_1 = _9_1;
                    this._2_1 = _2_1;
                }
                
                public Inners._9 _9_1 { get; }
                public Inners._2 _2_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _93 : _percentx00ⲻFF
            {
                public _93(Inners._9 _9_1, Inners._3 _3_1)
                {
                    this._9_1 = _9_1;
                    this._3_1 = _3_1;
                }
                
                public Inners._9 _9_1 { get; }
                public Inners._3 _3_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _94 : _percentx00ⲻFF
            {
                public _94(Inners._9 _9_1, Inners._4 _4_1)
                {
                    this._9_1 = _9_1;
                    this._4_1 = _4_1;
                }
                
                public Inners._9 _9_1 { get; }
                public Inners._4 _4_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _95 : _percentx00ⲻFF
            {
                public _95(Inners._9 _9_1, Inners._5 _5_1)
                {
                    this._9_1 = _9_1;
                    this._5_1 = _5_1;
                }
                
                public Inners._9 _9_1 { get; }
                public Inners._5 _5_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _96 : _percentx00ⲻFF
            {
                public _96(Inners._9 _9_1, Inners._6 _6_1)
                {
                    this._9_1 = _9_1;
                    this._6_1 = _6_1;
                }
                
                public Inners._9 _9_1 { get; }
                public Inners._6 _6_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _97 : _percentx00ⲻFF
            {
                public _97(Inners._9 _9_1, Inners._7 _7_1)
                {
                    this._9_1 = _9_1;
                    this._7_1 = _7_1;
                }
                
                public Inners._9 _9_1 { get; }
                public Inners._7 _7_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _98 : _percentx00ⲻFF
            {
                public _98(Inners._9 _9_1, Inners._8 _8_1)
                {
                    this._9_1 = _9_1;
                    this._8_1 = _8_1;
                }
                
                public Inners._9 _9_1 { get; }
                public Inners._8 _8_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _99 : _percentx00ⲻFF
            {
                public _99(Inners._9 _9_1, Inners._9 _9_2)
                {
                    this._9_1 = _9_1;
                    this._9_2 = _9_2;
                }
                
                public Inners._9 _9_1 { get; }
                public Inners._9 _9_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _9A : _percentx00ⲻFF
            {
                public _9A(Inners._9 _9_1, Inners._A _A_1)
                {
                    this._9_1 = _9_1;
                    this._A_1 = _A_1;
                }
                
                public Inners._9 _9_1 { get; }
                public Inners._A _A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _9B : _percentx00ⲻFF
            {
                public _9B(Inners._9 _9_1, Inners._B _B_1)
                {
                    this._9_1 = _9_1;
                    this._B_1 = _B_1;
                }
                
                public Inners._9 _9_1 { get; }
                public Inners._B _B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _9C : _percentx00ⲻFF
            {
                public _9C(Inners._9 _9_1, Inners._C _C_1)
                {
                    this._9_1 = _9_1;
                    this._C_1 = _C_1;
                }
                
                public Inners._9 _9_1 { get; }
                public Inners._C _C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _9D : _percentx00ⲻFF
            {
                public _9D(Inners._9 _9_1, Inners._D _D_1)
                {
                    this._9_1 = _9_1;
                    this._D_1 = _D_1;
                }
                
                public Inners._9 _9_1 { get; }
                public Inners._D _D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _9E : _percentx00ⲻFF
            {
                public _9E(Inners._9 _9_1, Inners._E _E_1)
                {
                    this._9_1 = _9_1;
                    this._E_1 = _E_1;
                }
                
                public Inners._9 _9_1 { get; }
                public Inners._E _E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _9F : _percentx00ⲻFF
            {
                public _9F(Inners._9 _9_1, Inners._F _F_1)
                {
                    this._9_1 = _9_1;
                    this._F_1 = _F_1;
                }
                
                public Inners._9 _9_1 { get; }
                public Inners._F _F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _A0 : _percentx00ⲻFF
            {
                public _A0(Inners._A _A_1, Inners._0 _0_1)
                {
                    this._A_1 = _A_1;
                    this._0_1 = _0_1;
                }
                
                public Inners._A _A_1 { get; }
                public Inners._0 _0_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _A1 : _percentx00ⲻFF
            {
                public _A1(Inners._A _A_1, Inners._1 _1_1)
                {
                    this._A_1 = _A_1;
                    this._1_1 = _1_1;
                }
                
                public Inners._A _A_1 { get; }
                public Inners._1 _1_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _A2 : _percentx00ⲻFF
            {
                public _A2(Inners._A _A_1, Inners._2 _2_1)
                {
                    this._A_1 = _A_1;
                    this._2_1 = _2_1;
                }
                
                public Inners._A _A_1 { get; }
                public Inners._2 _2_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _A3 : _percentx00ⲻFF
            {
                public _A3(Inners._A _A_1, Inners._3 _3_1)
                {
                    this._A_1 = _A_1;
                    this._3_1 = _3_1;
                }
                
                public Inners._A _A_1 { get; }
                public Inners._3 _3_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _A4 : _percentx00ⲻFF
            {
                public _A4(Inners._A _A_1, Inners._4 _4_1)
                {
                    this._A_1 = _A_1;
                    this._4_1 = _4_1;
                }
                
                public Inners._A _A_1 { get; }
                public Inners._4 _4_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _A5 : _percentx00ⲻFF
            {
                public _A5(Inners._A _A_1, Inners._5 _5_1)
                {
                    this._A_1 = _A_1;
                    this._5_1 = _5_1;
                }
                
                public Inners._A _A_1 { get; }
                public Inners._5 _5_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _A6 : _percentx00ⲻFF
            {
                public _A6(Inners._A _A_1, Inners._6 _6_1)
                {
                    this._A_1 = _A_1;
                    this._6_1 = _6_1;
                }
                
                public Inners._A _A_1 { get; }
                public Inners._6 _6_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _A7 : _percentx00ⲻFF
            {
                public _A7(Inners._A _A_1, Inners._7 _7_1)
                {
                    this._A_1 = _A_1;
                    this._7_1 = _7_1;
                }
                
                public Inners._A _A_1 { get; }
                public Inners._7 _7_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _A8 : _percentx00ⲻFF
            {
                public _A8(Inners._A _A_1, Inners._8 _8_1)
                {
                    this._A_1 = _A_1;
                    this._8_1 = _8_1;
                }
                
                public Inners._A _A_1 { get; }
                public Inners._8 _8_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _A9 : _percentx00ⲻFF
            {
                public _A9(Inners._A _A_1, Inners._9 _9_1)
                {
                    this._A_1 = _A_1;
                    this._9_1 = _9_1;
                }
                
                public Inners._A _A_1 { get; }
                public Inners._9 _9_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _AA : _percentx00ⲻFF
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
            
            public sealed class _AB : _percentx00ⲻFF
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
            
            public sealed class _AC : _percentx00ⲻFF
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
            
            public sealed class _AD : _percentx00ⲻFF
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
            
            public sealed class _AE : _percentx00ⲻFF
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
            
            public sealed class _AF : _percentx00ⲻFF
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
            
            public sealed class _B0 : _percentx00ⲻFF
            {
                public _B0(Inners._B _B_1, Inners._0 _0_1)
                {
                    this._B_1 = _B_1;
                    this._0_1 = _0_1;
                }
                
                public Inners._B _B_1 { get; }
                public Inners._0 _0_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _B1 : _percentx00ⲻFF
            {
                public _B1(Inners._B _B_1, Inners._1 _1_1)
                {
                    this._B_1 = _B_1;
                    this._1_1 = _1_1;
                }
                
                public Inners._B _B_1 { get; }
                public Inners._1 _1_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _B2 : _percentx00ⲻFF
            {
                public _B2(Inners._B _B_1, Inners._2 _2_1)
                {
                    this._B_1 = _B_1;
                    this._2_1 = _2_1;
                }
                
                public Inners._B _B_1 { get; }
                public Inners._2 _2_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _B3 : _percentx00ⲻFF
            {
                public _B3(Inners._B _B_1, Inners._3 _3_1)
                {
                    this._B_1 = _B_1;
                    this._3_1 = _3_1;
                }
                
                public Inners._B _B_1 { get; }
                public Inners._3 _3_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _B4 : _percentx00ⲻFF
            {
                public _B4(Inners._B _B_1, Inners._4 _4_1)
                {
                    this._B_1 = _B_1;
                    this._4_1 = _4_1;
                }
                
                public Inners._B _B_1 { get; }
                public Inners._4 _4_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _B5 : _percentx00ⲻFF
            {
                public _B5(Inners._B _B_1, Inners._5 _5_1)
                {
                    this._B_1 = _B_1;
                    this._5_1 = _5_1;
                }
                
                public Inners._B _B_1 { get; }
                public Inners._5 _5_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _B6 : _percentx00ⲻFF
            {
                public _B6(Inners._B _B_1, Inners._6 _6_1)
                {
                    this._B_1 = _B_1;
                    this._6_1 = _6_1;
                }
                
                public Inners._B _B_1 { get; }
                public Inners._6 _6_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _B7 : _percentx00ⲻFF
            {
                public _B7(Inners._B _B_1, Inners._7 _7_1)
                {
                    this._B_1 = _B_1;
                    this._7_1 = _7_1;
                }
                
                public Inners._B _B_1 { get; }
                public Inners._7 _7_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _B8 : _percentx00ⲻFF
            {
                public _B8(Inners._B _B_1, Inners._8 _8_1)
                {
                    this._B_1 = _B_1;
                    this._8_1 = _8_1;
                }
                
                public Inners._B _B_1 { get; }
                public Inners._8 _8_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _B9 : _percentx00ⲻFF
            {
                public _B9(Inners._B _B_1, Inners._9 _9_1)
                {
                    this._B_1 = _B_1;
                    this._9_1 = _9_1;
                }
                
                public Inners._B _B_1 { get; }
                public Inners._9 _9_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _BA : _percentx00ⲻFF
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
            
            public sealed class _BB : _percentx00ⲻFF
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
            
            public sealed class _BC : _percentx00ⲻFF
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
            
            public sealed class _BD : _percentx00ⲻFF
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
            
            public sealed class _BE : _percentx00ⲻFF
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
            
            public sealed class _BF : _percentx00ⲻFF
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
            
            public sealed class _C0 : _percentx00ⲻFF
            {
                public _C0(Inners._C _C_1, Inners._0 _0_1)
                {
                    this._C_1 = _C_1;
                    this._0_1 = _0_1;
                }
                
                public Inners._C _C_1 { get; }
                public Inners._0 _0_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _C1 : _percentx00ⲻFF
            {
                public _C1(Inners._C _C_1, Inners._1 _1_1)
                {
                    this._C_1 = _C_1;
                    this._1_1 = _1_1;
                }
                
                public Inners._C _C_1 { get; }
                public Inners._1 _1_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _C2 : _percentx00ⲻFF
            {
                public _C2(Inners._C _C_1, Inners._2 _2_1)
                {
                    this._C_1 = _C_1;
                    this._2_1 = _2_1;
                }
                
                public Inners._C _C_1 { get; }
                public Inners._2 _2_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _C3 : _percentx00ⲻFF
            {
                public _C3(Inners._C _C_1, Inners._3 _3_1)
                {
                    this._C_1 = _C_1;
                    this._3_1 = _3_1;
                }
                
                public Inners._C _C_1 { get; }
                public Inners._3 _3_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _C4 : _percentx00ⲻFF
            {
                public _C4(Inners._C _C_1, Inners._4 _4_1)
                {
                    this._C_1 = _C_1;
                    this._4_1 = _4_1;
                }
                
                public Inners._C _C_1 { get; }
                public Inners._4 _4_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _C5 : _percentx00ⲻFF
            {
                public _C5(Inners._C _C_1, Inners._5 _5_1)
                {
                    this._C_1 = _C_1;
                    this._5_1 = _5_1;
                }
                
                public Inners._C _C_1 { get; }
                public Inners._5 _5_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _C6 : _percentx00ⲻFF
            {
                public _C6(Inners._C _C_1, Inners._6 _6_1)
                {
                    this._C_1 = _C_1;
                    this._6_1 = _6_1;
                }
                
                public Inners._C _C_1 { get; }
                public Inners._6 _6_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _C7 : _percentx00ⲻFF
            {
                public _C7(Inners._C _C_1, Inners._7 _7_1)
                {
                    this._C_1 = _C_1;
                    this._7_1 = _7_1;
                }
                
                public Inners._C _C_1 { get; }
                public Inners._7 _7_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _C8 : _percentx00ⲻFF
            {
                public _C8(Inners._C _C_1, Inners._8 _8_1)
                {
                    this._C_1 = _C_1;
                    this._8_1 = _8_1;
                }
                
                public Inners._C _C_1 { get; }
                public Inners._8 _8_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _C9 : _percentx00ⲻFF
            {
                public _C9(Inners._C _C_1, Inners._9 _9_1)
                {
                    this._C_1 = _C_1;
                    this._9_1 = _9_1;
                }
                
                public Inners._C _C_1 { get; }
                public Inners._9 _9_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _CA : _percentx00ⲻFF
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
            
            public sealed class _CB : _percentx00ⲻFF
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
            
            public sealed class _CC : _percentx00ⲻFF
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
            
            public sealed class _CD : _percentx00ⲻFF
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
            
            public sealed class _CE : _percentx00ⲻFF
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
            
            public sealed class _CF : _percentx00ⲻFF
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
            
            public sealed class _D0 : _percentx00ⲻFF
            {
                public _D0(Inners._D _D_1, Inners._0 _0_1)
                {
                    this._D_1 = _D_1;
                    this._0_1 = _0_1;
                }
                
                public Inners._D _D_1 { get; }
                public Inners._0 _0_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _D1 : _percentx00ⲻFF
            {
                public _D1(Inners._D _D_1, Inners._1 _1_1)
                {
                    this._D_1 = _D_1;
                    this._1_1 = _1_1;
                }
                
                public Inners._D _D_1 { get; }
                public Inners._1 _1_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _D2 : _percentx00ⲻFF
            {
                public _D2(Inners._D _D_1, Inners._2 _2_1)
                {
                    this._D_1 = _D_1;
                    this._2_1 = _2_1;
                }
                
                public Inners._D _D_1 { get; }
                public Inners._2 _2_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _D3 : _percentx00ⲻFF
            {
                public _D3(Inners._D _D_1, Inners._3 _3_1)
                {
                    this._D_1 = _D_1;
                    this._3_1 = _3_1;
                }
                
                public Inners._D _D_1 { get; }
                public Inners._3 _3_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _D4 : _percentx00ⲻFF
            {
                public _D4(Inners._D _D_1, Inners._4 _4_1)
                {
                    this._D_1 = _D_1;
                    this._4_1 = _4_1;
                }
                
                public Inners._D _D_1 { get; }
                public Inners._4 _4_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _D5 : _percentx00ⲻFF
            {
                public _D5(Inners._D _D_1, Inners._5 _5_1)
                {
                    this._D_1 = _D_1;
                    this._5_1 = _5_1;
                }
                
                public Inners._D _D_1 { get; }
                public Inners._5 _5_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _D6 : _percentx00ⲻFF
            {
                public _D6(Inners._D _D_1, Inners._6 _6_1)
                {
                    this._D_1 = _D_1;
                    this._6_1 = _6_1;
                }
                
                public Inners._D _D_1 { get; }
                public Inners._6 _6_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _D7 : _percentx00ⲻFF
            {
                public _D7(Inners._D _D_1, Inners._7 _7_1)
                {
                    this._D_1 = _D_1;
                    this._7_1 = _7_1;
                }
                
                public Inners._D _D_1 { get; }
                public Inners._7 _7_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _D8 : _percentx00ⲻFF
            {
                public _D8(Inners._D _D_1, Inners._8 _8_1)
                {
                    this._D_1 = _D_1;
                    this._8_1 = _8_1;
                }
                
                public Inners._D _D_1 { get; }
                public Inners._8 _8_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _D9 : _percentx00ⲻFF
            {
                public _D9(Inners._D _D_1, Inners._9 _9_1)
                {
                    this._D_1 = _D_1;
                    this._9_1 = _9_1;
                }
                
                public Inners._D _D_1 { get; }
                public Inners._9 _9_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _DA : _percentx00ⲻFF
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
            
            public sealed class _DB : _percentx00ⲻFF
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
            
            public sealed class _DC : _percentx00ⲻFF
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
            
            public sealed class _DD : _percentx00ⲻFF
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
            
            public sealed class _DE : _percentx00ⲻFF
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
            
            public sealed class _DF : _percentx00ⲻFF
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
            
            public sealed class _E0 : _percentx00ⲻFF
            {
                public _E0(Inners._E _E_1, Inners._0 _0_1)
                {
                    this._E_1 = _E_1;
                    this._0_1 = _0_1;
                }
                
                public Inners._E _E_1 { get; }
                public Inners._0 _0_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _E1 : _percentx00ⲻFF
            {
                public _E1(Inners._E _E_1, Inners._1 _1_1)
                {
                    this._E_1 = _E_1;
                    this._1_1 = _1_1;
                }
                
                public Inners._E _E_1 { get; }
                public Inners._1 _1_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _E2 : _percentx00ⲻFF
            {
                public _E2(Inners._E _E_1, Inners._2 _2_1)
                {
                    this._E_1 = _E_1;
                    this._2_1 = _2_1;
                }
                
                public Inners._E _E_1 { get; }
                public Inners._2 _2_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _E3 : _percentx00ⲻFF
            {
                public _E3(Inners._E _E_1, Inners._3 _3_1)
                {
                    this._E_1 = _E_1;
                    this._3_1 = _3_1;
                }
                
                public Inners._E _E_1 { get; }
                public Inners._3 _3_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _E4 : _percentx00ⲻFF
            {
                public _E4(Inners._E _E_1, Inners._4 _4_1)
                {
                    this._E_1 = _E_1;
                    this._4_1 = _4_1;
                }
                
                public Inners._E _E_1 { get; }
                public Inners._4 _4_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _E5 : _percentx00ⲻFF
            {
                public _E5(Inners._E _E_1, Inners._5 _5_1)
                {
                    this._E_1 = _E_1;
                    this._5_1 = _5_1;
                }
                
                public Inners._E _E_1 { get; }
                public Inners._5 _5_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _E6 : _percentx00ⲻFF
            {
                public _E6(Inners._E _E_1, Inners._6 _6_1)
                {
                    this._E_1 = _E_1;
                    this._6_1 = _6_1;
                }
                
                public Inners._E _E_1 { get; }
                public Inners._6 _6_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _E7 : _percentx00ⲻFF
            {
                public _E7(Inners._E _E_1, Inners._7 _7_1)
                {
                    this._E_1 = _E_1;
                    this._7_1 = _7_1;
                }
                
                public Inners._E _E_1 { get; }
                public Inners._7 _7_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _E8 : _percentx00ⲻFF
            {
                public _E8(Inners._E _E_1, Inners._8 _8_1)
                {
                    this._E_1 = _E_1;
                    this._8_1 = _8_1;
                }
                
                public Inners._E _E_1 { get; }
                public Inners._8 _8_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _E9 : _percentx00ⲻFF
            {
                public _E9(Inners._E _E_1, Inners._9 _9_1)
                {
                    this._E_1 = _E_1;
                    this._9_1 = _9_1;
                }
                
                public Inners._E _E_1 { get; }
                public Inners._9 _9_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _EA : _percentx00ⲻFF
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
            
            public sealed class _EB : _percentx00ⲻFF
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
            
            public sealed class _EC : _percentx00ⲻFF
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
            
            public sealed class _ED : _percentx00ⲻFF
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
            
            public sealed class _EE : _percentx00ⲻFF
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
            
            public sealed class _EF : _percentx00ⲻFF
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
            
            public sealed class _F0 : _percentx00ⲻFF
            {
                public _F0(Inners._F _F_1, Inners._0 _0_1)
                {
                    this._F_1 = _F_1;
                    this._0_1 = _0_1;
                }
                
                public Inners._F _F_1 { get; }
                public Inners._0 _0_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _F1 : _percentx00ⲻFF
            {
                public _F1(Inners._F _F_1, Inners._1 _1_1)
                {
                    this._F_1 = _F_1;
                    this._1_1 = _1_1;
                }
                
                public Inners._F _F_1 { get; }
                public Inners._1 _1_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _F2 : _percentx00ⲻFF
            {
                public _F2(Inners._F _F_1, Inners._2 _2_1)
                {
                    this._F_1 = _F_1;
                    this._2_1 = _2_1;
                }
                
                public Inners._F _F_1 { get; }
                public Inners._2 _2_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _F3 : _percentx00ⲻFF
            {
                public _F3(Inners._F _F_1, Inners._3 _3_1)
                {
                    this._F_1 = _F_1;
                    this._3_1 = _3_1;
                }
                
                public Inners._F _F_1 { get; }
                public Inners._3 _3_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _F4 : _percentx00ⲻFF
            {
                public _F4(Inners._F _F_1, Inners._4 _4_1)
                {
                    this._F_1 = _F_1;
                    this._4_1 = _4_1;
                }
                
                public Inners._F _F_1 { get; }
                public Inners._4 _4_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _F5 : _percentx00ⲻFF
            {
                public _F5(Inners._F _F_1, Inners._5 _5_1)
                {
                    this._F_1 = _F_1;
                    this._5_1 = _5_1;
                }
                
                public Inners._F _F_1 { get; }
                public Inners._5 _5_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _F6 : _percentx00ⲻFF
            {
                public _F6(Inners._F _F_1, Inners._6 _6_1)
                {
                    this._F_1 = _F_1;
                    this._6_1 = _6_1;
                }
                
                public Inners._F _F_1 { get; }
                public Inners._6 _6_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _F7 : _percentx00ⲻFF
            {
                public _F7(Inners._F _F_1, Inners._7 _7_1)
                {
                    this._F_1 = _F_1;
                    this._7_1 = _7_1;
                }
                
                public Inners._F _F_1 { get; }
                public Inners._7 _7_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _F8 : _percentx00ⲻFF
            {
                public _F8(Inners._F _F_1, Inners._8 _8_1)
                {
                    this._F_1 = _F_1;
                    this._8_1 = _8_1;
                }
                
                public Inners._F _F_1 { get; }
                public Inners._8 _8_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _F9 : _percentx00ⲻFF
            {
                public _F9(Inners._F _F_1, Inners._9 _9_1)
                {
                    this._F_1 = _F_1;
                    this._9_1 = _9_1;
                }
                
                public Inners._F _F_1 { get; }
                public Inners._9 _9_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FA : _percentx00ⲻFF
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
            
            public sealed class _FB : _percentx00ⲻFF
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
            
            public sealed class _FC : _percentx00ⲻFF
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
            
            public sealed class _FD : _percentx00ⲻFF
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
            
            public sealed class _FE : _percentx00ⲻFF
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
            
            public sealed class _FF : _percentx00ⲻFF
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
        
        public sealed class _percentx20
        {
            public _percentx20(Inners._2 _2_1, Inners._0 _0_1)
            {
                this._2_1 = _2_1;
                this._0_1 = _0_1;
            }
            
            public Inners._2 _2_1 { get; }
            public Inners._0 _0_1 { get; }
        }
        
        public abstract class _percentx21ⲻ7E
        {
            private _percentx21ⲻ7E()
            {
            }
            
            protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
            
            public abstract class Visitor<TResult, TContext>
            {
                public TResult Visit(_percentx21ⲻ7E node, TContext context)
                {
                    return node.Dispatch(this, context);
                }
                
                protected internal abstract TResult Accept(_percentx21ⲻ7E._21 node, TContext context);
                protected internal abstract TResult Accept(_percentx21ⲻ7E._22 node, TContext context);
                protected internal abstract TResult Accept(_percentx21ⲻ7E._23 node, TContext context);
                protected internal abstract TResult Accept(_percentx21ⲻ7E._24 node, TContext context);
                protected internal abstract TResult Accept(_percentx21ⲻ7E._25 node, TContext context);
                protected internal abstract TResult Accept(_percentx21ⲻ7E._26 node, TContext context);
                protected internal abstract TResult Accept(_percentx21ⲻ7E._27 node, TContext context);
                protected internal abstract TResult Accept(_percentx21ⲻ7E._28 node, TContext context);
                protected internal abstract TResult Accept(_percentx21ⲻ7E._29 node, TContext context);
                protected internal abstract TResult Accept(_percentx21ⲻ7E._2A node, TContext context);
                protected internal abstract TResult Accept(_percentx21ⲻ7E._2B node, TContext context);
                protected internal abstract TResult Accept(_percentx21ⲻ7E._2C node, TContext context);
                protected internal abstract TResult Accept(_percentx21ⲻ7E._2D node, TContext context);
                protected internal abstract TResult Accept(_percentx21ⲻ7E._2E node, TContext context);
                protected internal abstract TResult Accept(_percentx21ⲻ7E._2F node, TContext context);
                protected internal abstract TResult Accept(_percentx21ⲻ7E._30 node, TContext context);
                protected internal abstract TResult Accept(_percentx21ⲻ7E._31 node, TContext context);
                protected internal abstract TResult Accept(_percentx21ⲻ7E._32 node, TContext context);
                protected internal abstract TResult Accept(_percentx21ⲻ7E._33 node, TContext context);
                protected internal abstract TResult Accept(_percentx21ⲻ7E._34 node, TContext context);
                protected internal abstract TResult Accept(_percentx21ⲻ7E._35 node, TContext context);
                protected internal abstract TResult Accept(_percentx21ⲻ7E._36 node, TContext context);
                protected internal abstract TResult Accept(_percentx21ⲻ7E._37 node, TContext context);
                protected internal abstract TResult Accept(_percentx21ⲻ7E._38 node, TContext context);
                protected internal abstract TResult Accept(_percentx21ⲻ7E._39 node, TContext context);
                protected internal abstract TResult Accept(_percentx21ⲻ7E._3A node, TContext context);
                protected internal abstract TResult Accept(_percentx21ⲻ7E._3B node, TContext context);
                protected internal abstract TResult Accept(_percentx21ⲻ7E._3C node, TContext context);
                protected internal abstract TResult Accept(_percentx21ⲻ7E._3D node, TContext context);
                protected internal abstract TResult Accept(_percentx21ⲻ7E._3E node, TContext context);
                protected internal abstract TResult Accept(_percentx21ⲻ7E._3F node, TContext context);
                protected internal abstract TResult Accept(_percentx21ⲻ7E._40 node, TContext context);
                protected internal abstract TResult Accept(_percentx21ⲻ7E._41 node, TContext context);
                protected internal abstract TResult Accept(_percentx21ⲻ7E._42 node, TContext context);
                protected internal abstract TResult Accept(_percentx21ⲻ7E._43 node, TContext context);
                protected internal abstract TResult Accept(_percentx21ⲻ7E._44 node, TContext context);
                protected internal abstract TResult Accept(_percentx21ⲻ7E._45 node, TContext context);
                protected internal abstract TResult Accept(_percentx21ⲻ7E._46 node, TContext context);
                protected internal abstract TResult Accept(_percentx21ⲻ7E._47 node, TContext context);
                protected internal abstract TResult Accept(_percentx21ⲻ7E._48 node, TContext context);
                protected internal abstract TResult Accept(_percentx21ⲻ7E._49 node, TContext context);
                protected internal abstract TResult Accept(_percentx21ⲻ7E._4A node, TContext context);
                protected internal abstract TResult Accept(_percentx21ⲻ7E._4B node, TContext context);
                protected internal abstract TResult Accept(_percentx21ⲻ7E._4C node, TContext context);
                protected internal abstract TResult Accept(_percentx21ⲻ7E._4D node, TContext context);
                protected internal abstract TResult Accept(_percentx21ⲻ7E._4E node, TContext context);
                protected internal abstract TResult Accept(_percentx21ⲻ7E._4F node, TContext context);
                protected internal abstract TResult Accept(_percentx21ⲻ7E._50 node, TContext context);
                protected internal abstract TResult Accept(_percentx21ⲻ7E._51 node, TContext context);
                protected internal abstract TResult Accept(_percentx21ⲻ7E._52 node, TContext context);
                protected internal abstract TResult Accept(_percentx21ⲻ7E._53 node, TContext context);
                protected internal abstract TResult Accept(_percentx21ⲻ7E._54 node, TContext context);
                protected internal abstract TResult Accept(_percentx21ⲻ7E._55 node, TContext context);
                protected internal abstract TResult Accept(_percentx21ⲻ7E._56 node, TContext context);
                protected internal abstract TResult Accept(_percentx21ⲻ7E._57 node, TContext context);
                protected internal abstract TResult Accept(_percentx21ⲻ7E._58 node, TContext context);
                protected internal abstract TResult Accept(_percentx21ⲻ7E._59 node, TContext context);
                protected internal abstract TResult Accept(_percentx21ⲻ7E._5A node, TContext context);
                protected internal abstract TResult Accept(_percentx21ⲻ7E._5B node, TContext context);
                protected internal abstract TResult Accept(_percentx21ⲻ7E._5C node, TContext context);
                protected internal abstract TResult Accept(_percentx21ⲻ7E._5D node, TContext context);
                protected internal abstract TResult Accept(_percentx21ⲻ7E._5E node, TContext context);
                protected internal abstract TResult Accept(_percentx21ⲻ7E._5F node, TContext context);
                protected internal abstract TResult Accept(_percentx21ⲻ7E._60 node, TContext context);
                protected internal abstract TResult Accept(_percentx21ⲻ7E._61 node, TContext context);
                protected internal abstract TResult Accept(_percentx21ⲻ7E._62 node, TContext context);
                protected internal abstract TResult Accept(_percentx21ⲻ7E._63 node, TContext context);
                protected internal abstract TResult Accept(_percentx21ⲻ7E._64 node, TContext context);
                protected internal abstract TResult Accept(_percentx21ⲻ7E._65 node, TContext context);
                protected internal abstract TResult Accept(_percentx21ⲻ7E._66 node, TContext context);
                protected internal abstract TResult Accept(_percentx21ⲻ7E._67 node, TContext context);
                protected internal abstract TResult Accept(_percentx21ⲻ7E._68 node, TContext context);
                protected internal abstract TResult Accept(_percentx21ⲻ7E._69 node, TContext context);
                protected internal abstract TResult Accept(_percentx21ⲻ7E._6A node, TContext context);
                protected internal abstract TResult Accept(_percentx21ⲻ7E._6B node, TContext context);
                protected internal abstract TResult Accept(_percentx21ⲻ7E._6C node, TContext context);
                protected internal abstract TResult Accept(_percentx21ⲻ7E._6D node, TContext context);
                protected internal abstract TResult Accept(_percentx21ⲻ7E._6E node, TContext context);
                protected internal abstract TResult Accept(_percentx21ⲻ7E._6F node, TContext context);
                protected internal abstract TResult Accept(_percentx21ⲻ7E._70 node, TContext context);
                protected internal abstract TResult Accept(_percentx21ⲻ7E._71 node, TContext context);
                protected internal abstract TResult Accept(_percentx21ⲻ7E._72 node, TContext context);
                protected internal abstract TResult Accept(_percentx21ⲻ7E._73 node, TContext context);
                protected internal abstract TResult Accept(_percentx21ⲻ7E._74 node, TContext context);
                protected internal abstract TResult Accept(_percentx21ⲻ7E._75 node, TContext context);
                protected internal abstract TResult Accept(_percentx21ⲻ7E._76 node, TContext context);
                protected internal abstract TResult Accept(_percentx21ⲻ7E._77 node, TContext context);
                protected internal abstract TResult Accept(_percentx21ⲻ7E._78 node, TContext context);
                protected internal abstract TResult Accept(_percentx21ⲻ7E._79 node, TContext context);
                protected internal abstract TResult Accept(_percentx21ⲻ7E._7A node, TContext context);
                protected internal abstract TResult Accept(_percentx21ⲻ7E._7B node, TContext context);
                protected internal abstract TResult Accept(_percentx21ⲻ7E._7C node, TContext context);
                protected internal abstract TResult Accept(_percentx21ⲻ7E._7D node, TContext context);
                protected internal abstract TResult Accept(_percentx21ⲻ7E._7E node, TContext context);
            }
            
            public sealed class _21 : _percentx21ⲻ7E
            {
                public _21(Inners._2 _2_1, Inners._1 _1_1)
                {
                    this._2_1 = _2_1;
                    this._1_1 = _1_1;
                }
                
                public Inners._2 _2_1 { get; }
                public Inners._1 _1_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _22 : _percentx21ⲻ7E
            {
                public _22(Inners._2 _2_1, Inners._2 _2_2)
                {
                    this._2_1 = _2_1;
                    this._2_2 = _2_2;
                }
                
                public Inners._2 _2_1 { get; }
                public Inners._2 _2_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _23 : _percentx21ⲻ7E
            {
                public _23(Inners._2 _2_1, Inners._3 _3_1)
                {
                    this._2_1 = _2_1;
                    this._3_1 = _3_1;
                }
                
                public Inners._2 _2_1 { get; }
                public Inners._3 _3_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _24 : _percentx21ⲻ7E
            {
                public _24(Inners._2 _2_1, Inners._4 _4_1)
                {
                    this._2_1 = _2_1;
                    this._4_1 = _4_1;
                }
                
                public Inners._2 _2_1 { get; }
                public Inners._4 _4_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _25 : _percentx21ⲻ7E
            {
                public _25(Inners._2 _2_1, Inners._5 _5_1)
                {
                    this._2_1 = _2_1;
                    this._5_1 = _5_1;
                }
                
                public Inners._2 _2_1 { get; }
                public Inners._5 _5_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _26 : _percentx21ⲻ7E
            {
                public _26(Inners._2 _2_1, Inners._6 _6_1)
                {
                    this._2_1 = _2_1;
                    this._6_1 = _6_1;
                }
                
                public Inners._2 _2_1 { get; }
                public Inners._6 _6_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _27 : _percentx21ⲻ7E
            {
                public _27(Inners._2 _2_1, Inners._7 _7_1)
                {
                    this._2_1 = _2_1;
                    this._7_1 = _7_1;
                }
                
                public Inners._2 _2_1 { get; }
                public Inners._7 _7_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _28 : _percentx21ⲻ7E
            {
                public _28(Inners._2 _2_1, Inners._8 _8_1)
                {
                    this._2_1 = _2_1;
                    this._8_1 = _8_1;
                }
                
                public Inners._2 _2_1 { get; }
                public Inners._8 _8_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _29 : _percentx21ⲻ7E
            {
                public _29(Inners._2 _2_1, Inners._9 _9_1)
                {
                    this._2_1 = _2_1;
                    this._9_1 = _9_1;
                }
                
                public Inners._2 _2_1 { get; }
                public Inners._9 _9_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _2A : _percentx21ⲻ7E
            {
                public _2A(Inners._2 _2_1, Inners._A _A_1)
                {
                    this._2_1 = _2_1;
                    this._A_1 = _A_1;
                }
                
                public Inners._2 _2_1 { get; }
                public Inners._A _A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _2B : _percentx21ⲻ7E
            {
                public _2B(Inners._2 _2_1, Inners._B _B_1)
                {
                    this._2_1 = _2_1;
                    this._B_1 = _B_1;
                }
                
                public Inners._2 _2_1 { get; }
                public Inners._B _B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _2C : _percentx21ⲻ7E
            {
                public _2C(Inners._2 _2_1, Inners._C _C_1)
                {
                    this._2_1 = _2_1;
                    this._C_1 = _C_1;
                }
                
                public Inners._2 _2_1 { get; }
                public Inners._C _C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _2D : _percentx21ⲻ7E
            {
                public _2D(Inners._2 _2_1, Inners._D _D_1)
                {
                    this._2_1 = _2_1;
                    this._D_1 = _D_1;
                }
                
                public Inners._2 _2_1 { get; }
                public Inners._D _D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _2E : _percentx21ⲻ7E
            {
                public _2E(Inners._2 _2_1, Inners._E _E_1)
                {
                    this._2_1 = _2_1;
                    this._E_1 = _E_1;
                }
                
                public Inners._2 _2_1 { get; }
                public Inners._E _E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _2F : _percentx21ⲻ7E
            {
                public _2F(Inners._2 _2_1, Inners._F _F_1)
                {
                    this._2_1 = _2_1;
                    this._F_1 = _F_1;
                }
                
                public Inners._2 _2_1 { get; }
                public Inners._F _F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _30 : _percentx21ⲻ7E
            {
                public _30(Inners._3 _3_1, Inners._0 _0_1)
                {
                    this._3_1 = _3_1;
                    this._0_1 = _0_1;
                }
                
                public Inners._3 _3_1 { get; }
                public Inners._0 _0_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _31 : _percentx21ⲻ7E
            {
                public _31(Inners._3 _3_1, Inners._1 _1_1)
                {
                    this._3_1 = _3_1;
                    this._1_1 = _1_1;
                }
                
                public Inners._3 _3_1 { get; }
                public Inners._1 _1_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _32 : _percentx21ⲻ7E
            {
                public _32(Inners._3 _3_1, Inners._2 _2_1)
                {
                    this._3_1 = _3_1;
                    this._2_1 = _2_1;
                }
                
                public Inners._3 _3_1 { get; }
                public Inners._2 _2_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _33 : _percentx21ⲻ7E
            {
                public _33(Inners._3 _3_1, Inners._3 _3_2)
                {
                    this._3_1 = _3_1;
                    this._3_2 = _3_2;
                }
                
                public Inners._3 _3_1 { get; }
                public Inners._3 _3_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _34 : _percentx21ⲻ7E
            {
                public _34(Inners._3 _3_1, Inners._4 _4_1)
                {
                    this._3_1 = _3_1;
                    this._4_1 = _4_1;
                }
                
                public Inners._3 _3_1 { get; }
                public Inners._4 _4_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _35 : _percentx21ⲻ7E
            {
                public _35(Inners._3 _3_1, Inners._5 _5_1)
                {
                    this._3_1 = _3_1;
                    this._5_1 = _5_1;
                }
                
                public Inners._3 _3_1 { get; }
                public Inners._5 _5_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _36 : _percentx21ⲻ7E
            {
                public _36(Inners._3 _3_1, Inners._6 _6_1)
                {
                    this._3_1 = _3_1;
                    this._6_1 = _6_1;
                }
                
                public Inners._3 _3_1 { get; }
                public Inners._6 _6_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _37 : _percentx21ⲻ7E
            {
                public _37(Inners._3 _3_1, Inners._7 _7_1)
                {
                    this._3_1 = _3_1;
                    this._7_1 = _7_1;
                }
                
                public Inners._3 _3_1 { get; }
                public Inners._7 _7_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _38 : _percentx21ⲻ7E
            {
                public _38(Inners._3 _3_1, Inners._8 _8_1)
                {
                    this._3_1 = _3_1;
                    this._8_1 = _8_1;
                }
                
                public Inners._3 _3_1 { get; }
                public Inners._8 _8_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _39 : _percentx21ⲻ7E
            {
                public _39(Inners._3 _3_1, Inners._9 _9_1)
                {
                    this._3_1 = _3_1;
                    this._9_1 = _9_1;
                }
                
                public Inners._3 _3_1 { get; }
                public Inners._9 _9_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _3A : _percentx21ⲻ7E
            {
                public _3A(Inners._3 _3_1, Inners._A _A_1)
                {
                    this._3_1 = _3_1;
                    this._A_1 = _A_1;
                }
                
                public Inners._3 _3_1 { get; }
                public Inners._A _A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _3B : _percentx21ⲻ7E
            {
                public _3B(Inners._3 _3_1, Inners._B _B_1)
                {
                    this._3_1 = _3_1;
                    this._B_1 = _B_1;
                }
                
                public Inners._3 _3_1 { get; }
                public Inners._B _B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _3C : _percentx21ⲻ7E
            {
                public _3C(Inners._3 _3_1, Inners._C _C_1)
                {
                    this._3_1 = _3_1;
                    this._C_1 = _C_1;
                }
                
                public Inners._3 _3_1 { get; }
                public Inners._C _C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _3D : _percentx21ⲻ7E
            {
                public _3D(Inners._3 _3_1, Inners._D _D_1)
                {
                    this._3_1 = _3_1;
                    this._D_1 = _D_1;
                }
                
                public Inners._3 _3_1 { get; }
                public Inners._D _D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _3E : _percentx21ⲻ7E
            {
                public _3E(Inners._3 _3_1, Inners._E _E_1)
                {
                    this._3_1 = _3_1;
                    this._E_1 = _E_1;
                }
                
                public Inners._3 _3_1 { get; }
                public Inners._E _E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _3F : _percentx21ⲻ7E
            {
                public _3F(Inners._3 _3_1, Inners._F _F_1)
                {
                    this._3_1 = _3_1;
                    this._F_1 = _F_1;
                }
                
                public Inners._3 _3_1 { get; }
                public Inners._F _F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _40 : _percentx21ⲻ7E
            {
                public _40(Inners._4 _4_1, Inners._0 _0_1)
                {
                    this._4_1 = _4_1;
                    this._0_1 = _0_1;
                }
                
                public Inners._4 _4_1 { get; }
                public Inners._0 _0_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _41 : _percentx21ⲻ7E
            {
                public _41(Inners._4 _4_1, Inners._1 _1_1)
                {
                    this._4_1 = _4_1;
                    this._1_1 = _1_1;
                }
                
                public Inners._4 _4_1 { get; }
                public Inners._1 _1_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _42 : _percentx21ⲻ7E
            {
                public _42(Inners._4 _4_1, Inners._2 _2_1)
                {
                    this._4_1 = _4_1;
                    this._2_1 = _2_1;
                }
                
                public Inners._4 _4_1 { get; }
                public Inners._2 _2_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _43 : _percentx21ⲻ7E
            {
                public _43(Inners._4 _4_1, Inners._3 _3_1)
                {
                    this._4_1 = _4_1;
                    this._3_1 = _3_1;
                }
                
                public Inners._4 _4_1 { get; }
                public Inners._3 _3_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _44 : _percentx21ⲻ7E
            {
                public _44(Inners._4 _4_1, Inners._4 _4_2)
                {
                    this._4_1 = _4_1;
                    this._4_2 = _4_2;
                }
                
                public Inners._4 _4_1 { get; }
                public Inners._4 _4_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _45 : _percentx21ⲻ7E
            {
                public _45(Inners._4 _4_1, Inners._5 _5_1)
                {
                    this._4_1 = _4_1;
                    this._5_1 = _5_1;
                }
                
                public Inners._4 _4_1 { get; }
                public Inners._5 _5_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _46 : _percentx21ⲻ7E
            {
                public _46(Inners._4 _4_1, Inners._6 _6_1)
                {
                    this._4_1 = _4_1;
                    this._6_1 = _6_1;
                }
                
                public Inners._4 _4_1 { get; }
                public Inners._6 _6_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _47 : _percentx21ⲻ7E
            {
                public _47(Inners._4 _4_1, Inners._7 _7_1)
                {
                    this._4_1 = _4_1;
                    this._7_1 = _7_1;
                }
                
                public Inners._4 _4_1 { get; }
                public Inners._7 _7_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _48 : _percentx21ⲻ7E
            {
                public _48(Inners._4 _4_1, Inners._8 _8_1)
                {
                    this._4_1 = _4_1;
                    this._8_1 = _8_1;
                }
                
                public Inners._4 _4_1 { get; }
                public Inners._8 _8_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _49 : _percentx21ⲻ7E
            {
                public _49(Inners._4 _4_1, Inners._9 _9_1)
                {
                    this._4_1 = _4_1;
                    this._9_1 = _9_1;
                }
                
                public Inners._4 _4_1 { get; }
                public Inners._9 _9_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _4A : _percentx21ⲻ7E
            {
                public _4A(Inners._4 _4_1, Inners._A _A_1)
                {
                    this._4_1 = _4_1;
                    this._A_1 = _A_1;
                }
                
                public Inners._4 _4_1 { get; }
                public Inners._A _A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _4B : _percentx21ⲻ7E
            {
                public _4B(Inners._4 _4_1, Inners._B _B_1)
                {
                    this._4_1 = _4_1;
                    this._B_1 = _B_1;
                }
                
                public Inners._4 _4_1 { get; }
                public Inners._B _B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _4C : _percentx21ⲻ7E
            {
                public _4C(Inners._4 _4_1, Inners._C _C_1)
                {
                    this._4_1 = _4_1;
                    this._C_1 = _C_1;
                }
                
                public Inners._4 _4_1 { get; }
                public Inners._C _C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _4D : _percentx21ⲻ7E
            {
                public _4D(Inners._4 _4_1, Inners._D _D_1)
                {
                    this._4_1 = _4_1;
                    this._D_1 = _D_1;
                }
                
                public Inners._4 _4_1 { get; }
                public Inners._D _D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _4E : _percentx21ⲻ7E
            {
                public _4E(Inners._4 _4_1, Inners._E _E_1)
                {
                    this._4_1 = _4_1;
                    this._E_1 = _E_1;
                }
                
                public Inners._4 _4_1 { get; }
                public Inners._E _E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _4F : _percentx21ⲻ7E
            {
                public _4F(Inners._4 _4_1, Inners._F _F_1)
                {
                    this._4_1 = _4_1;
                    this._F_1 = _F_1;
                }
                
                public Inners._4 _4_1 { get; }
                public Inners._F _F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _50 : _percentx21ⲻ7E
            {
                public _50(Inners._5 _5_1, Inners._0 _0_1)
                {
                    this._5_1 = _5_1;
                    this._0_1 = _0_1;
                }
                
                public Inners._5 _5_1 { get; }
                public Inners._0 _0_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _51 : _percentx21ⲻ7E
            {
                public _51(Inners._5 _5_1, Inners._1 _1_1)
                {
                    this._5_1 = _5_1;
                    this._1_1 = _1_1;
                }
                
                public Inners._5 _5_1 { get; }
                public Inners._1 _1_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _52 : _percentx21ⲻ7E
            {
                public _52(Inners._5 _5_1, Inners._2 _2_1)
                {
                    this._5_1 = _5_1;
                    this._2_1 = _2_1;
                }
                
                public Inners._5 _5_1 { get; }
                public Inners._2 _2_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _53 : _percentx21ⲻ7E
            {
                public _53(Inners._5 _5_1, Inners._3 _3_1)
                {
                    this._5_1 = _5_1;
                    this._3_1 = _3_1;
                }
                
                public Inners._5 _5_1 { get; }
                public Inners._3 _3_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _54 : _percentx21ⲻ7E
            {
                public _54(Inners._5 _5_1, Inners._4 _4_1)
                {
                    this._5_1 = _5_1;
                    this._4_1 = _4_1;
                }
                
                public Inners._5 _5_1 { get; }
                public Inners._4 _4_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _55 : _percentx21ⲻ7E
            {
                public _55(Inners._5 _5_1, Inners._5 _5_2)
                {
                    this._5_1 = _5_1;
                    this._5_2 = _5_2;
                }
                
                public Inners._5 _5_1 { get; }
                public Inners._5 _5_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _56 : _percentx21ⲻ7E
            {
                public _56(Inners._5 _5_1, Inners._6 _6_1)
                {
                    this._5_1 = _5_1;
                    this._6_1 = _6_1;
                }
                
                public Inners._5 _5_1 { get; }
                public Inners._6 _6_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _57 : _percentx21ⲻ7E
            {
                public _57(Inners._5 _5_1, Inners._7 _7_1)
                {
                    this._5_1 = _5_1;
                    this._7_1 = _7_1;
                }
                
                public Inners._5 _5_1 { get; }
                public Inners._7 _7_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _58 : _percentx21ⲻ7E
            {
                public _58(Inners._5 _5_1, Inners._8 _8_1)
                {
                    this._5_1 = _5_1;
                    this._8_1 = _8_1;
                }
                
                public Inners._5 _5_1 { get; }
                public Inners._8 _8_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _59 : _percentx21ⲻ7E
            {
                public _59(Inners._5 _5_1, Inners._9 _9_1)
                {
                    this._5_1 = _5_1;
                    this._9_1 = _9_1;
                }
                
                public Inners._5 _5_1 { get; }
                public Inners._9 _9_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _5A : _percentx21ⲻ7E
            {
                public _5A(Inners._5 _5_1, Inners._A _A_1)
                {
                    this._5_1 = _5_1;
                    this._A_1 = _A_1;
                }
                
                public Inners._5 _5_1 { get; }
                public Inners._A _A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _5B : _percentx21ⲻ7E
            {
                public _5B(Inners._5 _5_1, Inners._B _B_1)
                {
                    this._5_1 = _5_1;
                    this._B_1 = _B_1;
                }
                
                public Inners._5 _5_1 { get; }
                public Inners._B _B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _5C : _percentx21ⲻ7E
            {
                public _5C(Inners._5 _5_1, Inners._C _C_1)
                {
                    this._5_1 = _5_1;
                    this._C_1 = _C_1;
                }
                
                public Inners._5 _5_1 { get; }
                public Inners._C _C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _5D : _percentx21ⲻ7E
            {
                public _5D(Inners._5 _5_1, Inners._D _D_1)
                {
                    this._5_1 = _5_1;
                    this._D_1 = _D_1;
                }
                
                public Inners._5 _5_1 { get; }
                public Inners._D _D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _5E : _percentx21ⲻ7E
            {
                public _5E(Inners._5 _5_1, Inners._E _E_1)
                {
                    this._5_1 = _5_1;
                    this._E_1 = _E_1;
                }
                
                public Inners._5 _5_1 { get; }
                public Inners._E _E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _5F : _percentx21ⲻ7E
            {
                public _5F(Inners._5 _5_1, Inners._F _F_1)
                {
                    this._5_1 = _5_1;
                    this._F_1 = _F_1;
                }
                
                public Inners._5 _5_1 { get; }
                public Inners._F _F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _60 : _percentx21ⲻ7E
            {
                public _60(Inners._6 _6_1, Inners._0 _0_1)
                {
                    this._6_1 = _6_1;
                    this._0_1 = _0_1;
                }
                
                public Inners._6 _6_1 { get; }
                public Inners._0 _0_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _61 : _percentx21ⲻ7E
            {
                public _61(Inners._6 _6_1, Inners._1 _1_1)
                {
                    this._6_1 = _6_1;
                    this._1_1 = _1_1;
                }
                
                public Inners._6 _6_1 { get; }
                public Inners._1 _1_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _62 : _percentx21ⲻ7E
            {
                public _62(Inners._6 _6_1, Inners._2 _2_1)
                {
                    this._6_1 = _6_1;
                    this._2_1 = _2_1;
                }
                
                public Inners._6 _6_1 { get; }
                public Inners._2 _2_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _63 : _percentx21ⲻ7E
            {
                public _63(Inners._6 _6_1, Inners._3 _3_1)
                {
                    this._6_1 = _6_1;
                    this._3_1 = _3_1;
                }
                
                public Inners._6 _6_1 { get; }
                public Inners._3 _3_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _64 : _percentx21ⲻ7E
            {
                public _64(Inners._6 _6_1, Inners._4 _4_1)
                {
                    this._6_1 = _6_1;
                    this._4_1 = _4_1;
                }
                
                public Inners._6 _6_1 { get; }
                public Inners._4 _4_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _65 : _percentx21ⲻ7E
            {
                public _65(Inners._6 _6_1, Inners._5 _5_1)
                {
                    this._6_1 = _6_1;
                    this._5_1 = _5_1;
                }
                
                public Inners._6 _6_1 { get; }
                public Inners._5 _5_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _66 : _percentx21ⲻ7E
            {
                public _66(Inners._6 _6_1, Inners._6 _6_2)
                {
                    this._6_1 = _6_1;
                    this._6_2 = _6_2;
                }
                
                public Inners._6 _6_1 { get; }
                public Inners._6 _6_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _67 : _percentx21ⲻ7E
            {
                public _67(Inners._6 _6_1, Inners._7 _7_1)
                {
                    this._6_1 = _6_1;
                    this._7_1 = _7_1;
                }
                
                public Inners._6 _6_1 { get; }
                public Inners._7 _7_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _68 : _percentx21ⲻ7E
            {
                public _68(Inners._6 _6_1, Inners._8 _8_1)
                {
                    this._6_1 = _6_1;
                    this._8_1 = _8_1;
                }
                
                public Inners._6 _6_1 { get; }
                public Inners._8 _8_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _69 : _percentx21ⲻ7E
            {
                public _69(Inners._6 _6_1, Inners._9 _9_1)
                {
                    this._6_1 = _6_1;
                    this._9_1 = _9_1;
                }
                
                public Inners._6 _6_1 { get; }
                public Inners._9 _9_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _6A : _percentx21ⲻ7E
            {
                public _6A(Inners._6 _6_1, Inners._A _A_1)
                {
                    this._6_1 = _6_1;
                    this._A_1 = _A_1;
                }
                
                public Inners._6 _6_1 { get; }
                public Inners._A _A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _6B : _percentx21ⲻ7E
            {
                public _6B(Inners._6 _6_1, Inners._B _B_1)
                {
                    this._6_1 = _6_1;
                    this._B_1 = _B_1;
                }
                
                public Inners._6 _6_1 { get; }
                public Inners._B _B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _6C : _percentx21ⲻ7E
            {
                public _6C(Inners._6 _6_1, Inners._C _C_1)
                {
                    this._6_1 = _6_1;
                    this._C_1 = _C_1;
                }
                
                public Inners._6 _6_1 { get; }
                public Inners._C _C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _6D : _percentx21ⲻ7E
            {
                public _6D(Inners._6 _6_1, Inners._D _D_1)
                {
                    this._6_1 = _6_1;
                    this._D_1 = _D_1;
                }
                
                public Inners._6 _6_1 { get; }
                public Inners._D _D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _6E : _percentx21ⲻ7E
            {
                public _6E(Inners._6 _6_1, Inners._E _E_1)
                {
                    this._6_1 = _6_1;
                    this._E_1 = _E_1;
                }
                
                public Inners._6 _6_1 { get; }
                public Inners._E _E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _6F : _percentx21ⲻ7E
            {
                public _6F(Inners._6 _6_1, Inners._F _F_1)
                {
                    this._6_1 = _6_1;
                    this._F_1 = _F_1;
                }
                
                public Inners._6 _6_1 { get; }
                public Inners._F _F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _70 : _percentx21ⲻ7E
            {
                public _70(Inners._7 _7_1, Inners._0 _0_1)
                {
                    this._7_1 = _7_1;
                    this._0_1 = _0_1;
                }
                
                public Inners._7 _7_1 { get; }
                public Inners._0 _0_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _71 : _percentx21ⲻ7E
            {
                public _71(Inners._7 _7_1, Inners._1 _1_1)
                {
                    this._7_1 = _7_1;
                    this._1_1 = _1_1;
                }
                
                public Inners._7 _7_1 { get; }
                public Inners._1 _1_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _72 : _percentx21ⲻ7E
            {
                public _72(Inners._7 _7_1, Inners._2 _2_1)
                {
                    this._7_1 = _7_1;
                    this._2_1 = _2_1;
                }
                
                public Inners._7 _7_1 { get; }
                public Inners._2 _2_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _73 : _percentx21ⲻ7E
            {
                public _73(Inners._7 _7_1, Inners._3 _3_1)
                {
                    this._7_1 = _7_1;
                    this._3_1 = _3_1;
                }
                
                public Inners._7 _7_1 { get; }
                public Inners._3 _3_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _74 : _percentx21ⲻ7E
            {
                public _74(Inners._7 _7_1, Inners._4 _4_1)
                {
                    this._7_1 = _7_1;
                    this._4_1 = _4_1;
                }
                
                public Inners._7 _7_1 { get; }
                public Inners._4 _4_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _75 : _percentx21ⲻ7E
            {
                public _75(Inners._7 _7_1, Inners._5 _5_1)
                {
                    this._7_1 = _7_1;
                    this._5_1 = _5_1;
                }
                
                public Inners._7 _7_1 { get; }
                public Inners._5 _5_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _76 : _percentx21ⲻ7E
            {
                public _76(Inners._7 _7_1, Inners._6 _6_1)
                {
                    this._7_1 = _7_1;
                    this._6_1 = _6_1;
                }
                
                public Inners._7 _7_1 { get; }
                public Inners._6 _6_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _77 : _percentx21ⲻ7E
            {
                public _77(Inners._7 _7_1, Inners._7 _7_2)
                {
                    this._7_1 = _7_1;
                    this._7_2 = _7_2;
                }
                
                public Inners._7 _7_1 { get; }
                public Inners._7 _7_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _78 : _percentx21ⲻ7E
            {
                public _78(Inners._7 _7_1, Inners._8 _8_1)
                {
                    this._7_1 = _7_1;
                    this._8_1 = _8_1;
                }
                
                public Inners._7 _7_1 { get; }
                public Inners._8 _8_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _79 : _percentx21ⲻ7E
            {
                public _79(Inners._7 _7_1, Inners._9 _9_1)
                {
                    this._7_1 = _7_1;
                    this._9_1 = _9_1;
                }
                
                public Inners._7 _7_1 { get; }
                public Inners._9 _9_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _7A : _percentx21ⲻ7E
            {
                public _7A(Inners._7 _7_1, Inners._A _A_1)
                {
                    this._7_1 = _7_1;
                    this._A_1 = _A_1;
                }
                
                public Inners._7 _7_1 { get; }
                public Inners._A _A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _7B : _percentx21ⲻ7E
            {
                public _7B(Inners._7 _7_1, Inners._B _B_1)
                {
                    this._7_1 = _7_1;
                    this._B_1 = _B_1;
                }
                
                public Inners._7 _7_1 { get; }
                public Inners._B _B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _7C : _percentx21ⲻ7E
            {
                public _7C(Inners._7 _7_1, Inners._C _C_1)
                {
                    this._7_1 = _7_1;
                    this._C_1 = _C_1;
                }
                
                public Inners._7 _7_1 { get; }
                public Inners._C _C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _7D : _percentx21ⲻ7E
            {
                public _7D(Inners._7 _7_1, Inners._D _D_1)
                {
                    this._7_1 = _7_1;
                    this._D_1 = _D_1;
                }
                
                public Inners._7 _7_1 { get; }
                public Inners._D _D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _7E : _percentx21ⲻ7E
            {
                public _7E(Inners._7 _7_1, Inners._E _E_1)
                {
                    this._7_1 = _7_1;
                    this._E_1 = _E_1;
                }
                
                public Inners._7 _7_1 { get; }
                public Inners._E _E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
        }
    }
    
}
