namespace __GeneratedOdataV2.Trancsribers.Rules
{
    public sealed class _selectPathTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV2.CstNodes.Rules._selectPath>
    {
        private _selectPathTranscriber()
        {
        }
        
        public static _selectPathTranscriber Instance { get; } = new _selectPathTranscriber();
        
        public void Transcribe(__GeneratedOdataV2.CstNodes.Rules._selectPath value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV2.Trancsribers.Inners._ⲤcomplexPropertyⳆcomplexColPropertyↃTranscriber.Instance.Transcribe(value._ⲤcomplexPropertyⳆcomplexColPropertyↃ_1, builder);
if (value._ʺx2Fʺ_qualifiedComplexTypeName_1 != null)
{
__GeneratedOdataV2.Trancsribers.Inners._ʺx2Fʺ_qualifiedComplexTypeNameTranscriber.Instance.Transcribe(value._ʺx2Fʺ_qualifiedComplexTypeName_1, builder);
}

        }
    }
    
}
