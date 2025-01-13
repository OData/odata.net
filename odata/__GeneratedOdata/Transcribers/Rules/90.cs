namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _selectPathTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._selectPath>
    {
        private _selectPathTranscriber()
        {
        }
        
        public static _selectPathTranscriber Instance { get; } = new _selectPathTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._selectPath value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Inners._ⲤcomplexPropertyⳆcomplexColPropertyↃTranscriber.Instance.Transcribe(value._ⲤcomplexPropertyⳆcomplexColPropertyↃ_1, builder);
if (value._ʺx2Fʺ_qualifiedComplexTypeName_1 != null)
{
__GeneratedOdata.Trancsribers.Inners._ʺx2Fʺ_qualifiedComplexTypeNameTranscriber.Instance.Transcribe(value._ʺx2Fʺ_qualifiedComplexTypeName_1, builder);
}

        }
    }
    
}
