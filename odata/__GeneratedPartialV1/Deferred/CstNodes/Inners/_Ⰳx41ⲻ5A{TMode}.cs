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
            
            public abstract class Visitor<TResult, TContext>
            {
                public TResult Visit(_Ⰳx41ⲻ5A<TMode>.Realized node, TContext context)
                {
                    return node.Dispatch(this, context);
                }
                
                public TResult Visit(_Ⰳx41ⲻ5A<ParseMode.Realized> node, TContext context)
                {
                    //// TODO is there a way to avoid this cast?
return (node as _Ⰳx41ⲻ5A<TMode>.Realized)!.Dispatch(this, context);
                }
                
                protected internal abstract TResult Accept(_Ⰳx41ⲻ5A<TMode>.Realized._41 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx41ⲻ5A<TMode>.Realized._42 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx41ⲻ5A<TMode>.Realized._43 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx41ⲻ5A<TMode>.Realized._44 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx41ⲻ5A<TMode>.Realized._45 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx41ⲻ5A<TMode>.Realized._46 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx41ⲻ5A<TMode>.Realized._47 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx41ⲻ5A<TMode>.Realized._48 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx41ⲻ5A<TMode>.Realized._49 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx41ⲻ5A<TMode>.Realized._4A node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx41ⲻ5A<TMode>.Realized._4B node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx41ⲻ5A<TMode>.Realized._4C node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx41ⲻ5A<TMode>.Realized._4D node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx41ⲻ5A<TMode>.Realized._4E node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx41ⲻ5A<TMode>.Realized._4F node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx41ⲻ5A<TMode>.Realized._50 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx41ⲻ5A<TMode>.Realized._51 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx41ⲻ5A<TMode>.Realized._52 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx41ⲻ5A<TMode>.Realized._53 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx41ⲻ5A<TMode>.Realized._54 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx41ⲻ5A<TMode>.Realized._55 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx41ⲻ5A<TMode>.Realized._56 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx41ⲻ5A<TMode>.Realized._57 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx41ⲻ5A<TMode>.Realized._58 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx41ⲻ5A<TMode>.Realized._59 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx41ⲻ5A<TMode>.Realized._5A node, TContext context);
            }
            
            public sealed class _41 : _Ⰳx41ⲻ5A<TMode>.Realized
            {
                private _41(__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode> _4_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode> _1_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._4_1 = _4_1;
                    this._1_1 = _1_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._41>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._41> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode> _4_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode> _1_1 { get; }
                
                public override _Ⰳx41ⲻ5A<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx41ⲻ5A<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
                
                internal static IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>.Realized._41> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    throw new Exception();
                }
            }
            
            public sealed class _42 : _Ⰳx41ⲻ5A<TMode>.Realized
            {
                private _42(__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode> _4_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode> _2_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._4_1 = _4_1;
                    this._2_1 = _2_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._42>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._42> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode> _4_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode> _2_1 { get; }
                
                public override _Ⰳx41ⲻ5A<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx41ⲻ5A<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
                
                internal static IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>.Realized._42> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    throw new Exception();
                }
            }
            
            public sealed class _43 : _Ⰳx41ⲻ5A<TMode>.Realized
            {
                private _43(__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode> _4_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode> _3_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._4_1 = _4_1;
                    this._3_1 = _3_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._43>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._43> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode> _4_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode> _3_1 { get; }
                
                public override _Ⰳx41ⲻ5A<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx41ⲻ5A<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
                
                internal static IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>.Realized._43> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    throw new Exception();
                }
            }
            
            public sealed class _44 : _Ⰳx41ⲻ5A<TMode>.Realized
            {
                private _44(__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode> _4_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode> _4_2, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._4_1 = _4_1;
                    this._4_2 = _4_2;
                    this.realizationResult = new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._44>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._44> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode> _4_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode> _4_2 { get; }
                
                public override _Ⰳx41ⲻ5A<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx41ⲻ5A<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
                
                internal static IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>.Realized._44> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    throw new Exception();
                }
            }
            
            public sealed class _45 : _Ⰳx41ⲻ5A<TMode>.Realized
            {
                private _45(__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode> _4_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode> _5_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._4_1 = _4_1;
                    this._5_1 = _5_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._45>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._45> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode> _4_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode> _5_1 { get; }
                
                public override _Ⰳx41ⲻ5A<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx41ⲻ5A<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
                
                internal static IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>.Realized._45> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    throw new Exception();
                }
            }
            
            public sealed class _46 : _Ⰳx41ⲻ5A<TMode>.Realized
            {
                private _46(__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode> _4_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode> _6_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._4_1 = _4_1;
                    this._6_1 = _6_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._46>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._46> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode> _4_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode> _6_1 { get; }
                
                public override _Ⰳx41ⲻ5A<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx41ⲻ5A<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
                
                internal static IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>.Realized._46> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    throw new Exception();
                }
            }
            
            public sealed class _47 : _Ⰳx41ⲻ5A<TMode>.Realized
            {
                private _47(__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode> _4_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode> _7_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._4_1 = _4_1;
                    this._7_1 = _7_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._47>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._47> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode> _4_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode> _7_1 { get; }
                
                public override _Ⰳx41ⲻ5A<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx41ⲻ5A<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
                
                internal static IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>.Realized._47> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    throw new Exception();
                }
            }
            
            public sealed class _48 : _Ⰳx41ⲻ5A<TMode>.Realized
            {
                private _48(__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode> _4_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode> _8_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._4_1 = _4_1;
                    this._8_1 = _8_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._48>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._48> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode> _4_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode> _8_1 { get; }
                
                public override _Ⰳx41ⲻ5A<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx41ⲻ5A<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
                
                internal static IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>.Realized._48> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    throw new Exception();
                }
            }
            
            public sealed class _49 : _Ⰳx41ⲻ5A<TMode>.Realized
            {
                private _49(__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode> _4_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode> _9_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._4_1 = _4_1;
                    this._9_1 = _9_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._49>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._49> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode> _4_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode> _9_1 { get; }
                
                public override _Ⰳx41ⲻ5A<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx41ⲻ5A<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
                
                internal static IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>.Realized._49> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    throw new Exception();
                }
            }
            
            public sealed class _4A : _Ⰳx41ⲻ5A<TMode>.Realized
            {
                private _4A(__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode> _4_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode> _A_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._4_1 = _4_1;
                    this._A_1 = _A_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._4A>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._4A> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode> _4_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode> _A_1 { get; }
                
                public override _Ⰳx41ⲻ5A<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx41ⲻ5A<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
                
                internal static IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>.Realized._4A> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    throw new Exception();
                }
            }
            
            public sealed class _4B : _Ⰳx41ⲻ5A<TMode>.Realized
            {
                private _4B(__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode> _4_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode> _B_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._4_1 = _4_1;
                    this._B_1 = _B_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._4B>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._4B> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode> _4_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode> _B_1 { get; }
                
                public override _Ⰳx41ⲻ5A<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx41ⲻ5A<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
                
                internal static IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>.Realized._4B> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    throw new Exception();
                }
            }
            
            public sealed class _4C : _Ⰳx41ⲻ5A<TMode>.Realized
            {
                private _4C(__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode> _4_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode> _C_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._4_1 = _4_1;
                    this._C_1 = _C_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._4C>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._4C> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode> _4_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode> _C_1 { get; }
                
                public override _Ⰳx41ⲻ5A<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx41ⲻ5A<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
                
                internal static IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>.Realized._4C> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    throw new Exception();
                }
            }
            
            public sealed class _4D : _Ⰳx41ⲻ5A<TMode>.Realized
            {
                private _4D(__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode> _4_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode> _D_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._4_1 = _4_1;
                    this._D_1 = _D_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._4D>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._4D> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode> _4_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode> _D_1 { get; }
                
                public override _Ⰳx41ⲻ5A<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx41ⲻ5A<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
                
                internal static IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>.Realized._4D> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    throw new Exception();
                }
            }
            
            public sealed class _4E : _Ⰳx41ⲻ5A<TMode>.Realized
            {
                private _4E(__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode> _4_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode> _E_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._4_1 = _4_1;
                    this._E_1 = _E_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._4E>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._4E> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode> _4_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode> _E_1 { get; }
                
                public override _Ⰳx41ⲻ5A<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx41ⲻ5A<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
                
                internal static IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>.Realized._4E> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    throw new Exception();
                }
            }
            
            public sealed class _4F : _Ⰳx41ⲻ5A<TMode>.Realized
            {
                private _4F(__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode> _4_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode> _F_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._4_1 = _4_1;
                    this._F_1 = _F_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._4F>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._4F> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode> _4_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode> _F_1 { get; }
                
                public override _Ⰳx41ⲻ5A<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx41ⲻ5A<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
                
                internal static IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>.Realized._4F> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    throw new Exception();
                }
            }
            
            public sealed class _50 : _Ⰳx41ⲻ5A<TMode>.Realized
            {
                private _50(__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode> _5_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode> _0_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._5_1 = _5_1;
                    this._0_1 = _0_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._50>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._50> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode> _5_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode> _0_1 { get; }
                
                public override _Ⰳx41ⲻ5A<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx41ⲻ5A<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
                
                internal static IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>.Realized._50> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    throw new Exception();
                }
            }
            
            public sealed class _51 : _Ⰳx41ⲻ5A<TMode>.Realized
            {
                private _51(__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode> _5_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode> _1_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._5_1 = _5_1;
                    this._1_1 = _1_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._51>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._51> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode> _5_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode> _1_1 { get; }
                
                public override _Ⰳx41ⲻ5A<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx41ⲻ5A<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
                
                internal static IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>.Realized._51> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    throw new Exception();
                }
            }
            
            public sealed class _52 : _Ⰳx41ⲻ5A<TMode>.Realized
            {
                private _52(__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode> _5_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode> _2_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._5_1 = _5_1;
                    this._2_1 = _2_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._52>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._52> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode> _5_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode> _2_1 { get; }
                
                public override _Ⰳx41ⲻ5A<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx41ⲻ5A<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
                
                internal static IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>.Realized._52> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    throw new Exception();
                }
            }
            
            public sealed class _53 : _Ⰳx41ⲻ5A<TMode>.Realized
            {
                private _53(__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode> _5_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode> _3_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._5_1 = _5_1;
                    this._3_1 = _3_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._53>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._53> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode> _5_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode> _3_1 { get; }
                
                public override _Ⰳx41ⲻ5A<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx41ⲻ5A<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
                
                internal static IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>.Realized._53> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    throw new Exception();
                }
            }
            
            public sealed class _54 : _Ⰳx41ⲻ5A<TMode>.Realized
            {
                private _54(__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode> _5_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode> _4_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._5_1 = _5_1;
                    this._4_1 = _4_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._54>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._54> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode> _5_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode> _4_1 { get; }
                
                public override _Ⰳx41ⲻ5A<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx41ⲻ5A<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
                
                internal static IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>.Realized._54> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    throw new Exception();
                }
            }
            
            public sealed class _55 : _Ⰳx41ⲻ5A<TMode>.Realized
            {
                private _55(__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode> _5_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode> _5_2, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._5_1 = _5_1;
                    this._5_2 = _5_2;
                    this.realizationResult = new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._55>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._55> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode> _5_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode> _5_2 { get; }
                
                public override _Ⰳx41ⲻ5A<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx41ⲻ5A<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
                
                internal static IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>.Realized._55> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    throw new Exception();
                }
            }
            
            public sealed class _56 : _Ⰳx41ⲻ5A<TMode>.Realized
            {
                private _56(__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode> _5_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode> _6_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._5_1 = _5_1;
                    this._6_1 = _6_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._56>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._56> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode> _5_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode> _6_1 { get; }
                
                public override _Ⰳx41ⲻ5A<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx41ⲻ5A<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
                
                internal static IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>.Realized._56> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    throw new Exception();
                }
            }
            
            public sealed class _57 : _Ⰳx41ⲻ5A<TMode>.Realized
            {
                private _57(__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode> _5_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode> _7_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._5_1 = _5_1;
                    this._7_1 = _7_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._57>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._57> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode> _5_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode> _7_1 { get; }
                
                public override _Ⰳx41ⲻ5A<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx41ⲻ5A<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
                
                internal static IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>.Realized._57> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    throw new Exception();
                }
            }
            
            public sealed class _58 : _Ⰳx41ⲻ5A<TMode>.Realized
            {
                private _58(__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode> _5_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode> _8_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._5_1 = _5_1;
                    this._8_1 = _8_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._58>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._58> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode> _5_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode> _8_1 { get; }
                
                public override _Ⰳx41ⲻ5A<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx41ⲻ5A<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
                
                internal static IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>.Realized._58> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    throw new Exception();
                }
            }
            
            public sealed class _59 : _Ⰳx41ⲻ5A<TMode>.Realized
            {
                private _59(__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode> _5_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode> _9_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._5_1 = _5_1;
                    this._9_1 = _9_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._59>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._59> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode> _5_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode> _9_1 { get; }
                
                public override _Ⰳx41ⲻ5A<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx41ⲻ5A<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
                
                internal static IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>.Realized._59> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    throw new Exception();
                }
            }
            
            public sealed class _5A : _Ⰳx41ⲻ5A<TMode>.Realized
            {
                private _5A(__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode> _5_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode> _A_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._5_1 = _5_1;
                    this._A_1 = _A_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._5A>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx41ⲻ5A<TMode>.Realized._5A> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode> _5_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode> _A_1 { get; }
                
                public override _Ⰳx41ⲻ5A<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx41ⲻ5A<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
                
                internal static IRealizationResult<char, _Ⰳx41ⲻ5A<ParseMode.Realized>.Realized._5A> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    throw new Exception();
                }
            }
        }
    }
    
}
