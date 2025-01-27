namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _entityidParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._entityid> Instance { get; } = from _ʺx4Fx44x61x74x61x2Dʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx4Fx44x61x74x61x2DʺParser.Instance.Optional()
from _ʺx45x6Ex74x69x74x79x49x44ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx45x6Ex74x69x74x79x49x44ʺParser.Instance
from _ʺx3Aʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx3AʺParser.Instance
from _OWS_1 in __GeneratedOdata.Parsers.Rules._OWSParser.Instance
from _IRIⲻinⲻheader_1 in __GeneratedOdata.Parsers.Rules._IRIⲻinⲻheaderParser.Instance
select new __GeneratedOdata.CstNodes.Rules._entityid(_ʺx4Fx44x61x74x61x2Dʺ_1.GetOrElse(null), _ʺx45x6Ex74x69x74x79x49x44ʺ_1, _ʺx3Aʺ_1, _OWS_1, _IRIⲻinⲻheader_1);
    }
    
}
