namespace __GeneratedOdataV4.Trancsribers.Inners
{
    public sealed class _COMMA_computeItemTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Inners._COMMA_computeItem>
    {
        private _COMMA_computeItemTranscriber()
        {
        }
        
        public static _COMMA_computeItemTranscriber Instance { get; } = new _COMMA_computeItemTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Inners._COMMA_computeItem value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Rules._COMMATranscriber.Instance.Transcribe(value._COMMA_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._computeItemTranscriber.Instance.Transcribe(value._computeItem_1, builder);

        }
    }
    
}
