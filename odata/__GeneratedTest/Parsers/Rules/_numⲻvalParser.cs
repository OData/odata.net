namespace __GeneratedTest.Parsers.Rules
{
    using Sprache;
    
    public static class _numⲻvalParser
    {
        public static Parser<__Generated.CstNodes.Rules._numⲻval> Instance { get; } = from _ʺx25ʺ_1 in __GeneratedTest.Parsers.Inners._ʺx25ʺParser.Instance
from _ⲤbinⲻvalⳆdecⲻvalⳆhexⲻvalↃ_1 in __GeneratedTest.Parsers.Inners._ⲤbinⲻvalⳆdecⲻvalⳆhexⲻvalↃParser.Instance
select new __Generated.CstNodes.Rules._numⲻval(_ʺx25ʺ_1, _ⲤbinⲻvalⳆdecⲻvalⳆhexⲻvalↃ_1);
    }
    
}
