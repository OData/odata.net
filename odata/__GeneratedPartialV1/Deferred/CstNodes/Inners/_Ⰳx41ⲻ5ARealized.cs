namespace __GeneratedPartialV1.Deferred.CstNodes.Inners
{
    using System;
    using CombinatorParsingV3;
    
    public abstract class _Ⰳx41ⲻ5ARealized : IFromRealizedable<_Ⰳx41ⲻ5ADeferred>
    {
        private _Ⰳx41ⲻ5ARealized()
        {
        }
        
        public abstract _Ⰳx41ⲻ5ADeferred Convert();
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_Ⰳx41ⲻ5ARealized node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_41 node, TContext context);
            protected internal abstract TResult Accept(_42 node, TContext context);
            protected internal abstract TResult Accept(_43 node, TContext context);
            protected internal abstract TResult Accept(_44 node, TContext context);
            protected internal abstract TResult Accept(_45 node, TContext context);
            protected internal abstract TResult Accept(_46 node, TContext context);
            protected internal abstract TResult Accept(_47 node, TContext context);
            protected internal abstract TResult Accept(_48 node, TContext context);
            protected internal abstract TResult Accept(_49 node, TContext context);
            protected internal abstract TResult Accept(_4A node, TContext context);
            protected internal abstract TResult Accept(_4B node, TContext context);
            protected internal abstract TResult Accept(_4C node, TContext context);
            protected internal abstract TResult Accept(_4D node, TContext context);
            protected internal abstract TResult Accept(_4E node, TContext context);
            protected internal abstract TResult Accept(_4F node, TContext context);
            protected internal abstract TResult Accept(_50 node, TContext context);
            protected internal abstract TResult Accept(_51 node, TContext context);
            protected internal abstract TResult Accept(_52 node, TContext context);
            protected internal abstract TResult Accept(_53 node, TContext context);
            protected internal abstract TResult Accept(_54 node, TContext context);
            protected internal abstract TResult Accept(_55 node, TContext context);
            protected internal abstract TResult Accept(_56 node, TContext context);
            protected internal abstract TResult Accept(_57 node, TContext context);
            protected internal abstract TResult Accept(_58 node, TContext context);
            protected internal abstract TResult Accept(_59 node, TContext context);
            protected internal abstract TResult Accept(_5A node, TContext context);
        }
        
        public sealed class _41 : _Ⰳx41ⲻ5ARealized
        {
            private _41(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx41ⲻ5ARealized._41>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx41ⲻ5ARealized._41> RealizationResult { get; }
            public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1 { get; }
            public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<ParseMode.Realized> _1_1 { get; }
            
            public static IRealizationResult<char, _Ⰳx41ⲻ5ARealized._41> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx41ⲻ5ARealized._41>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx41ⲻ5ARealized._41>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx41ⲻ5ARealized._41(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx41ⲻ5ARealized._41>(false, default, input);
}
            }
            
            public override _Ⰳx41ⲻ5ADeferred Convert()
            {
                return new _Ⰳx41ⲻ5ADeferred(Future.Create(() => this.RealizationResult));
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _42 : _Ⰳx41ⲻ5ARealized
        {
            private _42(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx41ⲻ5ARealized._42>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx41ⲻ5ARealized._42> RealizationResult { get; }
            public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1 { get; }
            public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1 { get; }
            
            public static IRealizationResult<char, _Ⰳx41ⲻ5ARealized._42> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx41ⲻ5ARealized._42>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx41ⲻ5ARealized._42>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx41ⲻ5ARealized._42(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx41ⲻ5ARealized._42>(false, default, input);
}
            }
            
            public override _Ⰳx41ⲻ5ADeferred Convert()
            {
                return new _Ⰳx41ⲻ5ADeferred(Future.Create(() => this.RealizationResult));
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _43 : _Ⰳx41ⲻ5ARealized
        {
            private _43(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx41ⲻ5ARealized._43>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx41ⲻ5ARealized._43> RealizationResult { get; }
            public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1 { get; }
            public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1 { get; }
            
            public static IRealizationResult<char, _Ⰳx41ⲻ5ARealized._43> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx41ⲻ5ARealized._43>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx41ⲻ5ARealized._43>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx41ⲻ5ARealized._43(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx41ⲻ5ARealized._43>(false, default, input);
}
            }
            
            public override _Ⰳx41ⲻ5ADeferred Convert()
            {
                return new _Ⰳx41ⲻ5ADeferred(Future.Create(() => this.RealizationResult));
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _44 : _Ⰳx41ⲻ5ARealized
        {
            private _44(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx41ⲻ5ARealized._44>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx41ⲻ5ARealized._44> RealizationResult { get; }
            public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1 { get; }
            public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_2 { get; }
            
            public static IRealizationResult<char, _Ⰳx41ⲻ5ARealized._44> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx41ⲻ5ARealized._44>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx41ⲻ5ARealized._44>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx41ⲻ5ARealized._44(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx41ⲻ5ARealized._44>(false, default, input);
}
            }
            
            public override _Ⰳx41ⲻ5ADeferred Convert()
            {
                return new _Ⰳx41ⲻ5ADeferred(Future.Create(() => this.RealizationResult));
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _45 : _Ⰳx41ⲻ5ARealized
        {
            private _45(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx41ⲻ5ARealized._45>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx41ⲻ5ARealized._45> RealizationResult { get; }
            public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1 { get; }
            public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1 { get; }
            
            public static IRealizationResult<char, _Ⰳx41ⲻ5ARealized._45> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx41ⲻ5ARealized._45>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx41ⲻ5ARealized._45>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx41ⲻ5ARealized._45(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx41ⲻ5ARealized._45>(false, default, input);
}
            }
            
            public override _Ⰳx41ⲻ5ADeferred Convert()
            {
                return new _Ⰳx41ⲻ5ADeferred(Future.Create(() => this.RealizationResult));
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _46 : _Ⰳx41ⲻ5ARealized
        {
            private _46(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx41ⲻ5ARealized._46>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx41ⲻ5ARealized._46> RealizationResult { get; }
            public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1 { get; }
            public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1 { get; }
            
            public static IRealizationResult<char, _Ⰳx41ⲻ5ARealized._46> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx41ⲻ5ARealized._46>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx41ⲻ5ARealized._46>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx41ⲻ5ARealized._46(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx41ⲻ5ARealized._46>(false, default, input);
}
            }
            
            public override _Ⰳx41ⲻ5ADeferred Convert()
            {
                return new _Ⰳx41ⲻ5ADeferred(Future.Create(() => this.RealizationResult));
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _47 : _Ⰳx41ⲻ5ARealized
        {
            private _47(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx41ⲻ5ARealized._47>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx41ⲻ5ARealized._47> RealizationResult { get; }
            public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1 { get; }
            public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1 { get; }
            
            public static IRealizationResult<char, _Ⰳx41ⲻ5ARealized._47> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx41ⲻ5ARealized._47>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx41ⲻ5ARealized._47>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx41ⲻ5ARealized._47(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx41ⲻ5ARealized._47>(false, default, input);
}
            }
            
            public override _Ⰳx41ⲻ5ADeferred Convert()
            {
                return new _Ⰳx41ⲻ5ADeferred(Future.Create(() => this.RealizationResult));
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _48 : _Ⰳx41ⲻ5ARealized
        {
            private _48(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx41ⲻ5ARealized._48>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx41ⲻ5ARealized._48> RealizationResult { get; }
            public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1 { get; }
            public __GeneratedPartialV1.Deferred.CstNodes.Inners._8<ParseMode.Realized> _8_1 { get; }
            
            public static IRealizationResult<char, _Ⰳx41ⲻ5ARealized._48> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx41ⲻ5ARealized._48>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx41ⲻ5ARealized._48>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx41ⲻ5ARealized._48(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx41ⲻ5ARealized._48>(false, default, input);
}
            }
            
            public override _Ⰳx41ⲻ5ADeferred Convert()
            {
                return new _Ⰳx41ⲻ5ADeferred(Future.Create(() => this.RealizationResult));
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _49 : _Ⰳx41ⲻ5ARealized
        {
            private _49(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx41ⲻ5ARealized._49>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx41ⲻ5ARealized._49> RealizationResult { get; }
            public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1 { get; }
            public __GeneratedPartialV1.Deferred.CstNodes.Inners._9<ParseMode.Realized> _9_1 { get; }
            
            public static IRealizationResult<char, _Ⰳx41ⲻ5ARealized._49> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx41ⲻ5ARealized._49>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx41ⲻ5ARealized._49>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx41ⲻ5ARealized._49(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx41ⲻ5ARealized._49>(false, default, input);
}
            }
            
            public override _Ⰳx41ⲻ5ADeferred Convert()
            {
                return new _Ⰳx41ⲻ5ADeferred(Future.Create(() => this.RealizationResult));
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _4A : _Ⰳx41ⲻ5ARealized
        {
            private _4A(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx41ⲻ5ARealized._4A>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx41ⲻ5ARealized._4A> RealizationResult { get; }
            public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1 { get; }
            public __GeneratedPartialV1.Deferred.CstNodes.Inners._A<ParseMode.Realized> _A_1 { get; }
            
            public static IRealizationResult<char, _Ⰳx41ⲻ5ARealized._4A> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx41ⲻ5ARealized._4A>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx41ⲻ5ARealized._4A>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx41ⲻ5ARealized._4A(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx41ⲻ5ARealized._4A>(false, default, input);
}
            }
            
            public override _Ⰳx41ⲻ5ADeferred Convert()
            {
                return new _Ⰳx41ⲻ5ADeferred(Future.Create(() => this.RealizationResult));
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _4B : _Ⰳx41ⲻ5ARealized
        {
            private _4B(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx41ⲻ5ARealized._4B>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx41ⲻ5ARealized._4B> RealizationResult { get; }
            public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1 { get; }
            public __GeneratedPartialV1.Deferred.CstNodes.Inners._B<ParseMode.Realized> _B_1 { get; }
            
            public static IRealizationResult<char, _Ⰳx41ⲻ5ARealized._4B> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx41ⲻ5ARealized._4B>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx41ⲻ5ARealized._4B>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx41ⲻ5ARealized._4B(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx41ⲻ5ARealized._4B>(false, default, input);
}
            }
            
            public override _Ⰳx41ⲻ5ADeferred Convert()
            {
                return new _Ⰳx41ⲻ5ADeferred(Future.Create(() => this.RealizationResult));
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _4C : _Ⰳx41ⲻ5ARealized
        {
            private _4C(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx41ⲻ5ARealized._4C>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx41ⲻ5ARealized._4C> RealizationResult { get; }
            public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1 { get; }
            public __GeneratedPartialV1.Deferred.CstNodes.Inners._C<ParseMode.Realized> _C_1 { get; }
            
            public static IRealizationResult<char, _Ⰳx41ⲻ5ARealized._4C> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx41ⲻ5ARealized._4C>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx41ⲻ5ARealized._4C>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx41ⲻ5ARealized._4C(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx41ⲻ5ARealized._4C>(false, default, input);
}
            }
            
            public override _Ⰳx41ⲻ5ADeferred Convert()
            {
                return new _Ⰳx41ⲻ5ADeferred(Future.Create(() => this.RealizationResult));
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _4D : _Ⰳx41ⲻ5ARealized
        {
            private _4D(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx41ⲻ5ARealized._4D>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx41ⲻ5ARealized._4D> RealizationResult { get; }
            public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1 { get; }
            public __GeneratedPartialV1.Deferred.CstNodes.Inners._D<ParseMode.Realized> _D_1 { get; }
            
            public static IRealizationResult<char, _Ⰳx41ⲻ5ARealized._4D> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx41ⲻ5ARealized._4D>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx41ⲻ5ARealized._4D>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx41ⲻ5ARealized._4D(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx41ⲻ5ARealized._4D>(false, default, input);
}
            }
            
            public override _Ⰳx41ⲻ5ADeferred Convert()
            {
                return new _Ⰳx41ⲻ5ADeferred(Future.Create(() => this.RealizationResult));
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _4E : _Ⰳx41ⲻ5ARealized
        {
            private _4E(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx41ⲻ5ARealized._4E>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx41ⲻ5ARealized._4E> RealizationResult { get; }
            public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1 { get; }
            public __GeneratedPartialV1.Deferred.CstNodes.Inners._E<ParseMode.Realized> _E_1 { get; }
            
            public static IRealizationResult<char, _Ⰳx41ⲻ5ARealized._4E> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx41ⲻ5ARealized._4E>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx41ⲻ5ARealized._4E>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx41ⲻ5ARealized._4E(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx41ⲻ5ARealized._4E>(false, default, input);
}
            }
            
            public override _Ⰳx41ⲻ5ADeferred Convert()
            {
                return new _Ⰳx41ⲻ5ADeferred(Future.Create(() => this.RealizationResult));
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _4F : _Ⰳx41ⲻ5ARealized
        {
            private _4F(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx41ⲻ5ARealized._4F>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx41ⲻ5ARealized._4F> RealizationResult { get; }
            public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1 { get; }
            public __GeneratedPartialV1.Deferred.CstNodes.Inners._F<ParseMode.Realized> _F_1 { get; }
            
            public static IRealizationResult<char, _Ⰳx41ⲻ5ARealized._4F> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx41ⲻ5ARealized._4F>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx41ⲻ5ARealized._4F>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx41ⲻ5ARealized._4F(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx41ⲻ5ARealized._4F>(false, default, input);
}
            }
            
            public override _Ⰳx41ⲻ5ADeferred Convert()
            {
                return new _Ⰳx41ⲻ5ADeferred(Future.Create(() => this.RealizationResult));
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _50 : _Ⰳx41ⲻ5ARealized
        {
            private _50(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx41ⲻ5ARealized._50>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx41ⲻ5ARealized._50> RealizationResult { get; }
            public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1 { get; }
            public __GeneratedPartialV1.Deferred.CstNodes.Inners._0<ParseMode.Realized> _0_1 { get; }
            
            public static IRealizationResult<char, _Ⰳx41ⲻ5ARealized._50> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx41ⲻ5ARealized._50>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx41ⲻ5ARealized._50>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx41ⲻ5ARealized._50(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx41ⲻ5ARealized._50>(false, default, input);
}
            }
            
            public override _Ⰳx41ⲻ5ADeferred Convert()
            {
                return new _Ⰳx41ⲻ5ADeferred(Future.Create(() => this.RealizationResult));
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _51 : _Ⰳx41ⲻ5ARealized
        {
            private _51(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx41ⲻ5ARealized._51>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx41ⲻ5ARealized._51> RealizationResult { get; }
            public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1 { get; }
            public __GeneratedPartialV1.Deferred.CstNodes.Inners._1<ParseMode.Realized> _1_1 { get; }
            
            public static IRealizationResult<char, _Ⰳx41ⲻ5ARealized._51> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx41ⲻ5ARealized._51>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx41ⲻ5ARealized._51>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx41ⲻ5ARealized._51(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx41ⲻ5ARealized._51>(false, default, input);
}
            }
            
            public override _Ⰳx41ⲻ5ADeferred Convert()
            {
                return new _Ⰳx41ⲻ5ADeferred(Future.Create(() => this.RealizationResult));
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _52 : _Ⰳx41ⲻ5ARealized
        {
            private _52(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx41ⲻ5ARealized._52>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx41ⲻ5ARealized._52> RealizationResult { get; }
            public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1 { get; }
            public __GeneratedPartialV1.Deferred.CstNodes.Inners._2<ParseMode.Realized> _2_1 { get; }
            
            public static IRealizationResult<char, _Ⰳx41ⲻ5ARealized._52> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx41ⲻ5ARealized._52>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx41ⲻ5ARealized._52>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx41ⲻ5ARealized._52(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx41ⲻ5ARealized._52>(false, default, input);
}
            }
            
            public override _Ⰳx41ⲻ5ADeferred Convert()
            {
                return new _Ⰳx41ⲻ5ADeferred(Future.Create(() => this.RealizationResult));
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _53 : _Ⰳx41ⲻ5ARealized
        {
            private _53(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx41ⲻ5ARealized._53>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx41ⲻ5ARealized._53> RealizationResult { get; }
            public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1 { get; }
            public __GeneratedPartialV1.Deferred.CstNodes.Inners._3<ParseMode.Realized> _3_1 { get; }
            
            public static IRealizationResult<char, _Ⰳx41ⲻ5ARealized._53> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx41ⲻ5ARealized._53>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx41ⲻ5ARealized._53>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx41ⲻ5ARealized._53(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx41ⲻ5ARealized._53>(false, default, input);
}
            }
            
            public override _Ⰳx41ⲻ5ADeferred Convert()
            {
                return new _Ⰳx41ⲻ5ADeferred(Future.Create(() => this.RealizationResult));
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _54 : _Ⰳx41ⲻ5ARealized
        {
            private _54(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx41ⲻ5ARealized._54>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx41ⲻ5ARealized._54> RealizationResult { get; }
            public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1 { get; }
            public __GeneratedPartialV1.Deferred.CstNodes.Inners._4<ParseMode.Realized> _4_1 { get; }
            
            public static IRealizationResult<char, _Ⰳx41ⲻ5ARealized._54> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx41ⲻ5ARealized._54>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx41ⲻ5ARealized._54>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx41ⲻ5ARealized._54(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx41ⲻ5ARealized._54>(false, default, input);
}
            }
            
            public override _Ⰳx41ⲻ5ADeferred Convert()
            {
                return new _Ⰳx41ⲻ5ADeferred(Future.Create(() => this.RealizationResult));
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _55 : _Ⰳx41ⲻ5ARealized
        {
            private _55(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx41ⲻ5ARealized._55>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx41ⲻ5ARealized._55> RealizationResult { get; }
            public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1 { get; }
            public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_2 { get; }
            
            public static IRealizationResult<char, _Ⰳx41ⲻ5ARealized._55> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx41ⲻ5ARealized._55>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx41ⲻ5ARealized._55>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx41ⲻ5ARealized._55(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx41ⲻ5ARealized._55>(false, default, input);
}
            }
            
            public override _Ⰳx41ⲻ5ADeferred Convert()
            {
                return new _Ⰳx41ⲻ5ADeferred(Future.Create(() => this.RealizationResult));
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _56 : _Ⰳx41ⲻ5ARealized
        {
            private _56(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx41ⲻ5ARealized._56>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx41ⲻ5ARealized._56> RealizationResult { get; }
            public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1 { get; }
            public __GeneratedPartialV1.Deferred.CstNodes.Inners._6<ParseMode.Realized> _6_1 { get; }
            
            public static IRealizationResult<char, _Ⰳx41ⲻ5ARealized._56> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx41ⲻ5ARealized._56>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx41ⲻ5ARealized._56>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx41ⲻ5ARealized._56(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx41ⲻ5ARealized._56>(false, default, input);
}
            }
            
            public override _Ⰳx41ⲻ5ADeferred Convert()
            {
                return new _Ⰳx41ⲻ5ADeferred(Future.Create(() => this.RealizationResult));
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _57 : _Ⰳx41ⲻ5ARealized
        {
            private _57(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx41ⲻ5ARealized._57>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx41ⲻ5ARealized._57> RealizationResult { get; }
            public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1 { get; }
            public __GeneratedPartialV1.Deferred.CstNodes.Inners._7<ParseMode.Realized> _7_1 { get; }
            
            public static IRealizationResult<char, _Ⰳx41ⲻ5ARealized._57> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx41ⲻ5ARealized._57>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx41ⲻ5ARealized._57>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx41ⲻ5ARealized._57(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx41ⲻ5ARealized._57>(false, default, input);
}
            }
            
            public override _Ⰳx41ⲻ5ADeferred Convert()
            {
                return new _Ⰳx41ⲻ5ADeferred(Future.Create(() => this.RealizationResult));
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _58 : _Ⰳx41ⲻ5ARealized
        {
            private _58(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx41ⲻ5ARealized._58>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx41ⲻ5ARealized._58> RealizationResult { get; }
            public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1 { get; }
            public __GeneratedPartialV1.Deferred.CstNodes.Inners._8<ParseMode.Realized> _8_1 { get; }
            
            public static IRealizationResult<char, _Ⰳx41ⲻ5ARealized._58> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx41ⲻ5ARealized._58>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx41ⲻ5ARealized._58>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx41ⲻ5ARealized._58(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx41ⲻ5ARealized._58>(false, default, input);
}
            }
            
            public override _Ⰳx41ⲻ5ADeferred Convert()
            {
                return new _Ⰳx41ⲻ5ADeferred(Future.Create(() => this.RealizationResult));
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _59 : _Ⰳx41ⲻ5ARealized
        {
            private _59(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx41ⲻ5ARealized._59>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx41ⲻ5ARealized._59> RealizationResult { get; }
            public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1 { get; }
            public __GeneratedPartialV1.Deferred.CstNodes.Inners._9<ParseMode.Realized> _9_1 { get; }
            
            public static IRealizationResult<char, _Ⰳx41ⲻ5ARealized._59> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx41ⲻ5ARealized._59>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx41ⲻ5ARealized._59>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx41ⲻ5ARealized._59(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx41ⲻ5ARealized._59>(false, default, input);
}
            }
            
            public override _Ⰳx41ⲻ5ADeferred Convert()
            {
                return new _Ⰳx41ⲻ5ADeferred(Future.Create(() => this.RealizationResult));
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _5A : _Ⰳx41ⲻ5ARealized
        {
            private _5A(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx41ⲻ5ARealized._5A>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx41ⲻ5ARealized._5A> RealizationResult { get; }
            public __GeneratedPartialV1.Deferred.CstNodes.Inners._5<ParseMode.Realized> _5_1 { get; }
            public __GeneratedPartialV1.Deferred.CstNodes.Inners._A<ParseMode.Realized> _A_1 { get; }
            
            public static IRealizationResult<char, _Ⰳx41ⲻ5ARealized._5A> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx41ⲻ5ARealized._5A>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx41ⲻ5ARealized._5A>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx41ⲻ5ARealized._5A(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx41ⲻ5ARealized._5A>(false, default, input);
}
            }
            
            public override _Ⰳx41ⲻ5ADeferred Convert()
            {
                return new _Ⰳx41ⲻ5ADeferred(Future.Create(() => this.RealizationResult));
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
