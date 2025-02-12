namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _expandItemParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._expandItem> Instance { get; } = (_STAR_꘡refⳆOPEN_levels_CLOSE꘡Parser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._expandItem>(_ʺx24x76x61x6Cx75x65ʺParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._expandItem>(_expandPath_꘡ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡Ⳇcount_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡ⳆOPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSE꘡Parser.Instance);
        
        public static class _STAR_꘡refⳆOPEN_levels_CLOSE꘡Parser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._expandItem._STAR_꘡refⳆOPEN_levels_CLOSE꘡> Instance { get; } = from _STAR_1 in __GeneratedOdataV2.Parsers.Rules._STARParser.Instance
from _refⳆOPEN_levels_CLOSE_1 in __GeneratedOdataV2.Parsers.Inners._refⳆOPEN_levels_CLOSEParser.Instance.Optional()
select new __GeneratedOdataV2.CstNodes.Rules._expandItem._STAR_꘡refⳆOPEN_levels_CLOSE꘡(_STAR_1, _refⳆOPEN_levels_CLOSE_1.GetOrElse(null));
        }
        
        public static class _ʺx24x76x61x6Cx75x65ʺParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._expandItem._ʺx24x76x61x6Cx75x65ʺ> Instance { get; } = from _ʺx24x76x61x6Cx75x65ʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx24x76x61x6Cx75x65ʺParser.Instance
select __GeneratedOdataV2.CstNodes.Rules._expandItem._ʺx24x76x61x6Cx75x65ʺ.Instance;
        }
        
        public static class _expandPath_꘡ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡Ⳇcount_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡ⳆOPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSE꘡Parser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._expandItem._expandPath_꘡ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡Ⳇcount_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡ⳆOPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSE꘡> Instance { get; } = from _expandPath_1 in __GeneratedOdataV2.Parsers.Rules._expandPathParser.Instance
from _ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡Ⳇcount_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡ⳆOPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSE_1 in __GeneratedOdataV2.Parsers.Inners._ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡Ⳇcount_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡ⳆOPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSEParser.Instance.Optional()
select new __GeneratedOdataV2.CstNodes.Rules._expandItem._expandPath_꘡ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡Ⳇcount_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡ⳆOPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSE꘡(_expandPath_1, _ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡Ⳇcount_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡ⳆOPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSE_1.GetOrElse(null));
        }
    }
    
}
