namespace __GeneratedPartialV1.Deferred.CstNodes.Inners
{
    using System;
    using CombinatorParsingV3;
    
    public abstract class _Ⰳx00ⲻ1F<TMode> : IAstNode<char, _Ⰳx00ⲻ1F<ParseMode.Realized>>, IFromRealizedable<_Ⰳx00ⲻ1F<ParseMode.Deferred>> where TMode : ParseMode
    {
        private _Ⰳx00ⲻ1F()
        {
        }
        
        internal static _Ⰳx00ⲻ1F<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            return _Ⰳx00ⲻ1F<ParseMode.Deferred>.Deferred.Create(previousNodeRealizationResult);
        }
        
        public abstract _Ⰳx00ⲻ1F<ParseMode.Deferred> Convert();
        public abstract IRealizationResult<char, _Ⰳx00ⲻ1F<ParseMode.Realized>> Realize();
        
        public sealed class Deferred : _Ⰳx00ⲻ1F<ParseMode.Deferred>
        {
            private Deferred(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                this.previousNodeRealizationResult = previousNodeRealizationResult;
                
                this.realizationResult = Future.Create(() => this.RealizeImpl());
            }
            internal Deferred(IFuture<IRealizationResult<char, _Ⰳx00ⲻ1F<ParseMode.Realized>>> realizationResult)
            {
                this.realizationResult = realizationResult;
            }
            
            private IFuture<IRealizationResult<char>> previousNodeRealizationResult { get; }
            private IFuture<IRealizationResult<char, _Ⰳx00ⲻ1F<ParseMode.Realized>>> realizationResult { get; }
            
            internal static _Ⰳx00ⲻ1F<ParseMode.Deferred>.Deferred Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                return new _Ⰳx00ⲻ1F<ParseMode.Deferred>.Deferred(previousNodeRealizationResult);
            }
            
            public override _Ⰳx00ⲻ1F<ParseMode.Deferred> Convert()
            {
                return this;
            }
            
            public override IRealizationResult<char, _Ⰳx00ⲻ1F<ParseMode.Realized>> Realize()
            {
                return this.realizationResult.Value;
            }
            
            private IRealizationResult<char, _Ⰳx00ⲻ1F<ParseMode.Realized>> RealizeImpl()
            {
                throw new Exception("TODO");
            }
        }
        
        public abstract class Realized : _Ⰳx00ⲻ1F<ParseMode.Realized>
        {
            private Realized()
            {
            }
            
            protected abstract TResult Dispatch<TResult, TContext>(_Ⰳx00ⲻ1F<TMode>.Realized.Visitor<TResult, TContext> visitor, TContext context);
            
            public sealed class _00 : _Ⰳx00ⲻ1F<TMode>.Realized
            {
                public override _Ⰳx00ⲻ1F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻ1F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _01 : _Ⰳx00ⲻ1F<TMode>.Realized
            {
                public override _Ⰳx00ⲻ1F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻ1F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _02 : _Ⰳx00ⲻ1F<TMode>.Realized
            {
                public override _Ⰳx00ⲻ1F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻ1F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _03 : _Ⰳx00ⲻ1F<TMode>.Realized
            {
                public override _Ⰳx00ⲻ1F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻ1F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _04 : _Ⰳx00ⲻ1F<TMode>.Realized
            {
                public override _Ⰳx00ⲻ1F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻ1F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _05 : _Ⰳx00ⲻ1F<TMode>.Realized
            {
                public override _Ⰳx00ⲻ1F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻ1F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _06 : _Ⰳx00ⲻ1F<TMode>.Realized
            {
                public override _Ⰳx00ⲻ1F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻ1F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _07 : _Ⰳx00ⲻ1F<TMode>.Realized
            {
                public override _Ⰳx00ⲻ1F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻ1F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _08 : _Ⰳx00ⲻ1F<TMode>.Realized
            {
                public override _Ⰳx00ⲻ1F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻ1F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _09 : _Ⰳx00ⲻ1F<TMode>.Realized
            {
                public override _Ⰳx00ⲻ1F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻ1F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _0A : _Ⰳx00ⲻ1F<TMode>.Realized
            {
                public override _Ⰳx00ⲻ1F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻ1F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _0B : _Ⰳx00ⲻ1F<TMode>.Realized
            {
                public override _Ⰳx00ⲻ1F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻ1F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _0C : _Ⰳx00ⲻ1F<TMode>.Realized
            {
                public override _Ⰳx00ⲻ1F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻ1F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _0D : _Ⰳx00ⲻ1F<TMode>.Realized
            {
                public override _Ⰳx00ⲻ1F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻ1F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _0E : _Ⰳx00ⲻ1F<TMode>.Realized
            {
                public override _Ⰳx00ⲻ1F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻ1F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _0F : _Ⰳx00ⲻ1F<TMode>.Realized
            {
                public override _Ⰳx00ⲻ1F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻ1F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _10 : _Ⰳx00ⲻ1F<TMode>.Realized
            {
                public override _Ⰳx00ⲻ1F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻ1F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _11 : _Ⰳx00ⲻ1F<TMode>.Realized
            {
                public override _Ⰳx00ⲻ1F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻ1F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _12 : _Ⰳx00ⲻ1F<TMode>.Realized
            {
                public override _Ⰳx00ⲻ1F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻ1F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _13 : _Ⰳx00ⲻ1F<TMode>.Realized
            {
                public override _Ⰳx00ⲻ1F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻ1F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _14 : _Ⰳx00ⲻ1F<TMode>.Realized
            {
                public override _Ⰳx00ⲻ1F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻ1F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _15 : _Ⰳx00ⲻ1F<TMode>.Realized
            {
                public override _Ⰳx00ⲻ1F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻ1F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _16 : _Ⰳx00ⲻ1F<TMode>.Realized
            {
                public override _Ⰳx00ⲻ1F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻ1F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _17 : _Ⰳx00ⲻ1F<TMode>.Realized
            {
                public override _Ⰳx00ⲻ1F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻ1F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _18 : _Ⰳx00ⲻ1F<TMode>.Realized
            {
                public override _Ⰳx00ⲻ1F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻ1F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _19 : _Ⰳx00ⲻ1F<TMode>.Realized
            {
                public override _Ⰳx00ⲻ1F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻ1F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _1A : _Ⰳx00ⲻ1F<TMode>.Realized
            {
                public override _Ⰳx00ⲻ1F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻ1F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _1B : _Ⰳx00ⲻ1F<TMode>.Realized
            {
                public override _Ⰳx00ⲻ1F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻ1F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _1C : _Ⰳx00ⲻ1F<TMode>.Realized
            {
                public override _Ⰳx00ⲻ1F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻ1F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _1D : _Ⰳx00ⲻ1F<TMode>.Realized
            {
                public override _Ⰳx00ⲻ1F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻ1F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _1E : _Ⰳx00ⲻ1F<TMode>.Realized
            {
                public override _Ⰳx00ⲻ1F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻ1F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _1F : _Ⰳx00ⲻ1F<TMode>.Realized
            {
                public override _Ⰳx00ⲻ1F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻ1F<ParseMode.Realized>> Realize()
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
                public TResult Visit(_Ⰳx00ⲻ1F<ParseMode.Realized> node, TContext context)
                {
                    //// TODO is there a way to avoid this cast?
return (node as _Ⰳx00ⲻ1F<TMode>.Realized)!.Dispatch(this, context);
                }
                
                public TResult Visit(_Ⰳx00ⲻ1F<TMode>.Realized node, TContext context)
                {
                    return node.Dispatch(this, context);
                }
            }
        }
    }
    
}
