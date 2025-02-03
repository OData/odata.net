namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _dateTimeOffsetValueParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._dateTimeOffsetValue> Instance { get; } = from _year_1 in __GeneratedOdataV2.Parsers.Rules._yearParser.Instance
from _ʺx2Dʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx2DʺParser.Instance
from _month_1 in __GeneratedOdataV2.Parsers.Rules._monthParser.Instance
from _ʺx2Dʺ_2 in __GeneratedOdataV2.Parsers.Inners._ʺx2DʺParser.Instance
from _day_1 in __GeneratedOdataV2.Parsers.Rules._dayParser.Instance
from _ʺx54ʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx54ʺParser.Instance
from _hour_1 in __GeneratedOdataV2.Parsers.Rules._hourParser.Instance
from _ʺx3Aʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx3AʺParser.Instance
from _minute_1 in __GeneratedOdataV2.Parsers.Rules._minuteParser.Instance
from _ʺx3Aʺ_second_꘡ʺx2Eʺ_fractionalSeconds꘡_1 in __GeneratedOdataV2.Parsers.Inners._ʺx3Aʺ_second_꘡ʺx2Eʺ_fractionalSeconds꘡Parser.Instance.Optional()
from _Ⲥʺx5AʺⳆSIGN_hour_ʺx3Aʺ_minuteↃ_1 in __GeneratedOdataV2.Parsers.Inners._Ⲥʺx5AʺⳆSIGN_hour_ʺx3Aʺ_minuteↃParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._dateTimeOffsetValue(_year_1, _ʺx2Dʺ_1, _month_1, _ʺx2Dʺ_2, _day_1, _ʺx54ʺ_1, _hour_1, _ʺx3Aʺ_1, _minute_1, _ʺx3Aʺ_second_꘡ʺx2Eʺ_fractionalSeconds꘡_1.GetOrElse(null), _Ⲥʺx5AʺⳆSIGN_hour_ʺx3Aʺ_minuteↃ_1);
    }
    
}
