namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _DIGITParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._DIGIT> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._DIGIT>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._DIGIT> Parse(IInput<char>? input)
            {
                var _Ⰳx30ⲻ39_1 = __GeneratedOdataV4.Parsers.Inners._Ⰳx30ⲻ39Parser.Instance.Parse(input);
if (!_Ⰳx30ⲻ39_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._DIGIT)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._DIGIT(_Ⰳx30ⲻ39_1.Parsed), _Ⰳx30ⲻ39_1.Remainder);
            }
        }
    }
    
}
