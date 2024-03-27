function startCarousel(carouselId) {
	var myCarousel = document.getElementById(carouselId);
	var carousel = new bootstrap.Carousel(myCarousel);
	carousel.cycle();
}