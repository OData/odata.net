//---------------------------------------------------------------------
// <copyright file="DbContextHelper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Providers
{
    using System;
    using System.Collections.Generic;
#if EF6Provider
    using System.Data.Entity.Core.Objects;
#else
    using System.Data.Objects;
#endif
    using System.Diagnostics;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text;
    using System.Threading;

    /// <summary>
    /// A helper class to extract the ObjectContext and save method from a DbContext
    /// </summary>
    internal static class DbContextHelper
    {
        /// <summary>
        /// A cache of CLR types to their ObjectContextAccessor. Stores null if no ObjectContextAccessor can exist for a given type.
        /// </summary>
        private static readonly Dictionary<Type, DbContextAccessor> contextAccessorCache = new Dictionary<Type, DbContextAccessor>(EqualityComparer<Type>.Default);
        
        /// <summary>
        /// A lock for contextAccessorCache
        /// </summary>
        private static readonly ReaderWriterLockSlim cacheLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        /// <summary>
        /// Determines if a type is a DbContext
        /// </summary>
        /// <param name="type">The type to check</param>
        /// <returns>True is the type is a DbContext</returns>
        public static bool IsDbContextType(Type type)
        {
            return GetDbContextAccessor(type) != null;
        }

        /// <summary>
        /// Returns the ObjectContext for a particular instance. Either:
        ///   1. The object is an ObjectContext, or
        ///   2. The object is a DbContext and the ObjectContext can be retrieved
        /// </summary>
        /// <param name="o">The data source instance</param>
        /// <returns>The ObjectContext instance or null</returns>
        public static ObjectContext GetObjectContext(object o)
        {
             ObjectContext objectContext = o as ObjectContext;

            if (objectContext == null)
            {
                DbContextAccessor accessor = GetDbContextAccessor(o.GetType());
                if (accessor != null)
                {
                    objectContext = accessor.GetContext(o) as ObjectContext;
                }
            }

            return objectContext;
        }

        /// <summary>
        /// Returns the ObjectContext and SaveChanges method for a particular instance. Either:
        ///   1. The object is an ObjectContext, or
        ///   2. The object is a DbContext and the ObjectContext can be retrieved
        /// </summary>
        /// <param name="o">The data source instance</param>
        /// <param name="objectContext">The ObjectContext instance or null</param>
        /// <param name="saveChangesMethod">The SaveChanges method or null</param>
        public static void GetObjectContext(object o, out ObjectContext objectContext, out Func<int> saveChangesMethod)
        {
            objectContext = o as ObjectContext;
            saveChangesMethod = null;
            if (objectContext == null)
            {
                DbContextAccessor accessor = GetDbContextAccessor(o.GetType());
                if (accessor != null)
                {
                    objectContext = accessor.GetContext(o);
                    saveChangesMethod = () => { return accessor.SaveChanges(o); };
                }
            }
            else
            {
                saveChangesMethod = objectContext.SaveChanges;
            }
        }

        /// <summary>
        /// Determines if the exception is of the type DbEntityValidationException
        /// </summary>
        /// <param name="e">The exception instance to check.</param>
        /// <returns>True if the exception is derived from DbEntityValidationException, and False otherwise.</returns>
        public static bool IsDbEntityValidationException(Exception e)
        {
            return DbEntityValidationExceptionAccessor.IsDbEntityValidationException(e);
        }

        /// <summary>
        /// Wraps the exception given in a new exception, and gets a verbose message
        /// from the DbEntityValidationException derived instance.
        /// </summary>
        /// <param name="e">The DbEntityValidationException instance to be used to create the more verbose exception.</param>
        /// <returns>A new exception with a verbose message, and an innerException of the passed in exception.</returns>
        public static Exception WrapDbEntityValidationException(Exception e)
        {
            DbEntityValidationExceptionAccessor validation = new DbEntityValidationExceptionAccessor(e);
            return new InvalidOperationException(validation.CreateVerboseMessage(), e);
        }

        /// <summary>
        /// Gets the DbContext accessor for a type, or returns null if one cannot be made
        /// </summary>
        /// <param name="type">The type to analyze</param>
        /// <returns>The DbContextAccessor for the type, or null</returns>
        private static DbContextAccessor GetDbContextAccessor(Type type)
        {
            DbContextAccessor accessor;
            cacheLock.EnterUpgradeableReadLock();
            try
            {
                // Check if there is an accessor while in read lock
                if (!contextAccessorCache.TryGetValue(type, out accessor))
                {
                    cacheLock.EnterWriteLock();
                    try
                    {
                        // Make sure there is still no reader
                        if (!contextAccessorCache.TryGetValue(type, out accessor))
                        {
                            // Create a reader and add it to the cache
                            contextAccessorCache.Add(type, accessor = CreateDbContextAccessor(type));
                        }
                    }
                    finally
                    {
                        cacheLock.ExitWriteLock();
                    }
                }
            }
            finally
            {
                cacheLock.ExitUpgradeableReadLock();
            }

            return accessor;
        }

        /// <summary>
        /// Creates a DbContextAccessor for a given type if it exists
        /// </summary>
        /// <param name="type">The type to create the accessor for</param>
        /// <returns>A DbContextAccessor, or null if one cannot be created for the Type (i.e. it does not inherit from DbContext)</returns>
        private static DbContextAccessor CreateDbContextAccessor(Type type)
        {
            const BindingFlags DbContextBindingFlags = BindingFlags.Public | BindingFlags.Instance;
            const string DbContextTypeName = "System.Data.Entity.DbContext";
            const string IObjectContextAdapterTypeName = "System.Data.Entity.Infrastructure.IObjectContextAdapter";
           
            DbContextAccessor accessor = null;
            bool derivesFromDbContext = IsTypeOf(type, DbContextTypeName);

            if (derivesFromDbContext)
            {
                Type contextAdapterInterface = type.GetInterface(IObjectContextAdapterTypeName, false);
                if (contextAdapterInterface != null)
                {
                    PropertyInfo contextProperty = contextAdapterInterface.GetProperty("ObjectContext", DbContextBindingFlags);
                    if (contextProperty != null && contextProperty.GetGetMethod() != null)
                    {
                        MethodInfo saveChanges = type.GetMethod("SaveChanges", DbContextBindingFlags, null, Type.EmptyTypes, null);
                        if (saveChanges != null)
                        {
                            accessor = new DbContextAccessor();

                            // Generate the ObjectContext getter
                            ParameterExpression contextParameter = Expression.Parameter(typeof(object));
                            
                            accessor.GetContext = Expression.Lambda<Func<object, ObjectContext>>(
                                Expression.Property(
                                    Expression.Convert(contextParameter, contextAdapterInterface),
                                    contextProperty.GetGetMethod()),
                                contextParameter).Compile();

                            // Generate the SaveChanges method
                            accessor.SaveChanges = Expression.Lambda<Func<object, int>>(
                                Expression.Call(
                                    Expression.Convert(contextParameter, type),
                                    saveChanges),
                                contextParameter).Compile();
                        }
                    }
                }
            }

            return accessor;
        }

        /// <summary>
        /// Determines through reflection based on names if the type passed in a derived from the type name passed in.
        /// </summary>
        /// <param name="type">The type to have its derivation checked.</param>
        /// <param name="fromTypeName">The name of the type to check for derivation from</param>
        /// <returns>True if the type is of the type the TypeName refers to, False otherwise.</returns>
        private static bool IsTypeOf(Type type, string fromTypeName)
        {
            bool isDerivedFrom = false;
            Type typeToCheck = type;

            while (!isDerivedFrom && typeToCheck != typeof(object) && typeToCheck != null)
            {
                isDerivedFrom = String.Equals(typeToCheck.FullName, fromTypeName, StringComparison.Ordinal);
                typeToCheck = typeToCheck.BaseType;
            }

            return isDerivedFrom;
        }

        /// <summary>
        /// A tuple of the ObjectContext getter and SaveChanges method
        /// </summary>
        private class DbContextAccessor
        {
            /// <summary>A func to return the ObjectContext for an instance of a context</summary>
            public Func<object, ObjectContext> GetContext { get; set; }

            /// <summary>A func to call SaveChanges for an instance of a context</summary>
            public Func<object, int> SaveChanges { get; set; }
        }

        /// <summary>
        /// Class to wrap the DbEntityValidationException type so we don't have to staticly link to the EntityFramework.dll
        /// </summary>
        private class DbEntityValidationExceptionAccessor
        {
            /// <summary>The name of the type that this class is wrapping</summary>
            private const string DbEntityValidationExceptionTypeName = "System.Data.Entity.Validation.DbEntityValidationException";

            /// <summary>The instance that is being wrapped</summary>
            private readonly Exception instance;

            /// <summary>
            /// Creates an instance of this wrapper from the base Exception instance.
            /// </summary>
            /// <param name="instance">the DbEntityValidationException instance to be wrapped</param>
            public DbEntityValidationExceptionAccessor(Exception instance)
            {
                Debug.Assert(IsDbEntityValidationException(instance), "passed an exception of the wrong type");
                this.instance = instance;
            }

            /// <summary>
            /// Determines if the exception is of the type DbEntityValidationException
            /// </summary>
            /// <param name="e">The exception instance to check.</param>
            /// <returns>True if the exception is derived from DbEntityValidationException, and False otherwise.</returns>
            public static bool IsDbEntityValidationException(Exception e)
            {
                return DbContextHelper.IsTypeOf(e.GetType(), DbEntityValidationExceptionTypeName);
            }

            /// <summary>
            /// Walks through the collections on DbEntityValidationException, and DbEntityValidationResult 
            /// to get the messages from the DbValidationError objects, and appends them together.
            /// </summary>
            /// <returns>The appended list of DbValidationError messages</returns>
            public string CreateVerboseMessage()
            {
                const string ResultsCollectionPropertyName = "EntityValidationErrors";
                const string ErrorsCollectionPropertyName = "ValidationErrors";
                const string ErrorMessagePropertyName = "ErrorMessage";

                Type type = this.instance.GetType();
                PropertyInfo resultProperty = type.GetProperty(ResultsCollectionPropertyName);
                StringBuilder message = new StringBuilder();
                PropertyInfo errorsProperty = null;
                PropertyInfo errorMessageProperty = null;
                bool firstLine = true;
                
                foreach (object result in (System.Collections.IEnumerable)resultProperty.GetValue(this.instance, null))
                {
                    if (errorsProperty == null)
                    {
                        errorsProperty = result.GetType().GetProperty(ErrorsCollectionPropertyName);
                    }
                    
                    foreach (object error in (System.Collections.IEnumerable)errorsProperty.GetValue(result, null))
                    {
                        if (errorMessageProperty == null)
                        {
                            errorMessageProperty = error.GetType().GetProperty(ErrorMessagePropertyName);
                        }

                        if (firstLine)
                        {
                            firstLine = false;
                        }
                        else
                        {
                            message.AppendLine();
                        }

                        message.Append((string)errorMessageProperty.GetValue(error, null));
                    }
                }
                
                return message.ToString();
            }
        }
    }
}
