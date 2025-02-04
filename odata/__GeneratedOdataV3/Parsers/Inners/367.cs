namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _x7BParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._x7B> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._x7B>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._x7B> Parse(IInput<char>? input)
            {
                var _x7B = CombinatorParsingV2.Parse.Char((char)0x7B).Parse(input);
if (!_x7B.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._x7B)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._x7B.Instance, _x7B.Remainder);
            }
        }
    }
    
}
