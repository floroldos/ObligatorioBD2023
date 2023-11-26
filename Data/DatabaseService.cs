
using MySql.Data.MySqlClient;
using System.Text;

public class DatabaseService
{
    private MySqlConnection? connection;
    private MySqlCommand cmd = new MySqlCommand();
    private MySqlDataReader? reader;
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
            Desconectar();
            return true;
        }
        catch (MySqlException ex)
        {
            Console.WriteLine("Error al conectar a la base de datos: " + ex.Message);
            return false;
        }
        
    }

    public void Conectar()
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

    private void Desconectar()
    {
        if(connection != null){
            connection.Close();
            Console.WriteLine("Desconectado de la base de datos");
        }else{
            Console.Error.WriteLine("Se intento desconectarse de la base de datos pero no habia una conexion activa");
        }
    }

    //Funciones de la base de datos --------------------------------------------------------------------------------
    
    public bool CheckUser(int ci){
        Conectar();
        cmd.Connection = connection;
        cmd.CommandText = "SELECT * FROM Funcionarios WHERE CI = @number;" ;
        cmd.Parameters.Clear();
        cmd.Parameters.AddWithValue("@number", ci);
        cmd.Prepare();
        cmd.ExecuteNonQuery();
        reader = cmd.ExecuteReader();
        Desconectar();

        return reader.HasRows;
    }
    public bool InsertarRegistroAgenda(string ci, DateOnly agendDate)
    {
        Conectar();
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
        Desconectar();
        return check;
    }
    public bool InsertarFuncionario(string ci, string nombre, string apellido, DateOnly nacimiento, string direccion, string telefono, string email, string logId)
    {
        Conectar();
        cmd.Parameters.Clear();
        bool check = false;
        string query = $"INSERT INTO Funcionarios (Ci, Nombre, Apellido, Fch_Nacimiento, Direccion, Telefono, Email, LogId) VALUES (@ci, @nombre, @apellido, @nacimiento, @direccion, @telefono, @email)";
        cmd.Connection = connection;
        cmd.CommandText = query;
        cmd.Parameters.AddWithValue("@ci", ci);
        cmd.Parameters.AddWithValue("@nombre", nombre);
        cmd.Parameters.AddWithValue("@apellido", apellido);
        cmd.Parameters.AddWithValue("@nacimiento", nacimiento);
        cmd.Parameters.AddWithValue("@direccion", direccion);
        cmd.Parameters.AddWithValue("@telefono", telefono);
        cmd.Parameters.AddWithValue("@email", email);
        cmd.Parameters.AddWithValue("@logId", logId);
        cmd.Prepare();  
        int result = cmd.ExecuteNonQuery();
        if (result > 0)
        {
            check = true;
        }
        Desconectar();
        return check;
    }
}

