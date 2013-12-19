using Run00.SqlCopy;
using Run00.SqlCopySchema;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Data;
using System.Data.Objects;
using System.Reflection;

namespace Run00.SqlCopyData
{
	public class DataCopy : IDataCopy
	{
		public DataCopy(ISchemaReader schemaReader, ISchemaConverter schemaConverter, IDbRepositoryFactory repositoryFactory, ITableBulkCopy tableBulkCopy, IEnumerable<IEntityFilter> entityFilters, IEntityTableFactory entityTableFactory)
		{
			_schemaReader = schemaReader;
			_schemaConverter = schemaConverter;
			_repositoryFactory = repositoryFactory;
			_entityFilters = entityFilters;
			_tableBulkCopy = tableBulkCopy;
			_entityTableFactory = entityTableFactory;
		}

		void IDataCopy.CopyData(DatabaseInfo source, DatabaseInfo target)
		{
			foreach (var query in GetEntityQueries(source))
			{
				var entityDataTable = _entityTableFactory.Create(query);
				Console.WriteLine(String.Format("Copying data from {0}", entityDataTable.TableName));
				try
				{
					_tableBulkCopy.Copy(target, entityDataTable);
				}
				catch (Exception e)
				{
					Console.WriteLine(String.Format("CopyData encountered an error: {0} {1} {2}", e.Message, Environment.NewLine, e.StackTrace));
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
		private readonly IEnumerable<IEntityFilter> _entityFilters;
		private readonly ITableBulkCopy _tableBulkCopy;
		private readonly IEntityTableFactory _entityTableFactory;
	}
}
