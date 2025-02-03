namespace __GeneratedOdataV3.Trancsribers.Rules
{
    public sealed class _boundComplexColFunctionCallTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Rules._boundComplexColFunctionCall>
    {
        private _boundComplexColFunctionCallTranscriber()
        {
        }
        
        public static _boundComplexColFunctionCallTranscriber Instance { get; } = new _boundComplexColFunctionCallTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Rules._boundComplexColFunctionCall value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV3.Trancsribers.Rules._namespaceTranscriber.Instance.Transcribe(value._namespace_1, builder);
__GeneratedOdataV3.Trancsribers.Inners._ʺx2EʺTranscriber.Instance.Transcribe(value._ʺx2Eʺ_1, builder);
__GeneratedOdataV3.Trancsribers.Rules._complexColFunctionTranscriber.Instance.Transcribe(value._complexColFunction_1, builder);
__GeneratedOdataV3.Trancsribers.Rules._functionParametersTranscriber.Instance.Transcribe(value._functionParameters_1, builder);

        }
    }
    
}
