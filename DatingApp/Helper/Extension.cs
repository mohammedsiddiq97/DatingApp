using API.Helpers;
using System.Data.SqlTypes;
using System.Text.Json;

namespace DatingApp.Helper
{
    public static class Extension
    {
        public static void AddAppilcationError(this HttpResponse response , string message)
        {
            response.Headers.Add("ApplicationError", message);
            response.Headers.Add("Access-Control-Expose-Headers","ApplicationError");
            response.Headers.Add("Access-Control-Allow-Origin", "*");
        }
        public static int CalculateAge(this DateTime dateTime)
        {
            var age = DateTime.Today.Year - dateTime.Year;
            if (dateTime.Date > DateTime.Today.AddYears(-age))
            {
                age--;
            }

            return age;
        }
        public static void AddPaginationHeader(this HttpResponse response, int currentPage, int itemsPerPage, int totalItems, int totalPages)
        {
            var paginationHeader = new PaginationHeader(currentPage, itemsPerPage, totalItems, totalPages);
            var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            response.Headers.Add("Pagination", JsonSerializer.Serialize(paginationHeader,jsonOptions));
            //using this not to get cors error
            response.Headers.Add("Access-Control-Expose-Headers", "Pagination");
        } 
    }
}
