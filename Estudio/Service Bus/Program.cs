using System.Threading.Tasks; 

Console.WriteLine("TEST");
await Service.TestServiceBus();

Console.WriteLine("Press any key to end the application");
Console.ReadKey();