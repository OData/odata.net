namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _includeAnnotationsPreferenceParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._includeAnnotationsPreference> Instance { get; } = from _ʺx6Fx64x61x74x61x2Eʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx6Fx64x61x74x61x2EʺParser.Instance.Optional()
from _ʺx69x6Ex63x6Cx75x64x65x2Dx61x6Ex6Ex6Fx74x61x74x69x6Fx6Ex73ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx69x6Ex63x6Cx75x64x65x2Dx61x6Ex6Ex6Fx74x61x74x69x6Fx6Ex73ʺParser.Instance
from _EQⲻh_1 in __GeneratedOdata.Parsers.Rules._EQⲻhParser.Instance
from _DQUOTE_1 in __GeneratedOdata.Parsers.Rules._DQUOTEParser.Instance
from _annotationsList_1 in __GeneratedOdata.Parsers.Rules._annotationsListParser.Instance
from _DQUOTE_2 in __GeneratedOdata.Parsers.Rules._DQUOTEParser.Instance
select new __GeneratedOdata.CstNodes.Rules._includeAnnotationsPreference(_ʺx6Fx64x61x74x61x2Eʺ_1.GetOrElse(null), _ʺx69x6Ex63x6Cx75x64x65x2Dx61x6Ex6Ex6Fx74x61x74x69x6Fx6Ex73ʺ_1, _EQⲻh_1, _DQUOTE_1, _annotationsList_1, _DQUOTE_2);
    }
    
}
