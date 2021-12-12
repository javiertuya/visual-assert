# visual-assert
A set of assertion methods useful for comparing large strings that generates an html file with the differences highlighting the additions and deletions.

Currenty only java version, .NET version will be available soon

## Usage

Instantiate the `VisualAssert` class and perform an assert:

```
VisualAssert va = new VisualAssert();
va.assertEquals("abc def ghi\nmno pqr stu", "abc DEF ghi\nother line\nmno pqr stu");
```

This will produce an html file at the `target` directory that highlights the differences (additions in green, deletions in red).

The behaviour of the `VisualAssert` instance can be customized by calling the set* methods.

TODO

The assert can specify an additional message and the name of the differences file if required:

TODO

## Using from your CI environment

TODO