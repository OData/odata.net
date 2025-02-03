namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _primitiveColInUriParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._primitiveColInUri> Instance { get; } = from _beginⲻarray_1 in __GeneratedOdataV2.Parsers.Rules._beginⲻarrayParser.Instance
from _primitiveLiteralInJSON_ЖⲤvalueⲻseparator_primitiveLiteralInJSONↃ_1 in __GeneratedOdataV2.Parsers.Inners._primitiveLiteralInJSON_ЖⲤvalueⲻseparator_primitiveLiteralInJSONↃParser.Instance.Optional()
from _endⲻarray_1 in __GeneratedOdataV2.Parsers.Rules._endⲻarrayParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._primitiveColInUri(_beginⲻarray_1, _primitiveLiteralInJSON_ЖⲤvalueⲻseparator_primitiveLiteralInJSONↃ_1.GetOrElse(null), _endⲻarray_1);
    }
    
}
