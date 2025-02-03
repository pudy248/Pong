# Coding Standard

## Naming Conventions
As a rule of thumb, we adhere to Microsoft's C# Style Guidelines, with a few notable exceptions. As a rule of thumb, Hungarian notations which encode semantic information in names are not used, except for interface `IPrefixed` notation.
- Variables shall be `camelCase`.
- Functions and classes shall be `PascalCase`.
- Interfaces shall be `IPrefixedPascalCase`.
Private, static, virtual, etc. fields and methods do *not* get special names. IDEs are capable of communicating this information clearly as is. That means no `s_staticVar`, `_privateVar`, `v_VirtualFunc`, and so on.

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
