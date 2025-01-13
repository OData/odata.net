namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _selectParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._select> Instance { get; } = from _Ⲥʺx24x73x65x6Cx65x63x74ʺⳆʺx73x65x6Cx65x63x74ʺↃ_1 in __GeneratedOdata.Parsers.Inners._Ⲥʺx24x73x65x6Cx65x63x74ʺⳆʺx73x65x6Cx65x63x74ʺↃParser.Instance
from _EQ_1 in __GeneratedOdata.Parsers.Rules._EQParser.Instance
from _selectItem_1 in __GeneratedOdata.Parsers.Rules._selectItemParser.Instance
from _ⲤCOMMA_selectItemↃ_1 in __GeneratedOdata.Parsers.Inners._ⲤCOMMA_selectItemↃParser.Instance.Many()
select new __GeneratedOdata.CstNodes.Rules._select(_Ⲥʺx24x73x65x6Cx65x63x74ʺⳆʺx73x65x6Cx65x63x74ʺↃ_1, _EQ_1, _selectItem_1, _ⲤCOMMA_selectItemↃ_1);
    }
    
}
