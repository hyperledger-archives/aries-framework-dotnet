using System;
using System.Collections.Generic;
using System.Linq;
using AgentFramework.Core.Extensions;

namespace AgentFramework.Core.Models.Records.Search
{
    /// <summary>
    /// Wallet query builder
    /// </summary>
    public static class SearchQuery
    {
        /// <summary>
        /// Empty query
        /// </summary>
        public static ISearchQuery Empty = new SearchExpression<string>();

        /// <summary>
        /// Combine the specified queries with AND operator
        /// </summary>
        /// <returns>The and.</returns>
        /// <param name="queries">Queries.</param>
        public static ISearchQuery And(params ISearchQuery[] queries) => new AndSubquery(queries);

        /// <summary>
        /// Combine the specified queries with OR operator
        /// </summary>
        /// <returns>The or.</returns>
        /// <param name="queries">Queries.</param>
        public static ISearchQuery Or(params ISearchQuery[] queries) => new OrSubquery(queries);

        /// <summary>
        /// Apply NOT operator to the query
        /// </summary>
        /// <returns>The not.</returns>
        /// <param name="query">Query.</param>
        public static ISearchQuery Not(ISearchQuery query) => new NotSubquery(query);

        /// <summary>
        /// EQUAL expression
        /// </summary>
        /// <returns>The equal.</returns>
        /// <param name="name">Name.</param>
        /// <param name="value">Value.</param>
        public static ISearchQuery Equal(string name, string value) => new EqSubquery(name, value);

        /// <summary>
        /// EQUAL expression for DateTime
        /// </summary>
        /// <returns>The equal.</returns>
        /// <param name="name">Name.</param>
        /// <param name="value">DateTime Value.</param>
        public static ISearchQuery Equal(string name, DateTime value) => new EqSubquery(name, value.Ticks.ToString());

        /// <summary>
        /// NOT EQUAL expression
        /// </summary>
        /// <returns>The equal.</returns>
        /// <param name="name">Name.</param>
        /// <param name="value">Value.</param>
        public static ISearchQuery NotEqual(string name, string value) => new NotEqSubquery(name, value);

        /// <summary>
        /// NOT EQUAL expression for DateTime
        /// </summary>
        /// <returns>The equal.</returns>
        /// <param name="name">Name.</param>
        /// <param name="value">DateTime Value.</param>
        public static ISearchQuery NotEqual(string name, DateTime value) => new NotEqSubquery(name, value.Ticks.ToString());

        /// <summary>
        /// IN expression, search values found inside the collection
        /// </summary>
        /// <returns>The in.</returns>
        /// <param name="name">Name.</param>
        /// <param name="values">Values.</param>
        public static ISearchQuery In(string name, string[] values) => new InSubquery(name, values);

        /// <summary>
        /// LESS expression
        /// </summary>
        /// <returns>The less.</returns>
        /// <param name="name">Name.</param>
        /// <param name="value">Value.</param>
        public static ISearchQuery Less(string name, string value) => new LtSubquery(name, value);

        /// <summary>
        /// LESS expression for DateTime
        /// </summary>
        /// <returns>The less.</returns>
        /// <param name="name">Name.</param>
        /// <param name="value">Value.</param>
        public static ISearchQuery Less(string name, DateTime value) => new LtSubquery(name, value.Ticks.ToString());

        /// <summary>
        /// LESS THAN OR EQUAL expression
        /// </summary>
        /// <returns>The or equal.</returns>
        /// <param name="name">Name.</param>
        /// <param name="value">Value.</param>
        public static ISearchQuery LessOrEqual(string name, string value) => new LteSubquery(name, value);

        /// <summary>
        /// LESS THAN OR EQUAL expression for DateTime
        /// </summary>
        /// <returns>The or equal.</returns>
        /// <param name="name">Name.</param>
        /// <param name="value">Value.</param>
        public static ISearchQuery LessOrEqual(string name, DateTime value) => new LteSubquery(name, value.Ticks.ToString());

        /// <summary>
        /// GREATER THAN expression
        /// </summary>
        /// <returns>The greater.</returns>
        /// <param name="name">Name.</param>
        /// <param name="value">Value.</param>
        public static ISearchQuery Greater(string name, string value) => new GtSubquery(name, value);

        /// <summary>
        /// GREATER THAN expression for DateTime
        /// </summary>
        /// <returns>The greater.</returns>
        /// <param name="name">Name.</param>
        /// <param name="value">Value.</param>
        public static ISearchQuery Greater(string name, DateTime value) => new GtSubquery(name, value.Ticks.ToString());

        /// <summary>
        /// GREATER THAN OR EQUAL expression
        /// </summary>
        /// <returns>The or equal.</returns>
        /// <param name="name">Name.</param>
        /// <param name="value">Value.</param>
        public static ISearchQuery GreaterOrEqual(string name, string value) => new GteSubquery(name, value);

        /// <summary>
        /// GREATER THAN OR EQUAL expression
        /// </summary>
        /// <returns>The or equal.</returns>
        /// <param name="name">Name.</param>
        /// <param name="value">Value.</param>
        public static ISearchQuery GreaterOrEqual(string name, DateTime value) => new GteSubquery(name, value.Ticks.ToString());

        /// <summary>
        /// LIKE expression, checks if substring is contained anywhere in the value
        /// </summary>
        /// <returns>The contains.</returns>
        /// <param name="name">Name.</param>
        /// <param name="value">Value.</param>
        public static ISearchQuery Contains(string name, string value) => new LikeSubquery(name, $"%{value}%");

        /// <summary>
        /// LIKE expression, checks if substring is contained at the begining of the value
        /// </summary>
        /// <returns>The with.</returns>
        /// <param name="name">Name.</param>
        /// <param name="value">Value.</param>
        public static ISearchQuery StartsWith(string name, string value) => new LikeSubquery(name, $"{value}%");
        /// <summary>
        /// LIKE expression, checks if substring is contained at the end of the value
        /// </summary>
        /// <returns>The with.</returns>
        /// <param name="name">Name.</param>
        /// <param name="value">Value.</param>
        public static ISearchQuery EndsWith(string name, string value) => new LikeSubquery(name, $"%{value}");
    }

    /// <summary>
    /// Search query that represents Wallet Query Language (WQL) model
    /// </summary>
    public interface ISearchQuery
    {
    }

    internal class SearchExpression<T> : Dictionary<string, T>, ISearchQuery
    {
        internal SearchExpression() : base(capacity: 1)
        {
        }

        /// <summary>
        /// Returns a <see cref="string"/> that represents the current <see cref="Search.SearchExpression{T}"/>.
        /// </summary>
        /// <returns>A JSON <see cref="string"/> that represents the current <see cref="Search.SearchExpression{T}"/> as WQL.</returns>
        public override string ToString()
        {
            return this.ToJson();
        }
    }

    internal class AndSubquery : SearchExpression<IReadOnlyList<ISearchQuery>>
    {
        public AndSubquery(IEnumerable<ISearchQuery> collection)
        {
            if (collection == null || collection.Count() < 2)
            {
                throw new Exception("AND query must have 2 or more subqueries");
            }
            this["$and"] = new List<ISearchQuery>(collection).AsReadOnly();
        }
    }

    internal class OrSubquery : SearchExpression<IReadOnlyList<ISearchQuery>>
    {
        public OrSubquery(IEnumerable<ISearchQuery> collection)
        {
            if (collection == null || collection.Count() < 2)
            {
                throw new Exception("OR query must have 2 or more subqueries");
            }

            this["$or"] = new List<ISearchQuery>(collection).AsReadOnly();
        }
    }

    internal class NotSubquery : SearchExpression<ISearchQuery>
    {
        public NotSubquery(ISearchQuery query)
        {
            this["$not"] = query;
        }
    }

    internal class EqSubquery : SearchExpression<string>
    {
        public EqSubquery(string name, string value)
        {
            this[name] = value;
        }
    }

    internal class NotEqSubquery : SearchExpression<SearchExpression<string>>
    {
        public NotEqSubquery(string name, string value)
        {
            this[name] = new SearchExpression<string> { { "$neq", value } };
        }
    }

    internal class GtSubquery : SearchExpression<SearchExpression<string>>
    {
        public GtSubquery(string name, string value)
        {
            this[$"~{name}"] = new SearchExpression<string> { { "$gt", value } };
        }
    }

    internal class GteSubquery : SearchExpression<SearchExpression<string>>
    {
        public GteSubquery(string name, string value)
        {
            this[$"~{name}"] = new SearchExpression<string> { { "$gte", value } };
        }
    }

    internal class LtSubquery : SearchExpression<SearchExpression<string>>
    {
        public LtSubquery(string name, string value)
        {
            this[$"~{name}"] = new SearchExpression<string> { { "$lt", value } };
        }
    }

    internal class LteSubquery : SearchExpression<SearchExpression<string>>
    {
        public LteSubquery(string name, string value)
        {
            this[$"~{name}"] = new SearchExpression<string> { { "$lte", value } };
        }
    }

    internal class LikeSubquery : SearchExpression<SearchExpression<string>>
    {
        public LikeSubquery(string name, string value)
        {
            this[name] = new SearchExpression<string> { { "$like", value } };
        }
    }

    internal class InSubquery : SearchExpression<SearchExpression<IReadOnlyList<string>>>
    {
        public InSubquery(string name, IEnumerable<string> values)
        {
            if (values == null || !values.Any())
            {
                throw new Exception("IN query must have at least one value");
            }
            this[name] = new SearchExpression<IReadOnlyList<string>> { { "$in", new List<string>(values).AsReadOnly() } };
        }
    }
}
