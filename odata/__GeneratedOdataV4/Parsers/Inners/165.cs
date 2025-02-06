namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _Ⲥʺx24x66x6Fx72x6Dx61x74ʺⳆʺx66x6Fx72x6Dx61x74ʺↃParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._Ⲥʺx24x66x6Fx72x6Dx61x74ʺⳆʺx66x6Fx72x6Dx61x74ʺↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._Ⲥʺx24x66x6Fx72x6Dx61x74ʺⳆʺx66x6Fx72x6Dx61x74ʺↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._Ⲥʺx24x66x6Fx72x6Dx61x74ʺⳆʺx66x6Fx72x6Dx61x74ʺↃ> Parse(IInput<char>? input)
            {
                var _ʺx24x66x6Fx72x6Dx61x74ʺⳆʺx66x6Fx72x6Dx61x74ʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx24x66x6Fx72x6Dx61x74ʺⳆʺx66x6Fx72x6Dx61x74ʺParser.Instance.Parse(input);
if (!_ʺx24x66x6Fx72x6Dx61x74ʺⳆʺx66x6Fx72x6Dx61x74ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._Ⲥʺx24x66x6Fx72x6Dx61x74ʺⳆʺx66x6Fx72x6Dx61x74ʺↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._Ⲥʺx24x66x6Fx72x6Dx61x74ʺⳆʺx66x6Fx72x6Dx61x74ʺↃ(_ʺx24x66x6Fx72x6Dx61x74ʺⳆʺx66x6Fx72x6Dx61x74ʺ_1.Parsed), _ʺx24x66x6Fx72x6Dx61x74ʺⳆʺx66x6Fx72x6Dx61x74ʺ_1.Remainder);
            }
        }
    }
    
}
