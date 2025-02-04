namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _expandItemParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._expandItem> Instance { get; } = (_STAR_꘡refⳆOPEN_levels_CLOSE꘡Parser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._expandItem>(_ʺx24x76x61x6Cx75x65ʺParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._expandItem>(_expandPath_꘡ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡Ⳇcount_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡ⳆOPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSE꘡Parser.Instance);
        
        public static class _STAR_꘡refⳆOPEN_levels_CLOSE꘡Parser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._expandItem._STAR_꘡refⳆOPEN_levels_CLOSE꘡> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._expandItem._STAR_꘡refⳆOPEN_levels_CLOSE꘡>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._expandItem._STAR_꘡refⳆOPEN_levels_CLOSE꘡> Parse(IInput<char>? input)
                {
                    var _STAR_1 = __GeneratedOdataV3.Parsers.Rules._STARParser.Instance.Parse(input);
if (!_STAR_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._expandItem._STAR_꘡refⳆOPEN_levels_CLOSE꘡)!, input);
}

var _refⳆOPEN_levels_CLOSE_1 = __GeneratedOdataV3.Parsers.Inners._refⳆOPEN_levels_CLOSEParser.Instance.Optional().Parse(_STAR_1.Remainder);
if (!_refⳆOPEN_levels_CLOSE_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._expandItem._STAR_꘡refⳆOPEN_levels_CLOSE꘡)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._expandItem._STAR_꘡refⳆOPEN_levels_CLOSE꘡(_STAR_1.Parsed,  _refⳆOPEN_levels_CLOSE_1.Parsed.GetOrElse(null)), _refⳆOPEN_levels_CLOSE_1.Remainder);
                }
            }
        }
        
        public static class _ʺx24x76x61x6Cx75x65ʺParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._expandItem._ʺx24x76x61x6Cx75x65ʺ> Instance { get; } = from _ʺx24x76x61x6Cx75x65ʺ_1 in __GeneratedOdataV3.Parsers.Inners._ʺx24x76x61x6Cx75x65ʺParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._expandItem._ʺx24x76x61x6Cx75x65ʺ(_ʺx24x76x61x6Cx75x65ʺ_1);
        }
        
        public static class _expandPath_꘡ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡Ⳇcount_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡ⳆOPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSE꘡Parser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._expandItem._expandPath_꘡ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡Ⳇcount_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡ⳆOPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSE꘡> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._expandItem._expandPath_꘡ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡Ⳇcount_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡ⳆOPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSE꘡>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._expandItem._expandPath_꘡ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡Ⳇcount_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡ⳆOPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSE꘡> Parse(IInput<char>? input)
                {
                    var _expandPath_1 = __GeneratedOdataV3.Parsers.Rules._expandPathParser.Instance.Parse(input);
if (!_expandPath_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._expandItem._expandPath_꘡ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡Ⳇcount_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡ⳆOPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSE꘡)!, input);
}

var _ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡Ⳇcount_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡ⳆOPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSE_1 = __GeneratedOdataV3.Parsers.Inners._ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡Ⳇcount_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡ⳆOPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSEParser.Instance.Optional().Parse(_expandPath_1.Remainder);
if (!_ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡Ⳇcount_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡ⳆOPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSE_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._expandItem._expandPath_꘡ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡Ⳇcount_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡ⳆOPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSE꘡)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._expandItem._expandPath_꘡ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡Ⳇcount_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡ⳆOPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSE꘡(_expandPath_1.Parsed,  _ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡Ⳇcount_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡ⳆOPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSE_1.Parsed.GetOrElse(null)), _ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡Ⳇcount_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡ⳆOPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSE_1.Remainder);
                }
            }
        }
    }
    
}
