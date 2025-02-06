namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _boundPrimitiveFunctionCallTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._boundPrimitiveFunctionCall>
    {
        private _boundPrimitiveFunctionCallTranscriber()
        {
        }
        
        public static _boundPrimitiveFunctionCallTranscriber Instance { get; } = new _boundPrimitiveFunctionCallTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._boundPrimitiveFunctionCall value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Rules._namespaceTranscriber.Instance.Transcribe(value._namespace_1, builder);
__GeneratedOdataV4.Trancsribers.Inners._ʺx2EʺTranscriber.Instance.Transcribe(value._ʺx2Eʺ_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._primitiveFunctionTranscriber.Instance.Transcribe(value._primitiveFunction_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._functionParametersTranscriber.Instance.Transcribe(value._functionParameters_1, builder);

        }
    }
    
}
