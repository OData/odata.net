namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _odataⲻversionParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._odataⲻversion> Instance { get; } = from _ʺx4Fx44x61x74x61x2Dx56x65x72x73x69x6Fx6Eʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx4Fx44x61x74x61x2Dx56x65x72x73x69x6Fx6EʺParser.Instance
from _ʺx3Aʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx3AʺParser.Instance
from _OWS_1 in __GeneratedOdata.Parsers.Rules._OWSParser.Instance
from _ʺx34x2Ex30ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx34x2Ex30ʺParser.Instance
from _oneToNine_1 in __GeneratedOdata.Parsers.Rules._oneToNineParser.Instance.Optional()
select new __GeneratedOdata.CstNodes.Rules._odataⲻversion(_ʺx4Fx44x61x74x61x2Dx56x65x72x73x69x6Fx6Eʺ_1, _ʺx3Aʺ_1, _OWS_1, _ʺx34x2Ex30ʺ_1, _oneToNine_1.GetOrElse(null));
    }
    
}