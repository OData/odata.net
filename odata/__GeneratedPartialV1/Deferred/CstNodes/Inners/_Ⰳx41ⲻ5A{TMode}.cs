namespace __GeneratedPartialV1.Deferred.CstNodes.Inners
{
    using System;
    using CombinatorParsingV3;
    
    public abstract class _Ⰳx41ⲻ5A<TMode> : IAstNode<char, _Ⰳx41ⲻ5A<ParseMode.Realized>>, IFromRealizedable<_Ⰳx41ⲻ5A<ParseMode.Deferred>> where TMode : ParseMode
    {
        private _Ⰳx41ⲻ5A()
        {
        }
        
        internal static _Ⰳx41ⲻ5A<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            return _Ⰳx41ⲻ5A<ParseMode.Deferred>.Deferred.Create(previousNodeRealizationResult);
        }
        
        public abstract _Ⰳx41ⲻ5A<ParseMode.Deferred> Convert();
        public abstract IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>> Realize();
        
        public sealed class Deferred : _Ⰳx41ⲻ5A<ParseMode.Deferred>
        {
            private Deferred(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                this.previousNodeRealizationResult = previousNodeRealizationResult;
                
                this.realizationResult = Future.Create(() => this.RealizeImpl());
            }
            internal Deferred(IFuture<IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>>> realizationResult)
            {
                this.realizationResult = realizationResult;
            }
            
            private IFuture<IRealizationResult<char>> previousNodeRealizationResult { get; }
            private IFuture<IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>>> realizationResult { get; }
            
            internal static _Ⰳx41ⲻ5A<ParseMode.Deferred>.Deferred Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                return new _Ⰳx41ⲻ5A<ParseMode.Deferred>.Deferred(previousNodeRealizationResult);
            }
            
            public override _Ⰳx41ⲻ5A<ParseMode.Deferred> Convert()
            {
                return this;
            }
            
            public override IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>> Realize()
            {
                return this.realizationResult.Value;
            }
            
            private IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>> RealizeImpl()
            {
                throw new Exception("TODO");
            }
        }
        
        public abstract class Realized : _Ⰳx41ⲻ5A<ParseMode.Realized>
        {
            private Realized()
            {
            }
            
            protected abstract TResult Dispatch<TResult, TContext>(_Ⰳx41ⲻ5A<TMode>.Realized.Visitor<TResult, TContext> visitor, TContext context);
            
            public sealed class _41 : _Ⰳx41ⲻ5A<TMode>.Realized
            {
                public override _Ⰳx41ⲻ5A<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _42 : _Ⰳx41ⲻ5A<TMode>.Realized
            {
                public override _Ⰳx41ⲻ5A<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _43 : _Ⰳx41ⲻ5A<TMode>.Realized
            {
                public override _Ⰳx41ⲻ5A<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _44 : _Ⰳx41ⲻ5A<TMode>.Realized
            {
                public override _Ⰳx41ⲻ5A<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _45 : _Ⰳx41ⲻ5A<TMode>.Realized
            {
                public override _Ⰳx41ⲻ5A<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _46 : _Ⰳx41ⲻ5A<TMode>.Realized
            {
                public override _Ⰳx41ⲻ5A<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _47 : _Ⰳx41ⲻ5A<TMode>.Realized
            {
                public override _Ⰳx41ⲻ5A<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _48 : _Ⰳx41ⲻ5A<TMode>.Realized
            {
                public override _Ⰳx41ⲻ5A<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _49 : _Ⰳx41ⲻ5A<TMode>.Realized
            {
                public override _Ⰳx41ⲻ5A<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _4A : _Ⰳx41ⲻ5A<TMode>.Realized
            {
                public override _Ⰳx41ⲻ5A<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _4B : _Ⰳx41ⲻ5A<TMode>.Realized
            {
                public override _Ⰳx41ⲻ5A<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _4C : _Ⰳx41ⲻ5A<TMode>.Realized
            {
                public override _Ⰳx41ⲻ5A<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _4D : _Ⰳx41ⲻ5A<TMode>.Realized
            {
                public override _Ⰳx41ⲻ5A<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _4E : _Ⰳx41ⲻ5A<TMode>.Realized
            {
                public override _Ⰳx41ⲻ5A<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _4F : _Ⰳx41ⲻ5A<TMode>.Realized
            {
                public override _Ⰳx41ⲻ5A<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _50 : _Ⰳx41ⲻ5A<TMode>.Realized
            {
                public override _Ⰳx41ⲻ5A<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _51 : _Ⰳx41ⲻ5A<TMode>.Realized
            {
                public override _Ⰳx41ⲻ5A<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _52 : _Ⰳx41ⲻ5A<TMode>.Realized
            {
                public override _Ⰳx41ⲻ5A<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _53 : _Ⰳx41ⲻ5A<TMode>.Realized
            {
                public override _Ⰳx41ⲻ5A<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _54 : _Ⰳx41ⲻ5A<TMode>.Realized
            {
                public override _Ⰳx41ⲻ5A<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _55 : _Ⰳx41ⲻ5A<TMode>.Realized
            {
                public override _Ⰳx41ⲻ5A<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _56 : _Ⰳx41ⲻ5A<TMode>.Realized
            {
                public override _Ⰳx41ⲻ5A<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _57 : _Ⰳx41ⲻ5A<TMode>.Realized
            {
                public override _Ⰳx41ⲻ5A<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _58 : _Ⰳx41ⲻ5A<TMode>.Realized
            {
                public override _Ⰳx41ⲻ5A<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _59 : _Ⰳx41ⲻ5A<TMode>.Realized
            {
                public override _Ⰳx41ⲻ5A<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _5A : _Ⰳx41ⲻ5A<TMode>.Realized
            {
                public override _Ⰳx41ⲻ5A<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public abstract class Visitor<TResult, TContext>
            {
                public TResult Visit(_Ⰳx41ⲻ5A<ParseMode.Realized> node, TContext context)
                {
                    //// TODO is there a way to avoid this cast?
return (node as _Ⰳx41ⲻ5A<TMode>.Realized)!.Dispatch(this, context);
                }
                
                public TResult Visit(_Ⰳx41ⲻ5A<TMode>.Realized node, TContext context)
                {
                    return node.Dispatch(this, context);
                }
            }
        }
    }
    
}
