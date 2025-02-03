namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _entitySetParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._entitySet> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._entitySet>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._entitySet> Parse(IInput<char>? input)
            {
                var _entitySetName_1 = __GeneratedOdataV3.Parsers.Rules._entitySetNameParser.Instance.Parse(input);
if (!_entitySetName_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._entitySet)!, input);
}

var _ⲤcontainmentNavigationↃ_1 = Inners._ⲤcontainmentNavigationↃParser.Instance.Many().Parse(_entitySetName_1.Remainder);
if (!_ⲤcontainmentNavigationↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._entitySet)!, input);
}

var _ʺx2Fʺ_qualifiedEntityTypeName_1 = __GeneratedOdataV3.Parsers.Inners._ʺx2Fʺ_qualifiedEntityTypeNameParser.Instance.Optional().Parse(_ⲤcontainmentNavigationↃ_1.Remainder);
if (!_ʺx2Fʺ_qualifiedEntityTypeName_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._entitySet)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._entitySet(_entitySetName_1.Parsed, _ⲤcontainmentNavigationↃ_1.Parsed,  _ʺx2Fʺ_qualifiedEntityTypeName_1.Parsed.GetOrElse(null)), _ʺx2Fʺ_qualifiedEntityTypeName_1.Remainder);
            }
        }
    }
    
}
