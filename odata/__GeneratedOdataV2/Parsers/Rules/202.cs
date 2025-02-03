namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _primitivePropertyInUriParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._primitivePropertyInUri> Instance { get; } = from _quotationⲻmark_1 in __GeneratedOdataV2.Parsers.Rules._quotationⲻmarkParser.Instance
from _primitiveProperty_1 in __GeneratedOdataV2.Parsers.Rules._primitivePropertyParser.Instance
from _quotationⲻmark_2 in __GeneratedOdataV2.Parsers.Rules._quotationⲻmarkParser.Instance
from _nameⲻseparator_1 in __GeneratedOdataV2.Parsers.Rules._nameⲻseparatorParser.Instance
from _primitiveLiteralInJSON_1 in __GeneratedOdataV2.Parsers.Rules._primitiveLiteralInJSONParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._primitivePropertyInUri(_quotationⲻmark_1, _primitiveProperty_1, _quotationⲻmark_2, _nameⲻseparator_1, _primitiveLiteralInJSON_1);
    }
    
}
