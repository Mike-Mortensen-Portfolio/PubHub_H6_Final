﻿namespace PubHub.Common.Models.Books
{
    public class BookUpdateModel
    {
        public required int ContentTypeId { get; init; }
        public required int PublisherId { get; init; }
        public required string Title { get; set; }
        public byte[]? CoverImage { get; set; }
        public required byte[] BookContent { get; set; }
        public required DateOnly PublicationDate { get; init; }
        /// <summary>
        /// Represents the length of <see cref="BookContent"/>
        /// <br/>
        /// <br/>
        /// <strong>Note:</strong> This would represent pages if <see cref="BookContent"/> equates to <see cref="ContentTypeConstants.E_BOOK_CONTENT_TYPE"/>
        /// <br/>
        /// and a time span if <see cref="ContentTypeConstants.AUDIO_CONTENT_TYPE"/>
        /// </summary>
        public required double Length { get; set; }
        public bool IsHidden { get; set; }

        public int[] AuthorIds { get; set; } = [];
        public int[] GenreIds { get; set; } = [];
    }
}