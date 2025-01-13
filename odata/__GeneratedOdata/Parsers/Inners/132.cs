namespace __GeneratedOdata.Parsers.Inners
{
    using Sprache;
    
    public static class _oneToNine_ЖDIGITⳆʺx6Dx61x78ʺParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Inners._oneToNine_ЖDIGITⳆʺx6Dx61x78ʺ> Instance { get; } = (_oneToNine_ЖDIGITParser.Instance).Or<__GeneratedOdata.CstNodes.Inners._oneToNine_ЖDIGITⳆʺx6Dx61x78ʺ>(_ʺx6Dx61x78ʺParser.Instance);
        
        public static class _oneToNine_ЖDIGITParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Inners._oneToNine_ЖDIGITⳆʺx6Dx61x78ʺ._oneToNine_ЖDIGIT> Instance { get; } = from _oneToNine_1 in __GeneratedOdata.Parsers.Rules._oneToNineParser.Instance
from _DIGIT_1 in __GeneratedOdata.Parsers.Rules._DIGITParser.Instance.Many()
select new __GeneratedOdata.CstNodes.Inners._oneToNine_ЖDIGITⳆʺx6Dx61x78ʺ._oneToNine_ЖDIGIT(_oneToNine_1, _DIGIT_1);
        }
        
        public static class _ʺx6Dx61x78ʺParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Inners._oneToNine_ЖDIGITⳆʺx6Dx61x78ʺ._ʺx6Dx61x78ʺ> Instance { get; } = from _ʺx6Dx61x78ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx6Dx61x78ʺParser.Instance
select new __GeneratedOdata.CstNodes.Inners._oneToNine_ЖDIGITⳆʺx6Dx61x78ʺ._ʺx6Dx61x78ʺ(_ʺx6Dx61x78ʺ_1);
        }
    }
    
}
