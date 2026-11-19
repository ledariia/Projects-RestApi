using Domain;

namespace Tests.Data;

public static class BooksTestData
{
    public static Book FirstTestBook() => Book.Create(
        title: "Clean Code",
        author: "Robert Martin",
        genre: "Programming",
        pageCount: 250);

    public static Book SecondTestBook() => Book.Create(
        title: "Clean Architecture",
        author: "Robert Martin",
        genre: "Architecture",
        pageCount: 432);

    public static Book ThirdTestBook() => Book.Create(
        title: "Domain Driven Design",
        author: "Eric Evans",
        genre: "Architecture",
        pageCount: 560);

    public static Book FourthTestBook() => Book.Create(
        title: "Algorithms Unlocked",
        author: "Thomas Cormen",
        genre: "Algorithms",
        pageCount: 800);
}