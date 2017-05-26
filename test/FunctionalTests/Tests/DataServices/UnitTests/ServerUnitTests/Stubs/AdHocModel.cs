//---------------------------------------------------------------------
// <copyright file="AdHocModel.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Stubs
{
    #region Namespaces

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Test.Astoria;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Xml;
    using Microsoft.OData.Client;

    #endregion Namespaces

    [DebuggerDisplay("AssociationType: {Name}")]
    public class AdHocAssociationType
    {
        public string Name { get; set; }
        public List<AdHocAssociationTypeEnd> Ends { get; set; }

        /// <summary>The many side on one-to-many associations.</summary>
        public AdHocAssociationTypeEnd ManyEnd
        {
            get
            {
                foreach (AdHocAssociationTypeEnd end in this.Ends) if (end.Multiplicity == "*") return end;
                return null;
            }
        }

        /// <summary>The one side on one-to-many associations.</summary>
        public AdHocAssociationTypeEnd OneEnd
        {
            get
            {
                foreach (AdHocAssociationTypeEnd end in this.Ends) if (end.Multiplicity == "1") return end;
                return null;
            }
        }

        public AdHocAssociationTypeEnd OtherEnd(AdHocAssociationTypeEnd end)
        {
            if (end == Ends[0]) return Ends[1];
            return Ends[0];
        }

        /// <summary>Adds navigation properties to the type for both ends.</summary>
        public void AddNavigationProperties()
        {
            Ends[0].Type.Properties.Add(new AdHocNavigationProperty()
            {
                Name = Ends[1].RoleName,
                AssociationType = this,
                SourceEnd = Ends[0],
                Type = Ends[1].Type
            });
            Ends[1].Type.Properties.Add(new AdHocNavigationProperty()
            {
                Name = Ends[0].RoleName,
                AssociationType = this,
                SourceEnd = Ends[1],
                Type = Ends[0].Type
            });
        }
    }

    [DebuggerDisplay("AssociationSet: {Name}")]
    public class AdHocAssociationSet
    {
        public AdHocAssociationSet() { this.Ends = new List<AdHocAssociationSetEnd>(); }
        public string Name { get; set; }
        public AdHocAssociationType Type { get; set; }
        public List<AdHocAssociationSetEnd> Ends { get; set; }

        /// <summary>The many side on one-to-many associations.</summary>
        public AdHocAssociationSetEnd ManyEnd
        {
            get
            {
                foreach (AdHocAssociationSetEnd end in this.Ends) if (end.EndType.Multiplicity == "*") return end;
                return null;
            }
        }

        /// <summary>The one side on one-to-many associations.</summary>
        public AdHocAssociationSetEnd OneEnd
        {
            get
            {
                foreach (AdHocAssociationSetEnd end in this.Ends) if (end.EndType.Multiplicity == "1") return end;
                return null;
            }
        }
    }

    [DebuggerDisplay("AssociationTypeEnd {RoleName}:{Type} {Multiplicity}")]
    public class AdHocAssociationTypeEnd
    {
        public string RoleName { get; set; }
        public AdHocEntityType Type { get; set; }
        public string Multiplicity { get; set; }
    }

    public class AdHocAssociationSetEnd
    {
        public AdHocAssociationTypeEnd EndType { get; set; }
        public AdHocEntitySet EntitySet { get; set; }
    }

    public abstract class AdHocStructuralType : AdHocType
    {
        public AdHocStructuralType()
        {
            this.Namespace = "Ns";
        }

        public List<AdHocProperty> Properties { get; set; }
        public override string StorageName { get { return this.Name; } }
    }

    public class AdHocComplexType : AdHocStructuralType
    {
        public AdHocComplexType BaseType { get; set; }
        public override string StorageName { get { return this.Name; } }
    }

    public class AdHocEntityType : AdHocStructuralType
    {
        private static int typeID = 0;

        public AdHocEntityType(AdHocEntityType baseType)
        {
            this.BaseType = baseType;
            this.Name = "T" + ++typeID;
            this.Properties = new List<AdHocProperty>() { new AdHocScalarProperty() { Name = "PropertyOn" + this.Name, Type = new AdHocPrimitiveType(), IsNullable = true } };
            this.DerivedTypes = new List<AdHocEntityType>();
            this.BaseType.DerivedTypes.Add(this);
        }

        public AdHocEntityType() : this("T" + ++typeID)
        {
        }

        public AdHocEntityType(string name)
        {
            this.Name = name;

            this.Properties = new List<AdHocProperty>();
            this.Properties.Add(new AdHocScalarProperty() { Name = name + "ID", Type = new AdHocPrimitiveType() });
            this.Properties.Add(new AdHocScalarProperty() { Name = name + "Data", Type = new AdHocPrimitiveType() });

            this.KeyProperties = new List<AdHocProperty>() { this.Properties[0] };
            this.DerivedTypes = new List<AdHocEntityType>();
        }

        public IEnumerable<AdHocEntityType> SelfAndBaseTypes
        {
            get { AdHocEntityType t = this; while (t != null) { yield return t; t = t.BaseType; } }
        }

        public IEnumerable<AdHocEntityType> SelfAndDerivedTypes
        {
            get { yield return this; foreach (AdHocEntityType type in this.DerivedTypes.SelectMany((t) => t.SelfAndDerivedTypes)) yield return type; }
        }

        public IEnumerable<AdHocProperty> CalculatedKeyProperties
        {
            get { foreach (var t in this.SelfAndBaseTypes) if (t.KeyProperties != null && t.KeyProperties.Count > 0) return t.KeyProperties; return null; }
        }

        public AdHocEntityType BaseType { get; set; }
        public List<AdHocEntityType> DerivedTypes { get; set; }
        public List<AdHocProperty> KeyProperties { get; set; }
        public List<AdHocEPMInfo> EPMInfo { get; set; }
    }

    public class AdHocPrimitiveType : AdHocType
    {
        private static Dictionary<Type, string> map = TestUtil.CreateDictionary(
            new KeyValuePair<Type, string>(typeof(int), "int"),
            new KeyValuePair<Type, string>(typeof(Guid), "uniqueidentifier"),
            new KeyValuePair<Type, string>(typeof(string), "nvarchar"));
        private Type type;

        public AdHocPrimitiveType() : this(typeof(string)) { }
        public AdHocPrimitiveType(Type type) { this.ClrType = type; }
        public Type ClrType { get { return this.type; } set { this.type = value; this.Name = value.Name; this.Namespace = value.Namespace; } }
        public override string StorageName { get { return map[type]; } }
    }

    [DebuggerDisplay("EntitySet: {Name}")]
    public class AdHocEntitySet
    {
        public string Name { get; set; }
        public AdHocEntityType Type { get; set; }
    }

    [DebuggerDisplay("Type: {Name}")]
    public abstract class AdHocType
    {
        private string name;

        /// <summary>Full name of type, namespace-qualified as necessary.</summary>
        public string FullName
        {
            get
            {
                if (String.IsNullOrEmpty(this.Namespace))
                {
                    return this.Name;
                }
                else
                {
                    return this.Namespace + "." + this.Name;
                }
            }
        }

        public string Name
        {
            [DebuggerStepThrough]
            get { return this.name; }

            set
            {
                if (value == null || value.IndexOf('.') == -1)
                {
                    this.name = value;
                }
                else
                {
                    this.Namespace = value.Substring(0, value.LastIndexOf('.'));
                    this.name = value.Substring(value.LastIndexOf('.') + 1);
                }
            }
        }

        public string Namespace { get; set; }
        public abstract string StorageName { get; }
    }

    [DebuggerDisplay("Property: {Name}")]
    public class AdHocProperty
    {
        public string Name { get; set; }
        public AdHocType Type { get; set; }
        public AdHocEPMInfo EPMInfo { get; set; }
    }

    public class AdHocScalarProperty : AdHocProperty
    {
        public bool IsNullable { get; set; }
    }

    public class AdHocNavigationProperty : AdHocProperty
    {
        public AdHocAssociationType AssociationType;
        public AdHocAssociationTypeEnd SourceEnd;
    }

    public class AdHocEPMInfo
    {
        // KeepInContent is object to allow negative tests
        public object KeepInContent { get; set; }
        public string SourcePath { get; set; }
        public string TargetPath { get; set; }

        //atom is true
        //public AtomSyndicationItemProperty SyndicationItem { get; set; }
        //public AtomSyndicationTextContentKind? ContentKind { get; set; }
        //public String CriteriaValue { get; set; }

        //atom is false
        public string NsPrefix { get; set; }
        public string NsUri { get; set; }
    }

    [DebuggerDisplay("Lexer {text} {CurrentKind} : {CurrentValue}")]
    public class AdHocLexer<T>
    {
        private int offset;
        private char currentChar;
        private T currentKind;
        private bool nextCalled;
        private Match currentMatch;
        private readonly string text;
        private readonly IDictionary<char, T> symbols;
        private readonly IEnumerable<KeyValuePair<Regex, T>> tokens;
        private readonly bool whitespaceSignificant;

        public AdHocLexer(string text, IDictionary<char, T> symbols, IEnumerable<KeyValuePair<Regex, T>> tokens)
            : this(text, symbols, tokens, false)
        {
        }

        public AdHocLexer(string text, IDictionary<char, T> symbols, IEnumerable<KeyValuePair<Regex, T>> tokens, bool whitespaceSignificant)
        {
            this.currentKind = default(T);
            this.symbols = symbols;
            this.text = text;
            this.tokens = tokens;
            this.whitespaceSignificant = whitespaceSignificant;
        }

        public void CheckCurrentKind(T expectedKind, string message)
        {
            if (!this.CurrentKind.Equals(expectedKind))
            {
                throw Error(message);
            }
        }

        public string ReadDottedIdentifier(T identifierToken, T dotToken, char dotChar)
        {
            if (!this.CurrentKind.Equals(identifierToken))
            {
                throw Error("Identifier token expected.");
            }

            StringBuilder result = new StringBuilder(this.CurrentValue);
            while (true)
            {
                if (!this.ReadNext())
                {
                    break;
                }

                if (!this.CurrentKind.Equals(dotToken))
                {
                    break;
                }

                result.Append(dotChar);
                if (!this.ReadNextOrThrow().Equals(identifierToken))
                {
                    throw Error("Identifier token expected.");
                }

                result.Append(this.CurrentValue);
            }

            return result.ToString();
        }

        public bool ReadNext()
        {
            this.currentMatch = null;

            if (!nextCalled)
            {
                this.offset = -1;
                this.ReadNextChar();
                nextCalled = true;
            }

            if (!this.whitespaceSignificant)
            {
                while (Char.IsWhiteSpace(this.currentChar))
                {
                    this.ReadNextChar();
                }
            }

            if (this.offset == this.text.Length)
            {
                return false;
            }

            T value;
            if (symbols.TryGetValue(this.currentChar, out value))
            {
                this.currentKind = value;
                this.ReadNextChar();
                return true;
            }

            foreach (var pair in this.tokens)
            {
                Match match = pair.Key.Match(this.text, this.offset);
                if (match.Success && match.Index == this.offset)
                {
                    this.currentMatch = match;
                    this.currentKind = pair.Value;
                    this.offset = match.Index + match.Length - 1;
                    this.ReadNextChar();
                    return true;
                }
            }

            throw new Exception("No token or symbol found at " + this.text.Substring(this.offset));
        }

        public void ReadNextKindOrThrow(T kind, string message)
        {
            this.ReadNextOrThrow();
            this.CheckCurrentKind(kind, message);
        }

        public T ReadNextOrThrow()
        {
            if (!this.ReadNext())
            {
                throw this.Error("Unexpected end of text.");
            }

            return this.CurrentKind;
        }

        public Match CurrentMatch
        {
            get { return this.currentMatch; }
        }

        public T CurrentKind
        {
            get { return this.currentKind; }
        }

        public string CurrentValue
        {
            get
            {
                if (this.offset >= this.text.Length)
                {
                    return "<eof>";
                }

                if (this.CurrentMatch != null)
                {
                    return this.CurrentMatch.Value;
                }

                var q = this.symbols.Where((x) => x.Value.Equals(this.CurrentKind));
                if (q.Count() == 0)
                {
                    return "<no match for current value>";
                }

                return q.First().Key + " (" + this.CurrentKind.ToString() + ")";
            }
        }

        public Exception Error(string message)
        {
            return new Exception(
                "At offset " + this.offset + " on '" + this.CurrentValue + "' near '" +
                this.text.Substring(this.offset) + "': " + message);
        }

        private void ReadNextChar()
        {
            if (this.offset < this.text.Length)
            {
                this.offset++;
            }

            this.currentChar = this.offset < this.text.Length ? this.text[this.offset] : '\0';
        }
    }


    public class AdHocContextHelper
    {
        private static Dictionary<Type, Dictionary<string, IList>> persistedData;

        public static void ClearData()
        {
            persistedData = null;
        }

        public static IQueryable GetQueryable(RuntimeTypeHandle contextHandle, string name, RuntimeTypeHandle elementTypeHandle, string populationModel)
        {
            Type context = Type.GetTypeFromHandle(contextHandle);
            Type elementType = Type.GetTypeFromHandle(elementTypeHandle);
            return GetQueryableWithTypes(context, name, elementType, populationModel);
        }

        private static IQueryable GetQueryableWithTypes(Type context, string name, Type elementType, string populationModel)
        {
            if (persistedData == null)
            {
                persistedData = new Dictionary<Type, Dictionary<string, IList>>();
            }

            Dictionary<string, IList> itemSets;
            if (!persistedData.TryGetValue(context, out itemSets))
            {
                itemSets = new Dictionary<string, IList>();
                persistedData[context] = itemSets;
                PopulateModel(context, itemSets, populationModel);
            }

            return (IQueryable)itemSets[name].AsQueryable();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemSets"></param>
        /// <param name="populationModel"></param>
        /// <remarks>
        /// Grammar:
        /// model       ::= set-decl | model set-decl
        /// set-decl    ::= identifier ':' '[' records ']' ';'
        /// records     ::= record | records ',' record
        /// record      ::= '{' values '}'
        /// values      ::= value | values ',' value
        /// value       ::= name ':' string-constant
        /// </remarks>
        private static void PopulateModel(Type context, Dictionary<string, IList> itemSets, string populationModel)
        {
            const int OPENSQUARE = 1;
            const int CLOSESQUARE = 2;
            const int COMMA = 3;
            const int SEMICOLON = 4;
            const int STRING = 5;
            const int IDENTIFIER = 6;
            const int OPENBRACE = 7;
            const int CLOSEBRACE = 8;
            const int COLON = 9;

            AdHocLexer<int> lexer = new AdHocLexer<int>(populationModel,
                TestUtil.CreateDictionary<char, int>(
                    new KeyValuePair<char, int>('[', OPENSQUARE),
                    new KeyValuePair<char, int>(']', CLOSESQUARE),
                    new KeyValuePair<char, int>(',', COMMA),
                    new KeyValuePair<char, int>(';', SEMICOLON),
                    new KeyValuePair<char, int>('{', OPENBRACE),
                    new KeyValuePair<char, int>('}', CLOSEBRACE),
                    new KeyValuePair<char, int>(':', COLON)),
                TestUtil.CreateDictionary<Regex, int>(
                    new KeyValuePair<Regex, int>(new Regex("'[^']*'"), STRING),
                    new KeyValuePair<Regex, int>(new Regex("[a-zA-Z][a-zA-Z0-9]*"), IDENTIFIER)));

            // model.
            while (lexer.ReadNext())
            {
                // set-decl
                if (lexer.CurrentKind != IDENTIFIER) throw new Exception();
                string set = lexer.CurrentValue;
                Type elementType = TestUtil.GetIQueryableElement(context.GetProperty(set).PropertyType);
                IList records = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType));
                itemSets[set] = records;

                if (lexer.ReadNextOrThrow() != COLON) throw new Exception();
                if (lexer.ReadNextOrThrow() != OPENSQUARE) throw new Exception();
                // records.
                lexer.ReadNextOrThrow();
                while (lexer.CurrentKind != CLOSESQUARE)
                {
                    // record
                    if (lexer.CurrentKind != OPENBRACE) throw new Exception();
                    object record = Activator.CreateInstance(elementType);
                    records.Add(record);

                    // values
                    lexer.ReadNext();
                    while (lexer.CurrentKind != CLOSEBRACE)
                    {
                        // value
                        if (lexer.CurrentKind != IDENTIFIER) throw lexer.Error("Identifier expected.");
                        string propertyName = lexer.CurrentValue;
                        PropertyInfo property = elementType.GetProperty(propertyName);
                        if (lexer.ReadNextOrThrow() != COLON) throw lexer.Error("Colon expected between property name and value.");
                        lexer.ReadNextOrThrow();
                        if (lexer.CurrentKind != STRING) throw lexer.Error("String expected");
                        string value = lexer.CurrentValue;
                        value = value.Substring(1, value.Length - 2).Replace("''", "'");
                        property.SetValue(record, Convert.ChangeType(value, property.PropertyType), new object[0]);
                        lexer.ReadNextOrThrow();
                        if (lexer.CurrentKind != CLOSEBRACE)
                        {
                            if (lexer.CurrentKind != COMMA) throw lexer.Error("Comma or closing brace expected.");
                            lexer.ReadNextOrThrow();
                        }
                    }

                    if (lexer.CurrentKind != CLOSEBRACE) throw new Exception();
                    lexer.ReadNext();
                }
                if (lexer.CurrentKind != CLOSESQUARE) throw new Exception();
                if (lexer.ReadNextOrThrow() != SEMICOLON) throw new Exception();
            }
        }
    }

    /// <summary>An ad-hoc container.</summary>
    public class AdHocContainer
    {
        /// <summary>Number of containers created.</summary>
        private static int containerCount;

        public AdHocContainer()
        {
            this.Name = "AdHocContainer" + ++containerCount;
            this.Namespace = "Ns";
            this.AssociationSets = new List<AdHocAssociationSet>();
            this.EntitySets = new List<AdHocEntitySet>();
            this.ExtraAssociationTypes = new List<AdHocAssociationType>();
            this.ExtraComplexTypes = new List<AdHocComplexType>();
            this.ExtraEntityTypes = new List<AdHocEntityType>();
        }

        public string Name { get; set; }
        public string Namespace { get; set; }

        public List<AdHocAssociationSet> AssociationSets { get; set; }
        public List<AdHocEntitySet> EntitySets { get; set; }
        public List<AdHocAssociationType> ExtraAssociationTypes { get; set; }
        public List<AdHocComplexType> ExtraComplexTypes { get; set; }
        public List<AdHocEntityType> ExtraEntityTypes { get; set; }

        public AdHocAssociationType TryGetAssociationType(string name)
        {
            TestUtil.CheckArgumentNotNull(name, "name");
            foreach (AdHocAssociationType type in this.AssociationTypes)
            {
                if (type.Name == name)
                {
                    return type;
                }
            }

            return null;
        }

        public AdHocStructuralType TryGetStructuralType(string name)
        {
            TestUtil.CheckArgumentNotNull(name, "name");
            foreach (AdHocEntityType type in this.EntityTypes)
            {
                if (type.FullName == name || type.Name == name)
                {
                    return type;
                }
            }

            foreach (AdHocComplexType type in this.ComplexTypes)
            {
                if (type.FullName == name || type.Name == name)
                {
                    return type;
                }
            }

            return null;
        }

        public IEnumerable<AdHocAssociationType> AssociationTypes
        {
            get { return (from e in this.AssociationSets select e.Type).Concat(ExtraAssociationTypes).Distinct(); }
        }

        public IEnumerable<AdHocComplexType> ComplexTypes
        {
            get { return this.EntityTypes.SelectMany((e) => e.Properties).OfType<AdHocComplexType>().Concat(this.ExtraComplexTypes).Distinct(); }
        }

        public IEnumerable<AdHocEntityType> EntityTypes
        {
            get { return (from e in this.EntitySets.AsQueryable() select e.Type).Concat(ExtraEntityTypes).Distinct(); }
        }

        public void AddCollectionAssociation(AdHocEntitySet parentSet, AdHocEntitySet childSet)
        {
            TestUtil.CheckArgumentNotNull(parentSet, "parentSet");
            TestUtil.CheckArgumentNotNull(childSet, "childSet");

            // See if there's a type we can reuse.
            AdHocAssociationType associationType = GetOrCreateCollectionAssociationType(parentSet.Type, childSet.Type);
            AdHocAssociationSet set = new AdHocAssociationSet()
            {
                Name = parentSet.Name + childSet.Name,
                Type = associationType,
                Ends = new List<AdHocAssociationSetEnd>()
                {
                    new AdHocAssociationSetEnd() { EndType = associationType.OneEnd, EntitySet = parentSet },
                    new AdHocAssociationSetEnd() { EndType = associationType.ManyEnd, EntitySet = childSet },
                }
            };

            this.AssociationSets.Add(set);
        }

        public AdHocAssociationType GetOrCreateCollectionAssociationType(AdHocEntityType parentType, AdHocEntityType childType)
        {
            return GetOrCreateCollectionAssociationType(parentType, childType, "", true);
        }

        public AdHocAssociationType GetOrCreateCollectionAssociationType(AdHocEntityType parentType, AdHocEntityType childType, string name, bool addProperties)
        {
            TestUtil.CheckArgumentNotNull(parentType, "parentType");
            TestUtil.CheckArgumentNotNull(childType, "childType");
            TestUtil.CheckArgumentNotNull(name, "name");
            foreach (AdHocAssociationType associationType in this.AssociationTypes)
            {
                AdHocAssociationTypeEnd parentEnd;
                AdHocAssociationTypeEnd childEnd;
                if (associationType.Ends[0].Multiplicity == "1" &&
                    associationType.Ends[1].Multiplicity == "*")
                {
                    parentEnd = associationType.Ends[0];
                    childEnd = associationType.Ends[1];
                }
                else if (associationType.Ends[0].Multiplicity == "*" &&
                    associationType.Ends[1].Multiplicity == "1")
                {
                    childEnd = associationType.Ends[0];
                    parentEnd = associationType.Ends[1];
                }
                else
                {
                    continue;
                }

                if (parentEnd.Type == parentType && childEnd.Type == childType)
                {
                    if (name == "" || associationType.Name == name)
                    {
                        return associationType;
                    }
                }
            }

            // Not found, create one.
            AdHocAssociationType result = new AdHocAssociationType()
            {
                Ends = new List<AdHocAssociationTypeEnd>()
                    {
                        new AdHocAssociationTypeEnd()
                        {
                            Multiplicity = "1", RoleName = "Parent" + parentType.Name, Type = parentType
                        },
                        new AdHocAssociationTypeEnd()
                        {
                            Multiplicity = "*", RoleName = childType.Name + "Children", Type = childType
                        },
                    }
            };
            result.Name = (name != "") ? name : parentType.Name + childType.Name + "AssociationType";

            if (addProperties)
            {
                parentType.Properties.Add(new AdHocNavigationProperty()
                {
                    AssociationType = result,
                    Name = name + childType.Name + "Children",
                    SourceEnd = result.Ends[0],
                    Type = childType
                });
                childType.Properties.Add(new AdHocNavigationProperty()
                {
                    AssociationType = result,
                    Name = name + parentType.Name + "Parent",
                    SourceEnd = result.Ends[1],
                    Type = parentType
                });
            }

            return result;
        }
    }

    /// <summary>Use this class to represent a data model.</summary>
    public class AdHocModel
    {
        private const int SEMICOLON = 2;
        private const int OPENBRACE = 4;
        private const int CLOSEBRACE = 5;
        private const int IDENTIFIER = 8;
        private const int DOT = 11;
        private const int ASSOCIATIONTYPE = 13;
        private const int STAR = 14;
        private const int ONE = 15;
        private const int NAVIGATION = 16;

        private const string MetadataNsUri = AstoriaUnitTests.Data.XmlData.DataWebMetadataNamespace;

        /// <summary>Initializes a new empty <see cref="AdHocModel."/></summary>
        public AdHocModel()
        {
            this.ConceptualNs = TestXmlConstants.EdmOasisNamespace;
        }

        /// <summary>Initializes a new <see cref="AdHocModel"/> with the specified containers.</summary>
        /// <param name="containers">Containers to include in the model.</param>
        public AdHocModel(params AdHocContainer[] containers) : this()
        {
            this.Containers = new List<AdHocContainer>(containers);
        }

        public IEnumerable<AdHocAssociationType> AssociationTypes
        {
            get { return (this.Containers.SelectMany((c) => c.AssociationTypes)).Distinct(); }
        }

        public IEnumerable<AdHocComplexType> ComplexTypes
        {
            get { return (this.Containers.SelectMany((c) => c.ComplexTypes)).Distinct(); }
        }

        public IEnumerable<AdHocEntityType> EntityTypes
        {
            get { return (this.Containers.SelectMany((c) => c.EntityTypes)).Distinct(); }
        }

        public List<AdHocContainer> Containers { get; set; }

        /// <summary>Prefix to be used when constructing database.</summary>
        public string DatabasePrefix { get; set; }

        /// <summary>Default connection string for model.</summary>
        public string DefaultConnectionString { get; set; }

        /// <summary>Default entity connection string for model.</summary>
        public string DefaultEntityConnectionString { get; set; }

        public string Namespace
        {
            get
            {
                AdHocContainer firstContainer = this.Containers.FirstOrDefault();
                if (firstContainer == null)
                {
                    throw new NotSupportedException("AdHocModel takes its namespace from the first container.");
                }

                return firstContainer.Namespace;
            }
        }

        /// <summary></summary>
        /// <param name="text"></param>
        /// <returns></returns>
        /// <remarks>
        /// Grammar:
        /// declaration-list    ::= declaration | declaration declaration-list
        /// declaration         ::= dotted-identifier '=' 'entitytype' [':' dotted-identifier] complextype-spec ';'
        ///                         | dotted-identifier '=' 'complexvaltype' complextype-spec ';'
        ///                         | dotted-identifier '=' 'complexreftype' complextype-spec ';'
        ///                         | dotted-identifier '=' 'associationtype' associationtype-spec ';'
        ///                         | identifier ':' identifier(entitytype) ';'
        ///                         | identifier ':' identifier(association-type) '{' end-list '}' ';'
        /// associationtype-spec::= '{' end-type-list '}'
        /// end-type-list       ::= E | end-type end-type-list
        /// end-type            ::= identifier(role name) dotted-identifier multiplicity ';'
        /// complextype-spec    ::= '{' field-list '}'
        /// field-list          ::= E | field field-list
        /// field               ::= identifier dotted-identifier [key] ';'
        ///                         | 'navigation' identifier(name) dotted-identifier(associationtype) identifier(role-on-end) dotted-identifier(type)
        /// end-list            ::= E | end end-list
        /// end                 ::= identifier(end-type role name) identifier(entityset)
        /// 
        /// Examples:
        /// Ns.E1 = entitytype { ID string key; }; ES1: Ns.E1;
        /// 
        /// Ns.CT1 = complexreftype { CP1 string; }; Ns.ET1 = entitytype { ID string key; CT Ns.CT1; }; ES1: ET1;
        /// 
        /// Ns.ET1 = entitytype { PK int key; };
        /// Ns.ET2 = entitytype : Ns.ET1 { Name string; };
        /// ES1 : Ns.ET2;
        /// </remarks>
        public static AdHocModel ModelFromText(string text)
        {
            const int EQUALS = 1;
            const int COLON = 3;
            const int ENTITYTYPE = 6;
            const int KEY = 7;
            const int COMPLEXVALTYPE = 9;
            const int COMPLEXREFTYPE = 10;
            const int COLLECTION = 12;

            AdHocLexer<int> lexer = new AdHocLexer<int>(text,
                TestUtil.CreateDictionary<char, int>(
                    new KeyValuePair<char, int>('=', EQUALS),
                    new KeyValuePair<char, int>(';', SEMICOLON),
                    new KeyValuePair<char, int>(':', COLON),
                    new KeyValuePair<char, int>('{', OPENBRACE),
                    new KeyValuePair<char, int>('}', CLOSEBRACE),
                    new KeyValuePair<char, int>('.', DOT),
                    new KeyValuePair<char, int>('*', STAR),
                    new KeyValuePair<char, int>('1', ONE)),
                TestUtil.CreateDictionary<Regex, int>(
                    new KeyValuePair<Regex, int>(new Regex("entitytype"), ENTITYTYPE),
                    new KeyValuePair<Regex, int>(new Regex("key"), KEY),
                    new KeyValuePair<Regex, int>(new Regex("complexreftype"), COMPLEXREFTYPE),
                    new KeyValuePair<Regex, int>(new Regex("complexvaltype"), COMPLEXVALTYPE),
                    new KeyValuePair<Regex, int>(new Regex("collection"), COLLECTION),
                    new KeyValuePair<Regex, int>(new Regex("associationtype"), ASSOCIATIONTYPE),
                    new KeyValuePair<Regex, int>(new Regex("navigation"), NAVIGATION),
                    new KeyValuePair<Regex, int>(new Regex("[a-zA-Z][a-zA-Z_0-9]*"), IDENTIFIER)));

            AdHocContainer container = new AdHocContainer();

            // decl-list
            while (lexer.ReadNext())
            {
                // declaration
                lexer.CheckCurrentKind(IDENTIFIER, "Identifier expected.");
                string identifier = lexer.ReadDottedIdentifier(IDENTIFIER, DOT, '.');
                int kind = lexer.CurrentKind;
                if (kind == COLON)
                {
                    // declaration of variable.
                    lexer.ReadNextKindOrThrow(IDENTIFIER, "Identifier expected for type.");
                    string typeName = lexer.ReadDottedIdentifier(IDENTIFIER, DOT, '.');
                    AdHocEntityType type = container.TryGetStructuralType(typeName) as AdHocEntityType;
                    if (type != null)
                    {
                        container.EntitySets.Add(new AdHocEntitySet() { Name = identifier, Type = type });
                    }
                    else
                    {
                        AdHocAssociationType associationType = container.TryGetAssociationType(typeName);
                        if (associationType == null)
                        {
                            throw lexer.Error("Unable to find type '" + typeName + "'.");
                        }

                        container.AssociationSets.Add(ParseAssociationSetEnds(lexer, container, identifier, associationType));
                    }

                    lexer.CheckCurrentKind(SEMICOLON, "Semicolon expected after variable declaration.");
                }
                else
                {
                    lexer.CheckCurrentKind(EQUALS, "If not ':' for variable, '=' is expected for type.");

                    // declaration of type.
                    kind = lexer.ReadNextOrThrow();
                    AdHocEntityType entityType = null;
                    bool existing = false;
                    AdHocStructuralType type;
                    switch (kind)
                    {
                        case ENTITYTYPE:
                            // Append properties if an entity type is redeclared.
                            entityType = (AdHocEntityType)container.TryGetStructuralType(identifier);
                            if (entityType == null)
                            {
                                entityType = new AdHocEntityType();
                                entityType.KeyProperties.Clear();
                            }
                            else
                            {
                                existing = true;
                            }
                            type = entityType;
                            break;
                        case COMPLEXREFTYPE:
                        case COMPLEXVALTYPE:
                            // TODO: add a switch to indicate value type or reference type CLR generation.
                            type = new AdHocComplexType();
                            break;
                        case ASSOCIATIONTYPE:
                            ParseAssociationType(lexer, identifier, container);
                            if (lexer.ReadNextOrThrow() != SEMICOLON)
                            {
                                throw lexer.Error("Semicolong expected after association type specification.");
                            }

                            continue;
                        default:
                            throw lexer.Error("Expecting one of 'entitytype', 'complexreftype' or 'complexvaltype'.");
                    }

                    if (!existing)
                    {
                        type.Properties = new List<AdHocProperty>();
                        type.Namespace = "";
                        type.Name = identifier;
                    }

                    lexer.ReadNextOrThrow();
                    if (lexer.CurrentKind == COLON && kind == ENTITYTYPE)
                    {
                        lexer.ReadNextKindOrThrow(IDENTIFIER, "':' after entity type should be followed by base type.");
                        string baseIdentifier = lexer.ReadDottedIdentifier(IDENTIFIER, DOT, '.');
                        AdHocEntityType baseEntityType = (AdHocEntityType)container.TryGetStructuralType(baseIdentifier);
                        ((AdHocEntityType)type).BaseType = baseEntityType;
                    }

                    lexer.CheckCurrentKind(OPENBRACE, "Open brace expected for type definition.");
                    lexer.ReadNext();
                    while (lexer.CurrentKind != CLOSEBRACE)
                    {
                        // field
                        AdHocProperty property = null;
                        AdHocNavigationProperty navigationProperty = null;
                        if (lexer.CurrentKind == NAVIGATION)
                        {
                            navigationProperty = ParseNavigationProperty(lexer, container);
                            type.Properties.Add(navigationProperty);
                            lexer.CheckCurrentKind(SEMICOLON, "Semicolon expected after navigation property.");
                            lexer.ReadNextOrThrow();
                            continue;
                        }

                        lexer.CheckCurrentKind(IDENTIFIER, "Identifier for property name expected.");
                        string propertyName = lexer.CurrentValue;
                        lexer.ReadNextOrThrow();
                        lexer.CheckCurrentKind(IDENTIFIER, "Identifier for property type expected.");
                        string fieldType = lexer.ReadDottedIdentifier(IDENTIFIER, DOT, '.');
                        switch (fieldType)
                        {
                            case "string":
                                property = new AdHocScalarProperty();
                                property.Type = new AdHocPrimitiveType(typeof(string));
                                break;
                            case "int":
                                property = new AdHocScalarProperty();
                                property.Type = new AdHocPrimitiveType(typeof(int));
                                break;
                            case "guid":
                                property = new AdHocScalarProperty();
                                property.Type = new AdHocPrimitiveType(typeof(Guid));
                                break;
                            default:
                                AdHocType resolvedType = container.TryGetStructuralType(fieldType);
                                if (resolvedType == null)
                                {
                                    throw new Exception("Unrecognized field type '" + fieldType + "'.");
                                }

                                if (resolvedType is AdHocEntityType)
                                {
                                    navigationProperty = new AdHocNavigationProperty();
                                    property = navigationProperty;
                                    if (entityType == null)
                                    {
                                        throw lexer.Error("Only entity type can have navigation properties.");
                                    }
                                }
                                else
                                {
                                    property = new AdHocScalarProperty();
                                }

                                property.Type = resolvedType;
                                break;
                        }

                        type.Properties.Add(property);
                        property.Name = propertyName;

                        int fieldEndKind = lexer.CurrentKind;
                        if (fieldEndKind == KEY)
                        {
                            if (entityType == null) throw lexer.Error("Complex types cannot have key properties.");
                            entityType.KeyProperties.Add(property);
                            fieldEndKind = lexer.ReadNextOrThrow();
                        }

                        if (fieldEndKind == COLLECTION)
                        {
                            if (navigationProperty == null) throw lexer.Error("Only navigation properties can be collections.");
                            navigationProperty.AssociationType = container.GetOrCreateCollectionAssociationType(
                                entityType, (AdHocEntityType)navigationProperty.Type, navigationProperty.Name, false);
                            if (!container.AssociationTypes.Contains(navigationProperty.AssociationType))
                            {
                                container.ExtraAssociationTypes.Add(navigationProperty.AssociationType);
                            }

                            navigationProperty.SourceEnd = navigationProperty.AssociationType.OneEnd;
                            fieldEndKind = lexer.ReadNextOrThrow();
                        }

                        if (fieldEndKind != SEMICOLON)
                        {
                            throw lexer.Error("Semicolon expected after field declaration.");
                        }

                        lexer.ReadNextOrThrow();
                    }

                    if (entityType != null)
                    {
                        if (!container.ExtraEntityTypes.Contains(entityType))
                        {
                            container.ExtraEntityTypes.Add(entityType);
                        }
                    }
                    else
                    {
                        container.ExtraComplexTypes.Add((AdHocComplexType)type);
                    }

                    if (lexer.ReadNextOrThrow() != SEMICOLON)
                    {
                        throw lexer.Error("Semicolon expected after type/variable declaration.");
                    }
                }
            }

            AdHocModel model = new AdHocModel(container) { ConceptualNs = TestXmlConstants.EdmV1Namespace };
            return model;
        }

        private void WriteCreateTable(List<string> statements, string tableName, AdHocEntityType type)
        {
            StringBuilder createTableStatement = new StringBuilder();
            createTableStatement.AppendLine("CREATE TABLE [" + tableName + "] (");
            foreach (var key in type.Properties)
            {
                AppendColumnForProperty(createTableStatement, key, "");
            }

            // Include the keys from the base type.
            if (type.BaseType != null)
            {
                foreach (var baseType in type.SelfAndBaseTypes)
                {
                    if (baseType.KeyProperties == null) continue;
                    foreach (var property in baseType.KeyProperties)
                    {
                        AppendColumnForProperty(createTableStatement, property, "");
                    }
                }
            }

            // Include the keys for assocations if it's on the many-side of relationships.
            foreach (var associationType in this.Containers.SelectMany((c) => c.AssociationTypes).Where((a) => a.ManyEnd != null && a.ManyEnd.Type == type))
            {
                foreach (var property in associationType.OneEnd.Type.CalculatedKeyProperties)
                {
                    AppendColumnForProperty(createTableStatement, property, associationType.Name.Replace(".", ""));
                }
            }

            // Add the primary key.
            createTableStatement.Append("  CONSTRAINT " + tableName + "PKConstraint PRIMARY KEY(");
            string separator = "";
            foreach (var key in type.CalculatedKeyProperties)
            {
                createTableStatement.Append(separator + key.Name);
                separator = ", ";
            }
            createTableStatement.AppendLine(")");

            createTableStatement.AppendLine(")");
            statements.Add(createTableStatement.ToString());
        }

        /// <summary>Creates a database that supports this model.</summary>
        /// <remarks>
        /// This method needs to be mainted in very close association with WriteStorageModel.
        /// 
        /// Inheritance is implemented (as in WriteStorageModel) with table-per-type (TPT).
        /// </remarks>
        public void CreateDatabase()
        {
            string server = DataUtil.DefaultDataSource;

            List<string> statements = new List<string>();
            string databaseName = null;
            foreach (var container in this.Containers)
            {
                databaseName = this.DatabasePrefix + container.Name + "Storage";

                statements.AddRange(DataUtil.BuildTSqlForRecreateDatabase(databaseName));
                statements.Add("USE [" + databaseName + "]");

                foreach (var entitySet in container.EntitySets)
                {
                    if (entitySet.Type.BaseType == null && entitySet.Type.DerivedTypes.Count == 0)
                    {
                        WriteCreateTable(statements, entitySet.Name, entitySet.Type);
                    }
                    else
                    {
                        foreach (AdHocEntityType type in entitySet.Type.SelfAndDerivedTypes)
                        {
                            WriteCreateTable(statements, entitySet.Name + type.Name, type);
                        }
                    }
                }
            }

            DataUtil.ExecuteNonQueryStatements(DataUtil.BuildTrustedConnection(server, "master"), statements);

            this.DefaultConnectionString = DataUtil.BuildTrustedConnection(server, databaseName);
            Trace.WriteLine("Created database for [" + this.DefaultConnectionString + "]");

            if (!String.IsNullOrEmpty(this.DefaultEntityConnectionString))
            {
                var b = new System.Data.EntityClient.EntityConnectionStringBuilder(this.DefaultEntityConnectionString);
                b.ProviderConnectionString = this.DefaultConnectionString;
                this.DefaultEntityConnectionString = b.ConnectionString;
            }
        }

        /// <summary>Drops the database created by a call to <see cref="CreateDatabase"/>.</summary>
        public void DropDatabase()
        {
            System.Data.SqlClient.SqlConnection.ClearAllPools();
            DataUtil.ExecuteNonQueryStatements(
                DataUtil.BuildTrustedConnection(null, null),
                DataUtil.BuildTSqlForSafeDropDatabase(this.Containers[0].Name + "Storage"));
        }

        private static AdHocAssociationSet ParseAssociationSetEnds(AdHocLexer<int> lexer, AdHocContainer container, string identifier, AdHocAssociationType associationType)
        {
            Debug.Assert(lexer != null, "lexer != null");
            Debug.Assert(identifier != null, "identifier != null");
            Debug.Assert(associationType != null, "associationType != null");

            // identifier ':' identifier(association-type) '{' end-list '}' ';'
            AdHocAssociationSet result = new AdHocAssociationSet() { Name = identifier, Type = associationType };
            if (lexer.CurrentKind != OPENBRACE)
            {
                throw lexer.Error("Opening brace expected.");
            }

            // end-list ::= E | end end-list
            lexer.ReadNextOrThrow();
            while (lexer.CurrentKind != CLOSEBRACE)
            {
                // end ::= identifier(end-type role name) identifier(entityset)
                lexer.CheckCurrentKind(IDENTIFIER, "Identifier expected for end-type role name.");
                AdHocAssociationTypeEnd typeEnd = associationType.Ends.Where((e) => e.RoleName == lexer.CurrentValue).Single();
                lexer.ReadNextOrThrow();
                lexer.CheckCurrentKind(IDENTIFIER, "Identifier expected for entity set name.");
                AdHocEntitySet set = container.EntitySets.Where((e) => e.Name == lexer.CurrentValue).Single();
                result.Ends.Add(new AdHocAssociationSetEnd()
                {
                    EndType = typeEnd,
                    EntitySet = set
                });

                if (lexer.ReadNextOrThrow() != SEMICOLON)
                {
                    throw lexer.Error("Semicolon expected after end definition.");
                }
                lexer.ReadNextOrThrow();
            }

            lexer.ReadNextOrThrow();

            return result;
        }

        private static void ParseAssociationType(AdHocLexer<int> lexer, string identifier, AdHocContainer container)
        {
            Debug.Assert(lexer != null, "lexer != null");
            Debug.Assert(identifier != null, "identifier != null");
            Debug.Assert(container != null, "container != null");
            Debug.Assert(lexer.CurrentKind == ASSOCIATIONTYPE, "lexer.CurrentKind == ASSOCIATIONTYPE");

            // associationtype-spec::= '{' end-type-list '}'
            if (lexer.ReadNextOrThrow() != OPENBRACE)
            {
                throw lexer.Error("'{' expected");
            }

            AdHocAssociationType associationType = new AdHocAssociationType();
            associationType.Name = identifier;
            associationType.Ends = new List<AdHocAssociationTypeEnd>();

            lexer.ReadNextOrThrow();
            while (lexer.CurrentKind != CLOSEBRACE)
            {
                // end-type ::= identifier dotted-identifier multiplicity ';'
                if (lexer.CurrentKind != IDENTIFIER)
                {
                    throw lexer.Error("Identifier for role name expected.");
                }

                string roleName = lexer.CurrentValue;
                if (lexer.ReadNextOrThrow() != IDENTIFIER)
                {
                    throw lexer.Error("Type expected.");
                }

                string typeIdentifier = lexer.ReadDottedIdentifier(IDENTIFIER, DOT, '.');
                AdHocEntityType endType = container.TryGetStructuralType(typeIdentifier) as AdHocEntityType;
                if (endType == null)
                {
                    throw lexer.Error("Expected entity type for role type.");
                }

                string multiplicityText;
                int multiplicity = lexer.CurrentKind;
                switch (multiplicity)
                {
                    case STAR:
                        multiplicityText = "*";
                        break;
                    case ONE:
                        multiplicityText = "1";
                        break;
                    default:
                        throw lexer.Error("Multiplicity expected.");
                }

                AdHocAssociationTypeEnd end = new AdHocAssociationTypeEnd()
                {
                    RoleName = roleName,
                    Multiplicity = multiplicityText,
                    Type = endType
                };
                associationType.Ends.Add(end);

                if (lexer.ReadNextOrThrow() != SEMICOLON)
                {
                    throw lexer.Error("Semicolon expected after role.");
                }

                lexer.ReadNextOrThrow();
            }

            container.ExtraAssociationTypes.Add(associationType);
        }

        private static AdHocNavigationProperty ParseNavigationProperty(AdHocLexer<int> lexer, AdHocContainer container)
        {
            TestUtil.CheckArgumentNotNull(lexer, "lexer");
            TestUtil.CheckArgumentNotNull(container, "container");

            // 'navigation' identifier(name) dotted-identifier(associationtype) identifier(role-on-end) dotted-identifier(type)
            AdHocNavigationProperty result = new AdHocNavigationProperty();
            lexer.CheckCurrentKind(NAVIGATION, "'navigation' keyword expected.");

            lexer.ReadNextKindOrThrow(IDENTIFIER, "Identifier for property name expected.");
            result.Name = lexer.CurrentValue;

            lexer.ReadNextKindOrThrow(IDENTIFIER, "Identifier for association type expected.");
            string typeIdentifier = lexer.ReadDottedIdentifier(IDENTIFIER, DOT, '.');
            result.AssociationType = container.TryGetAssociationType(typeIdentifier);
            if (result.AssociationType == null)
            {
                throw lexer.Error("Unable to find association type '" + lexer.CurrentValue + "'.");
            }

            lexer.CheckCurrentKind(IDENTIFIER, "Identifier for role name expected.");
            result.SourceEnd = result.AssociationType.Ends.Where((e) => e.RoleName == lexer.CurrentValue).Single();

            lexer.ReadNextKindOrThrow(IDENTIFIER, "Identifier for target type expected.");
            result.Type = container.TryGetStructuralType(lexer.ReadDottedIdentifier(IDENTIFIER, DOT, '.'));

            return result;
        }

        private void AppendColumnForProperty(StringBuilder builder, AdHocProperty property, string prefix)
        {
            if (builder == null)
            {
                throw new ArgumentNullException("builder");
            }
            if (property == null)
            {
                throw new ArgumentNullException("property");
            }

            AdHocScalarProperty scalar = property as AdHocScalarProperty;
            if (scalar == null) return;

            AdHocStructuralType structuralType = property.Type as AdHocStructuralType;
            if (structuralType == null)
            {
                builder.Append("  " + prefix + scalar.Name + " ");
                builder.Append(scalar.Type.StorageName);
                if (scalar.Type.StorageName == "nvarchar")
                {
                    builder.Append("(128)");
                }
                builder.Append(' ');
                builder.Append(scalar.IsNullable ? "NULL" : "NOT NULL");
                builder.AppendLine(",");
            }
            else
            {
                foreach (var p in structuralType.Properties)
                {
                    AppendColumnForProperty(builder, p, property.Name);
                }
            }
        }

        private bool WriteModelDifferent(Action<XmlWriter> action, string path)
        {
            StringWriter streamWriter = new StringWriter();
            XmlWriter writer = XmlWriter.Create(streamWriter, WriterSettings);
            action(writer);
            writer.Flush();

            string text = streamWriter.ToString();
            if (File.Exists(path))
            {
                string existingText = File.ReadAllText(path);
                if (existingText == text)
                {
                    return false;
                }
            }

            File.WriteAllText(path, text, writer.Settings.Encoding);
            return true;
        }

        /// <summary>
        /// Generates a CSDL file from the model
        /// </summary>
        /// <param name="name">Name of the file, without the .csdl suffix</param>
        /// <returns>Path to the CSDL file</returns>
        public string WriteCSDLFile(string name)
        {
            string basePath = TestUtil.GeneratedFilesLocation;
            string generatedCSDLLocation = Path.Combine(basePath, name + ".csdl");
            if (AstoriaTestProperties.RuntimeEnvironment == TestRuntimeEnvironment.VisualStudioIde)
            {
                WriteModelDifferent(WriteConceptualModel, generatedCSDLLocation);
            }
            else
            {
                this.WriteConceptualModel(generatedCSDLLocation);
            }
            return generatedCSDLLocation;
        }

        /// <summary>Generates EDM model files and an assembly for this model.</summary>
        /// <param name="name">Base name for model files.</param>
        /// <param name="isReflectionProviderBased">
        /// Whether a regular CLR based (rather than ObjectContext-based) context should be generated.
        /// </param>
        /// <returns>The generated assembly.</returns>
        public Assembly GenerateModelsAndAssembly(string name, bool isReflectionProviderBased)
        {
            TestUtil.CheckArgumentNotNull(name, "name");
            string basePath = TestUtil.GeneratedFilesLocation;
            if (AstoriaTestProperties.RuntimeEnvironment == TestRuntimeEnvironment.VisualStudioIde)
            {
                // Consider reusing existing model / libraries.
                WriteModelDifferent(WriteConceptualModel, Path.Combine(basePath, name + ".csdl"));
                WriteModelDifferent(WriteMappingModel, Path.Combine(basePath, name + ".msl"));
                WriteModelDifferent(WriteStorageModel, Path.Combine(basePath, name + ".ssdl"));
            }
            else
            {
                this.WriteConceptualModel(Path.Combine(basePath, name + ".csdl"));
                this.WriteMappingModel(Path.Combine(basePath, name + ".msl"));
                this.WriteStorageModel(Path.Combine(basePath, name + ".ssdl"));
            }

            if (String.IsNullOrEmpty(this.DefaultEntityConnectionString) &&
                !String.IsNullOrEmpty(this.DefaultConnectionString))
            {
                var b = new System.Data.EntityClient.EntityConnectionStringBuilder();
                b.Metadata = Path.Combine(basePath, name + ".csdl") + "|" +
                    Path.Combine(basePath, name + ".msl") + "|" +
                    Path.Combine(basePath, name + ".ssdl");
                b.Provider = DataUtil.ProviderName;
                b.ProviderConnectionString = this.DefaultConnectionString;
                this.DefaultEntityConnectionString = b.ConnectionString;
            }

            string libraryPath = TestUtil.GenerateAssembly(
                Path.Combine(basePath, name + ".csdl"), isReflectionProviderBased,
                this.DefaultEntityConnectionString);
            return Assembly.LoadFile(libraryPath);
        }

        public TypeBuilder GeneratePocoModel(ModuleBuilder builder)
        {
            return GeneratePocoModel(builder, "");
        }

        public TypeBuilder GeneratePocoModel(ModuleBuilder builder, string populationModel)
        {
            const System.Reflection.MethodAttributes methodAttributes =
                System.Reflection.MethodAttributes.Public |
                System.Reflection.MethodAttributes.SpecialName |
                System.Reflection.MethodAttributes.HideBySig;
            const System.Reflection.PropertyAttributes propertyAttributes =
                System.Reflection.PropertyAttributes.HasDefault;
            const System.Reflection.TypeAttributes typeAttributes =
                System.Reflection.TypeAttributes.Public;

            if (builder == null)
            {
                throw new ArgumentNullException("builder");
            }

            // Generate a type for every entity set.
            Dictionary<AdHocType, TypeBuilder> builders = new Dictionary<AdHocType, TypeBuilder>();
            foreach (AdHocEntityType type in this.EntityTypes)
            {
                builders[type] = builder.DefineType(type.FullName, typeAttributes);
            }
            foreach (AdHocEntityType type in this.EntityTypes)
            {
                TypeBuilder typeBuilder = builders[type];
                ConstructorInfo ctor = typeof(KeyAttribute).GetConstructor(new Type[] { typeof(string[]) });
                string[] keys = type.KeyProperties.Select(kp => kp.Name).ToArray();
                typeBuilder.SetCustomAttribute(new CustomAttributeBuilder(ctor, new object[] { keys }));
                foreach (AdHocProperty property in type.Properties)
                {
                    AdHocScalarProperty scalar = property as AdHocScalarProperty;
                    if (scalar != null)
                    {
                        Type clrType = ((AdHocPrimitiveType)scalar.Type).ClrType;
                        FieldBuilder fieldBuilder = typeBuilder.DefineField("_" + scalar.Name, clrType, FieldAttributes.Private);
                        PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(scalar.Name, propertyAttributes, clrType, null);

                        GenerateStandardGetAndSetCode(methodAttributes, typeBuilder, fieldBuilder, propertyBuilder);
                    }
                    else
                    {
                        AdHocNavigationProperty navigation = property as AdHocNavigationProperty;
                        if (navigation != null)
                        {
                            Type propertyType;
                            bool initializeInstance;
                            if (navigation.AssociationType.OtherEnd(navigation.SourceEnd).Multiplicity == "*")
                            {
                                // many - use collection
                                TypeBuilder elementType = builders[navigation.Type];
                                propertyType = typeof(List<>).MakeGenericType(elementType.UnderlyingSystemType);
                                initializeInstance = true;
                            }
                            else
                            {
                                // one - use a reference
                                TypeBuilder referenceType = builders[navigation.Type];
                                propertyType = referenceType.UnderlyingSystemType;
                                initializeInstance = false;
                            }
                            FieldBuilder fieldBuilder = typeBuilder.DefineField("_" + property.Name, propertyType, FieldAttributes.Private);
                            if (initializeInstance)
                            {

                                // create the default constructor
                                ConstructorBuilder ctorBuilder = typeBuilder.DefineConstructor(
                                        MethodAttributes.Public | MethodAttributes.RTSpecialName | MethodAttributes.SpecialName,
                                        CallingConventions.Standard,
                                        new Type[0]);
                                ILGenerator generator = ctorBuilder.GetILGenerator();
                                generator.Emit(OpCodes.Ldarg_0);
                                generator.Emit(OpCodes.Newobj, TypeBuilder.GetConstructor(propertyType, propertyType.GetGenericTypeDefinition().GetConstructor(new Type[0])));
                                generator.Emit(OpCodes.Stfld, fieldBuilder);
                                generator.Emit(OpCodes.Ldarg_0);
                                generator.Emit(OpCodes.Call, typeof(Object).GetConstructor(new Type[0]));
                                generator.Emit(OpCodes.Ret);
                            }

                            PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(property.Name, propertyAttributes, propertyType, null);
                            GenerateStandardGetAndSetCode(methodAttributes, typeBuilder, fieldBuilder, propertyBuilder);

                        }
                        else
                        {
                            throw new NotImplementedException();
                        }
                    }

                }
            }

            // Generate a top-level 'object context'.
            TypeBuilder contextTypeBuilder = builder.DefineType(this.Containers[0].Name + "Context", typeAttributes);
            foreach (AdHocEntitySet entitySet in this.Containers[0].EntitySets)
            {
                TypeBuilder entitySetType = builders[entitySet.Type];
                Type constructedType = entitySetType.CreateType();
                Type clrType = typeof(IQueryable<>).MakeGenericType(constructedType);
                PropertyBuilder propertyBuilder = contextTypeBuilder.DefineProperty(entitySet.Name, propertyAttributes, clrType, null);
                MethodBuilder m = contextTypeBuilder.DefineMethod("get_" + entitySet.Name, methodAttributes, clrType, Type.EmptyTypes);
                ILGenerator generator = m.GetILGenerator();
                generator.Emit(OpCodes.Ldtoken, contextTypeBuilder);
                generator.Emit(OpCodes.Ldstr, entitySet.Name);
                generator.Emit(OpCodes.Ldtoken, constructedType);
                generator.Emit(OpCodes.Ldstr, populationModel);
                generator.Emit(OpCodes.Call, typeof(AdHocContextHelper).GetMethod("GetQueryable"));
                generator.Emit(OpCodes.Castclass, clrType);
                generator.Emit(OpCodes.Ret);
                propertyBuilder.SetGetMethod(m);
            }

            return contextTypeBuilder;
        }

        private static void GenerateStandardGetAndSetCode(MethodAttributes methodAttributes, TypeBuilder typeBuilder, FieldBuilder fieldBuilder, PropertyBuilder propertyBuilder)
        {
            Debug.Assert(fieldBuilder.FieldType == propertyBuilder.PropertyType, "can't generate standard Get and Set if the field type is not the same as the property type");
            // Generate standard get/set code.
            MethodBuilder m = typeBuilder.DefineMethod("get_" + propertyBuilder.Name, methodAttributes, propertyBuilder.PropertyType, Type.EmptyTypes);
            ILGenerator generator = m.GetILGenerator();
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldfld, fieldBuilder);
            generator.Emit(OpCodes.Ret);
            propertyBuilder.SetGetMethod(m);

            m = typeBuilder.DefineMethod("set_" + propertyBuilder.Name, methodAttributes, null, new Type[] { propertyBuilder.PropertyType });
            generator = m.GetILGenerator();
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldarg_1);
            generator.Emit(OpCodes.Stfld, fieldBuilder);
            generator.Emit(OpCodes.Ret);
            propertyBuilder.SetSetMethod(m);
        }

        public void WriteConceptualModel(string path)
        {
            using (XmlWriter writer = XmlWriter.Create(path, WriterSettings))
            {
                WriteConceptualModel(writer);
            }
        }

        public void WriteMappingModel(string path)
        {
            using (XmlWriter writer = XmlWriter.Create(path, WriterSettings))
            {
                WriteMappingModel(writer);
            }
        }

        public void WriteStorageModel(string path)
        {
            using (XmlWriter writer = XmlWriter.Create(path, WriterSettings))
            {
                WriteStorageModel(writer);
            }
        }

        public void WriteConceptualModel(XmlWriter writer)
        {
            writer.WriteStartElement("Schema", ConceptualNs);
            writer.WriteAttributeString("Alias", "Self");
            writer.WriteAttributeString("Namespace", this.Namespace);
            foreach (var container in this.Containers)
            {
                writer.WriteStartElement("EntityContainer", ConceptualNs);
                writer.WriteAttributeString("Name", container.Name);
                foreach (var entitySet in container.EntitySets)
                {
                    writer.WriteStartElement("EntitySet", ConceptualNs);
                    writer.WriteAttributeString("Name", entitySet.Name);
                    writer.WriteAttributeString("EntityType", entitySet.Type.FullName);
                    writer.WriteEndElement();
                }
                foreach (var associationSet in container.AssociationSets)
                {
                    writer.WriteStartElement("AssociationSet");
                    writer.WriteAttributeString("Name", associationSet.Name);
                    writer.WriteAttributeString("Association", "Self." + associationSet.Type.Name);
                    foreach (var end in associationSet.Ends)
                    {
                        writer.WriteStartElement("End");
                        // writer.WriteAttributeString("Role", end.EndType.Type.Name);
                        writer.WriteAttributeString("Role", end.EndType.RoleName);
                        writer.WriteAttributeString("EntitySet", end.EntitySet.Name);
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
            foreach (var type in this.EntityTypes)
            {
                writer.WriteStartElement("EntityType", ConceptualNs);
                writer.WriteAttributeString("Name", type.Name);
                if (type.BaseType != null)
                {
                    writer.WriteAttributeString("BaseType", "Self." + type.BaseType.Name);
                }
                if (type.EPMInfo != null)
                {
                    // WriteEPMInfo(writer, type.EPMInfo);
                }
                if (type.KeyProperties != null && type.KeyProperties.Count > 0)
                {
                    writer.WriteStartElement("Key", ConceptualNs);
                    foreach (var key in type.KeyProperties)
                    {
                        writer.WriteStartElement("PropertyRef");
                        writer.WriteAttributeString("Name", key.Name);
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                }
                foreach (var property in type.Properties)
                {
                    this.WriteConceptualProperty(writer, property);
                }
                writer.WriteEndElement();
            }

            foreach (var type in this.ComplexTypes)
            {
                writer.WriteStartElement("ComplexType");
                writer.WriteAttributeString("Name", type.Name);
                foreach (var property in type.Properties)
                {
                    this.WriteConceptualProperty(writer, property);
                }
                writer.WriteEndElement();
            }

            foreach (var type in this.AssociationTypes)
            {
                writer.WriteStartElement("Association");
                writer.WriteAttributeString("Name", type.Name);
                foreach (var end in type.Ends)
                {
                    writer.WriteStartElement("End");
                    writer.WriteAttributeString("Role", end.RoleName);
                    writer.WriteAttributeString("Type", end.Type.FullName);
                    writer.WriteAttributeString("Multiplicity", end.Multiplicity);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        private void WriteConceptualProperty(XmlWriter writer, AdHocProperty property)
        {
            AdHocScalarProperty scalar = property as AdHocScalarProperty;
            if (scalar != null)
            {
                writer.WriteStartElement("Property");
                writer.WriteAttributeString("Name", scalar.Name);
                if (scalar.Type is AdHocPrimitiveType)
                {
                    writer.WriteAttributeString("Type", scalar.Type.Name);
                }
                else
                {
                    writer.WriteAttributeString("Type", scalar.Type.FullName);
                }

                writer.WriteAttributeString("Nullable", scalar.IsNullable ? "true" : "false");
                // WriteEPMInfo(writer, property.EPMInfo, string.Empty);
                writer.WriteEndElement();
            }

            AdHocNavigationProperty navigation = property as AdHocNavigationProperty;
            if (navigation != null)
            {
                writer.WriteStartElement("NavigationProperty");
                writer.WriteAttributeString("Name", navigation.Name);
                writer.WriteAttributeString("Relationship", "Self." + navigation.AssociationType.Name);
                writer.WriteAttributeString("FromRole", navigation.SourceEnd.RoleName);
                writer.WriteAttributeString("ToRole", navigation.AssociationType.OtherEnd(navigation.SourceEnd).RoleName);
                writer.WriteEndElement();
            }
        }

        //private static void WriteEPMInfo(XmlWriter writer, List<AdHocEPMInfo> epmInfo)
        //{
        //    if(epmInfo != null)
        //    {
        //        for (int epmInfoIdx = 0; epmInfoIdx < epmInfo.Count; epmInfoIdx++)
        //        {
        //            WriteEPMInfo(writer, epmInfo[epmInfoIdx], epmInfoIdx == 0 ? string.Empty : "_" + epmInfoIdx.ToString());
        //        }
        //    }
        //}

        //private static void WriteEPMInfo(XmlWriter writer, AdHocEPMInfo epmInfo, string suffix)
        //{
        //    if (epmInfo != null)
        //    {
        //        if (suffix == null)
        //        {
        //            suffix = string.Empty;
        //        }

        //        // Allow negative testing - do not write FC_SourcePath attribute if null
        //        if (epmInfo.SourcePath != null)
        //        {
        //            writer.WriteAttributeString("FC_SourcePath" + suffix, MetadataNsUri, epmInfo.SourcePath);
        //        }

        //        writer.WriteAttributeString("FC_KeepInContent" + suffix, MetadataNsUri, (epmInfo.KeepInContent ?? string.Empty).ToString());

        //        // ContentKind and NsPrefix/NsUri are mutually exclusive. Here we are making it possible to specify both just 
        //        // to be able to test server for this condition. 
        //        if (epmInfo.ContentKind != null)
        //        {
        //            // Let's write an empty string if epmInfo.ContentKind is not a valid SyndicationTextConentKind value. This allows for some negative testing.
        //            writer.WriteAttributeString("FC_ContentKind" + suffix, MetadataNsUri, Enum.IsDefined(typeof(AtomSyndicationTextContentKind), epmInfo.ContentKind) ?
        //                TranslateContentKind((AtomSyndicationTextContentKind)epmInfo.ContentKind) :
        //                string.Empty);
        //        }

        //        // Allow writing the prefix even if the uri is null, for negative testing scenarios
        //        if (epmInfo.NsPrefix != null)
        //        {
        //            writer.WriteAttributeString("FC_NsPrefix" + suffix, MetadataNsUri, epmInfo.NsPrefix);
        //        }

        //        if (epmInfo.NsUri != null)
        //        {
        //            writer.WriteAttributeString("FC_NsUri" + suffix, MetadataNsUri, epmInfo.NsUri);
        //        }

        //        writer.WriteAttributeString("FC_TargetPath" + suffix, MetadataNsUri, epmInfo.TargetPath ?? TranslateSyndicationItem(epmInfo.SyndicationItem));

        //        if (epmInfo.CriteriaValue != null)
        //        {
        //            writer.WriteAttributeString("FC_CriteriaValue" + suffix, MetadataNsUri, epmInfo.CriteriaValue);
        //        }
        //    }
        //}

        //private static string TranslateContentKind(AtomSyndicationTextContentKind contentKind)
        //{
        //    switch (contentKind)
        //    {
        //        case AtomSyndicationTextContentKind.Plaintext:
        //            return "text";
        //        case AtomSyndicationTextContentKind.Html:
        //            return "html";
        //        case AtomSyndicationTextContentKind.Xhtml:
        //            return "xhtml";
        //        default:
        //            throw new ArgumentException("Unknown SyndicationTextContentKind enum value", "contentKind");
        //    }
        //}

        //private static string TranslateSyndicationItem(AtomSyndicationItemProperty itemProperty)
        //{
        //    switch (itemProperty)
        //    {
        //        case AtomSyndicationItemProperty.AuthorEmail:
        //        case AtomSyndicationItemProperty.AuthorName:
        //        case AtomSyndicationItemProperty.AuthorUri:
        //        case AtomSyndicationItemProperty.ContributorEmail:
        //        case AtomSyndicationItemProperty.ContributorName:
        //        case AtomSyndicationItemProperty.ContributorUri:
        //        case AtomSyndicationItemProperty.Published:
        //        case AtomSyndicationItemProperty.Rights:
        //        case AtomSyndicationItemProperty.Summary:
        //        case AtomSyndicationItemProperty.Title:
        //        case AtomSyndicationItemProperty.Updated:
        //            return "Syndication" + itemProperty.ToString();
        //        case AtomSyndicationItemProperty.CustomProperty:
        //            throw new NotSupportedException("AtomSyndicationItemProperty.CustomProperty");
        //        default:
        //            throw new ArgumentException("Unknown AtomSyndicationItemProperty enum value", "itemProperty");
        //    }
        //}

        public void WriteMappingModel(XmlWriter writer)
        {
            writer.WriteStartElement("Mapping", MappingNs);
            writer.WriteAttributeString("Space", "C-S");

            writer.WriteStartElement("Alias", MappingNs);
            writer.WriteAttributeString("Key", "CNs");
            writer.WriteAttributeString("Value", this.Namespace);
            writer.WriteEndElement();

            writer.WriteStartElement("Alias", MappingNs);
            writer.WriteAttributeString("Key", "SNs");
            writer.WriteAttributeString("Value", this.Namespace + ".Storage");
            writer.WriteEndElement();

            foreach (var container in this.Containers)
            {
                writer.WriteStartElement("EntityContainerMapping", MappingNs);
                writer.WriteAttributeString("CdmEntityContainer", container.Name);
                writer.WriteAttributeString("StorageEntityContainer", container.Name + "Storage");
                foreach (var entitySet in container.EntitySets)
                {
                    writer.WriteStartElement("EntitySetMapping", MappingNs);
                    writer.WriteAttributeString("Name", entitySet.Name);
                    if (entitySet.Type.BaseType == null && entitySet.Type.DerivedTypes.Count == 0)
                    {
                        writer.WriteAttributeString("TypeName", entitySet.Type.FullName);
                        writer.WriteAttributeString("StoreEntitySet", entitySet.Name + "Table");
                        foreach (var property in entitySet.Type.SelfAndBaseTypes.SelectMany((s) => s.Properties))
                        {
                            WriteMappingProperty(writer, property, "");
                        }
                    }
                    else
                    {
                        foreach (AdHocEntityType type in entitySet.Type.SelfAndDerivedTypes)
                        {
                            writer.WriteStartElement("EntityTypeMapping");
                            if (type.DerivedTypes.Count != 0)
                            {
                                writer.WriteAttributeString("TypeName", "IsTypeOf(" + type.FullName + ")");
                            }
                            else
                            {
                                writer.WriteAttributeString("TypeName", type.FullName);
                            }

                            writer.WriteStartElement("MappingFragment");
                            writer.WriteAttributeString("StoreEntitySet", entitySet.Name + type.Name + "Table");
                            foreach (var property in type.Properties)
                            {
                                WriteMappingProperty(writer, property, "");
                            }
                            foreach (AdHocEntityType baseType in type.SelfAndBaseTypes)
                            {
                                if (baseType == type)
                                {
                                    continue;
                                }

                                if (baseType.KeyProperties != null)
                                {
                                    foreach (var property in baseType.KeyProperties)
                                    {
                                        WriteMappingProperty(writer, property, "");
                                    }
                                }
                            }
                            writer.WriteEndElement();
                            writer.WriteEndElement();
                        }
                    }
                    writer.WriteEndElement();
                }

                foreach (var associationSet in container.AssociationSets)
                {
                    writer.WriteStartElement("AssociationSetMapping");
                    writer.WriteAttributeString("Name", associationSet.Name);
                    writer.WriteAttributeString("TypeName", "CNs." + associationSet.Type.Name);
                    writer.WriteAttributeString("StoreEntitySet",
                        TableNameForEntitySetAndType(associationSet.ManyEnd.EntitySet, associationSet.ManyEnd.EndType.Type));
                    foreach (var end in associationSet.Ends)
                    {
                        writer.WriteStartElement("EndProperty");
                        writer.WriteAttributeString("Name", end.EndType.RoleName);
                        foreach (var property in end.EndType.Type.CalculatedKeyProperties)
                        {
                            if (end.EndType.Multiplicity == "*")
                            {
                                WriteMappingProperty(writer, property, "");
                            }
                            else
                            {
                                WriteMappingProperty(writer, property, associationSet.Type.Name);
                            }
                        }
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        /// <summary>Writes the SSDL document for this model.</summary>
        /// <param name="writer">Writer to write XML to.</param>
        /// <remarks>Inheritance is implemented with table-per-type (TPT).</remarks>
        public void WriteStorageModel(XmlWriter writer)
        {
            TestUtil.CheckArgumentNotNull(writer, "writer");
            writer.WriteStartElement("Schema", StorageNs);
            writer.WriteAttributeString("Alias", "Self");
            writer.WriteAttributeString("Namespace", this.Namespace + ".Storage");
            writer.WriteAttributeString("Provider", "System.Data.SqlClient");
            writer.WriteAttributeString("ProviderManifestToken", "2005");
            foreach (var container in this.Containers)
            {
                writer.WriteStartElement("EntityContainer", StorageNs);
                writer.WriteAttributeString("Name", container.Name + "Storage");
                foreach (var entitySet in container.EntitySets)
                {
                    if (entitySet.Type.BaseType == null && entitySet.Type.DerivedTypes.Count == 0)
                    {
                        WriteStorageEntitySet(writer, entitySet.Name, entitySet.Type.Name);
                    }
                    else
                    {
                        foreach (AdHocEntityType type in entitySet.Type.SelfAndDerivedTypes)
                        {
                            WriteStorageEntitySet(writer, entitySet.Name + type.Name, type.Name);
                        }
                    }
                }
                writer.WriteEndElement();
            }

            foreach (var type in this.EntityTypes)
            {
                writer.WriteStartElement("EntityType", StorageNs);
                writer.WriteAttributeString("Name", type.Name);

                writer.WriteStartElement("Key", StorageNs);
                foreach (var key in type.CalculatedKeyProperties)
                {
                    writer.WriteStartElement("PropertyRef");
                    writer.WriteAttributeString("Name", key.Name);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();

                foreach (var property in type.Properties)
                {
                    this.WriteStorageProperty(writer, property, "");
                }

                // Include the keys from the base type.
                if (type.BaseType != null)
                {
                    foreach (var baseType in type.SelfAndBaseTypes)
                    {
                        if (baseType.KeyProperties == null) continue;
                        foreach (var property in baseType.KeyProperties)
                        {
                            this.WriteStorageProperty(writer, property, "");
                        }
                    }
                }

                // Include the keys for assocations if it's on the many-side of relationships.
                foreach (var associationType in this.Containers.SelectMany((c) => c.AssociationTypes).Where((a) => a.ManyEnd != null && a.ManyEnd.Type == type))
                {
                    foreach (var property in associationType.OneEnd.Type.CalculatedKeyProperties)
                    {
                        WriteStorageProperty(writer, property, associationType.Name.Replace(".", ""));
                    }
                }

                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        private XmlWriterSettings WriterSettings
        {
            get
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                return settings;
            }
        }

        private string TableNameForEntitySetAndType(AdHocEntitySet entitySet, AdHocEntityType type)
        {
            if (entitySet.Type.BaseType == null && entitySet.Type.DerivedTypes.Count == 0)
            {
                return entitySet.Name + "Table";
            }
            else
            {
                return entitySet.Name + type.Name + "Table";
            }
        }

        private void WriteMappingProperty(XmlWriter writer, AdHocProperty property, string prefix)
        {
            if (property is AdHocScalarProperty)
            {
                AdHocStructuralType structuralType = property.Type as AdHocStructuralType;
                if (structuralType == null)
                {
                    writer.WriteStartElement("ScalarProperty", MappingNs);
                    writer.WriteAttributeString("Name", property.Name);
                    writer.WriteAttributeString("ColumnName", prefix + property.Name);
                    writer.WriteEndElement();
                }
                else
                {
                    writer.WriteStartElement("ComplexProperty", MappingNs);
                    writer.WriteAttributeString("Name", property.Name);
                    writer.WriteAttributeString("TypeName", structuralType.FullName);
                    foreach (var p in structuralType.Properties)
                    {
                        WriteMappingProperty(writer, p, structuralType.Name);
                    }
                    writer.WriteEndElement();
                }
            }
        }

        private void WriteStorageEntitySet(XmlWriter writer, string tableName, string typeName)
        {
            writer.WriteStartElement("EntitySet", StorageNs);
            writer.WriteAttributeString("Name", tableName + "Table");
            writer.WriteAttributeString("EntityType", "Self." + typeName);
            writer.WriteAttributeString("Schema", "dbo");
            writer.WriteAttributeString("Table", tableName);
            writer.WriteEndElement();
        }

        private void WriteStorageProperty(XmlWriter writer, AdHocProperty property, string prefix)
        {
            AdHocScalarProperty scalar = property as AdHocScalarProperty;
            if (scalar == null) return;

            AdHocStructuralType structuralType = property.Type as AdHocStructuralType;
            if (structuralType == null)
            {
                writer.WriteStartElement("Property");
                writer.WriteAttributeString("Name", prefix + scalar.Name);
                writer.WriteAttributeString("Type", scalar.Type.StorageName);
                if (scalar.Type.StorageName == "nvarchar")
                {
                    writer.WriteAttributeString("MaxLength", "128");
                }
                writer.WriteAttributeString("Nullable", scalar.IsNullable ? "true" : "false");
                writer.WriteEndElement();
            }
            else
            {
                foreach (var p in structuralType.Properties)
                {
                    WriteStorageProperty(writer, p, structuralType.Name);
                }
            }
        }

        public string ConceptualNs { get; set; }
        private const string MappingNs = "urn:schemas-microsoft-com:windows:storage:mapping:CS";
        private const string StorageNs = "http://schemas.microsoft.com/ado/2006/04/edm/ssdl";
    }
}
