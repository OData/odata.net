namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _abstractSpatialTypeNameParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._abstractSpatialTypeName> Instance { get; } = (_ʺx47x65x6Fx67x72x61x70x68x79ʺParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._abstractSpatialTypeName>(_ʺx47x65x6Fx6Dx65x74x72x79ʺParser.Instance);
        
        public static class _ʺx47x65x6Fx67x72x61x70x68x79ʺParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._abstractSpatialTypeName._ʺx47x65x6Fx67x72x61x70x68x79ʺ> Instance { get; } = from _ʺx47x65x6Fx67x72x61x70x68x79ʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx47x65x6Fx67x72x61x70x68x79ʺParser.Instance
select __GeneratedOdataV2.CstNodes.Rules._abstractSpatialTypeName._ʺx47x65x6Fx67x72x61x70x68x79ʺ.Instance;
        }
        
        public static class _ʺx47x65x6Fx6Dx65x74x72x79ʺParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._abstractSpatialTypeName._ʺx47x65x6Fx6Dx65x74x72x79ʺ> Instance { get; } = from _ʺx47x65x6Fx6Dx65x74x72x79ʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx47x65x6Fx6Dx65x74x72x79ʺParser.Instance
select __GeneratedOdataV2.CstNodes.Rules._abstractSpatialTypeName._ʺx47x65x6Fx6Dx65x74x72x79ʺ.Instance;
        }
    }
    
}
