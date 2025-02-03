namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _EQⲻhParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._EQⲻh> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._EQⲻh>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._EQⲻh> Parse(IInput<char>? input)
            {
                var _BWSⲻh_1 = __GeneratedOdataV3.Parsers.Rules._BWSⲻhParser.Instance.Parse(input);
if (!_BWSⲻh_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._EQⲻh)!, input);
}

var _EQ_1 = __GeneratedOdataV3.Parsers.Rules._EQParser.Instance.Parse(_BWSⲻh_1.Remainder);
if (!_EQ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._EQⲻh)!, input);
}

var _BWSⲻh_2 = __GeneratedOdataV3.Parsers.Rules._BWSⲻhParser.Instance.Parse(_EQ_1.Remainder);
if (!_BWSⲻh_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._EQⲻh)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._EQⲻh(_BWSⲻh_1.Parsed, _EQ_1.Parsed,  _BWSⲻh_2.Parsed), _BWSⲻh_2.Remainder);
            }
        }
    }
    
}
