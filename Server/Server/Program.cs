using Server.App;
using Server.Models;

App app = new App("127.0.0.1", 8080);
await app.RunAsync();
