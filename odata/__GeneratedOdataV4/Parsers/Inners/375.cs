namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _x7DParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._x7D> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._x7D>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._x7D> Parse(IInput<char>? input)
            {
                var _x7D = CombinatorParsingV2.Parse.Char((char)0x7D).Parse(input);
if (!_x7D.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._x7D)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._x7D.Instance, _x7D.Remainder);
            }
        }
    }
    
}
