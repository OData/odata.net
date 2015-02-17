//---------------------------------------------------------------------
// <copyright file="AdoGuy.InMemory.Workspace.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace System.Data.Test.Astoria
{
    using System;
    using System.Collections.Generic;
    using System.Data.Test.Astoria;
    using System.IO;

    //[WorkspaceAttribute(DataLayerProviderKind = DataLayerProviderKind.InMemoryLinq, Name = "AdoGuyProvider", Priority = 2)]
    public class AdoGuyWorkspace : System.Data.Test.Astoria.InMemoryWorkspace
    {
        public AdoGuyWorkspace() : base("AdoGuy", "AdoGuyContext", "AdoGuyProvider")
        { this.Language = WorkspaceLanguage.CSharp; }

        public override ServiceContainer ServiceContainer
        {
            get
            {
                if (_serviceContainer == null)
                {
                    //Resource types here
                    ResourceType Games = Resource.ResourceType("Game", "AdoGuyContext",
                        Resource.Property("GameID", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("GameName", Clr.Types.String, NodeFacet.Nullable(true))
                    );

                    ResourceType GameConsoles = Resource.ResourceType("GameConsole", "AdoGuyContext",
                        Resource.Property("GameConsoleID", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("GameConsoleName", Clr.Types.String, NodeFacet.Nullable(true))
                    );

                    ResourceType Accessories = Resource.ResourceType("Accessory", "AdoGuyContext",
                        Resource.Property("AccessoryID", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("AccessoryName", Clr.Types.String, NodeFacet.Nullable(true))
                    );

                    ResourceType Products = Resource.ResourceType("Product", "AdoGuyContext",
                        Resource.Property("ProductID", Clr.Types.Int32, Resource.Key()),
                        Resource.Property("ProductName", Clr.Types.String, NodeFacet.Nullable(true))
                    );

                    //Explicity define Many to many relationships here
                    ResourceAssociationEnd ProductsRole = Resource.End("Products", Products, Multiplicity.One);
                    ResourceAssociationEnd GameConsolesRole = Resource.End("GameConsoles", GameConsoles, Multiplicity.Many);
                    ResourceAssociation FK_Products_GameConsoles = Resource.Association("FK_Products_GameConsoles", ProductsRole, GameConsolesRole);

                    //Resource navigation properties added here
                    Products.Properties.Add(Resource.Property("GameConsoles", Resource.Collection(GameConsoles), FK_Products_GameConsoles, ProductsRole, GameConsolesRole));
                    Products.Properties.Add(Resource.Property("ProductGame", Games));
                    Products.Properties.Add(Resource.Property("ProductAccessory", Accessories));

                    //Resource Containers added here
                    _serviceContainer = Resource.ServiceContainer(this, "AdoGuy",
                            Resource.ResourceContainer("Games", Games),
                            Resource.ResourceContainer("Accessories", Accessories),
                            Resource.ResourceContainer("GameConsoles", GameConsoles),
                            Resource.ResourceContainer("Products", Products)
                        );

                    foreach (ResourceContainer rc in _serviceContainer.ResourceContainers)
                    {
                        foreach (ResourceType t in rc.ResourceTypes)
                        {
                            t.InferAssociations();
                        }
                    }
                }
                return _serviceContainer;
            }
        }
        public override ResourceType LanguageDataResource()
        {
            return this.ServiceContainer.ResourceContainers["Games"].BaseType;
        }

    }
}
