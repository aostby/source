using System.Reflection;
using System.Text;

namespace Kolibri.net.Common.Utilities.Controller
{
    public class ResourceController
    {
     /// <summary>
     /// Henter ett objekt
     /// </summary>
     /// <param name="resourceName">Bilder: BringToFrontHS CopyHS CutHS delete DocumentHS health_tests_512 OpenFile openHS PasteHS PrintHS PrintPreviewHS PrintSetupHS saveHS SendToBackHS</param>
     /// <returns></returns>
   
        public static Stream GetResourceStream(Assembly assembly, string resourceName)
        {
            string[] resources = assembly.GetManifestResourceNames();
            EmbeddedResources res = new EmbeddedResources(assembly);
            string fullResourceName = res.GetResourceFullName(resourceName);
            return res.GetStream(fullResourceName);
        }
        public static byte[] GetResourceBytes(Assembly assembly, string resourceName)
        {
            return ByteUtilities.ReadFully(GetResourceStream(assembly, resourceName));
        }
        public static string GetResourceString(Assembly assembly, string resourceName)
        {
            return GetResourceString(assembly, resourceName, Encoding.UTF8);
        }
        internal static string GetResourceString(Assembly assembly, string resourceName, Encoding enc)
        {
            return enc.GetString(ByteUtilities.ReadFully(GetResourceStream(assembly, resourceName)));
        }

        /// <summary>
        /// Gets manifiest resource names from an assembly
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static string[] GetResourceNames(Assembly assembly)
        {
            string[] resources = assembly.GetManifestResourceNames();
            return resources;
        }
        public static string[] GetResourceNames()
        {
            return GetResourceNames(Assembly.GetExecutingAssembly());
        }
    }

    /// <summary>
    /// Class for getting embedded resources from an assembly
    /// </summary>
    internal class EmbeddedResources
    {

        public static EmbeddedResources callingResources;

        public static EmbeddedResources entryResources;

        public static EmbeddedResources executingResources;

        private Assembly assembly;

        private string[] resources;

        /// <summary>
        /// Eksample usage:
        /// EmbeddedResources res = new EmbeddedResources(assembly);
        /// return res.GetStream(resourceName);           
        /// </summary>
        /// <param name="assembly"></param>
        public EmbeddedResources(Assembly assembly)
        {
            this.assembly = assembly;
            resources = assembly.GetManifestResourceNames();
        }
        public Stream GetStream(string resName)
        {
            string[] possibleCandidates = resources.Where(s => s.Contains(resName)).ToArray();
            if (possibleCandidates.Length == 0)
            {
                return null;
            }
            else if (possibleCandidates.Length == 1)
            {
                return assembly.GetManifestResourceStream(possibleCandidates[0]);
            }
            else
            {
                throw new ArgumentException("Ambiguous name, cannot identify resource", "resName");
            }
        }

        internal string GetResourceFullName(string resName)
        {
            string[] possibleCandidates = resources.Where(s => s.Contains(resName)).ToArray();
            if (possibleCandidates.Length == 0)
            {
                return null;
            }
            else if (possibleCandidates.Length == 1)
            {
                return possibleCandidates[0];
            }
            else
            {
                throw new ArgumentException("Ambiguous name, cannot identify resource", "resName");
            }
        }

        public static EmbeddedResources CallingResources
        {
            get
            {
                if (callingResources == null)
                {
                    callingResources = new EmbeddedResources(Assembly.GetCallingAssembly());
                }

                return callingResources;
            }
        }

        public static EmbeddedResources EntryResources
        {
            get
            {
                if (entryResources == null)
                {
                    entryResources = new EmbeddedResources(Assembly.GetEntryAssembly());
                }

                return entryResources;
            }
        }

        public static EmbeddedResources ExecutingResources
        {
            get
            {
                if (executingResources == null)
                {
                    executingResources = new EmbeddedResources(Assembly.GetExecutingAssembly());
                }

                return executingResources;
            }
        }

       
     
    }

}
