namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _booleanValueParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._booleanValue> Instance { get; } = (_ʺx74x72x75x65ʺParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._booleanValue>(_ʺx66x61x6Cx73x65ʺParser.Instance);
        
        public static class _ʺx74x72x75x65ʺParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._booleanValue._ʺx74x72x75x65ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._booleanValue._ʺx74x72x75x65ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._booleanValue._ʺx74x72x75x65ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx74x72x75x65ʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx74x72x75x65ʺParser.Instance.Parse(input);
if (!_ʺx74x72x75x65ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._booleanValue._ʺx74x72x75x65ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Rules._booleanValue._ʺx74x72x75x65ʺ.Instance, _ʺx74x72x75x65ʺ_1.Remainder);
                }
            }
        }
        
        public static class _ʺx66x61x6Cx73x65ʺParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._booleanValue._ʺx66x61x6Cx73x65ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._booleanValue._ʺx66x61x6Cx73x65ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._booleanValue._ʺx66x61x6Cx73x65ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx66x61x6Cx73x65ʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx66x61x6Cx73x65ʺParser.Instance.Parse(input);
if (!_ʺx66x61x6Cx73x65ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._booleanValue._ʺx66x61x6Cx73x65ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Rules._booleanValue._ʺx66x61x6Cx73x65ʺ.Instance, _ʺx66x61x6Cx73x65ʺ_1.Remainder);
                }
            }
        }
    }
    
}
