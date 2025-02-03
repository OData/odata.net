namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _keyPathSegmentsParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._keyPathSegments> Instance { get; } = from _Ⲥʺx2Fʺ_keyPathLiteralↃ_1 in __GeneratedOdataV2.Parsers.Inners._Ⲥʺx2Fʺ_keyPathLiteralↃParser.Instance.Repeat(1, null)
select new __GeneratedOdataV2.CstNodes.Rules._keyPathSegments(new __GeneratedOdataV2.CstNodes.Inners.HelperRangedAtLeast1<__GeneratedOdataV2.CstNodes.Inners._Ⲥʺx2Fʺ_keyPathLiteralↃ>(_Ⲥʺx2Fʺ_keyPathLiteralↃ_1));
    }
    
}
