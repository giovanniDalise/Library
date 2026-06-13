  /**
   * API Gateway
   * - locale
   * - docker
   * - prod
   */
export const environment = {
  production: false,
  // In locale punti direttamente ai microservizi
  apiUrls: {
    books: 'http://localhost:5282/books',
    editors: 'http://localhost:5282/editors',
    authors: 'http://localhost:5282/authors',
    users: 'http://localhost:5022/users',
    auth: 'http://localhost:5073/auth'
  },
  apiPrefix: '', // Non serve prefisso in locale
  api: {
    books: {
      getBooks: '/getBooks',
      getById: '/{id}',
      getEditorDetail: '/getEditorDetail/{id}',
      addBook: '/addBook',
      update: '/{id}',
      delete: '/{id}'
    },
    editors: {
      getEditors: '/getEditors',
      getEditorDetail: '/{id}',
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
