namespace __GeneratedOdataV2.Trancsribers.Rules
{
    public sealed class _entityCastOptionTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV2.CstNodes.Rules._entityCastOption>
    {
        private _entityCastOptionTranscriber()
        {
        }
        
        public static _entityCastOptionTranscriber Instance { get; } = new _entityCastOptionTranscriber();
        
        public void Transcribe(__GeneratedOdataV2.CstNodes.Rules._entityCastOption value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV2.CstNodes.Rules._entityCastOption.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Rules._entityCastOption._entityIdOption node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Rules._entityIdOptionTranscriber.Instance.Transcribe(node._entityIdOption_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Rules._entityCastOption._expand node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Rules._expandTranscriber.Instance.Transcribe(node._expand_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Rules._entityCastOption._select node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Rules._selectTranscriber.Instance.Transcribe(node._select_1, context);

return default;
            }
        }
    }
    
}
