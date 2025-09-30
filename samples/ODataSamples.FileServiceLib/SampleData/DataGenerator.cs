using ODataSamples.FileServiceLib.Api;
using ODataSamples.FileServiceLib.Models;
using ODataSamples.FileServiceLib.Schema.Abstractions;
using ODataSamples.FileServiceLib.Schema.Common;
using ODataSamples.FileServiceLib.Streaming;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODataSamples.FileServiceLib.SampleData;

public static class DataGenerator
{
    public const int LargeFieldMultiplier = 16 * 1024; // 16K // makes sure the size exceeds default buffer size

    public static FindFileResponse CreateMultiFileResponseData(int count, DataGenerationOptions? options = null)
    {
        options ??= new DataGenerationOptions();
        var schema = new FileItemSchema();
        var fileItems = new List<ISchematizedObject<IFileItemSchema>>();
        
        // Create deterministic sample data for testing and benchmarking
        var fileTypes = new[] { ".pdf", ".docx", ".xlsx", ".pptx", ".txt", ".jpg", ".png", ".mp4", ".zip", ".xml" };
        var departments = new[] { "Marketing", "Engineering", "Sales", "HR", "Finance", "Operations", "Legal", "Design", "Support", "Research" };
        var baseDate = new DateTimeOffset(2024, 1, 1, 9, 0, 0, TimeSpan.Zero);
        
        for (int i = 0; i < count; i++)
        {
            var data = new Dictionary<IPropertyDefinition, object?>();
            var fileType = fileTypes[i % fileTypes.Length];
            var department = departments[i % departments.Length];
            
            // Set required properties
            data[schema.Id] = $"file-{i + 1}";
            data[schema.FileName] = $"{department}Report{i + 1:D2}{fileType}";
            data[schema.FileExtension] = fileType;
            data[schema.FileSize] = (long)(1024 * (i + 1) * 10); // Incremental file sizes
            data[schema.Version] = (i % 5) + 1; // Versions 1-5
            data[schema.IsProtected] = (i % 3) == 0; // Every 3rd file is protected
            data[schema.Description] = $"Sample {department.ToLower()} document for testing and demonstration purposes - Item {i + 1}";
            data[schema.ExternalId] = new Guid($"00000000-0000-0000-0000-{(i + 1):D12}");
            data[schema.CreatedAt] = baseDate.AddDays(i).AddHours(i % 24);
            
            // Set collection properties
            data[schema.Tags] = new List<string> 
            { 
                department.ToLower(), 
                "sample", 
                $"priority-{(i % 3) + 1}",
                fileType.TrimStart('.')
            };
            
            // Create ActivityStats collection
            var activityStats = new List<ActivityStat>
            {
                new ActivityStat 
                { 
                    Id = $"activity-{i + 1}-1",
                    Activity = FileAccessType.Access, 
                    Actor = $"user{(i % 5) + 1}@example.com",
                    ActivityDateTime = data[schema.CreatedAt] as DateTimeOffset? ?? DateTimeOffset.Now 
                },
                new ActivityStat 
                { 
                    Id = $"activity-{i + 1}-2",
                    Activity = FileAccessType.Edit, 
                    Actor = $"editor{(i % 3) + 1}@example.com",
                    ActivityDateTime = ((DateTimeOffset)data[schema.CreatedAt]!).AddDays(i % 7) 
                }
            };
            data[schema.ActivityStats] = activityStats;

            // Create sample binary data
            var sampleBinaryDataAsString = $"Sample content for file {i + 1} - {department} department";
            if (options.LargeBinaryPayload)
            {
                sampleBinaryDataAsString = RepeatString(sampleBinaryDataAsString, LargeFieldMultiplier);
            }

            data[schema.BinaryData] = Encoding.UTF8.GetBytes(sampleBinaryDataAsString);

            // TODO: We use List<byte> to represent a byte collection because
            // byte[] is automatically treated as binary data that will be base64-encoded.
            // This might change in a future iteration.
            data[schema.ByteCollection] = new List<byte> { 10, 20, 13, 42, 54 };

            // Create file content
            var fileContentText = $"File content for {department} report {i + 1}";
            var fileContentAnnotation = $"Generated sample data - Item {i + 1}";
            if (options.LargeTextPayload)
            {
                fileContentText = RepeatString(fileContentText, LargeFieldMultiplier);
                fileContentAnnotation = RepeatString(fileContentAnnotation, LargeFieldMultiplier);
            }

            var fileContent = new FileContent();
            if (options.StreamFileContentText)
            {
                fileContent.StreamableProperties ??= new Dictionary<string, IStreamingSource>();
                fileContent.StreamableProperties[nameof(FileContent.Text)] = new ByteArrayStreamingSource(Encoding.Unicode.GetBytes(fileContentText));
            }

            if (options.StreamFileContentAnnotation)
            {
                fileContent.StreamableProperties ??= new Dictionary<string, IStreamingSource>();
                fileContent.StreamableProperties[nameof(FileContent.Annotation)] = new ByteArrayStreamingSource(Encoding.Unicode.GetBytes(fileContentAnnotation));
            }

            data[schema.FileContent] = new FileContent
            {
                Text = fileContentText,
                Annotation = fileContentAnnotation
            };


            var accessControlEntries = new List<AccessControlEntry>
            {
                new AccessControlEntry 
                { 
                    AccessRight = (i % 2) == 0 ? AccessRight.Read : AccessRight.Write,
                    Claim = new Claim 
                    { 
                        Type = "user", 
                        Value = $"user{(i % 5) + 1}@example.com" 
                    }
                },
                new AccessControlEntry 
                { 
                    AccessRight = AccessRight.Read,
                    Claim = new Claim 
                    { 
                        Type = "group", 
                        Value = $"{department.ToLower()}-group" 
                    }
                }
            };

            data[schema.EntityACL] = new AccessControlList 
            { 
                Version = 1, 
                AccessControlEntries = accessControlEntries 
            };
            
            // Create AllExtensionsNames collection
            var extensionNames = new List<string> 
            { 
                "CustomMetadata", 
                "ProcessingInfo", 
                $"{department}Extension" 
            };
            data[schema.AllExtensionsNames] = extensionNames;
            
            // Create AllExtensions open property value
            var allExtensions = new ExtensionOpenPropertyValue();
            allExtensions.SetODataProperty("CustomMetadata", new ExtensionOpenPropertyValue(new Dictionary<string, object> 
            { 
                ["author"] = $"user{(i % 5) + 1}@example.com",
                ["category"] = department,
                ["priority"] = (i % 3) + 1
            }));
            allExtensions.SetODataProperty("ProcessingInfo", new ExtensionOpenPropertyValue(new Dictionary<string, object> 
            { 
                ["processed"] = (i % 2) == 0,
                ["processedDate"] = baseDate.AddDays(i).AddHours(2),
                ["processingEngine"] = "SampleEngine v1.0"
            }));

            data[schema.AllExtensions] = allExtensions;
            
            // Create ItemProperties dynamic open property value
            var itemProperties = new ExtensionOpenPropertyValue();
            itemProperties.SetODataProperty("customField1", $"value-{i + 1}");
            itemProperties.SetODataProperty("customField2", (i + 1) * 100);
            itemProperties.SetODataProperty("customField3", (i % 2) == 0);
            itemProperties.SetODataProperty("department", department);
            itemProperties.SetODataProperty("fileAge", i + 1);
            itemProperties.SetODataProperty("lastModifiedBy", $"editor{(i % 3) + 1}@example.com");
            itemProperties.SetODataProperty(
                "links",
                new IOpenPropertyValue[]
                {
                    new ExtensionOpenPropertyValue(new Dictionary<string, object>
                    {
                        ["rel"] = "self",
                        ["href"] = $"http://service/odata/Users('id')/Files('file-{i + 1}')"
                    }),
                    new ExtensionOpenPropertyValue(new Dictionary<string, object>
                    {
                        ["rel"] = "download",
                        ["href"] = $"http://service/odata/Users('id')/Files('file-{i + 1}')/$value"
                    }),
                    new ExtensionOpenPropertyValue(new Dictionary<string, object>
                    {
                        ["rel"] = "website",
                        ["href"] = $"http://example.com/files/{data[schema.FileName]}"
                    })
                }
             );
            
            // Add annotations for some properties
            itemProperties.SetODataAnnotation("customField1", "is.queryable", true);
            data[schema.ItemProperties] = itemProperties;
            
            // Create FileItem instance
            var fileItem = new FileItem(schema, data);
            fileItems.Add(fileItem);
        }
        
        return new FindFileResponse(fileItems, $"skip-token-{count}");
    }

    private static string RepeatString(string str, int count)
    {
        if (count <= 0) return string.Empty;
        return string.Concat(Enumerable.Repeat(str, count));
    }
}
