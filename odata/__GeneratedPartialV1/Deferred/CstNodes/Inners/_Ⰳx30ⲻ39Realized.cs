namespace __GeneratedPartialV1.Deferred.CstNodes.Inners
{
    using System;
    using CombinatorParsingV3;
    
    public abstract class _Ⰳx30ⲻ39Realized : IFromRealizedable<_Ⰳx30ⲻ39Deferred>
    {
        private _Ⰳx30ⲻ39Realized()
        {
        }
        
        public abstract _Ⰳx30ⲻ39Deferred Convert();
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_Ⰳx30ⲻ39Realized node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_30 node, TContext context);
            protected internal abstract TResult Accept(_31 node, TContext context);
            protected internal abstract TResult Accept(_32 node, TContext context);
            protected internal abstract TResult Accept(_33 node, TContext context);
            protected internal abstract TResult Accept(_34 node, TContext context);
            protected internal abstract TResult Accept(_35 node, TContext context);
            protected internal abstract TResult Accept(_36 node, TContext context);
            protected internal abstract TResult Accept(_37 node, TContext context);
            protected internal abstract TResult Accept(_38 node, TContext context);
            protected internal abstract TResult Accept(_39 node, TContext context);
        }
        
        public sealed class _30 : _Ⰳx30ⲻ39Realized
        {
            private _30(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx30ⲻ39Realized._30>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx30ⲻ39Realized._30> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx30ⲻ39Realized._30> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx30ⲻ39Realized._30>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx30ⲻ39Realized._30>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx30ⲻ39Realized._30(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx30ⲻ39Realized._30>(false, default, input);
}
            }
            
            public override _Ⰳx30ⲻ39Deferred Convert()
            {
                return new _Ⰳx30ⲻ39Deferred(Future.Create(() => this.RealizationResult));
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _31 : _Ⰳx30ⲻ39Realized
        {
            private _31(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx30ⲻ39Realized._31>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx30ⲻ39Realized._31> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx30ⲻ39Realized._31> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx30ⲻ39Realized._31>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx30ⲻ39Realized._31>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx30ⲻ39Realized._31(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx30ⲻ39Realized._31>(false, default, input);
}
            }
            
            public override _Ⰳx30ⲻ39Deferred Convert()
            {
                return new _Ⰳx30ⲻ39Deferred(Future.Create(() => this.RealizationResult));
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _32 : _Ⰳx30ⲻ39Realized
        {
            private _32(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx30ⲻ39Realized._32>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx30ⲻ39Realized._32> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx30ⲻ39Realized._32> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx30ⲻ39Realized._32>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx30ⲻ39Realized._32>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx30ⲻ39Realized._32(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx30ⲻ39Realized._32>(false, default, input);
}
            }
            
            public override _Ⰳx30ⲻ39Deferred Convert()
            {
                return new _Ⰳx30ⲻ39Deferred(Future.Create(() => this.RealizationResult));
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _33 : _Ⰳx30ⲻ39Realized
        {
            private _33(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx30ⲻ39Realized._33>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx30ⲻ39Realized._33> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx30ⲻ39Realized._33> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx30ⲻ39Realized._33>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx30ⲻ39Realized._33>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx30ⲻ39Realized._33(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx30ⲻ39Realized._33>(false, default, input);
}
            }
            
            public override _Ⰳx30ⲻ39Deferred Convert()
            {
                return new _Ⰳx30ⲻ39Deferred(Future.Create(() => this.RealizationResult));
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _34 : _Ⰳx30ⲻ39Realized
        {
            private _34(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx30ⲻ39Realized._34>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx30ⲻ39Realized._34> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx30ⲻ39Realized._34> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx30ⲻ39Realized._34>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx30ⲻ39Realized._34>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx30ⲻ39Realized._34(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx30ⲻ39Realized._34>(false, default, input);
}
            }
            
            public override _Ⰳx30ⲻ39Deferred Convert()
            {
                return new _Ⰳx30ⲻ39Deferred(Future.Create(() => this.RealizationResult));
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _35 : _Ⰳx30ⲻ39Realized
        {
            private _35(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx30ⲻ39Realized._35>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx30ⲻ39Realized._35> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx30ⲻ39Realized._35> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx30ⲻ39Realized._35>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx30ⲻ39Realized._35>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx30ⲻ39Realized._35(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx30ⲻ39Realized._35>(false, default, input);
}
            }
            
            public override _Ⰳx30ⲻ39Deferred Convert()
            {
                return new _Ⰳx30ⲻ39Deferred(Future.Create(() => this.RealizationResult));
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _36 : _Ⰳx30ⲻ39Realized
        {
            private _36(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx30ⲻ39Realized._36>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx30ⲻ39Realized._36> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx30ⲻ39Realized._36> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx30ⲻ39Realized._36>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx30ⲻ39Realized._36>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx30ⲻ39Realized._36(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx30ⲻ39Realized._36>(false, default, input);
}
            }
            
            public override _Ⰳx30ⲻ39Deferred Convert()
            {
                return new _Ⰳx30ⲻ39Deferred(Future.Create(() => this.RealizationResult));
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _37 : _Ⰳx30ⲻ39Realized
        {
            private _37(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx30ⲻ39Realized._37>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx30ⲻ39Realized._37> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx30ⲻ39Realized._37> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx30ⲻ39Realized._37>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx30ⲻ39Realized._37>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx30ⲻ39Realized._37(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx30ⲻ39Realized._37>(false, default, input);
}
            }
            
            public override _Ⰳx30ⲻ39Deferred Convert()
            {
                return new _Ⰳx30ⲻ39Deferred(Future.Create(() => this.RealizationResult));
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _38 : _Ⰳx30ⲻ39Realized
        {
            private _38(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx30ⲻ39Realized._38>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx30ⲻ39Realized._38> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx30ⲻ39Realized._38> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx30ⲻ39Realized._38>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx30ⲻ39Realized._38>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx30ⲻ39Realized._38(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx30ⲻ39Realized._38>(false, default, input);
}
            }
            
            public override _Ⰳx30ⲻ39Deferred Convert()
            {
                return new _Ⰳx30ⲻ39Deferred(Future.Create(() => this.RealizationResult));
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _39 : _Ⰳx30ⲻ39Realized
        {
            private _39(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx30ⲻ39Realized._39>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx30ⲻ39Realized._39> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx30ⲻ39Realized._39> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx30ⲻ39Realized._39>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx30ⲻ39Realized._39>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx30ⲻ39Realized._39(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx30ⲻ39Realized._39>(false, default, input);
}
            }
            
            public override _Ⰳx30ⲻ39Deferred Convert()
            {
                return new _Ⰳx30ⲻ39Deferred(Future.Create(() => this.RealizationResult));
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
