namespace __GeneratedOdataV3.Trancsribers.Inners
{
    public sealed class _DIGITⳆʺx41ʺⳆʺx42ʺⳆʺx44ʺⳆʺx45ʺⳆʺx46ʺTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Inners._DIGITⳆʺx41ʺⳆʺx42ʺⳆʺx44ʺⳆʺx45ʺⳆʺx46ʺ>
    {
        private _DIGITⳆʺx41ʺⳆʺx42ʺⳆʺx44ʺⳆʺx45ʺⳆʺx46ʺTranscriber()
        {
        }
        
        public static _DIGITⳆʺx41ʺⳆʺx42ʺⳆʺx44ʺⳆʺx45ʺⳆʺx46ʺTranscriber Instance { get; } = new _DIGITⳆʺx41ʺⳆʺx42ʺⳆʺx44ʺⳆʺx45ʺⳆʺx46ʺTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Inners._DIGITⳆʺx41ʺⳆʺx42ʺⳆʺx44ʺⳆʺx45ʺⳆʺx46ʺ value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV3.CstNodes.Inners._DIGITⳆʺx41ʺⳆʺx42ʺⳆʺx44ʺⳆʺx45ʺⳆʺx46ʺ.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Inners._DIGITⳆʺx41ʺⳆʺx42ʺⳆʺx44ʺⳆʺx45ʺⳆʺx46ʺ._DIGIT node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._DIGITTranscriber.Instance.Transcribe(node._DIGIT_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Inners._DIGITⳆʺx41ʺⳆʺx42ʺⳆʺx44ʺⳆʺx45ʺⳆʺx46ʺ._ʺx41ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Inners._ʺx41ʺTranscriber.Instance.Transcribe(node._ʺx41ʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Inners._DIGITⳆʺx41ʺⳆʺx42ʺⳆʺx44ʺⳆʺx45ʺⳆʺx46ʺ._ʺx42ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Inners._ʺx42ʺTranscriber.Instance.Transcribe(node._ʺx42ʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Inners._DIGITⳆʺx41ʺⳆʺx42ʺⳆʺx44ʺⳆʺx45ʺⳆʺx46ʺ._ʺx44ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Inners._ʺx44ʺTranscriber.Instance.Transcribe(node._ʺx44ʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Inners._DIGITⳆʺx41ʺⳆʺx42ʺⳆʺx44ʺⳆʺx45ʺⳆʺx46ʺ._ʺx45ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Inners._ʺx45ʺTranscriber.Instance.Transcribe(node._ʺx45ʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Inners._DIGITⳆʺx41ʺⳆʺx42ʺⳆʺx44ʺⳆʺx45ʺⳆʺx46ʺ._ʺx46ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Inners._ʺx46ʺTranscriber.Instance.Transcribe(node._ʺx46ʺ_1, context);

return default;
            }
        }
    }
    
}
