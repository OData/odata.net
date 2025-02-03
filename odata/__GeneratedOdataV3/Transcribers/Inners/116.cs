namespace __GeneratedOdataV3.Trancsribers.Inners
{
    public sealed class _ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡Ⳇcount_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡ⳆOPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSETranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Inners._ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡Ⳇcount_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡ⳆOPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSE>
    {
        private _ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡Ⳇcount_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡ⳆOPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSETranscriber()
        {
        }
        
        public static _ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡Ⳇcount_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡ⳆOPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSETranscriber Instance { get; } = new _ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡Ⳇcount_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡ⳆOPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSETranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Inners._ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡Ⳇcount_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡ⳆOPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSE value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV3.CstNodes.Inners._ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡Ⳇcount_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡ⳆOPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSE.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Inners._ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡Ⳇcount_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡ⳆOPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSE._ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._refTranscriber.Instance.Transcribe(node._ref_1, context);
if (node._OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE_1 != null)
{
__GeneratedOdataV3.Trancsribers.Inners._OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSETranscriber.Instance.Transcribe(node._OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE_1, context);
}

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Inners._ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡Ⳇcount_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡ⳆOPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSE._count_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._countTranscriber.Instance.Transcribe(node._count_1, context);
if (node._OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE_1 != null)
{
__GeneratedOdataV3.Trancsribers.Inners._OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSETranscriber.Instance.Transcribe(node._OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE_1, context);
}

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Inners._ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡Ⳇcount_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡ⳆOPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSE._OPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSE node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._OPENTranscriber.Instance.Transcribe(node._OPEN_1, context);
__GeneratedOdataV3.Trancsribers.Rules._expandOptionTranscriber.Instance.Transcribe(node._expandOption_1, context);
foreach (var _ⲤSEMI_expandOptionↃ_1 in node._ⲤSEMI_expandOptionↃ_1)
{
Inners._ⲤSEMI_expandOptionↃTranscriber.Instance.Transcribe(_ⲤSEMI_expandOptionↃ_1, context);
}
__GeneratedOdataV3.Trancsribers.Rules._CLOSETranscriber.Instance.Transcribe(node._CLOSE_1, context);

return default;
            }
        }
    }
    
}
