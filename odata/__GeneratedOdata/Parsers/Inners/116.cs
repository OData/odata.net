namespace __GeneratedOdata.Parsers.Inners
{
    using Sprache;
    
    public static class _ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡Ⳇcount_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡ⳆOPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSEParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Inners._ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡Ⳇcount_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡ⳆOPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSE> Instance { get; } = (_ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡Parser.Instance).Or<__GeneratedOdata.CstNodes.Inners._ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡Ⳇcount_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡ⳆOPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSE>(_count_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡Parser.Instance).Or<__GeneratedOdata.CstNodes.Inners._ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡Ⳇcount_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡ⳆOPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSE>(_OPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSEParser.Instance);
        
        public static class _ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡Parser
        {
            public static Parser<__GeneratedOdata.CstNodes.Inners._ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡Ⳇcount_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡ⳆOPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSE._ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡> Instance { get; } = from _ref_1 in __GeneratedOdata.Parsers.Rules._refParser.Instance
from _OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE_1 in __GeneratedOdata.Parsers.Inners._OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSEParser.Instance.Optional()
select new __GeneratedOdata.CstNodes.Inners._ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡Ⳇcount_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡ⳆOPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSE._ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡(_ref_1, _OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE_1.GetOrElse(null));
        }
        
        public static class _count_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡Parser
        {
            public static Parser<__GeneratedOdata.CstNodes.Inners._ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡Ⳇcount_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡ⳆOPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSE._count_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡> Instance { get; } = from _count_1 in __GeneratedOdata.Parsers.Rules._countParser.Instance
from _OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE_1 in __GeneratedOdata.Parsers.Inners._OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSEParser.Instance.Optional()
select new __GeneratedOdata.CstNodes.Inners._ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡Ⳇcount_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡ⳆOPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSE._count_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡(_count_1, _OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE_1.GetOrElse(null));
        }
        
        public static class _OPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSEParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Inners._ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡Ⳇcount_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡ⳆOPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSE._OPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSE> Instance { get; } = from _OPEN_1 in __GeneratedOdata.Parsers.Rules._OPENParser.Instance
from _expandOption_1 in __GeneratedOdata.Parsers.Rules._expandOptionParser.Instance
from _ⲤSEMI_expandOptionↃ_1 in __GeneratedOdata.Parsers.Inners._ⲤSEMI_expandOptionↃParser.Instance.Many()
from _CLOSE_1 in __GeneratedOdata.Parsers.Rules._CLOSEParser.Instance
select new __GeneratedOdata.CstNodes.Inners._ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡Ⳇcount_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡ⳆOPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSE._OPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSE(_OPEN_1, _expandOption_1, _ⲤSEMI_expandOptionↃ_1, _CLOSE_1);
        }
    }
    
}