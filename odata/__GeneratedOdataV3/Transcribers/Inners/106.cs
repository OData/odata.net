namespace __GeneratedOdataV3.Trancsribers.Inners
{
    public sealed class _refⳆOPEN_levels_CLOSETranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Inners._refⳆOPEN_levels_CLOSE>
    {
        private _refⳆOPEN_levels_CLOSETranscriber()
        {
        }
        
        public static _refⳆOPEN_levels_CLOSETranscriber Instance { get; } = new _refⳆOPEN_levels_CLOSETranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Inners._refⳆOPEN_levels_CLOSE value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV3.CstNodes.Inners._refⳆOPEN_levels_CLOSE.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Inners._refⳆOPEN_levels_CLOSE._ref node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._refTranscriber.Instance.Transcribe(node._ref_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Inners._refⳆOPEN_levels_CLOSE._OPEN_levels_CLOSE node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._OPENTranscriber.Instance.Transcribe(node._OPEN_1, context);
__GeneratedOdataV3.Trancsribers.Rules._levelsTranscriber.Instance.Transcribe(node._levels_1, context);
__GeneratedOdataV3.Trancsribers.Rules._CLOSETranscriber.Instance.Transcribe(node._CLOSE_1, context);

return default;
            }
        }
    }
    
}
