namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _authorityParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._authority> Instance { get; } = from _userinfo_ʺx40ʺ_1 in __GeneratedOdata.Parsers.Inners._userinfo_ʺx40ʺParser.Instance.Optional()
from _host_1 in __GeneratedOdata.Parsers.Rules._hostParser.Instance
from _ʺx3Aʺ_port_1 in __GeneratedOdata.Parsers.Inners._ʺx3Aʺ_portParser.Instance.Optional()
select new __GeneratedOdata.CstNodes.Rules._authority(_userinfo_ʺx40ʺ_1.GetOrElse(null), _host_1, _ʺx3Aʺ_port_1.GetOrElse(null));
    }
    
}
