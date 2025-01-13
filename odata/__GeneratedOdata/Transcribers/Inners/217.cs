namespace __GeneratedOdata.Trancsribers.Inners
{
    public sealed class _OPEN_parameterNames_CLOSETranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Inners._OPEN_parameterNames_CLOSE>
    {
        private _OPEN_parameterNames_CLOSETranscriber()
        {
        }
        
        public static _OPEN_parameterNames_CLOSETranscriber Instance { get; } = new _OPEN_parameterNames_CLOSETranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Inners._OPEN_parameterNames_CLOSE value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._OPENTranscriber.Instance.Transcribe(value._OPEN_1, builder);
__GeneratedOdata.Trancsribers.Rules._parameterNamesTranscriber.Instance.Transcribe(value._parameterNames_1, builder);
__GeneratedOdata.Trancsribers.Rules._CLOSETranscriber.Instance.Transcribe(value._CLOSE_1, builder);

        }
    }
    
}
