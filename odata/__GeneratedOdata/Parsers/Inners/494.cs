namespace __GeneratedOdata.Parsers.Inners
{
    using Sprache;
    
    public static class _SQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTEParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Inners._SQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTE> Instance { get; } = (_SQUOTEⲻinⲻstringParser.Instance).Or<__GeneratedOdata.CstNodes.Inners._SQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTE>(_pcharⲻnoⲻSQUOTEParser.Instance);
        
        public static class _SQUOTEⲻinⲻstringParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Inners._SQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTE._SQUOTEⲻinⲻstring> Instance { get; } = from _SQUOTEⲻinⲻstring_1 in __GeneratedOdata.Parsers.Rules._SQUOTEⲻinⲻstringParser.Instance
select new __GeneratedOdata.CstNodes.Inners._SQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTE._SQUOTEⲻinⲻstring(_SQUOTEⲻinⲻstring_1);
        }
        
        public static class _pcharⲻnoⲻSQUOTEParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Inners._SQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTE._pcharⲻnoⲻSQUOTE> Instance { get; } = from _pcharⲻnoⲻSQUOTE_1 in __GeneratedOdata.Parsers.Rules._pcharⲻnoⲻSQUOTEParser.Instance
select new __GeneratedOdata.CstNodes.Inners._SQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTE._pcharⲻnoⲻSQUOTE(_pcharⲻnoⲻSQUOTE_1);
        }
    }
    
}
