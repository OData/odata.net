namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _keyPathSegmentsParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._keyPathSegments> Instance { get; } = from _Ⲥʺx2Fʺ_keyPathLiteralↃ_1 in __GeneratedOdata.Parsers.Inners._Ⲥʺx2Fʺ_keyPathLiteralↃParser.Instance.Repeat(1, null)
select new __GeneratedOdata.CstNodes.Rules._keyPathSegments(new __GeneratedOdata.CstNodes.Inners.HelperRangedAtLeast1<__GeneratedOdata.CstNodes.Inners._Ⲥʺx2Fʺ_keyPathLiteralↃ>(_Ⲥʺx2Fʺ_keyPathLiteralↃ_1));
    }
    
}
