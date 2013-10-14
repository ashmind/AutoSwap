using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Roslyn.Compilers;
using Roslyn.Compilers.CSharp;

namespace AutoSwap {
    public class AutoSwapBuilder {
        public Type Rebuild(FileInfo sourceFile, Type lastType) {
            var dllName = string.Format(
                "{0}.{1}.dll",
                Path.GetFileNameWithoutExtension(sourceFile.Name),
                DateTimeOffset.Now.ToString("s").Replace(':', '_')
            );
            var references = lastType.Assembly.GetReferencedAssemblies()
                                              .Select(Assembly.Load)
                                              .Select(a => new MetadataFileReference(a.Location))
                                              .ToArray();

            var syntaxTree = SyntaxTree.ParseFile(sourceFile.FullName);
            var compilation = Compilation.Create(
                dllName,
                syntaxTrees: new[] { syntaxTree },
                references: references,
                options: new CompilationOptions(OutputKind.DynamicallyLinkedLibrary)
            );

            var stream = new MemoryStream();
            var emitResult = compilation.Emit(stream);
            if (!emitResult.Success)
                throw new Exception(string.Join(Environment.NewLine, emitResult.Diagnostics));

            return Assembly.Load(stream.ToArray()).GetExportedTypes().Single();
        }
    }
}