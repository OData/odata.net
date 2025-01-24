namespace __Generated.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _CRLFParser
    {
        public static IParser<char, __Generated.CstNodes.Rules._CRLF> Instance { get; } = from _CR_1 in __Generated.Parsers.Rules._CRParser.Instance
from _LF_1 in __Generated.Parsers.Rules._LFParser.Instance
select new __Generated.CstNodes.Rules._CRLF(_CR_1, _LF_1);
    }
    
}
