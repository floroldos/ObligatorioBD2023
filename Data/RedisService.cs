using System;
using NRedisStack;
using NRedisStack.RedisStackCommands;
using StackExchange.Redis;

public class RedisService
{
    ConnectionMultiplexer redis;

    public RedisService() 
    {
        redis = ConnectionMultiplexer.Connect("localhost:6379");
        IDatabase db = redis.GetDatabase();
    }

//mira, en el docker puse para q se cree la base de datos redis, dale docker-compose up, despues entra en localhost:8013 y 
//ahi podes ver la base de datos y podes ver los valores que se van guardando, y podes hacer consultas con la consola, bueno esta clase es para hacer las consultas

    public void SetValue(string key, string value) // esto agarra el key y el value y lo guarda
    {
        IDatabase db = redis.GetDatabase();
        db.StringSet(key, value);
    }

    //Cuando se registra el funcionario, se guarda en cache
    public void setListElement(string key, string value)
    //cuando se hace el checkuser, antes de hacer la consulta SQL tiene que venir aca y preguntar si el get devuelve algo, si no, hace SQL
    {
        IDatabase db = redis.GetDatabase();
        db.ListRightPush(key, value);
    }

    public void DeleteKey(string key)
    {
        IDatabase db = redis.GetDatabase();
        db.KeyDelete(key);
    } 
//Y como iria eso en el dbservice voy
    public string Get(string key) // aca esta el get, me queda poner en DataBaseService que haga esto antes siiiiiii, la documentacion oficial esta buena, te paso
    // https://redis.io/commands/
    // https://redis.io/docs/connect/clients/dotnet/
    {
        IDatabase db = redis.GetDatabase();
        var valor = db.StringGet(key);

        if(valor.HasValue){
            Console.WriteLine("Valor: " + valor.ToString());       
             
        }
        else{
            Console.WriteLine("Clave no encontrada");
        }
        return valor;
    }

    public RedisValue[] getList(string key)
    {
        IDatabase db = redis.GetDatabase();
        var lista = db.ListRange(key);

        if(lista != null){
            Console.WriteLine("Valores en la lista: ");

            foreach (RedisValue value in lista)
            {
                Console.WriteLine(value);
            }
             
        }
        else{
            Console.WriteLine("Lista no encontrada");
        }
        
        return lista;
    }

   
}