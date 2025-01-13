namespace __GeneratedOdata.Parsers.Inners
{
    using Sprache;
    
    public static class _ʺx30ʺ_3DIGITⳆoneToNine_3ЖDIGITParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Inners._ʺx30ʺ_3DIGITⳆoneToNine_3ЖDIGIT> Instance { get; } = (_ʺx30ʺ_3DIGITParser.Instance).Or<__GeneratedOdata.CstNodes.Inners._ʺx30ʺ_3DIGITⳆoneToNine_3ЖDIGIT>(_oneToNine_3ЖDIGITParser.Instance);
        
        public static class _ʺx30ʺ_3DIGITParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Inners._ʺx30ʺ_3DIGITⳆoneToNine_3ЖDIGIT._ʺx30ʺ_3DIGIT> Instance { get; } = from _ʺx30ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx30ʺParser.Instance
from _DIGIT_1 in __GeneratedOdata.Parsers.Rules._DIGITParser.Instance.Many()
select new __GeneratedOdata.CstNodes.Inners._ʺx30ʺ_3DIGITⳆoneToNine_3ЖDIGIT._ʺx30ʺ_3DIGIT(_ʺx30ʺ_1, _DIGIT_1);
        }
        
        public static class _oneToNine_3ЖDIGITParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Inners._ʺx30ʺ_3DIGITⳆoneToNine_3ЖDIGIT._oneToNine_3ЖDIGIT> Instance { get; } = from _oneToNine_1 in __GeneratedOdata.Parsers.Rules._oneToNineParser.Instance
from _DIGIT_1 in __GeneratedOdata.Parsers.Rules._DIGITParser.Instance.Many()
select new __GeneratedOdata.CstNodes.Inners._ʺx30ʺ_3DIGITⳆoneToNine_3ЖDIGIT._oneToNine_3ЖDIGIT(_oneToNine_1, _DIGIT_1);
        }
    }
    
}
