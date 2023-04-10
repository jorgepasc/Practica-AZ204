using System.Threading.Tasks; 
using Azure.Messaging.ServiceBus;

public static class Service
{


static ServiceBusSender sender;
static ServiceBusClient client;
// connection string to your Service Bus namespace
const string connectionString = "Endpoint=sb://az204svcbus16419.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=pf9BlnhG0ysSWmoAH96al/vqwTFdn96Z7+ASbHbYfJM=";
// name of your Service Bus topic
const string queueName = "az204-queue";
private const int numOfMessages = 3;

// the processor that reads and processes messages from the queue
static ServiceBusProcessor processor;


    public async static Task<bool> TestServiceBus()
    {
        // Create the clients that we'll use for sending and processing messages.
        client = new ServiceBusClient(connectionString);
        sender = client.CreateSender(queueName);
        // create a batch 
        using ServiceBusMessageBatch messageBatch = await sender.CreateMessageBatchAsync();
        for (int i = 1; i <= 3; i++)
        {
            // try adding a message to the batch
            if (!messageBatch.TryAddMessage(new ServiceBusMessage($"Message {i}")))
            {
            // if an exception occurs
                throw new Exception($"Exception {i} has occurred.");
            }
        }
            
        try 
        {
            // Use the producer client to send the batch of messages to the Service Bus queue
            await sender.SendMessagesAsync(messageBatch);
            Console.WriteLine($"A batch of {numOfMessages} messages has been published to the queue.");
        }
        finally
        {
            // Calling DisposeAsync on client types is required to ensure that network
            // resources and other unmanaged objects are properly cleaned up.
            await sender.DisposeAsync();
            await client.DisposeAsync();
            
        }
        return false;
    }

    public async static void TestServiceBusProcessMessage()
    {
        // Create the client object that will be used to create sender and receiver objects
        client = new ServiceBusClient(connectionString);
        // create a processor that we can use to process the messages
        processor = client.CreateProcessor(queueName, new ServiceBusProcessorOptions());
        try
        {
            // add handler to process messages
            processor.ProcessMessageAsync += MessageHandler;
            // add handler to process any errors
            processor.ProcessErrorAsync += ErrorHandler;
            // start processing 
            await processor.StartProcessingAsync();
            Console.WriteLine("Wait for a minute and then press any key to end the processing");
            Console.ReadKey();
            // stop processing 
            Console.WriteLine("\nStopping the receiver...");
            await processor.StopProcessingAsync();
            Console.WriteLine("Stopped receiving messages");
        }
        finally
        {
            // Calling DisposeAsync on client types is required to ensure that network
            // resources and other unmanaged objects are properly cleaned up.
            await processor.DisposeAsync();
            await client.DisposeAsync();
        }
    }

    // handle received messages
    static async Task MessageHandler(ProcessMessageEventArgs args)
    {
        string body = args.Message.Body.ToString();
        Console.WriteLine($"Received: {body}");
        // complete the message. messages is deleted from the queue. 
        await args.CompleteMessageAsync(args.Message);
    }
    // handle any errors when receiving messages
    static Task ErrorHandler(ProcessErrorEventArgs args)
    {
        Console.WriteLine(args.Exception.ToString());
        return Task.CompletedTask;
    }

}