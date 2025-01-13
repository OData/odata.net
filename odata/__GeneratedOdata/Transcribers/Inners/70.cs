namespace __GeneratedOdata.Trancsribers.Inners
{
    public sealed class _COMMA_entitySetNameTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Inners._COMMA_entitySetName>
    {
        private _COMMA_entitySetNameTranscriber()
        {
        }
        
        public static _COMMA_entitySetNameTranscriber Instance { get; } = new _COMMA_entitySetNameTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Inners._COMMA_entitySetName value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._COMMATranscriber.Instance.Transcribe(value._COMMA_1, builder);
__GeneratedOdata.Trancsribers.Rules._entitySetNameTranscriber.Instance.Transcribe(value._entitySetName_1, builder);

        }
    }
    
}
