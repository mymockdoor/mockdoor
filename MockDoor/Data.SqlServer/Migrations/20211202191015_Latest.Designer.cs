﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MockDoor.Data.Contexts;

#nullable disable

namespace MockDoor.Data.Migrations.SqlServer
{
    [DbContext(typeof(MockDoorMainContext))]
    [Migration("20211202191015_Latest")]
    partial class Latest
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("MockDoor.Data.Models.Headers.RequestHeader", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"), 1L, 1);

                    b.Property<string>("HeaderName")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int>("ServiceRequestID")
                        .HasColumnType("int");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.HasIndex("ServiceRequestID");

                    b.ToTable("RequestHeaders");
                });

            modelBuilder.Entity("MockDoor.Data.Models.Headers.ResponseHeader", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"), 1L, 1);

                    b.Property<string>("HeaderName")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int>("RequestResponseID")
                        .HasColumnType("int");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.HasIndex("RequestResponseID");

                    b.ToTable("ResponseHeaders");
                });

            modelBuilder.Entity("MockDoor.Data.Models.Headers.ServiceHeader", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"), 1L, 1);

                    b.Property<bool>("Enabled")
                        .HasColumnType("bit");

                    b.Property<string>("HeaderName")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<bool>("Incoming")
                        .HasColumnType("bit");

                    b.Property<int>("MicroserviceID")
                        .HasColumnType("int");

                    b.Property<bool>("Outgoing")
                        .HasColumnType("bit");

                    b.HasKey("ID");

                    b.HasIndex("MicroserviceID");

                    b.ToTable("ServiceHeaders");
                });

            modelBuilder.Entity("MockDoor.Data.Models.Microservice", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"), 1L, 1);

                    b.Property<bool>("Enabled")
                        .HasColumnType("bit");

                    b.Property<int>("FakeDelay")
                        .HasColumnType("int");

                    b.Property<int>("HeadersMode")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Path")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<bool>("ProxyMode")
                        .HasColumnType("bit");

                    b.Property<bool>("RandomiseMockResult")
                        .HasColumnType("bit");

                    b.Property<int>("ServiceGroupID")
                        .HasColumnType("int");

                    b.Property<string>("TargetUrl")
                        .HasMaxLength(450)
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("ID");

                    b.HasIndex("ServiceGroupID");

                    b.ToTable("Microservices");
                });

            modelBuilder.Entity("MockDoor.Data.Models.RequestResponse", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"), 1L, 1);

                    b.Property<string>("Body")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Checksum")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("varchar(32)");

                    b.Property<int>("Code")
                        .HasColumnType("int");

                    b.Property<string>("ContentType")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int>("Encoding")
                        .HasColumnType("int");

                    b.Property<int>("ServiceRequestId")
                        .HasColumnType("int");

                    b.HasKey("ID");

                    b.HasIndex("ServiceRequestId");

                    b.ToTable("RequestResponse");
                });

            modelBuilder.Entity("MockDoor.Data.Models.ServiceGroup", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"), 1L, 1);

                    b.Property<string>("DefaultHealthCheckUrl")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<bool>("Enabled")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Path")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int>("TenantID")
                        .HasColumnType("int");

                    b.HasKey("ID");

                    b.HasIndex("TenantID");

                    b.ToTable("ServiceGroups");
                });

            modelBuilder.Entity("MockDoor.Data.Models.ServiceRequest", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"), 1L, 1);

                    b.Property<DateTime>("CreatedUtc")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasDefaultValueSql("GETDATE()")
                        .HasColumnType("datetime2");

                    b.Property<bool>("ExactUrlMatch")
                        .HasColumnType("bit");

                    b.Property<bool>("ExpectAuthHeader")
                        .HasColumnType("bit");

                    b.Property<string>("FromBody")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FromUrl")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<int>("MicroserviceID")
                        .HasColumnType("int");

                    b.Property<int>("RestType")
                        .HasColumnType("int");

                    b.Property<long?>("TTLTicks")
                        .HasColumnType("bigint");

                    b.HasKey("ID");

                    b.HasIndex("MicroserviceID");

                    b.ToTable("ServiceRequests");
                });

            modelBuilder.Entity("MockDoor.Data.Models.Tenant", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"), 1L, 1);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Path")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("ID");

                    b.HasIndex("Path")
                        .IsUnique();

                    b.ToTable("Tenants");
                });

            modelBuilder.Entity("MockDoor.Data.Models.Headers.RequestHeader", b =>
                {
                    b.HasOne("MockDoor.Data.Models.ServiceRequest", null)
                        .WithMany("RequestHeaders")
                        .HasForeignKey("ServiceRequestID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("MockDoor.Data.Models.Headers.ResponseHeader", b =>
                {
                    b.HasOne("MockDoor.Data.Models.RequestResponse", null)
                        .WithMany("Headers")
                        .HasForeignKey("RequestResponseID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("MockDoor.Data.Models.Headers.ServiceHeader", b =>
                {
                    b.HasOne("MockDoor.Data.Models.Microservice", null)
                        .WithMany("Headers")
                        .HasForeignKey("MicroserviceID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("MockDoor.Data.Models.Microservice", b =>
                {
                    b.HasOne("MockDoor.Data.Models.ServiceGroup", "ServiceGroup")
                        .WithMany("Microservices")
                        .HasForeignKey("ServiceGroupID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ServiceGroup");
                });

            modelBuilder.Entity("MockDoor.Data.Models.RequestResponse", b =>
                {
                    b.HasOne("MockDoor.Data.Models.ServiceRequest", "ServiceRequest")
                        .WithMany("RequestResponses")
                        .HasForeignKey("ServiceRequestId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ServiceRequest");
                });

            modelBuilder.Entity("MockDoor.Data.Models.ServiceGroup", b =>
                {
                    b.HasOne("MockDoor.Data.Models.Tenant", "Tenant")
                        .WithMany("ServiceGroups")
                        .HasForeignKey("TenantID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Tenant");
                });

            modelBuilder.Entity("MockDoor.Data.Models.ServiceRequest", b =>
                {
                    b.HasOne("MockDoor.Data.Models.Microservice", "Microservice")
                        .WithMany("ServiceRequests")
                        .HasForeignKey("MicroserviceID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Microservice");
                });

            modelBuilder.Entity("MockDoor.Data.Models.Microservice", b =>
                {
                    b.Navigation("Headers");

                    b.Navigation("ServiceRequests");
                });

            modelBuilder.Entity("MockDoor.Data.Models.RequestResponse", b =>
                {
                    b.Navigation("Headers");
                });

            modelBuilder.Entity("MockDoor.Data.Models.ServiceGroup", b =>
                {
                    b.Navigation("Microservices");
                });

            modelBuilder.Entity("MockDoor.Data.Models.ServiceRequest", b =>
                {
                    b.Navigation("RequestHeaders");

                    b.Navigation("RequestResponses");
                });

            modelBuilder.Entity("MockDoor.Data.Models.Tenant", b =>
                {
                    b.Navigation("ServiceGroups");
                });
#pragma warning restore 612, 618
        }
    }
}
