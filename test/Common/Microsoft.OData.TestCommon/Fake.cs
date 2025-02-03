namespace Microsoft.OData.TestCommon
{
    /// <summary>
    /// A fake <see cref="System.Delegate"/> that is defined to refer to some construct in Microsoft.OData.TestCommon
    /// </summary>
    /// <param name="listener">The <see cref="DebugAssertTraceListener"/> that is referred to</param>
    /// <remarks>
    /// The test projects in this repository share some common build infrastructure through the use of build.props files. One of the shared components is the
    /// <see cref="DebugAssertTraceListener"/>. This component is used to prevent the build server from displaying a UI whenever there is a failure in
    /// <see cref="System.Diagnostics.Debug.Assert(bool)"/> or one of its overloads. Without this <see cref="System.Diagnostics.TraceListener"/>, the 
    /// <see cref="System.Diagnostics.DefaultTraceListener"/> would display a UI and the builder server would hang waiting for the UI to be closed, which would
    /// eventually cause the build to timeout. The <see cref="DebugAssertTraceListener"/> instead throws an <see cref="System.Exception"/>, which will cause an immediate
    /// test failure. This <see cref="DebugAssertTraceListener"/> is configured in the app.config file, which the build.props also adds into each test project, along
    /// with a reference to the Microsoft.OData.TestCommon project. When building, msbuild will see the reference to Microsoft.OData.TestCommon and will copy the
    /// Microsoft.OData.TestCommon.dll file into the output folder for each test project. However, the different test frameworks (mstest and xunit) will not copy all of the build output into the test runner folder. This means that the Microsoft.OData.TestCommon.dll file that is referred to in the app.config will not be present in the test runner folder, which will cause test failures. The test frameworks use reflection to determine which binaries to copy into the test runner folder, so in order for the test frameworks to copy the Microsoft.OData.TestCommon.dll file, this "fake" delegate was created, and the build.props adds it to each test project. The "fake" delegate refers to the <see cref="DebugAssertTraceListener"/>, so now each test project *does* reference Microsoft.OData.TestCommon.dll, and the test frameworks will know to copy that binary into the test runner folder.
    /// 
    /// A delegate was chosen for this purpose because, by itself, it cannot be interacted with and causes no computational overhead. 
    /// </remarks>
    internal delegate void Fake(DebugAssertTraceListener listener);
}
