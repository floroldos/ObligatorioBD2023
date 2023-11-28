
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Utilities;
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
    private const string CONNECTION_STRING = "Server=db;";

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
        }else{
            Console.Error.WriteLine("Se intento desconectarse de la base de datos pero no habia una conexion activa");
        }
    }

    //-------------------------------------- Funciones de la base de datos -------------------------------------- //
    public bool CheckUser(int ci){



            Connect();
            cmd.Connection = connection;
            cmd.CommandText = "SELECT * FROM Funcionarios WHERE CI = @number;" ;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@number", ci);
            cmd.Prepare();
            cmd.ExecuteNonQuery();
            reader = cmd.ExecuteReader();
            Disconnect();
        

        return reader.HasRows;
    }
    public bool Login(int ci, string hashedPassword){
        Connect();
        cmd.Connection = connection;
        cmd.CommandText = "SELECT LogId FROM Funcionarios WHERE CI = @number;";
        cmd.Parameters.Clear();
        cmd.Parameters.AddWithValue("@number", ci);
        cmd.Prepare();
        var logId = cmd.ExecuteScalar();
        if (logId == null)
        {
            throw new System.ArgumentException("El usuario no existe");
        }else{
            logId = (int)logId;
        }
        cmd.CommandText = "SELECT Password FROM Logins WHERE LogId = @logId;";
        cmd.Parameters.Clear();
        cmd.Parameters.AddWithValue("@logId", logId);
        cmd.Prepare();
        string dbPassword = (string)cmd.ExecuteScalar();
        Disconnect();
        if (dbPassword == hashedPassword)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool selectAgendas(string ci, string fecha){
        Console.WriteLine("a"); 
        Console.WriteLine(ci);
        Console.WriteLine(fecha);
        ci = "53692872";
        Connect();
        bool check = false;
        string query = $"SELECT * FROM Agenda WHERE Ci IS NULL AND Fch_Agenda = @fecha;";
        cmd.Connection = connection;
        cmd.Parameters.Clear();
        cmd.Parameters.AddWithValue("@fecha", fecha);
        cmd.CommandText = query;
        cmd.Prepare(); 
        cmd.ExecuteNonQuery(); 
        MySqlDataReader result = cmd.ExecuteReader();
        
        Disconnect();
        
        if (result != null)
        {
            Connect();
            cmd.Parameters.Clear();
            string queryUpd = $"UPDATE Agenda SET Ci = @ci WHERE Fch_Agenda = @fecha;";
            cmd.Connection = connection;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@fecha", fecha);
            cmd.Parameters.AddWithValue("@ci", ci);
            cmd.CommandText = queryUpd;
            cmd.Prepare();  
            var resultQuery = cmd.ExecuteNonQuery();
            if(resultQuery > 0 ){
                check = true;
            }
            Disconnect();
        }
        return check;
    }

    public bool InsertAgenda(int ci, DateOnly agendDate)
    {
        
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
    
    

    public bool InsertCarnet (int ci, DateTime fch_emision, DateTime fch_vencimiento, string comprobante){
        Connect();
        cmd.Parameters.Clear();
        bool check = false;
        var aux1 = fch_emision.ToString("yyyy-MM-dd");
        var aux2 = fch_vencimiento.ToString("yyyy-MM-dd");
        string query = $"INSERT INTO Carnet_Salud (Ci, Fch_Emision, Fch_Vencimiento, Comprobante) VALUES (@ci, @fch_emision, @fch_vencimiento, @comprobante)";
        cmd.Connection = connection;
        cmd.CommandText = query;
        cmd.Parameters.AddWithValue("@ci", ci);
        cmd.Parameters.AddWithValue("@fch_emision", aux1);
        cmd.Parameters.AddWithValue("@fch_vencimiento", aux2);
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
  
    public bool CheckIfValidDate()
    {
    bool validDate = false;

    Connect();

    DateTime currentDate = DateTime.Now;
    int YEAR = currentDate.Year;
    int MONTH = currentDate.Month;
    int SEMESTER;

    if (MONTH >= 1 && MONTH <= 6) {
        SEMESTER = 1;
    } else {
        SEMESTER = 2;
    }

    string query = $"SELECT * FROM Periodos_Actualizacion WHERE Fch_Inicio <= @date AND Fch_Fin >= @date AND Anio = @year AND Semestre = @semester;";

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

    Disconnect();
    return validDate;
    }

    public DateTime? GetLastValidDate(){
        Connect();

        DateTime currentDate = DateTime.Now;
        int YEAR = currentDate.Year;

        string query = $"SELECT * FROM Periodos_Actualizacion WHERE Anio = @year ORDER BY Fch_Fin DESC LIMIT 1;";
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


        Disconnect();
        return lastValidDate;
    }

    public bool CheckIfValidAdmin(int ci, string password)
    {
        bool validAdmin = false;

        Connect();

        string query = @"SELECT EXISTS (
            SELECT 1
            FROM UsuarioRol ur
            JOIN Funcionarios f ON ur.Ci_Funcionario = f.Ci
            JOIN Rol r ON ur.Id_Rol = r.Id
            JOIN Logins l ON f.LogId = l.LogId
            WHERE f.CI = @CI AND l.Password = @Password AND r.Rol = @AdminRole
        ) AS isAdmin;";

        cmd.Connection = connection;
        cmd.CommandText = query;
        cmd.Parameters.Clear();
        cmd.Parameters.AddWithValue("@AdminRole", "Admin");
        cmd.Parameters.AddWithValue("@Password", password);
        cmd.Parameters.AddWithValue("@Ci", ci);
        cmd.Prepare();
        cmd.ExecuteNonQuery();

        reader = cmd.ExecuteReader();

        if (reader.HasRows) {
            reader.Read();
            validAdmin = reader.GetBoolean(0);
        }

        Disconnect();
        return validAdmin;
   }

   public bool AddNewDate(DateTime start, DateTime end){
        Connect();


        string query = $"INSERT INTO Periodos_Actualizacion (Anio, Semestre, Fch_Inicio, Fch_Fin) VALUES (@year, @semester, @start, @end);";
        cmd.Connection = connection;
        cmd.CommandText = query;
        cmd.Parameters.Clear();
        cmd.Parameters.AddWithValue("@year", start.Year);
        cmd.Parameters.AddWithValue("@semester", start.Month <= 6 ? 1 : 2);
        cmd.Parameters.AddWithValue("@start", start);
        cmd.Parameters.AddWithValue("@end", end);
        cmd.Prepare();

        if(cmd.ExecuteNonQuery() > 0){
            Disconnect();
            return true;
        }else{
            Disconnect();
            return false;
        }
   }

   public bool ChangeCurrentDate(DateTime start, DateTime newStart, DateTime end, DateTime newEnd){
        Connect();

        string query = $"UPDATE Periodos_Actualizacion SET Fch_Inicio = @newStart, Fch_Fin = @newEnd WHERE Fch_Inicio = @start AND Fch_Fin = @end;";
        cmd.Connection = connection;
        cmd.CommandText = query;
        cmd.Parameters.Clear();
        cmd.Parameters.AddWithValue("@start", start.ToString("yyyy-MM-dd"));
        cmd.Parameters.AddWithValue("@end", end.ToString("yyyy-MM-dd"));
        cmd.Parameters.AddWithValue("@newStart", newStart.ToString("yyyy-MM-dd"));
        cmd.Parameters.AddWithValue("@newEnd", newEnd.ToString("yyyy-MM-dd"));

        //print query
        cmd.Prepare();

        if(cmd.ExecuteNonQuery() > 0){
            Disconnect();
            return true;
        }else{
            Disconnect();
            return false;
        }
   }

   public DateTime GetCurrentStartDate(){
         Connect();

        DateTime currentDate = DateTime.Now;
        int YEAR = currentDate.Year;

        string query = $"SELECT * FROM Periodos_Actualizacion WHERE Anio = @year ORDER BY Fch_Fin DESC LIMIT 1;";
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
            lastValidDate = reader.GetDateTime(2);
            Disconnect();
            return (DateTime)lastValidDate;
        }else{
            Disconnect();
            return DateTime.Now;
        }
   }

   public DateTime GetCurrentEndDate(){
         Connect();

        DateTime currentDate = DateTime.Now;
        int YEAR = currentDate.Year;

        string query = $"SELECT * FROM Periodos_Actualizacion WHERE Anio = @year ORDER BY Fch_Fin DESC LIMIT 1;";
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
            Disconnect();
            return (DateTime)lastValidDate;
        }else{
            Disconnect();
            return DateTime.Now;
        }
   }

    public bool UserRegisterTransaction(int ci, string name, string lastName, DateTime birthDate, string adress, int telephone, string email, string password)
    {
        bool transactionCheck = false;
        try
        {
            Connect();
            cmd.Connection = connection;
            cmd.Parameters.Clear();
            cmd.Transaction = cmd.Connection.BeginTransaction();
            string hashedPassword = MD5Hash.Hash.Content(password);
            bool transaction1 = RegisterUser(hashedPassword);
            cmd.CommandText = "SELECT MAX(LogId) FROM Logins;";
            string logID = cmd.ExecuteScalar().ToString();
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
    public bool InsertWorker(int ci, string name, string lastName, DateTime birthDate, string adress, int telephone, string email, int logID)
    {

        cmd.Parameters.Clear();
        cmd.Connection = connection;
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
        return check;
    }
    public bool RegisterUser(string hashedPassword)
    {
        cmd.Parameters.Clear();
        bool check = false;
        string query = $"INSERT INTO Logins (LogId, Password) SELECT MAX(LogId) + 1, @password FROM Logins;";
        cmd.Connection = connection;
        cmd.CommandText = query;
        cmd.Parameters.AddWithValue("@password", hashedPassword);
        cmd.Prepare();  
        int result = cmd.ExecuteNonQuery();
        if (result > 0)
        {
            check = true;
        }
        return check;
    }


    public string[] GenerateReport(){
        Connect();
        cmd.Connection = connection;
        cmd.CommandText = "SELECT * FROM Funcionarios f LEFT JOIN Carnet_Salud c ON f.Ci = c.Ci WHERE (c.Ci IS NULL OR c.Fch_Vencimiento < @date) AND f.Ci != 11111111;";        cmd.Parameters.Clear();
        cmd.Parameters.AddWithValue("@date", DateTime.Now);
        cmd.Prepare();
        cmd.ExecuteNonQuery();
        reader = cmd.ExecuteReader();
        
        string[] report = new string[reader.FieldCount];
        int i = 0;

        while(reader.Read()){
            report[i] = reader.GetString(0) + " - " + reader.GetString(1) + " - " + " Carnet no vigente o no actualizado";
            i++;
        }

        Disconnect();

        return report;

    }

    public bool AddNewAgendaDate(DateTime day){
        bool check = false;

        Console.WriteLine("Agregando nueva agenda");

        Connect();

        string query = $"INSERT INTO Agenda (Fch_Agenda) SELECT @day WHERE NOT EXISTS (SELECT * FROM Agenda WHERE Fch_Agenda = @day);";
        cmd.Connection = connection;
        cmd.CommandText = query;
        cmd.Parameters.Clear();
        cmd.Parameters.AddWithValue("@day", day.ToString("yyyy-MM-dd"));
        cmd.Prepare();
        
        if(cmd.ExecuteNonQuery() > 0){
            check = true;
        }

        Disconnect();

        if(check){
            Console.WriteLine("Nueva agenda agregada");
        }else{
            Console.WriteLine("No se pudo agregar la nueva agenda");
        }

        return check;
    }

    public string[] getAviableAgendas()
    {
        Connect();
        cmd.Connection = connection;
        cmd.CommandText = "SELECT * FROM Agenda WHERE ci IS NULL;";
        cmd.Prepare();
        cmd.ExecuteNonQuery();
        reader = cmd.ExecuteReader();
    
        string[] agendas = new string[reader.FieldCount];
        int i = 0;
    
        while(reader.Read()){
            if(reader.GetString(2) != null && reader.GetString(2) != ""){
                string date = reader.GetString(2).Split(" ")[0];
                if(date != null && date != ""){
                    agendas[i] = date;
                    i++;
                }
            }
        }
    
        Disconnect();
    
        return agendas;
    }
}