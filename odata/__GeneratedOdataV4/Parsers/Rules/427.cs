namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _VCHARParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._VCHAR> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._VCHAR>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._VCHAR> Parse(IInput<char>? input)
            {
                var _Ⰳx21ⲻ7E_1 = __GeneratedOdataV4.Parsers.Inners._Ⰳx21ⲻ7EParser.Instance.Parse(input);
if (!_Ⰳx21ⲻ7E_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._VCHAR)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._VCHAR(_Ⰳx21ⲻ7E_1.Parsed), _Ⰳx21ⲻ7E_1.Remainder);
            }
        }
    }
    
}
