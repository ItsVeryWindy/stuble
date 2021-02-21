using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Razor;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;

namespace Stuble.Razor
{
    public class RazorTemplateFactory : IRazorTemplateFactory
    {
        private readonly CSharpCompilationOptions _compilationOptions;
        private readonly EmitOptions _emitOptions;
        private readonly List<MetadataReference> _references;
        private readonly RazorProjectEngine _engine;

        public RazorTemplateFactory(ApplicationPartManager partManager)
        {
            _compilationOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                                    .WithSpecificDiagnosticOptions(
                                        new Dictionary<string, ReportDiagnostic>
                                        {
                                            ["CS1701"] = ReportDiagnostic.Suppress,
                                            ["CS1702"] = ReportDiagnostic.Suppress,
                                            ["CS1705"] = ReportDiagnostic.Suppress
                                        });

            _emitOptions = new EmitOptions(debugInformationFormat: DebugInformationFormat.PortablePdb);

            _references = new List<MetadataReference>();

            foreach (var part in partManager.ApplicationParts)
            {
                if (part is ICompilationReferencesProvider compilationReferencesProvider)
                {
                    _references.AddRange(compilationReferencesProvider.GetReferencePaths().Select(x => MetadataReference.CreateFromFile(x)));
                }

                if (part is AssemblyPart assemblyPart)
                {
                    _references.AddRange(assemblyPart.GetReferencePaths().Select(x => MetadataReference.CreateFromFile(x)));
                }
            }

            _engine = RazorProjectEngine.Create(RazorConfiguration.Default, EmptyRazorProjectFileSystem.Instance, builder =>
            {
                builder
                    .SetNamespace("Stuble.Templates")
                    .SetBaseType(typeof(StubTemplateBase).FullName);

                builder.SetCSharpLanguageVersion(LanguageVersion.CSharp8);

                builder.AddDefaultImports(@"
@using System
@using System.Threading.Tasks
");
            });
        }

        public IRazorTemplate Create(string content)
        {
            var codeDocument = _engine.Process(new TemplateRazorProjectItem(content));

            var csDocument = codeDocument.GetCSharpDocument();

            var sourceText = SourceText.From(csDocument.GeneratedCode, Encoding.UTF8);

            var syntaxTree = CSharpSyntaxTree.ParseText(sourceText);

            var compilation = CSharpCompilation
                .Create(Guid.NewGuid().ToString(), options: _compilationOptions, references: _references)
                .AddSyntaxTrees(syntaxTree);

            using MemoryStream assemblyStream = new MemoryStream();
            using MemoryStream pdbStream = new MemoryStream();

            var result = compilation.Emit(assemblyStream, pdbStream, options: _emitOptions);

            if (!result.Success)
            {
                var errors = result
                    .Diagnostics
                    .Where(d => d.IsWarningAsError || d.Severity == DiagnosticSeverity.Error)
                    .ToList();

                throw new RazorTemplateCompilationException(errors.Select(MapError));
            }

            assemblyStream.Seek(0, SeekOrigin.Begin);
            pdbStream.Seek(0, SeekOrigin.Begin);

            var context = new TemplateLoadContext();

            var assembly = context.LoadFromStream(assemblyStream, pdbStream);

            return new RazorTemplate(context, assembly.GetType("Stuble.Templates.Template"));
        }

        private static RazorTemplateError MapError(Diagnostic diagnostic)
        {
            var lineSpan = diagnostic.Location.SourceTree.GetMappedLineSpan(diagnostic.Location.SourceSpan);

            var message = diagnostic.GetMessage();

            return new RazorTemplateError(lineSpan.StartLinePosition.Line, lineSpan.StartLinePosition.Character, message);
        }

        private class RazorTemplate : IRazorTemplate
        {
            private TemplateLoadContext _context;
            private Type _type;

            public RazorTemplate(TemplateLoadContext context, Type type)
            {
                _context = context;
                _type = type;
            }

            public void Dispose()
            {
                _type = null;

                _context?.Unload();

                _context = null;
            }

            public string Render() => Activator.CreateInstance(_type).ToString();
        }

        private class EmptyRazorProjectFileSystem : RazorProjectFileSystem
        {
            public static EmptyRazorProjectFileSystem Instance { get; } = new EmptyRazorProjectFileSystem();

            private EmptyRazorProjectFileSystem()
            {

            }

            public override IEnumerable<RazorProjectItem> EnumerateItems(string basePath)
            {
                throw new NotImplementedException();
            }

            [Obsolete]
            public override RazorProjectItem GetItem(string path)
            {
                throw new NotImplementedException();
            }

            public override RazorProjectItem GetItem(string path, string fileKind)
            {
                throw new NotImplementedException();
            }
        }

        private class TemplateRazorProjectItem : RazorProjectItem
        {
            private readonly string _content;

            public override string BasePath => "/";

            public override string FilePath => "Template";

            public override string PhysicalPath => null;

            public override bool Exists => true;

            public TemplateRazorProjectItem(string content)
            {
                _content = content;
            }

            public override Stream Read()
            {
                var ms = new MemoryStream(Encoding.UTF8.GetBytes(_content));

                return ms;
            }
        }

        private class TemplateLoadContext : AssemblyLoadContext
        {
            public TemplateLoadContext() : base(true)
            {

            }

            protected override Assembly Load(AssemblyName assemblyName)
            {
                return Default.LoadFromAssemblyName(assemblyName);
            }

            protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
            {
                return IntPtr.Zero;
            }
        }
    }
}
