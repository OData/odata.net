namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _containmentNavigationTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._containmentNavigation>
    {
        private _containmentNavigationTranscriber()
        {
        }
        
        public static _containmentNavigationTranscriber Instance { get; } = new _containmentNavigationTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._containmentNavigation value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._keyPredicateTranscriber.Instance.Transcribe(value._keyPredicate_1, builder);
if (value._ʺx2Fʺ_qualifiedEntityTypeName_1 != null)
{
__GeneratedOdata.Trancsribers.Inners._ʺx2Fʺ_qualifiedEntityTypeNameTranscriber.Instance.Transcribe(value._ʺx2Fʺ_qualifiedEntityTypeName_1, builder);
}
__GeneratedOdata.Trancsribers.Rules._navigationTranscriber.Instance.Transcribe(value._navigation_1, builder);

        }
    }
    
}
