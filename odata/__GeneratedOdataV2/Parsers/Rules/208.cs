namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _endⲻobjectParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._endⲻobject> Instance { get; } = from _BWS_1 in __GeneratedOdataV2.Parsers.Rules._BWSParser.Instance
from _Ⲥʺx7DʺⳆʺx25x37x44ʺↃ_1 in __GeneratedOdataV2.Parsers.Inners._Ⲥʺx7DʺⳆʺx25x37x44ʺↃParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._endⲻobject(_BWS_1, _Ⲥʺx7DʺⳆʺx25x37x44ʺↃ_1);
    }
    
}
