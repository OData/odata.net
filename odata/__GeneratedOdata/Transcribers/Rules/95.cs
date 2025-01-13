namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _qualifiedFunctionNameTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._qualifiedFunctionName>
    {
        private _qualifiedFunctionNameTranscriber()
        {
        }
        
        public static _qualifiedFunctionNameTranscriber Instance { get; } = new _qualifiedFunctionNameTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._qualifiedFunctionName value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._namespaceTranscriber.Instance.Transcribe(value._namespace_1, builder);
__GeneratedOdata.Trancsribers.Inners._ʺx2EʺTranscriber.Instance.Transcribe(value._ʺx2Eʺ_1, builder);
__GeneratedOdata.Trancsribers.Rules._functionTranscriber.Instance.Transcribe(value._function_1, builder);
if (value._OPEN_parameterNames_CLOSE_1 != null)
{
__GeneratedOdata.Trancsribers.Inners._OPEN_parameterNames_CLOSETranscriber.Instance.Transcribe(value._OPEN_parameterNames_CLOSE_1, builder);
}

        }
    }
    
}
