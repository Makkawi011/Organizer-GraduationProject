# Summary:
Organizer a tool called that automates the process of organizing system architecture files and folders based on the developer’s preference. So, for now, that could be used by the client to organize the structure of their source code or a part of it. This will result in more usable source code as it will be easier to surf, access and read.

## Important note:
There are two versions of this project, the first is a Docker Container, which contains a [Console Project](https://github.com/MoMakkawi/Organizer), while (This Version Here) the second is Soucre Generator Run at Compite-Time.

# General idea:
This project is in the field of code generation, exactly in compile-time source code generation (source generator), It generates source code files during compile time.
this project presents a source code generator created with Roslyn source generator (SG) for .NET developers. the SG presented is called Organizer. It meant to organize any unorganized C# code as requested by the client, by restructuring the base types (classes, interfaces, records, enumerations, structs) in different folders and files according to the needs of the client using a set of services. The meaning of unorganized code in the scope of The Organizer is C# code files full of base types.
## The services provided by the Organizer are:
 * A service to create folder(s) to contain generated files.
 * A service to include base type(s) in a specific generated folder depending on a type name or a pattern.
 * A service to change the name(s) of specific type(s) depending on a base type name or pattern of multiple base types.
 * A service to ignore type(s) depending on a base type name or a pattern of multiple types.
 * Ability to exclude specific types from the creation or update patterns depending on a type name.

# About techniques:
 - .NET Compiler Roslyn.
 - .NET Standard 2.0 for source generator development.
 - xUnit.
 - .NET 7.0.

# Very-Well detailed documentation :
#### Contributed by Eng. [@Mohamad Amiri](https://github.com/MohaAmiry) and Reviewed by me.
[The Organizer Source Code Generator At Compile Time Documentation PDF Link](https://github.com/MohaAmiry/Organizer/blob/master/Organizer%20Official%20Document.pdf)
