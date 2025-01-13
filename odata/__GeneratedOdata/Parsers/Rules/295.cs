namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _timeOfDayValueParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._timeOfDayValue> Instance { get; } = from _hour_1 in __GeneratedOdata.Parsers.Rules._hourParser.Instance
from _ʺx3Aʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx3AʺParser.Instance
from _minute_1 in __GeneratedOdata.Parsers.Rules._minuteParser.Instance
from _ʺx3Aʺ_second_꘡ʺx2Eʺ_fractionalSeconds꘡_1 in __GeneratedOdata.Parsers.Inners._ʺx3Aʺ_second_꘡ʺx2Eʺ_fractionalSeconds꘡Parser.Instance.Optional()
select new __GeneratedOdata.CstNodes.Rules._timeOfDayValue(_hour_1, _ʺx3Aʺ_1, _minute_1, _ʺx3Aʺ_second_꘡ʺx2Eʺ_fractionalSeconds꘡_1.GetOrElse(null));
    }
    
}
