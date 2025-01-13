namespace __GeneratedTest.Parsers.Rules
{
    using Sprache;
    
    public static class _proseⲻvalParser
    {
        public static Parser<__Generated.CstNodes.Rules._proseⲻval> Instance { get; } = from _ʺx3Cʺ_1 in __GeneratedTest.Parsers.Inners._ʺx3CʺParser.Instance
from _ⲤⰃx20ⲻ3DⳆⰃx3Fⲻ7EↃ_1 in __GeneratedTest.Parsers.Inners._ⲤⰃx20ⲻ3DⳆⰃx3Fⲻ7EↃParser.Instance.Many()
from _ʺx3Eʺ_1 in __GeneratedTest.Parsers.Inners._ʺx3EʺParser.Instance
select new __Generated.CstNodes.Rules._proseⲻval(_ʺx3Cʺ_1, _ⲤⰃx20ⲻ3DⳆⰃx3Fⲻ7EↃ_1, _ʺx3Eʺ_1);
    }
    
}
