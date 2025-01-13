namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _expandItemParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._expandItem> Instance { get; } = (_STAR_꘡refⳆOPEN_levels_CLOSE꘡Parser.Instance).Or<__GeneratedOdata.CstNodes.Rules._expandItem>(_ʺx24x76x61x6Cx75x65ʺParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._expandItem>(_expandPath_꘡ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡Ⳇcount_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡ⳆOPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSE꘡Parser.Instance);
        
        public static class _STAR_꘡refⳆOPEN_levels_CLOSE꘡Parser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._expandItem._STAR_꘡refⳆOPEN_levels_CLOSE꘡> Instance { get; } = from _STAR_1 in __GeneratedOdata.Parsers.Rules._STARParser.Instance
from _refⳆOPEN_levels_CLOSE_1 in __GeneratedOdata.Parsers.Inners._refⳆOPEN_levels_CLOSEParser.Instance.Optional()
select new __GeneratedOdata.CstNodes.Rules._expandItem._STAR_꘡refⳆOPEN_levels_CLOSE꘡(_STAR_1, _refⳆOPEN_levels_CLOSE_1.GetOrElse(null));
        }
        
        public static class _ʺx24x76x61x6Cx75x65ʺParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._expandItem._ʺx24x76x61x6Cx75x65ʺ> Instance { get; } = from _ʺx24x76x61x6Cx75x65ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx24x76x61x6Cx75x65ʺParser.Instance
select new __GeneratedOdata.CstNodes.Rules._expandItem._ʺx24x76x61x6Cx75x65ʺ(_ʺx24x76x61x6Cx75x65ʺ_1);
        }
        
        public static class _expandPath_꘡ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡Ⳇcount_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡ⳆOPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSE꘡Parser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._expandItem._expandPath_꘡ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡Ⳇcount_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡ⳆOPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSE꘡> Instance { get; } = from _expandPath_1 in __GeneratedOdata.Parsers.Rules._expandPathParser.Instance
from _ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡Ⳇcount_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡ⳆOPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSE_1 in __GeneratedOdata.Parsers.Inners._ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡Ⳇcount_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡ⳆOPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSEParser.Instance.Optional()
select new __GeneratedOdata.CstNodes.Rules._expandItem._expandPath_꘡ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡Ⳇcount_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡ⳆOPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSE꘡(_expandPath_1, _ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡Ⳇcount_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡ⳆOPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSE_1.GetOrElse(null));
        }
    }
    
}
