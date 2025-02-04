# Coding Standard

## Naming Conventions
As a rule of thumb, we adhere to Microsoft's C# Style Guidelines, with only a few notable exceptions. Hungarian notations which encode semantic information in names must not be used, except for interface `IPrefixed` notation.

- Variables must be `camelCase`.
- Functions and classes must be `PascalCase`.
- Interfaces must be `IPrefixedPascalCase`.

Private, static, virtual, etc. fields and methods shall not receive special prefixes. IDEs are capable of communicating this information clearly as is. That means no `s_staticVar`, `_privateVar`, `v_VirtualFunc`, and so on. Discarded values must be assigned to the reserved identifier `_` as defined by the C# language specification.

Identifiers shall not use obscure abbreviations. Abbreviations in general MAY be used, but only if they can be understood by non-subject matter experts. Single-character identifiers may be used exclusively as indices, loop elements, trivial temporary or discarded values, and for *simple* and *universally understood* mathematical symbols, for example `float t` to denote time.
```c#
float ktRho; // Invalid - what does this refer to?
float x; // Invalid - not enough context or consistency of use to be understood.
float bsdfAlbedo; // Valid - BSDF is a suitably well-understood abbreviation.
for (int i = 0; i < 10; i++) { ... } // Valid - used as an index.
foreach (Vector2 p : positions) { ... } // Valid - used as a loop element, list name gives context.
var _ = ComputeBounds(); // Valid - used as a discard.
```

# Formatting
Indent using tabs. Open braces shall not be on new lines. This is different from the Visual Studio default, you can change the auto-formatter in Options -> Text Editor -> C# -> Code Style -> Formatting. Control flow keywords have spaces after them. Braces may be omitted around single line statements but must be on new lines.
```c#
for (int i = 0; i < 10; i++) {
    if (someBoolean) 
        DoSomething();
    else {
        DoOtherStuff();
    }
}
```
Avoid over-nesting. In general, do not exceed 3 levels of code indentation (relative to the enclosing function) except in control flow or loops which need such nesting algorithmically. To this end, prefer file-scope namespaces without brackets.

Lines must not exceed 120 columns. Line breaks to comply with this requirement should be avoided unless absolutely required. Expressions should not contain more than 3 nested sets of parentheses unless there are no reasonable segments to extract.

# Data structure design
Avoid overuse of variables when not obviously applicable. For example, while magic numbers and hardcoded values should be avoided, there are scenarios where they are clearly superior to meaningless variables. Hardcoded values should be moved to `const`-marked variables if they fulfill all of the following requirements:

- They will not be altered at either development time or runtime
- They encode a non-trivial value, excluding simple arithmetic values like 2 or 0.5
- They do not refer to another constant which is already named (ex. `Color.red`)
- They have meaning in isolation

Hardcoded values should be moved to `[SerializeField]`-marked private variables if they fulfill all of the following requirements:
- They will not be altered at runtime but will be altered at development time
- They have meaning in isolation
```c#
const float threehalfs = 1.5f; // Invalid - no meaning in isolation and is a simple arithmetic constant.
const float tau = Mathf.Pi * 2; // Invalid - aliases another named constant.
[SerializeField] Key left = Keyboard.A; // Invalid - not expected to change during development.
const float gasConstant = 8.314; // Valid - named, constant, non-trivial, and meaningful.
float 
```
