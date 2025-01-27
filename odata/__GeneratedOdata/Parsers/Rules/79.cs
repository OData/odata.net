namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _schemaversionParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._schemaversion> Instance { get; } = from _Ⲥʺx24x73x63x68x65x6Dx61x76x65x72x73x69x6Fx6EʺⳆʺx73x63x68x65x6Dx61x76x65x72x73x69x6Fx6EʺↃ_1 in __GeneratedOdata.Parsers.Inners._Ⲥʺx24x73x63x68x65x6Dx61x76x65x72x73x69x6Fx6EʺⳆʺx73x63x68x65x6Dx61x76x65x72x73x69x6Fx6EʺↃParser.Instance
from _EQ_1 in __GeneratedOdata.Parsers.Rules._EQParser.Instance
from _ⲤSTARⳆ1ЖunreservedↃ_1 in __GeneratedOdata.Parsers.Inners._ⲤSTARⳆ1ЖunreservedↃParser.Instance
select new __GeneratedOdata.CstNodes.Rules._schemaversion(_Ⲥʺx24x73x63x68x65x6Dx61x76x65x72x73x69x6Fx6EʺⳆʺx73x63x68x65x6Dx61x76x65x72x73x69x6Fx6EʺↃ_1, _EQ_1, _ⲤSTARⳆ1ЖunreservedↃ_1);
    }
    
}
