namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _odataRelativeUriParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._odataRelativeUri> Instance { get; } = (_ʺx24x62x61x74x63x68ʺ_꘡ʺx3Fʺ_batchOptions꘡Parser.Instance).Or<char, __GeneratedOdata.CstNodes.Rules._odataRelativeUri>(_ʺx24x65x6Ex74x69x74x79ʺ_ʺx3Fʺ_entityOptionsParser.Instance).Or<char, __GeneratedOdata.CstNodes.Rules._odataRelativeUri>(_ʺx24x65x6Ex74x69x74x79ʺ_ʺx2Fʺ_qualifiedEntityTypeName_ʺx3Fʺ_entityCastOptionsParser.Instance).Or<char, __GeneratedOdata.CstNodes.Rules._odataRelativeUri>(_ʺx24x6Dx65x74x61x64x61x74x61ʺ_꘡ʺx3Fʺ_metadataOptions꘡_꘡context꘡Parser.Instance).Or<char, __GeneratedOdata.CstNodes.Rules._odataRelativeUri>(_resourcePath_꘡ʺx3Fʺ_queryOptions꘡Parser.Instance);
        
        public static class _ʺx24x62x61x74x63x68ʺ_꘡ʺx3Fʺ_batchOptions꘡Parser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._odataRelativeUri._ʺx24x62x61x74x63x68ʺ_꘡ʺx3Fʺ_batchOptions꘡> Instance { get; } = from _ʺx24x62x61x74x63x68ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx24x62x61x74x63x68ʺParser.Instance
from _ʺx3Fʺ_batchOptions_1 in __GeneratedOdata.Parsers.Inners._ʺx3Fʺ_batchOptionsParser.Instance.Optional()
select new __GeneratedOdata.CstNodes.Rules._odataRelativeUri._ʺx24x62x61x74x63x68ʺ_꘡ʺx3Fʺ_batchOptions꘡(_ʺx24x62x61x74x63x68ʺ_1, _ʺx3Fʺ_batchOptions_1.GetOrElse(null));
        }
        
        public static class _ʺx24x65x6Ex74x69x74x79ʺ_ʺx3Fʺ_entityOptionsParser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._odataRelativeUri._ʺx24x65x6Ex74x69x74x79ʺ_ʺx3Fʺ_entityOptions> Instance { get; } = from _ʺx24x65x6Ex74x69x74x79ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx24x65x6Ex74x69x74x79ʺParser.Instance
from _ʺx3Fʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx3FʺParser.Instance
from _entityOptions_1 in __GeneratedOdata.Parsers.Rules._entityOptionsParser.Instance
select new __GeneratedOdata.CstNodes.Rules._odataRelativeUri._ʺx24x65x6Ex74x69x74x79ʺ_ʺx3Fʺ_entityOptions(_ʺx24x65x6Ex74x69x74x79ʺ_1, _ʺx3Fʺ_1, _entityOptions_1);
        }
        
        public static class _ʺx24x65x6Ex74x69x74x79ʺ_ʺx2Fʺ_qualifiedEntityTypeName_ʺx3Fʺ_entityCastOptionsParser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._odataRelativeUri._ʺx24x65x6Ex74x69x74x79ʺ_ʺx2Fʺ_qualifiedEntityTypeName_ʺx3Fʺ_entityCastOptions> Instance { get; } = from _ʺx24x65x6Ex74x69x74x79ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx24x65x6Ex74x69x74x79ʺParser.Instance
from _ʺx2Fʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx2FʺParser.Instance
from _qualifiedEntityTypeName_1 in __GeneratedOdata.Parsers.Rules._qualifiedEntityTypeNameParser.Instance
from _ʺx3Fʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx3FʺParser.Instance
from _entityCastOptions_1 in __GeneratedOdata.Parsers.Rules._entityCastOptionsParser.Instance
select new __GeneratedOdata.CstNodes.Rules._odataRelativeUri._ʺx24x65x6Ex74x69x74x79ʺ_ʺx2Fʺ_qualifiedEntityTypeName_ʺx3Fʺ_entityCastOptions(_ʺx24x65x6Ex74x69x74x79ʺ_1, _ʺx2Fʺ_1, _qualifiedEntityTypeName_1, _ʺx3Fʺ_1, _entityCastOptions_1);
        }
        
        public static class _ʺx24x6Dx65x74x61x64x61x74x61ʺ_꘡ʺx3Fʺ_metadataOptions꘡_꘡context꘡Parser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._odataRelativeUri._ʺx24x6Dx65x74x61x64x61x74x61ʺ_꘡ʺx3Fʺ_metadataOptions꘡_꘡context꘡> Instance { get; } = from _ʺx24x6Dx65x74x61x64x61x74x61ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx24x6Dx65x74x61x64x61x74x61ʺParser.Instance
from _ʺx3Fʺ_metadataOptions_1 in __GeneratedOdata.Parsers.Inners._ʺx3Fʺ_metadataOptionsParser.Instance.Optional()
from _context_1 in __GeneratedOdata.Parsers.Rules._contextParser.Instance.Optional()
select new __GeneratedOdata.CstNodes.Rules._odataRelativeUri._ʺx24x6Dx65x74x61x64x61x74x61ʺ_꘡ʺx3Fʺ_metadataOptions꘡_꘡context꘡(_ʺx24x6Dx65x74x61x64x61x74x61ʺ_1, _ʺx3Fʺ_metadataOptions_1.GetOrElse(null), _context_1.GetOrElse(null));
        }
        
        public static class _resourcePath_꘡ʺx3Fʺ_queryOptions꘡Parser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._odataRelativeUri._resourcePath_꘡ʺx3Fʺ_queryOptions꘡> Instance { get; } = from _resourcePath_1 in __GeneratedOdata.Parsers.Rules._resourcePathParser.Instance
from _ʺx3Fʺ_queryOptions_1 in __GeneratedOdata.Parsers.Inners._ʺx3Fʺ_queryOptionsParser.Instance.Optional()
select new __GeneratedOdata.CstNodes.Rules._odataRelativeUri._resourcePath_꘡ʺx3Fʺ_queryOptions꘡(_resourcePath_1, _ʺx3Fʺ_queryOptions_1.GetOrElse(null));
        }
    }
    
}
