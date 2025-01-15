namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _metadataOptionsParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._metadataOptions> Instance { get; } = from _metadataOption_1 in __GeneratedOdata.Parsers.Rules._metadataOptionParser.Instance
from _Ⲥʺx26ʺ_metadataOptionↃ_1 in Inners._Ⲥʺx26ʺ_metadataOptionↃParser.Instance.Many()
select new __GeneratedOdata.CstNodes.Rules._metadataOptions(_metadataOption_1, _Ⲥʺx26ʺ_metadataOptionↃ_1);
    }
    
}
