namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _complexColFunctionImportCallTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._complexColFunctionImportCall>
    {
        private _complexColFunctionImportCallTranscriber()
        {
        }
        
        public static _complexColFunctionImportCallTranscriber Instance { get; } = new _complexColFunctionImportCallTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._complexColFunctionImportCall value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._complexColFunctionImportTranscriber.Instance.Transcribe(value._complexColFunctionImport_1, builder);
__GeneratedOdata.Trancsribers.Rules._functionParametersTranscriber.Instance.Transcribe(value._functionParameters_1, builder);

        }
    }
    
}
