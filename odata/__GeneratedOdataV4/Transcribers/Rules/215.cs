namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _stringInJSONTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._stringInJSON>
    {
        private _stringInJSONTranscriber()
        {
        }
        
        public static _stringInJSONTranscriber Instance { get; } = new _stringInJSONTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._stringInJSON value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Rules._quotationⲻmarkTranscriber.Instance.Transcribe(value._quotationⲻmark_1, builder);
foreach (var _charInJSON_1 in value._charInJSON_1)
{
__GeneratedOdataV4.Trancsribers.Rules._charInJSONTranscriber.Instance.Transcribe(_charInJSON_1, builder);
}
__GeneratedOdataV4.Trancsribers.Rules._quotationⲻmarkTranscriber.Instance.Transcribe(value._quotationⲻmark_2, builder);

        }
    }
    
}
