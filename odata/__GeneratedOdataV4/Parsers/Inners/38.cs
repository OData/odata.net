namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ⲤparameterAliasⳆkeyPropertyValueↃParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ⲤparameterAliasⳆkeyPropertyValueↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ⲤparameterAliasⳆkeyPropertyValueↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ⲤparameterAliasⳆkeyPropertyValueↃ> Parse(IInput<char>? input)
            {
                var _parameterAliasⳆkeyPropertyValue_1 = __GeneratedOdataV4.Parsers.Inners._parameterAliasⳆkeyPropertyValueParser.Instance.Parse(input);
if (!_parameterAliasⳆkeyPropertyValue_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ⲤparameterAliasⳆkeyPropertyValueↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._ⲤparameterAliasⳆkeyPropertyValueↃ(_parameterAliasⳆkeyPropertyValue_1.Parsed), _parameterAliasⳆkeyPropertyValue_1.Remainder);
            }
        }
    }
    
}
