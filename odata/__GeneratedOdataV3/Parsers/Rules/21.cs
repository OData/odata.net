namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _filterInPathParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._filterInPath> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._filterInPath>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._filterInPath> Parse(IInput<char>? input)
            {
                var _ʺx2Fx24x66x69x6Cx74x65x72ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx2Fx24x66x69x6Cx74x65x72ʺParser.Instance.Parse(input);
if (!_ʺx2Fx24x66x69x6Cx74x65x72ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._filterInPath)!, input);
}

var _EQ_1 = __GeneratedOdataV3.Parsers.Rules._EQParser.Instance.Parse(_ʺx2Fx24x66x69x6Cx74x65x72ʺ_1.Remainder);
if (!_EQ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._filterInPath)!, input);
}

var _parameterAlias_1 = __GeneratedOdataV3.Parsers.Rules._parameterAliasParser.Instance.Parse(_EQ_1.Remainder);
if (!_parameterAlias_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._filterInPath)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._filterInPath(_ʺx2Fx24x66x69x6Cx74x65x72ʺ_1.Parsed, _EQ_1.Parsed,  _parameterAlias_1.Parsed), _parameterAlias_1.Remainder);
            }
        }
    }
    
}
