namespace __GeneratedOdataV3.Trancsribers.Rules
{
    public sealed class _primitiveColPathTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Rules._primitiveColPath>
    {
        private _primitiveColPathTranscriber()
        {
        }
        
        public static _primitiveColPathTranscriber Instance { get; } = new _primitiveColPathTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Rules._primitiveColPath value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV3.CstNodes.Rules._primitiveColPath.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._primitiveColPath._count node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._countTranscriber.Instance.Transcribe(node._count_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._primitiveColPath._boundOperation node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._boundOperationTranscriber.Instance.Transcribe(node._boundOperation_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._primitiveColPath._ordinalIndex node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._ordinalIndexTranscriber.Instance.Transcribe(node._ordinalIndex_1, context);

return default;
            }
        }
    }
    
}
