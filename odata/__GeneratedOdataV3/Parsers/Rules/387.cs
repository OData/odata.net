namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _userinfoParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._userinfo> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._userinfo>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._userinfo> Parse(IInput<char>? input)
            {
                var _ⲤunreservedⳆpctⲻencodedⳆsubⲻdelimsⳆʺx3AʺↃ_1 = Inners._ⲤunreservedⳆpctⲻencodedⳆsubⲻdelimsⳆʺx3AʺↃParser.Instance.Many().Parse(input);
if (!_ⲤunreservedⳆpctⲻencodedⳆsubⲻdelimsⳆʺx3AʺↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._userinfo)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._userinfo(_ⲤunreservedⳆpctⲻencodedⳆsubⲻdelimsⳆʺx3AʺↃ_1.Parsed), _ⲤunreservedⳆpctⲻencodedⳆsubⲻdelimsⳆʺx3AʺↃ_1.Remainder);
            }
        }
    }
    
}
