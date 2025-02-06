namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _Ⲥʺx24x69x6Ex64x65x78ʺⳆʺx69x6Ex64x65x78ʺↃParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._Ⲥʺx24x69x6Ex64x65x78ʺⳆʺx69x6Ex64x65x78ʺↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._Ⲥʺx24x69x6Ex64x65x78ʺⳆʺx69x6Ex64x65x78ʺↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._Ⲥʺx24x69x6Ex64x65x78ʺⳆʺx69x6Ex64x65x78ʺↃ> Parse(IInput<char>? input)
            {
                var _ʺx24x69x6Ex64x65x78ʺⳆʺx69x6Ex64x65x78ʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx24x69x6Ex64x65x78ʺⳆʺx69x6Ex64x65x78ʺParser.Instance.Parse(input);
if (!_ʺx24x69x6Ex64x65x78ʺⳆʺx69x6Ex64x65x78ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._Ⲥʺx24x69x6Ex64x65x78ʺⳆʺx69x6Ex64x65x78ʺↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._Ⲥʺx24x69x6Ex64x65x78ʺⳆʺx69x6Ex64x65x78ʺↃ(_ʺx24x69x6Ex64x65x78ʺⳆʺx69x6Ex64x65x78ʺ_1.Parsed), _ʺx24x69x6Ex64x65x78ʺⳆʺx69x6Ex64x65x78ʺ_1.Remainder);
            }
        }
    }
    
}
