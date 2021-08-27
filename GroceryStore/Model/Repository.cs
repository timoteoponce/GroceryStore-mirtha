using GroceryStore.Tests;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroceryStore.Model
{
    /// <summary>
    /// Component in charge of persisting/fetching/deleting all entities into a data store.
    /// </summary>
    public class Repository
    {
        const string DB_NAME = "database.sqlite";

        public void Save(Customer c)
        {
            using (var conn = InitConnection())
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = @"
                    INSERT INTO CUSTOMER(ID, FIRST_NAME, LAST_NAME, AGE)
                    VALUES($Id, $FirstName, $LastName, $Age)
                ";
                var param = cmd.Parameters;
                param.AddWithValue("$Id", c.Id);
                param.AddWithValue("$FirstName", c.FirstName);
                param.AddWithValue("$LastName", c.LastName);
                param.AddWithValue("$Age", c.Age);
                if (cmd.ExecuteNonQuery() != 1)
                {
                    throw new SystemException($"Customer could not be saved {c}");
                }
            }
        }

        /// <summary>
        /// Returns a list of all existing customers
        /// </summary>
        /// <returns>list with all existings customers, or empty if there are none</returns>
        public List<Customer> GetCustomers()
        {
            List<Customer> result = new();
            using (var conn = InitConnection())
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT ID, FIRST_NAME,LAST_NAME,AGE FROM CUSTOMER";
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(ReadCustomer(reader));
                    }
                }
            }
            return result;
        }

        private Customer ReadCustomer(SqliteDataReader reader)
        {
            return new Customer
            {
                Id = reader.GetInt32(0),
                FirstName = reader.GetString(1),
                LastName = reader.GetString(2),
                Age = reader.GetInt32(3)
            };
        }

        /// <summary>
        /// Method in charge of initializing the database and return an open-valid connection to an initialized db
        /// </summary>
        /// <returns>Connection for an initialized db</returns>
        private SqliteConnection InitConnection()
        {
            var connectionString = $"Data Source={DB_NAME}";
            //create tables if not present
            using (var conn = new SqliteConnection(connectionString))
            {
                conn.Open();
                List<string> createTableCommands = new()
                {
                    "CREATE TABLE IF NOT EXISTS CUSTOMER(ID INTEGER PRIMARY KEY, FIRST_NAME VARCHAR, LAST_NAME VARCHAR, AGE INTEGER)"
                };
                var cmd = conn.CreateCommand();
                foreach (var sql in createTableCommands)
                {
                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();
                }
            }
            return new SqliteConnection(connectionString);
        }


        /// <summary>
        /// Deletes the whole existing data if present in the current directory
        /// </summary>
        public void ClearData()
        {
            if (File.Exists(DB_NAME))
            {
                File.Delete(DB_NAME);
            }
        }
    }
}
