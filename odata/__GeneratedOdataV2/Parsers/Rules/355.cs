namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _allowEntityReferencesPreferenceParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._allowEntityReferencesPreference> Instance { get; } = from _ʺx6Fx64x61x74x61x2Eʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx6Fx64x61x74x61x2EʺParser.Instance.Optional()
from _ʺx61x6Cx6Cx6Fx77x2Dx65x6Ex74x69x74x79x72x65x66x65x72x65x6Ex63x65x73ʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx61x6Cx6Cx6Fx77x2Dx65x6Ex74x69x74x79x72x65x66x65x72x65x6Ex63x65x73ʺParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._allowEntityReferencesPreference(_ʺx6Fx64x61x74x61x2Eʺ_1.GetOrElse(null), _ʺx61x6Cx6Cx6Fx77x2Dx65x6Ex74x69x74x79x72x65x66x65x72x65x6Ex63x65x73ʺ_1);
    }
    
}
