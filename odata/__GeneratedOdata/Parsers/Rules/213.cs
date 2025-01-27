namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _valueⲻseparatorParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._valueⲻseparator> Instance { get; } = from _BWS_1 in __GeneratedOdata.Parsers.Rules._BWSParser.Instance
from _COMMA_1 in __GeneratedOdata.Parsers.Rules._COMMAParser.Instance
from _BWS_2 in __GeneratedOdata.Parsers.Rules._BWSParser.Instance
select new __GeneratedOdata.CstNodes.Rules._valueⲻseparator(_BWS_1, _COMMA_1, _BWS_2);
    }
    
}
