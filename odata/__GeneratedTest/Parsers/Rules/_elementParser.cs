namespace __GeneratedTest.Parsers.Rules
{
    using Sprache;
    
    public static class _elementParser
    {
        public static Parser<__Generated.CstNodes.Rules._element> Instance { get; } = (_rulenameParser.Instance).Or<__Generated.CstNodes.Rules._element>(_groupParser.Instance).Or<__Generated.CstNodes.Rules._element>(_optionParser.Instance).Or<__Generated.CstNodes.Rules._element>(_charⲻvalParser.Instance).Or<__Generated.CstNodes.Rules._element>(_numⲻvalParser.Instance).Or<__Generated.CstNodes.Rules._element>(_proseⲻvalParser.Instance);
        
        public static class _rulenameParser
        {
            public static Parser<__Generated.CstNodes.Rules._element._rulename> Instance { get; } = from _rulename_1 in __GeneratedTest.Parsers.Rules._rulenameParser.Instance
select new __Generated.CstNodes.Rules._element._rulename(_rulename_1);
        }
        
        public static class _groupParser
        {
            public static Parser<__Generated.CstNodes.Rules._element._group> Instance { get; } = from _group_1 in __GeneratedTest.Parsers.Rules._groupParser.Instance
select new __Generated.CstNodes.Rules._element._group(_group_1);
        }
        
        public static class _optionParser
        {
            public static Parser<__Generated.CstNodes.Rules._element._option> Instance { get; } = from _option_1 in __GeneratedTest.Parsers.Rules._optionParser.Instance
select new __Generated.CstNodes.Rules._element._option(_option_1);
        }
        
        public static class _charⲻvalParser
        {
            public static Parser<__Generated.CstNodes.Rules._element._charⲻval> Instance { get; } = from _charⲻval_1 in __GeneratedTest.Parsers.Rules._charⲻvalParser.Instance
select new __Generated.CstNodes.Rules._element._charⲻval(_charⲻval_1);
        }
        
        public static class _numⲻvalParser
        {
            public static Parser<__Generated.CstNodes.Rules._element._numⲻval> Instance { get; } = from _numⲻval_1 in __GeneratedTest.Parsers.Rules._numⲻvalParser.Instance
select new __Generated.CstNodes.Rules._element._numⲻval(_numⲻval_1);
        }
        
        public static class _proseⲻvalParser
        {
            public static Parser<__Generated.CstNodes.Rules._element._proseⲻval> Instance { get; } = from _proseⲻval_1 in __GeneratedTest.Parsers.Rules._proseⲻvalParser.Instance
select new __Generated.CstNodes.Rules._element._proseⲻval(_proseⲻval_1);
        }
    }
    
}
