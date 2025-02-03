namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _SQUOTEⲻinⲻstringParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._SQUOTEⲻinⲻstring> Instance { get; } = from _SQUOTE_1 in __GeneratedOdataV2.Parsers.Rules._SQUOTEParser.Instance
from _SQUOTE_2 in __GeneratedOdataV2.Parsers.Rules._SQUOTEParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._SQUOTEⲻinⲻstring(_SQUOTE_1, _SQUOTE_2);
    }
    
}
