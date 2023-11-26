
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Utilities;
using StackExchange.Redis;
using System.Data;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Drawing;

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

    public bool selectAgendas(string ci, string fecha){
        Connect();
        cmd.Parameters.Clear();
        bool check = false;
        string query = $"SELECT * FROM Agenda WHERE ci IS NULL AND Fch_Agenda = {fecha};";
        cmd.Connection = connection;
        cmd.CommandText = query;
        cmd.Prepare(); 
        cmd.ExecuteNonQuery(); 
        MySqlDataReader result = cmd.ExecuteReader();

        if (result != null)
        {
            cmd.Parameters.Clear();
            string queryUpd = $"UPDATE Agenda SET ci = {ci} WHERE Fch_Agenda = {fecha};";
            cmd.Connection = connection;
            cmd.CommandText = query;
            cmd.Prepare();  
            var resultQuery = cmd.ExecuteNonQuery();
            if(resultQuery > 0 ){
                check = true;
            }
            else{
                Console.WriteLine("Hubo un error actualizando la base de datos");
            }
        }
        Disconnect();
        return check;
    }

    public bool InsertAgenda(int ci, DateOnly agendDate)
    {
        
        redisService.SetValue(ci.ToString(), agendDate.ToString()); // Agregar la cedula del funcionario junto a la fecha de la agenda al cache
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
    public bool InsertWorker(int ci, string name, string lastName, DateTime birthDate, string adress, int telephone, string email, int logID)
    {

        redisService.setListElement("funcionarios", ci.ToString()); // Agregar el funcionario a la lista de funcionarios de la cache

        Connect();
        cmd.Parameters.Clear();
        cmd.Connection = connection;
        cmd.Connection.BeginTransaction();
        bool check = false;

        string query = $"INSERT INTO Funcionarios (Ci, Nombre, Apellido, Fch_Nacimiento, Direccion, Telefono, Email, LogId) VALUES (@ci, @nombre, @apellido, @nacimiento, @direccion, @telefono, @email, @logId)";
        cmd.CommandText = query;
        cmd.Parameters.AddWithValue("@ci", ci);
        cmd.Parameters.AddWithValue("@nombre", name);
        cmd.Parameters.AddWithValue("@apellido", lastName);
        cmd.Parameters.AddWithValue("@nacimiento", birthDate);
        cmd.Parameters.AddWithValue("@direccion", adress);
        cmd.Parameters.AddWithValue("@telefono", telephone);
        cmd.Parameters.AddWithValue("@email", email);
        cmd.Parameters.AddWithValue("@logId", logID);
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
        Console.WriteLine("ENTRO A REGISTER USER");
        Connect();
        cmd.Parameters.Clear();
        bool check = false;
        string query = $"INSERT INTO Logins (Password) VALUES (@password)";
        cmd.Connection = connection;
        cmd.CommandText = query;
        cmd.Parameters.AddWithValue("@password", hashedPassword);
        cmd.Prepare();  
        int result = cmd.ExecuteNonQuery();
        Console.WriteLine("EL RESULTADO DE INSERTAR EN LOGINS ES: " + result);
        if (result > 0)
        {
            check = true;
        }
        Disconnect();
        return check;
    }

    public bool InsertCarnet (DateTime fch_emision, DateTime fch_vencimiento, string comprobante){

        // redis Settear

        Connect();
        cmd.Parameters.Clear();
        bool check = false;
        string query = $"INSERT INTO Carnet_Salud (Fch_Emision, Fch_Vencimiento, Comprobante) VALUES (@fch_emision, @fch_vencimiento, @comprobante)";
        cmd.Connection = connection;
        cmd.CommandText = query;
        cmd.Parameters.AddWithValue("@fch_emision", fch_emision);
        cmd.Parameters.AddWithValue("@fch_vencimiento", fch_vencimiento);
        cmd.Parameters.AddWithValue("@comprobante", comprobante);
        cmd.Prepare();
        int result = cmd.ExecuteNonQuery();
        if (result > 0)
        {
            check = true;
        }
        Disconnect();
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
            Console.WriteLine("EL LOGID ES ESTE: " + logID);
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
