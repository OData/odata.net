namespace __GeneratedPartialV1.Deferred.CstNodes.Inners
{
    using System;
    using CombinatorParsingV3;
    
    public abstract class _Ⰳx00ⲻFF<TMode> : IAstNode<char, _Ⰳx00ⲻFF<ParseMode.Realized>>, IFromRealizedable<_Ⰳx00ⲻFF<ParseMode.Deferred>> where TMode : ParseMode
    {
        private _Ⰳx00ⲻFF()
        {
        }
        
        internal static _Ⰳx00ⲻFF<ParseMode.Deferred> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            return _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred.Create(previousNodeRealizationResult);
        }
        
        public abstract _Ⰳx00ⲻFF<ParseMode.Deferred> Convert();
        public abstract IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize();
        
        public sealed class Deferred : _Ⰳx00ⲻFF<ParseMode.Deferred>
        {
            private Deferred(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                this.previousNodeRealizationResult = previousNodeRealizationResult;
                
                this.realizationResult = Future.Create(() => this.RealizeImpl());
            }
            internal Deferred(IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult)
            {
                this.realizationResult = realizationResult;
            }
            
            private IFuture<IRealizationResult<char>> previousNodeRealizationResult { get; }
            private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
            
            internal static _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(previousNodeRealizationResult);
            }
            
            public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
            {
                return this;
            }
            
            public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
            {
                return this.realizationResult.Value;
            }
            
            private IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> RealizeImpl()
            {
                throw new Exception("TODO");
            }
        }
        
        public abstract class Realized : _Ⰳx00ⲻFF<ParseMode.Realized>
        {
            private Realized()
            {
            }
            
            protected abstract TResult Dispatch<TResult, TContext>(_Ⰳx00ⲻFF<TMode>.Realized.Visitor<TResult, TContext> visitor, TContext context);
            
            public abstract class Visitor<TResult, TContext>
            {
                public TResult Visit(_Ⰳx00ⲻFF<TMode>.Realized node, TContext context)
                {
                    return node.Dispatch(this, context);
                }
                
                public TResult Visit(_Ⰳx00ⲻFF<ParseMode.Realized> node, TContext context)
                {
                    //// TODO is there a way to avoid this cast?
return (node as _Ⰳx00ⲻFF<TMode>.Realized)!.Dispatch(this, context);
                }
                
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._00 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._01 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._02 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._03 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._04 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._05 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._06 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._07 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._08 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._09 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._0A node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._0B node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._0C node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._0D node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._0E node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._0F node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._10 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._11 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._12 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._13 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._14 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._15 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._16 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._17 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._18 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._19 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._1A node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._1B node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._1C node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._1D node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._1E node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._1F node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._20 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._21 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._22 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._23 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._24 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._25 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._26 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._27 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._28 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._29 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._2A node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._2B node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._2C node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._2D node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._2E node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._2F node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._30 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._31 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._32 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._33 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._34 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._35 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._36 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._37 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._38 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._39 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._3A node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._3B node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._3C node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._3D node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._3E node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._3F node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._40 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._41 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._42 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._43 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._44 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._45 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._46 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._47 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._48 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._49 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._4A node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._4B node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._4C node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._4D node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._4E node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._4F node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._50 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._51 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._52 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._53 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._54 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._55 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._56 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._57 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._58 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._59 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._5A node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._5B node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._5C node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._5D node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._5E node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._5F node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._60 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._61 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._62 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._63 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._64 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._65 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._66 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._67 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._68 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._69 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._6A node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._6B node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._6C node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._6D node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._6E node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._6F node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._70 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._71 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._72 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._73 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._74 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._75 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._76 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._77 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._78 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._79 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._7A node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._7B node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._7C node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._7D node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._7E node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._7F node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._80 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._81 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._82 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._83 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._84 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._85 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._86 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._87 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._88 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._89 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._8A node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._8B node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._8C node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._8D node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._8E node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._8F node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._90 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._91 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._92 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._93 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._94 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._95 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._96 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._97 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._98 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._99 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._9A node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._9B node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._9C node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._9D node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._9E node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._9F node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._A0 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._A1 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._A2 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._A3 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._A4 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._A5 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._A6 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._A7 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._A8 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._A9 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._AA node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._AB node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._AC node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._AD node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._AE node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._AF node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._B0 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._B1 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._B2 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._B3 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._B4 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._B5 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._B6 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._B7 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._B8 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._B9 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._BA node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._BB node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._BC node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._BD node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._BE node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._BF node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._C0 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._C1 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._C2 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._C3 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._C4 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._C5 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._C6 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._C7 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._C8 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._C9 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._CA node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._CB node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._CC node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._CD node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._CE node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._CF node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._D0 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._D1 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._D2 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._D3 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._D4 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._D5 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._D6 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._D7 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._D8 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._D9 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._DA node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._DB node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._DC node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._DD node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._DE node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._DF node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._E0 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._E1 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._E2 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._E3 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._E4 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._E5 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._E6 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._E7 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._E8 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._E9 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._EA node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._EB node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._EC node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._ED node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._EE node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._EF node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._F0 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._F1 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._F2 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._F3 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._F4 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._F5 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._F6 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._F7 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._F8 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._F9 node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._FA node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._FB node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._FC node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._FD node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._FE node, TContext context);
                protected internal abstract TResult Accept(_Ⰳx00ⲻFF<TMode>.Realized._FF node, TContext context);
            }
            
            public sealed class _00 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _00(__GeneratedPartialV1.Deferred.CstNodes.Inners._0<ParseMode.Realized> _0_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._0<ParseMode.Realized> _0_2, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._0_1 = _0_1;
                    this._0_2 = _0_2;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._00>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._00> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<ParseMode.Realized> _0_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<ParseMode.Realized> _0_2 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._00> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _0_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._0.Create(previousNodeRealizationResult).Realize();if (!_0_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._00>(false, default, _0_1.RemainingTokens);
}


var _0_2 = __GeneratedPartialV1.Deferred.CstNodes.Inners._0.Create(Future.Create(() => _0_1)).Realize();if (!_0_2.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._00>(false, default, _0_2.RemainingTokens);
}


var node = new _00(_0_1.RealizedValue, _0_2.RealizedValue, _0_2.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _01 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _01(__GeneratedPartialV1.Deferred.CstNodes.Inners._0<ParseMode.Realized> _0_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._1<ParseMode.Realized> _1_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._0_1 = _0_1;
                    this._1_1 = _1_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._01>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._01> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<ParseMode.Realized> _0_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<ParseMode.Realized> _1_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._01> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _0_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._0.Create(previousNodeRealizationResult).Realize();if (!_0_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._01>(false, default, _0_1.RemainingTokens);
}


var _1_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._1.Create(Future.Create(() => _0_1)).Realize();if (!_1_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._01>(false, default, _1_1.RemainingTokens);
}


var node = new _01(_0_1.RealizedValue, _1_1.RealizedValue, _1_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _02 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _02(__GeneratedPartialV1.Deferred.CstNodes.Inners._0<ParseMode.Realized> _0_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._0_1 = _0_1;
                    this._2_1 = _2_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._02>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._02> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<ParseMode.Realized> _0_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._02> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _0_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._0.Create(previousNodeRealizationResult).Realize();if (!_0_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._02>(false, default, _0_1.RemainingTokens);
}


var _2_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._2.Create(Future.Create(() => _0_1)).Realize();if (!_2_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._02>(false, default, _2_1.RemainingTokens);
}


var node = new _02(_0_1.RealizedValue, _2_1.RealizedValue, _2_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _03 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _03(__GeneratedPartialV1.Deferred.CstNodes.Inners._0<ParseMode.Realized> _0_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._0_1 = _0_1;
                    this._3_1 = _3_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._03>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._03> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<ParseMode.Realized> _0_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._03> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _0_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._0.Create(previousNodeRealizationResult).Realize();if (!_0_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._03>(false, default, _0_1.RemainingTokens);
}


var _3_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._3.Create(Future.Create(() => _0_1)).Realize();if (!_3_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._03>(false, default, _3_1.RemainingTokens);
}


var node = new _03(_0_1.RealizedValue, _3_1.RealizedValue, _3_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _04 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _04(__GeneratedPartialV1.Deferred.CstNodes.Inners._0<ParseMode.Realized> _0_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._0_1 = _0_1;
                    this._4_1 = _4_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._04>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._04> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<ParseMode.Realized> _0_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._04> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _0_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._0.Create(previousNodeRealizationResult).Realize();if (!_0_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._04>(false, default, _0_1.RemainingTokens);
}


var _4_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._4.Create(Future.Create(() => _0_1)).Realize();if (!_4_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._04>(false, default, _4_1.RemainingTokens);
}


var node = new _04(_0_1.RealizedValue, _4_1.RealizedValue, _4_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _05 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _05(__GeneratedPartialV1.Deferred.CstNodes.Inners._0<ParseMode.Realized> _0_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._0_1 = _0_1;
                    this._5_1 = _5_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._05>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._05> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<ParseMode.Realized> _0_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._05> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _0_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._0.Create(previousNodeRealizationResult).Realize();if (!_0_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._05>(false, default, _0_1.RemainingTokens);
}


var _5_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._5.Create(Future.Create(() => _0_1)).Realize();if (!_5_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._05>(false, default, _5_1.RemainingTokens);
}


var node = new _05(_0_1.RealizedValue, _5_1.RealizedValue, _5_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _06 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _06(__GeneratedPartialV1.Deferred.CstNodes.Inners._0<ParseMode.Realized> _0_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._0_1 = _0_1;
                    this._6_1 = _6_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._06>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._06> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<ParseMode.Realized> _0_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._06> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _0_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._0.Create(previousNodeRealizationResult).Realize();if (!_0_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._06>(false, default, _0_1.RemainingTokens);
}


var _6_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._6.Create(Future.Create(() => _0_1)).Realize();if (!_6_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._06>(false, default, _6_1.RemainingTokens);
}


var node = new _06(_0_1.RealizedValue, _6_1.RealizedValue, _6_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _07 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _07(__GeneratedPartialV1.Deferred.CstNodes.Inners._0<ParseMode.Realized> _0_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._0_1 = _0_1;
                    this._7_1 = _7_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._07>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._07> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<ParseMode.Realized> _0_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._07> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _0_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._0.Create(previousNodeRealizationResult).Realize();if (!_0_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._07>(false, default, _0_1.RemainingTokens);
}


var _7_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._7.Create(Future.Create(() => _0_1)).Realize();if (!_7_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._07>(false, default, _7_1.RemainingTokens);
}


var node = new _07(_0_1.RealizedValue, _7_1.RealizedValue, _7_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _08 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _08(__GeneratedPartialV1.Deferred.CstNodes.Inners._0<ParseMode.Realized> _0_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._8<ParseMode.Realized> _8_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._0_1 = _0_1;
                    this._8_1 = _8_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._08>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._08> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<ParseMode.Realized> _0_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._8<ParseMode.Realized> _8_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._08> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _0_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._0.Create(previousNodeRealizationResult).Realize();if (!_0_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._08>(false, default, _0_1.RemainingTokens);
}


var _8_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._8.Create(Future.Create(() => _0_1)).Realize();if (!_8_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._08>(false, default, _8_1.RemainingTokens);
}


var node = new _08(_0_1.RealizedValue, _8_1.RealizedValue, _8_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _09 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _09(__GeneratedPartialV1.Deferred.CstNodes.Inners._0<ParseMode.Realized> _0_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._9<ParseMode.Realized> _9_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._0_1 = _0_1;
                    this._9_1 = _9_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._09>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._09> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<ParseMode.Realized> _0_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._9<ParseMode.Realized> _9_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._09> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _0_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._0.Create(previousNodeRealizationResult).Realize();if (!_0_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._09>(false, default, _0_1.RemainingTokens);
}


var _9_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._9.Create(Future.Create(() => _0_1)).Realize();if (!_9_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._09>(false, default, _9_1.RemainingTokens);
}


var node = new _09(_0_1.RealizedValue, _9_1.RealizedValue, _9_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _0A : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _0A(__GeneratedPartialV1.Deferred.CstNodes.Inners._0<ParseMode.Realized> _0_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._A<ParseMode.Realized> _A_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._0_1 = _0_1;
                    this._A_1 = _A_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._0A>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._0A> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<ParseMode.Realized> _0_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._A<ParseMode.Realized> _A_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._0A> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _0_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._0.Create(previousNodeRealizationResult).Realize();if (!_0_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._0A>(false, default, _0_1.RemainingTokens);
}


var _A_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._A.Create(Future.Create(() => _0_1)).Realize();if (!_A_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._0A>(false, default, _A_1.RemainingTokens);
}


var node = new _0A(_0_1.RealizedValue, _A_1.RealizedValue, _A_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _0B : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _0B(__GeneratedPartialV1.Deferred.CstNodes.Inners._0<ParseMode.Realized> _0_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._B<ParseMode.Realized> _B_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._0_1 = _0_1;
                    this._B_1 = _B_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._0B>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._0B> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<ParseMode.Realized> _0_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._B<ParseMode.Realized> _B_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._0B> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _0_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._0.Create(previousNodeRealizationResult).Realize();if (!_0_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._0B>(false, default, _0_1.RemainingTokens);
}


var _B_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._B.Create(Future.Create(() => _0_1)).Realize();if (!_B_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._0B>(false, default, _B_1.RemainingTokens);
}


var node = new _0B(_0_1.RealizedValue, _B_1.RealizedValue, _B_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _0C : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _0C(__GeneratedPartialV1.Deferred.CstNodes.Inners._0<ParseMode.Realized> _0_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._C<ParseMode.Realized> _C_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._0_1 = _0_1;
                    this._C_1 = _C_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._0C>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._0C> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<ParseMode.Realized> _0_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._C<ParseMode.Realized> _C_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._0C> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _0_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._0.Create(previousNodeRealizationResult).Realize();if (!_0_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._0C>(false, default, _0_1.RemainingTokens);
}


var _C_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._C.Create(Future.Create(() => _0_1)).Realize();if (!_C_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._0C>(false, default, _C_1.RemainingTokens);
}


var node = new _0C(_0_1.RealizedValue, _C_1.RealizedValue, _C_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _0D : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _0D(__GeneratedPartialV1.Deferred.CstNodes.Inners._0<ParseMode.Realized> _0_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._D<ParseMode.Realized> _D_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._0_1 = _0_1;
                    this._D_1 = _D_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._0D>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._0D> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<ParseMode.Realized> _0_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._D<ParseMode.Realized> _D_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._0D> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _0_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._0.Create(previousNodeRealizationResult).Realize();if (!_0_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._0D>(false, default, _0_1.RemainingTokens);
}


var _D_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._D.Create(Future.Create(() => _0_1)).Realize();if (!_D_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._0D>(false, default, _D_1.RemainingTokens);
}


var node = new _0D(_0_1.RealizedValue, _D_1.RealizedValue, _D_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _0E : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _0E(__GeneratedPartialV1.Deferred.CstNodes.Inners._0<ParseMode.Realized> _0_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._E<ParseMode.Realized> _E_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._0_1 = _0_1;
                    this._E_1 = _E_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._0E>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._0E> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<ParseMode.Realized> _0_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._E<ParseMode.Realized> _E_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._0E> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _0_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._0.Create(previousNodeRealizationResult).Realize();if (!_0_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._0E>(false, default, _0_1.RemainingTokens);
}


var _E_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._E.Create(Future.Create(() => _0_1)).Realize();if (!_E_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._0E>(false, default, _E_1.RemainingTokens);
}


var node = new _0E(_0_1.RealizedValue, _E_1.RealizedValue, _E_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _0F : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _0F(__GeneratedPartialV1.Deferred.CstNodes.Inners._0<ParseMode.Realized> _0_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._F<ParseMode.Realized> _F_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._0_1 = _0_1;
                    this._F_1 = _F_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._0F>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._0F> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<ParseMode.Realized> _0_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._F<ParseMode.Realized> _F_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._0F> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _0_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._0.Create(previousNodeRealizationResult).Realize();if (!_0_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._0F>(false, default, _0_1.RemainingTokens);
}


var _F_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._F.Create(Future.Create(() => _0_1)).Realize();if (!_F_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._0F>(false, default, _F_1.RemainingTokens);
}


var node = new _0F(_0_1.RealizedValue, _F_1.RealizedValue, _F_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _10 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _10(__GeneratedPartialV1.Deferred.CstNodes.Inners._1<ParseMode.Realized> _1_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._0<ParseMode.Realized> _0_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._1_1 = _1_1;
                    this._0_1 = _0_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._10>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._10> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<ParseMode.Realized> _1_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<ParseMode.Realized> _0_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._10> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _1_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._1.Create(previousNodeRealizationResult).Realize();if (!_1_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._10>(false, default, _1_1.RemainingTokens);
}


var _0_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._0.Create(Future.Create(() => _1_1)).Realize();if (!_0_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._10>(false, default, _0_1.RemainingTokens);
}


var node = new _10(_1_1.RealizedValue, _0_1.RealizedValue, _0_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _11 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _11(__GeneratedPartialV1.Deferred.CstNodes.Inners._1<ParseMode.Realized> _1_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._1<ParseMode.Realized> _1_2, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._1_1 = _1_1;
                    this._1_2 = _1_2;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._11>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._11> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<ParseMode.Realized> _1_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<ParseMode.Realized> _1_2 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._11> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _1_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._1.Create(previousNodeRealizationResult).Realize();if (!_1_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._11>(false, default, _1_1.RemainingTokens);
}


var _1_2 = __GeneratedPartialV1.Deferred.CstNodes.Inners._1.Create(Future.Create(() => _1_1)).Realize();if (!_1_2.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._11>(false, default, _1_2.RemainingTokens);
}


var node = new _11(_1_1.RealizedValue, _1_2.RealizedValue, _1_2.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _12 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _12(__GeneratedPartialV1.Deferred.CstNodes.Inners._1<ParseMode.Realized> _1_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._1_1 = _1_1;
                    this._2_1 = _2_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._12>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._12> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<ParseMode.Realized> _1_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._12> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _1_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._1.Create(previousNodeRealizationResult).Realize();if (!_1_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._12>(false, default, _1_1.RemainingTokens);
}


var _2_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._2.Create(Future.Create(() => _1_1)).Realize();if (!_2_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._12>(false, default, _2_1.RemainingTokens);
}


var node = new _12(_1_1.RealizedValue, _2_1.RealizedValue, _2_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _13 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _13(__GeneratedPartialV1.Deferred.CstNodes.Inners._1<ParseMode.Realized> _1_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._1_1 = _1_1;
                    this._3_1 = _3_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._13>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._13> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<ParseMode.Realized> _1_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._13> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _1_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._1.Create(previousNodeRealizationResult).Realize();if (!_1_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._13>(false, default, _1_1.RemainingTokens);
}


var _3_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._3.Create(Future.Create(() => _1_1)).Realize();if (!_3_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._13>(false, default, _3_1.RemainingTokens);
}


var node = new _13(_1_1.RealizedValue, _3_1.RealizedValue, _3_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _14 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _14(__GeneratedPartialV1.Deferred.CstNodes.Inners._1<ParseMode.Realized> _1_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._1_1 = _1_1;
                    this._4_1 = _4_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._14>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._14> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<ParseMode.Realized> _1_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._14> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _1_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._1.Create(previousNodeRealizationResult).Realize();if (!_1_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._14>(false, default, _1_1.RemainingTokens);
}


var _4_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._4.Create(Future.Create(() => _1_1)).Realize();if (!_4_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._14>(false, default, _4_1.RemainingTokens);
}


var node = new _14(_1_1.RealizedValue, _4_1.RealizedValue, _4_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _15 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _15(__GeneratedPartialV1.Deferred.CstNodes.Inners._1<ParseMode.Realized> _1_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._1_1 = _1_1;
                    this._5_1 = _5_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._15>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._15> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<ParseMode.Realized> _1_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._15> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _1_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._1.Create(previousNodeRealizationResult).Realize();if (!_1_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._15>(false, default, _1_1.RemainingTokens);
}


var _5_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._5.Create(Future.Create(() => _1_1)).Realize();if (!_5_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._15>(false, default, _5_1.RemainingTokens);
}


var node = new _15(_1_1.RealizedValue, _5_1.RealizedValue, _5_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _16 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _16(__GeneratedPartialV1.Deferred.CstNodes.Inners._1<ParseMode.Realized> _1_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._1_1 = _1_1;
                    this._6_1 = _6_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._16>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._16> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<ParseMode.Realized> _1_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._16> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _1_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._1.Create(previousNodeRealizationResult).Realize();if (!_1_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._16>(false, default, _1_1.RemainingTokens);
}


var _6_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._6.Create(Future.Create(() => _1_1)).Realize();if (!_6_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._16>(false, default, _6_1.RemainingTokens);
}


var node = new _16(_1_1.RealizedValue, _6_1.RealizedValue, _6_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _17 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _17(__GeneratedPartialV1.Deferred.CstNodes.Inners._1<ParseMode.Realized> _1_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._1_1 = _1_1;
                    this._7_1 = _7_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._17>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._17> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<ParseMode.Realized> _1_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._17> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _1_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._1.Create(previousNodeRealizationResult).Realize();if (!_1_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._17>(false, default, _1_1.RemainingTokens);
}


var _7_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._7.Create(Future.Create(() => _1_1)).Realize();if (!_7_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._17>(false, default, _7_1.RemainingTokens);
}


var node = new _17(_1_1.RealizedValue, _7_1.RealizedValue, _7_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _18 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _18(__GeneratedPartialV1.Deferred.CstNodes.Inners._1<ParseMode.Realized> _1_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._8<ParseMode.Realized> _8_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._1_1 = _1_1;
                    this._8_1 = _8_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._18>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._18> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<ParseMode.Realized> _1_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._8<ParseMode.Realized> _8_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._18> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _1_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._1.Create(previousNodeRealizationResult).Realize();if (!_1_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._18>(false, default, _1_1.RemainingTokens);
}


var _8_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._8.Create(Future.Create(() => _1_1)).Realize();if (!_8_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._18>(false, default, _8_1.RemainingTokens);
}


var node = new _18(_1_1.RealizedValue, _8_1.RealizedValue, _8_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _19 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _19(__GeneratedPartialV1.Deferred.CstNodes.Inners._1<ParseMode.Realized> _1_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._9<ParseMode.Realized> _9_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._1_1 = _1_1;
                    this._9_1 = _9_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._19>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._19> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<ParseMode.Realized> _1_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._9<ParseMode.Realized> _9_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._19> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _1_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._1.Create(previousNodeRealizationResult).Realize();if (!_1_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._19>(false, default, _1_1.RemainingTokens);
}


var _9_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._9.Create(Future.Create(() => _1_1)).Realize();if (!_9_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._19>(false, default, _9_1.RemainingTokens);
}


var node = new _19(_1_1.RealizedValue, _9_1.RealizedValue, _9_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _1A : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _1A(__GeneratedPartialV1.Deferred.CstNodes.Inners._1<ParseMode.Realized> _1_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._A<ParseMode.Realized> _A_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._1_1 = _1_1;
                    this._A_1 = _A_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._1A>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._1A> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<ParseMode.Realized> _1_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._A<ParseMode.Realized> _A_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._1A> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _1_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._1.Create(previousNodeRealizationResult).Realize();if (!_1_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._1A>(false, default, _1_1.RemainingTokens);
}


var _A_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._A.Create(Future.Create(() => _1_1)).Realize();if (!_A_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._1A>(false, default, _A_1.RemainingTokens);
}


var node = new _1A(_1_1.RealizedValue, _A_1.RealizedValue, _A_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _1B : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _1B(__GeneratedPartialV1.Deferred.CstNodes.Inners._1<ParseMode.Realized> _1_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._B<ParseMode.Realized> _B_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._1_1 = _1_1;
                    this._B_1 = _B_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._1B>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._1B> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<ParseMode.Realized> _1_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._B<ParseMode.Realized> _B_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._1B> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _1_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._1.Create(previousNodeRealizationResult).Realize();if (!_1_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._1B>(false, default, _1_1.RemainingTokens);
}


var _B_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._B.Create(Future.Create(() => _1_1)).Realize();if (!_B_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._1B>(false, default, _B_1.RemainingTokens);
}


var node = new _1B(_1_1.RealizedValue, _B_1.RealizedValue, _B_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _1C : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _1C(__GeneratedPartialV1.Deferred.CstNodes.Inners._1<ParseMode.Realized> _1_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._C<ParseMode.Realized> _C_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._1_1 = _1_1;
                    this._C_1 = _C_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._1C>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._1C> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<ParseMode.Realized> _1_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._C<ParseMode.Realized> _C_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._1C> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _1_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._1.Create(previousNodeRealizationResult).Realize();if (!_1_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._1C>(false, default, _1_1.RemainingTokens);
}


var _C_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._C.Create(Future.Create(() => _1_1)).Realize();if (!_C_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._1C>(false, default, _C_1.RemainingTokens);
}


var node = new _1C(_1_1.RealizedValue, _C_1.RealizedValue, _C_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _1D : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _1D(__GeneratedPartialV1.Deferred.CstNodes.Inners._1<ParseMode.Realized> _1_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._D<ParseMode.Realized> _D_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._1_1 = _1_1;
                    this._D_1 = _D_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._1D>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._1D> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<ParseMode.Realized> _1_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._D<ParseMode.Realized> _D_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._1D> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _1_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._1.Create(previousNodeRealizationResult).Realize();if (!_1_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._1D>(false, default, _1_1.RemainingTokens);
}


var _D_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._D.Create(Future.Create(() => _1_1)).Realize();if (!_D_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._1D>(false, default, _D_1.RemainingTokens);
}


var node = new _1D(_1_1.RealizedValue, _D_1.RealizedValue, _D_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _1E : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _1E(__GeneratedPartialV1.Deferred.CstNodes.Inners._1<ParseMode.Realized> _1_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._E<ParseMode.Realized> _E_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._1_1 = _1_1;
                    this._E_1 = _E_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._1E>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._1E> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<ParseMode.Realized> _1_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._E<ParseMode.Realized> _E_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._1E> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _1_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._1.Create(previousNodeRealizationResult).Realize();if (!_1_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._1E>(false, default, _1_1.RemainingTokens);
}


var _E_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._E.Create(Future.Create(() => _1_1)).Realize();if (!_E_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._1E>(false, default, _E_1.RemainingTokens);
}


var node = new _1E(_1_1.RealizedValue, _E_1.RealizedValue, _E_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _1F : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _1F(__GeneratedPartialV1.Deferred.CstNodes.Inners._1<ParseMode.Realized> _1_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._F<ParseMode.Realized> _F_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._1_1 = _1_1;
                    this._F_1 = _F_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._1F>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._1F> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<ParseMode.Realized> _1_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._F<ParseMode.Realized> _F_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._1F> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _1_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._1.Create(previousNodeRealizationResult).Realize();if (!_1_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._1F>(false, default, _1_1.RemainingTokens);
}


var _F_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._F.Create(Future.Create(() => _1_1)).Realize();if (!_F_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._1F>(false, default, _F_1.RemainingTokens);
}


var node = new _1F(_1_1.RealizedValue, _F_1.RealizedValue, _F_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _20 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _20(__GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._0<ParseMode.Realized> _0_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._2_1 = _2_1;
                    this._0_1 = _0_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._20>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._20> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<ParseMode.Realized> _0_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._20> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _2_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._2.Create(previousNodeRealizationResult).Realize();if (!_2_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._20>(false, default, _2_1.RemainingTokens);
}


var _0_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._0.Create(Future.Create(() => _2_1)).Realize();if (!_0_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._20>(false, default, _0_1.RemainingTokens);
}


var node = new _20(_2_1.RealizedValue, _0_1.RealizedValue, _0_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _21 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _21(__GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._1<ParseMode.Realized> _1_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._2_1 = _2_1;
                    this._1_1 = _1_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._21>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._21> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<ParseMode.Realized> _1_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._21> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _2_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._2.Create(previousNodeRealizationResult).Realize();if (!_2_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._21>(false, default, _2_1.RemainingTokens);
}


var _1_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._1.Create(Future.Create(() => _2_1)).Realize();if (!_1_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._21>(false, default, _1_1.RemainingTokens);
}


var node = new _21(_2_1.RealizedValue, _1_1.RealizedValue, _1_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _22 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _22(__GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_2, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._2_1 = _2_1;
                    this._2_2 = _2_2;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._22>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._22> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_2 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._22> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _2_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._2.Create(previousNodeRealizationResult).Realize();if (!_2_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._22>(false, default, _2_1.RemainingTokens);
}


var _2_2 = __GeneratedPartialV1.Deferred.CstNodes.Inners._2.Create(Future.Create(() => _2_1)).Realize();if (!_2_2.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._22>(false, default, _2_2.RemainingTokens);
}


var node = new _22(_2_1.RealizedValue, _2_2.RealizedValue, _2_2.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _23 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _23(__GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._2_1 = _2_1;
                    this._3_1 = _3_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._23>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._23> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._23> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _2_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._2.Create(previousNodeRealizationResult).Realize();if (!_2_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._23>(false, default, _2_1.RemainingTokens);
}


var _3_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._3.Create(Future.Create(() => _2_1)).Realize();if (!_3_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._23>(false, default, _3_1.RemainingTokens);
}


var node = new _23(_2_1.RealizedValue, _3_1.RealizedValue, _3_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _24 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _24(__GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._2_1 = _2_1;
                    this._4_1 = _4_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._24>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._24> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._24> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _2_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._2.Create(previousNodeRealizationResult).Realize();if (!_2_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._24>(false, default, _2_1.RemainingTokens);
}


var _4_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._4.Create(Future.Create(() => _2_1)).Realize();if (!_4_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._24>(false, default, _4_1.RemainingTokens);
}


var node = new _24(_2_1.RealizedValue, _4_1.RealizedValue, _4_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _25 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _25(__GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._2_1 = _2_1;
                    this._5_1 = _5_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._25>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._25> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._25> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _2_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._2.Create(previousNodeRealizationResult).Realize();if (!_2_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._25>(false, default, _2_1.RemainingTokens);
}


var _5_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._5.Create(Future.Create(() => _2_1)).Realize();if (!_5_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._25>(false, default, _5_1.RemainingTokens);
}


var node = new _25(_2_1.RealizedValue, _5_1.RealizedValue, _5_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _26 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _26(__GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._2_1 = _2_1;
                    this._6_1 = _6_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._26>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._26> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._26> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _2_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._2.Create(previousNodeRealizationResult).Realize();if (!_2_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._26>(false, default, _2_1.RemainingTokens);
}


var _6_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._6.Create(Future.Create(() => _2_1)).Realize();if (!_6_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._26>(false, default, _6_1.RemainingTokens);
}


var node = new _26(_2_1.RealizedValue, _6_1.RealizedValue, _6_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _27 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _27(__GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._2_1 = _2_1;
                    this._7_1 = _7_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._27>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._27> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._27> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _2_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._2.Create(previousNodeRealizationResult).Realize();if (!_2_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._27>(false, default, _2_1.RemainingTokens);
}


var _7_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._7.Create(Future.Create(() => _2_1)).Realize();if (!_7_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._27>(false, default, _7_1.RemainingTokens);
}


var node = new _27(_2_1.RealizedValue, _7_1.RealizedValue, _7_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _28 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _28(__GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._8<ParseMode.Realized> _8_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._2_1 = _2_1;
                    this._8_1 = _8_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._28>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._28> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._8<ParseMode.Realized> _8_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._28> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _2_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._2.Create(previousNodeRealizationResult).Realize();if (!_2_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._28>(false, default, _2_1.RemainingTokens);
}


var _8_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._8.Create(Future.Create(() => _2_1)).Realize();if (!_8_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._28>(false, default, _8_1.RemainingTokens);
}


var node = new _28(_2_1.RealizedValue, _8_1.RealizedValue, _8_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _29 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _29(__GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._9<ParseMode.Realized> _9_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._2_1 = _2_1;
                    this._9_1 = _9_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._29>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._29> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._9<ParseMode.Realized> _9_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._29> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _2_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._2.Create(previousNodeRealizationResult).Realize();if (!_2_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._29>(false, default, _2_1.RemainingTokens);
}


var _9_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._9.Create(Future.Create(() => _2_1)).Realize();if (!_9_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._29>(false, default, _9_1.RemainingTokens);
}


var node = new _29(_2_1.RealizedValue, _9_1.RealizedValue, _9_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _2A : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _2A(__GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._A<ParseMode.Realized> _A_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._2_1 = _2_1;
                    this._A_1 = _A_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._2A>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._2A> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._A<ParseMode.Realized> _A_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._2A> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _2_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._2.Create(previousNodeRealizationResult).Realize();if (!_2_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._2A>(false, default, _2_1.RemainingTokens);
}


var _A_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._A.Create(Future.Create(() => _2_1)).Realize();if (!_A_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._2A>(false, default, _A_1.RemainingTokens);
}


var node = new _2A(_2_1.RealizedValue, _A_1.RealizedValue, _A_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _2B : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _2B(__GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._B<ParseMode.Realized> _B_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._2_1 = _2_1;
                    this._B_1 = _B_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._2B>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._2B> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._B<ParseMode.Realized> _B_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._2B> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _2_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._2.Create(previousNodeRealizationResult).Realize();if (!_2_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._2B>(false, default, _2_1.RemainingTokens);
}


var _B_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._B.Create(Future.Create(() => _2_1)).Realize();if (!_B_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._2B>(false, default, _B_1.RemainingTokens);
}


var node = new _2B(_2_1.RealizedValue, _B_1.RealizedValue, _B_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _2C : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _2C(__GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._C<ParseMode.Realized> _C_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._2_1 = _2_1;
                    this._C_1 = _C_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._2C>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._2C> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._C<ParseMode.Realized> _C_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._2C> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _2_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._2.Create(previousNodeRealizationResult).Realize();if (!_2_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._2C>(false, default, _2_1.RemainingTokens);
}


var _C_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._C.Create(Future.Create(() => _2_1)).Realize();if (!_C_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._2C>(false, default, _C_1.RemainingTokens);
}


var node = new _2C(_2_1.RealizedValue, _C_1.RealizedValue, _C_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _2D : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _2D(__GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._D<ParseMode.Realized> _D_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._2_1 = _2_1;
                    this._D_1 = _D_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._2D>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._2D> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._D<ParseMode.Realized> _D_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._2D> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _2_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._2.Create(previousNodeRealizationResult).Realize();if (!_2_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._2D>(false, default, _2_1.RemainingTokens);
}


var _D_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._D.Create(Future.Create(() => _2_1)).Realize();if (!_D_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._2D>(false, default, _D_1.RemainingTokens);
}


var node = new _2D(_2_1.RealizedValue, _D_1.RealizedValue, _D_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _2E : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _2E(__GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._E<ParseMode.Realized> _E_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._2_1 = _2_1;
                    this._E_1 = _E_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._2E>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._2E> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._E<ParseMode.Realized> _E_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._2E> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _2_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._2.Create(previousNodeRealizationResult).Realize();if (!_2_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._2E>(false, default, _2_1.RemainingTokens);
}


var _E_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._E.Create(Future.Create(() => _2_1)).Realize();if (!_E_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._2E>(false, default, _E_1.RemainingTokens);
}


var node = new _2E(_2_1.RealizedValue, _E_1.RealizedValue, _E_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _2F : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _2F(__GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._F<ParseMode.Realized> _F_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._2_1 = _2_1;
                    this._F_1 = _F_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._2F>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._2F> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._F<ParseMode.Realized> _F_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._2F> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _2_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._2.Create(previousNodeRealizationResult).Realize();if (!_2_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._2F>(false, default, _2_1.RemainingTokens);
}


var _F_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._F.Create(Future.Create(() => _2_1)).Realize();if (!_F_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._2F>(false, default, _F_1.RemainingTokens);
}


var node = new _2F(_2_1.RealizedValue, _F_1.RealizedValue, _F_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _30 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _30(__GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._0<ParseMode.Realized> _0_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._3_1 = _3_1;
                    this._0_1 = _0_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._30>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._30> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<ParseMode.Realized> _0_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._30> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _3_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._3.Create(previousNodeRealizationResult).Realize();if (!_3_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._30>(false, default, _3_1.RemainingTokens);
}


var _0_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._0.Create(Future.Create(() => _3_1)).Realize();if (!_0_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._30>(false, default, _0_1.RemainingTokens);
}


var node = new _30(_3_1.RealizedValue, _0_1.RealizedValue, _0_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _31 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _31(__GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._1<ParseMode.Realized> _1_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._3_1 = _3_1;
                    this._1_1 = _1_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._31>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._31> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<ParseMode.Realized> _1_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._31> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _3_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._3.Create(previousNodeRealizationResult).Realize();if (!_3_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._31>(false, default, _3_1.RemainingTokens);
}


var _1_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._1.Create(Future.Create(() => _3_1)).Realize();if (!_1_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._31>(false, default, _1_1.RemainingTokens);
}


var node = new _31(_3_1.RealizedValue, _1_1.RealizedValue, _1_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _32 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _32(__GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._3_1 = _3_1;
                    this._2_1 = _2_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._32>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._32> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._32> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _3_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._3.Create(previousNodeRealizationResult).Realize();if (!_3_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._32>(false, default, _3_1.RemainingTokens);
}


var _2_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._2.Create(Future.Create(() => _3_1)).Realize();if (!_2_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._32>(false, default, _2_1.RemainingTokens);
}


var node = new _32(_3_1.RealizedValue, _2_1.RealizedValue, _2_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _33 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _33(__GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_2, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._3_1 = _3_1;
                    this._3_2 = _3_2;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._33>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._33> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_2 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._33> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _3_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._3.Create(previousNodeRealizationResult).Realize();if (!_3_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._33>(false, default, _3_1.RemainingTokens);
}


var _3_2 = __GeneratedPartialV1.Deferred.CstNodes.Inners._3.Create(Future.Create(() => _3_1)).Realize();if (!_3_2.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._33>(false, default, _3_2.RemainingTokens);
}


var node = new _33(_3_1.RealizedValue, _3_2.RealizedValue, _3_2.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _34 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _34(__GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._3_1 = _3_1;
                    this._4_1 = _4_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._34>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._34> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._34> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _3_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._3.Create(previousNodeRealizationResult).Realize();if (!_3_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._34>(false, default, _3_1.RemainingTokens);
}


var _4_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._4.Create(Future.Create(() => _3_1)).Realize();if (!_4_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._34>(false, default, _4_1.RemainingTokens);
}


var node = new _34(_3_1.RealizedValue, _4_1.RealizedValue, _4_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _35 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _35(__GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._3_1 = _3_1;
                    this._5_1 = _5_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._35>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._35> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._35> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _3_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._3.Create(previousNodeRealizationResult).Realize();if (!_3_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._35>(false, default, _3_1.RemainingTokens);
}


var _5_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._5.Create(Future.Create(() => _3_1)).Realize();if (!_5_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._35>(false, default, _5_1.RemainingTokens);
}


var node = new _35(_3_1.RealizedValue, _5_1.RealizedValue, _5_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _36 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _36(__GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._3_1 = _3_1;
                    this._6_1 = _6_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._36>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._36> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._36> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _3_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._3.Create(previousNodeRealizationResult).Realize();if (!_3_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._36>(false, default, _3_1.RemainingTokens);
}


var _6_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._6.Create(Future.Create(() => _3_1)).Realize();if (!_6_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._36>(false, default, _6_1.RemainingTokens);
}


var node = new _36(_3_1.RealizedValue, _6_1.RealizedValue, _6_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _37 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _37(__GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._3_1 = _3_1;
                    this._7_1 = _7_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._37>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._37> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._37> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _3_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._3.Create(previousNodeRealizationResult).Realize();if (!_3_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._37>(false, default, _3_1.RemainingTokens);
}


var _7_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._7.Create(Future.Create(() => _3_1)).Realize();if (!_7_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._37>(false, default, _7_1.RemainingTokens);
}


var node = new _37(_3_1.RealizedValue, _7_1.RealizedValue, _7_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _38 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _38(__GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._8<ParseMode.Realized> _8_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._3_1 = _3_1;
                    this._8_1 = _8_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._38>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._38> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._8<ParseMode.Realized> _8_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._38> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _3_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._3.Create(previousNodeRealizationResult).Realize();if (!_3_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._38>(false, default, _3_1.RemainingTokens);
}


var _8_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._8.Create(Future.Create(() => _3_1)).Realize();if (!_8_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._38>(false, default, _8_1.RemainingTokens);
}


var node = new _38(_3_1.RealizedValue, _8_1.RealizedValue, _8_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _39 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _39(__GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._9<ParseMode.Realized> _9_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._3_1 = _3_1;
                    this._9_1 = _9_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._39>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._39> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._9<ParseMode.Realized> _9_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._39> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _3_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._3.Create(previousNodeRealizationResult).Realize();if (!_3_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._39>(false, default, _3_1.RemainingTokens);
}


var _9_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._9.Create(Future.Create(() => _3_1)).Realize();if (!_9_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._39>(false, default, _9_1.RemainingTokens);
}


var node = new _39(_3_1.RealizedValue, _9_1.RealizedValue, _9_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _3A : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _3A(__GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._A<ParseMode.Realized> _A_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._3_1 = _3_1;
                    this._A_1 = _A_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._3A>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._3A> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._A<ParseMode.Realized> _A_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._3A> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _3_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._3.Create(previousNodeRealizationResult).Realize();if (!_3_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._3A>(false, default, _3_1.RemainingTokens);
}


var _A_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._A.Create(Future.Create(() => _3_1)).Realize();if (!_A_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._3A>(false, default, _A_1.RemainingTokens);
}


var node = new _3A(_3_1.RealizedValue, _A_1.RealizedValue, _A_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _3B : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _3B(__GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._B<ParseMode.Realized> _B_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._3_1 = _3_1;
                    this._B_1 = _B_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._3B>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._3B> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._B<ParseMode.Realized> _B_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._3B> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _3_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._3.Create(previousNodeRealizationResult).Realize();if (!_3_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._3B>(false, default, _3_1.RemainingTokens);
}


var _B_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._B.Create(Future.Create(() => _3_1)).Realize();if (!_B_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._3B>(false, default, _B_1.RemainingTokens);
}


var node = new _3B(_3_1.RealizedValue, _B_1.RealizedValue, _B_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _3C : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _3C(__GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._C<ParseMode.Realized> _C_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._3_1 = _3_1;
                    this._C_1 = _C_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._3C>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._3C> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._C<ParseMode.Realized> _C_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._3C> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _3_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._3.Create(previousNodeRealizationResult).Realize();if (!_3_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._3C>(false, default, _3_1.RemainingTokens);
}


var _C_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._C.Create(Future.Create(() => _3_1)).Realize();if (!_C_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._3C>(false, default, _C_1.RemainingTokens);
}


var node = new _3C(_3_1.RealizedValue, _C_1.RealizedValue, _C_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _3D : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _3D(__GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._D<ParseMode.Realized> _D_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._3_1 = _3_1;
                    this._D_1 = _D_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._3D>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._3D> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._D<ParseMode.Realized> _D_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._3D> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _3_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._3.Create(previousNodeRealizationResult).Realize();if (!_3_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._3D>(false, default, _3_1.RemainingTokens);
}


var _D_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._D.Create(Future.Create(() => _3_1)).Realize();if (!_D_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._3D>(false, default, _D_1.RemainingTokens);
}


var node = new _3D(_3_1.RealizedValue, _D_1.RealizedValue, _D_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _3E : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _3E(__GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._E<ParseMode.Realized> _E_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._3_1 = _3_1;
                    this._E_1 = _E_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._3E>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._3E> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._E<ParseMode.Realized> _E_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._3E> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _3_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._3.Create(previousNodeRealizationResult).Realize();if (!_3_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._3E>(false, default, _3_1.RemainingTokens);
}


var _E_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._E.Create(Future.Create(() => _3_1)).Realize();if (!_E_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._3E>(false, default, _E_1.RemainingTokens);
}


var node = new _3E(_3_1.RealizedValue, _E_1.RealizedValue, _E_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _3F : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _3F(__GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._F<ParseMode.Realized> _F_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._3_1 = _3_1;
                    this._F_1 = _F_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._3F>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._3F> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._F<ParseMode.Realized> _F_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._3F> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _3_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._3.Create(previousNodeRealizationResult).Realize();if (!_3_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._3F>(false, default, _3_1.RemainingTokens);
}


var _F_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._F.Create(Future.Create(() => _3_1)).Realize();if (!_F_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._3F>(false, default, _F_1.RemainingTokens);
}


var node = new _3F(_3_1.RealizedValue, _F_1.RealizedValue, _F_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _40 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _40(__GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._0<ParseMode.Realized> _0_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._4_1 = _4_1;
                    this._0_1 = _0_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._40>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._40> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<ParseMode.Realized> _0_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._40> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _4_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._4.Create(previousNodeRealizationResult).Realize();if (!_4_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._40>(false, default, _4_1.RemainingTokens);
}


var _0_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._0.Create(Future.Create(() => _4_1)).Realize();if (!_0_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._40>(false, default, _0_1.RemainingTokens);
}


var node = new _40(_4_1.RealizedValue, _0_1.RealizedValue, _0_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _41 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _41(__GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._1<ParseMode.Realized> _1_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._4_1 = _4_1;
                    this._1_1 = _1_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._41>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._41> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<ParseMode.Realized> _1_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._41> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _4_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._4.Create(previousNodeRealizationResult).Realize();if (!_4_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._41>(false, default, _4_1.RemainingTokens);
}


var _1_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._1.Create(Future.Create(() => _4_1)).Realize();if (!_1_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._41>(false, default, _1_1.RemainingTokens);
}


var node = new _41(_4_1.RealizedValue, _1_1.RealizedValue, _1_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _42 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _42(__GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._4_1 = _4_1;
                    this._2_1 = _2_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._42>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._42> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._42> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _4_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._4.Create(previousNodeRealizationResult).Realize();if (!_4_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._42>(false, default, _4_1.RemainingTokens);
}


var _2_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._2.Create(Future.Create(() => _4_1)).Realize();if (!_2_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._42>(false, default, _2_1.RemainingTokens);
}


var node = new _42(_4_1.RealizedValue, _2_1.RealizedValue, _2_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _43 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _43(__GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._4_1 = _4_1;
                    this._3_1 = _3_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._43>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._43> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._43> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _4_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._4.Create(previousNodeRealizationResult).Realize();if (!_4_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._43>(false, default, _4_1.RemainingTokens);
}


var _3_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._3.Create(Future.Create(() => _4_1)).Realize();if (!_3_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._43>(false, default, _3_1.RemainingTokens);
}


var node = new _43(_4_1.RealizedValue, _3_1.RealizedValue, _3_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _44 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _44(__GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_2, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._4_1 = _4_1;
                    this._4_2 = _4_2;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._44>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._44> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_2 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._44> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _4_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._4.Create(previousNodeRealizationResult).Realize();if (!_4_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._44>(false, default, _4_1.RemainingTokens);
}


var _4_2 = __GeneratedPartialV1.Deferred.CstNodes.Inners._4.Create(Future.Create(() => _4_1)).Realize();if (!_4_2.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._44>(false, default, _4_2.RemainingTokens);
}


var node = new _44(_4_1.RealizedValue, _4_2.RealizedValue, _4_2.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _45 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _45(__GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._4_1 = _4_1;
                    this._5_1 = _5_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._45>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._45> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._45> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _4_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._4.Create(previousNodeRealizationResult).Realize();if (!_4_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._45>(false, default, _4_1.RemainingTokens);
}


var _5_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._5.Create(Future.Create(() => _4_1)).Realize();if (!_5_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._45>(false, default, _5_1.RemainingTokens);
}


var node = new _45(_4_1.RealizedValue, _5_1.RealizedValue, _5_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _46 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _46(__GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._4_1 = _4_1;
                    this._6_1 = _6_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._46>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._46> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._46> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _4_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._4.Create(previousNodeRealizationResult).Realize();if (!_4_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._46>(false, default, _4_1.RemainingTokens);
}


var _6_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._6.Create(Future.Create(() => _4_1)).Realize();if (!_6_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._46>(false, default, _6_1.RemainingTokens);
}


var node = new _46(_4_1.RealizedValue, _6_1.RealizedValue, _6_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _47 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _47(__GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._4_1 = _4_1;
                    this._7_1 = _7_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._47>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._47> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._47> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _4_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._4.Create(previousNodeRealizationResult).Realize();if (!_4_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._47>(false, default, _4_1.RemainingTokens);
}


var _7_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._7.Create(Future.Create(() => _4_1)).Realize();if (!_7_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._47>(false, default, _7_1.RemainingTokens);
}


var node = new _47(_4_1.RealizedValue, _7_1.RealizedValue, _7_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _48 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _48(__GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._8<ParseMode.Realized> _8_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._4_1 = _4_1;
                    this._8_1 = _8_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._48>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._48> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._8<ParseMode.Realized> _8_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._48> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _4_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._4.Create(previousNodeRealizationResult).Realize();if (!_4_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._48>(false, default, _4_1.RemainingTokens);
}


var _8_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._8.Create(Future.Create(() => _4_1)).Realize();if (!_8_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._48>(false, default, _8_1.RemainingTokens);
}


var node = new _48(_4_1.RealizedValue, _8_1.RealizedValue, _8_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _49 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _49(__GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._9<ParseMode.Realized> _9_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._4_1 = _4_1;
                    this._9_1 = _9_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._49>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._49> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._9<ParseMode.Realized> _9_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._49> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _4_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._4.Create(previousNodeRealizationResult).Realize();if (!_4_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._49>(false, default, _4_1.RemainingTokens);
}


var _9_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._9.Create(Future.Create(() => _4_1)).Realize();if (!_9_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._49>(false, default, _9_1.RemainingTokens);
}


var node = new _49(_4_1.RealizedValue, _9_1.RealizedValue, _9_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _4A : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _4A(__GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._A<ParseMode.Realized> _A_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._4_1 = _4_1;
                    this._A_1 = _A_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._4A>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._4A> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._A<ParseMode.Realized> _A_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._4A> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _4_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._4.Create(previousNodeRealizationResult).Realize();if (!_4_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._4A>(false, default, _4_1.RemainingTokens);
}


var _A_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._A.Create(Future.Create(() => _4_1)).Realize();if (!_A_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._4A>(false, default, _A_1.RemainingTokens);
}


var node = new _4A(_4_1.RealizedValue, _A_1.RealizedValue, _A_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _4B : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _4B(__GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._B<ParseMode.Realized> _B_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._4_1 = _4_1;
                    this._B_1 = _B_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._4B>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._4B> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._B<ParseMode.Realized> _B_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._4B> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _4_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._4.Create(previousNodeRealizationResult).Realize();if (!_4_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._4B>(false, default, _4_1.RemainingTokens);
}


var _B_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._B.Create(Future.Create(() => _4_1)).Realize();if (!_B_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._4B>(false, default, _B_1.RemainingTokens);
}


var node = new _4B(_4_1.RealizedValue, _B_1.RealizedValue, _B_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _4C : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _4C(__GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._C<ParseMode.Realized> _C_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._4_1 = _4_1;
                    this._C_1 = _C_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._4C>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._4C> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._C<ParseMode.Realized> _C_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._4C> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _4_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._4.Create(previousNodeRealizationResult).Realize();if (!_4_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._4C>(false, default, _4_1.RemainingTokens);
}


var _C_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._C.Create(Future.Create(() => _4_1)).Realize();if (!_C_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._4C>(false, default, _C_1.RemainingTokens);
}


var node = new _4C(_4_1.RealizedValue, _C_1.RealizedValue, _C_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _4D : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _4D(__GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._D<ParseMode.Realized> _D_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._4_1 = _4_1;
                    this._D_1 = _D_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._4D>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._4D> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._D<ParseMode.Realized> _D_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._4D> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _4_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._4.Create(previousNodeRealizationResult).Realize();if (!_4_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._4D>(false, default, _4_1.RemainingTokens);
}


var _D_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._D.Create(Future.Create(() => _4_1)).Realize();if (!_D_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._4D>(false, default, _D_1.RemainingTokens);
}


var node = new _4D(_4_1.RealizedValue, _D_1.RealizedValue, _D_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _4E : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _4E(__GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._E<ParseMode.Realized> _E_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._4_1 = _4_1;
                    this._E_1 = _E_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._4E>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._4E> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._E<ParseMode.Realized> _E_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._4E> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _4_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._4.Create(previousNodeRealizationResult).Realize();if (!_4_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._4E>(false, default, _4_1.RemainingTokens);
}


var _E_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._E.Create(Future.Create(() => _4_1)).Realize();if (!_E_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._4E>(false, default, _E_1.RemainingTokens);
}


var node = new _4E(_4_1.RealizedValue, _E_1.RealizedValue, _E_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _4F : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _4F(__GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._F<ParseMode.Realized> _F_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._4_1 = _4_1;
                    this._F_1 = _F_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._4F>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._4F> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._F<ParseMode.Realized> _F_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._4F> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _4_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._4.Create(previousNodeRealizationResult).Realize();if (!_4_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._4F>(false, default, _4_1.RemainingTokens);
}


var _F_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._F.Create(Future.Create(() => _4_1)).Realize();if (!_F_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._4F>(false, default, _F_1.RemainingTokens);
}


var node = new _4F(_4_1.RealizedValue, _F_1.RealizedValue, _F_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _50 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _50(__GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._0<ParseMode.Realized> _0_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._5_1 = _5_1;
                    this._0_1 = _0_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._50>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._50> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<ParseMode.Realized> _0_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._50> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _5_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._5.Create(previousNodeRealizationResult).Realize();if (!_5_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._50>(false, default, _5_1.RemainingTokens);
}


var _0_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._0.Create(Future.Create(() => _5_1)).Realize();if (!_0_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._50>(false, default, _0_1.RemainingTokens);
}


var node = new _50(_5_1.RealizedValue, _0_1.RealizedValue, _0_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _51 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _51(__GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._1<ParseMode.Realized> _1_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._5_1 = _5_1;
                    this._1_1 = _1_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._51>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._51> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<ParseMode.Realized> _1_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._51> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _5_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._5.Create(previousNodeRealizationResult).Realize();if (!_5_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._51>(false, default, _5_1.RemainingTokens);
}


var _1_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._1.Create(Future.Create(() => _5_1)).Realize();if (!_1_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._51>(false, default, _1_1.RemainingTokens);
}


var node = new _51(_5_1.RealizedValue, _1_1.RealizedValue, _1_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _52 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _52(__GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._5_1 = _5_1;
                    this._2_1 = _2_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._52>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._52> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._52> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _5_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._5.Create(previousNodeRealizationResult).Realize();if (!_5_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._52>(false, default, _5_1.RemainingTokens);
}


var _2_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._2.Create(Future.Create(() => _5_1)).Realize();if (!_2_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._52>(false, default, _2_1.RemainingTokens);
}


var node = new _52(_5_1.RealizedValue, _2_1.RealizedValue, _2_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _53 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _53(__GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._5_1 = _5_1;
                    this._3_1 = _3_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._53>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._53> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._53> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _5_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._5.Create(previousNodeRealizationResult).Realize();if (!_5_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._53>(false, default, _5_1.RemainingTokens);
}


var _3_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._3.Create(Future.Create(() => _5_1)).Realize();if (!_3_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._53>(false, default, _3_1.RemainingTokens);
}


var node = new _53(_5_1.RealizedValue, _3_1.RealizedValue, _3_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _54 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _54(__GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._5_1 = _5_1;
                    this._4_1 = _4_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._54>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._54> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._54> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _5_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._5.Create(previousNodeRealizationResult).Realize();if (!_5_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._54>(false, default, _5_1.RemainingTokens);
}


var _4_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._4.Create(Future.Create(() => _5_1)).Realize();if (!_4_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._54>(false, default, _4_1.RemainingTokens);
}


var node = new _54(_5_1.RealizedValue, _4_1.RealizedValue, _4_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _55 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _55(__GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_2, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._5_1 = _5_1;
                    this._5_2 = _5_2;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._55>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._55> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_2 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._55> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _5_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._5.Create(previousNodeRealizationResult).Realize();if (!_5_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._55>(false, default, _5_1.RemainingTokens);
}


var _5_2 = __GeneratedPartialV1.Deferred.CstNodes.Inners._5.Create(Future.Create(() => _5_1)).Realize();if (!_5_2.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._55>(false, default, _5_2.RemainingTokens);
}


var node = new _55(_5_1.RealizedValue, _5_2.RealizedValue, _5_2.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _56 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _56(__GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._5_1 = _5_1;
                    this._6_1 = _6_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._56>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._56> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._56> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _5_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._5.Create(previousNodeRealizationResult).Realize();if (!_5_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._56>(false, default, _5_1.RemainingTokens);
}


var _6_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._6.Create(Future.Create(() => _5_1)).Realize();if (!_6_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._56>(false, default, _6_1.RemainingTokens);
}


var node = new _56(_5_1.RealizedValue, _6_1.RealizedValue, _6_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _57 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _57(__GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._5_1 = _5_1;
                    this._7_1 = _7_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._57>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._57> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._57> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _5_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._5.Create(previousNodeRealizationResult).Realize();if (!_5_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._57>(false, default, _5_1.RemainingTokens);
}


var _7_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._7.Create(Future.Create(() => _5_1)).Realize();if (!_7_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._57>(false, default, _7_1.RemainingTokens);
}


var node = new _57(_5_1.RealizedValue, _7_1.RealizedValue, _7_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _58 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _58(__GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._8<ParseMode.Realized> _8_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._5_1 = _5_1;
                    this._8_1 = _8_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._58>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._58> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._8<ParseMode.Realized> _8_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._58> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _5_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._5.Create(previousNodeRealizationResult).Realize();if (!_5_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._58>(false, default, _5_1.RemainingTokens);
}


var _8_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._8.Create(Future.Create(() => _5_1)).Realize();if (!_8_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._58>(false, default, _8_1.RemainingTokens);
}


var node = new _58(_5_1.RealizedValue, _8_1.RealizedValue, _8_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _59 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _59(__GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._9<ParseMode.Realized> _9_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._5_1 = _5_1;
                    this._9_1 = _9_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._59>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._59> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._9<ParseMode.Realized> _9_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._59> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _5_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._5.Create(previousNodeRealizationResult).Realize();if (!_5_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._59>(false, default, _5_1.RemainingTokens);
}


var _9_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._9.Create(Future.Create(() => _5_1)).Realize();if (!_9_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._59>(false, default, _9_1.RemainingTokens);
}


var node = new _59(_5_1.RealizedValue, _9_1.RealizedValue, _9_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _5A : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _5A(__GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._A<ParseMode.Realized> _A_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._5_1 = _5_1;
                    this._A_1 = _A_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._5A>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._5A> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._A<ParseMode.Realized> _A_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._5A> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _5_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._5.Create(previousNodeRealizationResult).Realize();if (!_5_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._5A>(false, default, _5_1.RemainingTokens);
}


var _A_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._A.Create(Future.Create(() => _5_1)).Realize();if (!_A_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._5A>(false, default, _A_1.RemainingTokens);
}


var node = new _5A(_5_1.RealizedValue, _A_1.RealizedValue, _A_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _5B : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _5B(__GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._B<ParseMode.Realized> _B_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._5_1 = _5_1;
                    this._B_1 = _B_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._5B>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._5B> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._B<ParseMode.Realized> _B_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._5B> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _5_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._5.Create(previousNodeRealizationResult).Realize();if (!_5_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._5B>(false, default, _5_1.RemainingTokens);
}


var _B_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._B.Create(Future.Create(() => _5_1)).Realize();if (!_B_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._5B>(false, default, _B_1.RemainingTokens);
}


var node = new _5B(_5_1.RealizedValue, _B_1.RealizedValue, _B_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _5C : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _5C(__GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._C<ParseMode.Realized> _C_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._5_1 = _5_1;
                    this._C_1 = _C_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._5C>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._5C> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._C<ParseMode.Realized> _C_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._5C> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _5_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._5.Create(previousNodeRealizationResult).Realize();if (!_5_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._5C>(false, default, _5_1.RemainingTokens);
}


var _C_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._C.Create(Future.Create(() => _5_1)).Realize();if (!_C_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._5C>(false, default, _C_1.RemainingTokens);
}


var node = new _5C(_5_1.RealizedValue, _C_1.RealizedValue, _C_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _5D : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _5D(__GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._D<ParseMode.Realized> _D_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._5_1 = _5_1;
                    this._D_1 = _D_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._5D>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._5D> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._D<ParseMode.Realized> _D_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._5D> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _5_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._5.Create(previousNodeRealizationResult).Realize();if (!_5_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._5D>(false, default, _5_1.RemainingTokens);
}


var _D_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._D.Create(Future.Create(() => _5_1)).Realize();if (!_D_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._5D>(false, default, _D_1.RemainingTokens);
}


var node = new _5D(_5_1.RealizedValue, _D_1.RealizedValue, _D_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _5E : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _5E(__GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._E<ParseMode.Realized> _E_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._5_1 = _5_1;
                    this._E_1 = _E_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._5E>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._5E> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._E<ParseMode.Realized> _E_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._5E> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _5_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._5.Create(previousNodeRealizationResult).Realize();if (!_5_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._5E>(false, default, _5_1.RemainingTokens);
}


var _E_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._E.Create(Future.Create(() => _5_1)).Realize();if (!_E_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._5E>(false, default, _E_1.RemainingTokens);
}


var node = new _5E(_5_1.RealizedValue, _E_1.RealizedValue, _E_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _5F : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _5F(__GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._F<ParseMode.Realized> _F_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._5_1 = _5_1;
                    this._F_1 = _F_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._5F>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._5F> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._F<ParseMode.Realized> _F_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._5F> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _5_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._5.Create(previousNodeRealizationResult).Realize();if (!_5_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._5F>(false, default, _5_1.RemainingTokens);
}


var _F_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._F.Create(Future.Create(() => _5_1)).Realize();if (!_F_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._5F>(false, default, _F_1.RemainingTokens);
}


var node = new _5F(_5_1.RealizedValue, _F_1.RealizedValue, _F_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _60 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _60(__GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._0<ParseMode.Realized> _0_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._6_1 = _6_1;
                    this._0_1 = _0_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._60>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._60> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<ParseMode.Realized> _0_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._60> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _6_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._6.Create(previousNodeRealizationResult).Realize();if (!_6_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._60>(false, default, _6_1.RemainingTokens);
}


var _0_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._0.Create(Future.Create(() => _6_1)).Realize();if (!_0_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._60>(false, default, _0_1.RemainingTokens);
}


var node = new _60(_6_1.RealizedValue, _0_1.RealizedValue, _0_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _61 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _61(__GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._1<ParseMode.Realized> _1_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._6_1 = _6_1;
                    this._1_1 = _1_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._61>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._61> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<ParseMode.Realized> _1_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._61> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _6_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._6.Create(previousNodeRealizationResult).Realize();if (!_6_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._61>(false, default, _6_1.RemainingTokens);
}


var _1_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._1.Create(Future.Create(() => _6_1)).Realize();if (!_1_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._61>(false, default, _1_1.RemainingTokens);
}


var node = new _61(_6_1.RealizedValue, _1_1.RealizedValue, _1_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _62 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _62(__GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._6_1 = _6_1;
                    this._2_1 = _2_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._62>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._62> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._62> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _6_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._6.Create(previousNodeRealizationResult).Realize();if (!_6_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._62>(false, default, _6_1.RemainingTokens);
}


var _2_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._2.Create(Future.Create(() => _6_1)).Realize();if (!_2_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._62>(false, default, _2_1.RemainingTokens);
}


var node = new _62(_6_1.RealizedValue, _2_1.RealizedValue, _2_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _63 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _63(__GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._6_1 = _6_1;
                    this._3_1 = _3_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._63>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._63> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._63> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _6_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._6.Create(previousNodeRealizationResult).Realize();if (!_6_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._63>(false, default, _6_1.RemainingTokens);
}


var _3_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._3.Create(Future.Create(() => _6_1)).Realize();if (!_3_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._63>(false, default, _3_1.RemainingTokens);
}


var node = new _63(_6_1.RealizedValue, _3_1.RealizedValue, _3_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _64 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _64(__GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._6_1 = _6_1;
                    this._4_1 = _4_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._64>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._64> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._64> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _6_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._6.Create(previousNodeRealizationResult).Realize();if (!_6_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._64>(false, default, _6_1.RemainingTokens);
}


var _4_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._4.Create(Future.Create(() => _6_1)).Realize();if (!_4_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._64>(false, default, _4_1.RemainingTokens);
}


var node = new _64(_6_1.RealizedValue, _4_1.RealizedValue, _4_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _65 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _65(__GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._6_1 = _6_1;
                    this._5_1 = _5_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._65>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._65> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._65> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _6_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._6.Create(previousNodeRealizationResult).Realize();if (!_6_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._65>(false, default, _6_1.RemainingTokens);
}


var _5_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._5.Create(Future.Create(() => _6_1)).Realize();if (!_5_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._65>(false, default, _5_1.RemainingTokens);
}


var node = new _65(_6_1.RealizedValue, _5_1.RealizedValue, _5_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _66 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _66(__GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_2, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._6_1 = _6_1;
                    this._6_2 = _6_2;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._66>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._66> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_2 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._66> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _6_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._6.Create(previousNodeRealizationResult).Realize();if (!_6_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._66>(false, default, _6_1.RemainingTokens);
}


var _6_2 = __GeneratedPartialV1.Deferred.CstNodes.Inners._6.Create(Future.Create(() => _6_1)).Realize();if (!_6_2.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._66>(false, default, _6_2.RemainingTokens);
}


var node = new _66(_6_1.RealizedValue, _6_2.RealizedValue, _6_2.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _67 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _67(__GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._6_1 = _6_1;
                    this._7_1 = _7_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._67>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._67> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._67> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _6_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._6.Create(previousNodeRealizationResult).Realize();if (!_6_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._67>(false, default, _6_1.RemainingTokens);
}


var _7_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._7.Create(Future.Create(() => _6_1)).Realize();if (!_7_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._67>(false, default, _7_1.RemainingTokens);
}


var node = new _67(_6_1.RealizedValue, _7_1.RealizedValue, _7_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _68 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _68(__GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._8<ParseMode.Realized> _8_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._6_1 = _6_1;
                    this._8_1 = _8_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._68>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._68> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._8<ParseMode.Realized> _8_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._68> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _6_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._6.Create(previousNodeRealizationResult).Realize();if (!_6_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._68>(false, default, _6_1.RemainingTokens);
}


var _8_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._8.Create(Future.Create(() => _6_1)).Realize();if (!_8_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._68>(false, default, _8_1.RemainingTokens);
}


var node = new _68(_6_1.RealizedValue, _8_1.RealizedValue, _8_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _69 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _69(__GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._9<ParseMode.Realized> _9_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._6_1 = _6_1;
                    this._9_1 = _9_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._69>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._69> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._9<ParseMode.Realized> _9_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._69> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _6_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._6.Create(previousNodeRealizationResult).Realize();if (!_6_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._69>(false, default, _6_1.RemainingTokens);
}


var _9_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._9.Create(Future.Create(() => _6_1)).Realize();if (!_9_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._69>(false, default, _9_1.RemainingTokens);
}


var node = new _69(_6_1.RealizedValue, _9_1.RealizedValue, _9_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _6A : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _6A(__GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._A<ParseMode.Realized> _A_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._6_1 = _6_1;
                    this._A_1 = _A_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._6A>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._6A> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._A<ParseMode.Realized> _A_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._6A> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _6_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._6.Create(previousNodeRealizationResult).Realize();if (!_6_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._6A>(false, default, _6_1.RemainingTokens);
}


var _A_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._A.Create(Future.Create(() => _6_1)).Realize();if (!_A_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._6A>(false, default, _A_1.RemainingTokens);
}


var node = new _6A(_6_1.RealizedValue, _A_1.RealizedValue, _A_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _6B : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _6B(__GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._B<ParseMode.Realized> _B_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._6_1 = _6_1;
                    this._B_1 = _B_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._6B>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._6B> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._B<ParseMode.Realized> _B_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._6B> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _6_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._6.Create(previousNodeRealizationResult).Realize();if (!_6_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._6B>(false, default, _6_1.RemainingTokens);
}


var _B_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._B.Create(Future.Create(() => _6_1)).Realize();if (!_B_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._6B>(false, default, _B_1.RemainingTokens);
}


var node = new _6B(_6_1.RealizedValue, _B_1.RealizedValue, _B_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _6C : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _6C(__GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._C<ParseMode.Realized> _C_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._6_1 = _6_1;
                    this._C_1 = _C_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._6C>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._6C> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._C<ParseMode.Realized> _C_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._6C> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _6_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._6.Create(previousNodeRealizationResult).Realize();if (!_6_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._6C>(false, default, _6_1.RemainingTokens);
}


var _C_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._C.Create(Future.Create(() => _6_1)).Realize();if (!_C_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._6C>(false, default, _C_1.RemainingTokens);
}


var node = new _6C(_6_1.RealizedValue, _C_1.RealizedValue, _C_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _6D : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _6D(__GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._D<ParseMode.Realized> _D_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._6_1 = _6_1;
                    this._D_1 = _D_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._6D>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._6D> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._D<ParseMode.Realized> _D_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._6D> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _6_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._6.Create(previousNodeRealizationResult).Realize();if (!_6_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._6D>(false, default, _6_1.RemainingTokens);
}


var _D_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._D.Create(Future.Create(() => _6_1)).Realize();if (!_D_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._6D>(false, default, _D_1.RemainingTokens);
}


var node = new _6D(_6_1.RealizedValue, _D_1.RealizedValue, _D_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _6E : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _6E(__GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._E<ParseMode.Realized> _E_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._6_1 = _6_1;
                    this._E_1 = _E_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._6E>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._6E> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._E<ParseMode.Realized> _E_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._6E> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _6_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._6.Create(previousNodeRealizationResult).Realize();if (!_6_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._6E>(false, default, _6_1.RemainingTokens);
}


var _E_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._E.Create(Future.Create(() => _6_1)).Realize();if (!_E_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._6E>(false, default, _E_1.RemainingTokens);
}


var node = new _6E(_6_1.RealizedValue, _E_1.RealizedValue, _E_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _6F : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _6F(__GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._F<ParseMode.Realized> _F_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._6_1 = _6_1;
                    this._F_1 = _F_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._6F>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._6F> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._F<ParseMode.Realized> _F_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._6F> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _6_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._6.Create(previousNodeRealizationResult).Realize();if (!_6_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._6F>(false, default, _6_1.RemainingTokens);
}


var _F_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._F.Create(Future.Create(() => _6_1)).Realize();if (!_F_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._6F>(false, default, _F_1.RemainingTokens);
}


var node = new _6F(_6_1.RealizedValue, _F_1.RealizedValue, _F_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _70 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _70(__GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._0<ParseMode.Realized> _0_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._7_1 = _7_1;
                    this._0_1 = _0_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._70>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._70> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<ParseMode.Realized> _0_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._70> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _7_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._7.Create(previousNodeRealizationResult).Realize();if (!_7_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._70>(false, default, _7_1.RemainingTokens);
}


var _0_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._0.Create(Future.Create(() => _7_1)).Realize();if (!_0_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._70>(false, default, _0_1.RemainingTokens);
}


var node = new _70(_7_1.RealizedValue, _0_1.RealizedValue, _0_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _71 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _71(__GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._1<ParseMode.Realized> _1_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._7_1 = _7_1;
                    this._1_1 = _1_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._71>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._71> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<ParseMode.Realized> _1_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._71> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _7_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._7.Create(previousNodeRealizationResult).Realize();if (!_7_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._71>(false, default, _7_1.RemainingTokens);
}


var _1_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._1.Create(Future.Create(() => _7_1)).Realize();if (!_1_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._71>(false, default, _1_1.RemainingTokens);
}


var node = new _71(_7_1.RealizedValue, _1_1.RealizedValue, _1_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _72 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _72(__GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._7_1 = _7_1;
                    this._2_1 = _2_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._72>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._72> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._72> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _7_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._7.Create(previousNodeRealizationResult).Realize();if (!_7_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._72>(false, default, _7_1.RemainingTokens);
}


var _2_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._2.Create(Future.Create(() => _7_1)).Realize();if (!_2_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._72>(false, default, _2_1.RemainingTokens);
}


var node = new _72(_7_1.RealizedValue, _2_1.RealizedValue, _2_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _73 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _73(__GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._7_1 = _7_1;
                    this._3_1 = _3_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._73>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._73> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._73> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _7_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._7.Create(previousNodeRealizationResult).Realize();if (!_7_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._73>(false, default, _7_1.RemainingTokens);
}


var _3_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._3.Create(Future.Create(() => _7_1)).Realize();if (!_3_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._73>(false, default, _3_1.RemainingTokens);
}


var node = new _73(_7_1.RealizedValue, _3_1.RealizedValue, _3_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _74 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _74(__GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._7_1 = _7_1;
                    this._4_1 = _4_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._74>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._74> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._74> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _7_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._7.Create(previousNodeRealizationResult).Realize();if (!_7_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._74>(false, default, _7_1.RemainingTokens);
}


var _4_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._4.Create(Future.Create(() => _7_1)).Realize();if (!_4_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._74>(false, default, _4_1.RemainingTokens);
}


var node = new _74(_7_1.RealizedValue, _4_1.RealizedValue, _4_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _75 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _75(__GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._7_1 = _7_1;
                    this._5_1 = _5_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._75>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._75> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._75> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _7_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._7.Create(previousNodeRealizationResult).Realize();if (!_7_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._75>(false, default, _7_1.RemainingTokens);
}


var _5_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._5.Create(Future.Create(() => _7_1)).Realize();if (!_5_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._75>(false, default, _5_1.RemainingTokens);
}


var node = new _75(_7_1.RealizedValue, _5_1.RealizedValue, _5_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _76 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _76(__GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._7_1 = _7_1;
                    this._6_1 = _6_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._76>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._76> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._76> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _7_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._7.Create(previousNodeRealizationResult).Realize();if (!_7_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._76>(false, default, _7_1.RemainingTokens);
}


var _6_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._6.Create(Future.Create(() => _7_1)).Realize();if (!_6_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._76>(false, default, _6_1.RemainingTokens);
}


var node = new _76(_7_1.RealizedValue, _6_1.RealizedValue, _6_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _77 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _77(__GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_2, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._7_1 = _7_1;
                    this._7_2 = _7_2;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._77>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._77> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_2 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._77> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _7_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._7.Create(previousNodeRealizationResult).Realize();if (!_7_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._77>(false, default, _7_1.RemainingTokens);
}


var _7_2 = __GeneratedPartialV1.Deferred.CstNodes.Inners._7.Create(Future.Create(() => _7_1)).Realize();if (!_7_2.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._77>(false, default, _7_2.RemainingTokens);
}


var node = new _77(_7_1.RealizedValue, _7_2.RealizedValue, _7_2.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _78 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _78(__GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._8<ParseMode.Realized> _8_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._7_1 = _7_1;
                    this._8_1 = _8_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._78>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._78> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._8<ParseMode.Realized> _8_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._78> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _7_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._7.Create(previousNodeRealizationResult).Realize();if (!_7_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._78>(false, default, _7_1.RemainingTokens);
}


var _8_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._8.Create(Future.Create(() => _7_1)).Realize();if (!_8_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._78>(false, default, _8_1.RemainingTokens);
}


var node = new _78(_7_1.RealizedValue, _8_1.RealizedValue, _8_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _79 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _79(__GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._9<ParseMode.Realized> _9_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._7_1 = _7_1;
                    this._9_1 = _9_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._79>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._79> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._9<ParseMode.Realized> _9_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._79> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _7_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._7.Create(previousNodeRealizationResult).Realize();if (!_7_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._79>(false, default, _7_1.RemainingTokens);
}


var _9_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._9.Create(Future.Create(() => _7_1)).Realize();if (!_9_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._79>(false, default, _9_1.RemainingTokens);
}


var node = new _79(_7_1.RealizedValue, _9_1.RealizedValue, _9_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _7A : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _7A(__GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._A<ParseMode.Realized> _A_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._7_1 = _7_1;
                    this._A_1 = _A_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._7A>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._7A> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._A<ParseMode.Realized> _A_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._7A> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _7_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._7.Create(previousNodeRealizationResult).Realize();if (!_7_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._7A>(false, default, _7_1.RemainingTokens);
}


var _A_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._A.Create(Future.Create(() => _7_1)).Realize();if (!_A_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._7A>(false, default, _A_1.RemainingTokens);
}


var node = new _7A(_7_1.RealizedValue, _A_1.RealizedValue, _A_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _7B : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _7B(__GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._B<ParseMode.Realized> _B_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._7_1 = _7_1;
                    this._B_1 = _B_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._7B>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._7B> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._B<ParseMode.Realized> _B_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._7B> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _7_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._7.Create(previousNodeRealizationResult).Realize();if (!_7_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._7B>(false, default, _7_1.RemainingTokens);
}


var _B_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._B.Create(Future.Create(() => _7_1)).Realize();if (!_B_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._7B>(false, default, _B_1.RemainingTokens);
}


var node = new _7B(_7_1.RealizedValue, _B_1.RealizedValue, _B_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _7C : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _7C(__GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._C<ParseMode.Realized> _C_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._7_1 = _7_1;
                    this._C_1 = _C_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._7C>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._7C> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._C<ParseMode.Realized> _C_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._7C> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _7_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._7.Create(previousNodeRealizationResult).Realize();if (!_7_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._7C>(false, default, _7_1.RemainingTokens);
}


var _C_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._C.Create(Future.Create(() => _7_1)).Realize();if (!_C_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._7C>(false, default, _C_1.RemainingTokens);
}


var node = new _7C(_7_1.RealizedValue, _C_1.RealizedValue, _C_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _7D : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _7D(__GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._D<ParseMode.Realized> _D_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._7_1 = _7_1;
                    this._D_1 = _D_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._7D>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._7D> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._D<ParseMode.Realized> _D_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._7D> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _7_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._7.Create(previousNodeRealizationResult).Realize();if (!_7_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._7D>(false, default, _7_1.RemainingTokens);
}


var _D_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._D.Create(Future.Create(() => _7_1)).Realize();if (!_D_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._7D>(false, default, _D_1.RemainingTokens);
}


var node = new _7D(_7_1.RealizedValue, _D_1.RealizedValue, _D_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _7E : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _7E(__GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._E<ParseMode.Realized> _E_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._7_1 = _7_1;
                    this._E_1 = _E_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._7E>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._7E> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._E<ParseMode.Realized> _E_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._7E> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _7_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._7.Create(previousNodeRealizationResult).Realize();if (!_7_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._7E>(false, default, _7_1.RemainingTokens);
}


var _E_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._E.Create(Future.Create(() => _7_1)).Realize();if (!_E_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._7E>(false, default, _E_1.RemainingTokens);
}


var node = new _7E(_7_1.RealizedValue, _E_1.RealizedValue, _E_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _7F : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _7F(__GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._F<ParseMode.Realized> _F_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._7_1 = _7_1;
                    this._F_1 = _F_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._7F>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._7F> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._F<ParseMode.Realized> _F_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._7F> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _7_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._7.Create(previousNodeRealizationResult).Realize();if (!_7_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._7F>(false, default, _7_1.RemainingTokens);
}


var _F_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._F.Create(Future.Create(() => _7_1)).Realize();if (!_F_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._7F>(false, default, _F_1.RemainingTokens);
}


var node = new _7F(_7_1.RealizedValue, _F_1.RealizedValue, _F_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _80 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _80(__GeneratedPartialV1.Deferred.CstNodes.Inners._8<ParseMode.Realized> _8_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._0<ParseMode.Realized> _0_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._8_1 = _8_1;
                    this._0_1 = _0_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._80>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._80> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._8<ParseMode.Realized> _8_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<ParseMode.Realized> _0_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._80> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _8_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._8.Create(previousNodeRealizationResult).Realize();if (!_8_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._80>(false, default, _8_1.RemainingTokens);
}


var _0_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._0.Create(Future.Create(() => _8_1)).Realize();if (!_0_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._80>(false, default, _0_1.RemainingTokens);
}


var node = new _80(_8_1.RealizedValue, _0_1.RealizedValue, _0_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _81 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _81(__GeneratedPartialV1.Deferred.CstNodes.Inners._8<ParseMode.Realized> _8_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._1<ParseMode.Realized> _1_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._8_1 = _8_1;
                    this._1_1 = _1_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._81>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._81> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._8<ParseMode.Realized> _8_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<ParseMode.Realized> _1_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._81> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _8_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._8.Create(previousNodeRealizationResult).Realize();if (!_8_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._81>(false, default, _8_1.RemainingTokens);
}


var _1_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._1.Create(Future.Create(() => _8_1)).Realize();if (!_1_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._81>(false, default, _1_1.RemainingTokens);
}


var node = new _81(_8_1.RealizedValue, _1_1.RealizedValue, _1_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _82 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _82(__GeneratedPartialV1.Deferred.CstNodes.Inners._8<ParseMode.Realized> _8_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._8_1 = _8_1;
                    this._2_1 = _2_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._82>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._82> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._8<ParseMode.Realized> _8_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._82> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _8_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._8.Create(previousNodeRealizationResult).Realize();if (!_8_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._82>(false, default, _8_1.RemainingTokens);
}


var _2_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._2.Create(Future.Create(() => _8_1)).Realize();if (!_2_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._82>(false, default, _2_1.RemainingTokens);
}


var node = new _82(_8_1.RealizedValue, _2_1.RealizedValue, _2_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _83 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _83(__GeneratedPartialV1.Deferred.CstNodes.Inners._8<ParseMode.Realized> _8_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._8_1 = _8_1;
                    this._3_1 = _3_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._83>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._83> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._8<ParseMode.Realized> _8_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._83> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _8_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._8.Create(previousNodeRealizationResult).Realize();if (!_8_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._83>(false, default, _8_1.RemainingTokens);
}


var _3_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._3.Create(Future.Create(() => _8_1)).Realize();if (!_3_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._83>(false, default, _3_1.RemainingTokens);
}


var node = new _83(_8_1.RealizedValue, _3_1.RealizedValue, _3_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _84 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _84(__GeneratedPartialV1.Deferred.CstNodes.Inners._8<ParseMode.Realized> _8_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._8_1 = _8_1;
                    this._4_1 = _4_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._84>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._84> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._8<ParseMode.Realized> _8_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._84> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _8_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._8.Create(previousNodeRealizationResult).Realize();if (!_8_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._84>(false, default, _8_1.RemainingTokens);
}


var _4_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._4.Create(Future.Create(() => _8_1)).Realize();if (!_4_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._84>(false, default, _4_1.RemainingTokens);
}


var node = new _84(_8_1.RealizedValue, _4_1.RealizedValue, _4_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _85 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _85(__GeneratedPartialV1.Deferred.CstNodes.Inners._8<ParseMode.Realized> _8_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._8_1 = _8_1;
                    this._5_1 = _5_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._85>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._85> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._8<ParseMode.Realized> _8_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._85> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _8_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._8.Create(previousNodeRealizationResult).Realize();if (!_8_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._85>(false, default, _8_1.RemainingTokens);
}


var _5_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._5.Create(Future.Create(() => _8_1)).Realize();if (!_5_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._85>(false, default, _5_1.RemainingTokens);
}


var node = new _85(_8_1.RealizedValue, _5_1.RealizedValue, _5_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _86 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _86(__GeneratedPartialV1.Deferred.CstNodes.Inners._8<ParseMode.Realized> _8_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._8_1 = _8_1;
                    this._6_1 = _6_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._86>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._86> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._8<ParseMode.Realized> _8_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._86> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _8_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._8.Create(previousNodeRealizationResult).Realize();if (!_8_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._86>(false, default, _8_1.RemainingTokens);
}


var _6_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._6.Create(Future.Create(() => _8_1)).Realize();if (!_6_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._86>(false, default, _6_1.RemainingTokens);
}


var node = new _86(_8_1.RealizedValue, _6_1.RealizedValue, _6_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _87 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _87(__GeneratedPartialV1.Deferred.CstNodes.Inners._8<ParseMode.Realized> _8_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._8_1 = _8_1;
                    this._7_1 = _7_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._87>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._87> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._8<ParseMode.Realized> _8_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._87> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _8_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._8.Create(previousNodeRealizationResult).Realize();if (!_8_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._87>(false, default, _8_1.RemainingTokens);
}


var _7_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._7.Create(Future.Create(() => _8_1)).Realize();if (!_7_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._87>(false, default, _7_1.RemainingTokens);
}


var node = new _87(_8_1.RealizedValue, _7_1.RealizedValue, _7_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _88 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _88(__GeneratedPartialV1.Deferred.CstNodes.Inners._8<ParseMode.Realized> _8_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._8<ParseMode.Realized> _8_2, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._8_1 = _8_1;
                    this._8_2 = _8_2;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._88>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._88> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._8<ParseMode.Realized> _8_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._8<ParseMode.Realized> _8_2 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._88> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _8_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._8.Create(previousNodeRealizationResult).Realize();if (!_8_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._88>(false, default, _8_1.RemainingTokens);
}


var _8_2 = __GeneratedPartialV1.Deferred.CstNodes.Inners._8.Create(Future.Create(() => _8_1)).Realize();if (!_8_2.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._88>(false, default, _8_2.RemainingTokens);
}


var node = new _88(_8_1.RealizedValue, _8_2.RealizedValue, _8_2.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _89 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _89(__GeneratedPartialV1.Deferred.CstNodes.Inners._8<ParseMode.Realized> _8_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._9<ParseMode.Realized> _9_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._8_1 = _8_1;
                    this._9_1 = _9_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._89>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._89> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._8<ParseMode.Realized> _8_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._9<ParseMode.Realized> _9_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._89> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _8_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._8.Create(previousNodeRealizationResult).Realize();if (!_8_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._89>(false, default, _8_1.RemainingTokens);
}


var _9_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._9.Create(Future.Create(() => _8_1)).Realize();if (!_9_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._89>(false, default, _9_1.RemainingTokens);
}


var node = new _89(_8_1.RealizedValue, _9_1.RealizedValue, _9_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _8A : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _8A(__GeneratedPartialV1.Deferred.CstNodes.Inners._8<ParseMode.Realized> _8_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._A<ParseMode.Realized> _A_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._8_1 = _8_1;
                    this._A_1 = _A_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._8A>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._8A> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._8<ParseMode.Realized> _8_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._A<ParseMode.Realized> _A_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._8A> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _8_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._8.Create(previousNodeRealizationResult).Realize();if (!_8_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._8A>(false, default, _8_1.RemainingTokens);
}


var _A_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._A.Create(Future.Create(() => _8_1)).Realize();if (!_A_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._8A>(false, default, _A_1.RemainingTokens);
}


var node = new _8A(_8_1.RealizedValue, _A_1.RealizedValue, _A_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _8B : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _8B(__GeneratedPartialV1.Deferred.CstNodes.Inners._8<ParseMode.Realized> _8_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._B<ParseMode.Realized> _B_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._8_1 = _8_1;
                    this._B_1 = _B_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._8B>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._8B> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._8<ParseMode.Realized> _8_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._B<ParseMode.Realized> _B_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._8B> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _8_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._8.Create(previousNodeRealizationResult).Realize();if (!_8_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._8B>(false, default, _8_1.RemainingTokens);
}


var _B_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._B.Create(Future.Create(() => _8_1)).Realize();if (!_B_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._8B>(false, default, _B_1.RemainingTokens);
}


var node = new _8B(_8_1.RealizedValue, _B_1.RealizedValue, _B_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _8C : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _8C(__GeneratedPartialV1.Deferred.CstNodes.Inners._8<ParseMode.Realized> _8_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._C<ParseMode.Realized> _C_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._8_1 = _8_1;
                    this._C_1 = _C_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._8C>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._8C> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._8<ParseMode.Realized> _8_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._C<ParseMode.Realized> _C_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._8C> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _8_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._8.Create(previousNodeRealizationResult).Realize();if (!_8_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._8C>(false, default, _8_1.RemainingTokens);
}


var _C_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._C.Create(Future.Create(() => _8_1)).Realize();if (!_C_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._8C>(false, default, _C_1.RemainingTokens);
}


var node = new _8C(_8_1.RealizedValue, _C_1.RealizedValue, _C_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _8D : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _8D(__GeneratedPartialV1.Deferred.CstNodes.Inners._8<ParseMode.Realized> _8_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._D<ParseMode.Realized> _D_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._8_1 = _8_1;
                    this._D_1 = _D_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._8D>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._8D> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._8<ParseMode.Realized> _8_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._D<ParseMode.Realized> _D_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._8D> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _8_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._8.Create(previousNodeRealizationResult).Realize();if (!_8_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._8D>(false, default, _8_1.RemainingTokens);
}


var _D_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._D.Create(Future.Create(() => _8_1)).Realize();if (!_D_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._8D>(false, default, _D_1.RemainingTokens);
}


var node = new _8D(_8_1.RealizedValue, _D_1.RealizedValue, _D_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _8E : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _8E(__GeneratedPartialV1.Deferred.CstNodes.Inners._8<ParseMode.Realized> _8_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._E<ParseMode.Realized> _E_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._8_1 = _8_1;
                    this._E_1 = _E_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._8E>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._8E> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._8<ParseMode.Realized> _8_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._E<ParseMode.Realized> _E_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._8E> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _8_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._8.Create(previousNodeRealizationResult).Realize();if (!_8_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._8E>(false, default, _8_1.RemainingTokens);
}


var _E_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._E.Create(Future.Create(() => _8_1)).Realize();if (!_E_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._8E>(false, default, _E_1.RemainingTokens);
}


var node = new _8E(_8_1.RealizedValue, _E_1.RealizedValue, _E_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _8F : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _8F(__GeneratedPartialV1.Deferred.CstNodes.Inners._8<ParseMode.Realized> _8_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._F<ParseMode.Realized> _F_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._8_1 = _8_1;
                    this._F_1 = _F_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._8F>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._8F> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._8<ParseMode.Realized> _8_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._F<ParseMode.Realized> _F_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._8F> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _8_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._8.Create(previousNodeRealizationResult).Realize();if (!_8_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._8F>(false, default, _8_1.RemainingTokens);
}


var _F_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._F.Create(Future.Create(() => _8_1)).Realize();if (!_F_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._8F>(false, default, _F_1.RemainingTokens);
}


var node = new _8F(_8_1.RealizedValue, _F_1.RealizedValue, _F_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _90 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _90(__GeneratedPartialV1.Deferred.CstNodes.Inners._9<ParseMode.Realized> _9_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._0<ParseMode.Realized> _0_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._9_1 = _9_1;
                    this._0_1 = _0_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._90>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._90> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._9<ParseMode.Realized> _9_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<ParseMode.Realized> _0_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._90> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _9_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._9.Create(previousNodeRealizationResult).Realize();if (!_9_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._90>(false, default, _9_1.RemainingTokens);
}


var _0_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._0.Create(Future.Create(() => _9_1)).Realize();if (!_0_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._90>(false, default, _0_1.RemainingTokens);
}


var node = new _90(_9_1.RealizedValue, _0_1.RealizedValue, _0_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _91 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _91(__GeneratedPartialV1.Deferred.CstNodes.Inners._9<ParseMode.Realized> _9_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._1<ParseMode.Realized> _1_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._9_1 = _9_1;
                    this._1_1 = _1_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._91>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._91> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._9<ParseMode.Realized> _9_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<ParseMode.Realized> _1_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._91> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _9_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._9.Create(previousNodeRealizationResult).Realize();if (!_9_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._91>(false, default, _9_1.RemainingTokens);
}


var _1_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._1.Create(Future.Create(() => _9_1)).Realize();if (!_1_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._91>(false, default, _1_1.RemainingTokens);
}


var node = new _91(_9_1.RealizedValue, _1_1.RealizedValue, _1_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _92 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _92(__GeneratedPartialV1.Deferred.CstNodes.Inners._9<ParseMode.Realized> _9_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._9_1 = _9_1;
                    this._2_1 = _2_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._92>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._92> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._9<ParseMode.Realized> _9_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._92> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _9_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._9.Create(previousNodeRealizationResult).Realize();if (!_9_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._92>(false, default, _9_1.RemainingTokens);
}


var _2_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._2.Create(Future.Create(() => _9_1)).Realize();if (!_2_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._92>(false, default, _2_1.RemainingTokens);
}


var node = new _92(_9_1.RealizedValue, _2_1.RealizedValue, _2_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _93 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _93(__GeneratedPartialV1.Deferred.CstNodes.Inners._9<ParseMode.Realized> _9_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._9_1 = _9_1;
                    this._3_1 = _3_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._93>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._93> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._9<ParseMode.Realized> _9_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._93> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _9_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._9.Create(previousNodeRealizationResult).Realize();if (!_9_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._93>(false, default, _9_1.RemainingTokens);
}


var _3_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._3.Create(Future.Create(() => _9_1)).Realize();if (!_3_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._93>(false, default, _3_1.RemainingTokens);
}


var node = new _93(_9_1.RealizedValue, _3_1.RealizedValue, _3_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _94 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _94(__GeneratedPartialV1.Deferred.CstNodes.Inners._9<ParseMode.Realized> _9_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._9_1 = _9_1;
                    this._4_1 = _4_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._94>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._94> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._9<ParseMode.Realized> _9_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._94> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _9_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._9.Create(previousNodeRealizationResult).Realize();if (!_9_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._94>(false, default, _9_1.RemainingTokens);
}


var _4_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._4.Create(Future.Create(() => _9_1)).Realize();if (!_4_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._94>(false, default, _4_1.RemainingTokens);
}


var node = new _94(_9_1.RealizedValue, _4_1.RealizedValue, _4_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _95 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _95(__GeneratedPartialV1.Deferred.CstNodes.Inners._9<ParseMode.Realized> _9_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._9_1 = _9_1;
                    this._5_1 = _5_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._95>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._95> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._9<ParseMode.Realized> _9_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._95> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _9_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._9.Create(previousNodeRealizationResult).Realize();if (!_9_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._95>(false, default, _9_1.RemainingTokens);
}


var _5_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._5.Create(Future.Create(() => _9_1)).Realize();if (!_5_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._95>(false, default, _5_1.RemainingTokens);
}


var node = new _95(_9_1.RealizedValue, _5_1.RealizedValue, _5_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _96 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _96(__GeneratedPartialV1.Deferred.CstNodes.Inners._9<ParseMode.Realized> _9_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._9_1 = _9_1;
                    this._6_1 = _6_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._96>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._96> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._9<ParseMode.Realized> _9_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._96> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _9_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._9.Create(previousNodeRealizationResult).Realize();if (!_9_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._96>(false, default, _9_1.RemainingTokens);
}


var _6_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._6.Create(Future.Create(() => _9_1)).Realize();if (!_6_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._96>(false, default, _6_1.RemainingTokens);
}


var node = new _96(_9_1.RealizedValue, _6_1.RealizedValue, _6_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _97 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _97(__GeneratedPartialV1.Deferred.CstNodes.Inners._9<ParseMode.Realized> _9_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._9_1 = _9_1;
                    this._7_1 = _7_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._97>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._97> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._9<ParseMode.Realized> _9_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._97> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _9_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._9.Create(previousNodeRealizationResult).Realize();if (!_9_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._97>(false, default, _9_1.RemainingTokens);
}


var _7_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._7.Create(Future.Create(() => _9_1)).Realize();if (!_7_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._97>(false, default, _7_1.RemainingTokens);
}


var node = new _97(_9_1.RealizedValue, _7_1.RealizedValue, _7_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _98 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _98(__GeneratedPartialV1.Deferred.CstNodes.Inners._9<ParseMode.Realized> _9_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._8<ParseMode.Realized> _8_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._9_1 = _9_1;
                    this._8_1 = _8_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._98>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._98> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._9<ParseMode.Realized> _9_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._8<ParseMode.Realized> _8_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._98> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _9_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._9.Create(previousNodeRealizationResult).Realize();if (!_9_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._98>(false, default, _9_1.RemainingTokens);
}


var _8_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._8.Create(Future.Create(() => _9_1)).Realize();if (!_8_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._98>(false, default, _8_1.RemainingTokens);
}


var node = new _98(_9_1.RealizedValue, _8_1.RealizedValue, _8_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _99 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _99(__GeneratedPartialV1.Deferred.CstNodes.Inners._9<ParseMode.Realized> _9_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._9<ParseMode.Realized> _9_2, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._9_1 = _9_1;
                    this._9_2 = _9_2;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._99>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._99> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._9<ParseMode.Realized> _9_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._9<ParseMode.Realized> _9_2 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._99> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _9_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._9.Create(previousNodeRealizationResult).Realize();if (!_9_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._99>(false, default, _9_1.RemainingTokens);
}


var _9_2 = __GeneratedPartialV1.Deferred.CstNodes.Inners._9.Create(Future.Create(() => _9_1)).Realize();if (!_9_2.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._99>(false, default, _9_2.RemainingTokens);
}


var node = new _99(_9_1.RealizedValue, _9_2.RealizedValue, _9_2.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _9A : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _9A(__GeneratedPartialV1.Deferred.CstNodes.Inners._9<ParseMode.Realized> _9_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._A<ParseMode.Realized> _A_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._9_1 = _9_1;
                    this._A_1 = _A_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._9A>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._9A> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._9<ParseMode.Realized> _9_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._A<ParseMode.Realized> _A_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._9A> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _9_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._9.Create(previousNodeRealizationResult).Realize();if (!_9_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._9A>(false, default, _9_1.RemainingTokens);
}


var _A_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._A.Create(Future.Create(() => _9_1)).Realize();if (!_A_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._9A>(false, default, _A_1.RemainingTokens);
}


var node = new _9A(_9_1.RealizedValue, _A_1.RealizedValue, _A_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _9B : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _9B(__GeneratedPartialV1.Deferred.CstNodes.Inners._9<ParseMode.Realized> _9_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._B<ParseMode.Realized> _B_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._9_1 = _9_1;
                    this._B_1 = _B_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._9B>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._9B> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._9<ParseMode.Realized> _9_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._B<ParseMode.Realized> _B_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._9B> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _9_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._9.Create(previousNodeRealizationResult).Realize();if (!_9_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._9B>(false, default, _9_1.RemainingTokens);
}


var _B_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._B.Create(Future.Create(() => _9_1)).Realize();if (!_B_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._9B>(false, default, _B_1.RemainingTokens);
}


var node = new _9B(_9_1.RealizedValue, _B_1.RealizedValue, _B_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _9C : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _9C(__GeneratedPartialV1.Deferred.CstNodes.Inners._9<ParseMode.Realized> _9_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._C<ParseMode.Realized> _C_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._9_1 = _9_1;
                    this._C_1 = _C_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._9C>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._9C> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._9<ParseMode.Realized> _9_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._C<ParseMode.Realized> _C_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._9C> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _9_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._9.Create(previousNodeRealizationResult).Realize();if (!_9_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._9C>(false, default, _9_1.RemainingTokens);
}


var _C_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._C.Create(Future.Create(() => _9_1)).Realize();if (!_C_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._9C>(false, default, _C_1.RemainingTokens);
}


var node = new _9C(_9_1.RealizedValue, _C_1.RealizedValue, _C_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _9D : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _9D(__GeneratedPartialV1.Deferred.CstNodes.Inners._9<ParseMode.Realized> _9_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._D<ParseMode.Realized> _D_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._9_1 = _9_1;
                    this._D_1 = _D_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._9D>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._9D> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._9<ParseMode.Realized> _9_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._D<ParseMode.Realized> _D_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._9D> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _9_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._9.Create(previousNodeRealizationResult).Realize();if (!_9_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._9D>(false, default, _9_1.RemainingTokens);
}


var _D_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._D.Create(Future.Create(() => _9_1)).Realize();if (!_D_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._9D>(false, default, _D_1.RemainingTokens);
}


var node = new _9D(_9_1.RealizedValue, _D_1.RealizedValue, _D_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _9E : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _9E(__GeneratedPartialV1.Deferred.CstNodes.Inners._9<ParseMode.Realized> _9_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._E<ParseMode.Realized> _E_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._9_1 = _9_1;
                    this._E_1 = _E_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._9E>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._9E> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._9<ParseMode.Realized> _9_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._E<ParseMode.Realized> _E_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._9E> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _9_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._9.Create(previousNodeRealizationResult).Realize();if (!_9_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._9E>(false, default, _9_1.RemainingTokens);
}


var _E_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._E.Create(Future.Create(() => _9_1)).Realize();if (!_E_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._9E>(false, default, _E_1.RemainingTokens);
}


var node = new _9E(_9_1.RealizedValue, _E_1.RealizedValue, _E_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _9F : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _9F(__GeneratedPartialV1.Deferred.CstNodes.Inners._9<ParseMode.Realized> _9_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._F<ParseMode.Realized> _F_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._9_1 = _9_1;
                    this._F_1 = _F_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._9F>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._9F> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._9<ParseMode.Realized> _9_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._F<ParseMode.Realized> _F_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._9F> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _9_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._9.Create(previousNodeRealizationResult).Realize();if (!_9_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._9F>(false, default, _9_1.RemainingTokens);
}


var _F_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._F.Create(Future.Create(() => _9_1)).Realize();if (!_F_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._9F>(false, default, _F_1.RemainingTokens);
}


var node = new _9F(_9_1.RealizedValue, _F_1.RealizedValue, _F_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _A0 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _A0(__GeneratedPartialV1.Deferred.CstNodes.Inners._A<ParseMode.Realized> _A_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._0<ParseMode.Realized> _0_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._A_1 = _A_1;
                    this._0_1 = _0_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._A0>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._A0> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._A<ParseMode.Realized> _A_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<ParseMode.Realized> _0_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._A0> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _A_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._A.Create(previousNodeRealizationResult).Realize();if (!_A_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._A0>(false, default, _A_1.RemainingTokens);
}


var _0_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._0.Create(Future.Create(() => _A_1)).Realize();if (!_0_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._A0>(false, default, _0_1.RemainingTokens);
}


var node = new _A0(_A_1.RealizedValue, _0_1.RealizedValue, _0_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _A1 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _A1(__GeneratedPartialV1.Deferred.CstNodes.Inners._A<ParseMode.Realized> _A_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._1<ParseMode.Realized> _1_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._A_1 = _A_1;
                    this._1_1 = _1_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._A1>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._A1> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._A<ParseMode.Realized> _A_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<ParseMode.Realized> _1_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._A1> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _A_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._A.Create(previousNodeRealizationResult).Realize();if (!_A_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._A1>(false, default, _A_1.RemainingTokens);
}


var _1_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._1.Create(Future.Create(() => _A_1)).Realize();if (!_1_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._A1>(false, default, _1_1.RemainingTokens);
}


var node = new _A1(_A_1.RealizedValue, _1_1.RealizedValue, _1_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _A2 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _A2(__GeneratedPartialV1.Deferred.CstNodes.Inners._A<ParseMode.Realized> _A_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._A_1 = _A_1;
                    this._2_1 = _2_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._A2>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._A2> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._A<ParseMode.Realized> _A_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._A2> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _A_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._A.Create(previousNodeRealizationResult).Realize();if (!_A_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._A2>(false, default, _A_1.RemainingTokens);
}


var _2_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._2.Create(Future.Create(() => _A_1)).Realize();if (!_2_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._A2>(false, default, _2_1.RemainingTokens);
}


var node = new _A2(_A_1.RealizedValue, _2_1.RealizedValue, _2_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _A3 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _A3(__GeneratedPartialV1.Deferred.CstNodes.Inners._A<ParseMode.Realized> _A_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._A_1 = _A_1;
                    this._3_1 = _3_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._A3>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._A3> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._A<ParseMode.Realized> _A_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._A3> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _A_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._A.Create(previousNodeRealizationResult).Realize();if (!_A_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._A3>(false, default, _A_1.RemainingTokens);
}


var _3_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._3.Create(Future.Create(() => _A_1)).Realize();if (!_3_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._A3>(false, default, _3_1.RemainingTokens);
}


var node = new _A3(_A_1.RealizedValue, _3_1.RealizedValue, _3_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _A4 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _A4(__GeneratedPartialV1.Deferred.CstNodes.Inners._A<ParseMode.Realized> _A_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._A_1 = _A_1;
                    this._4_1 = _4_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._A4>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._A4> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._A<ParseMode.Realized> _A_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._A4> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _A_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._A.Create(previousNodeRealizationResult).Realize();if (!_A_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._A4>(false, default, _A_1.RemainingTokens);
}


var _4_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._4.Create(Future.Create(() => _A_1)).Realize();if (!_4_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._A4>(false, default, _4_1.RemainingTokens);
}


var node = new _A4(_A_1.RealizedValue, _4_1.RealizedValue, _4_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _A5 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _A5(__GeneratedPartialV1.Deferred.CstNodes.Inners._A<ParseMode.Realized> _A_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._A_1 = _A_1;
                    this._5_1 = _5_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._A5>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._A5> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._A<ParseMode.Realized> _A_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._A5> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _A_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._A.Create(previousNodeRealizationResult).Realize();if (!_A_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._A5>(false, default, _A_1.RemainingTokens);
}


var _5_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._5.Create(Future.Create(() => _A_1)).Realize();if (!_5_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._A5>(false, default, _5_1.RemainingTokens);
}


var node = new _A5(_A_1.RealizedValue, _5_1.RealizedValue, _5_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _A6 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _A6(__GeneratedPartialV1.Deferred.CstNodes.Inners._A<ParseMode.Realized> _A_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._A_1 = _A_1;
                    this._6_1 = _6_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._A6>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._A6> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._A<ParseMode.Realized> _A_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._A6> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _A_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._A.Create(previousNodeRealizationResult).Realize();if (!_A_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._A6>(false, default, _A_1.RemainingTokens);
}


var _6_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._6.Create(Future.Create(() => _A_1)).Realize();if (!_6_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._A6>(false, default, _6_1.RemainingTokens);
}


var node = new _A6(_A_1.RealizedValue, _6_1.RealizedValue, _6_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _A7 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _A7(__GeneratedPartialV1.Deferred.CstNodes.Inners._A<ParseMode.Realized> _A_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._A_1 = _A_1;
                    this._7_1 = _7_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._A7>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._A7> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._A<ParseMode.Realized> _A_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._A7> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _A_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._A.Create(previousNodeRealizationResult).Realize();if (!_A_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._A7>(false, default, _A_1.RemainingTokens);
}


var _7_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._7.Create(Future.Create(() => _A_1)).Realize();if (!_7_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._A7>(false, default, _7_1.RemainingTokens);
}


var node = new _A7(_A_1.RealizedValue, _7_1.RealizedValue, _7_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _A8 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _A8(__GeneratedPartialV1.Deferred.CstNodes.Inners._A<ParseMode.Realized> _A_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._8<ParseMode.Realized> _8_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._A_1 = _A_1;
                    this._8_1 = _8_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._A8>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._A8> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._A<ParseMode.Realized> _A_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._8<ParseMode.Realized> _8_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._A8> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _A_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._A.Create(previousNodeRealizationResult).Realize();if (!_A_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._A8>(false, default, _A_1.RemainingTokens);
}


var _8_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._8.Create(Future.Create(() => _A_1)).Realize();if (!_8_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._A8>(false, default, _8_1.RemainingTokens);
}


var node = new _A8(_A_1.RealizedValue, _8_1.RealizedValue, _8_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _A9 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _A9(__GeneratedPartialV1.Deferred.CstNodes.Inners._A<ParseMode.Realized> _A_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._9<ParseMode.Realized> _9_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._A_1 = _A_1;
                    this._9_1 = _9_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._A9>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._A9> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._A<ParseMode.Realized> _A_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._9<ParseMode.Realized> _9_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._A9> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _A_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._A.Create(previousNodeRealizationResult).Realize();if (!_A_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._A9>(false, default, _A_1.RemainingTokens);
}


var _9_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._9.Create(Future.Create(() => _A_1)).Realize();if (!_9_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._A9>(false, default, _9_1.RemainingTokens);
}


var node = new _A9(_A_1.RealizedValue, _9_1.RealizedValue, _9_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _AA : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _AA(__GeneratedPartialV1.Deferred.CstNodes.Inners._A<ParseMode.Realized> _A_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._A<ParseMode.Realized> _A_2, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._A_1 = _A_1;
                    this._A_2 = _A_2;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._AA>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._AA> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._A<ParseMode.Realized> _A_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._A<ParseMode.Realized> _A_2 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._AA> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _A_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._A.Create(previousNodeRealizationResult).Realize();if (!_A_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._AA>(false, default, _A_1.RemainingTokens);
}


var _A_2 = __GeneratedPartialV1.Deferred.CstNodes.Inners._A.Create(Future.Create(() => _A_1)).Realize();if (!_A_2.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._AA>(false, default, _A_2.RemainingTokens);
}


var node = new _AA(_A_1.RealizedValue, _A_2.RealizedValue, _A_2.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _AB : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _AB(__GeneratedPartialV1.Deferred.CstNodes.Inners._A<ParseMode.Realized> _A_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._B<ParseMode.Realized> _B_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._A_1 = _A_1;
                    this._B_1 = _B_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._AB>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._AB> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._A<ParseMode.Realized> _A_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._B<ParseMode.Realized> _B_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._AB> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _A_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._A.Create(previousNodeRealizationResult).Realize();if (!_A_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._AB>(false, default, _A_1.RemainingTokens);
}


var _B_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._B.Create(Future.Create(() => _A_1)).Realize();if (!_B_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._AB>(false, default, _B_1.RemainingTokens);
}


var node = new _AB(_A_1.RealizedValue, _B_1.RealizedValue, _B_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _AC : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _AC(__GeneratedPartialV1.Deferred.CstNodes.Inners._A<ParseMode.Realized> _A_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._C<ParseMode.Realized> _C_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._A_1 = _A_1;
                    this._C_1 = _C_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._AC>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._AC> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._A<ParseMode.Realized> _A_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._C<ParseMode.Realized> _C_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._AC> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _A_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._A.Create(previousNodeRealizationResult).Realize();if (!_A_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._AC>(false, default, _A_1.RemainingTokens);
}


var _C_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._C.Create(Future.Create(() => _A_1)).Realize();if (!_C_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._AC>(false, default, _C_1.RemainingTokens);
}


var node = new _AC(_A_1.RealizedValue, _C_1.RealizedValue, _C_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _AD : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _AD(__GeneratedPartialV1.Deferred.CstNodes.Inners._A<ParseMode.Realized> _A_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._D<ParseMode.Realized> _D_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._A_1 = _A_1;
                    this._D_1 = _D_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._AD>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._AD> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._A<ParseMode.Realized> _A_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._D<ParseMode.Realized> _D_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._AD> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _A_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._A.Create(previousNodeRealizationResult).Realize();if (!_A_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._AD>(false, default, _A_1.RemainingTokens);
}


var _D_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._D.Create(Future.Create(() => _A_1)).Realize();if (!_D_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._AD>(false, default, _D_1.RemainingTokens);
}


var node = new _AD(_A_1.RealizedValue, _D_1.RealizedValue, _D_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _AE : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _AE(__GeneratedPartialV1.Deferred.CstNodes.Inners._A<ParseMode.Realized> _A_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._E<ParseMode.Realized> _E_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._A_1 = _A_1;
                    this._E_1 = _E_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._AE>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._AE> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._A<ParseMode.Realized> _A_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._E<ParseMode.Realized> _E_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._AE> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _A_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._A.Create(previousNodeRealizationResult).Realize();if (!_A_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._AE>(false, default, _A_1.RemainingTokens);
}


var _E_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._E.Create(Future.Create(() => _A_1)).Realize();if (!_E_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._AE>(false, default, _E_1.RemainingTokens);
}


var node = new _AE(_A_1.RealizedValue, _E_1.RealizedValue, _E_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _AF : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _AF(__GeneratedPartialV1.Deferred.CstNodes.Inners._A<ParseMode.Realized> _A_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._F<ParseMode.Realized> _F_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._A_1 = _A_1;
                    this._F_1 = _F_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._AF>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._AF> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._A<ParseMode.Realized> _A_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._F<ParseMode.Realized> _F_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._AF> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _A_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._A.Create(previousNodeRealizationResult).Realize();if (!_A_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._AF>(false, default, _A_1.RemainingTokens);
}


var _F_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._F.Create(Future.Create(() => _A_1)).Realize();if (!_F_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._AF>(false, default, _F_1.RemainingTokens);
}


var node = new _AF(_A_1.RealizedValue, _F_1.RealizedValue, _F_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _B0 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _B0(__GeneratedPartialV1.Deferred.CstNodes.Inners._B<ParseMode.Realized> _B_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._0<ParseMode.Realized> _0_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._B_1 = _B_1;
                    this._0_1 = _0_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._B0>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._B0> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._B<ParseMode.Realized> _B_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<ParseMode.Realized> _0_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._B0> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _B_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._B.Create(previousNodeRealizationResult).Realize();if (!_B_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._B0>(false, default, _B_1.RemainingTokens);
}


var _0_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._0.Create(Future.Create(() => _B_1)).Realize();if (!_0_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._B0>(false, default, _0_1.RemainingTokens);
}


var node = new _B0(_B_1.RealizedValue, _0_1.RealizedValue, _0_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _B1 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _B1(__GeneratedPartialV1.Deferred.CstNodes.Inners._B<ParseMode.Realized> _B_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._1<ParseMode.Realized> _1_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._B_1 = _B_1;
                    this._1_1 = _1_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._B1>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._B1> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._B<ParseMode.Realized> _B_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<ParseMode.Realized> _1_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._B1> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _B_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._B.Create(previousNodeRealizationResult).Realize();if (!_B_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._B1>(false, default, _B_1.RemainingTokens);
}


var _1_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._1.Create(Future.Create(() => _B_1)).Realize();if (!_1_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._B1>(false, default, _1_1.RemainingTokens);
}


var node = new _B1(_B_1.RealizedValue, _1_1.RealizedValue, _1_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _B2 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _B2(__GeneratedPartialV1.Deferred.CstNodes.Inners._B<ParseMode.Realized> _B_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._B_1 = _B_1;
                    this._2_1 = _2_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._B2>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._B2> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._B<ParseMode.Realized> _B_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._B2> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _B_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._B.Create(previousNodeRealizationResult).Realize();if (!_B_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._B2>(false, default, _B_1.RemainingTokens);
}


var _2_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._2.Create(Future.Create(() => _B_1)).Realize();if (!_2_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._B2>(false, default, _2_1.RemainingTokens);
}


var node = new _B2(_B_1.RealizedValue, _2_1.RealizedValue, _2_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _B3 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _B3(__GeneratedPartialV1.Deferred.CstNodes.Inners._B<ParseMode.Realized> _B_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._B_1 = _B_1;
                    this._3_1 = _3_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._B3>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._B3> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._B<ParseMode.Realized> _B_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._B3> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _B_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._B.Create(previousNodeRealizationResult).Realize();if (!_B_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._B3>(false, default, _B_1.RemainingTokens);
}


var _3_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._3.Create(Future.Create(() => _B_1)).Realize();if (!_3_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._B3>(false, default, _3_1.RemainingTokens);
}


var node = new _B3(_B_1.RealizedValue, _3_1.RealizedValue, _3_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _B4 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _B4(__GeneratedPartialV1.Deferred.CstNodes.Inners._B<ParseMode.Realized> _B_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._B_1 = _B_1;
                    this._4_1 = _4_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._B4>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._B4> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._B<ParseMode.Realized> _B_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._B4> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _B_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._B.Create(previousNodeRealizationResult).Realize();if (!_B_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._B4>(false, default, _B_1.RemainingTokens);
}


var _4_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._4.Create(Future.Create(() => _B_1)).Realize();if (!_4_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._B4>(false, default, _4_1.RemainingTokens);
}


var node = new _B4(_B_1.RealizedValue, _4_1.RealizedValue, _4_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _B5 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _B5(__GeneratedPartialV1.Deferred.CstNodes.Inners._B<ParseMode.Realized> _B_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._B_1 = _B_1;
                    this._5_1 = _5_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._B5>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._B5> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._B<ParseMode.Realized> _B_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._B5> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _B_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._B.Create(previousNodeRealizationResult).Realize();if (!_B_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._B5>(false, default, _B_1.RemainingTokens);
}


var _5_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._5.Create(Future.Create(() => _B_1)).Realize();if (!_5_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._B5>(false, default, _5_1.RemainingTokens);
}


var node = new _B5(_B_1.RealizedValue, _5_1.RealizedValue, _5_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _B6 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _B6(__GeneratedPartialV1.Deferred.CstNodes.Inners._B<ParseMode.Realized> _B_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._B_1 = _B_1;
                    this._6_1 = _6_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._B6>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._B6> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._B<ParseMode.Realized> _B_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._B6> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _B_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._B.Create(previousNodeRealizationResult).Realize();if (!_B_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._B6>(false, default, _B_1.RemainingTokens);
}


var _6_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._6.Create(Future.Create(() => _B_1)).Realize();if (!_6_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._B6>(false, default, _6_1.RemainingTokens);
}


var node = new _B6(_B_1.RealizedValue, _6_1.RealizedValue, _6_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _B7 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _B7(__GeneratedPartialV1.Deferred.CstNodes.Inners._B<ParseMode.Realized> _B_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._B_1 = _B_1;
                    this._7_1 = _7_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._B7>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._B7> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._B<ParseMode.Realized> _B_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._B7> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _B_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._B.Create(previousNodeRealizationResult).Realize();if (!_B_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._B7>(false, default, _B_1.RemainingTokens);
}


var _7_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._7.Create(Future.Create(() => _B_1)).Realize();if (!_7_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._B7>(false, default, _7_1.RemainingTokens);
}


var node = new _B7(_B_1.RealizedValue, _7_1.RealizedValue, _7_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _B8 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _B8(__GeneratedPartialV1.Deferred.CstNodes.Inners._B<ParseMode.Realized> _B_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._8<ParseMode.Realized> _8_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._B_1 = _B_1;
                    this._8_1 = _8_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._B8>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._B8> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._B<ParseMode.Realized> _B_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._8<ParseMode.Realized> _8_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._B8> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _B_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._B.Create(previousNodeRealizationResult).Realize();if (!_B_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._B8>(false, default, _B_1.RemainingTokens);
}


var _8_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._8.Create(Future.Create(() => _B_1)).Realize();if (!_8_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._B8>(false, default, _8_1.RemainingTokens);
}


var node = new _B8(_B_1.RealizedValue, _8_1.RealizedValue, _8_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _B9 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _B9(__GeneratedPartialV1.Deferred.CstNodes.Inners._B<ParseMode.Realized> _B_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._9<ParseMode.Realized> _9_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._B_1 = _B_1;
                    this._9_1 = _9_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._B9>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._B9> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._B<ParseMode.Realized> _B_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._9<ParseMode.Realized> _9_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._B9> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _B_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._B.Create(previousNodeRealizationResult).Realize();if (!_B_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._B9>(false, default, _B_1.RemainingTokens);
}


var _9_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._9.Create(Future.Create(() => _B_1)).Realize();if (!_9_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._B9>(false, default, _9_1.RemainingTokens);
}


var node = new _B9(_B_1.RealizedValue, _9_1.RealizedValue, _9_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _BA : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _BA(__GeneratedPartialV1.Deferred.CstNodes.Inners._B<ParseMode.Realized> _B_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._A<ParseMode.Realized> _A_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._B_1 = _B_1;
                    this._A_1 = _A_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._BA>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._BA> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._B<ParseMode.Realized> _B_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._A<ParseMode.Realized> _A_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._BA> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _B_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._B.Create(previousNodeRealizationResult).Realize();if (!_B_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._BA>(false, default, _B_1.RemainingTokens);
}


var _A_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._A.Create(Future.Create(() => _B_1)).Realize();if (!_A_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._BA>(false, default, _A_1.RemainingTokens);
}


var node = new _BA(_B_1.RealizedValue, _A_1.RealizedValue, _A_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _BB : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _BB(__GeneratedPartialV1.Deferred.CstNodes.Inners._B<ParseMode.Realized> _B_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._B<ParseMode.Realized> _B_2, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._B_1 = _B_1;
                    this._B_2 = _B_2;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._BB>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._BB> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._B<ParseMode.Realized> _B_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._B<ParseMode.Realized> _B_2 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._BB> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _B_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._B.Create(previousNodeRealizationResult).Realize();if (!_B_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._BB>(false, default, _B_1.RemainingTokens);
}


var _B_2 = __GeneratedPartialV1.Deferred.CstNodes.Inners._B.Create(Future.Create(() => _B_1)).Realize();if (!_B_2.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._BB>(false, default, _B_2.RemainingTokens);
}


var node = new _BB(_B_1.RealizedValue, _B_2.RealizedValue, _B_2.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _BC : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _BC(__GeneratedPartialV1.Deferred.CstNodes.Inners._B<ParseMode.Realized> _B_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._C<ParseMode.Realized> _C_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._B_1 = _B_1;
                    this._C_1 = _C_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._BC>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._BC> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._B<ParseMode.Realized> _B_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._C<ParseMode.Realized> _C_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._BC> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _B_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._B.Create(previousNodeRealizationResult).Realize();if (!_B_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._BC>(false, default, _B_1.RemainingTokens);
}


var _C_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._C.Create(Future.Create(() => _B_1)).Realize();if (!_C_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._BC>(false, default, _C_1.RemainingTokens);
}


var node = new _BC(_B_1.RealizedValue, _C_1.RealizedValue, _C_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _BD : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _BD(__GeneratedPartialV1.Deferred.CstNodes.Inners._B<ParseMode.Realized> _B_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._D<ParseMode.Realized> _D_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._B_1 = _B_1;
                    this._D_1 = _D_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._BD>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._BD> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._B<ParseMode.Realized> _B_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._D<ParseMode.Realized> _D_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._BD> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _B_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._B.Create(previousNodeRealizationResult).Realize();if (!_B_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._BD>(false, default, _B_1.RemainingTokens);
}


var _D_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._D.Create(Future.Create(() => _B_1)).Realize();if (!_D_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._BD>(false, default, _D_1.RemainingTokens);
}


var node = new _BD(_B_1.RealizedValue, _D_1.RealizedValue, _D_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _BE : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _BE(__GeneratedPartialV1.Deferred.CstNodes.Inners._B<ParseMode.Realized> _B_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._E<ParseMode.Realized> _E_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._B_1 = _B_1;
                    this._E_1 = _E_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._BE>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._BE> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._B<ParseMode.Realized> _B_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._E<ParseMode.Realized> _E_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._BE> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _B_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._B.Create(previousNodeRealizationResult).Realize();if (!_B_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._BE>(false, default, _B_1.RemainingTokens);
}


var _E_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._E.Create(Future.Create(() => _B_1)).Realize();if (!_E_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._BE>(false, default, _E_1.RemainingTokens);
}


var node = new _BE(_B_1.RealizedValue, _E_1.RealizedValue, _E_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _BF : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _BF(__GeneratedPartialV1.Deferred.CstNodes.Inners._B<ParseMode.Realized> _B_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._F<ParseMode.Realized> _F_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._B_1 = _B_1;
                    this._F_1 = _F_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._BF>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._BF> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._B<ParseMode.Realized> _B_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._F<ParseMode.Realized> _F_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._BF> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _B_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._B.Create(previousNodeRealizationResult).Realize();if (!_B_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._BF>(false, default, _B_1.RemainingTokens);
}


var _F_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._F.Create(Future.Create(() => _B_1)).Realize();if (!_F_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._BF>(false, default, _F_1.RemainingTokens);
}


var node = new _BF(_B_1.RealizedValue, _F_1.RealizedValue, _F_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _C0 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _C0(__GeneratedPartialV1.Deferred.CstNodes.Inners._C<ParseMode.Realized> _C_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._0<ParseMode.Realized> _0_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._C_1 = _C_1;
                    this._0_1 = _0_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._C0>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._C0> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._C<ParseMode.Realized> _C_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<ParseMode.Realized> _0_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._C0> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _C_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._C.Create(previousNodeRealizationResult).Realize();if (!_C_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._C0>(false, default, _C_1.RemainingTokens);
}


var _0_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._0.Create(Future.Create(() => _C_1)).Realize();if (!_0_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._C0>(false, default, _0_1.RemainingTokens);
}


var node = new _C0(_C_1.RealizedValue, _0_1.RealizedValue, _0_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _C1 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _C1(__GeneratedPartialV1.Deferred.CstNodes.Inners._C<ParseMode.Realized> _C_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._1<ParseMode.Realized> _1_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._C_1 = _C_1;
                    this._1_1 = _1_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._C1>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._C1> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._C<ParseMode.Realized> _C_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<ParseMode.Realized> _1_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._C1> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _C_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._C.Create(previousNodeRealizationResult).Realize();if (!_C_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._C1>(false, default, _C_1.RemainingTokens);
}


var _1_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._1.Create(Future.Create(() => _C_1)).Realize();if (!_1_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._C1>(false, default, _1_1.RemainingTokens);
}


var node = new _C1(_C_1.RealizedValue, _1_1.RealizedValue, _1_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _C2 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _C2(__GeneratedPartialV1.Deferred.CstNodes.Inners._C<ParseMode.Realized> _C_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._C_1 = _C_1;
                    this._2_1 = _2_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._C2>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._C2> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._C<ParseMode.Realized> _C_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._C2> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _C_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._C.Create(previousNodeRealizationResult).Realize();if (!_C_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._C2>(false, default, _C_1.RemainingTokens);
}


var _2_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._2.Create(Future.Create(() => _C_1)).Realize();if (!_2_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._C2>(false, default, _2_1.RemainingTokens);
}


var node = new _C2(_C_1.RealizedValue, _2_1.RealizedValue, _2_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _C3 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _C3(__GeneratedPartialV1.Deferred.CstNodes.Inners._C<ParseMode.Realized> _C_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._C_1 = _C_1;
                    this._3_1 = _3_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._C3>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._C3> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._C<ParseMode.Realized> _C_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._C3> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _C_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._C.Create(previousNodeRealizationResult).Realize();if (!_C_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._C3>(false, default, _C_1.RemainingTokens);
}


var _3_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._3.Create(Future.Create(() => _C_1)).Realize();if (!_3_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._C3>(false, default, _3_1.RemainingTokens);
}


var node = new _C3(_C_1.RealizedValue, _3_1.RealizedValue, _3_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _C4 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _C4(__GeneratedPartialV1.Deferred.CstNodes.Inners._C<ParseMode.Realized> _C_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._C_1 = _C_1;
                    this._4_1 = _4_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._C4>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._C4> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._C<ParseMode.Realized> _C_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._C4> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _C_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._C.Create(previousNodeRealizationResult).Realize();if (!_C_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._C4>(false, default, _C_1.RemainingTokens);
}


var _4_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._4.Create(Future.Create(() => _C_1)).Realize();if (!_4_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._C4>(false, default, _4_1.RemainingTokens);
}


var node = new _C4(_C_1.RealizedValue, _4_1.RealizedValue, _4_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _C5 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _C5(__GeneratedPartialV1.Deferred.CstNodes.Inners._C<ParseMode.Realized> _C_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._C_1 = _C_1;
                    this._5_1 = _5_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._C5>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._C5> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._C<ParseMode.Realized> _C_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._C5> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _C_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._C.Create(previousNodeRealizationResult).Realize();if (!_C_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._C5>(false, default, _C_1.RemainingTokens);
}


var _5_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._5.Create(Future.Create(() => _C_1)).Realize();if (!_5_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._C5>(false, default, _5_1.RemainingTokens);
}


var node = new _C5(_C_1.RealizedValue, _5_1.RealizedValue, _5_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _C6 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _C6(__GeneratedPartialV1.Deferred.CstNodes.Inners._C<ParseMode.Realized> _C_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._C_1 = _C_1;
                    this._6_1 = _6_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._C6>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._C6> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._C<ParseMode.Realized> _C_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._C6> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _C_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._C.Create(previousNodeRealizationResult).Realize();if (!_C_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._C6>(false, default, _C_1.RemainingTokens);
}


var _6_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._6.Create(Future.Create(() => _C_1)).Realize();if (!_6_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._C6>(false, default, _6_1.RemainingTokens);
}


var node = new _C6(_C_1.RealizedValue, _6_1.RealizedValue, _6_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _C7 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _C7(__GeneratedPartialV1.Deferred.CstNodes.Inners._C<ParseMode.Realized> _C_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._C_1 = _C_1;
                    this._7_1 = _7_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._C7>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._C7> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._C<ParseMode.Realized> _C_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._C7> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _C_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._C.Create(previousNodeRealizationResult).Realize();if (!_C_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._C7>(false, default, _C_1.RemainingTokens);
}


var _7_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._7.Create(Future.Create(() => _C_1)).Realize();if (!_7_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._C7>(false, default, _7_1.RemainingTokens);
}


var node = new _C7(_C_1.RealizedValue, _7_1.RealizedValue, _7_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _C8 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _C8(__GeneratedPartialV1.Deferred.CstNodes.Inners._C<ParseMode.Realized> _C_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._8<ParseMode.Realized> _8_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._C_1 = _C_1;
                    this._8_1 = _8_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._C8>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._C8> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._C<ParseMode.Realized> _C_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._8<ParseMode.Realized> _8_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._C8> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _C_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._C.Create(previousNodeRealizationResult).Realize();if (!_C_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._C8>(false, default, _C_1.RemainingTokens);
}


var _8_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._8.Create(Future.Create(() => _C_1)).Realize();if (!_8_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._C8>(false, default, _8_1.RemainingTokens);
}


var node = new _C8(_C_1.RealizedValue, _8_1.RealizedValue, _8_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _C9 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _C9(__GeneratedPartialV1.Deferred.CstNodes.Inners._C<ParseMode.Realized> _C_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._9<ParseMode.Realized> _9_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._C_1 = _C_1;
                    this._9_1 = _9_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._C9>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._C9> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._C<ParseMode.Realized> _C_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._9<ParseMode.Realized> _9_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._C9> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _C_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._C.Create(previousNodeRealizationResult).Realize();if (!_C_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._C9>(false, default, _C_1.RemainingTokens);
}


var _9_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._9.Create(Future.Create(() => _C_1)).Realize();if (!_9_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._C9>(false, default, _9_1.RemainingTokens);
}


var node = new _C9(_C_1.RealizedValue, _9_1.RealizedValue, _9_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _CA : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _CA(__GeneratedPartialV1.Deferred.CstNodes.Inners._C<ParseMode.Realized> _C_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._A<ParseMode.Realized> _A_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._C_1 = _C_1;
                    this._A_1 = _A_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._CA>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._CA> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._C<ParseMode.Realized> _C_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._A<ParseMode.Realized> _A_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._CA> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _C_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._C.Create(previousNodeRealizationResult).Realize();if (!_C_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._CA>(false, default, _C_1.RemainingTokens);
}


var _A_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._A.Create(Future.Create(() => _C_1)).Realize();if (!_A_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._CA>(false, default, _A_1.RemainingTokens);
}


var node = new _CA(_C_1.RealizedValue, _A_1.RealizedValue, _A_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _CB : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _CB(__GeneratedPartialV1.Deferred.CstNodes.Inners._C<ParseMode.Realized> _C_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._B<ParseMode.Realized> _B_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._C_1 = _C_1;
                    this._B_1 = _B_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._CB>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._CB> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._C<ParseMode.Realized> _C_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._B<ParseMode.Realized> _B_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._CB> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _C_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._C.Create(previousNodeRealizationResult).Realize();if (!_C_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._CB>(false, default, _C_1.RemainingTokens);
}


var _B_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._B.Create(Future.Create(() => _C_1)).Realize();if (!_B_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._CB>(false, default, _B_1.RemainingTokens);
}


var node = new _CB(_C_1.RealizedValue, _B_1.RealizedValue, _B_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _CC : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _CC(__GeneratedPartialV1.Deferred.CstNodes.Inners._C<ParseMode.Realized> _C_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._C<ParseMode.Realized> _C_2, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._C_1 = _C_1;
                    this._C_2 = _C_2;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._CC>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._CC> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._C<ParseMode.Realized> _C_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._C<ParseMode.Realized> _C_2 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._CC> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _C_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._C.Create(previousNodeRealizationResult).Realize();if (!_C_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._CC>(false, default, _C_1.RemainingTokens);
}


var _C_2 = __GeneratedPartialV1.Deferred.CstNodes.Inners._C.Create(Future.Create(() => _C_1)).Realize();if (!_C_2.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._CC>(false, default, _C_2.RemainingTokens);
}


var node = new _CC(_C_1.RealizedValue, _C_2.RealizedValue, _C_2.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _CD : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _CD(__GeneratedPartialV1.Deferred.CstNodes.Inners._C<ParseMode.Realized> _C_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._D<ParseMode.Realized> _D_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._C_1 = _C_1;
                    this._D_1 = _D_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._CD>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._CD> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._C<ParseMode.Realized> _C_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._D<ParseMode.Realized> _D_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._CD> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _C_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._C.Create(previousNodeRealizationResult).Realize();if (!_C_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._CD>(false, default, _C_1.RemainingTokens);
}


var _D_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._D.Create(Future.Create(() => _C_1)).Realize();if (!_D_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._CD>(false, default, _D_1.RemainingTokens);
}


var node = new _CD(_C_1.RealizedValue, _D_1.RealizedValue, _D_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _CE : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _CE(__GeneratedPartialV1.Deferred.CstNodes.Inners._C<ParseMode.Realized> _C_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._E<ParseMode.Realized> _E_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._C_1 = _C_1;
                    this._E_1 = _E_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._CE>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._CE> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._C<ParseMode.Realized> _C_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._E<ParseMode.Realized> _E_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._CE> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _C_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._C.Create(previousNodeRealizationResult).Realize();if (!_C_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._CE>(false, default, _C_1.RemainingTokens);
}


var _E_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._E.Create(Future.Create(() => _C_1)).Realize();if (!_E_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._CE>(false, default, _E_1.RemainingTokens);
}


var node = new _CE(_C_1.RealizedValue, _E_1.RealizedValue, _E_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _CF : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _CF(__GeneratedPartialV1.Deferred.CstNodes.Inners._C<ParseMode.Realized> _C_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._F<ParseMode.Realized> _F_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._C_1 = _C_1;
                    this._F_1 = _F_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._CF>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._CF> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._C<ParseMode.Realized> _C_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._F<ParseMode.Realized> _F_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._CF> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _C_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._C.Create(previousNodeRealizationResult).Realize();if (!_C_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._CF>(false, default, _C_1.RemainingTokens);
}


var _F_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._F.Create(Future.Create(() => _C_1)).Realize();if (!_F_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._CF>(false, default, _F_1.RemainingTokens);
}


var node = new _CF(_C_1.RealizedValue, _F_1.RealizedValue, _F_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _D0 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _D0(__GeneratedPartialV1.Deferred.CstNodes.Inners._D<ParseMode.Realized> _D_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._0<ParseMode.Realized> _0_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._D_1 = _D_1;
                    this._0_1 = _0_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._D0>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._D0> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._D<ParseMode.Realized> _D_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<ParseMode.Realized> _0_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._D0> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _D_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._D.Create(previousNodeRealizationResult).Realize();if (!_D_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._D0>(false, default, _D_1.RemainingTokens);
}


var _0_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._0.Create(Future.Create(() => _D_1)).Realize();if (!_0_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._D0>(false, default, _0_1.RemainingTokens);
}


var node = new _D0(_D_1.RealizedValue, _0_1.RealizedValue, _0_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _D1 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _D1(__GeneratedPartialV1.Deferred.CstNodes.Inners._D<ParseMode.Realized> _D_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._1<ParseMode.Realized> _1_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._D_1 = _D_1;
                    this._1_1 = _1_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._D1>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._D1> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._D<ParseMode.Realized> _D_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<ParseMode.Realized> _1_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._D1> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _D_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._D.Create(previousNodeRealizationResult).Realize();if (!_D_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._D1>(false, default, _D_1.RemainingTokens);
}


var _1_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._1.Create(Future.Create(() => _D_1)).Realize();if (!_1_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._D1>(false, default, _1_1.RemainingTokens);
}


var node = new _D1(_D_1.RealizedValue, _1_1.RealizedValue, _1_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _D2 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _D2(__GeneratedPartialV1.Deferred.CstNodes.Inners._D<ParseMode.Realized> _D_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._D_1 = _D_1;
                    this._2_1 = _2_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._D2>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._D2> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._D<ParseMode.Realized> _D_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._D2> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _D_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._D.Create(previousNodeRealizationResult).Realize();if (!_D_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._D2>(false, default, _D_1.RemainingTokens);
}


var _2_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._2.Create(Future.Create(() => _D_1)).Realize();if (!_2_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._D2>(false, default, _2_1.RemainingTokens);
}


var node = new _D2(_D_1.RealizedValue, _2_1.RealizedValue, _2_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _D3 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _D3(__GeneratedPartialV1.Deferred.CstNodes.Inners._D<ParseMode.Realized> _D_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._D_1 = _D_1;
                    this._3_1 = _3_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._D3>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._D3> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._D<ParseMode.Realized> _D_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._D3> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _D_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._D.Create(previousNodeRealizationResult).Realize();if (!_D_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._D3>(false, default, _D_1.RemainingTokens);
}


var _3_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._3.Create(Future.Create(() => _D_1)).Realize();if (!_3_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._D3>(false, default, _3_1.RemainingTokens);
}


var node = new _D3(_D_1.RealizedValue, _3_1.RealizedValue, _3_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _D4 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _D4(__GeneratedPartialV1.Deferred.CstNodes.Inners._D<ParseMode.Realized> _D_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._D_1 = _D_1;
                    this._4_1 = _4_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._D4>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._D4> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._D<ParseMode.Realized> _D_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._D4> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _D_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._D.Create(previousNodeRealizationResult).Realize();if (!_D_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._D4>(false, default, _D_1.RemainingTokens);
}


var _4_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._4.Create(Future.Create(() => _D_1)).Realize();if (!_4_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._D4>(false, default, _4_1.RemainingTokens);
}


var node = new _D4(_D_1.RealizedValue, _4_1.RealizedValue, _4_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _D5 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _D5(__GeneratedPartialV1.Deferred.CstNodes.Inners._D<ParseMode.Realized> _D_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._D_1 = _D_1;
                    this._5_1 = _5_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._D5>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._D5> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._D<ParseMode.Realized> _D_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._D5> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _D_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._D.Create(previousNodeRealizationResult).Realize();if (!_D_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._D5>(false, default, _D_1.RemainingTokens);
}


var _5_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._5.Create(Future.Create(() => _D_1)).Realize();if (!_5_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._D5>(false, default, _5_1.RemainingTokens);
}


var node = new _D5(_D_1.RealizedValue, _5_1.RealizedValue, _5_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _D6 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _D6(__GeneratedPartialV1.Deferred.CstNodes.Inners._D<ParseMode.Realized> _D_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._D_1 = _D_1;
                    this._6_1 = _6_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._D6>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._D6> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._D<ParseMode.Realized> _D_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._D6> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _D_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._D.Create(previousNodeRealizationResult).Realize();if (!_D_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._D6>(false, default, _D_1.RemainingTokens);
}


var _6_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._6.Create(Future.Create(() => _D_1)).Realize();if (!_6_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._D6>(false, default, _6_1.RemainingTokens);
}


var node = new _D6(_D_1.RealizedValue, _6_1.RealizedValue, _6_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _D7 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _D7(__GeneratedPartialV1.Deferred.CstNodes.Inners._D<ParseMode.Realized> _D_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._D_1 = _D_1;
                    this._7_1 = _7_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._D7>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._D7> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._D<ParseMode.Realized> _D_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._D7> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _D_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._D.Create(previousNodeRealizationResult).Realize();if (!_D_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._D7>(false, default, _D_1.RemainingTokens);
}


var _7_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._7.Create(Future.Create(() => _D_1)).Realize();if (!_7_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._D7>(false, default, _7_1.RemainingTokens);
}


var node = new _D7(_D_1.RealizedValue, _7_1.RealizedValue, _7_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _D8 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _D8(__GeneratedPartialV1.Deferred.CstNodes.Inners._D<ParseMode.Realized> _D_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._8<ParseMode.Realized> _8_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._D_1 = _D_1;
                    this._8_1 = _8_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._D8>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._D8> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._D<ParseMode.Realized> _D_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._8<ParseMode.Realized> _8_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._D8> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _D_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._D.Create(previousNodeRealizationResult).Realize();if (!_D_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._D8>(false, default, _D_1.RemainingTokens);
}


var _8_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._8.Create(Future.Create(() => _D_1)).Realize();if (!_8_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._D8>(false, default, _8_1.RemainingTokens);
}


var node = new _D8(_D_1.RealizedValue, _8_1.RealizedValue, _8_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _D9 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _D9(__GeneratedPartialV1.Deferred.CstNodes.Inners._D<ParseMode.Realized> _D_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._9<ParseMode.Realized> _9_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._D_1 = _D_1;
                    this._9_1 = _9_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._D9>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._D9> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._D<ParseMode.Realized> _D_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._9<ParseMode.Realized> _9_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._D9> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _D_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._D.Create(previousNodeRealizationResult).Realize();if (!_D_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._D9>(false, default, _D_1.RemainingTokens);
}


var _9_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._9.Create(Future.Create(() => _D_1)).Realize();if (!_9_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._D9>(false, default, _9_1.RemainingTokens);
}


var node = new _D9(_D_1.RealizedValue, _9_1.RealizedValue, _9_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _DA : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _DA(__GeneratedPartialV1.Deferred.CstNodes.Inners._D<ParseMode.Realized> _D_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._A<ParseMode.Realized> _A_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._D_1 = _D_1;
                    this._A_1 = _A_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._DA>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._DA> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._D<ParseMode.Realized> _D_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._A<ParseMode.Realized> _A_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._DA> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _D_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._D.Create(previousNodeRealizationResult).Realize();if (!_D_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._DA>(false, default, _D_1.RemainingTokens);
}


var _A_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._A.Create(Future.Create(() => _D_1)).Realize();if (!_A_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._DA>(false, default, _A_1.RemainingTokens);
}


var node = new _DA(_D_1.RealizedValue, _A_1.RealizedValue, _A_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _DB : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _DB(__GeneratedPartialV1.Deferred.CstNodes.Inners._D<ParseMode.Realized> _D_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._B<ParseMode.Realized> _B_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._D_1 = _D_1;
                    this._B_1 = _B_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._DB>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._DB> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._D<ParseMode.Realized> _D_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._B<ParseMode.Realized> _B_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._DB> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _D_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._D.Create(previousNodeRealizationResult).Realize();if (!_D_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._DB>(false, default, _D_1.RemainingTokens);
}


var _B_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._B.Create(Future.Create(() => _D_1)).Realize();if (!_B_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._DB>(false, default, _B_1.RemainingTokens);
}


var node = new _DB(_D_1.RealizedValue, _B_1.RealizedValue, _B_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _DC : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _DC(__GeneratedPartialV1.Deferred.CstNodes.Inners._D<ParseMode.Realized> _D_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._C<ParseMode.Realized> _C_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._D_1 = _D_1;
                    this._C_1 = _C_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._DC>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._DC> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._D<ParseMode.Realized> _D_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._C<ParseMode.Realized> _C_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._DC> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _D_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._D.Create(previousNodeRealizationResult).Realize();if (!_D_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._DC>(false, default, _D_1.RemainingTokens);
}


var _C_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._C.Create(Future.Create(() => _D_1)).Realize();if (!_C_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._DC>(false, default, _C_1.RemainingTokens);
}


var node = new _DC(_D_1.RealizedValue, _C_1.RealizedValue, _C_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _DD : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _DD(__GeneratedPartialV1.Deferred.CstNodes.Inners._D<ParseMode.Realized> _D_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._D<ParseMode.Realized> _D_2, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._D_1 = _D_1;
                    this._D_2 = _D_2;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._DD>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._DD> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._D<ParseMode.Realized> _D_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._D<ParseMode.Realized> _D_2 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._DD> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _D_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._D.Create(previousNodeRealizationResult).Realize();if (!_D_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._DD>(false, default, _D_1.RemainingTokens);
}


var _D_2 = __GeneratedPartialV1.Deferred.CstNodes.Inners._D.Create(Future.Create(() => _D_1)).Realize();if (!_D_2.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._DD>(false, default, _D_2.RemainingTokens);
}


var node = new _DD(_D_1.RealizedValue, _D_2.RealizedValue, _D_2.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _DE : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _DE(__GeneratedPartialV1.Deferred.CstNodes.Inners._D<ParseMode.Realized> _D_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._E<ParseMode.Realized> _E_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._D_1 = _D_1;
                    this._E_1 = _E_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._DE>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._DE> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._D<ParseMode.Realized> _D_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._E<ParseMode.Realized> _E_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._DE> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _D_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._D.Create(previousNodeRealizationResult).Realize();if (!_D_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._DE>(false, default, _D_1.RemainingTokens);
}


var _E_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._E.Create(Future.Create(() => _D_1)).Realize();if (!_E_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._DE>(false, default, _E_1.RemainingTokens);
}


var node = new _DE(_D_1.RealizedValue, _E_1.RealizedValue, _E_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _DF : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _DF(__GeneratedPartialV1.Deferred.CstNodes.Inners._D<ParseMode.Realized> _D_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._F<ParseMode.Realized> _F_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._D_1 = _D_1;
                    this._F_1 = _F_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._DF>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._DF> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._D<ParseMode.Realized> _D_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._F<ParseMode.Realized> _F_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._DF> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _D_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._D.Create(previousNodeRealizationResult).Realize();if (!_D_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._DF>(false, default, _D_1.RemainingTokens);
}


var _F_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._F.Create(Future.Create(() => _D_1)).Realize();if (!_F_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._DF>(false, default, _F_1.RemainingTokens);
}


var node = new _DF(_D_1.RealizedValue, _F_1.RealizedValue, _F_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _E0 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _E0(__GeneratedPartialV1.Deferred.CstNodes.Inners._E<ParseMode.Realized> _E_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._0<ParseMode.Realized> _0_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._E_1 = _E_1;
                    this._0_1 = _0_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._E0>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._E0> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._E<ParseMode.Realized> _E_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<ParseMode.Realized> _0_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._E0> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _E_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._E.Create(previousNodeRealizationResult).Realize();if (!_E_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._E0>(false, default, _E_1.RemainingTokens);
}


var _0_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._0.Create(Future.Create(() => _E_1)).Realize();if (!_0_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._E0>(false, default, _0_1.RemainingTokens);
}


var node = new _E0(_E_1.RealizedValue, _0_1.RealizedValue, _0_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _E1 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _E1(__GeneratedPartialV1.Deferred.CstNodes.Inners._E<ParseMode.Realized> _E_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._1<ParseMode.Realized> _1_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._E_1 = _E_1;
                    this._1_1 = _1_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._E1>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._E1> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._E<ParseMode.Realized> _E_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<ParseMode.Realized> _1_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._E1> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _E_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._E.Create(previousNodeRealizationResult).Realize();if (!_E_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._E1>(false, default, _E_1.RemainingTokens);
}


var _1_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._1.Create(Future.Create(() => _E_1)).Realize();if (!_1_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._E1>(false, default, _1_1.RemainingTokens);
}


var node = new _E1(_E_1.RealizedValue, _1_1.RealizedValue, _1_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _E2 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _E2(__GeneratedPartialV1.Deferred.CstNodes.Inners._E<ParseMode.Realized> _E_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._E_1 = _E_1;
                    this._2_1 = _2_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._E2>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._E2> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._E<ParseMode.Realized> _E_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._E2> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _E_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._E.Create(previousNodeRealizationResult).Realize();if (!_E_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._E2>(false, default, _E_1.RemainingTokens);
}


var _2_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._2.Create(Future.Create(() => _E_1)).Realize();if (!_2_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._E2>(false, default, _2_1.RemainingTokens);
}


var node = new _E2(_E_1.RealizedValue, _2_1.RealizedValue, _2_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _E3 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _E3(__GeneratedPartialV1.Deferred.CstNodes.Inners._E<ParseMode.Realized> _E_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._E_1 = _E_1;
                    this._3_1 = _3_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._E3>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._E3> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._E<ParseMode.Realized> _E_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._E3> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _E_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._E.Create(previousNodeRealizationResult).Realize();if (!_E_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._E3>(false, default, _E_1.RemainingTokens);
}


var _3_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._3.Create(Future.Create(() => _E_1)).Realize();if (!_3_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._E3>(false, default, _3_1.RemainingTokens);
}


var node = new _E3(_E_1.RealizedValue, _3_1.RealizedValue, _3_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _E4 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _E4(__GeneratedPartialV1.Deferred.CstNodes.Inners._E<ParseMode.Realized> _E_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._E_1 = _E_1;
                    this._4_1 = _4_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._E4>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._E4> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._E<ParseMode.Realized> _E_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._E4> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _E_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._E.Create(previousNodeRealizationResult).Realize();if (!_E_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._E4>(false, default, _E_1.RemainingTokens);
}


var _4_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._4.Create(Future.Create(() => _E_1)).Realize();if (!_4_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._E4>(false, default, _4_1.RemainingTokens);
}


var node = new _E4(_E_1.RealizedValue, _4_1.RealizedValue, _4_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _E5 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _E5(__GeneratedPartialV1.Deferred.CstNodes.Inners._E<ParseMode.Realized> _E_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._E_1 = _E_1;
                    this._5_1 = _5_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._E5>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._E5> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._E<ParseMode.Realized> _E_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._E5> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _E_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._E.Create(previousNodeRealizationResult).Realize();if (!_E_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._E5>(false, default, _E_1.RemainingTokens);
}


var _5_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._5.Create(Future.Create(() => _E_1)).Realize();if (!_5_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._E5>(false, default, _5_1.RemainingTokens);
}


var node = new _E5(_E_1.RealizedValue, _5_1.RealizedValue, _5_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _E6 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _E6(__GeneratedPartialV1.Deferred.CstNodes.Inners._E<ParseMode.Realized> _E_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._E_1 = _E_1;
                    this._6_1 = _6_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._E6>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._E6> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._E<ParseMode.Realized> _E_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._E6> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _E_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._E.Create(previousNodeRealizationResult).Realize();if (!_E_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._E6>(false, default, _E_1.RemainingTokens);
}


var _6_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._6.Create(Future.Create(() => _E_1)).Realize();if (!_6_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._E6>(false, default, _6_1.RemainingTokens);
}


var node = new _E6(_E_1.RealizedValue, _6_1.RealizedValue, _6_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _E7 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _E7(__GeneratedPartialV1.Deferred.CstNodes.Inners._E<ParseMode.Realized> _E_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._E_1 = _E_1;
                    this._7_1 = _7_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._E7>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._E7> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._E<ParseMode.Realized> _E_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._E7> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _E_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._E.Create(previousNodeRealizationResult).Realize();if (!_E_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._E7>(false, default, _E_1.RemainingTokens);
}


var _7_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._7.Create(Future.Create(() => _E_1)).Realize();if (!_7_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._E7>(false, default, _7_1.RemainingTokens);
}


var node = new _E7(_E_1.RealizedValue, _7_1.RealizedValue, _7_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _E8 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _E8(__GeneratedPartialV1.Deferred.CstNodes.Inners._E<ParseMode.Realized> _E_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._8<ParseMode.Realized> _8_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._E_1 = _E_1;
                    this._8_1 = _8_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._E8>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._E8> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._E<ParseMode.Realized> _E_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._8<ParseMode.Realized> _8_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._E8> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _E_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._E.Create(previousNodeRealizationResult).Realize();if (!_E_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._E8>(false, default, _E_1.RemainingTokens);
}


var _8_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._8.Create(Future.Create(() => _E_1)).Realize();if (!_8_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._E8>(false, default, _8_1.RemainingTokens);
}


var node = new _E8(_E_1.RealizedValue, _8_1.RealizedValue, _8_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _E9 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _E9(__GeneratedPartialV1.Deferred.CstNodes.Inners._E<ParseMode.Realized> _E_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._9<ParseMode.Realized> _9_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._E_1 = _E_1;
                    this._9_1 = _9_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._E9>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._E9> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._E<ParseMode.Realized> _E_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._9<ParseMode.Realized> _9_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._E9> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _E_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._E.Create(previousNodeRealizationResult).Realize();if (!_E_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._E9>(false, default, _E_1.RemainingTokens);
}


var _9_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._9.Create(Future.Create(() => _E_1)).Realize();if (!_9_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._E9>(false, default, _9_1.RemainingTokens);
}


var node = new _E9(_E_1.RealizedValue, _9_1.RealizedValue, _9_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _EA : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _EA(__GeneratedPartialV1.Deferred.CstNodes.Inners._E<ParseMode.Realized> _E_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._A<ParseMode.Realized> _A_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._E_1 = _E_1;
                    this._A_1 = _A_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._EA>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._EA> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._E<ParseMode.Realized> _E_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._A<ParseMode.Realized> _A_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._EA> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _E_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._E.Create(previousNodeRealizationResult).Realize();if (!_E_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._EA>(false, default, _E_1.RemainingTokens);
}


var _A_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._A.Create(Future.Create(() => _E_1)).Realize();if (!_A_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._EA>(false, default, _A_1.RemainingTokens);
}


var node = new _EA(_E_1.RealizedValue, _A_1.RealizedValue, _A_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _EB : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _EB(__GeneratedPartialV1.Deferred.CstNodes.Inners._E<ParseMode.Realized> _E_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._B<ParseMode.Realized> _B_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._E_1 = _E_1;
                    this._B_1 = _B_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._EB>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._EB> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._E<ParseMode.Realized> _E_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._B<ParseMode.Realized> _B_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._EB> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _E_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._E.Create(previousNodeRealizationResult).Realize();if (!_E_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._EB>(false, default, _E_1.RemainingTokens);
}


var _B_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._B.Create(Future.Create(() => _E_1)).Realize();if (!_B_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._EB>(false, default, _B_1.RemainingTokens);
}


var node = new _EB(_E_1.RealizedValue, _B_1.RealizedValue, _B_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _EC : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _EC(__GeneratedPartialV1.Deferred.CstNodes.Inners._E<ParseMode.Realized> _E_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._C<ParseMode.Realized> _C_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._E_1 = _E_1;
                    this._C_1 = _C_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._EC>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._EC> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._E<ParseMode.Realized> _E_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._C<ParseMode.Realized> _C_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._EC> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _E_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._E.Create(previousNodeRealizationResult).Realize();if (!_E_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._EC>(false, default, _E_1.RemainingTokens);
}


var _C_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._C.Create(Future.Create(() => _E_1)).Realize();if (!_C_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._EC>(false, default, _C_1.RemainingTokens);
}


var node = new _EC(_E_1.RealizedValue, _C_1.RealizedValue, _C_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _ED : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _ED(__GeneratedPartialV1.Deferred.CstNodes.Inners._E<ParseMode.Realized> _E_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._D<ParseMode.Realized> _D_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._E_1 = _E_1;
                    this._D_1 = _D_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._ED>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._ED> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._E<ParseMode.Realized> _E_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._D<ParseMode.Realized> _D_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._ED> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _E_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._E.Create(previousNodeRealizationResult).Realize();if (!_E_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._ED>(false, default, _E_1.RemainingTokens);
}


var _D_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._D.Create(Future.Create(() => _E_1)).Realize();if (!_D_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._ED>(false, default, _D_1.RemainingTokens);
}


var node = new _ED(_E_1.RealizedValue, _D_1.RealizedValue, _D_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _EE : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _EE(__GeneratedPartialV1.Deferred.CstNodes.Inners._E<ParseMode.Realized> _E_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._E<ParseMode.Realized> _E_2, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._E_1 = _E_1;
                    this._E_2 = _E_2;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._EE>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._EE> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._E<ParseMode.Realized> _E_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._E<ParseMode.Realized> _E_2 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._EE> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _E_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._E.Create(previousNodeRealizationResult).Realize();if (!_E_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._EE>(false, default, _E_1.RemainingTokens);
}


var _E_2 = __GeneratedPartialV1.Deferred.CstNodes.Inners._E.Create(Future.Create(() => _E_1)).Realize();if (!_E_2.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._EE>(false, default, _E_2.RemainingTokens);
}


var node = new _EE(_E_1.RealizedValue, _E_2.RealizedValue, _E_2.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _EF : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _EF(__GeneratedPartialV1.Deferred.CstNodes.Inners._E<ParseMode.Realized> _E_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._F<ParseMode.Realized> _F_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._E_1 = _E_1;
                    this._F_1 = _F_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._EF>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._EF> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._E<ParseMode.Realized> _E_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._F<ParseMode.Realized> _F_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._EF> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _E_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._E.Create(previousNodeRealizationResult).Realize();if (!_E_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._EF>(false, default, _E_1.RemainingTokens);
}


var _F_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._F.Create(Future.Create(() => _E_1)).Realize();if (!_F_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._EF>(false, default, _F_1.RemainingTokens);
}


var node = new _EF(_E_1.RealizedValue, _F_1.RealizedValue, _F_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _F0 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _F0(__GeneratedPartialV1.Deferred.CstNodes.Inners._F<ParseMode.Realized> _F_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._0<ParseMode.Realized> _0_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._F_1 = _F_1;
                    this._0_1 = _0_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._F0>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._F0> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._F<ParseMode.Realized> _F_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<ParseMode.Realized> _0_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._F0> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _F_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._F.Create(previousNodeRealizationResult).Realize();if (!_F_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._F0>(false, default, _F_1.RemainingTokens);
}


var _0_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._0.Create(Future.Create(() => _F_1)).Realize();if (!_0_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._F0>(false, default, _0_1.RemainingTokens);
}


var node = new _F0(_F_1.RealizedValue, _0_1.RealizedValue, _0_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _F1 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _F1(__GeneratedPartialV1.Deferred.CstNodes.Inners._F<ParseMode.Realized> _F_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._1<ParseMode.Realized> _1_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._F_1 = _F_1;
                    this._1_1 = _1_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._F1>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._F1> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._F<ParseMode.Realized> _F_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<ParseMode.Realized> _1_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._F1> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _F_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._F.Create(previousNodeRealizationResult).Realize();if (!_F_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._F1>(false, default, _F_1.RemainingTokens);
}


var _1_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._1.Create(Future.Create(() => _F_1)).Realize();if (!_1_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._F1>(false, default, _1_1.RemainingTokens);
}


var node = new _F1(_F_1.RealizedValue, _1_1.RealizedValue, _1_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _F2 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _F2(__GeneratedPartialV1.Deferred.CstNodes.Inners._F<ParseMode.Realized> _F_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._F_1 = _F_1;
                    this._2_1 = _2_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._F2>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._F2> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._F<ParseMode.Realized> _F_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._F2> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _F_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._F.Create(previousNodeRealizationResult).Realize();if (!_F_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._F2>(false, default, _F_1.RemainingTokens);
}


var _2_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._2.Create(Future.Create(() => _F_1)).Realize();if (!_2_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._F2>(false, default, _2_1.RemainingTokens);
}


var node = new _F2(_F_1.RealizedValue, _2_1.RealizedValue, _2_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _F3 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _F3(__GeneratedPartialV1.Deferred.CstNodes.Inners._F<ParseMode.Realized> _F_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._F_1 = _F_1;
                    this._3_1 = _3_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._F3>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._F3> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._F<ParseMode.Realized> _F_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._F3> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _F_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._F.Create(previousNodeRealizationResult).Realize();if (!_F_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._F3>(false, default, _F_1.RemainingTokens);
}


var _3_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._3.Create(Future.Create(() => _F_1)).Realize();if (!_3_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._F3>(false, default, _3_1.RemainingTokens);
}


var node = new _F3(_F_1.RealizedValue, _3_1.RealizedValue, _3_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _F4 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _F4(__GeneratedPartialV1.Deferred.CstNodes.Inners._F<ParseMode.Realized> _F_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._F_1 = _F_1;
                    this._4_1 = _4_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._F4>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._F4> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._F<ParseMode.Realized> _F_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._F4> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _F_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._F.Create(previousNodeRealizationResult).Realize();if (!_F_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._F4>(false, default, _F_1.RemainingTokens);
}


var _4_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._4.Create(Future.Create(() => _F_1)).Realize();if (!_4_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._F4>(false, default, _4_1.RemainingTokens);
}


var node = new _F4(_F_1.RealizedValue, _4_1.RealizedValue, _4_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _F5 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _F5(__GeneratedPartialV1.Deferred.CstNodes.Inners._F<ParseMode.Realized> _F_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._F_1 = _F_1;
                    this._5_1 = _5_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._F5>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._F5> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._F<ParseMode.Realized> _F_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._F5> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _F_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._F.Create(previousNodeRealizationResult).Realize();if (!_F_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._F5>(false, default, _F_1.RemainingTokens);
}


var _5_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._5.Create(Future.Create(() => _F_1)).Realize();if (!_5_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._F5>(false, default, _5_1.RemainingTokens);
}


var node = new _F5(_F_1.RealizedValue, _5_1.RealizedValue, _5_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _F6 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _F6(__GeneratedPartialV1.Deferred.CstNodes.Inners._F<ParseMode.Realized> _F_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._F_1 = _F_1;
                    this._6_1 = _6_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._F6>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._F6> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._F<ParseMode.Realized> _F_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._F6> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _F_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._F.Create(previousNodeRealizationResult).Realize();if (!_F_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._F6>(false, default, _F_1.RemainingTokens);
}


var _6_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._6.Create(Future.Create(() => _F_1)).Realize();if (!_6_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._F6>(false, default, _6_1.RemainingTokens);
}


var node = new _F6(_F_1.RealizedValue, _6_1.RealizedValue, _6_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _F7 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _F7(__GeneratedPartialV1.Deferred.CstNodes.Inners._F<ParseMode.Realized> _F_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._F_1 = _F_1;
                    this._7_1 = _7_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._F7>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._F7> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._F<ParseMode.Realized> _F_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._F7> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _F_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._F.Create(previousNodeRealizationResult).Realize();if (!_F_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._F7>(false, default, _F_1.RemainingTokens);
}


var _7_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._7.Create(Future.Create(() => _F_1)).Realize();if (!_7_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._F7>(false, default, _7_1.RemainingTokens);
}


var node = new _F7(_F_1.RealizedValue, _7_1.RealizedValue, _7_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _F8 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _F8(__GeneratedPartialV1.Deferred.CstNodes.Inners._F<ParseMode.Realized> _F_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._8<ParseMode.Realized> _8_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._F_1 = _F_1;
                    this._8_1 = _8_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._F8>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._F8> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._F<ParseMode.Realized> _F_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._8<ParseMode.Realized> _8_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._F8> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _F_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._F.Create(previousNodeRealizationResult).Realize();if (!_F_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._F8>(false, default, _F_1.RemainingTokens);
}


var _8_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._8.Create(Future.Create(() => _F_1)).Realize();if (!_8_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._F8>(false, default, _8_1.RemainingTokens);
}


var node = new _F8(_F_1.RealizedValue, _8_1.RealizedValue, _8_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _F9 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _F9(__GeneratedPartialV1.Deferred.CstNodes.Inners._F<ParseMode.Realized> _F_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._9<ParseMode.Realized> _9_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._F_1 = _F_1;
                    this._9_1 = _9_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._F9>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._F9> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._F<ParseMode.Realized> _F_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._9<ParseMode.Realized> _9_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._F9> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _F_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._F.Create(previousNodeRealizationResult).Realize();if (!_F_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._F9>(false, default, _F_1.RemainingTokens);
}


var _9_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._9.Create(Future.Create(() => _F_1)).Realize();if (!_9_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._F9>(false, default, _9_1.RemainingTokens);
}


var node = new _F9(_F_1.RealizedValue, _9_1.RealizedValue, _9_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FA : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _FA(__GeneratedPartialV1.Deferred.CstNodes.Inners._F<ParseMode.Realized> _F_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._A<ParseMode.Realized> _A_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._F_1 = _F_1;
                    this._A_1 = _A_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._FA>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._FA> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._F<ParseMode.Realized> _F_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._A<ParseMode.Realized> _A_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._FA> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _F_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._F.Create(previousNodeRealizationResult).Realize();if (!_F_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._FA>(false, default, _F_1.RemainingTokens);
}


var _A_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._A.Create(Future.Create(() => _F_1)).Realize();if (!_A_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._FA>(false, default, _A_1.RemainingTokens);
}


var node = new _FA(_F_1.RealizedValue, _A_1.RealizedValue, _A_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FB : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _FB(__GeneratedPartialV1.Deferred.CstNodes.Inners._F<ParseMode.Realized> _F_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._B<ParseMode.Realized> _B_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._F_1 = _F_1;
                    this._B_1 = _B_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._FB>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._FB> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._F<ParseMode.Realized> _F_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._B<ParseMode.Realized> _B_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._FB> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _F_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._F.Create(previousNodeRealizationResult).Realize();if (!_F_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._FB>(false, default, _F_1.RemainingTokens);
}


var _B_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._B.Create(Future.Create(() => _F_1)).Realize();if (!_B_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._FB>(false, default, _B_1.RemainingTokens);
}


var node = new _FB(_F_1.RealizedValue, _B_1.RealizedValue, _B_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FC : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _FC(__GeneratedPartialV1.Deferred.CstNodes.Inners._F<ParseMode.Realized> _F_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._C<ParseMode.Realized> _C_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._F_1 = _F_1;
                    this._C_1 = _C_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._FC>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._FC> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._F<ParseMode.Realized> _F_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._C<ParseMode.Realized> _C_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._FC> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _F_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._F.Create(previousNodeRealizationResult).Realize();if (!_F_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._FC>(false, default, _F_1.RemainingTokens);
}


var _C_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._C.Create(Future.Create(() => _F_1)).Realize();if (!_C_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._FC>(false, default, _C_1.RemainingTokens);
}


var node = new _FC(_F_1.RealizedValue, _C_1.RealizedValue, _C_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FD : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _FD(__GeneratedPartialV1.Deferred.CstNodes.Inners._F<ParseMode.Realized> _F_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._D<ParseMode.Realized> _D_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._F_1 = _F_1;
                    this._D_1 = _D_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._FD>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._FD> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._F<ParseMode.Realized> _F_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._D<ParseMode.Realized> _D_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._FD> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _F_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._F.Create(previousNodeRealizationResult).Realize();if (!_F_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._FD>(false, default, _F_1.RemainingTokens);
}


var _D_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._D.Create(Future.Create(() => _F_1)).Realize();if (!_D_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._FD>(false, default, _D_1.RemainingTokens);
}


var node = new _FD(_F_1.RealizedValue, _D_1.RealizedValue, _D_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FE : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _FE(__GeneratedPartialV1.Deferred.CstNodes.Inners._F<ParseMode.Realized> _F_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._E<ParseMode.Realized> _E_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._F_1 = _F_1;
                    this._E_1 = _E_1;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._FE>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._FE> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._F<ParseMode.Realized> _F_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._E<ParseMode.Realized> _E_1 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._FE> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _F_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._F.Create(previousNodeRealizationResult).Realize();if (!_F_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._FE>(false, default, _F_1.RemainingTokens);
}


var _E_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._E.Create(Future.Create(() => _F_1)).Realize();if (!_E_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._FE>(false, default, _E_1.RemainingTokens);
}


var node = new _FE(_F_1.RealizedValue, _E_1.RealizedValue, _E_1.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class _FF : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _FF(__GeneratedPartialV1.Deferred.CstNodes.Inners._F<ParseMode.Realized> _F_1, __GeneratedPartialV1.Deferred.CstNodes.Inners._F<ParseMode.Realized> _F_2, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                    this._F_1 = _F_1;
                    this._F_2 = _F_2;
                    this.realizationResult = new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._FF>(true, this, nextTokens);
                }
                
                private IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._FF> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._F<ParseMode.Realized> _F_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._F<ParseMode.Realized> _F_2 { get; }
                
                internal static IRealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._FF> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
                {
                    var _F_1 = __GeneratedPartialV1.Deferred.CstNodes.Inners._F.Create(previousNodeRealizationResult).Realize();if (!_F_1.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._FF>(false, default, _F_1.RemainingTokens);
}


var _F_2 = __GeneratedPartialV1.Deferred.CstNodes.Inners._F.Create(Future.Create(() => _F_1)).Realize();if (!_F_2.Success)
{
return new RealizationResult<char, _Ⰳx00ⲻFF<TMode>.Realized._FF>(false, default, _F_2.RemainingTokens);
}


var node = new _FF(_F_1.RealizedValue, _F_2.RealizedValue, _F_2.RemainingTokens);
return node.realizationResult;
                }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    return new _Ⰳx00ⲻFF<ParseMode.Deferred>.Deferred(Future.Create(() => this.realizationResult));
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    return this.realizationResult;
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
        }
    }
    
}
