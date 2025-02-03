namespace __GeneratedOdataV2.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _primitiveLiteralInJSON_ЖⲤvalueⲻseparator_primitiveLiteralInJSONↃParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._primitiveLiteralInJSON_ЖⲤvalueⲻseparator_primitiveLiteralInJSONↃ> Instance { get; } = from _primitiveLiteralInJSON_1 in __GeneratedOdataV2.Parsers.Rules._primitiveLiteralInJSONParser.Instance
from _Ⲥvalueⲻseparator_primitiveLiteralInJSONↃ_1 in Inners._Ⲥvalueⲻseparator_primitiveLiteralInJSONↃParser.Instance.Many()
select new __GeneratedOdataV2.CstNodes.Inners._primitiveLiteralInJSON_ЖⲤvalueⲻseparator_primitiveLiteralInJSONↃ(_primitiveLiteralInJSON_1, _Ⲥvalueⲻseparator_primitiveLiteralInJSONↃ_1);
    }
    
}
