namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _zeroToFiftyNineParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._zeroToFiftyNine> Instance { get; } = from _Ⲥʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺⳆʺx34ʺⳆʺx35ʺↃ_1 in __GeneratedOdata.Parsers.Inners._Ⲥʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺⳆʺx34ʺⳆʺx35ʺↃParser.Instance
from _DIGIT_1 in __GeneratedOdata.Parsers.Rules._DIGITParser.Instance
select new __GeneratedOdata.CstNodes.Rules._zeroToFiftyNine(_Ⲥʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺⳆʺx34ʺⳆʺx35ʺↃ_1, _DIGIT_1);
    }
    
}
