using System.ComponentModel.DataAnnotations;
using PubHub.Common.Models.Books;

namespace PubHub.AdminPortal.Components.Models
{
    public class BookForm
    {
        // TODO (JBN): make this fit the new models being created for the Book.
        public Guid ContentTypeId { get; set; }
        [Required]
        public Guid PublisherId { get; set; }
        [Required]
        public string Title { get; set; }
        public byte[]? CoverImage { get; set; }
        [Required]
        public byte[] BookContent { get; set; }
        [Required]
        public DateOnly PublicationDate { get; set; }
        /// <summary>
        /// Represents the length of <see cref="BookContent"/>
        /// <br/>
        /// <br/>
        /// <strong>Note:</strong> This would represent pages if <see cref="BookContent"/> equates to <see cref="ContentTypeConstants.E_BOOK_CONTENT_TYPE"/>
        /// <br/>
        /// and a time span if <see cref="ContentTypeConstants.AUDIO_CONTENT_TYPE"/>
        /// </summary>
        [Required]
        public double Length { get; set; }
        public bool IsHidden { get; set; } = false;
        public Guid[] AuthorIds { get; set; } = [];
        public Guid[] GenreIds { get; set; } = [];

        // TODO (JBN): Remove the test hard-coded values.
        public BookCreateModel CreateBookModel()
        {
            return new BookCreateModel()
            {
                Title = Title,
                Length = Length,
                PublicationDate = new DateOnly(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day),
                BookContent = BookContent,
                PublisherId = new Guid("7bf64e8c-9a22-8299-8575-018e8ede1d91"),
                ContentTypeId = new Guid("484F3814-65DE-8400-8574-018E8EDE1D91"),
                CoverImage = CoverImage,
                AuthorIds = AuthorIds,
                GenreIds = GenreIds,
                IsHidden = IsHidden
            };
        }
    }
}
