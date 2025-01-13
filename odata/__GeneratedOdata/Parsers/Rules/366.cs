namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _waitPreferenceParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._waitPreference> Instance { get; } = from _ʺx77x61x69x74ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx77x61x69x74ʺParser.Instance
from _EQⲻh_1 in __GeneratedOdata.Parsers.Rules._EQⲻhParser.Instance
from _DIGIT_1 in __GeneratedOdata.Parsers.Rules._DIGITParser.Instance.Many()
select new __GeneratedOdata.CstNodes.Rules._waitPreference(_ʺx77x61x69x74ʺ_1, _EQⲻh_1, _DIGIT_1);
    }
    
}
