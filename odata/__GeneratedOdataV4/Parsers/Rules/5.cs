namespace __GeneratedOdataV4.Parsers.Rules
{
    using __GeneratedOdataV4.CstNodes.Rules;
    using CombinatorParsingV2;
    
    public static class _collectionNavigationParser
    {
        public static CombinatorParsingV3.IParser<char, CombinatorParsingV3.ParserExtensions.StringAdapter, __GeneratedOdataV4.CstNodes.Rules._collectionNavigation> Instance2 { get; } = new Parser2();

        private sealed class Parser2 : CombinatorParsingV3.IParser<char, CombinatorParsingV3.ParserExtensions.StringAdapter, __GeneratedOdataV4.CstNodes.Rules._collectionNavigation>
        {
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
            public _collectionNavigation Parse(CombinatorParsingV3.ParserExtensions.StringAdapter input, int start, out int newStart)
            {
                newStart = start;
                for (; newStart < start + 21; ++newStart)
                {
                    var next = input[newStart];
                }

                return default;
            }
        }

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
