namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _beginⲻarrayParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._beginⲻarray> Instance { get; } = from _BWS_1 in __GeneratedOdata.Parsers.Rules._BWSParser.Instance
from _Ⲥʺx5BʺⳆʺx25x35x42ʺↃ_1 in __GeneratedOdata.Parsers.Inners._Ⲥʺx5BʺⳆʺx25x35x42ʺↃParser.Instance
from _BWS_2 in __GeneratedOdata.Parsers.Rules._BWSParser.Instance
select new __GeneratedOdata.CstNodes.Rules._beginⲻarray(_BWS_1, _Ⲥʺx5BʺⳆʺx25x35x42ʺↃ_1, _BWS_2);
    }
    
}
