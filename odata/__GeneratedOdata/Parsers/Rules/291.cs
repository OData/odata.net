namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _dateValueParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._dateValue> Instance { get; } = from _year_1 in __GeneratedOdata.Parsers.Rules._yearParser.Instance
from _ʺx2Dʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx2DʺParser.Instance
from _month_1 in __GeneratedOdata.Parsers.Rules._monthParser.Instance
from _ʺx2Dʺ_2 in __GeneratedOdata.Parsers.Inners._ʺx2DʺParser.Instance
from _day_1 in __GeneratedOdata.Parsers.Rules._dayParser.Instance
select new __GeneratedOdata.CstNodes.Rules._dateValue(_year_1, _ʺx2Dʺ_1, _month_1, _ʺx2Dʺ_2, _day_1);
    }
    
}
