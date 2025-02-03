namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _4base64charParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._4base64char> Instance { get; } = from _base64char_1 in __GeneratedOdataV3.Parsers.Rules._base64charParser.Instance.Repeat(4, 4)
select new __GeneratedOdataV3.CstNodes.Inners._4base64char(new __GeneratedOdataV3.CstNodes.Inners.HelperRangedExactly4<__GeneratedOdataV3.CstNodes.Rules._base64char>(_base64char_1));
    }
    
}
