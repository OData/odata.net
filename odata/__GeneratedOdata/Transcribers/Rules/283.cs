namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _guidValueTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._guidValue>
    {
        private _guidValueTranscriber()
        {
        }
        
        public static _guidValueTranscriber Instance { get; } = new _guidValueTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._guidValue value, System.Text.StringBuilder builder)
        {
            foreach (var _HEXDIG_1 in value._HEXDIG_1)
{
__GeneratedOdata.Trancsribers.Rules._HEXDIGTranscriber.Instance.Transcribe(_HEXDIG_1, builder);
}
__GeneratedOdata.Trancsribers.Inners._ʺx2DʺTranscriber.Instance.Transcribe(value._ʺx2Dʺ_1, builder);
foreach (var _HEXDIG_2 in value._HEXDIG_2)
{
__GeneratedOdata.Trancsribers.Rules._HEXDIGTranscriber.Instance.Transcribe(_HEXDIG_2, builder);
}
__GeneratedOdata.Trancsribers.Inners._ʺx2DʺTranscriber.Instance.Transcribe(value._ʺx2Dʺ_2, builder);
foreach (var _HEXDIG_3 in value._HEXDIG_3)
{
__GeneratedOdata.Trancsribers.Rules._HEXDIGTranscriber.Instance.Transcribe(_HEXDIG_3, builder);
}
__GeneratedOdata.Trancsribers.Inners._ʺx2DʺTranscriber.Instance.Transcribe(value._ʺx2Dʺ_3, builder);
foreach (var _HEXDIG_4 in value._HEXDIG_4)
{
__GeneratedOdata.Trancsribers.Rules._HEXDIGTranscriber.Instance.Transcribe(_HEXDIG_4, builder);
}
__GeneratedOdata.Trancsribers.Inners._ʺx2DʺTranscriber.Instance.Transcribe(value._ʺx2Dʺ_4, builder);
foreach (var _HEXDIG_5 in value._HEXDIG_5)
{
__GeneratedOdata.Trancsribers.Rules._HEXDIGTranscriber.Instance.Transcribe(_HEXDIG_5, builder);
}

        }
    }
    
}