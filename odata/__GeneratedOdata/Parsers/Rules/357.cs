namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _continueOnErrorPreferenceParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._continueOnErrorPreference> Instance { get; } = from _ʺx6Fx64x61x74x61x2Eʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx6Fx64x61x74x61x2EʺParser.Instance.Optional()
from _ʺx63x6Fx6Ex74x69x6Ex75x65x2Dx6Fx6Ex2Dx65x72x72x6Fx72ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx63x6Fx6Ex74x69x6Ex75x65x2Dx6Fx6Ex2Dx65x72x72x6Fx72ʺParser.Instance
from _EQⲻh_booleanValue_1 in __GeneratedOdata.Parsers.Inners._EQⲻh_booleanValueParser.Instance.Optional()
select new __GeneratedOdata.CstNodes.Rules._continueOnErrorPreference(_ʺx6Fx64x61x74x61x2Eʺ_1.GetOrElse(null), _ʺx63x6Fx6Ex74x69x6Ex75x65x2Dx6Fx6Ex2Dx65x72x72x6Fx72ʺ_1, _EQⲻh_booleanValue_1.GetOrElse(null));
    }
    
}
