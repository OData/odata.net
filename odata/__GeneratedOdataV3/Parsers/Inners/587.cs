namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx6Dx61x78x70x61x67x65x73x69x7Ax65ʺParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx6Dx61x78x70x61x67x65x73x69x7Ax65ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx6Dx61x78x70x61x67x65x73x69x7Ax65ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx6Dx61x78x70x61x67x65x73x69x7Ax65ʺ> Parse(IInput<char>? input)
            {
                var _x6D_1 = __GeneratedOdataV3.Parsers.Inners._x6DParser.Instance.Parse(input);
if (!_x6D_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx6Dx61x78x70x61x67x65x73x69x7Ax65ʺ)!, input);
}

var _x61_1 = __GeneratedOdataV3.Parsers.Inners._x61Parser.Instance.Parse(_x6D_1.Remainder);
if (!_x61_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx6Dx61x78x70x61x67x65x73x69x7Ax65ʺ)!, input);
}

var _x78_1 = __GeneratedOdataV3.Parsers.Inners._x78Parser.Instance.Parse(_x61_1.Remainder);
if (!_x78_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx6Dx61x78x70x61x67x65x73x69x7Ax65ʺ)!, input);
}

var _x70_1 = __GeneratedOdataV3.Parsers.Inners._x70Parser.Instance.Parse(_x78_1.Remainder);
if (!_x70_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx6Dx61x78x70x61x67x65x73x69x7Ax65ʺ)!, input);
}

var _x61_2 = __GeneratedOdataV3.Parsers.Inners._x61Parser.Instance.Parse(_x70_1.Remainder);
if (!_x61_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx6Dx61x78x70x61x67x65x73x69x7Ax65ʺ)!, input);
}

var _x67_1 = __GeneratedOdataV3.Parsers.Inners._x67Parser.Instance.Parse(_x61_2.Remainder);
if (!_x67_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx6Dx61x78x70x61x67x65x73x69x7Ax65ʺ)!, input);
}

var _x65_1 = __GeneratedOdataV3.Parsers.Inners._x65Parser.Instance.Parse(_x67_1.Remainder);
if (!_x65_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx6Dx61x78x70x61x67x65x73x69x7Ax65ʺ)!, input);
}

var _x73_1 = __GeneratedOdataV3.Parsers.Inners._x73Parser.Instance.Parse(_x65_1.Remainder);
if (!_x73_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx6Dx61x78x70x61x67x65x73x69x7Ax65ʺ)!, input);
}

var _x69_1 = __GeneratedOdataV3.Parsers.Inners._x69Parser.Instance.Parse(_x73_1.Remainder);
if (!_x69_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx6Dx61x78x70x61x67x65x73x69x7Ax65ʺ)!, input);
}

var _x7A_1 = __GeneratedOdataV3.Parsers.Inners._x7AParser.Instance.Parse(_x69_1.Remainder);
if (!_x7A_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx6Dx61x78x70x61x67x65x73x69x7Ax65ʺ)!, input);
}

var _x65_2 = __GeneratedOdataV3.Parsers.Inners._x65Parser.Instance.Parse(_x7A_1.Remainder);
if (!_x65_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx6Dx61x78x70x61x67x65x73x69x7Ax65ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._ʺx6Dx61x78x70x61x67x65x73x69x7Ax65ʺ.Instance, _x65_2.Remainder);
            }
        }
    }
    
}
