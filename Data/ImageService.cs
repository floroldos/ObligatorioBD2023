using Microsoft.AspNetCore.Http;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

public interface IImageService
{
    Task<IEnumerable<ImageModel>> GetImagesAsync();
    Task UploadImageAsync(ImageModel model, IFormFile file);
}

public class ImageService : IImageService
{
    private readonly string _connectionString;

    public ImageService(string connectionString)
    {
        _connectionString = connectionString;
    }

    public Task<IEnumerable<ImageModel>> GetImagesAsync()
    {
        throw new NotImplementedException();
    }

    public async Task UploadImageAsync(ImageModel model, IFormFile file)
    {
        if (file != null && file.Length > 0)
        {
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                model.ImageData = memoryStream.ToArray();
            }

            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new MySqlCommand(
                    "INSERT INTO Images (Name, Description, ImageData) VALUES (@Name, @Description, @ImageData)", connection))
                {
                    command.Parameters.AddWithValue("@Name", model.Name);
                    command.Parameters.AddWithValue("@Description", model.Description);
                    command.Parameters.AddWithValue("@ImageData", model.ImageData);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
