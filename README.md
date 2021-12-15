# visual-assert

Assertion methods that generate an html file with the differences highlighting the additions and deletions. 
Useful when comparing large strings.

- From Java include the dependency as indicated in the 
  [Maven Central Repository](https://search.maven.org/artifact/io.github.javiertuya/visual-assert/2.0.0/jar)
- From .NET include the package in you project as indicated in 
  [NuGet](https://www.nuget.org/packages/VisualAssert/)

## Usage

From Java, instantiate the `VisualAssert` class and perform an assert:

```
VisualAssert va = new VisualAssert();
va.assertEquals("abc def ghi\nmno pqr stu", "abc DEF ghi\nother line\nmno pqr stu");
```

This will produce an html file at the `target` directory that highlights the differences (additions in green, deletions in red):

![diff-example](docs/diff-file-example.png "Diff example")

From .NET, everything works like Java, only with these differences:

- Method names are capitalized.
- The destination folder is `reports`, located at the level of the project folder.

## Customization

The behaviour of the `VisualAssert` instance can be customized by calling a number of set* methods. 
These methods follow a fluent style, so as, they can be concatenated in a single statement.

- `setReportSubdir(String reportSubdir)`: Sets the folder where generated files with the differences are stored (default is `target`).
- `setUseLocalAbsolutePath(boolean useLocalAbsolutePath)`: If set to true, the link with the differences file will include an file url with the absolute path to the file,
  useful when running tests from a development environment that allows links in the assertion messages (e.g. MS Visual Studio).
- `setShowExpectedAndActual(boolean showExpectedAndActual)`: If set to true, the assert message will include the whole content of the exepcted and actual strings that are compared.

The assert statement is overloaded to specify an additional message and the name of the differences file if required:

```
assertEquals(String expected, String actual, String message, String fileName)
```

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
