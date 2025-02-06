namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _x3DParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._x3D> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._x3D>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._x3D> Parse(IInput<char>? input)
            {
                var _x3D = CombinatorParsingV2.Parse.Char((char)0x3D).Parse(input);
if (!_x3D.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._x3D)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._x3D.Instance, _x3D.Remainder);
            }
        }
    }
    
}
