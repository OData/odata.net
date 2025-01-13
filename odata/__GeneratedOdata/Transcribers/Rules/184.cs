namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _hasExprTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._hasExpr>
    {
        private _hasExprTranscriber()
        {
        }
        
        public static _hasExprTranscriber Instance { get; } = new _hasExprTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._hasExpr value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._RWSTranscriber.Instance.Transcribe(value._RWS_1, builder);
__GeneratedOdata.Trancsribers.Inners._ʺx68x61x73ʺTranscriber.Instance.Transcribe(value._ʺx68x61x73ʺ_1, builder);
__GeneratedOdata.Trancsribers.Rules._RWSTranscriber.Instance.Transcribe(value._RWS_2, builder);
__GeneratedOdata.Trancsribers.Rules._enumTranscriber.Instance.Transcribe(value._enum_1, builder);

        }
    }
    
}
