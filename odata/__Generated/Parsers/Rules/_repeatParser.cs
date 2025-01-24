namespace __Generated.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _repeatParser
    {
        public static IParser<char, __Generated.CstNodes.Rules._repeat> Instance { get; } = (_Ⲥʺx2Aʺ_ЖDIGITↃParser.Instance).Or<char, __Generated.CstNodes.Rules._repeat>(_Ⲥ1ЖDIGIT_꘡ʺx2Aʺ_ЖDIGIT꘡ↃParser.Instance);
        
        public static class _Ⲥʺx2Aʺ_ЖDIGITↃParser
        {
            public static IParser<char, __Generated.CstNodes.Rules._repeat._Ⲥʺx2Aʺ_ЖDIGITↃ> Instance { get; } = from _Ⲥʺx2Aʺ_ЖDIGITↃ_1 in __Generated.Parsers.Inners._Ⲥʺx2Aʺ_ЖDIGITↃParser.Instance
select new __Generated.CstNodes.Rules._repeat._Ⲥʺx2Aʺ_ЖDIGITↃ(_Ⲥʺx2Aʺ_ЖDIGITↃ_1);
        }
        
        public static class _Ⲥ1ЖDIGIT_꘡ʺx2Aʺ_ЖDIGIT꘡ↃParser
        {
            public static IParser<char, __Generated.CstNodes.Rules._repeat._Ⲥ1ЖDIGIT_꘡ʺx2Aʺ_ЖDIGIT꘡Ↄ> Instance { get; } = from _Ⲥ1ЖDIGIT_꘡ʺx2Aʺ_ЖDIGIT꘡Ↄ_1 in __Generated.Parsers.Inners._Ⲥ1ЖDIGIT_꘡ʺx2Aʺ_ЖDIGIT꘡ↃParser.Instance
select new __Generated.CstNodes.Rules._repeat._Ⲥ1ЖDIGIT_꘡ʺx2Aʺ_ЖDIGIT꘡Ↄ(_Ⲥ1ЖDIGIT_꘡ʺx2Aʺ_ЖDIGIT꘡Ↄ_1);
        }
    }
    
}
