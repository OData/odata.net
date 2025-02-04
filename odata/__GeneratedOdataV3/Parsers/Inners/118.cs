namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ⲤqualifiedEntityTypeNameⳆqualifiedComplexTypeNameↃParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ⲤqualifiedEntityTypeNameⳆqualifiedComplexTypeNameↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ⲤqualifiedEntityTypeNameⳆqualifiedComplexTypeNameↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ⲤqualifiedEntityTypeNameⳆqualifiedComplexTypeNameↃ> Parse(IInput<char>? input)
            {
                var _qualifiedEntityTypeNameⳆqualifiedComplexTypeName_1 = __GeneratedOdataV3.Parsers.Inners._qualifiedEntityTypeNameⳆqualifiedComplexTypeNameParser.Instance.Parse(input);
if (!_qualifiedEntityTypeNameⳆqualifiedComplexTypeName_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ⲤqualifiedEntityTypeNameⳆqualifiedComplexTypeNameↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._ⲤqualifiedEntityTypeNameⳆqualifiedComplexTypeNameↃ(_qualifiedEntityTypeNameⳆqualifiedComplexTypeName_1.Parsed), _qualifiedEntityTypeNameⳆqualifiedComplexTypeName_1.Remainder);
            }
        }
    }
    
}
