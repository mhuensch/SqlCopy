using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Installer;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Run00.SqlCopy;
using Run00.SqlCopySqlServer.IntegrationTest.Artifacts;
using System;
using System.Data;
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
			var context = new SourceContext(TestDatabase.GetDatabaseConnection("SourceContext"));
			var samples = context.Samples.Where(s => s.Id == Guid.NewGuid());

			Locator.Initialize();
		}

		[TestMethod]
		public void TestMethod1()
		{
			var source = new DatabaseLocation(TestDatabase.GetServerName(), "SourceContext");
			var target = new DatabaseLocation(TestDatabase.GetServerName(), "SourceContextCopy");

			Locator.Test<ISchemaCopy>(a =>
				a.CopySchema(source, target)
			);

			Locator.Test<IDataCopy>(dc =>
				dc.CopyData(source, target, new[] { new CopyParameter() { Name = "TenantId", Value = "63BDDD01-D781-4064-83DE-18A3DDAAF178" } })
			);

		}
	}


	public static class TestDatabase
	{
		public static string GetServerName()
		{
			var name = string.Empty;

			if (File.Exists(_testDatabaseFile))
				name = File.ReadAllText(_testDatabaseFile);

			if (string.IsNullOrWhiteSpace(name) == false)
				return name;

			var availableServers = SmoApplication.EnumAvailableSqlServers(true);
			foreach (var serverRow in availableServers.Rows)
			{
				var items = ((DataRow)serverRow).ItemArray;
				if (items.Count() == 0)
					continue;

				name = items[0].ToString();
				break;
			}

			File.WriteAllText(_testDatabaseFile, name);

			return name;
		}
		public static string GetDatabaseConnection(string databaseName)
		{
			return string.Format(_connectionStringPattern, GetServerName(), databaseName);
		}

		private const string _testDatabaseFile = "TestDatabase";
		private const string _connectionStringPattern = "Data Source={0};Initial Catalog={1};Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False";
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
