namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx54x69x6Dx65x4Fx66x44x61x79ʺParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx54x69x6Dx65x4Fx66x44x61x79ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx54x69x6Dx65x4Fx66x44x61x79ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx54x69x6Dx65x4Fx66x44x61x79ʺ> Parse(IInput<char>? input)
            {
                var _x54_1 = __GeneratedOdataV4.Parsers.Inners._x54Parser.Instance.Parse(input);
if (!_x54_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx54x69x6Dx65x4Fx66x44x61x79ʺ)!, input);
}

var _x69_1 = __GeneratedOdataV4.Parsers.Inners._x69Parser.Instance.Parse(_x54_1.Remainder);
if (!_x69_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx54x69x6Dx65x4Fx66x44x61x79ʺ)!, input);
}

var _x6D_1 = __GeneratedOdataV4.Parsers.Inners._x6DParser.Instance.Parse(_x69_1.Remainder);
if (!_x6D_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx54x69x6Dx65x4Fx66x44x61x79ʺ)!, input);
}

var _x65_1 = __GeneratedOdataV4.Parsers.Inners._x65Parser.Instance.Parse(_x6D_1.Remainder);
if (!_x65_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx54x69x6Dx65x4Fx66x44x61x79ʺ)!, input);
}

var _x4F_1 = __GeneratedOdataV4.Parsers.Inners._x4FParser.Instance.Parse(_x65_1.Remainder);
if (!_x4F_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx54x69x6Dx65x4Fx66x44x61x79ʺ)!, input);
}

var _x66_1 = __GeneratedOdataV4.Parsers.Inners._x66Parser.Instance.Parse(_x4F_1.Remainder);
if (!_x66_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx54x69x6Dx65x4Fx66x44x61x79ʺ)!, input);
}

var _x44_1 = __GeneratedOdataV4.Parsers.Inners._x44Parser.Instance.Parse(_x66_1.Remainder);
if (!_x44_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx54x69x6Dx65x4Fx66x44x61x79ʺ)!, input);
}

var _x61_1 = __GeneratedOdataV4.Parsers.Inners._x61Parser.Instance.Parse(_x44_1.Remainder);
if (!_x61_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx54x69x6Dx65x4Fx66x44x61x79ʺ)!, input);
}

var _x79_1 = __GeneratedOdataV4.Parsers.Inners._x79Parser.Instance.Parse(_x61_1.Remainder);
if (!_x79_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx54x69x6Dx65x4Fx66x44x61x79ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._ʺx54x69x6Dx65x4Fx66x44x61x79ʺ.Instance, _x79_1.Remainder);
            }
        }
    }
    
}
