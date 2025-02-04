namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _annotationParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._annotation> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._annotation>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._annotation> Parse(IInput<char>? input)
            {
                var _AT_1 = __GeneratedOdataV3.Parsers.Rules._ATParser.Instance.Parse(input);
if (!_AT_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._annotation)!, input);
}

var _namespace_ʺx2Eʺ_1 = __GeneratedOdataV3.Parsers.Inners._namespace_ʺx2EʺParser.Instance.Optional().Parse(_AT_1.Remainder);
if (!_namespace_ʺx2Eʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._annotation)!, input);
}

var _termName_1 = __GeneratedOdataV3.Parsers.Rules._termNameParser.Instance.Parse(_namespace_ʺx2Eʺ_1.Remainder);
if (!_termName_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._annotation)!, input);
}

var _ʺx23ʺ_annotationQualifier_1 = __GeneratedOdataV3.Parsers.Inners._ʺx23ʺ_annotationQualifierParser.Instance.Optional().Parse(_termName_1.Remainder);
if (!_ʺx23ʺ_annotationQualifier_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._annotation)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._annotation(_AT_1.Parsed, _namespace_ʺx2Eʺ_1.Parsed.GetOrElse(null), _termName_1.Parsed, _ʺx23ʺ_annotationQualifier_1.Parsed.GetOrElse(null)), _ʺx23ʺ_annotationQualifier_1.Remainder);
            }
        }
    }
    
}
