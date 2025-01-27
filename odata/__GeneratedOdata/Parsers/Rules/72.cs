namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _orderbyParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._orderby> Instance { get; } = from _Ⲥʺx24x6Fx72x64x65x72x62x79ʺⳆʺx6Fx72x64x65x72x62x79ʺↃ_1 in __GeneratedOdata.Parsers.Inners._Ⲥʺx24x6Fx72x64x65x72x62x79ʺⳆʺx6Fx72x64x65x72x62x79ʺↃParser.Instance
from _EQ_1 in __GeneratedOdata.Parsers.Rules._EQParser.Instance
from _orderbyItem_1 in __GeneratedOdata.Parsers.Rules._orderbyItemParser.Instance
from _ⲤCOMMA_orderbyItemↃ_1 in Inners._ⲤCOMMA_orderbyItemↃParser.Instance.Many()
select new __GeneratedOdata.CstNodes.Rules._orderby(_Ⲥʺx24x6Fx72x64x65x72x62x79ʺⳆʺx6Fx72x64x65x72x62x79ʺↃ_1, _EQ_1, _orderbyItem_1, _ⲤCOMMA_orderbyItemↃ_1);
    }
    
}
