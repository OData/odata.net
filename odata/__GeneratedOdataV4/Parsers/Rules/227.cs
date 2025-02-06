namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _qualifiedTypeDefinitionNameParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._qualifiedTypeDefinitionName> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._qualifiedTypeDefinitionName>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._qualifiedTypeDefinitionName> Parse(IInput<char>? input)
            {
                var _namespace_1 = __GeneratedOdataV4.Parsers.Rules._namespaceParser.Instance.Parse(input);
if (!_namespace_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._qualifiedTypeDefinitionName)!, input);
}

var _ʺx2Eʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx2EʺParser.Instance.Parse(_namespace_1.Remainder);
if (!_ʺx2Eʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._qualifiedTypeDefinitionName)!, input);
}

var _typeDefinitionName_1 = __GeneratedOdataV4.Parsers.Rules._typeDefinitionNameParser.Instance.Parse(_ʺx2Eʺ_1.Remainder);
if (!_typeDefinitionName_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._qualifiedTypeDefinitionName)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._qualifiedTypeDefinitionName(_namespace_1.Parsed, _ʺx2Eʺ_1.Parsed, _typeDefinitionName_1.Parsed), _typeDefinitionName_1.Remainder);
            }
        }
    }
    
}
