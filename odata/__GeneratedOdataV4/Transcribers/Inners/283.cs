namespace __GeneratedOdataV4.Trancsribers.Inners
{
    public sealed class _parameterAliasⳆparameterValueTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Inners._parameterAliasⳆparameterValue>
    {
        private _parameterAliasⳆparameterValueTranscriber()
        {
        }
        
        public static _parameterAliasⳆparameterValueTranscriber Instance { get; } = new _parameterAliasⳆparameterValueTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Inners._parameterAliasⳆparameterValue value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV4.CstNodes.Inners._parameterAliasⳆparameterValue.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Inners._parameterAliasⳆparameterValue._parameterAlias node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._parameterAliasTranscriber.Instance.Transcribe(node._parameterAlias_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Inners._parameterAliasⳆparameterValue._parameterValue node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._parameterValueTranscriber.Instance.Transcribe(node._parameterValue_1, context);

return default;
            }
        }
    }
    
}
