# Contributing to `odata.net`

Thank you for considering contributing to [odata.net](/). We welcome contributions from the community and are excited to work with you. 
To ensure a smooth and collaborative process, please follow these guidelines.

## Coding Standards

To maintain a high level of code quality and consistency, please follow these coding standards:

### Code Style and Formatting
- Use PascalCase for class names, method names, and properties.
- Use camelCase for local variables and method parameters.
- Prefix interfaces with an "I" (e.g., `ICustomerService`).
- Prefix boolean variables with "is" or "Is" (e.g., `isConnected`).
- Avoid abbreviations or unclear acronyms.
- Do not use `var`; always use explicit types for variable declarations.
- Ensure `await` is followed by `configureAwait(false)` for non-blocking threads.

### Code Structure and Organization
- Organize code into namespaces that reflect the project's structure.
- Place each class in its own file named after the class.
- Use partial classes to split large classes into manageable pieces.
- Group related classes into the same namespace.

### Version Control
- Write clear and concise commit messages.
- Follow a consistent format, such as "Fixes #123: Description of the fix."
- Link the issue type to the pull request (PR) for better tracking and context.

### Testing and Quality Assurance
- Write unit tests for all public methods.
- Use testing frameworks like MSTest, or xUnit.
- Add E2E tests to ensure the entire application flow works as expected.
- Implement a code review process to ensure adherence to coding standards.

### Security and Performance
- Avoid hard-coded secrets and use secure storage mechanisms.
- Avoid including binary files like images, databases, etc., in the repository.
- Use profiling tools to identify and optimize performance bottlenecks. For example:
    - [sharpbench.dev](https://sharpbench.dev/)
    - [BenchmarkDotNet](https://benchmarkdotnet.org/)

## Submitting Contributions

1. Fork the repository and create a new branch for your feature or bug fix.
2. Write clear and concise commit messages.
3. Ensure your code adheres to the coding standards and passes all tests.
4. Submit a pull request with a detailed description of your changes.

## Code of Conduct

Please read and adhere to our [Security](/SECURITY.md)

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information, see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

Thank you for your contributions!