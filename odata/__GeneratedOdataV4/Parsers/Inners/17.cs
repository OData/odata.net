namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _x62Parser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._x62> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._x62>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._x62> Parse(IInput<char>? input)
            {
                var _x62 = CombinatorParsingV2.Parse.Char((char)0x62).Parse(input);
if (!_x62.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._x62)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._x62.Instance, _x62.Remainder);
            }
        }
    }
    
}
