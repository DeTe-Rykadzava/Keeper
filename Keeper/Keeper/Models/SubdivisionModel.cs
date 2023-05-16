using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;



namespace Keeper.Models;

public class SubdivisionModel
{
    public string subdivisionName { get; set; }

    public string employeeName { get; set; }

    public static async Task<IEnumerable<SubdivisionModel>?> GetSubdivisions(HttpClient client)
    {
        var result = await client.GetAsync("https://localhost:7125/api/Subdivision");

        if (!result.IsSuccessStatusCode)
            return null;

        try
        {
            var jsonString = await result.Content.ReadAsStringAsync();

            var subdivisionsToReturn = JsonSerializer.Deserialize<SubdivisionModel[]>(jsonString);
            return subdivisionsToReturn;
        }
        catch (System.Exception ex)
        {
            System.Console.WriteLine(ex.Message + "\n\n\n\n" + ex.InnerException);
            throw new JsonException("Не удалось преобразовать исходные данные");
        }

    }
}
