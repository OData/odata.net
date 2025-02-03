namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _idParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._id> Instance { get; } = from _Ⲥʺx24x69x64ʺⳆʺx69x64ʺↃ_1 in __GeneratedOdataV2.Parsers.Inners._Ⲥʺx24x69x64ʺⳆʺx69x64ʺↃParser.Instance
from _EQ_1 in __GeneratedOdataV2.Parsers.Rules._EQParser.Instance
from _IRIⲻinⲻquery_1 in __GeneratedOdataV2.Parsers.Rules._IRIⲻinⲻqueryParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._id(_Ⲥʺx24x69x64ʺⳆʺx69x64ʺↃ_1, _EQ_1, _IRIⲻinⲻquery_1);
    }
    
}
