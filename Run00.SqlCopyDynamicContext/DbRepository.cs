using IQToolkit;
using Run00.SqlCopy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Run00.SqlCopyDynamicContext
{
	public class DbRepository : IDbRepository
	{
		public DbRepository(IQueryProvider queryProvider, IEnumerable<Type> entityTypes)
		{
			_entities = new Dictionary<Type, IQueryable>();

			foreach (var entityType in entityTypes)
			{
				var constructor = typeof(Query<>).MakeGenericType(new Type[] { entityType }).GetConstructors().Where(c => c.GetParameters().Count() == 1).First();
				var queryable = constructor.Invoke(new object[] { queryProvider }) as IQueryable;
				_entities.Add(entityType, queryable);
			}
		}

		IQueryable<T> IDbRepository.GetEntities<T>()
		{
			return _entities[typeof(T)] as IQueryable<T>;
		}

		IQueryable IDbRepository.GetEntities(Type type)
		{
			return _entities[type];
		}

		IQueryable IDbRepository.GetEntities(string entityName)
		{
			var type = _entities.Keys
				.Where(t => t.FullName.Equals(entityName, StringComparison.InvariantCultureIgnoreCase))
				.FirstOrDefault();

			if (type == null)
				return null;

			return _entities[type];
		}

		private readonly Dictionary<Type, IQueryable> _entities;
	}
}
