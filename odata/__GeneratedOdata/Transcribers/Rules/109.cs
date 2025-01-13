namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _navigationTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._navigation>
    {
        private _navigationTranscriber()
        {
        }
        
        public static _navigationTranscriber Instance { get; } = new _navigationTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._navigation value, System.Text.StringBuilder builder)
        {
            foreach (var _Ⲥʺx2Fʺ_complexProperty_꘡ʺx2Fʺ_qualifiedComplexTypeName꘡Ↄ_1 in value._Ⲥʺx2Fʺ_complexProperty_꘡ʺx2Fʺ_qualifiedComplexTypeName꘡Ↄ_1)
{
__GeneratedOdata.Trancsribers.Inners._Ⲥʺx2Fʺ_complexProperty_꘡ʺx2Fʺ_qualifiedComplexTypeName꘡ↃTranscriber.Instance.Transcribe(_Ⲥʺx2Fʺ_complexProperty_꘡ʺx2Fʺ_qualifiedComplexTypeName꘡Ↄ_1, builder);
}
__GeneratedOdata.Trancsribers.Inners._ʺx2FʺTranscriber.Instance.Transcribe(value._ʺx2Fʺ_1, builder);
__GeneratedOdata.Trancsribers.Rules._navigationPropertyTranscriber.Instance.Transcribe(value._navigationProperty_1, builder);

        }
    }
    
}
