namespace __GeneratedOdata.Parsers.Inners
{
    using Sprache;
    
    public static class _ʺx26ʺ_entityIdOptionParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Inners._ʺx26ʺ_entityIdOption> Instance { get; } = from _ʺx26ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx26ʺParser.Instance
from _entityIdOption_1 in __GeneratedOdata.Parsers.Rules._entityIdOptionParser.Instance
select new __GeneratedOdata.CstNodes.Inners._ʺx26ʺ_entityIdOption(_ʺx26ʺ_1, _entityIdOption_1);
    }
    
}
