export const environment = {
  production: true,
  apiUrls: {
    books: 'http://localhost:8080/api/books',
    editors: 'http://localhost:8080/api/editors',
    authors: 'http://localhost:8080/api/authors',
    users: 'http://localhost:8080/api/users',
    auth: 'http://localhost:8080/api/auth'
  },
  apiPrefix: '/api',
  api: {
    books: {
      getBooks: '/getBooks',
      getById: '/{id}',
      getBookDetail: '/getBookDetail/{id}',
      addBook: '/addBook',
      update: '/{id}',
      delete: '/{id}'
    },
    editors: {
      getEditors: '/getEditors',
      getById: '/{id}',
      getEditorDetail: '/getEditorDetail/{id}',
      addEditor: '/addEditor',
      update: '/{id}',
      delete: '/{id}'
    },
    authors: {
      getAuthors: '/getAuthors',
      getById: '/{id}',
      getAuthorDetail: '/getAuthorDetail/{id}',
      addAuthor: '/addAuthor',
      update: '/{id}',
      delete: '/{id}'
    },           
    auth: {
      login: '/login'
    },    
    users: {
      addUser: '/addUser'
    }    
  }   
};
