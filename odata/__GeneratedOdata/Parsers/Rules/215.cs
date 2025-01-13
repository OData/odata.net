namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _stringInJSONParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._stringInJSON> Instance { get; } = from _quotationⲻmark_1 in __GeneratedOdata.Parsers.Rules._quotationⲻmarkParser.Instance
from _charInJSON_1 in __GeneratedOdata.Parsers.Rules._charInJSONParser.Instance.Many()
from _quotationⲻmark_2 in __GeneratedOdata.Parsers.Rules._quotationⲻmarkParser.Instance
select new __GeneratedOdata.CstNodes.Rules._stringInJSON(_quotationⲻmark_1, _charInJSON_1, _quotationⲻmark_2);
    }
    
}
