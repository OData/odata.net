namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx49ʺParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx49ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx49ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx49ʺ> Parse(IInput<char>? input)
            {
                var _x49_1 = __GeneratedOdataV4.Parsers.Inners._x49Parser.Instance.Parse(input);
if (!_x49_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx49ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._ʺx49ʺ.Instance, _x49_1.Remainder);
            }
        }
    }
    
}
