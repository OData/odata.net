namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _base64b16Transcriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._base64b16>
    {
        private _base64b16Transcriber()
        {
        }
        
        public static _base64b16Transcriber Instance { get; } = new _base64b16Transcriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._base64b16 value, System.Text.StringBuilder builder)
        {
            foreach (var _base64char_1 in value._base64char_1)
{
__GeneratedOdata.Trancsribers.Rules._base64charTranscriber.Instance.Transcribe(_base64char_1, builder);
}
__GeneratedOdata.Trancsribers.Inners._Ⲥʺx41ʺⳆʺx45ʺⳆʺx49ʺⳆʺx4DʺⳆʺx51ʺⳆʺx55ʺⳆʺx59ʺⳆʺx63ʺⳆʺx67ʺⳆʺx6BʺⳆʺx6FʺⳆʺx73ʺⳆʺx77ʺⳆʺx30ʺⳆʺx34ʺⳆʺx38ʺↃTranscriber.Instance.Transcribe(value._Ⲥʺx41ʺⳆʺx45ʺⳆʺx49ʺⳆʺx4DʺⳆʺx51ʺⳆʺx55ʺⳆʺx59ʺⳆʺx63ʺⳆʺx67ʺⳆʺx6BʺⳆʺx6FʺⳆʺx73ʺⳆʺx77ʺⳆʺx30ʺⳆʺx34ʺⳆʺx38ʺↃ_1, builder);
if (value._ʺx3Dʺ_1 != null)
{
__GeneratedOdata.Trancsribers.Inners._ʺx3DʺTranscriber.Instance.Transcribe(value._ʺx3Dʺ_1, builder);
}

        }
    }
    
}
