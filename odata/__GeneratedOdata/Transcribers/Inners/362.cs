namespace __GeneratedOdata.Trancsribers.Inners
{
    public sealed class _complexInUriⳆcomplexColInUriⳆprimitiveLiteralInJSONⳆprimitiveColInUriTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Inners._complexInUriⳆcomplexColInUriⳆprimitiveLiteralInJSONⳆprimitiveColInUri>
    {
        private _complexInUriⳆcomplexColInUriⳆprimitiveLiteralInJSONⳆprimitiveColInUriTranscriber()
        {
        }
        
        public static _complexInUriⳆcomplexColInUriⳆprimitiveLiteralInJSONⳆprimitiveColInUriTranscriber Instance { get; } = new _complexInUriⳆcomplexColInUriⳆprimitiveLiteralInJSONⳆprimitiveColInUriTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Inners._complexInUriⳆcomplexColInUriⳆprimitiveLiteralInJSONⳆprimitiveColInUri value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdata.CstNodes.Inners._complexInUriⳆcomplexColInUriⳆprimitiveLiteralInJSONⳆprimitiveColInUri.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Inners._complexInUriⳆcomplexColInUriⳆprimitiveLiteralInJSONⳆprimitiveColInUri._complexInUri node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._complexInUriTranscriber.Instance.Transcribe(node._complexInUri_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Inners._complexInUriⳆcomplexColInUriⳆprimitiveLiteralInJSONⳆprimitiveColInUri._complexColInUri node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._complexColInUriTranscriber.Instance.Transcribe(node._complexColInUri_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Inners._complexInUriⳆcomplexColInUriⳆprimitiveLiteralInJSONⳆprimitiveColInUri._primitiveLiteralInJSON node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._primitiveLiteralInJSONTranscriber.Instance.Transcribe(node._primitiveLiteralInJSON_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Inners._complexInUriⳆcomplexColInUriⳆprimitiveLiteralInJSONⳆprimitiveColInUri._primitiveColInUri node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._primitiveColInUriTranscriber.Instance.Transcribe(node._primitiveColInUri_1, context);

return default;
            }
        }
    }
    
}
