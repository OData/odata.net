namespace __GeneratedOdata.Parsers.Inners
{
    using Sprache;
    
    public static class _valueⲻseparator_primitiveLiteralInJSONParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Inners._valueⲻseparator_primitiveLiteralInJSON> Instance { get; } = from _valueⲻseparator_1 in __GeneratedOdata.Parsers.Rules._valueⲻseparatorParser.Instance
from _primitiveLiteralInJSON_1 in __GeneratedOdata.Parsers.Rules._primitiveLiteralInJSONParser.Instance
select new __GeneratedOdata.CstNodes.Inners._valueⲻseparator_primitiveLiteralInJSON(_valueⲻseparator_1, _primitiveLiteralInJSON_1);
    }
    
}