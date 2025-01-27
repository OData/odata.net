namespace __GeneratedOdata.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx30ʺ_3DIGITⳆoneToNine_3ЖDIGITParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Inners._ʺx30ʺ_3DIGITⳆoneToNine_3ЖDIGIT> Instance { get; } = (_ʺx30ʺ_3DIGITParser.Instance).Or<char, __GeneratedOdata.CstNodes.Inners._ʺx30ʺ_3DIGITⳆoneToNine_3ЖDIGIT>(_oneToNine_3ЖDIGITParser.Instance);
        
        public static class _ʺx30ʺ_3DIGITParser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Inners._ʺx30ʺ_3DIGITⳆoneToNine_3ЖDIGIT._ʺx30ʺ_3DIGIT> Instance { get; } = from _ʺx30ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx30ʺParser.Instance
from _DIGIT_1 in __GeneratedOdata.Parsers.Rules._DIGITParser.Instance.Repeat(3, 3)
select new __GeneratedOdata.CstNodes.Inners._ʺx30ʺ_3DIGITⳆoneToNine_3ЖDIGIT._ʺx30ʺ_3DIGIT(_ʺx30ʺ_1, new __GeneratedOdata.CstNodes.Inners.HelperRangedExactly3<__GeneratedOdata.CstNodes.Rules._DIGIT>(_DIGIT_1));
        }
        
        public static class _oneToNine_3ЖDIGITParser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Inners._ʺx30ʺ_3DIGITⳆoneToNine_3ЖDIGIT._oneToNine_3ЖDIGIT> Instance { get; } = from _oneToNine_1 in __GeneratedOdata.Parsers.Rules._oneToNineParser.Instance
from _DIGIT_1 in __GeneratedOdata.Parsers.Rules._DIGITParser.Instance.Repeat(3, null)
select new __GeneratedOdata.CstNodes.Inners._ʺx30ʺ_3DIGITⳆoneToNine_3ЖDIGIT._oneToNine_3ЖDIGIT(_oneToNine_1, new __GeneratedOdata.CstNodes.Inners.HelperRangedAtLeast3<__GeneratedOdata.CstNodes.Rules._DIGIT>(_DIGIT_1));
        }
    }
    
}
