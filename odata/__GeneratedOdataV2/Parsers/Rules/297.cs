namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _zeroToFiftyNineParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._zeroToFiftyNine> Instance { get; } = from _Ⲥʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺⳆʺx34ʺⳆʺx35ʺↃ_1 in __GeneratedOdataV2.Parsers.Inners._Ⲥʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺⳆʺx34ʺⳆʺx35ʺↃParser.Instance
from _DIGIT_1 in __GeneratedOdataV2.Parsers.Rules._DIGITParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._zeroToFiftyNine(_Ⲥʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺⳆʺx34ʺⳆʺx35ʺↃ_1, _DIGIT_1);
    }
    
}
