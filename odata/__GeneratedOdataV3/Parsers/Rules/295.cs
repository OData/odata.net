namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _timeOfDayValueParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._timeOfDayValue> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._timeOfDayValue>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._timeOfDayValue> Parse(IInput<char>? input)
            {
                var _hour_1 = __GeneratedOdataV3.Parsers.Rules._hourParser.Instance.Parse(input);
if (!_hour_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._timeOfDayValue)!, input);
}

var _ʺx3Aʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx3AʺParser.Instance.Parse(_hour_1.Remainder);
if (!_ʺx3Aʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._timeOfDayValue)!, input);
}

var _minute_1 = __GeneratedOdataV3.Parsers.Rules._minuteParser.Instance.Parse(_ʺx3Aʺ_1.Remainder);
if (!_minute_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._timeOfDayValue)!, input);
}

var _ʺx3Aʺ_second_꘡ʺx2Eʺ_fractionalSeconds꘡_1 = __GeneratedOdataV3.Parsers.Inners._ʺx3Aʺ_second_꘡ʺx2Eʺ_fractionalSeconds꘡Parser.Instance.Optional().Parse(_minute_1.Remainder);
if (!_ʺx3Aʺ_second_꘡ʺx2Eʺ_fractionalSeconds꘡_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._timeOfDayValue)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._timeOfDayValue(_hour_1.Parsed, _ʺx3Aʺ_1.Parsed, _minute_1.Parsed, _ʺx3Aʺ_second_꘡ʺx2Eʺ_fractionalSeconds꘡_1.Parsed.GetOrElse(null)), _ʺx3Aʺ_second_꘡ʺx2Eʺ_fractionalSeconds꘡_1.Remainder);
            }
        }
    }
    
}
