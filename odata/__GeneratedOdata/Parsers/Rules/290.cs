namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _SQUOTEⲻinⲻstringParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._SQUOTEⲻinⲻstring> Instance { get; } = from _SQUOTE_1 in __GeneratedOdata.Parsers.Rules._SQUOTEParser.Instance
from _SQUOTE_2 in __GeneratedOdata.Parsers.Rules._SQUOTEParser.Instance
select new __GeneratedOdata.CstNodes.Rules._SQUOTEⲻinⲻstring(_SQUOTE_1, _SQUOTE_2);
    }
    
}
