﻿using System;
using System.Collections.Generic;

namespace Guilded.Extensions
{
    public static class IEnumerableExtensions
    {
        public static List<TOut> ToListOfDifferentType<TIn, TOut>(this IEnumerable<TIn> source, Func<TIn, TOut> converter)
        {
            List<TOut> result = new List<TOut>();
            foreach (var item in source)
            {
                result.Add(converter(item));
            }
            return result;
        }
    }
}
