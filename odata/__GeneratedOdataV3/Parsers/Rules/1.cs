namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _odataUriParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._odataUri> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._odataUri>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._odataUri> Parse(IInput<char>? input)
            {
                var _serviceRoot_1 = __GeneratedOdataV3.Parsers.Rules._serviceRootParser.Instance.Parse(input);
if (!_serviceRoot_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._odataUri)!, input);
}

var _odataRelativeUri_1 = __GeneratedOdataV3.Parsers.Rules._odataRelativeUriParser.Instance.Optional().Parse(_serviceRoot_1.Remainder);
if (!_odataRelativeUri_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._odataUri)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._odataUri(_serviceRoot_1.Parsed,  _odataRelativeUri_1.Parsed.GetOrElse(null)), _odataRelativeUri_1.Remainder);
            }
        }
    }
    
}
