//---------------------------------------------------------------------
// <copyright file="CodeGeneratingDataInserter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Data.Test.Astoria
{
    /// <summary>
    /// An implementation of IDataInserter using code generation of direct IUpdatable calls
    /// Requires:
    ///  - The service's IUpdatable file has a replace-able static method with the signature 'private static void AddData(IUpdatable updatable)' 
    ///  - AddData is called during the RestoreData service operation
    /// </summary>
    public class CodeGeneratingDataInserter : IDataInserter
    {
        #region Constructor
        /// <summary>
        /// Constructs a new CodeGeneratingDataInserter for the given workspace
        /// </summary>
        /// <param name="w">the worspace to add data to</param>
        public CodeGeneratingDataInserter(Workspace w)
        {
            if (w.DataLayerProviderKind != DataLayerProviderKind.InMemoryLinq && w.DataLayerProviderKind != DataLayerProviderKind.NonClr)
                AstoriaTestLog.FailAndThrow("CodeGeneratingDataInserter only supported for InMemory and NonClr workspaces");
            workspace = w;
        }
        #endregion

        #region private fields
        /// <summary>
        /// Name of the server-side list used to store the entities
        /// </summary>
        private const string entityListName = "_generatedEntities";

        /// <summary>
        /// Used to build the code
        /// </summary>
        private StringBuilder code = new StringBuilder();

        /// <summary>
        /// Used to store the methods we generated to break up the code
        /// </summary>
        private Dictionary<string, string> intermediateMethods = new Dictionary<string, string>();

        /// <summary>
        /// Used to track the indexes for each entity
        /// </summary>
        private Dictionary<KeyedResourceInstance, int> entityIDs = new Dictionary<KeyedResourceInstance, int>();

        /// <summary>
        /// The workspace to add the code to
        /// </summary>
        private Workspace workspace;

        /// <summary>
        /// Whether or not the codegen contains a final IUpdatable.SaveChanges call
        /// </summary>
        private bool changesSaved = false;
        #endregion

        #region IDataInserter Members

        /// <summary>
        /// Event fired when AddEntity is called
        /// </summary>
        public event Action<KeyExpression, KeyedResourceInstance> OnAddingEntity;

        /// <summary>
        /// Event fired when AddAssociation is called
        /// </summary>
        public event Action<KeyedResourceInstance, ResourceProperty, KeyedResourceInstance> OnAddingAssociation;

        /// <summary>
        /// Because the generated code is part of the service implementation, this always returns true
        /// </summary>
        public bool BeforeServiceCreation
        {
            get { return true; }
        }

        /// <summary>
        /// Implements IDataInserter.AddAssociation by generating the appropriate IUpdatable.AddReferenceToCollection or IUpdatable.SetReference call
        /// </summary>
        /// <param name="parent">Update tree of parent, must have already been added using AddEntity</param>
        /// <param name="property">Navigation property for the association</param>
        /// <param name="child">Update tree of child, must have already been added using AddEntity</param>
        public void AddAssociation(KeyedResourceInstance parent, ResourceProperty property, KeyedResourceInstance child)
        {
            // look up the object names for these entities
            //
            string parentObjectName = entityListName + "[" + entityIDs[parent] + "]";
            string childObjectName = entityListName + "[" + entityIDs[child] + "]";

            // code-gen the appropriate IUpdatable call
            //
            code.AppendLine("//Adding association");
            if (property.OtherAssociationEnd.Multiplicity == Multiplicity.Many)
                code.AppendLine("updatable.AddReferenceToCollection(" + parentObjectName + ", \"" + property.Name + "\", " + childObjectName + ");");
            else
                code.AppendLine("updatable.SetReference(" + parentObjectName + ", \"" + property.Name + "\", " + childObjectName + ");");
            code.AppendLine();

            // Fire the event
            //
            if (this.OnAddingAssociation != null)
                OnAddingAssociation(parent, property, child);

            // ensure that a later Flush/Close will write the SaveChanges call
            changesSaved = false;
        }

        /// <summary>
        /// Implements IDataInserter.AddEntity by generating a IUpdatable.CreateResource call and multiple IUpdatable.SetValue calls
        /// </summary>
        /// <param name="key">Key expression for new entity</param>
        /// <param name="entity">Update tree for new entity</param>
        public void AddEntity(KeyExpression key, KeyedResourceInstance entity)
        {
            // generate a variable name for this entity and save it
            //
            int entityID = entityIDs.Count;
            entityIDs[entity] = entityID;
            string entityObjectName = "entity" + entityID;

            // code-gen the CreateResource and SetValue calls
            //
            code.AppendLine("//Adding entity");
            foreach(string line in WriteObject(entityObjectName, entity))
                code.AppendLine(line);
            code.AppendLine(entityListName + ".Add(" + entityObjectName + ");");
            code.AppendLine(string.Empty);

            // Fire the event
            //
            if (this.OnAddingEntity != null)
                OnAddingEntity(key, entity);

            // ensure that a later Flush/Close will write the SaveChanges call
            changesSaved = false;
        }

        /// <summary>
        /// Implements IDataInserter.Close by:
        /// 1) Calling flush
        /// 2) replacing the service's AddData method with the generated code
        /// 3) scheduling a post-service-creation action to invoke the RestoreData service operation
        /// </summary>
        public void Close()
        {
            // make sure all SaveChanges calls have been added
            //
            Flush();
            
            // replace AddData with the generated code
            //
            List<string> lines = new List<string>(intermediateMethods.Select(pair => "    " + pair.Key + "(updatable);"));
            lines.Insert(0, entityListName + ".Clear();");
            lines.Add(entityListName + ".Clear();");
            bool first = true;
            foreach (var pair in intermediateMethods)
            {
                lines.Add("}");
                lines.Add("");
                if (first)
                {
                    lines.Add("private static List<object> " + entityListName + " = new List<object>();");
                    lines.Add("");
                    first = false;
                }
                lines.Add("private static void " + pair.Key + "(IUpdatable updatable)");
                lines.Add("{");
                lines.Add(pair.Value);
            }
            // it will add the trailing }

            workspace.ServiceModifications.Interfaces.IUpdatable.ImplementingFile.AddMethod("AddData", new NewMethodInfo()
            {
                MethodSignature = "private static void AddData(IUpdatable updatable)",
                BodyText = string.Join(Environment.NewLine, lines.ToArray())
            });

            // invoke the RestoreData service operation after the service is created
            //
            workspace.AfterServiceCreation.Add(workspace.RestoreData);

            // clear IDs
            entityIDs.Clear();
        }

        /// <summary>
        /// Implements IDataInserter.Flush by code-generating an IUpdatable.SaveChanges call, if there are pending changes.
        /// Also clears the list of entity object names
        /// </summary>
        public void Flush()
        {
            // if there are pending changes, code-gen a SaveChanges call
            //
            if (!changesSaved)
            {
                code.AppendLine("// Saving changes");
                code.AppendLine("updatable.SaveChanges();");
                code.AppendLine(string.Empty);
                changesSaved = true;
            }

            // generate an intermediate method to store the operations so far
            string intermediateMethodName = "AddEntities_Intermediate" + intermediateMethods.Count;
            intermediateMethods.Add(intermediateMethodName, code.ToString());
            code = new StringBuilder();
        }
        #endregion

        #region private helper methods
        /// <summary>
        /// Writes the CreateResource and SetValue calls to create the given instance, recursing where necessary
        /// </summary>
        /// <param name="variableName">Name of variable to code-gen</param>
        /// <param name="instance">Entity/complex type to generate code for</param>
        /// <returns>generated lines of code</returns>
        private IEnumerable<string> WriteObject(string variableName, ComplexResourceInstance instance)
        {
            // determine the set and type for the CreateResource call
            //
            string typeName = workspace.ContextNamespace + "." + instance.TypeName;
            
            string setName;
            if (instance is KeyedResourceInstance)
                setName = '"' + (instance as KeyedResourceInstance).ResourceSetName + '"';
            else
                setName = "null";

            // return the CreateResource line
            //
            yield return "object " + variableName + " = updatable.CreateResource(" + setName + ", \"" + typeName + "\");";

            // if its an entity, and not a complex type, then generate the key properties as well
            //
            if (instance is KeyedResourceInstance)
            {
                foreach (ResourceInstanceSimpleProperty property in (instance as KeyedResourceInstance).KeyProperties.OfType<ResourceInstanceSimpleProperty>())
                {
                    yield return "updatable.SetValue(" + variableName + ", \"" + property.Name + "\", " + ConvertToCode((property as ResourceInstanceSimpleProperty).PropertyValue) + ");";
                }
            }

            // generate the SetValue calls for each property
            foreach (ResourceInstanceProperty property in instance.Properties)
            {
                // based on the type of property, generate the appropriate code
                //
                if (property is ResourceInstanceNavProperty)
                {
                    // not supported right now, this is non-trivial
                    throw new NotImplementedException();
                }
                else if (property is ResourceInstanceComplexProperty)
                {
                    // generate an instance of this property's value for passing to SetValue
                    //
                    string subVariableName = variableName + "_" + property.Name;

                    // recurse
                    foreach (string line in WriteObject(subVariableName, (property as ResourceInstanceComplexProperty).ComplexResourceInstance))
                        yield return line;

                    // return the SetValue call
                    //
                    yield return "updatable.SetValue(" + variableName + ", \"" + property.Name + "\", " + subVariableName + ");";
                }
                else if (property is ResourceInstanceSimpleProperty)
                {

                    // return the SetValue call
                    //
                    yield return "updatable.SetValue(" + variableName + ", \"" + property.Name + "\", " + ConvertToCode((property as ResourceInstanceSimpleProperty).PropertyValue) + ");";
                }
                else
                {
                    // should never hit this
                    throw new NotSupportedException();
                }   
            }
        }

        /// <summary>
        /// Returns code to instantiate an equivalent instance of the given value
        /// </summary>
        /// <param name="value">value to instantiate</param>
        /// <returns>code to instantiate the value</returns>
        private string ConvertToCode(object value)
        {
            Globalization.CultureInfo realCulture = Globalization.CultureInfo.CurrentCulture;

            // null is easy
            //
            if (value == null)
                return "null";

            try
            { 
                System.Threading.Thread.CurrentThread.CurrentCulture = Globalization.CultureInfo.InvariantCulture;

                // switch based on the type
                Type type = value.GetType();
                Type[] simpleTypes = new Type[] { typeof(bool), typeof(int) };
                Type[] castTypes = new Type[] { typeof(byte), typeof(short), typeof(ushort), typeof(sbyte) };

                if (simpleTypes.Contains(type))
                    return value.ToString().ToLower();
                else if (castTypes.Contains(type))
                    return "(" + type.FullName + ")(" + value.ToString() + ")";
                else if (type == typeof(uint))
                    return value.ToString() + "U";
                else if (type == typeof(long))
                    return value.ToString() + "L";
                else if (type == typeof(ulong))
                    return value.ToString() + "UL";
                else if (type == typeof(decimal))
                    return value.ToString() + "M";
                else if (type == typeof(float))
                {
                    if (float.IsNegativeInfinity((float)value))
                        return "float.NegativeInfinity";
                    if (float.IsPositiveInfinity((float)value))
                        return "float.PositiveInfinity";
                    if (float.IsNaN((float)value))
                        return "float.NaN";
                    if ((float)value == float.MaxValue)
                        return "float.MaxValue";
                    if ((float)value == float.MinValue)
                        return "float.MinValue";
                    return value.ToString() + "f";
                }
                else if (type == typeof(double))
                {
                    if (double.IsNegativeInfinity((double)value))
                        return "double.NegativeInfinity";
                    if (double.IsPositiveInfinity((double)value))
                        return "double.PositiveInfinity";
                    if (double.IsNaN((double)value))
                        return "double.NaN";
                    if ((double)value == double.MaxValue)
                        return "double.MaxValue";
                    if ((double)value == double.MinValue)
                        return "double.MinValue";
                    return value.ToString() + "d";
                }
                else if (type == typeof(string))
                    return "\"" + value + "\"";
                else if (type == typeof(byte[]))
                    return "new byte[] { " + string.Join(", ", (value as byte[]).Select(b => b.ToString()).ToArray()) + "}";
                else if (type == typeof(Guid))
                    return "new Guid(\"" + value.ToString() + "\")";
                else if (type == typeof(DateTime))
                {
                    DateTime dt = (DateTime)value;
                    return "new DateTime(" + dt.Ticks + ", DateTimeKind." + dt.Kind.ToString() + ")";
                }
                else
                {
                    // may need to add more cases in the future, for now this is sufficient
                    throw new NotSupportedException();
                }
            }
            finally
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = realCulture;
            }
        }

        #endregion
    }
}
