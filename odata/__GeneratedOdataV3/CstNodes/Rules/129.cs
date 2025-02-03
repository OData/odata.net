namespace __GeneratedOdataV3.CstNodes.Rules
{
    public abstract class _collectionPathExpr
    {
        private _collectionPathExpr()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_collectionPathExpr node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_collectionPathExpr._count_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡ node, TContext context);
            protected internal abstract TResult Accept(_collectionPathExpr._ʺx2Fʺ_boundFunctionExpr node, TContext context);
            protected internal abstract TResult Accept(_collectionPathExpr._ʺx2Fʺ_annotationExpr node, TContext context);
            protected internal abstract TResult Accept(_collectionPathExpr._ʺx2Fʺ_anyExpr node, TContext context);
            protected internal abstract TResult Accept(_collectionPathExpr._ʺx2Fʺ_allExpr node, TContext context);
        }
        
        public sealed class _count_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡ : _collectionPathExpr
        {
            public _count_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡(__GeneratedOdataV3.CstNodes.Rules._count _count_1, __GeneratedOdataV3.CstNodes.Inners._OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE? _OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE_1)
            {
                this._count_1 = _count_1;
                this._OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE_1 = _OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._count _count_1 { get; }
            public __GeneratedOdataV3.CstNodes.Inners._OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE? _OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx2Fʺ_boundFunctionExpr : _collectionPathExpr
        {
            public _ʺx2Fʺ_boundFunctionExpr(__GeneratedOdataV3.CstNodes.Inners._ʺx2Fʺ _ʺx2Fʺ_1, __GeneratedOdataV3.CstNodes.Rules._boundFunctionExpr _boundFunctionExpr_1)
            {
                this._ʺx2Fʺ_1 = _ʺx2Fʺ_1;
                this._boundFunctionExpr_1 = _boundFunctionExpr_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Inners._ʺx2Fʺ _ʺx2Fʺ_1 { get; }
            public __GeneratedOdataV3.CstNodes.Rules._boundFunctionExpr _boundFunctionExpr_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx2Fʺ_annotationExpr : _collectionPathExpr
        {
            public _ʺx2Fʺ_annotationExpr(__GeneratedOdataV3.CstNodes.Inners._ʺx2Fʺ _ʺx2Fʺ_1, __GeneratedOdataV3.CstNodes.Rules._annotationExpr _annotationExpr_1)
            {
                this._ʺx2Fʺ_1 = _ʺx2Fʺ_1;
                this._annotationExpr_1 = _annotationExpr_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Inners._ʺx2Fʺ _ʺx2Fʺ_1 { get; }
            public __GeneratedOdataV3.CstNodes.Rules._annotationExpr _annotationExpr_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx2Fʺ_anyExpr : _collectionPathExpr
        {
            public _ʺx2Fʺ_anyExpr(__GeneratedOdataV3.CstNodes.Inners._ʺx2Fʺ _ʺx2Fʺ_1, __GeneratedOdataV3.CstNodes.Rules._anyExpr _anyExpr_1)
            {
                this._ʺx2Fʺ_1 = _ʺx2Fʺ_1;
                this._anyExpr_1 = _anyExpr_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Inners._ʺx2Fʺ _ʺx2Fʺ_1 { get; }
            public __GeneratedOdataV3.CstNodes.Rules._anyExpr _anyExpr_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx2Fʺ_allExpr : _collectionPathExpr
        {
            public _ʺx2Fʺ_allExpr(__GeneratedOdataV3.CstNodes.Inners._ʺx2Fʺ _ʺx2Fʺ_1, __GeneratedOdataV3.CstNodes.Rules._allExpr _allExpr_1)
            {
                this._ʺx2Fʺ_1 = _ʺx2Fʺ_1;
                this._allExpr_1 = _allExpr_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Inners._ʺx2Fʺ _ʺx2Fʺ_1 { get; }
            public __GeneratedOdataV3.CstNodes.Rules._allExpr _allExpr_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
