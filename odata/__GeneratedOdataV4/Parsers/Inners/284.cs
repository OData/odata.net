namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ⲤparameterAliasⳆparameterValueↃParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ⲤparameterAliasⳆparameterValueↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ⲤparameterAliasⳆparameterValueↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ⲤparameterAliasⳆparameterValueↃ> Parse(IInput<char>? input)
            {
                var _parameterAliasⳆparameterValue_1 = __GeneratedOdataV4.Parsers.Inners._parameterAliasⳆparameterValueParser.Instance.Parse(input);
if (!_parameterAliasⳆparameterValue_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ⲤparameterAliasⳆparameterValueↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._ⲤparameterAliasⳆparameterValueↃ(_parameterAliasⳆparameterValue_1.Parsed), _parameterAliasⳆparameterValue_1.Remainder);
            }
        }
    }
    
}
