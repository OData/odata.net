namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _qualifiedEntityTypeNameParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._qualifiedEntityTypeName> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._qualifiedEntityTypeName>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._qualifiedEntityTypeName> Parse(IInput<char>? input)
            {
                var _namespace_1 = __GeneratedOdataV3.Parsers.Rules._namespaceParser.Instance.Parse(input);
if (!_namespace_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._qualifiedEntityTypeName)!, input);
}

var _ʺx2Eʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx2EʺParser.Instance.Parse(_namespace_1.Remainder);
if (!_ʺx2Eʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._qualifiedEntityTypeName)!, input);
}

var _entityTypeName_1 = __GeneratedOdataV3.Parsers.Rules._entityTypeNameParser.Instance.Parse(_ʺx2Eʺ_1.Remainder);
if (!_entityTypeName_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._qualifiedEntityTypeName)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._qualifiedEntityTypeName(_namespace_1.Parsed, _ʺx2Eʺ_1.Parsed, _entityTypeName_1.Parsed), _entityTypeName_1.Remainder);
            }
        }
    }
    
}
