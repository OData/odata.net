namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _nanInfinityParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._nanInfinity> Instance { get; } = (_ʺx4Ex61x4EʺParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._nanInfinity>(_ʺx2Dx49x4Ex46ʺParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._nanInfinity>(_ʺx49x4Ex46ʺParser.Instance);
        
        public static class _ʺx4Ex61x4EʺParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._nanInfinity._ʺx4Ex61x4Eʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._nanInfinity._ʺx4Ex61x4Eʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._nanInfinity._ʺx4Ex61x4Eʺ> Parse(IInput<char>? input)
                {
                    var _ʺx4Ex61x4Eʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx4Ex61x4EʺParser.Instance.Parse(input);
if (!_ʺx4Ex61x4Eʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._nanInfinity._ʺx4Ex61x4Eʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Rules._nanInfinity._ʺx4Ex61x4Eʺ.Instance, _ʺx4Ex61x4Eʺ_1.Remainder);
                }
            }
        }
        
        public static class _ʺx2Dx49x4Ex46ʺParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._nanInfinity._ʺx2Dx49x4Ex46ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._nanInfinity._ʺx2Dx49x4Ex46ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._nanInfinity._ʺx2Dx49x4Ex46ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx2Dx49x4Ex46ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx2Dx49x4Ex46ʺParser.Instance.Parse(input);
if (!_ʺx2Dx49x4Ex46ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._nanInfinity._ʺx2Dx49x4Ex46ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Rules._nanInfinity._ʺx2Dx49x4Ex46ʺ.Instance, _ʺx2Dx49x4Ex46ʺ_1.Remainder);
                }
            }
        }
        
        public static class _ʺx49x4Ex46ʺParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._nanInfinity._ʺx49x4Ex46ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._nanInfinity._ʺx49x4Ex46ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._nanInfinity._ʺx49x4Ex46ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx49x4Ex46ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx49x4Ex46ʺParser.Instance.Parse(input);
if (!_ʺx49x4Ex46ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._nanInfinity._ʺx49x4Ex46ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Rules._nanInfinity._ʺx49x4Ex46ʺ.Instance, _ʺx49x4Ex46ʺ_1.Remainder);
                }
            }
        }
    }
    
}
