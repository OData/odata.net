namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _entityColFunctionImportCallTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._entityColFunctionImportCall>
    {
        private _entityColFunctionImportCallTranscriber()
        {
        }
        
        public static _entityColFunctionImportCallTranscriber Instance { get; } = new _entityColFunctionImportCallTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._entityColFunctionImportCall value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._entityColFunctionImportTranscriber.Instance.Transcribe(value._entityColFunctionImport_1, builder);
__GeneratedOdata.Trancsribers.Rules._functionParametersTranscriber.Instance.Transcribe(value._functionParameters_1, builder);

        }
    }
    
}
