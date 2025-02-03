namespace __GeneratedOdataV3.Trancsribers.Inners
{
    public sealed class _ʺx2Fʺ_complexProperty_꘡ʺx2Fʺ_qualifiedComplexTypeName꘡Transcriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Inners._ʺx2Fʺ_complexProperty_꘡ʺx2Fʺ_qualifiedComplexTypeName꘡>
    {
        private _ʺx2Fʺ_complexProperty_꘡ʺx2Fʺ_qualifiedComplexTypeName꘡Transcriber()
        {
        }
        
        public static _ʺx2Fʺ_complexProperty_꘡ʺx2Fʺ_qualifiedComplexTypeName꘡Transcriber Instance { get; } = new _ʺx2Fʺ_complexProperty_꘡ʺx2Fʺ_qualifiedComplexTypeName꘡Transcriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Inners._ʺx2Fʺ_complexProperty_꘡ʺx2Fʺ_qualifiedComplexTypeName꘡ value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV3.Trancsribers.Inners._ʺx2FʺTranscriber.Instance.Transcribe(value._ʺx2Fʺ_1, builder);
__GeneratedOdataV3.Trancsribers.Rules._complexPropertyTranscriber.Instance.Transcribe(value._complexProperty_1, builder);
if (value._ʺx2Fʺ_qualifiedComplexTypeName_1 != null)
{
__GeneratedOdataV3.Trancsribers.Inners._ʺx2Fʺ_qualifiedComplexTypeNameTranscriber.Instance.Transcribe(value._ʺx2Fʺ_qualifiedComplexTypeName_1, builder);
}

        }
    }
    
}
