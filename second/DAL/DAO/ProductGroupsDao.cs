using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Data.SqlClient;
using second.DAL.Entity;

namespace second.DAL.DAO
{
    internal class ProductGroupsDao
    {
        private readonly SqlConnection _connection;

        public ProductGroupsDao(SqlConnection connection)
        {
            _connection = connection;
        }

        public List<Entity.ProductGroup> getAll()
        {
            SqlCommand command = new SqlCommand();
            command.Connection = _connection;
            command.CommandText = "SELECT * FROM ProductGroups";
            try
            {
                SqlDataReader reader = command.ExecuteReader();
                var ProductGroups = new List<Entity.ProductGroup>();
                while (reader.Read())
                {
                    // columns.Add($"Id {reader.GetGuid(0).ToString().Substring(0,4)}, Name : {reader.GetString(1)},\n Description: {reader.GetString(2)},\n Picture: {reader.GetString(3)}");
                    if (reader.IsDBNull(4))
                    {
                        ProductGroups.Add(new DAL.Entity.ProductGroup()
                        {
                            Id = reader.GetGuid(0),
                            Name = reader.GetString(1),
                            Description = reader.GetString(2),
                            Picture = reader.GetString(3),
                        });
                    }
                }
                reader.Close();
                return ProductGroups;
            }
            catch { throw; }
        }
        public void Add(Entity.ProductGroup productGroup)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = _connection;
            cmd.CommandText = $"INSERT INTO ProductGroups ( Id, Name, Description, Picture ) VALUES ( @id, @name, @description, @picture)";
            cmd.Prepare();
            cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.UniqueIdentifier ));
            cmd.Parameters.Add(new SqlParameter("@name", SqlDbType.NVarChar, 50));
            cmd.Parameters.Add(new SqlParameter("@description", SqlDbType.NVarChar, 50));
            cmd.Parameters.Add(new SqlParameter("@picture", SqlDbType.NVarChar, 50));
            cmd.Parameters[0].Value = productGroup.Id;
            cmd.Parameters[1].Value = productGroup.Name;
            cmd.Parameters[2].Value = productGroup.Description;
            cmd.Parameters[3].Value = productGroup.Picture;
            cmd.ExecuteNonQuery();
        }

        public void Delete(Entity.ProductGroup productGroup)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = _connection;
            cmd.CommandText = $"UPDATE ProductGroups SET DeleteDT = CURRENT_TIMESTAMP WHERE Id = @id ";
            cmd.Prepare();
            cmd.Parameters.Add(new SqlParameter("id", SqlDbType.UniqueIdentifier ));
            cmd.Parameters[0].Value = productGroup.Id;
            cmd.ExecuteNonQuery ();
        }

        public void Update(Entity.ProductGroup productGroup)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = _connection;
            cmd.CommandText = $"UPDATE ProductGroups SET Name = @name, Description = @description, Picture = @picture WHERE Id = @id ";
            cmd.Prepare();
            cmd.Parameters.Add(new SqlParameter("@name", SqlDbType.NVarChar, 50));
            cmd.Parameters.Add(new SqlParameter("@description", SqlDbType.NVarChar, 50));
            cmd.Parameters.Add(new SqlParameter("@picture", SqlDbType.NVarChar, 50));
            cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.UniqueIdentifier));
            cmd.Parameters[0].Value = productGroup.Name;
            cmd.Parameters[1].Value = productGroup.Description;
            cmd.Parameters[2].Value = productGroup.Picture;
            cmd.Parameters[3].Value = productGroup.Id;
            cmd.ExecuteNonQuery ();
        }

        public List<Entity.ProductGroup> getDeleted()
        {
            SqlCommand command = new SqlCommand();
            command.Connection = _connection;
            command.CommandText = "SELECT * FROM ProductGroups";
            try
            {
                SqlDataReader reader = command.ExecuteReader();
                var ProductGroups = new List<Entity.ProductGroup>();
                while (reader.Read())
                {
                    if (!reader.IsDBNull(4))
                    {
                        ProductGroups.Add(new DAL.Entity.ProductGroup()
                        {
                            Id = reader.GetGuid(0),
                            Name = reader.GetString(1),
                            Description = reader.GetString(2),
                            Picture = reader.GetString(3),
                        });
                    }
                }
                reader.Close();
                return ProductGroups;
            }
            catch { throw; }
        }

        public void Restore(Entity.ProductGroup productGroup)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = _connection;
            cmd.CommandText = $"UPDATE ProductGroups SET DeleteDT = NULL WHERE Id = @id ";
            cmd.Prepare();
            cmd.Parameters.Add(new SqlParameter("id", SqlDbType.UniqueIdentifier));
            cmd.Parameters[0].Value = productGroup.Id;
            cmd.ExecuteNonQuery();
        }
    }
}
