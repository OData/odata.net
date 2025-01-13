namespace __GeneratedOdata.Parsers.Inners
{
    using Sprache;
    
    public static class _STARⳆnamespace_ʺx2Eʺ_ⲤtermNameⳆSTARↃParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Inners._STARⳆnamespace_ʺx2Eʺ_ⲤtermNameⳆSTARↃ> Instance { get; } = (_STARParser.Instance).Or<__GeneratedOdata.CstNodes.Inners._STARⳆnamespace_ʺx2Eʺ_ⲤtermNameⳆSTARↃ>(_namespace_ʺx2Eʺ_ⲤtermNameⳆSTARↃParser.Instance);
        
        public static class _STARParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Inners._STARⳆnamespace_ʺx2Eʺ_ⲤtermNameⳆSTARↃ._STAR> Instance { get; } = from _STAR_1 in __GeneratedOdata.Parsers.Rules._STARParser.Instance
select new __GeneratedOdata.CstNodes.Inners._STARⳆnamespace_ʺx2Eʺ_ⲤtermNameⳆSTARↃ._STAR(_STAR_1);
        }
        
        public static class _namespace_ʺx2Eʺ_ⲤtermNameⳆSTARↃParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Inners._STARⳆnamespace_ʺx2Eʺ_ⲤtermNameⳆSTARↃ._namespace_ʺx2Eʺ_ⲤtermNameⳆSTARↃ> Instance { get; } = from _namespace_1 in __GeneratedOdata.Parsers.Rules._namespaceParser.Instance
from _ʺx2Eʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx2EʺParser.Instance
from _ⲤtermNameⳆSTARↃ_1 in __GeneratedOdata.Parsers.Inners._ⲤtermNameⳆSTARↃParser.Instance
select new __GeneratedOdata.CstNodes.Inners._STARⳆnamespace_ʺx2Eʺ_ⲤtermNameⳆSTARↃ._namespace_ʺx2Eʺ_ⲤtermNameⳆSTARↃ(_namespace_1, _ʺx2Eʺ_1, _ⲤtermNameⳆSTARↃ_1);
        }
    }
    
}
