﻿using Run00.SqlCopy;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace Run00.SqlCopySqlServer
{
	public interface IContextFactory
	{
		IDbRepository CreateContext(DatabaseInfo info, IEnumerable<Type> entityTypes);
	}
}
