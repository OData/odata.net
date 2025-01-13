namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _indexParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._index> Instance { get; } = from _Ⲥʺx24x69x6Ex64x65x78ʺⳆʺx69x6Ex64x65x78ʺↃ_1 in __GeneratedOdata.Parsers.Inners._Ⲥʺx24x69x6Ex64x65x78ʺⳆʺx69x6Ex64x65x78ʺↃParser.Instance
from _EQ_1 in __GeneratedOdata.Parsers.Rules._EQParser.Instance
from _DIGIT_1 in __GeneratedOdata.Parsers.Rules._DIGITParser.Instance.Many()
select new __GeneratedOdata.CstNodes.Rules._index(_Ⲥʺx24x69x6Ex64x65x78ʺⳆʺx69x6Ex64x65x78ʺↃ_1, _EQ_1, _DIGIT_1);
    }
    
}
