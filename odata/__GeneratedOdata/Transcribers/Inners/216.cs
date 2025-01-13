namespace __GeneratedOdata.Trancsribers.Inners
{
    public sealed class _OPEN_selectOption_ЖⲤSEMI_selectOptionↃ_CLOSEⳆʺx2Fʺ_selectPropertyTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Inners._OPEN_selectOption_ЖⲤSEMI_selectOptionↃ_CLOSEⳆʺx2Fʺ_selectProperty>
    {
        private _OPEN_selectOption_ЖⲤSEMI_selectOptionↃ_CLOSEⳆʺx2Fʺ_selectPropertyTranscriber()
        {
        }
        
        public static _OPEN_selectOption_ЖⲤSEMI_selectOptionↃ_CLOSEⳆʺx2Fʺ_selectPropertyTranscriber Instance { get; } = new _OPEN_selectOption_ЖⲤSEMI_selectOptionↃ_CLOSEⳆʺx2Fʺ_selectPropertyTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Inners._OPEN_selectOption_ЖⲤSEMI_selectOptionↃ_CLOSEⳆʺx2Fʺ_selectProperty value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdata.CstNodes.Inners._OPEN_selectOption_ЖⲤSEMI_selectOptionↃ_CLOSEⳆʺx2Fʺ_selectProperty.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Inners._OPEN_selectOption_ЖⲤSEMI_selectOptionↃ_CLOSEⳆʺx2Fʺ_selectProperty._OPEN_selectOption_ЖⲤSEMI_selectOptionↃ_CLOSE node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._OPENTranscriber.Instance.Transcribe(node._OPEN_1, context);
__GeneratedOdata.Trancsribers.Rules._selectOptionTranscriber.Instance.Transcribe(node._selectOption_1, context);
foreach (var _ⲤSEMI_selectOptionↃ_1 in node._ⲤSEMI_selectOptionↃ_1)
{
__GeneratedOdata.Trancsribers.Inners._ⲤSEMI_selectOptionↃTranscriber.Instance.Transcribe(_ⲤSEMI_selectOptionↃ_1, context);
}
__GeneratedOdata.Trancsribers.Rules._CLOSETranscriber.Instance.Transcribe(node._CLOSE_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Inners._OPEN_selectOption_ЖⲤSEMI_selectOptionↃ_CLOSEⳆʺx2Fʺ_selectProperty._ʺx2Fʺ_selectProperty node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Inners._ʺx2FʺTranscriber.Instance.Transcribe(node._ʺx2Fʺ_1, context);
__GeneratedOdata.Trancsribers.Rules._selectPropertyTranscriber.Instance.Transcribe(node._selectProperty_1, context);

return default;
            }
        }
    }
    
}
