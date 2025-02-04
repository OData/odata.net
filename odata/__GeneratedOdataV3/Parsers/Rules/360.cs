namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _annotationIdentifierParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._annotationIdentifier> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._annotationIdentifier>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._annotationIdentifier> Parse(IInput<char>? input)
            {
                var _excludeOperator_1 = __GeneratedOdataV3.Parsers.Rules._excludeOperatorParser.Instance.Optional().Parse(input);
if (!_excludeOperator_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._annotationIdentifier)!, input);
}

var _ⲤSTARⳆnamespace_ʺx2Eʺ_ⲤtermNameⳆSTARↃↃ_1 = __GeneratedOdataV3.Parsers.Inners._ⲤSTARⳆnamespace_ʺx2Eʺ_ⲤtermNameⳆSTARↃↃParser.Instance.Parse(_excludeOperator_1.Remainder);
if (!_ⲤSTARⳆnamespace_ʺx2Eʺ_ⲤtermNameⳆSTARↃↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._annotationIdentifier)!, input);
}

var _ʺx23ʺ_odataIdentifier_1 = __GeneratedOdataV3.Parsers.Inners._ʺx23ʺ_odataIdentifierParser.Instance.Optional().Parse(_ⲤSTARⳆnamespace_ʺx2Eʺ_ⲤtermNameⳆSTARↃↃ_1.Remainder);
if (!_ʺx23ʺ_odataIdentifier_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._annotationIdentifier)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._annotationIdentifier(_excludeOperator_1.Parsed.GetOrElse(null), _ⲤSTARⳆnamespace_ʺx2Eʺ_ⲤtermNameⳆSTARↃↃ_1.Parsed, _ʺx23ʺ_odataIdentifier_1.Parsed.GetOrElse(null)), _ʺx23ʺ_odataIdentifier_1.Remainder);
            }
        }
    }
    
}
