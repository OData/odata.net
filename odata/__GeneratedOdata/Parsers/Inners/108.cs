namespace __GeneratedOdata.Parsers.Inners
{
    using Sprache;
    
    public static class _SEMI_expandRefOptionParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Inners._SEMI_expandRefOption> Instance { get; } = from _SEMI_1 in __GeneratedOdata.Parsers.Rules._SEMIParser.Instance
from _expandRefOption_1 in __GeneratedOdata.Parsers.Rules._expandRefOptionParser.Instance
select new __GeneratedOdata.CstNodes.Inners._SEMI_expandRefOption(_SEMI_1, _expandRefOption_1);
    }
    
}
