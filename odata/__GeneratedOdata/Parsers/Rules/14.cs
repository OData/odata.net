namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _keyPathLiteralParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._keyPathLiteral> Instance { get; } = from _pchar_1 in __GeneratedOdata.Parsers.Rules._pcharParser.Instance.Many()
select new __GeneratedOdata.CstNodes.Rules._keyPathLiteral(_pchar_1);
    }
    
}
