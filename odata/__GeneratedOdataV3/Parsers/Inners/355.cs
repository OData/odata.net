namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _quotationⲻmark_primitiveColProperty_quotationⲻmark_nameⲻseparator_primitiveColInUriParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._quotationⲻmark_primitiveColProperty_quotationⲻmark_nameⲻseparator_primitiveColInUri> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._quotationⲻmark_primitiveColProperty_quotationⲻmark_nameⲻseparator_primitiveColInUri>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._quotationⲻmark_primitiveColProperty_quotationⲻmark_nameⲻseparator_primitiveColInUri> Parse(IInput<char>? input)
            {
                var _quotationⲻmark_1 = __GeneratedOdataV3.Parsers.Rules._quotationⲻmarkParser.Instance.Parse(input);
if (!_quotationⲻmark_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._quotationⲻmark_primitiveColProperty_quotationⲻmark_nameⲻseparator_primitiveColInUri)!, input);
}

var _primitiveColProperty_1 = __GeneratedOdataV3.Parsers.Rules._primitiveColPropertyParser.Instance.Parse(_quotationⲻmark_1.Remainder);
if (!_primitiveColProperty_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._quotationⲻmark_primitiveColProperty_quotationⲻmark_nameⲻseparator_primitiveColInUri)!, input);
}

var _quotationⲻmark_2 = __GeneratedOdataV3.Parsers.Rules._quotationⲻmarkParser.Instance.Parse(_primitiveColProperty_1.Remainder);
if (!_quotationⲻmark_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._quotationⲻmark_primitiveColProperty_quotationⲻmark_nameⲻseparator_primitiveColInUri)!, input);
}

var _nameⲻseparator_1 = __GeneratedOdataV3.Parsers.Rules._nameⲻseparatorParser.Instance.Parse(_quotationⲻmark_2.Remainder);
if (!_nameⲻseparator_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._quotationⲻmark_primitiveColProperty_quotationⲻmark_nameⲻseparator_primitiveColInUri)!, input);
}

var _primitiveColInUri_1 = __GeneratedOdataV3.Parsers.Rules._primitiveColInUriParser.Instance.Parse(_nameⲻseparator_1.Remainder);
if (!_primitiveColInUri_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._quotationⲻmark_primitiveColProperty_quotationⲻmark_nameⲻseparator_primitiveColInUri)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._quotationⲻmark_primitiveColProperty_quotationⲻmark_nameⲻseparator_primitiveColInUri(_quotationⲻmark_1.Parsed, _primitiveColProperty_1.Parsed, _quotationⲻmark_2.Parsed, _nameⲻseparator_1.Parsed,  _primitiveColInUri_1.Parsed), _primitiveColInUri_1.Remainder);
            }
        }
    }
    
}
