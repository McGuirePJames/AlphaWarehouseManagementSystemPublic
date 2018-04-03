using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;


namespace WarehouseManagementSystem.Models
{
    public class Database
    {
        public string Name { get; set; }
        public string ConnectionString { get; set; }

        internal static string GetConnectionString()
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings[""].ToString();

            return connectionString;
        }
        internal static string GetConnectionStringReadOnly()
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings[""].ToString();

            return connectionString;
        }
        internal static async Task<List<Models.Excel.Workbook.Worksheet.Row>> RunQuery(string sqlQuery)
        {
            List<Models.Excel.Workbook.Worksheet.Row> rows = new List<Models.Excel.Workbook.Worksheet.Row>();

            using (SqlConnection conn = new SqlConnection(WarehouseManagementSystem.Models.Database.GetConnectionStringReadOnly()))
            {
                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    try
                    {
                        await conn.OpenAsync();

                        SqlDataReader rdr = await cmd.ExecuteReaderAsync();

                        int rowCount = 1;

                        Models.Excel.Workbook.Worksheet.Row headerRow = new Models.Excel.Workbook.Worksheet.Row();
                        headerRow.RowNumber = rowCount;

                        for (int i = 1; i <= rdr.FieldCount; i++)
                        {
                            Models.Excel.Workbook.Worksheet.Cell cell = new Models.Excel.Workbook.Worksheet.Cell();
                            cell.ColumnNumber = i;
                            cell.RowNumber = rowCount;
                            cell.Value = rdr.GetName(i - 1);
                            headerRow.Cells.Add(cell);
                        }
                        rows.Add(headerRow);
                        rowCount++;
                        while (rdr.Read())
                        {

                            Models.Excel.Workbook.Worksheet.Row row = new Models.Excel.Workbook.Worksheet.Row();
                            row.RowNumber = rowCount;

                            for (int i = 1; i <= rdr.FieldCount; i++)
                            {
                                Models.Excel.Workbook.Worksheet.Cell cell = new Models.Excel.Workbook.Worksheet.Cell();
                                cell.ColumnNumber = i;
                                cell.RowNumber = rowCount;
                                cell.Value = rdr[i - 1].ToString();

                                row.Cells.Add(cell);
                            }
                            rows.Add(row);
                            rowCount++;
                        }
                    }
                    catch(SqlException sqlEx)
                    {
                        string sqlException = sqlEx.Message.ToString();
                    }
                    catch(Exception ex)
                    {
                        string exception = ex.Message.ToString();
                    }

                }
            }
            return rows;
        }

        public class Table
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public List<Models.Database.Table.Column> Columns { get; set; }

            internal static async Task<List<Models.Database.Table>> GetTables()
            {
                List<Models.Database.Table> tables = new List<Models.Database.Table>();

                string sqlQuery =
                            @"SELECT SYS.Tables.object_id, SYS.Tables.name " +
                            @"FROM SYS.Tables " +
                            @"ORDER BY SYS.Tables.Name ";

                using (SqlConnection conn = new SqlConnection(WarehouseManagementSystem.Models.Database.GetConnectionString()))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                    {
                        await conn.OpenAsync();

                        SqlDataReader rdr = await cmd.ExecuteReaderAsync();

                        while (rdr.Read())
                        {
                            Models.Database.Table table = new Models.Database.Table();
                            table.Id = Convert.ToInt32(rdr["object_id"]);
                            table.Name = rdr["name"].ToString();
                            tables.Add(table);
                        }
                    }
                }
                return tables;
            }

            public class Column
            {
                public int TableId { get; set; }
                public int Id { get; set; }
                public string Name { get; set; }
                public Boolean IsPrimaryKey { get; set; } = false;
                public Boolean IsForeignkey { get; set; } = false;
                public Key.ForeignKey ForeignKey { get; set; }
                public Key.PrimaryKey PrimaryKey { get; set; }

                internal static async Task<List<Models.Database.Table.Column>> GetColumns()
                {
                    List<Models.Database.Table.Column> columns = new List<Models.Database.Table.Column>();

                    string sqlQuery =
                                    @"SELECT SYS.Columns.object_id, SYS.Columns.name, SYS.Columns.column_id " +
                                    @"FROM SYS.Tables " +
                                    @"INNER JOIN SYS.Columns " +
                                    @"ON SYS.Tables.object_id = SYS.Columns.object_id " +
                                    @"ORDER BY SYS.Columns.column_id ASC";

                    using (SqlConnection conn = new SqlConnection(WarehouseManagementSystem.Models.Database.GetConnectionString()))
                    {
                        using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                        {
                            await conn.OpenAsync();

                            SqlDataReader rdr = await cmd.ExecuteReaderAsync();

                            while (rdr.Read())
                            {
                                Models.Database.Table.Column column = new Models.Database.Table.Column();
                                column.TableId = Convert.ToInt32(rdr["object_id"]);
                                column.Name = rdr["name"].ToString();
                                column.Id = Convert.ToInt32(rdr["column_id"]);
                                columns.Add(column);
                            }
                        }
                    }
                    return columns;
                }
            }   
        }
        public class Key
        {
            public class ForeignKey
            {
                public Models.Database.Table ParentTable { get; set; }
                public Models.Database.Table ChildTable { get; set; }
                public int ChildColumnId { get; set; }
                public int ParentColumnId { get; set; }

                public ForeignKey()
                {
                    this.ParentTable = new Models.Database.Table();
                    this.ChildTable = new Models.Database.Table();
                }

                internal static async Task<List<Models.Database.Key.ForeignKey>> GetForeignKeys()
                {
                    List<Models.Database.Key.ForeignKey> foreignKeys = new List<Models.Database.Key.ForeignKey>();

                    string sqlQuery =
                                @"SELECT Parent.Name[ParentTableName], Parent.Object_Id[ParentTableId], SYS.Foreign_Key_Columns.referenced_column_id[ParentColumnId], Child.Name[ChildTableName], Child.Object_Id[ChildTableId], SYS.Foreign_Key_Columns.parent_column_id[ChildColumnId] " +
                                @"FROM SYS.Foreign_Key_Columns " +
                                @"INNER JOIN SYS.Tables Child " +
                                @"ON SYS.Foreign_Key_Columns.parent_object_id = Child.object_id " +
                                @"INNER JOIN SYS.Tables Parent " +
                                @"ON SYS.Foreign_Key_Columns.referenced_object_id = Parent.object_id " +
                                @"INNER JOIN SYS.Columns " +
                                @"ON SYS.Foreign_Key_Columns.parent_object_id = SYS.Columns.object_id " +
                                @"AND SYS.Foreign_Key_Columns.referenced_column_id = SYS.Columns.column_id " +
                                @"ORDER BY Parent.Name ASC, SYS.Foreign_Key_Columns.parent_column_id ";

                    using (SqlConnection conn = new SqlConnection(WarehouseManagementSystem.Models.Database.GetConnectionString()))
                    {
                        using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                        {
                            await conn.OpenAsync();

                            SqlDataReader rdr = await cmd.ExecuteReaderAsync();

                            while (rdr.Read())
                            {
                                Models.Database.Key.ForeignKey foreignKey = new Models.Database.Key.ForeignKey();
                                foreignKey.ParentTable.Name = rdr["ParentTableName"].ToString();
                                foreignKey.ParentTable.Id = Convert.ToInt32(rdr["ParentTableId"]);
                                foreignKey.ChildTable.Name = rdr["ChildTableName"].ToString();
                                foreignKey.ChildTable.Id = Convert.ToInt32(rdr["ChildTableId"]);
                                foreignKey.ChildColumnId = Convert.ToInt32(rdr["ChildColumnId"]);
                                foreignKey.ParentColumnId = Convert.ToInt32(rdr["ParentColumnId"]);

                                foreignKeys.Add(foreignKey);
                            }
                        }
                    }
                    return foreignKeys;
                }
            }
            public class PrimaryKey
            {
                public Models.Database.Table Table { get; set; }
                public int ColumnId { get; set; }

                public PrimaryKey()
                {
                    this.Table = new Models.Database.Table();
                }
                internal static async Task<List<Models.Database.Key.PrimaryKey>> GetPrimaryKeys()
                {
                    List<Models.Database.Key.PrimaryKey> primaryKeys = new List<Models.Database.Key.PrimaryKey>();

                    string sqlQuery =
                                @"SELECT SYS.Tables.Name[TableName], SYS.Tables.Object_ID[TableID], SYS.Columns.Name[ColumnName], SYS.Columns.Column_ID[ColumnID] " +
                                @"FROM SYS.Key_Constraints " +
                                @"INNER JOIN SYS.Index_Columns " +
                                @"ON SYS.Key_Constraints.parent_object_id = SYS.Index_Columns.object_id " +
                                @"AND SYS.Key_Constraints.unique_index_id = SYS.Index_Columns.index_id " +
                                @"INNER JOIN SYS.Columns " +
                                @"ON SYS.Index_Columns.object_id = SYS.Columns.object_id " +
                                @"AND SYS.Index_Columns.column_id = SYS.Columns.column_id " +
                                @"INNER JOIN SYS.Tables " +
                                @"ON SYS.Columns.Object_Id = SYS.Tables.Object_ID " +
                                @"WHERE Key_Constraints.type = 'PK' ";

                    using (SqlConnection conn = new SqlConnection(WarehouseManagementSystem.Models.Database.GetConnectionString()))
                    {
                        using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                        {
                            await conn.OpenAsync();

                            SqlDataReader rdr = await cmd.ExecuteReaderAsync();

                            while (rdr.Read())
                            {
                                Models.Database.Key.PrimaryKey primaryKey = new Models.Database.Key.PrimaryKey();
                                primaryKey.Table.Name = rdr["TableName"].ToString();
                                primaryKey.Table.Id = Convert.ToInt32(rdr["TableID"]);
                                primaryKey.ColumnId = Convert.ToInt32(rdr["ColumnID"].ToString());

                                primaryKeys.Add(primaryKey);
                            }
                        }
                    }
                    return primaryKeys;
                }            
            }
        }
    }
}