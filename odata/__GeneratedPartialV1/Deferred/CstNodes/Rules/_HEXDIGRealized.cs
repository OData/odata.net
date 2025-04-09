namespace __GeneratedPartialV1.Deferred.CstNodes.Rules
{
    using System;
    using CombinatorParsingV3;
    
    public abstract class _HEXDIGRealized
    {
        private _HEXDIGRealized()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_HEXDIGRealized node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_DIGIT node, TContext context);
            protected internal abstract TResult Accept(_ʺx41ʺ node, TContext context);
            protected internal abstract TResult Accept(_ʺx42ʺ node, TContext context);
            protected internal abstract TResult Accept(_ʺx43ʺ node, TContext context);
            protected internal abstract TResult Accept(_ʺx44ʺ node, TContext context);
            protected internal abstract TResult Accept(_ʺx45ʺ node, TContext context);
            protected internal abstract TResult Accept(_ʺx46ʺ node, TContext context);
        }
        
        public sealed class _DIGIT : _HEXDIGRealized
        {
            private _DIGIT(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _HEXDIGRealized._DIGIT>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _HEXDIGRealized._DIGIT> RealizationResult { get; }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx41ʺ : _HEXDIGRealized
        {
            private _ʺx41ʺ(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _HEXDIGRealized._ʺx41ʺ>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _HEXDIGRealized._ʺx41ʺ> RealizationResult { get; }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx42ʺ : _HEXDIGRealized
        {
            private _ʺx42ʺ(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _HEXDIGRealized._ʺx42ʺ>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _HEXDIGRealized._ʺx42ʺ> RealizationResult { get; }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx43ʺ : _HEXDIGRealized
        {
            private _ʺx43ʺ(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _HEXDIGRealized._ʺx43ʺ>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _HEXDIGRealized._ʺx43ʺ> RealizationResult { get; }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx44ʺ : _HEXDIGRealized
        {
            private _ʺx44ʺ(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _HEXDIGRealized._ʺx44ʺ>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _HEXDIGRealized._ʺx44ʺ> RealizationResult { get; }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx45ʺ : _HEXDIGRealized
        {
            private _ʺx45ʺ(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _HEXDIGRealized._ʺx45ʺ>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _HEXDIGRealized._ʺx45ʺ> RealizationResult { get; }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx46ʺ : _HEXDIGRealized
        {
            private _ʺx46ʺ(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _HEXDIGRealized._ʺx46ʺ>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _HEXDIGRealized._ʺx46ʺ> RealizationResult { get; }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
