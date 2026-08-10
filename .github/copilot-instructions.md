# Copilot instructions for OData .NET

These instructions guide GitHub Copilot (code review and code generation) when working in the
`OData/odata.net` repository. They complement, and do not replace, `.github/CONTRIBUTING.md`,
`.editorconfig`, and the analyzer rules enforced by the build.

## Repository overview

OData .NET provides the core libraries for building and consuming OData v4/v4.01 services and
clients. The main shipping libraries live under `src/`:

- `Microsoft.OData.Core` – ODataLib: readers/writers, URI parser, serialization.
- `Microsoft.OData.Edm` – EDM object model, CSDL parsing/serialization, vocabularies.
- `Microsoft.OData.Client` – OData client (LINQ, materialization, request pipeline).
- `Microsoft.Spatial` – spatial types.

Active development targets the `dev-9.x` branch (the 9.x line, `net10.0`). The `main` branch is the
8.x line. When reviewing, confirm a change is targeting the right branch and framework.

## What to focus on in code review

Prioritize high-confidence, high-impact findings. Report:

- **Correctness bugs**: incorrect logic, off-by-one, wrong operator, null/empty handling, incorrect
  async/await usage, incorrect exception handling or swallowed exceptions.
- **Resource and lifetime issues**: undisposed `IDisposable` (streams, readers/writers,
  `CancellationTokenRegistration`, `CancellationTokenSource`). Every `CancellationToken.Register(...)`
  must have its returned registration disposed; watch for leaks when a long-lived token is reused
  across many calls (see issue #3583).
- **Public API changes**: any change to public/protected surface must be reflected in the
  `PublicAPI.Shipped.txt` / `PublicAPI.Unshipped.txt` files under each project's `PublicAPI/` folder
  (RS0016/RS0017 analyzers). Flag missing or breaking public API changes; breaking changes on a
  shipped surface need explicit justification.
- **Backward/forward compatibility**: OData is a widely consumed library. Flag behavioral changes to
  reader/writer output, URI parsing semantics, or serialization format that could break existing
  consumers or wire compatibility.
- **Spec conformance**: parsing/serialization changes should conform to the OData v4.01 protocol and
  URL conventions. Call out deviations.
- **Performance**: readers/writers and the URI parser are hot paths. Flag unnecessary allocations
  (prefer `ReadOnlySpan<char>`/`ReadOnlyMemory<char>` and existing pooling over `new string(...)`,
  substring, LINQ in hot loops), and repeated work that could be cached.
- **Security**: unbounded input growth, unvalidated input reflected into errors, potential DoS via
  deeply nested payloads or large key/segment counts.

## Coding conventions to enforce

- **`ConfigureAwait(false)`** on every awaited `Task`/`ValueTask` in product (`src/`) library code
  (CA2007 is a warning; product build must be warning-free). Do **not** require it in test code.
- **Culture-aware string APIs**: pass an explicit `StringComparison`/`IFormatProvider`. Use
  `StringComparison.Ordinal` for non-linguistic comparisons and `IndexOf(char, StringComparison)`
  overloads. CA1305 is an **error**, CA1307 a warning in `src/`.
- **`net10.0` / C# latest** on the 9.x line: `Date`/`TimeOfDay` are mapped to `System.DateOnly`/
  `System.TimeOnly`; prefer the `DateOnly`/`TimeOnly` overloads and `EdmValueParser.TryParseDateOnly`/
  `TryParseTimeOnly`. Do not reintroduce the legacy `Microsoft.OData.Edm.Date`/`TimeOfDay` structs on
  this branch.
- **Doc comments**: public and internal members should have XML doc comments; ensure `<see cref=...>`
  references are unambiguous (CS0419) and accurate.
- Follow the surrounding file's existing style (braces, spacing, `this.` usage) rather than imposing a
  new style; `.editorconfig` is authoritative.

## Tests

Every behavioral change must include tests. Follow `.github/CONTRIBUTING.md` test conventions:

- **Project/path/namespace correspondence**: test for `X/Y/Z/A.cs` goes in `X.Tests/Y/Z/ATests.cs`
  with namespace `X.Tests.Y.Z`. Product project `X` maps to test project `X.Tests`.
- Use **xUnit** for new tests (`Assert.*`, `[Fact]`/`[Theory]`). Do not add new MSTest cases.
- Prefer testing at the appropriate level: unit-test the changed component **and**, when behavior is
  observable through the public parser/reader/writer, add an end-to-end test (e.g. a URI-parsing
  change should also be covered by an `ODataUriParser.ParsePath` test).
- Utility/helper files in test projects must **not** end in `Tests`.

## Pull request expectations

- The PR should reference the issue it fixes (`Fixes #xxx`) and check the boxes in
  `.github/PULL_REQUEST_TEMPLATE.md` (tests added; build+test passed).
- Keep changes focused on the linked issue; flag unrelated or scope-creeping edits.

## Review style

- Only raise **high-confidence** issues (bugs, correctness, security, compatibility, missing tests or
  public-API entries). Do not comment on subjective style, formatting, or matters already handled by
  `.editorconfig`/analyzers.
- Be concise and specific: point to the exact line and explain the concrete impact and a suggested
  fix.
