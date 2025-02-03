namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx2Dx49x4Ex46ʺParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx2Dx49x4Ex46ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx2Dx49x4Ex46ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx2Dx49x4Ex46ʺ> Parse(IInput<char>? input)
            {
                var _x2D_1 = __GeneratedOdataV3.Parsers.Inners._x2DParser.Instance.Parse(input);
if (!_x2D_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx2Dx49x4Ex46ʺ)!, input);
}

var _x49_1 = __GeneratedOdataV3.Parsers.Inners._x49Parser.Instance.Parse(_x2D_1.Remainder);
if (!_x49_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx2Dx49x4Ex46ʺ)!, input);
}

var _x4E_1 = __GeneratedOdataV3.Parsers.Inners._x4EParser.Instance.Parse(_x49_1.Remainder);
if (!_x4E_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx2Dx49x4Ex46ʺ)!, input);
}

var _x46_1 = __GeneratedOdataV3.Parsers.Inners._x46Parser.Instance.Parse(_x4E_1.Remainder);
if (!_x46_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx2Dx49x4Ex46ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._ʺx2Dx49x4Ex46ʺ.Instance, _x46_1.Remainder);
            }
        }
    }
    
}
