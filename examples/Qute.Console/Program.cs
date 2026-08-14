using System.Text.Json;
using DotNetEnv;
using Qute.Directus;
using System.IO;

Env.Load(Path.Combine(AppContext.BaseDirectory, ".env"));

var baseUrl = Environment.GetEnvironmentVariable("DIRECTUS_URL") ?? throw new Exception("DIRECTUS_URL not found");
var token = Environment.GetEnvironmentVariable("DIRECTUS_TOKEN") ?? throw new Exception("DIRECTUS_TOKEN not found");

var client = new DirectusClient(new DirectusOptions
{
    BaseUrl = baseUrl,
    StaticToken = token
});

Console.WriteLine("Buscando municípios...\n");

try
{
    var user = await client.Users.GetCurrentAsync();
    if (user != null)
    {
        Console.WriteLine($"Seja bem vindo {user.FirstName} {user.LastName} ({user.Email})\n");
    }

    // Busca o município específico por ID e exibe a propriedade "nome"
    var municipio = await client.Items.GetByIdAsync("municipios", "1100205");
    if (municipio.ValueKind != JsonValueKind.Undefined && municipio.ValueKind != JsonValueKind.Null)
    {
        if (municipio.TryGetProperty("nome", out var nomeProp) && nomeProp.ValueKind == JsonValueKind.String)
        {
            Console.WriteLine($"Município (ID 1100205): {nomeProp.GetString()}");
        }
        else
        {
            Console.WriteLine("Propriedade 'nome' não encontrada no objeto do município.");
            Console.WriteLine(municipio.ToString());
        }
    }
    else
    {
        Console.WriteLine("Município não encontrado.");
    }

    var response = await client.Items.GetManyAsync<JsonElement>("municipios", q => q
        .Limit(10)
        .Offset(0)
        .Meta("*"));

    Console.WriteLine($"Total de municípios encontrados: {response.Meta?.TotalCount ?? response.Data.Count}");
    Console.WriteLine(new string('─', 60));

    foreach (var item in response.Data)
    {
        Console.WriteLine(item.ToString());
    }

    Console.WriteLine(new string('─', 60));
    Console.WriteLine($"\nExibidos: {response.Data.Count} municípios");
}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"Erro: {ex.Message}");
    Console.ResetColor();
}
