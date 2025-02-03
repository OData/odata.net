namespace __GeneratedOdataV2.Trancsribers.Inners
{
    public sealed class _4base64charTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV2.CstNodes.Inners._4base64char>
    {
        private _4base64charTranscriber()
        {
        }
        
        public static _4base64charTranscriber Instance { get; } = new _4base64charTranscriber();
        
        public void Transcribe(__GeneratedOdataV2.CstNodes.Inners._4base64char value, System.Text.StringBuilder builder)
        {
            foreach (var _base64char_1 in value._base64char_1)
{
__GeneratedOdataV2.Trancsribers.Rules._base64charTranscriber.Instance.Transcribe(_base64char_1, builder);
}

        }
    }
    
}
