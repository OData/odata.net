namespace __GeneratedOdata.Parsers.Inners
{
    using Sprache;
    
    public static class _SEMI_selectOptionPCParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Inners._SEMI_selectOptionPC> Instance { get; } = from _SEMI_1 in __GeneratedOdata.Parsers.Rules._SEMIParser.Instance
from _selectOptionPC_1 in __GeneratedOdata.Parsers.Rules._selectOptionPCParser.Instance
select new __GeneratedOdata.CstNodes.Inners._SEMI_selectOptionPC(_SEMI_1, _selectOptionPC_1);
    }
    
}
