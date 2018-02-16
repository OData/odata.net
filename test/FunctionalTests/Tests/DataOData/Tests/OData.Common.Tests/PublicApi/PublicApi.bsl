public enum Microsoft.Spatial.SpatialType : byte {
	Collection = 7
	FullGlobe = 11
	LineString = 2
	MultiLineString = 5
	MultiPoint = 4
	MultiPolygon = 6
	Point = 1
	Polygon = 3
	Unknown = 0
}

public interface Microsoft.Spatial.IGeographyProvider {
	Microsoft.Spatial.Geography ConstructedGeography  { public abstract get; }

	System.Action`1[[Microsoft.Spatial.Geography]] ProduceGeography {public abstract add;public abstract remove; }
}

public interface Microsoft.Spatial.IGeoJsonWriter {
	void AddPropertyName (string name)
	void AddValue (double value)
	void AddValue (string value)
	void EndArrayScope ()
	void EndObjectScope ()
	void StartArrayScope ()
	void StartObjectScope ()
}

public interface Microsoft.Spatial.IGeometryProvider {
	Microsoft.Spatial.Geometry ConstructedGeometry  { public abstract get; }

	System.Action`1[[Microsoft.Spatial.Geometry]] ProduceGeometry {public abstract add;public abstract remove; }
}

public interface Microsoft.Spatial.IShapeProvider : IGeographyProvider, IGeometryProvider {
}

public interface Microsoft.Spatial.ISpatial {
	Microsoft.Spatial.CoordinateSystem CoordinateSystem  { public abstract get; }
	bool IsEmpty  { public abstract get; }
}

public abstract class Microsoft.Spatial.Geography : ISpatial {
	protected Geography (Microsoft.Spatial.CoordinateSystem coordinateSystem, Microsoft.Spatial.SpatialImplementation creator)

	Microsoft.Spatial.CoordinateSystem CoordinateSystem  { public virtual get; }
	bool IsEmpty  { public abstract get; }

	public virtual void SendTo (Microsoft.Spatial.GeographyPipeline chain)
}

public abstract class Microsoft.Spatial.GeographyCollection : Microsoft.Spatial.Geography, ISpatial {
	protected GeographyCollection (Microsoft.Spatial.CoordinateSystem coordinateSystem, Microsoft.Spatial.SpatialImplementation creator)

	System.Collections.ObjectModel.ReadOnlyCollection`1[[Microsoft.Spatial.Geography]] Geographies  { public abstract get; }

	public bool Equals (Microsoft.Spatial.GeographyCollection other)
	public virtual bool Equals (object obj)
	public virtual int GetHashCode ()
}

public abstract class Microsoft.Spatial.GeographyCurve : Microsoft.Spatial.Geography, ISpatial {
	protected GeographyCurve (Microsoft.Spatial.CoordinateSystem coordinateSystem, Microsoft.Spatial.SpatialImplementation creator)
}

public abstract class Microsoft.Spatial.GeographyFullGlobe : Microsoft.Spatial.GeographySurface, ISpatial {
	protected GeographyFullGlobe (Microsoft.Spatial.CoordinateSystem coordinateSystem, Microsoft.Spatial.SpatialImplementation creator)

	public bool Equals (Microsoft.Spatial.GeographyFullGlobe other)
	public virtual bool Equals (object obj)
	public virtual int GetHashCode ()
}

public abstract class Microsoft.Spatial.GeographyLineString : Microsoft.Spatial.GeographyCurve, ISpatial {
	protected GeographyLineString (Microsoft.Spatial.CoordinateSystem coordinateSystem, Microsoft.Spatial.SpatialImplementation creator)

	System.Collections.ObjectModel.ReadOnlyCollection`1[[Microsoft.Spatial.GeographyPoint]] Points  { public abstract get; }

	public bool Equals (Microsoft.Spatial.GeographyLineString other)
	public virtual bool Equals (object obj)
	public virtual int GetHashCode ()
}

public abstract class Microsoft.Spatial.GeographyMultiCurve : Microsoft.Spatial.GeographyCollection, ISpatial {
	protected GeographyMultiCurve (Microsoft.Spatial.CoordinateSystem coordinateSystem, Microsoft.Spatial.SpatialImplementation creator)
}

public abstract class Microsoft.Spatial.GeographyMultiLineString : Microsoft.Spatial.GeographyMultiCurve, ISpatial {
	protected GeographyMultiLineString (Microsoft.Spatial.CoordinateSystem coordinateSystem, Microsoft.Spatial.SpatialImplementation creator)

	System.Collections.ObjectModel.ReadOnlyCollection`1[[Microsoft.Spatial.GeographyLineString]] LineStrings  { public abstract get; }

	public bool Equals (Microsoft.Spatial.GeographyMultiLineString other)
	public virtual bool Equals (object obj)
	public virtual int GetHashCode ()
}

public abstract class Microsoft.Spatial.GeographyMultiPoint : Microsoft.Spatial.GeographyCollection, ISpatial {
	protected GeographyMultiPoint (Microsoft.Spatial.CoordinateSystem coordinateSystem, Microsoft.Spatial.SpatialImplementation creator)

	System.Collections.ObjectModel.ReadOnlyCollection`1[[Microsoft.Spatial.GeographyPoint]] Points  { public abstract get; }

	public bool Equals (Microsoft.Spatial.GeographyMultiPoint other)
	public virtual bool Equals (object obj)
	public virtual int GetHashCode ()
}

public abstract class Microsoft.Spatial.GeographyMultiPolygon : Microsoft.Spatial.GeographyMultiSurface, ISpatial {
	protected GeographyMultiPolygon (Microsoft.Spatial.CoordinateSystem coordinateSystem, Microsoft.Spatial.SpatialImplementation creator)

	System.Collections.ObjectModel.ReadOnlyCollection`1[[Microsoft.Spatial.GeographyPolygon]] Polygons  { public abstract get; }

	public bool Equals (Microsoft.Spatial.GeographyMultiPolygon other)
	public virtual bool Equals (object obj)
	public virtual int GetHashCode ()
}

public abstract class Microsoft.Spatial.GeographyMultiSurface : Microsoft.Spatial.GeographyCollection, ISpatial {
	protected GeographyMultiSurface (Microsoft.Spatial.CoordinateSystem coordinateSystem, Microsoft.Spatial.SpatialImplementation creator)
}

public abstract class Microsoft.Spatial.GeographyPipeline {
	protected GeographyPipeline ()

	public abstract void BeginFigure (Microsoft.Spatial.GeographyPosition position)
	public abstract void BeginGeography (Microsoft.Spatial.SpatialType type)
	public abstract void EndFigure ()
	public abstract void EndGeography ()
	public abstract void LineTo (Microsoft.Spatial.GeographyPosition position)
	public abstract void Reset ()
	public abstract void SetCoordinateSystem (Microsoft.Spatial.CoordinateSystem coordinateSystem)
}

public abstract class Microsoft.Spatial.GeographyPoint : Microsoft.Spatial.Geography, ISpatial {
	protected GeographyPoint (Microsoft.Spatial.CoordinateSystem coordinateSystem, Microsoft.Spatial.SpatialImplementation creator)

	double Latitude  { public abstract get; }
	double Longitude  { public abstract get; }
	System.Nullable`1[[System.Double]] M  { public abstract get; }
	System.Nullable`1[[System.Double]] Z  { public abstract get; }

	public static Microsoft.Spatial.GeographyPoint Create (double latitude, double longitude)
	public static Microsoft.Spatial.GeographyPoint Create (double latitude, double longitude, System.Nullable`1[[System.Double]] z)
	public static Microsoft.Spatial.GeographyPoint Create (double latitude, double longitude, System.Nullable`1[[System.Double]] z, System.Nullable`1[[System.Double]] m)
	public static Microsoft.Spatial.GeographyPoint Create (Microsoft.Spatial.CoordinateSystem coordinateSystem, double latitude, double longitude, System.Nullable`1[[System.Double]] z, System.Nullable`1[[System.Double]] m)
	public bool Equals (Microsoft.Spatial.GeographyPoint other)
	public virtual bool Equals (object obj)
	public virtual int GetHashCode ()
}

public abstract class Microsoft.Spatial.GeographyPolygon : Microsoft.Spatial.GeographySurface, ISpatial {
	protected GeographyPolygon (Microsoft.Spatial.CoordinateSystem coordinateSystem, Microsoft.Spatial.SpatialImplementation creator)

	System.Collections.ObjectModel.ReadOnlyCollection`1[[Microsoft.Spatial.GeographyLineString]] Rings  { public abstract get; }

	public bool Equals (Microsoft.Spatial.GeographyPolygon other)
	public virtual bool Equals (object obj)
	public virtual int GetHashCode ()
}

public abstract class Microsoft.Spatial.GeographySurface : Microsoft.Spatial.Geography, ISpatial {
	protected GeographySurface (Microsoft.Spatial.CoordinateSystem coordinateSystem, Microsoft.Spatial.SpatialImplementation creator)
}

public abstract class Microsoft.Spatial.GeoJsonObjectFormatter {
	protected GeoJsonObjectFormatter ()

	public static Microsoft.Spatial.GeoJsonObjectFormatter Create ()
	public abstract Microsoft.Spatial.SpatialPipeline CreateWriter (Microsoft.Spatial.IGeoJsonWriter writer)
	public abstract T Read (System.Collections.Generic.IDictionary`2[[System.String],[System.Object]] source)
	public abstract System.Collections.Generic.IDictionary`2[[System.String],[System.Object]] Write (Microsoft.Spatial.ISpatial value)
}

public abstract class Microsoft.Spatial.Geometry : ISpatial {
	protected Geometry (Microsoft.Spatial.CoordinateSystem coordinateSystem, Microsoft.Spatial.SpatialImplementation creator)

	Microsoft.Spatial.CoordinateSystem CoordinateSystem  { public virtual get; }
	bool IsEmpty  { public abstract get; }

	public virtual void SendTo (Microsoft.Spatial.GeometryPipeline chain)
}

public abstract class Microsoft.Spatial.GeometryCollection : Microsoft.Spatial.Geometry, ISpatial {
	protected GeometryCollection (Microsoft.Spatial.CoordinateSystem coordinateSystem, Microsoft.Spatial.SpatialImplementation creator)

	System.Collections.ObjectModel.ReadOnlyCollection`1[[Microsoft.Spatial.Geometry]] Geometries  { public abstract get; }

	public bool Equals (Microsoft.Spatial.GeometryCollection other)
	public virtual bool Equals (object obj)
	public virtual int GetHashCode ()
}

public abstract class Microsoft.Spatial.GeometryCurve : Microsoft.Spatial.Geometry, ISpatial {
	protected GeometryCurve (Microsoft.Spatial.CoordinateSystem coordinateSystem, Microsoft.Spatial.SpatialImplementation creator)
}

public abstract class Microsoft.Spatial.GeometryLineString : Microsoft.Spatial.GeometryCurve, ISpatial {
	protected GeometryLineString (Microsoft.Spatial.CoordinateSystem coordinateSystem, Microsoft.Spatial.SpatialImplementation creator)

	System.Collections.ObjectModel.ReadOnlyCollection`1[[Microsoft.Spatial.GeometryPoint]] Points  { public abstract get; }

	public bool Equals (Microsoft.Spatial.GeometryLineString other)
	public virtual bool Equals (object obj)
	public virtual int GetHashCode ()
}

public abstract class Microsoft.Spatial.GeometryMultiCurve : Microsoft.Spatial.GeometryCollection, ISpatial {
	protected GeometryMultiCurve (Microsoft.Spatial.CoordinateSystem coordinateSystem, Microsoft.Spatial.SpatialImplementation creator)
}

public abstract class Microsoft.Spatial.GeometryMultiLineString : Microsoft.Spatial.GeometryMultiCurve, ISpatial {
	protected GeometryMultiLineString (Microsoft.Spatial.CoordinateSystem coordinateSystem, Microsoft.Spatial.SpatialImplementation creator)

	System.Collections.ObjectModel.ReadOnlyCollection`1[[Microsoft.Spatial.GeometryLineString]] LineStrings  { public abstract get; }

	public bool Equals (Microsoft.Spatial.GeometryMultiLineString other)
	public virtual bool Equals (object obj)
	public virtual int GetHashCode ()
}

public abstract class Microsoft.Spatial.GeometryMultiPoint : Microsoft.Spatial.GeometryCollection, ISpatial {
	protected GeometryMultiPoint (Microsoft.Spatial.CoordinateSystem coordinateSystem, Microsoft.Spatial.SpatialImplementation creator)

	System.Collections.ObjectModel.ReadOnlyCollection`1[[Microsoft.Spatial.GeometryPoint]] Points  { public abstract get; }

	public bool Equals (Microsoft.Spatial.GeometryMultiPoint other)
	public virtual bool Equals (object obj)
	public virtual int GetHashCode ()
}

public abstract class Microsoft.Spatial.GeometryMultiPolygon : Microsoft.Spatial.GeometryMultiSurface, ISpatial {
	protected GeometryMultiPolygon (Microsoft.Spatial.CoordinateSystem coordinateSystem, Microsoft.Spatial.SpatialImplementation creator)

	System.Collections.ObjectModel.ReadOnlyCollection`1[[Microsoft.Spatial.GeometryPolygon]] Polygons  { public abstract get; }

	public bool Equals (Microsoft.Spatial.GeometryMultiPolygon other)
	public virtual bool Equals (object obj)
	public virtual int GetHashCode ()
}

public abstract class Microsoft.Spatial.GeometryMultiSurface : Microsoft.Spatial.GeometryCollection, ISpatial {
}

public abstract class Microsoft.Spatial.GeometryPipeline {
	protected GeometryPipeline ()

	public abstract void BeginFigure (Microsoft.Spatial.GeometryPosition position)
	public abstract void BeginGeometry (Microsoft.Spatial.SpatialType type)
	public abstract void EndFigure ()
	public abstract void EndGeometry ()
	public abstract void LineTo (Microsoft.Spatial.GeometryPosition position)
	public abstract void Reset ()
	public abstract void SetCoordinateSystem (Microsoft.Spatial.CoordinateSystem coordinateSystem)
}

public abstract class Microsoft.Spatial.GeometryPoint : Microsoft.Spatial.Geometry, ISpatial {
	protected GeometryPoint (Microsoft.Spatial.CoordinateSystem coordinateSystem, Microsoft.Spatial.SpatialImplementation creator)

	System.Nullable`1[[System.Double]] M  { public abstract get; }
	double X  { public abstract get; }
	double Y  { public abstract get; }
	System.Nullable`1[[System.Double]] Z  { public abstract get; }

	public static Microsoft.Spatial.GeometryPoint Create (double x, double y)
	public static Microsoft.Spatial.GeometryPoint Create (double x, double y, System.Nullable`1[[System.Double]] z)
	public static Microsoft.Spatial.GeometryPoint Create (double x, double y, System.Nullable`1[[System.Double]] z, System.Nullable`1[[System.Double]] m)
	public static Microsoft.Spatial.GeometryPoint Create (Microsoft.Spatial.CoordinateSystem coordinateSystem, double x, double y, System.Nullable`1[[System.Double]] z, System.Nullable`1[[System.Double]] m)
	public bool Equals (Microsoft.Spatial.GeometryPoint other)
	public virtual bool Equals (object obj)
	public virtual int GetHashCode ()
}

public abstract class Microsoft.Spatial.GeometryPolygon : Microsoft.Spatial.GeometrySurface, ISpatial {
	protected GeometryPolygon (Microsoft.Spatial.CoordinateSystem coordinateSystem, Microsoft.Spatial.SpatialImplementation creator)

	System.Collections.ObjectModel.ReadOnlyCollection`1[[Microsoft.Spatial.GeometryLineString]] Rings  { public abstract get; }

	public bool Equals (Microsoft.Spatial.GeometryPolygon other)
	public virtual bool Equals (object obj)
	public virtual int GetHashCode ()
}

public abstract class Microsoft.Spatial.GeometrySurface : Microsoft.Spatial.Geometry, ISpatial {
}

public abstract class Microsoft.Spatial.GmlFormatter : Microsoft.Spatial.SpatialFormatter`2[[System.Xml.XmlReader],[System.Xml.XmlWriter]] {
	protected GmlFormatter (Microsoft.Spatial.SpatialImplementation creator)

	public static Microsoft.Spatial.GmlFormatter Create ()
}

public abstract class Microsoft.Spatial.SpatialFormatter`2 {
	protected SpatialFormatter`2 (Microsoft.Spatial.SpatialImplementation creator)

	public abstract Microsoft.Spatial.SpatialPipeline CreateWriter (TWriterStream writerStream)
	protected System.Collections.Generic.KeyValuePair`2[[Microsoft.Spatial.SpatialPipeline],[Microsoft.Spatial.IShapeProvider]] MakeValidatingBuilder ()
	public TResult Read (TReaderStream input)
	public void Read (TReaderStream input, Microsoft.Spatial.SpatialPipeline pipeline)
	protected abstract void ReadGeography (TReaderStream readerStream, Microsoft.Spatial.SpatialPipeline pipeline)
	protected abstract void ReadGeometry (TReaderStream readerStream, Microsoft.Spatial.SpatialPipeline pipeline)
	public void Write (Microsoft.Spatial.ISpatial spatial, TWriterStream writerStream)
}

public abstract class Microsoft.Spatial.SpatialImplementation {
	protected SpatialImplementation ()

	Microsoft.Spatial.SpatialImplementation CurrentImplementation  { public static get; }
	Microsoft.Spatial.SpatialOperations Operations  { public abstract get; public abstract set; }

	public abstract Microsoft.Spatial.SpatialBuilder CreateBuilder ()
	public abstract Microsoft.Spatial.GeoJsonObjectFormatter CreateGeoJsonObjectFormatter ()
	public abstract Microsoft.Spatial.GmlFormatter CreateGmlFormatter ()
	public abstract Microsoft.Spatial.SpatialPipeline CreateValidator ()
	public abstract Microsoft.Spatial.WellKnownTextSqlFormatter CreateWellKnownTextSqlFormatter ()
	public abstract Microsoft.Spatial.WellKnownTextSqlFormatter CreateWellKnownTextSqlFormatter (bool allowOnlyTwoDimensions)
}

public abstract class Microsoft.Spatial.SpatialOperations {
	protected SpatialOperations ()

	public virtual double Distance (Microsoft.Spatial.Geography operand1, Microsoft.Spatial.Geography operand2)
	public virtual double Distance (Microsoft.Spatial.Geometry operand1, Microsoft.Spatial.Geometry operand2)
	public virtual bool Intersects (Microsoft.Spatial.Geography operand1, Microsoft.Spatial.Geography operand2)
	public virtual bool Intersects (Microsoft.Spatial.Geometry operand1, Microsoft.Spatial.Geometry operand2)
	public virtual double Length (Microsoft.Spatial.Geography operand)
	public virtual double Length (Microsoft.Spatial.Geometry operand)
}

public abstract class Microsoft.Spatial.WellKnownTextSqlFormatter : Microsoft.Spatial.SpatialFormatter`2[[System.IO.TextReader],[System.IO.TextWriter]] {
	protected WellKnownTextSqlFormatter (Microsoft.Spatial.SpatialImplementation creator)

	public static Microsoft.Spatial.WellKnownTextSqlFormatter Create ()
	public static Microsoft.Spatial.WellKnownTextSqlFormatter Create (bool allowOnlyTwoDimensions)
}

[
ExtensionAttribute(),
]
public sealed class Microsoft.Spatial.FormatterExtensions {
	[
	ExtensionAttribute(),
	]
	public static string Write (Microsoft.Spatial.SpatialFormatter`2[[System.IO.TextReader],[System.IO.TextWriter]] formatter, Microsoft.Spatial.ISpatial spatial)

	[
	ExtensionAttribute(),
	]
	public static string Write (Microsoft.Spatial.SpatialFormatter`2[[System.Xml.XmlReader],[System.Xml.XmlWriter]] formatter, Microsoft.Spatial.ISpatial spatial)
}

[
ExtensionAttribute(),
]
public sealed class Microsoft.Spatial.GeographyOperationsExtensions {
	[
	ExtensionAttribute(),
	]
	public static System.Nullable`1[[System.Double]] Distance (Microsoft.Spatial.Geography operand1, Microsoft.Spatial.Geography operand2)

	[
	ExtensionAttribute(),
	]
	public static System.Nullable`1[[System.Boolean]] Intersects (Microsoft.Spatial.Geography operand1, Microsoft.Spatial.Geography operand2)

	[
	ExtensionAttribute(),
	]
	public static System.Nullable`1[[System.Double]] Length (Microsoft.Spatial.Geography operand)
}

[
ExtensionAttribute(),
]
public sealed class Microsoft.Spatial.GeometryOperationsExtensions {
	[
	ExtensionAttribute(),
	]
	public static System.Nullable`1[[System.Double]] Distance (Microsoft.Spatial.Geometry operand1, Microsoft.Spatial.Geometry operand2)

	[
	ExtensionAttribute(),
	]
	public static System.Nullable`1[[System.Boolean]] Intersects (Microsoft.Spatial.Geometry operand1, Microsoft.Spatial.Geometry operand2)

	[
	ExtensionAttribute(),
	]
	public static System.Nullable`1[[System.Double]] Length (Microsoft.Spatial.Geometry operand)
}

[
ExtensionAttribute(),
]
public sealed class Microsoft.Spatial.SpatialTypeExtensions {
	[
	ExtensionAttribute(),
	]
	public static void SendTo (Microsoft.Spatial.ISpatial shape, Microsoft.Spatial.SpatialPipeline destination)
}

public sealed class Microsoft.Spatial.SpatialValidator {
	public static Microsoft.Spatial.SpatialPipeline Create ()
}

public class Microsoft.Spatial.CoordinateSystem {
	public static readonly Microsoft.Spatial.CoordinateSystem DefaultGeography = GeographyCoordinateSystem(EpsgId=4326)
	public static readonly Microsoft.Spatial.CoordinateSystem DefaultGeometry = GeometryCoordinateSystem(EpsgId=0)

	System.Nullable`1[[System.Int32]] EpsgId  { public get; }
	string Id  { public get; }
	string Name  { public get; }

	public bool Equals (Microsoft.Spatial.CoordinateSystem other)
	public virtual bool Equals (object obj)
	public static Microsoft.Spatial.CoordinateSystem Geography (System.Nullable`1[[System.Int32]] epsgId)
	public static Microsoft.Spatial.CoordinateSystem Geometry (System.Nullable`1[[System.Int32]] epsgId)
	public virtual int GetHashCode ()
	public virtual string ToString ()
	public string ToWktId ()
}

public class Microsoft.Spatial.GeographyPosition : IEquatable`1 {
	public GeographyPosition (double latitude, double longitude)
	public GeographyPosition (double latitude, double longitude, System.Nullable`1[[System.Double]] z, System.Nullable`1[[System.Double]] m)

	double Latitude  { public get; }
	double Longitude  { public get; }
	System.Nullable`1[[System.Double]] M  { public get; }
	System.Nullable`1[[System.Double]] Z  { public get; }

	public virtual bool Equals (Microsoft.Spatial.GeographyPosition other)
	public virtual bool Equals (object obj)
	public virtual int GetHashCode ()
	public virtual string ToString ()
}

public class Microsoft.Spatial.GeometryPosition : IEquatable`1 {
	public GeometryPosition (double x, double y)
	public GeometryPosition (double x, double y, System.Nullable`1[[System.Double]] z, System.Nullable`1[[System.Double]] m)

	System.Nullable`1[[System.Double]] M  { public get; }
	double X  { public get; }
	double Y  { public get; }
	System.Nullable`1[[System.Double]] Z  { public get; }

	public virtual bool Equals (Microsoft.Spatial.GeometryPosition other)
	public virtual bool Equals (object obj)
	public virtual int GetHashCode ()
	public virtual string ToString ()
}

public class Microsoft.Spatial.ParseErrorException : System.Exception, _Exception, ISerializable {
	public ParseErrorException ()
	public ParseErrorException (string message)
	public ParseErrorException (string message, System.Exception innerException)
}

public class Microsoft.Spatial.SpatialBuilder : Microsoft.Spatial.SpatialPipeline, IGeographyProvider, IGeometryProvider, IShapeProvider {
	public SpatialBuilder (Microsoft.Spatial.GeographyPipeline geographyInput, Microsoft.Spatial.GeometryPipeline geometryInput, Microsoft.Spatial.IGeographyProvider geographyOutput, Microsoft.Spatial.IGeometryProvider geometryOutput)

	Microsoft.Spatial.Geography ConstructedGeography  { public virtual get; }
	Microsoft.Spatial.Geometry ConstructedGeometry  { public virtual get; }

	System.Action`1[[Microsoft.Spatial.Geography]] ProduceGeography {public virtual add;public virtual remove; }
	System.Action`1[[Microsoft.Spatial.Geometry]] ProduceGeometry {public virtual add;public virtual remove; }

	public static Microsoft.Spatial.SpatialBuilder Create ()
}

public class Microsoft.Spatial.SpatialPipeline {
	public SpatialPipeline ()
	public SpatialPipeline (Microsoft.Spatial.GeographyPipeline geographyPipeline, Microsoft.Spatial.GeometryPipeline geometryPipeline)

	Microsoft.Spatial.GeographyPipeline GeographyPipeline  { public virtual get; }
	Microsoft.Spatial.GeometryPipeline GeometryPipeline  { public virtual get; }
	Microsoft.Spatial.SpatialPipeline StartingLink  { public get; public set; }

	public virtual Microsoft.Spatial.SpatialPipeline ChainTo (Microsoft.Spatial.SpatialPipeline destination)
}

public enum Microsoft.OData.Edm.EdmContainerElementKind : int {
	ActionImport = 2
	EntitySet = 1
	FunctionImport = 3
	None = 0
	Singleton = 4
}

public enum Microsoft.OData.Edm.EdmExpressionKind : int {
	AnnotationPath = 25
	BinaryConstant = 1
	BooleanConstant = 2
	Cast = 15
	Collection = 12
	DateConstant = 22
	DateTimeOffsetConstant = 3
	DecimalConstant = 4
	DurationConstant = 9
	EnumMember = 24
	FloatingConstant = 5
	FunctionApplication = 17
	GuidConstant = 6
	If = 14
	IntegerConstant = 7
	IsType = 16
	Labeled = 19
	LabeledExpressionReference = 18
	NavigationPropertyPath = 21
	None = 0
	Null = 10
	Path = 13
	PropertyPath = 20
	Record = 11
	StringConstant = 8
	TimeOfDayConstant = 23
}

public enum Microsoft.OData.Edm.EdmMultiplicity : int {
	Many = 3
	One = 2
	Unknown = 0
	ZeroOrOne = 1
}

public enum Microsoft.OData.Edm.EdmNavigationSourceKind : int {
	ContainedEntitySet = 3
	EntitySet = 1
	None = 0
	Singleton = 2
	UnknownEntitySet = 4
}

public enum Microsoft.OData.Edm.EdmOnDeleteAction : int {
	Cascade = 1
	None = 0
}

public enum Microsoft.OData.Edm.EdmPathTypeKind : int {
	AnnotationPath = 1
	NavigationPropertyPath = 3
	None = 0
	PropertyPath = 2
}

public enum Microsoft.OData.Edm.EdmPrimitiveTypeKind : int {
	Binary = 1
	Boolean = 2
	Byte = 3
	Date = 32
	DateTimeOffset = 4
	Decimal = 5
	Double = 6
	Duration = 15
	Geography = 16
	GeographyCollection = 20
	GeographyLineString = 18
	GeographyMultiLineString = 22
	GeographyMultiPoint = 23
	GeographyMultiPolygon = 21
	GeographyPoint = 17
	GeographyPolygon = 19
	Geometry = 24
	GeometryCollection = 28
	GeometryLineString = 26
	GeometryMultiLineString = 30
	GeometryMultiPoint = 31
	GeometryMultiPolygon = 29
	GeometryPoint = 25
	GeometryPolygon = 27
	Guid = 7
	Int16 = 8
	Int32 = 9
	Int64 = 10
	None = 0
	PrimitiveType = 34
	SByte = 11
	Single = 12
	Stream = 14
	String = 13
	TimeOfDay = 33
}

public enum Microsoft.OData.Edm.EdmPropertyKind : int {
	Navigation = 2
	None = 0
	Structural = 1
}

public enum Microsoft.OData.Edm.EdmSchemaElementKind : int {
	Action = 3
	EntityContainer = 4
	Function = 5
	None = 0
	Term = 2
	TypeDefinition = 1
}

public enum Microsoft.OData.Edm.EdmTypeKind : int {
	Collection = 4
	Complex = 3
	Entity = 2
	EntityReference = 5
	Enum = 6
	None = 0
	Path = 9
	Primitive = 1
	TypeDefinition = 7
	Untyped = 8
}

public struct Microsoft.OData.Edm.Date : IComparable, IComparable`1, IEquatable`1 {
	public static readonly Microsoft.OData.Edm.Date MaxValue = 9999-12-31
	public static readonly Microsoft.OData.Edm.Date MinValue = 0001-01-01

	public Date (int year, int month, int day)

	int Day  { public get; }
	int Month  { public get; }
	Microsoft.OData.Edm.Date Now  { public static get; }
	int Year  { public get; }

	public Microsoft.OData.Edm.Date AddDays (int value)
	public Microsoft.OData.Edm.Date AddMonths (int value)
	public Microsoft.OData.Edm.Date AddYears (int value)
	public virtual int CompareTo (Microsoft.OData.Edm.Date other)
	public virtual int CompareTo (object obj)
	public virtual bool Equals (Microsoft.OData.Edm.Date other)
	public virtual bool Equals (object obj)
	public virtual int GetHashCode ()
	public static Microsoft.OData.Edm.Date Parse (string text)
	public static Microsoft.OData.Edm.Date Parse (string text, System.IFormatProvider provider)
	public virtual string ToString ()
	public static bool TryParse (string text, out Microsoft.OData.Edm.Date& result)
	public static bool TryParse (string text, System.IFormatProvider provider, out Microsoft.OData.Edm.Date& result)
}

public struct Microsoft.OData.Edm.TimeOfDay : IComparable, IComparable`1, IEquatable`1 {
	public static long MaxTickValue = 863999999999
	public static readonly Microsoft.OData.Edm.TimeOfDay MaxValue = 23:59:59.9999999
	public static long MinTickValue = 0
	public static readonly Microsoft.OData.Edm.TimeOfDay MinValue = 00:00:00.0000000
	public static long TicksPerHour = 36000000000
	public static long TicksPerMinute = 600000000
	public static long TicksPerSecond = 10000000

	public TimeOfDay (long ticks)
	public TimeOfDay (int hour, int minute, int second, int millisecond)

	int Hours  { public get; }
	long Milliseconds  { public get; }
	int Minutes  { public get; }
	Microsoft.OData.Edm.TimeOfDay Now  { public static get; }
	int Seconds  { public get; }
	long Ticks  { public get; }

	public virtual int CompareTo (Microsoft.OData.Edm.TimeOfDay other)
	public virtual int CompareTo (object obj)
	public virtual bool Equals (Microsoft.OData.Edm.TimeOfDay other)
	public virtual bool Equals (object obj)
	public virtual int GetHashCode ()
	public static Microsoft.OData.Edm.TimeOfDay Parse (string text)
	public static Microsoft.OData.Edm.TimeOfDay Parse (string text, System.IFormatProvider provider)
	public virtual string ToString ()
	public static bool TryParse (string text, out Microsoft.OData.Edm.TimeOfDay& result)
	public static bool TryParse (string text, System.IFormatProvider provider, out Microsoft.OData.Edm.TimeOfDay& result)
}

public interface Microsoft.OData.Edm.IEdmAction : IEdmElement, IEdmNamedElement, IEdmOperation, IEdmSchemaElement, IEdmVocabularyAnnotatable {
}

public interface Microsoft.OData.Edm.IEdmActionImport : IEdmElement, IEdmEntityContainerElement, IEdmNamedElement, IEdmOperationImport, IEdmVocabularyAnnotatable {
	Microsoft.OData.Edm.IEdmAction Action  { public abstract get; }
}

public interface Microsoft.OData.Edm.IEdmBinaryTypeReference : IEdmElement, IEdmPrimitiveTypeReference, IEdmTypeReference {
	bool IsUnbounded  { public abstract get; }
	System.Nullable`1[[System.Int32]] MaxLength  { public abstract get; }
}

public interface Microsoft.OData.Edm.IEdmCheckable {
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Validation.EdmError]] Errors  { public abstract get; }
}

public interface Microsoft.OData.Edm.IEdmCollectionType : IEdmElement, IEdmType {
	Microsoft.OData.Edm.IEdmTypeReference ElementType  { public abstract get; }
}

public interface Microsoft.OData.Edm.IEdmCollectionTypeReference : IEdmElement, IEdmTypeReference {
}

public interface Microsoft.OData.Edm.IEdmComplexType : IEdmElement, IEdmNamedElement, IEdmSchemaElement, IEdmSchemaType, IEdmStructuredType, IEdmType, IEdmVocabularyAnnotatable {
}

public interface Microsoft.OData.Edm.IEdmComplexTypeReference : IEdmElement, IEdmStructuredTypeReference, IEdmTypeReference {
}

public interface Microsoft.OData.Edm.IEdmContainedEntitySet : IEdmElement, IEdmEntitySetBase, IEdmNamedElement, IEdmNavigationSource {
	Microsoft.OData.Edm.IEdmNavigationProperty NavigationProperty  { public abstract get; }
	Microsoft.OData.Edm.IEdmNavigationSource ParentNavigationSource  { public abstract get; }
}

public interface Microsoft.OData.Edm.IEdmDecimalTypeReference : IEdmElement, IEdmPrimitiveTypeReference, IEdmTypeReference {
	System.Nullable`1[[System.Int32]] Precision  { public abstract get; }
	System.Nullable`1[[System.Int32]] Scale  { public abstract get; }
}

public interface Microsoft.OData.Edm.IEdmElement {
}

public interface Microsoft.OData.Edm.IEdmEntityContainer : IEdmElement, IEdmNamedElement, IEdmSchemaElement, IEdmVocabularyAnnotatable {
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmEntityContainerElement]] Elements  { public abstract get; }

	Microsoft.OData.Edm.IEdmEntitySet FindEntitySet (string setName)
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmOperationImport]] FindOperationImports (string operationName)
	Microsoft.OData.Edm.IEdmSingleton FindSingleton (string singletonName)
}

public interface Microsoft.OData.Edm.IEdmEntityContainerElement : IEdmElement, IEdmNamedElement, IEdmVocabularyAnnotatable {
	Microsoft.OData.Edm.IEdmEntityContainer Container  { public abstract get; }
	Microsoft.OData.Edm.EdmContainerElementKind ContainerElementKind  { public abstract get; }
}

public interface Microsoft.OData.Edm.IEdmEntityReferenceType : IEdmElement, IEdmType {
	Microsoft.OData.Edm.IEdmEntityType EntityType  { public abstract get; }
}

public interface Microsoft.OData.Edm.IEdmEntityReferenceTypeReference : IEdmElement, IEdmTypeReference {
}

public interface Microsoft.OData.Edm.IEdmEntitySet : IEdmElement, IEdmEntityContainerElement, IEdmEntitySetBase, IEdmNamedElement, IEdmNavigationSource, IEdmVocabularyAnnotatable {
	bool IncludeInServiceDocument  { public abstract get; }
}

public interface Microsoft.OData.Edm.IEdmEntitySetBase : IEdmElement, IEdmNamedElement, IEdmNavigationSource {
}

public interface Microsoft.OData.Edm.IEdmEntityType : IEdmElement, IEdmNamedElement, IEdmSchemaElement, IEdmSchemaType, IEdmStructuredType, IEdmType, IEdmVocabularyAnnotatable {
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmStructuralProperty]] DeclaredKey  { public abstract get; }
	bool HasStream  { public abstract get; }
}

public interface Microsoft.OData.Edm.IEdmEntityTypeReference : IEdmElement, IEdmStructuredTypeReference, IEdmTypeReference {
}

public interface Microsoft.OData.Edm.IEdmEnumMember : IEdmElement, IEdmNamedElement, IEdmVocabularyAnnotatable {
	Microsoft.OData.Edm.IEdmEnumType DeclaringType  { public abstract get; }
	Microsoft.OData.Edm.IEdmEnumMemberValue Value  { public abstract get; }
}

public interface Microsoft.OData.Edm.IEdmEnumMemberValue : IEdmElement {
	long Value  { public abstract get; }
}

public interface Microsoft.OData.Edm.IEdmEnumType : IEdmElement, IEdmNamedElement, IEdmSchemaElement, IEdmSchemaType, IEdmType, IEdmVocabularyAnnotatable {
	bool IsFlags  { public abstract get; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmEnumMember]] Members  { public abstract get; }
	Microsoft.OData.Edm.IEdmPrimitiveType UnderlyingType  { public abstract get; }
}

public interface Microsoft.OData.Edm.IEdmEnumTypeReference : IEdmElement, IEdmTypeReference {
}

public interface Microsoft.OData.Edm.IEdmExpression : IEdmElement {
	Microsoft.OData.Edm.EdmExpressionKind ExpressionKind  { public abstract get; }
}

public interface Microsoft.OData.Edm.IEdmFunction : IEdmElement, IEdmNamedElement, IEdmOperation, IEdmSchemaElement, IEdmVocabularyAnnotatable {
	bool IsComposable  { public abstract get; }
}

public interface Microsoft.OData.Edm.IEdmFunctionImport : IEdmElement, IEdmEntityContainerElement, IEdmNamedElement, IEdmOperationImport, IEdmVocabularyAnnotatable {
	Microsoft.OData.Edm.IEdmFunction Function  { public abstract get; }
	bool IncludeInServiceDocument  { public abstract get; }
}

public interface Microsoft.OData.Edm.IEdmInclude {
	string Alias  { public abstract get; }
	string Namespace  { public abstract get; }
}

public interface Microsoft.OData.Edm.IEdmIncludeAnnotations {
	string Qualifier  { public abstract get; }
	string TargetNamespace  { public abstract get; }
	string TermNamespace  { public abstract get; }
}

public interface Microsoft.OData.Edm.IEdmLocatable {
	Microsoft.OData.Edm.EdmLocation Location  { public abstract get; }
}

public interface Microsoft.OData.Edm.IEdmModel : IEdmElement {
	System.Collections.Generic.IEnumerable`1[[System.String]] DeclaredNamespaces  { public abstract get; }
	Microsoft.OData.Edm.Vocabularies.IEdmDirectValueAnnotationsManager DirectValueAnnotationsManager  { public abstract get; }
	Microsoft.OData.Edm.IEdmEntityContainer EntityContainer  { public abstract get; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmModel]] ReferencedModels  { public abstract get; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmSchemaElement]] SchemaElements  { public abstract get; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Vocabularies.IEdmVocabularyAnnotation]] VocabularyAnnotations  { public abstract get; }

	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmOperation]] FindDeclaredBoundOperations (Microsoft.OData.Edm.IEdmType bindingType)
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmOperation]] FindDeclaredBoundOperations (string qualifiedName, Microsoft.OData.Edm.IEdmType bindingType)
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmOperation]] FindDeclaredOperations (string qualifiedName)
	Microsoft.OData.Edm.Vocabularies.IEdmTerm FindDeclaredTerm (string qualifiedName)
	Microsoft.OData.Edm.IEdmSchemaType FindDeclaredType (string qualifiedName)
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Vocabularies.IEdmVocabularyAnnotation]] FindDeclaredVocabularyAnnotations (Microsoft.OData.Edm.Vocabularies.IEdmVocabularyAnnotatable element)
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmStructuredType]] FindDirectlyDerivedTypes (Microsoft.OData.Edm.IEdmStructuredType baseType)
}

public interface Microsoft.OData.Edm.IEdmNamedElement : IEdmElement {
	string Name  { public abstract get; }
}

public interface Microsoft.OData.Edm.IEdmNavigationProperty : IEdmElement, IEdmNamedElement, IEdmProperty, IEdmVocabularyAnnotatable {
	bool ContainsTarget  { public abstract get; }
	Microsoft.OData.Edm.EdmOnDeleteAction OnDelete  { public abstract get; }
	Microsoft.OData.Edm.IEdmNavigationProperty Partner  { public abstract get; }
	Microsoft.OData.Edm.IEdmReferentialConstraint ReferentialConstraint  { public abstract get; }
}

public interface Microsoft.OData.Edm.IEdmNavigationPropertyBinding {
	Microsoft.OData.Edm.IEdmNavigationProperty NavigationProperty  { public abstract get; }
	Microsoft.OData.Edm.IEdmPathExpression Path  { public abstract get; }
	Microsoft.OData.Edm.IEdmNavigationSource Target  { public abstract get; }
}

public interface Microsoft.OData.Edm.IEdmNavigationSource : IEdmElement, IEdmNamedElement {
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmNavigationPropertyBinding]] NavigationPropertyBindings  { public abstract get; }
	Microsoft.OData.Edm.IEdmPathExpression Path  { public abstract get; }
	Microsoft.OData.Edm.IEdmType Type  { public abstract get; }

	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmNavigationPropertyBinding]] FindNavigationPropertyBindings (Microsoft.OData.Edm.IEdmNavigationProperty navigationProperty)
	Microsoft.OData.Edm.IEdmNavigationSource FindNavigationTarget (Microsoft.OData.Edm.IEdmNavigationProperty navigationProperty)
	Microsoft.OData.Edm.IEdmNavigationSource FindNavigationTarget (Microsoft.OData.Edm.IEdmNavigationProperty navigationProperty, Microsoft.OData.Edm.IEdmPathExpression bindingPath)
}

public interface Microsoft.OData.Edm.IEdmOperation : IEdmElement, IEdmNamedElement, IEdmSchemaElement, IEdmVocabularyAnnotatable {
	Microsoft.OData.Edm.IEdmPathExpression EntitySetPath  { public abstract get; }
	bool IsBound  { public abstract get; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmOperationParameter]] Parameters  { public abstract get; }
	Microsoft.OData.Edm.IEdmTypeReference ReturnType  { public abstract get; }

	Microsoft.OData.Edm.IEdmOperationParameter FindParameter (string name)
}

public interface Microsoft.OData.Edm.IEdmOperationImport : IEdmElement, IEdmEntityContainerElement, IEdmNamedElement, IEdmVocabularyAnnotatable {
	Microsoft.OData.Edm.IEdmExpression EntitySet  { public abstract get; }
	Microsoft.OData.Edm.IEdmOperation Operation  { public abstract get; }
}

public interface Microsoft.OData.Edm.IEdmOperationParameter : IEdmElement, IEdmNamedElement, IEdmVocabularyAnnotatable {
	Microsoft.OData.Edm.IEdmOperation DeclaringOperation  { public abstract get; }
	Microsoft.OData.Edm.IEdmTypeReference Type  { public abstract get; }
}

public interface Microsoft.OData.Edm.IEdmOptionalParameter : IEdmElement, IEdmNamedElement, IEdmOperationParameter, IEdmVocabularyAnnotatable {
	string DefaultValueString  { public abstract get; }
}

public interface Microsoft.OData.Edm.IEdmPathExpression : IEdmElement, IEdmExpression {
	string Path  { public abstract get; }
	System.Collections.Generic.IEnumerable`1[[System.String]] PathSegments  { public abstract get; }
}

public interface Microsoft.OData.Edm.IEdmPathType : IEdmElement, IEdmNamedElement, IEdmSchemaElement, IEdmSchemaType, IEdmType, IEdmVocabularyAnnotatable {
	Microsoft.OData.Edm.EdmPathTypeKind PathKind  { public abstract get; }
}

public interface Microsoft.OData.Edm.IEdmPathTypeReference : IEdmElement, IEdmTypeReference {
}

public interface Microsoft.OData.Edm.IEdmPrimitiveType : IEdmElement, IEdmNamedElement, IEdmSchemaElement, IEdmSchemaType, IEdmType, IEdmVocabularyAnnotatable {
	Microsoft.OData.Edm.EdmPrimitiveTypeKind PrimitiveKind  { public abstract get; }
}

public interface Microsoft.OData.Edm.IEdmPrimitiveTypeReference : IEdmElement, IEdmTypeReference {
}

public interface Microsoft.OData.Edm.IEdmProperty : IEdmElement, IEdmNamedElement, IEdmVocabularyAnnotatable {
	Microsoft.OData.Edm.IEdmStructuredType DeclaringType  { public abstract get; }
	Microsoft.OData.Edm.EdmPropertyKind PropertyKind  { public abstract get; }
	Microsoft.OData.Edm.IEdmTypeReference Type  { public abstract get; }
}

public interface Microsoft.OData.Edm.IEdmReference : IEdmElement {
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmIncludeAnnotations]] IncludeAnnotations  { public abstract get; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmInclude]] Includes  { public abstract get; }
	System.Uri Uri  { public abstract get; }
}

public interface Microsoft.OData.Edm.IEdmReferentialConstraint {
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.EdmReferentialConstraintPropertyPair]] PropertyPairs  { public abstract get; }
}

public interface Microsoft.OData.Edm.IEdmSchemaElement : IEdmElement, IEdmNamedElement, IEdmVocabularyAnnotatable {
	string Namespace  { public abstract get; }
	Microsoft.OData.Edm.EdmSchemaElementKind SchemaElementKind  { public abstract get; }
}

public interface Microsoft.OData.Edm.IEdmSchemaType : IEdmElement, IEdmNamedElement, IEdmSchemaElement, IEdmType, IEdmVocabularyAnnotatable {
}

public interface Microsoft.OData.Edm.IEdmSingleton : IEdmElement, IEdmEntityContainerElement, IEdmNamedElement, IEdmNavigationSource, IEdmVocabularyAnnotatable {
}

public interface Microsoft.OData.Edm.IEdmSpatialTypeReference : IEdmElement, IEdmPrimitiveTypeReference, IEdmTypeReference {
	System.Nullable`1[[System.Int32]] SpatialReferenceIdentifier  { public abstract get; }
}

public interface Microsoft.OData.Edm.IEdmStringTypeReference : IEdmElement, IEdmPrimitiveTypeReference, IEdmTypeReference {
	bool IsUnbounded  { public abstract get; }
	System.Nullable`1[[System.Boolean]] IsUnicode  { public abstract get; }
	System.Nullable`1[[System.Int32]] MaxLength  { public abstract get; }
}

public interface Microsoft.OData.Edm.IEdmStructuralProperty : IEdmElement, IEdmNamedElement, IEdmProperty, IEdmVocabularyAnnotatable {
	string DefaultValueString  { public abstract get; }
}

public interface Microsoft.OData.Edm.IEdmStructuredType : IEdmElement, IEdmType {
	Microsoft.OData.Edm.IEdmStructuredType BaseType  { public abstract get; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmProperty]] DeclaredProperties  { public abstract get; }
	bool IsAbstract  { public abstract get; }
	bool IsOpen  { public abstract get; }

	Microsoft.OData.Edm.IEdmProperty FindProperty (string name)
}

public interface Microsoft.OData.Edm.IEdmStructuredTypeReference : IEdmElement, IEdmTypeReference {
}

public interface Microsoft.OData.Edm.IEdmTemporalTypeReference : IEdmElement, IEdmPrimitiveTypeReference, IEdmTypeReference {
	System.Nullable`1[[System.Int32]] Precision  { public abstract get; }
}

public interface Microsoft.OData.Edm.IEdmType : IEdmElement {
	Microsoft.OData.Edm.EdmTypeKind TypeKind  { public abstract get; }
}

public interface Microsoft.OData.Edm.IEdmTypeDefinition : IEdmElement, IEdmNamedElement, IEdmSchemaElement, IEdmSchemaType, IEdmType, IEdmVocabularyAnnotatable {
	Microsoft.OData.Edm.IEdmPrimitiveType UnderlyingType  { public abstract get; }
}

public interface Microsoft.OData.Edm.IEdmTypeDefinitionReference : IEdmElement, IEdmTypeReference {
	bool IsUnbounded  { public abstract get; }
	System.Nullable`1[[System.Boolean]] IsUnicode  { public abstract get; }
	System.Nullable`1[[System.Int32]] MaxLength  { public abstract get; }
	System.Nullable`1[[System.Int32]] Precision  { public abstract get; }
	System.Nullable`1[[System.Int32]] Scale  { public abstract get; }
	System.Nullable`1[[System.Int32]] SpatialReferenceIdentifier  { public abstract get; }
}

public interface Microsoft.OData.Edm.IEdmTypeReference : IEdmElement {
	Microsoft.OData.Edm.IEdmType Definition  { public abstract get; }
	bool IsNullable  { public abstract get; }
}

public interface Microsoft.OData.Edm.IEdmUnknownEntitySet : IEdmElement, IEdmEntitySetBase, IEdmNamedElement, IEdmNavigationSource {
}

public interface Microsoft.OData.Edm.IEdmUntypedType : IEdmElement, IEdmNamedElement, IEdmSchemaElement, IEdmSchemaType, IEdmType, IEdmVocabularyAnnotatable {
}

public interface Microsoft.OData.Edm.IEdmUntypedTypeReference : IEdmElement, IEdmTypeReference {
}

public interface Microsoft.OData.Edm.IPrimitiveValueConverter {
	object ConvertFromUnderlyingType (object value)
	object ConvertToUnderlyingType (object value)
}

public abstract class Microsoft.OData.Edm.EdmElement : IEdmElement {
	protected EdmElement ()
}

public abstract class Microsoft.OData.Edm.EdmEntitySetBase : Microsoft.OData.Edm.EdmNavigationSource, IEdmElement, IEdmEntitySetBase, IEdmNamedElement, IEdmNavigationSource {
	protected EdmEntitySetBase (string name, Microsoft.OData.Edm.IEdmEntityType elementType)

	Microsoft.OData.Edm.IEdmType Type  { public virtual get; }
}

public abstract class Microsoft.OData.Edm.EdmLocation {
	protected EdmLocation ()

	public abstract string ToString ()
}

public abstract class Microsoft.OData.Edm.EdmModelBase : Microsoft.OData.Edm.EdmElement, IEdmElement, IEdmModel {
	protected EdmModelBase (System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmModel]] referencedModels, Microsoft.OData.Edm.Vocabularies.IEdmDirectValueAnnotationsManager annotationsManager)

	System.Collections.Generic.IEnumerable`1[[System.String]] DeclaredNamespaces  { public abstract get; }
	Microsoft.OData.Edm.Vocabularies.IEdmDirectValueAnnotationsManager DirectValueAnnotationsManager  { public virtual get; }
	Microsoft.OData.Edm.IEdmEntityContainer EntityContainer  { public virtual get; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmModel]] ReferencedModels  { public virtual get; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmSchemaElement]] SchemaElements  { public abstract get; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Vocabularies.IEdmVocabularyAnnotation]] VocabularyAnnotations  { public virtual get; }

	protected void AddReferencedModel (Microsoft.OData.Edm.IEdmModel model)
	public virtual System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmOperation]] FindDeclaredBoundOperations (Microsoft.OData.Edm.IEdmType bindingType)
	public virtual System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmOperation]] FindDeclaredBoundOperations (string qualifiedName, Microsoft.OData.Edm.IEdmType bindingType)
	public virtual System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmOperation]] FindDeclaredOperations (string qualifiedName)
	public virtual Microsoft.OData.Edm.Vocabularies.IEdmTerm FindDeclaredTerm (string qualifiedName)
	public virtual Microsoft.OData.Edm.IEdmSchemaType FindDeclaredType (string qualifiedName)
	public virtual System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Vocabularies.IEdmVocabularyAnnotation]] FindDeclaredVocabularyAnnotations (Microsoft.OData.Edm.Vocabularies.IEdmVocabularyAnnotatable element)
	public abstract System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmStructuredType]] FindDirectlyDerivedTypes (Microsoft.OData.Edm.IEdmStructuredType baseType)
	protected void RegisterElement (Microsoft.OData.Edm.IEdmSchemaElement element)
}

[
DebuggerDisplayAttribute(),
]
public abstract class Microsoft.OData.Edm.EdmNamedElement : Microsoft.OData.Edm.EdmElement, IEdmElement, IEdmNamedElement {
	protected EdmNamedElement (string name)

	string Name  { public virtual get; }
}

public abstract class Microsoft.OData.Edm.EdmNavigationSource : Microsoft.OData.Edm.EdmNamedElement, IEdmElement, IEdmNamedElement, IEdmNavigationSource {
	protected EdmNavigationSource (string name)

	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmNavigationPropertyBinding]] NavigationPropertyBindings  { public virtual get; }
	Microsoft.OData.Edm.IEdmPathExpression Path  { public abstract get; }
	Microsoft.OData.Edm.IEdmType Type  { public abstract get; }

	public void AddNavigationTarget (Microsoft.OData.Edm.IEdmNavigationProperty navigationProperty, Microsoft.OData.Edm.IEdmNavigationSource target)
	public void AddNavigationTarget (Microsoft.OData.Edm.IEdmNavigationProperty navigationProperty, Microsoft.OData.Edm.IEdmNavigationSource target, Microsoft.OData.Edm.IEdmPathExpression bindingPath)
	public virtual System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmNavigationPropertyBinding]] FindNavigationPropertyBindings (Microsoft.OData.Edm.IEdmNavigationProperty navigationProperty)
	public virtual Microsoft.OData.Edm.IEdmNavigationSource FindNavigationTarget (Microsoft.OData.Edm.IEdmNavigationProperty navigationProperty)
	public virtual Microsoft.OData.Edm.IEdmNavigationSource FindNavigationTarget (Microsoft.OData.Edm.IEdmNavigationProperty navigationProperty, Microsoft.OData.Edm.IEdmPathExpression bindingPath)
}

public abstract class Microsoft.OData.Edm.EdmOperation : Microsoft.OData.Edm.EdmNamedElement, IEdmElement, IEdmNamedElement, IEdmOperation, IEdmSchemaElement, IEdmVocabularyAnnotatable {
	protected EdmOperation (string namespaceName, string name, Microsoft.OData.Edm.IEdmTypeReference returnType)
	protected EdmOperation (string namespaceName, string name, Microsoft.OData.Edm.IEdmTypeReference returnType, bool isBound, Microsoft.OData.Edm.IEdmPathExpression entitySetPathExpression)

	Microsoft.OData.Edm.IEdmPathExpression EntitySetPath  { public virtual get; }
	bool IsBound  { public virtual get; }
	string Namespace  { public virtual get; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmOperationParameter]] Parameters  { public virtual get; }
	Microsoft.OData.Edm.IEdmTypeReference ReturnType  { public virtual get; }
	Microsoft.OData.Edm.EdmSchemaElementKind SchemaElementKind  { public abstract get; }

	public Microsoft.OData.Edm.EdmOptionalParameter AddOptionalParameter (string name, Microsoft.OData.Edm.IEdmTypeReference type)
	public Microsoft.OData.Edm.EdmOptionalParameter AddOptionalParameter (string name, Microsoft.OData.Edm.IEdmTypeReference type, string defaultValue)
	public void AddParameter (Microsoft.OData.Edm.IEdmOperationParameter parameter)
	public Microsoft.OData.Edm.EdmOperationParameter AddParameter (string name, Microsoft.OData.Edm.IEdmTypeReference type)
	public virtual Microsoft.OData.Edm.IEdmOperationParameter FindParameter (string name)
}

public abstract class Microsoft.OData.Edm.EdmOperationImport : Microsoft.OData.Edm.EdmNamedElement, IEdmElement, IEdmEntityContainerElement, IEdmNamedElement, IEdmOperationImport, IEdmVocabularyAnnotatable {
	protected EdmOperationImport (Microsoft.OData.Edm.IEdmEntityContainer container, Microsoft.OData.Edm.IEdmOperation operation, string name, Microsoft.OData.Edm.IEdmExpression entitySet)

	Microsoft.OData.Edm.IEdmEntityContainer Container  { public virtual get; }
	Microsoft.OData.Edm.EdmContainerElementKind ContainerElementKind  { public abstract get; }
	Microsoft.OData.Edm.IEdmExpression EntitySet  { public virtual get; }
	Microsoft.OData.Edm.IEdmOperation Operation  { public virtual get; }

	protected abstract string OperationArgumentNullParameterName ()
}

public abstract class Microsoft.OData.Edm.EdmProperty : Microsoft.OData.Edm.EdmNamedElement, IEdmElement, IEdmNamedElement, IEdmProperty, IEdmVocabularyAnnotatable {
	protected EdmProperty (Microsoft.OData.Edm.IEdmStructuredType declaringType, string name, Microsoft.OData.Edm.IEdmTypeReference type)

	Microsoft.OData.Edm.IEdmStructuredType DeclaringType  { public virtual get; }
	Microsoft.OData.Edm.EdmPropertyKind PropertyKind  { public abstract get; }
	Microsoft.OData.Edm.IEdmTypeReference Type  { public virtual get; }
}

public abstract class Microsoft.OData.Edm.EdmStructuredType : Microsoft.OData.Edm.EdmType, IEdmElement, IEdmStructuredType, IEdmType {
	protected EdmStructuredType (bool isAbstract, bool isOpen, Microsoft.OData.Edm.IEdmStructuredType baseStructuredType)

	Microsoft.OData.Edm.IEdmStructuredType BaseType  { public virtual get; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmProperty]] DeclaredProperties  { public virtual get; }
	bool IsAbstract  { public virtual get; }
	bool IsOpen  { public virtual get; }
	System.Collections.Generic.IDictionary`2[[System.String],[Microsoft.OData.Edm.IEdmProperty]] PropertiesDictionary  { protected get; }

	public void AddProperty (Microsoft.OData.Edm.IEdmProperty property)
	public Microsoft.OData.Edm.EdmStructuralProperty AddStructuralProperty (string name, Microsoft.OData.Edm.EdmPrimitiveTypeKind type)
	public Microsoft.OData.Edm.EdmStructuralProperty AddStructuralProperty (string name, Microsoft.OData.Edm.IEdmTypeReference type)
	public Microsoft.OData.Edm.EdmStructuralProperty AddStructuralProperty (string name, Microsoft.OData.Edm.EdmPrimitiveTypeKind type, bool isNullable)
	public Microsoft.OData.Edm.EdmStructuralProperty AddStructuralProperty (string name, Microsoft.OData.Edm.IEdmTypeReference type, string defaultValue)
	public Microsoft.OData.Edm.EdmNavigationProperty AddUnidirectionalNavigation (Microsoft.OData.Edm.EdmNavigationPropertyInfo propertyInfo)
	public virtual Microsoft.OData.Edm.IEdmProperty FindProperty (string name)
}

public abstract class Microsoft.OData.Edm.EdmType : Microsoft.OData.Edm.EdmElement, IEdmElement, IEdmType {
	protected EdmType ()

	Microsoft.OData.Edm.EdmTypeKind TypeKind  { public abstract get; }

	public virtual string ToString ()
}

public abstract class Microsoft.OData.Edm.EdmTypeReference : Microsoft.OData.Edm.EdmElement, IEdmElement, IEdmTypeReference {
	protected EdmTypeReference (Microsoft.OData.Edm.IEdmType definition, bool isNullable)

	Microsoft.OData.Edm.IEdmType Definition  { public virtual get; }
	bool IsNullable  { public virtual get; }

	public virtual string ToString ()
}

public sealed class Microsoft.OData.Edm.EdmConstants {
	public static readonly System.Version EdmVersion4 = 4.0
	public static readonly System.Version EdmVersionLatest = 4.0
}

[
ExtensionAttribute(),
]
public sealed class Microsoft.OData.Edm.EdmElementComparer {
	[
	ExtensionAttribute(),
	]
	public static bool IsEquivalentTo (Microsoft.OData.Edm.IEdmType thisType, Microsoft.OData.Edm.IEdmType otherType)

	[
	ExtensionAttribute(),
	]
	public static bool IsEquivalentTo (Microsoft.OData.Edm.IEdmTypeReference thisType, Microsoft.OData.Edm.IEdmTypeReference otherType)
}

[
ExtensionAttribute(),
]
public sealed class Microsoft.OData.Edm.EdmTypeSemantics {
	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Edm.IEdmType AsActualType (Microsoft.OData.Edm.IEdmType type)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Edm.IEdmBinaryTypeReference AsBinary (Microsoft.OData.Edm.IEdmTypeReference type)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Edm.IEdmCollectionTypeReference AsCollection (Microsoft.OData.Edm.IEdmTypeReference type)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Edm.IEdmComplexTypeReference AsComplex (Microsoft.OData.Edm.IEdmTypeReference type)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Edm.IEdmDecimalTypeReference AsDecimal (Microsoft.OData.Edm.IEdmTypeReference type)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Edm.IEdmEntityTypeReference AsEntity (Microsoft.OData.Edm.IEdmTypeReference type)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Edm.IEdmEntityReferenceTypeReference AsEntityReference (Microsoft.OData.Edm.IEdmTypeReference type)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Edm.IEdmEnumTypeReference AsEnum (Microsoft.OData.Edm.IEdmTypeReference type)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Edm.IEdmPathTypeReference AsPath (Microsoft.OData.Edm.IEdmTypeReference type)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Edm.IEdmPrimitiveTypeReference AsPrimitive (Microsoft.OData.Edm.IEdmTypeReference type)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Edm.IEdmSpatialTypeReference AsSpatial (Microsoft.OData.Edm.IEdmTypeReference type)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Edm.IEdmStringTypeReference AsString (Microsoft.OData.Edm.IEdmTypeReference type)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Edm.IEdmStructuredTypeReference AsStructured (Microsoft.OData.Edm.IEdmTypeReference type)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Edm.IEdmTemporalTypeReference AsTemporal (Microsoft.OData.Edm.IEdmTypeReference type)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Edm.IEdmTypeDefinitionReference AsTypeDefinition (Microsoft.OData.Edm.IEdmTypeReference type)

	[
	ExtensionAttribute(),
	]
	public static bool InheritsFrom (Microsoft.OData.Edm.IEdmStructuredType type, Microsoft.OData.Edm.IEdmStructuredType potentialBaseType)

	[
	ExtensionAttribute(),
	]
	public static bool IsBinary (Microsoft.OData.Edm.IEdmTypeReference type)

	[
	ExtensionAttribute(),
	]
	public static bool IsBoolean (Microsoft.OData.Edm.IEdmTypeReference type)

	[
	ExtensionAttribute(),
	]
	public static bool IsByte (Microsoft.OData.Edm.IEdmTypeReference type)

	[
	ExtensionAttribute(),
	]
	public static bool IsCollection (Microsoft.OData.Edm.IEdmTypeReference type)

	[
	ExtensionAttribute(),
	]
	public static bool IsComplex (Microsoft.OData.Edm.IEdmTypeReference type)

	[
	ExtensionAttribute(),
	]
	public static bool IsDate (Microsoft.OData.Edm.IEdmTypeReference type)

	[
	ExtensionAttribute(),
	]
	public static bool IsDateTimeOffset (Microsoft.OData.Edm.IEdmTypeReference type)

	[
	ExtensionAttribute(),
	]
	public static bool IsDecimal (Microsoft.OData.Edm.IEdmType type)

	[
	ExtensionAttribute(),
	]
	public static bool IsDecimal (Microsoft.OData.Edm.IEdmTypeReference type)

	[
	ExtensionAttribute(),
	]
	public static bool IsDouble (Microsoft.OData.Edm.IEdmTypeReference type)

	[
	ExtensionAttribute(),
	]
	public static bool IsDuration (Microsoft.OData.Edm.IEdmTypeReference type)

	[
	ExtensionAttribute(),
	]
	public static bool IsEntity (Microsoft.OData.Edm.IEdmTypeReference type)

	[
	ExtensionAttribute(),
	]
	public static bool IsEntityReference (Microsoft.OData.Edm.IEdmTypeReference type)

	[
	ExtensionAttribute(),
	]
	public static bool IsEnum (Microsoft.OData.Edm.IEdmTypeReference type)

	[
	ExtensionAttribute(),
	]
	public static bool IsFloating (Microsoft.OData.Edm.IEdmTypeReference type)

	[
	ExtensionAttribute(),
	]
	public static bool IsGeography (Microsoft.OData.Edm.EdmPrimitiveTypeKind typeKind)

	[
	ExtensionAttribute(),
	]
	public static bool IsGeography (Microsoft.OData.Edm.IEdmType type)

	[
	ExtensionAttribute(),
	]
	public static bool IsGeography (Microsoft.OData.Edm.IEdmTypeReference type)

	[
	ExtensionAttribute(),
	]
	public static bool IsGeometry (Microsoft.OData.Edm.EdmPrimitiveTypeKind typeKind)

	[
	ExtensionAttribute(),
	]
	public static bool IsGeometry (Microsoft.OData.Edm.IEdmType type)

	[
	ExtensionAttribute(),
	]
	public static bool IsGeometry (Microsoft.OData.Edm.IEdmTypeReference type)

	[
	ExtensionAttribute(),
	]
	public static bool IsGuid (Microsoft.OData.Edm.IEdmTypeReference type)

	[
	ExtensionAttribute(),
	]
	public static bool IsInt16 (Microsoft.OData.Edm.IEdmTypeReference type)

	[
	ExtensionAttribute(),
	]
	public static bool IsInt32 (Microsoft.OData.Edm.IEdmTypeReference type)

	[
	ExtensionAttribute(),
	]
	public static bool IsInt64 (Microsoft.OData.Edm.IEdmTypeReference type)

	[
	ExtensionAttribute(),
	]
	public static bool IsIntegral (Microsoft.OData.Edm.EdmPrimitiveTypeKind primitiveTypeKind)

	[
	ExtensionAttribute(),
	]
	public static bool IsIntegral (Microsoft.OData.Edm.IEdmTypeReference type)

	[
	ExtensionAttribute(),
	]
	public static bool IsOnSameTypeHierarchyLineWith (Microsoft.OData.Edm.IEdmType thisType, Microsoft.OData.Edm.IEdmType otherType)

	[
	ExtensionAttribute(),
	]
	public static bool IsOrInheritsFrom (Microsoft.OData.Edm.IEdmType thisType, Microsoft.OData.Edm.IEdmType otherType)

	[
	ExtensionAttribute(),
	]
	public static bool IsPath (Microsoft.OData.Edm.IEdmTypeReference type)

	[
	ExtensionAttribute(),
	]
	public static bool IsPrimitive (Microsoft.OData.Edm.IEdmTypeReference type)

	[
	ExtensionAttribute(),
	]
	public static bool IsSByte (Microsoft.OData.Edm.IEdmTypeReference type)

	[
	ExtensionAttribute(),
	]
	public static bool IsSignedIntegral (Microsoft.OData.Edm.IEdmTypeReference type)

	[
	ExtensionAttribute(),
	]
	public static bool IsSingle (Microsoft.OData.Edm.IEdmTypeReference type)

	[
	ExtensionAttribute(),
	]
	public static bool IsSpatial (Microsoft.OData.Edm.EdmPrimitiveTypeKind typeKind)

	[
	ExtensionAttribute(),
	]
	public static bool IsSpatial (Microsoft.OData.Edm.IEdmType type)

	[
	ExtensionAttribute(),
	]
	public static bool IsSpatial (Microsoft.OData.Edm.IEdmTypeReference type)

	[
	ExtensionAttribute(),
	]
	public static bool IsStream (Microsoft.OData.Edm.IEdmType type)

	[
	ExtensionAttribute(),
	]
	public static bool IsStream (Microsoft.OData.Edm.IEdmTypeReference type)

	[
	ExtensionAttribute(),
	]
	public static bool IsString (Microsoft.OData.Edm.IEdmType type)

	[
	ExtensionAttribute(),
	]
	public static bool IsString (Microsoft.OData.Edm.IEdmTypeReference type)

	[
	ExtensionAttribute(),
	]
	public static bool IsStructured (Microsoft.OData.Edm.EdmTypeKind typeKind)

	[
	ExtensionAttribute(),
	]
	public static bool IsStructured (Microsoft.OData.Edm.IEdmTypeReference type)

	[
	ExtensionAttribute(),
	]
	public static bool IsTemporal (Microsoft.OData.Edm.EdmPrimitiveTypeKind typeKind)

	[
	ExtensionAttribute(),
	]
	public static bool IsTemporal (Microsoft.OData.Edm.IEdmType type)

	[
	ExtensionAttribute(),
	]
	public static bool IsTemporal (Microsoft.OData.Edm.IEdmTypeReference type)

	[
	ExtensionAttribute(),
	]
	public static bool IsTimeOfDay (Microsoft.OData.Edm.IEdmTypeReference type)

	[
	ExtensionAttribute(),
	]
	public static bool IsTypeDefinition (Microsoft.OData.Edm.IEdmTypeReference type)

	[
	ExtensionAttribute(),
	]
	public static bool IsUntyped (Microsoft.OData.Edm.IEdmTypeReference type)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Edm.EdmPrimitiveTypeKind PrimitiveKind (Microsoft.OData.Edm.IEdmTypeReference type)
}

[
ExtensionAttribute(),
]
public sealed class Microsoft.OData.Edm.EdmUtil {
	[
	ExtensionAttribute(),
	]
	public static string GetMimeType (Microsoft.OData.Edm.IEdmModel model, Microsoft.OData.Edm.IEdmOperation annotatableOperation)

	[
	ExtensionAttribute(),
	]
	public static string GetMimeType (Microsoft.OData.Edm.IEdmModel model, Microsoft.OData.Edm.IEdmProperty annotatableProperty)

	[
	ExtensionAttribute(),
	]
	public static void SetMimeType (Microsoft.OData.Edm.IEdmModel model, Microsoft.OData.Edm.IEdmOperation annotatableOperation, string mimeType)

	[
	ExtensionAttribute(),
	]
	public static void SetMimeType (Microsoft.OData.Edm.IEdmModel model, Microsoft.OData.Edm.IEdmProperty annotatableProperty, string mimeType)
}

[
ExtensionAttribute(),
]
public sealed class Microsoft.OData.Edm.EnumHelper {
	[
	ExtensionAttribute(),
	]
	public static string ToStringLiteral (Microsoft.OData.Edm.IEdmEnumTypeReference type, long value)

	[
	ExtensionAttribute(),
	]
	public static bool TryParseEnum (Microsoft.OData.Edm.IEdmEnumType enumType, string value, bool ignoreCase, out System.Int64& parseResult)
}

[
ExtensionAttribute(),
]
public sealed class Microsoft.OData.Edm.ExtensionMethods {
	[
	ExtensionAttribute(),
	]
	public static void AddAlternateKeyAnnotation (Microsoft.OData.Edm.EdmModel model, Microsoft.OData.Edm.IEdmEntityType type, System.Collections.Generic.IDictionary`2[[System.String],[Microsoft.OData.Edm.IEdmProperty]] alternateKey)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Edm.EdmComplexType AddComplexType (Microsoft.OData.Edm.EdmModel model, string namespaceName, string name)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Edm.EdmComplexType AddComplexType (Microsoft.OData.Edm.EdmModel model, string namespaceName, string name, Microsoft.OData.Edm.IEdmComplexType baseType)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Edm.EdmComplexType AddComplexType (Microsoft.OData.Edm.EdmModel model, string namespaceName, string name, Microsoft.OData.Edm.IEdmComplexType baseType, bool isAbstract)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Edm.EdmComplexType AddComplexType (Microsoft.OData.Edm.EdmModel model, string namespaceName, string name, Microsoft.OData.Edm.IEdmComplexType baseType, bool isAbstract, bool isOpen)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Edm.EdmEntityContainer AddEntityContainer (Microsoft.OData.Edm.EdmModel model, string namespaceName, string name)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Edm.EdmEntityType AddEntityType (Microsoft.OData.Edm.EdmModel model, string namespaceName, string name)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Edm.EdmEntityType AddEntityType (Microsoft.OData.Edm.EdmModel model, string namespaceName, string name, Microsoft.OData.Edm.IEdmEntityType baseType)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Edm.EdmEntityType AddEntityType (Microsoft.OData.Edm.EdmModel model, string namespaceName, string name, Microsoft.OData.Edm.IEdmEntityType baseType, bool isAbstract, bool isOpen)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Edm.EdmEntityType AddEntityType (Microsoft.OData.Edm.EdmModel model, string namespaceName, string name, Microsoft.OData.Edm.IEdmEntityType baseType, bool isAbstract, bool isOpen, bool hasStream)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Edm.IEdmType AsElementType (Microsoft.OData.Edm.IEdmType type)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Edm.IEdmComplexType BaseComplexType (Microsoft.OData.Edm.IEdmComplexType type)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Edm.IEdmComplexType BaseComplexType (Microsoft.OData.Edm.IEdmComplexTypeReference type)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Edm.IEdmEntityType BaseEntityType (Microsoft.OData.Edm.IEdmEntityType type)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Edm.IEdmEntityType BaseEntityType (Microsoft.OData.Edm.IEdmEntityTypeReference type)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Edm.IEdmStructuredType BaseType (Microsoft.OData.Edm.IEdmStructuredType type)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Edm.IEdmStructuredType BaseType (Microsoft.OData.Edm.IEdmStructuredTypeReference type)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Edm.IEdmCollectionType CollectionDefinition (Microsoft.OData.Edm.IEdmCollectionTypeReference type)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Edm.IEdmComplexType ComplexDefinition (Microsoft.OData.Edm.IEdmComplexTypeReference type)

	[
	ExtensionAttribute(),
	]
	public static System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmNavigationProperty]] DeclaredNavigationProperties (Microsoft.OData.Edm.IEdmStructuredType type)

	[
	ExtensionAttribute(),
	]
	public static System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmNavigationProperty]] DeclaredNavigationProperties (Microsoft.OData.Edm.IEdmStructuredTypeReference type)

	[
	ExtensionAttribute(),
	]
	public static System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmStructuralProperty]] DeclaredStructuralProperties (Microsoft.OData.Edm.IEdmStructuredType type)

	[
	ExtensionAttribute(),
	]
	public static System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmStructuralProperty]] DeclaredStructuralProperties (Microsoft.OData.Edm.IEdmStructuredTypeReference type)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Edm.IEdmEntityType DeclaringEntityType (Microsoft.OData.Edm.IEdmNavigationProperty property)

	[
	ExtensionAttribute(),
	]
	public static System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmStructuralProperty]] DependentProperties (Microsoft.OData.Edm.IEdmNavigationProperty navigationProperty)

	[
	ExtensionAttribute(),
	]
	public static System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Vocabularies.IEdmDirectValueAnnotation]] DirectValueAnnotations (Microsoft.OData.Edm.IEdmModel model, Microsoft.OData.Edm.IEdmElement element)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Edm.IEdmTypeReference ElementType (Microsoft.OData.Edm.IEdmCollectionTypeReference type)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Edm.IEdmEntityType EntityDefinition (Microsoft.OData.Edm.IEdmEntityTypeReference type)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Edm.IEdmEntityReferenceType EntityReferenceDefinition (Microsoft.OData.Edm.IEdmEntityReferenceTypeReference type)

	[
	ExtensionAttribute(),
	]
	public static System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmEntitySet]] EntitySets (Microsoft.OData.Edm.IEdmEntityContainer container)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Edm.IEdmEntityType EntityType (Microsoft.OData.Edm.IEdmEntityReferenceTypeReference type)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Edm.IEdmEntityType EntityType (Microsoft.OData.Edm.IEdmNavigationSource navigationSource)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Edm.IEdmEnumType EnumDefinition (Microsoft.OData.Edm.IEdmEnumTypeReference type)

	[
	ExtensionAttribute(),
	]
	public static bool ExistsContainer (Microsoft.OData.Edm.IEdmModel model, string containerName)

	[
	ExtensionAttribute(),
	]
	public static System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmOperation]] FilterByName (System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmOperation]] operations, bool forceFullyQualifiedNameFilter, string operationName)

	[
	ExtensionAttribute(),
	]
	public static System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmStructuredType]] FindAllDerivedTypes (Microsoft.OData.Edm.IEdmModel model, Microsoft.OData.Edm.IEdmStructuredType baseType)

	[
	ExtensionAttribute(),
	]
	public static System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmOperation]] FindBoundOperations (Microsoft.OData.Edm.IEdmModel model, Microsoft.OData.Edm.IEdmType bindingType)

	[
	ExtensionAttribute(),
	]
	public static System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmOperation]] FindBoundOperations (Microsoft.OData.Edm.IEdmModel model, string qualifiedName, Microsoft.OData.Edm.IEdmType bindingType)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Edm.IEdmEntitySet FindDeclaredEntitySet (Microsoft.OData.Edm.IEdmModel model, string qualifiedName)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Edm.IEdmNavigationSource FindDeclaredNavigationSource (Microsoft.OData.Edm.IEdmModel model, string qualifiedName)

	[
	ExtensionAttribute(),
	]
	public static System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmOperationImport]] FindDeclaredOperationImports (Microsoft.OData.Edm.IEdmModel model, string qualifiedName)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Edm.IEdmSingleton FindDeclaredSingleton (Microsoft.OData.Edm.IEdmModel model, string qualifiedName)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Edm.IEdmEntityContainer FindEntityContainer (Microsoft.OData.Edm.IEdmModel model, string qualifiedName)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Edm.IEdmNavigationProperty FindNavigationProperty (Microsoft.OData.Edm.IEdmStructuredTypeReference type, string name)

	[
	ExtensionAttribute(),
	]
	public static System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmOperation]] FindOperations (Microsoft.OData.Edm.IEdmModel model, string qualifiedName)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Edm.IEdmProperty FindProperty (Microsoft.OData.Edm.IEdmStructuredTypeReference type, string name)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Edm.Vocabularies.IEdmPropertyConstructor FindProperty (Microsoft.OData.Edm.Vocabularies.IEdmRecordExpression expression, string name)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Edm.Vocabularies.IEdmTerm FindTerm (Microsoft.OData.Edm.IEdmModel model, string qualifiedName)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Edm.IEdmSchemaType FindType (Microsoft.OData.Edm.IEdmModel model, string qualifiedName)

	[
	ExtensionAttribute(),
	]
	public static System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Vocabularies.IEdmVocabularyAnnotation]] FindVocabularyAnnotations (Microsoft.OData.Edm.IEdmModel model, Microsoft.OData.Edm.Vocabularies.IEdmVocabularyAnnotatable element)

	[
	ExtensionAttribute(),
	]
	public static IEnumerable`1 FindVocabularyAnnotations (Microsoft.OData.Edm.IEdmModel model, Microsoft.OData.Edm.Vocabularies.IEdmVocabularyAnnotatable element, Microsoft.OData.Edm.Vocabularies.IEdmTerm term)

	[
	ExtensionAttribute(),
	]
	public static IEnumerable`1 FindVocabularyAnnotations (Microsoft.OData.Edm.IEdmModel model, Microsoft.OData.Edm.Vocabularies.IEdmVocabularyAnnotatable element, string termName)

	[
	ExtensionAttribute(),
	]
	public static IEnumerable`1 FindVocabularyAnnotations (Microsoft.OData.Edm.IEdmModel model, Microsoft.OData.Edm.Vocabularies.IEdmVocabularyAnnotatable element, Microsoft.OData.Edm.Vocabularies.IEdmTerm term, string qualifier)

	[
	ExtensionAttribute(),
	]
	public static IEnumerable`1 FindVocabularyAnnotations (Microsoft.OData.Edm.IEdmModel model, Microsoft.OData.Edm.Vocabularies.IEdmVocabularyAnnotatable element, string termName, string qualifier)

	[
	ExtensionAttribute(),
	]
	public static System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Vocabularies.IEdmVocabularyAnnotation]] FindVocabularyAnnotationsIncludingInheritedAnnotations (Microsoft.OData.Edm.IEdmModel model, Microsoft.OData.Edm.Vocabularies.IEdmVocabularyAnnotatable element)

	[
	ExtensionAttribute(),
	]
	public static string FullName (Microsoft.OData.Edm.IEdmSchemaElement element)

	[
	ExtensionAttribute(),
	]
	public static string FullName (Microsoft.OData.Edm.IEdmTypeReference type)

	[
	ExtensionAttribute(),
	]
	public static string FullNavigationSourceName (Microsoft.OData.Edm.IEdmNavigationSource navigationSource)

	[
	ExtensionAttribute(),
	]
	public static string FullTypeName (Microsoft.OData.Edm.IEdmType type)

	[
	ExtensionAttribute(),
	]
	public static System.Collections.Generic.IEnumerable`1[[System.Collections.Generic.IDictionary`2[[System.String],[Microsoft.OData.Edm.IEdmProperty]]]] GetAlternateKeysAnnotation (Microsoft.OData.Edm.IEdmModel model, Microsoft.OData.Edm.IEdmEntityType type)

	[
	ExtensionAttribute(),
	]
	public static T GetAnnotationValue (Microsoft.OData.Edm.IEdmModel model, Microsoft.OData.Edm.IEdmElement element)

	[
	ExtensionAttribute(),
	]
	public static object GetAnnotationValue (Microsoft.OData.Edm.IEdmModel model, Microsoft.OData.Edm.IEdmElement element, string namespaceName, string localName)

	[
	ExtensionAttribute(),
	]
	public static T GetAnnotationValue (Microsoft.OData.Edm.IEdmModel model, Microsoft.OData.Edm.IEdmElement element, string namespaceName, string localName)

	[
	ExtensionAttribute(),
	]
	public static object[] GetAnnotationValues (Microsoft.OData.Edm.IEdmModel model, System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Vocabularies.IEdmDirectValueAnnotationBinding]] annotations)

	[
	ExtensionAttribute(),
	]
	public static string GetDescriptionAnnotation (Microsoft.OData.Edm.IEdmModel model, Microsoft.OData.Edm.Vocabularies.IEdmVocabularyAnnotatable target)

	[
	ExtensionAttribute(),
	]
	public static System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmReference]] GetEdmReferences (Microsoft.OData.Edm.IEdmModel model)

	[
	ExtensionAttribute(),
	]
	public static System.Version GetEdmVersion (Microsoft.OData.Edm.IEdmModel model)

	[
	ExtensionAttribute(),
	]
	public static string GetLongDescriptionAnnotation (Microsoft.OData.Edm.IEdmModel model, Microsoft.OData.Edm.Vocabularies.IEdmVocabularyAnnotatable target)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Edm.IEdmPathExpression GetPartnerPath (Microsoft.OData.Edm.IEdmNavigationProperty navigationProperty)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Edm.IPrimitiveValueConverter GetPrimitiveValueConverter (Microsoft.OData.Edm.IEdmModel model, Microsoft.OData.Edm.IEdmTypeReference type)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Edm.Vocabularies.IEdmValue GetTermValue (Microsoft.OData.Edm.IEdmModel model, Microsoft.OData.Edm.Vocabularies.IEdmStructuredValue context, Microsoft.OData.Edm.Vocabularies.IEdmTerm term, Microsoft.OData.Edm.Vocabularies.EdmExpressionEvaluator expressionEvaluator)

	[
	ExtensionAttribute(),
	]
	public static T GetTermValue (Microsoft.OData.Edm.IEdmModel model, Microsoft.OData.Edm.Vocabularies.IEdmStructuredValue context, Microsoft.OData.Edm.Vocabularies.IEdmTerm term, Microsoft.OData.Edm.Vocabularies.EdmToClrEvaluator evaluator)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Edm.Vocabularies.IEdmValue GetTermValue (Microsoft.OData.Edm.IEdmModel model, Microsoft.OData.Edm.Vocabularies.IEdmStructuredValue context, string termName, Microsoft.OData.Edm.Vocabularies.EdmExpressionEvaluator expressionEvaluator)

	[
	ExtensionAttribute(),
	]
	public static T GetTermValue (Microsoft.OData.Edm.IEdmModel model, Microsoft.OData.Edm.Vocabularies.IEdmStructuredValue context, string termName, Microsoft.OData.Edm.Vocabularies.EdmToClrEvaluator evaluator)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Edm.Vocabularies.IEdmValue GetTermValue (Microsoft.OData.Edm.IEdmModel model, Microsoft.OData.Edm.Vocabularies.IEdmVocabularyAnnotatable element, Microsoft.OData.Edm.Vocabularies.IEdmTerm term, Microsoft.OData.Edm.Vocabularies.EdmExpressionEvaluator expressionEvaluator)

	[
	ExtensionAttribute(),
	]
	public static T GetTermValue (Microsoft.OData.Edm.IEdmModel model, Microsoft.OData.Edm.Vocabularies.IEdmVocabularyAnnotatable element, Microsoft.OData.Edm.Vocabularies.IEdmTerm term, Microsoft.OData.Edm.Vocabularies.EdmToClrEvaluator evaluator)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Edm.Vocabularies.IEdmValue GetTermValue (Microsoft.OData.Edm.IEdmModel model, Microsoft.OData.Edm.Vocabularies.IEdmVocabularyAnnotatable element, string termName, Microsoft.OData.Edm.Vocabularies.EdmExpressionEvaluator expressionEvaluator)

	[
	ExtensionAttribute(),
	]
	public static T GetTermValue (Microsoft.OData.Edm.IEdmModel model, Microsoft.OData.Edm.Vocabularies.IEdmVocabularyAnnotatable element, string termName, Microsoft.OData.Edm.Vocabularies.EdmToClrEvaluator evaluator)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Edm.Vocabularies.IEdmValue GetTermValue (Microsoft.OData.Edm.IEdmModel model, Microsoft.OData.Edm.Vocabularies.IEdmStructuredValue context, Microsoft.OData.Edm.Vocabularies.IEdmTerm term, string qualifier, Microsoft.OData.Edm.Vocabularies.EdmExpressionEvaluator expressionEvaluator)

	[
	ExtensionAttribute(),
	]
	public static T GetTermValue (Microsoft.OData.Edm.IEdmModel model, Microsoft.OData.Edm.Vocabularies.IEdmStructuredValue context, Microsoft.OData.Edm.Vocabularies.IEdmTerm term, string qualifier, Microsoft.OData.Edm.Vocabularies.EdmToClrEvaluator evaluator)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Edm.Vocabularies.IEdmValue GetTermValue (Microsoft.OData.Edm.IEdmModel model, Microsoft.OData.Edm.Vocabularies.IEdmStructuredValue context, string termName, string qualifier, Microsoft.OData.Edm.Vocabularies.EdmExpressionEvaluator expressionEvaluator)

	[
	ExtensionAttribute(),
	]
	public static T GetTermValue (Microsoft.OData.Edm.IEdmModel model, Microsoft.OData.Edm.Vocabularies.IEdmStructuredValue context, string termName, string qualifier, Microsoft.OData.Edm.Vocabularies.EdmToClrEvaluator evaluator)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Edm.Vocabularies.IEdmValue GetTermValue (Microsoft.OData.Edm.IEdmModel model, Microsoft.OData.Edm.Vocabularies.IEdmVocabularyAnnotatable element, Microsoft.OData.Edm.Vocabularies.IEdmTerm term, string qualifier, Microsoft.OData.Edm.Vocabularies.EdmExpressionEvaluator expressionEvaluator)

	[
	ExtensionAttribute(),
	]
	public static T GetTermValue (Microsoft.OData.Edm.IEdmModel model, Microsoft.OData.Edm.Vocabularies.IEdmVocabularyAnnotatable element, Microsoft.OData.Edm.Vocabularies.IEdmTerm term, string qualifier, Microsoft.OData.Edm.Vocabularies.EdmToClrEvaluator evaluator)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Edm.Vocabularies.IEdmValue GetTermValue (Microsoft.OData.Edm.IEdmModel model, Microsoft.OData.Edm.Vocabularies.IEdmVocabularyAnnotatable element, string termName, string qualifier, Microsoft.OData.Edm.Vocabularies.EdmExpressionEvaluator expressionEvaluator)

	[
	ExtensionAttribute(),
	]
	public static T GetTermValue (Microsoft.OData.Edm.IEdmModel model, Microsoft.OData.Edm.Vocabularies.IEdmVocabularyAnnotatable element, string termName, string qualifier, Microsoft.OData.Edm.Vocabularies.EdmToClrEvaluator evaluator)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Edm.IEdmTypeDefinitionReference GetUInt16 (Microsoft.OData.Edm.EdmModel model, string namespaceName, bool isNullable)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Edm.IEdmTypeDefinitionReference GetUInt32 (Microsoft.OData.Edm.EdmModel model, string namespaceName, bool isNullable)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Edm.IEdmTypeDefinitionReference GetUInt64 (Microsoft.OData.Edm.EdmModel model, string namespaceName, bool isNullable)

	[
	ExtensionAttribute(),
	]
	public static bool HasDeclaredKeyProperty (Microsoft.OData.Edm.IEdmEntityType entityType, Microsoft.OData.Edm.IEdmProperty property)

	[
	ExtensionAttribute(),
	]
	public static bool HasEquivalentBindingType (Microsoft.OData.Edm.IEdmOperation operation, Microsoft.OData.Edm.IEdmType bindingType)

	[
	ExtensionAttribute(),
	]
	public static bool IsAbstract (Microsoft.OData.Edm.IEdmStructuredTypeReference type)

	[
	ExtensionAttribute(),
	]
	public static bool IsAction (Microsoft.OData.Edm.IEdmOperation operation)

	[
	ExtensionAttribute(),
	]
	public static bool IsActionImport (Microsoft.OData.Edm.IEdmOperationImport operationImport)

	[
	ExtensionAttribute(),
	]
	public static bool IsFunction (Microsoft.OData.Edm.IEdmOperation operation)

	[
	ExtensionAttribute(),
	]
	public static bool IsFunctionImport (Microsoft.OData.Edm.IEdmOperationImport operationImport)

	[
	ExtensionAttribute(),
	]
	public static bool IsOpen (Microsoft.OData.Edm.IEdmStructuredTypeReference type)

	[
	ExtensionAttribute(),
	]
	public static bool IsOpen (Microsoft.OData.Edm.IEdmType type)

	[
	ExtensionAttribute(),
	]
	public static bool IsPrincipal (Microsoft.OData.Edm.IEdmNavigationProperty navigationProperty)

	[
	ExtensionAttribute(),
	]
	public static System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmStructuralProperty]] Key (Microsoft.OData.Edm.IEdmEntityType type)

	[
	ExtensionAttribute(),
	]
	public static System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmStructuralProperty]] Key (Microsoft.OData.Edm.IEdmEntityTypeReference type)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Edm.EdmLocation Location (Microsoft.OData.Edm.IEdmElement item)

	[
	ExtensionAttribute(),
	]
	public static System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmNavigationProperty]] NavigationProperties (Microsoft.OData.Edm.IEdmStructuredType type)

	[
	ExtensionAttribute(),
	]
	public static System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmNavigationProperty]] NavigationProperties (Microsoft.OData.Edm.IEdmStructuredTypeReference type)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Edm.EdmNavigationSourceKind NavigationSourceKind (Microsoft.OData.Edm.IEdmNavigationSource navigationSource)

	[
	ExtensionAttribute(),
	]
	public static System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmOperationImport]] OperationImports (Microsoft.OData.Edm.IEdmEntityContainer container)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Edm.IEdmPrimitiveType PrimitiveDefinition (Microsoft.OData.Edm.IEdmPrimitiveTypeReference type)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Edm.EdmPrimitiveTypeKind PrimitiveKind (Microsoft.OData.Edm.IEdmPrimitiveTypeReference type)

	[
	ExtensionAttribute(),
	]
	public static System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmStructuralProperty]] PrincipalProperties (Microsoft.OData.Edm.IEdmNavigationProperty navigationProperty)

	[
	ExtensionAttribute(),
	]
	public static System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmProperty]] Properties (Microsoft.OData.Edm.IEdmStructuredType type)

	[
	ExtensionAttribute(),
	]
	public static System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmSchemaElement]] SchemaElementsAcrossModels (Microsoft.OData.Edm.IEdmModel model)

	[
	ExtensionAttribute(),
	]
	public static void SetAnnotationValue (Microsoft.OData.Edm.IEdmModel model, Microsoft.OData.Edm.IEdmElement element, T value)

	[
	ExtensionAttribute(),
	]
	public static void SetAnnotationValue (Microsoft.OData.Edm.IEdmModel model, Microsoft.OData.Edm.IEdmElement element, string namespaceName, string localName, object value)

	[
	ExtensionAttribute(),
	]
	public static void SetAnnotationValues (Microsoft.OData.Edm.IEdmModel model, System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Vocabularies.IEdmDirectValueAnnotationBinding]] annotations)

	[
	ExtensionAttribute(),
	]
	public static void SetChangeTrackingAnnotation (Microsoft.OData.Edm.EdmModel model, Microsoft.OData.Edm.IEdmEntityContainer target, bool isSupported)

	[
	ExtensionAttribute(),
	]
	public static void SetChangeTrackingAnnotation (Microsoft.OData.Edm.EdmModel model, Microsoft.OData.Edm.IEdmEntitySet target, bool isSupported, System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmStructuralProperty]] filterableProperties, System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmNavigationProperty]] expandableProperties)

	[
	ExtensionAttribute(),
	]
	public static void SetDescriptionAnnotation (Microsoft.OData.Edm.EdmModel model, Microsoft.OData.Edm.Vocabularies.IEdmVocabularyAnnotatable target, string description)

	[
	ExtensionAttribute(),
	]
	public static void SetEdmReferences (Microsoft.OData.Edm.IEdmModel model, System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmReference]] edmReferences)

	[
	ExtensionAttribute(),
	]
	public static void SetEdmVersion (Microsoft.OData.Edm.IEdmModel model, System.Version version)

	[
	ExtensionAttribute(),
	]
	public static void SetLongDescriptionAnnotation (Microsoft.OData.Edm.EdmModel model, Microsoft.OData.Edm.Vocabularies.IEdmVocabularyAnnotatable target, string description)

	[
	ExtensionAttribute(),
	]
	public static void SetOptimisticConcurrencyAnnotation (Microsoft.OData.Edm.EdmModel model, Microsoft.OData.Edm.IEdmEntitySet target, System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmStructuralProperty]] properties)

	[
	ExtensionAttribute(),
	]
	public static void SetPrimitiveValueConverter (Microsoft.OData.Edm.IEdmModel model, Microsoft.OData.Edm.IEdmTypeDefinitionReference typeDefinition, Microsoft.OData.Edm.IPrimitiveValueConverter converter)

	[
	ExtensionAttribute(),
	]
	public static string ShortQualifiedName (Microsoft.OData.Edm.IEdmSchemaElement element)

	[
	ExtensionAttribute(),
	]
	public static string ShortQualifiedName (Microsoft.OData.Edm.IEdmTypeReference type)

	[
	ExtensionAttribute(),
	]
	public static System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmSingleton]] Singletons (Microsoft.OData.Edm.IEdmEntityContainer container)

	[
	ExtensionAttribute(),
	]
	public static System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmStructuralProperty]] StructuralProperties (Microsoft.OData.Edm.IEdmStructuredType type)

	[
	ExtensionAttribute(),
	]
	public static System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmStructuralProperty]] StructuralProperties (Microsoft.OData.Edm.IEdmStructuredTypeReference type)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Edm.IEdmStructuredType StructuredDefinition (Microsoft.OData.Edm.IEdmStructuredTypeReference type)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Edm.EdmMultiplicity TargetMultiplicity (Microsoft.OData.Edm.IEdmNavigationProperty property)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Edm.Vocabularies.IEdmTerm Term (Microsoft.OData.Edm.Vocabularies.IEdmVocabularyAnnotation annotation)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Edm.IEdmEntityType ToEntityType (Microsoft.OData.Edm.IEdmNavigationProperty property)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Edm.IEdmStructuredType ToStructuredType (Microsoft.OData.Edm.IEdmTypeReference propertyTypeReference)

	[
	ExtensionAttribute(),
	]
	public static bool TryFindContainerQualifiedEntitySet (Microsoft.OData.Edm.IEdmModel model, string containerQualifiedEntitySetName, out Microsoft.OData.Edm.IEdmEntitySet& entitySet)

	[
	ExtensionAttribute(),
	]
	public static bool TryFindContainerQualifiedOperationImports (Microsoft.OData.Edm.IEdmModel model, string containerQualifiedOperationImportName, out System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmOperationImport]]& operationImports)

	[
	ExtensionAttribute(),
	]
	public static bool TryFindContainerQualifiedSingleton (Microsoft.OData.Edm.IEdmModel model, string containerQualifiedSingletonName, out Microsoft.OData.Edm.IEdmSingleton& singleton)

	[
	ExtensionAttribute(),
	]
	public static bool TryGetRelativeEntitySetPath (Microsoft.OData.Edm.IEdmOperationImport operationImport, Microsoft.OData.Edm.IEdmModel model, out Microsoft.OData.Edm.IEdmOperationParameter& parameter, out System.Collections.Generic.Dictionary`2[[Microsoft.OData.Edm.IEdmNavigationProperty],[Microsoft.OData.Edm.IEdmPathExpression]]& relativeNavigations, out System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Validation.EdmError]]& edmErrors)

	[
	ExtensionAttribute(),
	]
	public static bool TryGetRelativeEntitySetPath (Microsoft.OData.Edm.IEdmOperation operation, Microsoft.OData.Edm.IEdmModel model, out Microsoft.OData.Edm.IEdmOperationParameter& parameter, out System.Collections.Generic.Dictionary`2[[Microsoft.OData.Edm.IEdmNavigationProperty],[Microsoft.OData.Edm.IEdmPathExpression]]& relativeNavigations, out Microsoft.OData.Edm.IEdmEntityType& lastEntityType, out System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Validation.EdmError]]& errors)

	[
	ExtensionAttribute(),
	]
	public static bool TryGetStaticEntitySet (Microsoft.OData.Edm.IEdmOperationImport operationImport, Microsoft.OData.Edm.IEdmModel model, out Microsoft.OData.Edm.IEdmEntitySetBase& entitySet)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Edm.IEdmTypeDefinition TypeDefinition (Microsoft.OData.Edm.IEdmTypeDefinitionReference type)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Edm.EdmTypeKind TypeKind (Microsoft.OData.Edm.IEdmTypeReference type)

	[
	ExtensionAttribute(),
	]
	public static System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Vocabularies.IEdmVocabularyAnnotation]] VocabularyAnnotations (Microsoft.OData.Edm.Vocabularies.IEdmVocabularyAnnotatable element, Microsoft.OData.Edm.IEdmModel model)
}

[
ExtensionAttribute(),
]
public sealed class Microsoft.OData.Edm.ToTraceStringExtensionMethods {
	[
	ExtensionAttribute(),
	]
	public static string ToTraceString (Microsoft.OData.Edm.IEdmProperty property)

	[
	ExtensionAttribute(),
	]
	public static string ToTraceString (Microsoft.OData.Edm.IEdmSchemaElement schemaElement)

	[
	ExtensionAttribute(),
	]
	public static string ToTraceString (Microsoft.OData.Edm.IEdmSchemaType schemaType)

	[
	ExtensionAttribute(),
	]
	public static string ToTraceString (Microsoft.OData.Edm.IEdmType type)

	[
	ExtensionAttribute(),
	]
	public static string ToTraceString (Microsoft.OData.Edm.IEdmTypeReference type)
}

public class Microsoft.OData.Edm.EdmAction : Microsoft.OData.Edm.EdmOperation, IEdmAction, IEdmElement, IEdmNamedElement, IEdmOperation, IEdmSchemaElement, IEdmVocabularyAnnotatable {
	public EdmAction (string namespaceName, string name, Microsoft.OData.Edm.IEdmTypeReference returnType)
	public EdmAction (string namespaceName, string name, Microsoft.OData.Edm.IEdmTypeReference returnType, bool isBound, Microsoft.OData.Edm.IEdmPathExpression entitySetPathExpression)

	Microsoft.OData.Edm.EdmSchemaElementKind SchemaElementKind  { public virtual get; }
}

public class Microsoft.OData.Edm.EdmActionImport : Microsoft.OData.Edm.EdmOperationImport, IEdmActionImport, IEdmElement, IEdmEntityContainerElement, IEdmNamedElement, IEdmOperationImport, IEdmVocabularyAnnotatable {
	public EdmActionImport (Microsoft.OData.Edm.IEdmEntityContainer container, string name, Microsoft.OData.Edm.IEdmAction action)
	public EdmActionImport (Microsoft.OData.Edm.IEdmEntityContainer container, string name, Microsoft.OData.Edm.IEdmAction action, Microsoft.OData.Edm.IEdmExpression entitySetExpression)

	Microsoft.OData.Edm.IEdmAction Action  { public virtual get; }
	Microsoft.OData.Edm.EdmContainerElementKind ContainerElementKind  { public virtual get; }

	protected virtual string OperationArgumentNullParameterName ()
}

public class Microsoft.OData.Edm.EdmBinaryTypeReference : Microsoft.OData.Edm.EdmPrimitiveTypeReference, IEdmBinaryTypeReference, IEdmElement, IEdmPrimitiveTypeReference, IEdmTypeReference {
	public EdmBinaryTypeReference (Microsoft.OData.Edm.IEdmPrimitiveType definition, bool isNullable)
	public EdmBinaryTypeReference (Microsoft.OData.Edm.IEdmPrimitiveType definition, bool isNullable, bool isUnbounded, System.Nullable`1[[System.Int32]] maxLength)

	bool IsUnbounded  { public virtual get; }
	System.Nullable`1[[System.Int32]] MaxLength  { public virtual get; }
}

public class Microsoft.OData.Edm.EdmCollectionType : Microsoft.OData.Edm.EdmType, IEdmCollectionType, IEdmElement, IEdmType {
	public EdmCollectionType (Microsoft.OData.Edm.IEdmTypeReference elementType)

	Microsoft.OData.Edm.IEdmTypeReference ElementType  { public virtual get; }
	Microsoft.OData.Edm.EdmTypeKind TypeKind  { public virtual get; }
}

public class Microsoft.OData.Edm.EdmCollectionTypeReference : Microsoft.OData.Edm.EdmTypeReference, IEdmCollectionTypeReference, IEdmElement, IEdmTypeReference {
	public EdmCollectionTypeReference (Microsoft.OData.Edm.IEdmCollectionType collectionType)
}

public class Microsoft.OData.Edm.EdmComplexType : Microsoft.OData.Edm.EdmStructuredType, IEdmComplexType, IEdmElement, IEdmNamedElement, IEdmSchemaElement, IEdmSchemaType, IEdmStructuredType, IEdmType, IEdmVocabularyAnnotatable {
	public EdmComplexType (string namespaceName, string name)
	public EdmComplexType (string namespaceName, string name, Microsoft.OData.Edm.IEdmComplexType baseType)
	public EdmComplexType (string namespaceName, string name, Microsoft.OData.Edm.IEdmComplexType baseType, bool isAbstract)
	public EdmComplexType (string namespaceName, string name, Microsoft.OData.Edm.IEdmComplexType baseType, bool isAbstract, bool isOpen)

	string Name  { public virtual get; }
	string Namespace  { public virtual get; }
	Microsoft.OData.Edm.EdmSchemaElementKind SchemaElementKind  { public virtual get; }
	Microsoft.OData.Edm.EdmTypeKind TypeKind  { public virtual get; }
}

public class Microsoft.OData.Edm.EdmComplexTypeReference : Microsoft.OData.Edm.EdmTypeReference, IEdmComplexTypeReference, IEdmElement, IEdmStructuredTypeReference, IEdmTypeReference {
	public EdmComplexTypeReference (Microsoft.OData.Edm.IEdmComplexType complexType, bool isNullable)
}

public class Microsoft.OData.Edm.EdmCoreModel : Microsoft.OData.Edm.EdmElement, IEdmCoreModelElement, IEdmElement, IEdmModel {
	public static readonly Microsoft.OData.Edm.EdmCoreModel Instance = Microsoft.OData.Edm.EdmCoreModel

	System.Collections.Generic.IEnumerable`1[[System.String]] DeclaredNamespaces  { public virtual get; }
	Microsoft.OData.Edm.Vocabularies.IEdmDirectValueAnnotationsManager DirectValueAnnotationsManager  { public virtual get; }
	Microsoft.OData.Edm.IEdmEntityContainer EntityContainer  { public virtual get; }
	string Namespace  { public static get; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmModel]] ReferencedModels  { public virtual get; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmSchemaElement]] SchemaElements  { public virtual get; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Vocabularies.IEdmVocabularyAnnotation]] VocabularyAnnotations  { public virtual get; }

	public virtual System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmOperation]] FindDeclaredBoundOperations (Microsoft.OData.Edm.IEdmType bindingType)
	public virtual System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmOperation]] FindDeclaredBoundOperations (string qualifiedName, Microsoft.OData.Edm.IEdmType bindingType)
	public virtual System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmOperation]] FindDeclaredOperations (string qualifiedName)
	public virtual Microsoft.OData.Edm.Vocabularies.IEdmTerm FindDeclaredTerm (string qualifiedName)
	public virtual Microsoft.OData.Edm.IEdmSchemaType FindDeclaredType (string qualifiedName)
	public virtual System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Vocabularies.IEdmVocabularyAnnotation]] FindDeclaredVocabularyAnnotations (Microsoft.OData.Edm.Vocabularies.IEdmVocabularyAnnotatable element)
	public virtual System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmStructuredType]] FindDirectlyDerivedTypes (Microsoft.OData.Edm.IEdmStructuredType baseType)
	public System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmOperationImport]] FindOperationImportsByNameNonBindingParameterType (string operationImportName, System.Collections.Generic.IEnumerable`1[[System.String]] parameterNames)
	public Microsoft.OData.Edm.IEdmPathTypeReference GetAnnotationPath (bool isNullable)
	public Microsoft.OData.Edm.IEdmBinaryTypeReference GetBinary (bool isNullable)
	public Microsoft.OData.Edm.IEdmBinaryTypeReference GetBinary (bool isUnbounded, System.Nullable`1[[System.Int32]] maxLength, bool isNullable)
	public Microsoft.OData.Edm.IEdmPrimitiveTypeReference GetBoolean (bool isNullable)
	public Microsoft.OData.Edm.IEdmPrimitiveTypeReference GetByte (bool isNullable)
	public static Microsoft.OData.Edm.IEdmCollectionTypeReference GetCollection (Microsoft.OData.Edm.IEdmTypeReference elementType)
	public Microsoft.OData.Edm.IEdmComplexType GetComplexType ()
	public Microsoft.OData.Edm.IEdmComplexTypeReference GetComplexType (bool isNullable)
	public Microsoft.OData.Edm.IEdmPrimitiveTypeReference GetDate (bool isNullable)
	public Microsoft.OData.Edm.IEdmTemporalTypeReference GetDateTimeOffset (bool isNullable)
	public Microsoft.OData.Edm.IEdmDecimalTypeReference GetDecimal (bool isNullable)
	public Microsoft.OData.Edm.IEdmDecimalTypeReference GetDecimal (System.Nullable`1[[System.Int32]] precision, System.Nullable`1[[System.Int32]] scale, bool isNullable)
	public Microsoft.OData.Edm.IEdmPrimitiveTypeReference GetDouble (bool isNullable)
	public Microsoft.OData.Edm.IEdmTemporalTypeReference GetDuration (bool isNullable)
	public Microsoft.OData.Edm.IEdmEntityType GetEntityType ()
	public Microsoft.OData.Edm.IEdmEntityTypeReference GetEntityType (bool isNullable)
	public Microsoft.OData.Edm.IEdmPrimitiveTypeReference GetGuid (bool isNullable)
	public Microsoft.OData.Edm.IEdmPrimitiveTypeReference GetInt16 (bool isNullable)
	public Microsoft.OData.Edm.IEdmPrimitiveTypeReference GetInt32 (bool isNullable)
	public Microsoft.OData.Edm.IEdmPrimitiveTypeReference GetInt64 (bool isNullable)
	public Microsoft.OData.Edm.IEdmPathTypeReference GetNavigationPropertyPath (bool isNullable)
	public Microsoft.OData.Edm.IEdmPathType GetPathType (Microsoft.OData.Edm.EdmPathTypeKind kind)
	public Microsoft.OData.Edm.IEdmPathTypeReference GetPathType (Microsoft.OData.Edm.EdmPathTypeKind kind, bool isNullable)
	public Microsoft.OData.Edm.EdmPathTypeKind GetPathTypeKind (string typeName)
	public Microsoft.OData.Edm.IEdmPrimitiveTypeReference GetPrimitive (Microsoft.OData.Edm.EdmPrimitiveTypeKind kind, bool isNullable)
	public Microsoft.OData.Edm.IEdmPrimitiveType GetPrimitiveType ()
	public Microsoft.OData.Edm.IEdmPrimitiveType GetPrimitiveType (Microsoft.OData.Edm.EdmPrimitiveTypeKind kind)
	public Microsoft.OData.Edm.IEdmPrimitiveTypeReference GetPrimitiveType (bool isNullable)
	public Microsoft.OData.Edm.EdmPrimitiveTypeKind GetPrimitiveTypeKind (string typeName)
	public Microsoft.OData.Edm.IEdmPathTypeReference GetPropertyPath (bool isNullable)
	public Microsoft.OData.Edm.IEdmPrimitiveTypeReference GetSByte (bool isNullable)
	public Microsoft.OData.Edm.IEdmPrimitiveTypeReference GetSingle (bool isNullable)
	public Microsoft.OData.Edm.IEdmSpatialTypeReference GetSpatial (Microsoft.OData.Edm.EdmPrimitiveTypeKind kind, bool isNullable)
	public Microsoft.OData.Edm.IEdmSpatialTypeReference GetSpatial (Microsoft.OData.Edm.EdmPrimitiveTypeKind kind, System.Nullable`1[[System.Int32]] spatialReferenceIdentifier, bool isNullable)
	public Microsoft.OData.Edm.IEdmPrimitiveTypeReference GetStream (bool isNullable)
	public Microsoft.OData.Edm.IEdmStringTypeReference GetString (bool isNullable)
	public Microsoft.OData.Edm.IEdmStringTypeReference GetString (bool isUnbounded, System.Nullable`1[[System.Int32]] maxLength, System.Nullable`1[[System.Boolean]] isUnicode, bool isNullable)
	public Microsoft.OData.Edm.IEdmTemporalTypeReference GetTemporal (Microsoft.OData.Edm.EdmPrimitiveTypeKind kind, bool isNullable)
	public Microsoft.OData.Edm.IEdmTemporalTypeReference GetTemporal (Microsoft.OData.Edm.EdmPrimitiveTypeKind kind, System.Nullable`1[[System.Int32]] precision, bool isNullable)
	public Microsoft.OData.Edm.IEdmTemporalTypeReference GetTimeOfDay (bool isNullable)
	public Microsoft.OData.Edm.IEdmUntypedTypeReference GetUntyped ()
	public Microsoft.OData.Edm.IEdmUntypedType GetUntypedType ()
}

public class Microsoft.OData.Edm.EdmDecimalTypeReference : Microsoft.OData.Edm.EdmPrimitiveTypeReference, IEdmDecimalTypeReference, IEdmElement, IEdmPrimitiveTypeReference, IEdmTypeReference {
	public EdmDecimalTypeReference (Microsoft.OData.Edm.IEdmPrimitiveType definition, bool isNullable)
	public EdmDecimalTypeReference (Microsoft.OData.Edm.IEdmPrimitiveType definition, bool isNullable, System.Nullable`1[[System.Int32]] precision, System.Nullable`1[[System.Int32]] scale)

	System.Nullable`1[[System.Int32]] Precision  { public virtual get; }
	System.Nullable`1[[System.Int32]] Scale  { public virtual get; }
}

public class Microsoft.OData.Edm.EdmEntityContainer : Microsoft.OData.Edm.EdmElement, IEdmElement, IEdmEntityContainer, IEdmNamedElement, IEdmSchemaElement, IEdmVocabularyAnnotatable {
	public EdmEntityContainer (string namespaceName, string name)

	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmEntityContainerElement]] Elements  { public virtual get; }
	string Name  { public virtual get; }
	string Namespace  { public virtual get; }
	Microsoft.OData.Edm.EdmSchemaElementKind SchemaElementKind  { public virtual get; }

	public virtual Microsoft.OData.Edm.EdmActionImport AddActionImport (Microsoft.OData.Edm.IEdmAction action)
	public virtual Microsoft.OData.Edm.EdmActionImport AddActionImport (string name, Microsoft.OData.Edm.IEdmAction action)
	public virtual Microsoft.OData.Edm.EdmActionImport AddActionImport (string name, Microsoft.OData.Edm.IEdmAction action, Microsoft.OData.Edm.IEdmExpression entitySet)
	public void AddElement (Microsoft.OData.Edm.IEdmEntityContainerElement element)
	public virtual Microsoft.OData.Edm.EdmEntitySet AddEntitySet (string name, Microsoft.OData.Edm.IEdmEntityType elementType)
	public virtual Microsoft.OData.Edm.EdmEntitySet AddEntitySet (string name, Microsoft.OData.Edm.IEdmEntityType elementType, bool includeInServiceDocument)
	public virtual Microsoft.OData.Edm.EdmFunctionImport AddFunctionImport (Microsoft.OData.Edm.IEdmFunction function)
	public virtual Microsoft.OData.Edm.EdmFunctionImport AddFunctionImport (string name, Microsoft.OData.Edm.IEdmFunction function)
	public virtual Microsoft.OData.Edm.EdmFunctionImport AddFunctionImport (string name, Microsoft.OData.Edm.IEdmFunction function, Microsoft.OData.Edm.IEdmExpression entitySet)
	public virtual Microsoft.OData.Edm.EdmOperationImport AddFunctionImport (string name, Microsoft.OData.Edm.IEdmFunction function, Microsoft.OData.Edm.IEdmExpression entitySet, bool includeInServiceDocument)
	public virtual Microsoft.OData.Edm.EdmSingleton AddSingleton (string name, Microsoft.OData.Edm.IEdmEntityType entityType)
	public virtual Microsoft.OData.Edm.IEdmEntitySet FindEntitySet (string setName)
	public virtual System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmOperationImport]] FindOperationImports (string operationName)
	public virtual Microsoft.OData.Edm.IEdmSingleton FindSingleton (string singletonName)
}

public class Microsoft.OData.Edm.EdmEntityReferenceType : Microsoft.OData.Edm.EdmType, IEdmElement, IEdmEntityReferenceType, IEdmType {
	public EdmEntityReferenceType (Microsoft.OData.Edm.IEdmEntityType entityType)

	Microsoft.OData.Edm.IEdmEntityType EntityType  { public virtual get; }
	Microsoft.OData.Edm.EdmTypeKind TypeKind  { public virtual get; }
}

public class Microsoft.OData.Edm.EdmEntityReferenceTypeReference : Microsoft.OData.Edm.EdmTypeReference, IEdmElement, IEdmEntityReferenceTypeReference, IEdmTypeReference {
	public EdmEntityReferenceTypeReference (Microsoft.OData.Edm.IEdmEntityReferenceType entityReferenceType, bool isNullable)

	Microsoft.OData.Edm.IEdmEntityReferenceType EntityReferenceDefinition  { public get; }
}

public class Microsoft.OData.Edm.EdmEntitySet : Microsoft.OData.Edm.EdmEntitySetBase, IEdmElement, IEdmEntityContainerElement, IEdmEntitySet, IEdmEntitySetBase, IEdmNamedElement, IEdmNavigationSource, IEdmVocabularyAnnotatable {
	public EdmEntitySet (Microsoft.OData.Edm.IEdmEntityContainer container, string name, Microsoft.OData.Edm.IEdmEntityType elementType)
	public EdmEntitySet (Microsoft.OData.Edm.IEdmEntityContainer container, string name, Microsoft.OData.Edm.IEdmEntityType elementType, bool includeInServiceDocument)

	Microsoft.OData.Edm.IEdmEntityContainer Container  { public virtual get; }
	Microsoft.OData.Edm.EdmContainerElementKind ContainerElementKind  { public virtual get; }
	bool IncludeInServiceDocument  { public virtual get; }
	Microsoft.OData.Edm.IEdmPathExpression Path  { public virtual get; }
	Microsoft.OData.Edm.IEdmType Type  { public virtual get; }
}

public class Microsoft.OData.Edm.EdmEntityType : Microsoft.OData.Edm.EdmStructuredType, IEdmElement, IEdmEntityType, IEdmNamedElement, IEdmSchemaElement, IEdmSchemaType, IEdmStructuredType, IEdmType, IEdmVocabularyAnnotatable {
	public EdmEntityType (string namespaceName, string name)
	public EdmEntityType (string namespaceName, string name, Microsoft.OData.Edm.IEdmEntityType baseType)
	public EdmEntityType (string namespaceName, string name, Microsoft.OData.Edm.IEdmEntityType baseType, bool isAbstract, bool isOpen)
	public EdmEntityType (string namespaceName, string name, Microsoft.OData.Edm.IEdmEntityType baseType, bool isAbstract, bool isOpen, bool hasStream)

	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmStructuralProperty]] DeclaredKey  { public virtual get; }
	bool HasStream  { public virtual get; }
	string Name  { public virtual get; }
	string Namespace  { public virtual get; }
	Microsoft.OData.Edm.EdmSchemaElementKind SchemaElementKind  { public virtual get; }
	Microsoft.OData.Edm.EdmTypeKind TypeKind  { public virtual get; }

	public Microsoft.OData.Edm.EdmNavigationProperty AddBidirectionalNavigation (Microsoft.OData.Edm.EdmNavigationPropertyInfo propertyInfo, Microsoft.OData.Edm.EdmNavigationPropertyInfo partnerInfo)
	public void AddKeys (Microsoft.OData.Edm.IEdmStructuralProperty[] keyProperties)
	public void AddKeys (System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmStructuralProperty]] keyProperties)
	public void SetNavigationPropertyPartner (Microsoft.OData.Edm.EdmNavigationProperty navigationProperty, Microsoft.OData.Edm.IEdmPathExpression navigationPropertyPath, Microsoft.OData.Edm.EdmNavigationProperty partnerNavigationProperty, Microsoft.OData.Edm.IEdmPathExpression partnerNavigationPropertyPath)
}

public class Microsoft.OData.Edm.EdmEntityTypeReference : Microsoft.OData.Edm.EdmTypeReference, IEdmElement, IEdmEntityTypeReference, IEdmStructuredTypeReference, IEdmTypeReference {
	public EdmEntityTypeReference (Microsoft.OData.Edm.IEdmEntityType entityType, bool isNullable)
}

public class Microsoft.OData.Edm.EdmEnumMember : Microsoft.OData.Edm.EdmNamedElement, IEdmElement, IEdmEnumMember, IEdmNamedElement, IEdmVocabularyAnnotatable {
	public EdmEnumMember (Microsoft.OData.Edm.IEdmEnumType declaringType, string name, Microsoft.OData.Edm.IEdmEnumMemberValue value)

	Microsoft.OData.Edm.IEdmEnumType DeclaringType  { public virtual get; }
	Microsoft.OData.Edm.IEdmEnumMemberValue Value  { public virtual get; }
}

public class Microsoft.OData.Edm.EdmEnumMemberValue : IEdmElement, IEdmEnumMemberValue {
	public EdmEnumMemberValue (long value)

	long Value  { public virtual get; }
}

public class Microsoft.OData.Edm.EdmEnumType : Microsoft.OData.Edm.EdmType, IEdmElement, IEdmEnumType, IEdmNamedElement, IEdmSchemaElement, IEdmSchemaType, IEdmType, IEdmVocabularyAnnotatable {
	public EdmEnumType (string namespaceName, string name)
	public EdmEnumType (string namespaceName, string name, bool isFlags)
	public EdmEnumType (string namespaceName, string name, Microsoft.OData.Edm.EdmPrimitiveTypeKind underlyingType, bool isFlags)
	public EdmEnumType (string namespaceName, string name, Microsoft.OData.Edm.IEdmPrimitiveType underlyingType, bool isFlags)

	bool IsFlags  { public virtual get; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmEnumMember]] Members  { public virtual get; }
	string Name  { public virtual get; }
	string Namespace  { public virtual get; }
	Microsoft.OData.Edm.EdmSchemaElementKind SchemaElementKind  { public virtual get; }
	Microsoft.OData.Edm.EdmTypeKind TypeKind  { public virtual get; }
	Microsoft.OData.Edm.IEdmPrimitiveType UnderlyingType  { public virtual get; }

	public void AddMember (Microsoft.OData.Edm.IEdmEnumMember member)
	public Microsoft.OData.Edm.EdmEnumMember AddMember (string name, Microsoft.OData.Edm.IEdmEnumMemberValue value)
}

public class Microsoft.OData.Edm.EdmEnumTypeReference : Microsoft.OData.Edm.EdmTypeReference, IEdmElement, IEdmEnumTypeReference, IEdmTypeReference {
	public EdmEnumTypeReference (Microsoft.OData.Edm.IEdmEnumType enumType, bool isNullable)
}

public class Microsoft.OData.Edm.EdmFunction : Microsoft.OData.Edm.EdmOperation, IEdmElement, IEdmFunction, IEdmNamedElement, IEdmOperation, IEdmSchemaElement, IEdmVocabularyAnnotatable {
	public EdmFunction (string namespaceName, string name, Microsoft.OData.Edm.IEdmTypeReference returnType)
	public EdmFunction (string namespaceName, string name, Microsoft.OData.Edm.IEdmTypeReference returnType, bool isBound, Microsoft.OData.Edm.IEdmPathExpression entitySetPathExpression, bool isComposable)

	bool IsComposable  { public virtual get; }
	Microsoft.OData.Edm.EdmSchemaElementKind SchemaElementKind  { public virtual get; }
}

public class Microsoft.OData.Edm.EdmFunctionImport : Microsoft.OData.Edm.EdmOperationImport, IEdmElement, IEdmEntityContainerElement, IEdmFunctionImport, IEdmNamedElement, IEdmOperationImport, IEdmVocabularyAnnotatable {
	public EdmFunctionImport (Microsoft.OData.Edm.IEdmEntityContainer container, string name, Microsoft.OData.Edm.IEdmFunction function)
	public EdmFunctionImport (Microsoft.OData.Edm.IEdmEntityContainer container, string name, Microsoft.OData.Edm.IEdmFunction function, Microsoft.OData.Edm.IEdmExpression entitySetExpression, bool includeInServiceDocument)

	Microsoft.OData.Edm.EdmContainerElementKind ContainerElementKind  { public virtual get; }
	Microsoft.OData.Edm.IEdmFunction Function  { public virtual get; }
	bool IncludeInServiceDocument  { public virtual get; }

	protected virtual string OperationArgumentNullParameterName ()
}

public class Microsoft.OData.Edm.EdmInclude : IEdmInclude {
	public EdmInclude (string alias, string namespaceIncluded)

	string Alias  { public virtual get; }
	string Namespace  { public virtual get; }
}

public class Microsoft.OData.Edm.EdmIncludeAnnotations : IEdmIncludeAnnotations {
	public EdmIncludeAnnotations (string termNamespace, string qualifier, string targetNamespace)

	string Qualifier  { public virtual get; }
	string TargetNamespace  { public virtual get; }
	string TermNamespace  { public virtual get; }
}

public class Microsoft.OData.Edm.EdmModel : Microsoft.OData.Edm.EdmModelBase, IEdmElement, IEdmModel {
	public EdmModel ()

	System.Collections.Generic.IEnumerable`1[[System.String]] DeclaredNamespaces  { public virtual get; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmSchemaElement]] SchemaElements  { public virtual get; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Vocabularies.IEdmVocabularyAnnotation]] VocabularyAnnotations  { public virtual get; }

	public void AddElement (Microsoft.OData.Edm.IEdmSchemaElement element)
	public void AddElements (System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmSchemaElement]] newElements)
	public void AddReferencedModel (Microsoft.OData.Edm.IEdmModel model)
	public void AddVocabularyAnnotation (Microsoft.OData.Edm.Vocabularies.IEdmVocabularyAnnotation annotation)
	public virtual System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Vocabularies.IEdmVocabularyAnnotation]] FindDeclaredVocabularyAnnotations (Microsoft.OData.Edm.Vocabularies.IEdmVocabularyAnnotatable element)
	public virtual System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmStructuredType]] FindDirectlyDerivedTypes (Microsoft.OData.Edm.IEdmStructuredType baseType)
	public void SetVocabularyAnnotation (Microsoft.OData.Edm.Vocabularies.IEdmVocabularyAnnotation annotation)
}

public class Microsoft.OData.Edm.EdmNavigationPropertyBinding : IEdmNavigationPropertyBinding {
	public EdmNavigationPropertyBinding (Microsoft.OData.Edm.IEdmNavigationProperty navigationProperty, Microsoft.OData.Edm.IEdmNavigationSource target)
	public EdmNavigationPropertyBinding (Microsoft.OData.Edm.IEdmNavigationProperty navigationProperty, Microsoft.OData.Edm.IEdmNavigationSource target, Microsoft.OData.Edm.IEdmPathExpression bindingPath)

	Microsoft.OData.Edm.IEdmNavigationProperty NavigationProperty  { public virtual get; }
	Microsoft.OData.Edm.IEdmPathExpression Path  { public virtual get; }
	Microsoft.OData.Edm.IEdmNavigationSource Target  { public virtual get; }
}

public class Microsoft.OData.Edm.EdmOperationParameter : Microsoft.OData.Edm.EdmNamedElement, IEdmElement, IEdmNamedElement, IEdmOperationParameter, IEdmVocabularyAnnotatable {
	public EdmOperationParameter (Microsoft.OData.Edm.IEdmOperation declaringOperation, string name, Microsoft.OData.Edm.IEdmTypeReference type)

	Microsoft.OData.Edm.IEdmOperation DeclaringOperation  { public virtual get; }
	Microsoft.OData.Edm.IEdmTypeReference Type  { public virtual get; }
}

public class Microsoft.OData.Edm.EdmOptionalParameter : Microsoft.OData.Edm.EdmOperationParameter, IEdmElement, IEdmNamedElement, IEdmOperationParameter, IEdmOptionalParameter, IEdmVocabularyAnnotatable {
	public EdmOptionalParameter (Microsoft.OData.Edm.IEdmOperation declaringOperation, string name, Microsoft.OData.Edm.IEdmTypeReference type)
	public EdmOptionalParameter (Microsoft.OData.Edm.IEdmOperation declaringOperation, string name, Microsoft.OData.Edm.IEdmTypeReference type, string defaultValue)

	string DefaultValueString  { public virtual get; }
}

public class Microsoft.OData.Edm.EdmPathExpression : Microsoft.OData.Edm.EdmElement, IEdmElement, IEdmExpression, IEdmPathExpression {
	public EdmPathExpression (System.Collections.Generic.IEnumerable`1[[System.String]] pathSegments)
	public EdmPathExpression (string path)
	public EdmPathExpression (string[] pathSegments)

	Microsoft.OData.Edm.EdmExpressionKind ExpressionKind  { public virtual get; }
	string Path  { public virtual get; }
	System.Collections.Generic.IEnumerable`1[[System.String]] PathSegments  { public virtual get; }
}

public class Microsoft.OData.Edm.EdmPathTypeReference : Microsoft.OData.Edm.EdmTypeReference, IEdmElement, IEdmPathTypeReference, IEdmTypeReference {
	public EdmPathTypeReference (Microsoft.OData.Edm.IEdmPathType definition, bool isNullable)
}

public class Microsoft.OData.Edm.EdmPrimitiveTypeReference : Microsoft.OData.Edm.EdmTypeReference, IEdmElement, IEdmPrimitiveTypeReference, IEdmTypeReference {
	public EdmPrimitiveTypeReference (Microsoft.OData.Edm.IEdmPrimitiveType definition, bool isNullable)
}

public class Microsoft.OData.Edm.EdmReference : IEdmElement, IEdmReference {
	public EdmReference (System.Uri uri)

	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmIncludeAnnotations]] IncludeAnnotations  { public virtual get; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmInclude]] Includes  { public virtual get; }
	System.Uri Uri  { public virtual get; }

	public void AddInclude (Microsoft.OData.Edm.IEdmInclude edmInclude)
	public void AddIncludeAnnotations (Microsoft.OData.Edm.IEdmIncludeAnnotations edmIncludeAnnotations)
}

public class Microsoft.OData.Edm.EdmReferentialConstraint : IEdmReferentialConstraint {
	public EdmReferentialConstraint (System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.EdmReferentialConstraintPropertyPair]] propertyPairs)

	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.EdmReferentialConstraintPropertyPair]] PropertyPairs  { public virtual get; }

	public static Microsoft.OData.Edm.EdmReferentialConstraint Create (System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmStructuralProperty]] dependentProperties, System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmStructuralProperty]] principalProperties)
}

public class Microsoft.OData.Edm.EdmReferentialConstraintPropertyPair {
	public EdmReferentialConstraintPropertyPair (Microsoft.OData.Edm.IEdmStructuralProperty dependentProperty, Microsoft.OData.Edm.IEdmStructuralProperty principalProperty)

	Microsoft.OData.Edm.IEdmStructuralProperty DependentProperty  { public get; }
	Microsoft.OData.Edm.IEdmStructuralProperty PrincipalProperty  { public get; }
}

public class Microsoft.OData.Edm.EdmSingleton : Microsoft.OData.Edm.EdmNavigationSource, IEdmElement, IEdmEntityContainerElement, IEdmNamedElement, IEdmNavigationSource, IEdmSingleton, IEdmVocabularyAnnotatable {
	public EdmSingleton (Microsoft.OData.Edm.IEdmEntityContainer container, string name, Microsoft.OData.Edm.IEdmEntityType entityType)

	Microsoft.OData.Edm.IEdmEntityContainer Container  { public virtual get; }
	Microsoft.OData.Edm.EdmContainerElementKind ContainerElementKind  { public virtual get; }
	Microsoft.OData.Edm.IEdmPathExpression Path  { public virtual get; }
	Microsoft.OData.Edm.IEdmType Type  { public virtual get; }
}

public class Microsoft.OData.Edm.EdmSpatialTypeReference : Microsoft.OData.Edm.EdmPrimitiveTypeReference, IEdmElement, IEdmPrimitiveTypeReference, IEdmSpatialTypeReference, IEdmTypeReference {
	public EdmSpatialTypeReference (Microsoft.OData.Edm.IEdmPrimitiveType definition, bool isNullable)
	public EdmSpatialTypeReference (Microsoft.OData.Edm.IEdmPrimitiveType definition, bool isNullable, System.Nullable`1[[System.Int32]] spatialReferenceIdentifier)

	System.Nullable`1[[System.Int32]] SpatialReferenceIdentifier  { public virtual get; }
}

public class Microsoft.OData.Edm.EdmStringTypeReference : Microsoft.OData.Edm.EdmPrimitiveTypeReference, IEdmElement, IEdmPrimitiveTypeReference, IEdmStringTypeReference, IEdmTypeReference {
	public EdmStringTypeReference (Microsoft.OData.Edm.IEdmPrimitiveType definition, bool isNullable)
	public EdmStringTypeReference (Microsoft.OData.Edm.IEdmPrimitiveType definition, bool isNullable, bool isUnbounded, System.Nullable`1[[System.Int32]] maxLength, System.Nullable`1[[System.Boolean]] isUnicode)

	bool IsUnbounded  { public virtual get; }
	System.Nullable`1[[System.Boolean]] IsUnicode  { public virtual get; }
	System.Nullable`1[[System.Int32]] MaxLength  { public virtual get; }
}

public class Microsoft.OData.Edm.EdmStructuralProperty : Microsoft.OData.Edm.EdmProperty, IEdmElement, IEdmNamedElement, IEdmProperty, IEdmStructuralProperty, IEdmVocabularyAnnotatable {
	public EdmStructuralProperty (Microsoft.OData.Edm.IEdmStructuredType declaringType, string name, Microsoft.OData.Edm.IEdmTypeReference type)
	public EdmStructuralProperty (Microsoft.OData.Edm.IEdmStructuredType declaringType, string name, Microsoft.OData.Edm.IEdmTypeReference type, string defaultValueString)

	string DefaultValueString  { public virtual get; }
	Microsoft.OData.Edm.EdmPropertyKind PropertyKind  { public virtual get; }
}

public class Microsoft.OData.Edm.EdmTemporalTypeReference : Microsoft.OData.Edm.EdmPrimitiveTypeReference, IEdmElement, IEdmPrimitiveTypeReference, IEdmTemporalTypeReference, IEdmTypeReference {
	public EdmTemporalTypeReference (Microsoft.OData.Edm.IEdmPrimitiveType definition, bool isNullable)
	public EdmTemporalTypeReference (Microsoft.OData.Edm.IEdmPrimitiveType definition, bool isNullable, System.Nullable`1[[System.Int32]] precision)

	System.Nullable`1[[System.Int32]] Precision  { public virtual get; }
}

public class Microsoft.OData.Edm.EdmTypeDefinition : Microsoft.OData.Edm.EdmType, IEdmElement, IEdmNamedElement, IEdmSchemaElement, IEdmSchemaType, IEdmType, IEdmTypeDefinition, IEdmVocabularyAnnotatable {
	public EdmTypeDefinition (string namespaceName, string name, Microsoft.OData.Edm.EdmPrimitiveTypeKind underlyingType)
	public EdmTypeDefinition (string namespaceName, string name, Microsoft.OData.Edm.IEdmPrimitiveType underlyingType)

	string Name  { public virtual get; }
	string Namespace  { public virtual get; }
	Microsoft.OData.Edm.EdmSchemaElementKind SchemaElementKind  { public virtual get; }
	Microsoft.OData.Edm.EdmTypeKind TypeKind  { public virtual get; }
	Microsoft.OData.Edm.IEdmPrimitiveType UnderlyingType  { public virtual get; }
}

public class Microsoft.OData.Edm.EdmTypeDefinitionReference : Microsoft.OData.Edm.EdmTypeReference, IEdmElement, IEdmTypeDefinitionReference, IEdmTypeReference {
	public EdmTypeDefinitionReference (Microsoft.OData.Edm.IEdmTypeDefinition typeDefinition, bool isNullable)
	public EdmTypeDefinitionReference (Microsoft.OData.Edm.IEdmTypeDefinition typeDefinition, bool isNullable, bool isUnbounded, System.Nullable`1[[System.Int32]] maxLength, System.Nullable`1[[System.Boolean]] isUnicode, System.Nullable`1[[System.Int32]] precision, System.Nullable`1[[System.Int32]] scale, System.Nullable`1[[System.Int32]] spatialReferenceIdentifier)

	bool IsUnbounded  { public virtual get; }
	System.Nullable`1[[System.Boolean]] IsUnicode  { public virtual get; }
	System.Nullable`1[[System.Int32]] MaxLength  { public virtual get; }
	System.Nullable`1[[System.Int32]] Precision  { public virtual get; }
	System.Nullable`1[[System.Int32]] Scale  { public virtual get; }
	System.Nullable`1[[System.Int32]] SpatialReferenceIdentifier  { public virtual get; }
}

public class Microsoft.OData.Edm.EdmUntypedStructuredTypeReference : Microsoft.OData.Edm.EdmTypeReference, IEdmElement, IEdmStructuredTypeReference, IEdmTypeReference, IEdmUntypedTypeReference {
	public EdmUntypedStructuredTypeReference (Microsoft.OData.Edm.IEdmStructuredType definition)
}

public class Microsoft.OData.Edm.EdmUntypedTypeReference : Microsoft.OData.Edm.EdmTypeReference, IEdmElement, IEdmTypeReference, IEdmUntypedTypeReference {
	public EdmUntypedTypeReference (Microsoft.OData.Edm.IEdmUntypedType definition)
}

public sealed class Microsoft.OData.Edm.EdmNavigationProperty : Microsoft.OData.Edm.EdmProperty, IEdmElement, IEdmNamedElement, IEdmNavigationProperty, IEdmProperty, IEdmVocabularyAnnotatable {
	bool ContainsTarget  { public virtual get; }
	Microsoft.OData.Edm.EdmOnDeleteAction OnDelete  { public virtual get; }
	Microsoft.OData.Edm.IEdmNavigationProperty Partner  { public virtual get; }
	Microsoft.OData.Edm.EdmPropertyKind PropertyKind  { public virtual get; }
	Microsoft.OData.Edm.IEdmReferentialConstraint ReferentialConstraint  { public virtual get; }

	public static Microsoft.OData.Edm.EdmNavigationProperty CreateNavigationProperty (Microsoft.OData.Edm.IEdmStructuredType declaringType, Microsoft.OData.Edm.EdmNavigationPropertyInfo propertyInfo)
	public static Microsoft.OData.Edm.EdmNavigationProperty CreateNavigationPropertyWithPartner (Microsoft.OData.Edm.EdmNavigationPropertyInfo propertyInfo, Microsoft.OData.Edm.EdmNavigationPropertyInfo partnerInfo)
	public static Microsoft.OData.Edm.EdmNavigationProperty CreateNavigationPropertyWithPartner (string propertyName, Microsoft.OData.Edm.IEdmTypeReference propertyType, System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmStructuralProperty]] dependentProperties, System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmStructuralProperty]] principalProperties, bool containsTarget, Microsoft.OData.Edm.EdmOnDeleteAction onDelete, string partnerPropertyName, Microsoft.OData.Edm.IEdmTypeReference partnerPropertyType, System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmStructuralProperty]] partnerDependentProperties, System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmStructuralProperty]] partnerPrincipalProperties, bool partnerContainsTarget, Microsoft.OData.Edm.EdmOnDeleteAction partnerOnDelete)
}

public sealed class Microsoft.OData.Edm.EdmNavigationPropertyInfo {
	public EdmNavigationPropertyInfo ()

	bool ContainsTarget  { public get; public set; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmStructuralProperty]] DependentProperties  { public get; public set; }
	string Name  { public get; public set; }
	Microsoft.OData.Edm.EdmOnDeleteAction OnDelete  { public get; public set; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmStructuralProperty]] PrincipalProperties  { public get; public set; }
	Microsoft.OData.Edm.IEdmEntityType Target  { public get; public set; }
	Microsoft.OData.Edm.EdmMultiplicity TargetMultiplicity  { public get; public set; }

	public Microsoft.OData.Edm.EdmNavigationPropertyInfo Clone ()
}

public sealed class Microsoft.OData.Edm.EdmUntypedStructuredType : Microsoft.OData.Edm.EdmStructuredType, IEdmElement, IEdmNamedElement, IEdmSchemaElement, IEdmSchemaType, IEdmStructuredType, IEdmType, IEdmVocabularyAnnotatable {
	public EdmUntypedStructuredType ()
	public EdmUntypedStructuredType (string namespaceName, string name)

	string Name  { public virtual get; }
	string Namespace  { public virtual get; }
	Microsoft.OData.Edm.EdmSchemaElementKind SchemaElementKind  { public virtual get; }
	Microsoft.OData.Edm.EdmTypeKind TypeKind  { public virtual get; }
}

public enum Microsoft.OData.Edm.Csdl.CsdlTarget : int {
	EntityFramework = 0
	OData = 1
}

public enum Microsoft.OData.Edm.Csdl.EdmVocabularyAnnotationSerializationLocation : int {
	Inline = 0
	OutOfLine = 1
}

public sealed class Microsoft.OData.Edm.Csdl.CsdlConstants {
	public static readonly System.Version EdmxVersion4 = 4.0
	public static readonly System.Version EdmxVersionLatest = 4.0
}

public sealed class Microsoft.OData.Edm.Csdl.SchemaReader {
	public static bool TryParse (System.Collections.Generic.IEnumerable`1[[System.Xml.XmlReader]] readers, out Microsoft.OData.Edm.IEdmModel& model, out System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Validation.EdmError]]& errors)
	public static bool TryParse (System.Collections.Generic.IEnumerable`1[[System.Xml.XmlReader]] readers, Microsoft.OData.Edm.IEdmModel reference, out Microsoft.OData.Edm.IEdmModel& model, out System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Validation.EdmError]]& errors)
	public static bool TryParse (System.Collections.Generic.IEnumerable`1[[System.Xml.XmlReader]] readers, System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmModel]] references, out Microsoft.OData.Edm.IEdmModel& model, out System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Validation.EdmError]]& errors)
}

[
ExtensionAttribute(),
]
public sealed class Microsoft.OData.Edm.Csdl.SchemaWriter {
	[
	ExtensionAttribute(),
	]
	public static bool TryWriteSchema (Microsoft.OData.Edm.IEdmModel model, System.Func`2[[System.String],[System.Xml.XmlWriter]] writerProvider, out System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Validation.EdmError]]& errors)

	[
	ExtensionAttribute(),
	]
	public static bool TryWriteSchema (Microsoft.OData.Edm.IEdmModel model, System.Xml.XmlWriter writer, out System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Validation.EdmError]]& errors)
}

[
ExtensionAttribute(),
]
public sealed class Microsoft.OData.Edm.Csdl.SerializationExtensionMethods {
	[
	ExtensionAttribute(),
	]
	public static System.Version GetEdmxVersion (Microsoft.OData.Edm.IEdmModel model)

	[
	ExtensionAttribute(),
	]
	public static string GetNamespaceAlias (Microsoft.OData.Edm.IEdmModel model, string namespaceName)

	[
	ExtensionAttribute(),
	]
	public static System.Collections.Generic.IEnumerable`1[[System.Collections.Generic.KeyValuePair`2[[System.String],[System.String]]]] GetNamespacePrefixMappings (Microsoft.OData.Edm.IEdmModel model)

	[
	ExtensionAttribute(),
	]
	public static string GetSchemaNamespace (Microsoft.OData.Edm.Vocabularies.IEdmVocabularyAnnotation annotation, Microsoft.OData.Edm.IEdmModel model)

	[
	ExtensionAttribute(),
	]
	public static System.Nullable`1[[Microsoft.OData.Edm.Csdl.EdmVocabularyAnnotationSerializationLocation]] GetSerializationLocation (Microsoft.OData.Edm.Vocabularies.IEdmVocabularyAnnotation annotation, Microsoft.OData.Edm.IEdmModel model)

	[
	ExtensionAttribute(),
	]
	public static bool IsSerializedAsElement (Microsoft.OData.Edm.Vocabularies.IEdmValue value, Microsoft.OData.Edm.IEdmModel model)

	[
	ExtensionAttribute(),
	]
	public static System.Nullable`1[[System.Boolean]] IsValueExplicit (Microsoft.OData.Edm.IEdmEnumMember member, Microsoft.OData.Edm.IEdmModel model)

	[
	ExtensionAttribute(),
	]
	public static void SetEdmxVersion (Microsoft.OData.Edm.IEdmModel model, System.Version version)

	[
	ExtensionAttribute(),
	]
	public static void SetIsSerializedAsElement (Microsoft.OData.Edm.Vocabularies.IEdmValue value, Microsoft.OData.Edm.IEdmModel model, bool isSerializedAsElement)

	[
	ExtensionAttribute(),
	]
	public static void SetIsValueExplicit (Microsoft.OData.Edm.IEdmEnumMember member, Microsoft.OData.Edm.IEdmModel model, System.Nullable`1[[System.Boolean]] isExplicit)

	[
	ExtensionAttribute(),
	]
	public static void SetNamespaceAlias (Microsoft.OData.Edm.IEdmModel model, string namespaceName, string alias)

	[
	ExtensionAttribute(),
	]
	public static void SetNamespacePrefixMappings (Microsoft.OData.Edm.IEdmModel model, System.Collections.Generic.IEnumerable`1[[System.Collections.Generic.KeyValuePair`2[[System.String],[System.String]]]] mappings)

	[
	ExtensionAttribute(),
	]
	public static void SetSchemaNamespace (Microsoft.OData.Edm.Vocabularies.IEdmVocabularyAnnotation annotation, Microsoft.OData.Edm.IEdmModel model, string schemaNamespace)

	[
	ExtensionAttribute(),
	]
	public static void SetSerializationLocation (Microsoft.OData.Edm.Vocabularies.IEdmVocabularyAnnotation annotation, Microsoft.OData.Edm.IEdmModel model, System.Nullable`1[[Microsoft.OData.Edm.Csdl.EdmVocabularyAnnotationSerializationLocation]] location)
}

public class Microsoft.OData.Edm.Csdl.CsdlLocation : Microsoft.OData.Edm.EdmLocation {
	int LineNumber  { public get; }
	int LinePosition  { public get; }
	string Source  { public get; }

	public virtual string ToString ()
}

public class Microsoft.OData.Edm.Csdl.CsdlReader {
	public static Microsoft.OData.Edm.IEdmModel Parse (System.Xml.XmlReader reader)
	public static Microsoft.OData.Edm.IEdmModel Parse (System.Xml.XmlReader reader, Microsoft.OData.Edm.IEdmModel referencedModel)
	public static Microsoft.OData.Edm.IEdmModel Parse (System.Xml.XmlReader reader, System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmModel]] referencedModels)
	public static Microsoft.OData.Edm.IEdmModel Parse (System.Xml.XmlReader reader, System.Func`2[[System.Uri],[System.Xml.XmlReader]] getReferencedModelReaderFunc)
	public static bool TryParse (System.Xml.XmlReader reader, out Microsoft.OData.Edm.IEdmModel& model, out System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Validation.EdmError]]& errors)
	public static bool TryParse (System.Xml.XmlReader reader, Microsoft.OData.Edm.IEdmModel reference, out Microsoft.OData.Edm.IEdmModel& model, out System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Validation.EdmError]]& errors)
	public static bool TryParse (System.Xml.XmlReader reader, bool ignoreUnexpectedAttributesAndElements, out Microsoft.OData.Edm.IEdmModel& model, out System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Validation.EdmError]]& errors)
	public static bool TryParse (System.Xml.XmlReader reader, System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmModel]] references, out Microsoft.OData.Edm.IEdmModel& model, out System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Validation.EdmError]]& errors)
	public static bool TryParse (System.Xml.XmlReader reader, System.Func`2[[System.Uri],[System.Xml.XmlReader]] getReferencedModelReaderFunc, out Microsoft.OData.Edm.IEdmModel& model, out System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Validation.EdmError]]& errors)
	public static bool TryParse (System.Xml.XmlReader reader, System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmModel]] references, Microsoft.OData.Edm.Csdl.CsdlReaderSettings settings, out Microsoft.OData.Edm.IEdmModel& model, out System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Validation.EdmError]]& errors)
}

public class Microsoft.OData.Edm.Csdl.CsdlWriter {
	public static bool TryWriteCsdl (Microsoft.OData.Edm.IEdmModel model, System.Xml.XmlWriter writer, Microsoft.OData.Edm.Csdl.CsdlTarget target, out System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Validation.EdmError]]& errors)
}

[
DebuggerDisplayAttribute(),
]
public class Microsoft.OData.Edm.Csdl.EdmParseException : System.Exception, _Exception, ISerializable {
	public EdmParseException (System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Validation.EdmError]] parseErrors)

	System.Collections.ObjectModel.ReadOnlyCollection`1[[Microsoft.OData.Edm.Validation.EdmError]] Errors  { public get; }
}

public sealed class Microsoft.OData.Edm.Csdl.CsdlReaderSettings {
	public CsdlReaderSettings ()

	System.Func`2[[System.Uri],[System.Xml.XmlReader]] GetReferencedModelReaderFunc  { public get; public set; }
	bool IgnoreUnexpectedAttributesAndElements  { public get; public set; }
}

public enum Microsoft.OData.Edm.Validation.EdmErrorCode : int {
	AllNavigationPropertiesMustBeMapped = 346
	AlreadyDefined = 19
	BadAmbiguousElementBinding = 224
	BadCyclicComplex = 227
	BadCyclicEntity = 229
	BadCyclicEntityContainer = 228
	BadNavigationProperty = 74
	BadNonComputableAssociationEnd = 235
	BadPrincipalPropertiesInReferentialConstraint = 353
	BadProperty = 42
	BadUnresolvedComplexType = 98
	BadUnresolvedEntityContainer = 232
	BadUnresolvedEntitySet = 233
	BadUnresolvedEntityType = 281
	BadUnresolvedEnumMember = 302
	BadUnresolvedEnumType = 360
	BadUnresolvedLabeledElement = 301
	BadUnresolvedNavigationPropertyPath = 363
	BadUnresolvedOperation = 239
	BadUnresolvedParameter = 304
	BadUnresolvedPrimitiveType = 226
	BadUnresolvedProperty = 234
	BadUnresolvedTarget = 361
	BadUnresolvedTerm = 352
	BadUnresolvedType = 225
	BinaryConstantLengthOutOfRange = 332
	BinaryValueCannotHaveEmptyValue = 340
	BoundFunctionOverloadsMustHaveSameReturnType = 368
	BoundOperationMustHaveParameters = 268
	CannotAssertNullableTypeAsNonNullableType = 310
	CannotAssertPrimitiveExpressionAsNonPrimitiveType = 311
	CannotInferEntitySetWithMultipleSetsPerType = 356
	CollectionExpressionNotValidForNonCollectionType = 315
	ComplexTypeBaseTypeCannotBeEdmComplexType = 383
	ComplexTypeMustHaveComplexBaseType = 238
	ComplexTypeMustHaveProperties = 264
	ConcurrencyRedefinedOnSubtypeOfEntitySetType = 145
	ConstructibleEntitySetTypeInvalidFromEntityTypeRemoval = 231
	ContainerElementContainerNameIncorrect = 328
	DeclaringTypeMustBeCorrect = 245
	DeclaringTypeOfNavigationSourceCannotHavePathProperty = 386
	DependentPropertiesMustBelongToDependentEntity = 244
	DuplicateActions = 367
	DuplicateAlias = 321
	DuplicateAnnotation = 319
	DuplicateDependentProperty = 267
	DuplicateDirectValueAnnotationFullName = 354
	DuplicateEntityContainerMemberName = 218
	DuplicateEntityContainerName = 327
	DuplicateFunctions = 366
	DuplicateNavigationPropertyMapping = 345
	DuplicatePropertySpecifiedInEntityKey = 154
	EmptyFile = 12
	EndWithManyMultiplicityCannotHaveOperationsSpecified = 132
	EntityContainerElementMustNotHaveKindOfNone = 339
	EntityKeyMustBeScalar = 128
	EntityKeyMustNotBeBinary = 129
	EntityMustHaveEntityBaseType = 237
	EntitySetCanOnlyBeContainedByASingleNavigationProperty = 341
	EntitySetCanOnlyHaveSingleNavigationPropertyWithContainment = 343
	EntitySetRecursiveNavigationPropertyMappingsMustPointBackToSourceEntitySet = 223
	EntitySetTypeMustBeCollectionOfEntityType = 370
	EntityTypeBaseTypeCannotBeEdmEntityType = 382
	EntityTypeOfEntitySetCannotBeEdmEntityType = 385
	EntityTypeOfSingletonCannotBeEdmEntityType = 384
	EnumMemberMustHaveValue = 206
	EnumMemberValueOutOfRange = 292
	EnumMustHaveIntegerUnderlyingType = 351
	ExpressionEnumKindNotValidForAssertedType = 380
	ExpressionNotValidForTheAssertedType = 314
	ExpressionPrimitiveKindNotValidForAssertedType = 312
	FunctionImportWithParameterShouldNotBeIncludedInServiceDocument = 373
	FunctionMustHaveReturnType = 152
	ImpossibleAnnotationsTarget = 309
	InconsistentNavigationPropertyPartner = 342
	IncorrectNumberOfArguments = 320
	IntegerConstantValueOutOfRange = 330
	InterfaceCriticalCycleInTypeHierarchy = 82
	InterfaceCriticalEnumerableMustNotHaveNullElements = 79
	InterfaceCriticalEnumPropertyValueOutOfRange = 80
	InterfaceCriticalKindValueMismatch = 77
	InterfaceCriticalKindValueUnexpected = 78
	InterfaceCriticalNavigationPartnerInvalid = 81
	InterfaceCriticalPropertyValueMustNotBeNull = 76
	InvalidAbstractComplexType = 220
	InvalidAction = 96
	InvalidAssociation = 62
	InvalidAssociationSet = 279
	InvalidBinary = 283
	InvalidBoolean = 27
	InvalidCastExpressionIncorrectNumberOfOperands = 303
	InvalidDate = 375
	InvalidDateTime = 285
	InvalidDateTimeOffset = 286
	InvalidDecimal = 287
	InvalidDuration = 349
	InvalidElementAnnotation = 299
	InvalidEndEntitySet = 100
	InvalidEntitySetPath = 357
	InvalidEnumMemberPath = 358
	InvalidErrorCodeValue = 0
	InvalidFloatingPoint = 284
	InvalidGuid = 288
	InvalidIfExpressionIncorrectNumberOfOperands = 290
	InvalidInteger = 278
	InvalidIsTypeExpressionIncorrectNumberOfOperands = 293
	InvalidKey = 75
	InvalidLabeledElementExpressionIncorrectNumberOfOperands = 300
	InvalidLong = 277
	InvalidMaxLength = 276
	InvalidMultiplicity = 92
	InvalidMultiplicityOfDependentEnd = 116
	InvalidMultiplicityOfPrincipalEnd = 113
	InvalidName = 17
	InvalidNamespaceName = 163
	InvalidNavigationPropertyType = 258
	InvalidOnDelete = 97
	InvalidOperationImportParameterMode = 333
	InvalidParameterMode = 280
	InvalidPathFirstPathParameterNotMatchingFirstParameterName = 271
	InvalidPathInvalidTypeCastSegment = 250
	InvalidPathTypeCastSegmentMustBeEntityType = 251
	InvalidPathUnknownNavigationProperty = 252
	InvalidPathUnknownTypeCastSegment = 249
	InvalidPathWithNonEntityBindingParameter = 246
	InvalidPolymorphicComplexType = 221
	InvalidPrimitiveValue = 350
	InvalidPropertyInRelationshipConstraint = 111
	InvalidPropertyType = 44
	InvalidQualifiedName = 295
	InvalidRoleInRelationshipConstraint = 110
	InvalidSrid = 275
	InvalidTimeOfDay = 376
	InvalidTypeKindNone = 289
	InvalidTypeName = 294
	InvalidValue = 282
	InvalidVersionNumber = 25
	IsUnboundedCannotBeTrueWhileMaxLengthIsNotNull = 298
	KeyMissingOnEntityType = 159
	KeyPropertyMustBelongToEntity = 242
	KeyPropertyTypeCannotBeEdmPrimitiveType = 259
	MaxLengthOutOfRange = 272
	MetadataDocumentCannotHaveMoreThanOneEntityContainer = 365
	MismatchNumberOfPropertiesInRelationshipConstraint = 114
	MissingAttribute = 15
	MissingType = 18
	NameTooLong = 60
	NavigationMappingMustBeBidirectional = 344
	NavigationPropertyEntityMustNotIndirectlyContainItself = 222
	NavigationPropertyMappingMustPointToValidTargetForProperty = 109
	NavigationPropertyOfCollectionTypeMustNotTargetToSingleton = 371
	NavigationPropertyTypeInvalidBecauseOfBadAssociation = 236
	NavigationPropertyWithCollectionTypeCannotHaveNullableAttribute = 364
	NavigationPropertyWithNonRecursiveContainmentSourceMustBeFromOne = 307
	NavigationPropertyWithRecursiveContainmentSourceMustBeFromZeroOrOne = 306
	NavigationPropertyWithRecursiveContainmentTargetMustBeOptional = 305
	NavigationSourceTypeHasNoKeys = 133
	NoEntitySetsFoundForType = 355
	NoReadersProvided = 296
	NoSchemasProduced = 326
	NullableComplexTypeProperty = 157
	NullCannotBeAssertedToBeANonNullableType = 313
	NullXmlReader = 297
	OpenTypeNotSupported = 117
	OperationCannotHaveEntitySetPathWithUnBoundOperation = 269
	OperationImportCannotImportBoundOperation = 151
	OperationImportEntitySetExpressionIsInvalid = 103
	OperationImportEntityTypeDoesNotMatchEntitySet = 149
	OperationImportParameterIncorrectType = 265
	OperationImportReturnsEntitiesButDoesNotSpecifyEntitySet = 148
	OperationImportSpecifiesEntitySetButDoesNotReturnEntityType = 150
	OperationImportUnsupportedReturnType = 146
	OperationWithCollectionOfAbstractReturnTypeInvalid = 257
	OperationWithEntitySetPathAndReturnTypeTypeNotAssignable = 253
	OperationWithEntitySetPathResolvesToCollectionEntityTypeMismatchesEntityTypeReturnType = 254
	OperationWithEntitySetPathResolvesToEntityTypeMismatchesCollectionEntityTypeReturnType = 255
	OperationWithEntitySetPathReturnTypeInvalid = 256
	OperationWithInvalidEntitySetPathMissingCompletePath = 248
	PathExpressionHasNoEntityContext = 274
	PathIsNotValidForTheGivenContext = 362
	PrecisionOutOfRange = 51
	PrimitiveConstantExpressionNotValidForNonPrimitiveType = 329
	PrimitiveTypeMustNotHaveKindOfNone = 335
	PropertyMustNotHaveKindOfNone = 336
	PropertyTypeCannotBeCollectionOfAbstractType = 337
	QualifierMustBeSimpleName = 359
	RecordExpressionHasExtraProperties = 318
	RecordExpressionMissingRequiredProperty = 317
	RecordExpressionNotValidForNonStructuredType = 316
	ReferencedTypeMustHaveValidName = 322
	ReferenceElementMustContainAtLeastOneIncludeOrIncludeAnnotationsElement = 372
	ReferentialConstraintPrincipalEndMustBelongToAssociation = 243
	RequiredParametersMustPrecedeOptional = 379
	SameRoleReferredInReferentialConstraint = 119
	ScaleOutOfRange = 52
	SchemaElementMustNotHaveKindOfNone = 338
	SimilarRelationshipEnd = 153
	SingleFileExpected = 323
	SingletonTypeMustBeEntityType = 369
	StringConstantLengthOutOfRange = 331
	SystemNamespaceEncountered = 161
	TextNotAllowed = 11
	TypeAnnotationHasExtraProperties = 348
	TypeAnnotationMissingRequiredProperty = 347
	TypeDefinitionUnderlyingTypeCannotBeEdmPrimitiveType = 381
	TypeMismatchRelationshipConstraint = 112
	TypeMustNotHaveKindOfNone = 334
	TypeOfNavigationPropertyCannotHavePathProperty = 387
	TypeSemanticsCouldNotConvertTypeReference = 230
	UnboundFunctionOverloadHasIncorrectReturnType = 219
	UnderlyingTypeIsBadBecauseEnumTypeIsBad = 261
	UnexpectedXmlAttribute = 9
	UnexpectedXmlElement = 10
	UnexpectedXmlNodeType = 8
	UnknownEdmVersion = 325
	UnknownEdmxVersion = 324
	UnresolvedNavigationPropertyBindingPath = 378
	UnresolvedNavigationPropertyPartnerPath = 377
	UnresolvedReferenceUriInEdmxReference = 374
	XmlError = 5
}

public abstract class Microsoft.OData.Edm.Validation.ValidationRule {
	protected ValidationRule ()

	internal abstract void Evaluate (Microsoft.OData.Edm.Validation.ValidationContext context, object item)
}

[
ExtensionAttribute(),
]
public sealed class Microsoft.OData.Edm.Validation.EdmValidator {
	[
	ExtensionAttribute(),
	]
	public static bool Validate (Microsoft.OData.Edm.IEdmModel root, out System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Validation.EdmError]]& errors)

	[
	ExtensionAttribute(),
	]
	public static bool Validate (Microsoft.OData.Edm.IEdmModel root, Microsoft.OData.Edm.Validation.ValidationRuleSet ruleSet, out System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Validation.EdmError]]& errors)

	[
	ExtensionAttribute(),
	]
	public static bool Validate (Microsoft.OData.Edm.IEdmModel root, System.Version version, out System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Validation.EdmError]]& errors)
}

[
ExtensionAttribute(),
]
public sealed class Microsoft.OData.Edm.Validation.ExpressionTypeChecker {
	[
	ExtensionAttribute(),
	]
	public static bool TryCast (Microsoft.OData.Edm.IEdmExpression expression, Microsoft.OData.Edm.IEdmTypeReference type, out System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Validation.EdmError]]& discoveredErrors)

	[
	ExtensionAttribute(),
	]
	public static bool TryCast (Microsoft.OData.Edm.IEdmExpression expression, Microsoft.OData.Edm.IEdmTypeReference type, Microsoft.OData.Edm.IEdmType context, bool matchExactly, out System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Validation.EdmError]]& discoveredErrors)
}

[
ExtensionAttribute(),
]
public sealed class Microsoft.OData.Edm.Validation.ValidationExtensionMethods {
	[
	ExtensionAttribute(),
	]
	public static System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Validation.EdmError]] Errors (Microsoft.OData.Edm.IEdmElement element)

	[
	ExtensionAttribute(),
	]
	public static bool IsBad (Microsoft.OData.Edm.IEdmElement element)

	[
	ExtensionAttribute(),
	]
	public static System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Validation.EdmError]] TypeErrors (Microsoft.OData.Edm.IEdmTypeReference type)
}

public sealed class Microsoft.OData.Edm.Validation.ValidationRules {
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.Vocabularies.IEdmVocabularyAnnotation]] AnnotationInaccessibleTerm = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.Vocabularies.IEdmVocabularyAnnotation]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmBinaryTypeReference]] BinaryTypeReferenceBinaryMaxLengthNegative = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmBinaryTypeReference]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmBinaryTypeReference]] BinaryTypeReferenceBinaryUnboundedNotValidForMaxLength = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmBinaryTypeReference]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmOperation]] BoundOperationMustHaveParameters = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmOperation]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.Vocabularies.IEdmCollectionExpression]] CollectionExpressionAllElementsCorrectType = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.Vocabularies.IEdmCollectionExpression]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmDecimalTypeReference]] DecimalTypeReferencePrecisionOutOfRange = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmDecimalTypeReference]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmDecimalTypeReference]] DecimalTypeReferenceScaleOutOfRange = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmDecimalTypeReference]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.Vocabularies.IEdmDirectValueAnnotation]] DirectValueAnnotationHasXmlSerializableName = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.Vocabularies.IEdmDirectValueAnnotation]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmElement]] ElementDirectValueAnnotationFullNameMustBeUnique = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmElement]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmEntityContainer]] EntityContainerDuplicateEntityContainerMemberName = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmEntityContainer]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmEntityContainerElement]] EntityContainerElementMustNotHaveKindOfNone = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmEntityContainerElement]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmEntityReferenceType]] EntityReferenceTypeInaccessibleEntityType = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmEntityReferenceType]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmEntitySet]] EntitySetCanOnlyBeContainedByASingleNavigationProperty = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmEntitySet]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmEntitySet]] EntitySetRecursiveNavigationPropertyMappingsMustPointBackToSourceEntitySet = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmEntitySet]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmEntitySet]] EntitySetTypeCannotBeEdmEntityType = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmEntitySet]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmEntitySet]] EntitySetTypeMustBeCollectionOfEntityType = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmEntitySet]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmEntityType]] EntityTypeDuplicatePropertyNameSpecifiedInEntityKey = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmEntityType]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmEntityType]] EntityTypeEntityKeyMustBeScalar = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmEntityType]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmEntityType]] EntityTypeInvalidKeyKeyDefinedInBaseClass = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmEntityType]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmEntityType]] EntityTypeInvalidKeyNullablePart = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmEntityType]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmEntityType]] EntityTypeKeyMissingOnEntityType = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmEntityType]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmEntityType]] EntityTypeKeyPropertyMustBelongToEntity = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmEntityType]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmEntityType]] EntityTypeKeyTypeCannotBeEdmPrimitiveType = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmEntityType]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmEnumMember]] EnumMemberValueMustHaveSameTypeAsUnderlyingType = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmEnumMember]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmEnumType]] EnumMustHaveIntegerUnderlyingType = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmEnumType]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmEnumType]] EnumTypeEnumMemberNameAlreadyDefined = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmEnumType]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmEnumType]] EnumUnderlyingTypeCannotBeEdmPrimitiveType = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmEnumType]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.Vocabularies.IEdmApplyExpression]] FunctionApplicationExpressionParametersMatchAppliedFunction = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.Vocabularies.IEdmApplyExpression]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmFunctionImport]] FunctionImportWithParameterShouldNotBeIncludedInServiceDocument = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmFunctionImport]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmFunction]] FunctionMustHaveReturnType = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmFunction]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.Vocabularies.IEdmIfExpression]] IfExpressionAssertCorrectTestType = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.Vocabularies.IEdmIfExpression]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.Vocabularies.IEdmDirectValueAnnotation]] ImmediateValueAnnotationElementAnnotationHasNameAndNamespace = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.Vocabularies.IEdmDirectValueAnnotation]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.Vocabularies.IEdmDirectValueAnnotation]] ImmediateValueAnnotationElementAnnotationIsValid = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.Vocabularies.IEdmDirectValueAnnotation]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmModel]] ModelBoundFunctionOverloadsMustHaveSameReturnType = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmModel]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmModel]] ModelDuplicateEntityContainerName = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmModel]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmModel]] ModelDuplicateSchemaElementName = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmModel]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmNamedElement]] NamedElementNameIsNotAllowed = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmNamedElement]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmNamedElement]] NamedElementNameIsTooLong = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmNamedElement]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmNamedElement]] NamedElementNameMustNotBeEmptyOrWhiteSpace = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmNamedElement]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmNavigationSource]] NavigationMappingMustBeBidirectional = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmNavigationSource]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmNavigationSource]] NavigationPropertyBindingPathMustBeResolvable = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmNavigationSource]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmNavigationProperty]] NavigationPropertyCorrectType = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmNavigationProperty]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmNavigationProperty]] NavigationPropertyDependentEndMultiplicity = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmNavigationProperty]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmNavigationProperty]] NavigationPropertyDependentPropertiesMustBelongToDependentEntity = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmNavigationProperty]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmNavigationProperty]] NavigationPropertyDuplicateDependentProperty = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmNavigationProperty]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmNavigationProperty]] NavigationPropertyEndWithManyMultiplicityCannotHaveOperationsSpecified = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmNavigationProperty]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmNavigationProperty]] NavigationPropertyEntityMustNotIndirectlyContainItself = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmNavigationProperty]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmNavigationProperty]] NavigationPropertyInvalidOperationMultipleEndsInAssociatedNavigationProperties = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmNavigationProperty]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmNavigationSource]] NavigationPropertyMappingMustPointToValidTargetForProperty = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmNavigationSource]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmNavigationSource]] NavigationPropertyMappingsMustBeUnique = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmNavigationSource]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmNavigationProperty]] NavigationPropertyPartnerPathShouldBeResolvable = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmNavigationProperty]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmNavigationProperty]] NavigationPropertyPrincipalEndMultiplicity = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmNavigationProperty]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmNavigationProperty]] NavigationPropertyTypeCannotHavePathTypeProperty = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmNavigationProperty]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmNavigationProperty]] NavigationPropertyTypeMismatchRelationshipConstraint = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmNavigationProperty]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmNavigationProperty]] NavigationPropertyWithNonRecursiveContainmentSourceMustBeFromOne = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmNavigationProperty]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmNavigationProperty]] NavigationPropertyWithRecursiveContainmentSourceMustBeFromZeroOrOne = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmNavigationProperty]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmNavigationProperty]] NavigationPropertyWithRecursiveContainmentTargetMustBeOptional = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmNavigationProperty]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmNavigationSource]] NavigationSourceDeclaringTypeCannotHavePathTypeProperty = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmNavigationSource]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmNavigationSource]] NavigationSourceInaccessibleEntityType = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmNavigationSource]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmNavigationSource]] NavigationSourceTypeHasNoKeys = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmNavigationSource]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmComplexType]] OpenComplexTypeCannotHaveClosedDerivedComplexType = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmComplexType]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmOperation]] OperationEntitySetPathMustBeValid = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmOperation]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmOperationImport]] OperationImportCannotImportBoundOperation = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmOperationImport]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmOperationImport]] OperationImportEntitySetExpressionIsInvalid = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmOperationImport]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmOperationImport]] OperationImportEntityTypeDoesNotMatchEntitySet = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmOperationImport]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmOperation]] OperationParameterNameAlreadyDefinedDuplicate = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmOperation]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmOperation]] OperationReturnTypeCannotBeCollectionOfAbstractType = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmOperation]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmOperation]] OperationReturnTypeEntityTypeMustBeValid = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmOperation]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmOperation]] OperationUnsupportedReturnType = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmOperation]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmOperation]] OptionalParametersMustComeAfterRequiredParameters = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmOperation]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmPrimitiveType]] PrimitiveTypeMustNotHaveKindOfNone = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmPrimitiveType]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.Vocabularies.IEdmPrimitiveValue]] PrimitiveValueValidForType = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.Vocabularies.IEdmPrimitiveValue]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmProperty]] PropertyMustNotHaveKindOfNone = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmProperty]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmProperty]] PropertyTypeCannotBeCollectionOfAbstractType = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmProperty]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.Vocabularies.IEdmPropertyValueBinding]] PropertyValueBindingValueIsCorrectType = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.Vocabularies.IEdmPropertyValueBinding]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.Vocabularies.IEdmRecordExpression]] RecordExpressionPropertiesMatchType = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.Vocabularies.IEdmRecordExpression]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmSchemaElement]] SchemaElementMustNotHaveKindOfNone = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmSchemaElement]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmSchemaElement]] SchemaElementNamespaceIsNotAllowed = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmSchemaElement]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmSchemaElement]] SchemaElementNamespaceIsTooLong = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmSchemaElement]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmSchemaElement]] SchemaElementNamespaceMustNotBeEmptyOrWhiteSpace = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmSchemaElement]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmSchemaElement]] SchemaElementSystemNamespaceEncountered = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmSchemaElement]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmSingleton]] SingletonTypeCannotBeEdmEntityType = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmSingleton]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmSingleton]] SingletonTypeMustBeEntityType = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmSingleton]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmStringTypeReference]] StringTypeReferenceStringMaxLengthNegative = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmStringTypeReference]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmStringTypeReference]] StringTypeReferenceStringUnboundedNotValidForMaxLength = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmStringTypeReference]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmStructuralProperty]] StructuralPropertyInvalidPropertyType = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmStructuralProperty]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmStructuredType]] StructuredTypeBaseTypeCannotBeAbstractType = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmStructuredType]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmStructuredType]] StructuredTypeBaseTypeMustBeSameKindAsDerivedKind = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmStructuredType]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmStructuredType]] StructuredTypeInaccessibleBaseType = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmStructuredType]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmStructuredType]] StructuredTypeInvalidMemberNameMatchesTypeName = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmStructuredType]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmStructuredType]] StructuredTypePropertiesDeclaringTypeMustBeCorrect = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmStructuredType]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmStructuredType]] StructuredTypePropertyNameAlreadyDefined = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmStructuredType]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmTemporalTypeReference]] TemporalTypeReferencePrecisionOutOfRange = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmTemporalTypeReference]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmTypeDefinition]] TypeDefinitionUnderlyingTypeCannotBeEdmPrimitiveType = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmTypeDefinition]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmType]] TypeMustNotHaveKindOfNone = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmType]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmTypeReference]] TypeReferenceInaccessibleSchemaType = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmTypeReference]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmModel]] UnBoundFunctionOverloadsMustHaveIdenticalReturnTypes = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmModel]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.Vocabularies.IEdmVocabularyAnnotatable]] VocabularyAnnotatableNoDuplicateAnnotations = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.Vocabularies.IEdmVocabularyAnnotatable]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.Vocabularies.IEdmVocabularyAnnotation]] VocabularyAnnotationAssertCorrectExpressionType = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.Vocabularies.IEdmVocabularyAnnotation]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.Vocabularies.IEdmVocabularyAnnotation]] VocabularyAnnotationInaccessibleTarget = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.Vocabularies.IEdmVocabularyAnnotation]
}

public class Microsoft.OData.Edm.Validation.EdmError {
	public EdmError (Microsoft.OData.Edm.EdmLocation errorLocation, Microsoft.OData.Edm.Validation.EdmErrorCode errorCode, string errorMessage)

	Microsoft.OData.Edm.Validation.EdmErrorCode ErrorCode  { public get; }
	Microsoft.OData.Edm.EdmLocation ErrorLocation  { public get; }
	string ErrorMessage  { public get; }

	public virtual string ToString ()
}

public class Microsoft.OData.Edm.Validation.ObjectLocation : Microsoft.OData.Edm.EdmLocation {
	object Object  { public get; }

	public virtual string ToString ()
}

public sealed class Microsoft.OData.Edm.Validation.ValidationContext {
	Microsoft.OData.Edm.IEdmModel Model  { public get; }

	public void AddError (Microsoft.OData.Edm.Validation.EdmError error)
	public void AddError (Microsoft.OData.Edm.EdmLocation location, Microsoft.OData.Edm.Validation.EdmErrorCode errorCode, string errorMessage)
	public bool IsBad (Microsoft.OData.Edm.IEdmElement element)
}

public sealed class Microsoft.OData.Edm.Validation.ValidationRule`1 : Microsoft.OData.Edm.Validation.ValidationRule {
	public ValidationRule`1 (Action`2 validate)
}

public sealed class Microsoft.OData.Edm.Validation.ValidationRuleSet : IEnumerable, IEnumerable`1 {
	public ValidationRuleSet (System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Validation.ValidationRule]] rules)
	public ValidationRuleSet (System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Validation.ValidationRule]] baseSet, System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Validation.ValidationRule]] newRules)

	public static Microsoft.OData.Edm.Validation.ValidationRuleSet GetEdmModelRuleSet (System.Version version)
	public virtual System.Collections.Generic.IEnumerator`1[[Microsoft.OData.Edm.Validation.ValidationRule]] GetEnumerator ()
}

public enum Microsoft.OData.Edm.Vocabularies.EdmValueKind : int {
	Binary = 1
	Boolean = 2
	Collection = 3
	Date = 14
	DateTimeOffset = 4
	Decimal = 5
	Duration = 13
	Enum = 6
	Floating = 7
	Guid = 8
	Integer = 9
	None = 0
	Null = 10
	String = 11
	Structured = 12
	TimeOfDay = 15
}

public interface Microsoft.OData.Edm.Vocabularies.IEdmApplyExpression : IEdmElement, IEdmExpression {
	Microsoft.OData.Edm.IEdmFunction AppliedFunction  { public abstract get; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmExpression]] Arguments  { public abstract get; }
}

public interface Microsoft.OData.Edm.Vocabularies.IEdmBinaryConstantExpression : IEdmElement, IEdmExpression, IEdmBinaryValue, IEdmPrimitiveValue, IEdmValue {
}

public interface Microsoft.OData.Edm.Vocabularies.IEdmBinaryValue : IEdmElement, IEdmPrimitiveValue, IEdmValue {
	byte[] Value  { public abstract get; }
}

public interface Microsoft.OData.Edm.Vocabularies.IEdmBooleanConstantExpression : IEdmElement, IEdmExpression, IEdmBooleanValue, IEdmPrimitiveValue, IEdmValue {
}

public interface Microsoft.OData.Edm.Vocabularies.IEdmBooleanValue : IEdmElement, IEdmPrimitiveValue, IEdmValue {
	bool Value  { public abstract get; }
}

public interface Microsoft.OData.Edm.Vocabularies.IEdmCastExpression : IEdmElement, IEdmExpression {
	Microsoft.OData.Edm.IEdmExpression Operand  { public abstract get; }
	Microsoft.OData.Edm.IEdmTypeReference Type  { public abstract get; }
}

public interface Microsoft.OData.Edm.Vocabularies.IEdmCollectionExpression : IEdmElement, IEdmExpression {
	Microsoft.OData.Edm.IEdmTypeReference DeclaredType  { public abstract get; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmExpression]] Elements  { public abstract get; }
}

public interface Microsoft.OData.Edm.Vocabularies.IEdmCollectionValue : IEdmElement, IEdmValue {
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Vocabularies.IEdmDelayedValue]] Elements  { public abstract get; }
}

public interface Microsoft.OData.Edm.Vocabularies.IEdmDateConstantExpression : IEdmElement, IEdmExpression, IEdmDateValue, IEdmPrimitiveValue, IEdmValue {
}

public interface Microsoft.OData.Edm.Vocabularies.IEdmDateTimeOffsetConstantExpression : IEdmElement, IEdmExpression, IEdmDateTimeOffsetValue, IEdmPrimitiveValue, IEdmValue {
}

public interface Microsoft.OData.Edm.Vocabularies.IEdmDateTimeOffsetValue : IEdmElement, IEdmPrimitiveValue, IEdmValue {
	System.DateTimeOffset Value  { public abstract get; }
}

public interface Microsoft.OData.Edm.Vocabularies.IEdmDateValue : IEdmElement, IEdmPrimitiveValue, IEdmValue {
	Microsoft.OData.Edm.Date Value  { public abstract get; }
}

public interface Microsoft.OData.Edm.Vocabularies.IEdmDecimalConstantExpression : IEdmElement, IEdmExpression, IEdmDecimalValue, IEdmPrimitiveValue, IEdmValue {
}

public interface Microsoft.OData.Edm.Vocabularies.IEdmDecimalValue : IEdmElement, IEdmPrimitiveValue, IEdmValue {
	decimal Value  { public abstract get; }
}

public interface Microsoft.OData.Edm.Vocabularies.IEdmDelayedValue {
	Microsoft.OData.Edm.Vocabularies.IEdmValue Value  { public abstract get; }
}

public interface Microsoft.OData.Edm.Vocabularies.IEdmDirectValueAnnotation : IEdmElement, IEdmNamedElement {
	string NamespaceUri  { public abstract get; }
	object Value  { public abstract get; }
}

public interface Microsoft.OData.Edm.Vocabularies.IEdmDirectValueAnnotationBinding {
	Microsoft.OData.Edm.IEdmElement Element  { public abstract get; }
	string Name  { public abstract get; }
	string NamespaceUri  { public abstract get; }
	object Value  { public abstract get; }
}

public interface Microsoft.OData.Edm.Vocabularies.IEdmDirectValueAnnotationsManager {
	object GetAnnotationValue (Microsoft.OData.Edm.IEdmElement element, string namespaceName, string localName)
	object[] GetAnnotationValues (System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Vocabularies.IEdmDirectValueAnnotationBinding]] annotations)
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Vocabularies.IEdmDirectValueAnnotation]] GetDirectValueAnnotations (Microsoft.OData.Edm.IEdmElement element)
	void SetAnnotationValue (Microsoft.OData.Edm.IEdmElement element, string namespaceName, string localName, object value)
	void SetAnnotationValues (System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Vocabularies.IEdmDirectValueAnnotationBinding]] annotations)
}

public interface Microsoft.OData.Edm.Vocabularies.IEdmDurationConstantExpression : IEdmElement, IEdmExpression, IEdmDurationValue, IEdmPrimitiveValue, IEdmValue {
}

public interface Microsoft.OData.Edm.Vocabularies.IEdmDurationValue : IEdmElement, IEdmPrimitiveValue, IEdmValue {
	System.TimeSpan Value  { public abstract get; }
}

public interface Microsoft.OData.Edm.Vocabularies.IEdmEnumMemberExpression : IEdmElement, IEdmExpression {
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmEnumMember]] EnumMembers  { public abstract get; }
}

public interface Microsoft.OData.Edm.Vocabularies.IEdmEnumValue : IEdmElement, IEdmPrimitiveValue, IEdmValue {
	Microsoft.OData.Edm.IEdmEnumMemberValue Value  { public abstract get; }
}

public interface Microsoft.OData.Edm.Vocabularies.IEdmFloatingConstantExpression : IEdmElement, IEdmExpression, IEdmFloatingValue, IEdmPrimitiveValue, IEdmValue {
}

public interface Microsoft.OData.Edm.Vocabularies.IEdmFloatingValue : IEdmElement, IEdmPrimitiveValue, IEdmValue {
	double Value  { public abstract get; }
}

public interface Microsoft.OData.Edm.Vocabularies.IEdmGuidConstantExpression : IEdmElement, IEdmExpression, IEdmGuidValue, IEdmPrimitiveValue, IEdmValue {
}

public interface Microsoft.OData.Edm.Vocabularies.IEdmGuidValue : IEdmElement, IEdmPrimitiveValue, IEdmValue {
	System.Guid Value  { public abstract get; }
}

public interface Microsoft.OData.Edm.Vocabularies.IEdmIfExpression : IEdmElement, IEdmExpression {
	Microsoft.OData.Edm.IEdmExpression FalseExpression  { public abstract get; }
	Microsoft.OData.Edm.IEdmExpression TestExpression  { public abstract get; }
	Microsoft.OData.Edm.IEdmExpression TrueExpression  { public abstract get; }
}

public interface Microsoft.OData.Edm.Vocabularies.IEdmIntegerConstantExpression : IEdmElement, IEdmExpression, IEdmIntegerValue, IEdmPrimitiveValue, IEdmValue {
}

public interface Microsoft.OData.Edm.Vocabularies.IEdmIntegerValue : IEdmElement, IEdmPrimitiveValue, IEdmValue {
	long Value  { public abstract get; }
}

public interface Microsoft.OData.Edm.Vocabularies.IEdmIsTypeExpression : IEdmElement, IEdmExpression {
	Microsoft.OData.Edm.IEdmExpression Operand  { public abstract get; }
	Microsoft.OData.Edm.IEdmTypeReference Type  { public abstract get; }
}

public interface Microsoft.OData.Edm.Vocabularies.IEdmLabeledExpression : IEdmElement, IEdmExpression, IEdmNamedElement {
	Microsoft.OData.Edm.IEdmExpression Expression  { public abstract get; }
}

public interface Microsoft.OData.Edm.Vocabularies.IEdmLabeledExpressionReferenceExpression : IEdmElement, IEdmExpression {
	Microsoft.OData.Edm.Vocabularies.IEdmLabeledExpression ReferencedLabeledExpression  { public abstract get; }
}

public interface Microsoft.OData.Edm.Vocabularies.IEdmNullExpression : IEdmElement, IEdmExpression, IEdmNullValue, IEdmValue {
}

public interface Microsoft.OData.Edm.Vocabularies.IEdmNullValue : IEdmElement, IEdmValue {
}

public interface Microsoft.OData.Edm.Vocabularies.IEdmPrimitiveValue : IEdmElement, IEdmValue {
}

public interface Microsoft.OData.Edm.Vocabularies.IEdmPropertyConstructor : IEdmElement {
	string Name  { public abstract get; }
	Microsoft.OData.Edm.IEdmExpression Value  { public abstract get; }
}

public interface Microsoft.OData.Edm.Vocabularies.IEdmPropertyValue : IEdmDelayedValue {
	string Name  { public abstract get; }
}

public interface Microsoft.OData.Edm.Vocabularies.IEdmPropertyValueBinding : IEdmElement {
	Microsoft.OData.Edm.IEdmProperty BoundProperty  { public abstract get; }
	Microsoft.OData.Edm.IEdmExpression Value  { public abstract get; }
}

public interface Microsoft.OData.Edm.Vocabularies.IEdmRecordExpression : IEdmElement, IEdmExpression {
	Microsoft.OData.Edm.IEdmStructuredTypeReference DeclaredType  { public abstract get; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Vocabularies.IEdmPropertyConstructor]] Properties  { public abstract get; }
}

public interface Microsoft.OData.Edm.Vocabularies.IEdmStringConstantExpression : IEdmElement, IEdmExpression, IEdmPrimitiveValue, IEdmStringValue, IEdmValue {
}

public interface Microsoft.OData.Edm.Vocabularies.IEdmStringValue : IEdmElement, IEdmPrimitiveValue, IEdmValue {
	string Value  { public abstract get; }
}

public interface Microsoft.OData.Edm.Vocabularies.IEdmStructuredValue : IEdmElement, IEdmValue {
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Vocabularies.IEdmPropertyValue]] PropertyValues  { public abstract get; }

	Microsoft.OData.Edm.Vocabularies.IEdmPropertyValue FindPropertyValue (string propertyName)
}

public interface Microsoft.OData.Edm.Vocabularies.IEdmTerm : IEdmElement, IEdmNamedElement, IEdmSchemaElement, IEdmVocabularyAnnotatable {
	string AppliesTo  { public abstract get; }
	string DefaultValue  { public abstract get; }
	Microsoft.OData.Edm.IEdmTypeReference Type  { public abstract get; }
}

public interface Microsoft.OData.Edm.Vocabularies.IEdmTimeOfDayConstantExpression : IEdmElement, IEdmExpression, IEdmPrimitiveValue, IEdmTimeOfDayValue, IEdmValue {
}

public interface Microsoft.OData.Edm.Vocabularies.IEdmTimeOfDayValue : IEdmElement, IEdmPrimitiveValue, IEdmValue {
	Microsoft.OData.Edm.TimeOfDay Value  { public abstract get; }
}

public interface Microsoft.OData.Edm.Vocabularies.IEdmValue : IEdmElement {
	Microsoft.OData.Edm.IEdmTypeReference Type  { public abstract get; }
	Microsoft.OData.Edm.Vocabularies.EdmValueKind ValueKind  { public abstract get; }
}

public interface Microsoft.OData.Edm.Vocabularies.IEdmVocabularyAnnotatable : IEdmElement {
}

public interface Microsoft.OData.Edm.Vocabularies.IEdmVocabularyAnnotation : IEdmElement {
	string Qualifier  { public abstract get; }
	Microsoft.OData.Edm.Vocabularies.IEdmVocabularyAnnotatable Target  { public abstract get; }
	Microsoft.OData.Edm.Vocabularies.IEdmTerm Term  { public abstract get; }
	Microsoft.OData.Edm.IEdmExpression Value  { public abstract get; }
}

public abstract class Microsoft.OData.Edm.Vocabularies.EdmValue : IEdmElement, IEdmDelayedValue, IEdmValue {
	protected EdmValue (Microsoft.OData.Edm.IEdmTypeReference type)

	Microsoft.OData.Edm.IEdmTypeReference Type  { public virtual get; }
	Microsoft.OData.Edm.Vocabularies.EdmValueKind ValueKind  { public abstract get; }
}

public class Microsoft.OData.Edm.Vocabularies.EdmApplyExpression : Microsoft.OData.Edm.EdmElement, IEdmElement, IEdmExpression, IEdmApplyExpression {
	public EdmApplyExpression (Microsoft.OData.Edm.IEdmFunction appliedFunction, Microsoft.OData.Edm.IEdmExpression[] arguments)
	public EdmApplyExpression (Microsoft.OData.Edm.IEdmFunction appliedFunction, System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmExpression]] arguments)

	Microsoft.OData.Edm.IEdmFunction AppliedFunction  { public virtual get; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmExpression]] Arguments  { public virtual get; }
	Microsoft.OData.Edm.EdmExpressionKind ExpressionKind  { public virtual get; }
}

public class Microsoft.OData.Edm.Vocabularies.EdmBinaryConstant : Microsoft.OData.Edm.Vocabularies.EdmValue, IEdmElement, IEdmExpression, IEdmBinaryConstantExpression, IEdmBinaryValue, IEdmDelayedValue, IEdmPrimitiveValue, IEdmValue {
	public EdmBinaryConstant (byte[] value)
	public EdmBinaryConstant (Microsoft.OData.Edm.IEdmBinaryTypeReference type, byte[] value)

	Microsoft.OData.Edm.EdmExpressionKind ExpressionKind  { public virtual get; }
	byte[] Value  { public virtual get; }
	Microsoft.OData.Edm.Vocabularies.EdmValueKind ValueKind  { public virtual get; }
}

public class Microsoft.OData.Edm.Vocabularies.EdmBooleanConstant : Microsoft.OData.Edm.Vocabularies.EdmValue, IEdmElement, IEdmExpression, IEdmBooleanConstantExpression, IEdmBooleanValue, IEdmDelayedValue, IEdmPrimitiveValue, IEdmValue {
	public EdmBooleanConstant (bool value)
	public EdmBooleanConstant (Microsoft.OData.Edm.IEdmPrimitiveTypeReference type, bool value)

	Microsoft.OData.Edm.EdmExpressionKind ExpressionKind  { public virtual get; }
	bool Value  { public virtual get; }
	Microsoft.OData.Edm.Vocabularies.EdmValueKind ValueKind  { public virtual get; }
}

public class Microsoft.OData.Edm.Vocabularies.EdmCastExpression : Microsoft.OData.Edm.EdmElement, IEdmElement, IEdmExpression, IEdmCastExpression {
	public EdmCastExpression (Microsoft.OData.Edm.IEdmExpression operand, Microsoft.OData.Edm.IEdmTypeReference type)

	Microsoft.OData.Edm.EdmExpressionKind ExpressionKind  { public virtual get; }
	Microsoft.OData.Edm.IEdmExpression Operand  { public virtual get; }
	Microsoft.OData.Edm.IEdmTypeReference Type  { public virtual get; }
}

public class Microsoft.OData.Edm.Vocabularies.EdmCollectionExpression : Microsoft.OData.Edm.EdmElement, IEdmElement, IEdmExpression, IEdmCollectionExpression {
	public EdmCollectionExpression (Microsoft.OData.Edm.IEdmExpression[] elements)
	public EdmCollectionExpression (System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmExpression]] elements)
	public EdmCollectionExpression (Microsoft.OData.Edm.IEdmTypeReference declaredType, Microsoft.OData.Edm.IEdmExpression[] elements)
	public EdmCollectionExpression (Microsoft.OData.Edm.IEdmTypeReference declaredType, System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmExpression]] elements)

	Microsoft.OData.Edm.IEdmTypeReference DeclaredType  { public virtual get; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmExpression]] Elements  { public virtual get; }
	Microsoft.OData.Edm.EdmExpressionKind ExpressionKind  { public virtual get; }
}

public class Microsoft.OData.Edm.Vocabularies.EdmCollectionValue : Microsoft.OData.Edm.Vocabularies.EdmValue, IEdmElement, IEdmCollectionValue, IEdmDelayedValue, IEdmValue {
	public EdmCollectionValue (Microsoft.OData.Edm.IEdmCollectionTypeReference type, System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Vocabularies.IEdmDelayedValue]] elements)

	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Vocabularies.IEdmDelayedValue]] Elements  { public virtual get; }
	Microsoft.OData.Edm.Vocabularies.EdmValueKind ValueKind  { public virtual get; }
}

public class Microsoft.OData.Edm.Vocabularies.EdmDateConstant : Microsoft.OData.Edm.Vocabularies.EdmValue, IEdmElement, IEdmExpression, IEdmDateConstantExpression, IEdmDateValue, IEdmDelayedValue, IEdmPrimitiveValue, IEdmValue {
	public EdmDateConstant (Microsoft.OData.Edm.Date value)
	public EdmDateConstant (Microsoft.OData.Edm.IEdmPrimitiveTypeReference type, Microsoft.OData.Edm.Date value)

	Microsoft.OData.Edm.EdmExpressionKind ExpressionKind  { public virtual get; }
	Microsoft.OData.Edm.Date Value  { public virtual get; }
	Microsoft.OData.Edm.Vocabularies.EdmValueKind ValueKind  { public virtual get; }
}

public class Microsoft.OData.Edm.Vocabularies.EdmDateTimeOffsetConstant : Microsoft.OData.Edm.Vocabularies.EdmValue, IEdmElement, IEdmExpression, IEdmDateTimeOffsetConstantExpression, IEdmDateTimeOffsetValue, IEdmDelayedValue, IEdmPrimitiveValue, IEdmValue {
	public EdmDateTimeOffsetConstant (System.DateTimeOffset value)
	public EdmDateTimeOffsetConstant (Microsoft.OData.Edm.IEdmTemporalTypeReference type, System.DateTimeOffset value)

	Microsoft.OData.Edm.EdmExpressionKind ExpressionKind  { public virtual get; }
	System.DateTimeOffset Value  { public virtual get; }
	Microsoft.OData.Edm.Vocabularies.EdmValueKind ValueKind  { public virtual get; }
}

public class Microsoft.OData.Edm.Vocabularies.EdmDecimalConstant : Microsoft.OData.Edm.Vocabularies.EdmValue, IEdmElement, IEdmExpression, IEdmDecimalConstantExpression, IEdmDecimalValue, IEdmDelayedValue, IEdmPrimitiveValue, IEdmValue {
	public EdmDecimalConstant (decimal value)
	public EdmDecimalConstant (Microsoft.OData.Edm.IEdmDecimalTypeReference type, decimal value)

	Microsoft.OData.Edm.EdmExpressionKind ExpressionKind  { public virtual get; }
	decimal Value  { public virtual get; }
	Microsoft.OData.Edm.Vocabularies.EdmValueKind ValueKind  { public virtual get; }
}

public class Microsoft.OData.Edm.Vocabularies.EdmDirectValueAnnotation : Microsoft.OData.Edm.EdmNamedElement, IEdmElement, IEdmNamedElement, IEdmDirectValueAnnotation {
	public EdmDirectValueAnnotation (string namespaceUri, string name, object value)

	string NamespaceUri  { public virtual get; }
	object Value  { public virtual get; }
}

public class Microsoft.OData.Edm.Vocabularies.EdmDirectValueAnnotationBinding : IEdmDirectValueAnnotationBinding {
	public EdmDirectValueAnnotationBinding (Microsoft.OData.Edm.IEdmElement element, string namespaceUri, string name)
	public EdmDirectValueAnnotationBinding (Microsoft.OData.Edm.IEdmElement element, string namespaceUri, string name, object value)

	Microsoft.OData.Edm.IEdmElement Element  { public virtual get; }
	string Name  { public virtual get; }
	string NamespaceUri  { public virtual get; }
	object Value  { public virtual get; }
}

public class Microsoft.OData.Edm.Vocabularies.EdmDirectValueAnnotationsManager : IEdmDirectValueAnnotationsManager {
	public EdmDirectValueAnnotationsManager ()

	public virtual object GetAnnotationValue (Microsoft.OData.Edm.IEdmElement element, string namespaceName, string localName)
	public virtual object[] GetAnnotationValues (System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Vocabularies.IEdmDirectValueAnnotationBinding]] annotations)
	protected virtual System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Vocabularies.IEdmDirectValueAnnotation]] GetAttachedAnnotations (Microsoft.OData.Edm.IEdmElement element)
	public virtual System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Vocabularies.IEdmDirectValueAnnotation]] GetDirectValueAnnotations (Microsoft.OData.Edm.IEdmElement element)
	public virtual void SetAnnotationValue (Microsoft.OData.Edm.IEdmElement element, string namespaceName, string localName, object value)
	public virtual void SetAnnotationValues (System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Vocabularies.IEdmDirectValueAnnotationBinding]] annotations)
}

public class Microsoft.OData.Edm.Vocabularies.EdmDurationConstant : Microsoft.OData.Edm.Vocabularies.EdmValue, IEdmElement, IEdmExpression, IEdmDelayedValue, IEdmDurationConstantExpression, IEdmDurationValue, IEdmPrimitiveValue, IEdmValue {
	public EdmDurationConstant (System.TimeSpan value)
	public EdmDurationConstant (Microsoft.OData.Edm.IEdmTemporalTypeReference type, System.TimeSpan value)

	Microsoft.OData.Edm.EdmExpressionKind ExpressionKind  { public virtual get; }
	System.TimeSpan Value  { public virtual get; }
	Microsoft.OData.Edm.Vocabularies.EdmValueKind ValueKind  { public virtual get; }
}

public class Microsoft.OData.Edm.Vocabularies.EdmEnumMemberExpression : Microsoft.OData.Edm.EdmElement, IEdmElement, IEdmExpression, IEdmEnumMemberExpression {
	public EdmEnumMemberExpression (Microsoft.OData.Edm.IEdmEnumMember[] enumMembers)

	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmEnumMember]] EnumMembers  { public virtual get; }
	Microsoft.OData.Edm.EdmExpressionKind ExpressionKind  { public virtual get; }
}

public class Microsoft.OData.Edm.Vocabularies.EdmEnumValue : Microsoft.OData.Edm.Vocabularies.EdmValue, IEdmElement, IEdmDelayedValue, IEdmEnumValue, IEdmPrimitiveValue, IEdmValue {
	public EdmEnumValue (Microsoft.OData.Edm.IEdmEnumTypeReference type, Microsoft.OData.Edm.IEdmEnumMember member)
	public EdmEnumValue (Microsoft.OData.Edm.IEdmEnumTypeReference type, Microsoft.OData.Edm.IEdmEnumMemberValue value)

	Microsoft.OData.Edm.IEdmEnumMemberValue Value  { public virtual get; }
	Microsoft.OData.Edm.Vocabularies.EdmValueKind ValueKind  { public virtual get; }
}

public class Microsoft.OData.Edm.Vocabularies.EdmExpressionEvaluator {
	public EdmExpressionEvaluator (System.Collections.Generic.IDictionary`2[[Microsoft.OData.Edm.IEdmOperation],[System.Func`2[[Microsoft.OData.Edm.Vocabularies.IEdmValue[]],[Microsoft.OData.Edm.Vocabularies.IEdmValue]]]] builtInFunctions)
	public EdmExpressionEvaluator (System.Collections.Generic.IDictionary`2[[Microsoft.OData.Edm.IEdmOperation],[System.Func`2[[Microsoft.OData.Edm.Vocabularies.IEdmValue[]],[Microsoft.OData.Edm.Vocabularies.IEdmValue]]]] builtInFunctions, System.Func`3[[System.String],[Microsoft.OData.Edm.Vocabularies.IEdmValue[]],[Microsoft.OData.Edm.Vocabularies.IEdmValue]] lastChanceOperationApplier)
	public EdmExpressionEvaluator (System.Collections.Generic.IDictionary`2[[Microsoft.OData.Edm.IEdmOperation],[System.Func`2[[Microsoft.OData.Edm.Vocabularies.IEdmValue[]],[Microsoft.OData.Edm.Vocabularies.IEdmValue]]]] builtInFunctions, System.Func`3[[System.String],[Microsoft.OData.Edm.Vocabularies.IEdmValue[]],[Microsoft.OData.Edm.Vocabularies.IEdmValue]] lastChanceOperationApplier, System.Func`5[[Microsoft.OData.Edm.IEdmModel],[Microsoft.OData.Edm.IEdmType],[System.String],[System.String],[Microsoft.OData.Edm.IEdmExpression]] getAnnotationExpressionForType, System.Func`6[[Microsoft.OData.Edm.IEdmModel],[Microsoft.OData.Edm.IEdmType],[System.String],[System.String],[System.String],[Microsoft.OData.Edm.IEdmExpression]] getAnnotationExpressionForProperty, Microsoft.OData.Edm.IEdmModel edmModel)

	System.Func`3[[System.String],[Microsoft.OData.Edm.IEdmModel],[Microsoft.OData.Edm.IEdmType]] ResolveTypeFromName  { protected get; protected set; }

	public Microsoft.OData.Edm.Vocabularies.IEdmValue Evaluate (Microsoft.OData.Edm.IEdmExpression expression)
	public Microsoft.OData.Edm.Vocabularies.IEdmValue Evaluate (Microsoft.OData.Edm.IEdmExpression expression, Microsoft.OData.Edm.Vocabularies.IEdmStructuredValue context)
	public Microsoft.OData.Edm.Vocabularies.IEdmValue Evaluate (Microsoft.OData.Edm.IEdmExpression expression, Microsoft.OData.Edm.Vocabularies.IEdmStructuredValue context, Microsoft.OData.Edm.IEdmTypeReference targetType)
	protected static Microsoft.OData.Edm.IEdmType FindEdmType (string edmTypeName, Microsoft.OData.Edm.IEdmModel edmModel)
}

public class Microsoft.OData.Edm.Vocabularies.EdmFloatingConstant : Microsoft.OData.Edm.Vocabularies.EdmValue, IEdmElement, IEdmExpression, IEdmDelayedValue, IEdmFloatingConstantExpression, IEdmFloatingValue, IEdmPrimitiveValue, IEdmValue {
	public EdmFloatingConstant (double value)
	public EdmFloatingConstant (Microsoft.OData.Edm.IEdmPrimitiveTypeReference type, double value)

	Microsoft.OData.Edm.EdmExpressionKind ExpressionKind  { public virtual get; }
	double Value  { public virtual get; }
	Microsoft.OData.Edm.Vocabularies.EdmValueKind ValueKind  { public virtual get; }
}

public class Microsoft.OData.Edm.Vocabularies.EdmGuidConstant : Microsoft.OData.Edm.Vocabularies.EdmValue, IEdmElement, IEdmExpression, IEdmDelayedValue, IEdmGuidConstantExpression, IEdmGuidValue, IEdmPrimitiveValue, IEdmValue {
	public EdmGuidConstant (System.Guid value)
	public EdmGuidConstant (Microsoft.OData.Edm.IEdmPrimitiveTypeReference type, System.Guid value)

	Microsoft.OData.Edm.EdmExpressionKind ExpressionKind  { public virtual get; }
	System.Guid Value  { public virtual get; }
	Microsoft.OData.Edm.Vocabularies.EdmValueKind ValueKind  { public virtual get; }
}

public class Microsoft.OData.Edm.Vocabularies.EdmIfExpression : Microsoft.OData.Edm.EdmElement, IEdmElement, IEdmExpression, IEdmIfExpression {
	public EdmIfExpression (Microsoft.OData.Edm.IEdmExpression testExpression, Microsoft.OData.Edm.IEdmExpression trueExpression, Microsoft.OData.Edm.IEdmExpression falseExpression)

	Microsoft.OData.Edm.EdmExpressionKind ExpressionKind  { public virtual get; }
	Microsoft.OData.Edm.IEdmExpression FalseExpression  { public virtual get; }
	Microsoft.OData.Edm.IEdmExpression TestExpression  { public virtual get; }
	Microsoft.OData.Edm.IEdmExpression TrueExpression  { public virtual get; }
}

public class Microsoft.OData.Edm.Vocabularies.EdmIntegerConstant : Microsoft.OData.Edm.Vocabularies.EdmValue, IEdmElement, IEdmExpression, IEdmDelayedValue, IEdmIntegerConstantExpression, IEdmIntegerValue, IEdmPrimitiveValue, IEdmValue {
	public EdmIntegerConstant (long value)
	public EdmIntegerConstant (Microsoft.OData.Edm.IEdmPrimitiveTypeReference type, long value)

	Microsoft.OData.Edm.EdmExpressionKind ExpressionKind  { public virtual get; }
	long Value  { public virtual get; }
	Microsoft.OData.Edm.Vocabularies.EdmValueKind ValueKind  { public virtual get; }
}

public class Microsoft.OData.Edm.Vocabularies.EdmIsTypeExpression : Microsoft.OData.Edm.EdmElement, IEdmElement, IEdmExpression, IEdmIsTypeExpression {
	public EdmIsTypeExpression (Microsoft.OData.Edm.IEdmExpression operand, Microsoft.OData.Edm.IEdmTypeReference type)

	Microsoft.OData.Edm.EdmExpressionKind ExpressionKind  { public virtual get; }
	Microsoft.OData.Edm.IEdmExpression Operand  { public virtual get; }
	Microsoft.OData.Edm.IEdmTypeReference Type  { public virtual get; }
}

public class Microsoft.OData.Edm.Vocabularies.EdmLabeledExpression : Microsoft.OData.Edm.EdmElement, IEdmElement, IEdmExpression, IEdmNamedElement, IEdmLabeledExpression {
	public EdmLabeledExpression (string name, Microsoft.OData.Edm.IEdmExpression expression)

	Microsoft.OData.Edm.IEdmExpression Expression  { public virtual get; }
	Microsoft.OData.Edm.EdmExpressionKind ExpressionKind  { public virtual get; }
	string Name  { public virtual get; }
}

public class Microsoft.OData.Edm.Vocabularies.EdmLabeledExpressionReferenceExpression : Microsoft.OData.Edm.EdmElement, IEdmElement, IEdmExpression, IEdmLabeledExpressionReferenceExpression {
	public EdmLabeledExpressionReferenceExpression ()
	public EdmLabeledExpressionReferenceExpression (Microsoft.OData.Edm.Vocabularies.IEdmLabeledExpression referencedLabeledExpression)

	Microsoft.OData.Edm.EdmExpressionKind ExpressionKind  { public virtual get; }
	Microsoft.OData.Edm.Vocabularies.IEdmLabeledExpression ReferencedLabeledExpression  { public virtual get; public set; }
}

public class Microsoft.OData.Edm.Vocabularies.EdmNavigationPropertyPathExpression : Microsoft.OData.Edm.EdmPathExpression, IEdmElement, IEdmExpression, IEdmPathExpression {
	public EdmNavigationPropertyPathExpression (System.Collections.Generic.IEnumerable`1[[System.String]] pathSegments)
	public EdmNavigationPropertyPathExpression (string path)
	public EdmNavigationPropertyPathExpression (string[] pathSegments)

	Microsoft.OData.Edm.EdmExpressionKind ExpressionKind  { public virtual get; }
}

public class Microsoft.OData.Edm.Vocabularies.EdmNullExpression : Microsoft.OData.Edm.Vocabularies.EdmValue, IEdmElement, IEdmExpression, IEdmDelayedValue, IEdmNullExpression, IEdmNullValue, IEdmValue {
	public static Microsoft.OData.Edm.Vocabularies.EdmNullExpression Instance = Microsoft.OData.Edm.Vocabularies.EdmNullExpression

	Microsoft.OData.Edm.EdmExpressionKind ExpressionKind  { public virtual get; }
	Microsoft.OData.Edm.Vocabularies.EdmValueKind ValueKind  { public virtual get; }
}

public class Microsoft.OData.Edm.Vocabularies.EdmPropertyConstructor : Microsoft.OData.Edm.EdmElement, IEdmElement, IEdmPropertyConstructor {
	public EdmPropertyConstructor (string name, Microsoft.OData.Edm.IEdmExpression value)

	string Name  { public virtual get; }
	Microsoft.OData.Edm.IEdmExpression Value  { public virtual get; }
}

public class Microsoft.OData.Edm.Vocabularies.EdmPropertyPathExpression : Microsoft.OData.Edm.EdmPathExpression, IEdmElement, IEdmExpression, IEdmPathExpression {
	public EdmPropertyPathExpression (System.Collections.Generic.IEnumerable`1[[System.String]] pathSegments)
	public EdmPropertyPathExpression (string path)
	public EdmPropertyPathExpression (string[] pathSegments)

	Microsoft.OData.Edm.EdmExpressionKind ExpressionKind  { public virtual get; }
}

public class Microsoft.OData.Edm.Vocabularies.EdmPropertyValue : IEdmDelayedValue, IEdmPropertyValue {
	public EdmPropertyValue (string name)
	public EdmPropertyValue (string name, Microsoft.OData.Edm.Vocabularies.IEdmValue value)

	string Name  { public virtual get; }
	Microsoft.OData.Edm.Vocabularies.IEdmValue Value  { public virtual get; public set; }
}

public class Microsoft.OData.Edm.Vocabularies.EdmPropertyValueBinding : Microsoft.OData.Edm.EdmElement, IEdmElement, IEdmPropertyValueBinding {
	public EdmPropertyValueBinding (Microsoft.OData.Edm.IEdmProperty boundProperty, Microsoft.OData.Edm.IEdmExpression value)

	Microsoft.OData.Edm.IEdmProperty BoundProperty  { public virtual get; }
	Microsoft.OData.Edm.IEdmExpression Value  { public virtual get; }
}

public class Microsoft.OData.Edm.Vocabularies.EdmRecordExpression : Microsoft.OData.Edm.EdmElement, IEdmElement, IEdmExpression, IEdmRecordExpression {
	public EdmRecordExpression (Microsoft.OData.Edm.Vocabularies.IEdmPropertyConstructor[] properties)
	public EdmRecordExpression (System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Vocabularies.IEdmPropertyConstructor]] properties)
	public EdmRecordExpression (Microsoft.OData.Edm.IEdmStructuredTypeReference declaredType, Microsoft.OData.Edm.Vocabularies.IEdmPropertyConstructor[] properties)
	public EdmRecordExpression (Microsoft.OData.Edm.IEdmStructuredTypeReference declaredType, System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Vocabularies.IEdmPropertyConstructor]] properties)

	Microsoft.OData.Edm.IEdmStructuredTypeReference DeclaredType  { public virtual get; }
	Microsoft.OData.Edm.EdmExpressionKind ExpressionKind  { public virtual get; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Vocabularies.IEdmPropertyConstructor]] Properties  { public virtual get; }
}

public class Microsoft.OData.Edm.Vocabularies.EdmStringConstant : Microsoft.OData.Edm.Vocabularies.EdmValue, IEdmElement, IEdmExpression, IEdmDelayedValue, IEdmPrimitiveValue, IEdmStringConstantExpression, IEdmStringValue, IEdmValue {
	public EdmStringConstant (string value)
	public EdmStringConstant (Microsoft.OData.Edm.IEdmStringTypeReference type, string value)

	Microsoft.OData.Edm.EdmExpressionKind ExpressionKind  { public virtual get; }
	string Value  { public virtual get; }
	Microsoft.OData.Edm.Vocabularies.EdmValueKind ValueKind  { public virtual get; }
}

public class Microsoft.OData.Edm.Vocabularies.EdmStructuredValue : Microsoft.OData.Edm.Vocabularies.EdmValue, IEdmElement, IEdmDelayedValue, IEdmStructuredValue, IEdmValue {
	public EdmStructuredValue (Microsoft.OData.Edm.IEdmStructuredTypeReference type, System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Vocabularies.IEdmPropertyValue]] propertyValues)

	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Vocabularies.IEdmPropertyValue]] PropertyValues  { public virtual get; }
	Microsoft.OData.Edm.Vocabularies.EdmValueKind ValueKind  { public virtual get; }

	public virtual Microsoft.OData.Edm.Vocabularies.IEdmPropertyValue FindPropertyValue (string propertyName)
}

public class Microsoft.OData.Edm.Vocabularies.EdmTerm : Microsoft.OData.Edm.EdmNamedElement, IEdmElement, IEdmNamedElement, IEdmSchemaElement, IEdmTerm, IEdmVocabularyAnnotatable {
	public EdmTerm (string namespaceName, string name, Microsoft.OData.Edm.EdmPrimitiveTypeKind type)
	public EdmTerm (string namespaceName, string name, Microsoft.OData.Edm.IEdmTypeReference type)
	public EdmTerm (string namespaceName, string name, Microsoft.OData.Edm.EdmPrimitiveTypeKind type, string appliesTo)
	public EdmTerm (string namespaceName, string name, Microsoft.OData.Edm.IEdmTypeReference type, string appliesTo)
	public EdmTerm (string namespaceName, string name, Microsoft.OData.Edm.IEdmTypeReference type, string appliesTo, string defaultValue)

	string AppliesTo  { public virtual get; }
	string DefaultValue  { public virtual get; }
	string Namespace  { public virtual get; }
	Microsoft.OData.Edm.EdmSchemaElementKind SchemaElementKind  { public virtual get; }
	Microsoft.OData.Edm.IEdmTypeReference Type  { public virtual get; }
}

public class Microsoft.OData.Edm.Vocabularies.EdmTimeOfDayConstant : Microsoft.OData.Edm.Vocabularies.EdmValue, IEdmElement, IEdmExpression, IEdmDelayedValue, IEdmPrimitiveValue, IEdmTimeOfDayConstantExpression, IEdmTimeOfDayValue, IEdmValue {
	public EdmTimeOfDayConstant (Microsoft.OData.Edm.TimeOfDay value)
	public EdmTimeOfDayConstant (Microsoft.OData.Edm.IEdmTemporalTypeReference type, Microsoft.OData.Edm.TimeOfDay value)

	Microsoft.OData.Edm.EdmExpressionKind ExpressionKind  { public virtual get; }
	Microsoft.OData.Edm.TimeOfDay Value  { public virtual get; }
	Microsoft.OData.Edm.Vocabularies.EdmValueKind ValueKind  { public virtual get; }
}

public class Microsoft.OData.Edm.Vocabularies.EdmToClrConverter {
	public EdmToClrConverter ()
	public EdmToClrConverter (Microsoft.OData.Edm.Vocabularies.TryCreateObjectInstance tryCreateObjectInstanceDelegate)
	public EdmToClrConverter (Microsoft.OData.Edm.Vocabularies.TryCreateObjectInstance tryCreateObjectInstanceDelegate, Microsoft.OData.Edm.Vocabularies.TryGetClrPropertyInfo tryGetClrPropertyInfoDelegate, Microsoft.OData.Edm.Vocabularies.TryGetClrTypeName tryGetClrTypeNameDelegate)

	public T AsClrValue (Microsoft.OData.Edm.Vocabularies.IEdmValue edmValue)
	public object AsClrValue (Microsoft.OData.Edm.Vocabularies.IEdmValue edmValue, System.Type clrType)
	public void RegisterConvertedObject (Microsoft.OData.Edm.Vocabularies.IEdmStructuredValue edmValue, object clrObject)
}

public class Microsoft.OData.Edm.Vocabularies.EdmToClrEvaluator : Microsoft.OData.Edm.Vocabularies.EdmExpressionEvaluator {
	public EdmToClrEvaluator (System.Collections.Generic.IDictionary`2[[Microsoft.OData.Edm.IEdmOperation],[System.Func`2[[Microsoft.OData.Edm.Vocabularies.IEdmValue[]],[Microsoft.OData.Edm.Vocabularies.IEdmValue]]]] builtInFunctions)
	public EdmToClrEvaluator (System.Collections.Generic.IDictionary`2[[Microsoft.OData.Edm.IEdmOperation],[System.Func`2[[Microsoft.OData.Edm.Vocabularies.IEdmValue[]],[Microsoft.OData.Edm.Vocabularies.IEdmValue]]]] builtInFunctions, System.Func`3[[System.String],[Microsoft.OData.Edm.Vocabularies.IEdmValue[]],[Microsoft.OData.Edm.Vocabularies.IEdmValue]] lastChanceOperationApplier)
	public EdmToClrEvaluator (System.Collections.Generic.IDictionary`2[[Microsoft.OData.Edm.IEdmOperation],[System.Func`2[[Microsoft.OData.Edm.Vocabularies.IEdmValue[]],[Microsoft.OData.Edm.Vocabularies.IEdmValue]]]] builtInFunctions, System.Func`3[[System.String],[Microsoft.OData.Edm.Vocabularies.IEdmValue[]],[Microsoft.OData.Edm.Vocabularies.IEdmValue]] lastChanceOperationApplier, System.Func`5[[Microsoft.OData.Edm.IEdmModel],[Microsoft.OData.Edm.IEdmType],[System.String],[System.String],[Microsoft.OData.Edm.IEdmExpression]] getAnnotationExpressionForType, System.Func`6[[Microsoft.OData.Edm.IEdmModel],[Microsoft.OData.Edm.IEdmType],[System.String],[System.String],[System.String],[Microsoft.OData.Edm.IEdmExpression]] getAnnotationExpressionForProperty, Microsoft.OData.Edm.IEdmModel edmModel)

	Microsoft.OData.Edm.Vocabularies.EdmToClrConverter EdmToClrConverter  { public get; public set; }

	public T EvaluateToClrValue (Microsoft.OData.Edm.IEdmExpression expression)
	public T EvaluateToClrValue (Microsoft.OData.Edm.IEdmExpression expression, Microsoft.OData.Edm.Vocabularies.IEdmStructuredValue context)
	public T EvaluateToClrValue (Microsoft.OData.Edm.IEdmExpression expression, Microsoft.OData.Edm.Vocabularies.IEdmStructuredValue context, Microsoft.OData.Edm.IEdmTypeReference targetType)
}

public class Microsoft.OData.Edm.Vocabularies.EdmTypedDirectValueAnnotationBinding`1 : Microsoft.OData.Edm.EdmNamedElement, IEdmElement, IEdmNamedElement, IEdmDirectValueAnnotationBinding {
	public EdmTypedDirectValueAnnotationBinding`1 (Microsoft.OData.Edm.IEdmElement element, T value)

	Microsoft.OData.Edm.IEdmElement Element  { public virtual get; }
	string NamespaceUri  { public virtual get; }
	object Value  { public virtual get; }
}

public class Microsoft.OData.Edm.Vocabularies.EdmVocabularyAnnotation : Microsoft.OData.Edm.EdmElement, IEdmElement, IEdmVocabularyAnnotation {
	public EdmVocabularyAnnotation (Microsoft.OData.Edm.Vocabularies.IEdmVocabularyAnnotatable target, Microsoft.OData.Edm.Vocabularies.IEdmTerm term, Microsoft.OData.Edm.IEdmExpression value)
	public EdmVocabularyAnnotation (Microsoft.OData.Edm.Vocabularies.IEdmVocabularyAnnotatable target, Microsoft.OData.Edm.Vocabularies.IEdmTerm term, string qualifier, Microsoft.OData.Edm.IEdmExpression value)

	string Qualifier  { public virtual get; }
	Microsoft.OData.Edm.Vocabularies.IEdmVocabularyAnnotatable Target  { public virtual get; }
	Microsoft.OData.Edm.Vocabularies.IEdmTerm Term  { public virtual get; }
	Microsoft.OData.Edm.IEdmExpression Value  { public virtual get; }
}

public sealed class Microsoft.OData.Edm.Vocabularies.TryCreateObjectInstance : System.MulticastDelegate, ICloneable, ISerializable {
	public TryCreateObjectInstance (object object, System.IntPtr method)

	public virtual System.IAsyncResult BeginInvoke (Microsoft.OData.Edm.Vocabularies.IEdmStructuredValue edmValue, System.Type clrType, Microsoft.OData.Edm.Vocabularies.EdmToClrConverter converter, out System.Object& objectInstance, out System.Boolean& objectInstanceInitialized, System.AsyncCallback callback, object object)
	public virtual bool EndInvoke (out System.Object& objectInstance, out System.Boolean& objectInstanceInitialized, System.IAsyncResult result)
	public virtual bool Invoke (Microsoft.OData.Edm.Vocabularies.IEdmStructuredValue edmValue, System.Type clrType, Microsoft.OData.Edm.Vocabularies.EdmToClrConverter converter, out System.Object& objectInstance, out System.Boolean& objectInstanceInitialized)
}

public sealed class Microsoft.OData.Edm.Vocabularies.TryGetClrPropertyInfo : System.MulticastDelegate, ICloneable, ISerializable {
	public TryGetClrPropertyInfo (object object, System.IntPtr method)

	public virtual System.IAsyncResult BeginInvoke (System.Type clrType, string edmName, out System.Reflection.PropertyInfo& propertyInfo, System.AsyncCallback callback, object object)
	public virtual bool EndInvoke (out System.Reflection.PropertyInfo& propertyInfo, System.IAsyncResult result)
	public virtual bool Invoke (System.Type clrType, string edmName, out System.Reflection.PropertyInfo& propertyInfo)
}

public sealed class Microsoft.OData.Edm.Vocabularies.TryGetClrTypeName : System.MulticastDelegate, ICloneable, ISerializable {
	public TryGetClrTypeName (object object, System.IntPtr method)

	public virtual System.IAsyncResult BeginInvoke (Microsoft.OData.Edm.IEdmModel edmModel, string edmTypeName, out System.String& clrTypeName, System.AsyncCallback callback, object object)
	public virtual bool EndInvoke (out System.String& clrTypeName, System.IAsyncResult result)
	public virtual bool Invoke (Microsoft.OData.Edm.IEdmModel edmModel, string edmTypeName, out System.String& clrTypeName)
}

public sealed class Microsoft.OData.Edm.Vocabularies.V1.CapabilitiesVocabularyConstants {
	public static string ChangeTracking = "Org.OData.Capabilities.V1.ChangeTracking"
	public static string ChangeTrackingExpandableProperties = "ExpandableProperties"
	public static string ChangeTrackingFilterableProperties = "FilterableProperties"
	public static string ChangeTrackingSupported = "Supported"
}

public sealed class Microsoft.OData.Edm.Vocabularies.V1.CoreVocabularyConstants {
	public static string AcceptableMediaTypes = "Org.OData.Core.V1.AcceptableMediaTypes"
	public static string Computed = "Org.OData.Core.V1.Computed"
	public static string ConventionalIDs = "Org.OData.Core.V1.ConventionalIDs"
	public static string DereferenceableIDs = "Org.OData.Core.V1.DereferenceableIDs"
	public static string Description = "Org.OData.Core.V1.Description"
	public static string Immutable = "Org.OData.Core.V1.Immutable"
	public static string IsLanguageDependent = "Org.OData.Core.V1.IsLanguageDependent"
	public static string IsMediaType = "Org.OData.Core.V1.IsMediaType"
	public static string IsURL = "Org.OData.Core.V1.IsURL"
	public static string LongDescription = "Org.OData.Core.V1.LongDescription"
	public static string MediaType = "Org.OData.Core.V1.MediaType"
	public static string OptimisticConcurrency = "Org.OData.Core.V1.OptimisticConcurrency"
	public static string OptionalParameter = "Org.OData.Core.V1.OptionalParameter"
	public static string Permissions = "Org.OData.Core.V1.Permissions"
	public static string RequiresType = "Org.OData.Core.V1.RequiresType"
	public static string ResourcePath = "Org.OData.Core.V1.ResourcePath"
}

public sealed class Microsoft.OData.Edm.Vocabularies.V1.CoreVocabularyModel {
	public static readonly Microsoft.OData.Edm.Vocabularies.IEdmTerm AcceptableMediaTypesTerm = Microsoft.OData.Edm.Csdl.CsdlSemantics.CsdlSemanticsTerm
	public static readonly Microsoft.OData.Edm.Vocabularies.IEdmTerm ComputedTerm = Microsoft.OData.Edm.Csdl.CsdlSemantics.CsdlSemanticsTerm
	public static readonly Microsoft.OData.Edm.Vocabularies.IEdmTerm ConcurrencyTerm = Microsoft.OData.Edm.Csdl.CsdlSemantics.CsdlSemanticsTerm
	public static readonly Microsoft.OData.Edm.Vocabularies.IEdmTerm ConventionalIDsTerm = Microsoft.OData.Edm.Csdl.CsdlSemantics.CsdlSemanticsTerm
	public static readonly Microsoft.OData.Edm.Vocabularies.IEdmTerm DereferenceableIDsTerm = Microsoft.OData.Edm.Csdl.CsdlSemantics.CsdlSemanticsTerm
	public static readonly Microsoft.OData.Edm.Vocabularies.IEdmTerm DescriptionTerm = Microsoft.OData.Edm.Csdl.CsdlSemantics.CsdlSemanticsTerm
	public static readonly Microsoft.OData.Edm.Vocabularies.IEdmTerm ImmutableTerm = Microsoft.OData.Edm.Csdl.CsdlSemantics.CsdlSemanticsTerm
	public static readonly Microsoft.OData.Edm.IEdmModel Instance = Microsoft.OData.Edm.Csdl.CsdlSemantics.CsdlSemanticsModel
	public static readonly Microsoft.OData.Edm.Vocabularies.IEdmTerm IsLanguageDependentTerm = Microsoft.OData.Edm.Csdl.CsdlSemantics.CsdlSemanticsTerm
	public static readonly Microsoft.OData.Edm.Vocabularies.IEdmTerm IsMediaTypeTerm = Microsoft.OData.Edm.Csdl.CsdlSemantics.CsdlSemanticsTerm
	public static readonly Microsoft.OData.Edm.Vocabularies.IEdmTerm IsURLTerm = Microsoft.OData.Edm.Csdl.CsdlSemantics.CsdlSemanticsTerm
	public static readonly Microsoft.OData.Edm.Vocabularies.IEdmTerm LongDescriptionTerm = Microsoft.OData.Edm.Csdl.CsdlSemantics.CsdlSemanticsTerm
	public static readonly Microsoft.OData.Edm.Vocabularies.IEdmTerm MediaTypeTerm = Microsoft.OData.Edm.Csdl.CsdlSemantics.CsdlSemanticsTerm
	public static readonly Microsoft.OData.Edm.Vocabularies.IEdmTerm OptionalParameterTerm = Microsoft.OData.Edm.Csdl.CsdlSemantics.CsdlSemanticsTerm
	public static readonly Microsoft.OData.Edm.Vocabularies.IEdmTerm PermissionsTerm = Microsoft.OData.Edm.Csdl.CsdlSemantics.CsdlSemanticsTerm
	public static readonly Microsoft.OData.Edm.Vocabularies.IEdmTerm RequiresTypeTerm = Microsoft.OData.Edm.Csdl.CsdlSemantics.CsdlSemanticsTerm
	public static readonly Microsoft.OData.Edm.Vocabularies.IEdmTerm ResourcePathTerm = Microsoft.OData.Edm.Csdl.CsdlSemantics.CsdlSemanticsTerm
}

public sealed class Microsoft.OData.Edm.Vocabularies.Community.V1.AlternateKeysVocabularyConstants {
	public static string AlternateKeys = "OData.Community.Keys.V1.AlternateKeys"
}

public sealed class Microsoft.OData.Edm.Vocabularies.Community.V1.AlternateKeysVocabularyModel {
	public static readonly Microsoft.OData.Edm.Vocabularies.IEdmTerm AlternateKeysTerm = Microsoft.OData.Edm.Csdl.CsdlSemantics.CsdlSemanticsTerm
	public static readonly Microsoft.OData.Edm.IEdmModel Instance = Microsoft.OData.Edm.Csdl.CsdlSemantics.CsdlSemanticsModel
}

public enum Microsoft.OData.BatchPayloadUriOption : int {
	AbsoluteUri = 0
	AbsoluteUriUsingHostHeader = 1
	RelativeUri = 2
}

public enum Microsoft.OData.DeltaDeletedEntryReason : int {
	Changed = 1
	Deleted = 0
}

public enum Microsoft.OData.ODataBatchReaderState : int {
	ChangesetEnd = 3
	ChangesetStart = 2
	Completed = 4
	Exception = 5
	Initial = 0
	Operation = 1
}

protected enum Microsoft.OData.ODataBatchWriter+BatchWriterState : int {
	BatchCompleted = 7
	BatchStarted = 1
	ChangesetCompleted = 6
	ChangesetStarted = 2
	Error = 8
	OperationCreated = 3
	OperationStreamDisposed = 5
	OperationStreamRequested = 4
	Start = 0
}

public enum Microsoft.OData.ODataCollectionReaderState : int {
	CollectionEnd = 3
	CollectionStart = 1
	Completed = 5
	Exception = 4
	Start = 0
	Value = 2
}

public enum Microsoft.OData.ODataDeltaReaderState : int {
	Completed = 9
	DeltaDeletedEntry = 5
	DeltaDeletedLink = 7
	DeltaLink = 6
	DeltaResourceEnd = 4
	DeltaResourceSetEnd = 2
	DeltaResourceSetStart = 1
	DeltaResourceStart = 3
	Exception = 8
	NestedResource = 10
	Start = 0
}

public enum Microsoft.OData.ODataNullValueBehaviorKind : int {
	Default = 0
	DisableValidation = 2
	IgnoreValue = 1
}

public enum Microsoft.OData.ODataParameterReaderState : int {
	Collection = 2
	Completed = 4
	Exception = 3
	Resource = 5
	ResourceSet = 6
	Start = 0
	Value = 1
}

public enum Microsoft.OData.ODataPayloadKind : int {
	Asynchronous = 15
	Batch = 11
	BinaryValue = 6
	Collection = 7
	Delta = 14
	EntityReferenceLink = 3
	EntityReferenceLinks = 4
	Error = 10
	IndividualProperty = 13
	MetadataDocument = 9
	Parameter = 12
	Property = 2
	Resource = 1
	ResourceSet = 0
	ServiceDocument = 8
	Unsupported = 2147483647
	Value = 5
}

public enum Microsoft.OData.ODataPropertyKind : int {
	ETag = 2
	Key = 1
	Open = 3
	Unspecified = 0
}

public enum Microsoft.OData.ODataReaderState : int {
	Completed = 9
	DeletedResourceEnd = 14
	DeletedResourceStart = 13
	DeltaDeletedLink = 16
	DeltaLink = 15
	DeltaResourceSetEnd = 12
	DeltaResourceSetStart = 11
	EntityReferenceLink = 7
	Exception = 8
	NestedResourceInfoEnd = 6
	NestedResourceInfoStart = 5
	Primitive = 10
	ResourceEnd = 4
	ResourceSetEnd = 2
	ResourceSetStart = 1
	ResourceStart = 3
	Start = 0
}

public enum Microsoft.OData.ODataVersion : int {
	V4 = 0
	V401 = 1
}

public enum Microsoft.OData.ServiceLifetime : int {
	Scoped = 1
	Singleton = 0
	Transient = 2
}

[
FlagsAttribute(),
]
public enum Microsoft.OData.ValidationKinds : int {
	All = -1
	None = 0
	ThrowIfTypeConflictsWithMetadata = 4
	ThrowOnDuplicatePropertyNames = 1
	ThrowOnUndeclaredPropertyForNonOpenType = 2
}

public interface Microsoft.OData.IContainerBuilder {
	Microsoft.OData.IContainerBuilder AddService (Microsoft.OData.ServiceLifetime lifetime, System.Type serviceType, System.Func`2[[System.IServiceProvider],[System.Object]] implementationFactory)
	Microsoft.OData.IContainerBuilder AddService (Microsoft.OData.ServiceLifetime lifetime, System.Type serviceType, System.Type implementationType)
	System.IServiceProvider BuildContainer ()
}

public interface Microsoft.OData.IContainerProvider {
	System.IServiceProvider Container  { public abstract get; }
}

public interface Microsoft.OData.IODataPayloadUriConverter {
	System.Uri ConvertPayloadUri (System.Uri baseUri, System.Uri payloadUri)
}

public interface Microsoft.OData.IODataRequestMessage {
	System.Collections.Generic.IEnumerable`1[[System.Collections.Generic.KeyValuePair`2[[System.String],[System.String]]]] Headers  { public abstract get; }
	string Method  { public abstract get; public abstract set; }
	System.Uri Url  { public abstract get; public abstract set; }

	string GetHeader (string headerName)
	System.IO.Stream GetStream ()
	void SetHeader (string headerName, string headerValue)
}

public interface Microsoft.OData.IODataRequestMessageAsync : IODataRequestMessage {
	System.Threading.Tasks.Task`1[[System.IO.Stream]] GetStreamAsync ()
}

public interface Microsoft.OData.IODataResponseMessage {
	System.Collections.Generic.IEnumerable`1[[System.Collections.Generic.KeyValuePair`2[[System.String],[System.String]]]] Headers  { public abstract get; }
	int StatusCode  { public abstract get; public abstract set; }

	string GetHeader (string headerName)
	System.IO.Stream GetStream ()
	void SetHeader (string headerName, string headerValue)
}

public interface Microsoft.OData.IODataResponseMessageAsync : IODataResponseMessage {
	System.Threading.Tasks.Task`1[[System.IO.Stream]] GetStreamAsync ()
}

public abstract class Microsoft.OData.ODataAnnotatable {
	protected ODataAnnotatable ()

	Microsoft.OData.ODataTypeAnnotation TypeAnnotation  { public get; public set; }
}

public abstract class Microsoft.OData.ODataBatchReader : IODataBatchOperationListener {
	protected ODataBatchReader (Microsoft.OData.ODataInputContext inputContext, bool synchronous)

	string CurrentGroupId  { public get; }
	Microsoft.OData.ODataInputContext InputContext  { protected get; }
	Microsoft.OData.ODataBatchReaderState State  { public get; }

	protected Microsoft.OData.ODataBatchOperationRequestMessage BuildOperationRequestMessage (System.Func`1[[System.IO.Stream]] streamCreatorFunc, string method, System.Uri requestUri, Microsoft.OData.ODataBatchOperationHeaders headers, string contentId, string groupId, System.Collections.Generic.IEnumerable`1[[System.String]] dependsOnRequestIds, bool dependsOnIdsValidationRequired)
	protected Microsoft.OData.ODataBatchOperationResponseMessage BuildOperationResponseMessage (System.Func`1[[System.IO.Stream]] streamCreatorFunc, int statusCode, Microsoft.OData.ODataBatchOperationHeaders headers, string contentId, string groupId)
	public Microsoft.OData.ODataBatchOperationRequestMessage CreateOperationRequestMessage ()
	public System.Threading.Tasks.Task`1[[Microsoft.OData.ODataBatchOperationRequestMessage]] CreateOperationRequestMessageAsync ()
	protected abstract Microsoft.OData.ODataBatchOperationRequestMessage CreateOperationRequestMessageImplementation ()
	public Microsoft.OData.ODataBatchOperationResponseMessage CreateOperationResponseMessage ()
	public System.Threading.Tasks.Task`1[[Microsoft.OData.ODataBatchOperationResponseMessage]] CreateOperationResponseMessageAsync ()
	protected abstract Microsoft.OData.ODataBatchOperationResponseMessage CreateOperationResponseMessageImplementation ()
	protected virtual string GetCurrentGroupIdImplementation ()
	void Microsoft.OData.IODataBatchOperationListener.BatchOperationContentStreamDisposed ()
	void Microsoft.OData.IODataBatchOperationListener.BatchOperationContentStreamRequested ()
	System.Threading.Tasks.Task Microsoft.OData.IODataBatchOperationListener.BatchOperationContentStreamRequestedAsync ()
	public bool Read ()
	public System.Threading.Tasks.Task`1[[System.Boolean]] ReadAsync ()
	protected abstract Microsoft.OData.ODataBatchReaderState ReadAtChangesetEndImplementation ()
	protected abstract Microsoft.OData.ODataBatchReaderState ReadAtChangesetStartImplementation ()
	protected abstract Microsoft.OData.ODataBatchReaderState ReadAtOperationImplementation ()
	protected abstract Microsoft.OData.ODataBatchReaderState ReadAtStartImplementation ()
	protected void ThrowODataException (string errorMessage)
}

public abstract class Microsoft.OData.ODataBatchWriter : IODataBatchOperationListener, IODataOutputInStreamErrorListener {
	Microsoft.OData.ODataBatchOperationRequestMessage CurrentOperationRequestMessage  { protected get; protected set; }
	Microsoft.OData.ODataBatchOperationResponseMessage CurrentOperationResponseMessage  { protected get; protected set; }
	Microsoft.OData.ODataOutputContext OutputContext  { protected get; }

	public abstract void BatchOperationContentStreamDisposed ()
	public abstract void BatchOperationContentStreamRequested ()
	public abstract System.Threading.Tasks.Task BatchOperationContentStreamRequestedAsync ()
	protected Microsoft.OData.ODataBatchOperationRequestMessage BuildOperationRequestMessage (System.IO.Stream outputStream, string method, System.Uri uri, string contentId, string groupId, System.Collections.Generic.IEnumerable`1[[System.String]] dependsOnIds)
	protected Microsoft.OData.ODataBatchOperationResponseMessage BuildOperationResponseMessage (System.IO.Stream outputStream, string contentId, string groupId)
	public Microsoft.OData.ODataBatchOperationRequestMessage CreateOperationRequestMessage (string method, System.Uri uri, string contentId)
	public Microsoft.OData.ODataBatchOperationRequestMessage CreateOperationRequestMessage (string method, System.Uri uri, string contentId, Microsoft.OData.BatchPayloadUriOption payloadUriOption)
	public Microsoft.OData.ODataBatchOperationRequestMessage CreateOperationRequestMessage (string method, System.Uri uri, string contentId, Microsoft.OData.BatchPayloadUriOption payloadUriOption, System.Collections.Generic.IEnumerable`1[[System.String]] dependsOnIds)
	public System.Threading.Tasks.Task`1[[Microsoft.OData.ODataBatchOperationRequestMessage]] CreateOperationRequestMessageAsync (string method, System.Uri uri, string contentId)
	public System.Threading.Tasks.Task`1[[Microsoft.OData.ODataBatchOperationRequestMessage]] CreateOperationRequestMessageAsync (string method, System.Uri uri, string contentId, Microsoft.OData.BatchPayloadUriOption payloadUriOption)
	public System.Threading.Tasks.Task`1[[Microsoft.OData.ODataBatchOperationRequestMessage]] CreateOperationRequestMessageAsync (string method, System.Uri uri, string contentId, Microsoft.OData.BatchPayloadUriOption payloadUriOption, System.Collections.Generic.IList`1[[System.String]] dependsOnIds)
	protected abstract Microsoft.OData.ODataBatchOperationRequestMessage CreateOperationRequestMessageImplementation (string method, System.Uri uri, string contentId, Microsoft.OData.BatchPayloadUriOption payloadUriOption, System.Collections.Generic.IEnumerable`1[[System.String]] dependsOnIds)
	public Microsoft.OData.ODataBatchOperationResponseMessage CreateOperationResponseMessage (string contentId)
	public System.Threading.Tasks.Task`1[[Microsoft.OData.ODataBatchOperationResponseMessage]] CreateOperationResponseMessageAsync (string contentId)
	protected abstract Microsoft.OData.ODataBatchOperationResponseMessage CreateOperationResponseMessageImplementation (string contentId)
	public void Flush ()
	public System.Threading.Tasks.Task FlushAsync ()
	protected abstract System.Threading.Tasks.Task FlushAsynchronously ()
	protected abstract void FlushSynchronously ()
	protected abstract System.Collections.Generic.IEnumerable`1[[System.String]] GetDependsOnRequestIds (System.Collections.Generic.IEnumerable`1[[System.String]] dependsOnIds)
	public abstract void OnInStreamError ()
	protected void SetState (Microsoft.OData.ODataBatchWriter+BatchWriterState newState)
	protected abstract void VerifyNotDisposed ()
	public void WriteEndBatch ()
	public System.Threading.Tasks.Task WriteEndBatchAsync ()
	protected abstract void WriteEndBatchImplementation ()
	public void WriteEndChangeset ()
	public System.Threading.Tasks.Task WriteEndChangesetAsync ()
	protected abstract void WriteEndChangesetImplementation ()
	public void WriteStartBatch ()
	public System.Threading.Tasks.Task WriteStartBatchAsync ()
	protected abstract void WriteStartBatchImplementation ()
	public void WriteStartChangeset ()
	public void WriteStartChangeset (string changesetId)
	public System.Threading.Tasks.Task WriteStartChangesetAsync ()
	public System.Threading.Tasks.Task WriteStartChangesetAsync (string changesetId)
	protected abstract void WriteStartChangesetImplementation (string groupOrChangesetId)
}

public abstract class Microsoft.OData.ODataCollectionReader {
	protected ODataCollectionReader ()

	object Item  { public abstract get; }
	Microsoft.OData.ODataCollectionReaderState State  { public abstract get; }

	public abstract bool Read ()
	public abstract System.Threading.Tasks.Task`1[[System.Boolean]] ReadAsync ()
}

public abstract class Microsoft.OData.ODataCollectionWriter {
	protected ODataCollectionWriter ()

	public abstract void Flush ()
	public abstract System.Threading.Tasks.Task FlushAsync ()
	public abstract void WriteEnd ()
	public abstract System.Threading.Tasks.Task WriteEndAsync ()
	public abstract void WriteItem (object item)
	public abstract System.Threading.Tasks.Task WriteItemAsync (object item)
	public abstract void WriteStart (Microsoft.OData.ODataCollectionStart collectionStart)
	public abstract System.Threading.Tasks.Task WriteStartAsync (Microsoft.OData.ODataCollectionStart collectionStart)
}

public abstract class Microsoft.OData.ODataDeltaLinkBase : Microsoft.OData.ODataItem {
	protected ODataDeltaLinkBase (System.Uri source, System.Uri target, string relationship)

	string Relationship  { public get; public set; }
	System.Uri Source  { public get; public set; }
	System.Uri Target  { public get; public set; }
}

public abstract class Microsoft.OData.ODataDeltaReader {
	protected ODataDeltaReader ()

	Microsoft.OData.ODataItem Item  { public abstract get; }
	Microsoft.OData.ODataDeltaReaderState State  { public abstract get; }
	Microsoft.OData.ODataReaderState SubState  { public abstract get; }

	public abstract bool Read ()
	public abstract System.Threading.Tasks.Task`1[[System.Boolean]] ReadAsync ()
}

public abstract class Microsoft.OData.ODataDeltaWriter {
	protected ODataDeltaWriter ()

	public abstract void Flush ()
	public abstract System.Threading.Tasks.Task FlushAsync ()
	public abstract void WriteDeltaDeletedEntry (Microsoft.OData.ODataDeltaDeletedEntry deltaDeletedEntry)
	public abstract System.Threading.Tasks.Task WriteDeltaDeletedEntryAsync (Microsoft.OData.ODataDeltaDeletedEntry deltaDeletedEntry)
	public abstract void WriteDeltaDeletedLink (Microsoft.OData.ODataDeltaDeletedLink deltaDeletedLink)
	public abstract System.Threading.Tasks.Task WriteDeltaDeletedLinkAsync (Microsoft.OData.ODataDeltaDeletedLink deltaDeletedLink)
	public abstract void WriteDeltaLink (Microsoft.OData.ODataDeltaLink deltaLink)
	public abstract System.Threading.Tasks.Task WriteDeltaLinkAsync (Microsoft.OData.ODataDeltaLink deltaLink)
	public abstract void WriteEnd ()
	public abstract System.Threading.Tasks.Task WriteEndAsync ()
	public abstract void WriteStart (Microsoft.OData.ODataDeltaResourceSet deltaResourceSet)
	public abstract void WriteStart (Microsoft.OData.ODataNestedResourceInfo nestedResourceInfo)
	public abstract void WriteStart (Microsoft.OData.ODataResource deltaResource)
	public abstract void WriteStart (Microsoft.OData.ODataResourceSet expandedResourceSet)
	public abstract System.Threading.Tasks.Task WriteStartAsync (Microsoft.OData.ODataDeltaResourceSet deltaResourceSet)
	public abstract System.Threading.Tasks.Task WriteStartAsync (Microsoft.OData.ODataNestedResourceInfo nestedResourceInfo)
	public abstract System.Threading.Tasks.Task WriteStartAsync (Microsoft.OData.ODataResource deltaResource)
	public abstract System.Threading.Tasks.Task WriteStartAsync (Microsoft.OData.ODataResourceSet expandedResourceSet)
}

public abstract class Microsoft.OData.ODataFormat {
	protected ODataFormat ()

	Microsoft.OData.ODataFormat Batch  { public static get; }
	Microsoft.OData.ODataFormat Json  { public static get; }
	Microsoft.OData.ODataFormat Metadata  { public static get; }
	Microsoft.OData.ODataFormat RawValue  { public static get; }

	public abstract Microsoft.OData.ODataInputContext CreateInputContext (Microsoft.OData.ODataMessageInfo messageInfo, Microsoft.OData.ODataMessageReaderSettings messageReaderSettings)
	public abstract System.Threading.Tasks.Task`1[[Microsoft.OData.ODataInputContext]] CreateInputContextAsync (Microsoft.OData.ODataMessageInfo messageInfo, Microsoft.OData.ODataMessageReaderSettings messageReaderSettings)
	public abstract Microsoft.OData.ODataOutputContext CreateOutputContext (Microsoft.OData.ODataMessageInfo messageInfo, Microsoft.OData.ODataMessageWriterSettings messageWriterSettings)
	public abstract System.Threading.Tasks.Task`1[[Microsoft.OData.ODataOutputContext]] CreateOutputContextAsync (Microsoft.OData.ODataMessageInfo messageInfo, Microsoft.OData.ODataMessageWriterSettings messageWriterSettings)
	public abstract System.Collections.Generic.IEnumerable`1[[Microsoft.OData.ODataPayloadKind]] DetectPayloadKind (Microsoft.OData.ODataMessageInfo messageInfo, Microsoft.OData.ODataMessageReaderSettings settings)
	public abstract System.Threading.Tasks.Task`1[[System.Collections.Generic.IEnumerable`1[[Microsoft.OData.ODataPayloadKind]]]] DetectPayloadKindAsync (Microsoft.OData.ODataMessageInfo messageInfo, Microsoft.OData.ODataMessageReaderSettings settings)
	internal virtual string GetContentType (Microsoft.OData.ODataMediaType mediaType, System.Text.Encoding encoding, bool writingResponse, out System.Collections.Generic.IEnumerable`1[[System.Collections.Generic.KeyValuePair`2[[System.String],[System.String]]]]& mediaTypeParameters)
}

public abstract class Microsoft.OData.ODataInputContext : IDisposable {
	protected ODataInputContext (Microsoft.OData.ODataFormat format, Microsoft.OData.ODataMessageInfo messageInfo, Microsoft.OData.ODataMessageReaderSettings messageReaderSettings)

	Microsoft.OData.ODataMessageReaderSettings MessageReaderSettings  { public get; }
	Microsoft.OData.Edm.IEdmModel Model  { public get; }
	Microsoft.OData.IODataPayloadUriConverter PayloadUriConverter  { public get; }
	bool ReadingResponse  { public get; }
	bool Synchronous  { public get; }

	internal virtual Microsoft.OData.ODataAsynchronousReader CreateAsynchronousReader ()
	internal virtual System.Threading.Tasks.Task`1[[Microsoft.OData.ODataAsynchronousReader]] CreateAsynchronousReaderAsync ()
	internal virtual Microsoft.OData.ODataBatchReader CreateBatchReader ()
	internal virtual System.Threading.Tasks.Task`1[[Microsoft.OData.ODataBatchReader]] CreateBatchReaderAsync ()
	public virtual Microsoft.OData.ODataCollectionReader CreateCollectionReader (Microsoft.OData.Edm.IEdmTypeReference expectedItemTypeReference)
	public virtual System.Threading.Tasks.Task`1[[Microsoft.OData.ODataCollectionReader]] CreateCollectionReaderAsync (Microsoft.OData.Edm.IEdmTypeReference expectedItemTypeReference)
	internal virtual Microsoft.OData.ODataDeltaReader CreateDeltaReader (Microsoft.OData.Edm.IEdmEntitySetBase entitySet, Microsoft.OData.Edm.IEdmEntityType expectedBaseEntityType)
	internal virtual System.Threading.Tasks.Task`1[[Microsoft.OData.ODataDeltaReader]] CreateDeltaReaderAsync (Microsoft.OData.Edm.IEdmEntitySetBase entitySet, Microsoft.OData.Edm.IEdmEntityType expectedBaseEntityType)
	public virtual Microsoft.OData.ODataReader CreateDeltaResourceSetReader (Microsoft.OData.Edm.IEdmEntitySetBase entitySet, Microsoft.OData.Edm.IEdmStructuredType expectedResourceType)
	public virtual System.Threading.Tasks.Task`1[[Microsoft.OData.ODataReader]] CreateDeltaResourceSetReaderAsync (Microsoft.OData.Edm.IEdmEntitySetBase entitySet, Microsoft.OData.Edm.IEdmStructuredType expectedResourceType)
	public virtual Microsoft.OData.ODataParameterReader CreateParameterReader (Microsoft.OData.Edm.IEdmOperation operation)
	public virtual System.Threading.Tasks.Task`1[[Microsoft.OData.ODataParameterReader]] CreateParameterReaderAsync (Microsoft.OData.Edm.IEdmOperation operation)
	public virtual Microsoft.OData.ODataReader CreateResourceReader (Microsoft.OData.Edm.IEdmNavigationSource navigationSource, Microsoft.OData.Edm.IEdmStructuredType expectedResourceType)
	public virtual System.Threading.Tasks.Task`1[[Microsoft.OData.ODataReader]] CreateResourceReaderAsync (Microsoft.OData.Edm.IEdmNavigationSource navigationSource, Microsoft.OData.Edm.IEdmStructuredType expectedResourceType)
	public virtual Microsoft.OData.ODataReader CreateResourceSetReader (Microsoft.OData.Edm.IEdmEntitySetBase entitySet, Microsoft.OData.Edm.IEdmStructuredType expectedResourceType)
	public virtual System.Threading.Tasks.Task`1[[Microsoft.OData.ODataReader]] CreateResourceSetReaderAsync (Microsoft.OData.Edm.IEdmEntitySetBase entitySet, Microsoft.OData.Edm.IEdmStructuredType expectedResourceType)
	public virtual Microsoft.OData.ODataReader CreateUriParameterResourceReader (Microsoft.OData.Edm.IEdmNavigationSource navigationSource, Microsoft.OData.Edm.IEdmStructuredType expectedResourceType)
	public virtual System.Threading.Tasks.Task`1[[Microsoft.OData.ODataReader]] CreateUriParameterResourceReaderAsync (Microsoft.OData.Edm.IEdmNavigationSource navigationSource, Microsoft.OData.Edm.IEdmStructuredType expectedResourceType)
	public virtual Microsoft.OData.ODataReader CreateUriParameterResourceSetReader (Microsoft.OData.Edm.IEdmEntitySetBase entitySet, Microsoft.OData.Edm.IEdmStructuredType expectedResourceType)
	public virtual System.Threading.Tasks.Task`1[[Microsoft.OData.ODataReader]] CreateUriParameterResourceSetReaderAsync (Microsoft.OData.Edm.IEdmEntitySetBase entitySet, Microsoft.OData.Edm.IEdmStructuredType expectedResourceType)
	public virtual void Dispose ()
	protected virtual void Dispose (bool disposing)
	internal virtual Microsoft.OData.ODataEntityReferenceLink ReadEntityReferenceLink ()
	internal virtual System.Threading.Tasks.Task`1[[Microsoft.OData.ODataEntityReferenceLink]] ReadEntityReferenceLinkAsync ()
	internal virtual Microsoft.OData.ODataEntityReferenceLinks ReadEntityReferenceLinks ()
	internal virtual System.Threading.Tasks.Task`1[[Microsoft.OData.ODataEntityReferenceLinks]] ReadEntityReferenceLinksAsync ()
	public virtual Microsoft.OData.ODataError ReadError ()
	public virtual System.Threading.Tasks.Task`1[[Microsoft.OData.ODataError]] ReadErrorAsync ()
	internal virtual Microsoft.OData.Edm.IEdmModel ReadMetadataDocument (System.Func`2[[System.Uri],[System.Xml.XmlReader]] getReferencedModelReaderFunc)
	public virtual Microsoft.OData.ODataProperty ReadProperty (Microsoft.OData.Edm.IEdmStructuralProperty edmStructuralProperty, Microsoft.OData.Edm.IEdmTypeReference expectedPropertyTypeReference)
	public virtual System.Threading.Tasks.Task`1[[Microsoft.OData.ODataProperty]] ReadPropertyAsync (Microsoft.OData.Edm.IEdmStructuralProperty edmStructuralProperty, Microsoft.OData.Edm.IEdmTypeReference expectedPropertyTypeReference)
	internal virtual Microsoft.OData.ODataServiceDocument ReadServiceDocument ()
	internal virtual System.Threading.Tasks.Task`1[[Microsoft.OData.ODataServiceDocument]] ReadServiceDocumentAsync ()
	internal virtual object ReadValue (Microsoft.OData.Edm.IEdmPrimitiveTypeReference expectedPrimitiveTypeReference)
	internal virtual System.Threading.Tasks.Task`1[[System.Object]] ReadValueAsync (Microsoft.OData.Edm.IEdmPrimitiveTypeReference expectedPrimitiveTypeReference)
}

public abstract class Microsoft.OData.ODataItem : Microsoft.OData.ODataAnnotatable {
	protected ODataItem ()
}

public abstract class Microsoft.OData.ODataOperation : Microsoft.OData.ODataAnnotatable {
	protected ODataOperation ()

	System.Uri Metadata  { public get; public set; }
	System.Uri Target  { public get; public set; }
	string Title  { public get; public set; }
}

public abstract class Microsoft.OData.ODataOutputContext : IDisposable {
	protected ODataOutputContext (Microsoft.OData.ODataFormat format, Microsoft.OData.ODataMessageInfo messageInfo, Microsoft.OData.ODataMessageWriterSettings messageWriterSettings)

	Microsoft.OData.ODataMessageWriterSettings MessageWriterSettings  { public get; }
	Microsoft.OData.Edm.IEdmModel Model  { public get; }
	Microsoft.OData.IODataPayloadUriConverter PayloadUriConverter  { public get; }
	bool Synchronous  { public get; }
	bool WritingResponse  { public get; }

	internal virtual Microsoft.OData.ODataAsynchronousWriter CreateODataAsynchronousWriter ()
	internal virtual System.Threading.Tasks.Task`1[[Microsoft.OData.ODataAsynchronousWriter]] CreateODataAsynchronousWriterAsync ()
	internal virtual Microsoft.OData.ODataBatchWriter CreateODataBatchWriter ()
	internal virtual System.Threading.Tasks.Task`1[[Microsoft.OData.ODataBatchWriter]] CreateODataBatchWriterAsync ()
	public virtual Microsoft.OData.ODataCollectionWriter CreateODataCollectionWriter (Microsoft.OData.Edm.IEdmTypeReference itemTypeReference)
	public virtual System.Threading.Tasks.Task`1[[Microsoft.OData.ODataCollectionWriter]] CreateODataCollectionWriterAsync (Microsoft.OData.Edm.IEdmTypeReference itemTypeReference)
	public virtual Microsoft.OData.ODataWriter CreateODataDeltaResourceSetWriter (Microsoft.OData.Edm.IEdmEntitySetBase entitySet, Microsoft.OData.Edm.IEdmStructuredType resourceType)
	public virtual System.Threading.Tasks.Task`1[[Microsoft.OData.ODataWriter]] CreateODataDeltaResourceSetWriterAsync (Microsoft.OData.Edm.IEdmEntitySetBase entitySet, Microsoft.OData.Edm.IEdmStructuredType entityType)
	internal virtual Microsoft.OData.ODataDeltaWriter CreateODataDeltaWriter (Microsoft.OData.Edm.IEdmEntitySetBase entitySet, Microsoft.OData.Edm.IEdmEntityType entityType)
	internal virtual System.Threading.Tasks.Task`1[[Microsoft.OData.ODataDeltaWriter]] CreateODataDeltaWriterAsync (Microsoft.OData.Edm.IEdmEntitySetBase entitySet, Microsoft.OData.Edm.IEdmEntityType entityType)
	public virtual Microsoft.OData.ODataParameterWriter CreateODataParameterWriter (Microsoft.OData.Edm.IEdmOperation operation)
	public virtual System.Threading.Tasks.Task`1[[Microsoft.OData.ODataParameterWriter]] CreateODataParameterWriterAsync (Microsoft.OData.Edm.IEdmOperation operation)
	public virtual Microsoft.OData.ODataWriter CreateODataResourceSetWriter (Microsoft.OData.Edm.IEdmEntitySetBase entitySet, Microsoft.OData.Edm.IEdmStructuredType resourceType)
	public virtual System.Threading.Tasks.Task`1[[Microsoft.OData.ODataWriter]] CreateODataResourceSetWriterAsync (Microsoft.OData.Edm.IEdmEntitySetBase entitySet, Microsoft.OData.Edm.IEdmStructuredType entityType)
	public virtual Microsoft.OData.ODataWriter CreateODataResourceWriter (Microsoft.OData.Edm.IEdmNavigationSource navigationSource, Microsoft.OData.Edm.IEdmStructuredType resourceType)
	public virtual System.Threading.Tasks.Task`1[[Microsoft.OData.ODataWriter]] CreateODataResourceWriterAsync (Microsoft.OData.Edm.IEdmNavigationSource navigationSource, Microsoft.OData.Edm.IEdmStructuredType resourceType)
	public virtual Microsoft.OData.ODataWriter CreateODataUriParameterResourceSetWriter (Microsoft.OData.Edm.IEdmEntitySetBase entitySet, Microsoft.OData.Edm.IEdmStructuredType resourceType)
	public virtual System.Threading.Tasks.Task`1[[Microsoft.OData.ODataWriter]] CreateODataUriParameterResourceSetWriterAsync (Microsoft.OData.Edm.IEdmEntitySetBase entitySet, Microsoft.OData.Edm.IEdmStructuredType resourceType)
	public virtual Microsoft.OData.ODataWriter CreateODataUriParameterResourceWriter (Microsoft.OData.Edm.IEdmNavigationSource navigationSource, Microsoft.OData.Edm.IEdmStructuredType resourceType)
	public virtual System.Threading.Tasks.Task`1[[Microsoft.OData.ODataWriter]] CreateODataUriParameterResourceWriterAsync (Microsoft.OData.Edm.IEdmNavigationSource navigationSource, Microsoft.OData.Edm.IEdmStructuredType resourceType)
	public virtual void Dispose ()
	protected virtual void Dispose (bool disposing)
	internal virtual void WriteEntityReferenceLink (Microsoft.OData.ODataEntityReferenceLink link)
	internal virtual System.Threading.Tasks.Task WriteEntityReferenceLinkAsync (Microsoft.OData.ODataEntityReferenceLink link)
	internal virtual void WriteEntityReferenceLinks (Microsoft.OData.ODataEntityReferenceLinks links)
	internal virtual System.Threading.Tasks.Task WriteEntityReferenceLinksAsync (Microsoft.OData.ODataEntityReferenceLinks links)
	public virtual void WriteError (Microsoft.OData.ODataError odataError, bool includeDebugInformation)
	public virtual System.Threading.Tasks.Task WriteErrorAsync (Microsoft.OData.ODataError odataError, bool includeDebugInformation)
	internal virtual void WriteInStreamError (Microsoft.OData.ODataError error, bool includeDebugInformation)
	internal virtual System.Threading.Tasks.Task WriteInStreamErrorAsync (Microsoft.OData.ODataError error, bool includeDebugInformation)
	internal virtual void WriteMetadataDocument ()
	public virtual void WriteProperty (Microsoft.OData.ODataProperty odataProperty)
	public virtual System.Threading.Tasks.Task WritePropertyAsync (Microsoft.OData.ODataProperty odataProperty)
	internal virtual void WriteServiceDocument (Microsoft.OData.ODataServiceDocument serviceDocument)
	internal virtual System.Threading.Tasks.Task WriteServiceDocumentAsync (Microsoft.OData.ODataServiceDocument serviceDocument)
	internal virtual void WriteValue (object value)
	internal virtual System.Threading.Tasks.Task WriteValueAsync (object value)
}

public abstract class Microsoft.OData.ODataParameterReader {
	protected ODataParameterReader ()

	string Name  { public abstract get; }
	Microsoft.OData.ODataParameterReaderState State  { public abstract get; }
	object Value  { public abstract get; }

	public abstract Microsoft.OData.ODataCollectionReader CreateCollectionReader ()
	public abstract Microsoft.OData.ODataReader CreateResourceReader ()
	public abstract Microsoft.OData.ODataReader CreateResourceSetReader ()
	public abstract bool Read ()
	public abstract System.Threading.Tasks.Task`1[[System.Boolean]] ReadAsync ()
}

public abstract class Microsoft.OData.ODataParameterWriter {
	protected ODataParameterWriter ()

	public abstract Microsoft.OData.ODataCollectionWriter CreateCollectionWriter (string parameterName)
	public abstract System.Threading.Tasks.Task`1[[Microsoft.OData.ODataCollectionWriter]] CreateCollectionWriterAsync (string parameterName)
	public abstract Microsoft.OData.ODataWriter CreateResourceSetWriter (string parameterName)
	public abstract System.Threading.Tasks.Task`1[[Microsoft.OData.ODataWriter]] CreateResourceSetWriterAsync (string parameterName)
	public abstract Microsoft.OData.ODataWriter CreateResourceWriter (string parameterName)
	public abstract System.Threading.Tasks.Task`1[[Microsoft.OData.ODataWriter]] CreateResourceWriterAsync (string parameterName)
	public abstract void Flush ()
	public abstract System.Threading.Tasks.Task FlushAsync ()
	public abstract void WriteEnd ()
	public abstract System.Threading.Tasks.Task WriteEndAsync ()
	public abstract void WriteStart ()
	public abstract System.Threading.Tasks.Task WriteStartAsync ()
	public abstract void WriteValue (string parameterName, object parameterValue)
	public abstract System.Threading.Tasks.Task WriteValueAsync (string parameterName, object parameterValue)
}

public abstract class Microsoft.OData.ODataReader {
	protected ODataReader ()

	Microsoft.OData.ODataItem Item  { public abstract get; }
	Microsoft.OData.ODataReaderState State  { public abstract get; }

	public abstract bool Read ()
	public abstract System.Threading.Tasks.Task`1[[System.Boolean]] ReadAsync ()
}

[
DebuggerDisplayAttribute(),
]
public abstract class Microsoft.OData.ODataResourceBase : Microsoft.OData.ODataItem {
	protected ODataResourceBase ()

	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.ODataAction]] Actions  { public get; }
	System.Uri EditLink  { public get; public set; }
	string ETag  { public get; public set; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.ODataFunction]] Functions  { public get; }
	System.Uri Id  { public get; public set; }
	System.Collections.Generic.ICollection`1[[Microsoft.OData.ODataInstanceAnnotation]] InstanceAnnotations  { public get; public set; }
	bool IsTransient  { public get; public set; }
	Microsoft.OData.ODataStreamReferenceValue MediaResource  { public get; public set; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.ODataProperty]] Properties  { public get; public set; }
	System.Uri ReadLink  { public get; public set; }
	string TypeName  { public get; public set; }

	public void AddAction (Microsoft.OData.ODataAction action)
	public void AddFunction (Microsoft.OData.ODataFunction function)
}

public abstract class Microsoft.OData.ODataResourceSetBase : Microsoft.OData.ODataItem {
	protected ODataResourceSetBase ()

	System.Nullable`1[[System.Int64]] Count  { public get; public set; }
	System.Uri DeltaLink  { public get; public set; }
	System.Uri Id  { public get; public set; }
	System.Collections.Generic.ICollection`1[[Microsoft.OData.ODataInstanceAnnotation]] InstanceAnnotations  { public get; public set; }
	System.Uri NextPageLink  { public get; public set; }
	string TypeName  { public get; public set; }
}

public abstract class Microsoft.OData.ODataServiceDocumentElement : Microsoft.OData.ODataAnnotatable {
	protected ODataServiceDocumentElement ()

	string Name  { public get; public set; }
	string Title  { public get; public set; }
	System.Uri Url  { public get; public set; }
}

public abstract class Microsoft.OData.ODataValue : Microsoft.OData.ODataItem {
	protected ODataValue ()
}

public abstract class Microsoft.OData.ODataWriter {
	protected ODataWriter ()

	public abstract void Flush ()
	public abstract System.Threading.Tasks.Task FlushAsync ()
	public Microsoft.OData.ODataWriter Write (Microsoft.OData.ODataDeletedResource deletedResource)
	public Microsoft.OData.ODataWriter Write (Microsoft.OData.ODataDeltaDeletedLink deltaDeletedLink)
	public Microsoft.OData.ODataWriter Write (Microsoft.OData.ODataDeltaLink deltaLink)
	public Microsoft.OData.ODataWriter Write (Microsoft.OData.ODataDeltaResourceSet deltaResourceSet)
	public Microsoft.OData.ODataWriter Write (Microsoft.OData.ODataNestedResourceInfo nestedResourceInfo)
	public Microsoft.OData.ODataWriter Write (Microsoft.OData.ODataPrimitiveValue primitiveValue)
	public Microsoft.OData.ODataWriter Write (Microsoft.OData.ODataResource resource)
	public Microsoft.OData.ODataWriter Write (Microsoft.OData.ODataResourceSet resourceSet)
	public Microsoft.OData.ODataWriter Write (Microsoft.OData.ODataDeletedResource deletedResource, System.Action nestedAction)
	public Microsoft.OData.ODataWriter Write (Microsoft.OData.ODataDeltaResourceSet deltaResourceSet, System.Action nestedAction)
	public Microsoft.OData.ODataWriter Write (Microsoft.OData.ODataNestedResourceInfo nestedResourceInfo, System.Action nestedAction)
	public Microsoft.OData.ODataWriter Write (Microsoft.OData.ODataResource resource, System.Action nestedAction)
	public Microsoft.OData.ODataWriter Write (Microsoft.OData.ODataResourceSet resourceSet, System.Action nestedAction)
	public virtual void WriteDeltaDeletedLink (Microsoft.OData.ODataDeltaDeletedLink deltaDeletedLink)
	public virtual System.Threading.Tasks.Task WriteDeltaDeletedLinkAsync (Microsoft.OData.ODataDeltaDeletedLink deltaDeletedLink)
	public virtual void WriteDeltaLink (Microsoft.OData.ODataDeltaLink deltaLink)
	public virtual System.Threading.Tasks.Task WriteDeltaLinkAsync (Microsoft.OData.ODataDeltaLink deltaLink)
	public abstract void WriteEnd ()
	public abstract System.Threading.Tasks.Task WriteEndAsync ()
	public abstract void WriteEntityReferenceLink (Microsoft.OData.ODataEntityReferenceLink entityReferenceLink)
	public abstract System.Threading.Tasks.Task WriteEntityReferenceLinkAsync (Microsoft.OData.ODataEntityReferenceLink entityReferenceLink)
	public virtual void WritePrimitive (Microsoft.OData.ODataPrimitiveValue primitiveValue)
	public virtual System.Threading.Tasks.Task WritePrimitiveAsync (Microsoft.OData.ODataPrimitiveValue primitiveValue)
	public virtual void WriteStart (Microsoft.OData.ODataDeletedResource deletedResource)
	public virtual void WriteStart (Microsoft.OData.ODataDeltaResourceSet deltaResourceSet)
	public abstract void WriteStart (Microsoft.OData.ODataNestedResourceInfo nestedResourceInfo)
	public abstract void WriteStart (Microsoft.OData.ODataResource resource)
	public abstract void WriteStart (Microsoft.OData.ODataResourceSet resourceSet)
	public virtual System.Threading.Tasks.Task WriteStartAsync (Microsoft.OData.ODataDeletedResource deletedResource)
	public virtual System.Threading.Tasks.Task WriteStartAsync (Microsoft.OData.ODataDeltaResourceSet deltaResourceSet)
	public abstract System.Threading.Tasks.Task WriteStartAsync (Microsoft.OData.ODataNestedResourceInfo nestedResourceInfo)
	public abstract System.Threading.Tasks.Task WriteStartAsync (Microsoft.OData.ODataResource resource)
	public abstract System.Threading.Tasks.Task WriteStartAsync (Microsoft.OData.ODataResourceSet resourceSet)
}

[
ExtensionAttribute(),
]
public sealed class Microsoft.OData.ContainerBuilderExtensions {
	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.IContainerBuilder AddDefaultODataServices (Microsoft.OData.IContainerBuilder builder)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.IContainerBuilder AddService (Microsoft.OData.IContainerBuilder builder, Microsoft.OData.ServiceLifetime lifetime)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.IContainerBuilder AddService (Microsoft.OData.IContainerBuilder builder, Microsoft.OData.ServiceLifetime lifetime)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.IContainerBuilder AddService (Microsoft.OData.IContainerBuilder builder, Microsoft.OData.ServiceLifetime lifetime, Func`2 implementationFactory)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.IContainerBuilder AddService (Microsoft.OData.IContainerBuilder builder, Microsoft.OData.ServiceLifetime lifetime, System.Type serviceType)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.IContainerBuilder AddServicePrototype (Microsoft.OData.IContainerBuilder builder, TService instance)
}

public sealed class Microsoft.OData.ODataConstants {
	public static string ContentIdHeader = "Content-ID"
	public static string ContentTypeHeader = "Content-Type"
	public static string MethodDelete = "DELETE"
	public static string MethodGet = "GET"
	public static string MethodPatch = "PATCH"
	public static string MethodPost = "POST"
	public static string MethodPut = "PUT"
	public static string ODataVersionHeader = "OData-Version"
}

[
ExtensionAttribute(),
]
public sealed class Microsoft.OData.ODataMessageExtensions {
	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.ODataVersion GetODataVersion (Microsoft.OData.IODataRequestMessage message, Microsoft.OData.ODataVersion defaultVersion)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.ODataVersion GetODataVersion (Microsoft.OData.IODataResponseMessage message, Microsoft.OData.ODataVersion defaultVersion)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.ODataPreferenceHeader PreferenceAppliedHeader (Microsoft.OData.IODataResponseMessage responseMessage)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.ODataPreferenceHeader PreferHeader (Microsoft.OData.IODataRequestMessage requestMessage)
}

[
ExtensionAttribute(),
]
public sealed class Microsoft.OData.ODataObjectModelExtensions {
	[
	ExtensionAttribute(),
	]
	public static void SetSerializationInfo (Microsoft.OData.ODataCollectionStart collectionStart, Microsoft.OData.ODataCollectionStartSerializationInfo serializationInfo)

	[
	ExtensionAttribute(),
	]
	public static void SetSerializationInfo (Microsoft.OData.ODataDeltaDeletedEntry deltaDeletedEntry, Microsoft.OData.ODataDeltaSerializationInfo serializationInfo)

	[
	ExtensionAttribute(),
	]
	public static void SetSerializationInfo (Microsoft.OData.ODataDeltaDeletedEntry deltaDeletedEntry, Microsoft.OData.ODataResourceSerializationInfo serializationInfo)

	[
	ExtensionAttribute(),
	]
	public static void SetSerializationInfo (Microsoft.OData.ODataDeltaLinkBase deltalink, Microsoft.OData.ODataDeltaSerializationInfo serializationInfo)

	[
	ExtensionAttribute(),
	]
	public static void SetSerializationInfo (Microsoft.OData.ODataDeltaResourceSet deltaResourceSet, Microsoft.OData.ODataDeltaResourceSetSerializationInfo serializationInfo)

	[
	ExtensionAttribute(),
	]
	public static void SetSerializationInfo (Microsoft.OData.ODataDeltaResourceSet deltaResourceSet, Microsoft.OData.ODataResourceSerializationInfo serializationInfo)

	[
	ExtensionAttribute(),
	]
	public static void SetSerializationInfo (Microsoft.OData.ODataNestedResourceInfo nestedResourceInfo, Microsoft.OData.ODataNestedResourceInfoSerializationInfo serializationInfo)

	[
	ExtensionAttribute(),
	]
	public static void SetSerializationInfo (Microsoft.OData.ODataProperty property, Microsoft.OData.ODataPropertySerializationInfo serializationInfo)

	[
	ExtensionAttribute(),
	]
	public static void SetSerializationInfo (Microsoft.OData.ODataResource resource, Microsoft.OData.ODataResourceSerializationInfo serializationInfo)

	[
	ExtensionAttribute(),
	]
	public static void SetSerializationInfo (Microsoft.OData.ODataResourceBase resource, Microsoft.OData.ODataResourceSerializationInfo serializationInfo)

	[
	ExtensionAttribute(),
	]
	public static void SetSerializationInfo (Microsoft.OData.ODataResourceSet resourceSet, Microsoft.OData.ODataResourceSerializationInfo serializationInfo)
}

[
ExtensionAttribute(),
]
public sealed class Microsoft.OData.ODataUriExtensions {
	[
	ExtensionAttribute(),
	]
	public static System.Uri BuildUri (Microsoft.OData.ODataUri odataUri, Microsoft.OData.ODataUrlKeyDelimiter urlKeyDelimiter)
}

public sealed class Microsoft.OData.ODataUriUtils {
	public static object ConvertFromUriLiteral (string value, Microsoft.OData.ODataVersion version)
	public static object ConvertFromUriLiteral (string value, Microsoft.OData.ODataVersion version, Microsoft.OData.Edm.IEdmModel model, Microsoft.OData.Edm.IEdmTypeReference typeReference)
	public static string ConvertToUriLiteral (object value, Microsoft.OData.ODataVersion version)
	public static string ConvertToUriLiteral (object value, Microsoft.OData.ODataVersion version, Microsoft.OData.Edm.IEdmModel model)
}

[
ExtensionAttribute(),
]
public sealed class Microsoft.OData.ODataUtils {
	public static string AppendDefaultHeaderValue (string headerName, string headerValue)
	public static string AppendDefaultHeaderValue (string headerName, string headerValue, Microsoft.OData.ODataVersion version)
	public static System.Func`2[[System.String],[System.Boolean]] CreateAnnotationFilter (string annotationFilter)
	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.ODataServiceDocument GenerateServiceDocument (Microsoft.OData.Edm.IEdmModel model)

	public static Microsoft.OData.ODataFormat GetReadFormat (Microsoft.OData.ODataMessageReader messageReader)
	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.ODataNullValueBehaviorKind NullValueReadBehaviorKind (Microsoft.OData.Edm.IEdmModel model, Microsoft.OData.Edm.IEdmProperty property)

	public static string ODataVersionToString (Microsoft.OData.ODataVersion version)
	public static Microsoft.OData.ODataFormat SetHeadersForPayload (Microsoft.OData.ODataMessageWriter messageWriter, Microsoft.OData.ODataPayloadKind payloadKind)
	[
	ExtensionAttribute(),
	]
	public static void SetNullValueReaderBehavior (Microsoft.OData.Edm.IEdmModel model, Microsoft.OData.Edm.IEdmProperty property, Microsoft.OData.ODataNullValueBehaviorKind nullValueReadBehaviorKind)

	public static Microsoft.OData.ODataVersion StringToODataVersion (string version)
}

[
DebuggerDisplayAttribute(),
]
public class Microsoft.OData.ODataContentTypeException : Microsoft.OData.ODataException, _Exception, ISerializable {
	public ODataContentTypeException ()
	public ODataContentTypeException (string message)
	public ODataContentTypeException (string message, System.Exception innerException)
}

[
DebuggerDisplayAttribute(),
]
public class Microsoft.OData.ODataException : System.InvalidOperationException, _Exception, ISerializable {
	public ODataException ()
	public ODataException (string message)
	public ODataException (string message, System.Exception innerException)
}

public class Microsoft.OData.ODataMediaTypeResolver {
	public ODataMediaTypeResolver ()

	public virtual System.Collections.Generic.IEnumerable`1[[Microsoft.OData.ODataMediaTypeFormat]] GetMediaTypeFormats (Microsoft.OData.ODataPayloadKind payloadKind)
}

public class Microsoft.OData.ODataPayloadValueConverter {
	public ODataPayloadValueConverter ()

	public virtual object ConvertFromPayloadValue (object value, Microsoft.OData.Edm.IEdmTypeReference edmTypeReference)
	public virtual object ConvertToPayloadValue (object value, Microsoft.OData.Edm.IEdmTypeReference edmTypeReference)
}

public class Microsoft.OData.ODataPreferenceHeader {
	string AnnotationFilter  { public get; public set; }
	bool ContinueOnError  { public get; public set; }
	System.Nullable`1[[System.Int32]] MaxPageSize  { public get; public set; }
	bool RespondAsync  { public get; public set; }
	System.Nullable`1[[System.Boolean]] ReturnContent  { public get; public set; }
	bool TrackChanges  { public get; public set; }
	System.Nullable`1[[System.Int32]] Wait  { public get; public set; }

	protected void Clear (string preference)
	protected Microsoft.OData.HttpHeaderValueElement Get (string preferenceName)
	protected void Set (Microsoft.OData.HttpHeaderValueElement preference)
}

public sealed class Microsoft.OData.HttpHeaderValueElement {
	public HttpHeaderValueElement (string name, string value, System.Collections.Generic.IEnumerable`1[[System.Collections.Generic.KeyValuePair`2[[System.String],[System.String]]]] parameters)

	string Name  { public get; }
	System.Collections.Generic.IEnumerable`1[[System.Collections.Generic.KeyValuePair`2[[System.String],[System.String]]]] Parameters  { public get; }
	string Value  { public get; }

	public virtual string ToString ()
}

[
DebuggerDisplayAttribute(),
]
public sealed class Microsoft.OData.ODataAction : Microsoft.OData.ODataOperation {
	public ODataAction ()
}

public sealed class Microsoft.OData.ODataAsynchronousReader {
	public Microsoft.OData.ODataAsynchronousResponseMessage CreateResponseMessage ()
	public System.Threading.Tasks.Task`1[[Microsoft.OData.ODataAsynchronousResponseMessage]] CreateResponseMessageAsync ()
}

public sealed class Microsoft.OData.ODataAsynchronousResponseMessage : IContainerProvider, IODataResponseMessage, IODataResponseMessageAsync {
	System.IServiceProvider Container  { public virtual get; }
	System.Collections.Generic.IEnumerable`1[[System.Collections.Generic.KeyValuePair`2[[System.String],[System.String]]]] Headers  { public virtual get; }
	int StatusCode  { public virtual get; public virtual set; }

	public virtual string GetHeader (string headerName)
	public virtual System.IO.Stream GetStream ()
	public virtual System.Threading.Tasks.Task`1[[System.IO.Stream]] GetStreamAsync ()
	public virtual void SetHeader (string headerName, string headerValue)
}

public sealed class Microsoft.OData.ODataAsynchronousWriter : IODataOutputInStreamErrorListener {
	public Microsoft.OData.ODataAsynchronousResponseMessage CreateResponseMessage ()
	public System.Threading.Tasks.Task`1[[Microsoft.OData.ODataAsynchronousResponseMessage]] CreateResponseMessageAsync ()
	public void Flush ()
	public System.Threading.Tasks.Task FlushAsync ()
}

[
DefaultMemberAttribute(),
]
public sealed class Microsoft.OData.ODataBatchOperationHeaders : IEnumerable, IEnumerable`1 {
	public ODataBatchOperationHeaders ()

	string Item [string key] { public get; public set; }

	public void Add (string key, string value)
	public bool ContainsKeyOrdinal (string key)
	public virtual System.Collections.Generic.IEnumerator`1[[System.Collections.Generic.KeyValuePair`2[[System.String],[System.String]]]] GetEnumerator ()
	public bool Remove (string key)
	public bool TryGetValue (string key, out System.String& value)
}

public sealed class Microsoft.OData.ODataBatchOperationRequestMessage : IContainerProvider, IODataPayloadUriConverter, IODataRequestMessage, IODataRequestMessageAsync {
	public const readonly string ContentId = 

	System.IServiceProvider Container  { public virtual get; }
	System.Collections.Generic.IEnumerable`1[[System.String]] DependsOnIds  { public get; }
	string GroupId  { public get; }
	System.Collections.Generic.IEnumerable`1[[System.Collections.Generic.KeyValuePair`2[[System.String],[System.String]]]] Headers  { public virtual get; }
	string Method  { public virtual get; public virtual set; }
	System.Uri Url  { public virtual get; public virtual set; }

	public virtual string GetHeader (string headerName)
	public virtual System.IO.Stream GetStream ()
	public virtual System.Threading.Tasks.Task`1[[System.IO.Stream]] GetStreamAsync ()
	public virtual void SetHeader (string headerName, string headerValue)
}

public sealed class Microsoft.OData.ODataBatchOperationResponseMessage : IContainerProvider, IODataPayloadUriConverter, IODataResponseMessage, IODataResponseMessageAsync {
	public const readonly string ContentId = 

	System.IServiceProvider Container  { public virtual get; }
	string GroupId  { public get; }
	System.Collections.Generic.IEnumerable`1[[System.Collections.Generic.KeyValuePair`2[[System.String],[System.String]]]] Headers  { public virtual get; }
	int StatusCode  { public virtual get; public virtual set; }

	public virtual string GetHeader (string headerName)
	public virtual System.IO.Stream GetStream ()
	public virtual System.Threading.Tasks.Task`1[[System.IO.Stream]] GetStreamAsync ()
	public virtual void SetHeader (string headerName, string headerValue)
}

public sealed class Microsoft.OData.ODataCollectionStart : Microsoft.OData.ODataAnnotatable {
	public ODataCollectionStart ()

	System.Nullable`1[[System.Int64]] Count  { public get; public set; }
	string Name  { public get; public set; }
	System.Uri NextPageLink  { public get; public set; }
}

public sealed class Microsoft.OData.ODataCollectionStartSerializationInfo {
	public ODataCollectionStartSerializationInfo ()

	string CollectionTypeName  { public get; public set; }
}

public sealed class Microsoft.OData.ODataCollectionValue : Microsoft.OData.ODataValue {
	public ODataCollectionValue ()

	System.Collections.Generic.IEnumerable`1[[System.Object]] Items  { public get; public set; }
	string TypeName  { public get; public set; }
}

public sealed class Microsoft.OData.ODataDeletedResource : Microsoft.OData.ODataResourceBase {
	public ODataDeletedResource ()
	public ODataDeletedResource (System.Uri id, Microsoft.OData.DeltaDeletedEntryReason reason)

	System.Nullable`1[[Microsoft.OData.DeltaDeletedEntryReason]] Reason  { public get; public set; }
}

public sealed class Microsoft.OData.ODataDeltaDeletedEntry : Microsoft.OData.ODataItem {
	public ODataDeltaDeletedEntry (string id, Microsoft.OData.DeltaDeletedEntryReason reason)

	string Id  { public get; public set; }
	System.Nullable`1[[Microsoft.OData.DeltaDeletedEntryReason]] Reason  { public get; public set; }
}

public sealed class Microsoft.OData.ODataDeltaDeletedLink : Microsoft.OData.ODataDeltaLinkBase {
	public ODataDeltaDeletedLink (System.Uri source, System.Uri target, string relationship)
}

public sealed class Microsoft.OData.ODataDeltaLink : Microsoft.OData.ODataDeltaLinkBase {
	public ODataDeltaLink (System.Uri source, System.Uri target, string relationship)
}

public sealed class Microsoft.OData.ODataDeltaResourceSet : Microsoft.OData.ODataResourceSetBase {
	public ODataDeltaResourceSet ()
}

public sealed class Microsoft.OData.ODataDeltaResourceSetSerializationInfo {
	public ODataDeltaResourceSetSerializationInfo ()

	string EntitySetName  { public get; public set; }
	string EntityTypeName  { public get; public set; }
	string ExpectedTypeName  { public get; public set; }
}

public sealed class Microsoft.OData.ODataDeltaSerializationInfo {
	public ODataDeltaSerializationInfo ()

	string NavigationSourceName  { public get; public set; }
}

public sealed class Microsoft.OData.ODataEdmPropertyAnnotation {
	public ODataEdmPropertyAnnotation ()

	Microsoft.OData.ODataNullValueBehaviorKind NullValueReadBehaviorKind  { public get; public set; }
}

[
DebuggerDisplayAttribute(),
]
public sealed class Microsoft.OData.ODataEntityReferenceLink : Microsoft.OData.ODataItem {
	public ODataEntityReferenceLink ()

	System.Collections.Generic.ICollection`1[[Microsoft.OData.ODataInstanceAnnotation]] InstanceAnnotations  { public get; public set; }
	System.Uri Url  { public get; public set; }
}

public sealed class Microsoft.OData.ODataEntityReferenceLinks : Microsoft.OData.ODataAnnotatable {
	public ODataEntityReferenceLinks ()

	System.Nullable`1[[System.Int64]] Count  { public get; public set; }
	System.Collections.Generic.ICollection`1[[Microsoft.OData.ODataInstanceAnnotation]] InstanceAnnotations  { public get; public set; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.ODataEntityReferenceLink]] Links  { public get; public set; }
	System.Uri NextPageLink  { public get; public set; }
}

public sealed class Microsoft.OData.ODataEntitySetInfo : Microsoft.OData.ODataServiceDocumentElement {
	public ODataEntitySetInfo ()
}

public sealed class Microsoft.OData.ODataEnumValue : Microsoft.OData.ODataValue {
	public ODataEnumValue (string value)
	public ODataEnumValue (string value, string typeName)

	string TypeName  { public get; }
	string Value  { public get; }
}

[
DebuggerDisplayAttribute(),
]
public sealed class Microsoft.OData.ODataError : Microsoft.OData.ODataAnnotatable {
	public ODataError ()

	System.Collections.Generic.ICollection`1[[Microsoft.OData.ODataErrorDetail]] Details  { public get; public set; }
	string ErrorCode  { public get; public set; }
	Microsoft.OData.ODataInnerError InnerError  { public get; public set; }
	System.Collections.Generic.ICollection`1[[Microsoft.OData.ODataInstanceAnnotation]] InstanceAnnotations  { public get; public set; }
	string Message  { public get; public set; }
	string Target  { public get; public set; }

	public virtual string ToString ()
}

public sealed class Microsoft.OData.ODataErrorDetail {
	public ODataErrorDetail ()

	string ErrorCode  { public get; public set; }
	string Message  { public get; public set; }
	string Target  { public get; public set; }
}

[
DebuggerDisplayAttribute(),
]
public sealed class Microsoft.OData.ODataErrorException : Microsoft.OData.ODataException, _Exception, ISerializable {
	public ODataErrorException ()
	public ODataErrorException (Microsoft.OData.ODataError error)
	public ODataErrorException (string message)
	public ODataErrorException (string message, Microsoft.OData.ODataError error)
	public ODataErrorException (string message, System.Exception innerException)
	public ODataErrorException (string message, System.Exception innerException, Microsoft.OData.ODataError error)

	Microsoft.OData.ODataError Error  { public get; }
}

[
DebuggerDisplayAttribute(),
]
public sealed class Microsoft.OData.ODataFunction : Microsoft.OData.ODataOperation {
	public ODataFunction ()
}

public sealed class Microsoft.OData.ODataFunctionImportInfo : Microsoft.OData.ODataServiceDocumentElement {
	public ODataFunctionImportInfo ()
}

[
DebuggerDisplayAttribute(),
]
public sealed class Microsoft.OData.ODataInnerError {
	public ODataInnerError ()
	public ODataInnerError (System.Exception exception)

	Microsoft.OData.ODataInnerError InnerError  { public get; public set; }
	string Message  { public get; public set; }
	string StackTrace  { public get; public set; }
	string TypeName  { public get; public set; }
}

public sealed class Microsoft.OData.ODataInstanceAnnotation : Microsoft.OData.ODataAnnotatable {
	public ODataInstanceAnnotation (string name, Microsoft.OData.ODataValue value)

	string Name  { public get; }
	Microsoft.OData.ODataValue Value  { public get; }
}

[
DebuggerDisplayAttribute(),
]
public sealed class Microsoft.OData.ODataMediaType {
	public ODataMediaType (string type, string subType)
	public ODataMediaType (string type, string subType, System.Collections.Generic.IEnumerable`1[[System.Collections.Generic.KeyValuePair`2[[System.String],[System.String]]]] parameters)

	System.Collections.Generic.IEnumerable`1[[System.Collections.Generic.KeyValuePair`2[[System.String],[System.String]]]] Parameters  { public get; }
	string SubType  { public get; }
	string Type  { public get; }
}

public sealed class Microsoft.OData.ODataMediaTypeFormat {
	public ODataMediaTypeFormat (Microsoft.OData.ODataMediaType mediaType, Microsoft.OData.ODataFormat format)

	Microsoft.OData.ODataFormat Format  { public get; }
	Microsoft.OData.ODataMediaType MediaType  { public get; }
}

public sealed class Microsoft.OData.ODataMessageInfo {
	public ODataMessageInfo ()

	System.IServiceProvider Container  { public get; public set; }
	System.Text.Encoding Encoding  { public get; public set; }
	bool IsAsync  { public get; public set; }
	bool IsResponse  { public get; public set; }
	Microsoft.OData.ODataMediaType MediaType  { public get; public set; }
	System.IO.Stream MessageStream  { public get; public set; }
	Microsoft.OData.Edm.IEdmModel Model  { public get; public set; }
	Microsoft.OData.IODataPayloadUriConverter PayloadUriConverter  { public get; public set; }
}

public sealed class Microsoft.OData.ODataMessageQuotas {
	public ODataMessageQuotas ()
	public ODataMessageQuotas (Microsoft.OData.ODataMessageQuotas other)

	int MaxNestingDepth  { public get; public set; }
	int MaxOperationsPerChangeset  { public get; public set; }
	int MaxPartsPerBatch  { public get; public set; }
	long MaxReceivedMessageSize  { public get; public set; }
}

public sealed class Microsoft.OData.ODataMessageReader : IDisposable {
	public ODataMessageReader (Microsoft.OData.IODataRequestMessage requestMessage)
	public ODataMessageReader (Microsoft.OData.IODataResponseMessage responseMessage)
	public ODataMessageReader (Microsoft.OData.IODataRequestMessage requestMessage, Microsoft.OData.ODataMessageReaderSettings settings)
	public ODataMessageReader (Microsoft.OData.IODataResponseMessage responseMessage, Microsoft.OData.ODataMessageReaderSettings settings)
	public ODataMessageReader (Microsoft.OData.IODataRequestMessage requestMessage, Microsoft.OData.ODataMessageReaderSettings settings, Microsoft.OData.Edm.IEdmModel model)
	public ODataMessageReader (Microsoft.OData.IODataResponseMessage responseMessage, Microsoft.OData.ODataMessageReaderSettings settings, Microsoft.OData.Edm.IEdmModel model)

	public Microsoft.OData.ODataAsynchronousReader CreateODataAsynchronousReader ()
	public System.Threading.Tasks.Task`1[[Microsoft.OData.ODataAsynchronousReader]] CreateODataAsynchronousReaderAsync ()
	public Microsoft.OData.ODataBatchReader CreateODataBatchReader ()
	public System.Threading.Tasks.Task`1[[Microsoft.OData.ODataBatchReader]] CreateODataBatchReaderAsync ()
	public Microsoft.OData.ODataCollectionReader CreateODataCollectionReader ()
	public Microsoft.OData.ODataCollectionReader CreateODataCollectionReader (Microsoft.OData.Edm.IEdmTypeReference expectedItemTypeReference)
	public System.Threading.Tasks.Task`1[[Microsoft.OData.ODataCollectionReader]] CreateODataCollectionReaderAsync ()
	public System.Threading.Tasks.Task`1[[Microsoft.OData.ODataCollectionReader]] CreateODataCollectionReaderAsync (Microsoft.OData.Edm.IEdmTypeReference expectedItemTypeReference)
	[
	ObsoleteAttribute(),
	]
	public Microsoft.OData.ODataDeltaReader CreateODataDeltaReader (Microsoft.OData.Edm.IEdmEntitySetBase entitySet, Microsoft.OData.Edm.IEdmEntityType expectedBaseEntityType)

	[
	ObsoleteAttribute(),
	]
	public System.Threading.Tasks.Task`1[[Microsoft.OData.ODataDeltaReader]] CreateODataDeltaReaderAsync (Microsoft.OData.Edm.IEdmEntitySetBase entitySet, Microsoft.OData.Edm.IEdmEntityType expectedBaseEntityType)

	public Microsoft.OData.ODataReader CreateODataDeltaResourceSetReader ()
	public Microsoft.OData.ODataReader CreateODataDeltaResourceSetReader (Microsoft.OData.Edm.IEdmStructuredType expectedResourceType)
	public Microsoft.OData.ODataReader CreateODataDeltaResourceSetReader (Microsoft.OData.Edm.IEdmEntitySetBase entitySet, Microsoft.OData.Edm.IEdmStructuredType expectedResourceType)
	public System.Threading.Tasks.Task`1[[Microsoft.OData.ODataReader]] CreateODataDeltaResourceSetReaderAsync ()
	public System.Threading.Tasks.Task`1[[Microsoft.OData.ODataReader]] CreateODataDeltaResourceSetReaderAsync (Microsoft.OData.Edm.IEdmStructuredType expectedResourceType)
	public System.Threading.Tasks.Task`1[[Microsoft.OData.ODataReader]] CreateODataDeltaResourceSetReaderAsync (Microsoft.OData.Edm.IEdmEntitySetBase entitySet, Microsoft.OData.Edm.IEdmStructuredType expectedResourceType)
	public Microsoft.OData.ODataParameterReader CreateODataParameterReader (Microsoft.OData.Edm.IEdmOperation operation)
	public System.Threading.Tasks.Task`1[[Microsoft.OData.ODataParameterReader]] CreateODataParameterReaderAsync (Microsoft.OData.Edm.IEdmOperation operation)
	public Microsoft.OData.ODataReader CreateODataResourceReader ()
	public Microsoft.OData.ODataReader CreateODataResourceReader (Microsoft.OData.Edm.IEdmStructuredType resourceType)
	public Microsoft.OData.ODataReader CreateODataResourceReader (Microsoft.OData.Edm.IEdmNavigationSource navigationSource, Microsoft.OData.Edm.IEdmStructuredType resourceType)
	public System.Threading.Tasks.Task`1[[Microsoft.OData.ODataReader]] CreateODataResourceReaderAsync ()
	public System.Threading.Tasks.Task`1[[Microsoft.OData.ODataReader]] CreateODataResourceReaderAsync (Microsoft.OData.Edm.IEdmStructuredType resourceType)
	public System.Threading.Tasks.Task`1[[Microsoft.OData.ODataReader]] CreateODataResourceReaderAsync (Microsoft.OData.Edm.IEdmNavigationSource navigationSource, Microsoft.OData.Edm.IEdmStructuredType resourceType)
	public Microsoft.OData.ODataReader CreateODataResourceSetReader ()
	public Microsoft.OData.ODataReader CreateODataResourceSetReader (Microsoft.OData.Edm.IEdmStructuredType expectedResourceType)
	public Microsoft.OData.ODataReader CreateODataResourceSetReader (Microsoft.OData.Edm.IEdmEntitySetBase entitySet, Microsoft.OData.Edm.IEdmStructuredType expectedResourceType)
	public System.Threading.Tasks.Task`1[[Microsoft.OData.ODataReader]] CreateODataResourceSetReaderAsync ()
	public System.Threading.Tasks.Task`1[[Microsoft.OData.ODataReader]] CreateODataResourceSetReaderAsync (Microsoft.OData.Edm.IEdmStructuredType expectedResourceType)
	public System.Threading.Tasks.Task`1[[Microsoft.OData.ODataReader]] CreateODataResourceSetReaderAsync (Microsoft.OData.Edm.IEdmEntitySetBase entitySet, Microsoft.OData.Edm.IEdmStructuredType expectedResourceType)
	public Microsoft.OData.ODataReader CreateODataUriParameterResourceReader (Microsoft.OData.Edm.IEdmNavigationSource navigationSource, Microsoft.OData.Edm.IEdmStructuredType expectedResourceType)
	public System.Threading.Tasks.Task`1[[Microsoft.OData.ODataReader]] CreateODataUriParameterResourceReaderAsync (Microsoft.OData.Edm.IEdmNavigationSource navigationSource, Microsoft.OData.Edm.IEdmStructuredType expectedResourceType)
	public Microsoft.OData.ODataReader CreateODataUriParameterResourceSetReader (Microsoft.OData.Edm.IEdmEntitySetBase entitySet, Microsoft.OData.Edm.IEdmStructuredType expectedResourceType)
	public System.Threading.Tasks.Task`1[[Microsoft.OData.ODataReader]] CreateODataUriParameterResourceSetReaderAsync (Microsoft.OData.Edm.IEdmEntitySetBase entitySet, Microsoft.OData.Edm.IEdmStructuredType expectedResourceType)
	public System.Collections.Generic.IEnumerable`1[[Microsoft.OData.ODataPayloadKindDetectionResult]] DetectPayloadKind ()
	public System.Threading.Tasks.Task`1[[System.Collections.Generic.IEnumerable`1[[Microsoft.OData.ODataPayloadKindDetectionResult]]]] DetectPayloadKindAsync ()
	public virtual void Dispose ()
	public Microsoft.OData.ODataEntityReferenceLink ReadEntityReferenceLink ()
	public System.Threading.Tasks.Task`1[[Microsoft.OData.ODataEntityReferenceLink]] ReadEntityReferenceLinkAsync ()
	public Microsoft.OData.ODataEntityReferenceLinks ReadEntityReferenceLinks ()
	public System.Threading.Tasks.Task`1[[Microsoft.OData.ODataEntityReferenceLinks]] ReadEntityReferenceLinksAsync ()
	public Microsoft.OData.ODataError ReadError ()
	public System.Threading.Tasks.Task`1[[Microsoft.OData.ODataError]] ReadErrorAsync ()
	public Microsoft.OData.Edm.IEdmModel ReadMetadataDocument ()
	public Microsoft.OData.Edm.IEdmModel ReadMetadataDocument (System.Func`2[[System.Uri],[System.Xml.XmlReader]] getReferencedModelReaderFunc)
	public Microsoft.OData.ODataProperty ReadProperty ()
	public Microsoft.OData.ODataProperty ReadProperty (Microsoft.OData.Edm.IEdmStructuralProperty property)
	public Microsoft.OData.ODataProperty ReadProperty (Microsoft.OData.Edm.IEdmTypeReference expectedPropertyTypeReference)
	public System.Threading.Tasks.Task`1[[Microsoft.OData.ODataProperty]] ReadPropertyAsync ()
	public System.Threading.Tasks.Task`1[[Microsoft.OData.ODataProperty]] ReadPropertyAsync (Microsoft.OData.Edm.IEdmStructuralProperty property)
	public System.Threading.Tasks.Task`1[[Microsoft.OData.ODataProperty]] ReadPropertyAsync (Microsoft.OData.Edm.IEdmTypeReference expectedPropertyTypeReference)
	public Microsoft.OData.ODataServiceDocument ReadServiceDocument ()
	public System.Threading.Tasks.Task`1[[Microsoft.OData.ODataServiceDocument]] ReadServiceDocumentAsync ()
	public object ReadValue (Microsoft.OData.Edm.IEdmTypeReference expectedTypeReference)
	public System.Threading.Tasks.Task`1[[System.Object]] ReadValueAsync (Microsoft.OData.Edm.IEdmTypeReference expectedTypeReference)
}

public sealed class Microsoft.OData.ODataMessageReaderSettings {
	public ODataMessageReaderSettings ()
	public ODataMessageReaderSettings (Microsoft.OData.ODataVersion odataVersion)

	System.Uri BaseUri  { public get; public set; }
	System.Func`3[[Microsoft.OData.Edm.IEdmType],[System.String],[Microsoft.OData.Edm.IEdmType]] ClientCustomTypeResolver  { public get; public set; }
	bool EnableCharactersCheck  { public get; public set; }
	bool EnableMessageStreamDisposal  { public get; public set; }
	bool EnablePrimitiveTypeConversion  { public get; public set; }
	Microsoft.OData.ODataVersion MaxProtocolVersion  { public get; public set; }
	Microsoft.OData.ODataMessageQuotas MessageQuotas  { public get; public set; }
	System.Func`3[[System.Object],[System.String],[Microsoft.OData.Edm.IEdmTypeReference]] PrimitiveTypeResolver  { public get; public set; }
	bool ReadUntypedAsString  { public get; public set; }
	System.Func`2[[System.String],[System.Boolean]] ShouldIncludeAnnotation  { public get; public set; }
	Microsoft.OData.ValidationKinds Validations  { public get; public set; }
	System.Nullable`1[[Microsoft.OData.ODataVersion]] Version  { public get; public set; }

	public Microsoft.OData.ODataMessageReaderSettings Clone ()
}

public sealed class Microsoft.OData.ODataMessageWriter : IDisposable {
	public ODataMessageWriter (Microsoft.OData.IODataRequestMessage requestMessage)
	public ODataMessageWriter (Microsoft.OData.IODataResponseMessage responseMessage)
	public ODataMessageWriter (Microsoft.OData.IODataRequestMessage requestMessage, Microsoft.OData.ODataMessageWriterSettings settings)
	public ODataMessageWriter (Microsoft.OData.IODataResponseMessage responseMessage, Microsoft.OData.ODataMessageWriterSettings settings)
	public ODataMessageWriter (Microsoft.OData.IODataRequestMessage requestMessage, Microsoft.OData.ODataMessageWriterSettings settings, Microsoft.OData.Edm.IEdmModel model)
	public ODataMessageWriter (Microsoft.OData.IODataResponseMessage responseMessage, Microsoft.OData.ODataMessageWriterSettings settings, Microsoft.OData.Edm.IEdmModel model)

	public Microsoft.OData.ODataAsynchronousWriter CreateODataAsynchronousWriter ()
	public System.Threading.Tasks.Task`1[[Microsoft.OData.ODataAsynchronousWriter]] CreateODataAsynchronousWriterAsync ()
	public Microsoft.OData.ODataBatchWriter CreateODataBatchWriter ()
	public System.Threading.Tasks.Task`1[[Microsoft.OData.ODataBatchWriter]] CreateODataBatchWriterAsync ()
	public Microsoft.OData.ODataCollectionWriter CreateODataCollectionWriter ()
	public Microsoft.OData.ODataCollectionWriter CreateODataCollectionWriter (Microsoft.OData.Edm.IEdmTypeReference itemTypeReference)
	public System.Threading.Tasks.Task`1[[Microsoft.OData.ODataCollectionWriter]] CreateODataCollectionWriterAsync ()
	public System.Threading.Tasks.Task`1[[Microsoft.OData.ODataCollectionWriter]] CreateODataCollectionWriterAsync (Microsoft.OData.Edm.IEdmTypeReference itemTypeReference)
	public Microsoft.OData.ODataWriter CreateODataDeltaResourceSetWriter ()
	public Microsoft.OData.ODataWriter CreateODataDeltaResourceSetWriter (Microsoft.OData.Edm.IEdmEntitySetBase entitySet)
	public Microsoft.OData.ODataWriter CreateODataDeltaResourceSetWriter (Microsoft.OData.Edm.IEdmEntitySetBase entitySet, Microsoft.OData.Edm.IEdmStructuredType resourceType)
	public System.Threading.Tasks.Task`1[[Microsoft.OData.ODataWriter]] CreateODataDeltaResourceSetWriterAsync ()
	public System.Threading.Tasks.Task`1[[Microsoft.OData.ODataWriter]] CreateODataDeltaResourceSetWriterAsync (Microsoft.OData.Edm.IEdmEntitySetBase entitySet)
	public System.Threading.Tasks.Task`1[[Microsoft.OData.ODataWriter]] CreateODataDeltaResourceSetWriterAsync (Microsoft.OData.Edm.IEdmEntitySetBase entitySet, Microsoft.OData.Edm.IEdmEntityType entityType)
	[
	ObsoleteAttribute(),
	]
	public Microsoft.OData.ODataDeltaWriter CreateODataDeltaWriter (Microsoft.OData.Edm.IEdmEntitySetBase entitySet, Microsoft.OData.Edm.IEdmEntityType entityType)

	[
	ObsoleteAttribute(),
	]
	public System.Threading.Tasks.Task`1[[Microsoft.OData.ODataDeltaWriter]] CreateODataDeltaWriterAsync (Microsoft.OData.Edm.IEdmEntitySetBase entitySet, Microsoft.OData.Edm.IEdmEntityType entityType)

	public Microsoft.OData.ODataParameterWriter CreateODataParameterWriter (Microsoft.OData.Edm.IEdmOperation operation)
	public System.Threading.Tasks.Task`1[[Microsoft.OData.ODataParameterWriter]] CreateODataParameterWriterAsync (Microsoft.OData.Edm.IEdmOperation operation)
	public Microsoft.OData.ODataWriter CreateODataResourceSetWriter ()
	public Microsoft.OData.ODataWriter CreateODataResourceSetWriter (Microsoft.OData.Edm.IEdmEntitySetBase entitySet)
	public Microsoft.OData.ODataWriter CreateODataResourceSetWriter (Microsoft.OData.Edm.IEdmEntitySetBase entitySet, Microsoft.OData.Edm.IEdmStructuredType resourceType)
	public System.Threading.Tasks.Task`1[[Microsoft.OData.ODataWriter]] CreateODataResourceSetWriterAsync ()
	public System.Threading.Tasks.Task`1[[Microsoft.OData.ODataWriter]] CreateODataResourceSetWriterAsync (Microsoft.OData.Edm.IEdmEntitySetBase entitySet)
	public System.Threading.Tasks.Task`1[[Microsoft.OData.ODataWriter]] CreateODataResourceSetWriterAsync (Microsoft.OData.Edm.IEdmEntitySetBase entitySet, Microsoft.OData.Edm.IEdmEntityType entityType)
	public Microsoft.OData.ODataWriter CreateODataResourceWriter ()
	public Microsoft.OData.ODataWriter CreateODataResourceWriter (Microsoft.OData.Edm.IEdmNavigationSource navigationSource)
	public Microsoft.OData.ODataWriter CreateODataResourceWriter (Microsoft.OData.Edm.IEdmNavigationSource navigationSource, Microsoft.OData.Edm.IEdmStructuredType resourceType)
	public System.Threading.Tasks.Task`1[[Microsoft.OData.ODataWriter]] CreateODataResourceWriterAsync ()
	public System.Threading.Tasks.Task`1[[Microsoft.OData.ODataWriter]] CreateODataResourceWriterAsync (Microsoft.OData.Edm.IEdmNavigationSource navigationSource)
	public System.Threading.Tasks.Task`1[[Microsoft.OData.ODataWriter]] CreateODataResourceWriterAsync (Microsoft.OData.Edm.IEdmNavigationSource navigationSource, Microsoft.OData.Edm.IEdmStructuredType resourceType)
	public Microsoft.OData.ODataWriter CreateODataUriParameterResourceSetWriter (Microsoft.OData.Edm.IEdmEntitySetBase entitySetBase, Microsoft.OData.Edm.IEdmStructuredType resourceType)
	public System.Threading.Tasks.Task`1[[Microsoft.OData.ODataWriter]] CreateODataUriParameterResourceSetWriterAsync (Microsoft.OData.Edm.IEdmEntitySetBase entitySetBase, Microsoft.OData.Edm.IEdmStructuredType resourceType)
	public Microsoft.OData.ODataWriter CreateODataUriParameterResourceWriter (Microsoft.OData.Edm.IEdmNavigationSource navigationSource, Microsoft.OData.Edm.IEdmStructuredType resourceType)
	public System.Threading.Tasks.Task`1[[Microsoft.OData.ODataWriter]] CreateODataUriParameterResourceWriterAsync (Microsoft.OData.Edm.IEdmNavigationSource navigationSource, Microsoft.OData.Edm.IEdmStructuredType resourceType)
	public virtual void Dispose ()
	public void WriteEntityReferenceLink (Microsoft.OData.ODataEntityReferenceLink link)
	public System.Threading.Tasks.Task WriteEntityReferenceLinkAsync (Microsoft.OData.ODataEntityReferenceLink link)
	public void WriteEntityReferenceLinks (Microsoft.OData.ODataEntityReferenceLinks links)
	public System.Threading.Tasks.Task WriteEntityReferenceLinksAsync (Microsoft.OData.ODataEntityReferenceLinks links)
	public void WriteError (Microsoft.OData.ODataError error, bool includeDebugInformation)
	public System.Threading.Tasks.Task WriteErrorAsync (Microsoft.OData.ODataError error, bool includeDebugInformation)
	public void WriteMetadataDocument ()
	public void WriteProperty (Microsoft.OData.ODataProperty property)
	public System.Threading.Tasks.Task WritePropertyAsync (Microsoft.OData.ODataProperty property)
	public void WriteServiceDocument (Microsoft.OData.ODataServiceDocument serviceDocument)
	public System.Threading.Tasks.Task WriteServiceDocumentAsync (Microsoft.OData.ODataServiceDocument serviceDocument)
	public void WriteValue (object value)
	public System.Threading.Tasks.Task WriteValueAsync (object value)
}

public sealed class Microsoft.OData.ODataMessageWriterSettings {
	public ODataMessageWriterSettings ()

	System.Uri BaseUri  { public get; public set; }
	bool EnableCharactersCheck  { public get; public set; }
	bool EnableMessageStreamDisposal  { public get; public set; }
	string JsonPCallback  { public get; public set; }
	Microsoft.OData.ODataMessageQuotas MessageQuotas  { public get; public set; }
	Microsoft.OData.ODataUri ODataUri  { public get; public set; }
	Microsoft.OData.ValidationKinds Validations  { public get; public set; }
	System.Nullable`1[[Microsoft.OData.ODataVersion]] Version  { public get; public set; }

	public Microsoft.OData.ODataMessageWriterSettings Clone ()
	public void SetContentType (Microsoft.OData.ODataFormat payloadFormat)
	public void SetContentType (string acceptableMediaTypes, string acceptableCharSets)
}

[
DebuggerDisplayAttribute(),
]
public sealed class Microsoft.OData.ODataNestedResourceInfo : Microsoft.OData.ODataItem {
	public ODataNestedResourceInfo ()

	System.Uri AssociationLinkUrl  { public get; public set; }
	System.Nullable`1[[System.Boolean]] IsCollection  { public get; public set; }
	string Name  { public get; public set; }
	System.Uri Url  { public get; public set; }
}

public sealed class Microsoft.OData.ODataNestedResourceInfoSerializationInfo {
	public ODataNestedResourceInfoSerializationInfo ()

	bool IsComplex  { public get; public set; }
	bool IsUndeclared  { public get; public set; }
}

public sealed class Microsoft.OData.ODataNullValue : Microsoft.OData.ODataValue {
	public ODataNullValue ()
}

public sealed class Microsoft.OData.ODataPayloadKindDetectionResult {
	Microsoft.OData.ODataFormat Format  { public get; }
	Microsoft.OData.ODataPayloadKind PayloadKind  { public get; }
}

public sealed class Microsoft.OData.ODataPrimitiveValue : Microsoft.OData.ODataValue {
	public ODataPrimitiveValue (object value)

	object Value  { public get; }
}

public sealed class Microsoft.OData.ODataProperty : Microsoft.OData.ODataAnnotatable {
	public ODataProperty ()

	System.Collections.Generic.ICollection`1[[Microsoft.OData.ODataInstanceAnnotation]] InstanceAnnotations  { public get; public set; }
	string Name  { public get; public set; }
	object Value  { public get; public set; }
}

public sealed class Microsoft.OData.ODataPropertySerializationInfo {
	public ODataPropertySerializationInfo ()

	Microsoft.OData.ODataPropertyKind PropertyKind  { public get; public set; }
}

public sealed class Microsoft.OData.ODataResource : Microsoft.OData.ODataResourceBase {
	public ODataResource ()
}

public sealed class Microsoft.OData.ODataResourceSerializationInfo {
	public ODataResourceSerializationInfo ()

	string ExpectedTypeName  { public get; public set; }
	bool IsFromCollection  { public get; public set; }
	string NavigationSourceEntityTypeName  { public get; public set; }
	Microsoft.OData.Edm.EdmNavigationSourceKind NavigationSourceKind  { public get; public set; }
	string NavigationSourceName  { public get; public set; }
}

public sealed class Microsoft.OData.ODataResourceSet : Microsoft.OData.ODataResourceSetBase {
	public ODataResourceSet ()

	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.ODataAction]] Actions  { public get; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.ODataFunction]] Functions  { public get; }

	public void AddAction (Microsoft.OData.ODataAction action)
	public void AddFunction (Microsoft.OData.ODataFunction function)
}

public sealed class Microsoft.OData.ODataServiceDocument : Microsoft.OData.ODataAnnotatable {
	public ODataServiceDocument ()

	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.ODataEntitySetInfo]] EntitySets  { public get; public set; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.ODataFunctionImportInfo]] FunctionImports  { public get; public set; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.ODataSingletonInfo]] Singletons  { public get; public set; }
}

public sealed class Microsoft.OData.ODataSimplifiedOptions {
	public ODataSimplifiedOptions ()
	public ODataSimplifiedOptions (System.Nullable`1[[Microsoft.OData.ODataVersion]] version)

	bool EnableParsingKeyAsSegmentUrl  { public get; public set; }
	bool EnableReadingKeyAsSegment  { public get; public set; }
	bool EnableReadingODataAnnotationWithoutPrefix  { public get; public set; }
	bool EnableWritingKeyAsSegment  { public get; public set; }
	bool EnableWritingODataAnnotationWithoutPrefix  { public get; public set; }

	public Microsoft.OData.ODataSimplifiedOptions Clone ()
}

public sealed class Microsoft.OData.ODataSingletonInfo : Microsoft.OData.ODataServiceDocumentElement {
	public ODataSingletonInfo ()
}

public sealed class Microsoft.OData.ODataStreamReferenceValue : Microsoft.OData.ODataValue {
	public ODataStreamReferenceValue ()

	string ContentType  { public get; public set; }
	System.Uri EditLink  { public get; public set; }
	string ETag  { public get; public set; }
	System.Uri ReadLink  { public get; public set; }
}

public sealed class Microsoft.OData.ODataTypeAnnotation {
	public ODataTypeAnnotation ()
	public ODataTypeAnnotation (string typeName)

	string TypeName  { public get; }
}

public sealed class Microsoft.OData.ODataUntypedValue : Microsoft.OData.ODataValue {
	public ODataUntypedValue ()

	string RawValue  { public get; public set; }
}

public sealed class Microsoft.OData.ODataUri {
	public ODataUri ()

	Microsoft.OData.UriParser.Aggregation.ApplyClause Apply  { public get; public set; }
	Microsoft.OData.UriParser.ComputeClause Compute  { public get; public set; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.UriParser.QueryNode]] CustomQueryOptions  { public get; public set; }
	string DeltaToken  { public get; public set; }
	Microsoft.OData.UriParser.FilterClause Filter  { public get; public set; }
	Microsoft.OData.UriParser.OrderByClause OrderBy  { public get; public set; }
	System.Collections.Generic.IDictionary`2[[System.String],[Microsoft.OData.UriParser.SingleValueNode]] ParameterAliasNodes  { public get; }
	Microsoft.OData.UriParser.ODataPath Path  { public get; public set; }
	System.Nullable`1[[System.Boolean]] QueryCount  { public get; public set; }
	System.Uri RequestUri  { public get; public set; }
	Microsoft.OData.UriParser.SearchClause Search  { public get; public set; }
	Microsoft.OData.UriParser.SelectExpandClause SelectAndExpand  { public get; public set; }
	System.Uri ServiceRoot  { public get; public set; }
	System.Nullable`1[[System.Int64]] Skip  { public get; public set; }
	string SkipToken  { public get; public set; }
	System.Nullable`1[[System.Int64]] Top  { public get; public set; }

	public Microsoft.OData.ODataUri Clone ()
}

public sealed class Microsoft.OData.ODataUrlKeyDelimiter {
	Microsoft.OData.ODataUrlKeyDelimiter Parentheses  { public static get; }
	Microsoft.OData.ODataUrlKeyDelimiter Slash  { public static get; }
}

public enum Microsoft.OData.Json.JsonNodeType : int {
	EndArray = 4
	EndObject = 2
	EndOfInput = 7
	None = 0
	PrimitiveValue = 6
	Property = 5
	StartArray = 3
	StartObject = 1
}

public interface Microsoft.OData.Json.IJsonReader {
	bool IsIeee754Compatible  { public abstract get; }
	Microsoft.OData.Json.JsonNodeType NodeType  { public abstract get; }
	object Value  { public abstract get; }

	bool Read ()
}

public interface Microsoft.OData.Json.IJsonReaderFactory {
	Microsoft.OData.Json.IJsonReader CreateJsonReader (System.IO.TextReader textReader, bool isIeee754Compatible)
}

[
CLSCompliantAttribute(),
]
public interface Microsoft.OData.Json.IJsonWriter {
	void EndArrayScope ()
	void EndObjectScope ()
	void EndPaddingFunctionScope ()
	void Flush ()
	void StartArrayScope ()
	void StartObjectScope ()
	void StartPaddingFunctionScope ()
	void WriteName (string name)
	void WritePaddingFunctionName (string functionName)
	void WriteRawValue (string rawValue)
	void WriteValue (Microsoft.OData.Edm.Date value)
	void WriteValue (Microsoft.OData.Edm.TimeOfDay value)
	void WriteValue (bool value)
	void WriteValue (byte value)
	void WriteValue (byte[] value)
	void WriteValue (System.DateTimeOffset value)
	void WriteValue (decimal value)
	void WriteValue (double value)
	void WriteValue (System.Guid value)
	void WriteValue (short value)
	void WriteValue (int value)
	void WriteValue (long value)
	void WriteValue (System.SByte value)
	void WriteValue (float value)
	void WriteValue (string value)
	void WriteValue (System.TimeSpan value)
}

[
CLSCompliantAttribute(),
]
public interface Microsoft.OData.Json.IJsonWriterFactory {
	Microsoft.OData.Json.IJsonWriter CreateJsonWriter (System.IO.TextWriter textWriter, bool isIeee754Compatible)
}

public enum Microsoft.OData.UriParser.BinaryOperatorKind : int {
	Add = 8
	And = 1
	Divide = 11
	Equal = 2
	GreaterThan = 4
	GreaterThanOrEqual = 5
	Has = 13
	LessThan = 6
	LessThanOrEqual = 7
	Modulo = 12
	Multiply = 10
	NotEqual = 3
	Or = 0
	Subtract = 9
}

public enum Microsoft.OData.UriParser.OrderByDirection : int {
	Ascending = 0
	Descending = 1
}

public enum Microsoft.OData.UriParser.QueryNodeKind : int {
	All = 14
	Any = 9
	BinaryOperator = 4
	CollectionComplexNode = 26
	CollectionFunctionCall = 18
	CollectionNavigationNode = 10
	CollectionOpenPropertyAccess = 25
	CollectionPropertyAccess = 7
	CollectionResourceCast = 15
	CollectionResourceFunctionCall = 19
	Constant = 1
	Convert = 2
	Count = 28
	EntitySet = 22
	KeyLookup = 23
	NamedFunctionParameter = 20
	None = 0
	NonResourceRangeVariableReference = 3
	ParameterAlias = 21
	ResourceRangeVariableReference = 16
	SearchTerm = 24
	SingleComplexNode = 27
	SingleNavigationNode = 11
	SingleResourceCast = 13
	SingleResourceFunctionCall = 17
	SingleValueCast = 29
	SingleValueFunctionCall = 8
	SingleValueOpenPropertyAccess = 12
	SingleValuePropertyAccess = 6
	UnaryOperator = 5
}

public enum Microsoft.OData.UriParser.QueryTokenKind : int {
	Aggregate = 24
	AggregateExpression = 25
	AggregateGroupBy = 26
	All = 19
	Any = 15
	BinaryOperator = 3
	Compute = 27
	ComputeExpression = 28
	CustomQueryOption = 9
	DottedIdentifier = 17
	EndPath = 7
	Expand = 13
	ExpandTerm = 20
	FunctionCall = 6
	FunctionParameter = 21
	FunctionParameterAlias = 22
	InnerPath = 16
	Literal = 5
	OrderBy = 8
	RangeVariable = 18
	Select = 10
	Star = 11
	StringLiteral = 23
	TypeSegment = 14
	UnaryOperator = 4
}

public enum Microsoft.OData.UriParser.UnaryOperatorKind : int {
	Negate = 0
	Not = 1
}

public interface Microsoft.OData.UriParser.IPathSegmentTokenVisitor {
	void Visit (Microsoft.OData.UriParser.NonSystemToken tokenIn)
	void Visit (Microsoft.OData.UriParser.SystemToken tokenIn)
}

public interface Microsoft.OData.UriParser.IPathSegmentTokenVisitor`1 {
	T Visit (Microsoft.OData.UriParser.NonSystemToken tokenIn)
	T Visit (Microsoft.OData.UriParser.SystemToken tokenIn)
}

public interface Microsoft.OData.UriParser.ISyntacticTreeVisitor`1 {
	T Visit (Microsoft.OData.UriParser.Aggregation.AggregateExpressionToken tokenIn)
	T Visit (Microsoft.OData.UriParser.Aggregation.AggregateToken tokenIn)
	T Visit (Microsoft.OData.UriParser.Aggregation.GroupByToken tokenIn)
	T Visit (Microsoft.OData.UriParser.AllToken tokenIn)
	T Visit (Microsoft.OData.UriParser.AnyToken tokenIn)
	T Visit (Microsoft.OData.UriParser.BinaryOperatorToken tokenIn)
	T Visit (Microsoft.OData.UriParser.CustomQueryOptionToken tokenIn)
	T Visit (Microsoft.OData.UriParser.DottedIdentifierToken tokenIn)
	T Visit (Microsoft.OData.UriParser.EndPathToken tokenIn)
	T Visit (Microsoft.OData.UriParser.ExpandTermToken tokenIn)
	T Visit (Microsoft.OData.UriParser.ExpandToken tokenIn)
	T Visit (Microsoft.OData.UriParser.FunctionCallToken tokenIn)
	T Visit (Microsoft.OData.UriParser.FunctionParameterToken tokenIn)
	T Visit (Microsoft.OData.UriParser.InnerPathToken tokenIn)
	T Visit (Microsoft.OData.UriParser.LambdaToken tokenIn)
	T Visit (Microsoft.OData.UriParser.LiteralToken tokenIn)
	T Visit (Microsoft.OData.UriParser.OrderByToken tokenIn)
	T Visit (Microsoft.OData.UriParser.RangeVariableToken tokenIn)
	T Visit (Microsoft.OData.UriParser.SelectToken tokenIn)
	T Visit (Microsoft.OData.UriParser.StarToken tokenIn)
	T Visit (Microsoft.OData.UriParser.UnaryOperatorToken tokenIn)
}

public interface Microsoft.OData.UriParser.IUriLiteralParser {
	object ParseUriStringToType (string text, Microsoft.OData.Edm.IEdmTypeReference targetType, out Microsoft.OData.UriParser.UriLiteralParsingException& parsingException)
}

public abstract class Microsoft.OData.UriParser.CollectionNode : Microsoft.OData.UriParser.QueryNode {
	protected CollectionNode ()

	Microsoft.OData.Edm.IEdmCollectionTypeReference CollectionType  { public abstract get; }
	Microsoft.OData.Edm.IEdmTypeReference ItemType  { public abstract get; }
	Microsoft.OData.UriParser.QueryNodeKind Kind  { public virtual get; }
}

public abstract class Microsoft.OData.UriParser.CollectionResourceNode : Microsoft.OData.UriParser.CollectionNode {
	protected CollectionResourceNode ()

	Microsoft.OData.Edm.IEdmStructuredTypeReference ItemStructuredType  { public abstract get; }
	Microsoft.OData.Edm.IEdmNavigationSource NavigationSource  { public abstract get; }
}

public abstract class Microsoft.OData.UriParser.LambdaNode : Microsoft.OData.UriParser.SingleValueNode {
	protected LambdaNode (System.Collections.ObjectModel.Collection`1[[Microsoft.OData.UriParser.RangeVariable]] rangeVariables)
	protected LambdaNode (System.Collections.ObjectModel.Collection`1[[Microsoft.OData.UriParser.RangeVariable]] rangeVariables, Microsoft.OData.UriParser.RangeVariable currentRangeVariable)

	Microsoft.OData.UriParser.SingleValueNode Body  { public get; public set; }
	Microsoft.OData.UriParser.RangeVariable CurrentRangeVariable  { public get; }
	System.Collections.ObjectModel.Collection`1[[Microsoft.OData.UriParser.RangeVariable]] RangeVariables  { public get; }
	Microsoft.OData.UriParser.CollectionNode Source  { public get; public set; }
}

public abstract class Microsoft.OData.UriParser.LambdaToken : Microsoft.OData.UriParser.QueryToken {
	protected LambdaToken (Microsoft.OData.UriParser.QueryToken expression, string parameter, Microsoft.OData.UriParser.QueryToken parent)

	Microsoft.OData.UriParser.QueryToken Expression  { public get; }
	string Parameter  { public get; }
	Microsoft.OData.UriParser.QueryToken Parent  { public get; }

	public virtual T Accept (ISyntacticTreeVisitor`1 visitor)
}

public abstract class Microsoft.OData.UriParser.ODataPathSegment {
	protected ODataPathSegment ()

	Microsoft.OData.Edm.IEdmType EdmType  { public abstract get; }
	string Identifier  { public get; public set; }

	internal virtual bool Equals (Microsoft.OData.UriParser.ODataPathSegment other)
	public abstract void HandleWith (Microsoft.OData.UriParser.PathSegmentHandler handler)
	public abstract T TranslateWith (PathSegmentTranslator`1 translator)
}

public abstract class Microsoft.OData.UriParser.PathSegmentHandler {
	protected PathSegmentHandler ()

	public virtual void Handle (Microsoft.OData.UriParser.AnnotationSegment segment)
	public virtual void Handle (Microsoft.OData.UriParser.BatchReferenceSegment segment)
	public virtual void Handle (Microsoft.OData.UriParser.BatchSegment segment)
	public virtual void Handle (Microsoft.OData.UriParser.CountSegment segment)
	public virtual void Handle (Microsoft.OData.UriParser.DynamicPathSegment segment)
	public virtual void Handle (Microsoft.OData.UriParser.EntitySetSegment segment)
	public virtual void Handle (Microsoft.OData.UriParser.KeySegment segment)
	public virtual void Handle (Microsoft.OData.UriParser.MetadataSegment segment)
	public virtual void Handle (Microsoft.OData.UriParser.NavigationPropertyLinkSegment segment)
	public virtual void Handle (Microsoft.OData.UriParser.NavigationPropertySegment segment)
	public virtual void Handle (Microsoft.OData.UriParser.ODataPathSegment segment)
	public virtual void Handle (Microsoft.OData.UriParser.OperationImportSegment segment)
	public virtual void Handle (Microsoft.OData.UriParser.OperationSegment segment)
	public virtual void Handle (Microsoft.OData.UriParser.PathTemplateSegment segment)
	public virtual void Handle (Microsoft.OData.UriParser.PropertySegment segment)
	public virtual void Handle (Microsoft.OData.UriParser.SingletonSegment segment)
	public virtual void Handle (Microsoft.OData.UriParser.TypeSegment segment)
	public virtual void Handle (Microsoft.OData.UriParser.ValueSegment segment)
}

public abstract class Microsoft.OData.UriParser.PathSegmentToken {
	protected PathSegmentToken (Microsoft.OData.UriParser.PathSegmentToken nextToken)

	string Identifier  { public abstract get; }
	bool IsStructuralProperty  { public get; public set; }
	Microsoft.OData.UriParser.PathSegmentToken NextToken  { public get; }

	public abstract T Accept (IPathSegmentTokenVisitor`1 visitor)
	public abstract void Accept (Microsoft.OData.UriParser.IPathSegmentTokenVisitor visitor)
	public abstract bool IsNamespaceOrContainerQualified ()
}

public abstract class Microsoft.OData.UriParser.PathSegmentTranslator`1 {
	protected PathSegmentTranslator`1 ()

	public virtual T Translate (Microsoft.OData.UriParser.AnnotationSegment segment)
	public virtual T Translate (Microsoft.OData.UriParser.BatchReferenceSegment segment)
	public virtual T Translate (Microsoft.OData.UriParser.BatchSegment segment)
	public virtual T Translate (Microsoft.OData.UriParser.CountSegment segment)
	public virtual T Translate (Microsoft.OData.UriParser.DynamicPathSegment segment)
	public virtual T Translate (Microsoft.OData.UriParser.EntitySetSegment segment)
	public virtual T Translate (Microsoft.OData.UriParser.KeySegment segment)
	public virtual T Translate (Microsoft.OData.UriParser.MetadataSegment segment)
	public virtual T Translate (Microsoft.OData.UriParser.NavigationPropertyLinkSegment segment)
	public virtual T Translate (Microsoft.OData.UriParser.NavigationPropertySegment segment)
	public virtual T Translate (Microsoft.OData.UriParser.OperationImportSegment segment)
	public virtual T Translate (Microsoft.OData.UriParser.OperationSegment segment)
	public virtual T Translate (Microsoft.OData.UriParser.PathTemplateSegment segment)
	public virtual T Translate (Microsoft.OData.UriParser.PropertySegment segment)
	public virtual T Translate (Microsoft.OData.UriParser.SingletonSegment segment)
	public virtual T Translate (Microsoft.OData.UriParser.TypeSegment segment)
	public virtual T Translate (Microsoft.OData.UriParser.ValueSegment segment)
}

public abstract class Microsoft.OData.UriParser.PathToken : Microsoft.OData.UriParser.QueryToken {
	protected PathToken ()

	string Identifier  { public abstract get; }
	Microsoft.OData.UriParser.QueryToken NextToken  { public abstract get; public abstract set; }
}

public abstract class Microsoft.OData.UriParser.QueryNode {
	protected QueryNode ()

	Microsoft.OData.UriParser.QueryNodeKind Kind  { public abstract get; }

	public virtual T Accept (QueryNodeVisitor`1 visitor)
}

public abstract class Microsoft.OData.UriParser.QueryNodeVisitor`1 {
	protected QueryNodeVisitor`1 ()

	public virtual T Visit (Microsoft.OData.UriParser.AllNode nodeIn)
	public virtual T Visit (Microsoft.OData.UriParser.AnyNode nodeIn)
	public virtual T Visit (Microsoft.OData.UriParser.BinaryOperatorNode nodeIn)
	public virtual T Visit (Microsoft.OData.UriParser.CollectionComplexNode nodeIn)
	public virtual T Visit (Microsoft.OData.UriParser.CollectionFunctionCallNode nodeIn)
	public virtual T Visit (Microsoft.OData.UriParser.CollectionNavigationNode nodeIn)
	public virtual T Visit (Microsoft.OData.UriParser.CollectionOpenPropertyAccessNode nodeIn)
	public virtual T Visit (Microsoft.OData.UriParser.CollectionPropertyAccessNode nodeIn)
	public virtual T Visit (Microsoft.OData.UriParser.CollectionResourceCastNode nodeIn)
	public virtual T Visit (Microsoft.OData.UriParser.CollectionResourceFunctionCallNode nodeIn)
	public virtual T Visit (Microsoft.OData.UriParser.ConstantNode nodeIn)
	public virtual T Visit (Microsoft.OData.UriParser.ConvertNode nodeIn)
	public virtual T Visit (Microsoft.OData.UriParser.CountNode nodeIn)
	public virtual T Visit (Microsoft.OData.UriParser.NamedFunctionParameterNode nodeIn)
	public virtual T Visit (Microsoft.OData.UriParser.NonResourceRangeVariableReferenceNode nodeIn)
	public virtual T Visit (Microsoft.OData.UriParser.ParameterAliasNode nodeIn)
	public virtual T Visit (Microsoft.OData.UriParser.ResourceRangeVariableReferenceNode nodeIn)
	public virtual T Visit (Microsoft.OData.UriParser.SearchTermNode nodeIn)
	public virtual T Visit (Microsoft.OData.UriParser.SingleComplexNode nodeIn)
	public virtual T Visit (Microsoft.OData.UriParser.SingleNavigationNode nodeIn)
	public virtual T Visit (Microsoft.OData.UriParser.SingleResourceCastNode nodeIn)
	public virtual T Visit (Microsoft.OData.UriParser.SingleResourceFunctionCallNode nodeIn)
	public virtual T Visit (Microsoft.OData.UriParser.SingleValueCastNode nodeIn)
	public virtual T Visit (Microsoft.OData.UriParser.SingleValueFunctionCallNode nodeIn)
	public virtual T Visit (Microsoft.OData.UriParser.SingleValueOpenPropertyAccessNode nodeIn)
	public virtual T Visit (Microsoft.OData.UriParser.SingleValuePropertyAccessNode nodeIn)
	public virtual T Visit (Microsoft.OData.UriParser.UnaryOperatorNode nodeIn)
}

public abstract class Microsoft.OData.UriParser.QueryToken {
	public static readonly Microsoft.OData.UriParser.QueryToken[] EmptyTokens = Microsoft.OData.UriParser.QueryToken[]

	protected QueryToken ()

	Microsoft.OData.UriParser.QueryTokenKind Kind  { public abstract get; }

	public abstract T Accept (ISyntacticTreeVisitor`1 visitor)
}

public abstract class Microsoft.OData.UriParser.RangeVariable {
	protected RangeVariable ()

	int Kind  { public abstract get; }
	string Name  { public abstract get; }
	Microsoft.OData.Edm.IEdmTypeReference TypeReference  { public abstract get; }
}

public abstract class Microsoft.OData.UriParser.SelectItem {
	protected SelectItem ()

	public abstract void HandleWith (Microsoft.OData.UriParser.SelectItemHandler handler)
	public abstract T TranslateWith (SelectItemTranslator`1 translator)
}

public abstract class Microsoft.OData.UriParser.SelectItemHandler {
	protected SelectItemHandler ()

	public virtual void Handle (Microsoft.OData.UriParser.ExpandedNavigationSelectItem item)
	public virtual void Handle (Microsoft.OData.UriParser.ExpandedReferenceSelectItem item)
	public virtual void Handle (Microsoft.OData.UriParser.NamespaceQualifiedWildcardSelectItem item)
	public virtual void Handle (Microsoft.OData.UriParser.PathSelectItem item)
	public virtual void Handle (Microsoft.OData.UriParser.WildcardSelectItem item)
}

public abstract class Microsoft.OData.UriParser.SelectItemTranslator`1 {
	protected SelectItemTranslator`1 ()

	public virtual T Translate (Microsoft.OData.UriParser.ExpandedNavigationSelectItem item)
	public virtual T Translate (Microsoft.OData.UriParser.ExpandedReferenceSelectItem item)
	public virtual T Translate (Microsoft.OData.UriParser.NamespaceQualifiedWildcardSelectItem item)
	public virtual T Translate (Microsoft.OData.UriParser.PathSelectItem item)
	public virtual T Translate (Microsoft.OData.UriParser.WildcardSelectItem item)
}

public abstract class Microsoft.OData.UriParser.SingleEntityNode : Microsoft.OData.UriParser.SingleResourceNode {
	protected SingleEntityNode ()

	Microsoft.OData.Edm.IEdmEntityTypeReference EntityTypeReference  { public abstract get; }
}

public abstract class Microsoft.OData.UriParser.SingleResourceNode : Microsoft.OData.UriParser.SingleValueNode {
	protected SingleResourceNode ()

	Microsoft.OData.Edm.IEdmNavigationSource NavigationSource  { public abstract get; }
	Microsoft.OData.Edm.IEdmStructuredTypeReference StructuredTypeReference  { public abstract get; }
}

public abstract class Microsoft.OData.UriParser.SingleValueNode : Microsoft.OData.UriParser.QueryNode {
	protected SingleValueNode ()

	Microsoft.OData.UriParser.QueryNodeKind Kind  { public virtual get; }
	Microsoft.OData.Edm.IEdmTypeReference TypeReference  { public abstract get; }
}

public sealed class Microsoft.OData.UriParser.CustomUriFunctions {
	public static void AddCustomUriFunction (string functionName, Microsoft.OData.UriParser.FunctionSignatureWithReturnType functionSignature)
	public static bool RemoveCustomUriFunction (string functionName)
	public static bool RemoveCustomUriFunction (string functionName, Microsoft.OData.UriParser.FunctionSignatureWithReturnType functionSignature)
}

public sealed class Microsoft.OData.UriParser.CustomUriLiteralPrefixes {
	public static void AddCustomLiteralPrefix (string literalPrefix, Microsoft.OData.Edm.IEdmTypeReference literalEdmTypeReference)
	public static bool RemoveCustomLiteralPrefix (string literalPrefix)
}

public sealed class Microsoft.OData.UriParser.RangeVariableKind {
	public static int NonResource = 1
	public static int Resource = 0
}

public class Microsoft.OData.UriParser.CollectionComplexNode : Microsoft.OData.UriParser.CollectionResourceNode {
	public CollectionComplexNode (Microsoft.OData.UriParser.SingleResourceNode source, Microsoft.OData.Edm.IEdmProperty property)

	Microsoft.OData.Edm.IEdmCollectionTypeReference CollectionType  { public virtual get; }
	Microsoft.OData.Edm.IEdmStructuredTypeReference ItemStructuredType  { public virtual get; }
	Microsoft.OData.Edm.IEdmTypeReference ItemType  { public virtual get; }
	Microsoft.OData.Edm.IEdmNavigationSource NavigationSource  { public virtual get; }
	Microsoft.OData.Edm.IEdmProperty Property  { public get; }
	Microsoft.OData.UriParser.SingleResourceNode Source  { public get; }

	public virtual T Accept (QueryNodeVisitor`1 visitor)
}

public class Microsoft.OData.UriParser.ExpandedReferenceSelectItem : Microsoft.OData.UriParser.SelectItem {
	public ExpandedReferenceSelectItem (Microsoft.OData.UriParser.ODataExpandPath pathToNavigationProperty, Microsoft.OData.Edm.IEdmNavigationSource navigationSource)
	public ExpandedReferenceSelectItem (Microsoft.OData.UriParser.ODataExpandPath pathToNavigationProperty, Microsoft.OData.Edm.IEdmNavigationSource navigationSource, Microsoft.OData.UriParser.FilterClause filterOption, Microsoft.OData.UriParser.OrderByClause orderByOption, System.Nullable`1[[System.Int64]] topOption, System.Nullable`1[[System.Int64]] skipOption, System.Nullable`1[[System.Boolean]] countOption, Microsoft.OData.UriParser.SearchClause searchOption)
	public ExpandedReferenceSelectItem (Microsoft.OData.UriParser.ODataExpandPath pathToNavigationProperty, Microsoft.OData.Edm.IEdmNavigationSource navigationSource, Microsoft.OData.UriParser.FilterClause filterOption, Microsoft.OData.UriParser.OrderByClause orderByOption, System.Nullable`1[[System.Int64]] topOption, System.Nullable`1[[System.Int64]] skipOption, System.Nullable`1[[System.Boolean]] countOption, Microsoft.OData.UriParser.SearchClause searchOption, Microsoft.OData.UriParser.ComputeClause computeOption)

	Microsoft.OData.UriParser.ComputeClause ComputeOption  { public get; }
	System.Nullable`1[[System.Boolean]] CountOption  { public get; }
	Microsoft.OData.UriParser.FilterClause FilterOption  { public get; }
	Microsoft.OData.Edm.IEdmNavigationSource NavigationSource  { public get; }
	Microsoft.OData.UriParser.OrderByClause OrderByOption  { public get; }
	Microsoft.OData.UriParser.ODataExpandPath PathToNavigationProperty  { public get; }
	Microsoft.OData.UriParser.SearchClause SearchOption  { public get; }
	System.Nullable`1[[System.Int64]] SkipOption  { public get; }
	System.Nullable`1[[System.Int64]] TopOption  { public get; }

	public virtual void HandleWith (Microsoft.OData.UriParser.SelectItemHandler handler)
	public virtual T TranslateWith (SelectItemTranslator`1 translator)
}

public class Microsoft.OData.UriParser.NamedFunctionParameterNode : Microsoft.OData.UriParser.QueryNode {
	public NamedFunctionParameterNode (string name, Microsoft.OData.UriParser.QueryNode value)

	Microsoft.OData.UriParser.QueryNodeKind Kind  { public virtual get; }
	string Name  { public get; }
	Microsoft.OData.UriParser.QueryNode Value  { public get; }

	public virtual T Accept (QueryNodeVisitor`1 visitor)
}

public class Microsoft.OData.UriParser.ODataExpandPath : Microsoft.OData.UriParser.ODataPath, IEnumerable, IEnumerable`1 {
	public ODataExpandPath (Microsoft.OData.UriParser.ODataPathSegment[] segments)
	public ODataExpandPath (System.Collections.Generic.IEnumerable`1[[Microsoft.OData.UriParser.ODataPathSegment]] segments)
}

public class Microsoft.OData.UriParser.ODataPath : IEnumerable, IEnumerable`1 {
	public ODataPath (Microsoft.OData.UriParser.ODataPathSegment[] segments)
	public ODataPath (System.Collections.Generic.IEnumerable`1[[Microsoft.OData.UriParser.ODataPathSegment]] segments)

	int Count  { public get; }
	Microsoft.OData.UriParser.ODataPathSegment FirstSegment  { public get; }
	Microsoft.OData.UriParser.ODataPathSegment LastSegment  { public get; }

	public virtual System.Collections.Generic.IEnumerator`1[[Microsoft.OData.UriParser.ODataPathSegment]] GetEnumerator ()
	System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
	public IEnumerable`1 WalkWith (PathSegmentTranslator`1 translator)
	public void WalkWith (Microsoft.OData.UriParser.PathSegmentHandler handler)
}

public class Microsoft.OData.UriParser.ODataQueryOptionParser {
	public ODataQueryOptionParser (Microsoft.OData.Edm.IEdmModel model, Microsoft.OData.UriParser.ODataPath odataPath, System.Collections.Generic.IDictionary`2[[System.String],[System.String]] queryOptions)
	public ODataQueryOptionParser (Microsoft.OData.Edm.IEdmModel model, Microsoft.OData.Edm.IEdmType targetEdmType, Microsoft.OData.Edm.IEdmNavigationSource targetNavigationSource, System.Collections.Generic.IDictionary`2[[System.String],[System.String]] queryOptions)
	public ODataQueryOptionParser (Microsoft.OData.Edm.IEdmModel model, Microsoft.OData.UriParser.ODataPath odataPath, System.Collections.Generic.IDictionary`2[[System.String],[System.String]] queryOptions, System.IServiceProvider container)
	public ODataQueryOptionParser (Microsoft.OData.Edm.IEdmModel model, Microsoft.OData.Edm.IEdmType targetEdmType, Microsoft.OData.Edm.IEdmNavigationSource targetNavigationSource, System.Collections.Generic.IDictionary`2[[System.String],[System.String]] queryOptions, System.IServiceProvider container)

	System.Collections.Generic.IDictionary`2[[System.String],[Microsoft.OData.UriParser.SingleValueNode]] ParameterAliasNodes  { public get; }
	Microsoft.OData.UriParser.ODataUriResolver Resolver  { public get; public set; }
	Microsoft.OData.UriParser.ODataUriParserSettings Settings  { public get; }

	public Microsoft.OData.UriParser.Aggregation.ApplyClause ParseApply ()
	public Microsoft.OData.UriParser.ComputeClause ParseCompute ()
	public System.Nullable`1[[System.Boolean]] ParseCount ()
	public string ParseDeltaToken ()
	public Microsoft.OData.UriParser.FilterClause ParseFilter ()
	public Microsoft.OData.UriParser.OrderByClause ParseOrderBy ()
	public Microsoft.OData.UriParser.SearchClause ParseSearch ()
	public Microsoft.OData.UriParser.SelectExpandClause ParseSelectAndExpand ()
	public System.Nullable`1[[System.Int64]] ParseSkip ()
	public string ParseSkipToken ()
	public System.Nullable`1[[System.Int64]] ParseTop ()
}

public class Microsoft.OData.UriParser.ODataSelectPath : Microsoft.OData.UriParser.ODataPath, IEnumerable, IEnumerable`1 {
	public ODataSelectPath (Microsoft.OData.UriParser.ODataPathSegment[] segments)
	public ODataSelectPath (System.Collections.Generic.IEnumerable`1[[Microsoft.OData.UriParser.ODataPathSegment]] segments)
}

public class Microsoft.OData.UriParser.ODataUnresolvedFunctionParameterAlias {
	public ODataUnresolvedFunctionParameterAlias (string alias, Microsoft.OData.Edm.IEdmTypeReference type)

	string Alias  { public get; }
	Microsoft.OData.Edm.IEdmTypeReference Type  { public get; }
}

public class Microsoft.OData.UriParser.ODataUriResolver {
	public ODataUriResolver ()

	bool EnableCaseInsensitive  { public virtual get; public virtual set; }
	bool EnableNoDollarQueryOptions  { public virtual get; public virtual set; }
	Microsoft.OData.UriParser.TypeFacetsPromotionRules TypeFacetsPromotionRules  { public get; public set; }

	public virtual void PromoteBinaryOperandTypes (Microsoft.OData.UriParser.BinaryOperatorKind binaryOperatorKind, Microsoft.OData.UriParser.SingleValueNode& leftNode, Microsoft.OData.UriParser.SingleValueNode& rightNode, out Microsoft.OData.Edm.IEdmTypeReference& typeReference)
	public virtual System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmOperation]] ResolveBoundOperations (Microsoft.OData.Edm.IEdmModel model, string identifier, Microsoft.OData.Edm.IEdmType bindingType)
	public virtual System.Collections.Generic.IEnumerable`1[[System.Collections.Generic.KeyValuePair`2[[System.String],[System.Object]]]] ResolveKeys (Microsoft.OData.Edm.IEdmEntityType type, System.Collections.Generic.IDictionary`2[[System.String],[System.String]] namedValues, System.Func`3[[Microsoft.OData.Edm.IEdmTypeReference],[System.String],[System.Object]] convertFunc)
	public virtual System.Collections.Generic.IEnumerable`1[[System.Collections.Generic.KeyValuePair`2[[System.String],[System.Object]]]] ResolveKeys (Microsoft.OData.Edm.IEdmEntityType type, System.Collections.Generic.IList`1[[System.String]] positionalValues, System.Func`3[[Microsoft.OData.Edm.IEdmTypeReference],[System.String],[System.Object]] convertFunc)
	public virtual Microsoft.OData.Edm.IEdmNavigationSource ResolveNavigationSource (Microsoft.OData.Edm.IEdmModel model, string identifier)
	public virtual System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmOperationImport]] ResolveOperationImports (Microsoft.OData.Edm.IEdmModel model, string identifier)
	public virtual System.Collections.Generic.IDictionary`2[[Microsoft.OData.Edm.IEdmOperationParameter],[Microsoft.OData.UriParser.SingleValueNode]] ResolveOperationParameters (Microsoft.OData.Edm.IEdmOperation operation, System.Collections.Generic.IDictionary`2[[System.String],[Microsoft.OData.UriParser.SingleValueNode]] input)
	public virtual Microsoft.OData.Edm.IEdmProperty ResolveProperty (Microsoft.OData.Edm.IEdmStructuredType type, string propertyName)
	public virtual Microsoft.OData.Edm.Vocabularies.IEdmTerm ResolveTerm (Microsoft.OData.Edm.IEdmModel model, string termName)
	public virtual Microsoft.OData.Edm.IEdmSchemaType ResolveType (Microsoft.OData.Edm.IEdmModel model, string typeName)
	public virtual System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmOperation]] ResolveUnboundOperations (Microsoft.OData.Edm.IEdmModel model, string identifier)
}

public class Microsoft.OData.UriParser.ParameterAliasNode : Microsoft.OData.UriParser.SingleValueNode {
	public ParameterAliasNode (string alias, Microsoft.OData.Edm.IEdmTypeReference typeReference)

	string Alias  { public get; }
	Microsoft.OData.Edm.IEdmTypeReference TypeReference  { public virtual get; }

	public virtual T Accept (QueryNodeVisitor`1 visitor)
}

public class Microsoft.OData.UriParser.SingleComplexNode : Microsoft.OData.UriParser.SingleResourceNode {
	public SingleComplexNode (Microsoft.OData.UriParser.SingleResourceNode source, Microsoft.OData.Edm.IEdmProperty property)

	Microsoft.OData.Edm.IEdmNavigationSource NavigationSource  { public virtual get; }
	Microsoft.OData.Edm.IEdmProperty Property  { public get; }
	Microsoft.OData.UriParser.SingleResourceNode Source  { public get; }
	Microsoft.OData.Edm.IEdmStructuredTypeReference StructuredTypeReference  { public virtual get; }
	Microsoft.OData.Edm.IEdmTypeReference TypeReference  { public virtual get; }

	public virtual T Accept (QueryNodeVisitor`1 visitor)
}

public class Microsoft.OData.UriParser.TypeFacetsPromotionRules {
	public TypeFacetsPromotionRules ()

	public virtual System.Nullable`1[[System.Int32]] GetPromotedPrecision (System.Nullable`1[[System.Int32]] left, System.Nullable`1[[System.Int32]] right)
	public virtual System.Nullable`1[[System.Int32]] GetPromotedScale (System.Nullable`1[[System.Int32]] left, System.Nullable`1[[System.Int32]] right)
}

public class Microsoft.OData.UriParser.UnqualifiedODataUriResolver : Microsoft.OData.UriParser.ODataUriResolver {
	public UnqualifiedODataUriResolver ()

	public virtual System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmOperation]] ResolveBoundOperations (Microsoft.OData.Edm.IEdmModel model, string identifier, Microsoft.OData.Edm.IEdmType bindingType)
	public virtual System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmOperation]] ResolveUnboundOperations (Microsoft.OData.Edm.IEdmModel model, string identifier)
}

public class Microsoft.OData.UriParser.UriPathParser {
	public UriPathParser (Microsoft.OData.UriParser.ODataUriParserSettings settings)

	public virtual System.Collections.Generic.ICollection`1[[System.String]] ParsePathIntoSegments (System.Uri fullUri, System.Uri serviceBaseUri)
}

public sealed class Microsoft.OData.UriParser.AllNode : Microsoft.OData.UriParser.LambdaNode {
	public AllNode (System.Collections.ObjectModel.Collection`1[[Microsoft.OData.UriParser.RangeVariable]] rangeVariables)
	public AllNode (System.Collections.ObjectModel.Collection`1[[Microsoft.OData.UriParser.RangeVariable]] rangeVariables, Microsoft.OData.UriParser.RangeVariable currentRangeVariable)

	Microsoft.OData.Edm.IEdmTypeReference TypeReference  { public virtual get; }

	public virtual T Accept (QueryNodeVisitor`1 visitor)
}

public sealed class Microsoft.OData.UriParser.AllToken : Microsoft.OData.UriParser.LambdaToken {
	public AllToken (Microsoft.OData.UriParser.QueryToken expression, string parameter, Microsoft.OData.UriParser.QueryToken parent)

	Microsoft.OData.UriParser.QueryTokenKind Kind  { public virtual get; }

	public virtual T Accept (ISyntacticTreeVisitor`1 visitor)
}

public sealed class Microsoft.OData.UriParser.AlternateKeysODataUriResolver : Microsoft.OData.UriParser.ODataUriResolver {
	public AlternateKeysODataUriResolver (Microsoft.OData.Edm.IEdmModel model)

	public virtual System.Collections.Generic.IEnumerable`1[[System.Collections.Generic.KeyValuePair`2[[System.String],[System.Object]]]] ResolveKeys (Microsoft.OData.Edm.IEdmEntityType type, System.Collections.Generic.IDictionary`2[[System.String],[System.String]] namedValues, System.Func`3[[Microsoft.OData.Edm.IEdmTypeReference],[System.String],[System.Object]] convertFunc)
}

public sealed class Microsoft.OData.UriParser.AnnotationSegment : Microsoft.OData.UriParser.ODataPathSegment {
	public AnnotationSegment (Microsoft.OData.Edm.Vocabularies.IEdmTerm term)

	Microsoft.OData.Edm.IEdmType EdmType  { public virtual get; }
	Microsoft.OData.Edm.Vocabularies.IEdmTerm Term  { public get; }

	public virtual void HandleWith (Microsoft.OData.UriParser.PathSegmentHandler handler)
	public virtual T TranslateWith (PathSegmentTranslator`1 translator)
}

public sealed class Microsoft.OData.UriParser.AnyNode : Microsoft.OData.UriParser.LambdaNode {
	public AnyNode (System.Collections.ObjectModel.Collection`1[[Microsoft.OData.UriParser.RangeVariable]] parameters)
	public AnyNode (System.Collections.ObjectModel.Collection`1[[Microsoft.OData.UriParser.RangeVariable]] parameters, Microsoft.OData.UriParser.RangeVariable currentRangeVariable)

	Microsoft.OData.Edm.IEdmTypeReference TypeReference  { public virtual get; }

	public virtual T Accept (QueryNodeVisitor`1 visitor)
}

public sealed class Microsoft.OData.UriParser.AnyToken : Microsoft.OData.UriParser.LambdaToken {
	public AnyToken (Microsoft.OData.UriParser.QueryToken expression, string parameter, Microsoft.OData.UriParser.QueryToken parent)

	Microsoft.OData.UriParser.QueryTokenKind Kind  { public virtual get; }

	public virtual T Accept (ISyntacticTreeVisitor`1 visitor)
}

public sealed class Microsoft.OData.UriParser.BatchReferenceSegment : Microsoft.OData.UriParser.ODataPathSegment {
	public BatchReferenceSegment (string contentId, Microsoft.OData.Edm.IEdmType edmType, Microsoft.OData.Edm.IEdmEntitySetBase entitySet)

	string ContentId  { public get; }
	Microsoft.OData.Edm.IEdmType EdmType  { public virtual get; }
	Microsoft.OData.Edm.IEdmEntitySetBase EntitySet  { public get; }

	public virtual void HandleWith (Microsoft.OData.UriParser.PathSegmentHandler handler)
	public virtual T TranslateWith (PathSegmentTranslator`1 translator)
}

public sealed class Microsoft.OData.UriParser.BatchSegment : Microsoft.OData.UriParser.ODataPathSegment {
	public static readonly Microsoft.OData.UriParser.BatchSegment Instance = Microsoft.OData.UriParser.BatchSegment

	Microsoft.OData.Edm.IEdmType EdmType  { public virtual get; }

	public virtual void HandleWith (Microsoft.OData.UriParser.PathSegmentHandler handler)
	public virtual T TranslateWith (PathSegmentTranslator`1 translator)
}

public sealed class Microsoft.OData.UriParser.BinaryOperatorNode : Microsoft.OData.UriParser.SingleValueNode {
	public BinaryOperatorNode (Microsoft.OData.UriParser.BinaryOperatorKind operatorKind, Microsoft.OData.UriParser.SingleValueNode left, Microsoft.OData.UriParser.SingleValueNode right)

	Microsoft.OData.UriParser.SingleValueNode Left  { public get; }
	Microsoft.OData.UriParser.BinaryOperatorKind OperatorKind  { public get; }
	Microsoft.OData.UriParser.SingleValueNode Right  { public get; }
	Microsoft.OData.Edm.IEdmTypeReference TypeReference  { public virtual get; }

	public virtual T Accept (QueryNodeVisitor`1 visitor)
}

public sealed class Microsoft.OData.UriParser.BinaryOperatorToken : Microsoft.OData.UriParser.QueryToken {
	public BinaryOperatorToken (Microsoft.OData.UriParser.BinaryOperatorKind operatorKind, Microsoft.OData.UriParser.QueryToken left, Microsoft.OData.UriParser.QueryToken right)

	Microsoft.OData.UriParser.QueryTokenKind Kind  { public virtual get; }
	Microsoft.OData.UriParser.QueryToken Left  { public get; }
	Microsoft.OData.UriParser.BinaryOperatorKind OperatorKind  { public get; }
	Microsoft.OData.UriParser.QueryToken Right  { public get; }

	public virtual T Accept (ISyntacticTreeVisitor`1 visitor)
}

public sealed class Microsoft.OData.UriParser.CollectionFunctionCallNode : Microsoft.OData.UriParser.CollectionNode {
	public CollectionFunctionCallNode (string name, System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmFunction]] functions, System.Collections.Generic.IEnumerable`1[[Microsoft.OData.UriParser.QueryNode]] parameters, Microsoft.OData.Edm.IEdmCollectionTypeReference returnedCollectionType, Microsoft.OData.UriParser.QueryNode source)

	Microsoft.OData.Edm.IEdmCollectionTypeReference CollectionType  { public virtual get; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmFunction]] Functions  { public get; }
	Microsoft.OData.Edm.IEdmTypeReference ItemType  { public virtual get; }
	string Name  { public get; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.UriParser.QueryNode]] Parameters  { public get; }
	Microsoft.OData.UriParser.QueryNode Source  { public get; }

	public virtual T Accept (QueryNodeVisitor`1 visitor)
}

public sealed class Microsoft.OData.UriParser.CollectionNavigationNode : Microsoft.OData.UriParser.CollectionResourceNode {
	public CollectionNavigationNode (Microsoft.OData.UriParser.SingleResourceNode source, Microsoft.OData.Edm.IEdmNavigationProperty navigationProperty, Microsoft.OData.Edm.IEdmPathExpression bindingPath)

	Microsoft.OData.Edm.IEdmPathExpression BindingPath  { public get; }
	Microsoft.OData.Edm.IEdmCollectionTypeReference CollectionType  { public virtual get; }
	Microsoft.OData.Edm.IEdmEntityTypeReference EntityItemType  { public get; }
	Microsoft.OData.Edm.IEdmStructuredTypeReference ItemStructuredType  { public virtual get; }
	Microsoft.OData.Edm.IEdmTypeReference ItemType  { public virtual get; }
	Microsoft.OData.Edm.IEdmNavigationProperty NavigationProperty  { public get; }
	Microsoft.OData.Edm.IEdmNavigationSource NavigationSource  { public virtual get; }
	Microsoft.OData.UriParser.SingleResourceNode Source  { public get; }
	Microsoft.OData.Edm.EdmMultiplicity TargetMultiplicity  { public get; }

	public virtual T Accept (QueryNodeVisitor`1 visitor)
}

public sealed class Microsoft.OData.UriParser.CollectionOpenPropertyAccessNode : Microsoft.OData.UriParser.CollectionNode {
	public CollectionOpenPropertyAccessNode (Microsoft.OData.UriParser.SingleValueNode source, string openPropertyName)

	Microsoft.OData.Edm.IEdmCollectionTypeReference CollectionType  { public virtual get; }
	Microsoft.OData.Edm.IEdmTypeReference ItemType  { public virtual get; }
	string Name  { public get; }
	Microsoft.OData.UriParser.SingleValueNode Source  { public get; }

	public virtual T Accept (QueryNodeVisitor`1 visitor)
}

public sealed class Microsoft.OData.UriParser.CollectionPropertyAccessNode : Microsoft.OData.UriParser.CollectionNode {
	public CollectionPropertyAccessNode (Microsoft.OData.UriParser.SingleValueNode source, Microsoft.OData.Edm.IEdmProperty property)

	Microsoft.OData.Edm.IEdmCollectionTypeReference CollectionType  { public virtual get; }
	Microsoft.OData.Edm.IEdmTypeReference ItemType  { public virtual get; }
	Microsoft.OData.Edm.IEdmProperty Property  { public get; }
	Microsoft.OData.UriParser.SingleValueNode Source  { public get; }

	public virtual T Accept (QueryNodeVisitor`1 visitor)
}

public sealed class Microsoft.OData.UriParser.CollectionResourceCastNode : Microsoft.OData.UriParser.CollectionResourceNode {
	public CollectionResourceCastNode (Microsoft.OData.UriParser.CollectionResourceNode source, Microsoft.OData.Edm.IEdmStructuredType structuredType)

	Microsoft.OData.Edm.IEdmCollectionTypeReference CollectionType  { public virtual get; }
	Microsoft.OData.Edm.IEdmStructuredTypeReference ItemStructuredType  { public virtual get; }
	Microsoft.OData.Edm.IEdmTypeReference ItemType  { public virtual get; }
	Microsoft.OData.Edm.IEdmNavigationSource NavigationSource  { public virtual get; }
	Microsoft.OData.UriParser.CollectionResourceNode Source  { public get; }

	public virtual T Accept (QueryNodeVisitor`1 visitor)
}

public sealed class Microsoft.OData.UriParser.CollectionResourceFunctionCallNode : Microsoft.OData.UriParser.CollectionResourceNode {
	public CollectionResourceFunctionCallNode (string name, System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmFunction]] functions, System.Collections.Generic.IEnumerable`1[[Microsoft.OData.UriParser.QueryNode]] parameters, Microsoft.OData.Edm.IEdmCollectionTypeReference returnedCollectionTypeReference, Microsoft.OData.Edm.IEdmEntitySetBase navigationSource, Microsoft.OData.UriParser.QueryNode source)

	Microsoft.OData.Edm.IEdmCollectionTypeReference CollectionType  { public virtual get; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmFunction]] Functions  { public get; }
	Microsoft.OData.Edm.IEdmStructuredTypeReference ItemStructuredType  { public virtual get; }
	Microsoft.OData.Edm.IEdmTypeReference ItemType  { public virtual get; }
	string Name  { public get; }
	Microsoft.OData.Edm.IEdmNavigationSource NavigationSource  { public virtual get; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.UriParser.QueryNode]] Parameters  { public get; }
	Microsoft.OData.UriParser.QueryNode Source  { public get; }

	public virtual T Accept (QueryNodeVisitor`1 visitor)
}

public sealed class Microsoft.OData.UriParser.ComputeClause {
	public ComputeClause (System.Collections.Generic.IEnumerable`1[[Microsoft.OData.UriParser.ComputeExpression]] computedItems)

	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.UriParser.ComputeExpression]] ComputedItems  { public get; }
}

public sealed class Microsoft.OData.UriParser.ComputeExpression {
	public ComputeExpression (Microsoft.OData.UriParser.SingleValueNode expression, string alias, Microsoft.OData.Edm.IEdmTypeReference typeReference)

	string Alias  { public get; }
	Microsoft.OData.UriParser.SingleValueNode Expression  { public get; }
	Microsoft.OData.Edm.IEdmTypeReference TypeReference  { public get; }
}

public sealed class Microsoft.OData.UriParser.ComputeExpressionToken : Microsoft.OData.UriParser.QueryToken {
	public ComputeExpressionToken (Microsoft.OData.UriParser.QueryToken expression, string alias)

	string Alias  { public get; }
	Microsoft.OData.UriParser.QueryToken Expression  { public get; }
	Microsoft.OData.UriParser.QueryTokenKind Kind  { public virtual get; }

	public virtual T Accept (ISyntacticTreeVisitor`1 visitor)
}

public sealed class Microsoft.OData.UriParser.ComputeToken : Microsoft.OData.UriParser.Aggregation.ApplyTransformationToken {
	public ComputeToken (System.Collections.Generic.IEnumerable`1[[Microsoft.OData.UriParser.ComputeExpressionToken]] expressions)

	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.UriParser.ComputeExpressionToken]] Expressions  { public get; }
	Microsoft.OData.UriParser.QueryTokenKind Kind  { public virtual get; }

	public virtual T Accept (ISyntacticTreeVisitor`1 visitor)
}

public sealed class Microsoft.OData.UriParser.ConstantNode : Microsoft.OData.UriParser.SingleValueNode {
	public ConstantNode (object constantValue)
	public ConstantNode (object constantValue, string literalText)
	public ConstantNode (object constantValue, string literalText, Microsoft.OData.Edm.IEdmTypeReference typeReference)

	string LiteralText  { public get; }
	Microsoft.OData.Edm.IEdmTypeReference TypeReference  { public virtual get; }
	object Value  { public get; }

	public virtual T Accept (QueryNodeVisitor`1 visitor)
}

public sealed class Microsoft.OData.UriParser.ConvertNode : Microsoft.OData.UriParser.SingleValueNode {
	public ConvertNode (Microsoft.OData.UriParser.SingleValueNode source, Microsoft.OData.Edm.IEdmTypeReference typeReference)

	Microsoft.OData.UriParser.SingleValueNode Source  { public get; }
	Microsoft.OData.Edm.IEdmTypeReference TypeReference  { public virtual get; }

	public virtual T Accept (QueryNodeVisitor`1 visitor)
}

public sealed class Microsoft.OData.UriParser.CountNode : Microsoft.OData.UriParser.SingleValueNode {
	public CountNode (Microsoft.OData.UriParser.CollectionNode source)

	Microsoft.OData.UriParser.CollectionNode Source  { public get; }
	Microsoft.OData.Edm.IEdmTypeReference TypeReference  { public virtual get; }

	public virtual T Accept (QueryNodeVisitor`1 visitor)
}

public sealed class Microsoft.OData.UriParser.CountSegment : Microsoft.OData.UriParser.ODataPathSegment {
	public static readonly Microsoft.OData.UriParser.CountSegment Instance = Microsoft.OData.UriParser.CountSegment

	Microsoft.OData.Edm.IEdmType EdmType  { public virtual get; }

	public virtual void HandleWith (Microsoft.OData.UriParser.PathSegmentHandler handler)
	public virtual T TranslateWith (PathSegmentTranslator`1 translator)
}

public sealed class Microsoft.OData.UriParser.CountVirtualPropertyNode : Microsoft.OData.UriParser.SingleValueNode {
	public CountVirtualPropertyNode ()

	Microsoft.OData.UriParser.QueryNodeKind Kind  { public virtual get; }
	Microsoft.OData.Edm.IEdmTypeReference TypeReference  { public virtual get; }
}

public sealed class Microsoft.OData.UriParser.CustomQueryOptionToken : Microsoft.OData.UriParser.QueryToken {
	public CustomQueryOptionToken (string name, string value)

	Microsoft.OData.UriParser.QueryTokenKind Kind  { public virtual get; }
	string Name  { public get; }
	string Value  { public get; }

	public virtual T Accept (ISyntacticTreeVisitor`1 visitor)
}

public sealed class Microsoft.OData.UriParser.CustomUriLiteralParsers : IUriLiteralParser {
	public static void AddCustomUriLiteralParser (Microsoft.OData.UriParser.IUriLiteralParser customUriLiteralParser)
	public static void AddCustomUriLiteralParser (Microsoft.OData.Edm.IEdmTypeReference edmTypeReference, Microsoft.OData.UriParser.IUriLiteralParser customUriLiteralParser)
	public virtual object ParseUriStringToType (string text, Microsoft.OData.Edm.IEdmTypeReference targetType, out Microsoft.OData.UriParser.UriLiteralParsingException& parsingException)
	public static bool RemoveCustomUriLiteralParser (Microsoft.OData.UriParser.IUriLiteralParser customUriLiteralParser)
}

public sealed class Microsoft.OData.UriParser.DottedIdentifierToken : Microsoft.OData.UriParser.PathToken {
	public DottedIdentifierToken (string identifier, Microsoft.OData.UriParser.QueryToken nextToken)

	string Identifier  { public virtual get; }
	Microsoft.OData.UriParser.QueryTokenKind Kind  { public virtual get; }
	Microsoft.OData.UriParser.QueryToken NextToken  { public virtual get; public virtual set; }

	public virtual T Accept (ISyntacticTreeVisitor`1 visitor)
}

public sealed class Microsoft.OData.UriParser.DynamicPathSegment : Microsoft.OData.UriParser.ODataPathSegment {
	public DynamicPathSegment (string identifier)
	public DynamicPathSegment (string identifier, Microsoft.OData.Edm.IEdmType edmType, Microsoft.OData.Edm.IEdmNavigationSource navigationSource, bool singleResult)

	Microsoft.OData.Edm.IEdmType EdmType  { public virtual get; }

	public virtual void HandleWith (Microsoft.OData.UriParser.PathSegmentHandler handler)
	public virtual T TranslateWith (PathSegmentTranslator`1 translator)
}

public sealed class Microsoft.OData.UriParser.EndPathToken : Microsoft.OData.UriParser.PathToken {
	public EndPathToken (string identifier, Microsoft.OData.UriParser.QueryToken nextToken)

	string Identifier  { public virtual get; }
	Microsoft.OData.UriParser.QueryTokenKind Kind  { public virtual get; }
	Microsoft.OData.UriParser.QueryToken NextToken  { public virtual get; public virtual set; }

	public virtual T Accept (ISyntacticTreeVisitor`1 visitor)
}

public sealed class Microsoft.OData.UriParser.EntityIdSegment {
	System.Uri Id  { public get; }
}

public sealed class Microsoft.OData.UriParser.EntitySetSegment : Microsoft.OData.UriParser.ODataPathSegment {
	public EntitySetSegment (Microsoft.OData.Edm.IEdmEntitySet entitySet)

	Microsoft.OData.Edm.IEdmType EdmType  { public virtual get; }
	Microsoft.OData.Edm.IEdmEntitySet EntitySet  { public get; }

	public virtual void HandleWith (Microsoft.OData.UriParser.PathSegmentHandler handler)
	public virtual T TranslateWith (PathSegmentTranslator`1 translator)
}

public sealed class Microsoft.OData.UriParser.ExpandedNavigationSelectItem : Microsoft.OData.UriParser.ExpandedReferenceSelectItem {
	public ExpandedNavigationSelectItem (Microsoft.OData.UriParser.ODataExpandPath pathToNavigationProperty, Microsoft.OData.Edm.IEdmNavigationSource navigationSource, Microsoft.OData.UriParser.SelectExpandClause selectExpandOption)
	public ExpandedNavigationSelectItem (Microsoft.OData.UriParser.ODataExpandPath pathToNavigationProperty, Microsoft.OData.Edm.IEdmNavigationSource navigationSource, Microsoft.OData.UriParser.SelectExpandClause selectAndExpand, Microsoft.OData.UriParser.FilterClause filterOption, Microsoft.OData.UriParser.OrderByClause orderByOption, System.Nullable`1[[System.Int64]] topOption, System.Nullable`1[[System.Int64]] skipOption, System.Nullable`1[[System.Boolean]] countOption, Microsoft.OData.UriParser.SearchClause searchOption, Microsoft.OData.UriParser.LevelsClause levelsOption)
	public ExpandedNavigationSelectItem (Microsoft.OData.UriParser.ODataExpandPath pathToNavigationProperty, Microsoft.OData.Edm.IEdmNavigationSource navigationSource, Microsoft.OData.UriParser.SelectExpandClause selectAndExpand, Microsoft.OData.UriParser.FilterClause filterOption, Microsoft.OData.UriParser.OrderByClause orderByOption, System.Nullable`1[[System.Int64]] topOption, System.Nullable`1[[System.Int64]] skipOption, System.Nullable`1[[System.Boolean]] countOption, Microsoft.OData.UriParser.SearchClause searchOption, Microsoft.OData.UriParser.LevelsClause levelsOption, Microsoft.OData.UriParser.ComputeClause computeOption)

	Microsoft.OData.UriParser.LevelsClause LevelsOption  { public get; }
	Microsoft.OData.UriParser.SelectExpandClause SelectAndExpand  { public get; }

	public virtual void HandleWith (Microsoft.OData.UriParser.SelectItemHandler handler)
	public virtual T TranslateWith (SelectItemTranslator`1 translator)
}

public sealed class Microsoft.OData.UriParser.ExpandTermToken : Microsoft.OData.UriParser.QueryToken {
	public ExpandTermToken (Microsoft.OData.UriParser.PathSegmentToken pathToNavigationProp)
	public ExpandTermToken (Microsoft.OData.UriParser.PathSegmentToken pathToNavigationProp, Microsoft.OData.UriParser.SelectToken selectOption, Microsoft.OData.UriParser.ExpandToken expandOption)
	public ExpandTermToken (Microsoft.OData.UriParser.PathSegmentToken pathToNavigationProp, Microsoft.OData.UriParser.QueryToken filterOption, System.Collections.Generic.IEnumerable`1[[Microsoft.OData.UriParser.OrderByToken]] orderByOptions, System.Nullable`1[[System.Int64]] topOption, System.Nullable`1[[System.Int64]] skipOption, System.Nullable`1[[System.Boolean]] countQueryOption, System.Nullable`1[[System.Int64]] levelsOption, Microsoft.OData.UriParser.QueryToken searchOption, Microsoft.OData.UriParser.SelectToken selectOption, Microsoft.OData.UriParser.ExpandToken expandOption)
	public ExpandTermToken (Microsoft.OData.UriParser.PathSegmentToken pathToNavigationProp, Microsoft.OData.UriParser.QueryToken filterOption, System.Collections.Generic.IEnumerable`1[[Microsoft.OData.UriParser.OrderByToken]] orderByOptions, System.Nullable`1[[System.Int64]] topOption, System.Nullable`1[[System.Int64]] skipOption, System.Nullable`1[[System.Boolean]] countQueryOption, System.Nullable`1[[System.Int64]] levelsOption, Microsoft.OData.UriParser.QueryToken searchOption, Microsoft.OData.UriParser.SelectToken selectOption, Microsoft.OData.UriParser.ExpandToken expandOption, Microsoft.OData.UriParser.ComputeToken computeOption)

	Microsoft.OData.UriParser.ComputeToken ComputeOption  { public get; }
	System.Nullable`1[[System.Boolean]] CountQueryOption  { public get; }
	Microsoft.OData.UriParser.ExpandToken ExpandOption  { public get; }
	Microsoft.OData.UriParser.QueryToken FilterOption  { public get; }
	Microsoft.OData.UriParser.QueryTokenKind Kind  { public virtual get; }
	System.Nullable`1[[System.Int64]] LevelsOption  { public get; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.UriParser.OrderByToken]] OrderByOptions  { public get; }
	Microsoft.OData.UriParser.PathSegmentToken PathToNavigationProp  { public get; }
	Microsoft.OData.UriParser.QueryToken SearchOption  { public get; }
	Microsoft.OData.UriParser.SelectToken SelectOption  { public get; }
	System.Nullable`1[[System.Int64]] SkipOption  { public get; }
	System.Nullable`1[[System.Int64]] TopOption  { public get; }

	public virtual T Accept (ISyntacticTreeVisitor`1 visitor)
}

public sealed class Microsoft.OData.UriParser.ExpandToken : Microsoft.OData.UriParser.QueryToken {
	public ExpandToken (System.Collections.Generic.IEnumerable`1[[Microsoft.OData.UriParser.ExpandTermToken]] expandTerms)

	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.UriParser.ExpandTermToken]] ExpandTerms  { public get; }
	Microsoft.OData.UriParser.QueryTokenKind Kind  { public virtual get; }

	public virtual T Accept (ISyntacticTreeVisitor`1 visitor)
}

public sealed class Microsoft.OData.UriParser.FilterClause {
	public FilterClause (Microsoft.OData.UriParser.SingleValueNode expression, Microsoft.OData.UriParser.RangeVariable rangeVariable)

	Microsoft.OData.UriParser.SingleValueNode Expression  { public get; }
	Microsoft.OData.Edm.IEdmTypeReference ItemType  { public get; }
	Microsoft.OData.UriParser.RangeVariable RangeVariable  { public get; }
}

public sealed class Microsoft.OData.UriParser.FunctionCallToken : Microsoft.OData.UriParser.QueryToken {
	public FunctionCallToken (string name, System.Collections.Generic.IEnumerable`1[[Microsoft.OData.UriParser.QueryToken]] argumentValues)
	public FunctionCallToken (string name, System.Collections.Generic.IEnumerable`1[[Microsoft.OData.UriParser.FunctionParameterToken]] arguments, Microsoft.OData.UriParser.QueryToken source)

	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.UriParser.FunctionParameterToken]] Arguments  { public get; }
	Microsoft.OData.UriParser.QueryTokenKind Kind  { public virtual get; }
	string Name  { public get; }
	Microsoft.OData.UriParser.QueryToken Source  { public get; }

	public virtual T Accept (ISyntacticTreeVisitor`1 visitor)
}

public sealed class Microsoft.OData.UriParser.FunctionParameterToken : Microsoft.OData.UriParser.QueryToken {
	public static Microsoft.OData.UriParser.FunctionParameterToken[] EmptyParameterList = Microsoft.OData.UriParser.FunctionParameterToken[]

	public FunctionParameterToken (string parameterName, Microsoft.OData.UriParser.QueryToken valueToken)

	Microsoft.OData.UriParser.QueryTokenKind Kind  { public virtual get; }
	string ParameterName  { public get; }
	Microsoft.OData.UriParser.QueryToken ValueToken  { public get; }

	public virtual T Accept (ISyntacticTreeVisitor`1 visitor)
}

public sealed class Microsoft.OData.UriParser.FunctionSignatureWithReturnType {
	public FunctionSignatureWithReturnType (Microsoft.OData.Edm.IEdmTypeReference returnType, Microsoft.OData.Edm.IEdmTypeReference[] argumentTypes)

	Microsoft.OData.Edm.IEdmTypeReference[] ArgumentTypes  { public get; }
	Microsoft.OData.Edm.IEdmTypeReference ReturnType  { public get; }
}

public sealed class Microsoft.OData.UriParser.InnerPathToken : Microsoft.OData.UriParser.PathToken {
	public InnerPathToken (string identifier, Microsoft.OData.UriParser.QueryToken nextToken, System.Collections.Generic.IEnumerable`1[[Microsoft.OData.UriParser.NamedValue]] namedValues)

	string Identifier  { public virtual get; }
	Microsoft.OData.UriParser.QueryTokenKind Kind  { public virtual get; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.UriParser.NamedValue]] NamedValues  { public get; }
	Microsoft.OData.UriParser.QueryToken NextToken  { public virtual get; public virtual set; }

	public virtual T Accept (ISyntacticTreeVisitor`1 visitor)
}

public sealed class Microsoft.OData.UriParser.KeySegment : Microsoft.OData.UriParser.ODataPathSegment {
	public KeySegment (System.Collections.Generic.IEnumerable`1[[System.Collections.Generic.KeyValuePair`2[[System.String],[System.Object]]]] keys, Microsoft.OData.Edm.IEdmEntityType edmType, Microsoft.OData.Edm.IEdmNavigationSource navigationSource)
	public KeySegment (Microsoft.OData.UriParser.ODataPathSegment previous, System.Collections.Generic.IEnumerable`1[[System.Collections.Generic.KeyValuePair`2[[System.String],[System.Object]]]] keys, Microsoft.OData.Edm.IEdmEntityType edmType, Microsoft.OData.Edm.IEdmNavigationSource navigationSource)

	Microsoft.OData.Edm.IEdmType EdmType  { public virtual get; }
	System.Collections.Generic.IEnumerable`1[[System.Collections.Generic.KeyValuePair`2[[System.String],[System.Object]]]] Keys  { public get; }
	Microsoft.OData.Edm.IEdmNavigationSource NavigationSource  { public get; }

	public virtual void HandleWith (Microsoft.OData.UriParser.PathSegmentHandler handler)
	public virtual T TranslateWith (PathSegmentTranslator`1 translator)
}

public sealed class Microsoft.OData.UriParser.LevelsClause {
	public LevelsClause (bool isMaxLevel, long level)

	bool IsMaxLevel  { public get; }
	long Level  { public get; }
}

public sealed class Microsoft.OData.UriParser.LiteralToken : Microsoft.OData.UriParser.QueryToken {
	public LiteralToken (object value)

	Microsoft.OData.UriParser.QueryTokenKind Kind  { public virtual get; }
	object Value  { public get; }

	public virtual T Accept (ISyntacticTreeVisitor`1 visitor)
}

public sealed class Microsoft.OData.UriParser.MetadataSegment : Microsoft.OData.UriParser.ODataPathSegment {
	public static readonly Microsoft.OData.UriParser.MetadataSegment Instance = Microsoft.OData.UriParser.MetadataSegment

	Microsoft.OData.Edm.IEdmType EdmType  { public virtual get; }

	public virtual void HandleWith (Microsoft.OData.UriParser.PathSegmentHandler handler)
	public virtual T TranslateWith (PathSegmentTranslator`1 translator)
}

public sealed class Microsoft.OData.UriParser.NamedValue {
	public NamedValue (string name, Microsoft.OData.UriParser.LiteralToken value)

	string Name  { public get; }
	Microsoft.OData.UriParser.LiteralToken Value  { public get; }
}

public sealed class Microsoft.OData.UriParser.NamespaceQualifiedWildcardSelectItem : Microsoft.OData.UriParser.SelectItem {
	public NamespaceQualifiedWildcardSelectItem (string namespaceName)

	string Namespace  { public get; }

	public virtual void HandleWith (Microsoft.OData.UriParser.SelectItemHandler handler)
	public virtual T TranslateWith (SelectItemTranslator`1 translator)
}

public sealed class Microsoft.OData.UriParser.NavigationPropertyLinkSegment : Microsoft.OData.UriParser.ODataPathSegment {
	public NavigationPropertyLinkSegment (Microsoft.OData.Edm.IEdmNavigationProperty navigationProperty, Microsoft.OData.Edm.IEdmNavigationSource navigationSource)

	Microsoft.OData.Edm.IEdmType EdmType  { public virtual get; }
	Microsoft.OData.Edm.IEdmNavigationProperty NavigationProperty  { public get; }
	Microsoft.OData.Edm.IEdmNavigationSource NavigationSource  { public get; }

	public virtual void HandleWith (Microsoft.OData.UriParser.PathSegmentHandler handler)
	public virtual T TranslateWith (PathSegmentTranslator`1 translator)
}

public sealed class Microsoft.OData.UriParser.NavigationPropertySegment : Microsoft.OData.UriParser.ODataPathSegment {
	public NavigationPropertySegment (Microsoft.OData.Edm.IEdmNavigationProperty navigationProperty, Microsoft.OData.Edm.IEdmNavigationSource navigationSource)

	Microsoft.OData.Edm.IEdmType EdmType  { public virtual get; }
	Microsoft.OData.Edm.IEdmNavigationProperty NavigationProperty  { public get; }
	Microsoft.OData.Edm.IEdmNavigationSource NavigationSource  { public get; }

	public virtual void HandleWith (Microsoft.OData.UriParser.PathSegmentHandler handler)
	public virtual T TranslateWith (PathSegmentTranslator`1 translator)
}

public sealed class Microsoft.OData.UriParser.NonResourceRangeVariable : Microsoft.OData.UriParser.RangeVariable {
	public NonResourceRangeVariable (string name, Microsoft.OData.Edm.IEdmTypeReference typeReference, Microsoft.OData.UriParser.CollectionNode collectionNode)

	Microsoft.OData.UriParser.CollectionNode CollectionNode  { public get; }
	int Kind  { public virtual get; }
	string Name  { public virtual get; }
	Microsoft.OData.Edm.IEdmTypeReference TypeReference  { public virtual get; }
}

public sealed class Microsoft.OData.UriParser.NonResourceRangeVariableReferenceNode : Microsoft.OData.UriParser.SingleValueNode {
	public NonResourceRangeVariableReferenceNode (string name, Microsoft.OData.UriParser.NonResourceRangeVariable rangeVariable)

	string Name  { public get; }
	Microsoft.OData.UriParser.NonResourceRangeVariable RangeVariable  { public get; }
	Microsoft.OData.Edm.IEdmTypeReference TypeReference  { public virtual get; }

	public virtual T Accept (QueryNodeVisitor`1 visitor)
}

public sealed class Microsoft.OData.UriParser.NonSystemToken : Microsoft.OData.UriParser.PathSegmentToken {
	public NonSystemToken (string identifier, System.Collections.Generic.IEnumerable`1[[Microsoft.OData.UriParser.NamedValue]] namedValues, Microsoft.OData.UriParser.PathSegmentToken nextToken)

	string Identifier  { public virtual get; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.UriParser.NamedValue]] NamedValues  { public get; }

	public virtual T Accept (IPathSegmentTokenVisitor`1 visitor)
	public virtual void Accept (Microsoft.OData.UriParser.IPathSegmentTokenVisitor visitor)
	public virtual bool IsNamespaceOrContainerQualified ()
}

[
DebuggerDisplayAttribute(),
]
public sealed class Microsoft.OData.UriParser.ODataUnrecognizedPathException : Microsoft.OData.ODataException, _Exception, ISerializable {
	public ODataUnrecognizedPathException ()
	public ODataUnrecognizedPathException (string message)
	public ODataUnrecognizedPathException (string message, System.Exception innerException)

	string CurrentSegment  { public get; public set; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.UriParser.ODataPathSegment]] ParsedSegments  { public get; public set; }
	System.Collections.Generic.IEnumerable`1[[System.String]] UnparsedSegments  { public get; public set; }
}

public sealed class Microsoft.OData.UriParser.ODataUriParser {
	public ODataUriParser (Microsoft.OData.Edm.IEdmModel model, System.Uri relativeUri)
	public ODataUriParser (Microsoft.OData.Edm.IEdmModel model, System.Uri relativeUri, System.IServiceProvider container)
	public ODataUriParser (Microsoft.OData.Edm.IEdmModel model, System.Uri serviceRoot, System.Uri uri)
	public ODataUriParser (Microsoft.OData.Edm.IEdmModel model, System.Uri serviceRoot, System.Uri uri, System.IServiceProvider container)

	System.Func`2[[System.String],[Microsoft.OData.UriParser.BatchReferenceSegment]] BatchReferenceCallback  { public get; public set; }
	System.IServiceProvider Container  { public get; }
	System.Collections.Generic.IList`1[[System.Collections.Generic.KeyValuePair`2[[System.String],[System.String]]]] CustomQueryOptions  { public get; }
	bool EnableNoDollarQueryOptions  { public get; public set; }
	bool EnableUriTemplateParsing  { public get; public set; }
	Microsoft.OData.Edm.IEdmModel Model  { public get; }
	System.Collections.Generic.IDictionary`2[[System.String],[Microsoft.OData.UriParser.SingleValueNode]] ParameterAliasNodes  { public get; }
	Microsoft.OData.UriParser.ParseDynamicPathSegment ParseDynamicPathSegmentFunc  { public get; public set; }
	Microsoft.OData.UriParser.ODataUriResolver Resolver  { public get; public set; }
	System.Uri ServiceRoot  { public get; }
	Microsoft.OData.UriParser.ODataUriParserSettings Settings  { public get; }
	Microsoft.OData.ODataUrlKeyDelimiter UrlKeyDelimiter  { public get; public set; }

	public Microsoft.OData.UriParser.Aggregation.ApplyClause ParseApply ()
	public Microsoft.OData.UriParser.ComputeClause ParseCompute ()
	public System.Nullable`1[[System.Boolean]] ParseCount ()
	public string ParseDeltaToken ()
	public Microsoft.OData.UriParser.EntityIdSegment ParseEntityId ()
	public Microsoft.OData.UriParser.FilterClause ParseFilter ()
	public Microsoft.OData.UriParser.OrderByClause ParseOrderBy ()
	public Microsoft.OData.UriParser.ODataPath ParsePath ()
	public Microsoft.OData.UriParser.SearchClause ParseSearch ()
	public Microsoft.OData.UriParser.SelectExpandClause ParseSelectAndExpand ()
	public System.Nullable`1[[System.Int64]] ParseSkip ()
	public string ParseSkipToken ()
	public System.Nullable`1[[System.Int64]] ParseTop ()
	public Microsoft.OData.ODataUri ParseUri ()
}

public sealed class Microsoft.OData.UriParser.ODataUriParserSettings {
	public ODataUriParserSettings ()

	int MaximumExpansionCount  { public get; public set; }
	int MaximumExpansionDepth  { public get; public set; }
}

public sealed class Microsoft.OData.UriParser.OperationImportSegment : Microsoft.OData.UriParser.ODataPathSegment {
	public OperationImportSegment (Microsoft.OData.Edm.IEdmOperationImport operationImport, Microsoft.OData.Edm.IEdmEntitySetBase entitySet)
	public OperationImportSegment (System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmOperationImport]] operationImports, Microsoft.OData.Edm.IEdmEntitySetBase entitySet)
	public OperationImportSegment (Microsoft.OData.Edm.IEdmOperationImport operationImport, Microsoft.OData.Edm.IEdmEntitySetBase entitySet, System.Collections.Generic.IEnumerable`1[[Microsoft.OData.UriParser.OperationSegmentParameter]] parameters)
	public OperationImportSegment (System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmOperationImport]] operationImports, Microsoft.OData.Edm.IEdmEntitySetBase entitySet, System.Collections.Generic.IEnumerable`1[[Microsoft.OData.UriParser.OperationSegmentParameter]] parameters)

	Microsoft.OData.Edm.IEdmType EdmType  { public virtual get; }
	Microsoft.OData.Edm.IEdmEntitySetBase EntitySet  { public get; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmOperationImport]] OperationImports  { public get; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.UriParser.OperationSegmentParameter]] Parameters  { public get; }

	public virtual void HandleWith (Microsoft.OData.UriParser.PathSegmentHandler handler)
	public virtual T TranslateWith (PathSegmentTranslator`1 translator)
}

public sealed class Microsoft.OData.UriParser.OperationSegment : Microsoft.OData.UriParser.ODataPathSegment {
	public OperationSegment (Microsoft.OData.Edm.IEdmOperation operation, Microsoft.OData.Edm.IEdmEntitySetBase entitySet)
	public OperationSegment (System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmOperation]] operations, Microsoft.OData.Edm.IEdmEntitySetBase entitySet)
	public OperationSegment (Microsoft.OData.Edm.IEdmOperation operation, System.Collections.Generic.IEnumerable`1[[Microsoft.OData.UriParser.OperationSegmentParameter]] parameters, Microsoft.OData.Edm.IEdmEntitySetBase entitySet)
	public OperationSegment (System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmOperation]] operations, System.Collections.Generic.IEnumerable`1[[Microsoft.OData.UriParser.OperationSegmentParameter]] parameters, Microsoft.OData.Edm.IEdmEntitySetBase entitySet)

	Microsoft.OData.Edm.IEdmType EdmType  { public virtual get; }
	Microsoft.OData.Edm.IEdmEntitySetBase EntitySet  { public get; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmOperation]] Operations  { public get; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.UriParser.OperationSegmentParameter]] Parameters  { public get; }

	public virtual void HandleWith (Microsoft.OData.UriParser.PathSegmentHandler handler)
	public virtual T TranslateWith (PathSegmentTranslator`1 translator)
}

public sealed class Microsoft.OData.UriParser.OperationSegmentParameter {
	public OperationSegmentParameter (string name, object value)

	string Name  { public get; }
	object Value  { public get; }
}

public sealed class Microsoft.OData.UriParser.OrderByClause {
	public OrderByClause (Microsoft.OData.UriParser.OrderByClause thenBy, Microsoft.OData.UriParser.SingleValueNode expression, Microsoft.OData.UriParser.OrderByDirection direction, Microsoft.OData.UriParser.RangeVariable rangeVariable)

	Microsoft.OData.UriParser.OrderByDirection Direction  { public get; }
	Microsoft.OData.UriParser.SingleValueNode Expression  { public get; }
	Microsoft.OData.Edm.IEdmTypeReference ItemType  { public get; }
	Microsoft.OData.UriParser.RangeVariable RangeVariable  { public get; }
	Microsoft.OData.UriParser.OrderByClause ThenBy  { public get; }
}

public sealed class Microsoft.OData.UriParser.OrderByToken : Microsoft.OData.UriParser.QueryToken {
	public OrderByToken (Microsoft.OData.UriParser.QueryToken expression, Microsoft.OData.UriParser.OrderByDirection direction)

	Microsoft.OData.UriParser.OrderByDirection Direction  { public get; }
	Microsoft.OData.UriParser.QueryToken Expression  { public get; }
	Microsoft.OData.UriParser.QueryTokenKind Kind  { public virtual get; }

	public virtual T Accept (ISyntacticTreeVisitor`1 visitor)
}

public sealed class Microsoft.OData.UriParser.ParseDynamicPathSegment : System.MulticastDelegate, ICloneable, ISerializable {
	public ParseDynamicPathSegment (object object, System.IntPtr method)

	public virtual System.IAsyncResult BeginInvoke (Microsoft.OData.UriParser.ODataPathSegment previous, string identifier, string parenthesisExpression, System.AsyncCallback callback, object object)
	public virtual System.Collections.Generic.ICollection`1[[Microsoft.OData.UriParser.ODataPathSegment]] EndInvoke (System.IAsyncResult result)
	public virtual System.Collections.Generic.ICollection`1[[Microsoft.OData.UriParser.ODataPathSegment]] Invoke (Microsoft.OData.UriParser.ODataPathSegment previous, string identifier, string parenthesisExpression)
}

public sealed class Microsoft.OData.UriParser.PathSelectItem : Microsoft.OData.UriParser.SelectItem {
	public PathSelectItem (Microsoft.OData.UriParser.ODataSelectPath selectedPath)

	Microsoft.OData.UriParser.ODataSelectPath SelectedPath  { public get; }

	public virtual void HandleWith (Microsoft.OData.UriParser.SelectItemHandler handler)
	public virtual T TranslateWith (SelectItemTranslator`1 translator)
}

public sealed class Microsoft.OData.UriParser.PathTemplateSegment : Microsoft.OData.UriParser.ODataPathSegment {
	public PathTemplateSegment (string literalText)

	Microsoft.OData.Edm.IEdmType EdmType  { public virtual get; }
	string LiteralText  { public get; }

	public virtual void HandleWith (Microsoft.OData.UriParser.PathSegmentHandler handler)
	public virtual T TranslateWith (PathSegmentTranslator`1 translator)
}

public sealed class Microsoft.OData.UriParser.PropertySegment : Microsoft.OData.UriParser.ODataPathSegment {
	public PropertySegment (Microsoft.OData.Edm.IEdmStructuralProperty property)

	Microsoft.OData.Edm.IEdmType EdmType  { public virtual get; }
	Microsoft.OData.Edm.IEdmStructuralProperty Property  { public get; }

	public virtual void HandleWith (Microsoft.OData.UriParser.PathSegmentHandler handler)
	public virtual T TranslateWith (PathSegmentTranslator`1 translator)
}

public sealed class Microsoft.OData.UriParser.RangeVariableToken : Microsoft.OData.UriParser.QueryToken {
	public RangeVariableToken (string name)

	Microsoft.OData.UriParser.QueryTokenKind Kind  { public virtual get; }
	string Name  { public get; }

	public virtual T Accept (ISyntacticTreeVisitor`1 visitor)
}

public sealed class Microsoft.OData.UriParser.ResourceRangeVariable : Microsoft.OData.UriParser.RangeVariable {
	public ResourceRangeVariable (string name, Microsoft.OData.Edm.IEdmStructuredTypeReference structuredType, Microsoft.OData.Edm.IEdmNavigationSource navigationSource)
	public ResourceRangeVariable (string name, Microsoft.OData.Edm.IEdmStructuredTypeReference structuredType, Microsoft.OData.UriParser.CollectionResourceNode collectionResourceNode)

	Microsoft.OData.UriParser.CollectionResourceNode CollectionResourceNode  { public get; }
	int Kind  { public virtual get; }
	string Name  { public virtual get; }
	Microsoft.OData.Edm.IEdmNavigationSource NavigationSource  { public get; }
	Microsoft.OData.Edm.IEdmStructuredTypeReference StructuredTypeReference  { public get; }
	Microsoft.OData.Edm.IEdmTypeReference TypeReference  { public virtual get; }
}

public sealed class Microsoft.OData.UriParser.ResourceRangeVariableReferenceNode : Microsoft.OData.UriParser.SingleResourceNode {
	public ResourceRangeVariableReferenceNode (string name, Microsoft.OData.UriParser.ResourceRangeVariable rangeVariable)

	string Name  { public get; }
	Microsoft.OData.Edm.IEdmNavigationSource NavigationSource  { public virtual get; }
	Microsoft.OData.UriParser.ResourceRangeVariable RangeVariable  { public get; }
	Microsoft.OData.Edm.IEdmStructuredTypeReference StructuredTypeReference  { public virtual get; }
	Microsoft.OData.Edm.IEdmTypeReference TypeReference  { public virtual get; }

	public virtual T Accept (QueryNodeVisitor`1 visitor)
}

public sealed class Microsoft.OData.UriParser.SearchClause {
	public SearchClause (Microsoft.OData.UriParser.SingleValueNode expression)

	Microsoft.OData.UriParser.SingleValueNode Expression  { public get; }
}

public sealed class Microsoft.OData.UriParser.SearchTermNode : Microsoft.OData.UriParser.SingleValueNode {
	public SearchTermNode (string text)

	string Text  { public get; }
	Microsoft.OData.Edm.IEdmTypeReference TypeReference  { public virtual get; }

	public virtual T Accept (QueryNodeVisitor`1 visitor)
}

public sealed class Microsoft.OData.UriParser.SelectExpandClause {
	public SelectExpandClause (System.Collections.Generic.IEnumerable`1[[Microsoft.OData.UriParser.SelectItem]] selectedItems, bool allSelected)

	bool AllSelected  { public get; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.UriParser.SelectItem]] SelectedItems  { public get; }
}

public sealed class Microsoft.OData.UriParser.SelectToken : Microsoft.OData.UriParser.QueryToken {
	public SelectToken (System.Collections.Generic.IEnumerable`1[[Microsoft.OData.UriParser.PathSegmentToken]] properties)

	Microsoft.OData.UriParser.QueryTokenKind Kind  { public virtual get; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.UriParser.PathSegmentToken]] Properties  { public get; }

	public virtual T Accept (ISyntacticTreeVisitor`1 visitor)
}

public sealed class Microsoft.OData.UriParser.SingleNavigationNode : Microsoft.OData.UriParser.SingleEntityNode {
	public SingleNavigationNode (Microsoft.OData.UriParser.SingleResourceNode source, Microsoft.OData.Edm.IEdmNavigationProperty navigationProperty, Microsoft.OData.Edm.IEdmPathExpression bindingPath)

	Microsoft.OData.Edm.IEdmPathExpression BindingPath  { public get; }
	Microsoft.OData.Edm.IEdmEntityTypeReference EntityTypeReference  { public virtual get; }
	Microsoft.OData.Edm.IEdmNavigationProperty NavigationProperty  { public get; }
	Microsoft.OData.Edm.IEdmNavigationSource NavigationSource  { public virtual get; }
	Microsoft.OData.UriParser.SingleResourceNode Source  { public get; }
	Microsoft.OData.Edm.IEdmStructuredTypeReference StructuredTypeReference  { public virtual get; }
	Microsoft.OData.Edm.EdmMultiplicity TargetMultiplicity  { public get; }
	Microsoft.OData.Edm.IEdmTypeReference TypeReference  { public virtual get; }

	public virtual T Accept (QueryNodeVisitor`1 visitor)
}

public sealed class Microsoft.OData.UriParser.SingleResourceCastNode : Microsoft.OData.UriParser.SingleResourceNode {
	public SingleResourceCastNode (Microsoft.OData.UriParser.SingleResourceNode source, Microsoft.OData.Edm.IEdmStructuredType structuredType)

	Microsoft.OData.Edm.IEdmNavigationSource NavigationSource  { public virtual get; }
	Microsoft.OData.UriParser.SingleResourceNode Source  { public get; }
	Microsoft.OData.Edm.IEdmStructuredTypeReference StructuredTypeReference  { public virtual get; }
	Microsoft.OData.Edm.IEdmTypeReference TypeReference  { public virtual get; }

	public virtual T Accept (QueryNodeVisitor`1 visitor)
}

public sealed class Microsoft.OData.UriParser.SingleResourceFunctionCallNode : Microsoft.OData.UriParser.SingleResourceNode {
	public SingleResourceFunctionCallNode (string name, System.Collections.Generic.IEnumerable`1[[Microsoft.OData.UriParser.QueryNode]] parameters, Microsoft.OData.Edm.IEdmStructuredTypeReference returnedStructuredTypeReference, Microsoft.OData.Edm.IEdmNavigationSource navigationSource)
	public SingleResourceFunctionCallNode (string name, System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmFunction]] functions, System.Collections.Generic.IEnumerable`1[[Microsoft.OData.UriParser.QueryNode]] parameters, Microsoft.OData.Edm.IEdmStructuredTypeReference returnedStructuredTypeReference, Microsoft.OData.Edm.IEdmNavigationSource navigationSource, Microsoft.OData.UriParser.QueryNode source)

	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmFunction]] Functions  { public get; }
	string Name  { public get; }
	Microsoft.OData.Edm.IEdmNavigationSource NavigationSource  { public virtual get; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.UriParser.QueryNode]] Parameters  { public get; }
	Microsoft.OData.UriParser.QueryNode Source  { public get; }
	Microsoft.OData.Edm.IEdmStructuredTypeReference StructuredTypeReference  { public virtual get; }
	Microsoft.OData.Edm.IEdmTypeReference TypeReference  { public virtual get; }

	public virtual T Accept (QueryNodeVisitor`1 visitor)
}

public sealed class Microsoft.OData.UriParser.SingletonSegment : Microsoft.OData.UriParser.ODataPathSegment {
	public SingletonSegment (Microsoft.OData.Edm.IEdmSingleton singleton)

	Microsoft.OData.Edm.IEdmType EdmType  { public virtual get; }
	Microsoft.OData.Edm.IEdmSingleton Singleton  { public get; }

	public virtual void HandleWith (Microsoft.OData.UriParser.PathSegmentHandler handler)
	public virtual T TranslateWith (PathSegmentTranslator`1 translator)
}

public sealed class Microsoft.OData.UriParser.SingleValueCastNode : Microsoft.OData.UriParser.SingleValueNode {
	public SingleValueCastNode (Microsoft.OData.UriParser.SingleValueNode source, Microsoft.OData.Edm.IEdmPrimitiveType primitiveType)

	Microsoft.OData.UriParser.SingleValueNode Source  { public get; }
	Microsoft.OData.Edm.IEdmTypeReference TypeReference  { public virtual get; }

	public virtual T Accept (QueryNodeVisitor`1 visitor)
}

public sealed class Microsoft.OData.UriParser.SingleValueFunctionCallNode : Microsoft.OData.UriParser.SingleValueNode {
	public SingleValueFunctionCallNode (string name, System.Collections.Generic.IEnumerable`1[[Microsoft.OData.UriParser.QueryNode]] parameters, Microsoft.OData.Edm.IEdmTypeReference returnedTypeReference)
	public SingleValueFunctionCallNode (string name, System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmFunction]] functions, System.Collections.Generic.IEnumerable`1[[Microsoft.OData.UriParser.QueryNode]] parameters, Microsoft.OData.Edm.IEdmTypeReference returnedTypeReference, Microsoft.OData.UriParser.QueryNode source)

	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmFunction]] Functions  { public get; }
	string Name  { public get; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.UriParser.QueryNode]] Parameters  { public get; }
	Microsoft.OData.UriParser.QueryNode Source  { public get; }
	Microsoft.OData.Edm.IEdmTypeReference TypeReference  { public virtual get; }

	public virtual T Accept (QueryNodeVisitor`1 visitor)
}

public sealed class Microsoft.OData.UriParser.SingleValueOpenPropertyAccessNode : Microsoft.OData.UriParser.SingleValueNode {
	public SingleValueOpenPropertyAccessNode (Microsoft.OData.UriParser.SingleValueNode source, string openPropertyName)

	string Name  { public get; }
	Microsoft.OData.UriParser.SingleValueNode Source  { public get; }
	Microsoft.OData.Edm.IEdmTypeReference TypeReference  { public virtual get; }

	public virtual T Accept (QueryNodeVisitor`1 visitor)
}

public sealed class Microsoft.OData.UriParser.SingleValuePropertyAccessNode : Microsoft.OData.UriParser.SingleValueNode {
	public SingleValuePropertyAccessNode (Microsoft.OData.UriParser.SingleValueNode source, Microsoft.OData.Edm.IEdmProperty property)

	Microsoft.OData.Edm.IEdmProperty Property  { public get; }
	Microsoft.OData.UriParser.SingleValueNode Source  { public get; }
	Microsoft.OData.Edm.IEdmTypeReference TypeReference  { public virtual get; }

	public virtual T Accept (QueryNodeVisitor`1 visitor)
}

public sealed class Microsoft.OData.UriParser.StarToken : Microsoft.OData.UriParser.PathToken {
	public StarToken (Microsoft.OData.UriParser.QueryToken nextToken)

	string Identifier  { public virtual get; }
	Microsoft.OData.UriParser.QueryTokenKind Kind  { public virtual get; }
	Microsoft.OData.UriParser.QueryToken NextToken  { public virtual get; public virtual set; }

	public virtual T Accept (ISyntacticTreeVisitor`1 visitor)
}

public sealed class Microsoft.OData.UriParser.StringAsEnumResolver : Microsoft.OData.UriParser.ODataUriResolver {
	public StringAsEnumResolver ()

	public virtual void PromoteBinaryOperandTypes (Microsoft.OData.UriParser.BinaryOperatorKind binaryOperatorKind, Microsoft.OData.UriParser.SingleValueNode& leftNode, Microsoft.OData.UriParser.SingleValueNode& rightNode, out Microsoft.OData.Edm.IEdmTypeReference& typeReference)
	public virtual System.Collections.Generic.IEnumerable`1[[System.Collections.Generic.KeyValuePair`2[[System.String],[System.Object]]]] ResolveKeys (Microsoft.OData.Edm.IEdmEntityType type, System.Collections.Generic.IDictionary`2[[System.String],[System.String]] namedValues, System.Func`3[[Microsoft.OData.Edm.IEdmTypeReference],[System.String],[System.Object]] convertFunc)
	public virtual System.Collections.Generic.IEnumerable`1[[System.Collections.Generic.KeyValuePair`2[[System.String],[System.Object]]]] ResolveKeys (Microsoft.OData.Edm.IEdmEntityType type, System.Collections.Generic.IList`1[[System.String]] positionalValues, System.Func`3[[Microsoft.OData.Edm.IEdmTypeReference],[System.String],[System.Object]] convertFunc)
	public virtual System.Collections.Generic.IDictionary`2[[Microsoft.OData.Edm.IEdmOperationParameter],[Microsoft.OData.UriParser.SingleValueNode]] ResolveOperationParameters (Microsoft.OData.Edm.IEdmOperation operation, System.Collections.Generic.IDictionary`2[[System.String],[Microsoft.OData.UriParser.SingleValueNode]] input)
}

public sealed class Microsoft.OData.UriParser.SystemToken : Microsoft.OData.UriParser.PathSegmentToken {
	public SystemToken (string identifier, Microsoft.OData.UriParser.PathSegmentToken nextToken)

	string Identifier  { public virtual get; }

	public virtual T Accept (IPathSegmentTokenVisitor`1 visitor)
	public virtual void Accept (Microsoft.OData.UriParser.IPathSegmentTokenVisitor visitor)
	public virtual bool IsNamespaceOrContainerQualified ()
}

public sealed class Microsoft.OData.UriParser.TypeSegment : Microsoft.OData.UriParser.ODataPathSegment {
	public TypeSegment (Microsoft.OData.Edm.IEdmType actualType, Microsoft.OData.Edm.IEdmNavigationSource navigationSource)
	public TypeSegment (Microsoft.OData.Edm.IEdmType actualType, Microsoft.OData.Edm.IEdmType expectedType, Microsoft.OData.Edm.IEdmNavigationSource navigationSource)

	Microsoft.OData.Edm.IEdmType EdmType  { public virtual get; }
	Microsoft.OData.Edm.IEdmNavigationSource NavigationSource  { public get; }

	public virtual void HandleWith (Microsoft.OData.UriParser.PathSegmentHandler handler)
	public virtual T TranslateWith (PathSegmentTranslator`1 translator)
}

public sealed class Microsoft.OData.UriParser.UnaryOperatorNode : Microsoft.OData.UriParser.SingleValueNode {
	public UnaryOperatorNode (Microsoft.OData.UriParser.UnaryOperatorKind operatorKind, Microsoft.OData.UriParser.SingleValueNode operand)

	Microsoft.OData.UriParser.SingleValueNode Operand  { public get; }
	Microsoft.OData.UriParser.UnaryOperatorKind OperatorKind  { public get; }
	Microsoft.OData.Edm.IEdmTypeReference TypeReference  { public virtual get; }

	public virtual T Accept (QueryNodeVisitor`1 visitor)
}

public sealed class Microsoft.OData.UriParser.UnaryOperatorToken : Microsoft.OData.UriParser.QueryToken {
	public UnaryOperatorToken (Microsoft.OData.UriParser.UnaryOperatorKind operatorKind, Microsoft.OData.UriParser.QueryToken operand)

	Microsoft.OData.UriParser.QueryTokenKind Kind  { public virtual get; }
	Microsoft.OData.UriParser.QueryToken Operand  { public get; }
	Microsoft.OData.UriParser.UnaryOperatorKind OperatorKind  { public get; }

	public virtual T Accept (ISyntacticTreeVisitor`1 visitor)
}

[
DebuggerDisplayAttribute(),
]
public sealed class Microsoft.OData.UriParser.UriLiteralParsingException : Microsoft.OData.ODataException, _Exception, ISerializable {
	public UriLiteralParsingException ()
	public UriLiteralParsingException (string message)
	public UriLiteralParsingException (string message, System.Exception innerException)
}

public sealed class Microsoft.OData.UriParser.UriQueryExpressionParser {
	public UriQueryExpressionParser (int maxDepth)

	public Microsoft.OData.UriParser.QueryToken ParseFilter (string filter)
}

public sealed class Microsoft.OData.UriParser.UriTemplateExpression {
	public UriTemplateExpression ()

	Microsoft.OData.Edm.IEdmTypeReference ExpectedType  { public get; }
	string LiteralText  { public get; }
}

public sealed class Microsoft.OData.UriParser.ValueSegment : Microsoft.OData.UriParser.ODataPathSegment {
	public ValueSegment (Microsoft.OData.Edm.IEdmType previousType)

	Microsoft.OData.Edm.IEdmType EdmType  { public virtual get; }

	public virtual void HandleWith (Microsoft.OData.UriParser.PathSegmentHandler handler)
	public virtual T TranslateWith (PathSegmentTranslator`1 translator)
}

public sealed class Microsoft.OData.UriParser.WildcardSelectItem : Microsoft.OData.UriParser.SelectItem {
	public WildcardSelectItem ()

	public virtual void HandleWith (Microsoft.OData.UriParser.SelectItemHandler handler)
	public virtual T TranslateWith (SelectItemTranslator`1 translator)
}

public enum Microsoft.OData.UriParser.Aggregation.AggregationMethod : int {
	Average = 3
	CountDistinct = 4
	Custom = 6
	Max = 2
	Min = 1
	Sum = 0
	VirtualPropertyCount = 5
}

public enum Microsoft.OData.UriParser.Aggregation.TransformationNodeKind : int {
	Aggregate = 0
	Compute = 3
	Filter = 2
	GroupBy = 1
}

public abstract class Microsoft.OData.UriParser.Aggregation.ApplyTransformationToken : Microsoft.OData.UriParser.QueryToken {
	protected ApplyTransformationToken ()
}

public abstract class Microsoft.OData.UriParser.Aggregation.TransformationNode {
	protected TransformationNode ()

	Microsoft.OData.UriParser.Aggregation.TransformationNodeKind Kind  { public abstract get; }
}

public sealed class Microsoft.OData.UriParser.Aggregation.AggregateExpression {
	public AggregateExpression (Microsoft.OData.UriParser.SingleValueNode expression, Microsoft.OData.UriParser.Aggregation.AggregationMethod method, string alias, Microsoft.OData.Edm.IEdmTypeReference typeReference)
	public AggregateExpression (Microsoft.OData.UriParser.SingleValueNode expression, Microsoft.OData.UriParser.Aggregation.AggregationMethodDefinition methodDefinition, string alias, Microsoft.OData.Edm.IEdmTypeReference typeReference)

	string Alias  { public get; }
	Microsoft.OData.UriParser.SingleValueNode Expression  { public get; }
	Microsoft.OData.UriParser.Aggregation.AggregationMethod Method  { public get; }
	Microsoft.OData.UriParser.Aggregation.AggregationMethodDefinition MethodDefinition  { public get; }
	Microsoft.OData.Edm.IEdmTypeReference TypeReference  { public get; }
}

public sealed class Microsoft.OData.UriParser.Aggregation.AggregateExpressionToken : Microsoft.OData.UriParser.QueryToken {
	public AggregateExpressionToken (Microsoft.OData.UriParser.QueryToken expression, Microsoft.OData.UriParser.Aggregation.AggregationMethod method, string alias)
	public AggregateExpressionToken (Microsoft.OData.UriParser.QueryToken expression, Microsoft.OData.UriParser.Aggregation.AggregationMethodDefinition methodDefinition, string alias)

	string Alias  { public get; }
	Microsoft.OData.UriParser.QueryToken Expression  { public get; }
	Microsoft.OData.UriParser.QueryTokenKind Kind  { public virtual get; }
	Microsoft.OData.UriParser.Aggregation.AggregationMethod Method  { public get; }
	Microsoft.OData.UriParser.Aggregation.AggregationMethodDefinition MethodDefinition  { public get; }

	public virtual T Accept (ISyntacticTreeVisitor`1 visitor)
}

public sealed class Microsoft.OData.UriParser.Aggregation.AggregateToken : Microsoft.OData.UriParser.Aggregation.ApplyTransformationToken {
	public AggregateToken (System.Collections.Generic.IEnumerable`1[[Microsoft.OData.UriParser.Aggregation.AggregateExpressionToken]] expressions)

	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.UriParser.Aggregation.AggregateExpressionToken]] Expressions  { public get; }
	Microsoft.OData.UriParser.QueryTokenKind Kind  { public virtual get; }

	public virtual T Accept (ISyntacticTreeVisitor`1 visitor)
}

public sealed class Microsoft.OData.UriParser.Aggregation.AggregateTransformationNode : Microsoft.OData.UriParser.Aggregation.TransformationNode {
	public AggregateTransformationNode (System.Collections.Generic.IEnumerable`1[[Microsoft.OData.UriParser.Aggregation.AggregateExpression]] expressions)

	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.UriParser.Aggregation.AggregateExpression]] Expressions  { public get; }
	Microsoft.OData.UriParser.Aggregation.TransformationNodeKind Kind  { public virtual get; }
}

public sealed class Microsoft.OData.UriParser.Aggregation.AggregationMethodDefinition {
	public static Microsoft.OData.UriParser.Aggregation.AggregationMethodDefinition Average = Microsoft.OData.UriParser.Aggregation.AggregationMethodDefinition
	public static Microsoft.OData.UriParser.Aggregation.AggregationMethodDefinition CountDistinct = Microsoft.OData.UriParser.Aggregation.AggregationMethodDefinition
	public static Microsoft.OData.UriParser.Aggregation.AggregationMethodDefinition Max = Microsoft.OData.UriParser.Aggregation.AggregationMethodDefinition
	public static Microsoft.OData.UriParser.Aggregation.AggregationMethodDefinition Min = Microsoft.OData.UriParser.Aggregation.AggregationMethodDefinition
	public static Microsoft.OData.UriParser.Aggregation.AggregationMethodDefinition Sum = Microsoft.OData.UriParser.Aggregation.AggregationMethodDefinition
	public static Microsoft.OData.UriParser.Aggregation.AggregationMethodDefinition VirtualPropertyCount = Microsoft.OData.UriParser.Aggregation.AggregationMethodDefinition

	Microsoft.OData.UriParser.Aggregation.AggregationMethod MethodKind  { public get; }
	string MethodLabel  { public get; }

	public static Microsoft.OData.UriParser.Aggregation.AggregationMethodDefinition Custom (string customMethodLabel)
}

public sealed class Microsoft.OData.UriParser.Aggregation.ApplyClause {
	public ApplyClause (System.Collections.Generic.IList`1[[Microsoft.OData.UriParser.Aggregation.TransformationNode]] transformations)

	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.UriParser.Aggregation.TransformationNode]] Transformations  { public get; }
}

public sealed class Microsoft.OData.UriParser.Aggregation.ComputeTransformationNode : Microsoft.OData.UriParser.Aggregation.TransformationNode {
	public ComputeTransformationNode (System.Collections.Generic.IEnumerable`1[[Microsoft.OData.UriParser.ComputeExpression]] expressions)

	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.UriParser.ComputeExpression]] Expressions  { public get; }
	Microsoft.OData.UriParser.Aggregation.TransformationNodeKind Kind  { public virtual get; }
}

public sealed class Microsoft.OData.UriParser.Aggregation.FilterTransformationNode : Microsoft.OData.UriParser.Aggregation.TransformationNode {
	public FilterTransformationNode (Microsoft.OData.UriParser.FilterClause filterClause)

	Microsoft.OData.UriParser.FilterClause FilterClause  { public get; }
	Microsoft.OData.UriParser.Aggregation.TransformationNodeKind Kind  { public virtual get; }
}

public sealed class Microsoft.OData.UriParser.Aggregation.GroupByPropertyNode {
	public GroupByPropertyNode (string name, Microsoft.OData.UriParser.SingleValueNode expression)
	public GroupByPropertyNode (string name, Microsoft.OData.UriParser.SingleValueNode expression, Microsoft.OData.Edm.IEdmTypeReference type)

	System.Collections.Generic.IList`1[[Microsoft.OData.UriParser.Aggregation.GroupByPropertyNode]] ChildTransformations  { public get; public set; }
	Microsoft.OData.UriParser.SingleValueNode Expression  { public get; }
	string Name  { public get; }
	Microsoft.OData.Edm.IEdmTypeReference TypeReference  { public get; }
}

public sealed class Microsoft.OData.UriParser.Aggregation.GroupByToken : Microsoft.OData.UriParser.Aggregation.ApplyTransformationToken {
	public GroupByToken (System.Collections.Generic.IEnumerable`1[[Microsoft.OData.UriParser.EndPathToken]] properties, Microsoft.OData.UriParser.Aggregation.ApplyTransformationToken child)

	Microsoft.OData.UriParser.Aggregation.ApplyTransformationToken Child  { public get; }
	Microsoft.OData.UriParser.QueryTokenKind Kind  { public virtual get; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.UriParser.EndPathToken]] Properties  { public get; }

	public virtual T Accept (ISyntacticTreeVisitor`1 visitor)
}

public sealed class Microsoft.OData.UriParser.Aggregation.GroupByTransformationNode : Microsoft.OData.UriParser.Aggregation.TransformationNode {
	public GroupByTransformationNode (System.Collections.Generic.IList`1[[Microsoft.OData.UriParser.Aggregation.GroupByPropertyNode]] groupingProperties, Microsoft.OData.UriParser.Aggregation.TransformationNode childTransformations, Microsoft.OData.UriParser.CollectionNode source)

	Microsoft.OData.UriParser.Aggregation.TransformationNode ChildTransformations  { public get; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.UriParser.Aggregation.GroupByPropertyNode]] GroupingProperties  { public get; }
	Microsoft.OData.UriParser.Aggregation.TransformationNodeKind Kind  { public virtual get; }
	Microsoft.OData.UriParser.CollectionNode Source  { public get; }
}

public enum Microsoft.OData.Client.DataServiceResponsePreference : int {
	IncludeContent = 1
	NoContent = 2
	None = 0
}

public enum Microsoft.OData.Client.EntityParameterSendOption : int {
	SendFullProperties = 0
	SendOnlySetProperties = 1
}

[
FlagsAttribute(),
]
public enum Microsoft.OData.Client.EntityStates : int {
	Added = 4
	Deleted = 8
	Detached = 1
	Modified = 16
	Unchanged = 2
}

public enum Microsoft.OData.Client.MergeOption : int {
	AppendOnly = 0
	NoTracking = 3
	OverwriteChanges = 1
	PreserveChanges = 2
}

public enum Microsoft.OData.Client.ODataProtocolVersion : int {
	V4 = 0
	V401 = 1
}

[
FlagsAttribute(),
]
public enum Microsoft.OData.Client.SaveChangesOptions : int {
	BatchWithIndependentOperations = 16
	BatchWithSingleChangeset = 1
	ContinueOnError = 2
	None = 0
	PostOnlySetProperties = 8
	ReplaceOnUpdate = 4
}

public enum Microsoft.OData.Client.TrackingMode : int {
	AutoChangeTracking = 1
	None = 0
}

public abstract class Microsoft.OData.Client.DataServiceClientRequestMessage : IODataRequestMessage {
	public DataServiceClientRequestMessage (string actualMethod)

	string ActualMethod  { protected virtual get; }
	System.Net.ICredentials Credentials  { public abstract get; public abstract set; }
	System.Collections.Generic.IEnumerable`1[[System.Collections.Generic.KeyValuePair`2[[System.String],[System.String]]]] Headers  { public abstract get; }
	string Method  { public abstract get; public abstract set; }
	bool SendChunked  { public abstract get; public abstract set; }
	int Timeout  { public abstract get; public abstract set; }
	System.Uri Url  { public abstract get; public abstract set; }

	public abstract void Abort ()
	public abstract System.IAsyncResult BeginGetRequestStream (System.AsyncCallback callback, object state)
	public abstract System.IAsyncResult BeginGetResponse (System.AsyncCallback callback, object state)
	public abstract System.IO.Stream EndGetRequestStream (System.IAsyncResult asyncResult)
	public abstract Microsoft.OData.IODataResponseMessage EndGetResponse (System.IAsyncResult asyncResult)
	public abstract string GetHeader (string headerName)
	public abstract Microsoft.OData.IODataResponseMessage GetResponse ()
	public abstract System.IO.Stream GetStream ()
	public abstract void SetHeader (string headerName, string headerValue)
}

public abstract class Microsoft.OData.Client.DataServiceQuery : Microsoft.OData.Client.DataServiceRequest, IEnumerable, IQueryable {
	System.Linq.Expressions.Expression Expression  { public abstract get; }
	System.Linq.IQueryProvider Provider  { public abstract get; }

	public System.IAsyncResult BeginExecute (System.AsyncCallback callback, object state)
	internal abstract System.IAsyncResult BeginExecuteInternal (System.AsyncCallback callback, object state)
	public System.Collections.IEnumerable EndExecute (System.IAsyncResult asyncResult)
	internal abstract System.Collections.IEnumerable EndExecuteInternal (System.IAsyncResult asyncResult)
	public System.Collections.IEnumerable Execute ()
	public System.Threading.Tasks.Task`1[[System.Collections.IEnumerable]] ExecuteAsync ()
	internal abstract System.Collections.IEnumerable ExecuteInternal ()
	System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
}

[
DebuggerDisplayAttribute(),
]
public abstract class Microsoft.OData.Client.DataServiceQueryContinuation {
	System.Uri NextLinkUri  { public get; }

	public virtual string ToString ()
}

public abstract class Microsoft.OData.Client.DataServiceRequest {
	System.Type ElementType  { public abstract get; }
	System.Uri RequestUri  { public abstract get; }

	internal abstract Microsoft.OData.Client.QueryComponents QueryComponents (Microsoft.OData.Client.ClientEdmModel model)
}

public abstract class Microsoft.OData.Client.Descriptor {
	Microsoft.OData.Client.EntityStates State  { public get; }

	internal abstract void ClearChanges ()
}

public abstract class Microsoft.OData.Client.EntityTrackerBase {
	protected EntityTrackerBase ()

	internal abstract void AttachIdentity (Microsoft.OData.Client.EntityDescriptor entityDescriptorFromMaterializer, Microsoft.OData.Client.MergeOption metadataMergeOption)
	internal abstract void AttachLink (object source, string sourceProperty, object target, Microsoft.OData.Client.MergeOption linkMerge)
	internal abstract void DetachExistingLink (Microsoft.OData.Client.LinkDescriptor existingLink, bool targetDelete)
	internal abstract Microsoft.OData.Client.EntityDescriptor GetEntityDescriptor (object resource)
	internal abstract System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Client.LinkDescriptor]] GetLinks (object source, string sourceProperty)
	internal abstract Microsoft.OData.Client.EntityDescriptor InternalAttachEntityDescriptor (Microsoft.OData.Client.EntityDescriptor entityDescriptorFromMaterializer, bool failIfDuplicated)
	internal abstract object TryGetEntity (System.Uri resourceUri, out Microsoft.OData.Client.EntityStates& state)
}

public abstract class Microsoft.OData.Client.OperationDescriptor : Microsoft.OData.Client.Descriptor {
	System.Uri Metadata  { public get; }
	System.Uri Target  { public get; }
	string Title  { public get; }

	internal virtual void ClearChanges ()
}

public abstract class Microsoft.OData.Client.OperationParameter {
	protected OperationParameter (string name, object value)

	string Name  { public get; }
	object Value  { public get; }
}

public abstract class Microsoft.OData.Client.OperationResponse {
	System.Exception Error  { public get; public set; }
	System.Collections.Generic.IDictionary`2[[System.String],[System.String]] Headers  { public get; }
	int StatusCode  { public get; }
}

public sealed class Microsoft.OData.Client.Utility {
	public static System.Collections.Generic.IEnumerable`1[[System.Object]] GetCustomAttributes (System.Type type, System.Type attributeType, bool inherit)
}

public class Microsoft.OData.Client.BaseEntityType {
	public BaseEntityType ()

	Microsoft.OData.Client.DataServiceContext Context  { protected get; protected set; }
}

public class Microsoft.OData.Client.BuildingRequestEventArgs : System.EventArgs {
	Microsoft.OData.Client.Descriptor Descriptor  { public get; }
	System.Collections.Generic.IDictionary`2[[System.String],[System.String]] Headers  { public get; }
	string Method  { public get; public set; }
	System.Uri RequestUri  { public get; public set; }
}

public class Microsoft.OData.Client.DataServiceActionQuery {
	public DataServiceActionQuery (Microsoft.OData.Client.DataServiceContext context, string requestUriString, Microsoft.OData.Client.BodyOperationParameter[] parameters)

	System.Uri RequestUri  { public get; }

	public System.IAsyncResult BeginExecute (System.AsyncCallback callback, object state)
	public Microsoft.OData.Client.OperationResponse EndExecute (System.IAsyncResult asyncResult)
	public Microsoft.OData.Client.OperationResponse Execute ()
	public System.Threading.Tasks.Task`1[[Microsoft.OData.Client.OperationResponse]] ExecuteAsync ()
}

public class Microsoft.OData.Client.DataServiceClientConfigurations {
	Microsoft.OData.Client.DataServiceClientRequestPipelineConfiguration RequestPipeline  { public get; }
	Microsoft.OData.Client.DataServiceClientResponsePipelineConfiguration ResponsePipeline  { public get; }
}

public class Microsoft.OData.Client.DataServiceClientRequestMessageArgs {
	public DataServiceClientRequestMessageArgs (string method, System.Uri requestUri, bool useDefaultCredentials, bool usePostTunneling, System.Collections.Generic.IDictionary`2[[System.String],[System.String]] headers)

	string ActualMethod  { public get; }
	System.Collections.Generic.IDictionary`2[[System.String],[System.String]] Headers  { public get; }
	string Method  { public get; }
	System.Uri RequestUri  { public get; }
	bool UseDefaultCredentials  { public get; }
	bool UsePostTunneling  { public get; }
}

public class Microsoft.OData.Client.DataServiceClientRequestPipelineConfiguration {
	System.Func`2[[Microsoft.OData.Client.DataServiceClientRequestMessageArgs],[Microsoft.OData.Client.DataServiceClientRequestMessage]] OnMessageCreating  { public get; public set; }

	public Microsoft.OData.Client.DataServiceClientRequestPipelineConfiguration OnEntityReferenceLink (System.Action`1[[Microsoft.OData.Client.WritingEntityReferenceLinkArgs]] action)
	public Microsoft.OData.Client.DataServiceClientRequestPipelineConfiguration OnEntryEnding (System.Action`1[[Microsoft.OData.Client.WritingEntryArgs]] action)
	public Microsoft.OData.Client.DataServiceClientRequestPipelineConfiguration OnEntryStarting (System.Action`1[[Microsoft.OData.Client.WritingEntryArgs]] action)
	public Microsoft.OData.Client.DataServiceClientRequestPipelineConfiguration OnMessageWriterSettingsCreated (System.Action`1[[Microsoft.OData.Client.MessageWriterSettingsArgs]] args)
	public Microsoft.OData.Client.DataServiceClientRequestPipelineConfiguration OnNestedResourceInfoEnding (System.Action`1[[Microsoft.OData.Client.WritingNestedResourceInfoArgs]] action)
	public Microsoft.OData.Client.DataServiceClientRequestPipelineConfiguration OnNestedResourceInfoStarting (System.Action`1[[Microsoft.OData.Client.WritingNestedResourceInfoArgs]] action)
}

public class Microsoft.OData.Client.DataServiceClientResponsePipelineConfiguration {
	public Microsoft.OData.Client.DataServiceClientResponsePipelineConfiguration OnEntityMaterialized (System.Action`1[[Microsoft.OData.Client.MaterializedEntityArgs]] action)
	public Microsoft.OData.Client.DataServiceClientResponsePipelineConfiguration OnEntryEnded (System.Action`1[[Microsoft.OData.Client.ReadingEntryArgs]] action)
	public Microsoft.OData.Client.DataServiceClientResponsePipelineConfiguration OnEntryStarted (System.Action`1[[Microsoft.OData.Client.ReadingEntryArgs]] action)
	public Microsoft.OData.Client.DataServiceClientResponsePipelineConfiguration OnFeedEnded (System.Action`1[[Microsoft.OData.Client.ReadingFeedArgs]] action)
	public Microsoft.OData.Client.DataServiceClientResponsePipelineConfiguration OnFeedStarted (System.Action`1[[Microsoft.OData.Client.ReadingFeedArgs]] action)
	public Microsoft.OData.Client.DataServiceClientResponsePipelineConfiguration OnMessageReaderSettingsCreated (System.Action`1[[Microsoft.OData.Client.MessageReaderSettingsArgs]] messageReaderSettingsAction)
	public Microsoft.OData.Client.DataServiceClientResponsePipelineConfiguration OnNestedResourceInfoEnded (System.Action`1[[Microsoft.OData.Client.ReadingNestedResourceInfoArgs]] action)
	public Microsoft.OData.Client.DataServiceClientResponsePipelineConfiguration OnNestedResourceInfoStarted (System.Action`1[[Microsoft.OData.Client.ReadingNestedResourceInfoArgs]] action)
}

public class Microsoft.OData.Client.DataServiceCollection`1 : ObservableCollection`1, ICollection`1, IEnumerable`1, IList`1, IReadOnlyCollection`1, IReadOnlyList`1, ICollection, IEnumerable, IList, INotifyPropertyChanged, INotifyCollectionChanged {
	public DataServiceCollection`1 ()
	public DataServiceCollection`1 (DataServiceQuerySingle`1 item)
	public DataServiceCollection`1 (IEnumerable`1 items)
	public DataServiceCollection`1 (Microsoft.OData.Client.DataServiceContext context)
	public DataServiceCollection`1 (IEnumerable`1 items, Microsoft.OData.Client.TrackingMode trackingMode)
	public DataServiceCollection`1 (Microsoft.OData.Client.TrackingMode trackingMode, DataServiceQuerySingle`1 item)
	public DataServiceCollection`1 (Microsoft.OData.Client.DataServiceContext context, string entitySetName, System.Func`2[[Microsoft.OData.Client.EntityChangedParams],[System.Boolean]] entityChangedCallback, System.Func`2[[Microsoft.OData.Client.EntityCollectionChangedParams],[System.Boolean]] collectionChangedCallback)
	public DataServiceCollection`1 (IEnumerable`1 items, Microsoft.OData.Client.TrackingMode trackingMode, string entitySetName, System.Func`2[[Microsoft.OData.Client.EntityChangedParams],[System.Boolean]] entityChangedCallback, System.Func`2[[Microsoft.OData.Client.EntityCollectionChangedParams],[System.Boolean]] collectionChangedCallback)
	public DataServiceCollection`1 (Microsoft.OData.Client.DataServiceContext context, IEnumerable`1 items, Microsoft.OData.Client.TrackingMode trackingMode, string entitySetName, System.Func`2[[Microsoft.OData.Client.EntityChangedParams],[System.Boolean]] entityChangedCallback, System.Func`2[[Microsoft.OData.Client.EntityCollectionChangedParams],[System.Boolean]] collectionChangedCallback)

	DataServiceQueryContinuation`1 Continuation  { public get; public set; }

	System.EventHandler`1[[Microsoft.OData.Client.LoadCompletedEventArgs]] LoadCompleted {public add;public remove; }

	public void CancelAsyncLoad ()
	public void Clear (bool stopTracking)
	public void Detach ()
	protected virtual void InsertItem (int index, T item)
	public void Load (IEnumerable`1 items)
	public void Load (T item)
	public void LoadAsync ()
	public void LoadAsync (IQueryable`1 query)
	public void LoadAsync (System.Uri requestUri)
	public bool LoadNextPartialSetAsync ()
}

public class Microsoft.OData.Client.DataServiceContext {
	public DataServiceContext ()
	public DataServiceContext (System.Uri serviceRoot)
	public DataServiceContext (System.Uri serviceRoot, Microsoft.OData.Client.ODataProtocolVersion maxProtocolVersion)

	Microsoft.OData.Client.DataServiceResponsePreference AddAndUpdateResponsePreference  { public get; public set; }
	bool ApplyingChanges  { public get; }
	System.Uri BaseUri  { public get; public set; }
	Microsoft.OData.Client.DataServiceClientConfigurations Configurations  { public get; }
	System.Net.ICredentials Credentials  { public get; public set; }
	bool DisableInstanceAnnotationMaterialization  { public get; public set; }
	bool EnableWritingODataAnnotationWithoutPrefix  { public get; public set; }
	System.Collections.ObjectModel.ReadOnlyCollection`1[[Microsoft.OData.Client.EntityDescriptor]] Entities  { public get; }
	Microsoft.OData.Client.EntityParameterSendOption EntityParameterSendOption  { public get; public set; }
	Microsoft.OData.Client.EntityTracker EntityTracker  { public get; public set; }
	Microsoft.OData.Client.DataServiceClientFormat Format  { public get; }
	bool IgnoreResourceNotFoundException  { public get; public set; }
	System.Collections.ObjectModel.ReadOnlyCollection`1[[Microsoft.OData.Client.LinkDescriptor]] Links  { public get; }
	Microsoft.OData.Client.ODataProtocolVersion MaxProtocolVersion  { public get; }
	Microsoft.OData.Client.MergeOption MergeOption  { public get; public set; }
	System.Func`2[[System.String],[System.Uri]] ResolveEntitySet  { public get; public set; }
	System.Func`2[[System.Type],[System.String]] ResolveName  { public get; public set; }
	System.Func`2[[System.String],[System.Type]] ResolveType  { public get; public set; }
	Microsoft.OData.Client.SaveChangesOptions SaveChangesDefaultOptions  { public get; public set; }
	int Timeout  { public get; public set; }
	Microsoft.OData.Client.DataServiceUrlKeyDelimiter UrlKeyDelimiter  { public get; public set; }
	bool UsePostTunneling  { public get; public set; }

	System.EventHandler`1[[Microsoft.OData.Client.BuildingRequestEventArgs]] BuildingRequest {public add;public remove; }
	System.EventHandler`1[[Microsoft.OData.Client.ReceivingResponseEventArgs]] ReceivingResponse {public add;public remove; }
	System.EventHandler`1[[Microsoft.OData.Client.SendingRequest2EventArgs]] SendingRequest2 {public add;public remove; }

	public void AddLink (object source, string sourceProperty, object target)
	public void AddObject (string entitySetName, object entity)
	public void AddRelatedObject (object source, string sourceProperty, object target)
	public void AttachLink (object source, string sourceProperty, object target)
	public void AttachTo (string entitySetName, object entity)
	public void AttachTo (string entitySetName, object entity, string etag)
	public System.IAsyncResult BeginExecute (DataServiceQueryContinuation`1 continuation, System.AsyncCallback callback, object state)
	public System.IAsyncResult BeginExecute (System.Uri requestUri, System.AsyncCallback callback, object state)
	public System.IAsyncResult BeginExecute (System.Uri requestUri, System.AsyncCallback callback, object state, string httpMethod, Microsoft.OData.Client.OperationParameter[] operationParameters)
	public System.IAsyncResult BeginExecute (System.Uri requestUri, System.AsyncCallback callback, object state, string httpMethod, Microsoft.OData.Client.OperationParameter[] operationParameters)
	public System.IAsyncResult BeginExecute (System.Uri requestUri, System.AsyncCallback callback, object state, string httpMethod, bool singleResult, Microsoft.OData.Client.OperationParameter[] operationParameters)
	public System.IAsyncResult BeginExecuteBatch (System.AsyncCallback callback, object state, Microsoft.OData.Client.DataServiceRequest[] queries)
	public System.IAsyncResult BeginGetReadStream (object entity, Microsoft.OData.Client.DataServiceRequestArgs args, System.AsyncCallback callback, object state)
	public System.IAsyncResult BeginGetReadStream (object entity, string name, Microsoft.OData.Client.DataServiceRequestArgs args, System.AsyncCallback callback, object state)
	public System.IAsyncResult BeginLoadProperty (object entity, string propertyName, System.AsyncCallback callback, object state)
	public System.IAsyncResult BeginLoadProperty (object entity, string propertyName, Microsoft.OData.Client.DataServiceQueryContinuation continuation, System.AsyncCallback callback, object state)
	public System.IAsyncResult BeginLoadProperty (object entity, string propertyName, System.Uri nextLinkUri, System.AsyncCallback callback, object state)
	public System.IAsyncResult BeginSaveChanges (System.AsyncCallback callback, object state)
	public System.IAsyncResult BeginSaveChanges (Microsoft.OData.Client.SaveChangesOptions options, System.AsyncCallback callback, object state)
	public void CancelRequest (System.IAsyncResult asyncResult)
	public void ChangeState (object entity, Microsoft.OData.Client.EntityStates state)
	public DataServiceQuery`1 CreateFunctionQuery ()
	public DataServiceQuery`1 CreateFunctionQuery (string path, string functionName, bool isComposable, Microsoft.OData.Client.UriOperationParameter[] parameters)
	public DataServiceQuerySingle`1 CreateFunctionQuerySingle (string path, string functionName, bool isComposable, Microsoft.OData.Client.UriOperationParameter[] parameters)
	public DataServiceQuery`1 CreateQuery (string entitySetName)
	public DataServiceQuery`1 CreateQuery (string resourcePath, bool isComposable)
	public DataServiceQuery`1 CreateSingletonQuery (string singletonName)
	protected System.Type DefaultResolveType (string typeName, string fullNamespace, string languageDependentNamespace)
	public void DeleteLink (object source, string sourceProperty, object target)
	public void DeleteObject (object entity)
	public bool Detach (object entity)
	public bool DetachLink (object source, string sourceProperty, object target)
	public Microsoft.OData.Client.OperationResponse EndExecute (System.IAsyncResult asyncResult)
	public IEnumerable`1 EndExecute (System.IAsyncResult asyncResult)
	public Microsoft.OData.Client.DataServiceResponse EndExecuteBatch (System.IAsyncResult asyncResult)
	public Microsoft.OData.Client.DataServiceStreamResponse EndGetReadStream (System.IAsyncResult asyncResult)
	public Microsoft.OData.Client.QueryOperationResponse EndLoadProperty (System.IAsyncResult asyncResult)
	public Microsoft.OData.Client.DataServiceResponse EndSaveChanges (System.IAsyncResult asyncResult)
	public QueryOperationResponse`1 Execute (DataServiceQueryContinuation`1 continuation)
	public IEnumerable`1 Execute (System.Uri requestUri)
	public Microsoft.OData.Client.OperationResponse Execute (System.Uri requestUri, string httpMethod, Microsoft.OData.Client.OperationParameter[] operationParameters)
	public IEnumerable`1 Execute (System.Uri requestUri, string httpMethod, Microsoft.OData.Client.OperationParameter[] operationParameters)
	public IEnumerable`1 Execute (System.Uri requestUri, string httpMethod, bool singleResult, Microsoft.OData.Client.OperationParameter[] operationParameters)
	public Task`1 ExecuteAsync (DataServiceQueryContinuation`1 continuation)
	public Task`1 ExecuteAsync (System.Uri requestUri)
	public System.Threading.Tasks.Task`1[[Microsoft.OData.Client.OperationResponse]] ExecuteAsync (System.Uri requestUri, string httpMethod, Microsoft.OData.Client.OperationParameter[] operationParameters)
	public Task`1 ExecuteAsync (System.Uri requestUri, string httpMethod, Microsoft.OData.Client.OperationParameter[] operationParameters)
	public Task`1 ExecuteAsync (System.Uri requestUri, string httpMethod, bool singleResult, Microsoft.OData.Client.OperationParameter[] operationParameters)
	public Microsoft.OData.Client.DataServiceResponse ExecuteBatch (Microsoft.OData.Client.DataServiceRequest[] queries)
	public System.Threading.Tasks.Task`1[[Microsoft.OData.Client.DataServiceResponse]] ExecuteBatchAsync (Microsoft.OData.Client.DataServiceRequest[] queries)
	internal virtual Microsoft.OData.Edm.Vocabularies.IEdmVocabularyAnnotatable GetEdmOperationOrOperationImport (System.Reflection.MethodInfo methodInfo)
	public Microsoft.OData.Client.EntityDescriptor GetEntityDescriptor (object entity)
	internal virtual Microsoft.OData.Client.ODataResourceMetadataBuilder GetEntityMetadataBuilder (string entitySetName, Microsoft.OData.Edm.Vocabularies.IEdmStructuredValue entityInstance)
	public Microsoft.OData.Client.LinkDescriptor GetLinkDescriptor (object source, string sourceProperty, object target)
	public System.Uri GetMetadataUri ()
	public Microsoft.OData.Client.DataServiceStreamResponse GetReadStream (object entity)
	public Microsoft.OData.Client.DataServiceStreamResponse GetReadStream (object entity, Microsoft.OData.Client.DataServiceRequestArgs args)
	public Microsoft.OData.Client.DataServiceStreamResponse GetReadStream (object entity, string acceptContentType)
	public Microsoft.OData.Client.DataServiceStreamResponse GetReadStream (object entity, string name, Microsoft.OData.Client.DataServiceRequestArgs args)
	public System.Threading.Tasks.Task`1[[Microsoft.OData.Client.DataServiceStreamResponse]] GetReadStreamAsync (object entity, Microsoft.OData.Client.DataServiceRequestArgs args)
	public System.Threading.Tasks.Task`1[[Microsoft.OData.Client.DataServiceStreamResponse]] GetReadStreamAsync (object entity, string name, Microsoft.OData.Client.DataServiceRequestArgs args)
	public System.Uri GetReadStreamUri (object entity)
	public System.Uri GetReadStreamUri (object entity, string name)
	public Microsoft.OData.Client.QueryOperationResponse LoadProperty (object entity, string propertyName)
	public QueryOperationResponse`1 LoadProperty (object entity, string propertyName, DataServiceQueryContinuation`1 continuation)
	public Microsoft.OData.Client.QueryOperationResponse LoadProperty (object entity, string propertyName, Microsoft.OData.Client.DataServiceQueryContinuation continuation)
	public Microsoft.OData.Client.QueryOperationResponse LoadProperty (object entity, string propertyName, System.Uri nextLinkUri)
	public System.Threading.Tasks.Task`1[[Microsoft.OData.Client.QueryOperationResponse]] LoadPropertyAsync (object entity, string propertyName)
	public System.Threading.Tasks.Task`1[[Microsoft.OData.Client.QueryOperationResponse]] LoadPropertyAsync (object entity, string propertyName, Microsoft.OData.Client.DataServiceQueryContinuation continuation)
	public System.Threading.Tasks.Task`1[[Microsoft.OData.Client.QueryOperationResponse]] LoadPropertyAsync (object entity, string propertyName, System.Uri nextLinkUri)
	public Microsoft.OData.Client.DataServiceResponse SaveChanges ()
	public Microsoft.OData.Client.DataServiceResponse SaveChanges (Microsoft.OData.Client.SaveChangesOptions options)
	public System.Threading.Tasks.Task`1[[Microsoft.OData.Client.DataServiceResponse]] SaveChangesAsync ()
	public System.Threading.Tasks.Task`1[[Microsoft.OData.Client.DataServiceResponse]] SaveChangesAsync (Microsoft.OData.Client.SaveChangesOptions options)
	public void SetLink (object source, string sourceProperty, object target)
	public void SetSaveStream (object entity, System.IO.Stream stream, bool closeStream, Microsoft.OData.Client.DataServiceRequestArgs args)
	public void SetSaveStream (object entity, System.IO.Stream stream, bool closeStream, string contentType, string slug)
	public void SetSaveStream (object entity, string name, System.IO.Stream stream, bool closeStream, Microsoft.OData.Client.DataServiceRequestArgs args)
	public void SetSaveStream (object entity, string name, System.IO.Stream stream, bool closeStream, string contentType)
	public bool TryGetAnnotation (Expression`1 expression, string term, out TResult& annotation)
	public bool TryGetAnnotation (object source, string term, out TResult& annotation)
	public bool TryGetAnnotation (Expression`1 expression, string term, string qualifier, out TResult& annotation)
	public bool TryGetAnnotation (object source, string term, string qualifier, out TResult& annotation)
	public bool TryGetEntity (System.Uri identity, out TEntity& entity)
	public bool TryGetUri (object entity, out System.Uri& identity)
	public void UpdateObject (object entity)
	public void UpdateRelatedObject (object source, string sourceProperty, object target)
}

public class Microsoft.OData.Client.DataServiceQuery`1 : Microsoft.OData.Client.DataServiceQuery, IEnumerable`1, IQueryable`1, IEnumerable, IQueryable {
	public DataServiceQuery`1 (System.Linq.Expressions.Expression expression, Microsoft.OData.Client.DataServiceQueryProvider provider)
	public DataServiceQuery`1 (System.Linq.Expressions.Expression expression, Microsoft.OData.Client.DataServiceQueryProvider provider, bool isComposable)

	Microsoft.OData.Client.DataServiceContext Context  { public get; }
	System.Type ElementType  { public virtual get; }
	System.Linq.Expressions.Expression Expression  { public virtual get; }
	bool IsComposable  { public get; }
	System.Linq.IQueryProvider Provider  { public virtual get; }
	System.Uri RequestUri  { public virtual get; }

	public Microsoft.OData.Client.DataServiceQuery`1 AddQueryOption (string name, object value)
	public string AppendRequestUri (string nextSegment)
	public System.IAsyncResult BeginExecute (System.AsyncCallback callback, object state)
	internal virtual System.IAsyncResult BeginExecuteInternal (System.AsyncCallback callback, object state)
	public DataServiceQuery`1 CreateFunctionQuery (string functionName, bool isComposable, Microsoft.OData.Client.UriOperationParameter[] parameters)
	public DataServiceQuerySingle`1 CreateFunctionQuerySingle (string functionName, bool isComposable, Microsoft.OData.Client.UriOperationParameter[] parameters)
	public IEnumerable`1 EndExecute (System.IAsyncResult asyncResult)
	internal virtual System.Collections.IEnumerable EndExecuteInternal (System.IAsyncResult asyncResult)
	public IEnumerable`1 Execute ()
	public Task`1 ExecuteAsync ()
	internal virtual System.Collections.IEnumerable ExecuteInternal ()
	public Microsoft.OData.Client.DataServiceQuery`1 Expand (Expression`1 navigationPropertyAccessor)
	public Microsoft.OData.Client.DataServiceQuery`1 Expand (string path)
	public IEnumerable`1 GetAllPages ()
	public Task`1 GetAllPagesAsync ()
	public virtual IEnumerator`1 GetEnumerator ()
	public string GetKeyPath (string keyString)
	public string GetPath (string nextSegment)
	public Microsoft.OData.Client.DataServiceQuery`1 IncludeTotalCount ()
	internal virtual Microsoft.OData.Client.QueryComponents QueryComponents (Microsoft.OData.Client.ClientEdmModel model)
	System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
	public virtual string ToString ()
}

public class Microsoft.OData.Client.DataServiceQuery`1+DataServiceOrderedQuery : DataServiceQuery`1, IEnumerable`1, IOrderedQueryable`1, IQueryable`1, IEnumerable, IOrderedQueryable, IQueryable {
}

public class Microsoft.OData.Client.DataServiceQuerySingle`1 {
	public DataServiceQuerySingle`1 (Microsoft.OData.Client.DataServiceQuerySingle`1 query)
	public DataServiceQuerySingle`1 (Microsoft.OData.Client.DataServiceContext context, string path)
	public DataServiceQuerySingle`1 (Microsoft.OData.Client.DataServiceContext context, string path, bool isComposable)

	Microsoft.OData.Client.DataServiceContext Context  { public get; }
	bool IsComposable  { public get; }
	System.Uri RequestUri  { public get; }

	public string AppendRequestUri (string nextSegment)
	public System.IAsyncResult BeginGetValue (System.AsyncCallback callback, object state)
	public DataServiceQuerySingle`1 CastTo ()
	public DataServiceQuery`1 CreateFunctionQuery (string functionName, bool isComposable, Microsoft.OData.Client.UriOperationParameter[] parameters)
	public DataServiceQuerySingle`1 CreateFunctionQuerySingle (string functionName, bool isComposable, Microsoft.OData.Client.UriOperationParameter[] parameters)
	public TElement EndGetValue (System.IAsyncResult asyncResult)
	public Microsoft.OData.Client.DataServiceQuerySingle`1 Expand (Expression`1 navigationPropertyAccessor)
	public Microsoft.OData.Client.DataServiceQuerySingle`1 Expand (string path)
	public string GetPath (string nextSegment)
	public TElement GetValue ()
	public Task`1 GetValueAsync ()
	public DataServiceQuerySingle`1 Select (Expression`1 selector)
}

public class Microsoft.OData.Client.DataServiceRequestArgs {
	public DataServiceRequestArgs ()

	string AcceptContentType  { public get; public set; }
	string ContentType  { public get; public set; }
	System.Collections.Generic.Dictionary`2[[System.String],[System.String]] Headers  { public get; }
	string Slug  { public get; public set; }
}

[
SerializableAttribute(),
]
public class Microsoft.OData.Client.DataServiceTransportException : System.InvalidOperationException, _Exception, ISerializable {
	public DataServiceTransportException (Microsoft.OData.IODataResponseMessage response, System.Exception innerException)

	Microsoft.OData.IODataResponseMessage Response  { public get; }
}

public class Microsoft.OData.Client.EntityTracker : Microsoft.OData.Client.EntityTrackerBase {
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Client.EntityDescriptor]] Entities  { public get; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Client.LinkDescriptor]] Links  { public get; }

	internal virtual void AttachIdentity (Microsoft.OData.Client.EntityDescriptor entityDescriptorFromMaterializer, Microsoft.OData.Client.MergeOption metadataMergeOption)
	internal virtual void AttachLink (object source, string sourceProperty, object target, Microsoft.OData.Client.MergeOption linkMerge)
	internal virtual void DetachExistingLink (Microsoft.OData.Client.LinkDescriptor existingLink, bool targetDelete)
	internal virtual Microsoft.OData.Client.EntityDescriptor GetEntityDescriptor (object resource)
	internal virtual System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Client.LinkDescriptor]] GetLinks (object source, string sourceProperty)
	internal virtual Microsoft.OData.Client.EntityDescriptor InternalAttachEntityDescriptor (Microsoft.OData.Client.EntityDescriptor entityDescriptorFromMaterializer, bool failIfDuplicated)
	internal virtual object TryGetEntity (System.Uri resourceUri, out Microsoft.OData.Client.EntityStates& state)
	public Microsoft.OData.Client.EntityDescriptor TryGetEntityDescriptor (object entity)
}

public class Microsoft.OData.Client.HttpWebRequestMessage : Microsoft.OData.Client.DataServiceClientRequestMessage, IODataRequestMessage {
	public HttpWebRequestMessage (Microsoft.OData.Client.DataServiceClientRequestMessageArgs args)

	System.Net.ICredentials Credentials  { public virtual get; public virtual set; }
	System.Collections.Generic.IEnumerable`1[[System.Collections.Generic.KeyValuePair`2[[System.String],[System.String]]]] Headers  { public virtual get; }
	System.Net.HttpWebRequest HttpWebRequest  { public get; }
	string Method  { public virtual get; public virtual set; }
	bool SendChunked  { public virtual get; public virtual set; }
	int Timeout  { public virtual get; public virtual set; }
	System.Uri Url  { public virtual get; public virtual set; }

	public virtual void Abort ()
	public virtual System.IAsyncResult BeginGetRequestStream (System.AsyncCallback callback, object state)
	public virtual System.IAsyncResult BeginGetResponse (System.AsyncCallback callback, object state)
	public virtual System.IO.Stream EndGetRequestStream (System.IAsyncResult asyncResult)
	public virtual Microsoft.OData.IODataResponseMessage EndGetResponse (System.IAsyncResult asyncResult)
	public virtual string GetHeader (string headerName)
	public virtual Microsoft.OData.IODataResponseMessage GetResponse ()
	public virtual System.IO.Stream GetStream ()
	public virtual void SetHeader (string headerName, string headerValue)
}

public class Microsoft.OData.Client.HttpWebResponseMessage : IDisposable, IODataResponseMessage {
	public HttpWebResponseMessage (System.Net.HttpWebResponse httpResponse)
	public HttpWebResponseMessage (System.Collections.Generic.IDictionary`2[[System.String],[System.String]] headers, int statusCode, System.Func`1[[System.IO.Stream]] getResponseStream)

	System.Collections.Generic.IEnumerable`1[[System.Collections.Generic.KeyValuePair`2[[System.String],[System.String]]]] Headers  { public virtual get; }
	System.Net.HttpWebResponse Response  { public get; }
	int StatusCode  { public virtual get; public virtual set; }

	public virtual void Dispose ()
	protected virtual void Dispose (bool disposing)
	public virtual string GetHeader (string headerName)
	public virtual System.IO.Stream GetStream ()
	public virtual void SetHeader (string headerName, string headerValue)
}

public class Microsoft.OData.Client.InvokeResponse : Microsoft.OData.Client.OperationResponse {
	public InvokeResponse (System.Collections.Generic.Dictionary`2[[System.String],[System.String]] headers)
}

public class Microsoft.OData.Client.MessageReaderSettingsArgs {
	public MessageReaderSettingsArgs (Microsoft.OData.ODataMessageReaderSettings settings)

	Microsoft.OData.ODataMessageReaderSettings Settings  { public get; }
}

public class Microsoft.OData.Client.MessageWriterSettingsArgs {
	public MessageWriterSettingsArgs (Microsoft.OData.ODataMessageWriterSettings settings)

	Microsoft.OData.ODataMessageWriterSettings Settings  { public get; }
}

public class Microsoft.OData.Client.QueryOperationResponse : Microsoft.OData.Client.OperationResponse, IEnumerable {
	Microsoft.OData.Client.DataServiceRequest Query  { public get; }
	long TotalCount  { public virtual get; }

	public Microsoft.OData.Client.DataServiceQueryContinuation GetContinuation ()
	public DataServiceQueryContinuation`1 GetContinuation (IEnumerable`1 collection)
	public Microsoft.OData.Client.DataServiceQueryContinuation GetContinuation (System.Collections.IEnumerable collection)
	public virtual System.Collections.IEnumerator GetEnumerator ()
	protected T GetEnumeratorHelper (Func`1 getEnumerator)
}

public class Microsoft.OData.Client.ReceivingResponseEventArgs : System.EventArgs {
	public ReceivingResponseEventArgs (Microsoft.OData.IODataResponseMessage responseMessage, Microsoft.OData.Client.Descriptor descriptor)
	public ReceivingResponseEventArgs (Microsoft.OData.IODataResponseMessage responseMessage, Microsoft.OData.Client.Descriptor descriptor, bool isBatchPart)

	Microsoft.OData.Client.Descriptor Descriptor  { public get; }
	bool IsBatchPart  { public get; }
	Microsoft.OData.IODataResponseMessage ResponseMessage  { public get; }
}

public class Microsoft.OData.Client.SendingRequest2EventArgs : System.EventArgs {
	Microsoft.OData.Client.Descriptor Descriptor  { public get; }
	bool IsBatchPart  { public get; }
	Microsoft.OData.IODataRequestMessage RequestMessage  { public get; }
}

public class Microsoft.OData.Client.SendingRequestEventArgs : System.EventArgs {
	System.Net.WebRequest Request  { public get; public set; }
	System.Net.WebHeaderCollection RequestHeaders  { public get; }
}

public class Microsoft.OData.Client.Serializer {
	public static string GetKeyString (Microsoft.OData.Client.DataServiceContext context, System.Collections.Generic.Dictionary`2[[System.String],[System.Object]] keys)
	public static string GetParameterString (Microsoft.OData.Client.DataServiceContext context, Microsoft.OData.Client.OperationParameter[] parameters)
}

public class Microsoft.OData.Client.UriOperationParameter : Microsoft.OData.Client.OperationParameter {
	public UriOperationParameter (string name, object value)
}

public sealed class Microsoft.OData.Client.ActionDescriptor : Microsoft.OData.Client.OperationDescriptor {
	public ActionDescriptor ()
}

public sealed class Microsoft.OData.Client.BodyOperationParameter : Microsoft.OData.Client.OperationParameter {
	public BodyOperationParameter (string name, object value)
}

public sealed class Microsoft.OData.Client.ChangeOperationResponse : Microsoft.OData.Client.OperationResponse {
	Microsoft.OData.Client.Descriptor Descriptor  { public get; }
}

public sealed class Microsoft.OData.Client.DataServiceActionQuery`1 {
	public DataServiceActionQuery`1 (Microsoft.OData.Client.DataServiceContext context, string requestUriString, Microsoft.OData.Client.BodyOperationParameter[] parameters)

	System.Uri RequestUri  { public get; }

	public System.IAsyncResult BeginExecute (System.AsyncCallback callback, object state)
	public IEnumerable`1 EndExecute (System.IAsyncResult asyncResult)
	public IEnumerable`1 Execute ()
	public Task`1 ExecuteAsync ()
	public IEnumerator`1 GetEnumerator ()
}

public sealed class Microsoft.OData.Client.DataServiceActionQuerySingle`1 {
	public DataServiceActionQuerySingle`1 (Microsoft.OData.Client.DataServiceContext context, string requestUriString, Microsoft.OData.Client.BodyOperationParameter[] parameters)

	System.Uri RequestUri  { public get; }

	public System.IAsyncResult BeginGetValue (System.AsyncCallback callback, object state)
	public T EndGetValue (System.IAsyncResult asyncResult)
	public T GetValue ()
	public Task`1 GetValueAsync ()
}

[
SerializableAttribute(),
DebuggerDisplayAttribute(),
]
public sealed class Microsoft.OData.Client.DataServiceClientException : System.InvalidOperationException, _Exception, ISerializable {
	public DataServiceClientException ()
	public DataServiceClientException (string message)
	public DataServiceClientException (string message, System.Exception innerException)
	public DataServiceClientException (string message, int statusCode)
	public DataServiceClientException (string message, System.Exception innerException, int statusCode)

	int StatusCode  { public get; }
}

public sealed class Microsoft.OData.Client.DataServiceClientFormat {
	System.Func`1[[Microsoft.OData.Edm.IEdmModel]] LoadServiceModel  { public get; public set; }
	Microsoft.OData.ODataFormat ODataFormat  { public get; }

	public void UseJson ()
	public void UseJson (Microsoft.OData.Edm.IEdmModel serviceModel)
}

public sealed class Microsoft.OData.Client.DataServiceQueryContinuation`1 : Microsoft.OData.Client.DataServiceQueryContinuation {
}

[
SerializableAttribute(),
DebuggerDisplayAttribute(),
]
public sealed class Microsoft.OData.Client.DataServiceQueryException : System.InvalidOperationException, _Exception, ISerializable {
	public DataServiceQueryException ()
	public DataServiceQueryException (string message)
	public DataServiceQueryException (string message, System.Exception innerException)
	public DataServiceQueryException (string message, System.Exception innerException, Microsoft.OData.Client.QueryOperationResponse response)

	Microsoft.OData.Client.QueryOperationResponse Response  { public get; }
}

public sealed class Microsoft.OData.Client.DataServiceQueryProvider : IQueryProvider {
	public virtual System.Linq.IQueryable CreateQuery (System.Linq.Expressions.Expression expression)
	public virtual IQueryable`1 CreateQuery (System.Linq.Expressions.Expression expression)
	public virtual object Execute (System.Linq.Expressions.Expression expression)
	public virtual TResult Execute (System.Linq.Expressions.Expression expression)
}

public sealed class Microsoft.OData.Client.DataServiceRequest`1 : Microsoft.OData.Client.DataServiceRequest {
	public DataServiceRequest`1 (System.Uri requestUri)

	System.Type ElementType  { public virtual get; }
	System.Uri RequestUri  { public virtual get; }

	public virtual string ToString ()
}

[
SerializableAttribute(),
DebuggerDisplayAttribute(),
]
public sealed class Microsoft.OData.Client.DataServiceRequestException : System.InvalidOperationException, _Exception, ISerializable {
	public DataServiceRequestException ()
	public DataServiceRequestException (string message)
	public DataServiceRequestException (string message, System.Exception innerException)
	public DataServiceRequestException (string message, System.Exception innerException, Microsoft.OData.Client.DataServiceResponse response)

	Microsoft.OData.Client.DataServiceResponse Response  { public get; }
}

public sealed class Microsoft.OData.Client.DataServiceResponse : IEnumerable, IEnumerable`1 {
	System.Collections.Generic.IDictionary`2[[System.String],[System.String]] BatchHeaders  { public get; }
	int BatchStatusCode  { public get; }
	bool IsBatchResponse  { public get; }

	public virtual System.Collections.Generic.IEnumerator`1[[Microsoft.OData.Client.OperationResponse]] GetEnumerator ()
}

public sealed class Microsoft.OData.Client.DataServiceStreamLink : INotifyPropertyChanged {
	string ContentType  { public get; }
	System.Uri EditLink  { public get; }
	string ETag  { public get; }
	string Name  { public get; }
	System.Uri SelfLink  { public get; }

	System.ComponentModel.PropertyChangedEventHandler PropertyChanged {public virtual add;public virtual remove; }
}

public sealed class Microsoft.OData.Client.DataServiceStreamResponse : IDisposable {
	string ContentDisposition  { public get; }
	string ContentType  { public get; }
	System.Collections.Generic.Dictionary`2[[System.String],[System.String]] Headers  { public get; }
	System.IO.Stream Stream  { public get; }

	public virtual void Dispose ()
}

public sealed class Microsoft.OData.Client.DataServiceUrlKeyDelimiter {
	Microsoft.OData.Client.DataServiceUrlKeyDelimiter Parentheses  { public static get; }
	Microsoft.OData.Client.DataServiceUrlKeyDelimiter Slash  { public static get; }
}

public sealed class Microsoft.OData.Client.EntityChangedParams {
	Microsoft.OData.Client.DataServiceContext Context  { public get; }
	object Entity  { public get; }
	string PropertyName  { public get; }
	object PropertyValue  { public get; }
	string SourceEntitySet  { public get; }
	string TargetEntitySet  { public get; }
}

public sealed class Microsoft.OData.Client.EntityCollectionChangedParams {
	System.Collections.Specialized.NotifyCollectionChangedAction Action  { public get; }
	System.Collections.ICollection Collection  { public get; }
	Microsoft.OData.Client.DataServiceContext Context  { public get; }
	string PropertyName  { public get; }
	object SourceEntity  { public get; }
	string SourceEntitySet  { public get; }
	object TargetEntity  { public get; }
	string TargetEntitySet  { public get; }
}

[
DebuggerDisplayAttribute(),
]
public sealed class Microsoft.OData.Client.EntityDescriptor : Microsoft.OData.Client.Descriptor {
	System.Uri EditLink  { public get; }
	System.Uri EditStreamUri  { public get; }
	object Entity  { public get; }
	string ETag  { public get; public set; }
	System.Uri Identity  { public get; }
	System.Collections.ObjectModel.ReadOnlyCollection`1[[Microsoft.OData.Client.LinkInfo]] LinkInfos  { public get; }
	System.Collections.ObjectModel.ReadOnlyCollection`1[[Microsoft.OData.Client.OperationDescriptor]] OperationDescriptors  { public get; }
	Microsoft.OData.Client.EntityDescriptor ParentForInsert  { public get; }
	Microsoft.OData.Client.EntityDescriptor ParentForUpdate  { public get; }
	string ParentPropertyForInsert  { public get; }
	string ParentPropertyForUpdate  { public get; }
	System.Uri ReadStreamUri  { public get; }
	System.Uri SelfLink  { public get; }
	string ServerTypeName  { public get; }
	System.Collections.ObjectModel.ReadOnlyCollection`1[[Microsoft.OData.Client.StreamDescriptor]] StreamDescriptors  { public get; }
	string StreamETag  { public get; }
}

[
AttributeUsageAttribute(),
]
public sealed class Microsoft.OData.Client.EntitySetAttribute : System.Attribute, _Attribute {
	public EntitySetAttribute (string entitySet)

	string EntitySet  { public get; }
}

[
AttributeUsageAttribute(),
]
public sealed class Microsoft.OData.Client.EntityTypeAttribute : System.Attribute, _Attribute {
	public EntityTypeAttribute ()
}

public sealed class Microsoft.OData.Client.FunctionDescriptor : Microsoft.OData.Client.OperationDescriptor {
	public FunctionDescriptor ()
}

[
AttributeUsageAttribute(),
]
public sealed class Microsoft.OData.Client.HasStreamAttribute : System.Attribute, _Attribute {
	public HasStreamAttribute ()
}

[
AttributeUsageAttribute(),
]
public sealed class Microsoft.OData.Client.IgnoreClientPropertyAttribute : System.Attribute, _Attribute {
	public IgnoreClientPropertyAttribute ()
}

[
AttributeUsageAttribute(),
]
public sealed class Microsoft.OData.Client.KeyAttribute : System.Attribute, _Attribute {
	public KeyAttribute (string keyName)
	public KeyAttribute (string[] keyNames)

	System.Collections.ObjectModel.ReadOnlyCollection`1[[System.String]] KeyNames  { public get; }
}

[
DebuggerDisplayAttribute(),
]
public sealed class Microsoft.OData.Client.LinkDescriptor : Microsoft.OData.Client.Descriptor {
	object Source  { public get; }
	string SourceProperty  { public get; }
	object Target  { public get; }
}

public sealed class Microsoft.OData.Client.LinkInfo {
	System.Uri AssociationLink  { public get; }
	string Name  { public get; }
	System.Uri NavigationLink  { public get; }
}

public sealed class Microsoft.OData.Client.LoadCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
	Microsoft.OData.Client.QueryOperationResponse QueryOperationResponse  { public get; }
}

public sealed class Microsoft.OData.Client.MaterializedEntityArgs {
	public MaterializedEntityArgs (Microsoft.OData.ODataResource entry, object entity)

	object Entity  { public get; }
	Microsoft.OData.ODataResource Entry  { public get; }
}

[
AttributeUsageAttribute(),
]
public sealed class Microsoft.OData.Client.MediaEntryAttribute : System.Attribute, _Attribute {
	public MediaEntryAttribute (string mediaMemberName)

	string MediaMemberName  { public get; }
}

[
AttributeUsageAttribute(),
]
public sealed class Microsoft.OData.Client.MimeTypePropertyAttribute : System.Attribute, _Attribute {
	public MimeTypePropertyAttribute (string dataPropertyName, string mimeTypePropertyName)

	string DataPropertyName  { public get; }
	string MimeTypePropertyName  { public get; }
}

[
AttributeUsageAttribute(),
]
public sealed class Microsoft.OData.Client.NamedStreamAttribute : System.Attribute, _Attribute {
	public NamedStreamAttribute (string name)

	string Name  { public get; }
}

[
AttributeUsageAttribute(),
]
public sealed class Microsoft.OData.Client.OriginalNameAttribute : System.Attribute, _Attribute {
	public OriginalNameAttribute (string originalName)

	string OriginalName  { public get; }
}

public sealed class Microsoft.OData.Client.QueryOperationResponse`1 : Microsoft.OData.Client.QueryOperationResponse, IEnumerable`1, IEnumerable {
	long TotalCount  { public virtual get; }

	public DataServiceQueryContinuation`1 GetContinuation ()
	public virtual IEnumerator`1 GetEnumerator ()
}

public sealed class Microsoft.OData.Client.ReadingEntryArgs {
	public ReadingEntryArgs (Microsoft.OData.ODataResource entry)

	Microsoft.OData.ODataResource Entry  { public get; }
}

public sealed class Microsoft.OData.Client.ReadingFeedArgs {
	public ReadingFeedArgs (Microsoft.OData.ODataResourceSet feed)

	Microsoft.OData.ODataResourceSet Feed  { public get; }
}

public sealed class Microsoft.OData.Client.ReadingNestedResourceInfoArgs {
	public ReadingNestedResourceInfoArgs (Microsoft.OData.ODataNestedResourceInfo link)

	Microsoft.OData.ODataNestedResourceInfo Link  { public get; }
}

public sealed class Microsoft.OData.Client.StreamDescriptor : Microsoft.OData.Client.Descriptor {
	Microsoft.OData.Client.EntityDescriptor EntityDescriptor  { public get; public set; }
	Microsoft.OData.Client.DataServiceStreamLink StreamLink  { public get; }
}

public sealed class Microsoft.OData.Client.UriEntityOperationParameter : Microsoft.OData.Client.UriOperationParameter {
	public UriEntityOperationParameter (string name, object value)
	public UriEntityOperationParameter (string name, object value, bool useEntityReference)
}

public sealed class Microsoft.OData.Client.WritingEntityReferenceLinkArgs {
	public WritingEntityReferenceLinkArgs (Microsoft.OData.ODataEntityReferenceLink entityReferenceLink, object source, object target)

	Microsoft.OData.ODataEntityReferenceLink EntityReferenceLink  { public get; }
	object Source  { public get; }
	object Target  { public get; }
}

public sealed class Microsoft.OData.Client.WritingEntryArgs {
	public WritingEntryArgs (Microsoft.OData.ODataResource entry, object entity)

	object Entity  { public get; }
	Microsoft.OData.ODataResource Entry  { public get; }
}

public sealed class Microsoft.OData.Client.WritingNestedResourceInfoArgs {
	public WritingNestedResourceInfoArgs (Microsoft.OData.ODataNestedResourceInfo link, object source, object target)

	Microsoft.OData.ODataNestedResourceInfo Link  { public get; }
	object Source  { public get; }
	object Target  { public get; }
}

public enum Microsoft.OData.Client.ALinq.UriParser.QueryTokenKind : int {
	Aggregate = 24
	AggregateExpression = 25
	AggregateGroupBy = 26
	All = 19
	Any = 15
	BinaryOperator = 3
	Compute = 27
	ComputeExpression = 28
	CustomQueryOption = 9
	DottedIdentifier = 17
	EndPath = 7
	Expand = 13
	ExpandTerm = 20
	FunctionCall = 6
	FunctionParameter = 21
	FunctionParameterAlias = 22
	InnerPath = 16
	Literal = 5
	OrderBy = 8
	RangeVariable = 18
	Select = 10
	Star = 11
	StringLiteral = 23
	TypeSegment = 14
	UnaryOperator = 4
}

public interface Microsoft.OData.Client.ALinq.UriParser.IPathSegmentTokenVisitor {
	void Visit (Microsoft.OData.Client.ALinq.UriParser.NonSystemToken tokenIn)
	void Visit (Microsoft.OData.Client.ALinq.UriParser.SystemToken tokenIn)
}

public interface Microsoft.OData.Client.ALinq.UriParser.IPathSegmentTokenVisitor`1 {
	T Visit (Microsoft.OData.Client.ALinq.UriParser.NonSystemToken tokenIn)
	T Visit (Microsoft.OData.Client.ALinq.UriParser.SystemToken tokenIn)
}

public interface Microsoft.OData.Client.ALinq.UriParser.ISyntacticTreeVisitor`1 {
	T Visit (Microsoft.OData.Client.ALinq.UriParser.AggregateExpressionToken tokenIn)
	T Visit (Microsoft.OData.Client.ALinq.UriParser.AggregateToken tokenIn)
	T Visit (Microsoft.OData.Client.ALinq.UriParser.AllToken tokenIn)
	T Visit (Microsoft.OData.Client.ALinq.UriParser.AnyToken tokenIn)
	T Visit (Microsoft.OData.Client.ALinq.UriParser.BinaryOperatorToken tokenIn)
	T Visit (Microsoft.OData.Client.ALinq.UriParser.CustomQueryOptionToken tokenIn)
	T Visit (Microsoft.OData.Client.ALinq.UriParser.DottedIdentifierToken tokenIn)
	T Visit (Microsoft.OData.Client.ALinq.UriParser.EndPathToken tokenIn)
	T Visit (Microsoft.OData.Client.ALinq.UriParser.ExpandTermToken tokenIn)
	T Visit (Microsoft.OData.Client.ALinq.UriParser.ExpandToken tokenIn)
	T Visit (Microsoft.OData.Client.ALinq.UriParser.FunctionCallToken tokenIn)
	T Visit (Microsoft.OData.Client.ALinq.UriParser.FunctionParameterToken tokenIn)
	T Visit (Microsoft.OData.Client.ALinq.UriParser.GroupByToken tokenIn)
	T Visit (Microsoft.OData.Client.ALinq.UriParser.InnerPathToken tokenIn)
	T Visit (Microsoft.OData.Client.ALinq.UriParser.LambdaToken tokenIn)
	T Visit (Microsoft.OData.Client.ALinq.UriParser.LiteralToken tokenIn)
	T Visit (Microsoft.OData.Client.ALinq.UriParser.OrderByToken tokenIn)
	T Visit (Microsoft.OData.Client.ALinq.UriParser.RangeVariableToken tokenIn)
	T Visit (Microsoft.OData.Client.ALinq.UriParser.SelectToken tokenIn)
	T Visit (Microsoft.OData.Client.ALinq.UriParser.StarToken tokenIn)
	T Visit (Microsoft.OData.Client.ALinq.UriParser.UnaryOperatorToken tokenIn)
}

public abstract class Microsoft.OData.Client.ALinq.UriParser.ApplyTransformationToken : Microsoft.OData.Client.ALinq.UriParser.QueryToken {
	protected ApplyTransformationToken ()
}

public abstract class Microsoft.OData.Client.ALinq.UriParser.LambdaToken : Microsoft.OData.Client.ALinq.UriParser.QueryToken {
	protected LambdaToken (Microsoft.OData.Client.ALinq.UriParser.QueryToken expression, string parameter, Microsoft.OData.Client.ALinq.UriParser.QueryToken parent)

	Microsoft.OData.Client.ALinq.UriParser.QueryToken Expression  { public get; }
	string Parameter  { public get; }
	Microsoft.OData.Client.ALinq.UriParser.QueryToken Parent  { public get; }

	public virtual T Accept (ISyntacticTreeVisitor`1 visitor)
}

public abstract class Microsoft.OData.Client.ALinq.UriParser.PathSegmentToken {
	protected PathSegmentToken (Microsoft.OData.Client.ALinq.UriParser.PathSegmentToken nextToken)

	string Identifier  { public abstract get; }
	bool IsStructuralProperty  { public get; public set; }
	Microsoft.OData.Client.ALinq.UriParser.PathSegmentToken NextToken  { public get; }

	public abstract T Accept (IPathSegmentTokenVisitor`1 visitor)
	public abstract void Accept (Microsoft.OData.Client.ALinq.UriParser.IPathSegmentTokenVisitor visitor)
	public abstract bool IsNamespaceOrContainerQualified ()
}

public abstract class Microsoft.OData.Client.ALinq.UriParser.PathToken : Microsoft.OData.Client.ALinq.UriParser.QueryToken {
	protected PathToken ()

	string Identifier  { public abstract get; }
	Microsoft.OData.Client.ALinq.UriParser.QueryToken NextToken  { public abstract get; public abstract set; }
}

public abstract class Microsoft.OData.Client.ALinq.UriParser.QueryToken {
	public static readonly Microsoft.OData.Client.ALinq.UriParser.QueryToken[] EmptyTokens = Microsoft.OData.Client.ALinq.UriParser.QueryToken[]

	protected QueryToken ()

	Microsoft.OData.Client.ALinq.UriParser.QueryTokenKind Kind  { public abstract get; }

	public abstract T Accept (ISyntacticTreeVisitor`1 visitor)
}

public sealed class Microsoft.OData.Client.ALinq.UriParser.AggregateExpressionToken : Microsoft.OData.Client.ALinq.UriParser.QueryToken {
	public AggregateExpressionToken (Microsoft.OData.Client.ALinq.UriParser.QueryToken expression, Microsoft.OData.UriParser.Aggregation.AggregationMethod method, string alias)
	public AggregateExpressionToken (Microsoft.OData.Client.ALinq.UriParser.QueryToken expression, Microsoft.OData.UriParser.Aggregation.AggregationMethodDefinition methodDefinition, string alias)

	string Alias  { public get; }
	Microsoft.OData.Client.ALinq.UriParser.QueryToken Expression  { public get; }
	Microsoft.OData.Client.ALinq.UriParser.QueryTokenKind Kind  { public virtual get; }
	Microsoft.OData.UriParser.Aggregation.AggregationMethod Method  { public get; }
	Microsoft.OData.UriParser.Aggregation.AggregationMethodDefinition MethodDefinition  { public get; }

	public virtual T Accept (ISyntacticTreeVisitor`1 visitor)
}

public sealed class Microsoft.OData.Client.ALinq.UriParser.AggregateToken : Microsoft.OData.Client.ALinq.UriParser.ApplyTransformationToken {
	public AggregateToken (System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Client.ALinq.UriParser.AggregateExpressionToken]] expressions)

	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Client.ALinq.UriParser.AggregateExpressionToken]] Expressions  { public get; }
	Microsoft.OData.Client.ALinq.UriParser.QueryTokenKind Kind  { public virtual get; }

	public virtual T Accept (ISyntacticTreeVisitor`1 visitor)
}

public sealed class Microsoft.OData.Client.ALinq.UriParser.AllToken : Microsoft.OData.Client.ALinq.UriParser.LambdaToken {
	public AllToken (Microsoft.OData.Client.ALinq.UriParser.QueryToken expression, string parameter, Microsoft.OData.Client.ALinq.UriParser.QueryToken parent)

	Microsoft.OData.Client.ALinq.UriParser.QueryTokenKind Kind  { public virtual get; }

	public virtual T Accept (ISyntacticTreeVisitor`1 visitor)
}

public sealed class Microsoft.OData.Client.ALinq.UriParser.AnyToken : Microsoft.OData.Client.ALinq.UriParser.LambdaToken {
	public AnyToken (Microsoft.OData.Client.ALinq.UriParser.QueryToken expression, string parameter, Microsoft.OData.Client.ALinq.UriParser.QueryToken parent)

	Microsoft.OData.Client.ALinq.UriParser.QueryTokenKind Kind  { public virtual get; }

	public virtual T Accept (ISyntacticTreeVisitor`1 visitor)
}

public sealed class Microsoft.OData.Client.ALinq.UriParser.BinaryOperatorToken : Microsoft.OData.Client.ALinq.UriParser.QueryToken {
	public BinaryOperatorToken (Microsoft.OData.UriParser.BinaryOperatorKind operatorKind, Microsoft.OData.Client.ALinq.UriParser.QueryToken left, Microsoft.OData.Client.ALinq.UriParser.QueryToken right)

	Microsoft.OData.Client.ALinq.UriParser.QueryTokenKind Kind  { public virtual get; }
	Microsoft.OData.Client.ALinq.UriParser.QueryToken Left  { public get; }
	Microsoft.OData.UriParser.BinaryOperatorKind OperatorKind  { public get; }
	Microsoft.OData.Client.ALinq.UriParser.QueryToken Right  { public get; }

	public virtual T Accept (ISyntacticTreeVisitor`1 visitor)
}

public sealed class Microsoft.OData.Client.ALinq.UriParser.ComputeExpressionToken : Microsoft.OData.Client.ALinq.UriParser.QueryToken {
	public ComputeExpressionToken (Microsoft.OData.Client.ALinq.UriParser.QueryToken expression, string alias)

	string Alias  { public get; }
	Microsoft.OData.Client.ALinq.UriParser.QueryToken Expression  { public get; }
	Microsoft.OData.Client.ALinq.UriParser.QueryTokenKind Kind  { public virtual get; }

	public virtual T Accept (ISyntacticTreeVisitor`1 visitor)
}

public sealed class Microsoft.OData.Client.ALinq.UriParser.ComputeToken : Microsoft.OData.Client.ALinq.UriParser.ApplyTransformationToken {
	public ComputeToken (System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Client.ALinq.UriParser.ComputeExpressionToken]] expressions)

	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Client.ALinq.UriParser.ComputeExpressionToken]] Expressions  { public get; }
	Microsoft.OData.Client.ALinq.UriParser.QueryTokenKind Kind  { public virtual get; }

	public virtual T Accept (ISyntacticTreeVisitor`1 visitor)
}

public sealed class Microsoft.OData.Client.ALinq.UriParser.CustomQueryOptionToken : Microsoft.OData.Client.ALinq.UriParser.QueryToken {
	public CustomQueryOptionToken (string name, string value)

	Microsoft.OData.Client.ALinq.UriParser.QueryTokenKind Kind  { public virtual get; }
	string Name  { public get; }
	string Value  { public get; }

	public virtual T Accept (ISyntacticTreeVisitor`1 visitor)
}

public sealed class Microsoft.OData.Client.ALinq.UriParser.DottedIdentifierToken : Microsoft.OData.Client.ALinq.UriParser.PathToken {
	public DottedIdentifierToken (string identifier, Microsoft.OData.Client.ALinq.UriParser.QueryToken nextToken)

	string Identifier  { public virtual get; }
	Microsoft.OData.Client.ALinq.UriParser.QueryTokenKind Kind  { public virtual get; }
	Microsoft.OData.Client.ALinq.UriParser.QueryToken NextToken  { public virtual get; public virtual set; }

	public virtual T Accept (ISyntacticTreeVisitor`1 visitor)
}

public sealed class Microsoft.OData.Client.ALinq.UriParser.EndPathToken : Microsoft.OData.Client.ALinq.UriParser.PathToken {
	public EndPathToken (string identifier, Microsoft.OData.Client.ALinq.UriParser.QueryToken nextToken)

	string Identifier  { public virtual get; }
	Microsoft.OData.Client.ALinq.UriParser.QueryTokenKind Kind  { public virtual get; }
	Microsoft.OData.Client.ALinq.UriParser.QueryToken NextToken  { public virtual get; public virtual set; }

	public virtual T Accept (ISyntacticTreeVisitor`1 visitor)
}

public sealed class Microsoft.OData.Client.ALinq.UriParser.ExpandTermToken : Microsoft.OData.Client.ALinq.UriParser.QueryToken {
	public ExpandTermToken (Microsoft.OData.Client.ALinq.UriParser.PathSegmentToken pathToNavigationProp)
	public ExpandTermToken (Microsoft.OData.Client.ALinq.UriParser.PathSegmentToken pathToNavigationProp, Microsoft.OData.Client.ALinq.UriParser.SelectToken selectOption, Microsoft.OData.Client.ALinq.UriParser.ExpandToken expandOption)
	public ExpandTermToken (Microsoft.OData.Client.ALinq.UriParser.PathSegmentToken pathToNavigationProp, Microsoft.OData.Client.ALinq.UriParser.QueryToken filterOption, System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Client.ALinq.UriParser.OrderByToken]] orderByOptions, System.Nullable`1[[System.Int64]] topOption, System.Nullable`1[[System.Int64]] skipOption, System.Nullable`1[[System.Boolean]] countQueryOption, System.Nullable`1[[System.Int64]] levelsOption, Microsoft.OData.Client.ALinq.UriParser.QueryToken searchOption, Microsoft.OData.Client.ALinq.UriParser.SelectToken selectOption, Microsoft.OData.Client.ALinq.UriParser.ExpandToken expandOption)
	public ExpandTermToken (Microsoft.OData.Client.ALinq.UriParser.PathSegmentToken pathToNavigationProp, Microsoft.OData.Client.ALinq.UriParser.QueryToken filterOption, System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Client.ALinq.UriParser.OrderByToken]] orderByOptions, System.Nullable`1[[System.Int64]] topOption, System.Nullable`1[[System.Int64]] skipOption, System.Nullable`1[[System.Boolean]] countQueryOption, System.Nullable`1[[System.Int64]] levelsOption, Microsoft.OData.Client.ALinq.UriParser.QueryToken searchOption, Microsoft.OData.Client.ALinq.UriParser.SelectToken selectOption, Microsoft.OData.Client.ALinq.UriParser.ExpandToken expandOption, Microsoft.OData.Client.ALinq.UriParser.ComputeToken computeOption)

	Microsoft.OData.Client.ALinq.UriParser.ComputeToken ComputeOption  { public get; }
	System.Nullable`1[[System.Boolean]] CountQueryOption  { public get; }
	Microsoft.OData.Client.ALinq.UriParser.ExpandToken ExpandOption  { public get; }
	Microsoft.OData.Client.ALinq.UriParser.QueryToken FilterOption  { public get; }
	Microsoft.OData.Client.ALinq.UriParser.QueryTokenKind Kind  { public virtual get; }
	System.Nullable`1[[System.Int64]] LevelsOption  { public get; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Client.ALinq.UriParser.OrderByToken]] OrderByOptions  { public get; }
	Microsoft.OData.Client.ALinq.UriParser.PathSegmentToken PathToNavigationProp  { public get; }
	Microsoft.OData.Client.ALinq.UriParser.QueryToken SearchOption  { public get; }
	Microsoft.OData.Client.ALinq.UriParser.SelectToken SelectOption  { public get; }
	System.Nullable`1[[System.Int64]] SkipOption  { public get; }
	System.Nullable`1[[System.Int64]] TopOption  { public get; }

	public virtual T Accept (ISyntacticTreeVisitor`1 visitor)
}

public sealed class Microsoft.OData.Client.ALinq.UriParser.ExpandToken : Microsoft.OData.Client.ALinq.UriParser.QueryToken {
	public ExpandToken (System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Client.ALinq.UriParser.ExpandTermToken]] expandTerms)

	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Client.ALinq.UriParser.ExpandTermToken]] ExpandTerms  { public get; }
	Microsoft.OData.Client.ALinq.UriParser.QueryTokenKind Kind  { public virtual get; }

	public virtual T Accept (ISyntacticTreeVisitor`1 visitor)
}

public sealed class Microsoft.OData.Client.ALinq.UriParser.FunctionCallToken : Microsoft.OData.Client.ALinq.UriParser.QueryToken {
	public FunctionCallToken (string name, System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Client.ALinq.UriParser.QueryToken]] argumentValues)
	public FunctionCallToken (string name, System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Client.ALinq.UriParser.FunctionParameterToken]] arguments, Microsoft.OData.Client.ALinq.UriParser.QueryToken source)

	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Client.ALinq.UriParser.FunctionParameterToken]] Arguments  { public get; }
	Microsoft.OData.Client.ALinq.UriParser.QueryTokenKind Kind  { public virtual get; }
	string Name  { public get; }
	Microsoft.OData.Client.ALinq.UriParser.QueryToken Source  { public get; }

	public virtual T Accept (ISyntacticTreeVisitor`1 visitor)
}

public sealed class Microsoft.OData.Client.ALinq.UriParser.FunctionParameterToken : Microsoft.OData.Client.ALinq.UriParser.QueryToken {
	public static Microsoft.OData.Client.ALinq.UriParser.FunctionParameterToken[] EmptyParameterList = Microsoft.OData.Client.ALinq.UriParser.FunctionParameterToken[]

	public FunctionParameterToken (string parameterName, Microsoft.OData.Client.ALinq.UriParser.QueryToken valueToken)

	Microsoft.OData.Client.ALinq.UriParser.QueryTokenKind Kind  { public virtual get; }
	string ParameterName  { public get; }
	Microsoft.OData.Client.ALinq.UriParser.QueryToken ValueToken  { public get; }

	public virtual T Accept (ISyntacticTreeVisitor`1 visitor)
}

public sealed class Microsoft.OData.Client.ALinq.UriParser.GroupByToken : Microsoft.OData.Client.ALinq.UriParser.ApplyTransformationToken {
	public GroupByToken (System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Client.ALinq.UriParser.EndPathToken]] properties, Microsoft.OData.Client.ALinq.UriParser.ApplyTransformationToken child)

	Microsoft.OData.Client.ALinq.UriParser.ApplyTransformationToken Child  { public get; }
	Microsoft.OData.Client.ALinq.UriParser.QueryTokenKind Kind  { public virtual get; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Client.ALinq.UriParser.EndPathToken]] Properties  { public get; }

	public virtual T Accept (ISyntacticTreeVisitor`1 visitor)
}

public sealed class Microsoft.OData.Client.ALinq.UriParser.InnerPathToken : Microsoft.OData.Client.ALinq.UriParser.PathToken {
	public InnerPathToken (string identifier, Microsoft.OData.Client.ALinq.UriParser.QueryToken nextToken, System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Client.ALinq.UriParser.NamedValue]] namedValues)

	string Identifier  { public virtual get; }
	Microsoft.OData.Client.ALinq.UriParser.QueryTokenKind Kind  { public virtual get; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Client.ALinq.UriParser.NamedValue]] NamedValues  { public get; }
	Microsoft.OData.Client.ALinq.UriParser.QueryToken NextToken  { public virtual get; public virtual set; }

	public virtual T Accept (ISyntacticTreeVisitor`1 visitor)
}

public sealed class Microsoft.OData.Client.ALinq.UriParser.LiteralToken : Microsoft.OData.Client.ALinq.UriParser.QueryToken {
	public LiteralToken (object value)

	Microsoft.OData.Client.ALinq.UriParser.QueryTokenKind Kind  { public virtual get; }
	object Value  { public get; }

	public virtual T Accept (ISyntacticTreeVisitor`1 visitor)
}

public sealed class Microsoft.OData.Client.ALinq.UriParser.NamedValue {
	public NamedValue (string name, Microsoft.OData.Client.ALinq.UriParser.LiteralToken value)

	string Name  { public get; }
	Microsoft.OData.Client.ALinq.UriParser.LiteralToken Value  { public get; }
}

public sealed class Microsoft.OData.Client.ALinq.UriParser.NonSystemToken : Microsoft.OData.Client.ALinq.UriParser.PathSegmentToken {
	public NonSystemToken (string identifier, System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Client.ALinq.UriParser.NamedValue]] namedValues, Microsoft.OData.Client.ALinq.UriParser.PathSegmentToken nextToken)

	string Identifier  { public virtual get; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Client.ALinq.UriParser.NamedValue]] NamedValues  { public get; }

	public virtual T Accept (IPathSegmentTokenVisitor`1 visitor)
	public virtual void Accept (Microsoft.OData.Client.ALinq.UriParser.IPathSegmentTokenVisitor visitor)
	public virtual bool IsNamespaceOrContainerQualified ()
}

public sealed class Microsoft.OData.Client.ALinq.UriParser.OrderByToken : Microsoft.OData.Client.ALinq.UriParser.QueryToken {
	public OrderByToken (Microsoft.OData.Client.ALinq.UriParser.QueryToken expression, Microsoft.OData.UriParser.OrderByDirection direction)

	Microsoft.OData.UriParser.OrderByDirection Direction  { public get; }
	Microsoft.OData.Client.ALinq.UriParser.QueryToken Expression  { public get; }
	Microsoft.OData.Client.ALinq.UriParser.QueryTokenKind Kind  { public virtual get; }

	public virtual T Accept (ISyntacticTreeVisitor`1 visitor)
}

public sealed class Microsoft.OData.Client.ALinq.UriParser.RangeVariableToken : Microsoft.OData.Client.ALinq.UriParser.QueryToken {
	public RangeVariableToken (string name)

	Microsoft.OData.Client.ALinq.UriParser.QueryTokenKind Kind  { public virtual get; }
	string Name  { public get; }

	public virtual T Accept (ISyntacticTreeVisitor`1 visitor)
}

public sealed class Microsoft.OData.Client.ALinq.UriParser.SelectToken : Microsoft.OData.Client.ALinq.UriParser.QueryToken {
	public SelectToken (System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Client.ALinq.UriParser.PathSegmentToken]] properties)

	Microsoft.OData.Client.ALinq.UriParser.QueryTokenKind Kind  { public virtual get; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Client.ALinq.UriParser.PathSegmentToken]] Properties  { public get; }

	public virtual T Accept (ISyntacticTreeVisitor`1 visitor)
}

public sealed class Microsoft.OData.Client.ALinq.UriParser.StarToken : Microsoft.OData.Client.ALinq.UriParser.PathToken {
	public StarToken (Microsoft.OData.Client.ALinq.UriParser.QueryToken nextToken)

	string Identifier  { public virtual get; }
	Microsoft.OData.Client.ALinq.UriParser.QueryTokenKind Kind  { public virtual get; }
	Microsoft.OData.Client.ALinq.UriParser.QueryToken NextToken  { public virtual get; public virtual set; }

	public virtual T Accept (ISyntacticTreeVisitor`1 visitor)
}

public sealed class Microsoft.OData.Client.ALinq.UriParser.SystemToken : Microsoft.OData.Client.ALinq.UriParser.PathSegmentToken {
	public SystemToken (string identifier, Microsoft.OData.Client.ALinq.UriParser.PathSegmentToken nextToken)

	string Identifier  { public virtual get; }

	public virtual T Accept (IPathSegmentTokenVisitor`1 visitor)
	public virtual void Accept (Microsoft.OData.Client.ALinq.UriParser.IPathSegmentTokenVisitor visitor)
	public virtual bool IsNamespaceOrContainerQualified ()
}

public sealed class Microsoft.OData.Client.ALinq.UriParser.UnaryOperatorToken : Microsoft.OData.Client.ALinq.UriParser.QueryToken {
	public UnaryOperatorToken (Microsoft.OData.UriParser.UnaryOperatorKind operatorKind, Microsoft.OData.Client.ALinq.UriParser.QueryToken operand)

	Microsoft.OData.Client.ALinq.UriParser.QueryTokenKind Kind  { public virtual get; }
	Microsoft.OData.Client.ALinq.UriParser.QueryToken Operand  { public get; }
	Microsoft.OData.UriParser.UnaryOperatorKind OperatorKind  { public get; }

	public virtual T Accept (ISyntacticTreeVisitor`1 visitor)
}

