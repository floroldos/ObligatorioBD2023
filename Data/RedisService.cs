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

   public void SetValue(string key, string value) // O(1)
    {
        IDatabase db = redis.GetDatabase();
        db.StringSet(key, value);
    }

    public void setListElement(string key, string value) // O(1)
    {
        IDatabase db = redis.GetDatabase();
        db.ListRightPush(key, value);
    }

    public void DeleteKey(string key) // O(1)
    {
        IDatabase db = redis.GetDatabase();
        db.KeyDelete(key);
    } 
    public string Get(string key) // O(1)
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

    public RedisValue[] getList(string key) // O(1)
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