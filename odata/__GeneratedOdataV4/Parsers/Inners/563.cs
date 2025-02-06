namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _x56Parser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._x56> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._x56>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._x56> Parse(IInput<char>? input)
            {
                var _x56 = CombinatorParsingV2.Parse.Char((char)0x56).Parse(input);
if (!_x56.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._x56)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._x56.Instance, _x56.Remainder);
            }
        }
    }
    
}
