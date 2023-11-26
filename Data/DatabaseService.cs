using MySql.Data.MySqlClient;
using System.Text;
using System.Net.Mail;
using System.Net;


public class DatabaseService
{
    private MySqlConnection? connection;
    private MySqlCommand cmd = new MySqlCommand();
    private MySqlDataReader? reader;
    private const string CONNECTION_STRING = "Server=localhost;";

    private static DatabaseService? instance = null;
    public static DatabaseService GetInstance(){
        if(instance == null){
            instance = new DatabaseService();
        }
        return instance;
    }

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
        cmd.Prepare();
        cmd.Parameters.AddWithValue("@number", ci);
        cmd.ExecuteNonQuery();
        reader = cmd.ExecuteReader();
        Desconectar();

        return reader.HasRows;
    }

    public bool InsertarRegistroAgenda(string ci, DateOnly fechaAgenda)
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
    public bool CheckIfValidDate()
    {
    bool validDate = false;

    Conectar();

    DateTime currentDate = DateTime.Now;
    int YEAR = currentDate.Year;
    int MONTH = currentDate.Month;
    int SEMESTER;

    if (MONTH >= 1 && MONTH <= 6) {
        SEMESTER = 1;
    } else {
        SEMESTER = 2;
    }

    string query = $"SELECT * FROM Periodos_Actualizacion WHERE Fch_Inicio <= @date AND Fch_Fin >= @date AND Año = @year AND Semestre = @semester;";

    cmd.Connection = connection;
    cmd.CommandText = query;
    cmd.Parameters.Clear();
    cmd.Parameters.AddWithValue("@date", currentDate);
    cmd.Parameters.AddWithValue("@year", YEAR);
    cmd.Parameters.AddWithValue("@semester", SEMESTER);
    cmd.Prepare();
    cmd.ExecuteNonQuery();

    reader = cmd.ExecuteReader();

    if (reader.HasRows) {
        validDate = true;
    }

    Desconectar();
    return validDate;
    }

    public DateTime? GetLastValidDate(){
        Conectar();

        DateTime currentDate = DateTime.Now;
        int YEAR = currentDate.Year;

        string query = $"SELECT * FROM Periodos_Actualizacion WHERE Año = @year ORDER BY Fch_Fin DESC LIMIT 1;";
        cmd.Connection = connection;
        cmd.CommandText = query;
        cmd.Parameters.Clear();
        cmd.Parameters.AddWithValue("@year", YEAR);
        cmd.Prepare();
        cmd.ExecuteNonQuery();

        reader = cmd.ExecuteReader();

        DateTime? lastValidDate = null;

        if(reader.HasRows){
            reader.Read();
            lastValidDate = reader.GetDateTime(3);
        }


        Desconectar();
        return lastValidDate;
    }
}