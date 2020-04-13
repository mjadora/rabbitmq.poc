using System;
using RabbitMQ.Client;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using POC.Model;
using System.Collections.Generic;

class Send
{
    public static void Main()
    {
        Console.WriteLine(" Press [Q] to exit.");

        var clusteredHosts = new List<string>() { "localhost", "server1", "server2" };
        var factory = new ConnectionFactory();

        using (var connection = factory.CreateConnection(clusteredHosts))
        using (var channel = connection.CreateModel())
        {
            channel.QueueDeclare(queue: "worker.lbt", durable: true, exclusive: false, autoDelete: false, arguments: null);

            while (Console.ReadLine() != "Q")
            {
                var request = new PaymentRequest { Id = Guid.NewGuid(), Timestamp = DateTime.Now, User = new User { PaymentAccountId = "101", Name = "Mark" } };
                byte[] serializedRequest = ObjectToByteArray(request);

                channel.BasicPublish(exchange: "", routingKey: "worker.lbt", basicProperties: null, body: serializedRequest);
                Console.WriteLine(" [x] Sent request: {0}", request.Id);
            }
        }
        
        Console.ReadLine();
    }

    // Convert an object to a byte array
    public static byte[] ObjectToByteArray(object obj)
    {
        BinaryFormatter bf = new BinaryFormatter();
        using (var ms = new MemoryStream())
        {
            bf.Serialize(ms, obj);
            return ms.ToArray();
        }
    }
}