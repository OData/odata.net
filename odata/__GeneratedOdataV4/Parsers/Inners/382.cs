namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _x35Parser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._x35> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._x35>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._x35> Parse(IInput<char>? input)
            {
                var _x35 = CombinatorParsingV2.Parse.Char((char)0x35).Parse(input);
if (!_x35.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._x35)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._x35.Instance, _x35.Remainder);
            }
        }
    }
    
}
