namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _preferParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._prefer> Instance { get; } = from _ʺx50x72x65x66x65x72ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx50x72x65x66x65x72ʺParser.Instance
from _ʺx3Aʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx3AʺParser.Instance
from _OWS_1 in __GeneratedOdata.Parsers.Rules._OWSParser.Instance
from _preference_1 in __GeneratedOdata.Parsers.Rules._preferenceParser.Instance
from _ⲤCOMMA_preferenceↃ_1 in Inners._ⲤCOMMA_preferenceↃParser.Instance.Many()
select new __GeneratedOdata.CstNodes.Rules._prefer(_ʺx50x72x65x66x65x72ʺ_1, _ʺx3Aʺ_1, _OWS_1, _preference_1, _ⲤCOMMA_preferenceↃ_1);
    }
    
}
