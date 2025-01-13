namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _primitiveColFunctionImportCallTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._primitiveColFunctionImportCall>
    {
        private _primitiveColFunctionImportCallTranscriber()
        {
        }
        
        public static _primitiveColFunctionImportCallTranscriber Instance { get; } = new _primitiveColFunctionImportCallTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._primitiveColFunctionImportCall value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._primitiveColFunctionImportTranscriber.Instance.Transcribe(value._primitiveColFunctionImport_1, builder);
__GeneratedOdata.Trancsribers.Rules._functionParametersTranscriber.Instance.Transcribe(value._functionParameters_1, builder);

        }
    }
    
}
