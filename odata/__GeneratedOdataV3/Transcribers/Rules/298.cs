namespace __GeneratedOdataV3.Trancsribers.Rules
{
    public sealed class _yearTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Rules._year>
    {
        private _yearTranscriber()
        {
        }
        
        public static _yearTranscriber Instance { get; } = new _yearTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Rules._year value, System.Text.StringBuilder builder)
        {
            if (value._ʺx2Dʺ_1 != null)
{
__GeneratedOdataV3.Trancsribers.Inners._ʺx2DʺTranscriber.Instance.Transcribe(value._ʺx2Dʺ_1, builder);
}
__GeneratedOdataV3.Trancsribers.Inners._Ⲥʺx30ʺ_3DIGITⳆoneToNine_3ЖDIGITↃTranscriber.Instance.Transcribe(value._Ⲥʺx30ʺ_3DIGITⳆoneToNine_3ЖDIGITↃ_1, builder);

        }
    }
    
}
