namespace __GeneratedOdataV4.CstNodes.Inners
{
    public abstract class _STARⳆstreamPropertyⳆnavigationProperty_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡
    {
        private _STARⳆstreamPropertyⳆnavigationProperty_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_STARⳆstreamPropertyⳆnavigationProperty_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡ node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_STARⳆstreamPropertyⳆnavigationProperty_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡._STAR node, TContext context);
            protected internal abstract TResult Accept(_STARⳆstreamPropertyⳆnavigationProperty_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡._streamProperty node, TContext context);
            protected internal abstract TResult Accept(_STARⳆstreamPropertyⳆnavigationProperty_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡._navigationProperty_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡ node, TContext context);
        }
        
        public sealed class _STAR : _STARⳆstreamPropertyⳆnavigationProperty_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡
        {
            public _STAR(__GeneratedOdataV4.CstNodes.Rules._STAR _STAR_1)
            {
                this._STAR_1 = _STAR_1;
            }
            
            public __GeneratedOdataV4.CstNodes.Rules._STAR _STAR_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _streamProperty : _STARⳆstreamPropertyⳆnavigationProperty_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡
        {
            public _streamProperty(__GeneratedOdataV4.CstNodes.Rules._streamProperty _streamProperty_1)
            {
                this._streamProperty_1 = _streamProperty_1;
            }
            
            public __GeneratedOdataV4.CstNodes.Rules._streamProperty _streamProperty_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _navigationProperty_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡ : _STARⳆstreamPropertyⳆnavigationProperty_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡
        {
            public _navigationProperty_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡(__GeneratedOdataV4.CstNodes.Rules._navigationProperty _navigationProperty_1, __GeneratedOdataV4.CstNodes.Inners._ʺx2Fʺ_qualifiedEntityTypeName? _ʺx2Fʺ_qualifiedEntityTypeName_1)
            {
                this._navigationProperty_1 = _navigationProperty_1;
                this._ʺx2Fʺ_qualifiedEntityTypeName_1 = _ʺx2Fʺ_qualifiedEntityTypeName_1;
            }
            
            public __GeneratedOdataV4.CstNodes.Rules._navigationProperty _navigationProperty_1 { get; }
            public __GeneratedOdataV4.CstNodes.Inners._ʺx2Fʺ_qualifiedEntityTypeName? _ʺx2Fʺ_qualifiedEntityTypeName_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
