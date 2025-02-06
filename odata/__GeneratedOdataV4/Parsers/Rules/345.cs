namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _geometryPrefixParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._geometryPrefix> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._geometryPrefix>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._geometryPrefix> Parse(IInput<char>? input)
            {
                var _ʺx67x65x6Fx6Dx65x74x72x79ʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx67x65x6Fx6Dx65x74x72x79ʺParser.Instance.Parse(input);
if (!_ʺx67x65x6Fx6Dx65x74x72x79ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._geometryPrefix)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Rules._geometryPrefix.Instance, _ʺx67x65x6Fx6Dx65x74x72x79ʺ_1.Remainder);
            }
        }
    }
    
}
