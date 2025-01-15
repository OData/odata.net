namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _decⲻoctetParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._decⲻoctet> Instance { get; } = (_ʺx31ʺ_2DIGITParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._decⲻoctet>(_ʺx32ʺ_Ⰳx30ⲻ34_DIGITParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._decⲻoctet>(_ʺx32x35ʺ_Ⰳx30ⲻ35Parser.Instance).Or<__GeneratedOdata.CstNodes.Rules._decⲻoctet>(_Ⰳx31ⲻ39_DIGITParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._decⲻoctet>(_DIGITParser.Instance);
        
        public static class _ʺx31ʺ_2DIGITParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._decⲻoctet._ʺx31ʺ_2DIGIT> Instance { get; } = from _ʺx31ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx31ʺParser.Instance
from _DIGIT_1 in __GeneratedOdata.Parsers.Rules._DIGITParser.Instance.Repeat(2, 2)
select new __GeneratedOdata.CstNodes.Rules._decⲻoctet._ʺx31ʺ_2DIGIT(_ʺx31ʺ_1, new __GeneratedOdata.CstNodes.Inners.HelperRangedExactly2<__GeneratedOdata.CstNodes.Rules._DIGIT>(_DIGIT_1));
        }
        
        public static class _ʺx32ʺ_Ⰳx30ⲻ34_DIGITParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._decⲻoctet._ʺx32ʺ_Ⰳx30ⲻ34_DIGIT> Instance { get; } = from _ʺx32ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx32ʺParser.Instance
from _Ⰳx30ⲻ34_1 in __GeneratedOdata.Parsers.Inners._Ⰳx30ⲻ34Parser.Instance
from _DIGIT_1 in __GeneratedOdata.Parsers.Rules._DIGITParser.Instance
select new __GeneratedOdata.CstNodes.Rules._decⲻoctet._ʺx32ʺ_Ⰳx30ⲻ34_DIGIT(_ʺx32ʺ_1, _Ⰳx30ⲻ34_1, _DIGIT_1);
        }
        
        public static class _ʺx32x35ʺ_Ⰳx30ⲻ35Parser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._decⲻoctet._ʺx32x35ʺ_Ⰳx30ⲻ35> Instance { get; } = from _ʺx32x35ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx32x35ʺParser.Instance
from _Ⰳx30ⲻ35_1 in __GeneratedOdata.Parsers.Inners._Ⰳx30ⲻ35Parser.Instance
select new __GeneratedOdata.CstNodes.Rules._decⲻoctet._ʺx32x35ʺ_Ⰳx30ⲻ35(_ʺx32x35ʺ_1, _Ⰳx30ⲻ35_1);
        }
        
        public static class _Ⰳx31ⲻ39_DIGITParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._decⲻoctet._Ⰳx31ⲻ39_DIGIT> Instance { get; } = from _Ⰳx31ⲻ39_1 in __GeneratedOdata.Parsers.Inners._Ⰳx31ⲻ39Parser.Instance
from _DIGIT_1 in __GeneratedOdata.Parsers.Rules._DIGITParser.Instance
select new __GeneratedOdata.CstNodes.Rules._decⲻoctet._Ⰳx31ⲻ39_DIGIT(_Ⰳx31ⲻ39_1, _DIGIT_1);
        }
        
        public static class _DIGITParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._decⲻoctet._DIGIT> Instance { get; } = from _DIGIT_1 in __GeneratedOdata.Parsers.Rules._DIGITParser.Instance
select new __GeneratedOdata.CstNodes.Rules._decⲻoctet._DIGIT(_DIGIT_1);
        }
    }
    
}
