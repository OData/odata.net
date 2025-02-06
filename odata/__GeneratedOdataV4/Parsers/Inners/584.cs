namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ⲤSTARⳆnamespace_ʺx2Eʺ_ⲤtermNameⳆSTARↃↃParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ⲤSTARⳆnamespace_ʺx2Eʺ_ⲤtermNameⳆSTARↃↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ⲤSTARⳆnamespace_ʺx2Eʺ_ⲤtermNameⳆSTARↃↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ⲤSTARⳆnamespace_ʺx2Eʺ_ⲤtermNameⳆSTARↃↃ> Parse(IInput<char>? input)
            {
                var _STARⳆnamespace_ʺx2Eʺ_ⲤtermNameⳆSTARↃ_1 = __GeneratedOdataV4.Parsers.Inners._STARⳆnamespace_ʺx2Eʺ_ⲤtermNameⳆSTARↃParser.Instance.Parse(input);
if (!_STARⳆnamespace_ʺx2Eʺ_ⲤtermNameⳆSTARↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ⲤSTARⳆnamespace_ʺx2Eʺ_ⲤtermNameⳆSTARↃↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._ⲤSTARⳆnamespace_ʺx2Eʺ_ⲤtermNameⳆSTARↃↃ(_STARⳆnamespace_ʺx2Eʺ_ⲤtermNameⳆSTARↃ_1.Parsed), _STARⳆnamespace_ʺx2Eʺ_ⲤtermNameⳆSTARↃ_1.Remainder);
            }
        }
    }
    
}
