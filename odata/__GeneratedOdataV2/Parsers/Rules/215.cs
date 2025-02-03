namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _stringInJSONParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._stringInJSON> Instance { get; } = from _quotationⲻmark_1 in __GeneratedOdataV2.Parsers.Rules._quotationⲻmarkParser.Instance
from _charInJSON_1 in __GeneratedOdataV2.Parsers.Rules._charInJSONParser.Instance.Many()
from _quotationⲻmark_2 in __GeneratedOdataV2.Parsers.Rules._quotationⲻmarkParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._stringInJSON(_quotationⲻmark_1, _charInJSON_1, _quotationⲻmark_2);
    }
    
}
