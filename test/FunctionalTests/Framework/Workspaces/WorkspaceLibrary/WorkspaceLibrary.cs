//---------------------------------------------------------------------
// <copyright file="WorkspaceLibrary.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Data.Test.Astoria.TestExecutionLayer;
using System.Xml;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Test.ModuleCore; //TestFailedException;
using System.Reflection;
using System.Xml.Linq;

namespace System.Data.Test.Astoria
{
    public static class WorkspaceLibrary
    {
        public static void CreateDefaultDatabase(Workspace workspace)
        {
            if (workspace.Settings.SkipDataPopulation)
            {
                AstoriaTestLog.WriteLineIgnore("Creating empty database for workspace '" + workspace.Name + "'.");
                workspace.Database = new AstoriaDatabase(workspace.DatabasePrefixName, false);
            }
            else
            {
                AstoriaTestLog.WriteLineIgnore("Creating default database for workspace '" + workspace.Name + "'.");

                workspace.Database = new AstoriaDatabase(workspace.DatabasePrefixName, true);
                ResourceType ldr = workspace.LanguageDataResource();
                if (ldr != null)
                {
                    AstoriaTestLog.WriteLineIgnore("Inserting language data for workspace '" + workspace.Name + "'.");
                    workspace.Database.InsertLanguageData(workspace, ldr);
                }
            }
        }

        public static void AddSilverlightHostFiles(Workspace workspace)
        {
            Assembly resourceAssembly = workspace.GetType().Assembly;

            string webserviceWorkspaceDir = workspace.DataService != null ? workspace.DataService.DestinationFolder : workspace.HostSourceFolder;
            string webServiceBinDir = Path.Combine(webserviceWorkspaceDir, "Bin");
            string tempCLRBinDir = Path.Combine(webserviceWorkspaceDir, "tempCLR");
            string tempSLBinDir = Path.Combine(webserviceWorkspaceDir, "tempSL");
            string webServiceAppCodeDir = Path.Combine(webserviceWorkspaceDir, "App_Code");
            string webServiceClientBinDir = Path.Combine(webserviceWorkspaceDir, "ClientBin");
            IOUtil.EnsureDirectoryExists(webServiceClientBinDir);

            IOUtil.FindAndWriteResourceToFile(resourceAssembly, "AstoriaTestSilverlight.svc", Path.Combine(webserviceWorkspaceDir, "AstoriaTestSilverlight.svc"));
            IOUtil.FindAndWriteResourceToFile(resourceAssembly, "AstoriaTestSilverlightService.cs", Path.Combine(webServiceAppCodeDir, "AstoriaTestSilverlightService.cs"));

            if (AstoriaTestProperties.Client == ClientEnum.SILVERLIGHT)
            {
                File.Copy(Path.Combine(Environment.CurrentDirectory, "SilverlightAstoriaTestPage.html"), Path.Combine(webserviceWorkspaceDir, "SilverlightAstoriaTestPage.html"));
                //GenerateSilverlightXAP(resourceAssembly, webserviceWorkspaceDir, tempCLRBinDir, webServiceAppCodeDir, webServiceClientBinDir);
                File.Copy(Path.Combine(Environment.CurrentDirectory, "SilverlightAstoriaTest.xap"), Path.Combine(webServiceClientBinDir, "SilverlightAstoriaTest.xap"));
            }
        }
        public static void CopyClientAccessPolicyFile(string applicationURI, string dataServiceURI)
        {
            XName xnDomainName = XName.Get("domain");
            XName xnResource = XName.Get("resource");
            XName xaResourcePath = XName.Get("path");
            XName xnResourcesNode = XName.Get("grant-to");
            XName xaSubPathsAllowed = XName.Get("include-subpaths");
            XName xnApplicationsNode = XName.Get("allow-from");
            XName xaApplicationUri = XName.Get("uri");
            string systemDrive = Environment.GetEnvironmentVariable("SystemDrive");
            string clientAccessPolicyFilePath = String.Format("{0}\\inetpub\\wwwroot\\clientaccesspolicy.xml", systemDrive);
            XElement xePolicyFileTemplate = null;
            if (File.Exists(clientAccessPolicyFilePath))
            {
                xePolicyFileTemplate = XElement.Load(clientAccessPolicyFilePath);
            }
            switch (AstoriaTestProperties.PolicyFileType)
            {
                case ClientAccessPolicyFileType.CallerAnyDomain:
                    //we will need to overwrite the existing file if it exists
                    xePolicyFileTemplate = DeleteIfExists(clientAccessPolicyFilePath, xePolicyFileTemplate);
                    xePolicyFileTemplate = GetCapXml("CallerAnyDomain.xml");
                    xePolicyFileTemplate.Save(clientAccessPolicyFilePath);
                    break;
                case ClientAccessPolicyFileType.SubPathsNotallowed:
                    //we will need to overwrite the existing file if it exists
                    xePolicyFileTemplate = DeleteIfExists(clientAccessPolicyFilePath, xePolicyFileTemplate);
                    xePolicyFileTemplate = GetCapXml("SubPathsNotallowed.xml");
                    XElement topLevel = xePolicyFileTemplate.Elements().First();
                    topLevel.Descendants(xnResourcesNode).First().Add(
                      new XElement(xnResource,
                                  new XAttribute(xaResourcePath, dataServiceURI),
                                  new XAttribute(xaSubPathsAllowed, false)
                                  )
                      );
                    xePolicyFileTemplate.Save(clientAccessPolicyFilePath);
                    break;
                case ClientAccessPolicyFileType.NoCapFile:
                    DeleteIfExists(clientAccessPolicyFilePath, xePolicyFileTemplate);
                    break;
                case ClientAccessPolicyFileType.CallerDifferentDomain:
                case ClientAccessPolicyFileType.OnlyServiceRequestAllowed:
                    //we will need to overwrite the existing file if it exists
                    xePolicyFileTemplate = DeleteIfExists(clientAccessPolicyFilePath, xePolicyFileTemplate);
                    xePolicyFileTemplate = GetCapXml("OnlyServiceRequestAllowed.xml");
                    //add this application uri to the 
                    xePolicyFileTemplate.Element(xnResourcesNode).Add(
                      new XElement(xnResource, new XAttribute(xaResourcePath, dataServiceURI))
                      );
                    xePolicyFileTemplate.Element(xnApplicationsNode).Add(
                        new XElement(xnResource, new XAttribute(xaApplicationUri, applicationURI))
                        );
                    xePolicyFileTemplate.Save(clientAccessPolicyFilePath);
                    break;
                case ClientAccessPolicyFileType.HeadersBlocked:
                    xePolicyFileTemplate = DeleteIfExists(clientAccessPolicyFilePath, xePolicyFileTemplate);
                    xePolicyFileTemplate = GetCapXml("HeadersBlocked.xml");
                    xePolicyFileTemplate.Save(clientAccessPolicyFilePath);
                    break;

            }

        }

        private static XElement DeleteIfExists(string clientAccessPolicyFilePath, XElement xePolicyFileTemplate)
        {
            if (xePolicyFileTemplate != null)
            {
                xePolicyFileTemplate = null;
                IOUtil.EnsureFileDeleted(clientAccessPolicyFilePath);
            }
            return xePolicyFileTemplate;
        }

        private static XElement GetCapXml(string templateFileName)
        {
            IOUtil.FindAndWriteResourceToFile(typeof(Workspace).Assembly, templateFileName, Path.Combine(Environment.CurrentDirectory, templateFileName));
            return XElement.Load(Path.Combine(Environment.CurrentDirectory, templateFileName));

        }

        public static T Choose<T>(this IEnumerable<T> list)
        {
            if (list is IList<T>)
                return (list as IList<T>).Choose();
            else
                return list.ToList().Choose();
        }

        public static IEnumerable<T> Choose<T>(this IEnumerable<T> list, int count)
        {
            if (count < 0)
                throw new ArgumentException("Cannot be negative", "count");

            if (list is IList<T>)
                return (list as IList<T>).Choose(count);
            else
                return list.ToList().Choose(count);
        }

        public static IEnumerable<T> Choose<T>(this IEnumerable<T> list, int count, bool allowRepeats)
        {
            if (count < 0)
                throw new ArgumentException("Cannot be negative", "count");

            if (list is IList<T>)
                return (list as IList<T>).Choose(count, allowRepeats);
            else
                return list.ToList().Choose(count, allowRepeats);
        }

        public static T Choose<T>(this IList<T> list)
        {
            if (list.Count == 0)
                return default(T);
            if (list.Count == 1)
                return list[0];
            return list[AstoriaTestProperties.Random.Next(list.Count)];
        }

        public static IEnumerable<T> Choose<T>(this IList<T> list, int count)
        {
            return list.Choose(count, false);
        }

        public static IEnumerable<T> Choose<T>(this IList<T> list, int count, bool allowRepeats)
        {
            if (count < 0)
                throw new ArgumentException("Cannot be negative", "count");

            if (list.Count != 0)
            {
                if (allowRepeats)
                {
                    for (int i = 0; i < count; i++)
                        yield return list.Choose();
                }
                else
                {
                    if (list.Count <= count)
                    {
                        foreach (T element in list)
                            yield return element;
                    }
                    else
                    {
                        List<T> copy = new List<T>(list);
                        for (int i = 0; i < count; i++)
                        {
                            int position = AstoriaTestProperties.Random.Next(copy.Count);
                            T element = copy[position];
                            copy.RemoveAt(position);
                            yield return element;
                        }
                    }
                }
            }
        }

        public static IEnumerable<T> Scramble<T>(this IEnumerable<T> list)
        {
            return list.ToList().Scramble();
        }

        public static T Scramble<T>(this T list) where T : IList, new()
        {
            T newList = new T();

            foreach (object o in list)
                newList.Add(o);

            for (int i = 0; i < list.Count; i++)
            {
                int offset = AstoriaTestProperties.Random.Next(list.Count);
                object temp = newList[i];
                newList[i] = newList[offset];
                newList[offset] = temp;
            }
            return newList;
        }

        public static KeyExpressions Except(this KeyExpressions keys, IEnumerable<KeyExpression> toOmit)
        {
            return new KeyExpressions(keys.Where(k => !toOmit.Contains(k)));
        }
    }
}