namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _x7AParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._x7A> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._x7A>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._x7A> Parse(IInput<char>? input)
            {
                var _x7A = CombinatorParsingV2.Parse.Char((char)0x7A).Parse(input);
if (!_x7A.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._x7A)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._x7A.Instance, _x7A.Remainder);
            }
        }
    }
    
}
