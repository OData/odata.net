namespace __GeneratedOdata.Parsers.Inners
{
    using Sprache;
    
    public static class _ʺx26ʺ_entityCastOptionParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Inners._ʺx26ʺ_entityCastOption> Instance { get; } = from _ʺx26ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx26ʺParser.Instance
from _entityCastOption_1 in __GeneratedOdata.Parsers.Rules._entityCastOptionParser.Instance
select new __GeneratedOdata.CstNodes.Inners._ʺx26ʺ_entityCastOption(_ʺx26ʺ_1, _entityCastOption_1);
    }
    
}
