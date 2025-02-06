namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _x5FParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._x5F> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._x5F>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._x5F> Parse(IInput<char>? input)
            {
                var _x5F = CombinatorParsingV2.Parse.Char((char)0x5F).Parse(input);
if (!_x5F.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._x5F)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._x5F.Instance, _x5F.Remainder);
            }
        }
    }
    
}
