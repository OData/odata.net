namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _floorMethodCallExprTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._floorMethodCallExpr>
    {
        private _floorMethodCallExprTranscriber()
        {
        }
        
        public static _floorMethodCallExprTranscriber Instance { get; } = new _floorMethodCallExprTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._floorMethodCallExpr value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Inners._ʺx66x6Cx6Fx6Fx72ʺTranscriber.Instance.Transcribe(value._ʺx66x6Cx6Fx6Fx72ʺ_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._OPENTranscriber.Instance.Transcribe(value._OPEN_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._BWSTranscriber.Instance.Transcribe(value._BWS_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._commonExprTranscriber.Instance.Transcribe(value._commonExpr_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._BWSTranscriber.Instance.Transcribe(value._BWS_2, builder);
__GeneratedOdataV4.Trancsribers.Rules._CLOSETranscriber.Instance.Transcribe(value._CLOSE_1, builder);

        }
    }
    
}
