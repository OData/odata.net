namespace __Generated.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _cⲻwspParser
    {
        public static IParser<char, __Generated.CstNodes.Rules._cⲻwsp> Instance { get; } = (_WSPParser.Instance).Or<char, __Generated.CstNodes.Rules._cⲻwsp>(_Ⲥcⲻnl_WSPↃParser.Instance);
        
        public static class _WSPParser
        {
            public static IParser<char, __Generated.CstNodes.Rules._cⲻwsp._WSP> Instance { get; } = from _WSP_1 in __Generated.Parsers.Rules._WSPParser.Instance
select new __Generated.CstNodes.Rules._cⲻwsp._WSP(_WSP_1);
        }
        
        public static class _Ⲥcⲻnl_WSPↃParser
        {
            public static IParser<char, __Generated.CstNodes.Rules._cⲻwsp._Ⲥcⲻnl_WSPↃ> Instance { get; } = from _Ⲥcⲻnl_WSPↃ_1 in __Generated.Parsers.Inners._Ⲥcⲻnl_WSPↃParser.Instance
select new __Generated.CstNodes.Rules._cⲻwsp._Ⲥcⲻnl_WSPↃ(_Ⲥcⲻnl_WSPↃ_1);
        }
    }
    
}
