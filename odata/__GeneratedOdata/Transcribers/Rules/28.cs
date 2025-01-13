namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _actionImportCallTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._actionImportCall>
    {
        private _actionImportCallTranscriber()
        {
        }
        
        public static _actionImportCallTranscriber Instance { get; } = new _actionImportCallTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._actionImportCall value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._actionImportTranscriber.Instance.Transcribe(value._actionImport_1, builder);

        }
    }
    
}
