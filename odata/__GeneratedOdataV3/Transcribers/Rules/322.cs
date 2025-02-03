namespace __GeneratedOdataV3.Trancsribers.Rules
{
    public sealed class _multiPointLiteralTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Rules._multiPointLiteral>
    {
        private _multiPointLiteralTranscriber()
        {
        }
        
        public static _multiPointLiteralTranscriber Instance { get; } = new _multiPointLiteralTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Rules._multiPointLiteral value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV3.Trancsribers.Inners._ʺx4Dx75x6Cx74x69x50x6Fx69x6Ex74x28ʺTranscriber.Instance.Transcribe(value._ʺx4Dx75x6Cx74x69x50x6Fx69x6Ex74x28ʺ_1, builder);
if (value._pointData_ЖⲤCOMMA_pointDataↃ_1 != null)
{
__GeneratedOdataV3.Trancsribers.Inners._pointData_ЖⲤCOMMA_pointDataↃTranscriber.Instance.Transcribe(value._pointData_ЖⲤCOMMA_pointDataↃ_1, builder);
}
__GeneratedOdataV3.Trancsribers.Rules._CLOSETranscriber.Instance.Transcribe(value._CLOSE_1, builder);

        }
    }
    
}
