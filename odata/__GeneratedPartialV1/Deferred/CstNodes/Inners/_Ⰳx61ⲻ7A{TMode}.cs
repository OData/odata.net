namespace __GeneratedPartialV1.Deferred.CstNodes.Inners
{
    using System;
    using CombinatorParsingV3;
    
    public abstract class _Ⰳx61ⲻ7A<TMode> : IAstNode<char, _Ⰳx61ⲻ7A<ParseMode.Realized>>, IFromRealizedable<_Ⰳx61ⲻ7A<ParseMode.Deferred>> where TMode : ParseMode
    {
        private _Ⰳx61ⲻ7A()
        {
        }
        
        internal static _Ⰳx61ⲻ7A<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            return _Ⰳx61ⲻ7A<ParseMode.Deferred>.Deferred.Create(previousNodeRealizationResult);
        }
        
        public abstract _Ⰳx61ⲻ7A<ParseMode.Deferred> Convert();
        public abstract IRealizationResult<char, _Ⰳx61ⲻ7A<ParseMode.Realized>> Realize();
        
        public sealed class Deferred : _Ⰳx61ⲻ7A<ParseMode.Deferred>
        {
            private Deferred(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                this.previousNodeRealizationResult = previousNodeRealizationResult;
                
                this.realizationResult = Future.Create(() => this.RealizeImpl());
            }
            internal Deferred(IFuture<IRealizationResult<char, _Ⰳx61ⲻ7A<ParseMode.Realized>>> realizationResult)
            {
                this.realizationResult = realizationResult;
            }
            
            private IFuture<IRealizationResult<char>> previousNodeRealizationResult { get; }
            private IFuture<IRealizationResult<char, _Ⰳx61ⲻ7A<ParseMode.Realized>>> realizationResult { get; }
            
            internal static _Ⰳx61ⲻ7A<ParseMode.Deferred>.Deferred Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                return new _Ⰳx61ⲻ7A<ParseMode.Deferred>.Deferred(previousNodeRealizationResult);
            }
            
            public override _Ⰳx61ⲻ7A<ParseMode.Deferred> Convert()
            {
                return this;
            }
            
            public override IRealizationResult<char, _Ⰳx61ⲻ7A<ParseMode.Realized>> Realize()
            {
                return this.realizationResult.Value;
            }
            
            private IRealizationResult<char, _Ⰳx61ⲻ7A<ParseMode.Realized>> RealizeImpl()
            {
                throw new Exception("TODO");
            }
        }
        
        public abstract class Realized : _Ⰳx61ⲻ7A<ParseMode.Realized>
        {
            private Realized()
            {
            }
            
            protected abstract TResult Dispatch<TResult, TContext>(_Ⰳx61ⲻ7A<TMode>.Realized.Visitor<TResult, TContext> visitor, TContext context);
            
            public sealed class _61 : _Ⰳx61ⲻ7A<TMode>.Realized
            {
                public override _Ⰳx61ⲻ7A<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx61ⲻ7A<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _62 : _Ⰳx61ⲻ7A<TMode>.Realized
            {
                public override _Ⰳx61ⲻ7A<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx61ⲻ7A<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _63 : _Ⰳx61ⲻ7A<TMode>.Realized
            {
                public override _Ⰳx61ⲻ7A<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx61ⲻ7A<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _64 : _Ⰳx61ⲻ7A<TMode>.Realized
            {
                public override _Ⰳx61ⲻ7A<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx61ⲻ7A<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _65 : _Ⰳx61ⲻ7A<TMode>.Realized
            {
                public override _Ⰳx61ⲻ7A<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx61ⲻ7A<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _66 : _Ⰳx61ⲻ7A<TMode>.Realized
            {
                public override _Ⰳx61ⲻ7A<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx61ⲻ7A<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _67 : _Ⰳx61ⲻ7A<TMode>.Realized
            {
                public override _Ⰳx61ⲻ7A<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx61ⲻ7A<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _68 : _Ⰳx61ⲻ7A<TMode>.Realized
            {
                public override _Ⰳx61ⲻ7A<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx61ⲻ7A<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _69 : _Ⰳx61ⲻ7A<TMode>.Realized
            {
                public override _Ⰳx61ⲻ7A<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx61ⲻ7A<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _6A : _Ⰳx61ⲻ7A<TMode>.Realized
            {
                public override _Ⰳx61ⲻ7A<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx61ⲻ7A<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _6B : _Ⰳx61ⲻ7A<TMode>.Realized
            {
                public override _Ⰳx61ⲻ7A<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx61ⲻ7A<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _6C : _Ⰳx61ⲻ7A<TMode>.Realized
            {
                public override _Ⰳx61ⲻ7A<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx61ⲻ7A<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _6D : _Ⰳx61ⲻ7A<TMode>.Realized
            {
                public override _Ⰳx61ⲻ7A<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx61ⲻ7A<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _6E : _Ⰳx61ⲻ7A<TMode>.Realized
            {
                public override _Ⰳx61ⲻ7A<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx61ⲻ7A<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _6F : _Ⰳx61ⲻ7A<TMode>.Realized
            {
                public override _Ⰳx61ⲻ7A<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx61ⲻ7A<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _70 : _Ⰳx61ⲻ7A<TMode>.Realized
            {
                public override _Ⰳx61ⲻ7A<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx61ⲻ7A<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _71 : _Ⰳx61ⲻ7A<TMode>.Realized
            {
                public override _Ⰳx61ⲻ7A<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx61ⲻ7A<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _72 : _Ⰳx61ⲻ7A<TMode>.Realized
            {
                public override _Ⰳx61ⲻ7A<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx61ⲻ7A<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _73 : _Ⰳx61ⲻ7A<TMode>.Realized
            {
                public override _Ⰳx61ⲻ7A<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx61ⲻ7A<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _74 : _Ⰳx61ⲻ7A<TMode>.Realized
            {
                public override _Ⰳx61ⲻ7A<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx61ⲻ7A<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _75 : _Ⰳx61ⲻ7A<TMode>.Realized
            {
                public override _Ⰳx61ⲻ7A<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx61ⲻ7A<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _76 : _Ⰳx61ⲻ7A<TMode>.Realized
            {
                public override _Ⰳx61ⲻ7A<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx61ⲻ7A<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _77 : _Ⰳx61ⲻ7A<TMode>.Realized
            {
                public override _Ⰳx61ⲻ7A<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx61ⲻ7A<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _78 : _Ⰳx61ⲻ7A<TMode>.Realized
            {
                public override _Ⰳx61ⲻ7A<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx61ⲻ7A<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _79 : _Ⰳx61ⲻ7A<TMode>.Realized
            {
                public override _Ⰳx61ⲻ7A<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx61ⲻ7A<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _7A : _Ⰳx61ⲻ7A<TMode>.Realized
            {
                public override _Ⰳx61ⲻ7A<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx61ⲻ7A<ParseMode.Realized>> Realize()
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
                public TResult Visit(_Ⰳx61ⲻ7A<ParseMode.Realized> node, TContext context)
                {
                    //// TODO is there a way to avoid this cast?
return (node as _Ⰳx61ⲻ7A<TMode>.Realized)!.Dispatch(this, context);
                }
                
                public TResult Visit(_Ⰳx61ⲻ7A<TMode>.Realized node, TContext context)
                {
                    return node.Dispatch(this, context);
                }
            }
        }
    }
    
}
