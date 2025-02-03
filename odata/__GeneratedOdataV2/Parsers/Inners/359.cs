namespace __GeneratedOdataV2.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _valueⲻseparator_primitiveLiteralInJSONParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._valueⲻseparator_primitiveLiteralInJSON> Instance { get; } = from _valueⲻseparator_1 in __GeneratedOdataV2.Parsers.Rules._valueⲻseparatorParser.Instance
from _primitiveLiteralInJSON_1 in __GeneratedOdataV2.Parsers.Rules._primitiveLiteralInJSONParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._valueⲻseparator_primitiveLiteralInJSON(_valueⲻseparator_1, _primitiveLiteralInJSON_1);
    }
    
}
