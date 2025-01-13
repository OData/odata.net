namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _dayParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._day> Instance { get; } = (_ʺx30ʺ_oneToNineParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._day>(_Ⲥʺx31ʺⳆʺx32ʺↃ_DIGITParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._day>(_ʺx33ʺ_Ⲥʺx30ʺⳆʺx31ʺↃParser.Instance);
        
        public static class _ʺx30ʺ_oneToNineParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._day._ʺx30ʺ_oneToNine> Instance { get; } = from _ʺx30ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx30ʺParser.Instance
from _oneToNine_1 in __GeneratedOdata.Parsers.Rules._oneToNineParser.Instance
select new __GeneratedOdata.CstNodes.Rules._day._ʺx30ʺ_oneToNine(_ʺx30ʺ_1, _oneToNine_1);
        }
        
        public static class _Ⲥʺx31ʺⳆʺx32ʺↃ_DIGITParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._day._Ⲥʺx31ʺⳆʺx32ʺↃ_DIGIT> Instance { get; } = from _Ⲥʺx31ʺⳆʺx32ʺↃ_1 in __GeneratedOdata.Parsers.Inners._Ⲥʺx31ʺⳆʺx32ʺↃParser.Instance
from _DIGIT_1 in __GeneratedOdata.Parsers.Rules._DIGITParser.Instance
select new __GeneratedOdata.CstNodes.Rules._day._Ⲥʺx31ʺⳆʺx32ʺↃ_DIGIT(_Ⲥʺx31ʺⳆʺx32ʺↃ_1, _DIGIT_1);
        }
        
        public static class _ʺx33ʺ_Ⲥʺx30ʺⳆʺx31ʺↃParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._day._ʺx33ʺ_Ⲥʺx30ʺⳆʺx31ʺↃ> Instance { get; } = from _ʺx33ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx33ʺParser.Instance
from _Ⲥʺx30ʺⳆʺx31ʺↃ_1 in __GeneratedOdata.Parsers.Inners._Ⲥʺx30ʺⳆʺx31ʺↃParser.Instance
select new __GeneratedOdata.CstNodes.Rules._day._ʺx33ʺ_Ⲥʺx30ʺⳆʺx31ʺↃ(_ʺx33ʺ_1, _Ⲥʺx30ʺⳆʺx31ʺↃ_1);
        }
    }
    
}
