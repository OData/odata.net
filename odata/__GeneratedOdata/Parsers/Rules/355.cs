namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _allowEntityReferencesPreferenceParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._allowEntityReferencesPreference> Instance { get; } = from _ʺx6Fx64x61x74x61x2Eʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx6Fx64x61x74x61x2EʺParser.Instance.Optional()
from _ʺx61x6Cx6Cx6Fx77x2Dx65x6Ex74x69x74x79x72x65x66x65x72x65x6Ex63x65x73ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx61x6Cx6Cx6Fx77x2Dx65x6Ex74x69x74x79x72x65x66x65x72x65x6Ex63x65x73ʺParser.Instance
select new __GeneratedOdata.CstNodes.Rules._allowEntityReferencesPreference(_ʺx6Fx64x61x74x61x2Eʺ_1.GetOrElse(null), _ʺx61x6Cx6Cx6Fx77x2Dx65x6Ex74x69x74x79x72x65x66x65x72x65x6Ex63x65x73ʺ_1);
    }
    
}
