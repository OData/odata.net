//---------------------------------------------------------------------
// <copyright file="TrustedMethods.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace System.Data.Test.Astoria.FullTrust
{
    using System.Collections;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Security;
    using System.Security.Permissions;
    using Microsoft.Win32;
    using System.Collections.Generic;

    /// <summary>
    /// A collection of methods that are needed by service-side test infrastructure that cannot normally be executed in full trust
    /// </summary>
    public static class TrustedMethods
    {
        /// <summary>
        /// Clears the service's metadata cache using reflection
        /// </summary>
        [ReflectionPermission(SecurityAction.Assert, MemberAccess = true)]
        [SecuritySafeCritical]
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", MessageId = "System.Reflection.Assembly.LoadWithPartialName",
            Justification = "Don't want direct dependence on product, should work no matter what version is in use")]
        public static void ClearMetadataCache()
        {
            Assembly assembly = Assembly.LoadWithPartialName(DataFxAssemblyRef.Name.DataServices);
            if (assembly == null)
            {
                throw new Exception("Could not load " + DataFxAssemblyRef.File.DataServices);
            }

            Type metadataCacheType = assembly.GetType("Microsoft.OData.Service.Caching.MetadataCache`1", true);
            metadataCacheType = metadataCacheType.MakeGenericType(assembly.GetType("Microsoft.OData.Service.Caching.DataServiceCacheItem", true));

            FieldInfo metadataCacheField = metadataCacheType.GetField("cache", BindingFlags.NonPublic | BindingFlags.Static);
            if (metadataCacheField == null)
            {
                throw new Exception("Could not get non-public static field 'cache' from class 'Microsoft.OData.Service.Caching.MetadataCache'");
            }

            IDictionary dictionary = (IDictionary)metadataCacheField.GetValue(null);
            if (dictionary != null)
            {
                dictionary.Clear();
            }
        }

        /// <summary>
        /// Writes the given value to a file at the given path
        /// </summary>
        /// <param name="path">The path to write to</param>
        /// <param name="contents">The contents of the file</param>
        [FileIOPermission(SecurityAction.Assert, Unrestricted = true)]
        [SecuritySafeCritical]
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static void WriteAllText(string path, string contents)
        {
            File.WriteAllText(path, contents);
        }

        /// <summary>
        /// Invokes Path.GetTempPath()
        /// </summary>
        /// <returns>The return value of Path.GetTempPath()</returns>
        [EnvironmentPermission(SecurityAction.Assert, Unrestricted = true)]
        [SecuritySafeCritical]
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static string GetTempPath()
        {
            return Path.GetTempPath();
        }

        /// <summary>
        /// Deletes the file at the given path if it exists
        /// </summary>
        /// <param name="path">The path to the file to delete</param>
        [FileIOPermission(SecurityAction.Assert, Unrestricted = true)]
        [SecuritySafeCritical]
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static void DeleteFileIfExists(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        /// <summary>
        /// Gets the last write time of the file at the given path
        /// </summary>
        /// <param name="path">The path of the file</param>
        /// <returns>The result of File.GetLastWriteTime(path)</returns>
        [FileIOPermission(SecurityAction.Assert, Unrestricted = true)]
        [SecuritySafeCritical]
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static DateTime GetFileLastWriteTime(string path)
        {
            return File.GetLastWriteTime(path);
        }

        /// <summary>
        /// Gets the creation time of the file at the given path
        /// </summary>
        /// <param name="path">The path of the file</param>
        /// <returns>The result of File.GetCreationTime(path)</returns>
        [FileIOPermission(SecurityAction.Assert, Unrestricted = true)]
        [SecuritySafeCritical]
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static DateTime GetFileCreationTime(string path)
        {
            return File.GetCreationTime(path);
        }

        /// <summary>
        /// Copies the files and directories from the source path to the target path
        /// </summary>
        /// <param name="sourcePath">The path to copy from</param>
        /// <param name="targetPath">The path to copy to</param>
        [FileIOPermission(SecurityAction.Assert, Unrestricted = true)]
        [SecuritySafeCritical]
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static void CopyDirectoryRecursively(string sourcePath, string targetPath)
        {
            if (!Directory.Exists(sourcePath))
            {
                return;
            }

            TrustedMethods.EnsureDirectoryExists(targetPath);

            foreach (string filePath in Directory.GetFiles(sourcePath))
            {
                File.Copy(filePath, Path.Combine(targetPath, Path.GetFileName(filePath)), true);
            }

            foreach (string dirPath in Directory.GetDirectories(sourcePath))
            {
                CopyDirectoryRecursively(dirPath, dirPath.Replace(sourcePath, targetPath));
            }
        }

        /// <summary>
        /// If no directory exists at the given path, creates one (and all directories in between)
        /// </summary>
        /// <param name="path">The directory path</param>
        [FileIOPermission(SecurityAction.Assert, Unrestricted = true)]
        [SecuritySafeCritical]
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static void EnsureDirectoryExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        /// <summary>
        /// Gets the location of the given assembly
        /// </summary>
        /// <param name="assembly">The assembly to get the location of</param>
        /// <returns>assembly.Location</returns>
        [FileIOPermission(SecurityAction.Assert, Unrestricted = true)]
        [SecuritySafeCritical]
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static string GetAssemblyLocation(Assembly assembly)
        {
            return assembly.Location;
        }

        /// <summary>
        /// Get the value of the given environment variable
        /// </summary>
        /// <param name="name">The name of environment variable</param>
        /// <returns>value of environment variable</returns>
        [EnvironmentPermission(SecurityAction.Assert, Unrestricted = true)]
        [SecuritySafeCritical]
        public static string GetEnvironmentVariable(string name)
        {
            return Environment.GetEnvironmentVariable(name);
        }

        /// <summary>
        /// Get current directory
        /// </summary>
        /// <returns>Full path of current directory in string format</returns>
        [EnvironmentPermission(SecurityAction.Assert, Unrestricted = true)]
        [SecuritySafeCritical]
        public static string GetCurrentDirectory()
        {
            return Environment.CurrentDirectory;
        }

        /// <summary>
        /// Invoke and return edm object
        /// </summary>
        /// <param name="ci">constructor</param>
        /// <param name="connectionString">Parameter of the constructor</param>
        /// <returns>New instance</returns>
        [EnvironmentPermission(SecurityAction.Assert, Unrestricted = true)]
        [FileIOPermission(SecurityAction.Assert, Unrestricted = true)]
        [SecuritySafeCritical]
        public static object InvokeEdmConstructor(ConstructorInfo ci, string connectionString)
        {
            return ci.Invoke(new object[] { connectionString });
        }

        /// <summary>
        /// Check if file exists
        /// </summary>
        /// <param name="name">Path</param>
        /// <returns>true if file exists</returns>
        [FileIOPermission(SecurityAction.Assert, Unrestricted = true)]
        [SecuritySafeCritical]
        public static bool IsFileExists(string name)
        {
            return File.Exists(name);
        }

        /// <summary>
        /// Check if directory is not empty
        /// </summary>
        /// <param name="directoryName">Path of directory</param>
        /// <returns>true if directory is not empty</returns>
        [FileIOPermission(SecurityAction.Assert, Unrestricted = true)]
        [SecuritySafeCritical]
        public static bool IsDirectoryNotEmpty(string directoryName)
        {
            if (Directory.GetFiles(directoryName).Length > 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Create instance of designated type and parat
        /// </summary>
        /// <param name="type"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [SecurityPermission(SecurityAction.Assert, Unrestricted = true)]
        [SecuritySafeCritical]
        public static object CreateInstance(Type type, object[] parameters)
        {
            return Activator.CreateInstance(type, parameters);
        }

        /// <summary>
        /// Return file paths matching given directory and filename
        /// </summary>
        /// <param name="destinationFolder">Name of directory</param>
        /// <param name="outputFileName">Filename</param>
        /// <returns>All matching file paths</returns>
        [FileIOPermission(SecurityAction.Assert, Unrestricted = true)]
        [SecuritySafeCritical]
        public static string[] GetFilePaths(string destinationFolder, string outputFileName)
        {
            return Directory.GetFiles(destinationFolder, outputFileName, SearchOption.AllDirectories);
        }

        /// <summary>
        /// Get the path of IIS webroot
        /// </summary>
        /// <returns>IIS webroot in string format</returns>
        [EnvironmentPermission(SecurityAction.Assert, Unrestricted = true)]
        [FileIOPermission(SecurityAction.Assert, Unrestricted = true)]
        [SecuritySafeCritical]
        public static string GetIISRootPath()
        {
            return Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\InetStp", "PathWWWRoot", null).ToString();
        }

        /// <summary>
        /// FieldInfo.GetValue(obj)
        /// </summary>
        /// <param name="fi">fieldinfo</param>
        /// <param name="oec">object</param>
        /// <returns>fi.getvalue(oec)</returns>
        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        [SecuritySafeCritical]
        public static object FieldInfoGetValue(FieldInfo fi, object oec)
        {
            return fi.GetValue(oec);
        }

        /// <summary>
        /// FieldInfo.SetValue
        /// </summary>
        /// <param name="fi">field info</param>
        /// <param name="oec">object</param>
        /// <param name="value">value to set</param>
        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        [SecuritySafeCritical]
        public static void FieldInfoSetValue(FieldInfo fi, object oec, object value)
        {
            fi.SetValue(oec, value);
        }

        /// <summary>
        /// Invoke method using property info
        /// </summary>
        /// <param name="pinfo">property info</param>
        /// <param name="observer">observer</param>
        /// <param name="value">func</param>
        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        [SecuritySafeCritical]
        public static void PropertyInfoInvokeMethod(PropertyInfo pinfo, object observer, object value)
        {
            pinfo.GetSetMethod(true).Invoke(observer, new Object[] { value });
        }

        /// <summary>
        /// Invoke method
        /// </summary>
        /// <param name="method">target method</param>
        /// <param name="obj">obj parameter</param>
        /// <param name="parameters">parameters for invoking</param>
        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        [SecuritySafeCritical]
        public static void InvokeMethod(MethodInfo method, object obj, object[] parameters)
        {
             method.Invoke(obj, parameters);
        }

        /// <summary>
        /// Copy files.
        /// </summary>
        /// <param name="source">source file path</param>
        /// <param name="destination">destination file path</param>
        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        [SecuritySafeCritical]
        public static void CopyFile(string source, string destination)
        {
            File.Copy(source, destination, true);
        }

        /// <summary>
        /// Open text stream for files
        /// </summary>
        /// <param name="path">file path</param>
        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        [SecuritySafeCritical]
        public static StreamReader FileOpenText(string path)
        {
            return File.OpenText(path);
        }

        /// <summary>
        /// Execute method call expression
        /// </summary>
        /// <param name="queryable">instance of IQueryable</param>
        /// <param name="queryableSingle">method call expression</param>
        /// <returns>object</returns>
        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        [SecuritySafeCritical]
        public static object IQueryableProviderExecuteMethodCall(IQueryable queryable, object queryableSingle)
        {
            return queryable.Provider.Execute((MethodCallExpression)queryableSingle);
        }

        /// <summary>
        /// return result of function
        /// </summary>
        /// <typeparam name="T">return type</typeparam>
        /// <param name="f">function call</param>
        /// <returns>value with type T</returns>
        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        [SecuritySafeCritical]
        public static T ReturnFuncResult<T>(Func<T> f)
        {
            return f();
        }

        /// <summary>
        /// Get enumerator of IQueryable
        /// </summary>
        /// <param name="queryable">instance of IQueryable</param>
        /// <returns>enumerator of IQueryable</returns>
        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        [SecuritySafeCritical]
        public static IEnumerator IQueryableGetEnumerator(IQueryable queryable)
        {
            return queryable.GetEnumerator();
        }

        /// <summary>
        /// Get result list from IQueryable
        /// </summary>
        /// <param name="queryable">instance of IQueryable</param>
        /// <returns>result list of IQueryable</returns>
        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        [SecuritySafeCritical]
        public static List<object> IQueryableGetResultList(IQueryable queryable)
        {
            var list = new List<object>();
            foreach (var element in queryable)
            {
                list.Add(element);
            }

            return list;
        }

    }
}
