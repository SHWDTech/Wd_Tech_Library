using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace SHWDTech.ExtensionMethod
{
    public static class QueryableExtensionMehtod
    {
        /// <summary>
        /// Specifies the related objects to include in the query results.
        /// </summary>
        /// <param name="source">The source <see cref="IQueryable"/> on which to call Include.</param>
        /// <param name="condition">A boolean value to determine to include <see cref="path"/> or not.</param>
        /// <param name="path">The dot-separated list of related objects to return in the query results.</param>
        public static IQueryable IncludeIf(this IQueryable source, bool condition, string path)
        {
            return condition
                ? source.Include(path)
                : source;
        }

        /// <summary>
        /// Specifies the related objects to include in the query results.
        /// </summary>
        /// <param name="source">The source <see cref="IQueryable{T}"/> on which to call Include.</param>
        /// <param name="condition">A boolean value to determine to include <see cref="path"/> or not.</param>
        /// <param name="path">The dot-separated list of related objects to return in the query results.</param>
        public static IQueryable<T> IncludeIf<T>(this IQueryable<T> source, bool condition, string path)
        {
            return condition
                ? source.Include(path)
                : source;
        }

        /// <summary>
        /// Specifies the related objects to include in the query results.
        /// </summary>
        /// <param name="source">The source <see cref="IQueryable{T}"/> on which to call Include.</param>
        /// <param name="condition">A boolean value to determine to include <see cref="path"/> or not.</param>
        /// <param name="path">The type of navigation property being included.</param>
        public static IQueryable<T> IncludeIf<T, TProperty>(this IQueryable<T> source, bool condition, Expression<Func<T, TProperty>> path)
        {
            return condition
                ? source.Include(path)
                : source;
        }

        public static IQueryable<T> Include<T>(this IQueryable<T> source, IEnumerable<string> paths)
        {
            foreach (var path in paths)
            {
                source.Include(path);
            }

            return source;
        }

        public static IQueryable<T> Include<T, TProperty>(this IQueryable<T> source,
            IEnumerable<Expression<Func<T, TProperty>>>  paths)
        {
            foreach (var path in paths)
            {
                source.Include(path);
            }

            return source;
        }
    }
}
