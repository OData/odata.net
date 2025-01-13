namespace __GeneratedOdata.Parsers.Inners
{
    using Sprache;
    
    public static class _primitiveLiteralInJSON_ЖⲤvalueⲻseparator_primitiveLiteralInJSONↃParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Inners._primitiveLiteralInJSON_ЖⲤvalueⲻseparator_primitiveLiteralInJSONↃ> Instance { get; } = from _primitiveLiteralInJSON_1 in __GeneratedOdata.Parsers.Rules._primitiveLiteralInJSONParser.Instance
from _Ⲥvalueⲻseparator_primitiveLiteralInJSONↃ_1 in __GeneratedOdata.Parsers.Inners._Ⲥvalueⲻseparator_primitiveLiteralInJSONↃParser.Instance.Many()
select new __GeneratedOdata.CstNodes.Inners._primitiveLiteralInJSON_ЖⲤvalueⲻseparator_primitiveLiteralInJSONↃ(_primitiveLiteralInJSON_1, _Ⲥvalueⲻseparator_primitiveLiteralInJSONↃ_1);
    }
    
}
