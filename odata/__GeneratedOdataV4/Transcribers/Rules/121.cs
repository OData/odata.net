namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _annotationTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._annotation>
    {
        private _annotationTranscriber()
        {
        }
        
        public static _annotationTranscriber Instance { get; } = new _annotationTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._annotation value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Rules._ATTranscriber.Instance.Transcribe(value._AT_1, builder);
if (value._namespace_ʺx2Eʺ_1 != null)
{
__GeneratedOdataV4.Trancsribers.Inners._namespace_ʺx2EʺTranscriber.Instance.Transcribe(value._namespace_ʺx2Eʺ_1, builder);
}
__GeneratedOdataV4.Trancsribers.Rules._termNameTranscriber.Instance.Transcribe(value._termName_1, builder);
if (value._ʺx23ʺ_annotationQualifier_1 != null)
{
__GeneratedOdataV4.Trancsribers.Inners._ʺx23ʺ_annotationQualifierTranscriber.Instance.Transcribe(value._ʺx23ʺ_annotationQualifier_1, builder);
}

        }
    }
    
}
