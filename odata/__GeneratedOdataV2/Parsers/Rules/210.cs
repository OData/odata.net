namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _endⲻarrayParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._endⲻarray> Instance { get; } = from _BWS_1 in __GeneratedOdataV2.Parsers.Rules._BWSParser.Instance
from _Ⲥʺx5DʺⳆʺx25x35x44ʺↃ_1 in __GeneratedOdataV2.Parsers.Inners._Ⲥʺx5DʺⳆʺx25x35x44ʺↃParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._endⲻarray(_BWS_1, _Ⲥʺx5DʺⳆʺx25x35x44ʺↃ_1);
    }
    
}
