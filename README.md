<div align="center">
    <img src="/Images/PubHub.png" alt="Header_Image" width="300px" height="300px"/>
</div>
<div align="center">
    <p style="display:block; font-weight:normal; font-size:20px;">PubHub Distribution Service </p>
</div>

---

# Introduction
This project represents the final exam for the Data Technician with Speciality in Programming education.

PubHub is an audio- and e-book distribution service, which will stores many publishers products; offering them statistics and the ability to always administrate their products through an `Admin Portal`. PubHub also offers their own solution for consumers to access the publishers products through both a mobile application (`Book App`) and a web application (`Book Web`). Here the consumers can draw subscription and add books to their personal library as they wish; however if the subscription ends, the books will be made unavailable for the consumer. They can also buy books and be an owner of that book for as long as their Account isn't deleted by themselves.

PubHub also offers the service of Second-party stores and markets to makes a partner agreement with them, gaining access to the huge storage of audio- and e-books.

---

# Tables of contents

- [Projects](https://github.com/Mike-Mortensen-Portfolio/PubHub_H6_Final?tab=readme-ov-file#projects)
- [PubHub Architecture](https://github.com/Mike-Mortensen-Portfolio/PubHub_H6_Final?tab=readme-ov-file#pubhub-architecture)
- [API endpoint](https://github.com/Mike-Mortensen-Portfolio/PubHub_H6_Final?tab=readme-ov-file#api-endpoints)
- [Setup](https://github.com/Mike-Mortensen-Portfolio/PubHub_H6_Final?tab=readme-ov-file#setup)
  - [Admin portal](https://github.com/Mike-Mortensen-Portfolio/PubHub_H6_Final?tab=readme-ov-file#admin-portal)
  - [Book mobile](https://github.com/Mike-Mortensen-Portfolio/PubHub_H6_Final?tab=readme-ov-file#book-mobile)
  - [API](https://github.com/Mike-Mortensen-Portfolio/PubHub_H6_Final?tab=readme-ov-file#api)
- [Standards](https://github.com/Mike-Mortensen-Portfolio/PubHub_H6_Final?tab=readme-ov-file#standards)

---

# Projects

| **Project** | **Platform**            | **Languages**     | **Timeframe** | **Backend Store** |
|-------------|-------------------------|-------------------|---------------|-------------------|
| `App`       | .NET MAUI               | C#, XAML          |               | `Api`             |
| `Api`       | .NET RESTApi            | C#                |               | `MS SQL DB`       |
| `Web`       | .NET Blazor Server      | C#, JS, HTML, CSS |               | `Api`             |
| `Web`       | .NET Blazor WebAssembly | C#, JS, HTML, CSS |               | `Api`             |

---

# PubHub Architecture

![image](https://github.com/Mike-Mortensen-Portfolio/PubHub_H6_Final/assets/61870713/789d6775-c4a0-40fb-ad80-d6d9c465cace)

The diagram above demostrates the communication between the different sub-systems in the PubHub system. If we begin from the top-left side, we have 3 different client-side applications; `.NET MAUI: Book App`, `.NET Blazor WebAssembly: Book Web` and a `.NET Blazor Server: Admin Portal`. We can see that they all 3 will use the `.NET REST API` to communicate through to retrieve and send data values. 

At the top we can see two external systems, who also will be communicating with the `.NET REST API`. We have the `Payment Provider` who will be handling the transactions made on `Book App` and `Book Web` applications. Then there's the `Library Provider` which handles librarian users to become a part of the system, as well as providing data to PubHub with how many copies they have of their books, ready for borrowing. Lastly there's the `Second-Party Providers` which respresents any other stores or makets much like our `Book App` and `Book Web` who can make a partner agreement to gain access to PubHub's storage of books, on the conditions of giving statistics about those books in return.

Lastly, is the `MS SQL Server` which is an on-site server but, with backups to the cloud and to other servers. All Book data and account data is stored here.

---

# API Endpoints

We have a seperate [wiki page](https://github.com/Mike-Mortensen-Portfolio/PubHub_H6_Final/wiki/Api-Documentation#api-endpoints-overview) with overview of the tables.

In this overview, the API endpoint will have a link attached to them, where you can click to be granted a more detailed explaination of the individual endpoints. This includes the request and response body samples and what status codes they will give in case of success or failure.

---

# Setup

## Admin portal 
You will have to som lines to the `appSettings.json` to enable the Authorization part, as there is in the `Program.cs` a section to add a pair of custom policies. This is used so that we can get the authorized views depending on what account is signing into the application.

`appSettings.json` will have to contain the following with your own needed values:
```json
{    
    "ApiEndpoint": "<YourBaseUri>",
    "AppId": "<AppId>",
    "<AccountTypeName>": "<AccountTypeId>"
    "<AccountTypeName>": "<AccountTypeId>"
}
```

Then in the `Program.cs` we have to read that in when we're adding Authorization, also containing the values you set.
```c#
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("<AccountTypeName>", policy => policy.Requirements.Add(new CustomClaimRequirement("<Claim Key>", builder.Configuration.GetValue<string>("<AccountTypeName>") ?? throw new NullReferenceException("<AccountTypeName> couldn't be found."))));
    options.AddPolicy("<AccountTypeName>", policy => policy.Requirements.Add(new CustomClaimRequirement("<Claim Key>", builder.Configuration.GetValue<string>("<AccountTypeName>") ?? throw new NullReferenceException("<AccountTypeName> couldn't be found."))));
});
```

Addtionally the `Program.cs` will need the corresponding values for the connection to the API to function, where AddPubHubServices is a `ServiceExtension` adding the `httpClient` alongisde with all the `.AddScoped()` to use across the applications:
```c#
builder.Services.AddPubHubServices(options =>
{
    options.Address = builder.Configuration.GetValue<string>(<YourBaseUri>) ?? throw new NullReferenceException("API base address couldn't be found.");
    options.HttpClientName = <YourHttpClientName>;
    options.AppId = builder.Configuration.GetValue<string>(<AppId>) ?? throw new NullReferenceException("Application ID couldn't be found.");
    options.TokenInfoAsync = async (sp) =>
    {
        var localStorageService = sp.GetRequiredService<ILocalStorageService>();
        return new TokenInfo()
        {
            Token = await localStorageService.GetItemAsync<string>("token") ?? string.Empty,
            RefreshToken = await localStorageService.GetItemAsync<string>("refreshToken") ?? string.Empty
        };
    };
});
```

Lastly, we also added `HSTS` to the Admin portal so that we ensure that all communication and requests are being made over a secure transfer, and if somebody will try to access the application from `http` they're automatically redirected to the `https` browser instead by the use of `app.UseHttpsRedirection();`

```c#
builder.Services.AddHsts(options =>
{
    options.MaxAge = <TimeSpanFromDays>;    // Long-term security assurance and reduced vulnerability window.
});

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);    
    app.UseHsts();
}
```

## Book Mobile
The mobile application requires a TUNNEL_URL in `appSettings.json` that targets the API. If the tunnel is not present the app will not be able to connect to the API. See [this](https://learn.microsoft.com/da-dk/aspnet/core/test/dev-tunnels?view=aspnetcore-7.0) article for information on how to establish and use Dev Tunnels. 

The `appSettings.json` will have to contain the following with your own required values, for the book mobile to function:
```json
{
    "<ApiSection>": {
        "ApiEndpoint": "<DevTunnelUri>",
        "AppId": "<AppId>"
      },
    "<RegexSection>": {
        "EmailRegex": "^[a-zA-Z0-9._%-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,4}$"
      }
}
```

Addtionally the `MauiProgram.cs` will need the corresponding values for the connection to the API to function, where AddPubHubService is a `ServiceExtention` adding the `httpClient` alongside with all the `.AddScoped()` to use across the applications:
```c#
.AddPubHubServices(options =>
{
    options.Address = builder.Configuration.GetSection(<ApiSection>).GetValue<string>(<DevTunnelUri>) ?? throw new NullReferenceException("API base address couldn't be found.");
    options.HttpClientName = ApiConstants.HTTPCLIENT_NAME;
    options.AppId = builder.Configuration.GetSection(<ApiSection>).GetValue<string>(<AppId>) ?? throw new NullReferenceException("Application ID couldn't be found.");
    options.ConfigureForMobile = true;
    options.TokenInfoAsync = async (provider) =>
    {
        (bool IsSuccess, TokenInfo? tokens) = await User.TryGetCachedToken();
        if (!IsSuccess)
            return new TokenInfo { RefreshToken = string.Empty, Token = string.Empty };

        return tokens!;
    };
});
```

## API
We have a few setup requirements for the API to work properly, first we have our `JWT` which have some values which needs to be set up in the `appSettings.json` file so that our tokens can be signed with a key.

```json
"Jwt": {
  "Issuers": [
    "<ApiName>"
  ],
  "Lifetime": "<Lifetime>",
  "RefreshLifetime": "<RefreshLifetime>",
  "Audiences": [
    "<AudienceName>"
  ],
  "Key": "<KeyToSignTheJwtToken>"
}
```

Addtionally we have a section in the `appSettings.json` where we are white listing specific applications and what controllers and endpoints they will have access to. This is so we can limit access to the different endpoint requests depending on what application is making the request. This will be checked individually on each endpoint at the very start.

```json
"Whitelist": {
  "Apps": [
    {
      "AppId": "<AppId>",
      "Subjects": [
        "<AccountType>",
        "<AccountType>"
      ],
      "ControllerEndpoints": {
        "<ControllerName>": [
          "<EndpointName>",
          "<EndpointName>",
          "<EndpointName>",
          "<EndpointName>"
        ],
      }
    },
    {
      "AppId": "<AppId>",
      "Subjects": [
        "<AccountType>"
      ],
      "ControllerEndpoints": {
        "<ControllerName>": [
          "<EndpointName>",
          "<EndpointName>",
        ],
      }
    }
  ]
},
```

Additionally we have some configuration middleware being added in the `Program.cs` class, which will be used for different purposes.

First we're adding a Concurrently rate limiter to our API. We're adding this policy at the top of all our controllers, to prevent the API from any DoS attacks and to also disallow spam of requests to the API.
```c#
builder.Services.AddRateLimiter(rateLimterOptions =>
{
    rateLimterOptions.AddConcurrencyLimiter("<PolicyName>", options =>
    {
        options.PermitLimit = <RequestAllowedConcurrently>;
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        options.QueueLimit = <RequestsInQueue>;
    });

    rateLimterOptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});
```

Secondly we've added a specific way the api should react upon a validation problem, as it didn't correctly correspond to the way our `ErrorResponse.cs` was converting it. This was also to correctly handle when the `ModelState` was invalid. To see the specific error specification then click [here](https://github.com/Mike-Mortensen-Portfolio/PubHub_H6_Final/wiki/Api-Documentation#validation-problem-specification) for a better explaination.

```c#
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = actionContext =>
    {
        var problemDetails = new ValidationProblemDetails(actionContext.ModelState);

        var result = new BadRequestObjectResult(new
        {
            Status = <StatusCode>,
            Title = <Title>,
            Type = <Type>,
            Extensions = problemDetails.Errors.ToDictionary()
        });
        result.ContentTypes.Add("application/problem+json");

        return result;
    };
});
```

Lastly, we also added `HSTS` to the API so that we ensure that all communication and requests are being made over a secure transfer. `HSTS` ensures that the API will only accept requests from application browsers running on `https`. In the future, we'd want this to also be a part of preload list, so that it is officially safe.

```c#
builder.Services.AddHsts(options =>
{
    options.MaxAge = <TimeSpanFromDays>;    // Long-term security assurance and reduced vulnerability window.
});

if (!app.Environment.IsDevelopment())
{
    app.UseHsts(); // Enable HSTS middleware for non-development environments
}
```

---

# Standards

- **Source Control**
  - `Features` must be branched out and developed on an isolated branch and merged back into the `Developer` branch when done.
  - Branches will be named as followed: `[Initials]/[feature]` in all lowercase, with words seperated by dash.
    - Example would be: **msm/my-feature**
      
- **Code**
  - `Namespaces` must be constructed as follows: PubHub.[ProjectName].[FolderName].[SubFolderName]
    - We follow the `FBT`: Folder by type structure in our project.
  - `Fields` must be *private* or *protected*.
  - `Properties` must be *public*, *protected* or *internal*.
  - Our `Constants` follows the Screaming Snake Casing: THIS_IS_SCREAMING_SNAKE_CASING.
  - The API endpoint addresses are formed according to the URL standard; `https://pubhub.com/example-url/21`.
  - The mobile codebase must comply with the MVVM pattern, while both it and the web application must comply with DI patterns.
    
- **Error Handling**
  - The API will provide error responses that will be handled according to the [RFC9457](https://datatracker.ietf.org/doc/html/rfc9457) standard.
