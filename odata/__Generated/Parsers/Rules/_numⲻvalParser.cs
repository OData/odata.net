namespace __Generated.Parsers.Rules
{
    using Sprache;
    
    public static class _numⲻvalParser
    {
        public static Parser<__Generated.CstNodes.Rules._numⲻval> Instance { get; } = from _ʺx25ʺ_1 in __Generated.Parsers.Inners._ʺx25ʺParser.Instance
from _ⲤbinⲻvalⳆdecⲻvalⳆhexⲻvalↃ_1 in __Generated.Parsers.Inners._ⲤbinⲻvalⳆdecⲻvalⳆhexⲻvalↃParser.Instance
select new __Generated.CstNodes.Rules._numⲻval(_ʺx25ʺ_1, _ⲤbinⲻvalⳆdecⲻvalⳆhexⲻvalↃ_1);
    }
    
}
