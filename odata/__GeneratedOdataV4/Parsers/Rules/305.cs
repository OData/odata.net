namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _enumParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._enum> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._enum>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._enum> Parse(IInput<char>? input)
            {
                var _qualifiedEnumTypeName_1 = __GeneratedOdataV4.Parsers.Rules._qualifiedEnumTypeNameParser.Instance.Optional().Parse(input);
if (!_qualifiedEnumTypeName_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._enum)!, input);
}

var _SQUOTE_1 = __GeneratedOdataV4.Parsers.Rules._SQUOTEParser.Instance.Parse(_qualifiedEnumTypeName_1.Remainder);
if (!_SQUOTE_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._enum)!, input);
}

var _enumValue_1 = __GeneratedOdataV4.Parsers.Rules._enumValueParser.Instance.Parse(_SQUOTE_1.Remainder);
if (!_enumValue_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._enum)!, input);
}

var _SQUOTE_2 = __GeneratedOdataV4.Parsers.Rules._SQUOTEParser.Instance.Parse(_enumValue_1.Remainder);
if (!_SQUOTE_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._enum)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._enum(_qualifiedEnumTypeName_1.Parsed.GetOrElse(null), _SQUOTE_1.Parsed, _enumValue_1.Parsed, _SQUOTE_2.Parsed), _SQUOTE_2.Remainder);
            }
        }
    }
    
}
