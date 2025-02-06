namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _decⲻoctetTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._decⲻoctet>
    {
        private _decⲻoctetTranscriber()
        {
        }
        
        public static _decⲻoctetTranscriber Instance { get; } = new _decⲻoctetTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._decⲻoctet value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV4.CstNodes.Rules._decⲻoctet.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._decⲻoctet._ʺx31ʺ_2DIGIT node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Inners._ʺx31ʺTranscriber.Instance.Transcribe(node._ʺx31ʺ_1, context);
foreach (var _DIGIT_1 in node._DIGIT_1)
{
__GeneratedOdataV4.Trancsribers.Rules._DIGITTranscriber.Instance.Transcribe(_DIGIT_1, context);
}

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._decⲻoctet._ʺx32ʺ_Ⰳx30ⲻ34_DIGIT node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Inners._ʺx32ʺTranscriber.Instance.Transcribe(node._ʺx32ʺ_1, context);
__GeneratedOdataV4.Trancsribers.Inners._Ⰳx30ⲻ34Transcriber.Instance.Transcribe(node._Ⰳx30ⲻ34_1, context);
__GeneratedOdataV4.Trancsribers.Rules._DIGITTranscriber.Instance.Transcribe(node._DIGIT_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._decⲻoctet._ʺx32x35ʺ_Ⰳx30ⲻ35 node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Inners._ʺx32x35ʺTranscriber.Instance.Transcribe(node._ʺx32x35ʺ_1, context);
__GeneratedOdataV4.Trancsribers.Inners._Ⰳx30ⲻ35Transcriber.Instance.Transcribe(node._Ⰳx30ⲻ35_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._decⲻoctet._Ⰳx31ⲻ39_DIGIT node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Inners._Ⰳx31ⲻ39Transcriber.Instance.Transcribe(node._Ⰳx31ⲻ39_1, context);
__GeneratedOdataV4.Trancsribers.Rules._DIGITTranscriber.Instance.Transcribe(node._DIGIT_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._decⲻoctet._DIGIT node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._DIGITTranscriber.Instance.Transcribe(node._DIGIT_1, context);

return default;
            }
        }
    }
    
}
