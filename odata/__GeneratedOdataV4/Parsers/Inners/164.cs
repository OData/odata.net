namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx24x66x6Fx72x6Dx61x74ʺⳆʺx66x6Fx72x6Dx61x74ʺParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx24x66x6Fx72x6Dx61x74ʺⳆʺx66x6Fx72x6Dx61x74ʺ> Instance { get; } = (_ʺx24x66x6Fx72x6Dx61x74ʺParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Inners._ʺx24x66x6Fx72x6Dx61x74ʺⳆʺx66x6Fx72x6Dx61x74ʺ>(_ʺx66x6Fx72x6Dx61x74ʺParser.Instance);
        
        public static class _ʺx24x66x6Fx72x6Dx61x74ʺParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx24x66x6Fx72x6Dx61x74ʺⳆʺx66x6Fx72x6Dx61x74ʺ._ʺx24x66x6Fx72x6Dx61x74ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx24x66x6Fx72x6Dx61x74ʺⳆʺx66x6Fx72x6Dx61x74ʺ._ʺx24x66x6Fx72x6Dx61x74ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx24x66x6Fx72x6Dx61x74ʺⳆʺx66x6Fx72x6Dx61x74ʺ._ʺx24x66x6Fx72x6Dx61x74ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx24x66x6Fx72x6Dx61x74ʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx24x66x6Fx72x6Dx61x74ʺParser.Instance.Parse(input);
if (!_ʺx24x66x6Fx72x6Dx61x74ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx24x66x6Fx72x6Dx61x74ʺⳆʺx66x6Fx72x6Dx61x74ʺ._ʺx24x66x6Fx72x6Dx61x74ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._ʺx24x66x6Fx72x6Dx61x74ʺⳆʺx66x6Fx72x6Dx61x74ʺ._ʺx24x66x6Fx72x6Dx61x74ʺ.Instance, _ʺx24x66x6Fx72x6Dx61x74ʺ_1.Remainder);
                }
            }
        }
        
        public static class _ʺx66x6Fx72x6Dx61x74ʺParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx24x66x6Fx72x6Dx61x74ʺⳆʺx66x6Fx72x6Dx61x74ʺ._ʺx66x6Fx72x6Dx61x74ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx24x66x6Fx72x6Dx61x74ʺⳆʺx66x6Fx72x6Dx61x74ʺ._ʺx66x6Fx72x6Dx61x74ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx24x66x6Fx72x6Dx61x74ʺⳆʺx66x6Fx72x6Dx61x74ʺ._ʺx66x6Fx72x6Dx61x74ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx66x6Fx72x6Dx61x74ʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx66x6Fx72x6Dx61x74ʺParser.Instance.Parse(input);
if (!_ʺx66x6Fx72x6Dx61x74ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx24x66x6Fx72x6Dx61x74ʺⳆʺx66x6Fx72x6Dx61x74ʺ._ʺx66x6Fx72x6Dx61x74ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._ʺx24x66x6Fx72x6Dx61x74ʺⳆʺx66x6Fx72x6Dx61x74ʺ._ʺx66x6Fx72x6Dx61x74ʺ.Instance, _ʺx66x6Fx72x6Dx61x74ʺ_1.Remainder);
                }
            }
        }
    }
    
}
