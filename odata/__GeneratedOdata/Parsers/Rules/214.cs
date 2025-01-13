namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _primitiveLiteralInJSONParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._primitiveLiteralInJSON> Instance { get; } = (_stringInJSONParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._primitiveLiteralInJSON>(_numberInJSONParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._primitiveLiteralInJSON>(_ʺx74x72x75x65ʺParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._primitiveLiteralInJSON>(_ʺx66x61x6Cx73x65ʺParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._primitiveLiteralInJSON>(_ʺx6Ex75x6Cx6CʺParser.Instance);
        
        public static class _stringInJSONParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._primitiveLiteralInJSON._stringInJSON> Instance { get; } = from _stringInJSON_1 in __GeneratedOdata.Parsers.Rules._stringInJSONParser.Instance
select new __GeneratedOdata.CstNodes.Rules._primitiveLiteralInJSON._stringInJSON(_stringInJSON_1);
        }
        
        public static class _numberInJSONParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._primitiveLiteralInJSON._numberInJSON> Instance { get; } = from _numberInJSON_1 in __GeneratedOdata.Parsers.Rules._numberInJSONParser.Instance
select new __GeneratedOdata.CstNodes.Rules._primitiveLiteralInJSON._numberInJSON(_numberInJSON_1);
        }
        
        public static class _ʺx74x72x75x65ʺParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._primitiveLiteralInJSON._ʺx74x72x75x65ʺ> Instance { get; } = from _ʺx74x72x75x65ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx74x72x75x65ʺParser.Instance
select new __GeneratedOdata.CstNodes.Rules._primitiveLiteralInJSON._ʺx74x72x75x65ʺ(_ʺx74x72x75x65ʺ_1);
        }
        
        public static class _ʺx66x61x6Cx73x65ʺParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._primitiveLiteralInJSON._ʺx66x61x6Cx73x65ʺ> Instance { get; } = from _ʺx66x61x6Cx73x65ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx66x61x6Cx73x65ʺParser.Instance
select new __GeneratedOdata.CstNodes.Rules._primitiveLiteralInJSON._ʺx66x61x6Cx73x65ʺ(_ʺx66x61x6Cx73x65ʺ_1);
        }
        
        public static class _ʺx6Ex75x6Cx6CʺParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._primitiveLiteralInJSON._ʺx6Ex75x6Cx6Cʺ> Instance { get; } = from _ʺx6Ex75x6Cx6Cʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx6Ex75x6Cx6CʺParser.Instance
select new __GeneratedOdata.CstNodes.Rules._primitiveLiteralInJSON._ʺx6Ex75x6Cx6Cʺ(_ʺx6Ex75x6Cx6Cʺ_1);
        }
    }
    
}
