namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _primitiveFunctionImportParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveFunctionImport> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveFunctionImport>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._primitiveFunctionImport> Parse(IInput<char>? input)
            {
                var _odataIdentifier_1 = __GeneratedOdataV3.Parsers.Rules._odataIdentifierParser.Instance.Parse(input);
if (!_odataIdentifier_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._primitiveFunctionImport)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._primitiveFunctionImport(_odataIdentifier_1.Parsed), _odataIdentifier_1.Remainder);
            }
        }
    }
    
}
