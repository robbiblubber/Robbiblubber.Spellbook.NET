# Diabolic Data Notation Specification

Version 3.1


## Introduction

The Diabolic Data Notation (ddn) is a data interchange format introduced by
robbiblubber.org in February 2000 for the Diabolic Device Protocol (ddp).

It aims to be lightweight and easy to read and write for humans, as well as to
be easily processable by machines. ddn is basically a hierarchical
key-value-pair format. Although ddn was primarily intended to serve as a data
interchange notation for ddp, it was also widely used for storing
configuration data from the very beginning.

This document describes the specification version 3.1 as of October 2026.
Although ddn was part of the ddp specification until version 2.0, there have
been only minor changes from version 1.1. The specification changes are
tracked in the [version history](#version-history) at the end of this
document.

The robbiblubber.org Spellbook Libraries offer support for ddn.


## ddn Element

In the Diabolic Data Notation (ddn) the basic entity is a ddn Element. Each
element can be a ddn Value or a ddn Section. Every element except for the root
element has a name. Names are unique within their section.


### ddn Value

A ddn Value is a key-value-pair consisting of the name and an associated value.
The value is a string representation of the element's data contents that can
be interpreted as various data types as needed.

The value is assigned to the name with an equals sign, each ddn Value is
terminated by a semicolon. Special characters need to be masked, both in names
and values (see [Masking](#masking)). Leading and trailing whitespace is
ignored in names and values. If a name or value starts or ends with a space,
the space has to be masked.

```
name = value;
```

*Definition: ddn Value*

Readers should be lenient: the semicolon may be omitted before a closing
bracket and at the end of the text, and a name without equals sign and value
(`name;`) is a NULL value.


### ddn Section

A ddn Section is a ddn Element that contains other elements. A ddn Section
starts with its name, followed by any number of ddn Elements enclosed in curly
brackets. Whitespace is ignored.

A section cannot hold a value. A section may be empty.

```
name
{
    ...
}
```

*Definition: ddn Section*


### ddn Root Section

The topmost element is the ddn Root Section. A root section does not have a
name and is not enclosed in curly brackets. It only contains its child
elements.


### Duplicate Names

If a name appears more than once in the same section, a later value replaces
the earlier element, and a later section adds its elements to an earlier
section of the same name. Writers should not produce duplicate names.


## Comments

ddn supports C++ style comments.

A comment starts at the beginning of the text, after whitespace, or directly
after one of the structural characters `;`, `{`, `}` and `=`. Two slashes
inside a value are part of the value, so `url = http://robbiblubber.org;` does
not contain a comment. Writers mask slashes in values anyway.


### Single-Line Comments

Single-line comments start with `//` (two slashes) and end at the line break.

```
... // single-line comment
```

*Definition: Single-line comment*


### Multi-Line Comments

A multi-line comment is defined by the `/*` (slash, asterisk) characters,
followed by any sequence of characters (including line breaks), followed by
the `*/` characters.

```
... /*
    multi-line comment */ ...
```

*Definition: Multi-line comment*


## Masking

Special characters must be masked in ddn. Spaces need to be masked at the
beginning or end of a name or value, tabs and line breaks should always be
masked.

There is also a mask for NULL. It is used as a value on its own and not
combined with other characters.

The following list shows the masks defined in ddn:

| Character Name | Character | Mask | Remarks                                     |
|----------------|-----------|------|---------------------------------------------|
| Space          |           | `\_` | only required at beginning or end           |
| Equals sign    | `=`       | `\=` | unmasked as structural element              |
| Bracket open   | `{`       | `\{` | unmasked as structural element              |
| Bracket closed | `}`       | `\}` | unmasked as structural element              |
| Comma          | `,`       | `\,` | unmasked as structural element              |
| Semicolon      | `;`       | `\;` | unmasked as structural element              |
| Backslash      | `\`       | `\\` |                                             |
| Slash          | `/`       | `\/` | path separator, may start a comment         |
| Line break     |           | `\n` |                                             |
| Tab            |           | `\t` |                                             |
| NULL           |           | `\0` | represents a NULL value                     |

A backslash followed by any other character stands for that character.


## Data Representation

In ddn all data is represented in string format. Consuming applications may
interpret these string values as needed. The specification defines default
representations that are also supported by the library implementations and
are described in the following. Numbers, dates and times never depend on the
culture of the writing or reading system.


### Strings

String values are represented as simple strings. Quotes are not required.

```
str = This is a string;
```

*Sample: a string value*


### Characters

A character is represented as a string of one character. A space is written
as `\_`.

```
sep = \,;
```

*Sample: a character value*


### Integers

Integers are represented as decimal integer strings.

```
d = 28;
```

*Sample: an integer value*

```
d = -1;
```

*Sample: a negative integer value*


### Floating Point Numbers

Floating point values are represented as decimal strings with a dot as decimal
separator. An exponent may be used.

```
pi = 3.1415926536;
```

*Sample: a floating point value*

```
avogadro = 6.02214076E+23;
```

*Sample: a floating point value with exponent*


### Boolean Values

A Boolean is represented by the string expressions `true` and `false`.
Readers also accept numeric values like `0` and `1`, as well as `yes`/`no` and
`on`/`off`, regardless of case.

```
ok = true;
```

*Sample: a Boolean value*


### Enumerations

Enumeration values are represented by their names. Readers compare names
regardless of case and also accept the numeric value. Combined flags are
written as a masked comma-separated list of names.

```
day = Friday;
access = Read\, Write;
```

*Sample: enumeration values*


### Date/Time Values

Date and time values are represented as timestamps with time zone in the form
`yyyy-mm-dd hh:mm:ss.fff+HH:MM` or `yyyy-mm-dd hh:mm:ss.fff-HH:MM`.

All parts may be omitted, except the date: the fraction, the seconds, the
minutes and the whole time (`yyyy-mm-dd`). A missing time is midnight. A
timestamp without time zone denotes the local time of the reading system.
Readers also accept `T` instead of the space between date and time, `Z` for
UTC, and an offset without colon (`+0200`).

```
dat = 1977-08-14 15:27:22.926+02:00;
```

*Sample: a date/time value*

```
due = 2026-10-19;
```

*Sample: a date value*


### Time and Duration Values

A time of day is represented as `hh:mm:ss.fffffff`; the fraction and the
seconds may be omitted. A duration is represented as `[-][d.]hh:mm:ss[.fffffff]`
with an optional number of days.

```
start = 08:30:00;
timeout = 1.12:00:00;
```

*Sample: a time of day and a duration of one and a half days*


### Other Values

Universally unique identifiers are represented in the form
`xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx`. Other data types use their invariant
string representation, for example version numbers (`3.1.0`) and URIs.

```
id = 9b2e4c4a-6f1d-4a8e-9c1e-2f0d6b1a7c33;
web = https:\/\/robbiblubber.org;
```

*Sample: an identifier and a URI*


### NULL

The mask `\0` represents NULL. It is not combined with other characters.

```
n = \0;
```

*Sample: a NULL value*


### Array Values

ddn supports array values. These values consist of a comma-separated list of
string-represented values. The values inside the array follow the rules
described above for data representation, so commas inside an item are masked.
An item may be NULL.

```
arr1 = Peter, Paul, Mary;
```

*Sample: a string array*

```
arr2 = 1, 2, 3;
```

*Sample: an integer array*

```
arr3 = Peter, 2, \0, Mary;
```

*Sample: a mixed array with a NULL item*


### Complex Data Types

The robbiblubber.org Spellbook libraries support serialization of objects to
ddn. Objects are serialized as ddn Sections containing elements for all
serialized members.

In the serialization of arrays of complex types, each array element is
represented by a ddn Element (typically a section) whose name is its index,
starting at 0. Dictionaries are serialized the same way while the key is used
as the element name. Arrays of simple values are written as array values.

A section may contain an element named `$type` that holds the name of the
object's type. It is written only when the serializing application asks for
it, for example to restore objects of derived types, and is not part of the
object's data.

A simple value or array value that is serialized into a root section is
written to an element named `value`.


## Example

```
type = T;                                   // value
time = 2022-11-19 05:33:20+00:00;
winpath = \\\\svr1\\folder;
sec1                                        // section
{
    type = S;
    fab = Cotton;
    n = 20;
    sub1                                    // subsection
    {
        vals = 1.2, 1.4, 1.3;
    }
    sub2
    {
        vals = 2.3, 2.6, 1.7;
    }
}
sec2
{
    /* a multi-line comment in a single line */
    type = X;
    unused = true;
}
```

*Sample: ddn sample*


## ddn Addressing

ddn Elements are referred to by their (absolute or relative) path using the
containing sections separated by a `/` (slash). An absolute path starts at the
Root Element (`/`), a relative path at the current element. `.` denotes the
current element, `..` the parent element. A slash inside a name is masked as
`\/`.

```
/sec1/sub2/vals
```

*Sample: ddn path sample*

```
../sec2/type
```

*Sample: relative path from `sec1`*


## Version History

| Version | Date       | Changes                                                                 |
|---------|------------|-------------------------------------------------------------------------|
| 1.0     | 2000-02-16 | released specification                                                  |
| 1.1     | 2000-03-04 | support for ddn Sections                                                |
| 1.2     | 2001-04-23 | removed equals sign and semicolon in ddn Section declaration            |
| 2.0     | 2009-09-01 | support for array values                                                |
| 2.1     | 2010-02-16 | introduced masked whitespace characters                                 |
| 3.0     | 2022-04-21 | introduced masked character for NULL                                    |
| 3.1     | 2026-10-19 | defined where comments start; enumerations as names; timestamps without time zone are local time; representations for characters, times, durations and identifiers; optional `$type` element; duplicate names |
