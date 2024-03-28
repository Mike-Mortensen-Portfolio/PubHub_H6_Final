![PubHub](https://github.com/Mike-Mortensen-Portfolio/PubHub_H6_Final/assets/61870713/452eeb83-5649-4c6f-91c4-944f0e14cc77)

### PubHub Distribution Service

---

# Introduction
This project represents the final exam for the Data Technician with Speciality in Programming education.

PubHub is an audio- and e-book distribution service, which will stores many publishers products; offering them statistics and the ability to always administrate their products through an `Admin Portal`. PubHub also offers their own solution for consumers to access the publishers products through both a mobile application (`Book App`) and a web application (`Book Web`). Here the consumers can draw subscription and add books to their personal library as they wish; however if the subscription ends, the books will be made unavailable for the consumer. They can also buy books and be an owner of that book for as long as their Account isn't deleted by themselves.

PubHub also offers the service of Second-party stores and markets to makes a partner agreement with them, gaining access to the huge storage of audio- and e-books.

---

# Tables of contents

# Projects
| **Project** | **Platform**            | **Languages**     | **Timeframe** | **Backend Store** |
|-------------|-------------------------|-------------------|---------------|-------------------|
| `App`       | .NET MAUI               | C#, XAML          |               | `Api`             |
| `Api`       | .NET RESTApi            | C#                |               | `MS SQL DB`       |
| `Web`       | .NET Blazor Server      | C#, JS, HTML, CSS |               | `Api`             |
| `Web`       | .NET Blazor WebAssembly | C#, JS, HTML, CSS |               | `Api`             |

---

# Setup

---

#PubHub Architecture

![image](https://github.com/Mike-Mortensen-Portfolio/PubHub_H6_Final/assets/61870713/789d6775-c4a0-40fb-ad80-d6d9c465cace)

The diagram above demostrates the communication between the different sub-systems in the PubHub system. If we begin from the top-left side, we have 3 different client-side applications; `.NET MAUI: Book App`, `.NET Blazor WebAssembly: Book Web` and a `.NET Blazor Server: Admin Portal`. We can see that they all 3 will use the `.NET REST API` to communicate through to retrieve and send data values. 

At the top we can see two external systems, who also will be communicating with the `.NET REST API`. We have the `Payment Provider` who will be handling the transactions made on `Book App` and `Book Web` applications. Then there's the `Library Provider` which handles librarian users to become a part of the system, as well as providing data to PubHub with how many copies they have of their books, ready for borrowing. Lastly there's the `Second-Party Providers` which respresents any other stores or makets much like our `Book App` and `Book Web` who can make a partner agreement to gain access to PubHub's storage of books, on the conditions of giving statistics about those books in return.

Lastly, is the `MS SQL Server` which is an on-site server but, with backups to the cloud and to other servers. All Book data and account data is stored here.

---

# API Endpoints

---

# Standards
- Our `Constants` follows the Screaming Snake Casing: THIS_IS_SCREAMING_SNAKE_CASING.
- Branches will be named as followed: `[Initials]/[feature]` in all lowercase, with words seperated by dash.
  - Example would be: **msm/my-feature**
- We follow the **FBT**: Folder by type structure in our project.

