namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _containmentNavigationParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._containmentNavigation> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._containmentNavigation>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._containmentNavigation> Parse(IInput<char>? input)
            {
                var _keyPredicate_1 = __GeneratedOdataV4.Parsers.Rules._keyPredicateParser.Instance.Parse(input);
if (!_keyPredicate_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._containmentNavigation)!, input);
}

var _ʺx2Fʺ_qualifiedEntityTypeName_1 = __GeneratedOdataV4.Parsers.Inners._ʺx2Fʺ_qualifiedEntityTypeNameParser.Instance.Optional().Parse(_keyPredicate_1.Remainder);
if (!_ʺx2Fʺ_qualifiedEntityTypeName_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._containmentNavigation)!, input);
}

var _navigation_1 = __GeneratedOdataV4.Parsers.Rules._navigationParser.Instance.Parse(_ʺx2Fʺ_qualifiedEntityTypeName_1.Remainder);
if (!_navigation_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._containmentNavigation)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._containmentNavigation(_keyPredicate_1.Parsed, _ʺx2Fʺ_qualifiedEntityTypeName_1.Parsed.GetOrElse(null), _navigation_1.Parsed), _navigation_1.Remainder);
            }
        }
    }
    
}
