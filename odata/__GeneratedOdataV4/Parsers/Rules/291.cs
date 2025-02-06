namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _dateValueParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._dateValue> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._dateValue>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._dateValue> Parse(IInput<char>? input)
            {
                var _year_1 = __GeneratedOdataV4.Parsers.Rules._yearParser.Instance.Parse(input);
if (!_year_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._dateValue)!, input);
}

var _ʺx2Dʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx2DʺParser.Instance.Parse(_year_1.Remainder);
if (!_ʺx2Dʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._dateValue)!, input);
}

var _month_1 = __GeneratedOdataV4.Parsers.Rules._monthParser.Instance.Parse(_ʺx2Dʺ_1.Remainder);
if (!_month_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._dateValue)!, input);
}

var _ʺx2Dʺ_2 = __GeneratedOdataV4.Parsers.Inners._ʺx2DʺParser.Instance.Parse(_month_1.Remainder);
if (!_ʺx2Dʺ_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._dateValue)!, input);
}

var _day_1 = __GeneratedOdataV4.Parsers.Rules._dayParser.Instance.Parse(_ʺx2Dʺ_2.Remainder);
if (!_day_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._dateValue)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._dateValue(_year_1.Parsed, _ʺx2Dʺ_1.Parsed, _month_1.Parsed, _ʺx2Dʺ_2.Parsed, _day_1.Parsed), _day_1.Remainder);
            }
        }
    }
    
}
