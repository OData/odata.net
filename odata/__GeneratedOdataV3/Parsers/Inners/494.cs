namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _SQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTEParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._SQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTE> Instance { get; } = (_SQUOTEⲻinⲻstringParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._SQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTE>(_pcharⲻnoⲻSQUOTEParser.Instance);
        
        public static class _SQUOTEⲻinⲻstringParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._SQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTE._SQUOTEⲻinⲻstring> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._SQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTE._SQUOTEⲻinⲻstring>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._SQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTE._SQUOTEⲻinⲻstring> Parse(IInput<char>? input)
                {
                    var _SQUOTEⲻinⲻstring_1 = __GeneratedOdataV3.Parsers.Rules._SQUOTEⲻinⲻstringParser.Instance.Parse(input);
if (!_SQUOTEⲻinⲻstring_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._SQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTE._SQUOTEⲻinⲻstring)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._SQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTE._SQUOTEⲻinⲻstring(_SQUOTEⲻinⲻstring_1.Parsed), _SQUOTEⲻinⲻstring_1.Remainder);
                }
            }
        }
        
        public static class _pcharⲻnoⲻSQUOTEParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._SQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTE._pcharⲻnoⲻSQUOTE> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._SQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTE._pcharⲻnoⲻSQUOTE>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._SQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTE._pcharⲻnoⲻSQUOTE> Parse(IInput<char>? input)
                {
                    var _pcharⲻnoⲻSQUOTE_1 = __GeneratedOdataV3.Parsers.Rules._pcharⲻnoⲻSQUOTEParser.Instance.Parse(input);
if (!_pcharⲻnoⲻSQUOTE_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._SQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTE._pcharⲻnoⲻSQUOTE)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._SQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTE._pcharⲻnoⲻSQUOTE(_pcharⲻnoⲻSQUOTE_1.Parsed), _pcharⲻnoⲻSQUOTE_1.Remainder);
                }
            }
        }
    }
    
}
