namespace __GeneratedOdataV2.Trancsribers.Rules
{
    public sealed class _selectPropertyTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV2.CstNodes.Rules._selectProperty>
    {
        private _selectPropertyTranscriber()
        {
        }
        
        public static _selectPropertyTranscriber Instance { get; } = new _selectPropertyTranscriber();
        
        public void Transcribe(__GeneratedOdataV2.CstNodes.Rules._selectProperty value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV2.CstNodes.Rules._selectProperty.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Rules._selectProperty._primitiveProperty node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Rules._primitivePropertyTranscriber.Instance.Transcribe(node._primitiveProperty_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Rules._selectProperty._primitiveColProperty_꘡OPEN_selectOptionPC_ЖⲤSEMI_selectOptionPCↃ_CLOSE꘡ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Rules._primitiveColPropertyTranscriber.Instance.Transcribe(node._primitiveColProperty_1, context);
if (node._OPEN_selectOptionPC_ЖⲤSEMI_selectOptionPCↃ_CLOSE_1 != null)
{
__GeneratedOdataV2.Trancsribers.Inners._OPEN_selectOptionPC_ЖⲤSEMI_selectOptionPCↃ_CLOSETranscriber.Instance.Transcribe(node._OPEN_selectOptionPC_ЖⲤSEMI_selectOptionPCↃ_CLOSE_1, context);
}

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Rules._selectProperty._navigationProperty node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Rules._navigationPropertyTranscriber.Instance.Transcribe(node._navigationProperty_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Rules._selectProperty._selectPath_꘡OPEN_selectOption_ЖⲤSEMI_selectOptionↃ_CLOSEⳆʺx2Fʺ_selectProperty꘡ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Rules._selectPathTranscriber.Instance.Transcribe(node._selectPath_1, context);
if (node._OPEN_selectOption_ЖⲤSEMI_selectOptionↃ_CLOSEⳆʺx2Fʺ_selectProperty_1 != null)
{
__GeneratedOdataV2.Trancsribers.Inners._OPEN_selectOption_ЖⲤSEMI_selectOptionↃ_CLOSEⳆʺx2Fʺ_selectPropertyTranscriber.Instance.Transcribe(node._OPEN_selectOption_ЖⲤSEMI_selectOptionↃ_CLOSEⳆʺx2Fʺ_selectProperty_1, context);
}

return default;
            }
        }
    }
    
}
