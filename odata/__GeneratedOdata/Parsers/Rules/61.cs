namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _computeParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._compute> Instance { get; } = from _Ⲥʺx24x63x6Fx6Dx70x75x74x65ʺⳆʺx63x6Fx6Dx70x75x74x65ʺↃ_1 in __GeneratedOdata.Parsers.Inners._Ⲥʺx24x63x6Fx6Dx70x75x74x65ʺⳆʺx63x6Fx6Dx70x75x74x65ʺↃParser.Instance
from _EQ_1 in __GeneratedOdata.Parsers.Rules._EQParser.Instance
from _computeItem_1 in __GeneratedOdata.Parsers.Rules._computeItemParser.Instance
from _ⲤCOMMA_computeItemↃ_1 in Inners._ⲤCOMMA_computeItemↃParser.Instance.Many()
select new __GeneratedOdata.CstNodes.Rules._compute(_Ⲥʺx24x63x6Fx6Dx70x75x74x65ʺⳆʺx63x6Fx6Dx70x75x74x65ʺↃ_1, _EQ_1, _computeItem_1, _ⲤCOMMA_computeItemↃ_1);
    }
    
}
