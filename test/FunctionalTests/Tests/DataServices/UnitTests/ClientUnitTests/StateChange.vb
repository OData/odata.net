'---------------------------------------------------------------------
' <copyright file="StateChange.vb" company="Microsoft">
'      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
' </copyright>
'---------------------------------------------------------------------

Imports System
Imports Microsoft.OData.Client
Imports System.IO
Imports System.Net
Imports System.Linq
Imports System.Xml.Linq
Imports System.Text
Imports System.Collections.Generic
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports AstoriaUnitTests.Data
Imports AstoriaUnitTests.Stubs
Imports Microsoft.Test.ModuleCore

Partial Public Class ClientModule
    ' For comment out test cases, see github: https://github.com/OData/odata.net/issues/887
    <TestClass()> Public Class StateChange

        Private Shared web As TestWebRequest = Nothing
        Private sentRequests As New List(Of HttpWebRequest)()
        Private ctx As NorthwindSimpleModel.NorthwindContext = Nothing

#Region "Initialize DataService and create new context for each text"
        <ClassInitialize()> Public Shared Sub PerClassSetup(ByVal context As TestContext)
            web = TestWebRequest.CreateForInProcessWcf
            web.DataServiceType = ServiceModelData.Northwind.ServiceModelType
            web.StartService()
        End Sub

        <ClassCleanup()> Public Shared Sub PerClassCleanup()
            If Not web Is Nothing Then
                web.Dispose()
            End If
        End Sub

        <TestInitialize()> Public Sub PerTestSetup()
            Me.ctx = New NorthwindSimpleModel.NorthwindContext(web.ServiceRoot)
            'Me.'ctx.EnableAtom = True
            'Me.'ctx.Format.UseAtom()
            AddHandler Me.ctx.SendingRequest2, AddressOf SendingRequestListenHttpMethod
        End Sub

        <TestCleanup()> Public Sub PerTestCleanup()
            Me.ctx = Nothing
            Me.sentRequests.Clear()
        End Sub

        Private Sub SendingRequestListenHttpMethod(ByVal sender As Object, ByVal args As SendingRequest2EventArgs)
            Dim httpRequestMessage = TryCast(args.RequestMessage, HttpWebRequestMessage)
            If httpRequestMessage IsNot Nothing Then
                Dim httpRequest = httpRequestMessage.HttpWebRequest
                sentRequests.Add(httpRequest)
            End If
        End Sub
#End Region
        'Remove Atom
        ' <TestCategory("Partition2")> <TestMethod()>
        Public Sub EntityStates()
            Dim customer = NorthwindSimpleModel.Customers.CreateCustomers("ZZZZ", "Microsoft")

            Assert.IsFalse(ctx.Detach(customer), "detach false")
            Assert.AreEqual(SaveChangesOptions.None, ctx.SaveChangesDefaultOptions, "SaveChangesDefaultOptions")

            Try
                ctx.UpdateObject(customer)
                Assert.Fail("expected ArgumentException")
            Catch ex As ArgumentException
            End Try

            Try
                ctx.DeleteObject(customer)
                Assert.Fail("expected InvalidOperationException")
            Catch ex As InvalidOperationException
            End Try

            ValidateEntityStates()
            ValidateLinkStates()

            Util.SaveChanges(ctx, SaveChangesOptions.None, 0, 0, "Nothing")
            ValidateEntityStates()
            ValidateLinkStates()
            ValidateRequestMethods()

            ' AddObject
            ctx.AddToCustomers(customer)
            ValidateEntityStates(Microsoft.OData.Client.EntityStates.Added)
            ValidateLinkStates()

            Assert.IsTrue(ctx.Detach(customer), "detach true")
            ValidateEntityStates()
            ValidateLinkStates()

            ctx.AddToCustomers(customer)
            ValidateEntityStates(Microsoft.OData.Client.EntityStates.Added)
            ValidateLinkStates()

            Try
                ctx.AddToCustomers(customer)
                Assert.Fail("expected InvalidOperationException")
            Catch ex As InvalidOperationException
            End Try

            Try
                ctx.AddObject("NotCustomers", customer)
                Assert.Fail("expected InvalidOperationException")
            Catch ex As InvalidOperationException
            End Try

            Try
                ctx.AttachTo("Customers", customer)
                Assert.Fail("expected InvalidOperationException")
            Catch ex As InvalidOperationException
            End Try

            Try
                ctx.AttachTo("NotCustomers", customer)
                Assert.Fail("expected InvalidOperationException")
            Catch ex As InvalidOperationException
            End Try

            ctx.UpdateObject(customer) ' expect no change
            ValidateEntityStates(Microsoft.OData.Client.EntityStates.Added)
            ValidateLinkStates()

            ctx.DeleteObject(customer)
            ValidateEntityStates()
            ValidateLinkStates()

            ctx.AddToCustomers(customer)
            ValidateEntityStates(Microsoft.OData.Client.EntityStates.Added)
            ValidateLinkStates()

            ' POST
            Util.SaveChanges(ctx, SaveChangesOptions.None, 1, 0, "POST")
            ValidateEntityStates(Microsoft.OData.Client.EntityStates.Unchanged)
            ValidateLinkStates()
            ValidateRequestMethods("POST")

            Try
                ctx.AddToCustomers(customer)
                Assert.Fail("expected InvalidOperationException")
            Catch ex As InvalidOperationException
            End Try

            Try
                ctx.AddObject("NotCustomers", customer)
                Assert.Fail("expected InvalidOperationException")
            Catch ex As InvalidOperationException
            End Try

            Try
                ctx.AttachTo("Customers", customer)
                Assert.Fail("expected InvalidOperationException")
            Catch ex As InvalidOperationException
            End Try

            Try
                ctx.AttachTo("NotCustomers", customer)
                Assert.Fail("expected InvalidOperationException")
            Catch ex As InvalidOperationException
            End Try

            Util.SaveChanges(ctx, SaveChangesOptions.None, 0, 0, "Unchanged")
            ValidateEntityStates(Microsoft.OData.Client.EntityStates.Unchanged)
            ValidateLinkStates()
            ValidateRequestMethods()

            ' PATCH
            ctx.UpdateObject(customer)
            ValidateEntityStates(Microsoft.OData.Client.EntityStates.Modified)
            ValidateLinkStates()

            Try
                ctx.AddToCustomers(customer)
                Assert.Fail("expected InvalidOperationException")
            Catch ex As InvalidOperationException
            End Try

            Try
                ctx.AddObject("NotCustomers", customer)
                Assert.Fail("expected InvalidOperationException")
            Catch ex As InvalidOperationException
            End Try

            Try
                ctx.AttachTo("Customers", customer)
                Assert.Fail("expected InvalidOperationException")
            Catch ex As InvalidOperationException
            End Try

            Try
                ctx.AttachTo("NotCustomers", customer)
                Assert.Fail("expected InvalidOperationException")
            Catch ex As InvalidOperationException
            End Try

            Util.SaveChanges(ctx, SaveChangesOptions.None, 1, 0, "PATCH")
            ValidateEntityStates(Microsoft.OData.Client.EntityStates.Unchanged)
            ValidateLinkStates()
            ValidateRequestMethods("PATCH")

            ' DeleteObject
            ctx.DeleteObject(customer)
            ValidateEntityStates(Microsoft.OData.Client.EntityStates.Deleted)
            ValidateLinkStates()

            ctx.UpdateObject(customer) ' expect no change
            ValidateEntityStates(Microsoft.OData.Client.EntityStates.Deleted)
            ValidateLinkStates()

            ctx.DeleteObject(customer) ' expect no change
            ValidateEntityStates(Microsoft.OData.Client.EntityStates.Deleted)
            ValidateLinkStates()

            Try
                ctx.AddToCustomers(customer)
                Assert.Fail("expected InvalidOperationException")
            Catch ex As InvalidOperationException
            End Try

            Try
                ctx.AddObject("NotCustomers", customer)
                Assert.Fail("expected InvalidOperationException")
            Catch ex As InvalidOperationException
            End Try

            Try
                ctx.AttachTo("Customers", customer)
                Assert.Fail("expected InvalidOperationException")
            Catch ex As InvalidOperationException
            End Try

            Try
                ctx.AttachTo("NotCustomers", customer)
                Assert.Fail("expected InvalidOperationException")
            Catch ex As InvalidOperationException
            End Try

            ValidateEntityStates(Microsoft.OData.Client.EntityStates.Deleted)
            ValidateLinkStates()

            ' DELETE
            Util.SaveChanges(ctx, SaveChangesOptions.None, 1, 0, "DELETE")
            ValidateEntityStates()
            ValidateLinkStates()
            ValidateRequestMethods("DELETE")

            ' AttachTo
            ctx.AttachTo("Customers", customer)
            ValidateEntityStates(Microsoft.OData.Client.EntityStates.Unchanged)
            ValidateLinkStates()

            Try
                ctx.AddToCustomers(customer)
                Assert.Fail("expected InvalidOperationException")
            Catch ex As InvalidOperationException
            End Try

            Try
                ctx.AddObject("NotCustomers", customer)
                Assert.Fail("expected InvalidOperationException")
            Catch ex As InvalidOperationException
            End Try

            Try
                ctx.AttachTo("Customers", customer)
                Assert.Fail("expected InvalidOperationException")
            Catch ex As InvalidOperationException
            End Try

            Try
                ctx.AttachTo("NotCustomers", customer)
                Assert.Fail("expected InvalidOperationException")
            Catch ex As InvalidOperationException
            End Try

            ' PUT fail
            ctx.UpdateObject(customer)
            ValidateEntityStates(Microsoft.OData.Client.EntityStates.Modified)
            ValidateLinkStates()

            Util.SaveChanges(ctx, SaveChangesOptions.ReplaceOnUpdate, 0, 1, "PUT fail")
            ValidateEntityStates(Microsoft.OData.Client.EntityStates.Modified)
            ValidateLinkStates()
            ValidateRequestMethods("PUT")

            ' POST again
            ctx.Detach(customer)
            ctx.AddToCustomers(customer)
            Util.SaveChanges(ctx, SaveChangesOptions.None, 1, 0, "POST again")
            ValidateEntityStates(Microsoft.OData.Client.EntityStates.Unchanged)
            ValidateLinkStates()
            ValidateRequestMethods("POST")

            ' PUT
            ctx.UpdateObject(customer)
            ValidateEntityStates(Microsoft.OData.Client.EntityStates.Modified)
            ValidateLinkStates()

            Util.SaveChanges(ctx, SaveChangesOptions.ReplaceOnUpdate, 1, 0, "PUT")
            ValidateEntityStates(Microsoft.OData.Client.EntityStates.Unchanged)
            ValidateLinkStates()
            ValidateRequestMethods("PUT")

            ' Cleanup DELETE
            ctx.DeleteObject(customer)
            ValidateEntityStates(Microsoft.OData.Client.EntityStates.Deleted)
            ValidateLinkStates()

            Util.SaveChanges(ctx, SaveChangesOptions.None, 1, 0, "Cleanup")
            ValidateEntityStates()
            ValidateLinkStates()
            ValidateRequestMethods("DELETE")
        End Sub
        'Remove Atom
        ' <TestCategory("Partition2")> <TestMethod()>
        Public Sub CollectionLinkStates_AddDelete()
            Dim customer = NorthwindSimpleModel.Customers.CreateCustomers("ZZZY", "Microsoft")
            Dim order = NorthwindSimpleModel.Orders.CreateOrders(9999)

            ' Add everything
            ctx.AddToCustomers(customer)
            ctx.AddToOrders(order)
            ValidateEntityObjects(customer, order)
            ValidateEntityStates(Microsoft.OData.Client.EntityStates.Added, Microsoft.OData.Client.EntityStates.Added)
            ValidateLinkStates()

            Util.SaveChanges(ctx, SaveChangesOptions.None, 2, 0, "Add Customer, Add Order")
            ValidateEntityStates(Microsoft.OData.Client.EntityStates.Unchanged, Microsoft.OData.Client.EntityStates.Unchanged)
            ValidateLinkStates()
            ValidateRequestMethods("POST", "POST")

            ctx.AddLink(customer, "Orders", order)
            ctx.DeleteObject(order)
            Util.SaveChanges(ctx, SaveChangesOptions.None, 2, 0, "Add Customer -> Order, delete order")
            ValidateEntityObjects(customer)
            ValidateEntityStates(Microsoft.OData.Client.EntityStates.Unchanged)
            ValidateLinkStates()
            ValidateRequestMethods("POST", "DELETE")
        End Sub
        'Remove Atom
        ' <TestCategory("Partition2")> <TestMethod()>
        Public Sub CollectionLinkStates_AddSetDelete()
            Dim customer = NorthwindSimpleModel.Customers.CreateCustomers("ZZZX", "Microsoft")
            Dim order = NorthwindSimpleModel.Orders.CreateOrders(9999)

            ' Add everything
            ctx.AddToCustomers(customer)
            ctx.AddToOrders(order)
            ctx.SetLink(order, "Customers", customer)
            ValidateEntityObjects(customer, order)
            ValidateEntityStates(Microsoft.OData.Client.EntityStates.Added, Microsoft.OData.Client.EntityStates.Added)
            ValidateLinkStates(Microsoft.OData.Client.EntityStates.Modified)

            Util.SaveChanges(ctx, SaveChangesOptions.None, 2, 0, "Add Customer, Add Order")
            ValidateEntityStates(Microsoft.OData.Client.EntityStates.Unchanged, Microsoft.OData.Client.EntityStates.Unchanged)
            ValidateLinkStates(Microsoft.OData.Client.EntityStates.Unchanged)
            ValidateRequestMethods("POST", "POST")

            ctx.AddLink(customer, "Orders", order)
            ctx.DeleteObject(order)
            Util.SaveChanges(ctx, SaveChangesOptions.None, 2, 0, "Add Customer -> Order, delete order")
            ValidateEntityObjects(customer)
            ValidateEntityStates(Microsoft.OData.Client.EntityStates.Unchanged)
            ValidateLinkStates()
            ValidateRequestMethods("POST", "DELETE")
        End Sub
        'Remove Atom
        ' <TestCategory("Partition2")> <TestMethod()>
        Public Sub CollectionLinkStates_AddSetDelete_Batch()
            Dim customer = NorthwindSimpleModel.Customers.CreateCustomers("ZZZU", "Microsoft")
            Dim order = NorthwindSimpleModel.Orders.CreateOrders(9999)

            ' Add everything
            ctx.AddToCustomers(customer)
            ctx.AddToOrders(order)
            ctx.SetLink(order, "Customers", customer)
            ValidateEntityObjects(customer, order)
            ValidateEntityStates(Microsoft.OData.Client.EntityStates.Added, Microsoft.OData.Client.EntityStates.Added)
            ValidateLinkStates(Microsoft.OData.Client.EntityStates.Modified)

            Util.SaveChanges(ctx, SaveChangesOptions.BatchWithSingleChangeset, 3, 0, "Add Customer, Add Order")
            ValidateEntityStates(Microsoft.OData.Client.EntityStates.Unchanged, Microsoft.OData.Client.EntityStates.Unchanged)
            ValidateLinkStates(Microsoft.OData.Client.EntityStates.Unchanged)
            ValidateRequestMethods("POST")

            ctx.DeleteObject(order)
            Util.SaveChanges(ctx, SaveChangesOptions.BatchWithSingleChangeset, 1, 0, "Add Customer -> Order, delete order")
            ValidateEntityObjects(customer)
            ValidateEntityStates(Microsoft.OData.Client.EntityStates.Unchanged)
            ValidateLinkStates()
            ValidateRequestMethods("POST")
        End Sub
        'Remove Atom
        ' <TestCategory("Partition2")> <TestMethod()>
        Public Sub CollectionLinkStates_AddAdd_Delete()
            Dim customer = NorthwindSimpleModel.Customers.CreateCustomers("ZZZW", "Microsoft")
            Dim order = NorthwindSimpleModel.Orders.CreateOrders(9999)

            ' Add everything
            ctx.AddToCustomers(customer)
            ctx.AddToOrders(order)
            ctx.AddLink(customer, "Orders", order)
            ValidateEntityObjects(customer, order)
            ValidateEntityStates(Microsoft.OData.Client.EntityStates.Added, Microsoft.OData.Client.EntityStates.Added)
            ValidateLinkStates(Microsoft.OData.Client.EntityStates.Added)

            ctx.Detach(order)
            ValidateEntityObjects(customer)
            ValidateEntityStates(Microsoft.OData.Client.EntityStates.Added)
            ValidateLinkStates()

            ctx.AddToOrders(order)
            ctx.AddLink(customer, "Orders", order)
            ValidateEntityObjects(customer, order)
            ValidateEntityStates(Microsoft.OData.Client.EntityStates.Added, Microsoft.OData.Client.EntityStates.Added)
            ValidateLinkStates(Microsoft.OData.Client.EntityStates.Added)

            Util.SaveChanges(ctx, SaveChangesOptions.None, 3, 0, "Add Customer, Add Order w/ Collection Link")
            ValidateEntityStates(Microsoft.OData.Client.EntityStates.Unchanged, Microsoft.OData.Client.EntityStates.Unchanged)
            ValidateLinkStates(Microsoft.OData.Client.EntityStates.Unchanged)
            ValidateRequestMethods("POST", "POST", "POST") ' it's a collection, not a reference link

            ctx.DeleteLink(customer, "Orders", order)
            ctx.DeleteObject(customer)
            ValidateEntityObjects(order, customer)
            ValidateEntityStates(Microsoft.OData.Client.EntityStates.Unchanged, Microsoft.OData.Client.EntityStates.Deleted)
            ValidateLinkStates(Microsoft.OData.Client.EntityStates.Deleted)

            Util.SaveChanges(ctx, SaveChangesOptions.None, 2, 0, "Delete Customer->Order, Delete Customer")
            ValidateEntityObjects(order)
            ValidateEntityStates(Microsoft.OData.Client.EntityStates.Unchanged)
            ValidateLinkStates()
            ValidateRequestMethods("DELETE", "DELETE")

            ctx.DeleteObject(order)
            ValidateEntityObjects(order)
            ValidateEntityStates(Microsoft.OData.Client.EntityStates.Deleted)
            ValidateLinkStates()

            Util.SaveChanges(ctx, SaveChangesOptions.None, 1, 0, "delete all batch")
            ValidateEntityObjects()
            ValidateEntityStates()
            ValidateLinkStates()
            ValidateRequestMethods("DELETE")
        End Sub
        'Remove Atom
        ' <TestCategory("Partition2")> <TestMethod()>
        Public Sub CollectionLinkStates_Add_Add_Delete()
            Dim customer = NorthwindSimpleModel.Customers.CreateCustomers("ZZZV", "Microsoft")
            Dim order = NorthwindSimpleModel.Orders.CreateOrders(9998)

            ' Add everything
            ctx.AddToCustomers(customer)
            Util.SaveChanges(ctx, SaveChangesOptions.None, 1, 0, "Add Customer")
            ValidateRequestMethods("POST")

            ctx.AddToOrders(order)
            ctx.AddLink(customer, "Orders", order)
            ValidateEntityObjects(customer, order)
            ValidateEntityStates(Microsoft.OData.Client.EntityStates.Unchanged, Microsoft.OData.Client.EntityStates.Added)
            ValidateLinkStates(Microsoft.OData.Client.EntityStates.Added)

            Util.SaveChanges(ctx, SaveChangesOptions.None, 2, 0, "Add Add Order w/ Link")
            ValidateEntityObjects(customer, order)
            ValidateEntityStates(Microsoft.OData.Client.EntityStates.Unchanged, Microsoft.OData.Client.EntityStates.Unchanged)
            ValidateLinkStates(Microsoft.OData.Client.EntityStates.Unchanged)
            ValidateRequestMethods("POST", "POST")

            ctx.DeleteLink(customer, "Orders", order)
            ctx.DeleteObject(order)
            ValidateEntityObjects(customer, order)
            ValidateEntityStates(Microsoft.OData.Client.EntityStates.Unchanged, Microsoft.OData.Client.EntityStates.Deleted)
            ValidateLinkStates(Microsoft.OData.Client.EntityStates.Deleted)

            Util.SaveChanges(ctx, SaveChangesOptions.None, 2, 0, "Delete Delete order")
            ValidateEntityObjects(customer)
            ValidateEntityStates(Microsoft.OData.Client.EntityStates.Unchanged)
            ValidateLinkStates()
            ValidateRequestMethods("DELETE", "DELETE")

            ctx.DeleteObject(customer)
            Util.SaveChanges(ctx, SaveChangesOptions.None, 1, 0, "cleanup Delete")
            ValidateEntityObjects()
            ValidateEntityStates()
            ValidateLinkStates()
            ValidateRequestMethods("DELETE")
        End Sub
        'Remove Atom
        ' <TestCategory("Partition2")> <TestMethod()>
        Public Sub ReferenceLinkStates()
            Dim child = NorthwindSimpleModel.Territories.CreateTerritories("888886", "Astoria")
            Dim parent = NorthwindSimpleModel.Region.CreateRegion(999996, "Software")
            child.Region = parent

            ctx.AddToRegion(parent)
            ctx.AddToTerritories(child)
            ctx.SetLink(child, "Region", parent)
            ValidateEntityObjects(parent, child)
            ValidateEntityStates(Microsoft.OData.Client.EntityStates.Added, Microsoft.OData.Client.EntityStates.Added)
            ValidateLinkStates(Microsoft.OData.Client.EntityStates.Modified)

            Util.SaveChanges(ctx, SaveChangesOptions.None, 2, 0, "Add Territories, Add Region w/ Reference Link")
            ValidateEntityStates(Microsoft.OData.Client.EntityStates.Unchanged, Microsoft.OData.Client.EntityStates.Unchanged)
            ValidateLinkStates(Microsoft.OData.Client.EntityStates.Unchanged)
            ValidateRequestMethods("POST", "POST") ' it's a reference link

            ctx.DeleteObject(parent)
            ValidateEntityObjects(child, parent)
            ValidateEntityStates(Microsoft.OData.Client.EntityStates.Unchanged, Microsoft.OData.Client.EntityStates.Deleted)
            ValidateLinkStates(Microsoft.OData.Client.EntityStates.Unchanged)

            Util.SaveChanges(ctx, SaveChangesOptions.None, 0, 1, "Delete Region w/ Reference Link")
            ValidateEntityObjects(child, parent)
            ValidateEntityStates(Microsoft.OData.Client.EntityStates.Unchanged, Microsoft.OData.Client.EntityStates.Deleted)
            ValidateLinkStates(Microsoft.OData.Client.EntityStates.Unchanged)
            ValidateRequestMethods("DELETE")

            ctx.DeleteObject(child)
            ValidateEntityObjects(parent, child)
            ValidateEntityStates(Microsoft.OData.Client.EntityStates.Deleted, Microsoft.OData.Client.EntityStates.Deleted)
            ValidateLinkStates(Microsoft.OData.Client.EntityStates.Unchanged)

            Util.SaveChanges(ctx, SaveChangesOptions.BatchWithSingleChangeset, 2, 0, "Cleanup batch")
            ValidateEntityStates()
            ValidateLinkStates()
            ValidateRequestMethods("POST")
        End Sub
        'Remove Atom
        ' <TestCategory("Partition2")> <TestMethod()>
        Public Sub DeleteEntityAsync()
            Dim uriBefore As Uri = Nothing
            Dim uriAfter As Uri = Nothing
            Dim entityTemp As NorthwindSimpleModel.Customers = Nothing

            Dim customer1 = NorthwindSimpleModel.Customers.CreateCustomers("ZZZT", "Microsoft")
            ctx.AddToCustomers(customer1)
            Util.SaveChanges(ctx, SaveChangesOptions.None, 1, 0, "Create first customer")
            ctx.SaveChanges()

            ctx.DeleteObject(customer1)
            Assert.IsTrue(ctx.TryGetUri(customer1, uriBefore))
            Dim asyncResult = ctx.BeginSaveChanges(SaveChangesOptions.None, Nothing, Nothing)
            asyncResult.AsyncWaitHandle.WaitOne()

            Assert.IsTrue(ctx.TryGetUri(customer1, uriAfter))
            Assert.IsTrue(ctx.TryGetEntity(uriBefore, entityTemp))
            Assert.AreSame(customer1, entityTemp, "should be same customer1")
            Assert.AreEqual(uriBefore, uriAfter, "uri should be the same")

            ctx.EndSaveChanges(asyncResult)
            Assert.IsFalse(ctx.TryGetUri(customer1, uriAfter))
            Assert.IsFalse(ctx.TryGetEntity(uriBefore, customer1))
        End Sub

        <TestCategory("Partition2")> <TestMethod()> Public Sub DeleteEntityFail()
            Dim uriBefore As Uri = Nothing
            Dim uriAfter As Uri = Nothing

            Dim customer1 = NorthwindSimpleModel.Customers.CreateCustomers("ZZZS", "Microsoft")
            ctx.AttachTo("Customers", customer1)
            Assert.IsTrue(ctx.TryGetUri(customer1, uriBefore))

            ctx.DeleteObject(customer1)
            Dim asyncResult = ctx.BeginSaveChanges(Nothing, Nothing)
            asyncResult.AsyncWaitHandle.WaitOne()

            Assert.IsTrue(ctx.TryGetUri(customer1, uriAfter))
            Assert.AreEqual(uriBefore, uriAfter, "link1 changed")

            Try
                ctx.EndSaveChanges(asyncResult)
                Assert.Fail("expected exception")
            Catch ex As DataServiceRequestException

            End Try

            uriAfter = Nothing
            Assert.IsTrue(ctx.TryGetUri(customer1, uriAfter))
            Assert.AreEqual(uriBefore, uriAfter, "link2 changed")
        End Sub

        <TestCategory("Partition2")> <TestMethod()> Public Sub DeleteFailAddSucceed()
            Dim uriBefore As Uri = Nothing
            Dim uriAfter As Uri = Nothing

            Dim customer1 = NorthwindSimpleModel.Customers.CreateCustomers("ZZZR", "Microsoft")
            ctx.AttachTo("Customers", customer1)
            Assert.IsTrue(ctx.TryGetUri(customer1, uriBefore))

            ctx.DeleteObject(customer1)
            Dim customer2 = NorthwindSimpleModel.Customers.CreateCustomers("ZZZR", "Microsoft")
            ctx.AddToCustomers(customer2)

            Dim asyncResult = ctx.BeginSaveChanges(SaveChangesOptions.ContinueOnError, Nothing, Nothing)
            asyncResult.AsyncWaitHandle.WaitOne()

            Assert.IsFalse(ctx.TryGetUri(customer1, uriAfter))
            Assert.IsTrue(ctx.TryGetUri(customer2, uriAfter))
            Assert.AreEqual(uriBefore, uriAfter, "link1 changed")

            Try
                ctx.EndSaveChanges(asyncResult)
                Assert.Fail("expected exception")
            Catch ex As DataServiceRequestException

            End Try

            uriAfter = Nothing
            Assert.IsFalse(ctx.TryGetUri(customer1, uriAfter))
            Assert.IsTrue(ctx.TryGetUri(customer2, uriAfter))
            Assert.AreEqual(uriBefore, uriAfter, "link2 changed")

            ctx.Detach(customer2)
            ctx.SaveChanges()
        End Sub
        'Remove Atom
        ' <TestCategory("Partition2")> <TestMethod()>
        Public Sub AddSucceedDeleteSucceed()
            Dim uriBefore As Uri = Nothing
            Dim uriAfter As Uri = Nothing

            Dim customer2 = NorthwindSimpleModel.Customers.CreateCustomers("ZZZQ", "Microsoft")
            ctx.AddToCustomers(customer2)

            Dim customer1 = NorthwindSimpleModel.Customers.CreateCustomers("ZZZQ", "Microsoft")
            ctx.AttachTo("Customers", customer1)
            Assert.IsTrue(ctx.TryGetUri(customer1, uriBefore))
            ctx.DeleteObject(customer1)

            Dim asyncResult = ctx.BeginSaveChanges(SaveChangesOptions.ContinueOnError, Nothing, Nothing)
            asyncResult.AsyncWaitHandle.WaitOne()

            Assert.IsFalse(ctx.TryGetUri(customer1, uriAfter))
            Assert.IsTrue(ctx.TryGetUri(customer2, uriAfter))
            Assert.AreEqual(uriBefore, uriAfter, "link1 changed")

            'Try
            ctx.EndSaveChanges(asyncResult) ' weird scenario because database is now empty, but client has 1 unchanged
            'Assert.Fail("expected exception")
            'Catch ex As DataServiceRequestException
            'End Try

            uriAfter = Nothing
            Assert.IsFalse(ctx.TryGetUri(customer1, uriAfter))
            Assert.IsTrue(ctx.TryGetUri(customer2, uriAfter))
            Assert.AreEqual(uriBefore, uriAfter, "link2 changed")
        End Sub
        'Remove Atom
        ' <TestCategory("Partition2")> <TestMethod()>
        Public Sub SameKeyDeleteAdd()
            ' batching not in this list because the server will error
            For Each saveChangeOption In New SaveChangesOptions() {SaveChangesOptions.None, SaveChangesOptions.ReplaceOnUpdate, SaveChangesOptions.ContinueOnError}
                Dim customer1 = NorthwindSimpleModel.Customers.CreateCustomers("ZZZP", "Microsoft")
                ctx.AddToCustomers(customer1)
                Util.SaveChanges(ctx, saveChangeOption, 1, 0, "Create first customer")
                ctx.SaveChanges()

                ctx.DeleteObject(customer1)
                Dim customer2 = NorthwindSimpleModel.Customers.CreateCustomers("ZZZP", "Microsoft")
                ctx.AddToCustomers(customer2)
                Util.SaveChanges(ctx, saveChangeOption, 2, 0, "Delete customer, add new customer that has same Uri " & saveChangeOption.ToString())

                ctx.DeleteObject(customer2)
                Util.SaveChanges(ctx, saveChangeOption, 1, 0, "cleanup")
            Next
        End Sub
        'Remove Atom
        ' <TestCategory("Partition2")> <TestMethod()>
        Public Sub SameKeyDeleteAdd_Verify()
            Dim uriBefore As Uri = Nothing
            Dim uriAfter As Uri = Nothing
            Dim uriTemp As Uri = Nothing
            Dim entityTemp As NorthwindSimpleModel.Customers = Nothing

            Dim customer1 = NorthwindSimpleModel.Customers.CreateCustomers("ZZZO", "Microsoft")
            ctx.AddToCustomers(customer1)
            Util.SaveChanges(ctx, SaveChangesOptions.None, 1, 0, "Create first customer")
            ctx.SaveChanges()

            Assert.IsTrue(ctx.TryGetUri(customer1, uriBefore))

            ctx.DeleteObject(customer1)
            Dim customer2 = NorthwindSimpleModel.Customers.CreateCustomers("ZZZO", "Microsoft")
            ctx.AddToCustomers(customer2)
            Dim asyncResult = ctx.BeginSaveChanges(SaveChangesOptions.None, Nothing, Nothing)
            asyncResult.AsyncWaitHandle.WaitOne()

            Assert.IsFalse(ctx.TryGetUri(customer1, uriTemp))
            Assert.IsTrue(ctx.TryGetUri(customer2, uriAfter))
            Assert.IsTrue(ctx.TryGetEntity(uriBefore, entityTemp))
            Assert.AreSame(customer2, entityTemp, "should be same customer2")
            Assert.AreEqual(uriBefore, uriAfter, "uri should be the same")

            ctx.EndSaveChanges(asyncResult)
            Assert.IsTrue(ctx.TryGetUri(customer2, uriTemp))
            Assert.AreEqual(uriAfter, uriTemp, "uri should be the same")

            ctx.DeleteObject(customer2)
            Util.SaveChanges(ctx, SaveChangesOptions.None, 1, 0, "cleanup")
        End Sub

        Private Sub ValidateEntityObjects(ByVal ParamArray objects As Object())
            Dim entities = ctx.Entities
            Assert.AreEqual(objects.Length, entities.Count, "Entity count")
            For i As Int32 = 0 To objects.Length - 1
                Assert.IsTrue(Object.ReferenceEquals(objects(i), entities.Item(i).Entity), "Entities[{0}].Entity differs", i)
            Next
        End Sub

        Private Sub ValidateEntityStates(ByVal ParamArray states As Microsoft.OData.Client.EntityStates())
            Dim entities = ctx.Entities
            Dim x = String.Concat((From m In entities Select m.State).ToArray())
            Assert.AreEqual(states.Length, entities.Count, "Entity count {0}", x)
            For i As Int32 = 0 To states.Length - 1
                Assert.AreEqual(states(i), entities.Item(i).State, "Entities[{0}].State", i)
            Next
        End Sub

        Private Sub ValidateLinkStates(ByVal ParamArray states As Microsoft.OData.Client.EntityStates())
            Dim entities = ctx.Links
            Dim x = String.Concat((From m In entities Select m.State).Cast(Of Object)().ToArray())
            Assert.AreEqual(states.Length, entities.Count, "Link count {0}", x)
            For i As Int32 = 0 To states.Length - 1
                Assert.AreEqual(states(i), entities.Item(i).State, "Entities[{0}].State", i)
            Next
        End Sub

        Private Sub ValidateRequestMethods(ByVal ParamArray methods As String())
            Dim x = String.Concat((From m In Me.sentRequests Select m.Method).ToArray())
            Assert.AreEqual(methods.Length, Me.sentRequests.Count, "SendingRequest count {0}", x)
            For i As Int32 = 0 To methods.Length - 1
                Assert.AreEqual(methods(i), Me.sentRequests.Item(i).Method, "SendingRequest[{0}].Method", i)
            Next
            Me.sentRequests.Clear()
        End Sub

    End Class
End Class