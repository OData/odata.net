namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _x38Parser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._x38> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._x38>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._x38> Parse(IInput<char>? input)
            {
                var _x38 = CombinatorParsingV2.Parse.Char((char)0x38).Parse(input);
if (!_x38.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._x38)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._x38.Instance, _x38.Remainder);
            }
        }
    }
    
}
