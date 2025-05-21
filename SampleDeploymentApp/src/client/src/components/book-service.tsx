import { getApiUrl } from "../configuration";
import { Book } from "./book";

export const useBookService = () => {
   const getBooks = async (): Promise<Book[] | null> => {
      var response = await fetch(getApiUrl("books"), {
         method: "GET",
      });

      let books = (await response.json()) as Book[];
      console.log(books);
      return books;
   }

   return { getBooks };
}