namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _qualifiedFunctionNameParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._qualifiedFunctionName> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._qualifiedFunctionName>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._qualifiedFunctionName> Parse(IInput<char>? input)
            {
                var _namespace_1 = __GeneratedOdataV3.Parsers.Rules._namespaceParser.Instance.Parse(input);
if (!_namespace_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._qualifiedFunctionName)!, input);
}

var _ʺx2Eʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx2EʺParser.Instance.Parse(_namespace_1.Remainder);
if (!_ʺx2Eʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._qualifiedFunctionName)!, input);
}

var _function_1 = __GeneratedOdataV3.Parsers.Rules._functionParser.Instance.Parse(_ʺx2Eʺ_1.Remainder);
if (!_function_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._qualifiedFunctionName)!, input);
}

var _OPEN_parameterNames_CLOSE_1 = __GeneratedOdataV3.Parsers.Inners._OPEN_parameterNames_CLOSEParser.Instance.Optional().Parse(_function_1.Remainder);
if (!_OPEN_parameterNames_CLOSE_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._qualifiedFunctionName)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._qualifiedFunctionName(_namespace_1.Parsed, _ʺx2Eʺ_1.Parsed, _function_1.Parsed, _OPEN_parameterNames_CLOSE_1.Parsed.GetOrElse(null)), _OPEN_parameterNames_CLOSE_1.Remainder);
            }
        }
    }
    
}
