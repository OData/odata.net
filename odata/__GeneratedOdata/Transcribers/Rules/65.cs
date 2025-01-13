namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _expandItemTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._expandItem>
    {
        private _expandItemTranscriber()
        {
        }
        
        public static _expandItemTranscriber Instance { get; } = new _expandItemTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._expandItem value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdata.CstNodes.Rules._expandItem.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._expandItem._STAR_꘡refⳆOPEN_levels_CLOSE꘡ node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._STARTranscriber.Instance.Transcribe(node._STAR_1, context);
if (node._refⳆOPEN_levels_CLOSE_1 != null)
{
__GeneratedOdata.Trancsribers.Inners._refⳆOPEN_levels_CLOSETranscriber.Instance.Transcribe(node._refⳆOPEN_levels_CLOSE_1, context);
}

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._expandItem._ʺx24x76x61x6Cx75x65ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Inners._ʺx24x76x61x6Cx75x65ʺTranscriber.Instance.Transcribe(node._ʺx24x76x61x6Cx75x65ʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._expandItem._expandPath_꘡ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡Ⳇcount_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡ⳆOPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSE꘡ node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._expandPathTranscriber.Instance.Transcribe(node._expandPath_1, context);
if (node._ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡Ⳇcount_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡ⳆOPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSE_1 != null)
{
__GeneratedOdata.Trancsribers.Inners._ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡Ⳇcount_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡ⳆOPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSETranscriber.Instance.Transcribe(node._ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡Ⳇcount_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡ⳆOPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSE_1, context);
}

return default;
            }
        }
    }
    
}
