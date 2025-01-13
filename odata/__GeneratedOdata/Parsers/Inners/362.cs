namespace __GeneratedOdata.Parsers.Inners
{
    using Sprache;
    
    public static class _complexInUriⳆcomplexColInUriⳆprimitiveLiteralInJSONⳆprimitiveColInUriParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Inners._complexInUriⳆcomplexColInUriⳆprimitiveLiteralInJSONⳆprimitiveColInUri> Instance { get; } = (_complexInUriParser.Instance).Or<__GeneratedOdata.CstNodes.Inners._complexInUriⳆcomplexColInUriⳆprimitiveLiteralInJSONⳆprimitiveColInUri>(_complexColInUriParser.Instance).Or<__GeneratedOdata.CstNodes.Inners._complexInUriⳆcomplexColInUriⳆprimitiveLiteralInJSONⳆprimitiveColInUri>(_primitiveLiteralInJSONParser.Instance).Or<__GeneratedOdata.CstNodes.Inners._complexInUriⳆcomplexColInUriⳆprimitiveLiteralInJSONⳆprimitiveColInUri>(_primitiveColInUriParser.Instance);
        
        public static class _complexInUriParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Inners._complexInUriⳆcomplexColInUriⳆprimitiveLiteralInJSONⳆprimitiveColInUri._complexInUri> Instance { get; } = from _complexInUri_1 in __GeneratedOdata.Parsers.Rules._complexInUriParser.Instance
select new __GeneratedOdata.CstNodes.Inners._complexInUriⳆcomplexColInUriⳆprimitiveLiteralInJSONⳆprimitiveColInUri._complexInUri(_complexInUri_1);
        }
        
        public static class _complexColInUriParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Inners._complexInUriⳆcomplexColInUriⳆprimitiveLiteralInJSONⳆprimitiveColInUri._complexColInUri> Instance { get; } = from _complexColInUri_1 in __GeneratedOdata.Parsers.Rules._complexColInUriParser.Instance
select new __GeneratedOdata.CstNodes.Inners._complexInUriⳆcomplexColInUriⳆprimitiveLiteralInJSONⳆprimitiveColInUri._complexColInUri(_complexColInUri_1);
        }
        
        public static class _primitiveLiteralInJSONParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Inners._complexInUriⳆcomplexColInUriⳆprimitiveLiteralInJSONⳆprimitiveColInUri._primitiveLiteralInJSON> Instance { get; } = from _primitiveLiteralInJSON_1 in __GeneratedOdata.Parsers.Rules._primitiveLiteralInJSONParser.Instance
select new __GeneratedOdata.CstNodes.Inners._complexInUriⳆcomplexColInUriⳆprimitiveLiteralInJSONⳆprimitiveColInUri._primitiveLiteralInJSON(_primitiveLiteralInJSON_1);
        }
        
        public static class _primitiveColInUriParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Inners._complexInUriⳆcomplexColInUriⳆprimitiveLiteralInJSONⳆprimitiveColInUri._primitiveColInUri> Instance { get; } = from _primitiveColInUri_1 in __GeneratedOdata.Parsers.Rules._primitiveColInUriParser.Instance
select new __GeneratedOdata.CstNodes.Inners._complexInUriⳆcomplexColInUriⳆprimitiveLiteralInJSONⳆprimitiveColInUri._primitiveColInUri(_primitiveColInUri_1);
        }
    }
    
}
