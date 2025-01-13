namespace __GeneratedOdata.Parsers.Inners
{
    using Sprache;
    
    public static class _ʺx5AʺⳆSIGN_hour_ʺx3Aʺ_minuteParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Inners._ʺx5AʺⳆSIGN_hour_ʺx3Aʺ_minute> Instance { get; } = (_ʺx5AʺParser.Instance).Or<__GeneratedOdata.CstNodes.Inners._ʺx5AʺⳆSIGN_hour_ʺx3Aʺ_minute>(_SIGN_hour_ʺx3Aʺ_minuteParser.Instance);
        
        public static class _ʺx5AʺParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Inners._ʺx5AʺⳆSIGN_hour_ʺx3Aʺ_minute._ʺx5Aʺ> Instance { get; } = from _ʺx5Aʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx5AʺParser.Instance
select new __GeneratedOdata.CstNodes.Inners._ʺx5AʺⳆSIGN_hour_ʺx3Aʺ_minute._ʺx5Aʺ(_ʺx5Aʺ_1);
        }
        
        public static class _SIGN_hour_ʺx3Aʺ_minuteParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Inners._ʺx5AʺⳆSIGN_hour_ʺx3Aʺ_minute._SIGN_hour_ʺx3Aʺ_minute> Instance { get; } = from _SIGN_1 in __GeneratedOdata.Parsers.Rules._SIGNParser.Instance
from _hour_1 in __GeneratedOdata.Parsers.Rules._hourParser.Instance
from _ʺx3Aʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx3AʺParser.Instance
from _minute_1 in __GeneratedOdata.Parsers.Rules._minuteParser.Instance
select new __GeneratedOdata.CstNodes.Inners._ʺx5AʺⳆSIGN_hour_ʺx3Aʺ_minute._SIGN_hour_ʺx3Aʺ_minute(_SIGN_1, _hour_1, _ʺx3Aʺ_1, _minute_1);
        }
    }
    
}
