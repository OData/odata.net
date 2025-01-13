namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _boundPrimitiveColFunctionCallTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._boundPrimitiveColFunctionCall>
    {
        private _boundPrimitiveColFunctionCallTranscriber()
        {
        }
        
        public static _boundPrimitiveColFunctionCallTranscriber Instance { get; } = new _boundPrimitiveColFunctionCallTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._boundPrimitiveColFunctionCall value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._namespaceTranscriber.Instance.Transcribe(value._namespace_1, builder);
__GeneratedOdata.Trancsribers.Inners._ʺx2EʺTranscriber.Instance.Transcribe(value._ʺx2Eʺ_1, builder);
__GeneratedOdata.Trancsribers.Rules._primitiveColFunctionTranscriber.Instance.Transcribe(value._primitiveColFunction_1, builder);
__GeneratedOdata.Trancsribers.Rules._functionParametersTranscriber.Instance.Transcribe(value._functionParameters_1, builder);

        }
    }
    
}
