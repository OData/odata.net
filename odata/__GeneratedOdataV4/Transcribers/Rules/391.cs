namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _IPvFutureTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._IPvFuture>
    {
        private _IPvFutureTranscriber()
        {
        }
        
        public static _IPvFutureTranscriber Instance { get; } = new _IPvFutureTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._IPvFuture value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Inners._ʺx76ʺTranscriber.Instance.Transcribe(value._ʺx76ʺ_1, builder);
foreach (var _HEXDIG_1 in value._HEXDIG_1)
{
__GeneratedOdataV4.Trancsribers.Rules._HEXDIGTranscriber.Instance.Transcribe(_HEXDIG_1, builder);
}
__GeneratedOdataV4.Trancsribers.Inners._ʺx2EʺTranscriber.Instance.Transcribe(value._ʺx2Eʺ_1, builder);
foreach (var _ⲤunreservedⳆsubⲻdelimsⳆʺx3AʺↃ_1 in value._ⲤunreservedⳆsubⲻdelimsⳆʺx3AʺↃ_1)
{
__GeneratedOdataV4.Trancsribers.Inners._ⲤunreservedⳆsubⲻdelimsⳆʺx3AʺↃTranscriber.Instance.Transcribe(_ⲤunreservedⳆsubⲻdelimsⳆʺx3AʺↃ_1, builder);
}

        }
    }
    
}
