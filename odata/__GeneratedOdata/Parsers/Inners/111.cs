namespace __GeneratedOdata.Parsers.Inners
{
    using Sprache;
    
    public static class _SEMI_expandCountOptionParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Inners._SEMI_expandCountOption> Instance { get; } = from _SEMI_1 in __GeneratedOdata.Parsers.Rules._SEMIParser.Instance
from _expandCountOption_1 in __GeneratedOdata.Parsers.Rules._expandCountOptionParser.Instance
select new __GeneratedOdata.CstNodes.Inners._SEMI_expandCountOption(_SEMI_1, _expandCountOption_1);
    }
    
}
