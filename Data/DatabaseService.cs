
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Utilities;
using StackExchange.Redis;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

public class DatabaseService
{
    private MySqlConnection? connection;
    private MySqlCommand cmd = new MySqlCommand();
    private MySqlDataReader? reader;
    RedisService redisService = new RedisService();
    private const string CONNECTION_STRING = "Server=localhost;";

    //Conexión y desconexión a la base de datos --------------------------------------------------------------------
    public bool CheckConnection(){
        StringBuilder connectionString = new StringBuilder();
        connectionString.Append(CONNECTION_STRING);
        connectionString.Append("Database=ObligatorioBD2023;");
        connectionString.Append("Uid=root;");
        connectionString.Append("Pwd=bernardo;");

        connection = new MySqlConnection(connectionString.ToString());

        try
        {
            connection.Open();
            Console.WriteLine("Conectado a la base de datos... existe!");
            cmd.Connection = connection;
            Disconnect();
            return true;
        }
        catch (MySqlException ex)
        {
            Console.WriteLine("Error al conectar a la base de datos: " + ex.Message);
            return false;
        }
        
    }

    public void Connect()
    {
        StringBuilder connectionString = new StringBuilder();
        connectionString.Append(CONNECTION_STRING);
        connectionString.Append("Database=ObligatorioBD2023;");
        connectionString.Append("Uid=root;");
        connectionString.Append("Pwd=bernardo;");

        connection = new MySqlConnection(connectionString.ToString());

        try
        {
            connection.Open();
            Console.WriteLine("Conectado a la base de datos");
        }
        catch (MySqlException ex)
        {
            Console.WriteLine("Error al conectar a la base de datos: " + ex.Message);
        }
    }

    private void Disconnect()
    {
        if(connection != null){
            connection.Close();
            Console.WriteLine("Desconectado de la base de datos");
        }else{
            Console.Error.WriteLine("Se intento desconectarse de la base de datos pero no habia una conexion activa");
        }
    }

    //-------------------------------------- Funciones de la base de datos -------------------------------------- //
    public bool CheckUser(int ci){


        RedisValue[] lista = redisService.getList("funcionarios"); // Obtener la lista de funcionarios de la cache

        if(lista != null){
            foreach (RedisValue value in lista)
            {
                if(value == ci.ToString()){
                    return true; // Si el funcionario esta en la cache, no hace falta consultar a la base de datos
                }
            }
            return false; // El funcionario no está en la lista de funcionarios de la cache
        }
        else { // Si el funcionario no esta en la cache, hay que consultar a la base de datos
            Connect();
            cmd.Connection = connection;
            cmd.CommandText = "SELECT * FROM Funcionarios WHERE CI = @number;" ;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@number", ci);
            cmd.Prepare();
            cmd.ExecuteNonQuery();
            reader = cmd.ExecuteReader();
            Disconnect();
        }

        return reader.HasRows;
    }

    public bool InsertAgenda(int ci, DateOnly agendDate)
    {
        
        redisService.SetValue(ci.ToString(), agendDate.ToString()); // Agregar la cedula del funcionario junto a la fecha de la agenda al cache

        Connect();
        cmd.Parameters.Clear();
        bool check = false;
        var aux = agendDate.ToString("yyyy-MM-dd");
        string query = $"INSERT INTO Agenda (Ci, Fch_Agenda) VALUES (@number, @text);";
        cmd.Connection = connection;
        cmd.CommandText = query;
        cmd.Parameters.AddWithValue("@number", ci);
        cmd.Parameters.AddWithValue("@text", aux);
        cmd.Prepare();  
        int result = cmd.ExecuteNonQuery();
        if (result > 0)
        {
            check = true;
        }
        Disconnect();
        return check;
    }
    public bool InsertWorker(int ci, string name, string lastName, DateTime birthDate, string adress, int telephone, string email, int logId)
    {

        redisService.setListElement("funcionarios", ci.ToString()); // Agregar el funcionario a la lista de funcionarios de la cache

        Connect();
        cmd.Parameters.Clear();
        bool check = false;
        string query = $"INSERT INTO Funcionarios (Ci, Nombre, Apellido, Fch_Nacimiento, Direccion, Telefono, Email, LogId) VALUES (@ci, @nombre, @apellido, @nacimiento, @direccion, @telefono, @email, @logId)";
        cmd.Connection = connection;
        cmd.CommandText = query;
        cmd.Parameters.AddWithValue("@ci", ci);
        cmd.Parameters.AddWithValue("@nombre", name);
        cmd.Parameters.AddWithValue("@apellido", lastName);
        cmd.Parameters.AddWithValue("@nacimiento", birthDate);
        cmd.Parameters.AddWithValue("@direccion", adress);
        cmd.Parameters.AddWithValue("@telefono", telephone);
        cmd.Parameters.AddWithValue("@email", email);
        cmd.Prepare();  
        int result = cmd.ExecuteNonQuery();
        if (result > 0)
        {
            check = true;
        }
        Disconnect();
        return check;
    }
    public bool RegisterUser(string hashedPassword)
    {
        Connect();
        cmd.Parameters.Clear();
        bool check = false;
        string query = $"INSERT INTO Usuarios (Password) VALUES (@password)";
        cmd.Connection = connection;
        cmd.CommandText = query;
        cmd.Parameters.AddWithValue("@password", hashedPassword);
        cmd.Prepare();  
        int result = cmd.ExecuteNonQuery();
        if (result > 0)
        {
            check = true;
        }
        Disconnect();
        return check;
    }

    public bool InsertCarnet (string Fch_Emision, string Fch_Vencimiento, string Comprobante){

        Connect();
        cmd.Parameters.Clear();
        bool check = false;
        return check;

    }
    public bool UserRegisterTransaction(int ci, string name, string lastName, DateTime birthDate, string adress, int telephone, string email, string password)
    {
        bool transactionCheck = false;
        try
        {
            Connect();
            cmd.Connection = connection;
            cmd.Parameters.Clear();
            cmd.Transaction = connection.BeginTransaction();
            string hashedPassword = MD5Hash.Hash.Content(password);
            bool transaction1 = RegisterUser(hashedPassword);
            string logID = cmd.CommandText = "SELECT LAST_INSERT_ID();";
            bool transaction2 = InsertWorker(ci, name, lastName, birthDate, adress, telephone, email, Int32.Parse(logID));
            if (transaction1 && transaction2)
            {
                cmd.Transaction.Commit();
                transactionCheck = true;
            }
            else
            {
                cmd.Transaction.Rollback();
                transactionCheck = false;
            }
            Disconnect();
        }
        catch (MySqlException ex)
        {
            Console.WriteLine("Error al conectar a la base de datos: " + ex.Message);
        }
        return transactionCheck;
    }
}
 
