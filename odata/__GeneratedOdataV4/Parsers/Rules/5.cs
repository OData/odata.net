namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _collectionNavigationParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._collectionNavigation> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._collectionNavigation>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._collectionNavigation> Parse(IInput<char>? input)
            {

var _collectionNavPath_1 = __GeneratedOdataV4.Parsers.Rules._collectionNavPathParser.Instance.Optional().Parse(input);

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._collectionNavigation(null, _collectionNavPath_1.Parsed.GetOrElse(null)), _collectionNavPath_1.Remainder);
            }
        }
    }
    
}
