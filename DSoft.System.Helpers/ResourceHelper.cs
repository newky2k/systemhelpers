using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
namespace DSoft.System.Helpers
{
    /// <summary>
    /// Resource Helper.
    /// </summary>
    public class ResourceHelper
    {

        /// <summary>
        /// Retrieves the embedded resource names in the for the assembly that is calling the method
        /// </summary>
        /// <returns></returns>
        public static List<string> GetResourceNames()
        {
            var assm = Assembly.GetCallingAssembly();

            return GetResourceNames(assm);
        }



        /// <summary>
        ///  Retrieves the embedded resource names in the for the specified types containing assembly
        /// </summary>
        /// <returns></returns>
        public static List<string> GetResourceNames(Type assemblyType)
        {
            var assm = Assembly.GetAssembly(assemblyType);

            if (assm == null)
            {
                return [];
            }

            return GetResourceNames(assm);
        }

        /// <summary>
        /// Retrieves the embedded resource names in the for the specified assembly
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>System.Collections.Generic.List&lt;System.String&gt;.</returns>
        public static List<string> GetResourceNames(Assembly assembly)
        {
            var resources = assembly.GetManifestResourceNames();

            if (resources == null)
            {
                return [];
            }

            return resources.ToList();
        }

        /// <summary>
        /// Loads the resource.
        /// </summary>
        /// <param name="assemblyType">Type of the assembly.</param>
        /// <param name="name">The name of the resource.</param>
        /// <param name="resourcesFolder">The resources folder.</param>
        /// <returns></returns>
        public static MemoryStream LoadResource(Type assemblyType, string name, string resourcesFolder = "Resources")
        {
            MemoryStream aMem = new();

            var assm = Assembly.GetAssembly(assemblyType);

            if (assm == null)
            {
                return null;
            }

            var assemblyNamespace = assm.GetName().Name;

            var path = $"{assemblyNamespace}.{resourcesFolder}.{name}";

            var aStream = assm.GetManifestResourceStream(path);

            if (aStream == null)
            {
                return null;
            }

            aStream.CopyTo(aMem);

            return aMem;
        }

        /// <summary>
        /// Loads the resource.
        /// </summary>
        /// <param name="name">The name of the resource.</param>
        /// <param name="resourcesFolder">The resources folder.</param>
        /// <returns></returns>
        public static MemoryStream LoadResource(string name, string resourcesFolder = "Resources")
        {
            MemoryStream aMem = new();

            var assm = Assembly.GetCallingAssembly();

            var assemblyNamespace = assm.GetName().Name;

            var path = $"{assemblyNamespace}.{resourcesFolder}.{name}";

            var aStream = assm.GetManifestResourceStream(path);

            if (aStream == null)
            {
                return null;
            }

            aStream.CopyTo(aMem);

            return aMem;
        }

        /// <summary>
        /// Loads the resource.
        /// </summary>
        /// <param name="resourceName">Name of the resource.</param>
        /// <returns></returns>
        public static MemoryStream LoadResource(string resourceName)
        {
            MemoryStream aMem = new();

            var assm = Assembly.GetCallingAssembly();

            var aStream = assm.GetManifestResourceStream(resourceName);

            if (aStream == null)
            {
                return null;
            }

            aStream.CopyTo(aMem);

            return aMem;
        }
    }
}
