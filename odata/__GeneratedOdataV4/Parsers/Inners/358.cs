namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _Ⲥquotationⲻmark_complexColProperty_quotationⲻmark_nameⲻseparator_complexColInUriↃParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._Ⲥquotationⲻmark_complexColProperty_quotationⲻmark_nameⲻseparator_complexColInUriↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._Ⲥquotationⲻmark_complexColProperty_quotationⲻmark_nameⲻseparator_complexColInUriↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._Ⲥquotationⲻmark_complexColProperty_quotationⲻmark_nameⲻseparator_complexColInUriↃ> Parse(IInput<char>? input)
            {
                var _quotationⲻmark_complexColProperty_quotationⲻmark_nameⲻseparator_complexColInUri_1 = __GeneratedOdataV4.Parsers.Inners._quotationⲻmark_complexColProperty_quotationⲻmark_nameⲻseparator_complexColInUriParser.Instance.Parse(input);
if (!_quotationⲻmark_complexColProperty_quotationⲻmark_nameⲻseparator_complexColInUri_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._Ⲥquotationⲻmark_complexColProperty_quotationⲻmark_nameⲻseparator_complexColInUriↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._Ⲥquotationⲻmark_complexColProperty_quotationⲻmark_nameⲻseparator_complexColInUriↃ(_quotationⲻmark_complexColProperty_quotationⲻmark_nameⲻseparator_complexColInUri_1.Parsed), _quotationⲻmark_complexColProperty_quotationⲻmark_nameⲻseparator_complexColInUri_1.Remainder);
            }
        }
    }
    
}
