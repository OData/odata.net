namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _collectionNavigationParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._collectionNavigation> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._collectionNavigation>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._collectionNavigation> Parse(IInput<char>? input)
            {
                var _ʺx2Fʺ_qualifiedEntityTypeName_1 = __GeneratedOdataV3.Parsers.Inners._ʺx2Fʺ_qualifiedEntityTypeNameParser.Instance.Optional().Parse(input);
if (!_ʺx2Fʺ_qualifiedEntityTypeName_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._collectionNavigation)!, input);
}

var _collectionNavPath_1 = __GeneratedOdataV3.Parsers.Rules._collectionNavPathParser.Instance.Optional().Parse(_ʺx2Fʺ_qualifiedEntityTypeName_1.Remainder);
if (!_collectionNavPath_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._collectionNavigation)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._collectionNavigation(_ʺx2Fʺ_qualifiedEntityTypeName_1.Parsed.GetOrElse(null),  _collectionNavPath_1.Parsed.GetOrElse(null)), _collectionNavPath_1.Remainder);
            }
        }
    }
    
}
