namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _geoLiteralTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._geoLiteral>
    {
        private _geoLiteralTranscriber()
        {
        }
        
        public static _geoLiteralTranscriber Instance { get; } = new _geoLiteralTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._geoLiteral value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdata.CstNodes.Rules._geoLiteral.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._geoLiteral._collectionLiteral node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._collectionLiteralTranscriber.Instance.Transcribe(node._collectionLiteral_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._geoLiteral._lineStringLiteral node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._lineStringLiteralTranscriber.Instance.Transcribe(node._lineStringLiteral_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._geoLiteral._multiPointLiteral node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._multiPointLiteralTranscriber.Instance.Transcribe(node._multiPointLiteral_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._geoLiteral._multiLineStringLiteral node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._multiLineStringLiteralTranscriber.Instance.Transcribe(node._multiLineStringLiteral_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._geoLiteral._multiPolygonLiteral node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._multiPolygonLiteralTranscriber.Instance.Transcribe(node._multiPolygonLiteral_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._geoLiteral._pointLiteral node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._pointLiteralTranscriber.Instance.Transcribe(node._pointLiteral_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._geoLiteral._polygonLiteral node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._polygonLiteralTranscriber.Instance.Transcribe(node._polygonLiteral_1, context);

return default;
            }
        }
    }
    
}
