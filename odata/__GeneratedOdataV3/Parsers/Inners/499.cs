namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _x5AParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._x5A> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._x5A>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._x5A> Parse(IInput<char>? input)
            {
                var _x5A = CombinatorParsingV2.Parse.Char((char)0x5A).Parse(input);
if (!_x5A.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._x5A)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._x5A.Instance, _x5A.Remainder);
            }
        }
    }
    
}
