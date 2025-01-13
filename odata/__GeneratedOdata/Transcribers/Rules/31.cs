namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _boundEntityColFunctionCallTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._boundEntityColFunctionCall>
    {
        private _boundEntityColFunctionCallTranscriber()
        {
        }
        
        public static _boundEntityColFunctionCallTranscriber Instance { get; } = new _boundEntityColFunctionCallTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._boundEntityColFunctionCall value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._namespaceTranscriber.Instance.Transcribe(value._namespace_1, builder);
__GeneratedOdata.Trancsribers.Inners._ʺx2EʺTranscriber.Instance.Transcribe(value._ʺx2Eʺ_1, builder);
__GeneratedOdata.Trancsribers.Rules._entityColFunctionTranscriber.Instance.Transcribe(value._entityColFunction_1, builder);
__GeneratedOdata.Trancsribers.Rules._functionParametersTranscriber.Instance.Transcribe(value._functionParameters_1, builder);

        }
    }
    
}
