using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Roslyn.Compilers;
using Roslyn.Compilers.CSharp;

namespace AutoSwap {
    public class AutoSwapRecompiler {
        public Type Recompile(FileInfo sourceFile, Type originalType) {
            var dllName = string.Format(
                "{0}.{1}.dll",
                Path.GetFileNameWithoutExtension(sourceFile.Name),
                DateTimeOffset.Now.ToString("s").Replace(':', '_')
            );
            var references = originalType.Assembly.GetReferencedAssemblies()
                                                 .Select(Assembly.Load)
                                                 .Select(a => new MetadataFileReference(a.Location))
                                                 .ToList();
            references.Add(new MetadataFileReference(originalType.Assembly.Location));

            var syntaxTree = SyntaxTree.ParseFile(sourceFile.FullName);
            var syntaxRoot = syntaxTree.GetRoot();
            var typesToRemove = syntaxRoot.DescendantNodes().OfType<TypeDeclarationSyntax>()
                                                            .Where(t => t.Identifier.ValueText != originalType.Name);
            syntaxTree = SyntaxTree.Create(syntaxTree.GetRoot().RemoveNodes(typesToRemove, SyntaxRemoveOptions.KeepNoTrivia));

            var compilation = Compilation.Create(
                dllName,
                syntaxTrees: new[] { syntaxTree },
                references: references,
                options: new CompilationOptions(OutputKind.DynamicallyLinkedLibrary)
            );

            var dllStream = new MemoryStream();
            var pdbStream = new MemoryStream();
            var emitResult = compilation.Emit(dllStream, pdbStream: pdbStream);
            if (!emitResult.Success)
                throw new Exception(string.Join(Environment.NewLine, emitResult.Diagnostics));

            return Assembly.Load(dllStream.ToArray(), pdbStream.ToArray())
                           .GetType(originalType.FullName);
        }
    }
}