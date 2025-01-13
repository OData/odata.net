namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _boundComplexFunctionCallTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._boundComplexFunctionCall>
    {
        private _boundComplexFunctionCallTranscriber()
        {
        }
        
        public static _boundComplexFunctionCallTranscriber Instance { get; } = new _boundComplexFunctionCallTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._boundComplexFunctionCall value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._namespaceTranscriber.Instance.Transcribe(value._namespace_1, builder);
__GeneratedOdata.Trancsribers.Inners._ʺx2EʺTranscriber.Instance.Transcribe(value._ʺx2Eʺ_1, builder);
__GeneratedOdata.Trancsribers.Rules._complexFunctionTranscriber.Instance.Transcribe(value._complexFunction_1, builder);
__GeneratedOdata.Trancsribers.Rules._functionParametersTranscriber.Instance.Transcribe(value._functionParameters_1, builder);

        }
    }
    
}
