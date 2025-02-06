namespace __GeneratedOdataV4.Trancsribers.Inners
{
    public sealed class _base64b16Ⳇbase64b8Transcriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Inners._base64b16Ⳇbase64b8>
    {
        private _base64b16Ⳇbase64b8Transcriber()
        {
        }
        
        public static _base64b16Ⳇbase64b8Transcriber Instance { get; } = new _base64b16Ⳇbase64b8Transcriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Inners._base64b16Ⳇbase64b8 value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV4.CstNodes.Inners._base64b16Ⳇbase64b8.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Inners._base64b16Ⳇbase64b8._base64b16 node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._base64b16Transcriber.Instance.Transcribe(node._base64b16_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Inners._base64b16Ⳇbase64b8._base64b8 node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._base64b8Transcriber.Instance.Transcribe(node._base64b8_1, context);

return default;
            }
        }
    }
    
}
