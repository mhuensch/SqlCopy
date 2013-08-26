using Microsoft.SqlServer.Management.Smo;
using Run00.SqlCopy;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

		void IDataCopy.CopyData(DatabaseLocation source, DatabaseLocation target, IEnumerable<CopyParameter> copyParams)
		{
			var sourceSchema = _provider.GetSchema(source);
			var sourceTypes = _converter.ToEntityTypes(sourceSchema);
			var sourceContext = _contextFactory.CreateContext(sourceTypes);

			foreach (var type in sourceTypes)
			{
				var d = sourceContext.GetEntities(type);
				foreach (var filter in _entityFilters)
				{
					if (type.IsAssignableFrom(filter.EntityType) == false)
						continue;
	
					d = filter.Filter(d, sourceContext);
				}

				var query = d.ToString();
				var sourceConnection = default(SqlConnection);
				var command = new SqlCommand(query, sourceConnection);
				foreach (var param in copyParams)
					command.Parameters.Add(new SqlParameter(param.Name, param.Value));

				var reader = command.ExecuteReader();

				var sourceTable = sourceSchema.Tables.Where(t => t.Name == type.Name).Single();
				var targetConnection = default(SqlConnection);
				var copy = new SqlBulkCopy(targetConnection);
				copy.DestinationTableName = target.Database + "." + sourceTable.Schema + "." + sourceTable.Name;
				copy.WriteToServer(reader);
				reader.Close();
			}
		}

		private readonly ISchemaConverter _converter;
		private readonly ISchemaReader _provider;
		private readonly IContextFactory _contextFactory;
		private readonly IEnumerable<IEntityQueryFilter> _entityFilters;
	}
}
