using MySql.Data.MySqlClient;
using System.Text;

public class DatabaseService
{
    private MySqlConnection? connection;
    private MySqlCommand cmd = new MySqlCommand();
    private MySqlDataReader reader;

    //Conexión y desconexión a la base de datos --------------------------------------------------------------------
    public void Conectar()
    {
        StringBuilder connectionString = new StringBuilder();
        connectionString.Append("Server=10.4.102.230;");
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
    
    public async Task<bool> CheckUser(int ci){
        Conectar();
        cmd.Connection = connection;
        cmd.CommandText = "SELECT * FROM Funcionarios WHERE CI = @number;" ;
        cmd.Prepare();
        cmd.Parameters.AddWithValue("@number", ci);
        cmd.ExecuteNonQuery();
        reader = cmd.ExecuteReader();
        Desconectar();

        return reader.Read() ? true : false;

    }
    public async Task<bool> InsertarRegistroAgenda(string ci, DateOnly fechaAgenda)
    {
        Conectar();
        bool check = false;
        var aux = fechaAgenda.ToString("yyyy-MM-dd");
        string query = $"INSERT INTO Agenda (Ci, Fch_Agenda) VALUES (@number, @text);";
        cmd.Connection = connection;
        cmd.CommandText = query;
        cmd.Prepare();
        cmd.Parameters.AddWithValue("@number", ci);
        cmd.Parameters.AddWithValue("@text", aux);  
        int result = cmd.ExecuteNonQuery();
        if (result > 0)
        {
            check = true;
        }
        Desconectar();
        return check;
    }

}