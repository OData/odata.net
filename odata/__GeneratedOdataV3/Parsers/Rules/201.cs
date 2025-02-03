namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _annotationInUriParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._annotationInUri> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._annotationInUri>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._annotationInUri> Parse(IInput<char>? input)
            {
                var _quotationⲻmark_1 = __GeneratedOdataV3.Parsers.Rules._quotationⲻmarkParser.Instance.Parse(input);
if (!_quotationⲻmark_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._annotationInUri)!, input);
}

var _AT_1 = __GeneratedOdataV3.Parsers.Rules._ATParser.Instance.Parse(_quotationⲻmark_1.Remainder);
if (!_AT_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._annotationInUri)!, input);
}

var _namespace_1 = __GeneratedOdataV3.Parsers.Rules._namespaceParser.Instance.Parse(_AT_1.Remainder);
if (!_namespace_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._annotationInUri)!, input);
}

var _ʺx2Eʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx2EʺParser.Instance.Parse(_namespace_1.Remainder);
if (!_ʺx2Eʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._annotationInUri)!, input);
}

var _termName_1 = __GeneratedOdataV3.Parsers.Rules._termNameParser.Instance.Parse(_ʺx2Eʺ_1.Remainder);
if (!_termName_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._annotationInUri)!, input);
}

var _quotationⲻmark_2 = __GeneratedOdataV3.Parsers.Rules._quotationⲻmarkParser.Instance.Parse(_termName_1.Remainder);
if (!_quotationⲻmark_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._annotationInUri)!, input);
}

var _nameⲻseparator_1 = __GeneratedOdataV3.Parsers.Rules._nameⲻseparatorParser.Instance.Parse(_quotationⲻmark_2.Remainder);
if (!_nameⲻseparator_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._annotationInUri)!, input);
}

var _ⲤcomplexInUriⳆcomplexColInUriⳆprimitiveLiteralInJSONⳆprimitiveColInUriↃ_1 = __GeneratedOdataV3.Parsers.Inners._ⲤcomplexInUriⳆcomplexColInUriⳆprimitiveLiteralInJSONⳆprimitiveColInUriↃParser.Instance.Parse(_nameⲻseparator_1.Remainder);
if (!_ⲤcomplexInUriⳆcomplexColInUriⳆprimitiveLiteralInJSONⳆprimitiveColInUriↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._annotationInUri)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._annotationInUri(_quotationⲻmark_1.Parsed, _AT_1.Parsed, _namespace_1.Parsed, _ʺx2Eʺ_1.Parsed, _termName_1.Parsed, _quotationⲻmark_2.Parsed, _nameⲻseparator_1.Parsed,  _ⲤcomplexInUriⳆcomplexColInUriⳆprimitiveLiteralInJSONⳆprimitiveColInUriↃ_1.Parsed), _ⲤcomplexInUriⳆcomplexColInUriⳆprimitiveLiteralInJSONⳆprimitiveColInUriↃ_1.Remainder);
            }
        }
    }
    
}
