namespace __GeneratedTest.Parsers.Rules
{
    using Sprache;
    
    public static class _charⲻvalParser
    {
        public static Parser<__Generated.CstNodes.Rules._charⲻval> Instance { get; } = from _DQUOTE_1 in __GeneratedTest.Parsers.Rules._DQUOTEParser.Instance
from _ⲤⰃx20ⲻ21ⳆⰃx23ⲻ7EↃ_1 in __GeneratedTest.Parsers.Inners._ⲤⰃx20ⲻ21ⳆⰃx23ⲻ7EↃParser.Instance.Many()
from _DQUOTE_2 in __GeneratedTest.Parsers.Rules._DQUOTEParser.Instance
select new __Generated.CstNodes.Rules._charⲻval(_DQUOTE_1, _ⲤⰃx20ⲻ21ⳆⰃx23ⲻ7EↃ_1, _DQUOTE_2);
    }
    
}
