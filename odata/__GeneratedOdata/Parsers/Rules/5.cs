namespace __GeneratedOdata.Parsers.Rules
{
    using __GeneratedOdata.CstNodes.Rules;
    using CombinatorParsingV2;
    
    public static class _collectionNavigationParser
    {
        /*public static IParser<char, __GeneratedOdata.CstNodes.Rules._collectionNavigation> Instance { get; } = from _ʺx2Fʺ_qualifiedEntityTypeName_1 in __GeneratedOdata.Parsers.Inners._ʺx2Fʺ_qualifiedEntityTypeNameParser.Instance.Optional()
from _collectionNavPath_1 in __GeneratedOdata.Parsers.Rules._collectionNavPathParser.Instance.Optional()
select new __GeneratedOdata.CstNodes.Rules._collectionNavigation(_ʺx2Fʺ_qualifiedEntityTypeName_1.GetOrElse(null), _collectionNavPath_1.GetOrElse(null));
        */
        //// PERF
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._collectionNavigation> Instance { get; } = new Parser();

        private sealed class Parser : IParser<char, __GeneratedOdata.CstNodes.Rules._collectionNavigation>
        {
            public IOutput<char, _collectionNavigation> Parse(IInput<char>? input)
            {
                ////var _ʺx2Fʺ_qualifiedEntityTypeName_1 = __GeneratedOdata.Parsers.Inners._ʺx2Fʺ_qualifiedEntityTypeNameParser.Instance.Optional().Parse(input);
                var _collectionNavPath_1 = __GeneratedOdata.Parsers.Rules._collectionNavPathParser.Instance.Optional().Parse(input);
                return Output.Create(
                    true,
                    new __GeneratedOdata.CstNodes.Rules._collectionNavigation(null, _collectionNavPath_1.Parsed.GetOrElse(null)),
                     _collectionNavPath_1.Remainder);
            }
        }
    }
    
}
