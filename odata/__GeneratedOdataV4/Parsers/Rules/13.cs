namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _keyPathSegmentsParser
    {
        public static CombinatorParsingV3.IParser<char, CombinatorParsingV3.ParserExtensions.StringAdapter, __GeneratedOdataV4.CstNodes.Rules._keyPathSegments> Instance2 { get; } = new Parser2();

        private sealed class Parser2 : CombinatorParsingV3.IParser<char, CombinatorParsingV3.ParserExtensions.StringAdapter, __GeneratedOdataV4.CstNodes.Rules._keyPathSegments>
        {
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
            public __GeneratedOdataV4.CstNodes.Rules._keyPathSegments Parse(CombinatorParsingV3.ParserExtensions.StringAdapter input, int start, out int newStart)
            {
                newStart = start;
                for (; newStart < start + 5; ++newStart)
                {
                    var next = input[newStart];
                }

                return default;
            }
        }

        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._keyPathSegments> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._keyPathSegments>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._keyPathSegments> Parse(IInput<char>? input)
            {
                var _Ⲥʺx2Fʺ_keyPathLiteralↃ_1 = __GeneratedOdataV4.Parsers.Inners._Ⲥʺx2Fʺ_keyPathLiteralↃParser.Instance.Repeat(1, null).Parse(input);
if (!_Ⲥʺx2Fʺ_keyPathLiteralↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._keyPathSegments)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._keyPathSegments(new __GeneratedOdataV4.CstNodes.Inners.HelperRangedAtLeast1<__GeneratedOdataV4.CstNodes.Inners._Ⲥʺx2Fʺ_keyPathLiteralↃ>(_Ⲥʺx2Fʺ_keyPathLiteralↃ_1.Parsed)), _Ⲥʺx2Fʺ_keyPathLiteralↃ_1.Remainder);
            }
        }
    }
    
}
