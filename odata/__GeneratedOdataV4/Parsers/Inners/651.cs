namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ⲤunreservedⳆpctⲻencodedⳆsubⲻdelimsↃParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ⲤunreservedⳆpctⲻencodedⳆsubⲻdelimsↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ⲤunreservedⳆpctⲻencodedⳆsubⲻdelimsↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ⲤunreservedⳆpctⲻencodedⳆsubⲻdelimsↃ> Parse(IInput<char>? input)
            {
                var _unreservedⳆpctⲻencodedⳆsubⲻdelims_1 = __GeneratedOdataV4.Parsers.Inners._unreservedⳆpctⲻencodedⳆsubⲻdelimsParser.Instance.Parse(input);
if (!_unreservedⳆpctⲻencodedⳆsubⲻdelims_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ⲤunreservedⳆpctⲻencodedⳆsubⲻdelimsↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._ⲤunreservedⳆpctⲻencodedⳆsubⲻdelimsↃ(_unreservedⳆpctⲻencodedⳆsubⲻdelims_1.Parsed), _unreservedⳆpctⲻencodedⳆsubⲻdelims_1.Remainder);
            }
        }
    }
    
}
