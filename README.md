![Status](https://github.com/javiertuya/visual-assert/actions/workflows/test.yml/badge.svg)
[![Maven Central](https://img.shields.io/maven-central/v/io.github.javiertuya/visual-assert)](https://search.maven.org/artifact/io.github.javiertuya/visual-assert)
[![Nuget](https://img.shields.io/nuget/v/VisualAssert)](https://www.nuget.org/packages/VisualAssert/)

# visual-assert

Assertion methods that generate an html file with the differences highlighting the additions and deletions. 
Useful for comparing large strings.
Available on Java and .NET platforms.

- From Java include the `visual-assert` dependency as indicated in the 
  [Maven Central Repository](https://search.maven.org/artifact/io.github.javiertuya/visual-assert)
- From .NET include the `VisualAssert` package in you project as indicated in 
  [NuGet](https://www.nuget.org/packages/VisualAssert/)

## Usage

From Java, instantiate the `VisualAssert` class and perform an assert:

```
VisualAssert va = new VisualAssert();
va.assertEquals("abc def ghi\nmno pqr stu", "abc DEF ghi\nother line\nmno pqr stu");
```

This will produce an html file in the `target` directory that highlights the differences (additions in green, deletions in red):

![diff-example](docs/diff-file-example.png "Diff example")

The assert statement is overloaded to specify an additional message and the name of the differences file if required:

```
assertEquals(String expected, String actual, String message)
assertEquals(String expected, String actual, String message, String fileName)
```

From .NET, everything works like Java, only with these differences:

- Method names are capitalized.
- The destination folder is `reports`, located at the level of the project folder.

## Soft assertions

Soft assertions do not throw and exception immediately when an assertion fails, 
but record the assertion message and allow to continue the test and check other assertions.

Class `SoftVisualAssert` implements this type of assertions:
- After a number of assertions, method `assertAll()` will throw the exception 
  if at least one previous assertion failed. The message aggregates the messages of all failed assertions.
- If the soft assert instance is shared by more than one test, `assertClear()` must be called
  before the sequence of assertions to reset the stored messages.
- In addition to `assertEquals` a `fail` assertion is provided.

## Customization

The behaviour of the `VisualAssert` instance can be customized by calling a number of set* methods. 
These methods follow a fluent style, so as, they can be concatenated in a single statement.

- `setReportSubdir(String reportSubdir)`: Sets the folder where generated files with the differences are stored (default is `target`).
- `setSoftDifferences(boolean useSoftDifferences)`: By default (hard), differences in whitespaces are rendered as whitespace html entities and therefore, always visible in the html ouput.
If set to true (soft), some whitespace differences may be hidden from the html output.
- `setUseLocalAbsolutePath(boolean useLocalAbsolutePath)`: If set to true, the link with the differences file will include an file url with the absolute path to the file,
  useful when running tests from a development environment that allows links in the assertion messages (e.g. MS Visual Studio).
- `setShowExpectedAndActual(boolean showExpectedAndActual)`: If set to true, the assert message will include the whole content of the exepcted and actual strings that are compared.
- Note that when using `SoftVisualAssert` the constructor must be cast with `(VisualAssert)` to allow configuation using the above fluent methods.

## Publish from a CI environment

To publish the files with differences to Jenkins you can include the following statement in some steop of the project Jenkinsfile:

```
archiveArtifacts artifacts:'target/*.html', allowEmptyArchive:true
```

To create an artifact including the files with differences using GitHub Actions, you can include the following step in your workflow:

```
  - name: Publish test diff files
    if: always()
    uses: actions/upload-artifact@v2
    with:
      name: Diff files
      path: target/*.html
```
