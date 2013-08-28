using Microsoft.Samples.EntityDataReader;
using Run00.SqlCopy;
using Run00.SqlCopySchema;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Run00.SqlCopyData
{
	public class DataCopy : IDataCopy
	{
		public DataCopy(ISchemaReader schemaReader, ISchemaConverter schemaConverter, IDbRepositoryFactory repositoryFactory, IDataReaderFactory dataReaderFactory, ITableBulkCopy tableBulkCopy, IConnectionFactory connectionFactory, IEnumerable<IEntityQueryFilter> entityFilters)
		{
			_schemaReader = schemaReader;
			_schemaConverter = schemaConverter;
			_repositoryFactory = repositoryFactory;
			_entityFilters = entityFilters;
			_dataReaderFactory = dataReaderFactory;
			_connectionFactory = connectionFactory;
			_tableBulkCopy = tableBulkCopy;
		}

		void IDataCopy.CopyData(DatabaseInfo source, DatabaseInfo target)
		{
			var sourceSchema = _schemaReader.GetSchema(source);
			var sourceEntityTypes = _schemaConverter.ToEntityTypes(sourceSchema);
			var sourceRepository = _repositoryFactory.Create(source, sourceEntityTypes);
			var cast = typeof(Queryable).GetMethods().Where(m => m.Name == "Cast").First();

			foreach (var type in sourceEntityTypes)
			{
				var typeCast = cast.MakeGenericMethod(new Type[] { type });
				var entities = sourceRepository.GetEntities(type);

				foreach (var filter in _entityFilters)
				{
					if (filter.EntityType.IsAssignableFrom(type) == false)
						continue;

					entities = filter.Filter(entities, sourceRepository);
					entities = typeCast.Invoke(null, new object[] { entities }) as IQueryable;
				}

				using (var sourceConnection = _connectionFactory.Create(source))
				{
					sourceConnection.Open();

					var reader = _dataReaderFactory.Create(entities);
					var sourceTable = sourceSchema.Tables.Where(t => t.Name == type.Name).Single();
					using (var targetConnection = _connectionFactory.Create(target))
					{
						targetConnection.Open();
						var destinationTable = target.Database + "." + sourceTable.Schema + "." + sourceTable.Name;
						_tableBulkCopy.Copy(targetConnection, destinationTable, reader);
					}
				}

			}
		}

		private readonly ISchemaConverter _schemaConverter;
		private readonly ISchemaReader _schemaReader;
		private readonly IDbRepositoryFactory _repositoryFactory;
		private readonly IEnumerable<IEntityQueryFilter> _entityFilters;
		private readonly IConnectionFactory _connectionFactory;
		private readonly IDataReaderFactory _dataReaderFactory;
		private readonly ITableBulkCopy _tableBulkCopy;
	}
}
