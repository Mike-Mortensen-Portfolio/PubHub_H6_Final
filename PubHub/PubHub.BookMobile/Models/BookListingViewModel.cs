using System.Collections.ObjectModel;
using PubHub.Common.Models.Books;
using PubHub.Common.Models.Genres;

namespace PubHub.BookMobile.Models
{
    public class BookListingViewModel
    {
        public BookInfoModel? EBook;
        public BookInfoModel? AudioBook;
        public ImageSource ListingCover => GetCoverImage();
        public string Title { get; set; } = null!;
        public DateOnly PublicationDate { get; set; }
        public bool EBookInStock => EBook != null;
        public bool AudiobookInStock => AudioBook != null;
        public ObservableCollection<GenreInfoModel> Genres => GetGenres();

        /// <summary>
        /// Sets the cover image if one is present in <see cref="EBook"/>; otherwise, if not, it will try <see cref="AudioBook"/>. If no cover image could be found in either, a stock photo will be used.
        /// </summary>
        /// <returns></returns>
        private ImageSource GetCoverImage()
        {
            var coverImage = ((EBookInStock) ? (EBook!.CoverImage) : (AudioBook!.CoverImage));

            if (coverImage is null)
                return ImageSource.FromFile("stock_image.jpg");

            return ImageSource.FromStream(() => new MemoryStream(coverImage));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>A new <see cref="ObservableCollection{T}"/> of type <see cref="GenreInfoModel"/></returns>
        /// <exception cref="NullReferenceException"></exception>
        private ObservableCollection<GenreInfoModel> GetGenres()
        {
            var genres = ((EBookInStock) ? (EBook?.Genres) : (AudioBook?.Genres)) ?? throw new NullReferenceException("Genres was null. That's not supposed to happen");

            return new ObservableCollection<GenreInfoModel>(genres);
        }
    }
}
