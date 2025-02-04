namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _parameterAliasⳆparameterValueParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._parameterAliasⳆparameterValue> Instance { get; } = (_parameterAliasParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._parameterAliasⳆparameterValue>(_parameterValueParser.Instance);
        
        public static class _parameterAliasParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._parameterAliasⳆparameterValue._parameterAlias> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._parameterAliasⳆparameterValue._parameterAlias>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._parameterAliasⳆparameterValue._parameterAlias> Parse(IInput<char>? input)
                {
                    var _parameterAlias_1 = __GeneratedOdataV3.Parsers.Rules._parameterAliasParser.Instance.Parse(input);
if (!_parameterAlias_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._parameterAliasⳆparameterValue._parameterAlias)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._parameterAliasⳆparameterValue._parameterAlias(_parameterAlias_1.Parsed), _parameterAlias_1.Remainder);
                }
            }
        }
        
        public static class _parameterValueParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._parameterAliasⳆparameterValue._parameterValue> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._parameterAliasⳆparameterValue._parameterValue>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._parameterAliasⳆparameterValue._parameterValue> Parse(IInput<char>? input)
                {
                    var _parameterValue_1 = __GeneratedOdataV3.Parsers.Rules._parameterValueParser.Instance.Parse(input);
if (!_parameterValue_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._parameterAliasⳆparameterValue._parameterValue)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._parameterAliasⳆparameterValue._parameterValue(_parameterValue_1.Parsed), _parameterValue_1.Remainder);
                }
            }
        }
    }
    
}
