using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace OpenSoftware.DgmlTools.Reflection
{
    public class TypesLoader : IDisposable
    {
        private readonly IEnumerable<string> _assemblyPaths;
        private readonly ICollection<IExcludeFilter> _excludeFilters;
        private MetadataLoadContext _metadataLoadContext;

        /// <summary>
        /// Creates a new instance of the TypeLoader class
        /// </summary>
        /// <param name="assemblyPaths">specifies a collection off assemblies form wehich to load types</param>
        /// <param name="excludeFilters">specifies a list of filters that are used to exclude types</param>
        public TypesLoader(ICollection<string> assemblyPaths, ICollection<IExcludeFilter> excludeFilters = null)
        {
            _assemblyPaths = assemblyPaths;
            _excludeFilters = excludeFilters ?? new List<IExcludeFilter>();
        }

        /// <summary>
        /// Load types from specified collection of assemblies (and all type dependencies) excluding the types that
        /// match any of the exclude filters.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Type> Load()
        {
            var assemblyLocation = typeof(object).Assembly.Location;
            var assemblyPath = Path.GetDirectoryName(assemblyLocation);
            var netStandard = $"{assemblyPath}{Path.DirectorySeparatorChar}netstandard.dll";
            var paths = new[] {netStandard};
            var r = new SimplePathAssemblyResolver(paths);

            _metadataLoadContext = new MetadataLoadContext(r, "netstandard");

            var assemblies = _assemblyPaths.Select(_metadataLoadContext.LoadFromAssemblyPath);
            var types = assemblies
                .SelectMany(x => x.GetTypes())
                .Where(x => x.IsClass || x.IsInterface || x.IsEnum)
                .Where(new ExcludeFilters(_excludeFilters).Apply)
                .OrderBy(x => x.Name)
                .ToArray();
            return types;

        }

        public void Dispose()
        {
            _metadataLoadContext?.Dispose();
        }

        private class SimplePathAssemblyResolver : PathAssemblyResolver
        {
            public SimplePathAssemblyResolver(IEnumerable<string> assemblyPaths) : base(assemblyPaths)
            {
            }

            public override Assembly Resolve(MetadataLoadContext context, AssemblyName assemblyName)
            {
                var result = base.Resolve(context, assemblyName);
                if (result == null)
                {
                    var assemblyLocation = Path.GetDirectoryName(context.CoreAssembly.Location);

                    var newPath = $"{assemblyLocation}{Path.DirectorySeparatorChar}{assemblyName.Name}.dll";

                    if (File.Exists(newPath))
                    {
                        return context.LoadFromAssemblyPath(newPath);
                    }
                }

                return result;
            }
        }

        private class ExcludeFilters
        {
            private readonly ICollection<IExcludeFilter> _excludeFilters;

            internal ExcludeFilters(ICollection<IExcludeFilter> excludeFilters)
            {
                _excludeFilters = excludeFilters;
            }

            internal bool Apply(Type type)
            {
                return _excludeFilters.Any(filter => filter.Exclude(type)) == false;
            }
        }
    }
}