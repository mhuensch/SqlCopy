using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Entity;
using Run00.SqlCopySqlServer.IntegrationTest.Artifacts;
using System.Linq;
using Castle.Windsor;
using Castle.Windsor.Installer;
using Castle.MicroKernel.Registration;
using System.IO;
using Run00.SqlCopy;
using System.Linq.Expressions;

namespace Run00.SqlCopySqlServer.IntegrationTest
{
	[TestClass]
	public class CopyDatabase
	{
		[TestInitialize]
		public void CreateSourceDatabase()
		{
			System.Data.Entity.Database.SetInitializer(new SourceContextInitializer());
			var context = new SourceContext();
			
			//Force database to initialize
			var samples = context.Samples.Where(s => s.Id == Guid.NewGuid());

			Locator.Initialize();
		}

		[TestMethod]
		public void TestMethod1()
		{
			var source = new DatabaseLocation(@"MAIN\SQLEXPRESS", "SourceContext");
			var target = new DatabaseLocation(@"MAIN\SQLEXPRESS", "SourceContextCopy");
			
			Locator.Test<ISchemaCopy>(a =>
				a.CopySchema(source, target)
			);

		}
	}

	public static class Locator
	{
		public static void Initialize()
		{
			var currentDir = Directory.GetCurrentDirectory();
			_container = new WindsorContainer();
			_container.Install(FromAssembly.InDirectory(new AssemblyFilter(currentDir)));
		}

		public static void Test<T>(Action<T> action)
		{
			var service = _container.Resolve<T>();
			action.Invoke(service);
			_container.Release(service);
		}

		private static WindsorContainer _container;
	}
}
