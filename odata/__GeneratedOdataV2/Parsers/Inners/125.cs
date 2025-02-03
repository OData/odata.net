namespace __GeneratedOdataV2.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _STARⳆstreamPropertyⳆnavigationProperty_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡Parser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._STARⳆstreamPropertyⳆnavigationProperty_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡> Instance { get; } = (_STARParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Inners._STARⳆstreamPropertyⳆnavigationProperty_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡>(_streamPropertyParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Inners._STARⳆstreamPropertyⳆnavigationProperty_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡>(_navigationProperty_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡Parser.Instance);
        
        public static class _STARParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._STARⳆstreamPropertyⳆnavigationProperty_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡._STAR> Instance { get; } = from _STAR_1 in __GeneratedOdataV2.Parsers.Rules._STARParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._STARⳆstreamPropertyⳆnavigationProperty_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡._STAR(_STAR_1);
        }
        
        public static class _streamPropertyParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._STARⳆstreamPropertyⳆnavigationProperty_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡._streamProperty> Instance { get; } = from _streamProperty_1 in __GeneratedOdataV2.Parsers.Rules._streamPropertyParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._STARⳆstreamPropertyⳆnavigationProperty_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡._streamProperty(_streamProperty_1);
        }
        
        public static class _navigationProperty_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡Parser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._STARⳆstreamPropertyⳆnavigationProperty_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡._navigationProperty_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡> Instance { get; } = from _navigationProperty_1 in __GeneratedOdataV2.Parsers.Rules._navigationPropertyParser.Instance
from _ʺx2Fʺ_qualifiedEntityTypeName_1 in __GeneratedOdataV2.Parsers.Inners._ʺx2Fʺ_qualifiedEntityTypeNameParser.Instance.Optional()
select new __GeneratedOdataV2.CstNodes.Inners._STARⳆstreamPropertyⳆnavigationProperty_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡._navigationProperty_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡(_navigationProperty_1, _ʺx2Fʺ_qualifiedEntityTypeName_1.GetOrElse(null));
        }
    }
    
}
