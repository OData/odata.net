//---------------------------------------------------------------------
// <copyright file="ScanModel.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Test.ModuleCore;
using Microsoft.Test.KoKoMo;


namespace System.Data.Test.Astoria
    {
    ////////////////////////////////////////////////////////
    //  Scan Model
    //
    ////////////////////////////////////////////////////////   
    public class ScanModel : ExpressionModel
        {
        protected ResourceContainer _containerResult;
        protected new ResourceType _result;
        protected Workspace _w;
        protected bool _canInsert = false;
        protected bool _canDelete = false;

        public ResourceContainer ResultContainer
            {
            get { return _containerResult; }
            set { _containerResult = value; }
            }

        public new ResourceType Result
            {
            get { return _result; }
            set { _result = value; }
            }

        public Workspace Workspace
            {
            get { return _w; }
            }

        public bool CanInsert
            {
            get { return _canInsert; }
            set { _canInsert = value; }
            }

        public bool CanDelete
            {
            get { return _canDelete; }
            set { _canDelete = value; }
            }

        //Constructor
        public ScanModel(Workspace w)
            {
            _w = w;
            }

        [ModelAction]
        public void ChooseFrom()
            {
            bool bFound = false;
            while (!bFound)
                {
                ResourceContainer container = _w.ServiceContainer.ResourceContainers.Choose();

                if (container.BaseType.ClrType.IsAbstract == true) { continue; };
                if (container.Name.Contains( "Failure" )) { continue; }
                if (container.Name.Contains( "CustomerDemographics" )) { continue; }
                if (container.Name.Contains( "DataKey_Bit" )) { continue; }
                if (container.Name.Contains( "Baseline" )) { continue; }
                if (container.Name.Contains( "Uninvestigated" )) { continue; }

                //check for CanInsert/CanDelete
                ResourceType rt = container.ResourceTypes.FirstOrDefault();
                this.Result = rt;
                this.ResultContainer = container;
                this.CanInsert = CanInsertResource( rt );
                this.CanDelete = CanDeleteResource( rt );
                bFound = true;
                }
            this.Disabled = true;  //Done 
            }

        //Overrides
        public override ExpressionModel CreateModel()
            {
            return new ScanModel( this.Workspace );
            }

        public static bool CanInsertResource(ResourceType resourceType)
            {
            if (!resourceType.IsInsertable)
                return false;
            if (resourceType.IsAssociationEntity)
                return false;
            if (resourceType.IsChildRefEntity)
                return false;
            if (resourceType.AssociationsOneToOne.Where( a => a.Ends.Where( e => e.ResourceType.IsAssociationEntity ).Count() > 1 ).Count() > 1)
                return false;
            if (resourceType.AssociationsManyToOne.Count > 0)
                return false;
            if (resourceType.AssociationsManyToZeroToOne.Count > 0)
                return false;
            if (resourceType.AssociationsManyToMany.Count > 0)
                return false;
            if (resourceType.AssociationsOneToOne.Count > 0)
                return false;
            if (resourceType.AssociationsZeroOneToOne.Count > 0)
                return false;

            return true;
            }


        public static bool CanDeleteResource(ResourceType resourceType)
            {
            //if (resourceType.IsAssociationEntity)
            //    return false;
            //if (resourceType.IsChildRefEntity)
            //    return false;
            //if (resourceType.AssociationsZeroOneToOne.Count > 0)
            //    return false;
            //if (resourceType.AssociationsOneToOne.Count > 0)
            //    return false;
            //if (resourceType.AssociationsOneToMany.Count > 0)
            //    return false;
            //if (resourceType.AssociationsZeroOneToMany.Count > 0)
            //    return false;

            if (resourceType.AssociationsZeroOneToMany.Count > 0)
                return false;
            if (resourceType.AssociationsOneToMany.Count > 0)
                return false;
            if (resourceType.AssociationsOneToOne.Count > 0)
                return false;
            if (resourceType.AssociationsOneToZeroOne.Count > 0)
                return false;
            if (resourceType.AssociationsZeroOneToZeroOne.Count > 0)
                return false;
            if (resourceType.AssociationsManyToMany.Count > 0)
                return false;

            //bool skip = false;
            //foreach (ResourceAssociation resourceAssocation in resourceType.Associations)
            //{
            //    foreach (ResourceAssociationEnd end in resourceAssocation.Ends)
            //    {
            //        if (end.ResourceType.IsAssociationEntity)
            //        {
            //            skip = true;
            //            break;
            //        }
            //    }
            //    if (skip)
            //        break;
            //}

            //return skip;

            return true;
            }
        }
    }




