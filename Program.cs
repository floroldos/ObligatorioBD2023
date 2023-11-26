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
        Console.WriteLine("Email thread started...");
        while (true)
        {
            Console.WriteLine("Sending emails...");

            //db.sendEmails();

            //AQUI SE DEBERIAN DE AGARRAR LOS MAILS QUE NO ENVIARON EL CARNE DE SALUD Y MANDARLE UN EMAIL
            //GENERAR Y MANDAR EMAILS

            Console.WriteLine("Emails sent!");
            Console.WriteLine("Waiting 24 hours to send emails again...");
            Thread.Sleep(24 * 60 * 60 * 1000);
        }
    }
}