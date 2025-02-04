namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _dateTimeOffsetValueParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._dateTimeOffsetValue> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._dateTimeOffsetValue>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._dateTimeOffsetValue> Parse(IInput<char>? input)
            {
                var _year_1 = __GeneratedOdataV3.Parsers.Rules._yearParser.Instance.Parse(input);
if (!_year_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._dateTimeOffsetValue)!, input);
}

var _ʺx2Dʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx2DʺParser.Instance.Parse(_year_1.Remainder);
if (!_ʺx2Dʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._dateTimeOffsetValue)!, input);
}

var _month_1 = __GeneratedOdataV3.Parsers.Rules._monthParser.Instance.Parse(_ʺx2Dʺ_1.Remainder);
if (!_month_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._dateTimeOffsetValue)!, input);
}

var _ʺx2Dʺ_2 = __GeneratedOdataV3.Parsers.Inners._ʺx2DʺParser.Instance.Parse(_month_1.Remainder);
if (!_ʺx2Dʺ_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._dateTimeOffsetValue)!, input);
}

var _day_1 = __GeneratedOdataV3.Parsers.Rules._dayParser.Instance.Parse(_ʺx2Dʺ_2.Remainder);
if (!_day_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._dateTimeOffsetValue)!, input);
}

var _ʺx54ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx54ʺParser.Instance.Parse(_day_1.Remainder);
if (!_ʺx54ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._dateTimeOffsetValue)!, input);
}

var _hour_1 = __GeneratedOdataV3.Parsers.Rules._hourParser.Instance.Parse(_ʺx54ʺ_1.Remainder);
if (!_hour_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._dateTimeOffsetValue)!, input);
}

var _ʺx3Aʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx3AʺParser.Instance.Parse(_hour_1.Remainder);
if (!_ʺx3Aʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._dateTimeOffsetValue)!, input);
}

var _minute_1 = __GeneratedOdataV3.Parsers.Rules._minuteParser.Instance.Parse(_ʺx3Aʺ_1.Remainder);
if (!_minute_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._dateTimeOffsetValue)!, input);
}

var _ʺx3Aʺ_second_꘡ʺx2Eʺ_fractionalSeconds꘡_1 = __GeneratedOdataV3.Parsers.Inners._ʺx3Aʺ_second_꘡ʺx2Eʺ_fractionalSeconds꘡Parser.Instance.Optional().Parse(_minute_1.Remainder);
if (!_ʺx3Aʺ_second_꘡ʺx2Eʺ_fractionalSeconds꘡_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._dateTimeOffsetValue)!, input);
}

var _Ⲥʺx5AʺⳆSIGN_hour_ʺx3Aʺ_minuteↃ_1 = __GeneratedOdataV3.Parsers.Inners._Ⲥʺx5AʺⳆSIGN_hour_ʺx3Aʺ_minuteↃParser.Instance.Parse(_ʺx3Aʺ_second_꘡ʺx2Eʺ_fractionalSeconds꘡_1.Remainder);
if (!_Ⲥʺx5AʺⳆSIGN_hour_ʺx3Aʺ_minuteↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._dateTimeOffsetValue)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._dateTimeOffsetValue(_year_1.Parsed, _ʺx2Dʺ_1.Parsed, _month_1.Parsed, _ʺx2Dʺ_2.Parsed, _day_1.Parsed, _ʺx54ʺ_1.Parsed, _hour_1.Parsed, _ʺx3Aʺ_1.Parsed, _minute_1.Parsed, _ʺx3Aʺ_second_꘡ʺx2Eʺ_fractionalSeconds꘡_1.Parsed.GetOrElse(null), _Ⲥʺx5AʺⳆSIGN_hour_ʺx3Aʺ_minuteↃ_1.Parsed), _Ⲥʺx5AʺⳆSIGN_hour_ʺx3Aʺ_minuteↃ_1.Remainder);
            }
        }
    }
    
}
