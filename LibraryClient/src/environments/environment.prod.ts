  /**
   * API Gateway
   * - locale
   * - docker
   * - prod
   */
export const environment = {
  production: true,
  // In locale punti direttamente ai microservizi
  apiUrls: {
    books: 'http://localhost:5282/library',
    editors: 'http://localhost:5282/editors',
    authors: 'http://localhost:5282/authors',
    users: 'http://localhost:5022/users',
    auth: 'http://localhost:5073/auth',
  },
  apiPrefix: '', // Non serve prefisso in locale
  api: {
    books: {
      getBooks: '/getBooks',
      getById: '/{id}',
      create: '',
      update: '/{id}',
      delete: '/{id}'
    },
    editors: {
      getEditors: '/getEditors',
      getById: '/{id}'  
    },
    authors:{
      getAuthors: '/getAuthors'
    },           
    auth: {
      login: '/login'
    }    
  }  
};
