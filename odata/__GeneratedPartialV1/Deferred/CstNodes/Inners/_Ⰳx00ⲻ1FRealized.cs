namespace __GeneratedPartialV1.Deferred.CstNodes.Inners
{
    using System;
    using CombinatorParsingV3;
    
    public abstract class _Ⰳx00ⲻ1FRealized
    {
        private _Ⰳx00ⲻ1FRealized()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_Ⰳx00ⲻ1FRealized node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_00 node, TContext context);
            protected internal abstract TResult Accept(_01 node, TContext context);
            protected internal abstract TResult Accept(_02 node, TContext context);
            protected internal abstract TResult Accept(_03 node, TContext context);
            protected internal abstract TResult Accept(_04 node, TContext context);
            protected internal abstract TResult Accept(_05 node, TContext context);
            protected internal abstract TResult Accept(_06 node, TContext context);
            protected internal abstract TResult Accept(_07 node, TContext context);
            protected internal abstract TResult Accept(_08 node, TContext context);
            protected internal abstract TResult Accept(_09 node, TContext context);
            protected internal abstract TResult Accept(_0A node, TContext context);
            protected internal abstract TResult Accept(_0B node, TContext context);
            protected internal abstract TResult Accept(_0C node, TContext context);
            protected internal abstract TResult Accept(_0D node, TContext context);
            protected internal abstract TResult Accept(_0E node, TContext context);
            protected internal abstract TResult Accept(_0F node, TContext context);
            protected internal abstract TResult Accept(_10 node, TContext context);
            protected internal abstract TResult Accept(_11 node, TContext context);
            protected internal abstract TResult Accept(_12 node, TContext context);
            protected internal abstract TResult Accept(_13 node, TContext context);
            protected internal abstract TResult Accept(_14 node, TContext context);
            protected internal abstract TResult Accept(_15 node, TContext context);
            protected internal abstract TResult Accept(_16 node, TContext context);
            protected internal abstract TResult Accept(_17 node, TContext context);
            protected internal abstract TResult Accept(_18 node, TContext context);
            protected internal abstract TResult Accept(_19 node, TContext context);
            protected internal abstract TResult Accept(_1A node, TContext context);
            protected internal abstract TResult Accept(_1B node, TContext context);
            protected internal abstract TResult Accept(_1C node, TContext context);
            protected internal abstract TResult Accept(_1D node, TContext context);
            protected internal abstract TResult Accept(_1E node, TContext context);
            protected internal abstract TResult Accept(_1F node, TContext context);
        }
        
        public sealed class _00 : _Ⰳx00ⲻ1FRealized
        {
            private _00(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx00ⲻ1FRealized._00>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx00ⲻ1FRealized._00> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx00ⲻ1FRealized._00> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized._00>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized._00>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx00ⲻ1FRealized._00(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized._00>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _01 : _Ⰳx00ⲻ1FRealized
        {
            private _01(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx00ⲻ1FRealized._01>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx00ⲻ1FRealized._01> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx00ⲻ1FRealized._01> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized._01>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized._01>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx00ⲻ1FRealized._01(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized._01>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _02 : _Ⰳx00ⲻ1FRealized
        {
            private _02(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx00ⲻ1FRealized._02>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx00ⲻ1FRealized._02> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx00ⲻ1FRealized._02> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized._02>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized._02>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx00ⲻ1FRealized._02(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized._02>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _03 : _Ⰳx00ⲻ1FRealized
        {
            private _03(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx00ⲻ1FRealized._03>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx00ⲻ1FRealized._03> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx00ⲻ1FRealized._03> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized._03>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized._03>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx00ⲻ1FRealized._03(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized._03>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _04 : _Ⰳx00ⲻ1FRealized
        {
            private _04(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx00ⲻ1FRealized._04>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx00ⲻ1FRealized._04> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx00ⲻ1FRealized._04> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized._04>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized._04>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx00ⲻ1FRealized._04(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized._04>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _05 : _Ⰳx00ⲻ1FRealized
        {
            private _05(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx00ⲻ1FRealized._05>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx00ⲻ1FRealized._05> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx00ⲻ1FRealized._05> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized._05>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized._05>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx00ⲻ1FRealized._05(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized._05>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _06 : _Ⰳx00ⲻ1FRealized
        {
            private _06(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx00ⲻ1FRealized._06>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx00ⲻ1FRealized._06> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx00ⲻ1FRealized._06> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized._06>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized._06>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx00ⲻ1FRealized._06(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized._06>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _07 : _Ⰳx00ⲻ1FRealized
        {
            private _07(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx00ⲻ1FRealized._07>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx00ⲻ1FRealized._07> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx00ⲻ1FRealized._07> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized._07>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized._07>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx00ⲻ1FRealized._07(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized._07>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _08 : _Ⰳx00ⲻ1FRealized
        {
            private _08(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx00ⲻ1FRealized._08>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx00ⲻ1FRealized._08> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx00ⲻ1FRealized._08> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized._08>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized._08>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx00ⲻ1FRealized._08(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized._08>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _09 : _Ⰳx00ⲻ1FRealized
        {
            private _09(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx00ⲻ1FRealized._09>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx00ⲻ1FRealized._09> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx00ⲻ1FRealized._09> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized._09>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized._09>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx00ⲻ1FRealized._09(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized._09>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _0A : _Ⰳx00ⲻ1FRealized
        {
            private _0A(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx00ⲻ1FRealized._0A>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx00ⲻ1FRealized._0A> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx00ⲻ1FRealized._0A> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized._0A>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized._0A>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx00ⲻ1FRealized._0A(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized._0A>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _0B : _Ⰳx00ⲻ1FRealized
        {
            private _0B(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx00ⲻ1FRealized._0B>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx00ⲻ1FRealized._0B> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx00ⲻ1FRealized._0B> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized._0B>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized._0B>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx00ⲻ1FRealized._0B(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized._0B>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _0C : _Ⰳx00ⲻ1FRealized
        {
            private _0C(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx00ⲻ1FRealized._0C>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx00ⲻ1FRealized._0C> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx00ⲻ1FRealized._0C> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized._0C>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized._0C>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx00ⲻ1FRealized._0C(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized._0C>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _0D : _Ⰳx00ⲻ1FRealized
        {
            private _0D(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx00ⲻ1FRealized._0D>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx00ⲻ1FRealized._0D> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx00ⲻ1FRealized._0D> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized._0D>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized._0D>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx00ⲻ1FRealized._0D(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized._0D>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _0E : _Ⰳx00ⲻ1FRealized
        {
            private _0E(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx00ⲻ1FRealized._0E>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx00ⲻ1FRealized._0E> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx00ⲻ1FRealized._0E> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized._0E>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized._0E>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx00ⲻ1FRealized._0E(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized._0E>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _0F : _Ⰳx00ⲻ1FRealized
        {
            private _0F(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx00ⲻ1FRealized._0F>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx00ⲻ1FRealized._0F> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx00ⲻ1FRealized._0F> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized._0F>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized._0F>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx00ⲻ1FRealized._0F(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized._0F>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _10 : _Ⰳx00ⲻ1FRealized
        {
            private _10(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx00ⲻ1FRealized._10>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx00ⲻ1FRealized._10> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx00ⲻ1FRealized._10> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized._10>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized._10>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx00ⲻ1FRealized._10(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized._10>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _11 : _Ⰳx00ⲻ1FRealized
        {
            private _11(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx00ⲻ1FRealized._11>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx00ⲻ1FRealized._11> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx00ⲻ1FRealized._11> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized._11>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized._11>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx00ⲻ1FRealized._11(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized._11>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _12 : _Ⰳx00ⲻ1FRealized
        {
            private _12(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx00ⲻ1FRealized._12>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx00ⲻ1FRealized._12> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx00ⲻ1FRealized._12> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized._12>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized._12>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx00ⲻ1FRealized._12(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized._12>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _13 : _Ⰳx00ⲻ1FRealized
        {
            private _13(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx00ⲻ1FRealized._13>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx00ⲻ1FRealized._13> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx00ⲻ1FRealized._13> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized._13>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized._13>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx00ⲻ1FRealized._13(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized._13>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _14 : _Ⰳx00ⲻ1FRealized
        {
            private _14(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx00ⲻ1FRealized._14>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx00ⲻ1FRealized._14> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx00ⲻ1FRealized._14> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized._14>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized._14>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx00ⲻ1FRealized._14(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized._14>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _15 : _Ⰳx00ⲻ1FRealized
        {
            private _15(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx00ⲻ1FRealized._15>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx00ⲻ1FRealized._15> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx00ⲻ1FRealized._15> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized._15>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized._15>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx00ⲻ1FRealized._15(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized._15>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _16 : _Ⰳx00ⲻ1FRealized
        {
            private _16(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx00ⲻ1FRealized._16>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx00ⲻ1FRealized._16> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx00ⲻ1FRealized._16> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized._16>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized._16>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx00ⲻ1FRealized._16(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized._16>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _17 : _Ⰳx00ⲻ1FRealized
        {
            private _17(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx00ⲻ1FRealized._17>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx00ⲻ1FRealized._17> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx00ⲻ1FRealized._17> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized._17>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized._17>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx00ⲻ1FRealized._17(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized._17>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _18 : _Ⰳx00ⲻ1FRealized
        {
            private _18(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx00ⲻ1FRealized._18>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx00ⲻ1FRealized._18> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx00ⲻ1FRealized._18> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized._18>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized._18>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx00ⲻ1FRealized._18(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized._18>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _19 : _Ⰳx00ⲻ1FRealized
        {
            private _19(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx00ⲻ1FRealized._19>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx00ⲻ1FRealized._19> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx00ⲻ1FRealized._19> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized._19>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized._19>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx00ⲻ1FRealized._19(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized._19>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _1A : _Ⰳx00ⲻ1FRealized
        {
            private _1A(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx00ⲻ1FRealized._1A>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx00ⲻ1FRealized._1A> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx00ⲻ1FRealized._1A> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized._1A>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized._1A>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx00ⲻ1FRealized._1A(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized._1A>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _1B : _Ⰳx00ⲻ1FRealized
        {
            private _1B(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx00ⲻ1FRealized._1B>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx00ⲻ1FRealized._1B> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx00ⲻ1FRealized._1B> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized._1B>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized._1B>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx00ⲻ1FRealized._1B(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized._1B>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _1C : _Ⰳx00ⲻ1FRealized
        {
            private _1C(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx00ⲻ1FRealized._1C>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx00ⲻ1FRealized._1C> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx00ⲻ1FRealized._1C> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized._1C>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized._1C>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx00ⲻ1FRealized._1C(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized._1C>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _1D : _Ⰳx00ⲻ1FRealized
        {
            private _1D(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx00ⲻ1FRealized._1D>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx00ⲻ1FRealized._1D> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx00ⲻ1FRealized._1D> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized._1D>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized._1D>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx00ⲻ1FRealized._1D(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized._1D>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _1E : _Ⰳx00ⲻ1FRealized
        {
            private _1E(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx00ⲻ1FRealized._1E>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx00ⲻ1FRealized._1E> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx00ⲻ1FRealized._1E> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized._1E>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized._1E>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx00ⲻ1FRealized._1E(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized._1E>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _1F : _Ⰳx00ⲻ1FRealized
        {
            private _1F(ITokenStream<char>? nextTokens)
            {
                this.RealizationResult = new RealizationResult<char, _Ⰳx00ⲻ1FRealized._1F>(true, this, nextTokens);
            }
            
            private IRealizationResult<char, _Ⰳx00ⲻ1FRealized._1F> RealizationResult { get; }
            
            public static IRealizationResult<char, _Ⰳx00ⲻ1FRealized._1F> Create(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
            {
                var output = previousNodeRealizationResult.Value;
if (!output.Success)
{
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized._1F>(false, default, output.RemainingTokens);
}

var input = output.RemainingTokens;
if (input == null)
{
    //// TODO realizationresult.create would be nice
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized._1F>(false, default, input);
}

if (input.Current == 'A') //// TODO do this correctly...du's aren't always terminal nodes, and they aren't even terminal nodes in your case
{
    var a = new _Ⰳx00ⲻ1FRealized._1F(input.Next());
    return a.RealizationResult;
}
else
{
    return new RealizationResult<char, _Ⰳx00ⲻ1FRealized._1F>(false, default, input);
}
            }
            
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
