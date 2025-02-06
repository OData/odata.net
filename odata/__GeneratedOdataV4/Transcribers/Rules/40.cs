namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _complexColFunctionImportCallTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._complexColFunctionImportCall>
    {
        private _complexColFunctionImportCallTranscriber()
        {
        }
        
        public static _complexColFunctionImportCallTranscriber Instance { get; } = new _complexColFunctionImportCallTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._complexColFunctionImportCall value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Rules._complexColFunctionImportTranscriber.Instance.Transcribe(value._complexColFunctionImport_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._functionParametersTranscriber.Instance.Transcribe(value._functionParameters_1, builder);

        }
    }
    
}
