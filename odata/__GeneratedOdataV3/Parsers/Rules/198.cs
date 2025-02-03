namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _collectionPropertyInUriParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._collectionPropertyInUri> Instance { get; } = (_Ⲥquotationⲻmark_primitiveColProperty_quotationⲻmark_nameⲻseparator_primitiveColInUriↃParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._collectionPropertyInUri>(_Ⲥquotationⲻmark_complexColProperty_quotationⲻmark_nameⲻseparator_complexColInUriↃParser.Instance);
        
        public static class _Ⲥquotationⲻmark_primitiveColProperty_quotationⲻmark_nameⲻseparator_primitiveColInUriↃParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._collectionPropertyInUri._Ⲥquotationⲻmark_primitiveColProperty_quotationⲻmark_nameⲻseparator_primitiveColInUriↃ> Instance { get; } = from _Ⲥquotationⲻmark_primitiveColProperty_quotationⲻmark_nameⲻseparator_primitiveColInUriↃ_1 in __GeneratedOdataV3.Parsers.Inners._Ⲥquotationⲻmark_primitiveColProperty_quotationⲻmark_nameⲻseparator_primitiveColInUriↃParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._collectionPropertyInUri._Ⲥquotationⲻmark_primitiveColProperty_quotationⲻmark_nameⲻseparator_primitiveColInUriↃ(_Ⲥquotationⲻmark_primitiveColProperty_quotationⲻmark_nameⲻseparator_primitiveColInUriↃ_1);
        }
        
        public static class _Ⲥquotationⲻmark_complexColProperty_quotationⲻmark_nameⲻseparator_complexColInUriↃParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._collectionPropertyInUri._Ⲥquotationⲻmark_complexColProperty_quotationⲻmark_nameⲻseparator_complexColInUriↃ> Instance { get; } = from _Ⲥquotationⲻmark_complexColProperty_quotationⲻmark_nameⲻseparator_complexColInUriↃ_1 in __GeneratedOdataV3.Parsers.Inners._Ⲥquotationⲻmark_complexColProperty_quotationⲻmark_nameⲻseparator_complexColInUriↃParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._collectionPropertyInUri._Ⲥquotationⲻmark_complexColProperty_quotationⲻmark_nameⲻseparator_complexColInUriↃ(_Ⲥquotationⲻmark_complexColProperty_quotationⲻmark_nameⲻseparator_complexColInUriↃ_1);
        }
    }
    
}
