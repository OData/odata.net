namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _trackChangesPreferenceParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._trackChangesPreference> Instance { get; } = from _ʺx6Fx64x61x74x61x2Eʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx6Fx64x61x74x61x2EʺParser.Instance.Optional()
from _ʺx74x72x61x63x6Bx2Dx63x68x61x6Ex67x65x73ʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx74x72x61x63x6Bx2Dx63x68x61x6Ex67x65x73ʺParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._trackChangesPreference(_ʺx6Fx64x61x74x61x2Eʺ_1.GetOrElse(null), _ʺx74x72x61x63x6Bx2Dx63x68x61x6Ex67x65x73ʺ_1);
    }
    
}
