//---------------------------------------------------------------------
// <copyright file="AstoriaTestSilverlightService.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.OData.Service;
using System.Linq;
using System.Data;
using TestSL;
using System;

namespace TestSL
{
    public class History
    {
        public Message Incoming { get; set; }
        public Message OutGoing { get; set; }
        private static int globalHistoryID;
        private int historyID = System.Threading.Interlocked.Increment(ref globalHistoryID);
        public int HistoryID
        {
            get { return historyID; }
        }

    }
    public class Message
    {
        private static int globalMessageID;

        private int messageID = System.Threading.Interlocked.Increment(ref globalMessageID);

        // server generated auto-increment column
        public int MessageID
        {
            get { return this.messageID; }
            set { }
        }

        public string WhoSentMe { get; set; }
        public int InstanceID { get; set; }
        public string Method { get; set; }
        public string ReturnValue { get; set; }
        public string Parameter1 { get; set; }
        public string Parameter2 { get; set; }
        public string Parameter3 { get; set; }
        public string Parameter4 { get; set; }
        public string Parameter5 { get; set; }
        public string Parameter6 { get; set; }
        public string Exception { get; set; }
        public bool IsResultAsync { get; set; }
        public bool CountMode { get; set; }
        public bool CountAllPages { get; set; }
        public long ActualCount { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as Message);
        }

        public bool Equals(Message obj)
        {
            return (null != obj && obj.MessageID == this.MessageID);
        }

        public override int GetHashCode()
        {
            return (int)this.MessageID;
        }

    }
}

public class AstoriaTestSilverlightContext : IUpdatable
{
    public static List<Message> messages = new List<Message>();
    public static List<History> messageHistory = new List<History>();

    public Dictionary<int, object> tokens = new Dictionary<int, object>();
    private List<KeyValuePair<object, EntityState>> pendingChanges;
    private static bool? preserveChanges;

    public IQueryable<History> MessageHistory { get { return messageHistory.AsQueryable<History>(); } }
    public IQueryable<Message> Messages { get { return messages.AsQueryable<Message>(); } }

    // follow pattern in CustomDataContext
    // TODO: implement all members of IUpdatable

    internal List<KeyValuePair<object, EntityState>> PendingChanges
    {
        get
        {
            if (pendingChanges == null)
            {
                pendingChanges = new List<KeyValuePair<object, EntityState>>();
            }

            return pendingChanges;
        }
    }

    internal static void AddResource(Message resource, bool throwIfDuplicate)
    {
        List<Message> entitySetInstances = messages;
        if (entitySetInstances.Contains(resource))
        {

            if (throwIfDuplicate)
            {

                throw new DataServiceException(400, String.Format("Entity with the same key already present. EntityType: '{0}'",
                    resource.GetType().Name));
            }

            // if its already there, do not add it to the global context
            return;
        }

        entitySetInstances.Add(resource);
    }

    private static Message TryGetEntity(List<Message> collection, Message entity)
    {
        int index = collection.IndexOf(entity);
        if (0 <= index)
        {
            return collection[index];
        }

        return null;
    }

    private void DeleteEntity(List<Message> collection, Message entity, bool throwIfNotPresent)
    {
        Message entityToBeDeleted = TryGetEntity(collection, entity);

        if (entityToBeDeleted == null && throwIfNotPresent)
        {
            throw new Exception("No entity found with the given ID");
        }

        if (entityToBeDeleted != null)
        {
            if (collection.Contains(entityToBeDeleted))
            {
                if (!collection.Remove(entityToBeDeleted))
                {
                    throw new Exception("Entity wasn't deleted");
                }
            }
        }
    }

    internal static void ClearData()
    {
        messages = null;
    }

    object IUpdatable.CreateResource(string containerName, string fullTypeName)
    {
        if ("Messages" == containerName)
        {
            if ("TestSL.Message" == fullTypeName)
            {
                Message m = new Message();
                //int token = this.tokens.Count;
                //this.tokens.Add(token, m);

                KeyValuePair<object, EntityState> newOperation = new KeyValuePair<object, EntityState>(m, EntityState.Added);
                PendingChanges.Add(newOperation);
                return m;
            }
        }
        throw new Exception(string.Format("unknown containerName={0} fullTypeName={1}", containerName, fullTypeName));
    }

    object IUpdatable.GetResource(IQueryable query, string fullTypeName)
    {
        object resource = null;
        foreach (object r in query)
        {
            if (resource != null)
            {
                throw new ArgumentException(String.Format("Invalid Uri specified. The query '{0}' must refer to a single resource"));
            }

            resource = r;
        }

        if (fullTypeName != null && resource.GetType().FullName != fullTypeName)
        {
            throw new System.ArgumentException(String.Format("Invalid uri specified. ExpectedType: '{0}', ActualType: '{1}'", fullTypeName, resource.GetType().FullName));
        }

        //int token = this.tokens.Count;
        //this.tokens.Add(token, resource);
        // return token;
        return resource;
    }


    //object IUpdatable.ReplaceResource(IQueryable query, string fullTypeName)
    //{
    //    return query;
    //    //throw new NotImplementedException();
    //}
    object IUpdatable.ResetResource(object resource)
    {
        return resource;
    }

    object IUpdatable.GetValue(object token, string propertyName)
    {

        object targetResource = this.tokens[(int)token];
        PropertyInfo propertyInfo = targetResource.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);

        object propertyValue = propertyInfo.GetValue(targetResource, null);

        int newToken = this.tokens.Count;
        this.tokens.Add(newToken, propertyValue);
        propertyValue = newToken;

        return propertyValue;
    }

    void IUpdatable.SetValue(object m, string propertyName, object propertyValue)
    {
        object targetResource = m;
        PropertyInfo property = targetResource.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);

        if (propertyValue == null)
        {
            property.SetValue(targetResource, propertyValue, null);
        }
        else
        {
            // object actualPropertyValue = this.tokens[(int)propertyValue];
            property.SetValue(targetResource, propertyValue, null);
        }
    }

    void IUpdatable.SetReference(object token, string propertyName, object propertyValueToken)
    {
        object declaringResource = this.tokens[(int)token];
        object propertyValue;
        if (propertyValueToken == null)
        {
            propertyValue = null;
        }
        else
        {
            propertyValue = this.tokens[(int)propertyValueToken];
        }

        declaringResource.GetType().InvokeMember(
            propertyName,
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty,
            null,
            declaringResource,
            new object[] { propertyValue });
    }

    void IUpdatable.AddReferenceToCollection(object token, string propertyName, object elementResourceToken)
    {
        object targetObject = this.tokens[(int)token];
        object element = this.tokens[(int)elementResourceToken];
        object propertyValue = targetObject.GetType().InvokeMember(
            propertyName,
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty,
            null,
            targetObject,
            null);

        propertyValue.GetType().InvokeMember(
            "Add",
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod,
            null,
            propertyValue,
            new object[] { element });
    }

    void IUpdatable.RemoveReferenceFromCollection(object token, string propertyName, object elementResourceToken)
    {
        object targetObject = this.tokens[(int)token];
        object element = this.tokens[(int)elementResourceToken];
        object propertyValue = targetObject.GetType().InvokeMember(
            propertyName,
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty,
            null,
            targetObject,
            null);

        propertyValue.GetType().InvokeMember(
            "Remove",
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod,
            null,
            propertyValue,
            new object[] { element });
    }

    void IUpdatable.DeleteResource(object m)
    {
        //object objectToBeDeleted = this.tokens[(int)token];
        //this.PendingChanges.Add(new KeyValuePair<object, EntityState>(objectToBeDeleted, EntityState.Deleted));

        KeyValuePair<object, EntityState> newOperation = new KeyValuePair<object, EntityState>(m, EntityState.Deleted);
        PendingChanges.Add(newOperation);
    }

    void IUpdatable.SaveChanges()
    {
        if (this.Messages == null)
        {
            return;
        }

        foreach (KeyValuePair<object, EntityState> pendingChange in this.PendingChanges)
        {
            // find the entity set for the object
            List<Message> entitySetInstance = messages;
            switch (pendingChange.Value)
            {
                case EntityState.Added:
                    AddResource(pendingChange.Key as Message, true /*throwIfDuplicate*/);
                    break;
                case EntityState.Deleted:
                    DeleteEntity(entitySetInstance, pendingChange.Key as Message, true /*throwIfNotPresent*/);
                    break;
                default:
                    throw new Exception("Unsupported State");
            }
        }

        this.pendingChanges.Clear();
    }

    IEnumerable<KeyValuePair<string, object>> GetKeys(object resource)
    {
        return new KeyValuePair<string, object>[] { new KeyValuePair<string, object>(
                "MessageID",
                resource.GetType().InvokeMember(
                    "MessageID",
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.GetProperty,
                    null,
                    resource,
                    null))};
    }

    object IUpdatable.ResolveResource(object resource)
    {
        return resource;
    }

    void IUpdatable.ClearChanges()
    {
        if (this.pendingChanges != null)
        {
            this.pendingChanges.Clear();
        }
    }
}

public class AstoriaTestSilverlightService : DataService<AstoriaTestSilverlightContext>
{
    /// <summary>Initializes the service configuration.</summary>
    public static void InitializeService(IDataServiceConfiguration configuration)
    {
        configuration.SetEntitySetAccessRule("*", EntitySetRights.All);
        configuration.SetServiceOperationAccessRule("*", ServiceOperationRights.All);
        configuration.UseVerboseErrors = true;

    }


    [System.ServiceModel.Web.WebGet]
    public string ClearMessages()
    {
        AstoriaTestSilverlightContext.messages.Clear();
        return "Cleared all messages";
    }
    [System.ServiceModel.Web.WebGet]
    public string ClearHistory()
    {
        AstoriaTestSilverlightContext.messageHistory.Clear();
        return "Cleared all message History";
    }

    [ChangeInterceptor("Messages")]
    public void changeHistory(Message message, UpdateOperations updateOperation)
    {
        Func<History, bool> histFilter = hist => hist.Incoming.Method == message.Method;
        switch (updateOperation)
        {
            case UpdateOperations.Add:
                //If existing message , i.e WhoSentMe is SL 
                if (message.WhoSentMe == "SL")
                {
                    if (AstoriaTestSilverlightContext.messageHistory.Any<History>(histFilter))
                    {
                        History existingHistory = AstoriaTestSilverlightContext.messageHistory.Where(histFilter).Last<History>();
                        if (existingHistory != null)
                        {
                            AstoriaTestSilverlightContext.messageHistory.Where(histFilter).Last<History>().OutGoing = message;
                        }
                    }

                }
                else
                {
                    AstoriaTestSilverlightContext.messageHistory.Add(new History()
                    {
                        Incoming = message
                    });
                }
                break;
        }

    }

}
