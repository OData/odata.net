using ODataSamples.FileServiceLib.Api;
using ODataSamples.FileServiceLib.Models;
using ODataSamples.FileServiceLib.Schema.Abstractions;
using ODataSamples.FileServiceLib.Schema.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODataSamples.FileServiceLib.SampleData;

public static class SampleData
{
    public static FindFileResponse CreateMultiFileResponseData(int count)
    {
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
            data[schema.Id] = $"file-{i + 1:D3}";
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
            var sampleText = $"Sample content for file {i + 1} - {department} department";
            data[schema.BinaryData] = Encoding.UTF8.GetBytes(sampleText);
            data[schema.ByteCollection] = Encoding.UTF8.GetBytes($"Additional binary data {i + 1}");
            
            // Create file content
            data[schema.FileContent] = new FileContent
            {
                Text = $"File content for {department} report {i + 1}",
                Annotation = $"Generated sample data - Item {i + 1}"
            };
            
            // Create FileItem instance
            var fileItem = new FileItem(schema, data);
            fileItems.Add(fileItem);
        }
        
        return new FindFileResponse(fileItems, $"skip-token-{count}");
    }
}
