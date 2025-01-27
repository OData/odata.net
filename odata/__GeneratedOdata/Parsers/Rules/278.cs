namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _booleanValueParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._booleanValue> Instance { get; } = (_ʺx74x72x75x65ʺParser.Instance).Or<char, __GeneratedOdata.CstNodes.Rules._booleanValue>(_ʺx66x61x6Cx73x65ʺParser.Instance);
        
        public static class _ʺx74x72x75x65ʺParser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._booleanValue._ʺx74x72x75x65ʺ> Instance { get; } = from _ʺx74x72x75x65ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx74x72x75x65ʺParser.Instance
select new __GeneratedOdata.CstNodes.Rules._booleanValue._ʺx74x72x75x65ʺ(_ʺx74x72x75x65ʺ_1);
        }
        
        public static class _ʺx66x61x6Cx73x65ʺParser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._booleanValue._ʺx66x61x6Cx73x65ʺ> Instance { get; } = from _ʺx66x61x6Cx73x65ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx66x61x6Cx73x65ʺParser.Instance
select new __GeneratedOdata.CstNodes.Rules._booleanValue._ʺx66x61x6Cx73x65ʺ(_ʺx66x61x6Cx73x65ʺ_1);
        }
    }
    
}
