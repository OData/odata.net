namespace __GeneratedOdata.Parsers.Inners
{
    using Sprache;
    
    public static class _SEMI_selectOptionParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Inners._SEMI_selectOption> Instance { get; } = from _SEMI_1 in __GeneratedOdata.Parsers.Rules._SEMIParser.Instance
from _selectOption_1 in __GeneratedOdata.Parsers.Rules._selectOptionParser.Instance
select new __GeneratedOdata.CstNodes.Inners._SEMI_selectOption(_SEMI_1, _selectOption_1);
    }
    
}
