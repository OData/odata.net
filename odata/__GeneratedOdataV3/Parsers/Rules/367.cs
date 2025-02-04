namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _obsⲻtextParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._obsⲻtext> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._obsⲻtext>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._obsⲻtext> Parse(IInput<char>? input)
            {
                var _Ⰳx80ⲻFF_1 = __GeneratedOdataV3.Parsers.Inners._Ⰳx80ⲻFFParser.Instance.Parse(input);
if (!_Ⰳx80ⲻFF_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._obsⲻtext)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._obsⲻtext(_Ⰳx80ⲻFF_1.Parsed), _Ⰳx80ⲻFF_1.Remainder);
            }
        }
    }
    
}
