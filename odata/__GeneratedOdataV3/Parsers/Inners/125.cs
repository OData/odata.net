namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _STARⳆstreamPropertyⳆnavigationProperty_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡Parser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._STARⳆstreamPropertyⳆnavigationProperty_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡> Instance { get; } = (_STARParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._STARⳆstreamPropertyⳆnavigationProperty_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡>(_streamPropertyParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._STARⳆstreamPropertyⳆnavigationProperty_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡>(_navigationProperty_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡Parser.Instance);
        
        public static class _STARParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._STARⳆstreamPropertyⳆnavigationProperty_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡._STAR> Instance { get; } = from _STAR_1 in __GeneratedOdataV3.Parsers.Rules._STARParser.Instance
select new __GeneratedOdataV3.CstNodes.Inners._STARⳆstreamPropertyⳆnavigationProperty_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡._STAR(_STAR_1);
        }
        
        public static class _streamPropertyParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._STARⳆstreamPropertyⳆnavigationProperty_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡._streamProperty> Instance { get; } = from _streamProperty_1 in __GeneratedOdataV3.Parsers.Rules._streamPropertyParser.Instance
select new __GeneratedOdataV3.CstNodes.Inners._STARⳆstreamPropertyⳆnavigationProperty_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡._streamProperty(_streamProperty_1);
        }
        
        public static class _navigationProperty_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡Parser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._STARⳆstreamPropertyⳆnavigationProperty_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡._navigationProperty_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._STARⳆstreamPropertyⳆnavigationProperty_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡._navigationProperty_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._STARⳆstreamPropertyⳆnavigationProperty_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡._navigationProperty_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡> Parse(IInput<char>? input)
                {
                    var _navigationProperty_1 = __GeneratedOdataV3.Parsers.Rules._navigationPropertyParser.Instance.Parse(input);
if (!_navigationProperty_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._STARⳆstreamPropertyⳆnavigationProperty_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡._navigationProperty_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡)!, input);
}

var _ʺx2Fʺ_qualifiedEntityTypeName_1 = __GeneratedOdataV3.Parsers.Inners._ʺx2Fʺ_qualifiedEntityTypeNameParser.Instance.Optional().Parse(_navigationProperty_1.Remainder);
if (!_ʺx2Fʺ_qualifiedEntityTypeName_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._STARⳆstreamPropertyⳆnavigationProperty_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡._navigationProperty_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._STARⳆstreamPropertyⳆnavigationProperty_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡._navigationProperty_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡(_navigationProperty_1.Parsed,  _ʺx2Fʺ_qualifiedEntityTypeName_1.Parsed.GetOrElse(null)), _ʺx2Fʺ_qualifiedEntityTypeName_1.Remainder);
                }
            }
        }
    }
    
}
