namespace __GeneratedPartialV1.Deferred.CstNodes.Inners
{
    using System;
    using CombinatorParsingV3;
    
    public abstract class _Ⰳx21ⲻ7E<TMode> : IAstNode<char, _Ⰳx21ⲻ7E<ParseMode.Realized>>, IFromRealizedable<_Ⰳx21ⲻ7E<ParseMode.Deferred>> where TMode : ParseMode
    {
        private _Ⰳx21ⲻ7E()
        {
        }
        
        internal static _Ⰳx21ⲻ7E<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            return _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred.Create(previousNodeRealizationResult);
        }
        
        public abstract _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert();
        public abstract IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize();
        
        public sealed class Deferred : _Ⰳx21ⲻ7E<ParseMode.Deferred>
        {
            private Deferred(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                this.previousNodeRealizationResult = previousNodeRealizationResult;
                
                this.realizationResult = Future.Create(() => this.RealizeImpl());
            }
            internal Deferred(IFuture<IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>>> realizationResult)
            {
                this.realizationResult = realizationResult;
            }
            
            private IFuture<IRealizationResult<char>> previousNodeRealizationResult { get; }
            private IFuture<IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>>> realizationResult { get; }
            
            internal static _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                return new _Ⰳx21ⲻ7E<ParseMode.Deferred>.Deferred(previousNodeRealizationResult);
            }
            
            public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
            {
                return this;
            }
            
            public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
            {
                return this.realizationResult.Value;
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> RealizeImpl()
            {
                throw new Exception("TODO");
            }
        }
        
        public abstract class Realized : _Ⰳx21ⲻ7E<ParseMode.Realized>
        {
            private Realized()
            {
            }
            
            protected abstract TResult Dispatch<TResult, TContext>(_Ⰳx21ⲻ7E<TMode>.Realized.Visitor<TResult, TContext> visitor, TContext context);
            
            public sealed class _21 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _22 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _23 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _24 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _25 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _26 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _27 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _28 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _29 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _2A : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _2B : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _2C : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _2D : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _2E : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _2F : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _30 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _31 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _32 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _33 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _34 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _35 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _36 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _37 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _38 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _39 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _3A : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _3B : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _3C : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _3D : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _3E : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _3F : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _40 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _41 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _42 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _43 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _44 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _45 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _46 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _47 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _48 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _49 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _4A : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _4B : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _4C : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _4D : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _4E : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _4F : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _50 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _51 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _52 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _53 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _54 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _55 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _56 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _57 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _58 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _59 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _5A : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _5B : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _5C : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _5D : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _5E : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _5F : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _60 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _61 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _62 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _63 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _64 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _65 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _66 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _67 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _68 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _69 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _6A : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _6B : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _6C : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _6D : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _6E : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _6F : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _70 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _71 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _72 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _73 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _74 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _75 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _76 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _77 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _78 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _79 : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _7A : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _7B : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _7C : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _7D : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _7E : _Ⰳx21ⲻ7E<TMode>.Realized
            {
                public override _Ⰳx21ⲻ7E<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx21ⲻ7E<ParseMode.Realized>> Realize()
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
                public TResult Visit(_Ⰳx21ⲻ7E<ParseMode.Realized> node, TContext context)
                {
                    //// TODO is there a way to avoid this cast?
return (node as _Ⰳx21ⲻ7E<TMode>.Realized)!.Dispatch(this, context);
                }
                
                public TResult Visit(_Ⰳx21ⲻ7E<TMode>.Realized node, TContext context)
                {
                    return node.Dispatch(this, context);
                }
            }
        }
    }
    
}
