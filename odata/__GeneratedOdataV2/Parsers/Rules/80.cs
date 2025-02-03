namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _searchParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._search> Instance { get; } = from _Ⲥʺx24x73x65x61x72x63x68ʺⳆʺx73x65x61x72x63x68ʺↃ_1 in __GeneratedOdataV2.Parsers.Inners._Ⲥʺx24x73x65x61x72x63x68ʺⳆʺx73x65x61x72x63x68ʺↃParser.Instance
from _EQ_1 in __GeneratedOdataV2.Parsers.Rules._EQParser.Instance
from _BWS_1 in __GeneratedOdataV2.Parsers.Rules._BWSParser.Instance
from _searchExpr_1 in __GeneratedOdataV2.Parsers.Rules._searchExprParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._search(_Ⲥʺx24x73x65x61x72x63x68ʺⳆʺx73x65x61x72x63x68ʺↃ_1, _EQ_1, _BWS_1, _searchExpr_1);
    }
    
}
