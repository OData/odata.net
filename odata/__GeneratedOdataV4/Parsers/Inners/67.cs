namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ⲤparameterAliasⳆprimitiveLiteralↃParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ⲤparameterAliasⳆprimitiveLiteralↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ⲤparameterAliasⳆprimitiveLiteralↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ⲤparameterAliasⳆprimitiveLiteralↃ> Parse(IInput<char>? input)
            {
                var _parameterAliasⳆprimitiveLiteral_1 = __GeneratedOdataV4.Parsers.Inners._parameterAliasⳆprimitiveLiteralParser.Instance.Parse(input);
if (!_parameterAliasⳆprimitiveLiteral_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ⲤparameterAliasⳆprimitiveLiteralↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._ⲤparameterAliasⳆprimitiveLiteralↃ(_parameterAliasⳆprimitiveLiteral_1.Parsed), _parameterAliasⳆprimitiveLiteral_1.Remainder);
            }
        }
    }
    
}
