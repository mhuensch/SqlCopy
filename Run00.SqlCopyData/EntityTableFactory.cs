using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Run00.SqlCopyData
{
	//Simplified version of the code found here: http://msdn.microsoft.com/en-us/library/bb669096.aspx
	public class EntityTableFactory : IEntityTableFactory
	{
		/// <summary>
		/// Loads a DataTable from a sequence of objects.
		/// </summary>
		/// <param name="source">The sequence of objects to load into the DataTable.</param>
		/// <param name="table">The input table. The schema of the table must match that 
		/// the type T.  If the table is null, a new table is created with a schema 
		/// created from the public properties and fields of the type T.</param>
		/// <param name="options">Specifies how values from the source sequence will be applied to 
		/// existing rows in the table.</param>
		/// <returns>A DataTable created from the source sequence.</returns>
		public DataTable Create(IQueryable source)
		{
			var entityType = source.ElementType;
			var entityTypeProperties = entityType.GetProperties();

			var table = new DataTable(entityType.Namespace + "." + entityType.Name);
			var cols = entityTypeProperties.Select(p => new DataColumn(p.Name, p.PropertyType));
			table.Columns.AddRange(cols.ToArray());

			table.BeginLoadData();
			var entityEnumerator = source.GetEnumerator();

			while (entityEnumerator.MoveNext())
			{
				var values = entityTypeProperties.Select(p => p.GetValue(entityEnumerator.Current, null));
				table.LoadDataRow(values.ToArray(), true);
			}

			table.EndLoadData();

			return table;
		}
	}
}
