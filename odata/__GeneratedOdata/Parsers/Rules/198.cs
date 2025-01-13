namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _collectionPropertyInUriParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._collectionPropertyInUri> Instance { get; } = (_Ⲥquotationⲻmark_primitiveColProperty_quotationⲻmark_nameⲻseparator_primitiveColInUriↃParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._collectionPropertyInUri>(_Ⲥquotationⲻmark_complexColProperty_quotationⲻmark_nameⲻseparator_complexColInUriↃParser.Instance);
        
        public static class _Ⲥquotationⲻmark_primitiveColProperty_quotationⲻmark_nameⲻseparator_primitiveColInUriↃParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._collectionPropertyInUri._Ⲥquotationⲻmark_primitiveColProperty_quotationⲻmark_nameⲻseparator_primitiveColInUriↃ> Instance { get; } = from _Ⲥquotationⲻmark_primitiveColProperty_quotationⲻmark_nameⲻseparator_primitiveColInUriↃ_1 in __GeneratedOdata.Parsers.Inners._Ⲥquotationⲻmark_primitiveColProperty_quotationⲻmark_nameⲻseparator_primitiveColInUriↃParser.Instance
select new __GeneratedOdata.CstNodes.Rules._collectionPropertyInUri._Ⲥquotationⲻmark_primitiveColProperty_quotationⲻmark_nameⲻseparator_primitiveColInUriↃ(_Ⲥquotationⲻmark_primitiveColProperty_quotationⲻmark_nameⲻseparator_primitiveColInUriↃ_1);
        }
        
        public static class _Ⲥquotationⲻmark_complexColProperty_quotationⲻmark_nameⲻseparator_complexColInUriↃParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._collectionPropertyInUri._Ⲥquotationⲻmark_complexColProperty_quotationⲻmark_nameⲻseparator_complexColInUriↃ> Instance { get; } = from _Ⲥquotationⲻmark_complexColProperty_quotationⲻmark_nameⲻseparator_complexColInUriↃ_1 in __GeneratedOdata.Parsers.Inners._Ⲥquotationⲻmark_complexColProperty_quotationⲻmark_nameⲻseparator_complexColInUriↃParser.Instance
select new __GeneratedOdata.CstNodes.Rules._collectionPropertyInUri._Ⲥquotationⲻmark_complexColProperty_quotationⲻmark_nameⲻseparator_complexColInUriↃ(_Ⲥquotationⲻmark_complexColProperty_quotationⲻmark_nameⲻseparator_complexColInUriↃ_1);
        }
    }
    
}
