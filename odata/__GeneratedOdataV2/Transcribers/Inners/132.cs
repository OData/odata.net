namespace __GeneratedOdataV2.Trancsribers.Inners
{
    public sealed class _oneToNine_ЖDIGITⳆʺx6Dx61x78ʺTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV2.CstNodes.Inners._oneToNine_ЖDIGITⳆʺx6Dx61x78ʺ>
    {
        private _oneToNine_ЖDIGITⳆʺx6Dx61x78ʺTranscriber()
        {
        }
        
        public static _oneToNine_ЖDIGITⳆʺx6Dx61x78ʺTranscriber Instance { get; } = new _oneToNine_ЖDIGITⳆʺx6Dx61x78ʺTranscriber();
        
        public void Transcribe(__GeneratedOdataV2.CstNodes.Inners._oneToNine_ЖDIGITⳆʺx6Dx61x78ʺ value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV2.CstNodes.Inners._oneToNine_ЖDIGITⳆʺx6Dx61x78ʺ.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Inners._oneToNine_ЖDIGITⳆʺx6Dx61x78ʺ._oneToNine_ЖDIGIT node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Rules._oneToNineTranscriber.Instance.Transcribe(node._oneToNine_1, context);
foreach (var _DIGIT_1 in node._DIGIT_1)
{
__GeneratedOdataV2.Trancsribers.Rules._DIGITTranscriber.Instance.Transcribe(_DIGIT_1, context);
}

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Inners._oneToNine_ЖDIGITⳆʺx6Dx61x78ʺ._ʺx6Dx61x78ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Inners._ʺx6Dx61x78ʺTranscriber.Instance.Transcribe(node._ʺx6Dx61x78ʺ_1, context);

return default;
            }
        }
    }
    
}
