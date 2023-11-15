using System.Text;
using MySql.Data.MySqlClient;

public class DatabaseService
{
    private MySqlConnection? connection;
    
    public DatabaseService()
    {
        Conectar();
    }

    public void Conectar()
    {
        StringBuilder connectionString = new StringBuilder();
        connectionString.Append("Server=localhost;");
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
    
    public async Task<bool> CheckUser(int ci){
        string query = "SELECT * FROM Funcionarios WHERE CI = " + ci + ";" ;
        using MySqlCommand command = new MySqlCommand(query.ToString(), connection);
        using MySqlDataReader reader = command.ExecuteReader();
    
        if(reader.Read()){
            return true;
        }
        else{
            return false;
        }
    }
}