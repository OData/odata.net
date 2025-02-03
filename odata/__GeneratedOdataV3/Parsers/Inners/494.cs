namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _SQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTEParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._SQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTE> Instance { get; } = (_SQUOTEⲻinⲻstringParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._SQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTE>(_pcharⲻnoⲻSQUOTEParser.Instance);
        
        public static class _SQUOTEⲻinⲻstringParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._SQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTE._SQUOTEⲻinⲻstring> Instance { get; } = from _SQUOTEⲻinⲻstring_1 in __GeneratedOdataV3.Parsers.Rules._SQUOTEⲻinⲻstringParser.Instance
select new __GeneratedOdataV3.CstNodes.Inners._SQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTE._SQUOTEⲻinⲻstring(_SQUOTEⲻinⲻstring_1);
        }
        
        public static class _pcharⲻnoⲻSQUOTEParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._SQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTE._pcharⲻnoⲻSQUOTE> Instance { get; } = from _pcharⲻnoⲻSQUOTE_1 in __GeneratedOdataV3.Parsers.Rules._pcharⲻnoⲻSQUOTEParser.Instance
select new __GeneratedOdataV3.CstNodes.Inners._SQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTE._pcharⲻnoⲻSQUOTE(_pcharⲻnoⲻSQUOTE_1);
        }
    }
    
}
