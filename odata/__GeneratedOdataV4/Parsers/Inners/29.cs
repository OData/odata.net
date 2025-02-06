namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _x6DParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._x6D> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._x6D>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._x6D> Parse(IInput<char>? input)
            {
                var _x6D = CombinatorParsingV2.Parse.Char((char)0x6D).Parse(input);
if (!_x6D.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._x6D)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._x6D.Instance, _x6D.Remainder);
            }
        }
    }
    
}
