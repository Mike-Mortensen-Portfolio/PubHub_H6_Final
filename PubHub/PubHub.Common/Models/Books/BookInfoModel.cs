﻿namespace PubHub.Common.Models.Books
{
    public class BookInfoModel
    {
        public int Id { get; init; }
        public required PublisherInfoModel Publisher { get; init; } = null!;
        public required string Title { get; set; }
        public byte[]? CoverImage { get; set; }
        public required ContentTypeInfoModel ContentType { get; init; }
        public DateOnly PublicationDate { get; set; }
        public double Length { get; init; }
        public IList<GenreInfoModel> Genres { get; set; } = [];
        public IList<AuthorInfoModel> Authors { get; set; } = [];
    }
}