namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _x5DParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._x5D> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._x5D>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._x5D> Parse(IInput<char>? input)
            {
                var _x5D = CombinatorParsingV2.Parse.Char((char)0x5D).Parse(input);
if (!_x5D.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._x5D)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._x5D.Instance, _x5D.Remainder);
            }
        }
    }
    
}
