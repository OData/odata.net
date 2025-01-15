namespace __GeneratedOdata.Parsers.Inners
{
    using Sprache;
    
    public static class _4base64charParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Inners._4base64char> Instance { get; } = from _base64char_1 in __GeneratedOdata.Parsers.Rules._base64charParser.Instance.Repeat(4, 4)
select new __GeneratedOdata.CstNodes.Inners._4base64char(new __GeneratedOdata.CstNodes.Inners.HelperRangedExactly4<__GeneratedOdata.CstNodes.Rules._base64char>(_base64char_1));
    }
    
}
