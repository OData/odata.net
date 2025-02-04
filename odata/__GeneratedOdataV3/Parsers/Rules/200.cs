namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _complexPropertyInUriParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._complexPropertyInUri> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._complexPropertyInUri>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._complexPropertyInUri> Parse(IInput<char>? input)
            {
                var _quotationⲻmark_1 = __GeneratedOdataV3.Parsers.Rules._quotationⲻmarkParser.Instance.Parse(input);
if (!_quotationⲻmark_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._complexPropertyInUri)!, input);
}

var _complexProperty_1 = __GeneratedOdataV3.Parsers.Rules._complexPropertyParser.Instance.Parse(_quotationⲻmark_1.Remainder);
if (!_complexProperty_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._complexPropertyInUri)!, input);
}

var _quotationⲻmark_2 = __GeneratedOdataV3.Parsers.Rules._quotationⲻmarkParser.Instance.Parse(_complexProperty_1.Remainder);
if (!_quotationⲻmark_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._complexPropertyInUri)!, input);
}

var _nameⲻseparator_1 = __GeneratedOdataV3.Parsers.Rules._nameⲻseparatorParser.Instance.Parse(_quotationⲻmark_2.Remainder);
if (!_nameⲻseparator_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._complexPropertyInUri)!, input);
}

var _complexInUri_1 = __GeneratedOdataV3.Parsers.Rules._complexInUriParser.Instance.Parse(_nameⲻseparator_1.Remainder);
if (!_complexInUri_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._complexPropertyInUri)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._complexPropertyInUri(_quotationⲻmark_1.Parsed, _complexProperty_1.Parsed, _quotationⲻmark_2.Parsed, _nameⲻseparator_1.Parsed, _complexInUri_1.Parsed), _complexInUri_1.Remainder);
            }
        }
    }
    
}
