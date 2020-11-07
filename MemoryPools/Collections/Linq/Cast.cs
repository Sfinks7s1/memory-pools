﻿namespace MemoryPools.Collections.Linq
{
    public static partial class PoolingEnumerable
    {
        public static IPoolingEnumerable<TR> Cast<TR>(this IPoolingEnumerable source)
        {
	        if (source is IPoolingEnumerable<TR> res) return res;
            return Pool.Get<CastExprEnumerable<TR>>().Init(source);
        }
    }
}