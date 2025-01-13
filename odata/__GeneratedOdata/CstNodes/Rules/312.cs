namespace __GeneratedOdata.CstNodes.Rules
{
    public abstract class _geoLiteral
    {
        private _geoLiteral()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_geoLiteral node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_geoLiteral._collectionLiteral node, TContext context);
            protected internal abstract TResult Accept(_geoLiteral._lineStringLiteral node, TContext context);
            protected internal abstract TResult Accept(_geoLiteral._multiPointLiteral node, TContext context);
            protected internal abstract TResult Accept(_geoLiteral._multiLineStringLiteral node, TContext context);
            protected internal abstract TResult Accept(_geoLiteral._multiPolygonLiteral node, TContext context);
            protected internal abstract TResult Accept(_geoLiteral._pointLiteral node, TContext context);
            protected internal abstract TResult Accept(_geoLiteral._polygonLiteral node, TContext context);
        }
        
        public sealed class _collectionLiteral : _geoLiteral
        {
            public _collectionLiteral(__GeneratedOdata.CstNodes.Rules._collectionLiteral _collectionLiteral_1)
            {
                this._collectionLiteral_1 = _collectionLiteral_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._collectionLiteral _collectionLiteral_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _lineStringLiteral : _geoLiteral
        {
            public _lineStringLiteral(__GeneratedOdata.CstNodes.Rules._lineStringLiteral _lineStringLiteral_1)
            {
                this._lineStringLiteral_1 = _lineStringLiteral_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._lineStringLiteral _lineStringLiteral_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _multiPointLiteral : _geoLiteral
        {
            public _multiPointLiteral(__GeneratedOdata.CstNodes.Rules._multiPointLiteral _multiPointLiteral_1)
            {
                this._multiPointLiteral_1 = _multiPointLiteral_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._multiPointLiteral _multiPointLiteral_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _multiLineStringLiteral : _geoLiteral
        {
            public _multiLineStringLiteral(__GeneratedOdata.CstNodes.Rules._multiLineStringLiteral _multiLineStringLiteral_1)
            {
                this._multiLineStringLiteral_1 = _multiLineStringLiteral_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._multiLineStringLiteral _multiLineStringLiteral_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _multiPolygonLiteral : _geoLiteral
        {
            public _multiPolygonLiteral(__GeneratedOdata.CstNodes.Rules._multiPolygonLiteral _multiPolygonLiteral_1)
            {
                this._multiPolygonLiteral_1 = _multiPolygonLiteral_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._multiPolygonLiteral _multiPolygonLiteral_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _pointLiteral : _geoLiteral
        {
            public _pointLiteral(__GeneratedOdata.CstNodes.Rules._pointLiteral _pointLiteral_1)
            {
                this._pointLiteral_1 = _pointLiteral_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._pointLiteral _pointLiteral_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _polygonLiteral : _geoLiteral
        {
            public _polygonLiteral(__GeneratedOdata.CstNodes.Rules._polygonLiteral _polygonLiteral_1)
            {
                this._polygonLiteral_1 = _polygonLiteral_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._polygonLiteral _polygonLiteral_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
