## Assignment Description

Create a fully responsive CRUD web application using **Angular 20** for the front-end and **.NET 9 C#** for the back-end API.  
The application must support **user authentication with JWT**, use **Bootstrap** for layout and styling, include **Font Awesome** icons, and provide a separate **"My Quotes"** view where users can manage their favorite quotes.

---

## Requirements

### CRUD Functionality (Books)
- Implement a page that displays a list of all books.
- Create a start page with a button to add a new book.
- When the "Add New Book" button is clicked, navigate the user to a form where they can enter book details such as title, author, and publication date.
- After submitting the form, redirect the user back to the start page where the newly added book appears in the list.
- Each book should have an **Edit** button that opens a form for updating the book details.
- After submitting the edit form, redirect back to the start page and update the displayed book information.
- Each book should have a **Delete** button that removes the book from the list.
- After deleting a book, the updated list should reflect the removal.

---

## Token Handling (Authentication)
- Implement user authentication using **JWT (JSON Web Tokens)**.
- Provide a simple login page where users enter a username and password.
- Users must be able to register a new account and then use it to log in.
- After successful login, the back-end should generate a token and return it to the front-end.
- The front-end should securely store the token (e.g., local storage or a cookie) and use it for subsequent API requests.
- Implement token validation in the back-end so that only authenticated users can perform CRUD operations.

---

## “My Quotes” Page
- Implement a separate view called **"My Quotes"**.
- Display a list of five favorite quotes.
- Allow users to add, edit, and delete quotes.
- Provide a navigation menu to switch between the books view and the quotes view.

---

## Responsive Design Requirements
- Ensure that the layout and components adapt smoothly to different screen sizes, including desktop, tablet, and mobile.
- Test the application by resizing the browser window to verify correct layout adjustments.
- Ensure navigation menus collapse into a mobile-friendly menu on smaller screens.
- Verify that form fields, buttons, and UI elements maintain correct spacing and alignment across viewports.
- Test the application on multiple device types (e.g., smartphones, tablets) and browsers for consistent behavior.

---

## Bootstrap and Font Awesome
- Use **Bootstrap** to create a responsive, visually appealing layout.
- Use Bootstrap classes when designing buttons, forms, and other UI components.
- Include **Font Awesome** icons to enhance the user interface.
- Verify that Font Awesome icons render correctly and are used appropriately throughout the application.

---

## Additional Challenge (Optional)
- Implement a toggle button allowing the user to switch between **light** and **dark** mode themes.

---

## Submission
Host the application on a free hosting service such as **Netlify**, **Vercel**, or **Azure Static Web Apps**.  
Submit the link to the published application and provide the GitHub repository link.
