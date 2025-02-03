namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _quotationⲻmark_complexColProperty_quotationⲻmark_nameⲻseparator_complexColInUriParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._quotationⲻmark_complexColProperty_quotationⲻmark_nameⲻseparator_complexColInUri> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._quotationⲻmark_complexColProperty_quotationⲻmark_nameⲻseparator_complexColInUri>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._quotationⲻmark_complexColProperty_quotationⲻmark_nameⲻseparator_complexColInUri> Parse(IInput<char>? input)
            {
                var _quotationⲻmark_1 = __GeneratedOdataV3.Parsers.Rules._quotationⲻmarkParser.Instance.Parse(input);
if (!_quotationⲻmark_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._quotationⲻmark_complexColProperty_quotationⲻmark_nameⲻseparator_complexColInUri)!, input);
}

var _complexColProperty_1 = __GeneratedOdataV3.Parsers.Rules._complexColPropertyParser.Instance.Parse(_quotationⲻmark_1.Remainder);
if (!_complexColProperty_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._quotationⲻmark_complexColProperty_quotationⲻmark_nameⲻseparator_complexColInUri)!, input);
}

var _quotationⲻmark_2 = __GeneratedOdataV3.Parsers.Rules._quotationⲻmarkParser.Instance.Parse(_complexColProperty_1.Remainder);
if (!_quotationⲻmark_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._quotationⲻmark_complexColProperty_quotationⲻmark_nameⲻseparator_complexColInUri)!, input);
}

var _nameⲻseparator_1 = __GeneratedOdataV3.Parsers.Rules._nameⲻseparatorParser.Instance.Parse(_quotationⲻmark_2.Remainder);
if (!_nameⲻseparator_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._quotationⲻmark_complexColProperty_quotationⲻmark_nameⲻseparator_complexColInUri)!, input);
}

var _complexColInUri_1 = __GeneratedOdataV3.Parsers.Rules._complexColInUriParser.Instance.Parse(_nameⲻseparator_1.Remainder);
if (!_complexColInUri_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._quotationⲻmark_complexColProperty_quotationⲻmark_nameⲻseparator_complexColInUri)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._quotationⲻmark_complexColProperty_quotationⲻmark_nameⲻseparator_complexColInUri(_quotationⲻmark_1.Parsed, _complexColProperty_1.Parsed, _quotationⲻmark_2.Parsed, _nameⲻseparator_1.Parsed,  _complexColInUri_1.Parsed), _complexColInUri_1.Remainder);
            }
        }
    }
    
}
