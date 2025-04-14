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
                private _00(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode>> _0_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode>> _0_2, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode>> __0_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode>> __0_2 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode> _0_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode> _0_2 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _01 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _01(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode>> _0_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode>> _1_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode>> __0_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode>> __1_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode> _0_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode> _1_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _02 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _02(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode>> _0_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode>> _2_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode>> __0_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode>> __2_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode> _0_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode> _2_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _03 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _03(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode>> _0_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode>> _3_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode>> __0_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode>> __3_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode> _0_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode> _3_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _04 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _04(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode>> _0_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode>> _4_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode>> __0_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode>> __4_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode> _0_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode> _4_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _05 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _05(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode>> _0_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode>> _5_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode>> __0_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode>> __5_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode> _0_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode> _5_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _06 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _06(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode>> _0_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode>> _6_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode>> __0_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode>> __6_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode> _0_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode> _6_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _07 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _07(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode>> _0_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode>> _7_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode>> __0_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode>> __7_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode> _0_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode> _7_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _08 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _08(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode>> _0_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode>> _8_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode>> __0_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode>> __8_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode> _0_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode> _8_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _09 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _09(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode>> _0_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode>> _9_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode>> __0_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode>> __9_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode> _0_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode> _9_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _0A : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _0A(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode>> _0_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode>> _A_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode>> __0_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode>> __A_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode> _0_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode> _A_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _0B : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _0B(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode>> _0_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode>> _B_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode>> __0_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode>> __B_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode> _0_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode> _B_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _0C : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _0C(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode>> _0_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode>> _C_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode>> __0_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode>> __C_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode> _0_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode> _C_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _0D : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _0D(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode>> _0_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode>> _D_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode>> __0_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode>> __D_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode> _0_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode> _D_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _0E : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _0E(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode>> _0_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode>> _E_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode>> __0_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode>> __E_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode> _0_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode> _E_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _0F : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _0F(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode>> _0_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode>> _F_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode>> __0_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode>> __F_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode> _0_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode> _F_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _10 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _10(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode>> _1_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode>> _0_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode>> __1_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode>> __0_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode> _1_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode> _0_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _11 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _11(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode>> _1_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode>> _1_2, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode>> __1_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode>> __1_2 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode> _1_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode> _1_2 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _12 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _12(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode>> _1_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode>> _2_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode>> __1_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode>> __2_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode> _1_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode> _2_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _13 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _13(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode>> _1_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode>> _3_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode>> __1_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode>> __3_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode> _1_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode> _3_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _14 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _14(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode>> _1_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode>> _4_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode>> __1_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode>> __4_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode> _1_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode> _4_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _15 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _15(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode>> _1_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode>> _5_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode>> __1_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode>> __5_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode> _1_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode> _5_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _16 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _16(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode>> _1_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode>> _6_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode>> __1_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode>> __6_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode> _1_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode> _6_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _17 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _17(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode>> _1_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode>> _7_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode>> __1_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode>> __7_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode> _1_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode> _7_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _18 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _18(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode>> _1_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode>> _8_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode>> __1_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode>> __8_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode> _1_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode> _8_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _19 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _19(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode>> _1_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode>> _9_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode>> __1_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode>> __9_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode> _1_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode> _9_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _1A : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _1A(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode>> _1_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode>> _A_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode>> __1_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode>> __A_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode> _1_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode> _A_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _1B : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _1B(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode>> _1_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode>> _B_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode>> __1_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode>> __B_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode> _1_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode> _B_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _1C : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _1C(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode>> _1_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode>> _C_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode>> __1_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode>> __C_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode> _1_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode> _C_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _1D : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _1D(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode>> _1_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode>> _D_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode>> __1_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode>> __D_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode> _1_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode> _D_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _1E : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _1E(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode>> _1_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode>> _E_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode>> __1_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode>> __E_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode> _1_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode> _E_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _1F : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _1F(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode>> _1_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode>> _F_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode>> __1_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode>> __F_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode> _1_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode> _F_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _20 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _20(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode>> _2_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode>> _0_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode>> __2_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode>> __0_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode> _2_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode> _0_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _21 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _21(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode>> _2_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode>> _1_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode>> __2_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode>> __1_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode> _2_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode> _1_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _22 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _22(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode>> _2_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode>> _2_2, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode>> __2_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode>> __2_2 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode> _2_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode> _2_2 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _23 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _23(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode>> _2_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode>> _3_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode>> __2_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode>> __3_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode> _2_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode> _3_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _24 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _24(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode>> _2_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode>> _4_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode>> __2_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode>> __4_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode> _2_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode> _4_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _25 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _25(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode>> _2_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode>> _5_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode>> __2_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode>> __5_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode> _2_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode> _5_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _26 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _26(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode>> _2_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode>> _6_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode>> __2_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode>> __6_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode> _2_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode> _6_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _27 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _27(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode>> _2_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode>> _7_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode>> __2_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode>> __7_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode> _2_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode> _7_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _28 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _28(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode>> _2_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode>> _8_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode>> __2_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode>> __8_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode> _2_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode> _8_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _29 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _29(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode>> _2_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode>> _9_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode>> __2_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode>> __9_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode> _2_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode> _9_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _2A : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _2A(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode>> _2_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode>> _A_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode>> __2_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode>> __A_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode> _2_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode> _A_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _2B : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _2B(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode>> _2_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode>> _B_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode>> __2_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode>> __B_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode> _2_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode> _B_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _2C : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _2C(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode>> _2_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode>> _C_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode>> __2_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode>> __C_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode> _2_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode> _C_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _2D : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _2D(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode>> _2_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode>> _D_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode>> __2_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode>> __D_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode> _2_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode> _D_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _2E : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _2E(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode>> _2_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode>> _E_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode>> __2_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode>> __E_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode> _2_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode> _E_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _2F : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _2F(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode>> _2_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode>> _F_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode>> __2_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode>> __F_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode> _2_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode> _F_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _30 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _30(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode>> _3_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode>> _0_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode>> __3_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode>> __0_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode> _3_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode> _0_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _31 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _31(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode>> _3_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode>> _1_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode>> __3_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode>> __1_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode> _3_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode> _1_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _32 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _32(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode>> _3_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode>> _2_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode>> __3_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode>> __2_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode> _3_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode> _2_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _33 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _33(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode>> _3_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode>> _3_2, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode>> __3_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode>> __3_2 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode> _3_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode> _3_2 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _34 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _34(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode>> _3_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode>> _4_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode>> __3_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode>> __4_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode> _3_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode> _4_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _35 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _35(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode>> _3_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode>> _5_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode>> __3_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode>> __5_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode> _3_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode> _5_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _36 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _36(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode>> _3_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode>> _6_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode>> __3_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode>> __6_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode> _3_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode> _6_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _37 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _37(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode>> _3_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode>> _7_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode>> __3_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode>> __7_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode> _3_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode> _7_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _38 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _38(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode>> _3_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode>> _8_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode>> __3_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode>> __8_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode> _3_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode> _8_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _39 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _39(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode>> _3_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode>> _9_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode>> __3_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode>> __9_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode> _3_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode> _9_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _3A : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _3A(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode>> _3_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode>> _A_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode>> __3_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode>> __A_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode> _3_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode> _A_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _3B : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _3B(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode>> _3_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode>> _B_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode>> __3_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode>> __B_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode> _3_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode> _B_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _3C : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _3C(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode>> _3_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode>> _C_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode>> __3_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode>> __C_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode> _3_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode> _C_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _3D : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _3D(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode>> _3_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode>> _D_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode>> __3_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode>> __D_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode> _3_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode> _D_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _3E : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _3E(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode>> _3_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode>> _E_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode>> __3_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode>> __E_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode> _3_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode> _E_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _3F : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _3F(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode>> _3_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode>> _F_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode>> __3_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode>> __F_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode> _3_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode> _F_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _40 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _40(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode>> _4_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode>> _0_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode>> __4_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode>> __0_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode> _4_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode> _0_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _41 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _41(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode>> _4_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode>> _1_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode>> __4_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode>> __1_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode> _4_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode> _1_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _42 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _42(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode>> _4_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode>> _2_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode>> __4_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode>> __2_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode> _4_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode> _2_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _43 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _43(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode>> _4_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode>> _3_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode>> __4_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode>> __3_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode> _4_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode> _3_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _44 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _44(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode>> _4_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode>> _4_2, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode>> __4_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode>> __4_2 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode> _4_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode> _4_2 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _45 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _45(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode>> _4_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode>> _5_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode>> __4_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode>> __5_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode> _4_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode> _5_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _46 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _46(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode>> _4_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode>> _6_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode>> __4_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode>> __6_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode> _4_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode> _6_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _47 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _47(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode>> _4_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode>> _7_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode>> __4_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode>> __7_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode> _4_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode> _7_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _48 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _48(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode>> _4_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode>> _8_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode>> __4_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode>> __8_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode> _4_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode> _8_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _49 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _49(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode>> _4_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode>> _9_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode>> __4_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode>> __9_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode> _4_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode> _9_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _4A : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _4A(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode>> _4_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode>> _A_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode>> __4_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode>> __A_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode> _4_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode> _A_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _4B : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _4B(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode>> _4_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode>> _B_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode>> __4_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode>> __B_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode> _4_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode> _B_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _4C : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _4C(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode>> _4_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode>> _C_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode>> __4_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode>> __C_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode> _4_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode> _C_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _4D : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _4D(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode>> _4_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode>> _D_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode>> __4_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode>> __D_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode> _4_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode> _D_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _4E : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _4E(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode>> _4_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode>> _E_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode>> __4_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode>> __E_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode> _4_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode> _E_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _4F : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _4F(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode>> _4_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode>> _F_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode>> __4_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode>> __F_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode> _4_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode> _F_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _50 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _50(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode>> _5_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode>> _0_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode>> __5_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode>> __0_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode> _5_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode> _0_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _51 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _51(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode>> _5_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode>> _1_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode>> __5_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode>> __1_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode> _5_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode> _1_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _52 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _52(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode>> _5_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode>> _2_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode>> __5_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode>> __2_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode> _5_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode> _2_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _53 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _53(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode>> _5_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode>> _3_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode>> __5_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode>> __3_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode> _5_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode> _3_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _54 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _54(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode>> _5_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode>> _4_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode>> __5_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode>> __4_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode> _5_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode> _4_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _55 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _55(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode>> _5_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode>> _5_2, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode>> __5_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode>> __5_2 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode> _5_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode> _5_2 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _56 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _56(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode>> _5_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode>> _6_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode>> __5_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode>> __6_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode> _5_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode> _6_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _57 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _57(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode>> _5_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode>> _7_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode>> __5_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode>> __7_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode> _5_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode> _7_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _58 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _58(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode>> _5_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode>> _8_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode>> __5_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode>> __8_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode> _5_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode> _8_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _59 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _59(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode>> _5_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode>> _9_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode>> __5_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode>> __9_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode> _5_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode> _9_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _5A : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _5A(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode>> _5_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode>> _A_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode>> __5_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode>> __A_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode> _5_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode> _A_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _5B : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _5B(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode>> _5_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode>> _B_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode>> __5_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode>> __B_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode> _5_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode> _B_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _5C : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _5C(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode>> _5_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode>> _C_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode>> __5_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode>> __C_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode> _5_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode> _C_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _5D : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _5D(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode>> _5_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode>> _D_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode>> __5_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode>> __D_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode> _5_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode> _D_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _5E : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _5E(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode>> _5_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode>> _E_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode>> __5_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode>> __E_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode> _5_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode> _E_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _5F : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _5F(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode>> _5_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode>> _F_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode>> __5_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode>> __F_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode> _5_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode> _F_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _60 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _60(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode>> _6_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode>> _0_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode>> __6_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode>> __0_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode> _6_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode> _0_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _61 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _61(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode>> _6_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode>> _1_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode>> __6_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode>> __1_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode> _6_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode> _1_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _62 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _62(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode>> _6_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode>> _2_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode>> __6_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode>> __2_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode> _6_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode> _2_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _63 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _63(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode>> _6_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode>> _3_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode>> __6_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode>> __3_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode> _6_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode> _3_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _64 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _64(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode>> _6_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode>> _4_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode>> __6_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode>> __4_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode> _6_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode> _4_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _65 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _65(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode>> _6_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode>> _5_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode>> __6_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode>> __5_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode> _6_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode> _5_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _66 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _66(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode>> _6_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode>> _6_2, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode>> __6_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode>> __6_2 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode> _6_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode> _6_2 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _67 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _67(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode>> _6_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode>> _7_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode>> __6_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode>> __7_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode> _6_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode> _7_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _68 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _68(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode>> _6_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode>> _8_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode>> __6_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode>> __8_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode> _6_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode> _8_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _69 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _69(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode>> _6_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode>> _9_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode>> __6_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode>> __9_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode> _6_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode> _9_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _6A : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _6A(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode>> _6_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode>> _A_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode>> __6_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode>> __A_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode> _6_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode> _A_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _6B : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _6B(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode>> _6_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode>> _B_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode>> __6_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode>> __B_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode> _6_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode> _B_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _6C : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _6C(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode>> _6_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode>> _C_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode>> __6_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode>> __C_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode> _6_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode> _C_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _6D : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _6D(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode>> _6_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode>> _D_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode>> __6_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode>> __D_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode> _6_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode> _D_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _6E : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _6E(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode>> _6_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode>> _E_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode>> __6_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode>> __E_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode> _6_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode> _E_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _6F : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _6F(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode>> _6_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode>> _F_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode>> __6_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode>> __F_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode> _6_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode> _F_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _70 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _70(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode>> _7_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode>> _0_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode>> __7_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode>> __0_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode> _7_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode> _0_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _71 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _71(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode>> _7_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode>> _1_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode>> __7_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode>> __1_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode> _7_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode> _1_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _72 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _72(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode>> _7_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode>> _2_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode>> __7_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode>> __2_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode> _7_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode> _2_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _73 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _73(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode>> _7_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode>> _3_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode>> __7_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode>> __3_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode> _7_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode> _3_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _74 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _74(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode>> _7_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode>> _4_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode>> __7_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode>> __4_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode> _7_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode> _4_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _75 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _75(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode>> _7_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode>> _5_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode>> __7_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode>> __5_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode> _7_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode> _5_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _76 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _76(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode>> _7_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode>> _6_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode>> __7_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode>> __6_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode> _7_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode> _6_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _77 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _77(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode>> _7_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode>> _7_2, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode>> __7_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode>> __7_2 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode> _7_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode> _7_2 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _78 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _78(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode>> _7_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode>> _8_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode>> __7_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode>> __8_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode> _7_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode> _8_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _79 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _79(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode>> _7_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode>> _9_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode>> __7_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode>> __9_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode> _7_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode> _9_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _7A : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _7A(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode>> _7_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode>> _A_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode>> __7_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode>> __A_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode> _7_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode> _A_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _7B : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _7B(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode>> _7_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode>> _B_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode>> __7_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode>> __B_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode> _7_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode> _B_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _7C : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _7C(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode>> _7_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode>> _C_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode>> __7_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode>> __C_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode> _7_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode> _C_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _7D : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _7D(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode>> _7_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode>> _D_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode>> __7_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode>> __D_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode> _7_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode> _D_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _7E : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _7E(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode>> _7_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode>> _E_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode>> __7_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode>> __E_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode> _7_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode> _E_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _7F : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _7F(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode>> _7_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode>> _F_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode>> __7_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode>> __F_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode> _7_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode> _F_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _80 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _80(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode>> _8_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode>> _0_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode>> __8_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode>> __0_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode> _8_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode> _0_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _81 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _81(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode>> _8_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode>> _1_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode>> __8_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode>> __1_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode> _8_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode> _1_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _82 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _82(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode>> _8_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode>> _2_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode>> __8_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode>> __2_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode> _8_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode> _2_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _83 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _83(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode>> _8_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode>> _3_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode>> __8_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode>> __3_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode> _8_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode> _3_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _84 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _84(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode>> _8_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode>> _4_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode>> __8_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode>> __4_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode> _8_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode> _4_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _85 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _85(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode>> _8_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode>> _5_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode>> __8_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode>> __5_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode> _8_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode> _5_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _86 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _86(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode>> _8_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode>> _6_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode>> __8_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode>> __6_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode> _8_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode> _6_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _87 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _87(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode>> _8_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode>> _7_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode>> __8_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode>> __7_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode> _8_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode> _7_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _88 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _88(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode>> _8_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode>> _8_2, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode>> __8_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode>> __8_2 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode> _8_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode> _8_2 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _89 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _89(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode>> _8_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode>> _9_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode>> __8_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode>> __9_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode> _8_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode> _9_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _8A : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _8A(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode>> _8_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode>> _A_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode>> __8_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode>> __A_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode> _8_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode> _A_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _8B : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _8B(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode>> _8_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode>> _B_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode>> __8_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode>> __B_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode> _8_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode> _B_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _8C : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _8C(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode>> _8_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode>> _C_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode>> __8_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode>> __C_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode> _8_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode> _C_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _8D : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _8D(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode>> _8_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode>> _D_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode>> __8_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode>> __D_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode> _8_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode> _D_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _8E : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _8E(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode>> _8_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode>> _E_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode>> __8_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode>> __E_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode> _8_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode> _E_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _8F : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _8F(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode>> _8_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode>> _F_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode>> __8_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode>> __F_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode> _8_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode> _F_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _90 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _90(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode>> _9_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode>> _0_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode>> __9_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode>> __0_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode> _9_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode> _0_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _91 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _91(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode>> _9_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode>> _1_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode>> __9_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode>> __1_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode> _9_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode> _1_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _92 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _92(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode>> _9_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode>> _2_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode>> __9_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode>> __2_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode> _9_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode> _2_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _93 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _93(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode>> _9_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode>> _3_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode>> __9_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode>> __3_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode> _9_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode> _3_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _94 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _94(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode>> _9_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode>> _4_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode>> __9_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode>> __4_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode> _9_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode> _4_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _95 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _95(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode>> _9_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode>> _5_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode>> __9_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode>> __5_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode> _9_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode> _5_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _96 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _96(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode>> _9_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode>> _6_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode>> __9_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode>> __6_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode> _9_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode> _6_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _97 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _97(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode>> _9_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode>> _7_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode>> __9_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode>> __7_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode> _9_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode> _7_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _98 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _98(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode>> _9_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode>> _8_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode>> __9_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode>> __8_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode> _9_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode> _8_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _99 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _99(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode>> _9_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode>> _9_2, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode>> __9_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode>> __9_2 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode> _9_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode> _9_2 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _9A : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _9A(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode>> _9_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode>> _A_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode>> __9_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode>> __A_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode> _9_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode> _A_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _9B : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _9B(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode>> _9_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode>> _B_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode>> __9_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode>> __B_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode> _9_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode> _B_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _9C : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _9C(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode>> _9_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode>> _C_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode>> __9_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode>> __C_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode> _9_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode> _C_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _9D : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _9D(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode>> _9_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode>> _D_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode>> __9_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode>> __D_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode> _9_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode> _D_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _9E : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _9E(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode>> _9_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode>> _E_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode>> __9_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode>> __E_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode> _9_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode> _E_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _9F : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _9F(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode>> _9_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode>> _F_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode>> __9_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode>> __F_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode> _9_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode> _F_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _A0 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _A0(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode>> _A_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode>> _0_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode>> __A_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode>> __0_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode> _A_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode> _0_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _A1 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _A1(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode>> _A_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode>> _1_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode>> __A_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode>> __1_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode> _A_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode> _1_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _A2 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _A2(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode>> _A_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode>> _2_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode>> __A_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode>> __2_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode> _A_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode> _2_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _A3 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _A3(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode>> _A_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode>> _3_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode>> __A_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode>> __3_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode> _A_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode> _3_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _A4 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _A4(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode>> _A_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode>> _4_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode>> __A_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode>> __4_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode> _A_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode> _4_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _A5 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _A5(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode>> _A_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode>> _5_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode>> __A_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode>> __5_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode> _A_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode> _5_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _A6 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _A6(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode>> _A_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode>> _6_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode>> __A_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode>> __6_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode> _A_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode> _6_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _A7 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _A7(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode>> _A_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode>> _7_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode>> __A_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode>> __7_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode> _A_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode> _7_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _A8 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _A8(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode>> _A_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode>> _8_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode>> __A_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode>> __8_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode> _A_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode> _8_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _A9 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _A9(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode>> _A_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode>> _9_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode>> __A_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode>> __9_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode> _A_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode> _9_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _AA : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _AA(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode>> _A_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode>> _A_2, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode>> __A_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode>> __A_2 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode> _A_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode> _A_2 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _AB : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _AB(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode>> _A_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode>> _B_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode>> __A_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode>> __B_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode> _A_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode> _B_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _AC : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _AC(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode>> _A_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode>> _C_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode>> __A_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode>> __C_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode> _A_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode> _C_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _AD : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _AD(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode>> _A_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode>> _D_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode>> __A_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode>> __D_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode> _A_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode> _D_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _AE : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _AE(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode>> _A_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode>> _E_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode>> __A_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode>> __E_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode> _A_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode> _E_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _AF : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _AF(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode>> _A_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode>> _F_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode>> __A_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode>> __F_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode> _A_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode> _F_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _B0 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _B0(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode>> _B_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode>> _0_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode>> __B_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode>> __0_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode> _B_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode> _0_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _B1 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _B1(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode>> _B_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode>> _1_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode>> __B_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode>> __1_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode> _B_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode> _1_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _B2 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _B2(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode>> _B_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode>> _2_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode>> __B_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode>> __2_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode> _B_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode> _2_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _B3 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _B3(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode>> _B_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode>> _3_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode>> __B_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode>> __3_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode> _B_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode> _3_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _B4 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _B4(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode>> _B_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode>> _4_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode>> __B_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode>> __4_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode> _B_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode> _4_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _B5 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _B5(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode>> _B_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode>> _5_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode>> __B_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode>> __5_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode> _B_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode> _5_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _B6 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _B6(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode>> _B_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode>> _6_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode>> __B_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode>> __6_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode> _B_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode> _6_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _B7 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _B7(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode>> _B_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode>> _7_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode>> __B_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode>> __7_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode> _B_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode> _7_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _B8 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _B8(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode>> _B_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode>> _8_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode>> __B_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode>> __8_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode> _B_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode> _8_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _B9 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _B9(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode>> _B_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode>> _9_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode>> __B_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode>> __9_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode> _B_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode> _9_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _BA : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _BA(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode>> _B_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode>> _A_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode>> __B_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode>> __A_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode> _B_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode> _A_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _BB : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _BB(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode>> _B_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode>> _B_2, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode>> __B_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode>> __B_2 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode> _B_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode> _B_2 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _BC : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _BC(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode>> _B_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode>> _C_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode>> __B_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode>> __C_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode> _B_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode> _C_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _BD : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _BD(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode>> _B_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode>> _D_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode>> __B_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode>> __D_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode> _B_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode> _D_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _BE : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _BE(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode>> _B_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode>> _E_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode>> __B_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode>> __E_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode> _B_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode> _E_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _BF : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _BF(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode>> _B_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode>> _F_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode>> __B_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode>> __F_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode> _B_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode> _F_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _C0 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _C0(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode>> _C_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode>> _0_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode>> __C_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode>> __0_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode> _C_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode> _0_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _C1 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _C1(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode>> _C_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode>> _1_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode>> __C_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode>> __1_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode> _C_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode> _1_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _C2 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _C2(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode>> _C_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode>> _2_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode>> __C_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode>> __2_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode> _C_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode> _2_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _C3 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _C3(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode>> _C_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode>> _3_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode>> __C_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode>> __3_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode> _C_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode> _3_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _C4 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _C4(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode>> _C_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode>> _4_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode>> __C_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode>> __4_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode> _C_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode> _4_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _C5 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _C5(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode>> _C_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode>> _5_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode>> __C_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode>> __5_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode> _C_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode> _5_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _C6 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _C6(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode>> _C_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode>> _6_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode>> __C_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode>> __6_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode> _C_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode> _6_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _C7 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _C7(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode>> _C_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode>> _7_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode>> __C_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode>> __7_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode> _C_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode> _7_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _C8 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _C8(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode>> _C_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode>> _8_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode>> __C_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode>> __8_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode> _C_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode> _8_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _C9 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _C9(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode>> _C_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode>> _9_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode>> __C_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode>> __9_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode> _C_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode> _9_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _CA : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _CA(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode>> _C_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode>> _A_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode>> __C_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode>> __A_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode> _C_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode> _A_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _CB : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _CB(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode>> _C_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode>> _B_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode>> __C_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode>> __B_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode> _C_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode> _B_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _CC : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _CC(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode>> _C_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode>> _C_2, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode>> __C_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode>> __C_2 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode> _C_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode> _C_2 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _CD : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _CD(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode>> _C_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode>> _D_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode>> __C_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode>> __D_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode> _C_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode> _D_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _CE : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _CE(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode>> _C_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode>> _E_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode>> __C_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode>> __E_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode> _C_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode> _E_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _CF : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _CF(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode>> _C_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode>> _F_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode>> __C_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode>> __F_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode> _C_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode> _F_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _D0 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _D0(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode>> _D_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode>> _0_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode>> __D_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode>> __0_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode> _D_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode> _0_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _D1 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _D1(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode>> _D_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode>> _1_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode>> __D_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode>> __1_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode> _D_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode> _1_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _D2 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _D2(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode>> _D_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode>> _2_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode>> __D_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode>> __2_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode> _D_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode> _2_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _D3 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _D3(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode>> _D_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode>> _3_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode>> __D_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode>> __3_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode> _D_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode> _3_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _D4 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _D4(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode>> _D_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode>> _4_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode>> __D_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode>> __4_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode> _D_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode> _4_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _D5 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _D5(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode>> _D_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode>> _5_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode>> __D_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode>> __5_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode> _D_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode> _5_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _D6 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _D6(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode>> _D_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode>> _6_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode>> __D_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode>> __6_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode> _D_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode> _6_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _D7 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _D7(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode>> _D_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode>> _7_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode>> __D_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode>> __7_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode> _D_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode> _7_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _D8 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _D8(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode>> _D_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode>> _8_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode>> __D_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode>> __8_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode> _D_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode> _8_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _D9 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _D9(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode>> _D_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode>> _9_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode>> __D_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode>> __9_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode> _D_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode> _9_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _DA : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _DA(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode>> _D_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode>> _A_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode>> __D_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode>> __A_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode> _D_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode> _A_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _DB : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _DB(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode>> _D_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode>> _B_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode>> __D_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode>> __B_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode> _D_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode> _B_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _DC : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _DC(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode>> _D_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode>> _C_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode>> __D_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode>> __C_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode> _D_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode> _C_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _DD : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _DD(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode>> _D_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode>> _D_2, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode>> __D_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode>> __D_2 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode> _D_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode> _D_2 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _DE : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _DE(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode>> _D_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode>> _E_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode>> __D_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode>> __E_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode> _D_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode> _E_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _DF : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _DF(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode>> _D_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode>> _F_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode>> __D_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode>> __F_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode> _D_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode> _F_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _E0 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _E0(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode>> _E_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode>> _0_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode>> __E_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode>> __0_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode> _E_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode> _0_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _E1 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _E1(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode>> _E_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode>> _1_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode>> __E_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode>> __1_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode> _E_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode> _1_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _E2 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _E2(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode>> _E_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode>> _2_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode>> __E_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode>> __2_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode> _E_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode> _2_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _E3 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _E3(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode>> _E_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode>> _3_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode>> __E_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode>> __3_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode> _E_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode> _3_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _E4 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _E4(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode>> _E_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode>> _4_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode>> __E_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode>> __4_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode> _E_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode> _4_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _E5 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _E5(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode>> _E_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode>> _5_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode>> __E_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode>> __5_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode> _E_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode> _5_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _E6 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _E6(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode>> _E_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode>> _6_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode>> __E_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode>> __6_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode> _E_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode> _6_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _E7 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _E7(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode>> _E_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode>> _7_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode>> __E_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode>> __7_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode> _E_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode> _7_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _E8 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _E8(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode>> _E_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode>> _8_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode>> __E_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode>> __8_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode> _E_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode> _8_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _E9 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _E9(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode>> _E_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode>> _9_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode>> __E_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode>> __9_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode> _E_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode> _9_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _EA : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _EA(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode>> _E_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode>> _A_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode>> __E_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode>> __A_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode> _E_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode> _A_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _EB : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _EB(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode>> _E_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode>> _B_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode>> __E_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode>> __B_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode> _E_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode> _B_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _EC : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _EC(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode>> _E_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode>> _C_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode>> __E_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode>> __C_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode> _E_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode> _C_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _ED : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _ED(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode>> _E_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode>> _D_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode>> __E_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode>> __D_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode> _E_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode> _D_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _EE : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _EE(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode>> _E_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode>> _E_2, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode>> __E_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode>> __E_2 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode> _E_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode> _E_2 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _EF : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _EF(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode>> _E_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode>> _F_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode>> __E_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode>> __F_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode> _E_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode> _F_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _F0 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _F0(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode>> _F_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode>> _0_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode>> __F_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode>> __0_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode> _F_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<TMode> _0_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _F1 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _F1(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode>> _F_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode>> _1_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode>> __F_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode>> __1_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode> _F_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<TMode> _1_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _F2 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _F2(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode>> _F_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode>> _2_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode>> __F_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode>> __2_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode> _F_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<TMode> _2_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _F3 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _F3(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode>> _F_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode>> _3_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode>> __F_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode>> __3_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode> _F_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<TMode> _3_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _F4 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _F4(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode>> _F_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode>> _4_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode>> __F_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode>> __4_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode> _F_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<TMode> _4_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _F5 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _F5(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode>> _F_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode>> _5_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode>> __F_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode>> __5_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode> _F_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<TMode> _5_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _F6 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _F6(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode>> _F_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode>> _6_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode>> __F_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode>> __6_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode> _F_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<TMode> _6_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _F7 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _F7(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode>> _F_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode>> _7_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode>> __F_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode>> __7_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode> _F_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<TMode> _7_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _F8 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _F8(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode>> _F_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode>> _8_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode>> __F_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode>> __8_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode> _F_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._8<TMode> _8_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _F9 : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _F9(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode>> _F_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode>> _9_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode>> __F_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode>> __9_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode> _F_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._9<TMode> _9_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _FA : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _FA(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode>> _F_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode>> _A_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode>> __F_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode>> __A_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode> _F_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._A<TMode> _A_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _FB : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _FB(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode>> _F_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode>> _B_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode>> __F_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode>> __B_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode> _F_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._B<TMode> _B_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _FC : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _FC(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode>> _F_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode>> _C_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode>> __F_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode>> __C_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode> _F_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._C<TMode> _C_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _FD : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _FD(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode>> _F_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode>> _D_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode>> __F_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode>> __D_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode> _F_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._D<TMode> _D_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _FE : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _FE(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode>> _F_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode>> _E_1, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode>> __F_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode>> __E_1 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode> _F_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._E<TMode> _E_1 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
                {
                    throw new Exception("TODO");
                }
                
                protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    throw new Exception("TODO");
                }
            }
            
            public sealed class _FF : _Ⰳx00ⲻFF<TMode>.Realized
            {
                private _FF(IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode>> _F_1, IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode>> _F_2, ITokenStream<char>? nextTokens)
                {
                    if (typeof(TMode) != typeof(ParseMode.Realized))
                    {
                        throw new Exception("tODO");
                    }
                }
                
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode>> __F_1 { get; }
                private IFuture<__GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode>> __F_2 { get; }
                private IFuture<IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>>> realizationResult { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode> _F_1 { get; }
                public __GeneratedPartialV1.Deferred.CstNodes.Inners._F<TMode> _F_2 { get; }
                
                public override _Ⰳx00ⲻFF<ParseMode.Deferred> Convert()
                {
                    throw new Exception("TODO");
                }
                
                public override IRealizationResult<char, _Ⰳx00ⲻFF<ParseMode.Realized>> Realize()
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
