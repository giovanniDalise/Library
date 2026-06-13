export const environment = {
  production: true,
  apiUrls: {
    books: 'http://api-gateway/api/books',
    editors: 'http://api-gateway/api/editors',
    authors: 'http://api-gateway/api/authors',    
    users: 'http://api-gateway/api/users',
    auth: 'http://api-gateway/api/auth'
  },
  apiPrefix: '/api',
  api: {
    books: {
      getBooks: '/getBooks',
      getById: '/{id}',
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
      addAuthor: '/addAuthor',
      update: '/{id}',
      delete: '/{id}'
    },           
    auth: {
      login: '/login'
    }    
  }   
};
