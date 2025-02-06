namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _x3AParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._x3A> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._x3A>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._x3A> Parse(IInput<char>? input)
            {
                var _x3A = CombinatorParsingV2.Parse.Char((char)0x3A).Parse(input);
if (!_x3A.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._x3A)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._x3A.Instance, _x3A.Remainder);
            }
        }
    }
    
}
