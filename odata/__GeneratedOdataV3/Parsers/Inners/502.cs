namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _Ⲥʺx5AʺⳆSIGN_hour_ʺx3Aʺ_minuteↃParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._Ⲥʺx5AʺⳆSIGN_hour_ʺx3Aʺ_minuteↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._Ⲥʺx5AʺⳆSIGN_hour_ʺx3Aʺ_minuteↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._Ⲥʺx5AʺⳆSIGN_hour_ʺx3Aʺ_minuteↃ> Parse(IInput<char>? input)
            {
                var _ʺx5AʺⳆSIGN_hour_ʺx3Aʺ_minute_1 = __GeneratedOdataV3.Parsers.Inners._ʺx5AʺⳆSIGN_hour_ʺx3Aʺ_minuteParser.Instance.Parse(input);
if (!_ʺx5AʺⳆSIGN_hour_ʺx3Aʺ_minute_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._Ⲥʺx5AʺⳆSIGN_hour_ʺx3Aʺ_minuteↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._Ⲥʺx5AʺⳆSIGN_hour_ʺx3Aʺ_minuteↃ(_ʺx5AʺⳆSIGN_hour_ʺx3Aʺ_minute_1.Parsed), _ʺx5AʺⳆSIGN_hour_ʺx3Aʺ_minute_1.Remainder);
            }
        }
    }
    
}
