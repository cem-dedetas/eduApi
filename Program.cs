using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using WebApplication1.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using Org.BouncyCastle.Asn1.Ocsp;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:5173/",
                                              "*");
                      });
});


builder.Services.AddDbContext<DataContext>(options =>
{
    //from connetcion string
    options.UseMySQL( builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
   options.TokenValidationParameters = new TokenValidationParameters
   {
       ValidateIssuer = true, // Validate the token issuer
       ValidateAudience = true, // Validate the token audience
       ValidateLifetime = true, // Validate the token expiration
       ValidateIssuerSigningKey = true, // Validate the token signature
       ValidIssuer = "edubackendapi", // The issuer your tokens are signed by
       ValidAudience = "aud", // The audience the tokens are intended for
       IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("JwtSettings:SecretKey").Value)) // The secret key used to sign the tokens
   };
});

builder.Services.AddAuthorization(options =>
{

});




var app = builder.Build();
app.UseCors(builder =>
{
    builder.AllowAnyOrigin().AllowAnyMethod()
               .AllowAnyHeader();
});
app.UseAuthentication();
app.UseAuthorization();

var agoraServiceClass = new EduAPI.AgoraServiceClass();
var authServiceClass = new EduAPI.AuthService(builder.Configuration);

app.Use(async (context, next) =>
{

    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();

        if (context.Request.Headers.TryGetValue("Authorization", out var authHeader))
        {
            var token = authHeader.ToString().Replace("Bearer ", string.Empty);

            var user = await authServiceClass.getUserFromToken(dbContext, token);

            if (user != null)
            {
                context.Items["User"] = user;
            }
            else
            {
                context.Response.StatusCode = 401; // Unauthorized
                return;
            }
        }
    }

    await next.Invoke();
});

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}





app.MapGet("/record/getResourceID/{channelName}/{uid}", async (string channelName,string uid) =>
{

    var response = await agoraServiceClass.AcquireResource(channelName,uid);
    return Results.Ok( response);
})
.WithName("getResourceID");


app.MapPost("/record/start/{channelName}/{uid}", async (HttpRequest context, DataContext db, string uid, string channelName) =>
{

    var requestModel = await context.ReadFromJsonAsync<EduAPI.ParticipantModel>();
    var response = await agoraServiceClass.StartRecording(requestModel.rid, channelName, uid, requestModel.token, requestModel.ctype, requestModel.uid);
    return Results.Ok(response);
});

app.MapPost("/record/stop/{channelName}/{uid}/{rid}/{sid}", async (string rid, string sid, string uid, string channelName) =>
{

    var response = await agoraServiceClass.StopRecording(rid, sid, uid, channelName);
    return Results.Ok(response);
});

app.MapPost("/record/update/{channelName}/{uid}/{rid}/{sid}", async (string rid, string sid, string uid, string channelName) =>
{

    var response = await agoraServiceClass.StopRecording(rid, sid, uid, channelName);
    return Results.Ok(response);
});

app.MapGet("/token/generate/{channelName}/{uid}", async (string channelName, string uid) =>
{

    var response = await agoraServiceClass.TokenGenerator(channelName, uid);
    return Results.Ok(response);
});

app.MapGet("/token/generateRTM/{uid}", async (string uid) =>
{

    var response = await agoraServiceClass.TokenGenerator2(uid);
    return Results.Ok(response);
});


app.MapGet("/users", async (DataContext db) =>
{
    var users = await db.Users.ToListAsync();
    return Results.Ok(users);
});

app.MapGet("/users/{id}", async (DataContext db, int id) =>
{
    var user = await db.Users.FindAsync(id);
    return Results.Ok(user);
}).RequireAuthorization();


app.MapPut("/users/{id}", async (DataContext db, int id, User user) =>
{
    if (id != user.id)
    {
        return Results.BadRequest();
    }

    db.Entry(user).State = EntityState.Modified;
    await db.SaveChangesAsync();

    return Results.Ok(user);
});

app.MapDelete("/users/{id}", async (DataContext db, int id) =>
{
    var user = await db.Users.FindAsync(id);
    if (user == null)
    {
        return Results.NotFound();
    }

    db.Users.Remove(user);
    await db.SaveChangesAsync();

    return Results.Ok();
});

app.MapGet("/liveLecture/get/{channelCode}", async (HttpContext context, DataContext db, string channelCode) =>
{
    LiveLectureClass lecture = await db.LiveLectures.Include(l => l.Lecturer).FirstOrDefaultAsync(x => x.ShareUrl == channelCode);
    if (lecture == null)
    {
        return Results.NotFound();
    }


    return Results.Ok(lecture);
}
).RequireAuthorization();


app.MapGet("/liveLecture/create/{channelName}", async (HttpContext context,DataContext db, string channelName) =>
{
    var liveLecture = new LiveLectureClass();
    User lecturer = await db.Users.FirstOrDefaultAsync(x => x.id == ((User)context.Items["User"]).id);
    liveLecture.Lecturer = lecturer;
    liveLecture.Tags = new List<Tag>();
    liveLecture.Attendees = new List<User>();
    liveLecture.ChannelName = channelName;
    liveLecture.Date = DateTime.Now;
    liveLecture.ChatLog = new List<ChatMessage>();
    liveLecture.Recordings = new List<Recording>();



    //save live lecture to db and get live lecture 
    db.LiveLectures.Add(liveLecture);
    await db.SaveChangesAsync();
    //also update share url
    //create alphanumeric string for share url 6 digits
    var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    var stringChars = new char[6];
    var random = new Random();

    for (int i = 0; i < stringChars.Length; i++)
    {
        stringChars[i] = chars[random.Next(chars.Length)];
    }

    var finalString = new String(stringChars);

    liveLecture.ShareUrl = finalString;
    db.Entry(liveLecture).State = EntityState.Modified;
    await db.SaveChangesAsync();


    return Results.Ok(liveLecture);
}).RequireAuthorization();

app.MapPost("/auth/signup", async (DataContext db, UserSignUpDTO user) =>
{
    authServiceClass.CreatePasswordHash(user.password, out byte[] passwordHash, out byte[] passwordSalt);
    User newUser = new User
    {
        username = user.username,
        password_hash = passwordHash,
        password_salt = passwordSalt,
        email = user.email,
        id_number = user.id_number
    };
    db.Users.Add(newUser);
    await db.SaveChangesAsync();
    return Results.Ok(newUser);

});

app.MapPost("/auth/signin", async (DataContext db, UserSignInDTO user)=>{
    User userFromDb;
    Console.WriteLine(user.id_number);
    if (user.email !=null && user.email.Length > 0) {
        userFromDb = await db.Users.FirstOrDefaultAsync(x => x.email == user.email);
    }
    else
    {
        userFromDb = await db.Users.FirstOrDefaultAsync(x => x.id_number == user.id_number);
    }
    
    if (userFromDb == null)
    {
        return Results.BadRequest();
    }
    if (!authServiceClass.VerifyPasswordHash(user.password, userFromDb.password_hash, userFromDb.password_salt))
    {
        return Results.BadRequest();
    }
    var token = authServiceClass.CreateToken(userFromDb);
    return Results.Ok(token);
});

app.MapPost("/tags/add", async (HttpContext context, DataContext db, TagDTO fromBody) =>
{
    Tag existing = await db.Tags.FirstOrDefaultAsync(x => x.tag_name == fromBody.tag_name);
    if (existing != null) return Results.BadRequest("Alread Exists");
    Tag newTag = new Tag();
    newTag.tag_name = fromBody.tag_name;
    db.Tags.Add(newTag);
    await db.SaveChangesAsync();
    return Results.Ok(newTag);

}).RequireAuthorization();

app.MapPost("/tags/addMultiple", async (HttpContext context, DataContext db, TagDTO[] fromBody) =>
{
    int count = 0;
    int failed = 0;
    foreach (TagDTO tag in fromBody)
    {
        Tag existing = await db.Tags.FirstOrDefaultAsync(x => x.tag_name == tag.tag_name);
        if (existing != null)
        {
            failed++;
            continue;
        }
        Tag newTag = new Tag();
        newTag.tag_name = tag.tag_name;
        db.Tags.Add(newTag);
        await db.SaveChangesAsync();
        count++;
    }

    return Results.Ok(count + " new tags added succesfully.(Failed " + failed + ")");
}).RequireAuthorization();

app.MapPost("/liveLecture/{channelCode}/userJoin",async (HttpContext context, DataContext db, string channelCode) =>
{
        User user = await db.Users.FirstOrDefaultAsync(x => x.id == ((User)context.Items["User"]).id);
        LiveLectureClass lecture = await db.LiveLectures.FirstOrDefaultAsync(x => x.ShareUrl == channelCode);
        if (user == null || lecture == null)
        {
            return Results.BadRequest();
        }
        UserHistory userHistory = new UserHistory();
        userHistory.user = user;
        userHistory.liveLecture = lecture;
        userHistory.joinDate = DateTime.Now;
        userHistory.lastPingDate = userHistory.joinDate;
        db.UserHistories.Add(userHistory);
        await db.SaveChangesAsync();

        //update lecture and add user to participants if not exists
        if (lecture.Attendees == null)
        {
            lecture.Attendees = new List<User>();
        }
        if (!lecture.Attendees.Contains(user))
        {
            lecture.Attendees.Add(user);
        }
        db.Entry(lecture).State = EntityState.Modified;
        await db.SaveChangesAsync();

        return Results.Ok(userHistory);
}).RequireAuthorization();

app.MapPost("/liveLecture/{channelCode}/userPing", async (HttpContext context, DataContext db, string channelCode) =>
{
    User user = await db.Users.FirstOrDefaultAsync(x => x.id == ((User)context.Items["User"]).id);
    LiveLectureClass lecture = await db.LiveLectures.FirstOrDefaultAsync(x => x.ShareUrl == channelCode);
    if (user == null || lecture == null)
    {
        return Results.BadRequest();
    }
    UserHistory userHistory = await db.UserHistories.FirstOrDefaultAsync(x => x.user == user && x.liveLecture == lecture);
    if (userHistory == null)
    {
        return Results.BadRequest();
    }
    userHistory.lastPingDate = DateTime.Now;
    db.Entry(userHistory).State = EntityState.Modified;
    await db.SaveChangesAsync();

    return Results.Ok(userHistory);
}).RequireAuthorization();

app.MapGet("/tags", async (DataContext db) =>
{
    IEnumerable<Tag> users = await db.Tags.ToListAsync();
    return Results.Ok(users);
});

//`/liveLecture/${channelCode}/chat/addMessage`
app.MapPost("/liveLecture/{channelCode}/chat/addMessage", async (HttpContext context, DataContext db, string channelCode, MessageDTO _message) =>
{
    User user = await db.Users.FirstOrDefaultAsync(x => x.id == ((User)context.Items["User"]).id);
    LiveLectureClass lecture = await db.LiveLectures.FirstOrDefaultAsync(x => x.ShareUrl == channelCode);
    if (user == null || lecture == null)
    {
        return Results.BadRequest();
    }
    ChatMessage message = new ChatMessage();
    message.sender = user;
    message.timeStamp = _message.timestamp;
    message.content = _message.text;
    if (lecture.ChatLog == null)
    {
        lecture.ChatLog = new List<ChatMessage>();
    }
    lecture.ChatLog.Add(message);
    db.Entry(lecture).State = EntityState.Modified;
    await db.SaveChangesAsync();
    return Results.Ok(message);
}).RequireAuthorization();

app.MapPost("/liveLecture/createRecording/{channelName}/{sid}/{ruid}/{resourceId}",
    async (HttpContext contxt, DataContext db, string channelName, string sid, string ruid, string resourceId) =>
{

    LiveLectureClass lecture = await db.LiveLectures.FirstAsync(x => x.ChannelName == channelName);
    if (lecture == null) return Results.BadRequest();
    Recording record = new Recording();
    record.resourceId = resourceId;
    record.ruid = ruid;
    record.sid = sid;
    record.start = DateTime.Now;
    if(lecture.Recordings == null)
    {
        lecture.Recordings = new List<Recording>();
    }
    lecture.Recordings.Add(record);

    db.Entry(lecture).State = EntityState.Modified;
    await db.SaveChangesAsync();
    return Results.Ok(lecture);

});

app.MapPost("/liveLecture/endRecording/{sid}",
    async (HttpContext contxt, HttpRequest request, DataContext db, string sid) =>
    {
        var body = await request.ReadFromJsonAsync<someDTO>();

        //LiveLectureClass lecture = await db.LiveLectures.Include(l=> l.Recordings).FirstAsync(x => x.ChannelName == channelName);
        Recording record = await db.Recordings.FirstOrDefaultAsync(x => x.sid == sid);
        if (record == null) return Results.BadRequest("No recording");
        if (record.end != null) return Results.BadRequest("Already ended");

        record.end = DateTime.Now;
        record.url = body.url;

        db.Entry(record).State = EntityState.Modified;
        await db.SaveChangesAsync();
        return Results.Ok(record);

    });

app.MapPost("/liveLecture/addUrl/{channelCode}/{sid}",
    async (HttpContext contxt, DataContext db, string channelName, string sid, string url) =>
    {

        LiveLectureClass lecture = await db.LiveLectures.FirstAsync(x => x.ShareUrl == channelName);
        Recording record = lecture.Recordings.Find(x => x.sid == sid);
        if (record == null) return Results.BadRequest("No recording");
        record.url = url;

        db.Entry(record).State = EntityState.Modified;
        await db.SaveChangesAsync();
        return Results.Ok(record);

    });

app.MapGet("/liveLecture/getRecordingData/{channelCode}",
    async (HttpContext contxt, DataContext db, string channelCode) =>
    {

        LiveLectureClass lecture = await db.LiveLectures.FirstAsync(x => x.ShareUrl == channelCode);
        Recording record = lecture.Recordings.Find(x => x.end == null);
        if (record == null) return Results.BadRequest("No recording");
        return Results.Ok(record);

    });

//get live all lectures for all users
app.MapGet("/liveLecture/getAll", async (HttpContext context, DataContext db) =>
{

    var lectures = await db.LiveLectures.Include(l => l.Lecturer).Include(l => l.Recordings).Include(l => l.Tags).ToListAsync();
    return Results.Ok(lectures);
});



app.Run();

