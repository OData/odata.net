namespace __GeneratedOdataV3.Trancsribers.Rules
{
    public sealed class _entityFunctionImportCallTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Rules._entityFunctionImportCall>
    {
        private _entityFunctionImportCallTranscriber()
        {
        }
        
        public static _entityFunctionImportCallTranscriber Instance { get; } = new _entityFunctionImportCallTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Rules._entityFunctionImportCall value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV3.Trancsribers.Rules._entityFunctionImportTranscriber.Instance.Transcribe(value._entityFunctionImport_1, builder);
__GeneratedOdataV3.Trancsribers.Rules._functionParametersTranscriber.Instance.Transcribe(value._functionParameters_1, builder);

        }
    }
    
}
