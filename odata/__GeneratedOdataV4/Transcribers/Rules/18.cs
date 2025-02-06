namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _primitivePathTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._primitivePath>
    {
        private _primitivePathTranscriber()
        {
        }
        
        public static _primitivePathTranscriber Instance { get; } = new _primitivePathTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._primitivePath value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV4.CstNodes.Rules._primitivePath.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._primitivePath._value node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._valueTranscriber.Instance.Transcribe(node._value_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._primitivePath._boundOperation node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._boundOperationTranscriber.Instance.Transcribe(node._boundOperation_1, context);

return default;
            }
        }
    }
    
}
