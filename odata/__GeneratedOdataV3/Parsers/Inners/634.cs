namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ⲤunreservedⳆsubⲻdelimsⳆʺx3AʺↃParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ⲤunreservedⳆsubⲻdelimsⳆʺx3AʺↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ⲤunreservedⳆsubⲻdelimsⳆʺx3AʺↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ⲤunreservedⳆsubⲻdelimsⳆʺx3AʺↃ> Parse(IInput<char>? input)
            {
                var _unreservedⳆsubⲻdelimsⳆʺx3Aʺ_1 = __GeneratedOdataV3.Parsers.Inners._unreservedⳆsubⲻdelimsⳆʺx3AʺParser.Instance.Parse(input);
if (!_unreservedⳆsubⲻdelimsⳆʺx3Aʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ⲤunreservedⳆsubⲻdelimsⳆʺx3AʺↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._ⲤunreservedⳆsubⲻdelimsⳆʺx3AʺↃ(_unreservedⳆsubⲻdelimsⳆʺx3Aʺ_1.Parsed), _unreservedⳆsubⲻdelimsⳆʺx3Aʺ_1.Remainder);
            }
        }
    }
    
}
