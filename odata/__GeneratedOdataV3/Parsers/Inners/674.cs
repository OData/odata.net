namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ⲤDIGITⳆʺx41ʺⳆʺx42ʺⳆʺx44ʺⳆʺx45ʺⳆʺx46ʺↃParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ⲤDIGITⳆʺx41ʺⳆʺx42ʺⳆʺx44ʺⳆʺx45ʺⳆʺx46ʺↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ⲤDIGITⳆʺx41ʺⳆʺx42ʺⳆʺx44ʺⳆʺx45ʺⳆʺx46ʺↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ⲤDIGITⳆʺx41ʺⳆʺx42ʺⳆʺx44ʺⳆʺx45ʺⳆʺx46ʺↃ> Parse(IInput<char>? input)
            {
                var _DIGITⳆʺx41ʺⳆʺx42ʺⳆʺx44ʺⳆʺx45ʺⳆʺx46ʺ_1 = __GeneratedOdataV3.Parsers.Inners._DIGITⳆʺx41ʺⳆʺx42ʺⳆʺx44ʺⳆʺx45ʺⳆʺx46ʺParser.Instance.Parse(input);
if (!_DIGITⳆʺx41ʺⳆʺx42ʺⳆʺx44ʺⳆʺx45ʺⳆʺx46ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ⲤDIGITⳆʺx41ʺⳆʺx42ʺⳆʺx44ʺⳆʺx45ʺⳆʺx46ʺↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._ⲤDIGITⳆʺx41ʺⳆʺx42ʺⳆʺx44ʺⳆʺx45ʺⳆʺx46ʺↃ(_DIGITⳆʺx41ʺⳆʺx42ʺⳆʺx44ʺⳆʺx45ʺⳆʺx46ʺ_1.Parsed), _DIGITⳆʺx41ʺⳆʺx42ʺⳆʺx44ʺⳆʺx45ʺⳆʺx46ʺ_1.Remainder);
            }
        }
    }
    
}
