namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _x4FParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._x4F> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._x4F>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._x4F> Parse(IInput<char>? input)
            {
                var _x4F = CombinatorParsingV2.Parse.Char((char)0x4F).Parse(input);
if (!_x4F.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._x4F)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._x4F.Instance, _x4F.Remainder);
            }
        }
    }
    
}
