namespace __GeneratedOdataV3.Trancsribers.Rules
{
    public sealed class _primitiveValueTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Rules._primitiveValue>
    {
        private _primitiveValueTranscriber()
        {
        }
        
        public static _primitiveValueTranscriber Instance { get; } = new _primitiveValueTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Rules._primitiveValue value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV3.CstNodes.Rules._primitiveValue.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._primitiveValue._booleanValue node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._booleanValueTranscriber.Instance.Transcribe(node._booleanValue_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._primitiveValue._guidValue node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._guidValueTranscriber.Instance.Transcribe(node._guidValue_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._primitiveValue._durationValue node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._durationValueTranscriber.Instance.Transcribe(node._durationValue_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._primitiveValue._dateValue node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._dateValueTranscriber.Instance.Transcribe(node._dateValue_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._primitiveValue._dateTimeOffsetValue node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._dateTimeOffsetValueTranscriber.Instance.Transcribe(node._dateTimeOffsetValue_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._primitiveValue._timeOfDayValue node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._timeOfDayValueTranscriber.Instance.Transcribe(node._timeOfDayValue_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._primitiveValue._enumValue node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._enumValueTranscriber.Instance.Transcribe(node._enumValue_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._primitiveValue._fullCollectionLiteral node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._fullCollectionLiteralTranscriber.Instance.Transcribe(node._fullCollectionLiteral_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._primitiveValue._fullLineStringLiteral node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._fullLineStringLiteralTranscriber.Instance.Transcribe(node._fullLineStringLiteral_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._primitiveValue._fullMultiPointLiteral node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._fullMultiPointLiteralTranscriber.Instance.Transcribe(node._fullMultiPointLiteral_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._primitiveValue._fullMultiLineStringLiteral node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._fullMultiLineStringLiteralTranscriber.Instance.Transcribe(node._fullMultiLineStringLiteral_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._primitiveValue._fullMultiPolygonLiteral node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._fullMultiPolygonLiteralTranscriber.Instance.Transcribe(node._fullMultiPolygonLiteral_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._primitiveValue._fullPointLiteral node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._fullPointLiteralTranscriber.Instance.Transcribe(node._fullPointLiteral_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._primitiveValue._fullPolygonLiteral node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._fullPolygonLiteralTranscriber.Instance.Transcribe(node._fullPolygonLiteral_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._primitiveValue._decimalValue node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._decimalValueTranscriber.Instance.Transcribe(node._decimalValue_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._primitiveValue._doubleValue node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._doubleValueTranscriber.Instance.Transcribe(node._doubleValue_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._primitiveValue._singleValue node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._singleValueTranscriber.Instance.Transcribe(node._singleValue_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._primitiveValue._sbyteValue node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._sbyteValueTranscriber.Instance.Transcribe(node._sbyteValue_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._primitiveValue._byteValue node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._byteValueTranscriber.Instance.Transcribe(node._byteValue_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._primitiveValue._int16Value node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._int16ValueTranscriber.Instance.Transcribe(node._int16Value_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._primitiveValue._int32Value node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._int32ValueTranscriber.Instance.Transcribe(node._int32Value_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._primitiveValue._int64Value node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._int64ValueTranscriber.Instance.Transcribe(node._int64Value_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._primitiveValue._binaryValue node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._binaryValueTranscriber.Instance.Transcribe(node._binaryValue_1, context);

return default;
            }
        }
    }
    
}
