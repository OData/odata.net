namespace __Generated.CstNodes.Rules
{
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
            
            protected internal abstract TResult Accept(_ALPHA._Ⰳx41ⲻ5A node, TContext context);
            protected internal abstract TResult Accept(_ALPHA._Ⰳx61ⲻ7A node, TContext context);
        }
        
        public sealed class _Ⰳx41ⲻ5A : _ALPHA
        {
            public _Ⰳx41ⲻ5A(Inners._Ⰳx41ⲻ5A _Ⰳx41ⲻ5A_1)
            {
                this._Ⰳx41ⲻ5A_1 = _Ⰳx41ⲻ5A_1;
            }
            
            public Inners._Ⰳx41ⲻ5A _Ⰳx41ⲻ5A_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _Ⰳx61ⲻ7A : _ALPHA
        {
            public _Ⰳx61ⲻ7A(Inners._Ⰳx61ⲻ7A _Ⰳx61ⲻ7A_1)
            {
                this._Ⰳx61ⲻ7A_1 = _Ⰳx61ⲻ7A_1;
            }
            
            public Inners._Ⰳx61ⲻ7A _Ⰳx61ⲻ7A_1 { get; }
            
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
            
            protected internal abstract TResult Accept(_BIT._ʺx30ʺ node, TContext context);
            protected internal abstract TResult Accept(_BIT._ʺx31ʺ node, TContext context);
        }
        
        public sealed class _ʺx30ʺ : _BIT
        {
            public _ʺx30ʺ(Inners._ʺx30ʺ _ʺx30ʺ_1)
            {
                this._ʺx30ʺ_1 = _ʺx30ʺ_1;
            }
            
            public Inners._ʺx30ʺ _ʺx30ʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx31ʺ : _BIT
        {
            public _ʺx31ʺ(Inners._ʺx31ʺ _ʺx31ʺ_1)
            {
                this._ʺx31ʺ_1 = _ʺx31ʺ_1;
            }
            
            public Inners._ʺx31ʺ _ʺx31ʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
    public sealed class _CHAR
    {
        public _CHAR(Inners._Ⰳx01ⲻ7F _Ⰳx01ⲻ7F_1)
        {
            this._Ⰳx01ⲻ7F_1 = _Ⰳx01ⲻ7F_1;
        }
        
        public Inners._Ⰳx01ⲻ7F _Ⰳx01ⲻ7F_1 { get; }
    }
    
    public sealed class _CR
    {
        public _CR(Inners._Ⰳx0D _Ⰳx0D_1)
        {
            this._Ⰳx0D_1 = _Ⰳx0D_1;
        }
        
        public Inners._Ⰳx0D _Ⰳx0D_1 { get; }
    }
    
    public sealed class _CRLF
    {
        public _CRLF(__Generated.CstNodes.Rules._CR _CR_1, __Generated.CstNodes.Rules._LF _LF_1)
        {
            this._CR_1 = _CR_1;
            this._LF_1 = _LF_1;
        }
        
        public __Generated.CstNodes.Rules._CR _CR_1 { get; }
        public __Generated.CstNodes.Rules._LF _LF_1 { get; }
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
            
            protected internal abstract TResult Accept(_CTL._Ⰳx00ⲻ1F node, TContext context);
            protected internal abstract TResult Accept(_CTL._Ⰳx7F node, TContext context);
        }
        
        public sealed class _Ⰳx00ⲻ1F : _CTL
        {
            public _Ⰳx00ⲻ1F(Inners._Ⰳx00ⲻ1F _Ⰳx00ⲻ1F_1)
            {
                this._Ⰳx00ⲻ1F_1 = _Ⰳx00ⲻ1F_1;
            }
            
            public Inners._Ⰳx00ⲻ1F _Ⰳx00ⲻ1F_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _Ⰳx7F : _CTL
        {
            public _Ⰳx7F(Inners._Ⰳx7F _Ⰳx7F_1)
            {
                this._Ⰳx7F_1 = _Ⰳx7F_1;
            }
            
            public Inners._Ⰳx7F _Ⰳx7F_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
    public sealed class _DIGIT
    {
        public _DIGIT(Inners._Ⰳx30ⲻ39 _Ⰳx30ⲻ39_1)
        {
            this._Ⰳx30ⲻ39_1 = _Ⰳx30ⲻ39_1;
        }
        
        public Inners._Ⰳx30ⲻ39 _Ⰳx30ⲻ39_1 { get; }
    }
    
    public sealed class _DQUOTE
    {
        public _DQUOTE(Inners._Ⰳx22 _Ⰳx22_1)
        {
            this._Ⰳx22_1 = _Ⰳx22_1;
        }
        
        public Inners._Ⰳx22 _Ⰳx22_1 { get; }
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
            protected internal abstract TResult Accept(_HEXDIG._ʺx41ʺ node, TContext context);
            protected internal abstract TResult Accept(_HEXDIG._ʺx42ʺ node, TContext context);
            protected internal abstract TResult Accept(_HEXDIG._ʺx43ʺ node, TContext context);
            protected internal abstract TResult Accept(_HEXDIG._ʺx44ʺ node, TContext context);
            protected internal abstract TResult Accept(_HEXDIG._ʺx45ʺ node, TContext context);
            protected internal abstract TResult Accept(_HEXDIG._ʺx46ʺ node, TContext context);
        }
        
        public sealed class _DIGIT : _HEXDIG
        {
            public _DIGIT(__Generated.CstNodes.Rules._DIGIT _DIGIT_1)
            {
                this._DIGIT_1 = _DIGIT_1;
            }
            
            public __Generated.CstNodes.Rules._DIGIT _DIGIT_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx41ʺ : _HEXDIG
        {
            public _ʺx41ʺ(Inners._ʺx41ʺ _ʺx41ʺ_1)
            {
                this._ʺx41ʺ_1 = _ʺx41ʺ_1;
            }
            
            public Inners._ʺx41ʺ _ʺx41ʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx42ʺ : _HEXDIG
        {
            public _ʺx42ʺ(Inners._ʺx42ʺ _ʺx42ʺ_1)
            {
                this._ʺx42ʺ_1 = _ʺx42ʺ_1;
            }
            
            public Inners._ʺx42ʺ _ʺx42ʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx43ʺ : _HEXDIG
        {
            public _ʺx43ʺ(Inners._ʺx43ʺ _ʺx43ʺ_1)
            {
                this._ʺx43ʺ_1 = _ʺx43ʺ_1;
            }
            
            public Inners._ʺx43ʺ _ʺx43ʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx44ʺ : _HEXDIG
        {
            public _ʺx44ʺ(Inners._ʺx44ʺ _ʺx44ʺ_1)
            {
                this._ʺx44ʺ_1 = _ʺx44ʺ_1;
            }
            
            public Inners._ʺx44ʺ _ʺx44ʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx45ʺ : _HEXDIG
        {
            public _ʺx45ʺ(Inners._ʺx45ʺ _ʺx45ʺ_1)
            {
                this._ʺx45ʺ_1 = _ʺx45ʺ_1;
            }
            
            public Inners._ʺx45ʺ _ʺx45ʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx46ʺ : _HEXDIG
        {
            public _ʺx46ʺ(Inners._ʺx46ʺ _ʺx46ʺ_1)
            {
                this._ʺx46ʺ_1 = _ʺx46ʺ_1;
            }
            
            public Inners._ʺx46ʺ _ʺx46ʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
    public sealed class _HTAB
    {
        public _HTAB(Inners._Ⰳx09 _Ⰳx09_1)
        {
            this._Ⰳx09_1 = _Ⰳx09_1;
        }
        
        public Inners._Ⰳx09 _Ⰳx09_1 { get; }
    }
    
    public sealed class _LF
    {
        public _LF(Inners._Ⰳx0A _Ⰳx0A_1)
        {
            this._Ⰳx0A_1 = _Ⰳx0A_1;
        }
        
        public Inners._Ⰳx0A _Ⰳx0A_1 { get; }
    }
    
    public sealed class _LWSP
    {
        public _LWSP(System.Collections.Generic.IEnumerable<Inners._ⲤWSPⳆCRLF_WSPↃ> _ⲤWSPⳆCRLF_WSPↃ_1)
        {
            this._ⲤWSPⳆCRLF_WSPↃ_1 = _ⲤWSPⳆCRLF_WSPↃ_1;
        }
        
        public System.Collections.Generic.IEnumerable<Inners._ⲤWSPⳆCRLF_WSPↃ> _ⲤWSPⳆCRLF_WSPↃ_1 { get; }
    }
    
    public sealed class _OCTET
    {
        public _OCTET(Inners._Ⰳx00ⲻFF _Ⰳx00ⲻFF_1)
        {
            this._Ⰳx00ⲻFF_1 = _Ⰳx00ⲻFF_1;
        }
        
        public Inners._Ⰳx00ⲻFF _Ⰳx00ⲻFF_1 { get; }
    }
    
    public sealed class _SP
    {
        public _SP(Inners._Ⰳx20 _Ⰳx20_1)
        {
            this._Ⰳx20_1 = _Ⰳx20_1;
        }
        
        public Inners._Ⰳx20 _Ⰳx20_1 { get; }
    }
    
    public sealed class _VCHAR
    {
        public _VCHAR(Inners._Ⰳx21ⲻ7E _Ⰳx21ⲻ7E_1)
        {
            this._Ⰳx21ⲻ7E_1 = _Ⰳx21ⲻ7E_1;
        }
        
        public Inners._Ⰳx21ⲻ7E _Ⰳx21ⲻ7E_1 { get; }
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
            public _SP(__Generated.CstNodes.Rules._SP _SP_1)
            {
                this._SP_1 = _SP_1;
            }
            
            public __Generated.CstNodes.Rules._SP _SP_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _HTAB : _WSP
        {
            public _HTAB(__Generated.CstNodes.Rules._HTAB _HTAB_1)
            {
                this._HTAB_1 = _HTAB_1;
            }
            
            public __Generated.CstNodes.Rules._HTAB _HTAB_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
    public sealed class _rulelist
    {
        public _rulelist(System.Collections.Generic.IEnumerable<Inners._ⲤruleⳆⲤЖcⲻwsp_cⲻnlↃↃ> _ⲤruleⳆⲤЖcⲻwsp_cⲻnlↃↃ_1)
        {
            this._ⲤruleⳆⲤЖcⲻwsp_cⲻnlↃↃ_1 = _ⲤruleⳆⲤЖcⲻwsp_cⲻnlↃↃ_1;
        }
        
        public System.Collections.Generic.IEnumerable<Inners._ⲤruleⳆⲤЖcⲻwsp_cⲻnlↃↃ> _ⲤruleⳆⲤЖcⲻwsp_cⲻnlↃↃ_1 { get; }
    }
    
    public sealed class _rule
    {
        public _rule(__Generated.CstNodes.Rules._rulename _rulename_1, __Generated.CstNodes.Rules._definedⲻas _definedⲻas_1, __Generated.CstNodes.Rules._elements _elements_1, __Generated.CstNodes.Rules._cⲻnl _cⲻnl_1)
        {
            this._rulename_1 = _rulename_1;
            this._definedⲻas_1 = _definedⲻas_1;
            this._elements_1 = _elements_1;
            this._cⲻnl_1 = _cⲻnl_1;
        }
        
        public __Generated.CstNodes.Rules._rulename _rulename_1 { get; }
        public __Generated.CstNodes.Rules._definedⲻas _definedⲻas_1 { get; }
        public __Generated.CstNodes.Rules._elements _elements_1 { get; }
        public __Generated.CstNodes.Rules._cⲻnl _cⲻnl_1 { get; }
    }
    
    public sealed class _rulename
    {
        public _rulename(__Generated.CstNodes.Rules._ALPHA _ALPHA_1, System.Collections.Generic.IEnumerable<Inners._ⲤALPHAⳆDIGITⳆʺx2DʺↃ> _ⲤALPHAⳆDIGITⳆʺx2DʺↃ_1)
        {
            this._ALPHA_1 = _ALPHA_1;
            this._ⲤALPHAⳆDIGITⳆʺx2DʺↃ_1 = _ⲤALPHAⳆDIGITⳆʺx2DʺↃ_1;
        }
        
        public __Generated.CstNodes.Rules._ALPHA _ALPHA_1 { get; }
        public System.Collections.Generic.IEnumerable<Inners._ⲤALPHAⳆDIGITⳆʺx2DʺↃ> _ⲤALPHAⳆDIGITⳆʺx2DʺↃ_1 { get; }
    }
    
    public sealed class _definedⲻas
    {
        public _definedⲻas(System.Collections.Generic.IEnumerable<__Generated.CstNodes.Rules._cⲻwsp> _cⲻwsp_1, Inners._Ⲥʺx3DʺⳆʺx3Dx2FʺↃ _Ⲥʺx3DʺⳆʺx3Dx2FʺↃ_1, System.Collections.Generic.IEnumerable<__Generated.CstNodes.Rules._cⲻwsp> _cⲻwsp_2)
        {
            this._cⲻwsp_1 = _cⲻwsp_1;
            this._Ⲥʺx3DʺⳆʺx3Dx2FʺↃ_1 = _Ⲥʺx3DʺⳆʺx3Dx2FʺↃ_1;
            this._cⲻwsp_2 = _cⲻwsp_2;
        }
        
        public System.Collections.Generic.IEnumerable<__Generated.CstNodes.Rules._cⲻwsp> _cⲻwsp_1 { get; }
        public Inners._Ⲥʺx3DʺⳆʺx3Dx2FʺↃ _Ⲥʺx3DʺⳆʺx3Dx2FʺↃ_1 { get; }
        public System.Collections.Generic.IEnumerable<__Generated.CstNodes.Rules._cⲻwsp> _cⲻwsp_2 { get; }
    }
    
    public sealed class _elements
    {
        public _elements(__Generated.CstNodes.Rules._alternation _alternation_1, System.Collections.Generic.IEnumerable<__Generated.CstNodes.Rules._cⲻwsp> _cⲻwsp_1)
        {
            this._alternation_1 = _alternation_1;
            this._cⲻwsp_1 = _cⲻwsp_1;
        }
        
        public __Generated.CstNodes.Rules._alternation _alternation_1 { get; }
        public System.Collections.Generic.IEnumerable<__Generated.CstNodes.Rules._cⲻwsp> _cⲻwsp_1 { get; }
    }
    
    public abstract class _cⲻwsp
    {
        private _cⲻwsp()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_cⲻwsp node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_cⲻwsp._WSP node, TContext context);
            protected internal abstract TResult Accept(_cⲻwsp._Ⲥcⲻnl_WSPↃ node, TContext context);
        }
        
        public sealed class _WSP : _cⲻwsp
        {
            public _WSP(__Generated.CstNodes.Rules._WSP _WSP_1)
            {
                this._WSP_1 = _WSP_1;
            }
            
            public __Generated.CstNodes.Rules._WSP _WSP_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _Ⲥcⲻnl_WSPↃ : _cⲻwsp
        {
            public _Ⲥcⲻnl_WSPↃ(Inners._Ⲥcⲻnl_WSPↃ _Ⲥcⲻnl_WSPↃ_1)
            {
                this._Ⲥcⲻnl_WSPↃ_1 = _Ⲥcⲻnl_WSPↃ_1;
            }
            
            public Inners._Ⲥcⲻnl_WSPↃ _Ⲥcⲻnl_WSPↃ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
    public abstract class _cⲻnl
    {
        private _cⲻnl()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_cⲻnl node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_cⲻnl._comment node, TContext context);
            protected internal abstract TResult Accept(_cⲻnl._CRLF node, TContext context);
        }
        
        public sealed class _comment : _cⲻnl
        {
            public _comment(__Generated.CstNodes.Rules._comment _comment_1)
            {
                this._comment_1 = _comment_1;
            }
            
            public __Generated.CstNodes.Rules._comment _comment_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _CRLF : _cⲻnl
        {
            public _CRLF(__Generated.CstNodes.Rules._CRLF _CRLF_1)
            {
                this._CRLF_1 = _CRLF_1;
            }
            
            public __Generated.CstNodes.Rules._CRLF _CRLF_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
    public sealed class _comment
    {
        public _comment(Inners._ʺx3Bʺ _ʺx3Bʺ_1, System.Collections.Generic.IEnumerable<Inners._ⲤWSPⳆVCHARↃ> _ⲤWSPⳆVCHARↃ_1, __Generated.CstNodes.Rules._CRLF _CRLF_1)
        {
            this._ʺx3Bʺ_1 = _ʺx3Bʺ_1;
            this._ⲤWSPⳆVCHARↃ_1 = _ⲤWSPⳆVCHARↃ_1;
            this._CRLF_1 = _CRLF_1;
        }
        
        public Inners._ʺx3Bʺ _ʺx3Bʺ_1 { get; }
        public System.Collections.Generic.IEnumerable<Inners._ⲤWSPⳆVCHARↃ> _ⲤWSPⳆVCHARↃ_1 { get; }
        public __Generated.CstNodes.Rules._CRLF _CRLF_1 { get; }
    }
    
    public sealed class _alternation
    {
        public _alternation(__Generated.CstNodes.Rules._concatenation _concatenation_1, System.Collections.Generic.IEnumerable<Inners._ⲤЖcⲻwsp_ʺx2Fʺ_Жcⲻwsp_concatenationↃ> _ⲤЖcⲻwsp_ʺx2Fʺ_Жcⲻwsp_concatenationↃ_1)
        {
            this._concatenation_1 = _concatenation_1;
            this._ⲤЖcⲻwsp_ʺx2Fʺ_Жcⲻwsp_concatenationↃ_1 = _ⲤЖcⲻwsp_ʺx2Fʺ_Жcⲻwsp_concatenationↃ_1;
        }
        
        public __Generated.CstNodes.Rules._concatenation _concatenation_1 { get; }
        public System.Collections.Generic.IEnumerable<Inners._ⲤЖcⲻwsp_ʺx2Fʺ_Жcⲻwsp_concatenationↃ> _ⲤЖcⲻwsp_ʺx2Fʺ_Жcⲻwsp_concatenationↃ_1 { get; }
    }
    
    public sealed class _concatenation
    {
        public _concatenation(__Generated.CstNodes.Rules._repetition _repetition_1, System.Collections.Generic.IEnumerable<Inners._Ⲥ1Жcⲻwsp_repetitionↃ> _Ⲥ1Жcⲻwsp_repetitionↃ_1)
        {
            this._repetition_1 = _repetition_1;
            this._Ⲥ1Жcⲻwsp_repetitionↃ_1 = _Ⲥ1Жcⲻwsp_repetitionↃ_1;
        }
        
        public __Generated.CstNodes.Rules._repetition _repetition_1 { get; }
        public System.Collections.Generic.IEnumerable<Inners._Ⲥ1Жcⲻwsp_repetitionↃ> _Ⲥ1Жcⲻwsp_repetitionↃ_1 { get; }
    }
    
    public sealed class _repetition
    {
        public _repetition(__Generated.CstNodes.Rules._repeat? _repeat_1, __Generated.CstNodes.Rules._element _element_1)
        {
            this._repeat_1 = _repeat_1;
            this._element_1 = _element_1;
        }
        
        public __Generated.CstNodes.Rules._repeat? _repeat_1 { get; }
        public __Generated.CstNodes.Rules._element _element_1 { get; }
    }
    
    public abstract class _repeat
    {
        private _repeat()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_repeat node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_repeat._1ЖDIGIT node, TContext context);
            protected internal abstract TResult Accept(_repeat._ⲤЖDIGIT_ʺx2Aʺ_ЖDIGITↃ node, TContext context);
        }
        
        public sealed class _1ЖDIGIT : _repeat
        {
            public _1ЖDIGIT(System.Collections.Generic.IEnumerable<__Generated.CstNodes.Rules._DIGIT> _DIGIT_1)
            {
                this._DIGIT_1 = _DIGIT_1;
            }
            
            public System.Collections.Generic.IEnumerable<__Generated.CstNodes.Rules._DIGIT> _DIGIT_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ⲤЖDIGIT_ʺx2Aʺ_ЖDIGITↃ : _repeat
        {
            public _ⲤЖDIGIT_ʺx2Aʺ_ЖDIGITↃ(Inners._ⲤЖDIGIT_ʺx2Aʺ_ЖDIGITↃ _ⲤЖDIGIT_ʺx2Aʺ_ЖDIGITↃ_1)
            {
                this._ⲤЖDIGIT_ʺx2Aʺ_ЖDIGITↃ_1 = _ⲤЖDIGIT_ʺx2Aʺ_ЖDIGITↃ_1;
            }
            
            public Inners._ⲤЖDIGIT_ʺx2Aʺ_ЖDIGITↃ _ⲤЖDIGIT_ʺx2Aʺ_ЖDIGITↃ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
    public abstract class _element
    {
        private _element()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_element node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_element._rulename node, TContext context);
            protected internal abstract TResult Accept(_element._group node, TContext context);
            protected internal abstract TResult Accept(_element._option node, TContext context);
            protected internal abstract TResult Accept(_element._charⲻval node, TContext context);
            protected internal abstract TResult Accept(_element._numⲻval node, TContext context);
            protected internal abstract TResult Accept(_element._proseⲻval node, TContext context);
        }
        
        public sealed class _rulename : _element
        {
            public _rulename(__Generated.CstNodes.Rules._rulename _rulename_1)
            {
                this._rulename_1 = _rulename_1;
            }
            
            public __Generated.CstNodes.Rules._rulename _rulename_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _group : _element
        {
            public _group(__Generated.CstNodes.Rules._group _group_1)
            {
                this._group_1 = _group_1;
            }
            
            public __Generated.CstNodes.Rules._group _group_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _option : _element
        {
            public _option(__Generated.CstNodes.Rules._option _option_1)
            {
                this._option_1 = _option_1;
            }
            
            public __Generated.CstNodes.Rules._option _option_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _charⲻval : _element
        {
            public _charⲻval(__Generated.CstNodes.Rules._charⲻval _charⲻval_1)
            {
                this._charⲻval_1 = _charⲻval_1;
            }
            
            public __Generated.CstNodes.Rules._charⲻval _charⲻval_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _numⲻval : _element
        {
            public _numⲻval(__Generated.CstNodes.Rules._numⲻval _numⲻval_1)
            {
                this._numⲻval_1 = _numⲻval_1;
            }
            
            public __Generated.CstNodes.Rules._numⲻval _numⲻval_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _proseⲻval : _element
        {
            public _proseⲻval(__Generated.CstNodes.Rules._proseⲻval _proseⲻval_1)
            {
                this._proseⲻval_1 = _proseⲻval_1;
            }
            
            public __Generated.CstNodes.Rules._proseⲻval _proseⲻval_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
    public sealed class _group
    {
        public _group(Inners._ʺx28ʺ _ʺx28ʺ_1, System.Collections.Generic.IEnumerable<__Generated.CstNodes.Rules._cⲻwsp> _cⲻwsp_1, __Generated.CstNodes.Rules._alternation _alternation_1, System.Collections.Generic.IEnumerable<__Generated.CstNodes.Rules._cⲻwsp> _cⲻwsp_2, Inners._ʺx29ʺ _ʺx29ʺ_1)
        {
            this._ʺx28ʺ_1 = _ʺx28ʺ_1;
            this._cⲻwsp_1 = _cⲻwsp_1;
            this._alternation_1 = _alternation_1;
            this._cⲻwsp_2 = _cⲻwsp_2;
            this._ʺx29ʺ_1 = _ʺx29ʺ_1;
        }
        
        public Inners._ʺx28ʺ _ʺx28ʺ_1 { get; }
        public System.Collections.Generic.IEnumerable<__Generated.CstNodes.Rules._cⲻwsp> _cⲻwsp_1 { get; }
        public __Generated.CstNodes.Rules._alternation _alternation_1 { get; }
        public System.Collections.Generic.IEnumerable<__Generated.CstNodes.Rules._cⲻwsp> _cⲻwsp_2 { get; }
        public Inners._ʺx29ʺ _ʺx29ʺ_1 { get; }
    }
    
    public sealed class _option
    {
        public _option(Inners._ʺx5Bʺ _ʺx5Bʺ_1, System.Collections.Generic.IEnumerable<__Generated.CstNodes.Rules._cⲻwsp> _cⲻwsp_1, __Generated.CstNodes.Rules._alternation _alternation_1, System.Collections.Generic.IEnumerable<__Generated.CstNodes.Rules._cⲻwsp> _cⲻwsp_2, Inners._ʺx5Dʺ _ʺx5Dʺ_1)
        {
            this._ʺx5Bʺ_1 = _ʺx5Bʺ_1;
            this._cⲻwsp_1 = _cⲻwsp_1;
            this._alternation_1 = _alternation_1;
            this._cⲻwsp_2 = _cⲻwsp_2;
            this._ʺx5Dʺ_1 = _ʺx5Dʺ_1;
        }
        
        public Inners._ʺx5Bʺ _ʺx5Bʺ_1 { get; }
        public System.Collections.Generic.IEnumerable<__Generated.CstNodes.Rules._cⲻwsp> _cⲻwsp_1 { get; }
        public __Generated.CstNodes.Rules._alternation _alternation_1 { get; }
        public System.Collections.Generic.IEnumerable<__Generated.CstNodes.Rules._cⲻwsp> _cⲻwsp_2 { get; }
        public Inners._ʺx5Dʺ _ʺx5Dʺ_1 { get; }
    }
    
    public sealed class _charⲻval
    {
        public _charⲻval(__Generated.CstNodes.Rules._DQUOTE _DQUOTE_1, System.Collections.Generic.IEnumerable<Inners._ⲤⰃx20ⲻ21ⳆⰃx23ⲻ7EↃ> _ⲤⰃx20ⲻ21ⳆⰃx23ⲻ7EↃ_1, __Generated.CstNodes.Rules._DQUOTE _DQUOTE_2)
        {
            this._DQUOTE_1 = _DQUOTE_1;
            this._ⲤⰃx20ⲻ21ⳆⰃx23ⲻ7EↃ_1 = _ⲤⰃx20ⲻ21ⳆⰃx23ⲻ7EↃ_1;
            this._DQUOTE_2 = _DQUOTE_2;
        }
        
        public __Generated.CstNodes.Rules._DQUOTE _DQUOTE_1 { get; }
        public System.Collections.Generic.IEnumerable<Inners._ⲤⰃx20ⲻ21ⳆⰃx23ⲻ7EↃ> _ⲤⰃx20ⲻ21ⳆⰃx23ⲻ7EↃ_1 { get; }
        public __Generated.CstNodes.Rules._DQUOTE _DQUOTE_2 { get; }
    }
    
    public sealed class _numⲻval
    {
        public _numⲻval(Inners._ʺx25ʺ _ʺx25ʺ_1, Inners._ⲤbinⲻvalⳆdecⲻvalⳆhexⲻvalↃ _ⲤbinⲻvalⳆdecⲻvalⳆhexⲻvalↃ_1)
        {
            this._ʺx25ʺ_1 = _ʺx25ʺ_1;
            this._ⲤbinⲻvalⳆdecⲻvalⳆhexⲻvalↃ_1 = _ⲤbinⲻvalⳆdecⲻvalⳆhexⲻvalↃ_1;
        }
        
        public Inners._ʺx25ʺ _ʺx25ʺ_1 { get; }
        public Inners._ⲤbinⲻvalⳆdecⲻvalⳆhexⲻvalↃ _ⲤbinⲻvalⳆdecⲻvalⳆhexⲻvalↃ_1 { get; }
    }
    
    public sealed class _binⲻval
    {
        public _binⲻval(Inners._ʺx62ʺ _ʺx62ʺ_1, System.Collections.Generic.IEnumerable<__Generated.CstNodes.Rules._BIT> _BIT_1, Inners._1ЖⲤʺx2Eʺ_1ЖBITↃⳆⲤʺx2Dʺ_1ЖBITↃ? _1ЖⲤʺx2Eʺ_1ЖBITↃⳆⲤʺx2Dʺ_1ЖBITↃ_1)
        {
            this._ʺx62ʺ_1 = _ʺx62ʺ_1;
            this._BIT_1 = _BIT_1;
            this._1ЖⲤʺx2Eʺ_1ЖBITↃⳆⲤʺx2Dʺ_1ЖBITↃ_1 = _1ЖⲤʺx2Eʺ_1ЖBITↃⳆⲤʺx2Dʺ_1ЖBITↃ_1;
        }
        
        public Inners._ʺx62ʺ _ʺx62ʺ_1 { get; }
        public System.Collections.Generic.IEnumerable<__Generated.CstNodes.Rules._BIT> _BIT_1 { get; }
        public Inners._1ЖⲤʺx2Eʺ_1ЖBITↃⳆⲤʺx2Dʺ_1ЖBITↃ? _1ЖⲤʺx2Eʺ_1ЖBITↃⳆⲤʺx2Dʺ_1ЖBITↃ_1 { get; }
    }
    
    public sealed class _decⲻval
    {
        public _decⲻval(Inners._ʺx64ʺ _ʺx64ʺ_1, System.Collections.Generic.IEnumerable<__Generated.CstNodes.Rules._DIGIT> _DIGIT_1, Inners._1ЖⲤʺx2Eʺ_1ЖDIGITↃⳆⲤʺx2Dʺ_1ЖDIGITↃ? _1ЖⲤʺx2Eʺ_1ЖDIGITↃⳆⲤʺx2Dʺ_1ЖDIGITↃ_1)
        {
            this._ʺx64ʺ_1 = _ʺx64ʺ_1;
            this._DIGIT_1 = _DIGIT_1;
            this._1ЖⲤʺx2Eʺ_1ЖDIGITↃⳆⲤʺx2Dʺ_1ЖDIGITↃ_1 = _1ЖⲤʺx2Eʺ_1ЖDIGITↃⳆⲤʺx2Dʺ_1ЖDIGITↃ_1;
        }
        
        public Inners._ʺx64ʺ _ʺx64ʺ_1 { get; }
        public System.Collections.Generic.IEnumerable<__Generated.CstNodes.Rules._DIGIT> _DIGIT_1 { get; }
        public Inners._1ЖⲤʺx2Eʺ_1ЖDIGITↃⳆⲤʺx2Dʺ_1ЖDIGITↃ? _1ЖⲤʺx2Eʺ_1ЖDIGITↃⳆⲤʺx2Dʺ_1ЖDIGITↃ_1 { get; }
    }
    
    public sealed class _hexⲻval
    {
        public _hexⲻval(Inners._ʺx78ʺ _ʺx78ʺ_1, System.Collections.Generic.IEnumerable<__Generated.CstNodes.Rules._HEXDIG> _HEXDIG_1, Inners._1ЖⲤʺx2Eʺ_1ЖHEXDIGↃⳆⲤʺx2Dʺ_1ЖHEXDIGↃ? _1ЖⲤʺx2Eʺ_1ЖHEXDIGↃⳆⲤʺx2Dʺ_1ЖHEXDIGↃ_1)
        {
            this._ʺx78ʺ_1 = _ʺx78ʺ_1;
            this._HEXDIG_1 = _HEXDIG_1;
            this._1ЖⲤʺx2Eʺ_1ЖHEXDIGↃⳆⲤʺx2Dʺ_1ЖHEXDIGↃ_1 = _1ЖⲤʺx2Eʺ_1ЖHEXDIGↃⳆⲤʺx2Dʺ_1ЖHEXDIGↃ_1;
        }
        
        public Inners._ʺx78ʺ _ʺx78ʺ_1 { get; }
        public System.Collections.Generic.IEnumerable<__Generated.CstNodes.Rules._HEXDIG> _HEXDIG_1 { get; }
        public Inners._1ЖⲤʺx2Eʺ_1ЖHEXDIGↃⳆⲤʺx2Dʺ_1ЖHEXDIGↃ? _1ЖⲤʺx2Eʺ_1ЖHEXDIGↃⳆⲤʺx2Dʺ_1ЖHEXDIGↃ_1 { get; }
    }
    
    public sealed class _proseⲻval
    {
        public _proseⲻval(Inners._ʺx3Cʺ _ʺx3Cʺ_1, System.Collections.Generic.IEnumerable<Inners._ⲤⰃx20ⲻ3DⳆⰃx3Fⲻ7EↃ> _ⲤⰃx20ⲻ3DⳆⰃx3Fⲻ7EↃ_1, Inners._ʺx3Eʺ _ʺx3Eʺ_1)
        {
            this._ʺx3Cʺ_1 = _ʺx3Cʺ_1;
            this._ⲤⰃx20ⲻ3DⳆⰃx3Fⲻ7EↃ_1 = _ⲤⰃx20ⲻ3DⳆⰃx3Fⲻ7EↃ_1;
            this._ʺx3Eʺ_1 = _ʺx3Eʺ_1;
        }
        
        public Inners._ʺx3Cʺ _ʺx3Cʺ_1 { get; }
        public System.Collections.Generic.IEnumerable<Inners._ⲤⰃx20ⲻ3DⳆⰃx3Fⲻ7EↃ> _ⲤⰃx20ⲻ3DⳆⰃx3Fⲻ7EↃ_1 { get; }
        public Inners._ʺx3Eʺ _ʺx3Eʺ_1 { get; }
    }
    
}
