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
        private readonly IEnumerable<string> _assemblySearchPaths;
        private readonly ICollection<IExcludeFilter> _excludeFilters;
        /// <summary>
        /// Creates a new instance of the TypeLoader class
        /// </summary>
        /// <param name="assemblyPaths">specifies a collection off assemblies form wehich to load types</param>
        /// <param name="excludeFilters">specifies a list of filters that are used to exclude types</param>
        public TypesLoader(ICollection<string> assemblyPaths, ICollection<IExcludeFilter> excludeFilters = null)
        {
            _assemblyPaths = assemblyPaths;
            _excludeFilters = excludeFilters ?? new List<IExcludeFilter>();
            _assemblySearchPaths =  assemblyPaths.Select(Path.GetDirectoryName);
        }
        /// <summary>
        /// Load types from specified collection of assemblies (and all type dependencies) excluding the types that
        /// match any of the exclude filters.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Type> Load()
        {
            AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += CurrentDomainOnReflectionOnlyAssemblyResolve;
            var assemblies = _assemblyPaths.Select(Assembly.ReflectionOnlyLoadFrom);
            assemblies =
                assemblies.SelectMany(x => x.GetReferencedAssemblies().Select(LoadAssembly).Concat(new[] {x}));
            var types = assemblies
                .SelectMany(a => a.GetTypes()
                .Where(x => x.IsClass || x.IsInterface)
                    .Where(new ExcludeFilters(_excludeFilters).Apply));
            return types.OrderBy(x=>x.Name).ToArray();
        }

        private Assembly LoadAssembly(AssemblyName assemblyName)
        {
            var path = FindAssemblyByName(assemblyName);
            if (path == null)
            {
                return Assembly.ReflectionOnlyLoad(assemblyName.FullName);
            }
            return Assembly.ReflectionOnlyLoadFrom(path);
        }

        private string FindAssemblyByName(AssemblyName assemblyName)
        {
            foreach (var assemblySearchPath in _assemblySearchPaths)
            {
                var path = Path.Combine(assemblySearchPath, assemblyName.Name + ".dll");
                if (File.Exists(path)) return path;
            }
            return null;
        }

        private Assembly CurrentDomainOnReflectionOnlyAssemblyResolve(object sender, ResolveEventArgs args)
        {
            var assemblyName = new AssemblyName(args.Name);
            return LoadAssembly(assemblyName);
        }

        public void Dispose()
        {
            AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve -= CurrentDomainOnReflectionOnlyAssemblyResolve;
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