namespace __GeneratedOdata.Parsers.Inners
{
    using Sprache;
    
    public static class _entityCastOption_ʺx26ʺParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Inners._entityCastOption_ʺx26ʺ> Instance { get; } = from _entityCastOption_1 in __GeneratedOdata.Parsers.Rules._entityCastOptionParser.Instance
from _ʺx26ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx26ʺParser.Instance
select new __GeneratedOdata.CstNodes.Inners._entityCastOption_ʺx26ʺ(_entityCastOption_1, _ʺx26ʺ_1);
    }
    
}
