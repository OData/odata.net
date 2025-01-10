namespace __Generated.Trancsribers.Rules
{
    public sealed class _binⲻvalTranscriber : GeneratorV3.ITranscriber<__Generated.CstNodes.Rules._binⲻval>
    {
        private _binⲻvalTranscriber()
        {
        }
        
        public static _binⲻvalTranscriber Instance { get; } = new _binⲻvalTranscriber();
        
        public void Transcribe(__Generated.CstNodes.Rules._binⲻval value, System.Text.StringBuilder builder)
        {
            __Generated.Trancsribers.Inners._ʺx62ʺTranscriber.Instance.Transcribe(value._ʺx62ʺ_1, builder);
foreach (var _BIT_1 in value._BIT_1)
{
__Generated.Trancsribers.Rules._BITTranscriber.Instance.Transcribe(_BIT_1, builder);
}
if (value._1ЖⲤʺx2Eʺ_1ЖBITↃⳆⲤʺx2Dʺ_1ЖBITↃ_1 != null)
{
__Generated.Trancsribers.Inners._1ЖⲤʺx2Eʺ_1ЖBITↃⳆⲤʺx2Dʺ_1ЖBITↃTranscriber.Instance.Transcribe(value._1ЖⲤʺx2Eʺ_1ЖBITↃⳆⲤʺx2Dʺ_1ЖBITↃ_1, builder);
}

        }
    }
    
}
