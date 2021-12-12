# visual-assert

Assertion methods that generate an html file with the differences highlighting the additions and deletions. 
Useful when comparing large strings.

Currenty only available the java version, .NET version will be available soon

## Usage

Instantiate the `VisualAssert` class and perform an assert:

```
VisualAssert va = new VisualAssert();
va.assertEquals("abc def ghi\nmno pqr stu", "abc DEF ghi\nother line\nmno pqr stu");
```

This will produce an html file at the `target` directory that highlights the differences (additions in green, deletions in red).

The behaviour of the `VisualAssert` instance can be customized by calling the set* methods. 
These methods follow a fluent style, so as, they can be concatenated in a single statement.

- `setReportSubdir(String reportSubdir)`: Sets the folder where generated files with the differences are stored (`target`).
- `setUseLocalAbsolutePath(boolean useLocalAbsolutePath)`: If set to true, the link with the differences file will include an file url with the absolute path to the file,
  useful when running tests from a development environment that allows links in the messages (e.g. MS Visual Studio).
- `setShowExpectedAndActual(boolean showExpectedAndActual)`: If set to true, the assert message will include the whole content of the exepcted and actual strings that are compared.

The assert statement is overloaded to specify an additional message and the name of the differences file if required:

```
assertEquals(String expected, String actual, String message, String fileName)
```

## Using from your CI environment

TODO