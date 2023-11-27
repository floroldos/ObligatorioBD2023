using System.Threading;
using Blazored.LocalStorage;


class Program
{
    static Thread? webServerThread;
    static Thread? emailThread;
    static DatabaseService? db;

    static void Main(string[] args)
    {
        webServerThread = new Thread(() => taskWebServer(args));
        webServerThread.Start();
    }

    static void taskWebServer(string[] args)
    {
        Console.WriteLine("Web thread started...");
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddRazorPages();
        builder.Services.AddServerSideBlazor();
        builder.Services.AddSingleton<DatabaseService>();
        builder.Services.AddBlazoredLocalStorage();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
        }


        app.UseStaticFiles();

        app.UseRouting();

        app.MapBlazorHub();
        app.MapFallbackToPage("/_Host");

        db = DatabaseService.GetInstance();
        bool conectado = db.CheckConnection();
        if(conectado){
            emailThread = new Thread(() => taskEmailWaiter());
            emailThread.Start();
        }
        else{
            Console.WriteLine("Error connecting to database");
            Environment.Exit(0);
            return;
        }

        app.Run();
    }

    static void taskEmailWaiter()
    {
        Console.WriteLine("Report thread started...");
        while (true)
        {
            Console.WriteLine("Generating report...");

            string[] report = DatabaseService.GetInstance().GenerateReport();

            string path = "report.log";

            using(var writer = new StreamWriter(path, true)) {
                writer.WriteLine("========================= Reporte de carnet de salud =========================");
                writer.WriteLine("----------------------------- Fecha: " + DateTime.Now.ToString("dd/MM/yyyy") + " ------------------------------");
                foreach (string line in report) {
                    if (line != "" && line != null && line != " " && line != "\n") {
                        writer.WriteLine(line);
                    }
                }
                writer.WriteLine("=============================== Fin de reporte ===============================");
                writer.WriteLine("");
                writer.Close();
            }

            report = new string[0];


            Console.WriteLine("Report generated!");
            Console.WriteLine("Waiting 24 hours to generate another report...");
            Thread.Sleep(24 * 60 * 60 * 1000);
        }
    }
}