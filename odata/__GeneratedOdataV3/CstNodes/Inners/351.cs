namespace __GeneratedOdataV3.CstNodes.Inners
{
    public abstract class _annotationInUriⳆprimitivePropertyInUriⳆcomplexPropertyInUriⳆcollectionPropertyInUriⳆnavigationPropertyInUri
    {
        private _annotationInUriⳆprimitivePropertyInUriⳆcomplexPropertyInUriⳆcollectionPropertyInUriⳆnavigationPropertyInUri()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_annotationInUriⳆprimitivePropertyInUriⳆcomplexPropertyInUriⳆcollectionPropertyInUriⳆnavigationPropertyInUri node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_annotationInUriⳆprimitivePropertyInUriⳆcomplexPropertyInUriⳆcollectionPropertyInUriⳆnavigationPropertyInUri._annotationInUri node, TContext context);
            protected internal abstract TResult Accept(_annotationInUriⳆprimitivePropertyInUriⳆcomplexPropertyInUriⳆcollectionPropertyInUriⳆnavigationPropertyInUri._primitivePropertyInUri node, TContext context);
            protected internal abstract TResult Accept(_annotationInUriⳆprimitivePropertyInUriⳆcomplexPropertyInUriⳆcollectionPropertyInUriⳆnavigationPropertyInUri._complexPropertyInUri node, TContext context);
            protected internal abstract TResult Accept(_annotationInUriⳆprimitivePropertyInUriⳆcomplexPropertyInUriⳆcollectionPropertyInUriⳆnavigationPropertyInUri._collectionPropertyInUri node, TContext context);
            protected internal abstract TResult Accept(_annotationInUriⳆprimitivePropertyInUriⳆcomplexPropertyInUriⳆcollectionPropertyInUriⳆnavigationPropertyInUri._navigationPropertyInUri node, TContext context);
        }
        
        public sealed class _annotationInUri : _annotationInUriⳆprimitivePropertyInUriⳆcomplexPropertyInUriⳆcollectionPropertyInUriⳆnavigationPropertyInUri
        {
            public _annotationInUri(__GeneratedOdataV3.CstNodes.Rules._annotationInUri _annotationInUri_1)
            {
                this._annotationInUri_1 = _annotationInUri_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._annotationInUri _annotationInUri_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _primitivePropertyInUri : _annotationInUriⳆprimitivePropertyInUriⳆcomplexPropertyInUriⳆcollectionPropertyInUriⳆnavigationPropertyInUri
        {
            public _primitivePropertyInUri(__GeneratedOdataV3.CstNodes.Rules._primitivePropertyInUri _primitivePropertyInUri_1)
            {
                this._primitivePropertyInUri_1 = _primitivePropertyInUri_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._primitivePropertyInUri _primitivePropertyInUri_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _complexPropertyInUri : _annotationInUriⳆprimitivePropertyInUriⳆcomplexPropertyInUriⳆcollectionPropertyInUriⳆnavigationPropertyInUri
        {
            public _complexPropertyInUri(__GeneratedOdataV3.CstNodes.Rules._complexPropertyInUri _complexPropertyInUri_1)
            {
                this._complexPropertyInUri_1 = _complexPropertyInUri_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._complexPropertyInUri _complexPropertyInUri_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _collectionPropertyInUri : _annotationInUriⳆprimitivePropertyInUriⳆcomplexPropertyInUriⳆcollectionPropertyInUriⳆnavigationPropertyInUri
        {
            public _collectionPropertyInUri(__GeneratedOdataV3.CstNodes.Rules._collectionPropertyInUri _collectionPropertyInUri_1)
            {
                this._collectionPropertyInUri_1 = _collectionPropertyInUri_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._collectionPropertyInUri _collectionPropertyInUri_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _navigationPropertyInUri : _annotationInUriⳆprimitivePropertyInUriⳆcomplexPropertyInUriⳆcollectionPropertyInUriⳆnavigationPropertyInUri
        {
            public _navigationPropertyInUri(__GeneratedOdataV3.CstNodes.Rules._navigationPropertyInUri _navigationPropertyInUri_1)
            {
                this._navigationPropertyInUri_1 = _navigationPropertyInUri_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._navigationPropertyInUri _navigationPropertyInUri_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
