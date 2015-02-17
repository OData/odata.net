//---------------------------------------------------------------------
// <copyright file="CommonTypes.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using Microsoft.OData.Client;

namespace TestSL
{
    public enum QueryType
    {
        TopLevelEntitySet,
        Where1Level,
        SelectConstant,
        SelectEmpty,
        SelectAll,
        SelectNavProperty,
        OrderbyAsc,
        OrderByDesc,
        TakeAndSkip,
        TakeOnly,
        SkipOnly,
        First,
        Single,
        ThenByAsc,
        ThenByDesc,
        ExpandNavProp
    }

    public class Serializer
    {
        public static void SetResolveType(ResolveTypeDelegate handler)
        {
            ResolveClientType = handler;
        }
        public static string WriteObject(object entity)
        {
            if (entity == null)
                entity = "null";

            MemoryStream ms = new MemoryStream();
            DataContractSerializer serializer = new DataContractSerializer(entity.GetType());
            serializer.WriteObject(ms, entity);
            return Convert.ToBase64String(ms.ToArray());


        }
        public delegate Type ResolveTypeDelegate(string entitySetName);
        public static event ResolveTypeDelegate ResolveClientType;

        public static object ReadObject(string sEntityName, string sEntity)
        {
            MemoryStream ms = new MemoryStream(Convert.FromBase64String(sEntity));
            Type entityType = ResolveClientType(sEntityName);

            System.Runtime.Serialization.DataContractSerializer serializer = new DataContractSerializer(entityType);
            object clientType = serializer.ReadObject(ms);

            return clientType;
        }

        public static T ReadObject<T>(string sEntity)
        {
            MemoryStream ms = new MemoryStream(Convert.FromBase64String(sEntity));
            System.Runtime.Serialization.DataContractSerializer serializer = new DataContractSerializer(typeof(T));
            T clientType = (T)serializer.ReadObject(ms);

            return clientType;
        }
    }

    public class History
    {
        public Message Incoming { get; set; }
        public Message OutGoing { get; set; }
        public static int globalHistoryID;
        public int historyID = System.Threading.Interlocked.Increment(ref globalHistoryID);
        public int HistoryID
        {
            get { return historyID; }
        }

    }

    public class Message
    {
        //public static int globalMessageID;

        //public int messageID = System.Threading.Interlocked.Increment(ref globalMessageID);

        // server generated auto-increment column
        public int MessageID
        {
            get;
            set;
        }

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
        public string WhoSentMe { get; set; }
        public bool IsResultAsync { get; set; }


        //public override bool Equals(object obj)
        //{
        //    return Equals(obj as Message);
        //}

        //public bool Equals(Message obj)
        //{
        //    return (null != obj && obj.MessageID == this.MessageID);
        //}

        public override int GetHashCode()
        {
            return (int)this.MessageID;
        }

    }
}

namespace WrapperTypes
{
    using TestSL;
    using System.Collections;

    public enum ExecutionModes
    {
        Sync,
        Async
    }


    public class DispatchExecutor
    {
        public static List<Object> Instances
        {
            get;
            set;
        }


        public static int ResourceStarvationCounter { get; set; }
        public static int ResourceStarvationLimit { get; set; }

        public static event Delegates.ResolveEntityDelegate ResolveSameEntity;
        public static event Delegates.NormalizeEntityDelegate NormalizeEntity;
        public static event Delegates.NormalizeEntitiesDelegate NormalizeEntities;
        public static event Delegates.VerifyLinqTestDelegate VerifyLinqTest;
        public static event Delegates.ExecutionError ErrorInExecution;

    }
    public class Delegates
    {
        public delegate object ResolveEntityDelegate(object objToMatch, DataServiceContext ctx);
        public delegate object ResolveEntityNoKeyDelegate(object objToMatch, DataServiceContext ctx);
        public delegate bool VerifyLinqTestDelegate(IEnumerator baseline, IEnumerator linqQuery);
        public delegate void NormalizeEntityDelegate(Message message, object instance, ref object entity);
        public delegate void NormalizeEntitiesDelegate(Message message, object instance, ref object parent, ref object child);
        public delegate void ExecutionError(string MethodName, Message request, Exception Error);
        public delegate void ExecutionEnd(string MethodName, Message request, Message response);

    }
    [DataContract(Name = "EntityDescriptorWrapper", Namespace = "http://schemas.datacontract.org/2004/07/System.Data.Test.Astoria")]
    public class EntityDescriptorWrapper
    {
        [DataMember]
        public Microsoft.OData.Client.EntityStates State { get; set; }
        [DataMember]
        public System.Exception Error { get; set; }
        [DataMember]
        public string EntityString { get; set; }
        [DataMember]
        public string EntityType { get; set; }
        public EntityDescriptorWrapper(EntityDescriptor ed)
        {
            this.State = ed.State;

            //this.Error = ed.Error;
            //this.Entity = ed.Entity;
        }

        //public static Type[] KnownTypes()
        //{
        //    return new Type[] { typeof(ArubaClient.DefectBug) };
        //}

    }


    [DataContract(Name = "LinkDescriptorWrapper", Namespace = "http://schemas.datacontract.org/2004/07/System.Data.Test.Astoria")]
    public class LinkDescriptorWrapper
    {
        [DataMember]
        public Microsoft.OData.Client.EntityStates State { get; set; }
        [DataMember]
        public string SourceProperty { get; set; }
        [DataMember]
        public System.Exception Error { get; set; }
        // [DataMember]
        //public object Entity { get; set; }

        public LinkDescriptorWrapper() { }

        public LinkDescriptorWrapper(LinkDescriptor ed)
        {
            this.State = ed.State;
            this.SourceProperty = ed.SourceProperty;
            //this.Entity = ed.Entity;

        }
    }

    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/System.Data.Test.Astoria")]
    public class OperationResponseWrapper : System.Collections.IEnumerable //, System.Collections.IEnumerator 
    {
        public Microsoft.OData.Client.OperationResponse _OperationResponse = null;
        [DataMember]
        public int InstanceID { get; set; }

        public OperationResponseWrapper()
        {
        }
        public OperationResponseWrapper(OperationResponse orr)
        {
            _OperationResponse = orr;
        }
        [DataMember]
        public System.Collections.Generic.IDictionary<string, string> Headers
        {
            get
            {
                return this._OperationResponse.Headers;
            }
            set
            {
                setValueViaReflection(value, "Headers");
            }
        }
        public void setValueViaReflection(object value, string propertyName)
        {
            PropertyInfo propErrors = Type.GetType("OperationResponse").GetProperty(propertyName);
            if (propErrors != null)
            {
                propErrors.SetValue(_OperationResponse, value, null);
            }
        }
        [DataMember]
        public int StatusCode
        {
            get
            {
                return this._OperationResponse.StatusCode;
            }
            set
            {
                setValueViaReflection(value, "StatusCode");
            }
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }


    }


}

namespace System.Data.Test.Astoria.Tests
{


    public static class ReflectionHelper
    {
        public class Methods
        {
            public const string Where = @"System.Linq.IQueryable`1[TSource] Where[TSource](System.Linq.IQueryable`1[TSource], System.Linq.Expressions.Expression`1[System.Func`2[TSource,System.Boolean]])";
            public const string Select = @"System.Linq.IQueryable`1[TResult] Select[TSource,TResult](System.Linq.IQueryable`1[TSource], System.Linq.Expressions.Expression`1[System.Func`2[TSource,TResult]])";
            public const string SelectMany = @"System.Linq.IQueryable`1[TResult] SelectMany[TSource,TResult](System.Linq.IQueryable`1[TSource], System.Linq.Expressions.Expression`1[System.Func`2[TSource,System.Collections.Generic.IEnumerable`1[TResult]]])";
            public const string Top = @"System.Linq.IQueryable`1[TSource] Take[TSource](System.Linq.IQueryable`1[TSource], Int32)";
            public const string Skip = @"System.Linq.IQueryable`1[TSource] Skip[TSource](System.Linq.IQueryable`1[TSource], Int32)";
            public const string Sort = @"System.Linq.IOrderedQueryable`1[TSource] OrderBy[TSource,TKey](System.Linq.IQueryable`1[TSource], System.Linq.Expressions.Expression`1[System.Func`2[TSource,TKey]])";
            public const string SortDesc = @"System.Linq.IOrderedQueryable`1[TSource] OrderByDescending[TSource,TKey](System.Linq.IQueryable`1[TSource], System.Linq.Expressions.Expression`1[System.Func`2[TSource,TKey]])";
            public const string ThenBy = @"System.Linq.IOrderedQueryable`1[TSource] ThenBy[TSource,TKey](System.Linq.IOrderedQueryable`1[TSource], System.Linq.Expressions.Expression`1[System.Func`2[TSource,TKey]])";
            public const string ThenByDesc = @"System.Linq.IOrderedQueryable`1[TSource] ThenByDescending[TSource,TKey](System.Linq.IOrderedQueryable`1[TSource], System.Linq.Expressions.Expression`1[System.Func`2[TSource,TKey]])";
            public const string OfType = @"System.Linq.IQueryable`1[TResult] OfType[TResult](System.Linq.IQueryable)";
            public const string First = @"TSource First[TSource](System.Linq.IQueryable`1[TSource])";
            public const string FirstOrDefault = @"TSource FirstOrDefault[TSource](System.Linq.IQueryable`1[TSource])";
            public const string Single = @"TSource Single[TSource](System.Linq.IQueryable`1[TSource])";
            public const string SingleOrDefault = @"TSource SingleOrDefault[TSource](System.Linq.IQueryable`1[TSource])";
        }

        public static Dictionary<string, MethodInfo> methodTable = new Dictionary<string, MethodInfo>();

        public static MethodInfo GetStaticMethod(Type type, string name)
        {
            BindingFlags flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.Default;
            MethodInfo methodInfo = null;
            if (!methodTable.TryGetValue(name, out methodInfo))
            {
                foreach (MethodInfo m in type.GetMembers(flags))
                {
                    if (m.ToString() == name)
                    {
                        methodInfo = m;
                        methodTable.Add(name, m);
                    }
                }

            }

            return methodInfo;
        }
    }

}
namespace System.Data.Test.Astoria
{
    public class KVP
    {
        public string Key { get; set; }
        public object Value { get; set; }
    }
}