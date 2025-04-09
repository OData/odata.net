namespace __GeneratedPartialV1.Deferred.CstNodes.Inners
{
    using System;
    using CombinatorParsingV3;
    
    public abstract class _Ⰳx21ⲻ7ERealized
    {
        private _Ⰳx21ⲻ7ERealized()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_Ⰳx21ⲻ7ERealized node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_21 node, TContext context);
            protected internal abstract TResult Accept(_22 node, TContext context);
            protected internal abstract TResult Accept(_23 node, TContext context);
            protected internal abstract TResult Accept(_24 node, TContext context);
            protected internal abstract TResult Accept(_25 node, TContext context);
            protected internal abstract TResult Accept(_26 node, TContext context);
            protected internal abstract TResult Accept(_27 node, TContext context);
            protected internal abstract TResult Accept(_28 node, TContext context);
            protected internal abstract TResult Accept(_29 node, TContext context);
            protected internal abstract TResult Accept(_2A node, TContext context);
            protected internal abstract TResult Accept(_2B node, TContext context);
            protected internal abstract TResult Accept(_2C node, TContext context);
            protected internal abstract TResult Accept(_2D node, TContext context);
            protected internal abstract TResult Accept(_2E node, TContext context);
            protected internal abstract TResult Accept(_2F node, TContext context);
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
            protected internal abstract TResult Accept(_3A node, TContext context);
            protected internal abstract TResult Accept(_3B node, TContext context);
            protected internal abstract TResult Accept(_3C node, TContext context);
            protected internal abstract TResult Accept(_3D node, TContext context);
            protected internal abstract TResult Accept(_3E node, TContext context);
            protected internal abstract TResult Accept(_3F node, TContext context);
            protected internal abstract TResult Accept(_40 node, TContext context);
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
            protected internal abstract TResult Accept(_5B node, TContext context);
            protected internal abstract TResult Accept(_5C node, TContext context);
            protected internal abstract TResult Accept(_5D node, TContext context);
            protected internal abstract TResult Accept(_5E node, TContext context);
            protected internal abstract TResult Accept(_5F node, TContext context);
            protected internal abstract TResult Accept(_60 node, TContext context);
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
            protected internal abstract TResult Accept(_7B node, TContext context);
            protected internal abstract TResult Accept(_7C node, TContext context);
            protected internal abstract TResult Accept(_7D node, TContext context);
            protected internal abstract TResult Accept(_7E node, TContext context);
        }
        
        public sealed class _21 : _Ⰳx21ⲻ7ERealized
        {
            private _21(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx21ⲻ7ERealized._21>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7ERealized._21> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx21ⲻ7ERealized._21> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._21>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._21>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx21ⲻ7ERealized._21(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._21>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _22 : _Ⰳx21ⲻ7ERealized
        {
            private _22(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx21ⲻ7ERealized._22>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7ERealized._22> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx21ⲻ7ERealized._22> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._22>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._22>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx21ⲻ7ERealized._22(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._22>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _23 : _Ⰳx21ⲻ7ERealized
        {
            private _23(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx21ⲻ7ERealized._23>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7ERealized._23> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx21ⲻ7ERealized._23> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._23>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._23>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx21ⲻ7ERealized._23(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._23>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _24 : _Ⰳx21ⲻ7ERealized
        {
            private _24(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx21ⲻ7ERealized._24>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7ERealized._24> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx21ⲻ7ERealized._24> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._24>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._24>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx21ⲻ7ERealized._24(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._24>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _25 : _Ⰳx21ⲻ7ERealized
        {
            private _25(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx21ⲻ7ERealized._25>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7ERealized._25> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx21ⲻ7ERealized._25> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._25>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._25>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx21ⲻ7ERealized._25(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._25>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _26 : _Ⰳx21ⲻ7ERealized
        {
            private _26(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx21ⲻ7ERealized._26>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7ERealized._26> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx21ⲻ7ERealized._26> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._26>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._26>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx21ⲻ7ERealized._26(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._26>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _27 : _Ⰳx21ⲻ7ERealized
        {
            private _27(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx21ⲻ7ERealized._27>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7ERealized._27> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx21ⲻ7ERealized._27> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._27>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._27>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx21ⲻ7ERealized._27(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._27>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _28 : _Ⰳx21ⲻ7ERealized
        {
            private _28(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx21ⲻ7ERealized._28>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7ERealized._28> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx21ⲻ7ERealized._28> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._28>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._28>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx21ⲻ7ERealized._28(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._28>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _29 : _Ⰳx21ⲻ7ERealized
        {
            private _29(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx21ⲻ7ERealized._29>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7ERealized._29> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx21ⲻ7ERealized._29> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._29>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._29>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx21ⲻ7ERealized._29(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._29>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _2A : _Ⰳx21ⲻ7ERealized
        {
            private _2A(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx21ⲻ7ERealized._2A>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7ERealized._2A> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx21ⲻ7ERealized._2A> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._2A>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._2A>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx21ⲻ7ERealized._2A(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._2A>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _2B : _Ⰳx21ⲻ7ERealized
        {
            private _2B(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx21ⲻ7ERealized._2B>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7ERealized._2B> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx21ⲻ7ERealized._2B> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._2B>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._2B>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx21ⲻ7ERealized._2B(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._2B>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _2C : _Ⰳx21ⲻ7ERealized
        {
            private _2C(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx21ⲻ7ERealized._2C>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7ERealized._2C> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx21ⲻ7ERealized._2C> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._2C>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._2C>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx21ⲻ7ERealized._2C(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._2C>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _2D : _Ⰳx21ⲻ7ERealized
        {
            private _2D(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx21ⲻ7ERealized._2D>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7ERealized._2D> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx21ⲻ7ERealized._2D> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._2D>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._2D>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx21ⲻ7ERealized._2D(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._2D>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _2E : _Ⰳx21ⲻ7ERealized
        {
            private _2E(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx21ⲻ7ERealized._2E>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7ERealized._2E> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx21ⲻ7ERealized._2E> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._2E>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._2E>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx21ⲻ7ERealized._2E(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._2E>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _2F : _Ⰳx21ⲻ7ERealized
        {
            private _2F(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx21ⲻ7ERealized._2F>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7ERealized._2F> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx21ⲻ7ERealized._2F> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._2F>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._2F>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx21ⲻ7ERealized._2F(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._2F>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _30 : _Ⰳx21ⲻ7ERealized
        {
            private _30(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx21ⲻ7ERealized._30>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7ERealized._30> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx21ⲻ7ERealized._30> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._30>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._30>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx21ⲻ7ERealized._30(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._30>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _31 : _Ⰳx21ⲻ7ERealized
        {
            private _31(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx21ⲻ7ERealized._31>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7ERealized._31> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx21ⲻ7ERealized._31> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._31>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._31>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx21ⲻ7ERealized._31(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._31>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _32 : _Ⰳx21ⲻ7ERealized
        {
            private _32(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx21ⲻ7ERealized._32>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7ERealized._32> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx21ⲻ7ERealized._32> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._32>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._32>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx21ⲻ7ERealized._32(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._32>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _33 : _Ⰳx21ⲻ7ERealized
        {
            private _33(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx21ⲻ7ERealized._33>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7ERealized._33> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx21ⲻ7ERealized._33> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._33>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._33>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx21ⲻ7ERealized._33(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._33>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _34 : _Ⰳx21ⲻ7ERealized
        {
            private _34(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx21ⲻ7ERealized._34>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7ERealized._34> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx21ⲻ7ERealized._34> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._34>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._34>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx21ⲻ7ERealized._34(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._34>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _35 : _Ⰳx21ⲻ7ERealized
        {
            private _35(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx21ⲻ7ERealized._35>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7ERealized._35> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx21ⲻ7ERealized._35> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._35>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._35>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx21ⲻ7ERealized._35(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._35>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _36 : _Ⰳx21ⲻ7ERealized
        {
            private _36(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx21ⲻ7ERealized._36>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7ERealized._36> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx21ⲻ7ERealized._36> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._36>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._36>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx21ⲻ7ERealized._36(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._36>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _37 : _Ⰳx21ⲻ7ERealized
        {
            private _37(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx21ⲻ7ERealized._37>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7ERealized._37> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx21ⲻ7ERealized._37> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._37>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._37>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx21ⲻ7ERealized._37(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._37>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _38 : _Ⰳx21ⲻ7ERealized
        {
            private _38(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx21ⲻ7ERealized._38>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7ERealized._38> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx21ⲻ7ERealized._38> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._38>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._38>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx21ⲻ7ERealized._38(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._38>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _39 : _Ⰳx21ⲻ7ERealized
        {
            private _39(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx21ⲻ7ERealized._39>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7ERealized._39> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx21ⲻ7ERealized._39> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._39>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._39>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx21ⲻ7ERealized._39(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._39>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _3A : _Ⰳx21ⲻ7ERealized
        {
            private _3A(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx21ⲻ7ERealized._3A>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7ERealized._3A> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx21ⲻ7ERealized._3A> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._3A>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._3A>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx21ⲻ7ERealized._3A(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._3A>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _3B : _Ⰳx21ⲻ7ERealized
        {
            private _3B(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx21ⲻ7ERealized._3B>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7ERealized._3B> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx21ⲻ7ERealized._3B> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._3B>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._3B>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx21ⲻ7ERealized._3B(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._3B>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _3C : _Ⰳx21ⲻ7ERealized
        {
            private _3C(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx21ⲻ7ERealized._3C>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7ERealized._3C> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx21ⲻ7ERealized._3C> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._3C>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._3C>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx21ⲻ7ERealized._3C(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._3C>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _3D : _Ⰳx21ⲻ7ERealized
        {
            private _3D(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx21ⲻ7ERealized._3D>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7ERealized._3D> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx21ⲻ7ERealized._3D> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._3D>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._3D>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx21ⲻ7ERealized._3D(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._3D>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _3E : _Ⰳx21ⲻ7ERealized
        {
            private _3E(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx21ⲻ7ERealized._3E>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7ERealized._3E> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx21ⲻ7ERealized._3E> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._3E>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._3E>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx21ⲻ7ERealized._3E(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._3E>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _3F : _Ⰳx21ⲻ7ERealized
        {
            private _3F(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx21ⲻ7ERealized._3F>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7ERealized._3F> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx21ⲻ7ERealized._3F> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._3F>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._3F>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx21ⲻ7ERealized._3F(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._3F>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _40 : _Ⰳx21ⲻ7ERealized
        {
            private _40(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx21ⲻ7ERealized._40>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7ERealized._40> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx21ⲻ7ERealized._40> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._40>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._40>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx21ⲻ7ERealized._40(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._40>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _41 : _Ⰳx21ⲻ7ERealized
        {
            private _41(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx21ⲻ7ERealized._41>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7ERealized._41> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx21ⲻ7ERealized._41> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._41>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._41>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx21ⲻ7ERealized._41(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._41>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _42 : _Ⰳx21ⲻ7ERealized
        {
            private _42(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx21ⲻ7ERealized._42>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7ERealized._42> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx21ⲻ7ERealized._42> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._42>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._42>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx21ⲻ7ERealized._42(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._42>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _43 : _Ⰳx21ⲻ7ERealized
        {
            private _43(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx21ⲻ7ERealized._43>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7ERealized._43> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx21ⲻ7ERealized._43> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._43>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._43>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx21ⲻ7ERealized._43(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._43>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _44 : _Ⰳx21ⲻ7ERealized
        {
            private _44(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx21ⲻ7ERealized._44>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7ERealized._44> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx21ⲻ7ERealized._44> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._44>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._44>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx21ⲻ7ERealized._44(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._44>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _45 : _Ⰳx21ⲻ7ERealized
        {
            private _45(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx21ⲻ7ERealized._45>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7ERealized._45> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx21ⲻ7ERealized._45> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._45>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._45>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx21ⲻ7ERealized._45(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._45>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _46 : _Ⰳx21ⲻ7ERealized
        {
            private _46(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx21ⲻ7ERealized._46>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7ERealized._46> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx21ⲻ7ERealized._46> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._46>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._46>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx21ⲻ7ERealized._46(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._46>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _47 : _Ⰳx21ⲻ7ERealized
        {
            private _47(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx21ⲻ7ERealized._47>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7ERealized._47> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx21ⲻ7ERealized._47> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._47>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._47>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx21ⲻ7ERealized._47(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._47>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _48 : _Ⰳx21ⲻ7ERealized
        {
            private _48(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx21ⲻ7ERealized._48>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7ERealized._48> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx21ⲻ7ERealized._48> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._48>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._48>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx21ⲻ7ERealized._48(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._48>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _49 : _Ⰳx21ⲻ7ERealized
        {
            private _49(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx21ⲻ7ERealized._49>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7ERealized._49> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx21ⲻ7ERealized._49> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._49>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._49>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx21ⲻ7ERealized._49(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._49>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _4A : _Ⰳx21ⲻ7ERealized
        {
            private _4A(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx21ⲻ7ERealized._4A>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7ERealized._4A> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx21ⲻ7ERealized._4A> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._4A>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._4A>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx21ⲻ7ERealized._4A(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._4A>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _4B : _Ⰳx21ⲻ7ERealized
        {
            private _4B(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx21ⲻ7ERealized._4B>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7ERealized._4B> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx21ⲻ7ERealized._4B> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._4B>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._4B>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx21ⲻ7ERealized._4B(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._4B>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _4C : _Ⰳx21ⲻ7ERealized
        {
            private _4C(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx21ⲻ7ERealized._4C>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7ERealized._4C> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx21ⲻ7ERealized._4C> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._4C>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._4C>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx21ⲻ7ERealized._4C(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._4C>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _4D : _Ⰳx21ⲻ7ERealized
        {
            private _4D(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx21ⲻ7ERealized._4D>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7ERealized._4D> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx21ⲻ7ERealized._4D> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._4D>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._4D>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx21ⲻ7ERealized._4D(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._4D>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _4E : _Ⰳx21ⲻ7ERealized
        {
            private _4E(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx21ⲻ7ERealized._4E>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7ERealized._4E> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx21ⲻ7ERealized._4E> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._4E>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._4E>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx21ⲻ7ERealized._4E(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._4E>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _4F : _Ⰳx21ⲻ7ERealized
        {
            private _4F(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx21ⲻ7ERealized._4F>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7ERealized._4F> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx21ⲻ7ERealized._4F> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._4F>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._4F>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx21ⲻ7ERealized._4F(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._4F>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _50 : _Ⰳx21ⲻ7ERealized
        {
            private _50(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx21ⲻ7ERealized._50>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7ERealized._50> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx21ⲻ7ERealized._50> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._50>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._50>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx21ⲻ7ERealized._50(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._50>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _51 : _Ⰳx21ⲻ7ERealized
        {
            private _51(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx21ⲻ7ERealized._51>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7ERealized._51> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx21ⲻ7ERealized._51> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._51>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._51>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx21ⲻ7ERealized._51(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._51>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _52 : _Ⰳx21ⲻ7ERealized
        {
            private _52(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx21ⲻ7ERealized._52>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7ERealized._52> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx21ⲻ7ERealized._52> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._52>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._52>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx21ⲻ7ERealized._52(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._52>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _53 : _Ⰳx21ⲻ7ERealized
        {
            private _53(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx21ⲻ7ERealized._53>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7ERealized._53> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx21ⲻ7ERealized._53> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._53>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._53>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx21ⲻ7ERealized._53(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._53>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _54 : _Ⰳx21ⲻ7ERealized
        {
            private _54(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx21ⲻ7ERealized._54>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7ERealized._54> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx21ⲻ7ERealized._54> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._54>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._54>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx21ⲻ7ERealized._54(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._54>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _55 : _Ⰳx21ⲻ7ERealized
        {
            private _55(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx21ⲻ7ERealized._55>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7ERealized._55> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx21ⲻ7ERealized._55> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._55>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._55>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx21ⲻ7ERealized._55(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._55>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _56 : _Ⰳx21ⲻ7ERealized
        {
            private _56(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx21ⲻ7ERealized._56>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7ERealized._56> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx21ⲻ7ERealized._56> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._56>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._56>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx21ⲻ7ERealized._56(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._56>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _57 : _Ⰳx21ⲻ7ERealized
        {
            private _57(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx21ⲻ7ERealized._57>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7ERealized._57> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx21ⲻ7ERealized._57> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._57>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._57>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx21ⲻ7ERealized._57(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._57>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _58 : _Ⰳx21ⲻ7ERealized
        {
            private _58(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx21ⲻ7ERealized._58>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7ERealized._58> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx21ⲻ7ERealized._58> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._58>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._58>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx21ⲻ7ERealized._58(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._58>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _59 : _Ⰳx21ⲻ7ERealized
        {
            private _59(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx21ⲻ7ERealized._59>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7ERealized._59> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx21ⲻ7ERealized._59> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._59>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._59>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx21ⲻ7ERealized._59(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._59>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _5A : _Ⰳx21ⲻ7ERealized
        {
            private _5A(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx21ⲻ7ERealized._5A>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7ERealized._5A> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx21ⲻ7ERealized._5A> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._5A>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._5A>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx21ⲻ7ERealized._5A(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._5A>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _5B : _Ⰳx21ⲻ7ERealized
        {
            private _5B(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx21ⲻ7ERealized._5B>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7ERealized._5B> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx21ⲻ7ERealized._5B> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._5B>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._5B>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx21ⲻ7ERealized._5B(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._5B>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _5C : _Ⰳx21ⲻ7ERealized
        {
            private _5C(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx21ⲻ7ERealized._5C>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7ERealized._5C> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx21ⲻ7ERealized._5C> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._5C>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._5C>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx21ⲻ7ERealized._5C(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._5C>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _5D : _Ⰳx21ⲻ7ERealized
        {
            private _5D(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx21ⲻ7ERealized._5D>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7ERealized._5D> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx21ⲻ7ERealized._5D> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._5D>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._5D>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx21ⲻ7ERealized._5D(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._5D>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _5E : _Ⰳx21ⲻ7ERealized
        {
            private _5E(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx21ⲻ7ERealized._5E>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7ERealized._5E> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx21ⲻ7ERealized._5E> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._5E>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._5E>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx21ⲻ7ERealized._5E(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._5E>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _5F : _Ⰳx21ⲻ7ERealized
        {
            private _5F(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx21ⲻ7ERealized._5F>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7ERealized._5F> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx21ⲻ7ERealized._5F> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._5F>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._5F>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx21ⲻ7ERealized._5F(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._5F>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _60 : _Ⰳx21ⲻ7ERealized
        {
            private _60(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx21ⲻ7ERealized._60>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7ERealized._60> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx21ⲻ7ERealized._60> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._60>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._60>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx21ⲻ7ERealized._60(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._60>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _61 : _Ⰳx21ⲻ7ERealized
        {
            private _61(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx21ⲻ7ERealized._61>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7ERealized._61> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx21ⲻ7ERealized._61> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._61>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._61>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx21ⲻ7ERealized._61(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._61>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _62 : _Ⰳx21ⲻ7ERealized
        {
            private _62(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx21ⲻ7ERealized._62>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7ERealized._62> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx21ⲻ7ERealized._62> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._62>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._62>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx21ⲻ7ERealized._62(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._62>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _63 : _Ⰳx21ⲻ7ERealized
        {
            private _63(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx21ⲻ7ERealized._63>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7ERealized._63> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx21ⲻ7ERealized._63> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._63>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._63>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx21ⲻ7ERealized._63(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._63>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _64 : _Ⰳx21ⲻ7ERealized
        {
            private _64(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx21ⲻ7ERealized._64>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7ERealized._64> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx21ⲻ7ERealized._64> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._64>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._64>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx21ⲻ7ERealized._64(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._64>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _65 : _Ⰳx21ⲻ7ERealized
        {
            private _65(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx21ⲻ7ERealized._65>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7ERealized._65> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx21ⲻ7ERealized._65> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._65>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._65>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx21ⲻ7ERealized._65(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._65>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _66 : _Ⰳx21ⲻ7ERealized
        {
            private _66(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx21ⲻ7ERealized._66>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7ERealized._66> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx21ⲻ7ERealized._66> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._66>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._66>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx21ⲻ7ERealized._66(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._66>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _67 : _Ⰳx21ⲻ7ERealized
        {
            private _67(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx21ⲻ7ERealized._67>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7ERealized._67> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx21ⲻ7ERealized._67> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._67>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._67>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx21ⲻ7ERealized._67(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._67>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _68 : _Ⰳx21ⲻ7ERealized
        {
            private _68(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx21ⲻ7ERealized._68>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7ERealized._68> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx21ⲻ7ERealized._68> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._68>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._68>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx21ⲻ7ERealized._68(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._68>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _69 : _Ⰳx21ⲻ7ERealized
        {
            private _69(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx21ⲻ7ERealized._69>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7ERealized._69> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx21ⲻ7ERealized._69> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._69>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._69>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx21ⲻ7ERealized._69(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._69>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _6A : _Ⰳx21ⲻ7ERealized
        {
            private _6A(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx21ⲻ7ERealized._6A>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7ERealized._6A> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx21ⲻ7ERealized._6A> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._6A>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._6A>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx21ⲻ7ERealized._6A(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._6A>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _6B : _Ⰳx21ⲻ7ERealized
        {
            private _6B(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx21ⲻ7ERealized._6B>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7ERealized._6B> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx21ⲻ7ERealized._6B> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._6B>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._6B>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx21ⲻ7ERealized._6B(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._6B>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _6C : _Ⰳx21ⲻ7ERealized
        {
            private _6C(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx21ⲻ7ERealized._6C>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7ERealized._6C> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx21ⲻ7ERealized._6C> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._6C>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._6C>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx21ⲻ7ERealized._6C(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._6C>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _6D : _Ⰳx21ⲻ7ERealized
        {
            private _6D(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx21ⲻ7ERealized._6D>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7ERealized._6D> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx21ⲻ7ERealized._6D> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._6D>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._6D>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx21ⲻ7ERealized._6D(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._6D>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _6E : _Ⰳx21ⲻ7ERealized
        {
            private _6E(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx21ⲻ7ERealized._6E>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7ERealized._6E> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx21ⲻ7ERealized._6E> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._6E>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._6E>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx21ⲻ7ERealized._6E(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._6E>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _6F : _Ⰳx21ⲻ7ERealized
        {
            private _6F(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx21ⲻ7ERealized._6F>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7ERealized._6F> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx21ⲻ7ERealized._6F> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._6F>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._6F>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx21ⲻ7ERealized._6F(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._6F>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _70 : _Ⰳx21ⲻ7ERealized
        {
            private _70(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx21ⲻ7ERealized._70>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7ERealized._70> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx21ⲻ7ERealized._70> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._70>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._70>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx21ⲻ7ERealized._70(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._70>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _71 : _Ⰳx21ⲻ7ERealized
        {
            private _71(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx21ⲻ7ERealized._71>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7ERealized._71> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx21ⲻ7ERealized._71> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._71>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._71>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx21ⲻ7ERealized._71(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._71>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _72 : _Ⰳx21ⲻ7ERealized
        {
            private _72(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx21ⲻ7ERealized._72>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7ERealized._72> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx21ⲻ7ERealized._72> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._72>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._72>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx21ⲻ7ERealized._72(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._72>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _73 : _Ⰳx21ⲻ7ERealized
        {
            private _73(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx21ⲻ7ERealized._73>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7ERealized._73> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx21ⲻ7ERealized._73> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._73>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._73>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx21ⲻ7ERealized._73(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._73>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _74 : _Ⰳx21ⲻ7ERealized
        {
            private _74(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx21ⲻ7ERealized._74>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7ERealized._74> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx21ⲻ7ERealized._74> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._74>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._74>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx21ⲻ7ERealized._74(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._74>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _75 : _Ⰳx21ⲻ7ERealized
        {
            private _75(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx21ⲻ7ERealized._75>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7ERealized._75> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx21ⲻ7ERealized._75> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._75>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._75>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx21ⲻ7ERealized._75(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._75>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _76 : _Ⰳx21ⲻ7ERealized
        {
            private _76(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx21ⲻ7ERealized._76>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7ERealized._76> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx21ⲻ7ERealized._76> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._76>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._76>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx21ⲻ7ERealized._76(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._76>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _77 : _Ⰳx21ⲻ7ERealized
        {
            private _77(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx21ⲻ7ERealized._77>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7ERealized._77> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx21ⲻ7ERealized._77> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._77>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._77>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx21ⲻ7ERealized._77(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._77>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _78 : _Ⰳx21ⲻ7ERealized
        {
            private _78(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx21ⲻ7ERealized._78>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7ERealized._78> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx21ⲻ7ERealized._78> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._78>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._78>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx21ⲻ7ERealized._78(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._78>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _79 : _Ⰳx21ⲻ7ERealized
        {
            private _79(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx21ⲻ7ERealized._79>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7ERealized._79> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx21ⲻ7ERealized._79> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._79>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._79>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx21ⲻ7ERealized._79(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._79>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _7A : _Ⰳx21ⲻ7ERealized
        {
            private _7A(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx21ⲻ7ERealized._7A>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7ERealized._7A> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx21ⲻ7ERealized._7A> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._7A>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._7A>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx21ⲻ7ERealized._7A(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._7A>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _7B : _Ⰳx21ⲻ7ERealized
        {
            private _7B(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx21ⲻ7ERealized._7B>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7ERealized._7B> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx21ⲻ7ERealized._7B> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._7B>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._7B>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx21ⲻ7ERealized._7B(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._7B>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _7C : _Ⰳx21ⲻ7ERealized
        {
            private _7C(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx21ⲻ7ERealized._7C>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7ERealized._7C> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx21ⲻ7ERealized._7C> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._7C>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._7C>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx21ⲻ7ERealized._7C(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._7C>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _7D : _Ⰳx21ⲻ7ERealized
        {
            private _7D(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx21ⲻ7ERealized._7D>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7ERealized._7D> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx21ⲻ7ERealized._7D> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._7D>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._7D>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx21ⲻ7ERealized._7D(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._7D>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _7E : _Ⰳx21ⲻ7ERealized
        {
            private _7E(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx21ⲻ7ERealized._7E>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx21ⲻ7ERealized._7E> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx21ⲻ7ERealized._7E> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._7E>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._7E>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx21ⲻ7ERealized._7E(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx21ⲻ7ERealized._7E>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
