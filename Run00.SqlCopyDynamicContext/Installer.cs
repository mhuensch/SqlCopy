using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Run00.SqlCopy;

namespace Run00.SqlCopyDynamicContext
{
	public class Installer : IWindsorInstaller
	{
		void IWindsorInstaller.Install(IWindsorContainer container, IConfigurationStore store)
		{
			container.Register(Component.For<IDbRepositoryFactory>().ImplementedBy<DbRepositoryFactory>());
			container.Register(Component.For<IQueryBuilder>().ImplementedBy<QueryBuilder>());
		}
	}
}
