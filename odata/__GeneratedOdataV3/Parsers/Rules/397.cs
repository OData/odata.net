namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _regⲻnameParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._regⲻname> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._regⲻname>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._regⲻname> Parse(IInput<char>? input)
            {
                var _ⲤunreservedⳆpctⲻencodedⳆsubⲻdelimsↃ_1 = Inners._ⲤunreservedⳆpctⲻencodedⳆsubⲻdelimsↃParser.Instance.Many().Parse(input);
if (!_ⲤunreservedⳆpctⲻencodedⳆsubⲻdelimsↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._regⲻname)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._regⲻname(_ⲤunreservedⳆpctⲻencodedⳆsubⲻdelimsↃ_1.Parsed), _ⲤunreservedⳆpctⲻencodedⳆsubⲻdelimsↃ_1.Remainder);
            }
        }
    }
    
}
