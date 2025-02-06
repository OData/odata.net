namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _geographyPrefixParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._geographyPrefix> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._geographyPrefix>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._geographyPrefix> Parse(IInput<char>? input)
            {
                var _ʺx67x65x6Fx67x72x61x70x68x79ʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx67x65x6Fx67x72x61x70x68x79ʺParser.Instance.Parse(input);
if (!_ʺx67x65x6Fx67x72x61x70x68x79ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._geographyPrefix)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Rules._geographyPrefix.Instance, _ʺx67x65x6Fx67x72x61x70x68x79ʺ_1.Remainder);
            }
        }
    }
    
}
