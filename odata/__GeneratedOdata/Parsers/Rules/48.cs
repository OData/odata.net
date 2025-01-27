namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _crossjoinParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._crossjoin> Instance { get; } = from _ʺx24x63x72x6Fx73x73x6Ax6Fx69x6Eʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx24x63x72x6Fx73x73x6Ax6Fx69x6EʺParser.Instance
from _OPEN_1 in __GeneratedOdata.Parsers.Rules._OPENParser.Instance
from _entitySetName_1 in __GeneratedOdata.Parsers.Rules._entitySetNameParser.Instance
from _ⲤCOMMA_entitySetNameↃ_1 in Inners._ⲤCOMMA_entitySetNameↃParser.Instance.Many()
from _CLOSE_1 in __GeneratedOdata.Parsers.Rules._CLOSEParser.Instance
select new __GeneratedOdata.CstNodes.Rules._crossjoin(_ʺx24x63x72x6Fx73x73x6Ax6Fx69x6Eʺ_1, _OPEN_1, _entitySetName_1, _ⲤCOMMA_entitySetNameↃ_1, _CLOSE_1);
    }
    
}
