namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx23ʺ_annotationQualifierParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx23ʺ_annotationQualifier> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx23ʺ_annotationQualifier>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx23ʺ_annotationQualifier> Parse(IInput<char>? input)
            {
                var _ʺx23ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx23ʺParser.Instance.Parse(input);
if (!_ʺx23ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx23ʺ_annotationQualifier)!, input);
}

var _annotationQualifier_1 = __GeneratedOdataV3.Parsers.Rules._annotationQualifierParser.Instance.Parse(_ʺx23ʺ_1.Remainder);
if (!_annotationQualifier_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx23ʺ_annotationQualifier)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._ʺx23ʺ_annotationQualifier(_ʺx23ʺ_1.Parsed,  _annotationQualifier_1.Parsed), _annotationQualifier_1.Remainder);
            }
        }
    }
    
}
