namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _contextParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._context> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._context>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._context> Parse(IInput<char>? input)
            {
                var _ʺx23ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx23ʺParser.Instance.Parse(input);
if (!_ʺx23ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._context)!, input);
}

var _contextFragment_1 = __GeneratedOdataV3.Parsers.Rules._contextFragmentParser.Instance.Parse(_ʺx23ʺ_1.Remainder);
if (!_contextFragment_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._context)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._context(_ʺx23ʺ_1.Parsed,  _contextFragment_1.Parsed), _contextFragment_1.Remainder);
            }
        }
    }
    
}
