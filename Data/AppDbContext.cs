using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Text.Json;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using AbdmWrapperNet.Configuration;
using AbdmWrapperNet.Models;

namespace AbdmWrapperNet.Data;

public class AppDbContext : DbContext
{
    private readonly string _connectionString;
    private readonly string _provider;

    public AppDbContext(IOptions<AbdmConfig> config)
    {
        _provider = config.Value.DatabaseProvider ?? "SqlServer";
        _connectionString = _provider.Equals("Postgres", StringComparison.OrdinalIgnoreCase)
            ? config.Value.Postgres.ConnectionString
            : config.Value.SqlServer.ConnectionString;
    }

    public DbSet<Patient> Patients { get; set; } = null!;
    public DbSet<RequestLog> RequestLogs { get; set; } = null!;
    public DbSet<LinkToken> LinkTokens { get; set; } = null!;
    public DbSet<ConsentPatient> ConsentPatients { get; set; } = null!;
    public DbSet<ConsentRequestMapping> ConsentRequestMappings { get; set; } = null!;
    public DbSet<AbhaConsentLog> AbhaConsentLogs { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured && !string.IsNullOrEmpty(_connectionString))
        {
            if (_provider.Equals("Postgres", StringComparison.OrdinalIgnoreCase))
            {
                optionsBuilder.UseNpgsql(_connectionString);
            }
            else
            {
                optionsBuilder.UseSqlServer(_connectionString);
            }
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Converters using helper methods to avoid Expression Tree optional argument issues
        var careContextsConverter = new ValueConverter<List<CareContext>, string>(
            v => SerializeCareContexts(v),
            v => DeserializeCareContexts(v));

        var consentsConverter = new ValueConverter<List<Consent>, string>(
            v => SerializeConsents(v),
            v => DeserializeConsents(v));

        var objectConverter = new ValueConverter<object?, string>(
            v => SerializeObject(v),
            v => DeserializeObject(v));

        var bsonDocumentConverter = new ValueConverter<BsonDocument, string>(
            v => SerializeBsonDocument(v),
            v => DeserializeBsonDocument(v));

        string textColumnType = _provider.Equals("Postgres", StringComparison.OrdinalIgnoreCase)
            ? "text"
            : "nvarchar(max)";

        // Patient Configuration
        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Id).ValueGeneratedOnAdd();
            entity.Property(p => p.CareContexts)
                .HasConversion(careContextsConverter)
                .HasColumnType(textColumnType);
            entity.Property(p => p.Consents)
                .HasConversion(consentsConverter)
                .HasColumnType(textColumnType);
        });

        // RequestLog Configuration
        modelBuilder.Entity<RequestLog>(entity =>
        {
            entity.HasKey(r => r.Id);
            entity.Property(r => r.Id).ValueGeneratedOnAdd();
            entity.Property(r => r.Error)
                .HasConversion(objectConverter)
                .HasColumnType(textColumnType);
            entity.Property(r => r.RequestDetails)
                .HasConversion(bsonDocumentConverter)
                .HasColumnType(textColumnType);
            entity.Property(r => r.ResponseDetails)
                .HasConversion(bsonDocumentConverter)
                .HasColumnType(textColumnType);
        });

        // LinkToken Configuration
        modelBuilder.Entity<LinkToken>(entity =>
        {
            entity.HasKey(l => l.Id);
            entity.Property(l => l.Id).ValueGeneratedOnAdd();
        });

        // ConsentPatient Configuration
        modelBuilder.Entity<ConsentPatient>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Id).ValueGeneratedOnAdd();
        });

        // AbhaConsentLog Configuration
        modelBuilder.Entity<AbhaConsentLog>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Id).ValueGeneratedOnAdd();
        });
    }

    private static readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = false };

    private static string SerializeCareContexts(List<CareContext> careContexts)
    {
        return JsonSerializer.Serialize(careContexts, _jsonOptions);
    }

    private static List<CareContext> DeserializeCareContexts(string json)
    {
        return string.IsNullOrEmpty(json) ? new List<CareContext>() : JsonSerializer.Deserialize<List<CareContext>>(json, _jsonOptions) ?? new List<CareContext>();
    }

    private static string SerializeConsents(List<Consent> consents)
    {
        return JsonSerializer.Serialize(consents, _jsonOptions);
    }

    private static List<Consent> DeserializeConsents(string json)
    {
        return string.IsNullOrEmpty(json) ? new List<Consent>() : JsonSerializer.Deserialize<List<Consent>>(json, _jsonOptions) ?? new List<Consent>();
    }

    private static string SerializeObject(object? obj)
    {
        return obj == null ? "" : JsonSerializer.Serialize(obj, _jsonOptions);
    }

    private static object? DeserializeObject(string json)
    {
        return string.IsNullOrEmpty(json) ? null : JsonSerializer.Deserialize<object>(json, _jsonOptions);
    }

    private static string SerializeBsonDocument(BsonDocument doc)
    {
        return doc == null ? "{}" : doc.ToString() ?? "{}";
    }

    private static BsonDocument DeserializeBsonDocument(string json)
    {
        return string.IsNullOrEmpty(json) ? new BsonDocument() : BsonSerializer.Deserialize<BsonDocument>(json);
    }
}
