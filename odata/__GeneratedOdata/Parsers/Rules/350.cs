namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _isolationParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._isolation> Instance { get; } = from _ʺx4Fx44x61x74x61x2Dʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx4Fx44x61x74x61x2DʺParser.Instance.Optional()
from _ʺx49x73x6Fx6Cx61x74x69x6Fx6Eʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx49x73x6Fx6Cx61x74x69x6Fx6EʺParser.Instance
from _ʺx3Aʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx3AʺParser.Instance
from _OWS_1 in __GeneratedOdata.Parsers.Rules._OWSParser.Instance
from _ʺx73x6Ex61x70x73x68x6Fx74ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx73x6Ex61x70x73x68x6Fx74ʺParser.Instance
select new __GeneratedOdata.CstNodes.Rules._isolation(_ʺx4Fx44x61x74x61x2Dʺ_1.GetOrElse(null), _ʺx49x73x6Fx6Cx61x74x69x6Fx6Eʺ_1, _ʺx3Aʺ_1, _OWS_1, _ʺx73x6Ex61x70x73x68x6Fx74ʺ_1);
    }
    
}