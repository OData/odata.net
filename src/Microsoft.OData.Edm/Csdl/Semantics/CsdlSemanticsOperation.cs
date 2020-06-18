//---------------------------------------------------------------------
// <copyright file="CsdlSemanticsOperation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Vocabularies;
using Microsoft.OData.Edm.Vocabularies.V1;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
    /// <summary>
    /// Provides semantics for a CsdlOperation
    /// </summary>
    internal abstract class CsdlSemanticsOperation : CsdlSemanticsElement, IEdmOperation, IEdmFullNamedElement
    {
        private readonly string fullName;
        private readonly CsdlOperation operation;
        private readonly Cache<CsdlSemanticsOperation, IEdmPathExpression> entitySetPathCache = new Cache<CsdlSemanticsOperation, IEdmPathExpression>();
        private static readonly Func<CsdlSemanticsOperation, IEdmPathExpression> ComputeEntitySetPathFunc = (me) => me.ComputeEntitySetPath();

        private readonly Cache<CsdlSemanticsOperation, IEdmOperationReturn> returnCache = new Cache<CsdlSemanticsOperation, IEdmOperationReturn>();
        private static readonly Func<CsdlSemanticsOperation, IEdmOperationReturn> ComputeReturnFunc = (me) => me.ComputeReturn();

        private readonly Cache<CsdlSemanticsOperation, IEnumerable<IEdmOperationParameter>> parametersCache = new Cache<CsdlSemanticsOperation, IEnumerable<IEdmOperationParameter>>();
        private static readonly Func<CsdlSemanticsOperation, IEnumerable<IEdmOperationParameter>> ComputeParametersFunc = (me) => me.ComputeParameters();

        public CsdlSemanticsOperation(CsdlSemanticsSchema context, CsdlOperation operation)
            : base(operation)
        {
            this.Context = context;
            this.operation = operation;
            this.fullName = EdmUtil.GetFullNameForSchemaElement(this.Context?.Namespace, this.operation?.Name);
        }

        public abstract EdmSchemaElementKind SchemaElementKind { get; }

        public override CsdlSemanticsModel Model
        {
            get { return this.Context.Model; }
        }

        public string Name
        {
            get { return this.operation.Name; }
        }

        /// <summary>
        /// Gets the full name of this schema element.
        /// </summary>
        public string FullName
        {
            get { return this.fullName; }
        }

        public override CsdlElement Element
        {
            get { return this.operation; }
        }

        public string Namespace
        {
            get { return this.Context.Namespace; }
        }

        public bool IsBound
        {
            get { return this.operation.IsBound; }
        }

        public IEdmPathExpression EntitySetPath
        {
            get
            {
                return this.entitySetPathCache.GetValue(this, ComputeEntitySetPathFunc, null);
            }
        }

        public IEdmTypeReference ReturnType
        {
            get
            {
                if (this.operation.Return == null)
                {
                    return null;
                }

                return Return.Type;
            }
        }

        public IEdmOperationReturn Return
        {
            get { return this.returnCache.GetValue(this, ComputeReturnFunc, null); }
        }

        public IEnumerable<IEdmOperationParameter> Parameters
        {
            get { return this.parametersCache.GetValue(this, ComputeParametersFunc, null); }
        }

        public CsdlSemanticsSchema Context { get; private set; }

        public IEdmOperationParameter FindParameter(string name)
        {
            return this.Parameters.SingleOrDefault(p => p.Name == name);
        }

        internal static string ParameterizedTargetName(IList<IEdmOperationParameter> parameters)
        {
            int index = 0;
            int parameterCount = parameters.Count;

            StringBuilder sb = new StringBuilder("(");
            foreach (IEdmOperationParameter parameter in parameters)
            {
                string typeName = "";
                if (parameter.Type == null)
                {
                    typeName = CsdlConstants.TypeName_Untyped;
                }
                else if (parameter.Type.IsCollection())
                {
                    typeName = CsdlConstants.Value_Collection + "(" + parameter.Type.AsCollection().ElementType().FullName() + ")";
                }
                else if (parameter.Type.IsEntityReference())
                {
                    typeName = CsdlConstants.Value_Ref + "(" + parameter.Type.AsEntityReference().EntityType().FullName() + ")";
                }
                else
                {
                    typeName = parameter.Type.FullName();
                }

                sb.Append(typeName);
                index++;
                if (index < parameterCount)
                {
                    sb.Append(", ");
                }
            }

            sb.Append(")");
            return sb.ToString();
        }

        protected override IEnumerable<IEdmVocabularyAnnotation> ComputeInlineVocabularyAnnotations()
        {
            return this.Model.WrapInlineVocabularyAnnotations(this, this.Context);
        }

        private IEdmPathExpression ComputeEntitySetPath()
        {
            if (this.operation.EntitySetPath != null)
            {
                return new OperationPathExpression(this.operation.EntitySetPath) { Location = this.Location };
            }

            return null;
        }

        private IEdmOperationReturn ComputeReturn()
        {
            if (this.operation.Return == null)
            {
                return null;
            }

            return new CsdlSemanticsOperationReturn(this, this.operation.Return);
        }

        private IEnumerable<IEdmOperationParameter> ComputeParameters()
        {
            List<IEdmOperationParameter> parameters = new List<IEdmOperationParameter>();

            foreach (var parameter in this.operation.Parameters)
            {
                if (parameter.IsOptional)
                {
                    parameters.Add(new CsdlSemanticsOptionalParameter(this, parameter, parameter.DefaultValue));
                }
                else
                {
                    parameters.Add(new CsdlSemanticsOperationParameter(this, parameter));
                }
            }

            // Handle the out-of-line optional parameter annotation for parameters.
            // First, use the above built parameters to create the full target name, for example: NS.TestFunction(Edm.String, Edm.String, Edm.String)
            // Then, go through each parameters by visiting the out-of-line optional parameter annotation.
            // If we find at least one of out-of-line optional parameter annotation, we replace it as an optional parameter.
            // Otherwise, re-use the built parameter.
            // Be noted: if a parameter has inline and out-of-line optional parameter, the out-of-line will win.
            string fullName = Namespace + "." + Name;
            string fullParametersName = ParameterizedTargetName(parameters);

            List<IEdmOperationParameter> newParameters = new List<IEdmOperationParameter>(parameters.Count);
            foreach (var parameter in parameters)
            {
                // for example: NS.TestFunction(Edm.String, Edm.String, Edm.String)/OptionalParameter
                string fullTargetName = fullName + fullParametersName + "/" + parameter.Name;

                // for example: NS.TestFunction/OptionalParameter
                string targetName = fullName + "/" + parameter.Name;

                string defaultValue;

                if (TryGetOptionalParameterOutOfLineAnnotation(fullTargetName, targetName, out defaultValue))
                {
                    CsdlSemanticsOperationParameter csdlSemanticsParameter = (CsdlSemanticsOperationParameter)parameter;
                    newParameters.Add(new CsdlSemanticsOptionalParameter(this, (CsdlOperationParameter)csdlSemanticsParameter.Element, defaultValue));
                }
                else
                {
                    newParameters.Add(parameter);
                }
            }

            return newParameters;
        }

        private bool TryGetOptionalParameterOutOfLineAnnotation(string fullTargetName, string targetName, out string defaultValue)
        {
            defaultValue = null;
            bool isOptional = false;

            List<CsdlSemanticsAnnotations> annotations;

            // OData 4.0 applies annotations to all overloads of a function or action.
            // We still need to support annotating the non-overloaded version.
            // So, we should probably first check the fullTargetName and, if that doesn't match, check just the function or action name.
            bool found = Model.OutOfLineAnnotations.TryGetValue(fullTargetName, out annotations) ? true :
                Model.OutOfLineAnnotations.TryGetValue(targetName, out annotations);

            if (found)
            {
                foreach (var annotation in annotations)
                {
                    var optionalParameterAnnotation = annotation.Annotations.Annotations.FirstOrDefault(a =>
                        a.Term == CoreVocabularyModel.OptionalParameterTerm.ShortQualifiedName() ||
                        a.Term == CoreVocabularyModel.OptionalParameterTerm.FullName());

                    if (optionalParameterAnnotation != null)
                    {
                        isOptional = true;

                        CsdlRecordExpression optionalValueExpression = optionalParameterAnnotation.Expression as CsdlRecordExpression;
                        if (optionalValueExpression != null)
                        {
                            foreach (CsdlPropertyValue property in optionalValueExpression.PropertyValues)
                            {
                                if (property.Property == CsdlConstants.Attribute_DefaultValue)
                                {
                                    CsdlConstantExpression propertyValue = property.Expression as CsdlConstantExpression;
                                    if (propertyValue != null)
                                    {
                                        defaultValue = propertyValue.Value;
                                    }
                                }
                            }
                        }

                        break;
                    }
                }
            }

            return isOptional;
        }

        private sealed class OperationPathExpression : EdmPathExpression, IEdmLocatable
        {
            internal OperationPathExpression(string path) : base(path)
            {
            }

            public EdmLocation Location { get; set; }
        }
    }
}
