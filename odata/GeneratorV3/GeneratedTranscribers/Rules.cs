namespace Test.Transcribers.Rules
{
    using System.Text;
    
    using GeneratorV3;
    using GeneratorV3.Abnf;
    
    public sealed class _ALPHATranscriber : ITranscriber<_ALPHA>
    {
        private _ALPHATranscriber()
        {
        }
        
        public static _ALPHATranscriber Instance { get; } = new _ALPHATranscriber();
        
        public void Transcribe(_ALPHA value, StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : GeneratorV3.Abnf._ALPHA.Visitor<Root.Void, StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(GeneratorV3.Abnf._ALPHA._Ⰳx41ⲻ5A node, StringBuilder context)
            {
                Test.Transcribers.Inners._Ⰳx41ⲻ5ATranscriber.Instance.Transcribe(node._Ⰳx41ⲻ5A_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(GeneratorV3.Abnf._ALPHA._Ⰳx61ⲻ7A node, StringBuilder context)
            {
                Test.Transcribers.Inners._Ⰳx61ⲻ7ATranscriber.Instance.Transcribe(node._Ⰳx61ⲻ7A_1, context);

return default;
            }
        }
    }
    
    public sealed class _BITTranscriber : ITranscriber<_BIT>
    {
        private _BITTranscriber()
        {
        }
        
        public static _BITTranscriber Instance { get; } = new _BITTranscriber();
        
        public void Transcribe(_BIT value, StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : GeneratorV3.Abnf._BIT.Visitor<Root.Void, StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(GeneratorV3.Abnf._BIT._ʺx30ʺ node, StringBuilder context)
            {
                Test.Transcribers.Inners._ʺx30ʺTranscriber.Instance.Transcribe(node._ʺx30ʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(GeneratorV3.Abnf._BIT._ʺx31ʺ node, StringBuilder context)
            {
                Test.Transcribers.Inners._ʺx31ʺTranscriber.Instance.Transcribe(node._ʺx31ʺ_1, context);

return default;
            }
        }
    }
    
    public sealed class _CHARTranscriber : ITranscriber<_CHAR>
    {
        private _CHARTranscriber()
        {
        }
        
        public static _CHARTranscriber Instance { get; } = new _CHARTranscriber();
        
        public void Transcribe(_CHAR value, StringBuilder builder)
        {
            Test.Transcribers.Inners._Ⰳx01ⲻ7FTranscriber.Instance.Transcribe(value._Ⰳx01ⲻ7F_1, builder);

        }
    }
    
    public sealed class _CRTranscriber : ITranscriber<_CR>
    {
        private _CRTranscriber()
        {
        }
        
        public static _CRTranscriber Instance { get; } = new _CRTranscriber();
        
        public void Transcribe(_CR value, StringBuilder builder)
        {
            Test.Transcribers.Inners._Ⰳx0DTranscriber.Instance.Transcribe(value._Ⰳx0D_1, builder);

        }
    }
    
    public sealed class _CRLFTranscriber : ITranscriber<_CRLF>
    {
        private _CRLFTranscriber()
        {
        }
        
        public static _CRLFTranscriber Instance { get; } = new _CRLFTranscriber();
        
        public void Transcribe(_CRLF value, StringBuilder builder)
        {
            Test.Transcribers.Rules._CRTranscriber.Instance.Transcribe(value._CR_1, builder);
Test.Transcribers.Rules._LFTranscriber.Instance.Transcribe(value._LF_1, builder);

        }
    }
    
    public sealed class _CTLTranscriber : ITranscriber<_CTL>
    {
        private _CTLTranscriber()
        {
        }
        
        public static _CTLTranscriber Instance { get; } = new _CTLTranscriber();
        
        public void Transcribe(_CTL value, StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : GeneratorV3.Abnf._CTL.Visitor<Root.Void, StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(GeneratorV3.Abnf._CTL._Ⰳx00ⲻ1F node, StringBuilder context)
            {
                Test.Transcribers.Inners._Ⰳx00ⲻ1FTranscriber.Instance.Transcribe(node._Ⰳx00ⲻ1F_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(GeneratorV3.Abnf._CTL._Ⰳx7F node, StringBuilder context)
            {
                Test.Transcribers.Inners._Ⰳx7FTranscriber.Instance.Transcribe(node._Ⰳx7F_1, context);

return default;
            }
        }
    }
    
    public sealed class _DIGITTranscriber : ITranscriber<_DIGIT>
    {
        private _DIGITTranscriber()
        {
        }
        
        public static _DIGITTranscriber Instance { get; } = new _DIGITTranscriber();
        
        public void Transcribe(_DIGIT value, StringBuilder builder)
        {
            Test.Transcribers.Inners._Ⰳx30ⲻ39Transcriber.Instance.Transcribe(value._Ⰳx30ⲻ39_1, builder);

        }
    }
    
    public sealed class _DQUOTETranscriber : ITranscriber<_DQUOTE>
    {
        private _DQUOTETranscriber()
        {
        }
        
        public static _DQUOTETranscriber Instance { get; } = new _DQUOTETranscriber();
        
        public void Transcribe(_DQUOTE value, StringBuilder builder)
        {
            Test.Transcribers.Inners._Ⰳx22Transcriber.Instance.Transcribe(value._Ⰳx22_1, builder);

        }
    }
    
    public sealed class _HEXDIGTranscriber : ITranscriber<_HEXDIG>
    {
        private _HEXDIGTranscriber()
        {
        }
        
        public static _HEXDIGTranscriber Instance { get; } = new _HEXDIGTranscriber();
        
        public void Transcribe(_HEXDIG value, StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : GeneratorV3.Abnf._HEXDIG.Visitor<Root.Void, StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(GeneratorV3.Abnf._HEXDIG._DIGIT node, StringBuilder context)
            {
                Test.Transcribers.Rules._DIGITTranscriber.Instance.Transcribe(node._DIGIT_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(GeneratorV3.Abnf._HEXDIG._ʺx41ʺ node, StringBuilder context)
            {
                Test.Transcribers.Inners._ʺx41ʺTranscriber.Instance.Transcribe(node._ʺx41ʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(GeneratorV3.Abnf._HEXDIG._ʺx42ʺ node, StringBuilder context)
            {
                Test.Transcribers.Inners._ʺx42ʺTranscriber.Instance.Transcribe(node._ʺx42ʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(GeneratorV3.Abnf._HEXDIG._ʺx43ʺ node, StringBuilder context)
            {
                Test.Transcribers.Inners._ʺx43ʺTranscriber.Instance.Transcribe(node._ʺx43ʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(GeneratorV3.Abnf._HEXDIG._ʺx44ʺ node, StringBuilder context)
            {
                Test.Transcribers.Inners._ʺx44ʺTranscriber.Instance.Transcribe(node._ʺx44ʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(GeneratorV3.Abnf._HEXDIG._ʺx45ʺ node, StringBuilder context)
            {
                Test.Transcribers.Inners._ʺx45ʺTranscriber.Instance.Transcribe(node._ʺx45ʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(GeneratorV3.Abnf._HEXDIG._ʺx46ʺ node, StringBuilder context)
            {
                Test.Transcribers.Inners._ʺx46ʺTranscriber.Instance.Transcribe(node._ʺx46ʺ_1, context);

return default;
            }
        }
    }
    
    public sealed class _HTABTranscriber : ITranscriber<_HTAB>
    {
        private _HTABTranscriber()
        {
        }
        
        public static _HTABTranscriber Instance { get; } = new _HTABTranscriber();
        
        public void Transcribe(_HTAB value, StringBuilder builder)
        {
            Test.Transcribers.Inners._Ⰳx09Transcriber.Instance.Transcribe(value._Ⰳx09_1, builder);

        }
    }
    
    public sealed class _LFTranscriber : ITranscriber<_LF>
    {
        private _LFTranscriber()
        {
        }
        
        public static _LFTranscriber Instance { get; } = new _LFTranscriber();
        
        public void Transcribe(_LF value, StringBuilder builder)
        {
            Test.Transcribers.Inners._Ⰳx0ATranscriber.Instance.Transcribe(value._Ⰳx0A_1, builder);

        }
    }
    
    public sealed class _LWSPTranscriber : ITranscriber<_LWSP>
    {
        private _LWSPTranscriber()
        {
        }
        
        public static _LWSPTranscriber Instance { get; } = new _LWSPTranscriber();
        
        public void Transcribe(_LWSP value, StringBuilder builder)
        {
            foreach (var _ⲤWSPⳆCRLF_WSPↃ_1 in value._ⲤWSPⳆCRLF_WSPↃ_1)
{
Test.Transcribers.Inners._ⲤWSPⳆCRLF_WSPↃTranscriber.Instance.Transcribe(_ⲤWSPⳆCRLF_WSPↃ_1, builder);
}

        }
    }
    
    public sealed class _OCTETTranscriber : ITranscriber<_OCTET>
    {
        private _OCTETTranscriber()
        {
        }
        
        public static _OCTETTranscriber Instance { get; } = new _OCTETTranscriber();
        
        public void Transcribe(_OCTET value, StringBuilder builder)
        {
            Test.Transcribers.Inners._Ⰳx00ⲻFFTranscriber.Instance.Transcribe(value._Ⰳx00ⲻFF_1, builder);

        }
    }
    
    public sealed class _SPTranscriber : ITranscriber<_SP>
    {
        private _SPTranscriber()
        {
        }
        
        public static _SPTranscriber Instance { get; } = new _SPTranscriber();
        
        public void Transcribe(_SP value, StringBuilder builder)
        {
            Test.Transcribers.Inners._Ⰳx20Transcriber.Instance.Transcribe(value._Ⰳx20_1, builder);

        }
    }
    
    public sealed class _VCHARTranscriber : ITranscriber<_VCHAR>
    {
        private _VCHARTranscriber()
        {
        }
        
        public static _VCHARTranscriber Instance { get; } = new _VCHARTranscriber();
        
        public void Transcribe(_VCHAR value, StringBuilder builder)
        {
            Test.Transcribers.Inners._Ⰳx21ⲻ7ETranscriber.Instance.Transcribe(value._Ⰳx21ⲻ7E_1, builder);

        }
    }
    
    public sealed class _WSPTranscriber : ITranscriber<_WSP>
    {
        private _WSPTranscriber()
        {
        }
        
        public static _WSPTranscriber Instance { get; } = new _WSPTranscriber();
        
        public void Transcribe(_WSP value, StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : GeneratorV3.Abnf._WSP.Visitor<Root.Void, StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(GeneratorV3.Abnf._WSP._SP node, StringBuilder context)
            {
                Test.Transcribers.Rules._SPTranscriber.Instance.Transcribe(node._SP_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(GeneratorV3.Abnf._WSP._HTAB node, StringBuilder context)
            {
                Test.Transcribers.Rules._HTABTranscriber.Instance.Transcribe(node._HTAB_1, context);

return default;
            }
        }
    }
    
    public sealed class _rulelistTranscriber : ITranscriber<_rulelist>
    {
        private _rulelistTranscriber()
        {
        }
        
        public static _rulelistTranscriber Instance { get; } = new _rulelistTranscriber();
        
        public void Transcribe(_rulelist value, StringBuilder builder)
        {
            foreach (var _ⲤruleⳆⲤЖcⲻwsp_cⲻnlↃↃ_1 in value._ⲤruleⳆⲤЖcⲻwsp_cⲻnlↃↃ_1)
{
Test.Transcribers.Inners._ⲤruleⳆⲤЖcⲻwsp_cⲻnlↃↃTranscriber.Instance.Transcribe(_ⲤruleⳆⲤЖcⲻwsp_cⲻnlↃↃ_1, builder);
}

        }
    }
    
    public sealed class _ruleTranscriber : ITranscriber<_rule>
    {
        private _ruleTranscriber()
        {
        }
        
        public static _ruleTranscriber Instance { get; } = new _ruleTranscriber();
        
        public void Transcribe(_rule value, StringBuilder builder)
        {
            Test.Transcribers.Rules._rulenameTranscriber.Instance.Transcribe(value._rulename_1, builder);
Test.Transcribers.Rules._definedⲻasTranscriber.Instance.Transcribe(value._definedⲻas_1, builder);
Test.Transcribers.Rules._elementsTranscriber.Instance.Transcribe(value._elements_1, builder);
Test.Transcribers.Rules._cⲻnlTranscriber.Instance.Transcribe(value._cⲻnl_1, builder);

        }
    }
    
    public sealed class _rulenameTranscriber : ITranscriber<_rulename>
    {
        private _rulenameTranscriber()
        {
        }
        
        public static _rulenameTranscriber Instance { get; } = new _rulenameTranscriber();
        
        public void Transcribe(_rulename value, StringBuilder builder)
        {
            Test.Transcribers.Rules._ALPHATranscriber.Instance.Transcribe(value._ALPHA_1, builder);
foreach (var _ⲤALPHAⳆDIGITⳆʺx2DʺↃ_1 in value._ⲤALPHAⳆDIGITⳆʺx2DʺↃ_1)
{
Test.Transcribers.Inners._ⲤALPHAⳆDIGITⳆʺx2DʺↃTranscriber.Instance.Transcribe(_ⲤALPHAⳆDIGITⳆʺx2DʺↃ_1, builder);
}

        }
    }
    
    public sealed class _definedⲻasTranscriber : ITranscriber<_definedⲻas>
    {
        private _definedⲻasTranscriber()
        {
        }
        
        public static _definedⲻasTranscriber Instance { get; } = new _definedⲻasTranscriber();
        
        public void Transcribe(_definedⲻas value, StringBuilder builder)
        {
            foreach (var _cⲻwsp_1 in value._cⲻwsp_1)
{
Test.Transcribers.Rules._cⲻwspTranscriber.Instance.Transcribe(_cⲻwsp_1, builder);
}
Test.Transcribers.Inners._Ⲥʺx3DʺⳆʺx3Dx2FʺↃTranscriber.Instance.Transcribe(value._Ⲥʺx3DʺⳆʺx3Dx2FʺↃ_1, builder);
foreach (var _cⲻwsp_2 in value._cⲻwsp_2)
{
Test.Transcribers.Rules._cⲻwspTranscriber.Instance.Transcribe(_cⲻwsp_2, builder);
}

        }
    }
    
    public sealed class _elementsTranscriber : ITranscriber<_elements>
    {
        private _elementsTranscriber()
        {
        }
        
        public static _elementsTranscriber Instance { get; } = new _elementsTranscriber();
        
        public void Transcribe(_elements value, StringBuilder builder)
        {
            Test.Transcribers.Rules._alternationTranscriber.Instance.Transcribe(value._alternation_1, builder);
foreach (var _cⲻwsp_1 in value._cⲻwsp_1)
{
Test.Transcribers.Rules._cⲻwspTranscriber.Instance.Transcribe(_cⲻwsp_1, builder);
}

        }
    }
    
    public sealed class _cⲻwspTranscriber : ITranscriber<_cⲻwsp>
    {
        private _cⲻwspTranscriber()
        {
        }
        
        public static _cⲻwspTranscriber Instance { get; } = new _cⲻwspTranscriber();
        
        public void Transcribe(_cⲻwsp value, StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : GeneratorV3.Abnf._cⲻwsp.Visitor<Root.Void, StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(GeneratorV3.Abnf._cⲻwsp._WSP node, StringBuilder context)
            {
                Test.Transcribers.Rules._WSPTranscriber.Instance.Transcribe(node._WSP_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(GeneratorV3.Abnf._cⲻwsp._Ⲥcⲻnl_WSPↃ node, StringBuilder context)
            {
                Test.Transcribers.Inners._Ⲥcⲻnl_WSPↃTranscriber.Instance.Transcribe(node._Ⲥcⲻnl_WSPↃ_1, context);

return default;
            }
        }
    }
    
    public sealed class _cⲻnlTranscriber : ITranscriber<_cⲻnl>
    {
        private _cⲻnlTranscriber()
        {
        }
        
        public static _cⲻnlTranscriber Instance { get; } = new _cⲻnlTranscriber();
        
        public void Transcribe(_cⲻnl value, StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : GeneratorV3.Abnf._cⲻnl.Visitor<Root.Void, StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(GeneratorV3.Abnf._cⲻnl._comment node, StringBuilder context)
            {
                Test.Transcribers.Rules._commentTranscriber.Instance.Transcribe(node._comment_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(GeneratorV3.Abnf._cⲻnl._CRLF node, StringBuilder context)
            {
                Test.Transcribers.Rules._CRLFTranscriber.Instance.Transcribe(node._CRLF_1, context);

return default;
            }
        }
    }
    
    public sealed class _commentTranscriber : ITranscriber<_comment>
    {
        private _commentTranscriber()
        {
        }
        
        public static _commentTranscriber Instance { get; } = new _commentTranscriber();
        
        public void Transcribe(_comment value, StringBuilder builder)
        {
            Test.Transcribers.Inners._ʺx3BʺTranscriber.Instance.Transcribe(value._ʺx3Bʺ_1, builder);
foreach (var _ⲤWSPⳆVCHARↃ_1 in value._ⲤWSPⳆVCHARↃ_1)
{
Test.Transcribers.Inners._ⲤWSPⳆVCHARↃTranscriber.Instance.Transcribe(_ⲤWSPⳆVCHARↃ_1, builder);
}
Test.Transcribers.Rules._CRLFTranscriber.Instance.Transcribe(value._CRLF_1, builder);

        }
    }
    
    public sealed class _alternationTranscriber : ITranscriber<_alternation>
    {
        private _alternationTranscriber()
        {
        }
        
        public static _alternationTranscriber Instance { get; } = new _alternationTranscriber();
        
        public void Transcribe(_alternation value, StringBuilder builder)
        {
            Test.Transcribers.Rules._concatenationTranscriber.Instance.Transcribe(value._concatenation_1, builder);
foreach (var _ⲤЖcⲻwsp_ʺx2Fʺ_Жcⲻwsp_concatenationↃ_1 in value._ⲤЖcⲻwsp_ʺx2Fʺ_Жcⲻwsp_concatenationↃ_1)
{
Test.Transcribers.Inners._ⲤЖcⲻwsp_ʺx2Fʺ_Жcⲻwsp_concatenationↃTranscriber.Instance.Transcribe(_ⲤЖcⲻwsp_ʺx2Fʺ_Жcⲻwsp_concatenationↃ_1, builder);
}

        }
    }
    
    public sealed class _concatenationTranscriber : ITranscriber<_concatenation>
    {
        private _concatenationTranscriber()
        {
        }
        
        public static _concatenationTranscriber Instance { get; } = new _concatenationTranscriber();
        
        public void Transcribe(_concatenation value, StringBuilder builder)
        {
            Test.Transcribers.Rules._repetitionTranscriber.Instance.Transcribe(value._repetition_1, builder);
foreach (var _Ⲥ1Жcⲻwsp_repetitionↃ_1 in value._Ⲥ1Жcⲻwsp_repetitionↃ_1)
{
Test.Transcribers.Inners._Ⲥ1Жcⲻwsp_repetitionↃTranscriber.Instance.Transcribe(_Ⲥ1Жcⲻwsp_repetitionↃ_1, builder);
}

        }
    }
    
    public sealed class _repetitionTranscriber : ITranscriber<_repetition>
    {
        private _repetitionTranscriber()
        {
        }
        
        public static _repetitionTranscriber Instance { get; } = new _repetitionTranscriber();
        
        public void Transcribe(_repetition value, StringBuilder builder)
        {
            if (value._repeat_1 != null)
{
Test.Transcribers.Rules._repeatTranscriber.Instance.Transcribe(value._repeat_1, builder);
}
Test.Transcribers.Rules._elementTranscriber.Instance.Transcribe(value._element_1, builder);

        }
    }
    
    public sealed class _repeatTranscriber : ITranscriber<_repeat>
    {
        private _repeatTranscriber()
        {
        }
        
        public static _repeatTranscriber Instance { get; } = new _repeatTranscriber();
        
        public void Transcribe(_repeat value, StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : GeneratorV3.Abnf._repeat.Visitor<Root.Void, StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(GeneratorV3.Abnf._repeat._1ЖDIGIT node, StringBuilder context)
            {
                foreach (var _DIGIT_1 in node._DIGIT_1)
{
Test.Transcribers.Rules._DIGITTranscriber.Instance.Transcribe(_DIGIT_1, context);
}

return default;
            }
            
            protected internal override Root.Void Accept(GeneratorV3.Abnf._repeat._ⲤЖDIGIT_ʺx2Aʺ_ЖDIGITↃ node, StringBuilder context)
            {
                Test.Transcribers.Inners._ⲤЖDIGIT_ʺx2Aʺ_ЖDIGITↃTranscriber.Instance.Transcribe(node._ⲤЖDIGIT_ʺx2Aʺ_ЖDIGITↃ_1, context);

return default;
            }
        }
    }
    
    public sealed class _elementTranscriber : ITranscriber<_element>
    {
        private _elementTranscriber()
        {
        }
        
        public static _elementTranscriber Instance { get; } = new _elementTranscriber();
        
        public void Transcribe(_element value, StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : GeneratorV3.Abnf._element.Visitor<Root.Void, StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(GeneratorV3.Abnf._element._rulename node, StringBuilder context)
            {
                Test.Transcribers.Rules._rulenameTranscriber.Instance.Transcribe(node._rulename_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(GeneratorV3.Abnf._element._group node, StringBuilder context)
            {
                Test.Transcribers.Rules._groupTranscriber.Instance.Transcribe(node._group_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(GeneratorV3.Abnf._element._option node, StringBuilder context)
            {
                Test.Transcribers.Rules._optionTranscriber.Instance.Transcribe(node._option_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(GeneratorV3.Abnf._element._charⲻval node, StringBuilder context)
            {
                Test.Transcribers.Rules._charⲻvalTranscriber.Instance.Transcribe(node._charⲻval_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(GeneratorV3.Abnf._element._numⲻval node, StringBuilder context)
            {
                Test.Transcribers.Rules._numⲻvalTranscriber.Instance.Transcribe(node._numⲻval_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(GeneratorV3.Abnf._element._proseⲻval node, StringBuilder context)
            {
                Test.Transcribers.Rules._proseⲻvalTranscriber.Instance.Transcribe(node._proseⲻval_1, context);

return default;
            }
        }
    }
    
    public sealed class _groupTranscriber : ITranscriber<_group>
    {
        private _groupTranscriber()
        {
        }
        
        public static _groupTranscriber Instance { get; } = new _groupTranscriber();
        
        public void Transcribe(_group value, StringBuilder builder)
        {
            Test.Transcribers.Inners._ʺx28ʺTranscriber.Instance.Transcribe(value._ʺx28ʺ_1, builder);
foreach (var _cⲻwsp_1 in value._cⲻwsp_1)
{
Test.Transcribers.Rules._cⲻwspTranscriber.Instance.Transcribe(_cⲻwsp_1, builder);
}
Test.Transcribers.Rules._alternationTranscriber.Instance.Transcribe(value._alternation_1, builder);
foreach (var _cⲻwsp_2 in value._cⲻwsp_2)
{
Test.Transcribers.Rules._cⲻwspTranscriber.Instance.Transcribe(_cⲻwsp_2, builder);
}
Test.Transcribers.Inners._ʺx29ʺTranscriber.Instance.Transcribe(value._ʺx29ʺ_1, builder);

        }
    }
    
    public sealed class _optionTranscriber : ITranscriber<_option>
    {
        private _optionTranscriber()
        {
        }
        
        public static _optionTranscriber Instance { get; } = new _optionTranscriber();
        
        public void Transcribe(_option value, StringBuilder builder)
        {
            Test.Transcribers.Inners._ʺx5BʺTranscriber.Instance.Transcribe(value._ʺx5Bʺ_1, builder);
foreach (var _cⲻwsp_1 in value._cⲻwsp_1)
{
Test.Transcribers.Rules._cⲻwspTranscriber.Instance.Transcribe(_cⲻwsp_1, builder);
}
Test.Transcribers.Rules._alternationTranscriber.Instance.Transcribe(value._alternation_1, builder);
foreach (var _cⲻwsp_2 in value._cⲻwsp_2)
{
Test.Transcribers.Rules._cⲻwspTranscriber.Instance.Transcribe(_cⲻwsp_2, builder);
}
Test.Transcribers.Inners._ʺx5DʺTranscriber.Instance.Transcribe(value._ʺx5Dʺ_1, builder);

        }
    }
    
    public sealed class _charⲻvalTranscriber : ITranscriber<_charⲻval>
    {
        private _charⲻvalTranscriber()
        {
        }
        
        public static _charⲻvalTranscriber Instance { get; } = new _charⲻvalTranscriber();
        
        public void Transcribe(_charⲻval value, StringBuilder builder)
        {
            Test.Transcribers.Rules._DQUOTETranscriber.Instance.Transcribe(value._DQUOTE_1, builder);
foreach (var _ⲤⰃx20ⲻ21ⳆⰃx23ⲻ7EↃ_1 in value._ⲤⰃx20ⲻ21ⳆⰃx23ⲻ7EↃ_1)
{
Test.Transcribers.Inners._ⲤⰃx20ⲻ21ⳆⰃx23ⲻ7EↃTranscriber.Instance.Transcribe(_ⲤⰃx20ⲻ21ⳆⰃx23ⲻ7EↃ_1, builder);
}
Test.Transcribers.Rules._DQUOTETranscriber.Instance.Transcribe(value._DQUOTE_2, builder);

        }
    }
    
    public sealed class _numⲻvalTranscriber : ITranscriber<_numⲻval>
    {
        private _numⲻvalTranscriber()
        {
        }
        
        public static _numⲻvalTranscriber Instance { get; } = new _numⲻvalTranscriber();
        
        public void Transcribe(_numⲻval value, StringBuilder builder)
        {
            Test.Transcribers.Inners._ʺx25ʺTranscriber.Instance.Transcribe(value._ʺx25ʺ_1, builder);
Test.Transcribers.Inners._ⲤbinⲻvalⳆdecⲻvalⳆhexⲻvalↃTranscriber.Instance.Transcribe(value._ⲤbinⲻvalⳆdecⲻvalⳆhexⲻvalↃ_1, builder);

        }
    }
    
    public sealed class _binⲻvalTranscriber : ITranscriber<_binⲻval>
    {
        private _binⲻvalTranscriber()
        {
        }
        
        public static _binⲻvalTranscriber Instance { get; } = new _binⲻvalTranscriber();
        
        public void Transcribe(_binⲻval value, StringBuilder builder)
        {
            Test.Transcribers.Inners._ʺx62ʺTranscriber.Instance.Transcribe(value._ʺx62ʺ_1, builder);
foreach (var _BIT_1 in value._BIT_1)
{
Test.Transcribers.Rules._BITTranscriber.Instance.Transcribe(_BIT_1, builder);
}
if (value._1ЖⲤʺx2Eʺ_1ЖBITↃⳆⲤʺx2Dʺ_1ЖBITↃ_1 != null)
{
Test.Transcribers.Inners._1ЖⲤʺx2Eʺ_1ЖBITↃⳆⲤʺx2Dʺ_1ЖBITↃTranscriber.Instance.Transcribe(value._1ЖⲤʺx2Eʺ_1ЖBITↃⳆⲤʺx2Dʺ_1ЖBITↃ_1, builder);
}

        }
    }
    
    public sealed class _decⲻvalTranscriber : ITranscriber<_decⲻval>
    {
        private _decⲻvalTranscriber()
        {
        }
        
        public static _decⲻvalTranscriber Instance { get; } = new _decⲻvalTranscriber();
        
        public void Transcribe(_decⲻval value, StringBuilder builder)
        {
            Test.Transcribers.Inners._ʺx64ʺTranscriber.Instance.Transcribe(value._ʺx64ʺ_1, builder);
foreach (var _DIGIT_1 in value._DIGIT_1)
{
Test.Transcribers.Rules._DIGITTranscriber.Instance.Transcribe(_DIGIT_1, builder);
}
if (value._1ЖⲤʺx2Eʺ_1ЖDIGITↃⳆⲤʺx2Dʺ_1ЖDIGITↃ_1 != null)
{
Test.Transcribers.Inners._1ЖⲤʺx2Eʺ_1ЖDIGITↃⳆⲤʺx2Dʺ_1ЖDIGITↃTranscriber.Instance.Transcribe(value._1ЖⲤʺx2Eʺ_1ЖDIGITↃⳆⲤʺx2Dʺ_1ЖDIGITↃ_1, builder);
}

        }
    }
    
    public sealed class _hexⲻvalTranscriber : ITranscriber<_hexⲻval>
    {
        private _hexⲻvalTranscriber()
        {
        }
        
        public static _hexⲻvalTranscriber Instance { get; } = new _hexⲻvalTranscriber();
        
        public void Transcribe(_hexⲻval value, StringBuilder builder)
        {
            Test.Transcribers.Inners._ʺx78ʺTranscriber.Instance.Transcribe(value._ʺx78ʺ_1, builder);
foreach (var _HEXDIG_1 in value._HEXDIG_1)
{
Test.Transcribers.Rules._HEXDIGTranscriber.Instance.Transcribe(_HEXDIG_1, builder);
}
if (value._1ЖⲤʺx2Eʺ_1ЖHEXDIGↃⳆⲤʺx2Dʺ_1ЖHEXDIGↃ_1 != null)
{
Test.Transcribers.Inners._1ЖⲤʺx2Eʺ_1ЖHEXDIGↃⳆⲤʺx2Dʺ_1ЖHEXDIGↃTranscriber.Instance.Transcribe(value._1ЖⲤʺx2Eʺ_1ЖHEXDIGↃⳆⲤʺx2Dʺ_1ЖHEXDIGↃ_1, builder);
}

        }
    }
    
    public sealed class _proseⲻvalTranscriber : ITranscriber<_proseⲻval>
    {
        private _proseⲻvalTranscriber()
        {
        }
        
        public static _proseⲻvalTranscriber Instance { get; } = new _proseⲻvalTranscriber();
        
        public void Transcribe(_proseⲻval value, StringBuilder builder)
        {
            Test.Transcribers.Inners._ʺx3CʺTranscriber.Instance.Transcribe(value._ʺx3Cʺ_1, builder);
foreach (var _ⲤⰃx20ⲻ3DⳆⰃx3Fⲻ7EↃ_1 in value._ⲤⰃx20ⲻ3DⳆⰃx3Fⲻ7EↃ_1)
{
Test.Transcribers.Inners._ⲤⰃx20ⲻ3DⳆⰃx3Fⲻ7EↃTranscriber.Instance.Transcribe(_ⲤⰃx20ⲻ3DⳆⰃx3Fⲻ7EↃ_1, builder);
}
Test.Transcribers.Inners._ʺx3EʺTranscriber.Instance.Transcribe(value._ʺx3Eʺ_1, builder);

        }
    }
    
}