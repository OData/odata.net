namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _nullValueParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._nullValue> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._nullValue>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._nullValue> Parse(IInput<char>? input)
            {
                var _ʺx6Ex75x6Cx6Cʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx6Ex75x6Cx6CʺParser.Instance.Parse(input);
if (!_ʺx6Ex75x6Cx6Cʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._nullValue)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Rules._nullValue.Instance, _ʺx6Ex75x6Cx6Cʺ_1.Remainder);
            }
        }
    }
    
}
