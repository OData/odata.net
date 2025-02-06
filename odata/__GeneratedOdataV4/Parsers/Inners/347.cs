namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _valueⲻseparator_complexInUriParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._valueⲻseparator_complexInUri> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._valueⲻseparator_complexInUri>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._valueⲻseparator_complexInUri> Parse(IInput<char>? input)
            {
                var _valueⲻseparator_1 = __GeneratedOdataV4.Parsers.Rules._valueⲻseparatorParser.Instance.Parse(input);
if (!_valueⲻseparator_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._valueⲻseparator_complexInUri)!, input);
}

var _complexInUri_1 = __GeneratedOdataV4.Parsers.Rules._complexInUriParser.Instance.Parse(_valueⲻseparator_1.Remainder);
if (!_complexInUri_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._valueⲻseparator_complexInUri)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._valueⲻseparator_complexInUri(_valueⲻseparator_1.Parsed, _complexInUri_1.Parsed), _complexInUri_1.Remainder);
            }
        }
    }
    
}
