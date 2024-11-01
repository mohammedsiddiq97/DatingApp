namespace DatingApp.Dtos
{
    public class PhotosForCreationDto
    {
        public PhotosForCreationDto() { DateAdded = DateTime.UtcNow; }
        public string? Url { get; set; }
        public IFormFile File { get; set; }
        public string? Description { get; set; }
        public DateTime? DateAdded { get; set; }
        public string? PublicId { get; set; }
         

    }
}
