namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _singleNavigationTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._singleNavigation>
    {
        private _singleNavigationTranscriber()
        {
        }
        
        public static _singleNavigationTranscriber Instance { get; } = new _singleNavigationTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._singleNavigation value, System.Text.StringBuilder builder)
        {
            if (value._ʺx2Fʺ_qualifiedEntityTypeName_1 != null)
{
__GeneratedOdata.Trancsribers.Inners._ʺx2Fʺ_qualifiedEntityTypeNameTranscriber.Instance.Transcribe(value._ʺx2Fʺ_qualifiedEntityTypeName_1, builder);
}
if (value._ʺx2Fʺ_propertyPathⳆboundOperationⳆrefⳆvalue_1 != null)
{
__GeneratedOdata.Trancsribers.Inners._ʺx2Fʺ_propertyPathⳆboundOperationⳆrefⳆvalueTranscriber.Instance.Transcribe(value._ʺx2Fʺ_propertyPathⳆboundOperationⳆrefⳆvalue_1, builder);
}

        }
    }
    
}
