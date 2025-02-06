namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _primitiveLiteralTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._primitiveLiteral>
    {
        private _primitiveLiteralTranscriber()
        {
        }
        
        public static _primitiveLiteralTranscriber Instance { get; } = new _primitiveLiteralTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._primitiveLiteral value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV4.CstNodes.Rules._primitiveLiteral.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._primitiveLiteral._nullValue node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._nullValueTranscriber.Instance.Transcribe(node._nullValue_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._primitiveLiteral._booleanValue node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._booleanValueTranscriber.Instance.Transcribe(node._booleanValue_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._primitiveLiteral._guidValue node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._guidValueTranscriber.Instance.Transcribe(node._guidValue_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._primitiveLiteral._dateValue node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._dateValueTranscriber.Instance.Transcribe(node._dateValue_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._primitiveLiteral._dateTimeOffsetValue node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._dateTimeOffsetValueTranscriber.Instance.Transcribe(node._dateTimeOffsetValue_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._primitiveLiteral._timeOfDayValue node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._timeOfDayValueTranscriber.Instance.Transcribe(node._timeOfDayValue_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._primitiveLiteral._decimalValue node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._decimalValueTranscriber.Instance.Transcribe(node._decimalValue_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._primitiveLiteral._doubleValue node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._doubleValueTranscriber.Instance.Transcribe(node._doubleValue_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._primitiveLiteral._singleValue node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._singleValueTranscriber.Instance.Transcribe(node._singleValue_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._primitiveLiteral._sbyteValue node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._sbyteValueTranscriber.Instance.Transcribe(node._sbyteValue_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._primitiveLiteral._byteValue node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._byteValueTranscriber.Instance.Transcribe(node._byteValue_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._primitiveLiteral._int16Value node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._int16ValueTranscriber.Instance.Transcribe(node._int16Value_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._primitiveLiteral._int32Value node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._int32ValueTranscriber.Instance.Transcribe(node._int32Value_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._primitiveLiteral._int64Value node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._int64ValueTranscriber.Instance.Transcribe(node._int64Value_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._primitiveLiteral._string node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._stringTranscriber.Instance.Transcribe(node._string_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._primitiveLiteral._duration node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._durationTranscriber.Instance.Transcribe(node._duration_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._primitiveLiteral._enum node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._enumTranscriber.Instance.Transcribe(node._enum_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._primitiveLiteral._binary node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._binaryTranscriber.Instance.Transcribe(node._binary_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._primitiveLiteral._geographyCollection node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._geographyCollectionTranscriber.Instance.Transcribe(node._geographyCollection_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._primitiveLiteral._geographyLineString node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._geographyLineStringTranscriber.Instance.Transcribe(node._geographyLineString_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._primitiveLiteral._geographyMultiLineString node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._geographyMultiLineStringTranscriber.Instance.Transcribe(node._geographyMultiLineString_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._primitiveLiteral._geographyMultiPoint node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._geographyMultiPointTranscriber.Instance.Transcribe(node._geographyMultiPoint_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._primitiveLiteral._geographyMultiPolygon node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._geographyMultiPolygonTranscriber.Instance.Transcribe(node._geographyMultiPolygon_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._primitiveLiteral._geographyPoint node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._geographyPointTranscriber.Instance.Transcribe(node._geographyPoint_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._primitiveLiteral._geographyPolygon node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._geographyPolygonTranscriber.Instance.Transcribe(node._geographyPolygon_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._primitiveLiteral._geometryCollection node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._geometryCollectionTranscriber.Instance.Transcribe(node._geometryCollection_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._primitiveLiteral._geometryLineString node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._geometryLineStringTranscriber.Instance.Transcribe(node._geometryLineString_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._primitiveLiteral._geometryMultiLineString node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._geometryMultiLineStringTranscriber.Instance.Transcribe(node._geometryMultiLineString_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._primitiveLiteral._geometryMultiPoint node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._geometryMultiPointTranscriber.Instance.Transcribe(node._geometryMultiPoint_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._primitiveLiteral._geometryMultiPolygon node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._geometryMultiPolygonTranscriber.Instance.Transcribe(node._geometryMultiPolygon_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._primitiveLiteral._geometryPoint node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._geometryPointTranscriber.Instance.Transcribe(node._geometryPoint_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Rules._primitiveLiteral._geometryPolygon node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._geometryPolygonTranscriber.Instance.Transcribe(node._geometryPolygon_1, context);

return default;
            }
        }
    }
    
}
