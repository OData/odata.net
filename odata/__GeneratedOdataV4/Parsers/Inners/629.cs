namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ⲤunreservedⳆpctⲻencodedⳆsubⲻdelimsⳆʺx3AʺↃParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ⲤunreservedⳆpctⲻencodedⳆsubⲻdelimsⳆʺx3AʺↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ⲤunreservedⳆpctⲻencodedⳆsubⲻdelimsⳆʺx3AʺↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ⲤunreservedⳆpctⲻencodedⳆsubⲻdelimsⳆʺx3AʺↃ> Parse(IInput<char>? input)
            {
                var _unreservedⳆpctⲻencodedⳆsubⲻdelimsⳆʺx3Aʺ_1 = __GeneratedOdataV4.Parsers.Inners._unreservedⳆpctⲻencodedⳆsubⲻdelimsⳆʺx3AʺParser.Instance.Parse(input);
if (!_unreservedⳆpctⲻencodedⳆsubⲻdelimsⳆʺx3Aʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ⲤunreservedⳆpctⲻencodedⳆsubⲻdelimsⳆʺx3AʺↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._ⲤunreservedⳆpctⲻencodedⳆsubⲻdelimsⳆʺx3AʺↃ(_unreservedⳆpctⲻencodedⳆsubⲻdelimsⳆʺx3Aʺ_1.Parsed), _unreservedⳆpctⲻencodedⳆsubⲻdelimsⳆʺx3Aʺ_1.Remainder);
            }
        }
    }
    
}
