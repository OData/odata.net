namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _annotationsListParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._annotationsList> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._annotationsList>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._annotationsList> Parse(IInput<char>? input)
            {
                var _annotationIdentifier_1 = __GeneratedOdataV4.Parsers.Rules._annotationIdentifierParser.Instance.Parse(input);
if (!_annotationIdentifier_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._annotationsList)!, input);
}

var _ⲤCOMMA_annotationIdentifierↃ_1 = Inners._ⲤCOMMA_annotationIdentifierↃParser.Instance.Many().Parse(_annotationIdentifier_1.Remainder);
if (!_ⲤCOMMA_annotationIdentifierↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._annotationsList)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._annotationsList(_annotationIdentifier_1.Parsed, _ⲤCOMMA_annotationIdentifierↃ_1.Parsed), _ⲤCOMMA_annotationIdentifierↃ_1.Remainder);
            }
        }
    }
    
}
