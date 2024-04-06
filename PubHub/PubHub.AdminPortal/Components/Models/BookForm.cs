﻿using System.ComponentModel.DataAnnotations;
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
        public List<Guid> AuthorIds { get; set; } = [];
        public List<Guid> GenreIds { get; set; } = new();

        // TODO (JBN): Remove the test hard-coded values.
        public BookCreateModel CreateBookModel()
        {
            return new BookCreateModel()
            {
                Title = Title,
                Length = Length,
                PublicationDate = PublicationDate,
                BookContent = BookContent,
                PublisherId = PublisherId,
                ContentTypeId = ContentTypeId,
                CoverImage = CoverImage,
                AuthorIds = AuthorIds.ToArray(),
                GenreIds = GenreIds.ToArray(),
                IsHidden = IsHidden
            };
        }

        public BookUpdateModel UpdateBookModel()
        {
            return new BookUpdateModel()
            {
                Title = Title,
                Length = Length,
                PublicationDate = PublicationDate,
                BookContent = BookContent,
                PublisherId = TestData.PUBLISHER_ID,
                ContentTypeId =ContentTypeId,
                CoverImage = CoverImage,
                AuthorIds = AuthorIds.ToArray(),
                GenreIds = GenreIds.ToArray(),
                IsHidden = IsHidden
            };
        }
    }
}