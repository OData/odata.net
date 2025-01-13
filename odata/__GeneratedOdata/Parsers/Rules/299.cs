namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _monthParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._month> Instance { get; } = (_ʺx30ʺ_oneToNineParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._month>(_ʺx31ʺ_Ⲥʺx30ʺⳆʺx31ʺⳆʺx32ʺↃParser.Instance);
        
        public static class _ʺx30ʺ_oneToNineParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._month._ʺx30ʺ_oneToNine> Instance { get; } = from _ʺx30ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx30ʺParser.Instance
from _oneToNine_1 in __GeneratedOdata.Parsers.Rules._oneToNineParser.Instance
select new __GeneratedOdata.CstNodes.Rules._month._ʺx30ʺ_oneToNine(_ʺx30ʺ_1, _oneToNine_1);
        }
        
        public static class _ʺx31ʺ_Ⲥʺx30ʺⳆʺx31ʺⳆʺx32ʺↃParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._month._ʺx31ʺ_Ⲥʺx30ʺⳆʺx31ʺⳆʺx32ʺↃ> Instance { get; } = from _ʺx31ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx31ʺParser.Instance
from _Ⲥʺx30ʺⳆʺx31ʺⳆʺx32ʺↃ_1 in __GeneratedOdata.Parsers.Inners._Ⲥʺx30ʺⳆʺx31ʺⳆʺx32ʺↃParser.Instance
select new __GeneratedOdata.CstNodes.Rules._month._ʺx31ʺ_Ⲥʺx30ʺⳆʺx31ʺⳆʺx32ʺↃ(_ʺx31ʺ_1, _Ⲥʺx30ʺⳆʺx31ʺⳆʺx32ʺↃ_1);
        }
    }
    
}
