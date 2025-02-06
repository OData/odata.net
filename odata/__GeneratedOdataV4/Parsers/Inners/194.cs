namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx41x4Ex44ʺParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx41x4Ex44ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx41x4Ex44ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx41x4Ex44ʺ> Parse(IInput<char>? input)
            {
                var _x41_1 = __GeneratedOdataV4.Parsers.Inners._x41Parser.Instance.Parse(input);
if (!_x41_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx41x4Ex44ʺ)!, input);
}

var _x4E_1 = __GeneratedOdataV4.Parsers.Inners._x4EParser.Instance.Parse(_x41_1.Remainder);
if (!_x4E_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx41x4Ex44ʺ)!, input);
}

var _x44_1 = __GeneratedOdataV4.Parsers.Inners._x44Parser.Instance.Parse(_x4E_1.Remainder);
if (!_x44_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx41x4Ex44ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._ʺx41x4Ex44ʺ.Instance, _x44_1.Remainder);
            }
        }
    }
    
}
