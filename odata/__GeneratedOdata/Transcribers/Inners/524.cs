namespace __GeneratedOdata.Trancsribers.Inners
{
    public sealed class _ʺx30ʺ_3DIGITⳆoneToNine_3ЖDIGITTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Inners._ʺx30ʺ_3DIGITⳆoneToNine_3ЖDIGIT>
    {
        private _ʺx30ʺ_3DIGITⳆoneToNine_3ЖDIGITTranscriber()
        {
        }
        
        public static _ʺx30ʺ_3DIGITⳆoneToNine_3ЖDIGITTranscriber Instance { get; } = new _ʺx30ʺ_3DIGITⳆoneToNine_3ЖDIGITTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Inners._ʺx30ʺ_3DIGITⳆoneToNine_3ЖDIGIT value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdata.CstNodes.Inners._ʺx30ʺ_3DIGITⳆoneToNine_3ЖDIGIT.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Inners._ʺx30ʺ_3DIGITⳆoneToNine_3ЖDIGIT._ʺx30ʺ_3DIGIT node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Inners._ʺx30ʺTranscriber.Instance.Transcribe(node._ʺx30ʺ_1, context);
foreach (var _DIGIT_1 in node._DIGIT_1)
{
__GeneratedOdata.Trancsribers.Rules._DIGITTranscriber.Instance.Transcribe(_DIGIT_1, context);
}

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Inners._ʺx30ʺ_3DIGITⳆoneToNine_3ЖDIGIT._oneToNine_3ЖDIGIT node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._oneToNineTranscriber.Instance.Transcribe(node._oneToNine_1, context);
foreach (var _DIGIT_1 in node._DIGIT_1)
{
__GeneratedOdata.Trancsribers.Rules._DIGITTranscriber.Instance.Transcribe(_DIGIT_1, context);
}

return default;
            }
        }
    }
    
}
