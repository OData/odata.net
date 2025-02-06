namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx2EʺParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx2Eʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx2Eʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx2Eʺ> Parse(IInput<char>? input)
            {
                var _x2E_1 = __GeneratedOdataV4.Parsers.Inners._x2EParser.Instance.Parse(input);
if (!_x2E_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx2Eʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._ʺx2Eʺ.Instance, _x2E_1.Remainder);
            }
        }
    }
    
}
