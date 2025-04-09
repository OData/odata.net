namespace __GeneratedPartialV1.Deferred.CstNodes.Inners
{
    using System;
    using CombinatorParsingV3;
    
    public abstract class _Ⰳx61ⲻ7ARealized
    {
        private _Ⰳx61ⲻ7ARealized()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_Ⰳx61ⲻ7ARealized node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_61 node, TContext context);
            protected internal abstract TResult Accept(_62 node, TContext context);
            protected internal abstract TResult Accept(_63 node, TContext context);
            protected internal abstract TResult Accept(_64 node, TContext context);
            protected internal abstract TResult Accept(_65 node, TContext context);
            protected internal abstract TResult Accept(_66 node, TContext context);
            protected internal abstract TResult Accept(_67 node, TContext context);
            protected internal abstract TResult Accept(_68 node, TContext context);
            protected internal abstract TResult Accept(_69 node, TContext context);
            protected internal abstract TResult Accept(_6A node, TContext context);
            protected internal abstract TResult Accept(_6B node, TContext context);
            protected internal abstract TResult Accept(_6C node, TContext context);
            protected internal abstract TResult Accept(_6D node, TContext context);
            protected internal abstract TResult Accept(_6E node, TContext context);
            protected internal abstract TResult Accept(_6F node, TContext context);
            protected internal abstract TResult Accept(_70 node, TContext context);
            protected internal abstract TResult Accept(_71 node, TContext context);
            protected internal abstract TResult Accept(_72 node, TContext context);
            protected internal abstract TResult Accept(_73 node, TContext context);
            protected internal abstract TResult Accept(_74 node, TContext context);
            protected internal abstract TResult Accept(_75 node, TContext context);
            protected internal abstract TResult Accept(_76 node, TContext context);
            protected internal abstract TResult Accept(_77 node, TContext context);
            protected internal abstract TResult Accept(_78 node, TContext context);
            protected internal abstract TResult Accept(_79 node, TContext context);
            protected internal abstract TResult Accept(_7A node, TContext context);
        }
        
        public sealed class _61 : _Ⰳx61ⲻ7ARealized
        {
            private _61(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx61ⲻ7ARealized._61>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx61ⲻ7ARealized._61> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx61ⲻ7ARealized._61> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx61ⲻ7ARealized._61>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx61ⲻ7ARealized._61>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...
{
    var a = new _Ⰳx61ⲻ7ARealized._61(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx61ⲻ7ARealized._61>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _62 : _Ⰳx61ⲻ7ARealized
        {
            private _62(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx61ⲻ7ARealized._62>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx61ⲻ7ARealized._62> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx61ⲻ7ARealized._62> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx61ⲻ7ARealized._62>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx61ⲻ7ARealized._62>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...
{
    var a = new _Ⰳx61ⲻ7ARealized._62(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx61ⲻ7ARealized._62>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _63 : _Ⰳx61ⲻ7ARealized
        {
            private _63(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx61ⲻ7ARealized._63>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx61ⲻ7ARealized._63> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx61ⲻ7ARealized._63> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx61ⲻ7ARealized._63>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx61ⲻ7ARealized._63>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...
{
    var a = new _Ⰳx61ⲻ7ARealized._63(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx61ⲻ7ARealized._63>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _64 : _Ⰳx61ⲻ7ARealized
        {
            private _64(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx61ⲻ7ARealized._64>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx61ⲻ7ARealized._64> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx61ⲻ7ARealized._64> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx61ⲻ7ARealized._64>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx61ⲻ7ARealized._64>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...
{
    var a = new _Ⰳx61ⲻ7ARealized._64(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx61ⲻ7ARealized._64>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _65 : _Ⰳx61ⲻ7ARealized
        {
            private _65(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx61ⲻ7ARealized._65>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx61ⲻ7ARealized._65> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx61ⲻ7ARealized._65> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx61ⲻ7ARealized._65>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx61ⲻ7ARealized._65>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...
{
    var a = new _Ⰳx61ⲻ7ARealized._65(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx61ⲻ7ARealized._65>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _66 : _Ⰳx61ⲻ7ARealized
        {
            private _66(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx61ⲻ7ARealized._66>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx61ⲻ7ARealized._66> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx61ⲻ7ARealized._66> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx61ⲻ7ARealized._66>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx61ⲻ7ARealized._66>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...
{
    var a = new _Ⰳx61ⲻ7ARealized._66(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx61ⲻ7ARealized._66>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _67 : _Ⰳx61ⲻ7ARealized
        {
            private _67(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx61ⲻ7ARealized._67>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx61ⲻ7ARealized._67> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx61ⲻ7ARealized._67> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx61ⲻ7ARealized._67>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx61ⲻ7ARealized._67>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...
{
    var a = new _Ⰳx61ⲻ7ARealized._67(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx61ⲻ7ARealized._67>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _68 : _Ⰳx61ⲻ7ARealized
        {
            private _68(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx61ⲻ7ARealized._68>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx61ⲻ7ARealized._68> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx61ⲻ7ARealized._68> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx61ⲻ7ARealized._68>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx61ⲻ7ARealized._68>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...
{
    var a = new _Ⰳx61ⲻ7ARealized._68(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx61ⲻ7ARealized._68>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _69 : _Ⰳx61ⲻ7ARealized
        {
            private _69(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx61ⲻ7ARealized._69>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx61ⲻ7ARealized._69> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx61ⲻ7ARealized._69> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx61ⲻ7ARealized._69>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx61ⲻ7ARealized._69>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...
{
    var a = new _Ⰳx61ⲻ7ARealized._69(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx61ⲻ7ARealized._69>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _6A : _Ⰳx61ⲻ7ARealized
        {
            private _6A(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx61ⲻ7ARealized._6A>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx61ⲻ7ARealized._6A> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx61ⲻ7ARealized._6A> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx61ⲻ7ARealized._6A>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx61ⲻ7ARealized._6A>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...
{
    var a = new _Ⰳx61ⲻ7ARealized._6A(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx61ⲻ7ARealized._6A>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _6B : _Ⰳx61ⲻ7ARealized
        {
            private _6B(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx61ⲻ7ARealized._6B>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx61ⲻ7ARealized._6B> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx61ⲻ7ARealized._6B> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx61ⲻ7ARealized._6B>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx61ⲻ7ARealized._6B>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...
{
    var a = new _Ⰳx61ⲻ7ARealized._6B(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx61ⲻ7ARealized._6B>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _6C : _Ⰳx61ⲻ7ARealized
        {
            private _6C(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx61ⲻ7ARealized._6C>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx61ⲻ7ARealized._6C> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx61ⲻ7ARealized._6C> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx61ⲻ7ARealized._6C>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx61ⲻ7ARealized._6C>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...
{
    var a = new _Ⰳx61ⲻ7ARealized._6C(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx61ⲻ7ARealized._6C>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _6D : _Ⰳx61ⲻ7ARealized
        {
            private _6D(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx61ⲻ7ARealized._6D>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx61ⲻ7ARealized._6D> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx61ⲻ7ARealized._6D> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx61ⲻ7ARealized._6D>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx61ⲻ7ARealized._6D>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...
{
    var a = new _Ⰳx61ⲻ7ARealized._6D(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx61ⲻ7ARealized._6D>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _6E : _Ⰳx61ⲻ7ARealized
        {
            private _6E(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx61ⲻ7ARealized._6E>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx61ⲻ7ARealized._6E> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx61ⲻ7ARealized._6E> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx61ⲻ7ARealized._6E>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx61ⲻ7ARealized._6E>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...
{
    var a = new _Ⰳx61ⲻ7ARealized._6E(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx61ⲻ7ARealized._6E>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _6F : _Ⰳx61ⲻ7ARealized
        {
            private _6F(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx61ⲻ7ARealized._6F>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx61ⲻ7ARealized._6F> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx61ⲻ7ARealized._6F> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx61ⲻ7ARealized._6F>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx61ⲻ7ARealized._6F>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...
{
    var a = new _Ⰳx61ⲻ7ARealized._6F(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx61ⲻ7ARealized._6F>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _70 : _Ⰳx61ⲻ7ARealized
        {
            private _70(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx61ⲻ7ARealized._70>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx61ⲻ7ARealized._70> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx61ⲻ7ARealized._70> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx61ⲻ7ARealized._70>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx61ⲻ7ARealized._70>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...
{
    var a = new _Ⰳx61ⲻ7ARealized._70(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx61ⲻ7ARealized._70>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _71 : _Ⰳx61ⲻ7ARealized
        {
            private _71(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx61ⲻ7ARealized._71>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx61ⲻ7ARealized._71> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx61ⲻ7ARealized._71> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx61ⲻ7ARealized._71>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx61ⲻ7ARealized._71>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...
{
    var a = new _Ⰳx61ⲻ7ARealized._71(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx61ⲻ7ARealized._71>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _72 : _Ⰳx61ⲻ7ARealized
        {
            private _72(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx61ⲻ7ARealized._72>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx61ⲻ7ARealized._72> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx61ⲻ7ARealized._72> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx61ⲻ7ARealized._72>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx61ⲻ7ARealized._72>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...
{
    var a = new _Ⰳx61ⲻ7ARealized._72(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx61ⲻ7ARealized._72>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _73 : _Ⰳx61ⲻ7ARealized
        {
            private _73(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx61ⲻ7ARealized._73>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx61ⲻ7ARealized._73> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx61ⲻ7ARealized._73> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx61ⲻ7ARealized._73>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx61ⲻ7ARealized._73>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...
{
    var a = new _Ⰳx61ⲻ7ARealized._73(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx61ⲻ7ARealized._73>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _74 : _Ⰳx61ⲻ7ARealized
        {
            private _74(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx61ⲻ7ARealized._74>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx61ⲻ7ARealized._74> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx61ⲻ7ARealized._74> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx61ⲻ7ARealized._74>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx61ⲻ7ARealized._74>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...
{
    var a = new _Ⰳx61ⲻ7ARealized._74(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx61ⲻ7ARealized._74>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _75 : _Ⰳx61ⲻ7ARealized
        {
            private _75(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx61ⲻ7ARealized._75>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx61ⲻ7ARealized._75> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx61ⲻ7ARealized._75> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx61ⲻ7ARealized._75>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx61ⲻ7ARealized._75>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...
{
    var a = new _Ⰳx61ⲻ7ARealized._75(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx61ⲻ7ARealized._75>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _76 : _Ⰳx61ⲻ7ARealized
        {
            private _76(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx61ⲻ7ARealized._76>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx61ⲻ7ARealized._76> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx61ⲻ7ARealized._76> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx61ⲻ7ARealized._76>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx61ⲻ7ARealized._76>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...
{
    var a = new _Ⰳx61ⲻ7ARealized._76(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx61ⲻ7ARealized._76>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _77 : _Ⰳx61ⲻ7ARealized
        {
            private _77(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx61ⲻ7ARealized._77>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx61ⲻ7ARealized._77> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx61ⲻ7ARealized._77> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx61ⲻ7ARealized._77>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx61ⲻ7ARealized._77>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...
{
    var a = new _Ⰳx61ⲻ7ARealized._77(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx61ⲻ7ARealized._77>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _78 : _Ⰳx61ⲻ7ARealized
        {
            private _78(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx61ⲻ7ARealized._78>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx61ⲻ7ARealized._78> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx61ⲻ7ARealized._78> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx61ⲻ7ARealized._78>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx61ⲻ7ARealized._78>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...
{
    var a = new _Ⰳx61ⲻ7ARealized._78(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx61ⲻ7ARealized._78>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _79 : _Ⰳx61ⲻ7ARealized
        {
            private _79(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx61ⲻ7ARealized._79>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx61ⲻ7ARealized._79> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx61ⲻ7ARealized._79> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx61ⲻ7ARealized._79>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx61ⲻ7ARealized._79>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...
{
    var a = new _Ⰳx61ⲻ7ARealized._79(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx61ⲻ7ARealized._79>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _7A : _Ⰳx61ⲻ7ARealized
        {
            private _7A(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx61ⲻ7ARealized._7A>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx61ⲻ7ARealized._7A> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx61ⲻ7ARealized._7A> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx61ⲻ7ARealized._7A>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx61ⲻ7ARealized._7A>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...
{
    var a = new _Ⰳx61ⲻ7ARealized._7A(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx61ⲻ7ARealized._7A>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
