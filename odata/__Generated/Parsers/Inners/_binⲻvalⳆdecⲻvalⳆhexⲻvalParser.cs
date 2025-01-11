namespace __Generated.Parsers.Inners
{
    using Sprache;
    
    public static class _binⲻvalⳆdecⲻvalⳆhexⲻvalParser
    {
        public static Parser<__Generated.CstNodes.Inners._binⲻvalⳆdecⲻvalⳆhexⲻval> Instance { get; } = (_binⲻvalParser.Instance).Or<__Generated.CstNodes.Inners._binⲻvalⳆdecⲻvalⳆhexⲻval>(_decⲻvalParser.Instance).Or<__Generated.CstNodes.Inners._binⲻvalⳆdecⲻvalⳆhexⲻval>(_hexⲻvalParser.Instance);
        
        public static class _binⲻvalParser
        {
            public static Parser<__Generated.CstNodes.Inners._binⲻvalⳆdecⲻvalⳆhexⲻval._binⲻval> Instance { get; } = from _binⲻval_1 in __Generated.Parsers.Rules._binⲻvalParser.Instance
select new __Generated.CstNodes.Inners._binⲻvalⳆdecⲻvalⳆhexⲻval._binⲻval(_binⲻval_1);
        }
        
        public static class _decⲻvalParser
        {
            public static Parser<__Generated.CstNodes.Inners._binⲻvalⳆdecⲻvalⳆhexⲻval._decⲻval> Instance { get; } = from _decⲻval_1 in __Generated.Parsers.Rules._decⲻvalParser.Instance
select new __Generated.CstNodes.Inners._binⲻvalⳆdecⲻvalⳆhexⲻval._decⲻval(_decⲻval_1);
        }
        
        public static class _hexⲻvalParser
        {
            public static Parser<__Generated.CstNodes.Inners._binⲻvalⳆdecⲻvalⳆhexⲻval._hexⲻval> Instance { get; } = from _hexⲻval_1 in __Generated.Parsers.Rules._hexⲻvalParser.Instance
select new __Generated.CstNodes.Inners._binⲻvalⳆdecⲻvalⳆhexⲻval._hexⲻval(_hexⲻval_1);
        }
    }
    
}
