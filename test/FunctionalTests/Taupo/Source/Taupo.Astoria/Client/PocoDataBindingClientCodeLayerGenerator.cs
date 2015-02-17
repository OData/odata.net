//---------------------------------------------------------------------
// <copyright file="PocoDataBindingClientCodeLayerGenerator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Client
{
    using System;
    using System.CodeDom;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.Client;
    using Microsoft.Test.Taupo.Astoria.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.CodeDomExtensions;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.Types;

    /// <summary>
    /// Client Object layer generator for use in strongly-typed scenarios where types with DataBinding interfaces implemented are needed
    /// </summary>
    [ImplementationName(typeof(IClientCodeLayerGenerator), "Binding", HelpText = "Uses locally generated poco classes with DataBinding interfaces implemented")]
    public class PocoDataBindingClientCodeLayerGenerator : PocoClientCodeLayerGenerator
    {
        /// <summary>
        /// Gets the list of namespaces to import
        /// </summary>
        /// <returns>The namespaces needed for the objects</returns>
        protected override IList<string> GetNamespaceImports()
        {
            var list = base.GetNamespaceImports();
            list.Add("System.ComponentModel"); // for INotifyPropertyChanged
            list.Add("System.Collections.ObjectModel"); // for ObservableCollection<T>
            return list;
        }

        /// <summary>
        /// Fills in the given type declaration based on the given metadata and implements INotifyPropertyChanged on the type
        /// </summary>
        /// <param name="complexType">The complex type's metadata</param>
        /// <param name="complexTypeClass">The type declaration</param>
        protected override void DeclareComplexType(ComplexType complexType, CodeTypeDeclaration complexTypeClass)
        {
            base.DeclareComplexType(complexType, complexTypeClass);
            this.ImplementINotifyPropertyChanged(complexTypeClass);
        }

        /// <summary>
        /// Fills in the given type declaration based on the given metadata and implements INotifyPropertyChanged on the type
        /// </summary>
        /// <param name="entityType">The entity type's metadata</param>
        /// <param name="entityTypeClass">The type declaration</param>
        protected override void DeclareEntityType(EntityType entityType, CodeTypeDeclaration entityTypeClass)
        {
            base.DeclareEntityType(entityType, entityTypeClass);

            // if an entity type is a derived type, the base type should have implemented INotifyPropertyChanged
            if (entityType.BaseType == null)
            {
                this.ImplementINotifyPropertyChanged(entityTypeClass);
            }
        }

        /// <summary>
        /// Adds a property to the given type declaration based on the given metadata
        /// </summary>
        /// <param name="memberProperty">The property's metadata</param>
        /// <param name="parentClass">The type declaration</param>
        protected override void DeclareMemberProperty(MemberProperty memberProperty, CodeTypeDeclaration parentClass)
        {
            if (memberProperty.IsStream())
            {
                this.DeclareNamedStreamProperty(memberProperty, parentClass);
            }
            else
            {
                this.AddPropertyWithChangeNotification(parentClass, this.GetPropertyType(memberProperty, CodeGenerationTypeUsage.Declaration), memberProperty.Name);
            }
        }

        /// <summary>
        /// Adds a navigation property to the given type declaration based on the given metadata
        /// If the multiplicity is One, then we add a clr property which fires property change notifications
        /// </summary>
        /// <param name="navigationProperty">The property's metadata</param>
        /// <param name="parentClass">The type declaration</param>
        protected override void DeclareNavigationProperty(NavigationProperty navigationProperty, CodeTypeDeclaration parentClass)
        {
            if (navigationProperty.ToAssociationEnd.Multiplicity == EndMultiplicity.Many)
            {
                base.DeclareNavigationProperty(navigationProperty, parentClass);
            }
            else
            {
                this.AddPropertyWithChangeNotification(parentClass, this.GetPropertyType(navigationProperty, CodeGenerationTypeUsage.Declaration), navigationProperty.Name);
            }
        }

        /// <summary>
        /// Adds a property to the given type which raises PropertyChange Notifications when the property is set.
        /// </summary>
        /// <param name="type">The type to add property to.</param>
        /// <param name="propertyType">Type of the property.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>Added property.</returns>
        protected CodeMemberProperty AddPropertyWithChangeNotification(CodeTypeDeclaration type, CodeTypeReference propertyType, string propertyName)
        {
            // Declare a private property to be the backing field
            string backingFieldName = string.Format(CultureInfo.InvariantCulture, "backing{0}", propertyName);
            type.AddField(propertyType, backingFieldName);
            CodeMemberProperty propertyWithChangeNotifications = new CodeMemberProperty()
            {
                Name = propertyName,
                Type = propertyType,
                Attributes = MemberAttributes.Public
            };
            CodeThisReferenceExpression thisReference = new CodeThisReferenceExpression();

            // this will hold the reference to the backing field
            CodeFieldReferenceExpression backingFieldReference = new CodeFieldReferenceExpression(thisReference, backingFieldName);

            // add a getter which returns the backing field
            propertyWithChangeNotifications.GetStatements.Add(new CodeMethodReturnStatement(backingFieldReference));

            // add a statement which sets the value of the backing field
            CodeAssignStatement setStatement = new CodeAssignStatement(backingFieldReference, new CodeArgumentReferenceExpression("value"));

            // This will call the RaisePropertyChanged method passing in the propertyName
            CodeMethodInvokeExpression invokePropertyChangeNotifier = new CodeMethodInvokeExpression(thisReference, "RaisePropertyChanged", new CodeExpression[] { new CodePrimitiveExpression(propertyName) });

            propertyWithChangeNotifications.SetStatements.Add(setStatement);
            propertyWithChangeNotifications.SetStatements.Add(invokePropertyChangeNotifier);
            type.Members.Add(propertyWithChangeNotifications);
            return propertyWithChangeNotifications;
        }

        /// <summary>
        /// Adds an implementation of the INotifyPropertyChanged interface to the given type.
        /// </summary>
        /// <param name="type">The type declaration</param>
        /// <returns>Type with INotifyPropertyChanged implemented.</returns>
        protected CodeTypeDeclaration ImplementINotifyPropertyChanged(CodeTypeDeclaration type)
        {
            //// This method will implement the INotifyPropertyChanged interface 
            //// Here's an example :
            //// public class Customer :INotifyPropertyChanged {
            ////      public PropertyChangedEventHandler PropertyChanged;
            ////      public void RaisePropertyChanged(string propertyName) {
            ////          if( this.PropertyChanged !=null ) {
            ////              this.PropertyChanged (this, new PropertyChangedEventArgs( propertyName ) );
            ////          }
            ////      }
            //// }
            CodeThisReferenceExpression thisReference = new CodeThisReferenceExpression();
            CodeTypeReference notifyPropertyChanged = Code.TypeRef(typeof(INotifyPropertyChanged));

            // Add the implements INotifyPropertyChanged statement
            // public class Customer :INotifyPropertyChanged {
            type.BaseTypes.Add(notifyPropertyChanged);

            // Add the PropertyChanged event as a field to the type
            // public PropertyChangedEventHandler PropertyChanged;
            CodeMemberEvent propertyChangedEvent = type.AddEvent(Code.TypeRef(typeof(PropertyChangedEventHandler)), "PropertyChanged");
            propertyChangedEvent.ImplementationTypes.Add(notifyPropertyChanged);

            // this.PropertyChanged
            CodeEventReferenceExpression eventFieldReference = new CodeEventReferenceExpression(thisReference, "PropertyChanged");

            // Add the RaisePropertyChanged Method which will invoke the PropertyChanged handler whenever a property value changes
            // if( this.PropertyChanged !=null )
            CodeConditionStatement invokeEventHandlersIfAny = new CodeConditionStatement(new CodeBinaryOperatorExpression(eventFieldReference, CodeBinaryOperatorType.IdentityInequality, new CodePrimitiveExpression(null)));

            // this.PropertyChanged (this, new PropertyChangedEventArgs( propertyName ) );
            invokeEventHandlersIfAny.TrueStatements.Add(
                new CodeDelegateInvokeExpression(eventFieldReference, new CodeExpression[] { thisReference, new CodeObjectCreateExpression("PropertyChangedEventArgs", new CodeArgumentReferenceExpression("propertyName")) }));

            // public void RaisePropertyChanged
            CodeMemberMethod method = type.AddMethod("RaisePropertyChanged", MemberAttributes.Public);

            method.Parameters.Add(new CodeParameterDeclarationExpression("System.String", "propertyName"));

            ////      public void RaisePropertyChanged(string propertyName) {
            ////          if( this.PropertyChanged !=null ) {
            ////              this.PropertyChanged (this, new PropertyChangedEventArgs( propertyName ) );
            ////          }
            ////      }
            method.Statements.Add(invokeEventHandlersIfAny);
            return type;
        }

        /// <summary>
        /// Gets a type reference to use when declaring or instantiation a collection property
        /// </summary>
        /// <param name="usage">Whether the type is for declaration or instantiation</param>
        /// <param name="propertyAnnotations">The annotations of the property</param>
        /// <param name="elementDataType">The element type of the collection</param>
        /// <returns>The type of the property</returns>
        protected override CodeTypeReference GetCollectionType(CodeGenerationTypeUsage usage, IEnumerable<Annotation> propertyAnnotations, DataType elementDataType)
        {
            return Code.GenericType("ObservableCollection", this.BackingTypeResolver.ResolveClrTypeReference(elementDataType));
        }
    }
}
