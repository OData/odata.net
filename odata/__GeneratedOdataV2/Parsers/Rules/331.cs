namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _positionLiteralParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._positionLiteral> Instance { get; } = from _doubleValue_1 in __GeneratedOdataV2.Parsers.Rules._doubleValueParser.Instance
from _SP_1 in __GeneratedOdataV2.Parsers.Rules._SPParser.Instance
from _doubleValue_2 in __GeneratedOdataV2.Parsers.Rules._doubleValueParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._positionLiteral(_doubleValue_1, _SP_1, _doubleValue_2);
    }
    
}
