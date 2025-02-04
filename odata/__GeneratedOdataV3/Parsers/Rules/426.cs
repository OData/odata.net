namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _HTABParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._HTAB> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._HTAB>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._HTAB> Parse(IInput<char>? input)
            {
                var _Ⰳx09_1 = __GeneratedOdataV3.Parsers.Inners._Ⰳx09Parser.Instance.Parse(input);
if (!_Ⰳx09_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._HTAB)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Rules._HTAB.Instance, _Ⰳx09_1.Remainder);
            }
        }
    }
    
}
