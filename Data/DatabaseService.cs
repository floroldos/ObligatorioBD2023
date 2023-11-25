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
            cmd.CommandText = "CREATE TABLE IF NOT EXISTS Logins( LogId INT PRIMARY KEY , Password VARCHAR(50) NOT NULL );";
            cmd.ExecuteNonQuery();
            cmd.CommandText = "CREATE TABLE IF NOT EXISTS Funcionarios( Ci INT(8) PRIMARY KEY , Nombre VARCHAR(50) NOT NULL , Apellido VARCHAR(50) NOT NULL , Fch_Nacimiento DATE NOT NULL , Direccion VARCHAR(100) NOT NULL , Telefono INT NOT NULL , Email VARCHAR(100) NOT NULL , LogId INT NOT NULL , FOREIGN KEY (LogId) REFERENCES Logins(LogId) );";
            cmd.ExecuteNonQuery();
            cmd.CommandText = "CREATE TABLE IF NOT EXISTS Agenda( Nro INT PRIMARY KEY AUTO_INCREMENT , Ci INT(8) NOT NULL , Fch_Agenda DATE NOT NULL );";
            cmd.ExecuteNonQuery();
            cmd.CommandText = "CREATE TABLE IF NOT EXISTS Carnet_Salud( Ci INT(8) PRIMARY KEY , Fch_Emision DATE NOT NULL , Fch_Vencimiento DATE NOT NULL , Comprobante VARCHAR(200) NOT NULL , FOREIGN KEY (Ci) REFERENCES Funcionarios(Ci) );";
            cmd.ExecuteNonQuery();
            cmd.CommandText = "CREATE TABLE IF NOT EXISTS Periodos_Actualizacion( Año YEAR NOT NULL , Semestre VARCHAR(20) , Fch_Inicio DATE PRIMARY KEY , Fch_Fin DATE NOT NULL );";
            cmd.ExecuteNonQuery();
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

}