namespace __GeneratedOdataV3.Trancsribers.Rules
{
    public sealed class _boundEntityFunctionCallTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Rules._boundEntityFunctionCall>
    {
        private _boundEntityFunctionCallTranscriber()
        {
        }
        
        public static _boundEntityFunctionCallTranscriber Instance { get; } = new _boundEntityFunctionCallTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Rules._boundEntityFunctionCall value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV3.Trancsribers.Rules._namespaceTranscriber.Instance.Transcribe(value._namespace_1, builder);
__GeneratedOdataV3.Trancsribers.Inners._ʺx2EʺTranscriber.Instance.Transcribe(value._ʺx2Eʺ_1, builder);
__GeneratedOdataV3.Trancsribers.Rules._entityFunctionTranscriber.Instance.Transcribe(value._entityFunction_1, builder);
__GeneratedOdataV3.Trancsribers.Rules._functionParametersTranscriber.Instance.Transcribe(value._functionParameters_1, builder);

        }
    }
    
}
