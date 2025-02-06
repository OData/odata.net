namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx67x74ʺParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx67x74ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx67x74ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx67x74ʺ> Parse(IInput<char>? input)
            {
                var _x67_1 = __GeneratedOdataV4.Parsers.Inners._x67Parser.Instance.Parse(input);
if (!_x67_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx67x74ʺ)!, input);
}

var _x74_1 = __GeneratedOdataV4.Parsers.Inners._x74Parser.Instance.Parse(_x67_1.Remainder);
if (!_x74_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx67x74ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._ʺx67x74ʺ.Instance, _x74_1.Remainder);
            }
        }
    }
    
}
