namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _x27Parser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._x27> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._x27>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._x27> Parse(IInput<char>? input)
            {
                var _x27 = CombinatorParsingV2.Parse.Char((char)0x27).Parse(input);
if (!_x27.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._x27)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._x27.Instance, _x27.Remainder);
            }
        }
    }
    
}
