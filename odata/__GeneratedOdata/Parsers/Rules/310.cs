namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _fullCollectionLiteralParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._fullCollectionLiteral> Instance { get; } = from _sridLiteral_1 in __GeneratedOdata.Parsers.Rules._sridLiteralParser.Instance
from _collectionLiteral_1 in __GeneratedOdata.Parsers.Rules._collectionLiteralParser.Instance
select new __GeneratedOdata.CstNodes.Rules._fullCollectionLiteral(_sridLiteral_1, _collectionLiteral_1);
    }
    
}
