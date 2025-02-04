namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _x6AParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._x6A> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._x6A>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._x6A> Parse(IInput<char>? input)
            {
                var _x6A = CombinatorParsingV2.Parse.Char((char)0x6A).Parse(input);
if (!_x6A.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._x6A)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._x6A.Instance, _x6A.Remainder);
            }
        }
    }
    
}
