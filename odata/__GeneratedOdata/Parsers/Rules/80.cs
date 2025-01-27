namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _searchParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._search> Instance { get; } = from _Ⲥʺx24x73x65x61x72x63x68ʺⳆʺx73x65x61x72x63x68ʺↃ_1 in __GeneratedOdata.Parsers.Inners._Ⲥʺx24x73x65x61x72x63x68ʺⳆʺx73x65x61x72x63x68ʺↃParser.Instance
from _EQ_1 in __GeneratedOdata.Parsers.Rules._EQParser.Instance
from _BWS_1 in __GeneratedOdata.Parsers.Rules._BWSParser.Instance
from _searchExpr_1 in __GeneratedOdata.Parsers.Rules._searchExprParser.Instance
select new __GeneratedOdata.CstNodes.Rules._search(_Ⲥʺx24x73x65x61x72x63x68ʺⳆʺx73x65x61x72x63x68ʺↃ_1, _EQ_1, _BWS_1, _searchExpr_1);
    }
    
}
