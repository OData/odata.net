namespace __GeneratedOdata.Parsers.Inners
{
    using Sprache;
    
    public static class _oneToNine_ЖDIGITParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Inners._oneToNine_ЖDIGIT> Instance { get; } = from _oneToNine_1 in __GeneratedOdata.Parsers.Rules._oneToNineParser.Instance
from _DIGIT_1 in __GeneratedOdata.Parsers.Rules._DIGITParser.Instance.Many()
select new __GeneratedOdata.CstNodes.Inners._oneToNine_ЖDIGIT(_oneToNine_1, _DIGIT_1);
    }
    
}
