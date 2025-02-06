namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _primitiveTypeNameParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._primitiveTypeName> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._primitiveTypeName>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._primitiveTypeName> Parse(IInput<char>? input)
            {
                var _ʺx45x64x6Dx2Eʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx45x64x6Dx2EʺParser.Instance.Parse(input);
if (!_ʺx45x64x6Dx2Eʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._primitiveTypeName)!, input);
}

var _Ⲥʺx42x69x6Ex61x72x79ʺⳆʺx42x6Fx6Fx6Cx65x61x6EʺⳆʺx42x79x74x65ʺⳆʺx44x61x74x65ʺⳆʺx44x61x74x65x54x69x6Dx65x4Fx66x66x73x65x74ʺⳆʺx44x65x63x69x6Dx61x6CʺⳆʺx44x6Fx75x62x6Cx65ʺⳆʺx44x75x72x61x74x69x6Fx6EʺⳆʺx47x75x69x64ʺⳆʺx49x6Ex74x31x36ʺⳆʺx49x6Ex74x33x32ʺⳆʺx49x6Ex74x36x34ʺⳆʺx53x42x79x74x65ʺⳆʺx53x69x6Ex67x6Cx65ʺⳆʺx53x74x72x65x61x6DʺⳆʺx53x74x72x69x6Ex67ʺⳆʺx54x69x6Dx65x4Fx66x44x61x79ʺⳆabstractSpatialTypeName_꘡concreteSpatialTypeName꘡Ↄ_1 = __GeneratedOdataV4.Parsers.Inners._Ⲥʺx42x69x6Ex61x72x79ʺⳆʺx42x6Fx6Fx6Cx65x61x6EʺⳆʺx42x79x74x65ʺⳆʺx44x61x74x65ʺⳆʺx44x61x74x65x54x69x6Dx65x4Fx66x66x73x65x74ʺⳆʺx44x65x63x69x6Dx61x6CʺⳆʺx44x6Fx75x62x6Cx65ʺⳆʺx44x75x72x61x74x69x6Fx6EʺⳆʺx47x75x69x64ʺⳆʺx49x6Ex74x31x36ʺⳆʺx49x6Ex74x33x32ʺⳆʺx49x6Ex74x36x34ʺⳆʺx53x42x79x74x65ʺⳆʺx53x69x6Ex67x6Cx65ʺⳆʺx53x74x72x65x61x6DʺⳆʺx53x74x72x69x6Ex67ʺⳆʺx54x69x6Dx65x4Fx66x44x61x79ʺⳆabstractSpatialTypeName_꘡concreteSpatialTypeName꘡ↃParser.Instance.Parse(_ʺx45x64x6Dx2Eʺ_1.Remainder);
if (!_Ⲥʺx42x69x6Ex61x72x79ʺⳆʺx42x6Fx6Fx6Cx65x61x6EʺⳆʺx42x79x74x65ʺⳆʺx44x61x74x65ʺⳆʺx44x61x74x65x54x69x6Dx65x4Fx66x66x73x65x74ʺⳆʺx44x65x63x69x6Dx61x6CʺⳆʺx44x6Fx75x62x6Cx65ʺⳆʺx44x75x72x61x74x69x6Fx6EʺⳆʺx47x75x69x64ʺⳆʺx49x6Ex74x31x36ʺⳆʺx49x6Ex74x33x32ʺⳆʺx49x6Ex74x36x34ʺⳆʺx53x42x79x74x65ʺⳆʺx53x69x6Ex67x6Cx65ʺⳆʺx53x74x72x65x61x6DʺⳆʺx53x74x72x69x6Ex67ʺⳆʺx54x69x6Dx65x4Fx66x44x61x79ʺⳆabstractSpatialTypeName_꘡concreteSpatialTypeName꘡Ↄ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._primitiveTypeName)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._primitiveTypeName(_ʺx45x64x6Dx2Eʺ_1.Parsed, _Ⲥʺx42x69x6Ex61x72x79ʺⳆʺx42x6Fx6Fx6Cx65x61x6EʺⳆʺx42x79x74x65ʺⳆʺx44x61x74x65ʺⳆʺx44x61x74x65x54x69x6Dx65x4Fx66x66x73x65x74ʺⳆʺx44x65x63x69x6Dx61x6CʺⳆʺx44x6Fx75x62x6Cx65ʺⳆʺx44x75x72x61x74x69x6Fx6EʺⳆʺx47x75x69x64ʺⳆʺx49x6Ex74x31x36ʺⳆʺx49x6Ex74x33x32ʺⳆʺx49x6Ex74x36x34ʺⳆʺx53x42x79x74x65ʺⳆʺx53x69x6Ex67x6Cx65ʺⳆʺx53x74x72x65x61x6DʺⳆʺx53x74x72x69x6Ex67ʺⳆʺx54x69x6Dx65x4Fx66x44x61x79ʺⳆabstractSpatialTypeName_꘡concreteSpatialTypeName꘡Ↄ_1.Parsed), _Ⲥʺx42x69x6Ex61x72x79ʺⳆʺx42x6Fx6Fx6Cx65x61x6EʺⳆʺx42x79x74x65ʺⳆʺx44x61x74x65ʺⳆʺx44x61x74x65x54x69x6Dx65x4Fx66x66x73x65x74ʺⳆʺx44x65x63x69x6Dx61x6CʺⳆʺx44x6Fx75x62x6Cx65ʺⳆʺx44x75x72x61x74x69x6Fx6EʺⳆʺx47x75x69x64ʺⳆʺx49x6Ex74x31x36ʺⳆʺx49x6Ex74x33x32ʺⳆʺx49x6Ex74x36x34ʺⳆʺx53x42x79x74x65ʺⳆʺx53x69x6Ex67x6Cx65ʺⳆʺx53x74x72x65x61x6DʺⳆʺx53x74x72x69x6Ex67ʺⳆʺx54x69x6Dx65x4Fx66x44x61x79ʺⳆabstractSpatialTypeName_꘡concreteSpatialTypeName꘡Ↄ_1.Remainder);
            }
        }
    }
    
}
