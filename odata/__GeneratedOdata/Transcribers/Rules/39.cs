namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _complexFunctionImportCallTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._complexFunctionImportCall>
    {
        private _complexFunctionImportCallTranscriber()
        {
        }
        
        public static _complexFunctionImportCallTranscriber Instance { get; } = new _complexFunctionImportCallTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._complexFunctionImportCall value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._complexFunctionImportTranscriber.Instance.Transcribe(value._complexFunctionImport_1, builder);
__GeneratedOdata.Trancsribers.Rules._functionParametersTranscriber.Instance.Transcribe(value._functionParameters_1, builder);

        }
    }
    
}
