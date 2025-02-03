namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _beginⲻobjectParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._beginⲻobject> Instance { get; } = from _BWS_1 in __GeneratedOdataV2.Parsers.Rules._BWSParser.Instance
from _Ⲥʺx7BʺⳆʺx25x37x42ʺↃ_1 in __GeneratedOdataV2.Parsers.Inners._Ⲥʺx7BʺⳆʺx25x37x42ʺↃParser.Instance
from _BWS_2 in __GeneratedOdataV2.Parsers.Rules._BWSParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._beginⲻobject(_BWS_1, _Ⲥʺx7BʺⳆʺx25x37x42ʺↃ_1, _BWS_2);
    }
    
}
