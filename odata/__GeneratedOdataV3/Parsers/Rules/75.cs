namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _topParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._top> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._top>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._top> Parse(IInput<char>? input)
            {
                var _Ⲥʺx24x74x6Fx70ʺⳆʺx74x6Fx70ʺↃ_1 = __GeneratedOdataV3.Parsers.Inners._Ⲥʺx24x74x6Fx70ʺⳆʺx74x6Fx70ʺↃParser.Instance.Parse(input);
if (!_Ⲥʺx24x74x6Fx70ʺⳆʺx74x6Fx70ʺↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._top)!, input);
}

var _EQ_1 = __GeneratedOdataV3.Parsers.Rules._EQParser.Instance.Parse(_Ⲥʺx24x74x6Fx70ʺⳆʺx74x6Fx70ʺↃ_1.Remainder);
if (!_EQ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._top)!, input);
}

var _DIGIT_1 = __GeneratedOdataV3.Parsers.Rules._DIGITParser.Instance.Repeat(1, null).Parse(_EQ_1.Remainder);
if (!_DIGIT_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._top)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._top(_Ⲥʺx24x74x6Fx70ʺⳆʺx74x6Fx70ʺↃ_1.Parsed, _EQ_1.Parsed,  new __GeneratedOdataV3.CstNodes.Inners.HelperRangedAtLeast1<__GeneratedOdataV3.CstNodes.Rules._DIGIT>(_DIGIT_1.Parsed)), _DIGIT_1.Remainder);
            }
        }
    }
    
}
