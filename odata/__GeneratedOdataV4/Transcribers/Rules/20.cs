namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _complexPathTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._complexPath>
    {
        private _complexPathTranscriber()
        {
        }
        
        public static _complexPathTranscriber Instance { get; } = new _complexPathTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._complexPath value, System.Text.StringBuilder builder)
        {
            if (value._ʺx2Fʺ_qualifiedComplexTypeName_1 != null)
{
__GeneratedOdataV4.Trancsribers.Inners._ʺx2Fʺ_qualifiedComplexTypeNameTranscriber.Instance.Transcribe(value._ʺx2Fʺ_qualifiedComplexTypeName_1, builder);
}
if (value._ʺx2Fʺ_propertyPathⳆboundOperation_1 != null)
{
__GeneratedOdataV4.Trancsribers.Inners._ʺx2Fʺ_propertyPathⳆboundOperationTranscriber.Instance.Transcribe(value._ʺx2Fʺ_propertyPathⳆboundOperation_1, builder);
}

        }
    }
    
}
