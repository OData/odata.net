namespace __Generated.Trancsribers.Rules
{
    public sealed class _hexⲻvalTranscriber : GeneratorV3.ITranscriber<__Generated.CstNodes.Rules._hexⲻval>
    {
        private _hexⲻvalTranscriber()
        {
        }
        
        public static _hexⲻvalTranscriber Instance { get; } = new _hexⲻvalTranscriber();
        
        public void Transcribe(__Generated.CstNodes.Rules._hexⲻval value, System.Text.StringBuilder builder)
        {
            __Generated.Trancsribers.Inners._ʺx78ʺTranscriber.Instance.Transcribe(value._ʺx78ʺ_1, builder);
foreach (var _HEXDIG_1 in value._HEXDIG_1)
{
__Generated.Trancsribers.Rules._HEXDIGTranscriber.Instance.Transcribe(_HEXDIG_1, builder);
}
if (value._1ЖⲤʺx2Eʺ_1ЖHEXDIGↃⳆⲤʺx2Dʺ_1ЖHEXDIGↃ_1 != null)
{
__Generated.Trancsribers.Inners._1ЖⲤʺx2Eʺ_1ЖHEXDIGↃⳆⲤʺx2Dʺ_1ЖHEXDIGↃTranscriber.Instance.Transcribe(value._1ЖⲤʺx2Eʺ_1ЖHEXDIGↃⳆⲤʺx2Dʺ_1ЖHEXDIGↃ_1, builder);
}

        }
    }
    
}
