namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _navigation_ЖⲤcontainmentNavigationↃ_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡Parser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._navigation_ЖⲤcontainmentNavigationↃ_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._navigation_ЖⲤcontainmentNavigationↃ_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._navigation_ЖⲤcontainmentNavigationↃ_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡> Parse(IInput<char>? input)
            {
                var _navigation_1 = __GeneratedOdataV3.Parsers.Rules._navigationParser.Instance.Parse(input);
if (!_navigation_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._navigation_ЖⲤcontainmentNavigationↃ_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡)!, input);
}

var _ⲤcontainmentNavigationↃ_1 = Inners._ⲤcontainmentNavigationↃParser.Instance.Many().Parse(_navigation_1.Remainder);
if (!_ⲤcontainmentNavigationↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._navigation_ЖⲤcontainmentNavigationↃ_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡)!, input);
}

var _ʺx2Fʺ_qualifiedEntityTypeName_1 = __GeneratedOdataV3.Parsers.Inners._ʺx2Fʺ_qualifiedEntityTypeNameParser.Instance.Optional().Parse(_ⲤcontainmentNavigationↃ_1.Remainder);
if (!_ʺx2Fʺ_qualifiedEntityTypeName_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._navigation_ЖⲤcontainmentNavigationↃ_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._navigation_ЖⲤcontainmentNavigationↃ_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡(_navigation_1.Parsed, _ⲤcontainmentNavigationↃ_1.Parsed,  _ʺx2Fʺ_qualifiedEntityTypeName_1.Parsed.GetOrElse(null)), _ʺx2Fʺ_qualifiedEntityTypeName_1.Remainder);
            }
        }
    }
    
}
