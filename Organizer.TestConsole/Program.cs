using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Organizer.Client;
using Organizer.Client.Attributes;
using Organizer.Controller;

var code = File.ReadAllText("C:\\Users\\a\\Desktop\\Labs\\Organizer\\Organizer.TestConsole\\Program.cs");
var code12 = @"
class MyClass
{
    void MyMethod()
    {
        Console.WriteLine(""Hello, world!"");
    }
}
class class2
{
    MyClass x = new();
    MyClassmyClass x = new();
    void MyMethod()
    {
        Console.WriteLine(""Hello, world!"");
    }
}
";


string updatedCode = Regex.Replace(code12, @"\bMyClass\b", "NewClassName");

Console.WriteLine(updatedCode);


var tree = CSharpSyntaxTree.ParseText(code);

var organizerCtor = new[] { tree }
    .GetClasses()
    .GetOrganizerClass()
    .GetOrganizerConstructor();

MyClass? MyClass1 = null;
Console.WriteLine();

Console.WriteLine("hi ");
class Code : OrganizerServices
{
    [From("C:\\Users\\a\\Desktop\\Labs\\Organizer\\Organizer.TestLibrary")]
    [To("C:\\Users\\a\\Desktop\\Labs\\Organizer\\Organizer.TestLibrary")]
    public Code()
    {
        IgnoreType("Garpeg");
        IgnoreTypes("EndPoint");

        CreateFolder("API");
        {

            CreateFolder("Models");
            {
                ContainType(nameof(Code));
                ContainTypes("C");
            }
            CreateFolder("Request");
            {
                ContainTypes("Req", "req");
            }
            CreateFolder("Responce");
            {
                UpdateType(nameof(Code), "NewCode");
            }
        }
    }
}

class MyClass
{
    [NotNull]
    public int MyProperty { get; set; }
    public MyClass()
    {

    }
}