namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _collectionNavigationTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._collectionNavigation>
    {
        private _collectionNavigationTranscriber()
        {
        }
        
        public static _collectionNavigationTranscriber Instance { get; } = new _collectionNavigationTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._collectionNavigation value, System.Text.StringBuilder builder)
        {
            if (value._ʺx2Fʺ_qualifiedEntityTypeName_1 != null)
{
__GeneratedOdataV4.Trancsribers.Inners._ʺx2Fʺ_qualifiedEntityTypeNameTranscriber.Instance.Transcribe(value._ʺx2Fʺ_qualifiedEntityTypeName_1, builder);
}
if (value._collectionNavPath_1 != null)
{
__GeneratedOdataV4.Trancsribers.Rules._collectionNavPathTranscriber.Instance.Transcribe(value._collectionNavPath_1, builder);
}

        }
    }
    
}
