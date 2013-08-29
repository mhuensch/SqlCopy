using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Run00.SqlCopy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Run00.SqlCopy.IntegrationTest
{
	public class ArtifactInstaller : IWindsorInstaller
	{
		void IWindsorInstaller.Install(IWindsorContainer container, IConfigurationStore store)
		{
			container.Register(Component.For<IEntityInterfaceLocator>().ImplementedBy<EntityInterfaceLocator>());
			container.Register(Component.For<IEntityQueryFilter>().ImplementedBy<TestFilter>());
		}
	}
}
