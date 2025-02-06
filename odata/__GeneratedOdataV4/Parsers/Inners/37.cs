namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _parameterAliasⳆkeyPropertyValueParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._parameterAliasⳆkeyPropertyValue> Instance { get; } = (_parameterAliasParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Inners._parameterAliasⳆkeyPropertyValue>(_keyPropertyValueParser.Instance);
        
        public static class _parameterAliasParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._parameterAliasⳆkeyPropertyValue._parameterAlias> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._parameterAliasⳆkeyPropertyValue._parameterAlias>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._parameterAliasⳆkeyPropertyValue._parameterAlias> Parse(IInput<char>? input)
                {
                    var _parameterAlias_1 = __GeneratedOdataV4.Parsers.Rules._parameterAliasParser.Instance.Parse(input);
if (!_parameterAlias_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._parameterAliasⳆkeyPropertyValue._parameterAlias)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._parameterAliasⳆkeyPropertyValue._parameterAlias(_parameterAlias_1.Parsed), _parameterAlias_1.Remainder);
                }
            }
        }
        
        public static class _keyPropertyValueParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._parameterAliasⳆkeyPropertyValue._keyPropertyValue> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._parameterAliasⳆkeyPropertyValue._keyPropertyValue>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._parameterAliasⳆkeyPropertyValue._keyPropertyValue> Parse(IInput<char>? input)
                {
                    var _keyPropertyValue_1 = __GeneratedOdataV4.Parsers.Rules._keyPropertyValueParser.Instance.Parse(input);
if (!_keyPropertyValue_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._parameterAliasⳆkeyPropertyValue._keyPropertyValue)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._parameterAliasⳆkeyPropertyValue._keyPropertyValue(_keyPropertyValue_1.Parsed), _keyPropertyValue_1.Remainder);
                }
            }
        }
    }
    
}
