using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Organizer.Client.Attributes;

namespace Organizer.Controller
{
    internal static class Files
    {

        internal static string GetTargetDirectoryPath(this ConstructorDeclarationSyntax ctor)
        {
            var toPath = ctor
                .GetAttributes(typeof(To))
                .Single()
                .GetPath();

            return File.Exists(toPath) ? Path.GetDirectoryName(toPath) : toPath;
        }

        internal static IEnumerable<BaseTypeDeclarationSyntax> GetCustomerTypeDeclarationSyntaxes(this ConstructorDeclarationSyntax organizerCtor)
            => organizerCtor
                .GetAttributes(typeof(From))
                .GetPaths()
                .GetFilesContents()
                .GetBaseTypeSyntaxes();
        internal static IEnumerable<BaseTypeDeclarationSyntax> GetTypeDeclarationSyntaxesFrom(string code)
            => CSharpSyntaxTree.ParseText(code)
            .GetRoot()
            .DescendantNodes()
            .OfType<BaseTypeDeclarationSyntax>();

        private static IEnumerable<string> GetPaths(this IEnumerable<AttributeSyntax> attributes)
        {
            return attributes
            .SelectMany(a =>
            {
                List<string> paths = new List<string>();

                var inputPath = a.GetPath();
                if (File.Exists(inputPath)) paths.Add(inputPath);
                else paths.AddRange(inputPath.GetCSFilesPaths());

                return paths;
            });
        }

        private static string GetPath(this AttributeSyntax attribute)
        => attribute
            .ArgumentList
            .Arguments
            .First()
            .ToString()
            .Replace("\"", string.Empty);

        private static IEnumerable<string> GetCSFilesPaths(this string path)
            => Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories);

        private static IEnumerable<string> GetFilesContents(this IEnumerable<string> paths)
            => paths.Select(path => File.ReadAllText(path));

        private static IEnumerable<BaseTypeDeclarationSyntax> GetBaseTypeSyntaxes(this IEnumerable<string> codes)
            => codes.SelectMany(code => GetBaseTypeSyntaxes(code));

        private static IEnumerable<BaseTypeDeclarationSyntax> GetBaseTypeSyntaxes(this string code)
            => CSharpSyntaxTree.ParseText(code)
            .GetRoot()
            .DescendantNodes()
            .OfType<BaseTypeDeclarationSyntax>();


    }
}
