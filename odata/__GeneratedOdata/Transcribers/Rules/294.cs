namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _durationValueTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._durationValue>
    {
        private _durationValueTranscriber()
        {
        }
        
        public static _durationValueTranscriber Instance { get; } = new _durationValueTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._durationValue value, System.Text.StringBuilder builder)
        {
            if (value._SIGN_1 != null)
{
__GeneratedOdata.Trancsribers.Rules._SIGNTranscriber.Instance.Transcribe(value._SIGN_1, builder);
}
__GeneratedOdata.Trancsribers.Inners._ʺx50ʺTranscriber.Instance.Transcribe(value._ʺx50ʺ_1, builder);
if (value._1ЖDIGIT_ʺx44ʺ_1 != null)
{
__GeneratedOdata.Trancsribers.Inners._1ЖDIGIT_ʺx44ʺTranscriber.Instance.Transcribe(value._1ЖDIGIT_ʺx44ʺ_1, builder);
}
if (value._ʺx54ʺ_꘡1ЖDIGIT_ʺx48ʺ꘡_꘡1ЖDIGIT_ʺx4Dʺ꘡_꘡1ЖDIGIT_꘡ʺx2Eʺ_1ЖDIGIT꘡_ʺx53ʺ꘡_1 != null)
{
__GeneratedOdata.Trancsribers.Inners._ʺx54ʺ_꘡1ЖDIGIT_ʺx48ʺ꘡_꘡1ЖDIGIT_ʺx4Dʺ꘡_꘡1ЖDIGIT_꘡ʺx2Eʺ_1ЖDIGIT꘡_ʺx53ʺ꘡Transcriber.Instance.Transcribe(value._ʺx54ʺ_꘡1ЖDIGIT_ʺx48ʺ꘡_꘡1ЖDIGIT_ʺx4Dʺ꘡_꘡1ЖDIGIT_꘡ʺx2Eʺ_1ЖDIGIT꘡_ʺx53ʺ꘡_1, builder);
}

        }
    }
    
}
