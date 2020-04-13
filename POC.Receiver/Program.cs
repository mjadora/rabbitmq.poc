using POC.Model;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

class Receive
{
    public static void Main()
    {
        var clusteredHosts = new List<string>() { "localhost", "server1", "server2" };
        var factory = new ConnectionFactory();

        using (var connection = factory.CreateConnection(clusteredHosts))
        using (var channel = connection.CreateModel())
        {
            channel.QueueDeclare(queue: "worker.lbt", durable: true, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;

                PaymentRequest request = (PaymentRequest) ByteArrayToObject(body);
                Console.WriteLine(" [x] Received {0}", request.Id);
            };
            channel.BasicConsume(queue: "worker.lbt", autoAck: true, consumer: consumer);

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }

    public static object ByteArrayToObject(byte[] arrBytes)
    {
        using (var memStream = new MemoryStream())
        {
            var binForm = new BinaryFormatter();
            memStream.Write(arrBytes, 0, arrBytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            var obj = binForm.Deserialize(memStream);
            return obj;
        }
    }
}
