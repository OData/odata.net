namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _annotationIdentifierParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._annotationIdentifier> Instance { get; } = from _excludeOperator_1 in __GeneratedOdataV2.Parsers.Rules._excludeOperatorParser.Instance.Optional()
from _ⲤSTARⳆnamespace_ʺx2Eʺ_ⲤtermNameⳆSTARↃↃ_1 in __GeneratedOdataV2.Parsers.Inners._ⲤSTARⳆnamespace_ʺx2Eʺ_ⲤtermNameⳆSTARↃↃParser.Instance
from _ʺx23ʺ_odataIdentifier_1 in __GeneratedOdataV2.Parsers.Inners._ʺx23ʺ_odataIdentifierParser.Instance.Optional()
select new __GeneratedOdataV2.CstNodes.Rules._annotationIdentifier(_excludeOperator_1.GetOrElse(null), _ⲤSTARⳆnamespace_ʺx2Eʺ_ⲤtermNameⳆSTARↃↃ_1, _ʺx23ʺ_odataIdentifier_1.GetOrElse(null));
    }
    
}
