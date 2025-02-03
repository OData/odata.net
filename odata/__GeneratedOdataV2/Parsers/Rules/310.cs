namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _fullCollectionLiteralParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._fullCollectionLiteral> Instance { get; } = from _sridLiteral_1 in __GeneratedOdataV2.Parsers.Rules._sridLiteralParser.Instance
from _collectionLiteral_1 in __GeneratedOdataV2.Parsers.Rules._collectionLiteralParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._fullCollectionLiteral(_sridLiteral_1, _collectionLiteral_1);
    }
    
}
