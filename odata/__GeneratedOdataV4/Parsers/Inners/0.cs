namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _x68Parser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._x68> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._x68>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._x68> Parse(IInput<char>? input)
            {
                var _x68 = CombinatorParsingV2.Parse.Char((char)0x68).Parse(input);
if (!_x68.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._x68)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._x68.Instance, _x68.Remainder);
            }
        }
    }
    
}
