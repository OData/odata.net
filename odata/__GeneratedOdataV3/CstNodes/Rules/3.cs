namespace __GeneratedOdataV3.CstNodes.Rules
{
    public abstract class _odataRelativeUri
    {
        private _odataRelativeUri()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_odataRelativeUri node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_odataRelativeUri._ʺx24x62x61x74x63x68ʺ_꘡ʺx3Fʺ_batchOptions꘡ node, TContext context);
            protected internal abstract TResult Accept(_odataRelativeUri._ʺx24x65x6Ex74x69x74x79ʺ_ʺx3Fʺ_entityOptions node, TContext context);
            protected internal abstract TResult Accept(_odataRelativeUri._ʺx24x65x6Ex74x69x74x79ʺ_ʺx2Fʺ_qualifiedEntityTypeName_ʺx3Fʺ_entityCastOptions node, TContext context);
            protected internal abstract TResult Accept(_odataRelativeUri._ʺx24x6Dx65x74x61x64x61x74x61ʺ_꘡ʺx3Fʺ_metadataOptions꘡_꘡context꘡ node, TContext context);
            protected internal abstract TResult Accept(_odataRelativeUri._resourcePath_꘡ʺx3Fʺ_queryOptions꘡ node, TContext context);
        }
        
        public sealed class _ʺx24x62x61x74x63x68ʺ_꘡ʺx3Fʺ_batchOptions꘡ : _odataRelativeUri
        {
            public _ʺx24x62x61x74x63x68ʺ_꘡ʺx3Fʺ_batchOptions꘡(__GeneratedOdataV3.CstNodes.Inners._ʺx24x62x61x74x63x68ʺ _ʺx24x62x61x74x63x68ʺ_1, __GeneratedOdataV3.CstNodes.Inners._ʺx3Fʺ_batchOptions? _ʺx3Fʺ_batchOptions_1)
            {
                this._ʺx24x62x61x74x63x68ʺ_1 = _ʺx24x62x61x74x63x68ʺ_1;
                this._ʺx3Fʺ_batchOptions_1 = _ʺx3Fʺ_batchOptions_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Inners._ʺx24x62x61x74x63x68ʺ _ʺx24x62x61x74x63x68ʺ_1 { get; }
            public __GeneratedOdataV3.CstNodes.Inners._ʺx3Fʺ_batchOptions? _ʺx3Fʺ_batchOptions_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx24x65x6Ex74x69x74x79ʺ_ʺx3Fʺ_entityOptions : _odataRelativeUri
        {
            public _ʺx24x65x6Ex74x69x74x79ʺ_ʺx3Fʺ_entityOptions(__GeneratedOdataV3.CstNodes.Inners._ʺx24x65x6Ex74x69x74x79ʺ _ʺx24x65x6Ex74x69x74x79ʺ_1, __GeneratedOdataV3.CstNodes.Inners._ʺx3Fʺ _ʺx3Fʺ_1, __GeneratedOdataV3.CstNodes.Rules._entityOptions _entityOptions_1)
            {
                this._ʺx24x65x6Ex74x69x74x79ʺ_1 = _ʺx24x65x6Ex74x69x74x79ʺ_1;
                this._ʺx3Fʺ_1 = _ʺx3Fʺ_1;
                this._entityOptions_1 = _entityOptions_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Inners._ʺx24x65x6Ex74x69x74x79ʺ _ʺx24x65x6Ex74x69x74x79ʺ_1 { get; }
            public __GeneratedOdataV3.CstNodes.Inners._ʺx3Fʺ _ʺx3Fʺ_1 { get; }
            public __GeneratedOdataV3.CstNodes.Rules._entityOptions _entityOptions_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx24x65x6Ex74x69x74x79ʺ_ʺx2Fʺ_qualifiedEntityTypeName_ʺx3Fʺ_entityCastOptions : _odataRelativeUri
        {
            public _ʺx24x65x6Ex74x69x74x79ʺ_ʺx2Fʺ_qualifiedEntityTypeName_ʺx3Fʺ_entityCastOptions(__GeneratedOdataV3.CstNodes.Inners._ʺx24x65x6Ex74x69x74x79ʺ _ʺx24x65x6Ex74x69x74x79ʺ_1, __GeneratedOdataV3.CstNodes.Inners._ʺx2Fʺ _ʺx2Fʺ_1, __GeneratedOdataV3.CstNodes.Rules._qualifiedEntityTypeName _qualifiedEntityTypeName_1, __GeneratedOdataV3.CstNodes.Inners._ʺx3Fʺ _ʺx3Fʺ_1, __GeneratedOdataV3.CstNodes.Rules._entityCastOptions _entityCastOptions_1)
            {
                this._ʺx24x65x6Ex74x69x74x79ʺ_1 = _ʺx24x65x6Ex74x69x74x79ʺ_1;
                this._ʺx2Fʺ_1 = _ʺx2Fʺ_1;
                this._qualifiedEntityTypeName_1 = _qualifiedEntityTypeName_1;
                this._ʺx3Fʺ_1 = _ʺx3Fʺ_1;
                this._entityCastOptions_1 = _entityCastOptions_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Inners._ʺx24x65x6Ex74x69x74x79ʺ _ʺx24x65x6Ex74x69x74x79ʺ_1 { get; }
            public __GeneratedOdataV3.CstNodes.Inners._ʺx2Fʺ _ʺx2Fʺ_1 { get; }
            public __GeneratedOdataV3.CstNodes.Rules._qualifiedEntityTypeName _qualifiedEntityTypeName_1 { get; }
            public __GeneratedOdataV3.CstNodes.Inners._ʺx3Fʺ _ʺx3Fʺ_1 { get; }
            public __GeneratedOdataV3.CstNodes.Rules._entityCastOptions _entityCastOptions_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx24x6Dx65x74x61x64x61x74x61ʺ_꘡ʺx3Fʺ_metadataOptions꘡_꘡context꘡ : _odataRelativeUri
        {
            public _ʺx24x6Dx65x74x61x64x61x74x61ʺ_꘡ʺx3Fʺ_metadataOptions꘡_꘡context꘡(__GeneratedOdataV3.CstNodes.Inners._ʺx24x6Dx65x74x61x64x61x74x61ʺ _ʺx24x6Dx65x74x61x64x61x74x61ʺ_1, __GeneratedOdataV3.CstNodes.Inners._ʺx3Fʺ_metadataOptions? _ʺx3Fʺ_metadataOptions_1, __GeneratedOdataV3.CstNodes.Rules._context? _context_1)
            {
                this._ʺx24x6Dx65x74x61x64x61x74x61ʺ_1 = _ʺx24x6Dx65x74x61x64x61x74x61ʺ_1;
                this._ʺx3Fʺ_metadataOptions_1 = _ʺx3Fʺ_metadataOptions_1;
                this._context_1 = _context_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Inners._ʺx24x6Dx65x74x61x64x61x74x61ʺ _ʺx24x6Dx65x74x61x64x61x74x61ʺ_1 { get; }
            public __GeneratedOdataV3.CstNodes.Inners._ʺx3Fʺ_metadataOptions? _ʺx3Fʺ_metadataOptions_1 { get; }
            public __GeneratedOdataV3.CstNodes.Rules._context? _context_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _resourcePath_꘡ʺx3Fʺ_queryOptions꘡ : _odataRelativeUri
        {
            public _resourcePath_꘡ʺx3Fʺ_queryOptions꘡(__GeneratedOdataV3.CstNodes.Rules._resourcePath _resourcePath_1, __GeneratedOdataV3.CstNodes.Inners._ʺx3Fʺ_queryOptions? _ʺx3Fʺ_queryOptions_1)
            {
                this._resourcePath_1 = _resourcePath_1;
                this._ʺx3Fʺ_queryOptions_1 = _ʺx3Fʺ_queryOptions_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._resourcePath _resourcePath_1 { get; }
            public __GeneratedOdataV3.CstNodes.Inners._ʺx3Fʺ_queryOptions? _ʺx3Fʺ_queryOptions_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
