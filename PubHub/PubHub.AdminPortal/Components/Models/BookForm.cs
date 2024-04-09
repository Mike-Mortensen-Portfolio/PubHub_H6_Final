using System.ComponentModel.DataAnnotations;
using PubHub.Common.Models.Books;

namespace PubHub.AdminPortal.Components.Models
{
    public class BookForm
    {
        [Required(ErrorMessage = "Please choose a content type.")]
        public Guid ContentTypeId { get; set; }

        [Required(ErrorMessage = "Please enter a Publisher's name.")]
        public Guid PublisherId { get; set; }

        [Required(ErrorMessage = "Please enter a title for the book.")]
        [RegularExpression(@"^[^\\/:*;\.\)\(]+$", ErrorMessage = "The characters ':', '.' ';', '*', '/' and '\' are not allowed")]
        public string? Title { get; set; }

        [Required(ErrorMessage = "Please upload a cover image.")]
        public byte[]? CoverImage { get; set; }

        [Required(ErrorMessage = "Please upload content of the book.")]
        public byte[]? BookContent { get; set; }

        [Required(ErrorMessage = "Please select a date.")]
        public DateOnly PublicationDate { get; set; }

        /// <summary>
        /// Represents the length of <see cref="BookContent"/>
        /// <br/>
        /// <br/>
        /// <strong>Note:</strong> This would represent pages if <see cref="BookContent"/> equates to <see cref="ContentTypeConstants.E_BOOK_CONTENT_TYPE"/>
        /// <br/>
        /// and a time span if <see cref="ContentTypeConstants.AUDIO_CONTENT_TYPE"/>
        /// </summary>
        [Required(ErrorMessage = "Please give the length of the book, page total or seconds for audio book.")]
        [RegularExpression(@"^[^\\/:*;\.\)\(]+$", ErrorMessage = "The characters ':', '.' ';', '*', '/' and '\' are not allowed")]
        public double Length { get; set; }

        public bool IsHidden { get; set; } = false;

        [Required(ErrorMessage = "Please add at least one author.")]
        [RegularExpression(@"^[^\\/:*;\.\)\(]+$", ErrorMessage = "The characters ':', '.' ';', '*', '/' and '\' are not allowed")]
        public List<Guid>? Authors { get; set; }

        [Required(ErrorMessage = "Please select at least one genre.")]
        public List<Guid>? Genres { get; set; }

        public BookCreateModel CreateBookModel()
        {

            return new BookCreateModel()
            {
                Title = Title ?? string.Empty,
                Length = Length,
                PublicationDate = PublicationDate,
                BookContent = BookContent ?? [],
                PublisherId = PublisherId,
                ContentTypeId = ContentTypeId,
                CoverImage = CoverImage,
                AuthorIds = [.. Authors],
                GenreIds = [.. Genres],
                IsHidden = IsHidden
            };

        }

        public BookUpdateModel UpdateBookModel()
        {
            return new BookUpdateModel()
            {
                Title = Title ?? string.Empty,
                Length = Length,
                PublicationDate = PublicationDate,
                BookContent = BookContent ?? [],
                PublisherId = PublisherId,
                ContentTypeId =ContentTypeId,
                CoverImage = CoverImage,
                AuthorIds = [.. Authors],
                GenreIds = [.. Genres],
                IsHidden = IsHidden
            };
        }
    }
}
