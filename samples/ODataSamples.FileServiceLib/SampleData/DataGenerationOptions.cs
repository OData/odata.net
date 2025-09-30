using System;
using System.Collections.Generic;
using System.Text;

namespace ODataSamples.FileServiceLib.SampleData;

public class DataGenerationOptions
{
    public bool LargeTextPayload { get; set; }
    public bool LargeBinaryPayload { get; set; }
    public bool StreamFileContentText { get; set; }
    public bool StreamFileContentAnnotation { get; set; }
}
