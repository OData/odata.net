namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _Ⲥʺx30ʺⳆʺx31ʺↃParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._Ⲥʺx30ʺⳆʺx31ʺↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._Ⲥʺx30ʺⳆʺx31ʺↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._Ⲥʺx30ʺⳆʺx31ʺↃ> Parse(IInput<char>? input)
            {
                var _ʺx30ʺⳆʺx31ʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx30ʺⳆʺx31ʺParser.Instance.Parse(input);
if (!_ʺx30ʺⳆʺx31ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._Ⲥʺx30ʺⳆʺx31ʺↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._Ⲥʺx30ʺⳆʺx31ʺↃ(_ʺx30ʺⳆʺx31ʺ_1.Parsed), _ʺx30ʺⳆʺx31ʺ_1.Remainder);
            }
        }
    }
    
}
