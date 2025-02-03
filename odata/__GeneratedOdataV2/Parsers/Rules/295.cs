namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _timeOfDayValueParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._timeOfDayValue> Instance { get; } = from _hour_1 in __GeneratedOdataV2.Parsers.Rules._hourParser.Instance
from _ʺx3Aʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx3AʺParser.Instance
from _minute_1 in __GeneratedOdataV2.Parsers.Rules._minuteParser.Instance
from _ʺx3Aʺ_second_꘡ʺx2Eʺ_fractionalSeconds꘡_1 in __GeneratedOdataV2.Parsers.Inners._ʺx3Aʺ_second_꘡ʺx2Eʺ_fractionalSeconds꘡Parser.Instance.Optional()
select new __GeneratedOdataV2.CstNodes.Rules._timeOfDayValue(_hour_1, _ʺx3Aʺ_1, _minute_1, _ʺx3Aʺ_second_꘡ʺx2Eʺ_fractionalSeconds꘡_1.GetOrElse(null));
    }
    
}
