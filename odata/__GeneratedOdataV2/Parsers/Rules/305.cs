namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _enumParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._enum> Instance { get; } = from _qualifiedEnumTypeName_1 in __GeneratedOdataV2.Parsers.Rules._qualifiedEnumTypeNameParser.Instance.Optional()
from _SQUOTE_1 in __GeneratedOdataV2.Parsers.Rules._SQUOTEParser.Instance
from _enumValue_1 in __GeneratedOdataV2.Parsers.Rules._enumValueParser.Instance
from _SQUOTE_2 in __GeneratedOdataV2.Parsers.Rules._SQUOTEParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._enum(_qualifiedEnumTypeName_1.GetOrElse(null), _SQUOTE_1, _enumValue_1, _SQUOTE_2);
    }
    
}
