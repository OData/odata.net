namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _idParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._id> Instance { get; } = from _Ⲥʺx24x69x64ʺⳆʺx69x64ʺↃ_1 in __GeneratedOdata.Parsers.Inners._Ⲥʺx24x69x64ʺⳆʺx69x64ʺↃParser.Instance
from _EQ_1 in __GeneratedOdata.Parsers.Rules._EQParser.Instance
from _IRIⲻinⲻquery_1 in __GeneratedOdata.Parsers.Rules._IRIⲻinⲻqueryParser.Instance
select new __GeneratedOdata.CstNodes.Rules._id(_Ⲥʺx24x69x64ʺⳆʺx69x64ʺↃ_1, _EQ_1, _IRIⲻinⲻquery_1);
    }
    
}
