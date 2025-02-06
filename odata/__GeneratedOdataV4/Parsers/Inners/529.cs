namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _Ⲥʺx31ʺⳆʺx32ʺↃParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._Ⲥʺx31ʺⳆʺx32ʺↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._Ⲥʺx31ʺⳆʺx32ʺↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._Ⲥʺx31ʺⳆʺx32ʺↃ> Parse(IInput<char>? input)
            {
                var _ʺx31ʺⳆʺx32ʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx31ʺⳆʺx32ʺParser.Instance.Parse(input);
if (!_ʺx31ʺⳆʺx32ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._Ⲥʺx31ʺⳆʺx32ʺↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._Ⲥʺx31ʺⳆʺx32ʺↃ(_ʺx31ʺⳆʺx32ʺ_1.Parsed), _ʺx31ʺⳆʺx32ʺ_1.Remainder);
            }
        }
    }
    
}
