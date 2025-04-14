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
            
            public abstract class Visitor<TResult, TContext>
            {
                public TResult Visit(_Ⰳx00ⲻ1F<TMode>.Realized node, TContext context)
                {
                    return node.Dispatch(this, context);
                }
                
                public TResult Visit(_Ⰳx00ⲻ1F<ParseMode.Realized> node, TContext context)
                {
                    //// TODO is there a way to avoid this cast?
return (node as _Ⰳx00ⲻ1F<TMode>.Realized)!.Dispatch(this, context);
                }
                
                protected internal abstract TResult Accept(_Ⰳx00ⲻ1F<TMode>.Realized._00 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻ1F<TMode>.Realized._01 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻ1F<TMode>.Realized._02 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻ1F<TMode>.Realized._03 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻ1F<TMode>.Realized._04 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻ1F<TMode>.Realized._05 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻ1F<TMode>.Realized._06 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻ1F<TMode>.Realized._07 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻ1F<TMode>.Realized._08 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻ1F<TMode>.Realized._09 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻ1F<TMode>.Realized._0A node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻ1F<TMode>.Realized._0B node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻ1F<TMode>.Realized._0C node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻ1F<TMode>.Realized._0D node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻ1F<TMode>.Realized._0E node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻ1F<TMode>.Realized._0F node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻ1F<TMode>.Realized._10 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻ1F<TMode>.Realized._11 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻ1F<TMode>.Realized._12 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻ1F<TMode>.Realized._13 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻ1F<TMode>.Realized._14 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻ1F<TMode>.Realized._15 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻ1F<TMode>.Realized._16 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻ1F<TMode>.Realized._17 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻ1F<TMode>.Realized._18 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻ1F<TMode>.Realized._19 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻ1F<TMode>.Realized._1A node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻ1F<TMode>.Realized._1B node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻ1F<TMode>.Realized._1C node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻ1F<TMode>.Realized._1D node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻ1F<TMode>.Realized._1E node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻ1F<TMode>.Realized._1F node, TContext context);
            }
            
            public sealed class _00 : _Ⰳx00ⲻ1F<TMode>.Realized
            {
                private _00(__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode> _0_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode> _0_2, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._0_1 = _0_1;
                    this._0_2 = _0_2;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻ1F<TMode>.Realized._00>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻ1F<TMode>.Realized._00> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode> _0_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode> _0_2 { get; }
                
                public override _Ⰳx00ⲻ1F<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻ1F<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻ1F<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _01 : _Ⰳx00ⲻ1F<TMode>.Realized
            {
                private _01(__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode> _0_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode> _1_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._0_1 = _0_1;
                    this._1_1 = _1_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻ1F<TMode>.Realized._01>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻ1F<TMode>.Realized._01> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode> _0_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode> _1_1 { get; }
                
                public override _Ⰳx00ⲻ1F<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻ1F<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻ1F<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _02 : _Ⰳx00ⲻ1F<TMode>.Realized
            {
                private _02(__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode> _0_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode> _2_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._0_1 = _0_1;
                    this._2_1 = _2_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻ1F<TMode>.Realized._02>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻ1F<TMode>.Realized._02> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode> _0_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode> _2_1 { get; }
                
                public override _Ⰳx00ⲻ1F<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻ1F<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻ1F<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _03 : _Ⰳx00ⲻ1F<TMode>.Realized
            {
                private _03(__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode> _0_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode> _3_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._0_1 = _0_1;
                    this._3_1 = _3_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻ1F<TMode>.Realized._03>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻ1F<TMode>.Realized._03> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode> _0_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode> _3_1 { get; }
                
                public override _Ⰳx00ⲻ1F<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻ1F<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻ1F<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _04 : _Ⰳx00ⲻ1F<TMode>.Realized
            {
                private _04(__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode> _0_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode> _4_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._0_1 = _0_1;
                    this._4_1 = _4_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻ1F<TMode>.Realized._04>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻ1F<TMode>.Realized._04> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode> _0_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode> _4_1 { get; }
                
                public override _Ⰳx00ⲻ1F<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻ1F<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻ1F<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _05 : _Ⰳx00ⲻ1F<TMode>.Realized
            {
                private _05(__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode> _0_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode> _5_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._0_1 = _0_1;
                    this._5_1 = _5_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻ1F<TMode>.Realized._05>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻ1F<TMode>.Realized._05> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode> _0_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode> _5_1 { get; }
                
                public override _Ⰳx00ⲻ1F<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻ1F<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻ1F<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _06 : _Ⰳx00ⲻ1F<TMode>.Realized
            {
                private _06(__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode> _0_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode> _6_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._0_1 = _0_1;
                    this._6_1 = _6_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻ1F<TMode>.Realized._06>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻ1F<TMode>.Realized._06> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode> _0_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode> _6_1 { get; }
                
                public override _Ⰳx00ⲻ1F<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻ1F<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻ1F<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _07 : _Ⰳx00ⲻ1F<TMode>.Realized
            {
                private _07(__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode> _0_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode> _7_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._0_1 = _0_1;
                    this._7_1 = _7_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻ1F<TMode>.Realized._07>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻ1F<TMode>.Realized._07> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode> _0_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode> _7_1 { get; }
                
                public override _Ⰳx00ⲻ1F<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻ1F<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻ1F<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _08 : _Ⰳx00ⲻ1F<TMode>.Realized
            {
                private _08(__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode> _0_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode> _8_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._0_1 = _0_1;
                    this._8_1 = _8_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻ1F<TMode>.Realized._08>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻ1F<TMode>.Realized._08> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode> _0_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode> _8_1 { get; }
                
                public override _Ⰳx00ⲻ1F<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻ1F<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻ1F<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _09 : _Ⰳx00ⲻ1F<TMode>.Realized
            {
                private _09(__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode> _0_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode> _9_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._0_1 = _0_1;
                    this._9_1 = _9_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻ1F<TMode>.Realized._09>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻ1F<TMode>.Realized._09> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode> _0_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode> _9_1 { get; }
                
                public override _Ⰳx00ⲻ1F<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻ1F<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻ1F<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _0A : _Ⰳx00ⲻ1F<TMode>.Realized
            {
                private _0A(__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode> _0_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode> _A_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._0_1 = _0_1;
                    this._A_1 = _A_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻ1F<TMode>.Realized._0A>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻ1F<TMode>.Realized._0A> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode> _0_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode> _A_1 { get; }
                
                public override _Ⰳx00ⲻ1F<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻ1F<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻ1F<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _0B : _Ⰳx00ⲻ1F<TMode>.Realized
            {
                private _0B(__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode> _0_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode> _B_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._0_1 = _0_1;
                    this._B_1 = _B_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻ1F<TMode>.Realized._0B>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻ1F<TMode>.Realized._0B> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode> _0_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode> _B_1 { get; }
                
                public override _Ⰳx00ⲻ1F<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻ1F<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻ1F<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _0C : _Ⰳx00ⲻ1F<TMode>.Realized
            {
                private _0C(__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode> _0_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode> _C_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._0_1 = _0_1;
                    this._C_1 = _C_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻ1F<TMode>.Realized._0C>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻ1F<TMode>.Realized._0C> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode> _0_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode> _C_1 { get; }
                
                public override _Ⰳx00ⲻ1F<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻ1F<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻ1F<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _0D : _Ⰳx00ⲻ1F<TMode>.Realized
            {
                private _0D(__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode> _0_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode> _D_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._0_1 = _0_1;
                    this._D_1 = _D_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻ1F<TMode>.Realized._0D>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻ1F<TMode>.Realized._0D> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode> _0_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode> _D_1 { get; }
                
                public override _Ⰳx00ⲻ1F<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻ1F<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻ1F<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _0E : _Ⰳx00ⲻ1F<TMode>.Realized
            {
                private _0E(__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode> _0_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode> _E_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._0_1 = _0_1;
                    this._E_1 = _E_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻ1F<TMode>.Realized._0E>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻ1F<TMode>.Realized._0E> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode> _0_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode> _E_1 { get; }
                
                public override _Ⰳx00ⲻ1F<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻ1F<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻ1F<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _0F : _Ⰳx00ⲻ1F<TMode>.Realized
            {
                private _0F(__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode> _0_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode> _F_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._0_1 = _0_1;
                    this._F_1 = _F_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻ1F<TMode>.Realized._0F>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻ1F<TMode>.Realized._0F> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode> _0_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode> _F_1 { get; }
                
                public override _Ⰳx00ⲻ1F<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻ1F<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻ1F<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _10 : _Ⰳx00ⲻ1F<TMode>.Realized
            {
                private _10(__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode> _1_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode> _0_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._1_1 = _1_1;
                    this._0_1 = _0_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻ1F<TMode>.Realized._10>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻ1F<TMode>.Realized._10> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode> _1_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode> _0_1 { get; }
                
                public override _Ⰳx00ⲻ1F<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻ1F<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻ1F<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _11 : _Ⰳx00ⲻ1F<TMode>.Realized
            {
                private _11(__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode> _1_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode> _1_2, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._1_1 = _1_1;
                    this._1_2 = _1_2;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻ1F<TMode>.Realized._11>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻ1F<TMode>.Realized._11> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode> _1_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode> _1_2 { get; }
                
                public override _Ⰳx00ⲻ1F<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻ1F<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻ1F<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _12 : _Ⰳx00ⲻ1F<TMode>.Realized
            {
                private _12(__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode> _1_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode> _2_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._1_1 = _1_1;
                    this._2_1 = _2_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻ1F<TMode>.Realized._12>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻ1F<TMode>.Realized._12> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode> _1_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode> _2_1 { get; }
                
                public override _Ⰳx00ⲻ1F<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻ1F<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻ1F<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _13 : _Ⰳx00ⲻ1F<TMode>.Realized
            {
                private _13(__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode> _1_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode> _3_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._1_1 = _1_1;
                    this._3_1 = _3_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻ1F<TMode>.Realized._13>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻ1F<TMode>.Realized._13> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode> _1_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode> _3_1 { get; }
                
                public override _Ⰳx00ⲻ1F<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻ1F<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻ1F<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _14 : _Ⰳx00ⲻ1F<TMode>.Realized
            {
                private _14(__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode> _1_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode> _4_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._1_1 = _1_1;
                    this._4_1 = _4_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻ1F<TMode>.Realized._14>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻ1F<TMode>.Realized._14> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode> _1_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode> _4_1 { get; }
                
                public override _Ⰳx00ⲻ1F<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻ1F<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻ1F<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _15 : _Ⰳx00ⲻ1F<TMode>.Realized
            {
                private _15(__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode> _1_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode> _5_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._1_1 = _1_1;
                    this._5_1 = _5_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻ1F<TMode>.Realized._15>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻ1F<TMode>.Realized._15> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode> _1_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode> _5_1 { get; }
                
                public override _Ⰳx00ⲻ1F<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻ1F<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻ1F<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _16 : _Ⰳx00ⲻ1F<TMode>.Realized
            {
                private _16(__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode> _1_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode> _6_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._1_1 = _1_1;
                    this._6_1 = _6_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻ1F<TMode>.Realized._16>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻ1F<TMode>.Realized._16> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode> _1_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode> _6_1 { get; }
                
                public override _Ⰳx00ⲻ1F<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻ1F<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻ1F<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _17 : _Ⰳx00ⲻ1F<TMode>.Realized
            {
                private _17(__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode> _1_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode> _7_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._1_1 = _1_1;
                    this._7_1 = _7_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻ1F<TMode>.Realized._17>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻ1F<TMode>.Realized._17> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode> _1_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode> _7_1 { get; }
                
                public override _Ⰳx00ⲻ1F<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻ1F<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻ1F<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _18 : _Ⰳx00ⲻ1F<TMode>.Realized
            {
                private _18(__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode> _1_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode> _8_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._1_1 = _1_1;
                    this._8_1 = _8_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻ1F<TMode>.Realized._18>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻ1F<TMode>.Realized._18> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode> _1_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode> _8_1 { get; }
                
                public override _Ⰳx00ⲻ1F<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻ1F<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻ1F<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _19 : _Ⰳx00ⲻ1F<TMode>.Realized
            {
                private _19(__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode> _1_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode> _9_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._1_1 = _1_1;
                    this._9_1 = _9_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻ1F<TMode>.Realized._19>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻ1F<TMode>.Realized._19> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode> _1_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode> _9_1 { get; }
                
                public override _Ⰳx00ⲻ1F<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻ1F<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻ1F<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _1A : _Ⰳx00ⲻ1F<TMode>.Realized
            {
                private _1A(__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode> _1_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode> _A_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._1_1 = _1_1;
                    this._A_1 = _A_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻ1F<TMode>.Realized._1A>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻ1F<TMode>.Realized._1A> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode> _1_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode> _A_1 { get; }
                
                public override _Ⰳx00ⲻ1F<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻ1F<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻ1F<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _1B : _Ⰳx00ⲻ1F<TMode>.Realized
            {
                private _1B(__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode> _1_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode> _B_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._1_1 = _1_1;
                    this._B_1 = _B_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻ1F<TMode>.Realized._1B>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻ1F<TMode>.Realized._1B> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode> _1_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode> _B_1 { get; }
                
                public override _Ⰳx00ⲻ1F<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻ1F<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻ1F<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _1C : _Ⰳx00ⲻ1F<TMode>.Realized
            {
                private _1C(__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode> _1_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode> _C_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._1_1 = _1_1;
                    this._C_1 = _C_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻ1F<TMode>.Realized._1C>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻ1F<TMode>.Realized._1C> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode> _1_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode> _C_1 { get; }
                
                public override _Ⰳx00ⲻ1F<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻ1F<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻ1F<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _1D : _Ⰳx00ⲻ1F<TMode>.Realized
            {
                private _1D(__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode> _1_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode> _D_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._1_1 = _1_1;
                    this._D_1 = _D_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻ1F<TMode>.Realized._1D>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻ1F<TMode>.Realized._1D> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode> _1_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode> _D_1 { get; }
                
                public override _Ⰳx00ⲻ1F<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻ1F<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻ1F<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _1E : _Ⰳx00ⲻ1F<TMode>.Realized
            {
                private _1E(__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode> _1_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode> _E_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._1_1 = _1_1;
                    this._E_1 = _E_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻ1F<TMode>.Realized._1E>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻ1F<TMode>.Realized._1E> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode> _1_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode> _E_1 { get; }
                
                public override _Ⰳx00ⲻ1F<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻ1F<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻ1F<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _1F : _Ⰳx00ⲻ1F<TMode>.Realized
            {
                private _1F(__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode> _1_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode> _F_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._1_1 = _1_1;
                    this._F_1 = _F_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻ1F<TMode>.Realized._1F>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻ1F<TMode>.Realized._1F> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode> _1_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode> _F_1 { get; }
                
                public override _Ⰳx00ⲻ1F<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻ1F<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻ1F<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
        }
    }
    
}
