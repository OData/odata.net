namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ⲤtermNameⳆSTARↃParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ⲤtermNameⳆSTARↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ⲤtermNameⳆSTARↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ⲤtermNameⳆSTARↃ> Parse(IInput<char>? input)
            {
                var _termNameⳆSTAR_1 = __GeneratedOdataV4.Parsers.Inners._termNameⳆSTARParser.Instance.Parse(input);
if (!_termNameⳆSTAR_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ⲤtermNameⳆSTARↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._ⲤtermNameⳆSTARↃ(_termNameⳆSTAR_1.Parsed), _termNameⳆSTAR_1.Remainder);
            }
        }
    }
    
}
