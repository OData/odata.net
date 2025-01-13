namespace __GeneratedOdata.Trancsribers.Inners
{
    public sealed class _parameterAliasⳆprimitiveLiteralTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Inners._parameterAliasⳆprimitiveLiteral>
    {
        private _parameterAliasⳆprimitiveLiteralTranscriber()
        {
        }
        
        public static _parameterAliasⳆprimitiveLiteralTranscriber Instance { get; } = new _parameterAliasⳆprimitiveLiteralTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Inners._parameterAliasⳆprimitiveLiteral value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdata.CstNodes.Inners._parameterAliasⳆprimitiveLiteral.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Inners._parameterAliasⳆprimitiveLiteral._parameterAlias node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._parameterAliasTranscriber.Instance.Transcribe(node._parameterAlias_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Inners._parameterAliasⳆprimitiveLiteral._primitiveLiteral node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._primitiveLiteralTranscriber.Instance.Transcribe(node._primitiveLiteral_1, context);

return default;
            }
        }
    }
    
}
