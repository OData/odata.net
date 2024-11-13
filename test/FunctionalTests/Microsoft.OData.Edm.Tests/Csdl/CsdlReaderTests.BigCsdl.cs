//---------------------------------------------------------------------
// <copyright file="CsdlReaderTests.BigCsdl.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Tests.Csdl
{
    public partial class CsdlReaderTests
    {
#region BigCsdl
		private static string BigCsdl = @"
<edmx:Edmx
	xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"" Version=""4.0"">
	<edmx:DataServices>
		<Schema
			xmlns=""http://docs.oasis-open.org/odata/ns/edm"" Namespace=""Example.Database"">
			<EntityType Name=""User"">
				<Key>
					<PropertyRef Name=""UserId""/>
				</Key>
				<Property Name=""UserId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""LoginName"" Type=""Edm.String""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""LastLogin"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""State"" Type=""Edm.String""/>
				<Property Name=""email"" Type=""Edm.String""/>
				<Property Name=""Function"" Type=""Edm.String""/>
				<Property Name=""Department"" Type=""Edm.String""/>
				<Property Name=""IsSystemUser"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""LockedOut"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""RequirePasswordChange"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""LastPasswordChange"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""Culture"" Type=""Edm.String""/>
				<Property Name=""AvailabilityDayParts"" Type=""Edm.Int64""/>
				<Property Name=""Phone"" Type=""Edm.String""/>
				<Property Name=""Mobile"" Type=""Edm.String""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""MembershipProvider"" Type=""Example.Database.MembershipProvider"" Nullable=""false""/>
				<NavigationProperty Name=""Location"" Type=""Example.Database.Location"" Nullable=""false""/>
				<NavigationProperty Name=""PreferredGroup"" Type=""Example.Database.Group"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedUsers"" Type=""Collection(Example.Database.User)""/>
				<NavigationProperty Name=""ModifiedUsers"" Type=""Collection(Example.Database.User)""/>
				<NavigationProperty Name=""DeletedUsers"" Type=""Collection(Example.Database.User)""/>
				<NavigationProperty Name=""CreatedAccountProvider"" Type=""Collection(Example.Database.AccountProvider)""/>
				<NavigationProperty Name=""ModifiedAccountProvider"" Type=""Collection(Example.Database.AccountProvider)""/>
				<NavigationProperty Name=""CreatedAccounts"" Type=""Collection(Example.Database.Account)""/>
				<NavigationProperty Name=""ModifiedAccounts"" Type=""Collection(Example.Database.Account)""/>
				<NavigationProperty Name=""Accounts"" Type=""Collection(Example.Database.Account)""/>
				<NavigationProperty Name=""CreatedTables"" Type=""Collection(Example.Database.Table)""/>
				<NavigationProperty Name=""ModifiedTables"" Type=""Collection(Example.Database.Table)""/>
				<NavigationProperty Name=""CreatedFields"" Type=""Collection(Example.Database.Field)""/>
				<NavigationProperty Name=""ModifiedFields"" Type=""Collection(Example.Database.Field)""/>
				<NavigationProperty Name=""CreatedTexts"" Type=""Collection(Example.Database.Text)""/>
				<NavigationProperty Name=""ModifiedTexts"" Type=""Collection(Example.Database.Text)""/>
				<NavigationProperty Name=""CreatedEntities"" Type=""Collection(Example.Database.Entity)""/>
				<NavigationProperty Name=""ModifiedEntities"" Type=""Collection(Example.Database.Entity)""/>
				<NavigationProperty Name=""CreatedEntityUsers"" Type=""Collection(Example.Database.EntityUser)""/>
				<NavigationProperty Name=""ModifiedEntityUsers"" Type=""Collection(Example.Database.EntityUser)""/>
				<NavigationProperty Name=""EntityUsers"" Type=""Collection(Example.Database.EntityUser)""/>
				<NavigationProperty Name=""ModifiedEntityTypeEvents"" Type=""Collection(Example.Database.EntityTypeEvent)""/>
				<NavigationProperty Name=""CreatedEntityTypeEvents"" Type=""Collection(Example.Database.EntityTypeEvent)""/>
				<NavigationProperty Name=""CreatedEntityEvents"" Type=""Collection(Example.Database.EntityEvent)""/>
				<NavigationProperty Name=""ModifiedEntityEvents"" Type=""Collection(Example.Database.EntityEvent)""/>
				<NavigationProperty Name=""CreatedItemEvents"" Type=""Collection(Example.Database.ItemEvent)""/>
				<NavigationProperty Name=""ModifiedItemEvents"" Type=""Collection(Example.Database.ItemEvent)""/>
				<NavigationProperty Name=""ItemEventsByCancelledBy"" Type=""Collection(Example.Database.ItemEvent)""/>
				<NavigationProperty Name=""CreatedPhrases"" Type=""Collection(Example.Database.Phrase)""/>
				<NavigationProperty Name=""ModifiedPhrases"" Type=""Collection(Example.Database.Phrase)""/>
				<NavigationProperty Name=""DeletedPhrases"" Type=""Collection(Example.Database.Phrase)""/>
				<NavigationProperty Name=""CreatedPhraseItems"" Type=""Collection(Example.Database.PhraseItem)""/>
				<NavigationProperty Name=""ModifiedPhraseItems"" Type=""Collection(Example.Database.PhraseItem)""/>
				<NavigationProperty Name=""CreatedEntityFields"" Type=""Collection(Example.Database.EntityField)""/>
				<NavigationProperty Name=""ModifiedEntityFields"" Type=""Collection(Example.Database.EntityField)""/>
				<NavigationProperty Name=""ModifiedEntityFieldHistories"" Type=""Collection(Example.Database.EntityFieldHistory)""/>
				<NavigationProperty Name=""CreatedEntityFieldHistories"" Type=""Collection(Example.Database.EntityFieldHistory)""/>
				<NavigationProperty Name=""CreatedEntityPages"" Type=""Collection(Example.Database.EntityPage)""/>
				<NavigationProperty Name=""ModifiedEntityPages"" Type=""Collection(Example.Database.EntityPage)""/>
				<NavigationProperty Name=""CreatedRoles"" Type=""Collection(Example.Database.Role)""/>
				<NavigationProperty Name=""ModifiedRoles"" Type=""Collection(Example.Database.Role)""/>
				<NavigationProperty Name=""DeletedRoles"" Type=""Collection(Example.Database.Role)""/>
				<NavigationProperty Name=""CreatedUserRoles"" Type=""Collection(Example.Database.UserRole)""/>
				<NavigationProperty Name=""ModifiedUserRoles"" Type=""Collection(Example.Database.UserRole)""/>
				<NavigationProperty Name=""UserRoles"" Type=""Collection(Example.Database.UserRole)""/>
				<NavigationProperty Name=""CreatedSessions"" Type=""Collection(Example.Database.Session)""/>
				<NavigationProperty Name=""ModifiedSessions"" Type=""Collection(Example.Database.Session)""/>
				<NavigationProperty Name=""Sessions"" Type=""Collection(Example.Database.Session)""/>
				<NavigationProperty Name=""CreatedAddresses"" Type=""Collection(Example.Database.Address)""/>
				<NavigationProperty Name=""ModifiedAddresses"" Type=""Collection(Example.Database.Address)""/>
				<NavigationProperty Name=""CreatedNotes"" Type=""Collection(Example.Database.Note)""/>
				<NavigationProperty Name=""ModifiedNotes"" Type=""Collection(Example.Database.Note)""/>
				<NavigationProperty Name=""CreatedOperations"" Type=""Collection(Example.Database.Operation)""/>
				<NavigationProperty Name=""ModifiedOperations"" Type=""Collection(Example.Database.Operation)""/>
				<NavigationProperty Name=""DeletedOperations"" Type=""Collection(Example.Database.Operation)""/>
				<NavigationProperty Name=""CreatedOperationUsers"" Type=""Collection(Example.Database.OperationUser)""/>
				<NavigationProperty Name=""ModifiedOperationUsers"" Type=""Collection(Example.Database.OperationUser)""/>
				<NavigationProperty Name=""OperationUsers"" Type=""Collection(Example.Database.OperationUser)""/>
				<NavigationProperty Name=""CreatedTasks"" Type=""Collection(Example.Database.Task)""/>
				<NavigationProperty Name=""ModifiedTasks"" Type=""Collection(Example.Database.Task)""/>
				<NavigationProperty Name=""DeletedTasks"" Type=""Collection(Example.Database.Task)""/>
				<NavigationProperty Name=""CreatedRoleTasks"" Type=""Collection(Example.Database.RoleTask)""/>
				<NavigationProperty Name=""ModifiedRoleTasks"" Type=""Collection(Example.Database.RoleTask)""/>
				<NavigationProperty Name=""CreatedTaskOperations"" Type=""Collection(Example.Database.TaskOperation)""/>
				<NavigationProperty Name=""ModifiedTaskOperations"" Type=""Collection(Example.Database.TaskOperation)""/>
				<NavigationProperty Name=""ModifiedStoragePolicies"" Type=""Collection(Example.Database.StoragePolicy)""/>
				<NavigationProperty Name=""CreatedStoragePolicies"" Type=""Collection(Example.Database.StoragePolicy)""/>
				<NavigationProperty Name=""CreatedFileTypes"" Type=""Collection(Example.Database.FileType)""/>
				<NavigationProperty Name=""ModifiedFileTypes"" Type=""Collection(Example.Database.FileType)""/>
				<NavigationProperty Name=""CreatedFileTemplates"" Type=""Collection(Example.Database.FileTemplate)""/>
				<NavigationProperty Name=""ModifiedFileTemplates"" Type=""Collection(Example.Database.FileTemplate)""/>
				<NavigationProperty Name=""ModifiedFileGroups"" Type=""Collection(Example.Database.FileGroup)""/>
				<NavigationProperty Name=""CreatedFileGroups"" Type=""Collection(Example.Database.FileGroup)""/>
				<NavigationProperty Name=""DeletedFileGroups"" Type=""Collection(Example.Database.FileGroup)""/>
				<NavigationProperty Name=""ModifiedFiles"" Type=""Collection(Example.Database.File)""/>
				<NavigationProperty Name=""CreatedFiles"" Type=""Collection(Example.Database.File)""/>
				<NavigationProperty Name=""DeletedFiles"" Type=""Collection(Example.Database.File)""/>
				<NavigationProperty Name=""CreatedUserRequests"" Type=""Collection(Example.Database.UserRequest)""/>
				<NavigationProperty Name=""ModifiedUserRequests"" Type=""Collection(Example.Database.UserRequest)""/>
				<NavigationProperty Name=""CreatedUrlActions"" Type=""Collection(Example.Database.UrlAction)""/>
				<NavigationProperty Name=""ModifiedUrlActions"" Type=""Collection(Example.Database.UrlAction)""/>
				<NavigationProperty Name=""CreatedForms"" Type=""Collection(Example.Database.Form)""/>
				<NavigationProperty Name=""ModifiedForms"" Type=""Collection(Example.Database.Form)""/>
				<NavigationProperty Name=""DeletedForms"" Type=""Collection(Example.Database.Form)""/>
				<NavigationProperty Name=""CreatedFormFields"" Type=""Collection(Example.Database.FormField)""/>
				<NavigationProperty Name=""ModifiedFormFields"" Type=""Collection(Example.Database.FormField)""/>
				<NavigationProperty Name=""FormFieldsByEnteredBy"" Type=""Collection(Example.Database.FormField)""/>
				<NavigationProperty Name=""DeletedFormFields"" Type=""Collection(Example.Database.FormField)""/>
				<NavigationProperty Name=""FormFieldsByAuthorisedBy"" Type=""Collection(Example.Database.FormField)""/>
				<NavigationProperty Name=""CreatedEntityForms"" Type=""Collection(Example.Database.EntityForm)""/>
				<NavigationProperty Name=""ModifiedEntityForms"" Type=""Collection(Example.Database.EntityForm)""/>
				<NavigationProperty Name=""DeletedEntityForms"" Type=""Collection(Example.Database.EntityForm)""/>
				<NavigationProperty Name=""CreatedOutgoingMessages"" Type=""Collection(Example.Database.OutgoingMessage)""/>
				<NavigationProperty Name=""ModifiedOutgoingMessages"" Type=""Collection(Example.Database.OutgoingMessage)""/>
				<NavigationProperty Name=""CreatedFormTemplates"" Type=""Collection(Example.Database.FormTemplate)""/>
				<NavigationProperty Name=""ModifiedFormTemplates"" Type=""Collection(Example.Database.FormTemplate)""/>
				<NavigationProperty Name=""DeletedFormTemplates"" Type=""Collection(Example.Database.FormTemplate)""/>
				<NavigationProperty Name=""CreatedFormTemplateFields"" Type=""Collection(Example.Database.FormTemplateField)""/>
				<NavigationProperty Name=""ModifiedFormTemplateFields"" Type=""Collection(Example.Database.FormTemplateField)""/>
				<NavigationProperty Name=""DeletedFormTemplateFields"" Type=""Collection(Example.Database.FormTemplateField)""/>
				<NavigationProperty Name=""CreatedEntityFormTemplates"" Type=""Collection(Example.Database.EntityFormTemplate)""/>
				<NavigationProperty Name=""ModifiedEntityFormTemplates"" Type=""Collection(Example.Database.EntityFormTemplate)""/>
				<NavigationProperty Name=""DeletedEntityFormTemplates"" Type=""Collection(Example.Database.EntityFormTemplate)""/>
				<NavigationProperty Name=""CreatedExceptions"" Type=""Collection(Example.Database.Exception)""/>
				<NavigationProperty Name=""ModifiedExceptions"" Type=""Collection(Example.Database.Exception)""/>
				<NavigationProperty Name=""CreatedExceptionParameters"" Type=""Collection(Example.Database.ExceptionParameter)""/>
				<NavigationProperty Name=""ModifiedExceptionParameters"" Type=""Collection(Example.Database.ExceptionParameter)""/>
				<NavigationProperty Name=""CreatedFormTemplateVersions"" Type=""Collection(Example.Database.FormTemplateVersion)""/>
				<NavigationProperty Name=""ModifiedFormTemplateVersions"" Type=""Collection(Example.Database.FormTemplateVersion)""/>
				<NavigationProperty Name=""DeletedFormTemplateVersions"" Type=""Collection(Example.Database.FormTemplateVersion)""/>
				<NavigationProperty Name=""CreatedFormTemplateSections"" Type=""Collection(Example.Database.FormTemplateSection)""/>
				<NavigationProperty Name=""ModifiedFormTemplateSections"" Type=""Collection(Example.Database.FormTemplateSection)""/>
				<NavigationProperty Name=""DeletedFormTemplateSections"" Type=""Collection(Example.Database.FormTemplateSection)""/>
				<NavigationProperty Name=""CreatedModules"" Type=""Collection(Example.Database.Module)""/>
				<NavigationProperty Name=""ModifiedModules"" Type=""Collection(Example.Database.Module)""/>
				<NavigationProperty Name=""DeletedModules"" Type=""Collection(Example.Database.Module)""/>
				<NavigationProperty Name=""CreatedFolders"" Type=""Collection(Example.Database.Folder)""/>
				<NavigationProperty Name=""ModifiedFolders"" Type=""Collection(Example.Database.Folder)""/>
				<NavigationProperty Name=""CreatedFolderFilters"" Type=""Collection(Example.Database.FolderFilter)""/>
				<NavigationProperty Name=""ModifiedFolderFilters"" Type=""Collection(Example.Database.FolderFilter)""/>
				<NavigationProperty Name=""CreatedActions"" Type=""Collection(Example.Database.Action)""/>
				<NavigationProperty Name=""ModifiedActions"" Type=""Collection(Example.Database.Action)""/>
				<NavigationProperty Name=""CreatedActionParameters"" Type=""Collection(Example.Database.ActionParameter)""/>
				<NavigationProperty Name=""ModifiedActionParameters"" Type=""Collection(Example.Database.ActionParameter)""/>
				<NavigationProperty Name=""CreatedFolderEntities"" Type=""Collection(Example.Database.FolderEntity)""/>
				<NavigationProperty Name=""ModifiedFolderEntities"" Type=""Collection(Example.Database.FolderEntity)""/>
				<NavigationProperty Name=""CreatedApplications"" Type=""Collection(Example.Database.Application)""/>
				<NavigationProperty Name=""ModifiedApplications"" Type=""Collection(Example.Database.Application)""/>
				<NavigationProperty Name=""CreatedMenus"" Type=""Collection(Example.Database.Menu)""/>
				<NavigationProperty Name=""ModifiedMenus"" Type=""Collection(Example.Database.Menu)""/>
				<NavigationProperty Name=""CreatedAudits"" Type=""Collection(Example.Database.Audit)""/>
				<NavigationProperty Name=""ModifiedAudits"" Type=""Collection(Example.Database.Audit)""/>
				<NavigationProperty Name=""CreatedAuditItems"" Type=""Collection(Example.Database.AuditItem)""/>
				<NavigationProperty Name=""ModifiedAuditItems"" Type=""Collection(Example.Database.AuditItem)""/>
				<NavigationProperty Name=""CreatedAuditFields"" Type=""Collection(Example.Database.AuditField)""/>
				<NavigationProperty Name=""ModifiedAuditFields"" Type=""Collection(Example.Database.AuditField)""/>
				<NavigationProperty Name=""CreatedElectronicSignatureTypes"" Type=""Collection(Example.Database.ElectronicSignatureType)""/>
				<NavigationProperty Name=""ModifiedElectronicSignatureTypes"" Type=""Collection(Example.Database.ElectronicSignatureType)""/>
				<NavigationProperty Name=""DeletedElectronicSignatureTypes"" Type=""Collection(Example.Database.ElectronicSignatureType)""/>
				<NavigationProperty Name=""CreatedElectronicSignatureTypeFields"" Type=""Collection(Example.Database.ElectronicSignatureTypeField)""/>
				<NavigationProperty Name=""ModifiedElectronicSignatureTypeFields"" Type=""Collection(Example.Database.ElectronicSignatureTypeField)""/>
				<NavigationProperty Name=""DeletedElectronicSignatureTypeFields"" Type=""Collection(Example.Database.ElectronicSignatureTypeField)""/>
				<NavigationProperty Name=""CreatedElectronicSignatures"" Type=""Collection(Example.Database.ElectronicSignature)""/>
				<NavigationProperty Name=""ModifiedElectronicSignatures"" Type=""Collection(Example.Database.ElectronicSignature)""/>
				<NavigationProperty Name=""ElectronicSignaturesBySignedBy"" Type=""Collection(Example.Database.ElectronicSignature)""/>
				<NavigationProperty Name=""DeletedElectronicSignatures"" Type=""Collection(Example.Database.ElectronicSignature)""/>
				<NavigationProperty Name=""CreatedElectronicSignatureFields"" Type=""Collection(Example.Database.ElectronicSignatureField)""/>
				<NavigationProperty Name=""ModifiedElectronicSignatureFields"" Type=""Collection(Example.Database.ElectronicSignatureField)""/>
				<NavigationProperty Name=""DeletedElectronicSignatureFields"" Type=""Collection(Example.Database.ElectronicSignatureField)""/>
				<NavigationProperty Name=""CreatedSettings"" Type=""Collection(Example.Database.Setting)""/>
				<NavigationProperty Name=""ModifiedSettings"" Type=""Collection(Example.Database.Setting)""/>
				<NavigationProperty Name=""CreatedReportTypes"" Type=""Collection(Example.Database.ReportType)""/>
				<NavigationProperty Name=""ModifiedReportTypes"" Type=""Collection(Example.Database.ReportType)""/>
				<NavigationProperty Name=""DeletedReportTypes"" Type=""Collection(Example.Database.ReportType)""/>
				<NavigationProperty Name=""CreatedReportTypeFormats"" Type=""Collection(Example.Database.ReportTypeFormat)""/>
				<NavigationProperty Name=""ModifiedReportTypeFormats"" Type=""Collection(Example.Database.ReportTypeFormat)""/>
				<NavigationProperty Name=""CreatedReportTypeArguments"" Type=""Collection(Example.Database.ReportTypeArgument)""/>
				<NavigationProperty Name=""ModifiedReportTypeArguments"" Type=""Collection(Example.Database.ReportTypeArgument)""/>
				<NavigationProperty Name=""CreatedReportTypeFiles"" Type=""Collection(Example.Database.ReportTypeFile)""/>
				<NavigationProperty Name=""ModifiedReportTypeFiles"" Type=""Collection(Example.Database.ReportTypeFile)""/>
				<NavigationProperty Name=""CreatedMembershipProviders"" Type=""Collection(Example.Database.MembershipProvider)""/>
				<NavigationProperty Name=""ModifiedMembershipProviders"" Type=""Collection(Example.Database.MembershipProvider)""/>
				<NavigationProperty Name=""CreatedDataProviders"" Type=""Collection(Example.Database.DataProvider)""/>
				<NavigationProperty Name=""ModifiedDataProviders"" Type=""Collection(Example.Database.DataProvider)""/>
				<NavigationProperty Name=""CreatedEntityActions"" Type=""Collection(Example.Database.EntityAction)""/>
				<NavigationProperty Name=""ModifiedEntityActions"" Type=""Collection(Example.Database.EntityAction)""/>
				<NavigationProperty Name=""CreatedUserSettings"" Type=""Collection(Example.Database.UserSetting)""/>
				<NavigationProperty Name=""ModifiedUserSettings"" Type=""Collection(Example.Database.UserSetting)""/>
				<NavigationProperty Name=""CreatedDevices"" Type=""Collection(Example.Database.Device)""/>
				<NavigationProperty Name=""ModifiedDevices"" Type=""Collection(Example.Database.Device)""/>
				<NavigationProperty Name=""DeletedDevices"" Type=""Collection(Example.Database.Device)""/>
				<NavigationProperty Name=""CreatedLocations"" Type=""Collection(Example.Database.Location)""/>
				<NavigationProperty Name=""ModifiedLocations"" Type=""Collection(Example.Database.Location)""/>
				<NavigationProperty Name=""DeletedLocations"" Type=""Collection(Example.Database.Location)""/>
				<NavigationProperty Name=""ModifiedLocationTypes"" Type=""Collection(Example.Database.LocationType)""/>
				<NavigationProperty Name=""CreatedLocationTypes"" Type=""Collection(Example.Database.LocationType)""/>
				<NavigationProperty Name=""CreatedQueries"" Type=""Collection(Example.Database.Query)""/>
				<NavigationProperty Name=""ModifiedQueries"" Type=""Collection(Example.Database.Query)""/>
				<NavigationProperty Name=""CreatedQueryExpressions"" Type=""Collection(Example.Database.QueryExpression)""/>
				<NavigationProperty Name=""ModifiedQueryExpressions"" Type=""Collection(Example.Database.QueryExpression)""/>
				<NavigationProperty Name=""CreatedTimestampValues"" Type=""Collection(Example.Database.TimestampValue)""/>
				<NavigationProperty Name=""ModifiedTimestampValues"" Type=""Collection(Example.Database.TimestampValue)""/>
				<NavigationProperty Name=""CreatedDocumentTypes"" Type=""Collection(Example.Database.DocumentType)""/>
				<NavigationProperty Name=""ModifiedDocumentTypes"" Type=""Collection(Example.Database.DocumentType)""/>
				<NavigationProperty Name=""DocumentTypesByOwner"" Type=""Collection(Example.Database.DocumentType)""/>
				<NavigationProperty Name=""CreatedDocumentIndices"" Type=""Collection(Example.Database.DocumentIndex)""/>
				<NavigationProperty Name=""ModifiedDocumentIndices"" Type=""Collection(Example.Database.DocumentIndex)""/>
				<NavigationProperty Name=""CreatedDocuments"" Type=""Collection(Example.Database.Document)""/>
				<NavigationProperty Name=""ModifiedDocuments"" Type=""Collection(Example.Database.Document)""/>
				<NavigationProperty Name=""DocumentsByOwner"" Type=""Collection(Example.Database.Document)""/>
				<NavigationProperty Name=""CreatedDocumentVersions"" Type=""Collection(Example.Database.DocumentVersion)""/>
				<NavigationProperty Name=""ModifiedDocumentVersions"" Type=""Collection(Example.Database.DocumentVersion)""/>
				<NavigationProperty Name=""DocumentVersionsByOwner"" Type=""Collection(Example.Database.DocumentVersion)""/>
				<NavigationProperty Name=""DocumentVersionsByAuthorizer"" Type=""Collection(Example.Database.DocumentVersion)""/>
				<NavigationProperty Name=""DocumentVersionsByAuthorisedBy"" Type=""Collection(Example.Database.DocumentVersion)""/>
				<NavigationProperty Name=""CreatedChangeRequests"" Type=""Collection(Example.Database.ChangeRequest)""/>
				<NavigationProperty Name=""ModifiedChangeRequests"" Type=""Collection(Example.Database.ChangeRequest)""/>
				<NavigationProperty Name=""CreatedChangeRequestTypes"" Type=""Collection(Example.Database.ChangeRequestType)""/>
				<NavigationProperty Name=""ModifiedChangeRequestTypes"" Type=""Collection(Example.Database.ChangeRequestType)""/>
				<NavigationProperty Name=""CreatedCounters"" Type=""Collection(Example.Database.Counter)""/>
				<NavigationProperty Name=""ModifiedCounters"" Type=""Collection(Example.Database.Counter)""/>
				<NavigationProperty Name=""CreatedProperties"" Type=""Collection(Example.Database.Property)""/>
				<NavigationProperty Name=""ModifiedProperties"" Type=""Collection(Example.Database.Property)""/>
				<NavigationProperty Name=""CreatedEntityHierarchys"" Type=""Collection(Example.Database.EntityHierarchy)""/>
				<NavigationProperty Name=""ModifiedEntityHierarchys"" Type=""Collection(Example.Database.EntityHierarchy)""/>
				<NavigationProperty Name=""CreatedPrinters"" Type=""Collection(Example.Database.Printer)""/>
				<NavigationProperty Name=""ModifiedPrinters"" Type=""Collection(Example.Database.Printer)""/>
				<NavigationProperty Name=""CreatedCountries"" Type=""Collection(Example.Database.Country)""/>
				<NavigationProperty Name=""ModifiedCountries"" Type=""Collection(Example.Database.Country)""/>
				<NavigationProperty Name=""CreatedNewsItems"" Type=""Collection(Example.Database.NewsItem)""/>
				<NavigationProperty Name=""ModifiedNewsItems"" Type=""Collection(Example.Database.NewsItem)""/>
				<NavigationProperty Name=""DeletedNewsItems"" Type=""Collection(Example.Database.NewsItem)""/>
				<NavigationProperty Name=""CreatedTypeBindings"" Type=""Collection(Example.Database.TypeBinding)""/>
				<NavigationProperty Name=""ModifiedTypeBindings"" Type=""Collection(Example.Database.TypeBinding)""/>
				<NavigationProperty Name=""ModifiedDeviceTasks"" Type=""Collection(Example.Database.DeviceTask)""/>
				<NavigationProperty Name=""CreatedDeviceTasks"" Type=""Collection(Example.Database.DeviceTask)""/>
				<NavigationProperty Name=""CreatedUpgradeStates"" Type=""Collection(Example.Database.UpgradeState)""/>
				<NavigationProperty Name=""ModifiedUpgradeStates"" Type=""Collection(Example.Database.UpgradeState)""/>
				<NavigationProperty Name=""CreatedGroups"" Type=""Collection(Example.Database.Group)""/>
				<NavigationProperty Name=""ModifiedGroups"" Type=""Collection(Example.Database.Group)""/>
				<NavigationProperty Name=""GroupsByGroupLeader"" Type=""Collection(Example.Database.Group)""/>
				<NavigationProperty Name=""DeletedGroups"" Type=""Collection(Example.Database.Group)""/>
				<NavigationProperty Name=""CreatedUserGroups"" Type=""Collection(Example.Database.UserGroup)""/>
				<NavigationProperty Name=""ModifiedUserGroups"" Type=""Collection(Example.Database.UserGroup)""/>
				<NavigationProperty Name=""UserGroups"" Type=""Collection(Example.Database.UserGroup)""/>
				<NavigationProperty Name=""DeletedUserGroups"" Type=""Collection(Example.Database.UserGroup)""/>
				<NavigationProperty Name=""CreatedEntityFieldRules"" Type=""Collection(Example.Database.EntityFieldRule)""/>
				<NavigationProperty Name=""ModifiedEntityFieldRules"" Type=""Collection(Example.Database.EntityFieldRule)""/>
				<NavigationProperty Name=""CreatedSystemLogs"" Type=""Collection(Example.Database.SystemLog)""/>
				<NavigationProperty Name=""ModifiedSystemLogs"" Type=""Collection(Example.Database.SystemLog)""/>
				<NavigationProperty Name=""CreatedSystemLogItems"" Type=""Collection(Example.Database.SystemLogItem)""/>
				<NavigationProperty Name=""ModifiedSystemLogItems"" Type=""Collection(Example.Database.SystemLogItem)""/>
				<NavigationProperty Name=""ModifiedFonts"" Type=""Collection(Example.Database.Font)""/>
				<NavigationProperty Name=""CreatedFonts"" Type=""Collection(Example.Database.Font)""/>
				<NavigationProperty Name=""ModifiedCheckListTypes"" Type=""Collection(Example.Database.CheckListType)""/>
				<NavigationProperty Name=""CreatedCheckListTypes"" Type=""Collection(Example.Database.CheckListType)""/>
				<NavigationProperty Name=""DeletedCheckListTypes"" Type=""Collection(Example.Database.CheckListType)""/>
				<NavigationProperty Name=""ModifiedCheckListTypeItems"" Type=""Collection(Example.Database.CheckListTypeItem)""/>
				<NavigationProperty Name=""CreatedCheckListTypeItems"" Type=""Collection(Example.Database.CheckListTypeItem)""/>
				<NavigationProperty Name=""DeletedCheckListTypeItems"" Type=""Collection(Example.Database.CheckListTypeItem)""/>
				<NavigationProperty Name=""ModifiedCheckLists"" Type=""Collection(Example.Database.CheckList)""/>
				<NavigationProperty Name=""CreatedCheckLists"" Type=""Collection(Example.Database.CheckList)""/>
				<NavigationProperty Name=""DeletedCheckLists"" Type=""Collection(Example.Database.CheckList)""/>
				<NavigationProperty Name=""ModifiedCheckListItems"" Type=""Collection(Example.Database.CheckListItem)""/>
				<NavigationProperty Name=""CreatedCheckListItems"" Type=""Collection(Example.Database.CheckListItem)""/>
				<NavigationProperty Name=""DeletedCheckListItems"" Type=""Collection(Example.Database.CheckListItem)""/>
				<NavigationProperty Name=""ModifiedLocationItems"" Type=""Collection(Example.Database.LocationItem)""/>
				<NavigationProperty Name=""CreatedLocationItems"" Type=""Collection(Example.Database.LocationItem)""/>
				<NavigationProperty Name=""ModifiedLocationItemTypes"" Type=""Collection(Example.Database.LocationItemType)""/>
				<NavigationProperty Name=""CreatedLocationItemTypes"" Type=""Collection(Example.Database.LocationItemType)""/>
				<NavigationProperty Name=""ModifiedInstallations"" Type=""Collection(Example.Database.Installation)""/>
				<NavigationProperty Name=""CreatedInstallations"" Type=""Collection(Example.Database.Installation)""/>
				<NavigationProperty Name=""DeletedInstallations"" Type=""Collection(Example.Database.Installation)""/>
				<NavigationProperty Name=""ModifiedInstallationLogs"" Type=""Collection(Example.Database.InstallationLog)""/>
				<NavigationProperty Name=""CreatedInstallationLogs"" Type=""Collection(Example.Database.InstallationLog)""/>
				<NavigationProperty Name=""DeletedInstallationLogs"" Type=""Collection(Example.Database.InstallationLog)""/>
				<NavigationProperty Name=""ModifiedInstallationPackages"" Type=""Collection(Example.Database.InstallationPackage)""/>
				<NavigationProperty Name=""CreatedInstallationPackages"" Type=""Collection(Example.Database.InstallationPackage)""/>
				<NavigationProperty Name=""ModifiedConfigurationSystems"" Type=""Collection(Example.Database.ConfigurationSystem)""/>
				<NavigationProperty Name=""CreatedConfigurationSystems"" Type=""Collection(Example.Database.ConfigurationSystem)""/>
				<NavigationProperty Name=""DeletedConfigurationSystems"" Type=""Collection(Example.Database.ConfigurationSystem)""/>
				<NavigationProperty Name=""ModifiedConfigurationInstances"" Type=""Collection(Example.Database.ConfigurationInstance)""/>
				<NavigationProperty Name=""CreatedConfigurationInstances"" Type=""Collection(Example.Database.ConfigurationInstance)""/>
				<NavigationProperty Name=""DeletedConfigurationInstances"" Type=""Collection(Example.Database.ConfigurationInstance)""/>
				<NavigationProperty Name=""ModifiedConfigurationSystemMonitorings"" Type=""Collection(Example.Database.ConfigurationSystemMonitoring)""/>
				<NavigationProperty Name=""CreatedConfigurationSystemMonitorings"" Type=""Collection(Example.Database.ConfigurationSystemMonitoring)""/>
				<NavigationProperty Name=""CreatedCredentials"" Type=""Collection(Example.Database.Credential)""/>
				<NavigationProperty Name=""ModifiedCredentials"" Type=""Collection(Example.Database.Credential)""/>
				<NavigationProperty Name=""Credentials"" Type=""Collection(Example.Database.Credential)""/>
				<NavigationProperty Name=""DeletedCredentials"" Type=""Collection(Example.Database.Credential)""/>
				<NavigationProperty Name=""CreatedJobPositions"" Type=""Collection(Example.Database.JobPosition)""/>
				<NavigationProperty Name=""ModifiedJobPositions"" Type=""Collection(Example.Database.JobPosition)""/>
				<NavigationProperty Name=""CreatedTrainings"" Type=""Collection(Example.Database.Training)""/>
				<NavigationProperty Name=""ModifiedTrainings"" Type=""Collection(Example.Database.Training)""/>
				<NavigationProperty Name=""DeletedTrainings"" Type=""Collection(Example.Database.Training)""/>
				<NavigationProperty Name=""CreatedJobPositionTrainings"" Type=""Collection(Example.Database.JobPositionTraining)""/>
				<NavigationProperty Name=""ModifiedJobPositionTrainings"" Type=""Collection(Example.Database.JobPositionTraining)""/>
				<NavigationProperty Name=""CreatedUserTrainings"" Type=""Collection(Example.Database.UserTraining)""/>
				<NavigationProperty Name=""ModifiedUserTrainings"" Type=""Collection(Example.Database.UserTraining)""/>
				<NavigationProperty Name=""UserTrainings"" Type=""Collection(Example.Database.UserTraining)""/>
				<NavigationProperty Name=""DeletedUserTrainings"" Type=""Collection(Example.Database.UserTraining)""/>
				<NavigationProperty Name=""CreatedUserJobPositions"" Type=""Collection(Example.Database.UserJobPosition)""/>
				<NavigationProperty Name=""ModifiedUserJobPositions"" Type=""Collection(Example.Database.UserJobPosition)""/>
				<NavigationProperty Name=""UserJobPositions"" Type=""Collection(Example.Database.UserJobPosition)""/>
				<NavigationProperty Name=""ModifiedSchemaFiles"" Type=""Collection(Example.Database.SchemaFile)""/>
				<NavigationProperty Name=""CreatedSchemaFiles"" Type=""Collection(Example.Database.SchemaFile)""/>
				<NavigationProperty Name=""ModifiedRecordLocks"" Type=""Collection(Example.Database.RecordLock)""/>
				<NavigationProperty Name=""CreatedRecordLocks"" Type=""Collection(Example.Database.RecordLock)""/>
				<NavigationProperty Name=""ModifiedSubscriptions"" Type=""Collection(Example.Database.Subscription)""/>
				<NavigationProperty Name=""CreatedSubscriptions"" Type=""Collection(Example.Database.Subscription)""/>
				<NavigationProperty Name=""ModifiedSubscriptionSubjects"" Type=""Collection(Example.Database.SubscriptionSubject)""/>
				<NavigationProperty Name=""CreatedSubscriptionSubjects"" Type=""Collection(Example.Database.SubscriptionSubject)""/>
				<NavigationProperty Name=""ModifiedSubscriptionManagers"" Type=""Collection(Example.Database.SubscriptionManager)""/>
				<NavigationProperty Name=""CreatedSubscriptionManagers"" Type=""Collection(Example.Database.SubscriptionManager)""/>
				<NavigationProperty Name=""ModifiedSubscriptionNotifications"" Type=""Collection(Example.Database.SubscriptionNotification)""/>
				<NavigationProperty Name=""CreatedSubscriptionNotifications"" Type=""Collection(Example.Database.SubscriptionNotification)""/>
				<NavigationProperty Name=""LicenseUsers"" Type=""Collection(Example.Database.LicenseUser)""/>
				<NavigationProperty Name=""ModifiedLicenseUsers"" Type=""Collection(Example.Database.LicenseUser)""/>
				<NavigationProperty Name=""CreatedLicenseUsers"" Type=""Collection(Example.Database.LicenseUser)""/>
				<NavigationProperty Name=""DeletedLicenseUsers"" Type=""Collection(Example.Database.LicenseUser)""/>
				<NavigationProperty Name=""ModifiedConfigurationSystemLicenses"" Type=""Collection(Example.Database.ConfigurationSystemLicense)""/>
				<NavigationProperty Name=""CreatedConfigurationSystemLicenses"" Type=""Collection(Example.Database.ConfigurationSystemLicense)""/>
				<NavigationProperty Name=""DeletedConfigurationSystemLicenses"" Type=""Collection(Example.Database.ConfigurationSystemLicense)""/>
				<NavigationProperty Name=""CreatedProcessStationInstruments"" Type=""Collection(Example.Database.ProcessStationInstrument)""/>
				<NavigationProperty Name=""ModifiedProcessStationInstruments"" Type=""Collection(Example.Database.ProcessStationInstrument)""/>
				<NavigationProperty Name=""CreatedClients"" Type=""Collection(Example.Database.Client)""/>
				<NavigationProperty Name=""ModifiedClients"" Type=""Collection(Example.Database.Client)""/>
				<NavigationProperty Name=""DeletedClients"" Type=""Collection(Example.Database.Client)""/>
				<NavigationProperty Name=""CreatedClientContacts"" Type=""Collection(Example.Database.ClientContact)""/>
				<NavigationProperty Name=""ModifiedClientContacts"" Type=""Collection(Example.Database.ClientContact)""/>
				<NavigationProperty Name=""DeletedClientContacts"" Type=""Collection(Example.Database.ClientContact)""/>
				<NavigationProperty Name=""CreatedProjects"" Type=""Collection(Example.Database.Project)""/>
				<NavigationProperty Name=""ModifiedProjects"" Type=""Collection(Example.Database.Project)""/>
				<NavigationProperty Name=""ProjectsByProjectLeader"" Type=""Collection(Example.Database.Project)""/>
				<NavigationProperty Name=""DeletedProjects"" Type=""Collection(Example.Database.Project)""/>
				<NavigationProperty Name=""ProjectsByAuthorisedBy"" Type=""Collection(Example.Database.Project)""/>
				<NavigationProperty Name=""ProjectsByCancelledBy"" Type=""Collection(Example.Database.Project)""/>
				<NavigationProperty Name=""CreatedExperimentTypes"" Type=""Collection(Example.Database.ExperimentType)""/>
				<NavigationProperty Name=""ModifiedExperimentTypes"" Type=""Collection(Example.Database.ExperimentType)""/>
				<NavigationProperty Name=""DeletedExperimentTypes"" Type=""Collection(Example.Database.ExperimentType)""/>
				<NavigationProperty Name=""CreatedProjectTypes"" Type=""Collection(Example.Database.ProjectType)""/>
				<NavigationProperty Name=""ModifiedProjectTypes"" Type=""Collection(Example.Database.ProjectType)""/>
				<NavigationProperty Name=""DeletedProjectTypes"" Type=""Collection(Example.Database.ProjectType)""/>
				<NavigationProperty Name=""CreatedExperiments"" Type=""Collection(Example.Database.Experiment)""/>
				<NavigationProperty Name=""ModifiedExperiments"" Type=""Collection(Example.Database.Experiment)""/>
				<NavigationProperty Name=""ExperimentsByProjectLeader"" Type=""Collection(Example.Database.Experiment)""/>
				<NavigationProperty Name=""DeletedExperiments"" Type=""Collection(Example.Database.Experiment)""/>
				<NavigationProperty Name=""ExperimentsByAuthorisedBy"" Type=""Collection(Example.Database.Experiment)""/>
				<NavigationProperty Name=""CreatedProjectMembers"" Type=""Collection(Example.Database.ProjectMember)""/>
				<NavigationProperty Name=""ModifiedProjectMembers"" Type=""Collection(Example.Database.ProjectMember)""/>
				<NavigationProperty Name=""ProjectMembers"" Type=""Collection(Example.Database.ProjectMember)""/>
				<NavigationProperty Name=""CreatedExperimentMembers"" Type=""Collection(Example.Database.ExperimentMember)""/>
				<NavigationProperty Name=""ModifiedExperimentMembers"" Type=""Collection(Example.Database.ExperimentMember)""/>
				<NavigationProperty Name=""ExperimentMembers"" Type=""Collection(Example.Database.ExperimentMember)""/>
				<NavigationProperty Name=""CreatedSlotTypes"" Type=""Collection(Example.Database.SlotType)""/>
				<NavigationProperty Name=""ModifiedSlotTypes"" Type=""Collection(Example.Database.SlotType)""/>
				<NavigationProperty Name=""CreatedSlots"" Type=""Collection(Example.Database.Slot)""/>
				<NavigationProperty Name=""ModifiedSlots"" Type=""Collection(Example.Database.Slot)""/>
				<NavigationProperty Name=""CreatedScheduleTypes"" Type=""Collection(Example.Database.ScheduleType)""/>
				<NavigationProperty Name=""ModifiedScheduleTypes"" Type=""Collection(Example.Database.ScheduleType)""/>
				<NavigationProperty Name=""DeletedScheduleTypes"" Type=""Collection(Example.Database.ScheduleType)""/>
				<NavigationProperty Name=""CreatedSchedules"" Type=""Collection(Example.Database.Schedule)""/>
				<NavigationProperty Name=""ModifiedSchedules"" Type=""Collection(Example.Database.Schedule)""/>
				<NavigationProperty Name=""CreatedTests"" Type=""Collection(Example.Database.Test)""/>
				<NavigationProperty Name=""ModifiedTests"" Type=""Collection(Example.Database.Test)""/>
				<NavigationProperty Name=""BKDTestsByInternalResponsible"" Type=""Collection(Example.Database.Test)""/>
				<NavigationProperty Name=""TestsByCancelledBy"" Type=""Collection(Example.Database.Test)""/>
				<NavigationProperty Name=""TestsByCompletedBy"" Type=""Collection(Example.Database.Test)""/>
				<NavigationProperty Name=""TestsByRejectedBy"" Type=""Collection(Example.Database.Test)""/>
				<NavigationProperty Name=""CreatedUnits"" Type=""Collection(Example.Database.Unit)""/>
				<NavigationProperty Name=""ModifiedUnits"" Type=""Collection(Example.Database.Unit)""/>
				<NavigationProperty Name=""DeletedUnits"" Type=""Collection(Example.Database.Unit)""/>
				<NavigationProperty Name=""CreatedResultTypes"" Type=""Collection(Example.Database.ResultType)""/>
				<NavigationProperty Name=""ModifiedResultTypes"" Type=""Collection(Example.Database.ResultType)""/>
				<NavigationProperty Name=""DeletedResultTypes"" Type=""Collection(Example.Database.ResultType)""/>
				<NavigationProperty Name=""CreatedResults"" Type=""Collection(Example.Database.Result)""/>
				<NavigationProperty Name=""ModifiedResults"" Type=""Collection(Example.Database.Result)""/>
				<NavigationProperty Name=""ResultsByEnteredBy"" Type=""Collection(Example.Database.Result)""/>
				<NavigationProperty Name=""ResultsByCancelledBy"" Type=""Collection(Example.Database.Result)""/>
				<NavigationProperty Name=""ResultsByRejectedBy"" Type=""Collection(Example.Database.Result)""/>
				<NavigationProperty Name=""ResultsByAuthorisedBy"" Type=""Collection(Example.Database.Result)""/>
				<NavigationProperty Name=""ResultsByReviewedBy"" Type=""Collection(Example.Database.Result)""/>
				<NavigationProperty Name=""ResultsByCompletedBy"" Type=""Collection(Example.Database.Result)""/>
				<NavigationProperty Name=""CreatedResultTypeArguments"" Type=""Collection(Example.Database.ResultTypeArgument)""/>
				<NavigationProperty Name=""ModifiedResultTypeArguments"" Type=""Collection(Example.Database.ResultTypeArgument)""/>
				<NavigationProperty Name=""CreatedSlotTypeProperties"" Type=""Collection(Example.Database.SlotTypeProperty)""/>
				<NavigationProperty Name=""ModifiedSlotTypeProperties"" Type=""Collection(Example.Database.SlotTypeProperty)""/>
				<NavigationProperty Name=""CreatedScheduleTypeSlotTypes"" Type=""Collection(Example.Database.ScheduleTypeSlotType)""/>
				<NavigationProperty Name=""ModifiedScheduleTypeSlotTypes"" Type=""Collection(Example.Database.ScheduleTypeSlotType)""/>
				<NavigationProperty Name=""CreatedScheduleItems"" Type=""Collection(Example.Database.ScheduleItem)""/>
				<NavigationProperty Name=""ModifiedScheduleItems"" Type=""Collection(Example.Database.ScheduleItem)""/>
				<NavigationProperty Name=""DeletedScheduleItems"" Type=""Collection(Example.Database.ScheduleItem)""/>
				<NavigationProperty Name=""CreatedSlotItems"" Type=""Collection(Example.Database.SlotItem)""/>
				<NavigationProperty Name=""ModifiedSlotItems"" Type=""Collection(Example.Database.SlotItem)""/>
				<NavigationProperty Name=""CreatedSampleTypes"" Type=""Collection(Example.Database.SampleType)""/>
				<NavigationProperty Name=""ModifiedSampleTypes"" Type=""Collection(Example.Database.SampleType)""/>
				<NavigationProperty Name=""DeletedSampleTypes"" Type=""Collection(Example.Database.SampleType)""/>
				<NavigationProperty Name=""CreatedSamples"" Type=""Collection(Example.Database.Sample)""/>
				<NavigationProperty Name=""ModifiedSamples"" Type=""Collection(Example.Database.Sample)""/>
				<NavigationProperty Name=""BKDSamplesByInProcessBy"" Type=""Collection(Example.Database.Sample)""/>
				<NavigationProperty Name=""BKDSamplesByGrindedBy"" Type=""Collection(Example.Database.Sample)""/>
				<NavigationProperty Name=""BKDSamplesByPipettedBy"" Type=""Collection(Example.Database.Sample)""/>
				<NavigationProperty Name=""BKDSamplesByInternalResponsible"" Type=""Collection(Example.Database.Sample)""/>
				<NavigationProperty Name=""DeletedSamples"" Type=""Collection(Example.Database.Sample)""/>
				<NavigationProperty Name=""SamplesByCancelledBy"" Type=""Collection(Example.Database.Sample)""/>
				<NavigationProperty Name=""SamplesByAuthorisedBy"" Type=""Collection(Example.Database.Sample)""/>
				<NavigationProperty Name=""SamplesByRejectedBy"" Type=""Collection(Example.Database.Sample)""/>
				<NavigationProperty Name=""CreatedTestTypes"" Type=""Collection(Example.Database.TestType)""/>
				<NavigationProperty Name=""ModifiedTestTypes"" Type=""Collection(Example.Database.TestType)""/>
				<NavigationProperty Name=""DeletedTestTypes"" Type=""Collection(Example.Database.TestType)""/>
				<NavigationProperty Name=""CreatedTestTypeReportTypes"" Type=""Collection(Example.Database.TestTypeReportType)""/>
				<NavigationProperty Name=""ModifiedTestTypeReportTypes"" Type=""Collection(Example.Database.TestTypeReportType)""/>
				<NavigationProperty Name=""CreatedOrderTypeFields"" Type=""Collection(Example.Database.OrderTypeField)""/>
				<NavigationProperty Name=""ModifiedOrderTypeFields"" Type=""Collection(Example.Database.OrderTypeField)""/>
				<NavigationProperty Name=""CreatedSampleTypeFields"" Type=""Collection(Example.Database.SampleTypeField)""/>
				<NavigationProperty Name=""ModifiedSampleTypeFields"" Type=""Collection(Example.Database.SampleTypeField)""/>
				<NavigationProperty Name=""CreatedTestTypeFields"" Type=""Collection(Example.Database.TestTypeField)""/>
				<NavigationProperty Name=""ModifiedTestTypeFields"" Type=""Collection(Example.Database.TestTypeField)""/>
				<NavigationProperty Name=""CreatedResultTypeFields"" Type=""Collection(Example.Database.ResultTypeField)""/>
				<NavigationProperty Name=""ModifiedResultTypeFields"" Type=""Collection(Example.Database.ResultTypeField)""/>
				<NavigationProperty Name=""CreatedAdditionalTestTypes"" Type=""Collection(Example.Database.AdditionalTestType)""/>
				<NavigationProperty Name=""ModifiedAdditionalTestTypes"" Type=""Collection(Example.Database.AdditionalTestType)""/>
				<NavigationProperty Name=""CreatedSlotProperties"" Type=""Collection(Example.Database.SlotProperty)""/>
				<NavigationProperty Name=""ModifiedSlotProperties"" Type=""Collection(Example.Database.SlotProperty)""/>
				<NavigationProperty Name=""CreatedOrderTypes"" Type=""Collection(Example.Database.OrderType)""/>
				<NavigationProperty Name=""ModifiedOrderTypes"" Type=""Collection(Example.Database.OrderType)""/>
				<NavigationProperty Name=""DeletedOrderTypes"" Type=""Collection(Example.Database.OrderType)""/>
				<NavigationProperty Name=""CreatedOrders"" Type=""Collection(Example.Database.Order)""/>
				<NavigationProperty Name=""ModifiedOrders"" Type=""Collection(Example.Database.Order)""/>
				<NavigationProperty Name=""DeletedOrders"" Type=""Collection(Example.Database.Order)""/>
				<NavigationProperty Name=""OrdersByCancelledBy"" Type=""Collection(Example.Database.Order)""/>
				<NavigationProperty Name=""OrdersByRejectedBy"" Type=""Collection(Example.Database.Order)""/>
				<NavigationProperty Name=""OrdersByReceivedBy"" Type=""Collection(Example.Database.Order)""/>
				<NavigationProperty Name=""CreatedOrderReports"" Type=""Collection(Example.Database.OrderReport)""/>
				<NavigationProperty Name=""ModifiedOrderReports"" Type=""Collection(Example.Database.OrderReport)""/>
				<NavigationProperty Name=""OrderReportsByCancelledBy"" Type=""Collection(Example.Database.OrderReport)""/>
				<NavigationProperty Name=""OrderReportsByReviewedBy"" Type=""Collection(Example.Database.OrderReport)""/>
				<NavigationProperty Name=""CreatedOrderReportDestinations"" Type=""Collection(Example.Database.OrderReportDestination)""/>
				<NavigationProperty Name=""ModifiedOrderReportDestinations"" Type=""Collection(Example.Database.OrderReportDestination)""/>
				<NavigationProperty Name=""OrderReportDestinations"" Type=""Collection(Example.Database.OrderReportDestination)""/>
				<NavigationProperty Name=""CreatedClientReportTypes"" Type=""Collection(Example.Database.ClientReportType)""/>
				<NavigationProperty Name=""ModifiedClientReportTypes"" Type=""Collection(Example.Database.ClientReportType)""/>
				<NavigationProperty Name=""CreatedProjectDocuments"" Type=""Collection(Example.Database.ProjectDocument)""/>
				<NavigationProperty Name=""ModifiedProjectDocuments"" Type=""Collection(Example.Database.ProjectDocument)""/>
				<NavigationProperty Name=""CreatedContainerTypes"" Type=""Collection(Example.Database.ContainerType)""/>
				<NavigationProperty Name=""ModifiedContainerTypes"" Type=""Collection(Example.Database.ContainerType)""/>
				<NavigationProperty Name=""DeletedContainerTypes"" Type=""Collection(Example.Database.ContainerType)""/>
				<NavigationProperty Name=""CreatedContainers"" Type=""Collection(Example.Database.Container)""/>
				<NavigationProperty Name=""ModifiedContainers"" Type=""Collection(Example.Database.Container)""/>
				<NavigationProperty Name=""CreatedSampleContainers"" Type=""Collection(Example.Database.SampleContainer)""/>
				<NavigationProperty Name=""ModifiedSampleContainers"" Type=""Collection(Example.Database.SampleContainer)""/>
				<NavigationProperty Name=""ModifiedSampleTypeContainerTypes"" Type=""Collection(Example.Database.SampleTypeContainerType)""/>
				<NavigationProperty Name=""CreatedSampleTypeContainerTypes"" Type=""Collection(Example.Database.SampleTypeContainerType)""/>
				<NavigationProperty Name=""CreatedGraphTypes"" Type=""Collection(Example.Database.GraphType)""/>
				<NavigationProperty Name=""ModifiedGraphTypes"" Type=""Collection(Example.Database.GraphType)""/>
				<NavigationProperty Name=""DeletedGraphTypes"" Type=""Collection(Example.Database.GraphType)""/>
				<NavigationProperty Name=""CreatedGraphTypeSeries"" Type=""Collection(Example.Database.GraphTypeSerie)""/>
				<NavigationProperty Name=""ModifiedGraphTypeSeries"" Type=""Collection(Example.Database.GraphTypeSerie)""/>
				<NavigationProperty Name=""DeletedGraphTypeSeries"" Type=""Collection(Example.Database.GraphTypeSerie)""/>
				<NavigationProperty Name=""CreatedRackTypes"" Type=""Collection(Example.Database.RackType)""/>
				<NavigationProperty Name=""ModifiedRackTypes"" Type=""Collection(Example.Database.RackType)""/>
				<NavigationProperty Name=""DeletedRackTypes"" Type=""Collection(Example.Database.RackType)""/>
				<NavigationProperty Name=""CreatedRacks"" Type=""Collection(Example.Database.Rack)""/>
				<NavigationProperty Name=""ModifiedRacks"" Type=""Collection(Example.Database.Rack)""/>
				<NavigationProperty Name=""BKDRacksByFirstMeasurementBy"" Type=""Collection(Example.Database.Rack)""/>
				<NavigationProperty Name=""BKDRacksBySecondMeasurementBy"" Type=""Collection(Example.Database.Rack)""/>
				<NavigationProperty Name=""DeletedRacks"" Type=""Collection(Example.Database.Rack)""/>
				<NavigationProperty Name=""CreatedRackSamples"" Type=""Collection(Example.Database.RackSample)""/>
				<NavigationProperty Name=""ModifiedRackSamples"" Type=""Collection(Example.Database.RackSample)""/>
				<NavigationProperty Name=""BKDRackSamplesByFirstMeasurementBy"" Type=""Collection(Example.Database.RackSample)""/>
				<NavigationProperty Name=""BKDRackSamplesBySecondMeasurementBy"" Type=""Collection(Example.Database.RackSample)""/>
				<NavigationProperty Name=""DeletedRackSamples"" Type=""Collection(Example.Database.RackSample)""/>
				<NavigationProperty Name=""CreatedProcessStationTypes"" Type=""Collection(Example.Database.ProcessStationType)""/>
				<NavigationProperty Name=""ModifiedProcessStationTypes"" Type=""Collection(Example.Database.ProcessStationType)""/>
				<NavigationProperty Name=""DeletedProcessStationTypes"" Type=""Collection(Example.Database.ProcessStationType)""/>
				<NavigationProperty Name=""CreatedProcessStations"" Type=""Collection(Example.Database.ProcessStation)""/>
				<NavigationProperty Name=""ModifiedProcessStations"" Type=""Collection(Example.Database.ProcessStation)""/>
				<NavigationProperty Name=""DeletedProcessStations"" Type=""Collection(Example.Database.ProcessStation)""/>
				<NavigationProperty Name=""CreatedSampleTypeTestTypes"" Type=""Collection(Example.Database.SampleTypeTestType)""/>
				<NavigationProperty Name=""ModifiedSampleTypeTestTypes"" Type=""Collection(Example.Database.SampleTypeTestType)""/>
				<NavigationProperty Name=""CreatedOrderTypeSampleTypes"" Type=""Collection(Example.Database.OrderTypeSampleType)""/>
				<NavigationProperty Name=""ModifiedOrderTypeSampleTypes"" Type=""Collection(Example.Database.OrderTypeSampleType)""/>
				<NavigationProperty Name=""CreatedTestTypeResultTypes"" Type=""Collection(Example.Database.TestTypeResultType)""/>
				<NavigationProperty Name=""ModifiedTestTypeResultTypes"" Type=""Collection(Example.Database.TestTypeResultType)""/>
				<NavigationProperty Name=""CreatedProtocols"" Type=""Collection(Example.Database.Protocol)""/>
				<NavigationProperty Name=""ModifiedProtocols"" Type=""Collection(Example.Database.Protocol)""/>
				<NavigationProperty Name=""ProtocolsByProjectLeader"" Type=""Collection(Example.Database.Protocol)""/>
				<NavigationProperty Name=""CreatedQualificationTypes"" Type=""Collection(Example.Database.QualificationType)""/>
				<NavigationProperty Name=""ModifiedQualificationTypes"" Type=""Collection(Example.Database.QualificationType)""/>
				<NavigationProperty Name=""DeletedQualificationTypes"" Type=""Collection(Example.Database.QualificationType)""/>
				<NavigationProperty Name=""CreatedUserQualifications"" Type=""Collection(Example.Database.UserQualification)""/>
				<NavigationProperty Name=""ModifiedUserQualifications"" Type=""Collection(Example.Database.UserQualification)""/>
				<NavigationProperty Name=""UserQualifications"" Type=""Collection(Example.Database.UserQualification)""/>
				<NavigationProperty Name=""DeletedUserQualifications"" Type=""Collection(Example.Database.UserQualification)""/>
				<NavigationProperty Name=""CreatedScheduleTypeQualifications"" Type=""Collection(Example.Database.ScheduleTypeQualification)""/>
				<NavigationProperty Name=""ModifiedScheduleTypeQualifications"" Type=""Collection(Example.Database.ScheduleTypeQualification)""/>
				<NavigationProperty Name=""CreatedChemicals"" Type=""Collection(Example.Database.Chemical)""/>
				<NavigationProperty Name=""ModifiedChemicals"" Type=""Collection(Example.Database.Chemical)""/>
				<NavigationProperty Name=""DeletedChemicals"" Type=""Collection(Example.Database.Chemical)""/>
				<NavigationProperty Name=""CreatedReactions"" Type=""Collection(Example.Database.Reaction)""/>
				<NavigationProperty Name=""ModifiedReactions"" Type=""Collection(Example.Database.Reaction)""/>
				<NavigationProperty Name=""DeletedReactions"" Type=""Collection(Example.Database.Reaction)""/>
				<NavigationProperty Name=""CreatedInstrumentTypes"" Type=""Collection(Example.Database.InstrumentType)""/>
				<NavigationProperty Name=""ModifiedInstrumentTypes"" Type=""Collection(Example.Database.InstrumentType)""/>
				<NavigationProperty Name=""DeletedInstrumentTypes"" Type=""Collection(Example.Database.InstrumentType)""/>
				<NavigationProperty Name=""CreatedInstruments"" Type=""Collection(Example.Database.Instrument)""/>
				<NavigationProperty Name=""ModifiedInstruments"" Type=""Collection(Example.Database.Instrument)""/>
				<NavigationProperty Name=""InstrumentsByManagedBy"" Type=""Collection(Example.Database.Instrument)""/>
				<NavigationProperty Name=""DeletedInstruments"" Type=""Collection(Example.Database.Instrument)""/>
				<NavigationProperty Name=""CreatedInstrumentFiles"" Type=""Collection(Example.Database.InstrumentFile)""/>
				<NavigationProperty Name=""ModifiedInstrumentFiles"" Type=""Collection(Example.Database.InstrumentFile)""/>
				<NavigationProperty Name=""DeletedInstrumentFiles"" Type=""Collection(Example.Database.InstrumentFile)""/>
				<NavigationProperty Name=""InstrumentFilesByCancelledBy"" Type=""Collection(Example.Database.InstrumentFile)""/>
				<NavigationProperty Name=""CreatedMaterialTypes"" Type=""Collection(Example.Database.MaterialType)""/>
				<NavigationProperty Name=""ModifiedMaterialTypes"" Type=""Collection(Example.Database.MaterialType)""/>
				<NavigationProperty Name=""DeletedMaterialTypes"" Type=""Collection(Example.Database.MaterialType)""/>
				<NavigationProperty Name=""CreatedMaterials"" Type=""Collection(Example.Database.Material)""/>
				<NavigationProperty Name=""ModifiedMaterials"" Type=""Collection(Example.Database.Material)""/>
				<NavigationProperty Name=""DeletedMaterials"" Type=""Collection(Example.Database.Material)""/>
				<NavigationProperty Name=""MaterialsByAuthorisedBy"" Type=""Collection(Example.Database.Material)""/>
				<NavigationProperty Name=""MaterialsByReceivedBy"" Type=""Collection(Example.Database.Material)""/>
				<NavigationProperty Name=""CreatedBatchTypes"" Type=""Collection(Example.Database.BatchType)""/>
				<NavigationProperty Name=""ModifiedBatchTypes"" Type=""Collection(Example.Database.BatchType)""/>
				<NavigationProperty Name=""DeletedBatchTypes"" Type=""Collection(Example.Database.BatchType)""/>
				<NavigationProperty Name=""CreatedBatchFormulationTypes"" Type=""Collection(Example.Database.BatchFormulationType)""/>
				<NavigationProperty Name=""ModifiedBatchFormulationTypes"" Type=""Collection(Example.Database.BatchFormulationType)""/>
				<NavigationProperty Name=""DeletedBatchFormulationTypes"" Type=""Collection(Example.Database.BatchFormulationType)""/>
				<NavigationProperty Name=""CreatedBatches"" Type=""Collection(Example.Database.Batch)""/>
				<NavigationProperty Name=""ModifiedBatches"" Type=""Collection(Example.Database.Batch)""/>
				<NavigationProperty Name=""DeletedBatches"" Type=""Collection(Example.Database.Batch)""/>
				<NavigationProperty Name=""CreatedProductTypes"" Type=""Collection(Example.Database.ProductType)""/>
				<NavigationProperty Name=""ModifiedProductTypes"" Type=""Collection(Example.Database.ProductType)""/>
				<NavigationProperty Name=""DeletedProductTypes"" Type=""Collection(Example.Database.ProductType)""/>
				<NavigationProperty Name=""CreatedProductUnitTypes"" Type=""Collection(Example.Database.ProductUnitType)""/>
				<NavigationProperty Name=""ModifiedProductUnitTypes"" Type=""Collection(Example.Database.ProductUnitType)""/>
				<NavigationProperty Name=""DeletedProductUnitTypes"" Type=""Collection(Example.Database.ProductUnitType)""/>
				<NavigationProperty Name=""CreatedProductUnits"" Type=""Collection(Example.Database.ProductUnit)""/>
				<NavigationProperty Name=""ModifiedProductUnits"" Type=""Collection(Example.Database.ProductUnit)""/>
				<NavigationProperty Name=""DeletedProductUnits"" Type=""Collection(Example.Database.ProductUnit)""/>
				<NavigationProperty Name=""ModifiedControlChartTypes"" Type=""Collection(Example.Database.ControlChartType)""/>
				<NavigationProperty Name=""CreatedControlChartTypes"" Type=""Collection(Example.Database.ControlChartType)""/>
				<NavigationProperty Name=""DeletedControlChartTypes"" Type=""Collection(Example.Database.ControlChartType)""/>
				<NavigationProperty Name=""ModifiedControlCharts"" Type=""Collection(Example.Database.ControlChart)""/>
				<NavigationProperty Name=""CreatedControlCharts"" Type=""Collection(Example.Database.ControlChart)""/>
				<NavigationProperty Name=""DeletedControlCharts"" Type=""Collection(Example.Database.ControlChart)""/>
				<NavigationProperty Name=""ModifiedControlChartResults"" Type=""Collection(Example.Database.ControlChartResult)""/>
				<NavigationProperty Name=""CreatedControlChartResults"" Type=""Collection(Example.Database.ControlChartResult)""/>
				<NavigationProperty Name=""CreatedSequenceTypes"" Type=""Collection(Example.Database.SequenceType)""/>
				<NavigationProperty Name=""ModifiedSequenceTypes"" Type=""Collection(Example.Database.SequenceType)""/>
				<NavigationProperty Name=""DeletedSequenceTypes"" Type=""Collection(Example.Database.SequenceType)""/>
				<NavigationProperty Name=""CreatedSequences"" Type=""Collection(Example.Database.Sequence)""/>
				<NavigationProperty Name=""ModifiedSequences"" Type=""Collection(Example.Database.Sequence)""/>
				<NavigationProperty Name=""DeletedSequences"" Type=""Collection(Example.Database.Sequence)""/>
				<NavigationProperty Name=""SequencesByCompletedBy"" Type=""Collection(Example.Database.Sequence)""/>
				<NavigationProperty Name=""CreatedSequenceSamples"" Type=""Collection(Example.Database.SequenceSample)""/>
				<NavigationProperty Name=""ModifiedSequenceSamples"" Type=""Collection(Example.Database.SequenceSample)""/>
				<NavigationProperty Name=""DeletedSequenceSamples"" Type=""Collection(Example.Database.SequenceSample)""/>
				<NavigationProperty Name=""CreatedSequenceSampleResults"" Type=""Collection(Example.Database.SequenceSampleResult)""/>
				<NavigationProperty Name=""ModifiedSequenceSampleResults"" Type=""Collection(Example.Database.SequenceSampleResult)""/>
				<NavigationProperty Name=""DeletedSequenceSampleResults"" Type=""Collection(Example.Database.SequenceSampleResult)""/>
				<NavigationProperty Name=""CreatedInstrumentTypeResultTypes"" Type=""Collection(Example.Database.InstrumentTypeResultType)""/>
				<NavigationProperty Name=""ModifiedInstrumentTypeResultTypes"" Type=""Collection(Example.Database.InstrumentTypeResultType)""/>
				<NavigationProperty Name=""DeletedInstrumentTypeResultTypes"" Type=""Collection(Example.Database.InstrumentTypeResultType)""/>
				<NavigationProperty Name=""CreatedProducts"" Type=""Collection(Example.Database.Product)""/>
				<NavigationProperty Name=""ModifiedProducts"" Type=""Collection(Example.Database.Product)""/>
				<NavigationProperty Name=""DeletedProducts"" Type=""Collection(Example.Database.Product)""/>
				<NavigationProperty Name=""CreatedOrderForms"" Type=""Collection(Example.Database.OrderForm)""/>
				<NavigationProperty Name=""ModifiedOrderForms"" Type=""Collection(Example.Database.OrderForm)""/>
				<NavigationProperty Name=""DeletedOrderForms"" Type=""Collection(Example.Database.OrderForm)""/>
				<NavigationProperty Name=""CreatedClientProduct"" Type=""Collection(Example.Database.ClientProduct)""/>
				<NavigationProperty Name=""ModifiedClientProduct"" Type=""Collection(Example.Database.ClientProduct)""/>
				<NavigationProperty Name=""CreatedOrderFormFields"" Type=""Collection(Example.Database.OrderFormField)""/>
				<NavigationProperty Name=""ModifiedOrderFormFields"" Type=""Collection(Example.Database.OrderFormField)""/>
				<NavigationProperty Name=""CreatedOrderFormSamples"" Type=""Collection(Example.Database.OrderFormSample)""/>
				<NavigationProperty Name=""ModifiedOrderFormSamples"" Type=""Collection(Example.Database.OrderFormSample)""/>
				<NavigationProperty Name=""DeletedOrderFormSamples"" Type=""Collection(Example.Database.OrderFormSample)""/>
				<NavigationProperty Name=""CreatedOrderFormSampleFields"" Type=""Collection(Example.Database.OrderFormSampleField)""/>
				<NavigationProperty Name=""ModifiedOrderFormSampleFields"" Type=""Collection(Example.Database.OrderFormSampleField)""/>
				<NavigationProperty Name=""CreatedOrderFormTests"" Type=""Collection(Example.Database.OrderFormTest)""/>
				<NavigationProperty Name=""ModifiedOrderFormTests"" Type=""Collection(Example.Database.OrderFormTest)""/>
				<NavigationProperty Name=""CreatedOrderFormReports"" Type=""Collection(Example.Database.OrderFormReport)""/>
				<NavigationProperty Name=""ModifiedOrderFormReports"" Type=""Collection(Example.Database.OrderFormReport)""/>
				<NavigationProperty Name=""DeletedOrderFormReports"" Type=""Collection(Example.Database.OrderFormReport)""/>
				<NavigationProperty Name=""CreatedOrderFormReportDestinations"" Type=""Collection(Example.Database.OrderFormReportDestination)""/>
				<NavigationProperty Name=""ModifiedOrderFormReportDestinations"" Type=""Collection(Example.Database.OrderFormReportDestination)""/>
				<NavigationProperty Name=""OrderFormReportDestinations"" Type=""Collection(Example.Database.OrderFormReportDestination)""/>
				<NavigationProperty Name=""CreatedPriceLists"" Type=""Collection(Example.Database.PriceList)""/>
				<NavigationProperty Name=""ModifiedPriceLists"" Type=""Collection(Example.Database.PriceList)""/>
				<NavigationProperty Name=""DeletedPriceLists"" Type=""Collection(Example.Database.PriceList)""/>
				<NavigationProperty Name=""CreatedPriceListTestTypes"" Type=""Collection(Example.Database.PriceListTestType)""/>
				<NavigationProperty Name=""ModifiedPriceListTestTypes"" Type=""Collection(Example.Database.PriceListTestType)""/>
				<NavigationProperty Name=""DeletedPriceListTestTypes"" Type=""Collection(Example.Database.PriceListTestType)""/>
				<NavigationProperty Name=""ModifiedPriceListPriorities"" Type=""Collection(Example.Database.PriceListPriority)""/>
				<NavigationProperty Name=""CreatedPriceListPriorities"" Type=""Collection(Example.Database.PriceListPriority)""/>
				<NavigationProperty Name=""DeletedPriceListPriorities"" Type=""Collection(Example.Database.PriceListPriority)""/>
				<NavigationProperty Name=""CreatedAvailabilityRuleTypes"" Type=""Collection(Example.Database.AvailabilityRuleType)""/>
				<NavigationProperty Name=""ModifiedAvailabilityRuleTypes"" Type=""Collection(Example.Database.AvailabilityRuleType)""/>
				<NavigationProperty Name=""DeletedAvailabilityRuleTypes"" Type=""Collection(Example.Database.AvailabilityRuleType)""/>
				<NavigationProperty Name=""CreatedAvailabilityRules"" Type=""Collection(Example.Database.AvailabilityRule)""/>
				<NavigationProperty Name=""ModifiedAvailabilityRules"" Type=""Collection(Example.Database.AvailabilityRule)""/>
				<NavigationProperty Name=""AvailabilityRules"" Type=""Collection(Example.Database.AvailabilityRule)""/>
				<NavigationProperty Name=""DeletedAvailabilityRules"" Type=""Collection(Example.Database.AvailabilityRule)""/>
				<NavigationProperty Name=""CreatedScheduleRules"" Type=""Collection(Example.Database.ScheduleRule)""/>
				<NavigationProperty Name=""ModifiedScheduleRules"" Type=""Collection(Example.Database.ScheduleRule)""/>
				<NavigationProperty Name=""ScheduleRules"" Type=""Collection(Example.Database.ScheduleRule)""/>
				<NavigationProperty Name=""DeletedScheduleRules"" Type=""Collection(Example.Database.ScheduleRule)""/>
				<NavigationProperty Name=""CreatedUserAvailabilitys"" Type=""Collection(Example.Database.UserAvailability)""/>
				<NavigationProperty Name=""ModifiedUserAvailabilitys"" Type=""Collection(Example.Database.UserAvailability)""/>
				<NavigationProperty Name=""UserAvailabilitys"" Type=""Collection(Example.Database.UserAvailability)""/>
				<NavigationProperty Name=""CreatedTestTypeMethods"" Type=""Collection(Example.Database.TestTypeMethod)""/>
				<NavigationProperty Name=""ModifiedTestTypeMethods"" Type=""Collection(Example.Database.TestTypeMethod)""/>
				<NavigationProperty Name=""CreatedMethods"" Type=""Collection(Example.Database.Method)""/>
				<NavigationProperty Name=""ModifiedMethods"" Type=""Collection(Example.Database.Method)""/>
				<NavigationProperty Name=""CreatedEntitySpecifications"" Type=""Collection(Example.Database.EntitySpecification)""/>
				<NavigationProperty Name=""ModifiedEntitySpecifications"" Type=""Collection(Example.Database.EntitySpecification)""/>
				<NavigationProperty Name=""CreatedSpecifications"" Type=""Collection(Example.Database.Specification)""/>
				<NavigationProperty Name=""ModifiedSpecifications"" Type=""Collection(Example.Database.Specification)""/>
				<NavigationProperty Name=""DeletedSpecifications"" Type=""Collection(Example.Database.Specification)""/>
				<NavigationProperty Name=""CreatedSpecificationItems"" Type=""Collection(Example.Database.SpecificationItem)""/>
				<NavigationProperty Name=""ModifiedSpecificationItems"" Type=""Collection(Example.Database.SpecificationItem)""/>
				<NavigationProperty Name=""DeletedSpecificationItems"" Type=""Collection(Example.Database.SpecificationItem)""/>
				<NavigationProperty Name=""CreatedSpecificationResults"" Type=""Collection(Example.Database.SpecificationResult)""/>
				<NavigationProperty Name=""ModifiedSpecificationResults"" Type=""Collection(Example.Database.SpecificationResult)""/>
				<NavigationProperty Name=""CreatedClientUsers"" Type=""Collection(Example.Database.ClientUser)""/>
				<NavigationProperty Name=""ModifiedClientUsers"" Type=""Collection(Example.Database.ClientUser)""/>
				<NavigationProperty Name=""ClientUsers"" Type=""Collection(Example.Database.ClientUser)""/>
				<NavigationProperty Name=""CreatedInvoices"" Type=""Collection(Example.Database.Invoice)""/>
				<NavigationProperty Name=""ModifiedInvoices"" Type=""Collection(Example.Database.Invoice)""/>
				<NavigationProperty Name=""DeletedInvoices"" Type=""Collection(Example.Database.Invoice)""/>
				<NavigationProperty Name=""CreatedInvoiceOrders"" Type=""Collection(Example.Database.InvoiceOrder)""/>
				<NavigationProperty Name=""ModifiedInvoiceOrders"" Type=""Collection(Example.Database.InvoiceOrder)""/>
				<NavigationProperty Name=""CreatedInvoiceItems"" Type=""Collection(Example.Database.InvoiceItem)""/>
				<NavigationProperty Name=""ModifiedInvoiceItems"" Type=""Collection(Example.Database.InvoiceItem)""/>
				<NavigationProperty Name=""DeletedInvoiceItems"" Type=""Collection(Example.Database.InvoiceItem)""/>
				<NavigationProperty Name=""CreatedVatTypes"" Type=""Collection(Example.Database.VatType)""/>
				<NavigationProperty Name=""ModifiedVatTypes"" Type=""Collection(Example.Database.VatType)""/>
				<NavigationProperty Name=""CreatedBatchItems"" Type=""Collection(Example.Database.BatchItem)""/>
				<NavigationProperty Name=""ModifiedBatchItems"" Type=""Collection(Example.Database.BatchItem)""/>
				<NavigationProperty Name=""CreatedContainerTypeFields"" Type=""Collection(Example.Database.ContainerTypeField)""/>
				<NavigationProperty Name=""ModifiedContainerTypeFields"" Type=""Collection(Example.Database.ContainerTypeField)""/>
				<NavigationProperty Name=""CreatedPriorities"" Type=""Collection(Example.Database.Priority)""/>
				<NavigationProperty Name=""ModifiedPriorities"" Type=""Collection(Example.Database.Priority)""/>
				<NavigationProperty Name=""DeletedPriorities"" Type=""Collection(Example.Database.Priority)""/>
				<NavigationProperty Name=""CreatedTestTypePriorities"" Type=""Collection(Example.Database.TestTypePriority)""/>
				<NavigationProperty Name=""ModifiedTestTypePriorities"" Type=""Collection(Example.Database.TestTypePriority)""/>
				<NavigationProperty Name=""ModifiedLaboratories"" Type=""Collection(Example.Database.Laboratory)""/>
				<NavigationProperty Name=""CreatedLaboratories"" Type=""Collection(Example.Database.Laboratory)""/>
				<NavigationProperty Name=""DeletedLaboratories"" Type=""Collection(Example.Database.Laboratory)""/>
				<NavigationProperty Name=""CreatedQuotations"" Type=""Collection(Example.Database.Quotation)""/>
				<NavigationProperty Name=""ModifiedQuotations"" Type=""Collection(Example.Database.Quotation)""/>
				<NavigationProperty Name=""QuotationsByCancelledBy"" Type=""Collection(Example.Database.Quotation)""/>
				<NavigationProperty Name=""QuotationsByAuthorisedBy"" Type=""Collection(Example.Database.Quotation)""/>
				<NavigationProperty Name=""DeletedQuotations"" Type=""Collection(Example.Database.Quotation)""/>
				<NavigationProperty Name=""CreatedQuotationItemTypes"" Type=""Collection(Example.Database.QuotationItemType)""/>
				<NavigationProperty Name=""ModifiedQuotationItemTypes"" Type=""Collection(Example.Database.QuotationItemType)""/>
				<NavigationProperty Name=""DeletedQuotationItemTypes"" Type=""Collection(Example.Database.QuotationItemType)""/>
				<NavigationProperty Name=""CreatedArticles"" Type=""Collection(Example.Database.Article)""/>
				<NavigationProperty Name=""ModifiedArticles"" Type=""Collection(Example.Database.Article)""/>
				<NavigationProperty Name=""ModifiedArticleFields"" Type=""Collection(Example.Database.ArticleField)""/>
				<NavigationProperty Name=""CreatedArticleFields"" Type=""Collection(Example.Database.ArticleField)""/>
				<NavigationProperty Name=""CreatedQuotationLocations"" Type=""Collection(Example.Database.QuotationLocation)""/>
				<NavigationProperty Name=""ModifiedQuotationLocations"" Type=""Collection(Example.Database.QuotationLocation)""/>
				<NavigationProperty Name=""DeletedQuotationLocations"" Type=""Collection(Example.Database.QuotationLocation)""/>
				<NavigationProperty Name=""CreatedQuotationTotals"" Type=""Collection(Example.Database.QuotationTotal)""/>
				<NavigationProperty Name=""ModifiedQuotationTotals"" Type=""Collection(Example.Database.QuotationTotal)""/>
				<NavigationProperty Name=""CreatedQuotationItems"" Type=""Collection(Example.Database.QuotationItem)""/>
				<NavigationProperty Name=""ModifiedQuotationItems"" Type=""Collection(Example.Database.QuotationItem)""/>
				<NavigationProperty Name=""DeletedQuotationItems"" Type=""Collection(Example.Database.QuotationItem)""/>
				<NavigationProperty Name=""CreatedSequenceTypeResultTypes"" Type=""Collection(Example.Database.SequenceTypeResultType)""/>
				<NavigationProperty Name=""ModifiedSequenceTypeResultTypes"" Type=""Collection(Example.Database.SequenceTypeResultType)""/>
				<NavigationProperty Name=""ModifiedResultTypeEvents"" Type=""Collection(Example.Database.ResultTypeEvent)""/>
				<NavigationProperty Name=""CreatedResultTypeEvents"" Type=""Collection(Example.Database.ResultTypeEvent)""/>
				<NavigationProperty Name=""CreatedTestTypeTasks"" Type=""Collection(Example.Database.TestTypeTask)""/>
				<NavigationProperty Name=""ModifiedTestTypeTasks"" Type=""Collection(Example.Database.TestTypeTask)""/>
				<NavigationProperty Name=""CreatedInstrumentParts"" Type=""Collection(Example.Database.InstrumentPart)""/>
				<NavigationProperty Name=""ModifiedInstrumentParts"" Type=""Collection(Example.Database.InstrumentPart)""/>
				<NavigationProperty Name=""DeletedInstrumentParts"" Type=""Collection(Example.Database.InstrumentPart)""/>
				<NavigationProperty Name=""CreatedAccreditations"" Type=""Collection(Example.Database.Accreditation)""/>
				<NavigationProperty Name=""ModifiedAccreditations"" Type=""Collection(Example.Database.Accreditation)""/>
				<NavigationProperty Name=""CreatedAccreditationTestTypes"" Type=""Collection(Example.Database.AccreditationTestType)""/>
				<NavigationProperty Name=""ModifiedAccreditationTestTypes"" Type=""Collection(Example.Database.AccreditationTestType)""/>
				<NavigationProperty Name=""CreatedSampleTypeTasks"" Type=""Collection(Example.Database.SampleTypeTask)""/>
				<NavigationProperty Name=""ModifiedSampleTypeTasks"" Type=""Collection(Example.Database.SampleTypeTask)""/>
				<NavigationProperty Name=""CreatedTestTasks"" Type=""Collection(Example.Database.TestTask)""/>
				<NavigationProperty Name=""ModifiedTestTasks"" Type=""Collection(Example.Database.TestTask)""/>
				<NavigationProperty Name=""CreatedSampleTasks"" Type=""Collection(Example.Database.SampleTask)""/>
				<NavigationProperty Name=""ModifiedSampleTasks"" Type=""Collection(Example.Database.SampleTask)""/>
				<NavigationProperty Name=""TestTypeQualityControlTriggers"" Type=""Collection(Example.Database.TestTypeQualityControlTrigger)""/>
				<NavigationProperty Name=""ModifiedTestTypeQualityControlTriggers"" Type=""Collection(Example.Database.TestTypeQualityControlTrigger)""/>
				<NavigationProperty Name=""CreatedTestTypeQualityControlTriggers"" Type=""Collection(Example.Database.TestTypeQualityControlTrigger)""/>
				<NavigationProperty Name=""CreatedScheduleRuleSets"" Type=""Collection(Example.Database.ScheduleRuleSet)""/>
				<NavigationProperty Name=""ModifiedScheduleRuleSets"" Type=""Collection(Example.Database.ScheduleRuleSet)""/>
				<NavigationProperty Name=""ScheduleRuleSets"" Type=""Collection(Example.Database.ScheduleRuleSet)""/>
				<NavigationProperty Name=""DeletedScheduleRuleSets"" Type=""Collection(Example.Database.ScheduleRuleSet)""/>
				<NavigationProperty Name=""ModifiedLaboratorySampleTypes"" Type=""Collection(Example.Database.LaboratorySampleType)""/>
				<NavigationProperty Name=""CreatedLaboratorySampleTypes"" Type=""Collection(Example.Database.LaboratorySampleType)""/>
				<NavigationProperty Name=""ModifiedLaboratoryTestTypes"" Type=""Collection(Example.Database.LaboratoryTestType)""/>
				<NavigationProperty Name=""CreatedLaboratoryTestTypes"" Type=""Collection(Example.Database.LaboratoryTestType)""/>
				<NavigationProperty Name=""ModifiedLaboratoryUnits"" Type=""Collection(Example.Database.LaboratoryUnit)""/>
				<NavigationProperty Name=""CreatedLaboratoryUnits"" Type=""Collection(Example.Database.LaboratoryUnit)""/>
				<NavigationProperty Name=""ModifiedLaboratoryResultTypes"" Type=""Collection(Example.Database.LaboratoryResultType)""/>
				<NavigationProperty Name=""CreatedLaboratoryResultTypes"" Type=""Collection(Example.Database.LaboratoryResultType)""/>
				<NavigationProperty Name=""ModifiedLaboratoryProducts"" Type=""Collection(Example.Database.LaboratoryProduct)""/>
				<NavigationProperty Name=""CreatedLaboratoryProducts"" Type=""Collection(Example.Database.LaboratoryProduct)""/>
				<NavigationProperty Name=""ModifiedLaboratoryProductTypes"" Type=""Collection(Example.Database.LaboratoryProductType)""/>
				<NavigationProperty Name=""CreatedLaboratoryProductTypes"" Type=""Collection(Example.Database.LaboratoryProductType)""/>
				<NavigationProperty Name=""ModifiedSuites"" Type=""Collection(Example.Database.Suite)""/>
				<NavigationProperty Name=""CreatedSuites"" Type=""Collection(Example.Database.Suite)""/>
				<NavigationProperty Name=""DeletedSuites"" Type=""Collection(Example.Database.Suite)""/>
				<NavigationProperty Name=""ModifiedSuiteTestTypes"" Type=""Collection(Example.Database.SuiteTestType)""/>
				<NavigationProperty Name=""CreatedSuiteTestTypes"" Type=""Collection(Example.Database.SuiteTestType)""/>
				<NavigationProperty Name=""DeletedSuiteTestTypes"" Type=""Collection(Example.Database.SuiteTestType)""/>
				<NavigationProperty Name=""ModifiedSuiteOrderTaskTypes"" Type=""Collection(Example.Database.SuiteOrderTaskType)""/>
				<NavigationProperty Name=""CreatedSuiteOrderTaskTypes"" Type=""Collection(Example.Database.SuiteOrderTaskType)""/>
				<NavigationProperty Name=""DeletedSuiteOrderTaskTypes"" Type=""Collection(Example.Database.SuiteOrderTaskType)""/>
				<NavigationProperty Name=""ModifiedOrderTaskTypes"" Type=""Collection(Example.Database.OrderTaskType)""/>
				<NavigationProperty Name=""CreatedOrderTaskTypes"" Type=""Collection(Example.Database.OrderTaskType)""/>
				<NavigationProperty Name=""DeletedOrderTaskTypes"" Type=""Collection(Example.Database.OrderTaskType)""/>
				<NavigationProperty Name=""ModifiedSuiteOrderTasks"" Type=""Collection(Example.Database.SuiteOrderTask)""/>
				<NavigationProperty Name=""CreatedSuiteOrderTasks"" Type=""Collection(Example.Database.SuiteOrderTask)""/>
				<NavigationProperty Name=""SuiteOrderTasksByCompletedBy"" Type=""Collection(Example.Database.SuiteOrderTask)""/>
				<NavigationProperty Name=""SuiteOrderTasksByCancelledBy"" Type=""Collection(Example.Database.SuiteOrderTask)""/>
				<NavigationProperty Name=""ModifiedSuiteSamples"" Type=""Collection(Example.Database.SuiteSample)""/>
				<NavigationProperty Name=""CreatedSuiteSamples"" Type=""Collection(Example.Database.SuiteSample)""/>
				<NavigationProperty Name=""SuiteSamplesByCompletedBy"" Type=""Collection(Example.Database.SuiteSample)""/>
				<NavigationProperty Name=""SuiteSamplesByCancelledBy"" Type=""Collection(Example.Database.SuiteSample)""/>
				<NavigationProperty Name=""ModifiedOrderTasks"" Type=""Collection(Example.Database.OrderTask)""/>
				<NavigationProperty Name=""CreatedOrderTasks"" Type=""Collection(Example.Database.OrderTask)""/>
				<NavigationProperty Name=""OrderTasksByCompletedBy"" Type=""Collection(Example.Database.OrderTask)""/>
				<NavigationProperty Name=""ModifiedOrderTaskResults"" Type=""Collection(Example.Database.OrderTaskResult)""/>
				<NavigationProperty Name=""CreatedOrderTaskResults"" Type=""Collection(Example.Database.OrderTaskResult)""/>
				<NavigationProperty Name=""DeletedOrderTaskResults"" Type=""Collection(Example.Database.OrderTaskResult)""/>
				<NavigationProperty Name=""ModifiedOrderTaskResultDataPoints"" Type=""Collection(Example.Database.OrderTaskResultDataPoint)""/>
				<NavigationProperty Name=""CreatedOrderTaskResultDataPoints"" Type=""Collection(Example.Database.OrderTaskResultDataPoint)""/>
				<NavigationProperty Name=""DeletedOrderTaskResultDataPoints"" Type=""Collection(Example.Database.OrderTaskResultDataPoint)""/>
				<NavigationProperty Name=""ModifiedSuiteTests"" Type=""Collection(Example.Database.SuiteTest)""/>
				<NavigationProperty Name=""CreatedSuiteTests"" Type=""Collection(Example.Database.SuiteTest)""/>
				<NavigationProperty Name=""ModifiedControlSamples"" Type=""Collection(Example.Database.ControlSample)""/>
				<NavigationProperty Name=""CreatedControlSamples"" Type=""Collection(Example.Database.ControlSample)""/>
				<NavigationProperty Name=""ModifiedArticleTypes"" Type=""Collection(Example.Database.ArticleType)""/>
				<NavigationProperty Name=""CreatedArticleTypes"" Type=""Collection(Example.Database.ArticleType)""/>
				<NavigationProperty Name=""ModifiedArticleTypeFields"" Type=""Collection(Example.Database.ArticleTypeField)""/>
				<NavigationProperty Name=""CreatedArticleTypeFields"" Type=""Collection(Example.Database.ArticleTypeField)""/>
				<NavigationProperty Name=""CreatedComplaints"" Type=""Collection(Example.Database.Complaint)""/>
				<NavigationProperty Name=""ModifiedComplaints"" Type=""Collection(Example.Database.Complaint)""/>
				<NavigationProperty Name=""DeletedComplaints"" Type=""Collection(Example.Database.Complaint)""/>
				<NavigationProperty Name=""CreatedCorrectiveActions"" Type=""Collection(Example.Database.CorrectiveAction)""/>
				<NavigationProperty Name=""ModifiedCorrectiveActions"" Type=""Collection(Example.Database.CorrectiveAction)""/>
				<NavigationProperty Name=""DeletedCorrectiveActions"" Type=""Collection(Example.Database.CorrectiveAction)""/>
				<NavigationProperty Name=""CreatedComplaintCorrectiveActions"" Type=""Collection(Example.Database.ComplaintCorrectiveAction)""/>
				<NavigationProperty Name=""ModifiedComplaintCorrectiveActions"" Type=""Collection(Example.Database.ComplaintCorrectiveAction)""/>
				<NavigationProperty Name=""DeletedComplaintCorrectiveActions"" Type=""Collection(Example.Database.ComplaintCorrectiveAction)""/>
				<NavigationProperty Name=""ModifiedProjectTaskTypes"" Type=""Collection(Example.Database.ProjectTaskType)""/>
				<NavigationProperty Name=""CreatedProjectTaskTypes"" Type=""Collection(Example.Database.ProjectTaskType)""/>
				<NavigationProperty Name=""DeletedProjectTaskTypes"" Type=""Collection(Example.Database.ProjectTaskType)""/>
				<NavigationProperty Name=""BKDProjectTasksByAssingedUser"" Type=""Collection(Example.Database.ProjectTask)""/>
				<NavigationProperty Name=""ModifiedProjectTasks"" Type=""Collection(Example.Database.ProjectTask)""/>
				<NavigationProperty Name=""CreatedProjectTasks"" Type=""Collection(Example.Database.ProjectTask)""/>
				<NavigationProperty Name=""ProjectTasksByCompletedBy"" Type=""Collection(Example.Database.ProjectTask)""/>
				<NavigationProperty Name=""BKDModifiedCrops"" Type=""Collection(Example.Database.BKDCrop)""/>
				<NavigationProperty Name=""BKDCreatedCrops"" Type=""Collection(Example.Database.BKDCrop)""/>
				<NavigationProperty Name=""BKDModifiedCultivars"" Type=""Collection(Example.Database.BKDCultivar)""/>
				<NavigationProperty Name=""BKDCreatedCultivars"" Type=""Collection(Example.Database.BKDCultivar)""/>
				<NavigationProperty Name=""BKDModifiedPlantMaterials"" Type=""Collection(Example.Database.BKDPlantMaterial)""/>
				<NavigationProperty Name=""BKDCreatedPlantMaterials"" Type=""Collection(Example.Database.BKDPlantMaterial)""/>
				<NavigationProperty Name=""BKDModifiedTestAmounts"" Type=""Collection(Example.Database.BKDTestAmount)""/>
				<NavigationProperty Name=""BKDCreatedTestAmounts"" Type=""Collection(Example.Database.BKDTestAmount)""/>
				<NavigationProperty Name=""BKDModifiedSampleRemarks"" Type=""Collection(Example.Database.BKDSampleRemark)""/>
				<NavigationProperty Name=""BKDCreatedSampleRemarks"" Type=""Collection(Example.Database.BKDSampleRemark)""/>
				<NavigationProperty Name=""BKDModifiedProjectTaskComments"" Type=""Collection(Example.Database.BKDProjectTaskComment)""/>
				<NavigationProperty Name=""BKDCreatedProjectTaskComments"" Type=""Collection(Example.Database.BKDProjectTaskComment)""/>
				<NavigationProperty Name=""BKDModifiedProjectRules"" Type=""Collection(Example.Database.BKDProjectRule)""/>
				<NavigationProperty Name=""BKDCreatedProjectRules"" Type=""Collection(Example.Database.BKDProjectRule)""/>
				<NavigationProperty Name=""BKDIncubationBatchesByMadeBy"" Type=""Collection(Example.Database.BKDIncubationBatch)""/>
				<NavigationProperty Name=""BKDModifiedIncubationBatches"" Type=""Collection(Example.Database.BKDIncubationBatch)""/>
				<NavigationProperty Name=""BKDCreatedIncubationBatches"" Type=""Collection(Example.Database.BKDIncubationBatch)""/>
				<NavigationProperty Name=""BKDIncubationBatchUsers"" Type=""Collection(Example.Database.BKDIncubationBatchUser)""/>
				<NavigationProperty Name=""BKDModifiedIncubationBatchUsers"" Type=""Collection(Example.Database.BKDIncubationBatchUser)""/>
				<NavigationProperty Name=""BKDCreatedIncubationBatchUsers"" Type=""Collection(Example.Database.BKDIncubationBatchUser)""/>
				<NavigationProperty Name=""BKDModifiedIncubationFormulations"" Type=""Collection(Example.Database.BKDIncubationFormulation)""/>
				<NavigationProperty Name=""BKDCreatedIncubationFormulations"" Type=""Collection(Example.Database.BKDIncubationFormulation)""/>
				<NavigationProperty Name=""BKDModifiedCropMaterialIncubation"" Type=""Collection(Example.Database.BKDCropMaterialIncubation)""/>
				<NavigationProperty Name=""BKDCreatedCropMaterialIncubation"" Type=""Collection(Example.Database.BKDCropMaterialIncubation)""/>
			</EntityType>
			<EntityType Name=""MembershipProvider"">
				<Key>
					<PropertyRef Name=""MembershipProviderId""/>
				</Key>
				<Property Name=""MembershipProviderId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Enabled"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""ProviderName"" Type=""Edm.String""/>
				<Property Name=""Data"" Type=""Edm.String""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Users"" Type=""Collection(Example.Database.User)""/>
				<NavigationProperty Name=""AccountProvider"" Type=""Collection(Example.Database.AccountProvider)""/>
				<NavigationProperty Name=""Sessions"" Type=""Collection(Example.Database.Session)""/>
			</EntityType>
			<EntityType Name=""Location"">
				<Key>
					<PropertyRef Name=""LocationId""/>
				</Key>
				<Property Name=""LocationId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<Property Name=""Type"" Type=""Edm.String""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ParentLocation"" Type=""Example.Database.Location"" Nullable=""false""/>
				<NavigationProperty Name=""LocationType"" Type=""Example.Database.LocationType"" Nullable=""false""/>
				<NavigationProperty Name=""Printer"" Type=""Example.Database.Printer"" Nullable=""false""/>
				<NavigationProperty Name=""DefaultSchedule"" Type=""Example.Database.Schedule"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Users"" Type=""Collection(Example.Database.User)""/>
				<NavigationProperty Name=""EntityFormTemplates"" Type=""Collection(Example.Database.EntityFormTemplate)""/>
				<NavigationProperty Name=""LocationsByParentLocation"" Type=""Collection(Example.Database.Location)""/>
				<NavigationProperty Name=""Groups"" Type=""Collection(Example.Database.Group)""/>
				<NavigationProperty Name=""LocationItems"" Type=""Collection(Example.Database.LocationItem)""/>
				<NavigationProperty Name=""Projects"" Type=""Collection(Example.Database.Project)""/>
				<NavigationProperty Name=""Experiments"" Type=""Collection(Example.Database.Experiment)""/>
				<NavigationProperty Name=""ScheduleTypes"" Type=""Collection(Example.Database.ScheduleType)""/>
				<NavigationProperty Name=""ScheduleItems"" Type=""Collection(Example.Database.ScheduleItem)""/>
				<NavigationProperty Name=""Racks"" Type=""Collection(Example.Database.Rack)""/>
				<NavigationProperty Name=""ProcessStations"" Type=""Collection(Example.Database.ProcessStation)""/>
				<NavigationProperty Name=""Protocols"" Type=""Collection(Example.Database.Protocol)""/>
				<NavigationProperty Name=""InstrumentsByInstrumentLocation"" Type=""Collection(Example.Database.Instrument)""/>
				<NavigationProperty Name=""Materials"" Type=""Collection(Example.Database.Material)""/>
				<NavigationProperty Name=""Batches"" Type=""Collection(Example.Database.Batch)""/>
				<NavigationProperty Name=""ProductUnits"" Type=""Collection(Example.Database.ProductUnit)""/>
				<NavigationProperty Name=""Sequences"" Type=""Collection(Example.Database.Sequence)""/>
				<NavigationProperty Name=""Products"" Type=""Collection(Example.Database.Product)""/>
				<NavigationProperty Name=""QuotationLocations"" Type=""Collection(Example.Database.QuotationLocation)""/>
			</EntityType>
			<EntityType Name=""Group"">
				<Key>
					<PropertyRef Name=""GroupId""/>
				</Key>
				<Property Name=""GroupId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Location"" Type=""Example.Database.Location"" Nullable=""false""/>
				<NavigationProperty Name=""GroupLeader"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""UsersByPreferredGroup"" Type=""Collection(Example.Database.User)""/>
				<NavigationProperty Name=""FileTemplates"" Type=""Collection(Example.Database.FileTemplate)""/>
				<NavigationProperty Name=""FormTemplates"" Type=""Collection(Example.Database.FormTemplate)""/>
				<NavigationProperty Name=""ElectronicSignatureTypes"" Type=""Collection(Example.Database.ElectronicSignatureType)""/>
				<NavigationProperty Name=""ReportTypes"" Type=""Collection(Example.Database.ReportType)""/>
				<NavigationProperty Name=""Documents"" Type=""Collection(Example.Database.Document)""/>
				<NavigationProperty Name=""UserGroups"" Type=""Collection(Example.Database.UserGroup)""/>
				<NavigationProperty Name=""Installations"" Type=""Collection(Example.Database.Installation)""/>
				<NavigationProperty Name=""InstallationLogs"" Type=""Collection(Example.Database.InstallationLog)""/>
				<NavigationProperty Name=""ConfigurationSystems"" Type=""Collection(Example.Database.ConfigurationSystem)""/>
				<NavigationProperty Name=""ConfigurationInstances"" Type=""Collection(Example.Database.ConfigurationInstance)""/>
				<NavigationProperty Name=""Credentials"" Type=""Collection(Example.Database.Credential)""/>
				<NavigationProperty Name=""Clients"" Type=""Collection(Example.Database.Client)""/>
				<NavigationProperty Name=""Projects"" Type=""Collection(Example.Database.Project)""/>
				<NavigationProperty Name=""ExperimentTypes"" Type=""Collection(Example.Database.ExperimentType)""/>
				<NavigationProperty Name=""ProjectTypes"" Type=""Collection(Example.Database.ProjectType)""/>
				<NavigationProperty Name=""Experiments"" Type=""Collection(Example.Database.Experiment)""/>
				<NavigationProperty Name=""Tests"" Type=""Collection(Example.Database.Test)""/>
				<NavigationProperty Name=""ResultTypes"" Type=""Collection(Example.Database.ResultType)""/>
				<NavigationProperty Name=""Results"" Type=""Collection(Example.Database.Result)""/>
				<NavigationProperty Name=""SampleTypes"" Type=""Collection(Example.Database.SampleType)""/>
				<NavigationProperty Name=""Samples"" Type=""Collection(Example.Database.Sample)""/>
				<NavigationProperty Name=""TestTypes"" Type=""Collection(Example.Database.TestType)""/>
				<NavigationProperty Name=""OrderTypes"" Type=""Collection(Example.Database.OrderType)""/>
				<NavigationProperty Name=""Orders"" Type=""Collection(Example.Database.Order)""/>
				<NavigationProperty Name=""SequenceTypes"" Type=""Collection(Example.Database.SequenceType)""/>
				<NavigationProperty Name=""Sequences"" Type=""Collection(Example.Database.Sequence)""/>
				<NavigationProperty Name=""OrderForms"" Type=""Collection(Example.Database.OrderForm)""/>
				<NavigationProperty Name=""ScheduleRules"" Type=""Collection(Example.Database.ScheduleRule)""/>
				<NavigationProperty Name=""ScheduleRuleSets"" Type=""Collection(Example.Database.ScheduleRuleSet)""/>
				<NavigationProperty Name=""Suites"" Type=""Collection(Example.Database.Suite)""/>
				<NavigationProperty Name=""SuiteSamples"" Type=""Collection(Example.Database.SuiteSample)""/>
			</EntityType>
			<EntityType Name=""AccountProvider"">
				<Key>
					<PropertyRef Name=""AccountProviderId""/>
				</Key>
				<Property Name=""AccountProviderId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""ProviderName"" Type=""Edm.String""/>
				<Property Name=""Enabled"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""AutoCreateUser"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""Parameter"" Type=""Edm.String""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""MembershipProvider"" Type=""Example.Database.MembershipProvider"" Nullable=""false""/>
				<NavigationProperty Name=""Application"" Type=""Example.Database.Application"" Nullable=""false""/>
				<NavigationProperty Name=""Accounts"" Type=""Collection(Example.Database.Account)""/>
			</EntityType>
			<EntityType Name=""Account"">
				<Key>
					<PropertyRef Name=""AccountId""/>
				</Key>
				<Property Name=""AccountId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Status"" Type=""Edm.String""/>
				<Property Name=""LastLogin"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""PreferredUsername"" Type=""Edm.String""/>
				<Property Name=""Email"" Type=""Edm.String""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""AccountProvider"" Type=""Example.Database.AccountProvider"" Nullable=""false""/>
				<NavigationProperty Name=""User"" Type=""Example.Database.User"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""Table"">
				<Key>
					<PropertyRef Name=""TableId""/>
				</Key>
				<Property Name=""TableId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Pages"" Type=""Edm.String""/>
				<Property Name=""Key"" Type=""Edm.String""/>
				<Property Name=""PluralName"" Type=""Edm.String""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""NameField"" Type=""Example.Database.Field"" Nullable=""false""/>
				<NavigationProperty Name=""Fields"" Type=""Collection(Example.Database.Field)""/>
				<NavigationProperty Name=""FieldsByReferenceTable"" Type=""Collection(Example.Database.Field)""/>
				<NavigationProperty Name=""Entities"" Type=""Collection(Example.Database.Entity)""/>
			</EntityType>
			<EntityType Name=""Field"">
				<Key>
					<PropertyRef Name=""FieldId""/>
				</Key>
				<Property Name=""FieldId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Readonly"" Type=""Edm.Boolean""/>
				<Property Name=""DataType"" Type=""Edm.String""/>
				<Property Name=""IsUnique"" Type=""Edm.Boolean""/>
				<Property Name=""DefaultValue"" Type=""Edm.String""/>
				<Property Name=""Visible"" Type=""Edm.Boolean""/>
				<Property Name=""Required"" Type=""Edm.Boolean""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Table"" Type=""Example.Database.Table"" Nullable=""false""/>
				<NavigationProperty Name=""ReferenceTable"" Type=""Example.Database.Table"" Nullable=""false""/>
				<NavigationProperty Name=""ReferenceField"" Type=""Example.Database.Field"" Nullable=""false""/>
				<NavigationProperty Name=""Phrase"" Type=""Example.Database.Phrase"" Nullable=""false""/>
				<NavigationProperty Name=""TablesByNameField"" Type=""Collection(Example.Database.Table)""/>
				<NavigationProperty Name=""FieldsByReferenceField"" Type=""Collection(Example.Database.Field)""/>
				<NavigationProperty Name=""EntityFields"" Type=""Collection(Example.Database.EntityField)""/>
			</EntityType>
			<EntityType Name=""Text"">
				<Key>
					<PropertyRef Name=""TextId""/>
				</Key>
				<Property Name=""TextId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""nl"" Type=""Edm.String""/>
				<Property Name=""en"" Type=""Edm.String""/>
				<Property Name=""de"" Type=""Edm.String""/>
				<Property Name=""fr"" Type=""Edm.String""/>
				<Property Name=""es"" Type=""Edm.String""/>
				<Property Name=""it"" Type=""Edm.String""/>
				<Property Name=""Label"" Type=""Edm.String""/>
				<Property Name=""Type"" Type=""Edm.String""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""EntitiesByInstructionText"" Type=""Collection(Example.Database.Entity)""/>
				<NavigationProperty Name=""EntityFieldsByInstructionText"" Type=""Collection(Example.Database.EntityField)""/>
				<NavigationProperty Name=""EntityPagesByInstructionText"" Type=""Collection(Example.Database.EntityPage)""/>
				<NavigationProperty Name=""FormTemplatesByInstructionText"" Type=""Collection(Example.Database.FormTemplate)""/>
				<NavigationProperty Name=""FormTemplateFieldsByLabelText"" Type=""Collection(Example.Database.FormTemplateField)""/>
				<NavigationProperty Name=""FormTemplateFieldsByInstructionText"" Type=""Collection(Example.Database.FormTemplateField)""/>
				<NavigationProperty Name=""FormTemplateSectionsByLabelText"" Type=""Collection(Example.Database.FormTemplateSection)""/>
				<NavigationProperty Name=""ElectronicSignatureTypesByInstructionText"" Type=""Collection(Example.Database.ElectronicSignatureType)""/>
				<NavigationProperty Name=""Countries"" Type=""Collection(Example.Database.Country)""/>
				<NavigationProperty Name=""UserTrainingsByCommentsText"" Type=""Collection(Example.Database.UserTraining)""/>
				<NavigationProperty Name=""Units"" Type=""Collection(Example.Database.Unit)""/>
				<NavigationProperty Name=""ResultTypes"" Type=""Collection(Example.Database.ResultType)""/>
				<NavigationProperty Name=""ResultTypesByInstructionText"" Type=""Collection(Example.Database.ResultType)""/>
				<NavigationProperty Name=""SampleTypes"" Type=""Collection(Example.Database.SampleType)""/>
				<NavigationProperty Name=""TestTypes"" Type=""Collection(Example.Database.TestType)""/>
				<NavigationProperty Name=""TestTypesByInstructionText"" Type=""Collection(Example.Database.TestType)""/>
				<NavigationProperty Name=""OrderTypes"" Type=""Collection(Example.Database.OrderType)""/>
				<NavigationProperty Name=""Products"" Type=""Collection(Example.Database.Product)""/>
				<NavigationProperty Name=""Methods"" Type=""Collection(Example.Database.Method)""/>
				<NavigationProperty Name=""OrderTaskTypesByTitle"" Type=""Collection(Example.Database.OrderTaskType)""/>
				<NavigationProperty Name=""OrderTaskResultsByTextValue"" Type=""Collection(Example.Database.OrderTaskResult)""/>
			</EntityType>
			<EntityType Name=""Entity"">
				<Key>
					<PropertyRef Name=""EntityId""/>
				</Key>
				<Property Name=""EntityId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<Property Name=""Key"" Type=""Edm.String""/>
				<Property Name=""PluralName"" Type=""Edm.String""/>
				<Property Name=""Icon"" Type=""Edm.String""/>
				<Property Name=""ReadonlyExpression"" Type=""Edm.String""/>
				<Property Name=""ChildEntity"" Type=""Edm.Boolean""/>
				<Property Name=""AuditType"" Type=""Edm.String""/>
				<Property Name=""DefaultAccess"" Type=""Edm.String""/>
				<Property Name=""Versionable"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""OverrideUserSetting"" Type=""Edm.Boolean""/>
				<Property Name=""Instruction"" Type=""Edm.String""/>
				<Property Name=""IndexType"" Type=""Edm.String""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Table"" Type=""Example.Database.Table"" Nullable=""false""/>
				<NavigationProperty Name=""NameEntityField"" Type=""Example.Database.EntityField"" Nullable=""false""/>
				<NavigationProperty Name=""StatusEntityField"" Type=""Example.Database.EntityField"" Nullable=""false""/>
				<NavigationProperty Name=""FormTemplate"" Type=""Example.Database.FormTemplate"" Nullable=""false""/>
				<NavigationProperty Name=""InstructionText"" Type=""Example.Database.Text"" Nullable=""false""/>
				<NavigationProperty Name=""EntityUsers"" Type=""Collection(Example.Database.EntityUser)""/>
				<NavigationProperty Name=""EntityTypeEvents"" Type=""Collection(Example.Database.EntityTypeEvent)""/>
				<NavigationProperty Name=""EntityEvents"" Type=""Collection(Example.Database.EntityEvent)""/>
				<NavigationProperty Name=""ItemEvents"" Type=""Collection(Example.Database.ItemEvent)""/>
				<NavigationProperty Name=""EntityFields"" Type=""Collection(Example.Database.EntityField)""/>
				<NavigationProperty Name=""EntityFieldsByReferenceEntity"" Type=""Collection(Example.Database.EntityField)""/>
				<NavigationProperty Name=""EntityFieldHistories"" Type=""Collection(Example.Database.EntityFieldHistory)""/>
				<NavigationProperty Name=""EntityFieldHistoriesByLinkedEntity"" Type=""Collection(Example.Database.EntityFieldHistory)""/>
				<NavigationProperty Name=""EntityPages"" Type=""Collection(Example.Database.EntityPage)""/>
				<NavigationProperty Name=""Notes"" Type=""Collection(Example.Database.Note)""/>
				<NavigationProperty Name=""FileTemplates"" Type=""Collection(Example.Database.FileTemplate)""/>
				<NavigationProperty Name=""FileGroups"" Type=""Collection(Example.Database.FileGroup)""/>
				<NavigationProperty Name=""Files"" Type=""Collection(Example.Database.File)""/>
				<NavigationProperty Name=""Forms"" Type=""Collection(Example.Database.Form)""/>
				<NavigationProperty Name=""EntityForms"" Type=""Collection(Example.Database.EntityForm)""/>
				<NavigationProperty Name=""FormTemplates"" Type=""Collection(Example.Database.FormTemplate)""/>
				<NavigationProperty Name=""FormTemplateFields"" Type=""Collection(Example.Database.FormTemplateField)""/>
				<NavigationProperty Name=""EntityFormTemplates"" Type=""Collection(Example.Database.EntityFormTemplate)""/>
				<NavigationProperty Name=""Folders"" Type=""Collection(Example.Database.Folder)""/>
				<NavigationProperty Name=""FolderEntities"" Type=""Collection(Example.Database.FolderEntity)""/>
				<NavigationProperty Name=""FolderEntitiesByChildEntity"" Type=""Collection(Example.Database.FolderEntity)""/>
				<NavigationProperty Name=""Audits"" Type=""Collection(Example.Database.Audit)""/>
				<NavigationProperty Name=""AuditItems"" Type=""Collection(Example.Database.AuditItem)""/>
				<NavigationProperty Name=""ElectronicSignatureTypes"" Type=""Collection(Example.Database.ElectronicSignatureType)""/>
				<NavigationProperty Name=""ElectronicSignatures"" Type=""Collection(Example.Database.ElectronicSignature)""/>
				<NavigationProperty Name=""ReportTypes"" Type=""Collection(Example.Database.ReportType)""/>
				<NavigationProperty Name=""ReportTypeArguments"" Type=""Collection(Example.Database.ReportTypeArgument)""/>
				<NavigationProperty Name=""EntityActions"" Type=""Collection(Example.Database.EntityAction)""/>
				<NavigationProperty Name=""UserSettings"" Type=""Collection(Example.Database.UserSetting)""/>
				<NavigationProperty Name=""Queries"" Type=""Collection(Example.Database.Query)""/>
				<NavigationProperty Name=""TimestampValues"" Type=""Collection(Example.Database.TimestampValue)""/>
				<NavigationProperty Name=""Documents"" Type=""Collection(Example.Database.Document)""/>
				<NavigationProperty Name=""ChangeRequests"" Type=""Collection(Example.Database.ChangeRequest)""/>
				<NavigationProperty Name=""ChangeRequestTypes"" Type=""Collection(Example.Database.ChangeRequestType)""/>
				<NavigationProperty Name=""Counters"" Type=""Collection(Example.Database.Counter)""/>
				<NavigationProperty Name=""Properties"" Type=""Collection(Example.Database.Property)""/>
				<NavigationProperty Name=""PropertiesByLinkedEntity"" Type=""Collection(Example.Database.Property)""/>
				<NavigationProperty Name=""EntityHierarchysByParentEntity"" Type=""Collection(Example.Database.EntityHierarchy)""/>
				<NavigationProperty Name=""SystemLogs"" Type=""Collection(Example.Database.SystemLog)""/>
				<NavigationProperty Name=""CheckListTypes"" Type=""Collection(Example.Database.CheckListType)""/>
				<NavigationProperty Name=""CheckLists"" Type=""Collection(Example.Database.CheckList)""/>
				<NavigationProperty Name=""LocationItems"" Type=""Collection(Example.Database.LocationItem)""/>
				<NavigationProperty Name=""LocationItemTypes"" Type=""Collection(Example.Database.LocationItemType)""/>
				<NavigationProperty Name=""RecordLocks"" Type=""Collection(Example.Database.RecordLock)""/>
				<NavigationProperty Name=""SlotTypes"" Type=""Collection(Example.Database.SlotType)""/>
				<NavigationProperty Name=""SlotItems"" Type=""Collection(Example.Database.SlotItem)""/>
				<NavigationProperty Name=""EntitySpecifications"" Type=""Collection(Example.Database.EntitySpecification)""/>
				<NavigationProperty Name=""InvoiceItems"" Type=""Collection(Example.Database.InvoiceItem)""/>
				<NavigationProperty Name=""Articles"" Type=""Collection(Example.Database.Article)""/>
				<NavigationProperty Name=""OrderTaskResultDataPoints"" Type=""Collection(Example.Database.OrderTaskResultDataPoint)""/>
				<NavigationProperty Name=""ArticleTypes"" Type=""Collection(Example.Database.ArticleType)""/>
			</EntityType>
			<EntityType Name=""EntityUser"">
				<Key>
					<PropertyRef Name=""EntityUserId""/>
				</Key>
				<Property Name=""EntityUserId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""AccessLevel"" Type=""Edm.String""/>
				<Property Name=""ItemId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""EntityName"" Type=""Edm.String""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""User"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Entity"" Type=""Example.Database.Entity"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""EntityTypeEvent"">
				<Key>
					<PropertyRef Name=""EntityTypeEventId""/>
				</Key>
				<Property Name=""EntityTypeEventId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""OnCreate"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""OnUpdate"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""OnDelete"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""OrderNumber"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Final"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""Disabled"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""Condition"" Type=""Edm.String""/>
				<Property Name=""Data"" Type=""Edm.String""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ItemId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""EntityName"" Type=""Edm.String""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Entity"" Type=""Example.Database.Entity"" Nullable=""false""/>
				<NavigationProperty Name=""ItemEvents"" Type=""Collection(Example.Database.ItemEvent)""/>
			</EntityType>
			<EntityType Name=""EntityEvent"">
				<Key>
					<PropertyRef Name=""EntityEventId""/>
				</Key>
				<Property Name=""EntityEventId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""OnCreate"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""OnUpdate"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""OnDelete"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""OrderNumber"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Final"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""Disabled"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Type"" Type=""Edm.String""/>
				<Property Name=""Data"" Type=""Edm.String""/>
				<Property Name=""Condition"" Type=""Edm.String""/>
				<NavigationProperty Name=""Entity"" Type=""Example.Database.Entity"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""EntityField"" Type=""Example.Database.EntityField"" Nullable=""false""/>
				<NavigationProperty Name=""ItemEvents"" Type=""Collection(Example.Database.ItemEvent)""/>
			</EntityType>
			<EntityType Name=""ItemEvent"">
				<Key>
					<PropertyRef Name=""ItemEventId""/>
				</Key>
				<Property Name=""ItemEventId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Error"" Type=""Edm.String""/>
				<Property Name=""Type"" Type=""Edm.String""/>
				<Property Name=""Parameter"" Type=""Edm.String""/>
				<Property Name=""ItemId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Processed"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""ProcessedOn"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""Status"" Type=""Edm.String""/>
				<Property Name=""AssignedOn"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""EntityName"" Type=""Edm.String""/>
				<Property Name=""Cancelled"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""CancelledOn"" Type=""Edm.DateTimeOffset""/>
				<NavigationProperty Name=""EntityEvent"" Type=""Example.Database.EntityEvent"" Nullable=""false""/>
				<NavigationProperty Name=""EntityTypeEvent"" Type=""Example.Database.EntityTypeEvent"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Entity"" Type=""Example.Database.Entity"" Nullable=""false""/>
				<NavigationProperty Name=""Device"" Type=""Example.Database.Device"" Nullable=""false""/>
				<NavigationProperty Name=""CancelledBy"" Type=""Example.Database.User"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""Phrase"">
				<Key>
					<PropertyRef Name=""PhraseId""/>
				</Key>
				<Property Name=""PhraseId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<Property Name=""SystemPhrase"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""ShowValue"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""TimeStamp"" Type=""Edm.Binary""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Fields"" Type=""Collection(Example.Database.Field)""/>
				<NavigationProperty Name=""PhraseItems"" Type=""Collection(Example.Database.PhraseItem)""/>
				<NavigationProperty Name=""EntityFields"" Type=""Collection(Example.Database.EntityField)""/>
				<NavigationProperty Name=""FormTemplateFields"" Type=""Collection(Example.Database.FormTemplateField)""/>
				<NavigationProperty Name=""ReportTypeArguments"" Type=""Collection(Example.Database.ReportTypeArgument)""/>
				<NavigationProperty Name=""ResultTypes"" Type=""Collection(Example.Database.ResultType)""/>
				<NavigationProperty Name=""ResultTypesByDeviation"" Type=""Collection(Example.Database.ResultType)""/>
			</EntityType>
			<EntityType Name=""PhraseItem"">
				<Key>
					<PropertyRef Name=""PhraseItemId""/>
				</Key>
				<Property Name=""PhraseItemId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Value"" Type=""Edm.String""/>
				<Property Name=""nl"" Type=""Edm.String""/>
				<Property Name=""en"" Type=""Edm.String""/>
				<Property Name=""de"" Type=""Edm.String""/>
				<Property Name=""fr"" Type=""Edm.String""/>
				<Property Name=""es"" Type=""Edm.String""/>
				<Property Name=""it"" Type=""Edm.String""/>
				<Property Name=""DisplayValue"" Type=""Edm.String""/>
				<Property Name=""Color"" Type=""Edm.String""/>
				<Property Name=""Icon"" Type=""Edm.String""/>
				<Property Name=""OrderNumber"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Disabled"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""NumberValue"" Type=""Edm.Double""/>
				<Property Name=""Level"" Type=""Edm.Int64"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Phrase"" Type=""Example.Database.Phrase"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""EntityField"">
				<Key>
					<PropertyRef Name=""EntityFieldId""/>
				</Key>
				<Property Name=""EntityFieldId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Readonly"" Type=""Edm.Boolean""/>
				<Property Name=""DataType"" Type=""Edm.String""/>
				<Property Name=""OrderNumber"" Type=""Edm.Int64""/>
				<Property Name=""IsUnique"" Type=""Edm.Boolean""/>
				<Property Name=""DefaultValue"" Type=""Edm.String""/>
				<Property Name=""Visible"" Type=""Edm.Boolean""/>
				<Property Name=""Required"" Type=""Edm.Boolean""/>
				<Property Name=""MinimumLength"" Type=""Edm.Int64""/>
				<Property Name=""MaximumLength"" Type=""Edm.Int64""/>
				<Property Name=""ValidationRule"" Type=""Edm.String""/>
				<Property Name=""PickerType"" Type=""Edm.String""/>
				<Property Name=""AuditType"" Type=""Edm.String""/>
				<Property Name=""AllowOnlyPhraseValues"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DefaultWidth"" Type=""Edm.String""/>
				<Property Name=""Instruction"" Type=""Edm.String""/>
				<Property Name=""Calculated"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""Expression"" Type=""Edm.String""/>
				<Property Name=""MultipleValues"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""ReadonlyExpression"" Type=""Edm.String""/>
				<Property Name=""VisibleExpression"" Type=""Edm.String""/>
				<Property Name=""Format"" Type=""Edm.String""/>
				<Property Name=""MinimumValue"" Type=""Edm.Double""/>
				<Property Name=""MaximumValue"" Type=""Edm.Double""/>
				<Property Name=""DefaultValueExpression"" Type=""Edm.String""/>
				<Property Name=""TrackHistory"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""AllowCopyUpAndDown"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""IndexType"" Type=""Edm.String""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Entity"" Type=""Example.Database.Entity"" Nullable=""false""/>
				<NavigationProperty Name=""Field"" Type=""Example.Database.Field"" Nullable=""false""/>
				<NavigationProperty Name=""Phrase"" Type=""Example.Database.Phrase"" Nullable=""false""/>
				<NavigationProperty Name=""ReferenceEntityField"" Type=""Example.Database.EntityField"" Nullable=""false""/>
				<NavigationProperty Name=""ReferenceEntity"" Type=""Example.Database.Entity"" Nullable=""false""/>
				<NavigationProperty Name=""FilterQuery"" Type=""Example.Database.Query"" Nullable=""false""/>
				<NavigationProperty Name=""DependsOn"" Type=""Example.Database.EntityField"" Nullable=""false""/>
				<NavigationProperty Name=""Operation"" Type=""Example.Database.Operation"" Nullable=""false""/>
				<NavigationProperty Name=""InstructionText"" Type=""Example.Database.Text"" Nullable=""false""/>
				<NavigationProperty Name=""EntitiesByNameEntityField"" Type=""Collection(Example.Database.Entity)""/>
				<NavigationProperty Name=""EntitiesByStatusEntityField"" Type=""Collection(Example.Database.Entity)""/>
				<NavigationProperty Name=""EntityEvents"" Type=""Collection(Example.Database.EntityEvent)""/>
				<NavigationProperty Name=""EntityFieldsByReferenceEntityField"" Type=""Collection(Example.Database.EntityField)""/>
				<NavigationProperty Name=""EntityFieldsByDependsOn"" Type=""Collection(Example.Database.EntityField)""/>
				<NavigationProperty Name=""EntityFieldHistories"" Type=""Collection(Example.Database.EntityFieldHistory)""/>
				<NavigationProperty Name=""EntityPagesByGridEntityField"" Type=""Collection(Example.Database.EntityPage)""/>
				<NavigationProperty Name=""EntityPagesByEditRecordField"" Type=""Collection(Example.Database.EntityPage)""/>
				<NavigationProperty Name=""FolderFilters"" Type=""Collection(Example.Database.FolderFilter)""/>
				<NavigationProperty Name=""FolderEntities"" Type=""Collection(Example.Database.FolderEntity)""/>
				<NavigationProperty Name=""EntityHierarchys"" Type=""Collection(Example.Database.EntityHierarchy)""/>
				<NavigationProperty Name=""EntityFieldRules"" Type=""Collection(Example.Database.EntityFieldRule)""/>
				<NavigationProperty Name=""OrderTypeFields"" Type=""Collection(Example.Database.OrderTypeField)""/>
				<NavigationProperty Name=""SampleTypeFields"" Type=""Collection(Example.Database.SampleTypeField)""/>
				<NavigationProperty Name=""TestTypeFields"" Type=""Collection(Example.Database.TestTypeField)""/>
				<NavigationProperty Name=""ResultTypeFields"" Type=""Collection(Example.Database.ResultTypeField)""/>
				<NavigationProperty Name=""OrderFormFields"" Type=""Collection(Example.Database.OrderFormField)""/>
				<NavigationProperty Name=""OrderFormSampleFields"" Type=""Collection(Example.Database.OrderFormSampleField)""/>
				<NavigationProperty Name=""ContainerTypeFields"" Type=""Collection(Example.Database.ContainerTypeField)""/>
				<NavigationProperty Name=""ArticleFields"" Type=""Collection(Example.Database.ArticleField)""/>
				<NavigationProperty Name=""ArticleTypeFields"" Type=""Collection(Example.Database.ArticleTypeField)""/>
			</EntityType>
			<EntityType Name=""EntityFieldHistory"">
				<Key>
					<PropertyRef Name=""EntityFieldHistoryId""/>
				</Key>
				<Property Name=""EntityFieldHistoryId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Status"" Type=""Edm.String""/>
				<Property Name=""DataType"" Type=""Edm.String""/>
				<Property Name=""TextValue"" Type=""Edm.String""/>
				<Property Name=""DateValue"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""BooleanValue"" Type=""Edm.Boolean""/>
				<Property Name=""NumberValue"" Type=""Edm.Double""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ItemId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""EntityName"" Type=""Edm.String""/>
				<Property Name=""LinkedItemId"" Type=""Edm.Int64""/>
				<NavigationProperty Name=""EntityField"" Type=""Example.Database.EntityField"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Entity"" Type=""Example.Database.Entity"" Nullable=""false""/>
				<NavigationProperty Name=""LinkedEntity"" Type=""Example.Database.Entity"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""EntityPage"">
				<Key>
					<PropertyRef Name=""EntityPageId""/>
				</Key>
				<Property Name=""EntityPageId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Condition"" Type=""Edm.String""/>
				<Property Name=""OrderNumber"" Type=""Edm.Int64""/>
				<Property Name=""View"" Type=""Edm.String""/>
				<Property Name=""ViewModel"" Type=""Edm.String""/>
				<Property Name=""Enabled"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""Configuration"" Type=""Edm.String""/>
				<Property Name=""Instruction"" Type=""Edm.String""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Entity"" Type=""Example.Database.Entity"" Nullable=""false""/>
				<NavigationProperty Name=""EntityPageAction"" Type=""Example.Database.Action"" Nullable=""false""/>
				<NavigationProperty Name=""GridEntityField"" Type=""Example.Database.EntityField"" Nullable=""false""/>
				<NavigationProperty Name=""FormTemplate"" Type=""Example.Database.FormTemplate"" Nullable=""false""/>
				<NavigationProperty Name=""Module"" Type=""Example.Database.Module"" Nullable=""false""/>
				<NavigationProperty Name=""Query"" Type=""Example.Database.Query"" Nullable=""false""/>
				<NavigationProperty Name=""Operation"" Type=""Example.Database.Operation"" Nullable=""false""/>
				<NavigationProperty Name=""ReportType"" Type=""Example.Database.ReportType"" Nullable=""false""/>
				<NavigationProperty Name=""EditRecordField"" Type=""Example.Database.EntityField"" Nullable=""false""/>
				<NavigationProperty Name=""Application"" Type=""Example.Database.Application"" Nullable=""false""/>
				<NavigationProperty Name=""InstructionText"" Type=""Example.Database.Text"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""Role"">
				<Key>
					<PropertyRef Name=""RoleId""/>
				</Key>
				<Property Name=""RoleId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Type"" Type=""Edm.String""/>
				<Property Name=""IsSystemRole"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""UserRoles"" Type=""Collection(Example.Database.UserRole)""/>
				<NavigationProperty Name=""OperationUsers"" Type=""Collection(Example.Database.OperationUser)""/>
				<NavigationProperty Name=""RoleTasks"" Type=""Collection(Example.Database.RoleTask)""/>
			</EntityType>
			<EntityType Name=""UserRole"">
				<Key>
					<PropertyRef Name=""UserRoleId""/>
				</Key>
				<Property Name=""UserRoleId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Type"" Type=""Edm.String""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""User"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Role"" Type=""Example.Database.Role"" Nullable=""false""/>
				<NavigationProperty Name=""Application"" Type=""Example.Database.Application"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""Session"">
				<Key>
					<PropertyRef Name=""SessionId""/>
				</Key>
				<Property Name=""SessionId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""LoggedOn"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""Identity"" Type=""Edm.String""/>
				<Property Name=""ValidTo"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Host"" Type=""Edm.String""/>
				<Property Name=""IP"" Type=""Edm.String""/>
				<Property Name=""Agent"" Type=""Edm.String""/>
				<Property Name=""LastActivity"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""IdleSince"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""LoggedOff"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""ForwardedForHost"" Type=""Edm.String""/>
				<Property Name=""ForwardedForIP"" Type=""Edm.String""/>
				<Property Name=""SessionType"" Type=""Edm.String""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""User"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Parent"" Type=""Example.Database.Session"" Nullable=""false""/>
				<NavigationProperty Name=""MembershipProvider"" Type=""Example.Database.MembershipProvider"" Nullable=""false""/>
				<NavigationProperty Name=""Device"" Type=""Example.Database.Device"" Nullable=""false""/>
				<NavigationProperty Name=""SessionsByParent"" Type=""Collection(Example.Database.Session)""/>
				<NavigationProperty Name=""Audits"" Type=""Collection(Example.Database.Audit)""/>
				<NavigationProperty Name=""ElectronicSignatures"" Type=""Collection(Example.Database.ElectronicSignature)""/>
				<NavigationProperty Name=""InstallationLogs"" Type=""Collection(Example.Database.InstallationLog)""/>
				<NavigationProperty Name=""Credentials"" Type=""Collection(Example.Database.Credential)""/>
				<NavigationProperty Name=""Subscriptions"" Type=""Collection(Example.Database.Subscription)""/>
			</EntityType>
			<EntityType Name=""Address"">
				<Key>
					<PropertyRef Name=""AddressId""/>
				</Key>
				<Property Name=""AddressId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Entity"" Type=""Edm.String""/>
				<Property Name=""EntityId"" Type=""Edm.Int64""/>
				<Property Name=""Type"" Type=""Edm.String""/>
				<Property Name=""AddressLine1"" Type=""Edm.String""/>
				<Property Name=""AddressLine2"" Type=""Edm.String""/>
				<Property Name=""PostalCode"" Type=""Edm.String""/>
				<Property Name=""City"" Type=""Edm.String""/>
				<Property Name=""Country"" Type=""Edm.String""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""Note"">
				<Key>
					<PropertyRef Name=""NoteId""/>
				</Key>
				<Property Name=""NoteId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ItemId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Subject"" Type=""Edm.String""/>
				<Property Name=""Type"" Type=""Edm.String""/>
				<Property Name=""OrderNumber"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Text"" Type=""Edm.String""/>
				<Property Name=""DocumentId"" Type=""Edm.String""/>
				<Property Name=""CellIndex"" Type=""Edm.Int64""/>
				<Property Name=""EntityName"" Type=""Edm.String""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Entity"" Type=""Example.Database.Entity"" Nullable=""false""/>
				<NavigationProperty Name=""File"" Type=""Example.Database.File"" Nullable=""false""/>
				<NavigationProperty Name=""ReportType"" Type=""Example.Database.ReportType"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""Operation"">
				<Key>
					<PropertyRef Name=""OperationId""/>
				</Key>
				<Property Name=""Description"" Type=""Edm.String""/>
				<Property Name=""OperationId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Type"" Type=""Edm.String""/>
				<Property Name=""ClassName"" Type=""Edm.String""/>
				<Property Name=""Area"" Type=""Edm.String""/>
				<Property Name=""MethodName"" Type=""Edm.String""/>
				<Property Name=""Entity"" Type=""Edm.String""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""EntityFields"" Type=""Collection(Example.Database.EntityField)""/>
				<NavigationProperty Name=""EntityPages"" Type=""Collection(Example.Database.EntityPage)""/>
				<NavigationProperty Name=""OperationUsers"" Type=""Collection(Example.Database.OperationUser)""/>
				<NavigationProperty Name=""TaskOperations"" Type=""Collection(Example.Database.TaskOperation)""/>
				<NavigationProperty Name=""Modules"" Type=""Collection(Example.Database.Module)""/>
				<NavigationProperty Name=""Folders"" Type=""Collection(Example.Database.Folder)""/>
				<NavigationProperty Name=""Actions"" Type=""Collection(Example.Database.Action)""/>
				<NavigationProperty Name=""Applications"" Type=""Collection(Example.Database.Application)""/>
				<NavigationProperty Name=""Menus"" Type=""Collection(Example.Database.Menu)""/>
				<NavigationProperty Name=""ReportTypes"" Type=""Collection(Example.Database.ReportType)""/>
				<NavigationProperty Name=""ChangeRequestTypes"" Type=""Collection(Example.Database.ChangeRequestType)""/>
				<NavigationProperty Name=""Credentials"" Type=""Collection(Example.Database.Credential)""/>
				<NavigationProperty Name=""ProcessStations"" Type=""Collection(Example.Database.ProcessStation)""/>
			</EntityType>
			<EntityType Name=""OperationUser"">
				<Key>
					<PropertyRef Name=""OperationUserId""/>
				</Key>
				<Property Name=""OperationUserId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Operation"" Type=""Example.Database.Operation"" Nullable=""false""/>
				<NavigationProperty Name=""User"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Application"" Type=""Example.Database.Application"" Nullable=""false""/>
				<NavigationProperty Name=""Role"" Type=""Example.Database.Role"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""Task"">
				<Key>
					<PropertyRef Name=""TaskId""/>
				</Key>
				<Property Name=""TaskId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""IsSystemTask"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""RoleTasks"" Type=""Collection(Example.Database.RoleTask)""/>
				<NavigationProperty Name=""TaskOperations"" Type=""Collection(Example.Database.TaskOperation)""/>
			</EntityType>
			<EntityType Name=""RoleTask"">
				<Key>
					<PropertyRef Name=""RoleTaskId""/>
				</Key>
				<Property Name=""RoleTaskId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Role"" Type=""Example.Database.Role"" Nullable=""false""/>
				<NavigationProperty Name=""Task"" Type=""Example.Database.Task"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""TaskOperation"">
				<Key>
					<PropertyRef Name=""TaskOperationId""/>
				</Key>
				<Property Name=""TaskOperationId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Task"" Type=""Example.Database.Task"" Nullable=""false""/>
				<NavigationProperty Name=""Operation"" Type=""Example.Database.Operation"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""StoragePolicy"">
				<Key>
					<PropertyRef Name=""StoragePolicyId""/>
				</Key>
				<Property Name=""StoragePolicyId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<Property Name=""Type"" Type=""Edm.String""/>
				<Property Name=""Days"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""FileTypes"" Type=""Collection(Example.Database.FileType)""/>
				<NavigationProperty Name=""FileGroups"" Type=""Collection(Example.Database.FileGroup)""/>
				<NavigationProperty Name=""Files"" Type=""Collection(Example.Database.File)""/>
			</EntityType>
			<EntityType Name=""FileType"">
				<Key>
					<PropertyRef Name=""FileTypeId""/>
				</Key>
				<Property Name=""FileTypeId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModelProvider"" Type=""Edm.String""/>
				<Property Name=""ItemEventType"" Type=""Edm.String""/>
				<Property Name=""Schema"" Type=""Edm.String""/>
				<Property Name=""DeleteWhenExpired"" Type=""Edm.Boolean"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""StoragePolicy"" Type=""Example.Database.StoragePolicy"" Nullable=""false""/>
				<NavigationProperty Name=""Files"" Type=""Collection(Example.Database.File)""/>
			</EntityType>
			<EntityType Name=""FileTemplate"">
				<Key>
					<PropertyRef Name=""FileTemplateId""/>
				</Key>
				<Property Name=""FileTemplateId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ItemId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""EntityName"" Type=""Edm.String""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""File"" Type=""Example.Database.File"" Nullable=""false""/>
				<NavigationProperty Name=""Entity"" Type=""Example.Database.Entity"" Nullable=""false""/>
				<NavigationProperty Name=""Group"" Type=""Example.Database.Group"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""FileGroup"">
				<Key>
					<PropertyRef Name=""FileGroupId""/>
				</Key>
				<Property Name=""FileGroupId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Guid"" Type=""Edm.String""/>
				<Property Name=""Type"" Type=""Edm.String""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ItemId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""EntityName"" Type=""Edm.String""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<NavigationProperty Name=""Parent"" Type=""Example.Database.FileGroup"" Nullable=""false""/>
				<NavigationProperty Name=""StoragePolicy"" Type=""Example.Database.StoragePolicy"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Entity"" Type=""Example.Database.Entity"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""FileGroupsByParent"" Type=""Collection(Example.Database.FileGroup)""/>
				<NavigationProperty Name=""Files"" Type=""Collection(Example.Database.File)""/>
			</EntityType>
			<EntityType Name=""File"">
				<Key>
					<PropertyRef Name=""FileId""/>
				</Key>
				<Property Name=""FileId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""ItemId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""ValidTo"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Path"" Type=""Edm.String""/>
				<Property Name=""Signature"" Type=""Edm.String""/>
				<Property Name=""md5"" Type=""Edm.String""/>
				<Property Name=""sha256"" Type=""Edm.String""/>
				<Property Name=""Status"" Type=""Edm.String""/>
				<Property Name=""Content"" Type=""Edm.Binary""/>
				<Property Name=""Guid"" Type=""Edm.String""/>
				<Property Name=""ContentType"" Type=""Edm.String""/>
				<Property Name=""Size"" Type=""Edm.Int64""/>
				<Property Name=""Container"" Type=""Edm.String""/>
				<Property Name=""ExpiresOn"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""EntityName"" Type=""Edm.String""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""TimeStamp"" Type=""Edm.Binary""/>
				<NavigationProperty Name=""Entity"" Type=""Example.Database.Entity"" Nullable=""false""/>
				<NavigationProperty Name=""FileType"" Type=""Example.Database.FileType"" Nullable=""false""/>
				<NavigationProperty Name=""FileGroup"" Type=""Example.Database.FileGroup"" Nullable=""false""/>
				<NavigationProperty Name=""StoragePolicy"" Type=""Example.Database.StoragePolicy"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Notes"" Type=""Collection(Example.Database.Note)""/>
				<NavigationProperty Name=""FileTemplates"" Type=""Collection(Example.Database.FileTemplate)""/>
				<NavigationProperty Name=""ApplicationsByImage"" Type=""Collection(Example.Database.Application)""/>
				<NavigationProperty Name=""AuditFieldsByOldFile"" Type=""Collection(Example.Database.AuditField)""/>
				<NavigationProperty Name=""AuditFieldsByNewFile"" Type=""Collection(Example.Database.AuditField)""/>
				<NavigationProperty Name=""ReportTypeFiles"" Type=""Collection(Example.Database.ReportTypeFile)""/>
				<NavigationProperty Name=""Documents"" Type=""Collection(Example.Database.Document)""/>
				<NavigationProperty Name=""DocumentVersions"" Type=""Collection(Example.Database.DocumentVersion)""/>
				<NavigationProperty Name=""Properties"" Type=""Collection(Example.Database.Property)""/>
				<NavigationProperty Name=""NewsItemsByImage"" Type=""Collection(Example.Database.NewsItem)""/>
				<NavigationProperty Name=""SystemLogItems"" Type=""Collection(Example.Database.SystemLogItem)""/>
				<NavigationProperty Name=""Fonts"" Type=""Collection(Example.Database.Font)""/>
				<NavigationProperty Name=""InstallationPackages"" Type=""Collection(Example.Database.InstallationPackage)""/>
				<NavigationProperty Name=""UserTrainings"" Type=""Collection(Example.Database.UserTraining)""/>
				<NavigationProperty Name=""ProjectsByDocument"" Type=""Collection(Example.Database.Project)""/>
				<NavigationProperty Name=""ExperimentsByDocument"" Type=""Collection(Example.Database.Experiment)""/>
				<NavigationProperty Name=""Results"" Type=""Collection(Example.Database.Result)""/>
				<NavigationProperty Name=""OrderReports"" Type=""Collection(Example.Database.OrderReport)""/>
				<NavigationProperty Name=""ProjectDocuments"" Type=""Collection(Example.Database.ProjectDocument)""/>
				<NavigationProperty Name=""ProtocolsByDocument"" Type=""Collection(Example.Database.Protocol)""/>
				<NavigationProperty Name=""Chemicals"" Type=""Collection(Example.Database.Chemical)""/>
				<NavigationProperty Name=""Reactions"" Type=""Collection(Example.Database.Reaction)""/>
				<NavigationProperty Name=""InstrumentFiles"" Type=""Collection(Example.Database.InstrumentFile)""/>
				<NavigationProperty Name=""Invoices"" Type=""Collection(Example.Database.Invoice)""/>
			</EntityType>
			<EntityType Name=""UserRequest"">
				<Key>
					<PropertyRef Name=""UserRequestId""/>
				</Key>
				<Property Name=""UserRequestId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Request"" Type=""Edm.String""/>
				<Property Name=""Type"" Type=""Edm.String""/>
				<Property Name=""State"" Type=""Edm.String""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""UrlAction"">
				<Key>
					<PropertyRef Name=""UrlActionId""/>
				</Key>
				<Property Name=""UrlActionId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Area"" Type=""Edm.String""/>
				<Property Name=""Controller"" Type=""Edm.String""/>
				<Property Name=""Code"" Type=""Edm.String""/>
				<Property Name=""Type"" Type=""Edm.String""/>
				<Property Name=""Action"" Type=""Edm.String""/>
				<Property Name=""RecordId"" Type=""Edm.Int64""/>
				<Property Name=""ValidTo"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""IsValid"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""Data"" Type=""Edm.String""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Properties"" Type=""Collection(Example.Database.Property)""/>
			</EntityType>
			<EntityType Name=""Form"">
				<Key>
					<PropertyRef Name=""FormId""/>
				</Key>
				<Property Name=""FormId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ItemId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<Property Name=""Status"" Type=""Edm.String""/>
				<Property Name=""Row"" Type=""Edm.Int64""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""EntityName"" Type=""Edm.String""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Entity"" Type=""Example.Database.Entity"" Nullable=""false""/>
				<NavigationProperty Name=""FormTemplate"" Type=""Example.Database.FormTemplate"" Nullable=""false""/>
				<NavigationProperty Name=""FormTemplateVersion"" Type=""Example.Database.FormTemplateVersion"" Nullable=""false""/>
				<NavigationProperty Name=""FormTemplateSection"" Type=""Example.Database.FormTemplateSection"" Nullable=""false""/>
				<NavigationProperty Name=""Parent"" Type=""Example.Database.Form"" Nullable=""false""/>
				<NavigationProperty Name=""HorizontalForm"" Type=""Example.Database.Form"" Nullable=""false""/>
				<NavigationProperty Name=""VerticalForm"" Type=""Example.Database.Form"" Nullable=""false""/>
				<NavigationProperty Name=""EntityForm"" Type=""Example.Database.EntityForm"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""FormsByParent"" Type=""Collection(Example.Database.Form)""/>
				<NavigationProperty Name=""FormsByHorizontalForm"" Type=""Collection(Example.Database.Form)""/>
				<NavigationProperty Name=""FormsByVerticalForm"" Type=""Collection(Example.Database.Form)""/>
				<NavigationProperty Name=""FormFields"" Type=""Collection(Example.Database.FormField)""/>
			</EntityType>
			<EntityType Name=""FormField"">
				<Key>
					<PropertyRef Name=""FormFieldId""/>
				</Key>
				<Property Name=""FormFieldId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""OrderNumber"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Value"" Type=""Edm.String""/>
				<Property Name=""NumberValue"" Type=""Edm.Double""/>
				<Property Name=""ItemId"" Type=""Edm.Int64""/>
				<Property Name=""DateValue"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""TimeValue"" Type=""Edm.Duration""/>
				<Property Name=""EnteredOn"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""Authorised"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""AuthorisedOn"" Type=""Edm.DateTimeOffset""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Form"" Type=""Example.Database.Form"" Nullable=""false""/>
				<NavigationProperty Name=""FormTemplateField"" Type=""Example.Database.FormTemplateField"" Nullable=""false""/>
				<NavigationProperty Name=""EnteredBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""AuthorisedBy"" Type=""Example.Database.User"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""EntityForm"">
				<Key>
					<PropertyRef Name=""EntityFormId""/>
				</Key>
				<Property Name=""EntityFormId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Status"" Type=""Edm.String""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""ItemId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""OrderNumber"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""HasNotes"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""EntityName"" Type=""Edm.String""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""FormTemplate"" Type=""Example.Database.FormTemplate"" Nullable=""false""/>
				<NavigationProperty Name=""FormTemplateVersion"" Type=""Example.Database.FormTemplateVersion"" Nullable=""false""/>
				<NavigationProperty Name=""Entity"" Type=""Example.Database.Entity"" Nullable=""false""/>
				<NavigationProperty Name=""EntityFormTemplate"" Type=""Example.Database.EntityFormTemplate"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Forms"" Type=""Collection(Example.Database.Form)""/>
			</EntityType>
			<EntityType Name=""OutgoingMessage"">
				<Key>
					<PropertyRef Name=""OutgoingMessageId""/>
				</Key>
				<Property Name=""OutgoingMessageId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Type"" Type=""Edm.String""/>
				<Property Name=""Body"" Type=""Edm.String""/>
				<Property Name=""HtmlBody"" Type=""Edm.String""/>
				<Property Name=""Status"" Type=""Edm.String""/>
				<Property Name=""Destination"" Type=""Edm.String""/>
				<Property Name=""Bcc"" Type=""Edm.String""/>
				<Property Name=""Sender"" Type=""Edm.String""/>
				<Property Name=""Subject"" Type=""Edm.String""/>
				<Property Name=""StatusMessage"" Type=""Edm.String""/>
				<Property Name=""Data"" Type=""Edm.String""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""FormTemplate"">
				<Key>
					<PropertyRef Name=""FormTemplateId""/>
				</Key>
				<Property Name=""FormTemplateId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<Property Name=""Instruction"" Type=""Edm.String""/>
				<Property Name=""Type"" Type=""Edm.String""/>
				<Property Name=""Version"" Type=""Edm.Int64""/>
				<Property Name=""CanCreateGraph"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""CanCreateNewForm"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""OverrideUserSetting"" Type=""Edm.Boolean""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""CurrentVersion"" Type=""Example.Database.FormTemplateVersion"" Nullable=""false""/>
				<NavigationProperty Name=""Entity"" Type=""Example.Database.Entity"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Group"" Type=""Example.Database.Group"" Nullable=""false""/>
				<NavigationProperty Name=""InstructionText"" Type=""Example.Database.Text"" Nullable=""false""/>
				<NavigationProperty Name=""Entities"" Type=""Collection(Example.Database.Entity)""/>
				<NavigationProperty Name=""EntityPages"" Type=""Collection(Example.Database.EntityPage)""/>
				<NavigationProperty Name=""Forms"" Type=""Collection(Example.Database.Form)""/>
				<NavigationProperty Name=""EntityForms"" Type=""Collection(Example.Database.EntityForm)""/>
				<NavigationProperty Name=""EntityFormTemplates"" Type=""Collection(Example.Database.EntityFormTemplate)""/>
				<NavigationProperty Name=""FormTemplateVersions"" Type=""Collection(Example.Database.FormTemplateVersion)""/>
				<NavigationProperty Name=""FormTemplateSections"" Type=""Collection(Example.Database.FormTemplateSection)""/>
				<NavigationProperty Name=""SequenceTypesBySequenceResultEntryFormTemplate"" Type=""Collection(Example.Database.SequenceType)""/>
			</EntityType>
			<EntityType Name=""FormTemplateField"">
				<Key>
					<PropertyRef Name=""FormTemplateFieldId""/>
				</Key>
				<Property Name=""FormTemplateFieldId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""DataType"" Type=""Edm.String""/>
				<Property Name=""Required"" Type=""Edm.Boolean""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Readonly"" Type=""Edm.Boolean""/>
				<Property Name=""MinimumLength"" Type=""Edm.Int64""/>
				<Property Name=""MaximumLength"" Type=""Edm.Int64""/>
				<Property Name=""OrderNumber"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Section"" Type=""Edm.String""/>
				<Property Name=""Label"" Type=""Edm.String""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<Property Name=""Instruction"" Type=""Edm.String""/>
				<Property Name=""DefaultValue"" Type=""Edm.String""/>
				<Property Name=""RowDataExpression"" Type=""Edm.String""/>
				<Property Name=""MultipleValues"" Type=""Edm.Boolean""/>
				<Property Name=""Format"" Type=""Edm.String""/>
				<Property Name=""EnabledExpression"" Type=""Edm.String""/>
				<Property Name=""Expression"" Type=""Edm.String""/>
				<Property Name=""FieldExpression"" Type=""Edm.String""/>
				<Property Name=""IsNameField"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""MaximumValue"" Type=""Edm.Double""/>
				<Property Name=""MinimumValue"" Type=""Edm.Double""/>
				<Property Name=""Precision"" Type=""Edm.Int64""/>
				<Property Name=""TrueValue"" Type=""Edm.String""/>
				<Property Name=""FalseValue"" Type=""Edm.String""/>
				<Property Name=""CalculationExpression"" Type=""Edm.String""/>
				<Property Name=""AllowOnlyPhraseValues"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""Mask"" Type=""Edm.String""/>
				<Property Name=""Calculated"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""PickerType"" Type=""Edm.String""/>
				<Property Name=""VisibleExpression"" Type=""Edm.String""/>
				<Property Name=""DefaultWidth"" Type=""Edm.String""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Phrase"" Type=""Example.Database.Phrase"" Nullable=""false""/>
				<NavigationProperty Name=""FormTemplateSection"" Type=""Example.Database.FormTemplateSection"" Nullable=""false""/>
				<NavigationProperty Name=""Entity"" Type=""Example.Database.Entity"" Nullable=""false""/>
				<NavigationProperty Name=""DependsOn"" Type=""Example.Database.FormTemplateField"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""LabelText"" Type=""Example.Database.Text"" Nullable=""false""/>
				<NavigationProperty Name=""InstructionText"" Type=""Example.Database.Text"" Nullable=""false""/>
				<NavigationProperty Name=""FormFields"" Type=""Collection(Example.Database.FormField)""/>
				<NavigationProperty Name=""FormTemplateFieldsByDependsOn"" Type=""Collection(Example.Database.FormTemplateField)""/>
				<NavigationProperty Name=""FormTemplateVersionsByNameField"" Type=""Collection(Example.Database.FormTemplateVersion)""/>
			</EntityType>
			<EntityType Name=""EntityFormTemplate"">
				<Key>
					<PropertyRef Name=""EntityFormTemplateId""/>
				</Key>
				<Property Name=""EntityFormTemplateId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ItemId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""AuditType"" Type=""Edm.String""/>
				<Property Name=""OrderNumber"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<Property Name=""Type"" Type=""Edm.String""/>
				<Property Name=""ReadonlyExpression"" Type=""Edm.String""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""EntityName"" Type=""Edm.String""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""FormTemplate"" Type=""Example.Database.FormTemplate"" Nullable=""false""/>
				<NavigationProperty Name=""Entity"" Type=""Example.Database.Entity"" Nullable=""false""/>
				<NavigationProperty Name=""Location"" Type=""Example.Database.Location"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""EntityForms"" Type=""Collection(Example.Database.EntityForm)""/>
			</EntityType>
			<EntityType Name=""Exception"">
				<Key>
					<PropertyRef Name=""ExceptionId""/>
				</Key>
				<Property Name=""ExceptionId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Type"" Type=""Edm.String""/>
				<Property Name=""Stacktrace"" Type=""Edm.String""/>
				<Property Name=""Message"" Type=""Edm.String""/>
				<Property Name=""Source"" Type=""Edm.String""/>
				<Property Name=""TargetSite"" Type=""Edm.String""/>
				<Property Name=""email"" Type=""Edm.String""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<Property Name=""FollowUp"" Type=""Edm.String""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ParentException"" Type=""Example.Database.Exception"" Nullable=""false""/>
				<NavigationProperty Name=""ExceptionsByParentException"" Type=""Collection(Example.Database.Exception)""/>
				<NavigationProperty Name=""ExceptionParameters"" Type=""Collection(Example.Database.ExceptionParameter)""/>
				<NavigationProperty Name=""SystemLogItems"" Type=""Collection(Example.Database.SystemLogItem)""/>
			</EntityType>
			<EntityType Name=""ExceptionParameter"">
				<Key>
					<PropertyRef Name=""ExceptionParameterId""/>
				</Key>
				<Property Name=""ExceptionParameterId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Type"" Type=""Edm.String""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Value"" Type=""Edm.String""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Exception"" Type=""Example.Database.Exception"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""FormTemplateVersion"">
				<Key>
					<PropertyRef Name=""FormTemplateVersionId""/>
				</Key>
				<Property Name=""FormTemplateVersionId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Version"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""InputMethod"" Type=""Edm.String""/>
				<Property Name=""DefaultLayout"" Type=""Edm.String""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""NameField"" Type=""Example.Database.FormTemplateField"" Nullable=""false""/>
				<NavigationProperty Name=""FormTemplate"" Type=""Example.Database.FormTemplate"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Forms"" Type=""Collection(Example.Database.Form)""/>
				<NavigationProperty Name=""EntityForms"" Type=""Collection(Example.Database.EntityForm)""/>
				<NavigationProperty Name=""FormTemplatesByCurrentVersion"" Type=""Collection(Example.Database.FormTemplate)""/>
				<NavigationProperty Name=""FormTemplateSections"" Type=""Collection(Example.Database.FormTemplateSection)""/>
			</EntityType>
			<EntityType Name=""FormTemplateSection"">
				<Key>
					<PropertyRef Name=""FormTemplateSectionId""/>
				</Key>
				<Property Name=""FormTemplateSectionId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Label"" Type=""Edm.String""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<Property Name=""OrderNumber"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Type"" Type=""Edm.String""/>
				<Property Name=""RowDataExpression"" Type=""Edm.String""/>
				<Property Name=""RowDataSortExpression"" Type=""Edm.String""/>
				<Property Name=""CanAuthorise"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""FormTemplateVersion"" Type=""Example.Database.FormTemplateVersion"" Nullable=""false""/>
				<NavigationProperty Name=""FormTemplate"" Type=""Example.Database.FormTemplate"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""LabelText"" Type=""Example.Database.Text"" Nullable=""false""/>
				<NavigationProperty Name=""Forms"" Type=""Collection(Example.Database.Form)""/>
				<NavigationProperty Name=""FormTemplateFields"" Type=""Collection(Example.Database.FormTemplateField)""/>
			</EntityType>
			<EntityType Name=""Module"">
				<Key>
					<PropertyRef Name=""ModuleId""/>
				</Key>
				<Property Name=""ModuleId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Icon"" Type=""Edm.String""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<Property Name=""Type"" Type=""Edm.String""/>
				<Property Name=""View"" Type=""Edm.String""/>
				<Property Name=""Script"" Type=""Edm.String""/>
				<Property Name=""ScriptFile"" Type=""Edm.String""/>
				<Property Name=""Component"" Type=""Edm.String""/>
				<Property Name=""ClientModules"" Type=""Edm.String""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""TimeStamp"" Type=""Edm.Binary""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Operation"" Type=""Example.Database.Operation"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""EntityPages"" Type=""Collection(Example.Database.EntityPage)""/>
				<NavigationProperty Name=""Folders"" Type=""Collection(Example.Database.Folder)""/>
				<NavigationProperty Name=""FoldersByItemModule"" Type=""Collection(Example.Database.Folder)""/>
				<NavigationProperty Name=""Actions"" Type=""Collection(Example.Database.Action)""/>
				<NavigationProperty Name=""FolderEntities"" Type=""Collection(Example.Database.FolderEntity)""/>
				<NavigationProperty Name=""FolderEntitiesByItemModule"" Type=""Collection(Example.Database.FolderEntity)""/>
				<NavigationProperty Name=""DeviceTasks"" Type=""Collection(Example.Database.DeviceTask)""/>
				<NavigationProperty Name=""ResultTypes"" Type=""Collection(Example.Database.ResultType)""/>
				<NavigationProperty Name=""InstrumentTypesByTypescriptModule"" Type=""Collection(Example.Database.InstrumentType)""/>
			</EntityType>
			<EntityType Name=""Folder"">
				<Key>
					<PropertyRef Name=""FolderId""/>
				</Key>
				<Property Name=""FolderId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Icon"" Type=""Edm.String""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<Property Name=""Data"" Type=""Edm.String""/>
				<Property Name=""Configuration"" Type=""Edm.String""/>
				<Property Name=""SortExpression"" Type=""Edm.String""/>
				<Property Name=""FolderType"" Type=""Edm.String""/>
				<Property Name=""OrderNumber"" Type=""Edm.Int64""/>
				<Property Name=""DisplayExpression"" Type=""Edm.String""/>
				<Property Name=""Public"" Type=""Edm.Boolean"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ParentFolder"" Type=""Example.Database.Folder"" Nullable=""false""/>
				<NavigationProperty Name=""Entity"" Type=""Example.Database.Entity"" Nullable=""false""/>
				<NavigationProperty Name=""Action"" Type=""Example.Database.Action"" Nullable=""false""/>
				<NavigationProperty Name=""Operation"" Type=""Example.Database.Operation"" Nullable=""false""/>
				<NavigationProperty Name=""Application"" Type=""Example.Database.Application"" Nullable=""false""/>
				<NavigationProperty Name=""Module"" Type=""Example.Database.Module"" Nullable=""false""/>
				<NavigationProperty Name=""ItemModule"" Type=""Example.Database.Module"" Nullable=""false""/>
				<NavigationProperty Name=""FoldersByParentFolder"" Type=""Collection(Example.Database.Folder)""/>
				<NavigationProperty Name=""FolderFilters"" Type=""Collection(Example.Database.FolderFilter)""/>
				<NavigationProperty Name=""FolderEntities"" Type=""Collection(Example.Database.FolderEntity)""/>
			</EntityType>
			<EntityType Name=""FolderFilter"">
				<Key>
					<PropertyRef Name=""FolderFilterId""/>
				</Key>
				<Property Name=""FolderFilterId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""IsDefaultValue"" Type=""Edm.Boolean""/>
				<Property Name=""Condition"" Type=""Edm.String""/>
				<Property Name=""TextValue"" Type=""Edm.String""/>
				<Property Name=""NumberValue"" Type=""Edm.Double""/>
				<Property Name=""DateValue"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""ItemId"" Type=""Edm.Int64""/>
				<Property Name=""EntityName"" Type=""Edm.String""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Folder"" Type=""Example.Database.Folder"" Nullable=""false""/>
				<NavigationProperty Name=""FolderEntity"" Type=""Example.Database.FolderEntity"" Nullable=""false""/>
				<NavigationProperty Name=""EntityField"" Type=""Example.Database.EntityField"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""Action"">
				<Key>
					<PropertyRef Name=""ActionId""/>
				</Key>
				<Property Name=""ActionId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Controller"" Type=""Edm.String""/>
				<Property Name=""Area"" Type=""Edm.String""/>
				<Property Name=""Title"" Type=""Edm.String""/>
				<Property Name=""Type"" Type=""Edm.String""/>
				<Property Name=""Function"" Type=""Edm.String""/>
				<Property Name=""Icon"" Type=""Edm.String""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Operation"" Type=""Example.Database.Operation"" Nullable=""false""/>
				<NavigationProperty Name=""Module"" Type=""Example.Database.Module"" Nullable=""false""/>
				<NavigationProperty Name=""EntityPagesByEntityPageAction"" Type=""Collection(Example.Database.EntityPage)""/>
				<NavigationProperty Name=""Folders"" Type=""Collection(Example.Database.Folder)""/>
				<NavigationProperty Name=""ActionParameters"" Type=""Collection(Example.Database.ActionParameter)""/>
				<NavigationProperty Name=""FolderEntities"" Type=""Collection(Example.Database.FolderEntity)""/>
				<NavigationProperty Name=""Menus"" Type=""Collection(Example.Database.Menu)""/>
				<NavigationProperty Name=""EntityActions"" Type=""Collection(Example.Database.EntityAction)""/>
				<NavigationProperty Name=""EntityHierarchys"" Type=""Collection(Example.Database.EntityHierarchy)""/>
			</EntityType>
			<EntityType Name=""ActionParameter"">
				<Key>
					<PropertyRef Name=""ActionParameterId""/>
				</Key>
				<Property Name=""ActionParameterId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Value"" Type=""Edm.String""/>
				<Property Name=""OrderNumber"" Type=""Edm.Int64"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Action"" Type=""Example.Database.Action"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""FolderEntity"">
				<Key>
					<PropertyRef Name=""FolderEntityId""/>
				</Key>
				<Property Name=""FolderEntityId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""SortExpression"" Type=""Edm.String""/>
				<Property Name=""Mode"" Type=""Edm.String""/>
				<Property Name=""DisplayExpression"" Type=""Edm.String""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<Property Name=""Icon"" Type=""Edm.String""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Entity"" Type=""Example.Database.Entity"" Nullable=""false""/>
				<NavigationProperty Name=""Query"" Type=""Example.Database.Query"" Nullable=""false""/>
				<NavigationProperty Name=""ChildEntity"" Type=""Example.Database.Entity"" Nullable=""false""/>
				<NavigationProperty Name=""Folder"" Type=""Example.Database.Folder"" Nullable=""false""/>
				<NavigationProperty Name=""EntityField"" Type=""Example.Database.EntityField"" Nullable=""false""/>
				<NavigationProperty Name=""Action"" Type=""Example.Database.Action"" Nullable=""false""/>
				<NavigationProperty Name=""Module"" Type=""Example.Database.Module"" Nullable=""false""/>
				<NavigationProperty Name=""ItemModule"" Type=""Example.Database.Module"" Nullable=""false""/>
				<NavigationProperty Name=""FolderFilters"" Type=""Collection(Example.Database.FolderFilter)""/>
			</EntityType>
			<EntityType Name=""Application"">
				<Key>
					<PropertyRef Name=""ApplicationId""/>
				</Key>
				<Property Name=""ApplicationId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Theme"" Type=""Edm.String""/>
				<Property Name=""CmsConfiguration"" Type=""Edm.String""/>
				<Property Name=""Public"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""Data"" Type=""Edm.String""/>
				<Property Name=""ApplicationType"" Type=""Edm.String""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Image"" Type=""Example.Database.File"" Nullable=""false""/>
				<NavigationProperty Name=""PolicyDocumentType"" Type=""Example.Database.DocumentType"" Nullable=""false""/>
				<NavigationProperty Name=""Operation"" Type=""Example.Database.Operation"" Nullable=""false""/>
				<NavigationProperty Name=""AccountProvider"" Type=""Collection(Example.Database.AccountProvider)""/>
				<NavigationProperty Name=""EntityPages"" Type=""Collection(Example.Database.EntityPage)""/>
				<NavigationProperty Name=""UserRoles"" Type=""Collection(Example.Database.UserRole)""/>
				<NavigationProperty Name=""OperationUsers"" Type=""Collection(Example.Database.OperationUser)""/>
				<NavigationProperty Name=""Folders"" Type=""Collection(Example.Database.Folder)""/>
				<NavigationProperty Name=""Menus"" Type=""Collection(Example.Database.Menu)""/>
				<NavigationProperty Name=""Settings"" Type=""Collection(Example.Database.Setting)""/>
				<NavigationProperty Name=""UserGroups"" Type=""Collection(Example.Database.UserGroup)""/>
			</EntityType>
			<EntityType Name=""Menu"">
				<Key>
					<PropertyRef Name=""MenuId""/>
				</Key>
				<Property Name=""MenuId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Icon"" Type=""Edm.String""/>
				<Property Name=""OrderNumber"" Type=""Edm.Int64""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Application"" Type=""Example.Database.Application"" Nullable=""false""/>
				<NavigationProperty Name=""Action"" Type=""Example.Database.Action"" Nullable=""false""/>
				<NavigationProperty Name=""Parent"" Type=""Example.Database.Menu"" Nullable=""false""/>
				<NavigationProperty Name=""Operation"" Type=""Example.Database.Operation"" Nullable=""false""/>
				<NavigationProperty Name=""MenusByParent"" Type=""Collection(Example.Database.Menu)""/>
			</EntityType>
			<EntityType Name=""Audit"">
				<Key>
					<PropertyRef Name=""AuditId""/>
				</Key>
				<Property Name=""AuditId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ItemId"" Type=""Edm.Int64""/>
				<Property Name=""Reason"" Type=""Edm.String""/>
				<Property Name=""ItemName"" Type=""Edm.String""/>
				<Property Name=""AuditType"" Type=""Edm.String""/>
				<Property Name=""EntityActionName"" Type=""Edm.String""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Entity"" Type=""Example.Database.Entity"" Nullable=""false""/>
				<NavigationProperty Name=""Session"" Type=""Example.Database.Session"" Nullable=""false""/>
				<NavigationProperty Name=""AuditItems"" Type=""Collection(Example.Database.AuditItem)""/>
			</EntityType>
			<EntityType Name=""AuditItem"">
				<Key>
					<PropertyRef Name=""AuditItemId""/>
				</Key>
				<Property Name=""AuditItemId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Action"" Type=""Edm.String""/>
				<Property Name=""ItemId"" Type=""Edm.Int64""/>
				<Property Name=""ItemName"" Type=""Edm.String""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Audit"" Type=""Example.Database.Audit"" Nullable=""false""/>
				<NavigationProperty Name=""Entity"" Type=""Example.Database.Entity"" Nullable=""false""/>
				<NavigationProperty Name=""AuditFields"" Type=""Collection(Example.Database.AuditField)""/>
			</EntityType>
			<EntityType Name=""AuditField"">
				<Key>
					<PropertyRef Name=""AuditFieldId""/>
				</Key>
				<Property Name=""AuditFieldId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""OldValue"" Type=""Edm.String""/>
				<Property Name=""NewValue"" Type=""Edm.String""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""AuditItem"" Type=""Example.Database.AuditItem"" Nullable=""false""/>
				<NavigationProperty Name=""OldFile"" Type=""Example.Database.File"" Nullable=""false""/>
				<NavigationProperty Name=""NewFile"" Type=""Example.Database.File"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""ElectronicSignatureType"">
				<Key>
					<PropertyRef Name=""ElectronicSignatureTypeId""/>
				</Key>
				<Property Name=""ElectronicSignatureTypeId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""PublicKey"" Type=""Edm.String""/>
				<Property Name=""HashAlgorithm"" Type=""Edm.String""/>
				<Property Name=""ReadOnly"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""Instruction"" Type=""Edm.String""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Entity"" Type=""Example.Database.Entity"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Group"" Type=""Example.Database.Group"" Nullable=""false""/>
				<NavigationProperty Name=""InstructionText"" Type=""Example.Database.Text"" Nullable=""false""/>
				<NavigationProperty Name=""ElectronicSignatureTypeFields"" Type=""Collection(Example.Database.ElectronicSignatureTypeField)""/>
				<NavigationProperty Name=""ElectronicSignatureTypeFieldsByFieldElectronicSignatureType"" Type=""Collection(Example.Database.ElectronicSignatureTypeField)""/>
				<NavigationProperty Name=""ElectronicSignatures"" Type=""Collection(Example.Database.ElectronicSignature)""/>
			</EntityType>
			<EntityType Name=""ElectronicSignatureTypeField"">
				<Key>
					<PropertyRef Name=""ElectronicSignatureTypeFieldId""/>
				</Key>
				<Property Name=""ElectronicSignatureTypeFieldId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""FieldExpression"" Type=""Edm.String""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""OrderNumber"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ElectronicSignatureType"" Type=""Example.Database.ElectronicSignatureType"" Nullable=""false""/>
				<NavigationProperty Name=""FieldElectronicSignatureType"" Type=""Example.Database.ElectronicSignatureType"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""ElectronicSignature"">
				<Key>
					<PropertyRef Name=""ElectronicSignatureId""/>
				</Key>
				<Property Name=""ElectronicSignatureId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Hash"" Type=""Edm.String""/>
				<Property Name=""Signature"" Type=""Edm.String""/>
				<Property Name=""SignedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ReadOnly"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""ItemId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""EntityName"" Type=""Edm.String""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Entity"" Type=""Example.Database.Entity"" Nullable=""false""/>
				<NavigationProperty Name=""ElectronicSignatureType"" Type=""Example.Database.ElectronicSignatureType"" Nullable=""false""/>
				<NavigationProperty Name=""Parent"" Type=""Example.Database.ElectronicSignature"" Nullable=""false""/>
				<NavigationProperty Name=""SignedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Session"" Type=""Example.Database.Session"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ElectronicSignaturesByParent"" Type=""Collection(Example.Database.ElectronicSignature)""/>
				<NavigationProperty Name=""ElectronicSignatureFields"" Type=""Collection(Example.Database.ElectronicSignatureField)""/>
			</EntityType>
			<EntityType Name=""ElectronicSignatureField"">
				<Key>
					<PropertyRef Name=""ElectronicSignatureFieldId""/>
				</Key>
				<Property Name=""ElectronicSignatureFieldId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""OrderNumber"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Value"" Type=""Edm.String""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ElectronicSignature"" Type=""Example.Database.ElectronicSignature"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""Setting"">
				<Key>
					<PropertyRef Name=""SettingId""/>
				</Key>
				<Property Name=""SettingId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""DatabaseVersion"" Type=""Edm.String""/>
				<Property Name=""UpgradeActionIndex"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""DataMigrationActionIndex"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""ApplicationVersion"" Type=""Edm.String""/>
				<Property Name=""MailDeliveryMethod"" Type=""Edm.String""/>
				<Property Name=""MailPickupDirectoryLocation"" Type=""Edm.String""/>
				<Property Name=""MailServer"" Type=""Edm.String""/>
				<Property Name=""MailFrom"" Type=""Edm.String""/>
				<Property Name=""MailPort"" Type=""Edm.Int64""/>
				<Property Name=""MailSSL"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""MailUsername"" Type=""Edm.String""/>
				<Property Name=""Type"" Type=""Edm.String""/>
				<Property Name=""UpdateState"" Type=""Edm.String""/>
				<Property Name=""Data"" Type=""Edm.String""/>
				<Property Name=""PasswordHistoryRule"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""PasswordHistoryCount"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""MaxFailedLoginAttempts"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""FailedLoginTimeOutInMinutes"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""AccessControlAllowOrigin"" Type=""Edm.String""/>
				<Property Name=""AccessControlAllowHeaders"" Type=""Edm.String""/>
				<Property Name=""AccessControlAllowCredentials"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""InvoiceNumberFormat"" Type=""Edm.String""/>
				<Property Name=""ConceptInvoiceNumberFormat"" Type=""Edm.String""/>
				<Property Name=""MicroModuleStatusChange"" Type=""Edm.Boolean""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Application"" Type=""Example.Database.Application"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""ReportType"">
				<Key>
					<PropertyRef Name=""ReportTypeId""/>
				</Key>
				<Property Name=""ReportTypeId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Template"" Type=""Edm.String""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<Property Name=""Type"" Type=""Edm.String""/>
				<Property Name=""Cached"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""RefreshInterval"" Type=""Edm.Int64""/>
				<Property Name=""RefreshOn"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""GeneratedOn"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""GUID"" Type=""Edm.String""/>
				<Property Name=""NameFormat"" Type=""Edm.String""/>
				<Property Name=""CanSaveReport"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""OrderNumber"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CanMail"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DefaultCulture"" Type=""Edm.String""/>
				<Property Name=""CacheStatus"" Type=""Edm.String""/>
				<Property Name=""CachGenerationTime"" Type=""Edm.Int64""/>
				<Property Name=""QueriesAllPartitions"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""ReportCanBeSavedCondition"" Type=""Edm.String""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""DataProvider"" Type=""Example.Database.DataProvider"" Nullable=""false""/>
				<NavigationProperty Name=""Entity"" Type=""Example.Database.Entity"" Nullable=""false""/>
				<NavigationProperty Name=""DefaultPrinter"" Type=""Example.Database.Printer"" Nullable=""false""/>
				<NavigationProperty Name=""Operation"" Type=""Example.Database.Operation"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Group"" Type=""Example.Database.Group"" Nullable=""false""/>
				<NavigationProperty Name=""EntityPages"" Type=""Collection(Example.Database.EntityPage)""/>
				<NavigationProperty Name=""Notes"" Type=""Collection(Example.Database.Note)""/>
				<NavigationProperty Name=""ReportTypeFormats"" Type=""Collection(Example.Database.ReportTypeFormat)""/>
				<NavigationProperty Name=""ReportTypeArguments"" Type=""Collection(Example.Database.ReportTypeArgument)""/>
				<NavigationProperty Name=""ReportTypeFiles"" Type=""Collection(Example.Database.ReportTypeFile)""/>
				<NavigationProperty Name=""Documents"" Type=""Collection(Example.Database.Document)""/>
				<NavigationProperty Name=""ClientsByInvoiceReportType"" Type=""Collection(Example.Database.Client)""/>
				<NavigationProperty Name=""TestTypes"" Type=""Collection(Example.Database.TestType)""/>
				<NavigationProperty Name=""TestTypeReportTypes"" Type=""Collection(Example.Database.TestTypeReportType)""/>
				<NavigationProperty Name=""OrderReports"" Type=""Collection(Example.Database.OrderReport)""/>
				<NavigationProperty Name=""ClientReportTypes"" Type=""Collection(Example.Database.ClientReportType)""/>
				<NavigationProperty Name=""MaterialTypes"" Type=""Collection(Example.Database.MaterialType)""/>
				<NavigationProperty Name=""BatchTypes"" Type=""Collection(Example.Database.BatchType)""/>
				<NavigationProperty Name=""ProductTypes"" Type=""Collection(Example.Database.ProductType)""/>
				<NavigationProperty Name=""ProductUnitTypes"" Type=""Collection(Example.Database.ProductUnitType)""/>
				<NavigationProperty Name=""OrderFormsByOrderConfirmationReportType"" Type=""Collection(Example.Database.OrderForm)""/>
				<NavigationProperty Name=""OrderFormReports"" Type=""Collection(Example.Database.OrderFormReport)""/>
				<NavigationProperty Name=""Invoices"" Type=""Collection(Example.Database.Invoice)""/>
			</EntityType>
			<EntityType Name=""ReportTypeFormat"">
				<Key>
					<PropertyRef Name=""ReportTypeFormatId""/>
				</Key>
				<Property Name=""ReportTypeFormatId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Format"" Type=""Edm.String""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ReportType"" Type=""Example.Database.ReportType"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""ReportTypeArgument"">
				<Key>
					<PropertyRef Name=""ReportTypeArgumentId""/>
				</Key>
				<Property Name=""ReportTypeArgumentId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""DataType"" Type=""Edm.String""/>
				<Property Name=""OrderNumber"" Type=""Edm.Int64""/>
				<Property Name=""SelectMultiple"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""Required"" Type=""Edm.Boolean"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ReportType"" Type=""Example.Database.ReportType"" Nullable=""false""/>
				<NavigationProperty Name=""Entity"" Type=""Example.Database.Entity"" Nullable=""false""/>
				<NavigationProperty Name=""Phrase"" Type=""Example.Database.Phrase"" Nullable=""false""/>
				<NavigationProperty Name=""Query"" Type=""Example.Database.Query"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""ReportTypeFile"">
				<Key>
					<PropertyRef Name=""ReportTypeFileId""/>
				</Key>
				<Property Name=""ReportTypeFileId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ReportType"" Type=""Example.Database.ReportType"" Nullable=""false""/>
				<NavigationProperty Name=""File"" Type=""Example.Database.File"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""DataProvider"">
				<Key>
					<PropertyRef Name=""DataProviderId""/>
				</Key>
				<Property Name=""DataProviderId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""ProviderName"" Type=""Edm.String""/>
				<Property Name=""ConnectionString"" Type=""Edm.String""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ReportTypes"" Type=""Collection(Example.Database.ReportType)""/>
			</EntityType>
			<EntityType Name=""EntityAction"">
				<Key>
					<PropertyRef Name=""EntityActionId""/>
				</Key>
				<Property Name=""EntityActionId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Type"" Type=""Edm.String""/>
				<Property Name=""Condition"" Type=""Edm.String""/>
				<Property Name=""AuditType"" Type=""Edm.String""/>
				<Property Name=""CreateNote"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""Data"" Type=""Edm.String""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""OrderNumber"" Type=""Edm.Int64""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Entity"" Type=""Example.Database.Entity"" Nullable=""false""/>
				<NavigationProperty Name=""Action"" Type=""Example.Database.Action"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""UserSetting"">
				<Key>
					<PropertyRef Name=""UserSettingId""/>
				</Key>
				<Property Name=""UserSettingId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Value"" Type=""Edm.String""/>
				<Property Name=""ItemId"" Type=""Edm.Int64""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Entity"" Type=""Example.Database.Entity"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""Device"">
				<Key>
					<PropertyRef Name=""DeviceId""/>
				</Key>
				<Property Name=""DeviceId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<Property Name=""DeviceUniqueId"" Type=""Edm.String""/>
				<Property Name=""DeviceVersion"" Type=""Edm.String""/>
				<Property Name=""PlatformType"" Type=""Edm.String""/>
				<Property Name=""PlatformVersion"" Type=""Edm.String""/>
				<Property Name=""Platform"" Type=""Edm.String""/>
				<Property Name=""Configuration"" Type=""Edm.String""/>
				<Property Name=""Connected"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""Connection"" Type=""Edm.String""/>
				<Property Name=""Enabled"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""MobileCode"" Type=""Edm.String""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ItemEvents"" Type=""Collection(Example.Database.ItemEvent)""/>
				<NavigationProperty Name=""Sessions"" Type=""Collection(Example.Database.Session)""/>
				<NavigationProperty Name=""Printers"" Type=""Collection(Example.Database.Printer)""/>
				<NavigationProperty Name=""DeviceTasks"" Type=""Collection(Example.Database.DeviceTask)""/>
				<NavigationProperty Name=""Subscriptions"" Type=""Collection(Example.Database.Subscription)""/>
				<NavigationProperty Name=""Instruments"" Type=""Collection(Example.Database.Instrument)""/>
				<NavigationProperty Name=""InstrumentFiles"" Type=""Collection(Example.Database.InstrumentFile)""/>
			</EntityType>
			<EntityType Name=""LocationType"">
				<Key>
					<PropertyRef Name=""LocationTypeId""/>
				</Key>
				<Property Name=""LocationTypeId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Costs"" Type=""Edm.Double""/>
				<Property Name=""AdditionalCosts"" Type=""Edm.Double""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Locations"" Type=""Collection(Example.Database.Location)""/>
				<NavigationProperty Name=""LocationItemTypes"" Type=""Collection(Example.Database.LocationItemType)""/>
			</EntityType>
			<EntityType Name=""Query"">
				<Key>
					<PropertyRef Name=""QueryId""/>
				</Key>
				<Property Name=""QueryId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Statement"" Type=""Edm.String""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<Property Name=""IsSystemQuery"" Type=""Edm.Boolean"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Entity"" Type=""Example.Database.Entity"" Nullable=""false""/>
				<NavigationProperty Name=""EntityFieldsByFilterQuery"" Type=""Collection(Example.Database.EntityField)""/>
				<NavigationProperty Name=""EntityPages"" Type=""Collection(Example.Database.EntityPage)""/>
				<NavigationProperty Name=""FolderEntities"" Type=""Collection(Example.Database.FolderEntity)""/>
				<NavigationProperty Name=""ReportTypeArguments"" Type=""Collection(Example.Database.ReportTypeArgument)""/>
				<NavigationProperty Name=""QueryExpressions"" Type=""Collection(Example.Database.QueryExpression)""/>
			</EntityType>
			<EntityType Name=""QueryExpression"">
				<Key>
					<PropertyRef Name=""QueryExpressionId""/>
				</Key>
				<Property Name=""QueryExpressionId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Expression"" Type=""Edm.String""/>
				<Property Name=""DateValue"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""NumberValue"" Type=""Edm.Double"" Nullable=""false""/>
				<Property Name=""TextValue"" Type=""Edm.String""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Query"" Type=""Example.Database.Query"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""TimestampValue"">
				<Key>
					<PropertyRef Name=""TimestampValueId""/>
				</Key>
				<Property Name=""TimestampValueId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Type"" Type=""Edm.String""/>
				<Property Name=""LastTimestamp"" Type=""Edm.Binary""/>
				<Property Name=""TimeStamp"" Type=""Edm.Binary""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Entity"" Type=""Example.Database.Entity"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""DocumentType"">
				<Key>
					<PropertyRef Name=""DocumentTypeId""/>
				</Key>
				<Property Name=""DocumentTypeId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Content"" Type=""Edm.String""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<Property Name=""ManagedDocument"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DataDocument"" Type=""Edm.Boolean"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Owner"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ApplicationsByPolicyDocumentType"" Type=""Collection(Example.Database.Application)""/>
				<NavigationProperty Name=""DocumentIndices"" Type=""Collection(Example.Database.DocumentIndex)""/>
				<NavigationProperty Name=""Documents"" Type=""Collection(Example.Database.Document)""/>
				<NavigationProperty Name=""DocumentVersions"" Type=""Collection(Example.Database.DocumentVersion)""/>
			</EntityType>
			<EntityType Name=""DocumentIndex"">
				<Key>
					<PropertyRef Name=""DocumentIndexId""/>
				</Key>
				<Property Name=""DocumentIndexId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Schema"" Type=""Edm.String""/>
				<Property Name=""Collection"" Type=""Edm.String""/>
				<Property Name=""Indexer"" Type=""Edm.String""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""DocumentType"" Type=""Example.Database.DocumentType"" Nullable=""false""/>
				<NavigationProperty Name=""Documents"" Type=""Collection(Example.Database.Document)""/>
			</EntityType>
			<EntityType Name=""Document"">
				<Key>
					<PropertyRef Name=""DocumentId""/>
				</Key>
				<Property Name=""DocumentId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Content"" Type=""Edm.String""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""ItemId"" Type=""Edm.Int64""/>
				<Property Name=""Subject"" Type=""Edm.String""/>
				<Property Name=""Text"" Type=""Edm.String""/>
				<Property Name=""Type"" Type=""Edm.String""/>
				<Property Name=""DocumentGuid"" Type=""Edm.String""/>
				<Property Name=""Version"" Type=""Edm.Int64""/>
				<Property Name=""HasNotes"" Type=""Edm.Boolean"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""DocumentType"" Type=""Example.Database.DocumentType"" Nullable=""false""/>
				<NavigationProperty Name=""Entity"" Type=""Example.Database.Entity"" Nullable=""false""/>
				<NavigationProperty Name=""File"" Type=""Example.Database.File"" Nullable=""false""/>
				<NavigationProperty Name=""ReportType"" Type=""Example.Database.ReportType"" Nullable=""false""/>
				<NavigationProperty Name=""DocumentIndex"" Type=""Example.Database.DocumentIndex"" Nullable=""false""/>
				<NavigationProperty Name=""Owner"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Client"" Type=""Example.Database.Client"" Nullable=""false""/>
				<NavigationProperty Name=""Group"" Type=""Example.Database.Group"" Nullable=""false""/>
				<NavigationProperty Name=""DocumentVersionsByParent"" Type=""Collection(Example.Database.DocumentVersion)""/>
				<NavigationProperty Name=""InstallationsByOperationalQualificationDocument"" Type=""Collection(Example.Database.Installation)""/>
				<NavigationProperty Name=""ConfigurationSystemLicensesByPurchaseOrder"" Type=""Collection(Example.Database.ConfigurationSystemLicense)""/>
				<NavigationProperty Name=""ConfigurationSystemLicensesByLicenseAgreement"" Type=""Collection(Example.Database.ConfigurationSystemLicense)""/>
			</EntityType>
			<EntityType Name=""DocumentVersion"">
				<Key>
					<PropertyRef Name=""DocumentVersionId""/>
				</Key>
				<Property Name=""DocumentVersionId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Subject"" Type=""Edm.String""/>
				<Property Name=""Text"" Type=""Edm.String""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""ReleaseDate"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""Status"" Type=""Edm.String""/>
				<Property Name=""Version"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Review"" Type=""Edm.Boolean""/>
				<Property Name=""ReviewPeriod"" Type=""Edm.Int64""/>
				<Property Name=""ReviewBefore"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""Rejected"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""CheckedOut"" Type=""Edm.Boolean""/>
				<Property Name=""UniqueId"" Type=""Edm.String""/>
				<Property Name=""WorkflowInstance"" Type=""Edm.String""/>
				<Property Name=""Authorised"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""AuthorisedOn"" Type=""Edm.DateTimeOffset""/>
				<NavigationProperty Name=""Parent"" Type=""Example.Database.Document"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""DocumentType"" Type=""Example.Database.DocumentType"" Nullable=""false""/>
				<NavigationProperty Name=""File"" Type=""Example.Database.File"" Nullable=""false""/>
				<NavigationProperty Name=""Owner"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Authorizer"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""AuthorisedBy"" Type=""Example.Database.User"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""ChangeRequest"">
				<Key>
					<PropertyRef Name=""ChangeRequestId""/>
				</Key>
				<Property Name=""ChangeRequestId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""NameFormat"" Type=""Edm.String""/>
				<Property Name=""Subject"" Type=""Edm.String""/>
				<Property Name=""Text"" Type=""Edm.String""/>
				<Property Name=""Status"" Type=""Edm.String""/>
				<Property Name=""ItemId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Data"" Type=""Edm.String""/>
				<Property Name=""EntityName"" Type=""Edm.String""/>
				<NavigationProperty Name=""Entity"" Type=""Example.Database.Entity"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ChangeRequestType"" Type=""Example.Database.ChangeRequestType"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""ChangeRequestType"">
				<Key>
					<PropertyRef Name=""ChangeRequestTypeId""/>
				</Key>
				<Property Name=""ChangeRequestTypeId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Data"" Type=""Edm.String""/>
				<Property Name=""NameFormat"" Type=""Edm.String""/>
				<NavigationProperty Name=""Entity"" Type=""Example.Database.Entity"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Operation"" Type=""Example.Database.Operation"" Nullable=""false""/>
				<NavigationProperty Name=""ChangeRequests"" Type=""Collection(Example.Database.ChangeRequest)""/>
			</EntityType>
			<EntityType Name=""Counter"">
				<Key>
					<PropertyRef Name=""CounterId""/>
				</Key>
				<Property Name=""CounterId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Key"" Type=""Edm.String""/>
				<Property Name=""Value"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""ItemId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""EntityName"" Type=""Edm.String""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Entity"" Type=""Example.Database.Entity"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""Property"">
				<Key>
					<PropertyRef Name=""PropertyId""/>
				</Key>
				<Property Name=""PropertyId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Type"" Type=""Edm.String""/>
				<Property Name=""DataType"" Type=""Edm.String""/>
				<Property Name=""TextValue"" Type=""Edm.String""/>
				<Property Name=""DateValue"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""NumberValue"" Type=""Edm.Double""/>
				<Property Name=""OrderNumber"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""ItemId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""EntityName"" Type=""Edm.String""/>
				<Property Name=""LinkedItemId"" Type=""Edm.Int64""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""File"" Type=""Example.Database.File"" Nullable=""false""/>
				<NavigationProperty Name=""Entity"" Type=""Example.Database.Entity"" Nullable=""false""/>
				<NavigationProperty Name=""UrlAction"" Type=""Example.Database.UrlAction"" Nullable=""false""/>
				<NavigationProperty Name=""LinkedEntity"" Type=""Example.Database.Entity"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""EntityHierarchy"">
				<Key>
					<PropertyRef Name=""EntityHierarchyId""/>
				</Key>
				<Property Name=""EntityHierarchyId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ShowParent"" Type=""Edm.Boolean""/>
				<Property Name=""ShowChild"" Type=""Edm.Boolean""/>
				<Property Name=""ParentName"" Type=""Edm.String""/>
				<Property Name=""ChildName"" Type=""Edm.String""/>
				<Property Name=""ReadonlyExpression"" Type=""Edm.String""/>
				<Property Name=""AccessRule"" Type=""Edm.String""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ParentEntity"" Type=""Example.Database.Entity"" Nullable=""false""/>
				<NavigationProperty Name=""EntityField"" Type=""Example.Database.EntityField"" Nullable=""false""/>
				<NavigationProperty Name=""Action"" Type=""Example.Database.Action"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""Printer"">
				<Key>
					<PropertyRef Name=""PrinterId""/>
				</Key>
				<Property Name=""PrinterId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Type"" Type=""Edm.String""/>
				<Property Name=""PrinterModule"" Type=""Edm.String""/>
				<Property Name=""Address"" Type=""Edm.String""/>
				<Property Name=""Data"" Type=""Edm.String""/>
				<Property Name=""Connected"" Type=""Edm.Boolean""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Device"" Type=""Example.Database.Device"" Nullable=""false""/>
				<NavigationProperty Name=""ReportTypesByDefaultPrinter"" Type=""Collection(Example.Database.ReportType)""/>
				<NavigationProperty Name=""Locations"" Type=""Collection(Example.Database.Location)""/>
				<NavigationProperty Name=""OrderReportDestinations"" Type=""Collection(Example.Database.OrderReportDestination)""/>
				<NavigationProperty Name=""OrderFormReportDestinations"" Type=""Collection(Example.Database.OrderFormReportDestination)""/>
			</EntityType>
			<EntityType Name=""Country"">
				<Key>
					<PropertyRef Name=""CountryId""/>
				</Key>
				<Property Name=""CountryId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""TwoLetterISOCountryName"" Type=""Edm.String""/>
				<Property Name=""ThreeLetterISOCountriesName"" Type=""Edm.String""/>
				<Property Name=""NativeName"" Type=""Edm.String""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Text"" Type=""Example.Database.Text"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""NewsItem"">
				<Key>
					<PropertyRef Name=""NewsItemId""/>
				</Key>
				<Property Name=""NewsItemId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Type"" Type=""Edm.String""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<Property Name=""Date"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ValidFrom"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ValidTo"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Image"" Type=""Example.Database.File"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""TypeBinding"">
				<Key>
					<PropertyRef Name=""TypeBindingId""/>
				</Key>
				<Property Name=""TypeBindingId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Enabled"" Type=""Edm.Boolean""/>
				<Property Name=""Module"" Type=""Edm.String""/>
				<Property Name=""InstanceType"" Type=""Edm.String""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""DeviceTask"">
				<Key>
					<PropertyRef Name=""DeviceTaskId""/>
				</Key>
				<Property Name=""DeviceTaskId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Enabled"" Type=""Edm.Boolean""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""ExecutionGroup"" Type=""Edm.String""/>
				<Property Name=""Type"" Type=""Edm.String""/>
				<Property Name=""Interval"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Status"" Type=""Edm.String""/>
				<Property Name=""StartExecution"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""LastExecution"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""NextExecution"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""Data"" Type=""Edm.String""/>
				<Property Name=""IsServiceTask"" Type=""Edm.Boolean""/>
				<Property Name=""OrderNumber"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""LoggingLevel"" Type=""Edm.String""/>
				<Property Name=""TriggerType"" Type=""Edm.String""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<NavigationProperty Name=""Device"" Type=""Example.Database.Device"" Nullable=""false""/>
				<NavigationProperty Name=""Credential"" Type=""Example.Database.Credential"" Nullable=""false""/>
				<NavigationProperty Name=""Module"" Type=""Example.Database.Module"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""UpgradeState"">
				<Key>
					<PropertyRef Name=""UpgradeStateId""/>
				</Key>
				<Property Name=""UpgradeStateId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Version"" Type=""Edm.Double"" Nullable=""false""/>
				<Property Name=""Revision"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Status"" Type=""Edm.String""/>
				<Property Name=""PendingReason"" Type=""Edm.String""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""UserGroup"">
				<Key>
					<PropertyRef Name=""UserGroupId""/>
				</Key>
				<Property Name=""UserGroupId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Access"" Type=""Edm.String""/>
				<Property Name=""Type"" Type=""Edm.String""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""User"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Group"" Type=""Example.Database.Group"" Nullable=""false""/>
				<NavigationProperty Name=""Application"" Type=""Example.Database.Application"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""EntityFieldRule"">
				<Key>
					<PropertyRef Name=""EntityFieldRuleId""/>
				</Key>
				<Property Name=""EntityFieldRuleId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Module"" Type=""Edm.String""/>
				<Property Name=""Data"" Type=""Edm.String""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""EntityField"" Type=""Example.Database.EntityField"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""SystemLog"">
				<Key>
					<PropertyRef Name=""SystemLogId""/>
				</Key>
				<Property Name=""SystemLogId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ItemId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Type"" Type=""Edm.String""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<Property Name=""Status"" Type=""Edm.String""/>
				<Property Name=""EntityName"" Type=""Edm.String""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Entity"" Type=""Example.Database.Entity"" Nullable=""false""/>
				<NavigationProperty Name=""SystemLogItems"" Type=""Collection(Example.Database.SystemLogItem)""/>
			</EntityType>
			<EntityType Name=""SystemLogItem"">
				<Key>
					<PropertyRef Name=""SystemLogItemId""/>
				</Key>
				<Property Name=""SystemLogItemId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""TimeStamp"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Type"" Type=""Edm.String""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""SystemLog"" Type=""Example.Database.SystemLog"" Nullable=""false""/>
				<NavigationProperty Name=""Exception"" Type=""Example.Database.Exception"" Nullable=""false""/>
				<NavigationProperty Name=""File"" Type=""Example.Database.File"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""Font"">
				<Key>
					<PropertyRef Name=""FontId""/>
				</Key>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<Property Name=""FontId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<NavigationProperty Name=""File"" Type=""Example.Database.File"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""CheckListType"">
				<Key>
					<PropertyRef Name=""CheckListTypeId""/>
				</Key>
				<Property Name=""CheckListTypeId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<Property Name=""ParentName"" Type=""Edm.String""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ItemId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""EntityName"" Type=""Edm.String""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Entity"" Type=""Example.Database.Entity"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""CheckListTypeItems"" Type=""Collection(Example.Database.CheckListTypeItem)""/>
				<NavigationProperty Name=""CheckLists"" Type=""Collection(Example.Database.CheckList)""/>
			</EntityType>
			<EntityType Name=""CheckListTypeItem"">
				<Key>
					<PropertyRef Name=""CheckListTypeItemId""/>
				</Key>
				<Property Name=""CheckListTypeItemId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<NavigationProperty Name=""CheckListType"" Type=""Example.Database.CheckListType"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""CheckListItems"" Type=""Collection(Example.Database.CheckListItem)""/>
			</EntityType>
			<EntityType Name=""CheckList"">
				<Key>
					<PropertyRef Name=""CheckListId""/>
				</Key>
				<Property Name=""CheckListId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""ParentName"" Type=""Edm.String""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ItemId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""EntityName"" Type=""Edm.String""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<NavigationProperty Name=""CheckListType"" Type=""Example.Database.CheckListType"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Entity"" Type=""Example.Database.Entity"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""CheckListItems"" Type=""Collection(Example.Database.CheckListItem)""/>
			</EntityType>
			<EntityType Name=""CheckListItem"">
				<Key>
					<PropertyRef Name=""CheckListItemId""/>
				</Key>
				<Property Name=""CheckListItemId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Checked"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<NavigationProperty Name=""CheckListTypeItem"" Type=""Example.Database.CheckListTypeItem"" Nullable=""false""/>
				<NavigationProperty Name=""CheckList"" Type=""Example.Database.CheckList"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""LocationItem"">
				<Key>
					<PropertyRef Name=""LocationItemId""/>
				</Key>
				<Property Name=""LocationItemId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Status"" Type=""Edm.String""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ItemId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""EntityName"" Type=""Edm.String""/>
				<NavigationProperty Name=""Location"" Type=""Example.Database.Location"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Entity"" Type=""Example.Database.Entity"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""LocationItemType"">
				<Key>
					<PropertyRef Name=""LocationItemTypeId""/>
				</Key>
				<Property Name=""LocationItemTypeId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ItemId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""EntityName"" Type=""Edm.String""/>
				<NavigationProperty Name=""LocationType"" Type=""Example.Database.LocationType"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Entity"" Type=""Example.Database.Entity"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""Installation"">
				<Key>
					<PropertyRef Name=""InstallationId""/>
				</Key>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Version"" Type=""Edm.String""/>
				<Property Name=""Configuration"" Type=""Edm.String""/>
				<Property Name=""Branch"" Type=""Edm.String""/>
				<Property Name=""ConfigurationName"" Type=""Edm.String""/>
				<Property Name=""Status"" Type=""Edm.String""/>
				<Property Name=""GitHash"" Type=""Edm.String""/>
				<Property Name=""StatusReason"" Type=""Edm.String""/>
				<Property Name=""FinishedOn"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""InstallationId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<NavigationProperty Name=""OperationalQualificationDocument"" Type=""Example.Database.Document"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Group"" Type=""Example.Database.Group"" Nullable=""false""/>
				<NavigationProperty Name=""InstallationLogs"" Type=""Collection(Example.Database.InstallationLog)""/>
				<NavigationProperty Name=""InstallationPackages"" Type=""Collection(Example.Database.InstallationPackage)""/>
			</EntityType>
			<EntityType Name=""InstallationLog"">
				<Key>
					<PropertyRef Name=""InstallationLogId""/>
				</Key>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Pcakage"" Type=""Edm.String""/>
				<Property Name=""Status"" Type=""Edm.String""/>
				<Property Name=""StatusReason"" Type=""Edm.String""/>
				<Property Name=""InstallationLogId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<NavigationProperty Name=""Installation"" Type=""Example.Database.Installation"" Nullable=""false""/>
				<NavigationProperty Name=""ConfigurationInstance"" Type=""Example.Database.ConfigurationInstance"" Nullable=""false""/>
				<NavigationProperty Name=""Session"" Type=""Example.Database.Session"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Group"" Type=""Example.Database.Group"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""InstallationPackage"">
				<Key>
					<PropertyRef Name=""InstallationPackageId""/>
				</Key>
				<Property Name=""InstallationPackageId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""PackageId"" Type=""Edm.String""/>
				<Property Name=""PackageVersion"" Type=""Edm.String""/>
				<Property Name=""Type"" Type=""Edm.String""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<Property Name=""Tags"" Type=""Edm.String""/>
				<Property Name=""BuildVersion"" Type=""Edm.String""/>
				<Property Name=""Branch"" Type=""Edm.String""/>
				<Property Name=""BuildConfiguration"" Type=""Edm.String""/>
				<Property Name=""MetaData"" Type=""Edm.String""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<NavigationProperty Name=""File"" Type=""Example.Database.File"" Nullable=""false""/>
				<NavigationProperty Name=""Installation"" Type=""Example.Database.Installation"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""ConfigurationSystem"">
				<Key>
					<PropertyRef Name=""ConfigurationSystemId""/>
				</Key>
				<Property Name=""ConfigurationSystemId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<Property Name=""SystemType"" Type=""Edm.String""/>
				<Property Name=""Serial"" Type=""Edm.String""/>
				<Property Name=""Schema"" Type=""Edm.String""/>
				<Property Name=""ApplicationSecret"" Type=""Edm.String""/>
				<Property Name=""Salt"" Type=""Edm.String""/>
				<Property Name=""FileStorage"" Type=""Edm.String""/>
				<Property Name=""DataDocumentStorage"" Type=""Edm.String""/>
				<Property Name=""FileRoot"" Type=""Edm.String""/>
				<Property Name=""MembershipProvider"" Type=""Edm.String""/>
				<Property Name=""MembershipProviderDomain"" Type=""Edm.String""/>
				<Property Name=""AdminAccount"" Type=""Edm.String""/>
				<Property Name=""StartCms"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DocumentationLinkFormat"" Type=""Edm.String""/>
				<Property Name=""AllowRememberMe"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""ConfigurationName"" Type=""Edm.String""/>
				<Property Name=""ApplicationCulture"" Type=""Edm.String""/>
				<Property Name=""LockTimeOut"" Type=""Edm.Int64""/>
				<Property Name=""MaximumPasswordLength"" Type=""Edm.Int64""/>
				<Property Name=""MinimumPasswordLength"" Type=""Edm.Int64""/>
				<Property Name=""PasswordExpireDays"" Type=""Edm.Int64""/>
				<Property Name=""RedirectToLoginPage"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""Application"" Type=""Edm.String""/>
				<Property Name=""ClientModules"" Type=""Edm.String""/>
				<Property Name=""DeviceTaskModules"" Type=""Edm.String""/>
				<Property Name=""InstrumentModules"" Type=""Edm.String""/>
				<Property Name=""ServiceModules"" Type=""Edm.String""/>
				<Property Name=""WebModules"" Type=""Edm.String""/>
				<Property Name=""IsolationLevel"" Type=""Edm.String""/>
				<Property Name=""ConfigurationEditorResolver"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""RegisterCustomComponents"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""SupportedLanguages"" Type=""Edm.String""/>
				<Property Name=""WorkFactor"" Type=""Edm.Int64""/>
				<Property Name=""Policy"" Type=""Edm.String""/>
				<Property Name=""BackupErrorRecipients"" Type=""Edm.String""/>
				<Property Name=""Suffix"" Type=""Edm.String""/>
				<Property Name=""ValidatedNorm"" Type=""Edm.String""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Group"" Type=""Example.Database.Group"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ConfigurationInstances"" Type=""Collection(Example.Database.ConfigurationInstance)""/>
				<NavigationProperty Name=""ConfigurationSystemLicenses"" Type=""Collection(Example.Database.ConfigurationSystemLicense)""/>
			</EntityType>
			<EntityType Name=""ConfigurationInstance"">
				<Key>
					<PropertyRef Name=""ConfigurationInstanceId""/>
				</Key>
				<Property Name=""ConfigurationInstanceId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""DatabaseConnection"" Type=""Edm.String""/>
				<Property Name=""FileRoot"" Type=""Edm.String""/>
				<Property Name=""FileStorage"" Type=""Edm.String""/>
				<Property Name=""DataDocumentStorage"" Type=""Edm.String""/>
				<Property Name=""FileStorageConnection"" Type=""Edm.String""/>
				<Property Name=""MembershipProvider"" Type=""Edm.String""/>
				<Property Name=""MembershipProviderDomain"" Type=""Edm.String""/>
				<Property Name=""OfficeUpdateUrl"" Type=""Edm.String""/>
				<Property Name=""WebReportFileStorageRoot"" Type=""Edm.String""/>
				<Property Name=""ServiceDeviceName"" Type=""Edm.String""/>
				<Property Name=""DeployTarget"" Type=""Edm.String""/>
				<Property Name=""WebsiteUrl"" Type=""Edm.String""/>
				<Property Name=""Minify"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DebugScripts"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""CompiledScripts"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""SignalServer"" Type=""Edm.String""/>
				<Property Name=""SmtpServer"" Type=""Edm.String""/>
				<Property Name=""SmtpPort"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""DeploymentUrl"" Type=""Edm.String""/>
				<Property Name=""WebApplicationLocation"" Type=""Edm.String""/>
				<Property Name=""ServiceTasks"" Type=""Edm.String""/>
				<Property Name=""Monitor"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""Status"" Type=""Edm.String""/>
				<Property Name=""ErrorTime"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""ErrorMessage"" Type=""Edm.String""/>
				<Property Name=""TimeStamp"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""HardDiskStatus"" Type=""Edm.String""/>
				<Property Name=""ProcessorStatus"" Type=""Edm.String""/>
				<Property Name=""MemoryStatus"" Type=""Edm.String""/>
				<Property Name=""WebsiteStatus"" Type=""Edm.String""/>
				<Property Name=""BackupStorageConnection"" Type=""Edm.String""/>
				<Property Name=""BackupStatus"" Type=""Edm.String""/>
				<Property Name=""BackupDate"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""BackupLogStatus"" Type=""Edm.String""/>
				<Property Name=""BackupLogDate"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""BackupPolicy"" Type=""Edm.String""/>
				<Property Name=""RecipientListCombinationBehaviour"" Type=""Edm.String""/>
				<Property Name=""BackupErrorRecipients"" Type=""Edm.String""/>
				<Property Name=""DocumentVersionUpdateRule"" Type=""Edm.String""/>
				<Property Name=""AllowOQ"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DatabaseValidationResult"" Type=""Edm.String""/>
				<Property Name=""LastDatabaseValidation"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""LastReminder"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""ValidationIssueDate"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""ValidationTimeout"" Type=""Edm.Int64""/>
				<Property Name=""BindSessionToIP"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""Repository"" Type=""Edm.String""/>
				<Property Name=""Subscription"" Type=""Edm.String""/>
				<Property Name=""Region"" Type=""Edm.String""/>
				<Property Name=""DatabaseServer"" Type=""Edm.String""/>
				<Property Name=""ApplicationServer"" Type=""Edm.String""/>
				<Property Name=""PricingTier"" Type=""Edm.String""/>
				<Property Name=""PublicUrl"" Type=""Edm.String""/>
				<Property Name=""PortalPolicy"" Type=""Edm.String""/>
				<Property Name=""MonitoringConnection"" Type=""Edm.String""/>
				<Property Name=""DeploymentStatus"" Type=""Edm.String""/>
				<Property Name=""UpdatePolicy"" Type=""Edm.String""/>
				<Property Name=""Version"" Type=""Edm.String""/>
				<Property Name=""InstanceType"" Type=""Edm.String""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<NavigationProperty Name=""ConfigurationSystem"" Type=""Example.Database.ConfigurationSystem"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Group"" Type=""Example.Database.Group"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""InstallationLogs"" Type=""Collection(Example.Database.InstallationLog)""/>
				<NavigationProperty Name=""ConfigurationSystemMonitorings"" Type=""Collection(Example.Database.ConfigurationSystemMonitoring)""/>
				<NavigationProperty Name=""Credentials"" Type=""Collection(Example.Database.Credential)""/>
			</EntityType>
			<EntityType Name=""ConfigurationSystemMonitoring"">
				<Key>
					<PropertyRef Name=""ConfigurationSystemMonitoringId""/>
				</Key>
				<Property Name=""ConfigurationSystemMonitoringId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Type"" Type=""Edm.String""/>
				<Property Name=""NumericValue"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Message"" Type=""Edm.String""/>
				<Property Name=""Status"" Type=""Edm.String""/>
				<Property Name=""TimeStamp"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<NavigationProperty Name=""ConfigurationInstance"" Type=""Example.Database.ConfigurationInstance"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""Credential"">
				<Key>
					<PropertyRef Name=""CredentialId""/>
				</Key>
				<Property Name=""CredentialId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Guid"" Type=""Edm.String""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""GroupName"" Type=""Edm.String""/>
				<Property Name=""UserName"" Type=""Edm.String""/>
				<Property Name=""Uri"" Type=""Edm.String""/>
				<Property Name=""TokenUri"" Type=""Edm.String""/>
				<Property Name=""ExpiresOn"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""SpecialCharacters"" Type=""Edm.Boolean""/>
				<Property Name=""Length"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<Property Name=""ClientName"" Type=""Edm.String""/>
				<Property Name=""TenantName"" Type=""Edm.String""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""CredentialType"" Type=""Edm.String""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ConfigurationInstance"" Type=""Example.Database.ConfigurationInstance"" Nullable=""false""/>
				<NavigationProperty Name=""User"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Session"" Type=""Example.Database.Session"" Nullable=""false""/>
				<NavigationProperty Name=""Operation"" Type=""Example.Database.Operation"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Group"" Type=""Example.Database.Group"" Nullable=""false""/>
				<NavigationProperty Name=""DeviceTasks"" Type=""Collection(Example.Database.DeviceTask)""/>
			</EntityType>
			<EntityType Name=""JobPosition"">
				<Key>
					<PropertyRef Name=""JobPositionId""/>
				</Key>
				<Property Name=""JobPositionId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""JobPositionTrainings"" Type=""Collection(Example.Database.JobPositionTraining)""/>
				<NavigationProperty Name=""UserJobPositions"" Type=""Collection(Example.Database.UserJobPosition)""/>
			</EntityType>
			<EntityType Name=""Training"">
				<Key>
					<PropertyRef Name=""TrainingId""/>
				</Key>
				<Property Name=""TrainingId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""HasNotes"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""JobPositionTrainings"" Type=""Collection(Example.Database.JobPositionTraining)""/>
				<NavigationProperty Name=""UserTrainings"" Type=""Collection(Example.Database.UserTraining)""/>
			</EntityType>
			<EntityType Name=""JobPositionTraining"">
				<Key>
					<PropertyRef Name=""JobPositionTrainingId""/>
				</Key>
				<Property Name=""JobPositionTrainingId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""TimeStamp"" Type=""Edm.Binary""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""JobPosition"" Type=""Example.Database.JobPosition"" Nullable=""false""/>
				<NavigationProperty Name=""Training"" Type=""Example.Database.Training"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""UserTraining"">
				<Key>
					<PropertyRef Name=""UserTrainingId""/>
				</Key>
				<Property Name=""UserTrainingId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ValidFrom"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""ValidTo"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""Expired"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""Renewed"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""TrainingDate"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""Trained"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""TrainingLocation"" Type=""Edm.String""/>
				<Property Name=""AchivedResult"" Type=""Edm.String""/>
				<Property Name=""Comments"" Type=""Edm.String""/>
				<Property Name=""HasNotes"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""User"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Training"" Type=""Example.Database.Training"" Nullable=""false""/>
				<NavigationProperty Name=""File"" Type=""Example.Database.File"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""CommentsText"" Type=""Example.Database.Text"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""UserJobPosition"">
				<Key>
					<PropertyRef Name=""UserJobPositionId""/>
				</Key>
				<Property Name=""UserJobPositionId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""TimeStamp"" Type=""Edm.Binary""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""User"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""JobPosition"" Type=""Example.Database.JobPosition"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""SchemaFile"">
				<Key>
					<PropertyRef Name=""SchemaFileId""/>
				</Key>
				<Property Name=""SchemaFileId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Definition"" Type=""Edm.String""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""RecordLock"">
				<Key>
					<PropertyRef Name=""RecordLockId""/>
				</Key>
				<Property Name=""RecordLockId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""ValidTo"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ItemId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Value"" Type=""Edm.String""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<NavigationProperty Name=""Entity"" Type=""Example.Database.Entity"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""Subscription"">
				<Key>
					<PropertyRef Name=""SubscriptionId""/>
				</Key>
				<Property Name=""SubscriptionId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Active"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<NavigationProperty Name=""Device"" Type=""Example.Database.Device"" Nullable=""false""/>
				<NavigationProperty Name=""Session"" Type=""Example.Database.Session"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""SubscriptionSubjects"" Type=""Collection(Example.Database.SubscriptionSubject)""/>
				<NavigationProperty Name=""SubscriptionNotifications"" Type=""Collection(Example.Database.SubscriptionNotification)""/>
			</EntityType>
			<EntityType Name=""SubscriptionSubject"">
				<Key>
					<PropertyRef Name=""SubscriptionSubjectId""/>
				</Key>
				<Property Name=""SubscriptionSubjectId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<NavigationProperty Name=""Subscription"" Type=""Example.Database.Subscription"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""SubscriptionManager"">
				<Key>
					<PropertyRef Name=""SubscriptionManagerId""/>
				</Key>
				<Property Name=""SubscriptionManagerId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""DialogId"" Type=""Edm.String""/>
				<Property Name=""Type"" Type=""Edm.String""/>
				<Property Name=""Status"" Type=""Edm.String""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""SubscriptionNotification"">
				<Key>
					<PropertyRef Name=""SubscriptionNotificationId""/>
				</Key>
				<Property Name=""SubscriptionNotificationId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Type"" Type=""Edm.String""/>
				<Property Name=""Value"" Type=""Edm.String""/>
				<Property Name=""Status"" Type=""Edm.String""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<NavigationProperty Name=""Subscription"" Type=""Example.Database.Subscription"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""LicenseUser"">
				<Key>
					<PropertyRef Name=""LicenseUserId""/>
				</Key>
				<Property Name=""LicenseUserId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<NavigationProperty Name=""User"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ConfigurationSystemLicense"" Type=""Example.Database.ConfigurationSystemLicense"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""ConfigurationSystemLicense"">
				<Key>
					<PropertyRef Name=""ConfigurationSystemLicenseId""/>
				</Key>
				<Property Name=""ConfigurationSystemLicenseId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Type"" Type=""Edm.String""/>
				<Property Name=""Amount"" Type=""Edm.Int64""/>
				<Property Name=""Signature"" Type=""Edm.String""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<NavigationProperty Name=""ConfigurationSystem"" Type=""Example.Database.ConfigurationSystem"" Nullable=""false""/>
				<NavigationProperty Name=""PurchaseOrder"" Type=""Example.Database.Document"" Nullable=""false""/>
				<NavigationProperty Name=""LicenseAgreement"" Type=""Example.Database.Document"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""LicenseUsers"" Type=""Collection(Example.Database.LicenseUser)""/>
			</EntityType>
			<EntityType Name=""ProcessStationInstrument"">
				<Key>
					<PropertyRef Name=""ProcessStationInstrumentId""/>
				</Key>
				<Property Name=""ProcessStationInstrumentId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""ShowConnectionStatus"" Type=""Edm.Boolean"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ProcessStation"" Type=""Example.Database.ProcessStation"" Nullable=""false""/>
				<NavigationProperty Name=""Instrument"" Type=""Example.Database.Instrument"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""Client"">
				<Key>
					<PropertyRef Name=""ClientId""/>
				</Key>
				<Property Name=""ClientId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Code"" Type=""Edm.String""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<Property Name=""Type"" Type=""Edm.String""/>
				<Property Name=""Address"" Type=""Edm.String""/>
				<Property Name=""City"" Type=""Edm.String""/>
				<Property Name=""Country"" Type=""Edm.String""/>
				<Property Name=""PostalCode"" Type=""Edm.String""/>
				<Property Name=""Phone"" Type=""Edm.String""/>
				<Property Name=""Mobile"" Type=""Edm.String""/>
				<Property Name=""Fax"" Type=""Edm.String""/>
				<Property Name=""EMail"" Type=""Edm.String""/>
				<Property Name=""InvoiceLanguage"" Type=""Edm.String""/>
				<Property Name=""CollectiveInvoice"" Type=""Edm.String""/>
				<Property Name=""Discount"" Type=""Edm.Double""/>
				<Property Name=""HasNotes"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""TimeStamp"" Type=""Edm.Binary""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""PriceList"" Type=""Example.Database.PriceList"" Nullable=""false""/>
				<NavigationProperty Name=""InvoiceReportType"" Type=""Example.Database.ReportType"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Group"" Type=""Example.Database.Group"" Nullable=""false""/>
				<NavigationProperty Name=""Documents"" Type=""Collection(Example.Database.Document)""/>
				<NavigationProperty Name=""ClientContacts"" Type=""Collection(Example.Database.ClientContact)""/>
				<NavigationProperty Name=""Orders"" Type=""Collection(Example.Database.Order)""/>
				<NavigationProperty Name=""ClientReportTypes"" Type=""Collection(Example.Database.ClientReportType)""/>
				<NavigationProperty Name=""OrderForms"" Type=""Collection(Example.Database.OrderForm)""/>
				<NavigationProperty Name=""ClientProduct"" Type=""Collection(Example.Database.ClientProduct)""/>
				<NavigationProperty Name=""PriceLists"" Type=""Collection(Example.Database.PriceList)""/>
				<NavigationProperty Name=""ClientUsers"" Type=""Collection(Example.Database.ClientUser)""/>
				<NavigationProperty Name=""Invoices"" Type=""Collection(Example.Database.Invoice)""/>
				<NavigationProperty Name=""Quotations"" Type=""Collection(Example.Database.Quotation)""/>
			</EntityType>
			<EntityType Name=""ClientContact"">
				<Key>
					<PropertyRef Name=""ClientContactId""/>
				</Key>
				<Property Name=""ClientContactId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Code"" Type=""Edm.String""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<Property Name=""Address"" Type=""Edm.String""/>
				<Property Name=""City"" Type=""Edm.String""/>
				<Property Name=""Country"" Type=""Edm.String""/>
				<Property Name=""PostalCode"" Type=""Edm.String""/>
				<Property Name=""Phone"" Type=""Edm.String""/>
				<Property Name=""Mobile"" Type=""Edm.String""/>
				<Property Name=""Fax"" Type=""Edm.String""/>
				<Property Name=""EMail"" Type=""Edm.String""/>
				<Property Name=""ReceiveSampleDisposalReport"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""Active"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""HasNotes"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Client"" Type=""Example.Database.Client"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Orders"" Type=""Collection(Example.Database.Order)""/>
				<NavigationProperty Name=""OrderReportDestinations"" Type=""Collection(Example.Database.OrderReportDestination)""/>
				<NavigationProperty Name=""OrderFormReportDestinations"" Type=""Collection(Example.Database.OrderFormReportDestination)""/>
			</EntityType>
			<EntityType Name=""Project"">
				<Key>
					<PropertyRef Name=""ProjectId""/>
				</Key>
				<Property Name=""ProjectId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<Property Name=""Status"" Type=""Edm.String""/>
				<Property Name=""StartDate"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""EndDate"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""HasNotes"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""Authorised"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""AuthorisedOn"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""Cancelled"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""CancelledOn"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""BKDConclusion"" Type=""Edm.String""/>
				<Property Name=""BKDDescription"" Type=""Edm.String""/>
				<Property Name=""BKDGroup"" Type=""Edm.String""/>
				<Property Name=""BKDPriority"" Type=""Edm.String""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Document"" Type=""Example.Database.File"" Nullable=""false""/>
				<NavigationProperty Name=""Location"" Type=""Example.Database.Location"" Nullable=""false""/>
				<NavigationProperty Name=""ProjectLeader"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ProjectType"" Type=""Example.Database.ProjectType"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""AuthorisedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""CancelledBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Group"" Type=""Example.Database.Group"" Nullable=""false""/>
				<NavigationProperty Name=""Experiments"" Type=""Collection(Example.Database.Experiment)""/>
				<NavigationProperty Name=""ProjectMembers"" Type=""Collection(Example.Database.ProjectMember)""/>
				<NavigationProperty Name=""BKDSamples"" Type=""Collection(Example.Database.Sample)""/>
				<NavigationProperty Name=""Orders"" Type=""Collection(Example.Database.Order)""/>
				<NavigationProperty Name=""ProjectDocuments"" Type=""Collection(Example.Database.ProjectDocument)""/>
				<NavigationProperty Name=""Materials"" Type=""Collection(Example.Database.Material)""/>
				<NavigationProperty Name=""Batches"" Type=""Collection(Example.Database.Batch)""/>
				<NavigationProperty Name=""Products"" Type=""Collection(Example.Database.Product)""/>
				<NavigationProperty Name=""ProjectTasks"" Type=""Collection(Example.Database.ProjectTask)""/>
				<NavigationProperty Name=""BKDProjectRules"" Type=""Collection(Example.Database.BKDProjectRule)""/>
			</EntityType>
			<EntityType Name=""ExperimentType"">
				<Key>
					<PropertyRef Name=""ExperimentTypeId""/>
				</Key>
				<Property Name=""ExperimentTypeId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<Property Name=""NameFormat"" Type=""Edm.String""/>
				<Property Name=""Configuration"" Type=""Edm.String""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Group"" Type=""Example.Database.Group"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Experiments"" Type=""Collection(Example.Database.Experiment)""/>
			</EntityType>
			<EntityType Name=""ProjectType"">
				<Key>
					<PropertyRef Name=""ProjectTypeId""/>
				</Key>
				<Property Name=""ProjectTypeId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<Property Name=""NameFormat"" Type=""Edm.String""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Group"" Type=""Example.Database.Group"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Projects"" Type=""Collection(Example.Database.Project)""/>
			</EntityType>
			<EntityType Name=""Experiment"">
				<Key>
					<PropertyRef Name=""ExperimentId""/>
				</Key>
				<Property Name=""ExperimentId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<Property Name=""Status"" Type=""Edm.String""/>
				<Property Name=""StartDate"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""EndDate"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""Configuration"" Type=""Edm.String""/>
				<Property Name=""HasNotes"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""Authorised"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""AuthorisedOn"" Type=""Edm.DateTimeOffset""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Location"" Type=""Example.Database.Location"" Nullable=""false""/>
				<NavigationProperty Name=""Protocol"" Type=""Example.Database.Protocol"" Nullable=""false""/>
				<NavigationProperty Name=""Document"" Type=""Example.Database.File"" Nullable=""false""/>
				<NavigationProperty Name=""ProjectLeader"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Project"" Type=""Example.Database.Project"" Nullable=""false""/>
				<NavigationProperty Name=""ExperimentType"" Type=""Example.Database.ExperimentType"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""AuthorisedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Group"" Type=""Example.Database.Group"" Nullable=""false""/>
				<NavigationProperty Name=""ProjectMembers"" Type=""Collection(Example.Database.ProjectMember)""/>
				<NavigationProperty Name=""ExperimentMembers"" Type=""Collection(Example.Database.ExperimentMember)""/>
				<NavigationProperty Name=""ScheduleItems"" Type=""Collection(Example.Database.ScheduleItem)""/>
				<NavigationProperty Name=""Samples"" Type=""Collection(Example.Database.Sample)""/>
				<NavigationProperty Name=""Orders"" Type=""Collection(Example.Database.Order)""/>
				<NavigationProperty Name=""Reactions"" Type=""Collection(Example.Database.Reaction)""/>
				<NavigationProperty Name=""InstrumentFiles"" Type=""Collection(Example.Database.InstrumentFile)""/>
				<NavigationProperty Name=""Quotations"" Type=""Collection(Example.Database.Quotation)""/>
				<NavigationProperty Name=""QuotationLocations"" Type=""Collection(Example.Database.QuotationLocation)""/>
			</EntityType>
			<EntityType Name=""ProjectMember"">
				<Key>
					<PropertyRef Name=""ProjectMemberId""/>
				</Key>
				<Property Name=""ProjectMemberId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""User"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Experiment"" Type=""Example.Database.Experiment"" Nullable=""false""/>
				<NavigationProperty Name=""Project"" Type=""Example.Database.Project"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""ExperimentMember"">
				<Key>
					<PropertyRef Name=""ExperimentMemberId""/>
				</Key>
				<Property Name=""ExperimentMemberId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""User"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Experiment"" Type=""Example.Database.Experiment"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""SlotType"">
				<Key>
					<PropertyRef Name=""SlotTypeId""/>
				</Key>
				<Property Name=""SlotTypeId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<Property Name=""Capacity"" Type=""Edm.Int64""/>
				<Property Name=""SlotItemNameExpression"" Type=""Edm.String""/>
				<Property Name=""SlotNameExpression"" Type=""Edm.String""/>
				<Property Name=""Color"" Type=""Edm.String""/>
				<Property Name=""Icon"" Type=""Edm.String""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Entity"" Type=""Example.Database.Entity"" Nullable=""false""/>
				<NavigationProperty Name=""Slots"" Type=""Collection(Example.Database.Slot)""/>
				<NavigationProperty Name=""SlotTypeProperties"" Type=""Collection(Example.Database.SlotTypeProperty)""/>
				<NavigationProperty Name=""ScheduleTypeSlotTypes"" Type=""Collection(Example.Database.ScheduleTypeSlotType)""/>
			</EntityType>
			<EntityType Name=""Slot"">
				<Key>
					<PropertyRef Name=""SlotId""/>
				</Key>
				<Property Name=""SlotId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Date"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""EndDate"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""HashCode"" Type=""Edm.Int64""/>
				<Property Name=""OrderNumber"" Type=""Edm.Int64""/>
				<Property Name=""Status"" Type=""Edm.String""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Remark"" Type=""Edm.String""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""SlotType"" Type=""Example.Database.SlotType"" Nullable=""false""/>
				<NavigationProperty Name=""ScheduleType"" Type=""Example.Database.ScheduleType"" Nullable=""false""/>
				<NavigationProperty Name=""Schedule"" Type=""Example.Database.Schedule"" Nullable=""false""/>
				<NavigationProperty Name=""SlotItems"" Type=""Collection(Example.Database.SlotItem)""/>
				<NavigationProperty Name=""SlotProperties"" Type=""Collection(Example.Database.SlotProperty)""/>
				<NavigationProperty Name=""Racks"" Type=""Collection(Example.Database.Rack)""/>
				<NavigationProperty Name=""Sequences"" Type=""Collection(Example.Database.Sequence)""/>
			</EntityType>
			<EntityType Name=""ScheduleType"">
				<Key>
					<PropertyRef Name=""ScheduleTypeId""/>
				</Key>
				<Property Name=""ScheduleTypeId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<Property Name=""Type"" Type=""Edm.String""/>
				<Property Name=""OrderNumber"" Type=""Edm.Int64""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Schedule"" Type=""Example.Database.Schedule"" Nullable=""false""/>
				<NavigationProperty Name=""Location"" Type=""Example.Database.Location"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Slots"" Type=""Collection(Example.Database.Slot)""/>
				<NavigationProperty Name=""ScheduleTypeSlotTypes"" Type=""Collection(Example.Database.ScheduleTypeSlotType)""/>
				<NavigationProperty Name=""ScheduleTypeQualifications"" Type=""Collection(Example.Database.ScheduleTypeQualification)""/>
				<NavigationProperty Name=""ScheduleRules"" Type=""Collection(Example.Database.ScheduleRule)""/>
				<NavigationProperty Name=""ScheduleRuleSets"" Type=""Collection(Example.Database.ScheduleRuleSet)""/>
			</EntityType>
			<EntityType Name=""Schedule"">
				<Key>
					<PropertyRef Name=""ScheduleId""/>
				</Key>
				<Property Name=""ScheduleId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<Property Name=""ScheduleOnFriday"" Type=""Edm.Boolean"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""LocationsByDefaultSchedule"" Type=""Collection(Example.Database.Location)""/>
				<NavigationProperty Name=""Slots"" Type=""Collection(Example.Database.Slot)""/>
				<NavigationProperty Name=""ScheduleTypes"" Type=""Collection(Example.Database.ScheduleType)""/>
				<NavigationProperty Name=""ScheduleItems"" Type=""Collection(Example.Database.ScheduleItem)""/>
				<NavigationProperty Name=""QuotationLocations"" Type=""Collection(Example.Database.QuotationLocation)""/>
			</EntityType>
			<EntityType Name=""Test"">
				<Key>
					<PropertyRef Name=""TestId""/>
				</Key>
				<Property Name=""TestId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Replication"" Type=""Edm.Int64""/>
				<Property Name=""ScheduleDelay"" Type=""Edm.Int64""/>
				<Property Name=""Duration"" Type=""Edm.Int64""/>
				<Property Name=""Status"" Type=""Edm.String""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<Property Name=""DateResultsRequired"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""Accredited"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""SpecificationStatus"" Type=""Edm.String""/>
				<Property Name=""RequestedDilutions"" Type=""Edm.String""/>
				<Property Name=""Requested"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""Source"" Type=""Edm.String""/>
				<Property Name=""ScheduleStatus"" Type=""Edm.String""/>
				<Property Name=""Scheduled"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""TimeStamp"" Type=""Edm.Binary""/>
				<Property Name=""HasNotes"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""Cancelled"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""CancelledOn"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""Completed"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""CompletedOn"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""Rejected"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""RejectedOn"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""BackgroundScheduleStatus"" Type=""Edm.String""/>
				<Property Name=""PartitionKey"" Type=""Edm.Int32""/>
				<Property Name=""BKDInvoiced"" Type=""Edm.Boolean""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Sample"" Type=""Example.Database.Sample"" Nullable=""false""/>
				<NavigationProperty Name=""PrimarySample"" Type=""Example.Database.Sample"" Nullable=""false""/>
				<NavigationProperty Name=""Rack"" Type=""Example.Database.Rack"" Nullable=""false""/>
				<NavigationProperty Name=""TestType"" Type=""Example.Database.TestType"" Nullable=""false""/>
				<NavigationProperty Name=""ParentTest"" Type=""Example.Database.Test"" Nullable=""false""/>
				<NavigationProperty Name=""RequestedTest"" Type=""Example.Database.Test"" Nullable=""false""/>
				<NavigationProperty Name=""Method"" Type=""Example.Database.Method"" Nullable=""false""/>
				<NavigationProperty Name=""Priority"" Type=""Example.Database.Priority"" Nullable=""false""/>
				<NavigationProperty Name=""Laboratory"" Type=""Example.Database.Laboratory"" Nullable=""false""/>
				<NavigationProperty Name=""Instrument"" Type=""Example.Database.Instrument"" Nullable=""false""/>
				<NavigationProperty Name=""CancelledBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""CompletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""RejectedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Group"" Type=""Example.Database.Group"" Nullable=""false""/>
				<NavigationProperty Name=""BKDRack"" Type=""Example.Database.Rack"" Nullable=""false""/>
				<NavigationProperty Name=""BKDInternalResponsible"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""BKDCreatedFromProjectRule"" Type=""Example.Database.BKDProjectRule"" Nullable=""false""/>
				<NavigationProperty Name=""TestsByParentTest"" Type=""Collection(Example.Database.Test)""/>
				<NavigationProperty Name=""TestsByRequestedTest"" Type=""Collection(Example.Database.Test)""/>
				<NavigationProperty Name=""Results"" Type=""Collection(Example.Database.Result)""/>
				<NavigationProperty Name=""SamplesByRequestedTest"" Type=""Collection(Example.Database.Sample)""/>
				<NavigationProperty Name=""BKDRacks"" Type=""Collection(Example.Database.Rack)""/>
				<NavigationProperty Name=""TestTasks"" Type=""Collection(Example.Database.TestTask)""/>
				<NavigationProperty Name=""SuiteTests"" Type=""Collection(Example.Database.SuiteTest)""/>
			</EntityType>
			<EntityType Name=""Unit"">
				<Key>
					<PropertyRef Name=""UnitId""/>
				</Key>
				<Property Name=""UnitId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<Property Name=""ToBase"" Type=""Edm.String""/>
				<Property Name=""FromBase"" Type=""Edm.String""/>
				<Property Name=""HasNotes"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""BaseUnit"" Type=""Example.Database.Unit"" Nullable=""false""/>
				<NavigationProperty Name=""Text"" Type=""Example.Database.Text"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""UnitsByBaseUnit"" Type=""Collection(Example.Database.Unit)""/>
				<NavigationProperty Name=""ResultTypes"" Type=""Collection(Example.Database.ResultType)""/>
				<NavigationProperty Name=""Results"" Type=""Collection(Example.Database.Result)""/>
				<NavigationProperty Name=""ResultsByBaseUnit"" Type=""Collection(Example.Database.Result)""/>
				<NavigationProperty Name=""SamplesByAmountUnit"" Type=""Collection(Example.Database.Sample)""/>
				<NavigationProperty Name=""Materials"" Type=""Collection(Example.Database.Material)""/>
				<NavigationProperty Name=""BatchFormulationTypes"" Type=""Collection(Example.Database.BatchFormulationType)""/>
				<NavigationProperty Name=""Batches"" Type=""Collection(Example.Database.Batch)""/>
				<NavigationProperty Name=""ProductUnits"" Type=""Collection(Example.Database.ProductUnit)""/>
				<NavigationProperty Name=""SpecificationItems"" Type=""Collection(Example.Database.SpecificationItem)""/>
				<NavigationProperty Name=""TestTypeQualityControlTriggersByResultUnit"" Type=""Collection(Example.Database.TestTypeQualityControlTrigger)""/>
				<NavigationProperty Name=""LaboratoryUnits"" Type=""Collection(Example.Database.LaboratoryUnit)""/>
				<NavigationProperty Name=""OrderTaskResults"" Type=""Collection(Example.Database.OrderTaskResult)""/>
			</EntityType>
			<EntityType Name=""ResultType"">
				<Key>
					<PropertyRef Name=""ResultTypeId""/>
				</Key>
				<Property Name=""ResultTypeId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Concentration"" Type=""Edm.Double""/>
				<Property Name=""DataType"" Type=""Edm.String""/>
				<Property Name=""Expression"" Type=""Edm.String""/>
				<Property Name=""OrderNumber"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Reported"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""Format"" Type=""Edm.String""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<Property Name=""Code"" Type=""Edm.String""/>
				<Property Name=""NameFormat"" Type=""Edm.String""/>
				<Property Name=""Calculated"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""MinimumValue"" Type=""Edm.Double""/>
				<Property Name=""MaximumValue"" Type=""Edm.Double""/>
				<Property Name=""AllowOnlyPhraseValues"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""Instruction"" Type=""Edm.String""/>
				<Property Name=""Configuration"" Type=""Edm.String""/>
				<Property Name=""Type"" Type=""Edm.String""/>
				<Property Name=""MultipleValues"" Type=""Edm.Boolean""/>
				<Property Name=""LLOQCalculationRule"" Type=""Edm.String""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Phrase"" Type=""Example.Database.Phrase"" Nullable=""false""/>
				<NavigationProperty Name=""Unit"" Type=""Example.Database.Unit"" Nullable=""false""/>
				<NavigationProperty Name=""Text"" Type=""Example.Database.Text"" Nullable=""false""/>
				<NavigationProperty Name=""Chemical"" Type=""Example.Database.Chemical"" Nullable=""false""/>
				<NavigationProperty Name=""Module"" Type=""Example.Database.Module"" Nullable=""false""/>
				<NavigationProperty Name=""Group"" Type=""Example.Database.Group"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""InstructionText"" Type=""Example.Database.Text"" Nullable=""false""/>
				<NavigationProperty Name=""Deviation"" Type=""Example.Database.Phrase"" Nullable=""false""/>
				<NavigationProperty Name=""Results"" Type=""Collection(Example.Database.Result)""/>
				<NavigationProperty Name=""ResultTypeArgumentsBySourceResultType"" Type=""Collection(Example.Database.ResultTypeArgument)""/>
				<NavigationProperty Name=""ResultTypeArguments"" Type=""Collection(Example.Database.ResultTypeArgument)""/>
				<NavigationProperty Name=""ResultTypeFields"" Type=""Collection(Example.Database.ResultTypeField)""/>
				<NavigationProperty Name=""TestTypeResultTypes"" Type=""Collection(Example.Database.TestTypeResultType)""/>
				<NavigationProperty Name=""ControlChartTypes"" Type=""Collection(Example.Database.ControlChartType)""/>
				<NavigationProperty Name=""InstrumentTypeResultTypes"" Type=""Collection(Example.Database.InstrumentTypeResultType)""/>
				<NavigationProperty Name=""SpecificationItems"" Type=""Collection(Example.Database.SpecificationItem)""/>
				<NavigationProperty Name=""SpecificationItemsByTargetResultType"" Type=""Collection(Example.Database.SpecificationItem)""/>
				<NavigationProperty Name=""SequenceTypeResultTypes"" Type=""Collection(Example.Database.SequenceTypeResultType)""/>
				<NavigationProperty Name=""ResultTypeEvents"" Type=""Collection(Example.Database.ResultTypeEvent)""/>
				<NavigationProperty Name=""AccreditationTestTypes"" Type=""Collection(Example.Database.AccreditationTestType)""/>
				<NavigationProperty Name=""TestTypeQualityControlTriggers"" Type=""Collection(Example.Database.TestTypeQualityControlTrigger)""/>
				<NavigationProperty Name=""LaboratoryResultTypes"" Type=""Collection(Example.Database.LaboratoryResultType)""/>
			</EntityType>
			<EntityType Name=""Result"">
				<Key>
					<PropertyRef Name=""ResultId""/>
				</Key>
				<Property Name=""ResultId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Status"" Type=""Edm.String""/>
				<Property Name=""Prefix"" Type=""Edm.String""/>
				<Property Name=""TextValue"" Type=""Edm.String""/>
				<Property Name=""EnteredValue"" Type=""Edm.String""/>
				<Property Name=""NumberValue"" Type=""Edm.Double""/>
				<Property Name=""BaseNumberValue"" Type=""Edm.Double""/>
				<Property Name=""DateValue"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""EnteredOn"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""Dilution"" Type=""Edm.Double""/>
				<Property Name=""SignificantFigures"" Type=""Edm.Int64""/>
				<Property Name=""Replication"" Type=""Edm.Int64""/>
				<Property Name=""Reported"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""RequireReview"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""LLOQ"" Type=""Edm.Double""/>
				<Property Name=""UseInCountCalculation"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""Cancelled"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""CancelledOn"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""HasNotes"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""HasAudits"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""IsModifiedResult"" Type=""Edm.Boolean""/>
				<Property Name=""TimeStamp"" Type=""Edm.Binary""/>
				<Property Name=""Rejected"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""RejectedOn"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""Authorised"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""AuthorisedOn"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""Reviewed"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""ReviewedOn"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""Completed"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""CompletedOn"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""PartitionKey"" Type=""Edm.Int32""/>
				<Property Name=""Deviation"" Type=""Edm.String""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Test"" Type=""Example.Database.Test"" Nullable=""false""/>
				<NavigationProperty Name=""ResultType"" Type=""Example.Database.ResultType"" Nullable=""false""/>
				<NavigationProperty Name=""File"" Type=""Example.Database.File"" Nullable=""false""/>
				<NavigationProperty Name=""Unit"" Type=""Example.Database.Unit"" Nullable=""false""/>
				<NavigationProperty Name=""BaseUnit"" Type=""Example.Database.Unit"" Nullable=""false""/>
				<NavigationProperty Name=""EnteredBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Instrument"" Type=""Example.Database.Instrument"" Nullable=""false""/>
				<NavigationProperty Name=""CancelledBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""RejectedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""AuthorisedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ReviewedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""CompletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Group"" Type=""Example.Database.Group"" Nullable=""false""/>
				<NavigationProperty Name=""ControlChartResults"" Type=""Collection(Example.Database.ControlChartResult)""/>
				<NavigationProperty Name=""SequenceSampleResults"" Type=""Collection(Example.Database.SequenceSampleResult)""/>
				<NavigationProperty Name=""SpecificationResults"" Type=""Collection(Example.Database.SpecificationResult)""/>
			</EntityType>
			<EntityType Name=""ResultTypeArgument"">
				<Key>
					<PropertyRef Name=""ResultTypeArgumentId""/>
				</Key>
				<Property Name=""ResultTypeArgumentId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Type"" Type=""Edm.String""/>
				<Property Name=""Scope"" Type=""Edm.String""/>
				<Property Name=""NumberValue"" Type=""Edm.Double""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""SourceResultType"" Type=""Example.Database.ResultType"" Nullable=""false""/>
				<NavigationProperty Name=""ResultType"" Type=""Example.Database.ResultType"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""SlotTypeProperty"">
				<Key>
					<PropertyRef Name=""SlotTypePropertyId""/>
				</Key>
				<Property Name=""SlotTypePropertyId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Value"" Type=""Edm.String""/>
				<Property Name=""OrderNumber"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""DisplayLength"" Type=""Edm.Int64""/>
				<Property Name=""Display"" Type=""Edm.Boolean""/>
				<Property Name=""IsSlotProperty"" Type=""Edm.Boolean""/>
				<Property Name=""SlotExpression"" Type=""Edm.String""/>
				<Property Name=""ItemExpression"" Type=""Edm.String""/>
				<Property Name=""IsFilterProperty"" Type=""Edm.Boolean""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""SlotType"" Type=""Example.Database.SlotType"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""ScheduleTypeSlotType"">
				<Key>
					<PropertyRef Name=""ScheduleTypeSlotTypeId""/>
				</Key>
				<Property Name=""ScheduleTypeSlotTypeId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Active"" Type=""Edm.Boolean""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ScheduleType"" Type=""Example.Database.ScheduleType"" Nullable=""false""/>
				<NavigationProperty Name=""SlotType"" Type=""Example.Database.SlotType"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""ScheduleItem"">
				<Key>
					<PropertyRef Name=""ScheduleItemId""/>
				</Key>
				<Property Name=""ScheduleItemId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Status"" Type=""Edm.String""/>
				<Property Name=""StartDate"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""EndDate"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""PreparationDays"" Type=""Edm.Int64""/>
				<Property Name=""CleanupDays"" Type=""Edm.Int64""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Schedule"" Type=""Example.Database.Schedule"" Nullable=""false""/>
				<NavigationProperty Name=""Location"" Type=""Example.Database.Location"" Nullable=""false""/>
				<NavigationProperty Name=""Experiment"" Type=""Example.Database.Experiment"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""SlotItem"">
				<Key>
					<PropertyRef Name=""SlotItemId""/>
				</Key>
				<Property Name=""SlotItemId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""OrderNumber"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""ItemId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""EntityName"" Type=""Edm.String""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Entity"" Type=""Example.Database.Entity"" Nullable=""false""/>
				<NavigationProperty Name=""Slot"" Type=""Example.Database.Slot"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""SampleType"">
				<Key>
					<PropertyRef Name=""SampleTypeId""/>
				</Key>
				<Property Name=""SampleTypeId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<Property Name=""Status"" Type=""Edm.String""/>
				<Property Name=""NameFormat"" Type=""Edm.String""/>
				<Property Name=""IncubationTime"" Type=""Edm.Duration""/>
				<Property Name=""LoginRule"" Type=""Edm.String""/>
				<Property Name=""Type"" Type=""Edm.String""/>
				<Property Name=""Code"" Type=""Edm.String""/>
				<Property Name=""Configuration"" Type=""Edm.String""/>
				<Property Name=""TimeStamp"" Type=""Edm.Binary""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ParentSampleType"" Type=""Example.Database.SampleType"" Nullable=""false""/>
				<NavigationProperty Name=""Text"" Type=""Example.Database.Text"" Nullable=""false""/>
				<NavigationProperty Name=""DefaultContainerType"" Type=""Example.Database.ContainerType"" Nullable=""false""/>
				<NavigationProperty Name=""Group"" Type=""Example.Database.Group"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""SampleTypesByParentSampleType"" Type=""Collection(Example.Database.SampleType)""/>
				<NavigationProperty Name=""Samples"" Type=""Collection(Example.Database.Sample)""/>
				<NavigationProperty Name=""SamplesByControlSampleType"" Type=""Collection(Example.Database.Sample)""/>
				<NavigationProperty Name=""TestTypesByParentSampleType"" Type=""Collection(Example.Database.TestType)""/>
				<NavigationProperty Name=""SampleTypeFields"" Type=""Collection(Example.Database.SampleTypeField)""/>
				<NavigationProperty Name=""SampleTypeContainerTypes"" Type=""Collection(Example.Database.SampleTypeContainerType)""/>
				<NavigationProperty Name=""SampleTypeTestTypes"" Type=""Collection(Example.Database.SampleTypeTestType)""/>
				<NavigationProperty Name=""OrderTypeSampleTypes"" Type=""Collection(Example.Database.OrderTypeSampleType)""/>
				<NavigationProperty Name=""ControlChartTypes"" Type=""Collection(Example.Database.ControlChartType)""/>
				<NavigationProperty Name=""SequenceSamples"" Type=""Collection(Example.Database.SequenceSample)""/>
				<NavigationProperty Name=""OrderFormsByDefaultSampleType"" Type=""Collection(Example.Database.OrderForm)""/>
				<NavigationProperty Name=""OrderFormSamples"" Type=""Collection(Example.Database.OrderFormSample)""/>
				<NavigationProperty Name=""PriceListTestTypes"" Type=""Collection(Example.Database.PriceListTestType)""/>
				<NavigationProperty Name=""SpecificationItems"" Type=""Collection(Example.Database.SpecificationItem)""/>
				<NavigationProperty Name=""TestTypeTasks"" Type=""Collection(Example.Database.TestTypeTask)""/>
				<NavigationProperty Name=""SampleTypeTasks"" Type=""Collection(Example.Database.SampleTypeTask)""/>
				<NavigationProperty Name=""TestTypeQualityControlTriggers"" Type=""Collection(Example.Database.TestTypeQualityControlTrigger)""/>
				<NavigationProperty Name=""LaboratorySampleTypes"" Type=""Collection(Example.Database.LaboratorySampleType)""/>
				<NavigationProperty Name=""BKDProjectRules"" Type=""Collection(Example.Database.BKDProjectRule)""/>
			</EntityType>
			<EntityType Name=""Sample"">
				<Key>
					<PropertyRef Name=""SampleId""/>
				</Key>
				<Property Name=""SampleId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<Property Name=""Code"" Type=""Edm.String""/>
				<Property Name=""Status"" Type=""Edm.String""/>
				<Property Name=""SampleDate"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""DisposalDate"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""DisposedOn"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""DateResultsRequired"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""Containers"" Type=""Edm.Int64""/>
				<Property Name=""MobileCode"" Type=""Edm.String""/>
				<Property Name=""SpecificationStatus"" Type=""Edm.String""/>
				<Property Name=""Replication"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Amount"" Type=""Edm.Double""/>
				<Property Name=""HasNotes"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""Cancelled"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""CancelledOn"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""Authorised"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""AuthorisedOn"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""TimeStamp"" Type=""Edm.Binary""/>
				<Property Name=""Rejected"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""RejectedOn"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""PartitionKey"" Type=""Edm.Int32""/>
				<Property Name=""BKDInProcessOn"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""BKDInProcess"" Type=""Edm.Boolean""/>
				<Property Name=""BKDComment"" Type=""Edm.String""/>
				<Property Name=""BKDLotNumber"" Type=""Edm.String""/>
				<Property Name=""BKDGrindedOn"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""BKDGrinded"" Type=""Edm.Boolean""/>
				<Property Name=""BKDPipettedOn"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""BKDPipetted"" Type=""Edm.Boolean""/>
				<Property Name=""BKDSampleCode"" Type=""Edm.String""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Order"" Type=""Example.Database.Order"" Nullable=""false""/>
				<NavigationProperty Name=""Product"" Type=""Example.Database.Product"" Nullable=""false""/>
				<NavigationProperty Name=""Parent"" Type=""Example.Database.Sample"" Nullable=""false""/>
				<NavigationProperty Name=""SampleType"" Type=""Example.Database.SampleType"" Nullable=""false""/>
				<NavigationProperty Name=""PrimarySample"" Type=""Example.Database.Sample"" Nullable=""false""/>
				<NavigationProperty Name=""RequestedTest"" Type=""Example.Database.Test"" Nullable=""false""/>
				<NavigationProperty Name=""Experiment"" Type=""Example.Database.Experiment"" Nullable=""false""/>
				<NavigationProperty Name=""ClientProduct"" Type=""Example.Database.ClientProduct"" Nullable=""false""/>
				<NavigationProperty Name=""Priority"" Type=""Example.Database.Priority"" Nullable=""false""/>
				<NavigationProperty Name=""ControlSampleType"" Type=""Example.Database.SampleType"" Nullable=""false""/>
				<NavigationProperty Name=""AmountUnit"" Type=""Example.Database.Unit"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""CancelledBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""AuthorisedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""RejectedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Group"" Type=""Example.Database.Group"" Nullable=""false""/>
				<NavigationProperty Name=""BKDCrop"" Type=""Example.Database.BKDCrop"" Nullable=""false""/>
				<NavigationProperty Name=""BKDCultivar"" Type=""Example.Database.BKDCultivar"" Nullable=""false""/>
				<NavigationProperty Name=""BKDPlantMaterial"" Type=""Example.Database.BKDPlantMaterial"" Nullable=""false""/>
				<NavigationProperty Name=""BKDTestAmount"" Type=""Example.Database.BKDTestAmount"" Nullable=""false""/>
				<NavigationProperty Name=""BKDInProcessBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""BKDGrindedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""BKDPipettedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""BKDInternalResponsible"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""BKDCreatedFromProjectRule"" Type=""Example.Database.BKDProjectRule"" Nullable=""false""/>
				<NavigationProperty Name=""BKDProject"" Type=""Example.Database.Project"" Nullable=""false""/>
				<NavigationProperty Name=""Tests"" Type=""Collection(Example.Database.Test)""/>
				<NavigationProperty Name=""TestsByPrimarySample"" Type=""Collection(Example.Database.Test)""/>
				<NavigationProperty Name=""SamplesByParent"" Type=""Collection(Example.Database.Sample)""/>
				<NavigationProperty Name=""SamplesByPrimarySample"" Type=""Collection(Example.Database.Sample)""/>
				<NavigationProperty Name=""SampleContainers"" Type=""Collection(Example.Database.SampleContainer)""/>
				<NavigationProperty Name=""RackSamples"" Type=""Collection(Example.Database.RackSample)""/>
				<NavigationProperty Name=""SequenceSamples"" Type=""Collection(Example.Database.SequenceSample)""/>
				<NavigationProperty Name=""TestTasks"" Type=""Collection(Example.Database.TestTask)""/>
				<NavigationProperty Name=""SampleTasks"" Type=""Collection(Example.Database.SampleTask)""/>
				<NavigationProperty Name=""SuiteSamples"" Type=""Collection(Example.Database.SuiteSample)""/>
				<NavigationProperty Name=""ControlSamples"" Type=""Collection(Example.Database.ControlSample)""/>
				<NavigationProperty Name=""ControlSamplesByReferenceSample"" Type=""Collection(Example.Database.ControlSample)""/>
				<NavigationProperty Name=""Complaints"" Type=""Collection(Example.Database.Complaint)""/>
				<NavigationProperty Name=""BKDSampleRemarks"" Type=""Collection(Example.Database.BKDSampleRemark)""/>
			</EntityType>
			<EntityType Name=""TestType"">
				<Key>
					<PropertyRef Name=""TestTypeId""/>
				</Key>
				<Property Name=""TestTypeId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Code"" Type=""Edm.String""/>
				<Property Name=""Status"" Type=""Edm.String""/>
				<Property Name=""Type"" Type=""Edm.String""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<Property Name=""ScheduleStatus"" Type=""Edm.String""/>
				<Property Name=""ConfirmationRepeat"" Type=""Edm.Int64""/>
				<Property Name=""DefaultLayout"" Type=""Edm.String""/>
				<Property Name=""OrderNumber"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Instruction"" Type=""Edm.String""/>
				<Property Name=""ShowText"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""Configuration"" Type=""Edm.String""/>
				<Property Name=""usesInstrument"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""AutoComplete"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""NameFormat"" Type=""Edm.String""/>
				<Property Name=""TimeStamp"" Type=""Edm.Binary""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""BackgroundSchedulable"" Type=""Edm.Boolean"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Method"" Type=""Example.Database.Method"" Nullable=""false""/>
				<NavigationProperty Name=""ParentSampleType"" Type=""Example.Database.SampleType"" Nullable=""false""/>
				<NavigationProperty Name=""Text"" Type=""Example.Database.Text"" Nullable=""false""/>
				<NavigationProperty Name=""ReportType"" Type=""Example.Database.ReportType"" Nullable=""false""/>
				<NavigationProperty Name=""Group"" Type=""Example.Database.Group"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""InstructionText"" Type=""Example.Database.Text"" Nullable=""false""/>
				<NavigationProperty Name=""Tests"" Type=""Collection(Example.Database.Test)""/>
				<NavigationProperty Name=""TestTypeReportTypes"" Type=""Collection(Example.Database.TestTypeReportType)""/>
				<NavigationProperty Name=""TestTypeFields"" Type=""Collection(Example.Database.TestTypeField)""/>
				<NavigationProperty Name=""AdditionalTestTypesByParentTestType"" Type=""Collection(Example.Database.AdditionalTestType)""/>
				<NavigationProperty Name=""AdditionalTestTypes"" Type=""Collection(Example.Database.AdditionalTestType)""/>
				<NavigationProperty Name=""SampleTypeTestTypes"" Type=""Collection(Example.Database.SampleTypeTestType)""/>
				<NavigationProperty Name=""TestTypeResultTypes"" Type=""Collection(Example.Database.TestTypeResultType)""/>
				<NavigationProperty Name=""InstrumentTypeResultTypes"" Type=""Collection(Example.Database.InstrumentTypeResultType)""/>
				<NavigationProperty Name=""OrderFormTests"" Type=""Collection(Example.Database.OrderFormTest)""/>
				<NavigationProperty Name=""PriceListTestTypes"" Type=""Collection(Example.Database.PriceListTestType)""/>
				<NavigationProperty Name=""TestTypeMethods"" Type=""Collection(Example.Database.TestTypeMethod)""/>
				<NavigationProperty Name=""TestTypePriorities"" Type=""Collection(Example.Database.TestTypePriority)""/>
				<NavigationProperty Name=""TestTypeTasks"" Type=""Collection(Example.Database.TestTypeTask)""/>
				<NavigationProperty Name=""AccreditationTestTypes"" Type=""Collection(Example.Database.AccreditationTestType)""/>
				<NavigationProperty Name=""TestTypeQualityControlTriggers"" Type=""Collection(Example.Database.TestTypeQualityControlTrigger)""/>
				<NavigationProperty Name=""LaboratoryTestTypes"" Type=""Collection(Example.Database.LaboratoryTestType)""/>
				<NavigationProperty Name=""SuiteTestTypes"" Type=""Collection(Example.Database.SuiteTestType)""/>
				<NavigationProperty Name=""BKDProjectRulesByVirus"" Type=""Collection(Example.Database.BKDProjectRule)""/>
				<NavigationProperty Name=""BKDIncubationBatches"" Type=""Collection(Example.Database.BKDIncubationBatch)""/>
				<NavigationProperty Name=""BKDIncubationFormulations"" Type=""Collection(Example.Database.BKDIncubationFormulation)""/>
			</EntityType>
			<EntityType Name=""TestTypeReportType"">
				<Key>
					<PropertyRef Name=""TestTypeReportTypeId""/>
				</Key>
				<Property Name=""TestTypeReportTypeId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ReportType"" Type=""Example.Database.ReportType"" Nullable=""false""/>
				<NavigationProperty Name=""TestType"" Type=""Example.Database.TestType"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""OrderTypeField"">
				<Key>
					<PropertyRef Name=""OrderTypeFieldId""/>
				</Key>
				<Property Name=""OrderTypeFieldId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Value"" Type=""Edm.String""/>
				<Property Name=""OrderNumber"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""OrderField"" Type=""Edm.Boolean"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""OrderType"" Type=""Example.Database.OrderType"" Nullable=""false""/>
				<NavigationProperty Name=""EntityField"" Type=""Example.Database.EntityField"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""SampleTypeField"">
				<Key>
					<PropertyRef Name=""SampleTypeFieldId""/>
				</Key>
				<Property Name=""SampleTypeFieldId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Value"" Type=""Edm.String""/>
				<Property Name=""OrderNumber"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""OrderField"" Type=""Edm.Boolean"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""SampleType"" Type=""Example.Database.SampleType"" Nullable=""false""/>
				<NavigationProperty Name=""EntityField"" Type=""Example.Database.EntityField"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""TestTypeField"">
				<Key>
					<PropertyRef Name=""TestTypeFieldId""/>
				</Key>
				<Property Name=""TestTypeFieldId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""OrderNumber"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""TextValue"" Type=""Edm.String""/>
				<Property Name=""NumberValue"" Type=""Edm.Double""/>
				<Property Name=""DateValue"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""ItemId"" Type=""Edm.Int64""/>
				<Property Name=""EntityName"" Type=""Edm.String""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""TestType"" Type=""Example.Database.TestType"" Nullable=""false""/>
				<NavigationProperty Name=""EntityField"" Type=""Example.Database.EntityField"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""ResultTypeField"">
				<Key>
					<PropertyRef Name=""ResultTypeFieldId""/>
				</Key>
				<Property Name=""ResultTypeFieldId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Value"" Type=""Edm.String""/>
				<Property Name=""OrderNumber"" Type=""Edm.Int64"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ResultType"" Type=""Example.Database.ResultType"" Nullable=""false""/>
				<NavigationProperty Name=""EntityField"" Type=""Example.Database.EntityField"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""AdditionalTestType"">
				<Key>
					<PropertyRef Name=""AdditionalTestTypeId""/>
				</Key>
				<Property Name=""AdditionalTestTypeId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ParentTestType"" Type=""Example.Database.TestType"" Nullable=""false""/>
				<NavigationProperty Name=""TestType"" Type=""Example.Database.TestType"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""SlotProperty"">
				<Key>
					<PropertyRef Name=""SlotPropertyId""/>
				</Key>
				<Property Name=""SlotPropertyId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Value"" Type=""Edm.String""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Slot"" Type=""Example.Database.Slot"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""OrderType"">
				<Key>
					<PropertyRef Name=""OrderTypeId""/>
				</Key>
				<Property Name=""OrderTypeId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<Property Name=""Status"" Type=""Edm.String""/>
				<Property Name=""NameFormat"" Type=""Edm.String""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""PriceList"" Type=""Example.Database.PriceList"" Nullable=""false""/>
				<NavigationProperty Name=""Text"" Type=""Example.Database.Text"" Nullable=""false""/>
				<NavigationProperty Name=""Group"" Type=""Example.Database.Group"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""OrderTypeFields"" Type=""Collection(Example.Database.OrderTypeField)""/>
				<NavigationProperty Name=""Orders"" Type=""Collection(Example.Database.Order)""/>
				<NavigationProperty Name=""OrderTypeSampleTypes"" Type=""Collection(Example.Database.OrderTypeSampleType)""/>
				<NavigationProperty Name=""OrderForms"" Type=""Collection(Example.Database.OrderForm)""/>
				<NavigationProperty Name=""PriceLists"" Type=""Collection(Example.Database.PriceList)""/>
			</EntityType>
			<EntityType Name=""Order"">
				<Key>
					<PropertyRef Name=""OrderId""/>
				</Key>
				<Property Name=""OrderId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<Property Name=""Status"" Type=""Edm.String""/>
				<Property Name=""DateResultsRequired"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""ReportedOn"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""StartDate"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""CollectiveInvoice"" Type=""Edm.String""/>
				<Property Name=""MobileCode"" Type=""Edm.String""/>
				<Property Name=""SpecificationStatus"" Type=""Edm.String""/>
				<Property Name=""Discount"" Type=""Edm.Double""/>
				<Property Name=""HasNotes"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""Cancelled"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""CancelledOn"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""TimeStamp"" Type=""Edm.Binary""/>
				<Property Name=""Rejected"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""RejectedOn"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""Received"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""ReceivedOn"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""ArchiveDate"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""PartitionKey"" Type=""Edm.Int32""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Client"" Type=""Example.Database.Client"" Nullable=""false""/>
				<NavigationProperty Name=""OrderForm"" Type=""Example.Database.OrderForm"" Nullable=""false""/>
				<NavigationProperty Name=""PriceList"" Type=""Example.Database.PriceList"" Nullable=""false""/>
				<NavigationProperty Name=""OrderType"" Type=""Example.Database.OrderType"" Nullable=""false""/>
				<NavigationProperty Name=""Project"" Type=""Example.Database.Project"" Nullable=""false""/>
				<NavigationProperty Name=""Experiment"" Type=""Example.Database.Experiment"" Nullable=""false""/>
				<NavigationProperty Name=""Priority"" Type=""Example.Database.Priority"" Nullable=""false""/>
				<NavigationProperty Name=""ClientContact"" Type=""Example.Database.ClientContact"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""CancelledBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""RejectedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ReceivedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Group"" Type=""Example.Database.Group"" Nullable=""false""/>
				<NavigationProperty Name=""Samples"" Type=""Collection(Example.Database.Sample)""/>
				<NavigationProperty Name=""OrderReports"" Type=""Collection(Example.Database.OrderReport)""/>
				<NavigationProperty Name=""InvoiceOrders"" Type=""Collection(Example.Database.InvoiceOrder)""/>
				<NavigationProperty Name=""OrderTasks"" Type=""Collection(Example.Database.OrderTask)""/>
				<NavigationProperty Name=""Complaints"" Type=""Collection(Example.Database.Complaint)""/>
			</EntityType>
			<EntityType Name=""OrderReport"">
				<Key>
					<PropertyRef Name=""OrderReportId""/>
				</Key>
				<Property Name=""OrderReportId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Code"" Type=""Edm.String""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<Property Name=""Status"" Type=""Edm.String""/>
				<Property Name=""Contact"" Type=""Edm.String""/>
				<Property Name=""Data"" Type=""Edm.String""/>
				<Property Name=""Language"" Type=""Edm.String""/>
				<Property Name=""ReportedOn"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""DocumentGuid"" Type=""Edm.String""/>
				<Property Name=""VersionNumber"" Type=""Edm.Int64""/>
				<Property Name=""RequireReview"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""SendToOrderContact"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""SendToClientContact"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""Cancelled"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""CancelledOn"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""Reviewed"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""ReviewedOn"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""TimeStamp"" Type=""Edm.Binary""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Order"" Type=""Example.Database.Order"" Nullable=""false""/>
				<NavigationProperty Name=""ReportType"" Type=""Example.Database.ReportType"" Nullable=""false""/>
				<NavigationProperty Name=""File"" Type=""Example.Database.File"" Nullable=""false""/>
				<NavigationProperty Name=""CancelledBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ReviewedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""OrderReportDestinations"" Type=""Collection(Example.Database.OrderReportDestination)""/>
			</EntityType>
			<EntityType Name=""OrderReportDestination"">
				<Key>
					<PropertyRef Name=""OrderReportDestinationId""/>
				</Key>
				<Property Name=""OrderReportDestinationId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""DestinationType"" Type=""Edm.String""/>
				<Property Name=""EMail"" Type=""Edm.String""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""OrderReport"" Type=""Example.Database.OrderReport"" Nullable=""false""/>
				<NavigationProperty Name=""ClientContact"" Type=""Example.Database.ClientContact"" Nullable=""false""/>
				<NavigationProperty Name=""User"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Printer"" Type=""Example.Database.Printer"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""ClientReportType"">
				<Key>
					<PropertyRef Name=""ClientReportTypeId""/>
				</Key>
				<Property Name=""ClientReportTypeId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Contact"" Type=""Edm.String""/>
				<Property Name=""DestinationType"" Type=""Edm.String""/>
				<Property Name=""Destination"" Type=""Edm.String""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ReportType"" Type=""Example.Database.ReportType"" Nullable=""false""/>
				<NavigationProperty Name=""Client"" Type=""Example.Database.Client"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""ProjectDocument"">
				<Key>
					<PropertyRef Name=""ProjectDocumentId""/>
				</Key>
				<Property Name=""ProjectDocumentId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""GUID"" Type=""Edm.String""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Project"" Type=""Example.Database.Project"" Nullable=""false""/>
				<NavigationProperty Name=""File"" Type=""Example.Database.File"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""ContainerType"">
				<Key>
					<PropertyRef Name=""ContainerTypeId""/>
				</Key>
				<Property Name=""ContainerTypeId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""NameFormat"" Type=""Edm.String""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""SampleTypesByDefaultContainerType"" Type=""Collection(Example.Database.SampleType)""/>
				<NavigationProperty Name=""Containers"" Type=""Collection(Example.Database.Container)""/>
				<NavigationProperty Name=""SampleTypeContainerTypes"" Type=""Collection(Example.Database.SampleTypeContainerType)""/>
				<NavigationProperty Name=""ContainerTypeFields"" Type=""Collection(Example.Database.ContainerTypeField)""/>
			</EntityType>
			<EntityType Name=""Container"">
				<Key>
					<PropertyRef Name=""ContainerId""/>
				</Key>
				<Property Name=""ContainerId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ContainerType"" Type=""Example.Database.ContainerType"" Nullable=""false""/>
				<NavigationProperty Name=""SampleContainers"" Type=""Collection(Example.Database.SampleContainer)""/>
			</EntityType>
			<EntityType Name=""SampleContainer"">
				<Key>
					<PropertyRef Name=""SampleContainerId""/>
				</Key>
				<Property Name=""SampleContainerId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Container"" Type=""Example.Database.Container"" Nullable=""false""/>
				<NavigationProperty Name=""Sample"" Type=""Example.Database.Sample"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""SampleTypeContainerType"">
				<Key>
					<PropertyRef Name=""SampleTypeContainerTypeId""/>
				</Key>
				<Property Name=""SampleTypeContainerTypeId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<NavigationProperty Name=""SampleType"" Type=""Example.Database.SampleType"" Nullable=""false""/>
				<NavigationProperty Name=""ContainerType"" Type=""Example.Database.ContainerType"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""GraphType"">
				<Key>
					<PropertyRef Name=""GraphTypeId""/>
				</Key>
				<Property Name=""GraphTypeId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Title"" Type=""Edm.String""/>
				<Property Name=""Layout"" Type=""Edm.String""/>
				<Property Name=""ShowGrid"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""XAxisField"" Type=""Edm.String""/>
				<Property Name=""Mode"" Type=""Edm.String""/>
				<Property Name=""ShowSelectedForm"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""AutoScale"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""GraphTypeSeries"" Type=""Collection(Example.Database.GraphTypeSerie)""/>
			</EntityType>
			<EntityType Name=""GraphTypeSerie"">
				<Key>
					<PropertyRef Name=""GraphTypeSerieId""/>
				</Key>
				<Property Name=""GraphTypeSerieId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Type"" Type=""Edm.String""/>
				<Property Name=""Field"" Type=""Edm.String""/>
				<Property Name=""Section"" Type=""Edm.String""/>
				<Property Name=""ConstantValue"" Type=""Edm.Double""/>
				<Property Name=""Color"" Type=""Edm.String""/>
				<Property Name=""GraphDisplayType"" Type=""Edm.String""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""FormId"" Type=""Edm.Int64""/>
				<Property Name=""FieldId"" Type=""Edm.Int64""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""GraphType"" Type=""Example.Database.GraphType"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""RackType"">
				<Key>
					<PropertyRef Name=""RackTypeId""/>
				</Key>
				<Property Name=""RackTypeId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Data"" Type=""Edm.String""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<Property Name=""NameFormat"" Type=""Edm.String""/>
				<Property Name=""IncubationTime"" Type=""Edm.Int64""/>
				<Property Name=""HasNotes"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Racks"" Type=""Collection(Example.Database.Rack)""/>
				<NavigationProperty Name=""TestTypeTasks"" Type=""Collection(Example.Database.TestTypeTask)""/>
				<NavigationProperty Name=""SampleTypeTasks"" Type=""Collection(Example.Database.SampleTypeTask)""/>
			</EntityType>
			<EntityType Name=""Rack"">
				<Key>
					<PropertyRef Name=""RackId""/>
				</Key>
				<Property Name=""RackId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Status"" Type=""Edm.String""/>
				<Property Name=""Code"" Type=""Edm.String""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<Property Name=""Data"" Type=""Edm.String""/>
				<Property Name=""IncubationStart"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""IncubationEnd"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""Active"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""HasNotes"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""BKDFirstMeasurementOn"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""BKDFirstMeasurement"" Type=""Edm.Boolean""/>
				<Property Name=""BKDFirstMeasurementResult"" Type=""Edm.Double""/>
				<Property Name=""BKDSecondMeasurementOn"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""BKDSecondMeasurement"" Type=""Edm.Boolean""/>
				<Property Name=""BKDSecondMeasurementResult"" Type=""Edm.Double""/>
				<Property Name=""BKDConjugateAdded"" Type=""Edm.Boolean""/>
				<Property Name=""BKDSubstrateAdded"" Type=""Edm.Boolean""/>
				<Property Name=""BKDStampPlate"" Type=""Edm.Boolean""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""RackType"" Type=""Example.Database.RackType"" Nullable=""false""/>
				<NavigationProperty Name=""Parent"" Type=""Example.Database.Rack"" Nullable=""false""/>
				<NavigationProperty Name=""Slot"" Type=""Example.Database.Slot"" Nullable=""false""/>
				<NavigationProperty Name=""Location"" Type=""Example.Database.Location"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""BKDTest"" Type=""Example.Database.Test"" Nullable=""false""/>
				<NavigationProperty Name=""BKDFirstMeasurementBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""BKDSecondMeasurementBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""BKDIncubationBatch"" Type=""Example.Database.BKDIncubationBatch"" Nullable=""false""/>
				<NavigationProperty Name=""BKDSubstrateBatch"" Type=""Example.Database.BKDIncubationBatch"" Nullable=""false""/>
				<NavigationProperty Name=""Tests"" Type=""Collection(Example.Database.Test)""/>
				<NavigationProperty Name=""BKDTests"" Type=""Collection(Example.Database.Test)""/>
				<NavigationProperty Name=""RacksByParent"" Type=""Collection(Example.Database.Rack)""/>
				<NavigationProperty Name=""RackSamples"" Type=""Collection(Example.Database.RackSample)""/>
				<NavigationProperty Name=""Sequences"" Type=""Collection(Example.Database.Sequence)""/>
				<NavigationProperty Name=""TestTasks"" Type=""Collection(Example.Database.TestTask)""/>
				<NavigationProperty Name=""SampleTasks"" Type=""Collection(Example.Database.SampleTask)""/>
			</EntityType>
			<EntityType Name=""RackSample"">
				<Key>
					<PropertyRef Name=""RackSampleId""/>
				</Key>
				<Property Name=""RackSampleId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<Property Name=""Code"" Type=""Edm.String""/>
				<Property Name=""OrderNumber"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Status"" Type=""Edm.String""/>
				<Property Name=""HasNotes"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""BKDFirstMeasurementOn"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""BKDFirstMeasurement"" Type=""Edm.Boolean""/>
				<Property Name=""BKDFirstMeasurementResult"" Type=""Edm.Double""/>
				<Property Name=""BKDSecondMeasurementOn"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""BKDSecondMeasurement"" Type=""Edm.Boolean""/>
				<Property Name=""BKDSecondMeasurementResult"" Type=""Edm.Double""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Sample"" Type=""Example.Database.Sample"" Nullable=""false""/>
				<NavigationProperty Name=""Rack"" Type=""Example.Database.Rack"" Nullable=""false""/>
				<NavigationProperty Name=""Parent"" Type=""Example.Database.RackSample"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""BKDFirstMeasurementBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""BKDSecondMeasurementBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""RackSamplesByParent"" Type=""Collection(Example.Database.RackSample)""/>
				<NavigationProperty Name=""SequenceSamples"" Type=""Collection(Example.Database.SequenceSample)""/>
			</EntityType>
			<EntityType Name=""ProcessStationType"">
				<Key>
					<PropertyRef Name=""ProcessStationTypeId""/>
				</Key>
				<Property Name=""ProcessStationTypeId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<Property Name=""Data"" Type=""Edm.String""/>
				<Property Name=""HideTitle"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""HasNotes"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ProcessStations"" Type=""Collection(Example.Database.ProcessStation)""/>
			</EntityType>
			<EntityType Name=""ProcessStation"">
				<Key>
					<PropertyRef Name=""ProcessStationId""/>
				</Key>
				<Property Name=""ProcessStationId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<Property Name=""HideProperties"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""HasNotes"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ProcessStationType"" Type=""Example.Database.ProcessStationType"" Nullable=""false""/>
				<NavigationProperty Name=""Location"" Type=""Example.Database.Location"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Operation"" Type=""Example.Database.Operation"" Nullable=""false""/>
				<NavigationProperty Name=""ProcessStationInstruments"" Type=""Collection(Example.Database.ProcessStationInstrument)""/>
			</EntityType>
			<EntityType Name=""SampleTypeTestType"">
				<Key>
					<PropertyRef Name=""SampleTypeTestTypeId""/>
				</Key>
				<Property Name=""SampleTypeTestTypeId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""LoginRule"" Type=""Edm.String""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""SampleType"" Type=""Example.Database.SampleType"" Nullable=""false""/>
				<NavigationProperty Name=""TestType"" Type=""Example.Database.TestType"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""OrderTypeSampleType"">
				<Key>
					<PropertyRef Name=""OrderTypeSampleTypeId""/>
				</Key>
				<Property Name=""OrderTypeSampleTypeId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""OrderType"" Type=""Example.Database.OrderType"" Nullable=""false""/>
				<NavigationProperty Name=""SampleType"" Type=""Example.Database.SampleType"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""TestTypeResultType"">
				<Key>
					<PropertyRef Name=""TestTypeResultTypeId""/>
				</Key>
				<Property Name=""TestTypeResultTypeId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Required"" Type=""Edm.Boolean""/>
				<Property Name=""TestFormSection"" Type=""Edm.String""/>
				<Property Name=""OrderNumber"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""EnabledExpression"" Type=""Edm.String""/>
				<Property Name=""ValueExpression"" Type=""Edm.String""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""TestType"" Type=""Example.Database.TestType"" Nullable=""false""/>
				<NavigationProperty Name=""ResultType"" Type=""Example.Database.ResultType"" Nullable=""false""/>
				<NavigationProperty Name=""Method"" Type=""Example.Database.Method"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""Protocol"">
				<Key>
					<PropertyRef Name=""ProtocolId""/>
				</Key>
				<Property Name=""ProtocolId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<Property Name=""Status"" Type=""Edm.String""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Location"" Type=""Example.Database.Location"" Nullable=""false""/>
				<NavigationProperty Name=""Document"" Type=""Example.Database.File"" Nullable=""false""/>
				<NavigationProperty Name=""ProjectLeader"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Experiments"" Type=""Collection(Example.Database.Experiment)""/>
			</EntityType>
			<EntityType Name=""QualificationType"">
				<Key>
					<PropertyRef Name=""QualificationTypeId""/>
				</Key>
				<Property Name=""QualificationTypeId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""HasNotes"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""UserQualifications"" Type=""Collection(Example.Database.UserQualification)""/>
				<NavigationProperty Name=""ScheduleTypeQualifications"" Type=""Collection(Example.Database.ScheduleTypeQualification)""/>
			</EntityType>
			<EntityType Name=""UserQualification"">
				<Key>
					<PropertyRef Name=""UserQualificationId""/>
				</Key>
				<Property Name=""UserQualificationId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ValidFrom"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ValidTo"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Expired"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""Renewed"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""HasNotes"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""User"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""QualificationType"" Type=""Example.Database.QualificationType"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""ScheduleTypeQualification"">
				<Key>
					<PropertyRef Name=""ScheduleTypeQualificationId""/>
				</Key>
				<Property Name=""ScheduleTypeQualificationId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""HasNotes"" Type=""Edm.Boolean"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ScheduleType"" Type=""Example.Database.ScheduleType"" Nullable=""false""/>
				<NavigationProperty Name=""QualificationType"" Type=""Example.Database.QualificationType"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""Chemical"">
				<Key>
					<PropertyRef Name=""ChemicalId""/>
				</Key>
				<Property Name=""ChemicalId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Data"" Type=""Edm.String""/>
				<Property Name=""PubchemCompoundId"" Type=""Edm.String""/>
				<Property Name=""CasName"" Type=""Edm.String""/>
				<Property Name=""IUPACName"" Type=""Edm.String""/>
				<Property Name=""InChI"" Type=""Edm.String""/>
				<Property Name=""Mass"" Type=""Edm.Double""/>
				<Property Name=""Density"" Type=""Edm.Double""/>
				<Property Name=""MolecularWeight"" Type=""Edm.Double""/>
				<Property Name=""MolecularFormula"" Type=""Edm.String""/>
				<Property Name=""MolecularFormulaHtml"" Type=""Edm.String""/>
				<Property Name=""HasNotes"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""TimeStamp"" Type=""Edm.Binary""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""File"" Type=""Example.Database.File"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ResultTypes"" Type=""Collection(Example.Database.ResultType)""/>
			</EntityType>
			<EntityType Name=""Reaction"">
				<Key>
					<PropertyRef Name=""ReactionId""/>
				</Key>
				<Property Name=""ReactionId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Data"" Type=""Edm.String""/>
				<Property Name=""HasNotes"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""TimeStamp"" Type=""Edm.Binary""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""File"" Type=""Example.Database.File"" Nullable=""false""/>
				<NavigationProperty Name=""Experiment"" Type=""Example.Database.Experiment"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""InstrumentType"">
				<Key>
					<PropertyRef Name=""InstrumentTypeId""/>
				</Key>
				<Property Name=""InstrumentTypeId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Data"" Type=""Edm.String""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<Property Name=""InstrumentModule"" Type=""Edm.String""/>
				<Property Name=""InjectionVolume"" Type=""Edm.Double""/>
				<Property Name=""InjectionCount"" Type=""Edm.Int64""/>
				<Property Name=""InstrumentMethod"" Type=""Edm.String""/>
				<Property Name=""RunTime"" Type=""Edm.Double""/>
				<Property Name=""AutoCreateSequence"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""AutoCompleteSequence"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""HasNotes"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""SequenceType"" Type=""Example.Database.SequenceType"" Nullable=""false""/>
				<NavigationProperty Name=""TypescriptModule"" Type=""Example.Database.Module"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Instruments"" Type=""Collection(Example.Database.Instrument)""/>
				<NavigationProperty Name=""InstrumentTypeResultTypes"" Type=""Collection(Example.Database.InstrumentTypeResultType)""/>
				<NavigationProperty Name=""TestTypeTasks"" Type=""Collection(Example.Database.TestTypeTask)""/>
				<NavigationProperty Name=""SampleTypeTasks"" Type=""Collection(Example.Database.SampleTypeTask)""/>
			</EntityType>
			<EntityType Name=""Instrument"">
				<Key>
					<PropertyRef Name=""InstrumentId""/>
				</Key>
				<Property Name=""InstrumentId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<Property Name=""Data"" Type=""Edm.String""/>
				<Property Name=""CalibrationDate"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""MaintenanceDate"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""CalibrationInterval"" Type=""Edm.Int64""/>
				<Property Name=""MaintenanceInterval"" Type=""Edm.Int64""/>
				<Property Name=""HasNotes"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""InstrumentType"" Type=""Example.Database.InstrumentType"" Nullable=""false""/>
				<NavigationProperty Name=""Device"" Type=""Example.Database.Device"" Nullable=""false""/>
				<NavigationProperty Name=""ManagedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""InstrumentLocation"" Type=""Example.Database.Location"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ProcessStationInstruments"" Type=""Collection(Example.Database.ProcessStationInstrument)""/>
				<NavigationProperty Name=""Tests"" Type=""Collection(Example.Database.Test)""/>
				<NavigationProperty Name=""Results"" Type=""Collection(Example.Database.Result)""/>
				<NavigationProperty Name=""InstrumentFiles"" Type=""Collection(Example.Database.InstrumentFile)""/>
				<NavigationProperty Name=""Sequences"" Type=""Collection(Example.Database.Sequence)""/>
				<NavigationProperty Name=""InstrumentParts"" Type=""Collection(Example.Database.InstrumentPart)""/>
				<NavigationProperty Name=""TestTasks"" Type=""Collection(Example.Database.TestTask)""/>
				<NavigationProperty Name=""SampleTasks"" Type=""Collection(Example.Database.SampleTask)""/>
			</EntityType>
			<EntityType Name=""InstrumentFile"">
				<Key>
					<PropertyRef Name=""InstrumentFileId""/>
				</Key>
				<Property Name=""InstrumentFileId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Type"" Type=""Edm.String""/>
				<Property Name=""FileName"" Type=""Edm.String""/>
				<Property Name=""MD5"" Type=""Edm.String""/>
				<Property Name=""FileCreatedOn"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""SequenceData"" Type=""Edm.String""/>
				<Property Name=""Status"" Type=""Edm.String""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""Cancelled"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""CancelledOn"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""TimeStamp"" Type=""Edm.Binary""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Instrument"" Type=""Example.Database.Instrument"" Nullable=""false""/>
				<NavigationProperty Name=""File"" Type=""Example.Database.File"" Nullable=""false""/>
				<NavigationProperty Name=""Device"" Type=""Example.Database.Device"" Nullable=""false""/>
				<NavigationProperty Name=""Experiment"" Type=""Example.Database.Experiment"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""CancelledBy"" Type=""Example.Database.User"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""MaterialType"">
				<Key>
					<PropertyRef Name=""MaterialTypeId""/>
				</Key>
				<Property Name=""MaterialTypeId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<Property Name=""NameFormat"" Type=""Edm.String""/>
				<Property Name=""HasNotes"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ReportType"" Type=""Example.Database.ReportType"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Materials"" Type=""Collection(Example.Database.Material)""/>
				<NavigationProperty Name=""BatchFormulationTypesByFormulationMaterialType"" Type=""Collection(Example.Database.BatchFormulationType)""/>
				<NavigationProperty Name=""SampleTypeTasks"" Type=""Collection(Example.Database.SampleTypeTask)""/>
			</EntityType>
			<EntityType Name=""Material"">
				<Key>
					<PropertyRef Name=""MaterialId""/>
				</Key>
				<Property Name=""MaterialId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<Property Name=""Status"" Type=""Edm.String""/>
				<Property Name=""AlternativeName"" Type=""Edm.String""/>
				<Property Name=""Amount"" Type=""Edm.Double"" Nullable=""false""/>
				<Property Name=""HasNotes"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""Authorised"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""AuthorisedOn"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""Received"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""ReceivedOn"" Type=""Edm.DateTimeOffset""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""MaterialType"" Type=""Example.Database.MaterialType"" Nullable=""false""/>
				<NavigationProperty Name=""Unit"" Type=""Example.Database.Unit"" Nullable=""false""/>
				<NavigationProperty Name=""Project"" Type=""Example.Database.Project"" Nullable=""false""/>
				<NavigationProperty Name=""Batch"" Type=""Example.Database.Batch"" Nullable=""false""/>
				<NavigationProperty Name=""Location"" Type=""Example.Database.Location"" Nullable=""false""/>
				<NavigationProperty Name=""ParentMaterial"" Type=""Example.Database.Material"" Nullable=""false""/>
				<NavigationProperty Name=""BatchItem"" Type=""Example.Database.BatchItem"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""AuthorisedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ReceivedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""MaterialsByParentMaterial"" Type=""Collection(Example.Database.Material)""/>
				<NavigationProperty Name=""BatchItems"" Type=""Collection(Example.Database.BatchItem)""/>
				<NavigationProperty Name=""SampleTasks"" Type=""Collection(Example.Database.SampleTask)""/>
			</EntityType>
			<EntityType Name=""BatchType"">
				<Key>
					<PropertyRef Name=""BatchTypeId""/>
				</Key>
				<Property Name=""BatchTypeId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<Property Name=""NameFormat"" Type=""Edm.String""/>
				<Property Name=""BatchSize"" Type=""Edm.Int64""/>
				<Property Name=""HasNotes"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ReportType"" Type=""Example.Database.ReportType"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""BatchFormulationTypes"" Type=""Collection(Example.Database.BatchFormulationType)""/>
				<NavigationProperty Name=""BatchFormulationTypesByFormulationBatchType"" Type=""Collection(Example.Database.BatchFormulationType)""/>
				<NavigationProperty Name=""Batches"" Type=""Collection(Example.Database.Batch)""/>
			</EntityType>
			<EntityType Name=""BatchFormulationType"">
				<Key>
					<PropertyRef Name=""BatchFormulationTypeId""/>
				</Key>
				<Property Name=""BatchFormulationTypeId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Amount"" Type=""Edm.Double"" Nullable=""false""/>
				<Property Name=""OrderNumber"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""HasNotes"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<NavigationProperty Name=""BatchType"" Type=""Example.Database.BatchType"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Unit"" Type=""Example.Database.Unit"" Nullable=""false""/>
				<NavigationProperty Name=""FormulationMaterialType"" Type=""Example.Database.MaterialType"" Nullable=""false""/>
				<NavigationProperty Name=""FormulationBatchType"" Type=""Example.Database.BatchType"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""Batch"">
				<Key>
					<PropertyRef Name=""BatchId""/>
				</Key>
				<Property Name=""BatchId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Amount"" Type=""Edm.Double"" Nullable=""false""/>
				<Property Name=""Status"" Type=""Edm.String""/>
				<Property Name=""HasNotes"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Unit"" Type=""Example.Database.Unit"" Nullable=""false""/>
				<NavigationProperty Name=""Location"" Type=""Example.Database.Location"" Nullable=""false""/>
				<NavigationProperty Name=""Project"" Type=""Example.Database.Project"" Nullable=""false""/>
				<NavigationProperty Name=""BatchType"" Type=""Example.Database.BatchType"" Nullable=""false""/>
				<NavigationProperty Name=""ParentBatch"" Type=""Example.Database.Batch"" Nullable=""false""/>
				<NavigationProperty Name=""UsedInBatch"" Type=""Example.Database.Batch"" Nullable=""false""/>
				<NavigationProperty Name=""BatchItem"" Type=""Example.Database.BatchItem"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Materials"" Type=""Collection(Example.Database.Material)""/>
				<NavigationProperty Name=""BatchesByParentBatch"" Type=""Collection(Example.Database.Batch)""/>
				<NavigationProperty Name=""BatchesByUsedInBatch"" Type=""Collection(Example.Database.Batch)""/>
				<NavigationProperty Name=""Products"" Type=""Collection(Example.Database.Product)""/>
				<NavigationProperty Name=""BatchItemsByParentBatch"" Type=""Collection(Example.Database.BatchItem)""/>
				<NavigationProperty Name=""BatchItemsByChildBatch"" Type=""Collection(Example.Database.BatchItem)""/>
			</EntityType>
			<EntityType Name=""ProductType"">
				<Key>
					<PropertyRef Name=""ProductTypeId""/>
				</Key>
				<Property Name=""ProductTypeId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<Property Name=""NameFormat"" Type=""Edm.String""/>
				<Property Name=""HasNotes"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ReportType"" Type=""Example.Database.ReportType"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ProductUnitTypes"" Type=""Collection(Example.Database.ProductUnitType)""/>
				<NavigationProperty Name=""Products"" Type=""Collection(Example.Database.Product)""/>
				<NavigationProperty Name=""SpecificationItems"" Type=""Collection(Example.Database.SpecificationItem)""/>
				<NavigationProperty Name=""LaboratoryProductTypes"" Type=""Collection(Example.Database.LaboratoryProductType)""/>
			</EntityType>
			<EntityType Name=""ProductUnitType"">
				<Key>
					<PropertyRef Name=""ProductUnitTypeId""/>
				</Key>
				<Property Name=""ProductUnitTypeId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<Property Name=""NameFormat"" Type=""Edm.String""/>
				<Property Name=""HasNotes"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ProductType"" Type=""Example.Database.ProductType"" Nullable=""false""/>
				<NavigationProperty Name=""ReportType"" Type=""Example.Database.ReportType"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ProductUnits"" Type=""Collection(Example.Database.ProductUnit)""/>
			</EntityType>
			<EntityType Name=""ProductUnit"">
				<Key>
					<PropertyRef Name=""ProductUnitId""/>
				</Key>
				<Property Name=""ProductUnitId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<Property Name=""Amount"" Type=""Edm.Double"" Nullable=""false""/>
				<Property Name=""Status"" Type=""Edm.String""/>
				<Property Name=""HasNotes"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Unit"" Type=""Example.Database.Unit"" Nullable=""false""/>
				<NavigationProperty Name=""Location"" Type=""Example.Database.Location"" Nullable=""false""/>
				<NavigationProperty Name=""ProductUnitType"" Type=""Example.Database.ProductUnitType"" Nullable=""false""/>
				<NavigationProperty Name=""Product"" Type=""Example.Database.Product"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""ControlChartType"">
				<Key>
					<PropertyRef Name=""ControlChartTypeId""/>
				</Key>
				<Property Name=""ControlChartTypeId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<Property Name=""Data"" Type=""Edm.String""/>
				<Property Name=""NameFormat"" Type=""Edm.String""/>
				<Property Name=""MaximumResultsPerChart"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""StandardDeviationLowLimit"" Type=""Edm.Double""/>
				<Property Name=""StandardDeviationHighLimit"" Type=""Edm.Double""/>
				<Property Name=""Drift"" Type=""Edm.Double""/>
				<Property Name=""GrubbTest"" Type=""Edm.String""/>
				<Property Name=""GrubbConfidence"" Type=""Edm.String""/>
				<Property Name=""DisplayType"" Type=""Edm.String""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<NavigationProperty Name=""SampleType"" Type=""Example.Database.SampleType"" Nullable=""false""/>
				<NavigationProperty Name=""ResultType"" Type=""Example.Database.ResultType"" Nullable=""false""/>
				<NavigationProperty Name=""Product"" Type=""Example.Database.Product"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ControlCharts"" Type=""Collection(Example.Database.ControlChart)""/>
			</EntityType>
			<EntityType Name=""ControlChart"">
				<Key>
					<PropertyRef Name=""ControlChartId""/>
				</Key>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<Property Name=""Data"" Type=""Edm.String""/>
				<Property Name=""Status"" Type=""Edm.String""/>
				<Property Name=""ChartNumber"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""ControlChartId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""HasNotes"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<NavigationProperty Name=""ControlChartType"" Type=""Example.Database.ControlChartType"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ControlChartResults"" Type=""Collection(Example.Database.ControlChartResult)""/>
			</EntityType>
			<EntityType Name=""ControlChartResult"">
				<Key>
					<PropertyRef Name=""ControlChartResultId""/>
				</Key>
				<Property Name=""ControlChartResultId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Status"" Type=""Edm.String""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<NavigationProperty Name=""Result"" Type=""Example.Database.Result"" Nullable=""false""/>
				<NavigationProperty Name=""ControlChart"" Type=""Example.Database.ControlChart"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""SequenceType"">
				<Key>
					<PropertyRef Name=""SequenceTypeId""/>
				</Key>
				<Property Name=""SequenceTypeId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<Property Name=""Data"" Type=""Edm.String""/>
				<Property Name=""NameFormat"" Type=""Edm.String""/>
				<Property Name=""InjectionVolume"" Type=""Edm.Double""/>
				<Property Name=""InjectionCount"" Type=""Edm.Int64""/>
				<Property Name=""InstrumentMethod"" Type=""Edm.String""/>
				<Property Name=""RunTime"" Type=""Edm.Double""/>
				<Property Name=""ValidationRule"" Type=""Edm.String""/>
				<Property Name=""FixedNumberOfSample"" Type=""Edm.Boolean""/>
				<Property Name=""AddFileAsSampleNote"" Type=""Edm.Boolean""/>
				<Property Name=""HasNotes"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""SequenceResultEntryFormTemplate"" Type=""Example.Database.FormTemplate"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Group"" Type=""Example.Database.Group"" Nullable=""false""/>
				<NavigationProperty Name=""InstrumentTypes"" Type=""Collection(Example.Database.InstrumentType)""/>
				<NavigationProperty Name=""Sequences"" Type=""Collection(Example.Database.Sequence)""/>
				<NavigationProperty Name=""SequenceTypeResultTypes"" Type=""Collection(Example.Database.SequenceTypeResultType)""/>
			</EntityType>
			<EntityType Name=""Sequence"">
				<Key>
					<PropertyRef Name=""SequenceId""/>
				</Key>
				<Property Name=""SequenceId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Code"" Type=""Edm.String""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<Property Name=""ValidationStatus"" Type=""Edm.String""/>
				<Property Name=""Status"" Type=""Edm.String""/>
				<Property Name=""AuditType"" Type=""Edm.String""/>
				<Property Name=""ValidationLegend"" Type=""Edm.String""/>
				<Property Name=""HasNotes"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""Completed"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""CompletedOn"" Type=""Edm.DateTimeOffset""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Instrument"" Type=""Example.Database.Instrument"" Nullable=""false""/>
				<NavigationProperty Name=""SequenceType"" Type=""Example.Database.SequenceType"" Nullable=""false""/>
				<NavigationProperty Name=""Slot"" Type=""Example.Database.Slot"" Nullable=""false""/>
				<NavigationProperty Name=""Rack"" Type=""Example.Database.Rack"" Nullable=""false""/>
				<NavigationProperty Name=""Location"" Type=""Example.Database.Location"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""CompletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Group"" Type=""Example.Database.Group"" Nullable=""false""/>
				<NavigationProperty Name=""SequenceSamples"" Type=""Collection(Example.Database.SequenceSample)""/>
			</EntityType>
			<EntityType Name=""SequenceSample"">
				<Key>
					<PropertyRef Name=""SequenceSampleId""/>
				</Key>
				<Property Name=""SequenceSampleId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<Property Name=""Type"" Type=""Edm.String""/>
				<Property Name=""Position"" Type=""Edm.String""/>
				<Property Name=""Dilution"" Type=""Edm.Double""/>
				<Property Name=""OrderNumber"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Status"" Type=""Edm.String""/>
				<Property Name=""ValidationStatus"" Type=""Edm.String""/>
				<Property Name=""ApprovalReason"" Type=""Edm.String""/>
				<Property Name=""HasNotes"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Sample"" Type=""Example.Database.Sample"" Nullable=""false""/>
				<NavigationProperty Name=""SampleType"" Type=""Example.Database.SampleType"" Nullable=""false""/>
				<NavigationProperty Name=""RackSample"" Type=""Example.Database.RackSample"" Nullable=""false""/>
				<NavigationProperty Name=""Sequence"" Type=""Example.Database.Sequence"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""SequenceSampleResults"" Type=""Collection(Example.Database.SequenceSampleResult)""/>
			</EntityType>
			<EntityType Name=""SequenceSampleResult"">
				<Key>
					<PropertyRef Name=""SequenceSampleResultId""/>
				</Key>
				<Property Name=""SequenceSampleResultId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""EnteredOn"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""EnteredValue"" Type=""Edm.String""/>
				<Property Name=""NumberValue"" Type=""Edm.Double""/>
				<Property Name=""TextValue"" Type=""Edm.String""/>
				<Property Name=""Unit"" Type=""Edm.String""/>
				<Property Name=""Status"" Type=""Edm.String""/>
				<Property Name=""ValidationStatus"" Type=""Edm.String""/>
				<Property Name=""TestTypeName"" Type=""Edm.String""/>
				<Property Name=""LLOQ"" Type=""Edm.Double""/>
				<Property Name=""Dilution"" Type=""Edm.Double""/>
				<Property Name=""Replication"" Type=""Edm.Int64""/>
				<Property Name=""IsReplicationParent"" Type=""Edm.Boolean""/>
				<Property Name=""Prefix"" Type=""Edm.String""/>
				<Property Name=""HasNotes"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""SequenceSample"" Type=""Example.Database.SequenceSample"" Nullable=""false""/>
				<NavigationProperty Name=""Result"" Type=""Example.Database.Result"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""InstrumentTypeResultType"">
				<Key>
					<PropertyRef Name=""InstrumentTypeResultTypeId""/>
				</Key>
				<Property Name=""InstrumentTypeResultTypeId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""TestTypeName"" Type=""Edm.String""/>
				<Property Name=""WarningLow"" Type=""Edm.Double""/>
				<Property Name=""WarningHigh"" Type=""Edm.Double""/>
				<Property Name=""ActionLow"" Type=""Edm.Double""/>
				<Property Name=""ActionHigh"" Type=""Edm.Double""/>
				<Property Name=""HasNotes"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""InstrumentType"" Type=""Example.Database.InstrumentType"" Nullable=""false""/>
				<NavigationProperty Name=""ResultType"" Type=""Example.Database.ResultType"" Nullable=""false""/>
				<NavigationProperty Name=""TestType"" Type=""Example.Database.TestType"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""Product"">
				<Key>
					<PropertyRef Name=""ProductId""/>
				</Key>
				<Property Name=""ProductId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<Property Name=""Status"" Type=""Edm.String""/>
				<Property Name=""HasNotes"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""TimeStamp"" Type=""Edm.Binary""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Project"" Type=""Example.Database.Project"" Nullable=""false""/>
				<NavigationProperty Name=""ProductType"" Type=""Example.Database.ProductType"" Nullable=""false""/>
				<NavigationProperty Name=""Location"" Type=""Example.Database.Location"" Nullable=""false""/>
				<NavigationProperty Name=""Batch"" Type=""Example.Database.Batch"" Nullable=""false""/>
				<NavigationProperty Name=""Text"" Type=""Example.Database.Text"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Samples"" Type=""Collection(Example.Database.Sample)""/>
				<NavigationProperty Name=""ProductUnits"" Type=""Collection(Example.Database.ProductUnit)""/>
				<NavigationProperty Name=""ControlChartTypes"" Type=""Collection(Example.Database.ControlChartType)""/>
				<NavigationProperty Name=""ClientProduct"" Type=""Collection(Example.Database.ClientProduct)""/>
				<NavigationProperty Name=""PriceListTestTypes"" Type=""Collection(Example.Database.PriceListTestType)""/>
				<NavigationProperty Name=""SpecificationItems"" Type=""Collection(Example.Database.SpecificationItem)""/>
				<NavigationProperty Name=""LaboratoryProducts"" Type=""Collection(Example.Database.LaboratoryProduct)""/>
				<NavigationProperty Name=""Complaints"" Type=""Collection(Example.Database.Complaint)""/>
			</EntityType>
			<EntityType Name=""OrderForm"">
				<Key>
					<PropertyRef Name=""OrderFormId""/>
				</Key>
				<Property Name=""OrderFormId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Code"" Type=""Edm.String""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<Property Name=""Contact"" Type=""Edm.String""/>
				<Property Name=""Default"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""Type"" Type=""Edm.String""/>
				<Property Name=""Layout"" Type=""Edm.String""/>
				<Property Name=""CanEditContainers"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""MaximumContainers"" Type=""Edm.Int64""/>
				<Property Name=""EnableTestPriority"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""TestSelectionRule"" Type=""Edm.String""/>
				<Property Name=""CanEditFields"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""EditTestRule"" Type=""Edm.String""/>
				<Property Name=""AtLeaseOneTestRequiredPerSample"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""HasNotes"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""TimeStamp"" Type=""Edm.Binary""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Client"" Type=""Example.Database.Client"" Nullable=""false""/>
				<NavigationProperty Name=""PriceList"" Type=""Example.Database.PriceList"" Nullable=""false""/>
				<NavigationProperty Name=""OrderType"" Type=""Example.Database.OrderType"" Nullable=""false""/>
				<NavigationProperty Name=""OrderConfirmationReportType"" Type=""Example.Database.ReportType"" Nullable=""false""/>
				<NavigationProperty Name=""ParentOrderForm"" Type=""Example.Database.OrderForm"" Nullable=""false""/>
				<NavigationProperty Name=""DefaultSampleType"" Type=""Example.Database.SampleType"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Group"" Type=""Example.Database.Group"" Nullable=""false""/>
				<NavigationProperty Name=""Orders"" Type=""Collection(Example.Database.Order)""/>
				<NavigationProperty Name=""OrderFormsByParentOrderForm"" Type=""Collection(Example.Database.OrderForm)""/>
				<NavigationProperty Name=""OrderFormFields"" Type=""Collection(Example.Database.OrderFormField)""/>
				<NavigationProperty Name=""OrderFormSamples"" Type=""Collection(Example.Database.OrderFormSample)""/>
				<NavigationProperty Name=""OrderFormSampleFields"" Type=""Collection(Example.Database.OrderFormSampleField)""/>
				<NavigationProperty Name=""OrderFormTests"" Type=""Collection(Example.Database.OrderFormTest)""/>
				<NavigationProperty Name=""OrderFormReports"" Type=""Collection(Example.Database.OrderFormReport)""/>
			</EntityType>
			<EntityType Name=""ClientProduct"">
				<Key>
					<PropertyRef Name=""ClientProductId""/>
				</Key>
				<Property Name=""ClientProductId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<Property Name=""SpecificationRule"" Type=""Edm.String""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Client"" Type=""Example.Database.Client"" Nullable=""false""/>
				<NavigationProperty Name=""Product"" Type=""Example.Database.Product"" Nullable=""false""/>
				<NavigationProperty Name=""Samples"" Type=""Collection(Example.Database.Sample)""/>
			</EntityType>
			<EntityType Name=""OrderFormField"">
				<Key>
					<PropertyRef Name=""OrderFormFieldId""/>
				</Key>
				<Property Name=""OrderFormFieldId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Value"" Type=""Edm.String""/>
				<Property Name=""OrderNumber"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""StoreInTemplate"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""Readonly"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""Required"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""Visible"" Type=""Edm.Boolean"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""OrderForm"" Type=""Example.Database.OrderForm"" Nullable=""false""/>
				<NavigationProperty Name=""EntityField"" Type=""Example.Database.EntityField"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""OrderFormSample"">
				<Key>
					<PropertyRef Name=""OrderFormSampleId""/>
				</Key>
				<Property Name=""OrderFormSampleId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<Property Name=""SampleDate"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""OrderNumber"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""HasNotes"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""OrderForm"" Type=""Example.Database.OrderForm"" Nullable=""false""/>
				<NavigationProperty Name=""SampleType"" Type=""Example.Database.SampleType"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""OrderFormSampleFields"" Type=""Collection(Example.Database.OrderFormSampleField)""/>
				<NavigationProperty Name=""OrderFormTests"" Type=""Collection(Example.Database.OrderFormTest)""/>
			</EntityType>
			<EntityType Name=""OrderFormSampleField"">
				<Key>
					<PropertyRef Name=""OrderFormSampleFieldId""/>
				</Key>
				<Property Name=""OrderFormSampleFieldId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Value"" Type=""Edm.String""/>
				<Property Name=""OrderNumber"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""StoreInTemplate"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""Readonly"" Type=""Edm.Boolean"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""OrderFormSample"" Type=""Example.Database.OrderFormSample"" Nullable=""false""/>
				<NavigationProperty Name=""EntityField"" Type=""Example.Database.EntityField"" Nullable=""false""/>
				<NavigationProperty Name=""OrderForm"" Type=""Example.Database.OrderForm"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""OrderFormTest"">
				<Key>
					<PropertyRef Name=""OrderFormTestId""/>
				</Key>
				<Property Name=""OrderFormTestId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""OrderNumber"" Type=""Edm.Int64"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""OrderForm"" Type=""Example.Database.OrderForm"" Nullable=""false""/>
				<NavigationProperty Name=""TestType"" Type=""Example.Database.TestType"" Nullable=""false""/>
				<NavigationProperty Name=""OrderFormSample"" Type=""Example.Database.OrderFormSample"" Nullable=""false""/>
				<NavigationProperty Name=""Method"" Type=""Example.Database.Method"" Nullable=""false""/>
				<NavigationProperty Name=""Priority"" Type=""Example.Database.Priority"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""OrderFormReport"">
				<Key>
					<PropertyRef Name=""OrderFormReportId""/>
				</Key>
				<Property Name=""OrderFormReportId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<Property Name=""Language"" Type=""Edm.String""/>
				<Property Name=""SendToOrderContact"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""SendToClientContact"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""HasNotes"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""OrderForm"" Type=""Example.Database.OrderForm"" Nullable=""false""/>
				<NavigationProperty Name=""ReportType"" Type=""Example.Database.ReportType"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""OrderFormReportDestinations"" Type=""Collection(Example.Database.OrderFormReportDestination)""/>
			</EntityType>
			<EntityType Name=""OrderFormReportDestination"">
				<Key>
					<PropertyRef Name=""OrderFormReportDestinationId""/>
				</Key>
				<Property Name=""OrderFormReportDestinationId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""DestinationType"" Type=""Edm.String""/>
				<Property Name=""EMail"" Type=""Edm.String""/>
				<NavigationProperty Name=""OrderFormReport"" Type=""Example.Database.OrderFormReport"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ClientContact"" Type=""Example.Database.ClientContact"" Nullable=""false""/>
				<NavigationProperty Name=""User"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Printer"" Type=""Example.Database.Printer"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""PriceList"">
				<Key>
					<PropertyRef Name=""PriceListId""/>
				</Key>
				<Property Name=""PriceListId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Type"" Type=""Edm.String""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<Property Name=""StartDate"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""EndDate"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""HasNotes"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Client"" Type=""Example.Database.Client"" Nullable=""false""/>
				<NavigationProperty Name=""OrderType"" Type=""Example.Database.OrderType"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Clients"" Type=""Collection(Example.Database.Client)""/>
				<NavigationProperty Name=""OrderTypes"" Type=""Collection(Example.Database.OrderType)""/>
				<NavigationProperty Name=""Orders"" Type=""Collection(Example.Database.Order)""/>
				<NavigationProperty Name=""OrderForms"" Type=""Collection(Example.Database.OrderForm)""/>
				<NavigationProperty Name=""PriceListTestTypes"" Type=""Collection(Example.Database.PriceListTestType)""/>
				<NavigationProperty Name=""PriceListPriorities"" Type=""Collection(Example.Database.PriceListPriority)""/>
				<NavigationProperty Name=""InvoiceItems"" Type=""Collection(Example.Database.InvoiceItem)""/>
				<NavigationProperty Name=""Quotations"" Type=""Collection(Example.Database.Quotation)""/>
				<NavigationProperty Name=""Articles"" Type=""Collection(Example.Database.Article)""/>
			</EntityType>
			<EntityType Name=""PriceListTestType"">
				<Key>
					<PropertyRef Name=""PriceListTestTypeId""/>
				</Key>
				<Property Name=""PriceListTestTypeId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Amount"" Type=""Edm.String""/>
				<Property Name=""Price"" Type=""Edm.Double""/>
				<Property Name=""PriceRule"" Type=""Edm.String""/>
				<Property Name=""HasNotes"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""TestType"" Type=""Example.Database.TestType"" Nullable=""false""/>
				<NavigationProperty Name=""Method"" Type=""Example.Database.Method"" Nullable=""false""/>
				<NavigationProperty Name=""SampleType"" Type=""Example.Database.SampleType"" Nullable=""false""/>
				<NavigationProperty Name=""Product"" Type=""Example.Database.Product"" Nullable=""false""/>
				<NavigationProperty Name=""PriceList"" Type=""Example.Database.PriceList"" Nullable=""false""/>
				<NavigationProperty Name=""VatType"" Type=""Example.Database.VatType"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""InvoiceItems"" Type=""Collection(Example.Database.InvoiceItem)""/>
			</EntityType>
			<EntityType Name=""PriceListPriority"">
				<Key>
					<PropertyRef Name=""PriceListPriorityId""/>
				</Key>
				<Property Name=""PriceListPriorityId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Markup"" Type=""Edm.Double""/>
				<Property Name=""MarkupPercentage"" Type=""Edm.Double""/>
				<Property Name=""Discount"" Type=""Edm.Double""/>
				<Property Name=""DiscountPercentage"" Type=""Edm.Double""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<NavigationProperty Name=""PriceList"" Type=""Example.Database.PriceList"" Nullable=""false""/>
				<NavigationProperty Name=""Priority"" Type=""Example.Database.Priority"" Nullable=""false""/>
				<NavigationProperty Name=""VatType"" Type=""Example.Database.VatType"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""InvoiceItems"" Type=""Collection(Example.Database.InvoiceItem)""/>
			</EntityType>
			<EntityType Name=""AvailabilityRuleType"">
				<Key>
					<PropertyRef Name=""AvailabilityRuleTypeId""/>
				</Key>
				<Property Name=""AvailabilityRuleTypeId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<Property Name=""Data"" Type=""Edm.String""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""TimeStamp"" Type=""Edm.Binary""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""AvailabilityRules"" Type=""Collection(Example.Database.AvailabilityRule)""/>
			</EntityType>
			<EntityType Name=""AvailabilityRule"">
				<Key>
					<PropertyRef Name=""AvailabilityRuleId""/>
				</Key>
				<Property Name=""AvailabilityRuleId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<Property Name=""Data"" Type=""Edm.String""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""TimeStamp"" Type=""Edm.Binary""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""AvailabilityRuleType"" Type=""Example.Database.AvailabilityRuleType"" Nullable=""false""/>
				<NavigationProperty Name=""User"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""UserAvailabilitysByAvailibilityRule"" Type=""Collection(Example.Database.UserAvailability)""/>
			</EntityType>
			<EntityType Name=""ScheduleRule"">
				<Key>
					<PropertyRef Name=""ScheduleRuleId""/>
				</Key>
				<Property Name=""ScheduleRuleId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Date"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""DayPart"" Type=""Edm.String""/>
				<Property Name=""Comment"" Type=""Edm.String""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ScheduleType"" Type=""Example.Database.ScheduleType"" Nullable=""false""/>
				<NavigationProperty Name=""User"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Group"" Type=""Example.Database.Group"" Nullable=""false""/>
				<NavigationProperty Name=""ScheduleRuleSet"" Type=""Example.Database.ScheduleRuleSet"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""UserAvailability"">
				<Key>
					<PropertyRef Name=""UserAvailabilityId""/>
				</Key>
				<Property Name=""UserAvailabilityId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Date"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""DayPart"" Type=""Edm.String""/>
				<Property Name=""Availability"" Type=""Edm.Boolean"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""User"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""AvailibilityRule"" Type=""Example.Database.AvailabilityRule"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""TestTypeMethod"">
				<Key>
					<PropertyRef Name=""TestTypeMethodId""/>
				</Key>
				<Property Name=""TestTypeMethodId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""TestType"" Type=""Example.Database.TestType"" Nullable=""false""/>
				<NavigationProperty Name=""Method"" Type=""Example.Database.Method"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""Method"">
				<Key>
					<PropertyRef Name=""MethodId""/>
				</Key>
				<Property Name=""MethodId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<Property Name=""MethodSheetConfiguration"" Type=""Edm.String""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Text"" Type=""Example.Database.Text"" Nullable=""false""/>
				<NavigationProperty Name=""Tests"" Type=""Collection(Example.Database.Test)""/>
				<NavigationProperty Name=""TestTypes"" Type=""Collection(Example.Database.TestType)""/>
				<NavigationProperty Name=""TestTypeResultTypes"" Type=""Collection(Example.Database.TestTypeResultType)""/>
				<NavigationProperty Name=""OrderFormTests"" Type=""Collection(Example.Database.OrderFormTest)""/>
				<NavigationProperty Name=""PriceListTestTypes"" Type=""Collection(Example.Database.PriceListTestType)""/>
				<NavigationProperty Name=""TestTypeMethods"" Type=""Collection(Example.Database.TestTypeMethod)""/>
			</EntityType>
			<EntityType Name=""EntitySpecification"">
				<Key>
					<PropertyRef Name=""EntitySpecificationId""/>
				</Key>
				<Property Name=""EntitySpecificationId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ItemId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Type"" Type=""Edm.String""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<Property Name=""EntityName"" Type=""Edm.String""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Entity"" Type=""Example.Database.Entity"" Nullable=""false""/>
				<NavigationProperty Name=""Specification"" Type=""Example.Database.Specification"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""Specification"">
				<Key>
					<PropertyRef Name=""SpecificationId""/>
				</Key>
				<Property Name=""SpecificationId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Type"" Type=""Edm.String""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<Property Name=""SetReview"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""EntitySpecifications"" Type=""Collection(Example.Database.EntitySpecification)""/>
				<NavigationProperty Name=""SpecificationItems"" Type=""Collection(Example.Database.SpecificationItem)""/>
				<NavigationProperty Name=""SpecificationResults"" Type=""Collection(Example.Database.SpecificationResult)""/>
			</EntityType>
			<EntityType Name=""SpecificationItem"">
				<Key>
					<PropertyRef Name=""SpecificationItemId""/>
				</Key>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""LimitType"" Type=""Edm.String""/>
				<Property Name=""Type"" Type=""Edm.String""/>
				<Property Name=""SpecificationItemId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""MinimumValue"" Type=""Edm.Double""/>
				<Property Name=""MinimumValueOperator"" Type=""Edm.String""/>
				<Property Name=""ValidValues"" Type=""Edm.String""/>
				<Property Name=""InvalidValues"" Type=""Edm.String""/>
				<Property Name=""MaximumValue"" Type=""Edm.Double""/>
				<Property Name=""MaximumValueOperator"" Type=""Edm.String""/>
				<Property Name=""RangeType"" Type=""Edm.String""/>
				<Property Name=""RangeValue"" Type=""Edm.Double""/>
				<Property Name=""TargetValue"" Type=""Edm.Double""/>
				<Property Name=""TargetLowerValue"" Type=""Edm.Double""/>
				<Property Name=""TargetLowerValueOperator"" Type=""Edm.String""/>
				<Property Name=""TargetUpperValue"" Type=""Edm.Double""/>
				<Property Name=""TargetUpperValueOperator"" Type=""Edm.String""/>
				<Property Name=""OrderNumber"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Accredited"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""Scope"" Type=""Edm.String""/>
				<Property Name=""RoundResultsFirst"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""ValidFrom"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""ValidTo"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""Format"" Type=""Edm.String""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Specification"" Type=""Example.Database.Specification"" Nullable=""false""/>
				<NavigationProperty Name=""SampleType"" Type=""Example.Database.SampleType"" Nullable=""false""/>
				<NavigationProperty Name=""Product"" Type=""Example.Database.Product"" Nullable=""false""/>
				<NavigationProperty Name=""ProductType"" Type=""Example.Database.ProductType"" Nullable=""false""/>
				<NavigationProperty Name=""ResultType"" Type=""Example.Database.ResultType"" Nullable=""false""/>
				<NavigationProperty Name=""Unit"" Type=""Example.Database.Unit"" Nullable=""false""/>
				<NavigationProperty Name=""TargetResultType"" Type=""Example.Database.ResultType"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""SpecificationResults"" Type=""Collection(Example.Database.SpecificationResult)""/>
			</EntityType>
			<EntityType Name=""SpecificationResult"">
				<Key>
					<PropertyRef Name=""SpecificationResultId""/>
				</Key>
				<Property Name=""SpecificationResultId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Status"" Type=""Edm.String""/>
				<Property Name=""PartitionKey"" Type=""Edm.Int32""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Specification"" Type=""Example.Database.Specification"" Nullable=""false""/>
				<NavigationProperty Name=""SpecificationItem"" Type=""Example.Database.SpecificationItem"" Nullable=""false""/>
				<NavigationProperty Name=""Result"" Type=""Example.Database.Result"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""ClientUser"">
				<Key>
					<PropertyRef Name=""ClientUserId""/>
				</Key>
				<Property Name=""ClientUserId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Client"" Type=""Example.Database.Client"" Nullable=""false""/>
				<NavigationProperty Name=""User"" Type=""Example.Database.User"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""Invoice"">
				<Key>
					<PropertyRef Name=""InvoiceId""/>
				</Key>
				<Property Name=""InvoiceId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""TotalPrice"" Type=""Edm.Double""/>
				<Property Name=""TotalTax"" Type=""Edm.Double""/>
				<Property Name=""TotalDiscount"" Type=""Edm.Double""/>
				<Property Name=""TotalMarkup"" Type=""Edm.Double""/>
				<Property Name=""MarkupPercentage"" Type=""Edm.Double""/>
				<Property Name=""DiscountPercentage"" Type=""Edm.Double""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""InvoicedOn"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Status"" Type=""Edm.String""/>
				<Property Name=""DocumentGuid"" Type=""Edm.String""/>
				<Property Name=""CollectiveInvoice"" Type=""Edm.String""/>
				<Property Name=""Period"" Type=""Edm.Int64""/>
				<Property Name=""HasNotes"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""TimeStamp"" Type=""Edm.Binary""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Client"" Type=""Example.Database.Client"" Nullable=""false""/>
				<NavigationProperty Name=""File"" Type=""Example.Database.File"" Nullable=""false""/>
				<NavigationProperty Name=""ReportType"" Type=""Example.Database.ReportType"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""InvoiceOrders"" Type=""Collection(Example.Database.InvoiceOrder)""/>
				<NavigationProperty Name=""InvoiceItems"" Type=""Collection(Example.Database.InvoiceItem)""/>
			</EntityType>
			<EntityType Name=""InvoiceOrder"">
				<Key>
					<PropertyRef Name=""InvoiceOrderId""/>
				</Key>
				<Property Name=""InvoiceOrderId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""TotalPrice"" Type=""Edm.Double""/>
				<Property Name=""TotalTax"" Type=""Edm.Double""/>
				<Property Name=""TotalDiscount"" Type=""Edm.Double""/>
				<Property Name=""TotalMarkup"" Type=""Edm.Double""/>
				<Property Name=""MarkupPercentage"" Type=""Edm.Double""/>
				<Property Name=""DiscountPercentage"" Type=""Edm.Double""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""OrderDiscountPercentage"" Type=""Edm.Double""/>
				<Property Name=""ClientDiscountPercentage"" Type=""Edm.Double""/>
				<NavigationProperty Name=""Order"" Type=""Example.Database.Order"" Nullable=""false""/>
				<NavigationProperty Name=""Invoice"" Type=""Example.Database.Invoice"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""InvoiceItems"" Type=""Collection(Example.Database.InvoiceItem)""/>
			</EntityType>
			<EntityType Name=""InvoiceItem"">
				<Key>
					<PropertyRef Name=""InvoiceItemId""/>
				</Key>
				<Property Name=""InvoiceItemId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Price"" Type=""Edm.Double""/>
				<Property Name=""Tax"" Type=""Edm.Double""/>
				<Property Name=""Markup"" Type=""Edm.Double""/>
				<Property Name=""Discount"" Type=""Edm.Double""/>
				<Property Name=""PriceIncludingTax"" Type=""Edm.Double""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Type"" Type=""Edm.String""/>
				<Property Name=""OrderDiscount"" Type=""Edm.Double""/>
				<Property Name=""ClientDiscount"" Type=""Edm.Double""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""ItemId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""EntityName"" Type=""Edm.String""/>
				<NavigationProperty Name=""InvoiceOrder"" Type=""Example.Database.InvoiceOrder"" Nullable=""false""/>
				<NavigationProperty Name=""Invoice"" Type=""Example.Database.Invoice"" Nullable=""false""/>
				<NavigationProperty Name=""PriceList"" Type=""Example.Database.PriceList"" Nullable=""false""/>
				<NavigationProperty Name=""PriceListTestType"" Type=""Example.Database.PriceListTestType"" Nullable=""false""/>
				<NavigationProperty Name=""PriceListPriority"" Type=""Example.Database.PriceListPriority"" Nullable=""false""/>
				<NavigationProperty Name=""VatType"" Type=""Example.Database.VatType"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Entity"" Type=""Example.Database.Entity"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""VatType"">
				<Key>
					<PropertyRef Name=""VatTypeId""/>
				</Key>
				<Property Name=""VatTypeId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Percentage"" Type=""Edm.Double"" Nullable=""false""/>
				<Property Name=""CountryCode"" Type=""Edm.String""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""PriceListTestTypes"" Type=""Collection(Example.Database.PriceListTestType)""/>
				<NavigationProperty Name=""PriceListPriorities"" Type=""Collection(Example.Database.PriceListPriority)""/>
				<NavigationProperty Name=""InvoiceItems"" Type=""Collection(Example.Database.InvoiceItem)""/>
			</EntityType>
			<EntityType Name=""BatchItem"">
				<Key>
					<PropertyRef Name=""BatchItemId""/>
				</Key>
				<Property Name=""BatchItemId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Type"" Type=""Edm.String""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Material"" Type=""Example.Database.Material"" Nullable=""false""/>
				<NavigationProperty Name=""ParentBatch"" Type=""Example.Database.Batch"" Nullable=""false""/>
				<NavigationProperty Name=""ChildBatch"" Type=""Example.Database.Batch"" Nullable=""false""/>
				<NavigationProperty Name=""Materials"" Type=""Collection(Example.Database.Material)""/>
				<NavigationProperty Name=""Batches"" Type=""Collection(Example.Database.Batch)""/>
			</EntityType>
			<EntityType Name=""ContainerTypeField"">
				<Key>
					<PropertyRef Name=""ContainerTypeFieldId""/>
				</Key>
				<Property Name=""ContainerTypeFieldId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Value"" Type=""Edm.String""/>
				<Property Name=""OrderNumber"" Type=""Edm.Int64"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ContainerType"" Type=""Example.Database.ContainerType"" Nullable=""false""/>
				<NavigationProperty Name=""EntityField"" Type=""Example.Database.EntityField"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""Priority"">
				<Key>
					<PropertyRef Name=""PriorityId""/>
				</Key>
				<Property Name=""PriorityId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Type"" Type=""Edm.String""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""TimeStamp"" Type=""Edm.Binary""/>
				<Property Name=""BKDAmountOfDays"" Type=""Edm.Int64""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Tests"" Type=""Collection(Example.Database.Test)""/>
				<NavigationProperty Name=""Samples"" Type=""Collection(Example.Database.Sample)""/>
				<NavigationProperty Name=""Orders"" Type=""Collection(Example.Database.Order)""/>
				<NavigationProperty Name=""OrderFormTests"" Type=""Collection(Example.Database.OrderFormTest)""/>
				<NavigationProperty Name=""PriceListPriorities"" Type=""Collection(Example.Database.PriceListPriority)""/>
				<NavigationProperty Name=""TestTypePriorities"" Type=""Collection(Example.Database.TestTypePriority)""/>
				<NavigationProperty Name=""SuiteSamples"" Type=""Collection(Example.Database.SuiteSample)""/>
			</EntityType>
			<EntityType Name=""TestTypePriority"">
				<Key>
					<PropertyRef Name=""TestTypePriorityId""/>
				</Key>
				<Property Name=""TestTypePriorityId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Duration"" Type=""Edm.String""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Priority"" Type=""Example.Database.Priority"" Nullable=""false""/>
				<NavigationProperty Name=""TestType"" Type=""Example.Database.TestType"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""Laboratory"">
				<Key>
					<PropertyRef Name=""LaboratoryId""/>
				</Key>
				<Property Name=""LaboratoryId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<Property Name=""Code"" Type=""Edm.String""/>
				<Property Name=""ExternalCode"" Type=""Edm.String""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<NavigationProperty Name=""MappingLaboratory"" Type=""Example.Database.Laboratory"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Tests"" Type=""Collection(Example.Database.Test)""/>
				<NavigationProperty Name=""LaboratoriesByMappingLaboratory"" Type=""Collection(Example.Database.Laboratory)""/>
				<NavigationProperty Name=""AccreditationTestTypes"" Type=""Collection(Example.Database.AccreditationTestType)""/>
				<NavigationProperty Name=""LaboratorySampleTypes"" Type=""Collection(Example.Database.LaboratorySampleType)""/>
				<NavigationProperty Name=""LaboratoryTestTypes"" Type=""Collection(Example.Database.LaboratoryTestType)""/>
				<NavigationProperty Name=""LaboratoryUnits"" Type=""Collection(Example.Database.LaboratoryUnit)""/>
				<NavigationProperty Name=""LaboratoryResultTypes"" Type=""Collection(Example.Database.LaboratoryResultType)""/>
				<NavigationProperty Name=""LaboratoryProducts"" Type=""Collection(Example.Database.LaboratoryProduct)""/>
				<NavigationProperty Name=""LaboratoryProductTypes"" Type=""Collection(Example.Database.LaboratoryProductType)""/>
			</EntityType>
			<EntityType Name=""Quotation"">
				<Key>
					<PropertyRef Name=""QuotationId""/>
				</Key>
				<Property Name=""QuotationId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<Property Name=""Status"" Type=""Edm.String""/>
				<Property Name=""Code"" Type=""Edm.String""/>
				<Property Name=""AnimalType"" Type=""Edm.String""/>
				<Property Name=""Contact"" Type=""Edm.String""/>
				<Property Name=""ReferenceCode"" Type=""Edm.String""/>
				<Property Name=""Discount"" Type=""Edm.Double""/>
				<Property Name=""Remarks"" Type=""Edm.String""/>
				<Property Name=""StartDate"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""EndDate"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""TotalCost"" Type=""Edm.Double""/>
				<Property Name=""TotalAdditionalCosts"" Type=""Edm.Double""/>
				<Property Name=""TotalPrice"" Type=""Edm.Double""/>
				<Property Name=""Cancelled"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""CancelledOn"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""Authorised"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""AuthorisedOn"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""HasNotes"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""ExternalId"" Type=""Edm.Int64""/>
				<Property Name=""ExternalName"" Type=""Edm.String""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Client"" Type=""Example.Database.Client"" Nullable=""false""/>
				<NavigationProperty Name=""PriceList"" Type=""Example.Database.PriceList"" Nullable=""false""/>
				<NavigationProperty Name=""Experiment"" Type=""Example.Database.Experiment"" Nullable=""false""/>
				<NavigationProperty Name=""CancelledBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""AuthorisedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""QuotationLocations"" Type=""Collection(Example.Database.QuotationLocation)""/>
				<NavigationProperty Name=""QuotationTotals"" Type=""Collection(Example.Database.QuotationTotal)""/>
				<NavigationProperty Name=""QuotationItems"" Type=""Collection(Example.Database.QuotationItem)""/>
			</EntityType>
			<EntityType Name=""QuotationItemType"">
				<Key>
					<PropertyRef Name=""QuotationItemTypeId""/>
				</Key>
				<Property Name=""QuotationItemTypeId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""HasNotes"" Type=""Edm.Boolean"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""QuotationTotals"" Type=""Collection(Example.Database.QuotationTotal)""/>
				<NavigationProperty Name=""QuotationItems"" Type=""Collection(Example.Database.QuotationItem)""/>
			</EntityType>
			<EntityType Name=""Article"">
				<Key>
					<PropertyRef Name=""ArticleId""/>
				</Key>
				<Property Name=""ArticleId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<Property Name=""Costs"" Type=""Edm.Double""/>
				<Property Name=""AdditionalCosts"" Type=""Edm.Double""/>
				<Property Name=""Type"" Type=""Edm.String""/>
				<Property Name=""ItemId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""EntityName"" Type=""Edm.String""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""PriceList"" Type=""Example.Database.PriceList"" Nullable=""false""/>
				<NavigationProperty Name=""ArticleType"" Type=""Example.Database.ArticleType"" Nullable=""false""/>
				<NavigationProperty Name=""Entity"" Type=""Example.Database.Entity"" Nullable=""false""/>
				<NavigationProperty Name=""ArticleFields"" Type=""Collection(Example.Database.ArticleField)""/>
				<NavigationProperty Name=""QuotationLocations"" Type=""Collection(Example.Database.QuotationLocation)""/>
				<NavigationProperty Name=""QuotationItems"" Type=""Collection(Example.Database.QuotationItem)""/>
			</EntityType>
			<EntityType Name=""ArticleField"">
				<Key>
					<PropertyRef Name=""ArticleFieldId""/>
				</Key>
				<Property Name=""ArticleFieldId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Value"" Type=""Edm.String""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<NavigationProperty Name=""Article"" Type=""Example.Database.Article"" Nullable=""false""/>
				<NavigationProperty Name=""EntityField"" Type=""Example.Database.EntityField"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""QuotationLocation"">
				<Key>
					<PropertyRef Name=""QuotationLocationId""/>
				</Key>
				<Property Name=""QuotationLocationId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<Property Name=""StartDate"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""EndDate"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""PreparationDays"" Type=""Edm.Int64""/>
				<Property Name=""CleanupDays"" Type=""Edm.Int64""/>
				<Property Name=""NumberOfItems"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""NumberOfReplications"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""NumberOfPersons"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Duration"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""TotalAmount"" Type=""Edm.Int64""/>
				<Property Name=""Price"" Type=""Edm.Double""/>
				<Property Name=""AdditionalPrice"" Type=""Edm.Double""/>
				<Property Name=""Costs"" Type=""Edm.Double""/>
				<Property Name=""AdditionalCosts"" Type=""Edm.Double""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Quotation"" Type=""Example.Database.Quotation"" Nullable=""false""/>
				<NavigationProperty Name=""Location"" Type=""Example.Database.Location"" Nullable=""false""/>
				<NavigationProperty Name=""Schedule"" Type=""Example.Database.Schedule"" Nullable=""false""/>
				<NavigationProperty Name=""Article"" Type=""Example.Database.Article"" Nullable=""false""/>
				<NavigationProperty Name=""Experiment"" Type=""Example.Database.Experiment"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""QuotationTotal"">
				<Key>
					<PropertyRef Name=""QuotationTotalId""/>
				</Key>
				<Property Name=""QuotationTotalId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""TotalCosts"" Type=""Edm.Double""/>
				<Property Name=""TotalAdditionalCosts"" Type=""Edm.Double""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Quotation"" Type=""Example.Database.Quotation"" Nullable=""false""/>
				<NavigationProperty Name=""QuotationItemType"" Type=""Example.Database.QuotationItemType"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""QuotationItem"">
				<Key>
					<PropertyRef Name=""QuotationItemId""/>
				</Key>
				<Property Name=""QuotationItemId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<Property Name=""NumberOfItems"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""NumberOfReplications"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""NumberOfPersons"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Duration"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""TotalAmount"" Type=""Edm.Double""/>
				<Property Name=""Price"" Type=""Edm.Double""/>
				<Property Name=""AdditionalPrice"" Type=""Edm.Double""/>
				<Property Name=""Costs"" Type=""Edm.Double""/>
				<Property Name=""AdditionalCosts"" Type=""Edm.Double""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Quotation"" Type=""Example.Database.Quotation"" Nullable=""false""/>
				<NavigationProperty Name=""QuotationItemType"" Type=""Example.Database.QuotationItemType"" Nullable=""false""/>
				<NavigationProperty Name=""Article"" Type=""Example.Database.Article"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""SequenceTypeResultType"">
				<Key>
					<PropertyRef Name=""SequenceTypeResultTypeId""/>
				</Key>
				<Property Name=""SequenceTypeResultTypeId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""SequenceType"" Type=""Example.Database.SequenceType"" Nullable=""false""/>
				<NavigationProperty Name=""ResultType"" Type=""Example.Database.ResultType"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""ResultTypeEvent"">
				<Key>
					<PropertyRef Name=""ResultTypeEventId""/>
				</Key>
				<Property Name=""ResultTypeEventId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""EventType"" Type=""Edm.String""/>
				<Property Name=""Configuration"" Type=""Edm.String""/>
				<Property Name=""Condition"" Type=""Edm.String""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<NavigationProperty Name=""ResultType"" Type=""Example.Database.ResultType"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""TestTypeTask"">
				<Key>
					<PropertyRef Name=""TestTypeTaskId""/>
				</Key>
				<Property Name=""TestTypeTaskId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Type"" Type=""Edm.String""/>
				<Property Name=""OrderNumber"" Type=""Edm.Int64"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""TestType"" Type=""Example.Database.TestType"" Nullable=""false""/>
				<NavigationProperty Name=""SampleType"" Type=""Example.Database.SampleType"" Nullable=""false""/>
				<NavigationProperty Name=""RackType"" Type=""Example.Database.RackType"" Nullable=""false""/>
				<NavigationProperty Name=""InstrumentType"" Type=""Example.Database.InstrumentType"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""InstrumentPart"">
				<Key>
					<PropertyRef Name=""InstrumentPartId""/>
				</Key>
				<Property Name=""InstrumentPartId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Instrument"" Type=""Example.Database.Instrument"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""Accreditation"">
				<Key>
					<PropertyRef Name=""AccreditationId""/>
				</Key>
				<Property Name=""AccreditationId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<Property Name=""Symbol"" Type=""Edm.String""/>
				<Property Name=""StandardText"" Type=""Edm.String""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""AccreditationTestTypes"" Type=""Collection(Example.Database.AccreditationTestType)""/>
			</EntityType>
			<EntityType Name=""AccreditationTestType"">
				<Key>
					<PropertyRef Name=""AccreditationTestTypeId""/>
				</Key>
				<Property Name=""AccreditationTestTypeId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Accreditation"" Type=""Example.Database.Accreditation"" Nullable=""false""/>
				<NavigationProperty Name=""TestType"" Type=""Example.Database.TestType"" Nullable=""false""/>
				<NavigationProperty Name=""Laboratory"" Type=""Example.Database.Laboratory"" Nullable=""false""/>
				<NavigationProperty Name=""ResultType"" Type=""Example.Database.ResultType"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""SampleTypeTask"">
				<Key>
					<PropertyRef Name=""SampleTypeTaskId""/>
				</Key>
				<Property Name=""SampleTypeTaskId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Type"" Type=""Edm.String""/>
				<Property Name=""OrderNumber"" Type=""Edm.Int64"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""SampleType"" Type=""Example.Database.SampleType"" Nullable=""false""/>
				<NavigationProperty Name=""RackType"" Type=""Example.Database.RackType"" Nullable=""false""/>
				<NavigationProperty Name=""InstrumentType"" Type=""Example.Database.InstrumentType"" Nullable=""false""/>
				<NavigationProperty Name=""MaterialType"" Type=""Example.Database.MaterialType"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""TestTask"">
				<Key>
					<PropertyRef Name=""TestTaskId""/>
				</Key>
				<Property Name=""TestTaskId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Status"" Type=""Edm.String""/>
				<Property Name=""Type"" Type=""Edm.String""/>
				<Property Name=""DilutionRange"" Type=""Edm.String""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Test"" Type=""Example.Database.Test"" Nullable=""false""/>
				<NavigationProperty Name=""Sample"" Type=""Example.Database.Sample"" Nullable=""false""/>
				<NavigationProperty Name=""Rack"" Type=""Example.Database.Rack"" Nullable=""false""/>
				<NavigationProperty Name=""Instrument"" Type=""Example.Database.Instrument"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""SampleTask"">
				<Key>
					<PropertyRef Name=""SampleTaskId""/>
				</Key>
				<Property Name=""SampleTaskId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Status"" Type=""Edm.String""/>
				<Property Name=""Type"" Type=""Edm.String""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Sample"" Type=""Example.Database.Sample"" Nullable=""false""/>
				<NavigationProperty Name=""Rack"" Type=""Example.Database.Rack"" Nullable=""false""/>
				<NavigationProperty Name=""Instrument"" Type=""Example.Database.Instrument"" Nullable=""false""/>
				<NavigationProperty Name=""Material"" Type=""Example.Database.Material"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""TestTypeQualityControlTrigger"">
				<Key>
					<PropertyRef Name=""TestTypeQualityControlId""/>
				</Key>
				<Property Name=""TestTypeQualityControlId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""ResultTextValue"" Type=""Edm.String""/>
				<Property Name=""ResultNumberValue"" Type=""Edm.Double""/>
				<Property Name=""TriggerCount"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<NavigationProperty Name=""TestType"" Type=""Example.Database.TestType"" Nullable=""false""/>
				<NavigationProperty Name=""SampleType"" Type=""Example.Database.SampleType"" Nullable=""false""/>
				<NavigationProperty Name=""ResultType"" Type=""Example.Database.ResultType"" Nullable=""false""/>
				<NavigationProperty Name=""User"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ResultUnit"" Type=""Example.Database.Unit"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""ScheduleRuleSet"">
				<Key>
					<PropertyRef Name=""ScheduleRuleSetId""/>
				</Key>
				<Property Name=""ScheduleRuleSetId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""StartDate"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""EndDate"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ScheduleType"" Type=""Example.Database.ScheduleType"" Nullable=""false""/>
				<NavigationProperty Name=""User"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Group"" Type=""Example.Database.Group"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ScheduleRules"" Type=""Collection(Example.Database.ScheduleRule)""/>
			</EntityType>
			<EntityType Name=""LaboratorySampleType"">
				<Key>
					<PropertyRef Name=""LaboratorySampleTypeId""/>
				</Key>
				<Property Name=""LaboratorySampleTypeId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Ignore"" Type=""Edm.Boolean""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<NavigationProperty Name=""Laboratory"" Type=""Example.Database.Laboratory"" Nullable=""false""/>
				<NavigationProperty Name=""SampleType"" Type=""Example.Database.SampleType"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""LaboratoryTestType"">
				<Key>
					<PropertyRef Name=""LaboratoryTestTypeId""/>
				</Key>
				<Property Name=""LaboratoryTestTypeId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Ignore"" Type=""Edm.Boolean""/>
				<Property Name=""WorkflowName"" Type=""Edm.String""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<NavigationProperty Name=""Laboratory"" Type=""Example.Database.Laboratory"" Nullable=""false""/>
				<NavigationProperty Name=""TestType"" Type=""Example.Database.TestType"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""LaboratoryUnit"">
				<Key>
					<PropertyRef Name=""LaboratoryUnitId""/>
				</Key>
				<Property Name=""LaboratoryUnitId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Ignore"" Type=""Edm.Boolean""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<NavigationProperty Name=""Laboratory"" Type=""Example.Database.Laboratory"" Nullable=""false""/>
				<NavigationProperty Name=""Unit"" Type=""Example.Database.Unit"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""LaboratoryResultType"">
				<Key>
					<PropertyRef Name=""LaboratoryResultTypeId""/>
				</Key>
				<Property Name=""LaboratoryResultTypeId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Ignore"" Type=""Edm.Boolean""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<NavigationProperty Name=""Laboratory"" Type=""Example.Database.Laboratory"" Nullable=""false""/>
				<NavigationProperty Name=""ResultType"" Type=""Example.Database.ResultType"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""LaboratoryProduct"">
				<Key>
					<PropertyRef Name=""LaboratoryProductId""/>
				</Key>
				<Property Name=""LaboratoryProductId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Ignore"" Type=""Edm.Boolean""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<NavigationProperty Name=""Laboratory"" Type=""Example.Database.Laboratory"" Nullable=""false""/>
				<NavigationProperty Name=""Product"" Type=""Example.Database.Product"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""LaboratoryProductType"">
				<Key>
					<PropertyRef Name=""LaboratoryProductTypeId""/>
				</Key>
				<Property Name=""LaboratoryProductTypeId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Ignore"" Type=""Edm.Boolean""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<NavigationProperty Name=""Laboratory"" Type=""Example.Database.Laboratory"" Nullable=""false""/>
				<NavigationProperty Name=""ProductType"" Type=""Example.Database.ProductType"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""Suite"">
				<Key>
					<PropertyRef Name=""SuiteId""/>
				</Key>
				<Property Name=""SuiteId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<Property Name=""Status"" Type=""Edm.String""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Group"" Type=""Example.Database.Group"" Nullable=""false""/>
				<NavigationProperty Name=""SuiteTestTypes"" Type=""Collection(Example.Database.SuiteTestType)""/>
				<NavigationProperty Name=""SuiteOrderTaskTypes"" Type=""Collection(Example.Database.SuiteOrderTaskType)""/>
				<NavigationProperty Name=""SuiteOrderTasks"" Type=""Collection(Example.Database.SuiteOrderTask)""/>
				<NavigationProperty Name=""SuiteSamples"" Type=""Collection(Example.Database.SuiteSample)""/>
				<NavigationProperty Name=""SuiteTests"" Type=""Collection(Example.Database.SuiteTest)""/>
			</EntityType>
			<EntityType Name=""SuiteTestType"">
				<Key>
					<PropertyRef Name=""SuiteTestTypeId""/>
				</Key>
				<Property Name=""SuiteTestTypeId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""SuiteRule"" Type=""Edm.String""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<NavigationProperty Name=""Suite"" Type=""Example.Database.Suite"" Nullable=""false""/>
				<NavigationProperty Name=""TestType"" Type=""Example.Database.TestType"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""SuiteOrderTaskType"">
				<Key>
					<PropertyRef Name=""SuiteOrderTaskTypeId""/>
				</Key>
				<Property Name=""SuiteOrderTaskTypeId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<NavigationProperty Name=""Suite"" Type=""Example.Database.Suite"" Nullable=""false""/>
				<NavigationProperty Name=""OrderTaskType"" Type=""Example.Database.OrderTaskType"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""OrderTaskType"">
				<Key>
					<PropertyRef Name=""OrderTaskTypeId""/>
				</Key>
				<Property Name=""OrderTaskTypeId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<Property Name=""Code"" Type=""Edm.String""/>
				<Property Name=""Type"" Type=""Edm.String""/>
				<Property Name=""Data"" Type=""Edm.String""/>
				<Property Name=""ActiveFrom"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""ActiveUntil"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<NavigationProperty Name=""Title"" Type=""Example.Database.Text"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""SuiteOrderTaskTypes"" Type=""Collection(Example.Database.SuiteOrderTaskType)""/>
				<NavigationProperty Name=""OrderTasks"" Type=""Collection(Example.Database.OrderTask)""/>
			</EntityType>
			<EntityType Name=""SuiteOrderTask"">
				<Key>
					<PropertyRef Name=""SuiteOrderTaskId""/>
				</Key>
				<Property Name=""SuiteOrderTaskId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Status"" Type=""Edm.String""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Completed"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""CompletedOn"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""Cancelled"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""CancelledOn"" Type=""Edm.DateTimeOffset""/>
				<NavigationProperty Name=""Suite"" Type=""Example.Database.Suite"" Nullable=""false""/>
				<NavigationProperty Name=""OrderTask"" Type=""Example.Database.OrderTask"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""CompletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""CancelledBy"" Type=""Example.Database.User"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""SuiteSample"">
				<Key>
					<PropertyRef Name=""SuiteSampleId""/>
				</Key>
				<Property Name=""SuiteSampleId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Status"" Type=""Edm.String""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Completed"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""CompletedOn"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""Cancelled"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""CancelledOn"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""TimeStamp"" Type=""Edm.Binary""/>
				<Property Name=""PartitionKey"" Type=""Edm.Int32""/>
				<NavigationProperty Name=""Suite"" Type=""Example.Database.Suite"" Nullable=""false""/>
				<NavigationProperty Name=""Sample"" Type=""Example.Database.Sample"" Nullable=""false""/>
				<NavigationProperty Name=""Priority"" Type=""Example.Database.Priority"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""CompletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Group"" Type=""Example.Database.Group"" Nullable=""false""/>
				<NavigationProperty Name=""CancelledBy"" Type=""Example.Database.User"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""OrderTask"">
				<Key>
					<PropertyRef Name=""OrderTaskId""/>
				</Key>
				<Property Name=""OrderTaskId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Status"" Type=""Edm.String""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Completed"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""CompletedOn"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""TimeStamp"" Type=""Edm.Binary""/>
				<NavigationProperty Name=""Order"" Type=""Example.Database.Order"" Nullable=""false""/>
				<NavigationProperty Name=""OrderTaskType"" Type=""Example.Database.OrderTaskType"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""CompletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""SuiteOrderTasks"" Type=""Collection(Example.Database.SuiteOrderTask)""/>
				<NavigationProperty Name=""OrderTaskResults"" Type=""Collection(Example.Database.OrderTaskResult)""/>
			</EntityType>
			<EntityType Name=""OrderTaskResult"">
				<Key>
					<PropertyRef Name=""OrderTaskResultId""/>
				</Key>
				<Property Name=""OrderTaskResultId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""OrderNumber"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""DataType"" Type=""Edm.String""/>
				<Property Name=""NumberValue"" Type=""Edm.Double""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<NavigationProperty Name=""OrderTask"" Type=""Example.Database.OrderTask"" Nullable=""false""/>
				<NavigationProperty Name=""TextValue"" Type=""Example.Database.Text"" Nullable=""false""/>
				<NavigationProperty Name=""Unit"" Type=""Example.Database.Unit"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""OrderTaskResultDataPoints"" Type=""Collection(Example.Database.OrderTaskResultDataPoint)""/>
			</EntityType>
			<EntityType Name=""OrderTaskResultDataPoint"">
				<Key>
					<PropertyRef Name=""OrderTaskResultDataPointId""/>
				</Key>
				<Property Name=""OrderTaskResultDataPointId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""ItemId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""EntityName"" Type=""Edm.String""/>
				<NavigationProperty Name=""OrderTaskResult"" Type=""Example.Database.OrderTaskResult"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Entity"" Type=""Example.Database.Entity"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""SuiteTest"">
				<Key>
					<PropertyRef Name=""SuiteTestId""/>
				</Key>
				<Property Name=""SuiteTestId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""PartitionKey"" Type=""Edm.Int32""/>
				<NavigationProperty Name=""Suite"" Type=""Example.Database.Suite"" Nullable=""false""/>
				<NavigationProperty Name=""Test"" Type=""Example.Database.Test"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""ControlSample"">
				<Key>
					<PropertyRef Name=""ControlSampleId""/>
				</Key>
				<Property Name=""ControlSampleId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Type"" Type=""Edm.String""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<NavigationProperty Name=""Sample"" Type=""Example.Database.Sample"" Nullable=""false""/>
				<NavigationProperty Name=""ReferenceSample"" Type=""Example.Database.Sample"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""ArticleType"">
				<Key>
					<PropertyRef Name=""ArticleTypeId""/>
				</Key>
				<Property Name=""ArticleTypeId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ItemId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""EntityName"" Type=""Edm.String""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Entity"" Type=""Example.Database.Entity"" Nullable=""false""/>
				<NavigationProperty Name=""Articles"" Type=""Collection(Example.Database.Article)""/>
				<NavigationProperty Name=""ArticleTypeFields"" Type=""Collection(Example.Database.ArticleTypeField)""/>
			</EntityType>
			<EntityType Name=""ArticleTypeField"">
				<Key>
					<PropertyRef Name=""ArticleTypeFieldId""/>
				</Key>
				<Property Name=""ArticleTypeFieldId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""TextValue"" Type=""Edm.String""/>
				<Property Name=""NumberValue"" Type=""Edm.Double""/>
				<Property Name=""DateValue"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""ItemId"" Type=""Edm.Int64""/>
				<Property Name=""EntityName"" Type=""Edm.String""/>
				<NavigationProperty Name=""ArticleType"" Type=""Example.Database.ArticleType"" Nullable=""false""/>
				<NavigationProperty Name=""EntityField"" Type=""Example.Database.EntityField"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""Complaint"">
				<Key>
					<PropertyRef Name=""ComplaintId""/>
				</Key>
				<Property Name=""ComplaintId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<Property Name=""Type"" Type=""Edm.String""/>
				<Property Name=""Status"" Type=""Edm.String""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""TimeStamp"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""HasNotes"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<NavigationProperty Name=""Order"" Type=""Example.Database.Order"" Nullable=""false""/>
				<NavigationProperty Name=""Product"" Type=""Example.Database.Product"" Nullable=""false""/>
				<NavigationProperty Name=""Sample"" Type=""Example.Database.Sample"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""CorrectiveActions"" Type=""Collection(Example.Database.CorrectiveAction)""/>
				<NavigationProperty Name=""ComplaintCorrectiveActions"" Type=""Collection(Example.Database.ComplaintCorrectiveAction)""/>
			</EntityType>
			<EntityType Name=""CorrectiveAction"">
				<Key>
					<PropertyRef Name=""CorrectiveActionId""/>
				</Key>
				<Property Name=""CorrectiveActionId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<Property Name=""Status"" Type=""Edm.String""/>
				<Property Name=""Type"" Type=""Edm.String""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""TimeStamp"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<NavigationProperty Name=""Complaint"" Type=""Example.Database.Complaint"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ComplaintCorrectiveActions"" Type=""Collection(Example.Database.ComplaintCorrectiveAction)""/>
			</EntityType>
			<EntityType Name=""ComplaintCorrectiveAction"">
				<Key>
					<PropertyRef Name=""ComplaintCorrectiveActionId""/>
				</Key>
				<Property Name=""ComplaintCorrectiveActionId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""TimeStamp"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<NavigationProperty Name=""Complaint"" Type=""Example.Database.Complaint"" Nullable=""false""/>
				<NavigationProperty Name=""CorrectiveAction"" Type=""Example.Database.CorrectiveAction"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""ProjectTaskType"">
				<Key>
					<PropertyRef Name=""ProjectTaskTypeId""/>
				</Key>
				<Property Name=""ProjectTaskTypeId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Description"" Type=""Edm.String""/>
				<Property Name=""Code"" Type=""Edm.String""/>
				<Property Name=""Type"" Type=""Edm.String""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Deleted"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""DeletedOn"" Type=""Edm.DateTimeOffset""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""DeletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ProjectTasks"" Type=""Collection(Example.Database.ProjectTask)""/>
			</EntityType>
			<EntityType Name=""ProjectTask"">
				<Key>
					<PropertyRef Name=""ProjectTaskId""/>
				</Key>
				<Property Name=""ProjectTaskId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Status"" Type=""Edm.String""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Completed"" Type=""Edm.Boolean"" Nullable=""false""/>
				<Property Name=""CompletedOn"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""TimeStamp"" Type=""Edm.Binary""/>
				<Property Name=""BKDTitle"" Type=""Edm.String""/>
				<Property Name=""BKDDescription"" Type=""Edm.String""/>
				<Property Name=""BKDGroup"" Type=""Edm.String""/>
				<Property Name=""BKDPriority"" Type=""Edm.String""/>
				<NavigationProperty Name=""Project"" Type=""Example.Database.Project"" Nullable=""false""/>
				<NavigationProperty Name=""ProjectTaskType"" Type=""Example.Database.ProjectTaskType"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""CompletedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""BKDAssingedUser"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""BKDProjectTaskComments"" Type=""Collection(Example.Database.BKDProjectTaskComment)""/>
			</EntityType>
			<EntityType Name=""BKDCrop"">
				<Key>
					<PropertyRef Name=""CropId""/>
				</Key>
				<Property Name=""CropId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""EdibulbId"" Type=""Edm.String""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""MasterDataId"" Type=""Edm.Int64""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""BKDSamples"" Type=""Collection(Example.Database.Sample)""/>
				<NavigationProperty Name=""BKDCultivars"" Type=""Collection(Example.Database.BKDCultivar)""/>
				<NavigationProperty Name=""BKDProjectRules"" Type=""Collection(Example.Database.BKDProjectRule)""/>
				<NavigationProperty Name=""BKDCropMaterialIncubation"" Type=""Collection(Example.Database.BKDCropMaterialIncubation)""/>
			</EntityType>
			<EntityType Name=""BKDCultivar"">
				<Key>
					<PropertyRef Name=""CultivarId""/>
				</Key>
				<Property Name=""CultivarId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""EdibulbId"" Type=""Edm.String""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""MasterDataId"" Type=""Edm.Int64""/>
				<NavigationProperty Name=""Crop"" Type=""Example.Database.BKDCrop"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""BKDSamples"" Type=""Collection(Example.Database.Sample)""/>
			</EntityType>
			<EntityType Name=""BKDPlantMaterial"">
				<Key>
					<PropertyRef Name=""PlantMaterialId""/>
				</Key>
				<Property Name=""PlantMaterialId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""BKDSamples"" Type=""Collection(Example.Database.Sample)""/>
				<NavigationProperty Name=""BKDProjectRules"" Type=""Collection(Example.Database.BKDProjectRule)""/>
				<NavigationProperty Name=""BKDCropMaterialIncubation"" Type=""Collection(Example.Database.BKDCropMaterialIncubation)""/>
			</EntityType>
			<EntityType Name=""BKDTestAmount"">
				<Key>
					<PropertyRef Name=""TestAmountId""/>
				</Key>
				<Property Name=""TestAmountId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""Type"" Type=""Edm.String""/>
				<Property Name=""Amount"" Type=""Edm.Double""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""BKDSamples"" Type=""Collection(Example.Database.Sample)""/>
			</EntityType>
			<EntityType Name=""BKDSampleRemark"">
				<Key>
					<PropertyRef Name=""SampleRemarkId""/>
				</Key>
				<Property Name=""SampleRemarkId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Remark"" Type=""Edm.String""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Sample"" Type=""Example.Database.Sample"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""BKDProjectTaskComment"">
				<Key>
					<PropertyRef Name=""ProjectTaskCommentId""/>
				</Key>
				<Property Name=""ProjectTaskCommentId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Comment"" Type=""Edm.String""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<NavigationProperty Name=""ProjectTask"" Type=""Example.Database.ProjectTask"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""BKDProjectRule"">
				<Key>
					<PropertyRef Name=""ProjectRuleId""/>
				</Key>
				<Property Name=""ProjectRuleId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Name"" Type=""Edm.String""/>
				<Property Name=""StartDate"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""EndDate"" Type=""Edm.DateTimeOffset""/>
				<Property Name=""Counter"" Type=""Edm.Int64""/>
				<Property Name=""MaxPerDay"" Type=""Edm.Int64""/>
				<Property Name=""AddEvery"" Type=""Edm.Int64""/>
				<Property Name=""Max"" Type=""Edm.Int64""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<NavigationProperty Name=""PlantMaterial"" Type=""Example.Database.BKDPlantMaterial"" Nullable=""false""/>
				<NavigationProperty Name=""Crop"" Type=""Example.Database.BKDCrop"" Nullable=""false""/>
				<NavigationProperty Name=""Virus"" Type=""Example.Database.TestType"" Nullable=""false""/>
				<NavigationProperty Name=""SampleType"" Type=""Example.Database.SampleType"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""Project"" Type=""Example.Database.Project"" Nullable=""false""/>
				<NavigationProperty Name=""BKDTestsByCreatedFromProjectRule"" Type=""Collection(Example.Database.Test)""/>
				<NavigationProperty Name=""BKDSamplesByCreatedFromProjectRule"" Type=""Collection(Example.Database.Sample)""/>
			</EntityType>
			<EntityType Name=""BKDIncubationBatch"">
				<Key>
					<PropertyRef Name=""IncubationBatchId""/>
				</Key>
				<Property Name=""IncubationBatchId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Dilution"" Type=""Edm.String""/>
				<Property Name=""AmountOfPlates"" Type=""Edm.Int64""/>
				<Property Name=""AmountOfContainers"" Type=""Edm.Int64""/>
				<Property Name=""BufferPerContainer"" Type=""Edm.Double""/>
				<Property Name=""ConjugatePerContainer"" Type=""Edm.Double""/>
				<Property Name=""MilkpowderPerContainer"" Type=""Edm.Double""/>
				<Property Name=""NPSPerContainer"" Type=""Edm.Double""/>
				<Property Name=""SubstratePerContainer"" Type=""Edm.Double""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""Type"" Type=""Edm.String""/>
				<Property Name=""Status"" Type=""Edm.String""/>
				<Property Name=""MadeOn"" Type=""Edm.DateTimeOffset""/>
				<NavigationProperty Name=""TestType"" Type=""Example.Database.TestType"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""MadeBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""BKDRacks"" Type=""Collection(Example.Database.Rack)""/>
				<NavigationProperty Name=""BKDRacksBySubstrateBatch"" Type=""Collection(Example.Database.Rack)""/>
				<NavigationProperty Name=""BKDIncubationBatchUsers"" Type=""Collection(Example.Database.BKDIncubationBatchUser)""/>
			</EntityType>
			<EntityType Name=""BKDIncubationBatchUser"">
				<Key>
					<PropertyRef Name=""IncubationBatchUserId""/>
				</Key>
				<Property Name=""IncubationBatchUserId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<NavigationProperty Name=""IncubationBatch"" Type=""Example.Database.BKDIncubationBatch"" Nullable=""false""/>
				<NavigationProperty Name=""User"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""BKDIncubationFormulation"">
				<Key>
					<PropertyRef Name=""IncubationFormulationId""/>
				</Key>
				<Property Name=""IncubationFormulationId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""Type"" Type=""Edm.String""/>
				<Property Name=""DilutionAmountCold"" Type=""Edm.Int64""/>
				<Property Name=""DilutionAmountWarm"" Type=""Edm.Int64""/>
				<Property Name=""Amount"" Type=""Edm.Double""/>
				<Property Name=""PlateVolume"" Type=""Edm.Double""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<NavigationProperty Name=""TestType"" Type=""Example.Database.TestType"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
			</EntityType>
			<EntityType Name=""BKDCropMaterialIncubation"">
				<Key>
					<PropertyRef Name=""CropMaterialIncubationId""/>
				</Key>
				<Property Name=""CropMaterialIncubationId"" Type=""Edm.Int64"" Nullable=""false""/>
				<Property Name=""IncubationAfterPippeting"" Type=""Edm.Int64""/>
				<Property Name=""Conjugate"" Type=""Edm.String""/>
				<Property Name=""IncubationTime"" Type=""Edm.Int64""/>
				<Property Name=""ModifiedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<Property Name=""CreatedOn"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
				<NavigationProperty Name=""Crop"" Type=""Example.Database.BKDCrop"" Nullable=""false""/>
				<NavigationProperty Name=""PlantMaterial"" Type=""Example.Database.BKDPlantMaterial"" Nullable=""false""/>
				<NavigationProperty Name=""ModifiedBy"" Type=""Example.Database.User"" Nullable=""false""/>
				<NavigationProperty Name=""CreatedBy"" Type=""Example.Database.User"" Nullable=""false""/>
			</EntityType>
			<EntityContainer Name=""iLes"">
				<EntitySet Name=""Users"" EntityType=""Example.Database.User""/>
				<EntitySet Name=""AccountProvider"" EntityType=""Example.Database.AccountProvider""/>
				<EntitySet Name=""Accounts"" EntityType=""Example.Database.Account""/>
				<EntitySet Name=""Tables"" EntityType=""Example.Database.Table""/>
				<EntitySet Name=""Fields"" EntityType=""Example.Database.Field""/>
				<EntitySet Name=""Texts"" EntityType=""Example.Database.Text""/>
				<EntitySet Name=""Entities"" EntityType=""Example.Database.Entity""/>
				<EntitySet Name=""EntityUsers"" EntityType=""Example.Database.EntityUser""/>
				<EntitySet Name=""EntityTypeEvents"" EntityType=""Example.Database.EntityTypeEvent""/>
				<EntitySet Name=""EntityEvents"" EntityType=""Example.Database.EntityEvent""/>
				<EntitySet Name=""ItemEvents"" EntityType=""Example.Database.ItemEvent""/>
				<EntitySet Name=""Phrases"" EntityType=""Example.Database.Phrase""/>
				<EntitySet Name=""PhraseItems"" EntityType=""Example.Database.PhraseItem""/>
				<EntitySet Name=""EntityFields"" EntityType=""Example.Database.EntityField""/>
				<EntitySet Name=""EntityFieldHistories"" EntityType=""Example.Database.EntityFieldHistory""/>
				<EntitySet Name=""EntityPages"" EntityType=""Example.Database.EntityPage""/>
				<EntitySet Name=""Roles"" EntityType=""Example.Database.Role""/>
				<EntitySet Name=""UserRoles"" EntityType=""Example.Database.UserRole""/>
				<EntitySet Name=""Sessions"" EntityType=""Example.Database.Session""/>
				<EntitySet Name=""Addresses"" EntityType=""Example.Database.Address""/>
				<EntitySet Name=""Notes"" EntityType=""Example.Database.Note""/>
				<EntitySet Name=""Operations"" EntityType=""Example.Database.Operation""/>
				<EntitySet Name=""OperationUsers"" EntityType=""Example.Database.OperationUser""/>
				<EntitySet Name=""Tasks"" EntityType=""Example.Database.Task""/>
				<EntitySet Name=""RoleTasks"" EntityType=""Example.Database.RoleTask""/>
				<EntitySet Name=""TaskOperations"" EntityType=""Example.Database.TaskOperation""/>
				<EntitySet Name=""StoragePolicies"" EntityType=""Example.Database.StoragePolicy""/>
				<EntitySet Name=""FileTypes"" EntityType=""Example.Database.FileType""/>
				<EntitySet Name=""FileTemplates"" EntityType=""Example.Database.FileTemplate""/>
				<EntitySet Name=""FileGroups"" EntityType=""Example.Database.FileGroup""/>
				<EntitySet Name=""Files"" EntityType=""Example.Database.File""/>
				<EntitySet Name=""UserRequests"" EntityType=""Example.Database.UserRequest""/>
				<EntitySet Name=""UrlActions"" EntityType=""Example.Database.UrlAction""/>
				<EntitySet Name=""Forms"" EntityType=""Example.Database.Form""/>
				<EntitySet Name=""FormFields"" EntityType=""Example.Database.FormField""/>
				<EntitySet Name=""EntityForms"" EntityType=""Example.Database.EntityForm""/>
				<EntitySet Name=""OutgoingMessages"" EntityType=""Example.Database.OutgoingMessage""/>
				<EntitySet Name=""FormTemplates"" EntityType=""Example.Database.FormTemplate""/>
				<EntitySet Name=""FormTemplateFields"" EntityType=""Example.Database.FormTemplateField""/>
				<EntitySet Name=""EntityFormTemplates"" EntityType=""Example.Database.EntityFormTemplate""/>
				<EntitySet Name=""Exceptions"" EntityType=""Example.Database.Exception""/>
				<EntitySet Name=""ExceptionParameters"" EntityType=""Example.Database.ExceptionParameter""/>
				<EntitySet Name=""FormTemplateVersions"" EntityType=""Example.Database.FormTemplateVersion""/>
				<EntitySet Name=""FormTemplateSections"" EntityType=""Example.Database.FormTemplateSection""/>
				<EntitySet Name=""Modules"" EntityType=""Example.Database.Module""/>
				<EntitySet Name=""Folders"" EntityType=""Example.Database.Folder""/>
				<EntitySet Name=""FolderFilters"" EntityType=""Example.Database.FolderFilter""/>
				<EntitySet Name=""Actions"" EntityType=""Example.Database.Action""/>
				<EntitySet Name=""ActionParameters"" EntityType=""Example.Database.ActionParameter""/>
				<EntitySet Name=""FolderEntities"" EntityType=""Example.Database.FolderEntity""/>
				<EntitySet Name=""Applications"" EntityType=""Example.Database.Application""/>
				<EntitySet Name=""Menus"" EntityType=""Example.Database.Menu""/>
				<EntitySet Name=""Audits"" EntityType=""Example.Database.Audit""/>
				<EntitySet Name=""AuditItems"" EntityType=""Example.Database.AuditItem""/>
				<EntitySet Name=""AuditFields"" EntityType=""Example.Database.AuditField""/>
				<EntitySet Name=""ElectronicSignatureTypes"" EntityType=""Example.Database.ElectronicSignatureType""/>
				<EntitySet Name=""ElectronicSignatureTypeFields"" EntityType=""Example.Database.ElectronicSignatureTypeField""/>
				<EntitySet Name=""ElectronicSignatures"" EntityType=""Example.Database.ElectronicSignature""/>
				<EntitySet Name=""ElectronicSignatureFields"" EntityType=""Example.Database.ElectronicSignatureField""/>
				<EntitySet Name=""Settings"" EntityType=""Example.Database.Setting""/>
				<EntitySet Name=""ReportTypes"" EntityType=""Example.Database.ReportType""/>
				<EntitySet Name=""ReportTypeFormats"" EntityType=""Example.Database.ReportTypeFormat""/>
				<EntitySet Name=""ReportTypeArguments"" EntityType=""Example.Database.ReportTypeArgument""/>
				<EntitySet Name=""ReportTypeFiles"" EntityType=""Example.Database.ReportTypeFile""/>
				<EntitySet Name=""MembershipProviders"" EntityType=""Example.Database.MembershipProvider""/>
				<EntitySet Name=""DataProviders"" EntityType=""Example.Database.DataProvider""/>
				<EntitySet Name=""EntityActions"" EntityType=""Example.Database.EntityAction""/>
				<EntitySet Name=""UserSettings"" EntityType=""Example.Database.UserSetting""/>
				<EntitySet Name=""Devices"" EntityType=""Example.Database.Device""/>
				<EntitySet Name=""Locations"" EntityType=""Example.Database.Location""/>
				<EntitySet Name=""LocationTypes"" EntityType=""Example.Database.LocationType""/>
				<EntitySet Name=""Queries"" EntityType=""Example.Database.Query""/>
				<EntitySet Name=""QueryExpressions"" EntityType=""Example.Database.QueryExpression""/>
				<EntitySet Name=""TimestampValues"" EntityType=""Example.Database.TimestampValue""/>
				<EntitySet Name=""DocumentTypes"" EntityType=""Example.Database.DocumentType""/>
				<EntitySet Name=""DocumentIndices"" EntityType=""Example.Database.DocumentIndex""/>
				<EntitySet Name=""Documents"" EntityType=""Example.Database.Document""/>
				<EntitySet Name=""DocumentVersions"" EntityType=""Example.Database.DocumentVersion""/>
				<EntitySet Name=""ChangeRequests"" EntityType=""Example.Database.ChangeRequest""/>
				<EntitySet Name=""ChangeRequestTypes"" EntityType=""Example.Database.ChangeRequestType""/>
				<EntitySet Name=""Counters"" EntityType=""Example.Database.Counter""/>
				<EntitySet Name=""Properties"" EntityType=""Example.Database.Property""/>
				<EntitySet Name=""EntityHierarchys"" EntityType=""Example.Database.EntityHierarchy""/>
				<EntitySet Name=""Printers"" EntityType=""Example.Database.Printer""/>
				<EntitySet Name=""Countries"" EntityType=""Example.Database.Country""/>
				<EntitySet Name=""NewsItems"" EntityType=""Example.Database.NewsItem""/>
				<EntitySet Name=""TypeBindings"" EntityType=""Example.Database.TypeBinding""/>
				<EntitySet Name=""DeviceTasks"" EntityType=""Example.Database.DeviceTask""/>
				<EntitySet Name=""UpgradeStates"" EntityType=""Example.Database.UpgradeState""/>
				<EntitySet Name=""Groups"" EntityType=""Example.Database.Group""/>
				<EntitySet Name=""UserGroups"" EntityType=""Example.Database.UserGroup""/>
				<EntitySet Name=""EntityFieldRules"" EntityType=""Example.Database.EntityFieldRule""/>
				<EntitySet Name=""SystemLogs"" EntityType=""Example.Database.SystemLog""/>
				<EntitySet Name=""SystemLogItems"" EntityType=""Example.Database.SystemLogItem""/>
				<EntitySet Name=""Fonts"" EntityType=""Example.Database.Font""/>
				<EntitySet Name=""CheckListTypes"" EntityType=""Example.Database.CheckListType""/>
				<EntitySet Name=""CheckListTypeItems"" EntityType=""Example.Database.CheckListTypeItem""/>
				<EntitySet Name=""CheckLists"" EntityType=""Example.Database.CheckList""/>
				<EntitySet Name=""CheckListItems"" EntityType=""Example.Database.CheckListItem""/>
				<EntitySet Name=""LocationItems"" EntityType=""Example.Database.LocationItem""/>
				<EntitySet Name=""LocationItemTypes"" EntityType=""Example.Database.LocationItemType""/>
				<EntitySet Name=""Installations"" EntityType=""Example.Database.Installation""/>
				<EntitySet Name=""InstallationLogs"" EntityType=""Example.Database.InstallationLog""/>
				<EntitySet Name=""InstallationPackages"" EntityType=""Example.Database.InstallationPackage""/>
				<EntitySet Name=""ConfigurationSystems"" EntityType=""Example.Database.ConfigurationSystem""/>
				<EntitySet Name=""ConfigurationInstances"" EntityType=""Example.Database.ConfigurationInstance""/>
				<EntitySet Name=""ConfigurationSystemMonitorings"" EntityType=""Example.Database.ConfigurationSystemMonitoring""/>
				<EntitySet Name=""Credentials"" EntityType=""Example.Database.Credential""/>
				<EntitySet Name=""JobPositions"" EntityType=""Example.Database.JobPosition""/>
				<EntitySet Name=""Trainings"" EntityType=""Example.Database.Training""/>
				<EntitySet Name=""JobPositionTrainings"" EntityType=""Example.Database.JobPositionTraining""/>
				<EntitySet Name=""UserTrainings"" EntityType=""Example.Database.UserTraining""/>
				<EntitySet Name=""UserJobPositions"" EntityType=""Example.Database.UserJobPosition""/>
				<EntitySet Name=""SchemaFiles"" EntityType=""Example.Database.SchemaFile""/>
				<EntitySet Name=""RecordLocks"" EntityType=""Example.Database.RecordLock""/>
				<EntitySet Name=""Subscriptions"" EntityType=""Example.Database.Subscription""/>
				<EntitySet Name=""SubscriptionSubjects"" EntityType=""Example.Database.SubscriptionSubject""/>
				<EntitySet Name=""SubscriptionManagers"" EntityType=""Example.Database.SubscriptionManager""/>
				<EntitySet Name=""SubscriptionNotifications"" EntityType=""Example.Database.SubscriptionNotification""/>
				<EntitySet Name=""LicenseUsers"" EntityType=""Example.Database.LicenseUser""/>
				<EntitySet Name=""ConfigurationSystemLicenses"" EntityType=""Example.Database.ConfigurationSystemLicense""/>
				<EntitySet Name=""ProcessStationInstruments"" EntityType=""Example.Database.ProcessStationInstrument""/>
				<EntitySet Name=""Clients"" EntityType=""Example.Database.Client""/>
				<EntitySet Name=""ClientContacts"" EntityType=""Example.Database.ClientContact""/>
				<EntitySet Name=""Projects"" EntityType=""Example.Database.Project""/>
				<EntitySet Name=""ExperimentTypes"" EntityType=""Example.Database.ExperimentType""/>
				<EntitySet Name=""ProjectTypes"" EntityType=""Example.Database.ProjectType""/>
				<EntitySet Name=""Experiments"" EntityType=""Example.Database.Experiment""/>
				<EntitySet Name=""ProjectMembers"" EntityType=""Example.Database.ProjectMember""/>
				<EntitySet Name=""ExperimentMembers"" EntityType=""Example.Database.ExperimentMember""/>
				<EntitySet Name=""SlotTypes"" EntityType=""Example.Database.SlotType""/>
				<EntitySet Name=""Slots"" EntityType=""Example.Database.Slot""/>
				<EntitySet Name=""ScheduleTypes"" EntityType=""Example.Database.ScheduleType""/>
				<EntitySet Name=""Schedules"" EntityType=""Example.Database.Schedule""/>
				<EntitySet Name=""Tests"" EntityType=""Example.Database.Test""/>
				<EntitySet Name=""Units"" EntityType=""Example.Database.Unit""/>
				<EntitySet Name=""ResultTypes"" EntityType=""Example.Database.ResultType""/>
				<EntitySet Name=""Results"" EntityType=""Example.Database.Result""/>
				<EntitySet Name=""ResultTypeArguments"" EntityType=""Example.Database.ResultTypeArgument""/>
				<EntitySet Name=""SlotTypeProperties"" EntityType=""Example.Database.SlotTypeProperty""/>
				<EntitySet Name=""ScheduleTypeSlotTypes"" EntityType=""Example.Database.ScheduleTypeSlotType""/>
				<EntitySet Name=""ScheduleItems"" EntityType=""Example.Database.ScheduleItem""/>
				<EntitySet Name=""SlotItems"" EntityType=""Example.Database.SlotItem""/>
				<EntitySet Name=""SampleTypes"" EntityType=""Example.Database.SampleType""/>
				<EntitySet Name=""Samples"" EntityType=""Example.Database.Sample""/>
				<EntitySet Name=""TestTypes"" EntityType=""Example.Database.TestType""/>
				<EntitySet Name=""TestTypeReportTypes"" EntityType=""Example.Database.TestTypeReportType""/>
				<EntitySet Name=""OrderTypeFields"" EntityType=""Example.Database.OrderTypeField""/>
				<EntitySet Name=""SampleTypeFields"" EntityType=""Example.Database.SampleTypeField""/>
				<EntitySet Name=""TestTypeFields"" EntityType=""Example.Database.TestTypeField""/>
				<EntitySet Name=""ResultTypeFields"" EntityType=""Example.Database.ResultTypeField""/>
				<EntitySet Name=""AdditionalTestTypes"" EntityType=""Example.Database.AdditionalTestType""/>
				<EntitySet Name=""SlotProperties"" EntityType=""Example.Database.SlotProperty""/>
				<EntitySet Name=""OrderTypes"" EntityType=""Example.Database.OrderType""/>
				<EntitySet Name=""Orders"" EntityType=""Example.Database.Order""/>
				<EntitySet Name=""OrderReports"" EntityType=""Example.Database.OrderReport""/>
				<EntitySet Name=""OrderReportDestinations"" EntityType=""Example.Database.OrderReportDestination""/>
				<EntitySet Name=""ClientReportTypes"" EntityType=""Example.Database.ClientReportType""/>
				<EntitySet Name=""ProjectDocuments"" EntityType=""Example.Database.ProjectDocument""/>
				<EntitySet Name=""ContainerTypes"" EntityType=""Example.Database.ContainerType""/>
				<EntitySet Name=""Containers"" EntityType=""Example.Database.Container""/>
				<EntitySet Name=""SampleContainers"" EntityType=""Example.Database.SampleContainer""/>
				<EntitySet Name=""SampleTypeContainerTypes"" EntityType=""Example.Database.SampleTypeContainerType""/>
				<EntitySet Name=""GraphTypes"" EntityType=""Example.Database.GraphType""/>
				<EntitySet Name=""GraphTypeSeries"" EntityType=""Example.Database.GraphTypeSerie""/>
				<EntitySet Name=""RackTypes"" EntityType=""Example.Database.RackType""/>
				<EntitySet Name=""Racks"" EntityType=""Example.Database.Rack""/>
				<EntitySet Name=""RackSamples"" EntityType=""Example.Database.RackSample""/>
				<EntitySet Name=""ProcessStationTypes"" EntityType=""Example.Database.ProcessStationType""/>
				<EntitySet Name=""ProcessStations"" EntityType=""Example.Database.ProcessStation""/>
				<EntitySet Name=""SampleTypeTestTypes"" EntityType=""Example.Database.SampleTypeTestType""/>
				<EntitySet Name=""OrderTypeSampleTypes"" EntityType=""Example.Database.OrderTypeSampleType""/>
				<EntitySet Name=""TestTypeResultTypes"" EntityType=""Example.Database.TestTypeResultType""/>
				<EntitySet Name=""Protocols"" EntityType=""Example.Database.Protocol""/>
				<EntitySet Name=""QualificationTypes"" EntityType=""Example.Database.QualificationType""/>
				<EntitySet Name=""ScheduleTypeQualifications"" EntityType=""Example.Database.ScheduleTypeQualification""/>
				<EntitySet Name=""Chemicals"" EntityType=""Example.Database.Chemical""/>
				<EntitySet Name=""Reactions"" EntityType=""Example.Database.Reaction""/>
				<EntitySet Name=""InstrumentTypes"" EntityType=""Example.Database.InstrumentType""/>
				<EntitySet Name=""Instruments"" EntityType=""Example.Database.Instrument""/>
				<EntitySet Name=""InstrumentFiles"" EntityType=""Example.Database.InstrumentFile""/>
				<EntitySet Name=""MaterialTypes"" EntityType=""Example.Database.MaterialType""/>
				<EntitySet Name=""Materials"" EntityType=""Example.Database.Material""/>
				<EntitySet Name=""BatchTypes"" EntityType=""Example.Database.BatchType""/>
				<EntitySet Name=""BatchFormulationTypes"" EntityType=""Example.Database.BatchFormulationType""/>
				<EntitySet Name=""Batches"" EntityType=""Example.Database.Batch""/>
				<EntitySet Name=""ProductTypes"" EntityType=""Example.Database.ProductType""/>
				<EntitySet Name=""ProductUnitTypes"" EntityType=""Example.Database.ProductUnitType""/>
				<EntitySet Name=""ProductUnits"" EntityType=""Example.Database.ProductUnit""/>
				<EntitySet Name=""ControlChartTypes"" EntityType=""Example.Database.ControlChartType""/>
				<EntitySet Name=""ControlCharts"" EntityType=""Example.Database.ControlChart""/>
				<EntitySet Name=""ControlChartResults"" EntityType=""Example.Database.ControlChartResult""/>
				<EntitySet Name=""SequenceTypes"" EntityType=""Example.Database.SequenceType""/>
				<EntitySet Name=""Sequences"" EntityType=""Example.Database.Sequence""/>
				<EntitySet Name=""SequenceSamples"" EntityType=""Example.Database.SequenceSample""/>
				<EntitySet Name=""SequenceSampleResults"" EntityType=""Example.Database.SequenceSampleResult""/>
				<EntitySet Name=""InstrumentTypeResultTypes"" EntityType=""Example.Database.InstrumentTypeResultType""/>
				<EntitySet Name=""Products"" EntityType=""Example.Database.Product""/>
				<EntitySet Name=""OrderForms"" EntityType=""Example.Database.OrderForm""/>
				<EntitySet Name=""ClientProduct"" EntityType=""Example.Database.ClientProduct""/>
				<EntitySet Name=""OrderFormFields"" EntityType=""Example.Database.OrderFormField""/>
				<EntitySet Name=""OrderFormSamples"" EntityType=""Example.Database.OrderFormSample""/>
				<EntitySet Name=""OrderFormSampleFields"" EntityType=""Example.Database.OrderFormSampleField""/>
				<EntitySet Name=""OrderFormTests"" EntityType=""Example.Database.OrderFormTest""/>
				<EntitySet Name=""OrderFormReports"" EntityType=""Example.Database.OrderFormReport""/>
				<EntitySet Name=""OrderFormReportDestinations"" EntityType=""Example.Database.OrderFormReportDestination""/>
				<EntitySet Name=""PriceLists"" EntityType=""Example.Database.PriceList""/>
				<EntitySet Name=""PriceListTestTypes"" EntityType=""Example.Database.PriceListTestType""/>
				<EntitySet Name=""PriceListPriorities"" EntityType=""Example.Database.PriceListPriority""/>
				<EntitySet Name=""AvailabilityRuleTypes"" EntityType=""Example.Database.AvailabilityRuleType""/>
				<EntitySet Name=""AvailabilityRules"" EntityType=""Example.Database.AvailabilityRule""/>
				<EntitySet Name=""ScheduleRules"" EntityType=""Example.Database.ScheduleRule""/>
				<EntitySet Name=""UserAvailabilitys"" EntityType=""Example.Database.UserAvailability""/>
				<EntitySet Name=""TestTypeMethods"" EntityType=""Example.Database.TestTypeMethod""/>
				<EntitySet Name=""Methods"" EntityType=""Example.Database.Method""/>
				<EntitySet Name=""EntitySpecifications"" EntityType=""Example.Database.EntitySpecification""/>
				<EntitySet Name=""Specifications"" EntityType=""Example.Database.Specification""/>
				<EntitySet Name=""SpecificationItems"" EntityType=""Example.Database.SpecificationItem""/>
				<EntitySet Name=""SpecificationResults"" EntityType=""Example.Database.SpecificationResult""/>
				<EntitySet Name=""ClientUsers"" EntityType=""Example.Database.ClientUser""/>
				<EntitySet Name=""Invoices"" EntityType=""Example.Database.Invoice""/>
				<EntitySet Name=""InvoiceOrders"" EntityType=""Example.Database.InvoiceOrder""/>
				<EntitySet Name=""InvoiceItems"" EntityType=""Example.Database.InvoiceItem""/>
				<EntitySet Name=""VatTypes"" EntityType=""Example.Database.VatType""/>
				<EntitySet Name=""BatchItems"" EntityType=""Example.Database.BatchItem""/>
				<EntitySet Name=""ContainerTypeFields"" EntityType=""Example.Database.ContainerTypeField""/>
				<EntitySet Name=""Priorities"" EntityType=""Example.Database.Priority""/>
				<EntitySet Name=""TestTypePriorities"" EntityType=""Example.Database.TestTypePriority""/>
				<EntitySet Name=""Laboratories"" EntityType=""Example.Database.Laboratory""/>
				<EntitySet Name=""Quotations"" EntityType=""Example.Database.Quotation""/>
				<EntitySet Name=""QuotationItemTypes"" EntityType=""Example.Database.QuotationItemType""/>
				<EntitySet Name=""Articles"" EntityType=""Example.Database.Article""/>
				<EntitySet Name=""ArticleFields"" EntityType=""Example.Database.ArticleField""/>
				<EntitySet Name=""QuotationLocations"" EntityType=""Example.Database.QuotationLocation""/>
				<EntitySet Name=""QuotationTotals"" EntityType=""Example.Database.QuotationTotal""/>
				<EntitySet Name=""QuotationItems"" EntityType=""Example.Database.QuotationItem""/>
				<EntitySet Name=""SequenceTypeResultTypes"" EntityType=""Example.Database.SequenceTypeResultType""/>
				<EntitySet Name=""ResultTypeEvents"" EntityType=""Example.Database.ResultTypeEvent""/>
				<EntitySet Name=""TestTypeTasks"" EntityType=""Example.Database.TestTypeTask""/>
				<EntitySet Name=""InstrumentParts"" EntityType=""Example.Database.InstrumentPart""/>
				<EntitySet Name=""Accreditations"" EntityType=""Example.Database.Accreditation""/>
				<EntitySet Name=""AccreditationTestTypes"" EntityType=""Example.Database.AccreditationTestType""/>
				<EntitySet Name=""SampleTypeTasks"" EntityType=""Example.Database.SampleTypeTask""/>
				<EntitySet Name=""TestTasks"" EntityType=""Example.Database.TestTask""/>
				<EntitySet Name=""SampleTasks"" EntityType=""Example.Database.SampleTask""/>
				<EntitySet Name=""TestTypeQualityControlTriggers"" EntityType=""Example.Database.TestTypeQualityControlTrigger""/>
				<EntitySet Name=""ScheduleRuleSets"" EntityType=""Example.Database.ScheduleRuleSet""/>
				<EntitySet Name=""LaboratorySampleTypes"" EntityType=""Example.Database.LaboratorySampleType""/>
				<EntitySet Name=""LaboratoryTestTypes"" EntityType=""Example.Database.LaboratoryTestType""/>
				<EntitySet Name=""LaboratoryUnits"" EntityType=""Example.Database.LaboratoryUnit""/>
				<EntitySet Name=""LaboratoryResultTypes"" EntityType=""Example.Database.LaboratoryResultType""/>
				<EntitySet Name=""LaboratoryProducts"" EntityType=""Example.Database.LaboratoryProduct""/>
				<EntitySet Name=""LaboratoryProductTypes"" EntityType=""Example.Database.LaboratoryProductType""/>
				<EntitySet Name=""Suites"" EntityType=""Example.Database.Suite""/>
				<EntitySet Name=""SuiteTestTypes"" EntityType=""Example.Database.SuiteTestType""/>
				<EntitySet Name=""SuiteOrderTaskTypes"" EntityType=""Example.Database.SuiteOrderTaskType""/>
				<EntitySet Name=""OrderTaskTypes"" EntityType=""Example.Database.OrderTaskType""/>
				<EntitySet Name=""SuiteOrderTasks"" EntityType=""Example.Database.SuiteOrderTask""/>
				<EntitySet Name=""SuiteSamples"" EntityType=""Example.Database.SuiteSample""/>
				<EntitySet Name=""OrderTasks"" EntityType=""Example.Database.OrderTask""/>
				<EntitySet Name=""OrderTaskResults"" EntityType=""Example.Database.OrderTaskResult""/>
				<EntitySet Name=""OrderTaskResultDataPoints"" EntityType=""Example.Database.OrderTaskResultDataPoint""/>
				<EntitySet Name=""SuiteTests"" EntityType=""Example.Database.SuiteTest""/>
				<EntitySet Name=""ControlSamples"" EntityType=""Example.Database.ControlSample""/>
				<EntitySet Name=""ArticleTypes"" EntityType=""Example.Database.ArticleType""/>
				<EntitySet Name=""ArticleTypeFields"" EntityType=""Example.Database.ArticleTypeField""/>
				<EntitySet Name=""Complaints"" EntityType=""Example.Database.Complaint""/>
				<EntitySet Name=""CorrectiveActions"" EntityType=""Example.Database.CorrectiveAction""/>
				<EntitySet Name=""ComplaintCorrectiveActions"" EntityType=""Example.Database.ComplaintCorrectiveAction""/>
				<EntitySet Name=""ProjectTaskTypes"" EntityType=""Example.Database.ProjectTaskType""/>
				<EntitySet Name=""ProjectTasks"" EntityType=""Example.Database.ProjectTask""/>
				<EntitySet Name=""UserQualifications"" EntityType=""Example.Database.UserQualification""/>
				<EntitySet Name=""BKDCrops"" EntityType=""Example.Database.BKDCrop""/>
				<EntitySet Name=""BKDCultivars"" EntityType=""Example.Database.BKDCultivar""/>
				<EntitySet Name=""BKDPlantMaterials"" EntityType=""Example.Database.BKDPlantMaterial""/>
				<EntitySet Name=""BKDTestAmounts"" EntityType=""Example.Database.BKDTestAmount""/>
				<EntitySet Name=""BKDProjectTaskComments"" EntityType=""Example.Database.BKDProjectTaskComment""/>
				<EntitySet Name=""BKDProjectRules"" EntityType=""Example.Database.BKDProjectRule""/>
				<EntitySet Name=""BKDIncubationBatches"" EntityType=""Example.Database.BKDIncubationBatch""/>
				<EntitySet Name=""BKDIncubationFormulations"" EntityType=""Example.Database.BKDIncubationFormulation""/>
				<EntitySet Name=""BKDCropMaterialIncubation"" EntityType=""Example.Database.BKDCropMaterialIncubation""/>
				<EntitySet Name=""BKDIncubationBatchUsers"" EntityType=""Example.Database.BKDIncubationBatchUser""/>
				<EntitySet Name=""BKDSampleRemarks"" EntityType=""Example.Database.BKDSampleRemark""/>
			</EntityContainer>
		</Schema>
	</edmx:DataServices>
</edmx:Edmx>
";
#endregion
    }
}
