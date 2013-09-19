using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Run00.SqlCopy;
using Run00.SqlCopySchema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Run00.SqlCopySqlServer
{
	public class Installer : IWindsorInstaller
	{
		void IWindsorInstaller.Install(IWindsorContainer container, IConfigurationStore store)
		{
			container.Register(Component.For<ISchemaCopy>().ImplementedBy<SchemaCopy>());
			container.Register(Component.For<ISchemaReader>().ImplementedBy<SchemaReader>());
			container.Register(Component.For<ISchemaConverter>().ImplementedBy<SchemaConverter>());
			container.Register(Component.For<ITableBulkCopy>().ImplementedBy<TableBulkCopy>());
			container.Register(Component.For<IQueryProviderFactory>().ImplementedBy<QueryProviderFactory>());
		}
	}
}
