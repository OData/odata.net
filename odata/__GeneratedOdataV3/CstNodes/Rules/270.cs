namespace __GeneratedOdataV3.CstNodes.Rules
{
    public abstract class _primitiveLiteral
    {
        private _primitiveLiteral()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_primitiveLiteral node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_primitiveLiteral._nullValue node, TContext context);
            protected internal abstract TResult Accept(_primitiveLiteral._booleanValue node, TContext context);
            protected internal abstract TResult Accept(_primitiveLiteral._guidValue node, TContext context);
            protected internal abstract TResult Accept(_primitiveLiteral._dateValue node, TContext context);
            protected internal abstract TResult Accept(_primitiveLiteral._dateTimeOffsetValue node, TContext context);
            protected internal abstract TResult Accept(_primitiveLiteral._timeOfDayValue node, TContext context);
            protected internal abstract TResult Accept(_primitiveLiteral._decimalValue node, TContext context);
            protected internal abstract TResult Accept(_primitiveLiteral._doubleValue node, TContext context);
            protected internal abstract TResult Accept(_primitiveLiteral._singleValue node, TContext context);
            protected internal abstract TResult Accept(_primitiveLiteral._sbyteValue node, TContext context);
            protected internal abstract TResult Accept(_primitiveLiteral._byteValue node, TContext context);
            protected internal abstract TResult Accept(_primitiveLiteral._int16Value node, TContext context);
            protected internal abstract TResult Accept(_primitiveLiteral._int32Value node, TContext context);
            protected internal abstract TResult Accept(_primitiveLiteral._int64Value node, TContext context);
            protected internal abstract TResult Accept(_primitiveLiteral._string node, TContext context);
            protected internal abstract TResult Accept(_primitiveLiteral._duration node, TContext context);
            protected internal abstract TResult Accept(_primitiveLiteral._enum node, TContext context);
            protected internal abstract TResult Accept(_primitiveLiteral._binary node, TContext context);
            protected internal abstract TResult Accept(_primitiveLiteral._geographyCollection node, TContext context);
            protected internal abstract TResult Accept(_primitiveLiteral._geographyLineString node, TContext context);
            protected internal abstract TResult Accept(_primitiveLiteral._geographyMultiLineString node, TContext context);
            protected internal abstract TResult Accept(_primitiveLiteral._geographyMultiPoint node, TContext context);
            protected internal abstract TResult Accept(_primitiveLiteral._geographyMultiPolygon node, TContext context);
            protected internal abstract TResult Accept(_primitiveLiteral._geographyPoint node, TContext context);
            protected internal abstract TResult Accept(_primitiveLiteral._geographyPolygon node, TContext context);
            protected internal abstract TResult Accept(_primitiveLiteral._geometryCollection node, TContext context);
            protected internal abstract TResult Accept(_primitiveLiteral._geometryLineString node, TContext context);
            protected internal abstract TResult Accept(_primitiveLiteral._geometryMultiLineString node, TContext context);
            protected internal abstract TResult Accept(_primitiveLiteral._geometryMultiPoint node, TContext context);
            protected internal abstract TResult Accept(_primitiveLiteral._geometryMultiPolygon node, TContext context);
            protected internal abstract TResult Accept(_primitiveLiteral._geometryPoint node, TContext context);
            protected internal abstract TResult Accept(_primitiveLiteral._geometryPolygon node, TContext context);
        }
        
        public sealed class _nullValue : _primitiveLiteral
        {
            public _nullValue(__GeneratedOdataV3.CstNodes.Rules._nullValue _nullValue_1)
            {
                this._nullValue_1 = _nullValue_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._nullValue _nullValue_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _booleanValue : _primitiveLiteral
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
        
        public sealed class _guidValue : _primitiveLiteral
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
        
        public sealed class _dateValue : _primitiveLiteral
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
        
        public sealed class _dateTimeOffsetValue : _primitiveLiteral
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
        
        public sealed class _timeOfDayValue : _primitiveLiteral
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
        
        public sealed class _decimalValue : _primitiveLiteral
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
        
        public sealed class _doubleValue : _primitiveLiteral
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
        
        public sealed class _singleValue : _primitiveLiteral
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
        
        public sealed class _sbyteValue : _primitiveLiteral
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
        
        public sealed class _byteValue : _primitiveLiteral
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
        
        public sealed class _int16Value : _primitiveLiteral
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
        
        public sealed class _int32Value : _primitiveLiteral
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
        
        public sealed class _int64Value : _primitiveLiteral
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
        
        public sealed class _string : _primitiveLiteral
        {
            public _string(__GeneratedOdataV3.CstNodes.Rules._string _string_1)
            {
                this._string_1 = _string_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._string _string_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _duration : _primitiveLiteral
        {
            public _duration(__GeneratedOdataV3.CstNodes.Rules._duration _duration_1)
            {
                this._duration_1 = _duration_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._duration _duration_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _enum : _primitiveLiteral
        {
            public _enum(__GeneratedOdataV3.CstNodes.Rules._enum _enum_1)
            {
                this._enum_1 = _enum_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._enum _enum_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _binary : _primitiveLiteral
        {
            public _binary(__GeneratedOdataV3.CstNodes.Rules._binary _binary_1)
            {
                this._binary_1 = _binary_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._binary _binary_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _geographyCollection : _primitiveLiteral
        {
            public _geographyCollection(__GeneratedOdataV3.CstNodes.Rules._geographyCollection _geographyCollection_1)
            {
                this._geographyCollection_1 = _geographyCollection_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._geographyCollection _geographyCollection_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _geographyLineString : _primitiveLiteral
        {
            public _geographyLineString(__GeneratedOdataV3.CstNodes.Rules._geographyLineString _geographyLineString_1)
            {
                this._geographyLineString_1 = _geographyLineString_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._geographyLineString _geographyLineString_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _geographyMultiLineString : _primitiveLiteral
        {
            public _geographyMultiLineString(__GeneratedOdataV3.CstNodes.Rules._geographyMultiLineString _geographyMultiLineString_1)
            {
                this._geographyMultiLineString_1 = _geographyMultiLineString_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._geographyMultiLineString _geographyMultiLineString_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _geographyMultiPoint : _primitiveLiteral
        {
            public _geographyMultiPoint(__GeneratedOdataV3.CstNodes.Rules._geographyMultiPoint _geographyMultiPoint_1)
            {
                this._geographyMultiPoint_1 = _geographyMultiPoint_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._geographyMultiPoint _geographyMultiPoint_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _geographyMultiPolygon : _primitiveLiteral
        {
            public _geographyMultiPolygon(__GeneratedOdataV3.CstNodes.Rules._geographyMultiPolygon _geographyMultiPolygon_1)
            {
                this._geographyMultiPolygon_1 = _geographyMultiPolygon_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._geographyMultiPolygon _geographyMultiPolygon_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _geographyPoint : _primitiveLiteral
        {
            public _geographyPoint(__GeneratedOdataV3.CstNodes.Rules._geographyPoint _geographyPoint_1)
            {
                this._geographyPoint_1 = _geographyPoint_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._geographyPoint _geographyPoint_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _geographyPolygon : _primitiveLiteral
        {
            public _geographyPolygon(__GeneratedOdataV3.CstNodes.Rules._geographyPolygon _geographyPolygon_1)
            {
                this._geographyPolygon_1 = _geographyPolygon_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._geographyPolygon _geographyPolygon_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _geometryCollection : _primitiveLiteral
        {
            public _geometryCollection(__GeneratedOdataV3.CstNodes.Rules._geometryCollection _geometryCollection_1)
            {
                this._geometryCollection_1 = _geometryCollection_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._geometryCollection _geometryCollection_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _geometryLineString : _primitiveLiteral
        {
            public _geometryLineString(__GeneratedOdataV3.CstNodes.Rules._geometryLineString _geometryLineString_1)
            {
                this._geometryLineString_1 = _geometryLineString_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._geometryLineString _geometryLineString_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _geometryMultiLineString : _primitiveLiteral
        {
            public _geometryMultiLineString(__GeneratedOdataV3.CstNodes.Rules._geometryMultiLineString _geometryMultiLineString_1)
            {
                this._geometryMultiLineString_1 = _geometryMultiLineString_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._geometryMultiLineString _geometryMultiLineString_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _geometryMultiPoint : _primitiveLiteral
        {
            public _geometryMultiPoint(__GeneratedOdataV3.CstNodes.Rules._geometryMultiPoint _geometryMultiPoint_1)
            {
                this._geometryMultiPoint_1 = _geometryMultiPoint_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._geometryMultiPoint _geometryMultiPoint_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _geometryMultiPolygon : _primitiveLiteral
        {
            public _geometryMultiPolygon(__GeneratedOdataV3.CstNodes.Rules._geometryMultiPolygon _geometryMultiPolygon_1)
            {
                this._geometryMultiPolygon_1 = _geometryMultiPolygon_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._geometryMultiPolygon _geometryMultiPolygon_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _geometryPoint : _primitiveLiteral
        {
            public _geometryPoint(__GeneratedOdataV3.CstNodes.Rules._geometryPoint _geometryPoint_1)
            {
                this._geometryPoint_1 = _geometryPoint_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._geometryPoint _geometryPoint_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _geometryPolygon : _primitiveLiteral
        {
            public _geometryPolygon(__GeneratedOdataV3.CstNodes.Rules._geometryPolygon _geometryPolygon_1)
            {
                this._geometryPolygon_1 = _geometryPolygon_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._geometryPolygon _geometryPolygon_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
