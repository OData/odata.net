namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _selectListPropertyTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._selectListProperty>
    {
        private _selectListPropertyTranscriber()
        {
        }
        
        public static _selectListPropertyTranscriber Instance { get; } = new _selectListPropertyTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._selectListProperty value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdata.CstNodes.Rules._selectListProperty.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._selectListProperty._primitiveProperty node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._primitivePropertyTranscriber.Instance.Transcribe(node._primitiveProperty_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._selectListProperty._primitiveColProperty node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._primitiveColPropertyTranscriber.Instance.Transcribe(node._primitiveColProperty_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._selectListProperty._navigationProperty_꘡ʺx2Bʺ꘡_꘡selectList꘡ node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._navigationPropertyTranscriber.Instance.Transcribe(node._navigationProperty_1, context);
if (node._ʺx2Bʺ_1 != null)
{
__GeneratedOdata.Trancsribers.Inners._ʺx2BʺTranscriber.Instance.Transcribe(node._ʺx2Bʺ_1, context);
}
if (node._selectList_1 != null)
{
__GeneratedOdata.Trancsribers.Rules._selectListTranscriber.Instance.Transcribe(node._selectList_1, context);
}

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._selectListProperty._selectPath_꘡ʺx2Fʺ_selectListProperty꘡ node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._selectPathTranscriber.Instance.Transcribe(node._selectPath_1, context);
if (node._ʺx2Fʺ_selectListProperty_1 != null)
{
__GeneratedOdata.Trancsribers.Inners._ʺx2Fʺ_selectListPropertyTranscriber.Instance.Transcribe(node._ʺx2Fʺ_selectListProperty_1, context);
}

return default;
            }
        }
    }
    
}
