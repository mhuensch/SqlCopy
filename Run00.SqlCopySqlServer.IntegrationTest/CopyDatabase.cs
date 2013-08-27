using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Installer;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Win32;
using Run00.SqlCopy;
using Run00.SqlCopySqlServer.IntegrationTest.Artifacts;
using System;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace Run00.SqlCopySqlServer.IntegrationTest
{
	[TestClass]
	public class CopyDatabase
	{
		[TestInitialize]
		public void CreateSourceDatabase()
		{
			System.Data.Entity.Database.SetInitializer(new SourceContextInitializer());
			
			//Force database to initialize
			//var server = new Server();
			//var connection = new SqlConnectionStringBuilder();
			//connection.DataSource = server.Name;
			//server.Version.ToString();
			//if (string.IsNullOrEmpty(server.InstanceName))
			//	connection.DataSource = connection.DataSource + "\\" + server.InstanceName;
			//connection.IntegratedSecurity = true;
			//connection.InitialCatalog = "SourceContext";
			var context = new SourceContext(@"Data Source=(localdb)\v11.0;Initial Catalog=SourceContext;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False");
			var samples = context.Samples.Where(s => s.Id == Guid.NewGuid());

			Locator.Initialize();
		}

		[TestMethod]
		public void TestMethod1()
		{
			var source = new DatabaseLocation("(localdb)\v11.0", "SourceContext");
			var target = new DatabaseLocation("(localdb)\v11.0", "SourceContextToo");

			Locator.Test<ISchemaCopy>(a =>
				a.CopySchema(source, target)
			);

			Locator.Test<IDataCopy>(dc =>
				dc.CopyData(source, target, new[] { new CopyParameter() { Name = "TenantId", Value = "63BDDD01-D781-4064-83DE-18A3DDAAF178" } })
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
