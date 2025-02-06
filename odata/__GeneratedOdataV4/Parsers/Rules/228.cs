namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _qualifiedEnumTypeNameParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._qualifiedEnumTypeName> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._qualifiedEnumTypeName>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._qualifiedEnumTypeName> Parse(IInput<char>? input)
            {
                var _namespace_1 = __GeneratedOdataV4.Parsers.Rules._namespaceParser.Instance.Parse(input);
if (!_namespace_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._qualifiedEnumTypeName)!, input);
}

var _ʺx2Eʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx2EʺParser.Instance.Parse(_namespace_1.Remainder);
if (!_ʺx2Eʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._qualifiedEnumTypeName)!, input);
}

var _enumerationTypeName_1 = __GeneratedOdataV4.Parsers.Rules._enumerationTypeNameParser.Instance.Parse(_ʺx2Eʺ_1.Remainder);
if (!_enumerationTypeName_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._qualifiedEnumTypeName)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._qualifiedEnumTypeName(_namespace_1.Parsed, _ʺx2Eʺ_1.Parsed, _enumerationTypeName_1.Parsed), _enumerationTypeName_1.Remainder);
            }
        }
    }
    
}
