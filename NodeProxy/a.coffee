connect = require('connect')
http = require('http')

app = connect()
  .use(connect.favicon())
  .use(connect.logger('dev'))
  .use(connect.static('public'))
  .use(connect.directory('public'))
  .use(connect.cookieParser('my secret here'))
  .use(connect.session())
  .use((req, res) -> 
    res.end('Hello from Connecti!\n');
  );

http.createServer(app).listen(3000)
