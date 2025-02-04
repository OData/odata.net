namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ⲤCOMMA_annotationIdentifierↃParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ⲤCOMMA_annotationIdentifierↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ⲤCOMMA_annotationIdentifierↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ⲤCOMMA_annotationIdentifierↃ> Parse(IInput<char>? input)
            {
                var _COMMA_annotationIdentifier_1 = __GeneratedOdataV3.Parsers.Inners._COMMA_annotationIdentifierParser.Instance.Parse(input);
if (!_COMMA_annotationIdentifier_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ⲤCOMMA_annotationIdentifierↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._ⲤCOMMA_annotationIdentifierↃ(_COMMA_annotationIdentifier_1.Parsed), _COMMA_annotationIdentifier_1.Remainder);
            }
        }
    }
    
}
