using MySql.Data.MySqlClient;
using System.Text;

public class DatabaseService
{
    private MySqlConnection? connection;

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
        string query = "SELECT * FROM Funcionarios WHERE CI = " + ci + ";" ;
        using MySqlCommand command = new MySqlCommand(query.ToString(), connection);
        using MySqlDataReader reader = command.ExecuteReader();
        Desconectar();

        return reader.Read() ? true : false;

    }
    public async Task<bool> InsertarRegistroAgenda(string ci, DateOnly fechaAgenda)
    {
        bool check = false;
        string query = $"INSERT INTO Agenda VALUES ('{ci}', '{fechaAgenda}');";
        MySqlCommand command = new MySqlCommand(query, connection);
        int result = command.ExecuteNonQuery();
        if (command.ExecuteNonQuery() > 0)
        {
            check = true;
        }
        return check;
    }

}