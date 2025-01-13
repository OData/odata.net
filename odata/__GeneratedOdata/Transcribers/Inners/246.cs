namespace __GeneratedOdata.Trancsribers.Inners
{
    public sealed class _ʺx2Fʺ_complexProperty_꘡ʺx2Fʺ_qualifiedComplexTypeName꘡Transcriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Inners._ʺx2Fʺ_complexProperty_꘡ʺx2Fʺ_qualifiedComplexTypeName꘡>
    {
        private _ʺx2Fʺ_complexProperty_꘡ʺx2Fʺ_qualifiedComplexTypeName꘡Transcriber()
        {
        }
        
        public static _ʺx2Fʺ_complexProperty_꘡ʺx2Fʺ_qualifiedComplexTypeName꘡Transcriber Instance { get; } = new _ʺx2Fʺ_complexProperty_꘡ʺx2Fʺ_qualifiedComplexTypeName꘡Transcriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Inners._ʺx2Fʺ_complexProperty_꘡ʺx2Fʺ_qualifiedComplexTypeName꘡ value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Inners._ʺx2FʺTranscriber.Instance.Transcribe(value._ʺx2Fʺ_1, builder);
__GeneratedOdata.Trancsribers.Rules._complexPropertyTranscriber.Instance.Transcribe(value._complexProperty_1, builder);
if (value._ʺx2Fʺ_qualifiedComplexTypeName_1 != null)
{
__GeneratedOdata.Trancsribers.Inners._ʺx2Fʺ_qualifiedComplexTypeNameTranscriber.Instance.Transcribe(value._ʺx2Fʺ_qualifiedComplexTypeName_1, builder);
}

        }
    }
    
}
