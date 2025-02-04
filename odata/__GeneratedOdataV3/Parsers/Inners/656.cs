namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ⲤpcharⳆʺx2FʺⳆʺx3FʺↃParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ⲤpcharⳆʺx2FʺⳆʺx3FʺↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ⲤpcharⳆʺx2FʺⳆʺx3FʺↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ⲤpcharⳆʺx2FʺⳆʺx3FʺↃ> Parse(IInput<char>? input)
            {
                var _pcharⳆʺx2FʺⳆʺx3Fʺ_1 = __GeneratedOdataV3.Parsers.Inners._pcharⳆʺx2FʺⳆʺx3FʺParser.Instance.Parse(input);
if (!_pcharⳆʺx2FʺⳆʺx3Fʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ⲤpcharⳆʺx2FʺⳆʺx3FʺↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._ⲤpcharⳆʺx2FʺⳆʺx3FʺↃ(_pcharⳆʺx2FʺⳆʺx3Fʺ_1.Parsed), _pcharⳆʺx2FʺⳆʺx3Fʺ_1.Remainder);
            }
        }
    }
    
}
