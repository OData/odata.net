namespace __GeneratedOdataV3.Trancsribers.Rules
{
    public sealed class _primitiveFunctionImportCallTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Rules._primitiveFunctionImportCall>
    {
        private _primitiveFunctionImportCallTranscriber()
        {
        }
        
        public static _primitiveFunctionImportCallTranscriber Instance { get; } = new _primitiveFunctionImportCallTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Rules._primitiveFunctionImportCall value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV3.Trancsribers.Rules._primitiveFunctionImportTranscriber.Instance.Transcribe(value._primitiveFunctionImport_1, builder);
__GeneratedOdataV3.Trancsribers.Rules._functionParametersTranscriber.Instance.Transcribe(value._functionParameters_1, builder);

        }
    }
    
}
