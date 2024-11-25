using Kolibri.Common.PlugIn;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kolibri.Common.PlugIn
{
    public class ExtensibilityProvider
    {
        [Import(typeof(IFormPlugin))]
        private IFormPlugin _form; // Note: only field supported

        // ReSharper disable once ConvertToAutoProperty
        public IFormPlugin _formPlugin => _form;
        public CompositionContainer Container { get => _container;  }
        private CompositionContainer _container;

        public ExtensibilityProvider(string extensionsPath)
        { 

            //An aggregate catalog that combines multiple catalogs
            var catalog = new AggregateCatalog();

            //Adds all the parts found in the same assembly as the Program class
            catalog.Catalogs.Add(new AssemblyCatalog(GetType().Assembly));
            catalog.Catalogs.Add(new DirectoryCatalog(extensionsPath));

            //Create the CompositionContainer with the parts in the catalog
            CompositionContainer compositionContainer = new CompositionContainer(catalog);
              _container = compositionContainer;

            //Fill the imports of this object
            try
            {
                _container.ComposeParts(this);
            }
            catch (CompositionException compositionException)
            {
                Console.WriteLine(compositionException.ToString());
            }
        }
    }
}