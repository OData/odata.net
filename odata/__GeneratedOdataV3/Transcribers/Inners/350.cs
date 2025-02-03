namespace __GeneratedOdataV3.Trancsribers.Inners
{
    public sealed class _annotationInUriⳆprimitivePropertyInUriⳆcomplexPropertyInUriⳆcollectionPropertyInUriⳆnavigationPropertyInUriTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Inners._annotationInUriⳆprimitivePropertyInUriⳆcomplexPropertyInUriⳆcollectionPropertyInUriⳆnavigationPropertyInUri>
    {
        private _annotationInUriⳆprimitivePropertyInUriⳆcomplexPropertyInUriⳆcollectionPropertyInUriⳆnavigationPropertyInUriTranscriber()
        {
        }
        
        public static _annotationInUriⳆprimitivePropertyInUriⳆcomplexPropertyInUriⳆcollectionPropertyInUriⳆnavigationPropertyInUriTranscriber Instance { get; } = new _annotationInUriⳆprimitivePropertyInUriⳆcomplexPropertyInUriⳆcollectionPropertyInUriⳆnavigationPropertyInUriTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Inners._annotationInUriⳆprimitivePropertyInUriⳆcomplexPropertyInUriⳆcollectionPropertyInUriⳆnavigationPropertyInUri value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV3.CstNodes.Inners._annotationInUriⳆprimitivePropertyInUriⳆcomplexPropertyInUriⳆcollectionPropertyInUriⳆnavigationPropertyInUri.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Inners._annotationInUriⳆprimitivePropertyInUriⳆcomplexPropertyInUriⳆcollectionPropertyInUriⳆnavigationPropertyInUri._annotationInUri node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._annotationInUriTranscriber.Instance.Transcribe(node._annotationInUri_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Inners._annotationInUriⳆprimitivePropertyInUriⳆcomplexPropertyInUriⳆcollectionPropertyInUriⳆnavigationPropertyInUri._primitivePropertyInUri node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._primitivePropertyInUriTranscriber.Instance.Transcribe(node._primitivePropertyInUri_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Inners._annotationInUriⳆprimitivePropertyInUriⳆcomplexPropertyInUriⳆcollectionPropertyInUriⳆnavigationPropertyInUri._complexPropertyInUri node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._complexPropertyInUriTranscriber.Instance.Transcribe(node._complexPropertyInUri_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Inners._annotationInUriⳆprimitivePropertyInUriⳆcomplexPropertyInUriⳆcollectionPropertyInUriⳆnavigationPropertyInUri._collectionPropertyInUri node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._collectionPropertyInUriTranscriber.Instance.Transcribe(node._collectionPropertyInUri_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Inners._annotationInUriⳆprimitivePropertyInUriⳆcomplexPropertyInUriⳆcollectionPropertyInUriⳆnavigationPropertyInUri._navigationPropertyInUri node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._navigationPropertyInUriTranscriber.Instance.Transcribe(node._navigationPropertyInUri_1, context);

return default;
            }
        }
    }
    
}
