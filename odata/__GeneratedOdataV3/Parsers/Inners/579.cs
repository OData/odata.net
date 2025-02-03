namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _COMMA_annotationIdentifierParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._COMMA_annotationIdentifier> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._COMMA_annotationIdentifier>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._COMMA_annotationIdentifier> Parse(IInput<char>? input)
            {
                var _COMMA_1 = __GeneratedOdataV3.Parsers.Rules._COMMAParser.Instance.Parse(input);
if (!_COMMA_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._COMMA_annotationIdentifier)!, input);
}

var _annotationIdentifier_1 = __GeneratedOdataV3.Parsers.Rules._annotationIdentifierParser.Instance.Parse(_COMMA_1.Remainder);
if (!_annotationIdentifier_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._COMMA_annotationIdentifier)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._COMMA_annotationIdentifier(_COMMA_1.Parsed,  _annotationIdentifier_1.Parsed), _annotationIdentifier_1.Remainder);
            }
        }
    }
    
}
