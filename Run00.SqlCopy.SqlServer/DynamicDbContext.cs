using Run00.SqlCopy;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Run00.DynamicEf
{
	public class DynamicDbContext : DbContext
	{
		public DynamicDbContext(DatabaseInfo info, IEnumerable<Type> entityTypes) : base(info.ConnectionString)
		{
			_entityTypes = entityTypes;
		}

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
			foreach (var entityType in _entityTypes)
				AddEntityType(modelBuilder, entityType);
		}

		private void AddEntityType(DbModelBuilder modelBuilder, Type entityType)
		{
			var method = modelBuilder.GetType().GetMethod("Entity");
			method = method.MakeGenericMethod(new Type[] { entityType });
			method.Invoke(modelBuilder, null);
		}

		private readonly IEnumerable<Type> _entityTypes;

	}
}
