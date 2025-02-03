namespace __GeneratedOdataV2.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _complexInUriⳆcomplexColInUriⳆprimitiveLiteralInJSONⳆprimitiveColInUriParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._complexInUriⳆcomplexColInUriⳆprimitiveLiteralInJSONⳆprimitiveColInUri> Instance { get; } = (_complexInUriParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Inners._complexInUriⳆcomplexColInUriⳆprimitiveLiteralInJSONⳆprimitiveColInUri>(_complexColInUriParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Inners._complexInUriⳆcomplexColInUriⳆprimitiveLiteralInJSONⳆprimitiveColInUri>(_primitiveLiteralInJSONParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Inners._complexInUriⳆcomplexColInUriⳆprimitiveLiteralInJSONⳆprimitiveColInUri>(_primitiveColInUriParser.Instance);
        
        public static class _complexInUriParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._complexInUriⳆcomplexColInUriⳆprimitiveLiteralInJSONⳆprimitiveColInUri._complexInUri> Instance { get; } = from _complexInUri_1 in __GeneratedOdataV2.Parsers.Rules._complexInUriParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._complexInUriⳆcomplexColInUriⳆprimitiveLiteralInJSONⳆprimitiveColInUri._complexInUri(_complexInUri_1);
        }
        
        public static class _complexColInUriParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._complexInUriⳆcomplexColInUriⳆprimitiveLiteralInJSONⳆprimitiveColInUri._complexColInUri> Instance { get; } = from _complexColInUri_1 in __GeneratedOdataV2.Parsers.Rules._complexColInUriParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._complexInUriⳆcomplexColInUriⳆprimitiveLiteralInJSONⳆprimitiveColInUri._complexColInUri(_complexColInUri_1);
        }
        
        public static class _primitiveLiteralInJSONParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._complexInUriⳆcomplexColInUriⳆprimitiveLiteralInJSONⳆprimitiveColInUri._primitiveLiteralInJSON> Instance { get; } = from _primitiveLiteralInJSON_1 in __GeneratedOdataV2.Parsers.Rules._primitiveLiteralInJSONParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._complexInUriⳆcomplexColInUriⳆprimitiveLiteralInJSONⳆprimitiveColInUri._primitiveLiteralInJSON(_primitiveLiteralInJSON_1);
        }
        
        public static class _primitiveColInUriParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._complexInUriⳆcomplexColInUriⳆprimitiveLiteralInJSONⳆprimitiveColInUri._primitiveColInUri> Instance { get; } = from _primitiveColInUri_1 in __GeneratedOdataV2.Parsers.Rules._primitiveColInUriParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._complexInUriⳆcomplexColInUriⳆprimitiveLiteralInJSONⳆprimitiveColInUri._primitiveColInUri(_primitiveColInUri_1);
        }
    }
    
}
