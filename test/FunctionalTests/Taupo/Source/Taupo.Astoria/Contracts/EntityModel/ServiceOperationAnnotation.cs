//---------------------------------------------------------------------
// <copyright file="ServiceOperationAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.EntityModel
{
    using System.Runtime.Serialization;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    
    /// <summary>
    /// Annotation used to determine type of action
    /// </summary>
    public class ServiceOperationAnnotation : CompositeAnnotation
    {
        /// <summary>
        /// Initializes a new instance of the ServiceOperationAnnotation class.
        /// </summary>
        public ServiceOperationAnnotation()
        {
            this.SideEffecting = false;
            this.Composable = true;
            this.BatchSafe = true;
            this.EntitySetPath = null;
            this.BindingKind = OperationParameterBindingKind.Always;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Action is a Procedure or not
        /// </summary>
        public bool IsAction
        {
            get
            {
                return this.SideEffecting;
            }

            set
            {
                if (value)
                {
                    this.Composable = false;
                    this.SideEffecting = true;
                    this.BatchSafe = false;
                }
                else
                {
                    this.Composable = true;
                    this.SideEffecting = false;
                    this.BatchSafe = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets which type of binding kind on an Action is specified
        /// </summary>
        public OperationParameterBindingKind BindingKind { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether the action is side affecting by default it is false
        /// </summary>
        public bool SideEffecting { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether action is composable by default it is true
        /// </summary>
        public bool Composable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether action is batchable by default it is true
        /// </summary>
        public bool BatchSafe { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the FunctionImport m:EntitySetPath value
        /// </summary>
        public string EntitySetPath { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the EntitySet of a given ServiceOperation
        /// </summary>
        public string EntitySetName { get; set; }
    }
}