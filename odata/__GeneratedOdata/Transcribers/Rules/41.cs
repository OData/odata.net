namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _primitiveFunctionImportCallTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._primitiveFunctionImportCall>
    {
        private _primitiveFunctionImportCallTranscriber()
        {
        }
        
        public static _primitiveFunctionImportCallTranscriber Instance { get; } = new _primitiveFunctionImportCallTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._primitiveFunctionImportCall value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._primitiveFunctionImportTranscriber.Instance.Transcribe(value._primitiveFunctionImport_1, builder);
__GeneratedOdata.Trancsribers.Rules._functionParametersTranscriber.Instance.Transcribe(value._functionParameters_1, builder);

        }
    }
    
}
