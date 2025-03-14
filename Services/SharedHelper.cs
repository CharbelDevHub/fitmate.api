using System.Data;
using Dapper;
using fitmate.api.Models;
using Microsoft.Data.SqlClient;
namespace fitmate.api.Services
{
    public class SharedHelper
    {
        private readonly IConfiguration _configuration;

        public SharedHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected IDbConnection Connection { get; private set; }
        public void OpenConnection()
        {
            // Create a new SqlConnection using the connection string
            Connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            if (Connection.State != ConnectionState.Open)
            {
                Connection.Open();
            }
        }

        // Method to close the connection
        public void CloseConnection()
        {
            if (Connection != null && Connection.State != ConnectionState.Closed)
            {
                Connection.Close();
            }
        }

        public dynamic GetAll(string _tableName)
        {
            string query = " SELECT * FROM  " + _tableName;
            OpenConnection();
            if (_tableToQueryMap.TryGetValue(_tableName, out var queryFunc))
            {
                CloseConnection();
                return queryFunc(Connection, query);
            }

            return null;
        }

        public dynamic GetById(string _tableName, int id)
        {
            string query = " SELECT * FROM " + _tableName + " WHERE ID = " + id;
            if (_tableToQueryMap.TryGetValue(_tableName, out var queryFunc))
            {
                var result = queryFunc(Connection, query);
                if (result.Count() >= 1)
                {
                    return result.First();
                }
            }
            return null;
        }

        public dynamic Insert<T>(List<T> models, IDbTransaction transaction)
        {
            if (models == null || models.Count == 0)
            {
                return -1;
            }

            var properties = typeof(T).GetProperties()
                                      .Where(p => p.CanRead && p.CanWrite && p.Name != "id")
                                      .ToList();

            if (properties.Count == 0)
            {
                return -1;
            }

            var columns = string.Join(", ", properties.Select(p => p.Name));
            var values = string.Join(", ", models.Select((model, index) =>
                $"({string.Join(", ", properties.Select(p => $"@{p.Name}_{index}"))})"));

            var parameters = new DynamicParameters();
            for (int i = 0; i < models.Count; i++)
            {
                var model = models[i];
                foreach (var property in properties)
                {
                    parameters.Add($"{property.Name}_{i}", property.GetValue(model));
                }
            }

            var insertQuery = $"INSERT INTO {typeof(T).Name} ({columns}) VALUES {values}";

            var result = Connection.Execute(insertQuery, parameters, transaction);

            if (result == 0)
            {
                return -1;
            }

            return 1;
        }

        public int Update<T>(List<T> models, IDbTransaction transaction)
        {
            if (models == null || models.Count == 0)
            {
                return -1;
            }

            var updateQueries = new List<string>();
            var parameters = new List<DynamicParameters>();

            foreach (var model in models)
            {
                var properties = typeof(T).GetProperties()
                                          .Where(p => p.CanRead && p.CanWrite && p.Name != "id")
                                          .ToList();

                var setClause = string.Join(", ", properties.Select(p => $"{p.Name} = @{p.Name}"));
                var idProperty = typeof(T).GetProperty("id");
                var idValue = idProperty?.GetValue(model);

                if (idValue == null || Convert.ToInt32(idValue) <= 0)
                {
                    continue;
                }

                var modelParams = new DynamicParameters();
                foreach (var property in properties)
                {
                    modelParams.Add($"@{property.Name}", property.GetValue(model));
                }
                modelParams.Add("@id", idValue);

                var updateQuery = $"UPDATE {typeof(T).Name} SET {setClause} WHERE id = @id";
                updateQueries.Add(updateQuery);
                parameters.Add(modelParams);
            }

            if (updateQueries.Count == 0)
            {
                return -1;
            }

            var result = Connection.Execute(string.Join(";", updateQueries), parameters.Select(p => p).ToArray(), transaction);

            transaction.Commit();
            return result > 0 ? 1 : -1;
        }

        public dynamic Delete(string _tableName, List<int> ids)
        {
            if (ids == null || ids.Count == 0)
            {
                return -1;
            }

            var idList = string.Join(", ", ids);

            string query = $"DELETE FROM {_tableName} WHERE ID IN ({idList})";

            var result = Connection.Execute(query);

            return result > 0 ? 1 : -1;

        }


        private readonly Dictionary<string, Func<IDbConnection, string, IEnumerable<dynamic>>> _tableToQueryMap = new Dictionary<string, Func<IDbConnection, string, IEnumerable<dynamic>>>
        {
            { "EXERCISE_CATEGORY", (conn, query) => conn.Query<EXERCISE_CATEGORY>(query) },
            //{ "SEC_USERS", (conn, query) => conn.Query<SEC_USERS>(query) },
            //{ "SC_SCRIPT", (conn, query) => conn.Query<SC_SCRIPT>(query) },
            //{ "SC_COMPANY_ENV", (conn, query) => conn.Query<SC_COMPANY_ENV>(query) },
        };
    }
}
