namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _Ⲥʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺⳆʺx34ʺⳆʺx35ʺↃParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._Ⲥʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺⳆʺx34ʺⳆʺx35ʺↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._Ⲥʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺⳆʺx34ʺⳆʺx35ʺↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._Ⲥʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺⳆʺx34ʺⳆʺx35ʺↃ> Parse(IInput<char>? input)
            {
                var _ʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺⳆʺx34ʺⳆʺx35ʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺⳆʺx34ʺⳆʺx35ʺParser.Instance.Parse(input);
if (!_ʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺⳆʺx34ʺⳆʺx35ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._Ⲥʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺⳆʺx34ʺⳆʺx35ʺↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._Ⲥʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺⳆʺx34ʺⳆʺx35ʺↃ(_ʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺⳆʺx34ʺⳆʺx35ʺ_1.Parsed), _ʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺⳆʺx34ʺⳆʺx35ʺ_1.Remainder);
            }
        }
    }
    
}
