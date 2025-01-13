namespace __GeneratedOdata.Trancsribers.Inners
{
    public sealed class _base64b16Ⳇbase64b8Transcriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Inners._base64b16Ⳇbase64b8>
    {
        private _base64b16Ⳇbase64b8Transcriber()
        {
        }
        
        public static _base64b16Ⳇbase64b8Transcriber Instance { get; } = new _base64b16Ⳇbase64b8Transcriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Inners._base64b16Ⳇbase64b8 value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdata.CstNodes.Inners._base64b16Ⳇbase64b8.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Inners._base64b16Ⳇbase64b8._base64b16 node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._base64b16Transcriber.Instance.Transcribe(node._base64b16_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Inners._base64b16Ⳇbase64b8._base64b8 node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._base64b8Transcriber.Instance.Transcribe(node._base64b8_1, context);

return default;
            }
        }
    }
    
}
