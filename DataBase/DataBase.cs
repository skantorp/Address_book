using DataBase.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace DataBase
{
    public class AddressDataBase
    {
        private string ConnectionString = @"Data Source=DESKTOP-5MLILEF\SQLEXPRESS;Initial Catalog=AddressBook;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        private string ServerConnectionString = @"Data Source=DESKTOP-5MLILEF\SQLEXPRESS;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        public void CreateProcedures()
        {
            string userProcedureQuery = @"
                            
                            CREATE PROCEDURE GetAllUsers 
                            AS  
                            BEGIN  
            
                                SET NOCOUNT ON;  
   
                               SELECT * FROM Users
                            END   
                            ";

            string phoneProcedureQuery = @"
                            CREATE PROCEDURE SelectPhonesByUserId @UserId int 
                            AS  
                            BEGIN   
                                SET NOCOUNT ON;  
                                    
                               SELECT * FROM Phones WHERE UserId=@UserId
                            END ";

            string setUpDataProcedureQuery = @"CREATE PROCEDURE SettingUpData   
  
                                            AS  
                                            BEGIN   
                                                SET NOCOUNT ON;  
  
                                                insert into Users(Id, Name) values(1, 'Bohdan'),(2, 'Liubomyr'),(3, 'Boryslav'),(4, 'Stepan')


                                                                    DECLARE @Counter INT
                                                                    DECLARE @randomUserId INT
                                                        DECLARE @randomIsActive bit
                                                        SET @Counter = 1
                                                        WHILE(@Counter <= 9)
                                                        BEGIN

                                                            set @randomUserId = ABS(CHECKSUM(NEWID()) % (4)) + 1

                                                            set @randomIsActive = ABS(CHECKSUM(NEWID()) % (2))
                                                            insert into Phones(Id, Number, UserId, IsActive)

                                                            values(@Counter, '012 345 67 0' + CAST(@Counter AS nvarchar), @randomUserId, @randomIsActive)
                                                            SET @Counter = @Counter + 1
                                                        END 
  
                                            END ";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(userProcedureQuery, conn))
                {                   
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                }

                using (SqlCommand cmd = new SqlCommand(phoneProcedureQuery, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                }
                
                using (SqlCommand cmd = new SqlCommand(setUpDataProcedureQuery, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                }
                conn.Close();
            }
        }

        public bool CheckDbExists()
        {
            string sqlCreateDBQuery;
            bool result = false;


            try
            {
                sqlCreateDBQuery = string.Format("SELECT database_id FROM sys.databases WHERE Name = 'AddressBook'");

                using (var tmpConn = new SqlConnection(ServerConnectionString))
                {
                    using (SqlCommand sqlCmd = new SqlCommand(sqlCreateDBQuery, tmpConn))
                    {
                        tmpConn.Open();

                        object resultObj = sqlCmd.ExecuteScalar();

                        int databaseID = 0;

                        if (resultObj != null)
                        {
                            int.TryParse(resultObj.ToString(), out databaseID);
                        }

                        tmpConn.Close();

                        result = (databaseID > 0);
                    }
                }
            }
            catch
            {
                result = false;
            }

            return result;
        }

        void CreateDb()
        {
            string commandText = "CREATE DATABASE AddressBook;";

            using (SqlConnection conn = new SqlConnection(ServerConnectionString))
            using (SqlCommand cmd = new SqlCommand(commandText, conn))
            {
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }

        void CreateTables()
        {
            string commandText = @"
            CREATE TABLE Users(
                Id int NOT NULL PRIMARY KEY,
                Name varchar(255) NOT NULL
            );

            CREATE TABLE Phones(
                Id int NOT NULL PRIMARY KEY,
                Number varchar(255) NOT NULL,
                UserId int FOREIGN KEY REFERENCES Users(Id),
                IsActive bit
            ); ";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            using (SqlCommand cmd = new SqlCommand(commandText, conn))
            {
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }

        void SetUpData()
        {
            string commandText = @"exec SettingUpData";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            using (SqlCommand cmd = new SqlCommand(commandText, conn))
            {
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }

        public void SetUpDb()
        {
            CreateDb();
            CreateTables();
            CreateProcedures();
            SetUpData();
        }

        public List<UserDTO> GetAllUsers()
        {
            using (var conn = new SqlConnection(ConnectionString))
            using (var command = new SqlCommand("GetAllUsers", conn)
            {
                CommandType = CommandType.StoredProcedure
            })
            {
                List<UserDTO> retunvalue = new List<UserDTO>();
                conn.Open();

                SqlDataReader reader;
                reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        retunvalue.Add(new UserDTO { Id = reader.GetInt32(0), Name = reader.GetString(1) });
                    }
                }
                conn.Close();

                return retunvalue;
            }
        }

        public List<PhoneDTO> GetPhonesByUser(int UserId)
        {
            using (var conn = new SqlConnection(ConnectionString))
            using (var command = new SqlCommand("SelectPhonesByUserId", conn)
            {
                CommandType = CommandType.StoredProcedure
            })
            {
                command.Parameters.AddWithValue("@UserId", UserId);
                List<PhoneDTO> retunvalue = new List<PhoneDTO>();
                conn.Open();

                SqlDataReader reader;
                reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        retunvalue.Add(new PhoneDTO { Id = reader.GetInt32(0), Number = reader.GetString(1), UserId = reader.GetInt32(2), IsActive = reader.GetBoolean(3) });
                    }
                }
                conn.Close();

                return retunvalue;
            }
        }
    }
}
