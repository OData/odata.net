namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _x2AParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._x2A> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._x2A>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._x2A> Parse(IInput<char>? input)
            {
                var _x2A = CombinatorParsingV2.Parse.Char((char)0x2A).Parse(input);
if (!_x2A.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._x2A)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._x2A.Instance, _x2A.Remainder);
            }
        }
    }
    
}
