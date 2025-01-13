namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _IPvFutureTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._IPvFuture>
    {
        private _IPvFutureTranscriber()
        {
        }
        
        public static _IPvFutureTranscriber Instance { get; } = new _IPvFutureTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._IPvFuture value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Inners._ʺx76ʺTranscriber.Instance.Transcribe(value._ʺx76ʺ_1, builder);
foreach (var _HEXDIG_1 in value._HEXDIG_1)
{
__GeneratedOdata.Trancsribers.Rules._HEXDIGTranscriber.Instance.Transcribe(_HEXDIG_1, builder);
}
__GeneratedOdata.Trancsribers.Inners._ʺx2EʺTranscriber.Instance.Transcribe(value._ʺx2Eʺ_1, builder);
foreach (var _ⲤunreservedⳆsubⲻdelimsⳆʺx3AʺↃ_1 in value._ⲤunreservedⳆsubⲻdelimsⳆʺx3AʺↃ_1)
{
__GeneratedOdata.Trancsribers.Inners._ⲤunreservedⳆsubⲻdelimsⳆʺx3AʺↃTranscriber.Instance.Transcribe(_ⲤunreservedⳆsubⲻdelimsⳆʺx3AʺↃ_1, builder);
}

        }
    }
    
}
