namespace __GeneratedOdata.Parsers.Inners
{
    using Sprache;
    
    public static class _COMMA_singleEnumValueParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Inners._COMMA_singleEnumValue> Instance { get; } = from _COMMA_1 in __GeneratedOdata.Parsers.Rules._COMMAParser.Instance
from _singleEnumValue_1 in __GeneratedOdata.Parsers.Rules._singleEnumValueParser.Instance
select new __GeneratedOdata.CstNodes.Inners._COMMA_singleEnumValue(_COMMA_1, _singleEnumValue_1);
    }
    
}
