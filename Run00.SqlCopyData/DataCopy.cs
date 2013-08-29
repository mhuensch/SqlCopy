using Run00.SqlCopy;
using Run00.SqlCopySchema;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Data;

namespace Run00.SqlCopyData
{
	public class DataCopy : IDataCopy
	{
		public DataCopy(ISchemaReader schemaReader, ISchemaConverter schemaConverter, IDbRepositoryFactory repositoryFactory, IEntityTableFactory entityTableFactory, ITableBulkCopy tableBulkCopy, IConnectionFactory connectionFactory, IEnumerable<IEntityQueryFilter> entityFilters)
		{
			_schemaReader = schemaReader;
			_schemaConverter = schemaConverter;
			_repositoryFactory = repositoryFactory;
			_entityFilters = entityFilters;
			_entityTableFactory = entityTableFactory;
			_connectionFactory = connectionFactory;
			_tableBulkCopy = tableBulkCopy;
		}

		void IDataCopy.CopyData(DatabaseInfo source, DatabaseInfo target)
		{
		
			using (var sourceConnection = _connectionFactory.Create(source))
			{
				sourceConnection.Open();

				using (var targetConnection = _connectionFactory.Create(target))
				{
					targetConnection.Open();

					foreach (var query in GetEntityQueries(source))
					{
						var entityDataTable = _entityTableFactory.Create(query);
						_tableBulkCopy.Copy(targetConnection, entityDataTable);
					}
				}
			}
		}

		private IEnumerable<IQueryable> GetEntityQueries(DatabaseInfo info)
		{
			var result = new List<IQueryable>();
			var sourceSchema = _schemaReader.GetSchema(info);
			var sourceEntityTypes = _schemaConverter.ToEntityTypes(sourceSchema);
			var sourceRepository = _repositoryFactory.Create(info, sourceEntityTypes);

			foreach (var type in sourceEntityTypes)
			{
				var entities = sourceRepository.GetEntities(type);
				
				foreach (var filter in _entityFilters)
					entities = filter.Filter(entities, sourceRepository);

				result.Add(entities);
			}

			return result;
		}

		private readonly ISchemaConverter _schemaConverter;
		private readonly ISchemaReader _schemaReader;
		private readonly IDbRepositoryFactory _repositoryFactory;
		private readonly IEnumerable<IEntityQueryFilter> _entityFilters;
		private readonly IConnectionFactory _connectionFactory;
		private readonly IEntityTableFactory _entityTableFactory;
		private readonly ITableBulkCopy _tableBulkCopy;
	}
}
