namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _base64charParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._base64char> Instance { get; } = (_ALPHAParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._base64char>(_DIGITParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._base64char>(_ʺx2DʺParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._base64char>(_ʺx5FʺParser.Instance);
        
        public static class _ALPHAParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._base64char._ALPHA> Instance { get; } = from _ALPHA_1 in __GeneratedOdataV2.Parsers.Rules._ALPHAParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._base64char._ALPHA(_ALPHA_1);
        }
        
        public static class _DIGITParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._base64char._DIGIT> Instance { get; } = from _DIGIT_1 in __GeneratedOdataV2.Parsers.Rules._DIGITParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._base64char._DIGIT(_DIGIT_1);
        }
        
        public static class _ʺx2DʺParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._base64char._ʺx2Dʺ> Instance { get; } = from _ʺx2Dʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx2DʺParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._base64char._ʺx2Dʺ(_ʺx2Dʺ_1);
        }
        
        public static class _ʺx5FʺParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._base64char._ʺx5Fʺ> Instance { get; } = from _ʺx5Fʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx5FʺParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._base64char._ʺx5Fʺ(_ʺx5Fʺ_1);
        }
    }
    
}
