namespace __Generated.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ALPHAⳆDIGITⳆʺx2DʺParser
    {
        public static IParser<char, __Generated.CstNodes.Inners._ALPHAⳆDIGITⳆʺx2Dʺ> Instance { get; } = (_ALPHAParser.Instance).Or<char, __Generated.CstNodes.Inners._ALPHAⳆDIGITⳆʺx2Dʺ>(_DIGITParser.Instance).Or<char, __Generated.CstNodes.Inners._ALPHAⳆDIGITⳆʺx2Dʺ>(_ʺx2DʺParser.Instance);
        
        public static class _ALPHAParser
        {
            public static IParser<char, __Generated.CstNodes.Inners._ALPHAⳆDIGITⳆʺx2Dʺ._ALPHA> Instance { get; } = from _ALPHA_1 in __Generated.Parsers.Rules._ALPHAParser.Instance
select new __Generated.CstNodes.Inners._ALPHAⳆDIGITⳆʺx2Dʺ._ALPHA(_ALPHA_1);
        }
        
        public static class _DIGITParser
        {
            public static IParser<char, __Generated.CstNodes.Inners._ALPHAⳆDIGITⳆʺx2Dʺ._DIGIT> Instance { get; } = from _DIGIT_1 in __Generated.Parsers.Rules._DIGITParser.Instance
select new __Generated.CstNodes.Inners._ALPHAⳆDIGITⳆʺx2Dʺ._DIGIT(_DIGIT_1);
        }
        
        public static class _ʺx2DʺParser
        {
            public static IParser<char, __Generated.CstNodes.Inners._ALPHAⳆDIGITⳆʺx2Dʺ._ʺx2Dʺ> Instance { get; } = from _ʺx2Dʺ_1 in __Generated.Parsers.Inners._ʺx2DʺParser.Instance
select new __Generated.CstNodes.Inners._ALPHAⳆDIGITⳆʺx2Dʺ._ʺx2Dʺ(_ʺx2Dʺ_1);
        }
    }
    
}
