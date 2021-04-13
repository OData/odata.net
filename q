[1mdiff --git a/sln/OData.sln b/sln/OData.sln[m
[1mindex 66953c906..3a162cea6 100644[m
[1m--- a/sln/OData.sln[m
[1m+++ b/sln/OData.sln[m
[36m@@ -28,6 +28,8 @@[m [mProject("{9A19103F-16F7-4668-BE54-9A1E7A4F7556}") = "Microsoft.OData.Core.Tests"[m
 EndProject[m
 Project("{9A19103F-16F7-4668-BE54-9A1E7A4F7556}") = "Microsoft.OData.PublicApi.Tests", "..\test\PublicApiTests\Microsoft.OData.PublicApi.Tests.csproj", "{B33A9E4D-2DEA-489C-B04F-B4F1715B2342}"[m
 EndProject[m
[32m+[m[32mProject("{9A19103F-16F7-4668-BE54-9A1E7A4F7556}") = "Client-DependsOn", "..\..\Client-DependsOn\Client-DependsOn\Client-DependsOn.csproj", "{B82871E0-82E8-4F14-AE2C-21410BC690EC}"[m[41m[m
[32m+[m[32mEndProject[m[41m[m
 Global[m
 	GlobalSection(SolutionConfigurationPlatforms) = preSolution[m
 		Cover|Any CPU = Cover|Any CPU[m
[36m@@ -227,6 +229,24 @@[m [mGlobal[m
 		{B33A9E4D-2DEA-489C-B04F-B4F1715B2342}.Release|x64.Build.0 = Release|Any CPU[m
 		{B33A9E4D-2DEA-489C-B04F-B4F1715B2342}.Release|x86.ActiveCfg = Release|Any CPU[m
 		{B33A9E4D-2DEA-489C-B04F-B4F1715B2342}.Release|x86.Build.0 = Release|Any CPU[m
[32m+[m		[32m{B82871E0-82E8-4F14-AE2C-21410BC690EC}.Cover|Any CPU.ActiveCfg = Debug|Any CPU[m[41m[m
[32m+[m		[32m{B82871E0-82E8-4F14-AE2C-21410BC690EC}.Cover|Any CPU.Build.0 = Debug|Any CPU[m[41m[m
[32m+[m		[32m{B82871E0-82E8-4F14-AE2C-21410BC690EC}.Cover|x64.ActiveCfg = Debug|Any CPU[m[41m[m
[32m+[m		[32m{B82871E0-82E8-4F14-AE2C-21410BC690EC}.Cover|x64.Build.0 = Debug|Any CPU[m[41m[m
[32m+[m		[32m{B82871E0-82E8-4F14-AE2C-21410BC690EC}.Cover|x86.ActiveCfg = Debug|Any CPU[m[41m[m
[32m+[m		[32m{B82871E0-82E8-4F14-AE2C-21410BC690EC}.Cover|x86.Build.0 = Debug|Any CPU[m[41m[m
[32m+[m		[32m{B82871E0-82E8-4F14-AE2C-21410BC690EC}.Debug|Any CPU.ActiveCfg = Debug|Any CPU[m[41m[m
[32m+[m		[32m{B82871E0-82E8-4F14-AE2C-21410BC690EC}.Debug|Any CPU.Build.0 = Debug|Any CPU[m[41m[m
[32m+[m		[32m{B82871E0-82E8-4F14-AE2C-21410BC690EC}.Debug|x64.ActiveCfg = Debug|Any CPU[m[41m[m
[32m+[m		[32m{B82871E0-82E8-4F14-AE2C-21410BC690EC}.Debug|x64.Build.0 = Debug|Any CPU[m[41m[m
[32m+[m		[32m{B82871E0-82E8-4F14-AE2C-21410BC690EC}.Debug|x86.ActiveCfg = Debug|Any CPU[m[41m[m
[32m+[m		[32m{B82871E0-82E8-4F14-AE2C-21410BC690EC}.Debug|x86.Build.0 = Debug|Any CPU[m[41m[m
[32m+[m		[32m{B82871E0-82E8-4F14-AE2C-21410BC690EC}.Release|Any CPU.ActiveCfg = Release|Any CPU[m[41m[m
[32m+[m		[32m{B82871E0-82E8-4F14-AE2C-21410BC690EC}.Release|Any CPU.Build.0 = Release|Any CPU[m[41m[m
[32m+[m		[32m{B82871E0-82E8-4F14-AE2C-21410BC690EC}.Release|x64.ActiveCfg = Release|Any CPU[m[41m[m
[32m+[m		[32m{B82871E0-82E8-4F14-AE2C-21410BC690EC}.Release|x64.Build.0 = Release|Any CPU[m[41m[m
[32m+[m		[32m{B82871E0-82E8-4F14-AE2C-21410BC690EC}.Release|x86.ActiveCfg = Release|Any CPU[m[41m[m
[32m+[m		[32m{B82871E0-82E8-4F14-AE2C-21410BC690EC}.Release|x86.Build.0 = Release|Any CPU[m[41m[m
 	EndGlobalSection[m
 	GlobalSection(SolutionProperties) = preSolution[m
 		HideSolutionNode = FALSE[m
[1mdiff --git a/src/Microsoft.OData.Client/DataServiceContext.cs b/src/Microsoft.OData.Client/DataServiceContext.cs[m
[1mindex 9c1dac730..ddca933c2 100644[m
[1m--- a/src/Microsoft.OData.Client/DataServiceContext.cs[m
[1m+++ b/src/Microsoft.OData.Client/DataServiceContext.cs[m
[36m@@ -3946,17 +3946,11 @@[m [mnamespace Microsoft.OData.Client[m
 [m
             if (dependsOnObjects != null)[m
             {[m
[31m-                List<string> dependsOnIdsAsChangeOrders;[m
[31m-                HashSet<string> dependsOnChangeSetIds;[m
[31m-[m
[31m-                GetDependsOnChangeOrdersAndChangeSetIds([m
[31m-                    dependsOnObjects,[m
[31m-                    this.EntityTracker,[m
[31m-                    out dependsOnIdsAsChangeOrders,[m
[31m-                    out dependsOnChangeSetIds);[m
[31m-[m
[31m-                resource.DependsOnIds = dependsOnIdsAsChangeOrders;[m
[31m-                resource.DependsOnChangeSetIds = dependsOnChangeSetIds.ToList();[m
[32m+[m[32m                UpdateResourceWithDependsOnChangeOrdersAndChangeSetIds([m[41m[m
[32m+[m[32m                        resource,[m[41m[m
[32m+[m[32m                        dependsOnObjects,[m[41m[m
[32m+[m[32m                        this.EntityTracker,[m[41m[m
[32m+[m[32m                        out resource);[m[41m[m
             }[m
 [m
             resource.State = EntityStates.Modified;[m
[36m@@ -4012,17 +4006,11 @@[m [mnamespace Microsoft.OData.Client[m
 [m
                 if (dependsOnObjects != null)[m
                 {[m
[31m-                    List<string> dependsOnIdsAsChangeOrders;[m
[31m-                    HashSet<string> dependsOnChangeSetIds;[m
[31m-[m
[31m-                    GetDependsOnChangeOrdersAndChangeSetIds([m
[32m+[m[32m                    UpdateResourceWithDependsOnChangeOrdersAndChangeSetIds([m[41m[m
[32m+[m[32m                        resource,[m[41m[m
                         dependsOnObjects,[m
                         this.EntityTracker,[m
[31m-                        out dependsOnIdsAsChangeOrders,[m
[31m-                        out dependsOnChangeSetIds);[m
[31m-[m
[31m-                    resource.DependsOnIds = dependsOnIdsAsChangeOrders;[m
[31m-                    resource.DependsOnChangeSetIds = dependsOnChangeSetIds.ToList();[m
[32m+[m[32m                        out resource);[m[41m[m
                 }[m
 [m
                 // Leave related links alone which means we can have a link in the Added[m
[36m@@ -4048,21 +4036,24 @@[m [mnamespace Microsoft.OData.Client[m
             this.DeleteObjectInternal(entity, failIfInAddedState, null);[m
         }[m
 [m
[31m-        private static void GetDependsOnChangeOrdersAndChangeSetIds([m
[32m+[m[32m        private static void UpdateResourceWithDependsOnChangeOrdersAndChangeSetIds([m[41m[m
[32m+[m[32m            EntityDescriptor res,[m[41m[m
             object[] dependsOnObjects,[m
             EntityTracker entityTracker,[m
[31m-            out List<string> dependsOnIdsAsChangeOrders,[m
[31m-            out HashSet<string> dependsOnChangeSetIds)[m
[32m+[m[32m            out EntityDescriptor resource)[m[41m[m
         {[m
[31m-            dependsOnIdsAsChangeOrders = new List<string>();[m
[31m-            dependsOnChangeSetIds = new HashSet<string>();[m
[31m-[m
[32m+[m[32m            List<string> dependsOnIdsAsChangeOrders = new List<string>();[m[41m[m
[32m+[m[32m            HashSet<string> dependsOnChangeSetIds = new HashSet<string>();[m[41m[m
[32m+[m[32m            resource = res;[m[41m[m
             foreach (var obj in dependsOnObjects)[m
             {[m
                 EntityDescriptor dependsOnResource = entityTracker.TryGetEntityDescriptor(obj);[m
                 dependsOnIdsAsChangeOrders.Add(dependsOnResource.ChangeOrder.ToString(CultureInfo.InvariantCulture));[m
                 dependsOnChangeSetIds.Add(dependsOnResource.ChangeSetId);[m
             }[m
[32m+[m[41m[m
[32m+[m[32m            resource.DependsOnIds = dependsOnIdsAsChangeOrders;[m[41m[m
[32m+[m[32m            resource.DependsOnChangeSetIds = dependsOnChangeSetIds.ToList();[m[41m[m
         }[m
 [m
         /// <summary>[m
