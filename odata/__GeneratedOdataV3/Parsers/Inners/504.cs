namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx50ʺParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx50ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx50ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx50ʺ> Parse(IInput<char>? input)
            {
                var _x50_1 = __GeneratedOdataV3.Parsers.Inners._x50Parser.Instance.Parse(input);
if (!_x50_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx50ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._ʺx50ʺ.Instance, _x50_1.Remainder);
            }
        }
    }
    
}
