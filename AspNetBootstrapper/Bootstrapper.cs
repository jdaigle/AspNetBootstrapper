using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Hosting;

namespace AspNetBootstrapper
{
    public static class Bootstrapper
    {
        private static object locker = new object();
        private static bool bootstrapCompleted = false;

        public static void Bootstrap()
        {
            try
            {
                if (bootstrapCompleted) return;

                lock (locker)
                {
                    if (bootstrapCompleted) return;

                    HostingEnvironment.IncrementBusyCount();

                    var assembliesDir = AppDomain.CurrentDomain.BaseDirectory;

                    // Work around to flatten assembly names
                    var assemblyNames = new HashSet<string>();
                    var allAssemblies = GetAssembliesInDirectory(assembliesDir);
                    var assemblies = new List<Assembly>();

                    foreach (var item in allAssemblies)
                    {
                        if (assemblyNames.Contains(item.FullName))
                            continue;
                        assemblyNames.Add(item.FullName);
                        assemblies.Add(item);
                    }
                    var configClasses = ScanAssembliesForEndpoints(assemblies);

                    foreach (var item in configClasses)
                    {
                        var bootstrapper = (IApplicationServerBootstrapper)Activator.CreateInstance(item);
                        bootstrapper.Init();
                    }

                    HostingEnvironment.DecrementBusyCount();

                    bootstrapCompleted = true;
                }
            }
            catch (Exception e)
            {
                log4net.LogManager.GetLogger(typeof(Bootstrapper)).Error("Bootstrapper Failed", e);
                throw;
            }
        }

        private static IEnumerable<Assembly> GetAssembliesInDirectory(string path, params string[] assembliesToSkip)
        {
            foreach (var a in GetAssembliesInDirectoryWithExtension(path, "*.exe", assembliesToSkip))
                yield return a;
            foreach (var a in GetAssembliesInDirectoryWithExtension(path, "*.dll", assembliesToSkip))
                yield return a;
        }

        private static IEnumerable<Assembly> GetAssembliesInDirectoryWithExtension(string path, string extension, params string[] assembliesToSkip)
        {
            var result = new List<Assembly>();
            foreach (FileInfo file in new DirectoryInfo(path).GetFiles(extension, SearchOption.AllDirectories))
            {
                try
                {
                    if (assembliesToSkip.Contains(file.Name, StringComparer.InvariantCultureIgnoreCase))
                        continue;

                    result.Add(Assembly.LoadFrom(file.FullName));
                }
                catch (BadImageFormatException bif)
                {
                    throw new InvalidOperationException(
                        "Could not load " + file.FullName + ".",
                        bif);
                }
            }

            return result;
        }

        private static IEnumerable<Type> ScanAssembliesForEndpoints(IEnumerable<Assembly> assemblies)
        {
            foreach (var assembly in assemblies)
                foreach (var type in assembly.GetTypes().Where(t => typeof(IApplicationServerBootstrapper).IsAssignableFrom(t) && t != typeof(IApplicationServerBootstrapper)))
                {
                    yield return type;
                }
        }
    }
}
