namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _hourParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._hour> Instance { get; } = (_Ⲥʺx30ʺⳆʺx31ʺↃ_DIGITParser.Instance).Or<char, __GeneratedOdata.CstNodes.Rules._hour>(_ʺx32ʺ_Ⲥʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺↃParser.Instance);
        
        public static class _Ⲥʺx30ʺⳆʺx31ʺↃ_DIGITParser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._hour._Ⲥʺx30ʺⳆʺx31ʺↃ_DIGIT> Instance { get; } = from _Ⲥʺx30ʺⳆʺx31ʺↃ_1 in __GeneratedOdata.Parsers.Inners._Ⲥʺx30ʺⳆʺx31ʺↃParser.Instance
from _DIGIT_1 in __GeneratedOdata.Parsers.Rules._DIGITParser.Instance
select new __GeneratedOdata.CstNodes.Rules._hour._Ⲥʺx30ʺⳆʺx31ʺↃ_DIGIT(_Ⲥʺx30ʺⳆʺx31ʺↃ_1, _DIGIT_1);
        }
        
        public static class _ʺx32ʺ_Ⲥʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺↃParser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._hour._ʺx32ʺ_Ⲥʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺↃ> Instance { get; } = from _ʺx32ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx32ʺParser.Instance
from _Ⲥʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺↃ_1 in __GeneratedOdata.Parsers.Inners._Ⲥʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺↃParser.Instance
select new __GeneratedOdata.CstNodes.Rules._hour._ʺx32ʺ_Ⲥʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺↃ(_ʺx32ʺ_1, _Ⲥʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺↃ_1);
        }
    }
    
}
