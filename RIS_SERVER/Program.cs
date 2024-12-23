﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RIS_SERVER.entities;
using RIS_SERVER.server;
using RIS_SERVER.src.auth;
using RIS_SERVER.src.common.middlewares;
using RIS_SERVER.src.file;
using RIS_SERVER.src.jwt;
using RIS_SERVER.src.storage;
using RIS_SERVER.src.user;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .Build();

var serviceProvider = new ServiceCollection()
    .AddSingleton<IConfiguration>(configuration)

    .AddDbContext<AppDbContext>(options => options
        .UseSqlite(configuration
            .GetConnectionString("DefaultConnection")))

    .AddSingleton<FileHelper>()

    .AddSingleton<UserService>()
    .AddSingleton<TokenService>()
    .AddSingleton<AuthService>()
    .AddSingleton<StorageService>()
    .AddSingleton<FileService>()

    .AddSingleton<AuthHandler>()
    .AddSingleton<StorageHandler>()
    .AddSingleton<FileHandler>()
    .AddSingleton<UserHandler>()

    .AddSingleton<CheckUserMiddleware>()
    .AddSingleton<CheckStorageAccessMiddleware>()
    
    .BuildServiceProvider();

using (var scope = serviceProvider.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    context.Database.EnsureCreated();

    var authHandler = scope.ServiceProvider.GetRequiredService<AuthHandler>();
    var storageHandler = scope.ServiceProvider.GetRequiredService<StorageHandler>();
    var fileHandler = scope.ServiceProvider.GetRequiredService<FileHandler>();
    var userHandler = scope.ServiceProvider.GetRequiredService<UserHandler>();

    var checkUserMiddleware = scope.ServiceProvider.GetRequiredService<CheckUserMiddleware>();
    var checkStorageAccessMiddleware = scope.ServiceProvider.GetRequiredService<CheckStorageAccessMiddleware>();

    var handler = new Handler();

    handler.Add("auth/sign-in", authHandler.SignIn, []);
    handler.Add("auth/sign-up", authHandler.SignUp, []);

    handler.Add("user/find-many", userHandler.FindMany, []);
    handler.Add("user/me", userHandler.Me, [checkUserMiddleware]);

    handler.Add("storage/create", storageHandler.Create, [checkUserMiddleware]);
    handler.Add("storage/update", storageHandler.Update, [checkUserMiddleware, checkStorageAccessMiddleware]);
    handler.Add("storage/delete", storageHandler.Delete, [checkUserMiddleware, checkStorageAccessMiddleware]);
    handler.Add("storage/add-collaborator", storageHandler.AddCollaborator, [checkUserMiddleware, checkStorageAccessMiddleware]);
    handler.Add("storage/remove-collaborator", storageHandler.RemoveCollaborator, [checkUserMiddleware, checkStorageAccessMiddleware]);
    handler.Add("storage/find-many", storageHandler.FindMany, []);
    handler.Add("storage/find-many-user", storageHandler.FindUserStorages, [checkUserMiddleware]);

    handler.Add("file/upload-chunk", fileHandler.UploadChunk, [checkUserMiddleware, checkStorageAccessMiddleware]);
    handler.Add("file/upload-full", fileHandler.UploadFullFile, [checkUserMiddleware, checkStorageAccessMiddleware]);
    handler.Add("file/update", fileHandler.Update, [checkUserMiddleware, checkStorageAccessMiddleware]);
    handler.Add("file/delete", fileHandler.Delete, [checkUserMiddleware, checkStorageAccessMiddleware]);
    handler.Add("file/download-chunk", fileHandler.DownloadChunk, []);

    Server server = new Server(handler);

    server.Start();
}