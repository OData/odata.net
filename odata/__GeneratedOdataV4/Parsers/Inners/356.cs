namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _Ⲥquotationⲻmark_primitiveColProperty_quotationⲻmark_nameⲻseparator_primitiveColInUriↃParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._Ⲥquotationⲻmark_primitiveColProperty_quotationⲻmark_nameⲻseparator_primitiveColInUriↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._Ⲥquotationⲻmark_primitiveColProperty_quotationⲻmark_nameⲻseparator_primitiveColInUriↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._Ⲥquotationⲻmark_primitiveColProperty_quotationⲻmark_nameⲻseparator_primitiveColInUriↃ> Parse(IInput<char>? input)
            {
                var _quotationⲻmark_primitiveColProperty_quotationⲻmark_nameⲻseparator_primitiveColInUri_1 = __GeneratedOdataV4.Parsers.Inners._quotationⲻmark_primitiveColProperty_quotationⲻmark_nameⲻseparator_primitiveColInUriParser.Instance.Parse(input);
if (!_quotationⲻmark_primitiveColProperty_quotationⲻmark_nameⲻseparator_primitiveColInUri_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._Ⲥquotationⲻmark_primitiveColProperty_quotationⲻmark_nameⲻseparator_primitiveColInUriↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._Ⲥquotationⲻmark_primitiveColProperty_quotationⲻmark_nameⲻseparator_primitiveColInUriↃ(_quotationⲻmark_primitiveColProperty_quotationⲻmark_nameⲻseparator_primitiveColInUri_1.Parsed), _quotationⲻmark_primitiveColProperty_quotationⲻmark_nameⲻseparator_primitiveColInUri_1.Remainder);
            }
        }
    }
    
}
