namespace __GeneratedOdata.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _escapeⳆquotationⲻmarkParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Inners._escapeⳆquotationⲻmark> Instance { get; } = (_escapeParser.Instance).Or<char, __GeneratedOdata.CstNodes.Inners._escapeⳆquotationⲻmark>(_quotationⲻmarkParser.Instance);
        
        public static class _escapeParser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Inners._escapeⳆquotationⲻmark._escape> Instance { get; } = from _escape_1 in __GeneratedOdata.Parsers.Rules._escapeParser.Instance
select new __GeneratedOdata.CstNodes.Inners._escapeⳆquotationⲻmark._escape(_escape_1);
        }
        
        public static class _quotationⲻmarkParser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Inners._escapeⳆquotationⲻmark._quotationⲻmark> Instance { get; } = from _quotationⲻmark_1 in __GeneratedOdata.Parsers.Rules._quotationⲻmarkParser.Instance
select new __GeneratedOdata.CstNodes.Inners._escapeⳆquotationⲻmark._quotationⲻmark(_quotationⲻmark_1);
        }
    }
    
}
