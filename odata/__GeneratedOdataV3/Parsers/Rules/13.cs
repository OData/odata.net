namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _keyPathSegmentsParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._keyPathSegments> Instance { get; } = from _Ⲥʺx2Fʺ_keyPathLiteralↃ_1 in __GeneratedOdataV3.Parsers.Inners._Ⲥʺx2Fʺ_keyPathLiteralↃParser.Instance.Repeat(1, null)
select new __GeneratedOdataV3.CstNodes.Rules._keyPathSegments(new __GeneratedOdataV3.CstNodes.Inners.HelperRangedAtLeast1<__GeneratedOdataV3.CstNodes.Inners._Ⲥʺx2Fʺ_keyPathLiteralↃ>(_Ⲥʺx2Fʺ_keyPathLiteralↃ_1));
    }
    
}
