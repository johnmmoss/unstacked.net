import React from "react";
import { useEffect } from "react";
import { Book } from "./book";
import { useBookService } from "./book-service";
import './books.css'

export const BookList = () => {

   const [books, setBooks] = React.useState<Book[] | null>(null);
   const { getBooks } = useBookService();

   useEffect(() => {
      getBooks().then((books) => {
         setBooks(books);
      });
   }, []);

   return (

      <div className="content">
         {books == null ?
            <p>Books loading... </p>
            :
            <table>
               <thead>
                  <tr>
                     <th>ID</th>
                     <th>Title</th>
                     <th>Author</th>
                     <th>Published</th>
                  </tr>
               </thead>
               <tbody>
                  {books.map((row) => (
                     <tr key={row.id}>
                        <td>{row.id}</td>
                        <td>{row.title}</td>
                        <td>{row.author}</td>
                        <td>{row.publishedDate.toString()}</td>
                     </tr>
                  ))}
               </tbody>
            </table>
         }
      </div>
   );
}