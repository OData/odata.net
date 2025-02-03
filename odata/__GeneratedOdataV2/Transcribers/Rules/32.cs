namespace __GeneratedOdataV2.Trancsribers.Rules
{
    public sealed class _boundComplexFunctionCallTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV2.CstNodes.Rules._boundComplexFunctionCall>
    {
        private _boundComplexFunctionCallTranscriber()
        {
        }
        
        public static _boundComplexFunctionCallTranscriber Instance { get; } = new _boundComplexFunctionCallTranscriber();
        
        public void Transcribe(__GeneratedOdataV2.CstNodes.Rules._boundComplexFunctionCall value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV2.Trancsribers.Rules._namespaceTranscriber.Instance.Transcribe(value._namespace_1, builder);
__GeneratedOdataV2.Trancsribers.Inners._ʺx2EʺTranscriber.Instance.Transcribe(value._ʺx2Eʺ_1, builder);
__GeneratedOdataV2.Trancsribers.Rules._complexFunctionTranscriber.Instance.Transcribe(value._complexFunction_1, builder);
__GeneratedOdataV2.Trancsribers.Rules._functionParametersTranscriber.Instance.Transcribe(value._functionParameters_1, builder);

        }
    }
    
}
