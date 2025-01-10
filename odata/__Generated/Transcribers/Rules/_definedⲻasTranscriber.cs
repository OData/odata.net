namespace __Generated.Trancsribers.Rules
{
    public sealed class _definedⲻasTranscriber : GeneratorV3.ITranscriber<__Generated.CstNodes.Rules._definedⲻas>
    {
        private _definedⲻasTranscriber()
        {
        }
        
        public static _definedⲻasTranscriber Instance { get; } = new _definedⲻasTranscriber();
        
        public void Transcribe(__Generated.CstNodes.Rules._definedⲻas value, System.Text.StringBuilder builder)
        {
            foreach (var _cⲻwsp_1 in value._cⲻwsp_1)
{
__Generated.Trancsribers.Rules._cⲻwspTranscriber.Instance.Transcribe(_cⲻwsp_1, builder);
}
__Generated.Trancsribers.Inners._Ⲥʺx3DʺⳆʺx3Dx2FʺↃTranscriber.Instance.Transcribe(value._Ⲥʺx3DʺⳆʺx3Dx2FʺↃ_1, builder);
foreach (var _cⲻwsp_2 in value._cⲻwsp_2)
{
__Generated.Trancsribers.Rules._cⲻwspTranscriber.Instance.Transcribe(_cⲻwsp_2, builder);
}

        }
    }
    
}
