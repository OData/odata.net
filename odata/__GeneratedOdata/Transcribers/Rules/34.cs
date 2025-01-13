namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _boundPrimitiveFunctionCallTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._boundPrimitiveFunctionCall>
    {
        private _boundPrimitiveFunctionCallTranscriber()
        {
        }
        
        public static _boundPrimitiveFunctionCallTranscriber Instance { get; } = new _boundPrimitiveFunctionCallTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._boundPrimitiveFunctionCall value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._namespaceTranscriber.Instance.Transcribe(value._namespace_1, builder);
__GeneratedOdata.Trancsribers.Inners._ʺx2EʺTranscriber.Instance.Transcribe(value._ʺx2Eʺ_1, builder);
__GeneratedOdata.Trancsribers.Rules._primitiveFunctionTranscriber.Instance.Transcribe(value._primitiveFunction_1, builder);
__GeneratedOdata.Trancsribers.Rules._functionParametersTranscriber.Instance.Transcribe(value._functionParameters_1, builder);

        }
    }
    
}
