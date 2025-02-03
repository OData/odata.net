namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _userinfo_ʺx40ʺParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._userinfo_ʺx40ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._userinfo_ʺx40ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._userinfo_ʺx40ʺ> Parse(IInput<char>? input)
            {
                var _userinfo_1 = __GeneratedOdataV3.Parsers.Rules._userinfoParser.Instance.Parse(input);
if (!_userinfo_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._userinfo_ʺx40ʺ)!, input);
}

var _ʺx40ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx40ʺParser.Instance.Parse(_userinfo_1.Remainder);
if (!_ʺx40ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._userinfo_ʺx40ʺ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._userinfo_ʺx40ʺ(_userinfo_1.Parsed,  _ʺx40ʺ_1.Parsed), _ʺx40ʺ_1.Remainder);
            }
        }
    }
    
}
