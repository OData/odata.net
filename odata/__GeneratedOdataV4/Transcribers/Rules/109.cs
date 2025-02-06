namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _navigationTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._navigation>
    {
        private _navigationTranscriber()
        {
        }
        
        public static _navigationTranscriber Instance { get; } = new _navigationTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._navigation value, System.Text.StringBuilder builder)
        {
            foreach (var _Ⲥʺx2Fʺ_complexProperty_꘡ʺx2Fʺ_qualifiedComplexTypeName꘡Ↄ_1 in value._Ⲥʺx2Fʺ_complexProperty_꘡ʺx2Fʺ_qualifiedComplexTypeName꘡Ↄ_1)
{
Inners._Ⲥʺx2Fʺ_complexProperty_꘡ʺx2Fʺ_qualifiedComplexTypeName꘡ↃTranscriber.Instance.Transcribe(_Ⲥʺx2Fʺ_complexProperty_꘡ʺx2Fʺ_qualifiedComplexTypeName꘡Ↄ_1, builder);
}
__GeneratedOdataV4.Trancsribers.Inners._ʺx2FʺTranscriber.Instance.Transcribe(value._ʺx2Fʺ_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._navigationPropertyTranscriber.Instance.Transcribe(value._navigationProperty_1, builder);

        }
    }
    
}
