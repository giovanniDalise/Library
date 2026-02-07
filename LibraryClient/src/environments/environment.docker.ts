export const environment = {
  production: true,
  apiUrls: {
    books: 'http://api-gateway/api/books',
    users: 'http://api-gateway/api/users',
    auth: 'http://api-gateway/api/auth'
  },
  apiPrefix: '/api',
  api: {
    books: {
      getBooks: '/getBooks',
      getById: '/{id}',
      create: '',
      update: '/{id}',
      delete: '/{id}'
    },
    auth: {
      login: '/login'
    }    
  }   
};
