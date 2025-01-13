namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _primitiveColInUriParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._primitiveColInUri> Instance { get; } = from _beginⲻarray_1 in __GeneratedOdata.Parsers.Rules._beginⲻarrayParser.Instance
from _primitiveLiteralInJSON_ЖⲤvalueⲻseparator_primitiveLiteralInJSONↃ_1 in __GeneratedOdata.Parsers.Inners._primitiveLiteralInJSON_ЖⲤvalueⲻseparator_primitiveLiteralInJSONↃParser.Instance.Optional()
from _endⲻarray_1 in __GeneratedOdata.Parsers.Rules._endⲻarrayParser.Instance
select new __GeneratedOdata.CstNodes.Rules._primitiveColInUri(_beginⲻarray_1, _primitiveLiteralInJSON_ЖⲤvalueⲻseparator_primitiveLiteralInJSONↃ_1.GetOrElse(null), _endⲻarray_1);
    }
    
}
