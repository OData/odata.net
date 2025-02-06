namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _x2DParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._x2D> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._x2D>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._x2D> Parse(IInput<char>? input)
            {
                var _x2D = CombinatorParsingV2.Parse.Char((char)0x2D).Parse(input);
if (!_x2D.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._x2D)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._x2D.Instance, _x2D.Remainder);
            }
        }
    }
    
}
