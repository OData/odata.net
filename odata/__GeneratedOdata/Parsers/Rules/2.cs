namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _serviceRootParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._serviceRoot> Instance { get; } = from _Ⲥʺx68x74x74x70x73ʺⳆʺx68x74x74x70ʺↃ_1 in __GeneratedOdata.Parsers.Inners._Ⲥʺx68x74x74x70x73ʺⳆʺx68x74x74x70ʺↃParser.Instance
from _ʺx3Ax2Fx2Fʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx3Ax2Fx2FʺParser.Instance
from _host_1 in __GeneratedOdata.Parsers.Rules._hostParser.Instance
from _ʺx3Aʺ_port_1 in __GeneratedOdata.Parsers.Inners._ʺx3Aʺ_portParser.Instance.Optional()
from _ʺx2Fʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx2FʺParser.Instance
from _Ⲥsegmentⲻnz_ʺx2FʺↃ_1 in __GeneratedOdata.Parsers.Inners._Ⲥsegmentⲻnz_ʺx2FʺↃParser.Instance.Many()
select new __GeneratedOdata.CstNodes.Rules._serviceRoot(_Ⲥʺx68x74x74x70x73ʺⳆʺx68x74x74x70ʺↃ_1, _ʺx3Ax2Fx2Fʺ_1, _host_1, _ʺx3Aʺ_port_1.GetOrElse(null), _ʺx2Fʺ_1, _Ⲥsegmentⲻnz_ʺx2FʺↃ_1);
    }
    
}