namespace __GeneratedOdata.Parsers.Inners
{
    using Sprache;
    
    public static class _ʺx26ʺ_metadataOptionParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Inners._ʺx26ʺ_metadataOption> Instance { get; } = from _ʺx26ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx26ʺParser.Instance
from _metadataOption_1 in __GeneratedOdata.Parsers.Rules._metadataOptionParser.Instance
select new __GeneratedOdata.CstNodes.Inners._ʺx26ʺ_metadataOption(_ʺx26ʺ_1, _metadataOption_1);
    }
    
}
