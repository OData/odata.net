namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx4Ex4Fx54ʺParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx4Ex4Fx54ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx4Ex4Fx54ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx4Ex4Fx54ʺ> Parse(IInput<char>? input)
            {
                var _x4E_1 = __GeneratedOdataV3.Parsers.Inners._x4EParser.Instance.Parse(input);
if (!_x4E_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx4Ex4Fx54ʺ)!, input);
}

var _x4F_1 = __GeneratedOdataV3.Parsers.Inners._x4FParser.Instance.Parse(_x4E_1.Remainder);
if (!_x4F_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx4Ex4Fx54ʺ)!, input);
}

var _x54_1 = __GeneratedOdataV3.Parsers.Inners._x54Parser.Instance.Parse(_x4F_1.Remainder);
if (!_x54_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx4Ex4Fx54ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._ʺx4Ex4Fx54ʺ.Instance, _x54_1.Remainder);
            }
        }
    }
    
}
