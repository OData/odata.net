namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx4Dx75x6Cx74x69x50x6Fx6Cx79x67x6Fx6Ex28ʺParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx4Dx75x6Cx74x69x50x6Fx6Cx79x67x6Fx6Ex28ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx4Dx75x6Cx74x69x50x6Fx6Cx79x67x6Fx6Ex28ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx4Dx75x6Cx74x69x50x6Fx6Cx79x67x6Fx6Ex28ʺ> Parse(IInput<char>? input)
            {
                var _x4D_1 = __GeneratedOdataV4.Parsers.Inners._x4DParser.Instance.Parse(input);
if (!_x4D_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx4Dx75x6Cx74x69x50x6Fx6Cx79x67x6Fx6Ex28ʺ)!, input);
}

var _x75_1 = __GeneratedOdataV4.Parsers.Inners._x75Parser.Instance.Parse(_x4D_1.Remainder);
if (!_x75_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx4Dx75x6Cx74x69x50x6Fx6Cx79x67x6Fx6Ex28ʺ)!, input);
}

var _x6C_1 = __GeneratedOdataV4.Parsers.Inners._x6CParser.Instance.Parse(_x75_1.Remainder);
if (!_x6C_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx4Dx75x6Cx74x69x50x6Fx6Cx79x67x6Fx6Ex28ʺ)!, input);
}

var _x74_1 = __GeneratedOdataV4.Parsers.Inners._x74Parser.Instance.Parse(_x6C_1.Remainder);
if (!_x74_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx4Dx75x6Cx74x69x50x6Fx6Cx79x67x6Fx6Ex28ʺ)!, input);
}

var _x69_1 = __GeneratedOdataV4.Parsers.Inners._x69Parser.Instance.Parse(_x74_1.Remainder);
if (!_x69_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx4Dx75x6Cx74x69x50x6Fx6Cx79x67x6Fx6Ex28ʺ)!, input);
}

var _x50_1 = __GeneratedOdataV4.Parsers.Inners._x50Parser.Instance.Parse(_x69_1.Remainder);
if (!_x50_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx4Dx75x6Cx74x69x50x6Fx6Cx79x67x6Fx6Ex28ʺ)!, input);
}

var _x6F_1 = __GeneratedOdataV4.Parsers.Inners._x6FParser.Instance.Parse(_x50_1.Remainder);
if (!_x6F_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx4Dx75x6Cx74x69x50x6Fx6Cx79x67x6Fx6Ex28ʺ)!, input);
}

var _x6C_2 = __GeneratedOdataV4.Parsers.Inners._x6CParser.Instance.Parse(_x6F_1.Remainder);
if (!_x6C_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx4Dx75x6Cx74x69x50x6Fx6Cx79x67x6Fx6Ex28ʺ)!, input);
}

var _x79_1 = __GeneratedOdataV4.Parsers.Inners._x79Parser.Instance.Parse(_x6C_2.Remainder);
if (!_x79_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx4Dx75x6Cx74x69x50x6Fx6Cx79x67x6Fx6Ex28ʺ)!, input);
}

var _x67_1 = __GeneratedOdataV4.Parsers.Inners._x67Parser.Instance.Parse(_x79_1.Remainder);
if (!_x67_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx4Dx75x6Cx74x69x50x6Fx6Cx79x67x6Fx6Ex28ʺ)!, input);
}

var _x6F_2 = __GeneratedOdataV4.Parsers.Inners._x6FParser.Instance.Parse(_x67_1.Remainder);
if (!_x6F_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx4Dx75x6Cx74x69x50x6Fx6Cx79x67x6Fx6Ex28ʺ)!, input);
}

var _x6E_1 = __GeneratedOdataV4.Parsers.Inners._x6EParser.Instance.Parse(_x6F_2.Remainder);
if (!_x6E_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx4Dx75x6Cx74x69x50x6Fx6Cx79x67x6Fx6Ex28ʺ)!, input);
}

var _x28_1 = __GeneratedOdataV4.Parsers.Inners._x28Parser.Instance.Parse(_x6E_1.Remainder);
if (!_x28_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx4Dx75x6Cx74x69x50x6Fx6Cx79x67x6Fx6Ex28ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._ʺx4Dx75x6Cx74x69x50x6Fx6Cx79x67x6Fx6Ex28ʺ.Instance, _x28_1.Remainder);
            }
        }
    }
    
}
