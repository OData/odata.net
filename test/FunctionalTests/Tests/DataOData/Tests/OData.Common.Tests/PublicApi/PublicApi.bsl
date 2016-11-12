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
	public abstract Microsoft.Spatial.SpatialPipeline CreateWriter (Microsoft.Data.Spatial.IGeoJsonWriter writer)
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

public interface Microsoft.Data.Spatial.IGeoJsonWriter {
	void AddPropertyName (string name)
	void AddValue (double value)
	void AddValue (string value)
	void EndArrayScope ()
	void EndObjectScope ()
	void StartArrayScope ()
	void StartObjectScope ()
}

public enum Microsoft.OData.Edm.EdmConcurrencyMode : int {
	Fixed = 1
	None = 0
}

public enum Microsoft.OData.Edm.EdmContainerElementKind : int {
	ActionImport = 2
	EntitySet = 1
	FunctionImport = 3
	None = 0
	Singleton = 4
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
	TypeDefinition = 1
	ValueTerm = 2
}

public enum Microsoft.OData.Edm.EdmTermKind : int {
	None = 0
	Type = 1
	Value = 2
}

public enum Microsoft.OData.Edm.EdmTypeKind : int {
	Collection = 4
	Complex = 3
	Entity = 2
	EntityReference = 5
	Enum = 6
	None = 0
	Primitive = 1
	TypeDefinition = 7
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

public interface Microsoft.OData.Edm.IEdmComplexType : IEdmElement, IEdmNamedElement, IEdmSchemaElement, IEdmSchemaType, IEdmStructuredType, IEdmTerm, IEdmType, IEdmVocabularyAnnotatable {
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
}

public interface Microsoft.OData.Edm.IEdmEntitySetBase : IEdmElement, IEdmNamedElement, IEdmNavigationSource {
}

public interface Microsoft.OData.Edm.IEdmEntityType : IEdmElement, IEdmNamedElement, IEdmSchemaElement, IEdmSchemaType, IEdmStructuredType, IEdmTerm, IEdmType, IEdmVocabularyAnnotatable {
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmStructuralProperty]] DeclaredKey  { public abstract get; }
	bool HasStream  { public abstract get; }
}

public interface Microsoft.OData.Edm.IEdmEntityTypeReference : IEdmElement, IEdmStructuredTypeReference, IEdmTypeReference {
}

public interface Microsoft.OData.Edm.IEdmEnumMember : IEdmElement, IEdmNamedElement, IEdmVocabularyAnnotatable {
	Microsoft.OData.Edm.IEdmEnumType DeclaringType  { public abstract get; }
	Microsoft.OData.Edm.Values.IEdmPrimitiveValue Value  { public abstract get; }
}

public interface Microsoft.OData.Edm.IEdmEnumType : IEdmElement, IEdmNamedElement, IEdmSchemaElement, IEdmSchemaType, IEdmType, IEdmVocabularyAnnotatable {
	bool IsFlags  { public abstract get; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmEnumMember]] Members  { public abstract get; }
	Microsoft.OData.Edm.IEdmPrimitiveType UnderlyingType  { public abstract get; }
}

public interface Microsoft.OData.Edm.IEdmEnumTypeReference : IEdmElement, IEdmTypeReference {
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
	Microsoft.OData.Edm.Annotations.IEdmDirectValueAnnotationsManager DirectValueAnnotationsManager  { public abstract get; }
	Microsoft.OData.Edm.IEdmEntityContainer EntityContainer  { public abstract get; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmModel]] ReferencedModels  { public abstract get; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmSchemaElement]] SchemaElements  { public abstract get; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Annotations.IEdmVocabularyAnnotation]] VocabularyAnnotations  { public abstract get; }

	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmOperation]] FindDeclaredBoundOperations (Microsoft.OData.Edm.IEdmType bindingType)
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmOperation]] FindDeclaredBoundOperations (string qualifiedName, Microsoft.OData.Edm.IEdmType bindingType)
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmOperation]] FindDeclaredOperations (string qualifiedName)
	Microsoft.OData.Edm.IEdmSchemaType FindDeclaredType (string qualifiedName)
	Microsoft.OData.Edm.IEdmValueTerm FindDeclaredValueTerm (string qualifiedName)
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Annotations.IEdmVocabularyAnnotation]] FindDeclaredVocabularyAnnotations (Microsoft.OData.Edm.IEdmVocabularyAnnotatable element)
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
	Microsoft.OData.Edm.IEdmNavigationSource Target  { public abstract get; }
}

public interface Microsoft.OData.Edm.IEdmNavigationSource : IEdmElement, IEdmNamedElement {
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmNavigationPropertyBinding]] NavigationPropertyBindings  { public abstract get; }
	Microsoft.OData.Edm.Expressions.IEdmPathExpression Path  { public abstract get; }
	Microsoft.OData.Edm.IEdmType Type  { public abstract get; }

	Microsoft.OData.Edm.IEdmNavigationSource FindNavigationTarget (Microsoft.OData.Edm.IEdmNavigationProperty navigationProperty)
}

public interface Microsoft.OData.Edm.IEdmOperation : IEdmElement, IEdmNamedElement, IEdmSchemaElement, IEdmVocabularyAnnotatable {
	Microsoft.OData.Edm.Expressions.IEdmPathExpression EntitySetPath  { public abstract get; }
	bool IsBound  { public abstract get; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmOperationParameter]] Parameters  { public abstract get; }
	Microsoft.OData.Edm.IEdmTypeReference ReturnType  { public abstract get; }

	Microsoft.OData.Edm.IEdmOperationParameter FindParameter (string name)
}

public interface Microsoft.OData.Edm.IEdmOperationImport : IEdmElement, IEdmEntityContainerElement, IEdmNamedElement, IEdmVocabularyAnnotatable {
	Microsoft.OData.Edm.Expressions.IEdmExpression EntitySet  { public abstract get; }
	Microsoft.OData.Edm.IEdmOperation Operation  { public abstract get; }
}

public interface Microsoft.OData.Edm.IEdmOperationParameter : IEdmElement, IEdmNamedElement, IEdmVocabularyAnnotatable {
	Microsoft.OData.Edm.IEdmOperation DeclaringOperation  { public abstract get; }
	Microsoft.OData.Edm.IEdmTypeReference Type  { public abstract get; }
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
	string Uri  { public abstract get; }
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
	Microsoft.OData.Edm.EdmConcurrencyMode ConcurrencyMode  { public abstract get; }
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

public interface Microsoft.OData.Edm.IEdmTerm : IEdmElement, IEdmNamedElement, IEdmSchemaElement, IEdmVocabularyAnnotatable {
	Microsoft.OData.Edm.EdmTermKind TermKind  { public abstract get; }
}

public interface Microsoft.OData.Edm.IEdmType : IEdmElement {
	Microsoft.OData.Edm.EdmTypeKind TypeKind  { public abstract get; }
}

public interface Microsoft.OData.Edm.IEdmTypeDefinition : IEdmElement, IEdmNamedElement, IEdmSchemaElement, IEdmSchemaType, IEdmType, IEdmVocabularyAnnotatable {
	Microsoft.OData.Edm.IEdmPrimitiveType UnderlyingType  { public abstract get; }
}

public interface Microsoft.OData.Edm.IEdmTypeDefinitionReference : IEdmElement, IEdmTypeReference {
}

public interface Microsoft.OData.Edm.IEdmTypeReference : IEdmElement {
	Microsoft.OData.Edm.IEdmType Definition  { public abstract get; }
	bool IsNullable  { public abstract get; }
}

public interface Microsoft.OData.Edm.IEdmUnknownEntitySet : IEdmElement, IEdmEntitySetBase, IEdmNamedElement, IEdmNavigationSource {
}

public interface Microsoft.OData.Edm.IEdmValueTerm : IEdmElement, IEdmNamedElement, IEdmSchemaElement, IEdmTerm, IEdmVocabularyAnnotatable {
	string AppliesTo  { public abstract get; }
	string DefaultValue  { public abstract get; }
	Microsoft.OData.Edm.IEdmTypeReference Type  { public abstract get; }
}

public interface Microsoft.OData.Edm.IEdmVocabularyAnnotatable : IEdmElement {
}

public abstract class Microsoft.OData.Edm.EdmLocation {
	protected EdmLocation ()

	public abstract string ToString ()
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
	public static bool IsOrInheritsFrom (Microsoft.OData.Edm.IEdmType thisType, Microsoft.OData.Edm.IEdmType otherType)

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
	public static bool IsStream (Microsoft.OData.Edm.IEdmTypeReference type)

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
	public static void AddAlternateKeyAnnotation (Microsoft.OData.Edm.Library.EdmModel model, Microsoft.OData.Edm.IEdmEntityType type, System.Collections.Generic.IDictionary`2[[System.String],[Microsoft.OData.Edm.IEdmProperty]] alternateKey)

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
	public static System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmNavigationProperty]] DeclaredNavigationProperties (Microsoft.OData.Edm.IEdmEntityType type)

	[
	ExtensionAttribute(),
	]
	public static System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmNavigationProperty]] DeclaredNavigationProperties (Microsoft.OData.Edm.IEdmEntityTypeReference type)

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
	public static System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Annotations.IEdmDirectValueAnnotation]] DirectValueAnnotations (Microsoft.OData.Edm.IEdmModel model, Microsoft.OData.Edm.IEdmElement element)

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
	public static Microsoft.OData.Edm.IEdmNavigationProperty FindNavigationProperty (Microsoft.OData.Edm.IEdmEntityTypeReference type, string name)

	[
	ExtensionAttribute(),
	]
	public static System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmOperation]] FindOperations (Microsoft.OData.Edm.IEdmModel model, string qualifiedName)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Edm.Expressions.IEdmPropertyConstructor FindProperty (Microsoft.OData.Edm.Expressions.IEdmRecordExpression expression, string name)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Edm.IEdmProperty FindProperty (Microsoft.OData.Edm.IEdmStructuredTypeReference type, string name)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Edm.IEdmSchemaType FindType (Microsoft.OData.Edm.IEdmModel model, string qualifiedName)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Edm.IEdmValueTerm FindValueTerm (Microsoft.OData.Edm.IEdmModel model, string qualifiedName)

	[
	ExtensionAttribute(),
	]
	public static System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Annotations.IEdmVocabularyAnnotation]] FindVocabularyAnnotations (Microsoft.OData.Edm.IEdmModel model, Microsoft.OData.Edm.IEdmVocabularyAnnotatable element)

	[
	ExtensionAttribute(),
	]
	public static IEnumerable`1 FindVocabularyAnnotations (Microsoft.OData.Edm.IEdmModel model, Microsoft.OData.Edm.IEdmVocabularyAnnotatable element, Microsoft.OData.Edm.IEdmTerm term)

	[
	ExtensionAttribute(),
	]
	public static IEnumerable`1 FindVocabularyAnnotations (Microsoft.OData.Edm.IEdmModel model, Microsoft.OData.Edm.IEdmVocabularyAnnotatable element, string termName)

	[
	ExtensionAttribute(),
	]
	public static IEnumerable`1 FindVocabularyAnnotations (Microsoft.OData.Edm.IEdmModel model, Microsoft.OData.Edm.IEdmVocabularyAnnotatable element, Microsoft.OData.Edm.IEdmTerm term, string qualifier)

	[
	ExtensionAttribute(),
	]
	public static IEnumerable`1 FindVocabularyAnnotations (Microsoft.OData.Edm.IEdmModel model, Microsoft.OData.Edm.IEdmVocabularyAnnotatable element, string termName, string qualifier)

	[
	ExtensionAttribute(),
	]
	public static System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Annotations.IEdmVocabularyAnnotation]] FindVocabularyAnnotationsIncludingInheritedAnnotations (Microsoft.OData.Edm.IEdmModel model, Microsoft.OData.Edm.IEdmVocabularyAnnotatable element)

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
	public static object[] GetAnnotationValues (Microsoft.OData.Edm.IEdmModel model, System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Annotations.IEdmDirectValueAnnotationBinding]] annotations)

	[
	ExtensionAttribute(),
	]
	public static string GetDescriptionAnnotation (Microsoft.OData.Edm.IEdmModel model, Microsoft.OData.Edm.IEdmVocabularyAnnotatable target)

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
	public static string GetLongDescriptionAnnotation (Microsoft.OData.Edm.IEdmModel model, Microsoft.OData.Edm.IEdmVocabularyAnnotatable target)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Edm.PrimitiveValueConverters.IPrimitiveValueConverter GetPrimitiveValueConverter (Microsoft.OData.Edm.IEdmModel model, Microsoft.OData.Edm.IEdmTypeReference type)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Edm.Values.IEdmValue GetTermValue (Microsoft.OData.Edm.IEdmModel model, Microsoft.OData.Edm.IEdmVocabularyAnnotatable element, Microsoft.OData.Edm.IEdmValueTerm term, Microsoft.OData.Edm.Evaluation.EdmExpressionEvaluator expressionEvaluator)

	[
	ExtensionAttribute(),
	]
	public static T GetTermValue (Microsoft.OData.Edm.IEdmModel model, Microsoft.OData.Edm.IEdmVocabularyAnnotatable element, Microsoft.OData.Edm.IEdmValueTerm term, Microsoft.OData.Edm.Evaluation.EdmToClrEvaluator evaluator)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Edm.Values.IEdmValue GetTermValue (Microsoft.OData.Edm.IEdmModel model, Microsoft.OData.Edm.IEdmVocabularyAnnotatable element, string termName, Microsoft.OData.Edm.Evaluation.EdmExpressionEvaluator expressionEvaluator)

	[
	ExtensionAttribute(),
	]
	public static T GetTermValue (Microsoft.OData.Edm.IEdmModel model, Microsoft.OData.Edm.IEdmVocabularyAnnotatable element, string termName, Microsoft.OData.Edm.Evaluation.EdmToClrEvaluator evaluator)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Edm.Values.IEdmValue GetTermValue (Microsoft.OData.Edm.IEdmModel model, Microsoft.OData.Edm.Values.IEdmStructuredValue context, Microsoft.OData.Edm.IEdmValueTerm term, Microsoft.OData.Edm.Evaluation.EdmExpressionEvaluator expressionEvaluator)

	[
	ExtensionAttribute(),
	]
	public static T GetTermValue (Microsoft.OData.Edm.IEdmModel model, Microsoft.OData.Edm.Values.IEdmStructuredValue context, Microsoft.OData.Edm.IEdmValueTerm term, Microsoft.OData.Edm.Evaluation.EdmToClrEvaluator evaluator)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Edm.Values.IEdmValue GetTermValue (Microsoft.OData.Edm.IEdmModel model, Microsoft.OData.Edm.Values.IEdmStructuredValue context, string termName, Microsoft.OData.Edm.Evaluation.EdmExpressionEvaluator expressionEvaluator)

	[
	ExtensionAttribute(),
	]
	public static T GetTermValue (Microsoft.OData.Edm.IEdmModel model, Microsoft.OData.Edm.Values.IEdmStructuredValue context, string termName, Microsoft.OData.Edm.Evaluation.EdmToClrEvaluator evaluator)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Edm.Values.IEdmValue GetTermValue (Microsoft.OData.Edm.IEdmModel model, Microsoft.OData.Edm.IEdmVocabularyAnnotatable element, Microsoft.OData.Edm.IEdmValueTerm term, string qualifier, Microsoft.OData.Edm.Evaluation.EdmExpressionEvaluator expressionEvaluator)

	[
	ExtensionAttribute(),
	]
	public static T GetTermValue (Microsoft.OData.Edm.IEdmModel model, Microsoft.OData.Edm.IEdmVocabularyAnnotatable element, Microsoft.OData.Edm.IEdmValueTerm term, string qualifier, Microsoft.OData.Edm.Evaluation.EdmToClrEvaluator evaluator)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Edm.Values.IEdmValue GetTermValue (Microsoft.OData.Edm.IEdmModel model, Microsoft.OData.Edm.IEdmVocabularyAnnotatable element, string termName, string qualifier, Microsoft.OData.Edm.Evaluation.EdmExpressionEvaluator expressionEvaluator)

	[
	ExtensionAttribute(),
	]
	public static T GetTermValue (Microsoft.OData.Edm.IEdmModel model, Microsoft.OData.Edm.IEdmVocabularyAnnotatable element, string termName, string qualifier, Microsoft.OData.Edm.Evaluation.EdmToClrEvaluator evaluator)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Edm.Values.IEdmValue GetTermValue (Microsoft.OData.Edm.IEdmModel model, Microsoft.OData.Edm.Values.IEdmStructuredValue context, Microsoft.OData.Edm.IEdmValueTerm term, string qualifier, Microsoft.OData.Edm.Evaluation.EdmExpressionEvaluator expressionEvaluator)

	[
	ExtensionAttribute(),
	]
	public static T GetTermValue (Microsoft.OData.Edm.IEdmModel model, Microsoft.OData.Edm.Values.IEdmStructuredValue context, Microsoft.OData.Edm.IEdmValueTerm term, string qualifier, Microsoft.OData.Edm.Evaluation.EdmToClrEvaluator evaluator)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Edm.Values.IEdmValue GetTermValue (Microsoft.OData.Edm.IEdmModel model, Microsoft.OData.Edm.Values.IEdmStructuredValue context, string termName, string qualifier, Microsoft.OData.Edm.Evaluation.EdmExpressionEvaluator expressionEvaluator)

	[
	ExtensionAttribute(),
	]
	public static T GetTermValue (Microsoft.OData.Edm.IEdmModel model, Microsoft.OData.Edm.Values.IEdmStructuredValue context, string termName, string qualifier, Microsoft.OData.Edm.Evaluation.EdmToClrEvaluator evaluator)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Edm.IEdmTypeDefinitionReference GetUInt16 (Microsoft.OData.Edm.Library.EdmModel model, string namespaceName, bool isNullable)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Edm.IEdmTypeDefinitionReference GetUInt32 (Microsoft.OData.Edm.Library.EdmModel model, string namespaceName, bool isNullable)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Edm.IEdmTypeDefinitionReference GetUInt64 (Microsoft.OData.Edm.Library.EdmModel model, string namespaceName, bool isNullable)

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
	public static System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmNavigationProperty]] NavigationProperties (Microsoft.OData.Edm.IEdmEntityType type)

	[
	ExtensionAttribute(),
	]
	public static System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmNavigationProperty]] NavigationProperties (Microsoft.OData.Edm.IEdmEntityTypeReference type)

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
	public static void SetAnnotationValues (Microsoft.OData.Edm.IEdmModel model, System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Annotations.IEdmDirectValueAnnotationBinding]] annotations)

	[
	ExtensionAttribute(),
	]
	public static void SetChangeTrackingAnnotation (Microsoft.OData.Edm.Library.EdmModel model, Microsoft.OData.Edm.IEdmEntityContainer target, bool isSupported)

	[
	ExtensionAttribute(),
	]
	public static void SetChangeTrackingAnnotation (Microsoft.OData.Edm.Library.EdmModel model, Microsoft.OData.Edm.IEdmEntitySet target, bool isSupported, System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmStructuralProperty]] filterableProperties, System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmNavigationProperty]] expandableProperties)

	[
	ExtensionAttribute(),
	]
	public static void SetDescriptionAnnotation (Microsoft.OData.Edm.Library.EdmModel model, Microsoft.OData.Edm.IEdmVocabularyAnnotatable target, string description)

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
	public static void SetLongDescriptionAnnotation (Microsoft.OData.Edm.Library.EdmModel model, Microsoft.OData.Edm.IEdmVocabularyAnnotatable target, string description)

	[
	ExtensionAttribute(),
	]
	public static void SetOptimisticConcurrencyAnnotation (Microsoft.OData.Edm.Library.EdmModel model, Microsoft.OData.Edm.IEdmEntitySet target, System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmStructuralProperty]] properties)

	[
	ObsoleteAttribute(),
	ExtensionAttribute(),
	]
	public static void SetOptimisticConcurrencyControlAnnotation (Microsoft.OData.Edm.Library.EdmModel model, Microsoft.OData.Edm.IEdmEntitySet target, System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmStructuralProperty]] properties)

	[
	ExtensionAttribute(),
	]
	public static void SetPrimitiveValueConverter (Microsoft.OData.Edm.IEdmModel model, Microsoft.OData.Edm.IEdmTypeDefinitionReference typeDefinition, Microsoft.OData.Edm.PrimitiveValueConverters.IPrimitiveValueConverter converter)

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
	public static Microsoft.OData.Edm.IEdmEntityType ToEntityType (Microsoft.OData.Edm.IEdmNavigationProperty property)

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
	public static bool TryGetRelativeEntitySetPath (Microsoft.OData.Edm.IEdmOperationImport operationImport, Microsoft.OData.Edm.IEdmModel model, out Microsoft.OData.Edm.IEdmOperationParameter& parameter, out System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmNavigationProperty]]& relativePath, out System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Validation.EdmError]]& edmErrors)

	[
	ExtensionAttribute(),
	]
	public static bool TryGetRelativeEntitySetPath (Microsoft.OData.Edm.IEdmOperation operation, Microsoft.OData.Edm.IEdmModel model, out Microsoft.OData.Edm.IEdmOperationParameter& parameter, out System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmNavigationProperty]]& relativePath, out Microsoft.OData.Edm.IEdmEntityType& lastEntityType, out System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Validation.EdmError]]& errors)

	[
	ExtensionAttribute(),
	]
	public static bool TryGetStaticEntitySet (Microsoft.OData.Edm.IEdmOperationImport operationImport, out Microsoft.OData.Edm.IEdmEntitySet& entitySet)

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
	public static Microsoft.OData.Edm.IEdmValueTerm ValueTerm (Microsoft.OData.Edm.Annotations.IEdmValueAnnotation annotation)

	[
	ExtensionAttribute(),
	]
	public static System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Annotations.IEdmVocabularyAnnotation]] VocabularyAnnotations (Microsoft.OData.Edm.IEdmVocabularyAnnotatable element, Microsoft.OData.Edm.IEdmModel model)
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

public class Microsoft.OData.Edm.EdmReferentialConstraintPropertyPair {
	public EdmReferentialConstraintPropertyPair (Microsoft.OData.Edm.IEdmStructuralProperty dependentProperty, Microsoft.OData.Edm.IEdmStructuralProperty principalProperty)

	Microsoft.OData.Edm.IEdmStructuralProperty DependentProperty  { public get; }
	Microsoft.OData.Edm.IEdmStructuralProperty PrincipalProperty  { public get; }
}

public interface Microsoft.OData.Edm.Annotations.IEdmDirectValueAnnotation : IEdmElement, IEdmNamedElement {
	string NamespaceUri  { public abstract get; }
	object Value  { public abstract get; }
}

public interface Microsoft.OData.Edm.Annotations.IEdmDirectValueAnnotationBinding {
	Microsoft.OData.Edm.IEdmElement Element  { public abstract get; }
	string Name  { public abstract get; }
	string NamespaceUri  { public abstract get; }
	object Value  { public abstract get; }
}

public interface Microsoft.OData.Edm.Annotations.IEdmDirectValueAnnotationsManager {
	object GetAnnotationValue (Microsoft.OData.Edm.IEdmElement element, string namespaceName, string localName)
	object[] GetAnnotationValues (System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Annotations.IEdmDirectValueAnnotationBinding]] annotations)
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Annotations.IEdmDirectValueAnnotation]] GetDirectValueAnnotations (Microsoft.OData.Edm.IEdmElement element)
	void SetAnnotationValue (Microsoft.OData.Edm.IEdmElement element, string namespaceName, string localName, object value)
	void SetAnnotationValues (System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Annotations.IEdmDirectValueAnnotationBinding]] annotations)
}

public interface Microsoft.OData.Edm.Annotations.IEdmPropertyValueBinding : IEdmElement {
	Microsoft.OData.Edm.IEdmProperty BoundProperty  { public abstract get; }
	Microsoft.OData.Edm.Expressions.IEdmExpression Value  { public abstract get; }
}

public interface Microsoft.OData.Edm.Annotations.IEdmValueAnnotation : IEdmElement, IEdmVocabularyAnnotation {
	Microsoft.OData.Edm.Expressions.IEdmExpression Value  { public abstract get; }
}

public interface Microsoft.OData.Edm.Annotations.IEdmVocabularyAnnotation : IEdmElement {
	string Qualifier  { public abstract get; }
	Microsoft.OData.Edm.IEdmVocabularyAnnotatable Target  { public abstract get; }
	Microsoft.OData.Edm.IEdmTerm Term  { public abstract get; }
}

public enum Microsoft.OData.Edm.Csdl.EdmVocabularyAnnotationSerializationLocation : int {
	Inline = 0
	OutOfLine = 1
}

public enum Microsoft.OData.Edm.Csdl.EdmxTarget : int {
	EntityFramework = 0
	OData = 1
}

public sealed class Microsoft.OData.Edm.Csdl.CsdlConstants {
	public static readonly System.Version EdmxVersion4 = 4.0
	public static readonly System.Version EdmxVersionLatest = 4.0
}

public sealed class Microsoft.OData.Edm.Csdl.CsdlReader {
	public static bool TryParse (System.Collections.Generic.IEnumerable`1[[System.Xml.XmlReader]] readers, out Microsoft.OData.Edm.IEdmModel& model, out System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Validation.EdmError]]& errors)
	public static bool TryParse (System.Collections.Generic.IEnumerable`1[[System.Xml.XmlReader]] readers, Microsoft.OData.Edm.IEdmModel reference, out Microsoft.OData.Edm.IEdmModel& model, out System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Validation.EdmError]]& errors)
	public static bool TryParse (System.Collections.Generic.IEnumerable`1[[System.Xml.XmlReader]] readers, System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmModel]] references, out Microsoft.OData.Edm.IEdmModel& model, out System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Validation.EdmError]]& errors)
}

[
ExtensionAttribute(),
]
public sealed class Microsoft.OData.Edm.Csdl.CsdlWriter {
	[
	ExtensionAttribute(),
	]
	public static bool TryWriteCsdl (Microsoft.OData.Edm.IEdmModel model, System.Func`2[[System.String],[System.Xml.XmlWriter]] writerProvider, out System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Validation.EdmError]]& errors)

	[
	ExtensionAttribute(),
	]
	public static bool TryWriteCsdl (Microsoft.OData.Edm.IEdmModel model, System.Xml.XmlWriter writer, out System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Validation.EdmError]]& errors)
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
	public static string GetSchemaNamespace (Microsoft.OData.Edm.Annotations.IEdmVocabularyAnnotation annotation, Microsoft.OData.Edm.IEdmModel model)

	[
	ExtensionAttribute(),
	]
	public static System.Nullable`1[[Microsoft.OData.Edm.Csdl.EdmVocabularyAnnotationSerializationLocation]] GetSerializationLocation (Microsoft.OData.Edm.Annotations.IEdmVocabularyAnnotation annotation, Microsoft.OData.Edm.IEdmModel model)

	[
	ExtensionAttribute(),
	]
	public static bool IsSerializedAsElement (Microsoft.OData.Edm.Values.IEdmValue value, Microsoft.OData.Edm.IEdmModel model)

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
	public static void SetIsSerializedAsElement (Microsoft.OData.Edm.Values.IEdmValue value, Microsoft.OData.Edm.IEdmModel model, bool isSerializedAsElement)

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
	public static void SetSchemaNamespace (Microsoft.OData.Edm.Annotations.IEdmVocabularyAnnotation annotation, Microsoft.OData.Edm.IEdmModel model, string schemaNamespace)

	[
	ExtensionAttribute(),
	]
	public static void SetSerializationLocation (Microsoft.OData.Edm.Annotations.IEdmVocabularyAnnotation annotation, Microsoft.OData.Edm.IEdmModel model, System.Nullable`1[[Microsoft.OData.Edm.Csdl.EdmVocabularyAnnotationSerializationLocation]] location)
}

public class Microsoft.OData.Edm.Csdl.CsdlLocation : Microsoft.OData.Edm.EdmLocation {
	int LineNumber  { public get; }
	int LinePosition  { public get; }
	string Source  { public get; }

	public virtual string ToString ()
}

[
DebuggerDisplayAttribute(),
]
public class Microsoft.OData.Edm.Csdl.EdmParseException : System.Exception, _Exception, ISerializable {
	public EdmParseException (System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Validation.EdmError]] parseErrors)

	System.Collections.ObjectModel.ReadOnlyCollection`1[[Microsoft.OData.Edm.Validation.EdmError]] Errors  { public get; }
}

public class Microsoft.OData.Edm.Csdl.EdmxReader {
	public static Microsoft.OData.Edm.IEdmModel Parse (System.Xml.XmlReader reader)
	public static Microsoft.OData.Edm.IEdmModel Parse (System.Xml.XmlReader reader, Microsoft.OData.Edm.IEdmModel referencedModel)
	public static Microsoft.OData.Edm.IEdmModel Parse (System.Xml.XmlReader reader, System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmModel]] referencedModels)
	public static Microsoft.OData.Edm.IEdmModel Parse (System.Xml.XmlReader reader, System.Func`2[[System.Uri],[System.Xml.XmlReader]] getReferencedModelReaderFunc)
	public static bool TryParse (System.Xml.XmlReader reader, out Microsoft.OData.Edm.IEdmModel& model, out System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Validation.EdmError]]& errors)
	public static bool TryParse (System.Xml.XmlReader reader, Microsoft.OData.Edm.IEdmModel reference, out Microsoft.OData.Edm.IEdmModel& model, out System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Validation.EdmError]]& errors)
	public static bool TryParse (System.Xml.XmlReader reader, bool ignoreUnexpectedAttributesAndElements, out Microsoft.OData.Edm.IEdmModel& model, out System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Validation.EdmError]]& errors)
	public static bool TryParse (System.Xml.XmlReader reader, System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmModel]] references, out Microsoft.OData.Edm.IEdmModel& model, out System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Validation.EdmError]]& errors)
	public static bool TryParse (System.Xml.XmlReader reader, System.Func`2[[System.Uri],[System.Xml.XmlReader]] getReferencedModelReaderFunc, out Microsoft.OData.Edm.IEdmModel& model, out System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Validation.EdmError]]& errors)
	public static bool TryParse (System.Xml.XmlReader reader, System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmModel]] references, Microsoft.OData.Edm.Csdl.EdmxReaderSettings settings, out Microsoft.OData.Edm.IEdmModel& model, out System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Validation.EdmError]]& errors)
}

public class Microsoft.OData.Edm.Csdl.EdmxWriter {
	public static bool TryWriteEdmx (Microsoft.OData.Edm.IEdmModel model, System.Xml.XmlWriter writer, Microsoft.OData.Edm.Csdl.EdmxTarget target, out System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Validation.EdmError]]& errors)
}

public sealed class Microsoft.OData.Edm.Csdl.EdmxReaderSettings {
	public EdmxReaderSettings ()

	System.Func`2[[System.Uri],[System.Xml.XmlReader]] GetReferencedModelReaderFunc  { public get; public set; }
	bool IgnoreUnexpectedAttributesAndElements  { public get; public set; }
}

public class Microsoft.OData.Edm.EdmToClrConversion.EdmToClrConverter {
	public EdmToClrConverter ()
	public EdmToClrConverter (Microsoft.OData.Edm.EdmToClrConversion.TryCreateObjectInstance tryCreateObjectInstanceDelegate)
	public EdmToClrConverter (Microsoft.OData.Edm.EdmToClrConversion.TryCreateObjectInstance tryCreateObjectInstanceDelegate, Microsoft.OData.Edm.EdmToClrConversion.TryGetClrPropertyInfo tryGetClrPropertyInfoDelegate, Microsoft.OData.Edm.EdmToClrConversion.TryGetClrTypeName tryGetClrTypeNameDelegate)

	public T AsClrValue (Microsoft.OData.Edm.Values.IEdmValue edmValue)
	public object AsClrValue (Microsoft.OData.Edm.Values.IEdmValue edmValue, System.Type clrType)
	public void RegisterConvertedObject (Microsoft.OData.Edm.Values.IEdmStructuredValue edmValue, object clrObject)
}

public sealed class Microsoft.OData.Edm.EdmToClrConversion.TryCreateObjectInstance : System.MulticastDelegate, ICloneable, ISerializable {
	public TryCreateObjectInstance (object object, System.IntPtr method)

	public virtual System.IAsyncResult BeginInvoke (Microsoft.OData.Edm.Values.IEdmStructuredValue edmValue, System.Type clrType, Microsoft.OData.Edm.EdmToClrConversion.EdmToClrConverter converter, out System.Object& objectInstance, out System.Boolean& objectInstanceInitialized, System.AsyncCallback callback, object object)
	public virtual bool EndInvoke (out System.Object& objectInstance, out System.Boolean& objectInstanceInitialized, System.IAsyncResult result)
	public virtual bool Invoke (Microsoft.OData.Edm.Values.IEdmStructuredValue edmValue, System.Type clrType, Microsoft.OData.Edm.EdmToClrConversion.EdmToClrConverter converter, out System.Object& objectInstance, out System.Boolean& objectInstanceInitialized)
}

public sealed class Microsoft.OData.Edm.EdmToClrConversion.TryGetClrPropertyInfo : System.MulticastDelegate, ICloneable, ISerializable {
	public TryGetClrPropertyInfo (object object, System.IntPtr method)

	public virtual System.IAsyncResult BeginInvoke (System.Type clrType, string edmName, out System.Reflection.PropertyInfo& propertyInfo, System.AsyncCallback callback, object object)
	public virtual bool EndInvoke (out System.Reflection.PropertyInfo& propertyInfo, System.IAsyncResult result)
	public virtual bool Invoke (System.Type clrType, string edmName, out System.Reflection.PropertyInfo& propertyInfo)
}

public sealed class Microsoft.OData.Edm.EdmToClrConversion.TryGetClrTypeName : System.MulticastDelegate, ICloneable, ISerializable {
	public TryGetClrTypeName (object object, System.IntPtr method)

	public virtual System.IAsyncResult BeginInvoke (Microsoft.OData.Edm.IEdmModel edmModel, string edmTypeName, out System.String& clrTypeName, System.AsyncCallback callback, object object)
	public virtual bool EndInvoke (out System.String& clrTypeName, System.IAsyncResult result)
	public virtual bool Invoke (Microsoft.OData.Edm.IEdmModel edmModel, string edmTypeName, out System.String& clrTypeName)
}

public class Microsoft.OData.Edm.Evaluation.EdmExpressionEvaluator {
	public EdmExpressionEvaluator (System.Collections.Generic.IDictionary`2[[Microsoft.OData.Edm.IEdmOperation],[System.Func`2[[Microsoft.OData.Edm.Values.IEdmValue[]],[Microsoft.OData.Edm.Values.IEdmValue]]]] builtInFunctions)
	public EdmExpressionEvaluator (System.Collections.Generic.IDictionary`2[[Microsoft.OData.Edm.IEdmOperation],[System.Func`2[[Microsoft.OData.Edm.Values.IEdmValue[]],[Microsoft.OData.Edm.Values.IEdmValue]]]] builtInFunctions, System.Func`3[[System.String],[Microsoft.OData.Edm.Values.IEdmValue[]],[Microsoft.OData.Edm.Values.IEdmValue]] lastChanceOperationApplier)
	public EdmExpressionEvaluator (System.Collections.Generic.IDictionary`2[[Microsoft.OData.Edm.IEdmOperation],[System.Func`2[[Microsoft.OData.Edm.Values.IEdmValue[]],[Microsoft.OData.Edm.Values.IEdmValue]]]] builtInFunctions, System.Func`3[[System.String],[Microsoft.OData.Edm.Values.IEdmValue[]],[Microsoft.OData.Edm.Values.IEdmValue]] lastChanceOperationApplier, System.Func`5[[Microsoft.OData.Edm.IEdmModel],[Microsoft.OData.Edm.IEdmType],[System.String],[System.String],[Microsoft.OData.Edm.Expressions.IEdmExpression]] getAnnotationExpressionForType, System.Func`6[[Microsoft.OData.Edm.IEdmModel],[Microsoft.OData.Edm.IEdmType],[System.String],[System.String],[System.String],[Microsoft.OData.Edm.Expressions.IEdmExpression]] getAnnotationExpressionForProperty, Microsoft.OData.Edm.IEdmModel edmModel)

	System.Func`3[[System.String],[Microsoft.OData.Edm.IEdmModel],[Microsoft.OData.Edm.IEdmType]] ResolveTypeFromName  { protected get; protected set; }

	public Microsoft.OData.Edm.Values.IEdmValue Evaluate (Microsoft.OData.Edm.Expressions.IEdmExpression expression)
	public Microsoft.OData.Edm.Values.IEdmValue Evaluate (Microsoft.OData.Edm.Expressions.IEdmExpression expression, Microsoft.OData.Edm.Values.IEdmStructuredValue context)
	public Microsoft.OData.Edm.Values.IEdmValue Evaluate (Microsoft.OData.Edm.Expressions.IEdmExpression expression, Microsoft.OData.Edm.Values.IEdmStructuredValue context, Microsoft.OData.Edm.IEdmTypeReference targetType)
	protected static Microsoft.OData.Edm.IEdmType FindEdmType (string edmTypeName, Microsoft.OData.Edm.IEdmModel edmModel)
}

public class Microsoft.OData.Edm.Evaluation.EdmToClrEvaluator : Microsoft.OData.Edm.Evaluation.EdmExpressionEvaluator {
	public EdmToClrEvaluator (System.Collections.Generic.IDictionary`2[[Microsoft.OData.Edm.IEdmOperation],[System.Func`2[[Microsoft.OData.Edm.Values.IEdmValue[]],[Microsoft.OData.Edm.Values.IEdmValue]]]] builtInFunctions)
	public EdmToClrEvaluator (System.Collections.Generic.IDictionary`2[[Microsoft.OData.Edm.IEdmOperation],[System.Func`2[[Microsoft.OData.Edm.Values.IEdmValue[]],[Microsoft.OData.Edm.Values.IEdmValue]]]] builtInFunctions, System.Func`3[[System.String],[Microsoft.OData.Edm.Values.IEdmValue[]],[Microsoft.OData.Edm.Values.IEdmValue]] lastChanceOperationApplier)
	public EdmToClrEvaluator (System.Collections.Generic.IDictionary`2[[Microsoft.OData.Edm.IEdmOperation],[System.Func`2[[Microsoft.OData.Edm.Values.IEdmValue[]],[Microsoft.OData.Edm.Values.IEdmValue]]]] builtInFunctions, System.Func`3[[System.String],[Microsoft.OData.Edm.Values.IEdmValue[]],[Microsoft.OData.Edm.Values.IEdmValue]] lastChanceOperationApplier, System.Func`5[[Microsoft.OData.Edm.IEdmModel],[Microsoft.OData.Edm.IEdmType],[System.String],[System.String],[Microsoft.OData.Edm.Expressions.IEdmExpression]] getAnnotationExpressionForType, System.Func`6[[Microsoft.OData.Edm.IEdmModel],[Microsoft.OData.Edm.IEdmType],[System.String],[System.String],[System.String],[Microsoft.OData.Edm.Expressions.IEdmExpression]] getAnnotationExpressionForProperty, Microsoft.OData.Edm.IEdmModel edmModel)

	Microsoft.OData.Edm.EdmToClrConversion.EdmToClrConverter EdmToClrConverter  { public get; public set; }

	public T EvaluateToClrValue (Microsoft.OData.Edm.Expressions.IEdmExpression expression)
	public T EvaluateToClrValue (Microsoft.OData.Edm.Expressions.IEdmExpression expression, Microsoft.OData.Edm.Values.IEdmStructuredValue context)
	public T EvaluateToClrValue (Microsoft.OData.Edm.Expressions.IEdmExpression expression, Microsoft.OData.Edm.Values.IEdmStructuredValue context, Microsoft.OData.Edm.IEdmTypeReference targetType)
}

public enum Microsoft.OData.Edm.Expressions.EdmExpressionKind : int {
	BinaryConstant = 1
	BooleanConstant = 2
	Cast = 21
	Collection = 12
	DateConstant = 28
	DateTimeOffsetConstant = 3
	DecimalConstant = 4
	DurationConstant = 9
	EntitySetReference = 18
	EnumMember = 30
	EnumMemberReference = 19
	FloatingConstant = 5
	GuidConstant = 6
	If = 20
	IntegerConstant = 7
	IsType = 22
	Labeled = 25
	LabeledExpressionReference = 24
	NavigationPropertyPath = 27
	None = 0
	Null = 10
	OperationApplication = 23
	OperationReference = 15
	ParameterReference = 14
	Path = 13
	PropertyPath = 26
	PropertyReference = 16
	Record = 11
	StringConstant = 8
	TimeOfDayConstant = 29
	ValueTermReference = 17
}

public interface Microsoft.OData.Edm.Expressions.IEdmApplyExpression : IEdmElement, IEdmExpression {
	Microsoft.OData.Edm.Expressions.IEdmExpression AppliedOperation  { public abstract get; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Expressions.IEdmExpression]] Arguments  { public abstract get; }
}

public interface Microsoft.OData.Edm.Expressions.IEdmBinaryConstantExpression : IEdmElement, IEdmExpression, IEdmBinaryValue, IEdmPrimitiveValue, IEdmValue {
}

public interface Microsoft.OData.Edm.Expressions.IEdmBooleanConstantExpression : IEdmElement, IEdmExpression, IEdmBooleanValue, IEdmPrimitiveValue, IEdmValue {
}

public interface Microsoft.OData.Edm.Expressions.IEdmCastExpression : IEdmElement, IEdmExpression {
	Microsoft.OData.Edm.Expressions.IEdmExpression Operand  { public abstract get; }
	Microsoft.OData.Edm.IEdmTypeReference Type  { public abstract get; }
}

public interface Microsoft.OData.Edm.Expressions.IEdmCollectionExpression : IEdmElement, IEdmExpression {
	Microsoft.OData.Edm.IEdmTypeReference DeclaredType  { public abstract get; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Expressions.IEdmExpression]] Elements  { public abstract get; }
}

public interface Microsoft.OData.Edm.Expressions.IEdmDateConstantExpression : IEdmElement, IEdmExpression, IEdmDateValue, IEdmPrimitiveValue, IEdmValue {
}

public interface Microsoft.OData.Edm.Expressions.IEdmDateTimeOffsetConstantExpression : IEdmElement, IEdmExpression, IEdmDateTimeOffsetValue, IEdmPrimitiveValue, IEdmValue {
}

public interface Microsoft.OData.Edm.Expressions.IEdmDecimalConstantExpression : IEdmElement, IEdmExpression, IEdmDecimalValue, IEdmPrimitiveValue, IEdmValue {
}

public interface Microsoft.OData.Edm.Expressions.IEdmDurationConstantExpression : IEdmElement, IEdmExpression, IEdmDurationValue, IEdmPrimitiveValue, IEdmValue {
}

public interface Microsoft.OData.Edm.Expressions.IEdmEntitySetReferenceExpression : IEdmElement, IEdmExpression {
	Microsoft.OData.Edm.IEdmEntitySet ReferencedEntitySet  { public abstract get; }
}

public interface Microsoft.OData.Edm.Expressions.IEdmEnumMemberExpression : IEdmElement, IEdmExpression {
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmEnumMember]] EnumMembers  { public abstract get; }
}

public interface Microsoft.OData.Edm.Expressions.IEdmEnumMemberReferenceExpression : IEdmElement, IEdmExpression {
	Microsoft.OData.Edm.IEdmEnumMember ReferencedEnumMember  { public abstract get; }
}

public interface Microsoft.OData.Edm.Expressions.IEdmExpression : IEdmElement {
	Microsoft.OData.Edm.Expressions.EdmExpressionKind ExpressionKind  { public abstract get; }
}

public interface Microsoft.OData.Edm.Expressions.IEdmFloatingConstantExpression : IEdmElement, IEdmExpression, IEdmFloatingValue, IEdmPrimitiveValue, IEdmValue {
}

public interface Microsoft.OData.Edm.Expressions.IEdmGuidConstantExpression : IEdmElement, IEdmExpression, IEdmGuidValue, IEdmPrimitiveValue, IEdmValue {
}

public interface Microsoft.OData.Edm.Expressions.IEdmIfExpression : IEdmElement, IEdmExpression {
	Microsoft.OData.Edm.Expressions.IEdmExpression FalseExpression  { public abstract get; }
	Microsoft.OData.Edm.Expressions.IEdmExpression TestExpression  { public abstract get; }
	Microsoft.OData.Edm.Expressions.IEdmExpression TrueExpression  { public abstract get; }
}

public interface Microsoft.OData.Edm.Expressions.IEdmIntegerConstantExpression : IEdmElement, IEdmExpression, IEdmIntegerValue, IEdmPrimitiveValue, IEdmValue {
}

public interface Microsoft.OData.Edm.Expressions.IEdmIsTypeExpression : IEdmElement, IEdmExpression {
	Microsoft.OData.Edm.Expressions.IEdmExpression Operand  { public abstract get; }
	Microsoft.OData.Edm.IEdmTypeReference Type  { public abstract get; }
}

public interface Microsoft.OData.Edm.Expressions.IEdmLabeledExpression : IEdmElement, IEdmNamedElement, IEdmExpression {
	Microsoft.OData.Edm.Expressions.IEdmExpression Expression  { public abstract get; }
}

public interface Microsoft.OData.Edm.Expressions.IEdmLabeledExpressionReferenceExpression : IEdmElement, IEdmExpression {
	Microsoft.OData.Edm.Expressions.IEdmLabeledExpression ReferencedLabeledExpression  { public abstract get; }
}

public interface Microsoft.OData.Edm.Expressions.IEdmNullExpression : IEdmElement, IEdmExpression, IEdmNullValue, IEdmValue {
}

public interface Microsoft.OData.Edm.Expressions.IEdmOperationReferenceExpression : IEdmElement, IEdmExpression {
	Microsoft.OData.Edm.IEdmOperation ReferencedOperation  { public abstract get; }
}

public interface Microsoft.OData.Edm.Expressions.IEdmParameterReferenceExpression : IEdmElement, IEdmExpression {
	Microsoft.OData.Edm.IEdmOperationParameter ReferencedParameter  { public abstract get; }
}

public interface Microsoft.OData.Edm.Expressions.IEdmPathExpression : IEdmElement, IEdmExpression {
	System.Collections.Generic.IEnumerable`1[[System.String]] Path  { public abstract get; }
}

public interface Microsoft.OData.Edm.Expressions.IEdmPropertyConstructor : IEdmElement {
	string Name  { public abstract get; }
	Microsoft.OData.Edm.Expressions.IEdmExpression Value  { public abstract get; }
}

public interface Microsoft.OData.Edm.Expressions.IEdmPropertyReferenceExpression : IEdmElement, IEdmExpression {
	Microsoft.OData.Edm.Expressions.IEdmExpression Base  { public abstract get; }
	Microsoft.OData.Edm.IEdmProperty ReferencedProperty  { public abstract get; }
}

public interface Microsoft.OData.Edm.Expressions.IEdmRecordExpression : IEdmElement, IEdmExpression {
	Microsoft.OData.Edm.IEdmStructuredTypeReference DeclaredType  { public abstract get; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Expressions.IEdmPropertyConstructor]] Properties  { public abstract get; }
}

public interface Microsoft.OData.Edm.Expressions.IEdmStringConstantExpression : IEdmElement, IEdmExpression, IEdmPrimitiveValue, IEdmStringValue, IEdmValue {
}

public interface Microsoft.OData.Edm.Expressions.IEdmTimeOfDayConstantExpression : IEdmElement, IEdmExpression, IEdmPrimitiveValue, IEdmTimeOfDayValue, IEdmValue {
}

public interface Microsoft.OData.Edm.Expressions.IEdmValueTermReferenceExpression : IEdmElement, IEdmExpression {
	Microsoft.OData.Edm.Expressions.IEdmExpression Base  { public abstract get; }
	string Qualifier  { public abstract get; }
	Microsoft.OData.Edm.IEdmValueTerm Term  { public abstract get; }
}

public struct Microsoft.OData.Edm.Library.Date : IComparable, IComparable`1, IEquatable`1 {
	public static readonly Microsoft.OData.Edm.Library.Date MaxValue = 9999-12-31
	public static readonly Microsoft.OData.Edm.Library.Date MinValue = 0001-01-01

	public Date (int year, int month, int day)

	int Day  { public get; }
	int Month  { public get; }
	Microsoft.OData.Edm.Library.Date Now  { public static get; }
	int Year  { public get; }

	public Microsoft.OData.Edm.Library.Date AddDays (int value)
	public Microsoft.OData.Edm.Library.Date AddMonths (int value)
	public Microsoft.OData.Edm.Library.Date AddYears (int value)
	public virtual int CompareTo (Microsoft.OData.Edm.Library.Date other)
	public virtual int CompareTo (object obj)
	public virtual bool Equals (Microsoft.OData.Edm.Library.Date other)
	public virtual bool Equals (object obj)
	public virtual int GetHashCode ()
	public static Microsoft.OData.Edm.Library.Date Parse (string text)
	public static Microsoft.OData.Edm.Library.Date Parse (string text, System.IFormatProvider provider)
	public virtual string ToString ()
	public static bool TryParse (string text, out Microsoft.OData.Edm.Library.Date& result)
	public static bool TryParse (string text, System.IFormatProvider provider, out Microsoft.OData.Edm.Library.Date& result)
}

public struct Microsoft.OData.Edm.Library.TimeOfDay : IComparable, IComparable`1, IEquatable`1 {
	public static long MaxTickValue = 863999999999
	public static readonly Microsoft.OData.Edm.Library.TimeOfDay MaxValue = 23:59:59.9999999
	public static long MinTickValue = 0
	public static readonly Microsoft.OData.Edm.Library.TimeOfDay MinValue = 00:00:00.0000000
	public static long TicksPerHour = 36000000000
	public static long TicksPerMinute = 600000000
	public static long TicksPerSecond = 10000000

	public TimeOfDay (long ticks)
	public TimeOfDay (int hour, int minute, int second, int millisecond)

	int Hours  { public get; }
	long Milliseconds  { public get; }
	int Minutes  { public get; }
	Microsoft.OData.Edm.Library.TimeOfDay Now  { public static get; }
	int Seconds  { public get; }
	long Ticks  { public get; }

	public virtual int CompareTo (Microsoft.OData.Edm.Library.TimeOfDay other)
	public virtual int CompareTo (object obj)
	public virtual bool Equals (Microsoft.OData.Edm.Library.TimeOfDay other)
	public virtual bool Equals (object obj)
	public virtual int GetHashCode ()
	public static Microsoft.OData.Edm.Library.TimeOfDay Parse (string text)
	public static Microsoft.OData.Edm.Library.TimeOfDay Parse (string text, System.IFormatProvider provider)
	public virtual string ToString ()
	public static bool TryParse (string text, out Microsoft.OData.Edm.Library.TimeOfDay& result)
	public static bool TryParse (string text, System.IFormatProvider provider, out Microsoft.OData.Edm.Library.TimeOfDay& result)
}

public abstract class Microsoft.OData.Edm.Library.EdmElement : IEdmElement {
	protected EdmElement ()
}

public abstract class Microsoft.OData.Edm.Library.EdmEntitySetBase : Microsoft.OData.Edm.Library.EdmNavigationSource, IEdmElement, IEdmEntitySetBase, IEdmNamedElement, IEdmNavigationSource {
	protected EdmEntitySetBase (string name, Microsoft.OData.Edm.IEdmEntityType elementType)

	Microsoft.OData.Edm.IEdmType Type  { public virtual get; }
}

public abstract class Microsoft.OData.Edm.Library.EdmModelBase : Microsoft.OData.Edm.Library.EdmElement, IEdmElement, IEdmModel {
	protected EdmModelBase (System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmModel]] referencedModels, Microsoft.OData.Edm.Annotations.IEdmDirectValueAnnotationsManager annotationsManager)

	System.Collections.Generic.IEnumerable`1[[System.String]] DeclaredNamespaces  { public abstract get; }
	Microsoft.OData.Edm.Annotations.IEdmDirectValueAnnotationsManager DirectValueAnnotationsManager  { public virtual get; }
	Microsoft.OData.Edm.IEdmEntityContainer EntityContainer  { public virtual get; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmModel]] ReferencedModels  { public virtual get; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmSchemaElement]] SchemaElements  { public abstract get; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Annotations.IEdmVocabularyAnnotation]] VocabularyAnnotations  { public virtual get; }

	protected void AddReferencedModel (Microsoft.OData.Edm.IEdmModel model)
	public virtual System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmOperation]] FindDeclaredBoundOperations (Microsoft.OData.Edm.IEdmType bindingType)
	public virtual System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmOperation]] FindDeclaredBoundOperations (string qualifiedName, Microsoft.OData.Edm.IEdmType bindingType)
	public virtual System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmOperation]] FindDeclaredOperations (string qualifiedName)
	public virtual Microsoft.OData.Edm.IEdmSchemaType FindDeclaredType (string qualifiedName)
	public virtual Microsoft.OData.Edm.IEdmValueTerm FindDeclaredValueTerm (string qualifiedName)
	public virtual System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Annotations.IEdmVocabularyAnnotation]] FindDeclaredVocabularyAnnotations (Microsoft.OData.Edm.IEdmVocabularyAnnotatable element)
	public abstract System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmStructuredType]] FindDirectlyDerivedTypes (Microsoft.OData.Edm.IEdmStructuredType baseType)
	protected void RegisterElement (Microsoft.OData.Edm.IEdmSchemaElement element)
}

[
DebuggerDisplayAttribute(),
]
public abstract class Microsoft.OData.Edm.Library.EdmNamedElement : Microsoft.OData.Edm.Library.EdmElement, IEdmElement, IEdmNamedElement {
	protected EdmNamedElement (string name)

	string Name  { public virtual get; }
}

public abstract class Microsoft.OData.Edm.Library.EdmNavigationSource : Microsoft.OData.Edm.Library.EdmNamedElement, IEdmElement, IEdmNamedElement, IEdmNavigationSource {
	protected EdmNavigationSource (string name)

	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmNavigationPropertyBinding]] NavigationPropertyBindings  { public virtual get; }
	Microsoft.OData.Edm.Expressions.IEdmPathExpression Path  { public abstract get; }
	Microsoft.OData.Edm.IEdmType Type  { public abstract get; }

	public void AddNavigationTarget (Microsoft.OData.Edm.IEdmNavigationProperty property, Microsoft.OData.Edm.IEdmNavigationSource target)
	public virtual Microsoft.OData.Edm.IEdmNavigationSource FindNavigationTarget (Microsoft.OData.Edm.IEdmNavigationProperty property)
}

public abstract class Microsoft.OData.Edm.Library.EdmOperation : Microsoft.OData.Edm.Library.EdmNamedElement, IEdmElement, IEdmNamedElement, IEdmOperation, IEdmSchemaElement, IEdmVocabularyAnnotatable {
	protected EdmOperation (string namespaceName, string name, Microsoft.OData.Edm.IEdmTypeReference returnType)
	protected EdmOperation (string namespaceName, string name, Microsoft.OData.Edm.IEdmTypeReference returnType, bool isBound, Microsoft.OData.Edm.Expressions.IEdmPathExpression entitySetPathExpression)

	Microsoft.OData.Edm.Expressions.IEdmPathExpression EntitySetPath  { public virtual get; }
	bool IsBound  { public virtual get; }
	string Namespace  { public virtual get; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmOperationParameter]] Parameters  { public virtual get; }
	Microsoft.OData.Edm.IEdmTypeReference ReturnType  { public virtual get; }
	Microsoft.OData.Edm.EdmSchemaElementKind SchemaElementKind  { public abstract get; }

	public void AddParameter (Microsoft.OData.Edm.IEdmOperationParameter parameter)
	public Microsoft.OData.Edm.Library.EdmOperationParameter AddParameter (string name, Microsoft.OData.Edm.IEdmTypeReference type)
	public virtual Microsoft.OData.Edm.IEdmOperationParameter FindParameter (string name)
}

public abstract class Microsoft.OData.Edm.Library.EdmOperationImport : Microsoft.OData.Edm.Library.EdmNamedElement, IEdmElement, IEdmEntityContainerElement, IEdmNamedElement, IEdmOperationImport, IEdmVocabularyAnnotatable {
	protected EdmOperationImport (Microsoft.OData.Edm.IEdmEntityContainer container, Microsoft.OData.Edm.IEdmOperation operation, string name, Microsoft.OData.Edm.Expressions.IEdmExpression entitySet)

	Microsoft.OData.Edm.IEdmEntityContainer Container  { public virtual get; }
	Microsoft.OData.Edm.EdmContainerElementKind ContainerElementKind  { public abstract get; }
	Microsoft.OData.Edm.Expressions.IEdmExpression EntitySet  { public virtual get; }
	Microsoft.OData.Edm.IEdmOperation Operation  { public virtual get; }

	protected abstract string OperationArgumentNullParameterName ()
}

public abstract class Microsoft.OData.Edm.Library.EdmProperty : Microsoft.OData.Edm.Library.EdmNamedElement, IEdmElement, IEdmNamedElement, IEdmProperty, IEdmVocabularyAnnotatable {
	protected EdmProperty (Microsoft.OData.Edm.IEdmStructuredType declaringType, string name, Microsoft.OData.Edm.IEdmTypeReference type)

	Microsoft.OData.Edm.IEdmStructuredType DeclaringType  { public virtual get; }
	Microsoft.OData.Edm.EdmPropertyKind PropertyKind  { public abstract get; }
	Microsoft.OData.Edm.IEdmTypeReference Type  { public virtual get; }
}

public abstract class Microsoft.OData.Edm.Library.EdmStructuredType : Microsoft.OData.Edm.Library.EdmType, IEdmElement, IEdmStructuredType, IEdmType {
	protected EdmStructuredType (bool isAbstract, bool isOpen, Microsoft.OData.Edm.IEdmStructuredType baseStructuredType)

	Microsoft.OData.Edm.IEdmStructuredType BaseType  { public virtual get; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmProperty]] DeclaredProperties  { public virtual get; }
	bool IsAbstract  { public virtual get; }
	bool IsOpen  { public virtual get; }
	System.Collections.Generic.IDictionary`2[[System.String],[Microsoft.OData.Edm.IEdmProperty]] PropertiesDictionary  { protected get; }

	public void AddProperty (Microsoft.OData.Edm.IEdmProperty property)
	public Microsoft.OData.Edm.Library.EdmStructuralProperty AddStructuralProperty (string name, Microsoft.OData.Edm.EdmPrimitiveTypeKind type)
	public Microsoft.OData.Edm.Library.EdmStructuralProperty AddStructuralProperty (string name, Microsoft.OData.Edm.IEdmTypeReference type)
	public Microsoft.OData.Edm.Library.EdmStructuralProperty AddStructuralProperty (string name, Microsoft.OData.Edm.EdmPrimitiveTypeKind type, bool isNullable)
	public Microsoft.OData.Edm.Library.EdmStructuralProperty AddStructuralProperty (string name, Microsoft.OData.Edm.IEdmTypeReference type, string defaultValue, Microsoft.OData.Edm.EdmConcurrencyMode concurrencyMode)
	public virtual Microsoft.OData.Edm.IEdmProperty FindProperty (string name)
}

public abstract class Microsoft.OData.Edm.Library.EdmType : Microsoft.OData.Edm.Library.EdmElement, IEdmElement, IEdmType {
	protected EdmType ()

	Microsoft.OData.Edm.EdmTypeKind TypeKind  { public abstract get; }

	public virtual string ToString ()
}

public abstract class Microsoft.OData.Edm.Library.EdmTypeReference : Microsoft.OData.Edm.Library.EdmElement, IEdmElement, IEdmTypeReference {
	protected EdmTypeReference (Microsoft.OData.Edm.IEdmType definition, bool isNullable)

	Microsoft.OData.Edm.IEdmType Definition  { public virtual get; }
	bool IsNullable  { public virtual get; }

	public virtual string ToString ()
}

public sealed class Microsoft.OData.Edm.Library.EdmConstants {
	public static readonly System.Version EdmVersion4 = 4.0
	public static readonly System.Version EdmVersionLatest = 4.0
}

public class Microsoft.OData.Edm.Library.EdmAction : Microsoft.OData.Edm.Library.EdmOperation, IEdmAction, IEdmElement, IEdmNamedElement, IEdmOperation, IEdmSchemaElement, IEdmVocabularyAnnotatable {
	public EdmAction (string namespaceName, string name, Microsoft.OData.Edm.IEdmTypeReference returnType)
	public EdmAction (string namespaceName, string name, Microsoft.OData.Edm.IEdmTypeReference returnType, bool isBound, Microsoft.OData.Edm.Expressions.IEdmPathExpression entitySetPathExpression)

	Microsoft.OData.Edm.EdmSchemaElementKind SchemaElementKind  { public virtual get; }
}

public class Microsoft.OData.Edm.Library.EdmActionImport : Microsoft.OData.Edm.Library.EdmOperationImport, IEdmActionImport, IEdmElement, IEdmEntityContainerElement, IEdmNamedElement, IEdmOperationImport, IEdmVocabularyAnnotatable {
	public EdmActionImport (Microsoft.OData.Edm.IEdmEntityContainer container, string name, Microsoft.OData.Edm.IEdmAction action)
	public EdmActionImport (Microsoft.OData.Edm.IEdmEntityContainer container, string name, Microsoft.OData.Edm.IEdmAction action, Microsoft.OData.Edm.Expressions.IEdmExpression entitySetExpression)

	Microsoft.OData.Edm.IEdmAction Action  { public virtual get; }
	Microsoft.OData.Edm.EdmContainerElementKind ContainerElementKind  { public virtual get; }

	protected virtual string OperationArgumentNullParameterName ()
}

public class Microsoft.OData.Edm.Library.EdmBinaryTypeReference : Microsoft.OData.Edm.Library.EdmPrimitiveTypeReference, IEdmBinaryTypeReference, IEdmElement, IEdmPrimitiveTypeReference, IEdmTypeReference {
	public EdmBinaryTypeReference (Microsoft.OData.Edm.IEdmPrimitiveType definition, bool isNullable)
	public EdmBinaryTypeReference (Microsoft.OData.Edm.IEdmPrimitiveType definition, bool isNullable, bool isUnbounded, System.Nullable`1[[System.Int32]] maxLength)

	bool IsUnbounded  { public virtual get; }
	System.Nullable`1[[System.Int32]] MaxLength  { public virtual get; }
}

public class Microsoft.OData.Edm.Library.EdmCollectionType : Microsoft.OData.Edm.Library.EdmType, IEdmCollectionType, IEdmElement, IEdmType {
	public EdmCollectionType (Microsoft.OData.Edm.IEdmTypeReference elementType)

	Microsoft.OData.Edm.IEdmTypeReference ElementType  { public virtual get; }
	Microsoft.OData.Edm.EdmTypeKind TypeKind  { public virtual get; }
}

public class Microsoft.OData.Edm.Library.EdmCollectionTypeReference : Microsoft.OData.Edm.Library.EdmTypeReference, IEdmCollectionTypeReference, IEdmElement, IEdmTypeReference {
	public EdmCollectionTypeReference (Microsoft.OData.Edm.IEdmCollectionType collectionType)
}

public class Microsoft.OData.Edm.Library.EdmComplexType : Microsoft.OData.Edm.Library.EdmStructuredType, IEdmComplexType, IEdmElement, IEdmNamedElement, IEdmSchemaElement, IEdmSchemaType, IEdmStructuredType, IEdmTerm, IEdmType, IEdmVocabularyAnnotatable {
	public EdmComplexType (string namespaceName, string name)
	public EdmComplexType (string namespaceName, string name, Microsoft.OData.Edm.IEdmComplexType baseType)
	public EdmComplexType (string namespaceName, string name, Microsoft.OData.Edm.IEdmComplexType baseType, bool isAbstract)
	public EdmComplexType (string namespaceName, string name, Microsoft.OData.Edm.IEdmComplexType baseType, bool isAbstract, bool isOpen)

	string Name  { public virtual get; }
	string Namespace  { public virtual get; }
	Microsoft.OData.Edm.EdmSchemaElementKind SchemaElementKind  { public virtual get; }
	Microsoft.OData.Edm.EdmTermKind TermKind  { public virtual get; }
	Microsoft.OData.Edm.EdmTypeKind TypeKind  { public virtual get; }
}

public class Microsoft.OData.Edm.Library.EdmComplexTypeReference : Microsoft.OData.Edm.Library.EdmTypeReference, IEdmComplexTypeReference, IEdmElement, IEdmStructuredTypeReference, IEdmTypeReference {
	public EdmComplexTypeReference (Microsoft.OData.Edm.IEdmComplexType complexType, bool isNullable)
}

public class Microsoft.OData.Edm.Library.EdmCoreModel : Microsoft.OData.Edm.Library.EdmElement, IEdmElement, IEdmModel, IEdmValidCoreModelElement {
	public static readonly Microsoft.OData.Edm.Library.EdmCoreModel Instance = Microsoft.OData.Edm.Library.EdmCoreModel

	System.Collections.Generic.IEnumerable`1[[System.String]] DeclaredNamespaces  { public virtual get; }
	Microsoft.OData.Edm.Annotations.IEdmDirectValueAnnotationsManager DirectValueAnnotationsManager  { public virtual get; }
	Microsoft.OData.Edm.IEdmEntityContainer EntityContainer  { public virtual get; }
	string Namespace  { public static get; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmModel]] ReferencedModels  { public virtual get; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmSchemaElement]] SchemaElements  { public virtual get; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Annotations.IEdmVocabularyAnnotation]] VocabularyAnnotations  { public virtual get; }

	public virtual System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmOperation]] FindDeclaredBoundOperations (Microsoft.OData.Edm.IEdmType bindingType)
	public virtual System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmOperation]] FindDeclaredBoundOperations (string qualifiedName, Microsoft.OData.Edm.IEdmType bindingType)
	public virtual System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmOperation]] FindDeclaredOperations (string qualifiedName)
	public virtual Microsoft.OData.Edm.IEdmSchemaType FindDeclaredType (string qualifiedName)
	public virtual Microsoft.OData.Edm.IEdmValueTerm FindDeclaredValueTerm (string qualifiedName)
	public virtual System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Annotations.IEdmVocabularyAnnotation]] FindDeclaredVocabularyAnnotations (Microsoft.OData.Edm.IEdmVocabularyAnnotatable element)
	public virtual System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmStructuredType]] FindDirectlyDerivedTypes (Microsoft.OData.Edm.IEdmStructuredType baseType)
	public System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmOperationImport]] FindOperationImportsByNameNonBindingParameterType (string operationImportName, System.Collections.Generic.IEnumerable`1[[System.String]] parameterNames)
	public Microsoft.OData.Edm.IEdmBinaryTypeReference GetBinary (bool isNullable)
	public Microsoft.OData.Edm.IEdmBinaryTypeReference GetBinary (bool isUnbounded, System.Nullable`1[[System.Int32]] maxLength, bool isNullable)
	public Microsoft.OData.Edm.IEdmPrimitiveTypeReference GetBoolean (bool isNullable)
	public Microsoft.OData.Edm.IEdmPrimitiveTypeReference GetByte (bool isNullable)
	public static Microsoft.OData.Edm.IEdmCollectionTypeReference GetCollection (Microsoft.OData.Edm.IEdmTypeReference elementType)
	public Microsoft.OData.Edm.IEdmPrimitiveTypeReference GetDate (bool isNullable)
	public Microsoft.OData.Edm.IEdmTemporalTypeReference GetDateTimeOffset (bool isNullable)
	public Microsoft.OData.Edm.IEdmDecimalTypeReference GetDecimal (bool isNullable)
	public Microsoft.OData.Edm.IEdmDecimalTypeReference GetDecimal (System.Nullable`1[[System.Int32]] precision, System.Nullable`1[[System.Int32]] scale, bool isNullable)
	public Microsoft.OData.Edm.IEdmPrimitiveTypeReference GetDouble (bool isNullable)
	public Microsoft.OData.Edm.IEdmTemporalTypeReference GetDuration (bool isNullable)
	public Microsoft.OData.Edm.IEdmPrimitiveTypeReference GetGuid (bool isNullable)
	public Microsoft.OData.Edm.IEdmPrimitiveTypeReference GetInt16 (bool isNullable)
	public Microsoft.OData.Edm.IEdmPrimitiveTypeReference GetInt32 (bool isNullable)
	public Microsoft.OData.Edm.IEdmPrimitiveTypeReference GetInt64 (bool isNullable)
	public Microsoft.OData.Edm.IEdmPrimitiveTypeReference GetPrimitive (Microsoft.OData.Edm.EdmPrimitiveTypeKind kind, bool isNullable)
	public Microsoft.OData.Edm.IEdmPrimitiveType GetPrimitiveType (Microsoft.OData.Edm.EdmPrimitiveTypeKind kind)
	public Microsoft.OData.Edm.EdmPrimitiveTypeKind GetPrimitiveTypeKind (string typeName)
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
}

public class Microsoft.OData.Edm.Library.EdmDecimalTypeReference : Microsoft.OData.Edm.Library.EdmPrimitiveTypeReference, IEdmDecimalTypeReference, IEdmElement, IEdmPrimitiveTypeReference, IEdmTypeReference {
	public EdmDecimalTypeReference (Microsoft.OData.Edm.IEdmPrimitiveType definition, bool isNullable)
	public EdmDecimalTypeReference (Microsoft.OData.Edm.IEdmPrimitiveType definition, bool isNullable, System.Nullable`1[[System.Int32]] precision, System.Nullable`1[[System.Int32]] scale)

	System.Nullable`1[[System.Int32]] Precision  { public virtual get; }
	System.Nullable`1[[System.Int32]] Scale  { public virtual get; }
}

public class Microsoft.OData.Edm.Library.EdmEntityContainer : Microsoft.OData.Edm.Library.EdmElement, IEdmElement, IEdmEntityContainer, IEdmNamedElement, IEdmSchemaElement, IEdmVocabularyAnnotatable {
	public EdmEntityContainer (string namespaceName, string name)

	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmEntityContainerElement]] Elements  { public virtual get; }
	string Name  { public virtual get; }
	string Namespace  { public virtual get; }
	Microsoft.OData.Edm.EdmSchemaElementKind SchemaElementKind  { public virtual get; }

	public virtual Microsoft.OData.Edm.Library.EdmActionImport AddActionImport (Microsoft.OData.Edm.IEdmAction action)
	public virtual Microsoft.OData.Edm.Library.EdmActionImport AddActionImport (string name, Microsoft.OData.Edm.IEdmAction action)
	public virtual Microsoft.OData.Edm.Library.EdmActionImport AddActionImport (string name, Microsoft.OData.Edm.IEdmAction action, Microsoft.OData.Edm.Expressions.IEdmExpression entitySet)
	public void AddElement (Microsoft.OData.Edm.IEdmEntityContainerElement element)
	public virtual Microsoft.OData.Edm.Library.EdmEntitySet AddEntitySet (string name, Microsoft.OData.Edm.IEdmEntityType elementType)
	public virtual Microsoft.OData.Edm.Library.EdmFunctionImport AddFunctionImport (Microsoft.OData.Edm.IEdmFunction function)
	public virtual Microsoft.OData.Edm.Library.EdmFunctionImport AddFunctionImport (string name, Microsoft.OData.Edm.IEdmFunction function)
	public virtual Microsoft.OData.Edm.Library.EdmFunctionImport AddFunctionImport (string name, Microsoft.OData.Edm.IEdmFunction function, Microsoft.OData.Edm.Expressions.IEdmExpression entitySet)
	public virtual Microsoft.OData.Edm.Library.EdmOperationImport AddFunctionImport (string name, Microsoft.OData.Edm.IEdmFunction function, Microsoft.OData.Edm.Expressions.IEdmExpression entitySet, bool includeInServiceDocument)
	public virtual Microsoft.OData.Edm.Library.EdmSingleton AddSingleton (string name, Microsoft.OData.Edm.IEdmEntityType entityType)
	public virtual Microsoft.OData.Edm.IEdmEntitySet FindEntitySet (string setName)
	public virtual System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmOperationImport]] FindOperationImports (string operationName)
	public virtual Microsoft.OData.Edm.IEdmSingleton FindSingleton (string singletonName)
}

public class Microsoft.OData.Edm.Library.EdmEntityReferenceType : Microsoft.OData.Edm.Library.EdmType, IEdmElement, IEdmEntityReferenceType, IEdmType {
	public EdmEntityReferenceType (Microsoft.OData.Edm.IEdmEntityType entityType)

	Microsoft.OData.Edm.IEdmEntityType EntityType  { public virtual get; }
	Microsoft.OData.Edm.EdmTypeKind TypeKind  { public virtual get; }
}

public class Microsoft.OData.Edm.Library.EdmEntityReferenceTypeReference : Microsoft.OData.Edm.Library.EdmTypeReference, IEdmElement, IEdmEntityReferenceTypeReference, IEdmTypeReference {
	public EdmEntityReferenceTypeReference (Microsoft.OData.Edm.IEdmEntityReferenceType entityReferenceType, bool isNullable)

	Microsoft.OData.Edm.IEdmEntityReferenceType EntityReferenceDefinition  { public get; }
}

public class Microsoft.OData.Edm.Library.EdmEntitySet : Microsoft.OData.Edm.Library.EdmEntitySetBase, IEdmElement, IEdmEntityContainerElement, IEdmEntitySet, IEdmEntitySetBase, IEdmNamedElement, IEdmNavigationSource, IEdmVocabularyAnnotatable {
	public EdmEntitySet (Microsoft.OData.Edm.IEdmEntityContainer container, string name, Microsoft.OData.Edm.IEdmEntityType elementType)

	Microsoft.OData.Edm.IEdmEntityContainer Container  { public virtual get; }
	Microsoft.OData.Edm.EdmContainerElementKind ContainerElementKind  { public virtual get; }
	Microsoft.OData.Edm.Expressions.IEdmPathExpression Path  { public virtual get; }
	Microsoft.OData.Edm.IEdmType Type  { public virtual get; }
}

public class Microsoft.OData.Edm.Library.EdmEntityType : Microsoft.OData.Edm.Library.EdmStructuredType, IEdmElement, IEdmEntityType, IEdmNamedElement, IEdmSchemaElement, IEdmSchemaType, IEdmStructuredType, IEdmTerm, IEdmType, IEdmVocabularyAnnotatable {
	public EdmEntityType (string namespaceName, string name)
	public EdmEntityType (string namespaceName, string name, Microsoft.OData.Edm.IEdmEntityType baseType)
	public EdmEntityType (string namespaceName, string name, Microsoft.OData.Edm.IEdmEntityType baseType, bool isAbstract, bool isOpen)
	public EdmEntityType (string namespaceName, string name, Microsoft.OData.Edm.IEdmEntityType baseType, bool isAbstract, bool isOpen, bool hasStream)

	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmStructuralProperty]] DeclaredKey  { public virtual get; }
	bool HasStream  { public virtual get; }
	string Name  { public virtual get; }
	string Namespace  { public virtual get; }
	Microsoft.OData.Edm.EdmSchemaElementKind SchemaElementKind  { public virtual get; }
	Microsoft.OData.Edm.EdmTermKind TermKind  { public virtual get; }
	Microsoft.OData.Edm.EdmTypeKind TypeKind  { public virtual get; }

	public Microsoft.OData.Edm.Library.EdmNavigationProperty AddBidirectionalNavigation (Microsoft.OData.Edm.Library.EdmNavigationPropertyInfo propertyInfo, Microsoft.OData.Edm.Library.EdmNavigationPropertyInfo partnerInfo)
	public void AddKeys (Microsoft.OData.Edm.IEdmStructuralProperty[] keyProperties)
	public void AddKeys (System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmStructuralProperty]] keyProperties)
	public Microsoft.OData.Edm.Library.EdmNavigationProperty AddUnidirectionalNavigation (Microsoft.OData.Edm.Library.EdmNavigationPropertyInfo propertyInfo)
}

public class Microsoft.OData.Edm.Library.EdmEntityTypeReference : Microsoft.OData.Edm.Library.EdmTypeReference, IEdmElement, IEdmEntityTypeReference, IEdmStructuredTypeReference, IEdmTypeReference {
	public EdmEntityTypeReference (Microsoft.OData.Edm.IEdmEntityType entityType, bool isNullable)
}

public class Microsoft.OData.Edm.Library.EdmEnumMember : Microsoft.OData.Edm.Library.EdmNamedElement, IEdmElement, IEdmEnumMember, IEdmNamedElement, IEdmVocabularyAnnotatable {
	public EdmEnumMember (Microsoft.OData.Edm.IEdmEnumType declaringType, string name, Microsoft.OData.Edm.Values.IEdmPrimitiveValue value)

	Microsoft.OData.Edm.IEdmEnumType DeclaringType  { public virtual get; }
	Microsoft.OData.Edm.Values.IEdmPrimitiveValue Value  { public virtual get; }
}

public class Microsoft.OData.Edm.Library.EdmEnumType : Microsoft.OData.Edm.Library.EdmType, IEdmElement, IEdmEnumType, IEdmNamedElement, IEdmSchemaElement, IEdmSchemaType, IEdmType, IEdmVocabularyAnnotatable {
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
	public Microsoft.OData.Edm.Library.EdmEnumMember AddMember (string name, Microsoft.OData.Edm.Values.IEdmPrimitiveValue value)
}

public class Microsoft.OData.Edm.Library.EdmEnumTypeReference : Microsoft.OData.Edm.Library.EdmTypeReference, IEdmElement, IEdmEnumTypeReference, IEdmTypeReference {
	public EdmEnumTypeReference (Microsoft.OData.Edm.IEdmEnumType enumType, bool isNullable)
}

public class Microsoft.OData.Edm.Library.EdmFunction : Microsoft.OData.Edm.Library.EdmOperation, IEdmElement, IEdmFunction, IEdmNamedElement, IEdmOperation, IEdmSchemaElement, IEdmVocabularyAnnotatable {
	public EdmFunction (string namespaceName, string name, Microsoft.OData.Edm.IEdmTypeReference returnType)
	public EdmFunction (string namespaceName, string name, Microsoft.OData.Edm.IEdmTypeReference returnType, bool isBound, Microsoft.OData.Edm.Expressions.IEdmPathExpression entitySetPathExpression, bool isComposable)

	bool IsComposable  { public virtual get; }
	Microsoft.OData.Edm.EdmSchemaElementKind SchemaElementKind  { public virtual get; }
}

public class Microsoft.OData.Edm.Library.EdmFunctionImport : Microsoft.OData.Edm.Library.EdmOperationImport, IEdmElement, IEdmEntityContainerElement, IEdmFunctionImport, IEdmNamedElement, IEdmOperationImport, IEdmVocabularyAnnotatable {
	public EdmFunctionImport (Microsoft.OData.Edm.IEdmEntityContainer container, string name, Microsoft.OData.Edm.IEdmFunction function)
	public EdmFunctionImport (Microsoft.OData.Edm.IEdmEntityContainer container, string name, Microsoft.OData.Edm.IEdmFunction function, Microsoft.OData.Edm.Expressions.IEdmExpression entitySetExpression, bool includeInServiceDocument)

	Microsoft.OData.Edm.EdmContainerElementKind ContainerElementKind  { public virtual get; }
	Microsoft.OData.Edm.IEdmFunction Function  { public virtual get; }
	bool IncludeInServiceDocument  { public virtual get; }

	protected virtual string OperationArgumentNullParameterName ()
}

public class Microsoft.OData.Edm.Library.EdmInclude : IEdmInclude {
	public EdmInclude (string alias, string namespaceIncluded)

	string Alias  { public virtual get; }
	string Namespace  { public virtual get; }
}

public class Microsoft.OData.Edm.Library.EdmIncludeAnnotations : IEdmIncludeAnnotations {
	public EdmIncludeAnnotations (string termNamespace, string qualifier, string targetNamespace)

	string Qualifier  { public virtual get; }
	string TargetNamespace  { public virtual get; }
	string TermNamespace  { public virtual get; }
}

public class Microsoft.OData.Edm.Library.EdmModel : Microsoft.OData.Edm.Library.EdmModelBase, IEdmElement, IEdmModel {
	public EdmModel ()

	System.Collections.Generic.IEnumerable`1[[System.String]] DeclaredNamespaces  { public virtual get; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmSchemaElement]] SchemaElements  { public virtual get; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Annotations.IEdmVocabularyAnnotation]] VocabularyAnnotations  { public virtual get; }

	public void AddElement (Microsoft.OData.Edm.IEdmSchemaElement element)
	public void AddElements (System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmSchemaElement]] newElements)
	public void AddReferencedModel (Microsoft.OData.Edm.IEdmModel model)
	public void AddVocabularyAnnotation (Microsoft.OData.Edm.Annotations.IEdmVocabularyAnnotation annotation)
	public virtual System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Annotations.IEdmVocabularyAnnotation]] FindDeclaredVocabularyAnnotations (Microsoft.OData.Edm.IEdmVocabularyAnnotatable element)
	public virtual System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmStructuredType]] FindDirectlyDerivedTypes (Microsoft.OData.Edm.IEdmStructuredType baseType)
	public void SetVocabularyAnnotation (Microsoft.OData.Edm.Annotations.IEdmVocabularyAnnotation annotation)
}

public class Microsoft.OData.Edm.Library.EdmNavigationPropertyBinding : IEdmNavigationPropertyBinding {
	public EdmNavigationPropertyBinding (Microsoft.OData.Edm.IEdmNavigationProperty navigationProperty, Microsoft.OData.Edm.IEdmNavigationSource target)

	Microsoft.OData.Edm.IEdmNavigationProperty NavigationProperty  { public virtual get; }
	Microsoft.OData.Edm.IEdmNavigationSource Target  { public virtual get; }
}

public class Microsoft.OData.Edm.Library.EdmOperationParameter : Microsoft.OData.Edm.Library.EdmNamedElement, IEdmElement, IEdmNamedElement, IEdmOperationParameter, IEdmVocabularyAnnotatable {
	public EdmOperationParameter (Microsoft.OData.Edm.IEdmOperation declaringOperation, string name, Microsoft.OData.Edm.IEdmTypeReference type)

	Microsoft.OData.Edm.IEdmOperation DeclaringOperation  { public virtual get; }
	Microsoft.OData.Edm.IEdmTypeReference Type  { public virtual get; }
}

public class Microsoft.OData.Edm.Library.EdmPrimitiveTypeReference : Microsoft.OData.Edm.Library.EdmTypeReference, IEdmElement, IEdmPrimitiveTypeReference, IEdmTypeReference {
	public EdmPrimitiveTypeReference (Microsoft.OData.Edm.IEdmPrimitiveType definition, bool isNullable)
}

public class Microsoft.OData.Edm.Library.EdmReference : IEdmElement, IEdmReference {
	public EdmReference (string uri)

	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmIncludeAnnotations]] IncludeAnnotations  { public virtual get; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmInclude]] Includes  { public virtual get; }
	string Uri  { public virtual get; }

	public void AddInclude (Microsoft.OData.Edm.IEdmInclude edmInclude)
	public void AddIncludeAnnotations (Microsoft.OData.Edm.IEdmIncludeAnnotations edmIncludeAnnotations)
}

public class Microsoft.OData.Edm.Library.EdmReferentialConstraint : IEdmReferentialConstraint {
	public EdmReferentialConstraint (System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.EdmReferentialConstraintPropertyPair]] propertyPairs)

	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.EdmReferentialConstraintPropertyPair]] PropertyPairs  { public virtual get; }

	public static Microsoft.OData.Edm.Library.EdmReferentialConstraint Create (System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmStructuralProperty]] dependentProperties, System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmStructuralProperty]] principalProperties)
}

public class Microsoft.OData.Edm.Library.EdmSingleton : Microsoft.OData.Edm.Library.EdmNavigationSource, IEdmElement, IEdmEntityContainerElement, IEdmNamedElement, IEdmNavigationSource, IEdmSingleton, IEdmVocabularyAnnotatable {
	public EdmSingleton (Microsoft.OData.Edm.IEdmEntityContainer container, string name, Microsoft.OData.Edm.IEdmEntityType entityType)

	Microsoft.OData.Edm.IEdmEntityContainer Container  { public virtual get; }
	Microsoft.OData.Edm.EdmContainerElementKind ContainerElementKind  { public virtual get; }
	Microsoft.OData.Edm.Expressions.IEdmPathExpression Path  { public virtual get; }
	Microsoft.OData.Edm.IEdmType Type  { public virtual get; }
}

public class Microsoft.OData.Edm.Library.EdmSpatialTypeReference : Microsoft.OData.Edm.Library.EdmPrimitiveTypeReference, IEdmElement, IEdmPrimitiveTypeReference, IEdmSpatialTypeReference, IEdmTypeReference {
	public EdmSpatialTypeReference (Microsoft.OData.Edm.IEdmPrimitiveType definition, bool isNullable)
	public EdmSpatialTypeReference (Microsoft.OData.Edm.IEdmPrimitiveType definition, bool isNullable, System.Nullable`1[[System.Int32]] spatialReferenceIdentifier)

	System.Nullable`1[[System.Int32]] SpatialReferenceIdentifier  { public virtual get; }
}

public class Microsoft.OData.Edm.Library.EdmStringTypeReference : Microsoft.OData.Edm.Library.EdmPrimitiveTypeReference, IEdmElement, IEdmPrimitiveTypeReference, IEdmStringTypeReference, IEdmTypeReference {
	public EdmStringTypeReference (Microsoft.OData.Edm.IEdmPrimitiveType definition, bool isNullable)
	public EdmStringTypeReference (Microsoft.OData.Edm.IEdmPrimitiveType definition, bool isNullable, bool isUnbounded, System.Nullable`1[[System.Int32]] maxLength, System.Nullable`1[[System.Boolean]] isUnicode)

	bool IsUnbounded  { public virtual get; }
	System.Nullable`1[[System.Boolean]] IsUnicode  { public virtual get; }
	System.Nullable`1[[System.Int32]] MaxLength  { public virtual get; }
}

public class Microsoft.OData.Edm.Library.EdmStructuralProperty : Microsoft.OData.Edm.Library.EdmProperty, IEdmElement, IEdmNamedElement, IEdmProperty, IEdmStructuralProperty, IEdmVocabularyAnnotatable {
	public EdmStructuralProperty (Microsoft.OData.Edm.IEdmStructuredType declaringType, string name, Microsoft.OData.Edm.IEdmTypeReference type)
	public EdmStructuralProperty (Microsoft.OData.Edm.IEdmStructuredType declaringType, string name, Microsoft.OData.Edm.IEdmTypeReference type, string defaultValueString, Microsoft.OData.Edm.EdmConcurrencyMode concurrencyMode)

	Microsoft.OData.Edm.EdmConcurrencyMode ConcurrencyMode  { public virtual get; }
	string DefaultValueString  { public virtual get; }
	Microsoft.OData.Edm.EdmPropertyKind PropertyKind  { public virtual get; }
}

public class Microsoft.OData.Edm.Library.EdmTemporalTypeReference : Microsoft.OData.Edm.Library.EdmPrimitiveTypeReference, IEdmElement, IEdmPrimitiveTypeReference, IEdmTemporalTypeReference, IEdmTypeReference {
	public EdmTemporalTypeReference (Microsoft.OData.Edm.IEdmPrimitiveType definition, bool isNullable)
	public EdmTemporalTypeReference (Microsoft.OData.Edm.IEdmPrimitiveType definition, bool isNullable, System.Nullable`1[[System.Int32]] precision)

	System.Nullable`1[[System.Int32]] Precision  { public virtual get; }
}

public class Microsoft.OData.Edm.Library.EdmTerm : Microsoft.OData.Edm.Library.EdmNamedElement, IEdmElement, IEdmNamedElement, IEdmSchemaElement, IEdmTerm, IEdmValueTerm, IEdmVocabularyAnnotatable {
	public EdmTerm (string namespaceName, string name, Microsoft.OData.Edm.EdmPrimitiveTypeKind type)
	public EdmTerm (string namespaceName, string name, Microsoft.OData.Edm.IEdmTypeReference type)
	public EdmTerm (string namespaceName, string name, Microsoft.OData.Edm.EdmPrimitiveTypeKind type, string appliesTo)
	public EdmTerm (string namespaceName, string name, Microsoft.OData.Edm.IEdmTypeReference type, string appliesTo)
	public EdmTerm (string namespaceName, string name, Microsoft.OData.Edm.IEdmTypeReference type, string appliesTo, string defaultValue)

	string AppliesTo  { public virtual get; }
	string DefaultValue  { public virtual get; }
	string Namespace  { public virtual get; }
	Microsoft.OData.Edm.EdmSchemaElementKind SchemaElementKind  { public virtual get; }
	Microsoft.OData.Edm.EdmTermKind TermKind  { public virtual get; }
	Microsoft.OData.Edm.IEdmTypeReference Type  { public virtual get; }
}

public class Microsoft.OData.Edm.Library.EdmTypeDefinition : Microsoft.OData.Edm.Library.EdmType, IEdmElement, IEdmNamedElement, IEdmSchemaElement, IEdmSchemaType, IEdmType, IEdmTypeDefinition, IEdmVocabularyAnnotatable {
	public EdmTypeDefinition (string namespaceName, string name, Microsoft.OData.Edm.EdmPrimitiveTypeKind underlyingType)
	public EdmTypeDefinition (string namespaceName, string name, Microsoft.OData.Edm.IEdmPrimitiveType underlyingType)

	string Name  { public virtual get; }
	string Namespace  { public virtual get; }
	Microsoft.OData.Edm.EdmSchemaElementKind SchemaElementKind  { public virtual get; }
	Microsoft.OData.Edm.EdmTypeKind TypeKind  { public virtual get; }
	Microsoft.OData.Edm.IEdmPrimitiveType UnderlyingType  { public virtual get; }
}

public class Microsoft.OData.Edm.Library.EdmTypeDefinitionReference : Microsoft.OData.Edm.Library.EdmTypeReference, IEdmElement, IEdmTypeDefinitionReference, IEdmTypeReference {
	public EdmTypeDefinitionReference (Microsoft.OData.Edm.IEdmTypeDefinition typeDefinition, bool isNullable)
}

public sealed class Microsoft.OData.Edm.Library.EdmNavigationProperty : Microsoft.OData.Edm.Library.EdmProperty, IEdmElement, IEdmNamedElement, IEdmNavigationProperty, IEdmProperty, IEdmVocabularyAnnotatable {
	bool ContainsTarget  { public virtual get; }
	Microsoft.OData.Edm.IEdmEntityType DeclaringEntityType  { public get; }
	Microsoft.OData.Edm.EdmOnDeleteAction OnDelete  { public virtual get; }
	Microsoft.OData.Edm.IEdmNavigationProperty Partner  { public virtual get; }
	Microsoft.OData.Edm.EdmPropertyKind PropertyKind  { public virtual get; }
	Microsoft.OData.Edm.IEdmReferentialConstraint ReferentialConstraint  { public virtual get; }

	public static Microsoft.OData.Edm.Library.EdmNavigationProperty CreateNavigationProperty (Microsoft.OData.Edm.IEdmEntityType declaringType, Microsoft.OData.Edm.Library.EdmNavigationPropertyInfo propertyInfo)
	public static Microsoft.OData.Edm.Library.EdmNavigationProperty CreateNavigationPropertyWithPartner (Microsoft.OData.Edm.Library.EdmNavigationPropertyInfo propertyInfo, Microsoft.OData.Edm.Library.EdmNavigationPropertyInfo partnerInfo)
	public static Microsoft.OData.Edm.Library.EdmNavigationProperty CreateNavigationPropertyWithPartner (string propertyName, Microsoft.OData.Edm.IEdmTypeReference propertyType, System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmStructuralProperty]] dependentProperties, System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmStructuralProperty]] principalProperties, bool containsTarget, Microsoft.OData.Edm.EdmOnDeleteAction onDelete, string partnerPropertyName, Microsoft.OData.Edm.IEdmTypeReference partnerPropertyType, System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmStructuralProperty]] partnerDependentProperties, System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmStructuralProperty]] partnerPrincipalProperties, bool partnerContainsTarget, Microsoft.OData.Edm.EdmOnDeleteAction partnerOnDelete)
}

public sealed class Microsoft.OData.Edm.Library.EdmNavigationPropertyInfo {
	public EdmNavigationPropertyInfo ()

	bool ContainsTarget  { public get; public set; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmStructuralProperty]] DependentProperties  { public get; public set; }
	string Name  { public get; public set; }
	Microsoft.OData.Edm.EdmOnDeleteAction OnDelete  { public get; public set; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmStructuralProperty]] PrincipalProperties  { public get; public set; }
	Microsoft.OData.Edm.IEdmEntityType Target  { public get; public set; }
	Microsoft.OData.Edm.EdmMultiplicity TargetMultiplicity  { public get; public set; }

	public Microsoft.OData.Edm.Library.EdmNavigationPropertyInfo Clone ()
}

public interface Microsoft.OData.Edm.PrimitiveValueConverters.IPrimitiveValueConverter {
	object ConvertFromUnderlyingType (object value)
	object ConvertToUnderlyingType (object value)
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
	ComplexTypeMustHaveComplexBaseType = 238
	ComplexTypeMustHaveProperties = 264
	ConcurrencyRedefinedOnSubtypeOfEntitySetType = 145
	ConstructibleEntitySetTypeInvalidFromEntityTypeRemoval = 231
	ContainerElementContainerNameIncorrect = 328
	DeclaringTypeMustBeCorrect = 245
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
	EnumMemberTypeMustMatchEnumUnderlyingType = 292
	EnumMemberValueOutOfRange = 206
	EnumMustHaveIntegerUnderlyingType = 351
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
	InvalidConcurrencyMode = 144
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
	QualifierMustBeSimpleName = 359
	RecordExpressionHasExtraProperties = 318
	RecordExpressionMissingRequiredProperty = 317
	RecordExpressionNotValidForNonStructuredType = 316
	ReferencedTypeMustHaveValidName = 322
	ReferenceElementMustContainAtLeastOneIncludeOrIncludeAnnotationsElement = 372
	ReferentialConstraintPrincipalEndMustBelongToAssociation = 243
	SameRoleReferredInReferentialConstraint = 119
	ScaleOutOfRange = 52
	SchemaElementMustNotHaveKindOfNone = 338
	SimilarRelationshipEnd = 153
	SingleFileExpected = 323
	SingletonTypeMustBeEntityType = 369
	StringConstantLengthOutOfRange = 331
	SystemNamespaceEncountered = 161
	TermMustNotHaveKindOfNone = 337
	TextNotAllowed = 11
	TypeAnnotationHasExtraProperties = 348
	TypeAnnotationMissingRequiredProperty = 347
	TypeMismatchRelationshipConstraint = 112
	TypeMustNotHaveKindOfNone = 334
	TypeSemanticsCouldNotConvertTypeReference = 230
	UnboundFunctionOverloadHasIncorrectReturnType = 219
	UnderlyingTypeIsBadBecauseEnumTypeIsBad = 261
	UnexpectedXmlAttribute = 9
	UnexpectedXmlElement = 10
	UnexpectedXmlNodeType = 8
	UnknownEdmVersion = 325
	UnknownEdmxVersion = 324
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
	public static bool TryCast (Microsoft.OData.Edm.Expressions.IEdmExpression expression, Microsoft.OData.Edm.IEdmTypeReference type, out System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Validation.EdmError]]& discoveredErrors)

	[
	ExtensionAttribute(),
	]
	public static bool TryCast (Microsoft.OData.Edm.Expressions.IEdmExpression expression, Microsoft.OData.Edm.IEdmTypeReference type, Microsoft.OData.Edm.IEdmType context, bool matchExactly, out System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Validation.EdmError]]& discoveredErrors)
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
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.Annotations.IEdmValueAnnotation]] AnnotationInaccessibleTerm = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.Annotations.IEdmValueAnnotation]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmBinaryTypeReference]] BinaryTypeReferenceBinaryMaxLengthNegative = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmBinaryTypeReference]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmBinaryTypeReference]] BinaryTypeReferenceBinaryUnboundedNotValidForMaxLength = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmBinaryTypeReference]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmOperation]] BoundOperationMustHaveParameters = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmOperation]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.Expressions.IEdmCollectionExpression]] CollectionExpressionAllElementsCorrectType = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.Expressions.IEdmCollectionExpression]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmComplexType]] ComplexTypeInvalidAbstractComplexType = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmComplexType]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmComplexType]] ComplexTypeInvalidPolymorphicComplexType = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmComplexType]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmComplexType]] ComplexTypeMustContainProperties = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmComplexType]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmDecimalTypeReference]] DecimalTypeReferencePrecisionOutOfRange = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmDecimalTypeReference]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmDecimalTypeReference]] DecimalTypeReferenceScaleOutOfRange = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmDecimalTypeReference]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.Annotations.IEdmDirectValueAnnotation]] DirectValueAnnotationHasXmlSerializableName = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.Annotations.IEdmDirectValueAnnotation]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmElement]] ElementDirectValueAnnotationFullNameMustBeUnique = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmElement]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmEntityContainer]] EntityContainerDuplicateEntityContainerMemberName = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmEntityContainer]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmEntityContainerElement]] EntityContainerElementMustNotHaveKindOfNone = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmEntityContainerElement]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmEntityReferenceType]] EntityReferenceTypeInaccessibleEntityType = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmEntityReferenceType]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmEntitySet]] EntitySetCanOnlyBeContainedByASingleNavigationProperty = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmEntitySet]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmEntitySet]] EntitySetRecursiveNavigationPropertyMappingsMustPointBackToSourceEntitySet = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmEntitySet]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmEntitySet]] EntitySetTypeMustBeCollectionOfEntityType = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmEntitySet]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmEntityType]] EntityTypeDuplicatePropertyNameSpecifiedInEntityKey = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmEntityType]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmEntityType]] EntityTypeEntityKeyMustBeScalar = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmEntityType]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmEntityType]] EntityTypeInvalidKeyKeyDefinedInBaseClass = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmEntityType]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmEntityType]] EntityTypeInvalidKeyNullablePart = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmEntityType]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmEntityType]] EntityTypeKeyMissingOnEntityType = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmEntityType]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmEntityType]] EntityTypeKeyPropertyMustBelongToEntity = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmEntityType]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmEnumMember]] EnumMemberValueMustHaveSameTypeAsUnderlyingType = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmEnumMember]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmEnumType]] EnumMustHaveIntegerUnderlyingType = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmEnumType]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmEnumType]] EnumTypeEnumMemberNameAlreadyDefined = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmEnumType]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmFunctionImport]] FunctionImportWithParameterShouldNotBeIncludedInServiceDocument = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmFunctionImport]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmFunction]] FunctionMustHaveReturnType = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmFunction]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.Expressions.IEdmIfExpression]] IfExpressionAssertCorrectTestType = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.Expressions.IEdmIfExpression]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.Annotations.IEdmDirectValueAnnotation]] ImmediateValueAnnotationElementAnnotationHasNameAndNamespace = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.Annotations.IEdmDirectValueAnnotation]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.Annotations.IEdmDirectValueAnnotation]] ImmediateValueAnnotationElementAnnotationIsValid = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.Annotations.IEdmDirectValueAnnotation]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmModel]] ModelBoundFunctionOverloadsMustHaveSameReturnType = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmModel]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmModel]] ModelDuplicateEntityContainerName = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmModel]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmModel]] ModelDuplicateSchemaElementName = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmModel]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmNamedElement]] NamedElementNameIsNotAllowed = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmNamedElement]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmNamedElement]] NamedElementNameIsTooLong = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmNamedElement]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmNamedElement]] NamedElementNameMustNotBeEmptyOrWhiteSpace = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmNamedElement]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmNavigationSource]] NavigationMappingMustBeBidirectional = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmNavigationSource]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmNavigationProperty]] NavigationPropertyCorrectType = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmNavigationProperty]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmNavigationProperty]] NavigationPropertyDependentEndMultiplicity = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmNavigationProperty]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmNavigationProperty]] NavigationPropertyDependentPropertiesMustBelongToDependentEntity = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmNavigationProperty]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmNavigationProperty]] NavigationPropertyDuplicateDependentProperty = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmNavigationProperty]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmNavigationProperty]] NavigationPropertyEndWithManyMultiplicityCannotHaveOperationsSpecified = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmNavigationProperty]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmNavigationProperty]] NavigationPropertyEntityMustNotIndirectlyContainItself = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmNavigationProperty]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmNavigationProperty]] NavigationPropertyInvalidOperationMultipleEndsInAssociatedNavigationProperties = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmNavigationProperty]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmNavigationSource]] NavigationPropertyMappingMustPointToValidTargetForProperty = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmNavigationSource]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmNavigationSource]] NavigationPropertyMappingsMustBeUnique = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmNavigationSource]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmNavigationProperty]] NavigationPropertyPrincipalEndMultiplicity = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmNavigationProperty]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmNavigationProperty]] NavigationPropertyTypeMismatchRelationshipConstraint = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmNavigationProperty]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmNavigationProperty]] NavigationPropertyWithNonRecursiveContainmentSourceMustBeFromOne = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmNavigationProperty]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmNavigationProperty]] NavigationPropertyWithRecursiveContainmentSourceMustBeFromZeroOrOne = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmNavigationProperty]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmNavigationProperty]] NavigationPropertyWithRecursiveContainmentTargetMustBeOptional = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmNavigationProperty]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmNavigationSource]] NavigationSourceInaccessibleEntityType = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmNavigationSource]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmNavigationSource]] NavigationSourceTypeHasNoKeys = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmNavigationSource]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmStructuredType]] OnlyEntityTypesCanBeOpen = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmStructuredType]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmComplexType]] OpenComplexTypeCannotHaveClosedDerivedComplexType = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmComplexType]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.Expressions.IEdmApplyExpression]] OperationApplicationExpressionParametersMatchAppliedOperation = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.Expressions.IEdmApplyExpression]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmOperation]] OperationEntitySetPathMustBeValid = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmOperation]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmOperationImport]] OperationImportCannotImportBoundOperation = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmOperationImport]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmOperationImport]] OperationImportEntitySetExpressionIsInvalid = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmOperationImport]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmOperationImport]] OperationImportEntityTypeDoesNotMatchEntitySet = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmOperationImport]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmOperation]] OperationParameterNameAlreadyDefinedDuplicate = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmOperation]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmOperation]] OperationReturnTypeEntityTypeMustBeValid = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmOperation]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmOperation]] OperationUnsupportedReturnType = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmOperation]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmPrimitiveType]] PrimitiveTypeMustNotHaveKindOfNone = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmPrimitiveType]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.Values.IEdmPrimitiveValue]] PrimitiveValueValidForType = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.Values.IEdmPrimitiveValue]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmProperty]] PropertyMustNotHaveKindOfNone = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmProperty]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.Annotations.IEdmPropertyValueBinding]] PropertyValueBindingValueIsCorrectType = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.Annotations.IEdmPropertyValueBinding]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.Expressions.IEdmRecordExpression]] RecordExpressionPropertiesMatchType = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.Expressions.IEdmRecordExpression]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmSchemaElement]] SchemaElementMustNotHaveKindOfNone = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmSchemaElement]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmSchemaElement]] SchemaElementNamespaceIsNotAllowed = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmSchemaElement]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmSchemaElement]] SchemaElementNamespaceIsTooLong = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmSchemaElement]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmSchemaElement]] SchemaElementNamespaceMustNotBeEmptyOrWhiteSpace = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmSchemaElement]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmSchemaElement]] SchemaElementSystemNamespaceEncountered = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmSchemaElement]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmSingleton]] SingletonTypeMustBeEntityType = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmSingleton]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmStringTypeReference]] StringTypeReferenceStringMaxLengthNegative = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmStringTypeReference]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmStringTypeReference]] StringTypeReferenceStringUnboundedNotValidForMaxLength = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmStringTypeReference]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmStructuralProperty]] StructuralPropertyInvalidPropertyType = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmStructuralProperty]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmStructuralProperty]] StructuralPropertyInvalidPropertyTypeConcurrencyMode = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmStructuralProperty]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmStructuredType]] StructuredTypeBaseTypeMustBeSameKindAsDerivedKind = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmStructuredType]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmStructuredType]] StructuredTypeInaccessibleBaseType = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmStructuredType]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmStructuredType]] StructuredTypeInvalidMemberNameMatchesTypeName = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmStructuredType]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmStructuredType]] StructuredTypePropertiesDeclaringTypeMustBeCorrect = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmStructuredType]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmStructuredType]] StructuredTypePropertyNameAlreadyDefined = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmStructuredType]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmTemporalTypeReference]] TemporalTypeReferencePrecisionOutOfRange = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmTemporalTypeReference]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmTerm]] TermMustNotHaveKindOfNone = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmTerm]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmType]] TypeMustNotHaveKindOfNone = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmType]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmTypeReference]] TypeReferenceInaccessibleSchemaType = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmTypeReference]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmModel]] UnBoundFunctionOverloadsMustHaveIdenticalReturnTypes = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmModel]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.Annotations.IEdmValueAnnotation]] ValueAnnotationAssertCorrectExpressionType = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.Annotations.IEdmValueAnnotation]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.IEdmVocabularyAnnotatable]] VocabularyAnnotatableNoDuplicateAnnotations = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.IEdmVocabularyAnnotatable]
	public static readonly Microsoft.OData.Edm.Validation.ValidationRule`1[[Microsoft.OData.Edm.Annotations.IEdmVocabularyAnnotation]] VocabularyAnnotationInaccessibleTarget = Microsoft.OData.Edm.Validation.ValidationRule`1[Microsoft.OData.Edm.Annotations.IEdmVocabularyAnnotation]
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

public enum Microsoft.OData.Edm.Values.EdmValueKind : int {
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

public interface Microsoft.OData.Edm.Values.IEdmBinaryValue : IEdmElement, IEdmPrimitiveValue, IEdmValue {
	byte[] Value  { public abstract get; }
}

public interface Microsoft.OData.Edm.Values.IEdmBooleanValue : IEdmElement, IEdmPrimitiveValue, IEdmValue {
	bool Value  { public abstract get; }
}

public interface Microsoft.OData.Edm.Values.IEdmCollectionValue : IEdmElement, IEdmValue {
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Values.IEdmDelayedValue]] Elements  { public abstract get; }
}

public interface Microsoft.OData.Edm.Values.IEdmDateTimeOffsetValue : IEdmElement, IEdmPrimitiveValue, IEdmValue {
	System.DateTimeOffset Value  { public abstract get; }
}

public interface Microsoft.OData.Edm.Values.IEdmDateValue : IEdmElement, IEdmPrimitiveValue, IEdmValue {
	Microsoft.OData.Edm.Library.Date Value  { public abstract get; }
}

public interface Microsoft.OData.Edm.Values.IEdmDecimalValue : IEdmElement, IEdmPrimitiveValue, IEdmValue {
	decimal Value  { public abstract get; }
}

public interface Microsoft.OData.Edm.Values.IEdmDelayedValue {
	Microsoft.OData.Edm.Values.IEdmValue Value  { public abstract get; }
}

public interface Microsoft.OData.Edm.Values.IEdmDurationValue : IEdmElement, IEdmPrimitiveValue, IEdmValue {
	System.TimeSpan Value  { public abstract get; }
}

public interface Microsoft.OData.Edm.Values.IEdmEnumValue : IEdmElement, IEdmPrimitiveValue, IEdmValue {
	Microsoft.OData.Edm.Values.IEdmPrimitiveValue Value  { public abstract get; }
}

public interface Microsoft.OData.Edm.Values.IEdmFloatingValue : IEdmElement, IEdmPrimitiveValue, IEdmValue {
	double Value  { public abstract get; }
}

public interface Microsoft.OData.Edm.Values.IEdmGuidValue : IEdmElement, IEdmPrimitiveValue, IEdmValue {
	System.Guid Value  { public abstract get; }
}

public interface Microsoft.OData.Edm.Values.IEdmIntegerValue : IEdmElement, IEdmPrimitiveValue, IEdmValue {
	long Value  { public abstract get; }
}

public interface Microsoft.OData.Edm.Values.IEdmNullValue : IEdmElement, IEdmValue {
}

public interface Microsoft.OData.Edm.Values.IEdmPrimitiveValue : IEdmElement, IEdmValue {
}

public interface Microsoft.OData.Edm.Values.IEdmPropertyValue : IEdmDelayedValue {
	string Name  { public abstract get; }
}

public interface Microsoft.OData.Edm.Values.IEdmStringValue : IEdmElement, IEdmPrimitiveValue, IEdmValue {
	string Value  { public abstract get; }
}

public interface Microsoft.OData.Edm.Values.IEdmStructuredValue : IEdmElement, IEdmValue {
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Values.IEdmPropertyValue]] PropertyValues  { public abstract get; }

	Microsoft.OData.Edm.Values.IEdmPropertyValue FindPropertyValue (string propertyName)
}

public interface Microsoft.OData.Edm.Values.IEdmTimeOfDayValue : IEdmElement, IEdmPrimitiveValue, IEdmValue {
	Microsoft.OData.Edm.Library.TimeOfDay Value  { public abstract get; }
}

public interface Microsoft.OData.Edm.Values.IEdmValue : IEdmElement {
	Microsoft.OData.Edm.IEdmTypeReference Type  { public abstract get; }
	Microsoft.OData.Edm.Values.EdmValueKind ValueKind  { public abstract get; }
}

public sealed class Microsoft.OData.Edm.Vocabularis.CapabilitiesVocabularyConstants {
	public static string CapabilitiesChangeTracking = "Org.OData.Capabilities.V1.ChangeTracking"
	public static string CapabilitiesChangeTrackingExpandableProperties = "ExpandableProperties"
	public static string CapabilitiesChangeTrackingFilterableProperties = "FilterableProperties"
	public static string CapabilitiesChangeTrackingSupported = "Supported"
}

public sealed class Microsoft.OData.Edm.Vocabularis.CoreVocabularyConstants {
	public static string CoreDescription = "Org.OData.Core.V1.Description"
	public static string CoreLongDescription = "Org.OData.Core.V1.LongDescription"
	public static string CoreOptimisticConcurrency = "Org.OData.Core.V1.OptimisticConcurrency"
	public static string CoreOptimisticConcurrencyControl = "Org.OData.Core.V1.OptimisticConcurrencyControl"
}

public abstract class Microsoft.OData.Edm.Library.Annotations.EdmVocabularyAnnotation : Microsoft.OData.Edm.Library.EdmElement, IEdmElement, IEdmVocabularyAnnotation {
	protected EdmVocabularyAnnotation (Microsoft.OData.Edm.IEdmVocabularyAnnotatable target, Microsoft.OData.Edm.IEdmValueTerm term, string qualifier)

	string Qualifier  { public virtual get; }
	Microsoft.OData.Edm.IEdmVocabularyAnnotatable Target  { public virtual get; }
	Microsoft.OData.Edm.IEdmTerm Term  { public virtual get; }
}

public class Microsoft.OData.Edm.Library.Annotations.EdmAnnotation : Microsoft.OData.Edm.Library.Annotations.EdmVocabularyAnnotation, IEdmElement, IEdmValueAnnotation, IEdmVocabularyAnnotation {
	public EdmAnnotation (Microsoft.OData.Edm.IEdmVocabularyAnnotatable target, Microsoft.OData.Edm.IEdmValueTerm term, Microsoft.OData.Edm.Expressions.IEdmExpression value)
	public EdmAnnotation (Microsoft.OData.Edm.IEdmVocabularyAnnotatable target, Microsoft.OData.Edm.IEdmValueTerm term, string qualifier, Microsoft.OData.Edm.Expressions.IEdmExpression value)

	Microsoft.OData.Edm.Expressions.IEdmExpression Value  { public virtual get; }
}

public class Microsoft.OData.Edm.Library.Annotations.EdmDirectValueAnnotation : Microsoft.OData.Edm.Library.EdmNamedElement, IEdmElement, IEdmNamedElement, IEdmDirectValueAnnotation {
	public EdmDirectValueAnnotation (string namespaceUri, string name, object value)

	string NamespaceUri  { public virtual get; }
	object Value  { public virtual get; }
}

public class Microsoft.OData.Edm.Library.Annotations.EdmDirectValueAnnotationBinding : IEdmDirectValueAnnotationBinding {
	public EdmDirectValueAnnotationBinding (Microsoft.OData.Edm.IEdmElement element, string namespaceUri, string name)
	public EdmDirectValueAnnotationBinding (Microsoft.OData.Edm.IEdmElement element, string namespaceUri, string name, object value)

	Microsoft.OData.Edm.IEdmElement Element  { public virtual get; }
	string Name  { public virtual get; }
	string NamespaceUri  { public virtual get; }
	object Value  { public virtual get; }
}

public class Microsoft.OData.Edm.Library.Annotations.EdmDirectValueAnnotationsManager : IEdmDirectValueAnnotationsManager {
	public EdmDirectValueAnnotationsManager ()

	public virtual object GetAnnotationValue (Microsoft.OData.Edm.IEdmElement element, string namespaceName, string localName)
	public virtual object[] GetAnnotationValues (System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Annotations.IEdmDirectValueAnnotationBinding]] annotations)
	protected virtual System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Annotations.IEdmDirectValueAnnotation]] GetAttachedAnnotations (Microsoft.OData.Edm.IEdmElement element)
	public virtual System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Annotations.IEdmDirectValueAnnotation]] GetDirectValueAnnotations (Microsoft.OData.Edm.IEdmElement element)
	public virtual void SetAnnotationValue (Microsoft.OData.Edm.IEdmElement element, string namespaceName, string localName, object value)
	public virtual void SetAnnotationValues (System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Annotations.IEdmDirectValueAnnotationBinding]] annotations)
}

public class Microsoft.OData.Edm.Library.Annotations.EdmPropertyValueBinding : Microsoft.OData.Edm.Library.EdmElement, IEdmElement, IEdmPropertyValueBinding {
	public EdmPropertyValueBinding (Microsoft.OData.Edm.IEdmProperty boundProperty, Microsoft.OData.Edm.Expressions.IEdmExpression value)

	Microsoft.OData.Edm.IEdmProperty BoundProperty  { public virtual get; }
	Microsoft.OData.Edm.Expressions.IEdmExpression Value  { public virtual get; }
}

public class Microsoft.OData.Edm.Library.Annotations.EdmTypedDirectValueAnnotationBinding`1 : Microsoft.OData.Edm.Library.EdmNamedElement, IEdmElement, IEdmNamedElement, IEdmDirectValueAnnotationBinding {
	public EdmTypedDirectValueAnnotationBinding`1 (Microsoft.OData.Edm.IEdmElement element, T value)

	Microsoft.OData.Edm.IEdmElement Element  { public virtual get; }
	string NamespaceUri  { public virtual get; }
	object Value  { public virtual get; }
}

public class Microsoft.OData.Edm.Library.Expressions.EdmApplyExpression : Microsoft.OData.Edm.Library.EdmElement, IEdmElement, IEdmApplyExpression, IEdmExpression {
	public EdmApplyExpression (Microsoft.OData.Edm.Expressions.IEdmExpression appliedOperation, System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Expressions.IEdmExpression]] arguments)
	public EdmApplyExpression (Microsoft.OData.Edm.IEdmOperation appliedOperation, Microsoft.OData.Edm.Expressions.IEdmExpression[] arguments)
	public EdmApplyExpression (Microsoft.OData.Edm.IEdmOperation appliedOperation, System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Expressions.IEdmExpression]] arguments)

	Microsoft.OData.Edm.Expressions.IEdmExpression AppliedOperation  { public virtual get; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Expressions.IEdmExpression]] Arguments  { public virtual get; }
	Microsoft.OData.Edm.Expressions.EdmExpressionKind ExpressionKind  { public virtual get; }
}

public class Microsoft.OData.Edm.Library.Expressions.EdmCastExpression : Microsoft.OData.Edm.Library.EdmElement, IEdmElement, IEdmCastExpression, IEdmExpression {
	public EdmCastExpression (Microsoft.OData.Edm.Expressions.IEdmExpression operand, Microsoft.OData.Edm.IEdmTypeReference type)

	Microsoft.OData.Edm.Expressions.EdmExpressionKind ExpressionKind  { public virtual get; }
	Microsoft.OData.Edm.Expressions.IEdmExpression Operand  { public virtual get; }
	Microsoft.OData.Edm.IEdmTypeReference Type  { public virtual get; }
}

public class Microsoft.OData.Edm.Library.Expressions.EdmCollectionExpression : Microsoft.OData.Edm.Library.EdmElement, IEdmElement, IEdmCollectionExpression, IEdmExpression {
	public EdmCollectionExpression (Microsoft.OData.Edm.Expressions.IEdmExpression[] elements)
	public EdmCollectionExpression (System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Expressions.IEdmExpression]] elements)
	public EdmCollectionExpression (Microsoft.OData.Edm.IEdmTypeReference declaredType, Microsoft.OData.Edm.Expressions.IEdmExpression[] elements)
	public EdmCollectionExpression (Microsoft.OData.Edm.IEdmTypeReference declaredType, System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Expressions.IEdmExpression]] elements)

	Microsoft.OData.Edm.IEdmTypeReference DeclaredType  { public virtual get; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Expressions.IEdmExpression]] Elements  { public virtual get; }
	Microsoft.OData.Edm.Expressions.EdmExpressionKind ExpressionKind  { public virtual get; }
}

public class Microsoft.OData.Edm.Library.Expressions.EdmEntitySetReferenceExpression : Microsoft.OData.Edm.Library.EdmElement, IEdmElement, IEdmEntitySetReferenceExpression, IEdmExpression {
	public EdmEntitySetReferenceExpression (Microsoft.OData.Edm.IEdmEntitySet referencedEntitySet)

	Microsoft.OData.Edm.Expressions.EdmExpressionKind ExpressionKind  { public virtual get; }
	Microsoft.OData.Edm.IEdmEntitySet ReferencedEntitySet  { public virtual get; }
}

public class Microsoft.OData.Edm.Library.Expressions.EdmEnumMemberExpression : Microsoft.OData.Edm.Library.EdmElement, IEdmElement, IEdmEnumMemberExpression, IEdmExpression {
	public EdmEnumMemberExpression (Microsoft.OData.Edm.IEdmEnumMember[] enumMembers)

	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmEnumMember]] EnumMembers  { public virtual get; }
	Microsoft.OData.Edm.Expressions.EdmExpressionKind ExpressionKind  { public virtual get; }
}

public class Microsoft.OData.Edm.Library.Expressions.EdmEnumMemberReferenceExpression : Microsoft.OData.Edm.Library.EdmElement, IEdmElement, IEdmEnumMemberReferenceExpression, IEdmExpression {
	public EdmEnumMemberReferenceExpression (Microsoft.OData.Edm.IEdmEnumMember referencedEnumMember)

	Microsoft.OData.Edm.Expressions.EdmExpressionKind ExpressionKind  { public virtual get; }
	Microsoft.OData.Edm.IEdmEnumMember ReferencedEnumMember  { public virtual get; }
}

public class Microsoft.OData.Edm.Library.Expressions.EdmIfExpression : Microsoft.OData.Edm.Library.EdmElement, IEdmElement, IEdmExpression, IEdmIfExpression {
	public EdmIfExpression (Microsoft.OData.Edm.Expressions.IEdmExpression testExpression, Microsoft.OData.Edm.Expressions.IEdmExpression trueExpression, Microsoft.OData.Edm.Expressions.IEdmExpression falseExpression)

	Microsoft.OData.Edm.Expressions.EdmExpressionKind ExpressionKind  { public virtual get; }
	Microsoft.OData.Edm.Expressions.IEdmExpression FalseExpression  { public virtual get; }
	Microsoft.OData.Edm.Expressions.IEdmExpression TestExpression  { public virtual get; }
	Microsoft.OData.Edm.Expressions.IEdmExpression TrueExpression  { public virtual get; }
}

public class Microsoft.OData.Edm.Library.Expressions.EdmIsTypeExpression : Microsoft.OData.Edm.Library.EdmElement, IEdmElement, IEdmExpression, IEdmIsTypeExpression {
	public EdmIsTypeExpression (Microsoft.OData.Edm.Expressions.IEdmExpression operand, Microsoft.OData.Edm.IEdmTypeReference type)

	Microsoft.OData.Edm.Expressions.EdmExpressionKind ExpressionKind  { public virtual get; }
	Microsoft.OData.Edm.Expressions.IEdmExpression Operand  { public virtual get; }
	Microsoft.OData.Edm.IEdmTypeReference Type  { public virtual get; }
}

public class Microsoft.OData.Edm.Library.Expressions.EdmLabeledExpression : Microsoft.OData.Edm.Library.EdmElement, IEdmElement, IEdmNamedElement, IEdmExpression, IEdmLabeledExpression {
	public EdmLabeledExpression (string name, Microsoft.OData.Edm.Expressions.IEdmExpression expression)

	Microsoft.OData.Edm.Expressions.IEdmExpression Expression  { public virtual get; }
	Microsoft.OData.Edm.Expressions.EdmExpressionKind ExpressionKind  { public virtual get; }
	string Name  { public virtual get; }
}

public class Microsoft.OData.Edm.Library.Expressions.EdmLabeledExpressionReferenceExpression : Microsoft.OData.Edm.Library.EdmElement, IEdmElement, IEdmExpression, IEdmLabeledExpressionReferenceExpression {
	public EdmLabeledExpressionReferenceExpression ()
	public EdmLabeledExpressionReferenceExpression (Microsoft.OData.Edm.Expressions.IEdmLabeledExpression referencedLabeledExpression)

	Microsoft.OData.Edm.Expressions.EdmExpressionKind ExpressionKind  { public virtual get; }
	Microsoft.OData.Edm.Expressions.IEdmLabeledExpression ReferencedLabeledExpression  { public virtual get; public set; }
}

public class Microsoft.OData.Edm.Library.Expressions.EdmNavigationPropertyPathExpression : Microsoft.OData.Edm.Library.Expressions.EdmPathExpression, IEdmElement, IEdmExpression, IEdmPathExpression {
	public EdmNavigationPropertyPathExpression (System.Collections.Generic.IEnumerable`1[[System.String]] path)
	public EdmNavigationPropertyPathExpression (string path)
	public EdmNavigationPropertyPathExpression (string[] path)

	Microsoft.OData.Edm.Expressions.EdmExpressionKind ExpressionKind  { public virtual get; }
}

public class Microsoft.OData.Edm.Library.Expressions.EdmOperationReferenceExpression : Microsoft.OData.Edm.Library.EdmElement, IEdmElement, IEdmExpression, IEdmOperationReferenceExpression {
	public EdmOperationReferenceExpression (Microsoft.OData.Edm.IEdmOperation referencedOperation)

	Microsoft.OData.Edm.Expressions.EdmExpressionKind ExpressionKind  { public virtual get; }
	Microsoft.OData.Edm.IEdmOperation ReferencedOperation  { public virtual get; }
}

public class Microsoft.OData.Edm.Library.Expressions.EdmParameterReferenceExpression : Microsoft.OData.Edm.Library.EdmElement, IEdmElement, IEdmExpression, IEdmParameterReferenceExpression {
	public EdmParameterReferenceExpression (Microsoft.OData.Edm.IEdmOperationParameter referencedParameter)

	Microsoft.OData.Edm.Expressions.EdmExpressionKind ExpressionKind  { public virtual get; }
	Microsoft.OData.Edm.IEdmOperationParameter ReferencedParameter  { public virtual get; }
}

public class Microsoft.OData.Edm.Library.Expressions.EdmPathExpression : Microsoft.OData.Edm.Library.EdmElement, IEdmElement, IEdmExpression, IEdmPathExpression {
	public EdmPathExpression (System.Collections.Generic.IEnumerable`1[[System.String]] path)
	public EdmPathExpression (string path)
	public EdmPathExpression (string[] path)

	Microsoft.OData.Edm.Expressions.EdmExpressionKind ExpressionKind  { public virtual get; }
	System.Collections.Generic.IEnumerable`1[[System.String]] Path  { public virtual get; }
}

public class Microsoft.OData.Edm.Library.Expressions.EdmPropertyConstructor : Microsoft.OData.Edm.Library.EdmElement, IEdmElement, IEdmPropertyConstructor {
	public EdmPropertyConstructor (string name, Microsoft.OData.Edm.Expressions.IEdmExpression value)

	string Name  { public virtual get; }
	Microsoft.OData.Edm.Expressions.IEdmExpression Value  { public virtual get; }
}

public class Microsoft.OData.Edm.Library.Expressions.EdmPropertyPathExpression : Microsoft.OData.Edm.Library.Expressions.EdmPathExpression, IEdmElement, IEdmExpression, IEdmPathExpression {
	public EdmPropertyPathExpression (System.Collections.Generic.IEnumerable`1[[System.String]] path)
	public EdmPropertyPathExpression (string path)
	public EdmPropertyPathExpression (string[] path)

	Microsoft.OData.Edm.Expressions.EdmExpressionKind ExpressionKind  { public virtual get; }
}

public class Microsoft.OData.Edm.Library.Expressions.EdmPropertyReferenceExpression : Microsoft.OData.Edm.Library.EdmElement, IEdmElement, IEdmExpression, IEdmPropertyReferenceExpression {
	public EdmPropertyReferenceExpression (Microsoft.OData.Edm.Expressions.IEdmExpression baseExpression, Microsoft.OData.Edm.IEdmProperty referencedProperty)

	Microsoft.OData.Edm.Expressions.IEdmExpression Base  { public virtual get; }
	Microsoft.OData.Edm.Expressions.EdmExpressionKind ExpressionKind  { public virtual get; }
	Microsoft.OData.Edm.IEdmProperty ReferencedProperty  { public virtual get; }
}

public class Microsoft.OData.Edm.Library.Expressions.EdmRecordExpression : Microsoft.OData.Edm.Library.EdmElement, IEdmElement, IEdmExpression, IEdmRecordExpression {
	public EdmRecordExpression (Microsoft.OData.Edm.Expressions.IEdmPropertyConstructor[] properties)
	public EdmRecordExpression (System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Expressions.IEdmPropertyConstructor]] properties)
	public EdmRecordExpression (Microsoft.OData.Edm.IEdmStructuredTypeReference declaredType, Microsoft.OData.Edm.Expressions.IEdmPropertyConstructor[] properties)
	public EdmRecordExpression (Microsoft.OData.Edm.IEdmStructuredTypeReference declaredType, System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Expressions.IEdmPropertyConstructor]] properties)

	Microsoft.OData.Edm.IEdmStructuredTypeReference DeclaredType  { public virtual get; }
	Microsoft.OData.Edm.Expressions.EdmExpressionKind ExpressionKind  { public virtual get; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Expressions.IEdmPropertyConstructor]] Properties  { public virtual get; }
}

public class Microsoft.OData.Edm.Library.Expressions.EdmValueTermReferenceExpression : Microsoft.OData.Edm.Library.EdmElement, IEdmElement, IEdmExpression, IEdmValueTermReferenceExpression {
	public EdmValueTermReferenceExpression (Microsoft.OData.Edm.Expressions.IEdmExpression baseExpression, Microsoft.OData.Edm.IEdmValueTerm term)
	public EdmValueTermReferenceExpression (Microsoft.OData.Edm.Expressions.IEdmExpression baseExpression, Microsoft.OData.Edm.IEdmValueTerm term, string qualifier)

	Microsoft.OData.Edm.Expressions.IEdmExpression Base  { public virtual get; }
	Microsoft.OData.Edm.Expressions.EdmExpressionKind ExpressionKind  { public virtual get; }
	string Qualifier  { public virtual get; }
	Microsoft.OData.Edm.IEdmValueTerm Term  { public virtual get; }
}

public abstract class Microsoft.OData.Edm.Library.Values.EdmValue : IEdmElement, IEdmDelayedValue, IEdmValue {
	protected EdmValue (Microsoft.OData.Edm.IEdmTypeReference type)

	Microsoft.OData.Edm.IEdmTypeReference Type  { public virtual get; }
	Microsoft.OData.Edm.Values.EdmValueKind ValueKind  { public abstract get; }
}

public class Microsoft.OData.Edm.Library.Values.EdmBinaryConstant : Microsoft.OData.Edm.Library.Values.EdmValue, IEdmElement, IEdmBinaryConstantExpression, IEdmExpression, IEdmBinaryValue, IEdmDelayedValue, IEdmPrimitiveValue, IEdmValue {
	public EdmBinaryConstant (byte[] value)
	public EdmBinaryConstant (Microsoft.OData.Edm.IEdmBinaryTypeReference type, byte[] value)

	Microsoft.OData.Edm.Expressions.EdmExpressionKind ExpressionKind  { public virtual get; }
	byte[] Value  { public virtual get; }
	Microsoft.OData.Edm.Values.EdmValueKind ValueKind  { public virtual get; }
}

public class Microsoft.OData.Edm.Library.Values.EdmBooleanConstant : Microsoft.OData.Edm.Library.Values.EdmValue, IEdmElement, IEdmBooleanConstantExpression, IEdmExpression, IEdmBooleanValue, IEdmDelayedValue, IEdmPrimitiveValue, IEdmValue {
	public EdmBooleanConstant (bool value)
	public EdmBooleanConstant (Microsoft.OData.Edm.IEdmPrimitiveTypeReference type, bool value)

	Microsoft.OData.Edm.Expressions.EdmExpressionKind ExpressionKind  { public virtual get; }
	bool Value  { public virtual get; }
	Microsoft.OData.Edm.Values.EdmValueKind ValueKind  { public virtual get; }
}

public class Microsoft.OData.Edm.Library.Values.EdmCollectionValue : Microsoft.OData.Edm.Library.Values.EdmValue, IEdmElement, IEdmCollectionValue, IEdmDelayedValue, IEdmValue {
	public EdmCollectionValue (Microsoft.OData.Edm.IEdmCollectionTypeReference type, System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Values.IEdmDelayedValue]] elements)

	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Values.IEdmDelayedValue]] Elements  { public virtual get; }
	Microsoft.OData.Edm.Values.EdmValueKind ValueKind  { public virtual get; }
}

public class Microsoft.OData.Edm.Library.Values.EdmDateConstant : Microsoft.OData.Edm.Library.Values.EdmValue, IEdmElement, IEdmDateConstantExpression, IEdmExpression, IEdmDateValue, IEdmDelayedValue, IEdmPrimitiveValue, IEdmValue {
	public EdmDateConstant (Microsoft.OData.Edm.Library.Date value)
	public EdmDateConstant (Microsoft.OData.Edm.IEdmPrimitiveTypeReference type, Microsoft.OData.Edm.Library.Date value)

	Microsoft.OData.Edm.Expressions.EdmExpressionKind ExpressionKind  { public virtual get; }
	Microsoft.OData.Edm.Library.Date Value  { public virtual get; }
	Microsoft.OData.Edm.Values.EdmValueKind ValueKind  { public virtual get; }
}

public class Microsoft.OData.Edm.Library.Values.EdmDateTimeOffsetConstant : Microsoft.OData.Edm.Library.Values.EdmValue, IEdmElement, IEdmDateTimeOffsetConstantExpression, IEdmExpression, IEdmDateTimeOffsetValue, IEdmDelayedValue, IEdmPrimitiveValue, IEdmValue {
	public EdmDateTimeOffsetConstant (System.DateTimeOffset value)
	public EdmDateTimeOffsetConstant (Microsoft.OData.Edm.IEdmTemporalTypeReference type, System.DateTimeOffset value)

	Microsoft.OData.Edm.Expressions.EdmExpressionKind ExpressionKind  { public virtual get; }
	System.DateTimeOffset Value  { public virtual get; }
	Microsoft.OData.Edm.Values.EdmValueKind ValueKind  { public virtual get; }
}

public class Microsoft.OData.Edm.Library.Values.EdmDecimalConstant : Microsoft.OData.Edm.Library.Values.EdmValue, IEdmElement, IEdmDecimalConstantExpression, IEdmExpression, IEdmDecimalValue, IEdmDelayedValue, IEdmPrimitiveValue, IEdmValue {
	public EdmDecimalConstant (decimal value)
	public EdmDecimalConstant (Microsoft.OData.Edm.IEdmDecimalTypeReference type, decimal value)

	Microsoft.OData.Edm.Expressions.EdmExpressionKind ExpressionKind  { public virtual get; }
	decimal Value  { public virtual get; }
	Microsoft.OData.Edm.Values.EdmValueKind ValueKind  { public virtual get; }
}

public class Microsoft.OData.Edm.Library.Values.EdmDurationConstant : Microsoft.OData.Edm.Library.Values.EdmValue, IEdmElement, IEdmDurationConstantExpression, IEdmExpression, IEdmDelayedValue, IEdmDurationValue, IEdmPrimitiveValue, IEdmValue {
	public EdmDurationConstant (System.TimeSpan value)
	public EdmDurationConstant (Microsoft.OData.Edm.IEdmTemporalTypeReference type, System.TimeSpan value)

	Microsoft.OData.Edm.Expressions.EdmExpressionKind ExpressionKind  { public virtual get; }
	System.TimeSpan Value  { public virtual get; }
	Microsoft.OData.Edm.Values.EdmValueKind ValueKind  { public virtual get; }
}

public class Microsoft.OData.Edm.Library.Values.EdmEnumValue : Microsoft.OData.Edm.Library.Values.EdmValue, IEdmElement, IEdmDelayedValue, IEdmEnumValue, IEdmPrimitiveValue, IEdmValue {
	public EdmEnumValue (Microsoft.OData.Edm.IEdmEnumTypeReference type, Microsoft.OData.Edm.IEdmEnumMember member)
	public EdmEnumValue (Microsoft.OData.Edm.IEdmEnumTypeReference type, Microsoft.OData.Edm.Values.IEdmPrimitiveValue value)

	Microsoft.OData.Edm.Values.IEdmPrimitiveValue Value  { public virtual get; }
	Microsoft.OData.Edm.Values.EdmValueKind ValueKind  { public virtual get; }
}

public class Microsoft.OData.Edm.Library.Values.EdmFloatingConstant : Microsoft.OData.Edm.Library.Values.EdmValue, IEdmElement, IEdmExpression, IEdmFloatingConstantExpression, IEdmDelayedValue, IEdmFloatingValue, IEdmPrimitiveValue, IEdmValue {
	public EdmFloatingConstant (double value)
	public EdmFloatingConstant (Microsoft.OData.Edm.IEdmPrimitiveTypeReference type, double value)

	Microsoft.OData.Edm.Expressions.EdmExpressionKind ExpressionKind  { public virtual get; }
	double Value  { public virtual get; }
	Microsoft.OData.Edm.Values.EdmValueKind ValueKind  { public virtual get; }
}

public class Microsoft.OData.Edm.Library.Values.EdmGuidConstant : Microsoft.OData.Edm.Library.Values.EdmValue, IEdmElement, IEdmExpression, IEdmGuidConstantExpression, IEdmDelayedValue, IEdmGuidValue, IEdmPrimitiveValue, IEdmValue {
	public EdmGuidConstant (System.Guid value)
	public EdmGuidConstant (Microsoft.OData.Edm.IEdmPrimitiveTypeReference type, System.Guid value)

	Microsoft.OData.Edm.Expressions.EdmExpressionKind ExpressionKind  { public virtual get; }
	System.Guid Value  { public virtual get; }
	Microsoft.OData.Edm.Values.EdmValueKind ValueKind  { public virtual get; }
}

public class Microsoft.OData.Edm.Library.Values.EdmIntegerConstant : Microsoft.OData.Edm.Library.Values.EdmValue, IEdmElement, IEdmExpression, IEdmIntegerConstantExpression, IEdmDelayedValue, IEdmIntegerValue, IEdmPrimitiveValue, IEdmValue {
	public EdmIntegerConstant (long value)
	public EdmIntegerConstant (Microsoft.OData.Edm.IEdmPrimitiveTypeReference type, long value)

	Microsoft.OData.Edm.Expressions.EdmExpressionKind ExpressionKind  { public virtual get; }
	long Value  { public virtual get; }
	Microsoft.OData.Edm.Values.EdmValueKind ValueKind  { public virtual get; }
}

public class Microsoft.OData.Edm.Library.Values.EdmNullExpression : Microsoft.OData.Edm.Library.Values.EdmValue, IEdmElement, IEdmExpression, IEdmNullExpression, IEdmDelayedValue, IEdmNullValue, IEdmValue {
	public static Microsoft.OData.Edm.Library.Values.EdmNullExpression Instance = Microsoft.OData.Edm.Library.Values.EdmNullExpression

	Microsoft.OData.Edm.Expressions.EdmExpressionKind ExpressionKind  { public virtual get; }
	Microsoft.OData.Edm.Values.EdmValueKind ValueKind  { public virtual get; }
}

public class Microsoft.OData.Edm.Library.Values.EdmPropertyValue : IEdmDelayedValue, IEdmPropertyValue {
	public EdmPropertyValue (string name)
	public EdmPropertyValue (string name, Microsoft.OData.Edm.Values.IEdmValue value)

	string Name  { public virtual get; }
	Microsoft.OData.Edm.Values.IEdmValue Value  { public virtual get; public set; }
}

public class Microsoft.OData.Edm.Library.Values.EdmStringConstant : Microsoft.OData.Edm.Library.Values.EdmValue, IEdmElement, IEdmExpression, IEdmStringConstantExpression, IEdmDelayedValue, IEdmPrimitiveValue, IEdmStringValue, IEdmValue {
	public EdmStringConstant (string value)
	public EdmStringConstant (Microsoft.OData.Edm.IEdmStringTypeReference type, string value)

	Microsoft.OData.Edm.Expressions.EdmExpressionKind ExpressionKind  { public virtual get; }
	string Value  { public virtual get; }
	Microsoft.OData.Edm.Values.EdmValueKind ValueKind  { public virtual get; }
}

public class Microsoft.OData.Edm.Library.Values.EdmStructuredValue : Microsoft.OData.Edm.Library.Values.EdmValue, IEdmElement, IEdmDelayedValue, IEdmStructuredValue, IEdmValue {
	public EdmStructuredValue (Microsoft.OData.Edm.IEdmStructuredTypeReference type, System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Values.IEdmPropertyValue]] propertyValues)

	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.Values.IEdmPropertyValue]] PropertyValues  { public virtual get; }
	Microsoft.OData.Edm.Values.EdmValueKind ValueKind  { public virtual get; }

	public virtual Microsoft.OData.Edm.Values.IEdmPropertyValue FindPropertyValue (string propertyName)
}

public class Microsoft.OData.Edm.Library.Values.EdmTimeOfDayConstant : Microsoft.OData.Edm.Library.Values.EdmValue, IEdmElement, IEdmExpression, IEdmTimeOfDayConstantExpression, IEdmDelayedValue, IEdmPrimitiveValue, IEdmTimeOfDayValue, IEdmValue {
	public EdmTimeOfDayConstant (Microsoft.OData.Edm.Library.TimeOfDay value)
	public EdmTimeOfDayConstant (Microsoft.OData.Edm.IEdmTemporalTypeReference type, Microsoft.OData.Edm.Library.TimeOfDay value)

	Microsoft.OData.Edm.Expressions.EdmExpressionKind ExpressionKind  { public virtual get; }
	Microsoft.OData.Edm.Library.TimeOfDay Value  { public virtual get; }
	Microsoft.OData.Edm.Values.EdmValueKind ValueKind  { public virtual get; }
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
	public static string OptimisticConcurrencyControl = "Org.OData.Core.V1.OptimisticConcurrencyControl"
	public static string RequiresType = "Org.OData.Core.V1.RequiresType"
	public static string ResourcePath = "Org.OData.Core.V1.ResourcePath"
}

public sealed class Microsoft.OData.Edm.Vocabularies.V1.CoreVocabularyModel {
	public static readonly Microsoft.OData.Edm.IEdmValueTerm AcceptableMediaTypesTerm = Microsoft.OData.Edm.Csdl.CsdlSemantics.CsdlSemanticsValueTerm
	public static readonly Microsoft.OData.Edm.IEdmValueTerm ComputedTerm = Microsoft.OData.Edm.Csdl.CsdlSemantics.CsdlSemanticsValueTerm
	public static readonly Microsoft.OData.Edm.IEdmValueTerm ConcurrencyControlTerm = Microsoft.OData.Edm.Csdl.CsdlSemantics.CsdlSemanticsValueTerm
	public static readonly Microsoft.OData.Edm.IEdmValueTerm ConcurrencyTerm = Microsoft.OData.Edm.Csdl.CsdlSemantics.CsdlSemanticsValueTerm
	public static readonly Microsoft.OData.Edm.IEdmValueTerm ConventionalIDsTerm = Microsoft.OData.Edm.Csdl.CsdlSemantics.CsdlSemanticsValueTerm
	public static readonly Microsoft.OData.Edm.IEdmValueTerm DereferenceableIDsTerm = Microsoft.OData.Edm.Csdl.CsdlSemantics.CsdlSemanticsValueTerm
	public static readonly Microsoft.OData.Edm.IEdmValueTerm DescriptionTerm = Microsoft.OData.Edm.Csdl.CsdlSemantics.CsdlSemanticsValueTerm
	public static readonly Microsoft.OData.Edm.IEdmValueTerm ImmutableTerm = Microsoft.OData.Edm.Csdl.CsdlSemantics.CsdlSemanticsValueTerm
	public static readonly Microsoft.OData.Edm.IEdmModel Instance = Microsoft.OData.Edm.Csdl.CsdlSemantics.CsdlSemanticsModel
	public static readonly Microsoft.OData.Edm.IEdmValueTerm IsLanguageDependentTerm = Microsoft.OData.Edm.Csdl.CsdlSemantics.CsdlSemanticsValueTerm
	public static readonly Microsoft.OData.Edm.IEdmValueTerm IsMediaTypeTerm = Microsoft.OData.Edm.Csdl.CsdlSemantics.CsdlSemanticsValueTerm
	public static readonly Microsoft.OData.Edm.IEdmValueTerm IsURLTerm = Microsoft.OData.Edm.Csdl.CsdlSemantics.CsdlSemanticsValueTerm
	public static readonly Microsoft.OData.Edm.IEdmValueTerm LongDescriptionTerm = Microsoft.OData.Edm.Csdl.CsdlSemantics.CsdlSemanticsValueTerm
	public static readonly Microsoft.OData.Edm.IEdmValueTerm MediaTypeTerm = Microsoft.OData.Edm.Csdl.CsdlSemantics.CsdlSemanticsValueTerm
	public static readonly Microsoft.OData.Edm.IEdmValueTerm RequiresTypeTerm = Microsoft.OData.Edm.Csdl.CsdlSemantics.CsdlSemanticsValueTerm
	public static readonly Microsoft.OData.Edm.IEdmValueTerm ResourcePathTerm = Microsoft.OData.Edm.Csdl.CsdlSemantics.CsdlSemanticsValueTerm
}

public sealed class Microsoft.OData.Edm.Vocabularies.Community.V1.AlternateKeysVocabularyConstants {
	public static string AlternateKeys = "OData.Community.Keys.V1.AlternateKeys"
}

public sealed class Microsoft.OData.Edm.Vocabularies.Community.V1.AlternateKeysVocabularyModel {
	public static readonly Microsoft.OData.Edm.IEdmValueTerm AlternateKeysTerm = Microsoft.OData.Edm.Csdl.CsdlSemantics.CsdlSemanticsValueTerm
	public static readonly Microsoft.OData.Edm.IEdmModel Instance = Microsoft.OData.Edm.Csdl.CsdlSemantics.CsdlSemanticsModel
}

public enum Microsoft.OData.Core.DeltaDeletedEntryReason : int {
	Changed = 1
	Deleted = 0
}

public enum Microsoft.OData.Core.ODataBatchReaderState : int {
	ChangesetEnd = 3
	ChangesetStart = 2
	Completed = 4
	Exception = 5
	Initial = 0
	Operation = 1
}

public enum Microsoft.OData.Core.ODataCollectionReaderState : int {
	CollectionEnd = 3
	CollectionStart = 1
	Completed = 5
	Exception = 4
	Start = 0
	Value = 2
}

public enum Microsoft.OData.Core.ODataDeltaReaderState : int {
	Completed = 9
	DeltaDeletedEntry = 5
	DeltaDeletedLink = 7
	DeltaEntryEnd = 4
	DeltaEntryStart = 3
	DeltaFeedStart = 1
	DeltaLink = 6
	Exception = 8
	ExpandedNavigationProperty = 10
	FeedEnd = 2
	Start = 0
}

public enum Microsoft.OData.Core.ODataParameterReaderState : int {
	Collection = 2
	Completed = 4
	Entry = 5
	Exception = 3
	Feed = 6
	Start = 0
	Value = 1
}

public enum Microsoft.OData.Core.ODataPayloadKind : int {
	Asynchronous = 15
	Batch = 11
	BinaryValue = 6
	Collection = 7
	Delta = 14
	EntityReferenceLink = 3
	EntityReferenceLinks = 4
	Entry = 1
	Error = 10
	Feed = 0
	IndividualProperty = 13
	MetadataDocument = 9
	Parameter = 12
	Property = 2
	ServiceDocument = 8
	Unsupported = 2147483647
	Value = 5
}

public enum Microsoft.OData.Core.ODataPropertyKind : int {
	ETag = 2
	Key = 1
	Open = 3
	Unspecified = 0
}

public enum Microsoft.OData.Core.ODataReaderState : int {
	Completed = 9
	EntityReferenceLink = 7
	EntryEnd = 4
	EntryStart = 3
	Exception = 8
	FeedEnd = 2
	FeedStart = 1
	NavigationLinkEnd = 6
	NavigationLinkStart = 5
	Start = 0
}

[
FlagsAttribute(),
]
public enum Microsoft.OData.Core.ODataUndeclaredPropertyBehaviorKinds : int {
	IgnoreUndeclaredValueProperty = 1
	None = 0
	ReportUndeclaredLinkProperty = 2
}

public enum Microsoft.OData.Core.ODataVersion : int {
	V4 = 0
}

public interface Microsoft.OData.Core.IODataRequestMessage {
	System.Collections.Generic.IEnumerable`1[[System.Collections.Generic.KeyValuePair`2[[System.String],[System.String]]]] Headers  { public abstract get; }
	string Method  { public abstract get; public abstract set; }
	System.Uri Url  { public abstract get; public abstract set; }

	string GetHeader (string headerName)
	System.IO.Stream GetStream ()
	void SetHeader (string headerName, string headerValue)
}

public interface Microsoft.OData.Core.IODataRequestMessageAsync : IODataRequestMessage {
	System.Threading.Tasks.Task`1[[System.IO.Stream]] GetStreamAsync ()
}

public interface Microsoft.OData.Core.IODataResponseMessage {
	System.Collections.Generic.IEnumerable`1[[System.Collections.Generic.KeyValuePair`2[[System.String],[System.String]]]] Headers  { public abstract get; }
	int StatusCode  { public abstract get; public abstract set; }

	string GetHeader (string headerName)
	System.IO.Stream GetStream ()
	void SetHeader (string headerName, string headerValue)
}

public interface Microsoft.OData.Core.IODataResponseMessageAsync : IODataResponseMessage {
	System.Threading.Tasks.Task`1[[System.IO.Stream]] GetStreamAsync ()
}

public interface Microsoft.OData.Core.IODataUrlResolver {
	System.Uri ResolveUrl (System.Uri baseUri, System.Uri payloadUri)
}

public abstract class Microsoft.OData.Core.ODataAnnotatable {
	protected ODataAnnotatable ()

	public T GetAnnotation ()
	public void SetAnnotation (T annotation)
}

public abstract class Microsoft.OData.Core.ODataCollectionReader {
	protected ODataCollectionReader ()

	object Item  { public abstract get; }
	Microsoft.OData.Core.ODataCollectionReaderState State  { public abstract get; }

	public abstract bool Read ()
	public abstract System.Threading.Tasks.Task`1[[System.Boolean]] ReadAsync ()
}

public abstract class Microsoft.OData.Core.ODataCollectionWriter {
	protected ODataCollectionWriter ()

	public abstract void Flush ()
	public abstract System.Threading.Tasks.Task FlushAsync ()
	public abstract void WriteEnd ()
	public abstract System.Threading.Tasks.Task WriteEndAsync ()
	public abstract void WriteItem (object item)
	public abstract System.Threading.Tasks.Task WriteItemAsync (object item)
	public abstract void WriteStart (Microsoft.OData.Core.ODataCollectionStart collectionStart)
	public abstract System.Threading.Tasks.Task WriteStartAsync (Microsoft.OData.Core.ODataCollectionStart collectionStart)
}

public abstract class Microsoft.OData.Core.ODataDeltaLinkBase : Microsoft.OData.Core.ODataItem {
	protected ODataDeltaLinkBase (System.Uri source, System.Uri target, string relationship)

	string Relationship  { public get; public set; }
	System.Uri Source  { public get; public set; }
	System.Uri Target  { public get; public set; }
}

public abstract class Microsoft.OData.Core.ODataDeltaReader {
	protected ODataDeltaReader ()

	Microsoft.OData.Core.ODataItem Item  { public abstract get; }
	Microsoft.OData.Core.ODataDeltaReaderState State  { public abstract get; }
	Microsoft.OData.Core.ODataReaderState SubState  { public abstract get; }

	public abstract bool Read ()
	public abstract System.Threading.Tasks.Task`1[[System.Boolean]] ReadAsync ()
}

public abstract class Microsoft.OData.Core.ODataDeltaWriter {
	protected ODataDeltaWriter ()

	public abstract void Flush ()
	public abstract System.Threading.Tasks.Task FlushAsync ()
	public abstract void WriteDeltaDeletedEntry (Microsoft.OData.Core.ODataDeltaDeletedEntry deltaDeletedEntry)
	public abstract System.Threading.Tasks.Task WriteDeltaDeletedEntryAsync (Microsoft.OData.Core.ODataDeltaDeletedEntry deltaDeletedEntry)
	public abstract void WriteDeltaDeletedLink (Microsoft.OData.Core.ODataDeltaDeletedLink deltaDeletedLink)
	public abstract System.Threading.Tasks.Task WriteDeltaDeletedLinkAsync (Microsoft.OData.Core.ODataDeltaDeletedLink deltaDeletedLink)
	public abstract void WriteDeltaLink (Microsoft.OData.Core.ODataDeltaLink deltaLink)
	public abstract System.Threading.Tasks.Task WriteDeltaLinkAsync (Microsoft.OData.Core.ODataDeltaLink deltaLink)
	public abstract void WriteEnd ()
	public abstract System.Threading.Tasks.Task WriteEndAsync ()
	public abstract void WriteStart (Microsoft.OData.Core.ODataDeltaFeed deltaFeed)
	public abstract void WriteStart (Microsoft.OData.Core.ODataEntry deltaEntry)
	public abstract void WriteStart (Microsoft.OData.Core.ODataFeed expandedFeed)
	public abstract void WriteStart (Microsoft.OData.Core.ODataNavigationLink navigationLink)
	public abstract System.Threading.Tasks.Task WriteStartAsync (Microsoft.OData.Core.ODataDeltaFeed deltaFeed)
	public abstract System.Threading.Tasks.Task WriteStartAsync (Microsoft.OData.Core.ODataEntry deltaEntry)
	public abstract System.Threading.Tasks.Task WriteStartAsync (Microsoft.OData.Core.ODataFeed expandedFeed)
	public abstract System.Threading.Tasks.Task WriteStartAsync (Microsoft.OData.Core.ODataNavigationLink navigationLink)
}

public abstract class Microsoft.OData.Core.ODataFeedBase : Microsoft.OData.Core.ODataItem {
	protected ODataFeedBase ()

	System.Nullable`1[[System.Int64]] Count  { public get; public set; }
	System.Uri DeltaLink  { public get; public set; }
	System.Uri Id  { public get; public set; }
	System.Collections.Generic.ICollection`1[[Microsoft.OData.Core.ODataInstanceAnnotation]] InstanceAnnotations  { public get; public set; }
	System.Uri NextPageLink  { public get; public set; }
}

public abstract class Microsoft.OData.Core.ODataFormat {
	protected ODataFormat ()

	[
	ObsoleteAttribute(),
	]
	Microsoft.OData.Core.ODataFormat Atom  { public static get; }

	Microsoft.OData.Core.ODataFormat Batch  { public static get; }
	Microsoft.OData.Core.ODataFormat Json  { public static get; }
	Microsoft.OData.Core.ODataFormat Metadata  { public static get; }
	Microsoft.OData.Core.ODataFormat RawValue  { public static get; }

	public abstract Microsoft.OData.Core.ODataInputContext CreateInputContext (Microsoft.OData.Core.ODataMessageInfo messageInfo, Microsoft.OData.Core.ODataMessageReaderSettings messageReaderSettings)
	public abstract System.Threading.Tasks.Task`1[[Microsoft.OData.Core.ODataInputContext]] CreateInputContextAsync (Microsoft.OData.Core.ODataMessageInfo messageInfo, Microsoft.OData.Core.ODataMessageReaderSettings messageReaderSettings)
	public abstract Microsoft.OData.Core.ODataOutputContext CreateOutputContext (Microsoft.OData.Core.ODataMessageInfo messageInfo, Microsoft.OData.Core.ODataMessageWriterSettings messageWriterSettings)
	public abstract System.Threading.Tasks.Task`1[[Microsoft.OData.Core.ODataOutputContext]] CreateOutputContextAsync (Microsoft.OData.Core.ODataMessageInfo messageInfo, Microsoft.OData.Core.ODataMessageWriterSettings messageWriterSettings)
	public abstract System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Core.ODataPayloadKind]] DetectPayloadKind (Microsoft.OData.Core.ODataMessageInfo messageInfo, Microsoft.OData.Core.ODataMessageReaderSettings settings)
	public abstract System.Threading.Tasks.Task`1[[System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Core.ODataPayloadKind]]]] DetectPayloadKindAsync (Microsoft.OData.Core.ODataMessageInfo messageInfo, Microsoft.OData.Core.ODataMessageReaderSettings settings)
}

public abstract class Microsoft.OData.Core.ODataInputContext : IDisposable {
	protected ODataInputContext (Microsoft.OData.Core.ODataFormat format, Microsoft.OData.Core.ODataMessageReaderSettings messageReaderSettings, bool readingResponse, bool synchronous, Microsoft.OData.Edm.IEdmModel model, Microsoft.OData.Core.IODataUrlResolver urlResolver)

	Microsoft.OData.Core.ODataMessageReaderSettings MessageReaderSettings  { public get; }
	Microsoft.OData.Edm.IEdmModel Model  { public get; }
	bool ReadingResponse  { public get; }
	bool Synchronous  { public get; }
	Microsoft.OData.Core.IODataUrlResolver UrlResolver  { public get; }
	bool UseClientApiBehavior  { protected get; }
	bool UseDefaultApiBehavior  { protected get; }
	bool UseDefaultFormatBehavior  { protected get; }
	bool UseServerApiBehavior  { protected get; }
	bool UseServerFormatBehavior  { protected get; }

	internal virtual Microsoft.OData.Core.ODataAsynchronousReader CreateAsynchronousReader ()
	internal virtual System.Threading.Tasks.Task`1[[Microsoft.OData.Core.ODataAsynchronousReader]] CreateAsynchronousReaderAsync ()
	internal virtual Microsoft.OData.Core.ODataBatchReader CreateBatchReader (string batchBoundary)
	internal virtual System.Threading.Tasks.Task`1[[Microsoft.OData.Core.ODataBatchReader]] CreateBatchReaderAsync (string batchBoundary)
	public virtual Microsoft.OData.Core.ODataCollectionReader CreateCollectionReader (Microsoft.OData.Edm.IEdmTypeReference expectedItemTypeReference)
	public virtual System.Threading.Tasks.Task`1[[Microsoft.OData.Core.ODataCollectionReader]] CreateCollectionReaderAsync (Microsoft.OData.Edm.IEdmTypeReference expectedItemTypeReference)
	internal virtual Microsoft.OData.Core.ODataDeltaReader CreateDeltaReader (Microsoft.OData.Edm.IEdmEntitySetBase entitySet, Microsoft.OData.Edm.IEdmEntityType expectedBaseEntityType)
	internal virtual System.Threading.Tasks.Task`1[[Microsoft.OData.Core.ODataDeltaReader]] CreateDeltaReaderAsync (Microsoft.OData.Edm.IEdmEntitySetBase entitySet, Microsoft.OData.Edm.IEdmEntityType expectedBaseEntityType)
	public virtual Microsoft.OData.Core.ODataReader CreateEntryReader (Microsoft.OData.Edm.IEdmNavigationSource navigationSource, Microsoft.OData.Edm.IEdmEntityType expectedEntityType)
	public virtual System.Threading.Tasks.Task`1[[Microsoft.OData.Core.ODataReader]] CreateEntryReaderAsync (Microsoft.OData.Edm.IEdmNavigationSource navigationSource, Microsoft.OData.Edm.IEdmEntityType expectedEntityType)
	public virtual Microsoft.OData.Core.ODataReader CreateFeedReader (Microsoft.OData.Edm.IEdmEntitySetBase entitySet, Microsoft.OData.Edm.IEdmEntityType expectedBaseEntityType)
	public virtual System.Threading.Tasks.Task`1[[Microsoft.OData.Core.ODataReader]] CreateFeedReaderAsync (Microsoft.OData.Edm.IEdmEntitySetBase entitySet, Microsoft.OData.Edm.IEdmEntityType expectedBaseEntityType)
	public virtual Microsoft.OData.Core.ODataParameterReader CreateParameterReader (Microsoft.OData.Edm.IEdmOperation operation)
	public virtual System.Threading.Tasks.Task`1[[Microsoft.OData.Core.ODataParameterReader]] CreateParameterReaderAsync (Microsoft.OData.Edm.IEdmOperation operation)
	public virtual void Dispose ()
	protected virtual void Dispose (bool disposing)
	internal virtual Microsoft.OData.Core.ODataEntityReferenceLink ReadEntityReferenceLink ()
	internal virtual System.Threading.Tasks.Task`1[[Microsoft.OData.Core.ODataEntityReferenceLink]] ReadEntityReferenceLinkAsync ()
	internal virtual Microsoft.OData.Core.ODataEntityReferenceLinks ReadEntityReferenceLinks ()
	internal virtual System.Threading.Tasks.Task`1[[Microsoft.OData.Core.ODataEntityReferenceLinks]] ReadEntityReferenceLinksAsync ()
	public virtual Microsoft.OData.Core.ODataError ReadError ()
	public virtual System.Threading.Tasks.Task`1[[Microsoft.OData.Core.ODataError]] ReadErrorAsync ()
	internal virtual Microsoft.OData.Edm.IEdmModel ReadMetadataDocument (System.Func`2[[System.Uri],[System.Xml.XmlReader]] getReferencedModelReaderFunc)
	public virtual Microsoft.OData.Core.ODataProperty ReadProperty (Microsoft.OData.Edm.IEdmStructuralProperty property, Microsoft.OData.Edm.IEdmTypeReference expectedPropertyTypeReference)
	public virtual System.Threading.Tasks.Task`1[[Microsoft.OData.Core.ODataProperty]] ReadPropertyAsync (Microsoft.OData.Edm.IEdmStructuralProperty property, Microsoft.OData.Edm.IEdmTypeReference expectedPropertyTypeReference)
	internal virtual Microsoft.OData.Core.ODataServiceDocument ReadServiceDocument ()
	internal virtual System.Threading.Tasks.Task`1[[Microsoft.OData.Core.ODataServiceDocument]] ReadServiceDocumentAsync ()
	internal virtual object ReadValue (Microsoft.OData.Edm.IEdmPrimitiveTypeReference expectedPrimitiveTypeReference)
	internal virtual System.Threading.Tasks.Task`1[[System.Object]] ReadValueAsync (Microsoft.OData.Edm.IEdmPrimitiveTypeReference expectedPrimitiveTypeReference)
}

public abstract class Microsoft.OData.Core.ODataItem : Microsoft.OData.Core.ODataAnnotatable {
	protected ODataItem ()
}

public abstract class Microsoft.OData.Core.ODataMessageReaderSettingsBase {
	protected ODataMessageReaderSettingsBase ()
	protected ODataMessageReaderSettingsBase (Microsoft.OData.Core.ODataMessageReaderSettingsBase other)

	bool CheckCharacters  { public virtual get; public virtual set; }
	bool EnableAtomMetadataReading  { public virtual get; public virtual set; }
	Microsoft.OData.Core.ODataMessageQuotas MessageQuotas  { public virtual get; public virtual set; }
	System.Func`2[[System.String],[System.Boolean]] ShouldIncludeAnnotation  { public virtual get; public virtual set; }
}

public abstract class Microsoft.OData.Core.ODataMessageWriterSettingsBase {
	protected ODataMessageWriterSettingsBase ()
	protected ODataMessageWriterSettingsBase (Microsoft.OData.Core.ODataMessageWriterSettingsBase other)

	bool CheckCharacters  { public virtual get; public virtual set; }
	bool Indent  { public virtual get; public virtual set; }
	Microsoft.OData.Core.ODataMessageQuotas MessageQuotas  { public virtual get; public virtual set; }
}

public abstract class Microsoft.OData.Core.ODataOperation : Microsoft.OData.Core.ODataAnnotatable {
	protected ODataOperation ()

	System.Uri Metadata  { public get; public set; }
	System.Uri Target  { public get; public set; }
	string Title  { public get; public set; }
}

public abstract class Microsoft.OData.Core.ODataOutputContext : IDisposable {
	protected ODataOutputContext (Microsoft.OData.Core.ODataFormat format, Microsoft.OData.Core.ODataMessageWriterSettings messageWriterSettings, bool writingResponse, bool synchronous, Microsoft.OData.Edm.IEdmModel model, Microsoft.OData.Core.IODataUrlResolver urlResolver)

	Microsoft.OData.Core.ODataMessageWriterSettings MessageWriterSettings  { public get; }
	Microsoft.OData.Edm.IEdmModel Model  { public get; }
	bool Synchronous  { public get; }
	Microsoft.OData.Core.IODataUrlResolver UrlResolver  { public get; }
	bool UseClientFormatBehavior  { protected get; }
	bool UseDefaultFormatBehavior  { protected get; }
	bool UseServerApiBehavior  { protected get; }
	bool UseServerFormatBehavior  { protected get; }
	bool WritingResponse  { public get; }

	internal virtual Microsoft.OData.Core.ODataAsynchronousWriter CreateODataAsynchronousWriter ()
	internal virtual System.Threading.Tasks.Task`1[[Microsoft.OData.Core.ODataAsynchronousWriter]] CreateODataAsynchronousWriterAsync ()
	internal virtual Microsoft.OData.Core.ODataBatchWriter CreateODataBatchWriter (string batchBoundary)
	internal virtual System.Threading.Tasks.Task`1[[Microsoft.OData.Core.ODataBatchWriter]] CreateODataBatchWriterAsync (string batchBoundary)
	public virtual Microsoft.OData.Core.ODataCollectionWriter CreateODataCollectionWriter (Microsoft.OData.Edm.IEdmTypeReference itemTypeReference)
	public virtual System.Threading.Tasks.Task`1[[Microsoft.OData.Core.ODataCollectionWriter]] CreateODataCollectionWriterAsync (Microsoft.OData.Edm.IEdmTypeReference itemTypeReference)
	internal virtual Microsoft.OData.Core.ODataDeltaWriter CreateODataDeltaWriter (Microsoft.OData.Edm.IEdmEntitySetBase entitySet, Microsoft.OData.Edm.IEdmEntityType entityType)
	internal virtual System.Threading.Tasks.Task`1[[Microsoft.OData.Core.ODataDeltaWriter]] CreateODataDeltaWriterAsync (Microsoft.OData.Edm.IEdmEntitySetBase entitySet, Microsoft.OData.Edm.IEdmEntityType entityType)
	public virtual Microsoft.OData.Core.ODataWriter CreateODataEntryWriter (Microsoft.OData.Edm.IEdmNavigationSource navigationSource, Microsoft.OData.Edm.IEdmEntityType entityType)
	public virtual System.Threading.Tasks.Task`1[[Microsoft.OData.Core.ODataWriter]] CreateODataEntryWriterAsync (Microsoft.OData.Edm.IEdmNavigationSource navigationSource, Microsoft.OData.Edm.IEdmEntityType entityType)
	public virtual Microsoft.OData.Core.ODataWriter CreateODataFeedWriter (Microsoft.OData.Edm.IEdmEntitySetBase entitySet, Microsoft.OData.Edm.IEdmEntityType entityType)
	public virtual System.Threading.Tasks.Task`1[[Microsoft.OData.Core.ODataWriter]] CreateODataFeedWriterAsync (Microsoft.OData.Edm.IEdmEntitySetBase entitySet, Microsoft.OData.Edm.IEdmEntityType entityType)
	public virtual Microsoft.OData.Core.ODataParameterWriter CreateODataParameterWriter (Microsoft.OData.Edm.IEdmOperation operation)
	public virtual System.Threading.Tasks.Task`1[[Microsoft.OData.Core.ODataParameterWriter]] CreateODataParameterWriterAsync (Microsoft.OData.Edm.IEdmOperation operation)
	public virtual void Dispose ()
	protected virtual void Dispose (bool disposing)
	internal virtual void WriteEntityReferenceLink (Microsoft.OData.Core.ODataEntityReferenceLink link)
	internal virtual System.Threading.Tasks.Task WriteEntityReferenceLinkAsync (Microsoft.OData.Core.ODataEntityReferenceLink link)
	internal virtual void WriteEntityReferenceLinks (Microsoft.OData.Core.ODataEntityReferenceLinks links)
	internal virtual System.Threading.Tasks.Task WriteEntityReferenceLinksAsync (Microsoft.OData.Core.ODataEntityReferenceLinks links)
	public virtual void WriteError (Microsoft.OData.Core.ODataError error, bool includeDebugInformation)
	public virtual System.Threading.Tasks.Task WriteErrorAsync (Microsoft.OData.Core.ODataError error, bool includeDebugInformation)
	internal virtual void WriteInStreamError (Microsoft.OData.Core.ODataError error, bool includeDebugInformation)
	internal virtual System.Threading.Tasks.Task WriteInStreamErrorAsync (Microsoft.OData.Core.ODataError error, bool includeDebugInformation)
	internal virtual void WriteMetadataDocument ()
	public virtual void WriteProperty (Microsoft.OData.Core.ODataProperty property)
	public virtual System.Threading.Tasks.Task WritePropertyAsync (Microsoft.OData.Core.ODataProperty property)
	internal virtual void WriteServiceDocument (Microsoft.OData.Core.ODataServiceDocument serviceDocument)
	internal virtual System.Threading.Tasks.Task WriteServiceDocumentAsync (Microsoft.OData.Core.ODataServiceDocument serviceDocument)
	internal virtual void WriteValue (object value)
	internal virtual System.Threading.Tasks.Task WriteValueAsync (object value)
}

public abstract class Microsoft.OData.Core.ODataParameterReader {
	protected ODataParameterReader ()

	string Name  { public abstract get; }
	Microsoft.OData.Core.ODataParameterReaderState State  { public abstract get; }
	object Value  { public abstract get; }

	public abstract Microsoft.OData.Core.ODataCollectionReader CreateCollectionReader ()
	public abstract Microsoft.OData.Core.ODataReader CreateEntryReader ()
	public abstract Microsoft.OData.Core.ODataReader CreateFeedReader ()
	public abstract bool Read ()
	public abstract System.Threading.Tasks.Task`1[[System.Boolean]] ReadAsync ()
}

public abstract class Microsoft.OData.Core.ODataParameterWriter {
	protected ODataParameterWriter ()

	public abstract Microsoft.OData.Core.ODataCollectionWriter CreateCollectionWriter (string parameterName)
	public abstract System.Threading.Tasks.Task`1[[Microsoft.OData.Core.ODataCollectionWriter]] CreateCollectionWriterAsync (string parameterName)
	public abstract Microsoft.OData.Core.ODataWriter CreateEntryWriter (string parameterName)
	public abstract System.Threading.Tasks.Task`1[[Microsoft.OData.Core.ODataWriter]] CreateEntryWriterAsync (string parameterName)
	public abstract Microsoft.OData.Core.ODataWriter CreateFeedWriter (string parameterName)
	public abstract System.Threading.Tasks.Task`1[[Microsoft.OData.Core.ODataWriter]] CreateFeedWriterAsync (string parameterName)
	public abstract void Flush ()
	public abstract System.Threading.Tasks.Task FlushAsync ()
	public abstract void WriteEnd ()
	public abstract System.Threading.Tasks.Task WriteEndAsync ()
	public abstract void WriteStart ()
	public abstract System.Threading.Tasks.Task WriteStartAsync ()
	public abstract void WriteValue (string parameterName, object parameterValue)
	public abstract System.Threading.Tasks.Task WriteValueAsync (string parameterName, object parameterValue)
}

public abstract class Microsoft.OData.Core.ODataReader {
	protected ODataReader ()

	Microsoft.OData.Core.ODataItem Item  { public abstract get; }
	Microsoft.OData.Core.ODataReaderState State  { public abstract get; }

	public abstract bool Read ()
	public abstract System.Threading.Tasks.Task`1[[System.Boolean]] ReadAsync ()
}

public abstract class Microsoft.OData.Core.ODataServiceDocumentElement : Microsoft.OData.Core.ODataAnnotatable {
	protected ODataServiceDocumentElement ()

	string Name  { public get; public set; }
	string Title  { public get; public set; }
	System.Uri Url  { public get; public set; }
}

public abstract class Microsoft.OData.Core.ODataValue : Microsoft.OData.Core.ODataAnnotatable {
	protected ODataValue ()
}

public abstract class Microsoft.OData.Core.ODataWriter {
	protected ODataWriter ()

	public abstract void Flush ()
	public abstract System.Threading.Tasks.Task FlushAsync ()
	public abstract void WriteEnd ()
	public abstract System.Threading.Tasks.Task WriteEndAsync ()
	public abstract void WriteEntityReferenceLink (Microsoft.OData.Core.ODataEntityReferenceLink entityReferenceLink)
	public abstract System.Threading.Tasks.Task WriteEntityReferenceLinkAsync (Microsoft.OData.Core.ODataEntityReferenceLink entityReferenceLink)
	public abstract void WriteStart (Microsoft.OData.Core.ODataEntry entry)
	public abstract void WriteStart (Microsoft.OData.Core.ODataFeed feed)
	public abstract void WriteStart (Microsoft.OData.Core.ODataNavigationLink navigationLink)
	public abstract System.Threading.Tasks.Task WriteStartAsync (Microsoft.OData.Core.ODataEntry entry)
	public abstract System.Threading.Tasks.Task WriteStartAsync (Microsoft.OData.Core.ODataFeed feed)
	public abstract System.Threading.Tasks.Task WriteStartAsync (Microsoft.OData.Core.ODataNavigationLink navigationLink)
}

public sealed class Microsoft.OData.Core.ODataConstants {
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
public sealed class Microsoft.OData.Core.ODataMessageExtensions {
	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Core.ODataVersion GetODataVersion (Microsoft.OData.Core.IODataRequestMessage message, Microsoft.OData.Core.ODataVersion defaultVersion)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Core.ODataVersion GetODataVersion (Microsoft.OData.Core.IODataResponseMessage message, Microsoft.OData.Core.ODataVersion defaultVersion)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Core.ODataPreferenceHeader PreferenceAppliedHeader (Microsoft.OData.Core.IODataResponseMessage responseMessage)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Core.ODataPreferenceHeader PreferHeader (Microsoft.OData.Core.IODataRequestMessage requestMessage)
}

[
ExtensionAttribute(),
]
public sealed class Microsoft.OData.Core.ODataObjectModelExtensions {
	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Core.ODataPayloadValueConverter GetPayloadValueConverter (Microsoft.OData.Edm.IEdmModel model)

	[
	ExtensionAttribute(),
	]
	public static void SetPayloadValueConverter (Microsoft.OData.Edm.IEdmModel model, Microsoft.OData.Core.ODataPayloadValueConverter converter)

	[
	ExtensionAttribute(),
	]
	public static void SetSerializationInfo (Microsoft.OData.Core.ODataCollectionStart collectionStart, Microsoft.OData.Core.ODataCollectionStartSerializationInfo serializationInfo)

	[
	ExtensionAttribute(),
	]
	public static void SetSerializationInfo (Microsoft.OData.Core.ODataDeltaDeletedEntry deltaDeletedEntry, Microsoft.OData.Core.ODataDeltaSerializationInfo serializationInfo)

	[
	ExtensionAttribute(),
	]
	public static void SetSerializationInfo (Microsoft.OData.Core.ODataDeltaFeed deltaFeed, Microsoft.OData.Core.ODataDeltaFeedSerializationInfo serializationInfo)

	[
	ExtensionAttribute(),
	]
	public static void SetSerializationInfo (Microsoft.OData.Core.ODataDeltaLinkBase deltalink, Microsoft.OData.Core.ODataDeltaSerializationInfo serializationInfo)

	[
	ExtensionAttribute(),
	]
	public static void SetSerializationInfo (Microsoft.OData.Core.ODataEntry entry, Microsoft.OData.Core.ODataFeedAndEntrySerializationInfo serializationInfo)

	[
	ExtensionAttribute(),
	]
	public static void SetSerializationInfo (Microsoft.OData.Core.ODataFeed feed, Microsoft.OData.Core.ODataFeedAndEntrySerializationInfo serializationInfo)

	[
	ExtensionAttribute(),
	]
	public static void SetSerializationInfo (Microsoft.OData.Core.ODataProperty property, Microsoft.OData.Core.ODataPropertySerializationInfo serializationInfo)
}

[
ExtensionAttribute(),
]
public sealed class Microsoft.OData.Core.ODataUtils {
	public static string AppendDefaultHeaderValue (string headerName, string headerValue)
	public static System.Func`2[[System.String],[System.Boolean]] CreateAnnotationFilter (string annotationFilter)
	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Core.ODataServiceDocument GenerateServiceDocument (Microsoft.OData.Edm.IEdmModel model)

	public static Microsoft.OData.Core.ODataFormat GetReadFormat (Microsoft.OData.Core.ODataMessageReader messageReader)
	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Core.Metadata.ODataNullValueBehaviorKind NullValueReadBehaviorKind (Microsoft.OData.Edm.IEdmModel model, Microsoft.OData.Edm.IEdmProperty property)

	public static string ODataVersionToString (Microsoft.OData.Core.ODataVersion version)
	public static Microsoft.OData.Core.ODataFormat SetHeadersForPayload (Microsoft.OData.Core.ODataMessageWriter messageWriter, Microsoft.OData.Core.ODataPayloadKind payloadKind)
	[
	ExtensionAttribute(),
	]
	public static void SetNullValueReaderBehavior (Microsoft.OData.Edm.IEdmModel model, Microsoft.OData.Edm.IEdmProperty property, Microsoft.OData.Core.Metadata.ODataNullValueBehaviorKind nullValueReadBehaviorKind)

	public static Microsoft.OData.Core.ODataVersion StringToODataVersion (string version)
}

[
DebuggerDisplayAttribute(),
]
public class Microsoft.OData.Core.ODataContentTypeException : Microsoft.OData.Core.ODataException, _Exception, ISerializable {
	public ODataContentTypeException ()
	public ODataContentTypeException (string message)
	public ODataContentTypeException (string message, System.Exception innerException)
}

[
DebuggerDisplayAttribute(),
]
public class Microsoft.OData.Core.ODataException : System.InvalidOperationException, _Exception, ISerializable {
	public ODataException ()
	public ODataException (string message)
	public ODataException (string message, System.Exception innerException)
}

public class Microsoft.OData.Core.ODataMediaTypeResolver {
	public ODataMediaTypeResolver ()

	public virtual System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Core.ODataMediaTypeFormat]] GetMediaTypeFormats (Microsoft.OData.Core.ODataPayloadKind payloadKind)
}

public class Microsoft.OData.Core.ODataPayloadValueConverter {
	public ODataPayloadValueConverter ()

	public virtual object ConvertFromPayloadValue (object value, Microsoft.OData.Edm.IEdmTypeReference edmTypeReference)
	public virtual object ConvertToPayloadValue (object value, Microsoft.OData.Edm.IEdmTypeReference edmTypeReference)
}

[
DebuggerDisplayAttribute(),
]
public sealed class Microsoft.OData.Core.ODataAction : Microsoft.OData.Core.ODataOperation {
	public ODataAction ()
}

public sealed class Microsoft.OData.Core.ODataAsynchronousReader {
	public Microsoft.OData.Core.ODataAsynchronousResponseMessage CreateResponseMessage ()
	public System.Threading.Tasks.Task`1[[Microsoft.OData.Core.ODataAsynchronousResponseMessage]] CreateResponseMessageAsync ()
}

public sealed class Microsoft.OData.Core.ODataAsynchronousResponseMessage : IODataResponseMessage, IODataResponseMessageAsync {
	System.Collections.Generic.IEnumerable`1[[System.Collections.Generic.KeyValuePair`2[[System.String],[System.String]]]] Headers  { public virtual get; }
	int StatusCode  { public virtual get; public virtual set; }

	public virtual string GetHeader (string headerName)
	public virtual System.IO.Stream GetStream ()
	public virtual System.Threading.Tasks.Task`1[[System.IO.Stream]] GetStreamAsync ()
	public virtual void SetHeader (string headerName, string headerValue)
}

public sealed class Microsoft.OData.Core.ODataAsynchronousWriter : IODataOutputInStreamErrorListener {
	public Microsoft.OData.Core.ODataAsynchronousResponseMessage CreateResponseMessage ()
	public System.Threading.Tasks.Task`1[[Microsoft.OData.Core.ODataAsynchronousResponseMessage]] CreateResponseMessageAsync ()
	public void Flush ()
	public System.Threading.Tasks.Task FlushAsync ()
}

public sealed class Microsoft.OData.Core.ODataBatchOperationRequestMessage : IODataRequestMessage, IODataRequestMessageAsync, IODataUrlResolver {
	public const readonly string ContentId = 

	System.Collections.Generic.IEnumerable`1[[System.Collections.Generic.KeyValuePair`2[[System.String],[System.String]]]] Headers  { public virtual get; }
	string Method  { public virtual get; public virtual set; }
	System.Uri Url  { public virtual get; public virtual set; }

	public virtual string GetHeader (string headerName)
	public virtual System.IO.Stream GetStream ()
	public virtual System.Threading.Tasks.Task`1[[System.IO.Stream]] GetStreamAsync ()
	public virtual void SetHeader (string headerName, string headerValue)
}

public sealed class Microsoft.OData.Core.ODataBatchOperationResponseMessage : IODataResponseMessage, IODataResponseMessageAsync, IODataUrlResolver {
	public const readonly string ContentId = 

	System.Collections.Generic.IEnumerable`1[[System.Collections.Generic.KeyValuePair`2[[System.String],[System.String]]]] Headers  { public virtual get; }
	int StatusCode  { public virtual get; public virtual set; }

	public virtual string GetHeader (string headerName)
	public virtual System.IO.Stream GetStream ()
	public virtual System.Threading.Tasks.Task`1[[System.IO.Stream]] GetStreamAsync ()
	public virtual void SetHeader (string headerName, string headerValue)
}

public sealed class Microsoft.OData.Core.ODataBatchReader : IODataBatchOperationListener {
	Microsoft.OData.Core.ODataBatchReaderState State  { public get; }

	public Microsoft.OData.Core.ODataBatchOperationRequestMessage CreateOperationRequestMessage ()
	public System.Threading.Tasks.Task`1[[Microsoft.OData.Core.ODataBatchOperationRequestMessage]] CreateOperationRequestMessageAsync ()
	public Microsoft.OData.Core.ODataBatchOperationResponseMessage CreateOperationResponseMessage ()
	public System.Threading.Tasks.Task`1[[Microsoft.OData.Core.ODataBatchOperationResponseMessage]] CreateOperationResponseMessageAsync ()
	public bool Read ()
	public System.Threading.Tasks.Task`1[[System.Boolean]] ReadAsync ()
}

public sealed class Microsoft.OData.Core.ODataBatchWriter : IODataBatchOperationListener, IODataOutputInStreamErrorListener {
	public Microsoft.OData.Core.ODataBatchOperationRequestMessage CreateOperationRequestMessage (string method, System.Uri uri, string contentId)
	public System.Threading.Tasks.Task`1[[Microsoft.OData.Core.ODataBatchOperationRequestMessage]] CreateOperationRequestMessageAsync (string method, System.Uri uri, string contentId)
	public Microsoft.OData.Core.ODataBatchOperationResponseMessage CreateOperationResponseMessage (string contentId)
	public System.Threading.Tasks.Task`1[[Microsoft.OData.Core.ODataBatchOperationResponseMessage]] CreateOperationResponseMessageAsync (string contentId)
	public void Flush ()
	public System.Threading.Tasks.Task FlushAsync ()
	public void WriteEndBatch ()
	public System.Threading.Tasks.Task WriteEndBatchAsync ()
	public void WriteEndChangeset ()
	public System.Threading.Tasks.Task WriteEndChangesetAsync ()
	public void WriteStartBatch ()
	public System.Threading.Tasks.Task WriteStartBatchAsync ()
	public void WriteStartChangeset ()
	public System.Threading.Tasks.Task WriteStartChangesetAsync ()
}

public sealed class Microsoft.OData.Core.ODataCollectionStart : Microsoft.OData.Core.ODataAnnotatable {
	public ODataCollectionStart ()

	System.Nullable`1[[System.Int64]] Count  { public get; public set; }
	string Name  { public get; public set; }
	System.Uri NextPageLink  { public get; public set; }
}

public sealed class Microsoft.OData.Core.ODataCollectionStartSerializationInfo {
	public ODataCollectionStartSerializationInfo ()

	string CollectionTypeName  { public get; public set; }
}

public sealed class Microsoft.OData.Core.ODataCollectionValue : Microsoft.OData.Core.ODataValue {
	public ODataCollectionValue ()

	System.Collections.IEnumerable Items  { public get; public set; }
	string TypeName  { public get; public set; }
}

public sealed class Microsoft.OData.Core.ODataComplexValue : Microsoft.OData.Core.ODataValue {
	public ODataComplexValue ()

	System.Collections.Generic.ICollection`1[[Microsoft.OData.Core.ODataInstanceAnnotation]] InstanceAnnotations  { public get; public set; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Core.ODataProperty]] Properties  { public get; public set; }
	string TypeName  { public get; public set; }
}

public sealed class Microsoft.OData.Core.ODataDeltaDeletedEntry : Microsoft.OData.Core.ODataItem {
	public ODataDeltaDeletedEntry (string id, Microsoft.OData.Core.DeltaDeletedEntryReason reason)

	string Id  { public get; public set; }
	System.Nullable`1[[Microsoft.OData.Core.DeltaDeletedEntryReason]] Reason  { public get; public set; }
}

public sealed class Microsoft.OData.Core.ODataDeltaDeletedLink : Microsoft.OData.Core.ODataDeltaLinkBase {
	public ODataDeltaDeletedLink (System.Uri source, System.Uri target, string relationship)
}

public sealed class Microsoft.OData.Core.ODataDeltaFeed : Microsoft.OData.Core.ODataFeedBase {
	public ODataDeltaFeed ()
}

public sealed class Microsoft.OData.Core.ODataDeltaFeedSerializationInfo {
	public ODataDeltaFeedSerializationInfo ()

	string EntitySetName  { public get; public set; }
	string EntityTypeName  { public get; public set; }
	string ExpectedTypeName  { public get; public set; }
}

public sealed class Microsoft.OData.Core.ODataDeltaLink : Microsoft.OData.Core.ODataDeltaLinkBase {
	public ODataDeltaLink (System.Uri source, System.Uri target, string relationship)
}

public sealed class Microsoft.OData.Core.ODataDeltaSerializationInfo {
	public ODataDeltaSerializationInfo ()

	string NavigationSourceName  { public get; public set; }
}

[
DebuggerDisplayAttribute(),
]
public sealed class Microsoft.OData.Core.ODataEntityReferenceLink : Microsoft.OData.Core.ODataItem {
	public ODataEntityReferenceLink ()

	System.Collections.Generic.ICollection`1[[Microsoft.OData.Core.ODataInstanceAnnotation]] InstanceAnnotations  { public get; public set; }
	System.Uri Url  { public get; public set; }
}

public sealed class Microsoft.OData.Core.ODataEntityReferenceLinks : Microsoft.OData.Core.ODataAnnotatable {
	public ODataEntityReferenceLinks ()

	System.Nullable`1[[System.Int64]] Count  { public get; public set; }
	System.Collections.Generic.ICollection`1[[Microsoft.OData.Core.ODataInstanceAnnotation]] InstanceAnnotations  { public get; public set; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Core.ODataEntityReferenceLink]] Links  { public get; public set; }
	System.Uri NextPageLink  { public get; public set; }
}

public sealed class Microsoft.OData.Core.ODataEntitySetInfo : Microsoft.OData.Core.ODataServiceDocumentElement {
	public ODataEntitySetInfo ()
}

[
DebuggerDisplayAttribute(),
]
public sealed class Microsoft.OData.Core.ODataEntry : Microsoft.OData.Core.ODataItem {
	public ODataEntry ()

	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Core.ODataAction]] Actions  { public get; }
	System.Uri EditLink  { public get; public set; }
	string ETag  { public get; public set; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Core.ODataFunction]] Functions  { public get; }
	System.Uri Id  { public get; public set; }
	System.Collections.Generic.ICollection`1[[Microsoft.OData.Core.ODataInstanceAnnotation]] InstanceAnnotations  { public get; public set; }
	bool IsTransient  { public get; public set; }
	Microsoft.OData.Core.ODataStreamReferenceValue MediaResource  { public get; public set; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Core.ODataProperty]] Properties  { public get; public set; }
	System.Uri ReadLink  { public get; public set; }
	string TypeName  { public get; public set; }

	public void AddAction (Microsoft.OData.Core.ODataAction action)
	public void AddFunction (Microsoft.OData.Core.ODataFunction function)
}

public sealed class Microsoft.OData.Core.ODataEnumValue : Microsoft.OData.Core.ODataValue {
	public ODataEnumValue (string value)
	public ODataEnumValue (string value, string typeName)

	string TypeName  { public get; }
	string Value  { public get; }
}

[
DebuggerDisplayAttribute(),
]
public sealed class Microsoft.OData.Core.ODataError : Microsoft.OData.Core.ODataAnnotatable {
	public ODataError ()

	System.Collections.Generic.ICollection`1[[Microsoft.OData.Core.ODataErrorDetail]] Details  { public get; public set; }
	string ErrorCode  { public get; public set; }
	Microsoft.OData.Core.ODataInnerError InnerError  { public get; public set; }
	System.Collections.Generic.ICollection`1[[Microsoft.OData.Core.ODataInstanceAnnotation]] InstanceAnnotations  { public get; public set; }
	string Message  { public get; public set; }
	string Target  { public get; public set; }
}

public sealed class Microsoft.OData.Core.ODataErrorDetail {
	public ODataErrorDetail ()

	string ErrorCode  { public get; public set; }
	string Message  { public get; public set; }
	string Target  { public get; public set; }
}

[
DebuggerDisplayAttribute(),
]
public sealed class Microsoft.OData.Core.ODataErrorException : Microsoft.OData.Core.ODataException, _Exception, ISerializable {
	public ODataErrorException ()
	public ODataErrorException (Microsoft.OData.Core.ODataError error)
	public ODataErrorException (string message)
	public ODataErrorException (string message, Microsoft.OData.Core.ODataError error)
	public ODataErrorException (string message, System.Exception innerException)
	public ODataErrorException (string message, System.Exception innerException, Microsoft.OData.Core.ODataError error)

	Microsoft.OData.Core.ODataError Error  { public get; }
}

public sealed class Microsoft.OData.Core.ODataFeed : Microsoft.OData.Core.ODataFeedBase {
	public ODataFeed ()

	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Core.ODataAction]] Actions  { public get; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Core.ODataFunction]] Functions  { public get; }

	public void AddAction (Microsoft.OData.Core.ODataAction action)
	public void AddFunction (Microsoft.OData.Core.ODataFunction function)
}

public sealed class Microsoft.OData.Core.ODataFeedAndEntrySerializationInfo {
	public ODataFeedAndEntrySerializationInfo ()

	string ExpectedTypeName  { public get; public set; }
	bool IsFromCollection  { public get; public set; }
	string NavigationSourceEntityTypeName  { public get; public set; }
	Microsoft.OData.Edm.EdmNavigationSourceKind NavigationSourceKind  { public get; public set; }
	string NavigationSourceName  { public get; public set; }
}

[
DebuggerDisplayAttribute(),
]
public sealed class Microsoft.OData.Core.ODataFunction : Microsoft.OData.Core.ODataOperation {
	public ODataFunction ()
}

public sealed class Microsoft.OData.Core.ODataFunctionImportInfo : Microsoft.OData.Core.ODataServiceDocumentElement {
	public ODataFunctionImportInfo ()
}

[
DebuggerDisplayAttribute(),
]
public sealed class Microsoft.OData.Core.ODataInnerError {
	public ODataInnerError ()
	public ODataInnerError (System.Exception exception)

	Microsoft.OData.Core.ODataInnerError InnerError  { public get; public set; }
	string Message  { public get; public set; }
	string StackTrace  { public get; public set; }
	string TypeName  { public get; public set; }
}

public sealed class Microsoft.OData.Core.ODataInstanceAnnotation : Microsoft.OData.Core.ODataAnnotatable {
	public ODataInstanceAnnotation (string name, Microsoft.OData.Core.ODataValue value)

	string Name  { public get; }
	Microsoft.OData.Core.ODataValue Value  { public get; }
}

[
DebuggerDisplayAttribute(),
]
public sealed class Microsoft.OData.Core.ODataMediaType {
	public ODataMediaType (string type, string subType)
	public ODataMediaType (string type, string subType, System.Collections.Generic.IEnumerable`1[[System.Collections.Generic.KeyValuePair`2[[System.String],[System.String]]]] parameters)

	System.Collections.Generic.IEnumerable`1[[System.Collections.Generic.KeyValuePair`2[[System.String],[System.String]]]] Parameters  { public get; }
	string SubType  { public get; }
	string Type  { public get; }
}

public sealed class Microsoft.OData.Core.ODataMediaTypeFormat {
	public ODataMediaTypeFormat (Microsoft.OData.Core.ODataMediaType mediaType, Microsoft.OData.Core.ODataFormat format)

	Microsoft.OData.Core.ODataFormat Format  { public get; }
	Microsoft.OData.Core.ODataMediaType MediaType  { public get; }
}

public sealed class Microsoft.OData.Core.ODataMessageInfo {
	public ODataMessageInfo ()

	System.Text.Encoding Encoding  { public get; }
	System.Func`1[[System.IO.Stream]] GetMessageStream  { public get; }
	System.Func`1[[System.Threading.Tasks.Task`1[[System.IO.Stream]]]] GetMessageStreamAsync  { public get; }
	bool IsResponse  { public get; }
	Microsoft.OData.Core.ODataMediaType MediaType  { public get; }
	Microsoft.OData.Edm.IEdmModel Model  { public get; }
	Microsoft.OData.Core.IODataUrlResolver UrlResolver  { public get; }
}

public sealed class Microsoft.OData.Core.ODataMessageQuotas {
	public ODataMessageQuotas ()
	public ODataMessageQuotas (Microsoft.OData.Core.ODataMessageQuotas other)

	int MaxNestingDepth  { public get; public set; }
	int MaxOperationsPerChangeset  { public get; public set; }
	int MaxPartsPerBatch  { public get; public set; }
	long MaxReceivedMessageSize  { public get; public set; }
}

public sealed class Microsoft.OData.Core.ODataMessageReader : IDisposable {
	public ODataMessageReader (Microsoft.OData.Core.IODataRequestMessage requestMessage)
	public ODataMessageReader (Microsoft.OData.Core.IODataResponseMessage responseMessage)
	public ODataMessageReader (Microsoft.OData.Core.IODataRequestMessage requestMessage, Microsoft.OData.Core.ODataMessageReaderSettings settings)
	public ODataMessageReader (Microsoft.OData.Core.IODataResponseMessage responseMessage, Microsoft.OData.Core.ODataMessageReaderSettings settings)
	public ODataMessageReader (Microsoft.OData.Core.IODataRequestMessage requestMessage, Microsoft.OData.Core.ODataMessageReaderSettings settings, Microsoft.OData.Edm.IEdmModel model)
	public ODataMessageReader (Microsoft.OData.Core.IODataResponseMessage responseMessage, Microsoft.OData.Core.ODataMessageReaderSettings settings, Microsoft.OData.Edm.IEdmModel model)

	public Microsoft.OData.Core.ODataAsynchronousReader CreateODataAsynchronousReader ()
	public System.Threading.Tasks.Task`1[[Microsoft.OData.Core.ODataAsynchronousReader]] CreateODataAsynchronousReaderAsync ()
	public Microsoft.OData.Core.ODataBatchReader CreateODataBatchReader ()
	public System.Threading.Tasks.Task`1[[Microsoft.OData.Core.ODataBatchReader]] CreateODataBatchReaderAsync ()
	public Microsoft.OData.Core.ODataCollectionReader CreateODataCollectionReader ()
	public Microsoft.OData.Core.ODataCollectionReader CreateODataCollectionReader (Microsoft.OData.Edm.IEdmTypeReference expectedItemTypeReference)
	public System.Threading.Tasks.Task`1[[Microsoft.OData.Core.ODataCollectionReader]] CreateODataCollectionReaderAsync ()
	public System.Threading.Tasks.Task`1[[Microsoft.OData.Core.ODataCollectionReader]] CreateODataCollectionReaderAsync (Microsoft.OData.Edm.IEdmTypeReference expectedItemTypeReference)
	public Microsoft.OData.Core.ODataDeltaReader CreateODataDeltaReader (Microsoft.OData.Edm.IEdmEntitySetBase entitySet, Microsoft.OData.Edm.IEdmEntityType expectedBaseEntityType)
	public System.Threading.Tasks.Task`1[[Microsoft.OData.Core.ODataDeltaReader]] CreateODataDeltaReaderAsync (Microsoft.OData.Edm.IEdmEntitySetBase entitySet, Microsoft.OData.Edm.IEdmEntityType expectedBaseEntityType)
	public Microsoft.OData.Core.ODataReader CreateODataEntryReader ()
	public Microsoft.OData.Core.ODataReader CreateODataEntryReader (Microsoft.OData.Edm.IEdmEntityType entityType)
	public Microsoft.OData.Core.ODataReader CreateODataEntryReader (Microsoft.OData.Edm.IEdmNavigationSource navigationSource, Microsoft.OData.Edm.IEdmEntityType entityType)
	public System.Threading.Tasks.Task`1[[Microsoft.OData.Core.ODataReader]] CreateODataEntryReaderAsync ()
	public System.Threading.Tasks.Task`1[[Microsoft.OData.Core.ODataReader]] CreateODataEntryReaderAsync (Microsoft.OData.Edm.IEdmEntityType entityType)
	public System.Threading.Tasks.Task`1[[Microsoft.OData.Core.ODataReader]] CreateODataEntryReaderAsync (Microsoft.OData.Edm.IEdmNavigationSource navigationSource, Microsoft.OData.Edm.IEdmEntityType entityType)
	public Microsoft.OData.Core.ODataReader CreateODataFeedReader ()
	public Microsoft.OData.Core.ODataReader CreateODataFeedReader (Microsoft.OData.Edm.IEdmEntityType expectedBaseEntityType)
	public Microsoft.OData.Core.ODataReader CreateODataFeedReader (Microsoft.OData.Edm.IEdmEntitySetBase entitySet, Microsoft.OData.Edm.IEdmEntityType expectedBaseEntityType)
	public System.Threading.Tasks.Task`1[[Microsoft.OData.Core.ODataReader]] CreateODataFeedReaderAsync ()
	public System.Threading.Tasks.Task`1[[Microsoft.OData.Core.ODataReader]] CreateODataFeedReaderAsync (Microsoft.OData.Edm.IEdmEntityType expectedBaseEntityType)
	public System.Threading.Tasks.Task`1[[Microsoft.OData.Core.ODataReader]] CreateODataFeedReaderAsync (Microsoft.OData.Edm.IEdmEntitySetBase entitySet, Microsoft.OData.Edm.IEdmEntityType expectedBaseEntityType)
	public Microsoft.OData.Core.ODataParameterReader CreateODataParameterReader (Microsoft.OData.Edm.IEdmOperation operation)
	public System.Threading.Tasks.Task`1[[Microsoft.OData.Core.ODataParameterReader]] CreateODataParameterReaderAsync (Microsoft.OData.Edm.IEdmOperation operation)
	public System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Core.ODataPayloadKindDetectionResult]] DetectPayloadKind ()
	public System.Threading.Tasks.Task`1[[System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Core.ODataPayloadKindDetectionResult]]]] DetectPayloadKindAsync ()
	public virtual void Dispose ()
	public Microsoft.OData.Core.ODataEntityReferenceLink ReadEntityReferenceLink ()
	public System.Threading.Tasks.Task`1[[Microsoft.OData.Core.ODataEntityReferenceLink]] ReadEntityReferenceLinkAsync ()
	public Microsoft.OData.Core.ODataEntityReferenceLinks ReadEntityReferenceLinks ()
	public System.Threading.Tasks.Task`1[[Microsoft.OData.Core.ODataEntityReferenceLinks]] ReadEntityReferenceLinksAsync ()
	public Microsoft.OData.Core.ODataError ReadError ()
	public System.Threading.Tasks.Task`1[[Microsoft.OData.Core.ODataError]] ReadErrorAsync ()
	public Microsoft.OData.Edm.IEdmModel ReadMetadataDocument ()
	public Microsoft.OData.Edm.IEdmModel ReadMetadataDocument (System.Func`2[[System.Uri],[System.Xml.XmlReader]] getReferencedModelReaderFunc)
	public Microsoft.OData.Core.ODataProperty ReadProperty ()
	public Microsoft.OData.Core.ODataProperty ReadProperty (Microsoft.OData.Edm.IEdmStructuralProperty property)
	public Microsoft.OData.Core.ODataProperty ReadProperty (Microsoft.OData.Edm.IEdmTypeReference expectedPropertyTypeReference)
	public System.Threading.Tasks.Task`1[[Microsoft.OData.Core.ODataProperty]] ReadPropertyAsync ()
	public System.Threading.Tasks.Task`1[[Microsoft.OData.Core.ODataProperty]] ReadPropertyAsync (Microsoft.OData.Edm.IEdmStructuralProperty property)
	public System.Threading.Tasks.Task`1[[Microsoft.OData.Core.ODataProperty]] ReadPropertyAsync (Microsoft.OData.Edm.IEdmTypeReference expectedPropertyTypeReference)
	public Microsoft.OData.Core.ODataServiceDocument ReadServiceDocument ()
	public System.Threading.Tasks.Task`1[[Microsoft.OData.Core.ODataServiceDocument]] ReadServiceDocumentAsync ()
	public object ReadValue (Microsoft.OData.Edm.IEdmTypeReference expectedTypeReference)
	public System.Threading.Tasks.Task`1[[System.Object]] ReadValueAsync (Microsoft.OData.Edm.IEdmTypeReference expectedTypeReference)
}

public sealed class Microsoft.OData.Core.ODataMessageReaderSettings : Microsoft.OData.Core.ODataMessageReaderSettingsBase {
	public ODataMessageReaderSettings ()
	public ODataMessageReaderSettings (Microsoft.OData.Core.ODataMessageReaderSettings other)

	System.Uri BaseUri  { public get; public set; }
	bool DisableMessageStreamDisposal  { public get; public set; }
	bool DisablePrimitiveTypeConversion  { public get; public set; }
	bool EnableFullValidation  { public get; public set; }
	Microsoft.OData.Core.ODataVersion MaxProtocolVersion  { public get; public set; }
	Microsoft.OData.Core.ODataMediaTypeResolver MediaTypeResolver  { public get; public set; }
	bool ODataSimplified  { public get; public set; }
	System.Uri PayloadBaseUri  { public get; public set; }
	Microsoft.OData.Core.ODataUndeclaredPropertyBehaviorKinds UndeclaredPropertyBehaviorKinds  { public get; public set; }
	System.Nullable`1[[System.Boolean]] UseKeyAsSegment  { public get; public set; }

	public void EnableDefaultBehavior ()
	public void EnableODataServerBehavior ()
	public void EnableWcfDataServicesClientBehavior (System.Func`3[[Microsoft.OData.Edm.IEdmType],[System.String],[Microsoft.OData.Edm.IEdmType]] typeResolver)
}

public sealed class Microsoft.OData.Core.ODataMessageWriter : IDisposable {
	public ODataMessageWriter (Microsoft.OData.Core.IODataRequestMessage requestMessage)
	public ODataMessageWriter (Microsoft.OData.Core.IODataResponseMessage responseMessage)
	public ODataMessageWriter (Microsoft.OData.Core.IODataRequestMessage requestMessage, Microsoft.OData.Core.ODataMessageWriterSettings settings)
	public ODataMessageWriter (Microsoft.OData.Core.IODataResponseMessage responseMessage, Microsoft.OData.Core.ODataMessageWriterSettings settings)
	public ODataMessageWriter (Microsoft.OData.Core.IODataRequestMessage requestMessage, Microsoft.OData.Core.ODataMessageWriterSettings settings, Microsoft.OData.Edm.IEdmModel model)
	public ODataMessageWriter (Microsoft.OData.Core.IODataResponseMessage responseMessage, Microsoft.OData.Core.ODataMessageWriterSettings settings, Microsoft.OData.Edm.IEdmModel model)

	public Microsoft.OData.Core.ODataAsynchronousWriter CreateODataAsynchronousWriter ()
	public System.Threading.Tasks.Task`1[[Microsoft.OData.Core.ODataAsynchronousWriter]] CreateODataAsynchronousWriterAsync ()
	public Microsoft.OData.Core.ODataBatchWriter CreateODataBatchWriter ()
	public System.Threading.Tasks.Task`1[[Microsoft.OData.Core.ODataBatchWriter]] CreateODataBatchWriterAsync ()
	public Microsoft.OData.Core.ODataCollectionWriter CreateODataCollectionWriter ()
	public Microsoft.OData.Core.ODataCollectionWriter CreateODataCollectionWriter (Microsoft.OData.Edm.IEdmTypeReference itemTypeReference)
	public System.Threading.Tasks.Task`1[[Microsoft.OData.Core.ODataCollectionWriter]] CreateODataCollectionWriterAsync ()
	public System.Threading.Tasks.Task`1[[Microsoft.OData.Core.ODataCollectionWriter]] CreateODataCollectionWriterAsync (Microsoft.OData.Edm.IEdmTypeReference itemTypeReference)
	public Microsoft.OData.Core.ODataDeltaWriter CreateODataDeltaWriter (Microsoft.OData.Edm.IEdmEntitySetBase entitySet, Microsoft.OData.Edm.IEdmEntityType entityType)
	public System.Threading.Tasks.Task`1[[Microsoft.OData.Core.ODataDeltaWriter]] CreateODataDeltaWriterAsync (Microsoft.OData.Edm.IEdmEntitySetBase entitySet, Microsoft.OData.Edm.IEdmEntityType entityType)
	public Microsoft.OData.Core.ODataWriter CreateODataEntryWriter ()
	public Microsoft.OData.Core.ODataWriter CreateODataEntryWriter (Microsoft.OData.Edm.IEdmNavigationSource navigationSource)
	public Microsoft.OData.Core.ODataWriter CreateODataEntryWriter (Microsoft.OData.Edm.IEdmNavigationSource navigationSource, Microsoft.OData.Edm.IEdmEntityType entityType)
	public System.Threading.Tasks.Task`1[[Microsoft.OData.Core.ODataWriter]] CreateODataEntryWriterAsync ()
	public System.Threading.Tasks.Task`1[[Microsoft.OData.Core.ODataWriter]] CreateODataEntryWriterAsync (Microsoft.OData.Edm.IEdmNavigationSource navigationSource)
	public System.Threading.Tasks.Task`1[[Microsoft.OData.Core.ODataWriter]] CreateODataEntryWriterAsync (Microsoft.OData.Edm.IEdmNavigationSource navigationSource, Microsoft.OData.Edm.IEdmEntityType entityType)
	public Microsoft.OData.Core.ODataWriter CreateODataFeedWriter ()
	public Microsoft.OData.Core.ODataWriter CreateODataFeedWriter (Microsoft.OData.Edm.IEdmEntitySetBase entitySet)
	public Microsoft.OData.Core.ODataWriter CreateODataFeedWriter (Microsoft.OData.Edm.IEdmEntitySetBase entitySet, Microsoft.OData.Edm.IEdmEntityType entityType)
	public System.Threading.Tasks.Task`1[[Microsoft.OData.Core.ODataWriter]] CreateODataFeedWriterAsync ()
	public System.Threading.Tasks.Task`1[[Microsoft.OData.Core.ODataWriter]] CreateODataFeedWriterAsync (Microsoft.OData.Edm.IEdmEntitySetBase entitySet)
	public System.Threading.Tasks.Task`1[[Microsoft.OData.Core.ODataWriter]] CreateODataFeedWriterAsync (Microsoft.OData.Edm.IEdmEntitySetBase entitySet, Microsoft.OData.Edm.IEdmEntityType entityType)
	public Microsoft.OData.Core.ODataParameterWriter CreateODataParameterWriter (Microsoft.OData.Edm.IEdmOperation operation)
	public System.Threading.Tasks.Task`1[[Microsoft.OData.Core.ODataParameterWriter]] CreateODataParameterWriterAsync (Microsoft.OData.Edm.IEdmOperation operation)
	public virtual void Dispose ()
	public void WriteEntityReferenceLink (Microsoft.OData.Core.ODataEntityReferenceLink link)
	public System.Threading.Tasks.Task WriteEntityReferenceLinkAsync (Microsoft.OData.Core.ODataEntityReferenceLink link)
	public void WriteEntityReferenceLinks (Microsoft.OData.Core.ODataEntityReferenceLinks links)
	public System.Threading.Tasks.Task WriteEntityReferenceLinksAsync (Microsoft.OData.Core.ODataEntityReferenceLinks links)
	public void WriteError (Microsoft.OData.Core.ODataError error, bool includeDebugInformation)
	public System.Threading.Tasks.Task WriteErrorAsync (Microsoft.OData.Core.ODataError error, bool includeDebugInformation)
	public void WriteMetadataDocument ()
	public void WriteProperty (Microsoft.OData.Core.ODataProperty property)
	public System.Threading.Tasks.Task WritePropertyAsync (Microsoft.OData.Core.ODataProperty property)
	public void WriteServiceDocument (Microsoft.OData.Core.ODataServiceDocument serviceDocument)
	public System.Threading.Tasks.Task WriteServiceDocumentAsync (Microsoft.OData.Core.ODataServiceDocument serviceDocument)
	public void WriteValue (object value)
	public System.Threading.Tasks.Task WriteValueAsync (object value)
}

public sealed class Microsoft.OData.Core.ODataMessageWriterSettings : Microsoft.OData.Core.ODataMessageWriterSettingsBase {
	public ODataMessageWriterSettings ()
	public ODataMessageWriterSettings (Microsoft.OData.Core.ODataMessageWriterSettings other)

	bool AutoComputePayloadMetadataInJson  { public get; public set; }
	bool DisableMessageStreamDisposal  { public get; public set; }
	bool EnableFullValidation  { public get; public set; }
	string JsonPCallback  { public get; public set; }
	Microsoft.OData.Core.ODataMediaTypeResolver MediaTypeResolver  { public get; public set; }
	bool ODataSimplified  { public get; public set; }
	Microsoft.OData.Core.ODataUri ODataUri  { public get; public set; }
	System.Uri PayloadBaseUri  { public get; public set; }
	System.Nullable`1[[System.Boolean]] UseKeyAsSegment  { public get; public set; }
	System.Nullable`1[[Microsoft.OData.Core.ODataVersion]] Version  { public get; public set; }

	public void EnableDefaultBehavior ()
	public void EnableODataServerBehavior ()
	public void EnableODataServerBehavior (bool alwaysUseDefaultXmlNamespaceForRootElement)
	public void EnableWcfDataServicesClientBehavior ()
	public void SetContentType (Microsoft.OData.Core.ODataFormat payloadFormat)
	public void SetContentType (string acceptableMediaTypes, string acceptableCharSets)
}

[
DebuggerDisplayAttribute(),
]
public sealed class Microsoft.OData.Core.ODataNavigationLink : Microsoft.OData.Core.ODataItem {
	public ODataNavigationLink ()

	System.Uri AssociationLinkUrl  { public get; public set; }
	System.Nullable`1[[System.Boolean]] IsCollection  { public get; public set; }
	string Name  { public get; public set; }
	System.Uri Url  { public get; public set; }
}

public sealed class Microsoft.OData.Core.ODataNullValue : Microsoft.OData.Core.ODataValue {
	public ODataNullValue ()
}

public sealed class Microsoft.OData.Core.ODataPayloadKindDetectionResult {
	Microsoft.OData.Core.ODataFormat Format  { public get; }
	Microsoft.OData.Core.ODataPayloadKind PayloadKind  { public get; }
}

public sealed class Microsoft.OData.Core.ODataPreferenceHeader {
	string AnnotationFilter  { public get; public set; }
	bool ContinueOnError  { public get; public set; }
	System.Nullable`1[[System.Int32]] MaxPageSize  { public get; public set; }
	bool RespondAsync  { public get; public set; }
	System.Nullable`1[[System.Boolean]] ReturnContent  { public get; public set; }
	bool TrackChanges  { public get; public set; }
	System.Nullable`1[[System.Int32]] Wait  { public get; public set; }
}

public sealed class Microsoft.OData.Core.ODataPrimitiveValue : Microsoft.OData.Core.ODataValue {
	public ODataPrimitiveValue (object value)

	object Value  { public get; }
}

public sealed class Microsoft.OData.Core.ODataProperty : Microsoft.OData.Core.ODataAnnotatable {
	public ODataProperty ()

	System.Collections.Generic.ICollection`1[[Microsoft.OData.Core.ODataInstanceAnnotation]] InstanceAnnotations  { public get; public set; }
	string Name  { public get; public set; }
	object Value  { public get; public set; }
}

public sealed class Microsoft.OData.Core.ODataPropertySerializationInfo {
	public ODataPropertySerializationInfo ()

	Microsoft.OData.Core.ODataPropertyKind PropertyKind  { public get; public set; }
}

public sealed class Microsoft.OData.Core.ODataServiceDocument : Microsoft.OData.Core.ODataAnnotatable {
	public ODataServiceDocument ()

	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Core.ODataEntitySetInfo]] EntitySets  { public get; public set; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Core.ODataFunctionImportInfo]] FunctionImports  { public get; public set; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Core.ODataSingletonInfo]] Singletons  { public get; public set; }
}

public sealed class Microsoft.OData.Core.ODataSingletonInfo : Microsoft.OData.Core.ODataServiceDocumentElement {
	public ODataSingletonInfo ()
}

public sealed class Microsoft.OData.Core.ODataStreamReferenceValue : Microsoft.OData.Core.ODataValue {
	public ODataStreamReferenceValue ()

	string ContentType  { public get; public set; }
	System.Uri EditLink  { public get; public set; }
	string ETag  { public get; public set; }
	System.Uri ReadLink  { public get; public set; }
}

public sealed class Microsoft.OData.Core.ODataUntypedValue : Microsoft.OData.Core.ODataValue {
	public ODataUntypedValue ()

	string RawValue  { public get; public set; }
}

public sealed class Microsoft.OData.Core.ODataUri {
	public ODataUri ()

	Microsoft.OData.Core.UriParser.Aggregation.ApplyClause Apply  { public get; public set; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Core.UriParser.Semantic.QueryNode]] CustomQueryOptions  { public get; public set; }
	string DeltaToken  { public get; public set; }
	Microsoft.OData.Core.UriParser.Semantic.FilterClause Filter  { public get; public set; }
	Microsoft.OData.Core.UriParser.Semantic.OrderByClause OrderBy  { public get; public set; }
	System.Collections.Generic.IDictionary`2[[System.String],[Microsoft.OData.Core.UriParser.Semantic.SingleValueNode]] ParameterAliasNodes  { public get; }
	Microsoft.OData.Core.UriParser.Semantic.ODataPath Path  { public get; public set; }
	System.Nullable`1[[System.Boolean]] QueryCount  { public get; public set; }
	System.Uri RequestUri  { public get; public set; }
	Microsoft.OData.Core.UriParser.Semantic.SearchClause Search  { public get; public set; }
	Microsoft.OData.Core.UriParser.Semantic.SelectExpandClause SelectAndExpand  { public get; public set; }
	System.Uri ServiceRoot  { public get; public set; }
	System.Nullable`1[[System.Int64]] Skip  { public get; public set; }
	string SkipToken  { public get; public set; }
	System.Nullable`1[[System.Int64]] Top  { public get; public set; }

	public Microsoft.OData.Core.ODataUri Clone ()
}

public sealed class Microsoft.OData.Core.ProjectedPropertiesAnnotation {
	public ProjectedPropertiesAnnotation (System.Collections.Generic.IEnumerable`1[[System.String]] projectedPropertyNames)
}

public sealed class Microsoft.OData.Core.SerializationTypeNameAnnotation {
	public SerializationTypeNameAnnotation ()

	string TypeName  { public get; public set; }
}

public enum Microsoft.OData.Core.Atom.AtomSyndicationItemProperty : int {
	AuthorEmail = 1
	AuthorName = 2
	AuthorUri = 3
	ContributorEmail = 4
	ContributorName = 5
	ContributorUri = 6
	CustomProperty = 0
	Published = 8
	Rights = 9
	Summary = 10
	Title = 11
	Updated = 7
}

public enum Microsoft.OData.Core.Atom.AtomSyndicationTextContentKind : int {
	Html = 1
	Plaintext = 0
	Xhtml = 2
}

public enum Microsoft.OData.Core.Atom.AtomTextConstructKind : int {
	Html = 1
	Text = 0
	Xhtml = 2
}

[
ExtensionAttribute(),
]
public sealed class Microsoft.OData.Core.Atom.ExtensionMethods {
	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Core.Atom.AtomResourceCollectionMetadata Atom (Microsoft.OData.Core.ODataEntitySetInfo entitySet)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Core.Atom.AtomEntryMetadata Atom (Microsoft.OData.Core.ODataEntry entry)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Core.Atom.AtomFeedMetadata Atom (Microsoft.OData.Core.ODataFeed feed)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Core.Atom.AtomLinkMetadata Atom (Microsoft.OData.Core.ODataNavigationLink navigationLink)

	[
	ExtensionAttribute(),
	]
	public static Microsoft.OData.Core.Atom.AtomWorkspaceMetadata Atom (Microsoft.OData.Core.ODataServiceDocument serviceDocument)
}

public sealed class Microsoft.OData.Core.Atom.AtomCategoriesMetadata {
	public AtomCategoriesMetadata ()

	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Core.Atom.AtomCategoryMetadata]] Categories  { public get; public set; }
	System.Nullable`1[[System.Boolean]] Fixed  { public get; public set; }
	System.Uri Href  { public get; public set; }
	string Scheme  { public get; public set; }
}

public sealed class Microsoft.OData.Core.Atom.AtomCategoryMetadata : Microsoft.OData.Core.ODataAnnotatable {
	public AtomCategoryMetadata ()

	string Label  { public get; public set; }
	string Scheme  { public get; public set; }
	string Term  { public get; public set; }
}

public sealed class Microsoft.OData.Core.Atom.AtomEntryMetadata : Microsoft.OData.Core.ODataAnnotatable {
	public AtomEntryMetadata ()

	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Core.Atom.AtomPersonMetadata]] Authors  { public get; public set; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Core.Atom.AtomCategoryMetadata]] Categories  { public get; public set; }
	Microsoft.OData.Core.Atom.AtomCategoryMetadata CategoryWithTypeName  { public get; public set; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Core.Atom.AtomPersonMetadata]] Contributors  { public get; public set; }
	Microsoft.OData.Core.Atom.AtomLinkMetadata EditLink  { public get; public set; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Core.Atom.AtomLinkMetadata]] Links  { public get; public set; }
	System.Nullable`1[[System.DateTimeOffset]] Published  { public get; public set; }
	Microsoft.OData.Core.Atom.AtomTextConstruct Rights  { public get; public set; }
	Microsoft.OData.Core.Atom.AtomLinkMetadata SelfLink  { public get; public set; }
	Microsoft.OData.Core.Atom.AtomFeedMetadata Source  { public get; public set; }
	Microsoft.OData.Core.Atom.AtomTextConstruct Summary  { public get; public set; }
	Microsoft.OData.Core.Atom.AtomTextConstruct Title  { public get; public set; }
	System.Nullable`1[[System.DateTimeOffset]] Updated  { public get; public set; }
}

public sealed class Microsoft.OData.Core.Atom.AtomFeedMetadata : Microsoft.OData.Core.ODataAnnotatable {
	public AtomFeedMetadata ()

	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Core.Atom.AtomPersonMetadata]] Authors  { public get; public set; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Core.Atom.AtomCategoryMetadata]] Categories  { public get; public set; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Core.Atom.AtomPersonMetadata]] Contributors  { public get; public set; }
	Microsoft.OData.Core.Atom.AtomGeneratorMetadata Generator  { public get; public set; }
	System.Uri Icon  { public get; public set; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Core.Atom.AtomLinkMetadata]] Links  { public get; public set; }
	System.Uri Logo  { public get; public set; }
	Microsoft.OData.Core.Atom.AtomLinkMetadata NextPageLink  { public get; public set; }
	Microsoft.OData.Core.Atom.AtomTextConstruct Rights  { public get; public set; }
	Microsoft.OData.Core.Atom.AtomLinkMetadata SelfLink  { public get; public set; }
	System.Uri SourceId  { public get; public set; }
	Microsoft.OData.Core.Atom.AtomTextConstruct Subtitle  { public get; public set; }
	Microsoft.OData.Core.Atom.AtomTextConstruct Title  { public get; public set; }
	System.Nullable`1[[System.DateTimeOffset]] Updated  { public get; public set; }
}

public sealed class Microsoft.OData.Core.Atom.AtomGeneratorMetadata {
	public AtomGeneratorMetadata ()

	string Name  { public get; public set; }
	System.Uri Uri  { public get; public set; }
	string Version  { public get; public set; }
}

public sealed class Microsoft.OData.Core.Atom.AtomLinkMetadata : Microsoft.OData.Core.ODataAnnotatable {
	public AtomLinkMetadata ()

	System.Uri Href  { public get; public set; }
	string HrefLang  { public get; public set; }
	System.Nullable`1[[System.Int32]] Length  { public get; public set; }
	string MediaType  { public get; public set; }
	string Relation  { public get; public set; }
	string Title  { public get; public set; }
}

public sealed class Microsoft.OData.Core.Atom.AtomPersonMetadata : Microsoft.OData.Core.ODataAnnotatable {
	public AtomPersonMetadata ()

	string Email  { public get; public set; }
	string Name  { public get; public set; }
	System.Uri Uri  { public get; public set; }

	public static Microsoft.OData.Core.Atom.AtomPersonMetadata ToAtomPersonMetadata (string name)
}

public sealed class Microsoft.OData.Core.Atom.AtomResourceCollectionMetadata {
	public AtomResourceCollectionMetadata ()

	string Accept  { public get; public set; }
	Microsoft.OData.Core.Atom.AtomCategoriesMetadata Categories  { public get; public set; }
	Microsoft.OData.Core.Atom.AtomTextConstruct Title  { public get; public set; }
}

public sealed class Microsoft.OData.Core.Atom.AtomStreamReferenceMetadata : Microsoft.OData.Core.ODataAnnotatable {
	public AtomStreamReferenceMetadata ()

	Microsoft.OData.Core.Atom.AtomLinkMetadata EditLink  { public get; public set; }
	Microsoft.OData.Core.Atom.AtomLinkMetadata SelfLink  { public get; public set; }
}

public sealed class Microsoft.OData.Core.Atom.AtomTextConstruct : Microsoft.OData.Core.ODataAnnotatable {
	public AtomTextConstruct ()

	Microsoft.OData.Core.Atom.AtomTextConstructKind Kind  { public get; public set; }
	string Text  { public get; public set; }

	public static Microsoft.OData.Core.Atom.AtomTextConstruct ToTextConstruct (string text)
}

public sealed class Microsoft.OData.Core.Atom.AtomWorkspaceMetadata {
	public AtomWorkspaceMetadata ()

	Microsoft.OData.Core.Atom.AtomTextConstruct Title  { public get; public set; }
}

public enum Microsoft.OData.Core.Metadata.ODataNullValueBehaviorKind : int {
	Default = 0
	DisableValidation = 2
	IgnoreValue = 1
}

public sealed class Microsoft.OData.Core.Metadata.ODataEdmPropertyAnnotation {
	public ODataEdmPropertyAnnotation ()

	Microsoft.OData.Core.Metadata.ODataNullValueBehaviorKind NullValueReadBehaviorKind  { public get; public set; }
}

public sealed class Microsoft.OData.Core.UriBuilder.ODataUriBuilder {
	public ODataUriBuilder (Microsoft.OData.Core.UriParser.ODataUrlConventions urlConventions, Microsoft.OData.Core.ODataUri odataUri)

	public System.Uri BuildUri ()
}

public enum Microsoft.OData.Core.UriParser.OrderByDirection : int {
	Ascending = 0
	Descending = 1
}

public sealed class Microsoft.OData.Core.UriParser.CustomUriFunctions {
	public static void AddCustomUriFunction (string functionName, Microsoft.OData.Core.UriParser.FunctionSignatureWithReturnType functionSignature)
	public static bool RemoveCustomUriFunction (string functionName)
	public static bool RemoveCustomUriFunction (string functionName, Microsoft.OData.Core.UriParser.FunctionSignatureWithReturnType functionSignature)
}

public sealed class Microsoft.OData.Core.UriParser.CustomUriLiteralPrefixes {
	public static void AddCustomLiteralPrefix (string literalPrefix, Microsoft.OData.Edm.IEdmTypeReference literalEdmTypeReference)
	public static bool RemoveCustomLiteralPrefix (string literalPrefix)
}

public sealed class Microsoft.OData.Core.UriParser.ODataUriUtils {
	public static object ConvertFromUriLiteral (string value, Microsoft.OData.Core.ODataVersion version)
	public static object ConvertFromUriLiteral (string value, Microsoft.OData.Core.ODataVersion version, Microsoft.OData.Edm.IEdmModel model, Microsoft.OData.Edm.IEdmTypeReference typeReference)
	public static string ConvertToUriLiteral (object value, Microsoft.OData.Core.ODataVersion version)
	public static string ConvertToUriLiteral (object value, Microsoft.OData.Core.ODataVersion version, Microsoft.OData.Edm.IEdmModel model)
}

public class Microsoft.OData.Core.UriParser.ODataQueryOptionParser {
	public ODataQueryOptionParser (Microsoft.OData.Edm.IEdmModel model, Microsoft.OData.Edm.IEdmType targetEdmType, Microsoft.OData.Edm.IEdmNavigationSource targetNavigationSource, System.Collections.Generic.IDictionary`2[[System.String],[System.String]] queryOptions)

	System.Collections.Generic.IDictionary`2[[System.String],[Microsoft.OData.Core.UriParser.Semantic.SingleValueNode]] ParameterAliasNodes  { public get; }
	Microsoft.OData.Core.UriParser.Metadata.ODataUriResolver Resolver  { public get; public set; }
	Microsoft.OData.Core.UriParser.ODataUriParserSettings Settings  { public get; }

	public Microsoft.OData.Core.UriParser.Aggregation.ApplyClause ParseApply ()
	public System.Nullable`1[[System.Boolean]] ParseCount ()
	public string ParseDeltaToken ()
	public Microsoft.OData.Core.UriParser.Semantic.FilterClause ParseFilter ()
	public Microsoft.OData.Core.UriParser.Semantic.OrderByClause ParseOrderBy ()
	public Microsoft.OData.Core.UriParser.Semantic.SearchClause ParseSearch ()
	public Microsoft.OData.Core.UriParser.Semantic.SelectExpandClause ParseSelectAndExpand ()
	public System.Nullable`1[[System.Int64]] ParseSkip ()
	public string ParseSkipToken ()
	public System.Nullable`1[[System.Int64]] ParseTop ()
}

public sealed class Microsoft.OData.Core.UriParser.FunctionSignatureWithReturnType {
	public FunctionSignatureWithReturnType (Microsoft.OData.Edm.IEdmTypeReference returnType, Microsoft.OData.Edm.IEdmTypeReference[] argumentTypes)

	Microsoft.OData.Edm.IEdmTypeReference[] ArgumentTypes  { public get; }
	Microsoft.OData.Edm.IEdmTypeReference ReturnType  { public get; }
}

public sealed class Microsoft.OData.Core.UriParser.KeyPropertyValue {
	public KeyPropertyValue ()

	Microsoft.OData.Edm.IEdmProperty KeyProperty  { public get; public set; }
	Microsoft.OData.Core.UriParser.Semantic.SingleValueNode KeyValue  { public get; public set; }
}

[
DebuggerDisplayAttribute(),
]
public sealed class Microsoft.OData.Core.UriParser.ODataUnrecognizedPathException : Microsoft.OData.Core.ODataException, _Exception, ISerializable {
	public ODataUnrecognizedPathException ()
	public ODataUnrecognizedPathException (string message)
	public ODataUnrecognizedPathException (string message, System.Exception innerException)

	string CurrentSegment  { public get; public set; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Core.UriParser.Semantic.ODataPathSegment]] ParsedSegments  { public get; public set; }
	System.Collections.Generic.IEnumerable`1[[System.String]] UnparsedSegments  { public get; public set; }
}

public sealed class Microsoft.OData.Core.UriParser.ODataUriParser {
	public ODataUriParser (Microsoft.OData.Edm.IEdmModel model, System.Uri fullUri)
	public ODataUriParser (Microsoft.OData.Edm.IEdmModel model, System.Uri serviceRoot, System.Uri fullUri)

	System.Func`2[[System.String],[Microsoft.OData.Core.UriParser.Semantic.BatchReferenceSegment]] BatchReferenceCallback  { public get; public set; }
	bool EnableUriTemplateParsing  { public get; public set; }
	Microsoft.OData.Edm.IEdmModel Model  { public get; }
	System.Collections.Generic.IDictionary`2[[System.String],[Microsoft.OData.Core.UriParser.Semantic.SingleValueNode]] ParameterAliasNodes  { public get; }
	Microsoft.OData.Core.UriParser.Metadata.ODataUriResolver Resolver  { public get; public set; }
	System.Uri ServiceRoot  { public get; }
	Microsoft.OData.Core.UriParser.ODataUriParserSettings Settings  { public get; }
	Microsoft.OData.Core.UriParser.ODataUrlConventions UrlConventions  { public get; public set; }

	public Microsoft.OData.Core.UriParser.Aggregation.ApplyClause ParseApply ()
	public System.Nullable`1[[System.Boolean]] ParseCount ()
	public string ParseDeltaToken ()
	public Microsoft.OData.Core.UriParser.Semantic.EntityIdSegment ParseEntityId ()
	public Microsoft.OData.Core.UriParser.Semantic.FilterClause ParseFilter ()
	public Microsoft.OData.Core.UriParser.Semantic.OrderByClause ParseOrderBy ()
	public Microsoft.OData.Core.UriParser.Semantic.ODataPath ParsePath ()
	public Microsoft.OData.Core.UriParser.Semantic.SearchClause ParseSearch ()
	public Microsoft.OData.Core.UriParser.Semantic.SelectExpandClause ParseSelectAndExpand ()
	public System.Nullable`1[[System.Int64]] ParseSkip ()
	public string ParseSkipToken ()
	public System.Nullable`1[[System.Int64]] ParseTop ()
	public Microsoft.OData.Core.ODataUri ParseUri ()
}

public sealed class Microsoft.OData.Core.UriParser.ODataUriParserSettings {
	public ODataUriParserSettings ()

	int MaximumExpansionCount  { public get; public set; }
	int MaximumExpansionDepth  { public get; public set; }
}

public sealed class Microsoft.OData.Core.UriParser.ODataUrlConventions {
	Microsoft.OData.Core.UriParser.ODataUrlConventions Default  { public static get; }
	Microsoft.OData.Core.UriParser.ODataUrlConventions KeyAsSegment  { public static get; }
	Microsoft.OData.Core.UriParser.ODataUrlConventions ODataSimplified  { public static get; }
}

public enum Microsoft.OData.Core.UriParser.Aggregation.AggregationMethod : int {
	Average = 3
	CountDistinct = 4
	Max = 2
	Min = 1
	Sum = 0
}

public enum Microsoft.OData.Core.UriParser.Aggregation.TransformationNodeKind : int {
	Aggregate = 0
	Filter = 2
	GroupBy = 1
}

public abstract class Microsoft.OData.Core.UriParser.Aggregation.TransformationNode {
	protected TransformationNode ()

	Microsoft.OData.Core.UriParser.Aggregation.TransformationNodeKind Kind  { public abstract get; }
}

public sealed class Microsoft.OData.Core.UriParser.Aggregation.AggregateExpression {
	public AggregateExpression (Microsoft.OData.Core.UriParser.Semantic.SingleValueNode expression, Microsoft.OData.Core.UriParser.Aggregation.AggregationMethod method, string alias, Microsoft.OData.Edm.IEdmTypeReference typeReference)

	string Alias  { public get; }
	Microsoft.OData.Core.UriParser.Semantic.SingleValueNode Expression  { public get; }
	Microsoft.OData.Core.UriParser.Aggregation.AggregationMethod Method  { public get; }
	Microsoft.OData.Edm.IEdmTypeReference TypeReference  { public get; }
}

public sealed class Microsoft.OData.Core.UriParser.Aggregation.AggregateTransformationNode : Microsoft.OData.Core.UriParser.Aggregation.TransformationNode {
	public AggregateTransformationNode (System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Core.UriParser.Aggregation.AggregateExpression]] expressions)

	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Core.UriParser.Aggregation.AggregateExpression]] Expressions  { public get; }
	Microsoft.OData.Core.UriParser.Aggregation.TransformationNodeKind Kind  { public virtual get; }
}

public sealed class Microsoft.OData.Core.UriParser.Aggregation.ApplyClause {
	public ApplyClause (System.Collections.Generic.IList`1[[Microsoft.OData.Core.UriParser.Aggregation.TransformationNode]] transformations)

	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Core.UriParser.Aggregation.TransformationNode]] Transformations  { public get; }
}

public sealed class Microsoft.OData.Core.UriParser.Aggregation.FilterTransformationNode : Microsoft.OData.Core.UriParser.Aggregation.TransformationNode {
	public FilterTransformationNode (Microsoft.OData.Core.UriParser.Semantic.FilterClause filterClause)

	Microsoft.OData.Core.UriParser.Semantic.FilterClause FilterClause  { public get; }
	Microsoft.OData.Core.UriParser.Aggregation.TransformationNodeKind Kind  { public virtual get; }
}

public sealed class Microsoft.OData.Core.UriParser.Aggregation.GroupByPropertyNode {
	public GroupByPropertyNode (string name, Microsoft.OData.Core.UriParser.Semantic.SingleValueNode expression)
	public GroupByPropertyNode (string name, Microsoft.OData.Core.UriParser.Semantic.SingleValueNode expression, Microsoft.OData.Edm.IEdmTypeReference type)

	System.Collections.Generic.IList`1[[Microsoft.OData.Core.UriParser.Aggregation.GroupByPropertyNode]] ChildTransformations  { public get; public set; }
	Microsoft.OData.Core.UriParser.Semantic.SingleValueNode Expression  { public get; }
	string Name  { public get; }
	Microsoft.OData.Edm.IEdmTypeReference TypeReference  { public get; }
}

public sealed class Microsoft.OData.Core.UriParser.Aggregation.GroupByTransformationNode : Microsoft.OData.Core.UriParser.Aggregation.TransformationNode {
	public GroupByTransformationNode (System.Collections.Generic.IList`1[[Microsoft.OData.Core.UriParser.Aggregation.GroupByPropertyNode]] groupingProperties, Microsoft.OData.Core.UriParser.Aggregation.TransformationNode childTransformations, Microsoft.OData.Core.UriParser.Semantic.CollectionNode source)

	Microsoft.OData.Core.UriParser.Aggregation.TransformationNode ChildTransformations  { public get; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Core.UriParser.Aggregation.GroupByPropertyNode]] GroupingProperties  { public get; }
	Microsoft.OData.Core.UriParser.Aggregation.TransformationNodeKind Kind  { public virtual get; }
	Microsoft.OData.Core.UriParser.Semantic.CollectionNode Source  { public get; }
}

public class Microsoft.OData.Core.UriParser.Metadata.ODataUriResolver {
	public ODataUriResolver ()

	bool EnableCaseInsensitive  { public virtual get; public virtual set; }

	public virtual void PromoteBinaryOperandTypes (Microsoft.OData.Core.UriParser.TreeNodeKinds.BinaryOperatorKind binaryOperatorKind, Microsoft.OData.Core.UriParser.Semantic.SingleValueNode& leftNode, Microsoft.OData.Core.UriParser.Semantic.SingleValueNode& rightNode, out Microsoft.OData.Edm.IEdmTypeReference& typeReference)
	public virtual System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmOperation]] ResolveBoundOperations (Microsoft.OData.Edm.IEdmModel model, string identifier, Microsoft.OData.Edm.IEdmType bindingType)
	public virtual System.Collections.Generic.IEnumerable`1[[System.Collections.Generic.KeyValuePair`2[[System.String],[System.Object]]]] ResolveKeys (Microsoft.OData.Edm.IEdmEntityType type, System.Collections.Generic.IDictionary`2[[System.String],[System.String]] namedValues, System.Func`3[[Microsoft.OData.Edm.IEdmTypeReference],[System.String],[System.Object]] convertFunc)
	public virtual System.Collections.Generic.IEnumerable`1[[System.Collections.Generic.KeyValuePair`2[[System.String],[System.Object]]]] ResolveKeys (Microsoft.OData.Edm.IEdmEntityType type, System.Collections.Generic.IList`1[[System.String]] positionalValues, System.Func`3[[Microsoft.OData.Edm.IEdmTypeReference],[System.String],[System.Object]] convertFunc)
	public virtual Microsoft.OData.Edm.IEdmNavigationSource ResolveNavigationSource (Microsoft.OData.Edm.IEdmModel model, string identifier)
	public virtual System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmOperationImport]] ResolveOperationImports (Microsoft.OData.Edm.IEdmModel model, string identifier)
	public virtual System.Collections.Generic.IDictionary`2[[Microsoft.OData.Edm.IEdmOperationParameter],[Microsoft.OData.Core.UriParser.Semantic.SingleValueNode]] ResolveOperationParameters (Microsoft.OData.Edm.IEdmOperation operation, System.Collections.Generic.IDictionary`2[[System.String],[Microsoft.OData.Core.UriParser.Semantic.SingleValueNode]] input)
	public virtual Microsoft.OData.Edm.IEdmProperty ResolveProperty (Microsoft.OData.Edm.IEdmStructuredType type, string propertyName)
	public virtual Microsoft.OData.Edm.IEdmSchemaType ResolveType (Microsoft.OData.Edm.IEdmModel model, string typeName)
	public virtual System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmOperation]] ResolveUnboundOperations (Microsoft.OData.Edm.IEdmModel model, string identifier)
}

public class Microsoft.OData.Core.UriParser.Metadata.UnqualifiedODataUriResolver : Microsoft.OData.Core.UriParser.Metadata.ODataUriResolver {
	public UnqualifiedODataUriResolver ()

	public virtual System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmOperation]] ResolveBoundOperations (Microsoft.OData.Edm.IEdmModel model, string identifier, Microsoft.OData.Edm.IEdmType bindingType)
	public virtual System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmOperation]] ResolveUnboundOperations (Microsoft.OData.Edm.IEdmModel model, string identifier)
}

public sealed class Microsoft.OData.Core.UriParser.Metadata.AlternateKeysODataUriResolver : Microsoft.OData.Core.UriParser.Metadata.ODataUriResolver {
	public AlternateKeysODataUriResolver (Microsoft.OData.Edm.IEdmModel model)

	public virtual System.Collections.Generic.IEnumerable`1[[System.Collections.Generic.KeyValuePair`2[[System.String],[System.Object]]]] ResolveKeys (Microsoft.OData.Edm.IEdmEntityType type, System.Collections.Generic.IDictionary`2[[System.String],[System.String]] namedValues, System.Func`3[[Microsoft.OData.Edm.IEdmTypeReference],[System.String],[System.Object]] convertFunc)
}

public sealed class Microsoft.OData.Core.UriParser.Metadata.StringAsEnumResolver : Microsoft.OData.Core.UriParser.Metadata.ODataUriResolver {
	public StringAsEnumResolver ()

	public virtual void PromoteBinaryOperandTypes (Microsoft.OData.Core.UriParser.TreeNodeKinds.BinaryOperatorKind binaryOperatorKind, Microsoft.OData.Core.UriParser.Semantic.SingleValueNode& leftNode, Microsoft.OData.Core.UriParser.Semantic.SingleValueNode& rightNode, out Microsoft.OData.Edm.IEdmTypeReference& typeReference)
	public virtual System.Collections.Generic.IEnumerable`1[[System.Collections.Generic.KeyValuePair`2[[System.String],[System.Object]]]] ResolveKeys (Microsoft.OData.Edm.IEdmEntityType type, System.Collections.Generic.IDictionary`2[[System.String],[System.String]] namedValues, System.Func`3[[Microsoft.OData.Edm.IEdmTypeReference],[System.String],[System.Object]] convertFunc)
	public virtual System.Collections.Generic.IEnumerable`1[[System.Collections.Generic.KeyValuePair`2[[System.String],[System.Object]]]] ResolveKeys (Microsoft.OData.Edm.IEdmEntityType type, System.Collections.Generic.IList`1[[System.String]] positionalValues, System.Func`3[[Microsoft.OData.Edm.IEdmTypeReference],[System.String],[System.Object]] convertFunc)
	public virtual System.Collections.Generic.IDictionary`2[[Microsoft.OData.Edm.IEdmOperationParameter],[Microsoft.OData.Core.UriParser.Semantic.SingleValueNode]] ResolveOperationParameters (Microsoft.OData.Edm.IEdmOperation operation, System.Collections.Generic.IDictionary`2[[System.String],[Microsoft.OData.Core.UriParser.Semantic.SingleValueNode]] input)
}

public sealed class Microsoft.OData.Core.UriParser.Parsers.CustomUriLiteralParsers : IUriLiteralParser {
	public static void AddCustomUriLiteralParser (Microsoft.OData.Core.UriParser.Parsers.Common.IUriLiteralParser customUriLiteralParser)
	public static void AddCustomUriLiteralParser (Microsoft.OData.Edm.IEdmTypeReference edmTypeReference, Microsoft.OData.Core.UriParser.Parsers.Common.IUriLiteralParser customUriLiteralParser)
	public virtual object ParseUriStringToType (string text, Microsoft.OData.Edm.IEdmTypeReference targetType, out Microsoft.OData.Core.UriParser.Parsers.Common.UriLiteralParsingException& parsingException)
	public static bool RemoveCustomUriLiteralParser (Microsoft.OData.Core.UriParser.Parsers.Common.IUriLiteralParser customUriLiteralParser)
}

public abstract class Microsoft.OData.Core.UriParser.Semantic.CollectionNode : Microsoft.OData.Core.UriParser.Semantic.QueryNode {
	protected CollectionNode ()

	Microsoft.OData.Edm.IEdmCollectionTypeReference CollectionType  { public abstract get; }
	Microsoft.OData.Edm.IEdmTypeReference ItemType  { public abstract get; }
	Microsoft.OData.Core.UriParser.TreeNodeKinds.QueryNodeKind Kind  { public virtual get; }
}

public abstract class Microsoft.OData.Core.UriParser.Semantic.EntityCollectionNode : Microsoft.OData.Core.UriParser.Semantic.CollectionNode {
	protected EntityCollectionNode ()

	Microsoft.OData.Edm.IEdmEntityTypeReference EntityItemType  { public abstract get; }
	Microsoft.OData.Edm.IEdmNavigationSource NavigationSource  { public abstract get; }
}

public abstract class Microsoft.OData.Core.UriParser.Semantic.LambdaNode : Microsoft.OData.Core.UriParser.Semantic.SingleValueNode {
	protected LambdaNode (System.Collections.ObjectModel.Collection`1[[Microsoft.OData.Core.UriParser.Semantic.RangeVariable]] rangeVariables)
	protected LambdaNode (System.Collections.ObjectModel.Collection`1[[Microsoft.OData.Core.UriParser.Semantic.RangeVariable]] rangeVariables, Microsoft.OData.Core.UriParser.Semantic.RangeVariable currentRangeVariable)

	Microsoft.OData.Core.UriParser.Semantic.SingleValueNode Body  { public get; public set; }
	Microsoft.OData.Core.UriParser.Semantic.RangeVariable CurrentRangeVariable  { public get; }
	System.Collections.ObjectModel.Collection`1[[Microsoft.OData.Core.UriParser.Semantic.RangeVariable]] RangeVariables  { public get; }
	Microsoft.OData.Core.UriParser.Semantic.CollectionNode Source  { public get; public set; }
}

public abstract class Microsoft.OData.Core.UriParser.Semantic.ODataPathSegment : Microsoft.OData.Core.ODataAnnotatable {
	protected ODataPathSegment ()

	Microsoft.OData.Edm.IEdmType EdmType  { public abstract get; }

	internal virtual bool Equals (Microsoft.OData.Core.UriParser.Semantic.ODataPathSegment other)
	public abstract void HandleWith (Microsoft.OData.Core.UriParser.Visitors.PathSegmentHandler handler)
	public abstract T TranslateWith (PathSegmentTranslator`1 translator)
}

public abstract class Microsoft.OData.Core.UriParser.Semantic.QueryNode : Microsoft.OData.Core.ODataAnnotatable {
	protected QueryNode ()

	Microsoft.OData.Core.UriParser.TreeNodeKinds.QueryNodeKind Kind  { public abstract get; }

	public virtual T Accept (QueryNodeVisitor`1 visitor)
}

public abstract class Microsoft.OData.Core.UriParser.Semantic.RangeVariable : Microsoft.OData.Core.ODataAnnotatable {
	protected RangeVariable ()

	int Kind  { public abstract get; }
	string Name  { public abstract get; }
	Microsoft.OData.Edm.IEdmTypeReference TypeReference  { public abstract get; }
}

public abstract class Microsoft.OData.Core.UriParser.Semantic.SelectItem : Microsoft.OData.Core.ODataAnnotatable {
	protected SelectItem ()

	public abstract void HandleWith (Microsoft.OData.Core.UriParser.Visitors.SelectItemHandler handler)
	public abstract T TranslateWith (SelectItemTranslator`1 translator)
}

public abstract class Microsoft.OData.Core.UriParser.Semantic.SingleEntityNode : Microsoft.OData.Core.UriParser.Semantic.SingleValueNode {
	protected SingleEntityNode ()

	Microsoft.OData.Edm.IEdmEntityTypeReference EntityTypeReference  { public abstract get; }
	Microsoft.OData.Edm.IEdmNavigationSource NavigationSource  { public abstract get; }
}

public abstract class Microsoft.OData.Core.UriParser.Semantic.SingleValueNode : Microsoft.OData.Core.UriParser.Semantic.QueryNode {
	protected SingleValueNode ()

	Microsoft.OData.Core.UriParser.TreeNodeKinds.QueryNodeKind Kind  { public virtual get; }
	Microsoft.OData.Edm.IEdmTypeReference TypeReference  { public abstract get; }
}

public sealed class Microsoft.OData.Core.UriParser.Semantic.RangeVariableKind {
	public static int Entity = 0
	public static int Nonentity = 1
}

public class Microsoft.OData.Core.UriParser.Semantic.ExpandedReferenceSelectItem : Microsoft.OData.Core.UriParser.Semantic.SelectItem {
	public ExpandedReferenceSelectItem (Microsoft.OData.Core.UriParser.Semantic.ODataExpandPath pathToNavigationProperty, Microsoft.OData.Edm.IEdmNavigationSource navigationSource)
	public ExpandedReferenceSelectItem (Microsoft.OData.Core.UriParser.Semantic.ODataExpandPath pathToNavigationProperty, Microsoft.OData.Edm.IEdmNavigationSource navigationSource, Microsoft.OData.Core.UriParser.Semantic.FilterClause filterOption, Microsoft.OData.Core.UriParser.Semantic.OrderByClause orderByOption, System.Nullable`1[[System.Int64]] topOption, System.Nullable`1[[System.Int64]] skipOption, System.Nullable`1[[System.Boolean]] countOption, Microsoft.OData.Core.UriParser.Semantic.SearchClause searchOption)

	System.Nullable`1[[System.Boolean]] CountOption  { public get; }
	Microsoft.OData.Core.UriParser.Semantic.FilterClause FilterOption  { public get; }
	Microsoft.OData.Edm.IEdmNavigationSource NavigationSource  { public get; }
	Microsoft.OData.Core.UriParser.Semantic.OrderByClause OrderByOption  { public get; }
	Microsoft.OData.Core.UriParser.Semantic.ODataExpandPath PathToNavigationProperty  { public get; }
	Microsoft.OData.Core.UriParser.Semantic.SearchClause SearchOption  { public get; }
	System.Nullable`1[[System.Int64]] SkipOption  { public get; }
	System.Nullable`1[[System.Int64]] TopOption  { public get; }

	public virtual void HandleWith (Microsoft.OData.Core.UriParser.Visitors.SelectItemHandler handler)
	public virtual T TranslateWith (SelectItemTranslator`1 translator)
}

public class Microsoft.OData.Core.UriParser.Semantic.NamedFunctionParameterNode : Microsoft.OData.Core.UriParser.Semantic.QueryNode {
	public NamedFunctionParameterNode (string name, Microsoft.OData.Core.UriParser.Semantic.QueryNode value)

	Microsoft.OData.Core.UriParser.TreeNodeKinds.QueryNodeKind Kind  { public virtual get; }
	string Name  { public get; }
	Microsoft.OData.Core.UriParser.Semantic.QueryNode Value  { public get; }

	public virtual T Accept (QueryNodeVisitor`1 visitor)
}

public class Microsoft.OData.Core.UriParser.Semantic.ODataExpandPath : Microsoft.OData.Core.UriParser.Semantic.ODataPath, IEnumerable, IEnumerable`1 {
	public ODataExpandPath (Microsoft.OData.Core.UriParser.Semantic.ODataPathSegment[] segments)
	public ODataExpandPath (System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Core.UriParser.Semantic.ODataPathSegment]] segments)
}

public class Microsoft.OData.Core.UriParser.Semantic.ODataPath : Microsoft.OData.Core.ODataAnnotatable, IEnumerable, IEnumerable`1 {
	public ODataPath (Microsoft.OData.Core.UriParser.Semantic.ODataPathSegment[] segments)
	public ODataPath (System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Core.UriParser.Semantic.ODataPathSegment]] segments)

	int Count  { public get; }
	Microsoft.OData.Core.UriParser.Semantic.ODataPathSegment FirstSegment  { public get; }
	Microsoft.OData.Core.UriParser.Semantic.ODataPathSegment LastSegment  { public get; }

	public virtual System.Collections.Generic.IEnumerator`1[[Microsoft.OData.Core.UriParser.Semantic.ODataPathSegment]] GetEnumerator ()
	System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
	public IEnumerable`1 WalkWith (PathSegmentTranslator`1 translator)
	public void WalkWith (Microsoft.OData.Core.UriParser.Visitors.PathSegmentHandler handler)
}

public class Microsoft.OData.Core.UriParser.Semantic.ODataSelectPath : Microsoft.OData.Core.UriParser.Semantic.ODataPath, IEnumerable, IEnumerable`1 {
	public ODataSelectPath (Microsoft.OData.Core.UriParser.Semantic.ODataPathSegment[] segments)
	public ODataSelectPath (System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Core.UriParser.Semantic.ODataPathSegment]] segments)
}

public class Microsoft.OData.Core.UriParser.Semantic.ODataUnresolvedFunctionParameterAlias : Microsoft.OData.Core.ODataValue {
	public ODataUnresolvedFunctionParameterAlias (string alias, Microsoft.OData.Edm.IEdmTypeReference type)

	string Alias  { public get; }
	Microsoft.OData.Edm.IEdmTypeReference Type  { public get; }
}

public class Microsoft.OData.Core.UriParser.Semantic.ParameterAliasNode : Microsoft.OData.Core.UriParser.Semantic.SingleValueNode {
	public ParameterAliasNode (string alias, Microsoft.OData.Edm.IEdmTypeReference typeReference)

	string Alias  { public get; }
	Microsoft.OData.Edm.IEdmTypeReference TypeReference  { public virtual get; }

	public virtual T Accept (QueryNodeVisitor`1 visitor)
}

public sealed class Microsoft.OData.Core.UriParser.Semantic.AllNode : Microsoft.OData.Core.UriParser.Semantic.LambdaNode {
	public AllNode (System.Collections.ObjectModel.Collection`1[[Microsoft.OData.Core.UriParser.Semantic.RangeVariable]] rangeVariables)
	public AllNode (System.Collections.ObjectModel.Collection`1[[Microsoft.OData.Core.UriParser.Semantic.RangeVariable]] rangeVariables, Microsoft.OData.Core.UriParser.Semantic.RangeVariable currentRangeVariable)

	Microsoft.OData.Edm.IEdmTypeReference TypeReference  { public virtual get; }

	public virtual T Accept (QueryNodeVisitor`1 visitor)
}

public sealed class Microsoft.OData.Core.UriParser.Semantic.AnyNode : Microsoft.OData.Core.UriParser.Semantic.LambdaNode {
	public AnyNode (System.Collections.ObjectModel.Collection`1[[Microsoft.OData.Core.UriParser.Semantic.RangeVariable]] parameters)
	public AnyNode (System.Collections.ObjectModel.Collection`1[[Microsoft.OData.Core.UriParser.Semantic.RangeVariable]] parameters, Microsoft.OData.Core.UriParser.Semantic.RangeVariable currentRangeVariable)

	Microsoft.OData.Edm.IEdmTypeReference TypeReference  { public virtual get; }

	public virtual T Accept (QueryNodeVisitor`1 visitor)
}

public sealed class Microsoft.OData.Core.UriParser.Semantic.BatchReferenceSegment : Microsoft.OData.Core.UriParser.Semantic.ODataPathSegment {
	public BatchReferenceSegment (string contentId, Microsoft.OData.Edm.IEdmType edmType, Microsoft.OData.Edm.IEdmEntitySetBase entitySet)

	string ContentId  { public get; }
	Microsoft.OData.Edm.IEdmType EdmType  { public virtual get; }
	Microsoft.OData.Edm.IEdmEntitySetBase EntitySet  { public get; }

	public virtual void HandleWith (Microsoft.OData.Core.UriParser.Visitors.PathSegmentHandler handler)
	public virtual T TranslateWith (PathSegmentTranslator`1 translator)
}

public sealed class Microsoft.OData.Core.UriParser.Semantic.BatchSegment : Microsoft.OData.Core.UriParser.Semantic.ODataPathSegment {
	public static readonly Microsoft.OData.Core.UriParser.Semantic.BatchSegment Instance = Microsoft.OData.Core.UriParser.Semantic.BatchSegment

	Microsoft.OData.Edm.IEdmType EdmType  { public virtual get; }

	public virtual void HandleWith (Microsoft.OData.Core.UriParser.Visitors.PathSegmentHandler handler)
	public virtual T TranslateWith (PathSegmentTranslator`1 translator)
}

public sealed class Microsoft.OData.Core.UriParser.Semantic.BinaryOperatorNode : Microsoft.OData.Core.UriParser.Semantic.SingleValueNode {
	public BinaryOperatorNode (Microsoft.OData.Core.UriParser.TreeNodeKinds.BinaryOperatorKind operatorKind, Microsoft.OData.Core.UriParser.Semantic.SingleValueNode left, Microsoft.OData.Core.UriParser.Semantic.SingleValueNode right)

	Microsoft.OData.Core.UriParser.Semantic.SingleValueNode Left  { public get; }
	Microsoft.OData.Core.UriParser.TreeNodeKinds.BinaryOperatorKind OperatorKind  { public get; }
	Microsoft.OData.Core.UriParser.Semantic.SingleValueNode Right  { public get; }
	Microsoft.OData.Edm.IEdmTypeReference TypeReference  { public virtual get; }

	public virtual T Accept (QueryNodeVisitor`1 visitor)
}

public sealed class Microsoft.OData.Core.UriParser.Semantic.CollectionFunctionCallNode : Microsoft.OData.Core.UriParser.Semantic.CollectionNode {
	public CollectionFunctionCallNode (string name, System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmFunction]] functions, System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Core.UriParser.Semantic.QueryNode]] parameters, Microsoft.OData.Edm.IEdmCollectionTypeReference returnedCollectionType, Microsoft.OData.Core.UriParser.Semantic.QueryNode source)

	Microsoft.OData.Edm.IEdmCollectionTypeReference CollectionType  { public virtual get; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmFunction]] Functions  { public get; }
	Microsoft.OData.Edm.IEdmTypeReference ItemType  { public virtual get; }
	string Name  { public get; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Core.UriParser.Semantic.QueryNode]] Parameters  { public get; }
	Microsoft.OData.Core.UriParser.Semantic.QueryNode Source  { public get; }

	public virtual T Accept (QueryNodeVisitor`1 visitor)
}

public sealed class Microsoft.OData.Core.UriParser.Semantic.CollectionNavigationNode : Microsoft.OData.Core.UriParser.Semantic.EntityCollectionNode {
	public CollectionNavigationNode (Microsoft.OData.Edm.IEdmNavigationProperty navigationProperty, Microsoft.OData.Core.UriParser.Semantic.SingleEntityNode source)
	public CollectionNavigationNode (Microsoft.OData.Edm.IEdmNavigationProperty navigationProperty, Microsoft.OData.Edm.IEdmNavigationSource source)

	Microsoft.OData.Edm.IEdmCollectionTypeReference CollectionType  { public virtual get; }
	Microsoft.OData.Edm.IEdmEntityTypeReference EntityItemType  { public virtual get; }
	Microsoft.OData.Edm.IEdmTypeReference ItemType  { public virtual get; }
	Microsoft.OData.Edm.IEdmNavigationProperty NavigationProperty  { public get; }
	Microsoft.OData.Edm.IEdmNavigationSource NavigationSource  { public virtual get; }
	Microsoft.OData.Core.UriParser.Semantic.SingleValueNode Source  { public get; }
	Microsoft.OData.Edm.EdmMultiplicity TargetMultiplicity  { public get; }

	public virtual T Accept (QueryNodeVisitor`1 visitor)
}

public sealed class Microsoft.OData.Core.UriParser.Semantic.CollectionOpenPropertyAccessNode : Microsoft.OData.Core.UriParser.Semantic.CollectionNode {
	public CollectionOpenPropertyAccessNode (Microsoft.OData.Core.UriParser.Semantic.SingleValueNode source, string openPropertyName)

	Microsoft.OData.Edm.IEdmCollectionTypeReference CollectionType  { public virtual get; }
	Microsoft.OData.Edm.IEdmTypeReference ItemType  { public virtual get; }
	string Name  { public get; }
	Microsoft.OData.Core.UriParser.Semantic.SingleValueNode Source  { public get; }

	public virtual T Accept (QueryNodeVisitor`1 visitor)
}

public sealed class Microsoft.OData.Core.UriParser.Semantic.CollectionPropertyAccessNode : Microsoft.OData.Core.UriParser.Semantic.CollectionNode {
	public CollectionPropertyAccessNode (Microsoft.OData.Core.UriParser.Semantic.SingleValueNode source, Microsoft.OData.Edm.IEdmProperty property)

	Microsoft.OData.Edm.IEdmCollectionTypeReference CollectionType  { public virtual get; }
	Microsoft.OData.Edm.IEdmTypeReference ItemType  { public virtual get; }
	Microsoft.OData.Edm.IEdmProperty Property  { public get; }
	Microsoft.OData.Core.UriParser.Semantic.SingleValueNode Source  { public get; }

	public virtual T Accept (QueryNodeVisitor`1 visitor)
}

public sealed class Microsoft.OData.Core.UriParser.Semantic.CollectionPropertyCastNode : Microsoft.OData.Core.UriParser.Semantic.CollectionNode {
	public CollectionPropertyCastNode (Microsoft.OData.Core.UriParser.Semantic.CollectionPropertyAccessNode source, Microsoft.OData.Edm.IEdmComplexType complexType)

	Microsoft.OData.Edm.IEdmCollectionTypeReference CollectionType  { public virtual get; }
	Microsoft.OData.Edm.IEdmTypeReference ItemType  { public virtual get; }
	Microsoft.OData.Core.UriParser.Semantic.CollectionPropertyAccessNode Source  { public get; }

	public virtual T Accept (QueryNodeVisitor`1 visitor)
}

public sealed class Microsoft.OData.Core.UriParser.Semantic.ConstantNode : Microsoft.OData.Core.UriParser.Semantic.SingleValueNode {
	public ConstantNode (object constantValue)
	public ConstantNode (object constantValue, string literalText)
	public ConstantNode (object constantValue, string literalText, Microsoft.OData.Edm.IEdmTypeReference typeReference)

	string LiteralText  { public get; }
	Microsoft.OData.Edm.IEdmTypeReference TypeReference  { public virtual get; }
	object Value  { public get; }

	public virtual T Accept (QueryNodeVisitor`1 visitor)
}

public sealed class Microsoft.OData.Core.UriParser.Semantic.ConvertNode : Microsoft.OData.Core.UriParser.Semantic.SingleValueNode {
	public ConvertNode (Microsoft.OData.Core.UriParser.Semantic.SingleValueNode source, Microsoft.OData.Edm.IEdmTypeReference typeReference)

	Microsoft.OData.Core.UriParser.Semantic.SingleValueNode Source  { public get; }
	Microsoft.OData.Edm.IEdmTypeReference TypeReference  { public virtual get; }

	public virtual T Accept (QueryNodeVisitor`1 visitor)
}

public sealed class Microsoft.OData.Core.UriParser.Semantic.CountNode : Microsoft.OData.Core.UriParser.Semantic.SingleValueNode {
	public CountNode (Microsoft.OData.Core.UriParser.Semantic.CollectionNode source)

	Microsoft.OData.Core.UriParser.Semantic.CollectionNode Source  { public get; }
	Microsoft.OData.Edm.IEdmTypeReference TypeReference  { public virtual get; }

	public virtual T Accept (QueryNodeVisitor`1 visitor)
}

public sealed class Microsoft.OData.Core.UriParser.Semantic.CountSegment : Microsoft.OData.Core.UriParser.Semantic.ODataPathSegment {
	public static readonly Microsoft.OData.Core.UriParser.Semantic.CountSegment Instance = Microsoft.OData.Core.UriParser.Semantic.CountSegment

	Microsoft.OData.Edm.IEdmType EdmType  { public virtual get; }

	public virtual void HandleWith (Microsoft.OData.Core.UriParser.Visitors.PathSegmentHandler handler)
	public virtual T TranslateWith (PathSegmentTranslator`1 translator)
}

public sealed class Microsoft.OData.Core.UriParser.Semantic.EntityCollectionCastNode : Microsoft.OData.Core.UriParser.Semantic.EntityCollectionNode {
	public EntityCollectionCastNode (Microsoft.OData.Core.UriParser.Semantic.EntityCollectionNode source, Microsoft.OData.Edm.IEdmEntityType entityType)

	Microsoft.OData.Edm.IEdmCollectionTypeReference CollectionType  { public virtual get; }
	Microsoft.OData.Edm.IEdmEntityTypeReference EntityItemType  { public virtual get; }
	Microsoft.OData.Edm.IEdmTypeReference ItemType  { public virtual get; }
	Microsoft.OData.Edm.IEdmNavigationSource NavigationSource  { public virtual get; }
	Microsoft.OData.Core.UriParser.Semantic.EntityCollectionNode Source  { public get; }

	public virtual T Accept (QueryNodeVisitor`1 visitor)
}

public sealed class Microsoft.OData.Core.UriParser.Semantic.EntityCollectionFunctionCallNode : Microsoft.OData.Core.UriParser.Semantic.EntityCollectionNode {
	public EntityCollectionFunctionCallNode (string name, System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmFunction]] functions, System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Core.UriParser.Semantic.QueryNode]] parameters, Microsoft.OData.Edm.IEdmCollectionTypeReference returnedCollectionTypeReference, Microsoft.OData.Edm.IEdmEntitySetBase navigationSource, Microsoft.OData.Core.UriParser.Semantic.QueryNode source)

	Microsoft.OData.Edm.IEdmCollectionTypeReference CollectionType  { public virtual get; }
	Microsoft.OData.Edm.IEdmEntityTypeReference EntityItemType  { public virtual get; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmFunction]] Functions  { public get; }
	Microsoft.OData.Edm.IEdmTypeReference ItemType  { public virtual get; }
	string Name  { public get; }
	Microsoft.OData.Edm.IEdmNavigationSource NavigationSource  { public virtual get; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Core.UriParser.Semantic.QueryNode]] Parameters  { public get; }
	Microsoft.OData.Core.UriParser.Semantic.QueryNode Source  { public get; }

	public virtual T Accept (QueryNodeVisitor`1 visitor)
}

public sealed class Microsoft.OData.Core.UriParser.Semantic.EntityIdSegment {
	System.Uri Id  { public get; }
}

public sealed class Microsoft.OData.Core.UriParser.Semantic.EntityRangeVariable : Microsoft.OData.Core.UriParser.Semantic.RangeVariable {
	public EntityRangeVariable (string name, Microsoft.OData.Edm.IEdmEntityTypeReference entityType, Microsoft.OData.Core.UriParser.Semantic.EntityCollectionNode entityCollectionNode)
	public EntityRangeVariable (string name, Microsoft.OData.Edm.IEdmEntityTypeReference entityType, Microsoft.OData.Edm.IEdmNavigationSource navigationSource)

	Microsoft.OData.Core.UriParser.Semantic.EntityCollectionNode EntityCollectionNode  { public get; }
	Microsoft.OData.Edm.IEdmEntityTypeReference EntityTypeReference  { public get; }
	int Kind  { public virtual get; }
	string Name  { public virtual get; }
	Microsoft.OData.Edm.IEdmNavigationSource NavigationSource  { public get; }
	Microsoft.OData.Edm.IEdmTypeReference TypeReference  { public virtual get; }
}

public sealed class Microsoft.OData.Core.UriParser.Semantic.EntityRangeVariableReferenceNode : Microsoft.OData.Core.UriParser.Semantic.SingleEntityNode {
	public EntityRangeVariableReferenceNode (string name, Microsoft.OData.Core.UriParser.Semantic.EntityRangeVariable rangeVariable)

	Microsoft.OData.Edm.IEdmEntityTypeReference EntityTypeReference  { public virtual get; }
	string Name  { public get; }
	Microsoft.OData.Edm.IEdmNavigationSource NavigationSource  { public virtual get; }
	Microsoft.OData.Core.UriParser.Semantic.EntityRangeVariable RangeVariable  { public get; }
	Microsoft.OData.Edm.IEdmTypeReference TypeReference  { public virtual get; }

	public virtual T Accept (QueryNodeVisitor`1 visitor)
}

public sealed class Microsoft.OData.Core.UriParser.Semantic.EntitySetSegment : Microsoft.OData.Core.UriParser.Semantic.ODataPathSegment {
	public EntitySetSegment (Microsoft.OData.Edm.IEdmEntitySet entitySet)

	Microsoft.OData.Edm.IEdmType EdmType  { public virtual get; }
	Microsoft.OData.Edm.IEdmEntitySet EntitySet  { public get; }

	public virtual void HandleWith (Microsoft.OData.Core.UriParser.Visitors.PathSegmentHandler handler)
	public virtual T TranslateWith (PathSegmentTranslator`1 translator)
}

public sealed class Microsoft.OData.Core.UriParser.Semantic.ExpandedNavigationSelectItem : Microsoft.OData.Core.UriParser.Semantic.ExpandedReferenceSelectItem {
	public ExpandedNavigationSelectItem (Microsoft.OData.Core.UriParser.Semantic.ODataExpandPath pathToNavigationProperty, Microsoft.OData.Edm.IEdmNavigationSource navigationSource, Microsoft.OData.Core.UriParser.Semantic.SelectExpandClause selectExpandOption)
	public ExpandedNavigationSelectItem (Microsoft.OData.Core.UriParser.Semantic.ODataExpandPath pathToNavigationProperty, Microsoft.OData.Edm.IEdmNavigationSource navigationSource, Microsoft.OData.Core.UriParser.Semantic.SelectExpandClause selectAndExpand, Microsoft.OData.Core.UriParser.Semantic.FilterClause filterOption, Microsoft.OData.Core.UriParser.Semantic.OrderByClause orderByOption, System.Nullable`1[[System.Int64]] topOption, System.Nullable`1[[System.Int64]] skipOption, System.Nullable`1[[System.Boolean]] countOption, Microsoft.OData.Core.UriParser.Semantic.SearchClause searchOption, Microsoft.OData.Core.UriParser.Semantic.LevelsClause levelsOption)

	Microsoft.OData.Core.UriParser.Semantic.LevelsClause LevelsOption  { public get; }
	Microsoft.OData.Core.UriParser.Semantic.SelectExpandClause SelectAndExpand  { public get; }

	public virtual void HandleWith (Microsoft.OData.Core.UriParser.Visitors.SelectItemHandler handler)
	public virtual T TranslateWith (SelectItemTranslator`1 translator)
}

public sealed class Microsoft.OData.Core.UriParser.Semantic.FilterClause {
	public FilterClause (Microsoft.OData.Core.UriParser.Semantic.SingleValueNode expression, Microsoft.OData.Core.UriParser.Semantic.RangeVariable rangeVariable)

	Microsoft.OData.Core.UriParser.Semantic.SingleValueNode Expression  { public get; }
	Microsoft.OData.Edm.IEdmTypeReference ItemType  { public get; }
	Microsoft.OData.Core.UriParser.Semantic.RangeVariable RangeVariable  { public get; }
}

public sealed class Microsoft.OData.Core.UriParser.Semantic.KeySegment : Microsoft.OData.Core.UriParser.Semantic.ODataPathSegment {
	public KeySegment (System.Collections.Generic.IEnumerable`1[[System.Collections.Generic.KeyValuePair`2[[System.String],[System.Object]]]] keys, Microsoft.OData.Edm.IEdmEntityType edmType, Microsoft.OData.Edm.IEdmNavigationSource navigationSource)

	Microsoft.OData.Edm.IEdmType EdmType  { public virtual get; }
	System.Collections.Generic.IEnumerable`1[[System.Collections.Generic.KeyValuePair`2[[System.String],[System.Object]]]] Keys  { public get; }
	Microsoft.OData.Edm.IEdmNavigationSource NavigationSource  { public get; }

	public virtual void HandleWith (Microsoft.OData.Core.UriParser.Visitors.PathSegmentHandler handler)
	public virtual T TranslateWith (PathSegmentTranslator`1 translator)
}

public sealed class Microsoft.OData.Core.UriParser.Semantic.LevelsClause {
	public LevelsClause (bool isMaxLevel, long level)

	bool IsMaxLevel  { public get; }
	long Level  { public get; }
}

public sealed class Microsoft.OData.Core.UriParser.Semantic.MetadataSegment : Microsoft.OData.Core.UriParser.Semantic.ODataPathSegment {
	public static readonly Microsoft.OData.Core.UriParser.Semantic.MetadataSegment Instance = Microsoft.OData.Core.UriParser.Semantic.MetadataSegment

	Microsoft.OData.Edm.IEdmType EdmType  { public virtual get; }

	public virtual void HandleWith (Microsoft.OData.Core.UriParser.Visitors.PathSegmentHandler handler)
	public virtual T TranslateWith (PathSegmentTranslator`1 translator)
}

public sealed class Microsoft.OData.Core.UriParser.Semantic.NamespaceQualifiedWildcardSelectItem : Microsoft.OData.Core.UriParser.Semantic.SelectItem {
	public NamespaceQualifiedWildcardSelectItem (string namespaceName)

	string Namespace  { public get; }

	public virtual void HandleWith (Microsoft.OData.Core.UriParser.Visitors.SelectItemHandler handler)
	public virtual T TranslateWith (SelectItemTranslator`1 translator)
}

public sealed class Microsoft.OData.Core.UriParser.Semantic.NavigationPropertyLinkSegment : Microsoft.OData.Core.UriParser.Semantic.ODataPathSegment {
	public NavigationPropertyLinkSegment (Microsoft.OData.Edm.IEdmNavigationProperty navigationProperty, Microsoft.OData.Edm.IEdmNavigationSource navigationSource)

	Microsoft.OData.Edm.IEdmType EdmType  { public virtual get; }
	Microsoft.OData.Edm.IEdmNavigationProperty NavigationProperty  { public get; }
	Microsoft.OData.Edm.IEdmNavigationSource NavigationSource  { public get; }

	public virtual void HandleWith (Microsoft.OData.Core.UriParser.Visitors.PathSegmentHandler handler)
	public virtual T TranslateWith (PathSegmentTranslator`1 translator)
}

public sealed class Microsoft.OData.Core.UriParser.Semantic.NavigationPropertySegment : Microsoft.OData.Core.UriParser.Semantic.ODataPathSegment {
	public NavigationPropertySegment (Microsoft.OData.Edm.IEdmNavigationProperty navigationProperty, Microsoft.OData.Edm.IEdmNavigationSource navigationSource)

	Microsoft.OData.Edm.IEdmType EdmType  { public virtual get; }
	Microsoft.OData.Edm.IEdmNavigationProperty NavigationProperty  { public get; }
	Microsoft.OData.Edm.IEdmNavigationSource NavigationSource  { public get; }

	public virtual void HandleWith (Microsoft.OData.Core.UriParser.Visitors.PathSegmentHandler handler)
	public virtual T TranslateWith (PathSegmentTranslator`1 translator)
}

public sealed class Microsoft.OData.Core.UriParser.Semantic.NonentityRangeVariable : Microsoft.OData.Core.UriParser.Semantic.RangeVariable {
	public NonentityRangeVariable (string name, Microsoft.OData.Edm.IEdmTypeReference typeReference, Microsoft.OData.Core.UriParser.Semantic.CollectionNode collectionNode)

	Microsoft.OData.Core.UriParser.Semantic.CollectionNode CollectionNode  { public get; }
	int Kind  { public virtual get; }
	string Name  { public virtual get; }
	Microsoft.OData.Edm.IEdmTypeReference TypeReference  { public virtual get; }
}

public sealed class Microsoft.OData.Core.UriParser.Semantic.NonentityRangeVariableReferenceNode : Microsoft.OData.Core.UriParser.Semantic.SingleValueNode {
	public NonentityRangeVariableReferenceNode (string name, Microsoft.OData.Core.UriParser.Semantic.NonentityRangeVariable rangeVariable)

	string Name  { public get; }
	Microsoft.OData.Core.UriParser.Semantic.NonentityRangeVariable RangeVariable  { public get; }
	Microsoft.OData.Edm.IEdmTypeReference TypeReference  { public virtual get; }

	public virtual T Accept (QueryNodeVisitor`1 visitor)
}

public sealed class Microsoft.OData.Core.UriParser.Semantic.OpenPropertySegment : Microsoft.OData.Core.UriParser.Semantic.ODataPathSegment {
	public OpenPropertySegment (string propertyName)

	Microsoft.OData.Edm.IEdmType EdmType  { public virtual get; }
	string PropertyName  { public get; }

	public virtual void HandleWith (Microsoft.OData.Core.UriParser.Visitors.PathSegmentHandler handler)
	public virtual T TranslateWith (PathSegmentTranslator`1 translator)
}

public sealed class Microsoft.OData.Core.UriParser.Semantic.OperationImportSegment : Microsoft.OData.Core.UriParser.Semantic.ODataPathSegment {
	public OperationImportSegment (Microsoft.OData.Edm.IEdmOperationImport operationImport, Microsoft.OData.Edm.IEdmEntitySetBase entitySet)
	public OperationImportSegment (System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmOperationImport]] operationImports, Microsoft.OData.Edm.IEdmEntitySetBase entitySet)
	public OperationImportSegment (System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmOperationImport]] operationImports, Microsoft.OData.Edm.IEdmEntitySetBase entitySet, System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Core.UriParser.Semantic.OperationSegmentParameter]] parameters)

	Microsoft.OData.Edm.IEdmType EdmType  { public virtual get; }
	Microsoft.OData.Edm.IEdmEntitySetBase EntitySet  { public get; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmOperationImport]] OperationImports  { public get; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Core.UriParser.Semantic.OperationSegmentParameter]] Parameters  { public get; }

	public virtual void HandleWith (Microsoft.OData.Core.UriParser.Visitors.PathSegmentHandler handler)
	public virtual T TranslateWith (PathSegmentTranslator`1 translator)
}

public sealed class Microsoft.OData.Core.UriParser.Semantic.OperationSegment : Microsoft.OData.Core.UriParser.Semantic.ODataPathSegment {
	public OperationSegment (Microsoft.OData.Edm.IEdmOperation operation, Microsoft.OData.Edm.IEdmEntitySetBase entitySet)
	public OperationSegment (System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmOperation]] operations, Microsoft.OData.Edm.IEdmEntitySetBase entitySet)
	public OperationSegment (System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmOperation]] operations, System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Core.UriParser.Semantic.OperationSegmentParameter]] parameters, Microsoft.OData.Edm.IEdmEntitySetBase entitySet)

	Microsoft.OData.Edm.IEdmType EdmType  { public virtual get; }
	Microsoft.OData.Edm.IEdmEntitySetBase EntitySet  { public get; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmOperation]] Operations  { public get; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Core.UriParser.Semantic.OperationSegmentParameter]] Parameters  { public get; }

	public virtual void HandleWith (Microsoft.OData.Core.UriParser.Visitors.PathSegmentHandler handler)
	public virtual T TranslateWith (PathSegmentTranslator`1 translator)
}

public sealed class Microsoft.OData.Core.UriParser.Semantic.OperationSegmentParameter : Microsoft.OData.Core.ODataAnnotatable {
	public OperationSegmentParameter (string name, object value)

	string Name  { public get; }
	object Value  { public get; }
}

public sealed class Microsoft.OData.Core.UriParser.Semantic.OrderByClause {
	public OrderByClause (Microsoft.OData.Core.UriParser.Semantic.OrderByClause thenBy, Microsoft.OData.Core.UriParser.Semantic.SingleValueNode expression, Microsoft.OData.Core.UriParser.OrderByDirection direction, Microsoft.OData.Core.UriParser.Semantic.RangeVariable rangeVariable)

	Microsoft.OData.Core.UriParser.OrderByDirection Direction  { public get; }
	Microsoft.OData.Core.UriParser.Semantic.SingleValueNode Expression  { public get; }
	Microsoft.OData.Edm.IEdmTypeReference ItemType  { public get; }
	Microsoft.OData.Core.UriParser.Semantic.RangeVariable RangeVariable  { public get; }
	Microsoft.OData.Core.UriParser.Semantic.OrderByClause ThenBy  { public get; }
}

public sealed class Microsoft.OData.Core.UriParser.Semantic.PathSelectItem : Microsoft.OData.Core.UriParser.Semantic.SelectItem {
	public PathSelectItem (Microsoft.OData.Core.UriParser.Semantic.ODataSelectPath selectedPath)

	Microsoft.OData.Core.UriParser.Semantic.ODataSelectPath SelectedPath  { public get; }

	public virtual void HandleWith (Microsoft.OData.Core.UriParser.Visitors.SelectItemHandler handler)
	public virtual T TranslateWith (SelectItemTranslator`1 translator)
}

public sealed class Microsoft.OData.Core.UriParser.Semantic.PathTemplateSegment : Microsoft.OData.Core.UriParser.Semantic.ODataPathSegment {
	public PathTemplateSegment (string literalText)

	Microsoft.OData.Edm.IEdmType EdmType  { public virtual get; }
	string LiteralText  { public get; }

	public virtual void HandleWith (Microsoft.OData.Core.UriParser.Visitors.PathSegmentHandler handler)
	public virtual T TranslateWith (PathSegmentTranslator`1 translator)
}

public sealed class Microsoft.OData.Core.UriParser.Semantic.PropertySegment : Microsoft.OData.Core.UriParser.Semantic.ODataPathSegment {
	public PropertySegment (Microsoft.OData.Edm.IEdmStructuralProperty property)

	Microsoft.OData.Edm.IEdmType EdmType  { public virtual get; }
	Microsoft.OData.Edm.IEdmStructuralProperty Property  { public get; }

	public virtual void HandleWith (Microsoft.OData.Core.UriParser.Visitors.PathSegmentHandler handler)
	public virtual T TranslateWith (PathSegmentTranslator`1 translator)
}

public sealed class Microsoft.OData.Core.UriParser.Semantic.SearchClause {
	public SearchClause (Microsoft.OData.Core.UriParser.Semantic.SingleValueNode expression)

	Microsoft.OData.Core.UriParser.Semantic.SingleValueNode Expression  { public get; }
}

public sealed class Microsoft.OData.Core.UriParser.Semantic.SearchTermNode : Microsoft.OData.Core.UriParser.Semantic.SingleValueNode {
	public SearchTermNode (string text)

	string Text  { public get; }
	Microsoft.OData.Edm.IEdmTypeReference TypeReference  { public virtual get; }

	public virtual T Accept (QueryNodeVisitor`1 visitor)
}

public sealed class Microsoft.OData.Core.UriParser.Semantic.SelectExpandClause {
	public SelectExpandClause (System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Core.UriParser.Semantic.SelectItem]] selectedItems, bool allSelected)

	bool AllSelected  { public get; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Core.UriParser.Semantic.SelectItem]] SelectedItems  { public get; }
}

public sealed class Microsoft.OData.Core.UriParser.Semantic.SingleEntityCastNode : Microsoft.OData.Core.UriParser.Semantic.SingleEntityNode {
	public SingleEntityCastNode (Microsoft.OData.Core.UriParser.Semantic.SingleEntityNode source, Microsoft.OData.Edm.IEdmEntityType entityType)

	Microsoft.OData.Edm.IEdmEntityTypeReference EntityTypeReference  { public virtual get; }
	Microsoft.OData.Edm.IEdmNavigationSource NavigationSource  { public virtual get; }
	Microsoft.OData.Core.UriParser.Semantic.SingleEntityNode Source  { public get; }
	Microsoft.OData.Edm.IEdmTypeReference TypeReference  { public virtual get; }

	public virtual T Accept (QueryNodeVisitor`1 visitor)
}

public sealed class Microsoft.OData.Core.UriParser.Semantic.SingleEntityFunctionCallNode : Microsoft.OData.Core.UriParser.Semantic.SingleEntityNode {
	public SingleEntityFunctionCallNode (string name, System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Core.UriParser.Semantic.QueryNode]] parameters, Microsoft.OData.Edm.IEdmEntityTypeReference returnedEntityTypeReference, Microsoft.OData.Edm.IEdmNavigationSource navigationSource)
	public SingleEntityFunctionCallNode (string name, System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmFunction]] functions, System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Core.UriParser.Semantic.QueryNode]] parameters, Microsoft.OData.Edm.IEdmEntityTypeReference returnedEntityTypeReference, Microsoft.OData.Edm.IEdmNavigationSource navigationSource, Microsoft.OData.Core.UriParser.Semantic.QueryNode source)

	Microsoft.OData.Edm.IEdmEntityTypeReference EntityTypeReference  { public virtual get; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmFunction]] Functions  { public get; }
	string Name  { public get; }
	Microsoft.OData.Edm.IEdmNavigationSource NavigationSource  { public virtual get; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Core.UriParser.Semantic.QueryNode]] Parameters  { public get; }
	Microsoft.OData.Core.UriParser.Semantic.QueryNode Source  { public get; }
	Microsoft.OData.Edm.IEdmTypeReference TypeReference  { public virtual get; }

	public virtual T Accept (QueryNodeVisitor`1 visitor)
}

public sealed class Microsoft.OData.Core.UriParser.Semantic.SingleNavigationNode : Microsoft.OData.Core.UriParser.Semantic.SingleEntityNode {
	public SingleNavigationNode (Microsoft.OData.Edm.IEdmNavigationProperty navigationProperty, Microsoft.OData.Core.UriParser.Semantic.SingleEntityNode source)
	public SingleNavigationNode (Microsoft.OData.Edm.IEdmNavigationProperty navigationProperty, Microsoft.OData.Edm.IEdmNavigationSource sourceNavigationSource)

	Microsoft.OData.Edm.IEdmEntityTypeReference EntityTypeReference  { public virtual get; }
	Microsoft.OData.Edm.IEdmNavigationProperty NavigationProperty  { public get; }
	Microsoft.OData.Edm.IEdmNavigationSource NavigationSource  { public virtual get; }
	Microsoft.OData.Core.UriParser.Semantic.SingleEntityNode Source  { public get; }
	Microsoft.OData.Edm.EdmMultiplicity TargetMultiplicity  { public get; }
	Microsoft.OData.Edm.IEdmTypeReference TypeReference  { public virtual get; }

	public virtual T Accept (QueryNodeVisitor`1 visitor)
}

public sealed class Microsoft.OData.Core.UriParser.Semantic.SingletonSegment : Microsoft.OData.Core.UriParser.Semantic.ODataPathSegment {
	public SingletonSegment (Microsoft.OData.Edm.IEdmSingleton singleton)

	Microsoft.OData.Edm.IEdmType EdmType  { public virtual get; }
	Microsoft.OData.Edm.IEdmSingleton Singleton  { public get; }

	public virtual void HandleWith (Microsoft.OData.Core.UriParser.Visitors.PathSegmentHandler handler)
	public virtual T TranslateWith (PathSegmentTranslator`1 translator)
}

public sealed class Microsoft.OData.Core.UriParser.Semantic.SingleValueCastNode : Microsoft.OData.Core.UriParser.Semantic.SingleValueNode {
	public SingleValueCastNode (Microsoft.OData.Core.UriParser.Semantic.SingleValueNode source, Microsoft.OData.Edm.IEdmComplexType complexType)

	Microsoft.OData.Core.UriParser.Semantic.SingleValueNode Source  { public get; }
	Microsoft.OData.Edm.IEdmTypeReference TypeReference  { public virtual get; }

	public virtual T Accept (QueryNodeVisitor`1 visitor)
}

public sealed class Microsoft.OData.Core.UriParser.Semantic.SingleValueFunctionCallNode : Microsoft.OData.Core.UriParser.Semantic.SingleValueNode {
	public SingleValueFunctionCallNode (string name, System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Core.UriParser.Semantic.QueryNode]] parameters, Microsoft.OData.Edm.IEdmTypeReference returnedTypeReference)
	public SingleValueFunctionCallNode (string name, System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmFunction]] functions, System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Core.UriParser.Semantic.QueryNode]] parameters, Microsoft.OData.Edm.IEdmTypeReference returnedTypeReference, Microsoft.OData.Core.UriParser.Semantic.QueryNode source)

	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Edm.IEdmFunction]] Functions  { public get; }
	string Name  { public get; }
	System.Collections.Generic.IEnumerable`1[[Microsoft.OData.Core.UriParser.Semantic.QueryNode]] Parameters  { public get; }
	Microsoft.OData.Core.UriParser.Semantic.QueryNode Source  { public get; }
	Microsoft.OData.Edm.IEdmTypeReference TypeReference  { public virtual get; }

	public virtual T Accept (QueryNodeVisitor`1 visitor)
}

public sealed class Microsoft.OData.Core.UriParser.Semantic.SingleValueOpenPropertyAccessNode : Microsoft.OData.Core.UriParser.Semantic.SingleValueNode {
	public SingleValueOpenPropertyAccessNode (Microsoft.OData.Core.UriParser.Semantic.SingleValueNode source, string openPropertyName)

	string Name  { public get; }
	Microsoft.OData.Core.UriParser.Semantic.SingleValueNode Source  { public get; }
	Microsoft.OData.Edm.IEdmTypeReference TypeReference  { public virtual get; }

	public virtual T Accept (QueryNodeVisitor`1 visitor)
}

public sealed class Microsoft.OData.Core.UriParser.Semantic.SingleValuePropertyAccessNode : Microsoft.OData.Core.UriParser.Semantic.SingleValueNode {
	public SingleValuePropertyAccessNode (Microsoft.OData.Core.UriParser.Semantic.SingleValueNode source, Microsoft.OData.Edm.IEdmProperty property)

	Microsoft.OData.Edm.IEdmProperty Property  { public get; }
	Microsoft.OData.Core.UriParser.Semantic.SingleValueNode Source  { public get; }
	Microsoft.OData.Edm.IEdmTypeReference TypeReference  { public virtual get; }

	public virtual T Accept (QueryNodeVisitor`1 visitor)
}

public sealed class Microsoft.OData.Core.UriParser.Semantic.TypeSegment : Microsoft.OData.Core.UriParser.Semantic.ODataPathSegment {
	public TypeSegment (Microsoft.OData.Edm.IEdmType edmType, Microsoft.OData.Edm.IEdmNavigationSource navigationSource)

	Microsoft.OData.Edm.IEdmType EdmType  { public virtual get; }
	Microsoft.OData.Edm.IEdmNavigationSource NavigationSource  { public get; }

	public virtual void HandleWith (Microsoft.OData.Core.UriParser.Visitors.PathSegmentHandler handler)
	public virtual T TranslateWith (PathSegmentTranslator`1 translator)
}

public sealed class Microsoft.OData.Core.UriParser.Semantic.UnaryOperatorNode : Microsoft.OData.Core.UriParser.Semantic.SingleValueNode {
	public UnaryOperatorNode (Microsoft.OData.Core.UriParser.TreeNodeKinds.UnaryOperatorKind operatorKind, Microsoft.OData.Core.UriParser.Semantic.SingleValueNode operand)

	Microsoft.OData.Core.UriParser.Semantic.SingleValueNode Operand  { public get; }
	Microsoft.OData.Core.UriParser.TreeNodeKinds.UnaryOperatorKind OperatorKind  { public get; }
	Microsoft.OData.Edm.IEdmTypeReference TypeReference  { public virtual get; }

	public virtual T Accept (QueryNodeVisitor`1 visitor)
}

public sealed class Microsoft.OData.Core.UriParser.Semantic.UriTemplateExpression {
	public UriTemplateExpression ()

	Microsoft.OData.Edm.IEdmTypeReference ExpectedType  { public get; }
	string LiteralText  { public get; }
}

public sealed class Microsoft.OData.Core.UriParser.Semantic.ValueSegment : Microsoft.OData.Core.UriParser.Semantic.ODataPathSegment {
	public ValueSegment (Microsoft.OData.Edm.IEdmType previousType)

	Microsoft.OData.Edm.IEdmType EdmType  { public virtual get; }

	public virtual void HandleWith (Microsoft.OData.Core.UriParser.Visitors.PathSegmentHandler handler)
	public virtual T TranslateWith (PathSegmentTranslator`1 translator)
}

public sealed class Microsoft.OData.Core.UriParser.Semantic.WildcardSelectItem : Microsoft.OData.Core.UriParser.Semantic.SelectItem {
	public WildcardSelectItem ()

	public virtual void HandleWith (Microsoft.OData.Core.UriParser.Visitors.SelectItemHandler handler)
	public virtual T TranslateWith (SelectItemTranslator`1 translator)
}

public enum Microsoft.OData.Core.UriParser.TreeNodeKinds.BinaryOperatorKind : int {
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

public enum Microsoft.OData.Core.UriParser.TreeNodeKinds.QueryNodeKind : int {
	All = 14
	Any = 9
	BinaryOperator = 4
	CollectionFunctionCall = 18
	CollectionNavigationNode = 10
	CollectionOpenPropertyAccess = 25
	CollectionPropertyAccess = 7
	CollectionPropertyCast = 26
	Constant = 1
	Convert = 2
	EntityCollectionCast = 15
	EntityCollectionFunctionCall = 19
	EntityRangeVariableReference = 16
	EntitySet = 22
	KeyLookup = 23
	NamedFunctionParameter = 20
	None = 0
	NonentityRangeVariableReference = 3
	ParameterAlias = 21
	SearchTerm = 24
	SingleEntityCast = 13
	SingleEntityFunctionCall = 17
	SingleNavigationNode = 11
	SingleValueCast = 27
	SingleValueFunctionCall = 8
	SingleValueOpenPropertyAccess = 12
	SingleValuePropertyAccess = 6
	UnaryOperator = 5
}

public enum Microsoft.OData.Core.UriParser.TreeNodeKinds.UnaryOperatorKind : int {
	Negate = 0
	Not = 1
}

public abstract class Microsoft.OData.Core.UriParser.Visitors.PathSegmentHandler {
	protected PathSegmentHandler ()

	public virtual void Handle (Microsoft.OData.Core.UriParser.Semantic.BatchReferenceSegment segment)
	public virtual void Handle (Microsoft.OData.Core.UriParser.Semantic.BatchSegment segment)
	public virtual void Handle (Microsoft.OData.Core.UriParser.Semantic.CountSegment segment)
	public virtual void Handle (Microsoft.OData.Core.UriParser.Semantic.EntitySetSegment segment)
	public virtual void Handle (Microsoft.OData.Core.UriParser.Semantic.KeySegment segment)
	public virtual void Handle (Microsoft.OData.Core.UriParser.Semantic.MetadataSegment segment)
	public virtual void Handle (Microsoft.OData.Core.UriParser.Semantic.NavigationPropertyLinkSegment segment)
	public virtual void Handle (Microsoft.OData.Core.UriParser.Semantic.NavigationPropertySegment segment)
	public virtual void Handle (Microsoft.OData.Core.UriParser.Semantic.OpenPropertySegment segment)
	public virtual void Handle (Microsoft.OData.Core.UriParser.Semantic.OperationImportSegment segment)
	public virtual void Handle (Microsoft.OData.Core.UriParser.Semantic.OperationSegment segment)
	public virtual void Handle (Microsoft.OData.Core.UriParser.Semantic.PathTemplateSegment segment)
	public virtual void Handle (Microsoft.OData.Core.UriParser.Semantic.PropertySegment segment)
	public virtual void Handle (Microsoft.OData.Core.UriParser.Semantic.SingletonSegment segment)
	public virtual void Handle (Microsoft.OData.Core.UriParser.Semantic.TypeSegment segment)
	public virtual void Handle (Microsoft.OData.Core.UriParser.Semantic.ValueSegment segment)
}

public abstract class Microsoft.OData.Core.UriParser.Visitors.PathSegmentTranslator`1 {
	protected PathSegmentTranslator`1 ()

	public virtual T Translate (Microsoft.OData.Core.UriParser.Semantic.BatchReferenceSegment segment)
	public virtual T Translate (Microsoft.OData.Core.UriParser.Semantic.BatchSegment segment)
	public virtual T Translate (Microsoft.OData.Core.UriParser.Semantic.CountSegment segment)
	public virtual T Translate (Microsoft.OData.Core.UriParser.Semantic.EntitySetSegment segment)
	public virtual T Translate (Microsoft.OData.Core.UriParser.Semantic.KeySegment segment)
	public virtual T Translate (Microsoft.OData.Core.UriParser.Semantic.MetadataSegment segment)
	public virtual T Translate (Microsoft.OData.Core.UriParser.Semantic.NavigationPropertyLinkSegment segment)
	public virtual T Translate (Microsoft.OData.Core.UriParser.Semantic.NavigationPropertySegment segment)
	public virtual T Translate (Microsoft.OData.Core.UriParser.Semantic.OpenPropertySegment segment)
	public virtual T Translate (Microsoft.OData.Core.UriParser.Semantic.OperationImportSegment segment)
	public virtual T Translate (Microsoft.OData.Core.UriParser.Semantic.OperationSegment segment)
	public virtual T Translate (Microsoft.OData.Core.UriParser.Semantic.PathTemplateSegment segment)
	public virtual T Translate (Microsoft.OData.Core.UriParser.Semantic.PropertySegment segment)
	public virtual T Translate (Microsoft.OData.Core.UriParser.Semantic.SingletonSegment segment)
	public virtual T Translate (Microsoft.OData.Core.UriParser.Semantic.TypeSegment segment)
	public virtual T Translate (Microsoft.OData.Core.UriParser.Semantic.ValueSegment segment)
}

public abstract class Microsoft.OData.Core.UriParser.Visitors.QueryNodeVisitor`1 {
	protected QueryNodeVisitor`1 ()

	public virtual T Visit (Microsoft.OData.Core.UriParser.Semantic.AllNode nodeIn)
	public virtual T Visit (Microsoft.OData.Core.UriParser.Semantic.AnyNode nodeIn)
	public virtual T Visit (Microsoft.OData.Core.UriParser.Semantic.BinaryOperatorNode nodeIn)
	public virtual T Visit (Microsoft.OData.Core.UriParser.Semantic.CollectionFunctionCallNode nodeIn)
	public virtual T Visit (Microsoft.OData.Core.UriParser.Semantic.CollectionNavigationNode nodeIn)
	public virtual T Visit (Microsoft.OData.Core.UriParser.Semantic.CollectionOpenPropertyAccessNode nodeIn)
	public virtual T Visit (Microsoft.OData.Core.UriParser.Semantic.CollectionPropertyAccessNode nodeIn)
	public virtual T Visit (Microsoft.OData.Core.UriParser.Semantic.CollectionPropertyCastNode nodeIn)
	public virtual T Visit (Microsoft.OData.Core.UriParser.Semantic.ConstantNode nodeIn)
	public virtual T Visit (Microsoft.OData.Core.UriParser.Semantic.ConvertNode nodeIn)
	public virtual T Visit (Microsoft.OData.Core.UriParser.Semantic.CountNode nodeIn)
	public virtual T Visit (Microsoft.OData.Core.UriParser.Semantic.EntityCollectionCastNode nodeIn)
	public virtual T Visit (Microsoft.OData.Core.UriParser.Semantic.EntityCollectionFunctionCallNode nodeIn)
	public virtual T Visit (Microsoft.OData.Core.UriParser.Semantic.EntityRangeVariableReferenceNode nodeIn)
	public virtual T Visit (Microsoft.OData.Core.UriParser.Semantic.NamedFunctionParameterNode nodeIn)
	public virtual T Visit (Microsoft.OData.Core.UriParser.Semantic.NonentityRangeVariableReferenceNode nodeIn)
	public virtual T Visit (Microsoft.OData.Core.UriParser.Semantic.ParameterAliasNode nodeIn)
	public virtual T Visit (Microsoft.OData.Core.UriParser.Semantic.SearchTermNode nodeIn)
	public virtual T Visit (Microsoft.OData.Core.UriParser.Semantic.SingleEntityCastNode nodeIn)
	public virtual T Visit (Microsoft.OData.Core.UriParser.Semantic.SingleEntityFunctionCallNode nodeIn)
	public virtual T Visit (Microsoft.OData.Core.UriParser.Semantic.SingleNavigationNode nodeIn)
	public virtual T Visit (Microsoft.OData.Core.UriParser.Semantic.SingleValueCastNode nodeIn)
	public virtual T Visit (Microsoft.OData.Core.UriParser.Semantic.SingleValueFunctionCallNode nodeIn)
	public virtual T Visit (Microsoft.OData.Core.UriParser.Semantic.SingleValueOpenPropertyAccessNode nodeIn)
	public virtual T Visit (Microsoft.OData.Core.UriParser.Semantic.SingleValuePropertyAccessNode nodeIn)
	public virtual T Visit (Microsoft.OData.Core.UriParser.Semantic.UnaryOperatorNode nodeIn)
}

public abstract class Microsoft.OData.Core.UriParser.Visitors.SelectItemHandler {
	protected SelectItemHandler ()

	public virtual void Handle (Microsoft.OData.Core.UriParser.Semantic.ExpandedNavigationSelectItem item)
	public virtual void Handle (Microsoft.OData.Core.UriParser.Semantic.ExpandedReferenceSelectItem item)
	public virtual void Handle (Microsoft.OData.Core.UriParser.Semantic.NamespaceQualifiedWildcardSelectItem item)
	public virtual void Handle (Microsoft.OData.Core.UriParser.Semantic.PathSelectItem item)
	public virtual void Handle (Microsoft.OData.Core.UriParser.Semantic.WildcardSelectItem item)
}

public abstract class Microsoft.OData.Core.UriParser.Visitors.SelectItemTranslator`1 {
	protected SelectItemTranslator`1 ()

	public virtual T Translate (Microsoft.OData.Core.UriParser.Semantic.ExpandedNavigationSelectItem item)
	public virtual T Translate (Microsoft.OData.Core.UriParser.Semantic.ExpandedReferenceSelectItem item)
	public virtual T Translate (Microsoft.OData.Core.UriParser.Semantic.NamespaceQualifiedWildcardSelectItem item)
	public virtual T Translate (Microsoft.OData.Core.UriParser.Semantic.PathSelectItem item)
	public virtual T Translate (Microsoft.OData.Core.UriParser.Semantic.WildcardSelectItem item)
}

public interface Microsoft.OData.Core.UriParser.Parsers.Common.IUriLiteralParser {
	object ParseUriStringToType (string text, Microsoft.OData.Edm.IEdmTypeReference targetType, out Microsoft.OData.Core.UriParser.Parsers.Common.UriLiteralParsingException& parsingException)
}

[
DebuggerDisplayAttribute(),
]
public sealed class Microsoft.OData.Core.UriParser.Parsers.Common.UriLiteralParsingException : Microsoft.OData.Core.ODataException, _Exception, ISerializable {
	public UriLiteralParsingException ()
	public UriLiteralParsingException (string message)
	public UriLiteralParsingException (string message, System.Exception innerException)
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
	public abstract Microsoft.OData.Core.IODataResponseMessage EndGetResponse (System.IAsyncResult asyncResult)
	public abstract string GetHeader (string headerName)
	public abstract Microsoft.OData.Core.IODataResponseMessage GetResponse ()
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
	public Microsoft.OData.Client.DataServiceClientRequestPipelineConfiguration OnNavigationLinkEnding (System.Action`1[[Microsoft.OData.Client.WritingNavigationLinkArgs]] action)
	public Microsoft.OData.Client.DataServiceClientRequestPipelineConfiguration OnNavigationLinkStarting (System.Action`1[[Microsoft.OData.Client.WritingNavigationLinkArgs]] action)
}

public class Microsoft.OData.Client.DataServiceClientResponsePipelineConfiguration {
	public Microsoft.OData.Client.DataServiceClientResponsePipelineConfiguration OnEntityMaterialized (System.Action`1[[Microsoft.OData.Client.MaterializedEntityArgs]] action)
	public Microsoft.OData.Client.DataServiceClientResponsePipelineConfiguration OnEntryEnded (System.Action`1[[Microsoft.OData.Client.ReadingEntryArgs]] action)
	public Microsoft.OData.Client.DataServiceClientResponsePipelineConfiguration OnEntryStarted (System.Action`1[[Microsoft.OData.Client.ReadingEntryArgs]] action)
	public Microsoft.OData.Client.DataServiceClientResponsePipelineConfiguration OnFeedEnded (System.Action`1[[Microsoft.OData.Client.ReadingFeedArgs]] action)
	public Microsoft.OData.Client.DataServiceClientResponsePipelineConfiguration OnFeedStarted (System.Action`1[[Microsoft.OData.Client.ReadingFeedArgs]] action)
	public Microsoft.OData.Client.DataServiceClientResponsePipelineConfiguration OnMessageReaderSettingsCreated (System.Action`1[[Microsoft.OData.Client.MessageReaderSettingsArgs]] messageReaderSettingsAction)
	public Microsoft.OData.Client.DataServiceClientResponsePipelineConfiguration OnNavigationLinkEnded (System.Action`1[[Microsoft.OData.Client.ReadingNavigationLinkArgs]] action)
	public Microsoft.OData.Client.DataServiceClientResponsePipelineConfiguration OnNavigationLinkStarted (System.Action`1[[Microsoft.OData.Client.ReadingNavigationLinkArgs]] action)
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
	System.Collections.ObjectModel.ReadOnlyCollection`1[[Microsoft.OData.Client.EntityDescriptor]] Entities  { public get; }
	Microsoft.OData.Client.EntityParameterSendOption EntityParameterSendOption  { public get; public set; }
	Microsoft.OData.Client.EntityTracker EntityTracker  { public get; public set; }
	Microsoft.OData.Client.DataServiceClientFormat Format  { public get; }
	bool IgnoreMissingProperties  { public get; public set; }
	bool IgnoreResourceNotFoundException  { public get; public set; }
	System.Collections.ObjectModel.ReadOnlyCollection`1[[Microsoft.OData.Client.LinkDescriptor]] Links  { public get; }
	Microsoft.OData.Client.ODataProtocolVersion MaxProtocolVersion  { public get; }
	Microsoft.OData.Client.MergeOption MergeOption  { public get; public set; }
	bool ODataSimplified  { public get; public set; }
	System.Func`2[[System.String],[System.Uri]] ResolveEntitySet  { public get; public set; }
	System.Func`2[[System.Type],[System.String]] ResolveName  { public get; public set; }
	System.Func`2[[System.String],[System.Type]] ResolveType  { public get; public set; }
	Microsoft.OData.Client.SaveChangesOptions SaveChangesDefaultOptions  { public get; public set; }
	int Timeout  { public get; public set; }
	Microsoft.OData.Client.DataServiceUrlConventions UrlConventions  { public get; public set; }
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
	internal virtual Microsoft.OData.Edm.IEdmVocabularyAnnotatable GetEdmOperationOrOperationImport (System.Reflection.MethodInfo methodInfo)
	public Microsoft.OData.Client.EntityDescriptor GetEntityDescriptor (object entity)
	internal virtual Microsoft.OData.Client.ODataEntityMetadataBuilder GetEntityMetadataBuilder (string entitySetName, Microsoft.OData.Edm.Values.IEdmStructuredValue entityInstance)
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
	public DataServiceTransportException (Microsoft.OData.Core.IODataResponseMessage response, System.Exception innerException)

	Microsoft.OData.Core.IODataResponseMessage Response  { public get; }
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
	public virtual Microsoft.OData.Core.IODataResponseMessage EndGetResponse (System.IAsyncResult asyncResult)
	public virtual string GetHeader (string headerName)
	public virtual Microsoft.OData.Core.IODataResponseMessage GetResponse ()
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
	public MessageReaderSettingsArgs (Microsoft.OData.Core.ODataMessageReaderSettingsBase settings)

	Microsoft.OData.Core.ODataMessageReaderSettingsBase Settings  { public get; }
}

public class Microsoft.OData.Client.MessageWriterSettingsArgs {
	public MessageWriterSettingsArgs (Microsoft.OData.Core.ODataMessageWriterSettingsBase settings)

	Microsoft.OData.Core.ODataMessageWriterSettingsBase Settings  { public get; }
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
	public ReceivingResponseEventArgs (Microsoft.OData.Core.IODataResponseMessage responseMessage, Microsoft.OData.Client.Descriptor descriptor)
	public ReceivingResponseEventArgs (Microsoft.OData.Core.IODataResponseMessage responseMessage, Microsoft.OData.Client.Descriptor descriptor, bool isBatchPart)

	Microsoft.OData.Client.Descriptor Descriptor  { public get; }
	bool IsBatchPart  { public get; }
	Microsoft.OData.Core.IODataResponseMessage ResponseMessage  { public get; }
}

public class Microsoft.OData.Client.SendingRequest2EventArgs : System.EventArgs {
	Microsoft.OData.Client.Descriptor Descriptor  { public get; }
	bool IsBatchPart  { public get; }
	Microsoft.OData.Core.IODataRequestMessage RequestMessage  { public get; }
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
	Microsoft.OData.Core.ODataFormat ODataFormat  { public get; }

	[
	ObsoleteAttribute(),
	]
	public void UseAtom ()

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

public sealed class Microsoft.OData.Client.DataServiceUrlConventions {
	Microsoft.OData.Client.DataServiceUrlConventions Default  { public static get; }
	Microsoft.OData.Client.DataServiceUrlConventions KeyAsSegment  { public static get; }
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
	public MaterializedEntityArgs (Microsoft.OData.Core.ODataEntry entry, object entity)

	object Entity  { public get; }
	Microsoft.OData.Core.ODataEntry Entry  { public get; }
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
	public ReadingEntryArgs (Microsoft.OData.Core.ODataEntry entry)

	Microsoft.OData.Core.ODataEntry Entry  { public get; }
}

public sealed class Microsoft.OData.Client.ReadingFeedArgs {
	public ReadingFeedArgs (Microsoft.OData.Core.ODataFeed feed)

	Microsoft.OData.Core.ODataFeed Feed  { public get; }
}

public sealed class Microsoft.OData.Client.ReadingNavigationLinkArgs {
	public ReadingNavigationLinkArgs (Microsoft.OData.Core.ODataNavigationLink link)

	Microsoft.OData.Core.ODataNavigationLink Link  { public get; }
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
	public WritingEntityReferenceLinkArgs (Microsoft.OData.Core.ODataEntityReferenceLink entityReferenceLink, object source, object target)

	Microsoft.OData.Core.ODataEntityReferenceLink EntityReferenceLink  { public get; }
	object Source  { public get; }
	object Target  { public get; }
}

public sealed class Microsoft.OData.Client.WritingEntryArgs {
	public WritingEntryArgs (Microsoft.OData.Core.ODataEntry entry, object entity)

	object Entity  { public get; }
	Microsoft.OData.Core.ODataEntry Entry  { public get; }
}

public sealed class Microsoft.OData.Client.WritingNavigationLinkArgs {
	public WritingNavigationLinkArgs (Microsoft.OData.Core.ODataNavigationLink link, object source, object target)

	Microsoft.OData.Core.ODataNavigationLink Link  { public get; }
	object Source  { public get; }
	object Target  { public get; }
}

