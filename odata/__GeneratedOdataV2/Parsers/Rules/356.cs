namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _callbackPreferenceParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._callbackPreference> Instance { get; } = from _ʺx6Fx64x61x74x61x2Eʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx6Fx64x61x74x61x2EʺParser.Instance.Optional()
from _ʺx63x61x6Cx6Cx62x61x63x6Bʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx63x61x6Cx6Cx62x61x63x6BʺParser.Instance
from _OWS_1 in __GeneratedOdataV2.Parsers.Rules._OWSParser.Instance
from _ʺx3Bʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx3BʺParser.Instance
from _OWS_2 in __GeneratedOdataV2.Parsers.Rules._OWSParser.Instance
from _ʺx75x72x6Cʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx75x72x6CʺParser.Instance
from _EQⲻh_1 in __GeneratedOdataV2.Parsers.Rules._EQⲻhParser.Instance
from _DQUOTE_1 in __GeneratedOdataV2.Parsers.Rules._DQUOTEParser.Instance
from _URI_1 in __GeneratedOdataV2.Parsers.Rules._URIParser.Instance
from _DQUOTE_2 in __GeneratedOdataV2.Parsers.Rules._DQUOTEParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._callbackPreference(_ʺx6Fx64x61x74x61x2Eʺ_1.GetOrElse(null), _ʺx63x61x6Cx6Cx62x61x63x6Bʺ_1, _OWS_1, _ʺx3Bʺ_1, _OWS_2, _ʺx75x72x6Cʺ_1, _EQⲻh_1, _DQUOTE_1, _URI_1, _DQUOTE_2);
    }
    
}
