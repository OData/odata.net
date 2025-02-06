namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _x6FParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._x6F> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._x6F>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._x6F> Parse(IInput<char>? input)
            {
                var _x6F = CombinatorParsingV2.Parse.Char((char)0x6F).Parse(input);
if (!_x6F.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._x6F)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._x6F.Instance, _x6F.Remainder);
            }
        }
    }
    
}
