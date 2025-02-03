namespace __GeneratedOdataV2.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _4base64charParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._4base64char> Instance { get; } = from _base64char_1 in __GeneratedOdataV2.Parsers.Rules._base64charParser.Instance.Repeat(4, 4)
select new __GeneratedOdataV2.CstNodes.Inners._4base64char(new __GeneratedOdataV2.CstNodes.Inners.HelperRangedExactly4<__GeneratedOdataV2.CstNodes.Rules._base64char>(_base64char_1));
    }
    
}
