namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ⲤescapeⳆquotationⲻmarkↃParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ⲤescapeⳆquotationⲻmarkↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ⲤescapeⳆquotationⲻmarkↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ⲤescapeⳆquotationⲻmarkↃ> Parse(IInput<char>? input)
            {
                var _escapeⳆquotationⲻmark_1 = __GeneratedOdataV4.Parsers.Inners._escapeⳆquotationⲻmarkParser.Instance.Parse(input);
if (!_escapeⳆquotationⲻmark_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ⲤescapeⳆquotationⲻmarkↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._ⲤescapeⳆquotationⲻmarkↃ(_escapeⳆquotationⲻmark_1.Parsed), _escapeⳆquotationⲻmark_1.Remainder);
            }
        }
    }
    
}
