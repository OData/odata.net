namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _primitiveLiteralInJSONParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteralInJSON> Instance { get; } = (_stringInJSONParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteralInJSON>(_numberInJSONParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteralInJSON>(_ʺx74x72x75x65ʺParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteralInJSON>(_ʺx66x61x6Cx73x65ʺParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteralInJSON>(_ʺx6Ex75x6Cx6CʺParser.Instance);
        
        public static class _stringInJSONParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteralInJSON._stringInJSON> Instance { get; } = from _stringInJSON_1 in __GeneratedOdataV3.Parsers.Rules._stringInJSONParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._primitiveLiteralInJSON._stringInJSON(_stringInJSON_1);
        }
        
        public static class _numberInJSONParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteralInJSON._numberInJSON> Instance { get; } = from _numberInJSON_1 in __GeneratedOdataV3.Parsers.Rules._numberInJSONParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._primitiveLiteralInJSON._numberInJSON(_numberInJSON_1);
        }
        
        public static class _ʺx74x72x75x65ʺParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteralInJSON._ʺx74x72x75x65ʺ> Instance { get; } = from _ʺx74x72x75x65ʺ_1 in __GeneratedOdataV3.Parsers.Inners._ʺx74x72x75x65ʺParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._primitiveLiteralInJSON._ʺx74x72x75x65ʺ(_ʺx74x72x75x65ʺ_1);
        }
        
        public static class _ʺx66x61x6Cx73x65ʺParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteralInJSON._ʺx66x61x6Cx73x65ʺ> Instance { get; } = from _ʺx66x61x6Cx73x65ʺ_1 in __GeneratedOdataV3.Parsers.Inners._ʺx66x61x6Cx73x65ʺParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._primitiveLiteralInJSON._ʺx66x61x6Cx73x65ʺ(_ʺx66x61x6Cx73x65ʺ_1);
        }
        
        public static class _ʺx6Ex75x6Cx6CʺParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteralInJSON._ʺx6Ex75x6Cx6Cʺ> Instance { get; } = from _ʺx6Ex75x6Cx6Cʺ_1 in __GeneratedOdataV3.Parsers.Inners._ʺx6Ex75x6Cx6CʺParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._primitiveLiteralInJSON._ʺx6Ex75x6Cx6Cʺ(_ʺx6Ex75x6Cx6Cʺ_1);
        }
    }
    
}
