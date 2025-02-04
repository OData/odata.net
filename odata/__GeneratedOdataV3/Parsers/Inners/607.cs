namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _x2CParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._x2C> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._x2C>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._x2C> Parse(IInput<char>? input)
            {
                var _x2C = CombinatorParsingV2.Parse.Char((char)0x2C).Parse(input);
if (!_x2C.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._x2C)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._x2C.Instance, _x2C.Remainder);
            }
        }
    }
    
}
