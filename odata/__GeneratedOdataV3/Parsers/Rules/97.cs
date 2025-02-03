namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _deltatokenParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._deltatoken> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._deltatoken>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._deltatoken> Parse(IInput<char>? input)
            {
                var _ʺx24x64x65x6Cx74x61x74x6Fx6Bx65x6Eʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx24x64x65x6Cx74x61x74x6Fx6Bx65x6EʺParser.Instance.Parse(input);
if (!_ʺx24x64x65x6Cx74x61x74x6Fx6Bx65x6Eʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._deltatoken)!, input);
}

var _EQ_1 = __GeneratedOdataV3.Parsers.Rules._EQParser.Instance.Parse(_ʺx24x64x65x6Cx74x61x74x6Fx6Bx65x6Eʺ_1.Remainder);
if (!_EQ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._deltatoken)!, input);
}

var _ⲤqcharⲻnoⲻAMPↃ_1 = __GeneratedOdataV3.Parsers.Inners._ⲤqcharⲻnoⲻAMPↃParser.Instance.Repeat(1, null).Parse(_EQ_1.Remainder);
if (!_ⲤqcharⲻnoⲻAMPↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._deltatoken)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._deltatoken(_ʺx24x64x65x6Cx74x61x74x6Fx6Bx65x6Eʺ_1.Parsed, _EQ_1.Parsed,  new __GeneratedOdataV3.CstNodes.Inners.HelperRangedAtLeast1<__GeneratedOdataV3.CstNodes.Inners._ⲤqcharⲻnoⲻAMPↃ>(_ⲤqcharⲻnoⲻAMPↃ_1.Parsed)), _ⲤqcharⲻnoⲻAMPↃ_1.Remainder);
            }
        }
    }
    
}
