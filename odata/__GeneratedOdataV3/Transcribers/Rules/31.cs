namespace __GeneratedOdataV3.Trancsribers.Rules
{
    public sealed class _boundEntityColFunctionCallTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Rules._boundEntityColFunctionCall>
    {
        private _boundEntityColFunctionCallTranscriber()
        {
        }
        
        public static _boundEntityColFunctionCallTranscriber Instance { get; } = new _boundEntityColFunctionCallTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Rules._boundEntityColFunctionCall value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV3.Trancsribers.Rules._namespaceTranscriber.Instance.Transcribe(value._namespace_1, builder);
__GeneratedOdataV3.Trancsribers.Inners._ʺx2EʺTranscriber.Instance.Transcribe(value._ʺx2Eʺ_1, builder);
__GeneratedOdataV3.Trancsribers.Rules._entityColFunctionTranscriber.Instance.Transcribe(value._entityColFunction_1, builder);
__GeneratedOdataV3.Trancsribers.Rules._functionParametersTranscriber.Instance.Transcribe(value._functionParameters_1, builder);

        }
    }
    
}
