using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.Tests;

public class WriteValueHookSkipsPropertyIfNoValueWrittenTests
{
    [Fact]
    public async Task ODataPropertyInfoWriteValue_SkipsProperty_IfValueNotWritten()
    {
        List<BlogPost> posts = [
            new BlogPost
            {
                Id = 1,
                Title = "Test Post",
                Content = null
            },
            new BlogPost
            {
                Id = 2,
                Title = "Another Post",
                Content = "This is some content."
            }
        ];

        var model = CreateEdmModel();
        var odataUri = new ODataUriParser(
            model,
            new Uri("http://service/odata"),
            new Uri("Posts", UriKind.Relative)
        ).ParseUri();

        var options = new ODataSerializerOptions();
        options.AddTypeInfo<BlogPost>(new()
        {
            Properties = [
                new ODataPropertyInfo<BlogPost, int, DefaultState>()
                {
                    Name = "Id",
                    GetValue = (post, state) => post.Id
                },
                new ODataPropertyInfo<BlogPost, string, DefaultState>()
                {
                    Name = "Title",
                    GetValue = (post, state) => post.Title
                },
                new ODataPropertyInfo<BlogPost, string, DefaultState>()
                {
                    Name = "Content",
                    WriteValue = static (post, writer, state) =>
                    {
                        if (post.Content != null)
                        {
                            return writer.WriteValue(post.Content, state);
                        }

                        return true;
                    }
                }
            ]
        });

        var output = new MemoryStream();
        await ODataSerializer.WriteAsync(posts, output, odataUri, model, options);
        output.Position = 0;
        var actual = Encoding.UTF8.GetString(output.ToArray());
        var actualNormalized = JsonSerializer.Serialize(JsonDocument.Parse(actual));
        var expected =
            """
            {
              "@odata.context": "http://service/odata/$metadata#Posts",
              "value": [
                {
                  "Id": 1,
                  "Title": "Test Post"
                },
                {
                  "Id": 2,
                  "Title": "Another Post",
                  "Content": "This is some content."
                }
              ]
            }
            """;
        var expectedNormalized = JsonSerializer.Serialize(JsonDocument.Parse(expected));
        Assert.Equal(expectedNormalized, actualNormalized);
    }

    [Fact]
    public async Task ODataPropertyInfoWriteValueAsync_SkipsProperty_IfValueNotWritten()
    {
        List<BlogPost> posts = [
            new BlogPost
            {
                Id = 1,
                Title = "Test Post",
                Content = null
            },
            new BlogPost
            {
                Id = 2,
                Title = "Another Post",
                Content = "This is some content."
            }
        ];

        var model = CreateEdmModel();
        var odataUri = new ODataUriParser(
            model,
            new Uri("http://service/odata"),
            new Uri("Posts", UriKind.Relative)
        ).ParseUri();

        var options = new ODataSerializerOptions();
        options.AddTypeInfo<BlogPost>(new()
        {
            Properties = [
                new ODataPropertyInfo<BlogPost, int, DefaultState>()
                {
                    Name = "Id",
                    GetValue = (post, state) => post.Id
                },
                new ODataPropertyInfo<BlogPost, string, DefaultState>()
                {
                    Name = "Title",
                    GetValue = (post, state) => post.Title
                },
                new ODataPropertyInfo<BlogPost, string, DefaultState>()
                {
                    Name = "Content",
                    WriteValueAsync = async static (post, writer, state) =>
                    {
                        if (post.Content != null)
                        {
                            await writer.WriteValueAsync(post.Content, state);
                        }
                    }
                }
            ]
        });

        var output = new MemoryStream();
        await ODataSerializer.WriteAsync(posts, output, odataUri, model, options);
        output.Position = 0;
        var actual = Encoding.UTF8.GetString(output.ToArray());
        var actualNormalized = JsonSerializer.Serialize(JsonDocument.Parse(actual));
        var expected =
            """
            {
              "@odata.context": "http://service/odata/$metadata#Posts",
              "value": [
                {
                  "Id": 1,
                  "Title": "Test Post"
                },
                {
                  "Id": 2,
                  "Title": "Another Post",
                  "Content": "This is some content."
                }
              ]
            }
            """;
        var expectedNormalized = JsonSerializer.Serialize(JsonDocument.Parse(expected));
        Assert.Equal(expectedNormalized, actualNormalized);
    }

    [Fact]
    public async Task ODataAsyncPropertyWriter_SkipsProperty_IfValueNotWritten()
    {
        List<BlogPostWithAsyncWriter> posts = [
            new BlogPostWithAsyncWriter
            {
                Id = 1,
                Title = "Test Post",
                Content = null
            },
            new BlogPostWithAsyncWriter
            {
                Id = 2,
                Title = "Another Post",
                Content = "This is some content."
            }
        ];

        var model = CreateEdmModel();
        var odataUri = new ODataUriParser(
            model,
            new Uri("http://service/odata"),
            new Uri("Posts", UriKind.Relative)
        ).ParseUri();

        var options = new ODataSerializerOptions();

        var output = new MemoryStream();
        await ODataSerializer.WriteAsync(posts, output, odataUri, model, options);
        output.Position = 0;
        var actual = Encoding.UTF8.GetString(output.ToArray());
        var actualNormalized = JsonSerializer.Serialize(JsonDocument.Parse(actual));
        var expected =
            """
            {
              "@odata.context": "http://service/odata/$metadata#Posts",
              "value": [
                {
                  "Id": 1,
                  "Title": "Test Post"
                },
                {
                  "Id": 2,
                  "Title": "Another Post",
                  "Content": "This is some content."
                }
              ]
            }
            """;
        var expectedNormalized = JsonSerializer.Serialize(JsonDocument.Parse(expected));
        Assert.Equal(expectedNormalized, actualNormalized);
    }



    private static IEdmModel CreateEdmModel()
    {
        var model = new EdmModel();
        var post = model.AddEntityType("ns", "BlogPost");
        post.AddKeys(post.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
        post.AddStructuralProperty("Title", EdmPrimitiveTypeKind.String);
        post.AddStructuralProperty("Content", EdmPrimitiveTypeKind.String);

        model.AddEntityContainer("ns", "DefaultContainer")
             .AddEntitySet("Posts", post);

        return model;
    }

    class BlogPost
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
    }

    [ODataType("ns.BlogPost")]
    class BlogPostWithAsyncWriter
    {
        public int Id { get; set; }
        public string Title { get; set; }

        [ODataValueWriter(typeof(AsyncContentWriter))]
        public string Content { get; set; }
    }

    class AsyncContentWriter : ODataAsyncPropertyWriter<BlogPostWithAsyncWriter, string, DefaultState>
    {
        public override async ValueTask WriteValueAsync(BlogPostWithAsyncWriter resource, string propertyValue, IStreamValueWriter<DefaultState> writer, ODataWriterState<DefaultState> state)
        {
            if (propertyValue != null)
            {
                await writer.WriteValueAsync(propertyValue, state);
            }
        }
    }
}
