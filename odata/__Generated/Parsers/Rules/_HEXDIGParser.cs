namespace __Generated.Parsers.Rules
{
    using Sprache;
    
    public static class _HEXDIGParser
    {
        public static Parser<__Generated.CstNodes.Rules._HEXDIG> Instance { get; }
        
        public static class _DIGITParser
        {
            public static Parser<__Generated.CstNodes.Rules._HEXDIG._DIGIT> Instance { get; } = from _DIGIT_1 in __Generated.Parsers.Rules._DIGITParser.Instance
select new __Generated.CstNodes.Rules._HEXDIG._DIGIT(_DIGIT_1);
        }
        
        public static class _ʺx41ʺParser
        {
            public static Parser<__Generated.CstNodes.Rules._HEXDIG._ʺx41ʺ> Instance { get; } = from _ʺx41ʺ_1 in __Generated.Parsers.Inners._ʺx41ʺParser.Instance
select new __Generated.CstNodes.Rules._HEXDIG._ʺx41ʺ(_ʺx41ʺ_1);
        }
        
        public static class _ʺx42ʺParser
        {
            public static Parser<__Generated.CstNodes.Rules._HEXDIG._ʺx42ʺ> Instance { get; } = from _ʺx42ʺ_1 in __Generated.Parsers.Inners._ʺx42ʺParser.Instance
select new __Generated.CstNodes.Rules._HEXDIG._ʺx42ʺ(_ʺx42ʺ_1);
        }
        
        public static class _ʺx43ʺParser
        {
            public static Parser<__Generated.CstNodes.Rules._HEXDIG._ʺx43ʺ> Instance { get; } = from _ʺx43ʺ_1 in __Generated.Parsers.Inners._ʺx43ʺParser.Instance
select new __Generated.CstNodes.Rules._HEXDIG._ʺx43ʺ(_ʺx43ʺ_1);
        }
        
        public static class _ʺx44ʺParser
        {
            public static Parser<__Generated.CstNodes.Rules._HEXDIG._ʺx44ʺ> Instance { get; } = from _ʺx44ʺ_1 in __Generated.Parsers.Inners._ʺx44ʺParser.Instance
select new __Generated.CstNodes.Rules._HEXDIG._ʺx44ʺ(_ʺx44ʺ_1);
        }
        
        public static class _ʺx45ʺParser
        {
            public static Parser<__Generated.CstNodes.Rules._HEXDIG._ʺx45ʺ> Instance { get; } = from _ʺx45ʺ_1 in __Generated.Parsers.Inners._ʺx45ʺParser.Instance
select new __Generated.CstNodes.Rules._HEXDIG._ʺx45ʺ(_ʺx45ʺ_1);
        }
        
        public static class _ʺx46ʺParser
        {
            public static Parser<__Generated.CstNodes.Rules._HEXDIG._ʺx46ʺ> Instance { get; } = from _ʺx46ʺ_1 in __Generated.Parsers.Inners._ʺx46ʺParser.Instance
select new __Generated.CstNodes.Rules._HEXDIG._ʺx46ʺ(_ʺx46ʺ_1);
        }
    }
    
}
