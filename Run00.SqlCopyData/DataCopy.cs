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
		public DataCopy(ISchemaReader schemaReader, ISchemaConverter schemaConverter, IDbRepositoryFactory repositoryFactory, ITableBulkCopy tableBulkCopy, IEnumerable<IEntityFilter> entityFilters, ICommandBuilder commandBuilder)
		{
			_schemaReader = schemaReader;
			_schemaConverter = schemaConverter;
			_repositoryFactory = repositoryFactory;
			_entityFilters = entityFilters;
			_tableBulkCopy = tableBulkCopy;
			_commandBuilder = commandBuilder;
		}

		void IDataCopy.CopyData(DatabaseInfo source, DatabaseInfo target)
		{
			foreach (var query in GetEntityQueries(source))
			{
				var command = _commandBuilder.Build(source, query);
				_tableBulkCopy.Copy(target, query.ElementType.FullName, command.ExecuteReader());
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
		private readonly ICommandBuilder _commandBuilder;
	}
}
