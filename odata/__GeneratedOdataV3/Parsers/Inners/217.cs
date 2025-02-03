namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _OPEN_parameterNames_CLOSEParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._OPEN_parameterNames_CLOSE> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._OPEN_parameterNames_CLOSE>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._OPEN_parameterNames_CLOSE> Parse(IInput<char>? input)
            {
                var _OPEN_1 = __GeneratedOdataV3.Parsers.Rules._OPENParser.Instance.Parse(input);
if (!_OPEN_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._OPEN_parameterNames_CLOSE)!, input);
}

var _parameterNames_1 = __GeneratedOdataV3.Parsers.Rules._parameterNamesParser.Instance.Parse(_OPEN_1.Remainder);
if (!_parameterNames_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._OPEN_parameterNames_CLOSE)!, input);
}

var _CLOSE_1 = __GeneratedOdataV3.Parsers.Rules._CLOSEParser.Instance.Parse(_parameterNames_1.Remainder);
if (!_CLOSE_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._OPEN_parameterNames_CLOSE)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._OPEN_parameterNames_CLOSE(_OPEN_1.Parsed, _parameterNames_1.Parsed,  _CLOSE_1.Parsed), _CLOSE_1.Remainder);
            }
        }
    }
    
}
