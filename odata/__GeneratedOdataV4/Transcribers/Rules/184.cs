namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _hasExprTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._hasExpr>
    {
        private _hasExprTranscriber()
        {
        }
        
        public static _hasExprTranscriber Instance { get; } = new _hasExprTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._hasExpr value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Rules._RWSTranscriber.Instance.Transcribe(value._RWS_1, builder);
__GeneratedOdataV4.Trancsribers.Inners._ʺx68x61x73ʺTranscriber.Instance.Transcribe(value._ʺx68x61x73ʺ_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._RWSTranscriber.Instance.Transcribe(value._RWS_2, builder);
__GeneratedOdataV4.Trancsribers.Rules._enumTranscriber.Instance.Transcribe(value._enum_1, builder);

        }
    }
    
}
