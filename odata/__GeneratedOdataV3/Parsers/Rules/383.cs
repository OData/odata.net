namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _URIParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._URI> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._URI>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._URI> Parse(IInput<char>? input)
            {
                var _scheme_1 = __GeneratedOdataV3.Parsers.Rules._schemeParser.Instance.Parse(input);
if (!_scheme_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._URI)!, input);
}

var _ʺx3Aʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx3AʺParser.Instance.Parse(_scheme_1.Remainder);
if (!_ʺx3Aʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._URI)!, input);
}

var _hierⲻpart_1 = __GeneratedOdataV3.Parsers.Rules._hierⲻpartParser.Instance.Parse(_ʺx3Aʺ_1.Remainder);
if (!_hierⲻpart_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._URI)!, input);
}

var _ʺx3Fʺ_query_1 = __GeneratedOdataV3.Parsers.Inners._ʺx3Fʺ_queryParser.Instance.Optional().Parse(_hierⲻpart_1.Remainder);
if (!_ʺx3Fʺ_query_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._URI)!, input);
}

var _ʺx23ʺ_fragment_1 = __GeneratedOdataV3.Parsers.Inners._ʺx23ʺ_fragmentParser.Instance.Optional().Parse(_ʺx3Fʺ_query_1.Remainder);
if (!_ʺx23ʺ_fragment_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._URI)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._URI(_scheme_1.Parsed, _ʺx3Aʺ_1.Parsed, _hierⲻpart_1.Parsed, _ʺx3Fʺ_query_1.Parsed.GetOrElse(null), _ʺx23ʺ_fragment_1.Parsed.GetOrElse(null)), _ʺx23ʺ_fragment_1.Remainder);
            }
        }
    }
    
}
