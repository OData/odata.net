namespace __GeneratedOdataV2.Trancsribers.Inners
{
    public sealed class _parameterAliasⳆprimitiveLiteralTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV2.CstNodes.Inners._parameterAliasⳆprimitiveLiteral>
    {
        private _parameterAliasⳆprimitiveLiteralTranscriber()
        {
        }
        
        public static _parameterAliasⳆprimitiveLiteralTranscriber Instance { get; } = new _parameterAliasⳆprimitiveLiteralTranscriber();
        
        public void Transcribe(__GeneratedOdataV2.CstNodes.Inners._parameterAliasⳆprimitiveLiteral value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV2.CstNodes.Inners._parameterAliasⳆprimitiveLiteral.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Inners._parameterAliasⳆprimitiveLiteral._parameterAlias node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Rules._parameterAliasTranscriber.Instance.Transcribe(node._parameterAlias_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Inners._parameterAliasⳆprimitiveLiteral._primitiveLiteral node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Rules._primitiveLiteralTranscriber.Instance.Transcribe(node._primitiveLiteral_1, context);

return default;
            }
        }
    }
    
}
