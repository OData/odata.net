namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _qualifiedTypeNameParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._qualifiedTypeName> Instance { get; } = (_singleQualifiedTypeNameParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._qualifiedTypeName>(_ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Eʺ_OPEN_singleQualifiedTypeName_CLOSEParser.Instance);
        
        public static class _singleQualifiedTypeNameParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._qualifiedTypeName._singleQualifiedTypeName> Instance { get; } = from _singleQualifiedTypeName_1 in __GeneratedOdataV2.Parsers.Rules._singleQualifiedTypeNameParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._qualifiedTypeName._singleQualifiedTypeName(_singleQualifiedTypeName_1);
        }
        
        public static class _ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Eʺ_OPEN_singleQualifiedTypeName_CLOSEParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._qualifiedTypeName._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Eʺ_OPEN_singleQualifiedTypeName_CLOSE> Instance { get; } = from _ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Eʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6EʺParser.Instance
from _OPEN_1 in __GeneratedOdataV2.Parsers.Rules._OPENParser.Instance
from _singleQualifiedTypeName_1 in __GeneratedOdataV2.Parsers.Rules._singleQualifiedTypeNameParser.Instance
from _CLOSE_1 in __GeneratedOdataV2.Parsers.Rules._CLOSEParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._qualifiedTypeName._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Eʺ_OPEN_singleQualifiedTypeName_CLOSE(_ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Eʺ_1, _OPEN_1, _singleQualifiedTypeName_1, _CLOSE_1);
        }
    }
    
}
