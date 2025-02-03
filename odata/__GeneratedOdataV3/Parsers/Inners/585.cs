namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx23ʺ_odataIdentifierParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx23ʺ_odataIdentifier> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx23ʺ_odataIdentifier>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx23ʺ_odataIdentifier> Parse(IInput<char>? input)
            {
                var _ʺx23ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx23ʺParser.Instance.Parse(input);
if (!_ʺx23ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx23ʺ_odataIdentifier)!, input);
}

var _odataIdentifier_1 = __GeneratedOdataV3.Parsers.Rules._odataIdentifierParser.Instance.Parse(_ʺx23ʺ_1.Remainder);
if (!_odataIdentifier_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx23ʺ_odataIdentifier)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._ʺx23ʺ_odataIdentifier(_ʺx23ʺ_1.Parsed,  _odataIdentifier_1.Parsed), _odataIdentifier_1.Remainder);
            }
        }
    }
    
}
