namespace __GeneratedOdata.Parsers.Rules
{
    using __GeneratedOdata.CstNodes.Rules;
    using CombinatorParsingV2;
    
    public static class _keyPathSegmentsParser
    {
        /*public static IParser<char, __GeneratedOdata.CstNodes.Rules._keyPathSegments> Instance { get; } = from _Ⲥʺx2Fʺ_keyPathLiteralↃ_1 in __GeneratedOdata.Parsers.Inners._Ⲥʺx2Fʺ_keyPathLiteralↃParser.Instance.Repeat(1, null)
select new __GeneratedOdata.CstNodes.Rules._keyPathSegments(new __GeneratedOdata.CstNodes.Inners.HelperRangedAtLeast1<__GeneratedOdata.CstNodes.Inners._Ⲥʺx2Fʺ_keyPathLiteralↃ>(_Ⲥʺx2Fʺ_keyPathLiteralↃ_1));
        */
        //// PERF
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._keyPathSegments> Instance { get; } = new Parser();

        private sealed class Parser : IParser<char, __GeneratedOdata.CstNodes.Rules._keyPathSegments>
        {
            public IOutput<char, _keyPathSegments> Parse(IInput<char>? input)
            {
                var _Ⲥʺx2Fʺ_keyPathLiteralↃ_1 = __GeneratedOdata.Parsers.Inners._Ⲥʺx2Fʺ_keyPathLiteralↃParser.Instance.Repeat(1, null).Parse(input);
                if (!_Ⲥʺx2Fʺ_keyPathLiteralↃ_1.Success)
                {
                    return Output.Create(
                        false,
                        default(_keyPathSegments)!,
                        input);
                }

                return Output.Create(
                    true,
                    new __GeneratedOdata.CstNodes.Rules._keyPathSegments(new __GeneratedOdata.CstNodes.Inners.HelperRangedAtLeast1<__GeneratedOdata.CstNodes.Inners._Ⲥʺx2Fʺ_keyPathLiteralↃ>(_Ⲥʺx2Fʺ_keyPathLiteralↃ_1.Parsed)),
                    _Ⲥʺx2Fʺ_keyPathLiteralↃ_1.Remainder);
            }
        }
    }
    
}
