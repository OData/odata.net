namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx4Ex61x4EʺParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx4Ex61x4Eʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx4Ex61x4Eʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx4Ex61x4Eʺ> Parse(IInput<char>? input)
            {
                var _x4E_1 = __GeneratedOdataV3.Parsers.Inners._x4EParser.Instance.Parse(input);
if (!_x4E_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx4Ex61x4Eʺ)!, input);
}

var _x61_1 = __GeneratedOdataV3.Parsers.Inners._x61Parser.Instance.Parse(_x4E_1.Remainder);
if (!_x61_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx4Ex61x4Eʺ)!, input);
}

var _x4E_2 = __GeneratedOdataV3.Parsers.Inners._x4EParser.Instance.Parse(_x61_1.Remainder);
if (!_x4E_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx4Ex61x4Eʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._ʺx4Ex61x4Eʺ.Instance, _x4E_2.Remainder);
            }
        }
    }
    
}
