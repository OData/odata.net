namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _allOperationsInSchemaParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._allOperationsInSchema> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._allOperationsInSchema>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._allOperationsInSchema> Parse(IInput<char>? input)
            {
                var _namespace_1 = __GeneratedOdataV3.Parsers.Rules._namespaceParser.Instance.Parse(input);
if (!_namespace_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._allOperationsInSchema)!, input);
}

var _ʺx2Eʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx2EʺParser.Instance.Parse(_namespace_1.Remainder);
if (!_ʺx2Eʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._allOperationsInSchema)!, input);
}

var _STAR_1 = __GeneratedOdataV3.Parsers.Rules._STARParser.Instance.Parse(_ʺx2Eʺ_1.Remainder);
if (!_STAR_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._allOperationsInSchema)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._allOperationsInSchema(_namespace_1.Parsed, _ʺx2Eʺ_1.Parsed,  _STAR_1.Parsed), _STAR_1.Remainder);
            }
        }
    }
    
}
