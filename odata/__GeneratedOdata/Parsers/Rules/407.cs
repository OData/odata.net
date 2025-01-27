namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _unreservedParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._unreserved> Instance { get; } = (_ALPHAParser.Instance).Or<char, __GeneratedOdata.CstNodes.Rules._unreserved>(_DIGITParser.Instance).Or<char, __GeneratedOdata.CstNodes.Rules._unreserved>(_ʺx2DʺParser.Instance).Or<char, __GeneratedOdata.CstNodes.Rules._unreserved>(_ʺx2EʺParser.Instance).Or<char, __GeneratedOdata.CstNodes.Rules._unreserved>(_ʺx5FʺParser.Instance).Or<char, __GeneratedOdata.CstNodes.Rules._unreserved>(_ʺx7EʺParser.Instance);
        
        public static class _ALPHAParser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._unreserved._ALPHA> Instance { get; } = from _ALPHA_1 in __GeneratedOdata.Parsers.Rules._ALPHAParser.Instance
select new __GeneratedOdata.CstNodes.Rules._unreserved._ALPHA(_ALPHA_1);
        }
        
        public static class _DIGITParser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._unreserved._DIGIT> Instance { get; } = from _DIGIT_1 in __GeneratedOdata.Parsers.Rules._DIGITParser.Instance
select new __GeneratedOdata.CstNodes.Rules._unreserved._DIGIT(_DIGIT_1);
        }
        
        public static class _ʺx2DʺParser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._unreserved._ʺx2Dʺ> Instance { get; } = from _ʺx2Dʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx2DʺParser.Instance
select new __GeneratedOdata.CstNodes.Rules._unreserved._ʺx2Dʺ(_ʺx2Dʺ_1);
        }
        
        public static class _ʺx2EʺParser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._unreserved._ʺx2Eʺ> Instance { get; } = from _ʺx2Eʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx2EʺParser.Instance
select new __GeneratedOdata.CstNodes.Rules._unreserved._ʺx2Eʺ(_ʺx2Eʺ_1);
        }
        
        public static class _ʺx5FʺParser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._unreserved._ʺx5Fʺ> Instance { get; } = from _ʺx5Fʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx5FʺParser.Instance
select new __GeneratedOdata.CstNodes.Rules._unreserved._ʺx5Fʺ(_ʺx5Fʺ_1);
        }
        
        public static class _ʺx7EʺParser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._unreserved._ʺx7Eʺ> Instance { get; } = from _ʺx7Eʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx7EʺParser.Instance
select new __GeneratedOdata.CstNodes.Rules._unreserved._ʺx7Eʺ(_ʺx7Eʺ_1);
        }
    }
    
}
