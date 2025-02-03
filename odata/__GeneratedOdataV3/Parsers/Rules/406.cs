namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _pctⲻencodedParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._pctⲻencoded> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._pctⲻencoded>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._pctⲻencoded> Parse(IInput<char>? input)
            {
                var _ʺx25ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx25ʺParser.Instance.Parse(input);
if (!_ʺx25ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._pctⲻencoded)!, input);
}

var _HEXDIG_1 = __GeneratedOdataV3.Parsers.Rules._HEXDIGParser.Instance.Parse(_ʺx25ʺ_1.Remainder);
if (!_HEXDIG_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._pctⲻencoded)!, input);
}

var _HEXDIG_2 = __GeneratedOdataV3.Parsers.Rules._HEXDIGParser.Instance.Parse(_HEXDIG_1.Remainder);
if (!_HEXDIG_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._pctⲻencoded)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._pctⲻencoded(_ʺx25ʺ_1.Parsed, _HEXDIG_1.Parsed,  _HEXDIG_2.Parsed), _HEXDIG_2.Remainder);
            }
        }
    }
    
}
