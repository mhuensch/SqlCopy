using Microsoft.SqlServer.Management.Smo;
using Run00.SqlCopy;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Data.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Objects;
using Microsoft.Samples.EntityDataReader;

namespace Run00.SqlCopySqlServer
{
	public class DataCopy : IDataCopy
	{
		public DataCopy(ISchemaReader provider, ISchemaConverter converter, IContextFactory contextFactory, IEnumerable<IEntityQueryFilter> entityFilters)
		{
			_provider = provider;
			_converter = converter;
			_contextFactory = contextFactory;
			_entityFilters = entityFilters;
		}

		void IDataCopy.CopyData(DatabaseInfo source, DatabaseInfo target)
		{
			var sourceSchema = _provider.GetSchema(source);
			var sourceTypes = _converter.ToEntityTypes(sourceSchema);
			var sourceContext = _contextFactory.CreateContext(source, sourceTypes);
			var cast = typeof(Queryable).GetMethods().Where(m => m.Name == "Cast").First();

			foreach (var type in sourceTypes)
			{
				var typeCast = cast.MakeGenericMethod(new Type[] {type});
				var d = sourceContext.GetEntities(type);
				foreach (var filter in _entityFilters)
				{
					if (filter.EntityType.IsAssignableFrom(type) == false)
						continue;
	
					d = filter.Filter(d, sourceContext);
					d = typeCast.Invoke(null, new object[] { d }) as IQueryable;
				}

				//var objQueryType = typeof(ObjectQuery<>);
				//var genObjQueryType = objQueryType.MakeGenericType(new[] { type });
				//var prop = objQueryType.GetProperty("Parameters");

				using (var sourceConnection = new SqlConnection(source.ConnectionString))
				{
					sourceConnection.Open();

					//var command = new SqlCommand(query, sourceConnection);
					//foreach (var param in copyParams)
					//	command.Parameters.Add(new SqlParameter(param.Name, param.Value));

					//var reader = command.ExecuteReader();

					var reader = d.AsDataReader(type);
					var sourceTable = sourceSchema.Tables.Where(t => t.Name == type.Name).Single();
					using (var targetConnection = new SqlConnection(target.ConnectionString))
					{
						targetConnection.Open();
						var copy = new SqlBulkCopy(targetConnection);
						copy.DestinationTableName = target.Database + "." + sourceTable.Schema + "." + sourceTable.Name;
						copy.WriteToServer(reader);
						//reader.Close();
					}
				}

			}
		}

		private readonly ISchemaConverter _converter;
		private readonly ISchemaReader _provider;
		private readonly IContextFactory _contextFactory;
		private readonly IEnumerable<IEntityQueryFilter> _entityFilters;
	}
}
