namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _URITranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._URI>
    {
        private _URITranscriber()
        {
        }
        
        public static _URITranscriber Instance { get; } = new _URITranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._URI value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._schemeTranscriber.Instance.Transcribe(value._scheme_1, builder);
__GeneratedOdata.Trancsribers.Inners._ʺx3AʺTranscriber.Instance.Transcribe(value._ʺx3Aʺ_1, builder);
__GeneratedOdata.Trancsribers.Rules._hierⲻpartTranscriber.Instance.Transcribe(value._hierⲻpart_1, builder);
if (value._ʺx3Fʺ_query_1 != null)
{
__GeneratedOdata.Trancsribers.Inners._ʺx3Fʺ_queryTranscriber.Instance.Transcribe(value._ʺx3Fʺ_query_1, builder);
}
if (value._ʺx23ʺ_fragment_1 != null)
{
__GeneratedOdata.Trancsribers.Inners._ʺx23ʺ_fragmentTranscriber.Instance.Transcribe(value._ʺx23ʺ_fragment_1, builder);
}

        }
    }
    
}
