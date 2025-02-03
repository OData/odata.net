namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx63x6Fx6Ex74x69x6Ex75x65x2Dx6Fx6Ex2Dx65x72x72x6Fx72ʺParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx63x6Fx6Ex74x69x6Ex75x65x2Dx6Fx6Ex2Dx65x72x72x6Fx72ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx63x6Fx6Ex74x69x6Ex75x65x2Dx6Fx6Ex2Dx65x72x72x6Fx72ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx63x6Fx6Ex74x69x6Ex75x65x2Dx6Fx6Ex2Dx65x72x72x6Fx72ʺ> Parse(IInput<char>? input)
            {
                var _x63_1 = __GeneratedOdataV3.Parsers.Inners._x63Parser.Instance.Parse(input);
if (!_x63_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx63x6Fx6Ex74x69x6Ex75x65x2Dx6Fx6Ex2Dx65x72x72x6Fx72ʺ)!, input);
}

var _x6F_1 = __GeneratedOdataV3.Parsers.Inners._x6FParser.Instance.Parse(_x63_1.Remainder);
if (!_x6F_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx63x6Fx6Ex74x69x6Ex75x65x2Dx6Fx6Ex2Dx65x72x72x6Fx72ʺ)!, input);
}

var _x6E_1 = __GeneratedOdataV3.Parsers.Inners._x6EParser.Instance.Parse(_x6F_1.Remainder);
if (!_x6E_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx63x6Fx6Ex74x69x6Ex75x65x2Dx6Fx6Ex2Dx65x72x72x6Fx72ʺ)!, input);
}

var _x74_1 = __GeneratedOdataV3.Parsers.Inners._x74Parser.Instance.Parse(_x6E_1.Remainder);
if (!_x74_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx63x6Fx6Ex74x69x6Ex75x65x2Dx6Fx6Ex2Dx65x72x72x6Fx72ʺ)!, input);
}

var _x69_1 = __GeneratedOdataV3.Parsers.Inners._x69Parser.Instance.Parse(_x74_1.Remainder);
if (!_x69_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx63x6Fx6Ex74x69x6Ex75x65x2Dx6Fx6Ex2Dx65x72x72x6Fx72ʺ)!, input);
}

var _x6E_2 = __GeneratedOdataV3.Parsers.Inners._x6EParser.Instance.Parse(_x69_1.Remainder);
if (!_x6E_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx63x6Fx6Ex74x69x6Ex75x65x2Dx6Fx6Ex2Dx65x72x72x6Fx72ʺ)!, input);
}

var _x75_1 = __GeneratedOdataV3.Parsers.Inners._x75Parser.Instance.Parse(_x6E_2.Remainder);
if (!_x75_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx63x6Fx6Ex74x69x6Ex75x65x2Dx6Fx6Ex2Dx65x72x72x6Fx72ʺ)!, input);
}

var _x65_1 = __GeneratedOdataV3.Parsers.Inners._x65Parser.Instance.Parse(_x75_1.Remainder);
if (!_x65_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx63x6Fx6Ex74x69x6Ex75x65x2Dx6Fx6Ex2Dx65x72x72x6Fx72ʺ)!, input);
}

var _x2D_1 = __GeneratedOdataV3.Parsers.Inners._x2DParser.Instance.Parse(_x65_1.Remainder);
if (!_x2D_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx63x6Fx6Ex74x69x6Ex75x65x2Dx6Fx6Ex2Dx65x72x72x6Fx72ʺ)!, input);
}

var _x6F_2 = __GeneratedOdataV3.Parsers.Inners._x6FParser.Instance.Parse(_x2D_1.Remainder);
if (!_x6F_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx63x6Fx6Ex74x69x6Ex75x65x2Dx6Fx6Ex2Dx65x72x72x6Fx72ʺ)!, input);
}

var _x6E_3 = __GeneratedOdataV3.Parsers.Inners._x6EParser.Instance.Parse(_x6F_2.Remainder);
if (!_x6E_3.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx63x6Fx6Ex74x69x6Ex75x65x2Dx6Fx6Ex2Dx65x72x72x6Fx72ʺ)!, input);
}

var _x2D_2 = __GeneratedOdataV3.Parsers.Inners._x2DParser.Instance.Parse(_x6E_3.Remainder);
if (!_x2D_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx63x6Fx6Ex74x69x6Ex75x65x2Dx6Fx6Ex2Dx65x72x72x6Fx72ʺ)!, input);
}

var _x65_2 = __GeneratedOdataV3.Parsers.Inners._x65Parser.Instance.Parse(_x2D_2.Remainder);
if (!_x65_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx63x6Fx6Ex74x69x6Ex75x65x2Dx6Fx6Ex2Dx65x72x72x6Fx72ʺ)!, input);
}

var _x72_1 = __GeneratedOdataV3.Parsers.Inners._x72Parser.Instance.Parse(_x65_2.Remainder);
if (!_x72_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx63x6Fx6Ex74x69x6Ex75x65x2Dx6Fx6Ex2Dx65x72x72x6Fx72ʺ)!, input);
}

var _x72_2 = __GeneratedOdataV3.Parsers.Inners._x72Parser.Instance.Parse(_x72_1.Remainder);
if (!_x72_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx63x6Fx6Ex74x69x6Ex75x65x2Dx6Fx6Ex2Dx65x72x72x6Fx72ʺ)!, input);
}

var _x6F_3 = __GeneratedOdataV3.Parsers.Inners._x6FParser.Instance.Parse(_x72_2.Remainder);
if (!_x6F_3.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx63x6Fx6Ex74x69x6Ex75x65x2Dx6Fx6Ex2Dx65x72x72x6Fx72ʺ)!, input);
}

var _x72_3 = __GeneratedOdataV3.Parsers.Inners._x72Parser.Instance.Parse(_x6F_3.Remainder);
if (!_x72_3.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx63x6Fx6Ex74x69x6Ex75x65x2Dx6Fx6Ex2Dx65x72x72x6Fx72ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._ʺx63x6Fx6Ex74x69x6Ex75x65x2Dx6Fx6Ex2Dx65x72x72x6Fx72ʺ.Instance, _x72_3.Remainder);
            }
        }
    }
    
}
