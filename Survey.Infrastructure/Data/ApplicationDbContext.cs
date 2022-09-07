using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Survey.Core.Models;
using Microsoft.AspNetCore.Identity;

namespace Survey.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<Projects> Projects { get; set; }
        public DbSet<IdentityUser> IdentityUsers { get; set; }
        public DbSet<ProjectCategory> ProjectCategories { get; set; }
        public DbSet<Forms> Forms { get; set; }
        public DbSet<Questions> Questions { get; set; }
        public DbSet<QuestionOptions> QuestionOptions { get; set; }
        public DbSet<Answers> Answers { get; set; }
        public DbSet<SkipLogic> SkipLogic { get; set; }
        public DbSet<Dataset> Dataset { get; set; }
        public DbSet<EnrollDataset> EnrollDataset { get; set; }
        public DbSet<Cases> Cases { get; set; }
        public DbSet<CasesExcelHeaders> CasesExcelHeaders { get; set; }
        public DbSet<CasesExcelData> CasesExcelData { get; set; }
        public DbSet<CaseAssignedUsers> CaseAssignedIdentityUsers { get; set; }
        public DbSet<CaseAssignedForms> CaseAssignedForms { get; set; }
        public DbSet<SurveySubmission> SurveySubmissions { get; set; }
        public DbSet<SurveyDownload> SurveyDownloads { get; set; }
        public DbSet<UserProjectCategory> UserProjectCategories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Seed();
            modelBuilder.Entity<SurveySubmission>()
                .HasOne(x => x.Agent)
                .WithMany(x => x.SurveySubmissions)
                .HasForeignKey(x => x.AgentId)
                .OnDelete(DeleteBehavior.SetNull);


            modelBuilder.Entity<QuestionOptions>()
                .HasOne<Questions>(o => o.question)
                .WithMany(t => t.Options)
                .HasForeignKey(s => s.Question_Id);

            modelBuilder.Entity<SkipLogic>()
                  .HasOne(x => x.ChildQuestion)
                  .WithMany(x => x.skipChild)
                  .HasForeignKey(x => x.Child_Question_Id)
                  .IsRequired(false)
                   .OnDelete(DeleteBehavior.ClientCascade);


            modelBuilder.Entity<SkipLogic>()
                    .HasOne(x => x.ParentQuestion)
                    .WithMany(x => x.skipParent)
                    .HasForeignKey(x => x.Parent_Question_Id)
                    .IsRequired(false)
                    .OnDelete(DeleteBehavior.ClientCascade);


            modelBuilder.Entity<UserProjectCategory>()
                .HasKey(t => new { t.UserId, t.CategoryId });

            modelBuilder.Entity<UserProjectCategory>()
                .HasOne(pt => pt.Category)
                .WithMany(p => p.UserList)
                .HasForeignKey(pt => pt.CategoryId);

            modelBuilder.Entity<UserProjectCategory>()
                .HasOne(pt => pt.User)
                .WithMany(t => t.CategoryList)
                .HasForeignKey(pt => pt.UserId);

        }
    }
}
