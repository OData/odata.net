//---------------------------------------------------------------------
// <copyright file="CustomInstanceAnnotationsWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client.CustomInstanceAnnotationsTests.Utils
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Microsoft.OData.Service;
    using System.Linq;
    using Microsoft.OData;

    public class CustomInstanceAnnotationsWriter : DataServiceODataWriter
    {
        private Stack<CustomInstanceAnnotationsDescriptor> writtenItemsStack;
        public static List<CustomInstanceAnnotationsDescriptor> AnnotatedItemsBaseline = null;

        public CustomInstanceAnnotationsWriter(ODataWriter odataWriter)
            : base(odataWriter)
        {
            writtenItemsStack = new Stack<CustomInstanceAnnotationsDescriptor>();
            AnnotatedItemsBaseline = new List<CustomInstanceAnnotationsDescriptor>(); 
        }

        public override void WriteStart(DataServiceODataWriterFeedArgs args)
        {
            CustomInstanceAnnotationsDescriptor current;
            
            if (writtenItemsStack.Count == 0)
            {
                current = new CustomInstanceAnnotationsDescriptor
                {
                    TypeOfAnnotatedItem = typeof(ODataResourceSet),
                    Parent = null,
                    AnnotationsOnStart = new Collection<ODataInstanceAnnotation>(CustomInstanceAnnotationsGenerator.GetAnnotations("AnnotationOnFeed.AddedBeforeWriteStart.").Concat(CustomInstanceAnnotationsGenerator.GetAnnotationsWithTermInMetadata()).ToList()),
                    AnnotationsOnEnd = new Collection<ODataInstanceAnnotation>(CustomInstanceAnnotationsGenerator.GetAnnotations("AnnotationOnFeed.AddedAfterWriteStart.").ToList()),
                };
            }
            else
            {
                current = new CustomInstanceAnnotationsDescriptor 
                { 
                    TypeOfAnnotatedItem = typeof(ODataResourceSet),
                    Parent = writtenItemsStack.Peek(),
                    AnnotationsOnEnd = new Collection<ODataInstanceAnnotation>(), 
                    AnnotationsOnStart = new Collection<ODataInstanceAnnotation>()
                };
            }

            AnnotatedItemsBaseline.Add(current);
            writtenItemsStack.Push(current);

            foreach (var annotation in current.AnnotationsOnStart)
            {
                args.Feed.InstanceAnnotations.Add(annotation);
            }

            base.WriteStart(args);

            foreach (var annotation in current.AnnotationsOnEnd)
            {
                args.Feed.InstanceAnnotations.Add(annotation);
            }
        }

        public override void WriteStart(DataServiceODataWriterEntryArgs args)
        {
            CustomInstanceAnnotationsDescriptor current = new CustomInstanceAnnotationsDescriptor
            {
                TypeOfAnnotatedItem = typeof(ODataResource),
                Parent = writtenItemsStack.Count == 0 ? null : writtenItemsStack.Peek(),
                AnnotationsOnStart = new Collection<ODataInstanceAnnotation>(CustomInstanceAnnotationsGenerator.GetAnnotations("AnnotationOnEntry.AddedBeforeWriteStart.").ToList()),
                AnnotationsOnEnd = new Collection<ODataInstanceAnnotation>(CustomInstanceAnnotationsGenerator.GetAnnotations("AnnotationOnEntry.AddedAfterWriteStart.").Concat(CustomInstanceAnnotationsGenerator.GetAnnotationsWithTermInMetadata()).ToList()),
            };
            AnnotatedItemsBaseline.Add(current);
            writtenItemsStack.Push(current);

            foreach (var annotation in current.AnnotationsOnStart)
            {
                args.Entry.InstanceAnnotations.Add(annotation);
            }

            base.WriteStart(args);

            foreach (var annotation in current.AnnotationsOnEnd)
            {
                args.Entry.InstanceAnnotations.Add(annotation);
            }
        }

        public override void WriteStart(DataServiceODataWriterNestedResourceInfoArgs args)
        {
            var current = new CustomInstanceAnnotationsDescriptor { TypeOfAnnotatedItem = typeof(ODataNestedResourceInfo) };
            writtenItemsStack.Push(current);
            base.WriteStart(args);
        }

        public override void WriteEnd()
        {
            base.WriteEnd();
            writtenItemsStack.Pop();
        }
    }
}
