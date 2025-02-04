namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _Ⲥʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺↃParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._Ⲥʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._Ⲥʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._Ⲥʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺↃ> Parse(IInput<char>? input)
            {
                var _ʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺParser.Instance.Parse(input);
if (!_ʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._Ⲥʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._Ⲥʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺↃ(_ʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺ_1.Parsed), _ʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺ_1.Remainder);
            }
        }
    }
    
}
