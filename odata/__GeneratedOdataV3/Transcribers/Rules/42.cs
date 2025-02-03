namespace __GeneratedOdataV3.Trancsribers.Rules
{
    public sealed class _primitiveColFunctionImportCallTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Rules._primitiveColFunctionImportCall>
    {
        private _primitiveColFunctionImportCallTranscriber()
        {
        }
        
        public static _primitiveColFunctionImportCallTranscriber Instance { get; } = new _primitiveColFunctionImportCallTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Rules._primitiveColFunctionImportCall value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV3.Trancsribers.Rules._primitiveColFunctionImportTranscriber.Instance.Transcribe(value._primitiveColFunctionImport_1, builder);
__GeneratedOdataV3.Trancsribers.Rules._functionParametersTranscriber.Instance.Transcribe(value._functionParameters_1, builder);

        }
    }
    
}
