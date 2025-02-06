namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ⲤSTARⳆ1ЖunreservedↃParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ⲤSTARⳆ1ЖunreservedↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ⲤSTARⳆ1ЖunreservedↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ⲤSTARⳆ1ЖunreservedↃ> Parse(IInput<char>? input)
            {
                var _STARⳆ1Жunreserved_1 = __GeneratedOdataV4.Parsers.Inners._STARⳆ1ЖunreservedParser.Instance.Parse(input);
if (!_STARⳆ1Жunreserved_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ⲤSTARⳆ1ЖunreservedↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._ⲤSTARⳆ1ЖunreservedↃ(_STARⳆ1Жunreserved_1.Parsed), _STARⳆ1Жunreserved_1.Remainder);
            }
        }
    }
    
}
