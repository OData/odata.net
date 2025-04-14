namespace __GeneratedPartialV1.Deferred.CstNodes.Inners
{
    using System;
    using CombinatorParsingV3;
    
    public abstract class _Ⰳx01ⲻ7F<TMode> : IAstNode<char, _Ⰳx01ⲻ7F<ParseMode.Realized>>, IFromRealizedable<_Ⰳx01ⲻ7F<ParseMode.Deferred>> where TMode : ParseMode
    {
        private _Ⰳx01ⲻ7F()
        {
        }
        
        internal static _Ⰳx01ⲻ7F<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            return _Ⰳx01ⲻ7F<ParseMode.Deferred>.Deferred.Create(previousNodeRealizationResult);
        }
        
        public abstract _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert();
        public abstract IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize();
        
        public sealed class Deferred : _Ⰳx01ⲻ7F<ParseMode.Deferred>
        {
            private Deferred(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                this.previousNodeRealizationResult = previousNodeRealizationResult;
                
                this.realizationResult = Future.Create(() => this.RealizeImpl());
            }
            internal Deferred(IFuture<IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>>> realizationResult)
            {
                this.realizationResult = realizationResult;
            }
            
            private IFuture<IRealizationResult<char>> previousNodeRealizationResult { get; }
            private IFuture<IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>>> realizationResult { get; }
            
            internal static _Ⰳx01ⲻ7F<ParseMode.Deferred>.Deferred Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                return new _Ⰳx01ⲻ7F<ParseMode.Deferred>.Deferred(previousNodeRealizationResult);
            }
            
            public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
            {
                return this;
            }
            
            public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
            {
                return this.realizationResult.Value;
            }
            
            private IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> RealizeImpl()
            {
                throw new Exception("TODO");
            }
        }
        
        public abstract class Realized : _Ⰳx01ⲻ7F<ParseMode.Realized>
        {
            private Realized()
            {
            }
            
            protected abstract TResult Dispatch<TResult, TContext>(_Ⰳx01ⲻ7F<TMode>.Realized.Visitor<TResult, TContext> visitor, TContext context);
            
            public abstract class Visitor<TResult, TContext>
            {
                public TResult Visit(_Ⰳx01ⲻ7F<TMode>.Realized node, TContext context)
                {
                    return node.Dispatch(this, context);
                }
                
                public TResult Visit(_Ⰳx01ⲻ7F<ParseMode.Realized> node, TContext context)
                {
                    //// TODO is there a way to avoid this cast?
return (node as _Ⰳx01ⲻ7F<TMode>.Realized)!.Dispatch(this, context);
                }
                
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._01 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._02 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._03 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._04 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._05 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._06 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._07 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._08 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._09 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._0A node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._0B node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._0C node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._0D node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._0E node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._0F node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._10 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._11 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._12 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._13 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._14 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._15 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._16 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._17 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._18 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._19 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._1A node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._1B node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._1C node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._1D node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._1E node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._1F node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._20 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._21 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._22 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._23 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._24 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._25 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._26 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._27 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._28 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._29 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._2A node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._2B node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._2C node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._2D node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._2E node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._2F node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._30 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._31 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._32 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._33 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._34 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._35 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._36 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._37 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._38 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._39 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._3A node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._3B node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._3C node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._3D node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._3E node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._3F node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._40 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._41 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._42 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._43 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._44 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._45 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._46 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._47 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._48 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._49 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._4A node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._4B node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._4C node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._4D node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._4E node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._4F node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._50 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._51 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._52 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._53 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._54 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._55 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._56 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._57 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._58 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._59 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._5A node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._5B node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._5C node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._5D node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._5E node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._5F node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._60 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._61 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._62 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._63 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._64 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._65 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._66 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._67 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._68 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._69 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._6A node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._6B node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._6C node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._6D node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._6E node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._6F node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._70 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._71 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._72 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._73 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._74 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._75 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._76 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._77 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._78 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._79 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._7A node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._7B node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._7C node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._7D node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._7E node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx01ⲻ7F<TMode>.Realized._7F node, TContext context);
            }
            
            public sealed class _01 : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _01(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode>> _0_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode>> _1_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _02 : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _02(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode>> _0_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode>> _2_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _03 : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _03(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode>> _0_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode>> _3_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _04 : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _04(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode>> _0_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode>> _4_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _05 : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _05(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode>> _0_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode>> _5_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _06 : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _06(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode>> _0_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode>> _6_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _07 : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _07(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode>> _0_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode>> _7_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _08 : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _08(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode>> _0_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode>> _8_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _09 : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _09(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode>> _0_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode>> _9_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _0A : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _0A(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode>> _0_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode>> _A_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _0B : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _0B(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode>> _0_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode>> _B_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _0C : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _0C(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode>> _0_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode>> _C_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _0D : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _0D(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode>> _0_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode>> _D_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _0E : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _0E(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode>> _0_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode>> _E_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _0F : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _0F(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode>> _0_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode>> _F_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _10 : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _10(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode>> _1_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode>> _0_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _11 : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _11(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode>> _1_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode>> _1_2, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _12 : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _12(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode>> _1_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode>> _2_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _13 : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _13(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode>> _1_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode>> _3_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _14 : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _14(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode>> _1_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode>> _4_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _15 : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _15(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode>> _1_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode>> _5_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _16 : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _16(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode>> _1_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode>> _6_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _17 : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _17(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode>> _1_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode>> _7_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _18 : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _18(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode>> _1_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode>> _8_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _19 : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _19(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode>> _1_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode>> _9_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _1A : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _1A(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode>> _1_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode>> _A_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _1B : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _1B(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode>> _1_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode>> _B_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _1C : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _1C(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode>> _1_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode>> _C_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _1D : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _1D(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode>> _1_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode>> _D_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _1E : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _1E(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode>> _1_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode>> _E_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _1F : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _1F(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode>> _1_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode>> _F_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _20 : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _20(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode>> _2_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode>> _0_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _21 : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _21(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode>> _2_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode>> _1_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _22 : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _22(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode>> _2_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode>> _2_2, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _23 : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _23(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode>> _2_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode>> _3_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _24 : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _24(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode>> _2_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode>> _4_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _25 : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _25(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode>> _2_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode>> _5_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _26 : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _26(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode>> _2_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode>> _6_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _27 : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _27(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode>> _2_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode>> _7_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _28 : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _28(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode>> _2_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode>> _8_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _29 : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _29(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode>> _2_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode>> _9_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _2A : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _2A(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode>> _2_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode>> _A_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _2B : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _2B(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode>> _2_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode>> _B_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _2C : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _2C(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode>> _2_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode>> _C_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _2D : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _2D(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode>> _2_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode>> _D_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _2E : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _2E(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode>> _2_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode>> _E_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _2F : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _2F(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode>> _2_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode>> _F_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _30 : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _30(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode>> _3_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode>> _0_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _31 : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _31(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode>> _3_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode>> _1_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _32 : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _32(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode>> _3_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode>> _2_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _33 : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _33(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode>> _3_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode>> _3_2, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _34 : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _34(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode>> _3_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode>> _4_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _35 : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _35(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode>> _3_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode>> _5_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _36 : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _36(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode>> _3_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode>> _6_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _37 : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _37(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode>> _3_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode>> _7_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _38 : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _38(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode>> _3_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode>> _8_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _39 : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _39(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode>> _3_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode>> _9_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _3A : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _3A(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode>> _3_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode>> _A_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _3B : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _3B(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode>> _3_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode>> _B_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _3C : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _3C(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode>> _3_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode>> _C_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _3D : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _3D(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode>> _3_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode>> _D_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _3E : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _3E(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode>> _3_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode>> _E_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _3F : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _3F(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode>> _3_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode>> _F_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _40 : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _40(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode>> _4_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode>> _0_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _41 : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _41(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode>> _4_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode>> _1_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _42 : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _42(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode>> _4_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode>> _2_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _43 : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _43(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode>> _4_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode>> _3_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _44 : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _44(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode>> _4_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode>> _4_2, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _45 : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _45(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode>> _4_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode>> _5_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _46 : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _46(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode>> _4_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode>> _6_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _47 : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _47(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode>> _4_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode>> _7_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _48 : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _48(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode>> _4_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode>> _8_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _49 : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _49(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode>> _4_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode>> _9_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _4A : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _4A(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode>> _4_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode>> _A_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _4B : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _4B(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode>> _4_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode>> _B_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _4C : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _4C(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode>> _4_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode>> _C_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _4D : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _4D(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode>> _4_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode>> _D_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _4E : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _4E(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode>> _4_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode>> _E_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _4F : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _4F(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode>> _4_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode>> _F_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _50 : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _50(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode>> _5_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode>> _0_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _51 : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _51(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode>> _5_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode>> _1_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _52 : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _52(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode>> _5_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode>> _2_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _53 : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _53(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode>> _5_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode>> _3_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _54 : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _54(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode>> _5_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode>> _4_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _55 : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _55(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode>> _5_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode>> _5_2, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _56 : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _56(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode>> _5_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode>> _6_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _57 : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _57(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode>> _5_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode>> _7_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _58 : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _58(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode>> _5_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode>> _8_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _59 : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _59(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode>> _5_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode>> _9_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _5A : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _5A(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode>> _5_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode>> _A_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _5B : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _5B(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode>> _5_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode>> _B_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _5C : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _5C(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode>> _5_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode>> _C_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _5D : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _5D(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode>> _5_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode>> _D_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _5E : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _5E(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode>> _5_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode>> _E_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _5F : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _5F(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode>> _5_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode>> _F_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _60 : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _60(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode>> _6_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode>> _0_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _61 : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _61(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode>> _6_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode>> _1_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _62 : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _62(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode>> _6_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode>> _2_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _63 : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _63(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode>> _6_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode>> _3_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _64 : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _64(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode>> _6_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode>> _4_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _65 : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _65(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode>> _6_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode>> _5_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _66 : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _66(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode>> _6_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode>> _6_2, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _67 : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _67(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode>> _6_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode>> _7_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _68 : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _68(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode>> _6_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode>> _8_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _69 : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _69(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode>> _6_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode>> _9_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _6A : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _6A(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode>> _6_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode>> _A_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _6B : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _6B(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode>> _6_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode>> _B_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _6C : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _6C(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode>> _6_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode>> _C_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _6D : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _6D(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode>> _6_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode>> _D_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _6E : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _6E(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode>> _6_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode>> _E_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _6F : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _6F(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode>> _6_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode>> _F_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _70 : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _70(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode>> _7_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode>> _0_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _71 : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _71(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode>> _7_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode>> _1_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _72 : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _72(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode>> _7_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode>> _2_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _73 : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _73(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode>> _7_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode>> _3_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _74 : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _74(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode>> _7_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode>> _4_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _75 : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _75(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode>> _7_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode>> _5_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _76 : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _76(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode>> _7_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode>> _6_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _77 : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _77(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode>> _7_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode>> _7_2, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _78 : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _78(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode>> _7_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode>> _8_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _79 : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _79(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode>> _7_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode>> _9_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _7A : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _7A(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode>> _7_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode>> _A_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _7B : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _7B(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode>> _7_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode>> _B_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _7C : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _7C(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode>> _7_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode>> _C_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _7D : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _7D(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode>> _7_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode>> _D_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _7E : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _7E(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode>> _7_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode>> _E_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _7F : _Ⰳx01ⲻ7F<TMode>.Realized
            {
                private _7F(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode>> _7_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode>> _F_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                public override _Ⰳx01ⲻ7F<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx01ⲻ7F<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
        }
    }
    
}
