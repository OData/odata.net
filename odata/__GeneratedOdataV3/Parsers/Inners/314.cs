namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx6Ex6Fx77ʺParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx6Ex6Fx77ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx6Ex6Fx77ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx6Ex6Fx77ʺ> Parse(IInput<char>? input)
            {
                var _x6E_1 = __GeneratedOdataV3.Parsers.Inners._x6EParser.Instance.Parse(input);
if (!_x6E_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx6Ex6Fx77ʺ)!, input);
}

var _x6F_1 = __GeneratedOdataV3.Parsers.Inners._x6FParser.Instance.Parse(_x6E_1.Remainder);
if (!_x6F_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx6Ex6Fx77ʺ)!, input);
}

var _x77_1 = __GeneratedOdataV3.Parsers.Inners._x77Parser.Instance.Parse(_x6F_1.Remainder);
if (!_x77_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx6Ex6Fx77ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._ʺx6Ex6Fx77ʺ.Instance, _x77_1.Remainder);
            }
        }
    }
    
}
