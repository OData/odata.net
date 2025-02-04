namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ⲤSQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTEↃParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ⲤSQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTEↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ⲤSQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTEↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ⲤSQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTEↃ> Parse(IInput<char>? input)
            {
                var _SQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTE_1 = __GeneratedOdataV3.Parsers.Inners._SQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTEParser.Instance.Parse(input);
if (!_SQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTE_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ⲤSQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTEↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._ⲤSQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTEↃ(_SQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTE_1.Parsed), _SQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTE_1.Remainder);
            }
        }
    }
    
}
