namespace __GeneratedOdata.Parsers.Inners
{
    using Sprache;
    
    public static class _SEMI_expandOptionParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Inners._SEMI_expandOption> Instance { get; } = from _SEMI_1 in __GeneratedOdata.Parsers.Rules._SEMIParser.Instance
from _expandOption_1 in __GeneratedOdata.Parsers.Rules._expandOptionParser.Instance
select new __GeneratedOdata.CstNodes.Inners._SEMI_expandOption(_SEMI_1, _expandOption_1);
    }
    
}
