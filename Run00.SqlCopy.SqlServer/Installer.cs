﻿using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Run00.SqlCopy;
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
			container.Register(Component.For<IDataCopy>().ImplementedBy<DataCopy>());
			container.Register(Component.For<ISchemaReader>().ImplementedBy<SchemaReader>());
			container.Register(Component.For<ISchemaConverter>().ImplementedBy<SchemaConverter>());
			container.Register(Component.For<IContextFactory>().ImplementedBy<ContextFactory>());

			container.Kernel.Resolver.AddSubResolver(new CollectionResolver(container.Kernel, true));
		}
	}
}