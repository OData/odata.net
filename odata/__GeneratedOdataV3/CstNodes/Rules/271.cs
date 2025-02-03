namespace __GeneratedOdataV3.CstNodes.Rules
{
    public abstract class _primitiveValue
    {
        private _primitiveValue()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_primitiveValue node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_primitiveValue._booleanValue node, TContext context);
            protected internal abstract TResult Accept(_primitiveValue._guidValue node, TContext context);
            protected internal abstract TResult Accept(_primitiveValue._durationValue node, TContext context);
            protected internal abstract TResult Accept(_primitiveValue._dateValue node, TContext context);
            protected internal abstract TResult Accept(_primitiveValue._dateTimeOffsetValue node, TContext context);
            protected internal abstract TResult Accept(_primitiveValue._timeOfDayValue node, TContext context);
            protected internal abstract TResult Accept(_primitiveValue._enumValue node, TContext context);
            protected internal abstract TResult Accept(_primitiveValue._fullCollectionLiteral node, TContext context);
            protected internal abstract TResult Accept(_primitiveValue._fullLineStringLiteral node, TContext context);
            protected internal abstract TResult Accept(_primitiveValue._fullMultiPointLiteral node, TContext context);
            protected internal abstract TResult Accept(_primitiveValue._fullMultiLineStringLiteral node, TContext context);
            protected internal abstract TResult Accept(_primitiveValue._fullMultiPolygonLiteral node, TContext context);
            protected internal abstract TResult Accept(_primitiveValue._fullPointLiteral node, TContext context);
            protected internal abstract TResult Accept(_primitiveValue._fullPolygonLiteral node, TContext context);
            protected internal abstract TResult Accept(_primitiveValue._decimalValue node, TContext context);
            protected internal abstract TResult Accept(_primitiveValue._doubleValue node, TContext context);
            protected internal abstract TResult Accept(_primitiveValue._singleValue node, TContext context);
            protected internal abstract TResult Accept(_primitiveValue._sbyteValue node, TContext context);
            protected internal abstract TResult Accept(_primitiveValue._byteValue node, TContext context);
            protected internal abstract TResult Accept(_primitiveValue._int16Value node, TContext context);
            protected internal abstract TResult Accept(_primitiveValue._int32Value node, TContext context);
            protected internal abstract TResult Accept(_primitiveValue._int64Value node, TContext context);
            protected internal abstract TResult Accept(_primitiveValue._binaryValue node, TContext context);
        }
        
        public sealed class _booleanValue : _primitiveValue
        {
            public _booleanValue(__GeneratedOdataV3.CstNodes.Rules._booleanValue _booleanValue_1)
            {
                this._booleanValue_1 = _booleanValue_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._booleanValue _booleanValue_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _guidValue : _primitiveValue
        {
            public _guidValue(__GeneratedOdataV3.CstNodes.Rules._guidValue _guidValue_1)
            {
                this._guidValue_1 = _guidValue_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._guidValue _guidValue_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _durationValue : _primitiveValue
        {
            public _durationValue(__GeneratedOdataV3.CstNodes.Rules._durationValue _durationValue_1)
            {
                this._durationValue_1 = _durationValue_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._durationValue _durationValue_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _dateValue : _primitiveValue
        {
            public _dateValue(__GeneratedOdataV3.CstNodes.Rules._dateValue _dateValue_1)
            {
                this._dateValue_1 = _dateValue_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._dateValue _dateValue_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _dateTimeOffsetValue : _primitiveValue
        {
            public _dateTimeOffsetValue(__GeneratedOdataV3.CstNodes.Rules._dateTimeOffsetValue _dateTimeOffsetValue_1)
            {
                this._dateTimeOffsetValue_1 = _dateTimeOffsetValue_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._dateTimeOffsetValue _dateTimeOffsetValue_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _timeOfDayValue : _primitiveValue
        {
            public _timeOfDayValue(__GeneratedOdataV3.CstNodes.Rules._timeOfDayValue _timeOfDayValue_1)
            {
                this._timeOfDayValue_1 = _timeOfDayValue_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._timeOfDayValue _timeOfDayValue_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _enumValue : _primitiveValue
        {
            public _enumValue(__GeneratedOdataV3.CstNodes.Rules._enumValue _enumValue_1)
            {
                this._enumValue_1 = _enumValue_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._enumValue _enumValue_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _fullCollectionLiteral : _primitiveValue
        {
            public _fullCollectionLiteral(__GeneratedOdataV3.CstNodes.Rules._fullCollectionLiteral _fullCollectionLiteral_1)
            {
                this._fullCollectionLiteral_1 = _fullCollectionLiteral_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._fullCollectionLiteral _fullCollectionLiteral_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _fullLineStringLiteral : _primitiveValue
        {
            public _fullLineStringLiteral(__GeneratedOdataV3.CstNodes.Rules._fullLineStringLiteral _fullLineStringLiteral_1)
            {
                this._fullLineStringLiteral_1 = _fullLineStringLiteral_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._fullLineStringLiteral _fullLineStringLiteral_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _fullMultiPointLiteral : _primitiveValue
        {
            public _fullMultiPointLiteral(__GeneratedOdataV3.CstNodes.Rules._fullMultiPointLiteral _fullMultiPointLiteral_1)
            {
                this._fullMultiPointLiteral_1 = _fullMultiPointLiteral_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._fullMultiPointLiteral _fullMultiPointLiteral_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _fullMultiLineStringLiteral : _primitiveValue
        {
            public _fullMultiLineStringLiteral(__GeneratedOdataV3.CstNodes.Rules._fullMultiLineStringLiteral _fullMultiLineStringLiteral_1)
            {
                this._fullMultiLineStringLiteral_1 = _fullMultiLineStringLiteral_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._fullMultiLineStringLiteral _fullMultiLineStringLiteral_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _fullMultiPolygonLiteral : _primitiveValue
        {
            public _fullMultiPolygonLiteral(__GeneratedOdataV3.CstNodes.Rules._fullMultiPolygonLiteral _fullMultiPolygonLiteral_1)
            {
                this._fullMultiPolygonLiteral_1 = _fullMultiPolygonLiteral_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._fullMultiPolygonLiteral _fullMultiPolygonLiteral_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _fullPointLiteral : _primitiveValue
        {
            public _fullPointLiteral(__GeneratedOdataV3.CstNodes.Rules._fullPointLiteral _fullPointLiteral_1)
            {
                this._fullPointLiteral_1 = _fullPointLiteral_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._fullPointLiteral _fullPointLiteral_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _fullPolygonLiteral : _primitiveValue
        {
            public _fullPolygonLiteral(__GeneratedOdataV3.CstNodes.Rules._fullPolygonLiteral _fullPolygonLiteral_1)
            {
                this._fullPolygonLiteral_1 = _fullPolygonLiteral_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._fullPolygonLiteral _fullPolygonLiteral_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _decimalValue : _primitiveValue
        {
            public _decimalValue(__GeneratedOdataV3.CstNodes.Rules._decimalValue _decimalValue_1)
            {
                this._decimalValue_1 = _decimalValue_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._decimalValue _decimalValue_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _doubleValue : _primitiveValue
        {
            public _doubleValue(__GeneratedOdataV3.CstNodes.Rules._doubleValue _doubleValue_1)
            {
                this._doubleValue_1 = _doubleValue_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._doubleValue _doubleValue_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _singleValue : _primitiveValue
        {
            public _singleValue(__GeneratedOdataV3.CstNodes.Rules._singleValue _singleValue_1)
            {
                this._singleValue_1 = _singleValue_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._singleValue _singleValue_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _sbyteValue : _primitiveValue
        {
            public _sbyteValue(__GeneratedOdataV3.CstNodes.Rules._sbyteValue _sbyteValue_1)
            {
                this._sbyteValue_1 = _sbyteValue_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._sbyteValue _sbyteValue_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _byteValue : _primitiveValue
        {
            public _byteValue(__GeneratedOdataV3.CstNodes.Rules._byteValue _byteValue_1)
            {
                this._byteValue_1 = _byteValue_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._byteValue _byteValue_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _int16Value : _primitiveValue
        {
            public _int16Value(__GeneratedOdataV3.CstNodes.Rules._int16Value _int16Value_1)
            {
                this._int16Value_1 = _int16Value_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._int16Value _int16Value_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _int32Value : _primitiveValue
        {
            public _int32Value(__GeneratedOdataV3.CstNodes.Rules._int32Value _int32Value_1)
            {
                this._int32Value_1 = _int32Value_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._int32Value _int32Value_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _int64Value : _primitiveValue
        {
            public _int64Value(__GeneratedOdataV3.CstNodes.Rules._int64Value _int64Value_1)
            {
                this._int64Value_1 = _int64Value_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._int64Value _int64Value_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _binaryValue : _primitiveValue
        {
            public _binaryValue(__GeneratedOdataV3.CstNodes.Rules._binaryValue _binaryValue_1)
            {
                this._binaryValue_1 = _binaryValue_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._binaryValue _binaryValue_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
