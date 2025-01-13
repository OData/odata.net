namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _collectionNavigationTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._collectionNavigation>
    {
        private _collectionNavigationTranscriber()
        {
        }
        
        public static _collectionNavigationTranscriber Instance { get; } = new _collectionNavigationTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._collectionNavigation value, System.Text.StringBuilder builder)
        {
            if (value._ʺx2Fʺ_qualifiedEntityTypeName_1 != null)
{
__GeneratedOdata.Trancsribers.Inners._ʺx2Fʺ_qualifiedEntityTypeNameTranscriber.Instance.Transcribe(value._ʺx2Fʺ_qualifiedEntityTypeName_1, builder);
}
if (value._collectionNavPath_1 != null)
{
__GeneratedOdata.Trancsribers.Rules._collectionNavPathTranscriber.Instance.Transcribe(value._collectionNavPath_1, builder);
}

        }
    }
    
}
